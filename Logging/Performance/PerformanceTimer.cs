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

using JetBrains.Annotations;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading;

namespace WebApplications.Utilities.Logging.Performance
{
    /// <summary>
    ///   Times an operation using a performance timer.
    /// </summary>
    public class PerformanceTimer : PerformanceCounterHelper
    {
        /// <summary>
        /// Default counters for a category.
        /// </summary>
        [NotNull] private static readonly IEnumerable<CounterCreationData> _counterData = new[]
            {
                    new CounterCreationData("Total operations", "Total operations executed since the start of the process.", PerformanceCounterType.NumberOfItems64),
                    new CounterCreationData("Operations per second", "The number of operations per second.", PerformanceCounterType.RateOfCountsPerSecond64),
                new CounterCreationData("Average duration", "The average duration of each operation.",
                                        PerformanceCounterType.AverageTimer32),
                new CounterCreationData("Average duration Base", "The average duration base counter.",
                                        PerformanceCounterType.AverageBase),
                new CounterCreationData("Total warnings", "Total operations executed since the start of the process that have exceeded the warning duration threshhold.",
                                        PerformanceCounterType.NumberOfItems64),
                new CounterCreationData("Total criticals", "Total operations executed since the start of the process that have exceeded the critical duration threshhold.",
                                        PerformanceCounterType.NumberOfItems64)
            };

        /// <summary>
        /// Holds all counters.
        /// </summary>
        [NotNull] private static readonly ConcurrentDictionary<string, PerformanceTimer> _counters =
            new ConcurrentDictionary<string, PerformanceTimer>();

        /// <summary>
        /// The duration after which the warning counter is incremented.
        /// </summary>
        public readonly TimeSpan DefaultWarningDuration;

        /// <summary>
        /// The duration after which the critical counter is incremented.
        /// </summary>
        public readonly TimeSpan DefaultCriticalDuration;

        /// <summary>
        ///   Initializes a new instance of the <see cref="PerformanceTimer"/> class.
        /// </summary>
        /// <param name="categoryName">The name of the category.</param>
        /// <param name="defaultWarningDuration">
        ///   The duration of time that the operation should take before logging a warning.
        /// </param>
        /// <param name="defaultCriticalDuration">
        ///   The duration of time that the operation should take before logging an error.
        /// </param>
        private PerformanceTimer([NotNull] string categoryName, TimeSpan defaultWarningDuration = default(TimeSpan),
                                TimeSpan defaultCriticalDuration = default(TimeSpan))
            : base(categoryName, _counterData)
        {
            Contract.Requires(categoryName != null);
            Contract.Requires(defaultWarningDuration <= defaultCriticalDuration);
            if (defaultWarningDuration == default(TimeSpan))
                defaultWarningDuration = TimeSpan.MaxValue;
            if (defaultCriticalDuration == default(TimeSpan))
                defaultCriticalDuration = TimeSpan.MaxValue;

            if (defaultWarningDuration > defaultCriticalDuration)
                defaultCriticalDuration = defaultWarningDuration;

            DefaultWarningDuration = defaultWarningDuration;
            DefaultCriticalDuration = defaultCriticalDuration;
        }

        /// <summary>
        /// <para>
        /// Starts a timer that and returns an object that when disposed will update this
        /// instance with the time between calling this method and disposing the result.</para>
        ///   <para>This makes it easy to use a <see cref="PerformanceTimer" /> in a using block to time
        /// a block of code.</para>
        /// </summary>
        /// <param name="warningDuration">Duration before a warning is counted.</param>
        /// <param name="criticalDuration">Duration before a critical is counted.</param>
        /// <returns>IDisposable.</returns>
        public Timer Region(TimeSpan warningDuration = default (TimeSpan),
                            TimeSpan criticalDuration = default (TimeSpan))
        {
            return new Timer(this, warningDuration, criticalDuration);
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
        /// Gets the operations per second.
        /// </summary>
        /// <value>The count.</value>
        public TimeSpan AverageDuration
        {
            get { return IsValid ? TimeSpan.FromSeconds(Counters[2].NextValue()) : TimeSpan.Zero; }
        }

        /// <summary>
        /// Gets the current operation count.
        /// </summary>
        /// <value>The count.</value>
        public long Warnings
        {
            get { return IsValid ? Counters[4].RawValue : 0; }
        }

        /// <summary>
        /// Gets the current operation count.
        /// </summary>
        /// <value>The count.</value>
        public long Criticals
        {
            get { return IsValid ? Counters[5].RawValue : 0; }
        }

        /// <summary>
        ///   Increments the counters.
        /// </summary>
        /// <param name="duration">The <see cref="TimeSpan">duration</see> of the operation.</param>
        public void Increment(TimeSpan duration)
        {
            if (!IsValid)
                return;
            Counters[0].Increment();
            Counters[1].Increment();

            // Get the duration in CPU ticks rather than DateTime ticks.
            Counters[2].IncrementBy((duration.Ticks*Stopwatch.Frequency)/10000000);
            Counters[3].Increment();
            if (duration < DefaultWarningDuration)
                return;

            Counters[4].Increment();
            if (duration >= DefaultCriticalDuration)
                Counters[5].Increment();
        }

        /// <summary>
        /// Gets the performance counter with the specified category name.
        /// </summary>
        /// <param name="categoryName">Name of the category.</param>
        /// <param name="defaultWarningDuration">
        ///   The duration of time that the operation should take before logging a warning.
        /// </param>
        /// <param name="defaultCriticalDuration">
        ///   The duration of time that the operation should take before logging an error.
        /// </param>
        /// <returns>The performance counter.</returns>
        [NotNull]
        public static PerformanceTimer Get([NotNull] string categoryName, TimeSpan defaultWarningDuration = default(TimeSpan),
                                TimeSpan defaultCriticalDuration = default(TimeSpan))
        {
            return _counters.GetOrAdd(categoryName,
                                      n => new PerformanceTimer(n, defaultWarningDuration, defaultCriticalDuration));
        }

        /// <summary>
        /// Creates the specified performance timer (use during installation only).
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
        public static bool Create([NotNull] string categoryName, [NotNull] string categoryHelp)
        {
            return Create(categoryName, categoryHelp, _counterData);
        }

        /// <summary>
        /// Deletes the specified performance timer (use during uninstall only).
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
        public static bool Delete([NotNull] string categoryName)
        {
            return Delete(categoryName, _counterData);
        }

        /// <summary>
        /// Used to allow the timing of a region of code for a <see cref="PerformanceTimer"/>.
        /// </summary>
        public class Timer : IDisposable
        {
            /// <summary>
            /// The associated performance timer.
            /// </summary>
            [NotNull]
            public readonly PerformanceTimer PerformanceTimer;

            /// <summary>
            /// The duration after which the warning counter is incremented.
            /// </summary>
            public readonly TimeSpan WarningDuration;

            /// <summary>
            /// The duration after which the critical counter is incremented.
            /// </summary>
            public readonly TimeSpan CriticalDuration;

            private Stopwatch _stopwatch;
            private TimeSpan _elapsed;

            /// <summary>
            /// Gets the elapsed time (even after disposal).
            /// </summary>
            /// <value>The elapsed.</value>
            public TimeSpan Elapsed
            {
                get
                {
                    Stopwatch s = _stopwatch;
                    return s == null ? _elapsed : s.Elapsed;
                }
            }

            /// <summary>
            /// Gets a value indicating whether this <see cref="Timer" /> has exceeded the <see cref="WarningDuration"/>.
            /// </summary>
            /// <value><see langword="true" /> if warning; otherwise, <see langword="false" />.</value>
            public bool Warning
            {
                get { return Elapsed > WarningDuration; }
            }

            /// <summary>
            /// Gets a value indicating whether this <see cref="Timer" /> has exceeded the <see cref="CriticalDuration"/>.
            /// </summary>
            /// <value><see langword="true" /> if critical; otherwise, <see langword="false" />.</value>
            public bool Critical
            {
                get { return Elapsed > CriticalDuration; }
            }

            internal Timer([NotNull]PerformanceTimer performanceTimer, TimeSpan warningDuration, TimeSpan criticalDuration)
            {
                PerformanceTimer = performanceTimer;
                if (warningDuration == default(TimeSpan))
                    warningDuration = performanceTimer.DefaultWarningDuration;
                if (criticalDuration == default(TimeSpan))
                    criticalDuration = performanceTimer.DefaultCriticalDuration;

                if (warningDuration > criticalDuration)
                    criticalDuration = warningDuration;
                WarningDuration = warningDuration;
                CriticalDuration = criticalDuration;
                _stopwatch = new Stopwatch();
                _stopwatch.Start();
            }

            /// <summary>
            /// Disposes the timer updating the performance counters the first time this is called.
            /// </summary>
            /// <remarks>
            /// <para>
            /// Following disposal, the public properties of this type remain safely accessible.</para>
            /// </remarks>
            public void Dispose()
            {
                // Set s to null, and check we were the thread that succeeded.
                Stopwatch s = Interlocked.Exchange(ref _stopwatch, null);
                if (ReferenceEquals(s, null))
                    return;

                s.Stop();
                _elapsed = s.Elapsed;

                // Increment counters.
                PerformanceTimer.Increment(_elapsed);
            }
        }
    }
}