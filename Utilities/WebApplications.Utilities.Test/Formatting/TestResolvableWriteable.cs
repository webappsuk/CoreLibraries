using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Formatting;

namespace WebApplications.Utilities.Test.Formatting
{
    [TestClass]
    public class TestResolvableWriteable
    {
        private class RWTest : ResolvableWriteable
        {
            /// <summary>
            /// The verbose format
            /// </summary>
            [PublicAPI]
            [NotNull]
            public static readonly FormatBuilder VerboseFormat = new FormatBuilder("{Key}{Value}");
            /// <summary>
            /// The verbose format
            /// </summary>
            [PublicAPI]
            [NotNull]
            public static readonly FormatBuilder ReversedFormat = new FormatBuilder("{Value}{Key}");

            public readonly string Key;
            public readonly string Value;

            /// <summary>
            /// Initializes a new instance of the <see cref="RWTest"/> class.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <param name="value">The value.</param>
            public RWTest(string key, string value)
            {
                Key = key;
                Value = value;
            }

            /// <summary>
            /// Resolves the specified tag.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="chunk">The chunk.</param>
            /// <returns>A <see cref="Resolution" />.</returns>
            public override object Resolve(FormatWriteContext context, FormatChunk chunk)
            {
                // ReSharper disable once PossibleNullReferenceException
                switch (chunk.Tag.ToLowerInvariant())
                {
                    case "verbose":
                        return VerboseFormat;
                    case "reversed":
                        return ReversedFormat;
                    case "key":
                        return Key;
                    case "value":
                        return Value;
                    case "position":
                        return new Resolution(context.Position, true);
                    case "fill":
                        int length = Math.Min(
                            120,
                            context.Layout.Width.Value - context.Layout.FirstLineIndentSize.Value);
                        char c = !string.IsNullOrEmpty(chunk.Format) ? chunk.Format[0] : '-';
                        StringBuilder fill = new StringBuilder(length + context.Position < 1 ? 2 : 4);
                        if (context.Position > 0) fill.AppendLine();
                        fill.AppendLine(new string(c, length));
                        return new Resolution(fill.ToString(), true);
                    default:
                        return Resolution.Unknown;
                }
            }

            /// <summary>
            /// Gets the default format.
            /// </summary>
            /// <value>The default format.</value>
            public override FormatBuilder DefaultFormat
            {
                get { return VerboseFormat; }
            }
        }

        [TestMethod]
        public void TestResolableUsesDefaultFormat()
        {
            FormatBuilder builder = new FormatBuilder().AppendFormat("{0}", new RWTest("key", "value"));
            Assert.AreEqual("keyvalue", builder.ToString());
        }

        [TestMethod]
        public void TestResolableUsesCustomFormat()
        {
            FormatBuilder builder = new FormatBuilder().AppendFormat("{0:{Value}}", new RWTest("key", "value"));
            Assert.AreEqual("value", builder.ToString());
        }

        [TestMethod]
        public void TestResolableUsesNamedFormat()
        {
            FormatBuilder builder = new FormatBuilder().AppendFormat("{0:{Reversed}}", new RWTest("key", "value"));
            Assert.AreEqual("valuekey", builder.ToString());
        }

        [TestMethod]
        public void TestResolableRespectsAlignmentOnDefault()
        {
            var rwt = new RWTest("key", "value");
            FormatBuilder builder = new FormatBuilder(12, alignment: Alignment.Centre).AppendFormatLine("{0}", rwt);
            FormatBuilder builder2 = new FormatBuilder(12, alignment: Alignment.Centre).AppendFormatLine("{0:{Verbose}}", rwt);
            Assert.AreEqual("  keyvalue\r\n", builder.ToString());
            Assert.AreEqual("  keyvalue\r\n", builder2.ToString());
        }

        [TestMethod]
        public void TestResolableGetsCorrectPosition()
        {
            var rwt = new RWTest("key", "value");
            FormatBuilder builder = new FormatBuilder().AppendFormatLine(" {position} {position}\r\n{position}");
            Assert.AreEqual(" 1 3\r\n0\r\n", builder.ToString(rwt));
        }

        [TestMethod]
        public void TestResolableGetsCorrectPositionWithLayoutNone()
        {
            var rwt = new RWTest("key", "value");
            FormatBuilder builder = new FormatBuilder(20).AppendFormatLine(" {position} {position}\r\n{position}");
            Assert.AreEqual(" 1 3\r\n0\r\n", builder.ToString(rwt));
        }

        [TestMethod]
        public void TestResolableGetsCorrectPositionWithLayoutLeft()
        {
            var rwt = new RWTest("key", "value");
            FormatBuilder builder = new FormatBuilder(20, alignment: Alignment.Left).AppendFormatLine(" {position} {position}\r\n{position}");
            // Position is always pre-alignment...
            Assert.AreEqual("1 3\r\n0\r\n", builder.ToString(rwt));
        }

        [TestMethod]
        public void TestDynamicFill()
        {
            var rwt = new RWTest("key", "value");
            FormatBuilder builder = new FormatBuilder(10, firstLineIndentSize:2, indentSize:4, alignment: Alignment.Left)
                .AppendFormatLine("{fill}a{fill}{fill}");
            // Position is always pre-alignment...
            Assert.AreEqual("  --------\r\n  a\r\n  --------\r\n  --------\r\n\r\n", builder.ToString(rwt));
        }
    }
}
