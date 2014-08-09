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

namespace WebApplications.Utilities.Scheduling.Scheduled
{
    /// <summary>
    /// The result of a scheduled action.
    /// </summary>
    internal class ScheduledActionResult : IScheduledActionResult
    {
        /// <summary>
        /// When the execution was due.
        /// </summary>
        private readonly DateTime _due;

        /// <summary>
        /// When the execution duration.
        /// </summary>
        private TimeSpan _duration;

        /// <summary>
        /// Any exception that was thrown by the function.
        /// </summary>
        private Exception _exception;

        /// <summary>
        /// When the execution actually started.
        /// </summary>
        private DateTime _started;

        /// <summary>
        /// Whether this action was cancelled.
        /// </summary>
        private bool _cancelled;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledActionResult"/> class.
        /// </summary>
        /// <param name="due">The due.</param>
        /// <param name="started">The started.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="cancelled">if set to <see langword="true"/> the action was cancelled.</param>
        /// <remarks></remarks>
        internal ScheduledActionResult(
            DateTime due,
            DateTime started,
            TimeSpan duration,
            Exception exception,
            bool cancelled)
        {
            _due = due;
            _started = started;
            _duration = duration;
            _exception = exception;
            _cancelled = cancelled;
        }

        /// <inheritdoc/>
        public DateTime Due
        {
            get { return _due; }
        }

        /// <inheritdoc/>
        public TimeSpan Duration
        {
            get { return _duration; }
        }

        /// <inheritdoc/>
        public Exception Exception
        {
            get { return _exception; }
        }

        /// <inheritdoc/>
        public DateTime Started
        {
            get { return _started; }
        }

        /// <inheritdoc/>
        public bool Cancelled
        {
            get { return _cancelled; }
        }
    }
}