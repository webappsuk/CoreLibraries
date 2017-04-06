using System;
using System.IO;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Formatting;

namespace WebApplications.Utilities.Difference
{
    /// <summary>
    /// Gets a chunk of comparison.
    /// </summary>
    [PublicAPI]
    [Serializable]
    public class Chunk<T> : Writeable
    {
        /// <summary>
        /// <see langword="true"/> if <see cref="A"/> and <see cref="B"/> are considered equal.
        /// </summary>
        public readonly bool AreEqual;

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
        /// <param name="areEqual">if set to <see langword="true" /> <paramref name="a"/> and <paramref name="b"/> are considered equal.</param>
        /// <param name="a">The "A" list.</param>
        /// <param name="b">The "B" list.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        internal Chunk(
            bool areEqual,
            [CanBeNull] ReadOnlyWindow<T> a,
            [CanBeNull] ReadOnlyWindow<T> b)
        {
            if (a == null && b == null)
                throw new ArgumentNullException(nameof(a));
            AreEqual = areEqual;
            A = a;
            B = b;
        }

        /// <summary>
        /// Writes this instance to a <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="lineFormat">The format.</param>
        public override void WriteTo(TextWriter writer, FormatBuilder lineFormat = null)
            => WriteTo(writer, lineFormat, DifferenceExtensions.DefaultChunkFormat);

        /// <summary>
        /// Writes this instance to a <paramref name="writer" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="lineFormat">The format.</param>
        /// <param name="chunkFormat">The chunk format.</param>
        public void WriteTo(TextWriter writer, FormatBuilder lineFormat, FormatBuilder chunkFormat)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));

            if (lineFormat == null)
                lineFormat = DifferenceExtensions.DefaultLineFormat;
            if (chunkFormat == null)
                chunkFormat = DifferenceExtensions.DefaultChunkFormat;

            string[] aLines = A == null
                ? Array<string>.Empty
                : chunkFormat.ToString(new DictionaryResolvable() { { DifferenceExtensions.ChunkTag, A } })
                    .Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            string[] bLines = B == null
                ? Array<string>.Empty
                : chunkFormat.ToString(new DictionaryResolvable() { { DifferenceExtensions.ChunkTag, B } })
                    .Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            int maxLen = aLines.Length < bLines.Length ? bLines.Length : aLines.Length;

            string flag;
            if (AreEqual)
            {
                flag = " ";
            }
            else if (A == null)
            {
                flag = "+";
            }
            else if (B == null)
            {
                flag = "-";
            }
            else
            {
                flag = "!";
            }

            // Write out lines in chunk.
            for (int l = 0; l < maxLen; l++)
                lineFormat.WriteTo(
                    writer,
                    "G",
                    (_, c) =>
                    {
                        if (string.Equals(
                            c.Tag,
                            DifferenceExtensions.ChunkATag,
                            StringComparison.InvariantCultureIgnoreCase))
                            return l < aLines.Length ? aLines[l] : Resolution.Empty;
                        if (string.Equals(
                            c.Tag,
                            DifferenceExtensions.ChunkBTag,
                            StringComparison.InvariantCultureIgnoreCase))
                            return l < bLines.Length ? bLines[l] : Resolution.Empty;
                        if (string.Equals(
                            c.Tag,
                            DifferenceExtensions.FlagTag,
                            StringComparison.InvariantCultureIgnoreCase))
                            return flag;
                        if (string.Equals(
                            c.Tag,
                            DifferenceExtensions.SeparatorTag,
                            StringComparison.InvariantCultureIgnoreCase))
                            return " | ";

                        return Resolution.Unknown;
                    });
        }
    }
}