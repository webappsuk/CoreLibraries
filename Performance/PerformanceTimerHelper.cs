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

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Logging.Performance
{
    /// <summary>
    ///   Performance timers for operations.
    /// </summary>
    internal class PerformanceTimerHelper
    {
        /// <summary>
        ///   A lookup containing the timer counters.
        /// </summary>
        private static readonly ConcurrentDictionary<string, PerformanceTimerHelper> _counters =
            new ConcurrentDictionary<string, PerformanceTimerHelper>();

        /// <summary>
        ///   The timer counter category name.
        /// </summary>
        [NotNull] [UsedImplicitly] public readonly string CategoryName;

        /// <summary>
        ///   The length of <see cref="TimeSpan">time</see> for an operation before logging an error.
        /// </summary>
        [UsedImplicitly] public readonly TimeSpan CriticalTimeSpan;

        /// <summary>
        ///   The length of <see cref="TimeSpan">time</see> for an operation before logging a warning.
        /// </summary>
        [UsedImplicitly] public readonly TimeSpan WarningTimeSpan;

        /// <summary>
        ///   Used to lock initialisation.
        /// </summary>
        private readonly object _lock = new object();

        // Total number of tests executed
        // Average test duration
        private System.Diagnostics.PerformanceCounter _averageOperationDuration;
        // Average test duration base
        private System.Diagnostics.PerformanceCounter _averageOperationDurationBase;
        private bool _found;
        // Average test duration base
        private bool _initialised;
        private System.Diagnostics.PerformanceCounter _totalCriticals;
        private System.Diagnostics.PerformanceCounter _totalOperations;
        // Total tests executed per second
        private System.Diagnostics.PerformanceCounter _totalOperationsPerSecond;
        private System.Diagnostics.PerformanceCounter _totalWarnings;

        /// <summary>
        ///   Prevents a default instance of the <see cref="PerformanceTimerHelper"/> class from being created.
        /// </summary>
        /// <param name="categoryName">The name of the <see cref="PerformanceTimerHelper.CategoryName">category</see>.</param>
        /// <param name="warningTimeSpan">
        ///   The length of time that the operation should take before logging a warning.
        /// </param>
        /// <param name="criticalTimeSpan">
        ///   The length of time that the operation should take before logging an error.
        /// </param>
        private PerformanceTimerHelper([NotNull] string categoryName, TimeSpan warningTimeSpan,
                                       TimeSpan criticalTimeSpan)
        {
            CategoryName = categoryName;
            WarningTimeSpan = warningTimeSpan;
            CriticalTimeSpan = criticalTimeSpan;
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
                if (PerformanceCounterHelper.Errored)
                    return;

                _initialised = true;

                // Set up the performance counter(s)
                try
                {
                    // Set up the performance counter(s)
                    if (!PerformanceCounterCategory.Exists(CategoryName))
                    {
                        Log.Add(
                            Resources.PerformanceTimerHelper_Initialize_CategoryDoesNotExist,
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

                    _averageOperationDuration = new System.Diagnostics.PerformanceCounter
                                                    {
                                                        CategoryName = CategoryName,
                                                        CounterName = "Average Operation Duration",
                                                        MachineName = ".",
                                                        ReadOnly = false
                                                    };

                    _averageOperationDurationBase = new System.Diagnostics.PerformanceCounter
                                                        {
                                                            CategoryName = CategoryName,
                                                            CounterName = "Average Operation Duration Base",
                                                            MachineName = ".",
                                                            ReadOnly = false
                                                        };

                    _totalWarnings = new System.Diagnostics.PerformanceCounter
                                         {
                                             CategoryName = CategoryName,
                                             CounterName = "Total Operation Exceeding Warning Duration",
                                             MachineName = ".",
                                             ReadOnly = false
                                         };

                    _totalCriticals = new System.Diagnostics.PerformanceCounter
                                          {
                                              CategoryName = CategoryName,
                                              CounterName = "Total Operation Exceeding Critical Duration",
                                              MachineName = ".",
                                              ReadOnly = false
                                          };
                    _found = true;
                }
                catch (UnauthorizedAccessException unauthorizedAccessException)
                {
                    // There is a potential race condition here but it's non-critical.
                    // Better to avoid the locking overhead on every access to Errored.
                    if (!PerformanceCounterHelper.Errored)
                    {
                        PerformanceCounterHelper.Errored = true;
                        _found = false;
                        // Create error but don't throw.
                        new LoggingException(unauthorizedAccessException,
                                             Resources.PerformanceTimerHelper_Initialize_ProcessDoesNotHaveAccess,
                                             LogLevel.SystemNotification);
                    }
                }
                catch (Exception exception)
                {
                    PerformanceCounterHelper.Errored = true;
                    _found = false;
                    // Create error but don't throw.
                    new LoggingException(exception,
                                         Resources.PerformanceTimerHelper_Initialize_UnhandledExceptionOccurred,
                                         LogLevel.SystemNotification,
                                         CategoryName);
                }
            }
        }

        /// <summary>
        ///   Gets the timer with the specified category name.
        ///   A timer is added to the lookup if no match is found.
        /// </summary>
        /// <param name="categoryName">
        ///   The name of the <see cref="PerformanceTimerHelper.CategoryName">category</see>.
        /// </param>
        /// <param name="warningTimeSpan">
        ///   The length of time that the operation should take before logging a warning.
        /// </param>
        /// <param name="criticalTimeSpan">
        ///   The length of time that the operation should take before logging an error.
        /// </param>
        /// <returns>The added/retrieved timer with the <paramref name="categoryName"/> specified.</returns>
        public static PerformanceTimerHelper Get([NotNull] string categoryName, TimeSpan warningTimeSpan,
                                                 TimeSpan criticalTimeSpan)
        {
            PerformanceTimerHelper timerHelper = _counters.GetOrAdd(
                categoryName, n => new PerformanceTimerHelper(n, warningTimeSpan, criticalTimeSpan));

            // Ensure initialised.
            timerHelper.Initialise();
            return timerHelper;
        }

        /// <summary>
        ///   Increments the counters.
        /// </summary>
        /// <param name="duration">The <see cref="TimeSpan">duration</see> of the operation.</param>
        [Conditional("Performance")]
        public void IncrementCounters(TimeSpan duration)
        {
            if (!_found ||
                PerformanceCounterHelper.Errored)
                return;
            _totalOperations.Increment();
            _totalOperationsPerSecond.Increment();

            // Get the duration in CPU ticks rather than DateTime ticks.
            _averageOperationDuration.IncrementBy((duration.Ticks*Stopwatch.Frequency)/10000000);
            _averageOperationDurationBase.Increment();
            if (duration < WarningTimeSpan)
                return;

            _totalWarnings.Increment();
            if (duration >= CriticalTimeSpan)
                _totalCriticals.Increment();
        }
    }
}