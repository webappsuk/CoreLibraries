using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Database.Exceptions;
using WebApplications.Utilities.Logging;

namespace WebApplications.Utilities.Database.Test.TestSqlProgram
{
    public partial class SqlProgramTests
    {
        [TestMethod]
        public void ExecuteNonQuery_ExecutesSuccessfully()
        {
            string connectionString = CreateConnectionString("LocalData");
            SqlProgram nonQueryTest = new SqlProgram(connectionString: connectionString, name: "spNonQuery");
            int nonQueryResult = nonQueryTest.ExecuteNonQuery();
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public void ExecuteNonQueryAll_ExecutesSuccessfully()
        {
            string localDataConnString = CreateConnectionString("LocalData");
            string localDataCopyConnString = CreateConnectionString("LocalDataCopy");

            SqlProgram nonQueryTest =
                new SqlProgram(connection: new LoadBalancedConnection(localDataConnString, localDataCopyConnString),
                               name: "spNonQuery");
            List<int> nonQueryResult = nonQueryTest.ExecuteNonQueryAll().ToList();
            Assert.AreEqual(2, nonQueryResult.Count);

            foreach (int result in nonQueryResult)
                Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void ExecuteNonQuery_WithParameters_ExecutesSuccessfully()
        {
            string connectionString = CreateConnectionString("LocalData");
            SqlProgram<string, int> nonQueryTest = new SqlProgram<string, int>(connectionString: connectionString, name: "spNonQuery");

            string randomString = GenerateRandomString(20);
            int randomInt = Random.Next();

            int nonQueryResult = nonQueryTest.ExecuteNonQuery(
                c =>
                {
                    c.SetParameter("@stringParam", randomString);
                    c.SetParameter("@intParam", randomInt);
                });
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public void ExecuteNonQuery_WithNullableType_ExecutesSuccessfully()
        {
            string connectionString = CreateConnectionString("DifferentLocalData", true);
            SqlProgram<IEnumerable<Tuple<int?, string, bool>>> tableTypeTest =
                new SqlProgram<IEnumerable<Tuple<int?, string, bool>>>(connectionString, "spTakesTupleTable");

            int nonQueryResult =
                tableTypeTest.ExecuteNonQuery(
                    new List<Tuple<int?, string, bool>>
                        {
                            new Tuple<int?, string, bool>(null, GenerateRandomString(10), false),
                            new Tuple<int?, string, bool>(2, GenerateRandomString(10), true)
                        });
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public void ExecuteNonQueryAll_WithParameters_ExecutesSuccessfully()
        {
            string localDataConnString = CreateConnectionString("LocalData");
            string localDataCopyConnString = CreateConnectionString("LocalDataCopy");

            SqlProgram<string, int> nonQueryTest =
                new SqlProgram<string, int>(connection: new LoadBalancedConnection(localDataConnString, localDataCopyConnString),
                               name: "spNonQuery");

            string randomString = GenerateRandomString(20);
            int randomInt = Random.Next();

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
        public void ExecuteNonQueryAll_WithUnknownProgramForConnection_ThrowsLoggingException()
        {
            string localDataConnString = CreateConnectionString("LocalData");
            string localDataCopyConnString = CreateConnectionString("DifferentLocalData");

            SqlProgram<string, int> nonQueryTest =
                new SqlProgram<string, int>(connection: new LoadBalancedConnection(localDataConnString, localDataCopyConnString),
                               name: "spNonQuery");

            string randomString = GenerateRandomString();
            int randomInt = Random.Next();

            nonQueryTest.ExecuteNonQueryAll(
                c =>
                {
                    c.SetParameter("@stringParam", randomString);
                    c.SetParameter("@intParam", randomInt);
                });
        }

        [TestMethod]
        [ExpectedException(typeof(SqlProgramExecutionException))]
        public void ExecuteNonQuery_WithTimeoutSet_ThrowsSqlProgramExecutionExceptionOnTimeout()
        {
            string connectionString = CreateConnectionString("DifferentLocalData");
            SqlProgram<int> timeoutTest =
                new SqlProgram<int>(connectionString, "spTimeoutTest",
                                    defaultCommandTimeout: new TimeSpan(0, 0, 5));
            DateTime timeStarted = DateTime.Now;
            timeoutTest.ExecuteNonQuery(10);
            DateTime timeEnded = DateTime.Now;
            Assert.AreEqual(10, (int)(timeEnded - timeStarted).TotalSeconds);
        }

        [TestMethod]
        public void ExecuteNonQueryAsync_ExecutesProcedureSuccessfully()
        {
            string connectionString = CreateConnectionString("DifferentLocalData", true);
            SqlProgram<int> timeoutTest =
                new SqlProgram<int>(connectionString, "spTimeoutTest");
            Task<int> task = timeoutTest.ExecuteNonQueryAsync();
            task.Wait();
            Assert.IsTrue(task.IsCompleted);
        }

        [TestMethod]
        public void ExecuteNonQuery_WithEnumerableIntParameter_ExecutesSuccessfully()
        {
            string connectionString = CreateConnectionString("DifferentLocalData", true);
            SqlProgram<IEnumerable<int>> tableTypeTest =
                new SqlProgram<IEnumerable<int>>(connectionString, "spTakesIntTable");

            int nonQueryResult = tableTypeTest.ExecuteNonQuery(new[] { 1, 2, 3, 4 });
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public void ExecuteNonQuery_WithEnumerableKeyValuePairParameter_ExecutesSuccessfully()
        {
            string connectionString = CreateConnectionString("DifferentLocalData", true);
            SqlProgram<IEnumerable<KeyValuePair<int, string>>> tableTypeTest =
                new SqlProgram<IEnumerable<KeyValuePair<int, string>>>(connectionString, "spTakesKvpTable");

            int nonQueryResult = tableTypeTest.ExecuteNonQuery(
                new Dictionary<int, string>
                    {
                        {1, GenerateRandomString(10, false)},
                        {2, GenerateRandomString(10, false)},
                        {3, GenerateRandomString(10, false)}
                    });
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public void ExecuteNonQuery_WithTupleParameter_ExecutesSuccessfully()
        {
            string connectionString = CreateConnectionString("DifferentLocalData", true);
            SqlProgram<IEnumerable<Tuple<int, string, bool>>> tableTypeTest =
                new SqlProgram<IEnumerable<Tuple<int, string, bool>>>(connectionString, "spTakesTupleTable");

            int nonQueryResult =
                tableTypeTest.ExecuteNonQuery(
                    new List<Tuple<int, string, bool>>
                        {
                            new Tuple<int, string, bool>(1, GenerateRandomString(10), false),
                            new Tuple<int, string, bool>(2, GenerateRandomString(10), true)
                        });
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public void ExecuteNonQuery_WithTupleParameterAndOtherParameters_ExecutesSuccessfully()
        {
            string connectionString = CreateConnectionString("DifferentLocalData", true);
            SqlProgram<IEnumerable<Tuple<string, string, string, DateTime, short?>>, int, DateTime> tableTypeTest =
                new SqlProgram<IEnumerable<Tuple<string, string, string, DateTime, short?>>, int, DateTime>(
                    connectionString, "spTakesTupleTablePlusTwo");

            List<TestType> myList =
                new List<TestType>
                    {
                        new TestType
                            {
                                Name = GenerateRandomString(100),
                                Body = GenerateRandomString(500),
                                Email = GenerateRandomString(100),
                                Created = DateTime.Today,
                                LoginId = null
                            }
                    };

            int nonQueryResult =
                tableTypeTest.ExecuteNonQuery(
                        myList.ToTuple(e => e.Name, e => e.Body, e => e.Email, e => e.Created, e => e.LoginId),
                    Random.Next());

            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public void ExecuteNonQuery_WithNullableDateTimeParameterSetToNull_ExecutesSuccessfully()
        {
            string connectionString = CreateConnectionString("DifferentLocalData", true);
            SqlProgram<string, int?, decimal, bool, DateTime?> nullableTypesTest =
                new SqlProgram<string, int?, decimal, bool, DateTime?>(connectionString, "spUltimateSproc");

            nullableTypesTest.ExecuteNonQuery(GenerateRandomString(20), Random.Next(), decimal.Zero, true, null);
            Assert.IsNotNull(nullableTypesTest);
        }

        [TestMethod]
        public void ExecuteNonQuery_WithMoreNullableDateTimeParameterSetToNull_ExecutesSuccessfully()
        {
            string connectionString = CreateConnectionString("DifferentLocalData", true);
            var nullableTypesTest =
                new SqlProgram<int, DateTime?, DateTime?, DateTime?, DateTime?, int?, int?, int?>(connectionString, "spLoadsOfNullables");

            nullableTypesTest.ExecuteNonQuery(1, null, null, null, null, null, null, 1);
            Assert.IsNotNull(nullableTypesTest);
        }

        [TestMethod]
        public void ExecuteNonQuery_WithKeyValuePairParmeterAndAdditionalNullableTableParametersNotSet_ExecutesSuccessfully()
        {
            string connectionString = CreateConnectionString("DifferentLocalData", true);
            SqlProgram<IEnumerable<KeyValuePair<int, string>>> nullableTypesTest =
                new SqlProgram<IEnumerable<KeyValuePair<int, string>>>(connectionString, "spTakesTableAdditionalColumns");

            nullableTypesTest.ExecuteNonQuery(
                new Dictionary<int, string>
                    {
                        { Random.Next(), GenerateRandomString(50) },
                        { Random.Next(), GenerateRandomString(50) },
                        { Random.Next(), GenerateRandomString(50) }
                    });
            Assert.IsNotNull(nullableTypesTest);
        }

        [TestMethod]
        public void ExecuteNonQuery_WithTupleParmeterAndAdditionalNullableTableParametersNotSet_ExecutesSuccessfully()
        {
            string connectionString = CreateConnectionString("DifferentLocalData", true);
            SqlProgram<IEnumerable<Tuple<int, string>>> nullableTypesTest =
                new SqlProgram<IEnumerable<Tuple<int, string>>>(connectionString, "spTakesTableAdditionalColumns");

            nullableTypesTest.ExecuteNonQuery(
                new List<Tuple<int, string>>
                    {
                        new Tuple<int, string>(Random.Next(), GenerateRandomString(50)),
                        new Tuple<int, string>(Random.Next(), GenerateRandomString(50)),
                        new Tuple<int, string>(Random.Next(), GenerateRandomString(50)),
                        new Tuple<int, string>(Random.Next(), GenerateRandomString(50))
                    });
            Assert.IsNotNull(nullableTypesTest);
        }

        [TestMethod]
        public void ExecuteNonQuery_WithTupleParmeterAndSomeAdditionalNullableTableParametersSet_ExecutesSuccessfully()
        {
            string connectionString = CreateConnectionString("DifferentLocalData", true);
            SqlProgram<IEnumerable<Tuple<int, string, DateTime?>>> nullableTypesTest =
                new SqlProgram<IEnumerable<Tuple<int, string, DateTime?>>>(connectionString, "spTakesTableAdditionalColumns");

            nullableTypesTest.ExecuteNonQuery(
                new List<Tuple<int, string, DateTime?>>
                    {
                        new Tuple<int, string, DateTime?>(Random.Next(), GenerateRandomString(50), DateTime.Today),
                        new Tuple<int, string, DateTime?>(Random.Next(), GenerateRandomString(50), DateTime.Today),
                        new Tuple<int, string, DateTime?>(Random.Next(), GenerateRandomString(50), DateTime.Today)
                    });
            Assert.IsNotNull(nullableTypesTest);
        }

        [TestMethod]
        public void ExecuteNonQuery_WithTupleParmeterOfMixedNullableColumns_ExecutesSuccessfully()
        {
            string connectionString = CreateConnectionString("DifferentLocalData", true);
            SqlProgram<IEnumerable<Tuple<string, int?, DateTime>>> nullableTypesTest =
                new SqlProgram<IEnumerable<Tuple<string, int?, DateTime>>>(connectionString, "spTakesMixedNullableColumns");

            nullableTypesTest.ExecuteNonQuery(
                new List<Tuple<string, int?, DateTime>>
                    {
                        new Tuple<string, int?, DateTime>(GenerateRandomString(50), null, DateTime.Today),
                        new Tuple<string, int?, DateTime>(GenerateRandomString(50), null, DateTime.Today),
                        new Tuple<string, int?, DateTime>(GenerateRandomString(50), null, DateTime.Today)
                    });
            Assert.IsNotNull(nullableTypesTest);
        }

        [TestMethod]
        public void ExecuteNonQuery_WithTupleParameterSetToNull_ExecutesSuccessfully()
        {
            string connectionString = CreateConnectionString("DifferentLocalData", true);
            SqlProgram<IEnumerable<Tuple<int, string, bool>>> tableTypeTest =
                new SqlProgram<IEnumerable<Tuple<int, string, bool>>>(connectionString, "spTakesTupleTable");

            int nonQueryResult =
                tableTypeTest.ExecuteNonQuery(null);
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public void ExecuteNonQuery_WithEmptyTupleParameter_ExecutesSuccessfully()
        {
            string connectionString = CreateConnectionString("DifferentLocalData", true);
            SqlProgram<IEnumerable<Tuple<int, string, bool>>> tableTypeTest =
                new SqlProgram<IEnumerable<Tuple<int, string, bool>>>(connectionString, "spTakesTupleTable");

            int nonQueryResult =
                tableTypeTest.ExecuteNonQuery(new List<Tuple<int, string, bool>>());
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public void ExecuteNonQuery_WithXmlDocumentParameter_ExecutesSuccessfully()
        {
            System.Xml.XmlDocument document = new System.Xml.XmlDocument();
            System.Xml.XmlElement rootNode = document.CreateElement("MyTestRoot");
            System.Xml.XmlElement childNode = document.CreateElement("MyTestChild");
            System.Xml.XmlAttribute anAttribute = document.CreateAttribute("anattribute");

            childNode.InnerText = Guid.NewGuid().ToString();
            anAttribute.Value = Random.Next().ToString();
            rootNode.Attributes.Append(anAttribute);
            document.AppendChild(rootNode);
            rootNode.AppendChild(childNode);

            string connectionString = CreateConnectionString("DifferentLocalData", true);
            SqlProgram<System.Xml.XmlDocument> xmlTypeTest =
                new SqlProgram<System.Xml.XmlDocument>(connectionString, "spTakesXml");

            int nonQueryResult =
                xmlTypeTest.ExecuteNonQuery(document);
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public void ExecuteNonQuery_WithTableParameterWhichTakesXmlDocument_ExecutesSuccessfully()
        {
            System.Xml.XmlDocument document = new System.Xml.XmlDocument();
            System.Xml.XmlElement rootNode = document.CreateElement("MyTestRoot");
            System.Xml.XmlElement childNode = document.CreateElement("MyTestChild");
            System.Xml.XmlAttribute anAttribute = document.CreateAttribute("anattribute");

            childNode.InnerText = Guid.NewGuid().ToString();
            anAttribute.Value = Random.Next().ToString();
            rootNode.Attributes.Append(anAttribute);
            document.AppendChild(rootNode);
            rootNode.AppendChild(childNode);

            string connectionString = CreateConnectionString("DifferentLocalData", true);
            SqlProgram<IEnumerable<System.Xml.XmlDocument>> xmlTypeTest =
                new SqlProgram<IEnumerable<System.Xml.XmlDocument>>(connectionString, "spTakesTableXml");

            int nonQueryResult =
                xmlTypeTest.ExecuteNonQuery(new List<System.Xml.XmlDocument> { document, document });
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public void ExecuteNonQuery_WithXmlElementParameter_ExecutesSuccessfully()
        {
            System.Xml.XmlDocument document = new System.Xml.XmlDocument();
            System.Xml.XmlElement rootNode = document.CreateElement("MyTestRoot");
            System.Xml.XmlElement childNode = document.CreateElement("MyTestChild");
            System.Xml.XmlAttribute anAttribute = document.CreateAttribute("anattribute");

            childNode.InnerText = Guid.NewGuid().ToString();
            anAttribute.Value = Random.Next().ToString();
            rootNode.Attributes.Append(anAttribute);
            document.AppendChild(rootNode);
            rootNode.AppendChild(childNode);

            string connectionString = CreateConnectionString("DifferentLocalData", true);
            SqlProgram<System.Xml.XmlElement> xmlTypeTest =
                new SqlProgram<System.Xml.XmlElement>(connectionString, "spTakesXml");

            int nonQueryResult =
                xmlTypeTest.ExecuteNonQuery(rootNode);
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public void ExecuteNonQuery_WithTableParameterWhichTakesXmlElement_ExecutesSuccessfully()
        {
            System.Xml.XmlDocument document = new System.Xml.XmlDocument();
            System.Xml.XmlElement rootNode = document.CreateElement("MyTestRoot");
            System.Xml.XmlElement childNode = document.CreateElement("MyTestChild");
            System.Xml.XmlAttribute anAttribute = document.CreateAttribute("anattribute");

            childNode.InnerText = Guid.NewGuid().ToString();
            anAttribute.Value = Random.Next().ToString();
            rootNode.Attributes.Append(anAttribute);
            document.AppendChild(rootNode);
            rootNode.AppendChild(childNode);

            string connectionString = CreateConnectionString("DifferentLocalData", true);
            SqlProgram<IEnumerable<System.Xml.XmlElement>> xmlTypeTest =
                new SqlProgram<IEnumerable<System.Xml.XmlElement>>(connectionString, "spTakesTableXml");

            int nonQueryResult =
                xmlTypeTest.ExecuteNonQuery(new List<System.Xml.XmlElement> { rootNode, rootNode });
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public void ExecuteNonQuery_WithXDocumentParameter_ExecutesSuccessfully()
        {
            XDocument document = new XDocument();
            document.Add(new XElement("MyTestRoot", new XAttribute("myattribute", Random.Next()),
                new XElement("MyTestChild", Guid.NewGuid())));

            string connectionString = CreateConnectionString("DifferentLocalData", true);
            SqlProgram<XDocument> xmlTypeTest =
                new SqlProgram<XDocument>(connectionString, "spTakesXml");

            int nonQueryResult =
                xmlTypeTest.ExecuteNonQuery(document);
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public void ExecuteNonQuery_WithTableParameterWhichTakesXDocument_ExecutesSuccessfully()
        {
            XDocument document = new XDocument();
            document.Add(new XElement("MyTestRoot", new XAttribute("myattribute", Random.Next()),
                new XElement("MyTestChild", Guid.NewGuid())));

            string connectionString = CreateConnectionString("DifferentLocalData", true);
            SqlProgram<IEnumerable<XDocument>> xmlTypeTest =
                new SqlProgram<IEnumerable<XDocument>>(connectionString, "spTakesTableXml");

            int nonQueryResult =
                xmlTypeTest.ExecuteNonQuery(new List<XDocument> { document, document });
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public void ExecuteNonQuery_WithXElementParameter_ExecutesSuccessfully()
        {
            XElement element = new XElement("MyTestRoot", new XAttribute("myattribute", Random.Next()),
                                            new XElement("MyTestChild", Guid.NewGuid()));

            string connectionString = CreateConnectionString("DifferentLocalData", true);
            SqlProgram<XElement> xmlTypeTest =
                new SqlProgram<XElement>(connectionString, "spTakesXml");

            int nonQueryResult =
                xmlTypeTest.ExecuteNonQuery(element);
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public void ExecuteNonQuery_WithXElementParameter_XElement()
        {
            XElement element = new XElement("MyTestRoot", new XAttribute("myattribute", Random.Next()),
                                            new XElement("MyTestChild", Guid.NewGuid()));

            string connectionString = CreateConnectionString("DifferentLocalData", true);
            SqlProgram<IEnumerable<XElement>> xmlTypeTest =
                new SqlProgram<IEnumerable<XElement>>(connectionString, "spTakesTableXml");

            int nonQueryResult =
                xmlTypeTest.ExecuteNonQuery(new [] { element, element });
            Assert.AreEqual(-1, nonQueryResult);
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
