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
        }

        [TestMethod]
        public void TestCast()
        {
            TestClass tc = new TestClass();
            Accessor<TestClass> tca = tc;
            Assert.AreEqual(tc.PublicReadonlyField, tca["PublicReadonlyField"]);
            Assert.AreEqual(tc.PublicField, tca["PublicField"]);
            Assert.AreEqual(tc.PublicAutoProperty, tca["PublicAutoProperty"]);
            tc.PublicField = 2;
            tc.PublicAutoProperty = "test";
            Assert.AreEqual(tc.PublicField, tca["PublicField"]);
            Assert.AreEqual(tc.PublicAutoProperty, tca["PublicAutoProperty"]);
            tca["PublicField"] = 4;
            tca["PublicAutoProperty"] = "test2";
            Assert.AreEqual(tc.PublicField, 4);
            Assert.AreEqual(tc.PublicAutoProperty, "test2");
            Assert.AreEqual(tc.PublicField, tca["PublicField"]);
            Assert.AreEqual(tc.PublicAutoProperty, tca["PublicAutoProperty"]);
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
        }
    }
}