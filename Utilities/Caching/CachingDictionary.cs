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
using System.Linq;
using System.Runtime;
using System.Runtime.Caching;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Caching
{
    internal static class CachingDictionary
    {
        /// <summary>
        ///   The maximum sliding expiration
        /// </summary>
        internal static readonly TimeSpan MaxSlidingExpiration = TimeSpan.FromDays(365);
    }

    /// <summary>
    ///   A thread safe dictionary that caches and keeps track of elements in the cache.
    ///   This allows for greater functionality than the <see cref="EnhancedMemoryCache{TKey, TValue}" />,
    ///   including, key searches, non-string keys, etc.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    [PublicAPI]
    public class CachingDictionary<TKey, TValue> : CachingDictionaryBase<TKey, TValue>, IDictionary<TKey, TValue>,
        IDictionary
    {
        /// <summary>
        ///   The cache.
        /// </summary>
        [NotNull]
        private readonly MemoryCache _cache;

        /// <summary>
        ///   The keys cache.
        /// </summary>
        [NotNull]
        private readonly ConcurrentDictionary<TKey, string> _keys;

        #region Constructors
        /// <summary>
        ///   Initializes a new instance of the <see cref="CachingDictionary&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="cacheName">The name of the cache.</param>
        public CachingDictionary(string cacheName = null)
        {
            _keys = new ConcurrentDictionary<TKey, string>();
            // ReSharper disable once AssignNullToNotNullAttribute
            _cache = string.IsNullOrWhiteSpace(cacheName) ? MemoryCache.Default : new MemoryCache(cacheName);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="CachingDictionary&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="concurrencyLevel">The estimated number of threads that will update the lookup concurrently.</param>
        /// <param name="capacity">The initial number of elements that the dictionary can contain.</param>
        /// <param name="cacheName">The name of the cache.</param>
        public CachingDictionary(int concurrencyLevel, int capacity, string cacheName = null)
        {
            _keys = new ConcurrentDictionary<TKey, string>(concurrencyLevel, capacity);
            // ReSharper disable once AssignNullToNotNullAttribute
            _cache = string.IsNullOrWhiteSpace(cacheName) ? MemoryCache.Default : new MemoryCache(cacheName);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="CachingDictionary&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="comparer">The comparer to use when comparing keys.</param>
        /// <param name="cacheName">The name of the cache.</param>
        public CachingDictionary([NotNull] IEqualityComparer<TKey> comparer, string cacheName = null)
        {
            if (comparer == null) throw new ArgumentNullException("comparer");

            _keys = new ConcurrentDictionary<TKey, string>(comparer);
            // ReSharper disable once AssignNullToNotNullAttribute
            _cache = string.IsNullOrWhiteSpace(cacheName) ? MemoryCache.Default : new MemoryCache(cacheName);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="CachingDictionary&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="collection">The collection to copy into the dictionary.</param>
        /// <param name="cacheName">The name of the cache.</param>
        public CachingDictionary([NotNull] IEnumerable<KeyValuePair<TKey, TValue>> collection, string cacheName = null)
        {
            if (collection == null) throw new ArgumentNullException("collection");

            _keys = new ConcurrentDictionary<TKey, string>();
            foreach (KeyValuePair<TKey, TValue> kvp in collection)
                // ReSharper disable once AssignNullToNotNullAttribute - Let AddOrUpdate throw
                AddOrUpdate(kvp.Key, kvp.Value);
            // ReSharper disable once AssignNullToNotNullAttribute
            _cache = string.IsNullOrWhiteSpace(cacheName) ? MemoryCache.Default : new MemoryCache(cacheName);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="CachingDictionary&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="collection">The collection to copy into the dictionary.</param>
        /// <param name="comparer">The comparer to use when comparing keys.</param>
        /// <param name="cacheName">The name of the cache.</param>
        public CachingDictionary(
            [NotNull] IEnumerable<KeyValuePair<TKey, TValue>> collection,
            [NotNull] IEqualityComparer<TKey> comparer,
            string cacheName = null)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            if (comparer == null) throw new ArgumentNullException("comparer");

            _keys = new ConcurrentDictionary<TKey, string>(comparer);
            foreach (KeyValuePair<TKey, TValue> kvp in collection)
                // ReSharper disable once AssignNullToNotNullAttribute - Let AddOrUpdate throw
                AddOrUpdate(kvp.Key, kvp.Value);
            // ReSharper disable once AssignNullToNotNullAttribute
            _cache = string.IsNullOrWhiteSpace(cacheName) ? MemoryCache.Default : new MemoryCache(cacheName);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="CachingDictionary&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="concurrencyLevel">The estimated number of threads that will update the lookup concurrently.</param>
        /// <param name="capacity">The initial number of elements that the dictionary can contain.</param>
        /// <param name="comparer">The comparer to use when comparing keys.</param>
        /// <param name="cacheName">The name of the cache.</param>
        public CachingDictionary(
            int concurrencyLevel,
            int capacity,
            [NotNull] IEqualityComparer<TKey> comparer,
            string cacheName = null)
        {
            if (comparer == null) throw new ArgumentNullException("comparer");

            _keys = new ConcurrentDictionary<TKey, string>(concurrencyLevel, capacity, comparer);
            // ReSharper disable once AssignNullToNotNullAttribute
            _cache = string.IsNullOrWhiteSpace(cacheName) ? MemoryCache.Default : new MemoryCache(cacheName);
        }
        #endregion

        #region IDictionary Members
        /// <inheritdoc />
        void IDictionary.Add(object key, object value)
        {
            AddOrUpdate((TKey)key, (TValue)value);
        }

        /// <inheritdoc />
        bool IDictionary.Contains(object key)
        {
            return ContainsKey((TKey)key);
        }

        /// <inheritdoc />
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new DictionaryEnumerator<TKey, TValue>(this);
        }

        /// <inheritdoc />
        void IDictionary.Remove(object key)
        {
            TValue value;
            if (!TryRemove((TKey)key, out value))
                throw new KeyNotFoundException();
        }

        /// <inheritdoc />
        void ICollection.CopyTo(Array array, int index)
        {
            this.ToArray().CopyTo(array, index);
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
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return (ICollection)Keys; }
        }

        /// <inheritdoc />
        ICollection IDictionary.Values
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return (ICollection)Values; }
        }

        /// <inheritdoc />
        object IDictionary.this[object key]
        {
            get { return this[(TKey)key]; }
            set { this[(TKey)key] = (TValue)value; }
        }

        /// <inheritdoc />
        bool ICollection.IsSynchronized
        {
            get { return true; }
        }

        /// <inheritdoc />
        object ICollection.SyncRoot
        {
            get { throw new NotSupportedException(); }
        }
        #endregion

        #region IDictionary<TKey,TValue> Members
        /// <inheritdoc />
        public override bool ContainsKey([NotNull] TKey key)
        {
            if (key == null) throw new ArgumentNullException("key");

            return _keys.ContainsKey(key);
        }

        /// <inheritdoc />
        public override bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null) throw new ArgumentNullException("key");

            string guid;
            if (!_keys.TryGetValue(key, out guid))
            {
                value = default(TValue);
                return false;
            }

            // ReSharper disable AssignNullToNotNullAttribute
            object result = _cache.Get(guid);
            // ReSharper restore AssignNullToNotNullAttribute
            if (result == null)
            {
                value = default(TValue);
                return false;
            }
            value = ((Wrapper)result).Value;
            return true;
        }

        /// <inheritdoc />
        public override void Clear()
        {
            // Removing the value from the cache will cause the call back to remove keys.
            // ReSharper disable AssignNullToNotNullAttribute
            foreach (string guid in _keys.Values)
                _cache.Remove(guid);
            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            List<TKey> deadKeys = new List<TKey>();
            // Only yield values that are found.
            foreach (KeyValuePair<TKey, string> kvp in _keys)
            {
                Debug.Assert(kvp.Value != null);

                object result = _cache.Get(kvp.Value);
                // If we don't find the key, then add this to the dead keys list.
                if (result == null)
                {
                    deadKeys.Add(kvp.Key);
                    continue;
                }

                yield return new KeyValuePair<TKey, TValue>(kvp.Key, ((Wrapper)result).Value);
            }

            // Clean up any dead keys found during enumeration
            foreach (TKey key in deadKeys)
            {
                string guid;
                Debug.Assert(key != null);
                _keys.TryRemove(key, out guid);
            }
        }

        /// <inheritdoc />
        public TValue this[[NotNull] TKey key]
        {
            get
            {
                if (key == null) throw new ArgumentNullException("key");

                string guid;
                if (!_keys.TryGetValue(key, out guid))
                    throw new KeyNotFoundException();

                Debug.Assert(guid != null);

                object result = _cache.Get(guid);
                if (result == null)
                    throw new KeyNotFoundException();
                return ((Wrapper)result).Value;
            }
            set { AddOrUpdate(key, value); }
        }

        /// <inheritdoc />
        public int Count
        {
            get { return _keys.Count; }
        }

        /// <inheritdoc />
        public ICollection<TKey> Keys
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return this.Select(kvp => kvp.Key).ToList(); }
        }

        /// <inheritdoc />
        public ICollection<TValue> Values
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return this.Select(kvp => kvp.Value).ToList(); }
        }

        /// <inheritdoc />
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            this.ToArray().CopyTo(array, index);
        }

        /// <inheritdoc />
        void IDictionary<TKey, TValue>.Add([NotNull] TKey key, TValue value)
        {
            if (key == null) throw new ArgumentNullException("key");
            AddOrUpdate(key, value);
        }

        /// <inheritdoc />
        bool IDictionary<TKey, TValue>.Remove([NotNull] TKey key)
        {
            if (key == null) throw new ArgumentNullException("key");
            TValue value;
            return TryRemove(key, out value);
        }

        /// <inheritdoc />
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair)
        {
            // ReSharper disable once AssignNullToNotNullAttribute - Let AddOrUpdate throw
            AddOrUpdate(keyValuePair.Key, keyValuePair.Value);
        }

        /// <inheritdoc />
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
        {
            TValue value;
            return keyValuePair.Key != null &&
                   TryGetValue(keyValuePair.Key, out value) &&
                   Equals(value, keyValuePair.Value);
        }

        /// <inheritdoc />
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
        {
            TValue value;
            if (keyValuePair.Key != null &&
                TryRemove(keyValuePair.Key, out value))
            {
                if (Equals(value, keyValuePair.Value))
                    return true;

                // Restore value removed - potential race condition here, but liveable
                AddOrUpdate(keyValuePair.Key, value);
            }
            return false;
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

        /// <inheritdoc />
        public override bool TryAdd(
            [NotNull] TKey key,
            TValue value,
            DateTimeOffset absoluteExpiration,
            TimeSpan slidingExpiration)
        {
            if (key == null) throw new ArgumentNullException("key");

            string guid = Guid.NewGuid().ToString();
            if (!_keys.TryAdd(key, guid))
                return false;

            if (slidingExpiration > CachingDictionary.MaxSlidingExpiration)
                slidingExpiration = ObjectCache.NoSlidingExpiration;
            return _cache.Add(
                guid,
                new Wrapper(value),
                new CacheItemPolicy
                {
                    AbsoluteExpiration = absoluteExpiration,
                    SlidingExpiration = slidingExpiration,
                    RemovedCallback = a => _keys.TryRemove(key, out guid)
                });
        }

        /// <inheritdoc />
        public override bool TryRemove(TKey key, out TValue value)
        {
            if (key == null) throw new ArgumentNullException("key");

            string guid;
            if (_keys.TryRemove(key, out guid))
            {
                Debug.Assert(guid != null);
                object result = _cache.Remove(guid);
                if (result == null)
                {
                    value = default(TValue);
                    return false;
                }
                value = ((Wrapper)result).Value;
                return true;
            }
            value = default(TValue);
            return false;
        }

        /// <inheritdoc />
        public override TValue GetOrAdd(
            TKey key,
            TValue value,
            DateTimeOffset absoluteExpiration,
            TimeSpan slidingExpiration)
        {
            if (key == null) throw new ArgumentNullException("key");

            // Get or add a key
            string guid = _keys.GetOrAdd(key, k => Guid.NewGuid().ToString());
            Debug.Assert(guid != null);

            if (slidingExpiration > CachingDictionary.MaxSlidingExpiration)
                slidingExpiration = ObjectCache.NoSlidingExpiration;
            object result = _cache.AddOrGetExisting(
                guid,
                new Wrapper(value),
                new CacheItemPolicy
                {
                    AbsoluteExpiration = absoluteExpiration,
                    SlidingExpiration = slidingExpiration,
                    RemovedCallback = a => _keys.TryRemove(key, out guid)
                });
            return result == null ? value : ((Wrapper)result).Value;
        }

        /// <inheritdoc />
        public override TValue AddOrUpdate(
            TKey key,
            TValue value,
            DateTimeOffset absoluteExpiration,
            TimeSpan slidingExpiration)
        {
            if (key == null) throw new ArgumentNullException("key");

            // Get or add a key
            string guid = _keys.GetOrAdd(key, k => Guid.NewGuid().ToString());
            Debug.Assert(guid != null);

            if (slidingExpiration > CachingDictionary.MaxSlidingExpiration)
                slidingExpiration = ObjectCache.NoSlidingExpiration;
            _cache.Set(
                guid,
                new Wrapper(value),
                new CacheItemPolicy
                {
                    AbsoluteExpiration = absoluteExpiration,
                    SlidingExpiration = slidingExpiration,
                    RemovedCallback = a => _keys.TryRemove(key, out guid)
                });
            return value;
        }

        #region Nested type: Wrapper
        /// <summary>
        ///   A wrapper struct used to prevent <see langword="null"/> being placed in cache.
        /// </summary>
        private struct Wrapper
        {
            public readonly TValue Value;

            public Wrapper(TValue value)
            {
                Value = value;
            }
        }
        #endregion
    }
}