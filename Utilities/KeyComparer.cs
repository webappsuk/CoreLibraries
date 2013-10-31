using System.Collections.Generic;
using JetBrains.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Equality comparer for <see cref="KeyValuePair{TKey, TValue}"/> that only considers keys.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class KeyComparer<TKey, TValue> : IEqualityComparer<KeyValuePair<TKey, TValue>>
    {
        /// <summary>
        /// The default <see cref="KeyValuePair{TKey, TValue}"/>.
        /// </summary>
        [NotNull] public static readonly KeyComparer<TKey, TValue> Default = new KeyComparer<TKey, TValue>();

        [NotNull]
        private readonly IEqualityComparer<TKey> _comparer;

        /// <summary>
        /// Prevents multiple instances of the <see cref="KeyComparer{TKey, TValue}"/> class from being created.
        /// </summary>
        public KeyComparer([CanBeNull]IEqualityComparer<TKey> comparer = null)
        {
            _comparer = comparer ?? EqualityComparer<TKey>.Default;
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <paramref name="T" /> to compare.</param>
        /// <param name="y">The second object of type <paramref name="T" /> to compare.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public bool Equals(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
        {
            return _comparer.Equals(x.Key, y.Key);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object" /> for which a hash code is to be returned.</param>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public int GetHashCode(KeyValuePair<TKey, TValue> obj)
        {
            return ReferenceEquals(obj.Key, null)
                ? int.MinValue
                : _comparer.GetHashCode(obj.Key);
        }
    }
}
