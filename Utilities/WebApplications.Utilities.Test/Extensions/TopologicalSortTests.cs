#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Copyright (c) 2012, Web Applications UK Ltd
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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebApplications.Utilities.Test.Extensions
{
    [TestClass]
    public class TopologicalSortTests : UtilitiesTestBase
    {
        //TODO: cyclic dependancies, duplicate elements, duplicate edges, edges referencing unlisted elements

        [TestMethod]
        public void TopologicalSortEdges_SimpleDependancyChainOfValueTypes_CountMatchesInput()
        {
            IEnumerable<int> enumerable = Enumerable.Range(1, Random.Next(10, 100)).ToList();
            // Edges are such that each number is dependent on the number higher than it
            IEnumerable<KeyValuePair<int, int>> edges =
                enumerable.Skip(1).Select(n => new KeyValuePair<int, int>(n, n - 1));
            IEnumerable<int> result = enumerable.TopologicalSortEdges(edges);
            Assert.AreEqual(enumerable.Count(), result.Count());
        }

        [TestMethod]
        public void TopologicalSortEdges_SimpleDependancyChainOfValueTypes_ResultOrderIsCorrect()
        {
            IEnumerable<int> enumerable = Enumerable.Range(1, Random.Next(10, 100)).ToList();
            // Edges are such that each number is dependent on the number higher than it
            IEnumerable<KeyValuePair<int, int>> edges =
                enumerable.Skip(1).Select(n => new KeyValuePair<int, int>(n, n - 1));
            IEnumerable<int> result = enumerable.TopologicalSortEdges(edges);
            // Correct order for this set of edges is for the output to be the exact reverse of the input
            CollectionAssert.AreEqual(enumerable.Reverse().ToList(), result.ToList());
        }

        [TestMethod]
        public void TopologicalSortEdges_SimpleDependancyChainOfReferenceTypes_CountMatchesInput()
        {
            IEnumerable<TestReferenceType> enumerable =
                Enumerable.Range(1, Random.Next(10, 100)).Select(n => new TestReferenceType()).ToList();
            // Edges are such that each entry is dependent on the entry next in the list
            IEnumerable<KeyValuePair<TestReferenceType, TestReferenceType>> edges =
                enumerable.Skip(1).Zip(enumerable,
                                       (a, b) => new KeyValuePair<TestReferenceType, TestReferenceType>(a, b));
            IEnumerable<TestReferenceType> result = enumerable.TopologicalSortEdges(edges);
            Assert.AreEqual(enumerable.Count(), result.Count());
        }

        [TestMethod]
        public void TopologicalSortEdges_SimpleDependancyChainOfReferenceTypes_ResultOrderIsCorrect()
        {
            IEnumerable<TestReferenceType> enumerable =
                Enumerable.Range(1, Random.Next(10, 100)).Select(n => new TestReferenceType()).ToList();
            // Edges are such that each entry is dependent on the entry next in the list
            IEnumerable<KeyValuePair<TestReferenceType, TestReferenceType>> edges =
                enumerable.Skip(1).Zip(enumerable,
                                       (a, b) => new KeyValuePair<TestReferenceType, TestReferenceType>(a, b));
            IEnumerable<TestReferenceType> result = enumerable.TopologicalSortEdges(edges);
            // Correct order for this set of edges is for the output to be the exact reverse of the input
            CollectionAssert.AreEqual(enumerable.Reverse().ToList(), result.ToList());
        }

        [TestMethod]
        public void TopologicalSortEdges_NoDependancies_CountMatchesInput()
        {
            IEnumerable<TestReferenceType> enumerable =
                Enumerable.Range(1, Random.Next(10, 100)).Select(n => new TestReferenceType()).ToList();
            IEnumerable<KeyValuePair<TestReferenceType, TestReferenceType>> edges =
                new List<KeyValuePair<TestReferenceType, TestReferenceType>>();
            IEnumerable<TestReferenceType> result = enumerable.TopologicalSortEdges(edges);
            Assert.AreEqual(enumerable.Count(), result.Count());
        }

        [TestMethod]
        public void TopologicalSortEdges_NoDependancies_IsStable()
        {
            IEnumerable<TestReferenceType> enumerable =
                Enumerable.Range(1, Random.Next(10, 100)).Select(n => new TestReferenceType()).ToList();
            IEnumerable<KeyValuePair<TestReferenceType, TestReferenceType>> edges =
                new List<KeyValuePair<TestReferenceType, TestReferenceType>>();
            IEnumerable<TestReferenceType> result = enumerable.TopologicalSortEdges(edges);
            CollectionAssert.AreEqual(enumerable.ToList(), result.ToList());
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void TopologicalSortEdges_ZeroLengthLoop_ThrowsAInvalidOperationExceptionOnIteration()
        {
            IEnumerable<TestReferenceType> enumerable =
                Enumerable.Range(1, Random.Next(10, 100)).Select(n => new TestReferenceType()).ToList();
            // Have just one edge which connects the first entry to itself
            IEnumerable<KeyValuePair<TestReferenceType, TestReferenceType>> edges =
                new List<KeyValuePair<TestReferenceType, TestReferenceType>>
                    {new KeyValuePair<TestReferenceType, TestReferenceType>(enumerable.First(), enumerable.First())};
            IEnumerable<TestReferenceType> result = enumerable.TopologicalSortEdges(edges);
            result = result.ToList();
        }

        [TestMethod]
        public void TopologicalSortEdges_ZeroLengthLoop_DoesNotThrowErrorBeforeLastStep()
        {
            IEnumerable<TestReferenceType> enumerable =
                Enumerable.Range(1, Random.Next(10, 100)).Select(n => new TestReferenceType()).ToList();
            // Have just one edge which connects the first entry to itself
            IEnumerable<KeyValuePair<TestReferenceType, TestReferenceType>> edges =
                new List<KeyValuePair<TestReferenceType, TestReferenceType>>
                    {new KeyValuePair<TestReferenceType, TestReferenceType>(enumerable.First(), enumerable.First())};
            IEnumerable<TestReferenceType> result = enumerable.TopologicalSortEdges(edges);
            IEnumerable<TestReferenceType> allButLastStep = result.Take(enumerable.Count() - 1).ToList();
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void TopologicalSortEdges_TwoEntryLoop_ThrowsAInvalidOperationExceptionOnIteration()
        {
            IEnumerable<TestReferenceType> enumerable =
                Enumerable.Range(1, Random.Next(10, 100)).Select(n => new TestReferenceType()).ToList();
            // Have just two edges which connects the first and last entries to each other
            IEnumerable<KeyValuePair<TestReferenceType, TestReferenceType>> edges =
                new List<KeyValuePair<TestReferenceType, TestReferenceType>>
                    {
                        new KeyValuePair<TestReferenceType, TestReferenceType>(enumerable.First(), enumerable.Last()),
                        new KeyValuePair<TestReferenceType, TestReferenceType>(enumerable.Last(), enumerable.First())
                    };
            IEnumerable<TestReferenceType> result = enumerable.TopologicalSortEdges(edges);
            result = result.ToList();
        }

        [TestMethod]
        public void TopologicalSortEdges_TwoEntryLoop_DoesNotThrowErrorBeforeLastTwoSteps()
        {
            IEnumerable<TestReferenceType> enumerable =
                Enumerable.Range(1, Random.Next(10, 100)).Select(n => new TestReferenceType()).ToList();
            // Have just two edges which connects the first and last entries to each other
            IEnumerable<KeyValuePair<TestReferenceType, TestReferenceType>> edges =
                new List<KeyValuePair<TestReferenceType, TestReferenceType>>
                    {
                        new KeyValuePair<TestReferenceType, TestReferenceType>(enumerable.First(), enumerable.Last()),
                        new KeyValuePair<TestReferenceType, TestReferenceType>(enumerable.Last(), enumerable.First())
                    };
            IEnumerable<TestReferenceType> result = enumerable.TopologicalSortEdges(edges);
            IEnumerable<TestReferenceType> allButLastTwoSteps = result.Take(enumerable.Count() - 2).ToList();
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void TopologicalSortEdges_SimpleLoopingDependancyChain_ThrowsAInvalidOperationExceptionOnFirstStep()
        {
            IEnumerable<TestReferenceType> enumerable =
                Enumerable.Range(1, Random.Next(10, 100)).Select(n => new TestReferenceType()).ToList();
            // Edges are such that each entry is dependent on the entry next in the list
            List<KeyValuePair<TestReferenceType, TestReferenceType>> edges =
                enumerable.Skip(1).Zip(enumerable,
                                       (a, b) => new KeyValuePair<TestReferenceType, TestReferenceType>(a, b)).ToList();
            // Then add loop by connecting first entry to last
            edges.Add(new KeyValuePair<TestReferenceType, TestReferenceType>(enumerable.First(), enumerable.Last()));
            IEnumerable<TestReferenceType> result = enumerable.TopologicalSortEdges(edges);
            TestReferenceType firstStep = result.First();
        }

        [TestMethod]
        public void TopologicalSortEdges_SimpleDependancyChainWithDuplicateEdges_CountMatchesInput()
        {
            IEnumerable<TestReferenceType> enumerable =
                Enumerable.Range(1, Random.Next(10, 100)).Select(n => new TestReferenceType()).ToList();
            // Edges are such that each entry is dependent on the entry next in the list
            List<KeyValuePair<TestReferenceType, TestReferenceType>> edges =
                enumerable.Skip(1).Zip(enumerable,
                                       (a, b) => new KeyValuePair<TestReferenceType, TestReferenceType>(a, b)).ToList();
            // Introduce duplicate edge
            edges.Add(edges[Random.Next(0, edges.Count)]);
            IEnumerable<TestReferenceType> result = enumerable.TopologicalSortEdges(edges);
            Assert.AreEqual(enumerable.Count(), result.Count());
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentException))]
        public void TopologicalSortEdges_SimpleDependancyChainWithDuplicateItems_ThrowsArgumentException()
        {
            List<TestReferenceType> items =
                Enumerable.Range(1, Random.Next(10, 100)).Select(n => new TestReferenceType()).ToList();
            // Edges are such that each entry is dependent on the entry next in the list
            List<KeyValuePair<TestReferenceType, TestReferenceType>> edges =
                items.Skip(1).Zip(items, (a, b) => new KeyValuePair<TestReferenceType, TestReferenceType>(a, b)).ToList();
            // Introduce duplicate item
            items.Add(items[Random.Next(0, items.Count)]);
            IEnumerable<TestReferenceType> result = items.TopologicalSortEdges(edges);
            TestReferenceType firstStep = result.First();
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void
            TopologicalSortEdges_SimpleDependancyChainWithMissingDependancy_ThrowsAInvalidOperationExceptionOnIteration()
        {
            IEnumerable<TestReferenceType> enumerable =
                Enumerable.Range(1, Random.Next(10, 100)).Select(n => new TestReferenceType()).ToList();
            // Edges are such that each entry is dependent on the entry next in the list
            List<KeyValuePair<TestReferenceType, TestReferenceType>> edges =
                enumerable.Skip(1).Zip(enumerable,
                                       (a, b) => new KeyValuePair<TestReferenceType, TestReferenceType>(a, b)).ToList();
            // Then add edge making last item depend on an item which is not in the list
            edges.Add(new KeyValuePair<TestReferenceType, TestReferenceType>(new TestReferenceType(), enumerable.Last()));
            IEnumerable<TestReferenceType> result = enumerable.TopologicalSortEdges(edges).ToList();
        }

        [TestMethod]
        public void TopologicalSortEdges_SimpleDependancyChainWithMissingDependant_OutputCountMatchesInput()
        {
            IEnumerable<TestReferenceType> enumerable =
                Enumerable.Range(1, Random.Next(10, 100)).Select(n => new TestReferenceType()).ToList();
            // Edges are such that each entry is dependent on the entry next in the list
            List<KeyValuePair<TestReferenceType, TestReferenceType>> edges =
                enumerable.Skip(1).Zip(enumerable,
                                       (a, b) => new KeyValuePair<TestReferenceType, TestReferenceType>(a, b)).ToList();
            // Then add edge makeing last item depended on by an item which is not in the list
            edges.Add(new KeyValuePair<TestReferenceType, TestReferenceType>(enumerable.Last(), new TestReferenceType()));
            IEnumerable<TestReferenceType> result = enumerable.TopologicalSortEdges(edges);
            Assert.AreEqual(enumerable.Count(), result.Count());
        }

        [TestMethod]
        [ExpectedException(typeof (NullReferenceException))]
        public void TopologicalSortDependants_CallbackReturnsNull_ThrowsNullReferenceException()
        {
            IEnumerable<int> enumerable = Enumerable.Range(1, Random.Next(10, 100)).ToList();
            IEnumerable<int> result = enumerable.Reverse().TopologicalSortDependants(n => null).ToList();
        }

        [TestMethod]
        public void TopologicalSortDependants_SimpleDependancyChain_ResultOrderIsCorrect()
        {
            IEnumerable<int> enumerable = Enumerable.Range(1, Random.Next(10, 100)).ToList();
            IEnumerable<int> result =
                enumerable.TopologicalSortDependants(n => n > 1 ? new List<int> {n - 1} : new List<int>());
            // Correct order is reverse of original sequence
            CollectionAssert.AreEqual(enumerable.Reverse().ToList(), result.ToList());
        }

        [TestMethod]
        public void TopologicalSortDependencies_SimpleDependancyChain_ResultOrderIsCorrect()
        {
            IEnumerable<int> enumerable = Enumerable.Range(1, Random.Next(10, 100)).ToList();
            // reverse is added just to prove sorting actually occurs, as original order is the correct solution
            IEnumerable<int> result =
                enumerable.Reverse().TopologicalSortDependencies(n => n > 1 ? new List<int> {n - 1} : new List<int>());
            CollectionAssert.AreEqual(enumerable.ToList(), result.ToList());
        }

        [TestMethod]
        public void TopologicalSortDependencies_BinaryDependancyTree_EachItemComesAfterItsDependency()
        {
            IEnumerable<int> enumerable = Enumerable.Range(1, Random.Next(10, 100)).ToList();
            // reverse is added just to prove sorting actually occurs, as original order is a correct solution
            List<int> result =
                enumerable.Reverse().TopologicalSortDependencies(n => n > 1 ? new List<int> {n/2} : new List<int>()).
                    ToList();
            Assert.IsTrue(result.Select((i, n) => i > result.IndexOf(n/2)).All(b => b));
        }

        [TestMethod]
        public void TopologicalSortDependants_BinaryDependancyTree_EachItemComesAfterItsDependency()
        {
            IEnumerable<int> enumerable = Enumerable.Range(1, Random.Next(10, 100)).ToList();
            // reverse is added just to prove sorting actually occurs, as original order is a correct solution
            List<int> result =
                enumerable.Reverse().TopologicalSortDependants(
                    n => Enumerable.Range(0, 2).Select(i => n*2 + i).Where(m => m <= enumerable.Count())).ToList();
            Assert.IsTrue(result.Select((i, n) => i > result.IndexOf(n/2)).All(b => b));
        }

        [TestMethod]
        public void
            TopologicalSortDependants_TwoTiersWithAllItemsOnTopDependantOnAllItemsOnBottom_AllTopItemsComeAfterAllBottomItems
            ()
        {
            List<TestReferenceType> top =
                Enumerable.Range(1, Random.Next(10, 100)).Select(n => new TestReferenceType()).ToList();
            List<TestReferenceType> bottom =
                Enumerable.Range(1, Random.Next(10, 100)).Select(n => new TestReferenceType()).ToList();
            List<TestReferenceType> result =
                top.Concat(bottom).TopologicalSortDependants(
                    item => bottom.Contains(item) ? top : new List<TestReferenceType>()).ToList();
            CollectionAssert.AreEquivalent(result.Take(bottom.Count()).ToList(), bottom);
            CollectionAssert.AreEquivalent(result.Skip(bottom.Count()).ToList(), top);
        }

        [TestMethod]
        public void
            TopologicalSortDependencies_TwoTiersWithAllItemsOnTopDependantOnAllItemsOnBottom_AllTopItemsComeAfterAllBottomItems
            ()
        {
            List<TestReferenceType> top =
                Enumerable.Range(1, Random.Next(10, 100)).Select(n => new TestReferenceType()).ToList();
            List<TestReferenceType> bottom =
                Enumerable.Range(1, Random.Next(10, 100)).Select(n => new TestReferenceType()).ToList();
            List<TestReferenceType> result =
                top.Concat(bottom).TopologicalSortDependencies(
                    item => top.Contains(item) ? bottom : new List<TestReferenceType>()).ToList();
            CollectionAssert.AreEquivalent(result.Take(bottom.Count()).ToList(), bottom);
            CollectionAssert.AreEquivalent(result.Skip(bottom.Count()).ToList(), top);
        }

        [TestMethod]
        public void TopologicalSortDependencies_TwoTiersWithAllItemsOnTopDependantOnAllItemsOnBottom_IsStable()
        {
            // Same test as TopologicalSortDependencies_TwoTiersWithAllItemsOnTopDependantOnAllItemsOnBottom_AllTopItemsComeAfterAllBottomItems but also asserts result is stable
            List<TestReferenceType> top =
                Enumerable.Range(1, Random.Next(10, 100)).Select(n => new TestReferenceType()).ToList();
            List<TestReferenceType> bottom =
                Enumerable.Range(1, Random.Next(10, 100)).Select(n => new TestReferenceType()).ToList();
            List<TestReferenceType> result =
                top.Concat(bottom).TopologicalSortDependencies(
                    item => top.Contains(item) ? bottom : new List<TestReferenceType>()).ToList();
            CollectionAssert.AreEqual(result.Take(bottom.Count()).ToList(), bottom);
            CollectionAssert.AreEqual(result.Skip(bottom.Count()).ToList(), top);
        }

        #region Nested type: TestReferenceType
        private class TestReferenceType
        {
            public TestReferenceType()
            {
                // This allows us the user, when debugging, to distinguish between instances of the type
                Name = Guid.NewGuid();
            }

            public Guid Name { get; private set; }

            public override string ToString()
            {
                return String.Format("Test reference type, id:{0}.", Name);
            }
        }
        #endregion
    }
}