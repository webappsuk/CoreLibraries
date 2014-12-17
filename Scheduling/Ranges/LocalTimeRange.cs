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
using WebApplications.Utilities.Annotations;
using NodaTime;
using WebApplications.Utilities.Ranges;

namespace WebApplications.Utilities.Scheduling.Ranges
{
    /// <summary>
    /// A range of <see cref="LocalTime"/>.
    /// </summary>
    [PublicAPI]
    public class LocalTimeRange : Range<LocalTime, Period>, IFormattable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalTimeRange"/> class using the specified start and end times.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public LocalTimeRange(LocalTime start, LocalTime end)
            // ReSharper disable once AssignNullToNotNullAttribute
            : base(start, end, AutoStep(start, end))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalTimeRange" /> class using the specified start time and duration.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="duration">The duration.</param>
        public LocalTimeRange(LocalTime start, [NotNull] Period duration)
            // ReSharper disable once AssignNullToNotNullAttribute
            : base(start, start + duration, AutoStep(start, start + duration))
        {
            Contract.Requires<ArgumentNullException>(duration != null);
            // ReSharper disable once PossibleNullReferenceException
            Contract.Requires<ArgumentOutOfRangeException>(!duration.Normalize().HasDateComponent);
            Contract.Requires<ArgumentOutOfRangeException>(duration.IsPositive(start.LocalDateTime));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalTimeRange"/> class using the specified start time, end time and step.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="step">The step.</param>
        public LocalTimeRange(LocalTime start, LocalTime end, [NotNull] Period step)
            : base(start, end, step)
        {
            Contract.Requires<ArgumentNullException>(step != null);
            // ReSharper disable once PossibleNullReferenceException
            Contract.Requires<ArgumentOutOfRangeException>(!step.Normalize().HasDateComponent);
            Contract.Requires<ArgumentOutOfRangeException>(step.IsPositive(start.LocalDateTime));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalTimeRange" /> class using the specified start time, duration and step.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="step">The step.</param>
        public LocalTimeRange(LocalTime start, [NotNull] Period duration, [NotNull] Period step)
            : base(start, start + duration, step)
        {
            Contract.Requires<ArgumentNullException>(step != null);
            Contract.Requires<ArgumentNullException>(duration != null);
            // ReSharper disable PossibleNullReferenceException
            Contract.Requires<ArgumentOutOfRangeException>(!step.Normalize().HasDateComponent);
            Contract.Requires<ArgumentOutOfRangeException>(!duration.Normalize().HasDateComponent);
            // ReSharper restore PossibleNullReferenceException
            Contract.Requires<ArgumentOutOfRangeException>(step.IsPositive(start.LocalDateTime));
            Contract.Requires<ArgumentOutOfRangeException>(duration.IsPositive(start.LocalDateTime));
        }

        /// <summary>
        /// Given a start and end automatically returns a sensible step size.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>
        /// Period.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        [NotNull]
        [PublicAPI]
        public static Period AutoStep(LocalTime start, LocalTime end)
        {
            Contract.Requires<ArgumentOutOfRangeException>(start < end);
            Contract.Ensures(Contract.Result<Period>() != null);
            // ReSharper disable once PossibleNullReferenceException
            Contract.Ensures(!Contract.Result<Period>().HasDateComponent);

            // ReSharper disable once PossibleNullReferenceException
            Period delta = Period.Between(start, end).Normalize();
            Contract.Assert(delta != null);
            Contract.Assert(!delta.HasDateComponent);

            // ReSharper disable AssignNullToNotNullAttribute
            if (delta.Hours > 0)
                return Period.FromHours(1);
            if (delta.Minutes > 0)
                return Period.FromMinutes(1);
            if (delta.Seconds > 0)
                return Period.FromSeconds(1);
            if (delta.Milliseconds > 0)
                return Period.FromMilliseconds(1);
            return Period.FromTicks(1);
            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Gets a <see cref="LocalDateTimeRange"/> with these local times, on January 1st 1970 in the ISO calendar.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public LocalDateTimeRange LocalDateTimeRange
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            get { return new LocalDateTimeRange(Start.LocalDateTime, End.LocalDateTime, Step); }
        }

        /// <summary>
        /// Gets a <see cref="LocalDateTimeRange"/> from this local time range on the date given.
        /// </summary>
        /// <param name="date">The date to combine with the times in the range.</param>
        [NotNull]
        [PublicAPI]
        public LocalDateTimeRange On(LocalDate date)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            return new LocalDateTimeRange(Start.On(date), End.On(date), Step);
        }

        /// <summary>
        /// Gets a <see cref="LocalDateTimeRange" /> from this local time range on the date given.
        /// </summary>
        /// <param name="startDate">The date to combine with the start time.</param>
        /// <param name="endDate">The date to combine with the end time.</param>
        [NotNull]
        [PublicAPI]
        public LocalDateTimeRange On(LocalDate startDate, LocalDate endDate)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            return new LocalDateTimeRange(Start.On(startDate), End.On(endDate), Step);
        }

        /// <summary>
        /// Converts this range to a <see cref="TimeSpanRange"/>.
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public TimeSpanRange ToTimeSpanRange()
        {
            // ReSharper disable once PossibleNullReferenceException
            return new TimeSpanRange(
                new TimeSpan(Start.TickOfDay),
                new TimeSpan(End.TickOfDay),
                Step.ToDuration().ToTimeSpan());
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
        public string ToString(string format, IFormatProvider formatProvider = null)
        {
            return String.Format(
                "{0} - {1}",
                Start.ToString(format, formatProvider),
                End.ToString(format, formatProvider));
        }
    }
}