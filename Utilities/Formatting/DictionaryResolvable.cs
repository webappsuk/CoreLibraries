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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// A <see cref="Resolvable"/> object that contains a set of dictionary values.
    /// </summary>
    /// <typeparam name="TValue">The type of the t value.</typeparam>
    public class DictionaryResolvable<TValue> : Resolvable, ICollection<KeyValuePair<string, TValue>>
    {
        [NotNull]
        private readonly ConcurrentDictionary<string, TValue> _values;

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryResolvable{TValue}"/> class.
        /// </summary>
        /// <param name="isCaseSensitive">if set to <see langword="true" /> then tags are case sensitive.</param>
        /// <param name="resolveOuterTags">if set to <see langword="true" />  outer tags should be resolved automatically in formats.</param>
        /// <param name="capacity">The initial capacity.</param>
        /// <param name="concurrency">The concurrency level.</param>
        public DictionaryResolvable(
            bool isCaseSensitive = false,
            bool resolveOuterTags = true,
            int capacity = -1,
            int concurrency = -1)
            : base(isCaseSensitive, resolveOuterTags)
        {
            if (concurrency < 1) concurrency = Environment.ProcessorCount * 4;
            if (capacity < 0) capacity = 31;
            _values = new ConcurrentDictionary<string, TValue>(
                concurrency,
                capacity,
                isCaseSensitive ? StringComparer.CurrentCulture : StringComparer.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Adds the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="value">The value.</param>
        [PublicAPI]
        public void Add([NotNull] string tag, [CanBeNull] TValue value)
        {
            Contract.Requires(tag != null);
            _values.AddOrUpdate(tag, value, (t, e) => value);
        }

        /// <summary>
        /// Removes the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        [PublicAPI]
        public Optional<object> Remove([NotNull] string tag)
        {
            Contract.Requires(tag != null);
            TValue value;
            return _values.TryRemove(tag, out value)
                ? new Optional<object>(value)
                : Optional<object>.Unassigned;
        }

        /// <summary>
        /// Resolves the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>Optional&lt;System.Object&gt;.</returns>
        // ReSharper disable once CodeAnnotationAnalyzer
        public override Optional<object> Resolve(string tag)
        {
            TValue value;
            return _values.TryGetValue(tag, out value)
                ? new Optional<object>(value)
                : Optional<object>.Unassigned;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
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

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        public void Add(KeyValuePair<string, TValue> item)
        {
            Contract.Requires(item.Key != null);
            // ReSharper disable once AssignNullToNotNullAttribute
            Add(item.Key, item.Value);
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
        bool ICollection<KeyValuePair<string, TValue>>.Contains(KeyValuePair<string, TValue> item)
        {
            return ((ICollection<KeyValuePair<string, TValue>>) _values).Contains(item);
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        void ICollection<KeyValuePair<string, TValue>>.CopyTo(KeyValuePair<string, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, TValue>>) _values).CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        bool ICollection<KeyValuePair<string, TValue>>.Remove(KeyValuePair<string, TValue> item)
        {
            return ((ICollection<KeyValuePair<string, TValue>>) _values).Remove(item);
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
    }

    /// <summary>
    /// A <see cref="Resolvable" /> object that contains a set of dictionary values.
    /// </summary>
    public class DictionaryResolvable : DictionaryResolvable<object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryResolvable{TValue}"/> class.
        /// </summary>
        /// <param name="isCaseSensitive">if set to <see langword="true" /> then tags are case sensitive.</param>
        /// <param name="resolveOuterTags">if set to <see langword="true" />  outer tags should be resolved automatically in formats.</param>
        /// <param name="capacity">The initial capacity.</param>
        /// <param name="concurrency">The concurrency level.</param>
        public DictionaryResolvable(
            bool isCaseSensitive = false,
            bool resolveOuterTags = true,
            int capacity = -1,
            int concurrency = -1)
            : base(isCaseSensitive, resolveOuterTags, capacity, concurrency)
        {
        }
    }
}