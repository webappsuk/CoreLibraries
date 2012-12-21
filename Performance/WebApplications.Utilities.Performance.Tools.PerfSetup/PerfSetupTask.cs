using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            if (string.IsNullOrWhiteSpace(Path))
                Log.LogWarning("Not path supplied to PerfSetup");
            try
            {
                Scan.Execute(ScanMode.Add, Path, (s, l) =>
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
                return true;
            }
            catch (Exception e)
            {
                Log.LogError(e.Message);
                return false;
            }
        }
    }
}
