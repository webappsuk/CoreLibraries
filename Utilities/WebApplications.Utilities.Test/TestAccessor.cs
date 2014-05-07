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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebApplications.Utilities.Test
{
    [TestClass]
    public class TestAccessor
    {
        public class TestClass
        {
            public readonly bool PublicReadonlyField = true;
            public int PublicField;
            private int _privateField = 3;
            public string PublicAutoProperty { get; set; }
            private string _privateAutoProperty { get; set; }

            public int PublicSetterProperty
            {
                set { _privateField = value; }
            }
            public int PublicGetterProperty
            {
                get { return _privateField; }
            }
        }

        public class TestReadonlyClass
        {
            public readonly bool ReadonlyField = true;
            public bool ReadonlyProperty { get { return true; } }
            public bool PrivateSetter { get; private set; }
        }

        public class TestStaticsClass
        {
            public bool NonStaticField = false;
            public static bool StaticField = true;
            public bool NonStaticProperty { get; set; }
            public static bool StaticProperty { get; set; }

            static TestStaticsClass()
            {
                StaticProperty = true;
            }

            public TestStaticsClass()
            {
                NonStaticProperty = false;
            }
        }

        [TestMethod]
        public void TestCast()
        {
            TestClass tc = new TestClass();
            Accessor<TestClass> tca = tc;
            Assert.IsTrue(tca.ContainsKey("PublicReadonlyField"));
            Assert.IsTrue(tca.Contains(new KeyValuePair<string, object>("PublicField", tc.PublicField)));
            Assert.IsTrue(tca.ContainsKey("PublicAutoProperty"));
            Assert.AreEqual(tc.PublicReadonlyField, tca["PublicReadonlyField"]);
            Assert.AreEqual(tc.PublicField, tca["PublicField"]);
            Assert.AreEqual(tc.PublicAutoProperty, tca["PublicAutoProperty"]);
            tc.PublicField = 2;
            tc.PublicAutoProperty = "test";
            Assert.AreEqual(tc.PublicField, tca["PublicField"]);
            Assert.AreEqual(tc.PublicAutoProperty, tca["PublicAutoProperty"]);
            tca["PublicField"] = 4;
            tca["PublicAutoProperty"] = "test2";
            Assert.AreEqual(4, tc.PublicField);
            Assert.AreEqual("test2", tc.PublicAutoProperty);
            Assert.AreEqual(tc.PublicField, tca["PublicField"]);
            Assert.AreEqual(tc.PublicAutoProperty, tca["PublicAutoProperty"]);
        }

        [TestMethod]
        public void TestGetterSetterOnly()
        {
            TestClass tc = new TestClass();
            Accessor<TestClass> tca = tc;
            object value;
            Assert.IsFalse(tca.TryGetValue("PublicSetterProperty", out value));
            tca["PublicSetterProperty"] = 10;
            Assert.AreEqual(10, tc.PublicGetterProperty);
            Assert.AreEqual(tc.PublicGetterProperty, tca["PublicGetterProperty"]);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void TestSetonly()
        {
            TestClass tc = new TestClass();
            Accessor<TestClass> tca = tc;
            object value = tca["PublicSetterProperty"];
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void TestReadonly()
        {
            TestClass tc = new TestClass();
            Accessor<TestClass> tca = tc;
            tca["PublicReadonlyField"] = false;
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void TestInvalidCast()
        {
            TestClass tc = new TestClass();
            Accessor<TestClass> tca = tc;
            tca["PublicField"] = "invalid";
        }

        [TestMethod]
        public void TestPrivate()
        {
            TestClass tc = new TestClass();
            Accessor<TestClass> tca = new Accessor<TestClass>(tc, includeNonPublic: true);
            object i;
            Assert.IsTrue(tca.TryGetValue("_privateField", out i));
            Assert.AreEqual(3, (int)i);

            Assert.AreEqual(6, tca.Count);
        }

        [TestMethod]
        public void TestEnumerator()
        {
            TestClass tc = new TestClass();
            Accessor<TestClass> tca = tc;
            Assert.AreEqual(4, tca.Count);

            KeyValuePair<string, object>[] kvps = tca.OrderBy(kvp => kvp.Key).ToArray();

            List<string> keys = new List<string>();
            List<object> values = new List<object>();
            foreach (KeyValuePair<string, object> kvp in kvps)
            {
                Trace.WriteLine(string.Format("{0}: {1}", kvp.Key, kvp.Value));
                keys.Add(kvp.Key);
                values.Add(kvp.Value);
            }

            Assert.AreEqual(tca.Count, kvps.Length);

            CollectionAssert.IsSubsetOf(keys, tca.Keys.ToList());
            CollectionAssert.IsSubsetOf(values, tca.Values.ToList());
        }

        [TestMethod]
        public void TestDictionary()
        {
            TestClass tc = new TestClass();
            Accessor<TestClass> tca = new Accessor<TestClass>(tc, supportsNew: true);
            KeyValuePair<string, object> kvp1 = new KeyValuePair<string, object>("Key1", 123);
            KeyValuePair<string, object> kvp2 = new KeyValuePair<string, object>("Key2", 456);
            KeyValuePair<string, object> kvp3 = new KeyValuePair<string, object>("Key3", 789);

            Assert.AreEqual(4, tca.Count);
            tca.Add("PublicAutoProperty", "Value");

            Assert.AreEqual(4, tca.Count);
            Assert.AreEqual("Value", tc.PublicAutoProperty);

            tca.Add(new KeyValuePair<string, object>("PublicAutoProperty", "Value 2"));

            Assert.AreEqual(4, tca.Count);
            Assert.AreEqual("Value 2", tc.PublicAutoProperty);

            tca.Add(kvp1.Key, kvp1.Value);
            tca.Add(kvp2);
            tca.Add(kvp3);

            Assert.AreEqual(7, tca.Count);
            Assert.AreEqual(kvp1.Value, tca[kvp1.Key]);
            Assert.AreEqual(kvp2.Value, tca[kvp2.Key]);
            Assert.AreEqual(kvp3.Value, tca[kvp3.Key]);
            Assert.IsTrue(tca.ContainsKey(kvp1.Key));
            Assert.IsTrue(tca.Contains(kvp2));
            Assert.IsTrue(tca.ContainsKey(kvp3.Key));

            tca.Remove(kvp1.Key);

            Assert.AreEqual(6, tca.Count);
            Assert.IsFalse(tca.ContainsKey(kvp1.Key));

            tca.Remove(kvp2);

            Assert.AreEqual(5, tca.Count);
            Assert.IsFalse(tca.ContainsKey(kvp2.Key));

            tca.Clear();

            Assert.AreEqual(4, tca.Count);
            Assert.IsFalse(tca.ContainsKey(kvp3.Key));
        }

        [TestMethod]
        public void TestIsReadonly()
        {
            Assert.IsFalse(new Accessor<TestClass>(new TestClass()).IsReadOnly);
            Assert.IsTrue(new Accessor<TestReadonlyClass>(new TestReadonlyClass()).IsReadOnly);
        }

        [TestMethod]
        public void TestSnapshot()
        {
            TestClass tc = new TestClass();
            Accessor<TestClass> tca = tc;

            tc.PublicField = 123;

            Assert.AreEqual(tc.PublicField, tca["PublicField"]);

            IReadOnlyDictionary<string, object> snapshot = tca.Snapshot();

            tc.PublicField = 456;

            Assert.AreEqual(tc.PublicField, tca["PublicField"]);
            Assert.AreNotEqual(tc.PublicField, snapshot["PublicField"]);
            Assert.AreEqual(123, snapshot["PublicField"]);

            tca.Apply(snapshot);

            Assert.AreEqual(snapshot["PublicField"], tc.PublicField);
            Assert.AreEqual(tc.PublicField, tca["PublicField"]);
        }

        [TestMethod]
        public void TestNotIncludingInstance()
        {
            Accessor<TestClass> tca = new Accessor<TestClass>(null);
            Assert.AreEqual(0, tca.Count);

            tca = new Accessor<TestClass>(new TestClass(), includeInstance: false);
            Assert.AreEqual(0, tca.Count);
        }

        [TestMethod]
        public void TestStatics()
        {
            TestStaticsClass.StaticField = true;
            TestStaticsClass.StaticProperty = true;

            Accessor<TestStaticsClass> tca = new Accessor<TestStaticsClass>(new TestStaticsClass());
            Assert.AreEqual(4, tca.Count);

            Assert.AreEqual(false, tca["NonStaticField"]);
            Assert.AreEqual(false, tca["NonStaticProperty"]);
            Assert.AreEqual(true, tca["StaticField"]);
            Assert.AreEqual(true, tca["StaticProperty"]);

            tca = new Accessor<TestStaticsClass>(null);
            Assert.AreEqual(2, tca.Count);

            Assert.AreEqual(true, tca["StaticField"]);
            Assert.AreEqual(true, tca["StaticProperty"]);

            tca["StaticField"] = false;
            tca["StaticProperty"] = false;

            Assert.AreEqual(false, tca["StaticField"]);
            Assert.AreEqual(false, tca["StaticProperty"]);
        }
    }
}