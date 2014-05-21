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
using System.Diagnostics.Contracts;
using System.Globalization;
using System.ServiceProcess;
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
        /// The installation prompt.
        /// </summary>
        [NotNull]
        private static readonly FormatBuilder _promptInstall = new FormatBuilder()
            .AppendForegroundColor(ConsoleColor.Yellow)
            .AppendLine("Select one of :")
            .AppendLayout(indentSize: 3, firstLineIndentSize: 5)
            .AppendFormatLine("{Options:{<items>:\r\n{!fgcolor:Cyan}{Key}\t{!fgcolor:White}{Value}}}")
            .AppendPopLayout()
            .MakeReadOnly();

        /// <summary>
        /// Runs the specified service using the command console as a user interface.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="promptInstall">if set to <see langword="true" /> and running as administrator prompts for installation options.</param>
        /// <param name="allowConsoleInteraction">if set to <see langword="true" /> allows console command line.</param>
        /// <param name="defaultLogFormat">The default log format.</param>
        /// <param name="defaultLoggingLevels">The default logging levels.</param>
        /// <param name="token">The token.</param>
        /// <returns>An awaitable task.</returns>
        // ReSharper disable once CodeAnnotationAnalyzer
        public static async Task RunAsync(
            [NotNull] BaseService service,
            bool promptInstall = true,
            bool allowConsoleInteraction = true,
            [CanBeNull] FormatBuilder defaultLogFormat = null,
            LoggingLevels defaultLoggingLevels = LoggingLevels.All,
            CancellationToken token = default(CancellationToken))
        {
            Contract.Requires<RequiredContractException>(service != null, "Parameter_Null");
            if (!ConsoleHelper.IsConsole)
                return;
            Console.Clear();
            Log.SetTrace(validLevels: LoggingLevels.None);
            Log.SetConsole(defaultLogFormat ?? Log.ShortFormat, defaultLoggingLevels);
            await Log.Flush(token);

            if (promptInstall)
            {
                Contract.Assert(service.ServiceName != null);
                Console.Title = ServiceResources.ConsoleConnection_RunAsync_ConfigureTitle + service.ServiceName;
                bool done = false;
                do
                {
                    if (token.IsCancellationRequested) return;

                    Dictionary<string, string> options = new Dictionary<string, string>
                    {
                        {"I", "Install service."},
                        {"U", "Uninstall service."},
                        {"S", "Start service."},
                        {"R", "Restart service."},
                        {"T", "Stop service."},
                        {"P", "Pause service."},
                        {"C", "Continue service."},
                        {"Y", "Run service from command line."},
                        {"W", "Run service from command line under new credentials."},
                        {"Z", "Run service without interaction."},
                        {"X", "Exit."}
                    };

                    if (!allowConsoleInteraction)
                    {
                        options.Remove("Y");
                        options.Remove("W");
                    }

                    if (!BaseService.IsAdministrator)
                    {
                        options.Remove("I");
                        options.Remove("U");
                    }

                    if (ServiceUtils.ServiceIsInstalled(service.ServiceName))
                    {
                        ServiceControllerStatus state = ServiceUtils.GetServiceStatus(service.ServiceName);
                        new FormatBuilder()
                            .AppendForegroundColor(ConsoleColor.White)
                            .AppendFormatLine("The '{0}' service is installed and {1}.", service.ServiceName, state)
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
                        (_, c) => !String.Equals(c.Tag, "options", StringComparison.CurrentCultureIgnoreCase)
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
                                while (!ServiceUtils.ServiceIsInstalled(service.ServiceName))
                                {
                                    await Task.Delay(250);
                                    Console.Write('.');
                                }
                                Console.WriteLine(ServiceResources.ConsoleConnection_RunAsync_Done);
                                Console.WriteLine();
                                break;

                            case "U":
                                await service.Uninstall(ConsoleTextWriter.Default);

                                Console.Write(ServiceResources.ConsoleConnection_RunAsync_WaitUninstall);
                                while (ServiceUtils.ServiceIsInstalled(service.ServiceName))
                                {
                                    await Task.Delay(250);
                                    Console.Write('.');
                                }
                                Console.WriteLine(ServiceResources.ConsoleConnection_RunAsync_Done);
                                Console.WriteLine();
                                break;

                            case "R":
                                Console.Write(ServiceResources.ConsoleConnection_RunAsync_AttemptingStop);
                                await ServiceUtils.StopService(service.ServiceName, token);
                                Console.WriteLine(ServiceResources.ConsoleConnection_RunAsync_Done);
                                Console.WriteLine();
                                Console.Write(ServiceResources.ConsoleConnection_RunAsync_AttemptingStart);
                                await ServiceUtils.StartService(service.ServiceName, null, token);
                                Console.WriteLine(ServiceResources.ConsoleConnection_RunAsync_Done);
                                Console.WriteLine();
                                break;

                            case "S":
                                Console.Write(ServiceResources.ConsoleConnection_RunAsync_AttemptingStart);
                                await ServiceUtils.StartService(service.ServiceName, null, token);
                                Console.WriteLine(ServiceResources.ConsoleConnection_RunAsync_Done);
                                Console.WriteLine();
                                break;

                            case "T":
                                Console.Write(ServiceResources.ConsoleConnection_RunAsync_AttemptingStop);
                                await ServiceUtils.StopService(service.ServiceName, token);
                                Console.WriteLine(ServiceResources.ConsoleConnection_RunAsync_Done);
                                Console.WriteLine();
                                break;

                            case "P":
                                Console.Write(ServiceResources.ConsoleConnection_RunAsync_AttemptingPause);
                                await ServiceUtils.PauseService(service.ServiceName, token);
                                Console.WriteLine(ServiceResources.ConsoleConnection_RunAsync_Done);
                                break;

                            case "C":
                                Console.Write(ServiceResources.ConsoleConnection_RunAsync_AttemptingContinue);
                                await ServiceUtils.ContinueService(service.ServiceName, token);
                                Console.WriteLine(ServiceResources.ConsoleConnection_RunAsync_Done);
                                Console.WriteLine();
                                break;

                            case "Y":
                                done = true;
                                break;

                            case "W":
                                done = true;
                                GetUserNamePassword(out userName, out password);
                                if (userName == null)
                                    break;
                                Contract.Assert(password != null);
                                // Run in new security context.
                                using (new Impersonator(userName, password))
                                    await
                                        RunAsync(
                                            service,
                                            false,
                                            allowConsoleInteraction,
                                            defaultLogFormat,
                                            defaultLoggingLevels,
                                            token);
                                return;

                            case "Z":
                                allowConsoleInteraction = false;
                                done = true;
                                Console.WriteLine(ServiceResources.ConsoleConnection_RunAsync_RunningNonInteractive);
                                Console.WriteLine(ServiceResources.ConsoleConnection_RunAsync_RunningNonInteractive2);
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

            // Create connection
            Console.Title = ServiceResources.ConsoleConnection_RunAsync_RunningTitle + service.ServiceName;
            ConsoleConnection connection = new ConsoleConnection(defaultLogFormat, defaultLoggingLevels, token);
            Guid id = service.Connect(connection);
            // Combined cancellation tokens.
            CancellationToken t = token.CreateLinked(connection._cancellationTokenSource.Token);

            if (t.IsCancellationRequested) return;
            try
            {
                if (!allowConsoleInteraction)
                {
                    // Start the service
                    await service.StartService(ConsoleTextWriter.Default, null, t);
                    // Wait to be cancelled as nothing to do.
                    await t.WaitHandle;
                    return;
                }
                do
                {
                    // Flush logs
                    await Log.Flush(t);

                    if (t.IsCancellationRequested) break;

                    WritePrompt(service);
                    try
                    {
                        await service.ExecuteAsync(id, await Console.In.ReadLineAsync(), ConsoleTextWriter.Default, t);
                    }
                    catch (TaskCanceledException)
                    {
                        throw;
                    }
                    catch (Exception e)
                    {
                        if (!token.IsCancellationRequested)
                            new FormatBuilder()
                                .AppendForegroundColor(ConsoleColor.Red)
                                .Append("Error: ")
                                .AppendLine(e.Message)
                                .AppendResetForegroundColor()
                                .WriteToConsole();
                    }

                    // Let any async stuff done by the command have a bit of time, also throttle commands.
                    await Task.Delay(500, t);
                } while (!t.IsCancellationRequested);
            }
            catch (TaskCanceledException)
            {
            }
            finally
            {
                // ReSharper disable MethodSupportsCancellation
                Log.Flush().Wait();
                // ReSharper restore MethodSupportsCancellation
                service.Disconnect(id);
                Console.WriteLine(ServiceResources.ConsoleConnection_RunAsync_PressKeyToExit);
                Console.ReadKey(true);
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
                    .Append("User name: ")
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
                        .AppendLine("Invalid user name!")
                        .AppendForegroundColor(ConsoleColor.Gray)
                        .AppendLine(ServiceResources.Cmd_Install_UserName_Description)
                        .AppendResetForegroundColor()
                        .WriteToConsole();
                    continue;
                }

                new FormatBuilder()
                    .AppendForegroundColor(ConsoleColor.Cyan)
                    .Append("Password: ")
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