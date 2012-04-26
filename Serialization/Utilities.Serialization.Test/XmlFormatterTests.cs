using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;
using WebApplications.Utilities.Serialization;

namespace WebApplications.Utilities.Test.Serialization
{
    [TestClass]
    public class XmlFormatterTests : TestBase
    {

        #region Classes used to test serialization of various structures

        class NotSerializableButImplementsISerializableTestClass : ISerializable
        {
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("property", "value");
            }
        }

        [Serializable]
        class SerializableAndImplementsISerializableTestClass : ISerializable
        {
            private readonly String PropertyName;
            private readonly Object PropertyValue;

            public SerializableAndImplementsISerializableTestClass(String propertyName, object propertyValue)
            {
                PropertyName = propertyName;
                PropertyValue = propertyValue;
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue(PropertyName, PropertyValue);
            }
        }

        [Serializable]
        class SerializableAndImplementsISerializableGenericTestClass<T> : ISerializable
        {
            private readonly T Property;

            public SerializableAndImplementsISerializableGenericTestClass(T property)
            {
                Property = property;
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("property", Property);
            }
        }

        #endregion

        #region Helpers

        private XmlDocument SerializeWithXmlFormatterAndReturnXml( object obj )
        {
            IFormatter formatter = Serialize.GetXmlFormatter();
            Stream stream = new MemoryStream();
            formatter.Serialize(stream, obj);
            stream.Seek(0, SeekOrigin.Begin);
            XmlDocument xml = new XmlDocument();
            using (StreamReader reader = new StreamReader(stream))
            {
                xml.LoadXml(reader.ReadToEnd());
            }
            return xml;
        }

        private object GenerateSerializableAndImplementsISerializableTestClass(out String propertyName, out int propertyValue )
        {
            propertyName = String.Format("property{0}", Random.Next());
            propertyValue = Random.Next();
            return new SerializableAndImplementsISerializableTestClass(propertyName, propertyValue);
        } // TODO what if property contains invalid chars?

        #endregion

        [TestMethod]
        [ExpectedException(typeof(SerializationException))]
        public void Serialize_NotSerializableButImplementsISerializableTestClass_ThrowsSerializationException()
        {
            SerializeWithXmlFormatterAndReturnXml(new NotSerializableButImplementsISerializableTestClass());
        }

        [TestMethod]
        public void Serialize_SerializableAndImplementsISerializableTestClass_ReturnsValidXml()
        {
            String propertyName;
            int propertyValue;
            Object testObject = GenerateSerializableAndImplementsISerializableTestClass(out propertyName, out propertyValue);
            SerializeWithXmlFormatterAndReturnXml(testObject);
        }

        [TestMethod]
        public void Serialize_SerializableAndImplementsISerializableTestClass_FirstChildHasTypeAttributeMatchingTypeName()
        {
            String className = typeof(SerializableAndImplementsISerializableTestClass).Name;
            String propertyName;
            int propertyValue;
            Object testObject = GenerateSerializableAndImplementsISerializableTestClass(out propertyName, out propertyValue);
            XmlDocument xml = SerializeWithXmlFormatterAndReturnXml(testObject);
            Assert.IsNotNull(xml.FirstChild.Attributes);
            Assert.AreEqual(className, xml.FirstChild.Attributes.GetNamedItem("type", "http://www.w3.org/2001/XMLSchema-instance").Value);
        }

        [TestMethod]
        public void Serialize_SerializableAndImplementsISerializableGenericTestClass_FirstChildHasTypeAttributeMatchingTypeName()
        {
            String className = typeof(SerializableAndImplementsISerializableGenericTestClass<int>).Name;
            Object testObject = new SerializableAndImplementsISerializableGenericTestClass<int>(Random.Next());
            XmlDocument xml = SerializeWithXmlFormatterAndReturnXml(testObject);
            Assert.IsNotNull(xml.FirstChild.Attributes);
            Assert.AreEqual(className, xml.FirstChild.Attributes.GetNamedItem("type", "http://www.w3.org/2001/XMLSchema-instance").Value);
        }

        [TestMethod]
        public void Serialize_SerializableAndImplementsISerializableTestClass_ContainsNodeMatchingSerializedPropertyName()
        {
            String propertyName;
            int propertyValue;
            Object testObject = GenerateSerializableAndImplementsISerializableTestClass(out propertyName, out propertyValue);
            XmlDocument xml = SerializeWithXmlFormatterAndReturnXml(testObject);
            Assert.AreEqual(1, xml.GetElementsByTagName(propertyName).Count, "The serialized output of an ISerializable should contain a node whose tag name matches the name of the SerializationInfo property.");
        }

        [TestMethod]
        public void Serialize_SerializableAndImplementsISerializableTestClass_InnerTextOfPropertyNodeMatchesStringEquivilantOfPropertyValue()
        {
            String propertyName;
            int propertyValue;
            Object testObject = GenerateSerializableAndImplementsISerializableTestClass(out propertyName, out propertyValue);
            XmlDocument xml = SerializeWithXmlFormatterAndReturnXml(testObject);
            XmlNode propertyNode = xml.GetElementsByTagName(propertyName).Item(0);
            Assert.IsNotNull(propertyNode); // This should have already been covered by other tests
            Assert.AreEqual(propertyValue.ToString(CultureInfo.InvariantCulture), propertyNode.InnerText, "The serialized output of an ISerializable should contain a node whose value matches the value of the SerializationInfo property.");
        }
    }
}
