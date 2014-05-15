using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WebApplications.Utilities.Formatting;

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
            .Append(" > ");

        [NotNull]
        private static readonly FormatBuilder _pipeList = new FormatBuilder()
            .AppendForegroundColor(ConsoleColor.White)
            .AppendLine("Current matching pipes:")
            .AppendLayout(indentSize: 25, tabStops: new[] {7, 25})
            .AppendForegroundColor(ConsoleColor.Yellow)
            .AppendLine("Host\tName\tPipe")
            .AppendResetForegroundColor()
            .AppendFormatLine("{Pipes:{<items>:{<item>}{<join>:\r\n}}}");


        public static void Run(string pipe)
        {
            Run(new NamedPipeServerInfo(pipe));
        }

        public static void Run(NamedPipeServerInfo server = null)
        {
            if (!ConsoleHelper.IsConsole)
                return;


            while (server == null)
            {
                WriteServerList();
                ConsoleTextWriter.Default.WriteLine("Please specify a valid server name or pipe to connect to...");
                WritePrompt(server);
                server = NamedPipeClient.FindServer(Console.ReadLine());
            }
        }

        /// <summary>
        /// Writes the pipe list.
        /// </summary>
        private static void WriteServerList()
        {
            _pipeList.WriteToConsole(
                null,
                (_, c) =>
                {
                    if (!string.Equals(c.Tag, "pipes", StringComparison.CurrentCultureIgnoreCase))
                        return Resolution.Unknown;
                    var pipes = NamedPipeClient.GetServers().ToArray();
                    return pipes.Length > 0 ? pipes : Resolution.Null;
                });
        }

        /// <summary>
        /// Writes the prompt.
        /// </summary>
        private static void WritePrompt([CanBeNull] NamedPipeServerInfo server)
        {
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
