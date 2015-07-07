using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;
using WebApplications.Utilities.Database.Exceptions;

namespace WebApplications.Utilities.Database.Test.TestSqlProgram
{
    public partial class SqlProgramTests
    {
        [TestMethod]
        public async Task ExecuteScalar_WithNoParameters_ExecuteReturnsExpectedString()
        {
            SqlProgram program = await SqlProgram.Create((Connection)DifferentLocalDatabaseConnectionString, name: "spReturnsScalar");

            string scalarResult = program.ExecuteScalar<string>();
            Assert.AreEqual("HelloWorld", scalarResult);
        }

        [TestMethod]
        public async Task ExecuteScalarAll_WithNoParameters_ExecuteReturnsExpectedString()
        {
            SqlProgram program = await SqlProgram.Create(new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString), "spReturnsScalarString");
            IList<string> scalarResult = program.ExecuteScalarAll<string>().ToList();
            Assert.AreEqual(2, scalarResult.Count);

            foreach (string result in scalarResult)
                Assert.AreEqual("HelloWorld", result);
        }

        [TestMethod]
        public async Task ExecuteScalar_WithParameters_ReturnedExpectedString()
        {
            SqlProgram<string, int, decimal, bool> program =
                await SqlProgram<string, int, decimal, bool>.Create((Connection) DifferentLocalDatabaseConnectionString,
                                                           name: "spWithParametersReturnsScalarString");

            string randomString = Random.RandomString(20, false);
            string scalarResult = program.ExecuteScalar<string>(
                c =>
                {
                    c.SetParameter("@stringParam", randomString);
                    c.SetParameter("@intParam", AInt);
                    c.SetParameter("@decimalParam", ADecimal);
                    c.SetParameter("@boolParam", ABool);
                });

            Assert.AreEqual(string.Format("{0} - {1} - {2} - 1", randomString, AInt, ADecimal), scalarResult);
        }

        [TestMethod]
        public async Task ExecuteScalarAll_WithParameters_ReturnedExpectedString()
        {
            SqlProgram<string, int, decimal, bool> program =
                await SqlProgram<string, int, decimal, bool>.Create(
                    connection: new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString),
                    name: "spWithParametersReturnsScalar");

            string randomString = Random.RandomString(20, false);
            IList<string> scalarResult = program.ExecuteScalarAll<string>(
                c =>
                    {
                        c.SetParameter("@stringParam", randomString);
                        c.SetParameter("@intParam", AInt);
                        c.SetParameter("@decimalParam", ADecimal);
                        c.SetParameter("@boolParam", ABool);
                    }).ToList();

            Assert.AreEqual(2, scalarResult.Count);
            foreach (string result in scalarResult)
                Assert.AreEqual(string.Format("{0} - {1} - {2} - 1", randomString.Substring(0, randomString.Length), AInt, ADecimal),
                                result);
        }
    }
}
