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
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WebApplications.Utilities.Logging;
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
        public abstract ServiceState State { get; }

        /// <summary>
        /// The current instance is running as a service.
        /// </summary>
        public static readonly bool IsService;

        /// <summary>
        /// Initializes static members of the <see cref="BaseService"/> class.
        /// </summary>
        static BaseService()
        {
            IsService = !Environment.UserInteractive;
            if (IsService)
            {
                IsService = false;
                try
                {
                    // ReSharper disable PossibleNullReferenceException
                    Type entryType = Assembly.GetEntryAssembly().EntryPoint.ReflectedType;
                    // ReSharper restore PossibleNullReferenceException
                    while (entryType != typeof (object))
                    {
                        Contract.Assert(entryType != null);
                        if (entryType == typeof (ServiceBase))
                        {
                            IsService = true;
                            break;
                        }
                        entryType = entryType.BaseType;
                    }
                }
                    // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                }
            }

            // TODO Move to Utilities
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.Cancel();
            Cancelled = cts.Token;
        }

        /// <summary>
        /// The service controller for this service (if running as a service).
        /// </summary>
        [CanBeNull]
        protected readonly ServiceController ServiceController;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseService"/> class.
        /// </summary>
        protected BaseService([NotNull] string description)
        {
            Contract.Requires<RequiredContractException>(description != null, "Parameter_Null");
            ServiceName = description;
            AutoLog = false;
            CanStop = true;
            CanHandlePowerEvent = true;
            CanHandleSessionChangeEvent = true;
            CanPauseAndContinue = true;
            CanShutdown = true;
            if (IsService)
                ServiceController = new ServiceController(ServiceName);
        }

        // TODO Move to Utilities
        protected static readonly PauseToken Paused = new PauseTokenSource {IsPaused = true}.Token;
        protected static readonly CancellationToken Cancelled;


        /// <summary>
        /// Runs the service, either as a service or as a console application.
        /// </summary>
        /// <param name="allowConsole">if set to <see langword="true" /> allows console interaction whilst running in a console window.</param>
        /// <param name="namedPipeConfig">The named pipe configuration, if named pipes are supported.</param>
        /// <returns>An awaitable task.</returns>
        public void Run(bool allowConsole = true, [CanBeNull] NamedPipeConfig namedPipeConfig = null)
        {
            RunAsync(allowConsole, namedPipeConfig).Wait();
        }

        /// <summary>
        /// Runs the service, either as a service or as a console application.
        /// </summary>
        /// <param name="allowConsole">if set to <see langword="true" /> allows console interaction whilst running in a console window.</param>
        /// <param name="namedPipeConfig">The named pipe configuration, if named pipes are supported.</param>
        /// <returns>An awaitable task.</returns>
        [NotNull]
        public abstract Task RunAsync(bool allowConsole = true, [CanBeNull] NamedPipeConfig namedPipeConfig = null);

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
        /// When implemented in a derived class, executes when the system is shutting down. Specifies what should occur immediately prior to the system shutting down.
        /// </summary>
        protected virtual void DoShutdown()
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
        /// When implemented in a derived class, executes when the computer's power status has changed. This applies to laptop computers when they go into suspended mode, which is not the same as a system shutdown.
        /// </summary>
        /// <param name="powerStatus">A <see cref="T:System.ServiceProcess.PowerBroadcastStatus" /> that indicates a notification from the system about its power status.</param>
        /// <returns>When implemented in a derived class, the needs of your application determine what value to return. For example, if a QuerySuspend broadcast status is passed, you could cause your application to reject the query by returning false.</returns>
        protected virtual bool DoPowerEvent(PowerBroadcastStatus powerStatus)
        {
            return true;
        }

        /// <summary>
        /// Executes when a change event is received from a Terminal Server session.
        /// </summary>
        /// <param name="changeDescription">A <see cref="T:System.ServiceProcess.SessionChangeDescription" /> structure that identifies the change type.</param>
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
        /// Executes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="commandLine">The command line.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>The result.</returns>
        [NotNull]
        private string Execute(
            Guid id,
            [CanBeNull] string commandLine,
            [CanBeNull] IFormatProvider formatProvider = null)
        {
            if (string.IsNullOrWhiteSpace(commandLine))
                return string.Empty;
            using (StringWriter writer = new StringWriter(formatProvider))
            {
                Execute(id, commandLine, writer);
                return writer.ToString();
            }
        }

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

        /// <summary>
        /// Provides command help.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="command">Name of the command.</param>
        /// <param name="parameter">The parameter.</param>
        [PublicAPI]
        [ServiceCommand(typeof (ServiceResources), "Cmd_Help_Names", "Cmd_Help_Description", writerParameter: "writer")]
        protected abstract void Help(
            [NotNull] TextWriter writer,
            [CanBeNull] [SCP(typeof (ServiceResources), "Cmd_Help_Command_Description")] string command = null,
            [CanBeNull] [SCP(typeof (ServiceResources), "Cmd_Help_Parameter_Description")] string parameter = null);
    }

    /// <summary>
    /// Base implementation of a service.
    /// </summary>
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
        public static readonly string Description;

        /// <summary>
        /// The lock.
        /// </summary>
        [NotNull]
        private readonly object _lock = new object();

        private ServiceState _state;

        /// <summary>
        /// Gets the current state of the service.
        /// </summary>
        /// <value>The state.</value>
        public override ServiceState State
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
        private TaskCompletionSource<bool> _lifeTimeTask;

        /// <summary>
        /// Gets a <see cref="Utilities.Threading.PauseToken"/> that is paused when the service is not running, or paused.
        /// </summary>
        /// <value>A pause token.</value>
        [PublicAPI]
        public PauseToken PauseToken
        {
            get { return _pauseTokenSource.Token; }
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
            if (assembly.IsDefined(typeof (AssemblyDescriptionAttribute), false))
            {
                AssemblyDescriptionAttribute a =
                    Attribute.GetCustomAttribute(assembly, typeof (AssemblyDescriptionAttribute)) as
                        AssemblyDescriptionAttribute;
                if (a != null)
                {
                    Contract.Assert(a.Description != null);
                    Description = a.Description;
                }
            }

            if (string.IsNullOrWhiteSpace(Description))
                Description = "A windows service.";

            
            if (assembly.IsDefined(typeof (GuidAttribute), false))
            {
                GuidAttribute g =
                    Attribute.GetCustomAttribute(assembly, typeof (GuidAttribute)) as GuidAttribute;
                if (g != null)
                    AssemblyGuid = g.Value;
            }
            if (string.IsNullOrWhiteSpace(AssemblyGuid))
            {
                AssemblyGuid = Guid.NewGuid().ToString();
                Log.Add(LoggingLevel.Warning, () => ServiceResources.Err_BaseService_CouldNotLocateAssemblyGuid, assembly);
            }
        }

        /// <summary>
        /// The assembly unique identifier
        /// </summary>
        public static readonly string AssemblyGuid;

        /// <summary>
        /// The global mutext to prevent multiple services running on the same machine.
        /// </summary>
        private readonly EventWaitHandle _runEventWaitHandle;

        /// <summary>
        /// Whether the service currently owns the run mutex.
        /// </summary>
        private bool _hasRunMutex;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseService" /> class.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="sddlForm">SDDL string for the SID used to create the <see cref="SecurityIdentifier"/> object to 
        /// identify who can can start/stop the service.</param>
        protected BaseService([CanBeNull] string description, [NotNull] string sddlForm)
            : this(description, new SecurityIdentifier(sddlForm))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseService" /> class.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="sidType">One of the enumeration of well known sid types, the value must not be 
        /// <see cref="WellKnownSidType.LogonIdsSid" />.  This defines
        /// who can start/stop the service.</param>
        /// <param name="domainSid"><para>The domain SID. This value is required for the following <see cref="WellKnownSidType" /> values.
        /// This parameter is ignored for any other <see cref="WellKnownSidType" /> values.</para>
        /// <list type="bullet">
        ///   <item>
        ///     <description>AccountAdministratorSid</description>
        ///   </item>
        ///   <item>
        ///     <description>AccountGuestSid</description>
        ///   </item>
        ///   <item>
        ///     <description>AccountKrbtgtSid</description>
        ///   </item>
        ///   <item>
        ///     <description>AccountDomainAdminsSid</description>
        ///   </item>
        ///   <item>
        ///     <description>AccountDomainUsersSid</description>
        ///   </item>
        ///   <item>
        ///     <description>AccountDomainGuestsSid</description>
        ///   </item>
        ///   <item>
        ///     <description>AccountComputersSid</description>
        ///   </item>
        ///   <item>
        ///     <description>AccountControllersSid</description>
        ///   </item>
        ///   <item>
        ///     <description>AccountCertAdminsSid</description>
        ///   </item>
        ///   <item>
        ///     <description>AccountSchemaAdminsSid</description>
        ///   </item>
        ///   <item>
        ///     <description>AccountEnterpriseAdminsSid</description>
        ///   </item>
        ///   <item>
        ///     <description>AccountPolicyAdminsSid</description>
        ///   </item>
        ///   <item>
        ///     <description>AccountRasAndIasServersSid</description>
        ///   </item>
        /// </list></param>
        protected BaseService([CanBeNull] string description,
            WellKnownSidType sidType,
            SecurityIdentifier domainSid = null)
            : this(description, new SecurityIdentifier(sidType, domainSid))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseService" /> class.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="identity">The identity of users that can start/stop the service, defaults to world.</param>
        protected BaseService([CanBeNull] string description = null, IdentityReference identity = null)
            : base((string.IsNullOrWhiteSpace(description) || description.Length > 80) ? Description : description)
        {
            _state = ServiceState.Stopped;

            if (identity == null)
                identity = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

            EventWaitHandleSecurity sec = new EventWaitHandleSecurity();
            sec.AddAccessRule(new EventWaitHandleAccessRule(identity, EventWaitHandleRights.FullControl, AccessControlType.Allow));
            bool createdNew;
            _runEventWaitHandle = new EventWaitHandle(
                true, 
                EventResetMode.AutoReset,
                string.Format("Global\\{{{0}}} {1}", AssemblyGuid, description),
                out createdNew,
                sec);

            if (!createdNew)
                Log.Add(LoggingLevel.Warning, () => ServiceResources.Wrn_BaseService_EventHandlerAlreadyExists);
        }

        /// <summary>
        /// Runs the service, either as a service or as a console application.
        /// </summary>
        /// <param name="allowConsole">if set to <see langword="true" /> allows console interaction whilst running in a console window.</param>
        /// <param name="namedPipeConfig">The named pipe configuration, if named pipes are supported.</param>
        /// <returns>An awaitable task.</returns>
        public override Task RunAsync(bool allowConsole = true, NamedPipeConfig namedPipeConfig = null)
        {
            if (namedPipeConfig != null && namedPipeConfig.MaximumConnections > 0)
            {
                // TODO
            }

            if (IsService)
            {
                Run(this);
                return TaskResult.Completed;
            }
            lock (_lock)
            {
                // Create a task that completes when this service finally shutsdown.
                _lifeTimeTask = new TaskCompletionSource<bool>();

                // If we allow the console, connect the console UI, and wait until both tasks complete
                return allowConsole
                    ? Task.WhenAll(_lifeTimeTask.Task, ConsoleConnection.Run(this))
                    : _lifeTimeTask.Task;
            }
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Start command is sent to the service by the Service Control Manager (SCM) or when the operating system starts (for a service that starts automatically). Specifies actions to take when the service starts.
        /// </summary>
        /// <param name="args">Data passed by the start command.</param>
        // ReSharper disable once CodeAnnotationAnalyzer
        protected override sealed void OnStart([NotNull] string[] args)
        {
            using (PerfTimerStart.Region())
                lock (_lock)
                {
                    if (_state != ServiceState.Stopped)
                    {
                        Log.Add(
                            LoggingLevel.Error,
                            () => ServiceResources.Err_ServiceRunner_ServiceAlreadyRunning,
                            ServiceName);
                        return;
                    }

                    // Try to grab the global mutex
                    if (!_hasRunMutex) // Sanity check, should never have at this point!
                    {
                        try
                        {
                            _hasRunMutex = _runEventWaitHandle.WaitOne(1000, false);
                        }
                        catch (AbandonedMutexException)
                        {
                            // The mutex was abandoned by another process so we now own it.
                            _hasRunMutex = true;
                        }
                    }
                    if (!_hasRunMutex)
                        throw new ServiceException(() => ServiceResources.Err_BaseService_Failed_To_Acquire_Mutex);

                    _cancellationTokenSource = new CancellationTokenSource();
                    _pauseTokenSource.IsPaused = false;
                    _state = ServiceState.Running;
                    DoStart(args);
                }
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Stop command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service stops running.
        /// </summary>
        protected override sealed void OnStop()
        {
            using (PerfTimerStop.Region())
                lock (_lock)
                {
                    if (State != ServiceState.Running)
                    {
                        Log.Add(
                            LoggingLevel.Error,
                            () => ServiceResources.Err_ServiceRunner_Stop_ServiceNotRunning,
                            ServiceName);
                        return;
                    }
                    DoStop();
                    Contract.Assert(_cancellationTokenSource != null);
                    _state = ServiceState.Stopped;
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource = null;
                    _pauseTokenSource.IsPaused = true;

                    // Try to release the global mutex
                    if (_hasRunMutex) // This should always be true at this stage.
                    {
                        _runEventWaitHandle.Set();
                        _hasRunMutex = false;
                    }
                }
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Pause command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service pauses.
        /// </summary>
        protected override sealed void OnPause()
        {
            lock (_lock)
            {
                if (State != ServiceState.Running)
                {
                    Log.Add(
                        LoggingLevel.Error,
                        () => ServiceResources.Err_ServiceRunner_Pause_ServiceNotRunning,
                        ServiceName);
                    return;
                }
                DoPause();
                _state = ServiceState.Paused;
                _pauseTokenSource.IsPaused = true;
                PerfCounterPause.Increment();
            }
        }

        /// <summary>
        /// When implemented in a derived class, <see cref="M:System.ServiceProcess.ServiceBase.OnContinue" /> runs when a Continue command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service resumes normal functioning after being paused.
        /// </summary>
        protected override sealed void OnContinue()
        {
            lock (_lock)
            {
                if (State != ServiceState.Paused)
                {
                    Log.Add(
                        LoggingLevel.Error,
                        () => ServiceResources.Err_ServiceRunner_Continue_ServiceNotPaused,
                        ServiceName);
                    return;
                }
                _pauseTokenSource.IsPaused = false;
                _state = ServiceState.Running;
                DoContinue();
                PerfCounterContinue.Increment();
            }
        }

        /// <summary>
        /// When implemented in a derived class, executes when the system is shutting down. Specifies what should occur immediately prior to the system shutting down.
        /// </summary>
        protected override sealed void OnShutdown()
        {
            lock (_lock)
            {
                DoShutdown();
                _state = ServiceState.Shutdown;
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

                TaskCompletionSource<bool> ltt = Interlocked.Exchange(ref _lifeTimeTask, null);
                if (ltt != null)
                    ltt.TrySetResult(true);
            }
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
        /// When implemented in a derived class, executes when the computer's power status has changed. This applies to laptop computers when they go into suspended mode, which is not the same as a system shutdown.
        /// </summary>
        /// <param name="powerStatus">A <see cref="T:System.ServiceProcess.PowerBroadcastStatus" /> that indicates a notification from the system about its power status.</param>
        /// <returns>When implemented in a derived class, the needs of your application determine what value to return. For example, if a QuerySuspend broadcast status is passed, you could cause your application to reject the query by returning false.</returns>
        protected override sealed bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            lock (_lock)
            {
                bool result = DoPowerEvent(powerStatus);
                PerfCounterPowerEvent.Increment();
                return result;
            }
        }

        /// <summary>
        /// Executes when a change event is received from a Terminal Server session.
        /// </summary>
        /// <param name="changeDescription">A <see cref="T:System.ServiceProcess.SessionChangeDescription" /> structure that identifies the change type.</param>
        protected override sealed void OnSessionChange(SessionChangeDescription changeDescription)
        {
            lock (_lock)
            {
                DoSessionChange(changeDescription);
                PerfCounterSessionChange.Increment();
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
                Help(writer, commandName);
            }
            catch (Exception e)
            {
                Log.Add(e, LoggingLevel.Error, () => ServiceResources.Err_Command_Exception, commandName);
                Help(writer, commandName);
            }
        }

        /// <summary>
        /// Disconnects the specified user interface.
        /// </summary>
        /// <param name="id">The connection.</param>
        /// <returns><see langword="true" /> if disconnected, <see langword="false" /> otherwise.</returns>
        [PublicAPI]
        [ServiceCommand(typeof(ServiceResources), "Cmd_Disconnect_Names", "Cmd_Disconnect_Description",
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
                if (_hasRunMutex)
                {
                    _runEventWaitHandle.Set();
                    _hasRunMutex = false;
                }

                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource = null;
                }
                _pauseTokenSource.IsPaused = true;

                TaskCompletionSource<bool> ltt = Interlocked.Exchange(ref _lifeTimeTask, null);
                if (ltt != null)
                    ltt.TrySetResult(true);
            }
            base.Dispose(disposing);
        }
    }
}