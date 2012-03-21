#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities.Test
// File: TestCombGuid.cs
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
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;

namespace WebApplications.Utilities.Test
{
    [TestClass]
    public class TestCombGuid
    {
        private const int Loops = 10000000;

        [TestMethod]
        public void TestCombGuids()
        {
            const int sampleEvery = Loops/50;
            List<KeyValuePair<DateTime, CombGuid>> combGuids = new List<KeyValuePair<DateTime, CombGuid>>();
            Stopwatch s = new Stopwatch();
            s.Start();
            for (int i = 0; i < 100000; i++)
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

        [TestMethod]
        public void TestGuidPerformance()
        {
            List<Guid> guids = new List<Guid>(Loops);
            Stopwatch s = new Stopwatch();
            s.Start();
            for (int i = 0; i < 100000; i++)
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