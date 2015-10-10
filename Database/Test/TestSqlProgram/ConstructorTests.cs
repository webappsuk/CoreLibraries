using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Logging;

namespace WebApplications.Utilities.Database.Test.TestSqlProgram
{
    [TestClass]
    public partial class SqlProgramTests : SqlProgramTestBase
    {
        [TestMethod]
        public async Task Constructor_WithDefaults_SetsCommandTimeoutToThirtySeconds()
        {
            SqlProgram program = await SqlProgram.Create((Connection) LocalDatabaseConnectionString, "spNonQuery");
            Assert.IsNotNull(program);
            Assert.AreEqual(TimeSpan.FromSeconds(30), program.DefaultCommandTimeout);
        }

        [TestMethod]
        public async Task Constructor_WithNegativeTimeOutValue_SetsCommandTimeoutToThirtySeconds()
        {
            SqlProgram program = await SqlProgram.Create((Connection)LocalDatabaseConnectionString,
                                        name: "spNonQuery",
                                        defaultCommandTimeout: TimeSpan.FromSeconds(-1));

            Assert.IsNotNull(program);
            Assert.AreEqual(TimeSpan.FromSeconds(30), program.DefaultCommandTimeout);
        }

        [TestMethod]
        public async Task Constructor_WithDefaultCommmandTimeoutParameter_SetsCommandTimeoutToParameterValue()
        {
            const int timeoutSeconds = 40;
            SqlProgram program = await SqlProgram.Create((Connection)LocalDatabaseConnectionString,
                                        name: "spNonQuery",
                                        defaultCommandTimeout: TimeSpan.FromSeconds(timeoutSeconds));

            Assert.IsNotNull(program);
            Assert.AreEqual(TimeSpan.FromSeconds(timeoutSeconds), program.DefaultCommandTimeout);
        }

        [TestMethod]
        [ExpectedException(typeof(LoggingException))]
        public async Task Constructor_WithInvalidProgramName_ThrowsLoggingException()
        {
            SqlProgram<int> timeoutTest = await SqlProgram<int>.Create(
                (Connection)DifferentLocalDatabaseConnectionString,
                "invalidProgramName");
            Assert.IsNotNull(timeoutTest);
        }
    }
}
