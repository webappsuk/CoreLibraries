using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Scheduling.Schedules
{
    /// <summary>
    /// Defines a schedule based on a function.
    /// </summary>
    /// <remarks></remarks>
    public class FunctionalSchedule : ISchedule
    {
        private readonly ScheduleOptions _options;
        private readonly string _name;
        [NotNull]
        private readonly Func<DateTime, DateTime> _function;

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionalSchedule"/> class.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="options">The options.</param>
        /// <param name="name">The name.</param>
        /// <remarks></remarks>
        public FunctionalSchedule([NotNull]Func<DateTime, DateTime> function, ScheduleOptions options = ScheduleOptions.None, string name = null)
        {
            _function = function;
            _options = options;
            _name = name;
        }

        /// <inheritdoc/>
        public string Name
        {
            get { return _name; }
        }

        /// <inheritdoc/>
        public DateTime Next(DateTime last)
        {
            try
            {
                return _function(last);
            }
            catch
            {
                return DateTime.MaxValue;
            }
        }

        /// <inheritdoc/>
        public ScheduleOptions Options
        {
            get { return _options; }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "Next Run at " + Next(DateTime.Now);
        }
    }
}