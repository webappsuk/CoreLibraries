using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebApplications.Utilities.Performance.Test
{
    [TestClass]
    public class TestCounters
    {
        private static readonly PerfTimer _timer = PerfCounter.GetOrAdd<PerfTimer>("Test Timer", "Test timer help.");
        private static readonly PerfMetric _counter = PerfCounter.GetOrAdd<PerfMetric>("Test Counter", "Test counter help.");

        [TestMethod]
        public void TestStatics()
        {
            Assert.IsNotNull(_timer);
            Assert.IsNotNull(_counter);
        }

        [TestMethod]
        public void TestInstance()
        {
            PerfTimer t = PerfCounter.GetOrAdd<PerfTimer>("Test Timer");
            Assert.IsNotNull(_timer);
            Assert.AreSame(t, _timer);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestInvalid()
        {
            InvalidCounter s = PerfCounter.GetOrAdd<InvalidCounter>("Test fail");
            Assert.IsNotNull(_timer);
        }

        private class InvalidCounter :PerfCounter
        {
            public InvalidCounter() : base(null, null)
            {
            }
        }
    }
}
