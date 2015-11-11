using System.Linq;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Difference
{
    /// <summary>
    /// A chunk of a <see cref="string" />.
    /// </summary>
    [PublicAPI]
    public class StringChunk
    {
        /// <summary>
        /// The source of the chunk.
        /// </summary>
        public Source Source { get; }

        /// <summary>
        /// The chunk.
        /// </summary>
        [NotNull]
        public readonly string Chunk;

        /// <summary>
        /// The offset in the original string.
        /// </summary>
        public readonly int Offset;

        /// <summary>
        /// The length of the chunk.
        /// </summary>
        public readonly int Count;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringChunk" /> class.
        /// </summary>
        /// <param name="charChunk">The character chunk.</param>
        internal StringChunk([NotNull] Chunk<char> charChunk)
        {
            Source = charChunk.Source;
            Chunk = new string(charChunk.ToArray());
            Offset = charChunk.Offset;
            Count = charChunk.Count;
        }

        /// <inheritdoc />
        public override string ToString() => Chunk;
    }
}