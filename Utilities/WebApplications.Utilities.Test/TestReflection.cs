#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities.Test
// File: TestReflection.cs
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
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebApplications.Utilities.Test
{
    [TestClass]
    public class TestReflection
    {
        /// <summary>
        /// Tests the type accessor on a tuple instance.
        /// </summary>
        /// <remarks></remarks>
        [TestMethod]
        public void TestTupleType()
        {
            Tuple<int, string, bool> testTuple = new Tuple<int, string, bool>(1, "Test", true);

            Assert.AreEqual(typeof(int), testTuple.GetIndexType(0));
            Assert.AreEqual(typeof(string), testTuple.GetIndexType(1));
            Assert.AreEqual(typeof(bool), testTuple.GetIndexType(2));
        }

        /// <summary>
        /// Tests the types accessor on a tuple instance.
        /// </summary>
        /// <remarks></remarks>
        [TestMethod]
        public void TestTupleTypes()
        {
            Tuple<int, string, bool> testTuple = new Tuple<int, string, bool>(1, "Test", true);

            Type[] types = testTuple.GetIndexTypes();

            Assert.AreEqual(typeof(int), types[0]);
            Assert.AreEqual(typeof(string), types[1]);
            Assert.AreEqual(typeof(bool), types[2]);
        }

        /// <summary>
        /// Tests the types accessor on a tuple type.
        /// </summary>
        /// <remarks></remarks>
        [TestMethod]
        public void TestTupleTypesStatic()
        {
            Type[] types = typeof(Tuple<int, string, bool>).GetIndexTypes();

            Assert.AreEqual(typeof(int), types[0]);
            Assert.AreEqual(typeof(string), types[1]);
            Assert.AreEqual(typeof(bool), types[2]);
        }

        /// <summary>
        /// Tests the tuple indexer.
        /// </summary>
        /// <remarks></remarks>
        [TestMethod]
        public void TestTupleIndexer()
        {
            Tuple<int, int, int, int, int, int, int, Tuple<int, int, int, int, int, int, int, Tuple<int, int, int>>> t = CreateTuple();
            Func<int, object> indexer = t.GetTupleIndexer();
            for (int i = 0; i < 17; i++)
                Assert.AreEqual(i + 1, indexer(i));
        }

        /// <summary>
        /// Tests retrieving an indexer without an instance of a tuple (by it's type).
        /// </summary>
        /// <remarks></remarks>
        [TestMethod]
        public void TestTupleIndexerStatic()
        {
            Func<object, int, object> indexer =
                typeof (
                    Tuple
                        <int, int, int, int, int, int, int,
                            Tuple<int, int, int, int, int, int, int, Tuple<int, int, int>>>)
                    .GetTupleIndexer();
            
            Tuple<int, int, int, int, int, int, int, Tuple<int, int, int, int, int, int, int, Tuple<int, int, int>>> t = CreateTuple();
            for (int i = 0; i < 17; i++)
                Assert.AreEqual(i + 1, indexer(t, i));
        }

        /// <summary>
        /// Tests the tuple iterator.
        /// </summary>
        /// <remarks></remarks>
        [TestMethod]
        public void TestTupleIterator()
        {
            Tuple<int, int, int, int, int, int, int, Tuple<int, int, int, int, int, int, int, Tuple<int, int, int>>> t = CreateTuple();
            int i = 1;
            foreach (object o in t.GetTupleIterator())
                Assert.AreEqual(i++, (int) o);
        }

        /// <summary>
        /// Tests retrieving an iterator without an instance of a tuple (by it's type).
        /// </summary>
        /// <remarks></remarks>
        [TestMethod]
        public void TestTupleIteratorStatic()
        {
            Func<object, IEnumerable> iterator =
                typeof(
                    Tuple
                        <int, int, int, int, int, int, int,
                            Tuple<int, int, int, int, int, int, int, Tuple<int, int, int>>>)
                    .GetTupleIterator();
            Tuple<int, int, int, int, int, int, int, Tuple<int, int, int, int, int, int, int, Tuple<int, int, int>>> t = CreateTuple();
            int i = 1;
            foreach (object o in iterator(t))
                Assert.AreEqual(i++, (int)o);
        }

        /// <summary>
        /// Creates a tuple for the test with the value equal to the index.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        [NotNull]
        private Tuple<int, int, int, int, int, int, int, Tuple<int, int, int, int, int, int, int, Tuple<int, int, int>>> CreateTuple()
        {
            return new Tuple
                <int, int, int, int, int, int, int, Tuple<int, int, int, int, int, int, int, Tuple<int, int, int>>>(
                1, 2, 3, 4, 5, 6, 7,
                new Tuple<int, int, int, int, int, int, int, Tuple<int, int, int>>(
                    8, 9, 10, 11, 12, 13, 14,
                    new Tuple<int, int, int>(15, 16, 17)));
        }

        [TestMethod]
        public void TestStaticFunction()
        {
            Func<int, int, int> func = typeof (ReflectionTest<Guid>).GetMethod("Add").Func<int, int, int>();
            Assert.AreEqual(func(1, 2), 3);
        }

        [TestMethod]
        public void TestInstanceFunction()
        {
            Func<ReflectionTest<Guid>, int, int, int> func =
                typeof (ReflectionTest<Guid>).GetMethod("Subtract").Func<ReflectionTest<Guid>, int, int, int>();
            Func<ReflectionTest<Guid>, int, int, int> func2 =
                typeof (ReflectionTest<Guid>).GetMethod("Subtract").Func<ReflectionTest<Guid>, int, int, int>();
            ReflectionTest<Guid> rt = new ReflectionTest<Guid>(Guid.NewGuid());
            Assert.AreEqual(func(rt, 1, 2), -1);
        }

        [TestMethod]
        [ExpectedException(typeof (NotImplementedException))]
        public void TestStaticMethod()
        {
            Action<string> method = typeof (ReflectionTest<Guid>).GetMethod("StaticMethod").Action<string>();
            method("test");
        }

        [TestMethod]
        [ExpectedException(typeof (NotImplementedException))]
        public void TestInstanceMethod()
        {
            Action<ReflectionTest<Guid>, string> method =
                typeof (ReflectionTest<Guid>).GetMethod("InstanceMethod").Action<ReflectionTest<Guid>, string>();
            ReflectionTest<Guid> rt = new ReflectionTest<Guid>(Guid.NewGuid());
            method(rt, "test");
        }

        [TestMethod]
        public void TestConstructor()
        {
            Func<Guid, ReflectionTest<Guid>> constructor =
                typeof (ReflectionTest<Guid>).ConstructorFunc<Guid, ReflectionTest<Guid>>();
            Guid guid = Guid.NewGuid();
            ReflectionTest<Guid> rt = constructor(guid);
            Assert.AreEqual(rt.ID, guid);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void TestGetter()
        {
            ReflectionTest<Guid> rt = new ReflectionTest<Guid>(Guid.NewGuid());
            Func<ReflectionTest<Guid>, int> getA = Reflection.GetGetter<ReflectionTest<Guid>, int>("A");
            Assert.AreEqual(getA(rt), 3);

            Func<ReflectionTest<Guid>, int> getB = Reflection.GetGetter<ReflectionTest<Guid>, int>("B");
            Assert.AreEqual(getB(rt), 5);

            Func<ReflectionTest<Guid>, ReflectionTest<Guid>> getLast =
                Reflection.GetGetter<ReflectionTest<Guid>, ReflectionTest<Guid>>("Last");
            Assert.AreEqual(getLast(rt), rt);

            // Should throw exception - no getter for C
            Func<ReflectionTest<Guid>, int> getC = Reflection.GetGetter<ReflectionTest<Guid>, int>("C");
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void TestSetter()
        {
            ReflectionTest<Guid> rt = new ReflectionTest<Guid>(Guid.NewGuid());
            Func<ReflectionTest<Guid>, int, int> setA = Reflection.GetSetter<ReflectionTest<Guid>, int>("A");
            setA(rt, 5);
            Assert.AreEqual(rt.A, 5);

            Func<ReflectionTest<Guid>, int, int> setB = Reflection.GetSetter<ReflectionTest<Guid>, int>("B");
            Assert.AreEqual(setB(rt, 6), 6);

            Func<ReflectionTest<Guid>, int, int> setC = Reflection.GetSetter<ReflectionTest<Guid>, int>("C");
            setC(rt, 10);
            Assert.AreEqual(rt.SetC, 10);

            Func<ReflectionTest<Guid>, DateTime, DateTime> setDateTime =
                Reflection.GetSetter<ReflectionTest<Guid>, DateTime>("DateTime");
            setDateTime(null, DateTime.Now);

            // Should throw exception - no getter for C
            Func<ReflectionTest<Guid>, ReflectionTest<Guid>, ReflectionTest<Guid>> setLast =
                Reflection.GetSetter<ReflectionTest<Guid>, ReflectionTest<Guid>>("Last");
        }

        [TestMethod]
        public void TestRawDefaultValueSafe()
        {
            MethodInfo methodInfo = GetType().GetMethod("TestMethod");
            foreach (ParameterInfo pi in methodInfo.GetParameters())
            {
                Trace.WriteLine(string.Format(
                    "{1} {0} = {2}",
                    pi.Name, pi.ParameterType, pi.RawDefaultValueSafe()));
            }
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
            Assert.AreEqual((long)a, func2(a));

            // Explicit cast support
            long b = r.Next();
            Func<long, int> func3 = Reflection.GetConversion<Int64, int>();
            Assert.AreEqual((int)b, func3(b));

            // Boxing on the input and output, but with cast from int -> string (which will use IConvertible interface ToString(IFormatProvider))
            a = r.Next();
            Func<object, object> func4 = typeof (int).GetConversion(typeof (string));
            Assert.AreEqual(a.ToString(CultureInfo.InvariantCulture), (string)func4(a));

            a = r.Next();
            Func<int, TypeConverterTest> func5 = Reflection.GetConversion<int, TypeConverterTest>();
            TypeConverterTest t = func5(a);
            Assert.AreEqual(a, t.Value);
;
            Func<TypeConverterTest, int> func6 = Reflection.GetConversion<TypeConverterTest, int>();
            Assert.AreEqual(a, func6(t));

            int z =func6(t);
            z = (int)new TestConverter().ConvertTo(t, typeof (int));

            a = r.Next();
            Func<TypeConverterTest, string> func7 = Reflection.GetConversion<TypeConverterTest, string>();
            t = new TypeConverterTest(a);
            Assert.AreEqual("TEST" + a, func7(t));

            byte c = (byte)r.Next(Byte.MinValue, Byte.MaxValue);
            Func<ConvertibleTest, int> func8 = Reflection.GetConversion<ConvertibleTest, int>();
            Assert.AreEqual(c, func8(new ConvertibleTest(c)));
            
            Assert.IsNull(Reflection.GetConversion<string, int>());
        }

        public void TestMethod(int noDefault, int a = 1, int b = default(int), DateTime c = default(DateTime),
                               TimeSpan d = default(TimeSpan))
        {
        }

        /// <summary>
        /// Test class that has an associated type converter.
        /// </summary>
        /// <remarks></remarks>
        [TypeConverter(typeof(TestConverter))]
        public class TypeConverterTest
        {
            public readonly int Value;

            public TypeConverterTest(int value = 0)
            {
                Value = value;
            }

            /// <summary>
            /// Returns a <see cref="System.String"/> that represents this instance.
            /// </summary>
            /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
            /// <remarks></remarks>
            public override string ToString()
            {
                return "TEST" + Value.ToString(CultureInfo.InvariantCulture);
            }
        }

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
            /// <remarks></remarks>
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                return sourceType == typeof(int) || base.CanConvertFrom(context, sourceType);
            }

            /// <summary>
            /// Returns whether this converter can convert the object to the specified type, using the specified context.
            /// </summary>
            /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
            /// <param name="destinationType">A <see cref="T:System.Type"/> that represents the type you want to convert to.</param>
            /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
            /// <remarks></remarks>
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                return destinationType == typeof(int) || base.CanConvertTo(context, destinationType);
            }

            /// <summary>
            /// Converts the given object to the type of this converter, using the specified context and culture information.
            /// </summary>
            /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
            /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"/> to use as the current culture.</param>
            /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
            /// <returns>An <see cref="T:System.Object"/> that represents the converted value.</returns>
            /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
            /// <remarks></remarks>
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
            ///   
            /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
            /// <remarks></remarks>
            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType == typeof(int))
                    return value != null ? ((TypeConverterTest) value).Value : 0;
                return base.ConvertTo(context, culture, value, destinationType);
            }

        }

        public struct ConvertibleTest : IConvertible
        {
            public readonly byte Value;

            public ConvertibleTest(byte value)
            {
                Value = value;
            }

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
                return (sbyte)Value;
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
        }

        #region Nested type: ReflectionTest
        public class ReflectionTest<T>
        {
            private static ReflectionTest<T> _last;
            public readonly T ID;

            public int A;
            public int SetC;

            public ReflectionTest(T id)
            {
                ID = id;
                A = 3;
                B = 5;
                _last = this;
            }

            public int B { private get; set; }

            public static ReflectionTest<T> Last
            {
                get { return _last; }
            }

            public int C
            {
                set { SetC = value; }
            }

            public static DateTime DateTime { get; private set; }

            public static int Add(int a, int b)
            {
                return a + b;
            }

            public int Subtract(int a, int b)
            {
                return a - b;
            }

            public static void StaticMethod(string id)
            {
                throw new NotImplementedException("Static Method");
            }

            public void InstanceMethod(string id)
            {
                throw new NotImplementedException("Instance Method");
            }
        }
        #endregion
    }
}