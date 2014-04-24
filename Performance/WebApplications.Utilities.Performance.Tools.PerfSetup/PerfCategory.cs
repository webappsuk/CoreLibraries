#region © Copyright Web Applications (UK) Ltd, 2014.  All rights reserved.
// Copyright (c) 2014, Web Applications UK Ltd
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
using System.Linq;
using JetBrains.Annotations;
using Mono.Cecil;

namespace WebApplications.Utilities.Performance.Tools.PerfSetup
{
    /// <summary>
    /// Performance category information for an individual set of counters.
    /// </summary>
    internal class PerfCategory
    {
        /// <summary>
        /// Holds information by category.
        /// </summary>
        [NotNull]
        private static readonly ConcurrentDictionary<string, PerfCategory> _categories =
            new ConcurrentDictionary<string, PerfCategory>();

        /// <summary>
        /// The assemblies referencing the counter.
        /// </summary>
        private readonly List<string> _assemblies;

        /// <summary>
        /// The category name.
        /// </summary>
        [NotNull]
        public IEnumerable<string> Assemblies
        {
            get { return _assemblies; }
        }

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
        /// Initializes a new instance of the <see cref="PerfCategory" /> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="performanceType">Type of the performance.</param>
        /// <param name="categoryName">Name of the category.</param>
        private PerfCategory(
            [NotNull] string assembly,
            [NotNull] PerformanceType performanceType,
            [NotNull] string categoryName)
        {
            _assemblies = new List<string> {assembly};
            PerformanceType = performanceType;
            CategoryName = categoryName;
        }

        /// <summary>
        /// Adds or updates a performance counter categories information.
        /// </summary>
        /// <param name="categoryName">Name of the category.</param>
        /// <param name="typeReference">The type reference.</param>
        /// <param name="assembly">The assembly.</param>
        /// <param name="categoryHelp">The category help.</param>
        public static void Set(
            [NotNull] string categoryName,
            [NotNull] TypeReference typeReference,
            [NotNull] string assembly,
            string categoryHelp)
        {
            try
            {
                PerformanceType performanceType = PerformanceType.Get(typeReference);
                PerfCategory category = _categories.GetOrAdd(
                    categoryName,
                    n => new PerfCategory(assembly, performanceType, n));

                // Type cannot change.
                if (category.PerformanceType != performanceType)
                {
                    Logger.Add(
                        Level.Error,
                        "The '{0}' performance counter category was declared more than once with different types ('{1}' and '{2}') in assembly '{3}'.",
                        categoryName,
                        category.PerformanceType,
                        performanceType,
                        assembly);
                    return;
                }

                // Only update category help if not already set.
                if (!string.IsNullOrWhiteSpace(categoryHelp) &&
                    string.IsNullOrWhiteSpace(category.CategoryHelp))
                    category.CategoryHelp = categoryHelp;

                // Add assembly if not seen before
                if (!category._assemblies.Contains(assembly))
                    category._assemblies.Add(assembly);
            }
            catch (Exception e)
            {
                Logger.Add(
                    Level.Error,
                    "Failed to set performance category information for category '{0}' in assembly '{1}'. {2}",
                    categoryName,
                    assembly,
                    e.Message);
            }
        }

        /// <summary>
        /// Gets all categories.
        /// </summary>
        /// <value>All.</value>
        public static IEnumerable<PerfCategory> All
        {
            get { return _categories.Values; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="PerfCategory" /> is exists.
        /// </summary>
        /// <value><see langword="true" /> if exists; otherwise, <see langword="false" />.</value>
        public bool Exists
        {
            get
            {
                return PerformanceType.IsValid &&
                       (PerformanceCounterCategory.Exists(CategoryName)) &&
                       (PerformanceType.CounterCreationData.All(
                           c => PerformanceCounterCategory.CounterExists(c.CounterName, CategoryName)));
            }
        }

        public bool Created { get; private set; }

        /// <summary>
        /// Creates the counters on the specified machine name.
        /// </summary>
        /// <param name="machineName">Name of the machine.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public bool Create(string machineName)
        {
            if (!PerformanceType.IsValid)
                return false;
            if (Created) return true;
            try
            {
                if (PerformanceCounterCategory.Exists(CategoryName))
                {
                    if (
                        PerformanceType.CounterCreationData.All(
                            c => PerformanceCounterCategory.CounterExists(c.CounterName, CategoryName)))
                        return true;

                    // We don't have all the counters, so drop and recreate.
                    if (!Delete(machineName))
                        return false;
                }

                PerformanceCounterCategory.Create(
                    CategoryName,
                    CategoryHelp,
                    PerformanceCounterCategoryType.MultiInstance,
                    new CounterCreationDataCollection(PerformanceType.CounterCreationData));

                Created =
                    PerformanceType.CounterCreationData.All(
                        c => PerformanceCounterCategory.CounterExists(c.CounterName, CategoryName));
                Deleted = false;
                return Created;
            }
            catch
            {
                Created = false;
                return false;
            }
        }

        public bool Deleted { get; private set; }

        /// <summary>
        /// Deletes the counters from specified machine name.
        /// </summary>
        /// <param name="machineName">Name of the machine.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public bool Delete(string machineName)
        {
            if (!PerformanceType.IsValid)
            {
                Deleted = false;
                return false;
            }
            if (Deleted) return true;
            try
            {
                if (!PerformanceCounterCategory.Exists(CategoryName))
                    return true;

                PerformanceCounterCategory.Delete(CategoryName);
                Deleted = !PerformanceCounterCategory.Exists(CategoryName);
                Created = false;
                return Deleted;
            }
            catch
            {
                Deleted = false;
                return false;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format(
                "{0}: {1} [{2}]",
                string.Join(",", _assemblies),
                CategoryName,
                PerformanceType);
        }
    }
}