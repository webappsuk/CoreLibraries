using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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
            .AppendLayout(indentSize: 25, tabStops: new[] { 7, 25 })
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


        public static void Run(string description, [CanBeNull] string pipe)
        {
            Run(description, new NamedPipeServerInfo(pipe));
        }

        public static void Run(string description, [CanBeNull] NamedPipeServerInfo server = null)
        {
            if (!ConsoleHelper.IsConsole)
                return;

            RunAsync(description, server).Wait();
        }

        [NotNull]
        public static Task RunAsync(
            string description,
            [CanBeNull] string pipe,
            CancellationToken token = default(CancellationToken))
        {
            return RunAsync(description, new NamedPipeServerInfo(pipe), token);
        }

        [NotNull]
        public static async Task RunAsync(string description, [CanBeNull] NamedPipeServerInfo service = null, CancellationToken token = default(CancellationToken))
        {
            if (!ConsoleHelper.IsConsole)
                return;

            try
            {
                Log.SetTrace(validLevels: LoggingLevels.None);
                Log.SetConsole();

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

                        ConsoleTextWriter.Default.WriteLine("Scanning for service... press any key to stop");
                        while (services == null ||
                               services.Length < 1)
                        {
                            services = NamedPipeClient.GetServices().ToArray();
                            if (Console.KeyAvailable)
                                break;
                            await Task.Delay(500, token);
                            token.ThrowIfCancellationRequested();
                        }

                        if (services.Length > 0)
                            WriteServerList(services);

                        ConsoleTextWriter.Default.WriteLine(
                            "Please specify a valid service name or pipe to connect to; or press enter to use the first service found...");
                        string serviceName = Console.ReadLine();
                        service = !string.IsNullOrWhiteSpace(serviceName)
                            ? NamedPipeClient.FindService(serviceName)
                            : NamedPipeClient.GetServices().FirstOrDefault();
                    }

                    Console.Clear();
                    ConsoleTextWriter.Default.WriteLine("Connecting to {0}...", service.Name);
                    client = await NamedPipeClient.Connect(description, service, OnReceive, token);
                }

                Console.Title = string.Format("{0} connected to {1}", description, service.Name);
                _connected.WriteToConsole(
                    null,
                    new Dictionary<string, object>
                    {
                        {"ServiceName", client.ServiceName}
                    });

                await Task.Delay(200, token);

                while (client.State != PipeState.Closed)
                {
                    token.ThrowIfCancellationRequested();
                    WritePrompt(service);
                    string command = Console.ReadLine();

                    if (!string.IsNullOrWhiteSpace(command))
                        client.Execute(command, token)
                            .Subscribe(c => new FormatBuilder(c).WriteToConsole());

                    // Wait to allow any disconnects or logs to come through.
                    await Log.Flush(token);
                    await Task.Delay(200, token);
                }
            }
            catch (TaskCanceledException) { }
        }

        private static void OnReceive([CanBeNull] Message message)
        {
            if (message == null)
                return;

            LogResponse logResponse = message as LogResponse;
            if (logResponse != null && logResponse.Logs != null)
            {
                foreach (Log log in logResponse.Logs)
                    log.WriteTo(ConsoleTextWriter.Default);
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
        private static void WriteServerList(params NamedPipeServerInfo[] servers)
        {
            _serverList.WriteToConsole(
                null,
                (_, c) =>
                {
                    if (!string.Equals(c.Tag, "servers", StringComparison.CurrentCultureIgnoreCase))
                        return Resolution.Unknown;
                    return servers.Length > 0 ? servers : Resolution.Null;
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
