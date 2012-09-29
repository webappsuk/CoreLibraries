#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Copyright (c) 2012, Web Applications UK Ltd
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Web Applications UK Ltd nor the
//       names of its contributors may be used to endorse or promote products
//       derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL WEB APPLICATIONS UK LTD BE LIABLE FOR ANY
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System.Collections.Concurrent;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace WebApplications.Utilities.Logging.Performance
{
    /// <summary>
    ///   Performance counters used for operations.
    /// </summary>
    public class PerformanceCounter : PerformanceCounterHelper
    {
        /// <summary>
        /// Default counters for a category.
        /// </summary>
        [NotNull]
        private static readonly IEnumerable<CounterCreationData> _counterData = new[]
                {
                    new CounterCreationData("Count", "Total Operations Executed", PerformanceCounterType.NumberOfItems64),
                    new CounterCreationData("PerSecond", "Operations execute per second", PerformanceCounterType.RateOfCountsPerSecond64)
                };

        /// <summary>
        /// Holds all counters.
        /// </summary>
        [NotNull]
        private static readonly ConcurrentDictionary<string, PerformanceCounter> _counters =
            new ConcurrentDictionary<string, PerformanceCounter>();

        /// <summary>
        ///   Initializes a new instance of <see cref="PerformanceCounter"/>.
        /// </summary>
        /// <param name="categoryName">The performance counter's category name.</param>
        private PerformanceCounter([NotNull]string categoryName)
            : base(categoryName, _counterData)
        {
        }

        /// <summary>
        /// Gets the current operation count.
        /// </summary>
        /// <value>The count.</value>
        public long Count
        {
            get { return IsValid ? Counters[0].RawValue : 0; }
        }

        /// <summary>
        /// Gets the operations per second.
        /// </summary>
        /// <value>The count.</value>
        public float Rate
        {
            get { return IsValid ? Counters[1].NextValue() : float.NaN; }
        }

        /// <summary>
        ///   Increments the operation counters.
        /// </summary>
        public void Increment()
        {
            if (!IsValid)
                return;

            Counters[0].Increment();
            Counters[1].Increment();
        }

        /// <summary>
        /// Gets the performance counter with the specified category name.
        /// </summary>
        /// <param name="categoryName">Name of the category.</param>
        /// <returns>The performance counter.</returns>
        [NotNull]
        public static PerformanceCounter Get([NotNull]string categoryName)
        {
            return _counters.GetOrAdd(categoryName, n => new PerformanceCounter(n));
        }

        /// <summary>
        /// Creates the specified performance counter (use during installation only).
        /// </summary>
        /// <param name="categoryName">Name of the category.</param>
        /// <param name="categoryHelp">The category help.</param>
        /// <returns><see langword="true" /> if the category was created; otherwise <see langword="false" />.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <remarks><para>
        /// It is strongly recommended that new performance counter categories
        /// be created during the installation of the application, not during the execution
        /// of the application. This allows time for the operating system to refresh its list
        /// of registered performance counter categories. If the list has not been refreshed,
        /// the attempt to use the category will fail.
        ///   </para>
        ///   <para>
        /// To read performance counters in Windows Vista and later, Windows XP Professional
        /// x64 Edition, or Windows Server 2003, you must either be a member of the Performance
        /// Monitor Users group or have administrative privileges.
        ///   </para>
        ///   <para>
        /// To avoid having to elevate your privileges to access performance counters in Windows
        /// Vista and later, add yourself to the Performance Monitor Users group.
        ///   </para>
        ///   <para>
        /// In Windows Vista and later, User Account Control (UAC) determines the privileges of
        /// a user. If you are a member of the Built-in Administrators group, you are assigned
        /// two run-time access tokens: a standard user access token and an administrator access
        /// token. By default, you are in the standard user role. To execute the code that
        /// accesses performance counters, you must first elevate your privileges from standard
        /// user to administrator. You can do this when you start an application by right-clicking
        /// the application icon and indicating that you want to run as an administrator.
        ///   </para>
        ///   <para>
        /// For more information see MSDN : http://msdn.microsoft.com/EN-US/library/sb32hxtc(v=VS.110,d=hv.2).aspx
        ///   </para></remarks>
        public static bool Create([NotNull] string categoryName, [NotNull]string categoryHelp)
        {
            return Create(categoryName, categoryHelp, _counterData);
        }

        /// <summary>
        /// Deletes the specified performance counter (use during uninstall only).
        /// </summary>
        /// <param name="categoryName">Name of the category.</param>
        /// <returns><see langword="true"/> if the category no longer exists; otherwise <see langword="false"/>.</returns>
        /// <remarks><para>
        /// It is strongly recommended that performance counter categories are only removed
        /// during the removal of the application, not during the execution
        /// of the application.</para>
        ///   <para>
        /// To read performance counters in Windows Vista and later, Windows XP Professional
        /// x64 Edition, or Windows Server 2003, you must either be a member of the Performance
        /// Monitor Users group or have administrative privileges.
        ///   </para>
        ///   <para>
        /// To avoid having to elevate your privileges to access performance counters in Windows
        /// Vista and later, add yourself to the Performance Monitor Users group.
        ///   </para>
        ///   <para>
        /// In Windows Vista and later, User Account Control (UAC) determines the privileges of
        /// a user. If you are a member of the Built-in Administrators group, you are assigned
        /// two run-time access tokens: a standard user access token and an administrator access
        /// token. By default, you are in the standard user role. To execute the code that
        /// accesses performance counters, you must first elevate your privileges from standard
        /// user to administrator. You can do this when you start an application by right-clicking
        /// the application icon and indicating that you want to run as an administrator.
        ///   </para>
        ///   <para>
        /// For more information see MSDN : http://msdn.microsoft.com/EN-US/library/s55bz6c1(v=VS.110,d=hv.2).aspx
        ///   </para></remarks>
        protected static bool Delete([NotNull] string categoryName)
        {
            return Delete(categoryName, _counterData);
        }
    }
}