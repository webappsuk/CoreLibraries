#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: Utilities.Scheduling 
// Project: Utilities.Scheduling
// File: ScheduleElement.cs
// 
// This software, its object code and source code and all modifications made to
// the same (the “Software”) are, and shall at all times remain, the proprietary
// information and intellectual property rights of Web Applications (UK) Limited. 
// You are only entitled to use the Software as expressly permitted by Web
// Applications (UK) Limited within the Software Customisation and
// Licence Agreement (the “Agreement”).  Any copying, modification, decompiling,
// distribution, licensing, sale, transfer or other use of the Software other than
// as expressly permitted in the Agreement is expressly forbidden.  Web
// Applications (UK) Limited reserves its rights to take action against you and
// your employer in accordance with its contractual and common law rights
// (including injunctive relief) should you breach the terms of the Agreement or
// otherwise infringe its copyright or other intellectual property rights in the
// Software.
// 
// © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
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
        [ConfigurationProperty("calendarWeekRule", DefaultValue = CalendarWeekRule.FirstFourDayWeek, IsRequired = false)]
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