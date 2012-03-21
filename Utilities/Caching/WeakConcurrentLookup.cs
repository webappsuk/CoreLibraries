#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities
// File: WeakConcurrentLookup.cs
// 
// This software, its object code and source code and all modifications made to
// the same (the “Software”) are, and shall at all times remain, the proprietary
// information and intellectual property rights of Web Applications (UK) Limited. 
// You are only entitled to use the Software as expressly permitted by Web
// Applications (UK) Limited within the Software Customisation and
// Licence Agreement (the “Agreement”).  Any copying, modification, decompiling,
// distribution, licensing, sale, transfer or other use of the Software other than
// as expressly permitted in the Agreement is expressly forbidden.  Web
// Applications (UK) Limited reserves its rights to take action against you and
// your employer in accordance with its contractual and common law rights
// (including injunctive relief) should you breach the terms of the Agreement or
// otherwise infringe its copyright or other intellectual property rights in the
// Software.
// 
// © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
#endregion

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using WebApplications.Utilities.Enumerations;

namespace WebApplications.Utilities.Caching
{
    /// <summary>
    ///   A concurrent lookup, which allows a set of weak referenced objects to be grouped and manipulated by a key. 
    ///   If TValue implements <see cref="WebApplications.Utilities.Caching.IObservableFinalize"/> then items are
    ///   removed automatically, otherwise they are only removed when attempting to access (e.g. through iterating
    ///   through the collection, or 'counting', etc.)
    /// </summary>
    /// <typeparam name="TKey">The of the key.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    [UsedImplicitly]
    public class WeakConcurrentLookup<TKey, TValue> : ILookup<TKey, TValue> where TValue : class
    {
        /// <summary>
        ///   A <see cref="bool"/> to indicate whether the lookup supports resurrection.
        /// </summary>
        private readonly bool _allowResurrection;

        /// <summary>
        ///   The underlying weak references.
        /// </summary>
        [NotNull] private readonly ConcurrentDictionary<TKey, WeakGrouping> _dictionary;

        /// <summary>
        ///   A <see cref="bool"/> to indicate whether we are observing finalize.
        /// </summary>
        private readonly bool _observable;

        /// <summary>
        ///   The value comparer, used to check for equality.
        /// </summary>
        [NotNull] private readonly IEqualityComparer<TValue> _valueComparer;

        /// <summary>
        ///   Initializes a new instance of the <see cref="WeakConcurrentLookup&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="concurrencyLevel">The estimated number of threads that will update the lookup concurrently.</param>
        /// <param name="capacity">The initial number of elements that the lookup can contain.</param>
        /// <param name="comparer">The comparer to use when comparing keys.</param>
        /// <param name="valueComparer">The value comparer, used to check for equality.</param>
        /// <param name="allowResurrection">
        ///   If set to <see langword="true"/> then allow resurrections.
        ///   If unset will allow resurrection if the type does not support dispose.
        /// </param>
        public WeakConcurrentLookup(
            int concurrencyLevel = 0,
            int capacity = 0,
            [CanBeNull] IEqualityComparer<TKey> comparer = null,
            [CanBeNull] IEqualityComparer<TValue> valueComparer = null,
            TriState allowResurrection = default(TriState))
            : this(null, concurrencyLevel, capacity, comparer, valueComparer, allowResurrection)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="WeakConcurrentLookup&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="collection">The collection to copy into the lookup.</param>
        /// <param name="concurrencyLevel">The estimated number of threads that will update the lookup concurrently.</param>
        /// <param name="capacity">The initial number of elements that the lookup can contain.</param>
        /// <param name="comparer">The comparer to use when comparing keys.</param>
        /// <param name="valueComparer">The value comparer, used to check for equality.</param>
        /// <param name="allowResurrection">
        ///   If set to <see langword="true"/> then allow resurrections.
        ///   If unset will allow resurrection if the type does not support dispose.
        /// </param>
        public WeakConcurrentLookup(
            [CanBeNull] IEnumerable<KeyValuePair<TKey, TValue>> collection,
            int concurrencyLevel = 0,
            int capacity = 0,
            [CanBeNull] IEqualityComparer<TKey> comparer = null,
            [CanBeNull] IEqualityComparer<TValue> valueComparer = null,
            TriState allowResurrection = default(TriState))
        {
            // Set allow resurrection.
            _allowResurrection = allowResurrection == TriState.Undefined
                                     ? !ObservableWeakReference<TValue>.Disposable
                                     : allowResurrection == TriState.Yes;

            // We are only observing finalization if the type supports it and we're not allowing resurrection.
            _observable = !_allowResurrection && ObservableWeakReference<TValue>.ObservableFinalize;

            _valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;

            // Create underlying dictionary.
            _dictionary = new ConcurrentDictionary<TKey, WeakGrouping>(
                concurrencyLevel < 1 ? 4*Environment.ProcessorCount : concurrencyLevel,
                capacity < 1 ? 32 : capacity,
                comparer ?? EqualityComparer<TKey>.Default);

            if (collection == null) return;

            // ReSharper disable AssignNullToNotNullAttribute
            foreach (KeyValuePair<TKey, TValue> kvp in collection)
            {
                Add(kvp.Key, kvp.Value);
            }
            // ReSharper restore AssignNullToNotNullAttribute
        }

        #region ILookup<TKey,TValue> Members
        /// <summary>
        ///   Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///   A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<IGrouping<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.Values.GetEnumerator();
        }

        /// <summary>
        ///   Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///   A <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///   Determines whether a specified key exists in the lookup.
        /// </summary>
        /// <param name="key">The key to search for in the lookup.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the specified <paramref name="key"/> is in the lookup;
        ///   otherwise returns <see langword="false"/>.
        /// </returns>
        public bool Contains([NotNull] TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        /// <summary>
        ///   Gets the number of key/value collection pairs in the lookup.
        /// </summary>
        public int Count
        {
            get { return _dictionary.Count; }
        }

        /// <summary>
        ///   Retrieves the sequence of values indexed by a specified key.
        /// </summary>
        /// <param name="key">The key of the desired sequence of values.</param>
        /// <value>
        ///   An <see cref="T:System.Collections.Generic.IEnumerable`1">IEnumerable</see>
        ///   containing the sequence of values indexed by the specified<paramref name="key"/>.
        /// </value>
        public IEnumerable<TValue> this[[NotNull] TKey key]
        {
            get
            {
                IGrouping<TKey, TValue> grouping;
                // ReSharper disable AssignNullToNotNullAttribute
                return TryGet(key, out grouping) ? grouping : new WeakGrouping(this, key);
                // ReSharper restore AssignNullToNotNullAttribute
            }
        }
        #endregion

        /// <summary>
        ///   Tries to retrieve the group of values at the specified key.
        /// </summary>
        /// <param name="key">The key of the values to get.</param>
        /// <param name="value">The group of values to retrieve.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the value is retrieved; otherwise returns <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="key"/> is <see langword="null">null</see>.
        /// </exception>
        [UsedImplicitly]
        public bool TryGet([NotNull] TKey key, out IGrouping<TKey, TValue> value)
        {
            WeakGrouping weakGrouping;
            if (_dictionary.TryGetValue(key, out weakGrouping))
            {
                value = weakGrouping;
                return true;
            }
            value = null;
            return false;
        }

        /// <summary>
        ///   Adds the specified kvp to the lookup.
        ///   If the key already exists then the value is updated instead.
        /// </summary>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The corresponding values to add.</param>
        /// <returns>The new/updated values at the specified <paramref name="key"/>.</returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="key"/> is a <see langword="null"/>.
        /// </exception>
        [NotNull]
        [UsedImplicitly]
        public IEnumerable<TValue> Add([NotNull] TKey key, [NotNull] TValue value)
        {
            // ReSharper disable PossibleNullReferenceException
            return _dictionary.AddOrUpdate(key, k => new WeakGrouping(this, key, value), (k, g) => g.Add(value)) ??
                   Enumerable.Empty<TValue>();
            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>
        ///   Removes the entire group of values at the specified key.
        /// </summary>
        /// <param name="key">The key of the group to remove.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the group was removed successfully; otherwise returns <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="key"/> is a <see langword="null"/>.
        /// </exception>
        [UsedImplicitly]
        public bool Remove([NotNull] TKey key)
        {
            WeakGrouping weakGrouping;
            return _dictionary.TryRemove(key, out weakGrouping);
        }

        /// <summary>
        ///   Removes the specified value from the group.
        /// </summary>
        /// <param name="key">The key of the group to remove the value from.</param>
        /// <param name="value">The value to remove.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the <paramref name="value"/> was removed; otherwise returns <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="key"/> is a <see langword="null"/>.
        /// </exception>
        [UsedImplicitly]
        public bool Remove([NotNull] TKey key, [NotNull] TValue value)
        {
            WeakGrouping weakGrouping;
            return _dictionary.TryGetValue(key, out weakGrouping) && weakGrouping.Remove(value);
        }

        #region Nested type: WeakGrouping
        /// <summary>
        ///   A collection of weakly referenced objects that share a common key.
        /// </summary>
        private class WeakGrouping : IGrouping<TKey, TValue>
        {
            /// <summary>
            ///   The weak references.
            /// </summary>
            [NotNull] private readonly ConcurrentDictionary<Guid, WeakReference<TValue>> _dictionary =
                new ConcurrentDictionary<Guid, WeakReference<TValue>>();

            /// <summary>
            ///   The parent, which is the lookup that the group is contained in.
            /// </summary>
            private readonly WeakConcurrentLookup<TKey, TValue> _parent;

            /// <summary>
            ///   Initializes a new instance of the <see cref="WeakGrouping"/> class.
            /// </summary>
            /// <param name="parent">The lookup that the group is contained in.</param>
            /// <param name="key">The key.</param>
            internal WeakGrouping([NotNull] WeakConcurrentLookup<TKey, TValue> parent, [NotNull] TKey key)
            {
                Key = key;
                _parent = parent;
            }

            /// <summary>
            ///   Initializes a new instance of the <see cref="WeakGrouping"/> class.
            /// </summary>
            /// <param name="parent">The lookup that the group is contained in.</param>
            /// <param name="key">The key.</param>
            /// <param name="value">The values that correspond to <paramref name="key"/>.</param>
            internal WeakGrouping([NotNull] WeakConcurrentLookup<TKey, TValue> parent, [NotNull] TKey key,
                                  [NotNull] TValue value)
            {
                Key = key;
                _parent = parent;
                Add(value);
            }

            #region IGrouping<TKey,TValue> Members
            /// <summary>
            ///   Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>
            ///   A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
            /// </returns>
            /// <filterpriority>1</filterpriority>
            public IEnumerator<TValue> GetEnumerator()
            {
                List<Guid> deadGuids = new List<Guid>();
                foreach (KeyValuePair<Guid, WeakReference<TValue>> kvp in _dictionary)
                {
                    TValue target;
                    if (!kvp.Value.TryGetTarget(out target))
                        deadGuids.Add(kvp.Key);
                    else
                        yield return target;
                }
                foreach (Guid guid in deadGuids)
                {
                    WeakReference<TValue> weakReference;
                    if (!_dictionary.TryRemove(guid, out weakReference) || !_parent._observable) continue;

                    ObservableWeakReference<TValue> owf = weakReference as ObservableWeakReference<TValue>;
                    if (owf != null)
                        owf.Dispose();
                }
            }

            /// <summary>
            ///   Returns an enumerator that iterates through a collection.
            /// </summary>
            /// <returns>
            ///   A <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
            /// </returns>
            /// <filterpriority>2</filterpriority>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            /// <summary>
            /// Gets the key that corresponds to this group.
            /// </summary>
            [NotNull]
            public TKey Key { get; private set; }
            #endregion

            /// <summary>
            ///   Adds the specified value to the group.
            /// </summary>
            /// <param name="value">The value to add.</param>
            /// <returns>The new <paramref name="value"/> added to the group.</returns>
            public WeakGrouping Add([NotNull] TValue value)
            {
                Guid guid = Guid.NewGuid();
                WeakReference<TValue> weakReference;
                if (_parent._observable)
                {
                    ObservableWeakReference<TValue> owf = new ObservableWeakReference<TValue>(value,
                                                                                              _parent._allowResurrection);
                    owf.Finalized += (s, e) =>
                                         {
                                             WeakReference<TValue> removedValue;
                                             _dictionary.TryRemove(guid, out removedValue);
                                             owf.Dispose();
                                         };
                    weakReference = owf;
                }
                else
                {
                    weakReference = new WeakReference<TValue>(value, _parent._allowResurrection);
                }
                _dictionary.AddOrUpdate(guid,
                                        g => weakReference,
                                        (g, w) =>
                                            {
                                                if ((_parent._observable) &&
                                                    (w != null))
                                                {
                                                    ObservableWeakReference<TValue> o =
                                                        w as ObservableWeakReference<TValue>;
                                                    if (o != null)
                                                        o.Dispose();
                                                }
                                                return weakReference;
                                            });
                return this;
            }

            /// <summary>
            ///   Removes the specified value from the group.
            /// </summary>
            /// <param name="value">The value to remove.</param>
            /// <returns>
            ///   Returns <see langword="true"/> if the value was successfully removed; otherwise returns <see langword="false"/>.
            /// </returns>
            [UsedImplicitly]
            public bool Remove(TValue value)
            {
                // Scan dictionary looking for dead elements
                List<Guid> deadGuids = new List<Guid>();
                bool found = false;
                foreach (KeyValuePair<Guid, WeakReference<TValue>> kvp in _dictionary)
                {
                    TValue v;
                    if (!kvp.Value.TryGetTarget(out v))
                        deadGuids.Add(kvp.Key);

                    if (!_parent._valueComparer.Equals(v, value)) continue;
                    deadGuids.Add(kvp.Key);
                    found = true;
                    break;
                }
                foreach (Guid guid in deadGuids)
                {
                    WeakReference<TValue> weakReference;
                    if (!_dictionary.TryRemove(guid, out weakReference) ||
                        (weakReference == null)) continue;
                    ObservableWeakReference<TValue> owf = weakReference as ObservableWeakReference<TValue>;
                    if (owf != null)
                        owf.Dispose();
                }

                // If we're down to one remove grouping.
                if (_dictionary.Count < 1)
                {
                    WeakGrouping weakGrouping;
                    _parent._dictionary.TryRemove(Key, out weakGrouping);
                }
                return found;
            }
        }
        #endregion
    }
}