using System.Configuration;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var configuration = ScheduleConfiguration.Active;
            foreach (PeriodicSchedule schedule in configuration.Schedules)
            {
                Trace.WriteLine(schedule);
            }
        }
    }

    /// <summary>
    /// Test schedule configuration.
    /// </summary>
    /// <remarks></remarks>
    public class ScheduleConfiguration : ConfigurationSection<ScheduleConfiguration>
    {
        [ConfigurationProperty("schedule", IsRequired = true)]
        public ScheduleElement Schedule
        {
            get { return (ScheduleElement) this["schedule"]; }
            set { this["schedule"] = value; }
        }

        [ConfigurationProperty("schedules", IsRequired = true, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ScheduleCollection))]
        public ScheduleCollection Schedules
        {
            get { return (ScheduleCollection)this["schedules"]; }
            set { this["schedules"] = value; }
        }
    }
}
