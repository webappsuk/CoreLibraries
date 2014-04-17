using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            FormatBuilder fb = new FormatBuilder()
                .AppendLine(ConsoleWriter.Default.Layout.ToString())
                .AppendLine(FormatResources.LoremIpsum)
                .AppendLine(FormatResources.SedUtPerspiciatis)
                .Append(FormatResources.AtVeroEos);

            ConsoleWriter.Default.WriteLine(fb);
        }
    }
}
