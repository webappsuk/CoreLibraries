#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities.Test
// File: MiscellaneousExtensionsTests.cs
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
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;
using WebApplications.Utilities;

namespace WebApplications.Utilities.Test.Extentions
{
    [TestClass]
    public class MiscellaneousExtensionsTests : TestBase
    {

        [TestMethod]
        public void EpochStart_Value_IsUnixEpoch()
        {
            Assert.AreEqual(new DateTime(1970, 1, 1), Utilities.Extensions.EpochStart, "The value of EpochStart should be the UNIX epoch: 1st Jan 1970.");
        }

        [TestMethod]
        public void DefaultSplitCharacters_Value_AreAsGivenHere()
        {
            var expectedValue = new[] { ' ', ',', '\t', '\r', '\n', '|' };
            Assert.AreEqual(expectedValue.Length, Utilities.Extensions.DefaultSplitChars.Length, "The value of the DefaultSplitChars array should be space, comma, tab, carriage return, newline, and pipe.");
            foreach (Char value in expectedValue)
            {
                Assert.IsTrue(Utilities.Extensions.DefaultSplitChars.Contains(value), String.Format("The character '{0}' should be included in the DefaultSplitChars array", value));
            }
        }

        [TestMethod]
        public void ToOrdinal_Int_SameAsGetOrdinalWithValuePrepended()
        {
            int value = Random.Next();
            Assert.AreEqual(String.Format("{0}{1}",value.ToString(),value.GetOrdinal() ), value.ToOrdinal(), "The result of int.ToOrdinal should be the same as prepending the value to the result of int.GetOrdinal.");
        }

        [TestMethod]
        public void GetOrdinal_ValueInTeens_OrdinalIsTh()
        {
            int hundreds = Random.Next(0, 1000);
            int tens = 1;
            int units = Random.Next(0, 10);
            int value = hundreds * 100 + tens * 10 + units;
            Assert.AreEqual("th", value.GetOrdinal(), "The ordinal of any number where the next to last digit is 1 (e.g. 3219 and 12) is 'th'.");
        }

        [TestMethod]
        public void GetOrdinal_ValueEndsInOne_OrdinalIsSt()
        {
            int hundreds = Random.Next(0, 1000);
            // rule out the case of tens=1
            int tens;
            do
            {
                tens = Random.Next(0, 10);
            } while (tens == 1);
            int units = 1;
            int value = hundreds * 100 + tens * 10 + units;
            Assert.AreEqual("st", value.GetOrdinal(), "The ordinal of any number ending in a 1 (e.g. 21) is 'st'.");
        }

        [TestMethod]
        public void GetOrdinal_ValueEndsInTwo_OrdinalIsNd()
        {
            int hundreds = Random.Next(0, 1000);
            // rule out the case of tens=1
            int tens;
            do
            {
                tens = Random.Next(0, 10);
            } while (tens == 1);
            int units = 2;
            int value = hundreds * 100 + tens * 10 + units;
            Assert.AreEqual("nd", value.GetOrdinal(), "The ordinal of any number ending in a 2 (e.g. 32) is 'nd'.");
        }

        [TestMethod]
        public void GetOrdinal_ValueEndsInThree_OrdinalIsRd()
        {
            int hundreds = Random.Next(0, 1000);
            // rule out the case of tens=1
            int tens;
            do
            {
                tens = Random.Next(0, 10);
            } while (tens == 1);
            int units = 3;
            int value = hundreds * 100 + tens * 10 + units;
            Assert.AreEqual("rd", value.GetOrdinal(), "The ordinal of any number ending in a 1 (e.g. 453) is 'rd'.");
        }

        [TestMethod]
        public void GetOrdinal_ValueEndsInfourOrGreater_OrdinalIsth()
        {
            int hundreds = Random.Next(0, 1000);
            // rule out the case of tens=1
            int tens;
            do
            {
                tens = Random.Next(0, 10);
            } while (tens == 1);
            int units = Random.Next(4, 10);
            int value = hundreds * 100 + tens * 10 + units;
            Assert.AreEqual("th", value.GetOrdinal(), "The ordinal of any number ending in a 4 or greater (e.g. 454 or 929) is 'th'.");
        }

        [TestMethod]
        public void ToEnglish_Int_SameResultAsForDouble()
        {
            int value = Random.Next();
            Assert.AreEqual(((double)value).ToEnglish(), value.ToEnglish(), "The result of int.ToEnglish should be the same as with double.ToEnglish.");
        }

        [TestMethod]
        public void ToEnglish_Long_SameResultAsForDouble()
        {
            int value = Random.Next();
            Assert.AreEqual(((double)value).ToEnglish(), value.ToEnglish(), "The result of long.ToEnglish should be the same as with double.ToEnglish.");
        }

        [TestMethod]
        public void ToEnglish_NegativeNumber_PrefixedByNegative()
        {
            double value = -Random.Next();
            Assert.IsTrue(value.ToEnglish().StartsWith("Negative "), "For values less than zero, the result of ToEnglish should begin with 'Negative '.");
        }

        [TestMethod]
        public void ToEnglish_NegativeNumber_SameAsPositivePrefixedByWordNegative()
        {
            double value = Random.Next();
            Assert.AreEqual(String.Format("Negative {0}",value.ToEnglish()),(-value).ToEnglish(), "The result of ToEnglish for a negative number should be the same as its positive equivilant plus a prefix of 'Negative '.");
        }

        [TestMethod]
        public void ToEnglish_Zero_GivesZero()
        {
            Assert.Fail("Currently causes an infinite loop");
            Assert.AreEqual("Zero", 0.ToEnglish(), "The result of ToEnglish for the number 0 should be 'Zero'.");
        }

        [TestMethod]
        public void ToEnglish_ValueInHundreds_ProvidesWordAndAfterHundred()
        {
            int value = Random.Next(1, 100);
            Assert.AreEqual(String.Format("One Hundred And {0}",value.ToEnglish()), (100+value).ToEnglish(), "The result of ToEnglish for a number between 101 and 199 should start with 'One Hundred And'.");
        }

        [TestMethod]
        public void ToEnglish_FractionalValue_ContainsPoint()
        {
            double value = Random.Next() * Math.Pow(10, -Random.Next(1, 10));
            Assert.IsTrue(value.ToEnglish().Contains(" Point "), "Where values contain a fractional component, the result of ToEnglish should contain ' Point '.");
        }

        [Ignore] // TODO: finish this test
        [TestMethod]
        public void ToEnglish_FractionalValue_WordsAfterPointMatchesNumberOfDecimalPlaces()
        {
            int numPlaces = Random.Next(1, 20);
            double value = Random.Next() * Math.Pow(10, -numPlaces);
            Assert.AreEqual(numPlaces, value.ToEnglish().Split(new string[]{" Point "},StringSplitOptions.None)[1].Split(new string[]{" "},StringSplitOptions.None).Length, 
                "Where values contain a fractional component, the number of words after the word Point should be equal to the number of decimal places.");
        }

        [TestMethod]
        public void EqualsByRuntimeType_BothInputsNull_ReturnsTrue()
        {
            Assert.IsTrue(((object) null).EqualsByRuntimeType(null), "Using EqualsByRuntimeType to compare two nulls should return true.");
        }

        [TestMethod]
        public void EqualsByRuntimeType_FirstInputOnlyNull_ReturnsFalse()
        {
            Assert.IsFalse(((object)null).EqualsByRuntimeType(Random.Next()), "Using EqualsByRuntimeType to compare a null with anything not null should return false.");
        }

        [TestMethod]
        public void EqualsByRuntimeType_SecondInputOnlyNull_ReturnsFalse()
        {
            Assert.IsFalse(Random.Next().EqualsByRuntimeType(null), "Using EqualsByRuntimeType to compare a null with anything not null should return false.");
        }

        [TestMethod]
        public void EqualsByRuntimeType_InputsOfDifferentTypes_ReturnsFalse()
        {
            int value = Random.Next();
            Assert.IsFalse(value.EqualsByRuntimeType((long)value), "Using EqualsByRuntimeType to compare values of different types should return false.");
        }

        [TestMethod]
        public void EqualsByRuntimeType_InputsOfSameTypesButNotEqual_ReturnsFalse()
        {
            int value = Random.Next();
            Assert.IsFalse(value.EqualsByRuntimeType(value - Random.Next(1, 20)), "Using EqualsByRuntimeType to compare different values of the same type should return false.");
        }

        [TestMethod]
        public void EqualsByRuntimeType_InputsOfSameTypesAndEqual_ReturnsTrue()
        {
            int value = Random.Next();
            Assert.IsTrue(value.EqualsByRuntimeType(value), "Using EqualsByRuntimeType to compare equal values of the same type should return true.");
        }

        [TestMethod]
        public void DeepEquals_IdenticalLists_ReturnsTrue()
        {
            List<int> list = new List<int>() { 1, 2, 3, 4, 5, 6 };
            Assert.IsTrue(list.DeepEquals(list), "DeepEquals should be true for identical lists.");
        }

        [TestMethod]
        public void DeepEquals_BothNull_ReturnsTrue()
        {
            List<int> list = null;
            Assert.IsTrue(list.DeepEquals(null), "DeepEquals should be true for identical lists, even if they are both null.");
        }

        [TestMethod]
        public void DeepEquals_OneNull_ReturnsFalse()
        {
            List<int> list = new List<int>() { 1, 2, 3, 4, 5, 6 };
            Assert.IsFalse(list.DeepEquals(null), "DeepEquals should be false if only one list is null.");
            Assert.IsFalse(((List<int>)null).DeepEquals(list), "DeepEquals should be false if only one list is null.");
        }

        [TestMethod]
        public void DeepEquals_ListsOfSameSizeButOneDifferentValue_ReturnsFalse()
        {
            List<int> listA = new List<int>() { 1, 2, 3, 4, 5, 6 };
            List<int> listB = new List<int>() { 1, 2, 3, 99, 5, 6 };
            Assert.IsTrue(listA.DeepEquals(listB), "DeepEquals should be false for lists of identical length but with different values.");
        }

        [TestMethod]
        public void DeepEquals_EquivilantListsWithDuplicatedValues_ReturnsTrue()
        {
            List<int> listA = new List<int>() { 1, 2, 3, 4, 3, 6 };
            List<int> listB = new List<int>() { 1, 4, 3, 3, 2, 6 };
            Assert.IsTrue(listA.DeepEquals(listB), "DeepEquals should be true for lists whose contents contains duplicates, but the same number of each .");
        }

        [TestMethod]
        public void DeepEquals_ListsWithVaryingNumberOfDuplicatedValues_ReturnsFalse()
        {
            List<int> listA = new List<int>() { 1, 2, 3, 3, 5, 6 };
            List<int> listB = new List<int>() { 1, 2, 3, 5, 5, 6 };
            Assert.IsFalse(listA.DeepEquals(listB), "DeepEquals should be false for lists containing the same values, but with different amounts of each duplicated.");
        }

        [TestMethod]
        public void DeepEquals_ListsOfDifferentSize_ReturnsFalse()
        {
            List<int> listA = new List<int>() { 1, 2, 3, 4, 5, 6 };
            List<int> listB = new List<int>() { 1, 2, 3, 5, 6 };
            Assert.IsFalse(listA.DeepEquals(listB), "DeepEquals should be false for lists of different length.");
        }

        [TestMethod]
        public void DeepEquals_WithComparerIdenticalLists_ReturnsTrue()
        {
            List<string> list = new List<string>() { "a", "b", "c", "d", "e" };
            Assert.IsTrue(list.DeepEquals(list, StringComparer.OrdinalIgnoreCase), "DeepEquals should be true for identical lists.");
        }

        [TestMethod]
        public void DeepEquals_WithComparerEquivilantLists_ReturnsTrue()
        {
            List<string> list1 = new List<string>() { "a", "b", "C", "D", "e" };
            List<string> list2 = new List<string>() { "a", "B", "c", "d", "e" };
            Assert.IsTrue(list1.DeepEquals(list2, StringComparer.OrdinalIgnoreCase), "DeepEquals should be true for list where all items are deemed equivilant by the comparer supplied.");
        }

        [TestMethod]
        public void DeepEquals_WithComparerBothNull_ReturnsTrue()
        {
            List<string> list = null;
            Assert.IsTrue(list.DeepEquals(null, StringComparer.OrdinalIgnoreCase), "DeepEquals should be true for identical lists, even if they are both null.");
        }

        [TestMethod]
        public void DeepEquals_WithComparerOneNull_ReturnsFalse()
        {
            List<string> list = new List<string>() { "a", "b", "c", "d", "e" };
            Assert.IsFalse(list.DeepEquals(null, StringComparer.OrdinalIgnoreCase), "DeepEquals should be false if only one list is null.");
            Assert.IsFalse(((List<string>)null).DeepEquals(list, StringComparer.OrdinalIgnoreCase), "DeepEquals should be false if only one list is null.");
        }

        [TestMethod]
        public void DeepEquals_WithComparerListsOfSameSizeButOneDifferentValue_ReturnsFalse()
        {
            List<string> listA = new List<string>() { "a", "b", "c", "d", "e" };
            List<string> listB = new List<string>() { "a", "b", "c", "z", "e" };
            Assert.IsFalse(listA.DeepEquals(listB, StringComparer.OrdinalIgnoreCase), "DeepEquals should be false for lists of identical length but with different values.");
        }

        [TestMethod]
        public void DeepEquals_WithComparerEquivilantListsWithDuplicatedValues_ReturnsTrue()
        {
            List<string> listA = new List<string>() { "a", "b", "B", "d", "e" };
            List<string> listB = new List<string>() { "a", "B", "d", "B", "e" };
            Assert.IsTrue(listA.DeepEquals(listB, StringComparer.OrdinalIgnoreCase), "DeepEquals should be true for lists whose contents contains duplicates, but the same number of each .");
        }

        [TestMethod]
        public void DeepEquals_WithComparerListsWithVaryingNumberOfDuplicatedValues_ReturnsFalse()
        {
            List<string> listA = new List<string>() { "a", "b", "d", "d", "e" };
            List<string> listB = new List<string>() { "a", "b", "b", "d", "e" };
            Assert.IsFalse(listA.DeepEquals(listB, StringComparer.OrdinalIgnoreCase), "DeepEquals should be false for lists containing the same values, but with different amounts of each duplicated.");
        }

        [TestMethod]
        public void DeepEquals_WithComparerListsOfDifferentSize_ReturnsFalse()
        {
            List<string> listA = new List<string>() { "a", "b", "c", "d", "e" };
            List<string> listB = new List<string>() { "a", "b", "c", "d", "e", "f" };
            Assert.IsFalse(listA.DeepEquals(listB, StringComparer.OrdinalIgnoreCase), "DeepEquals should be false for lists of different length.");
        }

        [TestMethod]
        public void DeepEqualsSimple_IdenticalLists_ReturnsTrue()
        {
            List<int> list = new List<int>() { 1, 2, 3, 4, 5, 6 };
            Assert.IsTrue(list.DeepEqualsSimple(list), "DeepEqualsSimple should be true for identical lists.");
        }

        [TestMethod]
        public void DeepEqualsSimple_BothNull_ReturnsTrue()
        {
            List<int> list = null;
            Assert.IsTrue(list.DeepEqualsSimple(null), "DeepEqualsSimple should be true for identical lists, even if they are both null.");
        }

        [TestMethod]
        public void DeepEqualsSimple_OneNull_ReturnsFalse()
        {
            List<int> list = new List<int>() { 1, 2, 3, 4, 5, 6 };
            Assert.IsFalse(list.DeepEqualsSimple(null), "DeepEqualsSimple should be false if only one list is null.");
            Assert.IsFalse(((List<int>)null).DeepEqualsSimple(list), "DeepEqualsSimple should be false if only one list is null.");
        }

        [TestMethod]
        public void DeepEqualsSimple_ListsIdenticalValuesInDifferentOrder_ReturnsTrue()
        {
            List<int> listA = new List<int>() { 1, 2, 3, 4, 5, 6 };
            List<int> listB = new List<int>() { 2, 3, 1, 6, 5, 4 };
            Assert.IsTrue(listA.DeepEqualsSimple(listB), "DeepEqualsSimple should be true for lists of identical content but with different orders.");
        }

        [TestMethod]
        public void DeepEqualsSimple_ListsOfSameSizeButOneDifferentValue_ReturnsFalse()
        {
            List<int> listA = new List<int>() { 1, 2, 3, 4, 5, 6 };
            List<int> listB = new List<int>() { 1, 2, 3, 99, 5, 6 };
            Assert.IsFalse(listA.DeepEqualsSimple(listB), "DeepEqualsSimple should be false for lists of identical length but with different values.");
        }

        [TestMethod]
        public void DeepEqualsSimple_ListsOfDifferentSize_ReturnsFalse()
        {
            List<int> listA = new List<int>() { 1, 2, 3, 4, 5, 6 };
            List<int> listB = new List<int>() { 1, 2, 3, 5, 6 };
            Assert.IsFalse(listA.DeepEqualsSimple(listB), "DeepEqualsSimple should be false for lists of different length.");
        }

        [TestMethod]
        public void DeepEqualsSimple_WithComparerIdenticalLists_ReturnsTrue()
        {
            List<string> list = new List<string>() { "a", "b", "c", "d", "e" };
            Assert.IsTrue(list.DeepEqualsSimple(list, StringComparer.OrdinalIgnoreCase), "DeepEqualsSimple should be true for identical lists.");
        }

        [TestMethod]
        public void DeepEqualsSimple_WithComparerEquivilantLists_ReturnsTrue()
        {
            List<string> list1 = new List<string>() { "a", "b", "C", "D", "e" };
            List<string> list2 = new List<string>() { "a", "B", "c", "d", "e" };
            Assert.IsTrue(list1.DeepEqualsSimple(list2, StringComparer.OrdinalIgnoreCase), "DeepEqualsSimple should be true for list where all items are deemed equivilant by the comparer supplied.");
        }

        [TestMethod]
        public void DeepEqualsSimple_WithComparerBothNull_ReturnsTrue()
        {
            List<string> list = null;
            Assert.IsTrue(list.DeepEqualsSimple(null, StringComparer.OrdinalIgnoreCase), "DeepEqualsSimple should be true for identical lists, even if they are both null.");
        }

        [TestMethod]
        public void DeepEqualsSimple_WithComparerOneNull_ReturnsFalse()
        {
            List<string> list = new List<string>() { "a", "b", "c", "d", "e" };
            Assert.IsFalse(list.DeepEqualsSimple(null, StringComparer.OrdinalIgnoreCase), "DeepEqualsSimple should be false if only one list is null.");
            Assert.IsFalse(((List<string>)null).DeepEqualsSimple(list, StringComparer.OrdinalIgnoreCase), "DeepEqualsSimple should be false if only one list is null.");
        }

        [TestMethod]
        public void DeepEqualsSimple_WithComparerListsOfSameSizeButOneDifferentValue_ReturnsFalse()
        {
            List<string> listA = new List<string>() { "a", "b", "c", "d", "e" };
            List<string> listB = new List<string>() { "a", "b", "c", "z", "e" };
            Assert.IsFalse(listA.DeepEqualsSimple(listB, StringComparer.OrdinalIgnoreCase), "DeepEqualsSimple should be false for lists of identical length but with different values.");
        }

        [TestMethod]
        public void DeepEqualsSimple_WithComparerListsOfDifferentSize_ReturnsFalse()
        {
            List<string> listA = new List<string>() { "a", "b", "c", "d", "e" };
            List<string> listB = new List<string>() { "a", "b", "c", "d", "e", "f" };
            Assert.IsFalse(listA.DeepEqualsSimple(listB, StringComparer.OrdinalIgnoreCase), "DeepEqualsSimple should be false for lists of different length.");
        }

        [TestMethod]
        public void ToDictionary_EmptyNameValueCollection_GivesEmptyDict()
        {
            NameValueCollection nameValueCollection = new NameValueCollection();
            Dictionary<string, string> dict = nameValueCollection.ToDictionary();
            Assert.AreEqual(0, dict.Count, "Converting an empty nameValueCollection with ToDictionary should result in an empty dictionary");
        }

        [TestMethod]
        public void ToDictionary_NameValueCollection_HasSameItemCount()
        {
            NameValueCollection nameValueCollection = new NameValueCollection();
            int count = Random.Next(1, 20);
            for (int i = 0; i < count; i++)
            {
                nameValueCollection.Set(String.Format("{1} {0}", i, Random.Next()), Random.Next().ToString(CultureInfo.InvariantCulture));
            }
            Dictionary<string, string> dict = nameValueCollection.ToDictionary();
            Assert.AreEqual(nameValueCollection.Count, dict.Count, "Converting a nameValueCollection with ToDictionary should preserve item count");
        }

        [TestMethod]
        public void ToDictionary_NameValueCollection_ValuesMatch()
        {
            NameValueCollection nameValueCollection = new NameValueCollection();
            int count = Random.Next(5, 10);
            for (int i = 0; i < count; i++)
            {
                nameValueCollection.Set(String.Format("{1} {0}", i, Random.Next()), Random.Next().ToString(CultureInfo.InvariantCulture));
            }
            Dictionary<string, string> dict = nameValueCollection.ToDictionary();
            foreach (string key in nameValueCollection)
            {
                Assert.AreEqual(nameValueCollection.Get(key), dict[key], "After converting a nameValueCollection with ToDictionary, the values of each key should be preserved.");
            }
        }

        [TestMethod]
        public void ToDictionary_NameValueCollection_CaseInsensitiveKeys()
        {
            NameValueCollection nameValueCollection = new NameValueCollection();
            // Add the values using mixed case and upper case keys even though NameValueCollections are case insensitive
            nameValueCollection.Set("Keyname", Random.Next().ToString(CultureInfo.InvariantCulture));
            nameValueCollection.Set("KEYNAME", Random.Next().ToString(CultureInfo.InvariantCulture));
            Dictionary<string, string> dict = nameValueCollection.ToDictionary();
            Assert.IsTrue(dict.ContainsKey("keyname"), "After converting a NameValueCollection with ToDictionary, the keys should be case insensitive.");
            Assert.AreEqual(nameValueCollection.Get("keyname"), dict["keyname"], "After converting a nameValueCollection with ToDictionary, the keys should be case insensitive.");
            Assert.IsTrue(dict.ContainsKey("Keyname"), "After converting a NameValueCollection with ToDictionary, the keys should be case insensitive.");
            Assert.IsTrue(dict.ContainsKey("KEYNAME"), "After converting a NameValueCollection with ToDictionary, the keys should be case insensitive.");
        }

        [TestMethod]
        public void XmlEscape_Object_ConvertsToString()
        {
            object obj = Random.Next();
            Assert.AreEqual(obj.ToString(), obj.XmlEscape());
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void XmlEscape_NullObject_ThrowsNullReferenceException()
        {
            Assert.IsNull(((object)null).XmlEscape());
        }

        [TestMethod]
        public void XmlEscape_NullString_ReturnsNull()
        {
            Assert.IsNull(((String)null).XmlEscape());
        }

        [TestMethod]
        public void XmlEscape_EmptyString_ReturnsEmptyString()
        {
            Assert.AreEqual(String.Empty, String.Empty.XmlEscape());
        }

        [TestMethod]
        public void XmlEscape_UnicodeString_OutputContainsValidXmlText()
        {
            String output = GenerateRandomString().XmlEscape();
            XmlDocument xml = new XmlDocument();
            // The following line throws an exception if the output is not valid xml
            xml.LoadXml(String.Format("<xml>{0}</xml>", output));
        }

        [TestMethod]
        public void XmlEscape_AlphaNumericString_ReturnsStringUnchanged()
        {
            const String input = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
            String output = input.XmlEscape();
            Assert.AreEqual(input,output);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetEmbeddedXml_NullAssembly_ThrowsInvalidoperationException()
        {
            Assembly assembly = null;
            // ReSharper disable ExpressionIsAlwaysNull
            assembly.GetEmbeddedXml("filename");
            // ReSharper restore ExpressionIsAlwaysNull
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetEmbeddedXml_NullFilename_ThrowsInvalidoperationException()
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            assembly.GetEmbeddedXml(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetEmbeddedXml_WhitespaceFilename_ThrowsInvalidoperationException()
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            assembly.GetEmbeddedXml("       ");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetEmbeddedXml_FilenameNotMatchingAnyManifest_ThrowsInvalidoperationException()
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            assembly.GetEmbeddedXml("this filepath is assumed not to exist");
        }

        [TestMethod]
        public void GetEpochTime_UnixEpoch_ReturnsZero()
        {
            DateTime dateTime = new DateTime(1970, 1, 1);
            Assert.AreEqual(0, dateTime.GetEpochTime(), "Performing GetEpochTime on 1st Jan 1970 should return 0.");
        }

        [TestMethod]
        public void GetEpochTime_UnixEpochPlusSomeHours_ReturnsNumberOfHoursMultipliedBy3600000()
        {
            int hours = Random.Next(1,100);
            DateTime dateTime = new DateTime(1970, 1, 1).AddHours(hours);
            Assert.AreEqual(hours*3600000, dateTime.GetEpochTime(), "Performing GetEpochTime on (1st Jan 1970 + x hours) should return x*3600000.");
        }

        [TestMethod]
        public void GetEpochTime_DateTimeWithLocalTimeOutsideDaylightSavingTime_ReturnsMillisecondsPastUnixEpoch()
        {
            long seconds;
            DateTime dateTime;
            do
            {
                seconds = Random.Next();
                dateTime = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Local).AddSeconds(seconds);
            } while (dateTime.IsDaylightSavingTime());
            Assert.AreEqual(seconds * 1000, dateTime.GetEpochTime(), "Performing GetEpochTime outside of daylight saving time should return the number of milliseconds past the unix epoch.");
        }

        [TestMethod]
        public void GetEpochTime_DateTimeWithLocalTimeInDaylightSavingTime_ReturnsMillisecondsPastUnixEpoch()
        {
            long seconds;
            DateTime dateTime;
            do
            {
                seconds = Random.Next();
                dateTime = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Local).AddSeconds(seconds);
            } while (!dateTime.IsDaylightSavingTime());
            Assert.AreEqual(seconds * 1000, dateTime.GetEpochTime(), "Performing GetEpochTime in daylight saving time should return the number of milliseconds past the unix epoch.");
        }

        [TestMethod]
        public void GetEpochTime_DateTimeWithUTC_ReturnsMillisecondsPastUnixEpoch()
        {
            long seconds = Random.Next();
            DateTime dateTime = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc).AddSeconds(seconds);
            Assert.AreEqual(seconds * 1000, dateTime.GetEpochTime(), "Performing GetEpochTime should return the number of milliseconds past the unix epoch.");
        }

        [TestMethod]
        public void GetDateTime_ResultFromGetEpochTime_ReturnsOriginalDateTimeToWithinMillisecond()
        {   
            DateTime dateTime = DateTime.Now.AddTicks(Random.Next()).AddSeconds(-Random.Next());
            Assert.AreEqual(0, (dateTime - Utilities.Extensions.GetDateTime(dateTime.GetEpochTime())).TotalMilliseconds, 1.0);
        }

        [TestMethod]
        public void StripHTML_Tag_StripsTag()
        {
            const string testString = "Surrounding <htmltag> Text";
            Assert.IsFalse(testString.StripHTML().Contains("htmltag"), "StripHTML should remove all HTML tags from '{0}'.", testString);
        }

        [TestMethod]
        public void StripHTML_TagWithAttributes_StripsTag()
        {
            const string testString = "Surrounding <htmltag attribute=\"value\"> Text";
            Assert.IsFalse(testString.StripHTML().Contains("htmltag"), "StripHTML should remove all HTML tags from '{0}'.", testString);
        }

        [TestMethod]
        public void StripHTML_TagSplitAcrossTwoLines_StripsTag()
        {
            const string testString = "Surrounding <htmltag\n> Text";
            Assert.IsFalse(testString.StripHTML().Contains("htmltag"), "StripHTML should remove all HTML tags from '{0}'.", testString);
        }

        [TestMethod]
        public void StripHTML_TagsWithBracketsWithin_StripsEntireTag()
        {
            const string testString = "Surrounding <htmltag<>> Text";
            Assert.IsFalse(testString.StripHTML().Contains("htmltag"), "StripHTML should remove all HTML tags from '{0}'.", testString);
        }

        [TestMethod]
        public void StripHTML_EntityEncodedTags_TagsIgnored()
        {
            const string testString = "Surrounding &lt;htmltag&gt; Text";
            Assert.IsTrue(testString.StripHTML().Contains("htmltag"), "StripHTML should not remove the entity encoded (and thus safe) tags from '{0}'.", testString);
        }

        [TestMethod]
        public void ToRadians_ValuesInRangeZeroToThreeSixty__ConvertsFromDegreesToRadians()
        {
            double degrees = Random.NextDouble() * 360;
            Assert.AreEqual((degrees * Math.PI) / 180, degrees.ToRadians(), 1e-10, "ToRadians should convert a value in degrees to the value in radians (i.e. multiply by pi/180)");
        }

        [TestMethod]
        public void ToDegrees_ValuesInRangeZeroToTwoPi__ConvertsFromRadiansToDegrees()
        {
            double radians = Random.NextDouble() * Math.PI*2;
            Assert.AreEqual((radians * 180) / Math.PI, radians.ToDegrees(), 1e-10, "ToDegrees should convert a value in radians to the value in degrees (i.e. divide by pi/180)");
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void SafeFormat_NullString_ThrowsArgumentNullException()
        {
            int a = Random.Next();
            int b = Random.Next(0, 256);
            const string formatString = null;
            // This is a deliberate attempt to break things
            // ReSharper disable AssignNullToNotNullAttribute
            string result = formatString.SafeFormat(a, b);
            // ReSharper restore AssignNullToNotNullAttribute
        }

        [TestMethod]
        public void SafeFormat_CorrectFormatString_FormatsUsingGivenParameters()
        {
            int a = Random.Next();
            int b = Random.Next(0, 256);
            const string formatString = "Test string to format with two parameters: {1:X2} and {0}";
            Assert.AreEqual(String.Format(formatString, a, b), formatString.SafeFormat(a, b), "SafeFormat should use the string as a format string and format using the parameters supplied as values.");
        }

        [TestMethod]
        public void SafeFormat_CorrectFormatStringIncludingTabs_FormatsUsingGivenParametersPreservingTabs()
        {
            int a = Random.Next();
            int b = Random.Next(0, 256);
            const string formatString = "Test string containing tabs\t to format with two parameters: \t {1:X2} and \t {0}";
            Assert.AreEqual(String.Format(formatString, a, b), formatString.SafeFormat(a, b), "SafeFormat should use the string as a format string and format using the parameters supplied as values.");
        }

        [TestMethod]
        public void SafeFormat_IncorrectFormatString_ReturnFormatString()
        {
            int a = Random.Next();
            int b = Random.Next(0, 256);
            const string formatString = "Test string to format with too many parameters: {3} {1:X2} and {0}";
            Assert.AreEqual(formatString, formatString.SafeFormat(a, b), "SafeFormat should silently return the string if the string is not a valid format string for the parameters supplied.");
        }

        [TestMethod]
        public void GetDateTime_StandardGuid_GivesSameResultAsCombGuidGetDateTime()
        {
            Guid standardGuid = Guid.NewGuid();
            Assert.AreEqual(CombGuid.GetDateTime(standardGuid), standardGuid.GetDateTime());
        }

        [TestMethod]
        public void GetDateTime_CombGuid_GivesSameResultAsCombGuidGetDateTime()
        {
            Guid standardGuid = CombGuid.NewCombGuid();
            Assert.AreEqual(CombGuid.GetDateTime(standardGuid), standardGuid.GetDateTime());
        }

    }
}
