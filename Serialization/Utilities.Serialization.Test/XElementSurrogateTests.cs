using System;
using System.Runtime.Serialization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;

namespace WebApplications.Utilities.Serialization.Test
{
    [TestClass]
    public class XElementSurrogateTests : SerializationTestBase
    {
        [TestMethod]
        public void XElementSurrogate_Implements_ISerializationSurrogate()
        {
            XElementSurrogate instance = new XElementSurrogate();
            Assert.IsInstanceOfType(instance, typeof(ISerializationSurrogate));
        }

        private static String GetDataValueFromGetObjectData(XElement element)
        {
            SerializationInfo info = new SerializationInfo(typeof(XElementSurrogate), new FormatterConverter());
            ISerializationSurrogate surrogate = new XElementSurrogate();
            surrogate.GetObjectData(element, info, new StreamingContext());
            return info.GetString("data");
        }

        [TestMethod]
        public void GetObjectData_NullObject_SetsDataValueOfInfoToNull()
        {
            String data = GetDataValueFromGetObjectData(null);
            Assert.IsNull(data);
        }

        [TestMethod]
        public void GetObjectData_EmptyXElement_ReturnsSameAsToStringCalledWithDisableFormatting()
        {
            XElement element = new XElement("name");
            String data = GetDataValueFromGetObjectData(element);
            String toStringResult = element.ToString(SaveOptions.DisableFormatting);
            Assert.AreEqual(toStringResult, data);
        }

        [TestMethod]
        public void GetObjectData_XElementWithAttribute_ReturnsSameAsToStringCalledWithDisableFormatting()
        {
            XElement element = new XElement("name");
            element.SetAttributeValue("attrName", Random.RandomString());
            String data = GetDataValueFromGetObjectData(element);
            String toStringResult = element.ToString(SaveOptions.DisableFormatting);
            Assert.AreEqual(toStringResult, data);
        }

        [TestMethod]
        public void GetObjectData_XElementWithChildNodes_ReturnsSameAsToStringCalledWithDisableFormatting()
        {
            XElement element = new XElement("name");
            element.Add(new XElement("child"));
            String data = GetDataValueFromGetObjectData(element);
            String toStringResult = element.ToString(SaveOptions.DisableFormatting);
            Assert.AreEqual(toStringResult, data);
        }

        private static XElement SetObjectDataWithDataValue(String data)
        {
            SerializationInfo info = new SerializationInfo(typeof(XElementSurrogate), new FormatterConverter());
            ISerializationSurrogate surrogate = new XElementSurrogate();
            info.AddValue("data", data);
            return (XElement)surrogate.SetObjectData(default(XElement), info, new StreamingContext(), new SurrogateSelector());
        }

        [TestMethod]
        public void SetObjectData_EmptyXElement_ReturnsXElementWithSameName()
        {
            String elementName = String.Format("Name{0}", Random.RandomString());
            XElement element = new XElement(elementName);
            String data = GetDataValueFromGetObjectData(element);
            XElement result = SetObjectDataWithDataValue(data);
            Assert.AreEqual(elementName, result.Name);
        }

        [TestMethod]
        public void SetObjectData_XElementWithAttribute_ReturnsXElementWithAttribute()
        {
            String elementName = String.Format("Name{0}", Random.Next());
            XElement element = new XElement(elementName);
            String value = Random.RandomString();
            element.SetAttributeValue("attrName", value);
            String data = GetDataValueFromGetObjectData(element);
            XElement result = SetObjectDataWithDataValue(data);
            var attr = result.Attribute("attrName");
            Assert.IsNotNull(attr);
            Assert.AreEqual(value, attr.Value);
        }

        [TestMethod]
        public void SetObjectData_XElementWithChildren_ReturnsXElementWithCorrectNumberOfChildren()
        {
            String elementName = String.Format("Name{0}", Random.Next());
            XElement element = new XElement(elementName);
            int childCount = Random.Next(1, 10);
            for (int i = 0; i < childCount; i++)
                element.Add(new XElement("child"));
            String data = GetDataValueFromGetObjectData(element);
            XElement result = SetObjectDataWithDataValue(data);
            Assert.AreEqual(childCount, result.Nodes().Count());
        }

        [TestMethod]
        [ExpectedException(typeof(XmlException))]
        public void SetObjectData_DataValueContainsInvalidXml_ThrowsXmlException()
        {
            XElement result = SetObjectDataWithDataValue("invalid XML");
        }
    }
}
