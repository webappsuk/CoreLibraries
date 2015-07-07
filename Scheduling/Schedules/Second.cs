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
    /// Enumeration of the potential seconds in a minute.
    /// </summary>
    [Flags]
    [PublicAPI]
    public enum Second : ulong
    {
        /// <summary>
        /// The zeroth second in the minute.
        /// </summary>
        Zeroth = 1UL,

        /// <summary>
        /// The first second in the minute.
        /// </summary>
        First = 1UL << 1,

        /// <summary>
        /// The second second in the minute.
        /// </summary>
        Second = 1UL << 2,

        /// <summary>
        /// The third second in the minute.
        /// </summary>
        Third = 1UL << 3,

        /// <summary>
        /// The fourth second in the minute.
        /// </summary>
        Fourth = 1UL << 4,

        /// <summary>
        /// The fifth second in the minute.
        /// </summary>
        Fifth = 1UL << 5,

        /// <summary>
        /// The sixth second in the minute.
        /// </summary>
        Sixth = 1UL << 6,

        /// <summary>
        /// The seventh second in the minute.
        /// </summary>
        Seventh = 1UL << 7,

        /// <summary>
        /// The eighth second in the minute.
        /// </summary>
        Eighth = 1UL << 8,

        /// <summary>
        /// The ninth second in the minute.
        /// </summary>
        Ninth = 1UL << 9,

        /// <summary>
        /// The tenth second in the minute.
        /// </summary>
        Tenth = 1UL << 10,

        /// <summary>
        /// The eleventh second in the minute.
        /// </summary>
        Eleventh = 1UL << 11,

        /// <summary>
        /// The twelfth second in the minute.
        /// </summary>
        Twelfth = 1UL << 12,

        /// <summary>
        /// The thirteenth second in the minute.
        /// </summary>
        Thirteenth = 1UL << 13,

        /// <summary>
        /// The fourteenth second in the minute.
        /// </summary>
        Fourteenth = 1UL << 14,

        /// <summary>
        /// The fifteenth second in the minute.
        /// </summary>
        Fifteenth = 1UL << 15,

        /// <summary>
        /// The sixteenth second in the minute.
        /// </summary>
        Sixteenth = 1UL << 16,

        /// <summary>
        /// The seventeenth second in the minute.
        /// </summary>
        Seventeenth = 1UL << 17,

        /// <summary>
        /// The eighteenth second in the minute.
        /// </summary>
        Eighteenth = 1UL << 18,

        /// <summary>
        /// The nineteenth second in the minute.
        /// </summary>
        Nineteenth = 1UL << 19,

        /// <summary>
        /// The twentieth second in the minute.
        /// </summary>
        Twentieth = 1UL << 20,

        /// <summary>
        /// The twenty first second in the minute.
        /// </summary>
        TwentyFirst = 1UL << 21,

        /// <summary>
        /// The twenty second second in the minute.
        /// </summary>
        TwentySecond = 1UL << 22,

        /// <summary>
        /// The twenty third second in the minute.
        /// </summary>
        TwentyThird = 1UL << 23,

        /// <summary>
        /// The twenty fourth second in the minute.
        /// </summary>
        TwentyFourth = 1UL << 24,

        /// <summary>
        /// The twenty fifth second in the minute.
        /// </summary>
        TwentyFifth = 1UL << 25,

        /// <summary>
        /// The twenty sixth second in the minute.
        /// </summary>
        TwentySixth = 1UL << 26,

        /// <summary>
        /// The twenty seventh second in the minute.
        /// </summary>
        TwentySeventh = 1UL << 27,

        /// <summary>
        /// The twenty eighth second in the minute.
        /// </summary>
        TwentyEighth = 1UL << 28,

        /// <summary>
        /// The twenty ninth second in the minute.
        /// </summary>
        TwentyNinth = 1UL << 29,

        /// <summary>
        /// The thirtieth second in the minute.
        /// </summary>
        Thirtieth = 1UL << 30,

        /// <summary>
        /// The thirty first second in the minute.
        /// </summary>
        ThirtyFirst = 1UL << 31,

        /// <summary>
        /// The thirty second second in the minute.
        /// </summary>
        ThirtySecond = 1UL << 32,

        /// <summary>
        /// The thirty third second in the minute.
        /// </summary>
        ThirtyThird = 1UL << 33,

        /// <summary>
        /// The thirty fourth second in the minute.
        /// </summary>
        ThirtyFourth = 1UL << 34,

        /// <summary>
        /// The thirty fifth second in the minute.
        /// </summary>
        ThirtyFifth = 1UL << 35,

        /// <summary>
        /// The thirty sixth second in the minute.
        /// </summary>
        ThirtySixth = 1UL << 36,

        /// <summary>
        /// The thirty seventh second in the minute.
        /// </summary>
        ThirtySeventh = 1UL << 37,

        /// <summary>
        /// The thirty eighth second in the minute.
        /// </summary>
        ThirtyEighth = 1UL << 38,

        /// <summary>
        /// The thirty ninth second in the minute.
        /// </summary>
        ThirtyNinth = 1UL << 39,

        /// <summary>
        /// The fortieth second in the minute.
        /// </summary>
        Fortieth = 1UL << 40,

        /// <summary>
        /// The forty first second in the minute.
        /// </summary>
        FortyFirst = 1UL << 41,

        /// <summary>
        /// The forty second second in the minute.
        /// </summary>
        FortySecond = 1UL << 42,

        /// <summary>
        /// The forty third second in the minute.
        /// </summary>
        FortyThird = 1UL << 43,

        /// <summary>
        /// The forty forth second in the minute.
        /// </summary>
        FortyForth = 1UL << 44,

        /// <summary>
        /// The forty fifth second in the minute.
        /// </summary>
        FortyFifth = 1UL << 45,

        /// <summary>
        /// The forty sixth second in the minute.
        /// </summary>
        FortySixth = 1UL << 46,

        /// <summary>
        /// The forty seventh second in the minute.
        /// </summary>
        FortySeventh = 1UL << 47,

        /// <summary>
        /// The forty eighth second in the minute.
        /// </summary>
        FortyEighth = 1UL << 48,

        /// <summary>
        /// The forty ninth second in the minute.
        /// </summary>
        FortyNinth = 1UL << 49,

        /// <summary>
        /// The fiftieth second in the minute.
        /// </summary>
        Fiftieth = 1UL << 50,

        /// <summary>
        /// The fifty first second in the minute.
        /// </summary>
        FiftyFirst = 1UL << 51,

        /// <summary>
        /// The fifty second second in the minute.
        /// </summary>
        FiftySecond = 1UL << 52,

        /// <summary>
        /// The fifty third second in the minute.
        /// </summary>
        FiftyThird = 1UL << 53,

        /// <summary>
        /// The fifty forth second in the minute.
        /// </summary>
        FiftyForth = 1UL << 54,

        /// <summary>
        /// The fifty fifth second in the minute.
        /// </summary>
        FiftyFifth = 1UL << 55,

        /// <summary>
        /// The fifty sixth second in the minute.
        /// </summary>
        FiftySixth = 1UL << 56,

        /// <summary>
        /// The fifty seventh second in the minute.
        /// </summary>
        FiftySeventh = 1UL << 57,

        /// <summary>
        /// The fifty eighth second in the minute.
        /// </summary>
        FiftyEighth = 1UL << 58,

        /// <summary>
        /// The fifty ninth second in the minute.
        /// </summary>
        FiftyNinth = 1UL << 59,

        /// <summary>
        /// No seconds in the minute.
        /// </summary>
        Never = 0,

        /// <summary>
        /// The sixtyth second in the minute.
        /// </summary>
        Sixtyth = Zeroth,

        /// <summary>
        /// Every thirty seconds starting from the zeroth second.
        /// </summary>
        EveryThirtySeconds = Zeroth | Thirtieth,

        /// <summary>
        /// Every fifteen seconds starting from the zeroth second.
        /// </summary>
        EveryFifteenSeconds = EveryThirtySeconds | Fifteenth | FortyFifth,

        /// <summary>
        /// Every ten seconds starting from the zeroth second.
        /// </summary>
        EveryTenSeconds = EveryThirtySeconds | Tenth | Twentieth | Fortieth | Fiftieth,

        /// <summary>
        /// Every five seconds starting from the zeroth second.
        /// </summary>
        EveryFiveSeconds = EveryTenSeconds | EveryFifteenSeconds | Fifth | TwentyFifth | ThirtyFifth | FiftyFifth,

        /// <summary>
        /// Every three seconds starting from the zeroth second.
        /// </summary>
        EveryThreeSeconds =
            EveryFifteenSeconds | Third | Sixth | Ninth | Twelfth | Eighteenth | TwentyFirst | TwentyFourth |
            TwentySeventh | ThirtyThird | ThirtySixth | ThirtyNinth | FortySecond | FortyEighth | FiftyFirst |
            FiftyForth | FiftySeventh,

        /// <summary>
        /// Every two seconds starting from the zeroth second.
        /// </summary>
        EveryTwoSeconds =
            EveryTenSeconds | Second | Fourth | Sixth | Eighth | Twelfth | Fourteenth | Sixteenth | Eighteenth |
            TwentySecond | TwentyFourth | TwentySixth | TwentyEighth | ThirtySecond | ThirtyFourth | ThirtySixth |
            ThirtyEighth | FortySecond | FortyForth | FortySixth | FortyEighth | FiftySecond | FiftyForth |
            FiftySixth | FiftyEighth,

        /// <summary>
        /// Every two seconds starting from the first second.
        /// </summary>
        EveryOtherSecond = ~EveryTwoSeconds,

        /// <summary>
        /// Every second in the minute.
        /// </summary>
        Every = EveryTwoSeconds | EveryOtherSecond
    }
}