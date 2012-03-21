#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities.Cryptography 
// Project: WebApplications.Utilities.Cryptography.Test
// File: TestXmlEncryption.cs
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
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebApplications.Utilities.Cryptography.Test
{
    //public class TestXmlEncryption
    //{
    //    private XmlDocument GetTestXmlDocument()
    //    {
    //        Assembly a = Assembly.GetExecutingAssembly();
    //        XmlDocument document = new XmlDocument();
    //        using (Stream resource = a.GetManifestResourceStream("WebApplications.Utilities.Cryptography.Test.Test.xml"))
    //        {
    //            document.Load(resource);
    //        }
    //        Assert.IsNotNull(document);
    //        Assert.IsNotNull(document.DocumentElement);
    //        return document;
    //    }

    //    private XDocument GetTestXDocument()
    //    {
    //        Assembly a = Assembly.GetExecutingAssembly();
    //        XDocument document;
    //        using (Stream resource = a.GetManifestResourceStream("WebApplications.Utilities.Cryptography.Test.Test.xml"))
    //        {
    //            document = XDocument.Load(resource);
    //        }
    //        Assert.IsNotNull(document);
    //        Assert.IsNotNull(document.Root);
    //        return document;
    //    }

    //    [TestMethod]
    //    public void TestEncryptXmlDocument()
    //    {
    //        Stopwatch s = new Stopwatch();
    //        // Grab the document.
    //        XmlDocument doc = GetTestXmlDocument();
    //        string docStr = doc.OuterXml;

    //        s.Start();
    //        doc.Encrypt();
    //        s.Stop();
    //        Trace.WriteLine(string.Format("XmlDocument Encryption took {0}ms",
    //                                      (s.ElapsedTicks*1000M)/Stopwatch.Frequency));

    //        Assert.AreNotEqual(docStr, doc.OuterXml);

    //        s.Restart();
    //        XmlElement decryptedXml;
    //        bool? isLatestKey;
    //        Assert.IsTrue(doc.TryDecrypt(out decryptedXml, out isLatestKey));
    //        s.Stop();
    //        Assert.AreEqual(docStr, doc.OuterXml);
    //        Trace.WriteLine(string.Format("XmlDocument Encryption took {0}ms",
    //                                      (s.ElapsedTicks*1000M)/Stopwatch.Frequency));
    //    }

    //    [TestMethod]
    //    public void TestEncryptXDocument()
    //    {
    //        Stopwatch s = new Stopwatch();
    //        // Grab the document.
    //        XDocument doc = GetTestXDocument();
    //        string docStr = doc.ToString(SaveOptions.None);

    //        s.Start();
    //        doc.Encrypt();
    //        s.Stop();
    //        Trace.WriteLine(string.Format("XDocument Encryption took {0}ms",
    //                                      (s.ElapsedTicks*1000M)/Stopwatch.Frequency));

    //        Assert.AreNotEqual(docStr, doc.ToString(SaveOptions.None));

    //        s.Restart();
    //        XElement decryptedXml;
    //        bool? isLatestKey;
    //        Assert.IsTrue(doc.TryDecrypt(out decryptedXml, out isLatestKey));
    //        s.Stop();

    //        Assert.AreEqual(docStr, doc.ToString(SaveOptions.None));
    //        Trace.WriteLine(string.Format("XDocument Encryption took {0}ms",
    //                                      (s.ElapsedTicks*1000M)/Stopwatch.Frequency));
    //    }

    //    [TestMethod]
    //    public void TestEncryptRandomXmlNodes()
    //    {
    //        // Grab the document.
    //        XmlDocument doc = GetTestXmlDocument();
    //        string docStr = doc.OuterXml;
    //        Random r = new Random();

    //        bool encrypted = false;
    //        Stack<XmlElement> navStack = new Stack<XmlElement>();
    //        navStack.Push(doc.DocumentElement);
    //        while (navStack.Count > 0)
    //        {
    //            XmlElement element = navStack.Pop();
    //            if (element == null)
    //                continue;

    //            // Don't encrypt root element in this test
    //            if ((navStack.Count > 1) &&
    //                (r.Next(2) == 1))
    //            {
    //                // Encrypt node
    //                element.Encrypt();
    //                encrypted = true;
    //            }
    //            else if (element.HasChildNodes)
    //            {
    //                // Add child elements
    //                foreach (XmlNode node in element.ChildNodes)
    //                {
    //                    element = node as XmlElement;
    //                    if (element != null)
    //                        navStack.Push(element);
    //                }
    //            }
    //        }

    //        if (!encrypted)
    //        {
    //            Trace.WriteLine("Didn't encrypt!");
    //            Assert.AreEqual(docStr, doc.OuterXml);
    //            return;
    //        }


    //        Assert.AreNotEqual(docStr, doc.OuterXml);

    //        // Now decrypt
    //        bool? isLatestKey;
    //        XmlElement decryptedXml;
    //        Assert.IsTrue(doc.TryDecrypt(out decryptedXml, out isLatestKey));
    //        // TODO: Write with latestKey out
    //        // Assert.IsTrue(latestKey);
    //    }

    //    [TestMethod]
    //    public void TestEncryptRandomXNodes()
    //    {
    //        // Grab the document.
    //        XDocument doc = GetTestXDocument();
    //        string docStr = doc.ToString(SaveOptions.None);
    //        Random r = new Random();

    //        bool encrypted = false;
    //        Stack<XElement> navStack = new Stack<XElement>();
    //        navStack.Push(doc.Root);
    //        while (navStack.Count > 0)
    //        {
    //            XElement element = navStack.Pop();
    //            if (element == null)
    //                continue;

    //            // Don't encrypt root element in this test
    //            if ((navStack.Count > 1) &&
    //                (r.Next(2) == 1))
    //            {
    //                // Encrypt node
    //                element.Encrypt();
    //                encrypted = true;
    //            }
    //            else
    //            {
    //                // Add child elements
    //                foreach (XElement e in element.Elements())
    //                    navStack.Push(e);
    //            }
    //        }

    //        if (!encrypted)
    //        {
    //            Trace.WriteLine("Didn't encrypt!");
    //            Assert.AreEqual(docStr, doc.ToString());
    //            return;
    //        }


    //        Assert.AreNotEqual(docStr, doc.ToString(SaveOptions.None));

    //        // Now decrypt
    //        bool? isLatestKey;
    //        XElement decryptedXml;
    //        Assert.IsTrue(doc.TryDecrypt(out decryptedXml, out isLatestKey));
    //        Assert.IsTrue(isLatestKey.HasValue);
    //        Assert.IsTrue(isLatestKey.Value);
    //    }
    //}
}