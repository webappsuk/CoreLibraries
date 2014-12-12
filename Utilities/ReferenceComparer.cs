using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    /// A reference equality comparer.
    /// </summary>
    /// <remarks>This class uses <see cref="Object.ReferenceEquals"/> to determain reference equality and <see cref="RuntimeHelpers.GetHashCode(object)"/> for getting the hash code.</remarks>
    /// <typeparam name="T">The type of the objects to compare.</typeparam>
    [PublicAPI]
    public class ReferenceComparer<T> : IEqualityComparer<T>
        where T : class
    {
        /// <summary>
        /// The default comparer.
        /// </summary>
        [PublicAPI]
        public static ReferenceComparer<T> Default = new ReferenceComparer<T>();

        /// <summary>
        /// Prevents a default instance of the <see cref="ReferenceComparer{T}"/> class from being created.
        /// </summary>
        private ReferenceComparer()
        {
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <typeparamref name="T" /> to compare.</param>
        /// <param name="y">The second object of type <typeparamref name="T" /> to compare.</param>
        /// <returns>
        /// true if the specified objects are reference equal; otherwise, false.
        /// </returns>
        public bool Equals([CanBeNull] T x, [CanBeNull] T y)
        {
            return ReferenceEquals(x, y);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public int GetHashCode([CanBeNull] T obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}
