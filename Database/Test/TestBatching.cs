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
using System.Diagnostics;
using System.Text;
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
        private static void Setup(
            out SqlProgram<string, int> nonQueryProg,
            out SqlProgram<string, int, decimal, bool> returnsScalarProg,
            out SqlProgram<int, Out<int>, Out<int>> outputParametersProg,
            out SqlProgram<string, int, decimal, bool> returnsTableProg,
            out SqlProgram returnsXmlProg,
            out string randomString,
            out int randomInt,
            out decimal randomDecimal,
            out bool randomBool,
            out Out<int> output,
            out Out<int> inputOutput)
        {
            DatabaseElement database = DatabasesConfiguration.Active.Databases["test"];
            Assert.IsNotNull(database);

            nonQueryProg = database.GetSqlProgram<string, int>(
                    "spNonQuery",
                    "@stringParam",
                    "@intParam")
                .Result;

            returnsScalarProg = database.GetSqlProgram<string, int, decimal, bool>(
                    "spWithParametersReturnsScalar",
                    "@stringParam",
                    "@intParam",
                    "@decimalParam",
                    "@boolParam")
                .Result;

            outputParametersProg = database.GetSqlProgram<int, Out<int>, Out<int>>(
                    "spOutputParameters",
                    "@inputParam",
                    "@inputOutputParam",
                    "@outputParam")
                .Result;

            returnsTableProg = database.GetSqlProgram<string, int, decimal, bool>(
                    "spReturnsTable",
                    "@stringParam",
                    "@intParam",
                    "@decimalParam",
                    "@boolParam")
                .Result;

            returnsXmlProg = database.GetSqlProgram("spReturnsXml").Result;

            randomString = Random.RandomString(Encoding.GetEncoding(1252), maxLength: 20);
            randomInt = Random.RandomInt32();
            randomDecimal = Math.Round(Random.RandomDecimal() % 1_000_000_000m, 2);
            randomBool = Random.RandomBoolean();

            output = new Out<int>();
            inputOutput = new Out<int>(Random.RandomInt32() / 2);
        }

        [TestMethod]
        public async Task PerfTest()
        {
            for (int i = 0; i < 100; i++)
            {
                await TestBatchEverything();
                await TestNotBatchEverything();
                Trace.WriteLine("");
            }
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
                out string randomString,
                out int randomInt,
                out decimal randomDecimal,
                out bool randomBool,
                out Out<int> output,
                out Out<int> inputOutput);

            SqlBatch batch = new SqlBatch()
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
                    out SqlBatchResult<string> outputResult,
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
                    out SqlBatchResult tableResult,
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

            // ReSharper disable ConsiderUsingConfigureAwait,UseConfigureAwait
#pragma warning disable 4014
            nonQueryResult.GetResultAsync().ContinueWith(_ => Trace.WriteLine($"B spNonQuery @ {sw.Elapsed.TotalMilliseconds}ms"));
            scalarResult.GetResultAsync().ContinueWith(_ => Trace.WriteLine($"B spWithParametersReturnsScalar @ {sw.Elapsed.TotalMilliseconds}ms"));
            outputResult.GetResultAsync().ContinueWith(_ => Trace.WriteLine($"B spOutputParameters @ {sw.Elapsed.TotalMilliseconds}ms"));
            tableResult.GetResultAsync().ContinueWith(_ => Trace.WriteLine($"B spReturnsTable @ {sw.Elapsed.TotalMilliseconds}ms"));
            xmlResult.GetResultAsync().ContinueWith(_ => Trace.WriteLine($"B spReturnsXml @ {sw.Elapsed.TotalMilliseconds}ms"));
#pragma warning restore 4014

            await batch.ExecuteAsync();
            Trace.WriteLine($"B Done @ {sw.Elapsed.TotalMilliseconds}ms");
        }

        [TestMethod]
        public async Task TestNotBatchEverything()
        {
            Setup(
                out SqlProgram<string, int> nonQueryProg,
                out SqlProgram<string, int, decimal, bool> returnsScalarProg,
                out SqlProgram<int, Out<int>, Out<int>> outputParametersProg,
                out SqlProgram<string, int, decimal, bool> returnsTableProg,
                out SqlProgram returnsXmlProg,
                out string randomString,
                out int randomInt,
                out decimal randomDecimal,
                out bool randomBool,
                out Out<int> output,
                out Out<int> inputOutput);

            Stopwatch sw = Stopwatch.StartNew();

            await nonQueryProg.ExecuteNonQueryAsync(randomString, randomInt)
                .ContinueWith(_ => Trace.WriteLine($"N spNonQuery @ {sw.Elapsed.TotalMilliseconds}ms"));
            await returnsScalarProg.ExecuteScalarAsync<string>(randomString, randomInt, randomDecimal, randomBool)
                .ContinueWith(_ => Trace.WriteLine($"N spWithParametersReturnsScalar @ {sw.Elapsed.TotalMilliseconds}ms"));
            await outputParametersProg.ExecuteScalarAsync<string>(randomInt, inputOutput, output)
                .ContinueWith(_ => Trace.WriteLine($"N spOutputParameters @ {sw.Elapsed.TotalMilliseconds}ms"));
            await returnsTableProg.ExecuteReaderAsync(
                async (reader, token) =>
                {
                    Assert.IsTrue(await reader.ReadAsync(token));

                    Assert.AreEqual(randomString, reader.GetValue(0));
                    Assert.AreEqual(output.Value, reader.GetValue(1));
                    Assert.AreEqual(randomDecimal, reader.GetValue(2));
                    Assert.AreEqual(randomBool, reader.GetValue(3));
                },
                randomString,
                // Using output of previous program as input
                output.Value,
                randomDecimal,
                randomBool).ContinueWith(_ => Trace.WriteLine($"N spReturnsTable @ {sw.Elapsed.TotalMilliseconds}ms"));
            await returnsXmlProg.ExecuteXmlReaderAsync(
                (reader, token) =>
                {
                    string xml = XElement.Load(reader).ToString();
                    Assert.AreEqual("<foo>bar</foo>", xml);
                    return TaskResult.Completed;
                }).ContinueWith(_ => Trace.WriteLine($"N spReturnsXml @ {sw.Elapsed.TotalMilliseconds}ms"));
            Trace.WriteLine($"N Done @ {sw.Elapsed.TotalMilliseconds}ms");
        }
    }
}