using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;

namespace WebApplications.Utilities.Logging.Test
{
    [DeploymentItem("Resources\\", "Resources")]
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
