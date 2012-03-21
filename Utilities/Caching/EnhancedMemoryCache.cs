#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities
// File: EnhancedMemoryCache.cs
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
using System.Runtime.Caching;

namespace WebApplications.Utilities.Caching
{
    /// <summary>
    ///   OBSOLETE - Use <see cref="EnhancedMemoryCache&lt;TKey, Tvalue&gt;"/>
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    [Obsolete("Use the newer EnhancedMemoryCahce<TKey, TValue>")]
    public class EnhancedMemoryCache<TValue> : EnhancedMemoryCache<string, TValue>
    {
    }

    /// <summary>
    ///   An Enhanced Memory Cache implementation, uses the <see cref="MemoryCache.Default">default Memory Cache</see>,
    ///   but allows multiple instances to be created (they all map share the underlying memory cache). This ensures
    ///   type-safety on the cache values but does not allow the value to be a <see langword="null"/>. For null value
    ///   support you can use <see cref="EnhancedMemoryCacheNull&lt;TKey, TValue&gt;"/>, though there is a small
    ///   performance and memory overhead in doing so.
    /// </summary>
    /// <typeparam name="TKey">
    ///   The type of the key - it is vital that the <see cref="object.ToString()" /> method returns a unique value for discrimination.
    /// </typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class EnhancedMemoryCache<TKey, TValue> : CachingDictionaryBase<TKey, TValue>
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
        public EnhancedMemoryCache(string cacheName = null)
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
        ///   Initializes a new instance of the <see cref="EnhancedMemoryCache&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="defaultAbsoluteExpiration">
        ///   <para>The default absolute expiration.</para>
        ///   <para>This sets when the cache entry will be expired.</para>
        /// </param>
        /// <param name="defaultSlidingExpiration">
        ///   <para>The default sliding expiration.</para>
        ///   <para>This is the duration to wait before expiring the cache (if no requests are made for it during that period).</para>
        /// </param>
        /// <param name="cacheName">The name of the cache.</param>
        public EnhancedMemoryCache(
            TimeSpan defaultAbsoluteExpiration, TimeSpan defaultSlidingExpiration, string cacheName = null)
        {
            DefaultAbsoluteExpiration = defaultAbsoluteExpiration;
            DefaultSlidingExpiration = defaultSlidingExpiration;
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
        /// Either the inserted or retrieved value depending on whether there's already an existing entry with the same key.
        /// </returns>
        public override TValue GetOrAdd(
            TKey key, TValue value, DateTimeOffset absoluteExpiration, TimeSpan slidingExpiration)
        {
            if (slidingExpiration > _maxSlidingExpiration)
                slidingExpiration = ObjectCache.NoSlidingExpiration;
            object result = _cache.AddOrGetExisting(
                _instanceGuid + key,
                value,
                new CacheItemPolicy {AbsoluteExpiration = absoluteExpiration, SlidingExpiration = slidingExpiration},
                null);
            return result == null ? value : (TValue) result;
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
            TKey key, TValue value, DateTimeOffset absoluteExpiration, TimeSpan slidingExpiration)
        {
            if (slidingExpiration > _maxSlidingExpiration)
                slidingExpiration = ObjectCache.NoSlidingExpiration;
            _cache.Set(
                _instanceGuid + key,
                value,
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
            value = (TValue) result;
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
            value = (TValue) result;
            return true;
        }

        /// <summary>
        ///   Tries to insert an entry into the cache using the specified key and value.
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <param name="value">The object to cache.</param>
        /// <param name="absoluteExpiration">Sets when the cache entry will be expired.</param>
        /// <param name="slidingExpiration">
        ///   The duration to wait before expiring the cache (if no requests are made for it during that period).
        /// </param>
        /// <returns>
        ///   Returns <see langword="true"/> if the entry was inserted successfully; otherwise returns <see langword="false"/>.
        /// </returns>
        public override bool TryAdd(
            TKey key, TValue value, DateTimeOffset absoluteExpiration, TimeSpan slidingExpiration)
        {
            if (slidingExpiration > _maxSlidingExpiration)
                slidingExpiration = ObjectCache.NoSlidingExpiration;
            return _cache.Add(
                _instanceGuid + key,
                value,
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
    }
}