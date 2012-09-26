#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Copyright (c) 2012, Web Applications UK Ltd
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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;

namespace WebApplications.Utilities.Test.Extensions
{
    [TestClass]
    public class ToJSONTests : UtilitiesTestBase
    {
        // Using http://www.json.org/ as standard

        private static readonly Dictionary<string, string> _controlChars = new Dictionary<string, string>
                                                                               {
                                                                                   {@"\b", "\b"},
                                                                                   {@"\f", "\f"},
                                                                                   {@"\n", "\n"},
                                                                                   {@"\r", "\r"},
                                                                                   {@"\t", "\t"}
                                                                               };

        private static readonly Regex MatchEmptyList = new Regex(@"\[\s*\]");
        private static readonly Regex MatchListContents = new Regex(@"\[(?<contents>[^]]*)\]");
        private static readonly Regex MatchStringContents = new Regex(@"""(?<contents>(?:[^""]|\\"")*)""");
        private static readonly Regex MatchQuotationContents = new Regex(@"""(?<contents>.*)""");
        private static readonly Regex MatchParenthesesContents = new Regex(@"\((?<contents>[^)]*)\)");

        [TestMethod]
        public void ToJSON_EmptyString_IsDoubleQuotationMarkPair()
        {
            string json = string.Empty.ToJSON();
            Assert.AreEqual("\"\"", json);
        }

        [TestMethod]
        public void ToJSON_EmptyList_IsSquareBracketPair()
        {
            IEnumerable<string> list = new string[] {};
            string json = list.ToJSON();
            Assert.IsTrue(MatchEmptyList.IsMatch(json),
                          "The JSON representation of an empty list should be an empty pair of square brackets. Found <{0}>.",
                          json);
        }

        [TestMethod]
        public void ToJSON_StringWithoutEscapedChars_ConformsToStandards()
        {
            string entry = String.Format("Single entry {0}.", Random.Next());
                // Should not include characters which need escaping due to last Assert.
            string json = entry.ToJSON();
            Match stringMatch = MatchStringContents.Match(json);
            Assert.IsTrue(stringMatch.Success,
                          "The JSON representation of a string should be enclosed in double quotation marks with backslash escapes. Found <{0}>.",
                          json);
            Assert.AreEqual(entry, stringMatch.Groups["contents"].Value,
                            "An alphanumeric value should be unchanged when encapsulated using ToJSON.");
        }

        [TestMethod]
        public void ToJSON_NullString_ConformsToStandards()
        {
            string json = ((string) null).ToJSON();
            Assert.AreEqual("null", json,
                            "The JSON representation of null is the word null without any quotation marks. Found <{0}>.",
                            json);
        }

        [TestMethod]
        public void ToJSON_SingleEntryList_ConformsToStandards()
        {
            string entry = String.Format("Single entry {0}.", Random.Next());
                // Should not include characters which need escaping due to last Assert.
            IEnumerable<string> list = new[] {entry};
            string json = list.ToJSON();
            Match contentsMatch = MatchListContents.Match(json);
            Assert.IsTrue(contentsMatch.Success,
                          "The JSON representation of a list should be enclosed in square brackets. Found <{0}>.", json);
            Match stringMatch = MatchStringContents.Match(contentsMatch.Groups["contents"].Value);
            Assert.IsTrue(stringMatch.Success,
                          "The JSON representation of a string should be enclosed in double quotation marks with backslash escapes. Found <{0}>.",
                          json);
            Assert.AreEqual(entry, stringMatch.Groups["contents"].Value,
                            "An alphanumeric value should be unchanged when encapsulated using ToJSON.");
        }

        [TestMethod]
        public void ToJSON_MultiEntryList_ContainsCommaDeliminatedItems()
        {
            // These should not contain commas or characters which need escaping in order to simplify the process of seperation
            IEnumerable<string> list =
                Enumerable.Range(1, Random.Next(3, 10)).Select(n => String.Format("Single entry {0}.", Random.Next())).
                    ToList();
            string json = list.ToJSON();
            Match contentsMatch = MatchListContents.Match(json);
            Assert.IsTrue(contentsMatch.Success,
                          "The JSON representation of a list should be enclosed in square brackets. Found <{0}>.", json);
            IEnumerable<string> parsedList = contentsMatch.Groups["contents"].Value.Split(',');
            Assert.AreEqual(list.Count(), parsedList.Count(),
                            "The JSON representation of a list should contain the same number of comma deliminated values as there are items in the list.");
            foreach (Tuple<string, string> pair in parsedList.Zip(list, (a, b) => new Tuple<string, string>(a, b)))
            {
                Match stringMatch = MatchStringContents.Match(pair.Item1);
                Assert.IsTrue(stringMatch.Success,
                              "The JSON representation of a string should be enclosed in double quotation marks with backslash escapes. Found <{0}>.",
                              pair.Item1);
                Assert.AreEqual(pair.Item2, stringMatch.Groups["contents"].Value,
                                "An alphanumeric value should be unchanged when encapsulated using ToJSON.");
            }
        }

        [TestMethod]
        public void ToJSON_ControlCharactersInString_AreEscaped()
        {
            foreach (KeyValuePair<string, string> controlChar in _controlChars)
            {
                string entry = String.Format("Control char ({1}) {0}.", Random.Next(), controlChar.Value);
                string json = entry.ToJSON();
                Match stringMatch = MatchStringContents.Match(json);
                Assert.IsTrue(stringMatch.Success,
                              "The JSON representation of a string should be enclosed in double quotation marks with backslash escapes. Found <{0}>.",
                              json);
                Assert.AreEqual(controlChar.Key,
                                MatchParenthesesContents.Match(stringMatch.Groups["contents"].Value).Groups["contents"].
                                    Value,
                                "Control characters should be escaped with backslashes by ToJSON.");
            }
        }

        [TestMethod]
        public void ToJSON_ControlCharactersInList_AreEscaped()
        {
            foreach (KeyValuePair<string, string> controlChar in _controlChars)
            {
                string entry = String.Format("Control char ({1}) {0}.", Random.Next(), controlChar.Value);
                IEnumerable<string> list = new[] {entry};
                string json = list.ToJSON();
                Match contentsMatch = MatchListContents.Match(json);
                Assert.IsTrue(contentsMatch.Success,
                              "The JSON representation of a list should be enclosed in square brackets. Found <{0}>.",
                              json);
                Match stringMatch = MatchStringContents.Match(contentsMatch.Groups["contents"].Value);
                Assert.IsTrue(stringMatch.Success,
                              "The JSON representation of a string should be enclosed in double quotation marks with backslash escapes. Found <{0}>.",
                              json);
                Assert.AreEqual(controlChar.Key,
                                MatchParenthesesContents.Match(stringMatch.Groups["contents"].Value).Groups["contents"].
                                    Value,
                                "Control characters should be escaped with backslashes by ToJSON.");
            }
        }

        [TestMethod]
        public void ToJSON_SpeechMarks_AreEscaped()
        {
            string entry = String.Format("\"Ahoy!\", said the Captain. {0}.", Random.Next());
            IEnumerable<string> list = new[] {entry};
            string json = list.ToJSON();
            Match contentsMatch = MatchListContents.Match(json);
            Assert.IsTrue(contentsMatch.Success,
                          "The JSON representation of a list should be enclosed in square brackets. Found <{0}>.", json);
            Match stringMatch = MatchQuotationContents.Match(contentsMatch.Groups["contents"].Value);
            Assert.IsTrue(stringMatch.Success,
                          "The JSON representation of a string should be enclosed in double quotation marks with backslash escapes. Found <{0}>.",
                          json);
            Assert.AreEqual(entry.Replace("\"", "\\\""), stringMatch.Groups["contents"].Value,
                            "Quotation marks should be escaped with backslashes by ToJSON.");
        }

        [TestMethod]
        public void ToJSON_Backslashes_AreEscaped()
        {
            string entry = String.Format("Double escape test (\\) {0}.", Random.Next());
            IEnumerable<string> list = new[] {entry};
            string json = list.ToJSON();
            Match contentsMatch = MatchListContents.Match(json);
            Assert.IsTrue(contentsMatch.Success,
                          "The JSON representation of a list should be enclosed in square brackets. Found <{0}>.", json);
            Match stringMatch = MatchQuotationContents.Match(contentsMatch.Groups["contents"].Value);
            Assert.IsTrue(stringMatch.Success,
                          "The JSON representation of a string should be enclosed in double quotation marks with backslash escapes. Found <{0}>.",
                          json);
            Assert.AreEqual("\\\\",
                            MatchParenthesesContents.Match(stringMatch.Groups["contents"].Value).Groups["contents"].
                                Value,
                            "Backslashes should be double-escaped with backslashes by ToJSON.");
        }

        [TestMethod]
        public void AppendJSON_EmptyString_AppendsDoubleQuotationMarkPair()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendJSON(string.Empty);
            Assert.AreEqual("\"\"", stringBuilder.ToString());
        }

        [TestMethod]
        public void AppendJSON_NullString_AppendsWordNull()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendJSON(null);
            Assert.AreEqual("null", stringBuilder.ToString());
        }

        [TestMethod]
        public void AppendJSON_ExistingDataInStringBuilder_AppendsSameDataAsReturnedByToJSON()
        {
            StringBuilder stringBuilder = new StringBuilder();
            string existingData = Random.RandomString(Random.Next(10, 100));
            string addedString = Random.RandomString(Random.Next(10, 100));
            stringBuilder.Append(existingData);
            stringBuilder.AppendJSON(addedString);
            Assert.AreEqual(String.Format("{0}{1}", existingData, addedString.ToJSON()), stringBuilder.ToString());
        }
    }
}