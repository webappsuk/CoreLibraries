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
    public class LocalDateTimeRangeTests
    {
        [NotNull]
        public static readonly Period MinPeriod = Period.FromTicks(0);

        [NotNull]
        public static readonly Period MaxPeriod = Period.FromTicks((InstantRangeTests.TestMaxInstant - InstantRangeTests.TestMinInstant).Ticks).Normalize();

        public static readonly LocalDateTime TestMinLocalDateTime = ZonedDateTimeRangeTests.TestMinZonedDateTime.LocalDateTime;
        public static readonly LocalDateTime TestMaxLocalDateTime = ZonedDateTimeRangeTests.TestMaxZonedDateTime.LocalDateTime;

        [NotNull]
        protected static readonly Random Random = Tester.RandomGenerator;

        /// <summary>
        /// Choose a random date and time within a given range.
        /// </summary>
        /// <param name="minimum">The minimum date possible.</param>
        /// <param name="maximum">The maximum date possible.</param>
        /// <returns>
        /// A <see cref="T:System.LocalDateTime"/> between <paramref name="minimum"/> and <paramref name="maximum"/>.
        /// </returns>
        private static LocalDateTime RandomLocalDateTime(LocalDateTime minimum, LocalDateTime maximum)
        {
            return minimum + RandomPeriod(null, Period.Between(minimum, maximum));
        }

        /// <summary>
        /// Choose a random time span with length within a given range.
        /// </summary>
        /// <param name="minimum">The minumum Period.</param>
        /// <param name="maximum">The maximum Period.</param>
        /// <param name="hasDate">if set to <see langword="true" /> the returned period will have a date component.</param>
        /// <param name="hasTime">if set to <see langword="true" /> the returned period will have a time component.</param>
        /// <returns>
        /// A <see cref="T:System.Period" /> between <paramref name="minimum" /> and <paramref name="maximum" />.
        /// </returns>
        [NotNull]
        public static Period RandomPeriod(
            [CanBeNull] Period minimum,
            [NotNull] Period maximum,
            bool hasDate = true,
            bool hasTime = true)
        {
            if (minimum != null)
                minimum = minimum.Normalize();
            // ReSharper disable once AssignNullToNotNullAttribute
            maximum = maximum.Normalize();

            // ReSharper disable once PossibleNullReferenceException
            Period diff = minimum == null ? maximum : (maximum - minimum).Normalize();
            PeriodBuilder builder = new PeriodBuilder();

            Assert.IsNotNull(diff);

            bool first = true;

            if (hasDate)
            {
                if (diff.Years != 0)
                {
                    builder.Years = (long)(diff.Years * Random.NextDouble());
                    first = false;
                }

                if (first && diff.Months != 0)
                {
                    builder.Months = (long)(diff.Months * Random.NextDouble());
                    first = false;
                }
                else if (!first)
                    builder.Months = Random.Next(12);

                if (first && diff.Weeks != 0)
                {
                    builder.Weeks = (long)(diff.Weeks * Random.NextDouble());
                    first = false;
                }
                else if (!first)
                    builder.Weeks = Random.Next(4);

                if (first && diff.Days != 0)
                {
                    builder.Days = (long)(diff.Days * Random.NextDouble());
                    first = false;
                }
                else if (!first)
                    builder.Days = Random.Next(NodaConstants.DaysPerStandardWeek);

                if (minimum != null)
                {
                    builder.Years += minimum.Years;
                    builder.Months += minimum.Months;
                    builder.Weeks += minimum.Weeks;
                    builder.Days += minimum.Days;
                }
            }
            else
                first = !diff.HasDateComponent;

            if (hasTime)
            {
                if (first && diff.Hours != 0)
                {
                    builder.Hours = (long)(diff.Hours * Random.NextDouble());
                    first = false;
                }
                else if (!first)
                    builder.Hours = Random.Next(NodaConstants.HoursPerStandardDay);

                if (first && diff.Minutes != 0)
                {
                    builder.Minutes = (long)(diff.Minutes * Random.NextDouble());
                    first = false;
                }
                else if (!first)
                    builder.Minutes = Random.Next(NodaConstants.MinutesPerHour);

                if (first && diff.Seconds != 0)
                {
                    builder.Seconds = (long)(diff.Seconds * Random.NextDouble());
                    first = false;
                }
                else if (!first)
                    builder.Seconds = Random.Next(NodaConstants.SecondsPerMinute);

                if (first && diff.Milliseconds != 0)
                {
                    builder.Milliseconds = (long)(diff.Milliseconds * Random.NextDouble());
                    first = false;
                }
                else if (!first)
                    builder.Milliseconds = Random.Next(NodaConstants.MillisecondsPerSecond);

                if (first && diff.Ticks != 0)
                {
                    builder.Ticks = (long)(diff.Ticks * Random.NextDouble());
                    first = false;
                }
                else if (!first)
                    builder.Ticks = Random.Next((int)NodaConstants.TicksPerMillisecond);

                if (minimum != null)
                {
                    builder.Years += minimum.Hours;
                    builder.Months += minimum.Minutes;
                    builder.Weeks += minimum.Seconds;
                    builder.Days += minimum.Milliseconds;
                    builder.Days += minimum.Ticks;
                }
            }

            return builder.Build().Normalize();
        }

        [NotNull]
        public static Period PeriodDivide([NotNull] Period period, int divisor)
        {
            PeriodBuilder builder = period.Normalize().ToBuilder();

            if (builder.Years != 0)
            {
                if (builder.Years > divisor)
                {
                    builder.Years /= divisor;
                    return builder.Build().Normalize();
                }

                builder.Months += builder.Years * 12;
                builder.Years = 0;
            }

            if (builder.Months != 0)
            {
                if (builder.Months > divisor)
                {
                    builder.Months /= divisor;
                    return builder.Build().Normalize();
                }

                builder.Weeks += builder.Months * 4;
                builder.Months = 0;
            }

            if (builder.Weeks != 0)
            {
                if (builder.Weeks > divisor)
                {
                    builder.Weeks /= divisor;
                    return builder.Build().Normalize();
                }

                builder.Days += builder.Weeks * NodaConstants.DaysPerStandardWeek;
                builder.Weeks = 0;
            }

            if (builder.Days != 0)
            {
                if (builder.Days > divisor)
                {
                    builder.Days /= divisor;
                    return builder.Build().Normalize();
                }

                builder.Hours += builder.Days * NodaConstants.HoursPerStandardDay;
                builder.Days = 0;
            }

            if (builder.Hours != 0)
            {
                if (builder.Hours > divisor)
                {
                    builder.Hours /= divisor;
                    return builder.Build().Normalize();
                }

                builder.Minutes += builder.Hours * NodaConstants.MinutesPerHour;
                builder.Hours = 0;
            }

            if (builder.Minutes != 0)
            {
                if (builder.Minutes > divisor)
                {
                    builder.Minutes /= divisor;
                    return builder.Build().Normalize();
                }

                builder.Seconds += builder.Minutes * NodaConstants.SecondsPerMinute;
                builder.Minutes = 0;
            }

            if (builder.Seconds != 0)
            {
                if (builder.Seconds > divisor)
                {
                    builder.Seconds /= divisor;
                    return builder.Build().Normalize();
                }

                builder.Milliseconds += builder.Seconds * NodaConstants.MillisecondsPerSecond;
                builder.Seconds = 0;
            }

            if (builder.Milliseconds != 0)
            {
                if (builder.Milliseconds > divisor)
                {
                    builder.Milliseconds /= divisor;
                    return builder.Build().Normalize();
                }

                builder.Ticks += builder.Milliseconds * NodaConstants.TicksPerMillisecond;
                builder.Milliseconds = 0;
            }

            builder.Ticks /= divisor;

            return builder.Build().Normalize();
        }

        /// <summary>
        /// Choose a random time span of more than zero and less than one day.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Period"/> which is greater than zero but less than one day.
        /// </returns>
        [NotNull]
        private static Period RandomTimeOffset()
        {
            return RandomPeriod(null, Period.FromDays(1), false);
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void ToString_IsNotBlank()
        {
            Period length = RandomPeriod(MinPeriod + MinPeriod, MaxPeriod);
            LocalDateTime start = RandomLocalDateTime(TestMinLocalDateTime, TestMaxLocalDateTime - length);
            LocalDateTime end = start + length;
            Period step = RandomPeriod(MinPeriod, length - MinPeriod);

            LocalDateTimeRange localDateTimeRange = new LocalDateTimeRange(start, end, step);

            Assert.AreNotEqual(
                "",
                localDateTimeRange.ToString(),
                "String representation of range must not be an empty string");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void ToString_DoesDependOnTimeComponents()
        {
            Period length = RandomPeriod(MinPeriod + MinPeriod, MaxPeriod);
            LocalDateTime start = RandomLocalDateTime(TestMinLocalDateTime, TestMaxLocalDateTime - length);
            LocalDateTime end = start + length;
            Period step = RandomPeriod(MinPeriod, length - MinPeriod);

            LocalDateTimeRange localDateTimeRange = new LocalDateTimeRange(start, end, step);
            LocalDateTimeRange localDateTimeRangeWithTime = new LocalDateTimeRange(
                start + RandomTimeOffset(),
                end + RandomTimeOffset(),
                step);

            Assert.AreNotEqual(
                localDateTimeRange.ToString(),
                localDateTimeRangeWithTime.ToString(),
                "String representation of range should depend on the time components");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void ToString_HasCorrectFormat()
        {
            Period length = RandomPeriod(MinPeriod + MinPeriod, MaxPeriod);
            LocalDateTime start = RandomLocalDateTime(TestMinLocalDateTime, TestMaxLocalDateTime - length);
            LocalDateTime end = start + length;
            Period step = RandomPeriod(MinPeriod, length - MinPeriod);

            LocalDateTimeRange localDateTimeRange = new LocalDateTimeRange(start, end, step);

            Regex formatTest =
                new Regex(
                    @"^-?\d{4,5}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2} \w+ \(\+\d{2}\) - -?\d{4,5}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2} \w+ \(\+\d{2}\)$");

            Assert.IsTrue(
                formatTest.IsMatch(localDateTimeRange.ToString()),
                "String representation of range should be of format yyyy-mm-ddThh:mm:ss zone - yyyy-mm-ddThh:mm:ss zone");
        }

        [NotNull]
        private static LocalDateTimeRange GenerateLocalDateTimeRangeWithStepSmallerThanRange(
            out LocalDateTime start,
            out LocalDateTime end,
            out Period step)
        {
            Period length = RandomPeriod(MinPeriod + MinPeriod, MaxPeriod);
            start = RandomLocalDateTime(TestMinLocalDateTime, TestMaxLocalDateTime - length);
            end = start + length;
            step = RandomPeriod(MinPeriod, length - MinPeriod);

            return new LocalDateTimeRange(start, end, step);
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Start_StepSmallerThanRange_MatchesThatGivenWithTimeStripped()
        {
            LocalDateTime start, end;
            Period step;
            LocalDateTimeRange localDateTimeRange = GenerateLocalDateTimeRangeWithStepSmallerThanRange(
                out start,
                out end,
                out step);

            Assert.AreEqual(start, localDateTimeRange.Start, "Starting point field must match the value supplied.");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void End_StepSmallerThanRange_MatchesThatGivenWithTimeStripped()
        {
            LocalDateTime start, end;
            Period step;
            LocalDateTimeRange localDateTimeRange = GenerateLocalDateTimeRangeWithStepSmallerThanRange(
                out start,
                out end,
                out step);

            Assert.AreEqual(end, localDateTimeRange.End, "End point field must match the value supplied.");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Step_StepSmallerThanRange_MatchesThatGivenWithTimeStripped()
        {
            LocalDateTime start, end;
            Period step;
            LocalDateTimeRange localDateTimeRange = GenerateLocalDateTimeRangeWithStepSmallerThanRange(
                out start,
                out end,
                out step);

            Assert.AreEqual(step, localDateTimeRange.Step, "Step amount field must match the value supplied.");
        }

        [NotNull]
        private static LocalDateTimeRange GenerateLocalDateTimeRangeWithStepLargerThanRange(
            out LocalDateTime start,
            out LocalDateTime end,
            out Period step)
        {
            // subtracting MinPeriod twice from max length to give space for oversized step without converting type to perform the calculation to do so
            Period length = RandomPeriod(MinPeriod, MaxPeriod - MinPeriod - MinPeriod);
            start = RandomLocalDateTime(TestMinLocalDateTime, TestMaxLocalDateTime - length);
            end = start + length;
            step = RandomPeriod(length + MinPeriod, length + MinPeriod + MinPeriod);

            return new LocalDateTimeRange(start, end, step);
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Start_StepIsLargerThanRange_MatchesThatGivenWithTimeStripped()
        {
            LocalDateTime start, end;
            Period step;
            LocalDateTimeRange localDateTimeRange = GenerateLocalDateTimeRangeWithStepLargerThanRange(
                out start,
                out end,
                out step);

            Assert.AreEqual(start, localDateTimeRange.Start, "Starting point field must match the value supplied.");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void End_StepIsLargerThanRange_MatchesThatGivenWithTimeStripped()
        {
            LocalDateTime start, end;
            Period step;
            LocalDateTimeRange localDateTimeRange = GenerateLocalDateTimeRangeWithStepLargerThanRange(
                out start,
                out end,
                out step);

            Assert.AreEqual(end, localDateTimeRange.End, "End point field must match the value supplied.");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Step_StepIsLargerThanRange_MatchesThatGivenWithTimeStripped()
        {
            LocalDateTime start, end;
            Period step;
            LocalDateTimeRange localDateTimeRange = GenerateLocalDateTimeRangeWithStepLargerThanRange(
                out start,
                out end,
                out step);

            Assert.AreEqual(step, localDateTimeRange.Step, "Step amount field must match the value supplied.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))] // ReSharper disable once InconsistentNaming
        public void Constructor_EndBeforeStart_ThrowsArgumentOutOfRangeException()
        {
            Period length = RandomPeriod(MinPeriod, MaxPeriod);
            LocalDateTime start = RandomLocalDateTime(TestMinLocalDateTime + length, TestMaxLocalDateTime);
            LocalDateTime end = start - length;

            // ReSharper disable once UnusedVariable
            LocalDateTimeRange localDateTimeRange = new LocalDateTimeRange(start, end);
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Constructor_WithoutStepParam_DeltaLessThanMillisecond_StepDefaultsToOneTick()
        {
            Period length = RandomPeriod(Period.Zero, Period.FromMilliseconds(1));
            Trace.WriteLine(length);
            LocalDateTime start = RandomLocalDateTime(TestMinLocalDateTime, TestMaxLocalDateTime - length);
            LocalDateTime end = start + length;

            LocalDateTimeRange localDateTimeRange = new LocalDateTimeRange(start, end);

            Assert.AreEqual(start, localDateTimeRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, localDateTimeRange.End, "End point field must match the value supplied");
            Assert.AreEqual(Period.FromTicks(1), localDateTimeRange.Step, "Step amount must default to one tick");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Constructor_WithoutStepParam_DeltaLessThanSecond_StepDefaultsToOneMillisecond()
        {
            Period length = RandomPeriod(Period.FromMilliseconds(1), Period.FromSeconds(1));
            Trace.WriteLine(length);
            LocalDateTime start = RandomLocalDateTime(TestMinLocalDateTime, TestMaxLocalDateTime - length);
            LocalDateTime end = start + length;

            LocalDateTimeRange localDateTimeRange = new LocalDateTimeRange(start, end);

            Assert.AreEqual(start, localDateTimeRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, localDateTimeRange.End, "End point field must match the value supplied");
            Assert.AreEqual(
                Period.FromMilliseconds(1),
                localDateTimeRange.Step,
                "Step amount must default to one millisecond");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Constructor_WithoutStepParam_DeltaLessThanMinute_StepDefaultsToOneSecond()
        {
            Period length = RandomPeriod(Period.FromSeconds(1), Period.FromMinutes(1));
            Trace.WriteLine(length);
            LocalDateTime start = RandomLocalDateTime(TestMinLocalDateTime, TestMaxLocalDateTime - length);
            LocalDateTime end = start + length;

            LocalDateTimeRange localDateTimeRange = new LocalDateTimeRange(start, end);

            Assert.AreEqual(start, localDateTimeRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, localDateTimeRange.End, "End point field must match the value supplied");
            Assert.AreEqual(Period.FromSeconds(1), localDateTimeRange.Step, "Step amount must default to one second");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Constructor_WithoutStepParam_DeltaLessThanHour_StepDefaultsToOneMinute()
        {
            Period length = RandomPeriod(Period.FromMinutes(1), Period.FromHours(1));
            Trace.WriteLine(length);
            LocalDateTime start = RandomLocalDateTime(TestMinLocalDateTime, TestMaxLocalDateTime - length);
            LocalDateTime end = start + length;

            LocalDateTimeRange localDateTimeRange = new LocalDateTimeRange(start, end);

            Assert.AreEqual(start, localDateTimeRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, localDateTimeRange.End, "End point field must match the value supplied");
            Assert.AreEqual(Period.FromMinutes(1), localDateTimeRange.Step, "Step amount must default to one minute");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Constructor_WithoutStepParam_DeltaLessThanDay_StepDefaultsToOneHour()
        {
            Period length = RandomPeriod(Period.FromHours(1), Period.FromDays(1));
            Trace.WriteLine(length);
            LocalDateTime start = RandomLocalDateTime(TestMinLocalDateTime, TestMaxLocalDateTime - length);
            LocalDateTime end = start + length;

            LocalDateTimeRange localDateTimeRange = new LocalDateTimeRange(start, end);

            Assert.AreEqual(start, localDateTimeRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, localDateTimeRange.End, "End point field must match the value supplied");
            Assert.AreEqual(Period.FromHours(1), localDateTimeRange.Step, "Step amount must default to one hour");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Constructor_WithoutStepParam_DeltaGreaterThanDay_StepDefaultsToOneDay()
        {
            Period length = RandomPeriod(Period.FromDays(1), MaxPeriod);
            Trace.WriteLine(length);
            LocalDateTime start = RandomLocalDateTime(TestMinLocalDateTime, TestMaxLocalDateTime - length);
            LocalDateTime end = start + length;

            LocalDateTimeRange localDateTimeRange = new LocalDateTimeRange(start, end);

            Assert.AreEqual(start, localDateTimeRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, localDateTimeRange.End, "End point field must match the value supplied");
            Assert.AreEqual(
                Period.FromDays(1),
                localDateTimeRange.Step,
                "Step amount must default to one day");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void GetEnumerator_ValuesStayWithinRange()
        {
            Period length = RandomPeriod(MinPeriod, MaxPeriod);
            LocalDateTime start = RandomLocalDateTime(TestMinLocalDateTime, TestMaxLocalDateTime - length);
            LocalDateTime end = start + length;
            // note that the number of steps is limited to 100 or fewer
            Period step = PeriodDivide(length, Random.Next(4, 100));

            LocalDateTimeRange localDateTimeRange = new LocalDateTimeRange(start, end, step);

            foreach (LocalDateTime d in localDateTimeRange)
            {
                Assert.IsTrue(d >= start, "Value from iterator must by equal or above start parameter");
                Assert.IsTrue(d <= end, "Value from iterator must be equal or below end parameter");
            }
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void GetEnumerator_LengthDivisibleByStep_IterationCountMatchesCalculated()
        {
            Period length = RandomPeriod(MinPeriod, MaxPeriod);
            LocalDateTime start = RandomLocalDateTime(TestMinLocalDateTime, TestMaxLocalDateTime - length);
            LocalDateTime end = start + length;
            // note that the number of steps is limited to 1000 or fewer
            int steps = Random.Next(4, 1000);
            Period step = PeriodDivide(length, steps);

            //ensure that step size is a factor of the length of the range
            start += Period.FromTicks(length.Ticks % step.Ticks);

            LocalDateTimeRange localDateTimeRange = new LocalDateTimeRange(start, end, step);

            // Range endpoint is inclusive, so must take longo account this extra iteration
            Assert.AreEqual(
                steps,
                localDateTimeRange.Count(),
                "Iteration count should be (end-start)/step + 1 where endpoint is included");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void GetEnumerator_LengthNotDivisibleByStep_IterationCountMatchesCalculated()
        {
            Period length = RandomPeriod(MinPeriod, MaxPeriod);
            LocalDateTime start = RandomLocalDateTime(TestMinLocalDateTime, TestMaxLocalDateTime - length);
            LocalDateTime end = start + length;
            // note that the number of steps is limited to 1000 or fewer
            int steps = Random.Next(4, 1000);
            Period step = PeriodDivide(length, steps);

            //ensure that step size is not a factor of the length of the range
            if (length.Ticks % step.Ticks == 0)
            {
                start += RandomPeriod(MinPeriod, step - MinPeriod);
                length = Period.Between(start, end);
            }

            LocalDateTimeRange localDateTimeRange = new LocalDateTimeRange(start, end, step);

            Assert.AreEqual(
                steps,
                localDateTimeRange.Count(),
                "Iteration count should be (start-end)/step +1");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void GetEnumerator_Iterating_DifferenceBetweenIterationsMatchesStepSize()
        {
            Period length = RandomPeriod(MinPeriod, MaxPeriod);
            LocalDateTime start = RandomLocalDateTime(TestMinLocalDateTime, TestMaxLocalDateTime - length);
            LocalDateTime end = start + length;
            // note that the number of steps is limited to 100 or fewer
            Period step = PeriodDivide(length, Random.Next(4, 100));

            LocalDateTimeRange localDateTimeRange = new LocalDateTimeRange(start, end, step);

            LocalDateTime? previous = null;
            foreach (LocalDateTime d in localDateTimeRange)
            {
                if (previous.HasValue)
                {
                    Assert.AreEqual(
                        Period.Between(previous.Value, d).Normalize(),
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
            LocalDateTimeRange localDateTimeRange = new LocalDateTimeRange(
                TestMinLocalDateTime,
                TestMaxLocalDateTime,
                PeriodDivide(MaxPeriod, 16));

            bool iterated = localDateTimeRange.Any();

            Assert.AreEqual(true, iterated, "When iterating across full range, at least one value should be returned");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Bind_NumberBelowRange_ReturnsStart()
        {
            LocalDateTime start = RandomLocalDateTime(
                TestMinLocalDateTime + Period.FromDays(10),
                TestMaxLocalDateTime - Period.FromDays(10));
            LocalDateTime end = RandomLocalDateTime(start, TestMaxLocalDateTime - Period.FromDays(10));
            LocalDateTime testValue = RandomLocalDateTime(TestMinLocalDateTime, start);

            Assert.AreEqual(
                start,
                (new LocalDateTimeRange(start, end)).Bind(testValue),
                "Bind should return the lower bound of the range if the input is below the range");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Bind_NumberAboveRange_ReturnsEnd()
        {
            LocalDateTime start = RandomLocalDateTime(
                TestMinLocalDateTime + Period.FromDays(10),
                TestMaxLocalDateTime - Period.FromDays(10));
            LocalDateTime end = RandomLocalDateTime(start, TestMaxLocalDateTime - Period.FromDays(10));
            LocalDateTime testValue = RandomLocalDateTime(end, TestMaxLocalDateTime);

            Assert.AreEqual(
                end,
                (new LocalDateTimeRange(start, end)).Bind(testValue),
                "Bind should return the upper bound of the range if the input is above the range");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Bind_NumberWithinRange_ReturnsInput()
        {
            LocalDateTime start = RandomLocalDateTime(
                TestMinLocalDateTime + Period.FromDays(10),
                TestMaxLocalDateTime - Period.FromDays(10));
            LocalDateTime end = RandomLocalDateTime(start, TestMaxLocalDateTime - Period.FromDays(10));
            LocalDateTime testValue = RandomLocalDateTime(start, end);

            Assert.AreEqual(
                testValue,
                (new LocalDateTimeRange(start, end)).Bind(testValue),
                "Bind should return the input if it is within the range");
        }
    }
}