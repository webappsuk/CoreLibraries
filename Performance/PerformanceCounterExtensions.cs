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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using NodaTime;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Performance
{
    /// <summary>
    /// Extensions for performance counters.
    /// </summary>
    [PublicAPI]
    public static class PerformanceCounterExtensions
    {
        /// <summary>
        /// Any samples collected for a performance counter.
        /// </summary>
        [NotNull]
        private static readonly ConcurrentDictionary<PerformanceCounter, LinkedList<CounterSample>> _samples =
            new ConcurrentDictionary<PerformanceCounter, LinkedList<CounterSample>>();

        /// <summary>
        /// Gets the elapsed time between two samples
        /// </summary>
        /// <param name="oldSample">The old sample.</param>
        /// <param name="newSample">The new sample.</param>
        /// <returns>Duration.</returns>
        public static Duration GetElapsedTime(CounterSample oldSample, CounterSample newSample)
        {
            // No data [start time = 0] so return 0 
            if (newSample.RawValue == 0)
                return Duration.Zero;

            float eFreq = (ulong)oldSample.CounterFrequency;
            ulong o = (ulong)oldSample.CounterTimeStamp;
            ulong n = (ulong)newSample.CounterTimeStamp;
            return o >= n ||
                   eFreq <= 0.0f
                ? Duration.Zero
                : Duration.FromSeconds((long)((n - o) / eFreq));
        }

        /// <summary>
        /// Safely gets the next value of a performance counter, by ensuring that a minimum duration has passed since
        /// the last next value was requested.
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>System.Single.</returns>
        public static float SafeNextValue([NotNull] this PerformanceCounter counter)
        {
            if (counter == null) throw new ArgumentNullException("counter");

            LinkedList<CounterSample> samples = _samples.GetOrAdd(counter, c => new LinkedList<CounterSample>());
            // ReSharper disable once PossibleNullReferenceException
            lock (samples)
            {
                CounterSample nextSample = counter.NextSample();
                CounterSample lastSample = CounterSample.Empty;
                LinkedListNode<CounterSample> node = samples.Last;
                while (node != null)
                {
                    CounterSample sample = node.Value;
                    if (lastSample.TimeStamp == 0)
                    {
                        if (GetElapsedTime(sample, nextSample) >= Duration.FromSeconds(1))
                            lastSample = sample;
                    }
                    else
                    // This sample is no longer needed.
                        samples.Remove(node);

                    node = node.Previous;
                }
                samples.AddLast(nextSample);
                if (lastSample.TimeStamp == 0)
                {
                    // We haven't got a sample that's over a second old, so use the earliest sample we have.
                    if (samples.First == null) return 0.0f;
                    lastSample = samples.First.Value;
                }
                return CounterSample.Calculate(lastSample, nextSample);
            }
        }
    }
}