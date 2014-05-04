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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
                    .AppendLayout(
                        alignment: Alignment.Centre,
                        firstLineIndentSize: 4,
                        indentSize: 4,
                        rightMarginSize: 4)
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
                .AppendLayout(
                    50,
                    firstLineIndentSize: 1,
                    tabStops: new[] {6, 9, 20, 30, 40})
                .Append("A\tTab Stop\tAnother");

            int position = 0;
            Assert.AreEqual(" A    Tab Stop      Another", builder.ToString(null, ref position));
            Assert.AreEqual(27, position);
            Assert.AreEqual("A  Tab Stop  Another", builder.ToString(null, ref position));
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

            Assert.IsInstanceOfType(clone, typeof (FormatBuilder));

            Assert.AreEqual(builder.ToString(), clone.ToString());
        }

        [TestMethod]
        [Timeout(10000)]
        public void TestThreadSafety()
        {
            const ushort width = 80;
            using (StringWriter stringWriter = new StringWriter())
            using (
                FormatTextWriter formatTextWriter = new FormatTextWriter(
                    stringWriter,
                    width,
                    alignment: Alignment.Justify))
            {
                Stopwatch watch = Stopwatch.StartNew();
                Parallel.For(
                    0,
                    1000,
                    new ParallelOptions {MaxDegreeOfParallelism = 8},
                    i => formatTextWriter.Write(FormatResources.ButIMustExplain));
                watch.Stop();
                Trace.WriteLine(watch.Elapsed.TotalMilliseconds);
                Assert.AreEqual(50, formatTextWriter.Position);
                string result = stringWriter.ToString();

                string[] lines = result
                    .Split(new[] {Environment.NewLine}, StringSplitOptions.None);

                // Check number of lines and maximum line length, if we have any race conditions we expect these to change.
                Assert.AreEqual(12500, lines.Length);
                Assert.AreEqual(width, lines.Select(l => l.Length).Max());
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public void TestThreadSafetyNestedLayout()
        {
            const int width = 80;
            using (StringWriter stringWriter = new StringWriter())
            using (FormatTextWriter formatTextWriter = new FormatTextWriter(stringWriter, width))
            {
                Stopwatch watch = Stopwatch.StartNew();
                Parallel.For(
                    0,
                    1000,
                    new ParallelOptions {MaxDegreeOfParallelism = 1},
                    i => new FormatBuilder(width, alignment: Alignment.Justify)
                        .Append(FormatResources.ButIMustExplain)
                        .WriteTo(formatTextWriter));
                watch.Stop();
                Trace.WriteLine(watch.Elapsed.TotalMilliseconds);
                Assert.AreEqual(50, formatTextWriter.Position);
                string result = stringWriter.ToString();

                string[] lines = result
                    .Split(new[] {Environment.NewLine}, StringSplitOptions.None);

                // Check number of lines and maximum line length, if we have any race conditions we expect these to change.
                Assert.AreEqual(12500, lines.Length);
                Assert.AreEqual(width, lines.Select(l => l.Length).Max());
            }
        }

        [TestMethod]
        public void TestKeepPunctuationTogether()
        {
            FormatBuilder builder = new FormatBuilder(11, alignment: Alignment.Justify).Append("A test line.");
            Assert.AreEqual("A      test\r\nline.", builder.ToString());

            builder = new FormatBuilder(80, alignment: Alignment.Justify)
                .Append(
                    "{0}\tBut I must explain to you how all this mistaken idea of denouncing pleasure and praising pain was born and I will give!£$%$£%1£$%£$%£$%£$%£$%£$ you.");
            Assert.AreEqual("{0}     But  I must explain  to  you how  all this  mistaken  idea of denouncing\r\npleasure  and praising  pain  was born and  I will give!£$%$£%1£$%£$%£$%£$%£$%£$\r\nyou.", builder.ToString());

            // Split on hyphen
            builder = new FormatBuilder(12, alignment: Alignment.Justify).Append("A test line-split.");
            Assert.AreEqual("A test line-\r\nsplit.", builder.ToString());

            // Try to keep apostrophies together
            builder = new FormatBuilder(12, alignment: Alignment.Justify).Append("A test line's split.");
            Assert.AreEqual("A       test\r\nline's\r\nsplit.", builder.ToString());
        }

        [TestMethod]
        public void TestCentreOnlyOnValidWidth()
        {
            Assert.AreEqual(
                "Test\r\n",
                new FormatBuilder(int.MaxValue, alignment: Alignment.Centre).AppendLine("Test").ToString());
        }

        [TestMethod]
        public void TestWordBoundaries()
        {
            Assert.AreEqual(
                "\r\n\r\nA (bracket\r\n1",
                new FormatBuilder(100).AppendLine().AppendLine("\rA (bracket").Append(1).ToString());
            Assert.AreEqual(
                "A word's\r\n",
                new FormatBuilder(100).AppendLine("A word's").ToString());
            Assert.AreEqual(
                "A bracket)\r\n",
                new FormatBuilder(100).AppendLine("A bracket)").ToString());
            Assert.AreEqual(
                "A bracket)stuff\r\n",
                new FormatBuilder(100).AppendLine("A bracket)stuff").ToString());
            Assert.AreEqual(
                "A *'!\r\n",
                new FormatBuilder(100).AppendLine("A *'!").ToString());
            Assert.AreEqual(
                "A *'!\r\n*",
                new FormatBuilder(100).AppendLine("A *'!").Append("*").ToString());

            Assert.AreEqual(
                "A\r\n(b\r\n",
                new FormatBuilder(3).AppendLine("A (b").ToString());
            Assert.AreEqual(
                "A\r\na'b\r\n",
                new FormatBuilder(4).AppendLine("A a'b").ToString());
            Assert.AreEqual(
                "A\r\nb)\r\n",
                new FormatBuilder(3).AppendLine("A b)").ToString());
            Assert.AreEqual(
                "A *\r\n'!\r\n",
                new FormatBuilder(3).AppendLine("A *'!").ToString());
        }
    }
}