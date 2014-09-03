using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;

namespace WebApplications.Utilities.Database.Test.TestSqlProgram
{
    public partial class SqlProgramTests
    {
        [TestMethod]
        public async Task ExecuteNonQueryAsync_ExecutesSuccessfully()
        {
            SqlProgram nonQueryTest = await SqlProgram.Create((Connection) LocalDatabaseConnectionString, name: "spNonQuery");
            Task<int> nonQueryResult = nonQueryTest.ExecuteNonQueryAsync();
            Assert.IsNotNull(nonQueryResult);
            nonQueryResult.Wait();
            Assert.AreEqual(-1, nonQueryResult.Result);
        }

        [TestMethod]
        public async Task ExecuteNonQueryAsyncAll_ExecutesSuccessfully()
        {

            SqlProgram nonQueryTest = 
                await SqlProgram.Create(connection: new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString),
                       name: "spNonQuery");
            Task<IEnumerable<int>> nonQueryResult = nonQueryTest.ExecuteNonQueryAllAsync();
            nonQueryResult.Wait();
            Assert.AreEqual(2, nonQueryResult.Result.Count());

            foreach (int result in nonQueryResult.Result)
                Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public async Task ExecuteNonQueryAsync_WithParameters_ExecutesSuccessfully()
        {
            SqlProgram<string, int> nonQueryTest = await SqlProgram<string, int>.Create((Connection) LocalDatabaseConnectionString, name: "spNonQuery");

            Task<int> nonQueryResult = nonQueryTest.ExecuteNonQueryAsync(
                c =>
                {
                    c.SetParameter("@stringParam", Random.RandomString(20));
                    c.SetParameter("@intParam", Random.RandomInt32());
                });

            nonQueryResult.Wait();
            Assert.AreEqual(-1, nonQueryResult.Result);
        }

        [TestMethod]
        public async Task ExecuteNonQueryAllAsync_WithParameters_ExecutesSuccessfully()
        {
            SqlProgram<string, int> nonQueryTest =
                await SqlProgram<string, int>.Create(connection: new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString),
                               name: "spNonQuery");

            Task<IEnumerable<int>> nonQueryResult = nonQueryTest.ExecuteNonQueryAllAsync(
                c =>
                    {
                        c.SetParameter("@stringParam", Random.RandomString(20));
                        c.SetParameter("@intParam", Random.RandomInt32());
                    });

            nonQueryResult.Wait();
            Assert.AreEqual(2, nonQueryResult.Result.Count());

            foreach (int result in nonQueryResult.Result)
                Assert.AreEqual(-1, result);
        }
    }
}
