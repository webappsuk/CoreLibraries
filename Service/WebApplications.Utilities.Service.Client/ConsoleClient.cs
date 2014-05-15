using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplications.Utilities.Service.Client
{
    public class ConsoleClient
    {
        public static void Run(NamedPipeServerInfo server)
        {
            if (!ConsoleHelper.IsConsole)
                return;
            RunAsync(server).Wait();
        }

        public static Task RunAsync(NamedPipeServerInfo server)
        {
            return TaskResult.Completed;
        }
    }
}
