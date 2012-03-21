using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Logging.Operations;
using WebApplications.Utilities.Logging.Performance;

namespace WebApplications.Utilities.Test
{
    [TestClass]
    public class TestPerformanceCounter
    {
        private const string OperationCategoryName = "Testing Performance Counter";

        private static void RemoveCounter(string categoryName)
        {
            if (PerformanceCounterCategory.Exists(categoryName))
            {
                PerformanceCounterCategory.Delete(categoryName);
            }
        }

        [TestMethod]
        public void RemoveTestingCounters()
        {
            RemoveCounter(string.Format("WebApplications.Utilities.Test: {0}", OperationCategoryName));
        }

        [PerformanceCounter(OperationCategoryName)]
        private static double SlowMethod(int iterations)
        {
            Random random = new Random();
            double total = 0.00;
            for (int iteration = 0; iteration < iterations; iteration++)
            {
                for (int innerIteration = 0; innerIteration < random.Next(10000, 1000000); innerIteration++)
                {
                    total += innerIteration;
                }
            }

            return total;
        }

        [TestMethod]
        public void TestMethod1()
        {
            for (int iteration = 0; iteration < 100; iteration++)
            {
                Trace.WriteLine(SlowMethod(iteration));
            }
        }

        [TestMethod]
        [PerformanceCounter(OperationCategoryName)]
        public void TestMethodRemoveDuringUse()
        {
            for (int iteration = 0; iteration < 100; iteration++)
            {
                Trace.WriteLine(SlowMethod(iteration));
            }

            RemoveCounter(string.Format("WebApplications.Utilities.Test: {0}", OperationCategoryName));

            for (int iteration = 0; iteration < 100; iteration++)
            {
                Trace.WriteLine(SlowMethod(iteration));
            }
        }

        [TestMethod]
        [PerformanceTimer(OperationCategoryName, "0:0:0.005", "0:0:0.03")]
        public void TestMethodMultipleAttributes()
        {
            for (int iteration = 0; iteration < 100; iteration++)
            {
                Trace.WriteLine(SlowMethod(iteration));
            }
        }

        [TestMethod]
        [Operation("Creating Log", "WebApplications.Utilities.Logging")]
        public void AddCountersWithSameCategoryAsOtherAssemblies()
        {
            for (int iteration = 0; iteration < 100; iteration++)
            {
                Trace.WriteLine(SlowMethod(iteration));
            }
        }
    }
}
