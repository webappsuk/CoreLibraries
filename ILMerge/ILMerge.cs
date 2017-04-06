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

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WebApplications.Utilities.Annotations;

namespace ILMerge.Build.Task
{
    /// <summary>
    /// The possible assembly kinds.
    /// </summary>
    public enum Kind
    {
        /// <summary>
        /// A library
        /// </summary>
        [UsedImplicitly]
        Dll,

        /// <summary>
        /// A console application.
        /// </summary>
        [UsedImplicitly]
        Exe,

        /// <summary>
        /// A Windows application.
        /// </summary>
        [UsedImplicitly]
        WinExe,
    }

    /// <summary>
    /// ILMerge build task.
    /// </summary>
    [UsedImplicitly]
    public class ILMerge : Microsoft.Build.Utilities.Task
    {
        private string _attributeFile;
        private string _excludeFile;
        private string _logFile;
        private string _outputFile;
        private string _keyFile;
        private string _targetKind;

        /// <summary>
        /// Gets or sets the attribute file.
        /// </summary>
        /// <value>
        /// The attribute file.
        /// </value>
        [UsedImplicitly]
        public virtual string AttributeFile
        {
            get { return _attributeFile; }
            set
            {
                _attributeFile = ConvertEmptyToNull(value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the the "transitive closure" of the input assemblies is computed and added to the list of input assemblies.
        /// </summary>
        /// <remarks>An assembly is considered part of the transitive closure if it is referenced, either directly or indirectly, 
        /// from one of the originally specified input assemblies and it has an external reference to one of the input assemblies, 
        /// or one of the assemblies that has such a reference.</remarks>
        /// <value>
        ///   <see langword="true" /> if closed; otherwise, <see langword="false" />.
        /// </value>
        [UsedImplicitly]
        public virtual bool Closed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the assembly level attributes of each input assembly should be copied over into the target assembly.
        /// </summary>
        /// <value>
        ///   <see langword="true" /> if the assembly level attributes of each input assembly should be copied over into the target assembly.; otherwise, <see langword="false" />.
        /// </value>
        [UsedImplicitly]
        public virtual bool CopyAttributes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to create a .pdb file for the output assembly and merge into it any .pdb files found for input assemblies.
        /// </summary>
        /// <value>
        ///   <see langword="true" /> if a .pdb file should be created for the output assembly and merge into it any .pdb files found for input assemblies; otherwise, <see langword="false" />.
        /// </value>
        [UsedImplicitly]
        public virtual bool DebugInfo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether XML documentation files should be merged.
        /// </summary>
        /// <value>
        ///   <see langword="true" /> if XML documentation files should be merged; otherwise, <see langword="false" />.
        /// </value>
        [UsedImplicitly]
        public virtual bool MergeXml { get; set; }

        /// <summary>
        /// Gets or sets the regex to exclude files from being internalised.
        /// </summary>
        /// <value>
        /// The exclude file.
        /// </value>
        [UsedImplicitly]
        public virtual string ExcludeFile
        {
            get { return _excludeFile; }
            set
            {
                _excludeFile = ConvertEmptyToNull(value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether types in assemblies other than the primary assembly have their visibility modified.
        /// </summary>
        /// <remarks>When it is true, then all non-exempt types that are visible outside of their assembly have their visibility modified 
        /// so that they are not visible from outside of the merged assembly. A type is exempt if its full name matches a line from the 
        /// <see cref="ExcludeFile"/> using the .NET regular expression engine.</remarks>
        /// <value>
        ///   <see langword="true" /> if types in assemblies other than the primary assembly have their visibility modified; otherwise, <see langword="false" />.
        /// </value>
        [UsedImplicitly]
        public virtual bool Internalize { get; set; }

        /// <summary>
        /// Gets or sets the directories to be used to search for input assemblies.
        /// </summary>
        /// <value>
        /// The library path.
        /// </value>
        [UsedImplicitly]
        [ItemNotNull]
        public virtual ITaskItem[] LibraryPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether log messages are written.
        /// </summary>
        /// <remarks>If <see cref="Log"/> is true, but <see cref="LogFile"/> is null, then log messages are written to <see cref="Console.Out"/>.</remarks>
        /// <value>
        ///   <see langword="true" /> if log messages are written; otherwise, <see langword="false" />.
        /// </value>
        [UsedImplicitly]
        public virtual bool ShouldLog { get; set; }

        /// <summary>
        /// Gets or sets the path and filename that log messages are written to.
        /// </summary>
        /// <value>
        /// The log file.
        /// </value>
        [UsedImplicitly]
        public virtual string LogFile
        {
            get { return _logFile; }
            set
            {
                _logFile = ConvertEmptyToNull(value);
            }
        }

        /// <summary>
        /// Gets or sets the path and filename that the target assembly will be written to.
        /// </summary>
        /// <value>
        /// The output file.
        /// </value>
        [Required]
        [UsedImplicitly]
        public virtual string OutputFile
        {
            get { return _outputFile; }
            set
            {
                _outputFile = ConvertEmptyToNull(value);
            }
        }

        /// <summary>
        /// Gets or sets the path and filename to a .snk file.
        /// </summary>
        /// <remakrs>The target assembly will be signed with its contents and will then have a strong name.
        ///  It can be used with the DelaySign property (Section 2.9) to have the target assembly delay signed.
        ///  This can be done even if the primary assembly was fully signed.</remakrs>
        /// <value>
        /// The SNK file.
        /// </value>
        [UsedImplicitly]
        public virtual string SnkFile
        {
            get { return _keyFile; }
            set
            {
                _keyFile = ConvertEmptyToNull(value);
            }
        }

        /// <summary>
        /// Gets or sets the input assemblies.
        /// </summary>
        /// <value>
        /// The input assemblies.
        /// </value>
        [Required]
        [UsedImplicitly]
        [NotNull]
        public virtual ITaskItem[] InputAssemblies { get; set; }

        /// <summary>
        /// Gets or sets the whether the target assembly is created as a library, a console application or as a Windows application.
        /// </summary>
        /// <value>
        /// The kind of the target assembly.
        /// </value>
        [UsedImplicitly]
        public string TargetKind
        {
            get { return _targetKind; }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (Enum.IsDefined(typeof(Kind), value))
                    _targetKind = value;
                else
                {
                    // ReSharper disable once PossibleNullReferenceException
                    Log.LogWarning(
                        "TargetKind should be one of [Exe|Dll|WinExe] or null; set to null");
                    _targetKind = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the ILMerge tool path.
        /// </summary>
        /// <value>
        /// The il merge tool.
        /// </value>
        [Required]
        [UsedImplicitly]
        public string ILMergeTool { get; set; }

        /// <summary>
        /// Gets or sets the target framework version.
        /// </summary>
        /// <value>The target framework version.</value>
        [Required]
        [UsedImplicitly]
        public string TargetFrameworkVersion { get; set; }

        /// <summary>
        /// Gets or sets the target platform.
        /// </summary>
        /// <value>The target platform.</value>
        [Required]
        [UsedImplicitly]
        public string TargetPlatform { get; set; }

        /// <summary>
        /// Executes the task.
        /// </summary>
        public override bool Execute()
        {
            CommandLineBuilder commandLine = new CommandLineBuilder();
            commandLine.AppendSwitchIfNotNull("/attr:", _attributeFile);
            if (Closed) commandLine.AppendSwitch("/closed");
            if (CopyAttributes) commandLine.AppendSwitch("/copyattrs");
            if (DebugInfo) commandLine.AppendSwitch("/ndebug");
            if (Internalize) commandLine.AppendSwitch("/internalize:" + ExcludeFile);
            if (ShouldLog)
                if (LogFile == null) commandLine.AppendSwitch("/log");
                else
                {
                    commandLine.AppendSwitch("/log:");
                    commandLine.AppendFileNameIfNotNull(LogFile);
                }
            commandLine.AppendSwitchIfNotNull("/keyfile:", SnkFile);
            commandLine.AppendSwitchIfNotNull("/target:", TargetKind);
            if (MergeXml) commandLine.AppendSwitch("/xmldocs");

            TargetDotNetFrameworkVersion frameworkVersion;
            string framework;
            switch (TargetFrameworkVersion?.Trim().ToLowerInvariant())
            {
                //v2.0, v3.0, v3.5, v4.0, v4.5, and v4.5.1.
                case "v2.0":
                    frameworkVersion = TargetDotNetFrameworkVersion.Version20;
                    framework = "v2";
                    break;
                case "v3.0":
                    frameworkVersion = TargetDotNetFrameworkVersion.Version30;
                    framework = "v2";
                    break;
                case "v3.5":
                    frameworkVersion = TargetDotNetFrameworkVersion.Version35;
                    framework = "v2";
                    break;
                case "v4.0":
                    frameworkVersion = TargetDotNetFrameworkVersion.Version40;
                    framework = "v4";
                    break;
                // ReSharper disable RedundantCaseLabel
                case "v4.5":
                case "v4.5.1":
                case "v4.5.2":
                case "v4.6":
                // ReSharper restore RedundantCaseLabel
                default:
                    frameworkVersion = TargetDotNetFrameworkVersion.Version45;
                    framework = "v4";
                    break;
            }

            DotNetFrameworkArchitecture platformArchitecture;
            switch (TargetPlatform?.Trim().ToLowerInvariant())
            {
                case "x86":
                    platformArchitecture = DotNetFrameworkArchitecture.Bitness32;
                    break;
                case "x64":
                    platformArchitecture = DotNetFrameworkArchitecture.Bitness64;
                    break;
                default:
                    platformArchitecture = DotNetFrameworkArchitecture.Current;
                    break;
            }

            string toolPath = ToolLocationHelper.GetPathToDotNetFramework(
                frameworkVersion,
                platformArchitecture);

            Debug.Assert(Log != null);

            Log.LogMessage(
                MessageImportance.Normal,
                "Merge Framework: {0}, {1}",
                TargetFrameworkVersion,
                toolPath);

            commandLine.AppendSwitch($"/targetplatform:{framework},{toolPath}");

            if (LibraryPath != null)
            {
                List<string> list = new List<string>(LibraryPath.Select(taskItem => taskItem.ItemSpec)) { "." };
                foreach (string dir in list)
                    commandLine.AppendSwitchIfNotNull("/lib:", dir);
            }

            commandLine.AppendSwitchIfNotNull("/out:", OutputFile);

            commandLine.AppendFileNamesIfNotNull(InputAssemblies, " ");

            try
            {
                Log.LogMessage(
                    MessageImportance.Normal,
                    "Merging {0} assembl{1} to '{2}'.",
                    InputAssemblies.Length,
                    InputAssemblies.Length != 1 ? "ies" : "y",
                    _outputFile);

                Log.LogMessage(MessageImportance.Low, ILMergeTool + " " + commandLine);

                Process proc = new Process
                {
                    StartInfo = new ProcessStartInfo(ILMergeTool, commandLine.ToString())
                    {
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        UseShellExecute = false
                    }
                };
                proc.OutputDataReceived += (sender, args) =>
                {
                    Debug.Assert(args != null);

                    // Null is sent when the stream closes, so skip it
                    if (!string.IsNullOrEmpty(args.Data))
                        Log.LogMessage(args.Data);
                };
                proc.ErrorDataReceived += (sender, args) =>
                {
                    Debug.Assert(args != null);

                    // Null is sent when the stream closes, so skip it
                    if (!string.IsNullOrEmpty(args.Data))
                        Log.LogError(args.Data);
                };
                proc.Start();
                proc.BeginOutputReadLine();
                proc.BeginErrorReadLine();
                proc.WaitForExit();
                if (proc.ExitCode != 0)
                    Log.LogError("ILMerge exited with error code {0}.", proc.ExitCode);
                else
                    Log.LogMessage("ILMerge completed successfully.");
                return proc.ExitCode == 0;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private static string ConvertEmptyToNull(string iti) => !string.IsNullOrEmpty(iti) ? iti : null;
    }
}