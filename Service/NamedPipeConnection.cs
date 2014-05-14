﻿#region © Copyright Web Applications (UK) Ltd, 2014.  All rights reserved.
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
using System.IO;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WebApplications.Utilities.Logging;

namespace WebApplications.Utilities.Service
{
    /// <summary>
    /// Creates a server for a bi-directional pipe.
    /// </summary>
    public partial class NamedPipeServer
    {
        private enum PipeState
        {
            Starting,
            Open,
            Connected,
            Closed
        }

        /// <summary>
        /// Implements a connection to the service over a named pipe.
        /// </summary>
        private class NamedPipeConnection : IConnection, IDisposable
        {
            [NotNull]
            private NamedPipeServer _server;

            private CancellationTokenSource _cancellationTokenSource;

            private Task _serverTask;

            private PipeState _state = PipeState.Starting;

            /// <summary>
            /// The stream
            /// </summary>
            private NamedPipeServerStream _stream;

            private Guid _connectionGuid = Guid.Empty;

            /// <summary>
            /// Initializes a new instance of the <see cref="NamedPipeConnection"/> class.
            /// </summary>
            /// <param name="server">The server.</param>
            public NamedPipeConnection([NotNull] NamedPipeServer server)
            {
                _server = server;
                _cancellationTokenSource = new CancellationTokenSource();
            }

            /// <summary>
            /// Starts this instance.
            /// </summary>
            public void Start()
            {
                CancellationToken token = _cancellationTokenSource.Token;
                _serverTask = Task.Run(
                    async () =>
                    {
                        // Create pipe access rule to allow everyone to connect - may want to change this later
                        try
                        {
                            byte[] buffer = new byte[InBufferSize];
                            using (MemoryStream readerStream = new MemoryStream(InBufferSize))
                                // Create pipe stream.
                            using (NamedPipeServerStream stream = new NamedPipeServerStream(
                                _server.Name,
                                PipeDirection.InOut,
                                _server.MaximumConnections,
                                PipeTransmissionMode.Message,
                                PipeOptions.Asynchronous,
                                InBufferSize,
                                OutBufferSize,
                                _server._pipeSecurity))
                            {
                                _state = PipeState.Open;

                                // Wait for connection
                                await Task.Factory.FromAsync(
                                    stream.BeginWaitForConnection,
                                    stream.EndWaitForConnection,
                                    null).WithCancellation(token);

                                if (!token.IsCancellationRequested)
                                {
                                    // Connect this connection to the service.
                                    _connectionGuid = _server.Service.Connect(this);

                                    if (_connectionGuid != Guid.Empty)
                                    {
                                        // Set the stream.
                                        _stream = stream;

                                        // Keep going as long as we're connected.
                                        while (stream.IsConnected ||
                                               token.IsCancellationRequested ||
                                               _server.Service.State == ServiceState.Shutdown ||
                                               _connectionGuid == Guid.Empty)
                                        {
                                            _server.Add();
                                            _state = PipeState.Connected;

                                            // Read data in.
                                            int read = await _stream.ReadAsync(buffer, 0, InBufferSize, token);
                                            if (read < 1 ||
                                                token.IsCancellationRequested ||
                                                _server.Service.State == ServiceState.Shutdown ||
                                                _connectionGuid == Guid.Empty)
                                                break;

                                            // Write data to reader stream (no point in doing this async).
                                            readerStream.Write(buffer, 0, read);

                                            if (!_stream.IsMessageComplete) continue;

                                            // Reset the stream for read.
                                            readerStream.Seek(0, SeekOrigin.Begin);

                                            // TODO We need to deserialized our message now.

                                            // TODO Send to the associated service

                                            // Reset the stream for write.
                                            readerStream.Seek(0, SeekOrigin.Begin);
                                        }

                                        // Tell ther server we're disconnected.
                                        _server.Service.Disconnect(_connectionGuid);
                                    }
                                }

                                // Remove the stream.
                                _stream = null;
                                _state = PipeState.Closed;
                            }
                        }
                        catch (Exception exception)
                        {
                            // This is a safe race condition, we can tell the service we're disconnected as many times as we want.
                            Guid cg = _connectionGuid;
                            if (cg != Guid.Empty)
                            {
                                _connectionGuid = Guid.Empty;
                                _server.Service.Disconnect(cg);
                            }

                            _stream = null;
                            _state = PipeState.Closed;

                            Log.Add(exception, LoggingLevel.Error, () => ServiceResources.Err_NamedPipeConnection_Failed);
                        }
                        finally
                        {
                            _server.Remove(this);
                            Dispose();
                        }
                    },
                    token);
            }

            /// <summary>
            /// Gets the state.
            /// </summary>
            /// <value>The state.</value>
            public PipeState State
            {
                get
                {
                    Task stask = _serverTask;
                    if (stask == null) return PipeState.Closed;
                    switch (stask.Status)
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
            /// Called when the server disconnects.
            /// </summary>
            public void OnDisconnect()
            {
                CancellationTokenSource cts = _cancellationTokenSource;
                if (cts != null)
                    cts.Cancel();
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                // This is a safe race condition, we can tell the service we're disconnected as many times as we want.
                Guid cg = _connectionGuid;
                if (cg != Guid.Empty)
                {
                    _connectionGuid = Guid.Empty;
                    _server.Service.Disconnect(cg);
                }

                CancellationTokenSource cts = Interlocked.Exchange(ref _cancellationTokenSource, null);
                if (cts != null)
                {
                    cts.Cancel();
                    cts.Dispose();
                }

                Task stask = Interlocked.Exchange(ref _serverTask, null);
                if (stask != null)
                    stask.Dispose();

                var server = Interlocked.Exchange(ref _server, null);
                if (server != null)
                    server.Remove(this);
            }
        }
    }
}