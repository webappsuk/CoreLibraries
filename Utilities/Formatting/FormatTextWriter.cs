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
using System.IO;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// Wraps a <see cref="TextWriter"/>, providing position tracking and synchronized writing with a <see cref="Layout"/>.
    /// </summary>
    /// <remarks>
    /// This is not inherently thread safe, to make thread safe use a synchronization wrapper.
    /// </remarks>
    [PublicAPI]
    public sealed class FormatTextWriter : SerialTextWriter, ILayoutTextWriter
    {
        /// <summary>
        /// The underlying writer.
        /// </summary>
        [NotNull]
        private readonly UnderlyingFormatTextWriter _writer;

        /// <summary>
        /// Gets the width of the console.
        /// </summary>
        /// <value>The width of the console.</value>
        public int Width
        {
            get { return _writer.Width; }
        }

        /// <summary>
        /// Gets or sets the current horizontal position.
        /// </summary>
        /// <value>The position.</value>
        [PublicAPI]
        public int Position
        {
            get { return _writer.Position; }
            set { Context.Invoke(() => _writer.Position = value); }
        }

        /// <summary>
        /// Gets a value indicating whether the writer automatically wraps on reaching <see cref="Width" />.
        /// </summary>
        /// <value><see langword="true" /> if the writer automatically wraps; otherwise, <see langword="false" />.</value>
        public bool AutoWraps
        {
            get { return _writer.AutoWraps; }
        }

        /// <summary>
        /// Gets the current Layout.
        /// </summary>
        /// <value>The Layout.</value>
        [PublicAPI]
        [NotNull]
        public Layout Layout
        {
            get { return _writer.Builder.InitialLayout; }
        }

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FormatTextWriter" /> class.
        /// </summary>
        /// <param name="writer">The out writer.</param>
        /// <exception cref="System.InvalidOperationException">Cannot wrap an ILayoutTextWriter in a FormatTextWriter as this can cause issues with position tracking.</exception>
        public FormatTextWriter([NotNull] TextWriter writer)
            : base(new UnderlyingFormatTextWriter(writer, new FormatBuilder(), 0))
        {
            Contract.Requires(writer != null);
            _writer = (UnderlyingFormatTextWriter) Writer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatTextWriter" /> class.
        /// </summary>
        /// <param name="writer">The out writer.</param>
        /// <param name="defaultLayout">The default layout.</param>
        /// <param name="startPosition">The starting horizontal position.</param>
        public FormatTextWriter(
            [NotNull] TextWriter writer,
            [CanBeNull] Layout defaultLayout,
            int startPosition = 0)
            : base(new UnderlyingFormatTextWriter(writer, new FormatBuilder(defaultLayout), startPosition))
        {
            Contract.Requires(writer != null);
            _writer = (UnderlyingFormatTextWriter) Writer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatTextWriter" /> class.
        /// </summary>
        /// <param name="writer">The out writer.</param>
        /// <param name="width">The width.</param>
        /// <param name="indentSize">Size of the indent.</param>
        /// <param name="rightMarginSize">Size of the right margin.</param>
        /// <param name="indentChar">The indent character.</param>
        /// <param name="firstLineIndentSize">First size of the line indent.</param>
        /// <param name="tabStops">The tab stops.</param>
        /// <param name="tabSize">Size of the tab.</param>
        /// <param name="tabChar">The tab character.</param>
        /// <param name="alignment">The alignment.</param>
        /// <param name="splitLength">The split length.</param>
        /// <param name="hyphenate">The hyphenate.</param>
        /// <param name="hyphenChar">The hyphen character.</param>
        /// <param name="wrapMode">The wrap mode.</param>
        /// <param name="startPosition">The starting horizontal position.</param>
        public FormatTextWriter(
            [NotNull] TextWriter writer,
            Optional<int> width = default(Optional<int>),
            Optional<int> indentSize = default(Optional<int>),
            Optional<int> rightMarginSize = default(Optional<int>),
            Optional<char> indentChar = default(Optional<char>),
            Optional<int> firstLineIndentSize = default(Optional<int>),
            Optional<IEnumerable<int>> tabStops = default(Optional<IEnumerable<int>>),
            Optional<byte> tabSize = default(Optional<byte>),
            Optional<char> tabChar = default(Optional<char>),
            Optional<Alignment> alignment = default(Optional<Alignment>),
            Optional<byte> splitLength = default(Optional<byte>),
            Optional<bool> hyphenate = default(Optional<bool>),
            Optional<char> hyphenChar = default(Optional<char>),
            Optional<LayoutWrapMode> wrapMode = default(Optional<LayoutWrapMode>),
            int startPosition = 0)
            : base(new UnderlyingFormatTextWriter(
                       writer,
                       new FormatBuilder(
                           width,
                           indentSize,
                           rightMarginSize,
                           indentChar,
                           firstLineIndentSize,
                           tabStops,
                           tabSize,
                           tabChar,
                           alignment,
                           splitLength,
                           hyphenate,
                           hyphenChar,
                           wrapMode),
                       startPosition))
        {
            Contract.Requires(writer != null);
            _writer = (UnderlyingFormatTextWriter) Writer;
        }
        #endregion

        /// <summary>
        /// Sets the layout.
        /// </summary>
        /// <param name="newLayout">The new layout.</param>
        /// <returns>An awaitable task that returns the existing layout.</returns>
        [PublicAPI]
        [NotNull]
        public Layout ApplyLayout([CanBeNull] Layout newLayout)
        {
            if (newLayout == null) return _writer.Builder.InitialLayout;
            // ReSharper disable once AssignNullToNotNullAttribute
            return Context.Invoke(
                () =>
                {
                    if (newLayout == _writer.Builder.InitialLayout) return newLayout;

                    Layout existing = _writer.Builder.InitialLayout;
                    _writer.Builder = new FormatBuilder(
                        existing.Apply(
                            newLayout.Width,
                            newLayout.IndentSize,
                            newLayout.RightMarginSize,
                            newLayout.IndentChar,
                            newLayout.FirstLineIndentSize,
                            newLayout.TabStops,
                            newLayout.TabSize,
                            newLayout.TabChar,
                            newLayout.Alignment,
                            newLayout.SplitLength,
                            newLayout.Hyphenate,
                            newLayout.HyphenChar,
                            newLayout.WrapMode));
                    return existing;
                });
        }

        /// <summary>
        /// Sets the layout (if outputting to a layout writer).
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
        /// <param name="splitLength">if set to <see langword="true" /> then words will split across lines.</param>
        /// <param name="hyphenate">if set to <see langword="true" /> [hyphenate].</param>
        /// <param name="hyphenChar">The hyphenation character.</param>
        /// <param name="wrapMode">The line wrap mode.</param>
        /// <returns>An awaitable task that returns the existing layout.</returns>
        [PublicAPI]
        [NotNull]
        public Layout ApplyLayout(
            Optional<int> width = default(Optional<int>),
            Optional<int> indentSize = default(Optional<int>),
            Optional<int> rightMarginSize = default(Optional<int>),
            Optional<char> indentChar = default(Optional<char>),
            Optional<int> firstLineIndentSize = default(Optional<int>),
            Optional<IEnumerable<int>> tabStops = default(Optional<IEnumerable<int>>),
            Optional<byte> tabSize = default(Optional<byte>),
            Optional<char> tabChar = default(Optional<char>),
            Optional<Alignment> alignment = default(Optional<Alignment>),
            Optional<byte> splitLength = default(Optional<byte>),
            Optional<bool> hyphenate = default(Optional<bool>),
            Optional<char> hyphenChar = default(Optional<char>),
            Optional<LayoutWrapMode> wrapMode = default(Optional<LayoutWrapMode>))
        {
            if (!width.IsAssigned &&
                !indentSize.IsAssigned &&
                !rightMarginSize.IsAssigned &&
                !indentChar.IsAssigned &&
                !firstLineIndentSize.IsAssigned &&
                !tabStops.IsAssigned &&
                !tabSize.IsAssigned &&
                !tabChar.IsAssigned &&
                !alignment.IsAssigned &&
                !splitLength.IsAssigned &&
                !hyphenate.IsAssigned &&
                !hyphenChar.IsAssigned &&
                !wrapMode.IsAssigned)
                return _writer.Builder.InitialLayout;

            // ReSharper disable once AssignNullToNotNullAttribute
            return Context.Invoke(
                () =>
                {
                    Layout existing = _writer.Builder.InitialLayout;
                    _writer.Builder =
                        new FormatBuilder(
                            existing.Apply(
                                width,
                                indentSize,
                                rightMarginSize,
                                indentChar,
                                firstLineIndentSize,
                                tabStops,
                                tabSize,
                                tabChar,
                                alignment,
                                splitLength,
                                hyphenate,
                                hyphenChar,
                                wrapMode));
                    return existing;
                });
        }

        /// <summary>
        /// Wraps a <see cref="TextWriter"/>, providing position tracking and synchronized writing with a <see cref="Layout"/>.
        /// </summary>
        /// <remarks>
        /// This is not inherently thread safe, to make thread safe use a synchronization wrapper.
        /// </remarks>
        [PublicAPI]
        private sealed class UnderlyingFormatTextWriter : TextWriter, ILayoutTextWriter
        {
            /// <summary>
            /// The underlying writer
            /// </summary>
            [NotNull]
            private readonly TextWriter _writer;

            /// <summary>
            /// The builder
            /// </summary>
            [NotNull]
            public FormatBuilder Builder;

            /// <summary>
            /// Gets the width of the console.
            /// </summary>
            /// <value>The width of the console.</value>
            public int Width
            {
                get { return Builder.InitialLayout.Width.Value; }
            }

            /// <summary>
            /// Gets or sets the current horizontal position.
            /// </summary>
            /// <value>The position.</value>
            public int Position { get; set; }

            /// <summary>
            /// Gets a value indicating whether the writer automatically wraps on reaching <see cref="Width" />.
            /// </summary>
            /// <value><see langword="true" /> if the writer automatically wraps; otherwise, <see langword="false" />.</value>
            public bool AutoWraps
            {
                get { return false; }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="UnderlyingFormatTextWriter" /> class.
            /// </summary>
            /// <param name="writer">The writer.</param>
            /// <param name="builder">The builder.</param>
            /// <param name="startPosition">The start position.</param>
            /// <exception cref="System.InvalidOperationException">Cannot wrap an ILayoutTextWriter in a FormatTextWriter as this can cause issues with position tracking.</exception>
            public UnderlyingFormatTextWriter(
                [NotNull] TextWriter writer,
                [NotNull] FormatBuilder builder,
                int startPosition)
            {
                Contract.Requires(writer != null);
                Contract.Requires(builder != null);
                if (writer is ILayoutTextWriter)
                    throw new InvalidOperationException(
                        "Cannot wrap an ILayoutTextWriter in a FormatTextWriter as this can cause issues with position tracking.");
                _writer = writer;
                Builder = builder;
                Position = startPosition;
            }

            /// <summary>
            /// Gets the encoding.
            /// </summary>
            /// <value>The encoding.</value>
            public override Encoding Encoding
            {
                get { return _writer.Encoding; }
            }

            /// <summary>
            /// Gets an object that controls formatting.
            /// </summary>
            /// <returns>An <see cref="T:System.IFormatProvider" /> object for a specific culture, or the formatting of the current culture if no other culture is specified.</returns>
            public override IFormatProvider FormatProvider
            {
                get { return _writer.FormatProvider; }
            }

            /// <summary>
            /// Gets or sets the line terminator string used by the current TextWriter.
            /// </summary>
            /// <returns>The line terminator string for the current TextWriter.</returns>
            public override string NewLine
            {
                get { return _writer.NewLine; }
                set
                {
                    Contract.Requires(value != null);
                    _writer.NewLine = value;
                }
            }

            /// <summary>
            /// Flushes this instance.
            /// </summary>
            public override void Flush()
            {
                _writer.Flush();
            }

            /// <summary>
            /// Asynchronously clears all buffers for the current writer and causes any buffered data to be written to the underlying device.
            /// </summary>
            /// <returns>
            /// A task that represents the asynchronous flush operation.
            /// </returns>
            [NotNull]
            public override Task FlushAsync()
            {
                // We run everything synchronously, as we're synchronized!
                _writer.Flush();
                return TaskResult.Completed;
            }

            /// <summary>
            /// Writes the specified value.
            /// </summary>
            /// <param name="value">The value.</param>
            public override void Write(bool value)
            {
                Contract.Assert(Builder.IsEmpty);
                Builder.Append(value);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
            }

            /// <summary>
            /// Writes the specified value.
            /// </summary>
            /// <param name="value">The value.</param>
            public override void Write(char value)
            {
                Contract.Assert(Builder.IsEmpty);
                Builder.Append(value);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
            }

            /// <summary>
            /// Writes the specified buffer.
            /// </summary>
            /// <param name="buffer">The buffer.</param>
            public override void Write([CanBeNull] char[] buffer)
            {
                Contract.Assert(Builder.IsEmpty);
                Builder.Append(buffer);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
            }

            /// <summary>
            /// Writes the specified value.
            /// </summary>
            /// <param name="value">The value.</param>
            public override void Write(decimal value)
            {
                Contract.Assert(Builder.IsEmpty);
                Builder.Append(value);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
            }

            /// <summary>
            /// Writes the specified value.
            /// </summary>
            /// <param name="value">The value.</param>
            public override void Write(double value)
            {
                Contract.Assert(Builder.IsEmpty);
                Builder.Append(value);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
            }

            /// <summary>
            /// Writes the specified value.
            /// </summary>
            /// <param name="value">The value.</param>
            public override void Write(float value)
            {
                Contract.Assert(Builder.IsEmpty);
                Builder.Append(value);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
            }

            /// <summary>
            /// Writes the specified value.
            /// </summary>
            /// <param name="value">The value.</param>
            public override void Write(int value)
            {
                Contract.Assert(Builder.IsEmpty);
                Builder.Append(value);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
            }

            /// <summary>
            /// Writes the specified value.
            /// </summary>
            /// <param name="value">The value.</param>
            public override void Write(long value)
            {
                Contract.Assert(Builder.IsEmpty);
                Builder.Append(value);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
            }

            /// <summary>
            /// Writes the specified value.
            /// </summary>
            /// <param name="value">The value.</param>
            public override void Write([CanBeNull] object value)
            {
                Contract.Assert(Builder.IsEmpty);
                Builder.Append(value);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
            }

            /// <summary>
            /// Writes the specified value.
            /// </summary>
            /// <param name="value">The value.</param>
            public override void Write([CanBeNull] string value)
            {
                Contract.Assert(Builder.IsEmpty);
                Builder.AppendFormat(value);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
            }

            /// <summary>
            /// Writes the specified value.
            /// </summary>
            /// <param name="value">The value.</param>
            public override void Write(uint value)
            {
                Contract.Assert(Builder.IsEmpty);
                Builder.Append(value);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
            }

            /// <summary>
            /// Writes the specified value.
            /// </summary>
            /// <param name="value">The value.</param>
            public override void Write(ulong value)
            {
                Contract.Assert(Builder.IsEmpty);
                Builder.Append(value);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
            }

            /// <summary>
            /// Writes the specified format.
            /// </summary>
            /// <param name="format">The format.</param>
            /// <param name="arg0">The arg0.</param>
            public override void Write(string format, [CanBeNull] object arg0)
            {
                Contract.Requires(format != null);
                Contract.Assert(Builder.IsEmpty);
                Builder.AppendFormat(format, arg0);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
            }

            /// <summary>
            /// Writes the specified format.
            /// </summary>
            /// <param name="format">The format.</param>
            /// <param name="arg">The argument.</param>
            public override void Write(string format, params object[] arg)
            {
                Contract.Requires(format != null);
                Contract.Requires(arg != null);
                Contract.Assert(Builder.IsEmpty);
                Builder.AppendFormat(format, arg);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
            }

            /// <summary>
            /// Writes the specified buffer.
            /// </summary>
            /// <param name="buffer">The buffer.</param>
            /// <param name="index">The index.</param>
            /// <param name="count">The count.</param>
            public override void Write(char[] buffer, int index, int count)
            {
                Contract.Requires(buffer != null);
                Contract.Assert(Builder.IsEmpty);
                Builder.Append(buffer, index, count);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
            }

            /// <summary>
            /// Writes the specified format.
            /// </summary>
            /// <param name="format">The format.</param>
            /// <param name="arg0">The arg0.</param>
            /// <param name="arg1">The arg1.</param>
            public override void Write(string format, [CanBeNull] object arg0, [CanBeNull] object arg1)
            {
                Contract.Requires(format != null);
                Contract.Assert(Builder.IsEmpty);
                Builder.AppendFormat(format, arg0, arg1);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
            }

            /// <summary>
            /// Writes the specified format.
            /// </summary>
            /// <param name="format">The format.</param>
            /// <param name="arg0">The arg0.</param>
            /// <param name="arg1">The arg1.</param>
            /// <param name="arg2">The arg2.</param>
            public override void Write(
                string format,
                [CanBeNull] object arg0,
                [CanBeNull] object arg1,
                [CanBeNull] object arg2)
            {
                Contract.Requires(format != null);
                Contract.Assert(Builder.IsEmpty);
                Builder.AppendFormat(format, arg0, arg1, arg2);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
            }

            /// <summary>
            /// Writes the line.
            /// </summary>
            public override void WriteLine()
            {
                Contract.Assert(Builder.IsEmpty);
                Builder.AppendLine();
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
            }

            /// <summary>
            /// Writes the line.
            /// </summary>
            /// <param name="value">The value.</param>
            public override void WriteLine(bool value)
            {
                Contract.Assert(Builder.IsEmpty);
                Builder.AppendLine(value);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
            }

            /// <summary>
            /// Writes the line.
            /// </summary>
            /// <param name="value">The value.</param>
            public override void WriteLine(char value)
            {
                Contract.Assert(Builder.IsEmpty);
                Builder.AppendLine(value);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
            }

            /// <summary>
            /// Writes the line.
            /// </summary>
            /// <param name="buffer">The buffer.</param>
            public override void WriteLine([CanBeNull] char[] buffer)
            {
                Contract.Assert(Builder.IsEmpty);
                Builder.AppendLine(buffer);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
            }

            /// <summary>
            /// Writes the line.
            /// </summary>
            /// <param name="value">The value.</param>
            public override void WriteLine(decimal value)
            {
                Contract.Assert(Builder.IsEmpty);
                Builder.AppendLine(value);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
            }

            /// <summary>
            /// Writes the line.
            /// </summary>
            /// <param name="value">The value.</param>
            public override void WriteLine(double value)
            {
                Contract.Assert(Builder.IsEmpty);
                Builder.AppendLine(value);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
            }

            /// <summary>
            /// Writes the line.
            /// </summary>
            /// <param name="value">The value.</param>
            public override void WriteLine(float value)
            {
                Contract.Assert(Builder.IsEmpty);
                Builder.AppendLine(value);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
            }

            /// <summary>
            /// Writes the line.
            /// </summary>
            /// <param name="value">The value.</param>
            public override void WriteLine(int value)
            {
                Contract.Assert(Builder.IsEmpty);
                Builder.AppendLine(value);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
            }

            /// <summary>
            /// Writes the line.
            /// </summary>
            /// <param name="value">The value.</param>
            public override void WriteLine(long value)
            {
                Contract.Assert(Builder.IsEmpty);
                Builder.AppendLine(value);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
            }

            /// <summary>
            /// Writes the line.
            /// </summary>
            /// <param name="value">The value.</param>
            public override void WriteLine([CanBeNull] object value)
            {
                Contract.Assert(Builder.IsEmpty);
                Builder.AppendLine(value);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
            }

            /// <summary>
            /// Writes the line.
            /// </summary>
            /// <param name="value">The value.</param>
            public override void WriteLine([CanBeNull] string value)
            {
                Contract.Assert(Builder.IsEmpty);
                Builder.AppendFormatLine(value);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
            }

            /// <summary>
            /// Writes the line.
            /// </summary>
            /// <param name="value">The value.</param>
            public override void WriteLine(uint value)
            {
                Contract.Assert(Builder.IsEmpty);
                Builder.AppendLine(value);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
            }

            /// <summary>
            /// Writes the line.
            /// </summary>
            /// <param name="value">The value.</param>
            public override void WriteLine(ulong value)
            {
                Contract.Assert(Builder.IsEmpty);
                Builder.AppendLine(value);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
            }

            /// <summary>
            /// Writes the line.
            /// </summary>
            /// <param name="format">The format.</param>
            /// <param name="arg0">The arg0.</param>
            public override void WriteLine(string format, [CanBeNull] object arg0)
            {
                Contract.Requires(format != null);
                Contract.Assert(Builder.IsEmpty);
                Builder.AppendFormatLine(format, arg0);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
            }

            /// <summary>
            /// Writes the line.
            /// </summary>
            /// <param name="format">The format.</param>
            /// <param name="arg">The argument.</param>
            public override void WriteLine(string format, params object[] arg)
            {
                Contract.Requires(format != null);
                Contract.Requires(arg != null);
                Contract.Assert(Builder.IsEmpty);
                Builder.AppendFormatLine(format, arg);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
            }

            /// <summary>
            /// Writes the line.
            /// </summary>
            /// <param name="buffer">The buffer.</param>
            /// <param name="index">The index.</param>
            /// <param name="count">The count.</param>
            public override void WriteLine(char[] buffer, int index, int count)
            {
                Contract.Requires(buffer != null);
                Contract.Assert(Builder.IsEmpty);
                Builder.AppendLine(buffer, index, count);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
            }

            /// <summary>
            /// Writes the line.
            /// </summary>
            /// <param name="format">The format.</param>
            /// <param name="arg0">The arg0.</param>
            /// <param name="arg1">The arg1.</param>
            public override void WriteLine(string format, [CanBeNull] object arg0, [CanBeNull] object arg1)
            {
                Contract.Requires(format != null);
                Contract.Assert(Builder.IsEmpty);
                Builder.AppendFormatLine(format, arg0, arg1);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
            }

            /// <summary>
            /// Writes the line.
            /// </summary>
            /// <param name="format">The format.</param>
            /// <param name="arg0">The arg0.</param>
            /// <param name="arg1">The arg1.</param>
            /// <param name="arg2">The arg2.</param>
            public override void WriteLine(
                string format,
                [CanBeNull] object arg0,
                [CanBeNull] object arg1,
                [CanBeNull] object arg2)
            {
                Contract.Requires(format != null);
                Contract.Assert(Builder.IsEmpty);
                Builder.AppendFormatLine(format, arg0, arg1, arg2);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
            }

            /// <summary>
            /// Writes a character to the text string or stream asynchronously.
            /// </summary>
            /// <param name="value">The character to write to the text stream.</param>
            /// <returns>
            /// A task that represents the asynchronous write operation.
            /// </returns>
            [NotNull]
            public override Task WriteAsync(char value)
            {
                Contract.Assert(Builder.IsEmpty);
                Builder.Append(value);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
                return TaskResult.Completed;
            }

            /// <summary>
            /// Writes a subarray of characters to the text string or stream asynchronously.
            /// </summary>
            /// <param name="buffer">The character array to write data from.</param>
            /// <param name="index">The character position in the buffer at which to start retrieving data.</param>
            /// <param name="count">The number of characters to write.</param>
            /// <returns>
            /// A task that represents the asynchronous write operation.
            /// </returns>
            [NotNull]
            public override Task WriteAsync([NotNull] char[] buffer, int index, int count)
            {
                Contract.Requires(buffer != null);
                Contract.Assert(Builder.IsEmpty);
                Builder.Append(buffer, index, count);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
                return TaskResult.Completed;
            }

            /// <summary>
            /// Writes a string to the text string or stream asynchronously.
            /// </summary>
            /// <param name="value">The string to write. If <paramref name="value" /> is null, nothing is written to the text stream.</param>
            /// <returns>
            /// A task that represents the asynchronous write operation.
            /// </returns>
            [NotNull]
            public override Task WriteAsync([CanBeNull] string value)
            {
                Contract.Assert(Builder.IsEmpty);
                Builder.AppendFormat(value);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
                return TaskResult.Completed;
            }

            /// <summary>
            /// Writes a line terminator asynchronously to the text string or stream.
            /// </summary>
            /// <returns>
            /// A task that represents the asynchronous write operation.
            /// </returns>
            public override Task WriteLineAsync()
            {
                Contract.Assert(Builder.IsEmpty);
                Builder.AppendLine();
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
                return TaskResult.Completed;
            }

            /// <summary>
            /// Writes a character followed by a line terminator asynchronously to the text string or stream.
            /// </summary>
            /// <param name="value">The character to write to the text stream.</param>
            /// <returns>
            /// A task that represents the asynchronous write operation.
            /// </returns>
            [NotNull]
            public override Task WriteLineAsync(char value)
            {
                Contract.Assert(Builder.IsEmpty);
                Builder.AppendLine(value);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
                return TaskResult.Completed;
            }

            /// <summary>
            /// Writes a subarray of characters followed by a line terminator asynchronously to the text string or stream.
            /// </summary>
            /// <param name="buffer">The character array to write data from.</param>
            /// <param name="index">The character position in the buffer at which to start retrieving data.</param>
            /// <param name="count">The number of characters to write.</param>
            /// <returns>
            /// A task that represents the asynchronous write operation.
            /// </returns>
            [NotNull]
            public override Task WriteLineAsync([NotNull] char[] buffer, int index, int count)
            {
                Contract.Requires(buffer != null);
                Contract.Assert(Builder.IsEmpty);
                Builder.AppendLine(buffer, index, count);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
                return TaskResult.Completed;
            }

            /// <summary>
            /// Writes a string followed by a line terminator asynchronously to the text string or stream.
            /// </summary>
            /// <param name="value">The string to write. If the value is null, only a line terminator is written.</param>
            /// <returns>
            /// A task that represents the asynchronous write operation.
            /// </returns>
            [NotNull]
            public override Task WriteLineAsync([CanBeNull] string value)
            {
                Contract.Assert(Builder.IsEmpty);
                Builder.AppendFormatLine(value);
                Position = Builder.WriteTo(_writer, Position);
                Builder.Clear();
                return TaskResult.Completed;
            }
        }
    }
}