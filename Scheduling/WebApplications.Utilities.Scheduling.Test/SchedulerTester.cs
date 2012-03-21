﻿#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: Utilities.Scheduling 
// Project: Utilities.Scheduling.Test
// File: SchedulerTester.cs
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
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Scheduling.Scheduled;
using WebApplications.Utilities.Scheduling.Schedules;

namespace WebApplications.Utilities.Scheduling.Test
{
    public class SchedulerTester
    {
#if false
        private readonly Dictionary<ScheduledFunctionOld<DateTime>, List<TimeSpan>> _tests =
            new Dictionary<ScheduledFunctionOld<DateTime>, List<TimeSpan>>();

        private DateTime _fullStartTime = DateTime.Now;
        private DateTime _startTime = DateTime.Now;

        public static void WaitForStartOfSecond(bool requireEven = false)
        {
            // Wait until the start of a second
            while (DateTime.Now.Millisecond > 100 || (requireEven && DateTime.Now.Second%2 != 0))
            {
                Thread.Sleep(10);
            }
        }

        public ScheduledFunctionOld<DateTime> AddTest(PeriodicSchedule schedule, List<TimeSpan> expectedResults,
                                                   Func<ScheduledFunctionOld<DateTime>, DateTime> function = null)
        {
            return AddTest(SchedulerOLD.Add(schedule, function ?? TestFunction, enabled: false), expectedResults);
        }

        public ScheduledFunctionOld<DateTime> AddTest(ScheduledFunctionOld<DateTime> scheduledFunction,
                                                   List<TimeSpan> expectedResults)
        {
            _tests.Add(scheduledFunction, expectedResults);
            return scheduledFunction;
        }

        public void SetStartTime()
        {
            _fullStartTime = DateTime.Now;
            _startTime = new DateTime(_fullStartTime.Year, _fullStartTime.Month, _fullStartTime.Day, _fullStartTime.Hour,
                                      _fullStartTime.Minute, _fullStartTime.Second);
        }

        public void Start()
        {
            SetStartTime();

            foreach (KeyValuePair<ScheduledFunctionOld<DateTime>, List<TimeSpan>> test in _tests)
            {
                SchedulerOLD.Enable(test.Key);
            }
        }

        public void Stop()
        {
            foreach (KeyValuePair<ScheduledFunctionOld<DateTime>, List<TimeSpan>> test in _tests)
            {
                SchedulerOLD.Disable(test.Key);
            }
        }

        public bool Completed()
        {
            return _tests.All(test => test.Key.History.Count() >= test.Value.Count);
        }

        public void CheckResults()
        {
            int numberOfTestsPassed = 0;
            int testNumber = 1;

            foreach (KeyValuePair<ScheduledFunctionOld<DateTime>, List<TimeSpan>> test in _tests)
            {
                Trace.WriteLine(string.Format("Test #{1} (Start time: {2:hh:mm:ss.ffff} ({3:hh:mm:ss.ffff})):{0}",
                                              Environment.NewLine, testNumber, _startTime, _fullStartTime));

                bool testFailed = false;
                Queue<ScheduledFunctionResult<DateTime>> results =
                    new Queue<ScheduledFunctionResult<DateTime>>(test.Key.History);
                foreach (TimeSpan expectedTimeSpan in test.Value)
                {
                    if (results.Count < 1)
                    {
                        Trace.WriteLine(string.Format("**Not enough results available for test**{0}",
                                                      Environment.NewLine));
                        testFailed = true;
                        break;
                    }

                    ScheduledFunctionResult<DateTime> result = results.Dequeue();
                    bool resultFailed = false;

                    if (expectedTimeSpan == TimeSpan.MinValue)
                    {
                        // If TimeSpan.MinValue, expect immediate execution
                        if (Math.Abs((result.Result - _fullStartTime).TotalSeconds) > 0.10)
                        {
                            testFailed = true;
                            resultFailed = true;
                        }

                        OutputResult(result, _fullStartTime, TimeSpan.Zero, resultFailed);
                    }
                    else
                    {
                        TimeSpan resultantTimeSpan = result.Result - _startTime;

                        if (Math.Abs((resultantTimeSpan - expectedTimeSpan).TotalSeconds) > 0.50)
                        {
                            testFailed = true;
                            resultFailed = true;
                        }

                        OutputResult(result, _startTime, expectedTimeSpan, resultFailed);
                    }
                }

                if (results.Count > 0)
                {
                    Trace.WriteLine(string.Format("**Extra Results: ({1})**{0}", Environment.NewLine, results.Count));
                    testFailed = true;

                    foreach (ScheduledFunctionResult<DateTime> extraResult in results)
                    {
                        OutputResult(extraResult);
                    }
                }
                if (!testFailed)
                {
                    numberOfTestsPassed++;
                    Trace.WriteLine(string.Format("**Test Passed**{0}", Environment.NewLine));
                }
                else
                {
                    Trace.WriteLine(string.Format("**Test Failed**{0}", Environment.NewLine));
                }
                testNumber++;
            }

            if (numberOfTestsPassed == _tests.Count)
            {
                Trace.WriteLine(string.Format("All tests passed ({1} test(s)){0}", Environment.NewLine,
                                              numberOfTestsPassed));
            }
            else
            {
                Assert.Fail("{0} passed, {1} failed", numberOfTestsPassed, _tests.Count - numberOfTestsPassed);
            }
        }

        public void Run()
        {
            Start();
            while (!Completed())
            {
                Thread.Sleep(10);
            }
            Stop();

            CheckResults();
        }

        public static DateTime TestFunction(ScheduledFunctionOld<DateTime> scheduledFunction)
        {
            return DateTime.Now;
        }

        public static void OutputResult(ScheduledFunctionResult<DateTime> result)
        {
            Trace.WriteLine(
                string.Format(
                    "Result:\t\t{1:hh:mm:ss.ffff}{0}Due:\t\t{2:hh:mm:ss.ffff}{0}Started:\t{3:ss.ffff}{0}Duration:\t{4}ms{0}",
                    Environment.NewLine, result.Result, result.Due,
                    result.Started, result.Duration.TotalMilliseconds));
        }

        public static void OutputResult(ScheduledFunctionResult<DateTime> result, DateTime startTime,
                                        TimeSpan expectedTimeSpan, bool failed = false)
        {
            Trace.WriteLine(
                string.Format(
                    "Result:\t\t{1:hh:mm:ss.ffff}{0}Expected:\t{2:hh:mm:ss.ffff}{0}Expected Time Span:\t{3}s{0}Due:\t\t{4:hh:mm:ss.ffff}{0}Started:\t{5:ss.ffff}{0}Duration:\t{6}ms{7}{0}",
                    Environment.NewLine, result.Result, startTime + expectedTimeSpan, expectedTimeSpan.TotalSeconds,
                    result.Due,
                    result.Started, result.Duration.TotalMilliseconds,
                    failed ? string.Format("{0}**Failed**", Environment.NewLine) : string.Empty));
        }
#endif
    }
}