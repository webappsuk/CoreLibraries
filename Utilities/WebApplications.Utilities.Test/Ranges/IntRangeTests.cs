#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities.Test
// File: IntRangeTests.cs
// 
// This software, its object code and source code and all modifications made to
// the same (the “Software”) are, and shall at all times remain, the proprietary
// information and intellectual property rights of Web Applications (UK) Limited. 
// You are only entitled to use the Software as expressly permitted by Web
// Applications (UK) Limited within the Software Customisation and
// Licence Agreement (the “Agreement”).  Any copying, modification, decompiling,
// distribution, licensing, sale, transfer or other use of the Software other than
// as expressly permitted in the Agreement is expressly forbidden.  Web
// Applications (UK) Limited reserves its rights to take action against you and
// your employer in accordance with its contractual and common law rights
// (including injunctive relief) should you breach the terms of the Agreement or
// otherwise infringe its copyright or other intellectual property rights in the
// Software.
// 
// © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
#endregion

using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Ranges;

namespace WebApplications.Utilities.Test.Ranges
{
    [TestClass]
    public class IntRangeTests : UtilitiesTestBase 
    {
        [TestMethod]
        public void IntRange_ConvertingToString_IsNotBlank()
        {
            int length = Random.Next(1, int.MaxValue);
            int start = Random.Next(int.MinValue, int.MaxValue-length);
            int end = start+length;
            int step = Random.Next(1, length / 2);

            var intRange = new IntRange(start, end, step);

            Assert.AreNotEqual(string.Empty, intRange.ToString(), "String representation of range must not be an empty string");
        }

        [TestMethod]
        public void IntRange_StepSmallerThanIntRange_ParametersMatchThoseGiven()
        {
            int length = Random.Next(1, int.MaxValue);
            int start = Random.Next(int.MinValue, int.MaxValue - length);
            int end = start + length;
            int step = Random.Next(1, length/ 2);

            var intRange = new IntRange(start, end, step);

            Assert.AreEqual(start, intRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, intRange.End, "End point field must match the value supplied");
            Assert.AreEqual(step, intRange.Step, "Step amount field must match the value supplied");
        }

        [TestMethod]
        public void IntRange_StepIsLargerThanIntRange_ParametersMatchThoseGiven()
        {
            int length = Random.Next(1, int.MaxValue/2);
            int start = Random.Next(int.MinValue, int.MaxValue - length);
            int end = start + length;
            int step = length + Random.Next(1, int.MaxValue / 3);

            var intRange = new IntRange(start, end, step);

            Assert.AreEqual(start, intRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, intRange.End, "End point field must match the value supplied");
            Assert.AreEqual(step, intRange.Step, "Step amount field must match the value supplied");
        }

        [TestMethod]
        public void IntRange_ConstructorWithoutStepParam_StepDefaultsToOne()
        {
            int length = Random.Next(1, int.MaxValue);
            int start = Random.Next(int.MinValue, int.MaxValue - length);
            int end = start + length;

            var intRange = new IntRange(start, end);

            Assert.AreEqual(start, intRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, intRange.End, "End point field must match the value supplied");
            Assert.AreEqual(1, intRange.Step, "Step amount must default to one");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IntRange_EndBeforeStart_ThrowsArgumentOutOfRangeException()
        {
            int length = Random.Next(1, int.MaxValue);
            int start = Random.Next(int.MinValue + length, int.MaxValue);
            int end = start - length;

            var intRange = new IntRange(start, end);
        }

        [TestMethod]
        public void IntRange_Iterating_ValuesStayWithinRange()
        {
            int length = Random.Next(1, int.MaxValue);
            int start = Random.Next(int.MinValue, int.MaxValue - length);
            int end = start + length;
            // note that the number of steps is limited to 100 or fewer
            int step = length / Random.Next(4, Math.Max(4,Math.Min(length / 2, 100)));

            // ensure the step is at least 1 (as length of less than four causes it to round down to zero)
            if (step < 1) step = 1;

            var intRange = new IntRange(start, end, step);

            foreach (int i in intRange)
            {
                Assert.IsTrue(i >= start, "Value from iterator must by equal or above start parameter");
                Assert.IsTrue(i <= end, "Value from iterator must be equal or below end parameter");
            }
        }

        [TestMethod]
        public void IntRange_LengthDivisibleByStep_IterationCountMatchesCalculated()
        {
            int length = Random.Next(1, int.MaxValue);
            int start = Random.Next(int.MinValue, int.MaxValue - length);
            int end = start + length;
            // note that the number of steps is limited to 1000 or fewer
            int step = length / Random.Next(4, Math.Max(4,Math.Min(length / 2, 1000)));

            // In case range length is under 4, ensure the step is at least 1
            if (step < 1) step = 1;

            //ensure that step size is a factor of the length of the range
            start += length % step;

            var intRange = new IntRange(start, end, step);

            // Range endpoint is inclusive, so must take into account this extra iteration
            Assert.AreEqual( length / step + 1, intRange.Count(), "Iteration count should be (end-start)/step + 1 where endpoint is included");
        }

        [TestMethod]
        public void IntRange_LengthNotDivisibleByStep_IterationCountMatchesCalculated()
        {
            int length = Random.Next(1, int.MaxValue);
            int start = Random.Next(int.MinValue, int.MaxValue - length);
            int end = start + length;
            // note that the number of steps is limited to 1000 or fewer
            int step = length / Random.Next(4, Math.Max(4,Math.Min(length / 2, 1000)));

            // In case range length is under 4, ensure the step is at least 1
            if (step < 1) step = 1;

            //ensure that step size is not a factor of the length of the range
            if (length % step == 0)
            {
                start += Random.Next(1, step - 1);
                length = end - start;
            }

            var intRange = new IntRange(start, end, step);

            Assert.AreEqual( length / step + 1, intRange.Count(), "Iteration count should be (start-end)/step +1");
        }

        [TestMethod]
        public void IntRange_Iterating_DifferenceBetweenIterationsMatchesStepSize()
        {
            int length = Random.Next(1, int.MaxValue);
            int start = Random.Next(int.MinValue, int.MaxValue - length);
            int end = start + length;
            // note that the number of steps is limited to 100 or fewer
            int step = length / Random.Next(4, Math.Max(4,Math.Min(length / 2, 100)));

            // In case range length is under 4, ensure the step is at least 1
            if (step < 1) step = 1;

            var intRange = new IntRange(start, end, step);

            int? previous = null;
            foreach (int i in intRange)
            {
                if (previous.HasValue)
                {
                    Assert.AreEqual(i - previous, step, "Difference between iteration values should match the step value supplied");
                }
                previous = i;
            }
        }

        [TestMethod]
        public void IntRange_UsingLargestPossibleParameters_IteratesSuccessfully()
        {
            // Step chosen to avoid an unfeasible number of iterations
            var intRange = new IntRange(int.MinValue, int.MaxValue, int.MaxValue / 16);

            bool iterated = false;
            foreach (int i in intRange)
            {
                iterated = true;
            }

            Assert.AreEqual(true, iterated, "When iterating across full range, at least one value should be returned");
        }

        [TestMethod]
        public void IntRange_UsingZeroStep_PerformsNoIterations()
        {

            var intRange = new IntRange(-Random.Next(), Random.Next(), 0);

            List<int> result = intRange.ToList();

            Assert.AreEqual(0,result.Count);
        }

        [TestMethod]
        public void IntRange_NumberBelowRange_BindReturnsStart()
        {
            int start = Random.Next(int.MinValue / 2, int.MaxValue / 2);
            int end = Random.Next(start, int.MaxValue / 2);
            int testValue = Random.Next(int.MinValue, start);

            Assert.AreEqual(start, (new IntRange(start, end)).Bind(testValue),
                   "Bind should return the lower bound of the range if the input is below the range");
        }

        [TestMethod]
        public void IntRange_NumberAboveRange_BindReturnsEnd()
        {
            int start = Random.Next(int.MinValue / 2, int.MaxValue / 2);
            int end = Random.Next(start, int.MaxValue / 2);
            int testValue = Random.Next(end, int.MaxValue);

            Assert.AreEqual(end, (new IntRange(start, end)).Bind(testValue),
                   "Bind should return the upper bound of the range if the input is above the range");
        }

        [TestMethod]
        public void IntRange_NumberWithinRange_BindReturnsInput()
        {
            int start = Random.Next(int.MinValue / 2, int.MaxValue / 2);
            int end = Random.Next(start, int.MaxValue / 2);
            int testValue = Random.Next( start, end );

            Assert.AreEqual(testValue, (new IntRange(start, end)).Bind(testValue),
                   "Bind should return the input if it is within the range");
        }
    }
}
