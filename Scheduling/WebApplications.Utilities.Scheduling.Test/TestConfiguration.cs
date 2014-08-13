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
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using WebApplications.Utilities.Configuration;
using WebApplications.Utilities.Scheduling.Configuration;
using WebApplications.Utilities.Scheduling.Schedules;

namespace WebApplications.Utilities.Scheduling.Test
{
    [TestClass]
    public class TestConfiguration
    {
        [TestMethod]
        public void TestMethod1()
        {
            ISchedule oneOff = Scheduler.GetSchedule("OneOff");
            Assert.IsNotNull(oneOff);
            Assert.IsInstanceOfType(oneOff, typeof(OneOffSchedule));

            Instant i = Instant.FromDateTimeOffset(DateTimeOffset.Parse("13/01/2100 09:10:11 +00:00"));
            Assert.AreEqual(i, oneOff.Next(Instant.MinValue));
            Assert.AreEqual(Instant.MaxValue, oneOff.Next(i + Duration.FromTicks(1)));


            ISchedule gap = Scheduler.GetSchedule("Gap");
            Assert.IsNotNull(gap);
            Assert.IsInstanceOfType(gap, typeof(GapSchedule));

            Duration d = Duration.FromTimeSpan(TimeSpan.Parse("3.12:00:00"));
            Assert.AreEqual(i+d, gap.Next(i));

            ISchedule etm = Scheduler.GetSchedule("EveryTwoMonths");
            Assert.IsNotNull(etm);
            Assert.IsInstanceOfType(etm, typeof(PeriodicSchedule));



            ISchedule aggregate = Scheduler.GetSchedule("Aggregate");
            Assert.IsNotNull(aggregate);
            Assert.IsInstanceOfType(aggregate, typeof(AggregateSchedule));

            AggregateSchedule ags = aggregate as AggregateSchedule;
            Assert.IsNotNull(ags);

            ISchedule[] array = ags.ToArray();
            Assert.AreEqual(3, array.Length);
            Assert.AreSame(oneOff, array[0]);
            Assert.AreSame(gap, array[1]);
            Assert.AreSame(etm, array[2]);

        }
    }
}