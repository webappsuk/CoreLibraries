using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Implements <see cref="IGrouping{TKey, TElement}" />.
    /// </summary>
    internal class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        /// <summary>
        /// The key value pair.
        /// </summary>
        private KeyValuePair<TKey, List<TElement>> _keyValuePair;

        /// <summary>
        /// Initializes a new instance of the <see cref="Grouping" /> class.
        /// </summary>
        /// <param name="keyValuePair">The key value pair.</param>
        public Grouping(KeyValuePair<TKey, List<TElement>> keyValuePair)
        {
            _keyValuePair = keyValuePair;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Grouping" /> class.
        /// </summary>
        /// <param name="keyValuePair">The key value pair.</param>
        public Grouping(TKey key, List<TElement> value)
        {
            _keyValuePair = new KeyValuePair<TKey, List<TElement>>(key, value);
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>The key.</value>
        public TKey Key
        {
            get { return _keyValuePair.Key; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<TElement> GetEnumerator()
        {
            return _keyValuePair.Value.GetEnumerator();
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
    }
}
