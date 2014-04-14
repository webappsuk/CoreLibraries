using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebApplications.Utilities.Logging.Test
{
    [TestClass]
    public class ContractExceptionTest
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Log.Flush().Wait();
        }

        [TestMethod]
        public void TestRequiresException()
        {
            bool thrown = false;

            try
            {
                TestMethod(null);
            }
            catch (ContractException<Resources> e)
            {
                Assert.AreEqual("Precondition failed: str != null  TestContractFailed", e.Condition);
                Assert.AreEqual(string.Format(Resources.TestContractFailed, e.Condition), e.Message);
                thrown = true;
            }

            Assert.IsTrue(thrown, "Exception was not thrown");
        }

        private void TestMethod(string str)
        {
            Contract.Requires<ContractException<Resources>>(str != null, "TestContractFailed");
        }

        [TestCleanup]
        public void Cleanup()
        {
            Log.Flush().Wait();
        }
    }
}
