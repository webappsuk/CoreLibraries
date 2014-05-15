#region © Copyright Web Applications (UK) Ltd, 2014.  All rights reserved.
// Copyright (c) 2014, Web Applications UK Ltd
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Web Applications UK Ltd nor the
//       names of its contributors may be used to endorse or promote products
//       derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL WEB APPLICATIONS UK LTD BE LIABLE FOR ANY
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using ProtoBuf;
using WebApplications.Utilities.Caching;
using WebApplications.Utilities.Logging;
using WebApplications.Utilities.Service.PipeProtocol;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.Service.Client
{
    public class NamedPipeClient : IDisposable
    {
        /// <summary>
        /// The input buffer size
        /// </summary>
        [PublicAPI]
        public const int InBufferSize = 32768;

        /// <summary>
        /// The output buffer size
        /// </summary>
        [PublicAPI]
        public const int OutBufferSize = 16384;

        [NotNull]
        private readonly NamedPipeServerInfo _server;

        [NotNull]
        private readonly AsyncLock _writeLock = new AsyncLock();

        private PipeState _state = PipeState.Starting;

        private NamedPipeClientStream _stream;

        private CancellationTokenSource _cancellationTokenSource;

        private Task _clientTask;

        [NotNull]
        private readonly ConcurrentDictionary<Guid, TaskCompletionSource<Message>> _commandRequests = new ConcurrentDictionary<Guid, TaskCompletionSource<Message>>(); 

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>The state.</value>
        public PipeState State
        {
            get
            {
                Task ctask = _clientTask;
                if (ctask == null) return PipeState.Closed;
                switch (ctask.Status)
                {
                    case TaskStatus.Running:
                        return _state;
                    case TaskStatus.Created:
                    case TaskStatus.WaitingForActivation:
                    case TaskStatus.WaitingToRun:
                        return PipeState.Starting;
                    default:
                        return PipeState.Closed;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedPipeClient" /> class.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="onReceive">The action to call on receipt of a message.</param>
        /// <param name="connectionTimeout">The connection timeout (defaults to 5s).</param>
        private NamedPipeClient([NotNull] NamedPipeServerInfo server, [NotNull]Action<Message> onReceive, TimeSpan connectionTimeout = default(TimeSpan))
        {
            Contract.Requires(server != null);
            _server = server;
            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = _cancellationTokenSource.Token;
            if (connectionTimeout <= TimeSpan.Zero)
                connectionTimeout = TimeSpan.FromSeconds(5);
            _clientTask = Task.Run(
                async () =>
                {
                    try
                    {
                        byte[] buffer = new byte[InBufferSize];
                        using (MemoryStream readerStream = new MemoryStream(InBufferSize))
                        using (NamedPipeClientStream stream = new NamedPipeClientStream(
                            _server.Host,
                            _server.FullName,
                            PipeDirection.InOut,
                            PipeOptions.Asynchronous))
                        {
                            _state = PipeState.Open;

                            stream.Connect((int)Math.Ceiling(connectionTimeout.TotalMilliseconds));
                            stream.ReadMode = PipeTransmissionMode.Message;

                            if (!token.IsCancellationRequested)
                            {
                                // Set the stream.
                                _stream = stream;
                                _state = PipeState.AwaitingConnect;

                                // Kick off a connect request, but don't wait for it's result as we're the task that will receive it!
                                ConnectRequest connectRequest = new ConnectRequest("TODO Description");
                                using (await _writeLock.LockAsync(token))
                                    Serializer.Serialize(stream, connectRequest);

                                ConnectResponse connectResponse = null;
                                // Keep going as long as we're connected.
                                while (stream.IsConnected &&
                                       !token.IsCancellationRequested)
                                {
                                    // Read data in.
                                    int read = await stream.ReadAsync(buffer, 0, InBufferSize, token);
                                    if (read < 1 ||
                                        token.IsCancellationRequested)
                                        break;

                                    // Write data to reader stream (no point in doing this async).
                                    readerStream.Write(buffer, 0, read);

                                    if (!stream.IsMessageComplete) continue;

                                    // Deserialize the incoming message.
                                    readerStream.Seek(0, SeekOrigin.Begin);
                                    Message message = Serializer.Deserialize<Message>(readerStream);
                                    readerStream.Seek(0, SeekOrigin.Begin);
                                    readerStream.SetLength(0);

                                    if (connectResponse == null)
                                    {
                                        // We require a connect response to start
                                        connectResponse = message as ConnectResponse;
                                        if (connectResponse == null ||
                                            connectResponse.ID != connectRequest.ID)
                                            break;

                                        _state = PipeState.Connected;

                                        Log.Add(
                                            LoggingLevel.Notification,
                                            () => "TODO ClientResources.Not_NamedPipeConnection_Connection",
                                            connectResponse.ServiceName);

                                        // Observer the message.
                                        onReceive(message);
                                        continue;
                                    }

                                    // Observer the message.
                                    onReceive(message);

                                    Response response = message as Response;
                                    if (response != null)
                                    {
                                        TaskCompletionSource<Message> tcs;
                                        if (_commandRequests.TryRemove(response.ID, out tcs))
                                            tcs.TrySetResult(message);

                                        continue;
                                    }

                                    DisconnectResponse disconnectResponse = message as DisconnectResponse;
                                    if (disconnectResponse != null) 
                                        break;
                                }
                            }

                            // Remove the stream.
                            _stream = null;
                            _state = PipeState.Closed;

                        }
                    }
                    catch (Exception exception)
                    {
                        _stream = null;
                        _state = PipeState.Closed;
                        
                        // We only log if this wasn't a cancellation exception.
                        TaskCanceledException tce = exception as TaskCanceledException;
                        if (tce == null)
                            Log.Add(
                                exception,
                                LoggingLevel.Error,
                                () => "TODO ClientResources.Err_NamedPipeConnection_Failed");
                    }
                    finally
                    {
                        Dispose();
                    }
                }, token);
        }

        /// <summary>
        /// Connects to the specified pipe.
        /// </summary>
        /// <param name="pipe">The pipe.</param>
        /// <param name="onReceive">The action to call on receipt of a message.</param>
        /// <param name="connectionTimeout">The connection timeout (defaults to 5s).</param>
        /// <returns>A new <see cref="NamedPipeClient"/> that is connected to the given pipe.</returns>
        [CanBeNull]
        [PublicAPI]
        public static NamedPipeClient Connect([CanBeNull] string pipe, [NotNull]Action<Message> onReceive, TimeSpan connectionTimeout = default(TimeSpan))
        {
            NamedPipeServerInfo server = FindServer(pipe);
            if (server == null ||
                !server.IsValid)
                return null;

            return new NamedPipeClient(server, onReceive, connectionTimeout);
        }

        /// <summary>
        /// Connects to the specified pipe server.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="onReceive">The action to call on receipt of a message.</param>
        /// <param name="connectionTimeout">The connection timeout (defaults to 5s).</param>
        /// <returns>A new <see cref="NamedPipeClient" /> that is connected to the given pipe.</returns>
        [CanBeNull]
        [PublicAPI]
        public static NamedPipeClient Connect([CanBeNull] NamedPipeServerInfo server, [NotNull]Action<Message> onReceive, TimeSpan connectionTimeout = default(TimeSpan))
        {
            if (server == null ||
                !server.IsValid)
                return null;

            return new NamedPipeClient(server, onReceive, connectionTimeout);
        }

        /// <summary>
        /// Sends the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="token">The token.</param>
        /// <returns><see langword="true" /> if succeeded, <see langword="false" /> otherwise.</returns>
        [NotNull]
        private async Task<Message> Send([NotNull] Request request, CancellationToken token = default(CancellationToken))
        {
            using (await _writeLock.LockAsync(token))
            {
                if (_state != PipeState.Connected) return null;
                NamedPipeClientStream stream = _stream;
                if (stream == null) return null;

                CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

                token = token.CanBeCanceled
                    ? CancellationTokenSource.CreateLinkedTokenSource(token, cts.Token).Token
                    : cts.Token;

                TaskCompletionSource<Message> tcs = new TaskCompletionSource<Message>();
                token.Register(
                    () =>
                    {
                        tcs.TrySetCanceled();
                        TaskCompletionSource<Message> t;
                        _commandRequests.TryRemove(request.ID, out t);
                    });

                if (!_commandRequests.TryAdd(request.ID, tcs))
                    return null;

                Serializer.Serialize(stream, request);

                return await tcs.Task;
            }
        }

        /// <summary>
        /// Executes the specified command line.
        /// </summary>
        /// <param name="commandLine">The command line.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>An awaitable task that contains the result of the execution.</returns>
        [NotNull]
        [PublicAPI]
        public Task<string> Execute([CanBeNull] string commandLine, CancellationToken token = default (CancellationToken))
        {
            if (_clientTask == null ||
                string.IsNullOrWhiteSpace(commandLine))
                return TaskResult<string>.Default;

            return Send(new CommandRequest(commandLine), token)
                .ContinueWith(
                    t =>
                    {
                        CommandResponse response = t.Result as CommandResponse;
                        return response != null ? response.Result : null;
                    },
                    token,
                    TaskContinuationOptions.OnlyOnRanToCompletion,
                    TaskScheduler.Default);
        }

        /// <summary>
        /// Disconnects from the server.
        /// </summary>
        /// <returns>Task.</returns>
        [NotNull]
        [PublicAPI]
        public Task Disconnect(CancellationToken token = default(CancellationToken))
        {
            return Send(new DisconnectRequest(), token);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            CancellationTokenSource cts = Interlocked.Exchange(ref _cancellationTokenSource, null);
            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
            }

            _clientTask = null;
            _state = PipeState.Closed;

            foreach (Guid id in _commandRequests.Keys.ToArray())
            {
                TaskCompletionSource<Message> tcs;
                if (_commandRequests.TryRemove(id, out tcs))
                    tcs.TrySetCanceled();
            }
        }

        #region Find files kernal methods
        [StructLayout(LayoutKind.Sequential)]
        private struct FILETIME
        {
            public uint dwLowDateTime;
            public uint dwHighDateTime;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct WIN32_FIND_DATA
        {
            public uint dwFileAttributes;
            public FILETIME ftCreationTime;
            public FILETIME ftLastAccessTime;
            public FILETIME ftLastWriteTime;
            public uint nFileSizeHigh;
            public uint nFileSizeLow;
            public uint dwReserved0;
            public uint dwReserved1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string cFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            public string cAlternateFileName;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern int FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);
        [DllImport("kernel32.dll")]
        private static extern bool FindClose(IntPtr hFindFile);
        #endregion

        /// <summary>
        /// Gets the server pipes.
        /// </summary>
        /// <returns>An enumeration of pipes with the correct suffix.</returns>
        [NotNull]
        public static IEnumerable<NamedPipeServerInfo> GetServers()
        {
            // Note: Directory.GetFiles() can fail if there are pipes on the system with invalid characters,
            // to be safe we use the underlying kernal methods instead.
            IntPtr invalid = new IntPtr(-1);
            IntPtr handle = IntPtr.Zero;
            try
            {
                WIN32_FIND_DATA data;
                handle = FindFirstFile(@"\\.\pipe\*", out data);
                if (handle == invalid) yield break;

                do
                {
                    NamedPipeServerInfo nps = new NamedPipeServerInfo(@"\\.\pipe\" + data.cFileName);
                    if (nps.IsValid)
                        yield return nps;
                } while (FindNextFile(handle, out data) != 0);
                FindClose(handle);
                handle = invalid;
            }
            finally
            {
                if (handle != invalid)
                    FindClose(handle);
            }
        }

        /// <summary>
        /// Finds the server that matches the name or pipe specified.
        /// </summary>
        /// <param name="serverName">Name (or pipe) of the server.</param>
        /// <returns>The <see cref="NamedPipeServerInfo"/> if found; otherwise <see langword="null"/>.</returns>
        [CanBeNull]
        public static NamedPipeServerInfo FindServer([CanBeNull] string serverName)
        {
            if (string.IsNullOrWhiteSpace(serverName))
                return null;

            return
                GetServers()
                    .FirstOrDefault(
                        n => string.Equals(serverName, n.Name, StringComparison.CurrentCultureIgnoreCase) ||
                             string.Equals(serverName, n.Pipe, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}