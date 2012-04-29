using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebApplications.Testing.Test
{
    [TestClass]
    public class TestExtensionsTests : TestBase
    {
        [TestMethod]
        public void Stopwatch_ToString_ReturnsCorrectFormat()
        {
            Stopwatch s = new Stopwatch();
            Random random = Tester.RandomGenerator;
            s.Start();
            Thread.Sleep(random.Next(0, 1000));
            s.Stop();
            string randomString = random.GenerateRandomString();
            Assert.AreEqual(
                String.Format("Test stopwatch {0} completed in {1}ms.", randomString,
                              (s.ElapsedTicks*1000M)/Stopwatch.Frequency),
                s.ToString("Test stopwatch {0}", randomString));
        }
    }
}
