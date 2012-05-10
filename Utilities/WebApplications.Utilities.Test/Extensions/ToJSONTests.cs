#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities.Test
// File: ToJSONTests.cs
// 
// This software, its object code and source code and all modifications made to
// the same (the “Software”) are, and shall at all times remain, the proprietary
// information and intellectual property rights of Web Applications (UK) Limited. 
// You are only entitled to use the Software as expressly permitted by Web
// Applications (UK) Limited within the Software Customisation and
// Licence Agreement (the “Agreement”).  Any copying, modification, decompiling,
// distribution, licensing, sale, transfer or other use of the Software other than
// as expressly permitted in the Agreement is expressly forbidden.  Web
// Applications (UK) Limited reserves its rights to take action against you and
// your employer in accordance with its contractual and common law rights
// (including injunctive relief) should you breach the terms of the Agreement or
// otherwise infringe its copyright or other intellectual property rights in the
// Software.
// 
// © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
#endregion

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;

namespace WebApplications.Utilities.Test.Extensions
{
    [TestClass]
    public class ToJSONTests : UtilitiesTestBase
    {

        // Using http://www.json.org/ as standard

        private static readonly Dictionary<string, string> _controlChars = new Dictionary<string, string> { { @"\b", "\b" }, { @"\f", "\f" }, { @"\n", "\n" }, { @"\r", "\r" }, { @"\t", "\t" } };

        private static readonly Regex MatchEmptyList = new Regex(@"\[\s*\]");
        private static readonly Regex MatchListContents = new Regex(@"\[(?<contents>[^]]*)\]");
        private static readonly Regex MatchStringContents = new Regex(@"""(?<contents>(?:[^""]|\\"")*)""");
        private static readonly Regex MatchQuotationContents = new Regex(@"""(?<contents>.*)""");
        private static readonly Regex MatchParenthesesContents = new Regex(@"\((?<contents>[^)]*)\)");

        [TestMethod]
        public void ToJSON_EmptyString_IsDoubleQuotationMarkPair()
        {
            string json = string.Empty.ToJSON();
            Assert.AreEqual("\"\"",json);
        }

        [TestMethod]
        public void ToJSON_EmptyList_IsSquareBracketPair()
        {
            IEnumerable<string> list = new string[] { };
            string json = list.ToJSON();
            Assert.IsTrue(MatchEmptyList.IsMatch(json), "The JSON representation of an empty list should be an empty pair of square brackets. Found <{0}>.", json);
        }

        [TestMethod]
        public void ToJSON_StringWithoutEscapedChars_ConformsToStandards()
        {
            string entry = String.Format("Single entry {0}.", Random.Next()); // Should not include characters which need escaping due to last Assert.
            string json = entry.ToJSON();
            Match stringMatch = MatchStringContents.Match(json);
            Assert.IsTrue(stringMatch.Success, "The JSON representation of a string should be enclosed in double quotation marks with backslash escapes. Found <{0}>.", json);
            Assert.AreEqual(entry, stringMatch.Groups["contents"].Value, "An alphanumeric value should be unchanged when encapsulated using ToJSON.");
        }

        [TestMethod]
        public void ToJSON_NullString_ConformsToStandards()
        {
            string json = ((string)null).ToJSON();
            Assert.AreEqual("null", json, "The JSON representation of null is the word null without any quotation marks. Found <{0}>.",json);
        }

        [TestMethod]
        public void ToJSON_SingleEntryList_ConformsToStandards()
        {
            string entry = String.Format("Single entry {0}.", Random.Next()); // Should not include characters which need escaping due to last Assert.
            IEnumerable<string> list = new string[] { entry };
            string json = list.ToJSON();
            Match contentsMatch = MatchListContents.Match(json);
            Assert.IsTrue(contentsMatch.Success, "The JSON representation of a list should be enclosed in square brackets. Found <{0}>.", json);
            Match stringMatch = MatchStringContents.Match(contentsMatch.Groups["contents"].Value);
            Assert.IsTrue(stringMatch.Success, "The JSON representation of a string should be enclosed in double quotation marks with backslash escapes. Found <{0}>.", json);
            Assert.AreEqual(entry, stringMatch.Groups["contents"].Value, "An alphanumeric value should be unchanged when encapsulated using ToJSON.");
        }

        [TestMethod] public void ToJSON_MultiEntryList_ContainsCommaDeliminatedItems()
        {
            // These should not contain commas or characters which need escaping in order to simplify the process of seperation
            IEnumerable<string> list = Enumerable.Range(1,Random.Next(3,10)).Select( n => String.Format("Single entry {0}.", Random.Next()) ).ToList();
            string json = list.ToJSON();
            Match contentsMatch = MatchListContents.Match(json);
            Assert.IsTrue(contentsMatch.Success, "The JSON representation of a list should be enclosed in square brackets. Found <{0}>.", json);
            IEnumerable<string> parsedList = contentsMatch.Groups["contents"].Value.Split(',');
            Assert.AreEqual(list.Count(),parsedList.Count(),"The JSON representation of a list should contain the same number of comma deliminated values as there are items in the list.");
            foreach ( Tuple<string,string> pair in parsedList.Zip(list,(a,b) => new Tuple<string,string>(a,b)))
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
                Assert.IsTrue(stringMatch.Success, "The JSON representation of a string should be enclosed in double quotation marks with backslash escapes. Found <{0}>.", json);
                Assert.AreEqual(controlChar.Key, MatchParenthesesContents.Match(stringMatch.Groups["contents"].Value).Groups["contents"].Value,
                    "Control characters should be escaped with backslashes by ToJSON.");
            }
        }

        [TestMethod]
        public void ToJSON_ControlCharactersInList_AreEscaped()
        {
            foreach (KeyValuePair<string, string> controlChar in _controlChars)
            {
                string entry = String.Format("Control char ({1}) {0}.", Random.Next(), controlChar.Value);
                IEnumerable<string> list = new string[] { entry };
                string json = list.ToJSON();
                Match contentsMatch = MatchListContents.Match(json);
                Assert.IsTrue(contentsMatch.Success, "The JSON representation of a list should be enclosed in square brackets. Found <{0}>.", json);
                Match stringMatch = MatchStringContents.Match(contentsMatch.Groups["contents"].Value);
                Assert.IsTrue(stringMatch.Success, "The JSON representation of a string should be enclosed in double quotation marks with backslash escapes. Found <{0}>.", json);
                Assert.AreEqual(controlChar.Key, MatchParenthesesContents.Match(stringMatch.Groups["contents"].Value).Groups["contents"].Value,
                    "Control characters should be escaped with backslashes by ToJSON.");
            }
        }

        [TestMethod]
        public void ToJSON_SpeechMarks_AreEscaped()
        {
            string entry = String.Format("\"Ahoy!\", said the Captain. {0}.", Random.Next());
            IEnumerable<string> list = new string[] { entry };
            string json = list.ToJSON();
            Match contentsMatch = MatchListContents.Match(json);
            Assert.IsTrue(contentsMatch.Success, "The JSON representation of a list should be enclosed in square brackets. Found <{0}>.", json);
            Match stringMatch = MatchQuotationContents.Match(contentsMatch.Groups["contents"].Value);
            Assert.IsTrue(stringMatch.Success, "The JSON representation of a string should be enclosed in double quotation marks with backslash escapes. Found <{0}>.", json);
            Assert.AreEqual(entry.Replace("\"", "\\\""), stringMatch.Groups["contents"].Value,
                "Quotation marks should be escaped with backslashes by ToJSON.");
        }

        [TestMethod]
        public void ToJSON_Backslashes_AreEscaped()
        {
            string entry = String.Format("Double escape test (\\) {0}.", Random.Next());
            IEnumerable<string> list = new string[] { entry };
            string json = list.ToJSON();
            Match contentsMatch = MatchListContents.Match(json);
            Assert.IsTrue(contentsMatch.Success, "The JSON representation of a list should be enclosed in square brackets. Found <{0}>.", json);
            Match stringMatch = MatchQuotationContents.Match(contentsMatch.Groups["contents"].Value);
            Assert.IsTrue(stringMatch.Success, "The JSON representation of a string should be enclosed in double quotation marks with backslash escapes. Found <{0}>.", json);
            Assert.AreEqual("\\\\", MatchParenthesesContents.Match(stringMatch.Groups["contents"].Value).Groups["contents"].Value,
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
            Assert.AreEqual(String.Format("{0}{1}",existingData,addedString.ToJSON()), stringBuilder.ToString());
        }

    }
}
