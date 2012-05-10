using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;
using WebApplications.Utilities.Database.Configuration;
using WebApplications.Utilities.Database.Exceptions;

namespace WebApplications.Utilities.Database.Test.TestSqlProgram
{
    public partial class SqlProgramTests
    {
        [TestMethod]
        public void ExecuteReader_ExecutesSuccessfully()
        {
            SqlProgram readerTest = new SqlProgram(connectionString: _differentConnectionString,
                                                   name: "spUltimateSproc");

            readerTest.ExecuteReader();
        }

        [TestMethod]
        public void ExecuteReaderAll_ExecutesSuccessfully()
        {

            SqlProgram readerTest =
                new SqlProgram(connection: new LoadBalancedConnection(_localConnectionString, _localCopyConnectionString),
                               name: "spNonQuery");

            readerTest.ExecuteReader();

            // Can't really do any assertions here so test is just that it doesn't throw an exception.
        }

        [TestMethod]
        public void ExecuteReader_WithReturnResultSet_ExecutesSuccessfully()
        {
            SqlProgram readerTest = new SqlProgram(connectionString: _differentConnectionString,
                                                   name: "spUltimateSproc");

            dynamic result = readerTest.ExecuteReader<dynamic>(
                reader =>
                {
                    if (reader.Read())
                    {
                        return new
                        {
                            Name = reader.GetValue<string>(0),
                            Age = reader.GetValue<int>(1),
                            Balance = reader.GetValue<decimal>(2),
                            IsValued = reader.GetValue<bool>(3)
                        };
                    }

                    throw new Exception("Critical Test Error");
                });

            Assert.IsNotNull(result);

            // Read the sproc defaults.
            Assert.AreEqual("A Test String", result.Name);
            Assert.AreEqual(5, result.Age);
            Assert.AreEqual(200.15M, result.Balance);
            Assert.AreEqual(false, result.IsValued);
        }

        [TestMethod]
        public void ExecuteReader_WithAllParametersSet_ExecutesAndReturnsExpectedResult()
        {
            SqlProgram<string, int, decimal, bool> readerTest =
                new SqlProgram<string, int, decimal, bool>(_differentConnectionString, "spUltimateSproc");

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
                    if (reader.Read())
                    {
                        return new
                        {
                            Name = reader.GetValue<string>(0),
                            Age = reader.GetValue<int>(1),
                            Balance = reader.GetValue<decimal>(2),
                            IsValued = reader.GetValue<bool>(3)
                        };
                    }

                    throw new Exception("Critical Test Error");
                });

            Assert.IsNotNull(result);

            Assert.AreEqual(AString, result.Name);
            Assert.AreEqual(AInt, result.Age);
            Assert.AreEqual(ADecimal, result.Balance);
            Assert.AreEqual(ABool, result.IsValued);
        }

        [TestMethod]
        public void ExecuteReader_WithEnumerableIntParameter_ReturnsSingleColumnTableMatchingTheParameterType()
        {
            SqlProgram<IEnumerable<int>> tableTypeTest =
                new SqlProgram<IEnumerable<int>>(_differentConnectionString, "spTakesIntTable");

            IList<int> result = tableTypeTest.ExecuteReader(
                new[] { 0, 1, 2, 3 },
                reader =>
                {
                    List<int> resultSet = new List<int>();
                    while (reader.Read())
                        resultSet.Add(reader.GetValue<int>(0));
                    return resultSet;
                });

            Assert.AreEqual(4, result.Count);
            for (int i = 0; i < result.Count; i++)
                Assert.AreEqual(i, result[i]);
        }

        [TestMethod]
        public void ExecuteReader_WithEnumerableKeyValuePairParameter_ReturnsTwoColumnTableMatchingTheParameterTypes()
        {
            SqlProgram<IEnumerable<KeyValuePair<int, string>>> tableTypeTest =
                new SqlProgram<IEnumerable<KeyValuePair<int, string>>>(_differentConnectionString, "spTakesKvpTable");

            string str1 = Random.RandomString(10, false);
            string str2 = Random.RandomString(10, false);
            string str3 = Random.RandomString(10, false);

            IDictionary<int, string> result = tableTypeTest.ExecuteReader(
                new Dictionary<int, string>
                    {
                        {0, str1},
                        {1, str2},
                        {2, str3}
                    },
                reader =>
                {
                    IDictionary<int, string> resultSet = new Dictionary<int, string>();
                    while (reader.Read())
                        resultSet.Add(reader.GetValue<int>(0), reader.GetValue<string>(1));
                    return resultSet;
                });

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(str1, result[0]);
            Assert.AreEqual(str2, result[1]);
            Assert.AreEqual(str3, result[2]);
        }

        [TestMethod]
        public void ExecuteReader_WithTupleParameter_ExecutesSuccessfully()
        {
            SqlProgram<IEnumerable<Tuple<int, string, bool>>> tableTypeTest =
                new SqlProgram<IEnumerable<Tuple<int, string, bool>>>(_differentConnectionString, "spTakesTupleTable");

            string str1 = Random.RandomString(10, false);
            string str2 = Random.RandomString(10, false);

            IList<dynamic> result =
                tableTypeTest.ExecuteReader(
                    new List<Tuple<int, string, bool>>
                        {
                            new Tuple<int, string, bool>(1, str1, false),
                            new Tuple<int, string, bool>(2, str2, true)
                        },
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
        public void ExecuteReader_WithByteArrayParameter_ReturnsSameArrayInReader()
        {
            SqlProgram<byte[]> byteArrayTest = new SqlProgram<byte[]>(_differentConnectionString, "spTakeByteArrayLength10");

            int length = Random.Next(1, 10);
            byte[] testParam = new byte[length];
            Random.NextBytes(testParam);

            byteArrayTest.ExecuteReader(
                testParam,
                reader =>
                    {
                        if (reader.Read())
                        {
                            CollectionAssert.AreEqual(
                                testParam,
                                (ICollection) reader.GetValue(0));
                        }
                    });
        }

        [TestMethod]
        [ExpectedException(typeof(DatabaseSchemaException))]
        public void ExecuteReader_WithByteArrayParameterTooLong_ThrowsExceptionWhenTypeConstraintModeError()
        {
            // This Sql Program is configured in the app.config to use TypeConstraintMode Error, so will throw error
            // if byte[] is truncated.
            SqlProgram<byte[]> byteArrayTest = DatabasesConfiguration.GetConfiguredSqlProgram<byte[]>("test",
                                                                                                      "spTakeByteArrayLength10",
                                                                                                      "@byteArrayParam");

            byte[] testParam = new byte[11];
            Random.NextBytes(testParam);

            byteArrayTest.ExecuteReader(
                testParam,
                reader =>
                {
                    if (reader.Read())
                    {
                        CollectionAssert.AreEqual(
                            testParam,
                            (ICollection)reader.GetValue(0));
                    }
                });
        }

        [TestMethod]
        public void ExecuteReader_WithSerializableObjectParameter_ReturnsByteArray()
        {
            SqlProgram<TestSerializableObject> serializeObjectTest = new SqlProgram<TestSerializableObject>(_differentConnectionString, "spTakeByteArray");

            TestSerializableObject objecToSerialize = new TestSerializableObject { String1 = Random.RandomString(), String2 = Random.RandomString() };
            serializeObjectTest.ExecuteReader(
                objecToSerialize,
                reader =>
                    {
                        Assert.IsTrue(reader.Read());
                        Assert.IsInstanceOfType(reader.GetValue(0), typeof (byte[]));

                        // Deserialize object
                        TestSerializableObject deserializedObject =
                            (TestSerializableObject) reader.GetObjectByName(reader.GetName(0));

                        // Check we don't have same object instance.
                        Assert.IsFalse(ReferenceEquals(objecToSerialize, deserializedObject));

                        // Check equality of object instances using equality method.
                        Assert.AreEqual(objecToSerialize, deserializedObject);
                    });
        }

        [TestMethod]
        public void ExecuteReader_WithNestedTupleParameter_ExecutesSuccessfully()
        {
            SqlProgram<IEnumerable<Tuple<int, string, bool, bool, decimal, decimal, double, Tuple<string, short, TestSerializableObject, byte, DateTime, DateTime, XElement, Tuple<int, long, int, int>>>>> tupleTableTypeTest =
                new SqlProgram<IEnumerable<Tuple<int, string, bool, bool, decimal, decimal, double, Tuple<string, short, TestSerializableObject, byte, DateTime, DateTime, XElement, Tuple<int, long, int, int>>>>>(_differentConnectionString, "spTakesMultiTupleTable");

            var rows =
                new List
                    <
                        Tuple
                            <int, string, bool, bool, decimal, decimal, double,
                                Tuple
                                    <string, short, TestSerializableObject, byte, DateTime, DateTime, XElement,
                                        Tuple<int, long, int, int>>>>();

            for (int i = 0; i < Random.Next(3, 10); i++)
            {
                rows.Add(ExtendedTuple.Create(
                    Random.RandomInt32(),
                    Random.RandomString(),
                    false,
                    true,
                    Random.RandomDecimal(),
                    Random.RandomDecimal(),
                    Random.RandomDouble(),
                    Random.RandomString(),
                    Random.RandomInt16(),
                    new TestSerializableObject { String1 = Random.RandomString(), String2 = Random.RandomString() },
                    Random.RandomByte(),
                    Random.RandomDateTime(),
                    Random.RandomDateTime(),
                    new XElement("Test", new XAttribute("attribute", Random.Next())),
                    Random.RandomInt32(),
                    Random.RandomInt64(),
                    Random.RandomInt32(),
                    Random.RandomInt32()));
            }

            var indexer =
                typeof(
                    Tuple
                        <int, string, bool, bool, decimal, decimal, double,
                            Tuple<string, short, TestSerializableObject, byte, DateTime, DateTime, XElement, Tuple<int, long, int, int>>
                            >
                    ).GetTupleIndexer();

            tupleTableTypeTest.ExecuteReader(rows,
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
                                                             Assert.IsNotNull(resultCollection,
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
                                             });
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
                return String.Format("String1: {1}{0}String2: {2}",
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
                    return ((String1 != null ? String1.GetHashCode() : 0)*397) ^ (String2 != null ? String2.GetHashCode() : 0);
                }
            }
        }
    }
}
