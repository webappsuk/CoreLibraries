using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WebApplications.Utilities.Formatting;
using WebApplications.Utilities.Service.PipeProtocol;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.Service
{
    /// <summary>
    /// Creates a server for a bi-directional pipe.
    /// </summary>
    public partial class NamedPipeServer
    {
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
            private int _sequenceId = 0;

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

            private Task _flushTask;

            /// <summary>
            /// Initializes a new instance of the <see cref="ConnectedCommand" /> class.
            /// </summary>
            /// <param name="connectionGuid">The connection unique identifier.</param>
            /// <param name="service">The service.</param>
            /// <param name="connection">The connection.</param>
            /// <param name="request">The request.</param>
            public ConnectedCommand(Guid connectionGuid, [NotNull]BaseService service, [NotNull]NamedPipeConnection connection, [NotNull]CommandRequest request, CancellationToken token = default (CancellationToken))
            {
                ConnectionGuid = connectionGuid;
                _connection = connection;
                _request = request;
                _cancellationTokenSource = new CancellationTokenSource();
                var flushToken = token.CanBeCanceled
                    ? CancellationTokenSource.CreateLinkedTokenSource(token, _cancellationTokenSource.Token).Token
                    : _cancellationTokenSource.Token;

                _flushTask = Task.Run(
                    async () =>
                    {
                        do
                        {
                            await Task.Delay(250, token);
                            if (flushToken.IsCancellationRequested) return;
                            await Flush(0, flushToken);
                        } while (true);
                    },
                    flushToken);
                // Kick of task to run command.
                Task.Run(
                    async () =>
                    {
                        Exception exception = null;
                        try
                        {
                            service.Execute(ConnectionGuid, _request.CommandLine, this);
                            token.ThrowIfCancellationRequested();
                            await Flush(-1, token);
                        }
                        catch (Exception e)
                        {
                            if (!(e is TaskCanceledException))
                                exception = e;
                        }
                        if (exception != null)
                        {
                            try
                            {
                                await Flush(0, token);
                                _builder.Append(exception.Message);
                                await Flush(-2, token);
                            }
                            catch { }
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
                return _request.CommandLine;
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

                NamedPipeConnection connexion = Interlocked.Exchange(ref _connection, null);
                if (connexion != null)
                    connexion.Remove(this);

                base.Dispose(disposing);
            }

            [NotNull]
            private readonly AsyncLock _flushLock = new AsyncLock();

            /// <summary>
            /// Flushes the specified final.
            /// </summary>
            /// <param name="sequence">The sequence.</param>
            /// <param name="token">The token.</param>
            /// <returns>Task.</returns>
            private async Task Flush(int sequence, CancellationToken token = default(CancellationToken))
            {
                using (await _flushLock.LockAsync(token))
                {
                    string chunk;
                    lock (_builder)
                    {
                        chunk = _builder.ToString();
                        if ((sequence > -1) &&
                            string.IsNullOrEmpty(chunk))
                            return;
                        _builder.Clear();

                        if (_sequenceId < 0) return;
                        if (sequence < 0) _sequenceId = sequence;
                    }
                    Guid id = _request.ID;
                    NamedPipeConnection connection = _connection;
                    if (connection != null)
                        await connection.Send(new CommandResponse(id, _sequenceId, chunk), token);
                }
            }

            /// <summary>
            /// Closes the current writer and releases any system resources associated with the writer.
            /// </summary>
            public void Close()
            {
                base.Close();
            }

            /// <summary>
            /// Clears all buffers for the current writer and causes any buffered data to be written to the underlying device.
            /// </summary>
            public void Flush()
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
            public override void Write(char[] buffer)
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
            public override void Write(string value)
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
            public override void WriteLine(char[] buffer)
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
            public void WriteLine(string value)
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
            public override Encoding Encoding { get { return Encoding.Unicode; } }

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
