﻿#region © Copyright Web Applications (UK) Ltd, 2015.  All rights reserved.
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

namespace WebApplications.Utilities.Scheduling.Schedules
{
    /// <summary>
    /// Defines a schedule that runs at a specific <see cref="Instant"/>.
    /// </summary>
    [PublicAPI]
    public class OneOffSchedule : ISchedule
    {
        /// <summary>
        /// An instant in time.
        /// </summary>
        public readonly Instant Instant;

        private readonly string _name;

        /// <inheritdoc/>
        public string Name
        {
            get { return _name; }
        }

        private readonly ScheduleOptions _options;

        /// <inheritdoc/>
        public ScheduleOptions Options
        {
            get { return _options; }
        }

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="OneOffSchedule" /> class.
        /// </summary>
        /// <param name="instant">The instant.</param>
        /// <param name="options">The options.</param>
        public OneOffSchedule(Instant instant, ScheduleOptions options = ScheduleOptions.None)
        {
            Instant = instant;
            _options = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OneOffSchedule" /> class.
        /// </summary>
        /// <param name="name">An optional name for the schedule.</param>
        /// <param name="instant">The instant.</param>
        /// <param name="options">The options.</param>
        public OneOffSchedule([CanBeNull] string name, Instant instant, ScheduleOptions options = ScheduleOptions.None)
        {
            _name = name;
            Instant = instant;
            _options = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OneOffSchedule" /> class.
        /// </summary>
        /// <param name="zonedDateTime">The date and time.</param>
        /// <param name="options">The options.</param>
        public OneOffSchedule(ZonedDateTime zonedDateTime, ScheduleOptions options = ScheduleOptions.None)
        {
            Instant = zonedDateTime.ToInstant();
            _options = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OneOffSchedule" /> class.
        /// </summary>
        /// <param name="name">An optional name for the schedule.</param>
        /// <param name="zonedDateTime">The date and time.</param>
        /// <param name="options">The options.</param>
        public OneOffSchedule(
            [CanBeNull] string name,
            ZonedDateTime zonedDateTime,
            ScheduleOptions options = ScheduleOptions.None)
        {
            _name = name;
            Instant = zonedDateTime.ToInstant();
            _options = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OneOffSchedule" /> class, used by configuration system.
        /// </summary>
        /// <param name="name">An optional name for the schedule.</param>
        /// <param name="dateTime">The date and time.</param>
        /// <param name="options">The options.</param>
        [UsedImplicitly]
        private OneOffSchedule(
            [CanBeNull] string name,
            DateTimeOffset dateTime,
            ScheduleOptions options = ScheduleOptions.None)
        {
            _name = name;
            Instant = Instant.FromDateTimeOffset(dateTime);
            _options = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OneOffSchedule" /> class, used by configuration system.
        /// </summary>
        /// <param name="name">An optional name for the schedule.</param>
        /// <param name="dateTime">The date and time.</param>
        /// <param name="timeZone">The time zone.</param>
        /// <param name="options">The options.</param>
        [UsedImplicitly]
        private OneOffSchedule(
            [CanBeNull] string name,
            DateTime dateTime,
            [NotNull] string timeZone,
            ScheduleOptions options = ScheduleOptions.None)
        {
            if (timeZone == null) throw new ArgumentNullException("timeZone");

            DateTimeZone tz = TimeHelpers.DateTimeZoneProvider[timeZone];
            if (tz == null)
            {
                throw new ArgumentException(
                    // ReSharper disable once AssignNullToNotNullAttribute
                    string.Format(Resource.OneOffSchedule_InvalidTimeZone, timeZone),
                    "timeZone");
            }

            _name = name;
            Instant = tz.AtLeniently(LocalDateTime.FromDateTime(dateTime)).ToInstant();
            _options = options;
        }
        #endregion

        /// <inheritdoc/>
        public Instant Next(Instant last)
        {
            return Instant >= last ? Instant : Instant.MaxValue;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "Run Once at " + Instant;
        }
    }
}