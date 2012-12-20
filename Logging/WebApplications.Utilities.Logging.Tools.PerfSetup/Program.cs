using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using CmdLine;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace WebApplications.Utilities.Logging.Tools.PerfSetup
{
    class Program
    {
        static void Main(string[] args)
        {
            // Parse options
            try
            {
                Options options = CommandLine.Parse<Options>();
                Contract.Assert(options != null);

                // Check we have access to the performance counters.
                PerformanceCounterCategory.Exists("TestAccess", options.MachineName);

                string fullPath = Path.GetFullPath(options.Path);
                string[] parts = fullPath.Split('/', '\\');
                if (parts.Length < 1) throw new ArgumentOutOfRangeException("The '{0}' path was invalid.", options.Path);
                string path = string.Join("\\", parts.Take(parts.Length - 1));
                if (!Directory.Exists(path)) throw new ArgumentOutOfRangeException(string.Format("The '{0}' directory could not be found.", path));
                string end = parts.Last();
                string directory = Directory.GetDirectories(path, end).SingleOrDefault();
                string[] files;
                if (directory != null)
                {
                    files = Directory.GetFiles(directory);
                }
                else
                {
                    directory = path;
                    files = Directory.GetFiles(path, end);
                }
                Contract.Assert(files != null);

                files = files.Where(
                    f =>
                    {
                        string ext = Path.GetExtension(f).ToLower();
                        return (ext == ".dll" || ext == ".exe");
                    }).ToArray();

                if (files.Any())
                {
                    PerformanceInformation[] info = files.SelectMany(Load).ToArray();
                    if (info.Length > 0)
                    {
                        foreach (PerformanceInformation performanceInformation in info)
                        {
                            switch (options.Mode.ToLower())
                            {
                                case "add":
                                    var added = performanceInformation.Create(options.MachineName);
                                    Console.WriteLine(
                                        "Adding '{0}' {1}",
                                        performanceInformation,
                                                      added ? "succeeded" : "failed");
                                    break;
                                case "delete":
                                    var deleted = performanceInformation.Delete(options.MachineName);
                                    Console.WriteLine(
                                        "Deleting '{0}' {1}",
                                        performanceInformation,
                                        deleted ? "succeeded" : "failed");
                                    break;
                                default:
                                    // Treat everything else as list.
                                    Console.WriteLine("{0} - {1}", performanceInformation,
                                        performanceInformation.Exists ? "Exists" : "Missing");
                                    break;
                            }
                        }
                        Console.WriteLine("Found '{0}' counters.", info.Count());
                    }
                    else
                        Console.WriteLine("No valid performance counters found.", info.Count());
                }
                else
                    Console.WriteLine("The '{0}' path did not match any executables or dlls.", options.Path);

            }
            catch (CommandLineException exception)
            {
                Console.WriteLine(exception.ArgumentHelp.Message);
                Console.WriteLine(exception.ArgumentHelp.GetHelpText(Console.BufferWidth));
            }
        }

        internal static void Execute(Options options)
        {
            
        }

        private static IEnumerable<PerformanceInformation> Load(string assemblyPath)
        {
            //Creates an AssemblyDefinition from the "MyLibrary.dll" assembly
            AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(assemblyPath);
            ModuleDefinition[] referencingModules;
            if (assembly.Name.Name != "WebApplications.Utilities.Logging")
            {
                // Find any modules that reference logging.
                referencingModules = assembly.Modules
                                             .Where(m => m.AssemblyReferences
                                                          .FirstOrDefault(
                                                              ar => ar.Name == "WebApplications.Utilities.Logging") !=
                                                         null)
                                             .ToArray();
            }
            else
            {
                referencingModules = assembly.Modules.ToArray();
            }

            if (referencingModules.Length < 1)
                yield break;

            Queue<string> lastStrings = new Queue<string>(2);
            foreach (var module in referencingModules)
            {
                foreach (TypeDefinition type in module.Types)
                {
                    foreach (MethodDefinition method in type.Methods)
                    {
                        if (!method.HasBody) continue;

                        lastStrings.Clear();
                        foreach (Instruction instr in method.Body.Instructions)
                        {
                            if (instr.OpCode.Code == Code.Ldstr)
                            {
                                // We track last two load strings.
                                if (lastStrings.Count > 1)
                                    lastStrings.Dequeue();

                                lastStrings.Enqueue(instr.Operand as string);
                                continue;
                            }
                            if (instr.OpCode.Code == Code.Ldsfld)
                            {
                                continue;
                            }

                            if (instr.OpCode.Code != Code.Newobj)
                            {
                                // If we have any ops other than NewObj after our loads then the loads aren't for us.
                                lastStrings.Clear();
                                continue;
                            }

                            MethodReference methodReference = instr.Operand as MethodReference;
                            if (methodReference == null) continue;

                            TypeReference typeReference = methodReference.DeclaringType;
                            if ((typeReference == null) ||
                                (typeReference.FullName !=
                                "WebApplications.Utilities.Logging.Performance.PerformanceInformation"))
                                continue;

                            if (lastStrings.Count > 1)
                            {
                                int pCount = methodReference.Parameters.Count();
                                bool isTimer = pCount == 4;
                                Contract.Assert(isTimer || (pCount == 2));
                                string categoryName = lastStrings.Dequeue();
                                string categoryHelp = lastStrings.Dequeue();

                                if (!string.IsNullOrWhiteSpace(categoryName) &&
                                    !string.IsNullOrWhiteSpace(categoryHelp))
                                {
                                    // We have a constructor, create our own performance counter info.
                                    yield return
                                        new PerformanceInformation(assembly.Name.Name, categoryName, categoryHelp, isTimer);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Performance counter creation found in '{0}' but could not find category name and or category help - make sure you use inline strings as constructor parameters.  Press any key to continue");
                                Console.ReadKey();
                            }
                            lastStrings.Clear();
                        }
                    }
                }
            }
        }
    }
}
