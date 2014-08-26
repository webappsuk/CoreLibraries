using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;
using WebApplications.Utilities.Database.Exceptions;
using WebApplications.Utilities.Logging;

namespace WebApplications.Utilities.Database.Test.TestSqlProgram
{
    public partial class SqlProgramTests
    {
        private readonly string _localConnectionString = CreateConnectionString("LocalData");
        private readonly string _localConnectionStringWithAsync = CreateConnectionString("LocalData", true);

        private readonly string _localCopyConnectionString = CreateConnectionString("LocalDataCopy");
        private readonly string _localCopyConnectionStringWithAsync = CreateConnectionString("LocalDataCopy", true);

        private readonly string _differentConnectionString = CreateConnectionString("DifferentLocalData");
        private readonly string _differentConnectionStringWithAsync = CreateConnectionString("DifferentLocalData", true);

        [TestMethod]
        public void ExecuteNonQuery_ExecutesSuccessfully()
        {
            SqlProgram nonQueryTest = Create(connectionString: _localConnectionString, name: "spNonQuery");
            int nonQueryResult = nonQueryTest.ExecuteNonQuery();
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public void ExecuteNonQueryAll_ExecutesSuccessfully()
        {
            SqlProgram nonQueryTest =
                SqlProgram.Create(connection: new LoadBalancedConnection(_localConnectionString, _localCopyConnectionString),
                       name: "spNonQuery");
            List<int> nonQueryResult = nonQueryTest.ExecuteNonQueryAll().ToList();
            Assert.AreEqual(2, nonQueryResult.Count);

            foreach (int result in nonQueryResult)
                Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void ExecuteNonQuery_WithParameters_ExecutesSuccessfully()
        {
            SqlProgram<string, int> nonQueryTest = new SqlProgram<string, int>(connectionString: _localConnectionString, name: "spNonQuery");

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
        public void ExecuteNonQuery_WithNullableType_ExecutesSuccessfully()
        {
            SqlProgram<IEnumerable<Tuple<int?, string, bool>>> tableTypeTest =
                new SqlProgram<IEnumerable<Tuple<int?, string, bool>>>(_differentConnectionString, "spTakesTupleTable");

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
        public void ExecuteNonQueryAll_WithParameters_ExecutesSuccessfully()
        {
            SqlProgram<string, int> nonQueryTest =
                new SqlProgram<string, int>(connection: new LoadBalancedConnection(_localConnectionString, _localCopyConnectionString),
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
        public void ExecuteNonQueryAll_WithUnknownProgramForConnection_ThrowsLoggingException()
        {
            SqlProgram<string, int> nonQueryTest =
                new SqlProgram<string, int>(connection: new LoadBalancedConnection(_localConnectionString, _differentConnectionString),
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
        [ExpectedException(typeof(SqlProgramExecutionException))]
        public void ExecuteNonQuery_WithTimeoutSet_ThrowsSqlProgramExecutionExceptionOnTimeout()
        {
            SqlProgram<int> timeoutTest =
                new SqlProgram<int>(_differentConnectionString, "spTimeoutTest",
                                    defaultCommandTimeout: new TimeSpan(0, 0, 5));
            DateTime timeStarted = DateTime.Now;
            timeoutTest.ExecuteNonQuery(10);
            DateTime timeEnded = DateTime.Now;
            Assert.AreEqual(10, (int)(timeEnded - timeStarted).TotalSeconds);
        }

        [TestMethod]
        public void ExecuteNonQueryAsync_ExecutesProcedureSuccessfully()
        {
            SqlProgram<int> timeoutTest =
                new SqlProgram<int>(_differentConnectionString, "spTimeoutTest");
            Task<int> task = timeoutTest.ExecuteNonQueryAsync();
            task.Wait();
            Assert.IsTrue(task.IsCompleted);
        }

        [TestMethod]
        public void ExecuteNonQuery_WithEnumerableIntParameter_ExecutesSuccessfully()
        {
            SqlProgram<IEnumerable<int>> tableTypeTest =
                new SqlProgram<IEnumerable<int>>(_differentConnectionString, "spTakesIntTable");

            int nonQueryResult = tableTypeTest.ExecuteNonQuery(new[] { 1, 2, 3, 4 });
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public void ExecuteNonQuery_WithEnumerableKeyValuePairParameter_ExecutesSuccessfully()
        {
            SqlProgram<IEnumerable<KeyValuePair<int, string>>> tableTypeTest =
                new SqlProgram<IEnumerable<KeyValuePair<int, string>>>(_differentConnectionString, "spTakesKvpTable");

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
        public void ExecuteNonQuery_WithTupleParameter_ExecutesSuccessfully()
        {
            SqlProgram<IEnumerable<Tuple<int, string, bool>>> tableTypeTest =
                new SqlProgram<IEnumerable<Tuple<int, string, bool>>>(_differentConnectionString, "spTakesTupleTable");

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
        public void ExecuteNonQuery_WithTupleParameterAndOtherParameters_ExecutesSuccessfully()
        {
            SqlProgram<IEnumerable<Tuple<string, string, string, DateTime, short?>>, int, DateTime> tableTypeTest =
                new SqlProgram<IEnumerable<Tuple<string, string, string, DateTime, short?>>, int, DateTime>(
                    _differentConnectionString, "spTakesTupleTablePlusTwo");

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
        public void ExecuteNonQuery_WithNullableDateTimeParameterSetToNull_ExecutesSuccessfully()
        {
            SqlProgram<string, int?, decimal, bool, DateTime?> nullableTypesTest =
                new SqlProgram<string, int?, decimal, bool, DateTime?>(_differentConnectionString, "spUltimateSproc");

            nullableTypesTest.ExecuteNonQuery(Random.RandomString(20), Random.RandomInt32(), decimal.Zero, true, null);
            Assert.IsNotNull(nullableTypesTest);
        }

        [TestMethod]
        public void ExecuteNonQuery_WithMoreNullableDateTimeParameterSetToNull_ExecutesSuccessfully()
        {
            var nullableTypesTest =
                new SqlProgram<int, DateTime?, DateTime?, DateTime?, DateTime?, int?, int?, int?>(_differentConnectionString, "spLoadsOfNullables");

            nullableTypesTest.ExecuteNonQuery(1, null, null, null, null, null, null, 1);
            Assert.IsNotNull(nullableTypesTest);
        }

        [TestMethod]
        public void ExecuteNonQuery_WithKeyValuePairParmeterAndAdditionalNullableTableParametersNotSet_ExecutesSuccessfully()
        {
            SqlProgram<IEnumerable<KeyValuePair<int, string>>> nullableTypesTest =
                new SqlProgram<IEnumerable<KeyValuePair<int, string>>>(_differentConnectionString, "spTakesTableAdditionalColumns");

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
        public void ExecuteNonQuery_WithTupleParmeterAndAdditionalNullableTableParametersNotSet_ExecutesSuccessfully()
        {
            SqlProgram<IEnumerable<Tuple<int, string>>> nullableTypesTest =
                new SqlProgram<IEnumerable<Tuple<int, string>>>(_differentConnectionString, "spTakesTableAdditionalColumns");

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
        public void ExecuteNonQuery_WithTupleParmeterAndSomeAdditionalNullableTableParametersSet_ExecutesSuccessfully()
        {
            SqlProgram<IEnumerable<Tuple<int, string, DateTime?>>> nullableTypesTest =
                new SqlProgram<IEnumerable<Tuple<int, string, DateTime?>>>(_differentConnectionString, "spTakesTableAdditionalColumns");

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
        public void ExecuteNonQuery_WithTupleParmeterOfMixedNullableColumns_ExecutesSuccessfully()
        {
            SqlProgram<IEnumerable<Tuple<string, int?, DateTime>>> nullableTypesTest =
                new SqlProgram<IEnumerable<Tuple<string, int?, DateTime>>>(_differentConnectionString, "spTakesMixedNullableColumns");

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
        public void ExecuteNonQuery_WithTupleParameterSetToNull_ExecutesSuccessfully()
        {
            SqlProgram<IEnumerable<Tuple<int, string, bool>>> tableTypeTest =
                new SqlProgram<IEnumerable<Tuple<int, string, bool>>>(_differentConnectionString, "spTakesTupleTable");

            int nonQueryResult =
                tableTypeTest.ExecuteNonQuery(null);
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public void ExecuteNonQuery_WithEmptyTupleParameter_ExecutesSuccessfully()
        {
            SqlProgram<IEnumerable<Tuple<int, string, bool>>> tableTypeTest =
                new SqlProgram<IEnumerable<Tuple<int, string, bool>>>(_differentConnectionString, "spTakesTupleTable");

            int nonQueryResult =
                tableTypeTest.ExecuteNonQuery(Enumerable.Empty<Tuple<int, string, bool>>());
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public void ExecuteNonQuery_WithXmlDocumentParameter_ExecutesSuccessfully()
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
                new SqlProgram<System.Xml.XmlDocument>(_differentConnectionString, "spTakesXml");

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

            childNode.InnerText = Random.RandomString(20);
            anAttribute.Value = Random.RandomInt32().ToString();
            rootNode.Attributes.Append(anAttribute);
            document.AppendChild(rootNode);
            rootNode.AppendChild(childNode);

            SqlProgram<IEnumerable<System.Xml.XmlDocument>> xmlTypeTest =
                new SqlProgram<IEnumerable<System.Xml.XmlDocument>>(_differentConnectionString, "spTakesTableXml");

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

            childNode.InnerText = Random.RandomString(20);
            anAttribute.Value = Random.RandomInt32().ToString();
            rootNode.Attributes.Append(anAttribute);
            document.AppendChild(rootNode);
            rootNode.AppendChild(childNode);

            SqlProgram<System.Xml.XmlElement> xmlTypeTest =
                new SqlProgram<System.Xml.XmlElement>(_differentConnectionString, "spTakesXml");

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

            childNode.InnerText = Random.RandomString(20);
            anAttribute.Value = Random.RandomInt32().ToString();
            rootNode.Attributes.Append(anAttribute);
            document.AppendChild(rootNode);
            rootNode.AppendChild(childNode);

            SqlProgram<IEnumerable<System.Xml.XmlElement>> xmlTypeTest =
                new SqlProgram<IEnumerable<System.Xml.XmlElement>>(_differentConnectionString, "spTakesTableXml");

            int nonQueryResult =
                xmlTypeTest.ExecuteNonQuery(new List<System.Xml.XmlElement> { rootNode, rootNode });
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public void ExecuteNonQuery_WithXDocumentParameter_ExecutesSuccessfully()
        {
            XDocument document = new XDocument();
            document.Add(new XElement("MyTestRoot", new XAttribute("myattribute", Random.RandomInt32()),
                new XElement("MyTestChild", Random.RandomString(20))));

            SqlProgram<XDocument> xmlTypeTest =
                new SqlProgram<XDocument>(_differentConnectionString, "spTakesXml");

            int nonQueryResult =
                xmlTypeTest.ExecuteNonQuery(document);
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public void ExecuteNonQuery_WithTableParameterWhichTakesXDocument_ExecutesSuccessfully()
        {
            XDocument document = new XDocument();
            document.Add(new XElement("MyTestRoot", new XAttribute("myattribute", Random.RandomInt32()),
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
            XElement element = new XElement("MyTestRoot", new XAttribute("myattribute", Random.RandomInt32()),
                                            new XElement("MyTestChild", Random.RandomString(20)));

            SqlProgram<XElement> xmlTypeTest =
                new SqlProgram<XElement>(_differentConnectionString, "spTakesXml");

            int nonQueryResult =
                xmlTypeTest.ExecuteNonQuery(element);
            Assert.AreEqual(-1, nonQueryResult);
        }

        [TestMethod]
        public void ExecuteNonQuery_WithXElementParameter_XElement()
        {
            XElement element = new XElement("MyTestRoot", new XAttribute("myattribute", Random.RandomInt32()),
                                            new XElement("MyTestChild", Random.RandomString(20)));

            SqlProgram<IEnumerable<XElement>> xmlTypeTest =
                new SqlProgram<IEnumerable<XElement>>(_differentConnectionString, "spTakesTableXml");

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
