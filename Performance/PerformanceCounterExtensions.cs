using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using NodaTime;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Performance
{
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
        [PublicAPI]
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
                : Duration.FromSeconds((n - o) / eFreq);
        }

        /// <summary>
        /// Safely gets the next value of a performance counter, by ensuring that a minimum duration has passed since
        /// the last next value was requested.
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>System.Single.</returns>
        public static float SafeNextValue([NotNull] this PerformanceCounter counter)
        {
            Contract.Requires(counter != null);
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
