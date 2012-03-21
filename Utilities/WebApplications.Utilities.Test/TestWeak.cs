#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities.Test
// File: TestWeak.cs
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
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Caching;
using WebApplications.Testing;

namespace WebApplications.Utilities.Test
{
    [TestClass]
    public class TestWeak
    {
        [TestMethod]
        public void TestWeakConcurrentDictionaryReferences()
        {
            const int elements = 1000000;
            Random random = new Random();
            WeakConcurrentDictionary<int, TestClass> weakConcurrentDictionary =
                new WeakConcurrentDictionary<int, TestClass>(allowResurrection: false);
            ConcurrentDictionary<int, TestClass> referenceDictionary = new ConcurrentDictionary<int, TestClass>();
            int nullCount = 0;
            int unreferencedNullCount = 0;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Parallel.For(0, elements, l =>
                                          {
                                              // Include nulls ~25% of the time.
                                              TestClass t;
                                              if (random.Next(4) < 3)
                                                  t = new TestClass(random.Next(int.MinValue, int.MaxValue));
                                              else
                                              {
                                                  t = null;
                                                  Interlocked.Increment(ref nullCount);
                                              }

                                              weakConcurrentDictionary.Add(l, t);

                                              // Only keep references ~33% of the time.
                                              if (random.Next(3) == 0)
                                                  referenceDictionary.AddOrUpdate(l, t, (k, v) => t);
                                              else if (t == null)
                                                  Interlocked.Increment(ref unreferencedNullCount);
                                          });
            stopwatch.Stop();
            Trace.WriteLine(stopwatch.ToString("Populating dictionaries with {0} elements", elements));

            //GC.WaitForFullGCComplete(5000);
            stopwatch.Restart();
            long bytes = GC.GetTotalMemory(true);
            stopwatch.Stop();
            Trace.WriteLine(stopwatch.ToString("Garbage collection"));
            Trace.WriteLine(string.Format("Memory: {0}K", bytes/1024));

            // Check that we have l
            Assert.IsTrue(referenceDictionary.Count <= elements);

            int refCount = referenceDictionary.Count;

            stopwatch.Restart();
            int weakCount = weakConcurrentDictionary.Count;
            stopwatch.Stop();
            Trace.WriteLine(stopwatch.ToString("Counting '{0}' elements", weakCount));

            stopwatch.Restart();
            weakCount = weakConcurrentDictionary.Count;
            stopwatch.Stop();
            Trace.WriteLine(stopwatch.ToString("Counting '{0}' elements again", weakCount));

            int floor = refCount + unreferencedNullCount;

            Trace.WriteLine(
                string.Format(
                    "Referenced Dictionary Count: {1}{0}Weak Dictionary Count: {2}{0}Null values: {3} (unreferenced: {4}){0}Garbage Collected: {5}{0}Pending Collection: {6}{0}",
                    Environment.NewLine,
                    refCount,
                    weakCount,
                    nullCount,
                    unreferencedNullCount,
                    elements - weakCount,
                    weakCount - floor
                    ));

            // Check we only have references to referenced elements.
            Assert.AreEqual(refCount + unreferencedNullCount, weakCount);

            // Check everything that's still referenced is available.
            stopwatch.Restart();
            Parallel.ForEach(referenceDictionary,
                             kvp =>
                                 {
                                     TestClass value;
                                     Assert.IsTrue(weakConcurrentDictionary.TryGetValue(kvp.Key, out value));
                                     Assert.AreEqual(kvp.Value, value);
                                 });
            stopwatch.Stop();
            Trace.WriteLine(stopwatch.ToString("Checking '{0}' elements", weakCount));
        }

        [TestMethod]
        public void TestWeakConcurrentDictionaryReferencesObservable()
        {
            const int elements = 1000000;
            Random random = new Random();
            WeakConcurrentDictionary<int, ObservableTestClass> weakConcurrentDictionary =
                new WeakConcurrentDictionary<int, ObservableTestClass>(allowResurrection: false);
            ConcurrentDictionary<int, ObservableTestClass> referenceDictionary =
                new ConcurrentDictionary<int, ObservableTestClass>();
            int nullCount = 0;
            int unreferencedNullCount = 0;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Parallel.For(0, elements, l =>
                                          {
                                              // Include nulls ~25% of the time.
                                              ObservableTestClass t;
                                              if (random.Next(4) < 3)
                                                  t = new ObservableTestClass(random.Next(int.MinValue, int.MaxValue));
                                              else
                                              {
                                                  t = null;
                                                  Interlocked.Increment(ref nullCount);
                                              }

                                              weakConcurrentDictionary.Add(l, t);

                                              // Only keep references ~33% of the time.
                                              if (random.Next(3) == 0)
                                                  referenceDictionary.AddOrUpdate(l, t, (k, v) => t);
                                              else if (t == null)
                                                  Interlocked.Increment(ref unreferencedNullCount);
                                          });
            stopwatch.Stop();
            Trace.WriteLine(stopwatch.ToString("Populating dictionaries with {0} elements", elements));

            //GC.WaitForFullGCComplete(5000);
            stopwatch.Restart();
            long bytes = GC.GetTotalMemory(true);
            stopwatch.Stop();
            Trace.WriteLine(stopwatch.ToString("Garbage collection"));
            Trace.WriteLine(string.Format("Memory: {0}K", bytes/1024));

            // Check that we have l
            Assert.IsTrue(referenceDictionary.Count <= elements);

            int refCount = referenceDictionary.Count;

            stopwatch.Restart();
            int weakCount = weakConcurrentDictionary.Count;
            stopwatch.Stop();
            Trace.WriteLine(stopwatch.ToString("Counting '{0}' elements", weakCount));

            stopwatch.Restart();
            weakCount = weakConcurrentDictionary.Count;
            stopwatch.Stop();
            Trace.WriteLine(stopwatch.ToString("Counting '{0}' elements again", weakCount));

            int floor = refCount + unreferencedNullCount;

            Trace.WriteLine(
                string.Format(
                    "Referenced Dictionary Count: {1}{0}Weak Dictionary Count: {2}{0}Null values: {3} (unreferenced: {4}){0}Garbage Collected: {5}{0}Pending Collection: {6}{0}",
                    Environment.NewLine,
                    refCount,
                    weakCount,
                    nullCount,
                    unreferencedNullCount,
                    elements - weakCount,
                    weakCount - floor
                    ));

            // Check we only have references to referenced elements.
            Assert.AreEqual(refCount + unreferencedNullCount, weakCount);

            // Check everything that's still referenced is available.
            stopwatch.Restart();
            Parallel.ForEach(referenceDictionary,
                             kvp =>
                                 {
                                     ObservableTestClass value;
                                     Assert.IsTrue(weakConcurrentDictionary.TryGetValue(kvp.Key, out value));
                                     Assert.AreEqual(kvp.Value, value);
                                 });
            stopwatch.Stop();
            Trace.WriteLine(stopwatch.ToString("Checking '{0}' elements", weakCount));
        }


        [TestMethod]
        public void TestConcurrencySpeeds()
        {
            const int loops = 1000000;
            Stopwatch stopwatch = new Stopwatch();
            ConcurrentDictionary<int, TestClass> intKey = new ConcurrentDictionary<int, TestClass>();
            ConcurrentDictionary<int, TestClass> intKeyFast = new ConcurrentDictionary<int, TestClass>();
            ConcurrentDictionary<Guid, TestClass> guidKey = new ConcurrentDictionary<Guid, TestClass>();
            WeakConcurrentDictionary<int, TestClass> weakIntKey = new WeakConcurrentDictionary<int, TestClass>();
            WeakConcurrentDictionary<int, ObservableTestClass> weakIntKeyObservable =
                new WeakConcurrentDictionary<int, ObservableTestClass>();
            stopwatch.Start();
            Parallel.For(0, loops,
                         l =>
                             {
                                 Guid guid = Guid.NewGuid();
                                 intKey.AddOrUpdate(l, new TestClass(l), (k, v) => new TestClass(l));
                             });
            stopwatch.Stop();
            Trace.WriteLine(stopwatch.ToString("Int key added '{0}' elements", loops));

            stopwatch.Restart();
            Parallel.For(0, loops, l => intKeyFast.AddOrUpdate(l, new TestClass(l), (k, v) => new TestClass(l)));
            stopwatch.Stop();
            Trace.WriteLine(stopwatch.ToString("Int key added '{0}' elements (without GUID generation)", loops));

            stopwatch.Restart();
            Parallel.For(0, loops,
                         l =>
                             {
                                 Guid guid = Guid.NewGuid();
                                 guidKey.AddOrUpdate(guid, new TestClass(l), (k, v) => new TestClass(l));
                             });
            stopwatch.Stop();
            Trace.WriteLine(stopwatch.ToString("Guid key added '{0}' elements", loops));

            stopwatch.Restart();
            Parallel.For(0, loops,
                         l =>
                             {
                                 Guid guid = Guid.NewGuid();
                                 weakIntKey.AddOrUpdate(l, new TestClass(l), (k, v) => new TestClass(l));
                             });
            stopwatch.Stop();
            Trace.WriteLine(stopwatch.ToString("Weak added '{0}' elements", loops));

            stopwatch.Restart();
            Parallel.For(0, loops,
                         l =>
                             {
                                 Guid guid = Guid.NewGuid();
                                 weakIntKeyObservable.AddOrUpdate(l, new ObservableTestClass(l),
                                                                  (k, v) => new ObservableTestClass(l));
                             });
            stopwatch.Stop();
            Trace.WriteLine(stopwatch.ToString("Weak observable added '{0}' elements", loops));
        }

        #region Nested type: ObservableTestClass
        /// <summary>
        /// A test class for placing in a weak dictionary.
        /// </summary>
        /// <remarks></remarks>
        public class ObservableTestClass : IObservableFinalize
        {
            public readonly int? Value;

            /// <summary>
            /// Initializes a new instance of the <see cref="TestClass"/> class.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <remarks></remarks>
            public ObservableTestClass(int? value)
            {
                Value = value;
            }
        
        private EventHandler _finalized;

        /// <inheritdoc />
        public event EventHandler Finalized
        {
            add
            {
                if (_finalized == null)
                    GC.ReRegisterForFinalize(this);

                _finalized += value;
            }

            remove
            {
                _finalized -= value;

                if (_finalized == null)
                    GC.SuppressFinalize(this);
            }
        }

        /// <inheritdoc />
        ~ObservableTestClass()
        {
            if (_finalized != null)
                _finalized(this, EventArgs.Empty);
        }
        }
        #endregion

        #region Nested type: TestClass
        /// <summary>
        /// A test class for placing in a weak dictionary.
        /// </summary>
        /// <remarks></remarks>
        public class TestClass
        {
            public readonly int? Value;

            /// <summary>
            /// Initializes a new instance of the <see cref="TestClass"/> class.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <remarks></remarks>
            public TestClass(int? value)
            {
                Value = value;
            }
        }
        #endregion
    }
}