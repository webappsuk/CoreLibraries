using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;

namespace WebApplications.Utilities.Logging.Test
{
    [DeploymentItem("TimeZoneData", "TimeZoneData")]
    public abstract class LoggingTestBase : TestBase
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Log.Flush().Wait();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Log.Flush().Wait();
        }
    }
}
