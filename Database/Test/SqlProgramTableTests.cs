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
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Logging;

// ReSharper disable ConsiderUsingConfigureAwait
// ReSharper disable InconsistentNaming
#pragma warning disable 1591 // Missing XML commend

namespace WebApplications.Utilities.Database.Test
{
    [TestClass]
    public class SqlProgramTableTests : DatabaseTestBase
    {
        private const string AString = "John Dough";
        private const int AInt = 30;
        private const decimal ADecimal = -200.15M;
        private const bool ABool = true;

        [TestMethod]
        public async Task Constructor_WithoutParameters_Succeeds()
        {
            SqlProgram program = await SqlProgram.Create(
                LocalDatabaseConnection,
                "ProgramName",
                "dbo.NormalTable",
                CommandType.TableDirect);
            Assert.IsNotNull(program);
        }

        [TestMethod]
        [ExpectedException(typeof(LoggingException))]
        public async Task Constructor_WithParameter_ThrowsLoggingException()
        {
            SqlProgram<int> program = await SqlProgram<int>.Create(
                LocalDatabaseConnection,
                "ProgramName",
                "@param",
                "dbo.NormalTable",
                CommandType.TableDirect);
            Assert.IsNotNull(program);
        }

        [TestMethod]
        public async Task ExecuteNonQueryAsync_ExecutesSuccessfully()
        {
            SqlProgram nonQueryTest =
                await SqlProgram.Create(
                    LocalDatabaseConnection,
                    "spNonQuery",
                    "dbo.NormalTable",
                    CommandType.TableDirect);
            Task<int> nonQueryResult = nonQueryTest.ExecuteNonQueryAsync();
            Assert.IsNotNull(nonQueryResult);
            nonQueryResult.Wait();
            Assert.AreEqual(-1, nonQueryResult.Result);
        }

        [TestMethod]
        public async Task ExecuteNonQueryAsyncAll_ExecutesSuccessfully()
        {
            SqlProgram nonQueryTest =
                await SqlProgram.Create(
                    new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString),
                    "spNonQuery",
                    "dbo.NormalTable",
                    CommandType.TableDirect);
            Task<IEnumerable<int>> nonQueryResult = nonQueryTest.ExecuteNonQueryAllAsync();
            nonQueryResult.Wait();
            Assert.AreEqual(2, nonQueryResult.Result.Count());

            foreach (int result in nonQueryResult.Result)
                Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public async Task ExecuteReaderAsync_ExecutesAndReturnsExpectedResult()
        {
            SqlProgram readerTest =
                await SqlProgram.Create(
                    LocalDatabaseConnection,
                    "spReturnsTable",
                    "dbo.NormalTable",
                    CommandType.TableDirect);

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
            result.Wait();

            Assert.AreEqual(AString, result.Result.Name);
            Assert.AreEqual(AInt, result.Result.Age);
            Assert.AreEqual(ADecimal, result.Result.Balance);
            Assert.AreEqual(ABool, result.Result.IsValued);
        }

        [TestMethod]
        public async Task ExecuteReaderAllAsync_ExecutesAndReturnsExpectedResult()
        {
            SqlProgram readerTest = await SqlProgram.Create(
                new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString),
                "spReturnsTable",
                "dbo.NormalTable",
                CommandType.TableDirect);

            Task<IEnumerable<dynamic>> result = readerTest.ExecuteReaderAllAsync(
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
        public async Task ExecuteScalarAsync_ExecutesAndReturnsExpectedResult()
        {
            SqlProgram scalarTest =
                await SqlProgram.Create(
                    LocalDatabaseConnection,
                    "spReturnsScalar",
                    "dbo.ScalarTable",
                    CommandType.TableDirect);

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
                    "dbo.ScalarTable",
                    CommandType.TableDirect);

            Task<IEnumerable<string>> tasks = scalarTest.ExecuteScalarAllAsync<string>();
            Assert.AreEqual(2, tasks.Result.Count());
            tasks.Wait();
            Assert.IsTrue(tasks.IsCompleted);

            foreach (string result in tasks.Result)
                Assert.AreEqual("HelloWorld", result);
        }

        [TestMethod]
        public async Task ExecuteXmlReaderAsync_ExecuteReturnsExpectedString()
        {
            SqlProgram program = await SqlProgram.Create(
                LocalDatabaseConnection,
                "spReturnsXml",
                "dbo.XmlTable",
                CommandType.TableDirect);

            XElement result = await program.ExecuteXmlReaderAsync(async (reader, token) => XElement.Load(reader));
            Assert.AreEqual("<foo>bar</foo>", result.ToString());
        }

        [TestMethod]
        public async Task ExecuteXmlReaderAllAsync_ExecuteReturnsExpectedString()
        {
            SqlProgram program = await SqlProgram.Create(
                new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString),
                "spReturnsXml",
                "dbo.XmlTable",
                CommandType.TableDirect);

            XElement[] results = (await program.ExecuteXmlReaderAllAsync(async (reader, token) => XElement.Load(reader))).ToArray();
            Assert.AreEqual(2, results.Length);

            foreach (XElement result in results)
                Assert.AreEqual("<foo>bar</foo>", result.ToString());
        }

    }
}