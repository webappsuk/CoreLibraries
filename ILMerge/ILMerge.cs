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
using JetBrains.Annotations;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace ILMerge.Build.Tasks
{
    public enum Kind
    {
        Dll,
        Exe,
        WinExe,
    }

    [UsedImplicitly]
    public class ILMerge : Task
    {
        private string _attributeFile;
        private string _excludeFile;
        private string _logFile;
        private string _outputFile;
        private string _keyFile;
        private string _targetKind;
        private string _ilMergeTool;

        [UsedImplicitly]
        public virtual string AttributeFile
        {
            get { return _attributeFile; }
            set
            {
                _attributeFile = ConvertEmptyToNull(value);
            }
        }

        [UsedImplicitly]
        public virtual bool Closed { get; set; }

        [UsedImplicitly]
        public virtual bool CopyAttributes { get; set; }

        [UsedImplicitly]
        public virtual bool DebugInfo { get; set; }

        [UsedImplicitly]
        public virtual bool MergeXml { get; set; }

        [UsedImplicitly]
        public virtual string ExcludeFile
        {
            get { return _excludeFile; }
            set
            {
                _excludeFile = ConvertEmptyToNull(value);
            }
        }

        [UsedImplicitly]
        public virtual bool Internalize { get; set; }

        [UsedImplicitly]
        public virtual ITaskItem[] LibraryPath { get; set; }

        [UsedImplicitly]
        public virtual bool ShouldLog { get; set; }

        [UsedImplicitly]
        public virtual string LogFile
        {
            get { return _logFile; }
            set
            {
                _logFile = ConvertEmptyToNull(value);
            }
        }

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

        [UsedImplicitly]
        public virtual string SnkFile
        {
            get { return _keyFile; }
            set
            {
                _keyFile = ConvertEmptyToNull(value);
            }
        }

        [Required]
        [UsedImplicitly]
        public virtual ITaskItem[] InputAssemblies { get; set; }

        [UsedImplicitly]
        public string TargetKind
        {
            get { return _targetKind; }
            set
            {
                if (Enum.IsDefined(typeof(Kind), value))
                    _targetKind = value;
                else
                {
                    Log.LogWarning(
                        "TargetKind should be one of [Exe|Dll|WinExe] or null; set to null");
                    _targetKind = null;
                }
            }
        }

        [Required]
        [UsedImplicitly]
        public string ILMergeTool
        {
            get { return _ilMergeTool; }
            set
            {
                _ilMergeTool = value;
            }
        }

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
            string framework = BuildEngine.GetEnvironmentVariable("TargetFrameworkVersion").FirstOrDefault();
            switch (framework)
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
                case "v4.5":
                case "v4.5.1":
                case "v4.5.2":
                case "v4.6":
                default:
                    frameworkVersion = TargetDotNetFrameworkVersion.Version45;
                    framework = "v4";
                    break;
            }

            DotNetFrameworkArchitecture platformArchitecture;
            string platform = BuildEngine.GetEnvironmentVariable("Platform").FirstOrDefault();
            switch (platform)
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

            Log.LogMessage(
                MessageImportance.Normal,
                "Merge Framework: {0}, {1}",
                framework,
                toolPath);

            commandLine.AppendSwitch("/targetplatform:" + framework + "," + toolPath);

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
                    (object)InputAssemblies.Length,
                    InputAssemblies.Length != 1 ? (object)"ies" : (object)"y",
                    (object)_outputFile);

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
                    // Null is sent when the stream closes, so skip it
                    if (!string.IsNullOrEmpty(args.Data))
                        Log.LogMessage(args.Data);
                };
                proc.ErrorDataReceived += (sender, args) =>
                {
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

        private static string ConvertEmptyToNull(string iti)
        {
            if (!string.IsNullOrEmpty(iti))
                return iti;
            return null;
        }
    }
}