#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities.Logging 
// Project: WebApplications.Utilities.Logging
// File: PerformanceCounterHelper.cs
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
using System.Collections.Concurrent;
using System.Diagnostics;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Logging.Performance
{
    /// <summary>
    ///   Performance counters used for operations.
    /// </summary>
    internal class PerformanceCounterHelper
    {
        /// <summary>
        ///   A lookup containing the performance counters.
        /// </summary>
        private static readonly ConcurrentDictionary<string, PerformanceCounterHelper> _counters =
            new ConcurrentDictionary<string, PerformanceCounterHelper>();

        /// <summary>
        ///   Gets a value indicating whether the <see cref="PerformanceCounterHelper"/> has errored.
        /// </summary>
        public static bool Errored { get; internal set; } 

        /// <summary>
        ///   The performance counter's category.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public readonly string CategoryName;
        
        /// <summary>
        ///   Used to lock initialisation.
        /// </summary>
        private readonly object _lock = new object();
        private bool _found;
        private bool _initialised;
        private System.Diagnostics.PerformanceCounter _totalOperations;
        // Total tests executed per second
        private System.Diagnostics.PerformanceCounter _totalOperationsPerSecond;

        /// <summary>
        ///   Prevents a default instance of the <see cref="PerformanceCounterHelper"/> class from being created.
        /// </summary>
        /// <param name="categoryName">
        ///   The performance counter's <see cref="PerformanceCounterHelper.CategoryName">category name</see>.
        /// </param>
        private PerformanceCounterHelper([NotNull]string categoryName)
        {
            CategoryName = categoryName;
        }

        /// <summary>
        ///   Initialises this instance.
        /// </summary>
        [Conditional("Performance")]
        private void Initialise()
        {
            // Only initialise once.
            if (_initialised)
                return;

            // Only one thread can initialise.
            lock (_lock)
            {
                // Recheck
                if (_initialised)
                    return;

                // If we already got an access denied error, don't keep trying.
                if (Errored)
                    return;

                _initialised = true;

                // Set up the performance counter(s)
                try
                {
                    if (!PerformanceCounterCategory.Exists(CategoryName))
                    {
                        Log.Add(
                            Resources.PerformanceCounterHelper_Initialize_CategoryDoesNotExist,
                            LogLevel.SystemNotification,
                            CategoryName);
                        return;
                    }

                    _totalOperations = new System.Diagnostics.PerformanceCounter
                                           {
                                               CategoryName = CategoryName,
                                               CounterName = "Total Operations Executed",
                                               MachineName = ".",
                                               ReadOnly = false
                                           };

                    _totalOperationsPerSecond = new System.Diagnostics.PerformanceCounter
                                                    {
                                                        CategoryName = CategoryName,
                                                        CounterName = "Operations Executed / sec",
                                                        MachineName = ".",
                                                        ReadOnly = false
                                                    };
                    _found = true;
                }
                catch (UnauthorizedAccessException unauthorizedAccessException)
                {
                    // There is a potential race condition here but it's non-critical.
                    // Better to avoid the locking overhead on every access to Errored.
                    if (!Errored)
                    {
                        Errored = true;
                        _found = false;
                        // Create error but don't throw.
                        new LoggingException(unauthorizedAccessException,
                                             Resources.PerformanceCounterHelper_Initialize_ProcessDoesNotHaveAccess,
                                             LogLevel.SystemNotification);
                    }
                }
                catch (Exception exception)
                {
                    Errored = true;
                    _found = false;
                    // Create error but don't throw.
                    new LoggingException(exception,
                        Resources.PerformanceCounterHelper_Initialize_UnhandledExceptionOccurred,
                        LogLevel.SystemNotification,
                        CategoryName);
                }
            }
        }

        /// <summary>
        ///   Gets the performance counter with the specified category name.
        ///   A counter is added to the lookup if no match is found.
        /// </summary>
        /// <param name="categoryName">
        ///   The performance counter's <see cref="PerformanceCounterHelper.CategoryName">category name</see>.
        /// </param>
        /// <returns>The added/retrieved counter with the <paramref name="categoryName"/> specified.</returns>
        public static PerformanceCounterHelper Get([NotNull]string categoryName)
        {
            PerformanceCounterHelper counterHelper = _counters.GetOrAdd(
                categoryName, n => new PerformanceCounterHelper(n));

            // Ensure initialised.
            counterHelper.Initialise();
            return counterHelper;
        }

        /// <summary>
        ///   Increments the operation counters.
        /// </summary>
        [Conditional("Performance")]
        public void IncrementCounters()
        {
            if (!_found ||
                Errored)
                return;

            _totalOperations.Increment();
            _totalOperationsPerSecond.Increment();
        }
    }
}