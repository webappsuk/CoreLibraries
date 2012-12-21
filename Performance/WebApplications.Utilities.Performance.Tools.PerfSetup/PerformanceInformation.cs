using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using Mono.Cecil;

namespace WebApplications.Utilities.Performance.Tools.PerfSetup
{
    /// <summary>
    /// Performance information for an individual set of counters.
    /// </summary>
    internal class PerformanceInformation
    {
        /// <summary>
        /// Holds information by category.
        /// </summary>
        [NotNull]
        private static readonly ConcurrentDictionary<string, PerformanceInformation> _categories = new ConcurrentDictionary<string, PerformanceInformation>();

        /// <summary>
        /// The assemblies referencing the counter.
        /// </summary>
        private readonly List<string> _assemblies;

        /// <summary>
        /// The category name.
        /// </summary>
        [NotNull]
        public IEnumerable<string> Assemblies { get { return _assemblies; } }

        [NotNull]
        public readonly PerformanceType PerformanceType;

        /// <summary>
        /// The category name.
        /// </summary>
        [NotNull]
        public readonly string CategoryName;

        /// <summary>
        /// The category help.
        /// </summary>
        public string CategoryHelp { get; private set; }

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
        private PerformanceInformation([NotNull]string assembly, [NotNull] PerformanceType performanceType, [NotNull]string categoryName)
        {
            _assemblies = new List<string> {assembly};
            PerformanceType = performanceType;
            CategoryName = categoryName;
        }

        public static void Set([NotNull] string categoryName, [NotNull] TypeReference typeReference, [NotNull] string assembly, string categoryHelp)
        {
            PerformanceType performanceType = PerformanceType.Get(typeReference);
            var i = _categories.GetOrAdd(
                categoryName,
                n => new PerformanceInformation(assembly, performanceType, n));

            // Type cannot change.
            if (i.PerformanceType != performanceType)
                throw new InvalidOperationException(
                    string.Format(
                        "The '{0}' performance counter was declared more than once with different types ('{1}' and '{2}').",
                        categoryName,
                        i.PerformanceType,
                        performanceType));

            // Only update category help if not already set.
            if (!string.IsNullOrWhiteSpace(categoryHelp) &&
                string.IsNullOrWhiteSpace(i.CategoryHelp))
                i.CategoryHelp = categoryHelp;

            // Add assembly if not seen before
            if (!i._assemblies.Contains(assembly))
                i._assemblies.Add(assembly);
        }

        /// <summary>
        /// Gets all categories.
        /// </summary>
        /// <value>All.</value>
        public static IEnumerable<PerformanceInformation> All
        {
            get { return _categories.Values; }
        }

        public bool Exists
        {
            get
            {
                return (PerformanceCounterCategory.Exists(CategoryName)) &&
                       (PerformanceType.CounterCreationData.All(c => PerformanceCounterCategory.CounterExists(c.CounterName, CategoryName)));
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
                    if (PerformanceType.CounterCreationData.All(c => PerformanceCounterCategory.CounterExists(c.CounterName, CategoryName)))
                        return true;

                    // We don't have all the counters, so drop and recreate.
                    if (!Delete(machineName))
                        return false;
                }

                PerformanceCounterCategory.Create(CategoryName, CategoryHelp,
                                                  PerformanceCounterCategoryType.MultiInstance,
                                                  new CounterCreationDataCollection(PerformanceType.CounterCreationData));

                return PerformanceType.CounterCreationData.All(c => PerformanceCounterCategory.CounterExists(c.CounterName, CategoryName));
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
                                 string.Join(",", _assemblies),
                                 CategoryName,
                                 IsTimer ? " (Timer)" : string.Empty);
        }
    }
}