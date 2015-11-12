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
        /// <see langword="true"/> if <see cref="A"/> and <see cref="B"/> are considered equal.
        /// </summary>
        public bool AreEqual => A != null && B != null;

        /// <summary>
        /// The "A" string.
        /// </summary>
        [CanBeNull]
        public readonly string A;

        /// <summary>
        /// The offset of <see cref="A"/> in the original "A" string.
        /// </summary>
        public readonly int OffsetA;

        /// <summary>
        /// The "B" string.
        /// </summary>
        [CanBeNull]
        public readonly string B;

        /// <summary>
        /// The offset of <see cref="B"/> in the original "B" string.
        /// </summary>
        public readonly int OffsetB;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringChunk" /> class.
        /// </summary>
        /// <param name="charChunk">The character chunk.</param>
        internal StringChunk([NotNull] Chunk<char> charChunk)
        {
            if (charChunk.A != null)
            {
                A = new string(charChunk.A.ToArray());
                OffsetA = charChunk.A.Offset;
            }
            else OffsetA = -1;

            if (charChunk.B != null)
            {
                B = new string(charChunk.B.ToArray());
                OffsetB = charChunk.B.Offset;
            }
            else OffsetB = -1;
        }

        /// <inheritdoc />
        // ReSharper disable once AssignNullToNotNullAttribute
        public override string ToString() => B ?? A;
    }
}