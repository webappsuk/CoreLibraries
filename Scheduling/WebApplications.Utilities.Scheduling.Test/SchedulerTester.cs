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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Scheduling.Schedulable;

namespace WebApplications.Utilities.Scheduling.Test
{
    public class SchedulerTester
    {
        [NotNull]
        private readonly Dictionary<IScheduledFunction<DateTime>, IEnumerable<TimeSpan>> _tests =
            new Dictionary<IScheduledFunction<DateTime>, IEnumerable<TimeSpan>>();

        [NotNull]
        private readonly Scheduler _scheduler = new Scheduler();

        private DateTime _fullStartTime = DateTime.Now;
        private DateTime _startTime = DateTime.Now;

        public SchedulerTester()
        {
            _scheduler.Enabled = false;
        }

        public static void WaitForStartOfSecond(bool requireEven = false)
        {
            // Wait until the start of a second
            while (DateTime.Now.Millisecond > 100 ||
                   (requireEven && DateTime.Now.Second % 2 != 0))
                Thread.Sleep(10);
        }

        public IScheduledFunction<DateTime> AddTest(
            [NotNull] ISchedule schedule,
            [NotNull] IEnumerable<TimeSpan> expectedResults,
            ISchedulableFunction<DateTime> function = null)
        {
            return AddTest(_scheduler.Add(schedule, function ?? TestFunction), expectedResults);
        }

        public IScheduledFunction<DateTime> AddTest(
            [NotNull] IScheduledFunction<DateTime> scheduledFunction,
            [NotNull] IEnumerable<TimeSpan> expectedResults)
        {
            _tests.Add(scheduledFunction, expectedResults);
            return scheduledFunction;
        }

        public void SetStartTime()
        {
            _fullStartTime = DateTime.Now;
            _startTime = new DateTime(
                _fullStartTime.Year,
                _fullStartTime.Month,
                _fullStartTime.Day,
                _fullStartTime.Hour,
                _fullStartTime.Minute,
                _fullStartTime.Second);
        }

        public void Start()
        {
            SetStartTime();
            _scheduler.Enabled = true;
        }

        public void Stop()
        {
            _scheduler.Enabled = false;
        }

        public bool Completed()
        {
            return _tests.All(test => test.Key.History.Count() >= test.Value.Count());
        }

        public void CheckResults()
        {
            int numberOfTestsPassed = 0;
            int testNumber = 1;

            foreach (KeyValuePair<IScheduledFunction<DateTime>, IEnumerable<TimeSpan>> test in _tests)
            {
                Trace.WriteLine(
                    string.Format(
                        "Test #{1} (Start time: {2:hh:mm:ss.ffff} ({3:hh:mm:ss.ffff})):{0}",
                        Environment.NewLine,
                        testNumber,
                        _startTime,
                        _fullStartTime));

                bool testFailed = false;
                Queue<IScheduledFunctionResult<DateTime>> results =
                    new Queue<IScheduledFunctionResult<DateTime>>(test.Key.History);

                foreach (TimeSpan expectedTimeSpan in test.Value)
                {
                    if (results.Count < 1)
                    {
                        Trace.WriteLine(
                            string.Format(
                                "**Not enough results available for test**{0}",
                                Environment.NewLine));
                        testFailed = true;
                        break;
                    }

                    IScheduledFunctionResult<DateTime> result = results.Dequeue();
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

                    foreach (IScheduledFunctionResult<DateTime> extraResult in results)
                        OutputResult(extraResult);
                }
                if (!testFailed)
                {
                    numberOfTestsPassed++;
                    Trace.WriteLine(string.Format("**Test Passed**{0}", Environment.NewLine));
                }
                else
                    Trace.WriteLine(string.Format("**Test Failed**{0}", Environment.NewLine));
                testNumber++;
            }

            if (numberOfTestsPassed == _tests.Count)
            {
                Trace.WriteLine(
                    string.Format(
                        "All tests passed ({1} test(s)){0}",
                        Environment.NewLine,
                        numberOfTestsPassed));
            }
            else
                Assert.Fail("{0} passed, {1} failed", numberOfTestsPassed, _tests.Count - numberOfTestsPassed);
        }

        public void Run()
        {
            Start();
            while (!Completed())
                Thread.Sleep(500);
            Stop();

            CheckResults();
        }

        [NotNull]
        public static readonly ISchedulableFunction<DateTime> TestFunction =
            new SchedulableFunction<DateTime>(() => DateTime.Now);

        public static void OutputResult(IScheduledFunctionResult<DateTime> result)
        {
            Trace.WriteLine(
                string.Format(
                    "Result:\t\t{1:hh:mm:ss.ffff}{0}Due:\t\t{2:hh:mm:ss.ffff}{0}Started:\t{3:ss.ffff}{0}Duration:\t{4}ms{0}",
                    Environment.NewLine,
                    result.Result,
                    result.Due,
                    result.Started,
                    result.Duration.TotalMilliseconds));
        }

        public static void OutputResult(
            IScheduledFunctionResult<DateTime> result,
            DateTime startTime,
            TimeSpan expectedTimeSpan,
            bool failed = false)
        {
            Trace.WriteLine(
                string.Format(
                    "Result:\t\t{1:hh:mm:ss.ffff}{0}Expected:\t{2:hh:mm:ss.ffff}{0}Expected Time Span:\t{3}s{0}Due:\t\t{4:hh:mm:ss.ffff}{0}Started:\t{5:ss.ffff}{0}Duration:\t{6}ms{7}{0}",
                    Environment.NewLine,
                    result.Result,
                    startTime + expectedTimeSpan,
                    expectedTimeSpan.TotalSeconds,
                    result.Due,
                    result.Started,
                    result.Duration.TotalMilliseconds,
                    failed ? string.Format("{0}**Failed**", Environment.NewLine) : string.Empty));
        }
    }
}