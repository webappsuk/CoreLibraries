using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Performance.Tools.PerfSetup
{
    /// <summary>
    /// Performance information for an individual set of counters.
    /// </summary>
    public class PerformanceInformation
    {
        /// <summary>
        /// Default counters for a category.
        /// </summary>
        [NotNull] private static readonly CounterCreationData[] _counterData = new[]
            {
                new CounterCreationData("Total operations", "Total operations executed since the start of the process.",
                                        PerformanceCounterType.NumberOfItems64),
                new CounterCreationData("Operations per second", "The number of operations per second.",
                                        PerformanceCounterType.RateOfCountsPerSecond64),
                new CounterCreationData("Average duration", "The average duration of each operation.",
                                        PerformanceCounterType.AverageTimer32),
                new CounterCreationData("Average duration Base", "The average duration base counter.",
                                        PerformanceCounterType.AverageBase),
                new CounterCreationData("Total warnings",
                                        "Total operations executed since the start of the process that have exceeded the warning duration threshhold.",
                                        PerformanceCounterType.NumberOfItems64),
                new CounterCreationData("Total criticals",
                                        "Total operations executed since the start of the process that have exceeded the critical duration threshhold.",
                                        PerformanceCounterType.NumberOfItems64)
            };

        private readonly CounterCreationData[] _counters;

        /// <summary>
        /// The category name.
        /// </summary>
        [NotNull]
        public readonly string Assembly;

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
        /// Initializes a new instance of the <see cref="PerformanceInformation" /> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="categoryName">Name of the category.</param>
        /// <param name="categoryHelp">The category help.</param>
        /// <param name="isTimer">if set to <see langword="true" /> [is timer].</param>
        public PerformanceInformation([NotNull]string assembly, [NotNull]string categoryName, [NotNull]string categoryHelp, bool isTimer)
        {
            Assembly = assembly;
            CategoryName = categoryName;
            CategoryHelp = categoryHelp;
            IsTimer = isTimer;
            _counters = IsTimer ? _counterData : _counterData.Take(2).ToArray();
        }

        public bool Exists
        {
            get
            {
                return (PerformanceCounterCategory.Exists(CategoryName)) &&
                       (_counters.All(c => PerformanceCounterCategory.CounterExists(c.CounterName, CategoryName)));
            }
        }

        /// <summary>
        /// Creates the counters on the specified machine name.
        /// </summary>
        /// <param name="machineName">Name of the machine.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public bool Create(string machineName)
        {
            try
            {
                if (PerformanceCounterCategory.Exists(CategoryName))
                {
                    if (_counters.All(c => PerformanceCounterCategory.CounterExists(c.CounterName, CategoryName)))
                        return true;

                    // We don't have all the counters, so drop and recreate.
                    if (!Delete(machineName))
                        return false;
                }

                PerformanceCounterCategory.Create(CategoryName, CategoryHelp,
                                                  PerformanceCounterCategoryType.MultiInstance,
                                                  new CounterCreationDataCollection(_counters));

                return _counters.All(c => PerformanceCounterCategory.CounterExists(c.CounterName, CategoryName));
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes the counters from specified machine name.
        /// </summary>
        /// <param name="machineName">Name of the machine.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public bool Delete(string machineName)
        {
            try
            {
                if (!PerformanceCounterCategory.Exists(CategoryName))
                    return true;
                    
                PerformanceCounterCategory.Delete(CategoryName);
                return !PerformanceCounterCategory.Exists(CategoryName);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("{0}: {1}{2}",
                                 Assembly,
                                 CategoryName,
                                 IsTimer ? " (Timer)" : string.Empty);
        }
    }
}