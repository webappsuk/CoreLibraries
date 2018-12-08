#region © Copyright Web Applications (UK) Ltd, 2015.  All rights reserved.
// Copyright (c) 2015, Web Applications UK Ltd
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
using System.Globalization;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Formatting;
using WebApplications.Utilities.Logging;
using WebApplications.Utilities.Service.Common;
using WebApplications.Utilities.Service.Common.Control;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.Service
{
    /// <summary>
    /// Implements a console-based service connection, if running in a console.
    /// </summary>
    [PublicAPI]
    public class ConsoleConnection : IDisposable, IConnection
    {
        /// <summary>
        /// The command prompt format.
        /// </summary>
        [NotNull]
        private static readonly FormatBuilder _promptBuilder =
            new FormatBuilder(ServiceResources.ConsoleConnection_PromptFormat, true);

        private static readonly FormatBuilder _logFormatBuilder =
            new FormatBuilder(
                120,
                33,
                alignment: Alignment.Left,
                tabStops: new[] { 33 },
                format: ServiceResources.ConsoleConnection_LogFormat,
                isReadOnly: true);

        /// <summary>
        /// The installation prompt.
        /// </summary>
        [NotNull]
        private static readonly FormatBuilder _promptInstall =
            new FormatBuilder(ServiceResources.ConsoleConnection_InstallPromptFormat, true);

        /// <summary>
        /// The error format
        /// </summary>
        [NotNull]
        private static readonly FormatBuilder _errorFormat =
            new FormatBuilder(ServiceResources.ConsoleConnection_ErrorFormat, true);

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
            if (!ConsoleHelper.IsConsole) throw new InvalidOperationException(CommonResources.Not_In_Console);
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
        /// <param name="runMode">The run mode.</param>
        /// <param name="defaultLogFormat">The default log format.</param>
        /// <param name="defaultLoggingLevels">The default logging levels.</param>
        /// <param name="token">The token.</param>
        /// <returns>
        /// An awaitable task.
        /// </returns>
        // ReSharper disable once CodeAnnotationAnalyzer
        public static async Task RunAsync(
            [NotNull] BaseService service,
            RunMode runMode = RunMode.Default,
            [CanBeNull] FormatBuilder defaultLogFormat = null,
            LoggingLevels defaultLoggingLevels = LoggingLevels.All,
            CancellationToken token = default(CancellationToken))
        {
            if (service == null) throw new ArgumentNullException("service");

            if (!ConsoleHelper.IsConsole)
                return;
            Console.Clear();
            Log.SetTrace(validLevels: LoggingLevels.None);
            Log.SetConsole(defaultLogFormat ?? _logFormatBuilder, defaultLoggingLevels);
            await Log.Flush(token).ConfigureAwait(false);

            Impersonator impersonator = null;
            try
            {
                if (runMode.HasFlag(RunMode.Prompt))
                {
                    Debug.Assert(service.ServiceName != null);

                    // Whether we start will depend on the selected option in prompt.
                    runMode = runMode.Clear(RunMode.Start, true);

                    Console.Title = ServiceResources.ConsoleConnection_RunAsync_ConfigureTitle + service.ServiceName;
                    bool done = false;

                    do
                    {
                        if (token.IsCancellationRequested) return;

                        Dictionary<string, string> options = new Dictionary<string, string>
                        {
                            { "I", ServiceResources.ConsoleConnection_RunAsync_OptionInstall },
                            { "U", ServiceResources.ConsoleConnection_RunAsync_OptionUninstall },
                            { "S", ServiceResources.ConsoleConnection_RunAsync_OptionStart },
                            { "R", ServiceResources.ConsoleConnection_RunAsync_OptionRestart },
                            { "T", ServiceResources.ConsoleConnection_RunAsync_OptionStop },
                            { "P", ServiceResources.ConsoleConnection_RunAsync_OptionPause },
                            { "C", ServiceResources.ConsoleConnection_RunAsync_OptionContinue },
                            { "Y", ServiceResources.ConsoleConnection_RunAsync_OptionRunCmd },
                            { "V", ServiceResources.ConsoleConnection_RunAsync_OptionStartCmd },
                            { "W", ServiceResources.ConsoleConnection_RunAsync_OptionRunCmdNewCredentials },
                            { "Z", ServiceResources.ConsoleConnection_RunAsync_OptionRunNoInteraction },
                            { "X", ServiceResources.ConsoleConnection_RunAsync_OptionExit }
                        };

                        if (!runMode.HasFlag(RunMode.Interactive))
                        {
                            options.Remove("V");
                            options.Remove("Y");
                            options.Remove("W");
                        }

                        bool isAdmin;
                        string currentUser;
                        try
                        {
                            WindowsIdentity identity = WindowsIdentity.GetCurrent();
                            currentUser = identity.Name;
                            Debug.Assert(identity != null);
                            WindowsPrincipal principal = new WindowsPrincipal(identity);
                            isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
                        }
                        catch
                        {
                            isAdmin = false;
                            currentUser = null;
                        }

                        if (!string.IsNullOrEmpty(currentUser))
                        {
                            FormatBuilder fb = new FormatBuilder()
                                .AppendForegroundColor(ConsoleColor.Cyan)
                                .Append("Current User: ")
                                .AppendForegroundColor(ConsoleColor.White)
                                .Append(currentUser);
                            if (isAdmin)
                                fb
                                    .AppendForegroundColor(ConsoleColor.Yellow)
                                    .Append(" [Admin]");
                            fb.AppendLine().WriteToConsole();
                        }

                        if (!isAdmin)
                        {
                            options.Remove("I");
                            options.Remove("U");
                        }

                        if (Controller.ServiceIsInstalled(service.ServiceName))
                        {
                            ServiceControllerStatus state = Controller.GetServiceStatus(service.ServiceName);
                            new FormatBuilder()
                                .AppendForegroundColor(ConsoleColor.White)
                                .AppendFormatLine(
                                    ServiceResources.ConsoleConnection_RunAsync_ServiceInstalledState,
                                    service.ServiceName,
                                    state)
                                .AppendResetForegroundColor()
                                .WriteToConsole();

                            options.Remove("I");

                            switch (state)
                            {
                                case ServiceControllerStatus.StopPending:
                                case ServiceControllerStatus.Stopped:
                                    // Service is stopped or stopping.
                                    options.Remove("C");
                                    options.Remove("R");
                                    options.Remove("T");
                                    break;
                                case ServiceControllerStatus.StartPending:
                                case ServiceControllerStatus.ContinuePending:
                                case ServiceControllerStatus.Running:
                                    // Service is starting or running.
                                    options.Remove("S");
                                    options.Remove("C");
                                    break;
                                case ServiceControllerStatus.PausePending:
                                case ServiceControllerStatus.Paused:
                                    // Service is paused or pausing.
                                    options.Remove("S");
                                    options.Remove("R");
                                    options.Remove("T");
                                    options.Remove("P");
                                    break;
                                default:
                                    // Service is not installed - shouldn't happen.
                                    options.Remove("U");
                                    options.Remove("S");
                                    options.Remove("R");
                                    options.Remove("T");
                                    options.Remove("C");
                                    options.Remove("P");
                                    break;
                            }
                            options.Remove("V");
                            options.Remove("Y");
                            options.Remove("Z");
                        }
                        else
                        {
                            // No service installed.
                            options.Remove("U");
                            options.Remove("S");
                            options.Remove("R");
                            options.Remove("T");
                            options.Remove("P");
                            options.Remove("C");
                        }

                        _promptInstall.WriteToConsole(
                            null,
                            // ReSharper disable once PossibleNullReferenceException
                            (_, c) => !string.Equals(c.Tag, "options", StringComparison.CurrentCultureIgnoreCase)
                                ? Resolution.Unknown
                                : options);

                        string key;
                        do
                        {
                            key = Char.ToUpperInvariant(Console.ReadKey(true).KeyChar)
                                .ToString(CultureInfo.InvariantCulture);
                        } while (!options.ContainsKey(key));

                        try
                        {
                            string userName;
                            string password;

                            switch (key)
                            {
                                case "I":
                                    GetUserNamePassword(out userName, out password);

                                    service.Install(ConsoleTextWriter.Default, userName, password);

                                    Console.Write(ServiceResources.ConsoleConnection_RunAsync_WaitInstall);
                                    while (!Controller.ServiceIsInstalled(service.ServiceName))
                                    {
                                        await Task.Delay(250, token).ConfigureAwait(false);
                                        Console.Write('.');
                                    }
                                    Console.WriteLine(ServiceResources.Done);
                                    Console.WriteLine();
                                    break;

                                case "U":
                                    await service.Uninstall(ConsoleTextWriter.Default, token).ConfigureAwait(false);

                                    Console.Write(ServiceResources.ConsoleConnection_RunAsync_WaitUninstall);
                                    while (Controller.ServiceIsInstalled(service.ServiceName))
                                    {
                                        await Task.Delay(250, token).ConfigureAwait(false);
                                        Console.Write('.');
                                    }
                                    Console.WriteLine(ServiceResources.Done);
                                    Console.WriteLine();
                                    break;

                                case "R":
                                    Console.Write(ServiceResources.ConsoleConnection_RunAsync_AttemptingStop);
                                    await Controller.StopService(service.ServiceName, token).ConfigureAwait(false);
                                    Console.WriteLine(ServiceResources.Done);
                                    Console.WriteLine();
                                    Console.Write(ServiceResources.ConsoleConnection_RunAsync_AttemptingStart);
                                    await
                                        Controller.StartService(service.ServiceName, null, token).ConfigureAwait(false);
                                    Console.WriteLine(ServiceResources.Done);
                                    Console.WriteLine();
                                    break;

                                case "S":
                                    Console.Write(ServiceResources.ConsoleConnection_RunAsync_AttemptingStart);
                                    await
                                        Controller.StartService(service.ServiceName, null, token).ConfigureAwait(false);
                                    Console.WriteLine(ServiceResources.Done);
                                    Console.WriteLine();
                                    break;

                                case "T":
                                    Console.Write(ServiceResources.ConsoleConnection_RunAsync_AttemptingStop);
                                    await Controller.StopService(service.ServiceName, token).ConfigureAwait(false);
                                    Console.WriteLine(ServiceResources.Done);
                                    Console.WriteLine();
                                    break;

                                case "P":
                                    Console.Write(ServiceResources.ConsoleConnection_RunAsync_AttemptingPause);
                                    await Controller.PauseService(service.ServiceName, token).ConfigureAwait(false);
                                    Console.WriteLine(ServiceResources.Done);
                                    break;

                                case "C":
                                    Console.Write(ServiceResources.ConsoleConnection_RunAsync_AttemptingContinue);
                                    await Controller.ContinueService(service.ServiceName, token).ConfigureAwait(false);
                                    Console.WriteLine(ServiceResources.Done);
                                    Console.WriteLine();
                                    break;

                                case "V":
                                    runMode = runMode.Set(RunMode.Start, true).Set(RunMode.Interactive, true);
                                    done = true;
                                    break;

                                case "Y":
                                    runMode = runMode.Set(RunMode.Interactive, true);
                                    done = true;
                                    break;

                                case "W":
                                    GetUserNamePassword(out userName, out password);
                                    if (userName == null)
                                        break;
                                    Debug.Assert(password != null);

                                    Impersonator ei = impersonator;
                                    impersonator = null;
                                    if (ei != null)
                                        ei.Dispose();
                                    // Run in new security context.
                                    impersonator = new Impersonator(userName, password);
                                    break;

                                case "Z":
                                    runMode = runMode.Set(RunMode.Start, true).Clear(RunMode.Interactive, true);
                                    done = true;
                                    Console.WriteLine(ServiceResources.ConsoleConnection_RunAsync_RunningNonInteractive);
                                    Console.WriteLine(
                                        ServiceResources.ConsoleConnection_RunAsync_RunningNonInteractive2);
                                    Console.WriteLine();
                                    break;

                                default:
                                    return;
                            }
                        }
                        catch (TaskCanceledException)
                        {
                            return;
                        }
                        catch (Exception e)
                        {
                            if (!token.IsCancellationRequested)
                                Log.Add(e);
                        }
                    } while (!done);
                }
                else if (!runMode.HasFlag(RunMode.Interactive))
                    // If we don't show prompt and we're not interactive we should always start the service.
                    runMode = runMode.Set(RunMode.Start, true);

                // Create connection
                Console.Title = ServiceResources.ConsoleConnection_RunAsync_RunningTitle + service.ServiceName;
                ConsoleConnection connection = new ConsoleConnection(defaultLogFormat, defaultLoggingLevels, token);
                Guid id = service.Connect(connection);

                // Combined cancellation tokens.
                ITokenSource tSource = token.CreateLinked(connection._cancellationTokenSource.Token);
                try
                {
                    CancellationToken t = tSource.Token;

                    if (t.IsCancellationRequested) return;

                    if (runMode.HasFlag(RunMode.Start))
                    {
                        // Start the service
                        await service.StartService(ConsoleTextWriter.Default, null, t).ConfigureAwait(false);
                        if (t.IsCancellationRequested)
                            return;
                    }

                    if (!runMode.HasFlag(RunMode.Interactive))
                    {
                        // Wait to be cancelled as nothing to do.
                        await t.WaitHandle;
                        return;
                    }

                    do
                    {
                        // Flush logs
                        await Log.Flush(t).ConfigureAwait(false);

                        if (t.IsCancellationRequested) break;

                        WritePrompt(service);
                        try
                        {
                            string commandLine = await Console.In.ReadLineAsync().ConfigureAwait(false);
                            if (!string.IsNullOrWhiteSpace(commandLine))
                            {
                                bool completed = false;
                                ICancelableTokenSource commandCancellationSource = t.ToCancelable();
                                CancellationToken commandToken = commandCancellationSource.Token;

#pragma warning disable 4014
                                service.ExecuteAsync(id, commandLine, ConsoleTextWriter.Default, commandToken)
                                    .ContinueWith(
                                        task =>
                                        {
                                            Debug.Assert(task != null);

                                            completed = true;

                                            if (task.IsCompleted ||
                                                task.IsCanceled)
                                                return;

                                            if (task.IsFaulted)
                                            {
                                                Debug.Assert(task.Exception != null);
                                                _errorFormat.WriteToConsoleInstance(null, task.Exception);
                                            }
                                        },
                                        TaskContinuationOptions.ExecuteSynchronously);
#pragma warning restore 4014

                                while (!completed)
                                {
                                    if (!commandCancellationSource.IsCancellationRequested &&
                                        Console.KeyAvailable &&
                                        Console.ReadKey(true).Key == ConsoleKey.Escape)
                                    {
                                        // Cancel command
                                        Console.Write(ServiceResources.ConsoleConnection_RunAsync_Cancelling);
                                        commandCancellationSource.Cancel();
                                        break;
                                    }
                                    await Task.Delay(100, token).ConfigureAwait(false);
                                }
                            }
                        }
                        catch (TaskCanceledException)
                        {
                            throw;
                        }
                        catch (Exception e)
                        {
                            if (!t.IsCancellationRequested)
                                _errorFormat.WriteToConsoleInstance(null, e);
                        }

                        // Let any async stuff done by the command have a bit of time, also throttle commands.
                        await Task.Delay(500, t).ConfigureAwait(false);
                    } while (!t.IsCancellationRequested);
                }
                catch (TaskCanceledException)
                {
                }
                finally
                {
                    tSource.Dispose();

                    // ReSharper disable MethodSupportsCancellation
                    Log.Flush().Wait();
                    // ReSharper restore MethodSupportsCancellation
                    service.Disconnect(id);
                    Console.WriteLine(ServiceResources.ConsoleConnection_RunAsync_PressKeyToExit);
                    Console.ReadKey(true);
                }
            }
            finally
            {
                if (impersonator != null)
                    impersonator.Dispose();
            }
        }

        /// <summary>
        /// Gets the user name and password from the console.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        private static void GetUserNamePassword(out string userName, out string password)
        {
            do
            {
                new FormatBuilder()
                    .AppendForegroundColor(ConsoleColor.Cyan)
                    .Append(ServiceResources.ConsoleConnection_GetUserNamePassword_Username)
                    .AppendResetForegroundColor()
                    .WriteToConsole();
                userName = Console.ReadLine();
                if (String.IsNullOrWhiteSpace(userName))
                {
                    userName = null;
                    password = null;
                    break;
                }

                string[] unp = userName.Split('\\');
                if (unp.Length != 2 ||
                    String.IsNullOrWhiteSpace(unp[0]) ||
                    String.IsNullOrWhiteSpace(unp[1]))
                {
                    new FormatBuilder()
                        .AppendForegroundColor(ConsoleColor.Red)
                        .AppendLine(ServiceResources.ConsoleConnection_GetUserNamePassword_InvalidUserName)
                        .AppendForegroundColor(ConsoleColor.Gray)
                        .AppendLine(ServiceResources.Cmd_Install_UserName_Description)
                        .AppendResetForegroundColor()
                        .WriteToConsole();
                    continue;
                }

                new FormatBuilder()
                    .AppendForegroundColor(ConsoleColor.Cyan)
                    .Append(ServiceResources.ConsoleConnection_GetUserNamePassword_Password)
                    .AppendResetForegroundColor()
                    .WriteToConsole();
                password = ConsoleHelper.ReadPassword();
                break;
            } while (true);
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
        public FormatBuilder DefaultLogFormat
        {
            get { return _defaultLogFormat; }
        }

        /// <summary>
        /// Gets the default logging levels, this can be changed by commands.
        /// </summary>
        /// <value>The default logging levels.</value>
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
                    Debug.Assert(c != null);
                    Debug.Assert(c.Tag != null);
                    switch (c.Tag.ToLowerInvariant())
                    {
                        case "time":
                            return DateTime.Now.ToLocalTime();
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