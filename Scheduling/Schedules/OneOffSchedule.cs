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
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Scheduling.Schedules
{
    /// <summary>
    /// Defines a single datetime schedule
    /// </summary>
    public class OneOffSchedule : ISchedule
    {
        /// <summary>
        /// The Schedule DateTime
        /// </summary>
        [PublicAPI]
        public readonly DateTime ScheduleDateTime;

        /// <summary>
        /// The schedules optional name.
        /// </summary>
        private readonly string _name;

        /// <inheritdoc/>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OneOffSchedule" /> class.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <param name="name">An optional name for the schedule.</param>
        public OneOffSchedule(DateTime dateTime, [CanBeNull] string name = null)
        {
            ScheduleDateTime = dateTime;
            _name = name;
        }

        /// <inheritdoc/>
        public DateTime Next(DateTime last)
        {
            return ScheduleDateTime > DateTime.Now ? ScheduleDateTime : DateTime.MaxValue;
        }

        /// <inheritdoc/>
        public ScheduleOptions Options
        {
            get { return ScheduleOptions.None; }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "Run Once at " + ScheduleDateTime;
        }
    }
}
