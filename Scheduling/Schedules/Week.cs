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
    /// Enumeration of the potential weeks in a week.
    /// </summary>
    [Flags]
    public enum Week : ulong
    {
        /// <summary>
        /// Sometimes the first few days of a year are not in week one.
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
        TwentyFourth = (ulong)1 << 24,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        TwentyFifth = (ulong)1 << 25,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        TwentySixth = (ulong)1 << 26,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        TwentySeventh = (ulong)1 << 27,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        TwentyEighth = (ulong)1 << 28,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        TwentyNinth = (ulong)1 << 29,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        Thirtieth = (ulong)1 << 30,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        ThirtyFirst = (ulong)1 << 31,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        ThirtySecond = (ulong)1 << 32,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        ThirtyThird = (ulong)1 << 33,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        ThirtyFourth = (ulong)1 << 34,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        ThirtyFifth = (ulong)1 << 35,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        ThirtySixth = (ulong)1 << 36,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        ThirtySeventh = (ulong)1 << 37,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        ThirtyEighth = (ulong)1 << 38,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        ThirtyNinth = (ulong)1 << 39,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        Fortieth = (ulong)1 << 40,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        FortyFirst = (ulong)1 << 41,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        FortySecond = (ulong)1 << 42,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        FortyThird = (ulong)1 << 43,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        FortyForth = (ulong)1 << 44,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        FortyFifth = (ulong)1 << 45,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        FortySixth = (ulong)1 << 46,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        FortySeventh = (ulong)1 << 47,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        FortyEighth = (ulong)1 << 48,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        FortyNinth = (ulong)1 << 49,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        Fiftieth = (ulong)1 << 50,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        FiftyFirst = (ulong)1 << 51,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        FiftySecond = (ulong)1 << 52,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        Never = 0,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        EveryFourWeeks =
            First | Fifth | Ninth | Thirteenth | Seventeenth | TwentyFirst | TwentyFifth | TwentyNinth | ThirtyThird |
            ThirtySeventh | FortyFirst | FortyFifth | FortyNinth,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        EveryTwoWeeks =
            EveryFourWeeks | Third | Seventh | Eleventh | Fifteenth | Nineteenth | TwentyThird | TwentySeventh |
            ThirtyFirst | ThirtyFifth | ThirtyNinth | FortyThird | FortySeventh | FiftyFirst,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        EveryOtherWeek = ~EveryTwoWeeks,

        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        Every = EveryTwoWeeks | EveryOtherWeek
    }
}