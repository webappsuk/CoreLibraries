using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Scheduling.Schedules;

namespace WebApplications.Utilities.Scheduling.Test
{
    /// <summary>
    /// Summary description for TestSingleDateSchedule
    /// </summary>
    [TestClass]
    public class TestSingleDateSchedule
    {
        [TestMethod]
        public void PastDateTime()
        {
            OneOffSchedule oneOffSchedule = new OneOffSchedule(new DateTime(2011, 1, 1));
            Assert.AreEqual(DateTime.MaxValue, oneOffSchedule.Next(DateTime.Now));
        }

        [TestMethod]
        public void CurrentDateTime()
        {
            OneOffSchedule oneOffSchedule = new OneOffSchedule(DateTime.Now);
            Assert.AreEqual(DateTime.MaxValue, oneOffSchedule.Next(DateTime.Now));
        }

        [TestMethod]
        public void NextSecondDateTime()
        {
            OneOffSchedule oneOffSchedule
                = new OneOffSchedule(DateTime.Now + (new TimeSpan(0, 0, 1)));
            Assert.AreEqual(DateTime.Now + (new TimeSpan(0, 0, 1)), oneOffSchedule.Next(DateTime.Now));
        }

        [TestMethod]
        public void FutureDateTime()
        {
            OneOffSchedule oneOffSchedule 
                = new OneOffSchedule(DateTime.Now + (new TimeSpan(1, 0, 0, 0)));
            Assert.AreEqual((DateTime.Now + (new TimeSpan(1, 0, 0, 0))), oneOffSchedule.Next(DateTime.Now));
        }
    }
}
