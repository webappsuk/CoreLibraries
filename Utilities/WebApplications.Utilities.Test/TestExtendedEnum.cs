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
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;
using WebApplications.Utilities.Enumerations;

namespace WebApplications.Utilities.Test
{
    [TestClass]
    public class TestExtendedEnum
    {
        #region Colours enum
        [Flags]
        public enum Colours
        {
            [System.ComponentModel.Description("Descriptions are retrieved.")] None = 0,
            [System.ComponentModel.Description("And concatenated.")] DuplicateNoneName = 0,
            Red = 1,
            Rouge = 1,
            Green = 2,
            Blue = 16,
            Yellow = Red | Green,
            Cyan = Green | Blue,
            Mauve = Blue | 32
        }
        #endregion

        #region NotAFlag enum
        public enum NotAFlag
        {
            A,
            B,
            C,
            D
        }
        #endregion

        public const int Loops = 1000000;

        [TestMethod]
        [Ignore] // TODO: Create tests
        public void TestLookupPerformance()
        {
            // Load enum info.
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            ExtendedEnum<Colours>.Check(true);
            stopwatch.Stop();
            Trace.WriteLine(stopwatch.ToString("ExtendedEnum<Colours> initialization.", Loops));

            Random random = new Random();

            /*
             * Compare performance of casting
             */
            stopwatch.Restart();
            Parallel.For(0, Loops,
                         i =>
                             {
                                 long r = random.Next(6);
                                 Colours c = (Colours) r;
                                 long cl = (long) c;
                                 Assert.AreEqual(r, cl);
                             });
            stopwatch.Stop();
            Trace.WriteLine(stopwatch.ToString("{0} loops of casts.", Loops));

            // With generics we don't know the type and have to use Enum.ToObject & Convert.ToInt64
            stopwatch.Restart();
            Parallel.For(0, Loops,
                         i =>
                             {
                                 long r = random.Next(6);
                                 Colours c = (Colours) Enum.ToObject(typeof (Colours), r);
                                 long cl = Convert.ToInt64(c);
                                 Assert.AreEqual(r, cl);
                             });
            stopwatch.Stop();
            Trace.WriteLine(stopwatch.ToString("{0} loops of Enum.ToObject & Convert.ToInt64.", Loops));

            // New method
            stopwatch.Restart();
            Parallel.For(0, Loops,
                         i =>
                             {
                                 long r = random.Next(6);
                                 Colours c;
                                 Assert.IsTrue(ExtendedEnum<Colours>.TryGetValue(r, out c));
                                 long cl;
                                 Assert.IsTrue(ExtendedEnum<Colours>.TryGetLong(c, out cl));
                                 Assert.AreEqual(r, cl);
                             });
            stopwatch.Stop();
            Trace.WriteLine(stopwatch.ToString("{0} loops of ExtendedEnum<Colours> methods.", Loops));

            /*
             * Compare name lookup
             */

            stopwatch.Restart();
            Parallel.For(0, Loops,
                         i =>
                             {
                                 // Get colour consistently
                                 long r = random.Next(6);
                                 Colours c;
                                 Assert.IsTrue(ExtendedEnum<Colours>.TryGetValue(r, out c));

                                 Assert.IsNotNull(Enum.GetName(typeof (Colours), c), "Failed to get name");
                             });
            stopwatch.Stop();
            Trace.WriteLine(stopwatch.ToString("{0} loops of Enum.GetName.", Loops));

            stopwatch.Restart();
            Parallel.For(0, Loops,
                         i =>
                             {
                                 // Get colour consistently
                                 long r = random.Next(6);
                                 Colours c;
                                 Assert.IsTrue(ExtendedEnum<Colours>.TryGetValue(r, out c));

                                 string name;
                                 Assert.IsTrue(ExtendedEnum<Colours>.TryGetName(c, out name));
                             });
            stopwatch.Stop();
            Trace.WriteLine(stopwatch.ToString("{0} loops of ExtendedEnum<Colours>.TryGetName.", Loops));
        }

        [TestMethod]
        public void TestInformation()
        {
            // Not all structs are enums...
            Assert.IsFalse(ExtendedEnum<int>.IsEnum);
            Assert.IsFalse(ExtendedEnum<int>.IsFlag);

            // Test NotAFlag
            Assert.IsTrue(ExtendedEnum<NotAFlag>.IsEnum);
            Assert.IsFalse(ExtendedEnum<NotAFlag>.IsFlag);
            Assert.IsNotNull(ExtendedEnum<NotAFlag>.Names);
            Assert.AreEqual(4, ExtendedEnum<NotAFlag>.Names.Count());
            Assert.IsNotNull(ExtendedEnum<NotAFlag>.Values);
            Assert.AreEqual(4, ExtendedEnum<NotAFlag>.Values.Count());

            // Test Colours enum
            Assert.IsTrue(ExtendedEnum<Colours>.IsEnum);
            Assert.IsTrue(ExtendedEnum<Colours>.IsFlag);
            Assert.IsTrue(ExtendedEnum<Colours>.ExplicitNone);
            Assert.IsFalse(ExtendedEnum<Colours>.ExplicitAll);
            Assert.AreEqual("None", ExtendedEnum<Colours>.NoneName);
            Assert.AreEqual("Yellow | Mauve", ExtendedEnum<Colours>.AllName);
            Assert.IsNotNull(ExtendedEnum<Colours>.Names);
            Assert.AreEqual(9, ExtendedEnum<Colours>.Names.Count());
            Assert.IsNotNull(ExtendedEnum<Colours>.Values);
            Assert.AreEqual(7, ExtendedEnum<Colours>.Values.Count());
            Assert.IsNotNull(ExtendedEnum<Colours>.IndividualNames);
            Assert.AreEqual(6, ExtendedEnum<Colours>.IndividualNames.Count());
            Assert.IsNotNull(ExtendedEnum<Colours>.IndividualValues);
            Assert.AreEqual(4, ExtendedEnum<Colours>.IndividualValues.Count());
            Assert.IsNotNull(ExtendedEnum<Colours>.CombinationNames);
            Assert.AreEqual(3, ExtendedEnum<Colours>.CombinationNames.Count());
            Assert.IsNotNull(ExtendedEnum<Colours>.CombinationValues);
            Assert.AreEqual(3, ExtendedEnum<Colours>.CombinationValues.Count());
        }

        #region Test ValueDetail
        [TestMethod]
        public void TestValueDetail()
        {
            /*
             * Simple example
             */
            ExtendedEnum<Colours>.ValueDetail valueDetail = Colours.Red.GetValueDetail();
            Assert.AreEqual("Red", valueDetail.Name);
            Assert.AreEqual(Colours.Red, valueDetail.Value);
            Assert.AreEqual(1L, valueDetail.RawValue);
            Assert.AreEqual(1, valueDetail.Flags);

            // We have duplicate names
            Assert.AreEqual(2, valueDetail.AllNames.Count());
            Assert.IsTrue(valueDetail.AllNames.Contains("Red"));
            Assert.IsTrue(valueDetail.AllNames.Contains("Rouge"));

            Assert.IsTrue(valueDetail.IsExplicit);
            Assert.IsFalse(valueDetail.IsCombination);
            Assert.AreEqual(string.Empty, valueDetail.Description);

            /*
             * The 'None' Value is special, in that is always exits (even if implicit)
             * And it's Flags count is 0.
             */
            valueDetail = Colours.DuplicateNoneName.GetValueDetail();
            Assert.AreEqual(ExtendedEnum<Colours>.NoneValueDetail, valueDetail);
            Assert.AreEqual("None", valueDetail.Name);
            Assert.AreEqual(Colours.None, valueDetail.Value);
            Assert.AreEqual(Colours.DuplicateNoneName, valueDetail.Value);
            Assert.AreEqual(0L, valueDetail.RawValue);
            Assert.AreEqual(0, valueDetail.Flags);

            // We have duplicate names
            Assert.AreEqual(2, valueDetail.AllNames.Count());
            Assert.IsTrue(valueDetail.AllNames.Contains("None"));
            Assert.IsTrue(valueDetail.AllNames.Contains("DuplicateNoneName"));

            // We are explicitly defined.
            Assert.IsTrue(valueDetail.IsExplicit);
            Assert.IsFalse(valueDetail.IsCombination);

            // Because we have duplicate definitions, the descriptions are concatenated
            Assert.AreEqual(string.Format("{0}{1}{2}",
                                          "Descriptions are retrieved.",
                                          Environment.NewLine,
                                          "And concatenated."), valueDetail.Description);

            /*
             * The 'Mauve' Value is a combination value.
             */
            valueDetail = Colours.Mauve.GetValueDetail();
            Assert.AreEqual("Mauve", valueDetail.Name);
            Assert.AreEqual(Colours.Mauve, valueDetail.Value);
            Assert.AreEqual(48L, valueDetail.RawValue);
            // This is a combination flag so it's made up of two flags
            Assert.AreEqual(2, valueDetail.Flags);
            Assert.IsTrue(valueDetail.AllNames.Contains("Mauve"));
            Assert.IsTrue(valueDetail.IsExplicit);
            Assert.IsTrue(valueDetail.IsCombination);
            Assert.AreEqual(string.Empty, valueDetail.Description);

            /*
             * The '32' Value is implicit - it is defined as part of Mauve.
             * As such we have to allow implicits when we grab it's details.
             */
            valueDetail = ((Colours) 32).GetValueDetail(true);
            Assert.AreEqual("32", valueDetail.Name);
            Assert.AreEqual(((Colours) 32), valueDetail.Value);
            Assert.AreEqual(32L, valueDetail.RawValue);
            Assert.AreEqual(1, valueDetail.Flags);
            Assert.IsTrue(valueDetail.AllNames.Contains("32"));
            Assert.IsFalse(valueDetail.IsExplicit);
            Assert.IsFalse(valueDetail.IsCombination);
            Assert.AreEqual(string.Empty, valueDetail.Description);
        }
        #endregion

        #region Check examples
        [TestMethod]
        public void TestCheck()
        {
            ExtendedEnum<NotAFlag>.Check();
            ExtendedEnum<NotAFlag>.Check(false);
            ExtendedEnum<Colours>.Check();
            ExtendedEnum<Colours>.Check(TriState.True);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentException))]
        public void TestCheckFail()
        {
            ExtendedEnum<int>.Check();
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentException))]
        public void TestCheckFailFlag()
        {
            ExtendedEnum<NotAFlag>.Check(true);
        }

        [TestMethod]
        public void TestIsDefinedExplicit()
        {
            Assert.IsTrue(ExtendedEnum<Colours>.IsDefined(Colours.Red));
        }

        [TestMethod]
        public void TestIsDefinedExplicitFail()
        {
            Assert.IsFalse(ExtendedEnum<Colours>.IsDefined(32));
        }

        [TestMethod]
        public void TestIsDefinedImplicit()
        {
            Assert.IsTrue(ExtendedEnum<Colours>.IsDefined(32, true));
        }

        [TestMethod]
        public void TestIsDefinedImplicitFail()
        {
            Assert.IsFalse(ExtendedEnum<Colours>.IsDefined(8, true));
        }

        [TestMethod]
        public void TestIsNameDefinedExplicit()
        {
            Assert.IsTrue(ExtendedEnum<Colours>.IsDefined("Red"));
        }

        [TestMethod]
        public void TestIsNameDefinedExplicitFail()
        {
            Assert.IsFalse(ExtendedEnum<Colours>.IsDefined("32"));
        }

        [TestMethod]
        public void TestIsNameDefinedImplicit()
        {
            Assert.IsTrue(ExtendedEnum<Colours>.IsDefined("32", true));
            Assert.IsTrue(ExtendedEnum<Colours>.IsDefined("Red | Blue", true));
        }

        [TestMethod]
        public void TestIsNameDefinedImplicitFail()
        {
            Assert.IsFalse(ExtendedEnum<Colours>.IsDefined("8", true));
        }
        #endregion

        #region GetValue examples
        [TestMethod]
        public void TestEnumCasting()
        {
            // This is why traditional flag enums are bad!  This cast should fail as it has undefined bits.
            Colours c = (Colours) 256;
            Assert.AreEqual(256, (int) c);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void TestSafeCasting()
        {
            // This is the new safe way of casting a value to an enum, this will fail as it contains undefined flags (8).
            Colours c = 31.ToEnum<Colours>(true);
        }

        [TestMethod]
        public void TestSafeCastingImplicit()
        {
            // This will succeed as we're allowing implicit flag combinations.
            Colours c = 19.ToEnum<Colours>(true);
            Assert.AreEqual(19, (int) c);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void TestSafeCastingExplicit()
        {
            // This will fail because we don't allow implicit flag combinations
            // Red | Green | Blue = 19 but it is not explicitly defined!
            Colours c = 19.ToEnum<Colours>();
        }

        [TestMethod]
        public void TestGetValueFromName()
        {
            Colours c = ExtendedEnum<Colours>.GetValue("Yellow | Blue", true);
            Assert.AreEqual(Colours.Yellow | Colours.Blue, c);
        }

        [TestMethod]
        public void TestValueFromName2()
        {
            Colours c = ExtendedEnum<Colours>.GetValue("Green | Red", true);
            Assert.AreEqual(Colours.Yellow, c);
        }

        [TestMethod]
        public void TestNumericalGetValue()
        {
            Colours c = ExtendedEnum<Colours>.GetValue("32 | Red", true);
            Assert.AreEqual((Colours) 32 | Colours.Red, c);
        }
        #endregion

        #region GetLong tests
        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        [TestMethod]
        public void TestGetLongFromValueExplicit()
        {
            long l = ExtendedEnum<Colours>.GetLong(Colours.Red | Colours.Green | Colours.Blue);
        }

        [TestMethod]
        public void TestGetLongFromValueImplicit()
        {
            long l = ExtendedEnum<Colours>.GetLong(Colours.Red | Colours.Green | Colours.Blue, true);
            Assert.AreEqual(19, l);
        }

        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        [TestMethod]
        public void TestGetLongFromValueImplicitFail()
        {
            long l = ExtendedEnum<Colours>.GetLong((Colours) 15, true);
            Assert.AreEqual(19, l);
        }

        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        [TestMethod]
        public void TestGetLongFromNameExplicit()
        {
            long l = ExtendedEnum<Colours>.GetLong("Red | Green | Blue");
        }

        [TestMethod]
        public void TestGetLongFromNameImplicit()
        {
            long l = ExtendedEnum<Colours>.GetLong("Red | Green | Blue", true);
            Assert.AreEqual(19, l);
        }

        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        [TestMethod]
        public void TestGetLongFromNameImplicitFail()
        {
            long l = ExtendedEnum<Colours>.GetLong("Red | Green | Blue | Dog", true);
        }
        #endregion

        #region TryGetLong tests
        [TestMethod]
        public void TestTryGetLongFromValueExplicit()
        {
            long l;
            Assert.IsFalse(ExtendedEnum<Colours>.TryGetLong(Colours.Red | Colours.Green | Colours.Blue, out l));
        }

        [TestMethod]
        public void TestTryGetLongFromValueImplicit()
        {
            long l;
            Assert.IsTrue(ExtendedEnum<Colours>.TryGetLong(Colours.Red | Colours.Green | Colours.Blue, out l, true));
            Assert.AreEqual(19, l);
        }

        [TestMethod]
        public void TestTryGetLongFromValueImplicitFail()
        {
            long l;
            Assert.IsFalse(ExtendedEnum<Colours>.TryGetLong((Colours) 90, out l, true));
        }

        [TestMethod]
        public void TestTryGetLongFromNameExplicit()
        {
            long l;
            Assert.IsFalse(ExtendedEnum<Colours>.TryGetLong("Red | Green | Blue", out l));
        }

        [TestMethod]
        public void TestTryGetLongFromNameImplicit()
        {
            long l;
            Assert.IsTrue(ExtendedEnum<Colours>.TryGetLong("Red | Green | Blue", out l, true));
            Assert.AreEqual(19, l);
        }

        [TestMethod]
        public void TestTryGetLongFromNameImplicitFail()
        {
            long l;
            Assert.IsFalse(ExtendedEnum<Colours>.TryGetLong("Red | Green | Blue | Dog", out l, true));
        }
        #endregion

        #region Test GetName
        [TestMethod]
        public void TestGetNameExplicit()
        {
            Assert.AreEqual("Mauve", Colours.Mauve.GetName());
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void TestGetNameExplicitFail()
        {
            Assert.AreEqual("32", ((Colours) 32).GetName());
        }

        [TestMethod]
        public void TestGetNameImplicitDirect()
        {
            Assert.AreEqual("32", ExtendedEnum<Colours>.GetName(32, true));
        }

        [TestMethod]
        public void TestGetNameImplicit()
        {
            Assert.AreEqual("32", ((Colours) 32).GetName(true));
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void TestGetNameImplicitFail()
        {
            Assert.AreEqual("256", ((Colours) 256).GetName(true));
        }

        [TestMethod]
        public void TestGetAllName()
        {
            Assert.AreEqual(ExtendedEnum<Colours>.AllName,
                            ExtendedEnum<Colours>.AllValue.GetName(!ExtendedEnum<Colours>.ExplicitAll));
        }
        #endregion

        #region Test Bit Operations
        [TestMethod]
        public void TestAreSet()
        {
            Assert.IsTrue(Colours.Yellow.AreSet(Colours.Red));
            Assert.IsFalse(Colours.Yellow.AreSet(Colours.Blue));
            Assert.IsTrue(Colours.Mauve.AreSet((Colours) 32, true));
            Assert.IsFalse(Colours.Red.AreSet((Colours) 32, true));
        }

        [TestMethod]
        public void TestAreClear()
        {
            Assert.IsFalse(Colours.Yellow.AreClear(Colours.Red));
            Assert.IsTrue(Colours.Yellow.AreClear(Colours.Blue));
            Assert.IsFalse(Colours.Mauve.AreClear((Colours) 32, true));
            Assert.IsTrue(Colours.Red.AreClear((Colours) 32, true));
        }

        [TestMethod]
        public void TestExplicitSet()
        {
            Assert.AreEqual(Colours.Yellow, Colours.Red.Set(Colours.Green));
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void TestExplicitSetFail()
        {
            Assert.AreEqual(Colours.Red | Colours.Blue, Colours.Red.Set(Colours.Blue));
        }

        [TestMethod]
        public void TestImplicitSet()
        {
            Assert.AreEqual(Colours.Red | Colours.Blue, Colours.Red.Set(Colours.Blue, true));
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void TestImplicitSetFail()
        {
            // This fails because 8 is not an explicit or implicit flag
            Assert.AreEqual((Colours) 8 | Colours.Mauve, Colours.Mauve.Set((Colours) 8, true));
        }

        [TestMethod]
        public void TestExplicitClear()
        {
            Assert.AreEqual(Colours.Red, Colours.Yellow.Clear(Colours.Green));
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void TestExplicitClearFail()
        {
            // This succeeds as 32 is an implicit flag (part of Mauve).
            Assert.AreEqual((Colours) 32, Colours.Mauve.Clear(Colours.Blue));
        }

        [TestMethod]
        public void TestImplicitClear()
        {
            Assert.AreEqual((Colours) 32, Colours.Mauve.Clear(Colours.Blue, true));
        }

        [TestMethod]
        public void TestInvert()
        {
            // We have to include implicit flags here as All is implicit.
            Assert.IsFalse(ExtendedEnum<Colours>.ExplicitAll);
            Assert.AreEqual(ExtendedEnum<Colours>.NoneValue, ExtendedEnum<Colours>.AllValue.Invert(true));
        }

        [TestMethod]
        public void TestIntersect()
        {
            Assert.AreEqual(Colours.Green, Colours.Yellow.Intersect(Colours.Cyan));
        }
        #endregion

        #region Test split & combine
        [TestMethod]
        public void TestSplitFull()
        {
            Colours[] split = Colours.Mauve.SplitFlags(true).ToArray();
            Assert.IsNotNull(split);
            Assert.AreEqual(2, split.Length);
            Assert.AreEqual(Colours.Blue, split[0]);
            Assert.AreEqual((Colours) 32, split[1]);
        }

        [TestMethod]
        public void TestSplitIncludeCombinations()
        {
            // Blue + Yellow = Red | Green | Blue ('white')
            // if we split - allowing combinations, the precedence order will mean we get Red + Cyan,
            // which also = Red | Green | Blue
            Colours[] split = (Colours.Blue | Colours.Yellow).SplitFlags(true, true).ToArray();
            Assert.IsNotNull(split);
            Assert.AreEqual(2, split.Length);
            Assert.AreEqual(Colours.Red, split[0]);
            Assert.AreEqual(Colours.Cyan, split[1]);
        }

        [TestMethod]
        public void TestCombineExplicit()
        {
            List<Colours> colours = new List<Colours> {Colours.Red, Colours.Green};
            Assert.AreEqual(Colours.Yellow, colours.Combine());
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void TestCombineExplicitFail()
        {
            // This fails as purple is not explicitly defined.
            List<Colours> colours = new List<Colours> {Colours.Red, Colours.Blue};
            Assert.AreEqual(Colours.Red | Colours.Blue, colours.Combine());
        }

        [TestMethod]
        public void TestCombineImplicit()
        {
            // This fails as purple is not explicitly defined.
            List<Colours> colours = new List<Colours> {Colours.Red, Colours.Blue};
            Assert.AreEqual(Colours.Red | Colours.Blue, colours.Combine(true));
        }
        #endregion
    }
}