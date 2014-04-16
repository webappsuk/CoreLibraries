using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace WebApplications.Utilities
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
            ushort width,
            byte indentSize, 
            byte rightMarginSize,
            char indentChar,
            ushort firstLineIndentSize,
            IEnumerable<ushort> tabStops, 
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