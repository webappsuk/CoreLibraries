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
        [PublicAPI]
        public PeriodicSchedule(
            TimeSpan gap,
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
        [PublicAPI]
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
            : base(
                CreateFunction(
                    month,
                    week,
                    day,
                    weekDay,
                    hour,
                    minute,
                    second,
                    minimumGap,
                    calendar,
                    calendarWeekRule,
                    firstDayOfWeek),
                options,
                name)
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
            if ((month == Month.Never) ||
                (week == Week.Never) ||
                (day == Day.Never) ||
                (weekDay == WeekDay.Never) ||
                (hour == Hour.Never) ||
                (minute == Minute.Never) ||
                (second == Second.Never))
                return dt => DateTime.MaxValue;

            // Needed in case calendar is not provided
            if (calendar == null)
                calendar = CultureInfo.CurrentCulture.Calendar;

            // Every second case.
            if ((month == Month.Every) &&
                (week == Week.Every) &&
                (day == Day.Every) &&
                (weekDay == WeekDay.Every) &&
                (hour == Hour.Every) &&
                (minute == Minute.Every) &&
                (second == Second.Every))
            {
                return minimumGap > TimeSpan.Zero
                    ? (Func<DateTime, DateTime>)
                        (dt =>
                            dt < (DateTime.MaxValue - minimumGap)
                                ? calendar.AddSeconds(
                                    new DateTime(
                                        dt.Year,
                                        dt.Month,
                                        dt.Day,
                                        dt.Hour,
                                        dt.Minute,
                                        dt.Second),
                                    1).Add(
                                        minimumGap)
                                : DateTime.MaxValue)
                    : (dt => calendar.AddSeconds(
                        new DateTime(
                            dt.Year,
                            dt.Month,
                            dt.Day,
                            dt.Hour,
                            dt.Minute,
                            dt.Second),
                        1));
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