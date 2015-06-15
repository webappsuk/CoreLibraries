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

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using NodaTime;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Performance.Configuration;

namespace WebApplications.Utilities.Performance
{
    /// <summary>
    /// A counter which also tracks the current rate, by keeping track of the last <see cref="MaximumSamples"/>
    /// instances the counter was incremented.
    /// </summary>
    [PublicAPI]
    public class Counter : IEnumerable<Instant>
    {
        /// <summary>
        /// The maximum samples for rate calculation.
        /// </summary>
        public readonly int MaximumSamples;

        private long _count;
        private int _samplesCount;

        [NotNull]
        private readonly LinkedList<Instant> _samples;

        /// <summary>
        /// Initializes a new instance of the <see cref="Counter" /> class.
        /// </summary>
        /// <param name="maximumSamples">The maximum samples (defaults to <see cref="PerformanceConfiguration.DefaultMaximumSamples"/>.</param>
        public Counter(int maximumSamples = int.MaxValue)
        {
            if (maximumSamples == int.MaxValue) maximumSamples = PerformanceConfiguration.DefaultMaximumSamples;
            if (maximumSamples < 2) maximumSamples = 2;
            MaximumSamples = maximumSamples;

            _samples = new LinkedList<Instant>();
        }

        /// <summary>
        /// Gets the current count.
        /// </summary>
        /// <value>The count.</value>
        public long Count
        {
            get { return _count; }
        }

        /// <summary>
        /// Gets the current number of samples.
        /// </summary>
        /// <value>The count of samples.</value>
        public int SamplesCount
        {
            get { return _samplesCount; }
        }

        /// <summary>
        /// Gets the rate in operations per second.
        /// </summary>
        /// <value>The rate.</value>
        /// <remarks>Returns <see cref="double.PositiveInfinity"/> if there are less than two samples.</remarks>
        public double Rate
        {
            get
            {
                Instant start;
                Instant end;
                LinkedList<Instant> samples = _samples;
                double count;
                lock (samples)
                {
                    count = samples.Count;
                    if (count < 2) return double.PositiveInfinity;

                    Debug.Assert(samples.First != null);
                    Debug.Assert(samples.Last != null);

                    start = samples.First.Value;
                    end = samples.Last.Value;
                }
                return count / (end - start).TotalSeconds();
            }
        }

        /// <summary>
        /// Increments this instance.
        /// </summary>
        /// <param name="timeStamp">The time stamp (defaults to now).</param>
        public void Increment(Instant? timeStamp = null)
        {
            Instant lastIncrement = timeStamp ?? HighPrecisionClock.Instance.Now;
            Interlocked.Increment(ref _count);
            LinkedList<Instant> samples = _samples;
            lock (samples)
            {
                samples.AddLast(lastIncrement);
                if (samples.Count > MaximumSamples)
                    _samples.RemoveFirst();
                else _samplesCount++;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<Instant> GetEnumerator()
        {
            Instant[] copy;
            lock (_samples)
                copy = _samples.ToArray();
            return ((IEnumerable<Instant>)copy).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}