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
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using JetBrains.Annotations;
using NodaTime;
using WebApplications.Utilities.Logging;

namespace WebApplications.Utilities.Scheduling.Schedules
{
    /// <summary>
    /// Creates a schedule made up of multiple other schedules
    /// </summary>
    public class AggregateSchedule : ISchedule
    {
        [NotNull]
        private readonly IEnumerable<ISchedule> _scheduleCollection;

        private readonly string _name;

        /// <inheritdoc/>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateSchedule"/> class.
        /// </summary>
        /// <param name="scheduleCollection">A collection of schedules.</param>
        /// <param name="name">An optional name for the schedule.</param>
        [PublicAPI]
        public AggregateSchedule([NotNull] IEnumerable<ISchedule> scheduleCollection, [CanBeNull] string name = null)
        {
            Contract.Requires(scheduleCollection != null);
            // Duplicate collection
            _scheduleCollection = scheduleCollection.ToArray();
            _name = name;
            if (!_scheduleCollection.Any())
            {
                _options = ScheduleOptions.None;
                return;
            }

            // Calculate options and ensure all are identical.
            bool first = true;
            foreach (ISchedule schedule in _scheduleCollection)
            {
                if (schedule == null)
                    continue;

                if (first)
                {
                    _options = schedule.Options;
                    first = false;
                    continue;
                }
                if (schedule.Options != _options)
                    throw new LoggingException(() => Resource.AggregateSchedule_Different_Options);
            }
        }

        private readonly ScheduleOptions _options;

        /// <inheritdoc/>
        public Instant Next(Instant last)
        {
            Instant next = Instant.MaxValue;
            foreach (ISchedule schedule in _scheduleCollection)
            {
                Contract.Assert(schedule != null);
                Instant scheduleNext = schedule.Next(last);
                if (scheduleNext < last)
                    return last;

                if (scheduleNext < next)
                    next = scheduleNext;
            }
            return next;
        }

        /// <inheritdoc/>
        public ScheduleOptions Options
        {
            get { return _options; }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "Next Run at " + Next(Scheduler.Clock.Now);
        }
    }
}