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
using System.Runtime.Caching;

namespace WebApplications.Utilities.Caching
{
    /// <summary>
    ///   OBSOLETE - Use <see cref="EnhancedMemoryCacheNull&lt;TKey, Tvalue&gt;"/>
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    [Obsolete("Use the newer EnhancedMemoryCahce<TKey, TValue>")]
    public class EnhancedMemoryCacheNull<TValue> : EnhancedMemoryCacheNull<string, TValue>
    {
    }

    /// <summary>
    ///   An Enhanced Memory Cache implementation, uses the <see cref="MemoryCache.Default">default Memory Cache</see>
    ///   but allows  multiple instances to be created (they all map share the underlying memory cache). This ensures
    ///   type-safety on the cache values and allows the stored value to be a <see langword="null"/>. There is a small
    ///   performance penalty for this support so the <see cref="EnhancedMemoryCache&lt;TKey, TValue&gt;"/> should be
    ///   used in preference to this class in the case where nulls (<see langword="null"/>) are not possible.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class EnhancedMemoryCacheNull<TKey, TValue> : CachingDictionaryBase<TKey, TValue>
    {
        private static readonly TimeSpan _maxSlidingExpiration = TimeSpan.FromDays(365);

        /// <summary>
        ///   We implement the enhanced memory cache using the default memory cache.
        /// </summary>
        private readonly MemoryCache _cache;

        private readonly string _cacheName;
        private readonly string _instanceGuid = Guid.NewGuid().ToString();

        /// <summary>
        ///   Initializes a new instance of the <see cref="EnhancedMemoryCache&lt;TKey, TValue&gt;" /> class.
        /// </summary>
        /// <param name="cacheName">The name of the cache.</param>
        /// <param name="isolateKeys">
        ///   If set to <see langword="true"/> then isolates keys as underlying cache is shared.
        /// </param>
        public EnhancedMemoryCacheNull(string cacheName = null, bool isolateKeys = true)
        {
            if (string.IsNullOrWhiteSpace(cacheName))
            {
                _cache = MemoryCache.Default;
                _cacheName = null;
            }
            else
            {
                _cache = new MemoryCache(cacheName);
                _cacheName = cacheName;
            }
        }

        /// <summary>
        ///   Returns a <see cref="bool"/> value indicating whether the cache contains the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the key was found; otherwise returns <see langword="false"/>.
        /// </returns>
        public override bool ContainsKey(TKey key)
        {
            return _cache.Contains(_instanceGuid + key);
        }

        /// <summary>
        ///   Inserts a new entry into the cache using the specified key, the factory to create the value, the absolute expiration
        ///   and the sliding expiration (if an entry doesn't already exist otherwise the existing value is returned).
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <param name="value">The value to add to the cache.</param>
        /// <param name="absoluteExpiration">Sets when the cache entry will be expired.</param>
        /// <param name="slidingExpiration">
        ///   The duration to wait before expiring the cache (if no requests are made for it during that period).
        /// </param>
        /// <returns>
        ///   Either the inserted or retrieved value depending on whether there's already an existing entry with the same key.
        /// </returns>
        public override TValue GetOrAdd(
            TKey key,
            TValue value,
            DateTimeOffset absoluteExpiration,
            TimeSpan slidingExpiration)
        {
            if (slidingExpiration > _maxSlidingExpiration)
                slidingExpiration = ObjectCache.NoSlidingExpiration;
            Wrapper wrapper = new Wrapper(value);
            object result = _cache.AddOrGetExisting(
                _instanceGuid + key,
                wrapper,
                new CacheItemPolicy {AbsoluteExpiration = absoluteExpiration, SlidingExpiration = slidingExpiration},
                null);
            return result == null ? value : ((Wrapper) result).Value;
        }

        /// <summary>
        ///   Inserts a new entry into the cache using the specified key, value, absolute expiration and sliding expiration
        ///   (if an entry doesn't already exist otherwise the existing value is updated).
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <param name="value">The object to either insert or update the existing value with.</param>
        /// <param name="absoluteExpiration">Sets when the cache entry will be expired.</param>
        /// <param name="slidingExpiration">
        ///   The duration to wait before expiring the cache (if no requests are made for it during that period).
        /// </param>
        /// <returns>
        ///   Either the inserted or updated value depending on whether there's already an existing entry with the same key.
        /// </returns>
        public override TValue AddOrUpdate(
            TKey key,
            TValue value,
            DateTimeOffset absoluteExpiration,
            TimeSpan slidingExpiration)
        {
            if (slidingExpiration > _maxSlidingExpiration)
                slidingExpiration = ObjectCache.NoSlidingExpiration;
            Wrapper wrapper = new Wrapper(value);
            _cache.Set(
                _instanceGuid + key,
                wrapper,
                new CacheItemPolicy {AbsoluteExpiration = absoluteExpiration, SlidingExpiration = slidingExpiration},
                null);
            return value;
        }

        /// <summary>
        ///   Tries to retrieve the value using the specified key.
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <param name="value">The value retrieved.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the corresponding <paramref name="key"/> exists and the value was successfully retrieved;
        ///   otherwise <see langword="false"/>.
        /// </returns>
        public override bool TryGetValue(TKey key, out TValue value)
        {
            object result = _cache.Get(_instanceGuid + key);
            if (result == null)
            {
                value = default(TValue);
                return false;
            }
            value = ((Wrapper) result).Value;
            return true;
        }

        /// <summary>
        ///   Tries to remove an entry using the specified key.
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <param name="value">The value removed.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the corresponding <paramref name="key"/> exists and the value was successfully removed;
        ///   otherwise <see langword="false"/>.
        /// </returns>
        public override bool TryRemove(TKey key, out TValue value)
        {
            object result = _cache.Remove(_instanceGuid + key);
            if (result == null)
            {
                value = default(TValue);
                return false;
            }
            value = ((Wrapper) result).Value;
            return true;
        }

        /// <summary>
        ///   Tries to insert an entry into the cache using the specified key and value.
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <param name="value">The object to cache.</param>
        /// <param name="absoluteExpiration">The absolute expiration.</param>
        /// <param name="slidingExpiration">The sliding expiration.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the entry was inserted successfully; otherwise returns <see langword="false"/>.
        /// </returns>
        public override bool TryAdd(
            TKey key,
            TValue value,
            DateTimeOffset absoluteExpiration,
            TimeSpan slidingExpiration)
        {
            if (slidingExpiration > _maxSlidingExpiration)
                slidingExpiration = ObjectCache.NoSlidingExpiration;
            Wrapper wrapper = new Wrapper(value);
            return _cache.Add(
                _instanceGuid + key,
                wrapper,
                new CacheItemPolicy {AbsoluteExpiration = absoluteExpiration, SlidingExpiration = slidingExpiration});
        }

        /// <summary>
        ///   Flushes this instance.
        /// </summary>
        /// <exception cref="NotImplementedException">
        ///   <see cref="MemoryCache.Default"/> cannot be safely flushed.
        /// </exception>
        public override void Clear()
        {
            // Don't allow flushing of the default memory cache.
            if (string.IsNullOrWhiteSpace(_cacheName))
                throw new NotImplementedException(Resources.EnhancedMemoryCache_Clear_CannotSafelyFlush);

            _cache.Trim(100);
        }

        #region Nested type: Wrapper
        /// <summary>
        ///   A wrapper struct used to prevent nulls being placed in cache.
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