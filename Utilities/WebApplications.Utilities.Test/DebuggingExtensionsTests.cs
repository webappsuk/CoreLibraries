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
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;

namespace WebApplications.Utilities.Test
{
    [TestClass]
    public class DebuggingExtensionsTests : UtilitiesTestBase
    {
        [TestMethod]
        public void StopwatchToString_NullFormatString_NameDefaultsToStopwatch()
        {
            Stopwatch testStopwatch = new Stopwatch();
            Assert.AreEqual("Stopwatch completed in 0ms.", testStopwatch.ToString(null),
                            "Where a null format string is provided for the stopwatch name, the name should default to 'Stopwatch'.");
        }

        [TestMethod]
        public void StopwatchToString_FormatStringWithNoParameters_StopwatchReferreredToUsingFormatString()
        {
            String value = Random.RandomString();
            Stopwatch testStopwatch = new Stopwatch();
            Assert.AreEqual(String.Format("{0} completed in 0ms.", value), testStopwatch.ToString(value),
                            "Where a format string without additional parameters is provided, this is used for the stopwatch name.");
        }

        [TestMethod]
        public void StopwatchToString_FormatStringWithParameters_StopwatchReferreredToUsingFormattedString()
        {
            String value1 = Random.RandomString();
            String value2 = Random.RandomString();
            String value3 = Random.RandomString();
            Stopwatch testStopwatch = new Stopwatch();
            Assert.AreEqual(String.Format("{0} test {2} {1} completed in 0ms.", value1, value2, value3),
                            testStopwatch.ToString("{0} test {2} {1}", value1, value2, value3),
                            "Where a format string with parameters is provided, this is formatted and used for the stopwatch name.");
        }

        [TestMethod]
        public void StopwatchToString_WithTimeElapsed_ContainsTimeElapsed()
        {
            Stopwatch testStopwatch = new Stopwatch();
            testStopwatch.Start();
            Thread.Sleep(Random.Next(1, 5));
            testStopwatch.Stop();
            Assert.AreEqual(testStopwatch.ElapsedMilliseconds,
                            int.Parse(
                                Regex.Match(testStopwatch.ToString(null), "completed in ([0-9]+).?[0-9]*ms.", RegexOptions.None)
                                .Groups[1]
                                .ToString()),
                            "The number of milliseconds elapsed should be stated in the ToString result.");
        }
    }
}