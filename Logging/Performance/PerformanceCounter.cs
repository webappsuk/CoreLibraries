#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities.Logging 
// Project: WebApplications.Utilities.Logging
// File: PerformanceCounter.cs
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

namespace WebApplications.Utilities.Logging.Performance
{
    /// <summary>
    ///   Performance counters used for operations.
    /// </summary>
    public class PerformanceCounter : IDisposable
    {
#if Performance
        private readonly PerformanceCounterHelper _helper;
#endif

        /// <summary>
        ///   Initializes a new instance of <see cref="PerformanceCounter"/>.
        /// </summary>
        /// <param name="categoryName">The performance counter's category name.</param>
        public PerformanceCounter(string categoryName)
        {
#if Performance
            _helper = PerformanceCounterHelper.Get(categoryName);
#endif
        }

        /// <summary>
        ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
#if Performance
            _helper.IncrementCounters();
#endif
        }
    }
}