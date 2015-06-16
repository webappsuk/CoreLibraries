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

namespace WebApplications.Utilities.Scheduling.Schedules
{
    /// <summary>
    /// Enumeration of the potential weeks in a year.
    /// </summary>
    [Flags]
    [PublicAPI]
    public enum Week : ulong
    {
        /// <summary>
        /// The first few days of the year that are part of the last week of the previous year.
        /// </summary>
        Zeroth = 1UL,

        /// <summary>
        /// The first week in the year.
        /// </summary>
        First = 1UL << 1,

        /// <summary>
        /// The second week in the year.
        /// </summary>
        Second = 1UL << 2,

        /// <summary>
        /// The third week in the year.
        /// </summary>
        Third = 1UL << 3,

        /// <summary>
        /// The fourth week in the year.
        /// </summary>
        Fourth = 1UL << 4,

        /// <summary>
        /// The fifth week in the year.
        /// </summary>
        Fifth = 1UL << 5,

        /// <summary>
        /// The sixth week in the year.
        /// </summary>
        Sixth = 1UL << 6,

        /// <summary>
        /// The seventh week in the year.
        /// </summary>
        Seventh = 1UL << 7,

        /// <summary>
        /// The eighth week in the year.
        /// </summary>
        Eighth = 1UL << 8,

        /// <summary>
        /// The ninth week in the year.
        /// </summary>
        Ninth = 1UL << 9,

        /// <summary>
        /// The tenth week in the year.
        /// </summary>
        Tenth = 1UL << 10,

        /// <summary>
        /// The eleventh week in the year.
        /// </summary>
        Eleventh = 1UL << 11,

        /// <summary>
        /// The twelfth week in the year.
        /// </summary>
        Twelfth = 1UL << 12,

        /// <summary>
        /// The thirteenth week in the year.
        /// </summary>
        Thirteenth = 1UL << 13,

        /// <summary>
        /// The fourteenth week in the year.
        /// </summary>
        Fourteenth = 1UL << 14,

        /// <summary>
        /// The fifteenth week in the year.
        /// </summary>
        Fifteenth = 1UL << 15,

        /// <summary>
        /// The sixteenth week in the year.
        /// </summary>
        Sixteenth = 1UL << 16,

        /// <summary>
        /// The seventeenth week in the year.
        /// </summary>
        Seventeenth = 1UL << 17,

        /// <summary>
        /// The eighteenth week in the year.
        /// </summary>
        Eighteenth = 1UL << 18,

        /// <summary>
        /// The nineteenth week in the year.
        /// </summary>
        Nineteenth = 1UL << 19,

        /// <summary>
        /// The twentieth week in the year.
        /// </summary>
        Twentieth = 1UL << 20,

        /// <summary>
        /// The twenty first week in the year.
        /// </summary>
        TwentyFirst = 1UL << 21,

        /// <summary>
        /// The twenty second week in the year.
        /// </summary>
        TwentySecond = 1UL << 22,

        /// <summary>
        /// The twenty third week in the year.
        /// </summary>
        TwentyThird = 1UL << 23,

        /// <summary>
        /// The twenty fourth week in the year.
        /// </summary>
        TwentyFourth = 1UL << 24,

        /// <summary>
        /// The twenty fifth week in the year.
        /// </summary>
        TwentyFifth = 1UL << 25,

        /// <summary>
        /// The twenty sixth week in the year.
        /// </summary>
        TwentySixth = 1UL << 26,

        /// <summary>
        /// The twenty seventh week in the year.
        /// </summary>
        TwentySeventh = 1UL << 27,

        /// <summary>
        /// The twenty eighth week in the year.
        /// </summary>
        TwentyEighth = 1UL << 28,

        /// <summary>
        /// The twenty ninth week in the year.
        /// </summary>
        TwentyNinth = 1UL << 29,

        /// <summary>
        /// The thirtieth week in the year.
        /// </summary>
        Thirtieth = 1UL << 30,

        /// <summary>
        /// The thirty first week in the year.
        /// </summary>
        ThirtyFirst = 1UL << 31,

        /// <summary>
        /// The thirty second week in the year.
        /// </summary>
        ThirtySecond = 1UL << 32,

        /// <summary>
        /// The thirty third week in the year.
        /// </summary>
        ThirtyThird = 1UL << 33,

        /// <summary>
        /// The thirty fourth week in the year.
        /// </summary>
        ThirtyFourth = 1UL << 34,

        /// <summary>
        /// The thirty fifth week in the year.
        /// </summary>
        ThirtyFifth = 1UL << 35,

        /// <summary>
        /// The thirty sixth week in the year.
        /// </summary>
        ThirtySixth = 1UL << 36,

        /// <summary>
        /// The thirty seventh week in the year.
        /// </summary>
        ThirtySeventh = 1UL << 37,

        /// <summary>
        /// The thirty eighth week in the year.
        /// </summary>
        ThirtyEighth = 1UL << 38,

        /// <summary>
        /// The thirty ninth week in the year.
        /// </summary>
        ThirtyNinth = 1UL << 39,

        /// <summary>
        /// The fortieth week in the year.
        /// </summary>
        Fortieth = 1UL << 40,

        /// <summary>
        /// The forty first week in the year.
        /// </summary>
        FortyFirst = 1UL << 41,

        /// <summary>
        /// The forty second week in the year.
        /// </summary>
        FortySecond = 1UL << 42,

        /// <summary>
        /// The forty third week in the year.
        /// </summary>
        FortyThird = 1UL << 43,

        /// <summary>
        /// The forty forth week in the year.
        /// </summary>
        FortyForth = 1UL << 44,

        /// <summary>
        /// The forty fifth week in the year.
        /// </summary>
        FortyFifth = 1UL << 45,

        /// <summary>
        /// The forty sixth week in the year.
        /// </summary>
        FortySixth = 1UL << 46,

        /// <summary>
        /// The forty seventh week in the year.
        /// </summary>
        FortySeventh = 1UL << 47,

        /// <summary>
        /// The forty eighth week in the year.
        /// </summary>
        FortyEighth = 1UL << 48,

        /// <summary>
        /// The forty ninth week in the year.
        /// </summary>
        FortyNinth = 1UL << 49,

        /// <summary>
        /// The fiftieth week in the year.
        /// </summary>
        Fiftieth = 1UL << 50,

        /// <summary>
        /// The fifty first week in the year.
        /// </summary>
        FiftyFirst = 1UL << 51,

        /// <summary>
        /// The fifty second week in the year.
        /// </summary>
        FiftySecond = 1UL << 52,

        /// <summary>
        /// No weeks in the year.
        /// </summary>
        Never = 0,

        /// <summary>
        /// Every four weeks starting from the first week.
        /// </summary>
        EveryFourWeeks =
            First | Fifth | Ninth | Thirteenth | Seventeenth | TwentyFirst | TwentyFifth | TwentyNinth | ThirtyThird |
            ThirtySeventh | FortyFirst | FortyFifth | FortyNinth,

        /// <summary>
        /// Every two weeks starting from the first week.
        /// </summary>
        EveryTwoWeeks =
            EveryFourWeeks | Third | Seventh | Eleventh | Fifteenth | Nineteenth | TwentyThird | TwentySeventh |
            ThirtyFirst | ThirtyFifth | ThirtyNinth | FortyThird | FortySeventh | FiftyFirst,

        /// <summary>
        /// Every two weeks starting from the second week.
        /// </summary>
        EveryOtherWeek = ~EveryTwoWeeks,

        /// <summary>
        /// Every week in the year.
        /// </summary>
        Every = EveryTwoWeeks | EveryOtherWeek
    }
}