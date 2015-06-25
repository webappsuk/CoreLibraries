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
    /// Defines a schedule based on a function.
    /// </summary>
    /// <remarks></remarks>
    [PublicAPI]
    public class FunctionalSchedule : ISchedule, IEquatable<FunctionalSchedule>
    {
        private readonly ScheduleOptions _options;
        private readonly string _name;

        [NotNull]
        private readonly Func<Instant, Instant> _function;

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionalSchedule" /> class.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="options">The options.</param>
        public FunctionalSchedule(
            [NotNull] Func<Instant, Instant> function,
            ScheduleOptions options = ScheduleOptions.None)
        {
            if (function == null) throw new ArgumentNullException("function");

            _function = function;
            _options = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionalSchedule" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="function">The function.</param>
        /// <param name="options">The options.</param>
        public FunctionalSchedule(
            [CanBeNull] string name,
            [NotNull] Func<Instant, Instant> function,
            ScheduleOptions options = ScheduleOptions.None)
        {
            if (function == null) throw new ArgumentNullException("function");

            _function = function;
            _options = options;
            _name = name;
        }

        /// <summary>
        /// Gets the optional name.
        /// </summary>
        public string Name
        {
            get { return _name; }
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
            return _function(last);
        }

        /// <summary>
        /// Gets the options.
        /// </summary>
        public ScheduleOptions Options
        {
            get { return _options; }
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of type <see cref="FunctionalSchedule"/>.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public virtual bool Equals(FunctionalSchedule other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _options == other._options && string.Equals(_name, other._name) && _function.Equals(other._function);
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
            return Equals(other as FunctionalSchedule);
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
            return Equals(obj as FunctionalSchedule);
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
                int hashCode = (int)_options;
                hashCode = (hashCode * 397) ^ (_name != null ? _name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ _function.GetHashCode();
                return hashCode;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "Next Run at " + Next(TimeHelpers.Clock.Now);
        }
    }
}