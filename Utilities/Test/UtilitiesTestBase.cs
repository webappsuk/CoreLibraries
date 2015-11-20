#region © Copyright Web Applications (UK) Ltd, 2014.  All rights reserved.
// Copyright (c) 2014, Web Applications UK Ltd
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using WebApplications.Testing;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Difference;

namespace WebApplications.Utilities.Test
{
    /// <summary>
    /// Utilities Test Base.
    /// </summary>
    [DeploymentItem("Resources\\", "Resources")]
    public class UtilitiesTestBase : TestBase
    {
        protected static readonly Random Random = Tester.RandomGenerator;
        
        [NotNull]
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            Trace.WriteLine($"Begin test: {TestContext.TestName}");
            GC.Collect();
            GC.WaitForPendingFinalizers();
            TestContext.Properties["StartTicks"] = Stopwatch.GetTimestamp();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            long et = Stopwatch.GetTimestamp();

            object tso = TestContext.Properties["StartTicks"];
            string time;
            if (!(tso is long))
                time = "no timing information found!";
            else
            {
                long ms = (et - (long)tso) / TimeSpan.TicksPerMillisecond;
                time = $"time taken {ms}ms.";
            }
            Trace.WriteLine($"Ending test: {TestContext.TestName}, {time}");
        }

        public void AssertString(string expected, string actual, string message = null, TextTokenStrategy tokenStrategy = TextTokenStrategy.Character, TextOptions options = TextOptions.None, StringComparer comparer = null)
        {
            StringBuilder builder = new StringBuilder();
            if (expected == null)
            {
                if (actual != null)
                    builder.AppendLine($"Expected null, got <{actual.Escape()}>");
            }
            else if (actual == null)
                builder.AppendLine($"Expected <{expected.Escape()}>, got null");
            else
            {
                StringDifferences differences = expected.Diff(
                    actual,
                    tokenStrategy,
                    options,
                    comparer ?? StringComparer.CurrentCultureIgnoreCase);
                if (!differences.AreEqual)
                {
                    builder.AppendLine($"Expected <{expected}>, got <{actual}>");

                    foreach (StringChunk difference in differences.Where(c => !c.AreEqual))
                    {
                        if (difference.A != null)
                        builder.AppendLine(
                            difference.B != null
                                ? $"Expected '{difference.A.Escape()}' at Offset '{difference.OffsetA}' - got '{difference.B.Escape()}' at Offset '{difference.OffsetB}'"
                                : $"The string '{difference.A.Escape()}' at Offset '{difference.OffsetA}' was expected.");
                        else
                            builder.AppendLine($"The string '{difference.B.Escape()}' at Offset '{difference.OffsetB}' was unexpected.");
                    }
                }
            }

            if (builder.Length < 1) return;

            if (message != null)
                builder.AppendLine(message);

            Assert.Fail(builder.ToString());
        }
    }
}