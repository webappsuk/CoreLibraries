#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities.Logging 
// Project: WebApplications.Utilities.Logging
// File: RequiresPerformanceCounter.cs
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
    ///   An attribute added to assemblies to indicate a performance counter is required. This attribute is added
    ///   automatically by the performance system and should not be added manually.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public class RequiresPerformanceCounterAttribute : Attribute
    {
        /// <summary>
        ///   The performance counter category name.
        /// </summary>
        public readonly string CategoryName;

        /// <summary>
        ///   Whether the performance counter is a timer.
        /// </summary>
        public readonly bool IsTimer;

        /// <summary>
        ///   Initializes a new instance of the <see cref="RequiresPerformanceCounterAttribute"/> class.
        /// </summary>
        /// <param name="categoryName">The name of the category.</param>
        /// <param name="isTimer">If set to <see langword="true"/> the counter is a timer.</param>
        public RequiresPerformanceCounterAttribute(string categoryName, bool isTimer)
        {
            CategoryName = categoryName;
            IsTimer = isTimer;
        }
    }
}