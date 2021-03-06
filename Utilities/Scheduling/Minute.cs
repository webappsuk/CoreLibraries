﻿#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// This software, its object code and source code and all modifications made to
// the same (the “Software”) are, and shall at all times remain, the proprietary
// information and intellectual property rights of Web Applications (UK) Limited. 
// You are only entitled to use the Software as expressly permitted by Web
// Applications (UK) Limited within the Software Customisation and
// Licence Agreement (the “Agreement”).  Any copying, modification, decompiling,
// distribution, licensing, sale, transfer or other use of the Software other than
// as expressly permitted in the Agreement is expressly forbidden.  Web
// Applications (UK) Limited reserves its rights to take action against you and
// your employer in accordance with its contractual and common law rights
// (including injunctive relief) should you breach the terms of the Agreement or
// otherwise infringe its copyright or other intellectual property rights in the
// Software.
// 
// ©  Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
#endregion

using System;

namespace WebApplications.Utilities.Scheduling
{
    /// <summary>
    /// Enumeration of the potential minutes in a minute.
    /// </summary>
    [Flags]
    public enum Minute : ulong
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
        TwentyFourth = (ulong)1 << 24,
        /// <summary>
        /// 
        /// </summary>
        TwentyFifth = (ulong)1 << 25,
        /// <summary>
        /// 
        /// </summary>
        TwentySixth = (ulong)1 << 26,
        /// <summary>
        /// 
        /// </summary>
        TwentySeventh = (ulong)1 << 27,
        /// <summary>
        /// 
        /// </summary>
        TwentyEighth = (ulong)1 << 28,
        /// <summary>
        /// 
        /// </summary>
        TwentyNinth = (ulong)1 << 29,
        /// <summary>
        /// 
        /// </summary>
        Thirtieth = (ulong)1 << 30,
        /// <summary>
        /// 
        /// </summary>
        ThirtyFirst = (ulong)1 << 31,
        /// <summary>
        /// 
        /// </summary>
        ThirtySecond = (ulong)1 << 32,
        /// <summary>
        /// 
        /// </summary>
        ThirtyThird = (ulong)1 << 33,
        /// <summary>
        /// 
        /// </summary>
        ThirtyFourth = (ulong)1 << 34,
        /// <summary>
        /// 
        /// </summary>
        ThirtyFifth = (ulong)1 << 35,
        /// <summary>
        /// 
        /// </summary>
        ThirtySixth = (ulong)1 << 36,
        /// <summary>
        /// 
        /// </summary>
        ThirtySeventh = (ulong)1 << 37,
        /// <summary>
        /// 
        /// </summary>
        ThirtyEighth = (ulong)1 << 38,
        /// <summary>
        /// 
        /// </summary>
        ThirtyNinth = (ulong)1 << 39,
        /// <summary>
        /// 
        /// </summary>
        Fortieth = (ulong)1 << 40,
        /// <summary>
        /// 
        /// </summary>
        FortyFirst = (ulong)1 << 41,
        /// <summary>
        /// 
        /// </summary>
        FortySecond = (ulong)1 << 42,
        /// <summary>
        /// 
        /// </summary>
        FortyThird = (ulong)1 << 43,
        /// <summary>
        /// 
        /// </summary>
        FortyForth = (ulong)1 << 44,
        /// <summary>
        /// 
        /// </summary>
        FortyFifth = (ulong)1 << 45,
        /// <summary>
        /// 
        /// </summary>
        FortySixth = (ulong)1 << 46,
        /// <summary>
        /// 
        /// </summary>
        FortySeventh = (ulong)1 << 47,
        /// <summary>
        /// 
        /// </summary>
        FortyEighth = (ulong)1 << 48,
        /// <summary>
        /// 
        /// </summary>
        FortyNinth = (ulong)1 << 49,
        /// <summary>
        /// 
        /// </summary>
        Fiftieth = (ulong)1 << 50,
        /// <summary>
        /// 
        /// </summary>
        FiftyFirst = (ulong)1 << 51,
        /// <summary>
        /// 
        /// </summary>
        FiftySecond = (ulong)1 << 52,
        /// <summary>
        /// 
        /// </summary>
        FiftyThird = (ulong)1 << 53,
        /// <summary>
        /// 
        /// </summary>
        FiftyForth = (ulong)1 << 54,
        /// <summary>
        /// 
        /// </summary>
        FiftyFifth = (ulong)1 << 55,
        /// <summary>
        /// 
        /// </summary>
        FiftySixth = (ulong)1 << 56,
        /// <summary>
        /// 
        /// </summary>
        FiftySeventh = (ulong)1 << 57,
        /// <summary>
        /// 
        /// </summary>
        FiftyEighth = (ulong)1 << 58,
        /// <summary>
        /// 
        /// </summary>
        FiftyNinth = (ulong)1 << 59,

        /// <summary>
        /// 
        /// </summary>
        Never = 0,
        /// <summary>
        /// 
        /// </summary>
        Sixtyth = Zeroth,
        /// <summary>
        /// 
        /// </summary>
        EveryThirtyMinutes = Zeroth | Thirtieth,
        /// <summary>
        /// 
        /// </summary>
        EveryFifteenMinutes = EveryThirtyMinutes | Fifteenth | FortyFifth,
        /// <summary>
        /// 
        /// </summary>
        EveryTenMinutes = EveryThirtyMinutes | Tenth | Twentieth | Fortieth | Fiftieth,
        /// <summary>
        /// 
        /// </summary>
        EveryFiveMinutes = EveryTenMinutes | EveryFifteenMinutes | Fifth | TwentyFifth | ThirtyFifth | FiftyFifth,
        /// <summary>
        /// 
        /// </summary>
        EveryThreeMinutes =
                EveryFifteenMinutes | Third | Sixth | Ninth | Twelfth | Eighteenth | TwentyFirst | TwentyFourth |
                TwentySeventh | ThirtyThird | ThirtySixth | ThirtyNinth | FortySecond | FortyEighth | FiftyFirst |
                FiftyForth | FiftySeventh,
        /// <summary>
        /// 
        /// </summary>
        EveryTwoMinutes =
                EveryTenMinutes | Second | Fourth | Sixth | Eighth | Twelfth | Fourteenth | Sixteenth | Eighteenth |
                TwentySecond | TwentyFourth | TwentySixth | TwentyEighth | ThirtySecond | ThirtyFourth | ThirtySixth |
                ThirtyEighth | FortySecond | FortyForth | FortySixth | FortyEighth | FiftySecond | FiftyForth |
                FiftySixth | FiftyEighth,
        /// <summary>
        /// 
        /// </summary>
        EveryOtherMinute = ~EveryTwoMinutes,
        /// <summary>
        /// 
        /// </summary>
        Every = EveryTwoMinutes | EveryOtherMinute
    }
}