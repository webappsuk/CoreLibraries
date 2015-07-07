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
    /// Enumeration of the potential hours in a day.
    /// </summary>
    [Flags]
    [PublicAPI]
    public enum Hour
    {
        /// <summary>
        /// The zeroth hour in the day.
        /// </summary>
        Zeroth = 1,

        /// <summary>
        /// The first hour in the day.
        /// </summary>
        First = 1 << 1,

        /// <summary>
        /// The second hour in the day.
        /// </summary>
        Second = 1 << 2,

        /// <summary>
        /// The third hour in the day.
        /// </summary>
        Third = 1 << 3,

        /// <summary>
        /// The fourth hour in the day.
        /// </summary>
        Fourth = 1 << 4,

        /// <summary>
        /// The fifth hour in the day.
        /// </summary>
        Fifth = 1 << 5,

        /// <summary>
        /// The sixth hour in the day.
        /// </summary>
        Sixth = 1 << 6,

        /// <summary>
        /// The seventh hour in the day.
        /// </summary>
        Seventh = 1 << 7,

        /// <summary>
        /// The eighth hour in the day.
        /// </summary>
        Eighth = 1 << 8,

        /// <summary>
        /// The ninth hour in the day.
        /// </summary>
        Ninth = 1 << 9,

        /// <summary>
        /// The tenth hour in the day.
        /// </summary>
        Tenth = 1 << 10,

        /// <summary>
        /// The eleventh hour in the day.
        /// </summary>
        Eleventh = 1 << 11,

        /// <summary>
        /// The twelfth hour in the day.
        /// </summary>
        Twelfth = 1 << 12,

        /// <summary>
        /// The thirteenth hour in the day.
        /// </summary>
        Thirteenth = 1 << 13,

        /// <summary>
        /// The fourteenth hour in the day.
        /// </summary>
        Fourteenth = 1 << 14,

        /// <summary>
        /// The fifteenth hour in the day.
        /// </summary>
        Fifteenth = 1 << 15,

        /// <summary>
        /// The sixteenth hour in the day.
        /// </summary>
        Sixteenth = 1 << 16,

        /// <summary>
        /// The seventeenth hour in the day.
        /// </summary>
        Seventeenth = 1 << 17,

        /// <summary>
        /// The eighteenth hour in the day.
        /// </summary>
        Eighteenth = 1 << 18,

        /// <summary>
        /// The nineteenth hour in the day.
        /// </summary>
        Nineteenth = 1 << 19,

        /// <summary>
        /// The twentieth hour in the day.
        /// </summary>
        Twentieth = 1 << 20,

        /// <summary>
        /// The twenty first hour in the day.
        /// </summary>
        TwentyFirst = 1 << 21,

        /// <summary>
        /// The twenty second hour in the day.
        /// </summary>
        TwentySecond = 1 << 22,

        /// <summary>
        /// The twenty third hour in the day.
        /// </summary>
        TwentyThird = 1 << 23,

        /// <summary>
        /// No hours in the day.
        /// </summary>
        Never = 0,

        /// <summary>
        /// The twenty fourth hour in the day.
        /// </summary>
        TwentyFourth = Zeroth,

        /// <summary>
        /// Every twelve hours starting from the zeroth hour.
        /// </summary>
        EveryTwelveHours = Zeroth | Twelfth,

        /// <summary>
        /// Every six hours starting from the zeroth hour.
        /// </summary>
        EverySixHours = EveryTwelveHours | Sixth | Eighteenth,

        /// <summary>
        /// Every four hours starting from the zeroth hour.
        /// </summary>
        EveryFourHours = EveryTwelveHours | Fourth | Eighth | Sixteenth | Twentieth,

        /// <summary>
        /// Wvery three hours starting from the zeroth hour.
        /// </summary>
        EveryThreeHours = EverySixHours | Third | Ninth | Fifteenth | TwentyFirst,

        /// <summary>
        /// Every two hours starting from the zeroth hour.
        /// </summary>
        EveryTwoHours = EveryFourHours | Second | Sixth | Tenth | Fourteenth | Eighteenth | TwentySecond,

        /// <summary>
        /// Every two hours starting from the first hour.
        /// </summary>
        EveryOtherHour = ~EveryTwoHours,

        /// <summary>
        /// Every hour in the day.
        /// </summary>
        Every = EveryTwoHours | EveryOtherHour
    }
}