using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;
using WebApplications.Utilities.Relection;

namespace WebApplications.Utilities.Test.Reflection
{
    [TestClass]
    public class ExtendedTypeTests : TestBase
    {
        /// <summary>
        /// A class with complicated overloads illustrating classic failure cases.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <remarks></remarks>
        private class ComplexOverloads<T>
        {
            public T A()
            {
                return default(T);
            }
            public void A(int a) { }
            public void A(ref int a) { }
            public unsafe void A(int* a) { }
            public static void A(string a, string b = null) { }
            public void A(string a, out string b)
            {
                b = null;
            }

            public void A<TException>(string a) { }
            public void A<T1, T2>(string a) { }
            public void A<T1>(T a, ref T1 b) { }

            public static explicit operator int(ComplexOverloads<T> a)
            {
                return 1;
            }
            public static explicit operator Int16(ComplexOverloads<T> a)
            {
                return 1;
            }

            public readonly T Value;

            public string Value2;

            static ComplexOverloads()
            {
            }
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
                set {}
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
        }

        [TestMethod]
        public void ExtendedType_CanDisambiguateMembers()
        {
            ExtendedType et = ExtendedType.Get(typeof(ComplexOverloads<>));

            Methods methods = et.GetMethods("A");

            Assert.IsNotNull(methods);
            Assert.AreEqual(9, methods.Overloads.Count());

            Method invalidMethod = methods.GetOverload();
            Assert.IsNull(invalidMethod);

            Method method1 = methods.GetOverload(TypeSearch.T1);
            Assert.IsNotNull(method1);
            Assert.IsTrue(method1.Info.ContainsGenericParameters);

            invalidMethod = methods.GetOverload(TypeSearch.Void);
            Assert.IsNull(invalidMethod);

            Method method2 = methods.GetOverload(typeof(int), TypeSearch.Void);
            Assert.IsNotNull(method2);
            Assert.AreNotEqual(method1, method2);

            Method method3 = methods.GetOverload(typeof(int).MakeByRefType(), TypeSearch.Void);
            Assert.IsNotNull(method3);
            Assert.AreNotEqual(method3, method1);
            Assert.AreNotEqual(method3, method2);

            Method method3b = methods.GetOverload(typeof(int).MakePointerType(), TypeSearch.Void);
            Assert.IsNotNull(method3b);
            Assert.AreNotEqual(method3b, method1);
            Assert.AreNotEqual(method3b, method2);

            Method method4 = methods.GetOverload(typeof(string), typeof(string), TypeSearch.Void);
            Assert.IsNotNull(method4);
            Assert.IsTrue(method4.Info.IsStatic);
            Assert.AreNotEqual(method4, method1);
            Assert.AreNotEqual(method4, method2);
            Assert.AreNotEqual(method4, method3);

            Method method5 = methods.GetOverload(typeof(string), typeof(string).MakeByRefType(), TypeSearch.Void);
            Assert.IsNotNull(method5);
            Assert.AreNotEqual(method5, method1);
            Assert.AreNotEqual(method5, method2);
            Assert.AreNotEqual(method5, method3b);
            Assert.AreNotEqual(method5, method4);

            Method method6 = methods.GetOverload(1, typeof(string), TypeSearch.Void);
            Assert.IsNotNull(method6);
            Assert.AreNotEqual(method6, method1);
            Assert.AreNotEqual(method6, method2);
            Assert.AreNotEqual(method6, method3b);
            Assert.AreNotEqual(method6, method4);
            Assert.AreNotEqual(method6, method5);

            Method method7 = methods.GetOverload(2, typeof(string), TypeSearch.Void);
            Assert.IsNotNull(method7);
            Assert.AreNotEqual(method7, method1);
            Assert.AreNotEqual(method7, method2);
            Assert.AreNotEqual(method7, method3b);
            Assert.AreNotEqual(method7, method4);
            Assert.AreNotEqual(method7, method5);
            Assert.AreNotEqual(method7, method6);

            Method method8 = methods.GetOverload(1, new TypeSearch(GenericArgumentLocation.Type, "T"),
                                                 new TypeSearch(GenericArgumentLocation.Signature, 0, true),
                                                 TypeSearch.Void);
            Assert.IsNotNull(method8);
        }

        [TestMethod]
        public void ExtendedType_GetMethod_CreatesConcreteMethods()
        {
            // Get the open type.
            ExtendedType openType = ExtendedType.Get(typeof(ComplexOverloads<>));
            Assert.IsTrue(openType.Type.ContainsGenericParameters);

            // Note this not only creates a concrete method by setting the method type param to Guid
            // It also has to make the enclosing type concrete by changing it to ComplexOverloads<bool> as
            // the first paramter uses the type's generic parameter.
            //
            // It actually matches: public void A<T1>(T a, ref T1 b) {}
            Method concreteMethod = openType.GetMethod("A", 1, typeof(bool), typeof(Guid).MakeByRefType(), TypeSearch.Void);
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
            ExtendedType openType = ExtendedType.Get(typeof(ComplexOverloads<>));
            Assert.IsTrue(openType.Type.ContainsGenericParameters);

            // Close the type
            ExtendedType closedType = openType.CloseType(typeof(int));
            Assert.IsFalse(closedType.Type.ContainsGenericParameters);
            Assert.AreSame(openType.Type, closedType.Type.GetGenericTypeDefinition());

            // This time we have a concrete type.
            Method method = closedType.GetMethod("A", typeof(int));
            Assert.IsNotNull(method);
            // This method has no generic arguments and the type is concrete.
            Assert.IsFalse(method.Info.ContainsGenericParameters);

            Method method2 = closedType.GetMethod("A", 1, typeof(string), TypeSearch.Void);
            Assert.IsNotNull(method2);
            // This method has generic arguments and so it does contain generic parameters even though type is concrete.
            Assert.IsTrue(method2.Info.ContainsGenericParameters);

            // We can use the CloseMethod overload to close the method.
            Method method3 = method2.CloseMethod(typeof(int));
            Assert.IsNotNull(method3);
            // We haven't closed the extended type.
            Assert.AreSame(method2.ExtendedType, method3.ExtendedType);
            Assert.IsFalse(method3.ExtendedType.Type.ContainsGenericParameters);
            Assert.IsFalse(method3.Info.ContainsGenericParameters);

            // Finally we get the open type's open method.
            Method method4 = openType.GetMethod("A", 1, typeof(string), TypeSearch.Void);
            Assert.IsNotNull(method4);
            Assert.IsTrue(method4.Info.ContainsGenericParameters);
        }

        [TestMethod]
        public void ExtendedType_CanDisambiguateConstructors()
        {
            ExtendedType et = typeof(ComplexOverloads<>);

            // There are two ways to retrieve all the constructors
            Constructors constructors = et.Constructors;
            Assert.IsNotNull(constructors);

            // The second way is for consistency.
            Constructors constructors2 = et.GetConstructors();
            Assert.IsNotNull(constructors2);
            Assert.AreSame(constructors, constructors2);

            // Grab the static constructor
            Constructor staticConstructor = constructors.StaticConstructor;
            Assert.IsNotNull(staticConstructor);
            Assert.IsTrue(staticConstructor.Info.IsStatic);

            // Retrieve parameterless constructor
            Constructor constructor = et.GetConstructor();
            Assert.IsNotNull(constructor);

            // Retrieve the generic constructor
            Constructor genericConstructor = et.GetConstructor(TypeSearch.T1, typeof(string));
            Assert.IsNotNull(genericConstructor);
            Assert.AreNotSame(constructor, genericConstructor);
        }

        [TestMethod]
        public void ExtendedType_CanGetConcreteGenericConstructor()
        {
            // Retrieve the generic constructor for a generic type, but search for concrete types.
            Constructor genericConstructor = ((ExtendedType)typeof(ComplexOverloads<>)).GetConstructor(typeof(int), typeof(string));
            Assert.IsNotNull(genericConstructor);
            Assert.IsFalse(genericConstructor.ExtendedType.Type.ContainsGenericParameters);
            
            // Check that our declaring type has been changed to int automagically
            Assert.AreEqual(typeof(int), genericConstructor.ExtendedType.GenericArguments.First().Type);
        }

        [TestMethod]
        public void ExtendedType_CanDisambiguateIndexers()
        {
            // Retrieve indexers
            List<Indexer> indexers = ((ExtendedType) typeof (ComplexOverloads<>)).Indexers.ToList();
            Assert.IsNotNull(indexers);
            Assert.AreEqual(3, indexers.Count);
        }
    }
}
