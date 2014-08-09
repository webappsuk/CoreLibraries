﻿#region © Copyright Web Applications (UK) Ltd, 2014.  All rights reserved.
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
        Zeroth = (ulong)1,

        /// <summary>
        /// 
        /// </summary>
        First = (ulong)1 << 1,

        /// <summary>
        /// 
        /// </summary>
        Second = (ulong)1 << 2,

        /// <summary>
        /// 
        /// </summary>
        Third = (ulong)1 << 3,

        /// <summary>
        /// 
        /// </summary>
        Fourth = (ulong)1 << 4,

        /// <summary>
        /// 
        /// </summary>
        Fifth = (ulong)1 << 5,

        /// <summary>
        /// 
        /// </summary>
        Sixth = (ulong)1 << 6,

        /// <summary>
        /// 
        /// </summary>
        Seventh = (ulong)1 << 7,

        /// <summary>
        /// 
        /// </summary>
        Eighth = (ulong)1 << 8,

        /// <summary>
        /// 
        /// </summary>
        Ninth = (ulong)1 << 9,

        /// <summary>
        /// 
        /// </summary>
        Tenth = (ulong)1 << 10,

        /// <summary>
        /// 
        /// </summary>
        Eleventh = (ulong)1 << 11,

        /// <summary>
        /// 
        /// </summary>
        Twelfth = (ulong)1 << 12,

        /// <summary>
        /// 
        /// </summary>
        Thirteenth = (ulong)1 << 13,

        /// <summary>
        /// 
        /// </summary>
        Fourteenth = (ulong)1 << 14,

        /// <summary>
        /// 
        /// </summary>
        Fifteenth = (ulong)1 << 15,

        /// <summary>
        /// 
        /// </summary>
        Sixteenth = (ulong)1 << 16,

        /// <summary>
        /// 
        /// </summary>
        Seventeenth = (ulong)1 << 17,

        /// <summary>
        /// 
        /// </summary>
        Eighteenth = (ulong)1 << 18,

        /// <summary>
        /// 
        /// </summary>
        Nineteenth = (ulong)1 << 19,

        /// <summary>
        /// 
        /// </summary>
        Twentieth = (ulong)1 << 20,

        /// <summary>
        /// 
        /// </summary>
        TwentyFirst = (ulong)1 << 21,

        /// <summary>
        /// 
        /// </summary>
        TwentySecond = (ulong)1 << 22,

        /// <summary>
        /// 
        /// </summary>
        TwentyThird = (ulong)1 << 23,

        /// <summary>
        /// 
        /// </summary>
        Never = 0,

        /// <summary>
        /// 
        /// </summary>
        TwentyFourth = Zeroth,

        /// <summary>
        /// 
        /// </summary>
        EveryTwelveHours = Zeroth | Twelfth,

        /// <summary>
        /// 
        /// </summary>
        EverySixHours = EveryTwelveHours | Sixth | Eighteenth,

        /// <summary>
        /// 
        /// </summary>
        EveryFourHours = EveryTwelveHours | Fourth | Eighth | Sixteenth | Twentieth,

        /// <summary>
        /// 
        /// </summary>
        EveryThreeHours = EverySixHours | Third | Ninth | Fifteenth | TwentyFirst,

        /// <summary>
        /// 
        /// </summary>
        EveryTwoHours = EveryFourHours | Second | Sixth | Tenth | Fourteenth | Eighteenth | TwentySecond,

        /// <summary>
        /// 
        /// </summary>
        EveryOtherHour = ~EveryTwoHours,

        /// <summary>
        /// 
        /// </summary>
        Every = EveryTwoHours | EveryOtherHour
    }
}