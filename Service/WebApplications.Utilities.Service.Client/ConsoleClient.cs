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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WebApplications.Utilities.Formatting;
using WebApplications.Utilities.Logging;
using WebApplications.Utilities.Service.PipeProtocol;

namespace WebApplications.Utilities.Service.Client
{
    public static class ConsoleClient
    {
        [NotNull]
        private static readonly FormatBuilder _promptBuilder = new FormatBuilder()
            .AppendForegroundColor(ConsoleColor.Cyan)
            .AppendFormat("[{Time:hh:mm:ss.ffff}]")
            .AppendForegroundColor(ConsoleColor.Yellow)
            .AppendFormat("{Server: {Name}}")
            .AppendResetForegroundColor()
            .Append(" > ")
            .MakeReadOnly();

        [NotNull]
        private static readonly FormatBuilder _serverList = new FormatBuilder()
            .AppendForegroundColor(ConsoleColor.White)
            .AppendLine("Current matching pipes:")
            .AppendLayout(indentSize: 25, tabStops: new[] {7, 25})
            .AppendForegroundColor(ConsoleColor.Yellow)
            .AppendLine("Host\tName\tPipe")
            .AppendResetForegroundColor()
            .AppendFormatLine("{Servers:{<items>:{<item>}{<join>:\r\n}}}")
            .MakeReadOnly();

        [NotNull]
        private static readonly FormatBuilder _connected = new FormatBuilder()
            .AppendLine()
            .AppendForegroundColor(ConsoleColor.White)
            .Append("Connected to ")
            .AppendForegroundColor(ConsoleColor.Yellow)
            .AppendFormatLine("{ServiceName}")
            .AppendResetForegroundColor()
            .MakeReadOnly();

        /// <summary>
        /// Runs the client asynchronously, optionally connecting to the service with the given pipe. 
        /// If no pipe is given, or the pipe is invalid, the user will be prompted to select a service to connect to.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="pipe">The pipe.</param>
        [PublicAPI]
        public static void Run([NotNull] string description, [CanBeNull] string pipe)
        {
            Contract.Requires<RequiredContractException>(description != null, "Parameter_Null");
            Run(description, new NamedPipeServerInfo(pipe));
        }

        /// <summary>
        /// Runs the client, optionally connecting to the given service. 
        /// If no service is given, or the service is invalid, the user will be prompted to select a service to connect to.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="server">The server.</param>
        [PublicAPI]
        public static void Run([NotNull] string description, [CanBeNull] NamedPipeServerInfo server = null)
        {
            Contract.Requires<RequiredContractException>(description != null, "Parameter_Null");
            if (!ConsoleHelper.IsConsole)
                return;

            RunAsync(description, server).Wait();
        }

        /// <summary>
        /// Runs the client asynchronously, optionally connecting to the service with the given pipe. 
        /// If no pipe is given, or the pipe is invalid, the user will be prompted to select a service to connect to.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="pipe">The pipe.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        [NotNull]
        [PublicAPI]
        public static Task RunAsync(
            [NotNull] string description,
            [CanBeNull] string pipe,
            CancellationToken token = default(CancellationToken))
        {
            Contract.Requires<RequiredContractException>(description != null, "Parameter_Null");
            return RunAsync(description, new NamedPipeServerInfo(pipe), token);
        }

        /// <summary>
        /// Runs the client asynchronously, optionally connecting to the given service. 
        /// If no service is given, or the service is invalid, the user will be prompted to select a service to connect to.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="service">The service.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        [NotNull]
        [PublicAPI]
        public static async Task RunAsync(
            [NotNull] string description,
            [CanBeNull] NamedPipeServerInfo service = null,
            CancellationToken token = default(CancellationToken))
        {
            Contract.Requires<RequiredContractException>(description != null, "Parameter_Null");
            if (!ConsoleHelper.IsConsole)
                return;

            try
            {
                Log.SetTrace(validLevels: LoggingLevels.None);
                Log.SetConsole(Log.ShortFormat);

                Console.Title = description;
                NamedPipeClient client = null;
                while (client == null)
                {
                    token.ThrowIfCancellationRequested();
                    while (service == null ||
                           !service.IsValid)
                    {
                        Console.Clear();
                        NamedPipeServerInfo[] services = null;
                        await Log.Flush(token);

                        ConsoleTextWriter.Default.WriteLine(ClientResources.ConsoleClient_RunAsync_ScanningForService);
                        while (services == null ||
                               services.Length < 1)
                        {
                            services = NamedPipeClient.GetServices().ToArray();
                            if (Console.KeyAvailable)
                                break;
                            // ReSharper disable once PossibleNullReferenceException
                            await Task.Delay(500, token);
                            token.ThrowIfCancellationRequested();
                        }

                        if (services.Length > 0)
                            WriteServerList(services);

                        ConsoleTextWriter.Default.WriteLine(ClientResources.ConsoleClient_RunAsync_EnterServiceName);
                        string serviceName = Console.ReadLine();
                        service = !string.IsNullOrWhiteSpace(serviceName)
                            ? NamedPipeClient.FindService(serviceName)
                            : NamedPipeClient.GetServices().FirstOrDefault();
                    }

                    Console.Clear();
                    // ReSharper disable once AssignNullToNotNullAttribute
                    ConsoleTextWriter.Default.WriteLine(
                        ClientResources.ConsoleClient_RunAsync_ConnectingToService,
                        service.Name);

                    try
                    {
                        client =
                            await
                                NamedPipeClient.Connect(
                                    description,
                                    service,
                                    OnReceive,
                                    token.WithTimeout(10000));
                    }
                    catch (TaskCanceledException)
                    {
                        // ReSharper disable once AssignNullToNotNullAttribute
                        ConsoleTextWriter.Default.WriteLine(
                            ClientResources.ConsoleClient_RunAsync_TimedOut,
                            service.Name);
                        ConsoleTextWriter.Default.WriteLine(ClientResources.ConsoleClient_RunAsync_PressAnyKeyContinue);
                        client = null;
                        service = null;
                        Console.ReadKey(true);
                    }
                }

                // ReSharper disable once AssignNullToNotNullAttribute
                Console.Title = string.Format(
                    ClientResources.ConsoleClient_RunAsync_ConnectedTitle,
                    description,
                    service.Name);
                _connected.WriteToConsole(
                    null,
                    new Dictionary<string, object>
                    {
                        {"ServiceName", client.ServiceName}
                    });

                // ReSharper disable once PossibleNullReferenceException
                await Task.Delay(1100, token);
                await Log.Flush(token);

                while (client.State != PipeState.Closed)
                {
                    token.ThrowIfCancellationRequested();
                    WritePrompt(service);
                    string command = Console.ReadLine();

                    if (!string.IsNullOrWhiteSpace(command))
                        client.Execute(command, token)
                            .Subscribe(
                                c => new FormatBuilder(c).WriteToConsole(),
                                e =>
                                {
                                    Contract.Assert(e != null);
                                    if (!(e is TaskCanceledException))
                                        new FormatBuilder()
                                            .AppendForegroundColor(ConsoleColor.Red)
                                            .AppendLine(e.Message)
                                            .AppendResetForegroundColor()
                                            .WriteToConsole();
                                },
                                () => { },
                                token);

                    // Wait to allow any disconnects or logs to come through.
                    // ReSharper disable once PossibleNullReferenceException
                    await Task.Delay(1100, token);
                    await Log.Flush(token);
                }
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception e)
            {
                if (!token.IsCancellationRequested)
                    Log.Add(e);
            }
            await Log.Flush(token);
            // ReSharper disable once PossibleNullReferenceException
            await Task.Delay(200, token);
            Console.WriteLine(ClientResources.ConsoleClient_RunAsync_PressAnyKeyExit);
            Console.ReadKey(true);
        }

        private static void OnReceive([CanBeNull] Message message)
        {
            if (message == null)
                return;

            LogResponse logResponse = message as LogResponse;
            if (logResponse != null &&
                logResponse.Logs != null)
            {
                ConsoleTextWriter.Default.Context.Invoke(
                    () =>
                    {
                        if (Console.CursorLeft != 0)
                            ConsoleTextWriter.Default.WriteLine();
                        foreach (Log log in logResponse.Logs)
                        {
                            Contract.Assert(log != null);
                            log.WriteTo(ConsoleTextWriter.Default, Log.ShortFormat);
                        }
                    });

                return;
            }

            DisconnectResponse disconnectResponse = message as DisconnectResponse;
            if (disconnectResponse == null) return;
            ConsoleTextWriter.Default.WriteLine();
            ConsoleTextWriter.Default.WriteLine("Disconnected");
        }

        /// <summary>
        /// Writes the pipe list.
        /// </summary>
        private static void WriteServerList([CanBeNull] params NamedPipeServerInfo[] servers)
        {
            _serverList.WriteToConsole(
                null,
                (_, c) =>
                {
                    Contract.Assert(c != null);
                    Contract.Assert(c.Tag != null);
                    if (!string.Equals(c.Tag, "servers", StringComparison.CurrentCultureIgnoreCase))
                        return Resolution.Unknown;
                    return (servers != null) && (servers.Length > 0)
                        ? servers
                        : Resolution.Null;
                });
        }

        /// <summary>
        /// Writes the prompt.
        /// </summary>
        private static void WritePrompt([CanBeNull] NamedPipeServerInfo server)
        {
            if (Console.CursorLeft != 0)
                ConsoleTextWriter.Default.WriteLine();
            _promptBuilder.WriteToConsole(
                null,
                (_, c) =>
                {
                    Contract.Assert(c != null);
                    Contract.Assert(c.Tag != null);
                    switch (c.Tag.ToLowerInvariant())
                    {
                        case "time":
                            return DateTime.UtcNow;
                        case "server":
                            return server;
                        default:
                            return Resolution.Unknown;
                    }
                });
        }
    }
}