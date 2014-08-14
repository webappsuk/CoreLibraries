#region © Copyright Web Applications (UK) Ltd, 2014.  All rights reserved.
// Copyright (c) 2014, Web Applications UK Ltd
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

using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using WebApplications.Utilities.Scheduling.Scheduled;
using WebApplications.Utilities.Scheduling.Schedules;

namespace WebApplications.Utilities.Scheduling.Test
{
    [TestClass]
    public class TestScheduler
    {
        [TestMethod]
        public void TestAddActionRuns()
        {
            bool ran = false;
            Instant due = Scheduler.Clock.Now + Duration.FromMilliseconds(100);
            ScheduledAction action = Scheduler.Add(() => { ran = true; }, new OneOffSchedule(due));

            Assert.IsNotNull(action);
            Assert.IsTrue(action.Enabled);
            Assert.AreEqual(Scheduler.DefaultMaximumHistory, action.MaximumHistory);
            Assert.AreEqual(Scheduler.DefaultMaximumDuration, action.MaximumDuration);
            Thread.Sleep(1000);
            Assert.IsTrue(ran);
            Assert.AreEqual(1, action.History.Count());

            ScheduledActionResult history = action.History.FirstOrDefault();
            Assert.IsNotNull(history);
            Assert.IsFalse(history.Cancelled);
            Assert.IsNull(history.Exception);
            Assert.AreEqual(due, history.Due);
            Assert.IsTrue(history.Due <= history.Started);
        }

        [TestMethod]
        public void TestAddActionInPastDoesntRun()
        {
            bool ran = false;
            Instant due = Scheduler.Clock.Now - Duration.FromMilliseconds(1);
            ScheduledAction action = Scheduler.Add(() => { ran = true; }, new OneOffSchedule(due));

            Assert.IsNotNull(action);
            Assert.IsTrue(action.Enabled);
            Assert.AreEqual(Scheduler.DefaultMaximumHistory, action.MaximumHistory);
            Assert.AreEqual(Scheduler.DefaultMaximumDuration, action.MaximumDuration);
            Thread.Sleep(1000);
            Assert.IsFalse(ran);
            Assert.AreEqual(0, action.History.Count());
            Assert.AreEqual(Instant.MaxValue, action.NextDue);
        }

        [TestMethod]
        public void TestAddActionDisabledDoesntRun()
        {
            bool ran = false;
            Instant due = Scheduler.Clock.Now - Duration.FromMilliseconds(1);
            ScheduledAction action = Scheduler.Add(() => { ran = true; }, new OneOffSchedule(due));
            Assert.IsNotNull(action);
            
            action.Enabled = false;
            Assert.IsFalse(action.Enabled);
            
            Assert.AreEqual(Scheduler.DefaultMaximumHistory, action.MaximumHistory);
            Assert.AreEqual(Scheduler.DefaultMaximumDuration, action.MaximumDuration);
            Thread.Sleep(1000);
            Assert.IsFalse(ran);
            Assert.AreEqual(0, action.History.Count());
            Assert.AreEqual(Instant.MaxValue, action.NextDue);
        }

        [TestMethod]
        public void TestAddActionCancel()
        {
            bool ran = false;
            Instant due = Scheduler.Clock.Now + Duration.FromMilliseconds(100);
            Duration duration = Duration.FromMilliseconds(1);
            ScheduledAction action = Scheduler.Add(
                () =>
                {
                    ran = true;
                    Thread.Sleep(10);
                },
                new OneOffSchedule(due),
                maximumDuration: duration);

            Assert.IsNotNull(action);
            Assert.IsTrue(action.Enabled);
            Assert.AreEqual(Scheduler.DefaultMaximumHistory, action.MaximumHistory);
            Assert.AreEqual(duration, action.MaximumDuration);
            Thread.Sleep(1000);
            Assert.IsTrue(ran);
            Assert.AreEqual(1, action.History.Count());

            ScheduledActionResult history = action.History.FirstOrDefault();
            Assert.IsNotNull(history);
            Assert.IsTrue(history.Cancelled);
            Assert.IsNull(history.Exception);
            Assert.AreEqual(due, history.Due);
            Assert.IsTrue(history.Due <= history.Started);
        }

#if false
        [TestMethod]
        public void TestSeconds()
        {
            SchedulerTester schedulerTester = new SchedulerTester();

            PeriodicSchedule schedule = new PeriodicSchedule(
                hour: Hour.Every,
                minute: Minute.Every,
                second: Second.Every);
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

            SchedulerTester.WaitForStartOfSecond();
            schedulerTester.Run();
        }

        [TestMethod]
        public void TestTwoSeconds()
        {
            SchedulerTester.WaitForStartOfSecond(true);

            SchedulerTester schedulerTester = new SchedulerTester();

            PeriodicSchedule schedule = new PeriodicSchedule(
                hour: Hour.Every,
                minute: Minute.Every,
                second: Second.EveryTwoSeconds);
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
            SchedulerTester schedulerTester = new SchedulerTester();

            PeriodicSchedule schedule = new PeriodicSchedule(
                hour: Hour.Every,
                minute: Minute.Every,
                second: Second.Every);
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

            SchedulerTester.WaitForStartOfSecond(true);
            schedulerTester.Run();
        }

        [TestMethod]
        public void TestSharedSchedule()
        {
            SchedulerTester.WaitForStartOfSecond(true);

            SchedulerTester schedulerTester = new SchedulerTester();

            PeriodicSchedule schedule = new PeriodicSchedule(
                hour: Hour.Every,
                minute: Minute.Every,
                second: Second.Every);
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

#if false

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
            IScheduledFunction<int> scheduledFunction = Scheduler.Add<int>(schedule,
                                                                          function =>
                                                                          function.History.Count() > 0
                                                                              ? function.History.Last().Result + 1
                                                                              : 1, enabled: false);

            Scheduler.Enable(scheduledFunction);
            schedulerTester.Run();
            Scheduler.Disable(scheduledFunction);

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
                                                                       DateTime resultTime = DateTime.UtcNow;
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
                                                                       DateTime resultTime = DateTime.UtcNow;
                                                                       Scheduler.DisableAll();
                                                                       Thread.Sleep(1500);
                                                                       Scheduler.EnableAll();
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
                                                                        DateTime resultTime = DateTime.UtcNow;
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
            IScheduledFunction<DateTime> scheduledFunction = schedulerTester.AddTest(schedule, expectedResults);

            schedulerTester.Start();

            Thread.Sleep(200);
            Scheduler.Disable(scheduledFunction);
            Thread.Sleep(6000);
            Scheduler.Enable(scheduledFunction);

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
            IScheduledFunction<DateTime> scheduledFunction = schedulerTester.AddTest(schedule, expectedResults);

            schedulerTester.Start();

            Thread.Sleep(1500);
            Scheduler.Remove(scheduledFunction);

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

            DateTime startAfterTime = DateTime.UtcNow.AddSeconds(4);

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
                Scheduler.Add<DateTime>(schedule, SchedulerTester.TestFunction, enabled: false,
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

            schedulerTester.AddTest(Scheduler.Add<DateTime>(schedule, SchedulerTester.TestFunction, enabled: true),
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
                Scheduler.Add<DateTime>(schedule, SchedulerTester.TestFunction, enabled: false,
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
                Scheduler.Add<DateTime>(schedule, SchedulerTester.TestFunction, enabled: false, executeImmediately: true),
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
                Scheduler.Add<DateTime>(schedule, SchedulerTester.TestFunction, enabled: false, executeImmediately: true),
                expectedResults);

            schedulerTester.Run();
        }
#endif
#endif
    }
}