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
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using WebApplications.Utilities.Database.Exceptions;
using WebApplications.Utilities.IO;

// ReSharper disable ConsiderUsingConfigureAwait
// ReSharper disable InconsistentNaming
#pragma warning disable 1591 // Missing XML commend
#pragma warning disable 0618 // Type or member is obsolete

namespace WebApplications.Utilities.Database.Test
{
    public partial class SqlProgramTests
    {
        [TestMethod]
        public async Task ExecuteReaderAsync_ExecutesAndReturnsExpectedResult()
        {
            SqlProgram readerTest = await SqlProgram.Create(DifferentLocalDatabaseConnection, "spUltimateSproc");
            Task readerTask = readerTest.ExecuteReaderAsync();
            Assert.IsNotNull(readerTask);
        }

        [TestMethod]
        public async Task ExecuteReaderAllAsync_ExecutesAndReturnsExpectedResult()
        {
            SqlProgram program = await SqlProgram.Create(
                new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString),
                "spReturnsTable");

            Task readerTask = program.ExecuteReaderAllAsync();
            Assert.IsNotNull(readerTask);
        }

        [TestMethod]
        public async Task ExecuteReaderAsync_WithReturnType_ExecutesAndReturnsExpectedResult()
        {
            SqlProgram readerTest =
                await SqlProgram.Create(DifferentLocalDatabaseConnection, "spUltimateSproc");

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
                await SqlProgram<string, int, decimal, bool>.Create(
                    DifferentLocalDatabaseConnection,
                    "spUltimateSproc");

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
                new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString),
                "spReturnsTable");

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

        private dynamic CreateDatabaseResult(DbDataReader reader)
        {
            return new
            {
                Name = reader.GetValue<string>(0),
                Age = reader.GetValue<int>(1),
                Balance = reader.GetValue<decimal>(2),
                IsValued = reader.GetValue<bool>(3)
            };
        }


        [TestMethod]
        public async Task ExecuteReaderAsync_WithManualDisposal_ExecutesAndAllowsStreaming()
        {
            SqlProgram<byte[]> readerTest =
                await SqlProgram<byte[]>.Create(DifferentLocalDatabaseConnection, "spTakeByteArray");

            byte[] data = Encoding.UTF8.GetBytes(AString);
            string resultString;

            using (Stream stream = await readerTest.ExecuteReaderAsync(
                async (reader, disposable, token) =>
                {
                    if (await reader.ReadAsync(token))
                        return (Stream)new CloseableStream(reader.GetStream(0), disposable);

                    throw new Exception("Critical Test Error");
                },
                data).ConfigureAwait(false))
            {
                Assert.IsNotNull(stream);
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    resultString = reader.ReadToEnd();
            }
            Assert.AreEqual(AString, resultString);
        }

        [TestMethod]
        [ExpectedException(typeof(SqlProgramExecutionException), AllowDerivedTypes = true)]
        public async Task ExecuteReaderAsync_WithManualDisposal_Timeouts()
        {
            Duration commandTimeout = Duration.FromSeconds(1);
            SqlProgram<int> timeoutTest =
                await
                    SqlProgram<int>.Create(
                        DifferentLocalDatabaseConnection,
                        "spTimeoutTest",
                        defaultCommandTimeout: commandTimeout);

            // Expose all the properties from the inner lambda.
            dynamic result = await timeoutTest.ExecuteReaderAsync(
                    (reader, disposable, token) =>
                        Task.FromResult(new { Reader = reader, Disposable = disposable, Token = token }),
                    10)
                .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ExecuteReaderAsync_WithOutputParameters_ExecutesSuccessfully()
        {
            SqlProgram<int, Out<int>, Out<int>> program =
                await SqlProgram<int, Out<int>, Out<int>>.Create(LocalDatabaseConnection, "spOutputParameters");

            const int inputVal = 123;
            const int inputOutputVal = 321;

            Out<int> inputOutput = new Out<int>(inputOutputVal);
            Out<int> output = new Out<int>();

            string result = await program.ExecuteReaderAsync(
                async (reader, token) =>
                {
                    Assert.IsTrue(await reader.ReadAsync());

                    string res = reader.GetString(0);

                    Assert.IsFalse(await reader.ReadAsync());
                    Assert.IsFalse(await reader.NextResultAsync());

                    return res;
                },
                inputVal,
                inputOutput,
                output);
            Assert.AreEqual("<foo>bar</foo>", result);

            Assert.IsNull(inputOutput.OutputError, inputOutput.OutputError?.Message);
            Assert.IsNull(output.OutputError, output.OutputError?.Message);

            Assert.AreEqual(inputOutputVal * 2, inputOutput.OutputValue.Value);
            Assert.AreEqual(inputVal, output.OutputValue.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task ExecuteReaderAllAsync_WithOutputParametersAndOut_ThrowsArgumentException()
        {
            SqlProgram<int, Out<int>, Out<int>> program =
                await SqlProgram<int, Out<int>, Out<int>>.Create(
                    new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString),
                    "spOutputParameters");

            const int inputVal = 123;
            const int inputOutputVal = 321;

            Out<int> inputOutput = new Out<int>(inputOutputVal);
            Out<int> output = new Out<int>();

            await program.ExecuteReaderAllAsync(
                async (reader, token) => Assert.Fail("Shouldnt reach this point."),
                inputVal,
                inputOutput,
                output);
        }

        [TestMethod]
        public async Task ExecuteReaderAllAsync_WithOutputParametersAndMultiOut_ExecutesSuccessfully()
        {
            SqlProgram<int, Out<int>, Out<int>> program =
                await SqlProgram<int, Out<int>, Out<int>>.Create(
                    new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString),
                    "spOutputParameters");

            const int inputVal = 123;
            const int inputOutputVal = 321;

            MultiOut<int> inputOutput = new MultiOut<int>(inputOutputVal);
            MultiOut<int> output = new MultiOut<int>();

            string[] result = (await program.ExecuteReaderAllAsync(
                async (reader, token) =>
                {
                    Assert.IsTrue(await reader.ReadAsync());

                    string res = reader.GetString(0);

                    Assert.IsFalse(await reader.ReadAsync());
                    Assert.IsFalse(await reader.NextResultAsync());

                    return res;
                },
                inputVal,
                inputOutput,
                output)).ToArray();

            Assert.AreEqual(2, result.Length);
            Assert.IsTrue(result.All(i => i == "<foo>bar</foo>"));

            Assert.IsNull(inputOutput.OutputError, inputOutput.OutputError?.Message);
            Assert.IsNull(output.OutputError, output.OutputError?.Message);

            Assert.AreEqual(inputOutputVal * 2, inputOutput.OutputValue.Value);
            Assert.AreEqual(inputVal, output.OutputValue.Value);

            Assert.IsTrue(inputOutput.All(o => o.OutputValue.Value == inputOutputVal * 2));
            Assert.IsTrue(output.All(o => o.OutputValue.Value == inputVal));
        }
    }
}