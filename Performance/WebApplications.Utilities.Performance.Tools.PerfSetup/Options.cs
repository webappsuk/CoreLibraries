using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmdLine;

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
        public bool Help { get; set; }

        [CommandLineParameter(Command = "M", Default = ".", Name = "MachineName", Description = "Specifies the machine to run on, allowing counters to be administered on remote machines (defaults to '.').")]
        public string MachineName { get; set; }

        [CommandLineParameter(Command = "E", Default = true, Name = "ExecuteAgain", Description = "When executing on a 64 bit architecture, will execute again in either 32/64 bit mode to ensure that performance counters are added in both modes.")]
        public bool ExecuteAgain { get; set; }

        [CommandLineParameter(Name = "Mode", ParameterIndex = 1, Required = true, Description =
            @"Set the mode to one of the following:
Add - Add performance counters (requires run as administrator).
Delete - Delete performance counters (requires run as administrator).
List - Lists performance counters.")]
        public string Mode { get; set; }

        [CommandLineParameter(Name = "path", ParameterIndex = 2, Required = true, Description = "Specifies the file or files to scan, you can use wildcards or folders.")]
        public string Path { get; set; }
    }
}
