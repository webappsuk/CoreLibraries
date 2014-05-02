#region © Copyright Web Applications (UK) Ltd, 2014.  All rights reserved.
// Copyright (c) 2014, Web Applications UK Ltd
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
using System.Diagnostics.Contracts;
using System.Linq;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// A <see cref="Resolvable"/> object that contains a list of values.
    /// </summary>
    /// <typeparam name="TValue">The type of the t value.</typeparam>
    [Serializable]
    public class ListResolvable<TValue> : Resolvable, ICollection<TValue>
    {
        [NotNull]
        private readonly List<TValue> _values;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListResolvable{TValue}" /> class.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="resolveOuterTags">if set to <see langword="true" />  outer tags should be resolved automatically in formats.</param>
        public ListResolvable(
            [NotNull] IEnumerable<TValue> values,
            bool resolveOuterTags = true)
            : base(false, resolveOuterTags)
        {
            Contract.Requires(values != null);
            _values = new List<TValue>(values);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListResolvable{TValue}" /> class.
        /// </summary>
        /// <param name="resolveOuterTags">if set to <see langword="true" />  outer tags should be resolved automatically in formats.</param>
        /// <param name="capacity">The initial capacity.</param>
        public ListResolvable(
            bool resolveOuterTags = true,
            int capacity = 0)
            : base(false, resolveOuterTags)
        {
            _values = new List<TValue>(capacity);
        }

        /// <summary>
        /// Resolves the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>A <see cref="Resolution" />.</returns>
        // ReSharper disable once CodeAnnotationAnalyzer
        protected override Resolution Resolve(string tag)
        {
            int index;
            return int.TryParse(tag, out index) &&
                   index >= 0 &&
                   index < _values.Count
                ? new Resolution(_values[index])
                : Resolution.Unknown;
        }

        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        [PublicAPI]
        public void Add([CanBeNull] TValue value)
        {
            _values.Add(value);
        }

        /// <summary>
        /// Removes the specified tag.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><see langword="true" /> if value removed, <see langword="false" /> otherwise.</returns>
        [PublicAPI]
        public bool Remove([CanBeNull] TValue value)
        {
            return _values.Remove(value);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public void Clear()
        {
            _values.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.</returns>
        public bool Contains(TValue item)
        {
            return _values.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param><param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception><exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.</exception>
        // ReSharper disable once CodeAnnotationAnalyzer
        public void CopyTo(TValue[] array, int arrayIndex)
        {
            _values.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <value>The count.</value>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        public int Count
        {
            get { return _values.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        /// <value><see langword="true" /> if this instance is read only; otherwise, <see langword="false" />.</value>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, false.</returns>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<TValue> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    /// <summary>
    /// A <see cref="Resolvable" /> object that contains a set of dictionary values.
    /// </summary>
    [Serializable]
    public class ListResolvable : ListResolvable<object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListResolvable{TValue}" /> class.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="resolveOuterTags">if set to <see langword="true" />  outer tags should be resolved automatically in formats.</param>
        public ListResolvable(
            [NotNull] IEnumerable values,
            bool resolveOuterTags = true)
            : base(values.Cast<object>(), resolveOuterTags)
        {
            Contract.Requires(values != null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListResolvable{TValue}" /> class.
        /// </summary>
        /// <param name="resolveOuterTags">if set to <see langword="true" />  outer tags should be resolved automatically in formats.</param>
        /// <param name="capacity">The initial capacity.</param>
        public ListResolvable(
            bool resolveOuterTags = true,
            int capacity = 0)
            : base(resolveOuterTags, capacity)
        {
        }
    }
}