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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;

// ReSharper disable ConsiderUsingConfigureAwait
// ReSharper disable InconsistentNaming
#pragma warning disable 1591 // Missing XML commend
#pragma warning disable 0618 // Type or member is obsolete

namespace WebApplications.Utilities.Database.Test
{
    public partial class SqlProgramTests
    {
        [TestMethod]
        public async Task ExecuteScalar_WithNoParameters_ExecuteReturnsExpectedString()
        {
            SqlProgram program = await SqlProgram.Create(DifferentLocalDatabaseConnection, "spReturnsScalar");

            string scalarResult = program.ExecuteScalar<string>();
            Assert.AreEqual("HelloWorld", scalarResult);
        }

        [TestMethod]
        public async Task ExecuteScalarAll_WithNoParameters_ExecuteReturnsExpectedString()
        {
            SqlProgram program = await SqlProgram.Create(
                new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString),
                "spReturnsScalarString");
            IList<string> scalarResult = program.ExecuteScalarAll<string>().ToList();
            Assert.AreEqual(2, scalarResult.Count);

            foreach (string result in scalarResult)
                Assert.AreEqual("HelloWorld", result);
        }

        [TestMethod]
        public async Task ExecuteScalar_WithParameters_ReturnedExpectedString()
        {
            SqlProgram<string, int, decimal, bool> program =
                await SqlProgram<string, int, decimal, bool>.Create(
                    DifferentLocalDatabaseConnection,
                    "spWithParametersReturnsScalarString");

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
        public async Task ExecuteScalarAll_WithParameters_ReturnedExpectedString()
        {
            SqlProgram<string, int, decimal, bool> program =
                await SqlProgram<string, int, decimal, bool>.Create(
                    new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString),
                    "spWithParametersReturnsScalar");

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
                Assert.AreEqual(
                    string.Format(
                        "{0} - {1} - {2} - 1",
                        randomString.Substring(0, randomString.Length),
                        AInt,
                        ADecimal),
                    result);
        }

        [TestMethod]
        public async Task ExecuteScalar_WithOutputParameters_ExecutesSuccessfully()
        {
            SqlProgram<int, Out<int>, Out<int>> program =
                await SqlProgram<int, Out<int>, Out<int>>.Create(LocalDatabaseConnection, "spOutputParameters");

            const int inputVal = 123;
            const int inputOutputVal = 321;

            Out<int> inputOutput = new Out<int>(inputOutputVal);
            Out<int> output = new Out<int>();

            string scalarResult = program.ExecuteScalar<string>(inputVal, inputOutput, output);
            Assert.AreEqual("<foo>bar</foo>", scalarResult);

            Assert.IsNull(inputOutput.OutputError, inputOutput.OutputError?.Message);
            Assert.IsNull(output.OutputError, output.OutputError?.Message);

            Assert.AreEqual(inputOutputVal * 2, inputOutput.OutputValue.Value);
            Assert.AreEqual(inputVal, output.OutputValue.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task ExecuteScalarAll_WithOutputParametersAndOut_ThrowsArgumentException()
        {
            SqlProgram<int, Out<int>, Out<int>> program =
                await SqlProgram<int, Out<int>, Out<int>>.Create(
                    new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString),
                    "spOutputParameters");

            const int inputVal = 123;
            const int inputOutputVal = 321;

            Out<int> inputOutput = new Out<int>(inputOutputVal);
            Out<int> output = new Out<int>();

            program.ExecuteScalarAll<string>(inputVal, inputOutput, output);
        }

        [TestMethod]
        public async Task ExecuteScalarAll_WithOutputParametersAndMultiOut_ExecutesSuccessfully()
        {
            SqlProgram<int, Out<int>, Out<int>> program =
                await SqlProgram<int, Out<int>, Out<int>>.Create(
                    new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString),
                    "spOutputParameters");

            const int inputVal = 123;
            const int inputOutputVal = 321;

            MultiOut<int> inputOutput = new MultiOut<int>(inputOutputVal);
            MultiOut<int> output = new MultiOut<int>();

            string[] scalarResult = program.ExecuteScalarAll<string>(inputVal, inputOutput, output).ToArray();
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