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
using System.Collections.Generic;
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
            DateTime testDateTime = DateTime.UtcNow + (new TimeSpan(1, 0, 0, 0));
            OneOffSchedule oneOffScheduleschedule = new OneOffSchedule(testDateTime);
            IList<ISchedule> scheduleList = new List<ISchedule>();
            scheduleList.Add(oneOffScheduleschedule);
            AggregateSchedule aggregateSchedule = new AggregateSchedule(scheduleList);

            // Check aggregate schedule next matches single schedule next and the datetime I would expect
            Assert.AreEqual(aggregateSchedule.Next(DateTime.UtcNow), oneOffScheduleschedule.Next(DateTime.UtcNow));
            Assert.AreEqual(aggregateSchedule.Next(DateTime.UtcNow), testDateTime);
        }

        [TestMethod]
        public void SinglePastAggregateSchedule()
        {
            // Create a single (past) datetime schedule, place it in a list and use that list to construct a aggregate schedule
            DateTime testDateTime = DateTime.UtcNow - (new TimeSpan(1, 0, 0, 0));
            OneOffSchedule oneOffScheduleschedule = new OneOffSchedule(testDateTime);
            IList<ISchedule> scheduleList = new List<ISchedule>();
            scheduleList.Add(oneOffScheduleschedule);
            AggregateSchedule aggregateSchedule = new AggregateSchedule(scheduleList);

            // Check aggregate schedule next matches single schedule next and the datetime I would expect
            // This is because when a schedule is in the past the next method returns DateTime.Max
            Assert.AreEqual(aggregateSchedule.Next(DateTime.UtcNow), oneOffScheduleschedule.Next(DateTime.UtcNow));
            Assert.AreEqual(aggregateSchedule.Next(DateTime.UtcNow), DateTime.MaxValue);
        }

        [TestMethod]
        public void DoubleFutureAggregateSchedule()
        {
            // Create two (future) datetime schedules, place them in a list and use that list to construct a aggregate schedule 
            DateTime firstTestDateTime = DateTime.UtcNow + (new TimeSpan(1, 0, 0, 0));
            DateTime secondTestDateTime = firstTestDateTime + (new TimeSpan(1, 0, 0, 0));
            OneOffSchedule firstOneOffScheduleschedule = new OneOffSchedule(firstTestDateTime);
            OneOffSchedule secondOneOffScheduleschedule = new OneOffSchedule(secondTestDateTime);
            IList<ISchedule> scheduleList = new List<ISchedule>();
            scheduleList.Add(firstOneOffScheduleschedule);
            scheduleList.Add(secondOneOffScheduleschedule);
            AggregateSchedule aggregateSchedule = new AggregateSchedule(scheduleList);

            // Check aggregate schedule next matches first single schedule next and the datetime I would expect
            Assert.AreEqual(aggregateSchedule.Next(DateTime.UtcNow), firstOneOffScheduleschedule.Next(DateTime.UtcNow));
            Assert.AreEqual(aggregateSchedule.Next(DateTime.UtcNow), firstTestDateTime);
        }

        [TestMethod]
        public void DoublePastAggregateSchedule()
        {
            // Create two (past) datetime schedules, place them in a list and use that list to construct a aggregate schedule
            DateTime firstTestDateTime = DateTime.UtcNow - (new TimeSpan(1, 0, 0, 0));
            DateTime secondTestDateTime = firstTestDateTime - (new TimeSpan(1, 0, 0, 0));
            OneOffSchedule firstOneOffScheduleschedule = new OneOffSchedule(firstTestDateTime);
            OneOffSchedule secondOneOffScheduleschedule = new OneOffSchedule(secondTestDateTime);
            IList<ISchedule> scheduleList = new List<ISchedule>();
            scheduleList.Add(firstOneOffScheduleschedule);
            scheduleList.Add(secondOneOffScheduleschedule);
            AggregateSchedule aggregateSchedule = new AggregateSchedule(scheduleList);

            // Check aggregate schedule next matches both single schedules next and the datetime I would expect
            // This is because when a schedule is in the past the next method returns DateTime.Max
            Assert.AreEqual(aggregateSchedule.Next(DateTime.UtcNow), firstOneOffScheduleschedule.Next(DateTime.UtcNow));
            Assert.AreEqual(aggregateSchedule.Next(DateTime.UtcNow), secondOneOffScheduleschedule.Next(DateTime.UtcNow));
            Assert.AreEqual(aggregateSchedule.Next(DateTime.UtcNow), DateTime.MaxValue);
        }

        [TestMethod]
        public void PastFutureAggregateSchedule()
        {
            // Create two datetime schedules (past then future), 
            // place them in a list and use that list to construct a aggregate schedule
            DateTime firstTestDateTime = DateTime.UtcNow - (new TimeSpan(1, 0, 0, 0));
            DateTime secondTestDateTime = DateTime.UtcNow + (new TimeSpan(1, 0, 0, 0));
            OneOffSchedule firstOneOffScheduleschedule = new OneOffSchedule(firstTestDateTime);
            OneOffSchedule secondOneOffScheduleschedule = new OneOffSchedule(secondTestDateTime);
            IList<ISchedule> scheduleList = new List<ISchedule>();
            scheduleList.Add(firstOneOffScheduleschedule);
            scheduleList.Add(secondOneOffScheduleschedule);
            AggregateSchedule aggregateSchedule = new AggregateSchedule(scheduleList);

            // Check aggregate schedule next matches future datetime schedule and the datetime I would expect
            Assert.AreEqual(aggregateSchedule.Next(DateTime.UtcNow), secondOneOffScheduleschedule.Next(DateTime.UtcNow));
            Assert.AreEqual(aggregateSchedule.Next(DateTime.UtcNow), secondTestDateTime);
        }

        [TestMethod]
        public void FuturePastAggregateSchedule()
        {
            // Create two datetime schedules (future then past), 
            // place them in a list and use that list to construct a aggregate schedule
            DateTime firstTestDateTime = DateTime.UtcNow + (new TimeSpan(1, 0, 0, 0));
            DateTime secondTestDateTime = DateTime.UtcNow - (new TimeSpan(1, 0, 0, 0));
            OneOffSchedule firstOneOffScheduleschedule = new OneOffSchedule(firstTestDateTime);
            OneOffSchedule secondOneOffScheduleschedule = new OneOffSchedule(secondTestDateTime);
            IList<ISchedule> scheduleList = new List<ISchedule>();
            scheduleList.Add(firstOneOffScheduleschedule);
            scheduleList.Add(secondOneOffScheduleschedule);
            AggregateSchedule aggregateSchedule = new AggregateSchedule(scheduleList);

            // Check aggregate schedule next matches future datetime schedule and the datetime I would expect
            Assert.AreEqual(aggregateSchedule.Next(DateTime.UtcNow), firstOneOffScheduleschedule.Next(DateTime.UtcNow));
            Assert.AreEqual(aggregateSchedule.Next(DateTime.UtcNow), firstTestDateTime);
        }
    }
}