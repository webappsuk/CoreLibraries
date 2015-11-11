using System;
using System.Collections.Generic;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Difference
{
    /// <summary>
    /// Gets a chunk of comparison.
    /// </summary>
    [PublicAPI]
    public class Chunk<T> : ReadOnlyWindow<T>, IChunk<T>
    {
        /// <summary>
        /// The source of the chunk.
        /// </summary>
        public Source Source { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Chunk{T}"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="data">The data.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="offset" /> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="length" /> is out of range.</exception>
        internal Chunk(Source source, [NotNull] [ItemNotNull] IReadOnlyList<T> data, int offset, int length)
            : base(data, offset, length, false)
        {
            Source = source;
        }
    }
}