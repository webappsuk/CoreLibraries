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
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebApplications.Utilities.Test.Extensions
{
    [TestClass]
    public class EqualityFunctionsTests : UtilitiesTestBase
    {
        #region Test Classes

        #region Nested type: TestTypeWithEquals
        private class TestTypeWithEquals
        {
            public TestTypeWithEquals(bool returnValue)
            {
                EqualsCalled = false;
                ReturnValue = returnValue;
            }

            public bool EqualsCalled { get; private set; }
            public bool ReturnValue { get; private set; }

            public bool Equals(TestTypeWithEquals other)
            {
                EqualsCalled = true;
                return ReturnValue;
            }
        }
        #endregion

        #region Nested type: TestTypeWithEqualsWithTooManyParameters
        private class TestTypeWithEqualsWithTooManyParameters
        {
            public TestTypeWithEqualsWithTooManyParameters(bool returnValue)
            {
                EqualsCalled = false;
                ReturnValue = returnValue;
            }

            public bool EqualsCalled { get; private set; }
            public bool ReturnValue { get; private set; }

            public bool Equals(
                TestTypeWithEqualsWithTooManyParameters other,
                TestTypeWithEqualsWithTooManyParameters extra)
            {
                EqualsCalled = true;
                return ReturnValue;
            }
        }
        #endregion

        #region Nested type: TestTypeWithEqualsWithWrongType
        private class TestTypeWithEqualsWithWrongType
        {
            public TestTypeWithEqualsWithWrongType(bool returnValue)
            {
                EqualsCalled = false;
                ReturnValue = returnValue;
            }

            public bool EqualsCalled { get; private set; }
            public bool ReturnValue { get; private set; }

            public bool Equals(TestTypeWithEquals other)
            {
                EqualsCalled = true;
                return ReturnValue;
            }
        }
        #endregion

        #region Nested type: TestTypeWithStaticEquals
        private class TestTypeWithStaticEquals
        {
            static TestTypeWithStaticEquals()
            {
                EqualsCalled = false;
            }

            public TestTypeWithStaticEquals(bool? returnValue = null)
            {
                if (returnValue.HasValue)
                    ReturnValue = returnValue.Value;
            }

            public static bool EqualsCalled { get; private set; }
            public static bool ReturnValue { get; set; }

            public static bool Equals(TestTypeWithStaticEquals first, TestTypeWithStaticEquals second)
            {
                EqualsCalled = true;
                return ReturnValue;
            }
        }
        #endregion

        #region Nested type: TestTypeWithStaticEqualsWithTooFewParameters
        private class TestTypeWithStaticEqualsWithTooFewParameters
        {
            static TestTypeWithStaticEqualsWithTooFewParameters()
            {
                EqualsCalled = false;
            }

            public TestTypeWithStaticEqualsWithTooFewParameters(bool? returnValue = null)
            {
                if (returnValue.HasValue)
                    ReturnValue = returnValue.Value;
            }

            public static bool EqualsCalled { get; private set; }
            public static bool ReturnValue { get; set; }

            public static bool Equals(TestTypeWithStaticEqualsWithTooFewParameters other)
            {
                EqualsCalled = true;
                return ReturnValue;
            }
        }
        #endregion

        #region Nested type: TestTypeWithStaticEqualsWithWrongTypes
        private class TestTypeWithStaticEqualsWithWrongTypes
        {
            static TestTypeWithStaticEqualsWithWrongTypes()
            {
                EqualsCalled = false;
            }

            public TestTypeWithStaticEqualsWithWrongTypes(bool? returnValue = null)
            {
                if (returnValue.HasValue)
                    ReturnValue = returnValue.Value;
            }

            public static bool EqualsCalled { get; private set; }
            public static bool ReturnValue { get; set; }

            public static bool Equals(TestTypeWithStaticEquals first, TestTypeWithStaticEquals second)
            {
                EqualsCalled = true;
                return ReturnValue;
            }
        }
        #endregion

        #endregion

        [TestMethod]
        public void GetTypeEqualityFunction_Integers_ReturnsFunctionWhichReturnsTrueWhenBothInputsAreEqual()
        {
            Func<object, object, bool> equalityFunction = typeof (int).GetTypeEqualityFunction();
            int value = Random.Next();
            Assert.IsTrue(
                equalityFunction(value, value),
                "The function returned by GetTypeEqualityFunction for type int should return true when both inputs are equal.");
        }

        [TestMethod]
        public void GetTypeEqualityFunction_Integers_ReturnsFunctionWhichReturnsFalseWhenBothInputsAreDifferent()
        {
            Func<object, object, bool> equalityFunction = typeof (int).GetTypeEqualityFunction();
            int value = Random.Next();
            Assert.IsFalse(
                equalityFunction(value, value - Random.Next()),
                "The function returned by GetTypeEqualityFunction for type int should return false when both inputs are different.");
        }

        [ExpectedException(typeof (InvalidCastException))]
        [TestMethod]
        public void GetTypeEqualityFunction_IntegersButInputsToResultAreWrongType_ThrowsInvalidCastException()
        {
            Func<object, object, bool> equalityFunction = typeof (int).GetTypeEqualityFunction();
            int value = Random.Next();
            Assert.IsFalse(
                equalityFunction(
                    value.ToString(CultureInfo.InvariantCulture),
                    value.ToString(CultureInfo.InvariantCulture)),
                "The function returned by GetTypeEqualityFunction for type int should always return false when inputs are not ints.");
        }

        [TestMethod]
        public void GetTypeEqualityFunction_CustomTypeWithEqualsMethod_ReturnsFunctionWhichCallsEqualsFunction()
        {
            bool returnValue = Random.NextDouble() < 0.5;
            TestTypeWithEquals testObject = new TestTypeWithEquals(returnValue);

            Func<object, object, bool> equalityFunction = typeof (TestTypeWithEquals).GetTypeEqualityFunction();
            Assert.AreEqual(
                returnValue,
                equalityFunction(testObject, new TestTypeWithEquals(!returnValue)),
                "The function returned by GetTypeEqualityFunction should call the most appropriate custom Equals method for a custom type.");
            Assert.IsTrue(
                testObject.EqualsCalled,
                "The function returned by GetTypeEqualityFunction should call the most appropriate custom Equals method for a custom type.");
        }

        [TestMethod]
        public void GetTypeEqualityFunction_CustomTypeWithStaticEqualsMethod_ReturnsFunctionWhichCallsEqualsFunction()
        {
            bool returnValue = Random.NextDouble() < 0.5;
            TestTypeWithStaticEquals testObject = new TestTypeWithStaticEquals(returnValue);

            Func<object, object, bool> equalityFunction = typeof (TestTypeWithStaticEquals).GetTypeEqualityFunction();
            Assert.AreEqual(
                returnValue,
                equalityFunction(testObject, testObject),
                "The function returned by GetTypeEqualityFunction should call the most appropriate custom Equals method for a custom type.");
            Assert.IsTrue(
                TestTypeWithStaticEquals.EqualsCalled,
                "The function returned by GetTypeEqualityFunction should call the most appropriate custom Equals method for a custom type.");
        }

        [TestMethod]
        public void
            GetTypeEqualityFunction_CustomTypeWithStaticEqualsMethodWithTooFewParameters_IgnoresCustomMethodAndUsesObjectEquals
            ()
        {
            TestTypeWithStaticEqualsWithTooFewParameters testObject =
                new TestTypeWithStaticEqualsWithTooFewParameters(false);

            Func<object, object, bool> equalityFunction =
                typeof (TestTypeWithStaticEqualsWithTooFewParameters).GetTypeEqualityFunction();
            Assert.IsTrue(
                equalityFunction(testObject, testObject),
                "The function returned by GetTypeEqualityFunction should fall back on object.Equals when custom methods do no accept the correct parameters.");
            Assert.IsFalse(
                TestTypeWithStaticEqualsWithTooFewParameters.EqualsCalled,
                "The function returned by GetTypeEqualityFunction should fall back on object.Equals when custom methods do no accept the correct parameters.");
        }

        [TestMethod]
        public void
            GetTypeEqualityFunction_CustomTypeWithEqualsMethodWithTooManyParameters_IgnoresCustomMethodAndUsesObjectEquals
            ()
        {
            TestTypeWithEqualsWithTooManyParameters testObject = new TestTypeWithEqualsWithTooManyParameters(false);

            Func<object, object, bool> equalityFunction =
                typeof (TestTypeWithEqualsWithTooManyParameters).GetTypeEqualityFunction();
            Assert.IsTrue(
                equalityFunction(testObject, testObject),
                "The function returned by GetTypeEqualityFunction should fall back on object.Equals when custom methods do no accept the correct parameters.");
            Assert.IsFalse(
                testObject.EqualsCalled,
                "The function returned by GetTypeEqualityFunction should fall back on object.Equals when custom methods do no accept the correct parameters.");
        }

        [TestMethod]
        public void
            GetTypeEqualityFunction_CustomTypeWithStaticEqualsMethodWithWrongParameterType_IgnoresCustomMethodAndUsesObjectEquals
            ()
        {
            TestTypeWithStaticEqualsWithWrongTypes testObject = new TestTypeWithStaticEqualsWithWrongTypes(false);

            Func<object, object, bool> equalityFunction =
                typeof (TestTypeWithStaticEqualsWithWrongTypes).GetTypeEqualityFunction();
            Assert.IsTrue(
                equalityFunction(testObject, testObject),
                "The function returned by GetTypeEqualityFunction should fall back on object.Equals when custom methods do no accept the correct parameters.");
            Assert.IsFalse(
                TestTypeWithStaticEqualsWithWrongTypes.EqualsCalled,
                "The function returned by GetTypeEqualityFunction should fall back on object.Equals when custom methods do no accept the correct parameters.");
        }

        [TestMethod]
        public void
            GetTypeEqualityFunction_CustomTypeWithEqualsMethodWithWrongParameterType_IgnoresCustomMethodAndUsesObjectEquals
            ()
        {
            TestTypeWithEqualsWithWrongType testObject = new TestTypeWithEqualsWithWrongType(false);

            Func<object, object, bool> equalityFunction =
                typeof (TestTypeWithEqualsWithWrongType).GetTypeEqualityFunction();
            Assert.IsTrue(
                equalityFunction(testObject, testObject),
                "The function returned by GetTypeEqualityFunction should fall back on object.Equals when custom methods do no accept the correct parameters.");
            Assert.IsFalse(
                testObject.EqualsCalled,
                "The function returned by GetTypeEqualityFunction should fall back on object.Equals when custom methods do no accept the correct parameters.");
        }
    }
}