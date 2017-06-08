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

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;
using WebApplications.Utilities.Annotations;

// ReSharper disable ConsiderUsingConfigureAwait
// ReSharper disable InconsistentNaming
#pragma warning disable 1591 // Missing XML commend
#pragma warning disable 0618 // Type or member is obsolete

namespace WebApplications.Utilities.Database.Test
{
    public partial class SqlProgramTextTests
    {
        [NotNull]
        private static readonly string Different_spReturnsScalar =
            GetProgramText("spReturnsScalar", DifferentLocalDatabaseConnectionString);

        [NotNull]
        private static readonly string Local_spReturnsScalarString =
            GetProgramText("spReturnsScalarString", LocalDatabaseConnectionString);

        [NotNull]
        private static readonly string Different_spTakesParamAndReturnsScalar =
            GetProgramText("spTakesParamAndReturnsScalar", DifferentLocalDatabaseConnectionString);

        [TestMethod]
        public async Task ExecuteScalarAsync_ExecutesAndReturnsExpectedResult()
        {
            SqlProgram scalarTest =
                await SqlProgram.Create(
                    DifferentLocalDatabaseConnection,
                    "spReturnsScalar",
                    Different_spReturnsScalar,
                    CommandType.Text);

            Task<string> task = scalarTest.ExecuteScalarAsync<string>();
            Assert.IsNotNull(task);
            task.Wait();
            Assert.IsTrue(task.IsCompleted);
            Assert.AreEqual("HelloWorld", task.Result);
        }

        [TestMethod]
        public async Task ExecuteScalarAsyncAll_ExecutesAndReturnsExpectedResult()
        {
            string[] connectionStrings =
            {
                LocalDatabaseConnectionString,
                LocalDatabaseCopyConnectionString
            };

            SqlProgram scalarTest =
                await SqlProgram.Create(
                    new LoadBalancedConnection(connectionStrings),
                    "spReturnsScalarString",
                    Local_spReturnsScalarString,
                    CommandType.Text);

            Task<IEnumerable<string>> tasks = scalarTest.ExecuteScalarAllAsync<string>();
            Assert.AreEqual(2, tasks.Result.Count());
            tasks.Wait();
            Assert.IsTrue(tasks.IsCompleted);

            foreach (string result in tasks.Result)
                Assert.AreEqual("HelloWorld", result);
        }

        [TestMethod]
        public async Task ExecuteScalarAsync_WithParameter_ExecutesAndReturnsExpectedResult()
        {
            SqlProgram<string> scalarTest =
                await SqlProgram<string>.Create(
                    DifferentLocalDatabaseConnection,
                    "spTakesParamAndReturnsScalar",
                    "@firstName",
                    Different_spTakesParamAndReturnsScalar,
                    CommandType.Text);

            string randomString = Random.RandomString(10, false);
            Task<string> task = scalarTest.ExecuteScalarAsync<string>(randomString);

            Assert.IsNotNull(task);
            task.Wait();
            Assert.IsTrue(task.IsCompleted);
            Assert.AreEqual("Hello " + randomString, task.Result);
        }

        [TestMethod]
        public async Task ExecuteScalarAsync_WithOutputParameters_ExecutesSuccessfully()
        {
            SqlProgram<int, Out<int>, Out<int>> program =
                await SqlProgram<int, Out<int>, Out<int>>.Create(
                    LocalDatabaseConnection,
                    "spOutputParameters",
                    "@inputParam",
                    "@inputOutputParam",
                    "@outputParam",
                    Local_spOutputParameters,
                    CommandType.Text);

            const int inputVal = 123;
            const int inputOutputVal = 321;

            Out<int> inputOutput = new Out<int>(inputOutputVal);
            Out<int> output = new Out<int>();

            string scalarResult = await program.ExecuteScalarAsync<string>(inputVal, inputOutput, output);
            Assert.AreEqual("<foo>bar</foo>", scalarResult);

            Assert.IsNull(inputOutput.OutputError, inputOutput.OutputError?.Message);
            Assert.IsNull(output.OutputError, output.OutputError?.Message);

            Assert.AreEqual(inputOutputVal * 2, inputOutput.OutputValue.Value);
            Assert.AreEqual(inputVal, output.OutputValue.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task ExecuteScalarAllAsync_WithOutputParametersAndOut_ThrowsArgumentException()
        {
            SqlProgram<int, Out<int>, Out<int>> program =
                await SqlProgram<int, Out<int>, Out<int>>.Create(
                    new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString),
                    "spOutputParameters",
                    "@inputParam",
                    "@inputOutputParam",
                    "@outputParam",
                    Local_spOutputParameters,
                    CommandType.Text);

            const int inputVal = 123;
            const int inputOutputVal = 321;

            Out<int> inputOutput = new Out<int>(inputOutputVal);
            Out<int> output = new Out<int>();

            await program.ExecuteScalarAllAsync<string>(inputVal, inputOutput, output);
        }

        [TestMethod]
        public async Task ExecuteScalarAllAsync_WithOutputParametersAndMultiOut_ExecutesSuccessfully()
        {
            SqlProgram<int, Out<int>, Out<int>> program =
                await SqlProgram<int, Out<int>, Out<int>>.Create(
                    new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString),
                    "spOutputParameters",
                    "@inputParam",
                    "@inputOutputParam",
                    "@outputParam",
                    Local_spOutputParameters,
                    CommandType.Text);

            const int inputVal = 123;
            const int inputOutputVal = 321;

            MultiOut<int> inputOutput = new MultiOut<int>(inputOutputVal);
            MultiOut<int> output = new MultiOut<int>();

            string[] scalarResult =
                (await program.ExecuteScalarAllAsync<string>(inputVal, inputOutput, output)).ToArray();
            Assert.AreEqual(2, scalarResult.Length);
            Assert.IsTrue(scalarResult.All(i => i == "<foo>bar</foo>"));

            Assert.IsNull(inputOutput.OutputError, inputOutput.OutputError?.Message);
            Assert.IsNull(output.OutputError, output.OutputError?.Message);

            Assert.AreEqual(inputOutputVal * 2, inputOutput.OutputValue.Value);
            Assert.AreEqual(inputVal, output.OutputValue.Value);

            Assert.IsTrue(inputOutput.All(o => o.OutputValue.Value == inputOutputVal * 2));
            Assert.IsTrue(output.All(o => o.OutputValue.Value == inputVal));
        }
    }
}