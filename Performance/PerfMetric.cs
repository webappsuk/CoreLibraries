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
using System.Linq;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Diagnostics;

namespace WebApplications.Utilities.Performance
{
    /// <summary>
    ///   Performance counters used to count operations.
    /// </summary>
    public sealed class PerfMetric : PerfCounter
    {
        /// <summary>
        /// Default counters for a category.
        /// </summary>
        [NotNull] private static readonly CounterCreationData[] _counterData = new[]
            {
                new CounterCreationData("Total operations", "Total operations executed since the start of the process.",
                                        PerformanceCounterType.NumberOfItems64),
                new CounterCreationData("Operations per second", "The number of operations per second.",
                                        PerformanceCounterType.RateOfCountsPerSecond64)
            };
        
        /// <summary>
        ///   Initializes a new instance of <see cref="PerfMetric"/>.
        /// </summary>
        /// <param name="categoryName">The performance counter's category name.</param>
        private PerfMetric([NotNull]string categoryName)
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
        ///   Increments the operation counters.
        /// </summary>
        public void IncrementBy(long value)
        {
            if (!IsValid ||
                (value == 0))
                return;

            Counters[0].IncrementBy(value);
            Counters[1].IncrementBy(value);
        }

        /// <summary>
        ///   Decrements the operation counters.
        /// </summary>
        public void Decrement()
        {
            if (!IsValid)
                return;

            Counters[0].Decrement();
            Counters[1].Decrement();
        }

        /// <summary>
        ///   Decrements the operation counters.
        /// </summary>
        public void DecrementBy(long value)
        {
            if (!IsValid ||
                (value == 0))
                return;

            long decrement = -value;
            Counters[0].IncrementBy(decrement);
            Counters[1].IncrementBy(decrement);
        }
    }
}