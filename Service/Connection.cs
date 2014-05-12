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
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WebApplications.Utilities.Formatting;
using WebApplications.Utilities.Logging;
using WebApplications.Utilities.Logging.Interfaces;
using WebApplications.Utilities.Logging.Loggers;
using WebApplications.Utilities.Performance;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.Service
{
    /// <summary>
    /// Base implementation of a service.
    /// </summary>
    public abstract partial class BaseService<TService>
    {
        private class Connection : IDisposable
        {
            /// <summary>
            /// The identifier.
            /// </summary>
            [PublicAPI]
            public readonly Guid ID;

            /// <summary>
            /// The user interface
            /// </summary>
            [NotNull]
            [PublicAPI]
            public readonly IServiceUserInterface UserInterface;

            /// <summary>
            /// The subscription to the user interface commands.
            /// </summary>
            private CancellationTokenSource _cancellationTokenSource;

            /// <summary>
            /// The _logger
            /// </summary>
            private TextWriterLogger _logger;

            /// <summary>
            /// Gets the logger.
            /// </summary>
            /// <value>The logger.</value>
            [NotNull]
            [PublicAPI]
            public TextWriterLogger Logger
            {
                get
                {
                    if (_logger == null)
                        throw new ObjectDisposedException("Connection");
                    return _logger;
                }
            }

            /// <summary>
            /// The default format.
            /// </summary>
            public readonly FormatBuilder DefaultFormat;

            /// <summary>
            /// The default logging levels.
            /// </summary>
            public readonly LoggingLevels DefaultLoggingLevels;

            /// <summary>
            /// Initializes a new instance of the <see cref="Connection" /> class.
            /// </summary>
            /// <param name="service">The service.</param>
            /// <param name="id">The identifier.</param>
            /// <param name="userInterface">The user interface.</param>
            public Connection([NotNull] BaseService<TService> service, Guid id, [NotNull] IServiceUserInterface userInterface)
            {
                Contract.Requires<RequiredContractException>(userInterface != null, "Parameter_Null");
                Contract.Requires<RequiredContractException>(service != null, "Parameter_Null");
                ID = id;
                UserInterface = userInterface;

                DefaultFormat = userInterface.DefaultLogFormat ?? Log.ShortFormat;
                DefaultLoggingLevels = userInterface.DefaultLoggingLevels;

                // Send logs to writer.
                _logger = new TextWriterLogger(
                    string.Format("Log writer for '{0}' service connection.", id),
                    userInterface.Writer,
                    DefaultFormat,
                    DefaultLoggingLevels);
                Log.AddLogger(_logger);

                // Create task to read lines async.
                _cancellationTokenSource = new CancellationTokenSource();
                CancellationToken token = _cancellationTokenSource.Token;
                TextReader reader = userInterface.Reader;

                Task.Run(
                    async () =>
                    {
                        try
                        {
                            do
                            {
                                string line = await reader.ReadLineAsync();
                                token.ThrowIfCancellationRequested();

                                if (line == null)
                                    break;

                                service.OnCommand(this, line);
                            } while (true);
                            service.Disconnect(ID);
                        }
                        catch (Exception exception)
                        {
                            service.OnCommandError(this, exception);
                        }
                    }, token);
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                CancellationTokenSource cts = Interlocked.Exchange(ref _cancellationTokenSource, null);
                if ((cts != null) &&
                    (!cts.IsCancellationRequested))
                    cts.Cancel();

                TextWriterLogger logger = Interlocked.Exchange(ref _logger, null);
                if (logger == null) return;
                Log.Flush().Wait();
                Log.RemoveLogger(logger);
                logger.Dispose();
            }
        }
    }
}