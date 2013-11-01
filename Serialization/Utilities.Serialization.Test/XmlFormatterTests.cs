#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Copyright (c) 2012, Web Applications UK Ltd
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Web Applications UK Ltd nor the
//       names of its contributors may be used to endorse or promote products
//       derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL WEB APPLICATIONS UK LTD BE LIABLE FOR ANY
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebApplications.Utilities.Serialization.Test
{
    [TestClass]
    public class XmlFormatterTests : SerializationTestBase
    {
        #region Classes used to test serialization of various structures

        #region Nested type: NotSerializableButImplementsISerializableTestClass
        private class NotSerializableButImplementsISerializableTestClass : ISerializable
        {
            #region ISerializable Members
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("property", "value");
            }
            #endregion
        }
        #endregion

        #region Nested type: SerializableAndImplementsISerializableGenericTestClass
        [Serializable]
        private class SerializableAndImplementsISerializableGenericTestClass<T> : ISerializable
        {
            private readonly T Property;

            public SerializableAndImplementsISerializableGenericTestClass(T property)
            {
                Property = property;
            }

            #region ISerializable Members
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("property", Property);
            }
            #endregion
        }
        #endregion

        #region Nested type: SerializableAndImplementsISerializableTestClass
        [Serializable]
        private class SerializableAndImplementsISerializableTestClass : ISerializable
        {
            private readonly String PropertyName;
            private readonly Object PropertyValue;

            public SerializableAndImplementsISerializableTestClass(String propertyName, object propertyValue)
            {
                PropertyName = propertyName;
                PropertyValue = propertyValue;
            }

            #region ISerializable Members
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue(PropertyName, PropertyValue);
            }
            #endregion
        }
        #endregion

        #endregion

        #region Helpers
        private XmlDocument SerializeWithXmlFormatterAndReturnXml(object obj)
        {
            IFormatter formatter = Serialize.GetXmlFormatter();
            Stream stream = new MemoryStream();
            formatter.Serialize(stream, obj);
            stream.Seek(0, SeekOrigin.Begin);
            string xmlStr;
            using (StreamReader reader = new StreamReader(stream))
            {
                xmlStr = reader.ReadToEnd();
                Trace.WriteLine(xmlStr);
            }
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlStr);
            return xml;
        }

        private object GenerateSerializableAndImplementsISerializableTestClass(out String propertyName,
                                                                               out int propertyValue)
        {
            propertyName = String.Format("property{0}", Random.Next());
            propertyValue = Random.Next();
            return new SerializableAndImplementsISerializableTestClass(propertyName, propertyValue);
        }

        // TODO what if property contains invalid chars?
        #endregion

        [TestMethod]
        [ExpectedException(typeof (SerializationException))]
        public void Serialize_NotSerializableButImplementsISerializableTestClass_ThrowsSerializationException()
        {
            SerializeWithXmlFormatterAndReturnXml(new NotSerializableButImplementsISerializableTestClass());
        }

        [TestMethod]
        public void Serialize_SerializableAndImplementsISerializableTestClass_ReturnsValidXml()
        {
            String propertyName;
            int propertyValue;
            Object testObject = GenerateSerializableAndImplementsISerializableTestClass(out propertyName,
                                                                                        out propertyValue);
            SerializeWithXmlFormatterAndReturnXml(testObject);
        }

        [TestMethod]
        public void Serialize_SerializableAndImplementsISerializableTestClass_FirstChildHasTypeAttributeMatchingTypeName
            ()
        {
            String className = typeof (SerializableAndImplementsISerializableTestClass).Name;
            String propertyName;
            int propertyValue;
            Object testObject = GenerateSerializableAndImplementsISerializableTestClass(out propertyName,
                                                                                        out propertyValue);
            XmlDocument xml = SerializeWithXmlFormatterAndReturnXml(testObject);
            Assert.IsNotNull(xml.FirstChild.Attributes);
            Assert.AreEqual(className,
                            xml.FirstChild.Attributes.GetNamedItem("type", "http://www.w3.org/2001/XMLSchema-instance").
                                Value);
        }

        [TestMethod]
        // TODO XML Currently uses type name as node name which causes this to fail as it will include a `1 for generics which is invalid in node name.
        public void
            Serialize_SerializableAndImplementsISerializableGenericTestClass_FirstChildHasTypeAttributeMatchingTypeName()
        {
            String className = typeof (SerializableAndImplementsISerializableGenericTestClass<int>).Name;
            Object testObject = new SerializableAndImplementsISerializableGenericTestClass<int>(Random.Next());
            XmlDocument xml = SerializeWithXmlFormatterAndReturnXml(testObject);
            Assert.IsNotNull(xml.FirstChild.Attributes);
            Assert.AreEqual(className,
                            xml.FirstChild.Attributes.GetNamedItem("type", "http://www.w3.org/2001/XMLSchema-instance").
                                Value);
        }

        [TestMethod]
        public void Serialize_SerializableAndImplementsISerializableTestClass_ContainsNodeMatchingSerializedPropertyName
            ()
        {
            String propertyName;
            int propertyValue;
            Object testObject = GenerateSerializableAndImplementsISerializableTestClass(out propertyName,
                                                                                        out propertyValue);
            XmlDocument xml = SerializeWithXmlFormatterAndReturnXml(testObject);
            Assert.AreEqual(1, xml.GetElementsByTagName(propertyName).Count,
                            "The serialized output of an ISerializable should contain a node whose tag name matches the name of the SerializationInfo property.");
        }

        [TestMethod]
        public void
            Serialize_SerializableAndImplementsISerializableTestClass_InnerTextOfPropertyNodeMatchesStringEquivilantOfPropertyValue
            ()
        {
            String propertyName;
            int propertyValue;
            Object testObject = GenerateSerializableAndImplementsISerializableTestClass(out propertyName,
                                                                                        out propertyValue);
            XmlDocument xml = SerializeWithXmlFormatterAndReturnXml(testObject);
            XmlNode propertyNode = xml.GetElementsByTagName(propertyName).Item(0);
            Assert.IsNotNull(propertyNode); // This should have already been covered by other tests
            Assert.AreEqual(propertyValue.ToString(CultureInfo.InvariantCulture), propertyNode.InnerText,
                            "The serialized output of an ISerializable should contain a node whose value matches the value of the SerializationInfo property.");
        }
    }
}