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
        
        [TestMethod]
        public async Task ExecuteScalarAsync_WithOutputParameters_ExecutesSuccessfully()
        {
            SqlProgram<int, Out<int>, Out<int>> program =
                await SqlProgram<int, Out<int>, Out<int>>.Create((Connection)LocalDatabaseConnectionString, "spOutputParameters");

            const int inputVal = 123;
            const int inputOutputVal = 321;

            Out<int> inputOutput = new Out<int>(inputOutputVal);
            Out<int> output = new Out<int>();

            string scalarResult = await program.ExecuteScalarAsync<string>(inputVal, inputOutput, output);
            Assert.AreEqual("<foo>bar</foo>", scalarResult);

            Assert.IsNull(inputOutput.OutputError, inputOutput.OutputError?.Message);
            Assert.IsNull(output.OutputError, output.OutputError?.Message);

            Assert.AreEqual(inputOutputVal * 2, inputOutput.OutputValue.Value);
            Assert.AreEqual(inputVal, output.OutputValue.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task ExecuteScalarAllAsync_WithOutputParametersAndOut_ThrowsArgumentException()
        {
            SqlProgram<int, Out<int>, Out<int>> program =
                await SqlProgram<int, Out<int>, Out<int>>.Create(
                    new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString),
                    "spOutputParameters");

            const int inputVal = 123;
            const int inputOutputVal = 321;

            Out<int> inputOutput = new Out<int>(inputOutputVal);
            Out<int> output = new Out<int>();

            await program.ExecuteScalarAllAsync<string>(inputVal, inputOutput, output);
        }

        [TestMethod]
        public async Task ExecuteScalarAllAsync_WithOutputParametersAndMultiOut_ExecutesSuccessfully()
        {
            SqlProgram<int, Out<int>, Out<int>> program =
                await SqlProgram<int, Out<int>, Out<int>>.Create(
                    new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString),
                    "spOutputParameters");

            const int inputVal = 123;
            const int inputOutputVal = 321;

            MultiOut<int> inputOutput = new MultiOut<int>(inputOutputVal);
            MultiOut<int> output = new MultiOut<int>();

            string[] scalarResult = (await program.ExecuteScalarAllAsync<string>(inputVal, inputOutput, output)).ToArray();
            Assert.AreEqual(2, scalarResult.Length);
            Assert.IsTrue(scalarResult.All(i => i == "<foo>bar</foo>"));

            Assert.IsNull(inputOutput.OutputError, inputOutput.OutputError?.Message);
            Assert.IsNull(output.OutputError, output.OutputError?.Message);

            Assert.AreEqual(inputOutputVal * 2, inputOutput.OutputValue.Value);
            Assert.AreEqual(inputVal, output.OutputValue.Value);

            Assert.IsTrue(inputOutput.All(o => o.OutputValue.Value == inputOutputVal * 2));
            Assert.IsTrue(output.All(o => o.OutputValue.Value == inputVal));
        }
    }
}
