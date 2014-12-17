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
    /// Enumeration of the potential seconds in a minute.
    /// </summary>
    [Flags]
    public enum Hour : ulong
    {
        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        Zeroth = (ulong)1,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        First = (ulong)1 << 1,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        Second = (ulong)1 << 2,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        Third = (ulong)1 << 3,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        Fourth = (ulong)1 << 4,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        Fifth = (ulong)1 << 5,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        Sixth = (ulong)1 << 6,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        Seventh = (ulong)1 << 7,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        Eighth = (ulong)1 << 8,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        Ninth = (ulong)1 << 9,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        Tenth = (ulong)1 << 10,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        Eleventh = (ulong)1 << 11,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        Twelfth = (ulong)1 << 12,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        Thirteenth = (ulong)1 << 13,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        Fourteenth = (ulong)1 << 14,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        Fifteenth = (ulong)1 << 15,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        Sixteenth = (ulong)1 << 16,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        Seventeenth = (ulong)1 << 17,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        Eighteenth = (ulong)1 << 18,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        Nineteenth = (ulong)1 << 19,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        Twentieth = (ulong)1 << 20,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        TwentyFirst = (ulong)1 << 21,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        TwentySecond = (ulong)1 << 22,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        TwentyThird = (ulong)1 << 23,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        Never = 0,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        TwentyFourth = Zeroth,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        EveryTwelveHours = Zeroth | Twelfth,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        EverySixHours = EveryTwelveHours | Sixth | Eighteenth,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        EveryFourHours = EveryTwelveHours | Fourth | Eighth | Sixteenth | Twentieth,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        EveryThreeHours = EverySixHours | Third | Ninth | Fifteenth | TwentyFirst,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        EveryTwoHours = EveryFourHours | Second | Sixth | Tenth | Fourteenth | Eighteenth | TwentySecond,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        EveryOtherHour = ~EveryTwoHours,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        Every = EveryTwoHours | EveryOtherHour
    }
}