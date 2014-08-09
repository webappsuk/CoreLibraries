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
using System.Globalization;
using JetBrains.Annotations;
using WebApplications.Utilities.Configuration;
using WebApplications.Utilities.Scheduling.Schedules;
using ConfigurationElement = WebApplications.Utilities.Configuration.ConfigurationElement;

namespace WebApplications.Utilities.Scheduling.Configuration
{
    /// <summary>
    /// For use in <see cref="ConfigurationSection{T}"/>s, or <see cref="ScheduleCollection"/>s
    /// for easy specification of a schedule.
    /// </summary>
    /// <remarks></remarks>
    public class ScheduleElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        /// <remarks></remarks>
        [ConfigurationProperty("name", DefaultValue = "", IsRequired = false)]
        [StringValidator(MinLength = 0)]
        [NotNull]
        public string Name
        {
            get { return GetProperty<string>("name"); }
            set { SetProperty("name", value); }
        }

        /// <summary>
        /// Gets or sets the month.
        /// </summary>
        /// <value>The month.</value>
        /// <remarks></remarks>
        [ConfigurationProperty("month", DefaultValue = Month.Every, IsRequired = false)]
        public Month Month
        {
            get { return GetProperty<Month>("month"); }
            set { SetProperty("month", value); }
        }

        /// <summary>
        /// Gets or sets the week.
        /// </summary>
        /// <value>The week.</value>
        /// <remarks></remarks>
        [ConfigurationProperty("week", DefaultValue = Week.Every, IsRequired = false)]
        public Week Week
        {
            get { return GetProperty<Week>("week"); }
            set { SetProperty("week", value); }
        }

        /// <summary>
        /// Gets or sets the day.
        /// </summary>
        /// <value>The day.</value>
        /// <remarks></remarks>
        [ConfigurationProperty("day", DefaultValue = Day.Every, IsRequired = false)]
        public Day Day
        {
            get { return GetProperty<Day>("day"); }
            set { SetProperty("day", value); }
        }

        /// <summary>
        /// Gets or sets the week day.
        /// </summary>
        /// <value>The week day.</value>
        /// <remarks></remarks>
        [ConfigurationProperty("weekDay", DefaultValue = WeekDay.Every, IsRequired = false)]
        public WeekDay WeekDay
        {
            get { return GetProperty<WeekDay>("weekDay"); }
            set { SetProperty("weekDay", value); }
        }

        /// <summary>
        /// Gets or sets the hour.
        /// </summary>
        /// <value>The hour.</value>
        /// <remarks></remarks>
        [ConfigurationProperty("hour", DefaultValue = Hour.Every, IsRequired = false)]
        public Hour Hour
        {
            get { return GetProperty<Hour>("hour"); }
            set { SetProperty("hour", value); }
        }

        /// <summary>
        /// Gets or sets the minute.
        /// </summary>
        /// <value>The minute.</value>
        /// <remarks></remarks>
        [ConfigurationProperty("minute", DefaultValue = Minute.Every, IsRequired = false)]
        public Minute Minute
        {
            get { return GetProperty<Minute>("minute"); }
            set { SetProperty("minute", value); }
        }

        /// <summary>
        /// Gets or sets the second.
        /// </summary>
        /// <value>The second.</value>
        /// <remarks></remarks>
        [ConfigurationProperty("second", DefaultValue = Second.Every, IsRequired = false)]
        public Second Second
        {
            get { return GetProperty<Second>("second"); }
            set { SetProperty("second", value); }
        }

        /// <summary>
        /// Gets or sets the time span for the minimum gap.
        /// </summary>
        /// <value>The time span.</value>
        /// <remarks></remarks>
        [ConfigurationProperty("timeSpan", IsRequired = false)]
        [TimeSpanValidator(MinValueString = "00:00:00")]
        public TimeSpan TimeSpan
        {
            get { return GetProperty<TimeSpan>("timeSpan"); }
            set { SetProperty("timeSpan", value); }
        }

        /// <summary>
        /// Gets or sets the calendar week rule.
        /// </summary>
        /// <value>The calendar week rule.</value>
        /// <remarks></remarks>
        [ConfigurationProperty("calendarWeekRule", DefaultValue = CalendarWeekRule.FirstFourDayWeek, IsRequired = false)
        ]
        public CalendarWeekRule CalendarWeekRule
        {
            get { return GetProperty<CalendarWeekRule>("calendarWeekRule"); }
            set { SetProperty("calendarWeekRule", value); }
        }

        /// <summary>
        /// Gets or sets the first day of week.
        /// </summary>
        /// <value>The first day of week.</value>
        /// <remarks></remarks>
        [ConfigurationProperty("firstDayOfWeek", DefaultValue = DayOfWeek.Sunday, IsRequired = false)]
        public DayOfWeek FirstDayOfWeek
        {
            get { return GetProperty<DayOfWeek>("firstDayOfWeek"); }
            set { SetProperty("firstDayOfWeek", value); }
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="WebApplications.Utilities.Scheduling.Configuration.ScheduleElement"/>
        /// to <see cref="PeriodicSchedule"/>.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks></remarks>
        [NotNull]
        public static implicit operator PeriodicSchedule(ScheduleElement element)
        {
            return
                element == null
                    ? new PeriodicSchedule()
                    : new PeriodicSchedule(
                        month: element.Month,
                        week: element.Week,
                        day: element.Day,
                        weekDay: element.WeekDay,
                        hour: element.Hour,
                        minute: element.Minute,
                        second: element.Second,
                        minimumGap: element.TimeSpan,
                        calendarWeekRule: element.CalendarWeekRule,
                        firstDayOfWeek: element.FirstDayOfWeek);
        }
    }
}