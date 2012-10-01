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
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Ranges;

namespace WebApplications.Utilities.Test.Ranges
{
    [TestClass]
    public class DateTimeRangeTests : UtilitiesTestBase
    {
        private static readonly TimeSpan MinSpan = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan MaxSpan = (DateTime.MaxValue - DateTime.MinValue);

        /// <summary>
        /// Choose a random date and time within a given range.
        /// </summary>
        /// <param name="minimum">The minimum date possible.</param>
        /// <param name="maximum">The maximum date possible.</param>
        /// <returns>
        /// A <see cref="T:System.DateTime"/> between <paramref name="minimum"/> and <paramref name="maximum"/>.
        /// </returns>
        private static DateTime RandomDate(DateTime minimum, DateTime maximum)
        {
            return minimum + TimeSpan.FromTicks((long) ((maximum - minimum).Ticks*Random.NextDouble()));
        }

        /// <summary>
        /// Choose a random time span with length within a given range.
        /// </summary>
        /// <param name="minimum">The minumum timespan.</param>
        /// <param name="maximum">The maximum timespan.</param>
        /// <returns>
        /// A <see cref="T:System.TimeSpan"/> between <paramref name="minimum"/> and <paramref name="maximum"/>.
        /// </returns>
        private static TimeSpan RandomDuration(TimeSpan minimum, TimeSpan maximum)
        {
            return minimum + TimeSpan.FromTicks((long) ((maximum - minimum).Ticks*Random.NextDouble()));
        }

        /// <summary>
        /// Choose a random time span of more than zero and less than one day.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.TimeSpan"/> which is greater than zero but less than one day.
        /// </returns>
        private static TimeSpan RandomTimeOffset()
        {
            return TimeSpan.FromHours(Random.NextDouble()*11 + 0.5);
        }

        [TestMethod]
        public void ToString_IsNotBlank()
        {
            TimeSpan length = RandomDuration(MinSpan + MinSpan, MaxSpan);
            DateTime start = RandomDate(DateTime.MinValue, DateTime.MaxValue - length);
            DateTime end = start + length;
            TimeSpan step = RandomDuration(MinSpan, length - MinSpan);

            DateTimeRange dateRange = new DateTimeRange(start, end, step);

            Assert.AreNotEqual("", dateRange.ToString(), "String representation of range must not be an empty string");
        }

        [TestMethod]
        public void ToString_DoesDependOnTimeComponents()
        {
            TimeSpan length = RandomDuration(MinSpan + MinSpan, MaxSpan);
            DateTime start = RandomDate(DateTime.MinValue, DateTime.MaxValue - length);
            DateTime end = start + length;
            TimeSpan step = RandomDuration(MinSpan, length - MinSpan);

            DateTimeRange dateRange = new DateTimeRange(start, end, step);
            DateTimeRange dateRangeWithTime = new DateTimeRange(start + RandomTimeOffset(), end + RandomTimeOffset(),
                                                                step);

            Assert.AreNotEqual(dateRange.ToString(), dateRangeWithTime.ToString(),
                               "String representation of range should depend on the time components");
        }

        [TestMethod]
        public void ToString_HasCorrectFormat()
        {
            TimeSpan length = RandomDuration(MinSpan + MinSpan, MaxSpan);
            DateTime start = RandomDate(DateTime.MinValue, DateTime.MaxValue - length);
            DateTime end = start + length;
            TimeSpan step = RandomDuration(MinSpan, length - MinSpan);

            DateTimeRange dateRange = new DateTimeRange(start, end, step);

            Regex formatTest = new Regex(@"^\d{2}/\d{2}/\d{4} \d{2}:\d{2}:\d{2} - \d{2}/\d{2}/\d{4} \d{2}:\d{2}:\d{2}$");

            Assert.IsTrue(formatTest.IsMatch(dateRange.ToString()),
                          "String representation of range should be of format dd/mm/yyyy - dd/mm/yyyy");
        }

        private static DateTimeRange GenerateDateTimeRangeWithStepSmallerThanRange(out DateTime start, out DateTime end,
                                                                                   out TimeSpan step)
        {
            TimeSpan length = RandomDuration(MinSpan + MinSpan, MaxSpan);
            start = RandomDate(DateTime.MinValue, DateTime.MaxValue - length);
            end = start + length;
            step = RandomDuration(MinSpan, length - MinSpan);

            return new DateTimeRange(start, end, step);
        }

        [TestMethod]
        public void Start_StepSmallerThanRange_MatchesThatGivenWithTimeStripped()
        {
            DateTime start, end;
            TimeSpan step;
            DateTimeRange dateTimeRange = GenerateDateTimeRangeWithStepSmallerThanRange(out start, out end, out step);

            Assert.AreEqual(start, dateTimeRange.Start, "Starting point field must match the value supplied.");
        }

        [TestMethod]
        public void End_StepSmallerThanRange_MatchesThatGivenWithTimeStripped()
        {
            DateTime start, end;
            TimeSpan step;
            DateTimeRange dateTimeRange = GenerateDateTimeRangeWithStepSmallerThanRange(out start, out end, out step);

            Assert.AreEqual(end, dateTimeRange.End, "End point field must match the value supplied.");
        }

        [TestMethod]
        public void Step_StepSmallerThanRange_MatchesThatGivenWithTimeStripped()
        {
            DateTime start, end;
            TimeSpan step;
            DateTimeRange dateTimeRange = GenerateDateTimeRangeWithStepSmallerThanRange(out start, out end, out step);

            Assert.AreEqual(step, dateTimeRange.Step, "Step amount field must match the value supplied.");
        }

        private static DateTimeRange GenerateDateRangeWithStepLargerThanRange(out DateTime start, out DateTime end,
                                                                              out TimeSpan step)
        {
            // subtracting MinSpan twice from max length to give space for oversized step without converting type to perform the calculation to do so
            TimeSpan length = RandomDuration(MinSpan, MaxSpan - MinSpan - MinSpan);
            start = RandomDate(DateTime.MinValue, DateTime.MaxValue - length);
            end = start + length;
            step = RandomDuration(length + MinSpan, length + MinSpan + MinSpan);

            return new DateTimeRange(start, end, step);
        }

        [TestMethod]
        public void Start_StepIsLargerThanRange_MatchesThatGivenWithTimeStripped()
        {
            DateTime start, end;
            TimeSpan step;
            DateTimeRange dateTimeRange = GenerateDateRangeWithStepLargerThanRange(out start, out end, out step);

            Assert.AreEqual(start, dateTimeRange.Start, "Starting point field must match the value supplied.");
        }

        [TestMethod]
        public void End_StepIsLargerThanRange_MatchesThatGivenWithTimeStripped()
        {
            DateTime start, end;
            TimeSpan step;
            DateTimeRange dateTimeRange = GenerateDateRangeWithStepLargerThanRange(out start, out end, out step);

            Assert.AreEqual(end, dateTimeRange.End, "End point field must match the value supplied.");
        }

        [TestMethod]
        public void Step_StepIsLargerThanRange_MatchesThatGivenWithTimeStripped()
        {
            DateTime start, end;
            TimeSpan step;
            DateTimeRange dateTimeRange = GenerateDateRangeWithStepLargerThanRange(out start, out end, out step);

            Assert.AreEqual(step, dateTimeRange.Step, "Step amount field must match the value supplied.");
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void Constructor_EndBeforeStart_ThrowsArgumentOutOfRangeException()
        {
            TimeSpan length = RandomDuration(MinSpan, MaxSpan);
            DateTime start = RandomDate(DateTime.MinValue + length, DateTime.MaxValue);
            DateTime end = start - length;

            DateTimeRange dateRange = new DateTimeRange(start, end);
        }

        [TestMethod]
        public void Constructor_WithoutStepParam_DeltaLessThanMillisecond_StepDefaultsToOneTick()
        {
            TimeSpan length = RandomDuration(TimeSpan.Zero, TimeSpan.FromMilliseconds(1));
            Trace.WriteLine(length);
            DateTime start = RandomDate(DateTime.MinValue, DateTime.MaxValue - length);
            DateTime end = start + length;

            DateTimeRange dateRange = new DateTimeRange(start, end);

            Assert.AreEqual(start, dateRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, dateRange.End, "End point field must match the value supplied");
            Assert.AreEqual(TimeSpan.FromTicks(1), dateRange.Step, "Step amount must default to one tick");
        }

        [TestMethod]
        public void Constructor_WithoutStepParam_DeltaLessThanSecond_StepDefaultsToOneMillisecond()
        {
            TimeSpan length = RandomDuration(TimeSpan.FromMilliseconds(1), TimeSpan.FromSeconds(1));
            Trace.WriteLine(length);
            DateTime start = RandomDate(DateTime.MinValue, DateTime.MaxValue - length);
            DateTime end = start + length;

            DateTimeRange dateRange = new DateTimeRange(start, end);

            Assert.AreEqual(start, dateRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, dateRange.End, "End point field must match the value supplied");
            Assert.AreEqual(TimeSpan.FromMilliseconds(1), dateRange.Step, "Step amount must default to one second");
        }

        [TestMethod]
        public void Constructor_WithoutStepParam_DeltaLessThanMinute_StepDefaultsToOneSecond()
        {
            TimeSpan length = RandomDuration(TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(1));
            Trace.WriteLine(length);
            DateTime start = RandomDate(DateTime.MinValue, DateTime.MaxValue - length);
            DateTime end = start + length;

            DateTimeRange dateRange = new DateTimeRange(start, end);

            Assert.AreEqual(start, dateRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, dateRange.End, "End point field must match the value supplied");
            Assert.AreEqual(TimeSpan.FromSeconds(1), dateRange.Step, "Step amount must default to one second");
        }

        [TestMethod]
        public void Constructor_WithoutStepParam_DeltaLessThanHour_StepDefaultsToOneMinute()
        {
            TimeSpan length = RandomDuration(TimeSpan.FromMinutes(1), TimeSpan.FromHours(1));
            Trace.WriteLine(length);
            DateTime start = RandomDate(DateTime.MinValue, DateTime.MaxValue - length);
            DateTime end = start + length;

            DateTimeRange dateRange = new DateTimeRange(start, end);

            Assert.AreEqual(start, dateRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, dateRange.End, "End point field must match the value supplied");
            Assert.AreEqual(TimeSpan.FromMinutes(1), dateRange.Step, "Step amount must default to one minute");
        }

        [TestMethod]
        public void Constructor_WithoutStepParam_DeltaLessThanDay_StepDefaultsToOneHour()
        {
            TimeSpan length = RandomDuration(TimeSpan.FromHours(1), TimeSpan.FromDays(1));
            Trace.WriteLine(length);
            DateTime start = RandomDate(DateTime.MinValue, DateTime.MaxValue - length);
            DateTime end = start + length;

            DateTimeRange dateRange = new DateTimeRange(start, end);

            Assert.AreEqual(start, dateRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, dateRange.End, "End point field must match the value supplied");
            Assert.AreEqual(TimeSpan.FromHours(1), dateRange.Step, "Step amount must default to one minute");
        }

        [TestMethod]
        public void Constructor_WithoutStepParam_DeltaGreaterThanDay_StepDefaultsToOneDay()
        {
            TimeSpan length = RandomDuration(TimeSpan.FromDays(1), MaxSpan);
            Trace.WriteLine(length);
            DateTime start = RandomDate(DateTime.MinValue, DateTime.MaxValue - length);
            DateTime end = start + length;

            DateTimeRange dateRange = new DateTimeRange(start, end);

            Assert.AreEqual(start, dateRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, dateRange.End, "End point field must match the value supplied");
            Assert.AreEqual(TimeSpan.FromDays(1), dateRange.Step, "Step amount must default to one day");
        }

        [TestMethod]
        public void GetEnumerator_ValuesStayWithinRange()
        {
            TimeSpan length = RandomDuration(MinSpan, MaxSpan);
            DateTime start = RandomDate(DateTime.MinValue, DateTime.MaxValue - length);
            DateTime end = start + length;
            // note that the number of steps is limited to 100 or fewer
            TimeSpan step = TimeSpan.FromTicks(length.Ticks/Random.Next(4, 100));

            DateTimeRange dateRange = new DateTimeRange(start, end, step);

            foreach (DateTime d in dateRange)
            {
                Assert.IsTrue(d >= start, "Value from iterator must by equal or above start parameter");
                Assert.IsTrue(d <= end, "Value from iterator must be equal or below end parameter");
            }
        }

        [TestMethod]
        public void GetEnumerator_LengthDivisibleByStep_IterationCountMatchesCalculated()
        {
            TimeSpan length = RandomDuration(MinSpan, MaxSpan);
            DateTime start = RandomDate(DateTime.MinValue, DateTime.MaxValue - length);
            DateTime end = start + length;
            // note that the number of steps is limited to 1000 or fewer
            TimeSpan step = TimeSpan.FromTicks(length.Ticks/Random.Next(4, 1000));

            //ensure that step size is a factor of the length of the range
            start += TimeSpan.FromTicks(length.Ticks%step.Ticks);

            DateTimeRange dateRange = new DateTimeRange(start, end, step);

            // Range endpoint is inclusive, so must take longo account this extra iteration
            Assert.AreEqual(length.Ticks/step.Ticks + 1, dateRange.Count(),
                            "Iteration count should be (end-start)/step + 1 where endpoint is included");
        }

        [TestMethod]
        public void GetEnumerator_LengthNotDivisibleByStep_IterationCountMatchesCalculated()
        {
            TimeSpan length = RandomDuration(MinSpan, MaxSpan);
            DateTime start = RandomDate(DateTime.MinValue, DateTime.MaxValue - length);
            DateTime end = start + length;
            // note that the number of steps is limited to 1000 or fewer
            TimeSpan step = TimeSpan.FromTicks(length.Ticks/Random.Next(4, 1000));

            //ensure that step size is not a factor of the length of the range
            if (length.Ticks%step.Ticks == 0)
            {
                start += RandomDuration(MinSpan, step - MinSpan);
                length = end - start;
            }

            DateTimeRange dateRange = new DateTimeRange(start, end, step);

            Assert.AreEqual(length.Ticks/step.Ticks + 1, dateRange.Count(),
                            "Iteration count should be (start-end)/step +1");
        }

        [TestMethod]
        public void GetEnumerator_Iterating_DifferenceBetweenIterationsMatchesStepSize()
        {
            TimeSpan length = RandomDuration(MinSpan, MaxSpan);
            DateTime start = RandomDate(DateTime.MinValue, DateTime.MaxValue - length);
            DateTime end = start + length;
            // note that the number of steps is limited to 100 or fewer
            TimeSpan step = TimeSpan.FromTicks(length.Ticks/Random.Next(4, 100));

            DateTimeRange dateRange = new DateTimeRange(start, end, step);

            DateTime? previous = null;
            foreach (DateTime d in dateRange)
            {
                if (previous.HasValue)
                {
                    Assert.AreEqual(d - previous, step,
                                    "Difference between iteration values should match the step value supplied");
                }
                previous = d;
            }
        }

        [TestMethod]
        public void GetEnumerator_UsingLargestPossibleParameters_IteratesSuccessfully()
        {
            // Step chosen to avoid an unfeasible number of iterations
            DateTimeRange dateRange = new DateTimeRange(DateTime.MinValue, DateTime.MaxValue,
                                                        TimeSpan.FromTicks(MaxSpan.Ticks/16));

            bool iterated = false;
            foreach (DateTime d in dateRange)
            {
                iterated = true;
            }

            Assert.AreEqual(true, iterated, "When iterating across full range, at least one value should be returned");
        }

        [TestMethod]
        public void Bind_NumberBelowRange_ReturnsStart()
        {
            DateTime start = RandomDate(DateTime.MinValue + TimeSpan.FromDays(10),
                                        DateTime.MaxValue - TimeSpan.FromDays(10));
            DateTime end = RandomDate(start, DateTime.MaxValue - TimeSpan.FromDays(10));
            DateTime testValue = RandomDate(DateTime.MinValue, start);

            Assert.AreEqual(start, (new DateTimeRange(start, end)).Bind(testValue),
                            "Bind should return the lower bound of the range if the input is below the range");
        }

        [TestMethod]
        public void Bind_NumberAboveRange_ReturnsEnd()
        {
            DateTime start = RandomDate(DateTime.MinValue + TimeSpan.FromDays(10),
                                        DateTime.MaxValue - TimeSpan.FromDays(10));
            DateTime end = RandomDate(start, DateTime.MaxValue - TimeSpan.FromDays(10));
            DateTime testValue = RandomDate(end, DateTime.MaxValue);

            Assert.AreEqual(end, (new DateTimeRange(start, end)).Bind(testValue),
                            "Bind should return the upper bound of the range if the input is above the range");
        }

        [TestMethod]
        public void Bind_NumberWithinRange_ReturnsInput()
        {
            DateTime start = RandomDate(DateTime.MinValue + TimeSpan.FromDays(10),
                                        DateTime.MaxValue - TimeSpan.FromDays(10));
            DateTime end = RandomDate(start, DateTime.MaxValue - TimeSpan.FromDays(10));
            DateTime testValue = RandomDate(start, end);

            Assert.AreEqual(testValue, (new DateTimeRange(start, end)).Bind(testValue),
                            "Bind should return the input if it is within the range");
        }
    }
}