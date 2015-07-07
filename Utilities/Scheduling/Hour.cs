#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
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