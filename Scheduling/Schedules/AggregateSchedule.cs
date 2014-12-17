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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using WebApplications.Utilities.Annotations;
using NodaTime;

namespace WebApplications.Utilities.Scheduling.Schedules
{
    /// <summary>
    /// Creates a schedule made up of multiple other schedules
    /// </summary>
    public class AggregateSchedule : ISchedule, IEnumerable<ISchedule>
    {
        [NotNull]
        private readonly ISchedule[] _schedules;

        private readonly string _name;

        /// <inheritdoc/>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateSchedule" /> class, used by configuration system.
        /// </summary>
        /// <param name="name">An optional name for the schedule.</param>
        /// <param name="schedule1">The schedule1.</param>
        /// <param name="schedule2">The schedule2.</param>
        /// <param name="schedule3">The schedule3.</param>
        /// <param name="schedule4">The schedule4.</param>
        /// <param name="schedule5">The schedule5.</param>
        /// <param name="schedule6">The schedule6.</param>
        /// <param name="schedule7">The schedule7.</param>
        /// <param name="schedule8">The schedule8.</param>
        /// <param name="schedule9">The schedule9.</param>
        /// <param name="schedule10">The schedule10.</param>
        /// <param name="schedule11">The schedule11.</param>
        /// <param name="schedule12">The schedule12.</param>
        /// <param name="schedule13">The schedule13.</param>
        /// <param name="schedule14">The schedule14.</param>
        /// <param name="schedule15">The schedule15.</param>
        /// <param name="schedule16">The schedule16.</param>
        /// <param name="schedule17">The schedule17.</param>
        /// <param name="schedule18">The schedule18.</param>
        /// <param name="schedule19">The schedule19.</param>
        /// <param name="schedule20">The schedule20.</param>
        /// <param name="schedule21">The schedule21.</param>
        /// <param name="schedule22">The schedule22.</param>
        /// <param name="schedule23">The schedule23.</param>
        /// <param name="schedule24">The schedule24.</param>
        /// <param name="schedule25">The schedule25.</param>
        /// <param name="schedule26">The schedule26.</param>
        /// <param name="schedule27">The schedule27.</param>
        /// <param name="schedule28">The schedule28.</param>
        /// <param name="schedule29">The schedule29.</param>
        /// <param name="schedule30">The schedule30.</param>
        /// <param name="schedule31">The schedule31.</param>
        /// <param name="schedule32">The schedule32.</param>
        [UsedImplicitly]
        private AggregateSchedule(
            [CanBeNull] string name,
            [CanBeNull] string schedule1,
            [CanBeNull] string schedule2 = null,
            [CanBeNull] string schedule3 = null,
            [CanBeNull] string schedule4 = null,
            [CanBeNull] string schedule5 = null,
            [CanBeNull] string schedule6 = null,
            [CanBeNull] string schedule7 = null,
            [CanBeNull] string schedule8 = null,
            [CanBeNull] string schedule9 = null,
            [CanBeNull] string schedule10 = null,
            [CanBeNull] string schedule11 = null,
            [CanBeNull] string schedule12 = null,
            [CanBeNull] string schedule13 = null,
            [CanBeNull] string schedule14 = null,
            [CanBeNull] string schedule15 = null,
            [CanBeNull] string schedule16 = null,
            [CanBeNull] string schedule17 = null,
            [CanBeNull] string schedule18 = null,
            [CanBeNull] string schedule19 = null,
            [CanBeNull] string schedule20 = null,
            [CanBeNull] string schedule21 = null,
            [CanBeNull] string schedule22 = null,
            [CanBeNull] string schedule23 = null,
            [CanBeNull] string schedule24 = null,
            [CanBeNull] string schedule25 = null,
            [CanBeNull] string schedule26 = null,
            [CanBeNull] string schedule27 = null,
            [CanBeNull] string schedule28 = null,
            [CanBeNull] string schedule29 = null,
            [CanBeNull] string schedule30 = null,
            [CanBeNull] string schedule31 = null,
            [CanBeNull] string schedule32 = null)
            : this(name,
                   new[]
                   {
                       schedule1,
                       schedule2,
                       schedule3,
                       schedule4,
                       schedule5,
                       schedule6,
                       schedule7,
                       schedule8,
                       schedule9,
                       schedule10,
                       schedule11,
                       schedule12,
                       schedule13,
                       schedule14,
                       schedule15,
                       schedule16,
                       schedule17,
                       schedule18,
                       schedule19,
                       schedule20,
                       schedule21,
                       schedule22,
                       schedule23,
                       schedule24,
                       schedule25,
                       schedule26,
                       schedule27,
                       schedule28,
                       schedule29,
                       schedule30,
                       schedule31,
                       schedule32
                   }
                       .Where(n => !string.IsNullOrEmpty(n))
                       .Select(Scheduler.GetSchedule)
                       .ToArray())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateSchedule"/> class.
        /// </summary>
        /// <param name="schedules">An enumeration of schedules.</param>
        [PublicAPI]
        public AggregateSchedule([NotNull] IEnumerable<ISchedule> schedules)
            : this(null, schedules.ToArray())
        {
            Contract.Requires(schedules != null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateSchedule" /> class.
        /// </summary>
        /// <param name="name">An optional name for the schedule.</param>
        /// <param name="schedules">An enumeration of schedules.</param>
        [PublicAPI]
        public AggregateSchedule([CanBeNull] string name, [NotNull] IEnumerable<ISchedule> schedules)
            : this(name, schedules.ToArray())
        {
            Contract.Requires(schedules != null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateSchedule" /> class.
        /// </summary>
        /// <param name="schedules">A collection of schedules.</param>
        [PublicAPI]
        public AggregateSchedule([NotNull] params ISchedule[] schedules)
            : this(null, schedules)
        {
            Contract.Requires(schedules != null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateSchedule" /> class.
        /// </summary>
        /// <param name="name">An optional name for the schedule.</param>
        /// <param name="schedules">A collection of schedules.</param>
        /// <exception cref="System.ArgumentException">The specified schedules have differing options.</exception>
        [PublicAPI]
        public AggregateSchedule([CanBeNull] string name, [NotNull] params ISchedule[] schedules)
        {
            Contract.Requires(schedules != null);
            // Duplicate collection
            _schedules = schedules;
            _name = name;
            if (!_schedules.Any())
            {
                _options = ScheduleOptions.None;
                return;
            }

            // Calculate options and ensure all are identical.
            bool first = true;
            foreach (ISchedule schedule in _schedules)
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
                    throw new ArgumentException(Resource.AggregateSchedule_Different_Options, "schedules");
            }
        }

        private readonly ScheduleOptions _options;

        /// <inheritdoc/>
        public Instant Next(Instant last)
        {
            Instant next = Instant.MaxValue;
            foreach (ISchedule schedule in _schedules)
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

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<ISchedule> GetEnumerator()
        {
            return ((IEnumerable<ISchedule>)_schedules).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format(
                "Aggregate Schedule ({0} schedules), next Run at {1}",
                _schedules.Length,
                Next(Scheduler.Clock.Now));
        }
    }
}