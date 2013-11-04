﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Implements <see cref="ILookup{TKey, TElement}" />, and supports adding elements individually.
    /// </summary>
    /// <typeparam name="TKey">The type of the T key.</typeparam>
    /// <typeparam name="TElement">The type of the T element.</typeparam>
    public class Lookup<TKey, TElement> : ILookup<TKey, TElement>
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
        public Lookup(int capacity = 0, IEqualityComparer<TKey> comparer = null)
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
        /// Gets the <see cref="IEnumerable{" /> with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>IEnumerable{.</returns>
        public IEnumerable<TElement> this[TKey key]
        {
            get
            {
                List<TElement> value;
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
        public void Add(TKey key, TElement element)
        {
            List<TElement> list;
            if (!_data.TryGetValue(key, out list))
                _data[key] = list = new List<TElement>();
            _valuesCount++;
            list.Add(element);
        }

        /// <summary>
        /// Adds the specified key value pair.
        /// </summary>
        /// <param name="keyValuePair">The key value pair.</param>
        public void Add(KeyValuePair<TKey, TElement> keyValuePair)
        {
            List<TElement> list;
            if (!_data.TryGetValue(keyValuePair.Key, out list))
                _data[keyValuePair.Key] = list = new List<TElement>();
            _valuesCount++;
            list.Add(keyValuePair.Value);
        }

        /// <summary>
        /// Adds the specified values.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="elements">The elements.</param>
        public void AddRange(TKey key, [NotNull] IEnumerable<TElement> elements)
        {
            List<TElement> list;
            if (!_data.TryGetValue(key, out list))
                _data[key] = list = new List<TElement>();
            _valuesCount++;
            list.AddRange(elements);
        }

        /// <summary>
        /// Adds the specified values.
        /// </summary>
        /// <param name="elements">The elements.</param>
        public void AddRange(IEnumerable<KeyValuePair<TKey, TElement>> elements)
        {
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
                !list.Remove(element))
                return false;

            _valuesCount--;

            if (list.Count == 0)
                _data.Remove(key);

            return true;
        }
    }
}
