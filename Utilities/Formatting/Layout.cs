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

using System.Collections.Generic;
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
        /// The tab stops.
        /// </summary>
        [PublicAPI]
        [NotNull]
        public readonly IEnumerable<ushort> TabStops;

        /// <summary>
        /// The tab character is used to fill to next tab stop.
        /// </summary>
        [PublicAPI]
        public readonly char TabChar;

        /// <summary>
        /// The split words.
        /// </summary>
        [PublicAPI]
        public readonly bool SplitWords;

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
        /// <param name="splitWords">if set to <see langword="true" /> then words will split across lines.</param>
        public Layout(
            ushort width = 120,
            byte indentSize = 0,
            byte rightMarginSize = 0,
            char indentChar = ' ',
            ushort firstLineIndentSize = 0,
            IEnumerable<ushort> tabStops = null,
            char tabChar = ' ',
            bool splitWords = false)
        {
            Width = width;
            IndentSize = indentSize;
            RightMarginSize = rightMarginSize;
            IndentChar = indentChar;
            FirstLineIndentSize = firstLineIndentSize;
            TabStops = (tabStops == null
                ? Enumerable.Range(1, width / 3)
                    .Select(t => (ushort) (t * 3))
                : tabStops
                    .Where(t => t > 0 && t < width)
                    .OrderBy(t => t))
                .Union(new[] {width})
                .Distinct()
                .ToArray();
            TabChar = tabChar;
            SplitWords = splitWords;
        }
    }
}