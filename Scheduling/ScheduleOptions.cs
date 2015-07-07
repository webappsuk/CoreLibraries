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

namespace WebApplications.Utilities.Scheduling
{
    /// <summary>
    /// Schedule options.
    /// </summary>
    [Flags]
    [PublicAPI]
    public enum ScheduleOptions : byte
    {
        /// <summary>
        /// Default options.
        /// </summary>
        None = 0,

        /// <summary>
        /// The value passed into <see cref="ISchedule.Next"/> is the previous due <see cref="Instant"/>;
        /// otherwise it is the current <see cref="Instant"/> (i.e. when the last execution was complete).
        /// </summary>
        /// <remarks>
        /// In the event there has been no previous scheduled execution then this will be <see cref="Instant.MinValue"/>.
        /// </remarks>
        FromDue = 1 << 1,

        /// <summary>
        /// Aligns any next due date to the next second.
        /// </summary>
        AlignSeconds = 1 << 2,

        /// <summary>
        /// Aligns any next due date to the next minute.
        /// </summary>
        AlignMinutes = 1 << 3,

        /// <summary>
        /// Aligns any next due date to the next hour.
        /// </summary>
        AlignHours = 1 << 4
    }
}