using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;
using WebApplications.Utilities.Database.Exceptions;

namespace WebApplications.Utilities.Database.Test.TestSqlProgram
{
    public partial class SqlProgramTests
    {
        [TestMethod]
        public void ExecuteScalar_WithNoParameters_ExecuteReturnsExpectedString()
        {
            SqlProgram program = Create(connectionString: _differentConnectionString, name: "spReturnsScalar");

            string scalarResult = program.ExecuteScalar<string>();
            Assert.AreEqual("HelloWorld", scalarResult);
        }

        [TestMethod]
        public void ExecuteScalarAll_WithNoParameters_ExecuteReturnsExpectedString()
        {
            SqlProgram program = SqlProgram.Create(new LoadBalancedConnection(_localConnectionString, _localCopyConnectionString), "spReturnsScalarString");
            IList<string> scalarResult = program.ExecuteScalarAll<string>().ToList();
            Assert.AreEqual(2, scalarResult.Count);

            foreach (string result in scalarResult)
                Assert.AreEqual("HelloWorld", result);
        }

        [TestMethod]
        [ExpectedException(typeof(SqlProgramExecutionException))]
        public void ExecuteScalarAll_WithUnknownProgramForConnection_ThrowsSqlProgramExecutionException()
        {
            SqlProgram program = SqlProgram.Create(new LoadBalancedConnection(_localConnectionString, _differentConnectionString), "spReturnsScalar");
            program.ExecuteScalarAll<string>();
        }

        [TestMethod]
        public void ExecuteScalar_WithParameters_ReturnedExpectedString()
        {
            SqlProgram<string, int, decimal, bool> program =
                new SqlProgram<string, int, decimal, bool>(connectionString: _differentConnectionString,
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
        public void ExecuteScalarAll_WithParameters_ReturnedExpectedString()
        {
            SqlProgram<string, int, decimal, bool> program =
                new SqlProgram<string, int, decimal, bool>(
                    connection: new LoadBalancedConnection(_localConnectionString, _localCopyConnectionString),
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
