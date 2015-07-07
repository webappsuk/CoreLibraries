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
using System.Linq;
using System.Runtime.Serialization;
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
            Assert.IsInstanceOfType(instance, typeof (ISerializationSurrogate));
        }

        private static String GetDataValueFromGetObjectData(XElement element)
        {
            SerializationInfo info = new SerializationInfo(typeof (XElementSurrogate), new FormatterConverter());
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
            SerializationInfo info = new SerializationInfo(typeof (XElementSurrogate), new FormatterConverter());
            ISerializationSurrogate surrogate = new XElementSurrogate();
            info.AddValue("data", data);
            return
                (XElement)
                surrogate.SetObjectData(default(XElement), info, new StreamingContext(), new SurrogateSelector());
        }

        [TestMethod]
        public void SetObjectData_EmptyXElement_ReturnsXElementWithSameName()
        {
            String elementName = String.Format("Name{0}", Random.Next());
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
            XAttribute attr = result.Attribute("attrName");
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
        [ExpectedException(typeof (XmlException))]
        public void SetObjectData_DataValueContainsInvalidXml_ThrowsXmlException()
        {
            XElement result = SetObjectDataWithDataValue("invalid XML");
        }
    }
}