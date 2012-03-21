using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Scheduling.Schedules;

namespace WebApplications.Utilities.Scheduling.Test
{
    [TestClass]
    public class TestAggregateSchedule
    {
        [TestMethod]
        public void CreateEmptyAggregateSchedule()
        {
            List<ISchedule> scheduleCollection = new List<ISchedule>();
            new AggregateSchedule(scheduleCollection);
        }

        [TestMethod]
        public void CreateSingleScheduleAggregateSchedule()
        {
            List<ISchedule> scheduleCollection = new List<ISchedule>();
            ISchedule schedule = new OneOffSchedule(new DateTime(2010, 1, 1));
            scheduleCollection.Add(schedule);
            new AggregateSchedule(scheduleCollection);
        }

        [TestMethod]
        public void SingleFutureAggregateSchedule()
        {
            // Create a single (future) datetime schedule, place it in a list and use that list to construct a aggregate schedule
            DateTime testDateTime = DateTime.Now + (new TimeSpan(1, 0, 0, 0));
            OneOffSchedule oneOffScheduleschedule = new OneOffSchedule(testDateTime);
            IList<ISchedule> scheduleList = new List<ISchedule>();
            scheduleList.Add(oneOffScheduleschedule);
            AggregateSchedule aggregateSchedule = new AggregateSchedule(scheduleList);

            // Check aggregate schedule next matches single schedule next and the datetime I would expect
            Assert.AreEqual(aggregateSchedule.Next(DateTime.Now), oneOffScheduleschedule.Next(DateTime.Now));
            Assert.AreEqual(aggregateSchedule.Next(DateTime.Now), testDateTime);
        }

        [TestMethod]
        public void SinglePastAggregateSchedule()
        {
            // Create a single (past) datetime schedule, place it in a list and use that list to construct a aggregate schedule
            DateTime testDateTime = DateTime.Now - (new TimeSpan(1, 0, 0, 0));
            OneOffSchedule oneOffScheduleschedule = new OneOffSchedule(testDateTime);
            IList<ISchedule> scheduleList = new List<ISchedule>();
            scheduleList.Add(oneOffScheduleschedule);
            AggregateSchedule aggregateSchedule = new AggregateSchedule(scheduleList);

            // Check aggregate schedule next matches single schedule next and the datetime I would expect
            // This is because when a schedule is in the past the next method returns DateTime.Max
            Assert.AreEqual(aggregateSchedule.Next(DateTime.Now), oneOffScheduleschedule.Next(DateTime.Now));
            Assert.AreEqual(aggregateSchedule.Next(DateTime.Now), DateTime.MaxValue);
        }

        [TestMethod]
        public void DoubleFutureAggregateSchedule()
        {
            // Create two (future) datetime schedules, place them in a list and use that list to construct a aggregate schedule 
            DateTime firstTestDateTime = DateTime.Now + (new TimeSpan(1, 0, 0, 0));
            DateTime secondTestDateTime = firstTestDateTime + (new TimeSpan(1, 0, 0, 0));
            OneOffSchedule firstOneOffScheduleschedule = new OneOffSchedule(firstTestDateTime);
            OneOffSchedule secondOneOffScheduleschedule = new OneOffSchedule(secondTestDateTime);
            IList<ISchedule> scheduleList = new List<ISchedule>();
            scheduleList.Add(firstOneOffScheduleschedule);
            scheduleList.Add(secondOneOffScheduleschedule);
            AggregateSchedule aggregateSchedule = new AggregateSchedule(scheduleList);

            // Check aggregate schedule next matches first single schedule next and the datetime I would expect
            Assert.AreEqual(aggregateSchedule.Next(DateTime.Now), firstOneOffScheduleschedule.Next(DateTime.Now));
            Assert.AreEqual(aggregateSchedule.Next(DateTime.Now), firstTestDateTime);
        }

        [TestMethod]
        public void DoublePastAggregateSchedule()
        {
            // Create two (past) datetime schedules, place them in a list and use that list to construct a aggregate schedule
            DateTime firstTestDateTime = DateTime.Now - (new TimeSpan(1, 0, 0, 0));
            DateTime secondTestDateTime = firstTestDateTime - (new TimeSpan(1, 0, 0, 0));
            OneOffSchedule firstOneOffScheduleschedule = new OneOffSchedule(firstTestDateTime);
            OneOffSchedule secondOneOffScheduleschedule = new OneOffSchedule(secondTestDateTime);
            IList<ISchedule> scheduleList = new List<ISchedule>();
            scheduleList.Add(firstOneOffScheduleschedule);
            scheduleList.Add(secondOneOffScheduleschedule);
            AggregateSchedule aggregateSchedule = new AggregateSchedule(scheduleList);

            // Check aggregate schedule next matches both single schedules next and the datetime I would expect
            // This is because when a schedule is in the past the next method returns DateTime.Max
            Assert.AreEqual(aggregateSchedule.Next(DateTime.Now), firstOneOffScheduleschedule.Next(DateTime.Now));
            Assert.AreEqual(aggregateSchedule.Next(DateTime.Now), secondOneOffScheduleschedule.Next(DateTime.Now));
            Assert.AreEqual(aggregateSchedule.Next(DateTime.Now), DateTime.MaxValue);
        }

        [TestMethod]
        public void PastFutureAggregateSchedule()
        {
            // Create two datetime schedules (past then future), 
            // place them in a list and use that list to construct a aggregate schedule
            DateTime firstTestDateTime = DateTime.Now - (new TimeSpan(1, 0, 0, 0));
            DateTime secondTestDateTime = DateTime.Now + (new TimeSpan(1, 0, 0, 0));
            OneOffSchedule firstOneOffScheduleschedule = new OneOffSchedule(firstTestDateTime);
            OneOffSchedule secondOneOffScheduleschedule = new OneOffSchedule(secondTestDateTime);
            IList<ISchedule> scheduleList = new List<ISchedule>();
            scheduleList.Add(firstOneOffScheduleschedule);
            scheduleList.Add(secondOneOffScheduleschedule);
            AggregateSchedule aggregateSchedule = new AggregateSchedule(scheduleList);

            // Check aggregate schedule next matches future datetime schedule and the datetime I would expect
            Assert.AreEqual(aggregateSchedule.Next(DateTime.Now), secondOneOffScheduleschedule.Next(DateTime.Now));
            Assert.AreEqual(aggregateSchedule.Next(DateTime.Now), secondTestDateTime);
        }

        [TestMethod]
        public void FuturePastAggregateSchedule()
        {
            // Create two datetime schedules (future then past), 
            // place them in a list and use that list to construct a aggregate schedule
            DateTime firstTestDateTime = DateTime.Now + (new TimeSpan(1, 0, 0, 0));
            DateTime secondTestDateTime = DateTime.Now - (new TimeSpan(1, 0, 0, 0));
            OneOffSchedule firstOneOffScheduleschedule = new OneOffSchedule(firstTestDateTime);
            OneOffSchedule secondOneOffScheduleschedule = new OneOffSchedule(secondTestDateTime);
            IList<ISchedule> scheduleList = new List<ISchedule>();
            scheduleList.Add(firstOneOffScheduleschedule);
            scheduleList.Add(secondOneOffScheduleschedule);
            AggregateSchedule aggregateSchedule = new AggregateSchedule(scheduleList);

            // Check aggregate schedule next matches future datetime schedule and the datetime I would expect
            Assert.AreEqual(aggregateSchedule.Next(DateTime.Now), firstOneOffScheduleschedule.Next(DateTime.Now));
            Assert.AreEqual(aggregateSchedule.Next(DateTime.Now), firstTestDateTime);
        }
    }
}
