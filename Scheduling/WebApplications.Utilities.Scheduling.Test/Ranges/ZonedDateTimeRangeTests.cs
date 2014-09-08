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
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using WebApplications.Testing;
using WebApplications.Utilities.Scheduling.Ranges;

namespace WebApplications.Utilities.Scheduling.Test.Ranges
{
    [TestClass]
    public class ZonedDateTimeRangeTests
    {
        public static readonly Duration MinDuration = Duration.FromSeconds(1);
        public static readonly Duration MaxDuration = Duration.FromTicks(long.MaxValue);

        public static readonly ZonedDateTime TestMinZonedDateTime = new ZonedDateTime(
            InstantRangeTests.TestMinInstant,
            DateTimeZone.Utc);

        public static readonly ZonedDateTime TestMaxZonedDateTime = new ZonedDateTime(
            InstantRangeTests.TestMaxInstant,
            DateTimeZone.Utc);

        [NotNull]
        protected static readonly Random Random = Tester.RandomGenerator;

        /// <summary>
        /// Choose a random date and time within a given range.
        /// </summary>
        /// <param name="minimum">The minimum date possible.</param>
        /// <param name="maximum">The maximum date possible.</param>
        /// <returns>
        /// A <see cref="T:System.ZonedDateTime"/> between <paramref name="minimum"/> and <paramref name="maximum"/>.
        /// </returns>
        private static ZonedDateTime RandomZonedDateTime(ZonedDateTime minimum, ZonedDateTime maximum)
        {
            return minimum +
                   Duration.FromTicks((long)((maximum.ToInstant() - minimum.ToInstant()).Ticks * Random.NextDouble()));
        }

        /// <summary>
        /// Choose a random time span with length within a given range.
        /// </summary>
        /// <param name="minimum">The minumum Duration.</param>
        /// <param name="maximum">The maximum Duration.</param>
        /// <returns>
        /// A <see cref="T:System.Duration"/> between <paramref name="minimum"/> and <paramref name="maximum"/>.
        /// </returns>
        private static Duration RandomDuration(Duration minimum, Duration maximum)
        {
            return minimum + Duration.FromTicks((long)((maximum - minimum).Ticks * Random.NextDouble()));
        }

        /// <summary>
        /// Choose a random time span of more than zero and less than one day.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Duration"/> which is greater than zero but less than one day.
        /// </returns>
        private static Duration RandomTimeOffset()
        {
            return Duration.FromTicks((long)((Random.NextDouble() * 11 + 0.5) * 36000000000));
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void ToString_IsNotBlank()
        {
            Duration length = RandomDuration(MinDuration + MinDuration, MaxDuration);
            ZonedDateTime start = RandomZonedDateTime(TestMinZonedDateTime, TestMaxZonedDateTime - length);
            ZonedDateTime end = start + length;
            Duration step = RandomDuration(MinDuration, length - MinDuration);

            ZonedDateTimeRange zonedDateTimeRange = new ZonedDateTimeRange(start, end, step);

            Assert.AreNotEqual(
                "",
                zonedDateTimeRange.ToString(),
                "String representation of range must not be an empty string");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void ToString_DoesDependOnTimeComponents()
        {
            Duration length = RandomDuration(MinDuration + MinDuration, MaxDuration);
            ZonedDateTime start = RandomZonedDateTime(TestMinZonedDateTime, TestMaxZonedDateTime - length);
            ZonedDateTime end = start + length;
            Duration step = RandomDuration(MinDuration, length - MinDuration);

            ZonedDateTimeRange zonedDateTimeRange = new ZonedDateTimeRange(start, end, step);
            ZonedDateTimeRange zonedDateTimeRangeWithTime = new ZonedDateTimeRange(
                start + RandomTimeOffset(),
                end + RandomTimeOffset(),
                step);

            Assert.AreNotEqual(
                zonedDateTimeRange.ToString(),
                zonedDateTimeRangeWithTime.ToString(),
                "String representation of range should depend on the time components");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void ToString_HasCorrectFormat()
        {
            Duration length = RandomDuration(MinDuration + MinDuration, MaxDuration);
            ZonedDateTime start = RandomZonedDateTime(TestMinZonedDateTime, TestMaxZonedDateTime - length);
            ZonedDateTime end = start + length;
            Duration step = RandomDuration(MinDuration, length - MinDuration);

            ZonedDateTimeRange zonedDateTimeRange = new ZonedDateTimeRange(start, end, step);

            Regex formatTest =
                new Regex(
                    @"^-?\d{4,5}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2} \w+ \(\+\d{2}\) - -?\d{4,5}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2} \w+ \(\+\d{2}\)$");

            Assert.IsTrue(
                formatTest.IsMatch(zonedDateTimeRange.ToString()),
                "String representation of range should be of format yyyy-mm-ddThh:mm:ss zone - yyyy-mm-ddThh:mm:ss zone");
        }

        [NotNull]
        private static ZonedDateTimeRange GenerateZonedDateTimeRangeWithStepSmallerThanRange(
            out ZonedDateTime start,
            out ZonedDateTime end,
            out Duration step)
        {
            Duration length = RandomDuration(MinDuration + MinDuration, MaxDuration);
            start = RandomZonedDateTime(TestMinZonedDateTime, TestMaxZonedDateTime - length);
            end = start + length;
            step = RandomDuration(MinDuration, length - MinDuration);

            return new ZonedDateTimeRange(start, end, step);
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Start_StepSmallerThanRange_MatchesThatGivenWithTimeStripped()
        {
            ZonedDateTime start, end;
            Duration step;
            ZonedDateTimeRange zonedDateTimeRange = GenerateZonedDateTimeRangeWithStepSmallerThanRange(
                out start,
                out end,
                out step);

            Assert.AreEqual(start, zonedDateTimeRange.Start, "Starting point field must match the value supplied.");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void End_StepSmallerThanRange_MatchesThatGivenWithTimeStripped()
        {
            ZonedDateTime start, end;
            Duration step;
            ZonedDateTimeRange zonedDateTimeRange = GenerateZonedDateTimeRangeWithStepSmallerThanRange(
                out start,
                out end,
                out step);

            Assert.AreEqual(end, zonedDateTimeRange.End, "End point field must match the value supplied.");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Step_StepSmallerThanRange_MatchesThatGivenWithTimeStripped()
        {
            ZonedDateTime start, end;
            Duration step;
            ZonedDateTimeRange zonedDateTimeRange = GenerateZonedDateTimeRangeWithStepSmallerThanRange(
                out start,
                out end,
                out step);

            Assert.AreEqual(step, zonedDateTimeRange.Step, "Step amount field must match the value supplied.");
        }

        [NotNull]
        private static ZonedDateTimeRange GenerateZonedDateTimeRangeWithStepLargerThanRange(
            out ZonedDateTime start,
            out ZonedDateTime end,
            out Duration step)
        {
            // subtracting MinDuration twice from max length to give space for oversized step without converting type to perform the calculation to do so
            Duration length = RandomDuration(MinDuration, MaxDuration - MinDuration - MinDuration);
            start = RandomZonedDateTime(TestMinZonedDateTime, TestMaxZonedDateTime - length);
            end = start + length;
            step = RandomDuration(length + MinDuration, length + MinDuration + MinDuration);

            return new ZonedDateTimeRange(start, end, step);
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Start_StepIsLargerThanRange_MatchesThatGivenWithTimeStripped()
        {
            ZonedDateTime start, end;
            Duration step;
            ZonedDateTimeRange zonedDateTimeRange = GenerateZonedDateTimeRangeWithStepLargerThanRange(
                out start,
                out end,
                out step);

            Assert.AreEqual(start, zonedDateTimeRange.Start, "Starting point field must match the value supplied.");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void End_StepIsLargerThanRange_MatchesThatGivenWithTimeStripped()
        {
            ZonedDateTime start, end;
            Duration step;
            ZonedDateTimeRange zonedDateTimeRange = GenerateZonedDateTimeRangeWithStepLargerThanRange(
                out start,
                out end,
                out step);

            Assert.AreEqual(end, zonedDateTimeRange.End, "End point field must match the value supplied.");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Step_StepIsLargerThanRange_MatchesThatGivenWithTimeStripped()
        {
            ZonedDateTime start, end;
            Duration step;
            ZonedDateTimeRange zonedDateTimeRange = GenerateZonedDateTimeRangeWithStepLargerThanRange(
                out start,
                out end,
                out step);

            Assert.AreEqual(step, zonedDateTimeRange.Step, "Step amount field must match the value supplied.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))] // ReSharper disable once InconsistentNaming
        public void Constructor_EndBeforeStart_ThrowsArgumentOutOfRangeException()
        {
            Duration length = RandomDuration(MinDuration, MaxDuration);
            ZonedDateTime start = RandomZonedDateTime(TestMinZonedDateTime + length, TestMaxZonedDateTime);
            ZonedDateTime end = start - length;

            // ReSharper disable once UnusedVariable
            ZonedDateTimeRange zonedDateTimeRange = new ZonedDateTimeRange(start, end);
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Constructor_WithoutStepParam_DeltaLessThanMillisecond_StepDefaultsToOneTick()
        {
            Duration length = RandomDuration(Duration.Zero, Duration.FromMilliseconds(1));
            Trace.WriteLine(length);
            ZonedDateTime start = RandomZonedDateTime(TestMinZonedDateTime, TestMaxZonedDateTime - length);
            ZonedDateTime end = start + length;

            ZonedDateTimeRange zonedDateTimeRange = new ZonedDateTimeRange(start, end);

            Assert.AreEqual(start, zonedDateTimeRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, zonedDateTimeRange.End, "End point field must match the value supplied");
            Assert.AreEqual(Duration.FromTicks(1), zonedDateTimeRange.Step, "Step amount must default to one tick");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Constructor_WithoutStepParam_DeltaLessThanSecond_StepDefaultsToOneMillisecond()
        {
            Duration length = RandomDuration(Duration.FromMilliseconds(1), Duration.FromSeconds(1));
            Trace.WriteLine(length);
            ZonedDateTime start = RandomZonedDateTime(TestMinZonedDateTime, TestMaxZonedDateTime - length);
            ZonedDateTime end = start + length;

            ZonedDateTimeRange zonedDateTimeRange = new ZonedDateTimeRange(start, end);

            Assert.AreEqual(start, zonedDateTimeRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, zonedDateTimeRange.End, "End point field must match the value supplied");
            Assert.AreEqual(
                Duration.FromMilliseconds(1),
                zonedDateTimeRange.Step,
                "Step amount must default to one millisecond");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Constructor_WithoutStepParam_DeltaLessThanMinute_StepDefaultsToOneSecond()
        {
            Duration length = RandomDuration(Duration.FromSeconds(1), Duration.FromMinutes(1));
            Trace.WriteLine(length);
            ZonedDateTime start = RandomZonedDateTime(TestMinZonedDateTime, TestMaxZonedDateTime - length);
            ZonedDateTime end = start + length;

            ZonedDateTimeRange zonedDateTimeRange = new ZonedDateTimeRange(start, end);

            Assert.AreEqual(start, zonedDateTimeRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, zonedDateTimeRange.End, "End point field must match the value supplied");
            Assert.AreEqual(Duration.FromSeconds(1), zonedDateTimeRange.Step, "Step amount must default to one second");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Constructor_WithoutStepParam_DeltaLessThanHour_StepDefaultsToOneMinute()
        {
            Duration length = RandomDuration(Duration.FromMinutes(1), Duration.FromHours(1));
            Trace.WriteLine(length);
            ZonedDateTime start = RandomZonedDateTime(TestMinZonedDateTime, TestMaxZonedDateTime - length);
            ZonedDateTime end = start + length;

            ZonedDateTimeRange zonedDateTimeRange = new ZonedDateTimeRange(start, end);

            Assert.AreEqual(start, zonedDateTimeRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, zonedDateTimeRange.End, "End point field must match the value supplied");
            Assert.AreEqual(Duration.FromMinutes(1), zonedDateTimeRange.Step, "Step amount must default to one minute");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Constructor_WithoutStepParam_DeltaLessThanDay_StepDefaultsToOneHour()
        {
            Duration length = RandomDuration(Duration.FromHours(1), Duration.FromStandardDays(1));
            Trace.WriteLine(length);
            ZonedDateTime start = RandomZonedDateTime(TestMinZonedDateTime, TestMaxZonedDateTime - length);
            ZonedDateTime end = start + length;

            ZonedDateTimeRange zonedDateTimeRange = new ZonedDateTimeRange(start, end);

            Assert.AreEqual(start, zonedDateTimeRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, zonedDateTimeRange.End, "End point field must match the value supplied");
            Assert.AreEqual(Duration.FromHours(1), zonedDateTimeRange.Step, "Step amount must default to one hour");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Constructor_WithoutStepParam_DeltaGreaterThanDay_StepDefaultsToOneDay()
        {
            Duration length = RandomDuration(Duration.FromStandardDays(1), MaxDuration);
            Trace.WriteLine(length);
            ZonedDateTime start = RandomZonedDateTime(TestMinZonedDateTime, TestMaxZonedDateTime - length);
            ZonedDateTime end = start + length;

            ZonedDateTimeRange zonedDateTimeRange = new ZonedDateTimeRange(start, end);

            Assert.AreEqual(start, zonedDateTimeRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, zonedDateTimeRange.End, "End point field must match the value supplied");
            Assert.AreEqual(
                Duration.FromStandardDays(1),
                zonedDateTimeRange.Step,
                "Step amount must default to one day");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void GetEnumerator_ValuesStayWithinRange()
        {
            Duration length = RandomDuration(MinDuration, MaxDuration);
            ZonedDateTime start = RandomZonedDateTime(TestMinZonedDateTime, TestMaxZonedDateTime - length);
            ZonedDateTime end = start + length;
            // note that the number of steps is limited to 100 or fewer
            Duration step = Duration.FromTicks(length.Ticks / Random.Next(4, 100));

            ZonedDateTimeRange zonedDateTimeRange = new ZonedDateTimeRange(start, end, step);

            foreach (ZonedDateTime d in zonedDateTimeRange)
            {
                Assert.IsTrue(d >= start, "Value from iterator must by equal or above start parameter");
                Assert.IsTrue(d <= end, "Value from iterator must be equal or below end parameter");
            }
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void GetEnumerator_LengthDivisibleByStep_IterationCountMatchesCalculated()
        {
            Duration length = RandomDuration(MinDuration, MaxDuration);
            ZonedDateTime start = RandomZonedDateTime(TestMinZonedDateTime, TestMaxZonedDateTime - length);
            ZonedDateTime end = start + length;
            // note that the number of steps is limited to 1000 or fewer
            Duration step = Duration.FromTicks(length.Ticks / Random.Next(4, 1000));

            //ensure that step size is a factor of the length of the range
            start += Duration.FromTicks(length.Ticks % step.Ticks);

            ZonedDateTimeRange zonedDateTimeRange = new ZonedDateTimeRange(start, end, step);

            // Range endpoint is inclusive, so must take longo account this extra iteration
            Assert.AreEqual(
                length.Ticks / step.Ticks + 1,
                zonedDateTimeRange.Count(),
                "Iteration count should be (end-start)/step + 1 where endpoint is included");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void GetEnumerator_LengthNotDivisibleByStep_IterationCountMatchesCalculated()
        {
            Duration length = RandomDuration(MinDuration, MaxDuration);
            ZonedDateTime start = RandomZonedDateTime(TestMinZonedDateTime, TestMaxZonedDateTime - length);
            ZonedDateTime end = start + length;
            // note that the number of steps is limited to 1000 or fewer
            Duration step = Duration.FromTicks(length.Ticks / Random.Next(4, 1000));

            //ensure that step size is not a factor of the length of the range
            if (length.Ticks % step.Ticks == 0)
            {
                start += RandomDuration(MinDuration, step - MinDuration);
                length = end.ToInstant() - start.ToInstant();
            }

            ZonedDateTimeRange zonedDateTimeRange = new ZonedDateTimeRange(start, end, step);

            Assert.AreEqual(
                length.Ticks / step.Ticks + 1,
                zonedDateTimeRange.Count(),
                "Iteration count should be (start-end)/step +1");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void GetEnumerator_Iterating_DifferenceBetweenIterationsMatchesStepSize()
        {
            Duration length = RandomDuration(MinDuration, MaxDuration);
            ZonedDateTime start = RandomZonedDateTime(TestMinZonedDateTime, TestMaxZonedDateTime - length);
            ZonedDateTime end = start + length;
            // note that the number of steps is limited to 100 or fewer
            Duration step = Duration.FromTicks(length.Ticks / Random.Next(4, 100));

            ZonedDateTimeRange zonedDateTimeRange = new ZonedDateTimeRange(start, end, step);

            ZonedDateTime? previous = null;
            foreach (ZonedDateTime d in zonedDateTimeRange)
            {
                if (previous.HasValue)
                {
                    Assert.AreEqual(
                        d.ToInstant() - previous.Value.ToInstant(),
                        step,
                        "Difference between iteration values should match the step value supplied");
                }
                previous = d;
            }
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void GetEnumerator_UsingLargestPossibleParameters_IteratesSuccessfully()
        {
            // Step chosen to avoid an unfeasible number of iterations
            ZonedDateTimeRange zonedDateTimeRange = new ZonedDateTimeRange(
                TestMinZonedDateTime,
                TestMaxZonedDateTime,
                Duration.FromTicks(MaxDuration.Ticks / 16));

            bool iterated = false;
            foreach (ZonedDateTime d in zonedDateTimeRange)
                iterated = true;

            Assert.AreEqual(true, iterated, "When iterating across full range, at least one value should be returned");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Bind_NumberBelowRange_ReturnsStart()
        {
            ZonedDateTime start = RandomZonedDateTime(
                TestMinZonedDateTime + Duration.FromStandardDays(10),
                TestMaxZonedDateTime - Duration.FromStandardDays(10));
            ZonedDateTime end = RandomZonedDateTime(start, TestMaxZonedDateTime - Duration.FromStandardDays(10));
            ZonedDateTime testValue = RandomZonedDateTime(TestMinZonedDateTime, start);

            Assert.AreEqual(
                start,
                (new ZonedDateTimeRange(start, end)).Bind(testValue),
                "Bind should return the lower bound of the range if the input is below the range");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Bind_NumberAboveRange_ReturnsEnd()
        {
            ZonedDateTime start = RandomZonedDateTime(
                TestMinZonedDateTime + Duration.FromStandardDays(10),
                TestMaxZonedDateTime - Duration.FromStandardDays(10));
            ZonedDateTime end = RandomZonedDateTime(start, TestMaxZonedDateTime - Duration.FromStandardDays(10));
            ZonedDateTime testValue = RandomZonedDateTime(end, TestMaxZonedDateTime);

            Assert.AreEqual(
                end,
                (new ZonedDateTimeRange(start, end)).Bind(testValue),
                "Bind should return the upper bound of the range if the input is above the range");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Bind_NumberWithinRange_ReturnsInput()
        {
            ZonedDateTime start = RandomZonedDateTime(
                TestMinZonedDateTime + Duration.FromStandardDays(10),
                TestMaxZonedDateTime - Duration.FromStandardDays(10));
            ZonedDateTime end = RandomZonedDateTime(start, TestMaxZonedDateTime - Duration.FromStandardDays(10));
            ZonedDateTime testValue = RandomZonedDateTime(start, end);

            Assert.AreEqual(
                testValue,
                (new ZonedDateTimeRange(start, end)).Bind(testValue),
                "Bind should return the input if it is within the range");
        }
    }
}