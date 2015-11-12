using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Difference
{
    /// <summary>
    /// A set of differences between lines of text.
    /// </summary>
    [PublicAPI]
    public class LineDifferences : IReadOnlyList<LineChunk>
    {
        /// <summary>
        /// The chunks.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        private readonly IReadOnlyList<LineChunk> _chunks;

        /// <summary>
        /// The 'A' string.
        /// </summary>
        [NotNull]
        public readonly string A;

        /// <summary>
        /// The 'B' string.
        /// </summary>
        [NotNull]
        public readonly string B;

        /// <summary>
        /// Initializes a new instance of the <see cref="LineDifferences" /> class.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <param name="options">The options.</param>
        /// <param name="comparer">The comparer.</param>
        /// <exception cref="ArgumentNullException"><paramref name="a" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="b" /> is <see langword="null" />.</exception>
        internal LineDifferences(
            [NotNull] string a,
            [NotNull] string b,
            TextOptions options = TextOptions.Default,
            IEqualityComparer<char> comparer = null)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            if (b == null) throw new ArgumentNullException(nameof(b));
            A = a;
            B = b;
            if (comparer == null) comparer = CharComparer.CurrentCulture;
            throw new NotImplementedException();
        }
        
        /// <inheritdoc />
        [ItemNotNull]
        public IEnumerator<LineChunk> GetEnumerator() => _chunks.GetEnumerator();

        /// <inheritdoc />
        [ItemNotNull]
        IEnumerator IEnumerable.GetEnumerator() => _chunks.GetEnumerator();

        /// <inheritdoc />
        [ItemNotNull]
        public int Count => _chunks.Count;

        /// <inheritdoc />
        [NotNull]
        [ItemNotNull]
        // ReSharper disable once AssignNullToNotNullAttribute
        public LineChunk this[int index] => _chunks[index];

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents the differences.
        /// </summary>
        /// <param name="sep">The item separator.</param>
        /// <returns>A <see cref="System.String" /> that represents the differences.</returns>
        [NotNull]
        public string ToString(string sep)
        {
            // TODO Cache and make use of FormatBuilder, allow side-by-side comparison.
            StringBuilder builder = new StringBuilder();
            foreach (LineChunk chunk in _chunks)
            {
                string s = chunk.B;
                if (!chunk.AreEqual)
                    if (chunk.A == null)
                        builder.Append("+ ");
                    else
                    {
                        builder.Append("- ");
                        s = chunk.A;
                    }
                else
                    builder.Append("  ");
                // ReSharper disable once AssignNullToNotNullAttribute
                builder.Append(s);
                builder.Append(Environment.NewLine);
            }
            return builder.ToString();
        }
        // ReSharper restore PossibleNullReferenceException, ExceptionNotDocumented
    }

    public class LineChunk : StringChunk
    {
        internal LineChunk([NotNull] Chunk<char> charChunk)
            : base(charChunk)
        {
        }
    }
}