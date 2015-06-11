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
using WebApplications.Utilities.Interfaces.Caching;

namespace WebApplications.Utilities.Caching
{
    /// <summary>
    ///   Implements core functionality for caching classes.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    [PublicAPI]
    public abstract class CachingDictionaryBase<TKey, TValue> : ICaching<TKey, TValue>
    {
        private TimeSpan _defaultAbsoluteExpiration = TimeSpan.MaxValue;

        private TimeSpan _defaultSlidingExpiration = TimeSpan.Zero;

        /// <summary>
        ///   Initializes a new instance of the <see cref="CachingDictionaryBase{TKey,TValue}"/> class.
        /// </summary>
        protected CachingDictionaryBase()
        {
            DefaultAbsoluteExpiration = TimeSpan.MaxValue;
            DefaultSlidingExpiration = TimeSpan.MaxValue;
        }

        #region ICaching<TKey,TValue> Members
        /// <summary>
        ///   Gets or sets the default absolute expiration.
        /// </summary>
        public TimeSpan DefaultAbsoluteExpiration
        {
            get { return _defaultAbsoluteExpiration; }
            set
            {
                if (_defaultAbsoluteExpiration == value)
                    return;

                if (value != TimeSpan.MaxValue)
                    _defaultSlidingExpiration = TimeSpan.Zero;

                _defaultAbsoluteExpiration = value;
            }
        }

        /// <summary>
        ///   Gets or sets the default sliding expiration.
        /// </summary>
        public TimeSpan DefaultSlidingExpiration
        {
            get { return _defaultSlidingExpiration; }
            set
            {
                if (_defaultSlidingExpiration == value)
                    return;

                if (value != TimeSpan.MaxValue)
                    _defaultAbsoluteExpiration = TimeSpan.MaxValue;

                _defaultSlidingExpiration = value;
            }
        }

        /// <summary>
        ///   Checks whether an entry with the specified key currently exists within the cache.
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the cache contains an entry with the same key value as <paramref name="key"/>;
        ///   otherwise <see langword="false"/>.
        /// </returns>
        public abstract bool ContainsKey(TKey key);

        /// <summary>
        ///   Inserts a new entry into the cache using the specified key and value
        ///   (if an entry doesn't already exist otherwise the existing value is returned).
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <param name="value">The object to cache the value of.</param>
        /// <returns>
        ///   Either the inserted or retrieved value depending on whether there's already an existing entry with the same key.
        /// </returns>
        public virtual TValue GetOrAdd([NotNull] TKey key, TValue value)
        {
            if (key == null) throw new ArgumentNullException("key");
            return GetOrAdd(key, value, AbsoluteExpiration(), DefaultSlidingExpiration);
        }

        /// <summary>
        ///   Inserts a new entry into the cache using the specified key and value along with the absolute expiration
        ///   (if an entry doesn't already exist otherwise the existing value is returned).
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <param name="value">The object to cache the value of.</param>
        /// <param name="absoluteExpiration">Sets when the cache entry will be expired.</param>
        /// <returns>
        /// Either the inserted or retrieved value depending on whether there's already an existing entry with the same key.
        /// </returns>
        public TValue GetOrAdd([NotNull] TKey key, TValue value, DateTimeOffset absoluteExpiration)
        {
            if (key == null) throw new ArgumentNullException("key");
            return GetOrAdd(key, value, absoluteExpiration, DefaultSlidingExpiration);
        }

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
        ///   Either the inserted or retrieved value depending on whether there's already an existing entry with the same key.
        /// </returns>
        public TValue GetOrAdd([NotNull] TKey key, TValue value, TimeSpan slidingExpiration)
        {
            if (key == null) throw new ArgumentNullException("key");
            return GetOrAdd(key, value, AbsoluteExpiration(), slidingExpiration);
        }

        /// <summary>
        ///   Inserts a new entry into the cache using the specified key and a factory to create the value
        ///   (if an entry doesn't already exist otherwise the existing value is returned).
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <param name="addValueFactory">Used to create the value we want to cache.</param>
        /// <returns>
        ///   Either the inserted or retrieved value depending on whether there's already an existing entry with the same key.
        /// </returns>
        public TValue GetOrAdd([NotNull] TKey key, [NotNull] Func<TKey, TValue> addValueFactory)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (addValueFactory == null) throw new ArgumentNullException("addValueFactory");
            return GetOrAdd(key, addValueFactory, AbsoluteExpiration(), DefaultSlidingExpiration);
        }

        /// <summary>
        ///   Inserts a new entry into the cache using the specified key, the factory to create the value and the absolute expiration
        ///   (if an entry doesn't already exist otherwise the existing value is returned).
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <param name="addValueFactory">Used to create the value we want to cache.</param>
        /// <param name="absoluteExpiration">Sets when the cache entry will be expired.</param>
        /// <returns>
        ///   Either the inserted or retrieved value depending on whether there's already an existing entry with the same key.
        /// </returns>
        public TValue GetOrAdd(
            [NotNull] TKey key,
            [NotNull] Func<TKey, TValue> addValueFactory,
            DateTimeOffset absoluteExpiration)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (addValueFactory == null) throw new ArgumentNullException("addValueFactory");
            return GetOrAdd(key, addValueFactory, absoluteExpiration, DefaultSlidingExpiration);
        }

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
        public TValue GetOrAdd(
            [NotNull] TKey key,
            [NotNull] Func<TKey, TValue> addValueFactory,
            TimeSpan slidingExpiration)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (addValueFactory == null) throw new ArgumentNullException("addValueFactory");
            return GetOrAdd(key, addValueFactory, AbsoluteExpiration(), slidingExpiration);
        }

        /// <summary>
        ///   Inserts a new entry into the cache using the specified key and value
        ///   (if an entry doesn't already exist otherwise the existing value is updated).
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <param name="value">The object to either insert or update the existing value with.</param>
        /// <returns>
        ///   Either the inserted or updated value depending on whether there's already an existing entry with the same key.
        /// </returns>
        public TValue AddOrUpdate([NotNull] TKey key, TValue value)
        {
            if (key == null) throw new ArgumentNullException("key");
            return AddOrUpdate(key, value, AbsoluteExpiration(), DefaultSlidingExpiration);
        }

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
        public TValue AddOrUpdate([NotNull] TKey key, TValue value, DateTimeOffset absoluteExpiration)
        {
            if (key == null) throw new ArgumentNullException("key");
            return AddOrUpdate(key, value, absoluteExpiration, DefaultSlidingExpiration);
        }

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
        public TValue AddOrUpdate([NotNull] TKey key, TValue value, TimeSpan slidingExpiration)
        {
            if (key == null) throw new ArgumentNullException("key");
            return AddOrUpdate(key, value, AbsoluteExpiration(), slidingExpiration);
        }

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
        public TValue AddOrUpdate(
            [NotNull] TKey key,
            [NotNull] Func<TKey, TValue> addValueFactory,
            [NotNull] Func<TKey, TValue, TValue> updateValueFactory)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (addValueFactory == null) throw new ArgumentNullException("addValueFactory");
            if (updateValueFactory == null) throw new ArgumentNullException("updateValueFactory");
            return AddOrUpdate(
                key,
                addValueFactory,
                updateValueFactory,
                AbsoluteExpiration(),
                DefaultSlidingExpiration);
        }

        /// <summary>
        ///   Inserts a new entry into the cache using the key, the add/update factories and the absolute expiration values specified
        ///   (if an entry doesn't already exist otherwise the existing value is updated).
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <param name="addValueFactory">Used to create the value that we want to cache.</param>
        /// <param name="updateValueFactory">Used to update the value if it's already cached.</param>
        /// <param name="absoluteExpiration">Sets when the cache entry will be expired.</param>
        /// <returns>
        ///   Either the inserted or updated value depending on whether or not there's already an existing entry with the same key.
        /// </returns>
        public TValue AddOrUpdate(
            [NotNull] TKey key,
            [NotNull] Func<TKey, TValue> addValueFactory,
            [NotNull] Func<TKey, TValue, TValue> updateValueFactory,
            DateTimeOffset absoluteExpiration)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (addValueFactory == null) throw new ArgumentNullException("addValueFactory");
            if (updateValueFactory == null) throw new ArgumentNullException("updateValueFactory");
            return AddOrUpdate(
                key,
                addValueFactory,
                updateValueFactory,
                absoluteExpiration,
                DefaultSlidingExpiration);
        }

        /// <summary>
        ///   Inserts a new entry into the cache using the key, the add/update factories and the sliding expiration values specified
        ///   (if an entry doesn't already exist otherwise the existing value is updated).
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <param name="addValueFactory">Used to calculate the value we want to cache.</param>
        /// <param name="updateValueFactory">Used to update the value if it's already cached.</param>
        /// <param name="slidingExpiration">
        ///   The duration to wait before expiring the cache (if no requests are made for it during that period).
        /// </param>
        /// <returns>
        ///   Either the inserted or updated value depending on whether or not there's already an existing entry in the cache
        ///   with the same key as <paramref name="key"/>.
        /// </returns>
        public TValue AddOrUpdate(
            [NotNull] TKey key,
            [NotNull] Func<TKey, TValue> addValueFactory,
            [NotNull] Func<TKey, TValue, TValue> updateValueFactory,
            TimeSpan slidingExpiration)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (addValueFactory == null) throw new ArgumentNullException("addValueFactory");
            if (updateValueFactory == null) throw new ArgumentNullException("updateValueFactory");
            return AddOrUpdate(
                key,
                addValueFactory,
                updateValueFactory,
                AbsoluteExpiration(),
                slidingExpiration);
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
        public abstract bool TryGetValue([NotNull] TKey key, out TValue value);

        /// <summary>
        ///   Tries to remove an entry using the specified key.
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <param name="value">The value removed.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the corresponding <paramref name="key"/> exists and the value was successfully removed;
        ///   otherwise <see langword="false"/>.
        /// </returns>
        public abstract bool TryRemove([NotNull] TKey key, out TValue value);

        /// <summary>
        ///   Tries to insert an entry into the cache using the specified key and value.
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <param name="value">The object to cache.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the entry was inserted successfully; otherwise returns <see langword="false"/>.
        /// </returns>
        public bool TryAdd([NotNull] TKey key, TValue value)
        {
            if (key == null) throw new ArgumentNullException("key");
            return TryAdd(key, value, AbsoluteExpiration(), DefaultSlidingExpiration);
        }

        /// <summary>
        ///   Tries to insert an entry into the cache using the specified key, value and absolute expiration.
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <param name="value">The object to cache.</param>
        /// <param name="absoluteExpiration">Sets when the cache entry will be expired.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the entry was inserted successfully; otherwise returns <see langword="false"/>.
        /// </returns>
        public bool TryAdd([NotNull] TKey key, TValue value, DateTimeOffset absoluteExpiration)
        {
            if (key == null) throw new ArgumentNullException("key");
            return TryAdd(key, value, absoluteExpiration, DefaultSlidingExpiration);
        }

        /// <summary>
        ///   Tries to insert an entry into the cache using the specified key, value and sliding expiration.
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <param name="value">The object to cache.</param>
        /// <param name="slidingExpiration">
        ///   The duration to wait before expiring the cache (if no requests are made for it during that period).
        /// </param>
        /// <returns>
        /// Returns <see langword="true"/> if the entry was inserted successfully; otherwise returns <see langword="false"/>.
        /// </returns>
        public bool TryAdd([NotNull] TKey key, TValue value, TimeSpan slidingExpiration)
        {
            if (key == null) throw new ArgumentNullException("key");
            return TryAdd(key, value, AbsoluteExpiration(), slidingExpiration);
        }

        /// <summary>
        ///   Clears the cache.
        /// </summary>
        public abstract void Clear();
        #endregion

        /// <summary>
        ///   Inserts a new entry into the cache using the specified key, the value, the absolute expiration and the sliding expiration
        ///   (if an entry doesn't already exist otherwise the existing value is returned).
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <param name="value">The value.</param>
        /// <param name="absoluteExpiration">Sets when the cache entry will be expired.</param>
        /// <param name="slidingExpiration">
        ///   The duration to wait before expiring the cache (if no requests are made for it during that period).
        /// </param>
        /// <returns>
        ///   Either the inserted or retrieved value depending on whether there's already an existing entry with the same key.
        /// </returns>
        [PublicAPI]
        public abstract TValue GetOrAdd(
            [NotNull] TKey key,
            TValue value,
            DateTimeOffset absoluteExpiration,
            TimeSpan slidingExpiration);

        /// <summary>
        ///   Inserts a new entry into the cache using the specified key, the factory to create the value, the absolute expiration
        ///   and the sliding expiration (if an entry doesn't already exist otherwise the existing value is returned).
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <param name="addValueFactory">Used to create the value we want to cache.</param>
        /// <param name="absoluteExpiration">Sets when the cache entry will be expired.</param>
        /// <param name="slidingExpiration">
        ///   The duration to wait before expiring the cache (if no requests are made for it during that period).
        /// </param>
        /// <returns>
        ///   Either the inserted or retrieved value depending on whether there's already an existing entry with the same key.
        /// </returns>
        [PublicAPI]
        // ReSharper disable once VirtualMemberNeverOverriden.Global
        public virtual TValue GetOrAdd(
            [NotNull] TKey key,
            [NotNull] Func<TKey, TValue> addValueFactory,
            DateTimeOffset absoluteExpiration,
            TimeSpan slidingExpiration)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (addValueFactory == null) throw new ArgumentNullException("addValueFactory");

            TValue value;
            return TryGetValue(key, out value)
                ? value
                : GetOrAdd(key, addValueFactory(key), absoluteExpiration, slidingExpiration);
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
        [PublicAPI]
        public abstract TValue AddOrUpdate(
            [NotNull] TKey key,
            TValue value,
            DateTimeOffset absoluteExpiration,
            TimeSpan slidingExpiration);

        /// <summary>
        ///   Inserts a new entry into the cache using the specified key and the add/update factories
        ///   (if an entry doesn't already exist otherwise the existing value is updated).
        /// </summary>
        /// <param name="key">The unique identifier for the cache entry.</param>
        /// <param name="addValueFactory">Used to create the value that we want to cache.</param>
        /// <param name="updateValueFactory">Used to update the value if it's already cached.</param>
        /// <param name="absoluteExpiration">Sets when the cache entry will be expired.</param>
        /// <param name="slidingExpiration">
        ///   The duration to wait before expiring the cache (if no requests are made for it during that period).
        /// </param>
        /// <returns>
        ///   Either the inserted or updated value depending on whether there's already an existing entry with the same key.
        /// </returns>
        [PublicAPI] // ReSharper disable once VirtualMemberNeverOverriden.Global
        public virtual TValue AddOrUpdate(
            [NotNull] TKey key,
            [NotNull] Func<TKey, TValue> addValueFactory,
            [NotNull] Func<TKey, TValue, TValue> updateValueFactory,
            DateTimeOffset absoluteExpiration,
            TimeSpan slidingExpiration)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (addValueFactory == null) throw new ArgumentNullException("addValueFactory");
            if (updateValueFactory == null) throw new ArgumentNullException("updateValueFactory");
            TValue value;
            // Call factory outside of lock, and pass into standard AddOrUpdate
            return AddOrUpdate(
                key,
                TryGetValue(key, out value) ? updateValueFactory(key, value) : addValueFactory(key),
                absoluteExpiration,
                slidingExpiration);
        }

        /// <summary>
        ///   Tries to insert an entry into the cache using the specified key, value, absolute expiration and sliding expiration.
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
        [PublicAPI]
        public abstract bool TryAdd(
            TKey key,
            TValue value,
            DateTimeOffset absoluteExpiration,
            TimeSpan slidingExpiration);

        /// <summary>
        ///   Calculate the default absolute expiration.
        /// </summary>
        private DateTimeOffset AbsoluteExpiration()
        {
            return DefaultAbsoluteExpiration < TimeSpan.MaxValue
                ? DateTimeOffset.Now.Add(DefaultAbsoluteExpiration)
                : DateTimeOffset.MaxValue;
        }
    }
}