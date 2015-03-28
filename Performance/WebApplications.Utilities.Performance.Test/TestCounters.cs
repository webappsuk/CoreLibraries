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

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebApplications.Utilities.Performance.Test
{
    [TestClass]
    public class TestCounters
    {
        private static readonly PerfTimer _timer = PerfCategory.GetOrAdd<PerfTimer>("Test Timer", "Test timer help.");

        private static readonly PerfCounter _counter = PerfCategory.GetOrAdd<PerfCounter>(
            "Test Counter",
            "Test counter help.");

        [TestMethod]
        public void TestStatics()
        {
            Assert.IsNotNull(_timer);
            Assert.IsNotNull(_counter);
        }

        [TestMethod]
        public void TestInstance()
        {
            Assert.IsTrue(PerfCategory.Exists("Test Timer"));
            Assert.IsTrue(PerfCategory.Exists<PerfTimer>("Test Timer"));
            PerfTimer t = PerfCategory.GetOrAdd<PerfTimer>("Test Timer");
            Assert.IsNotNull(_timer);
            Assert.AreSame(t, _timer);
            Trace.WriteLine(t.ToString());
            using (t.Region())
                Thread.Sleep(50);
            Trace.WriteLine(t.ToString());
            Trace.WriteLine(t.ToString());

            Trace.WriteLine(t.ToString("{short}"));
            Trace.WriteLine(t.ToString(PerfCategory.ShortFormat));
            Trace.WriteLine(t.Rate);
        }

        public void TestAccessors()
        {
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void TestInvalid()
        {
            InvalidCounter s = PerfCategory.GetOrAdd<InvalidCounter>("Test fail");
            Assert.IsNotNull(_timer);
        }

        private class InvalidCounter : PerfCategory
        {
            public InvalidCounter()
                : base(null, null)
            {
            }
        }
    }
}