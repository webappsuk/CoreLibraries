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
using System.Linq;
using System.Threading;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Caching
{
    /// <summary>
    ///   Implements a thread safe FIFO queue of objects that will only remain present for a specific duration.
    /// </summary>
    /// <typeparam name="T">The type of the queued objects.</typeparam>
    [PublicAPI]
    public class CachingQueue<T> : IProducerConsumerCollection<T>
    {
        /// <summary>
        ///   The time that an element remains in the queue.
        /// </summary>
        public readonly TimeSpan CacheExpiry;

        /// <summary>
        ///   If the queue isn't used for this period of time then expired items are cleaned out automatically.
        ///   Normally a queue is only cleaned as it's used.
        ///   <see cref="TimeSpan.Zero"/> indicates the queue is never cleaned out automatically.
        /// </summary>
        public readonly TimeSpan CleanAfter;

        /// <summary>
        ///   The maximum length of the queue.
        /// </summary>
        public readonly int MaximumEntries;

        /// <summary>
        ///   A lock <see cref="object"/> to ensure that clean is not re-entrant.
        /// </summary>
        private readonly object _cleanLock = new object();

        /// <summary>
        ///   The <see cref="System.Threading.Timer"/> used for cleaning the queue automatically
        /// </summary>
        private readonly Timer _cleanTimer;

        /// <summary>
        ///   Holds the underlying queue.
        /// </summary>
        [NotNull]
        private readonly ConcurrentQueue<Wrapper> _queue = new ConcurrentQueue<Wrapper>();

        /// <summary>
        ///   Initializes a new instance of the <see cref="CachingQueue&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="cacheExpiry">The cache expiry.</param>
        /// <param name="maximumEntries">The maximum entries.</param>
        /// <param name="cleanAfter">
        ///   <para>If the queue is unused after this period of time then it is cleaned automatically.</para>
        ///   <para>A negative value or <see cref="TimeSpan.Zero"/> means the queue is only cleaned on use and not automatically.</para>
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <para><paramref name="cacheExpiry"/> must be more than 1 second.</para>
        ///   <para>-or-</para>
        ///   <paramref name="maximumEntries"/> must be at least 1.
        /// </exception>
        public CachingQueue(
            TimeSpan cacheExpiry = default(TimeSpan),
            int maximumEntries = int.MaxValue,
            TimeSpan cleanAfter = default(TimeSpan))
        {
            if (cacheExpiry == default(TimeSpan))
                cacheExpiry = TimeSpan.FromDays(1);
            else if (cacheExpiry < TimeSpan.FromSeconds(1))
                throw new ArgumentOutOfRangeException(
                    "cacheExpiry",
                    cacheExpiry,
                    Resources.CachingQueue_ExpiryTooShort);
            if (cleanAfter > TimeSpan.Zero)
            {
                // Create timer.
                _cleanTimer = new Timer(s => Clean(), null, (long)CleanAfter.TotalMilliseconds, -1);
                CleanAfter = cleanAfter;
            }
            else
                CleanAfter = TimeSpan.Zero;

            if (maximumEntries < 1)
                throw new ArgumentOutOfRangeException(
                    "maximumEntries",
                    maximumEntries,
                    Resources.CachingQueue_MaxEntriesLessThanOne);

            CacheExpiry = cacheExpiry;
            MaximumEntries = maximumEntries;
        }

        #region IProducerConsumerCollection<T> Members
        /// <summary>
        ///   Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///   A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<T> GetEnumerator()
        {
            // Get an enumerator.
            List<Wrapper>.Enumerator enumerator = _queue.ToList().GetEnumerator();

            while (enumerator.MoveNext())
            {
                // Skip expired elements
                if (enumerator.Current.IsExpired) continue;
                yield return enumerator.Current.Value;
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
        ///   Copies the elements of the <see cref="T:System.Collections.ICollection"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">
        ///   The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection"/>.
        ///   The <see cref="T:System.Array"/> must have zero-based indexing. </param><param name="index">The zero-based index in <paramref name="array"/> at which copying begins.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.</exception>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="array"/> is multidimensional.-or- The number of elements in the source <see cref="T:System.Collections.ICollection"/> is greater than the available
        ///   space from <paramref name="index"/> to the end of the destination <paramref name="array"/>.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        ///   The type of the source <see cref="T:System.Collections.ICollection"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            ((ICollection)this.ToList()).CopyTo(array, index);
        }

        /// <summary>
        ///   Gets the number of elements contained in the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        /// <returns>
        ///   The number of elements contained in the <see cref="T:System.Collections.ICollection"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        int ICollection.Count
        {
            get { return _queue.Count; }
        }

        /// <summary>
        ///   Gets an <see cref="object"/> that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        /// <returns>
        ///   An <see cref="object"/> that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        object ICollection.SyncRoot
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        ///   Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe).
        /// </summary>
        /// <returns>
        ///   Returns <see langword="true"/> if access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe);
        ///   otherwise returns <see langword="false"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        /// <summary>
        ///   Copies the elements of the <see cref="T:System.Collections.Concurrent.IProducerConsumerCollection`1">IProducerConsumerCollection</see>
        ///   to an <see cref="Array"/>, starting at a specified index.
        /// </summary>
        /// <param name="array">
        ///   <para>The one-dimensional array that is the destination of the elements copied from the
        ///   <see cref="T:System.Collections.Concurrent.IProducerConsumerCollection`1">IProducerConsumerCollection</see>.</para>
        ///   <para>The array must have zero-based indexing.</para>
        /// </param>
        /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="array"/> is <see langword="null">null</see>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.</exception>
        /// <exception cref="T:System.ArgumentException">
        ///   <para><paramref name="index"/> is greater than or equal to the length of the <paramref name="array"/>.</para>
        ///   <para>-or-</para>
        ///   <para>The number of elements in the source <see cref="T:System.Collections.Concurrent.ConcurrentQueue`1">ConcurrentQueue</see> is greater
        ///   than the available space from <paramref name="index"/> to the end of the destination <paramref name="array"/>.</para>
        /// </exception>
        void IProducerConsumerCollection<T>.CopyTo([NotNull] T[] array, int index)
        {
            this.ToList().CopyTo(array, index);
        }

        /// <summary>
        ///   Attempts to add an <see cref="object"/> to the
        ///   <see cref="T:System.Collections.Concurrent.IProducerConsumerCollection`1">IProducerConsumerCollection</see>.
        /// </summary>
        /// <param name="item">
        ///   The object to add to the <see cref="T:System.Collections.Concurrent.IProducerConsumerCollection`1">IProducerConsumerCollection</see>.
        /// </param>
        /// <returns>
        ///   Returns <see langword="true"/> if an <see cref="object"/> was removed and returned successfully; otherwise returns <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="item"/> was invalid for this collection.</exception>
        bool IProducerConsumerCollection<T>.TryAdd(T item)
        {
            Enqueue(item);
            return true;
        }

        /// <summary>
        ///   Attempts to remove and return an <see cref="object"/> from the
        ///   <see cref="T:System.Collections.Concurrent.IProducerConsumerCollection`1">IProducerConsumerCollection</see>.
        /// </summary>
        /// <param name="item">
        ///   When this method returns, if the object was removed and returned successfully, <paramref name="item"/> contains the removed object.
        ///   If no object was available to be removed, the value is unspecified.
        /// </param>
        /// <returns>
        ///   Returns <see langword="true"/> if an <see cref="object"/> was removed and returned successfully; otherwise returns <see langword="false"/>.
        /// </returns>
        bool IProducerConsumerCollection<T>.TryTake(out T item)
        {
            return TryDequeue(out item);
        }

        /// <summary>
        ///   Copies the elements contained in the <see cref="T:System.Collections.Concurrent.IProducerConsumerCollection`1">IProducerConsumerCollection</see>.
        ///   to a new <see cref="Array"/>.
        /// </summary>
        /// <returns>
        ///   A new <see cref="Array"/> containing the elements copied from the
        ///   <see cref="T:System.Collections.Concurrent.IProducerConsumerCollection`1">IProducerConsumerCollection</see>.
        /// </returns>
        T[] IProducerConsumerCollection<T>.ToArray()
        {
            return this.ToArray();
        }
        #endregion

        /// <summary>
        ///   Enqueues the specified item.
        /// </summary>
        /// <param name="item">The item to add to the queue.</param>
        public void Enqueue(T item)
        {
            _queue.Enqueue(new Wrapper(item, DateTime.Now.Add(CacheExpiry)));

            if (MaximumEntries == int.MaxValue) return;

            // Try to clear down excess items.
            Wrapper wrapper;
            while ((_queue.Count > MaximumEntries) &&
                   (_queue.TryDequeue(out wrapper)))
            {
            }
        }

        /// <summary>
        ///   Tries to dequeue the item at the beginning of the <see cref="CachingQueue&lt;T&gt;"/>.
        /// </summary>
        /// <param name="result">
        ///   <para>The result.</para>
        ///   <para>If no object is available to be removed then the default value of <typeparamref name="T"/> is returned.</para>
        /// </param>
        /// <returns>
        ///   Returns <see langword="true"/> if an item was removed from the beginning of the queue successfully;
        ///   otherwise returns <see langword="false"/>.
        /// </returns>
        /// <remarks>Will also dequeue any expired items at the front of the queue until a non-expired item is found.</remarks>
        public bool TryDequeue(out T result)
        {
            Wrapper wrapper;
            while (_queue.TryDequeue(out wrapper))
            {
                if (wrapper.IsExpired)
                    continue;
                result = wrapper.Value;
                return true;
            }
            result = default(T);
            return false;
        }

        /// <summary>
        ///   Tries to return the first non-expired item in the <see cref="CachingQueue&lt;T&gt;"/> without removing it.
        ///   If expired items are found to be first in the queue then they are dequeued.
        /// </summary>
        /// <param name="result">
        ///   <para>The result.</para>
        ///   <para>If no object is available then the default value of <typeparamref name="T"/> is returned.</para>
        /// </param>
        /// <returns>
        ///   Returns <see langword="true"/> if an object was successfully returned; otherwise returns <see langword="false"/>.
        /// </returns>
        public bool TryPeek(out T result)
        {
            Wrapper wrapper;
            while (_queue.TryPeek(out wrapper))
            {
                if (wrapper.IsExpired)
                {
                    _queue.TryDequeue(out wrapper);
                    continue;
                }
                result = wrapper.Value;
                return true;
            }
            result = default(T);
            return false;
        }

        /// <summary>
        ///   Cleans expired entries out of the queue.
        /// </summary>
        /// <remarks>
        ///   This is called automatically if <see cref="CachingQueue&lt;T&gt;.CleanAfter"/> is greater than <see cref="TimeSpan.Zero"/>.
        /// </remarks>
        public void Clean()
        {
            // Only one method should gain the lock at a time.
            // if we fail to get the lock a clean is already in progress so return immediately.
            if (!Monitor.TryEnter(_cleanLock))
                return;

            try
            {
                // Stop timer being called again.
                if (_cleanTimer != null)
                    _cleanTimer.Change(-1, -1);

                Wrapper wrapper;
                while (_queue.TryPeek(out wrapper))
                {
                    if (!wrapper.IsExpired) return;

                    _queue.TryDequeue(out wrapper);
                }
            }
            finally
            {
                // Set timer
                if (_cleanTimer != null)
                    _cleanTimer.Change((long)CleanAfter.TotalMilliseconds, -1);

                // Ensure we release the lock.
                Monitor.Exit(_cleanLock);
            }
        }

        /// <summary>
        ///   Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        ///   A <see cref="string"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return _queue.ToString();
        }

        #region Nested type: Wrapper
        /// <summary>
        ///   A wrapper for a queued element, holds information about creation <see cref="DateTime"/>.
        /// </summary>
        private struct Wrapper
        {
            /// <summary>
            ///   The value of the wrapped element.
            /// </summary>
            public readonly T Value;

            /// <summary>
            ///   The expiry date time.
            /// </summary>
            private readonly DateTime _expires;

            /// <summary>
            ///   Initializes a new instance of the <see cref="CachingQueue&lt;T&gt;.Wrapper"/> struct.
            /// </summary>
            /// <param name="value">The value of the wrapped element.</param>
            /// <param name="expires">The expiry date time.</param>
            public Wrapper(T value, DateTime expires)
                : this()
            {
                Value = value;
                _expires = expires;
            }

            /// <summary>
            ///   Returns a <see cref="bool"/> value indicating whether this instance is expired.
            /// </summary>
            public bool IsExpired
            {
                get { return DateTime.Now > _expires; }
            }

            /// <summary>
            ///   Returns a <see cref="string"/> that represents this instance.
            /// </summary>
            /// <returns>
            ///   A <see cref="string"/> representation of this instance.
            /// </returns>
            public override string ToString()
            {
                return String.Format("{0} {1}", Value, IsExpired ? " (EXPIRED)" : String.Empty);
            }
        }
        #endregion
    }
}