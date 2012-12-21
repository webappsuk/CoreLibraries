using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace WebApplications.Utilities.Performance.Tools.PerfSetup
{
    /// <summary>
    /// Class Scan
    /// </summary>
    public static class Scan
    {
        /// <summary>
        /// Scans the supplied <see paramref="fullPath"/> (to a directory or assembly) for performance counters and adds/deletes/lists
        /// them depending on the <see paramref="mode"/>.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="fullPath">The assemblies path.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="machineName">Name of the machine.</param>
        public static void Execute(
            ScanMode mode,
            [NotNull] string fullPath,
            [NotNull] Action<string, Level> logger,
            [NotNull] string machineName = ".")
        {
            // Check we have access to the performance counters.
            PerformanceCounterCategory.Exists("TestAccess", machineName);

            fullPath = Path.GetFullPath(fullPath.Trim());
            string[] parts = fullPath.Split('/', '\\');
            if (parts.Length < 1)
            {
                logger(String.Format("The '{0}' path was invalid.", fullPath), Level.Error);
                return;
            }

            // Handle trailing path seperators.
            if (string.IsNullOrWhiteSpace(parts.Last()))
                Array.Resize(ref parts, parts.Length - 1);

            string path = String.Join("\\", parts.Take(parts.Length - 1));
            if (!Directory.Exists(path))
            {
                logger(String.Format("The '{0}' directory could not be found.", fullPath), Level.Error);
                return;
            }
            string end = parts.Last();
            string directory = Directory.GetDirectories(path, end).SingleOrDefault();
            string[] files;
            files = directory != null ? Directory.GetFiles(directory) : Directory.GetFiles(path, end);
            Contract.Assert(files != null);

            files = files.Where(
                f =>
                    {
                        string ext = Path.GetExtension(f).ToLower();
                        return (ext == ".dll" || ext == ".exe");
                    }).ToArray();

            if (files.Any())
            {
                PerformanceInformation[] info = files.SelectMany<string, PerformanceInformation>(file => Load(file, logger)).ToArray();
                if (info.Length > 0)
                {
                    foreach (PerformanceInformation performanceInformation in info)
                    {
                        switch (mode)
                        {
                            case ScanMode.Add:
                                bool added = performanceInformation.Create(machineName);
                                logger(String.Format(
                                    "Adding '{0}' {1}",
                                    performanceInformation,
                                    added ? "succeeded" : "failed"), added ? Level.Normal : Level.Error);
                                break;
                            case ScanMode.Delete:
                                bool deleted = performanceInformation.Delete(machineName);
                                logger(String.Format(
                                    "Deleting '{0}' {1}",
                                    performanceInformation,
                                    deleted ? "succeeded" : "failed"), deleted ? Level.Normal : Level.Error);
                                break;
                            default:
                                // Treat everything else as list.
                                bool exists = performanceInformation.Exists;
                                logger(String.Format("{0} - {1}", performanceInformation,
                                                     exists ? "Exists" : "Missing"),
                                       exists ? Level.Normal : Level.Error);
                                break;
                        }
                    }
                    string operation;
                    switch (mode)
                    {
                        case ScanMode.Add:
                            operation = "Added";
                            break;
                        case ScanMode.Delete:
                            operation = "Deleted";
                            break;
                        default:
                            operation = "Listed";
                            break;
                    }
                    logger(String.Format("{0} '{1}' performance counters.", operation, info.Count()), Level.High);
                }
                else
                    logger(String.Format("No valid performance counters found."), Level.High);
            }
            else
                logger(String.Format("The '{0}' path did not match any executables or dlls, so no performance counters added.", fullPath),
                       Level.Warning);
        }

        public static IEnumerable<PerformanceInformation> Load(string assemblyPath, [NotNull] Action<string, Level> logger)
        {
            //Creates an AssemblyDefinition from the "MyLibrary.dll" assembly
            AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(assemblyPath);
            ModuleDefinition[] referencingModules;
            if (assembly.Name.Name != "WebApplications.Utilities.Performance")
            {
                // Find any modules that reference logging.
                referencingModules = assembly.Modules
                                             .Where(m => m.AssemblyReferences
                                                          .FirstOrDefault(
                                                              ar => ar.Name == "WebApplications.Utilities.Performance") !=
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
                                 "WebApplications.Utilities.Performance.PerformanceInformation"))
                                continue;

                            if (lastStrings.Count > 1)
                            {
                                int pCount = methodReference.Parameters.Count();
                                bool isTimer = pCount == 4;
                                Contract.Assert(isTimer || (pCount == 2));
                                string categoryName = lastStrings.Dequeue();
                                string categoryHelp = lastStrings.Dequeue();

                                if (!String.IsNullOrWhiteSpace(categoryName) &&
                                    !String.IsNullOrWhiteSpace(categoryHelp))
                                {
                                    // We have a constructor, create our own performance counter info.
                                    yield return
                                        new PerformanceInformation(assembly.Name.Name, categoryName, categoryHelp, isTimer);
                                }
                            }
                            else
                            {
                                logger(String.Format("Performance counter creation found in '{0}' but could not find category name and or category help - make sure you use inline strings as constructor parameters.  Press any key to continue"), Level.Error);
                            }
                            lastStrings.Clear();
                        }
                    }
                }
            }
        }
    }
}