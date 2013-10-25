using System.Collections.Generic;
using System.Linq;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Implements a lookup where the order of keys is maintained.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TElement">The element type.</typeparam>
    public interface IOrderedLookup<TKey, TElement> : ILookup<TKey, TElement>
    {
        /// <summary>
        /// Gets the <see cref="T:System.Collections.Generic.IEnumerable`1"/> sequence of values indexed by a specified key.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Collections.Generic.IEnumerable`1"/> sequence of values indexed by the specified key.
        /// </returns>
        /// <param name="key">The key of the desired sequence of values.</param>
        IEnumerable<TElement> this[int key] { get; }
    }
}