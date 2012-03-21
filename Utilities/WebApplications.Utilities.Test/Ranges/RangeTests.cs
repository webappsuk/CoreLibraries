#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities.Test
// File: RangeTests.cs
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
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;
using WebApplications.Utilities.Ranges;

namespace WebApplications.Utilities.Test.Ranges
{
    [TestClass]
    public class RangeTests : TestBase
    {

        [TestMethod]
        public void Range_NumberBelowRange_BindReturnsStart()
        {

            int start = Random.Next(int.MinValue / 2, int.MaxValue / 2);
            int end = Random.Next(start, int.MaxValue / 2);
            int testValue = Random.Next(int.MinValue, start);

            Assert.AreEqual(start, (new Range<int>(start, end)).Bind(testValue),
                   "Bind should return the lower bound of the range if the input is below the range");
        }

        [TestMethod]
        public void Range_NumberAboveRange_BindReturnsEnd()
        {

            int start = Random.Next(int.MinValue / 2, int.MaxValue / 2);
            int end = Random.Next(start, int.MaxValue / 2);
            int testValue = Random.Next(end, int.MaxValue);

            Assert.AreEqual(end, (new Range<int>(start, end)).Bind(testValue),
                   "Bind should return the upper bound of the range if the input is above the range");
        }

        [TestMethod]
        public void Range_NumberWithinRange_BindReturnsInput()
        {

            int start = Random.Next(int.MinValue / 2, int.MaxValue / 2);
            int end = Random.Next(start, int.MaxValue / 2);
            int testValue = Random.Next(start, end);

            Assert.AreEqual(testValue, (new Range<int>(start, end)).Bind(testValue),
                   "Bind should return the input if it is within the range");
        }

        [TestMethod]
        [ExpectedException(typeof(TypeInitializationException))]
        public void Range_IncompatibleTypes_ThrowsTypeInitializationException()
        {

            int start = Random.Next(int.MinValue / 2, int.MaxValue / 2);
            int end = Random.Next(start, int.MaxValue / 2);

            new Range<float, int>(start, end,1);
        }
    }
}
