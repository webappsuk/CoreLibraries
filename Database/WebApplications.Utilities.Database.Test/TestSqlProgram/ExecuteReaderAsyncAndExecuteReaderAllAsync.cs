using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebApplications.Utilities.Database.Test.TestSqlProgram
{
    public partial class SqlProgramTests
    {
        [TestMethod]
        public void ExecuteReaderAsync_ExecutesAndReturnsExpectedResult()
        {
            string connectionString = CreateConnectionString("DifferentLocalData", true);
            SqlProgram readerTest =
                new SqlProgram(connectionString, "spUltimateSproc");

            Task readerTask = readerTest.ExecuteReaderAsync();
            Assert.IsNotNull(readerTask);
        }

        [TestMethod]
        public void ExecuteReaderAllAsync_ExecutesAndReturnsExpectedResult()
        {
            string localDataConnString = CreateConnectionString("LocalData", true);
            string localDataCopyConnString = CreateConnectionString("LocalDataCopy", true);

            SqlProgram program = new SqlProgram(
                new LoadBalancedConnection(localDataConnString, localDataCopyConnString), "spReturnsTable");

            Task readerTask = program.ExecuteReaderAllAsync();
            Assert.IsNotNull(readerTask);
        }

        [TestMethod]
        public void ExecuteReaderAsync_WithReturnType_ExecutesAndReturnsExpectedResult()
        {
            string connectionString = CreateConnectionString("DifferentLocalData", true);
            SqlProgram readerTest =
                new SqlProgram(connectionString, "spUltimateSproc");

            Task<dynamic> result = readerTest.ExecuteReaderAsync(
                reader =>
                    {
                        if (reader.Read())
                        {
                            return CreateDatabaseResult(reader);
                        }

                        throw new Exception("Critical Test Error");
                    });

            Assert.IsNotNull(result);
            // Read the sproc defaults.
            Assert.AreEqual("A Test String", result.Result.Name);
            Assert.AreEqual(5, result.Result.Age);
            Assert.AreEqual(200.15M, result.Result.Balance);
            Assert.AreEqual(false, result.Result.IsValued);
        }

        [TestMethod]
        public void ExecuteReaderAsync_WithAllParametersSet_ExecutesAndReturnsExpectedResult()
        {
            string connectionString = CreateConnectionString("DifferentLocalData", true);
            SqlProgram<string, int, decimal, bool> readerTest =
                new SqlProgram<string, int, decimal, bool>(connectionString, "spUltimateSproc");

            Task<dynamic> result = readerTest.ExecuteReaderAsync(
                c =>
                    {
                        c.SetParameter("@stringParam", AString);
                        c.SetParameter("@intParam", AInt);
                        c.SetParameter("@decimalParam", ADecimal);
                        c.SetParameter("@boolParam", ABool);
                    },
                reader =>
                    {
                        if (reader.Read())
                        {
                            return CreateDatabaseResult(reader);
                        }

                        throw new Exception("Critical Test Error");
                    });

            Assert.IsNotNull(result);
            result.Wait();

            Assert.AreEqual(AString, result.Result.Name);
            Assert.AreEqual(AInt, result.Result.Age);
            Assert.AreEqual(ADecimal, result.Result.Balance);
            Assert.AreEqual(ABool, result.Result.IsValued);
        }

        [TestMethod]
        public void ExecuteReaderAllAsync_WithAllParametersSet_ExecutesAndReturnsExpectedResult()
        {
            string localDataConnString = CreateConnectionString("LocalData", true);
            string localDataCopyConnString = CreateConnectionString("LocalDataCopy", true);

            SqlProgram<string, int, decimal, bool> readerTest = new SqlProgram<string, int, decimal, bool>(
                new LoadBalancedConnection(localDataConnString, localDataCopyConnString), "spReturnsTable");

            Task<IEnumerable<dynamic>> result = readerTest.ExecuteReaderAllAsync(
                c =>
                {
                    c.SetParameter("@stringParam", AString);
                    c.SetParameter("@intParam", AInt);
                    c.SetParameter("@decimalParam", ADecimal);
                    c.SetParameter("@boolParam", ABool);
                },
                reader =>
                {
                    if (reader.Read())
                    {
                        return CreateDatabaseResult(reader);
                    }

                    throw new Exception("Critical Test Error");
                });

            Assert.IsNotNull(result);
            result.Wait();

            foreach (dynamic o in result.Result)
            {
                Assert.AreEqual(AString, o.Name);
                Assert.AreEqual(AInt, o.Age);
                Assert.AreEqual(ADecimal, o.Balance);
                Assert.AreEqual(ABool, o.IsValued);
            }
        }

        private dynamic CreateDatabaseResult(SqlDataReader reader)
        {
            return new
            {
                Name = reader.GetValue<string>(0),
                Age = reader.GetValue<int>(1),
                Balance = reader.GetValue<decimal>(2),
                IsValued = reader.GetValue<bool>(3)
            };
        }
    }
}
