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
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// Wraps a <see cref="TextWriter"/>, providing position tracking and synchronized writing with a <see cref="Layout"/>.
    /// </summary>
    /// <remarks>
    /// This is not inherently thread safe, to make thread safe use a synchronization wrapper.
    /// </remarks>
    [PublicAPI]
    public sealed class FormatTextWriter : TextWriter, ISerialTextWriter, ILayoutTextWriter
    {
        /// <summary>
        /// The <see cref="SynchronizationContext">synchronization context</see>.
        /// </summary>
        [NotNull]
        private readonly SerializingSynchronizationContext _context;

        /// <summary>
        /// Gets the <see cref="SynchronizationContext">synchronization context</see>.
        /// </summary>
        /// <value>The synchronization context.</value>
        public SerializingSynchronizationContext Context { get { return _context; } }

        /// <summary>
        /// The underlying writer.
        /// </summary>
        [NotNull]
        private readonly TextWriter _writer;

        [CanBeNull]
        private readonly ILayoutTextWriter _layoutTextWriter;

        /// <summary>
        /// The format builder
        /// </summary>
        [NotNull]
        private FormatBuilder _builder;

        /// <summary>
        /// The current horizontal position
        /// </summary>
        private ushort _position;

        /// <summary>
        /// Gets the width of the console.
        /// </summary>
        /// <value>The width of the console.</value>
        public ushort Width
        {
            get
            {
                return _layoutTextWriter == null ? _builder.InitialLayout.Width.Value : _layoutTextWriter.Width;
            }
        }

        /// <summary>
        /// Gets or sets the current horizontal position.
        /// </summary>
        /// <value>The position.</value>
        [PublicAPI]
        public ushort Position
        {
            get { return _layoutTextWriter == null ? _position : _layoutTextWriter.Position; }
            set
            {
                _context.Invoke(
                    () =>
                    {
                        if (_layoutTextWriter != null)
                        {
                            _layoutTextWriter.Position = value;
                            return;
                        }
                        if (value < 1) value = 0;
                        if (value == _position) return;
                        _position = value;
                    });
            }
        }

        /// <summary>
        /// Gets a value indicating whether the writer automatically wraps on reaching <see cref="Width" />.
        /// </summary>
        /// <value><see langword="true" /> if the writer automatically wraps; otherwise, <see langword="false" />.</value>
        public bool AutoWraps { get { return _layoutTextWriter != null && _layoutTextWriter.AutoWraps; } }

        /// <summary>
        /// Gets the current Layout.
        /// </summary>
        /// <value>The Layout.</value>
        [PublicAPI]
        [NotNull]
        public Layout Layout
        {
            get { return _builder.InitialLayout; }
        }

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FormatTextWriter" /> class.
        /// </summary>
        /// <param name="writer">The out writer.</param>
        /// <param name="startPosition">The starting horizontal position.</param>
        public FormatTextWriter(
            [NotNull] TextWriter writer,
            ushort startPosition = 0)
            : base(writer.FormatProvider)
        {
            Contract.Requires(writer != null);
            _writer = writer;
            ISerialTextWriter stw = writer as ISerialTextWriter;
            _context = stw != null ? stw.Context : new SerializingSynchronizationContext();
            _layoutTextWriter = writer as ILayoutTextWriter;
            if (_layoutTextWriter == null)
                Position = startPosition;
            _builder = new FormatBuilder();
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
            ushort startPosition = 0)
            : base(writer.FormatProvider)
        {
            Contract.Requires(writer != null);
            _writer = writer;
            ISerialTextWriter stw = writer as ISerialTextWriter;
            _context = stw != null ? stw.Context : new SerializingSynchronizationContext();
            _layoutTextWriter = writer as ILayoutTextWriter;
            if (_layoutTextWriter == null)
                Position = startPosition;
            _builder = new FormatBuilder(defaultLayout);
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
        /// <param name="splitWords">The split words.</param>
        /// <param name="hyphenate">The hyphenate.</param>
        /// <param name="hyphenChar">The hyphen character.</param>
        /// <param name="wrapMode">The wrap mode.</param>
        /// <param name="startPosition">The starting horizontal position.</param>
        public FormatTextWriter(
            [NotNull] TextWriter writer,
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
            Optional<LayoutWrapMode> wrapMode = default(Optional<LayoutWrapMode>),
            ushort startPosition = 0)
            : base(writer.FormatProvider)
        {
            Contract.Requires(writer != null);
            _writer = writer;
            ISerialTextWriter stw = writer as ISerialTextWriter;
            _context = stw != null ? stw.Context : new SerializingSynchronizationContext();
            _layoutTextWriter = writer as ILayoutTextWriter;
            if (_layoutTextWriter == null)
                Position = startPosition;
            _builder = new FormatBuilder(
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
            if (newLayout == null) return _builder.InitialLayout;
            // ReSharper disable once AssignNullToNotNullAttribute
            return _context.Invoke(
                () =>
                {
                    if (newLayout == _builder.InitialLayout) return newLayout;

                    Layout existing = _builder.InitialLayout;
                    _builder = new FormatBuilder(existing.Apply(newLayout));
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
        /// <param name="splitWords">if set to <see langword="true" /> then words will split across lines.</param>
        /// <param name="hyphenate">if set to <see langword="true" /> [hyphenate].</param>
        /// <param name="hyphenChar">The hyphenation character.</param>
        /// <param name="wrapMode">The line wrap mode.</param>
        /// <returns>An awaitable task that returns the existing layout.</returns>
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
                !splitWords.IsAssigned &&
                !hyphenate.IsAssigned &&
                !hyphenChar.IsAssigned &&
                !wrapMode.IsAssigned)
                return _builder.InitialLayout;

            // ReSharper disable once AssignNullToNotNullAttribute
            return _context.Invoke(
                () =>
                {
                    Layout existing = _builder.InitialLayout;
                    _builder =
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
                                splitWords,
                                hyphenate,
                                hyphenChar,
                                wrapMode));
                    return existing;
                });
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
                if (_writer.NewLine == value) return;

                _context.Invoke(() => _writer.NewLine = value);
            }
        }

        /// <summary>
        /// Flushes this instance.
        /// </summary>
        public override void Flush()
        {
            _context.Invoke(_writer.Flush);
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
            _context.Invoke(_writer.Flush);
            return TaskResult.Completed;
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(bool value)
        {
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.Append(value);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(char value)
        {
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.Append(value);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
        }

        /// <summary>
        /// Writes the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public override void Write([CanBeNull] char[] buffer)
        {
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.Append(buffer);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(decimal value)
        {
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.Append(value);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(double value)
        {
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.Append(value);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(float value)
        {
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.Append(value);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(int value)
        {
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.Append(value);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(long value)
        {
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.Append(value);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write([CanBeNull] object value)
        {
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.Append(value);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write([CanBeNull] string value)
        {
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.AppendFormat(value);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(uint value)
        {
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.Append(value);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(ulong value)
        {
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.Append(value);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
        }

        /// <summary>
        /// Writes the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        public override void Write(string format, [CanBeNull] object arg0)
        {
            Contract.Requires(format != null);
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.AppendFormat(format, arg0);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
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
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.AppendFormat(format, arg);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
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
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.Append(buffer, index, count);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
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
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.AppendFormat(format, arg0, arg1);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
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
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.AppendFormat(format, arg0, arg1, arg2);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        public override void WriteLine()
        {
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.AppendLine();
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(bool value)
        {
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.AppendLine(value);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(char value)
        {
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.AppendLine(value);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public override void WriteLine([CanBeNull] char[] buffer)
        {
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.AppendLine(buffer);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(decimal value)
        {
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.AppendLine(value);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(double value)
        {
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.AppendLine(value);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(float value)
        {
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.AppendLine(value);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(int value)
        {
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.AppendLine(value);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(long value)
        {
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.AppendLine(value);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine([CanBeNull] object value)
        {
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.AppendLine(value);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine([CanBeNull] string value)
        {
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.AppendFormatLine(value);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(uint value)
        {
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.AppendLine(value);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(ulong value)
        {
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.AppendLine(value);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        public override void WriteLine(string format, [CanBeNull] object arg0)
        {
            Contract.Requires(format != null);
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.AppendFormatLine(format, arg0);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
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
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.AppendFormatLine(format, arg);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
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
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.AppendLine(buffer, index, count);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
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
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.AppendFormatLine(format, arg0, arg1);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
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
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.AppendFormatLine(format, arg0, arg1, arg2);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
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
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.Append(value);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
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
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.Append(buffer, index, count);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
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
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.AppendFormat(value);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
            return TaskResult.Completed;
        }

        /// <summary>
        /// Writes a string to the text string or stream asynchronously.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg">The argument.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        [NotNull]
        [PublicAPI]
        public Task WriteAsync([CanBeNull] string format, [NotNull] params object[] arg)
        {
            Contract.Requires(arg != null);
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.AppendFormat(format, arg);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
            return TaskResult.Completed;
        }

        /// <summary>
        /// Writes a line terminator asynchronously to the text string or stream.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous write operation.
        /// </returns>
        [NotNull]
        public override Task WriteLineAsync()
        {
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.AppendLine();
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
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
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.AppendLine(value);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
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
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.AppendLine(buffer, index, count);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
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
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.AppendFormatLine(value);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
            return TaskResult.Completed;
        }

        /// <summary>
        /// Writes a string to the text string or stream asynchronously.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg">The argument.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        [NotNull]
        [PublicAPI]
        public Task WriteLineAsync([CanBeNull] string format, [NotNull] params object[] arg)
        {
            Contract.Requires(arg != null);
            Contract.Assert(_builder.IsEmpty);
            _context.Invoke(
                () =>
                {
                    _builder.AppendFormatLine(format, arg);
                    Position = _builder.WriteTo(_writer, Position);
                    _builder.Clear();
                });
            return TaskResult.Completed;
        }
    }
}