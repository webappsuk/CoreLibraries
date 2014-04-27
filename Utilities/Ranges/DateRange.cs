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
            return string.Format(
                "{0:dd/MM/yyyy} - {1:dd/MM/yyyy} [{2}]",
                Start,
                End,
                Days == 1
                    ? string.Format("{0} day", Days)
                    : string.Format("{0} days", Days)
                );
        }
    }
}