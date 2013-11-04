using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
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
                new[] {
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
                IGrouping<string, int> grouping = (IGrouping<string, int>)enumerator.Current;
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
    }
}
