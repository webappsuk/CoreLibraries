using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Logging;

namespace WebApplications.Utilities.Database.Test
{
    [TestClass]
    public class LoadBalanceConnectionTests : DatabaseTestBase
    {
        [TestMethod]
        [ExpectedException(typeof(LoggingException))]
        public void Constructor_WithInvalidConnectionStrings_ThrowsLoggingException()
        {
            LoadBalancedConnection loadBalancedConnection =
                new LoadBalancedConnection(connectionStrings: new[] { string.Empty, string.Empty });
        }

        [TestMethod]
        public void Constructor_WithValidConnectionString_CreatesInstanceWithCountOfOne()
        {
            LoadBalancedConnection loadBalancedConnection =
                new LoadBalancedConnection(connectionString: CreateConnectionString("LocalData"));
            Assert.IsNotNull(loadBalancedConnection);
            Assert.AreEqual(1, loadBalancedConnection.Count());
        }

        [TestMethod]
        public void Constructor_WithSingleConnectionStringAndSchemaIsIdenticalCheckSetToTrue_CreatesInstanceWithCountOfOne()
        {
            LoadBalancedConnection loadBalancedConnection =
                new LoadBalancedConnection(connectionString: CreateConnectionString("LocalData"));
            Assert.IsNotNull(loadBalancedConnection);
            Assert.AreEqual(1, loadBalancedConnection.Count());
        }

        [TestMethod]
        public void Constructor_WithMultipleIdenticalConnections_Deduplicates()
        {
            LoadBalancedConnection loadBalancedConnection =
                new LoadBalancedConnection(CreateConnectionString("LocalData"), CreateConnectionString("LocalData"));
            Assert.IsNotNull(loadBalancedConnection);
            Assert.AreEqual(1, loadBalancedConnection.Count());
        }

        [TestMethod]
        public void ToString_ReturnsExpectedString()
        {
            LoadBalancedConnection loadBalancedConnection =
                new LoadBalancedConnection(connectionStrings: new[] { LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString });

            Assert.AreEqual("Load balanced connection string with '2' connections.", loadBalancedConnection.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(LoggingException))]
        public void Constructor_WithNegativeWeighting_ThrowsLoggingException()
        {
            LoadBalancedConnection loadBalancedConnection =
                new LoadBalancedConnection(connectionStrings:
                    new List<KeyValuePair<string, double>>
                        {
                            new KeyValuePair<string, double>(LocalDatabaseConnectionString, -1)
                        });

            Assert.IsNull(loadBalancedConnection);
        }

        [TestMethod]
        public void CreateConnection_WithNoParameters_ReturnsSqlConnection()
        {
            //LoadBalancedConnection loadBalancedConnection =
            //    new LoadBalancedConnection(CreateConnectionString("LocalData"));

            //using (SqlConnection connection = loadBalancedConnection.GetSqlConnection())
            //{
            //    Assert.IsNotNull(connection);
            //    Assert.IsFalse(connection.State == ConnectionState.Open);

            //    // Check that we are always coerced to being asynchronous
            //    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connection.ConnectionString);
            //    Assert.IsNotNull(builder);
            //    Assert.IsTrue(builder.AsynchronousProcessing);
            //}
        }

        [TestMethod]
        [Ignore]
        public void ReloadSchemas()
        {
            //LoadBalancedConnection loadBalancedConnection =
            //    new LoadBalancedConnection(connectionString: CreateConnectionString("LocalData"));

            //bool reloadSchemas = loadBalancedConnection.ReloadSchemas();

            //// As we're reloading schemas instantly there should be no changes
            //Assert.IsFalse(reloadSchemas);
        }

        [TestMethod]
        public void GetEnumerator_ReturnsIEnumerator()
        {
            IEnumerable loadBalancedConnection =
                new LoadBalancedConnection(connectionString: CreateConnectionString("LocalData"));

            IEnumerator enumerator = loadBalancedConnection.GetEnumerator();
            Assert.IsNotNull(enumerator);
        }

        [TestMethod]
        public void Constructor_WithConnectionsWithIdenticalSchemas_CreatesInstance()
        {
            List<KeyValuePair<string, double>> connections =
                new List<KeyValuePair<string, double>>
                    {
                        new KeyValuePair<string, double>(CreateConnectionString("LocalData"), 0.5),
                        new KeyValuePair<string, double>(CreateConnectionString("LocalDataCopy"), 0.5)
                    };

            LoadBalancedConnection loadBalancedConnection =
                new LoadBalancedConnection(connectionStrings: connections);

            Assert.IsNotNull(loadBalancedConnection);
        }

        [TestMethod]
        [ExpectedException(typeof(LoggingException))]
        public void Constructor_WithEmptyConnectionsCollection_ThrowsLoggingException()
        {
            new LoadBalancedConnection(new string[] { });
        }

        [TestMethod]
        public void ReloadSchemas_WithNoChanges_ReturnsFalse()
        {
        //    List<KeyValuePair<string, double>> connections =
        //        new List<KeyValuePair<string, double>>
        //            {
        //                new KeyValuePair<string, double>(CreateConnectionString("LocalDataCopy"), 0.5),
        //                new KeyValuePair<string, double>(CreateConnectionString("LocalDataSecondCopy"), 0.5)
        //            };

        //    LoadBalancedConnection loadBalancedConnection = new LoadBalancedConnection(connections);
        //    bool reloadSchemas = loadBalancedConnection.ReloadSchemas();
        //    Assert.IsFalse(reloadSchemas);
        }

    }
}