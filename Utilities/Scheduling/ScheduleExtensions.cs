#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
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
// ©  Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace WebApplications.Utilities.Scheduling
{
    /// <summary>
    /// Extension methods for scheduling
    /// </summary>
    public static class ScheduleExtensions
    {
        #region Months
        /// <summary>
        /// Converts a <see cref="Month">Month enum</see> into a enumeration of month integers.
        /// </summary>
        /// <param name="month">The month.</param>
        /// <returns>An enumeration of month integers.</returns>
        public static IEnumerable<int> Months(this Month month)
        {
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
        public static Month Months(params int[] months)
        {
            return months.Months();
        }

        /// <summary>
        /// Converts an enumeration of integers into a <see cref="Month">Month enum</see>.
        /// </summary>
        /// <param name="months">The months.</param>
        /// <returns>A <see cref="Month">Month enum</see>.</returns>
        public static Month Months(this IEnumerable<int> months)
        {
            return (Month)months.Aggregate<int, ulong>(0, (current, m) => current | (ulong)1 << m);
        }
        #endregion

        #region Days
        /// <summary>
        /// Converts a <see cref="Month">Day enum</see> into a enumeration of day integers.
        /// </summary>
        /// <param name="day">The day.</param>
        /// <returns>An enumeration of day integers.</returns>
        public static IEnumerable<int> Days(this Day day)
        {
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
        public static Day Days(params int[] days)
        {
            return days.Days();
        }

        /// <summary>
        /// Converts an enumeration of integers into a <see cref="Day">Day enum</see>.
        /// </summary>
        /// <param name="days">The days.</param>
        /// <returns>A <see cref="Day">Day enum</see>.</returns>
        public static Day Days(this IEnumerable<int> days)
        {
            return (Day)days.Aggregate<int, ulong>(0, (current, d) => current | (ulong)1 << d);
        }
        #endregion

        #region Weeks
        /// <summary>
        /// Converts a <see cref="Month">Week enum</see> into a enumeration of week integers.
        /// </summary>
        /// <param name="week">The week.</param>
        /// <returns>An enumeration of week integers.</returns>
        public static IEnumerable<int> Weeks(this Week week)
        {
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
        /// Converts an array of integers into a <see cref="WeekNumber">Week enum</see>.
        /// </summary>
        /// <param name="weeks">The weeks.</param>
        /// <returns>A <see cref="WeekNumber">Week enum</see>.</returns>
        public static Week Weeks(params int[] weeks)
        {
            return weeks.Weeks();
        }

        /// <summary>
        /// Converts an enumeration of integers into a <see cref="WeekNumber">Week enum</see>.
        /// </summary>
        /// <param name="weeks">The weeks.</param>
        /// <returns>A <see cref="WeekNumber">Week enum</see>.</returns>
        public static Week Weeks(this IEnumerable<int> weeks)
        {
            return (Week)weeks.Aggregate<int, ulong>(0, (current, d) => current | (ulong)1 << d);
        }
        #endregion

        #region WeekDays
        /// <summary>
        /// Converts a <see cref="Month">WeekDay enum</see> into a enumeration of <see cref="DayOfWeek"/>.
        /// </summary>
        /// <param name="weekDay">The week day.</param>
        /// <returns>An enumeration of <see cref="DayOfWeek"/>.</returns>
        public static IEnumerable<DayOfWeek> WeekDays(this WeekDay weekDay)
        {
            List<DayOfWeek> valid = new List<DayOfWeek>();
            if (WeekDay.Sunday ==
                (weekDay & WeekDay.Sunday))
                valid.Add(DayOfWeek.Sunday);
            if (WeekDay.Monday ==
                (weekDay & WeekDay.Monday))
                valid.Add(DayOfWeek.Monday);
            if (WeekDay.Tuesday ==
                (weekDay & WeekDay.Tuesday))
                valid.Add(DayOfWeek.Tuesday);
            if (WeekDay.Wednesday ==
                (weekDay & WeekDay.Wednesday))
                valid.Add(DayOfWeek.Wednesday);
            if (WeekDay.Thursday ==
                (weekDay & WeekDay.Thursday))
                valid.Add(DayOfWeek.Thursday);
            if (WeekDay.Friday ==
                (weekDay & WeekDay.Friday))
                valid.Add(DayOfWeek.Friday);
            if (WeekDay.Saturday ==
                (weekDay & WeekDay.Saturday))
                valid.Add(DayOfWeek.Saturday);
            return valid;
        }

        /// <summary>
        /// Converts an array of <see cref="DayOfWeek"/> into a <see cref="WeekDay">WeekDay enum</see>.
        /// </summary>
        /// <param name="daysOfWeek">The weekDays.</param>
        /// <returns>A <see cref="WeekDay">WeekDay enum</see>.</returns>
        public static WeekDay WeekDays(params DayOfWeek[] daysOfWeek)
        {
            return daysOfWeek.WeekDays();
        }

        /// <summary>
        /// Converts an enumeration of <see cref="DayOfWeek"/> into a <see cref="WeekDay">WeekDay enum</see>.
        /// </summary>
        /// <param name="daysOfWeek">The weekDays.</param>
        /// <returns>A <see cref="WeekDay">WeekDay enum</see>.</returns>
        public static WeekDay WeekDays(this IEnumerable<DayOfWeek> daysOfWeek)
        {
            WeekDay weekDay = WeekDay.Never;
            foreach (DayOfWeek dayOfWeek in daysOfWeek)
            {
                switch (dayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        weekDay |= WeekDay.Sunday;
                        break;
                    case DayOfWeek.Monday:
                        weekDay |= WeekDay.Monday;
                        break;
                    case DayOfWeek.Tuesday:
                        weekDay |= WeekDay.Tuesday;
                        break;
                    case DayOfWeek.Wednesday:
                        weekDay |= WeekDay.Wednesday;
                        break;
                    case DayOfWeek.Thursday:
                        weekDay |= WeekDay.Thursday;
                        break;
                    case DayOfWeek.Friday:
                        weekDay |= WeekDay.Friday;
                        break;
                    case DayOfWeek.Saturday:
                        weekDay |= WeekDay.Saturday;
                        break;
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
        public static IEnumerable<int> Hours(this Hour hour)
        {
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
        public static Hour Hours(params int[] hours)
        {
            return hours.Hours();
        }

        /// <summary>
        /// Converts an enumeration of integers into a <see cref="Hour">Hour enum</see>.
        /// </summary>
        /// <param name="hours">The hours.</param>
        /// <returns>A <see cref="Hour">Hour enum</see>.</returns>
        public static Hour Hours(this IEnumerable<int> hours)
        {
            return (Hour)hours.Aggregate<int, ulong>(0, (current, h) => current | (ulong)1 << h);
        }
        #endregion

        #region Minutes
        /// <summary>
        /// Converts a <see cref="Month">Minute enum</see> into a enumeration of minute integers.
        /// </summary>
        /// <param name="minute">The minute.</param>
        /// <returns>An enumeration of minute integers.</returns>
        public static IEnumerable<int> Minutes(this Minute minute)
        {
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
        public static Minute Minutes(params int[] minutes)
        {
            return minutes.Minutes();
        }

        /// <summary>
        /// Converts an enumeration of integers into a <see cref="Minute">Minute enum</see>.
        /// </summary>
        /// <param name="minutes">The minutes.</param>
        /// <returns>A <see cref="Minute">Minute enum</see>.</returns>
        public static Minute Minutes(this IEnumerable<int> minutes)
        {
            return (Minute)minutes.Aggregate<int, ulong>(0, (current, m) => current | (ulong)1 << m);
        }
        #endregion

        #region Seconds
        /// <summary>
        /// Converts a <see cref="Month">Second enum</see> into a enumeration of second integers.
        /// </summary>
        /// <param name="second">The second.</param>
        /// <returns>An enumeration of second integers.</returns>
        public static IEnumerable<int> Seconds(this Second second)
        {
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
        public static Second Seconds(params int[] seconds)
        {
            return seconds.Seconds();
        }

        /// <summary>
        /// Converts an enumeration of integers into a <see cref="Second">Second enum</see>.
        /// </summary>
        /// <param name="seconds">The seconds.</param>
        /// <returns>A <see cref="Second">Second enum</see>.</returns>
        public static Second Seconds(this IEnumerable<int> seconds)
        {
            return (Second)seconds.Aggregate<int, ulong>(0, (current, s) => current | (ulong)1 << s);
        }
        #endregion

        /// <summary>
        /// Get's this week for a date.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <param name="calendar">The calendar.</param>
        /// <param name="calendarWeekRule">The calendar week rule.</param>
        /// <param name="firstDayOfWeek">The first day of week.</param>
        /// <returns>The week.</returns>
        public static int WeekNumber(
                this DateTime dateTime,
                Calendar calendar = null,
                CalendarWeekRule calendarWeekRule = CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek firstDayOfWeek = DayOfWeek.Sunday)
        {
            if (calendar == null)
                calendar = CultureInfo.CurrentCulture.Calendar;
            int weekNum = calendar.GetWeekOfYear(dateTime, calendarWeekRule, firstDayOfWeek);
            return ((weekNum > 51) && (calendar.GetMonth(dateTime) < 12)) ? 0 : weekNum;
        }

        /// <summary>
        /// Gets the first day of a week.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="week">The week.</param>
        /// <param name="calendar">The calendar.</param>
        /// <param name="calendarWeekRule">The calendar week rule.</param>
        /// <param name="firstDayOfWeek">The first day of week.</param>
        /// <returns>The date of the first day in the week.</returns>
        public static DateTime GetFirstDayOfWeek(
                int year,
                int week,
                Calendar calendar = null,
                CalendarWeekRule calendarWeekRule = CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek firstDayOfWeek = DayOfWeek.Sunday)
        {
            if (calendar == null)
                calendar = CultureInfo.CurrentCulture.Calendar;

            DateTime firstDay = new DateTime(year, 1, 7, calendar);

            while (calendar.GetDayOfWeek(firstDay) != firstDayOfWeek || WeekNumber(firstDay, calendar, calendarWeekRule, firstDayOfWeek) % 52 > 1)
            {
                firstDay = calendar.AddDays(firstDay, -1);
            }

            // Add the week
            return (week != 1) ? calendar.AddWeeks(firstDay, week - 1) : firstDay;
        }

        /// <summary>
        /// Get's the next valid second after the current .
        /// </summary>
        /// <param name="dateTime">The date time (fractions of a second are removed).</param>
        /// <param name="month">The month.</param>
        /// <param name="week">The week.</param>
        /// <param name="day">The day.</param>
        /// <param name="weekDay">The week day.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="calendar">The calendar.</param>
        /// <param name="calendarWeekRule">The calendar week rule.</param>
        /// <param name="firstDayOfWeek">The first day of week.</param>
        /// <param name="inclusive">if set to <c>true</c> can return the time specified, otherwise, starts at the next second..</param>
        /// <returns>
        /// The next valid date (or <see cref="DateTime.MaxValue"/> if none).
        /// </returns>
        public static DateTime NextValid(
                this DateTime dateTime,
                Month month = Month.Every,
                Week week = Week.Every,
                Day day = Day.Every,
                WeekDay weekDay = WeekDay.Every,
                Hour hour = Hour.Zeroth,
                Minute minute = Minute.Zeroth,
                Second second = Second.Zeroth,
                Calendar calendar = null,
                CalendarWeekRule calendarWeekRule = CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek firstDayOfWeek = DayOfWeek.Sunday,
                bool inclusive = false)
        {
            // Never case, if any are set to never, we'll never get a valid date.
            if ((month == Month.Never) || (week == Week.Never) ||
                (day == Day.Never) || (weekDay == WeekDay.Never) || (hour == Hour.Never) ||
                (minute == Minute.Never) ||
                (second == Second.Never))
                return DateTime.MaxValue;

            if (calendar == null)
                calendar = CultureInfo.CurrentCulture.Calendar;

            // Set the time to this second (or the next one if not inclusive), remove fractions of a second.
            dateTime = new DateTime(
                    dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
            if (!inclusive)
                dateTime = calendar.AddSeconds(dateTime, 1);

            // Every second case.
            if ((month == Month.Every) && (day == Day.Every) && (weekDay == WeekDay.Every) && (hour == Hour.Every) &&
                (minute == Minute.Every) && (second == Second.Every) &&
                (week == Week.Every))
                return calendar.AddSeconds(dateTime, 1);

            // Get days and months.
            IEnumerable<int> days = day.Days().OrderBy(dy => dy);
            IEnumerable<int> months = month.Months();

            // Remove months where the first day isn't in the month.
            int firstDay = days.First();
            if (firstDay > 28)
            {
                // 2000 is a leap year, so February has 29 days.
                months = months.Where(mn => calendar.GetDaysInMonth(2000, mn) >= firstDay);
                if (months.Count() < 1)
                    return DateTime.MaxValue;
            }

            // Get remaining date components.
            int y = calendar.GetYear(dateTime);
            int m = calendar.GetMonth(dateTime);
            int d = calendar.GetDayOfMonth(dateTime);

            int h = calendar.GetHour(dateTime);
            int n = calendar.GetMinute(dateTime);
            int s = calendar.GetSecond(dateTime);

            IEnumerable<int> weeks = week.Weeks();
            IEnumerable<DayOfWeek> weekDays = weekDay.WeekDays();
            IEnumerable<int> hours = hour.Hours().OrderBy(i => i);
            IEnumerable<int> minutes = minute.Minutes().OrderBy(i => i);
            IEnumerable<int> seconds = second.Seconds();
            
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
                        if ((d > 28) && (d > calendar.GetDaysInMonth(y, m)))
                            break;

                        // We have a potential day, check week and week day
                        dateTime = new DateTime(y, m, d, h, n, s);
                        if ((week != Week.Every) &&
                            (!weeks.Contains(dateTime.WeekNumber(calendar, calendarWeekRule, firstDayOfWeek))))
                            continue;
                        if ((weekDay != WeekDay.Every) &&
                            (!weekDays.Contains(calendar.GetDayOfWeek(dateTime))))
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
                                    return new DateTime(y, m, d, h, n, currentSecond, calendar);
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
                if (y > 9998)
                    return DateTime.MaxValue;

                // Start next year
                m = d = 1;
                h = n = s = 0;
            } while (true);
        }
    }
}