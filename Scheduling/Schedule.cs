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
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using NodaTime;
using WebApplications.Utilities.Scheduling.Schedules;

namespace WebApplications.Utilities.Scheduling
{
    /// <summary>
    /// Extension methods for scheduling
    /// </summary>
    public static class Schedule
    {
        /// <summary>
        /// A schedule that will never run.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly ISchedule Never = new OneOffSchedule("Never", Instant.MaxValue);

        /// <summary>
        /// The one second <see cref="Duration"/>.
        /// </summary>
        [PublicAPI]
        public static readonly Duration OneSecond = Duration.FromSeconds(1);

        /// <summary>
        /// The one standard day <see cref="Duration"/>.
        /// </summary>
        [PublicAPI]
        public static readonly Duration OneStandardDay = Duration.FromStandardDays(1);
        
        #region Duration
        /// <summary>
        /// Gets the value of the <see cref="Duration"/> expressed in whole and fractional milliseconds.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns>System.Double.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double TotalMilliseconds(this Duration duration)
        {
            return (double)duration.Ticks / NodaConstants.TicksPerMillisecond;
        }

        /// <summary>
        /// Gets the milliseconds component of the <see cref="Duration"/>.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns>System.Double.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Milliseconds(this Duration duration)
        {
            return (int)(duration.Ticks / NodaConstants.TicksPerMillisecond) % 1000;
        }

        /// <summary>
        /// Gets the value of the <see cref="Duration"/> expressed in whole and fractional seconds.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns>System.Double.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double TotalSeconds(this Duration duration)
        {
            return (double)duration.Ticks / NodaConstants.TicksPerSecond;
        }

        /// <summary>
        /// Gets the seconds component of the <see cref="Duration"/>.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns>System.Double.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Seconds(this Duration duration)
        {
            return (int)(duration.Ticks / NodaConstants.TicksPerSecond) % 60;
        }

        /// <summary>
        /// Gets the value of the <see cref="Duration"/> expressed in whole and fractional minutes.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns>System.Double.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double TotalMinutes(this Duration duration)
        {
            return (double)duration.Ticks / NodaConstants.TicksPerMinute;
        }

        /// <summary>
        /// Gets the minutes component of the <see cref="Duration"/>.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns>System.Double.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Minutes(this Duration duration)
        {
            return (int)(duration.Ticks / NodaConstants.TicksPerMinute) % 60;
        }

        /// <summary>
        /// Gets the value of the <see cref="Duration"/> expressed in whole and fractional hours.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns>System.Double.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double TotalHours(this Duration duration)
        {
            return (double)duration.Ticks / NodaConstants.TicksPerHour;
        }

        /// <summary>
        /// Gets the hours component of the <see cref="Duration"/>.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns>System.Double.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Hours(this Duration duration)
        {
            return (int)(duration.Ticks / NodaConstants.TicksPerHour) % 24;
        }

        /// <summary>
        /// Gets the value of the <see cref="Duration"/> expressed in whole and fractional standard days.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns>System.Double.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double TotalStandardDays(this Duration duration)
        {
            return (double)duration.Ticks / NodaConstants.TicksPerStandardDay;
        }

        /// <summary>
        /// Gets the standard days component of the <see cref="Duration"/>.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns>System.Double.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int StandardDays(this Duration duration)
        {
            return (int)(duration.Ticks / NodaConstants.TicksPerStandardDay);
        }

        /// <summary>
        /// Gets the value of the <see cref="Duration"/> expressed in whole and fractional standard days.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns>System.Double.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double TotalStandardWeeks(this Duration duration)
        {
            return (double)duration.Ticks / NodaConstants.TicksPerStandardWeek;
        }

        /// <summary>
        /// Gets the standard weeks component of the <see cref="Duration"/>.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns>System.Double.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int StandardWeeks(this Duration duration)
        {
            return (int)(duration.Ticks / NodaConstants.TicksPerStandardWeek);
        }
        #endregion

        #region Months
        /// <summary>
        /// Converts a <see cref="Month">Month enum</see> into a enumeration of month integers.
        /// </summary>
        /// <param name="month">The month.</param>
        /// <returns>An enumeration of month integers.</returns>
        [NotNull]
        [PublicAPI]
        public static IEnumerable<int> Months(this Month month)
        {
            Contract.Ensures(Contract.Result<IEnumerable<int>>() != null);
            // Cast to an integer
            ulong m = (ulong)month;

            // Check bits
            List<int> valid = new List<int>();
            for (int i = 1; i < 13; i++)
            {
                ulong bit = (ulong)1 << i;
                if ((m & bit) == bit)
                    valid.Add(i);
            }
            return valid;
        }

        /// <summary>
        /// Converts an array of integers into a <see cref="Month">Month enum</see>.
        /// </summary>
        /// <param name="months">The months.</param>
        /// <returns>A <see cref="Month">Month enum</see>.</returns>
        [PublicAPI]
        public static Month Months([NotNull] params int[] months)
        {
            Contract.Requires(months != null);
            return months.Months();
        }

        /// <summary>
        /// Converts an enumeration of integers into a <see cref="Month">Month enum</see>.
        /// </summary>
        /// <param name="months">The months.</param>
        /// <returns>A <see cref="Month">Month enum</see>.</returns>
        [PublicAPI]
        public static Month Months([NotNull] this IEnumerable<int> months)
        {
            Contract.Requires(months != null);
            return (Month)months.Aggregate<int, ulong>(0, (current, m) => current | (ulong)1 << m);
        }
        #endregion

        #region Days
        /// <summary>
        /// Converts a <see cref="Month">Day enum</see> into a enumeration of day integers.
        /// </summary>
        /// <param name="day">The day.</param>
        /// <returns>An enumeration of day integers.</returns>
        [NotNull]
        [PublicAPI]
        public static IEnumerable<int> Days(this Day day)
        {
            Contract.Ensures(Contract.Result<IEnumerable<int>>() != null);
            // Cast to an integer
            ulong d = (ulong)day;

            // Check bits
            List<int> valid = new List<int>();
            for (int i = 1; i < 32; i++)
            {
                ulong bit = (ulong)1 << i;
                if ((d & bit) == bit)
                    valid.Add(i);
            }
            return valid;
        }

        /// <summary>
        /// Converts an array of integers into a <see cref="Day">Day enum</see>.
        /// </summary>
        /// <param name="days">The days.</param>
        /// <returns>A <see cref="Day">Day enum</see>.</returns>
        [PublicAPI]
        public static Day Days([NotNull] params int[] days)
        {
            Contract.Requires(days != null);
            return days.Days();
        }

        /// <summary>
        /// Converts an enumeration of integers into a <see cref="Day">Day enum</see>.
        /// </summary>
        /// <param name="days">The days.</param>
        /// <returns>A <see cref="Day">Day enum</see>.</returns>
        [PublicAPI]
        public static Day Days([NotNull] this IEnumerable<int> days)
        {
            Contract.Requires(days != null);
            return (Day)days.Aggregate<int, ulong>(0, (current, d) => current | (ulong)1 << d);
        }
        #endregion

        #region Weeks
        /// <summary>
        /// Converts a <see cref="Month">Week enum</see> into a enumeration of week integers.
        /// </summary>
        /// <param name="week">The week.</param>
        /// <returns>An enumeration of week integers.</returns>
        [NotNull]
        [PublicAPI]
        public static IEnumerable<int> Weeks(this Week week)
        {
            Contract.Ensures(Contract.Result<IEnumerable<int>>() != null);
            // Cast to an integer
            ulong d = (ulong)week;

            // Check bits
            List<int> valid = new List<int>();
            for (int i = 0; i < 53; i++)
            {
                ulong bit = (ulong)1 << i;
                if ((d & bit) == bit)
                    valid.Add(i);
            }
            return valid;
        }

        /// <summary>
        /// Converts an array of integers into a <see cref="Week">Week enum</see>.
        /// </summary>
        /// <param name="weeks">The weeks.</param>
        /// <returns>A <see cref="Week">Week enum</see>.</returns>
        [PublicAPI]
        public static Week Weeks([NotNull] params int[] weeks)
        {
            Contract.Requires(weeks != null);
            return weeks.Weeks();
        }

        /// <summary>
        /// Converts an enumeration of integers into a <see cref="Week">Week enum</see>.
        /// </summary>
        /// <param name="weeks">The weeks.</param>
        /// <returns>A <see cref="Week">Week enum</see>.</returns>
        [PublicAPI]
        public static Week Weeks([NotNull] this IEnumerable<int> weeks)
        {
            Contract.Requires(weeks != null);
            return (Week)weeks.Aggregate<int, ulong>(0, (current, d) => current | (ulong)1 << d);
        }
        #endregion

        #region WeekDays
        /// <summary>
        /// Converts a <see cref="Month">WeekDay enum</see> into a enumeration of <see cref="DayOfWeek"/>.
        /// </summary>
        /// <param name="weekDay">The week day.</param>
        /// <returns>An enumeration of <see cref="DayOfWeek"/>.</returns>
        [NotNull]
        [PublicAPI]
        public static IEnumerable<IsoDayOfWeek> WeekDays(this WeekDay weekDay)
        {
            Contract.Ensures(Contract.Result<IEnumerable<IsoDayOfWeek>>() != null);
            List<IsoDayOfWeek> valid = new List<IsoDayOfWeek>();
            if (WeekDay.Sunday ==
                (weekDay & WeekDay.Sunday))
                valid.Add(IsoDayOfWeek.Sunday);
            if (WeekDay.Monday ==
                (weekDay & WeekDay.Monday))
                valid.Add(IsoDayOfWeek.Monday);
            if (WeekDay.Tuesday ==
                (weekDay & WeekDay.Tuesday))
                valid.Add(IsoDayOfWeek.Tuesday);
            if (WeekDay.Wednesday ==
                (weekDay & WeekDay.Wednesday))
                valid.Add(IsoDayOfWeek.Wednesday);
            if (WeekDay.Thursday ==
                (weekDay & WeekDay.Thursday))
                valid.Add(IsoDayOfWeek.Thursday);
            if (WeekDay.Friday ==
                (weekDay & WeekDay.Friday))
                valid.Add(IsoDayOfWeek.Friday);
            if (WeekDay.Saturday ==
                (weekDay & WeekDay.Saturday))
                valid.Add(IsoDayOfWeek.Saturday);
            return valid;
        }

        /// <summary>
        /// Converts an array of <see cref="DayOfWeek"/> into a <see cref="WeekDay">WeekDay enum</see>.
        /// </summary>
        /// <param name="daysOfWeek">The weekDays.</param>
        /// <returns>A <see cref="WeekDay">WeekDay enum</see>.</returns>
        [PublicAPI]
        public static WeekDay WeekDays([NotNull] params IsoDayOfWeek[] daysOfWeek)
        {
            Contract.Requires(daysOfWeek != null);
            return daysOfWeek.WeekDays();
        }

        /// <summary>
        /// Converts an enumeration of <see cref="DayOfWeek"/> into a <see cref="WeekDay">WeekDay enum</see>.
        /// </summary>
        /// <param name="daysOfWeek">The weekDays.</param>
        /// <returns>A <see cref="WeekDay">WeekDay enum</see>.</returns>
        [PublicAPI]
        public static WeekDay WeekDays([NotNull] this IEnumerable<IsoDayOfWeek> daysOfWeek)
        {
            Contract.Requires(daysOfWeek != null);
            WeekDay weekDay = WeekDay.Never;
            foreach (IsoDayOfWeek dayOfWeek in daysOfWeek)
            {
                switch (dayOfWeek)
                {
                    case IsoDayOfWeek.Sunday:
                        weekDay |= WeekDay.Sunday;
                        break;
                    case IsoDayOfWeek.Monday:
                        weekDay |= WeekDay.Monday;
                        break;
                    case IsoDayOfWeek.Tuesday:
                        weekDay |= WeekDay.Tuesday;
                        break;
                    case IsoDayOfWeek.Wednesday:
                        weekDay |= WeekDay.Wednesday;
                        break;
                    case IsoDayOfWeek.Thursday:
                        weekDay |= WeekDay.Thursday;
                        break;
                    case IsoDayOfWeek.Friday:
                        weekDay |= WeekDay.Friday;
                        break;
                    case IsoDayOfWeek.Saturday:
                        weekDay |= WeekDay.Saturday;
                        break;
                    default:
                        continue;
                }
            }
            return weekDay;
        }
        #endregion

        #region Hours
        /// <summary>
        /// Converts a <see cref="Month">Hour enum</see> into a enumeration of hour integers.
        /// </summary>
        /// <param name="hour">The hour.</param>
        /// <returns>An enumeration of hour integers.</returns>
        [NotNull]
        [PublicAPI]
        public static IEnumerable<int> Hours(this Hour hour)
        {
            Contract.Ensures(Contract.Result<IEnumerable<int>>() != null);
            // Cast to an integer
            ulong h = (ulong)hour;

            // Check bits
            List<int> valid = new List<int>();
            for (int i = 0; i < 24; i++)
            {
                ulong bit = (ulong)1 << i;
                if ((h & bit) == bit)
                    valid.Add(i);
            }
            return valid;
        }

        /// <summary>
        /// Converts an array of integers into a <see cref="Hour">Hour enum</see>.
        /// </summary>
        /// <param name="hours">The hours.</param>
        /// <returns>A <see cref="Hour">Hour enum</see>.</returns>
        [PublicAPI]
        public static Hour Hours([NotNull] params int[] hours)
        {
            Contract.Requires(hours != null);
            return hours.Hours();
        }

        /// <summary>
        /// Converts an enumeration of integers into a <see cref="Hour">Hour enum</see>.
        /// </summary>
        /// <param name="hours">The hours.</param>
        /// <returns>A <see cref="Hour">Hour enum</see>.</returns>
        [PublicAPI]
        public static Hour Hours([NotNull] this IEnumerable<int> hours)
        {
            Contract.Requires(hours != null);
            return (Hour)hours.Aggregate<int, ulong>(0, (current, h) => current | (ulong)1 << h);
        }
        #endregion

        #region Minutes
        /// <summary>
        /// Converts a <see cref="Month">Minute enum</see> into a enumeration of minute integers.
        /// </summary>
        /// <param name="minute">The minute.</param>
        /// <returns>An enumeration of minute integers.</returns>
        [NotNull]
        [PublicAPI]
        public static IEnumerable<int> Minutes(this Minute minute)
        {
            Contract.Ensures(Contract.Result<IEnumerable<int>>() != null);
            // Cast to an integer
            ulong m = (ulong)minute;

            // Check bits
            List<int> valid = new List<int>();
            for (int i = 0; i < 60; i++)
            {
                ulong bit = (ulong)1 << i;
                if ((m & bit) == bit)
                    valid.Add(i);
            }
            return valid;
        }

        /// <summary>
        /// Converts an array of integers into a <see cref="Minute">Minute enum</see>.
        /// </summary>
        /// <param name="minutes">The minutes.</param>
        /// <returns>A <see cref="Minute">Minute enum</see>.</returns>
        [PublicAPI]
        public static Minute Minutes([NotNull] params int[] minutes)
        {
            Contract.Requires(minutes != null);
            return minutes.Minutes();
        }

        /// <summary>
        /// Converts an enumeration of integers into a <see cref="Minute">Minute enum</see>.
        /// </summary>
        /// <param name="minutes">The minutes.</param>
        /// <returns>A <see cref="Minute">Minute enum</see>.</returns>
        [PublicAPI]
        public static Minute Minutes([NotNull] this IEnumerable<int> minutes)
        {
            Contract.Requires(minutes != null);
            return (Minute)minutes.Aggregate<int, ulong>(0, (current, m) => current | (ulong)1 << m);
        }
        #endregion

        #region Seconds
        /// <summary>
        /// Converts a <see cref="Month">Second enum</see> into a enumeration of second integers.
        /// </summary>
        /// <param name="second">The second.</param>
        /// <returns>An enumeration of second integers.</returns>
        [NotNull]
        [PublicAPI]
        public static IEnumerable<int> Seconds(this Second second)
        {
            Contract.Ensures(Contract.Result<IEnumerable<int>>() != null);
            // Cast to an integer
            ulong s = (ulong)second;

            // Check bits
            List<int> valid = new List<int>();
            for (int i = 0; i < 60; i++)
            {
                ulong bit = (ulong)1 << i;
                if ((s & bit) == bit)
                    valid.Add(i);
            }
            return valid;
        }

        /// <summary>
        /// Converts an array of integers into a <see cref="Second">Second enum</see>.
        /// </summary>
        /// <param name="seconds">The seconds.</param>
        /// <returns>A <see cref="Second">Second enum</see>.</returns>
        [PublicAPI]
        public static Second Seconds([NotNull] params int[] seconds)
        {
            Contract.Requires(seconds != null);
            return seconds.Seconds();
        }

        /// <summary>
        /// Converts an enumeration of integers into a <see cref="Second">Second enum</see>.
        /// </summary>
        /// <param name="seconds">The seconds.</param>
        /// <returns>A <see cref="Second">Second enum</see>.</returns>
        [PublicAPI]
        public static Second Seconds([NotNull] this IEnumerable<int> seconds)
        {
            Contract.Requires(seconds != null);
            return (Second)seconds.Aggregate<int, ulong>(0, (current, s) => current | (ulong)1 << s);
        }
        #endregion

        /// <summary>
        /// Floors the specified instant to the second.
        /// </summary>
        /// <param name="instant">The instant.</param>
        /// <returns>Instant.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [PublicAPI]
        public static Instant Floor(this Instant instant)
        {
            return new Instant((instant.Ticks / NodaConstants.TicksPerSecond) * NodaConstants.TicksPerSecond);
        }

        /// <summary>
        /// Ceilings the specified instant to the second.
        /// </summary>
        /// <param name="instant">The instant.</param>
        /// <returns>Instant.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [PublicAPI]
        public static Instant Ceiling(this Instant instant)
        {
            return
                new Instant(
                    ((instant.Ticks + NodaConstants.TicksPerSecond - 1) / NodaConstants.TicksPerSecond) *
                    NodaConstants.TicksPerSecond);
        }

        /// <summary>
        /// Gets the next valid second after the current <paramref name="instant" />.
        /// </summary>
        /// <param name="instant">The date time (fractions of a second are removed).</param>
        /// <param name="month">The month.</param>
        /// <param name="week">The week.</param>
        /// <param name="day">The day.</param>
        /// <param name="weekDay">The week day.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="calendarSystem">The calendar (defaults to ISO-8601 standard).</param>
        /// <param name="timeZone">The time zone (defaults to UTC).</param>
        /// <returns>The next valid date (or <see cref="DateTime.MaxValue" /> if none).</returns>
        [PublicAPI]
        public static Instant NextValid(
            this Instant instant,
            Month month = Month.Every,
            Week week = Week.Every,
            Day day = Day.Every,
            WeekDay weekDay = WeekDay.Every,
            Hour hour = Hour.Zeroth,
            Minute minute = Minute.Zeroth,
            Second second = Second.Zeroth,
            [CanBeNull] CalendarSystem calendarSystem = null,
            [CanBeNull] DateTimeZone timeZone = null)
        {
            Contract.Ensures(Contract.Result<Instant>() >= instant);

            // Never case, if any are set to never, we'll never get a valid date.
            if ((month == Month.Never) ||
                (week == Week.Never) ||
                (day == Day.Never) ||
                (weekDay == WeekDay.Never) ||
                (hour == Hour.Never) ||
                (minute == Minute.Never) ||
                (second == Second.Never))
                return Instant.MaxValue;

            if (calendarSystem == null)
                calendarSystem = CalendarSystem.Iso;
            if (timeZone == null)
                timeZone = DateTimeZone.Utc;
            Contract.Assert(calendarSystem != null);
            Contract.Assert(timeZone != null);

            // Move to next second.
            instant = Ceiling(instant);
            
            // Every second case.
            if ((month == Month.Every) &&
                (day == Day.Every) &&
                (weekDay == WeekDay.Every) &&
                (hour == Hour.Every) &&
                (minute == Minute.Every) &&
                (second == Second.Every) &&
                (week == Week.Every))
                return instant;

            // Get days and months.
            int[] days = Days(day).OrderBy(dy => dy).ToArray();
            int[] months = month.Months().ToArray();
            
            // Remove months where the first day isn't in the month.
            int firstDay = days.First();
            if (firstDay > 28)
            {
                // 2000 is a leap year, so February has 29 days.
                months = months.Where(mn => calendarSystem.GetDaysInMonth(2000, mn) >= firstDay).ToArray();
                if (months.Length < 1)
                    return Instant.MaxValue;
            }

            // Get zoned date time.
            ZonedDateTime zdt = new ZonedDateTime(instant, timeZone, calendarSystem);
            int y = zdt.Year;
            int m = zdt.Month;
            int d = zdt.Day;
            int h = zdt.Hour;
            int n = zdt.Minute;
            int s = zdt.Second;

            int[] weeks = week.Weeks().ToArray();

            IsoDayOfWeek[] weekDays = weekDay.WeekDays().ToArray();
            int[] hours = hour.Hours().OrderBy(i => i).ToArray();
            int[] minutes = minute.Minutes().OrderBy(i => i).ToArray();
            int[] seconds = second.Seconds().ToArray();

            do
            {
                foreach (int currentMonth in months)
                {
                    if (currentMonth < m)
                        continue;
                    if (currentMonth > m)
                    {
                        d = 1;
                        h = n = s = 0;
                    }
                    m = currentMonth;
                    foreach (int currentDay in days)
                    {
                        if (currentDay < d)
                            continue;
                        if (currentDay > d)
                            h = n = s = 0;
                        d = currentDay;

                        // Check day is valid for this month.
                        if (d > calendarSystem.GetDaysInMonth(y, m))
                            break;

                        // We have a potential day, check week and week day
                        zdt = timeZone.AtLeniently(new LocalDateTime(y, m, d, h, n, s, calendarSystem));
                        if ((week != Week.Every) &&
                            (!weeks.Contains(zdt.WeekOfWeekYear)))
                            continue;
                        if ((weekDay != WeekDay.Every) &&
                            (!weekDays.Contains(zdt.IsoDayOfWeek)))
                            continue;

                        // We have a date match, check time.
                        foreach (int currentHour in hours)
                        {
                            if (currentHour < h) continue;
                            if (currentHour > h)
                                n = s = 0;
                            h = currentHour;
                            foreach (int currentMinute in minutes)
                            {
                                if (currentMinute < n) continue;
                                if (currentMinute > n)
                                    s = 0;
                                n = currentMinute;
                                foreach (int currentSecond in seconds)
                                {
                                    if (currentSecond < s) continue;
                                    return timeZone.AtLeniently(new LocalDateTime(y, m, d, h, n, currentSecond, calendarSystem)).ToInstant();
                                }
                                n = s = 0;
                            }
                            h = n = s = 0;
                        }
                        d = 1;
                    }
                    d = 1;
                    h = n = s = 0;
                }
                y++;

                // Don't bother checking max year.
                if (y >= calendarSystem.MaxYear)
                    return Instant.MaxValue;

                // Start next year
                m = d = 1;
                h = n = s = 0;
            } while (true);
        }
    }
}