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
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using WebApplications.Utilities.Formatting;
using WebApplications.Utilities.Reflect;

namespace WebApplications.Utilities.Performance
{
    /// <summary>
    ///   Performance category helper.
    /// </summary>
    public abstract class PerfCategory : ResolvableWriteable, IReadOnlyDictionary<string, PerfCounterInfo>
    {
        /// <summary>
        /// Holds all counters.
        /// </summary>
        [NotNull]
        private static readonly ConcurrentDictionary<string, PerfCategory> _counters =
            new ConcurrentDictionary<string, PerfCategory>();

        /// <summary>
        /// The counter types.
        /// </summary>
        [NotNull]
        private static readonly ConcurrentDictionary<Type, PerfCategoryType> _counterTypes =
            new ConcurrentDictionary<Type, PerfCategoryType>();

        /// <summary>
        /// The current instance name for all performance counters.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly string InstanceGuid = Guid.NewGuid().ToString();

        /// <summary>
        /// The machine name used to access performance counters.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public const string MachineName = ".";

        /// <summary>
        /// Whether the current process has access to performance counters.
        /// </summary>
        [PublicAPI]
        public static readonly bool HasAccess;

        /// <summary>
        /// Gets all known counters.
        /// </summary>
        /// <value>All.</value>
        [NotNull]
        [PublicAPI]
        public static IEnumerable<PerfCategory> All
        {
            get { return _counters.Values; }
        }

        /// <summary>
        /// Gets all known counter types.
        /// </summary>
        /// <value>All.</value>
        [NotNull]
        [PublicAPI]
        public static IEnumerable<Type> AllTypes
        {
            get { return _counterTypes.Keys; }
        }

        /// <summary>
        /// Initializes static members of the <see cref="PerfCategory" /> class.
        /// </summary>
        /// <remarks>We only check access to performance counters once.</remarks>
        static PerfCategory()
        {
            try
            {
                // Check we have access to the performance counters.
                PerformanceCounterCategory.Exists("TestAccess", MachineName);
                HasAccess = true;
                // ReSharper disable once AssignNullToNotNullAttribute
                Trace.WriteLine(string.Format(Resources.PerformanceCounterHelper_Enabled, InstanceGuid));
            }
            catch
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                Trace.WriteLine(Resources.PerformanceCounterHelper_ProcessDoesNotHaveAccess);
                HasAccess = false;
            }
        }

        /// <summary>
        ///   The performance counter's category.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public readonly string CategoryName;

        /// <summary>
        /// Whether the counter is valid (exists and can be accessed).
        /// </summary>
        [PublicAPI]
        public readonly bool IsValid;

        /// <summary>
        /// The underlying counters.
        /// </summary>
        [NotNull]
        [PublicAPI]
        protected readonly PerformanceCounter[] Counters;

        /// <summary>
        /// The info dictionary, holds info by name.
        /// </summary>
        [NotNull]
        private readonly Dictionary<string, PerfCounterInfo> _infoDictionary = new Dictionary<string, PerfCounterInfo>();

        /// <summary>
        /// Creates a performance counter instance.
        /// </summary>
        /// <param name="categoryName">The performance counter's <see cref="PerfCategory.CategoryName">category name</see>.</param>
        /// <param name="counters">The counters.</param>
        protected PerfCategory([NotNull] string categoryName, [NotNull] IEnumerable<CounterCreationData> counters)
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
                    // ReSharper disable once AssignNullToNotNullAttribute
                    Trace.WriteLine(
                        string.Format(Resources.PerformanceCounterHelper_CategoryDoesNotExist, CategoryName));
                    IsValid = false;
                    return;
                }

                Counters = new PerformanceCounter[cArray.Length];
                for (int c = 0; c < cArray.Length; c++)
                {
                    CounterCreationData counter = cArray[c];
                    Contract.Assert(counter != null);
                    // ReSharper disable once AssignNullToNotNullAttribute
                    if (!PerformanceCounterCategory.CounterExists(counter.CounterName, categoryName))
                    {
                        Trace.WriteLine(
                            string.Format(
                            // ReSharper disable once AssignNullToNotNullAttribute
                                Resources.PerformanceCounterHelper_CounterDoesNotExist,
                                CategoryName,
                                counter.CounterName));
                        IsValid = false;
                        return;
                    }
                    Counters[c] = new PerformanceCounter
                    {
                        CategoryName = categoryName,
                        CounterName = counter.CounterName,
                        MachineName = MachineName,
                        InstanceLifetime = PerformanceCounterInstanceLifetime.Process,
                        InstanceName = InstanceGuid,
                        ReadOnly = false
                    };

                    // Read the first value to 'start' the counters.
                    Counters[c].SafeNextValue();
                }
                IsValid = true;
            }
            catch (UnauthorizedAccessException)
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                Trace.WriteLine(Resources.PerformanceCounterHelper_ProcessDoesNotHaveAccess);
                IsValid = false;
            }
            catch
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                Trace.WriteLine(
                    string.Format(Resources.PerformanceCounterHelper_UnhandledExceptionOccurred, CategoryName));
                IsValid = false;
            }
        }

        /// <summary>
        /// Adds the information.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="help">The help.</param>
        /// <param name="getLatestValueFunc">The get latest value function.</param>
        /// <remarks>
        /// You must only call this during construction.
        /// </remarks>
        protected void AddInfo<T>([NotNull] string name, [NotNull] string help, [NotNull] Func<T> getLatestValueFunc)
        {
            Contract.Requires(name != null);
            Contract.Requires(help != null);
            Contract.Requires(getLatestValueFunc != null);
            _infoDictionary.Add(name, new PerfCounterInfo<T>(name, help, getLatestValueFunc));
        }

        /// <summary>
        /// Gets the performance counter with the specified category name.
        /// </summary>
        /// <typeparam name="T">Type of the perf counter.</typeparam>
        /// <param name="categoryName">Name of the category.</param>
        /// <param name="categoryHelp">The category help (only used by PerfSetup).</param>
        /// <returns>The performance counter.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The <see paramref="PerfCategoryType" /> is not registered as a performance counter type.</exception>
        /// <remarks>
        /// <para>Note that the category help parameter is used by PerfSetup to create and administer counters.  This method
        /// should always be called directly and should always be passed string literals - otherwise the counters cannot be
        /// auto detected.</para>
        /// </remarks>
        [NotNull]
        [PublicAPI]
        // ReSharper disable once CodeAnnotationAnalyzer
        public static T GetOrAdd<T>([NotNull] string categoryName, [CanBeNull] string categoryHelp = null)
            where T : PerfCategory
        {
            // NOTE: Cant have Requires here as contract re-writing might change the method name and we need the name to be kept
            Contract.Assert(!string.IsNullOrWhiteSpace(categoryName));
            // ReSharper disable once AssignNullToNotNullAttribute
            PerfCategoryType pct = _counterTypes.GetOrAdd(typeof(T), t => new PerfCategoryType(t));
            Contract.Assert(pct != null);
            if (pct.Exception != null)
                throw pct.Exception;

            // ReSharper disable once AssignNullToNotNullAttribute
            return (T)_counters.GetOrAdd(categoryName, n => pct.Creator(n));
        }

        /// <summary>
        /// Whether the counter category exists.
        /// </summary>
        /// <param name="categoryName">Name of the category.</param>
        /// <returns><see langword="true"/> if the performance category exists; otherwise <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [PublicAPI]
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
        [PublicAPI]
        public static bool Exists<T>([NotNull] string categoryName)
            where T : PerfCategory
        {
            Contract.Requires(categoryName != null);
            return Exists(typeof(T), categoryName);
        }

        /// <summary>
        /// Whether the counter category exists.
        /// </summary>
        /// <param name="perfCategoryType">Type of the performance counter.</param>
        /// <param name="categoryName">Name of the category.</param>
        /// <returns><see langword="true" /> if the performance category exists; otherwise <see langword="false" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [PublicAPI]
        public static bool Exists([NotNull] Type perfCategoryType, [NotNull] string categoryName)
        {
            Contract.Requires(perfCategoryType != null);
            Contract.Requires(categoryName != null);

            // ReSharper disable once AssignNullToNotNullAttribute
            PerfCategoryType pct = _counterTypes.GetOrAdd(perfCategoryType, t => new PerfCategoryType(t));
            Contract.Assert(pct != null);
            if (pct.Exception != null)
                throw pct.Exception;

            return PerformanceCounterCategory.Exists(categoryName) &&
                // ReSharper disable once PossibleNullReferenceException,  AssignNullToNotNullAttribute
                   pct.CreationData.All(c => PerformanceCounterCategory.CounterExists(c.CounterName, categoryName));
        }

        /// <summary>
        /// Holds information about performance counter types.
        /// </summary>
        private class PerfCategoryType
        {
            /// <summary>
            /// The type.
            /// </summary>
            [PublicAPI]
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
            public readonly Func<string, PerfCategory> Creator;

            /// <summary>
            /// Initializes a new instance of the <see cref="PerfCategoryType" /> class.
            /// </summary>
            /// <param name="type">The type.</param>
            public PerfCategoryType([NotNull] Type type)
            {
                Contract.Requires(type != null);
                Type = type;
                if ((type == typeof(PerfCategory)) ||
                    !type.DescendsFrom(typeof(PerfCategory)))
                {
                    Exception =
                        new InvalidOperationException(
                            string.Format(
                        // ReSharper disable once AssignNullToNotNullAttribute
                                Resources.PerfCategoryType_Must_Descend_From_PerfCategory,
                                type.FullName));
                    return;
                }

                // Try to get constructor that takes a string.
                try
                {
                    Creator = type.ConstructorFunc<string, PerfCategory>(true);
                }
                catch (Exception e)
                {
                    Exception =
                        new InvalidOperationException(
                            string.Format(
                        // ReSharper disable once AssignNullToNotNullAttribute
                                Resources.PerfCategoryType_Invalid_Constructor,
                                type.FullName),
                            e);
                    return;
                }

                try
                {
                    // ReSharper disable once PossibleNullReferenceException
                    CreationData = ExtendedType.Get(type)
                        .Fields
                        .Single(
                        // ReSharper disable once PossibleNullReferenceException
                            f => f.Info.IsStatic &&
                                 f.Info.IsInitOnly &&
                                 f.ReturnType ==
                                 typeof(CounterCreationData[]))
                        .Getter<CounterCreationData[]>()();
                }
                catch (Exception e)
                {
                    // Store the exception.
                    Exception =
                        new InvalidOperationException(
                            string.Format(
                        // ReSharper disable once AssignNullToNotNullAttribute
                                Resources.PerfCategoryType_Missing_Static_Readonly_Field,
                                type.FullName),
                            e);
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<KeyValuePair<string, PerfCounterInfo>> GetEnumerator()
        {
            return _infoDictionary.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        /// <value>The count.</value>
        /// <returns>The number of elements in the collection. </returns>
        public int Count
        {
            get { return _infoDictionary.Count; }
        }

        /// <summary>
        /// Determines whether the read-only dictionary contains an element that has the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>true if the read-only dictionary contains an element that has the specified key; otherwise, false.</returns>
        public bool ContainsKey([NotNull] string key)
        {
            Contract.Requires(key != null);
            return _infoDictionary.ContainsKey(key);
        }

        /// <summary>
        /// Gets the value that is associated with the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value" /> parameter. This parameter is passed uninitialized.</param>
        /// <returns>true if the object that implements the <see cref="T:System.Collections.Generic.IReadOnlyDictionary`2" /> interface contains an element that has the specified key; otherwise, false.</returns>
        public bool TryGetValue([NotNull] string key, out PerfCounterInfo value)
        {
            Contract.Requires(key != null);
            return _infoDictionary.TryGetValue(key, out value);
        }

        /// <summary>
        /// Gets the element that has the specified key in the read-only dictionary.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>PerfCounterInfo.</returns>
        public PerfCounterInfo this[string key]
        {
            get
            {
                Contract.Requires(key != null);
                PerfCounterInfo value;
                return _infoDictionary.TryGetValue(key, out value) ? value : null;
            }
        }

        /// <summary>
        /// Gets an enumerable collection that contains the keys in the read-only dictionary.
        /// </summary>
        /// <value>The keys.</value>
        /// <returns>An enumerable collection that contains the keys in the read-only dictionary.</returns>
        [NotNull]
        public IEnumerable<string> Keys
        {
            get { return _infoDictionary.Keys; }
        }

        /// <summary>
        /// Gets an enumerable collection that contains the values in the read-only dictionary.
        /// </summary>
        /// <value>The values.</value>
        /// <returns>An enumerable collection that contains the values in the read-only dictionary.</returns>
        [NotNull]
        public IEnumerable<PerfCounterInfo> Values
        {
            get { return _infoDictionary.Values; }
        }

        /// <summary>
        /// The default builder for writing out a performance category.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly FormatBuilder VerboseFormat =
            new FormatBuilder(int.MaxValue, 22, tabStops: new[] { 3, 20, 22 })
                .AppendForegroundColor(Color.Yellow)
                .AppendFormat("{CategoryName}")
                .AppendResetForegroundColor()
                .AppendFormat("{Info:{<items>:\r\n\t{Name}\t: {Value}}}")
                .MakeReadOnly();

        /// <summary>
        /// The short format.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly FormatBuilder ShortFormat = new FormatBuilder()
                .AppendForegroundColor(Color.Yellow)
                .AppendFormat("{CategoryName}")
                .AppendResetForegroundColor()
                .AppendFormat("{Info:: {<items>:{Name}={Value}}{<join>:, }}")
                .MakeReadOnly();

        /// <summary>
        /// Gets the default format.
        /// </summary>
        /// <value>The default format.</value>
        public override FormatBuilder DefaultFormat
        {
            get { return VerboseFormat; }
        }

        /// <summary>
        /// Resolves the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>A <see cref="T:WebApplications.Utilities.Formatting.Resolution" />.</returns>
        /// <requires csharp="tag != null" vb="tag &lt;&gt; Nothing">tag != null</requires>
        // ReSharper disable once CodeAnnotationAnalyzer
        protected override Resolution Resolve(string tag)
        {
            // ReSharper disable once PossibleNullReferenceException
            switch (tag.ToLowerInvariant())
            {
                case "default":
                    return new Resolution(VerboseFormat);
                case "short":
                    return new Resolution(ShortFormat);
                case "categoryname":
                    return new Resolution(CategoryName);
                case "instanceguid":
                    return new Resolution(InstanceGuid);
                case "info":
                    return new Resolution(_infoDictionary.Values.Count > 0 ? _infoDictionary.Values : null);
                default:
                    return Resolution.Unknown;
            }
        }
    }
}