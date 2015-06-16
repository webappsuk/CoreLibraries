#region � Copyright Web Applications (UK) Ltd, 2015.  All rights reserved.
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
    /// Defines a schedule that runs after a specific gap.
    /// </summary>
    [PublicAPI]
    public class GapSchedule : ISchedule
    {
        /// <summary>
        /// The duration between runs.
        /// </summary>
        public readonly Duration Duration;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="GapSchedule"/> class.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <param name="options">The options.</param>
        public GapSchedule(Duration duration, ScheduleOptions options = ScheduleOptions.None)
        {
            Duration = duration < Duration.Zero ? Duration.Zero : duration;
            _options = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GapSchedule"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="options">The options.</param>
        public GapSchedule([CanBeNull] string name, Duration duration, ScheduleOptions options = ScheduleOptions.None)
        {
            _name = name;
            Duration = duration < Duration.Zero ? Duration.Zero : duration;
            _options = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GapSchedule"/> class.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <param name="options">The options.</param>
        public GapSchedule(TimeSpan timeSpan, ScheduleOptions options = ScheduleOptions.None)
        {
            Duration duration = Duration.FromTimeSpan(timeSpan);
            Duration = duration < Duration.Zero ? Duration.Zero : duration;
            _options = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GapSchedule"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="timeSpan">The time span.</param>
        /// <param name="options">The options.</param>
        public GapSchedule([CanBeNull] string name, TimeSpan timeSpan, ScheduleOptions options = ScheduleOptions.None)
        {
            Duration duration = Duration.FromTimeSpan(timeSpan);
            _name = name;
            Duration = duration < Duration.Zero ? Duration.Zero : duration;
            _options = options;
        }

        /// <inheritdoc/>
        public Instant Next(Instant last)
        {
            return last + Duration;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "Run every " + Duration.TotalMilliseconds() + "ms";
        }
    }
}