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
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Enumerations;

namespace WebApplications.Utilities.Test.Extensions
{
    [TestClass]
    public class TruncateTests : UtilitiesTestBase
    {
        private const TruncateOptions AllOptions =
            TruncateOptions.IncludeEllipsis | TruncateOptions.FinishWord |
            TruncateOptions.AllowLastWordToGoOverMaxLength;

        #region Create test strings
        private readonly List<KeyValuePair<string, string>> _longTestStrings = new List<KeyValuePair<string, string>>();
        private readonly List<KeyValuePair<string, string>> _shortTestStrings = new List<KeyValuePair<string, string>>();
        private int _maxLength;

        private static String RandomWord(int length)
        {
            char[] letters = "qwertyuiopasdfghjklzxcvbnm".ToCharArray();
            return String.Join("", Enumerable.Range(1, length).Select(n => letters[Random.Next(0, letters.Length)]));
        }

        private static String RandomSentence(int length)
        {
            StringBuilder builder = new StringBuilder(length);
            int left = length;
            while (left > 0)
            {
                if (left <= 6)
                {
                    builder.Append(RandomWord(left));
                    left = 0;
                }
                else
                {
                    int use = Random.Next(3, Math.Min(6, left - 4) + 1);
                    builder.Append(RandomWord(use));
                    builder.Append(" ");
                    left -= use + 1;
                }
            }
            return builder.ToString();
        }

        [TestInitialize]
        public void CreateTestStrings()
        {
            _maxLength = Random.Next(20, 100);

            _shortTestStrings.Add(new KeyValuePair<string, string>("sentence shorter than max length",
                                                                   RandomSentence(Random.Next(10, _maxLength))));
            _shortTestStrings.Add(new KeyValuePair<string, string>("single word (no spaces) shorter than max length",
                                                                   RandomWord(Random.Next(3, _maxLength))));
            _shortTestStrings.Add(new KeyValuePair<string, string>("sentence equal to max length",
                                                                   RandomSentence(_maxLength)));
            _shortTestStrings.Add(new KeyValuePair<string, string>("single word (no spaces) equal to max length",
                                                                   RandomWord(_maxLength)));

            _longTestStrings.Add(new KeyValuePair<string, string>("single word (no spaces) longer than max length",
                                                                  RandomWord(_maxLength + Random.Next(1, 100))));
            _longTestStrings.Add(
                new KeyValuePair<string, string>("sentence longer than max length and breakpoint mid-word",
                                                 RandomSentence(_maxLength - Random.Next(1, 4)) +
                                                 RandomWord(Random.Next(4, 7)) + RandomSentence(Random.Next(10, 30))));
            _longTestStrings.Add(
                new KeyValuePair<string, string>("sentence longer than max length and breakpoint inside first word",
                                                 RandomWord(_maxLength + Random.Next(1, 10)) + " " +
                                                 RandomSentence(Random.Next(10, 30))));
            _longTestStrings.Add(
                new KeyValuePair<string, string>("sentence longer than max length and breakpoint after a space",
                                                 RandomSentence(_maxLength - 1) + " " +
                                                 RandomSentence(Random.Next(10, 30))));
            _longTestStrings.Add(
                new KeyValuePair<string, string>("sentence longer than max length and breakpoint before a space",
                                                 RandomSentence(_maxLength) + " " + RandomSentence(Random.Next(10, 30))));
        }
        #endregion

        #region Internal tests
        [TestMethod]
        public void InternalTest_RandomSentence_LengthAsGiven()
        {
            for (int i = 0; i < 10; i++)
            {
                int length = Random.Next(3, 100);
                Assert.AreEqual(length, RandomSentence(length).Length,
                                "The random sentence generator should create a sentence of exactly the length requested.");
            }
        }

        [TestMethod]
        public void InternalTest_RandomSentence_WordsNoLessThanThreeCharacters()
        {
            for (int i = 0; i < 3; i++)
            {
                int length = Random.Next(3, 100);
                foreach (string word in RandomSentence(length).Split(' '))
                {
                    Assert.IsTrue(word.Length >= 3,
                                  "The random sentence generator should create words of at least 3 characters.");
                }
            }
        }

        [TestMethod]
        public void InternalTest_ShortTestStrings_FourExist()
        {
            Assert.AreEqual(4, _shortTestStrings.Count, "There should be four short test strings in this test class.");
        }

        [TestMethod]
        public void InternalTest_LongTestStrings_FiveExist()
        {
            Assert.AreEqual(5, _longTestStrings.Count, "There should be five long test strings in this test class.");
        }

        [TestMethod]
        public void InternalTest_FirstTwoShortTestStrings_ShorterThanMaxLength()
        {
            foreach (KeyValuePair<string, string> testString in _shortTestStrings.Take(2))
            {
                Assert.IsTrue(testString.Value.Length < _maxLength,
                              "Short test string of '{0}' should be shorter than max length.", testString.Key);
            }
        }

        [TestMethod]
        public void InternalTest_SecondTwoShortTestStrings_EqualToMaxLength()
        {
            foreach (KeyValuePair<string, string> testString in _shortTestStrings.Skip(2).Take(2))
            {
                Assert.AreEqual(_maxLength, testString.Value.Length,
                                String.Format("Short test string of '{0}' should be exactly max length.", testString.Key));
            }
        }

        [TestMethod]
        public void InternalTest_LongTestStrings_GreaterThanMaxLength()
        {
            foreach (KeyValuePair<string, string> testString in _longTestStrings)
            {
                Assert.IsTrue(testString.Value.Length > _maxLength,
                              "Long test string of '{0}' should be shorter than max length.", testString.Key);
            }
        }

        [TestMethod]
        public void InternalTest_FirstLongTestString_ContainsNoSpaces()
        {
            KeyValuePair<string, string> testString = _longTestStrings[0];
            Assert.IsFalse(testString.Value.Contains(" "), "The {0} should not contain spaces", testString.Key);
        }

        [TestMethod]
        public void InternalTest_AllExceptFirstLongTestString_ContainSpaces()
        {
            foreach (KeyValuePair<string, string> testString in _longTestStrings.Skip(1))
            {
                Assert.IsTrue(testString.Value.Contains(" "), "The {0} should not contain spaces", testString.Key);
            }
        }

        [TestMethod]
        public void InternalTest_SecondLongTestString_HasNoSpaceEitherSideOfBreakPoint()
        {
            KeyValuePair<string, string> testString = _longTestStrings[1];
            Assert.IsFalse(' ' == testString.Value.ElementAt(_maxLength - 1),
                           "The {0} should not contain spaces just before the break-point", testString.Key);
            Assert.IsFalse(' ' == testString.Value.ElementAt(_maxLength),
                           "The {0} should not contain spaces just before the break-point", testString.Key);
        }

        [TestMethod]
        public void InternalTest_ThirdLongTestString_HasNoSpaceUntilAfterBreakPoint()
        {
            KeyValuePair<string, string> testString = _longTestStrings[2];
            Assert.IsTrue(testString.Value.IndexOf(' ') > _maxLength,
                          "The {0} should not contain spaces until after the break-point", testString.Key);
        }

        [TestMethod]
        public void InternalTest_FourthLongTestString_HasSpaceBeforeBreakPoint()
        {
            KeyValuePair<string, string> testString = _longTestStrings[3];
            Assert.IsTrue(' ' == testString.Value.ElementAt(_maxLength - 1),
                          "The {0} should contain a space just before the break-point", testString.Key);
            Assert.IsFalse(' ' == testString.Value.ElementAt(_maxLength),
                           "The {0} should not contain spaces just before the break-point", testString.Key);
        }

        [TestMethod]
        public void InternalTest_FourthLongTestString_HasSpaceAfterBreakPoint()
        {
            KeyValuePair<string, string> testString = _longTestStrings[4];
            Assert.IsFalse(' ' == testString.Value.ElementAt(_maxLength - 1),
                           "The {0} should not contain a space just before the break-point", testString.Key);
            Assert.IsTrue(' ' == testString.Value.ElementAt(_maxLength),
                          "The {0} should contain a space just before the break-point", testString.Key);
        }
        #endregion

        #region Empty input tests
        [TestMethod]
        public void Truncate_NullString_ReturnsEmptyString()
        {
            Assert.AreEqual(String.Empty, ((String) null).Truncate(Random.Next(10, 100)),
                            "Using Truncate on a null string should always result in an empty string");
        }

        [TestMethod]
        public void Truncate_EmptyString_ReturnsEmptyString()
        {
            Assert.AreEqual(String.Empty, String.Empty.Truncate(Random.Next(10, 100)),
                            "Using Truncate on an empty string should always result in an empty string");
        }

        [TestMethod]
        public void Truncate_EmptyStringWithEllipsis_ReturnsEmptyString()
        {
            Assert.AreEqual(String.Empty, String.Empty.Truncate(Random.Next(10, 100), TruncateOptions.IncludeEllipsis),
                            "Using Truncate on an empty string should always result in an empty string");
        }
        #endregion

        #region Exceptions tests
        [ExpectedException(typeof (NullReferenceException))]
        [TestMethod]
        public void Truncate_NullEllipsisString_ThrowsNullReferenceException()
        {
            // Need to have the string to be truncated long enough for it to not be immediately returned
            int maxLength = Random.Next(5, 100);
            String sentence = RandomSentence(maxLength + Random.Next(1, 10));
            String result = sentence.Truncate(maxLength, TruncateOptions.IncludeEllipsis, null);
        }

        [ExpectedException(typeof (NullReferenceException))]
        [TestMethod]
        public void Truncate_NullEllipsisStringButNoEllipsisFlag_ThrowsNullReferenceException()
        {
            // Need to have the string to be truncated long enough for it to not be immediately returned
            int maxLength = Random.Next(5, 100);
            String sentence = RandomSentence(maxLength + Random.Next(1, 10));
            String result = sentence.Truncate(maxLength, AllOptions ^ TruncateOptions.IncludeEllipsis, null);
        }

        [TestMethod]
        public void Truncate_NullEllipsisStringButEllipsisLengthGiven_Works()
        {
            // Need to have the string to be truncated long enough for it to not be immediately returned
            int maxLength = Random.Next(5, 100);
            String sentence = RandomSentence(maxLength + Random.Next(1, 10));
            String result = sentence.Truncate(maxLength, AllOptions ^ TruncateOptions.IncludeEllipsis, null,
                                              Random.Next(0, maxLength));
        }

        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        [TestMethod]
        public void Truncate_EllipsisLengthGreaterThanMaxLength_ThrowsArgumentOutOfRangeException()
        {
            // Need to have the string to be truncated long enough for it to not be immediately returned
            int maxLength = Random.Next(5, 100);
            int ellipsisLength = Random.Next(maxLength + 1, maxLength + 10);
            String sentence = RandomSentence(maxLength + Random.Next(1, 10));
            String result = sentence.Truncate(maxLength, TruncateOptions.IncludeEllipsis, "...", ellipsisLength);
        }

        [TestMethod]
        public void Truncate_EllipsisLengthGreaterThanMaxLengthButNoEllipsisFlag_ThrowsArgumentOutOfRangeException()
        {
            // Need to have the string to be truncated long enough for it to not be immediately returned
            int maxLength = Random.Next(5, 100);
            int ellipsisLength = Random.Next(maxLength + 1, maxLength + 10);
            String sentence = RandomSentence(maxLength + Random.Next(1, 10));
            String result = sentence.Truncate(maxLength, AllOptions ^ TruncateOptions.IncludeEllipsis, "...",
                                              ellipsisLength);
        }

        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        [TestMethod]
        public void Truncate_MaxLengthLessThanZero_ThrowsArgumentOutOfRangeException()
        {
            int maxLength = Random.Next(-100, 0);
            String result = Random.Next().ToString().Truncate(maxLength);
        }
        #endregion

        #region No changes expected tests
        [TestMethod]
        public void Truncate_StringShorterOrEqualToMaxLength_SameStringReturned()
        {
            foreach (KeyValuePair<string, string> testString in _shortTestStrings)
            {
                Assert.AreEqual(testString.Value, testString.Value.Truncate(_maxLength),
                                String.Format("Truncating {0} should return the string, unchanged.", testString.Key));
            }
        }

        [TestMethod]
        public void Truncate_StringShorterOrEqualToMaxLengthWithIncludeEllipsis_SameStringReturned()
        {
            foreach (KeyValuePair<string, string> testString in _shortTestStrings)
            {
                Assert.AreEqual(testString.Value, testString.Value.Truncate(_maxLength, TruncateOptions.IncludeEllipsis),
                                String.Format("Truncating {0} should return the string, unchanged.", testString.Key));
            }
        }

        [TestMethod]
        public void Truncate_StringShorterOrEqualToMaxLengthWithAllOptions_SameStringReturned()
        {
            foreach (KeyValuePair<string, string> testString in _shortTestStrings)
            {
                Assert.AreEqual(testString.Value, testString.Value.Truncate(_maxLength, AllOptions),
                                String.Format("Truncating {0} should return the string, unchanged.", testString.Key));
            }
        }
        #endregion

        #region Ends with ellipsis tests
        [TestMethod]
        public void Truncate_StringLengthGreaterThanMaxLengthWithIncludeEllipsis_StringEndsWithEllipsis()
        {
            foreach (KeyValuePair<string, string> testString in _longTestStrings)
            {
                Assert.IsTrue(testString.Value.Truncate(_maxLength, TruncateOptions.IncludeEllipsis).EndsWith("..."),
                              "When truncating a {0} with IncludeEllipsis set, the result should end with '...'.",
                              testString.Key);
            }
        }

        [TestMethod]
        public void Truncate_StringLengthGreaterThanMaxLengthWithIncludeEllipsisAndFinishWord_StringEndsWithEllipsis()
        {
            foreach (KeyValuePair<string, string> testString in _longTestStrings)
            {
                Assert.IsTrue(
                    testString.Value.Truncate(_maxLength, TruncateOptions.IncludeEllipsis | TruncateOptions.FinishWord).
                        EndsWith("..."),
                    "When truncating a {0} with IncludeEllipsis and FinishWord set, the result should end with '...'.",
                    testString.Key);
            }
        }

        [TestMethod]
        public void
            Truncate_StringLengthGreaterThanMaxLengthWithIncludeEllipsisAndAllowLastWordToGoOverMaxLength_StringEndsWithEllipsis
            ()
        {
            foreach (KeyValuePair<string, string> testString in _longTestStrings)
            {
                Assert.IsTrue(
                    testString.Value.Truncate(_maxLength,
                                              TruncateOptions.IncludeEllipsis |
                                              TruncateOptions.AllowLastWordToGoOverMaxLength).EndsWith("..."),
                    "When truncating a {0} with IncludeEllipsis and AllowLastWordToGoOverMaxLength set, the result should end with '...'.",
                    testString.Key);
            }
        }

        [TestMethod]
        public void Truncate_StringLengthGreaterThanMaxLengthWithIncludeEllipsisAndAllOptions_StringEndsWithEllipsis()
        {
            // Have to exclude the first of _longTestStrings as it will not ever be truncated
            foreach (KeyValuePair<string, string> testString in _longTestStrings.Skip(1))
            {
                Assert.IsTrue(testString.Value.Truncate(_maxLength, AllOptions).EndsWith("..."),
                              "When truncating a {0} with IncludeEllipsis, FinishWord and AllowLastWordToGoOverMaxLength set, the result should end with '...'.",
                              testString.Key);
            }
        }
        #endregion

        #region Max length not violated tests
        [TestMethod]
        public void Truncate_StringLengthGreaterThanMaxLengthWithNoOptions_StringTruncatedToMaxLength()
        {
            foreach (KeyValuePair<string, string> testString in _longTestStrings)
            {
                Assert.IsTrue(testString.Value.Truncate(_maxLength).Length == _maxLength,
                              "When truncating a {0}, the result should be equal to the max length.", testString.Key);
            }
        }

        [TestMethod]
        public void Truncate_StringLengthGreaterThanMaxLengthWithIncludeEllipsis_StringTruncatedToMaxLength()
        {
            foreach (KeyValuePair<string, string> testString in _longTestStrings)
            {
                Assert.IsTrue(
                    testString.Value.Truncate(_maxLength, TruncateOptions.IncludeEllipsis).Length == _maxLength,
                    "When truncating a {0} and with IncludeEllipsis set, the result should be equal to the max length.",
                    testString.Key);
            }
        }

        [TestMethod]
        public void Truncate_StringLengthGreaterThanMaxLengthWithFinishWord_StringTruncatedToBeShorterThanMaxLength()
        {
            foreach (KeyValuePair<string, string> testString in _longTestStrings)
            {
                Assert.IsTrue(testString.Value.Truncate(_maxLength, TruncateOptions.FinishWord).Length <= _maxLength,
                              "When truncating a {0} and with FinishWord set, the result should be shorter, or equal to the max length.",
                              testString.Key);
            }
        }

        [TestMethod]
        public void
            Truncate_StringLengthGreaterThanMaxLengthWithIncludeEllipsisAndFinishWord_StringTruncatedToBeShorterThanMaxLength
            ()
        {
            foreach (KeyValuePair<string, string> testString in _longTestStrings)
            {
                Assert.IsTrue(
                    testString.Value.Truncate(_maxLength, TruncateOptions.IncludeEllipsis | TruncateOptions.FinishWord).
                        Length <= _maxLength,
                    "When truncating a {0} and with both IncludeEllipsis and FinishWord set, the result should be shorter, or equal to the max length.",
                    testString.Key);
            }
        }
        #endregion

        #region AllowLastWordToGoOverMaxLength related tests
        [TestMethod]
        public void Truncate_AllowLastWordToGoOverMaxLengthButNotFinishWord_FlagHasNoEffect()
        {
            foreach (KeyValuePair<string, string> testString in _longTestStrings)
            {
                Assert.AreEqual(testString.Value.Truncate(_maxLength, TruncateOptions.AllowLastWordToGoOverMaxLength),
                                testString.Value.Truncate(_maxLength),
                                String.Format(
                                    "When truncating a {0} with AllowLastWordToGoOverMaxLength set but without FinishWord set, the result should be the same as without AllowLastWordToGoOverMaxLength set.",
                                    testString.Key));
            }
        }

        [TestMethod]
        public void Truncate_AllowLastWordToGoOverMaxLengthButNotFinishWordWithIncludeEllipsis_FlagHasNoEffect()
        {
            foreach (KeyValuePair<string, string> testString in _longTestStrings)
            {
                Assert.AreEqual(
                    testString.Value.Truncate(_maxLength,
                                              TruncateOptions.AllowLastWordToGoOverMaxLength |
                                              TruncateOptions.IncludeEllipsis),
                    testString.Value.Truncate(_maxLength, TruncateOptions.IncludeEllipsis),
                    String.Format(
                        "When truncating a {0} with AllowLastWordToGoOverMaxLength and IncludeEllipsis set but without FinishWord set, the result should be the same as without AllowLastWordToGoOverMaxLength set.",
                        testString.Key));
            }
        }

        [TestMethod]
        public void
            Truncate_StringLengthGreaterThanMaxLengthWithFinishWordAndAllowLastWordToGoOverMaxLength_NoSpacesAfterMaxLength
            ()
        {
            foreach (KeyValuePair<string, string> testString in _longTestStrings)
            {
                Assert.IsTrue(
                    testString.Value.Truncate(_maxLength,
                                              TruncateOptions.FinishWord |
                                              TruncateOptions.AllowLastWordToGoOverMaxLength).LastIndexOf(' ') <
                    _maxLength,
                    "When truncating a {0} with FinishWord and AllowLastWordToGoOverMaxLength set, the result should contain no spaces in the text extending past the max length. i.e. a maximum of one word past the limit.",
                    testString.Key);
            }
        }
        #endregion

        #region FinishWord related tests
        [TestMethod]
        public void Truncate_StringLengthGreaterThanMaxLengthWithFinishWord_StringEndsWithFullWord()
        {
            foreach (KeyValuePair<string, string> testString in _longTestStrings)
            {
                int resultLength = testString.Value.Truncate(_maxLength, TruncateOptions.FinishWord).Length;

                Assert.IsTrue(
                    resultLength == testString.Value.Length || testString.Value.Substring(resultLength, 1) == " " ||
                    resultLength == 0,
                    "When truncating a {0} with FinishWord set, the result should end with a full word.",
                    testString.Key);
            }
        }

        [TestMethod]
        public void
            Truncate_StringLengthGreaterThanMaxLengthWithFinishWordAndAllowLastWordToGoOverMaxLength_StringEndsWithFullWord
            ()
        {
            foreach (KeyValuePair<string, string> testString in _longTestStrings)
            {
                int resultLength =
                    testString.Value.Truncate(_maxLength,
                                              TruncateOptions.FinishWord |
                                              TruncateOptions.AllowLastWordToGoOverMaxLength).Length;

                Assert.IsTrue(
                    resultLength == testString.Value.Length || testString.Value.Substring(resultLength, 1) == " ",
                    "When truncating a {0} with FinishWord and AllowLastWordToGoOverMaxLength set, the result should end with a full word.",
                    testString.Key);
            }
        }

        [TestMethod]
        public void Truncate_StringLengthGreaterThanMaxLengthWithFinishWordAndIncludeEllipsis_StringEndsWithFullWord()
        {
            foreach (KeyValuePair<string, string> testString in _longTestStrings)
            {
                string result = testString.Value.Truncate(_maxLength,
                                                          TruncateOptions.FinishWord | TruncateOptions.IncludeEllipsis);
                int resultLength = result.Length;

                Assert.IsTrue(
                    resultLength == testString.Value.Length || testString.Value.Substring(resultLength - 3, 1) == " " ||
                    result == "...",
                    "When truncating a {0} with FinishWord and IncludeEllipsis set, the result should end with a full word or no words.",
                    testString.Key);
            }
        }

        [TestMethod]
        public void Truncate_StringLengthGreaterThanMaxLengthWithFinishWordAndAllOptions_StringEndsWithFullWord()
        {
            foreach (KeyValuePair<string, string> testString in _longTestStrings)
            {
                int resultLength = testString.Value.Truncate(_maxLength, AllOptions).Length;

                Assert.IsTrue(
                    resultLength == testString.Value.Length || testString.Value.Substring(resultLength - 3, 1) == " ",
                    "When truncating a {0} with FinishWord, AllowLastWordToGoOverMaxLength and IncludeEllipsis set, the result should end with a full word.",
                    testString.Key);
            }
        }
        #endregion

        #region Custom Ellipsis tests
        [TestMethod]
        public void Truncate_CustomEllipsis_StringEndsWithChosenEllipsis()
        {
            foreach (KeyValuePair<string, string> testString in _longTestStrings)
            {
                string ellipsis = Random.Next().ToString(CultureInfo.InvariantCulture);
                Assert.IsTrue(
                    testString.Value.Truncate(_maxLength, TruncateOptions.IncludeEllipsis, ellipsis).EndsWith(ellipsis),
                    "When truncating a {0} using a custom ellipsis, the result should end with the ellipsis specified.",
                    testString.Key);
            }
        }

        [TestMethod]
        public void Truncate_CustomEllipsis_StringTruncatedToMaxLength()
        {
            foreach (KeyValuePair<string, string> testString in _longTestStrings)
            {
                string ellipsis = Random.Next().ToString(CultureInfo.InvariantCulture);
                Assert.AreEqual(_maxLength,
                                testString.Value.Truncate(_maxLength, TruncateOptions.IncludeEllipsis, ellipsis).Length,
                                String.Format(
                                    "When truncating a {0} using a custom ellipsis, the result should be the correct length.",
                                    testString.Key));
            }
        }

        [TestMethod]
        public void
            Truncate_CustomEllipsisLength_TruncatedLengthExcludingEllipsisIsEqualToMaxLengthMinusEllipsisLengthSpecified
            ()
        {
            foreach (KeyValuePair<string, string> testString in _longTestStrings)
            {
                string ellipsis = Random.Next().ToString(CultureInfo.InvariantCulture);
                int ellipsisLength = Random.Next(0, 10);
                Assert.AreEqual(_maxLength - ellipsisLength,
                                testString.Value.Truncate(_maxLength, TruncateOptions.IncludeEllipsis, ellipsis,
                                                          ellipsisLength).Length - ellipsis.Length,
                                String.Format(
                                    "When truncating a {0} using a custom ellipsis length, the result should be the correct length after adjusting for any discrepancy between the stated and actual length of the ellipsis.",
                                    testString.Key));
            }
        }
        #endregion
    }
}