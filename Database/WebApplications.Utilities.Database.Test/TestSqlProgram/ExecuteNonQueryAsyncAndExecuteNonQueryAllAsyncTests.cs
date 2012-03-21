using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebApplications.Utilities.Database.Test.TestSqlProgram
{
    public partial class SqlProgramTests
    {
        [TestMethod]
        public void ExecuteNonQueryAsync_ExecutesSuccessfully()
        {
            string connectionString = CreateConnectionString("LocalData", true);
            SqlProgram nonQueryTest = new SqlProgram(connectionString: connectionString, name: "spNonQuery");
            Task<int> nonQueryResult = nonQueryTest.ExecuteNonQueryAsync();
            Assert.IsNotNull(nonQueryResult);
            nonQueryResult.Wait();
            Assert.AreEqual(-1, nonQueryResult.Result);
        }

        [TestMethod]
        public void ExecuteNonQueryAsyncAll_ExecutesSuccessfully()
        {
            string localDataConnString = CreateConnectionString("LocalData", true);
            string localDataCopyConnString = CreateConnectionString("LocalDataCopy", true);

            SqlProgram nonQueryTest =
                new SqlProgram(connection: new LoadBalancedConnection(localDataConnString, localDataCopyConnString),
                               name: "spNonQuery");
            Task<IEnumerable<int>> nonQueryResult = nonQueryTest.ExecuteNonQueryAllAsync();
            nonQueryResult.Wait();
            Assert.AreEqual(2, nonQueryResult.Result.Count());

            foreach (int result in nonQueryResult.Result)
                Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void ExecuteNonQueryAsync_WithParameters_ExecutesSuccessfully()
        {
            string connectionString = CreateConnectionString("LocalData", true);
            SqlProgram<string, int> nonQueryTest = new SqlProgram<string, int>(connectionString: connectionString, name: "spNonQuery");

            string randomString = GenerateRandomString(20);
            int randomInt = Random.Next();

            Task<int> nonQueryResult = nonQueryTest.ExecuteNonQueryAsync(
                c =>
                {
                    c.SetParameter("@stringParam", randomString);
                    c.SetParameter("@intParam", randomInt);
                });

            nonQueryResult.Wait();
            Assert.AreEqual(-1, nonQueryResult.Result);
        }

        [TestMethod]
        public void ExecuteNonQueryAllAsync_WithParameters_ExecutesSuccessfully()
        {
            string localDataConnString = CreateConnectionString("LocalData", true);
            string localDataCopyConnString = CreateConnectionString("LocalDataCopy", true);

            SqlProgram<string, int> nonQueryTest =
                new SqlProgram<string, int>(connection: new LoadBalancedConnection(localDataConnString, localDataCopyConnString),
                               name: "spNonQuery");

            string randomString = GenerateRandomString(20);
            int randomInt = Random.Next();

            Task<IEnumerable<int>> nonQueryResult = nonQueryTest.ExecuteNonQueryAllAsync(
                c =>
                    {
                        c.SetParameter("@stringParam", randomString);
                        c.SetParameter("@intParam", randomInt);
                    });

            nonQueryResult.Wait();
            Assert.AreEqual(2, nonQueryResult.Result.Count());

            foreach (int result in nonQueryResult.Result)
                Assert.AreEqual(-1, result);
        }
    }
}
