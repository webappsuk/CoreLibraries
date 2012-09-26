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
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Caching
{
    /// <summary>
    ///   Implements a concurrent lookup, which allows a set of objects to be grouped and manipulated by a key.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    [UsedImplicitly]
    public class ConcurrentLookup<TKey, TValue> : ILookup<TKey, TValue>
    {
        /// <summary>
        ///   A concurrent dictionary that holds the underlying data.
        /// </summary>
        private readonly ConcurrentDictionary<TKey, Grouping> _dictionary;

        /// <summary>
        ///   The value comparer, used to check for equality.
        /// </summary>
        private readonly IEqualityComparer<TValue> _valueComparer;

        /// <summary>
        ///   Initializes a new instance of the <see cref="ConcurrentLookup&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="concurrencyLevel">
        ///   <para>The concurrency level.</para>
        ///   <para>This is the estimated number of threads that will update the lookup concurrently.</para>
        ///   <para>By default this is 4 * the processor count.</para>
        /// </param>
        /// <param name="capacity">
        ///   <para>The initial number of elements that the lookup can contain.</para>
        ///   <para>By default this is 32.</para>
        /// </param>
        /// <param name="comparer">The comparer to use when comparing keys.</param>
        /// <param name="valueComparer">The value comparer, used to check for equality.</param>
        public ConcurrentLookup(
            int concurrencyLevel = 0,
            int capacity = 0,
            [CanBeNull] IEqualityComparer<TKey> comparer = null,
            [CanBeNull] IEqualityComparer<TValue> valueComparer = null)
            : this(null, concurrencyLevel, capacity, comparer)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ConcurrentLookup&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="collection">The collection to copy into the lookup.</param>
        /// <param name="concurrencyLevel">
        ///   <para>The concurrency level.</para>
        ///   <para>This is the estimated number of threads that will update the lookup concurrently.</para>
        ///   <para>By default this is 4 * the processor count.</para>
        /// </param>
        /// <param name="capacity">
        ///   <para>The initial number of elements that the lookup can contain.</para>
        ///   <para>By default this is 32.</para>
        /// </param>
        /// <param name="comparer">The comparer to use when comparing keys.</param>
        /// <param name="valueComparer">The value comparer, used to check for equality.</param>
        public ConcurrentLookup(
            [CanBeNull] IEnumerable<KeyValuePair<TKey, TValue>> collection,
            int concurrencyLevel = 0,
            int capacity = 0,
            [CanBeNull] IEqualityComparer<TKey> comparer = null,
            [CanBeNull] IEqualityComparer<TValue> valueComparer = null)
        {
            _valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;

            // Create underlying dictionary.
            _dictionary = new ConcurrentDictionary<TKey, Grouping>(
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
        ///   Returns <see langword="true"/> if the specified <paramref name="key"/> is in the lookup; otherwise returns <see langword="false"/>.
        /// </returns>
        public bool Contains([NotNull] TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        /// <summary>
        /// Gets the number of key/value collection pairs in the lookup.
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
        ///   An <see cref="T:System.Collections.Generic.IEnumerable`1">IEnumerable</see> containing the sequence of values indexed by
        ///   the specified<paramref name="key"/>.
        /// </value>
        public IEnumerable<TValue> this[[NotNull] TKey key]
        {
            get
            {
                IGrouping<TKey, TValue> grouping;
                // ReSharper disable AssignNullToNotNullAttribute
                return TryGet(key, out grouping) ? grouping : new Grouping(this, key);
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
        ///   <paramref name="key"/> is a <see langword="null"/>.
        /// </exception>
        [UsedImplicitly]
        public bool TryGet([NotNull] TKey key, out IGrouping<TKey, TValue> value)
        {
            Grouping grouping;
            if (_dictionary.TryGetValue(key, out grouping))
            {
                value = grouping;
                return true;
            }
            value = null;
            return false;
        }

        /// <summary>
        ///   Adds the specified kvp to the lookup. If the key already exists then the value is updated instead.
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
            return _dictionary.AddOrUpdate(key, k => new Grouping(this, key, value), (k, g) => g.Add(value)) ??
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
            Grouping grouping;
            return _dictionary.TryRemove(key, out grouping);
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
            Grouping grouping;
            return _dictionary.TryGetValue(key, out grouping) && grouping.Remove(value);
        }

        #region Nested type: Grouping
        /// <summary>
        ///   A collection of objects that share a common key.
        /// </summary>
        private class Grouping : IGrouping<TKey, TValue>
        {
            /// <summary>
            ///   The references.
            /// </summary>
            [NotNull] private readonly ConcurrentDictionary<Guid, TValue> _dictionary =
                new ConcurrentDictionary<Guid, TValue>();

            /// <summary>
            ///   The parent, which is the lookup that the group is contained in.
            /// </summary>
            private readonly ConcurrentLookup<TKey, TValue> _parent;

            /// <summary>
            ///   Initializes a new instance of the <see cref="Grouping"/> class.
            /// </summary>
            /// <param name="parent">The lookup that the group is contained in.</param>
            /// <param name="key">The key.</param>
            internal Grouping([NotNull] ConcurrentLookup<TKey, TValue> parent, [NotNull] TKey key)
            {
                Key = key;
                _parent = parent;
            }

            /// <summary>
            ///   Initializes a new instance of the <see cref="Grouping"/> class.
            /// </summary>
            /// <param name="parent">The lookup that the group is contained in.</param>
            /// <param name="key">The key.</param>
            /// <param name="value">The values that correspond to <paramref name="key"/>.</param>
            internal Grouping([NotNull] ConcurrentLookup<TKey, TValue> parent, [NotNull] TKey key,
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
            ///   Gets the key that corresponds to this group.
            /// </summary>
            [NotNull]
            public TKey Key { get; private set; }
            #endregion

            /// <summary>
            ///   Adds the specified value to the group.
            /// </summary>
            /// <param name="value">The value to add.</param>
            /// <returns>The new <paramref name="value"/> added to the group.</returns>
            public Grouping Add([NotNull] TValue value)
            {
                Guid guid = Guid.NewGuid();
                _dictionary.AddOrUpdate(guid,
                                        g => value,
                                        (g, w) => value);
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
                foreach (KeyValuePair<Guid, TValue> kvp in _dictionary)
                {
                    if (!_parent._valueComparer.Equals(kvp.Value, value)) continue;

                    _dictionary.TryRemove(kvp.Key, out value);
                    if (_dictionary.Count < 1)
                    {
                        Grouping grouping;
                        _parent._dictionary.TryRemove(Key, out grouping);
                    }
                    return true;
                }
                return false;
            }
        }
        #endregion
    }
}