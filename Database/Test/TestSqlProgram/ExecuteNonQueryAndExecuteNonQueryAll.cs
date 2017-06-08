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
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using WebApplications.Testing;
using WebApplications.Utilities.Database.Exceptions;
using WebApplications.Utilities.Logging;

// ReSharper disable ConsiderUsingConfigureAwait
// ReSharper disable InconsistentNaming
#pragma warning disable 1591 // Missing XML commend
#pragma warning disable 0618 // Type or member is obsolete

namespace WebApplications.Utilities.Database.Test
{
    public partial class SqlProgramTests
    {
        [TestMethod]
        public async Task ExecuteNonQuery_ExecutesSuccessfully()
        {
            SqlProgram nonQueryTest = await SqlProgram.Create(LocalDatabaseConnection, "spNonQuery");
            int nonQueryResult = nonQueryTest.ExecuteNonQuery();
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public async Task ExecuteNonQueryAll_ExecutesSuccessfully()
        {
            SqlProgram nonQueryTest =
                await SqlProgram.Create(
                    new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString),
                    "spNonQuery");
            List<int> nonQueryResult = nonQueryTest.ExecuteNonQueryAll().ToList();
            Assert.AreEqual(2, nonQueryResult.Count);

            foreach (int result in nonQueryResult)
                Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public async Task ExecuteNonQuery_WithParameters_ExecutesSuccessfully()
        {
            SqlProgram<string, int> nonQueryTest =
                await SqlProgram<string, int>.Create(LocalDatabaseConnection, "spNonQuery");

            string randomString = Random.RandomString(20);
            int randomInt = Random.RandomInt32();

            int nonQueryResult = nonQueryTest.ExecuteNonQuery(
                c =>
                {
                    c.SetParameter("@stringParam", randomString);
                    c.SetParameter("@intParam", randomInt);
                });
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public async Task ExecuteNonQuery_WithNullableType_ExecutesSuccessfully()
        {
            SqlProgram<IEnumerable<Tuple<int?, string, bool>>> tableTypeTest =
                await SqlProgram<IEnumerable<Tuple<int?, string, bool>>>.Create(
                    DifferentLocalDatabaseConnection,
                    "spTakesTupleTable");

            int nonQueryResult =
                tableTypeTest.ExecuteNonQuery(
                    new List<Tuple<int?, string, bool>>
                    {
                        new Tuple<int?, string, bool>(null, Random.RandomString(), false),
                        new Tuple<int?, string, bool>(2, Random.RandomString(10), true)
                    });
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public async Task ExecuteNonQueryAll_WithParameters_ExecutesSuccessfully()
        {
            SqlProgram<string, int> nonQueryTest =
                await SqlProgram<string, int>.Create(
                    new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString),
                    "spNonQuery");

            string randomString = Random.RandomString(20);
            int randomInt = Random.RandomInt32();

            List<int> nonQueryResult = nonQueryTest.ExecuteNonQueryAll(
                c =>
                {
                    c.SetParameter("@stringParam", randomString);
                    c.SetParameter("@intParam", randomInt);
                }).ToList();
            Assert.AreEqual(2, nonQueryResult.Count);

            foreach (int result in nonQueryResult)
                Assert.AreEqual(-1, result);
        }

        [TestMethod]
        [ExpectedException(typeof(LoggingException))]
        public async Task ExecuteNonQueryAll_WithUnknownProgramForConnection_ThrowsLoggingException()
        {
            SqlProgram<string, int> nonQueryTest =
                await SqlProgram<string, int>.Create(
                    new LoadBalancedConnection(LocalDatabaseConnectionString, DifferentLocalDatabaseConnectionString),
                    "spNonQuery");

            string randomString = Random.RandomString();
            int randomInt = Random.RandomInt32();

            nonQueryTest.ExecuteNonQueryAll(
                c =>
                {
                    c.SetParameter("@stringParam", randomString);
                    c.SetParameter("@intParam", randomInt);
                });
        }

        [TestMethod]
        [ExpectedException(typeof(SqlProgramExecutionException), AllowDerivedTypes = true)]
        public async Task ExecuteNonQuery_WithTimeoutSet_ThrowsSqlProgramExecutionExceptionOnTimeout()
        {
            SqlProgram<int> timeoutTest =
                await SqlProgram<int>.Create(
                    DifferentLocalDatabaseConnection,
                    "spTimeoutTest",
                    defaultCommandTimeout: Duration.FromSeconds(5));
            DateTime timeStarted = DateTime.Now;
            timeoutTest.ExecuteNonQuery(10);
            DateTime timeEnded = DateTime.Now;
            Assert.AreEqual(10, (int)(timeEnded - timeStarted).TotalSeconds);
        }

        [TestMethod]
        public async Task ExecuteNonQueryAsync_ExecutesProcedureSuccessfully()
        {
            SqlProgram<int> timeoutTest =
                await SqlProgram<int>.Create(DifferentLocalDatabaseConnection, "spTimeoutTest");
            Task<int> task = timeoutTest.ExecuteNonQueryAsync();
            task.Wait();
            Assert.IsTrue(task.IsCompleted);
        }

        [TestMethod]
        public async Task ExecuteNonQuery_WithEnumerableIntParameter_ExecutesSuccessfully()
        {
            SqlProgram<IEnumerable<int>> tableTypeTest =
                await SqlProgram<IEnumerable<int>>.Create(DifferentLocalDatabaseConnection, "spTakesIntTable");

            int nonQueryResult = tableTypeTest.ExecuteNonQuery(new[] { 1, 2, 3, 4 });
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public async Task ExecuteNonQuery_WithEnumerableKeyValuePairParameter_ExecutesSuccessfully()
        {
            SqlProgram<IEnumerable<KeyValuePair<int, string>>> tableTypeTest =
                await SqlProgram<IEnumerable<KeyValuePair<int, string>>>.Create(
                    DifferentLocalDatabaseConnection,
                    "spTakesKvpTable");

            int nonQueryResult = tableTypeTest.ExecuteNonQuery(
                new Dictionary<int, string>
                {
                    { 1, Random.RandomString(10, false) },
                    { 2, Random.RandomString(10, false) },
                    { 3, Random.RandomString(10, false) }
                });
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public async Task ExecuteNonQuery_WithTupleParameter_ExecutesSuccessfully()
        {
            SqlProgram<IEnumerable<Tuple<int, string, bool>>> tableTypeTest =
                await SqlProgram<IEnumerable<Tuple<int, string, bool>>>.Create(
                    DifferentLocalDatabaseConnection,
                    "spTakesTupleTable");

            int nonQueryResult =
                tableTypeTest.ExecuteNonQuery(
                    new List<Tuple<int, string, bool>>
                    {
                        new Tuple<int, string, bool>(1, Random.RandomString(10), false),
                        new Tuple<int, string, bool>(2, Random.RandomString(10), true)
                    });
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public async Task ExecuteNonQuery_WithTupleParameterAndOtherParameters_ExecutesSuccessfully()
        {
            SqlProgram<IEnumerable<Tuple<string, string, string, DateTime, short?>>, int, DateTime> tableTypeTest =
                await SqlProgram<IEnumerable<Tuple<string, string, string, DateTime, short?>>, int, DateTime>.Create(
                    DifferentLocalDatabaseConnection,
                    "spTakesTupleTablePlusTwo");

            List<TestType> myList =
                new List<TestType>
                {
                    new TestType
                    {
                        Name = Random.RandomString(100),
                        Body = Random.RandomString(500),
                        Email = Random.RandomString(100),
                        Created = DateTime.Today,
                        LoginId = null
                    }
                };

            int nonQueryResult =
                tableTypeTest.ExecuteNonQuery(
                    myList.ToTuple(e => e.Name, e => e.Body, e => e.Email, e => e.Created, e => e.LoginId),
                    Random.RandomInt32());

            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public async Task ExecuteNonQuery_WithNullableDateTimeParameterSetToNull_ExecutesSuccessfully()
        {
            SqlProgram<string, int?, decimal, bool, DateTime?> nullableTypesTest =
                await SqlProgram<string, int?, decimal, bool, DateTime?>.Create(
                    DifferentLocalDatabaseConnection,
                    "spUltimateSproc");

            nullableTypesTest.ExecuteNonQuery(Random.RandomString(20), Random.RandomInt32(), decimal.Zero, true, null);
            Assert.IsNotNull(nullableTypesTest);
        }

        [TestMethod]
        public async Task ExecuteNonQuery_WithMoreNullableDateTimeParameterSetToNull_ExecutesSuccessfully()
        {
            var nullableTypesTest =
                await SqlProgram<int, DateTime?, DateTime?, DateTime?, DateTime?, int?, int?, int?>.Create(
                    DifferentLocalDatabaseConnection,
                    "spLoadsOfNullables");

            nullableTypesTest.ExecuteNonQuery(1, null, null, null, null, null, null, 1);
            Assert.IsNotNull(nullableTypesTest);
        }

        [TestMethod]
        public async Task ExecuteNonQuery_WithKeyValuePairParmeterAndAdditionalNullableTableParametersNotSet_ExecutesSuccessfully()
        {
            SqlProgram<IEnumerable<KeyValuePair<int, string>>> nullableTypesTest =
                await SqlProgram<IEnumerable<KeyValuePair<int, string>>>.Create(
                    DifferentLocalDatabaseConnection,
                    "spTakesTableAdditionalColumns");

            nullableTypesTest.ExecuteNonQuery(
                new Dictionary<int, string>
                {
                    { Random.RandomInt32(), Random.RandomString(50) },
                    { Random.RandomInt32(), Random.RandomString(50) },
                    { Random.RandomInt32(), Random.RandomString(50) }
                });
            Assert.IsNotNull(nullableTypesTest);
        }

        [TestMethod]
        public async Task ExecuteNonQuery_WithTupleParmeterAndAdditionalNullableTableParametersNotSet_ExecutesSuccessfully()
        {
            SqlProgram<IEnumerable<Tuple<int, string>>> nullableTypesTest =
                await SqlProgram<IEnumerable<Tuple<int, string>>>.Create(
                    DifferentLocalDatabaseConnection,
                    "spTakesTableAdditionalColumns");

            nullableTypesTest.ExecuteNonQuery(
                new List<Tuple<int, string>>
                {
                    new Tuple<int, string>(Random.RandomInt32(), Random.RandomString(50)),
                    new Tuple<int, string>(Random.RandomInt32(), Random.RandomString(50)),
                    new Tuple<int, string>(Random.RandomInt32(), Random.RandomString(50)),
                    new Tuple<int, string>(Random.RandomInt32(), Random.RandomString(50))
                });
            Assert.IsNotNull(nullableTypesTest);
        }

        [TestMethod]
        public async Task ExecuteNonQuery_WithTupleParmeterAndSomeAdditionalNullableTableParametersSet_ExecutesSuccessfully()
        {
            SqlProgram<IEnumerable<Tuple<int, string, DateTime?>>> nullableTypesTest =
                await SqlProgram<IEnumerable<Tuple<int, string, DateTime?>>>.Create(
                    DifferentLocalDatabaseConnection,
                    "spTakesTableAdditionalColumns");

            nullableTypesTest.ExecuteNonQuery(
                new List<Tuple<int, string, DateTime?>>
                {
                    new Tuple<int, string, DateTime?>(Random.RandomInt32(), Random.RandomString(50), DateTime.Today),
                    new Tuple<int, string, DateTime?>(Random.RandomInt32(), Random.RandomString(50), DateTime.Today),
                    new Tuple<int, string, DateTime?>(Random.RandomInt32(), Random.RandomString(50), DateTime.Today)
                });
            Assert.IsNotNull(nullableTypesTest);
        }

        [TestMethod]
        public async Task ExecuteNonQuery_WithTupleParmeterOfMixedNullableColumns_ExecutesSuccessfully()
        {
            SqlProgram<IEnumerable<Tuple<string, int?, DateTime>>> nullableTypesTest =
                await SqlProgram<IEnumerable<Tuple<string, int?, DateTime>>>.Create(
                    DifferentLocalDatabaseConnection,
                    "spTakesMixedNullableColumns");

            nullableTypesTest.ExecuteNonQuery(
                new List<Tuple<string, int?, DateTime>>
                {
                    new Tuple<string, int?, DateTime>(Random.RandomString(50), null, DateTime.Today),
                    new Tuple<string, int?, DateTime>(Random.RandomString(50), null, DateTime.Today),
                    new Tuple<string, int?, DateTime>(Random.RandomString(50), null, DateTime.Today)
                });
            Assert.IsNotNull(nullableTypesTest);
        }

        [TestMethod]
        public async Task ExecuteNonQuery_WithTupleParameterSetToNull_ExecutesSuccessfully()
        {
            SqlProgram<IEnumerable<Tuple<int, string, bool>>> tableTypeTest =
                await SqlProgram<IEnumerable<Tuple<int, string, bool>>>.Create(
                    DifferentLocalDatabaseConnection,
                    "spTakesTupleTable");

            int nonQueryResult =
                tableTypeTest.ExecuteNonQuery(null);
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public async Task ExecuteNonQuery_WithEmptyTupleParameter_ExecutesSuccessfully()
        {
            SqlProgram<IEnumerable<Tuple<int, string, bool>>> tableTypeTest =
                await SqlProgram<IEnumerable<Tuple<int, string, bool>>>.Create(
                    DifferentLocalDatabaseConnection,
                    "spTakesTupleTable");

            int nonQueryResult =
                tableTypeTest.ExecuteNonQuery(Enumerable.Empty<Tuple<int, string, bool>>());
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public async Task ExecuteNonQuery_WithXmlDocumentParameter_ExecutesSuccessfully()
        {
            XmlDocument document = new XmlDocument();
            XmlElement rootNode = document.CreateElement("MyTestRoot");
            XmlElement childNode = document.CreateElement("MyTestChild");
            XmlAttribute anAttribute = document.CreateAttribute("anattribute");

            childNode.InnerText = Random.RandomString(20);
            anAttribute.Value = Random.RandomInt32().ToString();
            rootNode.Attributes.Append(anAttribute);
            document.AppendChild(rootNode);
            rootNode.AppendChild(childNode);

            SqlProgram<XmlDocument> xmlTypeTest =
                await SqlProgram<XmlDocument>.Create(DifferentLocalDatabaseConnection, "spTakesXml");

            int nonQueryResult =
                xmlTypeTest.ExecuteNonQuery(document);
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public async Task ExecuteNonQuery_WithTableParameterWhichTakesXmlDocument_ExecutesSuccessfully()
        {
            XmlDocument document = new XmlDocument();
            XmlElement rootNode = document.CreateElement("MyTestRoot");
            XmlElement childNode = document.CreateElement("MyTestChild");
            XmlAttribute anAttribute = document.CreateAttribute("anattribute");

            childNode.InnerText = Random.RandomString(20);
            anAttribute.Value = Random.RandomInt32().ToString();
            rootNode.Attributes.Append(anAttribute);
            document.AppendChild(rootNode);
            rootNode.AppendChild(childNode);

            SqlProgram<IEnumerable<XmlDocument>> xmlTypeTest =
                await SqlProgram<IEnumerable<XmlDocument>>.Create(DifferentLocalDatabaseConnection, "spTakesTableXml");

            int nonQueryResult =
                xmlTypeTest.ExecuteNonQuery(new List<XmlDocument> { document, document });
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public async Task ExecuteNonQuery_WithXmlElementParameter_ExecutesSuccessfully()
        {
            XmlDocument document = new XmlDocument();
            XmlElement rootNode = document.CreateElement("MyTestRoot");
            XmlElement childNode = document.CreateElement("MyTestChild");
            XmlAttribute anAttribute = document.CreateAttribute("anattribute");

            childNode.InnerText = Random.RandomString(20);
            anAttribute.Value = Random.RandomInt32().ToString();
            rootNode.Attributes.Append(anAttribute);
            document.AppendChild(rootNode);
            rootNode.AppendChild(childNode);

            SqlProgram<XmlElement> xmlTypeTest =
                await SqlProgram<XmlElement>.Create(DifferentLocalDatabaseConnection, "spTakesXml");

            int nonQueryResult =
                xmlTypeTest.ExecuteNonQuery(rootNode);
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public async Task ExecuteNonQuery_WithTableParameterWhichTakesXmlElement_ExecutesSuccessfully()
        {
            XmlDocument document = new XmlDocument();
            XmlElement rootNode = document.CreateElement("MyTestRoot");
            XmlElement childNode = document.CreateElement("MyTestChild");
            XmlAttribute anAttribute = document.CreateAttribute("anattribute");

            childNode.InnerText = Random.RandomString(20);
            anAttribute.Value = Random.RandomInt32().ToString();
            rootNode.Attributes.Append(anAttribute);
            document.AppendChild(rootNode);
            rootNode.AppendChild(childNode);

            SqlProgram<IEnumerable<XmlElement>> xmlTypeTest =
                await SqlProgram<IEnumerable<XmlElement>>.Create(DifferentLocalDatabaseConnection, "spTakesTableXml");

            int nonQueryResult =
                xmlTypeTest.ExecuteNonQuery(new List<XmlElement> { rootNode, rootNode });
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public async Task ExecuteNonQuery_WithXDocumentParameter_ExecutesSuccessfully()
        {
            XDocument document = new XDocument();
            document.Add(
                new XElement(
                    "MyTestRoot",
                    new XAttribute("myattribute", Random.RandomInt32()),
                    new XElement("MyTestChild", Random.RandomString(20))));

            SqlProgram<XDocument> xmlTypeTest =
                await SqlProgram<XDocument>.Create(DifferentLocalDatabaseConnection, "spTakesXml");

            int nonQueryResult =
                xmlTypeTest.ExecuteNonQuery(document);
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public async Task ExecuteNonQuery_WithTableParameterWhichTakesXDocument_ExecutesSuccessfully()
        {
            XDocument document = new XDocument();
            document.Add(
                new XElement(
                    "MyTestRoot",
                    new XAttribute("myattribute", Random.RandomInt32()),
                    new XElement("MyTestChild", Guid.NewGuid())));

            string connectionString = CreateConnectionString("DifferentLocalData");
            SqlProgram<IEnumerable<XDocument>> xmlTypeTest =
                await SqlProgram<IEnumerable<XDocument>>.Create((Connection)connectionString, "spTakesTableXml");

            int nonQueryResult =
                xmlTypeTest.ExecuteNonQuery(new List<XDocument> { document, document });
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public async Task ExecuteNonQuery_WithXElementParameter_ExecutesSuccessfully()
        {
            XElement element = new XElement(
                "MyTestRoot",
                new XAttribute("myattribute", Random.RandomInt32()),
                new XElement("MyTestChild", Random.RandomString(20)));

            SqlProgram<XElement> xmlTypeTest =
                await SqlProgram<XElement>.Create(DifferentLocalDatabaseConnection, "spTakesXml");

            int nonQueryResult =
                xmlTypeTest.ExecuteNonQuery(element);
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public async Task ExecuteNonQuery_WithXElementParameter_XElement()
        {
            XElement element = new XElement(
                "MyTestRoot",
                new XAttribute("myattribute", Random.RandomInt32()),
                new XElement("MyTestChild", Random.RandomString(20)));

            SqlProgram<IEnumerable<XElement>> xmlTypeTest =
                await SqlProgram<IEnumerable<XElement>>.Create(DifferentLocalDatabaseConnection, "spTakesTableXml");

            int nonQueryResult =
                xmlTypeTest.ExecuteNonQuery(new[] { element, element });
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public async Task ExecuteNonQuery_WithOutputParameters_ExecutesSuccessfully()
        {
            SqlProgram<int, Out<int>, Out<int>> program =
                await SqlProgram<int, Out<int>, Out<int>>.Create(LocalDatabaseConnection, "spOutputParameters");

            const int inputVal = 123;
            const int inputOutputVal = 321;

            Out<int> inputOutput = new Out<int>(inputOutputVal);
            Out<int> output = new Out<int>();

            int nonQueryResult = program.ExecuteNonQuery(inputVal, inputOutput, output);
            Assert.AreEqual(-1, nonQueryResult);

            Assert.IsNull(inputOutput.OutputError, inputOutput.OutputError?.Message);
            Assert.IsNull(output.OutputError, output.OutputError?.Message);

            Assert.AreEqual(inputOutputVal * 2, inputOutput.OutputValue.Value);
            Assert.AreEqual(inputVal, output.OutputValue.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public async Task ExecuteNonQuery_WithOutputParametersNull_ThrowsException()
        {
            SqlProgram<int?, Out<int?>, Out<int>> program =
                await SqlProgram<int?, Out<int?>, Out<int>>.Create(LocalDatabaseConnection, "spOutputParameters");

            Out<int?> inputOutput = new Out<int?>(null);
            Out<int> output = new Out<int>();

            int nonQueryResult = program.ExecuteNonQuery(null, null, output);
            Assert.AreEqual(-1, nonQueryResult);

            Assert.IsNull(inputOutput.OutputError, inputOutput.OutputError?.Message);
            Assert.IsInstanceOfType(output.OutputError, typeof(InvalidCastException));

            Assert.AreEqual(null, inputOutput.OutputValue.Value);
            Assert.AreEqual(null, output.OutputValue.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task ExecuteNonQueryAll_WithOutputParametersAndOut_ThrowsArgumentException()
        {
            SqlProgram<int, Out<int>, Out<int>> program =
                await SqlProgram<int, Out<int>, Out<int>>.Create(
                    new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString),
                    "spOutputParameters");

            const int inputVal = 123;
            const int inputOutputVal = 321;

            Out<int> inputOutput = new Out<int>(inputOutputVal);
            Out<int> output = new Out<int>();

            program.ExecuteNonQueryAll(inputVal, inputOutput, output);
        }

        [TestMethod]
        public async Task ExecuteNonQueryAll_WithOutputParametersAndMultiOut_ExecutesSuccessfully()
        {
            SqlProgram<int, Out<int>, Out<int>> program =
                await SqlProgram<int, Out<int>, Out<int>>.Create(
                    new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString),
                    "spOutputParameters");

            const int inputVal = 123;
            const int inputOutputVal = 321;

            MultiOut<int> inputOutput = new MultiOut<int>(inputOutputVal);
            MultiOut<int> output = new MultiOut<int>();

            int[] nonQueryResult = program.ExecuteNonQueryAll(inputVal, inputOutput, output).ToArray();
            Assert.AreEqual(2, nonQueryResult.Length);
            Assert.IsTrue(nonQueryResult.All(i => i == -1));

            Assert.IsNull(inputOutput.OutputError, inputOutput.OutputError?.Message);
            Assert.IsNull(output.OutputError, output.OutputError?.Message);

            Assert.AreEqual(inputOutputVal * 2, inputOutput.OutputValue.Value);
            Assert.AreEqual(inputVal, output.OutputValue.Value);

            Assert.IsTrue(inputOutput.All(o => o.OutputValue.Value == inputOutputVal * 2));
            Assert.IsTrue(output.All(o => o.OutputValue.Value == inputVal));
        }

        private class TestType
        {
            public string Name { get; set; }
            public string Body { get; set; }
            public string Email { get; set; }
            public DateTime Created { get; set; }
            public short? LoginId { get; set; }
        }
    }
}