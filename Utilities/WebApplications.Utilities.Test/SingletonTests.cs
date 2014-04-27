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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebApplications.Utilities.Test
{
    [TestClass]
    public class SingletonTests : UtilitiesTestBase
    {
        [TestMethod]
        [ExpectedException(typeof (TypeInitializationException))]
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
            Assert.AreEqual(
                key,
                testSingleton.myKey,
                "When an instance of a singleton is created using GetSingleton, the constructor should be called with the key supplied as a parameter.");
        }

        [TestMethod]
        public void Singleton_WithDuplicateKeys_AreIdentical()
        {
            int key = Random.Next();
            TestSingletonWithKeyedConstructor<int> testSingleton1 = TestSingletonWithKeyedConstructor<int>.Get(key);
            TestSingletonWithKeyedConstructor<int> testSingleton2 = TestSingletonWithKeyedConstructor<int>.Get(key);
            TestSingletonWithKeyedConstructor<int> testSingleton3 = TestSingletonWithKeyedConstructor<int>.Get(key);
            Assert.AreSame(
                testSingleton1,
                testSingleton2,
                "Creating multiple singletons with the same key should result in the same object each time.");
            Assert.AreSame(
                testSingleton2,
                testSingleton3,
                "Creating multiple singletons with the same key should result in the same object each time.");
        }

        [TestMethod]
        public void InitialisedSingleton_Instantiated_CallsConstructerWithKey()
        {
            string key = Random.Next().ToString();
            TestInitialisedSingletonWithKeyedConstructor<string> testInitialisedSingleton =
                TestInitialisedSingletonWithKeyedConstructor<string>.Get(key);
            // The test class used here will set the value of the public field myKey from the key supplied to its constructer
            Assert.AreEqual(
                key,
                testInitialisedSingleton.myKey,
                "When an instance of an Initialised Singleton is created using GetSingleton, the constructor should be called with the key supplied as a parameter.");
        }

        [TestMethod]
        public void InitialisedSingleton_WithDuplicateKeys_AreIdentical()
        {
            int key = Random.Next();
            TestInitialisedSingletonWithKeyedConstructor<int> testInitialisedSingleton1 =
                TestInitialisedSingletonWithKeyedConstructor<int>.Get(key);
            TestInitialisedSingletonWithKeyedConstructor<int> testInitialisedSingleton2 =
                TestInitialisedSingletonWithKeyedConstructor<int>.Get(key);
            TestInitialisedSingletonWithKeyedConstructor<int> testInitialisedSingleton3 =
                TestInitialisedSingletonWithKeyedConstructor<int>.Get(key);
            Assert.AreSame(
                testInitialisedSingleton1,
                testInitialisedSingleton2,
                "Creating multiple Initialised Singletons with the same key should result in the same object each time.");
            Assert.AreSame(
                testInitialisedSingleton2,
                testInitialisedSingleton3,
                "Creating multiple Initialised Singletons with the same key should result in the same object each time.");
        }

        [TestMethod]
        public void InitialisedSingleton_WithDuplicateKeys_InitialisedOnce()
        {
            DateTime key = DateTime.Now;
            Assert.AreEqual(
                0,
                TestInitialisedSingletonWithKeyedConstructor<DateTime>.TimesInitialised,
                "This test relies on there being no previously created singletons of the type used here.");
            TestInitialisedSingletonWithKeyedConstructor<DateTime>[] testSingletons =
                Enumerable.Range(1, 10).AsParallel().Select(
                    x => TestInitialisedSingletonWithKeyedConstructor<DateTime>.Get(key)).ToArray();
            Assert.AreEqual(
                1,
                TestInitialisedSingletonWithKeyedConstructor<DateTime>.TimesInitialised,
                "When duplicate Initialised Singletons are created with the same key, the initialise method should only be called once.");
        }

        #region Nested type: TestInitialisedSingletonWithKeyedConstructor
        private class TestInitialisedSingletonWithKeyedConstructor<TKey> :
            InitialisedSingleton<TKey, TestInitialisedSingletonWithKeyedConstructor<TKey>>
        {
            public static int TimesInitialised;
            public TKey myKey;

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
        #endregion

        #region Nested type: TestSingletonWithKeyedConstructor
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
        #endregion

        #region Nested type: TestSingletonWithoutKeyedConstructor
        private class TestSingletonWithoutKeyedConstructor : Singleton<int, TestSingletonWithoutKeyedConstructor>
        {
            // Note: In order to create valid code, the correct base constructer must be called still
            public TestSingletonWithoutKeyedConstructor()
                : base(0)
            {
            }
        }
        #endregion
    }
}