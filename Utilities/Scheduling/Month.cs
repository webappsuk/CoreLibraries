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
    public enum Month : ulong
    {
        /// <summary>
        /// 
        /// </summary>
        January = 1 << 1,
        /// <summary>
        /// 
        /// </summary>
        February = 1 << 2,
        /// <summary>
        /// 
        /// </summary>
        March = 1 << 3,
        /// <summary>
        /// 
        /// </summary>
        April = 1 << 4,
        /// <summary>
        /// 
        /// </summary>
        May = 1 << 5,
        /// <summary>
        /// 
        /// </summary>
        June = 1 << 6,
        /// <summary>
        /// 
        /// </summary>
        July = 1 << 7,
        /// <summary>
        /// 
        /// </summary>
        August = 1 << 8,
        /// <summary>
        /// 
        /// </summary>
        September = 1 << 9,
        /// <summary>
        /// 
        /// </summary>
        October = 1 << 10,
        /// <summary>
        /// 
        /// </summary>
        November = 1 << 11,
        /// <summary>
        /// 
        /// </summary>
        December = 1 << 12,

        /// <summary>
        /// 
        /// </summary>
        Never = 0,
        /// <summary>
        /// 
        /// </summary>
        TwiceAYear = January | July,
        /// <summary>
        /// 
        /// </summary>
        FourTimesAYear = TwiceAYear | May | November,
        /// <summary>
        /// 
        /// </summary>
        ThreeTimesAYear = January | May | September,
        /// <summary>
        /// 
        /// </summary>
        EveryTwoMonths = FourTimesAYear | March | September,
        /// <summary>
        /// 
        /// </summary>
        EveryOtherMonth = ~EveryTwoMonths,
        /// <summary>
        /// 
        /// </summary>
        Every = EveryTwoMonths | EveryOtherMonth
    }
}