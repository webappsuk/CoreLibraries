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
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Formatting;
using WebApplications.Utilities.Performance.Configuration;
using WebApplications.Utilities.Reflect;

namespace WebApplications.Utilities.Performance
{
    /// <summary>
    ///   Performance category helper.
    /// </summary>
    [PublicAPI]
    public abstract class PerfCategory : ResolvableWriteable, IReadOnlyDictionary<string, PerfCounterInfo>
    {
        /// <summary>
        /// Holds all counters.
        /// </summary>
        [NotNull]
        private static readonly ConcurrentDictionary<string, PerfCategory> _categories =
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
        public static readonly string InstanceGuid = Guid.NewGuid().ToString();

        /// <summary>
        /// The machine name used to access performance counters.
        /// </summary>
        [NotNull]
        public const string MachineName = ".";

        /// <summary>
        /// Gets all known counters.
        /// </summary>
        /// <value>All.</value>
        [NotNull]
        public static IEnumerable<PerfCategory> All
        {
            get { return _categories.Values; }
        }

        /// <summary>
        /// Gets all known counter types.
        /// </summary>
        /// <value>All.</value>
        [NotNull]
        public static IEnumerable<Type> AllTypes
        {
            get { return _counterTypes.Keys; }
        }

        /// <summary>
        ///   The performance counter's category.
        /// </summary>
        [NotNull]
        public readonly string CategoryName;

        /// <summary>
        /// Whether the counter is valid (exists and can be accessed).
        /// </summary>
        public bool IsValid
        {
            get { return Counters != null; }
        }

        [CanBeNull]
        private PerformanceCounter[] _counters;

        /// <summary>
        /// The underlying counters.
        /// </summary>
        [CanBeNull]
        [ItemNotNull]
        protected PerformanceCounter[] Counters
        {
            get
            {
                Initialize();
                return _counters;
            }
        }

        private bool _initialized;

        /// <summary>
        /// The info dictionary, holds info by name.
        /// </summary>
        [NotNull]
        private readonly Dictionary<string, PerfCounterInfo> _infoDictionary = new Dictionary<string, PerfCounterInfo>();

        [NotNull]
        private readonly CounterCreationData[] _counterCreationData;

        /// <summary>
        /// Creates a performance counter instance.
        /// </summary>
        /// <param name="categoryName">The performance counter's <see cref="PerfCategory.CategoryName">category name</see>.</param>
        /// <param name="counters">The counters.</param>
        protected PerfCategory([NotNull] string categoryName, [NotNull] IEnumerable<CounterCreationData> counters)
        {
            if (categoryName == null) throw new ArgumentNullException("categoryName");
            if (counters == null) throw new ArgumentNullException("counters");

            CategoryName = categoryName;

            _counterCreationData = counters as CounterCreationData[] ?? counters.ToArray();
        }

        /// <summary>
        /// Initializes this instance on first use.
        /// </summary>
        private void Initialize()
        {
            if (_initialized) return;
            lock (_counterCreationData)
            {
                if (_initialized) return;

                if (!PerformanceConfiguration.IsEnabled ||
                    _counterCreationData.Length < 1)
                {
                    _initialized = true;
                    return;
                }

                // Set up the performance counter(s)
                try
                {
                    /*
                     * This can take a very long time, so we give it 2s!
                     */
                    // ReSharper disable once PossibleNullReferenceException
                    if (!Task.Run((Action)InitializeCounters).Wait(2000))
                        _counters = null;
                }
                catch (UnauthorizedAccessException)
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    Trace.WriteLine(Resources.PerformanceCounterHelper_ProcessDoesNotHaveAccess);
                    _counters = null;
                }
                catch
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    Trace.WriteLine(
                        string.Format(Resources.PerformanceCounterHelper_UnhandledExceptionOccurred, CategoryName));
                    _counters = null;
                }
                finally
                {
                    _initialized = true;
                }
            }
        }

        private void InitializeCounters()
        {
            if (!PerformanceCounterCategory.Exists(CategoryName))
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                Trace.WriteLine(string.Format(Resources.PerformanceCounterHelper_CategoryDoesNotExist, CategoryName));
                return;
            }

            PerformanceCounter[] counters = new PerformanceCounter[_counterCreationData.Length];
            for (int c = 0; c < _counterCreationData.Length; c++)
            {
                CounterCreationData counter = _counterCreationData[c];
                Debug.Assert(counter != null);
                // ReSharper disable once AssignNullToNotNullAttribute
                if (!PerformanceCounterCategory.CounterExists(counter.CounterName, CategoryName))
                {
                    // ReSharper disable AssignNullToNotNullAttribute
                    Trace.WriteLine(
                        string.Format(
                            Resources.PerformanceCounterHelper_CounterDoesNotExist,
                            CategoryName,
                            counter.CounterName));
                    // ReSharper restore AssignNullToNotNullAttribute
                    return;
                }
                counters[c] = new PerformanceCounter
                {
                    CategoryName = CategoryName,
                    CounterName = counter.CounterName,
                    MachineName = MachineName,
                    InstanceLifetime = PerformanceCounterInstanceLifetime.Process,
                    InstanceName = InstanceGuid,
                    ReadOnly = false
                };

                // Read the first value to 'start' the counters.
                counters[c].SafeNextValue();
            }
            _counters = counters;
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
            if (name == null) throw new ArgumentNullException("name");
            if (help == null) throw new ArgumentNullException("help");
            if (getLatestValueFunc == null) throw new ArgumentNullException("getLatestValueFunc");

            if (!_initialized)
                _infoDictionary.Add(name, new PerfCounterInfo<T>(name, help, getLatestValueFunc));
            else
                throw new InvalidOperationException(
                    string.Format(
                        // ReSharper disable once AssignNullToNotNullAttribute
                        Resources.PerformanceCounterHelper_AddInfo_Post_Initialize,
                        CategoryName,
                        name,
                        help));
        }

        /// <summary>
        /// Gets the performance counter with the specified category name.
        /// </summary>
        /// <typeparam name="T">Type of the perf counter.</typeparam>
        /// <param name="categoryName">Name of the category.</param>
        /// <param name="categoryHelp">The category help (only used by PerfSetup).</param>
        /// <returns>The performance counter.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The <typeparamref name="T">performance category type</typeparamref> is not registered as a performance counter type.</exception>
        /// <remarks>
        /// <para>Note that the category help parameter is used by PerfSetup to create and administer counters.  This method
        /// should always be called directly and should always be passed string literals - otherwise the counters cannot be
        /// auto detected.</para>
        /// </remarks>
        [NotNull]

        // ReSharper disable once CodeAnnotationAnalyzer
        public static T GetOrAdd<T>([NotNull] string categoryName, [CanBeNull] string categoryHelp = null)
            where T : PerfCategory
        {
            // NOTE: Cant have Requires here as contract re-writing might change the method name and we need the name to be kept
            Debug.Assert(!string.IsNullOrWhiteSpace(categoryName));
            // ReSharper disable once AssignNullToNotNullAttribute
            PerfCategoryType pct = _counterTypes.GetOrAdd(typeof(T), t => new PerfCategoryType(t));
            Debug.Assert(pct != null);
            if (pct.Exception != null)
                throw pct.Exception;

            // ReSharper disable once AssignNullToNotNullAttribute
            return (T)_categories.GetOrAdd(categoryName, n => pct.Creator(n));
        }

        /// <summary>
        /// Whether the counter category exists.
        /// </summary>
        /// <param name="categoryName">Name of the category.</param>
        /// <returns><see langword="true"/> if the performance category exists; otherwise <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Exists([NotNull] string categoryName)
        {
            if (categoryName == null) throw new ArgumentNullException("categoryName");
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
            where T : PerfCategory
        {
            if (categoryName == null) throw new ArgumentNullException("categoryName");
            return Exists(typeof(T), categoryName);
        }

        /// <summary>
        /// Whether the counter category exists.
        /// </summary>
        /// <param name="perfCategoryType">Type of the performance counter.</param>
        /// <param name="categoryName">Name of the category.</param>
        /// <returns><see langword="true" /> if the performance category exists; otherwise <see langword="false" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Exists([NotNull] Type perfCategoryType, [NotNull] string categoryName)
        {
            if (perfCategoryType == null) throw new ArgumentNullException("perfCategoryType");
            if (categoryName == null) throw new ArgumentNullException("categoryName");

            // ReSharper disable once AssignNullToNotNullAttribute
            PerfCategoryType pct = _counterTypes.GetOrAdd(perfCategoryType, t => new PerfCategoryType(t));
            Debug.Assert(pct != null);
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
            public readonly CounterCreationData[] CreationData = Array<CounterCreationData>.Empty;

            /// <summary>
            /// Creates an instance of the type.
            /// </summary>
            [NotNull]
            public readonly Func<string, PerfCategory> Creator = _invalidCreator;

            [NotNull]
            private static readonly Func<string, PerfCategory> _invalidCreator =
                s => { throw new InvalidOperationException(); };

            /// <summary>
            /// Initializes a new instance of the <see cref="PerfCategoryType" /> class.
            /// </summary>
            /// <param name="type">The type.</param>
            public PerfCategoryType([NotNull] Type type)
            {
                if (type == null) throw new ArgumentNullException("type");
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
                    // ReSharper disable PossibleNullReferenceException
                    // ReSharper disable once AssignNullToNotNullAttribute
                    CreationData = ExtendedType.Get(type)
                        .Fields
                        .Single(
                            f => f.Info.IsStatic &&
                                 f.Info.IsInitOnly &&
                                 f.ReturnType ==
                                 typeof(CounterCreationData[]))
                        .Getter<CounterCreationData[]>()();
                    // ReSharper restore PossibleNullReferenceException
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
            return _infoDictionary.TryGetValue(key, out value);
        }

        /// <summary>
        /// Gets the element that has the specified key in the read-only dictionary.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>PerfCounterInfo.</returns>
        public PerfCounterInfo this[[NotNull] string key]
        {
            get
            {
                if (key == null) throw new ArgumentNullException("key");
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
        /// <param name="context">The context.</param>
        /// <param name="chunk">The chunk.</param>
        /// <returns>An object that will be cached unless it is a <see cref="T:WebApplications.Utilities.Formatting.Resolution" />.</returns>
        // ReSharper disable once CodeAnnotationAnalyzer
        public override object Resolve(FormatWriteContext context, FormatChunk chunk)
        {
            // ReSharper disable once PossibleNullReferenceException
            switch (chunk.Tag.ToLowerInvariant())
            {
                case "default":
                    return VerboseFormat;
                case "short":
                    return ShortFormat;
                case "categoryname":
                    return CategoryName;
                case "instanceguid":
                    return InstanceGuid;
                case "info":
                    return _infoDictionary.Values.Count > 0 ? _infoDictionary.Values : Resolution.Null;
                default:
                    return Resolution.Unknown;
            }
        }
    }
}