using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplications.Utilities.Service.Client
{
    public class ConsoleClient
    {
        public static void Run(string pipe)
        {
            if (!ConsoleHelper.IsConsole)
                return;
            RunAsync(pipe).Wait();
        }

        public static Task RunAsync(string pipe)
        {
            Console.WriteLine("Pipe: "+pipe);
            return TaskResult.Completed;
        }
    }
}
