#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities.Test
// File: TestLimitedConcurrentQueue.cs
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
// © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
#endregion

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;
using WebApplications.Utilities.Caching;

namespace WebApplications.Utilities.Test
{
    [TestClass]
    public class TestCyclicConcurrentQueue : TestBase
    {
        private long _operations;

        [TestMethod]
        public void TestEnqueueDequeue()
        {
            int capacity = 10000; // (int)(BigArray<int>.BlockSize * 2.5);

            CyclicConcurrentQueue<string> queue = new CyclicConcurrentQueue<string>(capacity);

            string testStr = GenerateRandomString();
            queue.Enqueue(testStr);
            string item;
            Assert.IsTrue(queue.TryDequeue(out item));
            Assert.AreEqual(testStr, item);

            _operations = 0;

            long value = 0;
            int loops = 4;
            Action[] actions = new Action[loops];
            for (int i = 0; i < loops; i++)
            {
                actions[i] = i%2 == 0
                                 ? (Action)
                                   (() =>
                                    Parallel.For(0, Random.Next(1000000),
                                                 l =>
                                                     {
                                                         queue.Enqueue(String.Format("{0} - {1}", i, l));
                                                         Interlocked.Increment(ref _operations);
                                                     }))
                                 : (() =>
                                    Parallel.For(0, Random.Next(1000000), l =>
                                                                              {
                                                                                  string it;
                                                                                  if (queue.TryDequeue(out it))
                                                                                      Assert.IsNotNull(it);
                                                                                  Interlocked.Increment(ref _operations);
                                                                              }));
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Parallel.Invoke(actions);
            stopwatch.Stop();

            Trace.WriteLine(stopwatch.ToString("{0} Queue Operations", _operations));
            Trace.WriteLine(String.Format("Average: {0:N}ns", (stopwatch.ElapsedMilliseconds * 1000000) / _operations));

            string s;
            while (queue.TryDequeue(out s))
            {
            }

            Assert.AreEqual(0, queue.Count);
        }

        [TestMethod]
        public void TestSynchronous()
        {
            CyclicConcurrentQueue<string> queue = new CyclicConcurrentQueue<string>(1000);

            string testStr = GenerateRandomString();
            queue.Enqueue(testStr);
            string item;
            Assert.IsTrue(queue.TryDequeue(out item));
            Assert.AreEqual(testStr, item);
            int loops = 1791445;

            int swap = 0;
            bool enqueue = true;
            _operations = 0;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < loops; i++)
            {
                if (swap < 1)
                {
                    swap = Random.Next(1, 20);
                    enqueue = !enqueue;
                }
                if (enqueue)
                {
                    queue.Enqueue(i.ToString());
                }
                else
                {
                    if (queue.TryDequeue(out item))
                    {
                        Assert.IsNotNull(item);
                    }
                }
                Interlocked.Increment(ref _operations);
                swap--;
            }
            
            stopwatch.Stop();
            Trace.WriteLine(stopwatch.ToString("{0} Queue Operations", _operations));
            Trace.WriteLine(String.Format("Average: {0:N}ns", (stopwatch.ElapsedMilliseconds * 1000000) / _operations));
            List<string> list = queue.ToList();
            long count = queue.Count;
            Assert.AreEqual(queue.Count, list.Count);
            queue = new CyclicConcurrentQueue<string>(queue, count+10);
            Assert.AreEqual(count, queue.Count);
        }

        [TestMethod]
        public void TestCCEnqueueDequeue()
        {
            int capacity = 10000; // (int)(BigArray<int>.BlockSize * 2.5);

            ConcurrentQueue<string> queue = new ConcurrentQueue<string>();

            string testStr = GenerateRandomString();
            queue.Enqueue(testStr);
            string item;
            Assert.IsTrue(queue.TryDequeue(out item));
            Assert.AreEqual(testStr, item);

            _operations = 0;

            long value = 0;
            int loops = 4;
            Action[] actions = new Action[loops];
            for (int i = 0; i < loops; i++)
            {
                actions[i] = i % 2 == 0
                                 ? (Action)
                                   (() =>
                                    Parallel.For(0, Random.Next(1000000),
                                                 l =>
                                                 {
                                                     queue.Enqueue(String.Format("{0} - {1}", i, l));
                                                     Interlocked.Increment(ref _operations);
                                                 }))
                                 : (() =>
                                    Parallel.For(0, Random.Next(1000000), l =>
                                    {
                                        string it;
                                        if (queue.TryDequeue(out it))
                                            Assert.IsNotNull(it);
                                        Interlocked.Increment(ref _operations);
                                    }));
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Parallel.Invoke(actions);
            stopwatch.Stop();

            Trace.WriteLine(stopwatch.ToString("{0} Queue Operations", _operations));
            Trace.WriteLine(String.Format("Average: {0:N}ns", (stopwatch.ElapsedMilliseconds*1000000)/_operations));

            string s;
            while (queue.TryDequeue(out s))
            {
            }

            Assert.AreEqual(0, queue.Count);
        }

        [TestMethod]
        public void TestCCSynchronous()
        {
            ConcurrentQueue<string> queue = new ConcurrentQueue<string>();

            string testStr = GenerateRandomString();
            queue.Enqueue(testStr);
            string item;
            Assert.IsTrue(queue.TryDequeue(out item));
            Assert.AreEqual(testStr, item);
            int loops = 1791445;

            int swap = 0;
            bool enqueue = true;
            _operations = 0;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < loops; i++)
            {
                if (swap < 1)
                {
                    swap = Random.Next(1, 20);
                    enqueue = !enqueue;
                }
                if (enqueue)
                {
                    queue.Enqueue(i.ToString());
                }
                else
                {
                    if (queue.TryDequeue(out item))
                    {
                        Assert.IsNotNull(item);
                    }
                }
                Interlocked.Increment(ref _operations);
                swap--;
            }

            stopwatch.Stop();
            Trace.WriteLine(stopwatch.ToString("{0} Queue Operations", _operations));
            Trace.WriteLine(String.Format("Average: {0:N}ns", (stopwatch.ElapsedMilliseconds * 1000000) / _operations));
            List<string> list = queue.ToList();
            long count = queue.Count;
            Assert.AreEqual(queue.Count, list.Count);
            queue = new ConcurrentQueue<string>(queue);
            Assert.AreEqual(count, queue.Count);
        }
    }
}
