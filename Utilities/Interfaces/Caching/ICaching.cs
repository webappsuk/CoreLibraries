#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities
// File: ICaching.cs
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

namespace WebApplications.Utilities.Interfaces.Caching
{
    /// <summary>
    ///   Defines a generic caching interface.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    public interface ICaching<TKey, TValue>
    {
        /// <summary>
        ///   Sets the default absolute expiration.
        /// </summary>
        TimeSpan DefaultAbsoluteExpiration { get; set; }

        /// <summary>
        ///   Sets the default sliding expiration.
        /// </summary>
        TimeSpan DefaultSlidingExpiration { get; set; }

        /// <summary>
        /// Clears the cache.
        /// </summary>
        void Clear();

        /// <summary>
        ///   Checks whether an entry with the specified key currently exists within the cache.
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the cache contains an entry with the same key value as
        ///   <paramref name="key"/>; otherwise <see langword="false"/>.
        /// </returns>
        bool ContainsKey(TKey key);

        /// <summary>
        ///   Inserts a new entry into the cache using the specified key and value
        ///   (if an entry doesn't already exist otherwise the existing value is returned).
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <param name="value">The object to cache the value of.</param>
        /// <returns>
        ///   Either the inserted or retrieved value depending on whether there's already an existing
        ///   entry with the same key.
        /// </returns>
        TValue GetOrAdd(TKey key, TValue value);

        /// <summary>
        ///   Inserts a new entry into the cache using the specified key and value along with the absolute expiration
        ///   (if an entry doesn't already exist otherwise the existing value is returned).
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <param name="value">The object to cache the value of.</param>
        /// <param name="absoluteExpiration">Sets when the cache entry will be expired.</param>
        /// <returns>
        ///   Either the inserted or retrieved value depending on whether there's already an existing
        ///   entry with the same key.
        /// </returns>
        TValue GetOrAdd(TKey key, TValue value, DateTimeOffset absoluteExpiration);

        /// <summary>
        ///   Inserts a new entry into the cache using the specified key and value along with the sliding expiration
        ///   (if an entry doesn't already exist otherwise the existing value is returned).
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <param name="value">The object to cache the value of.</param>
        /// <param name="slidingExpiration">
        ///   The duration to wait before expiring the cache (if no requests are made for it during that period).
        /// </param>
        /// <returns>
        ///   Either the inserted or retrieved value depending on whether there's already an existing
        ///   entry with the same key.
        /// </returns>
        TValue GetOrAdd(TKey key, TValue value, TimeSpan slidingExpiration);

        /// <summary>
        ///   Inserts a new entry into the cache using the specified key and a factory to create the value
        ///   (if an entry doesn't already exist otherwise the existing value is returned).
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <param name="addValueFactory">Used to create the value we want to cache.</param>
        /// <returns>
        ///   Either the inserted or retrieved value depending on whether there's already an existing
        ///   entry with the same key.
        /// </returns>
        TValue GetOrAdd(TKey key, Func<TKey, TValue> addValueFactory);

        /// <summary>
        ///   Inserts a new entry into the cache using the specified key, the factory to create the value and the absolute expiration
        ///   (if an entry doesn't already exist otherwise the existing value is returned).
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <param name="addValueFactory">Used to create the value we want to cache.</param>
        /// <param name="absoluteExpiration">Sets when the cache entry will be expired.</param>
        /// <returns>
        ///   Either the inserted or retrieved value depending on whether there's already an existing
        ///   entry with the same key.
        /// </returns>
        TValue GetOrAdd(TKey key, Func<TKey, TValue> addValueFactory, DateTimeOffset absoluteExpiration);

        /// <summary>
        ///   Inserts a new entry into the cache using the specified key, the factory to create the value and the sliding expiration
        ///   (if an entry doesn't already exist otherwise the existing value is returned).
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <param name="addValueFactory">Used to create the value we want to cache.</param>
        /// <param name="slidingExpiration">
        ///   The duration to wait before expiring the cache (if no requests are made for it during that period).
        /// </param>
        /// <returns>
        ///   Either the inserted or retrieved value depending on whether there's already an existing entry with the same key.
        /// </returns>
        TValue GetOrAdd(TKey key, Func<TKey, TValue> addValueFactory, TimeSpan slidingExpiration);

        /// <summary>
        ///   Inserts a new entry into the cache using the specified key and value
        ///   (if an entry doesn't already exist otherwise the existing value is updated).
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <param name="value">The object to either insert or update the existing value with.</param>
        /// <returns>
        ///   Either the inserted or updated value depending on whether there's already an existing entry with the same key.
        /// </returns>
        TValue AddOrUpdate(TKey key, TValue value);

        /// <summary>
        ///   Inserts a new entry into the cache using the specified key, value and absolute expiration
        ///   (if an entry doesn't already exist otherwise the existing value is updated).
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <param name="value">The object to either insert or update the existing value with.</param>
        /// <param name="absoluteExpiration">Sets when the cache entry will be expired.</param>
        /// <returns>
        ///   Either the inserted or updated value depending on whether there's already an existing entry with the same key.
        /// </returns>
        TValue AddOrUpdate(TKey key, TValue value, DateTimeOffset absoluteExpiration);

        /// <summary>
        ///   Inserts a new entry into the cache using the specified key, value and sliding expiration
        ///   (if an entry doesn't already exist otherwise the existing value is updated).
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <param name="value">The object to either insert or update the existing value with.</param>
        /// <param name="slidingExpiration">
        ///   The duration to wait before expiring the cache (if no requests are made for it during that period).
        /// </param>
        /// <returns>
        ///   Either the inserted or updated value depending on whether there's already an existing entry with the same key.
        /// </returns>
        TValue AddOrUpdate(TKey key, TValue value, TimeSpan slidingExpiration);

        /// <summary>
        ///   Inserts a new entry into the cache using the specified key and the add/update factories
        ///   (if an entry doesn't already exist otherwise the existing value is updated).
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <param name="addValueFactory">Used to create the value that we want to cache.</param>
        /// <param name="updateValueFactory">Used to update the value if it's already cached.</param>
        /// <returns>
        ///   Either the inserted or updated value depending on whether there's already an existing entry with the same key.
        /// </returns>
        TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory);

        /// <summary>
        ///   Inserts a new entry into the cache using the key, the add/update factories and the absolute expiration
        ///   (if an entry doesn't already exist otherwise the existing value is updated).
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <param name="addValueFactory">Used to create the value that we want to cache.</param>
        /// <param name="updateValueFactory">Used to update the value if it's already cached.</param>
        /// <param name="absoluteExpiration">Sets when the cache entry will be expired.</param>
        /// <returns>
        ///   Either the inserted or updated value depending on whether or not there's already an existing entry with the same key.
        /// </returns>
        TValue AddOrUpdate(
            TKey key,
            Func<TKey, TValue> addValueFactory,
            Func<TKey, TValue, TValue> updateValueFactory,
            DateTimeOffset absoluteExpiration);

        /// <summary>
        ///   Inserts a new entry into the cache using the key, the add/update factories and the sliding expiration
        ///   (if an entry doesn't already exist otherwise the existing value is updated).
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <param name="addValueFactory">Used to calculate the value we want to cache.</param>
        /// <param name="updateValueFactory">Used to update the value if it's already cached.</param>
        /// <param name="slidingExpiration">
        ///   The duration to wait before expiring the cache (if no requests are made for it during that period).
        /// </param>
        /// <returns>
        ///   Either the inserted or updated value depending on whether or not there's already an existing
        ///   entry in the cache with the same key as <paramref name="key"/>.
        /// </returns>
        TValue AddOrUpdate(
            TKey key,
            Func<TKey, TValue> addValueFactory,
            Func<TKey, TValue, TValue> updateValueFactory,
            TimeSpan slidingExpiration);

        /// <summary>
        ///   Tries to retrieve the value using the specified key.
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <param name="value">The value retrieved.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the corresponding <paramref name="key"/> exists and the value was
        ///   successfully retrieved; otherwise <see langword="false"/>.
        /// </returns>
        bool TryGetValue(TKey key, out TValue value);

        /// <summary>
        ///   Tries to remove an entry using the specified key.
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <param name="value">The value removed.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the corresponding <paramref name="key"/> exists and the value was
        ///   successfully removed; otherwise <see langword="false"/>.
        /// </returns>
        bool TryRemove(TKey key, out TValue value);

        /// <summary>
        ///   Tries to insert an entry into the cache using the specified key and value.
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <param name="value">The object to cache.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the entry was inserted successfully; otherwise returns <see langword="false"/>.
        /// </returns>
        bool TryAdd(TKey key, TValue value);

        /// <summary>
        ///   Tries to insert an entry into the cache using the specified key, value and absolute expiration.
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <param name="value">The object to cache.</param>
        /// <param name="absoluteExpiration">Sets when the cache entry will be expired.</param>
        /// <returns>
        /// Returns <see langword="true"/> if the entry was inserted successfully; otherwise returns <see langword="false"/>.
        /// </returns>
        bool TryAdd(TKey key, TValue value, DateTimeOffset absoluteExpiration);

        /// <summary>
        ///   Tries to insert an entry into the cache using the specified key, value and sliding expiration.
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <param name="value">The object to cache.</param>
        /// <param name="slidingExpiration">
        ///   The duration to wait before expiring the cache (if no requests are made for it during that period).
        /// </param>
        /// <returns>
        ///   Returns <see langword="true"/> if the entry was inserted successfully; otherwise returns <see langword="false"/>.
        /// </returns>
        bool TryAdd(TKey key, TValue value, TimeSpan slidingExpiration);
    }
}