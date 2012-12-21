using System;
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Performance
{
    /// <summary>
    /// Holds information about performance counters.
    /// </summary>
    public class PerformanceInformation
    {
        /// <summary>
        /// The category name.
        /// </summary>
        [NotNull]
        public readonly string CategoryName;

        /// <summary>
        /// The category help.
        /// </summary>
        [NotNull]
        public readonly string CategoryHelp;

        /// <summary>
        /// Whether the counter is a timer.
        /// </summary>
        public readonly bool IsTimer;

        /// <summary>
        /// If the counter is a timer, holds the default warning duration.
        /// </summary>
        public readonly TimeSpan DefaultWarningDuration;

        /// <summary>
        /// If the coutner is a timer, holds the default critical duration.
        /// </summary>
        public readonly TimeSpan DefaultCriticalDuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceInformation" /> class to indicate a perforance counter.
        /// </summary>
        /// <param name="categoryName">Name of the category.</param>
        /// <param name="categoryHelp">The category help.</param>
        /// <remarks>Allows use a string literal in the constructor call as it can then be detected and the counters can be
        /// administered directly.</remarks>
        public PerformanceInformation(string categoryName, string categoryHelp)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(categoryName));
            Contract.Requires(!string.IsNullOrWhiteSpace(categoryHelp));
            CategoryName = categoryName;
            CategoryHelp = categoryHelp;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceInformation" /> class to indicate a perforance timer.
        /// </summary>
        /// <param name="categoryName">Name of the category.</param>
        /// <param name="categoryHelp">The category help.</param>
        /// <param name="defaultWarningDuration">Default duration of the warning.</param>
        /// <param name="defaultCriticalDuration">Default duration of the critical.</param>
        /// <remarks>Allows use a string literal in the constructor call as it can then be detected and the counters can be
        /// administered directly.</remarks>
        public PerformanceInformation(string categoryName, string categoryHelp, TimeSpan defaultWarningDuration, TimeSpan defaultCriticalDuration)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(categoryName));
            Contract.Requires(!string.IsNullOrWhiteSpace(categoryHelp));
            CategoryName = categoryName;
            CategoryHelp = categoryHelp;
            IsTimer = true;
            DefaultWarningDuration = defaultWarningDuration == default(TimeSpan)
                                         ? TimeSpan.MaxValue
                                         : defaultWarningDuration;
            DefaultCriticalDuration = defaultCriticalDuration == default(TimeSpan)
                                          ? TimeSpan.MaxValue
                                          : defaultCriticalDuration;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="PerformanceInformation" /> exists.
        /// </summary>
        /// <value><see langword="true" /> if exists; otherwise, <see langword="false" />.</value>
        public bool Exists
        {
            get
            {
                return IsTimer
                           ? PerformanceTimer.Exists(CategoryName)
                           : PerformanceCounter.Exists(CategoryName);
            }
        }
    }
}