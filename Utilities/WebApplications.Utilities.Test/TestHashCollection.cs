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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebApplications.Utilities.Test
{
    [TestClass]
    public class TestHashCollection
    {
        private class Modulo2Comparer : IEqualityComparer<int>
        {
            public bool Equals(int x, int y)
            {
                return GetHashCode(x) == GetHashCode(y);
            }

            public int GetHashCode(int obj)
            {
                return obj % 2;
            }
        }

        [TestMethod]
        public void TestInitialise()
        {
            HashCollection<int> hashCollection = new HashCollection<int> {1};
            Assert.IsTrue(hashCollection.Contains(1));
            Assert.IsFalse(hashCollection.Contains(2));
            Assert.IsFalse(hashCollection.Contains(3));

            hashCollection = new HashCollection<int>(new Modulo2Comparer()) {1};
            Assert.IsTrue(hashCollection.Contains(1));
            Assert.IsFalse(hashCollection.Contains(2));
            Assert.IsTrue(hashCollection.Contains(3));

            hashCollection = new HashCollection<int>(new[] {1}, new Modulo2Comparer());
            Assert.IsTrue(hashCollection.Contains(1));
            Assert.IsFalse(hashCollection.Contains(2));
            Assert.IsTrue(hashCollection.Contains(3));
        }

        [TestMethod]
        public void TestAdd()
        {
            HashCollection<int> hashCollection = new HashCollection<int>();
            Assert.IsTrue(hashCollection.Add(3));
            Assert.IsFalse(hashCollection.Add(3));
        }

        [TestMethod]
        public void TestICollectionCopyTo()
        {
            int[] array = new int[2];
            ICollection collection = new HashCollection<int> {1, 2};
            collection.CopyTo(array, 0);
            Assert.IsTrue(array[0] == 1 || array[0] == 1);
            Assert.IsTrue(array[1] == 2 || array[1] == 2);

            array = new[] {-1, -2, -3, -4, -5};
            collection = new HashCollection<int> {1, 2};
            collection.CopyTo(array, 2);
            Assert.IsTrue(array[2] == 1 || array[3] == 1);
            Assert.IsTrue(array[2] == 2 || array[3] == 2);
            Assert.AreEqual(-1, array[0]);
            Assert.AreEqual(-2, array[1]);
            Assert.AreEqual(-5, array[4]);
        }

        [TestMethod]
        public void TestISetMethods()
        {
            HashCollection<int> hashCollection = new HashCollection<int>();
            ISet set = hashCollection;

            set.Add(1);
            Assert.IsTrue(hashCollection.Contains(1));
            Assert.IsFalse(hashCollection.Contains(2));

            set.UnionWith(new[] {2});
            Assert.IsTrue(hashCollection.Contains(1));
            Assert.IsTrue(hashCollection.Contains(2));

            set.IntersectWith(new[] {2});
            Assert.IsFalse(hashCollection.Contains(1));
            Assert.IsTrue(hashCollection.Contains(2));

            hashCollection.Add(1);
            set.SymmetricExceptWith(new[] {2, 3});
            Assert.IsTrue(hashCollection.Contains(1));
            Assert.IsFalse(hashCollection.Contains(2));
            Assert.IsTrue(hashCollection.Contains(3));

            set.ExceptWith(new[] {2, 3});
            Assert.IsTrue(hashCollection.Contains(1));
            Assert.IsFalse(hashCollection.Contains(2));
            Assert.IsFalse(hashCollection.Contains(3));

            Assert.IsTrue(set.IsSupersetOf(Array<int>.Empty));
            Assert.IsFalse(set.IsSupersetOf(new[] {2}));
            Assert.IsTrue(set.IsSubsetOf(new[] {1, 2}));
            Assert.IsFalse(set.IsSubsetOf(new[] {2}));

            Assert.IsTrue(set.IsProperSupersetOf(Array<int>.Empty));
            Assert.IsFalse(set.IsProperSupersetOf(set));
            Assert.IsTrue(set.IsProperSubsetOf(new[] {1, 2}));
            Assert.IsFalse(set.IsProperSubsetOf(set));

            Assert.IsTrue(set.Overlaps(new[] {1, 2}));
            Assert.IsFalse(set.Overlaps(new[] {3, 4}));
        }

        [TestMethod]
        public void TestIntersect()
        {
            HashCollection<int> a = new HashCollection<int> {1, 2};
            HashCollection<int> b = new HashCollection<int> {2, 3};
            ISet c = a.Intersect(b);
            Assert.IsNotNull(c);
            Assert.IsFalse(c.Contains(1));
            Assert.IsTrue(c.Contains(2));
            Assert.IsFalse(c.Contains(3));
            Assert.IsFalse(c.Contains(4));
        }

        [TestMethod]
        public void TestEquality()
        {
            HashCollection<int> a = new HashCollection<int> {1, 2};
            HashCollection<int> b = new HashCollection<int> {2, 3};
            HashCollection<int> c = new HashCollection<int> {1, 2};

            Assert.IsFalse(a == b);
            Assert.IsTrue(a == c);
            Assert.IsFalse(null == a);
            Assert.IsFalse(a == null);

            Assert.IsTrue(a != b);
            Assert.IsFalse(a != c);
            Assert.IsTrue(null != a);
            Assert.IsTrue(a != null);

            Assert.AreNotEqual(a, b);
            Assert.AreEqual(a, c);
            Assert.AreEqual(a.GetHashCode(), c.GetHashCode());

            Assert.IsFalse(a.Equals(b));
            Assert.IsTrue(a.Equals(c));
            Assert.IsFalse(a.Equals(null));

            Assert.IsFalse(a.Equals((ISet) b));
            Assert.IsTrue(a.Equals((ISet) c));
            Assert.IsFalse(a.Equals((ISet) new HashCollection<int> {1}));
            Assert.IsFalse(a.Equals((ISet) null));

            Assert.IsFalse(a.Equals((IEnumerable) b));
            Assert.IsTrue(a.Equals((IEnumerable) c));
            Assert.IsFalse(a.Equals((IEnumerable) null));

            Assert.IsFalse(a.Equals(new[] {2, 3}));
            Assert.IsTrue(a.Equals(new[] {1, 2}));
            Assert.IsFalse(a.Equals((IEnumerable<int>) null));
        }

        [TestMethod]
        public void TestICollectionSyncMembers()
        {
            ICollection a = new HashCollection<int> {1, 2};
            Assert.AreSame(a, a.SyncRoot);
            Assert.IsFalse(a.IsSynchronized);
        }
    }
}