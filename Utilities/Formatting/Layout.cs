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
using System.Text;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// Defines a layout for use with a <see cref="LayoutBuilder"/>.
    /// </summary>
    public class Layout : IFormattable
    {
        /// <summary>
        /// The default layout (as specified by the current layout writer).
        /// </summary>
        [NotNull]
        public static readonly Layout Default = new Layout(
            120,
            0,
            0,
            ' ',
            0,
            null,
            4,
            ' ',
            Formatting.Alignment.Left,
            false,
            false,
            '-',
            LayoutWrapMode.NewLine);

        /// <summary>
        /// The empty layout makes no changes.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly Layout Empty = new Layout();

        /// <summary>
        /// The layout width.
        /// </summary>
        [PublicAPI]
        public readonly Optional<ushort> Width;

        /// <summary>
        /// The indent size.
        /// </summary>
        [PublicAPI]
        public readonly Optional<byte> IndentSize;

        /// <summary>
        /// The size of any right margin.
        /// </summary>
        [PublicAPI]
        public readonly Optional<byte> RightMarginSize;

        /// <summary>
        /// The indent character (is repeated <see cref="IndentSize"/> times).
        /// </summary>
        [PublicAPI]
        public readonly Optional<char> IndentChar;

        /// <summary>
        /// The first line indent size.
        /// </summary>
        [PublicAPI]
        public readonly Optional<ushort> FirstLineIndentSize;

        /// <summary>
        /// The tab stops, only valid for <see cref="T:AlignmentChar.Left"/> and <see cref="T:AlignmentChar.None"/>.
        /// </summary>
        [PublicAPI]
        public readonly Optional<IEnumerable<ushort>> TabStops;

        /// <summary>
        /// The tab size, used to produce tabs when the layout doesn't support tab stops.
        /// </summary>
        [PublicAPI]
        public readonly Optional<byte> TabSize;

        /// <summary>
        /// The tab character is used to fill to next tab stop.
        /// </summary>
        [PublicAPI]
        public readonly Optional<char> TabChar;

        /// <summary>
        /// The alignment.
        /// </summary>
        [PublicAPI]
        public readonly Optional<Alignment> Alignment;

        /// <summary>
        /// Whether to split words onto new lines, or move the entire word onto a newline.  Note if the word is longer than the line length
        /// it will always split.
        /// </summary>
        [PublicAPI]
        public readonly Optional<bool> SplitWords;

        /// <summary>
        /// Whether to add a hyphen when splitting words.
        /// </summary>
        [PublicAPI]
        public readonly Optional<bool> Hyphenate;

        /// <summary>
        /// The hyphenation character is used to split words.
        /// </summary>
        [PublicAPI]
        public readonly Optional<char> HyphenChar;

        /// <summary>
        /// The line wrap mode
        /// </summary>
        [PublicAPI]
        public readonly Optional<LayoutWrapMode> WrapMode;

        /// <summary>
        /// Gets a value indicating whether this <see cref="Layout"/> is complete.
        /// </summary>
        /// <value><see langword="true" /> if full; otherwise, <see langword="false" />.</value>
        [PublicAPI]
        public bool IsFull
        {
            get
            {
                // Confirm everything is assigned.
                return Width.IsAssigned &&
                       IndentSize.IsAssigned &&
                       RightMarginSize.IsAssigned &&
                       IndentChar.IsAssigned &&
                       FirstLineIndentSize.IsAssigned &&
                       TabStops.IsAssigned &&
                       TabSize.IsAssigned &&
                       TabChar.IsAssigned &&
                       Alignment.IsAssigned &&
                       SplitWords.IsAssigned &&
                       Hyphenate.IsAssigned &&
                       HyphenChar.IsAssigned &&
                       WrapMode.IsAssigned;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Layout"/> is totally incomplete.
        /// </summary>
        /// <value><see langword="true" /> if empty; otherwise, <see langword="false" />.</value>
        [PublicAPI]
        public bool IsEmpty
        {
            get
            {
                // Confirm everything is not assigned.
                return !Width.IsAssigned &&
                       !IndentSize.IsAssigned &&
                       !RightMarginSize.IsAssigned &&
                       !IndentChar.IsAssigned &&
                       !FirstLineIndentSize.IsAssigned &&
                       !TabStops.IsAssigned &&
                       !TabSize.IsAssigned &&
                       !TabChar.IsAssigned &&
                       !Alignment.IsAssigned &&
                       !SplitWords.IsAssigned &&
                       !Hyphenate.IsAssigned &&
                       !HyphenChar.IsAssigned &&
                       !WrapMode.IsAssigned;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Layout" /> class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="indentSize">Size of the indent.</param>
        /// <param name="rightMarginSize">Size of the right margin.</param>
        /// <param name="indentChar">The indent character.</param>
        /// <param name="firstLineIndentSize">First size of the line indent.</param>
        /// <param name="tabStops">The tab stops.</param>
        /// <param name="tabSize">Size of the tab.</param>
        /// <param name="tabChar">The tab character.</param>
        /// <param name="alignment">The alignment.</param>
        /// <param name="splitWords">if set to <see langword="true" /> then words will split across lines.</param>
        /// <param name="hyphenate">if set to <see langword="true" /> [hyphenate].</param>
        /// <param name="hyphenChar">The hyphenation character.</param>
        /// <param name="wrapMode">The line wrap mode.</param>
        public Layout(
            Optional<ushort> width = default(Optional<ushort>),
            Optional<byte> indentSize = default(Optional<byte>),
            Optional<byte> rightMarginSize = default(Optional<byte>),
            Optional<char> indentChar = default(Optional<char>),
            Optional<ushort> firstLineIndentSize = default(Optional<ushort>),
            Optional<IEnumerable<ushort>> tabStops = default(Optional<IEnumerable<ushort>>),
            Optional<byte> tabSize = default(Optional<byte>),
            Optional<char> tabChar = default(Optional<char>),
            Optional<Alignment> alignment = default(Optional<Alignment>),
            Optional<bool> splitWords = default(Optional<bool>),
            Optional<bool> hyphenate = default(Optional<bool>),
            Optional<char> hyphenChar = default(Optional<char>),
            Optional<LayoutWrapMode> wrapMode = default(Optional<LayoutWrapMode>))
        {
            // Normalize margins
            if (width.IsAssigned)
            {
                if (width.Value < 1)
                    width = 1;

                byte w = (byte)(width.Value - 1);
                if (indentSize.IsAssigned &&
                    indentSize.Value > w)
                    indentSize = w;
                if (firstLineIndentSize.IsAssigned &&
                    firstLineIndentSize.Value > w)
                    firstLineIndentSize = w;
                if (rightMarginSize.IsAssigned)
                {
                    if (indentSize.IsAssigned &&
                        rightMarginSize.Value > w - indentSize.Value)
                        rightMarginSize = (byte)(w - indentSize.Value);
                    if (firstLineIndentSize.IsAssigned &&
                        rightMarginSize.Value > w - firstLineIndentSize.Value)
                        rightMarginSize = (byte)(w - firstLineIndentSize.Value);
                }
                if (tabSize.IsAssigned)
                    if (tabSize.Value < 1) tabSize = 1;
                    else if (tabSize.Value > width.Value) tabSize = (byte)width.Value;

                // Only support tabstop on left/non alignments
                if (alignment.IsAssigned &&
                    tabStops.IsAssigned)
                    if ((alignment.Value == Formatting.Alignment.Left) ||
                        (alignment.Value == Formatting.Alignment.None))
                        tabStops = (!tabStops.IsAssigned || tabStops.IsNull
                            ? Enumerable.Range(1, width.Value / tabSize.Value)
                                .Select(t => (ushort)(t * tabSize.Value))
                            // ReSharper disable once AssignNullToNotNullAttribute
                            : tabStops.Value
                                .Where(t => t > 0 && t < width.Value)
                                .OrderBy(t => t))
                            .Distinct()
                            .ToArray();
                    else
                        tabStops = null;
            }

            Width = width;
            IndentSize = indentSize;
            RightMarginSize = rightMarginSize;
            IndentChar = indentChar;
            FirstLineIndentSize = firstLineIndentSize;
            TabStops = tabStops;
            TabSize = tabSize;
            TabChar = tabChar;
            Alignment = alignment;
            SplitWords = splitWords;
            Hyphenate = hyphenate;
            HyphenChar = hyphenChar;
            WrapMode = wrapMode;
        }

        /// <summary>
        /// Applies the specified layout to this layout returning a new, combined layout.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="indentSize">Size of the indent.</param>
        /// <param name="rightMarginSize">Size of the right margin.</param>
        /// <param name="indentChar">The indent character.</param>
        /// <param name="firstLineIndentSize">First size of the line indent.</param>
        /// <param name="tabStops">The tab stops.</param>
        /// <param name="tabSize">Size of the tab.</param>
        /// <param name="tabChar">The tab character.</param>
        /// <param name="alignment">The alignment.</param>
        /// <param name="splitWords">The split words.</param>
        /// <param name="hyphenate">The hyphenate.</param>
        /// <param name="hyphenChar">The hyphen character.</param>
        /// <param name="wrapMode">The line wrap mode.</param>
        /// <returns>
        /// Layout.
        /// </returns>
        [PublicAPI]
        [NotNull]
        public Layout Apply(
            Optional<ushort> width = default(Optional<ushort>),
            Optional<byte> indentSize = default(Optional<byte>),
            Optional<byte> rightMarginSize = default(Optional<byte>),
            Optional<char> indentChar = default(Optional<char>),
            Optional<ushort> firstLineIndentSize = default(Optional<ushort>),
            Optional<IEnumerable<ushort>> tabStops = default(Optional<IEnumerable<ushort>>),
            Optional<byte> tabSize = default(Optional<byte>),
            Optional<char> tabChar = default(Optional<char>),
            Optional<Alignment> alignment = default(Optional<Alignment>),
            Optional<bool> splitWords = default(Optional<bool>),
            Optional<bool> hyphenate = default(Optional<bool>),
            Optional<char> hyphenChar = default(Optional<char>),
            Optional<LayoutWrapMode> wrapMode = default(Optional<LayoutWrapMode>))
        {
            return new Layout(
                width.IsAssigned ? width : Width,
                indentSize.IsAssigned ? indentSize : IndentSize,
                rightMarginSize.IsAssigned ? rightMarginSize : RightMarginSize,
                indentChar.IsAssigned ? indentChar : IndentChar,
                firstLineIndentSize.IsAssigned ? firstLineIndentSize : FirstLineIndentSize,
                tabStops.IsAssigned ? tabStops : TabStops,
                tabSize.IsAssigned ? tabSize : TabSize,
                tabChar.IsAssigned ? tabChar : TabChar,
                alignment.IsAssigned ? alignment : Alignment,
                splitWords.IsAssigned ? splitWords : SplitWords,
                hyphenate.IsAssigned ? hyphenate : Hyphenate,
                hyphenChar.IsAssigned ? hyphenChar : HyphenChar,
                wrapMode.IsAssigned ? wrapMode : WrapMode);
        }

        /// <summary>
        /// Applies the specified layout to this layout returning a new, combined layout.
        /// </summary>
        /// <param name="layout">The layout.</param>
        /// <returns>Layout.</returns>
        [PublicAPI]
        [NotNull]
        public Layout Apply([CanBeNull] Layout layout)
        {
            return layout == null
                ? this
                : new Layout(
                    layout.Width.IsAssigned ? layout.Width : Width,
                    layout.IndentSize.IsAssigned ? layout.IndentSize : IndentSize,
                    layout.RightMarginSize.IsAssigned ? layout.RightMarginSize : RightMarginSize,
                    layout.IndentChar.IsAssigned ? layout.IndentChar : IndentChar,
                    layout.FirstLineIndentSize.IsAssigned ? layout.FirstLineIndentSize : FirstLineIndentSize,
                    layout.TabStops.IsAssigned ? layout.TabStops : TabStops,
                    layout.TabSize.IsAssigned ? layout.TabSize : TabSize,
                    layout.TabChar.IsAssigned ? layout.TabChar : TabChar,
                    layout.Alignment.IsAssigned ? layout.Alignment : Alignment,
                    layout.SplitWords.IsAssigned ? layout.SplitWords : SplitWords,
                    layout.Hyphenate.IsAssigned ? layout.Hyphenate : Hyphenate,
                    layout.HyphenChar.IsAssigned ? layout.HyphenChar : HyphenChar,
                    layout.WrapMode.IsAssigned ? layout.WrapMode : WrapMode);
        }

        /// <summary>
        /// Parses the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>A <see cref="Layout"/> if valid; otherwise <see langword="null"/>.</returns>
        [PublicAPI]
        [CanBeNull]
        public static Layout Parse([CanBeNull] string input)
        {
            Layout layout;
            return TryParse(input, out layout) ? layout : null;
        }

        /// <summary>
        /// Tries to parse the string into a valid <see cref="Layout"/>.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="layout">A <see cref="Layout" /> if valid; otherwise <see langword="null" />.</param>
        /// <returns><see langword="true" /> if parse succeeded, <see langword="false" /> otherwise.</returns>
        [PublicAPI]
        public static bool TryParse([CanBeNull] string input, out Layout layout)
        {
            if (input == null)
            {
                layout = null;
                return false;
            }

            string[] parts = input.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            layout = Empty;
            if (parts.Length < 1)
                return true;

            Optional<ushort> width = default(Optional<ushort>);
            Optional<byte> indentSize = default(Optional<byte>);
            Optional<byte> rightMarginSize = default(Optional<byte>);
            Optional<char> indentChar = default(Optional<char>);
            Optional<ushort> firstLineIndentSize = default(Optional<ushort>);
            Optional<IEnumerable<ushort>> tabStops = default(Optional<IEnumerable<ushort>>);
            Optional<byte> tabSize = default(Optional<byte>);
            Optional<char> tabChar = default(Optional<char>);
            Optional<Alignment> alignment = default(Optional<Alignment>);
            Optional<bool> splitWords = default(Optional<bool>);
            Optional<bool> hyphenate = default(Optional<bool>);
            Optional<char> hyphenChar = default(Optional<char>);
            Optional<LayoutWrapMode> wrapMode = default(Optional<LayoutWrapMode>);

            foreach (string part in parts)
            {
                Contract.Assert(!string.IsNullOrEmpty(part));

                if (part.Length < 2)
                    return false;

                char prefix = part[0];
                string s = part.Substring(1);

                switch (prefix)
                {
                    case 'w':
                        ushort w;
                        if (!ushort.TryParse(s, out w))
                            return false;
                        width = w;
                        break;
                    case 'i':
                        byte i;
                        if (!byte.TryParse(s, out i))
                            return false;
                        indentSize = i;
                        break;
                    case 'r':
                        byte r;
                        if (!byte.TryParse(s, out r))
                            return false;
                        rightMarginSize = r;
                        break;
                    case 'I':
                        if (s.Length != 1)
                            return false;
                        indentChar = s[0];
                        break;
                    case 'f':
                        ushort f;
                        if (!ushort.TryParse(s, out f))
                            return false;
                        firstLineIndentSize = f;
                        break;
                    case 'l':
                        bool ok = true;
                        tabStops = new Optional<IEnumerable<ushort>>(
                            s
                                .Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(
                                    tp =>
                                    {
                                        ushort tps;
                                        if (!ushort.TryParse(tp, out tps))
                                            ok = false;
                                        return tps;
                                    })
                                .TakeWhile(kvp => ok));
                        if (!ok)
                            return false;
                        break;
                    case 't':
                        byte t;
                        if (!byte.TryParse(s, out t))
                            return false;
                        tabSize = t;
                        break;
                    case 'T':
                        if (s.Length != 1)
                            return false;
                        tabChar = s[0];
                        break;
                    case 'a':
                        Alignment a;
                        if (!Enum.TryParse(s, true, out a))
                            return false;
                        alignment = a;
                        break;
                    case 's':
                        bool sb;
                        if (!bool.TryParse(s, out sb))
                            return false;
                        splitWords = sb;
                        break;
                    case 'h':
                        bool h;
                        if (!bool.TryParse(s, out h))
                            return false;
                        hyphenate = h;
                        break;
                    case 'H':
                        if (s.Length != 1)
                            return false;
                        hyphenChar = s[0];
                        break;
                    case 'p':
                        LayoutWrapMode wm;
                        if (!Enum.TryParse(s, true, out wm))
                            return false;
                        wrapMode = wm;
                        break;
                    default:
                        return false;
                }
            }

            layout = new Layout(
                width,
                indentSize,
                rightMarginSize,
                indentChar,
                firstLineIndentSize,
                tabStops,
                tabSize,
                tabChar,
                alignment,
                splitWords,
                hyphenate,
                hyphenChar,
                wrapMode);
            return true;
        }

        /// <summary>
        /// To the string.
        /// </summary>
        /// <returns>System.String.</returns>
        public override string ToString()
        {
            return ToString("g", null);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        [NotNull]
        [PublicAPI]
        public string ToString([CanBeNull] string format)
        {
            return ToString(format, null);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format to use.-or- A null reference (Nothing in Visual Basic) to use the default format defined for the type of the <see cref="T:System.IFormattable" /> implementation.</param>
        /// <param name="formatProvider">The provider to use to format the value.-or- A null reference (Nothing in Visual Basic) to obtain the numeric format information from the current locale setting of the operating system.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public string ToString([CanBeNull] string format, [CanBeNull] IFormatProvider formatProvider)
        {
            if (format == null)
                format = "g";
            StringBuilder sb = new StringBuilder();
            switch (format.ToLowerInvariant())
            {
                // Creates a string for indicating the positions of margins and tab stops
                case "l":
                    var layout = this;
                    bool partial = !layout.IsFull;

                    // If we have a partial layout, apply it to the default layout so we can display it.
                    if (!partial)
                        layout = Default.Apply(this);

                    char[] cArr = new char[Width.Value];
                    int rm = layout.Width.Value - 1 - layout.RightMarginSize.Value;
                    for (int i = 0; i < layout.Width.Value; i++)
                    {
                        bool up = (i == layout.FirstLineIndentSize.Value) ||
                                  (i == rm);
                        cArr[i] = i == layout.IndentSize.Value
                            ? (up ? 'X' : 'V')
                            : (up
                                ? '^'
                                : (layout.TabStops.Value != null && layout.TabStops.Value.Contains((ushort) i)
                                    ? 'L'
                                    : (i % 10 == 0
                                        ? (char) ('0' + (i / 10) % 10)
                                        : (partial
                                            ? '.'
                                            : '_'))));
                    }
                    return new string(cArr);

                // Creates a compact string that can be easily parsed back into a layout
                case "f":
                    if (Width.IsAssigned)
                        sb.Append('w').Append(Width.Value).Append(';');
                    if (IndentSize.IsAssigned)
                        sb.Append('i').Append(IndentSize.Value).Append(';');
                    if (RightMarginSize.IsAssigned)
                        sb.Append('r').Append(RightMarginSize.Value).Append(';');
                    if (IndentChar.IsAssigned)
                        sb.Append('I').Append(IndentChar.Value).Append(';');
                    if (FirstLineIndentSize.IsAssigned)
                        sb.Append('f').Append(FirstLineIndentSize.Value).Append(';');
                    if (TabStops.IsAssigned &&
                        TabStops.Value != null)
                        sb.Append('l')
                            .Append(string.Join("|", TabStops.Value.Select(t => t.ToString(formatProvider))))
                            .Append(';');
                    if (TabSize.IsAssigned)
                        sb.Append('t').Append(TabSize.Value).Append(';');
                    if (TabChar.IsAssigned)
                        sb.Append('T').Append(TabChar.Value).Append(';');
                    if (Alignment.IsAssigned)
                        sb.Append('a').Append(Alignment.Value).Append(';');
                    if (SplitWords.IsAssigned)
                        sb.Append('s').Append(SplitWords.Value).Append(';');
                    if (Hyphenate.IsAssigned)
                        sb.Append('h').Append(Hyphenate.Value).Append(';');
                    if (HyphenChar.IsAssigned)
                        sb.Append('H').Append(HyphenChar.Value).Append(';');
                    if (WrapMode.IsAssigned)
                        sb.Append('p').Append(WrapMode.Value).Append(';');

                    // Remove trailing ';'
                    if (sb.Length > 1)
                        sb.Remove(sb.Length - 1, 1);
                    break;

                // Creates a nice, human readable string
                default:
                    if (Width.IsAssigned)
                        sb.Append("Width = ").Append(Width.Value).Append(", ");
                    if (IndentSize.IsAssigned)
                        sb.Append("Indent Size = ").Append(IndentSize.Value).Append(", ");
                    if (RightMarginSize.IsAssigned)
                        sb.Append("Right Margin Size = ").Append(RightMarginSize.Value).Append(", ");
                    if (IndentChar.IsAssigned)
                        sb.Append("Indent Character = '").Append(IndentChar.Value).Append("', ");
                    if (FirstLineIndentSize.IsAssigned)
                        sb.Append("First Line Indent Size = ").Append(FirstLineIndentSize.Value).Append(", ");
                    if (TabStops.IsAssigned &&
                        TabStops.Value != null)
                        sb.Append("Tab Stops = [")
                            .Append(string.Join(" | ", TabStops.Value.Select(t => t.ToString(formatProvider))))
                            .Append("], ");
                    if (TabSize.IsAssigned)
                        sb.Append("Tab Size = ").Append(TabSize.Value).Append(", ");
                    if (TabChar.IsAssigned)
                        sb.Append("Tab Char = '").Append(TabChar.Value).Append("', ");
                    if (Alignment.IsAssigned)
                        sb.Append("Alignment = ").Append(Alignment.Value).Append(", ");
                    if (SplitWords.IsAssigned)
                        sb.Append("Split Words = ").Append(SplitWords.Value).Append(", ");
                    if (Hyphenate.IsAssigned)
                        sb.Append("Hyphenate = ").Append(Hyphenate.Value).Append(", ");
                    if (HyphenChar.IsAssigned)
                        sb.Append("Hyphen Character = '").Append(HyphenChar.Value).Append("', ");
                    if (WrapMode.IsAssigned)
                        sb.Append("Line Wrap Mode = ").Append(WrapMode.Value).Append(", ");

                    // Remove trailing ", "
                    if (sb.Length > 2)
                        sb.Remove(sb.Length - 2, 2);
                    break;
            }
            return sb.ToString();
        }
    }
}