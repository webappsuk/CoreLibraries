using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace WebApplications.Utilities.Logging.Tools.PerfSetup
{
    public class PerfSetupTask : Task
    {
        /// <summary>
        /// Gets or sets the mode (Defaults to add)
        /// </summary>
        /// <value>The assembly file.</value>
        /// <remarks></remarks>
        public string Mode { get; set; }

        [Required]
        public string Path { get; set; }

        public override bool Execute()
        {
            try
            {

            }
            catch (Exception e)
            {
                Log.LogError(e.Message);
                return false;
            }
        }
    }
}
