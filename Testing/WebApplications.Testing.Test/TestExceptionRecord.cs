#region © Copyright Web Applications (UK) Ltd, 2015.  All rights reserved.
// Copyright (c) 2015, Web Applications UK Ltd
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
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing.Data;

namespace WebApplications.Testing.Test
{
    [TestClass]
    public class TestExceptionRecord
    {
        [TestMethod]
        public void TestSqlExceptionPrototype()
        {
            Parallel.For(
                0,
                100,
                i =>
                {
                    Random random = Tester.RandomGenerator;
                    SqlErrorCollectionPrototype errorCollectionPrototype =
                        new SqlErrorCollectionPrototype();

                    int loops = Tester.RandomGenerator.Next(10) + 1;
                    for (int loop = 0; loop < loops; loop++)
                    {
                        // Generate random values.
                        int infoNumber = random.RandomInt32();
                        byte errorState = random.RandomByte();
                        byte errorClass = (byte)random.Next(1, 26);
                        string server = random.RandomString();
                        string errorMessage = random.RandomString();
                        string procedure = random.RandomString();
                        int lineNumber = random.RandomInt32();
                        uint wind32ErrorCode = (uint)Math.Abs(random.RandomInt32());

                        // Create prototype.
                        SqlErrorPrototype sqlErrorPrototype = new SqlErrorPrototype(
                            infoNumber,
                            errorState,
                            errorClass,
                            server,
                            errorMessage,
                            procedure,
                            lineNumber,
                            wind32ErrorCode);

                        // Test implicit cast
                        SqlError sqlError = sqlErrorPrototype;
                        Assert.IsNotNull(sqlError);

                        // Check SqlError created properly
                        Assert.AreEqual(infoNumber, sqlError.Number);
                        Assert.AreEqual(errorState, sqlError.State);
                        Assert.AreEqual(errorClass, sqlError.Class);
                        Assert.AreEqual(server, sqlError.Server);
                        Assert.AreEqual(errorMessage, sqlError.Message);
                        Assert.AreEqual(procedure, sqlError.Procedure);
                        Assert.AreEqual(lineNumber, sqlError.LineNumber);
                        Assert.AreEqual(sqlErrorPrototype.ToString(), sqlError.ToString());

                        errorCollectionPrototype.Add(sqlError);
                    }

                    Assert.AreEqual(loops, errorCollectionPrototype.Count);

                    // Test implicit cast
                    SqlErrorCollection collection = errorCollectionPrototype;

                    Assert.AreSame(errorCollectionPrototype.SqlErrorCollection, collection);

                    // Now create a SqlException
                    Guid connectionId = Guid.NewGuid();
                    SqlExceptionPrototype sqlExceptionPrototype = new SqlExceptionPrototype(
                        collection,
                        "9.0.0.0",
                        connectionId);

                    // Test implicit conversion
                    SqlException sqlException = sqlExceptionPrototype;
                    Assert.IsNotNull(sqlException);

                    // Check SqlException created properly - it uses the first error from the collection.
                    SqlError first = collection[0];
                    Debug.Assert(first != null);
                    Assert.AreEqual(first.Number, sqlException.Number);
                    Assert.AreEqual(first.State, sqlException.State);
                    Assert.AreEqual(first.Class, sqlException.Class);
                    Assert.AreEqual(first.Server, sqlException.Server);
                    //Assert.AreEqual(first.Message, sqlException.Message);
                    Assert.AreEqual(first.Procedure, sqlException.Procedure);
                    Assert.AreEqual(first.LineNumber, sqlException.LineNumber);
                });
        }
    }
}