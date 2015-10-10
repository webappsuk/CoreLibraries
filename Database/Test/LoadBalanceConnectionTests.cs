using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
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
        public async Task CreateConnection_WithNoParameters_ReturnsSqlConnection()
        {
            LoadBalancedConnection loadBalancedConnection =
                new LoadBalancedConnection(CreateConnectionString("LocalData"));

            using (SqlConnection connection = new SqlConnection(loadBalancedConnection.ChooseConnectionString()))
            {
                Assert.IsNotNull(connection);
                Assert.IsFalse(connection.State == ConnectionState.Open);

                // Check that we are always coerced to being asynchronous
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connection.ConnectionString);
                Assert.IsNotNull(builder);
                Assert.IsTrue(builder.AsynchronousProcessing);
            }
            
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

    }
}