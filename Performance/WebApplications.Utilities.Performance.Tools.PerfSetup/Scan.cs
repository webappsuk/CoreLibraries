using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Mono.Cecil;
using Mono.Cecil.Cil;
using WebApplications.Utilities;

namespace WebApplications.Utilities.Performance.Tools.PerfSetup
{
    /// <summary>
    /// Class Scan
    /// </summary>
    public static class Scan
    {
        /// <summary>
        /// The assembly resolver.
        /// </summary>
        public static readonly DefaultAssemblyResolver AssemblyResolver;
        public static readonly ReaderParameters ReaderParameters;

        static Scan()
        {
            AssemblyResolver = new DefaultAssemblyResolver();
            ReaderParameters = new ReaderParameters() { AssemblyResolver = AssemblyResolver };
        }

        /// <summary>
        /// Scans the supplied <see paramref="fullPath" /> (to a directory or assembly) for performance counters and adds/deletes/lists
        /// them depending on the <see paramref="mode" />.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="fullPath">The assemblies path.</param>
        /// <param name="machineName">Name of the machine.</param>
        public static void Execute(
            ScanMode mode,
            [NotNull] string fullPath,
            [NotNull] string machineName = ".")
        {
            // Check we have access to the performance counters.
            PerformanceCounterCategory.Exists("TestAccess", machineName);

            fullPath = Path.GetFullPath(fullPath.Trim());
            string[] parts = fullPath.Split('/', '\\');
            if (parts.Length < 1)
            {
                Logger.Add(Level.Error, "The '{0}' path was invalid.", fullPath);
                return;
            }

            // Handle trailing path seperators.
            if (string.IsNullOrWhiteSpace(parts.Last()))
                Array.Resize(ref parts, parts.Length - 1);

            string path = String.Join("\\", parts.Take(parts.Length - 1));
            if (!Directory.Exists(path))
            {
                Logger.Add(Level.Error, "The '{0}' directory could not be found.", fullPath);
                return;
            }
            string end = parts.Last();
            string directory = Directory.GetDirectories(path, end).SingleOrDefault();
            string[] files;
            if (directory != null) files = Directory.GetFiles(directory);
            else
            {
                directory = path;
                if (!Directory.Exists(directory))
                {
                    Logger.Add(Level.Error, "The '{0}' directory could not be found.", directory);
                    return;
                }
                files = Directory.GetFiles(path, end);
            }
            Contract.Assert(files != null);
            Contract.Assert(directory != null);

            AssemblyResolver.AddSearchDirectory(directory);

            files = files.Where(
                f =>
                {
                    string ext = Path.GetExtension(f).ToLower();
                    return (ext == ".dll" || ext == ".exe");
                }).ToArray();

            if (files.Any())
            {
                foreach (string file in files)
                    Load(file);

                int succeeded = 0;
                int failed = 0;
                foreach (PerfCategory performanceInformation in PerfCategory.All)
                {
                    bool success = false;
                    switch (mode)
                    {
                        case ScanMode.Add:
                            bool added = performanceInformation.Create(machineName);
                            Logger.Add(
                                added ? Level.Normal : Level.Error, 
                                "Adding '{0}' {1}",
                                performanceInformation,
                                added ? "succeeded" : "failed");
                            if (added)
                                succeeded++;
                            else
                                failed++;
                            break;
                        case ScanMode.Delete:
                            bool deleted = performanceInformation.Delete(machineName);
                            Logger.Add(
                                deleted ? Level.Normal : Level.Error, 
                                "Deleting '{0}' {1}",
                                performanceInformation,
                                deleted ? "succeeded" : "failed");
                            if (deleted)
                                succeeded++;
                            else
                                failed++;
                            break;
                        default:
                            // Treat everything else as list.
                            bool exists = performanceInformation.Exists;
                            Logger.Add(
                                exists ? Level.Normal : Level.Error, 
                                "'{0}' {1}",
                                performanceInformation,
                                exists ? "exists" : "is missing");
                            if (exists)
                                succeeded++;
                            else
                                failed++;
                            break;
                    }
                }

                if (succeeded + failed > 0)
                {
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
                            operation = "Found";
                            break;
                    }
                    Logger.Add(Level.High, "{0} '{1}' performance counters.{2}", operation, succeeded,
                               failed > 0 ? string.Format(" {0} failures.", failed) : string.Empty);
                }
                else
                    Logger.Add(Level.High, "No valid performance counters found.");
            }
            else
                Logger.Add(Level.Warning, "The '{0}' path did not match any executables or dlls, so no performance counters added.", fullPath);
        }

        /// <summary>
        /// Loads the specified assembly and checks for performance counter use.
        /// </summary>
        /// <param name="assemblyPath">The assembly path.</param>
        private static void Load(string assemblyPath)
        {
            AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(assemblyPath, ReaderParameters);
            if (assembly.Name.Name == "WebApplications.Utilities.Performance")
                return;

            // Find any modules that reference logging.
            ModuleDefinition[] referencingModules = assembly.Modules
                                                            .Where(m => m.AssemblyReferences
                                                                         .FirstOrDefault(
                                                                             ar => ar.Name == "WebApplications.Utilities.Performance") !=
                                                                        null)
                                                            .ToArray();

            if (referencingModules.Length < 1)
                return;

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
                            // Detect string literals loaded onto evaluation stack
                            if (instr.OpCode.Code == Code.Ldstr)
                            {
                                // We track last two load strings.
                                if (lastStrings.Count > 1)
                                    lastStrings.Dequeue();

                                lastStrings.Enqueue(instr.Operand as string);
                                continue;
                            }

                            // Detect nulls being loaded onto evaluation stack
                            if (instr.OpCode.Code == Code.Ldnull)
                            {
                                // We track last two load strings.
                                if (lastStrings.Count > 1)
                                    lastStrings.Dequeue();

                                lastStrings.Enqueue(null);
                                continue;
                            }

                            if (instr.OpCode.Code != Code.Call)
                            {
                                // If we have any ops other than NewObj after our loads then the loads aren't for us.
                                lastStrings.Clear();
                                continue;
                            }

                            // Make sure we have the right method signature.
                            GenericInstanceMethod methodReference = instr.Operand as GenericInstanceMethod;
                            if ((methodReference == null) ||
                                (methodReference.Name != "GetOrAdd") ||
                                !methodReference.HasGenericArguments ||
                                !methodReference.IsGenericInstance ||
                                (methodReference.GenericArguments.Count != 1) ||
                                (methodReference.Parameters.Count != 2) ||
                                (methodReference.Parameters[0].ParameterType.FullName != "System.String") ||
                                (methodReference.Parameters[1].ParameterType.FullName != "System.String"))
                                continue;

                            // Make sure it's on the right type.
                            TypeReference typeReference = methodReference.DeclaringType;
                            if ((typeReference == null) ||
                                (typeReference.FullName !=
                                 "WebApplications.Utilities.Performance.PerfCategory"))
                                continue;

                            TypeReference PerfCategoryType = methodReference.GenericArguments.First();
                            Contract.Assert(PerfCategoryType != null);

                            if (lastStrings.Count > 1)
                            {
                                int pCount = methodReference.Parameters.Count();
                                bool isTimer = pCount == 4;
                                Contract.Assert(isTimer || (pCount == 2));
                                string categoryName = lastStrings.Dequeue();
                                string categoryHelp = lastStrings.Dequeue();

                                // We have a constructor set performance information.
                                if (!String.IsNullOrWhiteSpace(categoryName))
                                {
                                    Logger.Add(
                                        Level.Low,
                                        "The '{0}' assembly calls PerfCategory.GetOrAdd<{1}>(\"{2}\", {3}).",
                                        assemblyPath,
                                        PerfCategoryType.Name,
                                        categoryName,
                                        categoryHelp == null ? "null" : "\"" + categoryHelp + "\"");

                                    PerfCategory.Set(categoryName, PerfCategoryType, assembly.Name.Name,
                                                     categoryHelp);
                                }
                                else
                                    Logger.Add(Level.Error,
                                               "Performance counter creation found in '{0}' but category name was null or empty - make sure you use inline strings as constructor parameters.  Press any key to continue",
                                               assemblyPath);
                            }
                            else
                            {
                                Logger.Add(Level.Error, "Performance counter creation found in '{0}' but could not find category name and or category help - make sure you use inline strings as constructor parameters.  Press any key to continue", assemblyPath);
                            }
                            lastStrings.Clear();
                        }
                    }
                }
            }
        }
    }
}