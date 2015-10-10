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
        public async Task ExecuteReaderAsync_ExecutesAndReturnsExpectedResult()
        {
            SqlProgram readerTest = await SqlProgram.Create((Connection)DifferentLocalDatabaseConnectionString, "spUltimateSproc");
            Task readerTask = readerTest.ExecuteReaderAsync();
            Assert.IsNotNull(readerTask);
        }

        [TestMethod]
        public async Task ExecuteReaderAllAsync_ExecutesAndReturnsExpectedResult()
        {
            SqlProgram program = await SqlProgram.Create(new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString), "spReturnsTable");

            Task readerTask = program.ExecuteReaderAllAsync();
            Assert.IsNotNull(readerTask);
        }

        [TestMethod]
        public async Task ExecuteReaderAsync_WithReturnType_ExecutesAndReturnsExpectedResult()
        {
            SqlProgram readerTest =
                await SqlProgram.Create((Connection)DifferentLocalDatabaseConnectionString, "spUltimateSproc");

            Task<dynamic> result = readerTest.ExecuteReaderAsync(
                async (reader, token) =>
                    {
                        if (await reader.ReadAsync(token))
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
        public async Task ExecuteReaderAsync_WithAllParametersSet_ExecutesAndReturnsExpectedResult()
        {
            SqlProgram<string, int, decimal, bool> readerTest =
                await SqlProgram<string, int, decimal, bool>.Create((Connection)DifferentLocalDatabaseConnectionString, "spUltimateSproc");

            Task<dynamic> result = readerTest.ExecuteReaderAsync(
                c =>
                    {
                        c.SetParameter("@stringParam", AString);
                        c.SetParameter("@intParam", AInt);
                        c.SetParameter("@decimalParam", ADecimal);
                        c.SetParameter("@boolParam", ABool);
                    },
                async (reader, token) =>
                    {
                        if (await reader.ReadAsync(token))
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
        public async Task ExecuteReaderAllAsync_WithAllParametersSet_ExecutesAndReturnsExpectedResult()
        {
            SqlProgram<string, int, decimal, bool> readerTest = await SqlProgram<string, int, decimal, bool>.Create(
                new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString), "spReturnsTable");

            Task<IEnumerable<dynamic>> result = readerTest.ExecuteReaderAllAsync(
                c =>
                {
                    c.SetParameter("@stringParam", AString);
                    c.SetParameter("@intParam", AInt);
                    c.SetParameter("@decimalParam", ADecimal);
                    c.SetParameter("@boolParam", ABool);
                },
                async (reader, token) =>
                {
                    if (await reader.ReadAsync(token))
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
