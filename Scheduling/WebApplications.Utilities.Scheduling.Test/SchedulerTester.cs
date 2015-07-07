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

namespace WebApplications.Utilities.Scheduling.Test
{
#if false
    public class SchedulerTester
    {
        [NotNull]
        private readonly Dictionary<ScheduledFunction<Instant>, IEnumerable<Duration>> _tests =
            new Dictionary<ScheduledFunction<Instant>, IEnumerable<Duration>>();
        
        private DateTime _fullStartTime = DateTime.UtcNow;
        private DateTime _startTime = DateTime.UtcNow;

        public SchedulerTester()
        {
            Scheduler.Enabled = false;
        }

        public static void WaitForStartOfSecond(bool requireEven = false)
        {
            // Wait until the start of a second
            while (DateTime.UtcNow.Millisecond > 100 ||
                   (requireEven && DateTime.UtcNow.Second % 2 != 0))
                Thread.Sleep(10);
        }

        public ScheduledFunction<Instant> AddTest([NotNull] ISchedule schedule, [NotNull] IEnumerable<Duration> expectedResults, ScheduledFunction<Instant>.SchedulableDueCancellableFunctionAsync function = null)
        {
            Assert.IsNotNull(schedule);
            Assert.IsNotNull(expectedResults);
            return AddTest(Scheduler.Add(function ?? TestFunction, schedule, expectedResults.Count()), expectedResults);
        }

        public ScheduledFunction<Instant> AddTest(
            [NotNull] ScheduledFunction<Instant> scheduledFunction,
            [NotNull] IEnumerable<Duration> expectedResults)
        {
            _tests.Add(scheduledFunction, expectedResults);
            return scheduledFunction;
        }

        public void SetStartTime()
        {
            _fullStartTime = DateTime.UtcNow;
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
            Scheduler.Enabled = true;
        }

        public void Stop()
        {
            Scheduler.Enabled = false;
        }

        public bool Completed()
        {
            return _tests.All(test => test.Key.History.Count() >= test.Value.Count());
        }

        public void CheckResults()
        {
            int numberOfTestsPassed = 0;
            int testNumber = 1;

            foreach (KeyValuePair<ScheduledFunction<DateTime>, IEnumerable<TimeSpan>> test in _tests)
            {
                Trace.WriteLine(
                    string.Format(
                        "Test #{1} (Start time: {2:hh:mm:ss.ffff} ({3:hh:mm:ss.ffff})):{0}",
                        Environment.NewLine,
                        testNumber,
                        _startTime,
                        _fullStartTime));

                bool testFailed = false;
                Queue<ScheduledFunctionResult<DateTime>> results =
                    new Queue<ScheduledFunctionResult<DateTime>>(test.Key.History);

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
        public static readonly ScheduledFunction<Instant>.SchedulableDueCancellableFunctionAsync TestFunction =
            (d, t) =>
            {
                Trace.WriteLine(string.Format("Hit Test Function.  Due - {0:ss.fffffff}, Now - {1:ss.fffffff}", d, DateTime.UtcNow));
                return Task.FromResult(d);
            };

        public static void OutputResult(ScheduledFunctionResult<DateTime> result)
        {
            Assert.IsNotNull(result);
            Trace.WriteLine(
                string.Format(
                    "Result:\t\t{1:hh:mm:ss.ffff}{0}Due:\t\t{2:hh:mm:ss.ffff}{0}Started:\t{3:ss.ffff}{0}Duration:\t{4}ms{0}",
                    Environment.NewLine,
                    result.Result,
                    result.Due,
                    result.Started,
                    result.Duration.TotalMilliseconds()));
        }

        public static void OutputResult(
            ScheduledFunctionResult<DateTime> result,
            DateTime startTime,
            TimeSpan expectedTimeSpan,
            bool failed = false)
        {
            Assert.IsNotNull(result);
            Trace.WriteLine(
                string.Format(
                    "Result:\t\t{1:hh:mm:ss.ffff}{0}Expected:\t{2:hh:mm:ss.ffff}{0}Expected Time Span:\t{3}s{0}Due:\t\t{4:hh:mm:ss.ffff}{0}Started:\t{5:ss.ffff}{0}Duration:\t{6}ms{7}{0}",
                    Environment.NewLine,
                    result.Result,
                    startTime + expectedTimeSpan,
                    expectedTimeSpan.TotalSeconds,
                    result.Due,
                    result.Started,
                    result.Duration.TotalMilliseconds(),
                    failed ? string.Format("{0}**Failed**", Environment.NewLine) : string.Empty));
        }
    }
#endif
}