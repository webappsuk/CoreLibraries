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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using WebApplications.Utilities.Reflect;

namespace WebApplications.Utilities.Performance
{
    /// <summary>
    ///   Performance counters used for operations.
    /// </summary>
    public abstract class PerfCounter
    {
        /// <summary>
        /// Holds all counters.
        /// </summary>
        [NotNull]
        private static readonly ConcurrentDictionary<string, PerfCounter> _counters =
            new ConcurrentDictionary<string, PerfCounter>();

        /// <summary>
        /// The counter types.
        /// </summary>
        [NotNull]
        private static readonly ConcurrentDictionary<Type, PerfCounterType> _counterTypes = new ConcurrentDictionary<Type, PerfCounterType>();

        /// <summary>
        /// The current instance name for all performance counters.
        /// </summary>
        [NotNull]
        public static readonly string InstanceGuid = Guid.NewGuid().ToString();

        /// <summary>
        /// The machine name used to access performance counters.
        /// </summary>
        [NotNull]
        public const string MachineName = ".";

        /// <summary>
        /// Whether the current process has access to performance counters.
        /// </summary>
        public static readonly bool HasAccess;

        /// <summary>
        /// Gets all known counters.
        /// </summary>
        /// <value>All.</value>
        public static IEnumerable<PerfCounter> All { get { return _counters.Values; } }

        /// <summary>
        /// Gets all known counter types.
        /// </summary>
        /// <value>All.</value>
        public static IEnumerable<Type> AllTypes { get { return _counterTypes.Keys; } }

        /// <summary>
        /// Initializes static members of the <see cref="PerfCounter" /> class.
        /// </summary>
        /// <remarks>We only check access to performance counters once.</remarks>
        static PerfCounter()
        {
            try
            {
                // Check we have access to the performance counters.
                PerformanceCounterCategory.Exists("TestAccess", MachineName);
                HasAccess = true;
                Trace.WriteLine(string.Format(Resources.PerformanceCounterHelper_Enabled, InstanceGuid));
            }
            catch
            {
                Trace.WriteLine(Resources.PerformanceCounterHelper_ProcessDoesNotHaveAccess);
                HasAccess = false;
            }
        }

        /// <summary>
        ///   The performance counter's category.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public readonly string CategoryName;

        /// <summary>
        /// Whether the counter is valid (exists and can be accessed).
        /// </summary>
        public readonly bool IsValid;

        /// <summary>
        /// The underlying counters.
        /// </summary>
        [NotNull]
        protected readonly PerformanceCounter[] Counters;

        /// <summary>
        /// Creates a performance counter instance.
        /// </summary>
        /// <param name="categoryName">The performance counter's <see cref="PerfCounter.CategoryName">category name</see>.</param>
        /// <param name="counters">The counters.</param>
        protected PerfCounter([NotNull] string categoryName, [NotNull]IEnumerable<CounterCreationData> counters)
        {
            Contract.Requires(categoryName != null);
            Contract.Requires(counters != null);
            CategoryName = categoryName;
            if (!HasAccess)
                return;

            CounterCreationData[] cArray = counters as CounterCreationData[] ?? counters.ToArray();
            if (cArray.Length < 1)
            {
                IsValid = false;
                return;
            }

            // Set up the performance counter(s)
            try
            {
                if (!PerformanceCounterCategory.Exists(CategoryName))
                {
                    Trace.WriteLine(string.Format(Resources.PerformanceCounterHelper_CategoryDoesNotExist, CategoryName));
                    IsValid = false;
                    return;
                }

                Counters = new System.Diagnostics.PerformanceCounter[cArray.Length];
                for (int c = 0; c < cArray.Length; c++)
                {
                    CounterCreationData counter = cArray[c];
                    if (!PerformanceCounterCategory.CounterExists(counter.CounterName, categoryName))
                    {
                        Trace.WriteLine(string.Format(Resources.PerformanceCounterHelper_CounterDoesNotExist, CategoryName, counter.CounterName));
                        IsValid = false;
                        return;
                    }
                    Counters[c] = new System.Diagnostics.PerformanceCounter()
                        {
                            CategoryName = categoryName,
                            CounterName = counter.CounterName,
                            MachineName = MachineName,
                            InstanceLifetime = PerformanceCounterInstanceLifetime.Process,
                            InstanceName = InstanceGuid,
                            ReadOnly = false
                        };

                    // Read the first value to 'start' the counters.
                    Counters[c].NextValue();
                }
                IsValid = true;
            }
            catch (UnauthorizedAccessException)
            {
                Trace.WriteLine(Resources.PerformanceCounterHelper_ProcessDoesNotHaveAccess);
                IsValid = false;
            }
            catch
            {
                Trace.WriteLine(string.Format(Resources.PerformanceCounterHelper_UnhandledExceptionOccurred, CategoryName));
                IsValid = false;
            }
        }

        /// <summary>
        /// Gets the performance counter with the specified category name.
        /// </summary>
        /// <typeparam name="T">Type of the perf counter.</typeparam>
        /// <param name="categoryName">Name of the category.</param>
        /// <param name="categoryHelp">The category help (only used by PerfSetup).</param>
        /// <returns>The performance counter.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The <see paramref="perfCounterType" /> is not registered as a performance counter type.</exception>
        /// <remarks>
        /// <para>Note that the category help parameter is used by PerfSetup to create and administer counters.  This method
        /// should always be called directly and should always be passed string literals - otherwise the counters cannot be
        /// auto detected.</para>
        /// </remarks>
        [NotNull]
        public static T GetOrAdd<T>([NotNull]string categoryName, string categoryHelp = null)
            where T : PerfCounter
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(categoryHelp));
            PerfCounterType pct = _counterTypes.GetOrAdd(typeof(T), t => new PerfCounterType(t));
            if (pct.Exception != null)
                throw pct.Exception;

            return (T)_counters.GetOrAdd(categoryName, n => pct.Creator(n));
        }

        /// <summary>
        /// Whether the counter category exists.
        /// </summary>
        /// <param name="categoryName">Name of the category.</param>
        /// <returns><see langword="true"/> if the performance category exists; otherwise <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Exists([NotNull] string categoryName)
        {
            Contract.Requires(categoryName != null);
            return PerformanceCounterCategory.Exists(categoryName);
        }

        /// <summary>
        /// Whether the counter category exists.
        /// </summary>
        /// <typeparam name="T">Type of the perf counter.</typeparam>
        /// <param name="categoryName">Name of the category.</param>
        /// <returns><see langword="true" /> if the performance category exists; otherwise <see langword="false" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Exists<T>([NotNull] string categoryName)
            where T : PerfCounter
        {
            Contract.Requires(categoryName != null);
            return Exists(typeof (T), categoryName);
        }

        /// <summary>
        /// Whether the counter category exists.
        /// </summary>
        /// <param name="perfCounterType">Type of the perf counter.</param>
        /// <param name="categoryName">Name of the category.</param>
        /// <returns><see langword="true" /> if the performance category exists; otherwise <see langword="false" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Exists([NotNull]Type perfCounterType, [NotNull] string categoryName)
        {
            Contract.Requires(perfCounterType != null);
            Contract.Requires(categoryName != null);

            PerfCounterType pct = _counterTypes.GetOrAdd(perfCounterType, t => new PerfCounterType(t));
            if (pct.Exception != null)
                throw pct.Exception;

            return PerformanceCounterCategory.Exists(categoryName) &&
                   pct.CreationData.All(c => PerformanceCounterCategory.CounterExists(c.CounterName, categoryName));
        }

        /// <summary>
        /// Holds information about performance counter types.
        /// </summary>
        private class PerfCounterType
        {
            /// <summary>
            /// The type.
            /// </summary>
            public readonly Type Type;

            /// <summary>
            /// The exception (if any)
            /// </summary>
            public readonly Exception Exception;

            /// <summary>
            /// Creates an instance of the type.
            /// </summary>
            [NotNull]
            public readonly CounterCreationData[] CreationData;

            /// <summary>
            /// Creates an instance of the type.
            /// </summary>
            [NotNull]
            public readonly Func<string, PerfCounter> Creator;

            /// <summary>
            /// Initializes a new instance of the <see cref="PerfCounterType" /> class.
            /// </summary>
            /// <param name="type">The type.</param>
            public PerfCounterType([NotNull] Type type)
            {
                Type = type;
                if ((type == typeof(PerfCounter)) ||
                    !type.DescendsFrom(typeof(PerfCounter)))
                {
                    Exception =
                        new InvalidOperationException(
                            string.Format("The performance counter type '{0}' does not descend from PerfCounter.",
                                          type.FullName));
                    return;
                }

                // Try to get constructor that takes a string.
                try
                {
                    Creator = type.ConstructorFunc<string, PerfCounter>(true);
                }
                catch (Exception e)
                {
                    Exception =
                        new InvalidOperationException(
                            string.Format("The performance counter type '{0}' does not have a constructor that takes a string.",
                                          type.FullName), e);
                    return;
                }

                try
                {
                    CreationData = ExtendedType.Get(type)
                                               .Fields
                                               .Single(f => f.Info.IsStatic &&
                                                            f.ReturnType ==
                                                            typeof (CounterCreationData[]))
                                               .Getter<CounterCreationData[]>()();
                }
                catch (Exception e)
                {
                    Exception =
                        new InvalidOperationException(
                            string.Format("The performance counter type '{0}' does not have a single static field of type CounterCreationData[].",
                                          type.FullName), e);
                    return;
                }
            }
        }
    }
}