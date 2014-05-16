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

        private PipeState _state = PipeState.Starting;

        private OverlappingPipeClient _stream;

        private CancellationTokenSource _cancellationTokenSource;

        private Task _clientTask;

        private string _serviceName;

        /// <summary>
        /// Gets the name of the service (whilst connected).
        /// </summary>
        /// <value>The name of the service.</value>
        [CanBeNull]
        public string ServiceName { get { return _serviceName; } }

        /// <summary>
        /// The connection completion source indicates connection has occured.
        /// </summary>
        private TaskCompletionSource<NamedPipeClient> _connectionCompletionSource = new TaskCompletionSource<NamedPipeClient>();

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
                    case TaskStatus.WaitingForActivation:
                        return _state;
                    case TaskStatus.Created:
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
        /// <param name="description">The client description.</param>
        /// <param name="server">The server.</param>
        /// <param name="onReceive">The action to call on receipt of a message.</param>
        /// <param name="token">The token.</param>
        private NamedPipeClient(string description, [NotNull] NamedPipeServerInfo server, [NotNull]Action<Message> onReceive, CancellationToken token = default(CancellationToken))
        {
            Contract.Requires(server != null);
            _server = server;
            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken disposeToken = _cancellationTokenSource.Token;
            _clientTask = Task.Run(
                async () =>
                {
                    try
                    {
                        using (OverlappingPipeClient stream = new OverlappingPipeClient(_server.Host, _server.FullName, PipeTransmissionMode.Message))
                        {
                            _state = PipeState.Open;

                            token = token.CanBeCanceled
                                ? CancellationTokenSource.CreateLinkedTokenSource(token, disposeToken).Token
                                : disposeToken;

                            // We need to support cancelling the connect.
                            await stream.Connect(token);

                            DisconnectResponse disconnectResponse = null;

                            if (!token.IsCancellationRequested)
                            {
                                // Set the stream.
                                _stream = stream;
                                _state = PipeState.AwaitingConnect;

                                // Kick off a connect request, but don't wait for it's result as we're the task that will receive it!
                                ConnectRequest connectRequest = new ConnectRequest(description);
                                await stream.WriteAsync(connectRequest.Serialize(), token);

                                ConnectResponse connectResponse = null;

                                // Keep going as long as we're connected.
                                while (stream.IsConnected &&
                                       !disposeToken.IsCancellationRequested)
                                {
                                    // Read data in.
                                    byte[] data = await stream.ReadAsync(disposeToken);

                                    // Deserialize the incoming message.
                                    Message message = Message.Deserialize(data);

                                    if (connectResponse == null)
                                    {
                                        // We require a connect response to start
                                        connectResponse = message as ConnectResponse;
                                        if (connectResponse == null ||
                                            connectResponse.ID != connectRequest.ID)
                                            break;

                                        _state = PipeState.Connected;
                                        _serviceName = connectResponse.ServiceName;

                                        Log.Add(
                                            LoggingLevel.Notification,
                                            () => "TODO ClientResources.Not_NamedPipeConnection_Connection",
                                            connectResponse.ServiceName);

                                        TaskCompletionSource<NamedPipeClient> ccs = Interlocked.Exchange(ref _connectionCompletionSource, null);
                                        if (ccs != null)
                                            ccs.TrySetResult(this);

                                        // Observer the message.
                                        onReceive(message);
                                        continue;
                                    }

                                    // Check for disconnect, we don't observe the message until the disconnect is complete.
                                    disconnectResponse = message as DisconnectResponse;
                                    if (disconnectResponse != null)
                                        break;

                                    // Observe the message.
                                    onReceive(message);

                                    Response response = message as Response;
                                    if (response != null)
                                    {
                                        TaskCompletionSource<Message> tcs;
                                        if (_commandRequests.TryRemove(response.ID, out tcs))
                                            tcs.TrySetResult(message);

                                        continue;
                                    }
                                }
                            }

                            // Remove the stream.
                            _stream = null;
                            _state = PipeState.Closed;
                            _serviceName = null;

                            // If we had a disconnect message observe it now that the disconnect has been actioned,
                            // this prevents the receiver thinking the connection is still active.
                            if (disconnectResponse != null)
                            {
                                onReceive(disconnectResponse);
                                TaskCompletionSource<Message> tcs;
                                if (_commandRequests.TryRemove(disconnectResponse.ID, out tcs))
                                    tcs.TrySetResult(disconnectResponse);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        _stream = null;
                        _state = PipeState.Closed;
                        _serviceName = null;
                        TaskCanceledException tce = exception as TaskCanceledException;

                        TaskCompletionSource<NamedPipeClient> ccs = Interlocked.Exchange(ref _connectionCompletionSource, null);
                        if (ccs != null)
                        {
                            if (tce != null)
                                ccs.TrySetCanceled();
                            else
                                ccs.TrySetException(exception);
                        }

                        // We only log if this wasn't a cancellation exception.
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
                }, disposeToken);
        }

        /// <summary>
        /// Connects to the specified pipe.
        /// </summary>
        /// <param name="description">The client description.</param>
        /// <param name="pipe">The pipe.</param>
        /// <param name="onReceive">The action to call on receipt of a message.</param>
        /// <param name="token">The token.</param>
        /// <returns>A new <see cref="NamedPipeClient" /> that is connected to the given pipe.</returns>
        [CanBeNull]
        [PublicAPI]
        public static Task<NamedPipeClient> Connect([NotNull]string description, [CanBeNull] string pipe, [NotNull]Action<Message> onReceive, CancellationToken token = default(CancellationToken))
        {
            NamedPipeServerInfo server = FindService(pipe);
            if (server == null ||
                !server.IsValid)
                return null;

            return Connect(description, server, onReceive, token);
        }

        /// <summary>
        /// Connects to the specified pipe server.
        /// </summary>
        /// <param name="description">The client description.</param>
        /// <param name="server">The server.</param>
        /// <param name="onReceive">The action to call on receipt of a message.</param>
        /// <param name="token">The token.</param>
        /// <returns>A new <see cref="NamedPipeClient" /> that is connected to the given pipe.</returns>
        [CanBeNull]
        [PublicAPI]
        public static Task<NamedPipeClient> Connect([NotNull]string description, [CanBeNull] NamedPipeServerInfo server, [NotNull]Action<Message> onReceive, CancellationToken token = default(CancellationToken))
        {
            if (server == null ||
                !server.IsValid)
                return null;

            NamedPipeClient npc = new NamedPipeClient(description, server, onReceive, token);
            TaskCompletionSource<NamedPipeClient> ccs = npc._connectionCompletionSource;
            return (ccs != null ? ccs.Task : Task.FromResult(npc));
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
            if (_state != PipeState.Connected) return null;
            OverlappingPipeClient stream = _stream;
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

            await stream.WriteAsync(request.Serialize(), token);

            return await tcs.Task;
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
                State != PipeState.Connected ||
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
            if (_clientTask == null ||
                State != PipeState.Connected)
                return TaskResult<string>.Default;

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

            TaskCompletionSource<NamedPipeClient> ccs = Interlocked.Exchange(ref _connectionCompletionSource, null);
            if (ccs != null)
                ccs.TrySetCanceled();

            _clientTask = null;
            _state = PipeState.Closed;
            _serviceName = null;

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
        public static IEnumerable<NamedPipeServerInfo> GetServices()
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
        /// <param name="serviceName">Name (or pipe) of the server.</param>
        /// <returns>The <see cref="NamedPipeServerInfo"/> if found; otherwise <see langword="null"/>.</returns>
        [CanBeNull]
        public static NamedPipeServerInfo FindService([CanBeNull] string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
                return null;

            return
                GetServices()
                    .FirstOrDefault(
                        n => string.Equals(serviceName, n.Name, StringComparison.CurrentCultureIgnoreCase) ||
                             string.Equals(serviceName, n.Pipe, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}