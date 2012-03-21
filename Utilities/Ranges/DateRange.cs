#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities
// File: DateRange.cs
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

namespace WebApplications.Utilities.Ranges
{
    /// <summary>
    ///   A range of <see cref="DateTime">date</see> values with the time component ignored.
    /// </summary>
    public class DateRange : Range<DateTime, TimeSpan>
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="DateRange"/> class using the specified start date and duration.
        /// </summary>
        /// <param name="start">The start date (inclusive).</param>
        /// <param name="days">The duration in days.</param>
        public DateRange(DateTime start, uint days)
            : base(start.Date, start.Date.AddDays(days), TimeSpan.FromDays(1))
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="DateRange"/> class using the specified start date and end date.
        /// </summary>
        /// <param name="start">The start date (inclusive).</param>
        /// <param name="end">The end date (inclusive).</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///   The <paramref name="start"/> date was greater than the <paramref name="end"/> date.
        /// </exception>
        public DateRange(DateTime start, DateTime end)
            : base(start.Date, end.Date, TimeSpan.FromDays(1))
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="DateRange"/> class using the specified start date, end date and step.
        /// </summary>
        /// <param name="start">The start date (inclusive).</param>
        /// <param name="end">The end date (inclusive).</param>
        /// <param name="step">The step between each date in the range (in days).</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///   The <paramref name="start"/> date was greater than the <paramref name="end"/> date.
        /// </exception>
        public DateRange(DateTime start, DateTime end, int step)
            : base(start.Date, end.Date, TimeSpan.FromDays(step))
        {
        }

        /// <summary>
        ///   Gets the number of days between the start date and the end date, inclusively.
        ///   This will return 1 where the start and end date are equal.
        /// </summary>
        /// <remarks>
        ///   This is always the same as <see cref="Nights"/> + 1.
        /// </remarks>
        public int Days
        {
            get { return (int) (End.Date - Start.Date).TotalDays + 1; }
        }

        /// <summary>
        ///   Gets the number of nights in the date range.
        ///   Where the start and end date are equal this will return 0.
        /// </summary>
        /// <remarks>
        ///    This is always the same as <see cref="Days"/> - 1.
        /// </remarks>
        public int Nights
        {
            get { return (int) (End.Date - Start.Date).TotalDays; }
        }

        /// <summary>
        ///   Returns a <see cref="string"/> representation of this instance.
        /// </summary>
        /// <returns>
        ///   A <see cref="string"/> representation of this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0:dd/MM/yyyy} - {1:dd/MM/yyyy} [{2}]", Start, End,
                Days == 1
                    ? string.Format("{0} day", Days)
                    : string.Format("{0} days", Days)
                );
        }
    }
}