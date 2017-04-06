using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;
using WebApplications.Utilities.Database.Exceptions;

namespace WebApplications.Utilities.Database.Test.TestSqlProgram
{
    public partial class SqlProgramTests
    {
        [TestMethod]
        public async Task ExecuteXmlReader_WithNoParameters_ExecuteReturnsExpectedString()
        {
            SqlProgram program = await SqlProgram.Create((Connection)DifferentLocalDatabaseConnectionString, name: "spReturnsXml");
            
            XElement result = program.ExecuteXmlReader(XElement.Load);
            Assert.AreEqual("<foo>bar</foo>", result.ToString());
        }

        [TestMethod]
        public async Task ExecuteXmlReaderAll_WithNoParameters_ExecuteReturnsExpectedString()
        {
            SqlProgram program = await SqlProgram.Create(
                new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString),
                "spReturnsXml");
            
            XElement[] results = program.ExecuteXmlReaderAll(XElement.Load).ToArray();
            Assert.AreEqual(2, results.Length);

            foreach (XElement result in results)
                Assert.AreEqual("<foo>bar</foo>", result.ToString());
        } 

        [TestMethod]
        public async Task ExecuteXmlReader_WithParameters_ReturnedExpectedXml()
        {
            SqlProgram<XElement> program =
                await SqlProgram<XElement>.Create(
                    (Connection)DifferentLocalDatabaseConnectionString,
                    name: "spTakesXml");

            XElement element = XElement.Parse("<foo>bar</foo>");

            XElement result = program.ExecuteXmlReader(XElement.Load, element);

            Assert.AreEqual(element.ToString(), result.ToString());
        }

        [TestMethod]
        public async Task ExecuteXmlReaderAll_WithParameters_ReturnedExpectedXml()
        {
            SqlProgram<XElement> program =
                await SqlProgram<XElement>.Create(
                    connection: new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString),
                    name: "spTakesXml");

            XElement element = XElement.Parse("<foo>bar</foo>");
            
            XElement[] results = program.ExecuteXmlReaderAll(XElement.Load, element).ToArray();
            Assert.AreEqual(2, results.Length);

            foreach (XElement result in results)
                Assert.AreEqual(element.ToString(), result.ToString());
        }

        [TestMethod]
        public async Task ExecuteXmlReader_WithOutputParameters_ExecutesSuccessfully()
        {
            SqlProgram<int, Out<int>, Out<int>> program =
                await SqlProgram<int, Out<int>, Out<int>>.Create((Connection)LocalDatabaseConnectionString, "spOutputParameters");

            const int inputVal = 123;
            const int inputOutputVal = 321;

            Out<int> inputOutput = new Out<int>(inputOutputVal);
            Out<int> output = new Out<int>();

            XElement result = program.ExecuteXmlReader(XElement.Load, inputVal, inputOutput, output);
            Assert.AreEqual("<foo>bar</foo>", result.ToString());

            Assert.IsNull(inputOutput.OutputError, inputOutput.OutputError?.Message);
            Assert.IsNull(output.OutputError, output.OutputError?.Message);

            Assert.AreEqual(inputOutputVal * 2, inputOutput.OutputValue.Value);
            Assert.AreEqual(inputVal, output.OutputValue.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task ExecuteXmlReaderAll_WithOutputParametersAndOut_ThrowsArgumentException()
        {
            SqlProgram<int, Out<int>, Out<int>> program =
                await SqlProgram<int, Out<int>, Out<int>>.Create(
                    new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString),
                    "spOutputParameters");

            const int inputVal = 123;
            const int inputOutputVal = 321;

            Out<int> inputOutput = new Out<int>(inputOutputVal);
            Out<int> output = new Out<int>();

            program.ExecuteXmlReaderAll(reader => Assert.Fail("Shouldnt reach this point"), inputVal, inputOutput, output);
        }

        [TestMethod]
        public async Task ExecuteXmlReaderAll_WithOutputParametersAndMultiOut_ExecutesSuccessfully()
        {
            SqlProgram<int, Out<int>, Out<int>> program =
                await SqlProgram<int, Out<int>, Out<int>>.Create(
                    new LoadBalancedConnection(LocalDatabaseConnectionString, LocalDatabaseCopyConnectionString),
                    "spOutputParameters");

            const int inputVal = 123;
            const int inputOutputVal = 321;

            MultiOut<int> inputOutput = new MultiOut<int>(inputOutputVal);
            MultiOut<int> output = new MultiOut<int>();

            XElement[] result = program.ExecuteXmlReaderAll(XElement.Load, inputVal, inputOutput, output).ToArray();
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
