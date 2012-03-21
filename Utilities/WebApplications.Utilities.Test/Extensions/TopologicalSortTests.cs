using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;

namespace WebApplications.Utilities.Test.Extensions
{
    [TestClass]
    public class TopologicalSortTests : TestBase
    {

        //TODO: cyclic dependancies, duplicate elements, duplicate edges, edges referencing unlisted elements

        private class TestReferenceType
        {
            public Guid Name { get; private set; }

            public TestReferenceType()
            {
                // This allows us the user, when debugging, to distinguish between instances of the type
                Name = Guid.NewGuid();
            }

            public override string ToString()
            {
                return String.Format("Test reference type, id:{0}.", Name);
            }
        }

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
            IEnumerable<TestReferenceType> enumerable = Enumerable.Range(1, Random.Next(10, 100)).Select(n => new TestReferenceType()).ToList();
            // Edges are such that each entry is dependent on the entry next in the list
            IEnumerable<KeyValuePair<TestReferenceType, TestReferenceType>> edges =
                enumerable.Skip(1).Zip(enumerable, (a, b) => new KeyValuePair<TestReferenceType, TestReferenceType>(a, b));
            IEnumerable<TestReferenceType> result = enumerable.TopologicalSortEdges(edges);
            Assert.AreEqual(enumerable.Count(), result.Count());
        }

        [TestMethod]
        public void TopologicalSortEdges_SimpleDependancyChainOfReferenceTypes_ResultOrderIsCorrect()
        {
            IEnumerable<TestReferenceType> enumerable = Enumerable.Range(1, Random.Next(10, 100)).Select(n => new TestReferenceType()).ToList();
            // Edges are such that each entry is dependent on the entry next in the list
            IEnumerable<KeyValuePair<TestReferenceType, TestReferenceType>> edges =
                enumerable.Skip(1).Zip(enumerable, (a, b) => new KeyValuePair<TestReferenceType, TestReferenceType>(a, b));
            IEnumerable<TestReferenceType> result = enumerable.TopologicalSortEdges(edges);
            // Correct order for this set of edges is for the output to be the exact reverse of the input
            CollectionAssert.AreEqual(enumerable.Reverse().ToList(), result.ToList());
            }
    
        [TestMethod]
        public void TopologicalSortEdges_NoDependancies_CountMatchesInput()
        {
            IEnumerable<TestReferenceType> enumerable = Enumerable.Range(1, Random.Next(10, 100)).Select(n => new TestReferenceType()).ToList();
            IEnumerable<KeyValuePair<TestReferenceType, TestReferenceType>> edges = new List<KeyValuePair<TestReferenceType, TestReferenceType>>();
            IEnumerable<TestReferenceType> result = enumerable.TopologicalSortEdges(edges);
            Assert.AreEqual(enumerable.Count(), result.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void TopologicalSortEdges_ZeroLengthLoop_ThrowsAInvalidOperationExceptionOnIteration()
        {
            IEnumerable<TestReferenceType> enumerable = Enumerable.Range(1, Random.Next(10, 100)).Select(n => new TestReferenceType()).ToList();
            // Have just one edge which connects the first entry to itself
            IEnumerable<KeyValuePair<TestReferenceType, TestReferenceType>> edges =
                new List<KeyValuePair<TestReferenceType, TestReferenceType>> { new KeyValuePair<TestReferenceType, TestReferenceType>(enumerable.First(), enumerable.First()) };
            IEnumerable<TestReferenceType> result = enumerable.TopologicalSortEdges(edges);
            result = result.ToList();
        }

        [TestMethod]
        public void TopologicalSortEdges_ZeroLengthLoop_DoesNotThrowErrorBeforeLastStep()
        {
            IEnumerable<TestReferenceType> enumerable = Enumerable.Range(1, Random.Next(10, 100)).Select(n => new TestReferenceType()).ToList();
            // Have just one edge which connects the first entry to itself
            IEnumerable<KeyValuePair<TestReferenceType, TestReferenceType>> edges =
                new List<KeyValuePair<TestReferenceType, TestReferenceType>> { new KeyValuePair<TestReferenceType, TestReferenceType>(enumerable.First(), enumerable.First()) };
            IEnumerable<TestReferenceType> result = enumerable.TopologicalSortEdges(edges);
            IEnumerable<TestReferenceType> allButLastStep = result.Take(enumerable.Count() - 1).ToList();
        }

        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void TopologicalSortEdges_TwoEntryLoop_ThrowsAInvalidOperationExceptionOnIteration()
        {
            IEnumerable<TestReferenceType> enumerable = Enumerable.Range(1, Random.Next(10, 100)).Select(n => new TestReferenceType()).ToList();
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
            IEnumerable<TestReferenceType> enumerable = Enumerable.Range(1, Random.Next(10, 100)).Select(n => new TestReferenceType()).ToList();
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
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void TopologicalSortEdges_SimpleLoopingDependancyChain_ThrowsAInvalidOperationExceptionOnFirstStep()
        {
            IEnumerable<TestReferenceType> enumerable = Enumerable.Range(1, Random.Next(10, 100)).Select(n => new TestReferenceType()).ToList();
            // Edges are such that each entry is dependent on the entry next in the list
            List<KeyValuePair<TestReferenceType, TestReferenceType>> edges =
                enumerable.Skip(1).Zip(enumerable, (a, b) => new KeyValuePair<TestReferenceType, TestReferenceType>(a, b)).ToList();
            // Then add loop by connecting first entry to last
            edges.Add(new KeyValuePair<TestReferenceType, TestReferenceType>(enumerable.First(),enumerable.Last()));
            IEnumerable<TestReferenceType> result = enumerable.TopologicalSortEdges(edges);
            TestReferenceType firstStep = result.First();
        }
    }
}
