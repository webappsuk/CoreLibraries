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
using NodaTime;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Scheduling.Schedules
{
    /// <summary>
    /// Defines a schedule that runs at a specific <see cref="Instant"/>.
    /// </summary>
    [PublicAPI]
    public class OneOffSchedule : ISchedule, IEquatable<OneOffSchedule>
    {
        /// <summary>
        /// An instant in time.
        /// </summary>
        public readonly Instant Instant;

        private readonly string _name;

        /// <summary>
        /// Gets the optional name.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        private readonly ScheduleOptions _options;

        /// <summary>
        /// Gets the options.
        /// </summary>
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

        /// <summary>
        /// Gets the next scheduled event.
        /// </summary>
        /// <param name="last">The last <see cref="Instant" /> the action was completed.</param>
        /// <returns>
        /// The next <see cref="Instant" /> in the schedule, or <see cref="NodaTime.Instant.MaxValue" /> for never/end of time.
        /// </returns>
        public Instant Next(Instant last)
        {
            return Instant >= last ? Instant : Instant.MaxValue;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public virtual bool Equals(OneOffSchedule other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _options == other._options && Instant.Equals(other.Instant) && string.Equals(_name, other._name);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public virtual bool Equals(ISchedule other)
        {
            return Equals(other as OneOffSchedule);
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
            return Equals(obj as OneOffSchedule);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Instant.GetHashCode();
                hashCode = (hashCode * 397) ^ (_name != null ? _name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)_options;
                return hashCode;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "Run Once at " + Instant;
        }
    }
}