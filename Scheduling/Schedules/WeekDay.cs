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
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Scheduling.Schedules
{
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum WeekDay
    {
        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        Sunday = 1,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        Monday = 1 << 1,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        Tuesday = 1 << 2,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        Wednesday = 1 << 3,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        Thursday = 1 << 4,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        Friday = 1 << 5,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        Saturday = 1 << 6,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        Never = 0,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        Weekend = Saturday | Sunday,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        WeekDay = Monday | Tuesday | Wednesday | Thursday | Friday,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        Every = Weekend | WeekDay
    }
}