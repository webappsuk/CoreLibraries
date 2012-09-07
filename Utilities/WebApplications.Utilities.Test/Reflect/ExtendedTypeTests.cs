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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;
using WebApplications.Utilities.Reflect;

namespace WebApplications.Utilities.Test.Reflect
{
    [TestClass]
    public class ExtendedTypeTests : UtilitiesTestBase
    {
        [TestMethod]
        public void ExtendedType_CanDisambiguateMembers()
        {
            ExtendedType et = ExtendedType.Get(typeof (ComplexOverloads<>));

            IEnumerable<Method> methods = et.GetMethods("A");

            Assert.IsNotNull(methods);
            Assert.AreEqual(9, methods.Count());

            Method invalidMethod = et.GetMethod("A");
            Assert.IsNull(invalidMethod);

            Method method1 = et.GetMethod("A", TypeSearch.T1);
            Assert.IsNotNull(method1);
            Assert.IsTrue(method1.Info.ContainsGenericParameters);

            // Demonstrate cast to void
            bool[] castsRequired;
            invalidMethod = et.GetMethod("A", 0, true, true, out castsRequired, TypeSearch.Void);
            Assert.IsNotNull(invalidMethod);
            Assert.IsTrue(castsRequired[0]);

            Method method2 = et.GetMethod("A", typeof (int), TypeSearch.Void);
            Assert.IsNotNull(method2);
            Assert.AreNotEqual(method1, method2);

            Method method3 = et.GetMethod("A", typeof (int).MakeByRefType(), TypeSearch.Void);
            Assert.IsNotNull(method3);
            Assert.AreNotEqual(method3, method1);
            Assert.AreNotEqual(method3, method2);

            Method method3b = et.GetMethod("A", typeof (int).MakePointerType(), TypeSearch.Void);
            Assert.IsNotNull(method3b);
            Assert.AreNotEqual(method3b, method1);
            Assert.AreNotEqual(method3b, method2);

            Method method4 = et.GetMethod("A", typeof (string), typeof (string), TypeSearch.Void);
            Assert.IsNotNull(method4);
            Assert.IsTrue(method4.Info.IsStatic);
            Assert.AreNotEqual(method4, method1);
            Assert.AreNotEqual(method4, method2);
            Assert.AreNotEqual(method4, method3);

            Method method5 = et.GetMethod("A", typeof (string), typeof (string).MakeByRefType(), TypeSearch.Void);
            Assert.IsNotNull(method5);
            Assert.AreNotEqual(method5, method1);
            Assert.AreNotEqual(method5, method2);
            Assert.AreNotEqual(method5, method3b);
            Assert.AreNotEqual(method5, method4);

            Method method6 = et.GetMethod("A", 1, typeof (string), TypeSearch.Void);
            Assert.IsNotNull(method6);
            Assert.AreNotEqual(method6, method1);
            Assert.AreNotEqual(method6, method2);
            Assert.AreNotEqual(method6, method3b);
            Assert.AreNotEqual(method6, method4);
            Assert.AreNotEqual(method6, method5);

            Method method7 = et.GetMethod("A", 2, typeof (string), TypeSearch.Void);
            Assert.IsNotNull(method7);
            Assert.AreNotEqual(method7, method1);
            Assert.AreNotEqual(method7, method2);
            Assert.AreNotEqual(method7, method3b);
            Assert.AreNotEqual(method7, method4);
            Assert.AreNotEqual(method7, method5);
            Assert.AreNotEqual(method7, method6);

            Method method8 = et.GetMethod("A", 1, new TypeSearch(GenericArgumentLocation.Type, "T"),
                                          new TypeSearch(GenericArgumentLocation.Signature, 0, true),
                                          TypeSearch.Void);
            Assert.IsNotNull(method8);
        }

        [TestMethod]
        public void ExtendedType_GetMethod_CreatesConcreteMethods()
        {
            // Get the open type.
            ExtendedType openType = ExtendedType.Get(typeof (ComplexOverloads<>));
            Assert.IsTrue(openType.Type.ContainsGenericParameters);

            // Note this not only creates a concrete method by setting the method type param to Guid
            // It also has to make the enclosing type concrete by changing it to ComplexOverloads<bool> as
            // the first parameter uses the type's generic parameter.
            //
            // It actually matches: public void A<T1>(T a, ref T1 b) {}
            Method concreteMethod = openType.GetMethod("A", 1, typeof (bool), typeof (Guid).MakeByRefType(),
                                                       TypeSearch.Void);
            Assert.IsNotNull(concreteMethod);
            Assert.IsFalse(concreteMethod.Info.ContainsGenericParameters);
            Assert.IsFalse(concreteMethod.ExtendedType.Type.ContainsGenericParameters);
            // The GetMethod
            Assert.AreNotSame(openType, concreteMethod.ExtendedType);
        }

        [TestMethod]
        public void ExtendedType_CanGetConcreteMethods()
        {
            // Get the open type.
            ExtendedType openType = ExtendedType.Get(typeof (ComplexOverloads<>));
            Assert.IsTrue(openType.Type.ContainsGenericParameters);

            // Close the type
            ExtendedType closedType = openType.CloseType(typeof (int));
            Assert.IsNotNull(closedType);
            Assert.IsFalse(closedType.Type.ContainsGenericParameters);
            Assert.AreSame(openType.Type, closedType.Type.GetGenericTypeDefinition());

            // This time we have a concrete type.
            Method method = closedType.GetMethod("A", typeof (int));
            Assert.IsNotNull(method);
            // This method has no generic arguments and the type is concrete.
            Assert.IsFalse(method.Info.ContainsGenericParameters);

            Method method2 = closedType.GetMethod("A", 1, typeof (string), TypeSearch.Void);
            Assert.IsNotNull(method2);
            // This method has generic arguments and so it does contain generic parameters even though type is concrete.
            Assert.IsTrue(method2.Info.ContainsGenericParameters);

            // We can use the CloseMethod overload to close the method.
            Method method3 = method2.Close(typeof (int));
            Assert.IsNotNull(method3);
            // We haven't closed the extended type.
            Assert.AreSame(method2.ExtendedType, method3.ExtendedType);
            Assert.IsFalse(method3.ExtendedType.Type.ContainsGenericParameters);
            Assert.IsFalse(method3.Info.ContainsGenericParameters);

            // Finally we get the open type's open method.
            Method method4 = openType.GetMethod("A", 1, typeof (string), TypeSearch.Void);
            Assert.IsNotNull(method4);
            Assert.IsTrue(method4.Info.ContainsGenericParameters);
        }

        [TestMethod]
        public void ExtendedType_CanDisambiguateConstructors()
        {
            // Implicit cast supported
            ExtendedType et = typeof (ComplexOverloads<>);

            // Grab all constructors
            IEnumerable<Constructor> constructors = et.Constructors;
            Assert.IsNotNull(constructors);
            // Note only two non-static constructors (static constructor treated separately).
            Assert.AreEqual(2, constructors.Count());

            // Grab the static constructor
            Constructor staticConstructor = et.StaticConstructor;
            Assert.IsNotNull(staticConstructor);
            Assert.IsTrue(staticConstructor.Info.IsStatic);
            Assert.IsFalse(constructors.Contains(staticConstructor));

            // Retrieve parameterless constructor
            Constructor constructor = et.GetConstructor(typeof (ComplexOverloads<int>));
            Assert.IsNotNull(constructor);

            // Retrieve the generic constructor
            Constructor genericConstructor = et.GetConstructor(TypeSearch.T1, typeof (string),
                                                               typeof (ComplexOverloads<int>));
            Assert.IsNotNull(genericConstructor);
            Assert.AreNotSame(constructor, genericConstructor);
        }

        [TestMethod]
        public void ExtendedType_CanGetConcreteGenericConstructor()
        {
            // Retrieve the generic constructor for a generic type, but search for concrete types.
            Constructor genericConstructor = ((ExtendedType) typeof (ComplexOverloads<>)).GetConstructor(
                typeof (int), typeof (string), typeof (ComplexOverloads<int>));
            Assert.IsNotNull(genericConstructor);
            Assert.IsFalse(genericConstructor.ExtendedType.Type.ContainsGenericParameters);

            // Check that our declaring type has been changed to int automagically
            Assert.AreEqual(typeof (int), genericConstructor.ExtendedType.GenericArguments.First().Type);
        }

        [TestMethod]
        public void ExtendedType_CanDisambiguateIndexers()
        {
            // Retrieve indexers
            List<Indexer> indexers = ((ExtendedType) typeof (ComplexOverloads<>)).Indexers.ToList();
            Assert.IsNotNull(indexers);
            Assert.AreEqual(3, indexers.Count);
        }

        [TestMethod]
        public void ExtendedType_CanDetectAutomaticProperties()
        {
            ExtendedType et = typeof (ComplexOverloads<>);
            Property property = et.GetProperty("Automatic");
            Assert.IsNotNull(property);
            Assert.IsTrue(property.IsAutomatic);
        }

        [TestMethod]
        public void Method_ParameterlessInstanceFuncValid()
        {
            ExtendedType et = typeof (ComplexOverloads<int>);
            Method method = et.GetMethod("GetOne", typeof (int));
            Assert.IsNotNull(method);
            Func<ComplexOverloads<int>, int> getOne = method.GetFunc(
                typeof (ComplexOverloads<int>), typeof (int)) as Func<ComplexOverloads<int>, int>;
            Assert.IsNotNull(getOne);

            ComplexOverloads<int> obj = new ComplexOverloads<int>();
            Assert.AreEqual(1, getOne(obj));
        }

        [TestMethod]
        public void Method_InstanceFuncValid()
        {
            ExtendedType et = typeof (ComplexOverloads<int>);
            Method method = et.GetMethod("Add", typeof (int), typeof (int), typeof (int));
            Assert.IsNotNull(method);
            Func<ComplexOverloads<int>, int, int, int> add = method.GetFunc(
                typeof (ComplexOverloads<int>), typeof (int), typeof (int), typeof (int)) as
                                                             Func<ComplexOverloads<int>, int, int, int>;
            Assert.IsNotNull(add);
            int a = Random.Next();
            int b = Random.Next();

            ComplexOverloads<int> obj = new ComplexOverloads<int>();
            Assert.AreEqual(a + b, add(obj, a, b));
        }

        [TestMethod]
        public void Method_ParameterlessStaticFuncValid()
        {
            ExtendedType et = typeof (ComplexOverloads<int>);
            Method method = et.GetMethod("StaticGetOne", typeof (int));
            Assert.IsNotNull(method);
            Func<int> getOne = method.GetFunc(typeof (int)) as Func<int>;
            Assert.IsNotNull(getOne);
            Assert.AreEqual(1, getOne());
        }

        [TestMethod]
        public void Method_StaticFuncValid()
        {
            ExtendedType et = typeof (ComplexOverloads<int>);
            Method method = et.GetMethod("StaticAdd", typeof (int), typeof (int), typeof (int));
            Assert.IsNotNull(method);
            Func<int, int, int> add = method.GetFunc(typeof (int), typeof (int), typeof (int)) as Func<int, int, int>;
            Assert.IsNotNull(add);
            int a = Random.Next();
            int b = Random.Next();
            Assert.AreEqual(a + b, add(a, b));
        }

        [TestMethod]
        public void Method_ParameterlessInstanceActionValid()
        {
            ExtendedType et = typeof (ComplexOverloads<int>);
            Method method = et.GetMethod("IncrementCounter", typeof (int));
            Assert.IsNotNull(method);
            Action<ComplexOverloads<int>> increment = method.GetAction(
                typeof (ComplexOverloads<int>)) as Action<ComplexOverloads<int>>;
            Assert.IsNotNull(increment);

            ComplexOverloads<int> obj = new ComplexOverloads<int>();
            int original = obj.Counter;
            increment(obj);
            Assert.AreEqual(original + 1, obj.Counter);
        }

        [TestMethod]
        public void Method_InstanceActionValid()
        {
            ExtendedType et = typeof (ComplexOverloads<int>);
            Method method = et.GetMethod("IncrementCounter", typeof (int), typeof (void));
            Assert.IsNotNull(method);
            Action<ComplexOverloads<int>, int> increment =
                method.GetAction(typeof (ComplexOverloads<int>), typeof (int)) as Action<ComplexOverloads<int>, int>;
            Assert.IsNotNull(increment);

            ComplexOverloads<int> obj = new ComplexOverloads<int>();
            int original = obj.Counter;
            int a = Random.Next();
            increment(obj, a);
            Assert.AreEqual(original + a, obj.Counter);
        }

        [TestMethod]
        public void Method_ParameterlessStaticActionValid()
        {
            ExtendedType et = typeof (ComplexOverloads<int>);
            Method method = et.GetMethod("StaticIncrementCounter", typeof (void));
            Assert.IsNotNull(method);
            Action increment = method.GetAction() as Action;
            Assert.IsNotNull(increment);

            int original = ComplexOverloads<int>.StaticCounter;
            increment();
            Assert.AreEqual(original + 1, ComplexOverloads<int>.StaticCounter);
        }

        [TestMethod]
        public void Method_StaticActionValid()
        {
            ExtendedType et = typeof (ComplexOverloads<int>);
            Method method = et.GetMethod("StaticIncrementCounter", typeof (int), typeof (void));
            Assert.IsNotNull(method);
            Action<int> increment = method.GetAction(typeof (int)) as Action<int>;
            Assert.IsNotNull(increment);

            int a = Random.Next();
            int original = ComplexOverloads<int>.StaticCounter;
            increment(a);
            Assert.AreEqual(original + a, ComplexOverloads<int>.StaticCounter);
        }

        [TestMethod]
        public void Constructor_FuncConstructorValid()
        {
            ExtendedType et = typeof (ComplexOverloads<int>);
            Constructor constructor = et.GetConstructor(typeof (int), typeof (string), typeof (ComplexOverloads<int>));
            Assert.IsNotNull(constructor);
            Func<int, string, ComplexOverloads<int>> constFunc =
                constructor.GetFunc(typeof (int), typeof (string), typeof (ComplexOverloads<int>)) as
                Func<int, string, ComplexOverloads<int>>;
            Assert.IsNotNull(constFunc);
            int a = Random.Next();
            string s = Random.RandomString();
            ComplexOverloads<int> co = constFunc(a, s);
            Assert.IsNotNull(co);
            Assert.AreEqual(a, co.Value);
            Assert.AreEqual(s, co.Value2);
        }

        [TestMethod]
        public void ExtendedType_SearchWithCast()
        {
            ExtendedType et = typeof (ComplexOverloads<int>);
            Method add = et.GetMethod("Add", typeof (short), typeof (short), typeof (int));
            Assert.IsNotNull(add);
        }

        #region Nested type: ComplexOverloads
        /// <summary>
        /// A class with complicated overloads illustrating classic failure cases.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <remarks></remarks>
        private class ComplexOverloads<T>
        {
            public static int StaticCounter;
            public readonly T Value;
            public int Counter;
            public string Value2;

            public ComplexOverloads()
            {
            }

            public ComplexOverloads(T value, string value2)
            {
                Value = value;
                Value2 = value2;
            }

            public T this[T index]
            {
                get { return default(T); }
                set { }
            }

            public int this[int index]
            {
                get { return 0; }
                set { }
            }

            public int this[T index1, int index2]
            {
                get { return 0; }
                set { }
            }

            public T Automatic { get; set; }

            public T A()
            {
                return default(T);
            }

            public void A(int a)
            {
            }

            public void A(ref int a)
            {
            }

            public unsafe void A(int* a)
            {
            }

            public static void A(string a, string b = null)
            {
            }

            public void A(string a, out string b)
            {
                b = "Test String";
            }

            public void A<TException>(string a)
            {
            }

            public void A<T1, T2>(string a)
            {
            }

            public void A<T1>(T a, ref T1 b)
            {
            }

            public static explicit operator int(ComplexOverloads<T> a)
            {
                return 1;
            }

            public static explicit operator Int16(ComplexOverloads<T> a)
            {
                return 1;
            }

            public int Add(int a, int b)
            {
                return a + b;
            }

            public int GetOne()
            {
                return 1;
            }

            public static int StaticAdd(int a, int b)
            {
                return a + b;
            }

            public static int StaticGetOne()
            {
                return 1;
            }

            public int IncrementCounter()
            {
                return Counter++;
            }

            public void IncrementCounter(int a)
            {
                Counter += a;
            }

            public static void StaticIncrementCounter()
            {
                StaticCounter++;
            }

            public static void StaticIncrementCounter(int a)
            {
                StaticCounter += a;
            }
        }
        #endregion
    }
}