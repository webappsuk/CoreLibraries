#region © Copyright Web Applications (UK) Ltd, 2014.  All rights reserved.
// Copyright (c) 2014, Web Applications UK Ltd
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
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace WebApplications.Utilities.Performance.Tools.PerfSetup
{
    /// <summary>
    /// Build task entry point.
    /// </summary>
    public class PerfSetupTask : Task
    {
        /// <summary>
        /// Gets or sets the mode (Defaults to add)
        /// </summary>
        /// <value>The assembly file.</value>
        /// <remarks></remarks>
        public string Mode { get; set; }

        /// <summary>
        /// Gets or sets the execution path.
        /// </summary>
        /// <value>The path.</value>
        [Required]
        public string Path { get; set; }

        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        /// <returns>true if the task successfully executed; otherwise, false.</returns>
        public override bool Execute()
        {
            Logger.AddLogger(
                (s, l) =>
                {
                    s = "PerfSetup: " + s;
                    switch (l)
                    {
                        case Level.Low:
                            Log.LogMessage(MessageImportance.Low, s);
                            break;
                        case Level.Normal:
                            Log.LogMessage(MessageImportance.Normal, s);
                            break;
                        case Level.High:
                            Log.LogMessage(MessageImportance.High, s);
                            break;
                        case Level.Warning:
                            Log.LogWarning(s);
                            break;
                        case Level.Error:
                            Log.LogError(s);
                            break;
                    }
                });

            if (string.IsNullOrWhiteSpace(Path))
                Logger.Add(Level.Warning, "Not path supplied to PerfSetup");
            try
            {
                Scan.Execute(ScanMode.Add, Path, ".", true);
                return true;
            }
            catch (Exception e)
            {
                Logger.Add(Level.Error, e.Message);
                return false;
            }
        }
    }
}