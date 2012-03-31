using System;
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
            public void A() {}
            public void A(string a) { }
            public void A(ref string a) { }
            public void A(string a, string b = null) { }
            public void A(string a, out string b)
            {
                b = null;
            }

            public void A<TException>(string a) { }
            public void A<T1, T2>(string a) { }

            public static explicit operator int(ComplexOverloads<T> a)
            {
                return 1;
            }
            public static explicit operator Int16(ComplexOverloads<T> a)
            {
                return 1;
            }
        }

        [TestMethod]
        public void ExtendedType_CanDisambiguateMembers()
        {
            ExtendedType et = ExtendedType.Get(typeof (ComplexOverloads<>));

            Methods methods = et.GetMethods("A");

            Assert.IsNotNull(methods);
            Assert.AreEqual(7, methods.Overloads.Count());

            Method method1 = methods.GetOverload(typeof(void));
            Assert.IsTrue(method1.Info.ContainsGenericParameters);
            Assert.IsNotNull(method1);

            Method method2 = methods.GetOverload(typeof(string), typeof(void));
            Assert.IsNotNull(method2);
            Assert.AreNotEqual(method1, method2);

            Method method3 = methods.GetOverload(typeof(string).MakeByRefType(), typeof(void));
            Assert.IsNotNull(method3);
            Assert.AreNotEqual(method3, method1);
            Assert.AreNotEqual(method3, method2);

            Method method4 = methods.GetOverload(typeof(string), typeof(string), typeof(void));
            Assert.IsNotNull(method4);
            Assert.AreNotEqual(method4, method1);
            Assert.AreNotEqual(method4, method2);
            Assert.AreNotEqual(method4, method3);

            Method method5 = methods.GetOverload(typeof(string), typeof(string).MakeByRefType(), typeof(void));
            Assert.IsNotNull(method5);
            Assert.AreNotEqual(method5, method1);
            Assert.AreNotEqual(method5, method2);
            Assert.AreNotEqual(method5, method3);
            Assert.AreNotEqual(method5, method4);

            Method method6 = methods.GetOverload(1, typeof(string), typeof(void));
            Assert.IsNotNull(method6);
            Assert.AreNotEqual(method6, method1);
            Assert.AreNotEqual(method6, method2);
            Assert.AreNotEqual(method6, method3);
            Assert.AreNotEqual(method6, method4);
            Assert.AreNotEqual(method6, method5);

            Method method7 = methods.GetOverload(2, typeof(string), typeof(void));
            Assert.IsNotNull(method7);
            Assert.AreNotEqual(method7, method1);
            Assert.AreNotEqual(method7, method2);
            Assert.AreNotEqual(method7, method3);
            Assert.AreNotEqual(method7, method4);
            Assert.AreNotEqual(method7, method5);
            Assert.AreNotEqual(method7, method6);
        }

        [TestMethod]
        public void ExtendedType_CanGetConcreteMethods()
        {
            // Get the open type.
            ExtendedType openType = ExtendedType.Get(typeof (ComplexOverloads<>));
            Assert.IsTrue(openType.Type.ContainsGenericParameters);

            // Close the type
            ExtendedType closedType = openType.CloseType(typeof (int));
            Assert.IsFalse(closedType.Type.ContainsGenericParameters);
            Assert.AreSame(openType.Type, closedType.Type.GetGenericTypeDefinition());

            // This time we have a concrete type.
            Method method = closedType.GetMethod("A", typeof(void));
            Assert.IsNotNull(method);
            // This method has no generic arguments and the type is concrete.
            Assert.IsFalse(method.Info.ContainsGenericParameters);

            Method method2 = closedType.GetMethod("A", 1, typeof(string), typeof(void));
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
            Method method4 = openType.GetMethod("A", 1, typeof(string), typeof(void));
            Assert.IsNotNull(method4);
            Assert.IsTrue(method4.Info.ContainsGenericParameters);

            // To close this method, we have to close the type and the method (i.e. two types)
            Method method5 = method4.CloseMethod(typeof(string), typeof(int));
            Assert.IsNotNull(method5);
            // The extended type will have been closed, so shouldn't be equal
            Assert.AreNotSame(method4.ExtendedType, method5.ExtendedType);
            Assert.IsTrue(method4.ExtendedType.Type.ContainsGenericParameters);
            Assert.IsFalse(method5.ExtendedType.Type.ContainsGenericParameters);
            Assert.IsFalse(method5.Info.ContainsGenericParameters);
        }
    }
}
