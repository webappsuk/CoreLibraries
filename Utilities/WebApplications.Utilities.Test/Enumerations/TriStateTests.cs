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
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Enumerations;

namespace WebApplications.Utilities.Test.Enumerations
{
    [TestClass]
    public class TriStateTests : UtilitiesTestBase
    {
        [TestMethod]
        public void TriState_TrueState_IsEquivilantToYesAndPositive()
        {
            Assert.AreEqual(TriState.True, TriState.Yes, "The TriStates 'yes' and 'true' should be equivalent.");
            Assert.AreEqual(
                TriState.True,
                TriState.Positive,
                "The TriStates 'positive' and 'true' should be equivalent.");
        }

        [TestMethod]
        public void TriState_FalseState_IsEquivilantToNoAndNegotive()
        {
            Assert.AreEqual(TriState.False, TriState.No, "The TriStates 'no' and 'false' should be equivalent.");
            Assert.AreEqual(
                TriState.False,
                TriState.Negative,
                "The TriStates 'negotive' and 'false' should be equivalent.");
        }

        [TestMethod]
        public void TriState_UnknownState_IsEquivilantToUndefinedAndEqual()
        {
            Assert.AreEqual(
                TriState.Unknown,
                TriState.Undefined,
                "The TriStates 'undefined' and 'unknown' should be equivalent.");
            Assert.AreEqual(
                TriState.Unknown,
                TriState.Equal,
                "The TriStates 'equal' and 'unknown' should be equivalent.");
        }

        [TestMethod]
        public void TriState_DistinctStates_AreNotEquivilent()
        {
            Assert.AreNotEqual(
                TriState.Unknown,
                TriState.True,
                "The TriStates 'true' and 'unknown' should not be equivalent.");
            Assert.AreNotEqual(
                TriState.Unknown,
                TriState.False,
                "The TriStates 'false' and 'unknown' should not be equivalent.");
            Assert.AreNotEqual(
                TriState.True,
                TriState.False,
                "The TriStates 'false' and 'true' should not be equivalent.");
        }

        [TestMethod]
        public void TriState_ConstructorWithoutValue_DefaultsToUnknown()
        {
            Assert.AreEqual(
                TriState.Unknown,
                new TriState(),
                "A TriState instance with no value should be equivalent to 'unknown'.");
        }

        [TestMethod]
        public void TriState_DefaultValue_DefaultsToUnknown()
        {
            Assert.AreEqual(
                TriState.Unknown,
                default(TriState),
                "The default TriState value should be equivalent to 'unknown'.");
        }

        [TestMethod]
        public void TriState_YesTriState_CastsToTrue()
        {
            Assert.AreEqual(
                true,
                (bool) TriState.Yes,
                "The 'yes' TriState should be equal to True when cast to a boolean.");
        }

        [TestMethod]
        public void TriState_NoTriState_CastsToFalse()
        {
            Assert.AreEqual(
                false,
                (bool) TriState.No,
                "The 'no' TriState should be equal to False when cast to a boolean.");
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidCastException))]
        public void TriState_UnknownTriStateCastToBool_ThrowsInvalidCastException()
        {
            bool value = (bool) TriState.Unknown;
        }

        [TestMethod]
        public void TriState_CastFromTrue_EquivilantToYes()
        {
            Assert.AreEqual(TriState.Yes, true, "The boolean True should cast into the 'yes' TriState.");
        }

        [TestMethod]
        public void TriState_CastFromFalse_EquivilantToNo()
        {
            Assert.AreEqual(TriState.No, false, "The boolean False should cast into the 'no' TriState.");
        }

        [TestMethod]
        public void TriState_YesTriState_CastsToOne()
        {
            Assert.AreEqual(1, (int) TriState.Yes, "The 'yes' TriState should be equal to 1 when cast to an int.");
        }

        [TestMethod]
        public void TriState_NoTriState_CastsToMinusOne()
        {
            Assert.AreEqual(-1, (int) TriState.No, "The 'no' TriState should be equal to -1 when cast to an int.");
        }

        [TestMethod]
        public void TriState_NoTriState_CastsToZero()
        {
            Assert.AreEqual(
                0,
                (int) TriState.Unknown,
                "The 'unknown' TriState should be equal to 0 when cast to an int.");
        }

        [TestMethod]
        public void TriState_CastFromOne_EquivilantToYes()
        {
            Assert.AreEqual(TriState.Yes, (TriState) 1, "The integer 1 should cast into the 'yes' TriState.");
        }

        [TestMethod]
        public void TriState_CastFromMinusOne_EquivilantToNo()
        {
            Assert.AreEqual(TriState.No, (TriState) (-1), "The integer -1 should cast into the 'no' TriState.");
        }

        [TestMethod]
        public void TriState_CastFromZero_EquivilantToUnknown()
        {
            Assert.AreEqual(TriState.Unknown, (TriState) 0, "The integer 0 should cast into the 'unknown' TriState.");
        }

        [TestMethod]
        public void TriState_CastFromPositiveInt_EquivilantToYes()
        {
            int value = Random.Next(1, int.MaxValue);
            Assert.AreEqual(TriState.Yes, (TriState) value, "Any positive integer should cast into the 'yes' TriState.");
        }

        [TestMethod]
        public void TriState_CastFromNegotiveInt_EquivilantToNo()
        {
            int value = Random.Next(int.MinValue, -1);
            Assert.AreEqual(TriState.No, (TriState) value, "Any negotive integer should cast into the 'no' TriState.");
        }

        [TestMethod]
        public void TriState_YesTriState_CastsToByteOne()
        {
            Assert.AreEqual(1, (byte) TriState.Yes, "The 'yes' TriState should be equal to 1 when cast to an byte.");
        }

        [TestMethod]
        public void TriState_NoTriState_CastsToByte255()
        {
            Assert.AreEqual(255, (byte) TriState.No, "The 'no' TriState should be equal to 255 when cast to an byte.");
        }

        [TestMethod]
        public void TriState_NoTriState_CastsToByteZero()
        {
            Assert.AreEqual(
                0,
                (byte) TriState.Unknown,
                "The 'unknown' TriState should be equal to 0 when cast to an byte.");
        }

        [TestMethod]
        public void TriState_CastFromByteOne_EquivilantToYes()
        {
            Assert.AreEqual(TriState.Yes, (TriState) ((byte) (1)), "The byte 1 should cast into the 'yes' TriState.");
        }

        [TestMethod]
        public void TriState_CastFromByte255_EquivilantToNo()
        {
            Assert.AreEqual(TriState.No, (TriState) ((byte) (255)), "The byte 255 should cast into the 'no' TriState.");
        }

        [TestMethod]
        public void TriState_CastFromByteZero_EquivilantToUnknown()
        {
            Assert.AreEqual(
                TriState.Unknown,
                (TriState) ((byte) (0)),
                "The byte 0 should cast into the 'unknown' TriState.");
        }

        [TestMethod]
        public void TriState_CastFromByteTwo_EquivilantToUnknown()
        {
            Assert.AreEqual(
                TriState.Unknown,
                (TriState) ((byte) (2)),
                "The byte 2 should cast into the 'unknown' TriState.");
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidCastException))]
        public void TriState_CastFromOutOfRangeByte_ThrowsInvalidCastException()
        {
            byte value = (byte) Random.Next(3, 254);
            TriState testTriState = (TriState) value;
        }

        [TestMethod]
        public void TriState_ToString_DefaultsToYesNoUnknownStyle()
        {
            TriState testTriState = (Random.Next() % 3 - 1);
            Assert.AreEqual(
                testTriState.ToString(TriState.Style.YesUnknownNo),
                testTriState.ToString(),
                "The string representation of a TriState should default to YesUnknownNo formatting.");
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void TriState_ToStringWithInvalidStyle_ThrowsArgumentOutOfRangeException()
        {
            TriState testTriState = (Random.Next() % 3 - 1);
            const TriState.Style testStyle = (TriState.Style) 4;
            // note: this test assumes there is no style with value 4
            Assert.AreEqual(
                "4",
                testStyle.ToString(),
                "For this test to correctly function, the style used should be invalid. When this holds, the string representation of the style is a number.");
            testTriState.ToString(testStyle);
        }

        // ReSharper disable ImpureMethodCallOnReadonlyValueField
        [TestMethod]
        public void TriState_YesStateToStringWithYesUnknownNoFormat_ReturnsYes()
        {
            Assert.AreEqual(
                "Yes",
                TriState.Yes.ToString(TriState.Style.YesUnknownNo),
                "The string representation of a Yes TriState with YesUnknownNo formatting should be 'Yes'.");
        }

        [TestMethod]
        public void TriState_NoStateToStringWithYesUnknownNoFormat_ReturnsNo()
        {
            Assert.AreEqual(
                "No",
                TriState.No.ToString(TriState.Style.YesUnknownNo),
                "The string representation of a No TriState with YesUnknownNo formatting should be 'No'.");
        }

        [TestMethod]
        public void TriState_UnknownStateToStringWithYesUnknownNoFormat_ReturnsUnknown()
        {
            Assert.AreEqual(
                "Unknown",
                TriState.Unknown.ToString(TriState.Style.YesUnknownNo),
                "The string representation of an Unknown TriState with YesUnknownNo formatting should be 'Unknown'.");
        }

        [TestMethod]
        public void TriState_YesStateToStringWithTrueUndefinedFalseFormat_ReturnsTrue()
        {
            Assert.AreEqual(
                "True",
                TriState.Yes.ToString(TriState.Style.TrueUndefinedFalse),
                "The string representation of a Yes TriState with TrueUndefinedFalse formatting should be 'True'.");
        }

        [TestMethod]
        public void TriState_NoStateToStringWithTrueUndefinedFalseFormat_ReturnsFalse()
        {
            Assert.AreEqual(
                "False",
                TriState.No.ToString(TriState.Style.TrueUndefinedFalse),
                "The string representation of a No TriState with TrueUndefinedFalse formatting should be 'False'.");
        }

        [TestMethod]
        public void TriState_UnknownStateToStringWithTrueUndefinedFalseFormat_ReturnsUndefined()
        {
            Assert.AreEqual(
                "Undefined",
                TriState.Unknown.ToString(TriState.Style.TrueUndefinedFalse),
                "The string representation of an Unknown TriState with TrueUndefinedFalse formatting should be 'Undefined'.");
        }

        [TestMethod]
        public void TriState_YesStateToStringWithNegativeEqualPositiveFormat_ReturnsPositive()
        {
            Assert.AreEqual(
                "Positive",
                TriState.Yes.ToString(TriState.Style.NegativeEqualPositive),
                "The string representation of a Yes TriState with NegativeEqualPositive formatting should be 'Positive'.");
        }

        [TestMethod]
        public void TriState_NoStateToStringWithNegativeEqualPositiveFormat_ReturnsNegative()
        {
            Assert.AreEqual(
                "Negative",
                TriState.No.ToString(TriState.Style.NegativeEqualPositive),
                "The string representation of a No TriState with NegativeEqualPositive formatting should be 'Negative'.");
        }

        [TestMethod]
        public void TriState_UnknownStateToStringWithYesUnknownNoFormat_ReturnsEqual()
        {
            Assert.AreEqual(
                "Equal",
                TriState.Unknown.ToString(TriState.Style.NegativeEqualPositive),
                "The string representation of an Unknown TriState with NegativeEqualPositive formatting should be 'Equal'.");
        }

        // ReSharper restore ImpureMethodCallOnReadonlyValueField

        [TestMethod]
        public void TriState_IFormattableWithFormatG_GivesSameResultAsToStringYesUnknownNo()
        {
            TriState testTriState = (Random.Next() % 3 - 1);
            Assert.AreEqual(
                testTriState.ToString("G", null),
                testTriState.ToString(TriState.Style.YesUnknownNo),
                "The result of IFormattable.ToString with format 'G' should be the same as ToString with style YesUnknownNo.");
        }

        [TestMethod]
        public void TriState_IFormattableWithFormatY_GivesSameResultAsToStringYesUnknownNo()
        {
            TriState testTriState = (Random.Next() % 3 - 1);
            Assert.AreEqual(
                testTriState.ToString("Y", null),
                testTriState.ToString(TriState.Style.YesUnknownNo),
                "The result of IFormattable.ToString with format 'Y' should be the same as ToString with style YesUnknownNo.");
        }

        [TestMethod]
        public void TriState_IFormattableWithFormatT_GivesSameResultAsToStringTrueUndefinedFalse()
        {
            TriState testTriState = (Random.Next() % 3 - 1);
            Assert.AreEqual(
                testTriState.ToString("T", null),
                testTriState.ToString(TriState.Style.TrueUndefinedFalse),
                "The result of IFormattable.ToString with format 'T' should be the same as ToString with style TrueUndefinedFalse.");
        }

        [TestMethod]
        public void TriState_IFormattableWithFormatN_GivesSameResultAsToStringNegativeEqualPositive()
        {
            TriState testTriState = (Random.Next() % 3 - 1);
            Assert.AreEqual(
                testTriState.ToString("N", null),
                testTriState.ToString(TriState.Style.NegativeEqualPositive),
                "The result of IFormattable.ToString with format 'N' should be the same as ToString with style NegativeEqualPositive.");
        }

        [TestMethod]
        public void TriState_IFormattableWithValidStyle_GivesSameResultAsToString()
        {
            Random randSource = new Random();
            TriState testTriState = (Random.Next() % 3 - 1);
            TriState.Style testStyle =
                (new List<TriState.Style>
                {
                    TriState.Style.YesUnknownNo,
                    TriState.Style.NegativeEqualPositive,
                    TriState.Style.TrueUndefinedFalse
                }).OrderBy(x => randSource.Next()).First();
            Assert.AreEqual(
                testTriState.ToString(testStyle),
                testTriState.ToString(testStyle.ToString(), null),
                "The result of IFormattable.ToString with the string representation of a style as the format should be the same as ToString with that same style.");
        }

        [TestMethod]
        [ExpectedException(typeof (FormatException))]
        public void TriState_IFormattableWithInvalidStyle_ThrowsFormatException()
        {
            Random randSource = new Random();
            TriState testTriState = (Random.Next() % 3 - 1);
            String testInvalidStyle = "ThisIsNotAValidStyleType";
            TriState.Style testStyle;
            Assert.IsFalse(Enum.TryParse(testInvalidStyle, false, out testStyle));
            String value = testTriState.ToString("ThisIsNotAValidStyleType", null);
        }
    }
}