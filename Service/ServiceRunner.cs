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
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using JetBrains.Annotations;
using WebApplications.Utilities.Logging;
using WebApplications.Utilities.Logging.Loggers;

namespace WebApplications.Utilities.Service
{
    /// <summary>
    /// Allows manual control of a service when in interactive mode.
    /// </summary>
    [PublicAPI]
    public class ServiceRunner : IDisposable
    {
        /// <summary>
        /// The default format for message logs.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public const string DefaultFormat = "{+Cyan}[{TimeStamp:HH:mm:ss.ffff}] {+?}{Message}";

        /// <summary>
        /// Action for starting a service.
        /// </summary>
        [NotNull]
        private static readonly Action<ServiceBase, string[]> _onStart =
            typeof(ServiceBase).GetMethod("OnStart", BindingFlags.Instance | BindingFlags.NonPublic)
                .Action<ServiceBase, string[]>();

        /// <summary>
        /// Action for pausing a service.
        /// </summary>
        [NotNull]
        private static readonly Action<ServiceBase> _onPause =
            typeof(ServiceBase).GetMethod("OnPause", BindingFlags.Instance | BindingFlags.NonPublic)
                .Action<ServiceBase>();

        /// <summary>
        /// Action for continuing a service.
        /// </summary>
        [NotNull]
        private static readonly Action<ServiceBase> _onContinue =
            typeof(ServiceBase).GetMethod("OnContinue", BindingFlags.Instance | BindingFlags.NonPublic)
                .Action<ServiceBase>();

        /// <summary>
        /// Action for stopping a service.
        /// </summary>
        [NotNull]
        private static readonly Action<ServiceBase> _onStop =
            typeof(ServiceBase).GetMethod("OnStop", BindingFlags.Instance | BindingFlags.NonPublic)
                .Action<ServiceBase>();

        /// <summary>
        /// Action for shutting down a service.
        /// </summary>
        [NotNull]
        private static readonly Action<ServiceBase> _onShutdown =
            typeof(ServiceBase).GetMethod("OnShutdown", BindingFlags.Instance | BindingFlags.NonPublic)
                .Action<ServiceBase>();

        /// <summary>
        /// Action for running a custom command.
        /// </summary>
        [NotNull]
        private static readonly Action<ServiceBase, int> _onCustomCommand =
            typeof(ServiceBase).GetMethod("OnCustomCommand", BindingFlags.Instance | BindingFlags.NonPublic)
                .Action<ServiceBase, int>();

        /// <summary>
        /// Function for raising a power event.
        /// </summary>
        [NotNull]
        private static readonly Func<ServiceBase, PowerBroadcastStatus, bool> _onPowerEvent =
            typeof(ServiceBase).GetMethod("OnPowerEvent", BindingFlags.Instance | BindingFlags.NonPublic)
                .Func<ServiceBase, PowerBroadcastStatus, bool>();

        /// <summary>
        /// Action for raising session changed event.
        /// </summary>
        [NotNull]
        private static readonly Action<ServiceBase, SessionChangeDescription> _onSessionChange =
            typeof(ServiceBase).GetMethod("OnSessionChange", BindingFlags.Instance | BindingFlags.NonPublic)
                .Action<ServiceBase, SessionChangeDescription>();

        /// <summary>
        /// Function for creating a <see cref="SessionChangeDescription"/>.
        /// </summary>
        [NotNull]
        private static readonly Func<SessionChangeReason, int, SessionChangeDescription>
            _createSessionChangeDescription =
                typeof(SessionChangeDescription).ConstructorFunc<SessionChangeReason, int, SessionChangeDescription>();

        /// <summary>
        /// The service.
        /// </summary>
        [NotNull]
        private readonly ServiceBase _service;

        /// <summary>
        /// The lock.
        /// </summary>
        [NotNull]
        private readonly object _lock = new object();

        /// <summary>
        /// Gets the name of the service.
        /// </summary>
        /// <value>The name.</value>
        [PublicAPI]
        [NotNull]
        public string Name
        {
            get
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                return _service.ServiceName;
            }
        }

        /// <summary>
        /// Gets the current state of the service.
        /// </summary>
        /// <value>The state.</value>
        [PublicAPI]
        public ServiceState State { get; private set; }

        /// <summary>
        /// 0 means not yet run.
        /// 1 means interactive.
        /// 2 means non-interactive.
        /// 3 means disposed.
        /// </summary>
        private int _interactionState;

        /// <summary>
        /// The logger
        /// </summary>
        [CanBeNull]
        private ConsoleLogger _logger;

        /// <summary>
        /// The currently executing command objects.
        /// </summary>
        [CanBeNull]
        private Dictionary<string, Tuple<object, ServiceRunnerCommand>> _commands;

        /// <summary>
        /// Gets a value indicating whether this instance is interactive.
        /// </summary>
        /// <value><see langword="true" /> if this instance is running and interactive; otherwise, <see langword="false" />.</value>
        [PublicAPI]
        public bool IsInteractive
        {
            get { return _interactionState == 1; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRunner" /> class.
        /// </summary>
        /// <param name="service">The service.</param>
        public ServiceRunner([NotNull] ServiceBase service)
        {
            Contract.Requires<RequiredContractException>(service != null, "Parameter_Null");
            _service = service;
        }

        /// <summary>
        /// Runs this instance interactively if there is a console, otherwise runs as a service.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="commandObjects">The command objects, are objects containing commands that can be understood in interactive mode.</param>
        [PublicAPI]
        public void Run([CanBeNull] string[] args, [NotNull] params object[] commandObjects)
        {
            if (ConsoleHelper.IsConsole)
                RunInteractive(args);
            else
                RunAsService();
        }

        /// <summary>
        /// Runs this instance interactively.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="commandObjects">The command objects, are objects containing commands that can be understood in interactive mode.</param>
        [PublicAPI]
        public void RunInteractive([NotNull]IServiceUI serviceUI, [CanBeNull] string[] args, [NotNull] params object[] commandObjects)
        {
            Contract.Requires<RequiredContractException>(commandObjects != null, "Parameter_Null");
            if (!ConsoleHelper.IsConsole)
            {
                Log.Add(
                    LoggingLevel.Error,
                    () => ServiceResources.Err_ServiceRunner_RunInteractive_ConsoleNotFound,
                    Name);
                return;
            }

            Console.Title = Name;
            int height = Console.WindowHeight;
            Console.SetWindowSize(160, height);
            Console.SetBufferSize(160, 10000);
            ConsoleHelper.Maximise();

            Log.SetConsole(DefaultFormat, LoggingLevels.AtLeastInformation);
            _logger = Log.GetLoggers<ConsoleLogger>().First();
            Contract.Assert(_logger != null);

            // Get the commands for each object, in reverse order, as later objects take priority
            _commands = new Dictionary<string, Tuple<object, ServiceRunnerCommand>>(
                1 + commandObjects.Length,
                StringComparer.CurrentCultureIgnoreCase);
            foreach (object commandObject in commandObjects.Reverse().Union(new[] { this }).Where(co => co != null))
            {
                Contract.Assert(commandObject != null);
                ServiceRunnerCommands src = ServiceRunnerCommands.Get(commandObject.GetType());
                foreach (ServiceRunnerCommand command in src.AllCommands)
                {
                    Contract.Assert(command != null);
                    foreach (string commandName in command.AllNames)
                    {
                        Contract.Assert(commandName != null);
                        Tuple<object, ServiceRunnerCommand> existing;
                        if (_commands.TryGetValue(commandName, out existing))
                        {
                            Contract.Assert(existing != null);
                            Contract.Assert(existing.Item2 != null);
                            Log.Add(
                                LoggingLevel.Warning,
                                () => ServiceResources.Wrn_Command_Already_Registered,
                                commandName,
                                src.InstanceType,
                                existing.Item2.InstanceType);
                            continue;
                        }
                        _commands[commandName] = new Tuple<object, ServiceRunnerCommand>(commandObject, command);
                    }
                }
            }

            // Flush logs
            Log.Flush().Wait();

            ConsoleHelper.SynchronizationContext.Invoke(
                () =>
                {
                    ConsoleHelper.WriteLine();
                    ConsoleHelper.WriteLine(
                        "{+Magenta}Running the '{0}' service in interactive mode, type 'help' for help.",
                        Name);
                    ConsoleHelper.WriteLine();
                });

            do
            {
                // Flush logs
                Log.Flush().Wait();

                // Write out prompt
                ConsoleHelper.SynchronizationContext.Invoke(
                    () =>
                    {
                        ConsoleHelper.WriteLine();
                        ConsoleColor serviceStatus;
                        switch (State)
                        {
                            case ServiceState.Stopped:
                                serviceStatus = ConsoleColor.Red;
                                break;
                            case ServiceState.Running:
                                serviceStatus = ConsoleColor.Green;
                                break;
                            case ServiceState.Paused:
                                serviceStatus = ConsoleColor.Yellow;
                                break;
                            default:
                                serviceStatus = ConsoleColor.White;
                                break;
                        }
                        ConsoleHelper.SetCustomColourName("ServiceStatus", serviceStatus);
                        ConsoleHelper.Write(
                            "{+Cyan}[{0:HH:mm:ss.ffff}] {+ServiceStatus}{1} {+_}> ",
                            DateTime.UtcNow,
                            State);
                    });

                string line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;

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

                // Lookup the command
                Contract.Assert(!string.IsNullOrWhiteSpace(commandName));

                Tuple<object, ServiceRunnerCommand> tuple;
                if (!_commands.TryGetValue(commandName, out tuple))
                {
                    ConsoleHelper.SynchronizationContext.Invoke(
                        () =>
                        {
                            ConsoleHelper.WriteLine(
                                "{+Red}'{0}' is not a recognised command, type 'help' for help.",
                                commandName);
                            ConsoleHelper.WriteLine();
                        });
                    Help();
                    continue;
                }

                // Run the command
                Contract.Assert(tuple != null);
                Contract.Assert(tuple.Item1 != null);
                Contract.Assert(tuple.Item2 != null);

                try
                {
                    if (!tuple.Item2.Run(tuple.Item1, line))
                    {
                        ConsoleHelper.SynchronizationContext.Invoke(
                            () =>
                            {
                                ConsoleHelper.WriteLine(
                                    "{+Red}'{0}' failed to execute.",
                                    commandName);
                                ConsoleHelper.WriteLine();
                            });
                        Help(commandName);
                    }
                }
                catch (Exception e)
                {
                    Log.Add(e, LoggingLevel.Error, () => ServiceResources.Err_Comman_Exception, commandName);
                    Help(commandName);
                }
            } while (State != ServiceState.Shutdown);
            _commands = null;
        }

        /// <summary>
        /// Runs the service as a service.
        /// </summary>
        [PublicAPI]
        public void RunAsService()
        {
            int previousState = Interlocked.CompareExchange(ref _interactionState, 2, 0);
            if (previousState != 0)
            {
                Log.Add(LoggingLevel.Error, () => ServiceResources.Err_ServiceRunner_ServiceAlreadyRunning, Name);
                return;
            }
            Log.Add(
                LoggingLevel.Information,
                () => ServiceResources.Inf_ServiceRunner_RunAsService_ServiceNotInteractive,
                Name);
            ServiceBase.Run(_service);
        }

        /// <summary>
        /// Writes out help.
        /// </summary>
        [PublicAPI]
        [ServiceRunnerCommand(typeof(ServiceResources), "Cmd_Help_Names", "Cmd_Help_Description")]
        public void Help(string command = null, string parameter = null)
        {
            Log.Flush().Wait();

            ConsoleHelper.SynchronizationContext.Invoke(
                () =>
                {
                    if (_commands == null)
                    {
                        ConsoleHelper.WriteLine("There are no commands currently registered!");
                        ConsoleHelper.WriteLine();
                        return;
                    }

                    if (command == null)
                    {
                        ConsoleHelper.WriteLine("The following commands are available:");
                        ConsoleHelper.WriteLine();
                        // List all commands
                        foreach (ServiceRunnerCommand src in _commands.Select(kvp => kvp.Value.Item2).Distinct())
                        {
                            Contract.Assert(src != null);
                            string[] names = src.AllNames.ToArray();
                            ConsoleHelper.Write(
                                "{+Green}{0}",
                                names[0]);
                            if (names.Length > 1)
                                ConsoleHelper.Write("{+DarkGreen}[{0}]", string.Join("|", names.Skip(1)));

                            foreach (ParameterInfo p in src.Parameters)
                            {
                                ConsoleHelper.Write(" {+White}{0}", p.Name);
                                if (p.HasDefaultValue)
                                {
                                    object dv = p.RawDefaultValueSafe();
                                    ConsoleHelper.Write("{+Gray}={0}", dv == null ? "null" : "'" + dv.ToString() + "'");
                                }
                                else if (p.ParameterType == typeof(string[]))
                                    ConsoleHelper.Write("{+Gray}...");
                            }

                            ConsoleHelper.WriteLine();
                            ConsoleHelper.WriteLine("\t" + src.Description);
                            ConsoleHelper.WriteLine();
                        }
                        ConsoleHelper.WriteLine("Type help <command> for more information.");
                    }

                    // TODO

                    /*
                    ConsoleHelper.WriteLine("The following commands are available:");

                    ConsoleHelper.WriteLine();
                    ConsoleHelper.WriteLine(
                        "{+Green}[start|g] <args...>\t{+_}Starts the service, passing in the specified arguments.");
                    ConsoleHelper.WriteLine("\t\t\te.g. start arg1 arg2 arg3");

                    ConsoleHelper.WriteLine();
                    ConsoleHelper.WriteLine("{+Green}[stop|s]\t\t{+_}Stops the service.");

                    ConsoleHelper.WriteLine();
                    ConsoleHelper.WriteLine("{+Green}[pause|p]\t\t{+_}Pauses the service.");

                    ConsoleHelper.WriteLine();
                    ConsoleHelper.WriteLine("{+Green}[continue|c]\t\t{+_}Continue the service.");

                    ConsoleHelper.WriteLine();
                    ConsoleHelper.WriteLine("{+Green}[command|.] <int>\t{+_}Executes the custom command number.");
                    ConsoleHelper.WriteLine("\t\t\te.g. command 54");

                    ConsoleHelper.WriteLine();
                    ConsoleHelper.WriteLine("{+Green}power <status>\t\t{+_}Sends the power status to the service.");
                    ConsoleHelper.WriteLine("\t\t\tStatus is one of -");
                    ConsoleHelper.WriteLine("\t\t\t  {+Cyan}QuerySuspend, QuerySuspendFailed, Suspend,");
                    ConsoleHelper.WriteLine("\t\t\t  {+Cyan}ResumeCritical, ResumeSuspend, BatteryLow,");
                    ConsoleHelper.WriteLine("\t\t\t  {+Cyan}PowerStatusChange, OemEvent, ResumeAutomatic");
                    ConsoleHelper.WriteLine("\t\t\te.g. power Suspend");

                    ConsoleHelper.WriteLine();
                    ConsoleHelper.WriteLine("{+Green}session <reason> <int>\t{+_}Executes the custom command number.");
                    ConsoleHelper.WriteLine("\t\t\tReason is one of -");
                    ConsoleHelper.WriteLine("\t\t\t  {+Cyan}ConsoleConnect, ConsoleDisconnect, RemoteConnect,");
                    ConsoleHelper.WriteLine("\t\t\t  {+Cyan}RemoteDisconnect, SessionLogon, SessionLogoff,");
                    ConsoleHelper.WriteLine("\t\t\t  {+Cyan}SessionLock, SessionUnlock, SessionRemoteControl");
                    ConsoleHelper.WriteLine("\t\t\te.g. session SessionLogon 0");

                    ConsoleHelper.WriteLine();
                    ConsoleHelper.WriteLine("{+Green}[shutdown|x|quit|exit]\t{+_}Shutdowns the service and quits.");

                    ConsoleHelper.WriteLine();
                    ConsoleHelper.WriteLine(
                        "{+Green}[logformat|lf] <format>\t{+_}Sets the log format (if supplied); or gets it.");
                    ConsoleHelper.WriteLine("\t\t\tUse default to restore the default format");

                    ConsoleHelper.WriteLine();
                    ConsoleHelper.WriteLine(
                        "{+Green}[loglevels|ll] <levels>\t{+_}Sets the log levels (if supplied); or gets them.");
                    ConsoleHelper.WriteLine("\t\t\tLevels is any combination of -");
                    ConsoleHelper.WriteLine("\t\t\t  {+Cyan}All, None, AtLeastCritical, AtLeastError,");
                    ConsoleHelper.WriteLine("\t\t\t  {+Cyan}AtLeastWarning, AtLeastSystemNotification,");
                    ConsoleHelper.WriteLine("\t\t\t  {+Cyan}AtLeastNotification, AtLeastInformation, Emergency");
                    ConsoleHelper.WriteLine("\t\t\t  {+Cyan}Critical, Error, Warning, SystemNotification");
                    ConsoleHelper.WriteLine("\t\t\t  {+Cyan}Notification, Information, Debugging");
                    ConsoleHelper.WriteLine("\t\t\te.g. loglevels AtLeastWarning");

                    ConsoleHelper.WriteLine();
                    ConsoleHelper.WriteLine(
                        "{+Green}[perf|p] <command> <options>\t{+_}Access performance counters.");
                    ConsoleHelper.WriteLine("\t\t\tCommand is one of -");
                    ConsoleHelper.WriteLine("\t\t\t  {+Cyan}All, None, AtLeastCritical, AtLeastError,");
                    ConsoleHelper.WriteLine("\t\t\t  {+Cyan}AtLeastWarning, AtLeastSystemNotification,");
                    ConsoleHelper.WriteLine("\t\t\t  {+Cyan}AtLeastNotification, AtLeastInformation, Emergency");
                    ConsoleHelper.WriteLine("\t\t\t  {+Cyan}Critical, Error, Warning, SystemNotification");
                    ConsoleHelper.WriteLine("\t\t\t  {+Cyan}Notification, Information, Debugging");
                    ConsoleHelper.WriteLine("\t\t\te.g. loglevels AtLeastWarning");

                    ConsoleHelper.WriteLine();
                    ConsoleHelper.WriteLine("{+Green}[help|?]\t\t{+_}Outputs this help.");
                     */
                });
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <param name="args">The arguments.</param>
        [PublicAPI]
        [ServiceRunnerCommand(typeof(ServiceResources), "Cmd_Start_Names", "Cmd_Start_Description")]
        public void Start([CanBeNull] string[] args)
        {
            lock (_lock)
            {
                int interactionState = Interlocked.CompareExchange(ref _interactionState, 1, 0);
                if (interactionState > 1)
                {
                    Log.Add(LoggingLevel.Error, () => ServiceResources.Err_ServiceRunner_ServiceNotInteractive, Name);
                    return;
                }

                if (State != ServiceState.Stopped)
                {
                    Log.Add(LoggingLevel.Error, () => ServiceResources.Err_ServiceRunner_ServiceAlreadyRunning, Name);
                    return;
                }

                Log.Add(LoggingLevel.Information, () => ServiceResources.Inf_ServiceRunner_Start_Starting, Name);
                Stopwatch s = Stopwatch.StartNew();
                try
                {
                    _onStart(_service, args);
                    Log.Add(
                        LoggingLevel.Information,
                        () => ServiceResources.Inf_ServiceRunner_Start_Started,
                        Name,
                        s.Elapsed.TotalMilliseconds);
                    State = ServiceState.Running;
                }
                catch (TargetInvocationException exception)
                {
                    Contract.Assert(exception.InnerException != null);
                    Log.Add(exception.InnerException);
                }
            }
        }

        /// <summary>
        /// Continues this instance.
        /// </summary>
        [PublicAPI]
        [ServiceRunnerCommand(typeof(ServiceResources), "Cmd_Continue_Names", "Cmd_Continue_Description")]
        protected void Continue()
        {
            lock (_lock)
            {
                int interactionState = Interlocked.CompareExchange(ref _interactionState, 1, 0);
                if (interactionState > 1)
                {
                    Log.Add(LoggingLevel.Error, () => ServiceResources.Err_ServiceRunner_ServiceNotInteractive, Name);
                    return;
                }

                if (State != ServiceState.Paused)
                {
                    Log.Add(
                        LoggingLevel.Error,
                        () => ServiceResources.Err_ServiceRunner_Continue_ServiceNotPaused,
                        Name);
                    return;
                }
                if (!_service.CanPauseAndContinue)
                {
                    Log.Add(
                        LoggingLevel.Information,
                        () => ServiceResources.Inf_ServiceRunner_Continue_NotSupported,
                        Name); // TODO Should this be an error?
                    return;
                }

                Log.Add(LoggingLevel.Information, () => ServiceResources.Inf_ServiceRunner_Continue_Continuing, Name);
                Stopwatch s = Stopwatch.StartNew();
                try
                {
                    _onContinue(_service);
                    Log.Add(
                        LoggingLevel.Information,
                        () => ServiceResources.Inf_ServiceRunner_Continue_Continued,
                        Name,
                        s.Elapsed.TotalMilliseconds);
                    State = ServiceState.Running;
                }
                catch (TargetInvocationException exception)
                {
                    Contract.Assert(exception.InnerException != null);
                    Log.Add(exception.InnerException);
                }
            }
        }

        /// <summary>
        /// Pauses this instance.
        /// </summary>
        [PublicAPI]
        [ServiceRunnerCommand(typeof(ServiceResources), "Cmd_Pause_Names", "Cmd_Pause_Description")]
        protected void Pause()
        {
            lock (_lock)
            {
                int interactionState = Interlocked.CompareExchange(ref _interactionState, 1, 0);
                if (interactionState > 1)
                {
                    Log.Add(LoggingLevel.Error, () => ServiceResources.Err_ServiceRunner_ServiceNotInteractive, Name);
                    return;
                }

                if (State != ServiceState.Running)
                {
                    Log.Add(LoggingLevel.Error, () => ServiceResources.Err_ServiceRunner_Pause_ServiceNotRunning, Name);
                    return;
                }
                if (!_service.CanPauseAndContinue)
                {
                    Log.Add(LoggingLevel.Information, () => ServiceResources.Inf_ServiceRunner_Pause_NotSupported, Name);
                    // TODO should this be an error?
                    return;
                }

                Log.Add(LoggingLevel.Information, () => ServiceResources.Inf_ServiceRunner_Pause_Pausing, Name);
                Stopwatch s = Stopwatch.StartNew();
                try
                {
                    _onPause(_service);
                    Log.Add(
                        LoggingLevel.Information,
                        () => ServiceResources.Inf_ServiceRunner_Pause_Paused,
                        Name,
                        s.Elapsed.TotalMilliseconds);
                    State = ServiceState.Paused;
                }
                catch (TargetInvocationException exception)
                {
                    Contract.Assert(exception.InnerException != null);
                    Log.Add(exception.InnerException);
                }
            }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        [PublicAPI]
        [ServiceRunnerCommand(typeof(ServiceResources), "Cmd_Stop_Names", "Cmd_Stop_Description")]
        protected void Stop()
        {
            lock (_lock)
            {
                int interactionState = Interlocked.CompareExchange(ref _interactionState, 1, 0);
                if (interactionState > 1)
                {
                    Log.Add(LoggingLevel.Error, () => ServiceResources.Err_ServiceRunner_ServiceNotInteractive, Name);
                    return;
                }

                if (State != ServiceState.Running)
                {
                    Log.Add(LoggingLevel.Error, () => ServiceResources.Err_ServiceRunner_Stop_ServiceNotRunning, Name);
                    return;
                }
                if (!_service.CanStop)
                {
                    Log.Add(LoggingLevel.Information, () => ServiceResources.Inf_ServiceRunner_Stop_NotSupported, Name);
                    // TODO should this be an error?
                    return;
                }

                Log.Add(LoggingLevel.Information, () => ServiceResources.Inf_ServiceRunner_Stop_Stopping, Name);
                Stopwatch s = Stopwatch.StartNew();
                try
                {
                    _onStop(_service);
                    Log.Add(
                        LoggingLevel.Information,
                        () => ServiceResources.Inf_ServiceRunner_Stop_Stopped,
                        Name,
                        s.Elapsed.TotalMilliseconds);
                    State = ServiceState.Stopped;
                }
                catch (TargetInvocationException exception)
                {
                    Contract.Assert(exception.InnerException != null);
                    Log.Add(exception.InnerException);
                }
            }
        }

        /// <summary>
        /// Shuts down this instance.
        /// </summary>
        [PublicAPI]
        [ServiceRunnerCommand(typeof(ServiceResources), "Cmd_Shutdown_Names", "Cmd_Shutdown_Description")]
        protected void Shutdown()
        {
            lock (_lock)
            {
                int interactionState = Interlocked.CompareExchange(ref _interactionState, 1, 0);
                if (interactionState > 1)
                {
                    Log.Add(LoggingLevel.Error, () => ServiceResources.Err_ServiceRunner_ServiceNotInteractive, Name);
                    return;
                }

                Log.Add(LoggingLevel.Information, () => ServiceResources.Inf_ServiceRunner_Shutdown_ShuttingDown, Name);
                Stopwatch s = Stopwatch.StartNew();
                try
                {
                    if (!_service.CanShutdown)
                        Log.Add(
                            LoggingLevel.Information,
                            () => ServiceResources.Inf_ServiceRunner_Shutdown_NotSupported,
                            Name); // TODO Should this be an error?
                    else
                    {
                        _onShutdown(_service);
                        Log.Add(
                            LoggingLevel.Information,
                            () => ServiceResources.Inf_ServiceRunner_Shutdown_ShutDown,
                            Name,
                            s.Elapsed.TotalMilliseconds);
                    }
                }
                catch (TargetInvocationException exception)
                {
                    Contract.Assert(exception.InnerException != null);
                    Log.Add(exception.InnerException);
                }
                // Mark for shutdown
                State = ServiceState.Shutdown;
            }
        }

        /// <summary>
        /// Runs the custom command on this instance.
        /// </summary>
        /// <param name="command">The command.</param>
        [PublicAPI]
        [ServiceRunnerCommand(typeof(ServiceResources), "Cmd_CustomCommand_Names", "Cmd_CustomCommand_Description")]
        protected void CustomCommand(int command)
        {
            lock (_lock)
            {
                int interactionState = Interlocked.CompareExchange(ref _interactionState, 1, 0);
                if (interactionState > 1)
                {
                    Log.Add(LoggingLevel.Error, () => ServiceResources.Err_ServiceRunner_ServiceNotInteractive, Name);
                    return;
                }

                Log.Add(
                    LoggingLevel.Information,
                    () => ServiceResources.Inf_ServiceRunner_CustomCommand_Running,
                    command,
                    Name);
                Stopwatch s = Stopwatch.StartNew();
                try
                {
                    _onCustomCommand(_service, command);
                    Log.Add(
                        LoggingLevel.Information,
                        () => ServiceResources.Inf_ServiceRunner_CustomCommand_Complete,
                        command,
                        Name,
                        s.Elapsed.TotalMilliseconds);
                }
                catch (TargetInvocationException exception)
                {
                    Contract.Assert(exception.InnerException != null);
                    Log.Add(exception.InnerException);
                }
            }
        }

        /// <summary>
        /// Sends the <see cref="PowerBroadcastStatus"/> to the service.
        /// </summary>
        /// <param name="powerStatus">The power status.</param>
        /// <returns><see langword="true" /> if failed, or the result of the call was <see langword="true"/>; <see langword="false" /> otherwise.</returns>
        [PublicAPI]
        [ServiceRunnerCommand(typeof(ServiceResources), "Cmd_PowerEvent_Names", "Cmd_PowerEvent_Description")]
        protected bool PowerEvent(PowerBroadcastStatus powerStatus)
        {
            lock (_lock)
            {
                int interactionState = Interlocked.CompareExchange(ref _interactionState, 1, 0);
                if (interactionState > 1)
                {
                    Log.Add(LoggingLevel.Error, () => ServiceResources.Err_ServiceRunner_ServiceNotInteractive, Name);
                    return true;
                }

                if (!_service.CanHandlePowerEvent)
                {
                    Log.Add(
                        LoggingLevel.Information,
                        () => ServiceResources.Inf_ServiceRunner_PowerEvent_NotSupported,
                        Name); // TODO should this be an error?
                    return true;
                }

                Log.Add(
                    LoggingLevel.Information,
                    () => ServiceResources.Inf_ServiceRunner_PowerEvent_Sending,
                    powerStatus,
                    Name);
                Stopwatch s = Stopwatch.StartNew();
                try
                {
                    bool result = _onPowerEvent(_service, powerStatus);
                    Log.Add(
                        LoggingLevel.Information,
                        () => ServiceResources.Inf_ServiceRunner_PowerEvent_Sent,
                        powerStatus,
                        Name,
                        s.Elapsed.TotalMilliseconds,
                        result);
                    return result;
                }
                catch (TargetInvocationException exception)
                {
                    Contract.Assert(exception.InnerException != null);
                    Log.Add(exception.InnerException);
                    return true;
                }
            }
        }

        /// <summary>
        /// Sends the <see cref="SessionChangeDescription" /> to the service.
        /// </summary>
        /// <param name="changeReason">The change reason.</param>
        /// <param name="sessionId">The session identifier.</param>
        [PublicAPI]
        [ServiceRunnerCommand(typeof(ServiceResources), "Cmd_SessionChange_Names", "Cmd_SessionChange_Description")]
        protected void SessionChange(SessionChangeReason changeReason, int sessionId)
        {
            lock (_lock)
            {
                int interactionState = Interlocked.CompareExchange(ref _interactionState, 1, 0);
                if (interactionState > 1)
                {
                    Log.Add(LoggingLevel.Error, () => ServiceResources.Err_ServiceRunner_ServiceNotInteractive, Name);
                    return;
                }

                if (!_service.CanHandleSessionChangeEvent)
                {
                    Log.Add(
                        LoggingLevel.Information,
                        () => ServiceResources.Inf_ServiceRunner_SessionChange_NotSupported,
                        Name); // TODO should this be an error?
                    return;
                }

                Log.Add(
                    LoggingLevel.Information,
                    () => ServiceResources.Inf_ServiceRunner_SessionChange_Sending,
                    changeReason,
                    sessionId,
                    Name);
                Stopwatch s = Stopwatch.StartNew();
                try
                {
                    _onSessionChange(_service, _createSessionChangeDescription(changeReason, sessionId));
                    Log.Add(
                        LoggingLevel.Information,
                        () => ServiceResources.Inf_ServiceRunner_SessionChange_Sent,
                        changeReason,
                        sessionId,
                        Name,
                        s.Elapsed.TotalMilliseconds);
                }
                catch (TargetInvocationException exception)
                {
                    Contract.Assert(exception.InnerException != null);
                    Log.Add(exception.InnerException);
                }
            }
        }

        /// <summary>
        /// Sends the <see cref="SessionChangeDescription" /> to the service.
        /// </summary>
        /// <param name="format">The format, if any; otherwise <see langword="null"/> to output current format.</param>
        [PublicAPI]
        [ServiceRunnerCommand(typeof(ServiceResources), "Cmd_LogFormat_Names", "Cmd_LogFormat_Description", true)]
        protected void LogFormat([CanBeNull] string format = null)
        {
            if (_logger == null)
            {
                Log.Add(LoggingLevel.Error, () => ServiceResources.Err_No_Console_Logger);
                return;
            }

            if (!string.IsNullOrEmpty(format))
            {
                format = format.Unescape();
                if (string.Equals(format, "default", StringComparison.InvariantCultureIgnoreCase))
                    format = DefaultFormat;
                _logger.Format = format;
            }
            else
                ConsoleHelper.WriteLine("Current format:\r\n{+White}{0}", _logger.Format.Escape());
        }

        /// <summary>
        /// Sends the <see cref="SessionChangeDescription" /> to the service.
        /// </summary>
        /// <param name="levels">The new <see cref="LoggingLevels"/>, if any; otherwise <see langword="null"/> to output current levels.</param>
        [PublicAPI]
        [ServiceRunnerCommand(typeof(ServiceResources), "Cmd_LogLevels_Names", "Cmd_LogLevels_Description")]
        protected void LogLevels([CanBeNull] LoggingLevels? levels = null)
        {
            if (_logger == null)
            {
                Log.Add(() => ServiceResources.Err_No_Console_Logger);
                return;
            }

            if (levels != null)
                _logger.ValidLevels = levels.Value;
            else
                ConsoleHelper.WriteLine("Current logging levels:\r\n{+White}{0}", Log.ValidLevels);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            int state = Interlocked.Exchange(ref _interactionState, 3);
            if (state > 2)
            {
                Log.Add(
                    LoggingLevel.Information,
                    () => ServiceResources.Inf_ServiceRunner_Dispose_AlreadyDisposed,
                    Name);
                return;
            }

            Log.Add(LoggingLevel.Information, () => ServiceResources.Inf_ServiceRunner_Dispose_Disposing, Name);
            Stopwatch s = Stopwatch.StartNew();
            try
            {
                _service.Dispose();
                Log.Add(
                    LoggingLevel.Information,
                    () => ServiceResources.Inf_ServiceRunner_Dispose_Disposed,
                    Name,
                    s.Elapsed.TotalMilliseconds);
                State = ServiceState.Running;
            }
            catch (TargetInvocationException exception)
            {
                Contract.Assert(exception.InnerException != null);
                Log.Add(exception.InnerException);
            }
        }
    }
}