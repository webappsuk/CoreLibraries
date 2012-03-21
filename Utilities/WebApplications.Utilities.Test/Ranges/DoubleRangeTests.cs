#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities.Test
// File: DoubleRangeTests.cs
// 
// This software, its object code and source code and all modifications made to
// the same (the “Software”) are, and shall at all times remain, the proprietary
// information and doubleellectual property rights of Web Applications (UK) Limited. 
// You are only entitled to use the Software as expressly permitted by Web
// Applications (UK) Limited within the Software Customisation and
// Licence Agreement (the “Agreement”).  Any copying, modification, decompiling,
// distribution, licensing, sale, transfer or other use of the Software other than
// as expressly permitted in the Agreement is expressly forbidden.  Web
// Applications (UK) Limited reserves its rights to take action against you and
// your employer in accordance with its contractual and common law rights
// (including injunctive relief) should you breach the terms of the Agreement or
// otherwise infringe its copyright or other doubleellectual property rights in the
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
    public class DoubleRangeTests : TestBase
    {

        private static double RandomDouble(double minimum, double maximum)
        {
            if (maximum < minimum)
                throw new ArgumentOutOfRangeException("maximum","Maximum value must be above minimum value");

            // ReSharper disable CompareOfFloatsByEqualityOperator
            // If value is ever exactly zero, set it to the smallest possible non-zero value because log(0)=-inf
            double lMin = (minimum == 0) ? Math.Log(double.Epsilon) : Math.Log(Math.Abs(minimum));
            double lMax = (maximum == 0) ? Math.Log(double.Epsilon) : Math.Log(Math.Abs(maximum));
            // ReSharper restore CompareOfFloatsByEqualityOperator

            bool negative = false;
            if (minimum < 0)
            {
                if (maximum > 0)
                {
                    // either select from min->zero or from zero->max; choose with probability which based on the relative linear (not logarithmic) size of these ranges
                    if (Random.NextDouble() < maximum/(maximum - minimum))
                    {
                        lMin = Math.Log(double.Epsilon);
                    }
                    else
                    {
                        lMax = Math.Log(double.Epsilon);
                        negative = true;
                    }
                }
                else
                {
                    negative = true;
                }
            }

            if( negative )
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
        private Tuple<double,double> RestrictedRandomRange()
        {
            // start by picking an order of magnitude to work on, note that the lower bound must be sqrt(epsilon) as we later multiply by another float
            double magnitude = Math.Exp( Random.NextDouble()*(Math.Log(double.MaxValue)-Math.Log(double.Epsilon)/2) + Math.Log(double.Epsilon)/2 );
            // pick sizes for the length and starting value which can later be scaled up to the magnitude chosen
            double length = Random.NextDouble() * 2;
            double start = Random.NextDouble() * (2-length) - 1;
            return new Tuple<double, double>(start*magnitude, length*magnitude);
        }

        [TestMethod]
        public void DoubleRange_ConvertingToString_IsNotBlank()
        {
            double length = RandomDouble(1, double.MaxValue);
            double start = RandomDouble(double.MinValue, double.MaxValue - length);
            double end = start + length;
            double step = RandomDouble(1, length / 2);

            var doubleRange = new DoubleRange(start, end, step);

            Assert.AreNotEqual("", doubleRange.ToString(), "String representation of range must not be an empty string");
        }

        [TestMethod]
        public void DoubleRange_StepSmallerThanDoubleRange_ParametersMatchThoseGiven()
        {
            var rand = new Random();

            double length = RandomDouble(1, double.MaxValue);
            double start = RandomDouble(double.MinValue, double.MaxValue - length);
            double end = start + length;
            double step = RandomDouble(1, length / 2);

            var doubleRange = new DoubleRange(start, end, step);

            Assert.AreEqual(start, doubleRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, doubleRange.End, "End point field must match the value supplied");
            Assert.AreEqual(step, doubleRange.Step, "Step amount field must match the value supplied");
        }

        [TestMethod]
        public void DoubleRange_StepIsLargerThanDoubleRange_ParametersMatchThoseGiven()
        {
            var rand = new Random();

            double length = RandomDouble(1, double.MaxValue / 2);
            double start = RandomDouble(double.MinValue, double.MaxValue - length);
            double end = start + length;
            double step = length + RandomDouble(1, double.MaxValue / 3);

            var doubleRange = new DoubleRange(start, end, step);

            Assert.AreEqual(start, doubleRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, doubleRange.End, "End point field must match the value supplied");
            Assert.AreEqual(step, doubleRange.Step, "Step amount field must match the value supplied");
        }

        [TestMethod]
        public void DoubleRange_ConstructorWithoutStepParam_StepDefaultsToOne()
        {
            var rand = new Random();

            double length = RandomDouble(1, double.MaxValue);
            double start = RandomDouble(double.MinValue, double.MaxValue - length);
            double end = start + length;

            var doubleRange = new DoubleRange(start, end);

            Assert.AreEqual(start, doubleRange.Start, "Starting point field must match the value supplied");
            Assert.AreEqual(end, doubleRange.End, "End point field must match the value supplied");
            Assert.AreEqual(1, doubleRange.Step, "Step amount must default to one");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DoubleRange_EndBeforeStart_ThrowsArgumentOutOfRangeException()
        {
            var rand = new Random();
            
            double start = RandomDouble(double.MinValue/2, double.MaxValue);
            double end = RandomDouble(double.MinValue, start);

            var doubleRange = new DoubleRange(start, end);
        }

        [TestMethod]
        public void DoubleRange_Iterating_ValuesStayWithinRange()
        {
            var rand = new Random();

            Tuple<double,double> rangeParams = RestrictedRandomRange();
            double start = rangeParams.Item1;
            double length = rangeParams.Item2;
            double end = start + length;
            // note that the number of steps is limited to 100 or fewer
            double step = length / rand.Next(4, 100);

            var doubleRange = new DoubleRange(start, end, step);

            foreach (double i in doubleRange)
            {
                Assert.IsTrue(i >= start, "Value from iterator must by equal or above start parameter");
                Assert.IsTrue(i <= end, "Value from iterator must be equal or below end parameter");
            }
        }

        [TestMethod]
        public void DoubleRange_LengthDivisibleByStep_IterationCountMatchesCalculated()
        {
            var rand = new Random();

            Tuple<double, double> rangeParams = RestrictedRandomRange();
            double start = rangeParams.Item1;
            double length = rangeParams.Item2;
            double end = start + length;
            // pick a power of two for the number of steps as floating points can store binary fractions more accurately
            double step = length / Math.Pow( 2, rand.Next(2, 5));

            //ensure that step size is a factor of the length of the range
            start += length % step;
            length += length % step;

            // Test that the attempt to create the correct scenario actually worked, as with floating point values this cannot be certain
            Assert.AreEqual(0, length%step,
                   "This test should be using a range length which is an exact multiple of the step size");

            var doubleRange = new DoubleRange(start, end, step);

            // Range endpoint is inclusive, so must take into account this extra iteration
            Assert.AreEqual(length / step + 1, doubleRange.Count(), "Iteration count should be (end-start)/step + 1 where endpoint is included");
        }

        [TestMethod]
        public void DoubleRange_LengthNotDivisibleByStep_IterationCountMatchesCalculated()
        {
            var rand = new Random();

            Tuple<double, double> rangeParams = RestrictedRandomRange();
            double start = rangeParams.Item1;
            double length = rangeParams.Item2;
            double end = start + length;
            // note that the number of steps is limited to 1000 or fewer
            double step = length / (rand.Next(4, 1000)+(rand.NextDouble()*0.8+0.1));

            //ensure that step size is not a factor of the length of the range
            if (length % step == 0)
            {
                double offset = (rand.NextDouble()*0.8+0.1)*step;
                start += offset;
                length += offset;
            }

            var doubleRange = new DoubleRange(start, end, step);

            Assert.AreEqual(Math.Ceiling(length / step), doubleRange.Count(), "Iteration count should be Ceil((start-end)/step)");
        }

        [TestMethod]
        public void DoubleRange_StepGreaterThanLength_IterationCountMatchesCalculated()
        {
            var rand = new Random();

            Tuple<double, double> rangeParams = RestrictedRandomRange();
            double start = rangeParams.Item1;
            double length = rangeParams.Item2;
            double end = start + length;
            double step = length * (2-rand.NextDouble());

            var doubleRange = new DoubleRange(start, end, step);

            Assert.AreEqual(1, doubleRange.Count(), "Iteration count should be one if the step is larger than the range");
        }

        [TestMethod]
        public void DoubleRange_Iterating_DifferenceBetweenIterationsMatchesStepSize()
        {
            var rand = new Random();

            Tuple<double, double> rangeParams = RestrictedRandomRange();
            double start = rangeParams.Item1;
            double length = rangeParams.Item2;
            double end = start + length;
            // note that the number of steps is limited to 100 or fewer
            double step = length / rand.Next(4, 100);

            // In case range length is under 4, ensure the step is at least 1
            if (step < 1) step = 1;

            var doubleRange = new DoubleRange(start, end, step);

            double? previous = null;
            foreach (double i in doubleRange)
            {
                if (previous.HasValue)
                {
                    Assert.AreEqual(i - previous, step, "Difference between iteration values should match the step value supplied");
                }
                previous = i;
            }
        }

        [TestMethod]
        public void DoubleRange_UsingLargestPossibleParameters_IteratesSuccessfully()
        {
            // Step chosen to avoid an unfeasible number of iterations
            var doubleRange = new DoubleRange(double.MinValue, double.MaxValue, double.MaxValue / 16);

            bool iterated = false;
            foreach (double x in doubleRange)
            {
                iterated = true;
            }

            Assert.AreEqual(true, iterated, "When iterating across full range, at least one value should be returned");
        }

        [TestMethod]
        public void DoubleRange_NumberBelowRange_BindReturnsStart()
        {
            double start = RandomDouble(double.MinValue / 2, double.MaxValue / 2);
            double end = RandomDouble(start, double.MaxValue / 2);
            double testValue = RandomDouble(double.MinValue, start);

            Assert.AreEqual(start, (new DoubleRange(start, end)).Bind(testValue),
                   "Bind should return the lower bound of the range if the input is below the range");
        }

        [TestMethod]
        public void DoubleRange_NumberAboveRange_BindReturnsEnd()
        {
            double start = RandomDouble(double.MinValue / 2, double.MaxValue / 2);
            double end = RandomDouble(start, double.MaxValue / 2);
            double testValue = RandomDouble(end, double.MaxValue);

            Assert.AreEqual(end, (new DoubleRange(start, end)).Bind(testValue),
                   "Bind should return the upper bound of the range if the input is above the range");
        }

        [TestMethod]
        public void DoubleRange_NumberWithinRange_BindReturnsInput()
        {
            double start = RandomDouble(double.MinValue / 2, double.MaxValue / 2);
            double end = RandomDouble(start, double.MaxValue / 2);
            double testValue = RandomDouble(start, end);

            Assert.AreEqual(testValue, (new DoubleRange(start, end)).Bind(testValue),
                   "Bind should return the input if it is within the range");
        }
    }
}
