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

using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;
using WebApplications.Utilities.Database.Configuration;

namespace WebApplications.Utilities.Database.Test
{
    [TestClass]
    public class TestBatching : DatabaseTestBase
    {
        [TestMethod]
        public async Task TestBatchEverything()
        {
            // ReSharper disable ConsiderUsingConfigureAwait,UseConfigureAwait
            DatabaseElement database = DatabasesConfiguration.Active.Databases["test"];
            Assert.IsNotNull(database);

            var nonQueryProg = await database.GetSqlProgram<string, int>(
                "spNonQuery",
                "@stringParam",
                "@intParam");

            var returnsScalarProg = await database.GetSqlProgram<string, int, decimal, bool>(
                "spWithParametersReturnsScalar",
                "@stringParam",
                "@intParam",
                "@decimalParam",
                "@boolParam");

            var outputParametersProg = await database.GetSqlProgram<int, Out<int>, Out<int>>(
                "spOutputParameters",
                "@inputParam",
                "@inputOutputParam",
                "@outputParam");

            var returnsTableProg = await database.GetSqlProgram<string, int, decimal, bool>(
                "spWithParametersReturnsScalar",
                "@stringParam",
                "@intParam",
                "@decimalParam",
                "@boolParam");


            string randomString = Random.RandomString(unicode: false);
            int randomInt = Random.RandomInt32();
            decimal randomDecimal = Random.RandomDecimal() % 1_000_000_000m;
            bool randomBool = Random.RandomBoolean();

            Out<int> output = new Out<int>();
            Out<int> inputOutput = new Out<int>(Random.RandomInt32());

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

                        Assert.AreEqual(reader.GetValue(0), randomString);
                        Assert.AreEqual(reader.GetValue(1), output.Value);
                        Assert.AreEqual(reader.GetValue(2), randomDecimal);
                        Assert.AreEqual(reader.GetValue(3), randomBool);
                    },
                    out _,
                    randomString,
                    // Using output of previous program as input
                    output,
                    randomDecimal,
                    randomBool);

            await batch.ExecuteAsync();

            Trace.WriteLine($"NonQuery: {await nonQueryResult.GetResultAsync()}");
            Trace.WriteLine($"Scalar: {await scalarResult.GetResultAsync()}");
            // ReSharper restore ConsiderUsingConfigureAwait,UseConfigureAwait
        }
    }
}