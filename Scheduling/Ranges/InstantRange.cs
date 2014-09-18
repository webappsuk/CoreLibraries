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
    ///   A range of <see cref="NodaTime.Instant">Instant</see>s.
    /// </summary>
    [PublicAPI]
    public class InstantRange : Range<Instant, Duration>, IFormattable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstantRange"/> class using the specified start and end instants.
        /// </summary>
        /// <param name="start">The start instant.</param>
        /// <param name="end">The end instant.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///   The <paramref name="start"/> instant was greater than the <paramref name="end"/> instant.
        /// </exception>
        /// <remarks>The step size is 00:00:01.</remarks>
        public InstantRange(Instant start, Instant end)
            : base(start, end, DurationRange.AutoStep(end - start))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstantRange" /> class using the specified start instant and duration.
        /// </summary>
        /// <param name="start">The start instant.</param>
        /// <param name="duration">The duration.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">The <paramref name="duration" /> was negative.</exception>
        /// <remarks>
        /// The step size is 00:00:01.
        /// </remarks>
        public InstantRange(Instant start, Duration duration)
            : base(start, start + duration, DurationRange.AutoStep(duration))
        {
            Contract.Requires<ArgumentOutOfRangeException>(duration >= Duration.Zero);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="InstantRange"/> class using the specified start instant,
        ///   end instant and step size.
        /// </summary>
        /// <param name="start">The start instant.</param>
        /// <param name="end">The end instant.</param>
        /// <param name="step">The step between each instant in the range.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">The <paramref name="start"/> was after the <see cref="DateTime">date</see> specified for <paramref name="end"/>.
        /// </exception>
        public InstantRange(Instant start, Instant end, Duration step)
            : base(start, end, step)
        {
        }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="InstantRange" /> class using the specified start instant,
        /// end instant and step size.
        /// </summary>
        /// <param name="start">The start instant.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="step">The step between each instant in the range.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">The <paramref name="duration" /> was negative.</exception>
        public InstantRange(Instant start, Duration duration, Duration step)
            : base(start, start + duration, step)
        {
            Contract.Requires<ArgumentOutOfRangeException>(duration >= Duration.Zero);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstantRange"/> class using the specified zoned date time range.
        /// </summary>
        /// <param name="zonedDateTimeRange">The zoned date time range.</param>
        public InstantRange([NotNull] ZonedDateTimeRange zonedDateTimeRange)
            : base(zonedDateTimeRange.Start.ToInstant(), zonedDateTimeRange.End.ToInstant(), zonedDateTimeRange.Step)
        {
        }

        /// <summary>
        /// Returns a <see cref="String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="String"/> representation of this instance.
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