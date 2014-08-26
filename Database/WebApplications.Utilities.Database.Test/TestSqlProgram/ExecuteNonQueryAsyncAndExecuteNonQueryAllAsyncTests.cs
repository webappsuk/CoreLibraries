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
        public void ExecuteNonQueryAsync_ExecutesSuccessfully()
        {
            SqlProgram nonQueryTest = Create(connectionString: _localConnectionStringWithAsync, name: "spNonQuery");
            Task<int> nonQueryResult = nonQueryTest.ExecuteNonQueryAsync();
            Assert.IsNotNull(nonQueryResult);
            nonQueryResult.Wait();
            Assert.AreEqual(-1, nonQueryResult.Result);
        }

        [TestMethod]
        public void ExecuteNonQueryAsyncAll_ExecutesSuccessfully()
        {

            SqlProgram nonQueryTest =
                SqlProgram.Create(connection: new LoadBalancedConnection(_localConnectionStringWithAsync, _localCopyConnectionStringWithAsync),
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
            SqlProgram<string, int> nonQueryTest = new SqlProgram<string, int>(connectionString: _localConnectionStringWithAsync, name: "spNonQuery");

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
        public void ExecuteNonQueryAllAsync_WithParameters_ExecutesSuccessfully()
        {
            SqlProgram<string, int> nonQueryTest =
                new SqlProgram<string, int>(connection: new LoadBalancedConnection(_localConnectionStringWithAsync, _localCopyConnectionStringWithAsync),
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
