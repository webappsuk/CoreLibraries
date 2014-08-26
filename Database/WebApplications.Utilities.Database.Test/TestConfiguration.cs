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
        public async Task TestLbc()
        {
            DatabasesConfiguration configuration = DatabasesConfiguration.Active;
            Assert.IsNotNull(configuration);

            // Get a program that is not mapped by the configuration
            SqlProgram unmappedProgram = await configuration.GetSqlProgram("test2", "spReturnsScalar");

            // Check name was not mapped
            Assert.AreEqual("spReturnsScalar", unmappedProgram.Name);

            // Check the sproc runs
            Assert.AreEqual("HelloWorld", unmappedProgram.ExecuteScalar<string>());

            // Get a program which does not use the default lbc.
            SqlProgram diffConnection = await configuration.GetSqlProgram("test", "spReturnsScalarString");

            // Check name was not mapped
            Assert.AreEqual("spReturnsScalarString", diffConnection.Name);

            // Check the sproc runs
            Assert.AreEqual("HelloWorld", diffConnection.ExecuteScalar<string>());


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
