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
    /// Implements <see cref="ILookup{TKey,TElement}" />, and supports adding elements individually.
    /// </summary>
    /// <typeparam name="TKey">The type of the T key.</typeparam>
    /// <typeparam name="TElement">The type of the T element.</typeparam>
    [PublicAPI]
    public class Lookup<TKey, TElement> : IReadOnlyLookup<TKey, TElement>
    {
        /// <summary>
        /// The underlying data.
        /// </summary>
        [NotNull]
        private readonly Dictionary<TKey, List<TElement>> _data;

        private int _valuesCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="Lookup{TKey, TElement}" /> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        /// <param name="comparer">The comparer.</param>
        public Lookup(int capacity = 0, [CanBeNull] IEqualityComparer<TKey> comparer = null)
        {
            _data = new Dictionary<TKey, List<TElement>>(capacity, comparer);
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get { return _data.Count; }
        }

        /// <summary>
        /// Gets the values count.
        /// </summary>
        /// <value>The values count.</value>
        public int ValuesCount
        {
            get { return _valuesCount; }
        }

        /// <summary>
        /// Gets the <see cref="IEnumerable{T}" /> with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>IEnumerable{.</returns>
        public IEnumerable<TElement> this[[NotNull] TKey key]
        {
            get
            {
                if (key == null) throw new ArgumentNullException("key");

                List<TElement> value;
                // ReSharper disable once AssignNullToNotNullAttribute
                return _data.TryGetValue(key, out value)
                    ? value
                    : Enumerable.Empty<TElement>();
            }
        }

        /// <summary>
        /// Tries to get the values with the given key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="values">The values.</param>
        /// <returns><see langword="true" /> if the key was found, <see langword="false" /> otherwise.</returns>
        public bool TryGetValues(TKey key, out IEnumerable<TElement> values)
        {
            List<TElement> list;
            if (!_data.TryGetValue(key, out list))
            {
                values = Enumerable.Empty<TElement>();
                return false;
            }
            values = list;
            return true;
        }

        /// <summary>
        /// Determines whether [contains] [the specified key].
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///     <see langword="true" /> if [contains] [the specified key]; otherwise, <see langword="false" />.
        /// </returns>
        public bool Contains(TKey key)
        {
            return _data.ContainsKey(key);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
        {
            return _data.Select(kvp => (IGrouping<TKey, TElement>)new Grouping<TKey, TElement>(kvp)).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds the specified key value pair.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="element">The element.</param>
        public void Add([NotNull] TKey key, TElement element)
        {
            if (key == null) throw new ArgumentNullException("key");

            List<TElement> list;
            if (!_data.TryGetValue(key, out list))
                _data[key] = list = new List<TElement>();
            _valuesCount++;
            // ReSharper disable once PossibleNullReferenceException
            list.Add(element);
        }

        /// <summary>
        /// Adds the specified key value pair.
        /// </summary>
        /// <param name="keyValuePair">The key value pair.</param>
        public void Add(KeyValuePair<TKey, TElement> keyValuePair)
        {
            // ReSharper disable once AssignNullToNotNullAttribute - Let Add throw
            Add(keyValuePair.Key, keyValuePair.Value);
        }

        /// <summary>
        /// Adds the specified values.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="elements">The elements.</param>
        public void AddRange([NotNull] TKey key, [InstantHandle] [NotNull] IEnumerable<TElement> elements)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (elements == null) throw new ArgumentNullException("elements");

            List<TElement> list;
            if (!_data.TryGetValue(key, out list))
                _data[key] = list = new List<TElement>();
            _valuesCount++;
            // ReSharper disable once PossibleNullReferenceException
            list.AddRange(elements);
        }

        /// <summary>
        /// Adds the specified values.
        /// </summary>
        /// <param name="elements">The elements.</param>
        public void AddRange([InstantHandle] [NotNull] IEnumerable<KeyValuePair<TKey, TElement>> elements)
        {
            if (elements == null) throw new ArgumentNullException("elements");
            foreach (KeyValuePair<TKey, TElement> kvp in elements)
                Add(kvp);
        }

        /// <summary>
        /// Removes all elements corresponding to a given key.
        /// </summary>
        /// <param name="key">The key</param>
        public bool Remove([NotNull] TKey key)
        {
            List<TElement> list;
            if (!_data.TryGetValue(key, out list))
                return false;

            // ReSharper disable once PossibleNullReferenceException
            _valuesCount -= list.Count;
            _data.Remove(key);
            return true;
        }

        /// <summary>
        /// Removes the first occurrence of a specified element corresponding to a given key.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="element">The element</param>
        public bool Remove([NotNull] TKey key, [NotNull] TElement element)
        {
            List<TElement> list;
            if (!_data.TryGetValue(key, out list) ||
                // ReSharper disable once PossibleNullReferenceException
                !list.Remove(element))
                return false;

            _valuesCount--;

            if (list.Count == 0)
                _data.Remove(key);

            return true;
        }

        /// <summary>
        /// Removes all the keys and values from the lookup.
        /// </summary>
        public void Clear()
        {
            _data.Clear();
        }
    }
}