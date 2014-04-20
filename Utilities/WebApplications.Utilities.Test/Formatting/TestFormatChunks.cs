using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Assert.AreEqual("{0}", chunk.Value);
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
            Assert.AreEqual("{0,-1}", chunk.Value);
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
            Assert.AreEqual("{0:G}", chunk.Value);
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
            Assert.AreEqual("{0:,}", chunk.Value);
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
            Assert.AreEqual("{0,-1:G}", chunk.Value);
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
            Assert.AreEqual("{0,-1:G}", chunk.Value);
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
            Assert.AreEqual("{0,-1:G}", chunk.Value);
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
                    .SetLayout(new Layout(40))
                    .AppendLine(FormatResources.AtVeroEos).ToString());
        }

        [TestMethod]
        public void TestPadToWrap()
        {
            const int width = 60;

            string text = new LayoutBuilder(new Layout(width, alignment: Alignment.Justify, wrapMode: LayoutWrapMode.PadToWrap))
                    .AppendLine(FormatResources.LoremIpsum)
                    .AppendLine()
                    .SetLayout(new Layout(alignment: Alignment.Left))
                    .AppendLine(FormatResources.SedUtPerspiciatis)
                    .AppendLine()
                    .SetLayout(new Layout(alignment: Alignment.Right))
                    .AppendLine(FormatResources.AtVeroEos)
                    .AppendLine()
                    .SetLayout(new Layout(alignment: Alignment.Centre, firstLineIndentSize: 4, indentSize: 4, rightMarginSize: 4))
                    .AppendLine(FormatResources.AtVeroEos).ToString();

            Assert.IsFalse(text.Contains('\r'), "Text should not contain new line characters");
            Assert.IsFalse(text.Contains('\n'), "Text should not contain new line characters");
            Assert.IsTrue(text.Length % width == 0, "Text length should be a multiple of the width");

            // Simulate console wrapping
            for (int i = 0; i < text.Length; i += width)
            {
                Trace.Write((i + width) >= text.Length ? text.Substring(i) : text.Substring(i, width));
                Trace.WriteLine("|");
            }
        }

        [TestMethod]
        public void TestResolve()
        {
            FormatBuilder builder = new FormatBuilder()
                .AppendFormat("{TestA}, {TestB}, {TestC}");

            string str = builder.ToString();
            Assert.AreEqual("{TestA}, {TestB}, {TestC}", str);
            Trace.WriteLine(str);

            builder.Resolve(
                new Dictionary<string, object>
                {
                    {"TestA", "abc"},
                    {"TestB", 4},
                    {"TestC", 5},
                });
            str = builder.ToString();
            Assert.AreEqual("abc, 4, 5", str);
            Trace.WriteLine(str);

            builder.Resolve(
                new Dictionary<string, object>
                {
                    {"TestA", "xyz"},
                    {"TestB", 20},
                    {"TestC", 30},
                });
            str = builder.ToString();
            Assert.AreEqual("xyz, 20, 30", str);
            Trace.WriteLine(str);
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
            builder.Resolve(
                new Dictionary<string, object>
                {
                    {"TestA", "abc"},
                    {"TestB", 4},
                    {"TestC", 5},
                });
            str = builder.ToString();
            Trace.WriteLine("    " + str);
            Assert.AreEqual("abc,    4, 5   , {TestD:F4}", str);

            Trace.WriteLine("'F' after resolving the first 3 points");
            str = builder.ToString("f", null);
            Trace.WriteLine("    " + str);
            Assert.AreEqual("{!control}{TestA}, {TestB,4}, {TestC,-4}, {TestD:F4}", str);

            Trace.WriteLine("'S' after resolving the first 3 points");
            str = builder.ToString("s", null);
            Trace.WriteLine("    " + str);
            Assert.AreEqual("abc,    4, 5   , ", str);
        }
    }
}
