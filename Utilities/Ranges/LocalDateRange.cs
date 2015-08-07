#region © Copyright Web Applications (UK) Ltd, 2015.  All rights reserved.
// Copyright (c) 2015, Web Applications UK Ltd
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
using NodaTime;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Ranges
{
    /// <summary>
    /// A range of <see cref="LocalDate"/>.
    /// </summary>
    [PublicAPI]
    public class LocalDateRange : Range<LocalDate, Period>, IFormattable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateRange" /> class using the specified start date and duration.
        /// </summary>
        /// <param name="start">The start date.</param>
        /// <param name="days">The duration in days.</param>
        public LocalDateRange(LocalDate start, uint days)
            // ReSharper disable once AssignNullToNotNullAttribute
            : base(start, start + Period.FromDays(days), AutoStep(start, start + Period.FromDays(days)))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateRange" /> class using the specified start date and duration.
        /// </summary>
        /// <param name="start">The start date.</param>
        /// <param name="duration">The duration.</param>
        public LocalDateRange(LocalDate start, [NotNull] Period duration)
            // ReSharper disable once AssignNullToNotNullAttribute
            : base(start, start + duration, AutoStep(start, start + duration))
        {
            if (duration == null) throw new ArgumentNullException(nameof(duration));
            // ReSharper disable once PossibleNullReferenceException
            if (duration.Normalize().HasTimeComponent)
                throw new ArgumentOutOfRangeException(nameof(duration), Resources.LocalDateRange_DurationCannotHaveTime);
            if (!duration.IsPositive(start))
                throw new ArgumentOutOfRangeException(nameof(duration), Resources.LocalRange_DurationMustBePositive);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateRange"/> class using the specified start and end date.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public LocalDateRange(LocalDate start, LocalDate end)
            // ReSharper disable once AssignNullToNotNullAttribute
            : base(start, end, AutoStep(start, end))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateRange"/> class using the specified start date, end date and step.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="step">The step.</param>
        public LocalDateRange(LocalDate start, LocalDate end, [NotNull] Period step)
            : base(start, end, step)
        {
            if (step == null) throw new ArgumentNullException(nameof(step));
            // ReSharper disable once PossibleNullReferenceException
            if (step.Normalize().HasTimeComponent)
                throw new ArgumentOutOfRangeException(nameof(step), Resources.LocalDateRange_StepCannotHaveTime);
            if (!step.IsPositive(start))
                throw new ArgumentOutOfRangeException(nameof(step), Resources.LocalRange_StepMustBePositive);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateRange" /> class using the specified start date, duration and step.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="days">The duration in days.</param>
        /// <param name="step">The step.</param>
        public LocalDateRange(LocalDate start, uint days, [NotNull] Period step)
            : base(start, start + Period.FromDays(days), step)
        {
            if (step == null) throw new ArgumentNullException(nameof(step));
            // ReSharper disable once PossibleNullReferenceException
            if (step.Normalize().HasTimeComponent)
                throw new ArgumentOutOfRangeException(nameof(step), Resources.LocalDateRange_StepCannotHaveTime);
            if (!step.IsPositive(start))
                throw new ArgumentOutOfRangeException(nameof(step), Resources.LocalRange_StepMustBePositive);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateRange" /> class using the specified start date, duration and step.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="step">The step.</param>
        public LocalDateRange(LocalDate start, [NotNull] Period duration, [NotNull] Period step)
            : base(start, start + duration, step)
        {
            if (duration == null) throw new ArgumentNullException(nameof(duration));
            if (step == null) throw new ArgumentNullException(nameof(step));
            // ReSharper disable PossibleNullReferenceException
            if (step.Normalize().HasTimeComponent)
                throw new ArgumentOutOfRangeException(nameof(step), Resources.LocalDateRange_StepCannotHaveTime);
            if (duration.Normalize().HasTimeComponent)
                throw new ArgumentOutOfRangeException(nameof(duration), Resources.LocalDateRange_DurationCannotHaveTime);
            // ReSharper restore PossibleNullReferenceException
            if (!step.IsPositive(start))
                throw new ArgumentOutOfRangeException(nameof(step), Resources.LocalRange_StepMustBePositive);
            if (!duration.IsPositive(start))
                throw new ArgumentOutOfRangeException(nameof(duration), Resources.LocalRange_DurationMustBePositive);
        }

        /// <summary>
        /// Gets the number of days between the start date and the end date, inclusively.
        /// This will return 1 where the start and end date are equal.
        /// </summary>
        /// <remarks>
        /// This is always the same as <see cref="Nights"/> + 1.
        /// </remarks>
        public int Days
        {
            get
            {
                // ReSharper disable once PossibleNullReferenceException
                return (int)Period.Between(Start, End, PeriodUnits.Days).Days + 1;
            }
        }

        /// <summary>
        /// Gets the number of nights in the date range.
        /// Where the start and end date are equal this will return 0.
        /// </summary>
        /// <remarks>
        /// This is always the same as <see cref="Days"/> - 1.
        /// </remarks>
        public int Nights
        {
            get
            {
                // ReSharper disable once PossibleNullReferenceException
                return (int)Period.Between(Start, End, PeriodUnits.Days).Days;
            }
        }

        /// <summary>
        /// Given a start and end automatically returns a sensible step size.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>
        /// Period.
        /// </returns>
        [NotNull]
        public static Period AutoStep(LocalDate start, LocalDate end)
        {
            CheckStartGreaterThanEnd(start, end);

            // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute
            if (Period.Between(start, end, PeriodUnits.Months).Months < 1)
                return TimeHelpers.OneDayPeriod;
            if (Period.Between(start, end, PeriodUnits.Years).Years < 1)
                return TimeHelpers.OneMonthPeriod;
            return TimeHelpers.OneYearPeriod;
            // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Gets a <see cref="LocalDateTimeRange" /> at the time given on the dates represented by this local date range.
        /// </summary>
        /// <param name="time">The time.</param>
        [NotNull]
        public LocalDateTimeRange At(LocalTime time)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            return new LocalDateTimeRange(Start.At(time), End.At(time), Step);
        }

        /// <summary>
        /// Gets a <see cref="LocalDateTimeRange" /> at the times given on the dates represented by this local date range.
        /// </summary>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        [NotNull]
        public LocalDateTimeRange At(LocalTime startTime, LocalTime endTime)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            return new LocalDateTimeRange(Start.At(startTime), End.At(endTime), Step);
        }

        /// <summary>
        /// Gets a <see cref="LocalDateTimeRange" /> at midnight on the dates represented by this local date range.
        /// </summary>
        [NotNull]
        public LocalDateTimeRange AtMidnight()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            return new LocalDateTimeRange(Start.AtMidnight(), End.AtMidnight(), Step);
        }

        /// <summary>
        /// Converts this range to a <see cref="DateRange"/> with a <see cref="DateTime.Kind" /> of <see cref="DateTimeKind.Unspecified"/>.
        /// </summary>
        [NotNull]
        public DateRange ToDateRangeUnspecified()
        {
            return new DateRange(
                new DateTime(Start.Year, Start.Month, Start.Day, 0, 0, 0, DateTimeKind.Unspecified),
                new DateTime(End.Year, End.Month, End.Day, 0, 0, 0, DateTimeKind.Unspecified),
                // ReSharper disable once PossibleNullReferenceException
                checked((int)Period.Between(Start, Start + Step, PeriodUnits.Days).Days));
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{Start} - {End}";
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
            return $"{Start.ToString(format, formatProvider)} - {End.ToString(format, formatProvider)}";
        }
    }
}