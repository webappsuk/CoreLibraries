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
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// Defines a layout for use with a <see cref="LayoutWriter"/>.
    /// </summary>
    public class Layout
    {
        /// <summary>
        /// The default tab size.
        /// </summary>
        public const byte DefaultTabSize = 4;

        /// <summary>
        /// Calculate a string representation of the layout.
        /// </summary>
        [NotNull]
        private readonly Lazy<string> _string;

        /// <summary>
        /// The layout width.
        /// </summary>
        [PublicAPI]
        public readonly ushort Width;

        /// <summary>
        /// The indent size.
        /// </summary>
        [PublicAPI]
        public readonly byte IndentSize;

        /// <summary>
        /// The size of any right margin.
        /// </summary>
        [PublicAPI]
        public readonly byte RightMarginSize;

        /// <summary>
        /// The indent character (is repeated <see cref="IndentSize"/> times).
        /// </summary>
        [PublicAPI]
        public readonly char IndentChar;

        /// <summary>
        /// The first line indent size.
        /// </summary>
        [PublicAPI]
        public readonly ushort FirstLineIndentSize;

        /// <summary>
        /// The tab stops, only valid for <see cref="T:Alignment.Left"/> and <see cref="T:Alignment.None"/>.
        /// </summary>
        [PublicAPI]
        [CanBeNull]
        public readonly IEnumerable<ushort> TabStops;

        /// <summary>
        /// The tab size.
        /// </summary>
        public readonly int TabSize;

        /// <summary>
        /// The tab character is used to fill to next tab stop.
        /// </summary>
        [PublicAPI]
        public readonly char TabChar;

        /// <summary>
        /// The alignment.
        /// </summary>
        [PublicAPI]
        public readonly Alignment Alignment;

        /// <summary>
        /// Whether to split words onto new lines, or move the entire word onto a newline.  Note if the word is longer than the line length
        /// it will always split.
        /// </summary>
        [PublicAPI]
        public readonly bool SplitWords;

        /// <summary>
        /// Whether to add a hyphen when splitting words.
        /// </summary>
        [PublicAPI]
        public readonly bool Hyphenate;

        /// <summary>
        /// The hyphenation character is used to split words.
        /// </summary>
        [PublicAPI]
        public readonly char HyphenChar;

        /// <summary>
        /// Initializes a new instance of the <see cref="Layout" /> class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="indentSize">Size of the indent.</param>
        /// <param name="rightMarginSize">Size of the right margin.</param>
        /// <param name="indentChar">The indent character.</param>
        /// <param name="firstLineIndentSize">First size of the line indent.</param>
        /// <param name="tabStops">The tab stops.</param>
        /// <param name="tabChar">The tab character.</param>
        /// <param name="alignment">The alignment.</param>
        /// <param name="splitWords">if set to <see langword="true" /> then words will split across lines.</param>
        /// <param name="hyphenate">if set to <see langword="true" /> [hyphenate].</param>
        /// <param name="hyphenChar">The hyphenation character.</param>
        public Layout(
            ushort width = 120,
            byte indentSize = 0,
            byte rightMarginSize = 0,
            char indentChar = ' ',
            ushort firstLineIndentSize = 0,
            IEnumerable<ushort> tabStops = null,
            char tabChar = ' ',
            Alignment alignment = Alignment.Left,
            bool splitWords = false,
            bool hyphenate = false,
            char hyphenChar = '-')
        {
            Contract.Requires(width > 0);
            // TODO Validate! (RM > LM, etc.)
            Width = width;
            IndentSize = indentSize;
            RightMarginSize = rightMarginSize;
            IndentChar = indentChar;
            FirstLineIndentSize = firstLineIndentSize;
            TabStops = (tabStops == null
                ? Enumerable.Range(1, width / DefaultTabSize)
                    .Select(t => (ushort) (t * DefaultTabSize))
                : tabStops
                    .Where(t => t > 0 && t < width)
                    .OrderBy(t => t))
                .Distinct()
                .ToArray();
            TabChar = tabChar;
            Alignment = alignment;
            SplitWords = splitWords;
            Hyphenate = hyphenate;
            HyphenChar = hyphenChar;

            _string = new Lazy<string>(
                () =>
                {
                    char[] cArr = new char[width];
                    int rm = width - 1 - rightMarginSize;
                    for (int i = 0; i < width; i++)
                    {
                        bool up = (i == firstLineIndentSize) ||
                                  (i == rm);
                        cArr[i] = i == indentSize
                            ? (up ? 'X' : 'V')
                            : (up
                                ? '^'
                                : (TabStops.Contains((ushort) i)
                                    ? 'L'
                                    : (i % 10 == 0
                                        ? (char) ('0' + (i / 10) % 10)
                                        : '.')));
                    }
                    return new string(cArr);
                });
        }

        /// <summary>
        /// To the string.
        /// </summary>
        /// <returns>System.String.</returns>
        public override string ToString()
        {
            return _string.Value ?? string.Empty;
        }
    }
}