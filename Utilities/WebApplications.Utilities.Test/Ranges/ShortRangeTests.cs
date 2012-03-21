#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities.Test
// File: ShortRangeTests.cs
// 
// This software, its object code and source code and all modifications made to
// the same (the “Software”) are, and shall at all times remain, the proprietary
// information and shortellectual property rights of Web Applications (UK) Limited. 
// You are only entitled to use the Software as expressly permitted by Web
// Applications (UK) Limited within the Software Customisation and
// Licence Agreement (the “Agreement”).  Any copying, modification, decompiling,
// distribution, licensing, sale, transfer or other use of the Software other than
// as expressly permitted in the Agreement is expressly forbidden.  Web
// Applications (UK) Limited reserves its rights to take action against you and
// your employer in accordance with its contractual and common law rights
// (including injunctive relief) should you breach the terms of the Agreement or
// otherwise infringe its copyright or other shortellectual property rights in the
// Software.
// 
// © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
#endregion

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;
using WebApplications.Utilities.Ranges;

namespace WebApplications.Utilities.Test.Ranges
{
    [TestClass]
    public class ShortRangeTests : TestBase
    {

        [TestMethod]
        public void ShortRange_ConvertingToString_IsNotBlank()
        {
            short length = (short) Random.Next(1, short.MaxValue);
            short start = (short) Random.Next(short.MinValue, short.MaxValue - length);
            short end = (short) (start + length);
            short step = (short) Random.Next(1, length / 2);

            var shortRange = new ShortRange(start, end, step);

            Assert.AreNotEqual("", shortRange.ToString(), "String representation of range must not be an empty string");
        }

        [TestMethod]
        public void ShortRange_StepSmallerThanShortRange_ParametersMatchThoseGiven()
        {
            short length = (short) Random.Next(1, short.MaxValue);
            short start = (short) Random.Next(short.MinValue, short.MaxValue - length);
            short end = (short) (start + length);
            short step = (short) Random.Next(1, length / 2);

            var shortRange = new ShortRange(start, end, step);

            Assert.AreEqual(start, shortRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, shortRange.End, "End point field must match the value supplied");
            Assert.AreEqual(step, shortRange.Step, "Step amount field must match the value supplied");
        }

        [TestMethod]
        public void ShortRange_StepIsLargerThanShortRange_ParametersMatchThoseGiven()
        {
            short length = (short) Random.Next(1, short.MaxValue / 2);
            short start = (short) Random.Next(short.MinValue, short.MaxValue - length);
            short end = (short) (start + length);
            short step = (short) (length + Random.Next(1, short.MaxValue / 3));

            var shortRange = new ShortRange(start, end, step);

            Assert.AreEqual(start, shortRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, shortRange.End, "End point field must match the value supplied");
            Assert.AreEqual(step, shortRange.Step, "Step amount field must match the value supplied");
        }

        [TestMethod]
        public void ShortRange_ConstructorWithoutStepParam_StepDefaultsToOne()
        {
            short length = (short) Random.Next(1, short.MaxValue);
            short start = (short) Random.Next(short.MinValue, short.MaxValue - length);
            short end = (short) (start + length);

            var shortRange = new ShortRange(start, end);

            Assert.AreEqual(start, shortRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, shortRange.End, "End point field must match the value supplied");
            Assert.AreEqual(1, shortRange.Step, "Step amount must default to one");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ShortRange_EndBeforeStart_ThrowsArgumentOutOfRangeException()
        {
            short length = (short) Random.Next(1, short.MaxValue);
            short start = (short)Random.Next(short.MinValue + length, short.MaxValue);
            short end = (short) (start - length);

            var shortRange = new ShortRange(start, end);
        }

        [TestMethod]
        public void ShortRange_Iterating_ValuesStayWithinRange()
        {
            short length = (short) Random.Next(1, short.MaxValue);
            short start = (short)Random.Next(short.MinValue, short.MaxValue - length);
            short end = (short) (start + length);
            // note that the number of steps is limited to 100 or fewer
            short step = (short) (length / Random.Next(4, Math.Max(4, Math.Min(length / 2, 100))));

            // ensure the step is at least 1 (as length of less than four causes it to round down to zero)
            if (step < 1) step = 1;

            var shortRange = new ShortRange(start, end, step);

            foreach (short i in shortRange)
            {
                Assert.IsTrue(i >= start, "Value from iterator must by equal or above start parameter");
                Assert.IsTrue(i <= end, "Value from iterator must be equal or below end parameter");
            }
        }

        [TestMethod]
        public void ShortRange_LengthDivisibleByStep_IterationCountMatchesCalculated()
        {
            short length = (short)Random.Next(1, short.MaxValue);
            short start = (short)Random.Next(short.MinValue, short.MaxValue - length);
            short end = (short)(start + length);
            // note that the number of steps is limited to 1000 or fewer
            short step = (short)(length / Random.Next(4, Math.Max(4, Math.Min(length / 2, 1000))));

            // In case range length is under 4, ensure the step is at least 1
            if (step < 1) step = 1;

            //ensure that step size is a factor of the length of the range
            start += (short) (length % step);

            var shortRange = new ShortRange(start, end, step);

            // Range endpoint is inclusive, so must take into account this extra iteration
            Assert.AreEqual(length / step + 1, shortRange.Count(), "Iteration count should be (end-start)/step + 1 where endpoint is included");
        }

        [TestMethod]
        public void ShortRange_LengthNotDivisibleByStep_IterationCountMatchesCalculated()
        {
            short length = (short)Random.Next(1, short.MaxValue);
            short start = (short)Random.Next(short.MinValue, short.MaxValue - length);
            short end = (short)(start + length);
            // note that the number of steps is limited to 1000 or fewer
            short step = (short)(length / Random.Next(4, Math.Max(4, Math.Min(length / 2, 1000))));

            // In case range length is under 4, ensure the step is at least 1
            if (step < 1) step = 1;

            //ensure that step size is not a factor of the length of the range
            if (length % step == 0)
            {
                start += (short)Random.Next(1, step - 1);
                length = (short) (end - start);
            }

            var shortRange = new ShortRange(start, end, step);

            Assert.AreEqual(length / step + 1, shortRange.Count(), "Iteration count should be (start-end)/step +1");
        }

        [TestMethod]
        public void ShortRange_Iterating_DifferenceBetweenIterationsMatchesStepSize()
        {
            short length = (short)Random.Next(1, short.MaxValue);
            short start = (short)Random.Next(short.MinValue, short.MaxValue - length);
            short end = (short)(start + length);
            // note that the number of steps is limited to 100 or fewer
            short step = (short)(length / Random.Next(4, Math.Max(4, Math.Min(length / 2, 100))));

            // In case range length is under 4, ensure the step is at least 1
            if (step < 1) step = 1;

            var shortRange = new ShortRange(start, end, step);

            short? previous = null;
            foreach (short i in shortRange)
            {
                if (previous.HasValue)
                {
                    Assert.AreEqual(i - previous, step, "Difference between iteration values should match the step value supplied");
                }
                previous = i;
            }
        }

        [TestMethod]
        public void ShortRange_UsingLargestPossibleParameters_IteratesSuccessfully()
        {
            // Step chosen to avoid an unfeasible number of iterations
            var shortRange = new ShortRange(short.MinValue, short.MaxValue, short.MaxValue / 16);

            bool iterated = false;
            foreach (short i in shortRange)
            {
                iterated = true;
            }

            Assert.AreEqual(true, iterated, "When iterating across full range, at least one value should be returned");
        }

        [TestMethod]
        public void ShortRange_NumberBelowRange_BindReturnsStart()
        {
            short start = (short) Random.Next(short.MinValue / 2, short.MaxValue / 2);
            short end = (short) Random.Next(start, short.MaxValue / 2);
            short testValue = (short) Random.Next(short.MinValue, start);

            Assert.AreEqual(start, (new ShortRange(start, end)).Bind(testValue),
                   "Bind should return the lower bound of the range if the input is below the range");
        }

        [TestMethod]
        public void ShortRange_NumberAboveRange_BindReturnsEnd()
        {
            short start = (short) Random.Next(short.MinValue / 2, short.MaxValue / 2);
            short end = (short) Random.Next(start, short.MaxValue / 2);
            short testValue = (short) Random.Next(end, short.MaxValue);

            Assert.AreEqual(end, (new ShortRange(start, end)).Bind(testValue),
                   "Bind should return the upper bound of the range if the input is above the range");
        }

        [TestMethod]
        public void ShortRange_NumberWithinRange_BindReturnsInput()
        {
            short start = (short) Random.Next(short.MinValue / 2, short.MaxValue / 2);
            short end = (short) Random.Next(start, short.MaxValue / 2);
            short testValue = (short) Random.Next(start, end);

            Assert.AreEqual(testValue, (new ShortRange(start, end)).Bind(testValue),
                   "Bind should return the input if it is within the range");
        }
    }
}
