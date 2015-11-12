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
using System.Linq;
using System.Text;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Difference
{
    /// <summary>
    /// A set of string differences.
    /// </summary>
    /// <seealso cref="Differences{T}"/>
    [PublicAPI]
    [Serializable]
    public class StringDifferences : IReadOnlyList<StringChunk>
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
        /// Initializes a new instance of the <see cref="StringDifferences" /> class.
        /// </summary>
        /// <param name="a">The 'A' string.</param>
        /// <param name="b">The 'B' string.</param>
        /// <param name="comparer">The character comparer.</param>
        /// <exception cref="System.ArgumentNullException">
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="a" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="b" /> is <see langword="null" />.</exception>
        internal StringDifferences(
            [NotNull] string a,
            [NotNull] string b,
            IEqualityComparer<char> comparer = null)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            if (b == null) throw new ArgumentNullException(nameof(b));
            A = a;
            B = b;
            if (comparer == null) comparer = CharComparer.CurrentCulture;

            _chunks = a.ToCharArray()
                .Diff(b.ToCharArray(), comparer)
                .Select(c => new StringChunk(c)).ToArray();
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
        /// Returns a <see cref="System.String" /> that represents the differences.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents the differences.</returns>
        public override string ToString()
        {
            // TODO Cache and make use of FormatBuilder
            StringBuilder builder = new StringBuilder();
            foreach (StringChunk chunk in _chunks)
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
    }
}