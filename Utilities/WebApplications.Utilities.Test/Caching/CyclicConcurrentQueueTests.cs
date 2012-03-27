using System;
using System.Collections.Concurrent;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;
using WebApplications.Utilities.Caching;

namespace WebApplications.Utilities.Test.Caching
{
    [TestClass]
    public class CyclicConcurrentQueueTests : TestBase
    {
        CyclicConcurrentQueue<T> CreateCyclicConcurrentQueue<T>(long capacity)
        {
            try
            {
                return new CyclicConcurrentQueue<T>(capacity);
            }
            catch (OutOfMemoryException)
            {
                Assert.Inconclusive("Ran out of memory trying to create test object.");
                return null;
            }
        }
        CyclicConcurrentQueue<T> CreateCyclicConcurrentQueue<T>(IEnumerable<T> collection, long capacity)
        {
            try
            {
                return new CyclicConcurrentQueue<T>(collection, capacity);
            }
            catch (OutOfMemoryException)
            {
                Assert.Inconclusive("Ran out of memory trying to create test object.");
                return null;
            }
        }

        [TestMethod]
        public void Constructor_CapacityInRange_DoesNotReturnNull()
        {
            long validCapacity = 1 + (long) Random.NextDouble()*(long.MaxValue - 2);
            CyclicConcurrentQueue<Guid> cyclicConcurrentQueue = CreateCyclicConcurrentQueue<Guid>(validCapacity);
            Assert.IsNotNull(cyclicConcurrentQueue);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_CapacityZero_ThrowsArgumentOutOfRangeException()
        {
            CyclicConcurrentQueue<Guid> cyclicConcurrentQueue = CreateCyclicConcurrentQueue<Guid>(0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_CapacityNegotive_ThrowsArgumentOutOfRangeException()
        {
            CyclicConcurrentQueue<Guid> cyclicConcurrentQueue = CreateCyclicConcurrentQueue<Guid>(-Random.Next());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_CapacityMaxLong_ThrowsArgumentOutOfRangeException()
        {
            CyclicConcurrentQueue<Guid> cyclicConcurrentQueue = CreateCyclicConcurrentQueue<Guid>(long.MaxValue);
        }

        [TestMethod]
        public void ToArray_InitialCollectionSmallerThanCapacity_MatchesInitialCollection()
        {
            List<Guid> initialValues = Enumerable.Range(1, Random.Next(10, 100)).Select(n => Guid.NewGuid()).ToList();
            long capacity = initialValues.Count + (long)Random.NextDouble() * (long.MaxValue - initialValues.Count);
            CyclicConcurrentQueue<Guid> cyclicConcurrentQueue = CreateCyclicConcurrentQueue<Guid>(initialValues, capacity);
            CollectionAssert.AreEqual(initialValues.ToArray(), cyclicConcurrentQueue.ToArray());
        }

        [TestMethod]
        public void ToArray_InitialCollectionLargerThanCapacity_MatchesEndPortionOfInitialCollection()
        {
            List<Guid> initialValues = Enumerable.Range(1, Random.Next(10, 100)).Select(n => Guid.NewGuid()).ToList();
            long capacity = Random.Next(5, initialValues.Count);
            CyclicConcurrentQueue<Guid> cyclicConcurrentQueue = CreateCyclicConcurrentQueue<Guid>(initialValues, capacity);
            CollectionAssert.AreEqual(initialValues.Skip(initialValues.Count-(int)capacity).ToArray(), cyclicConcurrentQueue.ToArray());
        }

        [TestMethod]
        public void Count_InitialCollectionSmallerThanCapacity_MatchesInitialCollection()
        {
            List<Guid> initialValues = Enumerable.Range(1, Random.Next(10, 100)).Select(n => Guid.NewGuid()).ToList();
            long capacity = initialValues.Count + (long)Random.NextDouble() * (long.MaxValue - initialValues.Count);
            CyclicConcurrentQueue<Guid> cyclicConcurrentQueue = CreateCyclicConcurrentQueue<Guid>(initialValues, capacity);
            Assert.AreEqual(initialValues.Count, cyclicConcurrentQueue.Count());
        }

        [TestMethod]
        public void Count_NoInitialCollection_IsZero()
        {
            long capacity = Random.Next()+10;
            CyclicConcurrentQueue<bool> cyclicConcurrentQueue = CreateCyclicConcurrentQueue<bool>(capacity);
            Assert.AreEqual(0, cyclicConcurrentQueue.Count());
        }

        [TestMethod]
        public void Count_OneEntryAddedUsingEnqueue_IsOne()
        {
            long capacity = Random.Next() + 10;
            CyclicConcurrentQueue<bool> cyclicConcurrentQueue = CreateCyclicConcurrentQueue<bool>(capacity);
            cyclicConcurrentQueue.Enqueue(Random.NextDouble() < 0.5);
            Assert.AreEqual(1, cyclicConcurrentQueue.Count());
        }

        [TestMethod]
        public void Count_MuliptleEntriesAddedUsingEnqueue_IsOne()
        {
            long capacity = Random.Next() + 10;
            int count = Random.Next(2, capacity > 10000 ? 10000 : (int) capacity );
            CyclicConcurrentQueue<bool> cyclicConcurrentQueue = CreateCyclicConcurrentQueue<bool>(capacity);
            for (int i = 0; i < count; i++)
                cyclicConcurrentQueue.Enqueue(Random.NextDouble() < 0.5);
            Assert.AreEqual(count, cyclicConcurrentQueue.Count());
        }

        [TestMethod]
        public void Count_MoreAddedUsingEnqueueThanCapacityHolds_IsSameAsCapacity()
        {
            long capacity = Random.Next(10,10000);
            int overlap = Random.Next(1, 9);
            CyclicConcurrentQueue<int> cyclicConcurrentQueue = CreateCyclicConcurrentQueue<int>(capacity);
            for (int i = 0; i < capacity + overlap; i++)
                cyclicConcurrentQueue.Enqueue(Random.Next());
            Assert.AreEqual(capacity, cyclicConcurrentQueue.Count());
        }

        [TestMethod]
        public void ToList_InitialCollectionAndItemsEnqueuedWithinCapacity_MatchesInitialCollectionAndExtraItemAtEnd()
        {
            List<int> initialValues = Enumerable.Range(1, Random.Next(10, 100)).Select(n => Random.Next()).ToList();
            List<int> addedValues = Enumerable.Range(1, Random.Next(10, 100)).Select(n => Random.Next()).ToList();
            long capacity = initialValues.Count + addedValues.Count + Random.Next(1, 100);
            CyclicConcurrentQueue<int> cyclicConcurrentQueue = CreateCyclicConcurrentQueue(initialValues, capacity);
            foreach (int addedValue in addedValues)
            {
                cyclicConcurrentQueue.Enqueue(addedValue);
            }
            List<int> expectedValues = initialValues.Concat(addedValues).ToList();
            CollectionAssert.AreEqual(expectedValues, cyclicConcurrentQueue.ToList());
        }

        [TestMethod]
        public void ToList_ItemsEnqueuedToExceedCapacity_EarlierValuesOverridenWhenPastCapacity()
        {
            List<int> values = Enumerable.Range(1, Random.Next(100, 1000)).Select(n => Random.Next()).ToList();
            long capacity = Random.Next(20, values.Count / 2);
            CyclicConcurrentQueue<int> cyclicConcurrentQueue = CreateCyclicConcurrentQueue<int>(capacity);
            foreach (int value in values)
            {
                cyclicConcurrentQueue.Enqueue(value);
            }
            List<int> expectedValues = values.Skip(values.Count - (int)capacity).ToList();
            CollectionAssert.AreEqual(expectedValues, cyclicConcurrentQueue.ToList());
        }

        [TestMethod]
        public void ToList_ItemsEnqueuedToExceedCapacityThenItemsDequeued_EarlierValuesOverridenWhenPastCapacityThenQueueShortenedOnDequeue()
        {
            List<int> values = Enumerable.Range(1, Random.Next(100, 1000)).Select(n => Random.Next()).ToList();
            long capacity = Random.Next(20, values.Count / 2);
            int amountToDequeue = Random.Next(1, (int)capacity / 2);
            CyclicConcurrentQueue<int> cyclicConcurrentQueue = CreateCyclicConcurrentQueue<int>(capacity);
            foreach (int value in values)
            {
                cyclicConcurrentQueue.Enqueue(value);
            }
            for(int i=0;i<amountToDequeue;i++)
            {
                int dequeued;
                Assert.IsTrue(cyclicConcurrentQueue.TryDequeue(out dequeued),"This test should always succeed when attempting to Dequeue elements.");
            }
            List<int> expectedValues = values.Skip(values.Count - (int)capacity).Skip(amountToDequeue).ToList();
            CollectionAssert.AreEqual(expectedValues, cyclicConcurrentQueue.ToList());
        }

        [TestMethod]
        public void TryDequeue_NothingQueued_ReturnsFalse()
        {
            long capacity = Random.Next(10, 10000);
            CyclicConcurrentQueue<int> cyclicConcurrentQueue = CreateCyclicConcurrentQueue<int>(capacity);
            int dequeued;
            Assert.IsFalse(cyclicConcurrentQueue.TryDequeue(out dequeued));
        }

        [TestMethod]
        public void TryDequeue_SomethingQueuedThenDequeuedAlready_ReturnsFalse()
        {
            long capacity = Random.Next(10, 10000);
            CyclicConcurrentQueue<int> cyclicConcurrentQueue = CreateCyclicConcurrentQueue<int>(capacity);
            cyclicConcurrentQueue.Enqueue(Random.Next());
            int dequeued;
            // Remove the enqueued item, and mark the test inconclusive if it fails
            if( !cyclicConcurrentQueue.TryDequeue(out dequeued) )
                Assert.Inconclusive("Could not successfully dequeue an item.");
            // We should now have and empty queue again.
            Assert.IsFalse(cyclicConcurrentQueue.TryDequeue(out dequeued));
        }

        [TestMethod]
        public void TryDequeue_SomethingQueued_ReturnsTrue()
        {
            long capacity = Random.Next(10, 10000);
            CyclicConcurrentQueue<int> cyclicConcurrentQueue = CreateCyclicConcurrentQueue<int>(capacity);
            cyclicConcurrentQueue.Enqueue(Random.Next());
            int dequeued;
            Assert.IsTrue(cyclicConcurrentQueue.TryDequeue(out dequeued));
        }

        [TestMethod]
        public void TryDequeue_SomethingQueued_OutputsLeastRecentlyEnqueuedItem()
        {
            long capacity = Random.Next(10, 10000);
            List<int> enqueuedItems = Enumerable.Range(1, Random.Next(2, (int)capacity)).Select(n => Random.Next()).ToList();
            CyclicConcurrentQueue<int> cyclicConcurrentQueue = CreateCyclicConcurrentQueue<int>(capacity);
            foreach (int enqueuedItem in enqueuedItems)
            {
                cyclicConcurrentQueue.Enqueue(enqueuedItem);
            }
            int dequeued;
            cyclicConcurrentQueue.TryDequeue(out dequeued);
            Assert.AreEqual(enqueuedItems.First(), dequeued);
        }

        [TestMethod]
        public void Count_SomethingDequeuedSuccessfully_ValueDropsByOne()
        {
            List<int> initialValues = Enumerable.Range(1, Random.Next(10, 10000)).Select(n => Random.Next()).ToList();
            long capacity = Random.Next(10, 10000);
            CyclicConcurrentQueue<int> cyclicConcurrentQueue = CreateCyclicConcurrentQueue(initialValues,capacity);
            int previousCount = cyclicConcurrentQueue.Count();
            int dequeued;
            if (cyclicConcurrentQueue.TryDequeue(out dequeued))
            {
                Assert.AreEqual(previousCount-1, cyclicConcurrentQueue.Count());
            }
            else
            {
                Assert.Inconclusive("Could not successfully dequeue an item.");
            }
        }

        [TestMethod]
        public void TryPeek_NothingQueued_ReturnsFalse()
        {
            long capacity = Random.Next(10, 10000);
            CyclicConcurrentQueue<int> cyclicConcurrentQueue = CreateCyclicConcurrentQueue<int>(capacity);
            int peeked;
            Assert.IsFalse(cyclicConcurrentQueue.TryPeek(out peeked));
        }

        [TestMethod]
        public void TryPeek_SomethingQueued_ReturnsTrue()
        {
            long capacity = Random.Next(10, 10000);
            CyclicConcurrentQueue<int> cyclicConcurrentQueue = CreateCyclicConcurrentQueue<int>(capacity);
            cyclicConcurrentQueue.Enqueue(Random.Next());
            int peeked;
            Assert.IsTrue(cyclicConcurrentQueue.TryPeek(out peeked));
        }

        [TestMethod]
        public void TryPeek_ItemsQueued_OutputsLeastRecentlyEnqueuedItem()
        {
            long capacity = Random.Next(10, 10000);
            List<int> enqueuedItems = Enumerable.Range(1,Random.Next(2,(int)capacity)).Select(n=>Random.Next()).ToList();
            CyclicConcurrentQueue<int> cyclicConcurrentQueue = CreateCyclicConcurrentQueue<int>(capacity);
            foreach (int enqueuedItem in enqueuedItems)
            {
                cyclicConcurrentQueue.Enqueue(enqueuedItem);
            }
            int peeked;
            cyclicConcurrentQueue.TryPeek(out peeked);
            Assert.AreEqual(enqueuedItems.First(), peeked);
        }

        [TestMethod]
        public void TryPeek_SomethingQueued_OutputsSameValueWhenCalledTwice()
        {
            long capacity = Random.Next(10, 10000);
            int enqueued = Random.Next();
            CyclicConcurrentQueue<int> cyclicConcurrentQueue = CreateCyclicConcurrentQueue<int>(capacity);
            cyclicConcurrentQueue.Enqueue(enqueued);
            int peeked1, peeked2;
            cyclicConcurrentQueue.TryPeek(out peeked1);
            cyclicConcurrentQueue.TryPeek(out peeked2);
            Assert.AreEqual(peeked1,peeked2);
        }

        [TestMethod]
        public void Count_SomethingPeekedSuccessfully_ValueStaysTheSame()
        {
            List<int> initialValues = Enumerable.Range(1, Random.Next(10, 10000)).Select(n => Random.Next()).ToList();
            long capacity = Random.Next(10, 10000);
            CyclicConcurrentQueue<int> cyclicConcurrentQueue = CreateCyclicConcurrentQueue(initialValues, capacity);
            int previousCount = cyclicConcurrentQueue.Count();
            int peeked;
            if (cyclicConcurrentQueue.TryPeek(out peeked))
            {
                Assert.AreEqual(previousCount, cyclicConcurrentQueue.Count());
            }
            else
            {
                Assert.Inconclusive("Could not successfully peek at an item.");
            }
        }

        [TestMethod]
        public void GetEnumerator_ItemEnqueuedAfterGetAndBeforeIterating_EnqueuedValueIncludedInIteration()
        {
            List<int> initialValues = Enumerable.Range(1, Random.Next(10, 10000)).Select(n => Random.Next()).ToList();
            long capacity = Random.Next(10, 10000);
            CyclicConcurrentQueue<int> cyclicConcurrentQueue = CreateCyclicConcurrentQueue(initialValues, capacity);
            List<int> iterationResult = new List<int>();
            int enqueued = Random.Next();
            using (IEnumerator<int> enumerator = cyclicConcurrentQueue.GetEnumerator())
            {
                cyclicConcurrentQueue.Enqueue(enqueued);
                while (enumerator.MoveNext())
                    iterationResult.Add(enumerator.Current);
            }
            Assert.AreEqual(enqueued, iterationResult.Last());
        }

        [TestMethod]
        public void GetEnumerator_AtCapacityAndItemEnqueuedWhilstIterating_IterationCountNeverExceedsCapacity()
        {
            List<int> initialValues = Enumerable.Range(1, Random.Next(10, 10000)).Select(n => Random.Next()).ToList();
            long capacity = initialValues.Count;
            CyclicConcurrentQueue<int> cyclicConcurrentQueue = CreateCyclicConcurrentQueue(initialValues, capacity);
            List<int> iterationResult = new List<int>();
            List<int> enqueuedValues = Enumerable.Range(1, Random.Next(1, 100)).Select(n => Random.Next()).ToList();
            using (IEnumerator<int> enumerator = cyclicConcurrentQueue.GetEnumerator())
            {
                //Start iterating a single time
                enumerator.MoveNext();
                iterationResult.Add(enumerator.Current);

                //Add extra values
                foreach (int enqueuedValue in enqueuedValues)
                {
                    cyclicConcurrentQueue.Enqueue(enqueuedValue);
                }

                //Finish iterating
                while (enumerator.MoveNext())
                    iterationResult.Add(enumerator.Current);
            }
            Assert.IsTrue(capacity < iterationResult.Count, "Iteration count ({0}) should always be smaller than the capacity ({1})", iterationResult.Count, capacity);
        }

        [TestMethod]
        public void Enqueue_CalledInParallel_AllValuesAdded()
        {
            List<int> values = Enumerable.Range(1, Random.Next(10, 10000)).Select(n => Random.Next()).ToList();
            long capacity = values.Count + Random.Next(1, 1000);
            CyclicConcurrentQueue<int> cyclicConcurrentQueue = CreateCyclicConcurrentQueue<int>(capacity);
            Parallel.ForEach(values, cyclicConcurrentQueue.Enqueue);
            CollectionAssert.AreEquivalent(values, cyclicConcurrentQueue);
        }

        [TestMethod]
        public void TryDequeue_CalledInParallelForEveryElement_AllValuesRemovedExactlyOnce()
        {
            List<Guid> values = Enumerable.Range(1, Random.Next(10, 10000)).Select(n => Guid.NewGuid()).ToList();
            long capacity = values.Count + Random.Next(1, 1000);
            CyclicConcurrentQueue<Guid> cyclicConcurrentQueue = CreateCyclicConcurrentQueue(values,capacity);
            ConcurrentBag<Guid> dequeuedValues = new ConcurrentBag<Guid>();
            Parallel.For(0,values.Count, i =>
                                             {
                                                 Guid value;
                                                 cyclicConcurrentQueue.TryDequeue(out value);
                                                 dequeuedValues.Add(value);
                                             });
            CollectionAssert.AreEquivalent(values, dequeuedValues);
            Assert.AreEqual(0, cyclicConcurrentQueue.Count);
        }
    }
}
