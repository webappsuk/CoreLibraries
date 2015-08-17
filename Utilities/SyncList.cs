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
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Provides synchronised access to a <see cref="IList{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements of the list.</typeparam>
    [PublicAPI]
    public class SyncList<T> : IList<T>, IList, IReadOnlyList<T>
    {
        [NotNull]
        private readonly IList<T> _list;

        [NotNull]
        private readonly object _syncRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncList{T}"/> class that is empty and has the default initial capacity.
        /// </summary>
        public SyncList()
        {
            _list = new List<T>();
            _syncRoot = ((ICollection)_list).SyncRoot;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncList{T}"/> class that is empty and has the specific <paramref name="capacity"/>.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public SyncList(int capacity)
        {
            _list = new List<T>(capacity);
            _syncRoot = ((ICollection)_list).SyncRoot;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncList{T}"/> class that wraps access to the specified <paramref name="list"/>, 
        /// using its <see cref="ICollection.SyncRoot"/> for synchronisation.
        /// </summary>
        /// <param name="list">The list to synchronise.</param>
        public SyncList([NotNull] IList<T> list)
        {
            _list = list;
            _syncRoot = ((ICollection)_list).SyncRoot;
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="SyncList{T}" />.
        /// </summary>
        public int Count
        {
            get { lock (_syncRoot) return _list.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="SyncList{T}" /> is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { lock (_syncRoot) return _list.IsReadOnly; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IList" /> has a fixed size.
        /// </summary>
        bool IList.IsFixedSize
        {
            get { lock (_syncRoot) return ((IList)_list).IsFixedSize; }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.
        /// </summary>
        object ICollection.SyncRoot
        {
            get { return _syncRoot; }
        }

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe).
        /// </summary>
        bool ICollection.IsSynchronized
        {
            get { return true; }
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        object IList.this[int index]
        {
            get { lock (_syncRoot) return ((IList)_list)[index]; }
            set { lock (_syncRoot) ((IList)_list)[index] = value; }
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public T this[int index]
        {
            get { lock (_syncRoot) return _list[index]; }
            set { lock (_syncRoot) _list[index] = value; }
        }

        /// <summary>
        /// Adds an item to the <see cref="SyncList{T}" />.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="SyncList{T}" />.</param>
        public void Add(T item)
        {
            lock (_syncRoot)
                _list.Add(item);
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="SyncList{T}"/>.
        /// </summary>
        /// <param name="collection">The collection whose elements should be added to the end of the <see cref="SyncList{T}"/>. 
        /// The collection itself cannot be null, but it can contain elements that are null, if type <typeparamref name="T"/> is a reference type.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public void AddRange([NotNull] IEnumerable<T> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            foreach (T item in collection)
                Add(item);
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.IList" />.
        /// </summary>
        /// <param name="value">The object to add to the <see cref="T:System.Collections.IList" />.</param>
        /// <returns>
        /// The position into which the new element was inserted, or -1 to indicate that the item was not inserted into the collection.
        /// </returns>
        int IList.Add(object value)
        {
            lock (_syncRoot)
                return ((IList)_list).Add(value);
        }

        /// <summary>
        /// Inserts an item to the <see cref="SyncList{T}" /> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="SyncList{T}" />.</param>
        public void Insert(int index, T item)
        {
            lock (_syncRoot)
                _list.Insert(index, item);
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.IList" /> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="value" /> should be inserted.</param>
        /// <param name="value">The object to insert into the <see cref="T:System.Collections.IList" />.</param>
        void IList.Insert(int index, object value)
        {
            lock (_syncRoot)
                ((IList)_list).Insert(index, value);
        }

        /// <summary>
        /// Determines whether the <see cref="SyncList{T}" /> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="SyncList{T}" />.</param>
        /// <returns>
        /// true if <paramref name="item" /> is found in the <see cref="SyncList{T}" />; otherwise, false.
        /// </returns>
        public bool Contains(T item)
        {
            lock (_syncRoot)
                return _list.Contains(item);
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.IList" /> contains a specific value.
        /// </summary>
        /// <param name="value">The object to locate in the <see cref="T:System.Collections.IList" />.</param>
        /// <returns>
        /// true if the <see cref="T:System.Object" /> is found in the <see cref="T:System.Collections.IList" />; otherwise, false.
        /// </returns>
        bool IList.Contains(object value)
        {
            lock (_syncRoot)
                return ((IList)_list).Contains(value);
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="SyncList{T}" />.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="SyncList{T}" />.</param>
        /// <returns>
        /// The index of <paramref name="item" /> if found in the list; otherwise, -1.
        /// </returns>
        public int IndexOf(T item)
        {
            lock (_syncRoot)
                return _list.IndexOf(item);
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.IList" />.
        /// </summary>
        /// <param name="value">The object to locate in the <see cref="T:System.Collections.IList" />.</param>
        /// <returns>
        /// The index of <paramref name="value" /> if found in the list; otherwise, -1.
        /// </returns>
        int IList.IndexOf(object value)
        {
            lock (_syncRoot)
                return ((IList)_list).IndexOf(value);
        }

        /// <summary>
        /// Copies the elements of the <see cref="SyncList{T}"/> to an <see cref="T:System.Array"/>,
        /// starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from 
        /// <see cref="SyncList{T}"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException">The number of elements in the source <see cref="SyncList{T}"/>
        /// is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (_syncRoot)
                _list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.ICollection" /> to an <see cref="T:System.Array" />, 
        /// starting at a particular <see cref="T:System.Array" /> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied 
        /// from <see cref="T:System.Collections.ICollection" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        void ICollection.CopyTo(Array array, int index)
        {
            lock (_syncRoot)
                ((ICollection)_list).CopyTo(array, index);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="SyncList{T}" />.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="SyncList{T}" />.</param>
        /// <returns>
        /// true if <paramref name="item" /> was successfully removed from the <see cref="SyncList{T}" />; otherwise, false. 
        /// This method also returns false if <paramref name="item" /> is not found in the original <see cref="SyncList{T}" />.
        /// </returns>
        public bool Remove(T item)
        {
            lock (_syncRoot)
                return _list.Remove(item);
        }

        /// <summary>
        /// Removes the <see cref="SyncList{T}" /> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            lock (_syncRoot)
                _list.RemoveAt(index);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.IList" />.
        /// </summary>
        /// <param name="value">The object to remove from the <see cref="T:System.Collections.IList" />.</param>
        void IList.Remove(object value)
        {
            lock (_syncRoot)
                ((IList)_list).Remove(value);
        }

        /// <summary>
        /// Removes all items from the <see cref="SyncList{T}" />.
        /// </summary>
        public void Clear()
        {
            lock (_syncRoot)
                _list.Clear();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            List<T> list;
            lock (_syncRoot)
            {
                list = new List<T>(_list.Count);
                foreach (T item in _list)
                    list.Add(item);
            }
            return list.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            List<T> list;
            lock (_syncRoot)
            {
                list = new List<T>(_list.Count);
                foreach (T item in ((IEnumerable)_list))
                    list.Add(item);
            }
            return list.GetEnumerator();
        }
    }
}