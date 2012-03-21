#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities.Test
// File: EqualityFunctionsTests.cs
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
using System.Globalization;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebApplications.Testing;
using WebApplications.Utilities;

namespace WebApplications.Utilities.Test.Extensions
{
    [TestClass]
    public class EqualityFunctionsTests : TestBase
    {

        #region Test Classes

        private class TestTypeWithEquals
        {
            public bool EqualsCalled { get; private set; }
            public bool ReturnValue { get; private set; }

            public TestTypeWithEquals(bool returnValue)
            {
                EqualsCalled = false;
                ReturnValue = returnValue;
            }

            public bool Equals(TestTypeWithEquals other)
            {
                EqualsCalled = true;
                return ReturnValue;
            }
        }

        private class TestTypeWithEqualsWithWrongType
        {
            public bool EqualsCalled { get; private set; }
            public bool ReturnValue { get; private set; }

            public TestTypeWithEqualsWithWrongType(bool returnValue)
            {
                EqualsCalled = false;
                ReturnValue = returnValue;
            }

            public bool Equals(TestTypeWithEquals other)
            {
                EqualsCalled = true;
                return ReturnValue;
            }
        }

        private class TestTypeWithEqualsWithTooManyParameters
        {
            public bool EqualsCalled { get; private set; }
            public bool ReturnValue { get; private set; }

            public TestTypeWithEqualsWithTooManyParameters(bool returnValue)
            {
                EqualsCalled = false;
                ReturnValue = returnValue;
            }

            public bool Equals(TestTypeWithEqualsWithTooManyParameters other, TestTypeWithEqualsWithTooManyParameters extra)
            {
                EqualsCalled = true;
                return ReturnValue;
            }
        }

        private class TestTypeWithStaticEquals
        {
            static public bool EqualsCalled { get; private set; }
            static public bool ReturnValue { get; set; }

            static TestTypeWithStaticEquals()
            {
                EqualsCalled = false;
            }

            public TestTypeWithStaticEquals( bool? returnValue = null )
            {
                if( returnValue.HasValue )
                    ReturnValue = returnValue.Value;
            }

            static public bool Equals(TestTypeWithStaticEquals first, TestTypeWithStaticEquals second)
            {
                EqualsCalled = true;
                return ReturnValue;
            }
        }

        private class TestTypeWithStaticEqualsWithTooFewParameters
        {
            static public bool EqualsCalled { get; private set; }
            static public bool ReturnValue { get; set; }

            static TestTypeWithStaticEqualsWithTooFewParameters()
            {
                EqualsCalled = false;
            }

            public TestTypeWithStaticEqualsWithTooFewParameters(bool? returnValue = null)
            {
                if( returnValue.HasValue )
                    ReturnValue = returnValue.Value;
            }

            static public bool Equals(TestTypeWithStaticEqualsWithTooFewParameters other)
            {
                EqualsCalled = true;
                return ReturnValue;
            }
        }

        private class TestTypeWithStaticEqualsWithWrongTypes
        {
            static public bool EqualsCalled { get; private set; }
            static public bool ReturnValue { get; set; }

            static TestTypeWithStaticEqualsWithWrongTypes()
            {
                EqualsCalled = false;
            }

            public TestTypeWithStaticEqualsWithWrongTypes( bool? returnValue = null )
            {
                if( returnValue.HasValue )
                    ReturnValue = returnValue.Value;
            }

            static public bool Equals(TestTypeWithStaticEquals first, TestTypeWithStaticEquals second)
            {
                EqualsCalled = true;
                return ReturnValue;
            }
        }

        #endregion

        [TestMethod]
        public void GetTypeEqualityFunction_Integers_ReturnsFunctionWhichReturnsTrueWhenBothInputsAreEqual()
        {
            Func<object, object, bool> equalityFunction = typeof(int).GetTypeEqualityFunction();
            int value = Random.Next();
            Assert.IsTrue(equalityFunction(value, value), "The function returned by GetTypeEqualityFunction for type int should return true when both inputs are equal.");
        }

        [TestMethod]
        public void GetTypeEqualityFunction_Integers_ReturnsFunctionWhichReturnsFalseWhenBothInputsAreDifferent()
        {
            Func<object, object, bool> equalityFunction = typeof(int).GetTypeEqualityFunction();
            int value = Random.Next();
            Assert.IsFalse(equalityFunction(value, value-Random.Next()), "The function returned by GetTypeEqualityFunction for type int should return false when both inputs are different.");
        }

        [ExpectedException(typeof(InvalidCastException))]
        [TestMethod]
        public void GetTypeEqualityFunction_IntegersButInputsToResultAreWrongType_ThrowsInvalidCastException()
        {
            Func<object, object, bool> equalityFunction = typeof(int).GetTypeEqualityFunction();
            int value = Random.Next();
            Assert.IsFalse(equalityFunction(value.ToString(CultureInfo.InvariantCulture), value.ToString(CultureInfo.InvariantCulture)), "The function returned by GetTypeEqualityFunction for type int should always return false when inputs are not ints.");
        }

        [TestMethod]
        public void GetTypeEqualityFunction_CustomTypeWithEqualsMethod_ReturnsFunctionWhichCallsEqualsFunction()
        {
            bool returnValue = Random.NextDouble() < 0.5;
            TestTypeWithEquals testObject = new TestTypeWithEquals(returnValue);

            Func<object, object, bool> equalityFunction = typeof(TestTypeWithEquals).GetTypeEqualityFunction();
            Assert.AreEqual(returnValue, equalityFunction(testObject, new TestTypeWithEquals(!returnValue)), "The function returned by GetTypeEqualityFunction should call the most appropriate custom Equals method for a custom type.");
            Assert.IsTrue(testObject.EqualsCalled, "The function returned by GetTypeEqualityFunction should call the most appropriate custom Equals method for a custom type.");
        }

        [TestMethod]
        public void GetTypeEqualityFunction_CustomTypeWithStaticEqualsMethod_ReturnsFunctionWhichCallsEqualsFunction()
        {
            bool returnValue = Random.NextDouble() < 0.5;
            TestTypeWithStaticEquals testObject = new TestTypeWithStaticEquals(returnValue);

            Func<object, object, bool> equalityFunction = typeof(TestTypeWithStaticEquals).GetTypeEqualityFunction();
            Assert.AreEqual(returnValue, equalityFunction(testObject, testObject), "The function returned by GetTypeEqualityFunction should call the most appropriate custom Equals method for a custom type.");
            Assert.IsTrue(TestTypeWithStaticEquals.EqualsCalled, "The function returned by GetTypeEqualityFunction should call the most appropriate custom Equals method for a custom type.");
        }

        [TestMethod]
        public void GetTypeEqualityFunction_CustomTypeWithStaticEqualsMethodWithTooFewParameters_IgnoresCustomMethodAndUsesObjectEquals()
        {
            TestTypeWithStaticEqualsWithTooFewParameters testObject = new TestTypeWithStaticEqualsWithTooFewParameters(false);

            Func<object, object, bool> equalityFunction = typeof(TestTypeWithStaticEqualsWithTooFewParameters).GetTypeEqualityFunction();
            Assert.IsTrue(equalityFunction(testObject, testObject), "The function returned by GetTypeEqualityFunction should fall back on object.Equals when custom methods do no accept the correct parameters.");
            Assert.IsFalse(TestTypeWithStaticEqualsWithTooFewParameters.EqualsCalled, "The function returned by GetTypeEqualityFunction should fall back on object.Equals when custom methods do no accept the correct parameters.");
        }

        [TestMethod]
        public void GetTypeEqualityFunction_CustomTypeWithEqualsMethodWithTooManyParameters_IgnoresCustomMethodAndUsesObjectEquals()
        {
            TestTypeWithEqualsWithTooManyParameters testObject = new TestTypeWithEqualsWithTooManyParameters(false);

            Func<object, object, bool> equalityFunction = typeof(TestTypeWithEqualsWithTooManyParameters).GetTypeEqualityFunction();
            Assert.IsTrue(equalityFunction(testObject, testObject), "The function returned by GetTypeEqualityFunction should fall back on object.Equals when custom methods do no accept the correct parameters.");
            Assert.IsFalse(testObject.EqualsCalled, "The function returned by GetTypeEqualityFunction should fall back on object.Equals when custom methods do no accept the correct parameters.");
        }

        [TestMethod]
        public void GetTypeEqualityFunction_CustomTypeWithStaticEqualsMethodWithWrongParameterType_IgnoresCustomMethodAndUsesObjectEquals()
        {
            TestTypeWithStaticEqualsWithWrongTypes testObject = new TestTypeWithStaticEqualsWithWrongTypes(false);

            Func<object, object, bool> equalityFunction = typeof(TestTypeWithStaticEqualsWithWrongTypes).GetTypeEqualityFunction();
            Assert.IsTrue(equalityFunction(testObject, testObject), "The function returned by GetTypeEqualityFunction should fall back on object.Equals when custom methods do no accept the correct parameters.");
            Assert.IsFalse(TestTypeWithStaticEqualsWithWrongTypes.EqualsCalled, "The function returned by GetTypeEqualityFunction should fall back on object.Equals when custom methods do no accept the correct parameters.");
        }

        [TestMethod]
        public void GetTypeEqualityFunction_CustomTypeWithEqualsMethodWithWrongParameterType_IgnoresCustomMethodAndUsesObjectEquals()
        {
            TestTypeWithEqualsWithWrongType testObject = new TestTypeWithEqualsWithWrongType(false);

            Func<object, object, bool> equalityFunction = typeof(TestTypeWithEqualsWithWrongType).GetTypeEqualityFunction();
            Assert.IsTrue(equalityFunction(testObject, testObject), "The function returned by GetTypeEqualityFunction should fall back on object.Equals when custom methods do no accept the correct parameters.");
            Assert.IsFalse(testObject.EqualsCalled, "The function returned by GetTypeEqualityFunction should fall back on object.Equals when custom methods do no accept the correct parameters.");
        }
    }
}
