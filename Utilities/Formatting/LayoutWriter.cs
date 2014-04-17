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
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// Lays out text written to the underlying text writer.
    /// </summary>
    public class LayoutWriter : FormatWriter
    {
        /// <summary>
        /// Holds an unaligned line.
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
            public Line([NotNull] Layout layout, Alignment alignment, int start, int end)
            {
                Contract.Requires(layout != null);
                Contract.Requires(start < end);
                Layout = layout;
                _alignment = alignment;
                Start = start;
                End = end;
            }

            /// <summary>
            /// Finishes a line.
            /// </summary>
            [PublicAPI]
            public void Finish(bool terminated, bool isEndOfParagraph)
            {
                _terminated = terminated;
                if (_alignment == Alignment.None) return;
                // If the line finished mid-line, we can't align fully,
                // we downgrade to Left alignment, unless we have no alignment already.
                if (!terminated &&
                    _chunks.LastOrDefault() != "\r")
                    _alignment = Alignment.Left;
                else if (_alignment != Alignment.Centre)
                {
                    // If this is the last line in a paragraph we don't justify.
                    if (isEndOfParagraph &&
                        _alignment == Alignment.Justify)
                        _alignment = Alignment.Left;

                    // We strip trailing white space.
                    for (int c = _chunks.Count - 1; c > -1; c--)
                    {
                        string chunk = _chunks[c];
                        Contract.Assert(!string.IsNullOrEmpty(chunk));
                        if (!Char.IsWhiteSpace(chunk[0]))
                            break;
                        _chunks.RemoveAt(c);
                        _length -= chunk.Length;
                    }
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
            /// Gets the last word's length
            /// </summary>
            /// <value>The last word's of the length.</value>
            public int LastWordLength
            {
                get
                {
                    string lastWord = _chunks.LastOrDefault();
                    return string.IsNullOrEmpty(lastWord) ? 0 : lastWord.Length;
                }
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
        }

        /// <summary>
        /// The default layout.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly Layout DefaultLayout = new Layout();

        /// <summary>
        /// The current layout
        /// </summary>
        [NotNull]
        private readonly Layout _defaultLayout;

        /// <summary>
        /// The current layout
        /// </summary>
        [NotNull]
        private Layout _layout;

        private bool _firstLine;
        private int _position;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutWriter" /> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="layout">The layout.</param>
        /// <param name="startPosition">The start position, if the writer is currently not at the start of a line.</param>
        /// <param name="firstLine">if set to <see langword="true" /> the start position is on the first line.</param>
        /// <param name="formatProvider">The format provider.</param>
        public LayoutWriter(
            [NotNull] TextWriter writer,
            [CanBeNull] Layout layout = null,
            int startPosition = 0,
            bool firstLine = true,
            [CanBeNull] IFormatProvider formatProvider = null)
            : base(writer, formatProvider)
        {
            Contract.Requires(writer != null);
            _defaultLayout = _layout = Layout.Default.Apply(layout);
            Contract.Assert(_layout.IsFull);
            _position = startPosition;
            _firstLine = firstLine;
        }

        /// <summary>
        /// Gets the current horizontal position.
        /// </summary>
        /// <value>The position.</value>
        [PublicAPI]
        public int Position
        {
            get { return _position; }
        }

        /// <summary>
        /// Gets a value indicating whether the write point is currently on the first line of a paragraph..
        /// </summary>
        /// <value><see langword="true" /> if at the first line; otherwise, <see langword="false" />.</value>
        [PublicAPI]
        public bool FirstLine
        {
            get { return _firstLine; }
        }

        /// <summary>
        /// Gets or sets the layout.
        /// </summary>
        /// <value>The layout.</value>
        [PublicAPI]
        [NotNull]
        // ReSharper disable once CodeAnnotationAnalyzer
        public Layout Layout
        {
            get { return _layout; }
        }

        /// <summary>
        /// Applies the specified layout to the current layout, returning the new, combined layout.
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
        /// <returns>Layout.</returns>
        [PublicAPI]
        [NotNull]
        public Layout ApplyLayout(
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
            Optional<char> hyphenChar = default(Optional<char>))
        {
            using (Lock.LockAsync().Result)
                return (_layout = _layout.Apply(
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
                    hyphenChar));
        }

        /// <summary>
        /// Applies the layout to the current layout, returning the new, combined layout.
        /// </summary>
        /// <param name="layout">The layout.</param>
        /// <returns>Layout.</returns>
        [NotNull]
        [PublicAPI]
        public Layout ApplyLayout([CanBeNull] Layout layout)
        {
            if (layout == null) return _layout;
            using (Lock.LockAsync().Result)
                return (_layout = _layout.Apply(layout));
        }

        /// <summary>
        /// Gets the line chunks from a set of chunks.
        /// </summary>
        /// <param name="chunks">The chunks.</param>
        /// <param name="provider">The format provider.</param>
        /// <returns>An enumeration of chunks.</returns>
        [NotNull]
        private static IEnumerable<string> GetLineChunks(
            [NotNull] IEnumerable<string> chunks,
            [NotNull] IFormatProvider provider)
        {
            Contract.Requires(chunks != null);
            Contract.Requires(provider != null);
            StringBuilder word = new StringBuilder();
            bool lastCharR = false;
            foreach (char ch in chunks
                .Where(chunk => !string.IsNullOrEmpty(chunk))
                .SelectMany(ch => ch))
            {
                if (!char.IsWhiteSpace(ch))
                {
                    word.Append(ch);
                    continue;
                }
                if (word.Length > 0)
                {
                    yield return word.ToString();
                    word.Clear();
                }

                if (ch == '\n')
                {
                    // Skip '\n' after '\r'
                    if (!lastCharR)
                        yield return "\r";

                    lastCharR = false;
                    continue;
                }

                lastCharR = ch == '\r';

                yield return ch.ToString(provider);
            }
            if (word.Length > 0)
                yield return word.ToString();
        }

        /// <summary>
        /// Gets the lines from an enumeration of chunks.
        /// </summary>
        /// <param name="chunks">The chunks.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        [NotNull]
        private IEnumerable<Line> GetLines([NotNull] IEnumerable<string> chunks)
        {
            Contract.Requires(chunks != null);

            // Only grab the layout at the start of each line.
            Layout layout = _layout;

            // Create the first line, if we're part way through a line then we cannot align the remainder of the line.
            Line line = _position > 0
                ? new Line(layout, Alignment.None, _position, layout.Width.Value - layout.RightMarginSize.Value)
                : new Line(
                    layout,
                    layout.Alignment.Value,
                    _firstLine ? layout.FirstLineIndentSize.Value : layout.IndentSize.Value,
                    layout.Width.Value - layout.RightMarginSize.Value);
            _firstLine = false;

            bool splitWords = layout.SplitWords.Value;
            int hyphenate = layout.Hyphenate.Value ? 1 : 0;

            IEnumerator<string> ce = chunks.GetEnumerator();
            string word = null;
            bool newLine = false;
            do
            {
                // Check if we need to start a new line.
                if (newLine)
                {
                    // Close out existing line.
                    line.Finish(true, _firstLine);
                    yield return line;

                    // Start a new line
                    layout = _layout;
                    line = new Line(
                        layout,
                        layout.Alignment.Value,
                        _firstLine ? layout.FirstLineIndentSize.Value : layout.IndentSize.Value,
                        layout.Width.Value - layout.RightMarginSize.Value);
                    _firstLine = false;
                    splitWords = layout.SplitWords.Value;
                    hyphenate = layout.Hyphenate.Value ? 1 : 0;
                    newLine = false;
                }

                // If we don't have a word, get one.
                if (string.IsNullOrEmpty(word))
                {
                    if (!ce.MoveNext())
                    {
                        if (line.ChunkCount > 0)
                        {
                            line.Finish(false, _firstLine);
                            yield return line;

                            // Store the position for later
                            _position = line.Position;
                        }
                        else
                            _position = 0;

                        // No more words, so finish.
                        yield break;
                    }
                    word = ce.Current;
                    Contract.Assert(word != null);
                }

                char c = word[0];

                // Check if we're at the start of a line.
                bool split = splitWords;
                if (line.IsEmpty)
                {
                    // Skip spaces at the start of a line, if we have an alignment
                    if ((c == ' ') &&
                        (line.Alignment != Alignment.None))
                    {
                        word = null;
                        continue;
                    }

                    // We split this word if it's too long, as we're going from the start of a line.
                    split = true;
                }

                // Check for newline
                if (c == '\r')
                {
                    newLine = true;
                    _firstLine = true;
                    word = null;
                    continue;
                }

                int remaining = line.Remaining;

                // Check for tab
                if (c == '\t')
                {
                    if (remaining < 1)
                    {
                        // Process tab on a new line, as we're at the end of this one.
                        newLine = true;
                        continue;
                    }

                    int tabSize;
                    if (layout.TabStops.IsAssigned &&
                        layout.TabStops.Value != null)
                    {
                        int nextTab = layout.TabStops.Value.FirstOrDefault(t => t > line.Position);
                        tabSize = nextTab > line.Position
                            ? nextTab - line.Position
                            : layout.TabSize.Value;
                    }
                    else
                        tabSize = layout.TabSize.Value;

                    // Change word to spacer
                    word = new string(layout.TabChar.Value, tabSize);
                }

                // Append word if short enough.
                if (word.Length <= remaining)
                {
                    line.Add(word);
                    {
                        word = null;
                        continue;
                    }
                }

                // The word is too long to fit on the current line.
                if (split &&
                    (remaining > hyphenate))
                {
                    // Split the current word to fill remaining space
                    int splitPoint = remaining - hyphenate;
                    string part = word.Substring(0, splitPoint);
                    if (hyphenate > 0) part += layout.HyphenChar;
                    line.Add(part);
                    word = word.Substring(splitPoint);
                }

                // Start a new line
                newLine = true;
            } while (true);
        }

        /// <summary>
        /// Aligns the specified lines.
        /// </summary>
        /// <param name="lines">The lines.</param>
        /// <returns>An enumeration of terminated lines, laid out for writing.</returns>
        [NotNull]
        private IEnumerable<string> Align([NotNull] IEnumerable<Line> lines)
        {
            Contract.Requires(lines != null);
            StringBuilder lb = new StringBuilder(_layout.Width.Value);
            bool dontIndentFirstLine = _position > 0;
            foreach (Line line in lines)
            {
                Contract.Assert(line != null);
                char indentChar = line.Layout.IndentChar.Value;
                int indent;
                Queue<int> spacers = null;
                // Calculate indentation
                switch (line.Alignment)
                {
                    case Alignment.None:
                    case Alignment.Left:
                        indent = line.Start;
                        break;
                    case Alignment.Centre:
                        indent = (line.Start + line.End - line.Length) / 2;
                        break;
                    case Alignment.Right:
                        indent = line.End - line.Length;
                        break;
                    case Alignment.Justify:
                        indent = line.Start;
                        int remaining = line.Remaining;
                        if (remaining > 0)
                        {
                            decimal space = (decimal)(line.End - line.LastWordLength - line.Start) / remaining;
                            int o = (int)Math.Round(space / 2);
                            spacers = new Queue<int>(Enumerable.Range(0, remaining).Select(r => o + (int)(space * r)));
                        }
                        break;
                    default:
                        indent = 0;
                        break;
                }

                if (dontIndentFirstLine)
                    dontIndentFirstLine = false;
                else if (indent > 0)
                    lb.Append(indentChar, indent);

                int p = 0;
                foreach (string chunk in line)
                {
                    Contract.Assert(chunk != null);
                    lb.Append(chunk);

                    // Check if we have to add justification spaces
                    if (spacers == null) continue;
                    p += chunk.Length;

                    while ((spacers.Count > 0) &&
                           (spacers.Peek() <= p))
                    {
                        lb.Append(indentChar);
                        spacers.Dequeue();
                        p++;
                    }

                    // Check if justification is finished
                    if (spacers.Count < 1)
                        spacers = null;
                }

                // Add any remaining justification spaces
                if (line.Terminated)
                    lb.AppendLine();
                else if ((spacers != null) &&
                         (spacers.Count > 0))
                    lb.Append(indentChar, spacers.Count);

                yield return lb.ToString();
                lb.Clear();
            }
        }

        /// <summary>
        /// Gets a string represent of each chunk to write.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="builder">The builder.</param>
        /// <returns>A string representation of the chunk; otherwise <see langword="null" /> to skip.</returns>
        // ReSharper disable once CodeAnnotationAnalyzer
        protected override sealed string GetString(IFormatProvider formatProvider, FormatBuilder builder)
        {
            StringBuilder sb = new StringBuilder();
            if (formatProvider == null)
                formatProvider = FormatProvider;
            foreach (string line in Align(
                GetLines(
                    GetLineChunks(
                // ReSharper disable once AssignNullToNotNullAttribute
                        builder.Select(c => GetChunk(formatProvider, c)),
                        formatProvider))))
                sb.Append(line);
            return sb.ToString();
        }

        /// <summary>
        /// Gets a string represent of each chunk to write.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="chunk">The chunk.</param>
        /// <returns>A string representation of the chunk; otherwise <see langword="null"/> to skip.</returns>
        [CanBeNull]
        protected virtual string GetChunk([NotNull] IFormatProvider formatProvider, [NotNull] FormatChunk chunk)
        {
            Contract.Requires(formatProvider != null);
            Contract.Requires(chunk != null);
            Layout newLayout;
            /*
             * Check for supported control tags,
             * e.g. {Layout:w30}
             * or {Layout} to reset.
             */
            if (string.Equals(chunk.Tag, "layout", StringComparison.InvariantCultureIgnoreCase))
            {
                if (string.IsNullOrEmpty(chunk.Format))
                    _layout = _defaultLayout;
                else if (Layout.TryParse(chunk.Format, out newLayout))
                    _layout = _layout.Apply(newLayout);
                return null;
            }

            /*
             * Check for FormatBuilder's control chunks
             */
            if (chunk.IsControl)
            {
                newLayout = chunk.Value as Layout;
                if (newLayout != null)
                {
                    _layout = ReferenceEquals(newLayout, Layout.Default)
                        ? _defaultLayout
                        : _layout.Apply(newLayout);
                    return null;
                }
            }

            return chunk.ToString(null, formatProvider);
        }
    }
}