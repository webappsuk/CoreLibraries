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
using WebApplications.Utilities.Ranges;

namespace WebApplications.Utilities.Scheduling.Ranges
{
    /// <summary>
    ///   A range of <see cref="NodaTime.ZonedDateTime">ZonedDateTime</see>s.
    /// </summary>
    public class ZonedDateTimeRange : Range<ZonedDateTime, Duration>, IFormattable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ZonedDateTimeRange" /> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public ZonedDateTimeRange(ZonedDateTime start, ZonedDateTime end)
            : base(start, end, DurationRange.AutoStep(end.ToInstant() - start.ToInstant()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZonedDateTimeRange" /> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="step">The step.</param>
        public ZonedDateTimeRange(ZonedDateTime start, ZonedDateTime end, Duration step)
            : base(start, end, step)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZonedDateTimeRange"/> class.
        /// </summary>
        /// <param name="instantRange">The instant range.</param>
        /// <param name="dateTimeZone">The date time zone.</param>
        public ZonedDateTimeRange([NotNull]InstantRange instantRange, DateTimeZone dateTimeZone)
            : base(new ZonedDateTime(instantRange.Start, dateTimeZone), new ZonedDateTime(instantRange.End, dateTimeZone), instantRange.Step)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZonedDateTimeRange"/> class.
        /// </summary>
        /// <param name="instantRange">The instant range.</param>
        /// <param name="startDateTimeZone">The start date time zone.</param>
        /// <param name="endDateTimeZone">The end date time zone.</param>
        public ZonedDateTimeRange([NotNull]InstantRange instantRange, DateTimeZone startDateTimeZone, DateTimeZone endDateTimeZone)
            : base(new ZonedDateTime(instantRange.Start, startDateTimeZone), new ZonedDateTime(instantRange.End, endDateTimeZone))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZonedDateTimeRange"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="dateTimeZone">The date time zone.</param>
        public ZonedDateTimeRange(Instant start, Instant end, DateTimeZone dateTimeZone)
            : base(new ZonedDateTime(start, dateTimeZone), new ZonedDateTime(end, dateTimeZone))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZonedDateTimeRange"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="dateTimeZone">The date time zone.</param>
        /// <param name="step">The step.</param>
        public ZonedDateTimeRange(Instant start, Instant end, DateTimeZone dateTimeZone, Duration step)
            : base(new ZonedDateTime(start, dateTimeZone), new ZonedDateTime(end, dateTimeZone), step)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZonedDateTimeRange"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="startDateTimeZone">The start date time zone.</param>
        /// <param name="end">The end.</param>
        /// <param name="endDateTimeZone">The end date time zone.</param>
        public ZonedDateTimeRange(Instant start, DateTimeZone startDateTimeZone, Instant end, DateTimeZone endDateTimeZone)
            : base(new ZonedDateTime(start, startDateTimeZone), new ZonedDateTime(end, endDateTimeZone))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZonedDateTimeRange"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="startDateTimeZone">The start date time zone.</param>
        /// <param name="end">The end.</param>
        /// <param name="endDateTimeZone">The end date time zone.</param>
        /// <param name="step">The step.</param>
        public ZonedDateTimeRange(Instant start, DateTimeZone startDateTimeZone, Instant end, DateTimeZone endDateTimeZone, Duration step)
            : base(new ZonedDateTime(start, startDateTimeZone), new ZonedDateTime(end, endDateTimeZone), step)
        {
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="ZonedDateTimeRange"/> to <see cref="InstantRange"/>.
        /// </summary>
        /// <param name="zonedDateTimeRange">The zoned date time range.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator InstantRange(ZonedDateTimeRange zonedDateTimeRange)
        {
            return new InstantRange(zonedDateTimeRange);
        }


        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return String.Format("{0} - {1}", Start, End);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        [StringFormatMethod("format")]
        public string ToString(string format, IFormatProvider formatProvider = null)
        {
            return String.Format(
                "{0} - {1}",
                Start.ToString(format, formatProvider),
                End.ToString(format, formatProvider));
        }
    }
}