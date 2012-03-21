#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Solution: WebApplications.Utilities.Scheduling 
// Project: WebApplications.Utilities.Scheduling
// File: SchedulableAction.cs
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
// © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
#endregion

using System;
using JetBrains.Annotations;
using WebApplications.Utilities.Scheduling.ScheduledFunctions;

namespace WebApplications.Utilities.Scheduling.Schedulable
{
    /// <summary>
    /// Creates a schedulable action from an <see cref="Action"/>.
    /// </summary>
    /// <remarks></remarks>
    public class SchedulableAction : SchedulableActionBase
    {
        /// <summary>
        /// The action.
        /// </summary>
        [NotNull]
        private readonly Action _action;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulableAction"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <remarks></remarks>
        public SchedulableAction([NotNull]Action action)
        {
            _action = action;
        }

        /// <inheritdoc/>
        public override void Execute()
        {
            _action();
        }
    }
}