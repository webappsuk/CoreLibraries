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
using Moq;
using WebApplications.Testing;

namespace WebApplications.Utilities.Test.Extensions
{
    [TestClass]
    public class GetObjectsByIdTests : UtilitiesTestBase
    {
        // This value should be in WebApplications.Utilities.Extensions.DefaultSplitChars (which sadly is not a compile time constant)
        private const String DefaultIdSeparator = ",";

        /// <summary>
        /// Creates a string representing the list, separated by the chosen separator.
        /// </summary>
        /// <typeparam name="T">Type of entries in the list.</typeparam>
        /// <param name="list">The list to convert.</param>
        /// <param name="separator">The string used to separate items in the list.</param>
        /// <returns>A string representing the list.</returns>
        private static String CreateListOfIds<T>(IEnumerable<T> list, String separator = DefaultIdSeparator)
        {
            return String.Join(separator, list);
        }

        /// <summary>
        /// Creates the comma seperated list with at least one 'bad value': a value which cannot represent a number.
        /// The number of valid items in the list will be preserved.
        /// </summary>
        /// <typeparam name="T">Type of entries in the list.</typeparam>
        /// <param name="list">The list to convert and corrupt.</param>
        /// <returns>A string representing the list, but with at least one non-numeric entry.</returns>
        private static String CreateCommaSeperatedListWithBadValue<T>(IList<T> list)
        {
            String listString = CreateListOfIds(list);
            int breakpoint = Random.Next(0, list.Count());
            return CreateListOfIds(list.Take(breakpoint)) + ",NaN," + CreateListOfIds(list.Skip(breakpoint));
        }

        /// <summary>
        /// Returns the list with at least one duplicate value included.
        /// </summary>
        private static List<T> AddDuplicatesToList<T>(IList<T> source)
        {
            int size = Random.Next(source.Count + 1, source.Count*2);
            List<T> output = new List<T>(size);
            for (int i = 0; i < size; i++)
            {
                output.Add(source[Random.Next(0, source.Count)]);
            }
            return output;
        }

        #region GetObjectsById
        /// <summary>
        /// Creates a distinct random list of integers, each in the range [0,int.MaxValue).
        /// </summary>
        private static List<int> CreateRandomIdList(int maxId = int.MaxValue)
        {
            return Enumerable.Range(1, Random.Next(5, 20)).Select(n => Random.Next(0, maxId)).Distinct().ToList();
        }

        [ExpectedException(typeof (NullReferenceException))]
        [TestMethod]
        public void GetObjectsById_NullList_ThrowsNullReferenceException()
        {
            ((string) null).GetObjectsById(i => i);
        }

        [TestMethod]
        public void GetObjectsById_ValidList_OutputCountSameAsInput()
        {
            List<int> list = CreateRandomIdList();
            String csvList = CreateListOfIds(list);
            IEnumerable<int> resultEnumeration = csvList.GetObjectsById(n => n);
            Assert.AreEqual(list.Count, resultEnumeration.Count(),
                            "The number of items returned by GetObjectsById should be equal to the number of distinct valid items in the comma-seperated input.");
        }

        [TestMethod]
        public void GetObjectsById_ValidListContainingWhitspace_OutputCountSameAsInput()
        {
            List<int> list = CreateRandomIdList();
            String csvList = CreateListOfIds(list, DefaultIdSeparator + " ");
            IEnumerable<int> resultEnumeration = csvList.GetObjectsById(n => n);
            Assert.AreEqual(list.Count, resultEnumeration.Count(),
                            "The number of items returned by GetObjectsById should be equal to the number of distinct valid items in the comma-seperated input.");
        }

        [TestMethod]
        public void GetObjectsById_ValidListAndCustomSeparator_OutputCountSameAsInput()
        {
            String separator;
            int num;
            do
            {
                separator = Random.RandomString(1);
            } while (int.TryParse(separator, out num));
            List<int> list = CreateRandomIdList();
            String csvList = CreateListOfIds(list, separator);
            IEnumerable<int> resultEnumeration = csvList.GetObjectsById(n => n, new[] {separator[0]});
            Assert.AreEqual(list.Count, resultEnumeration.Count(),
                            "The number of items returned by GetObjectsById should be equal to the number of distinct valid items in the input, separated by the specified separator.");
        }

        [TestMethod]
        public void GetObjectsById_ListWithDuplicates_OutputCountSameAsDistinctInput()
        {
            List<int> listWithDuplicates = AddDuplicatesToList(CreateRandomIdList());
            String csvListWithDuplicates = CreateListOfIds(listWithDuplicates);
            IEnumerable<int> resultEnumeration = csvListWithDuplicates.GetObjectsById(n => n);
            Assert.AreEqual(listWithDuplicates.Distinct().Count(), resultEnumeration.Count(),
                            "The number of items returned by GetObjectsById should be equal to the number of distinct valid items in the comma-seperated input.");
        }

        [TestMethod]
        public void GetObjectsById_ListWithDuplicatesWithoutExecuteImmediately_getObjectFuncNotCalledFromFunction()
        {
            Mock<ITest<int, int>> mock = new Mock<ITest<int, int>>();
            mock.Setup(m => m.Function(It.IsAny<int>())).Returns<int>(n => n);

            List<int> listWithDuplicates = AddDuplicatesToList(CreateRandomIdList());
            String csvListWithDuplicates = CreateListOfIds(listWithDuplicates);
            IEnumerable<int> resultEnumeration = csvListWithDuplicates.GetObjectsById(mock.Object.Function);
            Assert.IsNotNull(resultEnumeration);
            mock.Verify(m => m.Function(It.IsAny<int>()), Times.Never(),
                        "Unless the executeImmediately argument is true, the getObjectFunction should not be called until the result is enumerated through.");
        }

        [TestMethod]
        public void GetObjectsById_ListWithDuplicatesAndExecuteImmediately_getObjectFuncCalledOncePerDistinctInput()
        {
            Mock<ITest<int, int>> mock = new Mock<ITest<int, int>>();
            mock.Setup(m => m.Function(It.IsAny<int>())).Returns<int>(n => n);

            List<int> listWithDuplicates = AddDuplicatesToList(CreateRandomIdList());
            String csvListWithDuplicates = CreateListOfIds(listWithDuplicates);
            IEnumerable<int> resultEnumeration = csvListWithDuplicates.GetObjectsById(mock.Object.Function,
                                                                                      executeImmediately: true);
            Assert.IsNotNull(resultEnumeration);
            mock.Verify(m => m.Function(It.IsAny<int>()), Times.Exactly(listWithDuplicates.Distinct().Count()),
                        "The getObject function supplied to GetObjectsById should be called once for each of the distinct valid items in the comma-seperated input.");
        }

        [TestMethod]
        public void GetObjectsById_ListWithInvalidEntriesAndExecuteImmediately_getObjectFuncCalledOncePerDistinctInput()
        {
            Mock<ITest<int, int>> mock = new Mock<ITest<int, int>>();
            mock.Setup(m => m.Function(It.IsAny<int>())).Returns<int>(n => n);

            List<int> list = CreateRandomIdList();
            String csvListWithBadValue = CreateCommaSeperatedListWithBadValue(list);
            IEnumerable<int> resultEnumeration = csvListWithBadValue.GetObjectsById(mock.Object.Function,
                                                                                    executeImmediately: true);
            Assert.IsNotNull(resultEnumeration);
            mock.Verify(m => m.Function(It.IsAny<int>()), Times.Exactly(list.Count()),
                        "The getObject function supplied to GetObjectsById should be called once for each of the distinct valid items in the comma-seperated input.");
        }

        [TestMethod]
        public void GetObjectsById_ListWithInvalidEntries_OutputCountSameAsNumberOfValidValuesInInput()
        {
            List<int> list = CreateRandomIdList();
            String csvListWithBadValue = CreateCommaSeperatedListWithBadValue(list);
            IEnumerable<int> resultEnumeration = csvListWithBadValue.GetObjectsById(n => n);
            Assert.AreEqual(list.Count(), resultEnumeration.Count(),
                            "The number of items returned by GetObjectsById should be equal to the number of distinct valid items in the comma-seperated input.");
        }

        [TestMethod]
        public void GetObjectsById_getObjectFunctionReturnsNullableType_OutputCanContainNullValues()
        {
            List<int> list = CreateRandomIdList();
            String csvList = CreateListOfIds(list);
            IEnumerable<int?> resultEnumeration = csvList.GetObjectsById<int?>(n => null);
            Assert.IsTrue(resultEnumeration.Contains(null),
                          "When the result of the getObject function is a nullable value type, null values should not be removed from the final output.");
        }

        [TestMethod]
        public void GetObjectsById_getObjectFunctionReturnsNull_OutputDoesNotContainNullValues()
        {
            List<int> list = CreateRandomIdList();
            String csvList = CreateListOfIds(list);
            IEnumerable<Object> resultEnumeration = csvList.GetObjectsById<Object>(n => null);
            Assert.IsFalse(resultEnumeration.Contains(null),
                           "When the result of the getObject function is null, these null values should be removed from the final output.");
        }

        [TestMethod]
        public void GetObjectsById_getObjectFunctionReturnsNullOccasionally_OutputCountMatchesNumberOfNonNullValues()
        {
            List<int> evenNumbers = CreateRandomIdList(int.MaxValue/2).Select(n => n*2).ToList();
            List<int> oddNumbers = CreateRandomIdList(int.MaxValue/3).Select(n => n*2 + 1).ToList();
            String csvList = CreateListOfIds(evenNumbers.Concat(oddNumbers));
            // The getObject function used here will return null for everything in the evenNumbers list, but non-null for all oddNumbers
            IEnumerable<Object> resultEnumeration = csvList.GetObjectsById(n => n%2 == 0 ? null : (Object) n);
            Assert.AreEqual(oddNumbers.Count, resultEnumeration.Count(),
                            "When the result of the getObject function can be null, any null values should be removed from the final output.");
        }
        #endregion

        #region GetObjectsById16
        /// <summary>
        /// Creates a distinct random list of integers, each in the range [0,short.MaxValue).
        /// </summary>
        private static List<short> CreateRandomId16List(short maxId = short.MaxValue)
        {
            return
                Enumerable.Range(1, Random.Next(5, 20)).Select(n => (short) Random.Next(0, maxId)).Distinct().ToList();
        }

        [ExpectedException(typeof (NullReferenceException))]
        [TestMethod]
        public void GetObjectsById16_NullList_ThrowsNullReferenceException()
        {
            ((string) null).GetObjectsById16(i => i);
        }

        [TestMethod]
        public void GetObjectsById16_ValidList_OutputCountSameAsInput()
        {
            List<short> list = CreateRandomId16List();
            String csvList = CreateListOfIds(list);
            IEnumerable<short> resultEnumeration = csvList.GetObjectsById16(n => n);
            Assert.AreEqual(list.Count, resultEnumeration.Count(),
                            "The number of items returned by GetObjectsById16 should be equal to the number of distinct valid items in the comma-seperated input.");
        }

        [TestMethod]
        public void GetObjectsById16_ValidListContainingWhitspace_OutputCountSameAsInput()
        {
            List<short> list = CreateRandomId16List();
            String csvList = CreateListOfIds(list, DefaultIdSeparator + " ");
            IEnumerable<short> resultEnumeration = csvList.GetObjectsById16(n => n);
            Assert.AreEqual(list.Count, resultEnumeration.Count(),
                            "The number of items returned by GetObjectsById16 should be equal to the number of distinct valid items in the comma-seperated input.");
        }

        [TestMethod]
        public void GetObjectsById16_ValidListAndCustomSeparator_OutputCountSameAsInput()
        {
            String separator;
            int num;
            do
            {
                separator = Random.RandomString(1);
            } while (int.TryParse(separator, out num));
            List<short> list = CreateRandomId16List();
            String csvList = CreateListOfIds(list, separator);
            IEnumerable<short> resultEnumeration = csvList.GetObjectsById16(n => n, new[] {separator[0]});
            Assert.AreEqual(list.Count, resultEnumeration.Count(),
                            "The number of items returned by GetObjectsById16 should be equal to the number of distinct valid items in the input, separated by the specified separator.");
        }

        [TestMethod]
        public void GetObjectsById16_ListWithDuplicates_OutputCountSameAsDistinctInput()
        {
            List<short> listWithDuplicates = AddDuplicatesToList(CreateRandomId16List());
            String csvListWithDuplicates = CreateListOfIds(listWithDuplicates);
            IEnumerable<short> resultEnumeration = csvListWithDuplicates.GetObjectsById16(n => n);
            Assert.AreEqual(listWithDuplicates.Distinct().Count(), resultEnumeration.Count(),
                            "The number of items returned by GetObjectsById16 should be equal to the number of distinct valid items in the comma-seperated input.");
        }

        [TestMethod]
        public void GetObjectsById16_ListWithDuplicatesWithoutExecuteImmediately_getObjectFuncNotCalledFromFunction()
        {
            Mock<ITest<short, short>> mock = new Mock<ITest<short, short>>();
            mock.Setup(m => m.Function(It.IsAny<short>())).Returns<short>(n => n);

            List<short> listWithDuplicates = AddDuplicatesToList(CreateRandomId16List());
            String csvListWithDuplicates = CreateListOfIds(listWithDuplicates);
            IEnumerable<short> resultEnumeration = csvListWithDuplicates.GetObjectsById16(mock.Object.Function);
            Assert.IsNotNull(resultEnumeration);
            mock.Verify(m => m.Function(It.IsAny<short>()), Times.Never(),
                        "Unless the executeImmediately argument is true, the getObjectFunction should not be called until the result is enumerated through.");
        }

        [TestMethod]
        public void GetObjectsById16_ListWithDuplicatesAndExecuteImmediately_getObjectFuncCalledOncePerDistinctInput()
        {
            Mock<ITest<short, short>> mock = new Mock<ITest<short, short>>();
            mock.Setup(m => m.Function(It.IsAny<short>())).Returns<short>(n => n);

            List<short> listWithDuplicates = AddDuplicatesToList(CreateRandomId16List());
            String csvListWithDuplicates = CreateListOfIds(listWithDuplicates);
            IEnumerable<short> resultEnumeration = csvListWithDuplicates.GetObjectsById16(mock.Object.Function,
                                                                                          executeImmediately: true);
            Assert.IsNotNull(resultEnumeration);
            mock.Verify(m => m.Function(It.IsAny<short>()), Times.Exactly(listWithDuplicates.Distinct().Count()),
                        "The getObject function supplied to GetObjectsById16 should be called once for each of the distinct valid items in the comma-seperated input.");
        }

        [TestMethod]
        public void GetObjectsById16_ListWithInvalidEntriesAndExecuteImmediately_getObjectFuncCalledOncePerDistinctInput
            ()
        {
            Mock<ITest<short, short>> mock = new Mock<ITest<short, short>>();
            mock.Setup(m => m.Function(It.IsAny<short>())).Returns<short>(n => n);

            List<short> list = CreateRandomId16List();
            String csvListWithBadValue = CreateCommaSeperatedListWithBadValue(list);
            IEnumerable<short> resultEnumeration = csvListWithBadValue.GetObjectsById16(mock.Object.Function,
                                                                                        executeImmediately: true);
            Assert.IsNotNull(resultEnumeration);
            mock.Verify(m => m.Function(It.IsAny<short>()), Times.Exactly(list.Count()),
                        "The getObject function supplied to GetObjectsById16 should be called once for each of the distinct valid items in the comma-seperated input.");
        }

        [TestMethod]
        public void GetObjectsById16_ListWithInvalidEntries_OutputCountSameAsNumberOfValidValuesInInput()
        {
            List<short> list = CreateRandomId16List();
            String csvListWithBadValue = CreateCommaSeperatedListWithBadValue(list);
            IEnumerable<short> resultEnumeration = csvListWithBadValue.GetObjectsById16(n => n);
            Assert.AreEqual(list.Count(), resultEnumeration.Count(),
                            "The number of items returned by GetObjectsById16 should be equal to the number of distinct valid items in the comma-seperated input.");
        }

        [TestMethod]
        public void GetObjectsById16_getObjectFunctionReturnsNullableType_OutputCanContainNullValues()
        {
            List<short> list = CreateRandomId16List();
            String csvList = CreateListOfIds(list);
            IEnumerable<short?> resultEnumeration = csvList.GetObjectsById16<short?>(n => null);
            Assert.IsTrue(resultEnumeration.Contains(null),
                          "When the result of the getObject function is a nullable value type, null values should not be removed from the final output.");
        }

        [TestMethod]
        public void GetObjectsById16_getObjectFunctionReturnsNull_OutputDoesNotContainNullValues()
        {
            List<short> list = CreateRandomId16List();
            String csvList = CreateListOfIds(list);
            IEnumerable<Object> resultEnumeration = csvList.GetObjectsById16<Object>(n => null);
            Assert.IsFalse(resultEnumeration.Contains(null),
                           "When the result of the getObject function is null, these null values should be removed from the final output.");
        }

        [TestMethod]
        public void GetObjectsById16_getObjectFunctionReturnsNullOccasionally_OutputCountMatchesNumberOfNonNullValues()
        {
            List<short> evenNumbers = CreateRandomId16List(short.MaxValue/2).Select(n => (short) (n*2)).ToList();
            List<short> oddNumbers = CreateRandomId16List(short.MaxValue/3).Select(n => (short) (n*2 + 1)).ToList();
            String csvList = CreateListOfIds(evenNumbers.Concat(oddNumbers));
            // The getObject function used here will return null for everything in the evenNumbers list, but non-null for all oddNumbers
            IEnumerable<Object> resultEnumeration = csvList.GetObjectsById16(n => n%2 == 0 ? null : (Object) n);
            Assert.AreEqual(oddNumbers.Count, resultEnumeration.Count(),
                            "When the result of the getObject function can be null, any null values should be removed from the final output.");
        }
        #endregion

        #region GetObjectsById64
        /// <summary>
        /// Creates a distinct random list of integers, each in the range [0,long.MaxValue).
        /// </summary>
        private static List<long> CreateRandomId64List(long maxId = long.MaxValue)
        {
            return
                Enumerable.Range(1, Random.Next(5, 20)).Select(n => (long) (maxId*Random.NextDouble())).Distinct().
                    ToList();
        }

        [ExpectedException(typeof (NullReferenceException))]
        [TestMethod]
        public void GetObjectsById64_NullList_ThrowsNullReferenceException()
        {
            ((string) null).GetObjectsById64(i => i);
        }

        [TestMethod]
        public void GetObjectsById64_ValidList_OutputCountSameAsInput()
        {
            List<long> list = CreateRandomId64List();
            String csvList = CreateListOfIds(list);
            IEnumerable<long> resultEnumeration = csvList.GetObjectsById64(n => n);
            Assert.AreEqual(list.Count, resultEnumeration.Count(),
                            "The number of items returned by GetObjectsById64 should be equal to the number of distinct valid items in the comma-seperated input.");
        }

        [TestMethod]
        public void GetObjectsById64_ValidListContainingWhitspace_OutputCountSameAsInput()
        {
            List<long> list = CreateRandomId64List();
            String csvList = CreateListOfIds(list, DefaultIdSeparator + " ");
            IEnumerable<long> resultEnumeration = csvList.GetObjectsById64(n => n);
            Assert.AreEqual(list.Count, resultEnumeration.Count(),
                            "The number of items returned by GetObjectsById64 should be equal to the number of distinct valid items in the comma-seperated input.");
        }

        [TestMethod]
        public void GetObjectsById64_ValidListAndCustomSeparator_OutputCountSameAsInput()
        {
            String separator;
            long num;
            do
            {
                separator = Random.RandomString(1);
            } while (long.TryParse(separator, out num));
            List<long> list = CreateRandomId64List();
            String csvList = CreateListOfIds(list, separator);
            IEnumerable<long> resultEnumeration = csvList.GetObjectsById64(n => n, new[] {separator[0]});
            Assert.AreEqual(list.Count, resultEnumeration.Count(),
                            "The number of items returned by GetObjectsById64 should be equal to the number of distinct valid items in the input, separated by the specified separator.");
        }

        [TestMethod]
        public void GetObjectsById64_ListWithDuplicates_OutputCountSameAsDistinctInput()
        {
            List<long> listWithDuplicates = AddDuplicatesToList(CreateRandomId64List());
            String csvListWithDuplicates = CreateListOfIds(listWithDuplicates);
            IEnumerable<long> resultEnumeration = csvListWithDuplicates.GetObjectsById64(n => n);
            Assert.AreEqual(listWithDuplicates.Distinct().Count(), resultEnumeration.Count(),
                            "The number of items returned by GetObjectsById64 should be equal to the number of distinct valid items in the comma-seperated input.");
        }

        [TestMethod]
        public void GetObjectsById64_ListWithDuplicatesWithoutExecuteImmediately_getObjectFuncNotCalledFromFunction()
        {
            Mock<ITest<long, long>> mock = new Mock<ITest<long, long>>();
            mock.Setup(m => m.Function(It.IsAny<long>())).Returns<long>(n => n);

            List<long> listWithDuplicates = AddDuplicatesToList(CreateRandomId64List());
            String csvListWithDuplicates = CreateListOfIds(listWithDuplicates);
            IEnumerable<long> resultEnumeration = csvListWithDuplicates.GetObjectsById64(mock.Object.Function);
            Assert.IsNotNull(resultEnumeration);
            mock.Verify(m => m.Function(It.IsAny<long>()), Times.Never(),
                        "Unless the executeImmediately argument is true, the getObjectFunction should not be called until the result is enumerated through.");
        }

        [TestMethod]
        public void GetObjectsById64_ListWithDuplicatesAndExecuteImmediately_getObjectFuncCalledOncePerDistinctInput()
        {
            Mock<ITest<long, long>> mock = new Mock<ITest<long, long>>();
            mock.Setup(m => m.Function(It.IsAny<long>())).Returns<long>(n => n);

            List<long> listWithDuplicates = AddDuplicatesToList(CreateRandomId64List());
            String csvListWithDuplicates = CreateListOfIds(listWithDuplicates);
            IEnumerable<long> resultEnumeration = csvListWithDuplicates.GetObjectsById64(mock.Object.Function,
                                                                                         executeImmediately: true);
            Assert.IsNotNull(resultEnumeration);
            mock.Verify(m => m.Function(It.IsAny<long>()), Times.Exactly(listWithDuplicates.Distinct().Count()),
                        "The getObject function supplied to GetObjectsById64 should be called once for each of the distinct valid items in the comma-seperated input.");
        }

        [TestMethod]
        public void GetObjectsById64_ListWithInvalidEntriesAndExecuteImmediately_getObjectFuncCalledOncePerDistinctInput
            ()
        {
            Mock<ITest<long, long>> mock = new Mock<ITest<long, long>>();
            mock.Setup(m => m.Function(It.IsAny<long>())).Returns<long>(n => n);

            List<long> list = CreateRandomId64List();
            String csvListWithBadValue = CreateCommaSeperatedListWithBadValue(list);
            IEnumerable<long> resultEnumeration = csvListWithBadValue.GetObjectsById64(mock.Object.Function,
                                                                                       executeImmediately: true);
            Assert.IsNotNull(resultEnumeration);
            mock.Verify(m => m.Function(It.IsAny<long>()), Times.Exactly(list.Count()),
                        "The getObject function supplied to GetObjectsById64 should be called once for each of the distinct valid items in the comma-seperated input.");
        }

        [TestMethod]
        public void GetObjectsById64_ListWithInvalidEntries_OutputCountSameAsNumberOfValidValuesInInput()
        {
            List<long> list = CreateRandomId64List();
            String csvListWithBadValue = CreateCommaSeperatedListWithBadValue(list);
            IEnumerable<long> resultEnumeration = csvListWithBadValue.GetObjectsById64(n => n);
            Assert.AreEqual(list.Count(), resultEnumeration.Count(),
                            "The number of items returned by GetObjectsById64 should be equal to the number of distinct valid items in the comma-seperated input.");
        }

        [TestMethod]
        public void GetObjectsById64_getObjectFunctionReturnsNullableType_OutputCanContainNullValues()
        {
            List<long> list = CreateRandomId64List();
            String csvList = CreateListOfIds(list);
            IEnumerable<long?> resultEnumeration = csvList.GetObjectsById64<long?>(n => null);
            Assert.IsTrue(resultEnumeration.Contains(null),
                          "When the result of the getObject function is a nullable value type, null values should not be removed from the final output.");
        }

        [TestMethod]
        public void GetObjectsById64_getObjectFunctionReturnsNull_OutputDoesNotContainNullValues()
        {
            List<long> list = CreateRandomId64List();
            String csvList = CreateListOfIds(list);
            IEnumerable<Object> resultEnumeration = csvList.GetObjectsById64<Object>(n => null);
            Assert.IsFalse(resultEnumeration.Contains(null),
                           "When the result of the getObject function is null, these null values should be removed from the final output.");
        }

        [TestMethod]
        public void GetObjectsById64_getObjectFunctionReturnsNullOccasionally_OutputCountMatchesNumberOfNonNullValues()
        {
            List<long> evenNumbers = CreateRandomId64List(long.MaxValue/2).Select(n => n*2).ToList();
            List<long> oddNumbers = CreateRandomId64List(long.MaxValue/3).Select(n => n*2 + 1).ToList();
            String csvList = CreateListOfIds(evenNumbers.Concat(oddNumbers));
            // The getObject function used here will return null for everything in the evenNumbers list, but non-null for all oddNumbers
            IEnumerable<Object> resultEnumeration = csvList.GetObjectsById64(n => n%2 == 0 ? null : (Object) n);
            Assert.AreEqual(oddNumbers.Count, resultEnumeration.Count(),
                            "When the result of the getObject function can be null, any null values should be removed from the final output.");
        }
        #endregion

        #region Nested type: ITest
        /// <summary>
        /// Used with Moq to add call counters to lambda functions.
        /// </summary>
        /// <typeparam name="TIn">The type of the input.</typeparam>
        /// <typeparam name="TOut">The type of the output.</typeparam>
        public interface ITest<in TIn, out TOut>
        {
            TOut Function(TIn iIn);
        }
        #endregion
    }
}