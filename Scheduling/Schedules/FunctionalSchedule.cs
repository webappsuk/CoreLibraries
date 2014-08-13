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
using System.Diagnostics.Contracts;
using JetBrains.Annotations;
using NodaTime;

namespace WebApplications.Utilities.Scheduling.Schedules
{
    /// <summary>
    /// Defines a schedule based on a function.
    /// </summary>
    /// <remarks></remarks>
    public class FunctionalSchedule : ISchedule
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
        [PublicAPI]
        public FunctionalSchedule(
            [NotNull] Func<Instant, Instant> function,
            ScheduleOptions options = ScheduleOptions.None)
        {
            Contract.Requires(function != null);
            _function = function;
            _options = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionalSchedule" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="function">The function.</param>
        /// <param name="options">The options.</param>
        [PublicAPI]
        public FunctionalSchedule(
            [CanBeNull] string name,
            [NotNull] Func<Instant, Instant> function,
            ScheduleOptions options = ScheduleOptions.None)
        {
            Contract.Requires(function != null);
            _function = function;
            _options = options;
            _name = name;
        }

        /// <inheritdoc/>
        public string Name
        {
            get { return _name; }
        }

        /// <inheritdoc/>
        public Instant Next(Instant last)
        {
            return _function(last);
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