using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Formatting;

namespace WebApplications.Utilities.Test.Formatting
{
    [TestClass]
    public class TestFormatBuilderLayout
    {
        [TestMethod]
        public void TestLayout()
        {
            Trace.WriteLine(
                new FormatBuilder(new Layout(60, alignment: Alignment.Justify, firstLineIndentSize: 5))
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
                new FormatBuilder(new Layout(width, alignment: Alignment.Justify, wrapMode: LayoutWrapMode.PadToWrap))
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
        public void TestTabStops()
        {
            FormatBuilder builder = new FormatBuilder();
            builder
                .AppendLayout(50,
                    firstLineIndentSize: 1,
                    tabStops: new[] { 6, 9, 20, 30, 40 })
                .Append("A\tTab Stop\tAnother");

            int position = 0;
            Assert.AreEqual(" A    Tab Stop      Another", builder.ToString(ref position));
            Assert.AreEqual(27, position);
            Assert.AreEqual("A  Tab Stop  Another", builder.ToString(ref position));
            Assert.AreEqual(47, position);
        }

        [TestMethod]
        public void TestCloneLayoutBuilder()
        {
            FormatBuilder builder = new FormatBuilder(50)
                .AppendLine(FormatResources.LoremIpsum)
                .AppendForegroundColor("Red")
                .AppendLine(FormatResources.SedUtPerspiciatis)
                .AppendLine(FormatResources.AtVeroEos)
                .AppendFormatLine("Some text with a {0} thing", "format");

            FormatBuilder clone = builder.Clone();

            Assert.IsInstanceOfType(clone, typeof(FormatBuilder));

            Assert.IsTrue(builder.SequenceEqual(clone), "Chunks are not equal");
            Assert.AreEqual(builder.ToString(), clone.ToString());
        }

        [TestMethod]
        [Timeout(1000)]
        public void TestThreadSafety()
        {
            const ushort width = 80;
            using (StringWriter stringWriter = new StringWriter())
            {
                using (FormatTextWriter formatTextWriter = new FormatTextWriter(stringWriter, width, alignment: Alignment.Justify))
                {
                    Stopwatch watch = Stopwatch.StartNew();
                    Parallel.For(
                        0,
                        1000,
                        new ParallelOptions() { MaxDegreeOfParallelism = 8 },
                        i => formatTextWriter.Write(FormatResources.ButIMustExplain));
                    watch.Stop();
                    Trace.WriteLine(watch.Elapsed.TotalMilliseconds);
                    Assert.AreEqual(9, formatTextWriter.Position);
                    string result = stringWriter.ToString();

                    string[] lines = result
                        .Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                    // Check number of lines and maximum line length, if we have any race conditions we expect these to change.
                    Assert.AreEqual(12501, lines.Length);
                    Assert.AreEqual(width, lines.Select(l => l.Length).Max());
                }
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public void TestThreadSafetyNestedLayout()
        {
            const ushort width = 80;
            using (StringWriter stringWriter = new StringWriter())
            {
                using (FormatTextWriter formatTextWriter = new FormatTextWriter(stringWriter, width))
                {
                    Stopwatch watch = Stopwatch.StartNew();
                    Parallel.For(
                        0,
                        1000,
                        new ParallelOptions() { MaxDegreeOfParallelism = 1 },
                        i => new FormatBuilder(width, alignment: Alignment.Justify)
                            .AppendFormat(FormatResources.ButIMustExplain, i)
                            .WriteTo(formatTextWriter));
                    watch.Stop();
                    Trace.WriteLine(watch.Elapsed.TotalMilliseconds);
                    Assert.AreEqual(9, formatTextWriter.Position);
                    string result = stringWriter.ToString();

                    string[] lines = result
                        .Split(new[] {Environment.NewLine}, StringSplitOptions.None);

                    // Check number of lines and maximum line length, if we have any race conditions we expect these to change.
                    Assert.AreEqual(12501, lines.Length);
                    Assert.AreEqual(width, lines.Select(l => l.Length).Max());
                }
            }
        }
    }
}