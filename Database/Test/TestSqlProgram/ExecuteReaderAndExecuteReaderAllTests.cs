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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;
using WebApplications.Utilities.Database.Configuration;
using WebApplications.Utilities.Database.Exceptions;

// ReSharper disable ConsiderUsingConfigureAwait
// ReSharper disable InconsistentNaming
#pragma warning disable 1591 // Missing XML commend
#pragma warning disable 0618 // Type or member is obsolete

namespace WebApplications.Utilities.Database.Test
{
    public partial class SqlProgramTests
    {
        [TestMethod]
        public async Task ExecuteReader_ExecutesSuccessfully()
        {
            SqlProgram readerTest = await SqlProgram.Create(
                DifferentLocalDatabaseConnection,
                "spUltimateSproc");

            readerTest.ExecuteReader();
        }

        [TestMethod]
        public async Task ExecuteReaderAll_ExecutesSuccessfully()
        {
            SqlProgram readerTest =
                await SqlProgram.Create(
                    new LoadBalancedConnection(
                        LocalDatabaseConnectionString,
                        LocalDatabaseCopyConnectionString),
                    "spNonQuery");

            readerTest.ExecuteReaderAll();

            // Can't really do any assertions here so test is just that it doesn't throw an exception.
        }

        [TestMethod]
        public async Task ExecuteReader_WithReturnResultSet_ExecutesSuccessfully()
        {
            SqlProgram readerTest = await SqlProgram.Create(
                DifferentLocalDatabaseConnection,
                "spUltimateSproc");

            dynamic result = readerTest.ExecuteReader<dynamic>(
                reader =>
                {
                    Assert.IsTrue(reader.Read());

                    var res = new
                    {
                        Name = reader.GetValue<string>(0),
                        Age = reader.GetValue<int>(1),
                        Balance = reader.GetValue<decimal>(2),
                        IsValued = reader.GetValue<bool>(3)
                    };

                    Assert.IsFalse(reader.Read());
                    Assert.IsFalse(reader.NextResult());

                    return res;
                });

            Assert.IsNotNull(result);

            // Read the sproc defaults.
            Assert.AreEqual("A Test String", result.Name);
            Assert.AreEqual(5, result.Age);
            Assert.AreEqual(200.15M, result.Balance);
            Assert.AreEqual(false, result.IsValued);
        }

        [TestMethod]
        public async Task ExecuteReader_WithAllParametersSet_ExecutesAndReturnsExpectedResult()
        {
            SqlProgram<string, int, decimal, bool> readerTest =
                await SqlProgram<string, int, decimal, bool>.Create(
                    DifferentLocalDatabaseConnection,
                    "spUltimateSproc");

            dynamic result = readerTest.ExecuteReader<dynamic>(
                c =>
                {
                    c.SetParameter("@stringParam", AString);
                    c.SetParameter("@intParam", AInt);
                    c.SetParameter("@decimalParam", ADecimal);
                    c.SetParameter("@boolParam", ABool);
                },
                reader =>
                {
                    Assert.IsTrue(reader.Read());

                    var res = new
                    {
                        Name = reader.GetValue<string>(0),
                        Age = reader.GetValue<int>(1),
                        Balance = reader.GetValue<decimal>(2),
                        IsValued = reader.GetValue<bool>(3)
                    };

                    Assert.IsFalse(reader.Read());
                    Assert.IsFalse(reader.NextResult());

                    return res;
                });

            Assert.IsNotNull(result);

            Assert.AreEqual(AString, result.Name);
            Assert.AreEqual(AInt, result.Age);
            Assert.AreEqual(ADecimal, result.Balance);
            Assert.AreEqual(ABool, result.IsValued);
        }

        [TestMethod]
        public async Task ExecuteReader_WithEnumerableIntParameter_ReturnsSingleColumnTableMatchingTheParameterType()
        {
            SqlProgram<IEnumerable<int>> tableTypeTest =
                await SqlProgram<IEnumerable<int>>.Create(
                    DifferentLocalDatabaseConnection,
                    "spTakesIntTable");

            IList<int> result = tableTypeTest.ExecuteReader(
                reader =>
                {
                    List<int> resultSet = new List<int>();
                    while (reader.Read())
                        resultSet.Add(reader.GetValue<int>(0));
                    return resultSet;
                },
                new[] { 0, 1, 2, 3 });

            Assert.AreEqual(4, result.Count);
            for (int i = 0; i < result.Count; i++)
                Assert.AreEqual(i, result[i]);
        }

        [TestMethod]
        public async Task
            ExecuteReader_WithEnumerableKeyValuePairParameter_ReturnsTwoColumnTableMatchingTheParameterTypes()
        {
            SqlProgram<IEnumerable<KeyValuePair<int, string>>> tableTypeTest =
                await SqlProgram<IEnumerable<KeyValuePair<int, string>>>.Create(
                    DifferentLocalDatabaseConnection,
                    "spTakesKvpTable");

            string str1 = Random.RandomString(10, false);
            string str2 = Random.RandomString(10, false);
            string str3 = Random.RandomString(10, false);

            IDictionary<int, string> result = tableTypeTest.ExecuteReader(
                reader =>
                {
                    IDictionary<int, string> resultSet = new Dictionary<int, string>();
                    while (reader.Read())
                        resultSet.Add(reader.GetValue<int>(0), reader.GetValue<string>(1));
                    return resultSet;
                },
                new Dictionary<int, string>
                {
                    { 0, str1 },
                    { 1, str2 },
                    { 2, str3 }
                });

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(str1, result[0]);
            Assert.AreEqual(str2, result[1]);
            Assert.AreEqual(str3, result[2]);
        }

        [TestMethod]
        public async Task ExecuteReader_WithTupleParameter_ExecutesSuccessfully()
        {
            SqlProgram<IEnumerable<Tuple<int, string, bool>>> tableTypeTest =
                await SqlProgram<IEnumerable<Tuple<int, string, bool>>>.Create(
                    DifferentLocalDatabaseConnection,
                    "spTakesTupleTable");

            string str1 = Random.RandomString(10, false);
            string str2 = Random.RandomString(10, false);

            IList<dynamic> result =
                tableTypeTest.ExecuteReader(
                    reader =>
                    {
                        IList<dynamic> resultSet = new List<dynamic>();
                        while (reader.Read())
                        {
                            resultSet.Add(
                                new
                                {
                                    IntColumn = reader.GetValue<int>(0),
                                    StringColumn = reader.GetValue<string>(1),
                                    BoolColumn = reader.GetValue<bool>(2)
                                });
                        }

                        return resultSet;
                    },
                    new List<Tuple<int, string, bool>>
                    {
                        new Tuple<int, string, bool>(1, str1, false),
                        new Tuple<int, string, bool>(2, str2, true)
                    });

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result[0].IntColumn);
            Assert.AreEqual(2, result[1].IntColumn);
            Assert.AreEqual(str1, result[0].StringColumn);
            Assert.AreEqual(str2, result[1].StringColumn);
            Assert.IsFalse(result[0].BoolColumn);
            Assert.IsTrue(result[1].BoolColumn);
        }

        [TestMethod]
        public async Task ExecuteReader_WithByteArrayParameter_ReturnsSameArrayInReader()
        {
            SqlProgram<byte[]> byteArrayTest = await SqlProgram<byte[]>.Create(
                DifferentLocalDatabaseConnection,
                "spTakeByteArrayLength10");

            int length = Random.Next(1, 10);
            byte[] testParam = new byte[length];
            Random.NextBytes(testParam);

            byteArrayTest.ExecuteReader(
                reader =>
                {
                    if (reader.Read())
                    {
                        CollectionAssert.AreEqual(
                            testParam,
                            (ICollection)reader.GetValue(0));
                    }
                },
                testParam);
        }

        [TestMethod]
        [ExpectedException(typeof(SqlProgramExecutionException), AllowDerivedTypes = true)]
        public async Task ExecuteReader_WithByteArrayParameterTooLong_ThrowsExceptionWhenTypeConstraintModeError()
        {
            // This Sql Program is configured in the app.config to use TypeConstraintMode Error, so will throw error
            // if byte[] is truncated.
            SqlProgram<byte[]> byteArrayTest = await DatabasesConfiguration.GetConfiguredSqlProgram<byte[]>(
                "test",
                "spTakeByteArrayLength10",
                "@byteArrayParam");

            byte[] testParam = new byte[11];
            Random.NextBytes(testParam);

            byteArrayTest.ExecuteReader(
                reader =>
                {
                    if (reader.Read())
                    {
                        CollectionAssert.AreEqual(
                            testParam,
                            (ICollection)reader.GetValue(0));
                    }
                },
                testParam);
        }

        [TestMethod]
        public async Task ExecuteReader_WithSerializableObjectParameter_ReturnsByteArray()
        {
            SqlProgram<TestSerializableObject> serializeObjectTest = await SqlProgram<TestSerializableObject>.Create(
                DifferentLocalDatabaseConnection,
                "spTakeByteArray");

            TestSerializableObject objecToSerialize =
                new TestSerializableObject { String1 = Random.RandomString(), String2 = Random.RandomString() };
            serializeObjectTest.ExecuteReader(
                reader =>
                {
                    Assert.IsTrue(reader.Read());
                    Assert.IsInstanceOfType(reader.GetValue(0), typeof(byte[]));

                    // Deserialize object
                    TestSerializableObject deserializedObject =
                        (TestSerializableObject)reader.GetObjectByName(reader.GetName(0));

                    // Check we don't have same object instance.
                    Assert.IsFalse(ReferenceEquals(objecToSerialize, deserializedObject));

                    // Check equality of object instances using equality method.
                    Assert.AreEqual(objecToSerialize, deserializedObject);
                },
                objecToSerialize);
        }

        [TestMethod]
        public async Task ExecuteReader_WithNestedTupleParameter_ExecutesSuccessfully()
        {
            SqlProgram<IEnumerable<Tuple<int, string, bool, bool, decimal, decimal, double, Tuple<string, short,
                    TestSerializableObject, byte, DateTime, DateTime, XElement, Tuple<int, long, int, int>>>>>
                tupleTableTypeTest =
                    await SqlProgram<IEnumerable<Tuple<int, string, bool, bool, decimal, decimal, double,
                        Tuple<string, short, TestSerializableObject, byte, DateTime, DateTime, XElement,
                            Tuple<int, long, int, int>>>>>.Create(
                        DifferentLocalDatabaseConnection,
                        "spTakesMultiTupleTable");

            var rows = new List<Tuple<int, string, bool, bool, decimal, decimal, double, Tuple<string, short,
                TestSerializableObject, byte, DateTime, DateTime, XElement, Tuple<int, long, int, int>>>>();

            for (int i = 0; i < Random.Next(3, 10); i++)
            {
                rows.Add(
                    ExtendedTuple.Create(
                        Random.RandomInt32(),
                        Random.RandomString(50, false),
                        false,
                        true,
                        RandomSqlSafeDecimal(),
                        RandomSqlSafeDecimal(),
                        Random.RandomDouble(),
                        Random.RandomString(),
                        Random.RandomInt16(),
                        new TestSerializableObject { String1 = Random.RandomString(), String2 = Random.RandomString() },
                        Random.RandomByte(),
                        RandomSqlSafeDateTime(),
                        RandomSqlSafeDateTime(),
                        new XElement("Test", new XAttribute("attribute", Random.Next())),
                        Random.RandomInt32(),
                        Random.RandomInt64(),
                        Random.RandomInt32(),
                        Random.RandomInt32()));
            }

            var indexer = typeof(Tuple<int, string, bool, bool, decimal, decimal, double, Tuple<string, short,
                    TestSerializableObject, byte, DateTime, DateTime, XElement, Tuple<int, long, int, int>>>)
                .GetTupleIndexer();

            tupleTableTypeTest.ExecuteReader(
                reader =>
                {
                    int r = 0;
                    while (reader.Read())
                    {
                        var tuple = rows[r++];
                        for (int c = 0; c < 18; c++)
                        {
                            // Get the tuple value that was passed into the sproc.
                            object cell = indexer(tuple, c);

                            // Check collections (e.g. byte[])
                            ICollection cellCollection = cell as ICollection;
                            if (cellCollection != null)
                            {
                                ICollection resultCollection =
                                    reader.GetValue(c) as ICollection;
                                Assert.IsNotNull(
                                    resultCollection,
                                    "The db did not return a collection");
                                CollectionAssert.AreEqual(cellCollection, resultCollection);
                                continue;
                            }

                            // Check serialized object
                            TestSerializableObject serializedObject =
                                cell as TestSerializableObject;
                            if (serializedObject != null)
                            {
                                // Deserialize object
                                TestSerializableObject deserializedObject =
                                    (TestSerializableObject)reader.GetObjectByName(reader.GetName(c));

                                // Check we don't have same object instance.
                                Assert.IsFalse(ReferenceEquals(serializedObject, deserializedObject));

                                // Check equality of object instances using equality method.
                                Assert.AreEqual(serializedObject, deserializedObject);
                                continue;
                            }

                            // Check XELement
                            XElement xelement = cell as XElement;
                            if (xelement != null)
                            {
                                XElement result = XElement.Parse(reader.GetString(c));
                                Assert.AreEqual(xelement.ToString(), result.ToString());
                                continue;
                            }

                            Assert.AreEqual(cell, reader.GetValue(c));
                        }
                    }
                },
                rows);
        }

        [TestMethod]
        public async Task ExecuteReader_WithOutputParameters_ExecutesSuccessfully()
        {
            SqlProgram<int, Out<int>, Out<int>> program =
                await SqlProgram<int, Out<int>, Out<int>>.Create(
                    LocalDatabaseConnection,
                    "spOutputParameters");

            const int inputVal = 123;
            const int inputOutputVal = 321;

            Out<int> inputOutput = new Out<int>(inputOutputVal);
            Out<int> output = new Out<int>();

            string result = program.ExecuteReader(
                (reader) =>
                {
                    Assert.IsTrue(reader.Read());

                    string res = reader.GetString(0);

                    Assert.IsFalse(reader.Read());
                    Assert.IsFalse(reader.NextResult());

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
        public async Task ExecuteReaderAll_WithOutputParametersAndOut_ThrowsArgumentException()
        {
            SqlProgram<int, Out<int>, Out<int>> program =
                await SqlProgram<int, Out<int>, Out<int>>.Create(
                    new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString),
                    "spOutputParameters");

            const int inputVal = 123;
            const int inputOutputVal = 321;

            Out<int> inputOutput = new Out<int>(inputOutputVal);
            Out<int> output = new Out<int>();

            program.ExecuteReaderAll(
                (reader) => { Assert.Fail("Shouldnt reach this point."); },
                inputVal,
                inputOutput,
                output);
        }

        [TestMethod]
        public async Task ExecuteReaderAll_WithOutputParametersAndMultiOut_ExecutesSuccessfully()
        {
            SqlProgram<int, Out<int>, Out<int>> program =
                await SqlProgram<int, Out<int>, Out<int>>.Create(
                    new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString),
                    "spOutputParameters");

            const int inputVal = 123;
            const int inputOutputVal = 321;

            MultiOut<int> inputOutput = new MultiOut<int>(inputOutputVal);
            MultiOut<int> output = new MultiOut<int>();

            string[] result = program.ExecuteReaderAll(
                (reader) =>
                {
                    Assert.IsTrue(reader.Read());

                    string res = reader.GetString(0);

                    Assert.IsFalse(reader.Read());
                    Assert.IsFalse(reader.NextResult());

                    return res;
                },
                inputVal,
                inputOutput,
                output).ToArray();

            Assert.AreEqual(2, result.Length);
            Assert.IsTrue(result.All(i => i == "<foo>bar</foo>"));

            Assert.IsNull(inputOutput.OutputError, inputOutput.OutputError?.Message);
            Assert.IsNull(output.OutputError, output.OutputError?.Message);

            Assert.AreEqual(inputOutputVal * 2, inputOutput.OutputValue.Value);
            Assert.AreEqual(inputVal, output.OutputValue.Value);

            Assert.IsTrue(inputOutput.All(o => o.OutputValue.Value == inputOutputVal * 2));
            Assert.IsTrue(output.All(o => o.OutputValue.Value == inputVal));
        }

        /// <summary>
        /// Generates a random decimal between -999999999.999999999 and 999999999.999999999, which can be passed
        /// to spTakesMultiTupleTable without causing truncation errors.
        /// </summary>
        /// <returns></returns>
        private static decimal RandomSqlSafeDecimal()
        {
            return new decimal(Random.NextDouble() * 1999999999.99999998 - 999999999.999999999);
        }

        /// <summary>
        /// Generates a random DateTime between 1970 and 2038, without fractional seconds, which can be passed
        /// to spTakesMultiTupleTable without causing truncation errors.
        /// </summary>
        /// <returns></returns>
        private static DateTime RandomSqlSafeDateTime()
        {
            return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds((UInt32)Random.Next());
        }

        [Serializable]
        private class TestSerializableObject
        {
            public string String1 { get; set; }
            public string String2 { get; set; }

            /// <summary>
            /// Returns a <see cref="System.String"/> that represents this instance.
            /// </summary>
            /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
            /// <remarks></remarks>
            public override string ToString()
            {
                return String.Format(
                    "String1: {1}{0}String2: {2}",
                    Environment.NewLine,
                    String1,
                    String2);
            }

            /// <summary>
            /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
            /// </summary>
            /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
            /// <returns><see langword="true"/> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <see langword="false"/>.</returns>
            /// <remarks></remarks>
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj.GetType() == typeof(TestSerializableObject) && Equals((TestSerializableObject)obj);
            }

            /// <summary>
            /// Equalses the specified other.
            /// </summary>
            /// <param name="other">The other.</param>
            /// <returns></returns>
            /// <remarks></remarks>
            public bool Equals(TestSerializableObject other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(other.String1, String1) && Equals(other.String2, String2);
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
            /// <remarks></remarks>
            public override int GetHashCode()
            {
                unchecked
                {
                    return ((String1 != null ? String1.GetHashCode() : 0) * 397) ^
                           (String2 != null ? String2.GetHashCode() : 0);
                }
            }
        }
    }
}