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
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;

namespace WebApplications.Utilities.Test
{
    [TestClass]
    public class TestCombGuid
    {
        private const int Loops = 100;

        [Ignore] // Fails due to memory issues. TODO: Delete test (after writing unit tests to do the same thing)
        [TestMethod]
        public void TestCombGuids()
        {
            const int sampleEvery = Loops/50;
            List<KeyValuePair<DateTime, CombGuid>> combGuids = new List<KeyValuePair<DateTime, CombGuid>>();
            Stopwatch s = new Stopwatch();
            s.Start();
            for (int i = 0; i < 100; i++)
            {
                DateTime now = DateTime.Now;
                combGuids.Add(new KeyValuePair<DateTime, CombGuid>(now, CombGuid.NewCombGuid(now)));
            }
            s.Stop();
            Trace.WriteLine(s.ToString("Creating {0} CombGuids", Loops));
            List<CombGuid> seen = new List<CombGuid>(Loops);
            CombGuid last = CombGuid.Empty;
            int count = 0;
            TimeSpan maxDelta = TimeSpan.Zero;
            foreach (KeyValuePair<DateTime, CombGuid> kvp in combGuids)
            {
                // Check unique
                Assert.IsFalse(seen.Contains(kvp.Value), "Duplicate CombGuid {0} found", kvp.Value);
                Assert.IsTrue(kvp.Value >= last, "CombGuid {0} out of order", kvp.Value);

                TimeSpan delta = kvp.Key - kvp.Value.Guid.GetDateTime();
                if (delta < TimeSpan.Zero)
                    delta = TimeSpan.Zero - delta;

                if (delta > maxDelta)
                    maxDelta = delta;

                Assert.IsTrue(delta < TimeSpan.FromMilliseconds(4), "The loss of precision was greater than 4ms - {0}",
                              delta.TotalMilliseconds);

                if (count++%sampleEvery == 0)
                    Trace.WriteLine(string.Format("{0} - {1}", kvp.Key, kvp.Value));
            }
            Trace.WriteLine(string.Format("The maximum delta was {0}ms.", maxDelta.TotalMilliseconds));
        }

        [Ignore] // This is not a test. Also, it breaks.
        [TestMethod]
        public void TestGuidPerformance()
        {
            List<Guid> guids = new List<Guid>(Loops);
            Stopwatch s = new Stopwatch();
            s.Start();
            for (int i = 0; i < 100; i++)
                guids.Add(Guid.NewGuid());
            s.Stop();
            Trace.WriteLine(s.ToString("Creating {0} Guids", Loops));
        }

        [TestMethod]
        public void TestSqlCombGuid()
        {
            // The following comb guids were created in SQL using
            // CAST(CAST(newid() AS BINARY(10)) + CAST(@Mydate AS BINARY(6)) AS UNIQUEIDENTIFIER)
            Dictionary<DateTime, CombGuid> combGuids = new Dictionary<DateTime, CombGuid>
                                                           {
                                                               {
                                                                   DateTime.Parse("2011-08-24 21:01:35.647"),
                                                                   CombGuid.Parse("251E6F52-490F-41B1-8BF1-9F49015A81D6")
                                                               },
                                                               {
                                                                   DateTime.Parse("2011-08-24 21:03:05.300"),
                                                                   CombGuid.Parse("CCF545D5-37D2-40C9-B5F5-9F49015AEAE6")
                                                               },
                                                               {
                                                                   DateTime.Parse("2011-08-24 21:04:00.513"),
                                                                   CombGuid.Parse("7430E43E-98BA-4374-9231-9F49015B2B9A")
                                                               },
                                                               {
                                                                   DateTime.Parse("2011-08-24 21:08:40.957"),
                                                                   CombGuid.Parse("66AF19E4-E72C-476F-A921-9F49015C743F")
                                                               },
                                                           };

            TimeSpan maxDelta = TimeSpan.Zero;
            foreach (KeyValuePair<DateTime, CombGuid> kvp in combGuids)
            {
                TimeSpan delta = kvp.Value.Created - kvp.Key;
                if (delta < TimeSpan.Zero)
                    delta = TimeSpan.Zero - delta;

                if (delta > maxDelta)
                    maxDelta = delta;

                // Theoretically we should never get more than 3.3333ms delta (but it can due to rounding get a little higher up to about 3.7ms)
                Assert.IsTrue(delta < TimeSpan.FromMilliseconds(10), "The loss of precision was greater than 10ms - {0}",
                              delta.TotalMilliseconds);
            }

            Trace.WriteLine(string.Format("The maximum delta for SQL CombGuids was {0}ms.", maxDelta.TotalMilliseconds));
        }
    }
}