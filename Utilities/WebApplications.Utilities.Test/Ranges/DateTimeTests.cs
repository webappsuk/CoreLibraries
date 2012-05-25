#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities.Test
// File: DateTimeRangeTests.cs
// 
// This software, its object code and source code and all modifications made to
// the same (the “Software”) are, and shall at all times remain, the proprietary
// information and longellectual property rights of Web Applications (UK) Limited. 
// You are only entitled to use the Software as expressly permitted by Web
// Applications (UK) Limited within the Software Customisation and
// Licence Agreement (the “Agreement”).  Any copying, modification, decompiling,
// distribution, licensing, sale, transfer or other use of the Software other than
// as expressly permitted in the Agreement is expressly forbidden.  Web
// Applications (UK) Limited reserves its rights to take action against you and
// your employer in accordance with its contractual and common law rights
// (including injunctive relief) should you breach the terms of the Agreement or
// otherwise infringe its copyright or other longellectual property rights in the
// Software.
// 
// © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
#endregion

using System;
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
            return minimum + TimeSpan.FromTicks((long) ((maximum - minimum).Ticks * Random.NextDouble()));
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
            return minimum + TimeSpan.FromTicks((long)((maximum - minimum).Ticks * Random.NextDouble()));
        }

        /// <summary>
        /// Choose a random time span of more than zero and less than one day.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.TimeSpan"/> which is greater than zero but less than one day.
        /// </returns>
        private static TimeSpan RandomTimeOffset()
        {
            return TimeSpan.FromHours(Random.NextDouble() * 11 + 0.5);
        }

        [TestMethod]
        public void ToString_IsNotBlank()
        {
            TimeSpan length = RandomDuration(MinSpan + MinSpan, MaxSpan);
            DateTime start = RandomDate(DateTime.MinValue, DateTime.MaxValue - length);
            DateTime end = start + length;
            TimeSpan step = RandomDuration(MinSpan, length - MinSpan);

            var dateRange = new DateTimeRange(start, end, step);

            Assert.AreNotEqual("", dateRange.ToString(), "String representation of range must not be an empty string");
        }

        [TestMethod]
        public void ToString_DoesDependOnTimeComponents()
        {
            TimeSpan length = RandomDuration(MinSpan + MinSpan, MaxSpan);
            DateTime start = RandomDate(DateTime.MinValue, DateTime.MaxValue - length);
            DateTime end = start + length;
            TimeSpan step = RandomDuration(MinSpan, length - MinSpan);

            var dateRange = new DateTimeRange(start, end, step);
            var dateRangeWithTime = new DateTimeRange(start + RandomTimeOffset(), end + RandomTimeOffset(), step);

            Assert.AreNotEqual(dateRange.ToString(), dateRangeWithTime.ToString(), "String representation of range should depend on the time components");
        }

        [TestMethod]
        public void ToString_HasCorrectFormat()
        {
            TimeSpan length = RandomDuration(MinSpan + MinSpan, MaxSpan);
            DateTime start = RandomDate(DateTime.MinValue, DateTime.MaxValue - length);
            DateTime end = start + length;
            TimeSpan step = RandomDuration(MinSpan, length - MinSpan);

            var dateRange = new DateTimeRange(start, end, step);

            Regex formatTest = new Regex(@"^\d{2}/\d{2}/\d{4} \d{2}:\d{2}:\d{2} - \d{2}/\d{2}/\d{4} \d{2}:\d{2}:\d{2}$");

            Assert.IsTrue(formatTest.IsMatch(dateRange.ToString()), "String representation of range should be of format dd/mm/yyyy - dd/mm/yyyy");
        }

        private static DateTimeRange GenerateDateTimeRangeWithStepSmallerThanRange(out DateTime start, out DateTime end, out TimeSpan step)
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

        private static DateTimeRange GenerateDateRangeWithStepLargerThanRange(out DateTime start, out DateTime end, out TimeSpan step)
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
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_EndBeforeStart_ThrowsArgumentOutOfRangeException()
        {
            TimeSpan length = RandomDuration(MinSpan, MaxSpan);
            DateTime start = RandomDate(DateTime.MinValue + length, DateTime.MaxValue);
            DateTime end = start - length;

            var dateRange = new DateTimeRange(start, end);
        }

        [TestMethod]
        public void Constructor_WithoutStepParam_StepDefaultsToOneSecond()
        {
            TimeSpan length = RandomDuration(MinSpan, MaxSpan);
            DateTime start = RandomDate(DateTime.MinValue, DateTime.MaxValue - length);
            DateTime end = start + length;

            var dateRange = new DateTimeRange(start, end);

            Assert.AreEqual(start, dateRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, dateRange.End, "End point field must match the value supplied");
            Assert.AreEqual(TimeSpan.FromSeconds(1), dateRange.Step, "Step amount must default to one");
        }

        [TestMethod]
        public void GetEnumerator_ValuesStayWithinRange()
        {
            TimeSpan length = RandomDuration(MinSpan, MaxSpan);
            DateTime start = RandomDate(DateTime.MinValue, DateTime.MaxValue - length);
            DateTime end = start + length;
            // note that the number of steps is limited to 100 or fewer
            TimeSpan step = TimeSpan.FromTicks(length.Ticks / Random.Next(4, 100));

            var dateRange = new DateTimeRange(start, end, step);

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
            TimeSpan step = TimeSpan.FromTicks(length.Ticks / Random.Next(4, 1000));

            //ensure that step size is a factor of the length of the range
            start += TimeSpan.FromTicks(length.Ticks % step.Ticks);

            var dateRange = new DateTimeRange(start, end, step);

            // Range endpoint is inclusive, so must take longo account this extra iteration
            Assert.AreEqual(length.Ticks / step.Ticks + 1, dateRange.Count(), "Iteration count should be (end-start)/step + 1 where endpoint is included");
        }

        [TestMethod]
        public void GetEnumerator_LengthNotDivisibleByStep_IterationCountMatchesCalculated()
        {
            TimeSpan length = RandomDuration(MinSpan, MaxSpan);
            DateTime start = RandomDate(DateTime.MinValue, DateTime.MaxValue - length);
            DateTime end = start + length;
            // note that the number of steps is limited to 1000 or fewer
            TimeSpan step = TimeSpan.FromTicks(length.Ticks / Random.Next(4, 1000));

            //ensure that step size is not a factor of the length of the range
            if (length.Ticks % step.Ticks == 0)
            {
                start += RandomDuration(MinSpan, step - MinSpan);
                length = end - start;
            }

            var dateRange = new DateTimeRange(start, end, step);

            Assert.AreEqual(length.Ticks / step.Ticks + 1, dateRange.Count(), "Iteration count should be (start-end)/step +1");
        }

        [TestMethod]
        public void GetEnumerator_Iterating_DifferenceBetweenIterationsMatchesStepSize()
        {
            TimeSpan length = RandomDuration(MinSpan, MaxSpan);
            DateTime start = RandomDate(DateTime.MinValue, DateTime.MaxValue - length);
            DateTime end = start + length;
            // note that the number of steps is limited to 100 or fewer
            TimeSpan step = TimeSpan.FromTicks(length.Ticks / Random.Next(4, 100));

            var dateRange = new DateTimeRange(start, end, step);

            DateTime? previous = null;
            foreach (DateTime d in dateRange)
            {
                if (previous.HasValue)
                {
                    Assert.AreEqual(d - previous, step, "Difference between iteration values should match the step value supplied");
                }
                previous = d;
            }
        }

        [TestMethod]
        public void GetEnumerator_UsingLargestPossibleParameters_IteratesSuccessfully()
        {
            // Step chosen to avoid an unfeasible number of iterations
            var dateRange = new DateTimeRange(DateTime.MinValue, DateTime.MaxValue, TimeSpan.FromTicks(MaxSpan.Ticks / 16));

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
            DateTime start = RandomDate(DateTime.MinValue + TimeSpan.FromDays(10), DateTime.MaxValue - TimeSpan.FromDays(10));
            DateTime end = RandomDate(start, DateTime.MaxValue - TimeSpan.FromDays(10));
            DateTime testValue = RandomDate(DateTime.MinValue, start);

            Assert.AreEqual(start, (new DateTimeRange(start, end)).Bind(testValue),
                   "Bind should return the lower bound of the range if the input is below the range");
        }

        [TestMethod]
        public void Bind_NumberAboveRange_ReturnsEnd()
        {
            DateTime start = RandomDate(DateTime.MinValue + TimeSpan.FromDays(10), DateTime.MaxValue - TimeSpan.FromDays(10));
            DateTime end = RandomDate(start, DateTime.MaxValue - TimeSpan.FromDays(10));
            DateTime testValue = RandomDate(end, DateTime.MaxValue);

            Assert.AreEqual(end, (new DateTimeRange(start, end)).Bind(testValue),
                   "Bind should return the upper bound of the range if the input is above the range");
        }

        [TestMethod]
        public void Bind_NumberWithinRange_ReturnsInput()
        {
            DateTime start = RandomDate(DateTime.MinValue + TimeSpan.FromDays(10), DateTime.MaxValue - TimeSpan.FromDays(10));
            DateTime end = RandomDate(start, DateTime.MaxValue - TimeSpan.FromDays(10));
            DateTime testValue = RandomDate(start, end);

            Assert.AreEqual(testValue, (new DateTimeRange(start, end)).Bind(testValue),
                   "Bind should return the input if it is within the range");
        }
    }
}
