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
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WebApplications.Utilities.Formatting;
using WebApplications.Utilities.Performance;
using WebApplications.Utilities.Service.Common.Control;
using SCP = WebApplications.Utilities.Service.ServiceCommandParameterAttribute;

namespace WebApplications.Utilities.Service
{
    /// <summary>
    /// Base implementation of a service, you should always extends the generic version of this class.
    /// </summary>
    public abstract partial class BaseService
    {
        /// <summary>
        /// Function for creating a <see cref="SessionChangeDescription"/>.
        /// </summary>
        [NotNull]
        protected static readonly Func<SessionChangeReason, int, SessionChangeDescription>
            CreateSessionChangeDescription =
                typeof(SessionChangeDescription).ConstructorFunc<SessionChangeReason, int, SessionChangeDescription>();

        /// <summary>
        /// Provides command help.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="command">Name of the command.</param>
        /// <param name="parameter">The parameter.</param>
        [PublicAPI]
        [ServiceCommand(typeof(ServiceResources), "Cmd_Help_Names", "Cmd_Help_Description")]
        protected abstract void Help(
            [NotNull] TextWriter writer,
            [CanBeNull] [SCP(typeof(ServiceResources), "Cmd_Help_Command_Description")] string command = null,
            [CanBeNull] [SCP(typeof(ServiceResources), "Cmd_Help_Parameter_Description")] string parameter = null);

        /// <summary>
        /// Install services.
        /// Sends the 
        /// <see cref="SessionChangeDescription" /> to the service.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The password.</param>
        [PublicAPI]
        [ServiceCommand(typeof(ServiceResources), "Cmd_Install_Names", "Cmd_Install_Description")]
        public abstract void Install(
            [NotNull] TextWriter writer,
            [CanBeNull] [SCP(typeof(ServiceResources), "Cmd_Install_UserName_Description")] string userName = null,
            [CanBeNull] [SCP(typeof(ServiceResources), "Cmd_Install_Password_Description")] string password = null);

        /// <summary>
        /// Uninstall services.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        [NotNull]
        [PublicAPI]
        [ServiceCommand(typeof(ServiceResources), "Cmd_Uninstall_Names", "Cmd_Uninstall_Description")]
        public abstract Task<bool> Uninstall(
            [NotNull] TextWriter writer,
            CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        [NotNull]
        [PublicAPI]
        [ServiceCommand(typeof(ServiceResources), "Cmd_Start_Names", "Cmd_Start_Description")]
        public abstract Task<bool> StartService(
            [NotNull] TextWriter writer,
            [CanBeNull] [SCP(typeof(ServiceResources), "Cmd_Start_Args_Description")] string[] args,
            CancellationToken token = default (CancellationToken));

        /// <summary>
        /// Stops this instance.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="token">The token.</param>
        [NotNull]
        [PublicAPI]
        [ServiceCommand(typeof(ServiceResources), "Cmd_Stop_Names", "Cmd_Stop_Description")]
        public abstract Task<bool> StopService(
            [NotNull] TextWriter writer,
            CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Pauses this instance.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="token">The token.</param>
        [NotNull]
        [PublicAPI]
        [ServiceCommand(typeof(ServiceResources), "Cmd_Pause_Names", "Cmd_Pause_Description")]
        public abstract Task<bool> Pause(
            [NotNull] TextWriter writer,
            CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Continues this instance.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="token">The token.</param>
        [NotNull]
        [PublicAPI]
        [ServiceCommand(typeof(ServiceResources), "Cmd_Continue_Names", "Cmd_Continue_Description")]
        public abstract Task<bool> Continue(
            [NotNull] TextWriter writer,
            CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Shuts down this instance.
        /// </summary>
        [PublicAPI]
        [ServiceCommand(typeof(ServiceResources), "Cmd_Shutdown_Names", "Cmd_Shutdown_Description")]
        public abstract bool Shutdown([NotNull] TextWriter writer);

        /// <summary>
        /// Runs the custom command on this instance.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="command">The command.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        [NotNull]
        [PublicAPI]
        [ServiceCommand(typeof(ServiceResources), "Cmd_CustomCommand_Names", "Cmd_CustomCommand_Description")]
        public abstract Task<bool> CustomCommand(
            [NotNull] TextWriter writer,
            [SCP(typeof(ServiceResources), "Cmd_CustomCommand_Command_Description")] int command,
            CancellationToken token = default (CancellationToken));

        /// <summary>
        /// Sends the <see cref="PowerBroadcastStatus" /> to the service.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="powerStatus">The power status.</param>
        /// <returns>
        ///   <see langword="true" /> if failed, or the result of the call was <see langword="true" />; <see langword="false" /> otherwise.
        /// </returns>
        [PublicAPI]
        [ServiceCommand(typeof(ServiceResources), "Cmd_PowerEvent_Names", "Cmd_PowerEvent_Description")]
        public abstract bool PowerEvent(
            [NotNull] TextWriter writer,
            [SCP(typeof(ServiceResources), "Cmd_PowerEvent_PowerStatus_Description")] PowerBroadcastStatus powerStatus);

        /// <summary>
        /// Sends the <see cref="SessionChangeDescription" /> to the service.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="changeReason">The change reason.</param>
        /// <param name="sessionId">The session identifier.</param>
        [PublicAPI]
        [ServiceCommand(typeof(ServiceResources), "Cmd_SessionChange_Names", "Cmd_SessionChange_Description")]
        public abstract void SessionChange(
            [NotNull] TextWriter writer,
            [SCP(typeof(ServiceResources), "Cmd_SessionChange_ChangeReason_Description")] SessionChangeReason
                changeReason,
            [SCP(typeof(ServiceResources), "Cmd_SessionChange_SessionID_Description")] int sessionId);

        /// <summary>
        /// Gets the details of the performance counters loaded.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="category">The category.</param>
        [PublicAPI]
        [ServiceCommand(typeof(ServiceResources), "Cmd_Performance_Names", "Cmd_Performance_Description", true)]
        public abstract void Performance(
            [NotNull] TextWriter writer,
            [CanBeNull] [SCP(typeof(ServiceResources), "Cmd_Performance_Category_Description")] string category = null);

        #region Formats
        // TODO Move the strings to resources...?
        // ReSharper disable FormatStringProblem
        /// <summary>
        /// The format to use for outputting help for all commands.
        /// </summary>
        [NotNull]
        protected static readonly FormatBuilder AllCommandsHelpFormat =
            new FormatBuilder(ServiceResources.Commands_AllCommandsHelpFormat, true);

        /// <summary>
        /// The format to use for outputting help for a single command.
        /// </summary>
        [NotNull]
        protected static readonly FormatBuilder CommandHelpFormat =
            new FormatBuilder(
                firstLineIndentSize: 4,
                indentSize: 4,
                format: ServiceResources.Commands_CommandHelpFormat,
                isReadOnly: true);

        /// <summary>
        /// The format to use for outputting help for a single command.
        /// </summary>
        [NotNull]
        protected static readonly FormatBuilder ParameterHelpFormat =
            new FormatBuilder(
                firstLineIndentSize: 4,
                indentSize: 4,
                format: ServiceResources.Commands_ParameterHelpFormat,
                isReadOnly: true);

        /// <summary>
        /// The format to use when the command name given to the help command does not exist.
        /// </summary>
        [NotNull]
        protected static readonly FormatBuilder HelpCommandNotFoundFormat =
            new FormatBuilder(ServiceResources.Command_Help_CommandNotFoundFormat, true);

        /// <summary>
        /// The format to use when the command parameter name given to the help command does not exist.
        /// </summary>
        [NotNull]
        protected static readonly FormatBuilder HelpCommandParameterNotFoundFormat =
            new FormatBuilder(ServiceResources.Command_Help_CommandParameterNotFoundFormat, true);

        /// <summary>
        /// The format for listing all performance categories.
        /// </summary>
        [NotNull]
        protected static readonly FormatBuilder AllPerformanceCategoriesFormat =
            new FormatBuilder(ServiceResources.Command_AllPerformanceCategoriesFormat, true);

        /// <summary>
        /// The format to use when the category name given to the performance command does not exist.
        /// </summary>
        [NotNull]
        protected static readonly FormatBuilder PerformanceCatergoryNotFoundFormat =
            new FormatBuilder(ServiceResources.Command_Performance_CatergoryNotFoundFormat, true);

        // ReSharper restore FormatStringProblem
        #endregion
    }

    /// <summary>
    /// Base implementation of a service.
    /// </summary>
    public abstract partial class BaseService<TService>
    {
        /// <summary>
        /// Provides command help.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="command">Name of the command.</param>
        /// <param name="parameter">The parameter.</param>
        // ReSharper disable once CodeAnnotationAnalyzer
        protected override void Help(TextWriter writer, string command = null, string parameter = null)
        {
            if (Commands.Count < 1)
            {
                writer.WriteLine(ServiceResources.BaseService_Help_NoCommands);
                writer.WriteLine();
                return;
            }

            // If no command was given, or the command they gave does not exist, then all the commands will be listed.
            ServiceCommand cmd = null;
            if (command == null ||
                !Commands.TryGetValue(command, out cmd))
            {
                // If the command they gave does not exist, show an error
                if (command != null &&
                    cmd == null)
                    HelpCommandNotFoundFormat.WriteTo(
                        writer,
                        null,
                        (_, c) =>
                            // ReSharper disable once PossibleNullReferenceException
                            string.Equals(c.Tag, "name", StringComparison.CurrentCultureIgnoreCase)
                                ? command
                                : Resolution.Unknown);

                // List all the commands
                AllCommandsHelpFormat.WriteTo(
                    writer,
                    null,
                    (_, c) =>
                        // ReSharper disable PossibleNullReferenceException
                        string.Equals(c.Tag, "commands", StringComparison.CurrentCultureIgnoreCase)
                        // ReSharper disable once AssignNullToNotNullAttribute
                            ? Commands.Values.Distinct().OrderBy(d => d.Name)
                        // ReSharper restore PossibleNullReferenceException
                            : Resolution.Unknown);
                return;
            }
            Contract.Assert(cmd != null);

            ParameterInfo parameterInfo =
                parameter == null
                    ? null
                    : cmd.ArgumentParameters.FirstOrDefault(
                // ReSharper disable once PossibleNullReferenceException
                        p => string.Equals(p.Name, parameter, StringComparison.CurrentCultureIgnoreCase));

            // If the parameter name given does not exist, show an error
            if (parameter != null &&
                parameterInfo == null)
                HelpCommandParameterNotFoundFormat.WriteTo(
                    writer,
                    null,
                    (_, c) =>
                    {
                        Contract.Assert(c != null);
                        Contract.Assert(c.Tag != null);
                        switch (c.Tag.ToLowerInvariant())
                        {
                            case "paramname":
                                return parameter;
                            case "commandname":
                                return command;
                            default:
                                return Resolution.Unknown;
                        }
                    });

            // If there was no parameter given, or it did not exist, then show all the parameters for the command
            if (parameterInfo == null)
            {
                CommandHelpFormat.WriteTo(
                    writer,
                    null,
                    (_, c) =>
                        // ReSharper disable once PossibleNullReferenceException
                        string.Equals(c.Tag, "command", StringComparison.CurrentCultureIgnoreCase)
                            ? cmd
                            : Resolution.Unknown);
                return;
            }

            // Show the details for a single parameter
            ParameterHelpFormat.WriteTo(
                writer,
                null,
                (_, c) =>
                {
                    Contract.Assert(c != null);
                    Contract.Assert(c.Tag != null);
                    switch (c.Tag.ToLowerInvariant())
                    {
                        case "command":
                            return cmd;
                        case "parameter":
                            return cmd.ResolveParameter(parameterInfo);
                        default:
                            return Resolution.Unknown;
                    }
                });
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        [PublicAPI]
        public override Task<bool> StartService(
            TextWriter writer,
            string[] args,
            CancellationToken token = default (CancellationToken))
        {
            if (args == null)
                args = new string[0];

            Contract.Assert(ServiceName != null);
            if (IsService)
                return Controller.StartService(ServiceName, token: token);

            switch (_state)
            {
                case ServiceControllerStatus.Stopped:
                    break;
                default:
                    // ReSharper disable once AssignNullToNotNullAttribute
                    writer.WriteLine(ServiceResources.Err_ServiceRunner_ServiceAlreadyRunning, ServiceName);
                    return TaskResult.False;
            }
            OnStart(args);
            return TaskResult.True;
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="token">The token.</param>
        [PublicAPI]
        public override Task<bool> StopService(TextWriter writer, CancellationToken token = default(CancellationToken))
        {
            Contract.Assert(ServiceName != null);
            if (IsService)
                return Controller.StopService(ServiceName, token);

            switch (_state)
            {
                case ServiceControllerStatus.Running:
                case ServiceControllerStatus.Paused:
                    break;
                default:
                    // ReSharper disable once AssignNullToNotNullAttribute
                    writer.WriteLine(ServiceResources.Err_ServiceRunner_Stop_ServiceNotRunning, ServiceName);
                    return TaskResult.False;
            }
            OnStop();
            return TaskResult.True;
        }

        /// <summary>
        /// Pauses this instance.
        /// </summary>
        [PublicAPI]
        public override Task<bool> Pause(TextWriter writer, CancellationToken token = default(CancellationToken))
        {
            Contract.Assert(ServiceName != null);
            if (IsService)
                return Controller.PauseService(ServiceName, token);

            if (State != ServiceControllerStatus.Running)
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                writer.WriteLine(ServiceResources.Err_ServiceRunner_Pause_ServiceNotRunning, ServiceName);
                return TaskResult.False;
            }
            OnPause();
            return TaskResult.True;
        }

        /// <summary>
        /// Continues this instance.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        [PublicAPI]
        public override Task<bool> Continue(TextWriter writer, CancellationToken token = default(CancellationToken))
        {
            Contract.Assert(ServiceName != null);
            if (IsService)
                return Controller.ContinueService(ServiceName, token);

            if (State != ServiceControllerStatus.Paused)
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                writer.WriteLine(ServiceResources.Err_ServiceRunner_Continue_ServiceNotPaused, ServiceName);
                return TaskResult.False;
            }
            OnContinue();
            return TaskResult.True;
        }

        /// <summary>
        /// Shuts down this instance.
        /// </summary>
        [PublicAPI]
        public override bool Shutdown(TextWriter writer)
        {
            if (IsService)
            {
                writer.WriteLine(
                    // ReSharper disable once AssignNullToNotNullAttribute
                    ServiceResources.Err_ServiceRunner_ServiceNotInteractive,
                    ServiceName);
                return false;
            }

            OnShutdown();
            return true;
        }

        /// <summary>
        /// Runs the custom command on this instance.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="command">The command.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        [PublicAPI]
        public override Task<bool> CustomCommand(
            TextWriter writer,
            int command,
            CancellationToken token = default (CancellationToken))
        {
            if (command < 128 ||
                command > 255)
            {
                writer.WriteLine("Service command must be between 128 and 255");
                return TaskResult.False;
            }

            Contract.Assert(ServiceName != null);
            if (IsService)
                return Controller.CommandService(ServiceName, command, token);

            OnCustomCommand(command);
            return TaskResult.True;
        }

        /// <summary>
        /// Sends the <see cref="PowerBroadcastStatus" /> to the service.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="powerStatus">The power status.</param>
        /// <returns>
        ///   <see langword="true" /> if failed, or the result of the call was <see langword="true" />; <see langword="false" /> otherwise.
        /// </returns>
        [PublicAPI]
        public override bool PowerEvent(TextWriter writer, PowerBroadcastStatus powerStatus)
        {
            lock (_lock)
            {
                if (IsService)
                {
                    writer.WriteLine(
                        // ReSharper disable once AssignNullToNotNullAttribute
                        ServiceResources.Err_ServiceRunner_ServiceNotInteractive,
                        ServiceName);
                    return true;
                }
                bool result = OnPowerEvent(powerStatus);

                writer.Write(
                    // ReSharper disable once AssignNullToNotNullAttribute
                    ServiceResources.Inf_ServiceRunner_PowerEvent_Sent,
                    powerStatus,
                    ServiceName,
                    result);
                return result;
            }
        }

        /// <summary>
        /// Sends the <see cref="SessionChangeDescription" /> to the service.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="changeReason">The change reason.</param>
        /// <param name="sessionId">The session identifier.</param>
        [PublicAPI]
        public override void SessionChange(TextWriter writer, SessionChangeReason changeReason, int sessionId)
        {
            lock (_lock)
            {
                if (IsService)
                {
                    writer.WriteLine(
                        // ReSharper disable once AssignNullToNotNullAttribute
                        ServiceResources.Err_ServiceRunner_ServiceNotInteractive,
                        ServiceName);
                    return;
                }

                OnSessionChange(CreateSessionChangeDescription(changeReason, sessionId));

                writer.WriteLine(
                    // ReSharper disable once AssignNullToNotNullAttribute
                    ServiceResources.Inf_ServiceRunner_SessionChange_Sent,
                    changeReason,
                    sessionId,
                    ServiceName);
            }
        }

        /// <summary>
        /// Install services.
        /// Sends the 
        /// <see cref="SessionChangeDescription" /> to the service.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The password.</param>
        [PublicAPI]
        public override void Install(TextWriter writer, string userName = null, string password = null)
        {
            if (!IsAdministrator)
            {
                writer.WriteLine(ServiceResources.Err_Install_Requires_Administrator);
                return;
            }

            if (IsService)
            {
                writer.WriteLine(
                    // ReSharper disable once AssignNullToNotNullAttribute
                    ServiceResources.Err_ServiceRunner_ServiceNotInteractive,
                    ServiceName);
                return;
            }

            // Validate user name/password if supplied.
            if (userName != null)
            {
                string[] unp = userName.Split('\\');
                if (unp.Length != 2 ||
                    string.IsNullOrWhiteSpace(unp[0]) ||
                    string.IsNullOrWhiteSpace(unp[1]))
                {
                    writer.WriteLine(ServiceResources.BaseService_Install_InvalidUserName);
                    return;
                }
                if (string.IsNullOrEmpty(password))
                {
                    writer.WriteLine(ServiceResources.BaseService_Install_InvalidPassword);
                    return;
                }
            }
            else
                password = null;

            writer.WriteLine(
                // ReSharper disable once AssignNullToNotNullAttribute
                ServiceResources.Inf_ServiceRunner_Install,
                ServiceName);

            string fileName = Process.GetCurrentProcess().MainModule.FileName;
            if (fileName.EndsWith(".vshost.exe", StringComparison.InvariantCultureIgnoreCase) &&
                (fileName.Length > 11))
            {
                string realFileName = fileName.Substring(0, fileName.Length - 11) + ".exe";
                if (File.Exists(realFileName))
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    writer.WriteLine(ServiceResources.BaseService_Install_vshostProcess, realFileName);
                    fileName = realFileName;
                }
            }

            // Copy service into a new directory.
            string installDirectory = Directory.CreateDirectory(
                Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.CommonProgramFiles,
                        Environment.SpecialFolderOption.Create),
                    Guid.NewGuid().ToString("D"))).FullName;

            // ReSharper disable once AssignNullToNotNullAttribute
            writer.Write(ServiceResources.BaseService_Install_CopyingService, installDirectory);

            // ReSharper disable AssignNullToNotNullAttribute
            foreach (string file in Directory.GetFiles(Path.GetDirectoryName(fileName)))
                File.Copy(file, Path.Combine(installDirectory, Path.GetFileName(file)));
            // ReSharper restore AssignNullToNotNullAttribute

            writer.WriteLine(ServiceResources.Done);

            // Change filename to new location.
            fileName = Path.Combine(installDirectory, Path.GetFileName(fileName));

            Contract.Assert(ServiceName != null);
            Controller.Install(ServiceName, DisplayName, AssemblyDescription, fileName, userName, password);
            writer.WriteLine(
                // ReSharper disable once AssignNullToNotNullAttribute
                ServiceResources.Inf_ServiceRunner_Installed,
                ServiceName,
                fileName);
        }

        /// <summary>
        /// Uninstall services.
        /// </summary>
        [PublicAPI]
        public override async Task<bool> Uninstall(
            TextWriter writer,
            CancellationToken token = default (CancellationToken))
        {
            if (!IsAdministrator)
            {
                writer.WriteLine(ServiceResources.Err_Uninstall_Requires_Administrator);
                return false;
            }
            if (IsService)
            {
                writer.WriteLine(
                    // ReSharper disable once AssignNullToNotNullAttribute
                    ServiceResources.Err_ServiceRunner_ServiceNotInteractive,
                    ServiceName);
                return false;
            }

            writer.WriteLine(
                // ReSharper disable once AssignNullToNotNullAttribute
                ServiceResources.Inf_ServiceRunner_Uninstall,
                ServiceName);
            Stopwatch s = Stopwatch.StartNew();

            Contract.Assert(ServiceName != null);
            await Controller.Uninstall(ServiceName, token).ConfigureAwait(false);

            writer.WriteLine(
                // ReSharper disable once AssignNullToNotNullAttribute
                ServiceResources.Inf_ServiceRunner_Uninstalled,
                ServiceName,
                s.Elapsed.TotalMilliseconds);
            return true;
        }

        /// <summary>
        /// Gets the details of the performance counters loaded.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="category">The category.</param>
        [PublicAPI]
        public override void Performance(TextWriter writer, string category = null)
        {
            lock (_lock)
            {
                bool categoryOmitted = string.IsNullOrWhiteSpace(category);

                PerfCategory cat = categoryOmitted
                    ? null
                    : PerfCategory.All.FirstOrDefault(
                    // ReSharper disable once PossibleNullReferenceException
                        p => string.Equals(p.CategoryName, category, StringComparison.CurrentCultureIgnoreCase));

                if (!categoryOmitted &&
                    cat == null)
                    PerformanceCatergoryNotFoundFormat.WriteTo(
                        writer,
                        null,
                        (_, c) =>
                            // ReSharper disable once PossibleNullReferenceException
                            string.Equals(c.Tag, "name", StringComparison.CurrentCultureIgnoreCase)
                                ? category
                                : Resolution.Unknown);

                if (cat == null) // Or the counter does not exist
                {
                    AllPerformanceCategoriesFormat.WriteTo(
                        writer,
                        null,
                        (_, c) =>
                        {
                            Contract.Assert(c != null);
                            Contract.Assert(c.Tag != null);
                            switch (c.Tag.ToLowerInvariant())
                            {
                                case "counters":
                                    // ReSharper disable once PossibleNullReferenceException
                                    return PerfCategory.All.OrderBy(pc => pc.CategoryName);
                                case "guid":
                                    return PerfCategory.InstanceGuid;
                                default:
                                    return Resolution.Unknown;
                            }
                        });
                    return;
                }

                cat.WriteTo(writer, PerfCategory.VerboseFormat);
                writer.WriteLine();
                writer.WriteLine();
            }
        }
    }
}