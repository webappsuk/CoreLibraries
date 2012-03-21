#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: Utilities.Scheduling 
// Project: Utilities.Scheduling.Test
// File: TestScheduler.cs
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
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Scheduling.Scheduled;
using WebApplications.Utilities.Scheduling.Schedules;

namespace WebApplications.Utilities.Scheduling.Test
{
    [TestClass]
    public class TestScheduler
    {
#if false
        [TestMethod]
        public void TestSeconds()
        {
            SchedulerTester.WaitForStartOfSecond();

            SchedulerTester schedulerTester = new SchedulerTester();

            PeriodicSchedule schedule = new PeriodicSchedule(hour: Hour.Every, minute: Minute.Every, second: Second.Every);
            List<TimeSpan> expectedResults = new List<TimeSpan>
                                                 {
                                                     new TimeSpan(0, 0, 1),
                                                     new TimeSpan(0, 0, 2),
                                                     new TimeSpan(0, 0, 3),
                                                     new TimeSpan(0, 0, 4),
                                                     new TimeSpan(0, 0, 5),
                                                     new TimeSpan(0, 0, 6),
                                                     new TimeSpan(0, 0, 7),
                                                     new TimeSpan(0, 0, 8),
                                                     new TimeSpan(0, 0, 9)
                                                 };
            schedulerTester.AddTest(schedule, expectedResults);

            schedulerTester.Run();
        }

        [TestMethod]
        public void TestTwoSeconds()
        {
            SchedulerTester.WaitForStartOfSecond(true);

            SchedulerTester schedulerTester = new SchedulerTester();

            PeriodicSchedule schedule = new PeriodicSchedule(hour: Hour.Every, minute: Minute.Every, second: Second.EveryTwoSeconds);
            List<TimeSpan> expectedResults = new List<TimeSpan>
                                                 {
                                                     new TimeSpan(0, 0, 2),
                                                     new TimeSpan(0, 0, 4),
                                                     new TimeSpan(0, 0, 6),
                                                     new TimeSpan(0, 0, 8),
                                                     new TimeSpan(0, 0, 10),
                                                     new TimeSpan(0, 0, 12),
                                                     new TimeSpan(0, 0, 14),
                                                     new TimeSpan(0, 0, 16),
                                                     new TimeSpan(0, 0, 18)
                                                 };
            schedulerTester.AddTest(schedule, expectedResults);

            schedulerTester.Run();
        }

        [TestMethod]
        public void TestDoubleSchedule()
        {
            SchedulerTester.WaitForStartOfSecond(true);

            SchedulerTester schedulerTester = new SchedulerTester();

            PeriodicSchedule schedule = new PeriodicSchedule(hour: Hour.Every, minute: Minute.Every, second: Second.Every);
            List<TimeSpan> expectedResults = new List<TimeSpan>
                                                 {
                                                     new TimeSpan(0, 0, 1),
                                                     new TimeSpan(0, 0, 2),
                                                     new TimeSpan(0, 0, 3),
                                                     new TimeSpan(0, 0, 4),
                                                     new TimeSpan(0, 0, 5),
                                                     new TimeSpan(0, 0, 6),
                                                     new TimeSpan(0, 0, 7),
                                                     new TimeSpan(0, 0, 8),
                                                     new TimeSpan(0, 0, 9)
                                                 };
            schedulerTester.AddTest(schedule, expectedResults);

            schedule = new PeriodicSchedule(hour: Hour.Every, minute: Minute.Every, second: Second.EveryTwoSeconds);
            expectedResults = new List<TimeSpan>
                                  {
                                      new TimeSpan(0, 0, 2),
                                      new TimeSpan(0, 0, 4),
                                      new TimeSpan(0, 0, 6),
                                      new TimeSpan(0, 0, 8)
                                  };
            schedulerTester.AddTest(schedule, expectedResults);

            schedulerTester.Run();
        }

        [TestMethod]
        public void TestSharedSchedule()
        {
            SchedulerTester.WaitForStartOfSecond(true);

            SchedulerTester schedulerTester = new SchedulerTester();

            PeriodicSchedule schedule = new PeriodicSchedule(hour: Hour.Every, minute: Minute.Every, second: Second.Every);
            List<TimeSpan> expectedResults = new List<TimeSpan>
                                                 {
                                                     new TimeSpan(0, 0, 1),
                                                     new TimeSpan(0, 0, 2),
                                                     new TimeSpan(0, 0, 3),
                                                     new TimeSpan(0, 0, 4),
                                                     new TimeSpan(0, 0, 5),
                                                     new TimeSpan(0, 0, 6),
                                                     new TimeSpan(0, 0, 7),
                                                     new TimeSpan(0, 0, 8),
                                                     new TimeSpan(0, 0, 9)
                                                 };
            schedulerTester.AddTest(schedule, expectedResults);

            expectedResults = new List<TimeSpan>
                                  {
                                      new TimeSpan(0, 0, 1),
                                      new TimeSpan(0, 0, 2),
                                      new TimeSpan(0, 0, 3),
                                      new TimeSpan(0, 0, 4),
                                      new TimeSpan(0, 0, 5),
                                      new TimeSpan(0, 0, 6),
                                      new TimeSpan(0, 0, 7),
                                      new TimeSpan(0, 0, 8),
                                      new TimeSpan(0, 0, 9)
                                  };
            schedulerTester.AddTest(schedule, expectedResults);

            schedulerTester.Run();
        }

        [TestMethod]
        public void TestTwoSchedulers()
        {
            SchedulerTester.WaitForStartOfSecond();

            SchedulerTester schedulerTester = new SchedulerTester();

            PeriodicSchedule schedule = new PeriodicSchedule(hour: Hour.Every, minute: Minute.Every, second: Second.Every);
            List<TimeSpan> expectedResults = new List<TimeSpan>
                                                 {
                                                     new TimeSpan(0, 0, 1),
                                                     new TimeSpan(0, 0, 2),
                                                     new TimeSpan(0, 0, 3),
                                                     new TimeSpan(0, 0, 4),
                                                     new TimeSpan(0, 0, 5),
                                                     new TimeSpan(0, 0, 6),
                                                     new TimeSpan(0, 0, 7),
                                                     new TimeSpan(0, 0, 8),
                                                     new TimeSpan(0, 0, 9)
                                                 };
            schedulerTester.AddTest(schedule, expectedResults);

            schedule = new PeriodicSchedule(hour: Hour.Every, minute: Minute.Every, second: Second.Every);
            ScheduledFunctionOld<int> scheduledFunction = SchedulerOLD.Add<int>(schedule,
                                                                          function =>
                                                                          function.History.Count() > 0
                                                                              ? function.History.Last().Result + 1
                                                                              : 1, enabled: false);

            SchedulerOLD.Enable(scheduledFunction);
            schedulerTester.Run();
            SchedulerOLD.Disable(scheduledFunction);

            Queue<ScheduledFunctionResult<int>> results =
                new Queue<ScheduledFunctionResult<int>>(scheduledFunction.History);
            int resultCount = 0;
            while (results.Count > 0)
            {
                resultCount++;

                if (resultCount > 9)
                {
                    Assert.Fail("ScheduledFunction<int>: Too many results returned");
                    break;
                }

                Assert.AreEqual(resultCount, results.Dequeue().Result);
            }

            if (resultCount < 9)
            {
                Assert.Fail("ScheduledFunction<int>: Not enough results returned");
            }
        }

        [TestMethod]
        public void TestFunctionSlow()
        {
            SchedulerTester.WaitForStartOfSecond();

            SchedulerTester schedulerTester = new SchedulerTester();

            PeriodicSchedule schedule = new PeriodicSchedule(hour: Hour.Every, minute: Minute.Every, second: Second.Every);
            List<TimeSpan> expectedResults = new List<TimeSpan>
                                                 {
                                                     new TimeSpan(0, 0, 1),
                                                     new TimeSpan(0, 0, 3),
                                                     new TimeSpan(0, 0, 5),
                                                     new TimeSpan(0, 0, 7),
                                                     new TimeSpan(0, 0, 9)
                                                 };
            schedulerTester.AddTest(schedule, expectedResults, delegate
                                                                   {
                                                                       DateTime resultTime = DateTime.Now;
                                                                       Thread.Sleep(1500);
                                                                       return resultTime;
                                                                   });

            schedulerTester.Run();
        }

        [TestMethod]
        public void TestFunctionPause()
        {
            SchedulerTester.WaitForStartOfSecond();

            SchedulerTester schedulerTester = new SchedulerTester();

            PeriodicSchedule schedule = new PeriodicSchedule(hour: Hour.Every, minute: Minute.Every, second: Second.Every);
            List<TimeSpan> expectedResults = new List<TimeSpan>
                                                 {
                                                     new TimeSpan(0, 0, 1),
                                                     new TimeSpan(0, 0, 3),
                                                     new TimeSpan(0, 0, 5),
                                                     new TimeSpan(0, 0, 7),
                                                     new TimeSpan(0, 0, 9)
                                                 };
            schedulerTester.AddTest(schedule, expectedResults, delegate
                                                                   {
                                                                       DateTime resultTime = DateTime.Now;
                                                                       SchedulerOLD.DisableAll();
                                                                       Thread.Sleep(1500);
                                                                       SchedulerOLD.EnableAll();
                                                                       return resultTime;
                                                                   });

            schedulerTester.Run();
        }

        [TestMethod]
        public void TestFunctionAdd()
        {
            SchedulerTester.WaitForStartOfSecond();

            SchedulerTester schedulerTester1 = new SchedulerTester();
            SchedulerTester schedulerTester2 = null;

            PeriodicSchedule schedule = new PeriodicSchedule(hour: Hour.Every, minute: Minute.Every, second: Second.Every);
            List<TimeSpan> expectedResults = new List<TimeSpan>
                                                 {
                                                     new TimeSpan(0, 0, 1),
                                                     new TimeSpan(0, 0, 2)
                                                 };
            schedulerTester1.AddTest(schedule, expectedResults, delegate
                                                                    {
                                                                        DateTime resultTime = DateTime.Now;
                                                                        if (schedulerTester2 == null)
                                                                        {
                                                                            schedulerTester2 = new SchedulerTester();

                                                                            schedule = new PeriodicSchedule(hour: Hour.Every,
                                                                                                    minute: Minute.Every,
                                                                                                    second: Second.Every);
                                                                            expectedResults = new List<TimeSpan>
                                                                                                  {
                                                                                                      new TimeSpan(0, 0,
                                                                                                                   1),
                                                                                                      new TimeSpan(0, 0,
                                                                                                                   2),
                                                                                                      new TimeSpan(0, 0,
                                                                                                                   3),
                                                                                                      new TimeSpan(0, 0,
                                                                                                                   4)
                                                                                                  };
                                                                            schedulerTester2.AddTest(schedule,
                                                                                                     expectedResults);

                                                                            schedulerTester2.Start();
                                                                        }
                                                                        return resultTime;
                                                                    });

            schedulerTester1.Run();

            while (schedulerTester2 == null || !schedulerTester2.Completed())
            {
                Thread.Sleep(10);
            }
            schedulerTester2.Stop();

            schedulerTester2.CheckResults();
        }

        [TestMethod]
        public void TestDisable()
        {
            SchedulerTester.WaitForStartOfSecond();

            SchedulerTester schedulerTester = new SchedulerTester();

            PeriodicSchedule schedule = new PeriodicSchedule(hour: Hour.Every, minute: Minute.Every, second: Second.Every);
            List<TimeSpan> expectedResults = new List<TimeSpan>
                                                 {
                                                     new TimeSpan(0, 0, 1),
                                                     new TimeSpan(0, 0, 2),
                                                     new TimeSpan(0, 0, 3),
                                                     new TimeSpan(0, 0, 4),
                                                     new TimeSpan(0, 0, 5),
                                                     new TimeSpan(0, 0, 6),
                                                     new TimeSpan(0, 0, 7),
                                                     new TimeSpan(0, 0, 8),
                                                     new TimeSpan(0, 0, 9)
                                                 };
            schedulerTester.AddTest(schedule, expectedResults);

            schedule = new PeriodicSchedule(hour: Hour.Every, minute: Minute.Every, second: Second.Every);
            expectedResults = new List<TimeSpan>
                                  {
                                      new TimeSpan(0, 0, 6),
                                      new TimeSpan(0, 0, 7),
                                      new TimeSpan(0, 0, 8),
                                      new TimeSpan(0, 0, 9)
                                  };
            ScheduledFunctionOld<DateTime> scheduledFunction = schedulerTester.AddTest(schedule, expectedResults);

            schedulerTester.Start();

            Thread.Sleep(200);
            SchedulerOLD.Disable(scheduledFunction);
            Thread.Sleep(6000);
            SchedulerOLD.Enable(scheduledFunction);

            while (!schedulerTester.Completed())
            {
                Thread.Sleep(10);
            }
            schedulerTester.Stop();

            schedulerTester.CheckResults();
        }

        [TestMethod]
        public void TestRemove()
        {
            SchedulerTester.WaitForStartOfSecond();

            SchedulerTester schedulerTester = new SchedulerTester();

            PeriodicSchedule schedule = new PeriodicSchedule(hour: Hour.Every, minute: Minute.Every, second: Second.Every);
            List<TimeSpan> expectedResults = new List<TimeSpan>
                                                 {
                                                     new TimeSpan(0, 0, 1),
                                                     new TimeSpan(0, 0, 2),
                                                     new TimeSpan(0, 0, 3),
                                                     new TimeSpan(0, 0, 4),
                                                     new TimeSpan(0, 0, 5),
                                                     new TimeSpan(0, 0, 6),
                                                     new TimeSpan(0, 0, 7),
                                                     new TimeSpan(0, 0, 8),
                                                     new TimeSpan(0, 0, 9)
                                                 };
            schedulerTester.AddTest(schedule, expectedResults);

            schedule = new PeriodicSchedule(hour: Hour.Every, minute: Minute.Every, second: Second.Every);
            expectedResults = new List<TimeSpan>
                                  {
                                      new TimeSpan(0, 0, 1)
                                  };
            ScheduledFunctionOld<DateTime> scheduledFunction = schedulerTester.AddTest(schedule, expectedResults);

            schedulerTester.Start();

            Thread.Sleep(1500);
            SchedulerOLD.Remove(scheduledFunction);

            while (!schedulerTester.Completed())
            {
                Thread.Sleep(10);
            }
            schedulerTester.Stop();

            schedulerTester.CheckResults();
        }

        [TestMethod]
        public void TestStartAfter()
        {
            SchedulerTester.WaitForStartOfSecond();

            SchedulerTester schedulerTester = new SchedulerTester();

            DateTime startAfterTime = DateTime.Now.AddSeconds(4);

            PeriodicSchedule schedule = new PeriodicSchedule(hour: Hour.Every, minute: Minute.Every, second: Second.Every);
            List<TimeSpan> expectedResults = new List<TimeSpan>
                                                 {
                                                     new TimeSpan(0, 0, 5),
                                                     new TimeSpan(0, 0, 6),
                                                     new TimeSpan(0, 0, 7),
                                                     new TimeSpan(0, 0, 8),
                                                     new TimeSpan(0, 0, 9)
                                                 };
            schedulerTester.AddTest(
                SchedulerOLD.Add<DateTime>(schedule, SchedulerTester.TestFunction, enabled: false,
                                        startAfter: startAfterTime), expectedResults);

            schedulerTester.Run();
        }

        [TestMethod]
        public void TestStartEnabled()
        {
            SchedulerTester.WaitForStartOfSecond();

            SchedulerTester schedulerTester = new SchedulerTester();
            PeriodicSchedule schedule = new PeriodicSchedule(hour: Hour.Every, minute: Minute.Every, second: Second.Every);
            List<TimeSpan> expectedResults = new List<TimeSpan>
                                                 {
                                                     new TimeSpan(0, 0, 1),
                                                     new TimeSpan(0, 0, 2),
                                                     new TimeSpan(0, 0, 3),
                                                     new TimeSpan(0, 0, 4),
                                                     new TimeSpan(0, 0, 5),
                                                     new TimeSpan(0, 0, 6),
                                                     new TimeSpan(0, 0, 7),
                                                     new TimeSpan(0, 0, 8),
                                                     new TimeSpan(0, 0, 9)
                                                 };

            schedulerTester.SetStartTime();

            schedulerTester.AddTest(SchedulerOLD.Add<DateTime>(schedule, SchedulerTester.TestFunction, enabled: true),
                                    expectedResults);

            while (!schedulerTester.Completed())
            {
                Thread.Sleep(10);
            }
            schedulerTester.Stop();

            schedulerTester.CheckResults();
        }

        [TestMethod]
        public void TestExecuteImmediately1()
        {
            SchedulerTester.WaitForStartOfSecond(true);

            SchedulerTester schedulerTester = new SchedulerTester();
            PeriodicSchedule schedule = new PeriodicSchedule(hour: Hour.Every, minute: Minute.Every, second: Second.EveryOtherSecond);
            List<TimeSpan> expectedResults = new List<TimeSpan>
                                                 {
                                                     new TimeSpan(0, 0, 1),
                                                     new TimeSpan(0, 0, 3),
                                                     new TimeSpan(0, 0, 5),
                                                     new TimeSpan(0, 0, 7),
                                                     new TimeSpan(0, 0, 9)
                                                 };
            schedulerTester.AddTest(
                SchedulerOLD.Add<DateTime>(schedule, SchedulerTester.TestFunction, enabled: false,
                                        executeImmediately: false), expectedResults);

            schedulerTester.Run();
        }

        [TestMethod]
        public void TestExecuteImmediately2()
        {
            SchedulerTester.WaitForStartOfSecond(true);

            SchedulerTester schedulerTester = new SchedulerTester();
            PeriodicSchedule schedule = new PeriodicSchedule(hour: Hour.Every, minute: Minute.Every, second: Second.EveryOtherSecond);
            List<TimeSpan> expectedResults = new List<TimeSpan>
                                                 {
                                                     TimeSpan.MinValue,
                                                     // Immediate execution
                                                     new TimeSpan(0, 0, 1),
                                                     new TimeSpan(0, 0, 3),
                                                     new TimeSpan(0, 0, 5),
                                                     new TimeSpan(0, 0, 7),
                                                     new TimeSpan(0, 0, 9)
                                                 };
            schedulerTester.AddTest(
                SchedulerOLD.Add<DateTime>(schedule, SchedulerTester.TestFunction, enabled: false, executeImmediately: true),
                expectedResults);

            schedulerTester.Run();
        }

        [TestMethod]
        public void TestMinimumGap1()
        {
            SchedulerTester.WaitForStartOfSecond();

            SchedulerTester schedulerTester = new SchedulerTester();
            PeriodicSchedule schedule = new PeriodicSchedule(hour: Hour.Every, minute: Minute.Every, second: Second.Every,
                                             minimumGap: TimeSpan.FromSeconds(2));
            List<TimeSpan> expectedResults = new List<TimeSpan>
                                                 {
                                                     new TimeSpan(0, 0, 3),
                                                     new TimeSpan(0, 0, 6),
                                                     new TimeSpan(0, 0, 9),
                                                     new TimeSpan(0, 0, 12),
                                                     new TimeSpan(0, 0, 15)
                                                 };
            schedulerTester.AddTest(schedule, expectedResults);

            schedulerTester.Run();
        }

        [TestMethod]
        public void TestMinimumGap2()
        {
            SchedulerTester.WaitForStartOfSecond();

            SchedulerTester schedulerTester = new SchedulerTester();
            PeriodicSchedule schedule = new PeriodicSchedule(hour: Hour.Every, minute: Minute.Every, second: Second.Every,
                                             minimumGap: TimeSpan.FromSeconds(2));
            List<TimeSpan> expectedResults = new List<TimeSpan>
                                                 {
                                                     TimeSpan.MinValue,
                                                     // Immediate execution
                                                     new TimeSpan(0, 0, 3),
                                                     new TimeSpan(0, 0, 6),
                                                     new TimeSpan(0, 0, 9),
                                                     new TimeSpan(0, 0, 12),
                                                     new TimeSpan(0, 0, 15)
                                                 };
            schedulerTester.AddTest(
                SchedulerOLD.Add<DateTime>(schedule, SchedulerTester.TestFunction, enabled: false, executeImmediately: true),
                expectedResults);

            schedulerTester.Run();
        }
#endif
    }
}