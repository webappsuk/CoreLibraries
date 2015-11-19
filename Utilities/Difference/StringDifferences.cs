#region © Copyright Web Applications (UK) Ltd, 2015.  All rights reserved.
// Copyright (c) 2015, Web Applications UK Ltd
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Web Applications UK Ltd nor the
//       names of its contributors may be used to endorse or promote products
//       derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL WEB APPLICATIONS UK LTD BE LIABLE FOR ANY
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Formatting;

namespace WebApplications.Utilities.Difference
{
    /// <summary>
    /// A set of string differences.
    /// </summary>
    /// <seealso cref="Differences{T}"/>
    [PublicAPI]
    [Serializable]
    public class StringDifferences : Writeable, IReadOnlyList<StringChunk>
    {
        /// <summary>
        /// The chunks.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        private readonly IReadOnlyList<StringChunk> _chunks;
        
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
        /// Whether <see cref="A"/> and <see cref="B"/> are consider equal.
        /// </summary>
        /// <value>The are equal.</value>
        public bool AreEqual => _chunks.All(c => c.AreEqual);

        /// <summary>
        /// Initializes a new instance of the <see cref="StringDifferences" /> class.
        /// </summary>
        /// <param name="a">The 'A' string.</param>
        /// <param name="offsetA">The offset to the start of a window in the first string.</param>
        /// <param name="lengthA">The length of the window in the first string.</param>
        /// <param name="b">The 'B' string.</param>
        /// <param name="offsetB">The offset to the start of a window in the second string.</param>
        /// <param name="lengthB">The length of the window in the second string.</param>
        /// <param name="textOptions">The text options.</param>
        /// <param name="comparer">The character comparer.</param>
        /// <exception cref="ArgumentNullException"><paramref name="a" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="b" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="comparer" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="offsetA" /> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="lengthA" /> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="offsetB" /> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="lengthB" /> is out of range.</exception>
        /// <exception cref="Exception">The <paramref name="comparer" /> throws an exception.</exception>
        internal StringDifferences(
            [NotNull] string a,
            int offsetA,
            int lengthA,
            [NotNull] string b,
            int offsetB,
            int lengthB,
            TextOptions textOptions,
            [NotNull] Func<char, char, bool> comparer)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            if (b == null) throw new ArgumentNullException(nameof(b));
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            A = a;
            B = b;

            if (textOptions != TextOptions.None)
            {
                // Wrap the comparer with an additional check to handle special characters.
                Func<char, char, bool> oc = comparer;
                if (textOptions.HasFlag(TextOptions.IgnoreWhiteSpace))
                    // Ignore white space - treat all whitespace as the same (note this will handle line endings too).
                    comparer = (x, y) => char.IsWhiteSpace(x) ? char.IsWhiteSpace(y) : oc(x, y);
                else if (textOptions.HasFlag(TextOptions.NormalizeLineEndings))
                // Just normalize line endings - treat '\r' and '\n\ as the same
                    comparer = (x, y) => x == '\r' || x == '\n' ? y == '\r' || y == '\n' : oc(x, y);
            }

            // Map strings based on text options
            StringMap aMap = a.ToMapped(textOptions);
            StringMap bMap = b.ToMapped(textOptions);

            // Perform diff on mapped string
            Differences<char> chunks = aMap.Diff(bMap, comparer);

            // Special case simple equality
            if (chunks.Count < 2)
            {
                Chunk<char> chunk = chunks.Single();
                // ReSharper disable once PossibleNullReferenceException
                _chunks = new[] { new StringChunk(chunk.AreEqual, a, 0, b, 0) };
                return;
            }

            // To reverse the mapping we first calculate the split points in the original strings, and find
            // the last reference to the original strings in each chunk.
            int[] aEnds = new int[chunks.Count];
            int[] bEnds = new int[chunks.Count];
            int lastA = 0;
            int lastB = 0;
            for (int i = 0; i < chunks.Count; i++)
            {
                Chunk<char> chunk = chunks[i];
                ReadOnlyWindow<char> chunkA = chunk.A;
                ReadOnlyWindow<char> chunkB = chunk.B;
                if (chunk.A != null)
                {
                    aEnds[i] = aMap.GetOriginalIndex(chunkA.Offset + chunkA.Count - 1) + 1;
                    lastA = i;
                }
                else aEnds[i] = -1;

                if (chunk.B != null)
                {
                    bEnds[i] = bMap.GetOriginalIndex(chunkB.Offset + chunkB.Count - 1) + 1;
                    lastB = i;
                }
                else bEnds[i] = -1;
            }
            
            // Now we're ready to build up a new chunk array based on the original strings
            StringChunk[] stringChunks = new StringChunk[chunks.Count];
            int aStart = 0;
            int bStart = 0;
            for (int i = 0; i < chunks.Count; i++)
            {
                int aEnd = i == lastA ? aMap.OriginalCount : aEnds[i];
                int bEnd = i == lastB ? bMap.OriginalCount : bEnds[i];

                string ac = aEnd > -1 ? a.Substring(aStart, aEnd - aStart) : null;
                string bc = bEnd > -1 ? b.Substring(bStart, bEnd - bStart) : null;

                stringChunks[i] = new StringChunk(chunks[i].AreEqual, ac, aEnd > -1 ? aStart : -1, bc, bEnd > -1 ? bStart : -1);

                if (aEnd > -1) aStart = aEnd;
                if (bEnd > -1) bStart = bEnd;
            }

            _chunks = stringChunks;
        }
        
        /// <inheritdoc />
        [ItemNotNull]
        public IEnumerator<StringChunk> GetEnumerator() => _chunks.GetEnumerator();

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
        public StringChunk this[int index] => _chunks[index];

        /// <summary>
        /// Writes this instance to a <paramref name="writer" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="lineFormat">The format.</param>
        public override void WriteTo(TextWriter writer, FormatBuilder lineFormat = null)
            => WriteTo(writer, lineFormat, DifferenceExtensions.DefaultStringChunkFormat);

        /// <summary>
        /// Writes this instance to a <paramref name="writer" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="lineFormat">The format.</param>
        /// <param name="chunkFormat">The chunk format.</param>
        public void WriteTo(TextWriter writer, FormatBuilder lineFormat, FormatBuilder chunkFormat)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));

            foreach (StringChunk chunk in _chunks)
                chunk.WriteTo(writer, lineFormat, chunkFormat);
        }
    }
}