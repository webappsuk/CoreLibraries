using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using WebApplications.Utilities.Configuration;
using WebApplications.Utilities.Scheduling.Schedules;

namespace WebApplications.Utilities.Scheduling.Configuration
{
    /// <summary>
    /// A collection of schedules for use in <see cref="ConfigurationSection"/>s.
    /// </summary>
    /// <remarks></remarks>
    public class ScheduleCollection : ConfigurationElementCollection<string, ScheduleElement>, IEnumerable<PeriodicSchedule>
    {
        /// <summary>
        /// Gets the element key.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected override string GetElementKey(ScheduleElement element)
        {
            return element.Name;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.</returns>
        /// <remarks></remarks>
        IEnumerator<PeriodicSchedule> IEnumerable<PeriodicSchedule>.GetEnumerator()
        {
            return this.Cast<PeriodicSchedule>().GetEnumerator();
        }
    }
}
