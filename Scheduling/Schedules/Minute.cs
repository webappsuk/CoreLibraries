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
    /// Enumeration of the potential minutes in an hour.
    /// </summary>
    [Flags]
    [PublicAPI]
    public enum Minute : ulong
    {
        /// <summary>
        /// The zeroth minute in the hour.
        /// </summary>
        Zeroth = 1UL,

        /// <summary>
        /// The first minute in the hour.
        /// </summary>
        First = 1UL << 1,

        /// <summary>
        /// The second minute in the hour.
        /// </summary>
        Second = 1UL << 2,

        /// <summary>
        /// The third minute in the hour.
        /// </summary>
        Third = 1UL << 3,

        /// <summary>
        /// The fourth minute in the hour.
        /// </summary>
        Fourth = 1UL << 4,

        /// <summary>
        /// The fifth minute in the hour.
        /// </summary>
        Fifth = 1UL << 5,

        /// <summary>
        /// The sixth minute in the hour.
        /// </summary>
        Sixth = 1UL << 6,

        /// <summary>
        /// The seventh minute in the hour.
        /// </summary>
        Seventh = 1UL << 7,

        /// <summary>
        /// The eighth minute in the hour.
        /// </summary>
        Eighth = 1UL << 8,

        /// <summary>
        /// The ninth minute in the hour.
        /// </summary>
        Ninth = 1UL << 9,

        /// <summary>
        /// The tenth minute in the hour.
        /// </summary>
        Tenth = 1UL << 10,

        /// <summary>
        /// The eleventh minute in the hour.
        /// </summary>
        Eleventh = 1UL << 11,

        /// <summary>
        /// The twelfth minute in the hour.
        /// </summary>
        Twelfth = 1UL << 12,

        /// <summary>
        /// The thirteenth minute in the hour.
        /// </summary>
        Thirteenth = 1UL << 13,

        /// <summary>
        /// The fourteenth minute in the hour.
        /// </summary>
        Fourteenth = 1UL << 14,

        /// <summary>
        /// The fifteenth minute in the hour.
        /// </summary>
        Fifteenth = 1UL << 15,

        /// <summary>
        /// The sixteenth minute in the hour.
        /// </summary>
        Sixteenth = 1UL << 16,

        /// <summary>
        /// The seventeenth minute in the hour.
        /// </summary>
        Seventeenth = 1UL << 17,

        /// <summary>
        /// The eighteenth minute in the hour.
        /// </summary>
        Eighteenth = 1UL << 18,

        /// <summary>
        /// The nineteenth minute in the hour.
        /// </summary>
        Nineteenth = 1UL << 19,

        /// <summary>
        /// The twentieth minute in the hour.
        /// </summary>
        Twentieth = 1UL << 20,

        /// <summary>
        /// The twenty first minute in the hour.
        /// </summary>
        TwentyFirst = 1UL << 21,

        /// <summary>
        /// The twenty second minute in the hour.
        /// </summary>
        TwentySecond = 1UL << 22,

        /// <summary>
        /// The twenty third minute in the hour.
        /// </summary>
        TwentyThird = 1UL << 23,

        /// <summary>
        /// The twenty fourth minute in the hour.
        /// </summary>
        TwentyFourth = 1UL << 24,

        /// <summary>
        /// The twenty fifth minute in the hour.
        /// </summary>
        TwentyFifth = 1UL << 25,

        /// <summary>
        /// The twenty sixth minute in the hour.
        /// </summary>
        TwentySixth = 1UL << 26,

        /// <summary>
        /// The twenty seventh minute in the hour.
        /// </summary>
        TwentySeventh = 1UL << 27,

        /// <summary>
        /// The twenty eighth minute in the hour.
        /// </summary>
        TwentyEighth = 1UL << 28,

        /// <summary>
        /// The twenty ninth minute in the hour.
        /// </summary>
        TwentyNinth = 1UL << 29,

        /// <summary>
        /// The thirtieth minute in the hour.
        /// </summary>
        Thirtieth = 1UL << 30,

        /// <summary>
        /// The thirty first minute in the hour.
        /// </summary>
        ThirtyFirst = 1UL << 31,

        /// <summary>
        /// The thirty second minute in the hour.
        /// </summary>
        ThirtySecond = 1UL << 32,

        /// <summary>
        /// The thirty third minute in the hour.
        /// </summary>
        ThirtyThird = 1UL << 33,

        /// <summary>
        /// The thirty fourth minute in the hour.
        /// </summary>
        ThirtyFourth = 1UL << 34,

        /// <summary>
        /// The thirty fifth minute in the hour.
        /// </summary>
        ThirtyFifth = 1UL << 35,

        /// <summary>
        /// The thirty sixth minute in the hour.
        /// </summary>
        ThirtySixth = 1UL << 36,

        /// <summary>
        /// The thirty seventh minute in the hour.
        /// </summary>
        ThirtySeventh = 1UL << 37,

        /// <summary>
        /// The thirty eighth minute in the hour.
        /// </summary>
        ThirtyEighth = 1UL << 38,

        /// <summary>
        /// The thirty ninth minute in the hour.
        /// </summary>
        ThirtyNinth = 1UL << 39,

        /// <summary>
        /// The fortieth minute in the hour.
        /// </summary>
        Fortieth = 1UL << 40,

        /// <summary>
        /// The forty first minute in the hour.
        /// </summary>
        FortyFirst = 1UL << 41,

        /// <summary>
        /// The forty second minute in the hour.
        /// </summary>
        FortySecond = 1UL << 42,

        /// <summary>
        /// The forty third minute in the hour.
        /// </summary>
        FortyThird = 1UL << 43,

        /// <summary>
        /// The forty forth minute in the hour.
        /// </summary>
        FortyForth = 1UL << 44,

        /// <summary>
        /// The forty fifth minute in the hour.
        /// </summary>
        FortyFifth = 1UL << 45,

        /// <summary>
        /// The forty sixth minute in the hour.
        /// </summary>
        FortySixth = 1UL << 46,

        /// <summary>
        /// The forty seventh minute in the hour.
        /// </summary>
        FortySeventh = 1UL << 47,

        /// <summary>
        /// The forty eighth minute in the hour.
        /// </summary>
        FortyEighth = 1UL << 48,

        /// <summary>
        /// The forty ninth minute in the hour.
        /// </summary>
        FortyNinth = 1UL << 49,

        /// <summary>
        /// The fiftieth minute in the hour.
        /// </summary>
        Fiftieth = 1UL << 50,

        /// <summary>
        /// The fifty first minute in the hour.
        /// </summary>
        FiftyFirst = 1UL << 51,

        /// <summary>
        /// The fifty second minute in the hour.
        /// </summary>
        FiftySecond = 1UL << 52,

        /// <summary>
        /// The fifty third minute in the hour.
        /// </summary>
        FiftyThird = 1UL << 53,

        /// <summary>
        /// The fifty forth minute in the hour.
        /// </summary>
        FiftyForth = 1UL << 54,

        /// <summary>
        /// The fifty fifth minute in the hour.
        /// </summary>
        FiftyFifth = 1UL << 55,

        /// <summary>
        /// The fifty sixth minute in the hour.
        /// </summary>
        FiftySixth = 1UL << 56,

        /// <summary>
        /// The fifty seventh minute in the hour.
        /// </summary>
        FiftySeventh = 1UL << 57,

        /// <summary>
        /// The fifty eighth minute in the hour.
        /// </summary>
        FiftyEighth = 1UL << 58,

        /// <summary>
        /// The fifty ninth minute in the hour.
        /// </summary>
        FiftyNinth = 1UL << 59,

        /// <summary>
        /// No minutes in the hour.
        /// </summary>
        Never = 0,

        /// <summary>
        /// The sixtyth minute in the hour.
        /// </summary>
        Sixtyth = Zeroth,

        /// <summary>
        /// Every thirty minutes starting from the zeroth minute.
        /// </summary>
        EveryThirtyMinutes = Zeroth | Thirtieth,

        /// <summary>
        /// Every fifteen minutes starting from the zeroth minute.
        /// </summary>
        EveryFifteenMinutes = EveryThirtyMinutes | Fifteenth | FortyFifth,

        /// <summary>
        /// Every ten minutes starting from the zeroth minute.
        /// </summary>
        EveryTenMinutes = EveryThirtyMinutes | Tenth | Twentieth | Fortieth | Fiftieth,

        /// <summary>
        /// Every five minutes starting from the zeroth minute.
        /// </summary>
        EveryFiveMinutes = EveryTenMinutes | EveryFifteenMinutes | Fifth | TwentyFifth | ThirtyFifth | FiftyFifth,

        /// <summary>
        /// Every three minutes starting from the zeroth minute.
        /// </summary>
        EveryThreeMinutes =
            EveryFifteenMinutes | Third | Sixth | Ninth | Twelfth | Eighteenth | TwentyFirst | TwentyFourth |
            TwentySeventh | ThirtyThird | ThirtySixth | ThirtyNinth | FortySecond | FortyEighth | FiftyFirst |
            FiftyForth | FiftySeventh,

        /// <summary>
        /// Every two minutes starting from the zeroth minute.
        /// </summary>
        EveryTwoMinutes =
            EveryTenMinutes | Second | Fourth | Sixth | Eighth | Twelfth | Fourteenth | Sixteenth | Eighteenth |
            TwentySecond | TwentyFourth | TwentySixth | TwentyEighth | ThirtySecond | ThirtyFourth | ThirtySixth |
            ThirtyEighth | FortySecond | FortyForth | FortySixth | FortyEighth | FiftySecond | FiftyForth |
            FiftySixth | FiftyEighth,

        /// <summary>
        /// Every two minutes starting from the first minute.
        /// </summary>
        EveryOtherMinute = ~EveryTwoMinutes,

        /// <summary>
        /// Every minute in the hour.
        /// </summary>
        Every = EveryTwoMinutes | EveryOtherMinute
    }
}