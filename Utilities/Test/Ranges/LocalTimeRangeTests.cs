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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Ranges;

namespace WebApplications.Utilities.Test.Ranges
{
    [TestClass]
    public class LocalTimeRangeTests : LocalRangeTestsBase
    {
        [NotNull]
        public static readonly Period MinPeriod = Period.FromTicks(0);

        [NotNull]
        public static readonly Period MaxPeriod = Period.FromTicks(NodaConstants.TicksPerStandardDay - 1).Normalize();

        public static readonly LocalTime MinLocalTime = new LocalTime();
        public static readonly LocalTime MaxLocalTime = new LocalTime(
            NodaConstants.HoursPerStandardDay - 1,
            NodaConstants.MinutesPerHour - 1,
            NodaConstants.SecondsPerMinute - 1,
            NodaConstants.MillisecondsPerSecond - 1,
            (int)NodaConstants.TicksPerMillisecond - 1);

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void ToString_IsNotBlank()
        {
            Period length = RandomPeriod(MinPeriod + MinPeriod, MaxPeriod, false);
            LocalTime start = RandomLocalTime(MinLocalTime, MaxLocalTime - length);
            LocalTime end = start + length;
            Period step = RandomPeriod(MinPeriod, length - MinPeriod, false);

            LocalTimeRange localRange = new LocalTimeRange(start, end, step);

            Assert.AreNotEqual(
                "",
                localRange.ToString(),
                "String representation of range must not be an empty string");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void ToString_HasCorrectFormat()
        {
            Period length = RandomPeriod(MinPeriod + MinPeriod, MaxPeriod, false);
            LocalTime start = RandomLocalTime(MinLocalTime, MaxLocalTime - length);
            LocalTime end = start + length;
            Period step = RandomPeriod(MinPeriod, length - MinPeriod, false);

            LocalTimeRange localRange = new LocalTimeRange(start, end, step);

            Assert.AreEqual($"{start} - {end}", localRange.ToString());
        }

        [NotNull]
        private static LocalTimeRange GenerateLocalTimeRangeWithStepSmallerThanRange(
            out LocalTime start,
            out LocalTime end,
            out Period step)
        {
            Period length = RandomPeriod(MinPeriod + MinPeriod, MaxPeriod, false);
            start = RandomLocalTime(MinLocalTime, MaxLocalTime - length);
            end = start + length;
            step = RandomPeriod(MinPeriod, length - MinPeriod, false);

            return new LocalTimeRange(start, end, step);
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Start_StepSmallerThanRange_MatchesThatGivenWithTimeStripped()
        {
            LocalTime start, end;
            Period step;
            LocalTimeRange localRange = GenerateLocalTimeRangeWithStepSmallerThanRange(
                out start,
                out end,
                out step);

            Assert.AreEqual(start, localRange.Start, "Starting point field must match the value supplied.");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void End_StepSmallerThanRange_MatchesThatGivenWithTimeStripped()
        {
            LocalTime start, end;
            Period step;
            LocalTimeRange localRange = GenerateLocalTimeRangeWithStepSmallerThanRange(
                out start,
                out end,
                out step);

            Assert.AreEqual(end, localRange.End, "End point field must match the value supplied.");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Step_StepSmallerThanRange_MatchesThatGivenWithTimeStripped()
        {
            LocalTime start, end;
            Period step;
            LocalTimeRange localRange = GenerateLocalTimeRangeWithStepSmallerThanRange(
                out start,
                out end,
                out step);

            Assert.AreEqual(step, localRange.Step, "Step amount field must match the value supplied.");
        }

        [NotNull]
        private static LocalTimeRange GenerateLocalTimeRangeWithStepLargerThanRange(
            out LocalTime start,
            out LocalTime end,
            out Period step)
        {
            // subtracting MinPeriod twice from max length to give space for oversized step without converting type to perform the calculation to do so
            Period length = RandomPeriod(MinPeriod, MaxPeriod - MinPeriod - MinPeriod, false);
            start = RandomLocalTime(MinLocalTime, MaxLocalTime - length);
            end = start + length;
            step = RandomPeriod(length + MinPeriod, length + MinPeriod + MinPeriod, false);

            return new LocalTimeRange(start, end, step);
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Start_StepIsLargerThanRange_MatchesThatGivenWithTimeStripped()
        {
            LocalTime start, end;
            Period step;
            LocalTimeRange localRange = GenerateLocalTimeRangeWithStepLargerThanRange(
                out start,
                out end,
                out step);

            Assert.AreEqual(start, localRange.Start, "Starting point field must match the value supplied.");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void End_StepIsLargerThanRange_MatchesThatGivenWithTimeStripped()
        {
            LocalTime start, end;
            Period step;
            LocalTimeRange localRange = GenerateLocalTimeRangeWithStepLargerThanRange(
                out start,
                out end,
                out step);

            Assert.AreEqual(end, localRange.End, "End point field must match the value supplied.");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Step_StepIsLargerThanRange_MatchesThatGivenWithTimeStripped()
        {
            LocalTime start, end;
            Period step;
            LocalTimeRange localRange = GenerateLocalTimeRangeWithStepLargerThanRange(
                out start,
                out end,
                out step);

            Assert.AreEqual(step, localRange.Step, "Step amount field must match the value supplied.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))] // ReSharper disable once InconsistentNaming
        public void Constructor_EndBeforeStart_ThrowsArgumentOutOfRangeException()
        {
            Period length = RandomPeriod(MinPeriod, MaxPeriod, false);
            LocalTime start = RandomLocalTime(MinLocalTime + length, MaxLocalTime);
            LocalTime end = start - length;

            // ReSharper disable once UnusedVariable
            LocalTimeRange localRange = new LocalTimeRange(start, end);
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Constructor_WithoutStepParam_DeltaLessThanMillisecond_StepDefaultsToOneTick()
        {
            Period length = RandomPeriod(Period.Zero, Period.FromMilliseconds(1), false);
            Trace.WriteLine(length);
            LocalTime start = RandomLocalTime(MinLocalTime, MaxLocalTime - length);
            LocalTime end = start + length;

            LocalTimeRange localRange = new LocalTimeRange(start, end);

            Assert.AreEqual(start, localRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, localRange.End, "End point field must match the value supplied");
            Assert.AreEqual(Period.FromTicks(1), localRange.Step, "Step amount must default to one tick");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Constructor_WithoutStepParam_DeltaLessThanSecond_StepDefaultsToOneMillisecond()
        {
            Period length = RandomPeriod(Period.FromMilliseconds(1), Period.FromSeconds(1), false);
            Trace.WriteLine(length);
            LocalTime start = RandomLocalTime(MinLocalTime, MaxLocalTime - length);
            LocalTime end = start + length;

            LocalTimeRange localRange = new LocalTimeRange(start, end);

            Assert.AreEqual(start, localRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, localRange.End, "End point field must match the value supplied");
            Assert.AreEqual(
                Period.FromMilliseconds(1),
                localRange.Step,
                "Step amount must default to one millisecond");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Constructor_WithoutStepParam_DeltaLessThanMinute_StepDefaultsToOneSecond()
        {
            Period length = RandomPeriod(Period.FromSeconds(1), Period.FromMinutes(1), false);
            Trace.WriteLine(length);
            LocalTime start = RandomLocalTime(MinLocalTime, MaxLocalTime - length);
            LocalTime end = start + length;

            LocalTimeRange localRange = new LocalTimeRange(start, end);

            Assert.AreEqual(start, localRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, localRange.End, "End point field must match the value supplied");
            Assert.AreEqual(Period.FromSeconds(1), localRange.Step, "Step amount must default to one second");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Constructor_WithoutStepParam_DeltaLessThanHour_StepDefaultsToOneMinute()
        {
            Period length = RandomPeriod(Period.FromMinutes(1), Period.FromHours(1), false);
            Trace.WriteLine(length);
            LocalTime start = RandomLocalTime(MinLocalTime, MaxLocalTime - length);
            LocalTime end = start + length;

            LocalTimeRange localRange = new LocalTimeRange(start, end);

            Assert.AreEqual(start, localRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, localRange.End, "End point field must match the value supplied");
            Assert.AreEqual(Period.FromMinutes(1), localRange.Step, "Step amount must default to one minute");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Constructor_WithoutStepParam_DeltaLessThanDay_StepDefaultsToOneHour()
        {
            Period length = RandomPeriod(Period.FromHours(1), Period.FromDays(1), false);
            Trace.WriteLine(length);
            LocalTime start = RandomLocalTime(MinLocalTime, MaxLocalTime - length);
            LocalTime end = start + length;

            LocalTimeRange localRange = new LocalTimeRange(start, end);

            Assert.AreEqual(start, localRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, localRange.End, "End point field must match the value supplied");
            Assert.AreEqual(Period.FromHours(1), localRange.Step, "Step amount must default to one hour");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void GetEnumerator_ValuesStayWithinRange()
        {
            Period length = RandomPeriod(MinPeriod, MaxPeriod, false);
            LocalTime start = RandomLocalTime(MinLocalTime, MaxLocalTime - length);
            LocalTime end = start + length;
            // note that the number of steps is limited to 100 or fewer
            Period step = PeriodDivideApprox(length, Random.Next(4, 100));

            LocalTimeRange localRange = new LocalTimeRange(start, end, step);

            foreach (LocalTime d in localRange)
            {
                Assert.IsTrue(d >= start, "Value from iterator must by equal or above start parameter");
                Assert.IsTrue(d <= end, "Value from iterator must be equal or below end parameter");
            }
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void GetEnumerator_LengthDivisibleByStep_IterationCountMatchesCalculated()
        {
            Period length = RandomPeriod(MinPeriod, MaxPeriod, false);
            LocalTime start = RandomLocalTime(MinLocalTime, MaxLocalTime - length);
            LocalTime end = start + length;
            // note that the number of steps is limited to 1000 or fewer
            Period step = PeriodDivideApprox(length, Random.Next(4, 1000));

            //ensure that step size is a factor of the length of the range
            start += Period.FromTicks(length.TicksFrom(start) % step.TicksFrom(start));

            LocalTimeRange localRange = new LocalTimeRange(start, end, step);

            long ticksA = start.TicksTo(end);
            long ticksB = step.TicksFrom(start);

            // Range endpoint is inclusive, so must take longo account this extra iteration
            Assert.AreEqual(
                (ticksA / ticksB) + 1,
                localRange.Count(),
                "Iteration count should be (end-start)/step + 1 where endpoint is included");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void GetEnumerator_LengthNotDivisibleByStep_IterationCountMatchesCalculated()
        {
            Period length = RandomPeriod(MinPeriod, MaxPeriod, false);
            LocalTime start = RandomLocalTime(MinLocalTime, MaxLocalTime - length);
            LocalTime end = start + length;
            // note that the number of steps is limited to 1000 or fewer
            int steps = Random.Next(4, 1000);
            Period step = PeriodDivideApprox(length, steps);

            //ensure that step size is not a factor of the length of the range
            if (length.TicksFrom(start) % step.TicksFrom(start) == 0)
            {
                start += RandomPeriod(MinPeriod, step - MinPeriod, false);
            }

            LocalTimeRange localRange = new LocalTimeRange(start, end, step);

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
            Period length = RandomPeriod(MinPeriod, MaxPeriod, false);
            LocalTime start = RandomLocalTime(MinLocalTime, MaxLocalTime - length);
            LocalTime end = start + length;
            // note that the number of steps is limited to 100 or fewer
            Period step = PeriodDivideApprox(length, Random.Next(4, 100));

            LocalTimeRange localRange = new LocalTimeRange(start, end, step);

            LocalTime? previous = null;
            foreach (LocalTime d in localRange)
            {
                if (previous.HasValue)
                {
                    Assert.AreEqual(
                        Period.Between(previous.Value, d),
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
            LocalTimeRange localRange = new LocalTimeRange(
                MinLocalTime,
                MaxLocalTime,
                PeriodDivideApprox(MaxPeriod, 16));

            bool iterated = localRange.Any();

            Assert.AreEqual(true, iterated, "When iterating across full range, at least one value should be returned");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Bind_NumberBelowRange_ReturnsStart()
        {
            LocalTime start = RandomLocalTime(
                MinLocalTime + Period.FromMinutes(10),
                MaxLocalTime - Period.FromMinutes(10));
            LocalTime end = RandomLocalTime(start, MaxLocalTime - Period.FromMinutes(10));
            LocalTime testValue = RandomLocalTime(MinLocalTime, start);

            Assert.AreEqual(
                start,
                (new LocalTimeRange(start, end)).Bind(testValue),
                "Bind should return the lower bound of the range if the input is below the range");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Bind_NumberAboveRange_ReturnsEnd()
        {
            LocalTime start = RandomLocalTime(
                MinLocalTime + Period.FromMinutes(10),
                MaxLocalTime - Period.FromMinutes(10));
            LocalTime end = RandomLocalTime(start, MaxLocalTime - Period.FromMinutes(10));
            LocalTime testValue = RandomLocalTime(end, MaxLocalTime);

            Assert.AreEqual(
                end,
                (new LocalTimeRange(start, end)).Bind(testValue),
                "Bind should return the upper bound of the range if the input is above the range");
        }

        [TestMethod] // ReSharper disable once InconsistentNaming
        public void Bind_NumberWithinRange_ReturnsInput()
        {
            LocalTime start = RandomLocalTime(
                MinLocalTime + Period.FromMinutes(10),
                MaxLocalTime - Period.FromMinutes(10));
            LocalTime end = RandomLocalTime(start, MaxLocalTime - Period.FromMinutes(10));
            LocalTime testValue = RandomLocalTime(start, end);

            Assert.AreEqual(
                testValue,
                (new LocalTimeRange(start, end)).Bind(testValue),
                "Bind should return the input if it is within the range");
        }
    }
}