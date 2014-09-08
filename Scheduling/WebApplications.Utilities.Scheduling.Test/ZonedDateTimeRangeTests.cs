using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using WebApplications.Testing;
using WebApplications.Utilities.Scheduling.Ranges;

namespace WebApplications.Utilities.Scheduling.Test
{
    [TestClass]
    public class ZonedDateTimeRangeTests
    {
        private static readonly Duration MinDuration = Duration.FromSeconds(1);
        private static readonly Duration MaxDuration = Duration.FromTicks(long.MaxValue);
        private static readonly ZonedDateTime TestMinZonedDateTime = new ZonedDateTime(InstantRangeTests.TestMinInstant, DateTimeZone.Utc);
        private static readonly ZonedDateTime TestMaxZonedDateTime = new ZonedDateTime(InstantRangeTests.TestMaxInstant, DateTimeZone.Utc);
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
            return minimum + Duration.FromTicks((long)((maximum.ToInstant() - minimum.ToInstant()).Ticks * Random.NextDouble()));
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

        [TestMethod]
        public void ToString_IsNotBlank()
        {
            Duration length = RandomDuration(MinDuration + MinDuration, MaxDuration);
            ZonedDateTime start = RandomZonedDateTime(TestMinZonedDateTime, TestMaxZonedDateTime - length);
            ZonedDateTime end = start + length;
            Duration step = RandomDuration(MinDuration, length - MinDuration);

            ZonedDateTimeRange zonedDateTimeRange = new ZonedDateTimeRange(start, end, step);

            Assert.AreNotEqual("", zonedDateTimeRange.ToString(), "String representation of range must not be an empty string");
        }

        [TestMethod]
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

        [TestMethod]
        public void ToString_HasCorrectFormat()
        {
            Duration length = RandomDuration(MinDuration + MinDuration, MaxDuration);
            ZonedDateTime start = RandomZonedDateTime(TestMinZonedDateTime, TestMaxZonedDateTime - length);
            ZonedDateTime end = start + length;
            Duration step = RandomDuration(MinDuration, length - MinDuration);

            ZonedDateTimeRange zonedDateTimeRange = new ZonedDateTimeRange(start, end, step);

            Regex formatTest = new Regex(@"^-?\d{4,5}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2} \w+ \(\+\d{2}\) - -?\d{4,5}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2} \w+ \(\+\d{2}\)$");

            Assert.IsTrue(
                formatTest.IsMatch(zonedDateTimeRange.ToString()),
                "String representation of range should be of format yyyy-mm-ddThh:mm:ss zone - yyyy-mm-ddThh:mm:ss zone");
        }

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

        [TestMethod]
        public void Start_StepSmallerThanRange_MatchesThatGivenWithTimeStripped()
        {
            ZonedDateTime start, end;
            Duration step;
            ZonedDateTimeRange zonedDateTimeRange = GenerateZonedDateTimeRangeWithStepSmallerThanRange(out start, out end, out step);

            Assert.AreEqual(start, zonedDateTimeRange.Start, "Starting point field must match the value supplied.");
        }

        [TestMethod]
        public void End_StepSmallerThanRange_MatchesThatGivenWithTimeStripped()
        {
            ZonedDateTime start, end;
            Duration step;
            ZonedDateTimeRange zonedDateTimeRange = GenerateZonedDateTimeRangeWithStepSmallerThanRange(out start, out end, out step);

            Assert.AreEqual(end, zonedDateTimeRange.End, "End point field must match the value supplied.");
        }

        [TestMethod]
        public void Step_StepSmallerThanRange_MatchesThatGivenWithTimeStripped()
        {
            ZonedDateTime start, end;
            Duration step;
            ZonedDateTimeRange zonedDateTimeRange = GenerateZonedDateTimeRangeWithStepSmallerThanRange(out start, out end, out step);

            Assert.AreEqual(step, zonedDateTimeRange.Step, "Step amount field must match the value supplied.");
        }

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

        [TestMethod]
        public void Start_StepIsLargerThanRange_MatchesThatGivenWithTimeStripped()
        {
            ZonedDateTime start, end;
            Duration step;
            ZonedDateTimeRange ZonedDateTimeRange = GenerateZonedDateTimeRangeWithStepLargerThanRange(out start, out end, out step);

            Assert.AreEqual(start, ZonedDateTimeRange.Start, "Starting point field must match the value supplied.");
        }

        [TestMethod]
        public void End_StepIsLargerThanRange_MatchesThatGivenWithTimeStripped()
        {
            ZonedDateTime start, end;
            Duration step;
            ZonedDateTimeRange ZonedDateTimeRange = GenerateZonedDateTimeRangeWithStepLargerThanRange(out start, out end, out step);

            Assert.AreEqual(end, ZonedDateTimeRange.End, "End point field must match the value supplied.");
        }

        [TestMethod]
        public void Step_StepIsLargerThanRange_MatchesThatGivenWithTimeStripped()
        {
            ZonedDateTime start, end;
            Duration step;
            ZonedDateTimeRange ZonedDateTimeRange = GenerateZonedDateTimeRangeWithStepLargerThanRange(out start, out end, out step);

            Assert.AreEqual(step, ZonedDateTimeRange.Step, "Step amount field must match the value supplied.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_EndBeforeStart_ThrowsArgumentOutOfRangeException()
        {
            Duration length = RandomDuration(MinDuration, MaxDuration);
            ZonedDateTime start = RandomZonedDateTime(TestMinZonedDateTime + length, TestMaxZonedDateTime);
            ZonedDateTime end = start - length;

            ZonedDateTimeRange ZonedDateTimeRange = new ZonedDateTimeRange(start, end);
        }

        [TestMethod]
        public void Constructor_WithoutStepParam_DeltaLessThanMillisecond_StepDefaultsToOneTick()
        {
            Duration length = RandomDuration(Duration.Zero, Duration.FromMilliseconds(1));
            Trace.WriteLine(length);
            ZonedDateTime start = RandomZonedDateTime(TestMinZonedDateTime, TestMaxZonedDateTime - length);
            ZonedDateTime end = start + length;

            ZonedDateTimeRange ZonedDateTimeRange = new ZonedDateTimeRange(start, end);

            Assert.AreEqual(start, ZonedDateTimeRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, ZonedDateTimeRange.End, "End point field must match the value supplied");
            Assert.AreEqual(Duration.FromTicks(1), ZonedDateTimeRange.Step, "Step amount must default to one tick");
        }

        [TestMethod]
        public void Constructor_WithoutStepParam_DeltaLessThanSecond_StepDefaultsToOneMillisecond()
        {
            Duration length = RandomDuration(Duration.FromMilliseconds(1), Duration.FromSeconds(1));
            Trace.WriteLine(length);
            ZonedDateTime start = RandomZonedDateTime(TestMinZonedDateTime, TestMaxZonedDateTime - length);
            ZonedDateTime end = start + length;

            ZonedDateTimeRange ZonedDateTimeRange = new ZonedDateTimeRange(start, end);

            Assert.AreEqual(start, ZonedDateTimeRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, ZonedDateTimeRange.End, "End point field must match the value supplied");
            Assert.AreEqual(Duration.FromMilliseconds(1), ZonedDateTimeRange.Step, "Step amount must default to one millisecond");
        }

        [TestMethod]
        public void Constructor_WithoutStepParam_DeltaLessThanMinute_StepDefaultsToOneSecond()
        {
            Duration length = RandomDuration(Duration.FromSeconds(1), Duration.FromMinutes(1));
            Trace.WriteLine(length);
            ZonedDateTime start = RandomZonedDateTime(TestMinZonedDateTime, TestMaxZonedDateTime - length);
            ZonedDateTime end = start + length;

            ZonedDateTimeRange ZonedDateTimeRange = new ZonedDateTimeRange(start, end);

            Assert.AreEqual(start, ZonedDateTimeRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, ZonedDateTimeRange.End, "End point field must match the value supplied");
            Assert.AreEqual(Duration.FromSeconds(1), ZonedDateTimeRange.Step, "Step amount must default to one second");
        }

        [TestMethod]
        public void Constructor_WithoutStepParam_DeltaLessThanHour_StepDefaultsToOneMinute()
        {
            Duration length = RandomDuration(Duration.FromMinutes(1), Duration.FromHours(1));
            Trace.WriteLine(length);
            ZonedDateTime start = RandomZonedDateTime(TestMinZonedDateTime, TestMaxZonedDateTime - length);
            ZonedDateTime end = start + length;

            ZonedDateTimeRange ZonedDateTimeRange = new ZonedDateTimeRange(start, end);

            Assert.AreEqual(start, ZonedDateTimeRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, ZonedDateTimeRange.End, "End point field must match the value supplied");
            Assert.AreEqual(Duration.FromMinutes(1), ZonedDateTimeRange.Step, "Step amount must default to one minute");
        }

        [TestMethod]
        public void Constructor_WithoutStepParam_DeltaLessThanDay_StepDefaultsToOneHour()
        {
            Duration length = RandomDuration(Duration.FromHours(1), Duration.FromStandardDays(1));
            Trace.WriteLine(length);
            ZonedDateTime start = RandomZonedDateTime(TestMinZonedDateTime, TestMaxZonedDateTime - length);
            ZonedDateTime end = start + length;

            ZonedDateTimeRange ZonedDateTimeRange = new ZonedDateTimeRange(start, end);

            Assert.AreEqual(start, ZonedDateTimeRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, ZonedDateTimeRange.End, "End point field must match the value supplied");
            Assert.AreEqual(Duration.FromHours(1), ZonedDateTimeRange.Step, "Step amount must default to one hour");
        }

        [TestMethod]
        public void Constructor_WithoutStepParam_DeltaGreaterThanDay_StepDefaultsToOneDay()
        {
            Duration length = RandomDuration(Duration.FromStandardDays(1), MaxDuration);
            Trace.WriteLine(length);
            ZonedDateTime start = RandomZonedDateTime(TestMinZonedDateTime, TestMaxZonedDateTime - length);
            ZonedDateTime end = start + length;

            ZonedDateTimeRange ZonedDateTimeRange = new ZonedDateTimeRange(start, end);

            Assert.AreEqual(start, ZonedDateTimeRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, ZonedDateTimeRange.End, "End point field must match the value supplied");
            Assert.AreEqual(Duration.FromStandardDays(1), ZonedDateTimeRange.Step, "Step amount must default to one day");
        }

        [TestMethod]
        public void GetEnumerator_ValuesStayWithinRange()
        {
            Duration length = RandomDuration(MinDuration, MaxDuration);
            ZonedDateTime start = RandomZonedDateTime(TestMinZonedDateTime, TestMaxZonedDateTime - length);
            ZonedDateTime end = start + length;
            // note that the number of steps is limited to 100 or fewer
            Duration step = Duration.FromTicks(length.Ticks / Random.Next(4, 100));

            ZonedDateTimeRange ZonedDateTimeRange = new ZonedDateTimeRange(start, end, step);

            foreach (ZonedDateTime d in ZonedDateTimeRange)
            {
                Assert.IsTrue(d >= start, "Value from iterator must by equal or above start parameter");
                Assert.IsTrue(d <= end, "Value from iterator must be equal or below end parameter");
            }
        }

        [TestMethod]
        public void GetEnumerator_LengthDivisibleByStep_IterationCountMatchesCalculated()
        {
            Duration length = RandomDuration(MinDuration, MaxDuration);
            ZonedDateTime start = RandomZonedDateTime(TestMinZonedDateTime, TestMaxZonedDateTime - length);
            ZonedDateTime end = start + length;
            // note that the number of steps is limited to 1000 or fewer
            Duration step = Duration.FromTicks(length.Ticks / Random.Next(4, 1000));

            //ensure that step size is a factor of the length of the range
            start += Duration.FromTicks(length.Ticks % step.Ticks);

            ZonedDateTimeRange ZonedDateTimeRange = new ZonedDateTimeRange(start, end, step);

            // Range endpoint is inclusive, so must take longo account this extra iteration
            Assert.AreEqual(
                length.Ticks / step.Ticks + 1,
                ZonedDateTimeRange.Count(),
                "Iteration count should be (end-start)/step + 1 where endpoint is included");
        }

        [TestMethod]
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

            ZonedDateTimeRange ZonedDateTimeRange = new ZonedDateTimeRange(start, end, step);

            Assert.AreEqual(
                length.Ticks / step.Ticks + 1,
                ZonedDateTimeRange.Count(),
                "Iteration count should be (start-end)/step +1");
        }

        [TestMethod]
        public void GetEnumerator_Iterating_DifferenceBetweenIterationsMatchesStepSize()
        {
            Duration length = RandomDuration(MinDuration, MaxDuration);
            ZonedDateTime start = RandomZonedDateTime(TestMinZonedDateTime, TestMaxZonedDateTime - length);
            ZonedDateTime end = start + length;
            // note that the number of steps is limited to 100 or fewer
            Duration step = Duration.FromTicks(length.Ticks / Random.Next(4, 100));

            ZonedDateTimeRange ZonedDateTimeRange = new ZonedDateTimeRange(start, end, step);

            ZonedDateTime? previous = null;
            foreach (ZonedDateTime d in ZonedDateTimeRange)
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

        [TestMethod]
        public void GetEnumerator_UsingLargestPossibleParameters_IteratesSuccessfully()
        {
            // Step chosen to avoid an unfeasible number of iterations
            ZonedDateTimeRange ZonedDateTimeRange = new ZonedDateTimeRange(
                TestMinZonedDateTime,
                TestMaxZonedDateTime,
                Duration.FromTicks(MaxDuration.Ticks / 16));

            bool iterated = false;
            foreach (ZonedDateTime d in ZonedDateTimeRange)
                iterated = true;

            Assert.AreEqual(true, iterated, "When iterating across full range, at least one value should be returned");
        }

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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