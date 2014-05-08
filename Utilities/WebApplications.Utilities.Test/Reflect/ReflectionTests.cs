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
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;
using WebApplications.Utilities.Reflect;

namespace WebApplications.Utilities.Test.Reflect
{
    [TestClass]
    public class ReflectionTests : UtilitiesTestBase
    {
        /// <summary>
        /// Tests reflection of events.
        /// </summary>
        [UsedImplicitly]
        public event EventHandler TestEvent;

        [UsedImplicitly]
        public void OnTestEvent(EventArgs e)
        {
            EventHandler handler = TestEvent;
            if (handler != null) handler(this, e);
        }

        /// <summary>
        /// Tests the reflector constructor.
        /// </summary>
        [TestMethod]
        public void Test_ReflectorStaticConstructor()
        {
            int aCount = 0;
            int tCount = 0;
            int aeCount = 0;
            int tlCount = 0;
            int nsCount = 0;
            int rCount = 0;
            int mCount = 0;
            Stopwatch s = new Stopwatch();
            s.Start();
            // Get every loaded assembly - this will include the framework, so lot's of types to test with!
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                aCount++;
                foreach (Type t in a.GetTypes().Where(t => !t.IsGenericTypeDefinition))
                {
                    Assert.IsNotNull(t);

                    // Make generic type
                    try
                    {
                        ExtendedType et = ExtendedType.Get(t);
                        Assert.IsNotNull(et);
                        Assert.IsNotNull(et.Methods);
                        mCount += et.Methods.Count();
                        rCount++;
                    }
                    catch (NotSupportedException)
                    {
                        nsCount++;
                    }
                    catch (ArgumentException)
                    {
                        // Not all types can be a generic argument (e.g. System.Void)
                        aeCount++;
                    }
                    catch (TypeLoadException)
                    {
                        // Not all types can be loaded
                        tlCount++;
                    }
                }
            }
            s.Stop();
            Trace.WriteLine(
                s.ToString(
                    "Reflecting '{0}' of '{1}' types in '{2}' assemblies [{3} type load errors, {4} argument exceptions, {5} not supported] - {6} methods found.",
                    rCount,
                    tCount,
                    aCount,
                    tlCount,
                    aeCount,
                    nsCount,
                    mCount));
        }

        [TestMethod]
        public void GetMethod_InstanceFunction_ReturnsLambdaForRequestedFunction()
        {
            int a = Random.Next(0, 10);
            int b = Random.Next(0, 10);
            ReflectionTestClass<String> testInstance = new ReflectionTestClass<String>("Test");
            // Set the expected output for the dummy
            testInstance.InstanceFunctionOutput = a + b;
            Func<ReflectionTestClass<String>, int, int, int> func =
                typeof (ReflectionTestClass<String>).GetMethod("InstanceFunction").Func
                    <ReflectionTestClass<String>, int, int, int>();
            Assert.IsNotNull(func, "The lambda returned must not be null.");
            // Note that there is no simple way to test if we have the correct function, so we instead test that parameters are received correctly
            Assert.AreEqual(
                a + b,
                func(testInstance, a, b),
                "When called, the lambda returned by GetMethod should return the value the requested function returns.");
            Assert.AreEqual(
                a,
                testInstance.InstanceFunctionInputA,
                "When called with parameters, the lambda calls the requested function using these parameters.");
            Assert.AreEqual(
                b,
                testInstance.InstanceFunctionInputB,
                "When called with parameters, the lambda calls the requested function using these parameters.");
        }

        [TestMethod]
        public void GetMethod_StaticMethod_ReturnsLambdaForRequestedMethod()
        {
            string value = Random.Next().ToString();
            ReflectionTestClass<Guid>.StaticMethodInput = null;
            Action<string> method = typeof (ReflectionTestClass<Guid>).GetMethod("StaticMethod").Action<string>();
            Assert.IsNotNull(method, "The lambda returned must not be null.");
            // Note that there is no simple way to test if we have the correct function, so we instead test that parameters are received correctly
            method(value);
            Assert.AreEqual(
                value,
                ReflectionTestClass<Guid>.StaticMethodInput,
                "When called with parameters, the lambda calls the requested method using these parameters.");
        }

        [TestMethod]
        public void GetMethod_InstanceMethod_ReturnsLambdaForRequestedMethod()
        {
            string value = Random.Next().ToString();
            ReflectionTestClass<Guid> testInstance = new ReflectionTestClass<Guid>(Guid.NewGuid());
            Action<ReflectionTestClass<Guid>, string> method =
                typeof (ReflectionTestClass<Guid>).GetMethod("InstanceMethod").Action<ReflectionTestClass<Guid>, string>
                    ();
            Assert.IsNotNull(method, "The lambda returned must not be null.");
            // Note that there is no simple way to test if we have the correct function, so we instead test that parameters are received correctly
            method(testInstance, value);
            Assert.AreEqual(
                value,
                testInstance.InstanceMethodInput,
                "When called with parameters, the lambda calls the requested method using these parameters.");
        }

        [TestMethod]
        public void GetMethod_Constructor_ReturnsLambdaForRequestedConstructor()
        {
            Func<Guid, ReflectionTestClass<Guid>> constructor =
                typeof (ReflectionTestClass<Guid>).ConstructorFunc<Guid, ReflectionTestClass<Guid>>();
            Guid guid = Guid.NewGuid();
            ReflectionTestClass<Guid> testInstance = constructor(guid);
            Assert.AreEqual(guid, testInstance.ID);
        }

        [TestMethod]
        public void GetGetter_Field_ReturnsLambdaGetterForRequestedField()
        {
            int value = Random.Next();
            ReflectionTestClass<Guid> testInstance = new ReflectionTestClass<Guid>(Guid.NewGuid());
            Func<ReflectionTestClass<Guid>, int> getA =
                typeof (ReflectionTestClass<Guid>).GetGetter<ReflectionTestClass<Guid>, int>("A");
            testInstance.A = value;
            Assert.AreEqual(
                testInstance.A,
                getA(testInstance),
                "The lambda function returned by GetGetter should return the value of the field given as the parameter.");
        }

        [TestMethod]
        public void GetGetter_PropertyWithPrivateGetter_ReturnsLambdaGetterForRequestedProperty()
        {
            int value = Random.Next();
            ReflectionTestClass<Guid> testInstance = new ReflectionTestClass<Guid>(Guid.NewGuid());
            Func<ReflectionTestClass<Guid>, int> getB =
                typeof (ReflectionTestClass<Guid>).GetGetter<ReflectionTestClass<Guid>, int>("B");
            testInstance.B = value;
            Assert.AreEqual(
                value,
                getB(testInstance),
                "The lambda function returned by GetGetter should return the value of the requested property.");
        }

        [TestMethod]
        public void GetGetter_PropertyWithExplicitGetter_ReturnsLambdaGetterForRequestedProperty()
        {
            ReflectionTestClass<Guid> testInstance = new ReflectionTestClass<Guid>(Guid.NewGuid());
            Func<ReflectionTestClass<Guid>> getLast =
                typeof (ReflectionTestClass<Guid>).GetGetter<ReflectionTestClass<Guid>>("Last");
            Assert.AreEqual(
                testInstance,
                getLast(),
                "The lambda function returned by GetGetter should return the value of the requested property.");
        }

        [TestMethod]
        public void GetSetter_Field_ReturnsLambdaForRequestedField()
        {
            int value = Random.Next();
            ReflectionTestClass<Guid> testInstance = new ReflectionTestClass<Guid>(Guid.NewGuid());

            Action<ReflectionTestClass<Guid>, int> setA =
                typeof (ReflectionTestClass<Guid>).GetSetter<ReflectionTestClass<Guid>, int>("A");
            setA(testInstance, value);
            Assert.AreEqual(
                value,
                testInstance.A,
                "The lambda function returned by GetSetter should change the value of the specified field.");
        }

        [TestMethod]
        public void GetSetter_PropertyWithPrivateGetter_ReturnsLambdaWhichReturnsNewValueOfSpecifiedProperty()
        {
            int value = Random.Next();
            ReflectionTestClass<Guid> testInstance = new ReflectionTestClass<Guid>(Guid.NewGuid());

            Action<ReflectionTestClass<Guid>, int> setB =
                typeof (ReflectionTestClass<Guid>).GetSetter<ReflectionTestClass<Guid>, int>("B");
            setB(testInstance, value);
        }

        [TestMethod]
        public void GetSetter_PropertyWithExplicitSetter_ReturnsLambdaForRequestedProperty()
        {
            int value = Random.Next();
            ReflectionTestClass<Guid> testInstance = new ReflectionTestClass<Guid>(Guid.NewGuid());

            Action<ReflectionTestClass<Guid>, int> setC =
                typeof (ReflectionTestClass<Guid>).GetSetter<ReflectionTestClass<Guid>, int>("C");
            setC(testInstance, value);
            Assert.AreEqual(testInstance.SetC, value);
        }

        [TestMethod]
        public void GetSetter_PropertyWithPrivateSetter_ReturnsLambdaForRequestedProperty()
        {
            DateTime value = DateTime.Now;
            ReflectionTestClass<Guid> testInstance = new ReflectionTestClass<Guid>(Guid.NewGuid());

            Action<DateTime> setDateTime =
                typeof (ReflectionTestClass<Guid>).GetSetter<DateTime>("DateTime");
            setDateTime(value);
            Assert.AreEqual(value, ReflectionTestClass<Guid>.DateTime);
        }

        [TestMethod]
        [ExpectedException(typeof (NullReferenceException))]
        public void GetSetter_PassingNullToSetterLambda_ThrowsNullReference()
        {
            Action<ReflectionTestClass<Guid>, DateTime> setDateTime =
                typeof (ReflectionTestClass<Guid>).GetSetter<ReflectionTestClass<Guid>, DateTime>("DateTime");
            setDateTime(null, DateTime.Now);
        }

        [TestMethod]
        public void GetSetter_PropertyWithNoSetter_ReturnsNull()
        {
            ReflectionTestClass<Guid> testInstance = new ReflectionTestClass<Guid>(Guid.NewGuid());

            Action<ReflectionTestClass<Guid>> setLast =
                typeof (ReflectionTestClass<Guid>).GetSetter<ReflectionTestClass<Guid>>("Last");

            Assert.IsNull(setLast);
        }

        [TestMethod]
        public void GetSetter_ConstantField_ReturnsNull()
        {
            Action<int> setConst = typeof(ReflectionTestClass<Guid>).GetSetter<int>("Constant");
            Assert.IsNull(setConst);
        }

        [TestMethod]
        public void TestConversions()
        {
            Random r = new Random();

            // Easiest case - no casts necessary
            int a = r.Next();
            Func<int, int> func1 = Reflection.GetConversion<int, int>();
            Assert.AreEqual(a, func1(a));

            // Implicit casts support
            a = r.Next();
            Func<int, long> func2 = Reflection.GetConversion<int, Int64>();
            Assert.AreEqual(a, func2(a));

            // Explicit cast support
            long b = r.Next();
            Func<long, int> func3 = Reflection.GetConversion<Int64, int>();
            Assert.AreEqual((int) b, func3(b));

            // Boxing on the input and output, but with cast from int -> string (which will use IConvertible interface ToString(IFormatProvider))
            a = r.Next();
            Func<object, object> func4 = typeof (int).GetConversion(typeof (string));
            Assert.AreEqual(a.ToString(CultureInfo.InvariantCulture), (string) func4(a));

            a = r.Next();
            Func<int, TypeConverterTest> func5 = Reflection.GetConversion<int, TypeConverterTest>();
            TypeConverterTest t = func5(a);
            Assert.AreEqual(a, t.Value);

            Func<TypeConverterTest, int> func6 = Reflection.GetConversion<TypeConverterTest, int>();
            Assert.AreEqual(a, func6(t));

            int z = func6(t);
            z = (int) new TestConverter().ConvertTo(t, typeof (int));

            a = r.Next();
            Func<TypeConverterTest, string> func7 = Reflection.GetConversion<TypeConverterTest, string>();
            t = new TypeConverterTest(a);
            Assert.AreEqual("TEST" + a, func7(t));

            byte c = (byte) r.Next(Byte.MinValue, Byte.MaxValue);
            Func<ConvertibleTest, int> func8 = Reflection.GetConversion<ConvertibleTest, int>();
            Assert.AreEqual(c, func8(new ConvertibleTest(c)));

            //Assert.IsNull(Reflection.GetConversion<string, int>());
        }

        #region Nested type: ConvertibleTest
        public struct ConvertibleTest : IConvertible
        {
            public readonly byte Value;

            public ConvertibleTest(byte value)
            {
                Value = value;
            }

            #region IConvertible Members
            public TypeCode GetTypeCode()
            {
                return TypeCode.Byte;
            }

            public bool ToBoolean(IFormatProvider provider)
            {
                return Value > 0;
            }

            public char ToChar(IFormatProvider provider)
            {
                return (Char) Value;
            }

            public sbyte ToSByte(IFormatProvider provider)
            {
                return (sbyte) Value;
            }

            public byte ToByte(IFormatProvider provider)
            {
                return Value;
            }

            public short ToInt16(IFormatProvider provider)
            {
                return Value;
            }

            public ushort ToUInt16(IFormatProvider provider)
            {
                return Value;
            }

            public int ToInt32(IFormatProvider provider)
            {
                return Value;
            }

            public uint ToUInt32(IFormatProvider provider)
            {
                return Value;
            }

            public long ToInt64(IFormatProvider provider)
            {
                return Value;
            }

            public ulong ToUInt64(IFormatProvider provider)
            {
                return Value;
            }

            public float ToSingle(IFormatProvider provider)
            {
                return Value;
            }

            public double ToDouble(IFormatProvider provider)
            {
                return Value;
            }

            public decimal ToDecimal(IFormatProvider provider)
            {
                return Value;
            }

            public DateTime ToDateTime(IFormatProvider provider)
            {
                return DateTime.Now;
            }

            public string ToString(IFormatProvider provider)
            {
                return Value.ToString(provider);
            }

            public object ToType(Type conversionType, IFormatProvider provider)
            {
                return null;
            }
            #endregion
        }
        #endregion

        #region Nested type: ReflectionTestClass
        public class ReflectionTestClass<T>
        {
            public const int Constant = 1;

            private static ReflectionTestClass<T> _last;
            public static int? StaticFunctionInputA = null;
            public static int? StaticFunctionInputB = null;
            public static int? StaticFunctionOutput = null;
            public static string StaticMethodInput = null;
            public readonly T ID;

            public int A;
            public int? InstanceFunctionInputA = null;
            public int? InstanceFunctionInputB = null;
            public int? InstanceFunctionOutput = null;
            public string InstanceMethodInput = null;
            public int SetC;

            public ReflectionTestClass(T id)
            {
                ID = id;
                _last = this;
            }

            public ReflectionTestClass()
            {
                _last = this;
            }

            public int B { private get; set; }

            public static ReflectionTestClass<T> Last
            {
                get { return _last; }
            }

            public int C
            {
                set { SetC = value; }
            }

            public static DateTime DateTime { get; private set; }


            public static int StaticFunction(int a, int b)
            {
                StaticFunctionInputA = a;
                StaticFunctionInputB = b;
                Assert.IsNotNull(
                    StaticFunctionOutput,
                    "The StaticFunctionOutput field must be set in order to test the output of the static function");
                return StaticFunctionOutput.Value;
            }

            public int InstanceFunction(int a, int b)
            {
                InstanceFunctionInputA = a;
                InstanceFunctionInputB = b;
                Assert.IsNotNull(
                    InstanceFunctionOutput,
                    "The InstanceFunctionOutput field must be set in order to test the output of the instance function");
                return InstanceFunctionOutput.Value;
            }

            public static void StaticMethod(string id)
            {
                StaticMethodInput = id;
            }

            public void InstanceMethod(string id)
            {
                // Identified by changing an instance field's value
                InstanceMethodInput = id;
            }
        }
        #endregion

        #region Nested type: TestConverter
        /// <summary>
        /// Example of a converter for the TypeConverterTest class.
        /// </summary>
        /// <remarks>
        /// This is a simple example, support conversion between int and string.
        /// </remarks>
        public class TestConverter : TypeConverter
        {
            /// <summary>
            /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
            /// </summary>
            /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
            /// <param name="sourceType">A <see cref="T:System.Type"/> that represents the type you want to convert from.</param>
            /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                return sourceType == typeof (int) || base.CanConvertFrom(context, sourceType);
            }

            /// <summary>
            /// Returns whether this converter can convert the object to the specified type, using the specified context.
            /// </summary>
            /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
            /// <param name="destinationType">A <see cref="T:System.Type"/> that represents the type you want to convert to.</param>
            /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                return destinationType == typeof (int) || base.CanConvertTo(context, destinationType);
            }

            /// <summary>
            /// Converts the given object to the type of this converter, using the specified context and culture information.
            /// </summary>
            /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
            /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"/> to use as the current culture.</param>
            /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
            /// <returns>An <see cref="T:System.Object"/> that represents the converted value.</returns>
            /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                return value is int ? new TypeConverterTest((int) value) : base.ConvertFrom(context, culture, value);
            }

            /// <summary>
            /// Converts the given value object to the specified type, using the specified context and culture information.
            /// </summary>
            /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
            /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"/>. If null is passed, the current culture is assumed.</param>
            /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
            /// <param name="destinationType">The <see cref="T:System.Type"/> to convert the <paramref name="value"/> parameter to.</param>
            /// <returns>An <see cref="T:System.Object"/> that represents the converted value.</returns>
            /// <exception cref="T:System.ArgumentNullException">The <paramref name="destinationType"/> parameter is null. </exception>
            /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
            public override object ConvertTo(
                ITypeDescriptorContext context,
                CultureInfo culture,
                object value,
                Type destinationType)
            {
                if (destinationType == typeof (int))
                    return value != null ? ((TypeConverterTest) value).Value : 0;
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
        #endregion

        #region Nested type: TypeConverterTest
        /// <summary>
        /// Test class that has an associated type converter.
        /// </summary>
        [TypeConverter(typeof (TestConverter))]
        public class TypeConverterTest
        {
            public readonly int Value;

            public TypeConverterTest(int value = 0)
            {
                Value = value;
            }

            public override string ToString()
            {
                return "TEST" + Value.ToString(CultureInfo.InvariantCulture);
            }
        }
        #endregion

        #region Nested type: ArithmeticFuncTests
        [TestMethod]
        public void TestAddFunc()
        {
            Func<int, int, int> func = typeof (int).AddFunc<int>();
            Assert.IsTrue(func(6, 6) == 12);
            Assert.IsTrue(func(12, 6) == 18);
        }

        [TestMethod]
        public void TestSubstractFunc()
        {
            Func<int, int, int> func = typeof (int).SubtractFunc<int>();
            Assert.IsTrue(func(6, 6) == 0);
            Assert.IsTrue(func(12, 6) == 6);
            Assert.IsTrue(func(6, 12) == -6);
        }

        [TestMethod]
        public void TestLessThanFunc()
        {
            Func<string, string, bool> func = typeof (int).LessThanFunc<string>();
            Assert.IsTrue(func("5", "6"));
            Assert.IsFalse(func("7", "6"));
            Assert.IsFalse(func("7", "7"));
        }

        [TestMethod]
        public void TestLessThanOrEqualToFunc()
        {
            Func<string, string, bool> func = typeof (int).LessThanOrEqualFunc<string>();
            Assert.IsTrue(func("5", "6"));
            Assert.IsFalse(func("7", "6"));
            Assert.IsTrue(func("7", "7"));
        }

        [TestMethod]
        public void TestGreaterThanFunc()
        {
            Func<string, string, bool> func = typeof (int).GreaterThanFunc<string>();
            Assert.IsTrue(func("6", "5"));
            Assert.IsFalse(func("6", "7"));
            Assert.IsFalse(func("7", "7"));
        }

        [TestMethod]
        public void TestGreaterThanOrEqualToFunc()
        {
            Func<string, string, bool> func = typeof (int).GreaterThanOrEqualFunc<string>();
            Assert.IsTrue(func("6", "5"));
            Assert.IsFalse(func("6", "7"));
            Assert.IsTrue(func("7", "7"));
        }

        [TestMethod]
        public void TestAndAlsoFunc()
        {
            Func<bool, bool, bool> func = typeof (bool).AndAlsoFunc<bool>();
            Assert.IsTrue(func(true, true));
            Assert.IsFalse(func(true, false));
            Assert.IsFalse(func(false, true));
            Assert.IsFalse(func(false, false));
        }

        [TestMethod]
        public void TestOrElseFunc()
        {
            Func<string, string, bool> func = typeof (bool).OrElseFunc<string>();
            Assert.IsTrue(func("True", "True"));
            Assert.IsTrue(func("True", "False"));
            Assert.IsTrue(func("False", "True"));
            Assert.IsFalse(func("False", "False"));
        }
        #endregion
    }
}