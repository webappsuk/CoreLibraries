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
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Performance.Configuration;

namespace WebApplications.Utilities.Performance
{
    /// <summary>
    ///   Performance counters used to count operations.
    /// </summary>
    [PublicAPI]
    public sealed class PerfCounter : PerfCategory
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
                PerformanceCounterType.RateOfCountsPerSecond64)
        };

        /// <summary>
        ///   Initializes a new instance of <see cref="PerfCounter"/>.
        /// </summary>
        /// <param name="categoryName">The performance counter's category name.</param>
        private PerfCounter([NotNull] string categoryName)
            : base(categoryName, _counterData)
        {
            Counter = new Counter();
            // ReSharper disable PossibleNullReferenceException
            AddInfo("Count", "Total operations executed since the start of the process.", () => Counter.Count);
            AddInfo("Rate", "The number of operations per second.", () => Counter.Rate);
            AddInfo("Samples", "The number of samples.", () => Counter.SamplesCount);
            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>
        /// The counter.
        /// </summary>
        [NotNull]
        public readonly Counter Counter;

        /// <summary>
        ///   Increments the operation counters.
        /// </summary>
        public void Increment()
        {
            if (!PerformanceConfiguration.IsEnabled)
                return;

            Counter.Increment();

            if (!IsValid)
                return;
            Debug.Assert(Counters != null);
            Debug.Assert(Counters.Length == 2);

            // ReSharper disable PossibleNullReferenceException
            Counters[0].Increment();
            Counters[1].Increment();
            // ReSharper restore PossibleNullReferenceException
        }
    }
}