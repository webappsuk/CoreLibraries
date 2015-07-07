using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Configuration;
using WebApplications.Utilities.Database.Configuration;

namespace WebApplications.Utilities.Database.Test
{
    [TestClass]
    public class TestConfiguration : DatabaseTestBase
    {
        [TestMethod]
        public void Test_ActiveConfiguration_IsNotNull()
        {
            DatabasesConfiguration configuration = DatabasesConfiguration.Active;
            Assert.IsNotNull(configuration);
        }

        [TestMethod]
        public async Task Test_UnmappedProgram_RunsAndReturnsExpectedResult()
        {
            DatabasesConfiguration configuration = DatabasesConfiguration.Active;
            SqlProgram unmappedProgram = await configuration.GetSqlProgram("test2", "spReturnsScalar");

            Assert.AreEqual("spReturnsScalar", unmappedProgram.Name);
            Assert.AreEqual("HelloWorld", await unmappedProgram.ExecuteScalarAsync<string>());
        }

        [TestMethod]
        public async Task Test_UnmappedProgramOnDifferentConnection_RunsAndReturnsExpectedResult()
        {
            DatabasesConfiguration configuration = DatabasesConfiguration.Active;

            // Get a program which does not use the default lbc.
            SqlProgram diffConnection = await configuration.GetSqlProgram("test", "spReturnsScalarString");

            // Check name was not mapped
            Assert.AreEqual("spReturnsScalarString", diffConnection.Name);

            // Check the sproc runs
            Assert.AreEqual("HelloWorld", await diffConnection.ExecuteScalarAsync<string>());
        }

        [TestMethod]
        public async Task Test_CheckMappedProgramReturnsExpectedResult()
        {
            DatabasesConfiguration configuration = DatabasesConfiguration.Active;

            // Get a program that is mapped by the configuration
            SqlProgram<string> mappedProgram = await configuration.GetSqlProgram<string>("test2", "TestProgram", "@P1");

            // Check the names were mapped.
            Assert.AreEqual("spTakesParamAndReturnsScalar", mappedProgram.Name);
            Assert.AreEqual("@firstName", mappedProgram.Parameters.Single().Key);

            // Check the sproc runs.
            string name = Guid.NewGuid().ToString().Substring(0, 10);
            string result = mappedProgram.ExecuteScalar<string>(name);
            Assert.AreEqual("Hello " + name, result);
        }
    }
}
