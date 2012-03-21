#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities.Test
// File: SingletonTests.cs
// 
// This software, its object code and source code and all modifications made to
// the same (the “Software”) are, and shall at all times remain, the proprietary
// information and shortellectual property rights of Web Applications (UK) Limited. 
// You are only entitled to use the Software as expressly permitted by Web
// Applications (UK) Limited within the Software Customisation and
// Licence Agreement (the “Agreement”).  Any copying, modification, decompiling,
// distribution, licensing, sale, transfer or other use of the Software other than
// as expressly permitted in the Agreement is expressly forbidden.  Web
// Applications (UK) Limited reserves its rights to take action against you and
// your employer in accordance with its contractual and common law rights
// (including injunctive relief) should you breach the terms of the Agreement or
// otherwise infringe its copyright or other shortellectual property rights in the
// Software.
// 
// © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
#endregion

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;

namespace WebApplications.Utilities.Test
{
    [TestClass]
    public class SingletonTests : TestBase
    {

        private class TestSingletonWithoutKeyedConstructor : Singleton<int, TestSingletonWithoutKeyedConstructor>
        {
            // Note: In order to create valid code, the correct base constructer must be called still
            public TestSingletonWithoutKeyedConstructor() : base(0)
            {
            }
        }

        private class TestSingletonWithKeyedConstructor<TKey> : Singleton<TKey, TestSingletonWithKeyedConstructor<TKey>>
        {
            public TKey myKey;

            protected TestSingletonWithKeyedConstructor(TKey key)
                : base(key)
            {
                myKey = key;
            }

            public static TestSingletonWithKeyedConstructor<TKey> Get(TKey key)
            {
                return GetSingleton(key);
            }
        }

        private class TestInitialisedSingletonWithKeyedConstructor<TKey> : InitialisedSingleton<TKey, TestInitialisedSingletonWithKeyedConstructor<TKey>>
        {
            public TKey myKey;

            public static int TimesInitialised = 0;

            protected TestInitialisedSingletonWithKeyedConstructor(TKey key)
                : base(key)
            {
                myKey = key;
            }

            public static TestInitialisedSingletonWithKeyedConstructor<TKey> Get(TKey key)
            {
                return GetSingleton(key);
            }

            protected override void Initialise()
            {
                TimesInitialised++;
            }
        }


        [TestMethod]
        [ExpectedException(typeof(TypeInitializationException))]
        public void Singleton_NoAppropriateConstructor_ThrowsTypeInitializationException()
        {
            // The test class here does not have a constructor which accepts a key parameter.
            TestSingletonWithoutKeyedConstructor testSingleton = new TestSingletonWithoutKeyedConstructor();
        }

        [TestMethod]
        public void Singleton_Instantiated_CallsConstructerWithKey()
        {
            string key = Random.Next().ToString();
            TestSingletonWithKeyedConstructor<string> testSingleton = TestSingletonWithKeyedConstructor<string>.Get(key);
            // The test class used here will set the value of the public field myKey from the key supplied to its constructer
            Assert.AreEqual(key, testSingleton.myKey, "When an instance of a singleton is created using GetSingleton, the constructor should be called with the key supplied as a parameter.");
        }

        [TestMethod]
        public void Singleton_WithDuplicateKeys_AreIdentical()
        {
            int key = Random.Next();
            TestSingletonWithKeyedConstructor<int> testSingleton1 = TestSingletonWithKeyedConstructor<int>.Get(key);
            TestSingletonWithKeyedConstructor<int> testSingleton2 = TestSingletonWithKeyedConstructor<int>.Get(key);
            TestSingletonWithKeyedConstructor<int> testSingleton3 = TestSingletonWithKeyedConstructor<int>.Get(key);
            Assert.AreSame(testSingleton1, testSingleton2, "Creating multiple singletons with the same key should result in the same object each time.");
            Assert.AreSame(testSingleton2, testSingleton3, "Creating multiple singletons with the same key should result in the same object each time.");
        }

        [TestMethod]
        public void InitialisedSingleton_Instantiated_CallsConstructerWithKey()
        {
            string key = Random.Next().ToString();
            TestInitialisedSingletonWithKeyedConstructor<string> testInitialisedSingleton = TestInitialisedSingletonWithKeyedConstructor<string>.Get(key);
            // The test class used here will set the value of the public field myKey from the key supplied to its constructer
            Assert.AreEqual(key, testInitialisedSingleton.myKey, "When an instance of an Initialised Singleton is created using GetSingleton, the constructor should be called with the key supplied as a parameter.");
        }

        [TestMethod]
        public void InitialisedSingleton_WithDuplicateKeys_AreIdentical()
        {
            int key = Random.Next();
            TestInitialisedSingletonWithKeyedConstructor<int> testInitialisedSingleton1 = TestInitialisedSingletonWithKeyedConstructor<int>.Get(key);
            TestInitialisedSingletonWithKeyedConstructor<int> testInitialisedSingleton2 = TestInitialisedSingletonWithKeyedConstructor<int>.Get(key);
            TestInitialisedSingletonWithKeyedConstructor<int> testInitialisedSingleton3 = TestInitialisedSingletonWithKeyedConstructor<int>.Get(key);
            Assert.AreSame(testInitialisedSingleton1, testInitialisedSingleton2, "Creating multiple Initialised Singletons with the same key should result in the same object each time.");
            Assert.AreSame(testInitialisedSingleton2, testInitialisedSingleton3, "Creating multiple Initialised Singletons with the same key should result in the same object each time.");
        }

        [TestMethod]
        public void InitialisedSingleton_WithDuplicateKeys_InitialisedOnce()
        {
            DateTime key = DateTime.Now;
            Assert.AreEqual(0, TestInitialisedSingletonWithKeyedConstructor<DateTime>.TimesInitialised,
                   "This test relies on there being no previously created singletons of the type used here.");
            var testSingletons = Enumerable.Range(1, 10).AsParallel().Select(x => TestInitialisedSingletonWithKeyedConstructor<DateTime>.Get(key)).ToArray();
            Assert.AreEqual(1, TestInitialisedSingletonWithKeyedConstructor<DateTime>.TimesInitialised,
                   "When duplicate Initialised Singletons are created with the same key, the initialise method should only be called once.");
        }
    }
}
