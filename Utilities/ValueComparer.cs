using System.Collections.Generic;
using JetBrains.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Equality comparer for <see cref="KeyValuePair{TKey, TValue}"/> that only considers values.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class ValueComparer<TKey, TValue> : IEqualityComparer<KeyValuePair<TKey, TValue>>
    {
        /// <summary>
        /// The default <see cref="KeyValuePair{TKey, TValue}"/>.
        /// </summary>
        [NotNull]
        public static readonly ValueComparer<TKey, TValue> Default = new ValueComparer<TKey, TValue>();

        [NotNull]
        private readonly IEqualityComparer<TValue> _comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueComparer{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="comparer">The value comparer.</param>
        public ValueComparer([CanBeNull]IEqualityComparer<TValue> comparer = null)
        {
            _comparer = comparer ?? EqualityComparer<TValue>.Default;
        }

        /// <summary>
        /// Equalses the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>System.Boolean.</returns>
        public bool Equals(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
        {
            return _comparer.Equals(x.Value, y.Value);
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>System.Int32.</returns>
        public int GetHashCode(KeyValuePair<TKey, TValue> obj)
        {
            return ReferenceEquals(obj.Value, null)
                ? int.MinValue
                : _comparer.GetHashCode(obj.Value);
        }
    }
}