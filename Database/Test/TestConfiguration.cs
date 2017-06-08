using System;
using System.Configuration;
using System.Data;
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
        public async Task Test_CheckMappedProgram_ReturnsExpectedResult()
        {
            DatabasesConfiguration configuration = DatabasesConfiguration.Active;

            configuration.Databases["test2"].Programs["TestProgram"].MapTo = "spTakesParamAndReturnsScalar";

            // Get a program that is mapped by the configuration
            SqlProgram<string> mappedProgram = await configuration.GetSqlProgram<string>("test2", "TestProgram", "@P1");

            // Check the names were mapped.
            Assert.AreEqual("spTakesParamAndReturnsScalar", mappedProgram.Text);
            Assert.AreEqual(CommandType.StoredProcedure, mappedProgram.Type);
            Assert.AreEqual("@firstName", mappedProgram.Parameters.Single().Name);
            Assert.AreEqual("varchar", mappedProgram.Parameters.Single().SqlType?.Name);
            Assert.AreEqual((short)10, mappedProgram.Parameters.Single().SqlType?.Size?.MaximumLength);

            // Check the sproc runs.
            string name = Guid.NewGuid().ToString().Substring(0, 10);
            string result = mappedProgram.ExecuteScalar<string>(name);
            Assert.AreEqual("Hello " + name, result);
        }

        [TestMethod]
        public async Task Test_CheckMappedText_ReturnsExpectedResult()
        {
            DatabasesConfiguration configuration = DatabasesConfiguration.Active;

            string programText = GetProgramText(
                "spTakesParamAndReturnsScalar",
                DifferentLocalDatabaseConnectionString);

            configuration.Databases["test2"].Programs["TestProgram"].Text = programText;

            // Get a program that is mapped by the configuration
            SqlProgram<string> mappedProgram = await configuration.GetSqlProgram<string>("test2", "TestProgram", "@P1");

            // Check the names and types were mapped.
            Assert.AreEqual(programText.NormaliseLineEndings(), mappedProgram.Text.NormaliseLineEndings());
            Assert.AreEqual(CommandType.Text, mappedProgram.Type);
            Assert.AreEqual("@firstName", mappedProgram.Parameters.Single().Name);
            Assert.AreEqual("varchar", mappedProgram.Parameters.Single().SqlType?.Name);
            Assert.AreEqual((short)10, mappedProgram.Parameters.Single().SqlType?.Size?.MaximumLength);

            Assert.AreEqual("@firstName", mappedProgram.Mappings.Single().Parameters.Single().FullName);
            Assert.AreEqual("varchar", mappedProgram.Mappings.Single().Parameters.Single().Type.Name);
            Assert.AreEqual((short)10, mappedProgram.Mappings.Single().Parameters.Single().Type.Size.MaximumLength);

            // Check the text runs.
            string name = Guid.NewGuid().ToString().Substring(0, 10);
            string result = mappedProgram.ExecuteScalar<string>(name);
            Assert.AreEqual("Hello " + name, result);
        }
    }
}
