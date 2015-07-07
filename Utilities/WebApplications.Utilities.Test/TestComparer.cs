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