#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities.Test
// File: LongRangeTests.cs
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Ranges;

namespace WebApplications.Utilities.Test.Ranges
{
    [TestClass]
    public class LongRangeTests : UtilitiesTestBase
    {
        private static long RandomLong( long minimum, long maximum )
        {
            return (long) (minimum + ((ulong)(maximum - minimum) * Random.NextDouble()));
        }

        [TestMethod]
        public void LongRange_ConvertingToString_IsNotBlank()
        {
            long length = RandomLong(1, long.MaxValue);
            long start = RandomLong(long.MinValue, long.MaxValue - length);
            long end = start + length;
            long step = RandomLong(1, length / 2);

            var longRange = new LongRange(start, end, step);

            Assert.AreNotEqual("", longRange.ToString(), "String representation of range must not be an empty string");
        }

        [TestMethod]
        public void LongRange_StepSmallerThanLongRange_ParametersMatchThoseGiven()
        {
            long length = RandomLong(1, long.MaxValue);
            long start = RandomLong(long.MinValue, long.MaxValue - length);
            long end = start + length;
            long step = RandomLong(1, length / 2);

            var longRange = new LongRange(start, end, step);

            Assert.AreEqual(start, longRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, longRange.End, "End point field must match the value supplied");
            Assert.AreEqual(step, longRange.Step, "Step amount field must match the value supplied");
        }

        [TestMethod]
        public void LongRange_StepIsLargerThanLongRange_ParametersMatchThoseGiven()
        {
            long length = RandomLong(1, long.MaxValue / 2);
            long start = RandomLong(long.MinValue, long.MaxValue - length);
            long end = start + length;
            long step = length + RandomLong(1, long.MaxValue / 3);

            var longRange = new LongRange(start, end, step);

            Assert.AreEqual(start, longRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, longRange.End, "End point field must match the value supplied");
            Assert.AreEqual(step, longRange.Step, "Step amount field must match the value supplied");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void LongRange_EndBeforeStart_ThrowsArgumentOutOfRangeException()
        {
            long length = RandomLong(1, long.MaxValue);
            long start = RandomLong(long.MinValue + length, long.MaxValue);
            long end = start - length;

            var longRange = new LongRange(start, end);
        }

        [TestMethod]
        public void LongRange_ConstructorWithoutStepParam_StepDefaultsToOne()
        {
            long length = RandomLong(1, long.MaxValue);
            long start = RandomLong(long.MinValue, long.MaxValue - length);
            long end = start + length;

            var longRange = new LongRange(start, end);

            Assert.AreEqual(start, longRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, longRange.End, "End point field must match the value supplied");
            Assert.AreEqual(1, longRange.Step, "Step amount must default to one");
        }

        [TestMethod]
        public void LongRange_Iterating_ValuesStayWithinRange()
        {
            long length = RandomLong(1, long.MaxValue);
            long start = RandomLong(long.MinValue, long.MaxValue - length);
            long end = start + length;
            // note that the number of steps is limited to 100 or fewer
            long step = length / RandomLong(4, Math.Max(4, Math.Min(length / 2, 100)));

            // nsure the step is at least 1 (as length of less than four causes it to round down to zero)
            if (step < 1) step = 1;

            var longRange = new LongRange(start, end, step);

            foreach (long i in longRange)
            {
                Assert.IsTrue(i >= start, "Value from iterator must by equal or above start parameter");
                Assert.IsTrue(i <= end, "Value from iterator must be equal or below end parameter");
            }
        }

        [TestMethod]
        public void LongRange_LengthDivisibleByStep_IterationCountMatchesCalculated()
        {
            long length = RandomLong(1, long.MaxValue);
            long start = RandomLong(long.MinValue, long.MaxValue - length);
            long end = start + length;
            // note that the number of steps is limited to 1000 or fewer
            long step = length / RandomLong(4, Math.Max(4, Math.Min(length / 2, 1000)));

            // In case range length is under 4, ensure the step is at least 1
            if (step < 1) step = 1;

            //ensure that step size is a factor of the length of the range
            start += length % step;

            var longRange = new LongRange(start, end, step);

            // Range endpoint is inclusive, so must take into account this extra iteration
            Assert.AreEqual(length / step + 1, longRange.Count(), "Iteration count should be (end-start)/step + 1 where endpoint is included");
        }

        [TestMethod]
        public void LongRange_LengthNotDivisibleByStep_IterationCountMatchesCalculated()
        {
            long length = RandomLong(1, long.MaxValue);
            long start = RandomLong(long.MinValue, long.MaxValue - length);
            long end = start + length;
            // note that the number of steps is limited to 1000 or fewer
            long step = length / RandomLong(4, Math.Max(4, Math.Min(length / 2, 1000)));

            // In case range length is under 4, ensure the step is at least 1
            if (step < 1) step = 1;

            //ensure that step size is not a factor of the length of the range
            if (length % step == 0)
            {
                start += RandomLong(1, step - 1);
                length = end - start;
            }

            var longRange = new LongRange(start, end, step);

            Assert.AreEqual(length / step + 1, longRange.Count(), "Iteration count should be (start-end)/step +1");
        }

        [TestMethod]
        public void LongRange_Iterating_DifferenceBetweenIterationsMatchesStepSize()
        {
            long length = RandomLong(1, long.MaxValue);
            long start = RandomLong(long.MinValue, long.MaxValue - length);
            long end = start + length;
            // note that the number of steps is limited to 100 or fewer
            long step = length / RandomLong(4, Math.Max(4, Math.Min(length / 2, 100)));

            // In case range length is under 4, ensure the step is at least 1
            if (step < 1) step = 1;

            var longRange = new LongRange(start, end, step);

            long? previous = null;
            foreach (long i in longRange)
            {
                if (previous.HasValue)
                {
                    Assert.AreEqual(i - previous, step, "Difference between iteration values should match the step value supplied");
                }
                previous = i;
            }
        }

        [TestMethod]
        public void LongRange_UsingLargestPossibleParameters_IteratesSuccessfully()
        {
            // Step chosen to avoid an unfeasible number of iterations
            var longRange = new LongRange(long.MinValue, long.MaxValue, long.MaxValue / 16);

            bool iterated = false;
            foreach (long i in longRange)
            {
                iterated = true;
            }

            Assert.AreEqual(true,iterated,"When iterating across full range, at least one value should be returned");
        }

        [TestMethod]
        public void LongRange_NumberBelowRange_BindReturnsStart()
        {
            long start = RandomLong(long.MinValue / 2, long.MaxValue / 2);
            long end = RandomLong(start, long.MaxValue / 2);
            long testValue = RandomLong(long.MinValue, start);

            Assert.AreEqual(start, (new LongRange(start, end)).Bind(testValue),
                   "Bind should return the lower bound of the range if the input is below the range");
        }

        [TestMethod]
        public void LongRange_NumberAboveRange_BindReturnsEnd()
        {
            long start = RandomLong(long.MinValue / 2, long.MaxValue / 2);
            long end = RandomLong(start, long.MaxValue / 2);
            long testValue = RandomLong(end, long.MaxValue);

            Assert.AreEqual(end, (new LongRange(start, end)).Bind(testValue),
                   "Bind should return the upper bound of the range if the input is above the range");
        }

        [TestMethod]
        public void LongRange_NumberWithinRange_BindReturnsInput()
        {
            long start = RandomLong(long.MinValue / 2, long.MaxValue / 2);
            long end = RandomLong(start, long.MaxValue / 2);
            long testValue = RandomLong(start, end);

            Assert.AreEqual(testValue, (new LongRange(start, end)).Bind(testValue),
                   "Bind should return the input if it is within the range");
        }
    }
}
