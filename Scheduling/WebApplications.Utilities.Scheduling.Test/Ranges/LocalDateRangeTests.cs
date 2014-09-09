using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using WebApplications.Utilities.Scheduling.Ranges;

namespace WebApplications.Utilities.Scheduling.Test.Ranges
{
    [TestClass]
    public class LocalDateRangeTests : LocalRangeTestsBase
    {
        [NotNull]
        public static readonly Period MinPeriod = Period.FromTicks(0);

        [NotNull]
        public static readonly Period MaxPeriod = Period.FromTicks((InstantRangeTests.TestMaxInstant - InstantRangeTests.TestMinInstant).Ticks).Normalize();

        public static readonly LocalDate MinLocalDate = ZonedDateTimeRangeTests.TestMinZonedDateTime.Date;
        public static readonly LocalDate MaxLocalDate = ZonedDateTimeRangeTests.TestMaxZonedDateTime.Date;

        [NotNull]
        private Period FixStep(LocalDate start, [NotNull] Period period)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            return new LocalDateRange(start, start + period + period, period).Step;
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void ToString_IsNotBlank()
        {
            Period length = RandomPeriod(MinPeriod + MinPeriod, MaxPeriod, true, false);
            LocalDate start = RandomLocalDate(MinLocalDate, MaxLocalDate - length);
            LocalDate end = start + length;
            Period step = RandomPeriod(MinPeriod, length - MinPeriod, true, false);

            LocalDateRange localRange = new LocalDateRange(start, end, step);

            Assert.AreNotEqual(
                "",
                localRange.ToString(),
                "String representation of range must not be an empty string");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void ToString_HasCorrectFormat()
        {
            Period length = RandomPeriod(MinPeriod + MinPeriod, MaxPeriod, true, false);
            LocalDate start = RandomLocalDate(MinLocalDate, MaxLocalDate - length);
            LocalDate end = start + length;
            Period step = RandomPeriod(MinPeriod, length - MinPeriod, true, false);

            LocalDateRange localRange = new LocalDateRange(start, end, step);

            Regex formatTest =
                new Regex(@"^\d{2} \w+ -?\d{4,5} - \d{2} \w+ -?\d{4,5}$");

            Trace.WriteLine(localRange.ToString());

            Assert.IsTrue(
                formatTest.IsMatch(localRange.ToString()),
                "String representation of range should be of format dd MMMM yyyy - dd MMMM yyyy");
        }

        [NotNull]
        private static LocalDateRange GenerateLocalDateRangeWithStepSmallerThanRange(
            out LocalDate start,
            out LocalDate end,
            out Period step)
        {
            Period length = RandomPeriod(MinPeriod + MinPeriod, MaxPeriod, true, false);
            start = RandomLocalDate(MinLocalDate, MaxLocalDate - length);
            end = start + length;
            step = RandomPeriod(MinPeriod, length - MinPeriod, true, false);

            return new LocalDateRange(start, end, step);
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Start_StepSmallerThanRange_MatchesThatGivenWithTimeStripped()
        {
            LocalDate start, end;
            Period step;
            LocalDateRange localRange = GenerateLocalDateRangeWithStepSmallerThanRange(
                out start,
                out end,
                out step);

            Assert.AreEqual(start, localRange.Start, "Starting point field must match the value supplied.");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void End_StepSmallerThanRange_MatchesThatGivenWithTimeStripped()
        {
            LocalDate start, end;
            Period step;
            LocalDateRange localRange = GenerateLocalDateRangeWithStepSmallerThanRange(
                out start,
                out end,
                out step);

            Assert.AreEqual(end, localRange.End, "End point field must match the value supplied.");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Step_StepSmallerThanRange_MatchesThatGivenWithTimeStripped()
        {
            LocalDate start, end;
            Period step;
            LocalDateRange localRange = GenerateLocalDateRangeWithStepSmallerThanRange(
                out start,
                out end,
                out step);

            Assert.AreEqual(step, localRange.Step, "Step amount field must match the value supplied.");
        }

        [NotNull]
        private static LocalDateRange GenerateLocalDateRangeWithStepLargerThanRange(
            out LocalDate start,
            out LocalDate end,
            out Period step)
        {
            // subtracting MinPeriod twice from max length to give space for oversized step without converting type to perform the calculation to do so
            Period length = RandomPeriod(MinPeriod, MaxPeriod - MinPeriod - MinPeriod, true, false);
            start = RandomLocalDate(MinLocalDate, MaxLocalDate - length);
            end = start + length;
            step = RandomPeriod(length + MinPeriod, length + MinPeriod + MinPeriod, true, false);

            return new LocalDateRange(start, end, step);
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Start_StepIsLargerThanRange_MatchesThatGivenWithTimeStripped()
        {
            LocalDate start, end;
            Period step;
            LocalDateRange localRange = GenerateLocalDateRangeWithStepLargerThanRange(
                out start,
                out end,
                out step);

            Assert.AreEqual(start, localRange.Start, "Starting point field must match the value supplied.");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void End_StepIsLargerThanRange_MatchesThatGivenWithTimeStripped()
        {
            LocalDate start, end;
            Period step;
            LocalDateRange localRange = GenerateLocalDateRangeWithStepLargerThanRange(
                out start,
                out end,
                out step);

            Assert.AreEqual(end, localRange.End, "End point field must match the value supplied.");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Step_StepIsLargerThanRange_MatchesThatGivenWithTimeStripped()
        {
            LocalDate start, end;
            Period step;
            LocalDateRange localRange = GenerateLocalDateRangeWithStepLargerThanRange(
                out start,
                out end,
                out step);

            Assert.AreEqual(step, localRange.Step, "Step amount field must match the value supplied.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))] // ReSharper disable once InconsistentNaming
        public void Constructor_EndBeforeStart_ThrowsArgumentOutOfRangeException()
        {
            Period length = RandomPeriod(MinPeriod, MaxPeriod, true, false);
            LocalDate start = RandomLocalDate(MinLocalDate + length, MaxLocalDate);
            LocalDate end = start - length;

            // ReSharper disable once UnusedVariable
            LocalDateRange localRange = new LocalDateRange(start, end);
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void GetEnumerator_ValuesStayWithinRange()
        {
            Period length = RandomPeriod(MinPeriod, MaxPeriod, true, false);
            LocalDate start = RandomLocalDate(MinLocalDate, MaxLocalDate - length);
            LocalDate end = start + length;
            // note that the number of steps is limited to 100 or fewer
            Period step = PeriodDivideApprox(length, Random.Next(4, 100), false);

            LocalDateRange localRange = new LocalDateRange(start, end, step);

            foreach (LocalDate d in localRange)
            {
                Assert.IsTrue(d >= start, "Value from iterator must by equal or above start parameter");
                Assert.IsTrue(d <= end, "Value from iterator must be equal or below end parameter");
            }
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void GetEnumerator_LengthDivisibleByStep_IterationCountMatchesCalculated()
        {
            Period length = RandomPeriod(MinPeriod, MaxPeriod, true, false);
            LocalDate start = RandomLocalDate(MinLocalDate, MaxLocalDate - length);
            LocalDate end = start + length;
            // note that the number of steps is limited to 1000 or fewer
            Period step = PeriodDivideApprox(length, Random.Next(4, 1000), false);

            //ensure that step size is a factor of the length of the range
            start += Period.FromTicks(length.TicksFrom(start) % step.TicksFrom(start)).Normalize();

            LocalDateRange localRange = new LocalDateRange(start, end, step);

            long ticksA = start.TicksTo(end);
            long ticksB = step.TicksFrom(start);

            // Range endpoint is inclusive, so must take longo(?) account this extra iteration
            Assert.AreEqual(
                (ticksA / ticksB) + 1,
                localRange.Count(),
                "Iteration count should be (end-start)/step + 1 where endpoint is included");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void GetEnumerator_LengthNotDivisibleByStep_IterationCountMatchesCalculated()
        {
            Period length = RandomPeriod(MinPeriod, MaxPeriod, true, false);
            LocalDate start = RandomLocalDate(MinLocalDate, MaxLocalDate - length);
            LocalDate end = start + length;
            // note that the number of steps is limited to 1000 or fewer
            Period step = PeriodDivideApprox(length, Random.Next(4, 1000));

            //ensure that step size is not a factor of the length of the range
            if (length.TicksFrom(start) % step.TicksFrom(start) == 0)
            {
                start += RandomPeriod(MinPeriod, step - MinPeriod, true, false);
            }

            LocalDateRange localRange = new LocalDateRange(start, end, step);

            long ticksA = start.TicksTo(end);
            long ticksB = step.TicksFrom(start);

            Assert.AreEqual(
                (ticksA / ticksB) + 1,
                localRange.Count(),
                "Iteration count should be (start-end)/step +1");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void GetEnumerator_Iterating_DifferenceBetweenIterationsMatchesStepSize()
        {
            Period length = RandomPeriod(MinPeriod, MaxPeriod, true, false);
            LocalDate start = RandomLocalDate(MinLocalDate, MaxLocalDate - length);
            LocalDate end = start + length;
            // note that the number of steps is limited to 100 or fewer
            Period step = PeriodDivideApprox(length, Random.Next(4, 100), false);

            LocalDateRange localRange = new LocalDateRange(start, end, step);

            LocalDate? previous = null;
            foreach (LocalDate d in localRange)
            {
                if (previous.HasValue)
                {
                    IComparer<Period> comparer = Period.CreateComparer(previous.Value.At(new LocalTime()));

                    Assert.AreEqual(
                        0,
                        comparer.Compare(Period.Between(previous.Value, d), step),
                        "Difference between iteration values should match the step value supplied");
                }
                previous = d;
            }
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void GetEnumerator_UsingLargestPossibleParameters_IteratesSuccessfully()
        {
            // Step chosen to avoid an unfeasible number of iterations
            LocalDateRange localRange = new LocalDateRange(
                MinLocalDate,
                MaxLocalDate,
                PeriodDivideApprox(MaxPeriod, 16, false));

            bool iterated = localRange.Any();

            Assert.AreEqual(true, iterated, "When iterating across full range, at least one value should be returned");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Bind_NumberBelowRange_ReturnsStart()
        {
            LocalDate start = RandomLocalDate(
                MinLocalDate + Period.FromDays(10),
                MaxLocalDate - Period.FromDays(10));
            LocalDate end = RandomLocalDate(start, MaxLocalDate - Period.FromDays(10));
            LocalDate testValue = RandomLocalDate(MinLocalDate, start);

            Assert.AreEqual(
                start,
                (new LocalDateRange(start, end)).Bind(testValue),
                "Bind should return the lower bound of the range if the input is below the range");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Bind_NumberAboveRange_ReturnsEnd()
        {
            LocalDate start = RandomLocalDate(
                MinLocalDate + Period.FromDays(10),
                MaxLocalDate - Period.FromDays(10));
            LocalDate end = RandomLocalDate(start, MaxLocalDate - Period.FromDays(10));
            LocalDate testValue = RandomLocalDate(end, MaxLocalDate);

            Assert.AreEqual(
                end,
                (new LocalDateRange(start, end)).Bind(testValue),
                "Bind should return the upper bound of the range if the input is above the range");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Bind_NumberWithinRange_ReturnsInput()
        {
            LocalDate start = RandomLocalDate(
                MinLocalDate + Period.FromDays(10),
                MaxLocalDate - Period.FromDays(10));
            LocalDate end = RandomLocalDate(start, MaxLocalDate - Period.FromDays(10));
            LocalDate testValue = RandomLocalDate(start, end);

            Assert.AreEqual(
                testValue,
                (new LocalDateRange(start, end)).Bind(testValue),
                "Bind should return the input if it is within the range");
        }
    }
}