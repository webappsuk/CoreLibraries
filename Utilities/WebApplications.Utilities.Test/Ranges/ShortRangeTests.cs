#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Copyright (c) 2012, Web Applications UK Ltd
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Ranges;

namespace WebApplications.Utilities.Test.Ranges
{
    [TestClass]
    public class ShortRangeTests : UtilitiesTestBase
    {
        [TestMethod]
        public void ShortRange_ConvertingToString_IsNotBlank()
        {
            short length = (short) Random.Next(1, short.MaxValue);
            short start = (short) Random.Next(short.MinValue, short.MaxValue - length);
            short end = (short) (start + length);
            short step = (short) Random.Next(1, length/2);

            ShortRange shortRange = new ShortRange(start, end, step);

            Assert.AreNotEqual(string.Empty, shortRange.ToString(),
                               "String representation of range must not be an empty string");
        }

        [TestMethod]
        public void ShortRange_StepSmallerThanShortRange_ParametersMatchThoseGiven()
        {
            short length = (short) Random.Next(1, short.MaxValue);
            short start = (short) Random.Next(short.MinValue, short.MaxValue - length);
            short end = (short) (start + length);
            short step = (short) Random.Next(1, length/2);

            ShortRange shortRange = new ShortRange(start, end, step);

            Assert.AreEqual(start, shortRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, shortRange.End, "End point field must match the value supplied");
            Assert.AreEqual(step, shortRange.Step, "Step amount field must match the value supplied");
        }

        [TestMethod]
        public void ShortRange_StepIsLargerThanShortRange_ParametersMatchThoseGiven()
        {
            short length = (short) Random.Next(1, short.MaxValue/2);
            short start = (short) Random.Next(short.MinValue, short.MaxValue - length);
            short end = (short) (start + length);
            short step = (short) (length + Random.Next(1, short.MaxValue/3));

            ShortRange shortRange = new ShortRange(start, end, step);

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

            ShortRange shortRange = new ShortRange(start, end);

            Assert.AreEqual(start, shortRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, shortRange.End, "End point field must match the value supplied");
            Assert.AreEqual(1, shortRange.Step, "Step amount must default to one");
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void ShortRange_EndBeforeStart_ThrowsArgumentOutOfRangeException()
        {
            short length = (short) Random.Next(1, short.MaxValue);
            short start = (short) Random.Next(short.MinValue + length, short.MaxValue);
            short end = (short) (start - length);

            ShortRange shortRange = new ShortRange(start, end);
        }

        [TestMethod]
        public void ShortRange_Iterating_ValuesStayWithinRange()
        {
            short length = (short) Random.Next(1, short.MaxValue);
            short start = (short) Random.Next(short.MinValue, short.MaxValue - length);
            short end = (short) (start + length);
            // note that the number of steps is limited to 100 or fewer
            short step = (short) (length/Random.Next(4, Math.Max(4, Math.Min(length/2, 100))));

            // ensure the step is at least 1 (as length of less than four causes it to round down to zero)
            if (step < 1) step = 1;

            ShortRange shortRange = new ShortRange(start, end, step);

            foreach (short i in shortRange)
            {
                Assert.IsTrue(i >= start, "Value from iterator must by equal or above start parameter");
                Assert.IsTrue(i <= end, "Value from iterator must be equal or below end parameter");
            }
        }

        [TestMethod]
        public void ShortRange_LengthDivisibleByStep_IterationCountMatchesCalculated()
        {
            short length = (short) Random.Next(1, short.MaxValue);
            short start = (short) Random.Next(short.MinValue, short.MaxValue - length);
            short end = (short) (start + length);
            // note that the number of steps is limited to 1000 or fewer
            short step = (short) (length/Random.Next(4, Math.Max(4, Math.Min(length/2, 1000))));

            // In case range length is under 4, ensure the step is at least 1
            if (step < 1) step = 1;

            //ensure that step size is a factor of the length of the range
            start += (short) (length%step);

            ShortRange shortRange = new ShortRange(start, end, step);

            // Range endpoint is inclusive, so must take into account this extra iteration
            Assert.AreEqual(length/step + 1, shortRange.Count(),
                            "Iteration count should be (end-start)/step + 1 where endpoint is included");
        }

        [TestMethod]
        public void ShortRange_LengthNotDivisibleByStep_IterationCountMatchesCalculated()
        {
            short length = (short) Random.Next(1, short.MaxValue);
            short start = (short) Random.Next(short.MinValue, short.MaxValue - length);
            short end = (short) (start + length);
            // note that the number of steps is limited to 1000 or fewer
            short step = (short) (length/Random.Next(4, Math.Max(4, Math.Min(length/2, 1000))));

            // In case range length is under 4, ensure the step is at least 2
            if (step < 2) step = 2;

            //ensure that step size is not a factor of the length of the range
            if (length%step == 0)
            {
                start += (short) Random.Next(1, step - 1);
                length = (short) (end - start);
            }

            ShortRange shortRange = new ShortRange(start, end, step);

            Assert.AreEqual(length/step + 1, shortRange.Count(), "Iteration count should be (start-end)/step +1");
        }

        [TestMethod]
        public void ShortRange_Iterating_DifferenceBetweenIterationsMatchesStepSize()
        {
            short length = (short) Random.Next(1, short.MaxValue);
            short start = (short) Random.Next(short.MinValue, short.MaxValue - length);
            short end = (short) (start + length);
            // note that the number of steps is limited to 100 or fewer
            short step = (short) (length/Random.Next(4, Math.Max(4, Math.Min(length/2, 100))));

            // In case range length is under 4, ensure the step is at least 1
            if (step < 1) step = 1;

            ShortRange shortRange = new ShortRange(start, end, step);

            short? previous = null;
            foreach (short i in shortRange)
            {
                if (previous.HasValue)
                {
                    Assert.AreEqual(i - previous, step,
                                    "Difference between iteration values should match the step value supplied");
                }
                previous = i;
            }
        }

        [TestMethod]
        public void ShortRange_UsingLargestPossibleParameters_IteratesSuccessfully()
        {
            // Step chosen to avoid an unfeasible number of iterations
            ShortRange shortRange = new ShortRange(short.MinValue, short.MaxValue, short.MaxValue/16);

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
            short start = (short) Random.Next(short.MinValue/2, short.MaxValue/2);
            short end = (short) Random.Next(start, short.MaxValue/2);
            short testValue = (short) Random.Next(short.MinValue, start);

            Assert.AreEqual(start, (new ShortRange(start, end)).Bind(testValue),
                            "Bind should return the lower bound of the range if the input is below the range");
        }

        [TestMethod]
        public void ShortRange_NumberAboveRange_BindReturnsEnd()
        {
            short start = (short) Random.Next(short.MinValue/2, short.MaxValue/2);
            short end = (short) Random.Next(start, short.MaxValue/2);
            short testValue = (short) Random.Next(end, short.MaxValue);

            Assert.AreEqual(end, (new ShortRange(start, end)).Bind(testValue),
                            "Bind should return the upper bound of the range if the input is above the range");
        }

        [TestMethod]
        public void ShortRange_NumberWithinRange_BindReturnsInput()
        {
            short start = (short) Random.Next(short.MinValue/2, short.MaxValue/2);
            short end = (short) Random.Next(start, short.MaxValue/2);
            short testValue = (short) Random.Next(start, end);

            Assert.AreEqual(testValue, (new ShortRange(start, end)).Bind(testValue),
                            "Bind should return the input if it is within the range");
        }
    }
}