using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApplications.Utilities.Database.Configuration;

namespace WebApplications.Utilities.Database.Test
{
    [TestClass]
    public class TestPerformance : DatabaseTestBase
    {
        private const string ReturnsScalarProcedureName = "spReturnsScalar";
        private const string ReturnsScalarProcedureResult = "HelloWorld";
        private const string ReturnsScalarWithParamsProcedureName = "spParamsInScalarOut";
        private const string ReturnsTableProcedureName = "spReturnsTable";

        private const int Loops = 1000;
        private const int TestLoops = 100;

        private static double StartClock()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            return Stopwatch.GetTimestamp();
        }

        private static double TicksToMs(double ticks)
        {
            return (double)ticks / Stopwatch.Frequency;
        }

        [TestMethod]
        public async Task TestDatabasesFromConfigAreLoaded()
        {
            var schema = await DatabasesConfiguration.Active.GetSchema("test");

            Assert.IsNotNull(schema);
            Trace.WriteLine(string.Format("{0}: {1}", schema.Guid, schema.ConnectionString));
        }

        [TestMethod]
        public async Task TestScalarStringReturnsExpectedValue()
        {
            SqlProgram program =
                await
                    DatabasesConfiguration.Active.GetSqlProgram(
                        "test",
                        ReturnsScalarProcedureName,
                        null,
                        true,
                        false);

            Assert.IsNotNull(program, "Program is null");

            string helloWorld = await program.ExecuteScalarAsync<string>();

            Assert.IsTrue(
                String.Equals(helloWorld, ReturnsScalarProcedureResult),
                String.Format(
                    "The values {0} and {1} are not equal",
                    helloWorld,
                    ReturnsScalarProcedureResult));
        }

        [TestMethod]
        public async Task TestSqlProgram_ExecuteReaderAsync()
        {
            int rowCount = 0;

            // ReSharper disable once PossibleNullReferenceException
            await Task.WhenAll(
                Enumerable.Range(0, Loops)
                    .Select(
                        async _ =>
                        {
                            SqlProgram<string, int, decimal, bool> program =
                                await
                                    DatabasesConfiguration.Active.GetSqlProgram<string, int, decimal, bool>(
                                        "test",
                                        ReturnsTableProcedureName,
                                        "@stringParam",
                                        "@intParam",
                                        "@decimalParam",
                                        "@boolParam",
                                        true);

                            Assert.IsNotNull(program);

                            await program.ExecuteReaderAsync(
                                async (dataReader, cancellationToken) =>
                                {
                                    Assert.IsNotNull(dataReader);
                                    Assert.IsNotNull(cancellationToken);

                                    while (await dataReader.ReadAsync(cancellationToken))
                                    {
                                        // Operation modes
                                        Interlocked.Increment(ref rowCount);
                                        Assert.AreEqual("system", dataReader.GetString(0), "Strings are not equal");
                                        Assert.AreEqual(-1, dataReader.GetInt32(1), "Ints are not equal");
                                        Assert.AreEqual(100M, dataReader.GetDecimal(2), "decimals are not equal");
                                        Assert.AreEqual(true, dataReader.GetBoolean(3), "bools are not equal");
                                    }
                                    // ReSharper disable once PossibleNullReferenceException
                                    await dataReader.NextResultAsync(cancellationToken);

                                    // ReSharper disable once PossibleNullReferenceException
                                    while (await dataReader.ReadAsync(cancellationToken))
                                    {
                                        // Guids
                                        Interlocked.Increment(ref rowCount);
                                    }
                                },
                                "system",
                                -1,
                                100M,
                                true);

                        }));
            Assert.AreEqual(Loops, rowCount, "Loop count and row count should be equal");
        }

        [TestMethod]
        public async Task TestSqlNormal_ExecuteReaderAsync()
        {
            int rowCount = 0;

            await Task.WhenAll(
                Enumerable.Range(0, Loops)
                    .Select(
                        async _ =>
                        {
                            using (SqlConnection sqlConnection = new SqlConnection(LocalDatabaseConnectionString))
                            {
                                // Open the connection
                                await sqlConnection.OpenAsync();

                                using (SqlCommand sqlCommand = new SqlCommand(ReturnsTableProcedureName, sqlConnection)
                                {
                                    CommandType = CommandType.StoredProcedure
                                })
                                {
                                    // To be absolutely fair, this needs improving to accurately set the SqlParameters (not all the sizes are correct)
                                    sqlCommand.Parameters.AddWithValue("@stringParam", "system");
                                    sqlCommand.Parameters.AddWithValue("@intParam", -1);
                                    sqlCommand.Parameters.AddWithValue("@decimalParam", 100M);
                                    sqlCommand.Parameters.AddWithValue("@boolParam", true);

                                    // Execute command
                                    using (SqlDataReader dataReader = await sqlCommand.ExecuteReaderAsync())
                                    {
                                        Assert.IsNotNull(dataReader);

                                        while (await dataReader.ReadAsync())
                                        {
                                            // Operation modes
                                            Interlocked.Increment(ref rowCount);
                                            Assert.AreEqual("system", dataReader.GetString(0), "Strings are not equal");
                                            Assert.AreEqual(-1, dataReader.GetInt32(1), "Ints are not equal");
                                            Assert.AreEqual(100M, dataReader.GetDecimal(2), "decimals are not equal");
                                            Assert.AreEqual(true, dataReader.GetBoolean(3), "bools are not equal");
                                        }
                                        await dataReader.NextResultAsync();

                                        while (await dataReader.ReadAsync())
                                        {
                                            // Guids
                                            Interlocked.Increment(ref rowCount);
                                        }
                                    }
                                }
                            }
                        }));

            Assert.AreEqual(Loops, rowCount, "Loop count and row count should be equal");

        }


        [TestMethod]
        public async Task TestSqlNormal_ExecuteReader()
        {
            int rowCount = 0;

#pragma warning disable 1998
            await Task.WhenAll(
                Enumerable.Range(0, Loops)
                    .Select(
                        async _ =>
                        {
                            using (SqlConnection sqlConnection = new SqlConnection(LocalDatabaseConnectionString))
                            {
                                // Open the connection
                                sqlConnection.Open();

                                using (SqlCommand sqlCommand = new SqlCommand(ReturnsTableProcedureName, sqlConnection)
                                {
                                    CommandType = CommandType.StoredProcedure
                                })
                                {
                                    // To be absolutely fair, this needs improving to accurately set the SqlParameters (not all the sizes are correct)
                                    sqlCommand.Parameters.AddWithValue("@stringParam", "system");
                                    sqlCommand.Parameters.AddWithValue("@intParam", -1);
                                    sqlCommand.Parameters.AddWithValue("@decimalParam", 100M);
                                    sqlCommand.Parameters.AddWithValue("@boolParam", true);

                                    // Execute command
                                    using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                                    {
                                        Assert.IsNotNull(dataReader);

                                        while (dataReader.Read())
                                        {
                                            // Operation modes
                                            Interlocked.Increment(ref rowCount);
                                            Assert.AreEqual("system", dataReader.GetString(0), "Strings are not equal");
                                            Assert.AreEqual(-1, dataReader.GetInt32(1), "Ints are not equal");
                                            Assert.AreEqual(100M, dataReader.GetDecimal(2), "decimals are not equal");
                                            Assert.AreEqual(true, dataReader.GetBoolean(3), "bools are not equal");
                                        }
                                        dataReader.NextResult();

                                        while (dataReader.Read())
                                        {
                                            // Guids
                                            Interlocked.Increment(ref rowCount);
                                        }
                                    }
                                }
                            }
                        }));

#pragma warning restore 1998

            Assert.AreEqual(Loops, rowCount, "Loop count and row count should be equal");

        }

        [TestMethod]
        public async Task TestSqlProgram_ExecuteReader()
        {
            int rowCount = 0;

            // ReSharper disable once PossibleNullReferenceException
            await Task.WhenAll(
                Enumerable.Range(0, Loops)
                    .Select(
                        async _ =>
                        {
                            SqlProgram<string, int, decimal, bool> program =
                                await
                                    DatabasesConfiguration.Active.GetSqlProgram<string, int, decimal, bool>(
                                        "test",
                                        ReturnsTableProcedureName,
                                        "@stringParam",
                                        "@intParam",
                                        "@decimalParam",
                                        "@boolParam",
                                        true);

                            Assert.IsNotNull(program);

                            program.ExecuteReader(
                                (dataReader) =>
                                {
                                    Assert.IsNotNull(dataReader);

                                    while (dataReader.Read())
                                    {
                                        // Operation modes
                                        Interlocked.Increment(ref rowCount);
                                        Assert.AreEqual("system", dataReader.GetString(0), "Strings are not equal");
                                        Assert.AreEqual(-1, dataReader.GetInt32(1), "Ints are not equal");
                                        Assert.AreEqual(100M, dataReader.GetDecimal(2), "decimals are not equal");
                                        Assert.AreEqual(true, dataReader.GetBoolean(3), "bools are not equal");
                                    }
                                    dataReader.NextResult();

                                    while (dataReader.Read())
                                    {
                                        // Guids
                                        Interlocked.Increment(ref rowCount);
                                    }
                                },
                                "system",
                                -1,
                                100M,
                                true);

                        }));
            Assert.AreEqual(Loops, rowCount, "Loop count and row count should be equal");

        }

        [TestMethod]
        public async Task TestSqlProgram_ScalarWithParameters()
        {
            const string stringParam = "System";
            const int intParam = -1;
            const decimal decimalParam = 100M;
            const bool boolParam = true;

            string expectedResult = String.Format(
                "{0} - {1} - {2:0.00} - {3}",
                stringParam,
                intParam,
                decimalParam,
                Convert.ToByte(boolParam));

            SqlProgram<string, int, decimal, bool> program =
                await DatabasesConfiguration.Active.GetSqlProgram<string, int, decimal, bool>(
                    "test",
                    ReturnsScalarWithParamsProcedureName,
                    "@stringParam",
                    "@intParam",
                    "@decimalParam",
                    "@boolParam");

            Assert.IsNotNull(program);

            string actualResult =
                await program.ExecuteScalarAsync<string>(stringParam, intParam, decimalParam, boolParam);

            Assert.AreEqual(
                expectedResult,
                actualResult,
                String.Format(
                    "The string '{0}' (expected) is not equal to '{1}' (actual)",
                    expectedResult,
                    actualResult));
        }

        [TestMethod]
        [Ignore]
        public async Task TestAll()
        {

            double start, end, testProgram = 0D, testProgramAsync = 0D,
                testSql = 0D, testSqlAsync = 0D;

            await TestDatabasesFromConfigAreLoaded();

            for (int a = -2; a < TestLoops; ++a)
            {

                start = StartClock();
                await TestSqlProgram_ExecuteReader();
                end = Stopwatch.GetTimestamp();
                if (a >= 0)
                    testProgram += end - start;

                start = StartClock();
                await TestSqlProgram_ExecuteReaderAsync();
                end = Stopwatch.GetTimestamp();
                if (a >= 0)
                    testProgramAsync += end - start;

                start = StartClock();
                await TestSqlNormal_ExecuteReader();
                end = Stopwatch.GetTimestamp();
                if (a >= 0)
                    testSql += end - start;

                start = StartClock();
                await TestSqlNormal_ExecuteReaderAsync();
                end = Stopwatch.GetTimestamp();
                if (a >= 0)
                    testSqlAsync += end - start;
            }

            Trace.WriteLine(
                String.Format(
                    "TestSqlProgram_ExecuteReader: Average time taken over {0} loops is {1}ms, {2}ms per loop",
                    TestLoops,
                    TicksToMs(testProgram / TestLoops),
                    TicksToMs(testProgram / TestLoops) / Loops));

            Trace.WriteLine(
                String.Format(
                    "TestSqlProgram_ExecuteReaderAsync: Average time taken over {0} loops is {1}ms, {2}ms per loop",
                    TestLoops,
                    TicksToMs(testProgramAsync / TestLoops),
                    TicksToMs(testProgramAsync / TestLoops) / Loops));

            Trace.WriteLine(
                 String.Format(
                    "TestSqlNormal_ExecuteReader: Average time taken over {0} loops is {1}ms, {2}ms per loop",
                    TestLoops,
                    TicksToMs(testSql / TestLoops),
                    TicksToMs(testSql / TestLoops) / Loops));

            Trace.WriteLine(
                String.Format(
                    "TestSqlNormal_ExecuteReaderAsync: Average time taken over {0} loops is {1}ms, {2}ms per loop",
                    TestLoops,
                    TicksToMs(testSqlAsync / TestLoops),
                    TicksToMs(testSqlAsync / TestLoops) / Loops));

        }
    }
}
