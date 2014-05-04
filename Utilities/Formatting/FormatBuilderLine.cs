#region © Copyright Web Applications (UK) Ltd, 2014.  All rights reserved.
// Copyright (c) 2014, Web Applications UK Ltd
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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Formatting
{
    [PublicAPI]
    public sealed partial class FormatBuilder
    {
        /// <summary>
        /// Holds an unaligned line of text, ready for alignment.
        /// </summary>
        private class Line : IEnumerable<string>
        {
            [NotNull]
            private readonly List<string> _chunks = new List<string>();

            /// <summary>
            /// The layout for the line.
            /// </summary>
            [NotNull]
            public readonly Layout Layout;

            /// <summary>
            /// The alignment.
            /// </summary>
            [PublicAPI]
            public Alignment Alignment
            {
                get { return _alignment; }
            }

            /// <summary>
            /// The line start.
            /// </summary>
            [PublicAPI]
            public readonly int Start;

            /// <summary>
            /// The line end.
            /// </summary>
            [PublicAPI]
            public readonly int End;

            /// <summary>
            /// Indicates if this line is the first line of a paragraph.
            /// </summary>
            [PublicAPI]
            public readonly bool IsFirstLine;

            /// <summary>
            /// The line length.
            /// </summary>
            private int _length;

            private Alignment _alignment;
            private bool _terminated;

            /// <summary>
            /// Initializes a new instance of the <see cref="Line" /> class.
            /// </summary>
            /// <param name="layout">The layout for the line.</param>
            /// <param name="alignment">The alignment.</param>
            /// <param name="start">The start.</param>
            /// <param name="end">The end.</param>
            /// <param name="isFirstLine">if set to <see langword="true" /> this line is the first line of a paragraph.</param>
            public Line([NotNull] Layout layout, Alignment alignment, int start, int end, bool isFirstLine)
            {
                Contract.Requires(layout != null);
                Contract.Requires(start < end);
                Layout = layout;
                _alignment = alignment;
                Start = start;
                End = end;
                IsFirstLine = isFirstLine;
            }

            /// <summary>
            /// Finishes a line.
            /// </summary>
            [PublicAPI]
            public void Finish(bool endOfParagraph)
            {
                _terminated = true;
                switch (_alignment)
                {
                    case Alignment.Left:
                    case Alignment.Right:
                        break;
                    case Alignment.Justify:
                        if (!endOfParagraph)
                        _alignment = Alignment.Left;
                        break;
                    default:
                        return;
                }

                // We strip trailing white space.
                for (int c = _chunks.Count - 1; c > -1; c--)
                {
                    string chunk = _chunks[c];
                    if (chunk == null)
                        continue;
                    if (!Char.IsWhiteSpace(chunk[0]))
                        break;
                    _chunks.RemoveAt(c);
                    _length -= chunk.Length;
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="Line"/> is terminated with a new line.
            /// </summary>
            /// <value><see langword="true" /> if terminated; otherwise, <see langword="false" />.</value>
            public bool Terminated
            {
                get { return _terminated; }
            }

            /// <summary>
            /// Adds the specified chunk to this line.
            /// </summary>
            /// <param name="chunk">The chunk.</param>
            [PublicAPI]
            public void Add([NotNull] string chunk)
            {
                Contract.Requires(chunk != null);
                Contract.Requires(chunk.Length > 0);
                _chunks.Add(chunk);
                _length += chunk.Length;
            }
            
            /// <summary>
            /// Gets the last white space location.
            /// </summary>
            /// <value>The last white space.</value>
            [PublicAPI]
            public int LastWhiteSpace
            {
                get
                {
                    bool seenWord = false;
                    int pos = Position;
                    int c = _chunks.Count;
                    while (c > 0)
                    {
                        string chunk = _chunks[--c];
                        if (chunk == null) continue;
                        pos -= chunk.Length;
                        if (!string.IsNullOrWhiteSpace(chunk))
                        {
                            seenWord = true;
                            continue;
                        }
                        if (seenWord) return pos;
                    }
                    return -1;
                }
            }
            
            /// <summary>
            /// Gets the chunk count.
            /// </summary>
            /// <value>The chunk count.</value>
            [PublicAPI]
            public int ChunkCount
            {
                get { return _chunks.Count; }
            }

            /// <summary>
            /// Gets the maximum length of the line.
            /// </summary>
            /// <value>The maximum length of the line.</value>
            [PublicAPI]
            public int MaximumLength
            {
                get { return End - Start; }
            }

            /// <summary>
            /// Gets a value indicating whether this instance is empty.
            /// </summary>
            /// <value><see langword="true" /> if this instance is empty; otherwise, <see langword="false" />.</value>
            [PublicAPI]
            public bool IsEmpty
            {
                get { return _length < 1; }
            }

            /// <summary>
            /// Gets the length of the line.
            /// </summary>
            /// <value>The length of the line.</value>
            [PublicAPI]
            public int Length
            {
                get { return _length; }
            }

            /// <summary>
            /// Gets the current horizontal position.
            /// </summary>
            /// <value>The remaining space.</value>
            [PublicAPI]
            public int Position
            {
                get { return Start + _length; }
            }

            /// <summary>
            /// Gets the remaining space on the line.
            /// </summary>
            /// <value>The remaining space.</value>
            [PublicAPI]
            public int Remaining
            {
                get { return End - Start - _length; }
            }

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
            public IEnumerator<string> GetEnumerator()
            {
                return _chunks.GetEnumerator();
            }

            /// <summary>
            /// Returns an enumerator that iterates through a collection.
            /// </summary>
            /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            /// <summary>
            /// Returns a <see cref="System.String" /> that represents this instance.
            /// </summary>
            /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
            public override string ToString()
            {
                return ToString(null);
            }

            /// <summary>
            /// Returns a <see cref="System.String" /> that represents this instance.
            /// </summary>
            /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
            [PublicAPI]
            [NotNull]
            public string ToString([CanBeNull] IFormatProvider provider)
            {
                // Used during debugging only!
                StringBuilder builder = new StringBuilder(End);
                if (Start > 0)
                    builder.Append(' ', Start);
                foreach (var chunk in _chunks)
                    if (chunk != null)
                        builder.Append(chunk.ToString(provider));
                return builder.ToString();
            }
        }
    }
}