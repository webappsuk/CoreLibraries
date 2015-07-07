#region © Copyright Web Applications (UK) Ltd, 2015.  All rights reserved.
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NodaTime;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Scheduling.Schedules
{
    /// <summary>
    /// Creates a schedule made up of multiple other schedules
    /// </summary>
    [PublicAPI]
    public class AggregateSchedule : ISchedule, IEnumerable<ISchedule>, IEquatable<AggregateSchedule>
    {
        [NotNull]
        [ItemNotNull]
        private readonly ISchedule[] _schedules;

        private readonly string _name;

        private readonly ScheduleOptions _options;

        private readonly int _hashCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateSchedule" /> class, used by configuration system.
        /// </summary>
        /// <param name="name">An optional name for the schedule.</param>
        /// <param name="schedule1">The 1st schedule.</param>
        /// <param name="schedule2">The 2nd schedule.</param>
        /// <param name="schedule3">The 3rd schedule.</param>
        /// <param name="schedule4">The 4th schedule.</param>
        /// <param name="schedule5">The 5th schedule.</param>
        /// <param name="schedule6">The 6th schedule.</param>
        /// <param name="schedule7">The 7th schedule.</param>
        /// <param name="schedule8">The 8th schedule.</param>
        /// <param name="schedule9">The 9th schedule.</param>
        /// <param name="schedule10">The 10th schedule.</param>
        /// <param name="schedule11">The 11th schedule.</param>
        /// <param name="schedule12">The 12th schedule.</param>
        /// <param name="schedule13">The 13th schedule.</param>
        /// <param name="schedule14">The 14th schedule.</param>
        /// <param name="schedule15">The 15th schedule.</param>
        /// <param name="schedule16">The 16th schedule.</param>
        /// <param name="schedule17">The 17th schedule.</param>
        /// <param name="schedule18">The 18th schedule.</param>
        /// <param name="schedule19">The 19th schedule.</param>
        /// <param name="schedule20">The 20th schedule.</param>
        /// <param name="schedule21">The 21st schedule.</param>
        /// <param name="schedule22">The 22nd schedule.</param>
        /// <param name="schedule23">The 23rd schedule.</param>
        /// <param name="schedule24">The 24th schedule.</param>
        /// <param name="schedule25">The 25th schedule.</param>
        /// <param name="schedule26">The 26th schedule.</param>
        /// <param name="schedule27">The 27th schedule.</param>
        /// <param name="schedule28">The 28th schedule.</param>
        /// <param name="schedule29">The 29th schedule.</param>
        /// <param name="schedule30">The 30th schedule.</param>
        /// <param name="schedule31">The 31st schedule.</param>
        /// <param name="schedule32">The 32nd schedule.</param>
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
        public AggregateSchedule([NotNull] IEnumerable<ISchedule> schedules)
            : this(null, schedules.ToArray())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateSchedule" /> class.
        /// </summary>
        /// <param name="name">An optional name for the schedule.</param>
        /// <param name="schedules">An enumeration of schedules.</param>
        public AggregateSchedule([CanBeNull] string name, [NotNull] IEnumerable<ISchedule> schedules)
            : this(name, schedules.ToArray())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateSchedule" /> class.
        /// </summary>
        /// <param name="schedules">A collection of schedules.</param>
        public AggregateSchedule([NotNull] params ISchedule[] schedules)
            : this(null, schedules)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateSchedule" /> class.
        /// </summary>
        /// <param name="name">An optional name for the schedule.</param>
        /// <param name="schedules">A collection of schedules.</param>
        /// <exception cref="System.ArgumentException">The specified schedules have differing options.</exception>
        public AggregateSchedule([CanBeNull] string name, [NotNull] params ISchedule[] schedules)
        {
            if (schedules == null) throw new ArgumentNullException("schedules");

            // Duplicate collection
            _schedules = schedules.Where(s => s != null).ToArray();
            _name = name;
            if (!_schedules.Any())
            {
                _options = ScheduleOptions.None;
                return;
            }

            unchecked
            {
                int hashCode = _schedules.Length;

                // Calculate options and ensure all are identical.
                bool first = true;
                foreach (ISchedule schedule in _schedules.OrderBy(s => s.Name))
                {
                    Debug.Assert(schedule != null);

                    hashCode = (hashCode * 397) ^ schedule.GetHashCode();

                    if (first)
                    {
                        _options = schedule.Options;
                        first = false;
                        continue;
                    }

                    if (schedule.Options != _options)
                        throw new ArgumentException(Resource.AggregateSchedule_Different_Options, "schedules");
                }

                hashCode = (hashCode * 397) ^ (_name != null ? _name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)_options;
                _hashCode = hashCode;
            }
        }

        /// <summary>
        /// Gets the next scheduled event.
        /// </summary>
        /// <param name="last">The last <see cref="Instant" /> the action was completed.</param>
        /// <returns>
        /// The next <see cref="Instant" /> in the schedule, or <see cref="Instant.MaxValue" /> for never/end of time.
        /// </returns>
        public Instant Next(Instant last)
        {
            Instant next = Instant.MaxValue;
            foreach (ISchedule schedule in _schedules)
            {
                Debug.Assert(schedule != null);
                Instant scheduleNext = schedule.Next(last);
                if (scheduleNext < last)
                    return last;

                if (scheduleNext < next)
                    next = scheduleNext;
            }
            return next;
        }

        /// <summary>
        /// Gets the optional name.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <remarks></remarks>
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

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                "Aggregate Schedule ({0} schedules), next Run at {1}",
                _schedules.Length,
                Next(TimeHelpers.Clock.Now));
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of type <see cref="AggregateSchedule"/>.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public virtual bool Equals(AggregateSchedule other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _hashCode == other._hashCode &&
                    _options == other._options &&
                   string.Equals(_name, other._name) &&
                // ReSharper disable PossibleNullReferenceException
                   _schedules.OrderBy(s => s.Name).SequenceEqual(other._schedules.OrderBy(s => s.Name));
            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of type <see cref="ISchedule"/>.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public virtual bool Equals(ISchedule other)
        {
            return Equals(other as AggregateSchedule);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <see langword="true" /> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <see langword="false" />.
        /// </returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as AggregateSchedule);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}