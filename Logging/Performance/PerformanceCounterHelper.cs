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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Logging.Performance
{
    /// <summary>
    ///   Performance counters used for operations.
    /// </summary>
    public abstract class PerformanceCounterHelper
    {
        /// <summary>
        /// The current instance name for all performance counters.
        /// </summary>
        [NotNull]
        public static readonly string InstanceGuid = Guid.NewGuid().ToString();

        /// <summary>
        /// The machine name used to access performance counters.
        /// </summary>
        [NotNull]
        public const string MachineName = ".";

        /// <summary>
        /// Whether the current process has access to performance counters.
        /// </summary>
        public static readonly bool HasAccess;

        /// <summary>
        /// Initializes static members of the <see cref="PerformanceCounterHelper" /> class.
        /// </summary>
        /// <remarks>We only check access to performance counters once.</remarks>
        static PerformanceCounterHelper()
        {
            try
            {
                // Check we have access to the performance counters.
                PerformanceCounterCategory.Exists("TestAccess", MachineName);
                HasAccess = true;
                Log.Add(Resources.PerformanceCounterHelper_Enabled, LogLevel.Information, InstanceGuid);
            }
            catch (Exception exception)
            {
                Log.Add(exception, LogLevel.SystemNotification);
                Log.Add(Resources.PerformanceCounterHelper_ProcessDoesNotHaveAccess, LogLevel.SystemNotification);
                HasAccess = false;
            }
        }

        /// <summary>
        ///   The performance counter's category.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public readonly string CategoryName;

        /// <summary>
        /// Whether the counter is valid.
        /// </summary>
        public readonly bool IsValid;

        /// <summary>
        /// The underlying counters.
        /// </summary>
        [NotNull]
        protected System.Diagnostics.PerformanceCounter[] Counters;

        /// <summary>
        /// Creates a performance counter instance.
        /// </summary>
        /// <param name="categoryName">The performance counter's <see cref="PerformanceCounterHelper.CategoryName">category name</see>.</param>
        /// <param name="counters">The counters.</param>
        protected PerformanceCounterHelper([NotNull] string categoryName, [NotNull]IEnumerable<CounterCreationData> counters)
        {
            CategoryName = categoryName;
            if (!HasAccess)
                return;

            CounterCreationData[] cArray = counters as CounterCreationData[] ?? counters.ToArray();
            if (cArray.Length < 1)
            {
                IsValid = false;
                return;
            }

            // Set up the performance counter(s)
            try
            {
                if (!PerformanceCounterCategory.Exists(CategoryName))
                {
                    Log.Add(
                        Resources.PerformanceCounterHelper_CategoryDoesNotExist,
                        LogLevel.SystemNotification,
                        CategoryName);
                    IsValid = false;
                    return;
                }

                Counters = new System.Diagnostics.PerformanceCounter[cArray.Length];
                for (int c = 0; c < cArray.Length; c++)
                {
                    CounterCreationData counter = cArray[c];
                    if (!PerformanceCounterCategory.CounterExists(counter.CounterName, categoryName))
                    {
                        Log.Add(
                            Resources.PerformanceCounterHelper_CounterDoesNotExist,
                            LogLevel.SystemNotification,
                            CategoryName, counter.CounterName);
                        IsValid = false;
                        return;
                    }
                    Counters[c] = new System.Diagnostics.PerformanceCounter()
                        {
                            CategoryName = categoryName,
                            CounterName = counter.CounterName,
                            MachineName = MachineName,
                            InstanceLifetime = PerformanceCounterInstanceLifetime.Process,
                            InstanceName = InstanceGuid,
                            ReadOnly = false
                        };

                    // Read the first value to 'start' the counters.
                    Counters[c].NextValue();
                }
                IsValid = true;
            }
            catch (UnauthorizedAccessException unauthorizedAccessException)
            {
                Log.Add(unauthorizedAccessException, LogLevel.SystemNotification);
                Log.Add(Resources.PerformanceCounterHelper_ProcessDoesNotHaveAccess, LogLevel.SystemNotification);
                IsValid = false;
            }
            catch (Exception exception)
            {
                // Create error but don't throw.
                new LoggingException(exception,
                                     Resources.PerformanceCounterHelper_UnhandledExceptionOccurred,
                                     LogLevel.SystemNotification,
                                     CategoryName);
                IsValid = false;
            }
        }

        /// <summary>
        /// Holds enumeration of all counters for creation/deletion.
        /// </summary>
        [NotNull]
        private static readonly PerformanceInformation[] _performanceCounters = new[]
            {
                new PerformanceInformation("Logged new item", "Tracks every time a log entry is logged."),
                new PerformanceInformation("Logged exception", "Tracks every time an exception is logged.")
            };

        [NotNull]
        internal static readonly PerformanceCounter PerfCounterNewItem = _performanceCounters.GetPerformanceCounter(0);
        [NotNull]
        internal static readonly PerformanceCounter PerfCounterException = _performanceCounters.GetPerformanceCounter(1);
    }
}