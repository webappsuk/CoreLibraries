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
using JetBrains.Annotations;
using NodaTime;

namespace WebApplications.Utilities.Scheduling.Schedules
{
    /// <summary>
    /// Defines a single datetime schedule
    /// </summary>
    public class OneOffSchedule : ISchedule
    {
        /// <summary>
        /// An instant in time.
        /// </summary>
        [PublicAPI]
        public readonly Instant Instant;

        /// <summary>
        /// The schedules optional name.
        /// </summary>
        private readonly string _name;

        /// <inheritdoc/>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OneOffSchedule" /> class.
        /// </summary>
        /// <param name="instant">The instant.</param>
        /// <param name="name">An optional name for the schedule.</param>
        [PublicAPI]
        public OneOffSchedule(Instant instant, [CanBeNull] string name = null)
        {
            Instant = instant;
            _name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OneOffSchedule" /> class.
        /// </summary>
        /// <param name="dateTime">The date and time.</param>
        /// <param name="name">An optional name for the schedule.</param>
        [PublicAPI]
        public OneOffSchedule(ZonedDateTime dateTime, [CanBeNull] string name = null)
        {
            Instant = dateTime.ToInstant();
            _name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OneOffSchedule" /> class.
        /// </summary>
        /// <param name="dateTimeUTC">The date and time (UTC).</param>
        /// <param name="name">An optional name for the schedule.</param>
        [PublicAPI]
        public OneOffSchedule(DateTime dateTimeUTC, [CanBeNull] string name = null)
        {
            Instant = Instant.FromDateTimeUtc(dateTimeUTC);
            _name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OneOffSchedule" /> class.
        /// </summary>
        /// <param name="dateTime">The date and time.</param>
        /// <param name="name">An optional name for the schedule.</param>
        [PublicAPI]
        public OneOffSchedule(DateTimeOffset dateTime, [CanBeNull] string name = null)
        {
            Instant = Instant.FromDateTimeOffset(dateTime);
            _name = name;
        }

        /// <inheritdoc/>
        public Instant Next(Instant last)
        {
            return Instant > last ? Instant : Instant.MaxValue;
        }

        /// <inheritdoc/>
        public ScheduleOptions Options
        {
            get { return ScheduleOptions.None; }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "Run Once at " + Instant;
        }
    }
}