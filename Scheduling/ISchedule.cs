#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: Utilities.Scheduling 
// Project: Utilities.Scheduling
// File: Schedule.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Scheduling
{
    /// <summary>
    /// Schedule Interface
    /// </summary>
    public interface ISchedule
    {
        /// <summary>
        /// Gets the optional name.
        /// </summary>
        [CanBeNull]
        string Name { get; }
        
        /// <summary>
        /// Get's the next scheduled event.
        /// </summary>
        /// <param name="last">The last time the action was completed.</param>
        /// <returns>Next time in schedule.</returns>
        DateTime Next(DateTime last);

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <remarks></remarks>
        ScheduleOptions Options { get; }
    }
}
