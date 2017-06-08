using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Annotations;

// ReSharper disable ConsiderUsingConfigureAwait
// ReSharper disable InconsistentNaming
#pragma warning disable 1591 // Missing XML commend
#pragma warning disable 0618 // Type or member is obsolete

namespace WebApplications.Utilities.Database.Test
{
    public partial class SqlProgramTextTests
    {
        [NotNull]
        private static readonly string Different_spReturnsXml =
            GetProgramText("spReturnsXml", DifferentLocalDatabaseConnectionString);

        [NotNull]
        private static readonly string Different_spTakesXml =
            GetProgramText("spTakesXml", DifferentLocalDatabaseConnectionString);

        [TestMethod]
        public async Task ExecuteXmlReaderAsync_WithNoParameters_ExecuteReturnsExpectedString()
        {
            SqlProgram program = await SqlProgram.Create(
                DifferentLocalDatabaseConnection,
                "spReturnsXml",
                Different_spReturnsXml,
                CommandType.Text);
            
            XElement result = await program.ExecuteXmlReaderAsync(async (reader, token) => XElement.Load(reader));
            Assert.AreEqual("<foo>bar</foo>", result.ToString());
        }

        [TestMethod]
        public async Task ExecuteXmlReaderAllAsync_WithNoParameters_ExecuteReturnsExpectedString()
        {
            SqlProgram program = await SqlProgram.Create(
                new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString),
                "spReturnsXml",
                Different_spReturnsXml,
                CommandType.Text);
            
            XElement[] results = (await program.ExecuteXmlReaderAllAsync(async (reader, token) => XElement.Load(reader))).ToArray();
            Assert.AreEqual(2, results.Length);

            foreach (XElement result in results)
                Assert.AreEqual("<foo>bar</foo>", result.ToString());
        } 

        [TestMethod]
        public async Task ExecuteXmlReaderAsync_WithParameters_ReturnedExpectedXml()
        {
            SqlProgram<XElement> program =
                await SqlProgram<XElement>.Create(
                    DifferentLocalDatabaseConnection,
                    "spTakesXml",
                    "@xmlParam",
                    Different_spTakesXml,
                    CommandType.Text);

            XElement element = XElement.Parse("<foo>bar</foo>");

            XElement result = await program.ExecuteXmlReaderAsync(async (reader, token) => XElement.Load(reader), element);

            Assert.AreEqual(element.ToString(), result.ToString());
        }

        [TestMethod]
        public async Task ExecuteXmlReaderAllAsync_WithParameters_ReturnedExpectedXml()
        {
            SqlProgram<XElement> program =
                await SqlProgram<XElement>.Create(
                    new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString),
                    "spTakesXml",
                    "@xmlParam",
                    Different_spTakesXml,
                    CommandType.Text);

            XElement element = XElement.Parse("<foo>bar</foo>");
            
            XElement[] results = (await program.ExecuteXmlReaderAllAsync(async (reader, token) => XElement.Load(reader), element)).ToArray();
            Assert.AreEqual(2, results.Length);

            foreach (XElement result in results)
                Assert.AreEqual(element.ToString(), result.ToString());
        }

        [TestMethod]
        public async Task ExecuteXmlReaderAsync_WithOutputParameters_ExecutesSuccessfully()
        {
            SqlProgram<int, Out<int>, Out<int>> program =
                await SqlProgram<int, Out<int>, Out<int>>.Create(
                    LocalDatabaseConnection,
                    "spOutputParameters",
                    "@inputParam",
                    "@inputOutputParam",
                    "@outputParam",
                    Local_spOutputParameters,
                    CommandType.Text);

            const int inputVal = 123;
            const int inputOutputVal = 321;

            Out<int> inputOutput = new Out<int>(inputOutputVal);
            Out<int> output = new Out<int>();

            XElement result = await program.ExecuteXmlReaderAsync(
                async (reader, token) => XElement.Load(reader),
                inputVal,
                inputOutput,
                output);

            Assert.AreEqual("<foo>bar</foo>", result.ToString());

            Assert.IsNull(inputOutput.OutputError, inputOutput.OutputError?.Message);
            Assert.IsNull(output.OutputError, output.OutputError?.Message);

            Assert.AreEqual(inputOutputVal * 2, inputOutput.OutputValue.Value);
            Assert.AreEqual(inputVal, output.OutputValue.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task ExecuteXmlReaderAllAsync_WithOutputParametersAndOut_ThrowsArgumentException()
        {
            SqlProgram<int, Out<int>, Out<int>> program =
                await SqlProgram<int, Out<int>, Out<int>>.Create(
                    new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString),
                    "spOutputParameters",
                    "@inputParam",
                    "@inputOutputParam",
                    "@outputParam",
                    Local_spOutputParameters,
                    CommandType.Text);

            const int inputVal = 123;
            const int inputOutputVal = 321;

            Out<int> inputOutput = new Out<int>(inputOutputVal);
            Out<int> output = new Out<int>();

            await program.ExecuteXmlReaderAllAsync(
                async (reader, token) => Assert.Fail("Shouldnt reach this point"),
                inputVal,
                inputOutput,
                output);
        }

        [TestMethod]
        public async Task ExecuteXmlReaderAllAsync_WithOutputParametersAndMultiOut_ExecutesSuccessfully()
        {
            SqlProgram<int, Out<int>, Out<int>> program =
                await SqlProgram<int, Out<int>, Out<int>>.Create(
                    new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString),
                    "spOutputParameters",
                    "@inputParam",
                    "@inputOutputParam",
                    "@outputParam",
                    Local_spOutputParameters,
                    CommandType.Text);

            const int inputVal = 123;
            const int inputOutputVal = 321;

            MultiOut<int> inputOutput = new MultiOut<int>(inputOutputVal);
            MultiOut<int> output = new MultiOut<int>();

            XElement[] result = (await program.ExecuteXmlReaderAllAsync(
                async (reader, token) => XElement.Load(reader),
                inputVal,
                inputOutput,
                output)).ToArray();

            Assert.AreEqual(2, result.Length);
            Assert.IsTrue(result.All(i => i.ToString() == "<foo>bar</foo>"));

            Assert.IsNull(inputOutput.OutputError, inputOutput.OutputError?.Message);
            Assert.IsNull(output.OutputError, output.OutputError?.Message);

            Assert.AreEqual(inputOutputVal * 2, inputOutput.OutputValue.Value);
            Assert.AreEqual(inputVal, output.OutputValue.Value);

            Assert.IsTrue(inputOutput.All(o => o.OutputValue.Value == inputOutputVal * 2));
            Assert.IsTrue(output.All(o => o.OutputValue.Value == inputVal));
        }
    }
}
