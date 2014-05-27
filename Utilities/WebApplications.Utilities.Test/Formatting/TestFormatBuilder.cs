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
using System.Drawing;
using System.IO;
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
        public void TestFormatBuilderToStringWhitespaceAndAlign()
        {
            // Create a format builder with limited width and special characters.
            FormatBuilder f = new FormatBuilder(5, format: "{tag}\t\twith tabs\r\nand new lines.");
            Assert.AreEqual("{tag}\t\twith tabs\r\nand new lines.", f.ToString("f"));
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
        public void TestReadOnly()
        {
            FormatBuilder builder = new FormatBuilder("Test", true);
            Assert.IsTrue(builder.IsReadOnly);
            bool thrown;
            try
            {
                builder.AppendLine();
                thrown = false;
            }
            catch
            {
                thrown = true;
            }
            Assert.IsTrue(thrown);
        }

        [TestMethod]
        public void TestMakeReadOnly()
        {
            FormatBuilder builder = new FormatBuilder("Test");
            Assert.IsFalse(builder.IsReadOnly);
            builder.AppendLine();

            builder.MakeReadOnly();
            Assert.IsTrue(builder.IsReadOnly);

            bool thrown;
            try
            {
                builder.AppendLine();
                thrown = false;
            }
            catch
            {
                thrown = true;
            }
            Assert.IsTrue(thrown);
        }

        [TestMethod]
        public void TestCloneRemovesReadonly()
        {
            FormatBuilder builder = new FormatBuilder("Test", true);
            FormatBuilder builder2 = builder.Clone();
            Assert.IsTrue(builder.IsReadOnly);
            Assert.IsFalse(builder2.IsReadOnly);
            builder2.AppendLine();
        }

        [TestMethod]
        public void TestCloneAddsReadonly()
        {
            FormatBuilder builder = new FormatBuilder("Test");
            FormatBuilder builder2 = builder.Clone(true);
            Assert.IsFalse(builder.IsReadOnly);
            Assert.IsTrue(builder2.IsReadOnly);
            bool thrown;
            try
            {
                builder2.AppendLine();
                thrown = false;
            }
            catch (Exception)
            {
                thrown = true;
            }
            Assert.IsTrue(thrown);
        }

        [TestMethod]
        public void TestCloneKeepsReadonly()
        {
            FormatBuilder builder = new FormatBuilder("Test", true);
            FormatBuilder builder2 = builder.Clone(true);
            Assert.IsTrue(builder.IsReadOnly);
            Assert.IsTrue(builder2.IsReadOnly);

            bool thrown;
            try
            {
                builder2.AppendLine();
                thrown = false;
            }
            catch (Exception)
            {
                thrown = true;
            }
            Assert.IsTrue(thrown);
        }
        
        [TestMethod]
        public void TestResolveDictionary()
        {
            FormatBuilder builder = new FormatBuilder("{A}{B}{C}");
            builder.Resolve(
                new Dictionary<string, object>
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
            builder.Resolve((_, c) => c.Tag.ToLowerInvariant() == "b" ? 5 : Resolution.Unknown);
            Assert.AreEqual("{A}5{C}", builder.ToString());
        }

        [TestMethod]
        public void TestNestedResolver()
        {
            FormatBuilder builder = new FormatBuilder("{t:A {t:nested {t}}}");
            builder.Resolve((_, c) => string.Equals(c.Tag, "t", StringComparison.CurrentCultureIgnoreCase) ? "tag" : Resolution.Unknown);
            Assert.AreEqual("A nested tag", builder.ToString());
        }

        [TestMethod]
        public void TestNestedResolution()
        {
            FormatBuilder builder = new FormatBuilder("{t:A {t:nested {t}}}");
            Assert.AreEqual("{t:A {t:nested {t}}}", builder.ToString("G"));
            Assert.AreEqual(string.Empty, builder.ToString("S"));
            Assert.AreEqual("{t:A {t:nested {t}}}", builder.ToString("F"));
            Assert.AreEqual(
                "A nested tag",
                builder.ToString((_, c) => string.Equals(c.Tag, "t", StringComparison.CurrentCultureIgnoreCase) ? "tag" : Resolution.Unknown));
        }

        [TestMethod]
        public void TestResolveToNull()
        {
            FormatBuilder builder = new FormatBuilder("{A}-{B}");
            Assert.AreEqual("-", builder.ToString(
                    (_, c) =>
                    {
                        string tag = c.Tag.ToLowerInvariant();
                        switch (tag)
                        {
                            case "a":
                                return Resolution.Null;
                            case "b":
                                return null;
                            default:
                                return tag;
                        }
                    }));
        }

        [TestMethod]
        public void TestResolveToBuilder()
        {
            FormatBuilder builderA = new FormatBuilder("{A:{A}} {B} {C}");
            FormatBuilder builderB = new FormatBuilder("{Builder}");
            Assert.AreEqual(
                "a b c",
                builderB.ToString(
                    (_, c) =>
                    {
                        string tag = c.Tag.ToLowerInvariant();
                        switch (tag)
                        {
                            case "builder":
                                return builderA;
                            default:
                                return tag;
                        }
                    }));
        }

        [TestMethod]
        public void TestAutoPositioning()
        {
            using (StringWriter writer = new StringWriter())
            using (FormatTextWriter fw = new FormatTextWriter(writer, 5))
            {
                fw.WriteLine();
                Assert.AreEqual(0, fw.Position);
                fw.WriteLine("12345");
                Assert.AreEqual(0, fw.Position);
                fw.Write("12345\r\n");
                Assert.AreEqual(0, fw.Position);
                fw.Write("1234\r\n");
                Assert.AreEqual(0, fw.Position);
                fw.Write("1234");
                Assert.AreEqual(4, fw.Position);
                fw.Write("123456");
                Assert.AreEqual(1, fw.Position);
                fw.Write("123456");
                Assert.AreEqual(1, fw.Position);
                fw.Write("12345\r\n1");
                Assert.AreEqual(1, fw.Position);
            }
        }

        [TestMethod]
        public void TestManualPositioning()
        {
            int position = 0;
            new FormatBuilder("12345\r\n").ToString(null, ref position);
            Assert.AreEqual(0, position);
            new FormatBuilder("12345\r\n1").ToString(null, ref position);
            Assert.AreEqual(1, position);
            new FormatBuilder().AppendLine("1234").ToString(null, ref position);
            Assert.AreEqual(0, position);
            new FormatBuilder().AppendLine("1234").Append("1").ToString(null, ref position);
            Assert.AreEqual(1, position);
        }

        [TestMethod]
        public void TestResolveControlChunk()
        {
            FormatBuilder builder = new FormatBuilder()
                .AppendFormat("{" + FormatBuilder.ForegroundColorTag + ":Custom}")
                .Resolve(
                    (_, c) =>
                    {
                        if (string.Equals(
                            c.Tag,
                            FormatBuilder.ForegroundColorTag,
                            StringComparison.CurrentCultureIgnoreCase) &&
                            string.Equals(c.Format, "Custom"))
                            return Color.Green;
                        return Resolution.Unknown;
                    },
                    false,
                    true);

            using (TestColoredTextWriter writer = new TestColoredTextWriter(true))
            {
                builder.WriteTo(writer);
                Assert.AreEqual("{fg:Green}", writer.ToString());
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void TestTrailingControlChunks()
        {
            FormatBuilder builder = new FormatBuilder(100)
                .Append("Text")
                .AppendFormat("{" + FormatBuilder.ForegroundColorTag + "}");

            using (TestColoredTextWriter writer = new TestColoredTextWriter())
            {
                builder.WriteTo(writer);
                Assert.AreEqual("Text{/fg}", writer.ToString());
            }

            builder.Clear();
            builder.AppendLine("Text")
                .AppendFormat("{" + FormatBuilder.ForegroundColorTag + "}");

            using (TestColoredTextWriter writer = new TestColoredTextWriter())
            {
                builder.WriteTo(writer);
                Assert.AreEqual("Text\r\n{/fg}", writer.ToString());
            }
        }
    }
}