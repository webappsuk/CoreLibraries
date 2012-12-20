using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Performance
{
    /// <summary>
    /// Extensions for performance counters.
    /// </summary>
    public static class PerformanceCounterExtensions
    {
        /// <summary>
        /// Returns all the missing performance counters.
        /// </summary>
        /// <param name="performanceCounters">The performance counters.</param>
        /// <returns>An enumeration of any missing performance counters.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<PerformanceInformation> Missing([NotNull]
            this IEnumerable<PerformanceInformation> performanceCounters)
        {
            return performanceCounters.Where(i => !i.Exists);
        }

        /// <summary>
        /// Whether all the performance counters exist.
        /// </summary>
        /// <param name="performanceCounters">The performance counters.</param>
        /// <returns><c>true</c> if all the counters exist fully, <c>false</c> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Exists([NotNull]
            this IEnumerable<PerformanceInformation> performanceCounters)
        {
            return !Missing(performanceCounters).Any();
        }
    }
}