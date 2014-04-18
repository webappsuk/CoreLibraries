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
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// Lays out text written to the underlying text writer.
    /// </summary>
    public class LayoutBuilder : FormatBuilder
    {
        /// <summary>
        /// Holds an unaligned line.
        /// </summary>
        private class Line : IEnumerable<string>
        {
            [NotNull]
            private readonly List<string> _chunks = new List<string>();

            [NotNull]
            private readonly List<FormatChunk> _controls = new List<FormatChunk>();

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
            /// Adds the specified chunk to this line.
            /// </summary>
            /// <param name="chunk">The chunk.</param>
            [PublicAPI]
            public void AddControl([NotNull] FormatChunk chunk)
            {
                Contract.Requires(chunk != null);
                Contract.Requires(chunk.IsControl);
                _chunks.Add(null);
                _controls.Add(chunk);
            }

            /// <summary>
            /// Gets the controls.
            /// </summary>
            /// <value>The controls.</value>
            [NotNull]
            public IEnumerable<FormatChunk> Controls
            {
                get { return _controls; }
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
        private readonly Layout _initialLayout;

        /// <summary>
        /// The current layout
        /// </summary>
        [NotNull]
        private Layout _layout;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutBuilder" /> class.
        /// </summary>
        /// <param name="layout">The layout.</param>
        public LayoutBuilder(
            [CanBeNull] Layout layout = null)
        {
            _initialLayout = _layout = Layout.Default.Apply(layout);
            Contract.Assert(_layout.IsFull);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutBuilder" /> class.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="layout">The layout.</param>
        public LayoutBuilder(
            [CanBeNull] IEnumerable<object> values,
            [CanBeNull] Layout layout = null)
            : base(values)
        {
            _initialLayout = _layout = Layout.Default.Apply(layout);
            Contract.Assert(_layout.IsFull);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutBuilder" /> class.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="layout">The layout.</param>
        public LayoutBuilder(
            [CanBeNull] IReadOnlyDictionary<string, object> values,
            [CanBeNull] Layout layout = null)
            : base(values)
        {
            _initialLayout = _layout = Layout.Default.Apply(layout);
            Contract.Assert(_layout.IsFull);
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
            return (_layout = _layout.Apply(layout));
        }

        /// <summary>
        /// Gets the line chunks from a set of chunks.
        /// </summary>
        /// <param name="chunks">The chunks.</param>
        /// <param name="format">The format.</param>
        /// <param name="provider">The format provider.</param>
        /// <returns>An enumeration of chunks.</returns>
        [NotNull]
        private static Tuple<IEnumerable<string>, IEnumerable<FormatChunk>> GetLineChunks(
            [NotNull] IEnumerable<FormatChunk> chunks,
            [CanBeNull] string format,
            [CanBeNull] IFormatProvider provider)
        {
            Contract.Requires(chunks != null);
            List<string> words = new List<string>();
            List<FormatChunk> controlChunks = new List<FormatChunk>();

            StringBuilder word = new StringBuilder();
            bool lastCharR = false;
            foreach (FormatChunk chunk in chunks)
            {
                Contract.Assert(chunk != null);
                if (chunk.IsControl)
                {
                    // Use null to indicate location of a control chunks
                    words.Add(null);
                    controlChunks.Add(chunk);
                    continue;
                }

                string chunk1 = chunk.ToString(format, provider);
                if (string.IsNullOrEmpty(chunk1)) continue;

                foreach (char ch in chunk1)
                {
                    if (!char.IsWhiteSpace(ch))
                    {
                        word.Append(ch);
                        continue;
                    }
                    if (word.Length > 0)
                    {
                        words.Add(word.ToString());
                        word.Clear();
                    }

                    if (ch == '\n')
                    {
                        // Skip '\n' after '\r'
                        if (!lastCharR)
                            words.Add("\r");

                        lastCharR = false;
                        continue;
                    }

                    lastCharR = ch == '\r';

                    words.Add(ch.ToString(provider));
                }
            }
            if (word.Length > 0)
                words.Add(word.ToString());

            return new Tuple<IEnumerable<string>, IEnumerable<FormatChunk>>(words, controlChunks);
        }

        /// <summary>
        /// Gets the lines from an enumeration of chunks.
        /// </summary>
        /// <param name="chunks">The chunks.</param>
        /// <param name="position">The position.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        [NotNull]
        private IEnumerable<Line> GetLines(
            [NotNull] Tuple<IEnumerable<string>, IEnumerable<FormatChunk>> chunks,
            int position)
        {
            Contract.Requires(chunks != null);
            Contract.Requires(chunks.Item1 != null);
            Contract.Requires(chunks.Item2 != null);

            // Only grab the layout at the start of each line.
            Layout layout = _layout;

            // Create the first line, if we're part way through a line then we cannot align the remainder of the line.
            Line line = position > 0
                ? new Line(
                    layout,
                    Alignment.None,
                    position,
                    layout.Width.Value - layout.RightMarginSize.Value)
                : new Line(
                    layout,
                    layout.Alignment.Value,
                    layout.FirstLineIndentSize.Value,
                    layout.Width.Value - layout.RightMarginSize.Value);
            bool firstLine = false;

            bool splitWords = layout.SplitWords.Value;
            int hyphenate = layout.Hyphenate.Value ? 1 : 0;

            IEnumerator<string> chunkEnumerator = chunks.Item1.GetEnumerator();
            IEnumerator<FormatChunk> controlEnumerator = chunks.Item2.GetEnumerator();

            string word = null;
            bool newLine = false;
            do
            {
                // Check if we need to start a new line.
                if (newLine)
                {
                    // Close out existing line.
                    line.Finish(true, firstLine);
                    yield return line;

                    // Start a new line
                    layout = _layout;
                    line = new Line(
                        layout,
                        layout.Alignment.Value,
                        firstLine ? layout.FirstLineIndentSize.Value : layout.IndentSize.Value,
                        layout.Width.Value - layout.RightMarginSize.Value);
                    firstLine = false;
                    splitWords = layout.SplitWords.Value;
                    hyphenate = layout.Hyphenate.Value ? 1 : 0;
                    newLine = false;
                }

                // If we don't have a word, get one.
                if (string.IsNullOrEmpty(word))
                    do
                    {
                        if (!chunkEnumerator.MoveNext())
                        {
                            if (line.ChunkCount > 0)
                            {
                                line.Finish(false, false);
                                yield return line;
                            }

                            // No more words, so finish.
                            yield break;
                        }
                        word = chunkEnumerator.Current;

                        // Check if we have a control marker
                        if (!string.IsNullOrEmpty(word)) break;

                        bool success = controlEnumerator.MoveNext();
                        Contract.Assert(success);
                        Contract.Assert(controlEnumerator.Current != null);
                        line.AddControl(controlEnumerator.Current);
                    } while (true);

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
                    firstLine = true;
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
        /// <param name="position">The position.</param>
        /// <returns>An enumeration of terminated lines, laid out for writing.</returns>
        [NotNull]
        private IEnumerable<FormatChunk> Align([NotNull] IEnumerable<Line> lines, int position)
        {
            Contract.Requires(lines != null);
            StringBuilder lb = new StringBuilder(_layout.Width.Value);
            bool dontIndentFirstLine = position > 0;
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
                            decimal space = (decimal) (line.End - line.LastWordLength - line.Start) / remaining;
                            int o = (int) Math.Round(space / 2);
                            spacers = new Queue<int>(Enumerable.Range(0, remaining).Select(r => o + (int) (space * r)));
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
                IEnumerator<FormatChunk> controlEnumerator = line.Controls.GetEnumerator();
                foreach (string chunk in line)
                {
                    if (string.IsNullOrEmpty(chunk))
                    {
                        // We got a control chunk, so need to split line
                        if (lb.Length > 0)
                        {
                            yield return FormatChunk.Create(lb.ToString());
                            lb.Clear();
                        }
                        bool success = controlEnumerator.MoveNext();
                        Contract.Assert(success);
                        Contract.Assert(controlEnumerator.Current != null);
                        yield return controlEnumerator.Current;
                        continue;
                    }

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

                if (lb.Length > 0)
                {
                    yield return FormatChunk.Create(lb.ToString());
                    lb.Clear();
                }
                lb.Clear();
            }
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format passed to each chunk.</param>
        /// <param name="formatProvider">The format provider.</param>
        [PublicAPI]
        public override void WriteTo(TextWriter writer, string format = null, IFormatProvider formatProvider = null)
        {
            WriteTo(writer, format, formatProvider, 0);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format passed to each chunk.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="position">The position.</param>
        [PublicAPI]
        public virtual void WriteTo(
            [CanBeNull] TextWriter writer,
            [CanBeNull] string format,
            [CanBeNull] IFormatProvider formatProvider,
            int position)
        {
            if (writer == null) return;
            StringBuilder sb = new StringBuilder();
            // Get sections based on control codes
            foreach (
                FormatChunk chunk in
                    Align(GetLines(GetLineChunks(this, format, formatProvider), position), position))
            {
                Contract.Assert(chunk != null);
                if (chunk.IsControl)
                {
                    if (sb.Length > 0)
                    {
                        writer.Write(sb.ToString());
                        sb.Clear();
                    }
                    OnControlChunk(chunk, writer, format, formatProvider);
                }
                else
                    sb.Append(chunk.ToString(format, formatProvider));
            }

            if (sb.Length > 0)
                writer.Write(sb.ToString());

            // Restore the initial layout, in case someone tries to write us out again.
            _layout = _initialLayout;
        }


        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" /> asynchronously.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format passed to each chunk.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>An awaitable task.</returns>
        [PublicAPI]
        public override Task WriteToAsync(
            TextWriter writer,
            string format = null,
            IFormatProvider formatProvider = null)
        {
            return WriteToAsync(writer, format, formatProvider, 0);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" /> asynchronously.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format passed to each chunk.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="position">The position.</param>
        /// <returns>An awaitable task.</returns>
        [PublicAPI]
        [NotNull]
        public virtual async Task WriteToAsync(
            [CanBeNull] TextWriter writer,
            [CanBeNull] string format,
            [CanBeNull] IFormatProvider formatProvider,
            int position)
        {
            if (writer == null) return;

            StringBuilder sb = new StringBuilder();
            // Get sections based on control codes
            foreach (
                FormatChunk chunk in
                    Align(GetLines(GetLineChunks(this, format, formatProvider), position), position))
            {
                Contract.Assert(chunk != null);
                if (chunk.IsControl)
                {
                    if (sb.Length > 0)
                    {
                        // ReSharper disable once PossibleNullReferenceException
                        await writer.WriteAsync(sb.ToString());
                        sb.Clear();
                    }
                    OnControlChunk(chunk, writer, format, formatProvider);
                }
                else
                    sb.Append(chunk.ToString(format, formatProvider));
            }

            if (sb.Length > 0)
                // ReSharper disable once PossibleNullReferenceException
                await writer.WriteAsync(sb.ToString());

            // Restore the initial layout, in case someone tries to write us out again.
            _layout = _initialLayout;
        }

        /// <summary>
        /// Called when a control chunk is encountered.
        /// </summary>
        /// <param name="controlChunk">The control chunk.</param>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        // ReSharper disable once CodeAnnotationAnalyzer
        protected override void OnControlChunk(
            FormatChunk controlChunk,
            TextWriter writer,
            string format,
            IFormatProvider formatProvider)
        {
            Layout newLayout;
            if (string.Equals(controlChunk.Tag, "layout", StringComparison.InvariantCultureIgnoreCase))
            {
                if (string.IsNullOrEmpty(controlChunk.Format))
                    _layout = _initialLayout;
                else if (Layout.TryParse(controlChunk.Format, out newLayout))
                    _layout = _layout.Apply(newLayout);
                return;
            }

            newLayout = controlChunk.Value as Layout;
            if (newLayout == null) return;

            _layout = ReferenceEquals(newLayout, Layout.Default)
                ? _initialLayout
                : _layout.Apply(newLayout);
        }
    }
}