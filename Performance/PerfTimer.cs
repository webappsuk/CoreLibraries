#region © Copyright Web Applications (UK) Ltd, 2015.  All rights reserved.
// Copyright (c) 2015, Web Applications UK Ltd
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

using System.Diagnostics;
using System.Diagnostics.Contracts;
using NodaTime;
using WebApplications.Utilities.Annotations;

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
                PerformanceCounterType.AverageBase)
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="PerfTimer" /> class.
        /// </summary>
        /// <param name="categoryName">The name of the category.</param>
        private PerfTimer([NotNull] string categoryName)
            : base(categoryName, _counterData)
        {
            Contract.Requires(categoryName != null);
            Timers = new Timers();
            // ReSharper disable PossibleNullReferenceException
            AddInfo("Count", "Total operations executed since the start of the process.", () => Timers.Count);
            AddInfo("Rate", "The number of operations per second.", () => Timers.Rate);
            AddInfo("Total Duration", "The total duration.", () => Timers.TotalDuration);
            AddInfo("Average Duration", "The average duration of each operation.", () => Timers.AverageDuration);
            AddInfo("Samples", "The number of samples.", () => Timers.SamplesCount);
            AddInfo("Samples Total", "The total duration of the samples.", () => Timers.TotalSampleDuration);
            AddInfo(
                "Samples Average",
                "The average duration of each operation in the samples.",
                () => Timers.AverageSampleDuration);
            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>
        /// The timers collection.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public readonly Timers Timers;

        /// <summary>
        /// <para>
        /// Starts a timer that and returns an object that when disposed will update this
        /// instance with the time between calling this method and disposing the result.</para>
        ///   <para>This makes it easy to use a <see cref="PerfTimer" /> in a using block to time
        /// a block of code.</para>
        /// </summary>
        /// <returns>IDisposable.</returns>
        [NotNull]
        [PublicAPI]
        public RegionTimer Region()
        {
            return new RegionTimer(IncrementBy);
        }

        /// <summary>
        /// Increments the counters.
        /// </summary>
        /// <param name="regionTimer">The region timer.</param>
        private void IncrementBy([NotNull] RegionTimer regionTimer)
        {
            Timers.Increment(regionTimer);

            Duration duration = regionTimer.Elapsed;
            if (!IsValid)
                return;

            Counters[0].Increment();
            Counters[1].Increment();

            // Get the duration in CPU ticks rather than DateTime ticks.
            Counters[2].IncrementBy((duration.Ticks * Stopwatch.Frequency) / 10000000);
            Counters[3].Increment();
        }
    }
}