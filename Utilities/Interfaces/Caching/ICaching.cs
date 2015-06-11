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
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Interfaces.Caching
{
    /// <summary>
    ///   Defines a generic caching interface.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    [PublicAPI]
    public interface ICaching<TKey, TValue>
    {
        /// <summary>
        ///   Sets the default absolute expiration.
        /// </summary>
        [PublicAPI]
        TimeSpan DefaultAbsoluteExpiration { get; set; }

        /// <summary>
        ///   Sets the default sliding expiration.
        /// </summary>
        [PublicAPI]
        TimeSpan DefaultSlidingExpiration { get; set; }

        /// <summary>
        /// Clears the cache.
        /// </summary>
        [PublicAPI]
        void Clear();

        /// <summary>
        ///   Checks whether an entry with the specified key currently exists within the cache.
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the cache contains an entry with the same key value as
        ///   <paramref name="key"/>; otherwise <see langword="false"/>.
        /// </returns>
        [PublicAPI]
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
        [PublicAPI]
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
        [PublicAPI]
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
        [PublicAPI]
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
        [PublicAPI]
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
        [PublicAPI]
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
        [PublicAPI]
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
        [PublicAPI]
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
        [PublicAPI]
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
        [PublicAPI]
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
        [PublicAPI]
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
        [PublicAPI]
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
        [PublicAPI]
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
        [PublicAPI]
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
        [PublicAPI]
        bool TryRemove(TKey key, out TValue value);

        /// <summary>
        ///   Tries to insert an entry into the cache using the specified key and value.
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <param name="value">The object to cache.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the entry was inserted successfully; otherwise returns <see langword="false"/>.
        /// </returns>
        [PublicAPI]
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
        [PublicAPI]
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
        [PublicAPI]
        bool TryAdd(TKey key, TValue value, TimeSpan slidingExpiration);
    }
}