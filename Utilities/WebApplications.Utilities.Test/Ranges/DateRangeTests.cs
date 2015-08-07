#region © Copyright Web Applications (UK) Ltd, 2015.  All rights reserved.
// Copyright (c) 2015, Web Applications UK Ltd
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
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Ranges;

namespace WebApplications.Utilities.Test.Ranges
{
    [TestClass]
    public class DateRangeTests : UtilitiesTestBase
    {
        private static readonly int MaxDays = (DateTime.MaxValue - DateTime.MinValue).Days;

        /// <summary>
        /// Choose a random date (without any time component) within a given range.
        /// </summary>
        /// <param name="minimum">The minimum date possible.</param>
        /// <param name="maximum">The maximum date possible.</param>
        /// <returns>
        /// A <see cref="T:System.DateTime"/> between <paramref name="minimum"/> and <paramref name="maximum"/>, with the time component set to midnight.
        /// </returns>
        private static DateTime RandomDate(DateTime minimum, DateTime maximum)
        {
            return minimum.AddDays(Math.Floor((maximum - minimum).Days * Random.NextDouble()));
        }

        /// <summary>
        /// Choose a random time span with length within a given range.
        /// </summary>
        /// <param name="minimum">The minimum length in days.</param>
        /// <param name="maximum">The maximum length in days.</param>
        /// <returns>
        /// A <see cref="T:System.TimeSpan"/> with the number of days between <paramref name="minimum"/> and <paramref name="maximum"/>.
        /// </returns>
        private static TimeSpan RandomDuration(int minimum, int maximum)
        {
            return TimeSpan.FromDays(Random.Next(minimum, maximum));
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
            TimeSpan length = RandomDuration(1, MaxDays);
            DateTime start = RandomDate(DateTime.MinValue, DateTime.MaxValue - length);
            DateTime end = start + length;
            int step = Random.Next(1, length.Days / 2);

            DateRange dateRange = new DateRange(start, end, step);

            Assert.AreNotEqual("", dateRange.ToString(), "String representation of range must not be an empty string");
        }

        [TestMethod]
        public void ToString_DoesNotDependOnTimeComponents()
        {
            TimeSpan length = RandomDuration(1, MaxDays);
            DateTime start = RandomDate(DateTime.MinValue, DateTime.MaxValue - length);
            DateTime end = start + length;
            int step = Random.Next(1, length.Days / 2);

            DateRange dateRange = new DateRange(start, end, step);
            DateRange dateRangeWithTime = new DateRange(start + RandomTimeOffset(), end + RandomTimeOffset(), step);

            Assert.AreEqual(
                dateRange.ToString(),
                dateRangeWithTime.ToString(),
                "String representation of range must not depend on the (ignored) time components");
        }

        [TestMethod]
        public void ToString_HasCorrectFormat()
        {
            TimeSpan length = RandomDuration(2, MaxDays);
            DateTime start = RandomDate(DateTime.MinValue, DateTime.MaxValue - length);
            DateTime end = start + length;
            int step = Random.Next(1, length.Days / 2);

            DateRange dateRange = new DateRange(start, end, step);
            Assert.AreEqual($"{start:d} - {end:d} [{length.TotalDays + 1} days]", dateRange.ToString());
        }

        private static DateRange GenerateDateRangeWithStepSmallerThanRange(
            out DateTime start,
            out DateTime end,
            out int step)
        {
            TimeSpan length = RandomDuration(1, MaxDays);
            start = RandomDate(DateTime.MinValue, DateTime.MaxValue - length);
            end = start + length;
            step = Random.Next(1, length.Days / 2);

            return new DateRange(start + RandomTimeOffset(), end + RandomTimeOffset(), step);
        }

        [TestMethod]
        public void Start_StepSmallerThanRange_MatchesThatGivenWithTimeStripped()
        {
            DateTime start, end;
            int step;
            DateRange dateRange = GenerateDateRangeWithStepSmallerThanRange(out start, out end, out step);

            Assert.AreEqual(
                start,
                dateRange.Start,
                "Starting point field must match the value supplied but with time component stripped");
        }

        [TestMethod]
        public void End_StepSmallerThanRange_MatchesThatGivenWithTimeStripped()
        {
            DateTime start, end;
            int step;
            DateRange dateRange = GenerateDateRangeWithStepSmallerThanRange(out start, out end, out step);

            Assert.AreEqual(
                end,
                dateRange.End,
                "End point field must match the value supplied but with time component stripped");
        }

        [TestMethod]
        public void Step_StepSmallerThanRange_MatchesThatGivenWithTimeStripped()
        {
            DateTime start, end;
            int step;
            DateRange dateRange = GenerateDateRangeWithStepSmallerThanRange(out start, out end, out step);

            Assert.AreEqual(
                TimeSpan.FromDays(step),
                dateRange.Step,
                "Step amount field must match the value supplied in days");
        }

        private static DateRange GenerateDateRangeWithStepLargerThanRange(
            out DateTime start,
            out DateTime end,
            out int step)
        {
            TimeSpan length = RandomDuration(1, MaxDays - 10);
            start = RandomDate(DateTime.MinValue, DateTime.MaxValue - length);
            end = start + length;
            step = Random.Next(length.Days + 1, length.Days + 10);

            return new DateRange(start + RandomTimeOffset(), end + RandomTimeOffset(), step);
        }

        [TestMethod]
        public void Start_StepIsLargerThanRange_MatchesThatGivenWithTimeStripped()
        {
            DateTime start, end;
            int step;

            DateRange dateRange = GenerateDateRangeWithStepLargerThanRange(out start, out end, out step);

            Assert.AreEqual(
                start,
                dateRange.Start,
                "Starting point field must match the value supplied but with time component stripped");
        }

        [TestMethod]
        public void End_StepIsLargerThanRange_MatchesThatGivenWithTimeStripped()
        {
            DateTime start, end;
            int step;

            DateRange dateRange = GenerateDateRangeWithStepLargerThanRange(out start, out end, out step);

            Assert.AreEqual(
                end,
                dateRange.End,
                "End point field must match the value supplied but with time component stripped");
        }

        [TestMethod]
        public void Step_StepIsLargerThanRange_MatchesThatGivenWithTimeStripped()
        {
            DateTime start, end;
            int step;

            DateRange dateRange = GenerateDateRangeWithStepLargerThanRange(out start, out end, out step);

            Assert.AreEqual(
                TimeSpan.FromDays(step),
                dateRange.Step,
                "Step amount field must match the value supplied in days");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_EndBeforeStart_ThrowsArgumentOutOfRangeException()
        {
            TimeSpan length = RandomDuration(1, MaxDays);
            DateTime start = RandomDate(DateTime.MinValue + length, DateTime.MaxValue);
            DateTime end = start - length;

            DateRange dateRange = new DateRange(start, end);
        }

        [TestMethod]
        public void Constructor_WithoutStepParam_StepDefaultsToOneDay()
        {
            TimeSpan length = RandomDuration(1, MaxDays);
            DateTime start = RandomDate(DateTime.MinValue, DateTime.MaxValue - length);
            DateTime end = start + length;

            DateRange dateRange = new DateRange(start, end);

            Assert.AreEqual(start, dateRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, dateRange.End, "End point field must match the value supplied");
            Assert.AreEqual(1, dateRange.Step.Days, "Step amount must default to one");
        }

        [TestMethod]
        public void Days_ReturnsNumberOfDaysCoveredByRangeIncludingEndpoints()
        {
            TimeSpan length = RandomDuration(1, MaxDays);
            DateTime start = RandomDate(DateTime.MinValue, DateTime.MaxValue - length);
            DateTime end = start + length;

            DateRange dateRange = new DateRange(start, end);

            Assert.AreEqual(
                length.Days + 1,
                dateRange.Days,
                "Days property correctly calculates number of days covered by the range, including both endpoints.");
        }

        [TestMethod]
        public void Nights_ReturnsDurationOfRangeInDays()
        {
            TimeSpan length = RandomDuration(1, MaxDays);
            DateTime start = RandomDate(DateTime.MinValue, DateTime.MaxValue - length);
            DateTime end = start + length;

            DateRange dateRange = new DateRange(start, end);

            Assert.AreEqual(
                length.Days,
                dateRange.Nights,
                "Nights property correctly calculates number of days covered by the range, excluding the final endpoint.");
        }

        [TestMethod]
        public void GetEnumerator_ValuesStayWithinRange()
        {
            TimeSpan length = RandomDuration(1, MaxDays);
            DateTime start = RandomDate(DateTime.MinValue, DateTime.MaxValue - length);
            DateTime end = start + length;
            // note that the number of steps is limited to 100 or fewer
            int step = length.Days / Random.Next(4, Math.Max(4, Math.Min(length.Days / 2, 100)));

            // ensure the step is at least 1 (as length of less than four causes it to round down to zero)
            if (step < 1) step = 1;

            DateRange dateRange = new DateRange(start, end, step);

            foreach (DateTime d in dateRange)
            {
                Assert.IsTrue(d >= start, "Value from iterator must by equal or above start parameter");
                Assert.IsTrue(d <= end, "Value from iterator must be equal or below end parameter");
            }
        }

        [TestMethod]
        public void GetEnumerator_LengthDivisibleByStep_CountMatchesCalculated()
        {
            TimeSpan length = RandomDuration(1, MaxDays);
            DateTime start = RandomDate(DateTime.MinValue, DateTime.MaxValue - length);
            DateTime end = start + length;
            // note that the number of steps is limited to 1000 or fewer
            int step = length.Days / Random.Next(4, Math.Max(4, Math.Min(length.Days / 2, 1000)));

            // In case range length is under 4, ensure the step is at least 1
            if (step < 1) step = 1;

            //ensure that step size is a factor of the length of the range
            start += TimeSpan.FromDays(length.Days % step);

            DateRange dateRange = new DateRange(start, end, step);

            // Range endpoint is inclusive, so must take longo account this extra iteration
            Assert.AreEqual(
                length.Days / step + 1,
                dateRange.Count(),
                "Iteration count should be (end-start)/step + 1 where endpoint is included");
        }

        [TestMethod]
        public void GetEnumerator_LengthNotDivisibleByStep_CountMatchesCalculated()
        {
            TimeSpan length = RandomDuration(1, MaxDays);
            DateTime start = RandomDate(DateTime.MinValue, DateTime.MaxValue - length);
            DateTime end = start + length;
            // note that the number of steps is limited to 1000 or fewer
            int step = length.Days / Random.Next(4, Math.Max(4, Math.Min(length.Days / 2, 1000)));

            // In case range length is under 4, ensure the step is at least 2
            if (step < 2) step = 2;

            //ensure that step size is not a factor of the length of the range
            if (length.Days % step == 0)
            {
                start += RandomDuration(1, step - 1);
                length = end - start;
            }

            DateRange dateRange = new DateRange(start, end, step);

            Assert.AreEqual(length.Days / step + 1, dateRange.Count(), "Iteration count should be (start-end)/step +1");
        }

        [TestMethod]
        public void GetEnumerator_DifferenceBetweenIterationsMatchesStepSize()
        {
            TimeSpan length = RandomDuration(1, MaxDays);
            DateTime start = RandomDate(DateTime.MinValue, DateTime.MaxValue - length);
            DateTime end = start + length;
            // note that the number of steps is limited to 100 or fewer
            int step = length.Days / Random.Next(4, Math.Max(4, Math.Min(length.Days / 2, 100)));

            // In case range length is under 4, ensure the step is at least 1
            if (step < 1) step = 1;

            DateRange dateRange = new DateRange(start, end, step);

            TimeSpan difference = TimeSpan.FromDays(step);

            DateTime? previous = null;
            foreach (DateTime d in dateRange)
            {
                if (previous.HasValue)
                    Assert.AreEqual(
                        d - previous,
                        difference,
                        "Difference between iteration values should match the step value supplied");
                previous = d;
            }
        }

        [TestMethod]
        public void GetEnumerator_UsingLargestPossibleParameters_IteratesSuccessfully()
        {
            // Step chosen to avoid an unfeasible number of iterations
            DateRange dateRange = new DateRange(DateTime.MinValue, DateTime.MaxValue, MaxDays / 16);

            bool iterated = false;
            foreach (DateTime d in dateRange)
                iterated = true;

            Assert.AreEqual(true, iterated, "When iterating across full range, at least one value should be returned");
        }

        [TestMethod]
        public void Bind_NumberBelowRange_ReturnsStart()
        {
            DateTime start = RandomDate(
                DateTime.MinValue + TimeSpan.FromDays(10),
                DateTime.MaxValue - TimeSpan.FromDays(10));
            DateTime end = RandomDate(start, DateTime.MaxValue - TimeSpan.FromDays(10));
            DateTime testValue = RandomDate(DateTime.MinValue, start);

            Assert.AreEqual(
                start,
                (new DateRange(start, end)).Bind(testValue),
                "Bind should return the lower bound of the range if the input is below the range");
        }

        [TestMethod]
        public void Bind_NumberAboveRange_ReturnsEnd()
        {
            DateTime start = RandomDate(
                DateTime.MinValue + TimeSpan.FromDays(10),
                DateTime.MaxValue - TimeSpan.FromDays(10));
            DateTime end = RandomDate(start, DateTime.MaxValue - TimeSpan.FromDays(10));
            DateTime testValue = RandomDate(end, DateTime.MaxValue);

            Assert.AreEqual(
                end,
                (new DateRange(start, end)).Bind(testValue),
                "Bind should return the upper bound of the range if the input is above the range");
        }

        [TestMethod]
        public void Bind_NumberWithinRange_ReturnsInput()
        {
            DateTime start = RandomDate(
                DateTime.MinValue + TimeSpan.FromDays(10),
                DateTime.MaxValue - TimeSpan.FromDays(10));
            DateTime end = RandomDate(start, DateTime.MaxValue - TimeSpan.FromDays(10));
            DateTime testValue = RandomDate(start, end);

            Assert.AreEqual(
                testValue,
                (new DateRange(start, end)).Bind(testValue),
                "Bind should return the input if it is within the range");
        }
    }
}