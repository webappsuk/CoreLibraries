#region © Copyright Web Applications (UK) Ltd, 2014.  All rights reserved.
// Copyright (c) 2014, Web Applications UK Ltd
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.SqlServer.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Formatting;

namespace WebApplications.Utilities.Test.Formatting
{
    [TestClass]
    public class TestFormatBuilder
    {
        [TestMethod]
        public void TestFormatBuilderValues()
        {
            Assert.AreEqual(
                "1 True 2.3",
                new FormatBuilder()
                    .Append(1)
                    .Append(' ')
                    .Append(true)
                    .Append(' ')
                    .Append(2.3)
                    .ToString());
        }

        [TestMethod]
        public void TestFormatBuilderToString()
        {
            FormatBuilder builder =
                new FormatBuilder().AppendFormat("{!control}{TestA}, {TestB,4}, {TestC,-4}, {TestD:F4}");

            Trace.WriteLine("default before resolving");
            string str = builder.ToString();
            Trace.WriteLine("    " + str);
            Assert.AreEqual("{TestA}, {TestB,4}, {TestC,-4}, {TestD:F4}", str);

            Trace.WriteLine("default after resolving the first 3 points");
            Dictionary<string, object> dictionary = new Dictionary<string, object>
            {
                {"TestA", "abc"},
                {"TestB", 4},
                {"TestC", 5},
            };
            str = builder.ToString(dictionary);
            Trace.WriteLine("    " + str);
            Assert.AreEqual("abc,    4, 5   , {TestD:F4}", str);

            Trace.WriteLine("'F' after resolving the first 3 points");
            str = builder.ToString("f", null, dictionary);
            Trace.WriteLine("    " + str);
            Assert.AreEqual("{!control}{TestA}, {TestB,4}, {TestC,-4}, {TestD:F4}", str);

            Trace.WriteLine("'S' after resolving the first 3 points");
            str = builder.ToString("s", null, dictionary);
            Trace.WriteLine("    " + str);
            Assert.AreEqual("abc,    4, 5   , ", str);
        }

        [TestMethod]
        public void TestCloneFormatBuilder()
        {
            FormatBuilder builder = new FormatBuilder()
                .AppendLayout(50)
                .AppendLine(FormatResources.LoremIpsum)
                .AppendForegroundColor("Red")
                .AppendLine(FormatResources.SedUtPerspiciatis)
                .AppendLine(FormatResources.AtVeroEos)
                .AppendFormatLine("Some text with a {0} thing", "format");

            FormatBuilder clone = builder.Clone();

            Assert.IsTrue(builder.SequenceEqual(clone), "Chunks are not equal");
            Assert.AreEqual(builder.ToString(), clone.ToString());
        }

        [TestMethod]
        public void TestFormatBuilderToStringValues()
        {
            Dictionary<string, object> dictionary =
                new Dictionary<string, object>
                {
                    {"TestA", "abc"},
                    {"TestB", 4},
                    {"TestC", 5},
                };

            FormatBuilder builder =
                new FormatBuilder().AppendFormat("{!control}{TestA}, {TestB,4}, {TestC,-4}, {TestD:F4}");

            Trace.WriteLine("default with none");
            string str = builder.ToString();
            Trace.WriteLine("    " + str);
            Assert.AreEqual("{TestA}, {TestB,4}, {TestC,-4}, {TestD:F4}", str);

            Trace.WriteLine("default with the first 3 points");
            str = builder.ToString(dictionary);
            Trace.WriteLine("    " + str);
            Assert.AreEqual("abc,    4, 5   , {TestD:F4}", str);

            Trace.WriteLine("'F' with the first 3 points");
            str = builder.ToString("f", null, dictionary);
            Trace.WriteLine("    " + str);
            Assert.AreEqual("{!control}{TestA}, {TestB,4}, {TestC,-4}, {TestD:F4}", str);

            Trace.WriteLine("'S' with the first 3 points");
            str = builder.ToString("s", null, dictionary);
            Trace.WriteLine("    " + str);
            Assert.AreEqual("abc,    4, 5   , ", str);
        }

        [TestMethod]
        public void TestNestedResolution()
        {
            FormatBuilder builder = new FormatBuilder()
                .AppendFormat("{t:A {t:nested {t}}}");
            Assert.AreEqual("{t:A {t:nested {t}}}", builder.ToString("G"));
            Assert.AreEqual(string.Empty, builder.ToString("S"));
            Assert.AreEqual("{t:A {t:nested {t}}}", builder.ToString("F"));
            Assert.AreEqual(
                "A nested tag",
                builder.ToString(
                    c =>
                    {
                        // This demonstrates how we can perform tag nesting.
                        if (!string.Equals(c.Tag, "t"))
                            return c.Value;

                        // If the tag doesn't have a format, we output the value.
                        if (string.IsNullOrEmpty(c.Format))
                            return "tag";

                        // Otherwise we output a format builder for the format, which will itself
                        // be resolved with this resolver.
                        return new FormatBuilder().AppendFormat(c.Format);
                    }));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestReadOnly()
        {
            FormatBuilder builder = new FormatBuilder("Test", true);
            builder.AppendLine();
        }

        [TestMethod]
        public void TestCloneRemovesReadonly()
        {
            FormatBuilder builder = new FormatBuilder("Test", true);
            FormatBuilder builder2 = builder.Clone();
            builder2.AppendLine();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestCloneAddsReadonly()
        {
            FormatBuilder builder = new FormatBuilder("Test");
            FormatBuilder builder2 = builder.Clone(true);
            builder2.AppendLine();
        }

        [TestMethod]
        public void TestResolveArray()
        {
            FormatBuilder builder = new FormatBuilder("{0}{1}{2}");
            builder.Resolve(3,4,5);
            Assert.AreEqual("345", builder.ToString());
        }

        [TestMethod]
        public void TestResolveDictionary()
        {
            FormatBuilder builder = new FormatBuilder("{A}{B}{C}");
            builder.Resolve(new Dictionary<string, object>
            {
                {"A", 'D'},
                {"B", "E"},
                {"C", 3}
            });
            Assert.AreEqual("DE3", builder.ToString());
        }

        [TestMethod]
        public void TestResolver()
        {
            FormatBuilder builder = new FormatBuilder("{A}{B}{C}");
            builder.Resolve(chunk => chunk.Tag == "B" ? 5 : (object)null);
            Assert.AreEqual("{A}5{C}", builder.ToString());
        }

        [TestMethod]
        public void TestNestedResolver()
        {
            FormatBuilder builder = new FormatBuilder("{t:A {t:nested {t}}}");
            builder.Resolve(c =>
            {
                // This demonstrates how we can perform tag nesting.
                if (!string.Equals(c.Tag, "t"))
                    return c.Value;

                // If the tag doesn't have a format, we output the value.
                if (string.IsNullOrEmpty(c.Format))
                    return "tag";

                // Otherwise we output a format builder for the format, which will itself
                // be resolved with this resolver.
                return new FormatBuilder().AppendFormat(c.Format);
            });
            Assert.AreEqual("A nested tag", builder.ToString());
        }
    }
}