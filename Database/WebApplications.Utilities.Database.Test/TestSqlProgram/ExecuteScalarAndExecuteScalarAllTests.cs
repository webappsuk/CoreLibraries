using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Database.Exceptions;

namespace WebApplications.Utilities.Database.Test.TestSqlProgram
{
    public partial class SqlProgramTests
    {
        [TestMethod]
        public void ExecuteScalar_WithNoParameters_ExecuteReturnsExpectedString()
        {
            string connectionString = CreateConnectionString("DifferentLocalData");
            SqlProgram program = new SqlProgram(connectionString: connectionString, name: "spReturnsScalar");

            string scalarResult = program.ExecuteScalar<string>();
            Assert.AreEqual("HelloWorld", scalarResult);
        }

        [TestMethod]
        public void ExecuteScalarAll_WithNoParameters_ExecuteReturnsExpectedString()
        {
            string localDataConnString = CreateConnectionString("LocalData");
            string localDataCopyConnString = CreateConnectionString("LocalDataCopy");

            SqlProgram program = new SqlProgram(new LoadBalancedConnection(localDataConnString, localDataCopyConnString), "spReturnsScalarString");
            IList<string> scalarResult = program.ExecuteScalarAll<string>().ToList();
            Assert.AreEqual(2, scalarResult.Count);

            foreach (string result in scalarResult)
                Assert.AreEqual("HelloWorld", result);
        }

        [TestMethod]
        [ExpectedException(typeof(SqlProgramExecutionException))]
        public void ExecuteScalarAll_WithUnknownProgramForConnection_ThrowsSqlProgramExecutionException()
        {
            string localDataConnString = CreateConnectionString("LocalData");
            string localDataCopyConnString = CreateConnectionString("DifferentLocalData");

            SqlProgram program = new SqlProgram(new LoadBalancedConnection(localDataConnString, localDataCopyConnString), "spReturnsScalar");
            program.ExecuteScalarAll<string>();
        }

        [TestMethod]
        public void ExecuteScalar_WithParameters_ReturnedExpectedString()
        {
            string connectionString = CreateConnectionString("DifferentLocalData");
            SqlProgram<string, int, decimal, bool> program =
                new SqlProgram<string, int, decimal, bool>(connectionString: connectionString,
                                                           name: "spWithParametersReturnsScalarString");

            string randomString = GenerateRandomString(20, false);
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
            string localDataConnString = CreateConnectionString("LocalData");
            string localDataCopyConnString = CreateConnectionString("LocalDataCopy");

            SqlProgram<string, int, decimal, bool> program =
                new SqlProgram<string, int, decimal, bool>(
                    connection: new LoadBalancedConnection(localDataConnString, localDataCopyConnString),
                    name: "spWithParametersReturnsScalar");

            string randomString = GenerateRandomString(20, false);
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
                Assert.AreEqual(string.Format("{0} - {1} - {2} - 1", randomString.Substring(0, 20), AInt, ADecimal),
                                result);
        }
    }
}
