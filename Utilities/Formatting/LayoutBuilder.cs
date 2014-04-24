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
                        if (chunk == null)
                            continue;
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
            [PublicAPI]
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
        /// Gets the initial layout to use when resetting the layout.
        /// </summary>
        /// <value>
        /// The initial layout.
        /// </value>
        [NotNull]
        [PublicAPI]
        public Layout InitialLayout { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutBuilder" /> class.
        /// </summary>
        /// <param name="layout">The layout.</param>
        public LayoutBuilder([CanBeNull] Layout layout = null)
        {
            InitialLayout = Layout.Default.Apply(layout);
            Contract.Assert(InitialLayout.IsFull);
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <param name="makeReadonly">If set to <see langword="true"/>, the returned builder will be readonly.</param>
        /// <returns>
        /// A shallow copy of this builder.
        /// </returns>
        public override FormatBuilder Clone(bool makeReadonly = false)
        {
            if (IsReadonly)
                return this;

            LayoutBuilder layoutBuilder = new LayoutBuilder(InitialLayout);
            layoutBuilder.Append(this);
            if (makeReadonly)
                layoutBuilder.MakeReadonly();
            return layoutBuilder;
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
            [NotNull] [InstantHandle] IEnumerable<FormatChunk> chunks,
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
                    if (word.Length > 0)
                    {
                        words.Add(word.ToString());
                        word.Clear();
                    }

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
            Layout nextLayout = InitialLayout;
            Layout layout = nextLayout;

            // Create the first line, if we're part way through a line then we cannot align the remainder of the line.
            Line line = position > 0
                ? new Line(
                    layout,
                    Alignment.None,
                    position,
                    layout.Width.Value - layout.RightMarginSize.Value,
                    false)
                : new Line(
                    layout,
                    layout.Alignment.Value,
                    layout.FirstLineIndentSize.Value,
                    layout.Width.Value - layout.RightMarginSize.Value,
                    true);
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
                    layout = nextLayout;
                    line = new Line(
                        layout,
                        layout.Alignment.Value,
                        firstLine ? layout.FirstLineIndentSize.Value : layout.IndentSize.Value,
                        layout.Width.Value - layout.RightMarginSize.Value,
                        firstLine);
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

                        FormatChunk controlChunk = controlEnumerator.Current;
                        Contract.Assert(controlChunk != null);

                        // If the control chunk is a layout chunk, we need to get the layout
                        if (string.Equals(controlChunk.Tag, "!layout", StringComparison.InvariantCultureIgnoreCase))
                        {
                            Layout newLayout = controlChunk.Value as Layout;

                            if (newLayout != null)
                                nextLayout = ReferenceEquals(newLayout, Layout.Default)
                                    ? InitialLayout
                                    : nextLayout.Apply(newLayout);
                            else if (string.IsNullOrEmpty(controlChunk.Format))
                                nextLayout = InitialLayout;
                            else if (Layout.TryParse(controlChunk.Format, out newLayout))
                                nextLayout = nextLayout.Apply(newLayout);

                            if (!line.IsEmpty) continue;

                            // If the line is empty, we can create a new line using the new layout

                            firstLine = line.IsFirstLine;

                            // Start a new line
                            layout = nextLayout;
                            line = new Line(
                                layout,
                                layout.Alignment.Value,
                                firstLine ? layout.FirstLineIndentSize.Value : layout.IndentSize.Value,
                                layout.Width.Value - layout.RightMarginSize.Value,
                                firstLine);
                            firstLine = false;
                            splitWords = layout.SplitWords.Value;
                            hyphenate = layout.Hyphenate.Value ? 1 : 0;
                        }
                        else
                            line.AddControl(controlChunk);

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
        private IEnumerable<FormatChunk> Align([NotNull] [InstantHandle] IEnumerable<Line> lines, ref int position)
        {
            Contract.Requires(lines != null);
            StringBuilder lb = new StringBuilder(InitialLayout.Width.Value);
            bool dontIndentFirstLine = position > 0;

            List<FormatChunk> chunks = new List<FormatChunk>();

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
                IEnumerator<FormatChunk> controlEnumerator = line.Controls.GetEnumerator();
                foreach (string chunk in line)
                {
                    if (string.IsNullOrEmpty(chunk))
                    {
                        // We got a control chunk, so need to split line
                        if (lb.Length > 0)
                        {
                            chunks.Add(FormatChunk.Create(lb.ToString()));
                            lb.Clear();
                        }
                        bool success = controlEnumerator.MoveNext();
                        Contract.Assert(success);
                        Contract.Assert(controlEnumerator.Current != null);
                        chunks.Add(controlEnumerator.Current);
                        continue;
                    }

                    lb.Append(chunk);
                    p += chunk.Length;

                    // Check if we have to add justification spaces
                    if (spacers == null) continue;

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

                position = p + indent;

                // Add any remaining justification spaces
                if (line.Terminated)
                {
                    switch (line.Layout.WrapMode.Value)
                    {
                        case LayoutWrapMode.NewLine:
                            lb.AppendLine();
                            break;
                        case LayoutWrapMode.NewLineOnShort:
                            if (position < line.Layout.Width.Value)
                                lb.AppendLine();
                            break;
                        case LayoutWrapMode.PadToWrap:
                            lb.Append(line.Layout.IndentChar.Value, line.Layout.Width.Value - position);
                            break;
                        default:
                            Contract.Assert(false);
                            break;
                    }
                }
                else if ((spacers != null) &&
                         (spacers.Count > 0))
                    lb.Append(indentChar, spacers.Count);

                if (lb.Length > 0)
                {
                    chunks.Add(FormatChunk.Create(lb.ToString()));
                    lb.Clear();
                }
                lb.Clear();
            }
            return chunks;
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        [PublicAPI]
        public int WriteTo([CanBeNull] TextWriter writer, int position)
        {
            if (writer == null) return position;
            return WriteTo(this, writer, "G", null, position);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        [PublicAPI]
        public int WriteTo([CanBeNull] TextWriter writer, [CanBeNull] IFormatProvider formatProvider, int position)
        {
            if (writer == null) return position;
            return WriteTo(this, writer, "G", formatProvider, position);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="position">The position.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        [PublicAPI]
        public int WriteTo(
            [CanBeNull] TextWriter writer,
            [CanBeNull] string format,
            [CanBeNull] IFormatProvider formatProvider,
            int position,
            [CanBeNull] params object[] values)
        {
            if (writer == null) return position;
            if (format == null)
                format = "G";
            return WriteTo(
                values != null && values.Length > 0
                    ? Resolve(this, values)
                    : this,
                writer,
                format,
                formatProvider,
                position);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="values">The values.</param>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        [PublicAPI]
        public int WriteTo(
            [CanBeNull] TextWriter writer,
            [CanBeNull] string format,
            [CanBeNull] IFormatProvider formatProvider,
            [CanBeNull] [InstantHandle] IEnumerable<object> values,
            int position)
        {
            if (writer == null) return position;
            if (format == null)
                format = "G";
            if (values != null)
            {
                object[] vArray = values.ToArray();
                if (vArray.Length > 0)
                {
                    return WriteTo(
                        Resolve(this, vArray),
                        writer,
                        format,
                        formatProvider,
                        position);
                }
            }

            return WriteTo(
                this,
                writer,
                format,
                formatProvider,
                position);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="values">The values.</param>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        [PublicAPI]
        public int WriteTo(
            [CanBeNull] TextWriter writer,
            [CanBeNull] string format,
            [CanBeNull] IFormatProvider formatProvider,
            [CanBeNull] IReadOnlyDictionary<string, object> values,
            int position)
        {
            if (writer == null) return position;
            if (format == null)
                format = "G";
            return WriteTo(
                values != null && values.Count > 0
                    ? Resolve(this, values)
                    : this,
                writer,
                format,
                formatProvider,
                position);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="resolver">The resolver.</param>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        [PublicAPI]
        public int WriteTo(
            [CanBeNull] TextWriter writer,
            [CanBeNull] string format,
            [CanBeNull] IFormatProvider formatProvider,
            [CanBeNull] [InstantHandle] Func<FormatChunk, Optional<object>> resolver,
            int position)
        {
            if (writer == null) return position;
            if (format == null)
                format = "G";
            return WriteTo(
                resolver != null
                    ? Resolve(this, resolver)
                    : this,
                writer,
                format,
                formatProvider,
                position);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="chunks"></param>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format passed to each chunk.</param>
        /// <param name="formatProvider">The format provider.</param>
        [PublicAPI]
        protected override void WriteToInternal(
            IEnumerable<FormatChunk> chunks,
            TextWriter writer,
            string format,
            IFormatProvider formatProvider)
        {
            WriteTo(chunks, writer, format, formatProvider, 0);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="chunks">The chunks.</param>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format passed to each chunk.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        [PublicAPI]
        protected virtual int WriteTo(
            [NotNull] IEnumerable<FormatChunk> chunks,
            [NotNull] TextWriter writer,
            [NotNull] string format,
            [CanBeNull] IFormatProvider formatProvider,
            int position)
        {
            bool writeTags = string.Equals(format, "f", StringComparison.InvariantCultureIgnoreCase);

            if (position < 0) position = 0;
            StringBuilder sb = new StringBuilder();
            // Get sections based on control codes
            foreach (FormatChunk chunk in
                    Align(GetLines(GetLineChunks(chunks, format, formatProvider), position), ref position))
            {
                Contract.Assert(chunk != null);

                if (chunk.IsControl &&
                    !writeTags)
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

            return position;
        }
        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        [NotNull]
        [PublicAPI]
        public Task<int> WriteToAsync([CanBeNull] TextWriter writer, int position)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            if (writer == null) return Task.FromResult(position);
            return WriteToAsync(this, writer, "G", null, position);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        [NotNull]
        [PublicAPI]
        public Task<int> WriteToAsync([CanBeNull] TextWriter writer, [CanBeNull] IFormatProvider formatProvider, int position)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            if (writer == null) return Task.FromResult(position);
            return WriteToAsync(this, writer, "G", formatProvider, position);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="position">The position.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        [NotNull]
        [PublicAPI]
        public Task<int> WriteToAsync(
            [CanBeNull] TextWriter writer,
            [CanBeNull] string format,
            [CanBeNull] IFormatProvider formatProvider,
            int position,
            [CanBeNull] params object[] values)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            if (writer == null) return Task.FromResult(position);
            if (format == null)
                format = "G";
            return WriteToAsync(
                values != null && values.Length > 0
                    ? Resolve(this, values)
                    : this,
                writer,
                format,
                formatProvider,
                position);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="values">The values.</param>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        [NotNull]
        [PublicAPI]
        public Task<int> WriteToAsync(
            [CanBeNull] TextWriter writer,
            [CanBeNull] string format,
            [CanBeNull] IFormatProvider formatProvider,
            [CanBeNull] [InstantHandle] IEnumerable<object> values,
            int position)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            if (writer == null) return Task.FromResult(position);
            if (format == null)
                format = "G";
            if (values != null)
            {
                object[] vArray = values.ToArray();
                if (vArray.Length > 0)
                {
                    return WriteToAsync(
                        Resolve(this, vArray),
                        writer,
                        format,
                        formatProvider,
                        position);
                }
            }

            return WriteToAsync(
                this,
                writer,
                format,
                formatProvider,
                        position);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="values">The values.</param>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        [NotNull]
        [PublicAPI]
        public Task<int> WriteToAsync(
            [CanBeNull] TextWriter writer,
            [CanBeNull] string format,
            [CanBeNull] IFormatProvider formatProvider,
            [CanBeNull] IReadOnlyDictionary<string, object> values,
            int position)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            if (writer == null) return Task.FromResult(position);
            if (format == null)
                format = "G";
            return WriteToAsync(
                values != null && values.Count > 0
                    ? Resolve(this, values)
                    : this,
                writer,
                format,
                formatProvider,
                position);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="resolver">The resolver.</param>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        [NotNull]
        [PublicAPI]
        public Task<int> WriteToAsync(
            [CanBeNull] TextWriter writer,
            [CanBeNull] string format,
            [CanBeNull] IFormatProvider formatProvider,
            [CanBeNull]  [InstantHandle] Func<FormatChunk, Optional<object>> resolver,
            int position)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            if (writer == null) return Task.FromResult(position);
            if (format == null)
                format = "G";
            return WriteToAsync(
                resolver != null
                    ? Resolve(this, resolver)
                    : this,
                writer,
                format,
                formatProvider,
                position);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="chunks"></param>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format passed to each chunk.</param>
        /// <param name="formatProvider">The format provider.</param>
        [PublicAPI]
        protected override Task WriteToInternalAsync(
            IEnumerable<FormatChunk> chunks,
            TextWriter writer,
            string format,
            IFormatProvider formatProvider)
        {
            return WriteToAsync(chunks, writer, format, formatProvider, 0);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="chunks">The chunks.</param>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format passed to each chunk.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        [NotNull]
        [PublicAPI]
        protected virtual async Task<int> WriteToAsync(
            [NotNull] IEnumerable<FormatChunk> chunks,
            [NotNull] TextWriter writer,
            [NotNull] string format,
            [CanBeNull] IFormatProvider formatProvider,
            int position)
        {
            bool writeTags = string.Equals(format, "f", StringComparison.InvariantCultureIgnoreCase);

            if (position < 0) position = 0;
            StringBuilder sb = new StringBuilder();
            // Get sections based on control codes
            foreach (FormatChunk chunk in
                    Align(GetLines(GetLineChunks(chunks, format, formatProvider), position), ref position))
            {
                Contract.Assert(chunk != null);

                if (chunk.IsControl &&
                    !writeTags)
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

            return position;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="position">The start position.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        [NotNull]
        [PublicAPI]
        public string ToString(int position)
        {
            return ToString(position, null, null);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="position">The start position.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        [NotNull]
        [PublicAPI]
        public string ToString(int position, [CanBeNull] IFormatProvider formatProvider)
        {
            return ToString(position, null, formatProvider);
        }

        /// <summary>
        /// To the string.
        /// </summary>
        /// <param name="position">The start position.</param>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>System.String.</returns>
        [NotNull]
        [PublicAPI]
        public virtual string ToString(
            int position,
            [CanBeNull] string format,
            [CanBeNull] IFormatProvider formatProvider)
        {
            using (StringWriter writer = new StringWriter())
            {
                WriteTo(writer, format, formatProvider, position);
                return writer.ToString();
            }
        }
    }
}