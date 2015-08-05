#region © Copyright Web Applications (UK) Ltd, 2015.  All rights reserved.
// Copyright (c) 2015, Web Applications UK Ltd
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace WebApplications.Utilities.Performance.Tools.PerfSetup
{
    /// <summary>
    /// Class Scan
    /// </summary>
    [PublicAPI]
    public static class Scan
    {
        /// <summary>
        /// The assembly resolver.
        /// </summary>
        [NotNull]
        public static readonly DefaultAssemblyResolver AssemblyResolver;

        /// <summary>
        /// The reader parameters.
        /// </summary>
        [NotNull]
        public static readonly ReaderParameters ReaderParameters;

        static Scan()
        {
            AssemblyResolver = new DefaultAssemblyResolver();
            ReaderParameters = new ReaderParameters { AssemblyResolver = AssemblyResolver };
        }

        /// <summary>
        /// Scans the supplied <see paramref="fullPath" /> (to a directory or assembly) for performance counters and adds/deletes/lists
        /// them depending on the <see paramref="mode" />.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="fullPath">The assemblies path.</param>
        /// <param name="executeAgain">if set to <see langword="true" /> execute's again on 64-bit OS.</param>
        public static void Execute(
            ScanMode mode,
            [NotNull] string fullPath,
            bool executeAgain)
        {
            if (fullPath == null) throw new ArgumentNullException("fullPath");

            // Check we have access to the performance counters.
            PerformanceCounterCategory.Exists("TestAccess");

            string[] files;
            string directory;
            try
            {
                fullPath = Path.GetFullPath(fullPath.Trim());
                if (File.Exists(fullPath))
                {
                    directory = Path.GetDirectoryName(fullPath);
                    files = new[] { fullPath };
                }
                else if (Directory.Exists(fullPath))
                {
                    directory = fullPath;
                    files = Directory.GetFiles(fullPath)
                        .Where(
                            f =>
                            {
                                // ReSharper disable once PossibleNullReferenceException
                                string ext = Path.GetExtension(f).ToLower();
                                return (ext == ".dll" || ext == ".exe");
                            }).ToArray();
                }
                else
                {
                    Logger.Add(Level.Error, "The '{0}' path is neither a file or directory.", fullPath);
                    return;
                }
            }
            catch (Exception e)
            {
                Logger.Add(Level.Error, "The '{0}' path was invalid. {1}", fullPath, e.Message);
                return;
            }

            Debug.Assert(files != null);
            Debug.Assert(directory != null);

            if (!files.Any())
            {
                Logger.Add(
                    Level.Warning,
                    "The '{0}' path did not match any executables or dlls, so no performance counters added.",
                    fullPath);
                return;
            }

            AssemblyResolver.AddSearchDirectory(directory);
            foreach (string file in files)
                Load(file);

            int succeeded = 0;
            int failed = 0;
            foreach (PerfCategory performanceInformation in PerfCategory.All)
                switch (mode)
                {
                    case ScanMode.Add:
                        bool added = performanceInformation.Create();
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
                        bool deleted = performanceInformation.Delete();
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
                Logger.Add(
                    Level.High,
                    "{0} '{1}' performance counters.{2}",
                    operation,
                    succeeded,
                    failed > 0 ? string.Format(" {0} failures.", failed) : string.Empty);

                if (executeAgain &&
                    Environment.Is64BitOperatingSystem)
                {
                    bool bit64 = Environment.Is64BitProcess;
                    Logger.Add(
                        Level.High,
                        "Running PerfSetup in {0} bit process on 64 bit operating system, will run again in {1} bit mode to ensure counters are added to both environments!",
                        bit64 ? 64 : 32,
                        bit64 ? 32 : 64);
                    ExecuteAgain(mode, fullPath, !bit64);
                }
            }
            else
                Logger.Add(Level.High, "No valid performance counters found.");
        }

        /// <summary>
        /// Loads the specified assembly and checks for performance counter use.
        /// </summary>
        /// <param name="assemblyPath">The assembly path.</param>
        private static void Load(string assemblyPath)
        {
            AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(assemblyPath, ReaderParameters);
            Debug.Assert(assembly != null);
            Debug.Assert(assembly.Name != null);
            if (assembly.Name.Name == "WebApplications.Utilities.Performance")
                return;

            // Find any modules that reference logging.
            // ReSharper disable once AssignNullToNotNullAttribute
            ModuleDefinition[] referencingModules = assembly.Modules
                .Where(
                    // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute
                    m => m.AssemblyReferences
                        .FirstOrDefault(
                            ar => ar.Name == "WebApplications.Utilities.Performance") !=
                         null)
                // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute
                .ToArray();

            if (referencingModules.Length < 1)
                return;

            Queue<string> lastStrings = new Queue<string>(2);
            foreach (ModuleDefinition module in referencingModules)
            {
                Debug.Assert(module != null);
                Debug.Assert(module.Types != null);
                foreach (TypeDefinition type in module.Types)
                {
                    Debug.Assert(type != null);
                    Debug.Assert(type.Methods != null);
                    foreach (MethodDefinition method in type.Methods)
                    {
                        Debug.Assert(method != null);

                        if (!method.HasBody) continue;

                        lastStrings.Clear();

                        Debug.Assert(method.Body != null);
                        Debug.Assert(method.Body.Instructions != null);
                        foreach (Instruction instr in method.Body.Instructions)
                        {
                            Debug.Assert(instr != null);

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
                                // ReSharper disable PossibleNullReferenceException
                                (methodReference.GenericArguments.Count != 1) ||
                                (methodReference.Parameters.Count != 2) ||
                                (methodReference.Parameters[0].ParameterType.FullName != "System.String") ||
                                (methodReference.Parameters[1].ParameterType.FullName != "System.String"))
                                // ReSharper restore PossibleNullReferenceException
                                continue;

                            // Make sure it's on the right type.
                            TypeReference typeReference = methodReference.DeclaringType;
                            if ((typeReference == null) ||
                                (typeReference.FullName !=
                                 "WebApplications.Utilities.Performance.PerfCategory"))
                                continue;

                            TypeReference perfCategoryType = methodReference.GenericArguments.First();
                            Debug.Assert(perfCategoryType != null);

                            if (lastStrings.Count > 1)
                            {
                                int pCount = methodReference.Parameters.Count();
                                bool isTimer = pCount == 4;
                                Debug.Assert(isTimer || (pCount == 2));
                                string categoryName = lastStrings.Dequeue();
                                string categoryHelp = lastStrings.Dequeue();

                                // We have a constructor set performance information.
                                if (!String.IsNullOrWhiteSpace(categoryName))
                                {
                                    Logger.Add(
                                        Level.Low,
                                        "The '{0}' assembly calls PerfCategory.GetOrAdd<{1}>(\"{2}\", {3}).",
                                        assemblyPath,
                                        perfCategoryType.Name,
                                        categoryName,
                                        categoryHelp == null ? "null" : "\"" + categoryHelp + "\"");

                                    PerfCategory.Set(
                                        categoryName,
                                        perfCategoryType,
                                        // ReSharper disable once AssignNullToNotNullAttribute
                                        assembly.Name.Name,
                                        categoryHelp);
                                }
                                else
                                    Logger.Add(
                                        Level.Error,
                                        "Performance counter creation found in '{0}' but category name was null or empty - make sure you use inline strings as constructor parameters.  Press any key to continue",
                                        assemblyPath);
                            }
                            else
                                Logger.Add(
                                    Level.Error,
                                    "Performance counter creation found in '{0}' but could not find category name and or category help - make sure you use inline strings as constructor parameters.  Press any key to continue",
                                    assemblyPath);
                            lastStrings.Clear();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Launches a new process in the opposite mode (64/32 bit).
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="fullPath">The full path.</param>
        /// <param name="bit64">if set to <see langword="true" /> [bit64].</param>
        private static void ExecuteAgain(
            ScanMode mode,
            [NotNull] string fullPath,
            bool bit64)
        {
            string executable = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) +
                                @"\PerfSetup" +
                                (bit64
                                    ? "64"
                                    : "32") +
                                ".exe";

            if (!File.Exists(executable))
            {
                Logger.Add(
                    Level.Error,
                    "Could not find '{0}' executable, could not add performance counters in {1} bit mode.",
                    executable,
                    bit64 ? 64 : 32);
                return;
            }

            try
            {
                if (fullPath.EndsWith(@"\"))
                    fullPath = fullPath.Substring(0, fullPath.Length - 1);

                ProcessStartInfo processStartInfo = new ProcessStartInfo(
                    executable,
                    string.Format(
                        "/E- {0} \"{1}\"",
                        mode,
                        fullPath))
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                };

                Logger.Add(Level.Low, "Executing: {0} {1}", processStartInfo.FileName, processStartInfo.Arguments);

                Process process = Process.Start(processStartInfo);
                Debug.Assert(process != null);

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                Logger.Add(Level.Low, output);
                Logger.Add(Level.High, "Completed execution in {0} mode.", bit64 ? 64 : 32);
            }
            catch (Exception e)
            {
                Logger.Add(Level.Error, "Failed execution in {0} mode. {1}", bit64 ? 64 : 32, e.Message);
            }
        }
    }
}