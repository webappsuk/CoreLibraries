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
        ///   The performance counter's category.
        /// </summary>
        [NotNull] [UsedImplicitly] public readonly string CategoryName;

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
        private PerformanceCounterHelper([NotNull] string categoryName)
        {
            CategoryName = categoryName;
        }

        /// <summary>
        ///   Gets a value indicating whether the <see cref="PerformanceCounterHelper"/> has errored.
        /// </summary>
        public static bool Errored { get; internal set; }

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
        public static PerformanceCounterHelper Get([NotNull] string categoryName)
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