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
    /// A range of <see cref="LocalTime"/>.
    /// </summary>
    [PublicAPI]
    public class LocalTimeRange : Range<LocalTime, Period>, IFormattable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalTimeRange"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public LocalTimeRange(LocalTime start, LocalTime end)
            // ReSharper disable once AssignNullToNotNullAttribute
            : base(start, end, AutoStep(Period.Between(start, end)))
        {
        }

        /// <summary>
        /// Given a delta automatically returns a sensible step size.
        /// </summary>
        /// <param name="delta">The delta.</param>
        /// <returns>Period.</returns>
        [NotNull]
        [PublicAPI]
        public static Period AutoStep([NotNull] Period delta)
        {
            Contract.Requires(delta != null);
            Contract.Ensures(Contract.Result<Period>() != null);
            // ReSharper disable once PossibleNullReferenceException
            Contract.Ensures(!Contract.Result<Period>().HasDateComponent);

            // ReSharper disable once AssignNullToNotNullAttribute
            delta = delta.Normalize();
            Contract.Assert(delta != null);

            if (delta.Months < 0 ||
                delta.Years < 0 ||
                delta.Weeks < 0 ||
                delta.Days < 0 ||
                delta.Hours < 0 ||
                delta.Minutes < 0 ||
                delta.Seconds < 0 ||
                delta.Milliseconds < 0 ||
                delta.Ticks < 0)
                throw new ArgumentOutOfRangeException();

            // ReSharper disable AssignNullToNotNullAttribute
            if (delta.Months > 0 ||
                delta.Years > 0 ||
                delta.Weeks > 0 ||
                delta.Days > 0 ||
                delta.Hours > 0)
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
        /// Initializes a new instance of the <see cref="LocalTimeRange"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="step">The step.</param>
        public LocalTimeRange(LocalTime start, LocalTime end, [NotNull] Period step)
            : base(start, end, step)
        {
            Contract.Requires(step != null);
            Contract.Requires(!step.HasDateComponent);
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