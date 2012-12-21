using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Performance
{
    /// <summary>
    /// Extensions methods for performance counters.
    /// </summary>
    public static class PerformanceExtensions
    {
        /// <summary>
        /// Gets the performance timer.
        /// </summary>
        /// <param name="performanceInformation">The performance information.</param>
        /// <returns>PerformanceTimer.</returns>
        [NotNull]
        public static PerformanceTimer GetPerformanceTimer([NotNull]this PerformanceInformation performanceInformation)
        {
            Contract.Assert(performanceInformation.IsTimer);
            return PerformanceTimer.Get(performanceInformation.CategoryName,
                                        performanceInformation.DefaultWarningDuration,
                                        performanceInformation.DefaultCriticalDuration);
        }

        /// <summary>
        /// Gets the performance counter.
        /// </summary>
        /// <param name="performanceInformation">The performance information.</param>
        /// <returns>PerformanceTimer.</returns>
        [NotNull]
        public static PerformanceCounter GetPerformanceCounter([NotNull]this PerformanceInformation performanceInformation)
        {
            Contract.Assert(!performanceInformation.IsTimer);
            return PerformanceCounter.Get(performanceInformation.CategoryName);
        }

        /// <summary>
        /// Gets the performance timer based on the index into <see paramref="counters" />.
        /// </summary>
        /// <param name="counters">The counters.</param>
        /// <param name="index">The index.</param>
        /// <returns>PerformanceTimer.</returns>
        [NotNull]
        public static PerformanceTimer GetPerformanceTimer([NotNull]this PerformanceInformation[] counters, int index)
        {
            return GetPerformanceTimer(counters[index]);
        }

        /// <summary>
        /// Gets the performance counter based on the index into <see paramref="counters" />.
        /// </summary>
        /// <param name="counters">The counters.</param>
        /// <param name="index">The index.</param>
        /// <returns>PerformanceTimer.</returns>
        [NotNull]
        public static PerformanceCounter GetPerformanceCounter([NotNull]this PerformanceInformation[] counters, int index)
        {
            return GetPerformanceCounter(counters[index]);
        }
    }
}
