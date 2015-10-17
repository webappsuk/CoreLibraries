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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using System;
using WebApplications.Testing;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Test.Ranges
{
    public class LocalRangeTestsBase
    {
        [NotNull]
        protected static readonly Random Random = Tester.RandomGenerator;

        public const int MonthsPerYear = 12;
        public const int WeeksPerMonth = 4;

        /// <summary>
        /// Choose a random date and time within a given range.
        /// </summary>
        /// <param name="minimum">The minimum date possible.</param>
        /// <param name="maximum">The maximum date possible.</param>
        /// <returns>
        /// A <see cref="T:System.LocalDateTime"/> between <paramref name="minimum"/> and <paramref name="maximum"/>.
        /// </returns>
        protected static LocalDateTime RandomLocalDateTime(LocalDateTime minimum, LocalDateTime maximum)
        {
            return minimum + RandomPeriod(null, Period.Between(minimum, maximum));
        }

        /// <summary>
        /// Choose a random time within a given range.
        /// </summary>
        /// <param name="minimum">The minimum time possible.</param>
        /// <param name="maximum">The maximum time possible.</param>
        /// <returns>
        /// A <see cref="T:System.LocalTime"/> between <paramref name="minimum"/> and <paramref name="maximum"/>.
        /// </returns>
        protected static LocalTime RandomLocalTime(LocalTime minimum, LocalTime maximum)
        {
            return minimum + RandomPeriod(null, Period.Between(minimum, maximum), false);
        }

        /// <summary>
        /// Choose a random date within a given range.
        /// </summary>
        /// <param name="minimum">The minimum date possible.</param>
        /// <param name="maximum">The maximum date possible.</param>
        /// <returns>
        /// A <see cref="T:System.LocalDate"/> between <paramref name="minimum"/> and <paramref name="maximum"/>.
        /// </returns>
        protected static LocalDate RandomLocalDate(LocalDate minimum, LocalDate maximum)
        {
            return minimum + RandomPeriod(null, Period.Between(minimum, maximum), true, false);
        }

        /// <summary>
        /// Choose a random time span of more than zero and less than one day.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Period"/> which is greater than zero but less than one day.
        /// </returns>
        [NotNull]
        protected static Period RandomTimeOffset()
        {
            return RandomPeriod(null, Period.FromDays(1), false);
        }

        /// <summary>
        /// Gets a random period within a given range.
        /// </summary>
        /// <param name="minimum">The minumum Period. If null, min will be 0.</param>
        /// <param name="maximum">The maximum Period.</param>
        /// <param name="hasDate">if set to <see langword="true" /> the returned period will have a date component.</param>
        /// <param name="hasTime">if set to <see langword="true" /> the returned period will have a time component.</param>
        /// <returns>
        /// A <see cref="T:System.Period" /> between <paramref name="minimum" /> and <paramref name="maximum" />.
        /// </returns>
        [NotNull]
        protected static Period RandomPeriod(
            [CanBeNull] Period minimum,
            [NotNull] Period maximum,
            bool hasDate = true,
            bool hasTime = true)
        {
            if (minimum != null)
                minimum = minimum.Normalize();
            // ReSharper disable once AssignNullToNotNullAttribute
            maximum = maximum.Normalize();

            // ReSharper disable once PossibleNullReferenceException
            Period diff = minimum == null ? maximum : (maximum - minimum).Normalize();
            PeriodBuilder builder = new PeriodBuilder();

            Assert.IsNotNull(diff);

            bool first = true;

            if (hasDate)
            {
                if (diff.Years != 0)
                {
                    builder.Years = (long)(diff.Years * Random.NextDouble());
                    first = false;
                }

                if (first && diff.Months != 0)
                {
                    builder.Months = (long)(diff.Months * Random.NextDouble());
                    first = false;
                }
                else if (!first)
                    builder.Months = Random.Next(MonthsPerYear);

                if (first && diff.Weeks != 0)
                {
                    builder.Weeks = (long)(diff.Weeks * Random.NextDouble());
                    first = false;
                }
                else if (!first)
                    builder.Weeks = Random.Next(WeeksPerMonth);

                if (first && diff.Days != 0)
                {
                    builder.Days = (long)(diff.Days * Random.NextDouble());
                    first = false;
                }
                else if (!first)
                    builder.Days = Random.Next(NodaConstants.DaysPerStandardWeek);

                if (minimum != null)
                {
                    builder.Years += minimum.Years;
                    builder.Months += minimum.Months;
                    builder.Weeks += minimum.Weeks;
                    builder.Days += minimum.Days;
                }
            }
            else
                first = !diff.HasDateComponent;

            if (hasTime)
            {
                if (first && diff.Hours != 0)
                {
                    builder.Hours = (long)(diff.Hours * Random.NextDouble());
                    first = false;
                }
                else if (!first)
                    builder.Hours = Random.Next(NodaConstants.HoursPerStandardDay);

                if (first && diff.Minutes != 0)
                {
                    builder.Minutes = (long)(diff.Minutes * Random.NextDouble());
                    first = false;
                }
                else if (!first)
                    builder.Minutes = Random.Next(NodaConstants.MinutesPerHour);

                if (first && diff.Seconds != 0)
                {
                    builder.Seconds = (long)(diff.Seconds * Random.NextDouble());
                    first = false;
                }
                else if (!first)
                    builder.Seconds = Random.Next(NodaConstants.SecondsPerMinute);

                if (first && diff.Milliseconds != 0)
                {
                    builder.Milliseconds = (long)(diff.Milliseconds * Random.NextDouble());
                    first = false;
                }
                else if (!first)
                    builder.Milliseconds = Random.Next(NodaConstants.MillisecondsPerSecond);

                if (first && diff.Ticks != 0)
                {
                    builder.Ticks = (long)(diff.Ticks * Random.NextDouble());
                    first = false;
                }
                else if (!first)
                    builder.Ticks = Random.Next((int)NodaConstants.TicksPerMillisecond);

                if (minimum != null)
                {
                    builder.Hours += minimum.Hours;
                    builder.Minutes += minimum.Minutes;
                    builder.Seconds += minimum.Seconds;
                    builder.Milliseconds += minimum.Milliseconds;
                    builder.Ticks += minimum.Ticks;
                }
            }

            return builder.Build().Normalize();
        }

        /// <summary>
        /// Gets the approximate result of dividing a <paramref name="period" /> by a number.
        /// </summary>
        /// <param name="period">The period.</param>
        /// <param name="divisor">The divisor.</param>
        /// <param name="hasTime">if set to <c>true</c> <paramref name="period"/> includes a time element (hours/minutes/seconds).</param>
        /// <returns>Period.</returns>
        [NotNull]
        protected static Period PeriodDivideApprox([NotNull] Period period, int divisor, bool hasTime = true)
        {
            PeriodBuilder builder = period.Normalize().ToBuilder();

            if (!hasTime)
            {
                builder.Hours = 0;
                builder.Minutes = 0;
                builder.Seconds = 0;
                builder.Milliseconds = 0;
                builder.Ticks = 0;
            }

            if (builder.Years != 0)
            {
                if (builder.Years > divisor)
                {
                    builder.Years /= divisor;
                    return builder.Build().Normalize();
                }

                builder.Months += builder.Years * MonthsPerYear;
                builder.Years = 0;
            }

            if (builder.Months != 0)
            {
                if (builder.Months > divisor)
                {
                    builder.Months /= divisor;
                    return builder.Build().Normalize();
                }

                builder.Weeks += builder.Months * WeeksPerMonth;
                builder.Months = 0;
            }

            if (builder.Weeks != 0)
            {
                if (builder.Weeks > divisor)
                {
                    builder.Weeks /= divisor;
                    return builder.Build().Normalize();
                }

                builder.Days += builder.Weeks * NodaConstants.DaysPerStandardWeek;
                builder.Weeks = 0;
            }

            if (builder.Days != 0)
            {
                if (builder.Days > divisor)
                {
                    builder.Days /= divisor;
                    return builder.Build().Normalize();
                }

                builder.Hours += builder.Days * NodaConstants.HoursPerStandardDay;
                builder.Days = 0;
            }

            if (!hasTime)
            {
                builder.Days = 1;
                builder.Hours = 0;

                return builder.Build().Normalize();
            }

            if (builder.Hours != 0)
            {
                if (builder.Hours > divisor)
                {
                    builder.Hours /= divisor;
                    return builder.Build().Normalize();
                }

                builder.Minutes += builder.Hours * NodaConstants.MinutesPerHour;
                builder.Hours = 0;
            }

            if (builder.Minutes != 0)
            {
                if (builder.Minutes > divisor)
                {
                    builder.Minutes /= divisor;
                    return builder.Build().Normalize();
                }

                builder.Seconds += builder.Minutes * NodaConstants.SecondsPerMinute;
                builder.Minutes = 0;
            }

            if (builder.Seconds != 0)
            {
                if (builder.Seconds > divisor)
                {
                    builder.Seconds /= divisor;
                    return builder.Build().Normalize();
                }

                builder.Milliseconds += builder.Seconds * NodaConstants.MillisecondsPerSecond;
                builder.Seconds = 0;
            }

            if (builder.Milliseconds != 0)
            {
                if (builder.Milliseconds > divisor)
                {
                    builder.Milliseconds /= divisor;
                    return builder.Build().Normalize();
                }

                builder.Ticks += builder.Milliseconds * NodaConstants.TicksPerMillisecond;
                builder.Milliseconds = 0;
            }

            builder.Ticks /= divisor;

            return builder.Build().Normalize();
        }
    }
}