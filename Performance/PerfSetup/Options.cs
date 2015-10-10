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

using CmdLine;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Performance.Tools.PerfSetup
{
    /// <summary>
    /// The applications command line options.
    /// </summary>
    [CommandLineArguments(Program = "PerfSetup", Title = "Web Applications Performance Setup",
        Description = "Provides utilities for interacting with Web Applications Performance Counters.")]
    internal class Options
    {
        [CommandLineParameter(Command = "?", Default = false, Description = "Show Help", Name = "Help", IsHelp = true)]
        public bool Help
        {
            get;
            [UsedImplicitly]
            set;
        }

        [CommandLineParameter(Command = "E", Default = true, Name = "ExecuteAgain",
            Description =
                "When executing on a 64 bit architecture, will execute again in either 32/64 bit mode to ensure that performance counters are added in both modes."
            )]
        public bool ExecuteAgain
        {
            get;
            [UsedImplicitly]
            set;
        }

        [CommandLineParameter(Name = "Mode", ParameterIndex = 1, Required = true, Description =
            @"Set the mode to one of the following:
Add - Add performance counters (requires run as administrator).
Delete - Delete performance counters (requires run as administrator).
List - Lists performance counters.")]
        public string Mode
        {
            get;
            [UsedImplicitly]
            set;
        }

        [CommandLineParameter(Name = "path", ParameterIndex = 2, Required = true,
            Description = "Specifies the path or file to scan.")]
        public string Path
        {
            get;
            [UsedImplicitly]
            set;
        }
    }
}