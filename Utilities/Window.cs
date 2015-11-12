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
    /// Holds a window on existing data.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <seealso cref="IList{T}"/>
    /// <seealso cref="IList"/>
    /// <seealso cref="ReadOnlyWindow{T}"/>
    [PublicAPI]
    [Serializable]
    public class Window<T> : IList<T>, IList
    {
        /// <summary>
        /// Gets the underlying data.
        /// </summary>
        /// <value>The data.</value>
        [NotNull]
        protected readonly IList<T> Data;

        /// <summary>
        /// Whether you can expand the window using <see cref="GetSubset"/>.
        /// </summary>
        public readonly bool AllowExpansion;

        /// <summary>
        /// The offset.
        /// </summary>
        public readonly int Offset;

        /// <summary>
        /// Gets the length of the underlying data.
        /// </summary>
        /// <value>The length.</value>
        public int Length => Data.Count;

        /// <summary>
        /// The delta between the data count and the window count (allows the window to change with the underlying data).
        /// </summary>
        private readonly int _delta;

        /// <summary>
        /// Initializes a new instance of the <see cref="Window{T}" /> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="offset">The offset (defaults to zero).</param>
        /// <param name="length">The length (defaults to the data length - the offset).</param>
        /// <param name="allowExpansion"><see langword="true"/> to allow expansion; <see langword="false"/> otherwise.</param>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="offset" /> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="length" /> is out of range.</exception>
        /// <exception cref="ArgumentException">The data must not be readonly - consider using <see cref="ReadOnlyWindow{T}"/> instead.</exception>
        public Window([NotNull] IList<T> data, int offset = 0, int length = -1, bool allowExpansion = false)
        {
            if (data.IsReadOnly)
                throw new ArgumentException(Resources.Window_Window_ReadOnly_Data, nameof(data));
            Data = data;
            int l = Data.Count;


            if (offset < 0 || offset > l)
                throw new ArgumentOutOfRangeException(nameof(offset));

            if (length < 0)
                length = l - offset;
            else if (length + offset > l)
                throw new ArgumentOutOfRangeException(nameof(length));

            Offset = offset;
            _delta = l - length;
            AllowExpansion = allowExpansion;
        }

        /// <inheritdoc />
        public void Add(T item)
        {
            if (_delta < 1 || Offset >= Data.Count)
                Data.Add(item);
            else
                Data.Insert(Data.Count - _delta, item);
        }

        /// <inheritdoc />
        int IList.Add(object value)
        {
            if (!(value is T)) return -1;

            if (_delta < 1 || Offset >= Data.Count)
                Data.Add((T)value);
            else
                Data.Insert(Data.Count - _delta, (T)value);

            return Data.Count <= Offset ? -1 : Data.Count - Offset - 1;
        }

        /// <inheritdoc />
        public void Clear()
        {
            if (Offset == 0 && _delta < 1)
            {
                Data.Clear();
                return;
            }

            while (Data.Count - _delta > 0)
                Data.RemoveAt(Offset);
        }

        /// <inheritdoc />
        public bool Contains(T item)
        {
            if (Offset == 0 && _delta < 1)
                return Data.Contains(item);

            return IndexOf(item) > -1;
        }

        /// <inheritdoc />
        bool IList.Contains(object value) => value is T && Contains((T)value);

        /// <inheritdoc />
        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = Offset; i < Data.Count - _delta + Offset; i++)
                array[arrayIndex++] = Data[i];
        }

        /// <inheritdoc />
        void ICollection.CopyTo(Array array, int index)
        {
            for (int i = Offset; i < Data.Count - _delta + Offset; i++)
                array.SetValue(Data[i], index++);
        }

        /// <inheritdoc />
        public bool Remove(T item)
        {
            if (Offset == 0 && _delta < 1)
                return Data.Remove(item);

            int index = IndexOf(item);
            if (index < 0) return false;
            RemoveAt(index);
            return true;
        }

        /// <inheritdoc />
        void IList.Remove(object value)
        {
            if (value is T)
                Remove((T)value);
        }

        /// <inheritdoc />
        public int Count => _delta > Data.Count ? 0 : Data.Count - _delta;

        /// <inheritdoc />
        object ICollection.SyncRoot
        {
            get
            {
                ICollection collection = Data as ICollection;
                return collection?.SyncRoot ?? Data;
            }
        }

        /// <inheritdoc />
        bool ICollection.IsSynchronized
        {
            get
            {
                ICollection collection = Data as ICollection;
                return collection?.IsSynchronized ?? false;
            }
        }

        /// <inheritdoc />
        public bool IsReadOnly => Data.IsReadOnly;

        /// <inheritdoc />
        bool IList.IsFixedSize
        {
            get
            {
                IList list = Data as IList;
                return list?.IsFixedSize ?? false;
            }
        }

        /// <inheritdoc />
        public int IndexOf(T item)
        {
            int index = Data.IndexOf(item) - Offset;
            if (index < 0 || index > Data.Count - _delta - 1)
                index = -1;
            return index;
        }

        /// <inheritdoc />
        int IList.IndexOf(object value) => value is T ? IndexOf((T)value) : -1;

        /// <inheritdoc />
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="index"/> is out of range.</exception>
        public void Insert(int index, T item)
        {
            if (index < 0 || index >= Data.Count - _delta)
                throw new ArgumentOutOfRangeException(nameof(index));
            Data.Insert(Offset + index, item);
        }

        /// <inheritdoc />
        void IList.Insert(int index, object value) => Insert(index, (T)value);

        /// <inheritdoc />
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="index"/> is out of range.</exception>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= Data.Count - _delta)
                throw new ArgumentOutOfRangeException(nameof(index));

            Data.RemoveAt(Offset + index);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentOutOfRangeException" accessor="get">The <paramref name="index"/> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException" accessor="set">The <paramref name="index"/> is out of range.</exception>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Data.Count - _delta)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return Data[index + Offset];
            }
            set
            {
                if (index < 0 || index >= Data.Count - _delta)
                    throw new ArgumentOutOfRangeException(nameof(index));
                Data[index + Offset] = value;
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentOutOfRangeException" accessor="get">The <paramref name="index"/> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException" accessor="set">The <paramref name="index"/> is out of range.</exception>
        object IList.this[int index]
        {
            get
            {
                if (index < 0 || index >= Data.Count - _delta)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return Data[index + Offset];
            }
            set
            {
                if (index < 0 || index >= Data.Count - _delta)
                    throw new ArgumentOutOfRangeException(nameof(index));
                Data[index + Offset] = (T)value;
            }
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = Offset; i < Data.Count - _delta + Offset; i++)
                yield return Data[i];
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = Offset; i < Data.Count - _delta + Offset; i++)
                yield return Data[i];
        }

        /// <summary>
        /// Gets a subset of the data.
        /// </summary>
        /// <param name="offset">The offset (can be negative to expand/move window).</param>
        /// <param name="length">The length.</param>
        /// <returns>A subset <see cref="Window{T}"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="offset" /> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="length" /> is out of range.</exception>
        [NotNull]
        public Window<T> GetSubset(int offset, int length)
        {
            if (offset == 0 && length == Data.Count - _delta) return this;

            if (AllowExpansion)
            {
                if (offset < -Offset)
                    throw new ArgumentOutOfRangeException(nameof(offset));
                if (length > Data.Count - offset)
                    throw new ArgumentOutOfRangeException(nameof(length));
            }
            else
            {
                if (offset < 0)
                    throw new ArgumentOutOfRangeException(nameof(offset));
                if (length > Data.Count - _delta)
                    throw new ArgumentOutOfRangeException(nameof(length));
            }

            return new Window<T>(Data, Offset + offset, length, AllowExpansion);
        }

        /// <summary>
        /// Implements the implicit cast operator to <see cref="ReadOnlyWindow{T}"/>.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <returns>A <see cref="ReadOnlyWindow{T}"/>.</returns>
        [ContractAnnotation("window:null=>null;window:notnull=>notnull")]
        public static implicit operator ReadOnlyWindow<T>(Window<T> window)
            => window != null
                ? new ReadOnlyWindow<T>(
                    window.Data as IReadOnlyList<T> ?? window.Data.ToArray(),
                    window.Offset,
                    window.Count,
                    window.AllowExpansion)
                : null;

        /// <summary>
        /// Implements the implicit cast operator from <see cref="List{T}" />.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns>A <see cref="Window{T}" />.</returns>
        [ContractAnnotation("list:null=>null;list:notnull=>notnull")]
        public static implicit operator Window<T>(List<T> list)
            => list != null
            ? new Window<T>(list)
            : null;

        /// <summary>
        /// Implements the implicit cast operator from an array of <typeparamref name="T"/>.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns>A <see cref="Window{T}" />.</returns>
        [ContractAnnotation("list:null=>null;list:notnull=>notnull")]
        public static implicit operator Window<T>(T[] array)
            => array != null
            ? new Window<T>(array)
            : null;
    }
}