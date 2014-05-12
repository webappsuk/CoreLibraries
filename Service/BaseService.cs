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
using WebApplications.Utilities.Logging;
using WebApplications.Utilities.Logging.Interfaces;
using WebApplications.Utilities.Logging.Loggers;
using WebApplications.Utilities.Performance;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.Service
{
    /// <summary>
    /// Base implementation of a service, you should always extends the generic version of this class.
    /// </summary>
    public abstract class BaseService : ServiceBase
    {
        /// <summary>
        /// Initializes static members of the <see cref="BaseService"/> class.
        /// </summary>
        static BaseService()
        {
            // TODO Move to Utilities
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.Cancel();
            Cancelled = cts.Token;
        }

        // TODO Move to Utilities
        protected static readonly PauseToken Paused = new PauseTokenSource { IsPaused = true }.Token;
        protected static readonly CancellationToken Cancelled;

        /// <summary>
        /// When implemented in a derived class, executes when a Start command is sent to the service by the Service Control Manager (SCM) or when the operating system starts (for a service that starts automatically). Specifies actions to take when the service starts.
        /// </summary>
        /// <param name="args">Data passed by the start command.</param>
        protected virtual void DoStart([NotNull] string[] args)
        {
            Contract.Requires<RequiredContractException>(args != null, "Parameter_Null");
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Stop command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service stops running.
        /// </summary>
        protected virtual void DoStop()
        {
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Pause command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service pauses.
        /// </summary>
        protected virtual void DoPause()
        {
        }

        /// <summary>
        /// When implemented in a derived class, <see cref="M:System.ServiceProcess.ServiceBase.OnContinue" /> runs when a Continue command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service resumes normal functioning after being paused.
        /// </summary>
        protected virtual void DoContinue()
        {
        }

        /// <summary>
        /// When implemented in a derived class, <see cref="M:System.ServiceProcess.ServiceBase.OnCustomCommand(System.Int32)" /> executes when the Service Control Manager (SCM) passes a custom command to the service. Specifies actions to take when a command with the specified parameter value occurs.
        /// </summary>
        /// <param name="command">The command message sent to the service.</param>
        protected virtual void DoCustomCommand(int command)
        {
        }

        /// <summary>
        /// Connects the specified user interface.
        /// </summary>
        /// <param name="userInterface">The user interface.</param>
        /// <returns>A connection GUID.</returns>
        public abstract Guid Connect([NotNull] IServiceUserInterface userInterface);

        /// <summary>
        /// Disconnects the specified user interface.
        /// </summary>
        /// <param name="id">The connection.</param>
        /// <returns><see langword="true" /> if disconnected, <see langword="false" /> otherwise.</returns>
        public abstract bool Disconnect(Guid id);

        /// <summary>
        /// Provides command help.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="parameter">The parameter.</param>
        [PublicAPI]
        [ServiceRunnerCommand(typeof(ServiceResources), "Cmd_Help_Names", "Cmd_Help_Description", writerParameter: "writer")]
        protected abstract void Help([NotNull]TextWriter writer, [CanBeNull] string commandName = null, [CanBeNull] string parameter = null);

        /// <summary>
        /// Runs this instance.
        /// </summary>
        /// TODO Does this cause a problem, if we run multiple services one a time?? Why is the base implementation not done this way??
        public void Run()
        {
            Run(this);
        }
    }

    /// <summary>
    /// Base implementation of a service.
    /// </summary>
    public abstract class BaseService<TService> : BaseService
        where TService : BaseService<TService>
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

                // Send logs to writer.
                _logger = new TextWriterLogger(string.Format("Log writer for '{0}' service connection.", id), userInterface.Writer);
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

        #region Performance Counters
        // ReSharper disable MemberCanBePrivate.Global
        [NotNull]
        internal static readonly PerfTimer PerfTimerStart = PerfCategory.GetOrAdd<PerfTimer>(
            "Service Start",
            "Service starting up.");

        [NotNull]
        internal static readonly PerfTimer PerfTimerStop = PerfCategory.GetOrAdd<PerfTimer>(
            "Service Stop",
            "Service stopping.");

        [NotNull]
        internal static readonly PerfTimer PerfTimerCustomCommand = PerfCategory.GetOrAdd<PerfTimer>(
            "Service Command",
            "Service running custom command.");

        [NotNull]
        internal static readonly PerfCounter PerfCounterPause = PerfCategory.GetOrAdd<PerfCounter>(
            "Service Pause",
            "Service paused.");

        [NotNull]
        internal static readonly PerfCounter PerfCounterContinue = PerfCategory.GetOrAdd<PerfCounter>(
            "Service Continue",
            "Service continued.");

        // ReSharper restore MemberCanBePrivate.Global
        #endregion

        /// <summary>
        /// The commands supported by this service.
        /// </summary>
        [PublicAPI]
        [NotNull]
        public static readonly IReadOnlyDictionary<string, ServiceRunnerCommand> Commands;

        /// <summary>
        /// The service assembly description.
        /// </summary>
        [PublicAPI]
        [NotNull]
        public static readonly string Description;

        /// <summary>
        /// The lock.
        /// </summary>
        [NotNull]
        private readonly object _lock = new object();

        /// <summary>
        /// Any connected user interfaces.
        /// </summary>
        [NotNull]
        private readonly Dictionary<Guid, Connection> _connections = new Dictionary<Guid, Connection>();

        /// <summary>
        /// The <see cref="PauseTokenSource"/>.
        /// </summary>
        private PauseTokenSource _pauseTokenSource = new PauseTokenSource();

        /// <summary>
        /// The <see cref="CancellationTokenSource"/>.
        /// </summary>
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Gets a <see cref="Utilities.Threading.PauseToken"/> that is paused when the service is not running, or paused.
        /// </summary>
        /// <value>A pause token.</value>
        [PublicAPI]
        public PauseToken PauseToken
        {
            get
            {
                lock (_lock)
                {
                    PauseTokenSource ts = _pauseTokenSource;
                    return ts == null ? Paused : ts.Token;
                }
            }
        }

        /// <summary>
        /// Gets a <see cref="CancellationToken"/> that is cancelled when the service is not running.
        /// </summary>
        /// <value>A pause token.</value>
        [PublicAPI]
        public CancellationToken CancellationToken
        {
            get
            {
                lock (_lock)
                {
                    CancellationTokenSource ts = _cancellationTokenSource;
                    return ts == null ? Cancelled : ts.Token;
                }
            }
        }

        /// <summary>
        /// Initializes static members of the <see cref="BaseService"/> class.
        /// </summary>
        static BaseService()
        {
            MethodInfo[] allMethods = typeof(TService)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .ToArray();
            Dictionary<string, ServiceRunnerCommand> commands =
                new Dictionary<string, ServiceRunnerCommand>(
                    allMethods.Length * 3,
                    StringComparer.CurrentCultureIgnoreCase);
            foreach (MethodInfo method in allMethods)
            {
                Contract.Assert(method != null);
                ServiceRunnerCommand src;
                try
                {
                    ServiceRunnerCommandAttribute attribute = method
                        .GetCustomAttributes(typeof(ServiceRunnerCommandAttribute), true)
                        .OfType<ServiceRunnerCommandAttribute>()
                        .FirstOrDefault();
                    if (attribute == null) continue;
                    if (method.IsGenericMethod)
                    {
                        Log.Add(
                            LoggingLevel.Warning,
                            () => ServiceResources.Wrn_Command_Invalid_Generic,
                            method);
                        continue;
                    }

                    src = new ServiceRunnerCommand(method, attribute);
                }
                catch (Exception e)
                {
                    Log.Add(
                        e,
                        LoggingLevel.Warning,
                        () => ServiceResources.Wrn_ServiceCommand_Creation_Failed,
                        method);
                    continue;
                }

                // Add command aliases to dictionary
                foreach (string name in src.AllNames)
                {
                    Contract.Assert(name != null);
                    ServiceRunnerCommand existing;
                    if (commands.TryGetValue(name, out existing))
                    {
                        Contract.Assert(existing != null);
                        Log.Add(
                            LoggingLevel.Warning,
                            () => ServiceResources.Wrn_Command_Alias_Already_Used_By_Other_Command,
                            name,
                            src.Name,
                            existing.Name);
                    }
                    commands[name] = src;
                }
            }
            Commands = new ReadOnlyDictionary<string, ServiceRunnerCommand>(commands);

            Assembly assembly = typeof(TService).Assembly;
            if (assembly.IsDefined(typeof(AssemblyDescriptionAttribute), false))
            {
                AssemblyDescriptionAttribute a =
                    Attribute.GetCustomAttribute(assembly, typeof(AssemblyDescriptionAttribute)) as
                        AssemblyDescriptionAttribute;
                if (a != null)
                {
                    Contract.Assert(a.Description != null);
                    Description = a.Description;
                }
            }

            if (string.IsNullOrWhiteSpace(Description))
                Description = "A windows service.";

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseService"/> class.
        /// </summary>
        protected BaseService([CanBeNull] string description = null)
        {
            if (string.IsNullOrWhiteSpace(description) || description.Length > 80)
                description = Description;
            ServiceName = description;
            AutoLog = false;
            CanStop = true;
            CanHandlePowerEvent = true;
            CanHandleSessionChangeEvent = true;
            CanPauseAndContinue = true;
            CanShutdown = true;
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Start command is sent to the service by the Service Control Manager (SCM) or when the operating system starts (for a service that starts automatically). Specifies actions to take when the service starts.
        /// </summary>
        /// <param name="args">Data passed by the start command.</param>
        // ReSharper disable once CodeAnnotationAnalyzer
        protected override sealed void OnStart([NotNull] string[] args)
        {
            using (PerfTimerStart.Region())
            {
                lock (_lock)
                {
                    _cancellationTokenSource = new CancellationTokenSource();
                    _pauseTokenSource = new PauseTokenSource();
                }
                DoStart(args);
            }
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Stop command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service stops running.
        /// </summary>
        protected override sealed void OnStop()
        {
            using (PerfTimerStop.Region())
            {
                DoStop();
                lock (_lock)
                {
                    Contract.Assert(_cancellationTokenSource != null);
                    Contract.Assert(_pauseTokenSource != null);
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource = null;
                    _pauseTokenSource.IsPaused = true;
                    _pauseTokenSource = null;
                }
            }
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Pause command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service pauses.
        /// </summary>
        protected override sealed void OnPause()
        {
            Contract.Assert(_pauseTokenSource != null);
            _pauseTokenSource.IsPaused = true;
            PerfCounterPause.Increment();
            DoPause();
        }

        /// <summary>
        /// When implemented in a derived class, <see cref="M:System.ServiceProcess.ServiceBase.OnContinue" /> runs when a Continue command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service resumes normal functioning after being paused.
        /// </summary>
        protected override sealed void OnContinue()
        {
            Contract.Assert(_pauseTokenSource != null);
            _pauseTokenSource.IsPaused = false;
            PerfCounterContinue.Increment();
            DoContinue();
        }

        /// <summary>
        /// When implemented in a derived class, <see cref="M:System.ServiceProcess.ServiceBase.OnCustomCommand(System.Int32)" /> executes when the Service Control Manager (SCM) passes a custom command to the service. Specifies actions to take when a command with the specified parameter value occurs.
        /// </summary>
        /// <param name="command">The command message sent to the service.</param>
        protected override sealed void OnCustomCommand(int command)
        {
            using (PerfTimerCustomCommand.Region())
                DoCustomCommand(command);
        }

        /// <summary>
        /// Connects the specified user interface.
        /// </summary>
        /// <param name="userInterface">The user interface.</param>
        /// <returns>A connection GUID.</returns>
        public override Guid Connect([NotNull] IServiceUserInterface userInterface)
        {
            Contract.Requires<RequiredContractException>(userInterface != null, "Parameter_Null");
            lock (_lock)
            {
                Guid connectionGuid;
                do
                {
                    // Technically this loop should be unnecessary, but it's cheap.
                    connectionGuid = Guid.NewGuid();
                } while (_connections.ContainsKey(connectionGuid));
                _connections[connectionGuid] = new Connection(this, connectionGuid, userInterface);
                return connectionGuid;
            }
        }

        /// <summary>
        /// Disconnects the specified user interface.
        /// </summary>
        /// <param name="id">The connection.</param>
        /// <returns><see langword="true" /> if disconnected, <see langword="false" /> otherwise.</returns>
        public override bool Disconnect(Guid id)
        {
            Contract.Requires<RequiredContractException>(id != Guid.Empty, "Parameter_Guid_Empty");
            lock (_lock)
            {
                Connection connection;
                if (!_connections.TryGetValue(id, out connection))
                    return false;

                _connections.Remove(id);
                Contract.Assert(connection != null);
                connection.Dispose();
                return true;
            }
        }

        /// <summary>
        /// Called when a command is received from a connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="line">The command.</param>
        // ReSharper disable once CodeAnnotationAnalyzer
        private void OnCommand([NotNull]Connection connection, [CanBeNull]string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return;

            // Find the first split point, and grab the command
            line = line.TrimStart();
            int firstSpace = 0;
            do
            {
                if (Char.IsWhiteSpace(line[firstSpace]))
                    break;
                firstSpace++;
            } while (firstSpace < line.Length);
            string commandName = line.Substring(0, firstSpace);
            line = firstSpace < line.Length ? line.Substring(firstSpace + 1) : string.Empty;

            TextWriter writer = connection.UserInterface.Writer;
            Contract.Assert(writer != null);

            ServiceRunnerCommand src;
            if (!Commands.TryGetValue(commandName, out src))
            {
                Help(writer);
                return;
            }

            Contract.Assert(src != null);
            try
            {
                if (src.Run(this, writer, connection.ID, line)) return;
                // TODO write failed message first.
                Help(writer, commandName);
            }
            catch (Exception e)
            {
                Log.Add(e, LoggingLevel.Error, () => ServiceResources.Err_Comman_Exception, commandName);
                Help(writer, commandName);
            }
        }

        /// <summary>
        /// Called when an error is received on a command observable.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="exception">The exception.</param>
        // ReSharper disable once CodeAnnotationAnalyzer
        private void OnCommandError([NotNull]Connection connection, [CanBeNull]Exception exception)
        {
            Log.Add(exception, LoggingLevel.Critical, () => ServiceResources.Cri_Base_Service_Command_Error, connection.ID);
            Disconnect(connection.ID);
        }

        /// <summary>
        /// Provides command help.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="parameter">The parameter.</param>
        // ReSharper disable once CodeAnnotationAnalyzer
        protected override void Help(TextWriter writer, string commandName = null, string parameter = null)
        {
            Contract.Requires<RequiredContractException>(writer != null, "Parameter_Null");
        }

        /// <summary>
        /// Disposes of the resources (other than memory) used by the <see cref="T:System.ServiceProcess.ServiceBase" />.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            lock (_lock)
            {
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource = null;
                }
                if (_pauseTokenSource != null)
                {
                    _pauseTokenSource.IsPaused = true;
                    _pauseTokenSource = null;
                }
            }
            base.Dispose(disposing);
        }
    }
}