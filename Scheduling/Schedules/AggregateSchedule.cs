using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using WebApplications.Utilities.Logging;

namespace WebApplications.Utilities.Scheduling.Schedules
{
    /// <summary>
    /// Creates a schedule made up of multiple other schedules
    /// </summary>
    public class AggregateSchedule : ISchedule
    {
        [NotNull]
        private readonly IEnumerable<ISchedule> _scheduleCollection;

        private readonly string _name;

        /// <inheritdoc/>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateSchedule"/> class.
        /// </summary>
        /// <param name="scheduleCollection">A collection of schedules.</param>
        /// <param name="name">An optional name for the schedule.</param>
        public AggregateSchedule([NotNull]IEnumerable<ISchedule> scheduleCollection, string name = null)
        {
            _scheduleCollection = scheduleCollection.ToList();
            _name = name;
            if (!_scheduleCollection.Any())
            {
                _options = ScheduleOptions.None;
                return;
            }
            
            // Calculate options and ensure all are identical.
            bool first = true;
            foreach (var schedule in _scheduleCollection)
            {
                if (schedule == null)
                    throw new ArgumentException("Cannot have null schedules in schedule collection.",
                                                "scheduleCollection");

                if (first)
                {
                    _options = schedule.Options;
                    first = false;
                    continue;
                }
                if (schedule.Options != _options)
                    throw new ArgumentException(
                        "Cannot create an aggregate schedule out of schedules which have different options.",
                        "scheduleCollection");
            }
        }

        private readonly ScheduleOptions _options;

        /// <inheritdoc/>
        public DateTime Next(DateTime last)
        {
            DateTime next = DateTime.MaxValue;
            foreach (ISchedule schedule in _scheduleCollection)
            {
                DateTime scheduleNext = schedule.Next(last);
                if (scheduleNext < last)
                {
                    Log.Add("Schedule '{0}' returned a DateTime in the past.", schedule.Name);
                    continue;
                }
                if (scheduleNext < next)
                    next = scheduleNext;
            }
            return next;
        }

        /// <inheritdoc/>
        public ScheduleOptions Options
        {
            get { return _options; }
        }

        #region IEnumerableCode
        /// <inheritdoc/>
        public IEnumerator<DateTime> GetEnumerator()
        {
            DateTime next = DateTime.MinValue;
            while ((next = Next(next)) < DateTime.MaxValue)
                yield return next;
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        /// <inheritdoc/>
        public override string ToString()
        {
            return "Next Run at " + Next(DateTime.Now);
        }
    }
}