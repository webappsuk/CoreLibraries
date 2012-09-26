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
    public class DecimalRangeTests : UtilitiesTestBase
    {
        /// <summary>
        /// Chose a random decimal using a uniform distribution on a given range.
        /// </summary>
        /// <param name="minimum">The minumum value.</param>
        /// <param name="maximum">The maximum value.</param>
        /// <returns>
        /// A <see cref="T:System.Decimal"/> between <paramref name="minimum"/> and <paramref name="maximum"/>.
        /// </returns>
        private static decimal RandomDecimal(decimal minimum, decimal maximum)
        {
            bool resized = false;
            if (minimum < decimal.MinValue/10 || maximum > decimal.MaxValue/10)
            {
                resized = true;
                // In this situation maximum-min is likely to be too large, so drop everything down one order of magnitude to do the calculations
                maximum /= 10;
                minimum /= 10;
            }
            return (resized ? 10M : 1M)*(minimum + (decimal) (Random.NextDouble()*(double) (maximum - minimum)));
        }

        /// <summary>
        /// Chose a length and starting value for a range at random, avoiding rounding errors caused by having the two values differ too much in order of magnitude.
        /// This method is currently far from ideal as it results in a poor distribution of values.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Tuple"/> of form (start,length) specifying the starting point and length of the range
        /// </returns>
        private Tuple<decimal, decimal> RestrictedRandomRange()
        {
            // start by picking an order of magnitude to work on
            decimal magnitude = 1M/(decimal) Math.Pow(10, Random.Next(0, 24));
            // pick sizes for the length and starting value which can later be scaled up to the magnitude chosen
            decimal length = RandomDecimal(0, decimal.MaxValue);
            decimal start = RandomDecimal(decimal.MinValue, decimal.MaxValue - length);
            return new Tuple<decimal, decimal>(start*magnitude, length*magnitude);
        }

        [TestMethod]
        public void DecimalRange_ConvertingToString_IsNotBlank()
        {
            decimal length = RandomDecimal(1, decimal.MaxValue);
            decimal start = RandomDecimal(decimal.MinValue, decimal.MaxValue - length);
            decimal end = start + length;
            decimal step = RandomDecimal(1, length/2);

            DecimalRange decimalRange = new DecimalRange(start, end, step);

            Assert.AreNotEqual("", decimalRange.ToString(), "String representation of range must not be an empty string");
        }

        [TestMethod]
        public void DecimalRange_StepSmallerThanDecimalRange_ParametersMatchThoseGiven()
        {
            decimal length = RandomDecimal(1, decimal.MaxValue);
            decimal start = RandomDecimal(decimal.MinValue, decimal.MaxValue - length);
            decimal end = start + length;
            decimal step = RandomDecimal(1, length/2);

            DecimalRange decimalRange = new DecimalRange(start, end, step);

            Assert.AreEqual(start, decimalRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, decimalRange.End, "End point field must match the value supplied");
            Assert.AreEqual(step, decimalRange.Step, "Step amount field must match the value supplied");
        }

        [TestMethod]
        public void DecimalRange_StepIsLargerThanDecimalRange_ParametersMatchThoseGiven()
        {
            decimal length = RandomDecimal(1, decimal.MaxValue/2);
            decimal start = RandomDecimal(decimal.MinValue, decimal.MaxValue - length);
            decimal end = start + length;
            decimal step = length + RandomDecimal(1, decimal.MaxValue/3);

            DecimalRange decimalRange = new DecimalRange(start, end, step);

            Assert.AreEqual(start, decimalRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, decimalRange.End, "End point field must match the value supplied");
            Assert.AreEqual(step, decimalRange.Step, "Step amount field must match the value supplied");
        }

        [TestMethod]
        public void DecimalRange_ConstructorWithoutStepParam_StepDefaultsToOne()
        {
            decimal length = RandomDecimal(1, decimal.MaxValue);
            decimal start = RandomDecimal(decimal.MinValue, decimal.MaxValue - length);
            decimal end = start + length;

            DecimalRange decimalRange = new DecimalRange(start, end);

            Assert.AreEqual(start, decimalRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, decimalRange.End, "End point field must match the value supplied");
            Assert.AreEqual(1, decimalRange.Step, "Step amount must default to one");
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void DecimalRange_EndBeforeStart_ThrowsArgumentOutOfRangeException()
        {
            decimal start = RandomDecimal(decimal.MinValue/2, decimal.MaxValue);
            decimal end = RandomDecimal(decimal.MinValue, start);

            DecimalRange decimalRange = new DecimalRange(start, end);
        }

        [TestMethod]
        public void DecimalRange_Iterating_ValuesStayWithinRange()
        {
            Random rand = new Random();

            Tuple<decimal, decimal> rangeParams = RestrictedRandomRange();
            decimal start = rangeParams.Item1;
            decimal length = rangeParams.Item2;
            decimal end = start + length;
            // note that the number of steps is limited to 100 or fewer
            decimal step = length/rand.Next(4, 100);

            DecimalRange decimalRange = new DecimalRange(start, end, step);

            foreach (decimal i in decimalRange)
            {
                Assert.IsTrue(i >= start, "Value from iterator must by equal or above start parameter");
                Assert.IsTrue(i <= end, "Value from iterator must be equal or below end parameter");
            }
        }

        [TestMethod]
        public void DecimalRange_LengthDivisibleByStep_IterationCountMatchesCalculated()
        {
            Random rand = new Random();

            Tuple<decimal, decimal> rangeParams = RestrictedRandomRange();
            decimal start = rangeParams.Item1;
            decimal length = rangeParams.Item2;
            decimal end = start + length;
            // pick a power of ten for the number of steps as decimal uses base 10 for the floating point
            decimal step = length/(decimal) Math.Pow(10, rand.Next(2, 5));

            //ensure that step size is a factor of the length of the range
            start += length%step;
            length += length%step;

            // Test that the attempt to create the correct scenario actually worked, as with floating point values this cannot be certain
            Assert.AreEqual(0, length%step,
                            "This test should be using a range length which is an exact multiple of the step size");

            DecimalRange decimalRange = new DecimalRange(start, end, step);

            // Range endpoint is inclusive, so must take into account this extra iteration
            Assert.AreEqual(length/step + 1, decimalRange.Count(),
                            "Iteration count should be (end-start)/step + 1 where endpoint is included");
        }

        [TestMethod]
        public void DecimalRange_LengthNotDivisibleByStep_IterationCountMatchesCalculated()
        {
            Random rand = new Random();

            Tuple<decimal, decimal> rangeParams = RestrictedRandomRange();
            decimal start = rangeParams.Item1;
            decimal length = rangeParams.Item2;
            decimal end = start + length;
            // note that the number of steps is limited to 1000 or fewer
            decimal step = length/(decimal) (rand.Next(4, 1000) + (rand.NextDouble()*0.8 + 0.1));

            //ensure that step size is not a factor of the length of the range
            if (length%step == 0)
            {
                decimal offset = (decimal) (rand.NextDouble()*0.8 + 0.1)*step;
                start += offset;
                length += offset;
            }

            DecimalRange decimalRange = new DecimalRange(start, end, step);

            Assert.AreEqual(Math.Ceiling(length/step), decimalRange.Count(),
                            "Iteration count should be Ceil((start-end)/step)");
        }

        [TestMethod]
        public void DecimalRange_StepGreaterThanLength_IterationCountMatchesCalculated()
        {
            Random rand = new Random();

            Tuple<decimal, decimal> rangeParams = RestrictedRandomRange();
            decimal start = rangeParams.Item1;
            decimal length = rangeParams.Item2;
            decimal end = start + length;
            decimal step = length*(decimal) (2 - rand.NextDouble());

            DecimalRange decimalRange = new DecimalRange(start, end, step);

            Assert.AreEqual(1, decimalRange.Count(),
                            "Iteration count should be one if the step is larger than the range");
        }

        [TestMethod]
        public void DecimalRange_Iterating_DifferenceBetweenIterationsMatchesStepSize()
        {
            Random rand = new Random();

            Tuple<decimal, decimal> rangeParams = RestrictedRandomRange();
            decimal start = rangeParams.Item1;
            decimal length = rangeParams.Item2;
            decimal end = start + length;
            // note that the number of steps is limited to 100 or fewer
            decimal step = length/rand.Next(4, 100);

            // In case range length is under 4, ensure the step is at least 1
            if (step < 1) step = 1;

            DecimalRange decimalRange = new DecimalRange(start, end, step);

            decimal? previous = null;
            foreach (decimal i in decimalRange)
            {
                if (previous.HasValue)
                {
                    Assert.AreEqual((double) (i - previous.Value), (double) step, (double) step*1e-6,
                                    "Difference between iteration values should match the step value supplied to within one millionth");
                }
                previous = i;
            }
        }

        [TestMethod]
        public void DecimalRange_UsingLargestPossibleParameters_IteratesSuccessfully()
        {
            // Step chosen to avoid an unfeasible number of iterations
            DecimalRange decimalRange = new DecimalRange(decimal.MinValue, decimal.MaxValue, decimal.MaxValue/10);

            bool iterated = false;
            foreach (decimal x in decimalRange)
            {
                iterated = true;
            }

            Assert.AreEqual(true, iterated, "When iterating across full range, at least one value should be returned");
        }

        [TestMethod]
        public void DecimalRange_NumberBelowRange_BindReturnsStart()
        {
            decimal start = RandomDecimal(decimal.MinValue/2, decimal.MaxValue/2);
            decimal end = RandomDecimal(start, decimal.MaxValue/2);
            decimal testValue = RandomDecimal(decimal.MinValue, start);

            Assert.AreEqual(start, (new DecimalRange(start, end)).Bind(testValue),
                            "Bind should return the lower bound of the range if the input is below the range");
        }

        [TestMethod]
        public void DecimalRange_NumberAboveRange_BindReturnsEnd()
        {
            decimal start = RandomDecimal(decimal.MinValue/2, decimal.MaxValue/2);
            decimal end = RandomDecimal(start, decimal.MaxValue/2);
            decimal testValue = RandomDecimal(end, decimal.MaxValue);

            Assert.AreEqual(end, (new DecimalRange(start, end)).Bind(testValue),
                            "Bind should return the upper bound of the range if the input is above the range");
        }

        [TestMethod]
        public void DecimalRange_NumberWithinRange_BindReturnsInput()
        {
            decimal start = RandomDecimal(decimal.MinValue/2, decimal.MaxValue/2);
            decimal end = RandomDecimal(start, decimal.MaxValue/2);
            decimal testValue = RandomDecimal(start, end);

            Assert.AreEqual(testValue, (new DecimalRange(start, end)).Bind(testValue),
                            "Bind should return the input if it is within the range");
        }
    }
}