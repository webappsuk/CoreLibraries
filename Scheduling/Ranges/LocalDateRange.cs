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
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using NodaTime;
using WebApplications.Utilities.Ranges;

namespace WebApplications.Utilities.Scheduling.Ranges
{
    /// <summary>
    /// A range of <see cref="LocalDate"/>.
    /// </summary>
    [PublicAPI]
    public class LocalDateRange : Range<LocalDate, Period>, IFormattable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateRange"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public LocalDateRange(LocalDate start, LocalDate end)
            // ReSharper disable once AssignNullToNotNullAttribute
            : base(start, end, AutoStep(start, Period.Between(start, end)))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateRange"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="step">The step.</param>
        public LocalDateRange(LocalDate start, LocalDate end, [NotNull] Period step)
            : base(start, end, FixUnits(start, step))
        {
            Contract.Requires(step != null);
            Contract.Requires(!step.HasTimeComponent);
            Contract.Requires(step.IsPositive(start.AtMidnight()));
        }

        /// <summary>
        /// Given a delta automatically returns a sensible step size.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="delta">The delta.</param>
        /// <returns>
        /// Period.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        [NotNull]
        [PublicAPI]
        public static Period AutoStep(LocalDate start, [NotNull] Period delta)
        {
            Contract.Requires(delta != null);
            Contract.Ensures(Contract.Result<Period>() != null);
            // ReSharper disable once PossibleNullReferenceException
            Contract.Ensures(!Contract.Result<Period>().HasTimeComponent);

            // ReSharper disable once AssignNullToNotNullAttribute
            delta = FixUnits(start, delta);
            Contract.Assert(delta != null);

            if (delta.Days < 0)
                throw new ArgumentOutOfRangeException();

            long days = delta.Days < 10 ? 1 : delta.Days / 10;
            Contract.Assert(days > 0);

            // ReSharper disable once AssignNullToNotNullAttribute
            return Period.FromDays(days);
        }

        /// <summary>
        /// Fixes the units of a period so that they only contain fixed length fields.
        /// If the field contains months/years then the length of the period depends on the month/year.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="period">The period.</param>
        /// <returns></returns>
        [NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Period FixUnits(LocalDate start, [NotNull] Period period)
        {
            return Period.Between(start, start + period, PeriodUnits.Days);
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