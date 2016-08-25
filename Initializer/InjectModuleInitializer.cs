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
using System.Diagnostics;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Initializer
{
    /// <summary>
    /// Build task for injecting a module initializer.
    /// </summary>
    /// <remarks></remarks>
    [PublicAPI]
    public class InjectModuleInitializer : Task
    {
        /// <summary>
        /// Gets or sets the assembly file.
        /// </summary>
        /// <value>The assembly file.</value>
        /// <remarks></remarks>
        [Required]
        public string AssemblyFile { get; set; }

        /// <summary>
        /// Gets or sets the module initializer's type name (Defaults to 'ModuleIntiliazer').
        /// </summary>
        /// <value>The module initializer's type name.</value>
        /// <remarks></remarks>
        public string TypeName { get; set; }

        /// <summary>
        /// Gets or sets the module initializer's method name (Defaults to 'Initialize').
        /// </summary>
        /// <value>The module initializer's method name.</value>
        /// <remarks></remarks>
        public string MethodName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the instrumentation should be done in the current <see cref="AppDomain"/>.
        /// </summary>
        /// <value><c>true</c> if a new <see cref="AppDomain"/> should be used; otherwise, <c>false</c> to perform the injection in the current <see cref="AppDomain"/>.</value>
        /// <remarks></remarks>
        public bool UseIsolatedAppDomain { get; set; }

        /// <summary>
        /// Gets or sets the strong name key pair (if any).
        /// </summary>
        /// <value>The strong name key pair.</value>
        /// <remarks></remarks>
        public string StrongNameKeyPair { get; set; }

        /// <summary>
        /// Gets or sets the directories that are searched for assemblies referenced by the assemly <see cref="AssemblyFile" />.
        /// The directory containing said assembly is included by default.
        /// </summary>
        /// <value>
        /// The assembly search dirs.
        /// </value>
        public virtual ITaskItem[] AssemblySearchDirs { get; set; }

        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        /// <returns>true if the task successfully executed; otherwise, false.</returns>
        /// <remarks></remarks>
        public override bool Execute()
        {
            Debug.Assert(Log != null);

            string[] searchDirs = AssemblySearchDirs == null
                ? new string[0]
                : AssemblySearchDirs.Select(ti => ti.ItemSpec).ToArray();

            foreach (Output output in Injector.Inject(
                    AssemblyFile,
                    TypeName,
                    MethodName,
                    StrongNameKeyPair,
                    UseIsolatedAppDomain,
                    searchDirs))
                switch (output.Importance)
                {
                    case OutputImportance.Error:
                        Log.LogError(output.Message);
                        break;
                    case OutputImportance.Warning:
                        Log.LogWarning(output.Message);
                        break;
                    case OutputImportance.MessageHigh:
                        Log.LogMessage(MessageImportance.High, output.Message);
                        break;
                    case OutputImportance.MessageNormal:
                        Log.LogMessage(MessageImportance.Normal, output.Message);
                        break;
                    case OutputImportance.MessageLow:
                        Log.LogMessage(MessageImportance.Low, output.Message);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            return true;
        }
    }
}