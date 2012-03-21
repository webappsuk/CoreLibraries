using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Logging;

namespace WebApplications.Utilities.Database.Test.TestSqlProgram
{
    [TestClass]
    public partial class SqlProgramTests : SqlProgramTestBase
    {
        [TestMethod]
        public void Constructor_WithDefaults_SetsCommandTimeoutToThirtySeconds()
        {
            string connectionString = CreateConnectionString("LocalData", false);
            SqlProgram program = new SqlProgram(connectionString: connectionString, name: "MyProgram");
            Assert.IsNotNull(program);
            Assert.AreEqual(TimeSpan.FromSeconds(30), program.DefaultCommandTimeout);
        }

        [TestMethod]
        public void Constructor_WithNegativeTimeOutValue_SetsCommandTimeoutToThirtySeconds()
        {
            string connectionString = CreateConnectionString("LocalData", false);
            SqlProgram program = new SqlProgram(
                connectionString: connectionString,
                name: "MyProgram",
                defaultCommandTimeout: TimeSpan.FromSeconds(-1));

            Assert.IsNotNull(program);
            Assert.AreEqual(TimeSpan.FromSeconds(30), program.DefaultCommandTimeout);
        }

        [TestMethod]
        public void Constructor_WithDefaultCommmandTimeoutParameter_SetsCommandTimeoutToParameterValue()
        {
            const int timeoutSeconds = 40;
            string connectionString = CreateConnectionString("LocalData", false);
            SqlProgram program = new SqlProgram(
                connectionString: connectionString,
                name: "MyProgram",
                defaultCommandTimeout: TimeSpan.FromSeconds(timeoutSeconds));

            Assert.IsNotNull(program);
            Assert.AreEqual(TimeSpan.FromSeconds(timeoutSeconds), program.DefaultCommandTimeout);
        }

        [TestMethod]
        [ExpectedException(typeof(LoggingException))]
        public void Constructor_WithInvalidProgramName_ThrowsLoggingException()
        {
            string connectionString = CreateConnectionString("DifferentLocalData", false);
            SqlProgram<int> timeoutTest =
                new SqlProgram<int>(connectionString, "invalidProgramName",
                                    defaultCommandTimeout: new TimeSpan(0, 1, 0));
            Assert.IsNotNull(timeoutTest);
        }
    }
}
