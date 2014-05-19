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
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WebApplications.Utilities.Formatting;
using WebApplications.Utilities.Logging;

namespace WebApplications.Utilities.Service
{
    /// <summary>
    /// Implements a console-based service connection, if running in a console.
    /// </summary>
    public class ConsoleConnection : IDisposable, IConnection
    {
        [NotNull]
        private static readonly FormatBuilder _promptBuilder = new FormatBuilder()
            .AppendForegroundColor(ConsoleColor.Cyan)
            .AppendFormat("[{Time:hh:mm:ss.ffff}] ")
            .AppendForegroundColor(ConsoleColor.Yellow)
            .AppendFormat("{State}")
            .AppendResetForegroundColor()
            .Append(" > ")
            .MakeReadOnly();

        /// <summary>
        /// The task completion source
        /// </summary>
        [NotNull]
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// The default log format.
        /// </summary>
        [CanBeNull]
        private readonly FormatBuilder _defaultLogFormat;

        /// <summary>
        /// The _default logging levels
        /// </summary>
        private readonly LoggingLevels _defaultLoggingLevels;

        /// <summary>
        /// Prevents a default instance of the <see cref="ConsoleConnection"/> class from being created.
        /// </summary>
        // ReSharper disable once CodeAnnotationAnalyzer
        private ConsoleConnection(
            FormatBuilder defaultLogFormat,
            LoggingLevels defaultLoggingLevels,
            CancellationToken token)
        {
            Contract.Requires<RequiredContractException>(ConsoleHelper.IsConsole, "Not_In_Console");
            _defaultLogFormat = defaultLogFormat;
            _defaultLoggingLevels = defaultLoggingLevels;
            _cancellationTokenSource = token.CanBeCanceled
                // ReSharper disable once PossiblyMistakenUseOfParamsMethod
                ? CancellationTokenSource.CreateLinkedTokenSource(token)
                : new CancellationTokenSource();
        }

        /// <summary>
        /// Runs the specified service using the command console as a user interface.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="defaultLogFormat">The default log format.</param>
        /// <param name="defaultLoggingLevels">The default logging levels.</param>
        /// <param name="token">The token.</param>
        /// <returns>An awaitable task.</returns>
        // ReSharper disable once CodeAnnotationAnalyzer
        public static Task RunAsync(
            [NotNull] BaseService service,
            FormatBuilder defaultLogFormat = null,
            LoggingLevels defaultLoggingLevels = LoggingLevels.All,
            CancellationToken token = default(CancellationToken))
        {
            Contract.Requires<RequiredContractException>(service != null, "Parameter_Null");
            if (!ConsoleHelper.IsConsole)
                return TaskResult.Completed;
            Console.Clear();
            Console.Title = service.ServiceName;
            Log.SetTrace(validLevels: LoggingLevels.None);
            Log.SetConsole(defaultLogFormat ?? Log.ShortFormat, defaultLoggingLevels);

            ConsoleConnection connection = new ConsoleConnection(defaultLogFormat, defaultLoggingLevels, token);
            Guid id = service.Connect(connection);
            CancellationToken t = connection._cancellationTokenSource.Token;
            return Task.Run(
                async () =>
                {
                    try
                    {
                        do
                        {
                            // Flush logs
                            await Log.Flush(t);
                            WritePrompt(service);
                            service.Execute(id, await Console.In.ReadLineAsync(), ConsoleTextWriter.Default);

                            // Let any async stuff done by the command have a bit of time, also throttle commands.
                            await Task.Delay(500, t);
                        } while (!t.IsCancellationRequested);
                    }
                    catch (TaskCanceledException)
                    {
                    }
                    finally
                    {
                        Log.Flush().Wait();
                        service.Disconnect(id);
                    }
                },
                t);
        }

        /// <summary>
        /// Called when the server disconnects the UI.
        /// </summary>
        public void OnDisconnect()
        {
            _cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Gets the default log format, this can be changed by commands.
        /// </summary>
        /// <value>The default log format.</value>
        [CanBeNull]
        [PublicAPI]
        public FormatBuilder DefaultLogFormat
        {
            get { return _defaultLogFormat; }
        }

        /// <summary>
        /// Gets the default logging levels, this can be changed by commands.
        /// </summary>
        /// <value>The default logging levels.</value>
        [PublicAPI]
        public LoggingLevels DefaultLoggingLevels
        {
            get { return _defaultLoggingLevels; }
        }

        /// <summary>
        /// Writes the prompt.
        /// </summary>
        private static void WritePrompt([NotNull] BaseService service)
        {
            if (Console.CursorLeft != 0)
                ConsoleTextWriter.Default.WriteLine();
            _promptBuilder.WriteToConsole(
                null,
                (_, c) =>
                {
                    switch (c.Tag.ToLowerInvariant())
                    {
                        case "time":
                            return DateTime.UtcNow;
                        case "state":
                            return service.State;
                        default:
                            return Resolution.Unknown;
                    }
                });
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            CancellationTokenSource cts = Interlocked.Exchange(ref _cancellationTokenSource, null);
            if (cts != null)
                cts.Dispose();
        }
    }
}