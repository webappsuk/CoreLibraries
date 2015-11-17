#region © Copyright Web Applications (UK) Ltd, 2015.  All rights reserved.
// Copyright (c) 2015, Web Applications UK Ltd
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WebApplications.Utilities.Formatting;
using WebApplications.Utilities.Globalization;

namespace WebApplications.Utilities.Test.Extensions
{
    [TestClass]
    public class FormatExtensionTests : UtilitiesTestBase
    {
        [TestMethod]
        public void SafeFormat_NullString_ThrowsArgumentNullException()
        {
            int a = Random.Next();
            int b = Random.Next(0, 256);
            const string formatString = null;
            string result = formatString.SafeFormat(a, b);
            Assert.AreEqual(
                formatString,
                result,
                "Passing a null format string to SafeFormat should result in a null output.");
        }

        [TestMethod]
        public void SafeFormat_CorrectFormatString_FormatsUsingGivenParameters()
        {
            int a = Random.Next();
            int b = Random.Next(0, 256);
            const string formatString = "Test string to format with two parameters: {1:X2} and {0}";
            Assert.AreEqual(
                String.Format(formatString, a, b),
                formatString.SafeFormat(a, b),
                "SafeFormat should use the string as a format string and format using the parameters supplied as values.");
        }

        [TestMethod]
        public void SafeFormat_CorrectFormatStringIncludingTabs_FormatsUsingGivenParametersPreservingTabs()
        {
            int a = Random.Next();
            int b = Random.Next(0, 256);
            const string formatString =
                "Test string containing tabs\t to format with two parameters: \t {1:X2} and \t {0}";
            Assert.AreEqual(
                String.Format(formatString, a, b),
                formatString.SafeFormat(a, b),
                "SafeFormat should use the string as a format string and format using the parameters supplied as values.");
        }

        [TestMethod]
        public void SafeFormat_IncorrectFormatParameters_ReturnFormatString()
        {
            int a = Random.Next();
            int b = Random.Next(0, 256);
            const string formatString = "Test string to format with too many parameters: {3} {1:X2} and {0}";
            Assert.AreEqual(
                String.Format(formatString, a, b, null, "{3}"),
                formatString.SafeFormat(a, b),
                "SafeFormat should silently return the string with as many fill points filled in if parameters are missing.");
        }

        [TestMethod]
        public void SafeFormat_IncorrectFormatString_ReturnFormatString()
        {
            int a = Random.Next();
            int b = Random.Next(0, 256);
            const string formatString = "Test string to format with invalid format: {1:z} and {0}";
            Assert.AreEqual(
                string.Format("Test string to format with invalid format: {1} and {0}", a, b),
                formatString.SafeFormat(a, b),
                "SafeFormat should fall back to the default format if the format is invalid.");
        }

        [TestMethod]
        public void SafeFormat_Test_Format_Provider_Dates()
        {
            // Pick two random cultures.
            const string formatString = "Culture: {0}, DateTime: {1}";
            Parallel.For(
                0,
                20,
                _ =>
                {
                    DateTime now = DateTime.Now;
                    ExtendedCultureInfo cultureInfo = CultureInfoProvider.Bcl.All.Choose();
                    Assert.IsNotNull(cultureInfo);
                    string s = formatString.SafeFormat((IFormatProvider)cultureInfo, cultureInfo.Name, now);
                    Trace.WriteLine(s);
                    AssertString(
                        string.Format(cultureInfo, formatString, cultureInfo.Name, now),
                        s,
                        "SafeFormat does not use the provided IFormatter.");
                });
        }

        [TestMethod]
        public void SafeFormat_Test_Escape_Brace()
        {
            Assert.AreEqual(
                "}",
                "}}".SafeFormat(0),
                "SafeFormat should treat '}}' as an escaped '}' to be consistent with string.Format.");
            Assert.AreEqual("a}a", "a}a".SafeFormat(0), "SafeFormat should treat a single '}' as '}' to be safe.");
            Assert.AreEqual(
                "}",
                "}".SafeFormat(0),
                "SafeFormat should treat a single '}' at end of line as '}' to be safe.");
            Assert.AreEqual(
                "a}",
                "a}".SafeFormat(0),
                "SafeFormat should treat a single '}' at end of line as '}' to be safe.");

            Assert.AreEqual(
                "{",
                "{{".SafeFormat(0),
                "SafeFormat should treat '{{' as an escaped '{' to be consistent with string.Format.");
            Assert.AreEqual(
                "{",
                "{".SafeFormat(0),
                "SafeFormat should treat a single '{' at the end of a line as '{' to be safe.");
            Assert.AreEqual(
                "a{",
                "a{".SafeFormat(0),
                "SafeFormat should treat a single '{' at the end of a line as '{' to be safe.");
            Assert.AreEqual("{}", "{}".SafeFormat(0), "SafeFormat should accept '{}'.");
            Assert.AreEqual("{1}", "{1}".SafeFormat(0), "SafeFormat should accept braces with invalid substitutions.");
            Assert.AreEqual("{a}", "{a}".SafeFormat(0), "SafeFormat should accept braces with invalid substitutions.");
            Assert.AreEqual("{a1}", "{a1}".SafeFormat(0), "SafeFormat should accept braces with invalid substitutions.");
        }

        [TestMethod]
        public void SafeFormat_Test_Padding()
        {
            Assert.AreEqual("  1", "{0,3}".SafeFormat(1), "Left pad does not work.");
            Assert.AreEqual("1  ", "{0,-3}".SafeFormat(1), "Right pad does not work.");
            Assert.AreEqual("  1", "{0 ,3}".SafeFormat(1), "Left pad does not work with white space.");
            Assert.AreEqual("1  ", "{0,\t-3}".SafeFormat(1), "Right pad does not work with white space.");
            Assert.AreEqual("1  ", "{0 , -3}".SafeFormat(1), "Right pad does not work with white space..");
            Assert.AreEqual("{0,-3", "{0,-3".SafeFormat(1), "SafeFormat should survive closing brace.");
        }
    }
}