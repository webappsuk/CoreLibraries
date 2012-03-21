#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities.Test
// File: TestSerialization.cs
// 
// This software, its object code and source code and all modifications made to
// the same (the “Software”) are, and shall at all times remain, the proprietary
// information and intellectual property rights of Web Applications (UK) Limited. 
// You are only entitled to use the Software as expressly permitted by Web
// Applications (UK) Limited within the Software Customisation and
// Licence Agreement (the “Agreement”).  Any copying, modification, decompiling,
// distribution, licensing, sale, transfer or other use of the Software other than
// as expressly permitted in the Agreement is expressly forbidden.  Web
// Applications (UK) Limited reserves its rights to take action against you and
// your employer in accordance with its contractual and common law rights
// (including injunctive relief) should you breach the terms of the Agreement or
// otherwise infringe its copyright or other intellectual property rights in the
// Software.
// 
// © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Serialization;

namespace WebApplications.Utilities.Test
{
    [TestClass]
    public class TestSerialization
    {
        [TestMethod]
        public void GetFormatter_LegacyTests()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<Test><SomeXml>Some Context</SomeXml><SomeMore/></Test>");
            XmlElement el = doc.DocumentElement;

            IFormatter formatter = Serialize.GetFormatter();
            TestSerializableObject(el, new XElement("ANode", 1), new XElement("BNode", "B"),
                                    new List<string> { "A", "B", "C" }, null, formatter);
            TestSerializableObject((XmlElement)el.FirstChild, new XElement("ANode", 1), new XElement("BNode", "B"),
                                    new[] { "A", "B", "C" }, null, formatter);
            TestSerializableObject(null, new XElement("ANode", 1), new XElement("BNode", "B"), new[] { "A", "B", "C" },
                                    null, formatter);
            TestSerializableObject(null, null, null, new[] { "A", "B", "C" }, null, formatter);
            TestSerializableObject(null, null, null, null, null, formatter);
            TestSerializableObject(null, null, null, null,
                                    new Dictionary<string, SerializableObject>
                                        {
                                            {"A", new SerializableObject {Xml2 = new XElement("Xml2")}},
                                            {"B", null}
                                        }, formatter);
            /* TODO TestSerializableObject(el, new XElement("ANode", 1), new XElement("BNode", "B"),
                                    new List<string> {"A", "B", "C"}.Where(s => s != "B"));
                 */
        }

        [Serializable]
        class SerializableTestClass
        {
            public int Number { get; private set; }
            public string Words { get; private set; }
            public KeyValuePair<string,int> KeyValuePair { get; private set; }
            public SerializableTestClass Nested { get; private set; }
            public SerializableTestClass(int number,string words, KeyValuePair<string,int> keyValuePair, SerializableTestClass nested = null)
            {
                Number = number;
                Words = words;
                KeyValuePair = keyValuePair;
                Nested = nested;
            }
            public void GetObjectData(SerializationInfo info,StreamingContext context)
            {
                info.AddValue("number", Number);
                info.AddValue("words", Words);
                info.AddValue("kvp", KeyValuePair);
                info.AddValue("nested", Nested);
            }
        }

        [TestMethod]
        public void GetXMLFormatter_LegacyTests()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<Test><SomeXml>Some Context</SomeXml><SomeMore/></Test>");
            XmlElement el = doc.DocumentElement;

            IFormatter formatter = Serialize.GetXmlFormatter();
            Stream stream = new MemoryStream();
            formatter.Serialize(stream,new SerializableTestClass(42,"hello world",new KeyValuePair<string, int>("moose",656),new SerializableTestClass(9,"nested",new KeyValuePair<string, int>("lemon",0))));
            stream.Seek(0,SeekOrigin.Begin);
            String result = null;
            using(StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
            Assert.AreEqual("mice", result);
            TestSerializableObject(el, new XElement("ANode", 1), new XElement("BNode", "B"),
                                    new List<string> { "A", "B", "C" }, null, formatter);
            TestSerializableObject((XmlElement)el.FirstChild, new XElement("ANode", 1), new XElement("BNode", "B"),
                                    new[] { "A", "B", "C" }, null, formatter);
            TestSerializableObject(null, new XElement("ANode", 1), new XElement("BNode", "B"), new[] { "A", "B", "C" },
                                    null, formatter);
            TestSerializableObject(null, null, null, new[] { "A", "B", "C" }, null, formatter);
            TestSerializableObject(null, null, null, null, null, formatter);
            TestSerializableObject(null, null, null, null,
                                    new Dictionary<string, SerializableObject>
                                        {
                                            {"A", new SerializableObject {Xml2 = new XElement("Xml2")}},
                                            {"B", null}
                                        }, formatter);
            /* TODO TestSerializableObject(el, new XElement("ANode", 1), new XElement("BNode", "B"),
                                    new List<string> {"A", "B", "C"}.Where(s => s != "B"));
                 */
        }

        private void TestSerializableObject(XmlElement xmlOld, XElement xml1, XElement xml2, IEnumerable<string> strings,
                                            Dictionary<string, SerializableObject> dictionary,
                                            IFormatter formatter = null)
        {
            formatter = formatter ?? Serialize.GetFormatter();
            SerializableObject sObj = new SerializableObject
                                          {
                                              XmlOld = xmlOld,
                                              Xml1 = xml1,
                                              Xml2 = xml2,
                                              Strings = strings,
                                              Dictionary = dictionary
                                          };
            string sStr = sObj.SerializeToString(formatter);
            Trace.WriteLine(string.Format("Serialized to \"{0}\"", sStr));
            SerializableObject dObj = sStr.Deserialize<SerializableObject>(formatter);
            Assert.IsTrue(sObj.Equals(dObj));
            Trace.WriteLine(string.Empty);
        }
    }

    /// <summary>
    /// Object w
    /// </summary>
    /// <remarks></remarks>
    [Serializable]
    public class SerializableObject : IEquatable<SerializableObject>
    {
        public Dictionary<string, SerializableObject> Dictionary;
        public IEnumerable<string> Strings;
        public XElement Xml1;
        public XElement Xml2;
        public XmlElement XmlOld;

        #region IEquatable<SerializableObject> Members
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
        /// <remarks></remarks>
        public bool Equals(SerializableObject other)
        {
            if (other == null)
                return false;

            if (XmlOld == null)
            {
                if (other.XmlOld != null)
                    return false;
            }
            else
            {
                if (other.XmlOld == null)
                    return false;

                if (XmlOld.ToString() != other.XmlOld.ToString())
                    return false;
            }

            if (Xml1 == null)
            {
                if (other.Xml1 != null)
                    return false;
            }
            else
            {
                if (other.Xml1 == null)
                    return false;

                if (Xml1.ToString() != other.Xml1.ToString())
                    return false;
            }

            if (Xml2 == null)
            {
                if (other.Xml2 != null)
                    return false;
            }
            else
            {
                if (other.Xml2 == null)
                    return false;

                if (Xml2.ToString() != other.Xml2.ToString())
                    return false;
            }

            if (Strings == null)
            {
                if (other.Strings != null)
                    return false;
            }
            else
            {
                if ((other.Strings == null) || (!Strings.SequenceEqual(other.Strings)))
                    return false;
            }

            if (Dictionary == null)
            {
                if (other.Dictionary != null)
                    return false;
            }
            else
            {
                if (other.Dictionary == null)
                    return false;

                Dictionary<string, SerializableObject>.Enumerator de = Dictionary.GetEnumerator();
                Dictionary<string, SerializableObject>.Enumerator oe = Dictionary.GetEnumerator();
                while (de.MoveNext())
                {
                    if (!oe.MoveNext())
                        return false;
                    if (de.Current.Key != oe.Current.Key)
                        return false;
                    if (de.Current.Value == null)
                    {
                        if (oe.Current.Value != null)
                            return false;
                    }
                    else
                    {
                        if ((oe.Current.Value == null) || (!de.Current.Value.Equals(oe.Current.Value)))
                            return false;
                    }
                }
            }

            return true;
        }
        #endregion
    }
}