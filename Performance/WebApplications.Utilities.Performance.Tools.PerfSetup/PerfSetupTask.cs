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
            try
            {
                Options o = new Options
                    {
                        Help = false,
                        Mode = Mode ?? "Add",
                        MachineName = ".",
                        Path = Path
                    };

                Program.Execute(o, (s, l) =>
                    {
                        switch (l)
                        {
                            case Level.Message:
                                Log.LogMessage(s);
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
