using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebApplications.Testing;

namespace WebApplications.Utilities.Serialization.Test
{
    [TestClass]
    public class SerializeTests : SerializationTestBase
    {
        // Warning: As the class under tests has many static methods and fields, test order will inevitably affect test results. Bear this in mind if adding new tests.

        private class SurrogateTestClass : ISerializationSurrogate
        {
            public void GetObjectData(object o, SerializationInfo info, StreamingContext context) { }
            public object SetObjectData(object o, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                return o;
            }
        }

        private class NonSurrogateTestClass
        {
        }

        private class TestClass { }
        private class TestClass1 { }
        private class TestClass2 { }
        private class TestClass3 { }
        private class TestClass4 { }
        private class TestClass5 { }

        private static List<string> GenerateRandomListOfString()
        {
            return Enumerable.Range(10, Random.Next(10,30)).Select(n => Random.RandomString(n)).ToList();
        }

        private static bool CompareStreamData(Stream stream, byte[] data)
        {
            return (data.All(b => b == stream.ReadByte()));
        }

        private static bool CompareByteArray(byte[] data1, byte[] data2)
        {
            return data1.Zip(data2, (a, b) => a == b).All(x=>x);
        }

        [TestMethod]
        public void SurrogateSelector_InitialState_ContainsXmlElementSurrogate()
        {
            ISurrogateSelector selector;
            Assert.IsInstanceOfType(Serialize.SurrogateSelector.GetSurrogate(typeof(XmlElement), new StreamingContext(), out selector), typeof(XmlElementSurrogate));
        }

        [TestMethod]
        public void SurrogateSelector_InitialState_ContainsXElementSurrogate()
        {
            ISurrogateSelector selector;
            Assert.IsInstanceOfType(Serialize.SurrogateSelector.GetSurrogate(typeof(XElement), new StreamingContext(), out selector), typeof(XElementSurrogate));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddOrUpdateSurrogate_NullType_ThrowsArgumentNullException()
        {
            // ReSharper disable AssignNullToNotNullAttribute
            Serialize.AddOrUpdateSurrogate(null, typeof(SurrogateTestClass));
            // ReSharper restore AssignNullToNotNullAttribute
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddOrUpdateSurrogate_NullSurrogate_ThrowsArgumentNullException()
        {
            // ReSharper disable AssignNullToNotNullAttribute
            Serialize.AddOrUpdateSurrogate(typeof(TestClass), null);
            // ReSharper restore AssignNullToNotNullAttribute
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AddOrUpdateSurrogate_InterfacePassedAsSurrogate_ThrowsArgumentOutOfRangeException()
        {
            Serialize.AddOrUpdateSurrogate(typeof(TestClass), typeof(ISerializationSurrogate));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AddOrUpdateSurrogate_SurrogateNotISerializationSurrogate_ThrowsArgumentOutOfRangeException()
        {
            Serialize.AddOrUpdateSurrogate(typeof(TestClass), typeof(NonSurrogateTestClass));
        }

        [TestMethod]
        public void SurrogateSelector_SetUsingTypeSafeFormOfAddOrUpdateSurrogate_ContainsAddedSurrogates()
        {
            Serialize.AddOrUpdateSurrogate<TestClass1>(typeof(SurrogateTestClass));
            ISurrogateSelector selector;
            Assert.IsInstanceOfType(Serialize.SurrogateSelector.GetSurrogate(typeof(TestClass1), new StreamingContext(), out selector), typeof(SurrogateTestClass));
        }

        [TestMethod]
        public void SurrogateSelector_SetUsingFullyTypeSafeFormOfAddOrUpdateSurrogate_ContainsAddedSurrogates()
        {
            Serialize.AddOrUpdateSurrogate<TestClass2,SurrogateTestClass>();
            ISurrogateSelector selector;
            Assert.IsInstanceOfType(Serialize.SurrogateSelector.GetSurrogate(typeof(TestClass2), new StreamingContext(), out selector), typeof(SurrogateTestClass));
        }

        [TestMethod]
        public void SurrogateSelector_SetUsingOrdinaryFormOfAddOrUpdateSurrogate_ContainsAddedSurrogates()
        {
            Serialize.AddOrUpdateSurrogate(typeof(TestClass3),typeof(SurrogateTestClass));
            ISurrogateSelector selector;
            Assert.IsInstanceOfType(Serialize.SurrogateSelector.GetSurrogate(typeof(TestClass3), new StreamingContext(), out selector), typeof(SurrogateTestClass));
        }

        [TestMethod]
        public void GetFormatter_SurrogatePreviouslyAdded_SurrogateExistsInSurrogateSelectorOfResult()
        {
            Serialize.AddOrUpdateSurrogate(typeof(TestClass4), typeof(SurrogateTestClass));
            BinaryFormatter formatter = Serialize.GetFormatter();
            ISurrogateSelector selector;
            Assert.IsInstanceOfType(formatter.SurrogateSelector.GetSurrogate(typeof(TestClass4), new StreamingContext(), out selector), typeof(SurrogateTestClass));
        }

        [TestMethod]
        public void GetFormatter_DefaultParams_AssemblyFormatOfResultIsSimple()
        {
            BinaryFormatter formatter = Serialize.GetFormatter();
            Assert.AreEqual(FormatterAssemblyStyle.Simple, formatter.AssemblyFormat);
        }

        [TestMethod]
        public void GetFormatter_DefaultParams_BinderOfResultIsDefault()
        {
            BinaryFormatter formatter = Serialize.GetFormatter();
            Assert.AreEqual(ExtendedSerializationBinder.Default, formatter.Binder);
        }

        [TestMethod]
        public void GetFormatter_ContextNotSupplied_NullContextIsUsed()
        {
            BinaryFormatter formatter = Serialize.GetFormatter();
            Assert.AreEqual(new StreamingContext(StreamingContextStates.Other, null), formatter.Context);
        }

        [TestMethod]
        public void GetFormatter_ContextSupplied_ContextIsUsed()
        {
            object context = Random.Next();
            BinaryFormatter formatter = Serialize.GetFormatter(context);
            Assert.AreEqual(new StreamingContext(StreamingContextStates.Other, context), formatter.Context);
        }

        [TestMethod]
        public void GetXmlFormatter_SurrogatePreviouslyAdded_SurrogateExistsInSurrogateSelectorOfResult()
        {
            Serialize.AddOrUpdateSurrogate(typeof(TestClass5), typeof(SurrogateTestClass));
            XmlFormatter formatter = Serialize.GetXmlFormatter();
            ISurrogateSelector selector;
            Assert.IsInstanceOfType(formatter.SurrogateSelector.GetSurrogate(typeof(TestClass5), new StreamingContext(), out selector), typeof(SurrogateTestClass));
        }

        [TestMethod]
        public void GetXmlFormatter_DefaultParams_BinderOfResultIsDefault()
        {
            XmlFormatter formatter = Serialize.GetXmlFormatter();
            Assert.AreEqual(ExtendedSerializationBinder.Default, formatter.Binder);
        }

        [TestMethod]
        public void GetXmlFormatter_ContextNotSupplied_NullContextIsUsed()
        {
            XmlFormatter formatter = Serialize.GetXmlFormatter();
            Assert.AreEqual(new StreamingContext(StreamingContextStates.Other, null), formatter.Context);
        }

        [TestMethod]
        public void GetXmlFormatter_ContextSupplied_ContextIsUsed()
        {
            object context = Random.Next();
            XmlFormatter formatter = Serialize.GetXmlFormatter(context);
            Assert.AreEqual(new StreamingContext(StreamingContextStates.Other, context), formatter.Context);
        }

        private static readonly Regex ValidBase64 = new Regex("^[a-zA-Z0-9+/=]*$");

        [TestMethod]
        public void AppendSerialization_ListOfIntsInput_StringBuilderContainsValidBase64()
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<int> listOfInts = Enumerable.Range(Random.Next(1, 1000), Random.Next(10, 1000)).ToList();
            stringBuilder.AppendSerialization(listOfInts);
            String result = stringBuilder.ToString();
            Assert.AreNotEqual("", result);
            Assert.IsTrue(ValidBase64.IsMatch(result));
        }

        [TestMethod]
        public void AppendSerialization_UnicodeStringInputAsEnumerable_StringBuilderContainsValidBase64()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendSerialization(Random.RandomString());
            String result = stringBuilder.ToString();
            Assert.AreNotEqual("", result);
            Assert.IsTrue(ValidBase64.IsMatch(result));
        }

        [TestMethod]
        public void AppendSerialization_UnicodeStringsInputAsObject_StringBuilderContainsValidBase64()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendSerialization((Object)Random.RandomString());
            String result = stringBuilder.ToString();
            Assert.AreNotEqual("", result);
            Assert.IsTrue(ValidBase64.IsMatch(result));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AppendSerialization_NullInput_ThrowsArgumentNullException()
        {
            StringBuilder stringBuilder = new StringBuilder();
            // ReSharper disable AssignNullToNotNullAttribute
            stringBuilder.AppendSerialization(null);
            // ReSharper restore AssignNullToNotNullAttribute
            String result = stringBuilder.ToString();
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void AppendSerialization_CustomFormatter_SerializeMethodOfFormatterCalledWithObj()
        {
            object obj = Random.RandomString();
            var mockFormatter = new Mock<IFormatter>();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendSerialization(obj, formatter: mockFormatter.Object);
            String result = stringBuilder.ToString();
            mockFormatter.Verify(f => f.Serialize(It.IsAny<Stream>(), obj), Times.Once());
        }

        [TestMethod]
        public void AppendSerialization_CustomFormatterForEnumerable_SerializeMethodOfFormatterCalledWithEnumerableToList()
        {
            String[] list = GenerateRandomListOfString().ToArray();
            IEnumerable<String> enumerable = list;

            var mockFormatter = new Mock<IFormatter>();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendSerialization(enumerable, formatter: mockFormatter.Object);
            String result = stringBuilder.ToString();
            mockFormatter.Verify(f => f.Serialize(It.IsAny<Stream>(), It.IsAny<List<String>>()), Times.Once(), "The IEnumerable should be converted to a list before being sent to the formatter.");
            mockFormatter.Verify(f => f.Serialize(It.IsAny<Stream>(), list), Times.Once());
        }

        [TestMethod]
        public void Deserialize_ResultFromAppendSerialization_ReturnsOriginalObject()
        {
            List<String> source = GenerateRandomListOfString();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendSerialization(source);
            String serializedString = stringBuilder.ToString();

            List<String> result = serializedString.Deserialize<List<String>>();
            Assert.IsNotNull(result);
            Assert.AreEqual(String.Join(",", source), String.Join(",", result));
        }

        [TestMethod]
        public void Deserialize_CustomFormatterAndByteArrayData_DeserializeMethodOfFormatterCalledWithData()
        {
            byte[] data = Enumerable.Range(1, Random.Next(5, 100)).Select(n => (byte)Random.Next(0, 256)).ToArray();
            var mockFormatter = new Mock<IFormatter>();
            // We must compare the stream immediately, and so we return the result of the comparison as the deserialized object to confirm at the end
            mockFormatter.Setup(f => f.Deserialize(It.IsAny<Stream>())).Returns((Stream s) => CompareStreamData(s, data));
            bool result = data.Deserialize<bool>(mockFormatter.Object);
            Assert.IsTrue(result); // This is the result of the comparison operation
        }

        [TestMethod]
        public void Deserialize_CustomFormatterAndBase64EncodedData_DeserializeMethodOfFormatterCalledWithUnencodedData()
        {
            byte[] data = Enumerable.Range(1, Random.Next(5, 100)).Select(n => (byte)Random.Next(0, 256)).ToArray();
            String base64EncodedData = Convert.ToBase64String(data);
            var mockFormatter = new Mock<IFormatter>();
            // We must compare the stream immediately, and so we return the result of the comparison as the deserialized object to confirm at the end
            mockFormatter.Setup(f => f.Deserialize(It.IsAny<Stream>())).Returns((Stream s) => CompareStreamData(s, data));
            bool result = base64EncodedData.Deserialize<bool>(mockFormatter.Object);
            Assert.IsTrue(result); // This is the result of the comparison operation
        }

        [TestMethod]
        public void SerializeToStringBuilder_ObjectInput_ResultContainsValidBase64()
        {
            object obj = (Object)GenerateRandomListOfString();
            StringBuilder stringBuilder = obj.SerializeToStringBuilder();
            String result = stringBuilder.ToString();
            Assert.IsTrue(ValidBase64.IsMatch(result));
        }

        [TestMethod]
        public void SerializeToStringBuilder_CustomFormatterAndObjectInput_SerializeMethodOfFormatterCalledWithObj()
        {
            object obj = (Object)GenerateRandomListOfString();
            var mockFormatter = new Mock<IFormatter>();
            StringBuilder stringBuilder = obj.SerializeToStringBuilder(mockFormatter.Object);
            String result = stringBuilder.ToString();
            mockFormatter.Verify(f => f.Serialize(It.IsAny<Stream>(), obj), Times.Once());
        }

        [TestMethod]
        public void SerializeToStringBuilder_EnumerationInput_ResultContainsValidBase64()
        {
            IEnumerable<String> enumerable = GenerateRandomListOfString();
            StringBuilder stringBuilder = enumerable.SerializeToStringBuilder();
            String result = stringBuilder.ToString();
            Assert.IsTrue(ValidBase64.IsMatch(result));
        }

        [TestMethod]
        public void SerializeToStringBuilder_CustomFormatterAndEnumerationInput_SerializeMethodOfFormatterCalledWithObj()
        {
            IEnumerable<String> enumerable = GenerateRandomListOfString().ToArray();
            var mockFormatter = new Mock<IFormatter>();
            StringBuilder stringBuilder = enumerable.SerializeToStringBuilder(mockFormatter.Object);
            String result = stringBuilder.ToString();
            mockFormatter.Verify(f => f.Serialize(It.IsAny<Stream>(), It.IsAny<List<String>>()), Times.Once(), "The IEnumerable should be converted to a list before being sent to the formatter.");
            mockFormatter.Verify(f => f.Serialize(It.IsAny<Stream>(), enumerable), Times.Once());
        }

        [TestMethod]
        public void SerializeToString_ObjectInput_ResultContainsValidBase64()
        {
            object obj = (Object)GenerateRandomListOfString();
            String result = obj.SerializeToString();
            Assert.IsTrue(ValidBase64.IsMatch(result));
        }

        [TestMethod]
        public void SerializeToString_CustomFormatterAndObjectInput_SerializeMethodOfFormatterCalledWithObj()
        {
            object obj = (Object)GenerateRandomListOfString();
            var mockFormatter = new Mock<IFormatter>();
            String result = obj.SerializeToString(mockFormatter.Object);
            mockFormatter.Verify(f => f.Serialize(It.IsAny<Stream>(), obj), Times.Once());
        }

        [TestMethod]
        public void SerializeToString_EnumerationInput_ResultContainsValidBase64()
        {
            IEnumerable<String> enumerable = GenerateRandomListOfString();
            String result = enumerable.SerializeToString();
            Assert.IsTrue(ValidBase64.IsMatch(result));
        }

        [TestMethod]
        public void SerializeToString_CustomFormatterAndEnumerationInput_SerializeMethodOfFormatterCalledWithObjConvertedToList()
        {
            IEnumerable<String> enumerable = GenerateRandomListOfString().ToArray();
            var mockFormatter = new Mock<IFormatter>();
            String result = enumerable.SerializeToString(mockFormatter.Object);
            mockFormatter.Verify(f => f.Serialize(It.IsAny<Stream>(), It.IsAny<List<String>>()), Times.Once(), "The IEnumerable should be converted to a list before being sent to the formatter.");
            mockFormatter.Verify(f => f.Serialize(It.IsAny<Stream>(), enumerable), Times.Once());
        }

        [TestMethod]
        public void SerializeToString_CustomFormatterAndObjectInput_ReturnsBase64EncodedOutputFromFormatter()
        {
            object obj = (Object)GenerateRandomListOfString();
            int dataLength = Random.Next(10, 20);
            byte[] data = Enumerable.Range(1, dataLength).Select(x => (byte)Random.Next(0, 256)).ToArray();
            var mockFormatter = new Mock<IFormatter>();
            mockFormatter.Setup(f => f.Serialize(It.IsAny<Stream>(), It.IsAny<object>())).Callback((Stream s, object o) => s.Write(data, 0, dataLength));
            String result = obj.SerializeToString(mockFormatter.Object);
            Assert.AreEqual(Convert.ToBase64String(data), result, "With SerializeToString, the output of the formatter should be base64 encoded and returned as the result.");
        }

        [TestMethod]
        public void SerializeToByteArray_ObjectInput_Base64EncodingOfResultMatchesSerializeToString()
        {
            object obj = (Object)GenerateRandomListOfString();
            String stringResult = obj.SerializeToString();
            String encodedByteResult = Convert.ToBase64String(obj.SerializeToByteArray());
            Assert.AreEqual(stringResult, encodedByteResult);
        }

        [TestMethod]
        public void SerializeToByteArray_IEnumerableInput_Base64EncodingOfResultMatchesSerializeToString()
        {
            IEnumerable<String> enumerable = GenerateRandomListOfString().ToArray();
            String stringResult = enumerable.SerializeToString();
            String encodedByteResult = Convert.ToBase64String(enumerable.SerializeToByteArray());
            Assert.AreEqual(stringResult, encodedByteResult);
        }

        [TestMethod]
        public void SerializeToByteArray_CustomFormatterAndObjectInput_SerializeMethodOfFormatterCalledWithObj()
        {
            object obj = (Object)GenerateRandomListOfString();
            var mockFormatter = new Mock<IFormatter>();
            Byte[] result = obj.SerializeToByteArray(mockFormatter.Object);
            mockFormatter.Verify(f => f.Serialize(It.IsAny<Stream>(), obj), Times.Once());
        }

        [TestMethod]
        public void SerializeToByteArray_CustomFormatterAndEnumerationInput_SerializeMethodOfFormatterCalledWithObjConvertedToList()
        {
            IEnumerable<String> enumerable = GenerateRandomListOfString().ToArray();
            var mockFormatter = new Mock<IFormatter>();
            Byte[] result = enumerable.SerializeToByteArray(mockFormatter.Object);
            mockFormatter.Verify(f => f.Serialize(It.IsAny<Stream>(), It.IsAny<List<String>>()), Times.Once(), "The IEnumerable should be converted to a list before being sent to the formatter.");
            mockFormatter.Verify(f => f.Serialize(It.IsAny<Stream>(), enumerable), Times.Once());
        }

        [TestMethod]
        public void SerializeToByteArray_CustomFormatterAndObjectInput_ReturnsOutputFromFormatter()
        {
            object obj = (Object)GenerateRandomListOfString();
            int dataLength = Random.Next(10, 20);
            byte[] data = Enumerable.Range(1, dataLength).Select(x => (byte)Random.Next(0, 256)).ToArray();
            var mockFormatter = new Mock<IFormatter>();
            mockFormatter.Setup(f => f.Serialize(It.IsAny<Stream>(), It.IsAny<object>())).Callback((Stream s, object o) => s.Write(data, 0, dataLength));
            Byte[] result = obj.SerializeToByteArray(mockFormatter.Object);
            Assert.IsTrue(CompareByteArray(data, result), "With SerializeToByteArray, the output of the formatter should be directly returned as the result.");
        }
    }
}
