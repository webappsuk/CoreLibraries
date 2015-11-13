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
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Difference
{
    /// <summary>
    /// A chunk of a <see cref="string" />.
    /// </summary>
    [PublicAPI]
    [Serializable]
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
        /// <param name="a">The original A string.</param>
        /// <param name="aMap">The original map for A.</param>
        /// <param name="b">The original B string.</param>
        /// <param name="bMap">The original map for B.</param>
        /// <param name="charChunk">The character chunk.</param>
        internal StringChunk(
            [NotNull] string a,
            [NotNull] ReadOnlyOffsetMap<char> aMap,
            [NotNull] string b,
            [NotNull] ReadOnlyOffsetMap<char> bMap,
            [NotNull] Chunk<char> charChunk)
        {
            if (charChunk.A != null)
                Map(a, aMap, charChunk.A, out A, out OffsetA);
            else OffsetA = -1;

            if (charChunk.B != null)
                Map(b, bMap, charChunk.B, out B, out OffsetB);
            else OffsetB = -1;
        }

        /// <summary>
        /// Maps back to the original string.
        /// </summary>
        /// <param name="input">a.</param>
        /// <param name="map">a map.</param>
        /// <param name="window">The cc.</param>
        /// <param name="output">The output.</param>
        /// <param name="offset">The offset.</param>
        private void Map(
                    [NotNull] string input,
                    [NotNull] ReadOnlyOffsetMap<char> map,
                    [NotNull] ReadOnlyWindow<char> window,
                    [NotNull] out string output,
                    out int offset)
        {
            // TODO This just doesn't work as it doesn't expand properly
            int wo = window.Offset;
            int o = wo < 1 ? 0 : map.MapIndex(wo);
            int l;
            
            int wc = window.Count;
            if (wc < 1) l = 0;
            else l = 1 + map.MapIndex(wo + wc - 1) - o;

            output = input.Substring(o, l);
            offset = o;
        }

        /// <inheritdoc />
        // ReSharper disable once AssignNullToNotNullAttribute
        public override string ToString() => B ?? A;
    }
}