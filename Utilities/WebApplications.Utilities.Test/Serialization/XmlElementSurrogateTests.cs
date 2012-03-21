using System;
using System.Runtime.Serialization;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebApplications.Testing;
using WebApplications.Utilities.Serialization;

namespace WebApplications.Utilities.Test.Serialization
{
    [TestClass]
    public class XmlElementSurrogateTests : TestBase
    {
        [TestMethod]
        public void XmlElementSurrogate_Implements_ISerializationSurrogate()
        {
            XmlElementSurrogate instance = new XmlElementSurrogate();
            Assert.IsInstanceOfType(instance, typeof(ISerializationSurrogate) );
        }

        private static String GetDataValueFromGetObjectData( XmlElement element )
        {
            SerializationInfo info = new SerializationInfo(typeof(XmlElementSurrogate), new FormatterConverter());
            ISerializationSurrogate surrogate = new XmlElementSurrogate();
            surrogate.GetObjectData(element, info, new StreamingContext());
            return info.GetString("data");
        }

        [TestMethod]
        public void GetObjectData_NullObject_SetsDataValueOfInfoToNull()
        {
            String data = GetDataValueFromGetObjectData(null);
            Assert.IsNull(data);
        }

        private static XmlElement CreateXmlElementFromString( String xml )
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc.DocumentElement;
        }

        [TestMethod]
        public void GetObjectData_EmptyNode_SetsDataValueOfInfoToOuterXmlOfNode()
        {
            XmlElement element = CreateXmlElementFromString("<node/>");

            String data = GetDataValueFromGetObjectData(element);
            Assert.AreEqual(element.OuterXml, data);
        }

        [TestMethod]
        public void GetObjectData_EmptyNodeWithAttributes_SetsDataValueOfInfoToOuterXmlOfNode()
        {
            XmlElement element = CreateXmlElementFromString("<node attr=\"some value\"/>");

            String data = GetDataValueFromGetObjectData(element);
            Assert.AreEqual(element.OuterXml, data);
        }

        [TestMethod]
        public void GetObjectData_NodeContainingText_SetsDataValueOfInfoToOuterXmlOfNode()
        {
            XmlElement element = CreateXmlElementFromString("<node>some text</node>");

            String data = GetDataValueFromGetObjectData(element);
            Assert.AreEqual(element.OuterXml, data);
        }

        [TestMethod]
        public void GetObjectData_NodeContainingChildNodes_SetsDataValueOfInfoToOuterXmlOfNode()
        {
            XmlElement element = CreateXmlElementFromString("<node><child/><child/></node>");

            String data = GetDataValueFromGetObjectData(element);
            Assert.AreEqual(element.OuterXml, data);
        }

        [TestMethod]
        public void SetObjectData_NullDataValue_ReturnsNull()
        {
            String data = GetDataValueFromGetObjectData(null);
            Assert.IsNull(data);
        }

        private static XmlElement SetObjectDataWithDataValue( String data )
        {
            SerializationInfo info = new SerializationInfo(typeof(XmlElementSurrogate), new FormatterConverter());
            ISerializationSurrogate surrogate = new XmlElementSurrogate();
            info.AddValue("data",data);
            return (XmlElement) surrogate.SetObjectData(default(XmlElement),info,new StreamingContext(),new SurrogateSelector());
        }

        [TestMethod]
        public void SetObjectData_DataValueContainsXmlForEmptyNode_ReturnsSingleNodeOfCorrectName()
        {
            XmlElement element = SetObjectDataWithDataValue("<node/>");
            Assert.AreEqual("node", element.Name);
            Assert.AreEqual(0, element.ChildNodes.Count);
        }

        [TestMethod]
        public void SetObjectData_DataValueContainsXmlForNodeWithAttribute_ReturnsNodeWithCorrectAttribute()
        {
            XmlElement element = SetObjectDataWithDataValue("<node attr=\"some value\"/>");
            Assert.AreEqual("some value", element.GetAttribute("attr"));
        }

        [TestMethod]
        public void SetObjectData_DataValueContainsXmlForNodeWithTwoChildren_ReturnsNodeWithCorrectNumberOfChildren()
        {
            XmlElement element = SetObjectDataWithDataValue("<node><child/><child/></node>");
            Assert.AreEqual(2, element.ChildNodes.Count);
        }

        [TestMethod]
        public void SetObjectData_DataValueContainsXmlForNodeContainingText_ReturnsSingleNodeContainingText()
        {
            XmlElement element = SetObjectDataWithDataValue("<node>some text &amp; an entity</node>");
            Assert.AreEqual("some text & an entity", element.InnerText);
        }

        [TestMethod]
        [ExpectedException(typeof(XmlException))]
        public void SetObjectData_DataValueContainsInvalidXml_ThrowsXmlException()
        {
            XmlElement element = SetObjectDataWithDataValue("invalid xml");
        }
    }
}
