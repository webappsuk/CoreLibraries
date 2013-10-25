using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Implements a lookup where the order of keys is maintained.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TElement">The element type.</typeparam>
    public class OrderedLookup<TKey, TElement> : IOrderedLookup<TKey, TElement>
    {
        /// <summary>
        /// The underlying keys mapped to the list index.
        /// </summary>
        [NotNull] private readonly Dictionary<TKey, int> _keys;

        /// <summary>
        /// The values in order.
        /// </summary>
        [NotNull]
        private readonly List<KeyValuePair<TKey, List<TElement>>> _values;

        private int _valuesCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="Lookup{TKey, TElement}" /> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        /// <param name="comparer">The comparer.</param>
        public OrderedLookup(int capacity = 0, IEqualityComparer<TKey> comparer = null)
        {
            _keys = new Dictionary<TKey, int>(capacity, comparer);
            _values = new List<KeyValuePair<TKey, List<TElement>>>(capacity);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
        {
            return _values
                .Select(kvp => (IGrouping<TKey, TElement>)new Grouping<TKey, TElement>(kvp))
                .GetEnumerator();
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
        /// Determines whether [contains] [the specified key].
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///     <see langword="true" /> if [contains] [the specified key]; otherwise, <see langword="false" />.
        /// </returns>
        public bool Contains(TKey key)
        {
            return _keys.ContainsKey(key);
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get { return _keys.Count; }
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
        /// Gets the <see cref="IEnumerable{TElement}" /> with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>IEnumerable{.</returns>
        public IEnumerable<TElement> this[TKey key]
        {
            get
            {
                int index;
                // ReSharper disable once AssignNullToNotNullAttribute
                return _keys.TryGetValue(key, out index)
                    ? _values[index].Value
                    : Enumerable.Empty<TElement>();
            }
        }

        /// <summary>
        /// Gets the <see cref="IEnumerable{TElement}"/> with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>IEnumerable{.</returns>
        public IEnumerable<TElement> this[int key]
        {
            get
            {
                return key < _values.Count
                    ? _values[key].Value
                    : Enumerable.Empty<TElement>();
            }
        }

        /// <summary>
        /// Adds the specified key value pair.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="element">The element.</param>
        public void Add(TKey key, TElement element)
        {
            int index;
            List<TElement> list;
            if (_keys.TryGetValue(key, out index))
                list = _values[index].Value;
            else
            {
                list = new List<TElement>();
                _keys.Add(key, _values.Count);
                _values.Add(new KeyValuePair<TKey, List<TElement>>(key, list));
            }
            _valuesCount++;
            list.Add(element);
        }

        /// <summary>
        /// Adds the specified key value pair.
        /// </summary>
        /// <param name="keyValuePair">The key value pair.</param>
        public void Add(KeyValuePair<TKey, TElement> keyValuePair)
        {
            int index;
            List<TElement> list;
            if (_keys.TryGetValue(keyValuePair.Key, out index))
                list = _values[index].Value;
            else
            {
                list = new List<TElement>();
                _keys.Add(keyValuePair.Key, _values.Count);
                _values.Add(new KeyValuePair<TKey, List<TElement>>(keyValuePair.Key, list));
            }
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
            int index;
            List<TElement> list;
            if (_keys.TryGetValue(key, out index))
                list = _values[index].Value;
            else
            {
                list = new List<TElement>();
                _keys.Add(key, _values.Count);
                _values.Add(new KeyValuePair<TKey, List<TElement>>(key, list));
            }
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
    }
}