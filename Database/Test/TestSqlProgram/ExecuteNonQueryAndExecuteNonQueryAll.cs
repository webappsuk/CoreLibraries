using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using WebApplications.Testing;
using WebApplications.Utilities.Database.Exceptions;
using WebApplications.Utilities.Logging;

namespace WebApplications.Utilities.Database.Test.TestSqlProgram
{
    public partial class SqlProgramTests
    {

        [TestMethod]
        public async Task ExecuteNonQuery_ExecutesSuccessfully()
        {
            SqlProgram nonQueryTest = await SqlProgram.Create((Connection)LocalDatabaseConnectionString, name: "spNonQuery");
            int nonQueryResult = nonQueryTest.ExecuteNonQuery();
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public async Task  ExecuteNonQueryAll_ExecutesSuccessfully()
        {
            SqlProgram nonQueryTest =
                await SqlProgram.Create(connection: new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString),
                       name: "spNonQuery");
            List<int> nonQueryResult = nonQueryTest.ExecuteNonQueryAll().ToList();
            Assert.AreEqual(2, nonQueryResult.Count);

            foreach (int result in nonQueryResult)
                Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public async Task ExecuteNonQuery_WithParameters_ExecutesSuccessfully()
        {
            SqlProgram<string, int> nonQueryTest = await SqlProgram<string, int>.Create((Connection)LocalDatabaseConnectionString, name: "spNonQuery");

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
                await SqlProgram<IEnumerable<Tuple<int?, string, bool>>>.Create((Connection)DifferentLocalDatabaseConnectionString, "spTakesTupleTable");

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
                await SqlProgram<string, int>.Create(connection: new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString),
                               name: "spNonQuery");

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
                await SqlProgram<string, int>.Create(connection: new LoadBalancedConnection(LocalDatabaseConnectionString, DifferentLocalDatabaseConnectionString),
                               name: "spNonQuery");

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
                await SqlProgram<int>.Create((Connection)DifferentLocalDatabaseConnectionString, "spTimeoutTest",
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
                await SqlProgram<int>.Create((Connection)DifferentLocalDatabaseConnectionString, "spTimeoutTest");
            Task<int> task = timeoutTest.ExecuteNonQueryAsync();
            task.Wait();
            Assert.IsTrue(task.IsCompleted);
        }

        [TestMethod]
        public async Task ExecuteNonQuery_WithEnumerableIntParameter_ExecutesSuccessfully()
        {
            SqlProgram<IEnumerable<int>> tableTypeTest =
                await SqlProgram<IEnumerable<int>>.Create((Connection)DifferentLocalDatabaseConnectionString, "spTakesIntTable");

            int nonQueryResult = tableTypeTest.ExecuteNonQuery(new[] { 1, 2, 3, 4 });
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public async Task ExecuteNonQuery_WithEnumerableKeyValuePairParameter_ExecutesSuccessfully()
        {
            SqlProgram<IEnumerable<KeyValuePair<int, string>>> tableTypeTest =
                await SqlProgram<IEnumerable<KeyValuePair<int, string>>>.Create((Connection)DifferentLocalDatabaseConnectionString, "spTakesKvpTable");

            int nonQueryResult = tableTypeTest.ExecuteNonQuery(
                new Dictionary<int, string>
                    {
                        {1, Random.RandomString(10, false)},
                        {2, Random.RandomString(10, false)},
                        {3, Random.RandomString(10, false)}
                    });
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public async Task ExecuteNonQuery_WithTupleParameter_ExecutesSuccessfully()
        {
            SqlProgram<IEnumerable<Tuple<int, string, bool>>> tableTypeTest =
                await SqlProgram<IEnumerable<Tuple<int, string, bool>>>.Create((Connection)DifferentLocalDatabaseConnectionString, "spTakesTupleTable");

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
                    (Connection)DifferentLocalDatabaseConnectionString, "spTakesTupleTablePlusTwo");

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
                await SqlProgram<string, int?, decimal, bool, DateTime?>.Create((Connection)DifferentLocalDatabaseConnectionString, "spUltimateSproc");

            nullableTypesTest.ExecuteNonQuery(Random.RandomString(20), Random.RandomInt32(), decimal.Zero, true, null);
            Assert.IsNotNull(nullableTypesTest);
        }

        [TestMethod]
        public async Task ExecuteNonQuery_WithMoreNullableDateTimeParameterSetToNull_ExecutesSuccessfully()
        {
            var nullableTypesTest =
                await SqlProgram<int, DateTime?, DateTime?, DateTime?, DateTime?, int?, int?, int?>.Create((Connection)DifferentLocalDatabaseConnectionString, "spLoadsOfNullables");

            nullableTypesTest.ExecuteNonQuery(1, null, null, null, null, null, null, 1);
            Assert.IsNotNull(nullableTypesTest);
        }

        [TestMethod]
        public async Task ExecuteNonQuery_WithKeyValuePairParmeterAndAdditionalNullableTableParametersNotSet_ExecutesSuccessfully()
        {
            SqlProgram<IEnumerable<KeyValuePair<int, string>>> nullableTypesTest =
                await SqlProgram<IEnumerable<KeyValuePair<int, string>>>.Create((Connection)DifferentLocalDatabaseConnectionString, "spTakesTableAdditionalColumns");

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
                await SqlProgram<IEnumerable<Tuple<int, string>>>.Create((Connection)DifferentLocalDatabaseConnectionString, "spTakesTableAdditionalColumns");

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
                await SqlProgram<IEnumerable<Tuple<int, string, DateTime?>>>.Create((Connection)DifferentLocalDatabaseConnectionString, "spTakesTableAdditionalColumns");

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
                await SqlProgram<IEnumerable<Tuple<string, int?, DateTime>>>.Create((Connection)DifferentLocalDatabaseConnectionString, "spTakesMixedNullableColumns");

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
                await SqlProgram<IEnumerable<Tuple<int, string, bool>>>.Create((Connection)DifferentLocalDatabaseConnectionString, "spTakesTupleTable");

            int nonQueryResult =
                tableTypeTest.ExecuteNonQuery(null);
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public async Task ExecuteNonQuery_WithEmptyTupleParameter_ExecutesSuccessfully()
        {
            SqlProgram<IEnumerable<Tuple<int, string, bool>>> tableTypeTest =
                await SqlProgram<IEnumerable<Tuple<int, string, bool>>>.Create((Connection)DifferentLocalDatabaseConnectionString, "spTakesTupleTable");

            int nonQueryResult =
                tableTypeTest.ExecuteNonQuery(Enumerable.Empty<Tuple<int, string, bool>>());
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public async Task ExecuteNonQuery_WithXmlDocumentParameter_ExecutesSuccessfully()
        {
            System.Xml.XmlDocument document = new System.Xml.XmlDocument();
            System.Xml.XmlElement rootNode = document.CreateElement("MyTestRoot");
            System.Xml.XmlElement childNode = document.CreateElement("MyTestChild");
            System.Xml.XmlAttribute anAttribute = document.CreateAttribute("anattribute");

            childNode.InnerText = Random.RandomString(20);
            anAttribute.Value = Random.RandomInt32().ToString();
            rootNode.Attributes.Append(anAttribute);
            document.AppendChild(rootNode);
            rootNode.AppendChild(childNode);

            SqlProgram<System.Xml.XmlDocument> xmlTypeTest =
                await SqlProgram<System.Xml.XmlDocument>.Create((Connection)DifferentLocalDatabaseConnectionString, "spTakesXml");

            int nonQueryResult =
                xmlTypeTest.ExecuteNonQuery(document);
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public async Task ExecuteNonQuery_WithTableParameterWhichTakesXmlDocument_ExecutesSuccessfully()
        {
            System.Xml.XmlDocument document = new System.Xml.XmlDocument();
            System.Xml.XmlElement rootNode = document.CreateElement("MyTestRoot");
            System.Xml.XmlElement childNode = document.CreateElement("MyTestChild");
            System.Xml.XmlAttribute anAttribute = document.CreateAttribute("anattribute");

            childNode.InnerText = Random.RandomString(20);
            anAttribute.Value = Random.RandomInt32().ToString();
            rootNode.Attributes.Append(anAttribute);
            document.AppendChild(rootNode);
            rootNode.AppendChild(childNode);

            SqlProgram<IEnumerable<System.Xml.XmlDocument>> xmlTypeTest =
                await SqlProgram<IEnumerable<System.Xml.XmlDocument>>.Create((Connection)DifferentLocalDatabaseConnectionString, "spTakesTableXml");

            int nonQueryResult =
                xmlTypeTest.ExecuteNonQuery(new List<System.Xml.XmlDocument> { document, document });
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public async Task ExecuteNonQuery_WithXmlElementParameter_ExecutesSuccessfully()
        {
            System.Xml.XmlDocument document = new System.Xml.XmlDocument();
            System.Xml.XmlElement rootNode = document.CreateElement("MyTestRoot");
            System.Xml.XmlElement childNode = document.CreateElement("MyTestChild");
            System.Xml.XmlAttribute anAttribute = document.CreateAttribute("anattribute");

            childNode.InnerText = Random.RandomString(20);
            anAttribute.Value = Random.RandomInt32().ToString();
            rootNode.Attributes.Append(anAttribute);
            document.AppendChild(rootNode);
            rootNode.AppendChild(childNode);

            SqlProgram<System.Xml.XmlElement> xmlTypeTest =
                await SqlProgram<System.Xml.XmlElement>.Create((Connection)DifferentLocalDatabaseConnectionString, "spTakesXml");

            int nonQueryResult =
                xmlTypeTest.ExecuteNonQuery(rootNode);
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public async Task ExecuteNonQuery_WithTableParameterWhichTakesXmlElement_ExecutesSuccessfully()
        {
            System.Xml.XmlDocument document = new System.Xml.XmlDocument();
            System.Xml.XmlElement rootNode = document.CreateElement("MyTestRoot");
            System.Xml.XmlElement childNode = document.CreateElement("MyTestChild");
            System.Xml.XmlAttribute anAttribute = document.CreateAttribute("anattribute");

            childNode.InnerText = Random.RandomString(20);
            anAttribute.Value = Random.RandomInt32().ToString();
            rootNode.Attributes.Append(anAttribute);
            document.AppendChild(rootNode);
            rootNode.AppendChild(childNode);

            SqlProgram<IEnumerable<System.Xml.XmlElement>> xmlTypeTest =
                await SqlProgram<IEnumerable<System.Xml.XmlElement>>.Create((Connection)DifferentLocalDatabaseConnectionString, "spTakesTableXml");

            int nonQueryResult =
                xmlTypeTest.ExecuteNonQuery(new List<System.Xml.XmlElement> { rootNode, rootNode });
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public async Task ExecuteNonQuery_WithXDocumentParameter_ExecutesSuccessfully()
        {
            XDocument document = new XDocument();
            document.Add(new XElement("MyTestRoot", new XAttribute("myattribute", Random.RandomInt32()),
                new XElement("MyTestChild", Random.RandomString(20))));

            SqlProgram<XDocument> xmlTypeTest =
                await SqlProgram<XDocument>.Create((Connection)DifferentLocalDatabaseConnectionString, "spTakesXml");

            int nonQueryResult =
                xmlTypeTest.ExecuteNonQuery(document);
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public async Task ExecuteNonQuery_WithTableParameterWhichTakesXDocument_ExecutesSuccessfully()
        {
            XDocument document = new XDocument();
            document.Add(new XElement("MyTestRoot", new XAttribute("myattribute", Random.RandomInt32()),
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
            XElement element = new XElement("MyTestRoot", new XAttribute("myattribute", Random.RandomInt32()),
                                            new XElement("MyTestChild", Random.RandomString(20)));

            SqlProgram<XElement> xmlTypeTest =
                await SqlProgram<XElement>.Create((Connection)DifferentLocalDatabaseConnectionString, "spTakesXml");

            int nonQueryResult =
                xmlTypeTest.ExecuteNonQuery(element);
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public async Task ExecuteNonQuery_WithXElementParameter_XElement()
        {
            XElement element = new XElement("MyTestRoot", new XAttribute("myattribute", Random.RandomInt32()),
                                            new XElement("MyTestChild", Random.RandomString(20)));

            SqlProgram<IEnumerable<XElement>> xmlTypeTest =
                await SqlProgram<IEnumerable<XElement>>.Create((Connection)DifferentLocalDatabaseConnectionString, "spTakesTableXml");

            int nonQueryResult =
                xmlTypeTest.ExecuteNonQuery(new [] { element, element });
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public async Task ExecuteNonQuery_WithOutputParameters_ExecutesSuccessfully()
        {
            SqlProgram<int, Out<int>, Out<int>> program =
                await SqlProgram<int, Out<int>, Out<int>>.Create((Connection)LocalDatabaseConnectionString, "spOutputParameters");

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
                await SqlProgram<int?, Out<int?>, Out<int>>.Create((Connection)LocalDatabaseConnectionString, "spOutputParameters");
            
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
