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
using System.Runtime.Serialization;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;

namespace WebApplications.Utilities.Serialization.Test
{
    [TestClass]
    public class XmlElementSurrogateTests : TestBase
    {
        [TestMethod]
        public void XmlElementSurrogate_Implements_ISerializationSurrogate()
        {
            XmlElementSurrogate instance = new XmlElementSurrogate();
            Assert.IsInstanceOfType(instance, typeof (ISerializationSurrogate));
        }

        private static String GetDataValueFromGetObjectData(XmlElement element)
        {
            SerializationInfo info = new SerializationInfo(typeof (XmlElementSurrogate), new FormatterConverter());
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

        private static XmlElement CreateXmlElementFromString(String xml)
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

        private static XmlElement SetObjectDataWithDataValue(String data)
        {
            SerializationInfo info = new SerializationInfo(typeof (XmlElementSurrogate), new FormatterConverter());
            ISerializationSurrogate surrogate = new XmlElementSurrogate();
            info.AddValue("data", data);
            return
                (XmlElement)
                surrogate.SetObjectData(default(XmlElement), info, new StreamingContext(), new SurrogateSelector());
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
        [ExpectedException(typeof (XmlException))]
        public void SetObjectData_DataValueContainsInvalidXml_ThrowsXmlException()
        {
            XmlElement element = SetObjectDataWithDataValue("invalid xml");
        }
    }
}