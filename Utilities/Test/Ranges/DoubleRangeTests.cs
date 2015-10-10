#region © Copyright Web Applications (UK) Ltd, 2015.  All rights reserved.
// Copyright (c) 2015, Web Applications UK Ltd
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
    public class DoubleRangeTests : UtilitiesTestBase
    {
        private static double RandomDouble(double minimum, double maximum)
        {
            if (maximum < minimum)
                throw new ArgumentOutOfRangeException("maximum", "Maximum value must be above minimum value");

            // ReSharper disable CompareOfFloatsByEqualityOperator
            // If value is ever exactly zero, set it to the smallest possible non-zero value because log(0)=-inf
            double lMin = (minimum == 0) ? Math.Log(double.Epsilon) : Math.Log(Math.Abs(minimum));
            double lMax = (maximum == 0) ? Math.Log(double.Epsilon) : Math.Log(Math.Abs(maximum));
            // ReSharper restore CompareOfFloatsByEqualityOperator

            bool negative = false;
            if (minimum < 0)
                if (maximum > 0)
                    // either select from min->zero or from zero->max; choose with probability which based on the relative linear (not logarithmic) size of these ranges
                    if (Random.NextDouble() < maximum / (maximum - minimum))
                        lMin = Math.Log(double.Epsilon);
                    else
                    {
                        lMax = Math.Log(double.Epsilon);
                        negative = true;
                    }
                else
                    negative = true;

            if (negative)
            {
                double temp = lMin;
                lMin = lMax;
                lMax = temp;
            }

            return (negative ? -1 : 1) * Math.Exp(lMin + ((lMax - lMin) * Random.NextDouble()));
        }

        /// <summary>
        /// Chose a length and starting value for a range at random, avoiding rounding errors caused by having the two values differ too much in order of magnitude.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Tuple"/> of form (start,length) specifying the starting point and length of the range
        /// </returns>
        private Tuple<double, double> RestrictedRandomRange()
        {
            // start by picking an order of magnitude to work on, note that the lower bound must be sqrt(epsilon) as we later multiply by another float
            double magnitude =
                Math.Exp(
                    Random.NextDouble() * (Math.Log(double.MaxValue) - Math.Log(double.Epsilon) / 2) +
                    Math.Log(double.Epsilon) / 2);
            // pick sizes for the length and starting value which can later be scaled up to the magnitude chosen
            double length = Random.NextDouble() * 2;
            double start = Random.NextDouble() * (2 - length) - 1;
            return new Tuple<double, double>(start * magnitude, length * magnitude);
        }

        [TestMethod]
        public void DoubleRange_ConvertingToString_IsNotBlank()
        {
            double length = RandomDouble(1, double.MaxValue);
            double start = RandomDouble(double.MinValue, double.MaxValue - length);
            double end = start + length;
            double step = RandomDouble(1, length / 2);

            DoubleRange doubleRange = new DoubleRange(start, end, step);

            Assert.AreNotEqual("", doubleRange.ToString(), "String representation of range must not be an empty string");
        }

        [TestMethod]
        public void DoubleRange_StepSmallerThanDoubleRange_ParametersMatchThoseGiven()
        {
            Random rand = new Random();

            double length = RandomDouble(1, double.MaxValue);
            double start = RandomDouble(double.MinValue, double.MaxValue - length);
            double end = start + length;
            double step = RandomDouble(1, length / 2);

            DoubleRange doubleRange = new DoubleRange(start, end, step);

            Assert.AreEqual(start, doubleRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, doubleRange.End, "End point field must match the value supplied");
            Assert.AreEqual(step, doubleRange.Step, "Step amount field must match the value supplied");
        }

        [TestMethod]
        public void DoubleRange_StepIsLargerThanDoubleRange_ParametersMatchThoseGiven()
        {
            Random rand = new Random();

            double length = RandomDouble(1, double.MaxValue / 2);
            double start = RandomDouble(double.MinValue, double.MaxValue - length);
            double end = start + length;
            double step = length + RandomDouble(1, double.MaxValue / 3);

            DoubleRange doubleRange = new DoubleRange(start, end, step);

            Assert.AreEqual(start, doubleRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, doubleRange.End, "End point field must match the value supplied");
            Assert.AreEqual(step, doubleRange.Step, "Step amount field must match the value supplied");
        }

        [TestMethod]
        public void DoubleRange_ConstructorWithoutStepParam_StepDefaultsToOne()
        {
            Random rand = new Random();

            double length = RandomDouble(1, double.MaxValue);
            double start = RandomDouble(double.MinValue, double.MaxValue - length);
            double end = start + length;

            DoubleRange doubleRange = new DoubleRange(start, end);

            Assert.AreEqual(start, doubleRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, doubleRange.End, "End point field must match the value supplied");
            Assert.AreEqual(1, doubleRange.Step, "Step amount must default to one");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DoubleRange_EndBeforeStart_ThrowsArgumentOutOfRangeException()
        {
            Random rand = new Random();

            double start = RandomDouble(double.MinValue / 2, double.MaxValue);
            double end = RandomDouble(double.MinValue, start);

            DoubleRange doubleRange = new DoubleRange(start, end);
        }

        [TestMethod]
        public void DoubleRange_Iterating_ValuesStayWithinRange()
        {
            Random rand = new Random();

            Tuple<double, double> rangeParams = RestrictedRandomRange();
            double start = rangeParams.Item1;
            double length = rangeParams.Item2;
            double end = start + length;
            // note that the number of steps is limited to 100 or fewer
            double step = length / rand.Next(4, 100);

            DoubleRange doubleRange = new DoubleRange(start, end, step);

            foreach (double i in doubleRange)
            {
                Assert.IsTrue(i >= start, "Value from iterator must by equal or above start parameter");
                Assert.IsTrue(i <= end, "Value from iterator must be equal or below end parameter");
            }
        }

        [TestMethod]
        public void DoubleRange_LengthNotDivisibleByStep_IterationCountMatchesCalculated()
        {
            Random rand = new Random();

            Tuple<double, double> rangeParams = RestrictedRandomRange();
            double start = rangeParams.Item1;
            double length = rangeParams.Item2;
            double end = start + length;
            // note that the number of steps is limited to 1000 or fewer
            double step = length / (rand.Next(4, 1000) + (rand.NextDouble() * 0.8 + 0.1));

            //ensure that step size is not a factor of the length of the range
            if (length % step == 0)
            {
                double offset = (rand.NextDouble() * 0.8 + 0.1) * step;
                start += offset;
                length += offset;
            }

            DoubleRange doubleRange = new DoubleRange(start, end, step);

            Assert.AreEqual(
                Math.Ceiling(length / step),
                doubleRange.Count(),
                "Iteration count should be Ceil((start-end)/step)");
        }

        [TestMethod]
        public void DoubleRange_StepGreaterThanLength_IterationCountMatchesCalculated()
        {
            Random rand = new Random();

            Tuple<double, double> rangeParams = RestrictedRandomRange();
            double start = rangeParams.Item1;
            double length = rangeParams.Item2;
            double end = start + length;
            double step = length * (2 - rand.NextDouble());

            DoubleRange doubleRange = new DoubleRange(start, end, step);

            Assert.AreEqual(
                1,
                doubleRange.Count(),
                "Iteration count should be one if the step is larger than the range");
        }

        [TestMethod]
        public void DoubleRange_Iterating_DifferenceBetweenIterationsMatchesStepSize()
        {
            Random rand = new Random();

            Tuple<double, double> rangeParams = RestrictedRandomRange();
            double start = rangeParams.Item1;
            double length = rangeParams.Item2;
            double end = start + length;
            // note that the number of steps is limited to 100 or fewer
            double step = length / rand.Next(4, 100);

            // In case range length is under 4, ensure the step is at least 1
            if (step < 1) step = 1;

            DoubleRange doubleRange = new DoubleRange(start, end, step);

            double? previous = null;
            foreach (double i in doubleRange)
            {
                if (previous.HasValue)
                    Assert.AreEqual(
                        i - previous.Value,
                        step,
                        step * 1e-6,
                        "Difference between iteration values should match the step value supplied to within one millionth");
                previous = i;
            }
        }

        [TestMethod]
        public void DoubleRange_UsingLargestPossibleParameters_IteratesSuccessfully()
        {
            // Step chosen to avoid an unfeasible number of iterations
            DoubleRange doubleRange = new DoubleRange(double.MinValue, double.MaxValue, double.MaxValue / 16);

            bool iterated = false;
            foreach (double x in doubleRange)
                iterated = true;

            Assert.AreEqual(true, iterated, "When iterating across full range, at least one value should be returned");
        }

        [TestMethod]
        public void DoubleRange_NumberBelowRange_BindReturnsStart()
        {
            double start = RandomDouble(double.MinValue / 2, double.MaxValue / 2);
            double end = RandomDouble(start, double.MaxValue / 2);
            double testValue = RandomDouble(double.MinValue, start);

            Assert.AreEqual(
                start,
                (new DoubleRange(start, end)).Bind(testValue),
                "Bind should return the lower bound of the range if the input is below the range");
        }

        [TestMethod]
        public void DoubleRange_NumberAboveRange_BindReturnsEnd()
        {
            double start = RandomDouble(double.MinValue / 2, double.MaxValue / 2);
            double end = RandomDouble(start, double.MaxValue / 2);
            double testValue = RandomDouble(end, double.MaxValue);

            Assert.AreEqual(
                end,
                (new DoubleRange(start, end)).Bind(testValue),
                "Bind should return the upper bound of the range if the input is above the range");
        }

        [TestMethod]
        public void DoubleRange_NumberWithinRange_BindReturnsInput()
        {
            double start = RandomDouble(double.MinValue / 2, double.MaxValue / 2);
            double end = RandomDouble(start, double.MaxValue / 2);
            double testValue = RandomDouble(start, end);

            Assert.AreEqual(
                testValue,
                (new DoubleRange(start, end)).Bind(testValue),
                "Bind should return the input if it is within the range");
        }
    }
}