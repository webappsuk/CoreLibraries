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
using System.Diagnostics;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Caching;
using WebApplications.Utilities.Interfaces.Caching;

namespace WebApplications.Utilities.Test
{
    [TestClass]
    public class TestCacheDictionary
    {
        public const int CacheSize = 1000000;
        public const int MaximumLoopCount = 20;

        public int AddedFixed;
        public int AddedRandomExpiry;
        public int AddedRandomTimeSpan;
        public int CheckSuccess;
        public int LoopCount;
        public int RemoveFailed;
        public int RemoveSucceeded;

        [TestMethod]
        //[Ignore]
        public void TestMemoryCachePerformance()
        {
            AddedFixed = 0;
            AddedRandomExpiry = 0;
            AddedRandomTimeSpan = 0;
            RemoveSucceeded = 0;
            RemoveFailed = 0;
            CheckSuccess = 0;
            LoopCount = 0;

            MemoryCache cache = MemoryCache.Default;

            const int loopSize = CacheSize*4/3;
            double averageLoopMs = 0;
            do
            {
                DateTime loopStart = DateTime.Now;
                DateTime fixedExpiry = loopStart.Add(TimeSpan.FromMinutes(1));
                Parallel.For(
                    0,
                    loopSize,
                    i =>
                        {
                            Random random = new Random();
                            string key = i.ToString();

                            switch (i%4)
                            {
                                case 0:
                                    // Add with fixed expiry
                                    cache.AddOrGetExisting(key, i, fixedExpiry);
                                    Interlocked.Increment(ref AddedFixed);
                                    break;
                                case 1:
                                    // Add with random expiry
                                    cache.AddOrGetExisting(
                                        key, i, DateTime.Now.AddMilliseconds(random.Next(0, 60000)));
                                    Interlocked.Increment(ref AddedRandomExpiry);
                                    break;
                                case 2:
                                    cache.AddOrGetExisting(
                                        key,
                                        i,
                                        new CacheItemPolicy
                                            {
                                                SlidingExpiration =
                                                    TimeSpan.FromMilliseconds(random.Next(0, 60000))
                                            });
                                    Interlocked.Increment(ref AddedRandomTimeSpan);
                                    break;
                                case 3:
                                    int removeKey = i + LoopCount%3;
                                    // Try remove
                                    object oldCacheItem;
                                    if ((oldCacheItem = cache.Remove(removeKey.ToString())) != null)
                                    {
                                        Assert.AreEqual(removeKey, (int) oldCacheItem);
                                        Interlocked.Increment(ref RemoveSucceeded);
                                    }
                                    else
                                        Interlocked.Increment(ref RemoveFailed);

                                    break;
                            }
                        });
                LoopCount++;
                double loopDurationMs = (DateTime.Now - loopStart).TotalMilliseconds;
                averageLoopMs = ((averageLoopMs*(LoopCount - 1)) + loopDurationMs)/LoopCount;
                Trace.WriteLine(string.Format("#{0} Loop duration - {1} ms", LoopCount, loopDurationMs));
            } while (LoopCount < MaximumLoopCount);

            Trace.Write(
                string.Format(
                    "Completed {2} Loops{0}{1}Added fixed expiry = {3}{0}{1}Added random expiry = {4}{0}{1}Added random lifetime = {5}{0}{1}Remove succeeded = {6}{0}{1}Remove Failed = {7}{0}Check success = {8}{0}{0}Average Loop Duration (ms) = {9}",
                    Environment.NewLine,
                    '\t',
                    LoopCount,
                    AddedFixed,
                    AddedRandomTimeSpan,
                    AddedRandomTimeSpan,
                    RemoveSucceeded,
                    RemoveFailed,
                    CheckSuccess,
                    averageLoopMs));
        }

        [TestMethod]
        //[Ignore]
        public void TestEnhancedMemoryCachePerformance()
        {
            TestCache(new EnhancedMemoryCache<string, int>(), "Enhanced memory cache.");
        }

        [TestMethod]
        //[Ignore]
        public void TestCachingDictionaryPerformance()
        {
            TestCache(new CachingDictionary<string, int>(), "Caching Dictionary.");
        }

        [TestMethod]
        //[Ignore]
        public void TestCachingDictionaryClear()
        {
            Random random = new Random();
            CachingDictionary<int, Guid> cache = new CachingDictionary<int, Guid>();

            // Build cache in parallel
            Parallel.For(
                0,
                CacheSize,
                i => cache.AddOrUpdate(i, Guid.NewGuid(), DateTime.Now.AddMilliseconds(random.Next(1000, 60000))));

            Parallel.Invoke(
                () =>
                    {
                        int count = 0;
// ReSharper disable LoopCanBeConvertedToQuery
                        foreach (Guid guid in cache.Values)
// ReSharper restore LoopCanBeConvertedToQuery
                        {
                            count++;
                        }
                        Trace.WriteLine(string.Format("Count was {0}", count));
                    },
                cache.Clear);

            Trace.WriteLine(cache.ToString());
        }

        private void TestCache(ICaching<string, int> cache, string description)
        {
            AddedFixed = 0;
            AddedRandomExpiry = 0;
            AddedRandomTimeSpan = 0;
            RemoveSucceeded = 0;
            RemoveFailed = 0;
            CheckSuccess = 0;
            LoopCount = 0;

            const int loopSize = CacheSize*4/3;
            double averageLoopMs = 0;
            do
            {
                DateTime loopStart = DateTime.Now;
                DateTime fixedExpiry = loopStart.Add(TimeSpan.FromMinutes(1));
                Parallel.For(
                    0,
                    loopSize,
                    i =>
                        {
                            Random random = new Random();
                            string key = i.ToString();

                            switch (i%4)
                            {
                                case 0:
                                    // Add with fixed expiry
                                    cache.GetOrAdd(key, i, fixedExpiry);
                                    Interlocked.Increment(ref AddedFixed);
                                    break;
                                case 1:
                                    // Add with random expiry
                                    cache.GetOrAdd(key, i, DateTime.Now.AddMilliseconds(random.Next(1000, 60000)));
                                    Interlocked.Increment(ref AddedRandomExpiry);
                                    break;
                                case 2:
                                    cache.GetOrAdd(key, i, TimeSpan.FromMilliseconds(random.Next(1000, 60000)));
                                    Interlocked.Increment(ref AddedRandomTimeSpan);
                                    break;
                                case 3:
                                    int removeKey = i + LoopCount%3;
                                    // Try remove
                                    int oldCacheItem;
                                    if (cache.TryRemove(removeKey.ToString(), out oldCacheItem))
                                    {
                                        Assert.AreEqual(removeKey, oldCacheItem);
                                        Interlocked.Increment(ref RemoveSucceeded);
                                    }
                                    else
                                        Interlocked.Increment(ref RemoveFailed);

                                    break;
                            }
                        });
                LoopCount++;
                double loopDurationMs = (DateTime.Now - loopStart).TotalMilliseconds;
                averageLoopMs = ((averageLoopMs*(LoopCount - 1)) + loopDurationMs)/LoopCount;
                Trace.WriteLine(string.Format("#{0} Loop duration - {1} ms", LoopCount, loopDurationMs));
            } while (LoopCount < MaximumLoopCount);

            Trace.WriteLine(description);
            Trace.Write(
                string.Format(
                    "Completed {2} Loops{0}{1}Added fixed expiry = {3}{0}{1}Added random expiry = {4}{0}{1}Added random lifetime = {5}{0}{1}Remove succeeded = {6}{0}{1}Remove Failed = {7}{0}Check success = {8}{0}{0}Average Loop Duration (ms) = {9}{0}{0}",
                    Environment.NewLine,
                    '\t',
                    LoopCount,
                    AddedFixed,
                    AddedRandomTimeSpan,
                    AddedRandomTimeSpan,
                    RemoveSucceeded,
                    RemoveFailed,
                    CheckSuccess,
                    averageLoopMs));
        }
    }
}