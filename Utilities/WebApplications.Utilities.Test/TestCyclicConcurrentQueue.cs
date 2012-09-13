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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;
using WebApplications.Utilities.Caching;

namespace WebApplications.Utilities.Test
{
    [TestClass]
    public class TestCyclicConcurrentQueue : UtilitiesTestBase
    {
        private long _operations;

        [TestMethod]
        public void TestEnqueueDequeue()
        {
            int capacity = 10000; // (int)(BigArray<int>.BlockSize * 2.5);

            CyclicConcurrentQueue<string> queue = new CyclicConcurrentQueue<string>(capacity);

            string testStr = Random.RandomString();
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
            Trace.WriteLine(String.Format("Average: {0:N}ns", (stopwatch.ElapsedMilliseconds*1000000)/_operations));

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

            string testStr = Random.RandomString();
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
            Trace.WriteLine(String.Format("Average: {0:N}ns", (stopwatch.ElapsedMilliseconds*1000000)/_operations));
            List<string> list = queue.ToList();
            long count = queue.Count;
            Assert.AreEqual(queue.Count, list.Count);
            queue = new CyclicConcurrentQueue<string>(queue, count + 10);
            Assert.AreEqual(count, queue.Count);
        }

        [TestMethod]
        public void TestCCEnqueueDequeue()
        {
            int capacity = 10000; // (int)(BigArray<int>.BlockSize * 2.5);

            ConcurrentQueue<string> queue = new ConcurrentQueue<string>();

            string testStr = Random.RandomString();
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

            string testStr = Random.RandomString();
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
            Trace.WriteLine(String.Format("Average: {0:N}ns", (stopwatch.ElapsedMilliseconds*1000000)/_operations));
            List<string> list = queue.ToList();
            long count = queue.Count;
            Assert.AreEqual(queue.Count, list.Count);
            queue = new ConcurrentQueue<string>(queue);
            Assert.AreEqual(count, queue.Count);
        }
    }
}