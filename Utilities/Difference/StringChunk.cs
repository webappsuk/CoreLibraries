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
using System.IO;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Formatting;

namespace WebApplications.Utilities.Difference
{
    /// <summary>
    /// A chunk of a <see cref="string" />.
    /// </summary>
    [PublicAPI]
    [Serializable]
    public class StringChunk : Writeable
    {
        /// <summary>
        /// <see langword="true"/> if <see cref="A"/> and <see cref="B"/> are considered equal.
        /// </summary>
        public readonly bool AreEqual;

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
        /// <param name="a">The original A string.</param>
        /// <param name="offsetA">The offset a.</param>
        /// <param name="b">The original B string.</param>
        /// <param name="offsetB">The offset b.</param>
        internal StringChunk(
            bool areEqual,
            [CanBeNull] string a,
            int offsetA,
            [CanBeNull] string b,
            int offsetB)
        {
            AreEqual = areEqual;
            A = a;
            B = b;
            OffsetA = offsetA;
            OffsetB = offsetB;
        }

        /// <summary>
        /// Writes this instance to a <paramref name="writer"/>.
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

            if (lineFormat == null)
                lineFormat = DifferenceExtensions.DefaultLineFormat;
            if (chunkFormat == null)
                chunkFormat = DifferenceExtensions.DefaultStringChunkFormat;

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
                            DifferenceExtensions.SeperatorTag,
                            StringComparison.InvariantCultureIgnoreCase))
                            return " | ";

                        return Resolution.Unknown;
                    });
        }
    }
}