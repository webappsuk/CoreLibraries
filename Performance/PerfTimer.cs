#region © Copyright Web Applications (UK) Ltd, 2014.  All rights reserved.
// Copyright (c) 2014, Web Applications UK Ltd
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
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Performance
{
    /// <summary>
    ///   Times an operation using a performance timer.
    /// </summary>
    [PublicAPI]
    public sealed class PerfTimer : PerfCategory
    {
        /// <summary>
        /// Default counters for a category.
        /// </summary>
        [NotNull]
        private static readonly CounterCreationData[] _counterData =
        {
            new CounterCreationData(
                "Total operations",
                "Total operations executed since the start of the process.",
                PerformanceCounterType.NumberOfItems64),
            new CounterCreationData(
                "Operations per second",
                "The number of operations per second.",
                PerformanceCounterType.RateOfCountsPerSecond64),
            new CounterCreationData(
                "Average duration",
                "The average duration of each operation.",
                PerformanceCounterType.AverageTimer32),
            new CounterCreationData(
                "Average duration Base",
                "The average duration base counter.",
                PerformanceCounterType.AverageBase),
            new CounterCreationData(
                "Total warnings",
                "Total operations executed since the start of the process that have exceeded the warning duration threshhold.",
                PerformanceCounterType.NumberOfItems64),
            new CounterCreationData(
                "Total criticals",
                "Total operations executed since the start of the process that have exceeded the critical duration threshhold.",
                PerformanceCounterType.NumberOfItems64)
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="PerfTimer" /> class.
        /// </summary>
        /// <param name="categoryName">The name of the category.</param>
        private PerfTimer([NotNull] string categoryName)
            : base(categoryName, _counterData)
        {
            Contract.Requires(categoryName != null);
            if (!IsValid) return;
            // ReSharper disable PossibleNullReferenceException
            AddInfo("Count", "Total operations executed since the start of the process.", () => Counters[0].RawValue);
            AddInfo("Rate", "The number of operations per second.", () => Counters[1].NextValue());
            AddInfo(
                "Average Duration",
                "The average duration of each operation.",
                () => TimeSpan.FromSeconds(Counters[2].NextValue()));
            AddInfo(
                "Warnings",
                "Total operations executed since the start of the process that have exceeded the warning duration threshhold.",
                () => Counters[4].RawValue);
            AddInfo(
                "Criticals",
                "Total operations executed since the start of the process that have exceeded the critical duration threshhold.",
                () => Counters[5].RawValue);
            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>
        /// <para>
        /// Starts a timer that and returns an object that when disposed will update this
        /// instance with the time between calling this method and disposing the result.</para>
        ///   <para>This makes it easy to use a <see cref="PerfTimer" /> in a using block to time
        /// a block of code.</para>
        /// </summary>
        /// <param name="warningDuration">Duration before a warning is counted (defaults to infinite).</param>
        /// <param name="criticalDuration">Duration before a critical is counted (defaults to infinite).</param>
        /// <returns>IDisposable.</returns>
        [NotNull]
        [PublicAPI]
        public Timer Region(
            TimeSpan warningDuration = default (TimeSpan),
            TimeSpan criticalDuration = default (TimeSpan))
        {
            return new Timer(this, warningDuration, criticalDuration);
        }

        /// <summary>
        /// Gets the current operation count.
        /// </summary>
        /// <value>The count.</value>
        [PublicAPI]
        public long OperationCount
        {
            get { return IsValid ? Counters[0].RawValue : 0; }
        }

        /// <summary>
        /// Gets the operations per second.
        /// </summary>
        /// <value>The count.</value>
        [PublicAPI]
        public float Rate
        {
            get { return IsValid ? Counters[1].NextValue() : float.NaN; }
        }

        /// <summary>
        /// Gets the operations per second.
        /// </summary>
        /// <value>The count.</value>
        [PublicAPI]
        public TimeSpan AverageDuration
        {
            get { return IsValid ? TimeSpan.FromSeconds(Counters[2].NextValue()) : TimeSpan.Zero; }
        }

        /// <summary>
        /// Gets the current operation count.
        /// </summary>
        /// <value>The count.</value>
        [PublicAPI]
        public long Warnings
        {
            get { return IsValid ? Counters[4].RawValue : 0; }
        }

        /// <summary>
        /// Gets the current operation count.
        /// </summary>
        /// <value>The count.</value>
        [PublicAPI]
        public long Criticals
        {
            get { return IsValid ? Counters[5].RawValue : 0; }
        }

        /// <summary>
        ///   Increments the counters.
        /// </summary>
        /// <param name="duration">The <see cref="TimeSpan">duration</see> of the operation.</param>
        /// <param name="warningDuration">Duration before a warning is counted (defaults to infinite).</param>
        /// <param name="criticalDuration">Duration before a critical is counted (defaults to infinite).</param>
        public void IncrementBy(
            TimeSpan duration,
            TimeSpan warningDuration = default (TimeSpan),
            TimeSpan criticalDuration = default (TimeSpan))
        {
            if (!IsValid ||
                (duration == TimeSpan.Zero))
                return;
            Counters[0].Increment();
            Counters[1].Increment();

            // Get the duration in CPU ticks rather than DateTime ticks.
            Counters[2].IncrementBy((duration.Ticks * Stopwatch.Frequency) / 10000000);
            Counters[3].Increment();

            if ((warningDuration == default(TimeSpan)) ||
                (duration < warningDuration))
                return;

            Counters[4].Increment();

            if ((criticalDuration == default(TimeSpan)) ||
                (duration < criticalDuration))
                return;

            Counters[5].Increment();
        }

        /// <summary>
        ///   Increments the counters.
        /// </summary>
        /// <param name="duration">The <see cref="TimeSpan">duration</see> of the operation.</param>
        /// <param name="warningDuration">Duration before a warning is counted (defaults to infinite).</param>
        /// <param name="criticalDuration">Duration before a critical is counted (defaults to infinite).</param>
        [PublicAPI]
        public void DecrementBy(
            TimeSpan duration,
            TimeSpan warningDuration = default (TimeSpan),
            TimeSpan criticalDuration = default (TimeSpan))
        {
            if (!IsValid ||
                (duration == TimeSpan.Zero))
                return;

            Counters[0].Decrement();
            Counters[1].Decrement();

            // Get the duration in CPU ticks rather than DateTime ticks.
            Counters[2].IncrementBy(((-duration).Ticks * Stopwatch.Frequency) / 10000000);
            Counters[3].Decrement();

            if ((warningDuration == default(TimeSpan)) ||
                (duration < warningDuration))
                return;

            Counters[4].Decrement();

            if ((criticalDuration == default(TimeSpan)) ||
                (duration < criticalDuration))
                return;

            Counters[5].Decrement();
        }

        /// <summary>
        /// Used to allow the timing of a region of code for a <see cref="PerfTimer"/>.
        /// </summary>
        public class Timer : IDisposable
        {
            /// <summary>
            /// The associated performance timer.
            /// </summary>
            [NotNull]
            public readonly PerfTimer PerfTimer;

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

            internal Timer([NotNull] PerfTimer perfTimer, TimeSpan warningDuration, TimeSpan criticalDuration)
            {
                PerfTimer = perfTimer;
                if (warningDuration == default(TimeSpan))
                    warningDuration = TimeSpan.MaxValue;
                if (criticalDuration == default(TimeSpan))
                    criticalDuration = TimeSpan.MaxValue;

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
                PerfTimer.IncrementBy(_elapsed, WarningDuration, CriticalDuration);
            }
        }
    }
}