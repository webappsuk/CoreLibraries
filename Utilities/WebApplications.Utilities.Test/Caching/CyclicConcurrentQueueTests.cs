using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;
using WebApplications.Utilities.Caching;

namespace WebApplications.Utilities.Test.Caching
{
    [TestClass]
    public class CyclicConcurrentQueueTests : TestBase
    {

        [TestMethod]
        public void Constructor_CapacityInRange_DoesNotReturnNull()
        {
            long validCapacity = 1 + (long) Random.NextDouble()*(long.MaxValue - 2);
            CyclicConcurrentQueue<Guid> cyclicConcurrentQueue = new CyclicConcurrentQueue<Guid>(validCapacity);
            Assert.IsNotNull(cyclicConcurrentQueue);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_CapacityZero_ThrowsArgumentOutOfRangeException()
        {
            CyclicConcurrentQueue<Guid> cyclicConcurrentQueue = new CyclicConcurrentQueue<Guid>(0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_CapacityNegotive_ThrowsArgumentOutOfRangeException()
        {
            CyclicConcurrentQueue<Guid> cyclicConcurrentQueue = new CyclicConcurrentQueue<Guid>(-Random.Next());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_CapacityMaxLong_ThrowsArgumentOutOfRangeException()
        {
            CyclicConcurrentQueue<Guid> cyclicConcurrentQueue = new CyclicConcurrentQueue<Guid>(long.MaxValue);
        }

        [TestMethod]
        public void ToArray_InitialCollectionSmallerThanCapacity_MatchesInitialCollection()
        {
            List<Guid> initialValues = Enumerable.Range(1, Random.Next(10, 100)).Select(n => Guid.NewGuid()).ToList();
            long capacity = initialValues.Count + (long)Random.NextDouble() * (long.MaxValue - initialValues.Count);
            CyclicConcurrentQueue<Guid> cyclicConcurrentQueue = new CyclicConcurrentQueue<Guid>(initialValues, capacity);
            CollectionAssert.AreEqual(initialValues.ToArray(), cyclicConcurrentQueue.ToArray());
        }

        [TestMethod]
        public void ToArray_InitialCollectionLargerThanCapacity_MatchesEndPortionOfInitialCollection()
        {
            List<Guid> initialValues = Enumerable.Range(1, Random.Next(10, 100)).Select(n => Guid.NewGuid()).ToList();
            long capacity = Random.Next(5, initialValues.Count);
            CyclicConcurrentQueue<Guid> cyclicConcurrentQueue = new CyclicConcurrentQueue<Guid>(initialValues, capacity);
            CollectionAssert.AreEqual(initialValues.Skip(initialValues.Count-(int)capacity).ToArray(), cyclicConcurrentQueue.ToArray());
        }

        [TestMethod]
        public void Count_InitialCollectionSmallerThanCapacity_MatchesInitialCollection()
        {
            List<Guid> initialValues = Enumerable.Range(1, Random.Next(10, 100)).Select(n => Guid.NewGuid()).ToList();
            long capacity = initialValues.Count + (long)Random.NextDouble() * (long.MaxValue - initialValues.Count);
            CyclicConcurrentQueue<Guid> cyclicConcurrentQueue = new CyclicConcurrentQueue<Guid>(initialValues, capacity);
            Assert.AreEqual(initialValues.Count, cyclicConcurrentQueue.Count());
        }

        [TestMethod]
        public void Count_NoInitialCollection_IsZero()
        {
            long capacity = Random.Next()+10;
            CyclicConcurrentQueue<Guid> cyclicConcurrentQueue = new CyclicConcurrentQueue<Guid>(capacity);
            Assert.AreEqual(0, cyclicConcurrentQueue.Count());
        }

        [TestMethod]
        public void Count_OneEntryAdded_IsOne()
        {
            long capacity = Random.Next()+10;
            CyclicConcurrentQueue<Guid> cyclicConcurrentQueue = new CyclicConcurrentQueue<Guid>(capacity);
            cyclicConcurrentQueue.Enqueue(Guid.NewGuid());
            Assert.AreEqual(1, cyclicConcurrentQueue.Count());
        }

        //TODO does it loop around as expected on enqueue? does it remove things correctly? does it know when all thingsa have been removed? Can you add several items at once?
    }
}
