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
    public class TestFormatBuilder
    {
        [TestMethod]
        public void TestFormatBuilderToString()
        {
            TestToStringFormats(new FormatBuilder());
        }

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

        public static void TestToStringFormats([NotNull] FormatBuilder builder)
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
        public void TestFormatBuilderToStringValues()
        {
            TestToStringValues(new FormatBuilder());
        }

        public static void TestToStringValues([NotNull] FormatBuilder builder)
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