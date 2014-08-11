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
using System.Diagnostics.Contracts;
using JetBrains.Annotations;
using NodaTime;

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
        [NotNull]
        public readonly CalendarSystem CalendarSystem;

        /// <summary>
        /// The time zone.
        /// </summary>
        [PublicAPI]
        [NotNull]
        public readonly DateTimeZone TimeZone;

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
        public readonly Duration MinimumGap;

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
        /// Initializes a new instance of the <see cref="PeriodicSchedule" /> class.
        /// </summary>
        /// <param name="gap">The gap.</param>
        /// <param name="calendarSystem">The calendar.</param>
        /// <param name="timeZone">The time zone.</param>
        /// <param name="options">The options.</param>
        /// <param name="name">The name.</param>
        [PublicAPI]
        public PeriodicSchedule(
            Duration gap = default(Duration),
            [CanBeNull] CalendarSystem calendarSystem = null,
            [CanBeNull] DateTimeZone timeZone = null,
            ScheduleOptions options = ScheduleOptions.None,
            [CanBeNull] string name = null)
            : this(
                name,
                Month.Every,
                Week.Every,
                Day.Every,
                WeekDay.Every,
                Hour.Every,
                Minute.Every,
                Second.Every,
                gap < Duration.Zero ? Duration.Zero : gap,
                // ReSharper disable AssignNullToNotNullAttribute
                calendarSystem ?? CalendarSystem.Iso,
                timeZone ?? DateTimeZone.Utc,
                // ReSharper restore AssignNullToNotNullAttribute
                options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PeriodicSchedule" /> class.
        /// </summary>
        /// <param name="month">The month.</param>
        /// <param name="week">The week.</param>
        /// <param name="day">The day.</param>
        /// <param name="weekDay">The week day.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="minimumGap">The minimum gap.</param>
        /// <param name="calendarSystem">The calendar.</param>
        /// <param name="timeZone">The time zone.</param>
        /// <param name="options">The options.</param>
        /// <param name="name">The name.</param>
        [PublicAPI]
        public PeriodicSchedule(
            Month month = Month.Every,
            Week week = Week.Every,
            Day day = Day.Every,
            WeekDay weekDay = WeekDay.Every,
            Hour hour = Hour.Zeroth,
            Minute minute = Minute.Zeroth,
            Second second = Second.Zeroth,
            Duration minimumGap = default(Duration),
            [CanBeNull] CalendarSystem calendarSystem = null,
            [CanBeNull] DateTimeZone timeZone = null,
            ScheduleOptions options = ScheduleOptions.None,
            [CanBeNull] string name = null)
            : this(
                name,
                month,
                week,
                day,
                weekDay,
                hour,
                minute,
                second,
                minimumGap < Duration.Zero ? Duration.Zero : minimumGap,
                // ReSharper disable AssignNullToNotNullAttribute
                calendarSystem ?? CalendarSystem.Iso,
                timeZone ?? DateTimeZone.Utc,
                // ReSharper restore AssignNullToNotNullAttribute
                options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PeriodicSchedule" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="month">The month.</param>
        /// <param name="week">The week.</param>
        /// <param name="day">The day.</param>
        /// <param name="weekDay">The week day.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="minimumGap">The minimum gap.</param>
        /// <param name="calendarSystem">The calendar.</param>
        /// <param name="timeZone">The time zone.</param>
        /// <param name="options">The options.</param>
        [PublicAPI]
        private PeriodicSchedule(
            [CanBeNull] string name,
            Month month,
            Week week,
            Day day,
            WeekDay weekDay,
            Hour hour,
            Minute minute,
            Second second,
            Duration minimumGap,
            [NotNull] CalendarSystem calendarSystem,
            [NotNull] DateTimeZone timeZone,
            ScheduleOptions options)
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
                    calendarSystem,
                    timeZone),
                options,
                name)
        {
            Contract.Requires(calendarSystem != null);
            Contract.Requires(timeZone != null);
            Month = month;
            Week = week;
            Day = day;
            WeekDay = weekDay;
            Hour = hour;
            Minute = minute;
            Second = second;
            MinimumGap = minimumGap;
            CalendarSystem = calendarSystem;
            TimeZone = timeZone;
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
        /// <param name="calendarSystem">The calendar.</param>
        /// <param name="timeZone">The time zone.</param>
        /// <returns>Func&lt;DateTime, DateTime&gt;.</returns>
        [NotNull]
        private static Func<Instant, Instant> CreateFunction(
            Month month,
            Week week,
            Day day,
            WeekDay weekDay,
            Hour hour,
            Minute minute,
            Second second,
            Duration minimumGap,
            [NotNull] CalendarSystem calendarSystem,
            [NotNull] DateTimeZone timeZone)
        {
            Contract.Requires(minimumGap != null);
            Contract.Requires(calendarSystem != null);
            Contract.Requires(timeZone != null);
            Contract.Ensures(Contract.Result<Func<Instant, Instant>>() != null);
            // Never case
            if ((month == Month.Never) ||
                (week == Week.Never) ||
                (day == Day.Never) ||
                (weekDay == WeekDay.Never) ||
                (hour == Hour.Never) ||
                (minute == Minute.Never) ||
                (second == Second.Never))
                return dt => Instant.MaxValue;

            // Every second case.
            if ((month == Month.Every) &&
                (week == Week.Every) &&
                (day == Day.Every) &&
                (weekDay == WeekDay.Every) &&
                (hour == Hour.Every) &&
                (minute == Minute.Every) &&
                (second == Second.Every))
            {
                return minimumGap <= Duration.Zero
                    ? i => i + Schedule.OneSecond
                    : (Func<Instant, Instant>)
                        (i =>
                            i < (Instant.MaxValue - minimumGap)
                                ? (i + minimumGap).Floor()
                                : Instant.MaxValue);
            }

            // Use NextValid extension method
            return minimumGap <= Duration.Zero
                ? i =>
                    i.NextValid(
                        month,
                        week,
                        day,
                        weekDay,
                        hour,
                        minute,
                        second,
                        calendarSystem,
                        timeZone)
                : (Func<Instant, Instant>)
                    (i =>
                        i < (Instant.MaxValue - minimumGap)
                            ? (i + minimumGap).NextValid(
                                month,
                                week,
                                day,
                                weekDay,
                                hour,
                                minute,
                                second,
                                calendarSystem,
                                timeZone)
                            : Instant.MaxValue);
        }
    }
}