#region © Copyright Web Applications (UK) Ltd, 2013.  All rights reserved.
// Copyright (c) 2013, Web Applications UK Ltd
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
using System.Runtime;
using JetBrains.Annotations;
using WebApplications.Utilities.Enumerations;

namespace WebApplications.Utilities.Caching
{
    /// <summary>
    ///   Synchronized dictionary keeping only weak references to the items (but not to their keys) it contains.
    ///   If TValue implements <see cref="WebApplications.Utilities.Caching.IObservableFinalize"/> then items are
    ///   removed automatically, otherwise they are only removed when attempting to access (e.g. through iterating
    ///   through the collection, or 'counting', etc.)
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    [UsedImplicitly]
    public class WeakConcurrentDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary
        where TValue : class
    {
        /// <summary>
        ///   A <see cref="bool"/> indicating whether the dictionary supports resurrection.
        /// </summary>
        private readonly bool _allowResurrection;

        /// <summary>
        ///   The underlying weak references.
        /// </summary>
        [NotNull] private readonly ConcurrentDictionary<TKey, WeakReference<TValue>> _dictionary;

        /// <summary>
        ///   A <see cref="bool"/> to indicate whether we are observing finalize.
        /// </summary>
        private readonly bool _observable;

        /// <summary>
        ///   Initializes a new instance of the <see cref="WeakConcurrentDictionary&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="concurrencyLevel">The estimated number of threads that will update the lookup concurrently.</param>
        /// <param name="capacity">The initial number of elements that the lookup can contain.</param>
        /// <param name="comparer">The comparer to use when comparing keys.</param>
        /// <param name="allowResurrection">
        ///   If set to <see langword="true"/> then allow resurrections.
        ///   If unset will allow resurrection if the type does not support dispose.
        /// </param>
        public WeakConcurrentDictionary(
            int concurrencyLevel = 0,
            int capacity = 0,
            [NotNull] IEqualityComparer<TKey> comparer = null,
            TriState allowResurrection = default(TriState))
            : this(null, concurrencyLevel, capacity, comparer, allowResurrection)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="WeakConcurrentDictionary&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="collection">The collection to copy into the dictionary.</param>
        /// <param name="concurrencyLevel">The estimated number of threads that will update the lookup concurrently.</param>
        /// <param name="capacity">The initial number of elements that the lookup can contain.</param>
        /// <param name="comparer">The comparer to use when comparing keys.</param>
        /// <param name="allowResurrection">
        ///   If set to <see langword="true"/> then allow resurrections.
        ///   If unset will allow resurrection if the type does not support dispose.</param>
        public WeakConcurrentDictionary(
            [CanBeNull] IEnumerable<KeyValuePair<TKey, TValue>> collection,
            int concurrencyLevel = 0,
            int capacity = 0,
            [CanBeNull] IEqualityComparer<TKey> comparer = null,
            TriState allowResurrection = default(TriState))
        {
            // Create underlying dictionary.
            _dictionary = new ConcurrentDictionary<TKey, WeakReference<TValue>>(
                concurrencyLevel < 1 ? 4*Environment.ProcessorCount : concurrencyLevel,
                capacity < 1 ? 32 : capacity,
                comparer ?? EqualityComparer<TKey>.Default);

            // Set allow resurrection.
            _allowResurrection = allowResurrection == TriState.Undefined
                                     ? !ObservableWeakReference<TValue>.Disposable
                                     : allowResurrection == TriState.Yes;

            // We are only observing finalization if the type supports it and we're not allowing resurrection.
            _observable = !_allowResurrection && ObservableWeakReference<TValue>.ObservableFinalize;

            if (collection == null) return;

            // ReSharper disable AssignNullToNotNullAttribute
            foreach (KeyValuePair<TKey, TValue> kvp in collection)
                Add(kvp.Key, kvp.Value);
            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        ///   Gets a <see cref="bool"/> value indicating whether this instance is empty.
        /// </summary>
        [UsedImplicitly]
        public bool IsEmpty
        {
            get
            {
                // This will remove dead keys.
                return Count < 1;
            }
        }

        #region IDictionary Members
        /// <inheritdoc />
        void IDictionary.Add(object key, object value)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (!(key is TKey))
                throw new ArgumentException(Resources.WeakConcurrentDictionary_Add_KeyTypeIncorrect);
            TValue obj;
            try
            {
                obj = (TValue) value;
            }
            catch (InvalidCastException ex)
            {
                throw new ArgumentException(Resources.WeakConcurrentDictionary_Add_ValueTypeIncorrect, ex);
            }
            Add((TKey) key, obj);
        }

        /// <inheritdoc />
        bool IDictionary.Contains(object key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (key is TKey)
                return ContainsKey((TKey) key);
            return false;
        }

        /// <inheritdoc />
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new DictionaryEnumerator<TKey, TValue>(this);
        }

        /// <inheritdoc />
        void IDictionary.Remove(object key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (!(key is TKey))
                return;
            TValue obj;
            TryRemove((TKey) key, out obj);
        }

        /// <inheritdoc />
        object IDictionary.this[object key]
        {
            get { return this[(TKey) key]; }
            set { this[(TKey) key] = (TValue) value; }
        }

        /// <inheritdoc />
        void ICollection.CopyTo(Array array, int index)
        {
            KeyValuePair<TKey, TValue>[] array1 = array as KeyValuePair<TKey, TValue>[];
            if (array1 != null)
            {
                CopyToPairs(array1, index);
            }
            else
            {
                DictionaryEntry[] array2 = array as DictionaryEntry[];
                if (array2 != null)
                {
                    CopyToEntries(array2, index);
                }
                else
                {
                    object[] array3 = array as object[];
                    if (array3 == null)
                        throw new ArgumentException(Resources.WeakConcurrentDictionary_CopyTo_InvalidArrayType, "array");
                    CopyToObjects(array3, index);
                }
            }
        }

        /// <inheritdoc />
        bool IDictionary.IsFixedSize
        {
            get { return false; }
        }

        /// <inheritdoc />
        bool IDictionary.IsReadOnly
        {
            get { return false; }
        }

        /// <inheritdoc />
        ICollection IDictionary.Keys
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return (ICollection) Keys; }
        }

        /// <inheritdoc />
        ICollection IDictionary.Values
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return (ICollection) Values; }
        }

        /// <inheritdoc />
        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        /// <inheritdoc />
        object ICollection.SyncRoot
        {
            get { throw new NotSupportedException(); }
        }
        #endregion

        #region IDictionary<TKey,TValue> Members
        /// <inheritdoc />
        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        /// <inheritdoc />
        public bool TryGetValue(TKey key, out TValue value)
        {
            WeakReference<TValue> weakReference;
            bool found = _dictionary.TryGetValue(key, out weakReference);
            if (found)
            {
                if (weakReference == null)
                    value = default(TValue);
                else if (!weakReference.TryGetTarget(out value))
                {
                    _dictionary.TryRemove(key, out weakReference);
                    return false;
                }
                return true;
            }

            value = default(TValue);
            return false;
        }

        /// <inheritdoc />
        public void Clear()
        {
            _dictionary.Clear();
        }

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            List<TKey> deadKeys = new List<TKey>(_dictionary.Count);
            // Only yield alive values.
            foreach (KeyValuePair<TKey, WeakReference<TValue>> kvp in _dictionary)
            {
                TValue value;
                if (kvp.Value == null)
                    yield return new KeyValuePair<TKey, TValue>(kvp.Key, default(TValue));
                else if (kvp.Value.TryGetTarget(out value))
                    yield return new KeyValuePair<TKey, TValue>(kvp.Key, value);
                else
                    deadKeys.Add(kvp.Key);
            }

            // Clean up any dead keys found during enumeration
            foreach (TKey key in deadKeys)
            {
                WeakReference<TValue> weakReference;
                // ReSharper disable AssignNullToNotNullAttribute
                _dictionary.TryRemove(key, out weakReference);
                // ReSharper restore AssignNullToNotNullAttribute
            }
        }

        /// <inheritdoc />
        public TValue this[TKey key]
        {
            get
            {
                TValue value;
                if (!TryGetValue(key, out value))
                    throw new KeyNotFoundException();
                return value;
            }
            set { AddOrUpdate(key, value, (k, v) => value); }
        }

        /// <inheritdoc />
        public int Count
        {
            get
            {
                // This will remove dead keys whilst counting.
                int count = 0;
                using (IEnumerator<KeyValuePair<TKey, TValue>> e = GetEnumerator())
                {
                    checked
                    {
                        while (e.MoveNext()) count++;
                    }
                }
                return count;
            }
        }

        /// <inheritdoc />
        public ICollection<TKey> Keys
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return (ICollection<TKey>) this.Select(kvp => kvp.Key); }
        }


        /// <inheritdoc />
        public ICollection<TValue> Values
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return (ICollection<TValue>) this.Select(kvp => kvp.Value); }
        }

        /// <inheritdoc />
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            CopyToPairs(array, index);
        }

        /// <inheritdoc />
        public void Add(TKey key, TValue value)
        {
            if (!TryAdd(key, value))
                throw new ArgumentException(Resources.WeakConcurrentDictionary_Add_KeyAlreadyExists);
        }

        /// <inheritdoc />
        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            TValue obj;
            return TryRemove(key, out obj);
        }

        /// <inheritdoc />
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair)
        {
            // ReSharper disable RedundantCast
            if ((object) keyValuePair.Key == null)
                throw new ArgumentNullException("keyValuePair", Resources.WeakConcurrentDictionary_KeyIsNull);
            // ReSharper restore RedundantCast
            Add(keyValuePair.Key, keyValuePair.Value);
        }

        /// <inheritdoc />
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
        {
            // ReSharper disable RedundantCast
            if ((object) keyValuePair.Key == null)
                throw new ArgumentNullException("keyValuePair", Resources.WeakConcurrentDictionary_KeyIsNull);
            // ReSharper restore RedundantCast
            TValue x;
            return TryGetValue(keyValuePair.Key, out x) &&
                   EqualityComparer<TValue>.Default.Equals(x, keyValuePair.Value);
        }

        /// <inheritdoc />
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
        {
            // ReSharper disable RedundantCast
            if ((object) keyValuePair.Key == null)
                throw new ArgumentNullException("keyValuePair", Resources.WeakConcurrentDictionary_KeyIsNull);
            // ReSharper restore RedundantCast

            TValue obj;
            return TryRemove(keyValuePair.Key, out obj) &&
                   EqualityComparer<TValue>.Default.Equals(obj, keyValuePair.Value);
        }

        /// <inheritdoc />
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return false; }
        }
        #endregion

        /// <summary>
        ///   Register the finalize event where appropriate.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="weakReference">The weak reference.</param>
        private void RegisterFinalize([NotNull] TKey key, [CanBeNull] WeakReference<TValue> weakReference)
        {
            if (!_observable || (weakReference == null)) return;
            ObservableWeakReference<TValue> owf = weakReference as ObservableWeakReference<TValue>;
            if (owf == null) return;
            owf.Finalized += (s, e) =>
                {
                    WeakReference<TValue> oldwr;

                    if (_dictionary.TryRemove(key, out oldwr) &&
                        (oldwr != weakReference))
                    {
                        // If we removed something else, add it back!
                        _dictionary.TryAdd(key, oldwr);
                    }
                    owf.Dispose();
                };
        }

        /// <summary>
        ///   Unregisters the finalize event where appropriate.
        /// </summary>
        /// <param name="weakReference">The weak reference.</param>.
        private void UnregisterFinalize([CanBeNull] WeakReference<TValue> weakReference)
        {
            if (!_observable || (weakReference == null)) return;
            ObservableWeakReference<TValue> owf = weakReference as ObservableWeakReference<TValue>;
            if (owf == null) return;
            owf.Dispose();
        }

        /// <summary>
        ///   Wraps the specified value in a weak reference.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="register">If set to <see langword="true"/> registers the finalize event immediately.</param>
        /// <returns>The wrapped value.</returns>
        private WeakReference<TValue> Wrap([NotNull] TKey key, [CanBeNull] TValue value, bool register = true)
        {
            if (value == null)
                return null;

            if (!_observable)
                return new WeakReference<TValue>(value, _allowResurrection);
            ObservableWeakReference<TValue> owf = new ObservableWeakReference<TValue>(value, _allowResurrection);
            if (register)
                RegisterFinalize(key, owf);
            return owf;
        }

        /// <summary>
        /// Tries to add an entry of the specified key and value.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the kvp was added successfully; otherwise returns <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="key"/> is a <see langword="null"/>.
        /// </exception>
        [UsedImplicitly]
        public bool TryAdd([NotNull] TKey key, TValue value)
        {
            return _dictionary.TryAdd(key, Wrap(key, value));
        }

        /// <summary>
        ///   Tries to remove the entry at the specified key.
        /// </summary>
        /// <param name="key">The key of the entry to remove.</param>
        /// <param name="value">The value of the removed object.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the entry was removed successfully; otherwise returns <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="key"/> is a <see langword="null"/>.
        /// </exception>
        [UsedImplicitly]
        public bool TryRemove([NotNull] TKey key, out TValue value)
        {
            WeakReference<TValue> weakReference;
            if (_dictionary.TryRemove(key, out weakReference))
            {
                bool found = weakReference.TryGetTarget(out value);
                UnregisterFinalize(weakReference);
                return found;
            }
            value = default(TValue);
            return false;
        }

        /// <summary>
        ///   Tries to update the entry.
        /// </summary>
        /// <param name="key">The key of the entry to update.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="comparisonValue">The comparison value.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the value was updated; otherwise returns <see langword="false"/>.
        /// </returns>
        [UsedImplicitly]
        public bool TryUpdate([NotNull] TKey key, TValue newValue, TValue comparisonValue)
        {
            WeakReference<TValue> newWeakReference = Wrap(key, newValue, false);
            if (_dictionary.TryUpdate(key, newWeakReference, Wrap(key, comparisonValue, false)))
            {
                // If we updated, subscribed to finalize event.
                RegisterFinalize(key, newWeakReference);
                return true;
            }
            return false;
        }

        /// <summary>
        ///   Copies the dictionary entries to an <see cref="Array"/>.
        /// </summary>
        [UsedImplicitly]
        public KeyValuePair<TKey, TValue>[] ToArray()
        {
            return ((IEnumerable<KeyValuePair<TKey, TValue>>) this).ToArray();
        }

        /// <summary>
        ///   Gets or adds the entry using the key and the function specified.
        /// </summary>
        /// <param name="key">The key of the entry to get/add.</param>
        /// <param name="valueFactory">Used to create the value we want to cache.</param>
        /// <returns>
        ///   Either the inserted or retrieved value depending on whether there's already an existing entry with the same key.
        /// </returns>
        [UsedImplicitly]
        public TValue GetOrAdd([NotNull] TKey key, [NotNull] Func<TKey, TValue> valueFactory)
        {
            WeakReference<TValue> weakReference = _dictionary.GetOrAdd(key, k => Wrap(key, valueFactory(k)));
            if (weakReference == null)
                return default(TValue);

            TValue value;
            // It is possible we have got a GC'd object, in which case call add or update.
            return !weakReference.TryGetTarget(out value)
                       ? AddOrUpdate(key, valueFactory, (k, v) => valueFactory(k))
                       : value;
        }

        /// <summary>
        ///   Gets or adds the entry using the key and value specified.
        /// </summary>
        /// <param name="key">The key of the entry to get/add.</param>
        /// <param name="value">The value to add.</param>
        /// <returns>
        ///   Either the inserted or retrieved value depending on whether there's already an existing entry with the same key.
        /// </returns>
        [UsedImplicitly]
        public TValue GetOrAdd([NotNull] TKey key, TValue value)
        {
            return GetOrAdd(key, k => value);
        }

        /// <summary>
        ///   Adds or updates the entry using the specified key, value and a function for updating the value if the entry exists.
        /// </summary>
        /// <param name="key">The key of the entry to add/update.</param>
        /// <param name="addValueFactory">Used to create the value that we want to cache.</param>
        /// <param name="updateValueFactory">Used to update the value if the entry already exists.</param>
        /// <returns>
        ///   Either the inserted or updated value depending on whether there's already an existing entry with the same key.
        /// </returns>
        [UsedImplicitly]
        public TValue AddOrUpdate(
            [NotNull] TKey key, [NotNull] Func<TKey, TValue> addValueFactory,
            [NotNull] Func<TKey, TValue, TValue> updateValueFactory)
        {
            WeakReference<TValue> weakReference = _dictionary
                .AddOrUpdate(
                    key,
                    k => Wrap(key, addValueFactory(k)),
                    (k, v) =>
                        {
                            TValue value;
                            if (v == null)
                                return Wrap(key, updateValueFactory(k, default(TValue)));

                            if (v.TryGetTarget(out value))
                            {
                                v.Target = updateValueFactory(key, value);
                                return v;
                            }

                            UnregisterFinalize(v);

                            return Wrap(key, addValueFactory(k));
                        });

            // Now retrieve the new value
            TValue newValue;
            weakReference.TryGetTarget(out newValue);
            return newValue;
        }

        /// <summary>
        ///   Adds or updates the entry using the specified key, value and a function for updating the value if the entry exists.
        /// </summary>
        /// <param name="key">The key of the entry to add/update.</param>
        /// <param name="addValue">The value to add.</param>
        /// <param name="updateValueFactory">Used to update the value if the entry already exists.</param>
        /// <returns>
        ///   Either the inserted or updated value depending on whether there's already an existing entry with the same key.
        /// </returns>
        [UsedImplicitly]
        public TValue AddOrUpdate([NotNull] TKey key, TValue addValue,
                                  [NotNull] Func<TKey, TValue, TValue> updateValueFactory)
        {
            return AddOrUpdate(key, k => addValue, updateValueFactory);
        }

        /// <summary>
        ///   Copies the entries to an array of <see cref="System.Object">objects</see>.
        /// </summary>
        /// <param name="array">The destination array.</param>
        /// <param name="index">The index at which copying begins.</param>
        private void CopyToObjects([NotNull] object[] array, int index)
        {
            foreach (KeyValuePair<TKey, TValue> kvp in this)
            {
                array[index++] = kvp;
            }
        }

        /// <summary>
        ///   Copies the entries to an array of <see cref="System.Collections.DictionaryEntry">dictionary entries</see>.
        /// </summary>
        /// <param name="array">The destination array.</param>
        /// <param name="index">The index at which copying begins.</param>
        private void CopyToEntries([NotNull] DictionaryEntry[] array, int index)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            foreach (KeyValuePair<TKey, TValue> kvp in this)
                array[index++] = new DictionaryEntry(kvp.Key, kvp.Value);
            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        ///   Copies the entries to an array of <see cref="System.Collections.Generic.KeyValuePair&lt;TKey, TValue&gt;">key value pairs</see>.
        /// </summary>
        /// <param name="array">The destination array.</param>
        /// <param name="index">The index at which copying begins.</param>
        private void CopyToPairs([NotNull] KeyValuePair<TKey, TValue>[] array, int index)
        {
            foreach (KeyValuePair<TKey, TValue> kvp in this)
                array[index++] = kvp;
        }
    }
}