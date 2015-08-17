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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Threading
{
    /// <summary>
    /// A collection that stores its values in individual thread local collections. This collection is best suited for multiple concurrent additions.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    /// <remarks>The <see cref="Add"/> and <see cref="AddRange"/> methods are thread safe, however any other methods are not guaranteed to be thread safe.
    /// After calling <see cref="Dispose"/>, the collection becomes read-only.</remarks>
    [PublicAPI]
    public class ThreadLocalCollection<T> : ICollection<T>, IReadOnlyCollection<T>, IDisposable
    {
        [CanBeNull]
        private ThreadLocal<List<T>> _local = new ThreadLocal<List<T>>(() => new List<T>(), true);

        [CanBeNull]
        private T[] _values;

        /// <summary>
        /// Adds an item to the <see cref="ThreadLocalCollection{T}"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="ThreadLocalCollection{T}"/>.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="ThreadLocalCollection{T}"/> is read-only.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="ThreadLocalCollection{T}"/> was disposed by another thread while executing the method.</exception>
        public void Add(T item)
        {
            ThreadLocal<List<T>> local = _local;
            if (local == null)
                throw new NotSupportedException("The collection is read-only.");

            Debug.Assert(local.Value != null);
            local.Value.Add(item);
        }

        /// <summary>
        /// Adds a collection of elements to the <see cref="ThreadLocalCollection{T}"/>
        /// </summary>
        /// <param name="items">The collection of items to add to the collection.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="ThreadLocalCollection{T}"/> is read-only.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="ThreadLocalCollection{T}"/> was disposed by another thread while executing the method.</exception>
        public void AddRange([NotNull] IEnumerable<T> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            ThreadLocal<List<T>> local = _local;
            if (local == null)
                throw new NotSupportedException("The collection is read-only.");

            Debug.Assert(local.Value != null);
            local.Value.AddRange(items);
        }

        /// <summary>
        /// Removes all items from the <see cref="ThreadLocalCollection{T}"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="ThreadLocalCollection{T}"/> is read-only. </exception>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="ThreadLocalCollection{T}"/> was disposed by another thread while executing the method.</exception>
        public void Clear()
        {
            ThreadLocal<List<T>> local = _local;
            if (local == null)
                throw new NotSupportedException("The collection is read-only.");

            Debug.Assert(local.Values != null);
            foreach (List<T> list in local.Values)
            {
                Debug.Assert(list != null);
                list.Clear();
            }
        }

        /// <summary>
        /// Determines whether the <see cref="ThreadLocalCollection{T}"/> contains a specific value.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="ThreadLocalCollection{T}"/>; otherwise, false.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="ThreadLocalCollection{T}"/>.</param>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="ThreadLocalCollection{T}"/> was disposed by another thread while executing the method.</exception>
        public bool Contains(T item)
        {
            ThreadLocal<List<T>> local = _local;
            if (local == null)
            {
                Debug.Assert(_values != null);
                return _values.Contains(item);
            }

            Debug.Assert(local.Values != null);
            foreach (List<T> list in local.Values)
            {
                Debug.Assert(list != null);
                if (list.Contains(item)) return true;
            }
            return false;
        }

        /// <summary>
        /// Copies the elements of the <see cref="ThreadLocalCollection{T}"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="ThreadLocalCollection{T}"/>. 
        /// The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException">The number of elements in the source <see cref="ThreadLocalCollection{T}"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="ThreadLocalCollection{T}"/> was disposed by another thread while executing the method.</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            ThreadLocal<List<T>> local = _local;
            if (local == null)
            {
                Debug.Assert(_values != null);
                _values.CopyTo(array, arrayIndex);
                return;
            }

            Debug.Assert(local.Values != null);
            foreach (List<T> list in local.Values)
            {
                Debug.Assert(list != null);

                list.CopyTo(array, arrayIndex);
                arrayIndex += list.Count;
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. 
        /// This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="ThreadLocalCollection{T}"/> was disposed by another thread while executing the method.</exception>
        public bool Remove(T item)
        {
            ThreadLocal<List<T>> local = _local;
            if (local == null)
                throw new NotSupportedException("The collection is read-only.");

            Debug.Assert(local.Values != null);
            foreach (List<T> list in local.Values)
            {
                Debug.Assert(list != null);
                if (list.Remove(item))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ThreadLocalCollection{T}"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="ThreadLocalCollection{T}"/>.
        /// </returns>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="ThreadLocalCollection{T}"/> was disposed by another thread while executing the method.</exception>
        // ReSharper disable once AssignNullToNotNullAttribute
        // ReSharper disable PossibleNullReferenceException
        public int Count => _local?.Values.Sum(l => l.Count) ?? _values.Length;

        // ReSharper restore PossibleNullReferenceException

        /// <summary>
        /// Gets a value indicating whether the <see cref="ThreadLocalCollection{T}"/> is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="ThreadLocalCollection{T}"/> is read-only; otherwise, false.
        /// </returns>
        public bool IsReadOnly => _local == null;

        /// <summary>
        /// Copies the elements of the <see cref="ThreadLocalCollection{T}"/> to a new array.
        /// </summary>
        /// <returns>
        /// An array containing copies of the elements of the <see cref="ThreadLocalCollection{T}"/>.
        /// </returns>
        public T[] ToArray()
        {
            ThreadLocal<List<T>> local = _local;
            if (local == null)
            {
                Debug.Assert(_values != null);
                return (T[])_values.Clone();
            }

            T[] array = new T[Count];
            int i = 0;
            Debug.Assert(local.Values != null);
            foreach (List<T> list in local.Values)
            {
                Debug.Assert(list != null);

                list.CopyTo(array, i);
                i += list.Count;
            }
            Debug.Assert(i == array.Length);

            return array;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="ThreadLocalCollection{T}"/> was disposed by another thread while executing the method.</exception>
        public IEnumerator<T> GetEnumerator()
        {
            ThreadLocal<List<T>> local = _local;
            if (local == null)
            {
                Debug.Assert(_values != null);
                // ReSharper disable once PossibleNullReferenceException
                return _values.AsEnumerable().GetEnumerator();
            }

            Debug.Assert(local.Values != null);
            return local.Values.SelectMany(l => l).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="ThreadLocalCollection{T}"/> was disposed by another thread while executing the method.</exception>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            ThreadLocal<List<T>> local = _local;
            if (local == null)
                return;

            if (_values != null)
                return;

            Debug.Assert(local.Values != null);
            T[] values = local.Values.SelectMany(l => l).ToArray();

            if (_values != null)
                return;

            _values = values;

            _local = null;
            local.Dispose();
        }
    }
}