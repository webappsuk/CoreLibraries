#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: Utilities.Scheduling 
// Project: Utilities.Scheduling
// File: ScheduledActionResult.cs
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

namespace WebApplications.Utilities.Scheduling.Scheduled
{
    /// <summary>
    /// The result of a scheduled action.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    internal class ScheduledFunctionResult<TResult> : ScheduledActionResult, IScheduledFunctionResult<TResult>
    {
        /// <summary>
        /// The result.
        /// </summary>
        private readonly TResult _result;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledFunctionResult{TResult}"/> class.
        /// </summary>
        /// <param name="due">The due.</param>
        /// <param name="started">The started.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="cancelled">if set to <see langword="true"/> [cancelled].</param>
        /// <param name="result">The result.</param>
        /// <remarks></remarks>
        internal ScheduledFunctionResult(DateTime due, DateTime started, TimeSpan duration, Exception exception, bool cancelled, TResult result)
            : base(due, started, duration, exception, cancelled)
        {
            _result = result;
        }

        /// <summary>
        /// The result.
        /// </summary>
        public TResult Result
        {
            get { return _result; }
        }
    }
}