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

using WebApplications.Utilities.Annotations;
using NodaTime;

namespace WebApplications.Utilities.Scheduling.Test.Ranges
{
    public static class LocalHelpers
    {
        /// <summary>
        /// Gets the number of ticks a between a start and end date and time.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns></returns>
        public static long TicksTo(this LocalDateTime start, LocalDateTime end)
        {
            return Period.Between(start, end, PeriodUnits.Ticks).Ticks;
        }

        /// <summary>
        /// Gets the number of ticks a between a start and end date.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns></returns>
        public static long TicksTo(this LocalDate start, LocalDate end)
        {
            return Period.Between(start.AtMidnight(), end.AtMidnight(), PeriodUnits.Ticks).Ticks;
        }

        /// <summary>
        /// Gets the number of ticks a between a start and end time.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns></returns>
        public static long TicksTo(this LocalTime start, LocalTime end)
        {
            return Period.Between(start, end, PeriodUnits.Ticks).Ticks;
        }

        /// <summary>
        /// Gets the number of ticks a period represents based on a given start date and time.
        /// </summary>
        /// <param name="period">The period.</param>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public static long TicksFrom([NotNull] this Period period, LocalDateTime start)
        {
            return Period.Between(start, start + period, PeriodUnits.Ticks).Ticks;
        }

        /// <summary>
        /// Gets the number of ticks a period represents based on a given start date.
        /// </summary>
        /// <param name="period">The period.</param>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public static long TicksFrom([NotNull] this Period period, LocalDate start)
        {
            return Period.Between(start.AtMidnight(), start.AtMidnight() + period, PeriodUnits.Ticks).Ticks;
        }

        /// <summary>
        /// Gets the number of ticks a period represents based on a given start time.
        /// </summary>
        /// <param name="period">The period.</param>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public static long TicksFrom([NotNull] this Period period, LocalTime start)
        {
            return Period.Between(start, start + period, PeriodUnits.Ticks).Ticks;
        }
    }
}