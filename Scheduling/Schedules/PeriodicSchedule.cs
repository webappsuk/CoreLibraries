#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: Utilities.Scheduling 
// Project: Utilities.Scheduling
// File: Schedule.cs
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
using System.Globalization;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Scheduling.Schedules
{
    /// <summary>
    /// Defines a schedule.
    /// </summary>
    public class PeriodicSchedule : FunctionalSchedule
    {
        /// <summary>
        /// The calendar.
        /// </summary>
        [PublicAPI]
        [CanBeNull]
        public readonly Calendar Calendar;

        /// <summary>
        /// The calendar week rule.
        /// </summary>
        [PublicAPI]
        public readonly CalendarWeekRule CalendarWeekRule;

        /// <summary>
        /// The day.
        /// </summary>
        [PublicAPI]
        public readonly Day Day;

        /// <summary>
        /// The day of the week.
        /// </summary>
        [PublicAPI]
        public readonly DayOfWeek FirstDayOfWeek;

        /// <summary>
        /// The hour.
        /// </summary>
        [PublicAPI]
        public readonly Hour Hour;

        /// <summary>
        /// The minimum gap, added to all supplied date times.
        /// </summary>
        [PublicAPI]
        public readonly TimeSpan MinimumGap;

        /// <summary>
        /// The minute.
        /// </summary>
        [PublicAPI]
        public readonly Minute Minute;

        /// <summary>
        /// The month.
        /// </summary>
        [PublicAPI]
        public readonly Month Month;

        /// <summary>
        /// The second.
        /// </summary>
        [PublicAPI]
        public readonly Second Second;

        /// <summary>
        /// The week.
        /// </summary>
        [PublicAPI]
        public readonly Week Week;

        /// <summary>
        /// The weekday.
        /// </summary>
        [PublicAPI]
        public readonly WeekDay WeekDay;

        /// <summary>
        /// Initializes a new instance of the <see cref="PeriodicSchedule"/> class.
        /// </summary>
        /// <param name="gap">The gap.</param>
        /// <param name="options">The options.</param>
        /// <param name="name">The name.</param>
        /// <remarks></remarks>
        public PeriodicSchedule(TimeSpan gap,
            ScheduleOptions options = ScheduleOptions.None,
            [CanBeNull] string name = null)
            : this(
                hour: Hour.Every,
                minute: Minute.Every,
                second: Second.Every,
                minimumGap: gap,
                name: name,
                options: options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PeriodicSchedule"/> class.
        /// </summary>
        /// <param name="month">The month.</param>
        /// <param name="week">The week.</param>
        /// <param name="day">The day.</param>
        /// <param name="weekDay">The week day.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="minimumGap">The minimum gap.</param>
        /// <param name="calendar">The calendar.</param>
        /// <param name="calendarWeekRule">The calendar week rule.</param>
        /// <param name="firstDayOfWeek">The first day of week.</param>
        /// <param name="options">The options.</param>
        /// <param name="name">The name.</param>
        /// <remarks></remarks>
        public PeriodicSchedule(
            Month month = Month.Every,
            Week week = Week.Every,
            Day day = Day.Every,
            WeekDay weekDay = WeekDay.Every,
            Hour hour = Hour.Zeroth,
            Minute minute = Minute.Zeroth,
            Second second = Second.Zeroth,
            TimeSpan minimumGap = default(TimeSpan),
            [CanBeNull] Calendar calendar = null,
            CalendarWeekRule calendarWeekRule = CalendarWeekRule.FirstFourDayWeek,
            DayOfWeek firstDayOfWeek = DayOfWeek.Sunday,
            ScheduleOptions options = ScheduleOptions.None,
            [CanBeNull] string name = null)
            : base(CreateFunction(month, week, day, weekDay, hour, minute, second, minimumGap, calendar, calendarWeekRule, firstDayOfWeek), options, name)
        {
            // If minimum gap is negative, set to zero.
            if (minimumGap < TimeSpan.Zero)
                minimumGap = TimeSpan.Zero;

            MinimumGap = minimumGap;

            // Needed in case calendar is not provided
            if (calendar == null)
                calendar = CultureInfo.CurrentCulture.Calendar;

            Month = month;
            Week = week;
            Day = day;
            WeekDay = weekDay;
            Hour = hour;
            Minute = minute;
            Second = second;
            Calendar = calendar;
            CalendarWeekRule = calendarWeekRule;
            FirstDayOfWeek = firstDayOfWeek;
        }

        /// <summary>
        /// Creates the function.
        /// </summary>
        /// <param name="month">The month.</param>
        /// <param name="week">The week.</param>
        /// <param name="day">The day.</param>
        /// <param name="weekDay">The week day.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="minimumGap">The minimum gap.</param>
        /// <param name="calendar">The calendar.</param>
        /// <param name="calendarWeekRule">The calendar week rule.</param>
        /// <param name="firstDayOfWeek">The first day of week.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        [NotNull]
        private static Func<DateTime, DateTime> CreateFunction(
            Month month,
            Week week,
            Day day,
            WeekDay weekDay,
            Hour hour,
            Minute minute,
            Second second,
            TimeSpan minimumGap,
            [CanBeNull] Calendar calendar,
            CalendarWeekRule calendarWeekRule,
            DayOfWeek firstDayOfWeek)
        {
            // Never case
            if ((month == Month.Never) || (week == Week.Never) || (day == Day.Never) || (weekDay == WeekDay.Never) ||
                (hour == Hour.Never) ||
                (minute == Minute.Never) ||
                (second == Second.Never))
                return dt => DateTime.MaxValue;

            // Needed in case calendar is not provided
            if (calendar == null)
                calendar = CultureInfo.CurrentCulture.Calendar;

            // Every second case.
            if ((month == Month.Every) && (week == Week.Every) && (day == Day.Every) && (weekDay == WeekDay.Every) &&
                (hour == Hour.Every) &&
                (minute == Minute.Every) &&
                (second == Second.Every))
            {
                return minimumGap > TimeSpan.Zero
                                       ? (Func<DateTime, DateTime>)
                                         (dt =>
                                          dt < (DateTime.MaxValue - minimumGap)
                                              ? calendar.AddSeconds(new DateTime(dt.Year, dt.Month, dt.Day,
                                                                                 dt.Hour, dt.Minute, dt.Second), 1).Add(
                                                                                     minimumGap)
                                              : DateTime.MaxValue)
                                       : (dt => calendar.AddSeconds(new DateTime(dt.Year, dt.Month, dt.Day,
                                                                                 dt.Hour, dt.Minute, dt.Second), 1));
            }

            // Use NextValid extension method
            return minimumGap > TimeSpan.Zero
                                   ? (Func<DateTime, DateTime>)
                                     (dt =>
                                      dt < (DateTime.MaxValue - minimumGap)
                                          ? dt.Add(minimumGap).NextValid(
                                              month,
                                              week,
                                              day,
                                              weekDay,
                                              hour,
                                              minute,
                                              second,
                                              calendar,
                                              calendarWeekRule,
                                              firstDayOfWeek)
                                          : DateTime.MaxValue)
                                   : (dt =>
                                      dt.NextValid(
                                          month,
                                          week,
                                          day,
                                          weekDay,
                                          hour,
                                          minute,
                                          second,
                                          calendar,
                                          calendarWeekRule,
                                          firstDayOfWeek));
        }
    }
}