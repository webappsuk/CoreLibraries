#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities.Test
// File: TestReflection.cs
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebApplications.Utilities.Test
{
    [TestClass]
    public class TupleExtensionsTests
    {


        private List<int> testValues;
        private Tuple<int, String, bool> testTuple;
        private Tuple<int, int, int, int, int, int, int, Tuple<int, int, int, int, int, int, int, Tuple<int, int, int>>> testNestedTuple;
        private Type testTupleType;
        private Type testNestedTupleType;
        private ExtendedTuple<Tuple<int, String, bool>> testExtendedTuple;
        
        // In order to test ExtendedTuple<T> in cases where T is not a tuple, but does fit the criteria for T
        private class MyStructuralClass: IStructuralEquatable, IStructuralComparable, IComparable
        {
            int IComparable.CompareTo(object obj)
            {
                return 0;
            }
            bool IStructuralEquatable.Equals(object obj, IEqualityComparer comparer)
            {
                return false;
            }
            int IStructuralComparable.CompareTo(object obj, IComparer comparer)
            {
                return 0;
            }
            int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
            {
                return 0;
            }
        }

        /// <summary>
        /// Creates two test cases: a triplet with different types at each index; and a tuple filled with nested tuples for the test with the value equal to the index. 
        /// The types of these tuples are also stored to private fields, as are forms of each test case wrapped inside the ExtendedTuple class.
        /// </summary>
        [TestInitialize]
        public void CreateTestCases()
        {
            Random randSource = new Random();
            testValues = new List<int>(Enumerable.Range(1, 17).Select(x => randSource.Next()));
            testTuple = new Tuple<int, String, bool>(testValues[0], testValues[1].ToString(), testValues[2]%2==0 );
            testTupleType = typeof(Tuple<int, String, bool>);
            testNestedTuple = new Tuple
                <int, int, int, int, int, int, int, Tuple<int, int, int, int, int, int, int, Tuple<int, int, int>>>(
                testValues[0], testValues[1], testValues[2], testValues[3], testValues[4], testValues[5], testValues[6], 
                new Tuple<int, int, int, int, int, int, int, Tuple<int, int, int>>(
                    testValues[7], testValues[8], testValues[9], testValues[10], testValues[11], testValues[12], testValues[13], 
                    new Tuple<int, int, int>(testValues[14], testValues[15], testValues[16])));
            testNestedTupleType = typeof(Tuple<int, int, int, int, int, int, int, Tuple<int, int, int, int, int, int, int, Tuple<int, int, int>>>);
            testExtendedTuple = new ExtendedTuple<Tuple<int, String, bool>>(testTuple);
        }

        /// <summary>
        /// Tests the type accessor on a tuple instance.
        /// </summary>
        /// <remarks></remarks>
        [TestMethod]
        public void GetIndexType_TupleInstance_ReturnsTypeForGivenIndex()
        {
            Assert.AreEqual(typeof(int), testTuple.GetIndexType(0), "The GetIndexType tuple extension method should return the type used for a particular index.");
            Assert.AreEqual(typeof(String), testTuple.GetIndexType(1), "The GetIndexType tuple extension method should return the type used for a particular index.");
            Assert.AreEqual(typeof(bool), testTuple.GetIndexType(2), "The GetIndexType tuple extension method should return the type used for a particular index.");
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void GetIndexType_TupleInstanceAndIndexOutOfBounds_ThrowsIndexOutOfRangeException()
        {
            testTuple.GetIndexType(3);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void GetIndexType_TupleInstanceAndIndexNegotive_ThrowsIndexOutOfRangeException()
        {
            testTuple.GetIndexType(-1);
        }

        [TestMethod]
        public void GetTupleItem_TupleInstance_ReturnsValueForGivenIndex()
        {
            Assert.AreEqual(testValues[0], testTuple.GetTupleItem<Tuple<int, String, bool>, int>(0), "The GetTupleItem tuple extension method should return the value at a particular index.");
            Assert.AreEqual(testValues[1].ToString(), testTuple.GetTupleItem<Tuple<int, String, bool>, String>(1), "The GetTupleItem tuple extension method should return the value at a particular index.");
            Assert.AreEqual(testValues[2] % 2 == 0, testTuple.GetTupleItem<Tuple<int, String, bool>, bool>(2), "The GetTupleItem tuple extension method should return the value at a particular index.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void GetTupleItem_TupleInstanceWithInvalidCast_InvalidCastException()
        {
            int value = testTuple.GetTupleItem<Tuple<int, String, bool>, int>(1); // note that the type for index=1 is String, not int
        }

        /// <summary>
        /// Tests the types accessor on a tuple instance.
        /// </summary>
        /// <remarks></remarks>
        [TestMethod]
        public void GetIndexTypes_TupleInstance_ReturnsListWithSameLengthAsTuple()
        {
            Type[] types = testTuple.GetIndexTypes();

            Assert.AreEqual(3, types.Length, "The GetIndexTypes tuple extension method should return a list with a matching length to the tuple.");
        }

        /// <summary>
        /// Tests the types accessor on a tuple type.
        /// </summary>
        /// <remarks></remarks>
        [TestMethod]
        public void GetIndexTypes_TupleInstance_ReturnsListOfTypesUsedInTuple()
        {
            Type[] types = testTuple.GetIndexTypes();

            Assert.AreEqual(typeof(int), types[0], "The GetIndexTypes tuple extension method should return a list where the nth element is the type of the nth element of the tuple.");
            Assert.AreEqual(typeof(String), types[1], "The GetIndexTypes tuple extension method should return a list where the nth element is the type of the nth element of the tuple.");
            Assert.AreEqual(typeof(bool), types[2], "The GetIndexTypes tuple extension method should return a list where the nth element is the type of the nth element of the tuple.");
        }

        /// <summary>
        /// Tests the types accessor on a tuple type.
        /// </summary>
        /// <remarks></remarks>
        [TestMethod]
        public void GetIndexTypes_TupleType_ReturnsListOfTypesUsedInTuple()
        {
            Type[] types = testTupleType.GetIndexTypes();

            Assert.AreEqual(typeof(int), types[0], "The GetIndexTypes tuple extension method should return a list where the nth element is the type of the nth element of the tuple.");
            Assert.AreEqual(typeof(String), types[1], "The GetIndexTypes tuple extension method should return a list where the nth element is the type of the nth element of the tuple.");
            Assert.AreEqual(typeof(bool), types[2], "The GetIndexTypes tuple extension method should return a list where the nth element is the type of the nth element of the tuple.");
        }

        /// <summary>
        /// Tests the tuple indexer.
        /// </summary>
        /// <remarks></remarks>
        [TestMethod]
        public void GetTupleIndexer_TupleInstanceWithNestedTuples_ReturnsLambdaWhichProvidesValueAtRequestedIndex()
        {
            Func<int, object> indexer = testNestedTuple.GetTupleIndexer();
            for (int i = 0; i < testValues.Count; i++)
                Assert.AreEqual(testValues[i], indexer(i), "The GetTupleIndexer extension method should return an lambda function which returns the value at a given index of the tuple. ");
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void GetTupleIndexer_TupleInstanceAndIndexOutOfBounds_ThrowsIndexOutOfBoundsException()
        {
            Func<int, object> indexer = testNestedTuple.GetTupleIndexer();
            indexer(17);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void GetTupleIndexer_TupleInstanceAndIndexNegotive_ThrowsIndexOutOfBoundsException()
        {
            Func<int, object> indexer = testNestedTuple.GetTupleIndexer();
            indexer(-1);
        }

        /// <summary>
        /// Tests retrieving an indexer without an instance of a tuple (by it's type).
        /// </summary>
        /// <remarks></remarks>
        [TestMethod]
        public void GetTupleIndexer_TupleTypeWithNestedTuples_ReturnsLambdaWhichProvidesValueAtRequestedIndexForSuppliedInstance()
        {
            Func<object, int, object> indexer = testNestedTupleType.GetTupleIndexer();

            for (int i = 0; i < testValues.Count; i++)
                Assert.AreEqual(testValues[i], indexer(testNestedTuple, i), "The returned lambda from the GetTupleIndexer extension method should return the values of the tuple instance supplied at the requested index. ");
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void GetTupleIndexer_TupleTypeAndIndexOutOfBounds_ThrowsIndexOutOfBoundsException()
        {
            Func<object, int, object> indexer = testNestedTupleType.GetTupleIndexer();

            indexer(testNestedTuple, testValues.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void GetTupleIndexer_TupleTypeAndIndexNegotive_ThrowsIndexOutOfBoundsException()
        {
            Func<object, int, object> indexer = testNestedTupleType.GetTupleIndexer();

            indexer(testNestedTuple, -1);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void GetTupleIndexer_TupleTypeWithInstanceOfDifferentType_ThrowsInvalidCastException()
        {
            Func<object, int, object> indexer = testTupleType.GetTupleIndexer();

            indexer(testNestedTuple, 0);
        }

        /// <summary>
        /// Tests the tuple iterator.
        /// </summary>
        /// <remarks></remarks>
        [TestMethod]
        public void GetTupleIterator_TupleInstanceWithNestedTuples_IteratesAsThoughAllElementsWereAFlatList()
        {
            int i = 0;
            foreach (object o in testNestedTuple.GetTupleIterator())
                Assert.AreEqual(testValues[i++], (int)o, "The GetTupleIterator extension method should return an iterator which iterates through all values of the tuple. ");
        }

        /// <summary>
        /// Tests retrieving an iterator without an instance of a tuple (by it's type).
        /// </summary>
        /// <remarks></remarks>
        [TestMethod]
        public void GetTupleIterator_TupleTypeWithNestedTuples_IteratesAsThoughAllElementsWereAFlatList()
        {
            Func<object, IEnumerable> iterator = testNestedTupleType.GetTupleIterator();
            int i = 0;
            foreach (object o in iterator(testNestedTuple))
                Assert.AreEqual(testValues[i++], (int)o, "The GetTupleIterator extension method should return an iterator which iterates through all values of the tuple instance supplied. ");
        }

        [TestMethod]
        [ExpectedException(typeof(TypeInitializationException))]
        public void ExtendedTuple_InvalidTupleType_ThrowsTypeInitializationException()
        {
            var testCase = new ExtendedTuple<MyStructuralClass>(new MyStructuralClass());
        }

        [TestMethod]
        public void ExtendedTuple_Indexes_ReturnValueForGivenIndex()
        {
            Assert.AreEqual(testValues[0], testExtendedTuple[0], "The ExtendedTuple wrapper should allow the values of the tuple to be referenced by index.");
            Assert.AreEqual(testValues[1].ToString(), testExtendedTuple[1], "The ExtendedTuple wrapper should allow the values of the tuple to be referenced by index.");
            Assert.AreEqual(testValues[2] % 2 == 0, testExtendedTuple[2], "The ExtendedTuple wrapper should allow the values of the tuple to be referenced by index.");
        }

        [TestMethod]
        public void ExtendedTuple_GetItem_ReturnsValueForGivenIndex()
        {
            Assert.AreEqual(testValues[0], testExtendedTuple.GetItem<int>(0), "The GetItem method of the ExtendedTuple wrapper should return values of the tuple at the requested index.");
            Assert.AreEqual(testValues[1].ToString(), testExtendedTuple.GetItem<String>(1), "The GetItem method of the ExtendedTuple wrapper should return values of the tuple at the requested index.");
            Assert.AreEqual(testValues[2] % 2 == 0, testExtendedTuple.GetItem<bool>(2), "The GetItem method of the ExtendedTuple wrapper should return values of the tuple at the requested index.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void ExtendedTuple_GetItemWithMismatchedType_ThrowsInvalidCastException()
        {
            int value = testExtendedTuple.GetItem<int>(1); // note that the type at index=1 is actually String
        }

        [TestMethod]
        public void BuilderTest()
        {
            Tuple<string, int, int, int, double> a = ExtendedTuple.Create("Hello", 4, 5, 6, 7.0);

            IEnumerable<int> intEnum = new[] {1, 2, 3, 4};

            IEnumerable<Tuple<int, int, string>> tuples =intEnum.ToTuple(c => c, c => c + 1, c=> c.ToString());

            Trace.WriteLine(string.Join(Environment.NewLine, tuples.Select(t => t.ToString())));
        }

        [TestMethod]
        public void ExtendedTuple_TestTupleIndexerWithNullableTypes()
        {
            Tuple<int?, int> t = new Tuple<int?, int>(null,2);
            var et = new ExtendedTuple<Tuple<int?, int>>(t);
            Assert.AreEqual(null, et[0]);
        }
    }
}