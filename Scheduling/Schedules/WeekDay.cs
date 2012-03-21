#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: Utilities.Scheduling 
// Project: Utilities.Scheduling
// File: WeekDay.cs
// 
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
// © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
#endregion

using System;

namespace WebApplications.Utilities.Scheduling.Schedules
{
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum WeekDay
    {
        /// <summary>
        /// 
        /// </summary>
        Sunday = 1,

        /// <summary>
        /// 
        /// </summary>
        Monday = 1 << 1,

        /// <summary>
        /// 
        /// </summary>
        Tuesday = 1 << 2,

        /// <summary>
        /// 
        /// </summary>
        Wednesday = 1 << 3,

        /// <summary>
        /// 
        /// </summary>
        Thursday = 1 << 4,

        /// <summary>
        /// 
        /// </summary>
        Friday = 1 << 5,

        /// <summary>
        /// 
        /// </summary>
        Saturday = 1 << 6,

        /// <summary>
        /// 
        /// </summary>
        Never = 0,

        /// <summary>
        /// 
        /// </summary>
        Weekend = Saturday | Sunday,

        /// <summary>
        /// 
        /// </summary>
        WeekDay = Monday | Tuesday | Wednesday | Thursday | Friday,

        /// <summary>
        /// 
        /// </summary>
        Every = Weekend | WeekDay
    }
}