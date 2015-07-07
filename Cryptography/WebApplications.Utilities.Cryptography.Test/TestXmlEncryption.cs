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