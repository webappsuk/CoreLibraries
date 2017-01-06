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

        [TestMethod]
        public async Task ExecuteNonQueryAsync_WithOutputParameters_ExecutesSuccessfully()
        {
            SqlProgram<int, Out<int>, Out<int>> program =
                await SqlProgram<int, Out<int>, Out<int>>.Create((Connection)LocalDatabaseConnectionString, "spOutputParameters");

            const int inputVal = 123;
            const int inputOutputVal = 321;

            Out<int> inputOutput = new Out<int>(inputOutputVal);
            Out<int> output = new Out<int>();

            int nonQueryResult = await program.ExecuteNonQueryAsync(inputVal, inputOutput, output);
            Assert.AreEqual(-1, nonQueryResult);

            Assert.IsNull(inputOutput.OutputError, inputOutput.OutputError?.Message);
            Assert.IsNull(output.OutputError, output.OutputError?.Message);

            Assert.AreEqual(inputOutputVal * 2, inputOutput.OutputValue.Value);
            Assert.AreEqual(inputVal, output.OutputValue.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public async Task ExecuteNonQueryAsync_WithOutputParametersNull_ThrowsException()
        {
            SqlProgram<int?, Out<int?>, Out<int>> program =
                await SqlProgram<int?, Out<int?>, Out<int>>.Create((Connection)LocalDatabaseConnectionString, "spOutputParameters");

            Out<int?> inputOutput = new Out<int?>(null);
            Out<int> output = new Out<int>();

            int nonQueryResult = await program.ExecuteNonQueryAsync(null, null, output);
            Assert.AreEqual(-1, nonQueryResult);

            Assert.IsNull(inputOutput.OutputError, inputOutput.OutputError?.Message);
            Assert.IsInstanceOfType(output.OutputError, typeof(InvalidCastException));

            Assert.AreEqual(null, inputOutput.OutputValue.Value);
            Assert.AreEqual(null, output.OutputValue.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task ExecuteNonQueryAllAsync_WithOutputParametersAndOut_ThrowsArgumentException()
        {
            SqlProgram<int, Out<int>, Out<int>> program =
                await SqlProgram<int, Out<int>, Out<int>>.Create(
                    new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString),
                    "spOutputParameters");

            const int inputVal = 123;
            const int inputOutputVal = 321;

            Out<int> inputOutput = new Out<int>(inputOutputVal);
            Out<int> output = new Out<int>();

            await program.ExecuteNonQueryAllAsync(inputVal, inputOutput, output);
        }

        [TestMethod]
        public async Task ExecuteNonQueryAllAsync_WithOutputParametersAndMultiOut_ExecutesSuccessfully()
        {
            SqlProgram<int, Out<int>, Out<int>> program =
                await SqlProgram<int, Out<int>, Out<int>>.Create(
                    new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString),
                    "spOutputParameters");

            const int inputVal = 123;
            const int inputOutputVal = 321;

            MultiOut<int> inputOutput = new MultiOut<int>(inputOutputVal);
            MultiOut<int> output = new MultiOut<int>();

            int[] nonQueryResult = (await program.ExecuteNonQueryAllAsync(inputVal, inputOutput, output)).ToArray();
            Assert.AreEqual(2, nonQueryResult.Length);
            Assert.IsTrue(nonQueryResult.All(i => i == -1));

            Assert.IsNull(inputOutput.OutputError, inputOutput.OutputError?.Message);
            Assert.IsNull(output.OutputError, output.OutputError?.Message);

            Assert.AreEqual(inputOutputVal * 2, inputOutput.OutputValue.Value);
            Assert.AreEqual(inputVal, output.OutputValue.Value);

            Assert.IsTrue(inputOutput.All(o => o.OutputValue.Value == inputOutputVal * 2));
            Assert.IsTrue(output.All(o => o.OutputValue.Value == inputVal));
        }
    }
}
