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
using NodaTime;
using System;
using System.Linq;
using WebApplications.Utilities.Annotations;
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
            Assert.AreEqual(i + d, gap.Next(i));

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

        [TestMethod]
        public void TestConfigurationChangeNoChanges()
        {
            SchedulerConfiguration current = SchedulerConfiguration.Active;
            try
            {
                ISchedule oneOff = Scheduler.GetSchedule("OneOff");
                ISchedule gap = Scheduler.GetSchedule("Gap");
                ISchedule etm = Scheduler.GetSchedule("EveryTwoMonths");
                ISchedule aggregate = Scheduler.GetSchedule("Aggregate");
                bool enabled = Scheduler.Enabled;
                Duration defaultMaxDuration = Scheduler.DefaultMaximumDuration;
                int defaultMaxHistory = Scheduler.DefaultMaximumHistory;

                SchedulerConfiguration.Active = CloneConfig(current);

                Assert.AreSame(oneOff, Scheduler.GetSchedule("OneOff"));
                Assert.AreSame(gap, Scheduler.GetSchedule("Gap"));
                Assert.AreSame(etm, Scheduler.GetSchedule("EveryTwoMonths"));
                Assert.AreSame(aggregate, Scheduler.GetSchedule("Aggregate"));
                Assert.AreEqual(Scheduler.Enabled, enabled);
                Assert.AreEqual(Scheduler.DefaultMaximumDuration, defaultMaxDuration);
                Assert.AreEqual(Scheduler.DefaultMaximumHistory, defaultMaxHistory);
            }
            finally
            {
                SchedulerConfiguration.Active = current;
            }
        }

        [TestMethod]
        public void TestConfigurationChangeEnabled()
        {
            SchedulerConfiguration current = SchedulerConfiguration.Active;
            try
            {
                ISchedule oneOff = Scheduler.GetSchedule("OneOff");
                ISchedule gap = Scheduler.GetSchedule("Gap");
                ISchedule etm = Scheduler.GetSchedule("EveryTwoMonths");
                ISchedule aggregate = Scheduler.GetSchedule("Aggregate");

                Assert.AreEqual(Scheduler.Enabled, true);

                SchedulerConfiguration newConfig = CloneConfig(current);
                newConfig.Enabled = false;

                SchedulerConfiguration.Active = newConfig;

                Assert.AreEqual(Scheduler.Enabled, false);

                Assert.AreSame(oneOff, Scheduler.GetSchedule("OneOff"));
                Assert.AreSame(gap, Scheduler.GetSchedule("Gap"));
                Assert.AreSame(etm, Scheduler.GetSchedule("EveryTwoMonths"));
                Assert.AreSame(aggregate, Scheduler.GetSchedule("Aggregate"));
            }
            finally
            {
                SchedulerConfiguration.Active = current;
            }
        }

        [TestMethod]
        public void TestConfigurationChangeAggregate()
        {
            SchedulerConfiguration current = SchedulerConfiguration.Active;
            try
            {
                ISchedule oneOff = Scheduler.GetSchedule("OneOff");
                ISchedule gap = Scheduler.GetSchedule("Gap");
                ISchedule etm = Scheduler.GetSchedule("EveryTwoMonths");
                ISchedule aggregate = Scheduler.GetSchedule("Aggregate");

                SchedulerConfiguration newConfig = CloneConfig(current);
                newConfig.Schedules["Aggregate"].Parameters.Remove("schedule3");

                SchedulerConfiguration.Active = newConfig;

                Assert.AreSame(oneOff, Scheduler.GetSchedule("OneOff"));
                Assert.AreSame(gap, Scheduler.GetSchedule("Gap"));
                Assert.AreSame(etm, Scheduler.GetSchedule("EveryTwoMonths"));
                AggregateSchedule newAggregate = Scheduler.GetSchedule("Aggregate") as AggregateSchedule;
                Assert.IsNotNull(newAggregate);
                Assert.AreNotSame(aggregate, newAggregate);
                Assert.AreEqual(2, newAggregate.Count());
            }
            finally
            {
                SchedulerConfiguration.Active = current;
            }
        }

        [TestMethod]
        public void TestConfigurationChangeGap()
        {
            SchedulerConfiguration current = SchedulerConfiguration.Active;
            try
            {
                ISchedule oneOff = Scheduler.GetSchedule("OneOff");
                ISchedule gap = Scheduler.GetSchedule("Gap");
                ISchedule etm = Scheduler.GetSchedule("EveryTwoMonths");
                ISchedule aggregate = Scheduler.GetSchedule("Aggregate");

                SchedulerConfiguration newConfig = CloneConfig(current);
                newConfig.Schedules["Gap"].Parameters["timeSpan"].Value = "4.00:00:00";

                SchedulerConfiguration.Active = newConfig;

                Assert.AreSame(oneOff, Scheduler.GetSchedule("OneOff"));
                Assert.AreNotSame(gap, Scheduler.GetSchedule("Gap"));
                Assert.AreSame(etm, Scheduler.GetSchedule("EveryTwoMonths"));
                Assert.AreNotSame(aggregate, Scheduler.GetSchedule("Aggregate"));

                ISchedule newGap = Scheduler.GetSchedule("Gap");
                Assert.IsNotNull(newGap);
                Assert.IsInstanceOfType(newGap, typeof(GapSchedule));

                Instant i = Instant.FromDateTimeOffset(DateTimeOffset.Parse("13/01/2100 09:10:11 +00:00"));
                Duration d = Duration.FromTimeSpan(TimeSpan.Parse("4.00:00:00"));
                Assert.AreEqual(i + d, newGap.Next(i));

                AggregateSchedule newAggregate = Scheduler.GetSchedule("Aggregate") as AggregateSchedule;
                Assert.IsNotNull(newAggregate);

                ISchedule[] array = newAggregate.ToArray();
                Assert.AreEqual(3, array.Length);
                Assert.AreSame(oneOff, array[0]);
                Assert.AreSame(newGap, array[1]);
                Assert.AreSame(etm, array[2]);
            }
            finally
            {
                SchedulerConfiguration.Active = current;
            }
        } 

        [NotNull]
        private SchedulerConfiguration CloneConfig([NotNull] SchedulerConfiguration config)
        {
            return new SchedulerConfiguration
            {
                DefaultMaximumDuration = config.DefaultMaximumDuration,
                DefautlMaximumHistory = config.DefautlMaximumHistory,
                Enabled = config.Enabled,
                Schedules = CloneCollection(config.Schedules)
            };
        }

        [NotNull]
        private ScheduleCollection CloneCollection([NotNull][ItemNotNull] ScheduleCollection collection)
        {
            ScheduleCollection newCollection = new ScheduleCollection();

            foreach (ScheduleElement element in collection)
            {
                newCollection.Add(new ScheduleElement
                {
                    Name = element.Name,
                    Options = element.Options,
                    Type = element.Type,
                    Parameters = CloneCollection(element.Parameters)
                });
            }

            return newCollection;
        }

        [NotNull]
        private ParameterCollection CloneCollection([NotNull] [ItemNotNull] ParameterCollection collection)
        {
            ParameterCollection newCollection = new ParameterCollection();

            foreach (ParameterElement element in collection)
            {
                newCollection.Add(new ParameterElement
                {
                    Name = element.Name,
                    Value = element.Value,
                    IsRequired = element.IsRequired,
                    Type = element.Type,
                    TypeConverter = element.TypeConverter
                });
            }

            return newCollection;
        }
    }
}