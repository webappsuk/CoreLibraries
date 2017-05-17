#region © Copyright Web Applications (UK) Ltd, 2017.  All rights reserved.
// Copyright (c) 2017, Web Applications UK Ltd
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Web Applications UK Ltd nor the
//       names of its contributors may be used to endorse or promote products
//       derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL WEB APPLICATIONS UK LTD BE LIABLE FOR ANY
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

// ReSharper disable ConsiderUsingConfigureAwait,UseConfigureAwait

using System;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;
using WebApplications.Utilities.Database.Configuration;

namespace WebApplications.Utilities.Database.Test
{
    [TestClass]
    public class TestBatching : DatabaseTestBase
    {
        private static int _count = 0;
        private static double _nonQueryTime;
        private static double _scalarTime;
        private static double _outputTime;
        private static double _tableTime;
        private static double _xmlTime;
        private static double _doneTime;

        private static double _batchedNonQueryTime;
        private static double _batchedScalarTime;
        private static double _batchedOutputTime;
        private static double _batchedTableTime;
        private static double _batchedXmlTime;
        private static double _batchedDoneTime;

        private static SqlProgram<string, int> _nonQueryProg;
        private static SqlProgram<string, int, decimal, bool> _returnsScalarProg;
        private static SqlProgram<int, Out<int>, Out<int>> _outputParametersProg;
        private static SqlProgram<string, int, decimal, bool> _returnsTableProg;
        private static SqlProgram _returnsXmlProg;
        private static SqlProgram<string, int, int> _raiserrorProg;

        private static void AddTime(Stopwatch sw, ref double counter)
        {
            double elapsed = sw.Elapsed.TotalMilliseconds;

            if (_count >= 0)
                counter += elapsed;
        }

        private static void Setup(
            out SqlProgram<string, int> nonQueryProg,
            out SqlProgram<string, int, decimal, bool> returnsScalarProg,
            out SqlProgram<int, Out<int>, Out<int>> outputParametersProg,
            out SqlProgram<string, int, decimal, bool> returnsTableProg,
            out SqlProgram returnsXmlProg,
            out SqlProgram<string, int, int> raiserrorProg,
            out string randomString,
            out int randomInt,
            out decimal randomDecimal,
            out bool randomBool,
            out Out<int> output,
            out Out<int> inputOutput)
        {
            DatabaseElement database = DatabasesConfiguration.Active.Databases["test"];
            Assert.IsNotNull(database);

            nonQueryProg =
                _nonQueryProg ?? (_nonQueryProg = database.GetSqlProgram<string, int>(
                        "spNonQuery",
                        "@stringParam",
                        "@intParam")
                    .Result);

            returnsScalarProg =
                _returnsScalarProg ?? (_returnsScalarProg = database.GetSqlProgram<string, int, decimal, bool>(
                        "spWithParametersReturnsScalar",
                        "@stringParam",
                        "@intParam",
                        "@decimalParam",
                        "@boolParam")
                    .Result);

            outputParametersProg =
                _outputParametersProg ?? (_outputParametersProg = database
                    .GetSqlProgram<int, Out<int>, Out<int>>(
                        "spOutputParameters",
                        "@inputParam",
                        "@inputOutputParam",
                        "@outputParam")
                    .Result);

            returnsTableProg =
                _returnsTableProg ?? (_returnsTableProg = database
                    .GetSqlProgram<string, int, decimal, bool>(
                        "spReturnsTable",
                        "@stringParam",
                        "@intParam",
                        "@decimalParam",
                        "@boolParam")
                    .Result);

            raiserrorProg =
                _raiserrorProg ?? (_raiserrorProg = database.GetSqlProgram<string, int, int>(
                        "spRaiserror",
                        "@message",
                        "@severity",
                        "@state")
                    .Result);

            returnsXmlProg = _returnsXmlProg ?? (_returnsXmlProg = database.GetSqlProgram("spReturnsXml").Result);

            randomString = Random.RandomString(Encoding.ASCII, maxLength: 20);
            randomInt = Random.RandomInt32();
            randomDecimal = Math.Round(Random.RandomDecimal() % 1_000_000_000m, 2);
            randomBool = Random.RandomBoolean();

            output = new Out<int>();
            inputOutput = new Out<int>(Random.RandomInt32() / 2);
        }

        [TestMethod]
        public async Task TestBatchEverything()
        {
            Setup(
                out SqlProgram<string, int> nonQueryProg,
                out SqlProgram<string, int, decimal, bool> returnsScalarProg,
                out SqlProgram<int, Out<int>, Out<int>> outputParametersProg,
                out SqlProgram<string, int, decimal, bool> returnsTableProg,
                out SqlProgram returnsXmlProg,
                out SqlProgram<string, int, int> raiserrorProg,
                out string randomString,
                out int randomInt,
                out decimal randomDecimal,
                out bool randomBool,
                out Out<int> output,
                out Out<int> inputOutput);

            int inputOutputVal = inputOutput.Value;

            SqlBatchResult<string> outputResult = null;
            SqlBatchResult tableResult = null;

            SqlBatch batch = SqlBatch.CreateTransaction(IsolationLevel.ReadUncommitted)
                .AddExecuteNonQuery(
                    nonQueryProg,
                    out SqlBatchResult<int> nonQueryResult,
                    randomString,
                    randomInt)
                .AddExecuteScalar(
                    returnsScalarProg,
                    out SqlBatchResult<string> scalarResult,
                    randomString,
                    randomInt,
                    randomDecimal,
                    randomBool)
                .AddTransactionBatch(
                    b => b
                        .AddExecuteNonQuery(
                            raiserrorProg,
                            out _,
                            "Error!",
                            16,
                            0)
                        .AddExecuteScalar(
                            outputParametersProg,
                            out outputResult,
                            randomInt,
                            inputOutput,
                            output),
                    IsolationLevel.Serializable)
                .AddExecuteReader(
                    returnsTableProg,
                    async (reader, token) =>
                    {
                        Assert.IsTrue(await reader.ReadAsync(token));

                        Assert.AreEqual(randomString, reader.GetValue(0));
                        Assert.AreEqual(output.Value, reader.GetValue(1));
                        Assert.AreEqual(randomDecimal, reader.GetValue(2));
                        Assert.AreEqual(randomBool, reader.GetValue(3));
                    },
                    out tableResult,
                    randomString,
                    // Using output of previous program as input
                    output,
                    randomDecimal,
                    randomBool)
                .AddExecuteXmlReader(
                    returnsXmlProg,
                    (reader, token) =>
                    {
                        string xml = XElement.Load(reader).ToString();
                        Assert.AreEqual("<foo>bar</foo>", xml);
                        return TaskResult.Completed;
                    },
                    out SqlBatchResult xmlResult);

#pragma warning disable 4014
            Task<int> task1 = nonQueryResult.GetResultAsync();
            Task<string> task2 = scalarResult.GetResultAsync();
            Task<string> task3 = outputResult.GetResultAsync();
            Task task4 = tableResult.GetResultAsync();
            Task task5 = xmlResult.GetResultAsync();
#pragma warning restore 4014

            try
            {
                await batch.ExecuteAsync();

                using (CancellationTokenSource cts = new CancellationTokenSource(5000))
                {
                    CancellationToken token = cts.Token;
                    await task1.WithCancellation(token);
                    await task2.WithCancellation(token);
                    await task3.WithCancellation(token);
                    await task4.WithCancellation(token);
                    await task5.WithCancellation(token);
                }
            }
            finally
            {
                Trace.WriteLine("SQL:");
                Trace.WriteLine(GetSql(batch) ?? "<not available>");
            }

            Assert.AreEqual(-1, task1.Result);

            Assert.AreEqual(
                $"{randomString.Substring(0, randomString.Length)} - {randomInt} - {randomDecimal:F2} - {(randomBool ? "1" : "0")}",
                task2.Result);
            
            Assert.AreEqual("<foo>bar</foo>", task3.Result);

            Assert.IsNull(inputOutput.OutputError, inputOutput.OutputError?.Message);
            Assert.IsNull(output.OutputError, output.OutputError?.Message);

            Assert.AreEqual(inputOutputVal * 2, inputOutput.OutputValue.Value);
            Assert.AreEqual(randomInt, output.OutputValue.Value);
        }

        [TestMethod]
        public async Task PerfTest()
        {
            // In a separate method so they can be filtered out of performance traces
            async Task JIT()
            {
                _count = -1;
                await RunBatched();
                await RunNotBatched();
                await RunBatched();
                await RunNotBatched();
            }

            await JIT();

            for (_count = 0; _count < 500; _count++)
            {
                Trace.Write(_count);
                await RunBatched();
                Trace.Write(".");
                await RunNotBatched();
                Trace.Write(".");
            }
            Trace.WriteLine("");
            Trace.WriteLine("");

            Trace.WriteLine($"B spNonQuery in {_batchedNonQueryTime / _count}ms");
            Trace.WriteLine($"B spWithParametersReturnsScalar in {_batchedScalarTime / _count}ms");
            Trace.WriteLine($"B spOutputParameters in {_batchedOutputTime / _count}ms");
            Trace.WriteLine($"B spReturnsTable in {_batchedTableTime / _count}ms");
            Trace.WriteLine($"B spReturnsXml in {_batchedXmlTime / _count}ms");
            Trace.WriteLine($"B Done in {_batchedDoneTime / _count}ms");

            Trace.WriteLine("");

            Trace.WriteLine($"N spNonQuery in {_nonQueryTime / _count}ms");
            Trace.WriteLine($"N spWithParametersReturnsScalar in {_scalarTime / _count}ms");
            Trace.WriteLine($"N spOutputParameters in {_outputTime / _count}ms");
            Trace.WriteLine($"N spReturnsTable in {_tableTime / _count}ms");
            Trace.WriteLine($"N spReturnsXml in {_xmlTime / _count}ms");
            Trace.WriteLine($"N Done in {_doneTime / _count}ms");

            _count = 0;
        }

        [TestMethod]
        private async Task RunBatched()
        {
            Setup(
                out SqlProgram<string, int> nonQueryProg,
                out SqlProgram<string, int, decimal, bool> returnsScalarProg,
                out SqlProgram<int, Out<int>, Out<int>> outputParametersProg,
                out SqlProgram<string, int, decimal, bool> returnsTableProg,
                out SqlProgram returnsXmlProg,
                out _,
                out string randomString,
                out int randomInt,
                out decimal randomDecimal,
                out bool randomBool,
                out Out<int> output,
                out Out<int> inputOutput);

            SqlBatchResult<string> outputResult = null;
            SqlBatchResult tableResult = null;

            SqlBatch batch = SqlBatch.Create()
                .AddExecuteNonQuery(
                    nonQueryProg,
                    out SqlBatchResult<int> nonQueryResult,
                    randomString,
                    randomInt)
                .AddExecuteScalar(
                    returnsScalarProg,
                    out SqlBatchResult<string> scalarResult,
                    randomString,
                    randomInt,
                    randomDecimal,
                    randomBool)
                .AddExecuteScalar(
                    outputParametersProg,
                    out outputResult,
                    randomInt,
                    inputOutput,
                    output)
                .AddExecuteReader(
                    returnsTableProg,
                    async (reader, token) =>
                    {
                        Assert.IsTrue(await reader.ReadAsync(token));

                        Assert.AreEqual(randomString, reader.GetValue(0));
                        Assert.AreEqual(output.Value, reader.GetValue(1));
                        Assert.AreEqual(randomDecimal, reader.GetValue(2));
                        Assert.AreEqual(randomBool, reader.GetValue(3));
                    },
                    out tableResult,
                    randomString,
                    // Using output of previous program as input
                    output,
                    randomDecimal,
                    randomBool)
                .AddExecuteXmlReader(
                    returnsXmlProg,
                    (reader, token) =>
                    {
                        string xml = XElement.Load(reader).ToString();
                        Assert.AreEqual("<foo>bar</foo>", xml);
                        return TaskResult.Completed;
                    },
                    out SqlBatchResult xmlResult);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            using (CancellationTokenSource cts = new CancellationTokenSource(5000))
            try
            {
                CancellationToken token = cts.Token;

                await nonQueryResult.GetResultAsync(token);
                AddTime(sw, ref _batchedNonQueryTime);

                await scalarResult.GetResultAsync(token);
                AddTime(sw, ref _batchedScalarTime);

                await outputResult.GetResultAsync(token);
                AddTime(sw, ref _batchedOutputTime);

                await tableResult.GetResultAsync(token);
                AddTime(sw, ref _batchedTableTime);

                await xmlResult.GetResultAsync(token);
                AddTime(sw, ref _batchedXmlTime);

                await batch.ExecuteAsync(token);
                AddTime(sw, ref _batchedDoneTime);
            }
            finally
            {
                if (_count == 0)
                {
                    Trace.WriteLine("SQL:");
                    Trace.WriteLine(GetSql(batch) ?? "<not available>");
                }
            }
        }

        [TestMethod]
        private async Task RunNotBatched()
        {
            Setup(
                out SqlProgram<string, int> nonQueryProg,
                out SqlProgram<string, int, decimal, bool> returnsScalarProg,
                out SqlProgram<int, Out<int>, Out<int>> outputParametersProg,
                out SqlProgram<string, int, decimal, bool> returnsTableProg,
                out SqlProgram returnsXmlProg,
                out _,
                out string randomString,
                out int randomInt,
                out decimal randomDecimal,
                out bool randomBool,
                out Out<int> output,
                out Out<int> inputOutput);

            Stopwatch sw = Stopwatch.StartNew();

            using (CancellationTokenSource cts = new CancellationTokenSource(5000))
            {
                CancellationToken token = cts.Token;

                await nonQueryProg.ExecuteNonQueryAsync(randomString, randomInt, cancellationToken: token);
                AddTime(sw, ref _nonQueryTime);

                await returnsScalarProg.ExecuteScalarAsync<string>(
                    randomString,
                    randomInt,
                    randomDecimal,
                    randomBool,
                    cancellationToken: token);
                AddTime(sw, ref _scalarTime);

                await outputParametersProg.ExecuteScalarAsync<string>(
                    randomInt,
                    inputOutput,
                    output,
                    cancellationToken: token);
                AddTime(sw, ref _outputTime);

                await returnsTableProg.ExecuteReaderAsync(
                    async (reader, tk) =>
                    {
                        Assert.IsTrue(await reader.ReadAsync(tk));

                        Assert.AreEqual(randomString, reader.GetValue(0));
                        Assert.AreEqual(output.Value, reader.GetValue(1));
                        Assert.AreEqual(randomDecimal, reader.GetValue(2));
                        Assert.AreEqual(randomBool, reader.GetValue(3));
                    },
                    randomString,
                    // Using output of previous program as input
                    output.Value,
                    randomDecimal,
                    randomBool,
                    cancellationToken: token);
                AddTime(sw, ref _tableTime);

                await returnsXmlProg.ExecuteXmlReaderAsync(
                    (reader, tk) =>
                    {
                        string xml = XElement.Load(reader).ToString();
                        Assert.AreEqual("<foo>bar</foo>", xml);
                        return TaskResult.Completed;
                    },
                    cancellationToken: token);
                AddTime(sw, ref _xmlTime);

                AddTime(sw, ref _doneTime);
            }
        }

        private static string GetSql(SqlBatch batch)
        {
            FieldInfo field = typeof(SqlBatch).GetField("_sql", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null) return null;

            return (string)field.GetValue(batch);
        }
    }
}