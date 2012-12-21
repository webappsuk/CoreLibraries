using System;
using System.Diagnostics.Contracts;
using System.Net;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using CmdLine;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Performance.Tools.PerfSetup
{
    /// <summary>
    /// Console program entry point.
    /// </summary>
    [UsedImplicitly]
    class Program
    {
        static void Main(string[] args)
        {
            // Parse options
            try
            {
                Options options = CommandLine.Parse<Options>();
                Contract.Assert(options != null);

                if (options.Help) return;

                ScanMode mode;
                switch (options.Mode.ToLower())
                {
                    case "add":
                        mode = ScanMode.Add;
                        break;
                    case "delete":
                        mode = ScanMode.Delete;
                        break;
                    default:
                        mode = ScanMode.List;
                        break;
                }


                Scan.Execute(
                    mode,
                    options.Path,
                    (s, l) =>
                    Console.WriteLine(l == Level.Error || l == Level.Warning
                                          ? string.Format("{0}: {1}", l.ToString(), s)
                                          : s),
                    options.MachineName);
            }
            catch (CommandLineException commandLineException)
            {
                Console.WriteLine(commandLineException.ArgumentHelp.Message);
                Console.WriteLine(commandLineException.ArgumentHelp.GetHelpText(Console.BufferWidth));
            }
            catch (Exception exception)
            {
                Console.WriteLine("Fatal error occurred: " + exception.Message);
            }
        }
    }
}
