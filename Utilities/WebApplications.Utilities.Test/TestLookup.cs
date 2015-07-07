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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WebApplications.Utilities.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebApplications.Utilities.Test
{
    [TestClass]
    public class TestLookup
    {
        [TestMethod]
        public void TestCreateLookup()
        {
            Lookup<string, int> lookup = new Lookup<string, int>(10);
            Assert.AreEqual(0, lookup.Count);
        }

        [TestMethod]
        public void TestGetByIndex()
        {
            Lookup<string, int> lookup = new Lookup<string, int>
            {
                {"one", 1},
                {"two", 2},
                {"one", -1}
            };
            Assert.AreEqual(2, lookup.Count);
            int[] one = lookup["one"].ToArray();
            Assert.IsTrue(one.Contains(1));
            Assert.IsTrue(one.Contains(-1));
            Assert.AreEqual(2, one.Length);
        }

        [TestMethod]
        public void TestContains()
        {
            Lookup<string, int> lookup = new Lookup<string, int>
            {
                {"one", 1},
                {"two", 2},
                {"one", -1}
            };
            Assert.IsTrue(lookup.Contains("one"));
            Assert.IsTrue(lookup.Contains("two"));
            Assert.IsFalse(lookup.Contains("three"));
        }

        [TestMethod]
        public void TestGenericGetEnumerator()
        {
            IEnumerable<IGrouping<string, int>> lookup = new Lookup<string, int>
            {
                {"one", 1},
                {"two", 2},
                {"one", -1}
            };
            AssertContents(lookup.GetEnumerator());
        }

        [TestMethod]
        public void TestNonGenericGetEnumerator()
        {
            IEnumerable lookup = new Lookup<string, int>
            {
                {"one", 1},
                {"two", 2},
                {"one", -1}
            };
            AssertContents(lookup.GetEnumerator());
        }

        [TestMethod]
        public void TestAddKeyValuePair()
        {
            Lookup<string, int> lookup = new Lookup<string, int>
            {
                new KeyValuePair<string, int>("one", 1),
                new KeyValuePair<string, int>("one", -1),
                new KeyValuePair<string, int>("two", 2)
            };
            AssertContents(lookup.GetEnumerator());
        }

        [TestMethod]
        public void TestAddRangeOfKeyValuePairs()
        {
            Lookup<string, int> lookup = new Lookup<string, int>();
            lookup.AddRange(
                new[]
                {
                    new KeyValuePair<string, int>("one", 1),
                    new KeyValuePair<string, int>("one", -1),
                    new KeyValuePair<string, int>("two", 2)
                });
            AssertContents(lookup.GetEnumerator());
        }

        private static void AssertContents([NotNull] IEnumerator enumerator)
        {
            bool foundOne = false;
            bool foundTwo = false;
            while (enumerator.MoveNext())
            {
                IGrouping<string, int> grouping = (IGrouping<string, int>) enumerator.Current;
                Assert.IsNotNull(grouping);
                switch (grouping.Key)
                {
                    case "one":
                        Assert.IsFalse(foundOne, "1 appears twice.");
                        int[] one = grouping.ToArray();
                        Assert.IsTrue(one.Contains(1));
                        Assert.IsTrue(one.Contains(-1));
                        Assert.AreEqual(2, one.Length);
                        foundOne = true;
                        break;
                    case "two":
                        Assert.IsFalse(foundTwo, "2 appears twice.");
                        Assert.AreEqual(2, grouping.Single());
                        foundTwo = true;
                        break;
                    default:
                        Assert.Fail("Unexpected group key found.");
                        break;
                }
            }
            Assert.IsTrue(foundOne, "Did not enumerate 'one'.");
            Assert.IsTrue(foundTwo, "Did not enumerate 'two'.");
        }

        [TestMethod]
        public void TestGroupingNonGenericEnumerator()
        {
            Lookup<string, int> lookup = new Lookup<string, int>
            {
                {"one", 1},
                {"one", 2}
            };
            IEnumerable grouping = lookup.Single();
            Assert.IsNotNull(grouping);
            bool foundOne = false;
            bool foundTwo = false;
            foreach (int i in grouping)
                switch (i)
                {
                    case 1:
                        Assert.IsFalse(foundOne, "1 appears twice.");
                        foundOne = true;
                        break;
                    case 2:
                        Assert.IsFalse(foundTwo, "2 appears twice.");
                        foundTwo = true;
                        break;
                    default:
                        Assert.Fail("Unexpected value found.");
                        break;
                }
            Assert.IsTrue(foundOne, "Did not enumerate 'one'.");
            Assert.IsTrue(foundTwo, "Did not enumerate 'two'.");
        }

        [TestMethod]
        public void TestRemoveKey()
        {
            Lookup<string, int> lookup = new Lookup<string, int>
            {
                {"one", 1},
                {"one", 2}
            };

            Assert.IsFalse(lookup.Remove("two"));
            Assert.AreEqual(2, lookup.ValuesCount);
            Assert.AreEqual(2, lookup["one"].Count());

            Assert.IsTrue(lookup.Remove("one"));
            Assert.AreEqual(0, lookup.ValuesCount);
            Assert.IsFalse(lookup.Contains("one"));
        }

        [TestMethod]
        public void TestRemoveElement()
        {
            Lookup<string, int> lookup = new Lookup<string, int>
            {
                {"one", 1},
                {"one", 2}
            };

            Assert.IsFalse(lookup.Remove("one", 0));
            Assert.AreEqual(2, lookup.ValuesCount);
            Assert.AreEqual(2, lookup["one"].Count());

            Assert.IsTrue(lookup.Remove("one", 1));
            Assert.AreEqual(1, lookup.ValuesCount);
            Assert.AreEqual(1, lookup["one"].Count());

            Assert.IsTrue(lookup.Remove("one", 2));
            Assert.AreEqual(0, lookup.ValuesCount);
            Assert.IsFalse(lookup.Contains("one"));
        }
    }
}