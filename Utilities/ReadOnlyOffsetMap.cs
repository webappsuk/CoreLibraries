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
using System.Linq;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Holds an ordered collection of mappings to a single piece of underlying data.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <seealso cref="ReadOnlyMap{T}"/>
    /// <seealso cref="IReadOnlyList{T}"/>
    /// <seealso cref="ICollection"/>
    /// <seealso cref="ReadOnlyWindow{T}"/>
    /// <remarks>
    /// <para>The <see cref="ReadOnlyOffsetMap{T}"/> extends a <see cref="List{T}"/> of <see cref="Mapping"/>, and
    /// so can be initialized and manipulated as a list.  When in use it can be cast to a <see cref="IReadOnlyList{T}"/>
    /// of <typeparamref name="T"/> or an <see cref="ICollection"/>, at which point it acts like a single collection
    /// mapping requests for calls onto the underlying data.</para>
    /// <para>The <see cref="ReadOnlyMap{T}"/> can be used where you wish to map onto multiple data sources.</para>
    /// </remarks>
    [Serializable]
    [PublicAPI]
    public class ReadOnlyOffsetMap<T> : List<Mapping>, IReadOnlyList<T>, ICollection
    {
        /// <summary>
        /// The underlying data.
        /// </summary>
        [NotNull]
        public readonly IReadOnlyList<T> Data;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyOffsetMap{T}" /> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="capacity">The capacity.</param>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null" />.</exception>
        public ReadOnlyOffsetMap([NotNull] IReadOnlyList<T> data, int capacity = 0)
            : base(capacity)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            Data = data;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyOffsetMap{T}" /> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="mappings">The initial mappings.</param>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="mappings"/> is <see langword="null" />.</exception>
        public ReadOnlyOffsetMap([NotNull] IReadOnlyList<T> data, [NotNull] IEnumerable<Mapping> mappings)
            :base(mappings)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            Data = data;
        }

        /// <inheritdoc/>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            foreach (Mapping mapping in this)
            {
                if (mapping.Length < 1) continue;

                for (int i = mapping.Offset; i < mapping.Offset + mapping.Length; i++)
                    yield return Data[i];
            }
        }
        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();

        /// <summary>
        /// Creates and then adds a new <see cref="Mapping"/>.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <returns>A new instance of a <see cref="Mapping"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/> is less than zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> is less than zero.</exception>
        public Mapping Add(int offset, int length)
        {
            Mapping mapping = new Mapping(offset, length);
            Add(mapping);
            return mapping;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="item.Offset"/> is less than zero or more than the count of the underlying data.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="item.Length"/> is less than zero, or exceeds the bounds of the underlying data when combined with the <paramref name="item.Offset"/>.</exception>
        public new void Add(Mapping item)
        {
            if (item.Offset > Data.Count)
                throw new ArgumentOutOfRangeException(nameof(item.Offset));
            if (item.Length + item.Offset > Data.Count)
                throw new ArgumentOutOfRangeException(nameof(item.Length));
            base.Add(item);
        }

        /// <summary>
        /// Creates and then inserts a new <see cref="Mapping"/> at the <paramref name="index">specified index</paramref>.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <returns>A new instance of a <see cref="Mapping"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/> is less than zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> is less than zero.</exception>
        public Mapping Insert(int index, int offset, int length)
        {
            Mapping mapping = new Mapping(offset, length);
            Insert(index, mapping);
            return mapping;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="item.Offset"/> is less than zero or more than the count of the underlying data.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="item.Length"/> is less than zero, or exceeds the bounds of the underlying data when combined with the <paramref name="item.Offset"/>.</exception>
        public new void Insert(int index, Mapping item)
        {
            if (index < 0 || index > base.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (item.Offset > Data.Count)
                throw new ArgumentOutOfRangeException(nameof(item.Offset));
            if (item.Length +item.Offset > Data.Count)
                throw new ArgumentOutOfRangeException(nameof(item.Length));

            base.Insert(index, item);
        }
        
        /// <inheritdoc/>
        void ICollection.CopyTo(Array array, int index)
        {
            foreach (T item in ((IEnumerable<T>)this))
                array.SetValue(item, index++);
        }
        
        /// <inheritdoc/>
        object ICollection.SyncRoot => (Data as ICollection)?.SyncRoot ?? Data;

        /// <inheritdoc/>
        bool ICollection.IsSynchronized => false;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <inheritdoc/>
        int ICollection.Count => ((IEnumerable<Mapping>)this).Sum(m => m.Length);

        /// <inheritdoc/>
        int IReadOnlyCollection<T>.Count => ((IEnumerable<Mapping>)this).Sum(m => m.Length);

        /// <inheritdoc/>
        T IReadOnlyList<T>.this[int index]
        {
            get
            {
                if (index < 0)
                    throw new IndexOutOfRangeException(nameof(index));
                foreach (Mapping mapping in this)
                {
                    if (index < mapping.Length) return Data[mapping.Offset + index];
                    index -= mapping.Length;
                }
                throw new IndexOutOfRangeException(nameof(index));
            }
        }

        /// <summary>
        /// Maps the specified index to the underlying data index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The index in the underlying data.</returns>
        /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is out of range.</exception>
        public int MapIndex(int index)
        {
            if (index < 0)
                throw new IndexOutOfRangeException(nameof(index));
            foreach (Mapping mapping in this)
            {
                if (index < mapping.Length) return mapping.Offset + index;
                index -= mapping.Length;
            }
            throw new IndexOutOfRangeException(nameof(index));
        }
    }
}