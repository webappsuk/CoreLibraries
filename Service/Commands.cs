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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using JetBrains.Annotations;
using WebApplications.Utilities.Formatting;
using WebApplications.Utilities.Logging;
using WebApplications.Utilities.Logging.Loggers;
using WebApplications.Utilities.Performance;
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

        #region Formats
        // ReSharper disable FormatStringProblem
        /// <summary>
        /// The format to use for outputting help for all commands.
        /// </summary>
        [NotNull]
        protected static readonly FormatBuilder AllCommandsHelpFormat =
            new FormatBuilder()
                .AppendForegroundColor(Color.White)
                .AppendLine("The following commands are available:")
                .AppendResetForegroundColor()
                .AppendLine()
                .AppendFormatLine(
                    "{commands:{<items>:{<item>:" +
                    "{!fgcolor:Lime}{Name}{AltNames:{!fgcolor:Green} [{<items>:{<item>}}{<join>:|}]}{!fgcolor}\r\n" +
                    "{!layout:f4;i4}{Parameters:{<items>:{<item>:" +
                    "{!fgcolor:White}{Name}{DefaultValue:{!fgcolor:Silver}={DefaultValue}}{!fgcolor}{Params:{Params}...}\r\n" +
                    "}}}{!layout}{!layout:f8;i8}{Description}{!layout}}}{<join>:\r\n}}")
                .AppendLine()
                .AppendFormatLine("Type 'help {!fgcolor:Lime}<command>{!fgcolor}' for more information on a specific command.")
                .AppendLine()
                .MakeReadOnly();

        /// <summary>
        /// The format to use for outputting help for a single command.
        /// </summary>
        [NotNull]
        protected static readonly FormatBuilder CommandHelpFormat =
            new FormatBuilder(Optional<int>.Unassigned, firstLineIndentSize: 4, indentSize: 4)
                .AppendFormatLine(
                    "{!layout:f0}{!fgcolor:White}Help for the {!fgcolor:Lime}'{Command:{Name}}'{!fgcolor:White} command.{!fgcolor}{!layout}")
                .AppendFormat(
                    "{Command:{AltNames:{!layout:f0}Alternate names:\r\n{!layout}{!fgcolor:White}{<items>:{<item>}}{<join>:, }{!fgcolor}\r\n}}")
                .AppendLine()
                .AppendLayout(firstLineIndentSize: 0)
                .AppendLine("Description: ")
                .AppendPopLayout()
                .AppendFormatLine("{Command:{Description}}")
                .AppendLine()
                .AppendFormat(
                    "{Command:{Parameters:{!layout:f0}Parameters:{!layout}\r\n{<items>:" +
                    "{<item>:{!fgcolor:White}" +
                    "{Name}" +
                    "{DefaultValue:{!fgcolor:Silver}={DefaultValue}}{!fgcolor}" +
                    "{Params:{Params}...}" +
                    "{Description:\r\n{!layout:f8;i10}{Description}{!layout}}\r\n" +
                    "}}" +
                    "\r\n{!layout:f0}Type 'help {!fgcolor:Lime}{Command:{Name}} {!fgcolor:White}<parameter>{!fgcolor}' for more information on a specific parameter of the command.\r\n" +
                    "}}")
                .MakeReadOnly();

        /// <summary>
        /// The format to use for outputting help for a single command.
        /// </summary>
        [NotNull]
        protected static readonly FormatBuilder ParameterHelpFormat =
            new FormatBuilder(Optional<int>.Unassigned, firstLineIndentSize: 4, indentSize: 4)
                .AppendFormatLine(
                    "{!layout:f0}{!fgcolor:White}Help for the {!fgcolor:Lime}'{Parameter:{Name}}'{!fgcolor:White} parameter for the {!fgcolor:Lime}'{Command:{Name}}'{!fgcolor:White} command.{!fgcolor}{!layout}")
                .AppendFormat(
                    "{Parameter:{DefaultValue:{!layout:f0}Default value:{!layout}{!fgcolor:White}{DefaultValue}{!fgcolor}\r\n}}")
                .AppendLine()
                .AppendLayout(firstLineIndentSize: 0)
                .AppendLine("Description:")
                .AppendPopLayout()
                .AppendFormatLine("{Parameter:{Description}}")
                .AppendLine()
                .MakeReadOnly();

        /// <summary>
        /// The format to use when the command name given to the help command does not exist.
        /// </summary>
        [NotNull]
        protected static readonly FormatBuilder HelpCommandNotFoundFormat =
            new FormatBuilder()
                .AppendForegroundColor(Color.Red)
                .AppendFormatLine("The command '{Name}' does not exist.")
                .AppendLine()
                .MakeReadOnly();

        /// <summary>
        /// The format to use when the command parameter name given to the help command does not exist.
        /// </summary>
        [NotNull]
        protected static readonly FormatBuilder HelpCommandParameterNotFoundFormat =
            new FormatBuilder()
                .AppendForegroundColor(Color.Red)
                .AppendFormatLine("The parameter '{ParamName}' does not exist on the '{CommandName}' command.")
                .AppendLine()
                .MakeReadOnly();

        /// <summary>
        /// The format for listing all performance categories.
        /// </summary>
        [NotNull]
        protected static readonly FormatBuilder AllPerformanceCategoriesFormat =
            new FormatBuilder()
                .AppendForegroundColor(Color.White)
                .AppendLine("The following performance categories are loaded:")
                .AppendLine()
                .AppendForegroundColor(Color.Yellow)
                .AppendFormatLine(
                    "{counters:{<items>:{<item>:{CategoryName}}}{<join>:\r\n}}")
                .AppendResetForegroundColor()
                .AppendLine()
                .AppendFormatLine("Type 'perf {!fgcolor:Yellow}<category>{!fgcolor}' for more details of a specific counter.")
                .AppendLine()
                .MakeReadOnly();

        /// <summary>
        /// The format to use when the category name given to the performance command does not exist.
        /// </summary>
        [NotNull]
        protected static readonly FormatBuilder PerformanceCatergoryNotFoundFormat =
            new FormatBuilder()
                .AppendForegroundColor(Color.Red)
                .AppendFormatLine("The command '{Name}' does not exist.")
                .AppendLine()
                .MakeReadOnly();

        // ReSharper restore FormatStringProblem, FormatStringProblem
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
                writer.WriteLine("There are no commands registered");
                writer.WriteLine();
                return;
            }

            // If no command was given, or the command they gave does not exist, then all the commands will be listed.
            ServiceCommand cmd = null;
            if (command == null ||
                !Commands.TryGetValue(command, out cmd))
            {
                // If the command they gave does not exist, show an error
                if (command != null && cmd == null)
                {
                    HelpCommandNotFoundFormat.WriteTo(
                        writer,
                        null,
                        (_, c) =>
                            string.Equals(c.Tag, "name", StringComparison.CurrentCultureIgnoreCase)
                                ? command
                                : Resolution.Unknown);
                }

                // List all the commands
                AllCommandsHelpFormat.WriteTo(
                    writer,
                    null,
                    (_, c) =>
                        string.Equals(c.Tag, "commands", StringComparison.CurrentCultureIgnoreCase)
                            ? Commands.Values.Distinct().OrderBy(d => d.Name)
                            : Resolution.Unknown);
                return;
            }
            Contract.Assert(cmd != null);

            ParameterInfo parameterInfo =
                parameter == null
                    ? null
                    : cmd.ArgumentParameters.FirstOrDefault(
                        p => string.Equals(p.Name, parameter, StringComparison.CurrentCultureIgnoreCase));

            // If the parameter name given does not exist, show an error
            if (parameter != null && parameterInfo == null)
            {
                HelpCommandParameterNotFoundFormat.WriteTo(
                    writer,
                    null,
                    (_, c) =>
                    {
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
            }

            // If there was no parameter given, or it did not exist, then show all the parameters for the command
            if (parameterInfo == null)
            {
                CommandHelpFormat.WriteTo(
                    writer,
                    null,
                    (_, c) =>
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
        /// <param name="args">The arguments.</param>
        [PublicAPI]
        [ServiceCommand(typeof(ServiceResources), "Cmd_Start_Names", "Cmd_Start_Description")]
        public void Start([CanBeNull] [SCP(typeof(ServiceResources), "Cmd_Start_Args_Description")] string[] args)
        {
            lock (_lock)
            {
                if (State != ServiceState.Stopped)
                {
                    Log.Add(
                        LoggingLevel.Error,
                        () => ServiceResources.Err_ServiceRunner_ServiceAlreadyRunning,
                        ServiceName);
                    return;
                }

                if (args == null)
                    args = new string[0];

                Log.Add(LoggingLevel.Information, () => ServiceResources.Inf_ServiceRunner_Start_Starting, ServiceName);
                Stopwatch s = Stopwatch.StartNew();
                try
                {
                    if (IsService)
                    {
                        Contract.Assert(ServiceController != null);
                        ServiceController.Start(args);
                    }
                    else
                        OnStart(args);

                    Log.Add(
                        LoggingLevel.Information,
                        () => ServiceResources.Inf_ServiceRunner_Start_Started,
                        ServiceName,
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
        /// Stops this instance.
        /// </summary>
        [PublicAPI]
        [ServiceCommand(typeof(ServiceResources), "Cmd_Stop_Names", "Cmd_Stop_Description")]
        public void StopService()
        {
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

                Log.Add(LoggingLevel.Information, () => ServiceResources.Inf_ServiceRunner_Stop_Stopping, ServiceName);
                Stopwatch s = Stopwatch.StartNew();
                try
                {
                    if (IsService)
                    {
                        Contract.Assert(ServiceController != null);
                        ServiceController.Stop();
                    }
                    else
                        OnStop();
                    Log.Add(
                        LoggingLevel.Information,
                        () => ServiceResources.Inf_ServiceRunner_Stop_Stopped,
                        ServiceName,
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
        /// Pauses this instance.
        /// </summary>
        [PublicAPI]
        [ServiceCommand(typeof(ServiceResources), "Cmd_Pause_Names", "Cmd_Pause_Description")]
        public void Pause()
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

                Log.Add(LoggingLevel.Information, () => ServiceResources.Inf_ServiceRunner_Pause_Pausing, ServiceName);
                Stopwatch s = Stopwatch.StartNew();
                try
                {
                    if (IsService)
                    {
                        Contract.Assert(ServiceController != null);
                        ServiceController.Pause();
                    }
                    else
                        OnPause();
                    Log.Add(
                        LoggingLevel.Information,
                        () => ServiceResources.Inf_ServiceRunner_Pause_Paused,
                        ServiceName,
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
        /// Continues this instance.
        /// </summary>
        [PublicAPI]
        [ServiceCommand(typeof(ServiceResources), "Cmd_Continue_Names", "Cmd_Continue_Description")]
        public void Continue()
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

                Log.Add(
                    LoggingLevel.Information,
                    () => ServiceResources.Inf_ServiceRunner_Continue_Continuing,
                    ServiceName);
                Stopwatch s = Stopwatch.StartNew();
                try
                {
                    if (IsService)
                    {
                        Contract.Assert(ServiceController != null);
                        ServiceController.Continue();
                    }
                    else
                        OnContinue();
                    Log.Add(
                        LoggingLevel.Information,
                        () => ServiceResources.Inf_ServiceRunner_Continue_Continued,
                        ServiceName,
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
        /// Shuts down this instance.
        /// </summary>
        [PublicAPI]
        [ServiceCommand(typeof(ServiceResources), "Cmd_Shutdown_Names", "Cmd_Shutdown_Description")]
        public void Shutdown()
        {
            lock (_lock)
            {
                Log.Add(
                    LoggingLevel.Information,
                    () => ServiceResources.Inf_ServiceRunner_Shutdown_ShuttingDown,
                    ServiceName);
                Stopwatch s = Stopwatch.StartNew();
                try
                {
                    if (IsService)
                    {
                        Log.Add(
                            LoggingLevel.Error,
                            () => ServiceResources.Err_ServiceRunner_ServiceNotInteractive,
                            ServiceName);
                        return;
                    }

                    OnShutdown();
                    Log.Add(
                        LoggingLevel.Information,
                        () => ServiceResources.Inf_ServiceRunner_Shutdown_ShutDown,
                        ServiceName,
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
        /// Runs the custom command on this instance.
        /// </summary>
        /// <param name="command">The command.</param>
        [PublicAPI]
        [ServiceCommand(typeof(ServiceResources), "Cmd_CustomCommand_Names", "Cmd_CustomCommand_Description")]
        public void CustomCommand([SCP(typeof(ServiceResources), "Cmd_CustomCommand_Command_Description")] int command)
        {
            lock (_lock)
            {
                Log.Add(
                    LoggingLevel.Information,
                    () => ServiceResources.Inf_ServiceRunner_CustomCommand_Running,
                    command,
                    ServiceName);
                Stopwatch s = Stopwatch.StartNew();
                try
                {
                    if (IsService)
                    {
                        Contract.Assert(ServiceController != null);
                        ServiceController.ExecuteCommand(command);
                    }
                    else
                        OnCustomCommand(command);
                    Log.Add(
                        LoggingLevel.Information,
                        () => ServiceResources.Inf_ServiceRunner_CustomCommand_Complete,
                        command,
                        ServiceName,
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
        [ServiceCommand(typeof(ServiceResources), "Cmd_PowerEvent_Names", "Cmd_PowerEvent_Description")]
        public bool PowerEvent(
            [SCP(typeof(ServiceResources), "Cmd_PowerEvent_PowerStatus_Description")] PowerBroadcastStatus powerStatus)
        {
            lock (_lock)
            {
                Log.Add(
                    LoggingLevel.Information,
                    () => ServiceResources.Inf_ServiceRunner_PowerEvent_Sending,
                    powerStatus,
                    ServiceName);
                Stopwatch s = Stopwatch.StartNew();
                try
                {
                    if (IsService)
                    {
                        Log.Add(
                            LoggingLevel.Error,
                            () => ServiceResources.Err_ServiceRunner_ServiceNotInteractive,
                            ServiceName);
                        return true;
                    }
                    bool result = OnPowerEvent(powerStatus);
                    Log.Add(
                        LoggingLevel.Information,
                        () => ServiceResources.Inf_ServiceRunner_PowerEvent_Sent,
                        powerStatus,
                        ServiceName,
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
        [ServiceCommand(typeof(ServiceResources), "Cmd_SessionChange_Names", "Cmd_SessionChange_Description")]
        public void SessionChange(
            [SCP(typeof(ServiceResources), "Cmd_SessionChange_ChangeReason_Description")] SessionChangeReason
                changeReason,
            [SCP(typeof(ServiceResources), "Cmd_SessionChange_SessionID_Description")] int sessionId)
        {
            lock (_lock)
            {
                Log.Add(
                    LoggingLevel.Information,
                    () => ServiceResources.Inf_ServiceRunner_SessionChange_Sending,
                    changeReason,
                    sessionId,
                    ServiceName);
                Stopwatch s = Stopwatch.StartNew();
                try
                {
                    if (IsService)
                    {
                        Log.Add(
                            LoggingLevel.Error,
                            () => ServiceResources.Err_ServiceRunner_ServiceNotInteractive,
                            ServiceName);
                        return;
                    }

                    OnSessionChange(CreateSessionChangeDescription(changeReason, sessionId));
                    Log.Add(
                        LoggingLevel.Information,
                        () => ServiceResources.Inf_ServiceRunner_SessionChange_Sent,
                        changeReason,
                        sessionId,
                        ServiceName,
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
        /// Gets the details of the performance counters loaded.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="category">The category.</param>
        [PublicAPI]
        [ServiceCommand(typeof(ServiceResources), "Cmd_Performance_Names", "Cmd_Performance_Description", true, writerParameter: "writer")]
        public void Performance(
            [NotNull] TextWriter writer,
            [CanBeNull][SCP(typeof(ServiceResources), "Cmd_Performance_Category_Description")] string category = null)
        {
            lock (_lock)
            {
                bool categoryOmitted = string.IsNullOrWhiteSpace(category);

                PerfCategory cat = categoryOmitted
                    ? null
                    : PerfCategory.All.FirstOrDefault(p => string.Equals(p.CategoryName, category, StringComparison.CurrentCultureIgnoreCase));

                if (!categoryOmitted && cat == null)
                {
                    PerformanceCatergoryNotFoundFormat.WriteTo(
                        writer,
                        null,
                        (_, c) =>
                            string.Equals(c.Tag, "name", StringComparison.CurrentCultureIgnoreCase)
                                ? category
                                : Resolution.Unknown);
                }

                if (cat == null) // Or the counter does not exist
                {
                    AllPerformanceCategoriesFormat.WriteTo(
                        writer,
                        null,
                        (_, c) =>
                            string.Equals(c.Tag, "counters", StringComparison.CurrentCultureIgnoreCase)
                                ? PerfCategory.All.OrderBy(pc => pc.CategoryName)
                                : Resolution.Unknown);
                    return;
                }

                cat.WriteTo(writer, PerfCategory.VerboseFormat);
                writer.WriteLine();
                writer.WriteLine();
            }
        }
    }
}