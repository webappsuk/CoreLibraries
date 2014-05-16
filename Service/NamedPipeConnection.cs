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
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using ProtoBuf;
using WebApplications.Utilities.Logging;
using WebApplications.Utilities.Service.PipeProtocol;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.Service
{
    /// <summary>
    /// Creates a server for a bi-directional pipe.
    /// </summary>
    public partial class NamedPipeServer
    {
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
            private OverlappingPipeServer _stream;

            private Guid _connectionGuid = Guid.Empty;

            private string _connectionDescription = "Unknown";

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
            /// Sends the specified message.
            /// </summary>
            /// <param name="message">The message.</param>
            /// <returns><see langword="true" /> if succeeded, <see langword="false" /> otherwise.</returns>
            [NotNull]
            public Task<bool> Send([NotNull]Message message, CancellationToken token = default(CancellationToken))
            {
                return _state != PipeState.Connected ? TaskResult.False : Send(message.Serialize(), token);
            }

            /// <summary>
            /// Sends the specified message.
            /// </summary>
            /// <param name="data">The data.</param>
            /// <param name="token">The token.</param>
            /// <returns><see langword="true" /> if succeeded, <see langword="false" /> otherwise.</returns>
            [NotNull]
            public async Task<bool> Send([NotNull] byte[] data, CancellationToken token = default(CancellationToken))
            {
                    if (_state != PipeState.Connected) return false;
                    OverlappingPipeServer stream = _stream;
                    if (stream == null) return false;
                    await stream.WriteAsync(data, token);
                    return true;
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
                        // The disconnect GUID can be set by a disconnect request if the client requests it.
                        Guid disconnectGuid = Guid.Empty;

                        // Create pipe access rule to allow everyone to connect - may want to change this later
                        try
                        {
                            // Create pipe stream.
                            using (OverlappingPipeServer stream = new OverlappingPipeServer(
                                _server.Name,
                                _server.MaximumConnections,
                                PipeTransmissionMode.Message, 
                                InBufferSize,
                                OutBufferSize,
                                _server._pipeSecurity))
                            {
                                _state = PipeState.Open;

                                // Wait for connection
                                await stream.Connect(token);

                                if (!token.IsCancellationRequested)
                                {
                                    // Connect this connection to the service.
                                    _connectionGuid = _server.Service.Connect(this);

                                    if (_connectionGuid != Guid.Empty)
                                    {
                                        ConnectRequest connectRequest = null;

                                        // Set the stream.
                                        _stream = stream;

                                        _server.Add();
                                        _state = PipeState.AwaitingConnect;

                                        // Keep going as long as we're connected.
                                        while (stream.IsConnected &&
                                               !token.IsCancellationRequested &&
                                               _server.Service.State != ServiceState.Shutdown &&
                                               _connectionGuid != Guid.Empty)
                                        {
                                            // Read data in.
                                            byte[] data = await stream.ReadAsync(token);
                                            if (data == null ||
                                                token.IsCancellationRequested ||
                                                _server.Service.State == ServiceState.Shutdown ||
                                                _connectionGuid == Guid.Empty)
                                                break;

                                            // Deserialize the incoming message.
                                            Message message = Message.Deserialize(data);

                                            Request request = message as Request;

                                            // We only accept requests, anything else is a protocol error and so we must disconnect.
                                            if (request == null)
                                                break;

                                            if (connectRequest == null)
                                            {
                                                // We require a connect request to start
                                                connectRequest = request as ConnectRequest;
                                                if (connectRequest == null)
                                                    break;

                                                _state = PipeState.Connected;
                                                _connectionDescription = connectRequest.Description;
                                                Log.Add(
                                                    LoggingLevel.Notification,
                                                    () => ServiceResources.Not_NamedPipeConnection_Connection,
                                                    _connectionDescription);
                                                
                                                await
                                                    Send(
                                                        new ConnectResponse(request.ID, _server.Service.ServiceName),
                                                        token);
                                                continue;
                                            }

                                            DisconnectRequest disconnectRequest = request as DisconnectRequest;
                                            if (disconnectRequest != null)
                                            {
                                                // Set the guid for disconnect.
                                                disconnectGuid = disconnectRequest.ID;
                                                break;
                                            }

                                            CommandRequest commandRequest = request as CommandRequest;
                                            if (commandRequest == null)
                                                continue;

                                            CommandResponse response;
                                            using (StringWriter resultWriter = new StringWriter())
                                            {
                                                _server.Service.Execute(
                                                    _connectionGuid,
                                                    commandRequest.CommandLine,
                                                    resultWriter);
                                                response = new CommandResponse(
                                                    request.ID,
                                                    resultWriter.ToString()); // TODO Use custom writer.
                                            }

                                            await Send(response, token);

                                            // Reset the stream for write.
                                        }

                                        if (stream.IsConnected)
                                            await Send(new DisconnectResponse(disconnectGuid), token);
                                    }
                                }

                                // Remove the stream.
                                _stream = null;
                                _state = PipeState.Closed;
                            }
                        }
                        catch (TaskCanceledException)
                        {
                            // Don't log cancellation.
                        }
                        catch (IOException ioe)
                        {
                            // Common exception caused by sudden disconnect, lower level
                            Log.Add(
                                ioe,
                                LoggingLevel.Information,
                                () => ServiceResources.Err_NamedPipeConnection_Failed);
                        }
                        catch (Exception exception)
                        {
                            Log.Add(
                                exception,
                                LoggingLevel.Error,
                                () => ServiceResources.Err_NamedPipeConnection_Failed);
                        }
                        finally
                        {
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
            /// Called when the server disconnects.
            /// </summary>
            public void OnDisconnect()
            {
                CancellationTokenSource cts = _cancellationTokenSource;
                if (cts != null)
                    cts.Cancel();
            }

            /// <summary>
            /// Returns a <see cref="System.String" /> that represents this instance.
            /// </summary>
            /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
            public override string ToString()
            {
                return State.ToString();
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                _state = PipeState.Closed;
                // This is a safe race condition, we can tell the service we're disconnected as many times as we want.
                Guid cg = _connectionGuid;
                if (cg != Guid.Empty)
                {
                    _connectionGuid = Guid.Empty;
                    _server.Service.Disconnect(cg);
                    Log.Add(
                        LoggingLevel.Notification,
                        () => ServiceResources.Not_NamedPipeConnection_Disconnected,
                        _connectionDescription);
                }

                CancellationTokenSource cts = Interlocked.Exchange(ref _cancellationTokenSource, null);
                if (cts != null)
                {
                    cts.Cancel();
                    cts.Dispose();
                }

                _serverTask = null;
                NamedPipeServer server = Interlocked.Exchange(ref _server, null);
                if (server != null)
                    server.Remove(this);
            }
        }
    }
}