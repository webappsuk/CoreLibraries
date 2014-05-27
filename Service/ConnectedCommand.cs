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
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WebApplications.Utilities.Formatting;
using WebApplications.Utilities.Service.Common;
using WebApplications.Utilities.Service.Common.Protocol;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.Service
{
    /// <summary>
    /// Creates a server for a bi-directional pipe.
    /// </summary>
    internal partial class NamedPipeServer
    {
        /// <summary>
        /// Holds a single executing command.
        /// </summary>
        private class ConnectedCommand : TextWriter, IColoredTextWriter
        {
            /// <summary>
            /// The connection unique identifier.
            /// </summary>
            public readonly Guid ConnectionGuid;

            private NamedPipeConnection _connection;

            [NotNull]
            private readonly CommandRequest _request;

            private CancellationTokenSource _cancellationTokenSource;

            /// <summary>
            /// The sequence identifier.
            /// </summary>
            private int _sequenceId;

            /// <summary>
            /// The write buffer.
            /// </summary>
            [NotNull]
            private readonly StringBuilder _builder = new StringBuilder();

            /// <summary>
            /// The escape chars
            /// </summary>
            [NotNull]
            private readonly HashSet<char> _escapeChars = new HashSet<char>(new[] { '{', '}', '\\' });

            /// <summary>
            /// The request identifier.
            /// </summary>
            public readonly Guid ID;

            /// <summary>
            /// The cancel request, if being cancelled as a result of a request.
            /// </summary>
            private CommandCancelRequest _cancelRequest;

            /// <summary>
            /// Initializes a new instance of the <see cref="ConnectedCommand" /> class.
            /// </summary>
            /// <param name="connectionGuid">The connection unique identifier.</param>
            /// <param name="service">The service.</param>
            /// <param name="connection">The connection.</param>
            /// <param name="request">The request.</param>
            /// <param name="token">The cancellation token.</param>
            public ConnectedCommand(
                Guid connectionGuid,
                [NotNull] BaseService service,
                [NotNull] NamedPipeConnection connection,
                [NotNull] CommandRequest request,
                CancellationToken token = default (CancellationToken))
            {
                ConnectionGuid = connectionGuid;
                _connection = connection;
                _request = request;
                _cancellationTokenSource = new CancellationTokenSource();
                ID = _request.ID;

                token = token.CreateLinked(_cancellationTokenSource.Token);

                Task.Run(
                    async () =>
                    {
                        try
                        {
                            do
                            {
                                // ReSharper disable once PossibleNullReferenceException
                                await Task.Delay(250, token).ConfigureAwait(false);
                                if (token.IsCancellationRequested) return;
                                await Flush(0, token).ConfigureAwait(false);
                            } while (true);
                        }
                        catch (TaskCanceledException)
                        {
                        }
                    },
                    token);
                // Kick of task to run command.
                Task.Run(
                    async () =>
                    {
                        Exception exception = null;
                        bool cancelled = false;
                        try
                        {
                            await service.ExecuteAsync(ConnectionGuid, _request.CommandLine, this, token).ConfigureAwait(false);
                            if (!token.IsCancellationRequested)
                                await Flush(-1, token).ConfigureAwait(false);
                            else
                                cancelled = true;
                        }
                        catch (OperationCanceledException)
                        {
                            cancelled = true;
                        }
                        catch (Exception e)
                        {
                            exception = e;
                        }

                        if (cancelled)
                        {
                            using (await _flushLock.LockAsync(token).ConfigureAwait(false))
                            {
                                try
                                {
                                    await
                                        connection.Send(
                                            new CommandCancelResponse(
                                                _cancelRequest != null ? _cancelRequest.ID : Guid.Empty,
                                                ID),
                                            Constants.FireAndForgetToken)
                                            .ConfigureAwait(false);
                                }
                                catch (OperationCanceledException) { }
                                return;
                            }
                        }

                        if (exception != null)
                            try
                            {
                                await Flush(0, token).ConfigureAwait(false);
                                _builder.Append(exception.Message);
                                await Flush(-2, token).ConfigureAwait(false);
                            }
                            // ReSharper disable once EmptyGeneralCatchClause
                            catch
                            {
                            }

                        Dispose(true);
                    },
                    token);
            }

            /// <summary>
            /// Returns a <see cref="System.String" /> that represents this instance.
            /// </summary>
            /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
            public override string ToString()
            {
                Contract.Assert(_request.CommandLine != null);
                return _request.CommandLine;
            }

            /// <summary>
            /// Cancels the specified request.
            /// </summary>
            /// <param name="request">The request.</param>
            public void Cancel([NotNull] CommandCancelRequest request)
            {
                Contract.Requires<RequiredContractException>(request != null, "Parameter_Null");
                _cancelRequest = request;
                CancellationTokenSource cts = Interlocked.Exchange(ref _cancellationTokenSource, null);
                if (cts != null)
                    cts.Cancel();
            }

            /// <summary>
            /// Releases the unmanaged resources used by the <see cref="T:System.IO.TextWriter" /> and optionally releases the managed resources.
            /// </summary>
            /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
            protected override void Dispose(bool disposing)
            {
                CancellationTokenSource cts = Interlocked.Exchange(ref _cancellationTokenSource, null);
                if (cts != null)
                    cts.Cancel();

                NamedPipeConnection connection = Interlocked.Exchange(ref _connection, null);
                if (connection != null)
                    connection.Remove(this);

                base.Dispose(disposing);
            }

            /// <summary>
            /// The flush lock.
            /// </summary>
            [NotNull]
            private readonly AsyncLock _flushLock = new AsyncLock();

            /// <summary>
            /// Flushes the specified final.
            /// </summary>
            /// <param name="sequence">The sequence.</param>
            /// <param name="token">The token.</param>
            /// <returns>Task.</returns>
            [NotNull]
            private async Task Flush(int sequence, CancellationToken token = default(CancellationToken))
            {
                using (await _flushLock.LockAsync(token).ConfigureAwait(false))
                {
                    string chunk;
                    bool increment = false;
                    lock (_builder)
                    {
                        chunk = _builder.ToString();
                        if ((sequence > -1) &&
                            string.IsNullOrEmpty(chunk))
                            return;
                        _builder.Clear();

                        if (_sequenceId < 0) return;
                        if (sequence < 0) _sequenceId = sequence;
                        else increment = true;
                    }
                    NamedPipeConnection connection = _connection;
                    if (connection != null)
                        await connection.Send(new CommandResponse(ID, _sequenceId, chunk), token).ConfigureAwait(false);
                    if (increment)
                        lock (_builder)
                            _sequenceId++;
                }
            }

            /// <summary>
            /// Clears all buffers for the current writer and causes any buffered data to be written to the underlying device.
            /// </summary>
            public override void Flush()
            {
                Flush(0).Wait();
            }

            /// <summary>
            /// Writes a character to the text string or stream.
            /// </summary>
            /// <param name="value">The character to write to the text stream.</param>
            public override void Write(char value)
            {
                lock (_builder)
                {
                    if (_escapeChars.Contains(value))
                        _builder.Append('\\');
                    _builder.Append(value);
                }
            }

            /// <summary>
            /// Writes a character array to the text string or stream.
            /// </summary>
            /// <param name="buffer">The character array to write to the text stream.</param>
            public override void Write([NotNull] char[] buffer)
            {
                lock (_builder)
                {
                    foreach (char c in buffer)
                    {
                        if (_escapeChars.Contains(c))
                            _builder.Append('\\');
                        _builder.Append(c);
                    }
                }
            }

            /// <summary>
            /// Writes a string to the text string or stream.
            /// </summary>
            /// <param name="value">The string to write.</param>
            public override void Write([NotNull] string value)
            {
                lock (_builder)
                {
                    foreach (char c in value)
                    {
                        if (_escapeChars.Contains(c))
                            _builder.Append('\\');
                        _builder.Append(c);
                    }
                }
            }

            /// <summary>
            /// Writes the line.
            /// </summary>
            public override void WriteLine()
            {
                lock (_builder)
                    _builder.AppendLine();
            }

            /// <summary>
            /// Writes the line.
            /// </summary>
            /// <param name="value">The value.</param>
            public override void WriteLine(char value)
            {
                lock (_builder)
                {
                    _builder.Append(value);
                    _builder.AppendLine();
                }
            }

            /// <summary>
            /// Writes an array of characters followed by a line terminator to the text string or stream.
            /// </summary>
            /// <param name="buffer">The character array from which data is read.</param>
            public override void WriteLine([NotNull] char[] buffer)
            {
                lock (_builder)
                {
                    foreach (char c in buffer)
                    {
                        if (_escapeChars.Contains(c))
                            _builder.Append('\\');
                        _builder.Append(c);
                    }
                    _builder.AppendLine();
                }
            }

            /// <summary>
            /// Writes a string followed by a line terminator to the text string or stream.
            /// </summary>
            /// <param name="value">The string to write. If <paramref name="value" /> is null, only the line terminator is written.</param>
            public override void WriteLine([NotNull] string value)
            {
                lock (_builder)
                {
                    foreach (char c in value)
                    {
                        if (_escapeChars.Contains(c))
                            _builder.Append('\\');
                        _builder.Append(c);
                    }
                    _builder.AppendLine();
                }
            }

            /// <summary>
            /// When overridden in a derived class, returns the character encoding in which the output is written.
            /// </summary>
            /// <value>The encoding.</value>
            /// <returns>The character encoding in which the output is written.</returns>
            public override Encoding Encoding
            {
                get { return Encoding.Unicode; }
            }

            /// <summary>
            /// Resets the foreground and background colors of the writer.
            /// </summary>
            public void ResetColors()
            {
                lock (_builder)
                    _builder.Append("{" + FormatBuilder.ResetColorsTag + "}");
            }

            /// <summary>
            /// Resets the foreground color of the writer.
            /// </summary>
            public void ResetForegroundColor()
            {
                lock (_builder)
                    _builder.AppendFormat("{{{0}}}", FormatBuilder.ForegroundColorTag);
            }

            /// <summary>
            /// Sets the foreground color of the writer.
            /// </summary>
            /// <param name="color">The color.</param>
            public void SetForegroundColor(Color color)
            {
                lock (_builder)
                    _builder.AppendFormat("{{{0}:{1}}}", FormatBuilder.ForegroundColorTag, ColorHelper.GetName(color));
            }

            /// <summary>
            /// Sets the background color of the writer.
            /// </summary>
            public void ResetBackgroundColor()
            {
                lock (_builder)
                    _builder.AppendFormat("{{{0}}}", FormatBuilder.BackgroundColorTag);
            }

            /// <summary>
            /// Sets the background color of the writer.
            /// </summary>
            /// <param name="color">The color.</param>
            public void SetBackgroundColor(Color color)
            {
                lock (_builder)
                    _builder.AppendFormat("{{{0}:{1}}}", FormatBuilder.BackgroundColorTag, ColorHelper.GetName(color));
            }
        }
    }
}