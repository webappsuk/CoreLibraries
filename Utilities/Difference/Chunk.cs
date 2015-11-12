using System;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Difference
{
    /// <summary>
    /// Gets a chunk of comparison.
    /// </summary>
    [PublicAPI]
    [Serializable]
    public class Chunk<T>
    {
        /// <summary>
        /// <see langword="true"/> if <see cref="A"/> and <see cref="B"/> are considered equal.
        /// </summary>
        public bool AreEqual => A != null && B != null;

        /// <summary>
        /// The "A" list, if any.
        /// </summary>
        [CanBeNull]
        public readonly ReadOnlyWindow<T> A;


        /// <summary>
        /// The "B" list, if any.
        /// </summary>
        [CanBeNull]
        public readonly ReadOnlyWindow<T> B;

        /// <summary>
        /// Initializes a new instance of the <see cref="Chunk{T}" /> class.
        /// </summary>
        /// <param name="a">The "A" list.</param>
        /// <param name="b">The "B" list.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="ArgumentNullException"><paramref name="a" /> is <see langword="null" /> and
        /// <paramref name="b" /> is <see langword="null" /> (only one can be <see langword="null" /> at a time.</exception>
        internal Chunk(
            [CanBeNull] ReadOnlyWindow<T> a,
            [CanBeNull] ReadOnlyWindow<T> b)
        {
            if (a == null && b == null)
                throw new ArgumentNullException(nameof(a));
            A = a;
            B = b;
        }
    }
}