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
    /// Enumeration of the potential days in a month.
    /// </summary>
    [Flags]
    [PublicAPI]
    public enum Day : uint
    {
        /// <summary>
        /// The first day of the month.
        /// </summary>
        First = 1U << 1,

        /// <summary>
        /// The second day of the month.
        /// </summary>
        Second = 1U << 2,

        /// <summary>
        /// The third day of the month.
        /// </summary>
        Third = 1U << 3,

        /// <summary>
        /// The fourth day of the month.
        /// </summary>
        Fourth = 1U << 4,

        /// <summary>
        /// The fifth day of the month.
        /// </summary>
        Fifth = 1U << 5,

        /// <summary>
        /// The sixth day of the month.
        /// </summary>
        Sixth = 1U << 6,

        /// <summary>
        /// The seventh day of the month.
        /// </summary>
        Seventh = 1U << 7,

        /// <summary>
        /// The eighth day of the month.
        /// </summary>
        Eighth = 1U << 8,

        /// <summary>
        /// The ninth day of the month.
        /// </summary>
        Ninth = 1U << 9,

        /// <summary>
        /// The tenth day of the month.
        /// </summary>
        Tenth = 1U << 10,

        /// <summary>
        /// The eleventh day of the month.
        /// </summary>
        Eleventh = 1U << 11,

        /// <summary>
        /// The twelfth day of the month.
        /// </summary>
        Twelfth = 1U << 12,

        /// <summary>
        /// The thirteenth day of the month.
        /// </summary>
        Thirteenth = 1U << 13,

        /// <summary>
        /// The fourteenth day of the month.
        /// </summary>
        Fourteenth = 1U << 14,

        /// <summary>
        /// The fifteenth day of the month.
        /// </summary>
        Fifteenth = 1U << 15,

        /// <summary>
        /// The sixteenth day of the month.
        /// </summary>
        Sixteenth = 1U << 16,

        /// <summary>
        /// The seventeenth day of the month.
        /// </summary>
        Seventeenth = 1U << 17,

        /// <summary>
        /// The eighteenth day of the month.
        /// </summary>
        Eighteenth = 1U << 18,

        /// <summary>
        /// The nineteenth day of the month.
        /// </summary>
        Nineteenth = 1U << 19,

        /// <summary>
        /// The twentieth day of the month.
        /// </summary>
        Twentieth = 1U << 20,

        /// <summary>
        /// The twenty first day of the month.
        /// </summary>
        TwentyFirst = 1U << 21,

        /// <summary>
        /// The twenty second day of the month.
        /// </summary>
        TwentySecond = 1U << 22,

        /// <summary>
        /// The twenty third day of the month.
        /// </summary>
        TwentyThird = 1U << 23,

        /// <summary>
        /// The twenty fourth day of the month.
        /// </summary>
        TwentyFourth = 1U << 24,

        /// <summary>
        /// The twenty fifth day of the month.
        /// </summary>
        TwentyFifth = 1U << 25,

        /// <summary>
        /// The twenty sixth day of the month.
        /// </summary>
        TwentySixth = 1U << 26,

        /// <summary>
        /// The twenty seventh day of the month.
        /// </summary>
        TwentySeventh = 1U << 27,

        /// <summary>
        /// The twenty eighth day of the month.
        /// </summary>
        TwentyEighth = 1U << 28,

        /// <summary>
        /// The twenty ninth day of the month.
        /// </summary>
        TwentyNinth = 1U << 29,

        /// <summary>
        /// The thirtieth day of the month.
        /// </summary>
        Thirtieth = 1U << 30,

        /// <summary>
        /// The thirty first day of the month.
        /// </summary>
        ThirtyFirst = 1U << 31,

        /// <summary>
        /// No day of the month.
        /// </summary>
        Never = 0,

        /// <summary>
        /// Every day of the month.
        /// </summary>
        Every =
            First | Second | Third | Fourth | Fifth | Sixth | Seventh | Eighth | Ninth | Tenth | Eleventh | Twelfth |
            Thirteenth | Fourteenth | Fifteenth | Sixteenth | Seventeenth | Eighteenth | Nineteenth | Twentieth |
            TwentyFirst | TwentySecond | TwentyThird | TwentyFourth | TwentyFifth | TwentySixth | TwentySeventh |
            TwentyEighth | TwentyNinth | Thirtieth | ThirtyFirst
    }
}