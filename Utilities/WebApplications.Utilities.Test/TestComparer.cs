#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities.Test
// File: TestComparer.cs
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

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebApplications.Utilities.Test
{
    [TestClass]
    public class TestComparer
    {
        [TestMethod]
        public void TestEqualityBuilder()
        {
            IEnumerable<Tester> a = new List<Tester>
                                        {
                                            new Tester(1, 2),
                                            new Tester(3, 4),
                                            new Tester(4, 10),
                                            new Tester(10, 12),
                                            new Tester(14, 432)
                                        };
            IEnumerable<Tester> b = new List<Tester>
                                        {
                                            new Tester(1, 2),
                                            new Tester(3, 4),
                                            new Tester(4, 10),
                                            new Tester(10, 12),
                                            new Tester(14, 432)
                                        };

            // Demonstrate that standard method returns false as references are different
            Assert.IsFalse(a.SequenceEqual(b));

            // Demonstrate the using an equality builder allows deeper comparison
            Assert.IsTrue(a.SequenceEqual(b, new EqualityBuilder<Tester>((x, y) => x.A == y.A && x.B == y.B)));
        }

        [TestMethod]
        public void TestComparerBuilder()
        {
            // Create two identical lists, save for the order.
            IEnumerable<Tester> a = new List<Tester>
                                        {
                                            new Tester(1, 2),
                                            new Tester(14, 432),
                                            new Tester(4, 10),
                                            new Tester(3, 4),
                                            new Tester(10, 12)
                                        };
            IEnumerable<Tester> b = new List<Tester>
                                        {
                                            new Tester(1, 2),
                                            new Tester(3, 4),
                                            new Tester(4, 10),
                                            new Tester(10, 12),
                                            new Tester(14, 432)
                                        };

            // Create comparer that compares A then B
            ComparerBuilder<Tester> comparer =
                new ComparerBuilder<Tester>((x, y) => (x.A != y.A) ? x.B.CompareTo(y.B) : x.A.CompareTo(y.A));

            Assert.IsFalse(b.SequenceEqual(a, comparer));

            // Sort using the comparer
            IOrderedEnumerable<Tester> c = a.OrderBy(t => t, comparer);

            // Now check sequence equality again
            Assert.IsTrue(b.SequenceEqual(c, comparer));
        }

        #region Nested type: Tester
        public class Tester
        {
            public readonly int A;
            public readonly int B;

            public Tester(int a, int b)
            {
                A = a;
                B = b;
            }
        }
        #endregion
    }
}