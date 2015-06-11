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
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Ranges
{
    /// <summary>
    /// A range of <see cref="TimeSpan">s</see> values with the time component ignored.
    /// </summary>
    [PublicAPI]
    public class TimeSpanRange : Range<TimeSpan>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeSpanRange"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public TimeSpanRange(TimeSpan start, TimeSpan end)
            : base(start, end, AutoStep(end - start))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeSpanRange"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="step">The step in days.</param>
        public TimeSpanRange(TimeSpan start, TimeSpan end, TimeSpan step)
            : base(start, end, step)
        {
        }

        /// <summary>
        /// Given a delta automatically returns a sensible step size.
        /// </summary>
        /// <param name="delta">The delta.</param>
        /// <returns>TimeSpan.</returns>
        public static TimeSpan AutoStep(TimeSpan delta)
        {
            if (delta < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException("delta", delta, Resources.TimeSpanRange_AutoStep_DeltaNegative);

            if (delta < TimeSpan.FromMilliseconds(1))
                return TimeSpan.FromTicks(1);
            if (delta < TimeSpan.FromSeconds(1))
                return TimeSpan.FromMilliseconds(1);
            if (delta < TimeSpan.FromMinutes(1))
                return TimeSpan.FromSeconds(1);
            if (delta < TimeSpan.FromHours(1))
                return TimeSpan.FromMinutes(1);
            if (delta < TimeSpan.FromDays(1))
                return TimeSpan.FromHours(1);
            return TimeSpan.FromDays(1);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format("{0} - {1}", Start, End);
        }
    }
}