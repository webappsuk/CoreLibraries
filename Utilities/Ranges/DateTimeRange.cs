#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities
// File: DateTimeRange.cs
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
    ///   A range of <see cref="System.DateTime">DateTime</see>s.
    /// </summary>
    public class DateTimeRange : Range<DateTime, TimeSpan>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeRange"/> class using the specified start date and end date.
        /// </summary>
        /// <param name="start">The start date (inclusive).</param>
        /// <param name="end">The end date (inclusive).</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///   The <paramref name="start"/> date was greater than the <paramref name="end"/> date.
        /// </exception>
        /// <remarks>The step size is 00:00:01.</remarks>
        public DateTimeRange(DateTime start, DateTime end)
            : base(start, end, TimeSpan.FromSeconds(1))
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="DateTimeRange"/> class using the specified start date,
        ///   end date and step size.
        /// </summary>
        /// <param name="start">The start date (inclusive).</param>
        /// <param name="end">The end date (inclusive).</param>
        /// <param name="step">The step between each date in the range (in days).</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///   The <paramref name="start"/> was after the <see cref="DateTime">date</see> specified for
        /// <paramref name="end"/>.
        /// </exception>
        public DateTimeRange(DateTime start, DateTime end, TimeSpan step)
            : base(start, end, step)
        {
        }

        /// <summary>
        ///   Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        ///   A <see cref="string"/> representation of this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} - {1}", Start, End);
        }
    }
}