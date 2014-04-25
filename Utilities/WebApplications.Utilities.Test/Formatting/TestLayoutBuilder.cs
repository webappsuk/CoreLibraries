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
    public class TestLayoutBuilder
    {
        [TestMethod]
        public void TestLayout()
        {
            Trace.WriteLine(
                new LayoutBuilder(new Layout(60, alignment: Alignment.Justify, firstLineIndentSize: 5))
                    .AppendLine(FormatResources.LoremIpsum)
                    .Append("{!layout:w50}")
                    .AppendLine(FormatResources.SedUtPerspiciatis)
                    .AppendLayout(40)
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
                    .AppendLayout(alignment: Alignment.Left)
                    .AppendLine(FormatResources.SedUtPerspiciatis)
                    .AppendLine()
                    .AppendLayout(alignment: Alignment.Right)
                    .AppendLine(FormatResources.AtVeroEos)
                    .AppendLine()
                    .AppendLayout(alignment: Alignment.Centre, firstLineIndentSize: 4, indentSize: 4, rightMarginSize: 4)
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
        public void TestLayoutBuilderToString()
        {
            // Expect LayoutBuilder with default layout to behave the same as FormatBuilder
            TestFormatBuilder.TestToStringFormats(new LayoutBuilder());
        }

        [TestMethod]
        public void TestTabStops()
        {
            LayoutBuilder builder = new LayoutBuilder();
            builder
                .AppendLayout(50,
                    firstLineIndentSize: 1,
                    tabStops: new ushort[] {6, 9, 20, 30, 40})
                .Append("A\tTab Stop\tAnother");

            ushort position = 0;
            Assert.AreEqual(" A    Tab Stop      Another", builder.ToString(ref position));
            Assert.AreEqual(27, position);
            Assert.AreEqual("A  Tab Stop  Another", builder.ToString(ref position));
            Assert.AreEqual(47, position);
        }

        [TestMethod]
        public void TestCloneLayoutBuilder()
        {
            FormatBuilder builder = new LayoutBuilder()
                .AppendLayout(50)
                .AppendLine(FormatResources.LoremIpsum)
                .AppendForegroundColor("Red")
                .AppendLine(FormatResources.SedUtPerspiciatis)
                .AppendLine(FormatResources.AtVeroEos)
                .AppendFormatLine("Some text with a {0} thing", "format");

            FormatBuilder clone = builder.Clone();

            Assert.IsInstanceOfType(clone, typeof(LayoutBuilder));

            Assert.IsTrue(builder.SequenceEqual(clone), "Chunks are not equal");
            Assert.AreEqual(builder.ToString(), clone.ToString());
        }

        [TestMethod]
        public void TestLayoutBuilderToStringValues()
        {
            TestFormatBuilder.TestToStringValues(new LayoutBuilder());
        }
    }
}