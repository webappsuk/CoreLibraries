using System;
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
        public async Task ExecuteScalarAsync_ExecutesAndReturnsExpectedResult()
        {
            SqlProgram scalarTest =
                await SqlProgram.Create((Connection)DifferentLocalDatabaseConnectionString, "spReturnsScalar");

            Task<string> task = scalarTest.ExecuteScalarAsync<string>();
            Assert.IsNotNull(task);
            task.Wait();
            Assert.IsTrue(task.IsCompleted);
            Assert.AreEqual("HelloWorld", task.Result);
        }

        [TestMethod]
        public async Task ExecuteScalarAsyncAll_ExecutesAndReturnsExpectedResult()
        {
            string[] connectionStrings =
                    {
                        LocalDatabaseConnectionString,
                        LocalDatabaseCopyConnectionString
                    };

            SqlProgram scalarTest =
            await SqlProgram.Create(new LoadBalancedConnection(connectionStrings), "spReturnsScalarString");

            Task<IEnumerable<string>> tasks = scalarTest.ExecuteScalarAllAsync<string>();
            Assert.AreEqual(2, tasks.Result.Count());
            tasks.Wait();
            Assert.IsTrue(tasks.IsCompleted);

            foreach (string result in tasks.Result)
                Assert.AreEqual("HelloWorld", result);
        }

        [TestMethod]
        public async Task ExecuteScalarAsync_WithParameter_ExecutesAndReturnsExpectedResult()
        {
            SqlProgram<string> scalarTest =
                await SqlProgram<string>.Create((Connection)DifferentLocalDatabaseConnectionString, "spTakesParamAndReturnsScalar", "@firstName");

            string randomString = Random.RandomString(10, false);
            Task<string> task = scalarTest.ExecuteScalarAsync<string>(randomString);

            Assert.IsNotNull(task);
            task.Wait();
            Assert.IsTrue(task.IsCompleted);
            Assert.AreEqual("Hello " + randomString, task.Result);
        }

        [TestMethod]
        public async Task ExecuteScalarAsync_WithParameterSetViaAction_ExecutesAndReturnsExpectedResult()
        {
            SqlProgram<string> scalarTest =
                await SqlProgram<string>.Create((Connection)DifferentLocalDatabaseConnectionString, "spTakesParamAndReturnsScalar");
            string name = Random.RandomString(10, false);
            Task<string> task = scalarTest.ExecuteScalarAsync<string>(c => c.SetParameter("@firstName", name));
            Assert.IsNotNull(task);
            task.Wait();
            Assert.IsTrue(task.IsCompleted);

            // Note we won't get the full GUID back as the name is truncated.
            Assert.AreEqual("Hello " + name, task.Result);
        }
    }
}
