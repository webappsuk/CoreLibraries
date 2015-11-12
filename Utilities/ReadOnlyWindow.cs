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
    /// Holds a window on existing data.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    [PublicAPI]
    public class ReadOnlyWindow<T> : IReadOnlyList<T>, ICollection
    {
        /// <summary>
        /// The data to be compared.
        /// </summary>
        [NotNull]
        protected readonly IReadOnlyList<T> Data;

        /// <summary>
        /// Whether you can expand the window using <see cref="GetSubset"/>.
        /// </summary>
        public readonly bool AllowExpansion;

        /// <summary>
        /// The offset.
        /// </summary>
        public readonly int Offset;

        /// <summary>
        /// The delta between the data count and the window count (allows the window to change with the underlying data).
        /// </summary>
        private readonly int _delta;

        /// <summary>
        /// Gets the length of the underlying data.
        /// </summary>
        /// <value>The length.</value>
        public int Length => Data.Count;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyWindow{T}" /> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="offset">The offset (defaults to zero).</param>
        /// <param name="length">The length (defaults to the data length - the offset).</param>
        /// <param name="allowExpansion"><see langword="true"/> to allow expansion; <see langword="false"/> otherwise.</param>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="offset" /> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="length" /> is out of range.</exception>
        public ReadOnlyWindow([NotNull] IReadOnlyList<T> data, int offset = 0, int length = -1, bool allowExpansion = false)
        {
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

        /// <summary>
        /// Gets the <see cref="int" /> with the <paramref name="index">specified index</paramref>.
        /// </summary>
        /// <returns>An <see cref="int"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="index"/> is out of range.</exception>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Data.Count - _delta + Offset)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return Data[index + Offset];
            }
        }

        /// <inheritdoc />
        void ICollection.CopyTo(Array array, int index)
        {
            for (int i = Offset; i < Data.Count - _delta + Offset; i++)
                array.SetValue(Data[i], index++);
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
        /// <returns>A subset <see cref="ReadOnlyWindow{T}"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="offset" /> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="length" /> is out of range.</exception>
        [NotNull]
        public ReadOnlyWindow<T> GetSubset(int offset, int length)
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

            return new ReadOnlyWindow<T>(Data, Offset + offset, length, AllowExpansion);
        }

        /// <summary>
        /// Implements the implicit cast operator from <see cref="List{T}" />.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns>A <see cref="ReadOnlyWindow{T}" />.</returns>
        [ContractAnnotation("list:null=>null;list:notnull=>notnull")]
        public static implicit operator ReadOnlyWindow<T>(List<T> list)
            => list != null
            ? new ReadOnlyWindow<T>(list)
            : null;

        /// <summary>
        /// Implements the implicit cast operator from an array of <typeparamref name="T"/>.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns>A <see cref="ReadOnlyWindow{T}" />.</returns>
        [ContractAnnotation("list:null=>null;list:notnull=>notnull")]
        public static implicit operator ReadOnlyWindow<T>(T[] array)
            => array != null
            ? new ReadOnlyWindow<T>(array)
            : null;
    }
}