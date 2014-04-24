using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Formatting;

namespace WebApplications.Utilities.Test.Formatting
{
    [TestClass]
    public class TestFormatChunks
    {
        [TestMethod]
        public void TestNull()
        {
            FormatChunk[] chunks = ((string)null).FormatChunks().ToArray();
            Assert.AreEqual(0, chunks.Length);
        }

        [TestMethod]
        public void TestEmpty()
        {
            FormatChunk[] chunks = "".FormatChunks().ToArray();
            Assert.AreEqual(0, chunks.Length);
        }

        [TestMethod]
        public void TestNoFillPoint()
        {
            FormatChunk[] chunks = " ".FormatChunks().ToArray();
            Assert.AreEqual(1, chunks.Length);
            FormatChunk chunk = chunks[0];
            Assert.IsNotNull(chunk);
            Assert.AreEqual(" ", chunk.Value);
            Assert.IsFalse(chunk.IsFillPoint);
        }

        [TestMethod]
        public void TestSimpleFillPoint()
        {
            FormatChunk[] chunks = "{0}".FormatChunks().ToArray();
            Assert.AreEqual(1, chunks.Length);
            FormatChunk chunk = chunks[0];
            Assert.IsNotNull(chunk);
            Assert.AreEqual("{0}", chunk.ToString("F", null));
            Assert.IsNull(chunk.Value);
            Assert.IsTrue(chunk.IsFillPoint);
            Assert.AreEqual("0", chunk.Tag);
            Assert.IsNull(chunk.Alignment);
            Assert.IsNull(chunk.Format);
        }

        [TestMethod]
        public void TestAlignedFillPoint()
        {
            FormatChunk[] chunks = "{0,-1}".FormatChunks().ToArray();
            Assert.AreEqual(1, chunks.Length);
            FormatChunk chunk = chunks[0];
            Assert.IsNotNull(chunk);
            Assert.AreEqual("{0,-1}", chunk.ToString("F", null));
            Assert.IsNull(chunk.Value);
            Assert.IsTrue(chunk.IsFillPoint);
            Assert.AreEqual("0", chunk.Tag);
            Assert.AreEqual(-1, chunk.Alignment);
            Assert.IsNull(chunk.Format);
        }

        [TestMethod]
        public void TestInvalidAlignedFillPoint()
        {
            FormatChunk[] chunks = "{0,}".FormatChunks().ToArray();
            Assert.AreEqual(1, chunks.Length);
            FormatChunk chunk = chunks[0];
            Assert.IsNotNull(chunk);
            Assert.AreEqual("{0,}", chunk.Value);
            Assert.IsFalse(chunk.IsFillPoint);
        }

        [TestMethod]
        public void TestInvalidIntAlignedFillPoint()
        {
            FormatChunk[] chunks = "{0,a}".FormatChunks().ToArray();
            Assert.AreEqual(1, chunks.Length);
            FormatChunk chunk = chunks[0];
            Assert.IsNotNull(chunk);
            Assert.AreEqual("{0,a}", chunk.Value);
            Assert.IsFalse(chunk.IsFillPoint);
        }

        [TestMethod]
        public void TestFormatFillPoint()
        {
            FormatChunk[] chunks = "{0:G}".FormatChunks().ToArray();
            Assert.AreEqual(1, chunks.Length);
            FormatChunk chunk = chunks[0];
            Assert.IsNotNull(chunk);
            Assert.AreEqual("{0:G}", chunk.ToString("F", null));
            Assert.IsNull(chunk.Value);
            Assert.IsTrue(chunk.IsFillPoint);
            Assert.AreEqual("0", chunk.Tag);
            Assert.IsNull(chunk.Alignment);
            Assert.AreEqual("G", chunk.Format);
        }

        [TestMethod]
        public void TestCommaFormatFillPoint()
        {
            FormatChunk[] chunks = "{0:,}".FormatChunks().ToArray();
            Assert.AreEqual(1, chunks.Length);
            FormatChunk chunk = chunks[0];
            Assert.IsNotNull(chunk);
            Assert.AreEqual("{0:,}", chunk.ToString("F", null));
            Assert.IsNull(chunk.Value);
            Assert.IsTrue(chunk.IsFillPoint);
            Assert.AreEqual("0", chunk.Tag);
            Assert.IsNull(chunk.Alignment);
            Assert.AreEqual(",", chunk.Format);
        }

        [TestMethod]
        public void TestInvalidFormatFillPoint()
        {
            FormatChunk[] chunks = "{0:}".FormatChunks().ToArray();
            Assert.AreEqual(1, chunks.Length);
            FormatChunk chunk = chunks[0];
            Assert.IsNotNull(chunk);
            Assert.IsNotNull(chunk);
            Assert.AreEqual("{0:}", chunk.Value);
            Assert.IsFalse(chunk.IsFillPoint);
        }

        [TestMethod]
        public void TestFullFillPoint()
        {
            FormatChunk[] chunks = "{0,-1:G}".FormatChunks().ToArray();
            Assert.AreEqual(1, chunks.Length);
            FormatChunk chunk = chunks[0];
            Assert.IsNotNull(chunk);
            Assert.AreEqual("{0,-1:G}", chunk.ToString("F", null));
            Assert.IsNull(chunk.Value);
            Assert.IsTrue(chunk.IsFillPoint);
            Assert.AreEqual("0", chunk.Tag);
            Assert.AreEqual(-1, chunk.Alignment);
            Assert.AreEqual("G", chunk.Format);
        }

        [TestMethod]
        public void TestInvalidFullFillPoint()
        {
            FormatChunk[] chunks = "{0,:}".FormatChunks().ToArray();
            Assert.AreEqual(1, chunks.Length);
            FormatChunk chunk = chunks[0];
            Assert.IsNotNull(chunk);
            Assert.AreEqual("{0,:}", chunk.Value);
            Assert.IsFalse(chunk.IsFillPoint);
        }

        [TestMethod]
        public void TestInvalidIntFullFillPoint()
        {
            FormatChunk[] chunks = "{0,a:G}".FormatChunks().ToArray();
            Assert.AreEqual(1, chunks.Length);
            FormatChunk chunk = chunks[0];
            Assert.IsNotNull(chunk);
            Assert.AreEqual("{0,a:G}", chunk.Value);
            Assert.IsFalse(chunk.IsFillPoint);
        }

        [TestMethod]
        public void TestLeadingSpace()
        {
            FormatChunk[] chunks = " {0,-1:G}".FormatChunks().ToArray();
            Assert.AreEqual(2, chunks.Length);
            FormatChunk chunk = chunks[0];
            Assert.IsNotNull(chunk);
            Assert.AreEqual(" ", chunk.Value);
            Assert.IsFalse(chunk.IsFillPoint);

            chunk = chunks[1];
            Assert.IsNotNull(chunk);
            Assert.AreEqual("{0,-1:G}", chunk.ToString("F", null));
            Assert.IsNull(chunk.Value);
            Assert.IsTrue(chunk.IsFillPoint);
            Assert.AreEqual("0", chunk.Tag);
            Assert.AreEqual(-1, chunk.Alignment);
            Assert.AreEqual("G", chunk.Format);
        }

        [TestMethod]
        public void TestTrailingSpace()
        {
            FormatChunk[] chunks = "{0,-1:G} ".FormatChunks().ToArray();
            Assert.AreEqual(2, chunks.Length);
            FormatChunk chunk = chunks[0];
            Assert.IsNotNull(chunk);
            Assert.AreEqual("{0,-1:G}", chunk.ToString("F", null));
            Assert.IsNull(chunk.Value);
            Assert.IsTrue(chunk.IsFillPoint);
            Assert.AreEqual("0", chunk.Tag);
            Assert.AreEqual(-1, chunk.Alignment);
            Assert.AreEqual("G", chunk.Format);

            chunk = chunks[1];
            Assert.IsNotNull(chunk);
            Assert.AreEqual(" ", chunk.Value);
            Assert.IsFalse(chunk.IsFillPoint);
        }

        [TestMethod]
        public void TestLayout()
        {
            Trace.WriteLine(
                new LayoutBuilder(new Layout(60, alignment: Alignment.Justify, firstLineIndentSize: 5))
                    .AppendLine(FormatResources.LoremIpsum)
                    .Append("{!layout:w50}")
                    .AppendLine(FormatResources.SedUtPerspiciatis)
                    .SetLayout(40)
                    .AppendLine(FormatResources.AtVeroEos).ToString());
        }

        [TestMethod]
        public void TestPadToWrap()
        {
            const int width = 60;

            string text =
                new LayoutBuilder(new Layout(width, alignment: Alignment.Justify, wrapMode: LayoutWrapMode.PadToWrap))
                    .AppendLine(FormatResources.LoremIpsum)
                    .AppendLine()
                    .SetLayout(alignment: Alignment.Left)
                    .AppendLine(FormatResources.SedUtPerspiciatis)
                    .AppendLine()
                    .SetLayout(alignment: Alignment.Right)
                    .AppendLine(FormatResources.AtVeroEos)
                    .AppendLine()
                    .SetLayout(alignment: Alignment.Centre, firstLineIndentSize: 4, indentSize: 4, rightMarginSize: 4)
                    .AppendLine(FormatResources.AtVeroEos).ToString();

            // Simulate console wrapping
            for (int i = 0; i < text.Length; i += width)
            {
                Trace.Write((i + width) >= text.Length ? text.Substring(i) : text.Substring(i, width));
                Trace.WriteLine("|");
            }

            Assert.IsFalse(text.Contains('\r'), "Text should not contain new line characters");
            Assert.IsFalse(text.Contains('\n'), "Text should not contain new line characters");
            Assert.IsTrue(text.Length % width == 0, "Text length should be a multiple of the width");
        }

        [TestMethod]
        public void TestFormatBuilderToString()
        {
            TestToStringFormats(new FormatBuilder());
        }

        [TestMethod]
        public void TestLayoutBuilderToString()
        {
            // Expect LayoutBuilder with default layout to behave the same as FormatBuilder
            TestToStringFormats(new LayoutBuilder());
        }

        private void TestToStringFormats([NotNull] FormatBuilder builder)
        {
            builder.AppendFormat("{!control}{TestA}, {TestB,4}, {TestC,-4}, {TestD:F4}");

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
                .SetLayout(50)
                .AppendLine(FormatResources.LoremIpsum)
                .ConsoleForeColour("Red")
                .AppendLine(FormatResources.SedUtPerspiciatis)
                .AppendLine(FormatResources.AtVeroEos)
                .AppendFormatLine("Some text with a {0} thing", "format");

            FormatBuilder clone = builder.Clone();

            Assert.IsTrue(builder.SequenceEqual(clone), "Chunks are not equal");
            Assert.AreEqual(builder.ToString(), clone.ToString());
        }

        [TestMethod]
        public void TestCloneLayoutBuilder()
        {
            FormatBuilder builder = new LayoutBuilder()
                .SetLayout(50)
                .AppendLine(FormatResources.LoremIpsum)
                .ConsoleForeColour("Red")
                .AppendLine(FormatResources.SedUtPerspiciatis)
                .AppendLine(FormatResources.AtVeroEos)
                .AppendFormatLine("Some text with a {0} thing", "format");

            FormatBuilder clone = builder.Clone();

            Assert.IsInstanceOfType(clone, typeof(LayoutBuilder));

            Assert.IsTrue(builder.SequenceEqual(clone), "Chunks are not equal");
            Assert.AreEqual(builder.ToString(), clone.ToString());
        }

        [TestMethod]
        public void TestFormatBuilderToStringValues()
        {
            TestToStringValues(new FormatBuilder());
        }

        [TestMethod]
        public void TestLayoutBuilderToStringValues()
        {
            TestToStringValues(new LayoutBuilder());
        }

        private void TestToStringValues([NotNull] FormatBuilder builder)
        {
            var dictionary =
                new Dictionary<string, object>
                {
                    {"TestA", "abc"},
                    {"TestB", 4},
                    {"TestC", 5},
                };

            builder.AppendFormat("{!control}{TestA}, {TestB,4}, {TestC,-4}, {TestD:F4}");

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
        public void TestNestedChunks()
        {
            // Any character that appears after a '\' will allways be added. This onl has an effect for the '{', '}' and '\' characters, all others do not need to be escaped
            FormatChunk[] chunks = @"This: {Tag:Is a very {complicated: {nested} format string with \{{escaped}\} tags and other \\ \'\characters\}}} and some trailing text".FormatChunks().ToArray();

            // ReSharper disable PossibleNullReferenceException
            Assert.AreEqual(3, chunks.Length);
            Assert.AreEqual("This: ", chunks[0].Value);
            Assert.AreEqual("Tag", chunks[1].Tag);
            Assert.AreEqual(@"Is a very {complicated: {nested} format string with \{{escaped}\} tags and other \\ \'\characters\}}", chunks[1].Format);
            Assert.AreEqual(" and some trailing text", chunks[2].Value);

            chunks = chunks[1].Format.FormatChunks().ToArray();
            Assert.AreEqual(2, chunks.Length);
            Assert.AreEqual("Is a very ", chunks[0].Value);
            Assert.AreEqual("complicated", chunks[1].Tag);
            Assert.AreEqual(@" {nested} format string with \{{escaped}\} tags and other \\ \'\characters\}", chunks[1].Format);

            chunks = chunks[1].Format.FormatChunks().ToArray();
            Assert.AreEqual(5, chunks.Length);
            Assert.AreEqual(" ", chunks[0].Value);
            Assert.AreEqual("nested", chunks[1].Tag);
            Assert.AreEqual(@" format string with {", chunks[2].Value);
            Assert.AreEqual("escaped", chunks[3].Tag);
            Assert.AreEqual(@"} tags and other \ 'characters}", chunks[4].Value);
            // ReSharper restore PossibleNullReferenceException
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
    }
}