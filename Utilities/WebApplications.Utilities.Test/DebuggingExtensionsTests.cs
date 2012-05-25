#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities.Test
// File: DebuggingExtensionsTests.cs
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
using System.Text.RegularExpressions;
using System.Threading;
using WebApplications.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebApplications.Utilities.Test
{
    [TestClass]
    public class DebuggingExtensionsTests : UtilitiesTestBase
    {

        [TestMethod]
        public void StopwatchToString_NullFormatString_NameDefaultsToStopwatch()
        {
            Stopwatch testStopwatch = new Stopwatch();
            Assert.AreEqual("Stopwatch completed in 0ms.", testStopwatch.ToString(null), "Where a null format string is provided for the stopwatch name, the name should default to 'Stopwatch'.");
        }

        [Ignore] // TODO: Determine if this test is meant to be failing by checking specification
        [TestMethod]
        public void StopwatchToString_NoFormatString_NameDefaultsToStopwatch()
        {
            Stopwatch testStopwatch = new Stopwatch();
            Assert.AreEqual("Stopwatch completed in 0ms.", testStopwatch.ToString(), "Where no format string is provided for the stopwatch name, the name should default to 'Stopwatch'.");
        }

        [TestMethod]
        public void StopwatchToString_FormatStringWithNoParameters_StopwatchReferreredToUsingFormatString()
        {
            String value = Random.RandomString();
            Stopwatch testStopwatch = new Stopwatch();
            Assert.AreEqual(String.Format("{0} completed in 0ms.", value), testStopwatch.ToString(value), "Where a format string without additional parameters is provided, this is used for the stopwatch name.");
        }

        [TestMethod]
        public void StopwatchToString_FormatStringWithParameters_StopwatchReferreredToUsingFormattedString()
        {
            String value1 = Random.RandomString();
            String value2 = Random.RandomString();
            String value3 = Random.RandomString();
            Stopwatch testStopwatch = new Stopwatch();
            Assert.AreEqual(String.Format("{0} test {2} {1} completed in 0ms.", value1, value2, value3), testStopwatch.ToString("{0} test {2} {1}", value1, value2, value3),
                "Where a format string with parameters is provided, this is formatted and used for the stopwatch name.");
        }

        [TestMethod]
        public void StopwatchToString_WithTimeElapsed_ContainsTimeElapsed()
        {
            Stopwatch testStopwatch = new Stopwatch();
            testStopwatch.Start();
            Thread.Sleep(Random.Next(1,5));
            testStopwatch.Stop();
            Assert.AreEqual(testStopwatch.ElapsedMilliseconds,
                            int.Parse( Regex.Match( testStopwatch.ToString(null), "completed in ([0-9]+).?[0-9]*ms.", RegexOptions.None ).Groups[1].ToString() ),
                "The number of milliseconds elapsed should be stated in the ToString result.");
        }
    }
}
