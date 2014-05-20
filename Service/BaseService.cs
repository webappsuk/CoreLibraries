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
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WebApplications.Utilities.Logging;
using WebApplications.Utilities.Logging.Loggers;
using WebApplications.Utilities.Performance;
using WebApplications.Utilities.Threading;
using SCP = WebApplications.Utilities.Service.ServiceCommandParameterAttribute;

namespace WebApplications.Utilities.Service
{
    /// <summary>
    /// Base implementation of a service, you should always extends the generic version of this class.
    /// </summary>
    public abstract partial class BaseService : ServiceBase
    {
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

        [NotNull]
        internal static readonly PerfCounter PerfCounterPowerEvent = PerfCategory.GetOrAdd<PerfCounter>(
            "Service Power Event",
            "Service power event occured.");

        [NotNull]
        internal static readonly PerfCounter PerfCounterSessionChange = PerfCategory.GetOrAdd<PerfCounter>(
            "Service Session Change",
            "Service session changed.");

        // ReSharper restore MemberCanBePrivate.Global
        #endregion

        /// <summary>
        /// Gets the current state of the service.
        /// </summary>
        /// <value>The state.</value>
        [PublicAPI]
        public abstract ServiceControllerStatus State { get; }

        /// <summary>
        /// The current process is running as a service.
        /// </summary>
        private static readonly bool _isServiceProcess;

        /// <summary>
        /// Gets an event log you can use to write notification of service command calls, such as Start and Stop, to the Application event log.
        /// </summary>
        /// <value>The event log.</value>
        /// <returns>An <see cref="T:System.Diagnostics.EventLog" /> instance whose source is registered to the Application log.</returns>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" />
        ///   <IPermission class="System.Diagnostics.EventLogPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public override EventLog EventLog
        {
            get { return _eventLog; }
        }

        /// <summary>
        /// The event logger
        /// </summary>
        [NotNull]
        private readonly EventLog _eventLog;

        private static readonly bool _isAdministrator;

        /// <summary>
        /// Determines whether this instance is an administrator.
        /// </summary>
        /// <returns><see langword="true" /> if this instance is administrator; otherwise, <see langword="false" />.</returns>
        public static bool IsAdministrator
        {
            get { return _isAdministrator; }
        }

        /// <summary>
        /// Gets a <see cref="Utilities.Threading.PauseToken"/> that is paused when the service is not running, or paused.
        /// </summary>
        /// <value>A pause token.</value>
        [PublicAPI]
        public abstract PauseToken PauseToken { get; }

        /// <summary>
        /// Gets a <see cref="CancellationToken"/> that is cancelled when the service is not running.
        /// </summary>
        /// <value>A pause token.</value>
        [PublicAPI]
        public abstract CancellationToken CancellationToken { get; }

        /// <summary>
        /// Initializes static members of the <see cref="BaseService"/> class.
        /// </summary>
        static BaseService()
        {
            _isServiceProcess = !Environment.UserInteractive;

            try
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                Contract.Assert(identity != null);
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                _isAdministrator = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch
            {
                _isAdministrator = false;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseService" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="description">The description.</param>
        /// <param name="serverConfiguration">The server configuration.</param>
        protected BaseService(
            [NotNull] string name,
            [CanBeNull] string displayName,
            [CanBeNull] string description,
            [CanBeNull] ServerConfig serverConfiguration = null)
        {
            Contract.Requires<RequiredContractException>(name != null, "Parameter_Null");
            ServiceName = name;
            DisplayName = displayName ?? name;
            Description = description ?? DisplayName;
            ServerConfiguration = serverConfiguration ?? ServerConfig.Default;
            AutoLog = false;
            CanStop = true;
            CanHandlePowerEvent = true;
            CanHandleSessionChangeEvent = true;
            CanPauseAndContinue = true;
            CanShutdown = true;
            IsService = _isServiceProcess &&
                        ServiceUtils.ServiceIsInstalled(name) &&
                        ServiceUtils.GetServiceStatus(name) == ServiceControllerStatus.StartPending;

            // Create event log.
            EventLogger eventLogger = Log.GetLoggers<EventLogger>().FirstOrDefault();
            if (eventLogger != null)
            {
                string source = Log.ApplicationName;
                if (string.IsNullOrWhiteSpace(source))
                    source = "Application";
                else if (source.Length > 254)
                    source = source.Substring(0, 254);

                _eventLog = new EventLog
                {
                    Source = source,
                    MachineName = eventLogger.MachineName,
                    Log = eventLogger.EventLog
                };
            }
            else
                _eventLog = new EventLog
                {
                    Source = "Application",
                    MachineName = ".",
                    Log = ServiceName
                };

            if (_eventLog.MachineName == ".")
            {
                // Create the event log if necessary.
                ((ISupportInitialize) (_eventLog)).BeginInit();
                if (!EventLog.SourceExists(_eventLog.Source))
                    EventLog.CreateEventSource(_eventLog.Source, _eventLog.Log);
                ((ISupportInitialize) (_eventLog)).EndInit();
            }
        }

        /// <summary>
        /// Whether the service is running as a service.
        /// </summary>
        [PublicAPI]
        public readonly bool IsService;

        /// <summary>
        /// The display name.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public readonly string DisplayName;

        /// <summary>
        /// The description.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public readonly string Description;

        /// <summary>
        /// The server configuration.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public readonly ServerConfig ServerConfiguration;

        /// <summary>
        /// Runs the service, either as a service or as a console application.
        /// </summary>
        /// <param name="promptInstall">if set to <see langword="true" /> provides installation options.</param>
        /// <param name="allowConsole">if set to <see langword="true" /> allows console interaction whilst running in a console window.</param>
        [PublicAPI]
        public void Run(bool promptInstall = true, bool allowConsole = true)
        {
            // ReSharper disable MethodSupportsCancellation
            RunAsync(promptInstall, allowConsole).Wait();
            // ReSharper restore MethodSupportsCancellation
        }

        /// <summary>
        /// Runs the service, either as a service or as a console application.
        /// </summary>
        /// <param name="promptInstall">if set to <see langword="true" /> provides installation options.</param>
        /// <param name="allowConsoleInteraction">if set to <see langword="true" /> allows console interaction whilst running in a console window.</param>
        /// <param name="token">The token.</param>
        /// <returns>An awaitable task.</returns>
        [NotNull]
        [PublicAPI]
        public abstract Task RunAsync(
            bool promptInstall = true,
            bool allowConsoleInteraction = true,
            CancellationToken token = default(CancellationToken));

        /// <summary>
        /// When implemented in a derived class, executes when a Start command is sent to the service by the Service Control Manager (SCM) or when the operating system starts (for a service that starts automatically). Specifies actions to take when the service starts.
        /// </summary>
        /// <param name="args">Data passed by the start command.</param>
        [PublicAPI]
        protected virtual void DoStart([NotNull] string[] args)
        {
            Contract.Requires<RequiredContractException>(args != null, "Parameter_Null");
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Stop command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service stops running.
        /// </summary>
        [PublicAPI]
        protected virtual void DoStop()
        {
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Pause command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service pauses.
        /// </summary>
        [PublicAPI]
        protected virtual void DoPause()
        {
        }

        /// <summary>
        /// When implemented in a derived class, <see cref="M:System.ServiceProcess.ServiceBase.OnContinue" /> runs when a Continue command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service resumes normal functioning after being paused.
        /// </summary>
        [PublicAPI]
        protected virtual void DoContinue()
        {
        }

        /// <summary>
        /// When implemented in a derived class, executes when the system is shutting down. Specifies what should occur immediately prior to the system shutting down.
        /// </summary>
        [PublicAPI]
        protected virtual void DoShutdown()
        {
        }

        /// <summary>
        /// When implemented in a derived class, <see cref="M:System.ServiceProcess.ServiceBase.OnCustomCommand(System.Int32)" /> executes when the Service Control Manager (SCM) passes a custom command to the service. Specifies actions to take when a command with the specified parameter value occurs.
        /// </summary>
        /// <param name="command">The command message sent to the service.</param>
        [PublicAPI]
        protected virtual void DoCustomCommand(int command)
        {
        }

        /// <summary>
        /// When implemented in a derived class, executes when the computer's power status has changed. This applies to laptop computers when they go into suspended mode, which is not the same as a system shutdown.
        /// </summary>
        /// <param name="powerStatus">A <see cref="T:System.ServiceProcess.PowerBroadcastStatus" /> that indicates a notification from the system about its power status.</param>
        /// <returns>When implemented in a derived class, the needs of your application determine what value to return. For example, if a QuerySuspend broadcast status is passed, you could cause your application to reject the query by returning false.</returns>
        [PublicAPI]
        protected virtual bool DoPowerEvent(PowerBroadcastStatus powerStatus)
        {
            return true;
        }

        /// <summary>
        /// Executes when a change event is received from a Terminal Server session.
        /// </summary>
        /// <param name="changeDescription">A <see cref="T:System.ServiceProcess.SessionChangeDescription" /> structure that identifies the change type.</param>
        [PublicAPI]
        protected virtual void DoSessionChange(SessionChangeDescription changeDescription)
        {
        }

        /// <summary>
        /// Connects the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns>
        /// A connection GUID.
        /// </returns>
        public abstract Guid Connect([NotNull] IConnection connection);

        /// <summary>
        /// Executes the command line, and writes the result to the specified writer.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="commandLine">The command line.</param>
        /// <param name="writer">The result writer.</param>
        public abstract void Execute(Guid id, [CanBeNull] string commandLine, [NotNull] TextWriter writer);

        /// <summary>
        /// Disconnects the specified user interface.
        /// </summary>
        /// <param name="id">The connection.</param>
        /// <returns><see langword="true" /> if disconnected, <see langword="false" /> otherwise.</returns>
        public abstract bool Disconnect(Guid id);
    }

    /// <summary>
    /// Base implementation of a service.
    /// </summary>
    [PublicAPI]
    public abstract partial class BaseService<TService> : BaseService
        where TService : BaseService<TService>
    {
        /// <summary>
        /// The commands supported by this service.
        /// </summary>
        [PublicAPI]
        [NotNull]
        // ReSharper disable once StaticFieldInGenericType
        public static readonly IReadOnlyDictionary<string, ServiceCommand> Commands;

        /// <summary>
        /// The service assembly description.
        /// </summary>
        [PublicAPI]
        [NotNull]
        // ReSharper disable once StaticFieldInGenericType
        public static readonly string AssemblyTitle;

        /// <summary>
        /// The service assembly description.
        /// </summary>
        [PublicAPI]
        [NotNull]
        // ReSharper disable once StaticFieldInGenericType
        public static readonly string AssemblyDescription;

        /// <summary>
        /// The assembly unique identifier
        /// </summary>
        [NotNull]
        [PublicAPI]
        // ReSharper disable once StaticFieldInGenericType
        public static readonly string AssemblyGuid;

        /// <summary>
        /// The lock.
        /// </summary>
        [NotNull]
        private readonly object _lock = new object();

        private ServiceControllerStatus _state = ServiceControllerStatus.Stopped;

        /// <summary>
        /// Gets the current state of the service.
        /// </summary>
        /// <value>The state.</value>
        public override ServiceControllerStatus State
        {
            get { return _state; }
        }

        /// <summary>
        /// Any connected user interfaces.
        /// </summary>
        [NotNull]
        private readonly Dictionary<Guid, Connection> _connections = new Dictionary<Guid, Connection>();

        /// <summary>
        /// The <see cref="PauseTokenSource"/>.
        /// </summary>
        [NotNull]
        private readonly PauseTokenSource _pauseTokenSource = new PauseTokenSource();

        /// <summary>
        /// The <see cref="CancellationTokenSource"/>.
        /// </summary>
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// The <see cref="CancellationTokenSource"/>.
        /// </summary>
        private TaskCompletionSource<bool> _lifeTimeTaskCompletionSource;

        /// <summary>
        /// Gets a <see cref="Utilities.Threading.PauseToken"/> that is paused when the service is not running, or paused.
        /// </summary>
        /// <value>A pause token.</value>
        [PublicAPI]
        public override PauseToken PauseToken
        {
            get { return _pauseTokenSource.Token; }
        }

        /// <summary>
        /// Gets a <see cref="CancellationToken"/> that is cancelled when the service is not running.
        /// </summary>
        /// <value>A pause token.</value>
        [PublicAPI]
        public override CancellationToken CancellationToken
        {
            get
            {
                lock (_lock)
                {
                    CancellationTokenSource ts = _cancellationTokenSource;
                    return ts == null ? TaskResult.CancelledToken : ts.Token;
                }
            }
        }

        /// <summary>
        /// Initializes static members of the <see cref="BaseService"/> class.
        /// </summary>
        static BaseService()
        {
            MethodInfo[] allMethods = typeof (TService)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .ToArray();
            Dictionary<string, ServiceCommand> commands =
                new Dictionary<string, ServiceCommand>(
                    allMethods.Length * 3,
                    StringComparer.CurrentCultureIgnoreCase);
            foreach (MethodInfo method in allMethods)
            {
                Contract.Assert(method != null);
                ServiceCommand src;
                try
                {
                    ServiceCommandAttribute attribute = method
                        .GetCustomAttributes(typeof (ServiceCommandAttribute), true)
                        .OfType<ServiceCommandAttribute>()
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

                    src = new ServiceCommand(method, attribute);
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
                    ServiceCommand existing;
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
            Commands = new ReadOnlyDictionary<string, ServiceCommand>(commands);

            Assembly assembly = typeof (TService).Assembly;

            if (assembly.IsDefined(typeof (AssemblyTitleAttribute), false))
            {
                AssemblyTitleAttribute a =
                    Attribute.GetCustomAttribute(assembly, typeof (AssemblyTitleAttribute)) as
                        AssemblyTitleAttribute;
                if (a != null)
                {
                    Contract.Assert(a.Title != null);
                    AssemblyTitle = a.Title;
                }
            }

            if (string.IsNullOrWhiteSpace(AssemblyDescription))
                AssemblyDescription = "A windows service.";

            if (assembly.IsDefined(typeof (AssemblyDescriptionAttribute), false))
            {
                AssemblyDescriptionAttribute a =
                    Attribute.GetCustomAttribute(assembly, typeof (AssemblyDescriptionAttribute)) as
                        AssemblyDescriptionAttribute;
                if (a != null)
                {
                    Contract.Assert(a.Description != null);
                    AssemblyDescription = a.Description;
                }
            }

            if (string.IsNullOrWhiteSpace(AssemblyDescription))
                AssemblyDescription = "A windows service.";


            if (assembly.IsDefined(typeof (GuidAttribute), false))
            {
                GuidAttribute g = Attribute.GetCustomAttribute(assembly, typeof (GuidAttribute)) as GuidAttribute;
                if (g != null)
                    AssemblyGuid = g.Value;
            }
            if (string.IsNullOrWhiteSpace(AssemblyGuid))
            {
                AssemblyGuid = Guid.NewGuid().ToString();
                Log.Add(
                    LoggingLevel.Warning,
                    () => ServiceResources.Err_BaseService_CouldNotLocateAssemblyGuid,
                    assembly);
            }
        }

        /// <summary>
        /// The global wait handler to prevent multiple services running on the same machine.
        /// </summary>
        private EventWaitHandle _runEventWaitHandle;

        /// <summary>
        /// Security for the global event wait handle.
        /// </summary>
        private readonly EventWaitHandleSecurity _eventWaitHandleSecurity;

        /// <summary>
        /// The named pipe server.
        /// </summary>
        private NamedPipeServer _namedPipeServer;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseService" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="description">The description.</param>
        /// <param name="serverConfiguration">The server configuration.</param>
        /// <param name="identity">The identity of users that can start/stop the service, defaults to world.</param>
        protected BaseService(
            [CanBeNull] string name = null,
            [CanBeNull] string displayName = null,
            [CanBeNull] string description = null,
            [CanBeNull] ServerConfig serverConfiguration = null,
            [CanBeNull] IdentityReference identity = null)
            : base(
                (string.IsNullOrWhiteSpace(name) || name.Length > 128) ? AssemblyTitle : name,
                displayName,
                (string.IsNullOrWhiteSpace(description) || description.Length > 80) ? AssemblyDescription : description,
                serverConfiguration)
        {
            try
            {
                if (identity == null)
                    identity = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

                _eventWaitHandleSecurity = new EventWaitHandleSecurity();
                _eventWaitHandleSecurity.AddAccessRule(
                    new EventWaitHandleAccessRule(identity, EventWaitHandleRights.FullControl, AccessControlType.Allow));

                _state = IsService ? ServiceUtils.GetServiceStatus(ServiceName) : ServiceControllerStatus.Stopped;
            }
            catch (Exception e)
            {
                Log.Add(e, LoggingLevel.Error, ServiceResources.Err_BaseService_FatalConstructorError);
            }
        }

        /// <summary>
        /// Runs the service, either as a service or as a console application.
        /// </summary>
        /// <param name="promptInstall">if set to <see langword="true" /> provides installation options.</param>
        /// <param name="allowConsoleInteraction">if set to <see langword="true" /> allows console interaction whilst running in a console window.</param>
        /// <param name="token">The token.</param>
        /// <returns>
        /// An awaitable task.
        /// </returns>
        public override Task RunAsync(
            bool promptInstall = true,
            bool allowConsoleInteraction = true,
            CancellationToken token = default(CancellationToken))
        {
            if (!IsAdministrator)
                Log.Add(
                    LoggingLevel.Information,
                    ServiceResources.Inf_BaseService_RunAsync_NotRunningAsAdmin);

            if (IsService)
            {
                Run(this);
                return TaskResult.Completed;
            }
            lock (_lock)
            {
                _lifeTimeTaskCompletionSource = new TaskCompletionSource<bool>();
                // ReSharper disable AssignNullToNotNullAttribute
                return ConsoleHelper.IsConsole
                    ? (Task) Task.WhenAny(
                        _lifeTimeTaskCompletionSource.Task,
                        ConsoleConnection.RunAsync(this, promptInstall, allowConsoleInteraction, token: token))
                    : _lifeTimeTaskCompletionSource.Task;
                // ReSharper restore AssignNullToNotNullAttribute
            }
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Start command is sent to the service by the Service Control Manager (SCM) or when the operating system starts (for a service that starts automatically). Specifies actions to take when the service starts.
        /// </summary>
        /// <param name="args">Data passed by the start command.</param>
        // ReSharper disable once CodeAnnotationAnalyzer
        protected override sealed void OnStart([NotNull] string[] args)
        {
            if (IsService)
                RequestAdditionalTime(5000);
            try
            {
                using (PerfTimer.Timer region = PerfTimerStart.Region())
                {
                    Log.Add(
                        LoggingLevel.Information,
                        () => ServiceResources.Inf_ServiceRunner_Start_Starting,
                        ServiceName);

                    lock (_lock)
                    {
                        if (IsService)
                            RequestAdditionalTime(5000);
                        if (!IsService)
                            switch (_state)
                            {
                                case ServiceControllerStatus.Stopped:
                                    break;
                                default:
                                    Log.Add(
                                        LoggingLevel.Error,
                                        () => ServiceResources.Err_ServiceRunner_ServiceAlreadyRunning,
                                        ServiceName);
                                    return;
                            }
                        _state = ServiceControllerStatus.StartPending;

                        // Try to grab the global mutex
                        bool hasHandle = false;
                        try
                        {
                            if (_runEventWaitHandle == null) // Sanity check, should be null!
                            {
                                // Create the wait handle.
                                bool createdNew;
                                _runEventWaitHandle = new EventWaitHandle(
                                    true,
                                    EventResetMode.AutoReset,
                                    string.Format("Global\\{{{0}}} {1}", AssemblyGuid, ServiceName),
                                    out createdNew,
                                    _eventWaitHandleSecurity);

                                // We should be the first to create the handle.
                                if (!createdNew)
                                    Log.Add(
                                        LoggingLevel.Warning,
                                        () => ServiceResources.Wrn_BaseService_EventHandlerAlreadyExists);
                            }

                            hasHandle = _runEventWaitHandle.WaitOne(1000, false);
                        }
                        catch (Exception)
                        {
                            // Clean up if we somehow have the handle but also have an exception?!
                            if (_runEventWaitHandle != null)
                            {
                                if (hasHandle)
                                    _runEventWaitHandle.Set();
                                _runEventWaitHandle.Dispose();
                                _runEventWaitHandle = null;
                            }
                        }
                        if (!hasHandle)
                            throw new ServiceException(
                                () => ServiceResources.Err_BaseService_Failed_To_Acquire_WaitHandle);

                        _cancellationTokenSource = new CancellationTokenSource();
                        _pauseTokenSource.IsPaused = false;
                        DoStart(args);

                        // Finally start named pipe server
                        if (ServerConfiguration.MaximumConnections > 0)
                            _namedPipeServer = new NamedPipeServer(this, ServerConfiguration);

                        _state = ServiceControllerStatus.Running;
                    }

                    Log.Add(
                        LoggingLevel.Information,
                        () => ServiceResources.Inf_ServiceRunner_Start_Started,
                        ServiceName,
                        region.Elapsed.TotalMilliseconds);
                }
            }
            catch (Exception e)
            {
                Log.Add(e, LoggingLevel.Error, ServiceResources.Err_BaseService_OnStart_FatalError);
                throw;
            }
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Stop command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service stops running.
        /// </summary>
        protected override sealed void OnStop()
        {
            if (IsService)
                RequestAdditionalTime(5000);
            try
            {
                using (PerfTimer.Timer region = PerfTimerStop.Region())
                {
                    Log.Add(
                        LoggingLevel.Information,
                        () => ServiceResources.Inf_ServiceRunner_Stop_Stopping,
                        ServiceName);

                    lock (_lock)
                    {
                        if (IsService)
                            RequestAdditionalTime(5000);
                        switch (_state)
                        {
                            case ServiceControllerStatus.Running:
                            case ServiceControllerStatus.Paused:
                                break;
                            default:
                                Log.Add(
                                    LoggingLevel.Error,
                                    () => ServiceResources.Err_ServiceRunner_Stop_ServiceNotRunning,
                                    ServiceName);
                                return;
                        }
                        _state = ServiceControllerStatus.StopPending;
                        if (_namedPipeServer != null)
                        {
                            _namedPipeServer.Dispose();
                            _namedPipeServer = null;
                        }
                        DoStop();
                        Contract.Assert(_cancellationTokenSource != null);
                        _cancellationTokenSource.Cancel();
                        _cancellationTokenSource = null;
                        _pauseTokenSource.IsPaused = true;

                        // Try to release the global mutex
                        if (_runEventWaitHandle != null) // This should always be true
                        {
                            _runEventWaitHandle.Set();
                            _runEventWaitHandle.Dispose();
                            _runEventWaitHandle = null;
                        }
                        _state = ServiceControllerStatus.Stopped;
                    }
                    Log.Add(
                        LoggingLevel.Information,
                        () => ServiceResources.Inf_ServiceRunner_Stop_Stopped,
                        ServiceName,
                        region.Elapsed.TotalMilliseconds);
                }
            }
            catch (Exception e)
            {
                Log.Add(e, LoggingLevel.Error, ServiceResources.Err_BaseService_OnStop_FatalError);
                throw;
            }
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Pause command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service pauses.
        /// </summary>
        protected override sealed void OnPause()
        {
            if (IsService)
                RequestAdditionalTime(5000);
            try
            {
                Log.Add(LoggingLevel.Information, () => ServiceResources.Inf_ServiceRunner_Pause_Pausing, ServiceName);
                lock (_lock)
                {
                    if (IsService)
                        RequestAdditionalTime(5000);
                    if (State != ServiceControllerStatus.Running)
                    {
                        Log.Add(
                            LoggingLevel.Error,
                            () => ServiceResources.Err_ServiceRunner_Pause_ServiceNotRunning,
                            ServiceName);
                        return;
                    }
                    _state = ServiceControllerStatus.PausePending;
                    DoPause();
                    _pauseTokenSource.IsPaused = true;
                    PerfCounterPause.Increment();
                    _state = ServiceControllerStatus.Paused;

                    Log.Add(
                        LoggingLevel.Information,
                        () => ServiceResources.Inf_ServiceRunner_Pause_Paused,
                        ServiceName);
                }
            }
            catch (Exception e)
            {
                Log.Add(e, LoggingLevel.Error, ServiceResources.Err_BaseService_OnPause_FatalError);
                throw;
            }
        }

        /// <summary>
        /// When implemented in a derived class, <see cref="M:System.ServiceProcess.ServiceBase.OnContinue" /> runs when a Continue command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service resumes normal functioning after being paused.
        /// </summary>
        protected override sealed void OnContinue()
        {
            if (IsService)
                RequestAdditionalTime(5000);
            try
            {
                Log.Add(
                    LoggingLevel.Information,
                    () => ServiceResources.Inf_ServiceRunner_Continue_Continuing,
                    ServiceName);
                lock (_lock)
                {
                    if (IsService)
                        RequestAdditionalTime(5000);
                    if (State != ServiceControllerStatus.Paused)
                    {
                        Log.Add(
                            LoggingLevel.Error,
                            () => ServiceResources.Err_ServiceRunner_Continue_ServiceNotPaused,
                            ServiceName);
                        return;
                    }
                    _state = ServiceControllerStatus.ContinuePending;
                    _pauseTokenSource.IsPaused = false;
                    DoContinue();
                    PerfCounterContinue.Increment();
                    _state = ServiceControllerStatus.Running;

                    Log.Add(
                        LoggingLevel.Information,
                        () => ServiceResources.Inf_ServiceRunner_Continue_Continued,
                        ServiceName);
                }
            }
            catch (Exception e)
            {
                Log.Add(e, LoggingLevel.Error, ServiceResources.Err_BaseService_OnContinue_FatalError);
                throw;
            }
        }

        /// <summary>
        /// When implemented in a derived class, executes when the system is shutting down. Specifies what should occur immediately prior to the system shutting down.
        /// </summary>
        protected override sealed void OnShutdown()
        {
            try
            {
                Log.Add(
                    LoggingLevel.Information,
                    () => ServiceResources.Inf_ServiceRunner_Shutdown_ShuttingDown,
                    ServiceName);
                lock (_lock)
                {
                    _state = ServiceControllerStatus.StopPending;
                    if (_namedPipeServer != null)
                    {
                        _namedPipeServer.Dispose();
                        _namedPipeServer = null;
                    }
                    DoShutdown();
                    CancellationTokenSource cts = Interlocked.Exchange(ref _cancellationTokenSource, null);
                    if (cts != null)
                        cts.Cancel();
                    _pauseTokenSource.IsPaused = true;

                    // Disconnect all connected user interfaces
                    foreach (Connection connection in _connections.Values.ToArray())
                    {
                        Contract.Assert(connection != null);
                        Disconnect(connection.ID);
                    }

                    TaskCompletionSource<bool> ltt = Interlocked.Exchange(ref _lifeTimeTaskCompletionSource, null);
                    if (ltt != null)
                        ltt.TrySetResult(true);
                    _state = ServiceControllerStatus.Stopped;

                    Log.Add(
                        LoggingLevel.Information,
                        () => ServiceResources.Inf_ServiceRunner_Shutdown_ShutDown,
                        ServiceName);
                }
            }
            catch (Exception e)
            {
                Log.Add(e, LoggingLevel.Error, ServiceResources.Err_BaseService_OnShutdown_FatalError);
                throw;
            }
            finally
            {
                // ReSharper disable MethodSupportsCancellation
                Log.Flush().Wait();
                // ReSharper restore MethodSupportsCancellation
            }
        }

        /// <summary>
        /// When implemented in a derived class, <see cref="M:System.ServiceProcess.ServiceBase.OnCustomCommand(System.Int32)" /> executes when the Service Control Manager (SCM) passes a custom command to the service. Specifies actions to take when a command with the specified parameter value occurs.
        /// </summary>
        /// <param name="command">The command message sent to the service.</param>
        protected override sealed void OnCustomCommand(int command)
        {
            try
            {
                Log.Add(
                    LoggingLevel.Information,
                    () => ServiceResources.Inf_ServiceRunner_CustomCommand_Running,
                    command,
                    ServiceName);
                using (PerfTimer.Timer region = PerfTimerCustomCommand.Region())
                {
                    DoCustomCommand(command);

                    Log.Add(
                        LoggingLevel.Information,
                        () => ServiceResources.Inf_ServiceRunner_CustomCommand_Complete,
                        command,
                        ServiceName,
                        region.Elapsed.TotalMilliseconds);
                }
            }
            catch (Exception e)
            {
                Log.Add(e, LoggingLevel.Error, ServiceResources.Err_BaseService_OnCustomCommand_FatalError, command);
                throw;
            }
        }

        /// <summary>
        /// When implemented in a derived class, executes when the computer's power status has changed. This applies to laptop computers when they go into suspended mode, which is not the same as a system shutdown.
        /// </summary>
        /// <param name="powerStatus">A <see cref="T:System.ServiceProcess.PowerBroadcastStatus" /> that indicates a notification from the system about its power status.</param>
        /// <returns>When implemented in a derived class, the needs of your application determine what value to return. For example, if a QuerySuspend broadcast status is passed, you could cause your application to reject the query by returning false.</returns>
        protected override sealed bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            try
            {
                Log.Add(
                    LoggingLevel.Information,
                    () => ServiceResources.Inf_ServiceRunner_PowerEvent_Sending,
                    powerStatus,
                    ServiceName);
                lock (_lock)
                {
                    bool result = DoPowerEvent(powerStatus);
                    PerfCounterPowerEvent.Increment();

                    Log.Add(
                        LoggingLevel.Information,
                        () => ServiceResources.Inf_ServiceRunner_PowerEvent_Sent,
                        powerStatus,
                        ServiceName,
                        result);
                    return result;
                }
            }
            catch (Exception e)
            {
                Log.Add(e, LoggingLevel.Error, ServiceResources.Err_BaseService_OnPowerEvent_FatalError);
                throw;
            }
            finally
            {
                // ReSharper disable MethodSupportsCancellation
                Log.Flush().Wait();
                // ReSharper restore MethodSupportsCancellation
            }
        }

        /// <summary>
        /// Executes when a change event is received from a Terminal Server session.
        /// </summary>
        /// <param name="changeDescription">A <see cref="T:System.ServiceProcess.SessionChangeDescription" /> structure that identifies the change type.</param>
        protected override sealed void OnSessionChange(SessionChangeDescription changeDescription)
        {
            try
            {
                Log.Add(
                    LoggingLevel.Information,
                    () => ServiceResources.Inf_ServiceRunner_SessionChange_Sending,
                    changeDescription.Reason,
                    changeDescription.SessionId,
                    ServiceName);
                lock (_lock)
                {
                    DoSessionChange(changeDescription);
                    PerfCounterSessionChange.Increment();

                    Log.Add(
                        LoggingLevel.Information,
                        () => ServiceResources.Inf_ServiceRunner_SessionChange_Sent,
                        changeDescription.Reason,
                        changeDescription.SessionId,
                        ServiceName);
                }
            }
            catch (Exception e)
            {
                Log.Add(e, LoggingLevel.Error, ServiceResources.Err_BaseService_OnSessionChange_FatalError);
                throw;
            }
        }

        /// <summary>
        /// Connects the specified user interface.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns>
        /// A connection GUID.
        /// </returns>
        // ReSharper disable once CodeAnnotationAnalyzer
        public override Guid Connect(IConnection connection)
        {
            lock (_lock)
            {
                Guid connectionGuid;
                do
                {
                    // Technically this loop should be unnecessary, but it's cheap.
                    connectionGuid = Guid.NewGuid();
                } while (_connections.ContainsKey(connectionGuid));
                _connections[connectionGuid] = new Connection(connectionGuid, connection);
                return connectionGuid;
            }
        }

        /// <summary>
        /// Executes the command line, and writes the result to the specified writer.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="commandLine">The command line.</param>
        /// <param name="writer">The result writer.</param>
        // ReSharper disable once CodeAnnotationAnalyzer
        public override void Execute(Guid id, string commandLine, TextWriter writer)
        {
            Connection connection;
            if (!_connections.TryGetValue(id, out connection) ||
                string.IsNullOrWhiteSpace(commandLine))
                return;

            // Find the first split point, and grab the command
            commandLine = commandLine.TrimStart();
            int firstSpace = 0;
            do
            {
                if (Char.IsWhiteSpace(commandLine[firstSpace]))
                    break;
                firstSpace++;
            } while (firstSpace < commandLine.Length);
            string commandName = commandLine.Substring(0, firstSpace);
            commandLine = firstSpace < commandLine.Length ? commandLine.Substring(firstSpace + 1) : string.Empty;

            ServiceCommand src;
            if (!Commands.TryGetValue(commandName, out src))
            {
                Log.Add(() => ServiceResources.Err_Unknown_Command, commandName);
                Help(writer);
                return;
            }

            Contract.Assert(src != null);
            try
            {
                if (src.Run(this, writer, id, commandLine)) return;
                Log.Add(() => ServiceResources.Err_Command_Failed, commandName);
            }
            catch (Exception e)
            {
                Log.Add(e, LoggingLevel.Error, () => ServiceResources.Err_Command_Exception, commandName);
                throw;
            }
        }

        /// <summary>
        /// Disconnects the specified user interface.
        /// </summary>
        /// <param name="id">The connection.</param>
        /// <returns><see langword="true" /> if disconnected, <see langword="false" /> otherwise.</returns>
        [PublicAPI]
        [ServiceCommand(typeof (ServiceResources), "Cmd_Disconnect_Names", "Cmd_Disconnect_Description",
            idParameter: "id")]
        public override bool Disconnect(Guid id)
        {
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
        /// Disposes of the resources (other than memory) used by the <see cref="T:System.ServiceProcess.ServiceBase" />.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            lock (_lock)
            {
                if (_namedPipeServer != null)
                {
                    _namedPipeServer.Dispose();
                    _namedPipeServer = null;
                }

                if (_runEventWaitHandle != null)
                {
                    _runEventWaitHandle.Set();
                    _runEventWaitHandle.Dispose();
                    _runEventWaitHandle = null;
                }

                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource = null;
                }
                _pauseTokenSource.IsPaused = true;

                TaskCompletionSource<bool> ltt = Interlocked.Exchange(ref _lifeTimeTaskCompletionSource, null);
                if (ltt != null)
                    ltt.TrySetResult(true);
            }
            base.Dispose(disposing);
        }
    }
}