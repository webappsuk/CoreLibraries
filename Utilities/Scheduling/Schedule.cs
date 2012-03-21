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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace WebApplications.Utilities.Scheduling
{
    /// <summary>
    /// Defines a schedule.
    /// </summary>
    public class Schedule : IEnumerable<DateTime>
    {
        /// <summary>
        /// The function that, given a <see cref="DateTime"/> get's the next scheduled event (or DateTime.MaxValue if no more).
        /// </summary>
        private readonly Func<DateTime, DateTime> _getNextSchedule;

        /// <summary>
        /// The minimum gap, added to all supplied date times.
        /// </summary>
        public readonly TimeSpan MinimumGap;

        /// <summary>
        /// Initializes a new instance of the <see cref="Schedule"/> class.
        /// </summary>
        /// <param name="month">The month.</param>
        /// <param name="week">The week.</param>
        /// <param name="day">The day.</param>
        /// <param name="weekDay">The week day.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="minimumGap">The minimum gap (added to all date times before calculating the next valid date time).</param>
        /// <param name="calendar">The calendar.</param>
        /// <param name="calendarWeekRule">The calendar week rule.</param>
        /// <param name="firstDayOfWeek">The first day of week.</param>
        public Schedule(
                Month month = Month.Every,
                Week week = Week.Every,
                Day day = Day.Every,
                WeekDay weekDay = WeekDay.Every,
                Hour hour = Hour.Zeroth,
                Minute minute = Minute.Zeroth,
                Second second = Second.Zeroth,
                TimeSpan minimumGap = default(TimeSpan),
                Calendar calendar = null,
                CalendarWeekRule calendarWeekRule = CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek firstDayOfWeek = DayOfWeek.Sunday)
        {
            // If minimum gap is negative, set to zero.
            if (minimumGap < TimeSpan.Zero)
                minimumGap = TimeSpan.Zero;

            MinimumGap = minimumGap;

            // Needed in case calendar is not provided
            if (calendar == null)
                calendar = CultureInfo.CurrentCulture.Calendar;

            // Every second case.
            if ((month == Month.Every) && (week == Week.Every) && (day == Day.Every) && (weekDay == WeekDay.Every) && (hour == Hour.Every) &&
                (minute == Minute.Every) &&
                (second == Second.Every))
            {
                this._getNextSchedule = minimumGap > TimeSpan.Zero
                                                ? (Func<DateTime, DateTime>)
                                                  (dt =>
                                                   dt < (DateTime.MaxValue - this.MinimumGap)
                                                           ? calendar.AddSeconds(new DateTime(dt.Year, dt.Month, dt.Day,
                                                               dt.Hour, dt.Minute, dt.Second), 1).Add(minimumGap)
                                                           : DateTime.MaxValue)
                                                : (dt => calendar.AddSeconds(new DateTime(dt.Year, dt.Month, dt.Day,
                                                    dt.Hour, dt.Minute, dt.Second), 1));
                return;
            }

            // Never case
            if ((month == Month.Never) || (week==Week.Never) || (day == Day.Never) || (weekDay == WeekDay.Never) || (hour == Hour.Never) ||
                (minute == Minute.Never) ||
                (second == Second.Never))
            {
                this._getNextSchedule = dt => DateTime.MaxValue;
                return;
            }

            // Use NextValid extension method
            this._getNextSchedule = minimumGap > TimeSpan.Zero
                                            ? (Func<DateTime, DateTime>)
                                              (dt =>
                                               dt < (DateTime.MaxValue - this.MinimumGap)
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

        /// <summary>
        /// Initializes a new instance of the <see cref="Schedule"/> class.
        /// </summary>
        /// <param name="getNextSchedule">The function that, given a <see cref="DateTime"/> get's the next scheduled event
        /// (or DateTime.MaxValue if no more).</param>
        public Schedule(Func<DateTime, DateTime> getNextSchedule)
        {
            this._getNextSchedule = getNextSchedule;
        }

        #region IEnumerable<DateTime> Members
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<DateTime> GetEnumerator()
        {
            DateTime next = DateTime.MinValue;
            while ((next = this.Next(next)) <
                   DateTime.MaxValue)
                yield return next;
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion

        /// <summary>
        /// Get's the next scheduled event.
        /// </summary>
        /// <param name="last">The last.</param>
        /// <returns>Next time in schedule.</returns>
        public DateTime Next(DateTime last)
        {
            try
            {
                return this._getNextSchedule(last);
            }
            catch
            {
                return DateTime.MaxValue;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "Schedule - Next " + this._getNextSchedule(DateTime.Now);
        }
    }
}