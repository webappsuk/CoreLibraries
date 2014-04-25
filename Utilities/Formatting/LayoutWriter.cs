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
    [PublicAPI]
    public class LayoutTextWriter : TextWriter
    {
        /// <summary>
        /// The out writer
        /// </summary>
        [NotNull]
        private readonly TextWriter _outWriter;

        /// <summary>
        /// The lock.
        /// </summary>
        [NotNull]
        private readonly AsyncLock _lock = new AsyncLock();

        /// <summary>
        /// The current horizontal position
        /// </summary>
        private int _position;

        /// <summary>
        /// The layout builder
        /// </summary>
        [NotNull]
        private LayoutBuilder _builder;

        /// <summary>
        /// Gets or sets the current horizontal position.
        /// </summary>
        /// <value>The position.</value>
        [PublicAPI]
        public int Position
        {
            get { return _position; }
        }

        /// <summary>
        /// Gets or sets the defaultLayout.
        /// </summary>
        /// <value>The defaultLayout.</value>
        [NotNull]
        [PublicAPI]
        public Layout Layout
        {
            get { return _builder.InitialLayout; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutTextWriter" /> class.
        /// </summary>
        /// <param name="outWriter">The out writer.</param>
        /// <param name="defaultLayout">The default layout.</param>
        /// <param name="startPosition">The starting horizontal position.</param>
        public LayoutTextWriter(
            [NotNull] TextWriter outWriter,
            [CanBeNull] Layout defaultLayout = null,
            int startPosition = 0)
        {
            Contract.Requires(outWriter != null);
            _outWriter = outWriter;
            _position = startPosition;
            _builder = new LayoutBuilder(defaultLayout);
        }

        /// <summary>
        /// Sets the layout.
        /// </summary>
        /// <param name="newLayout">The new layout.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>An awaitable task that returns the existing layout.</returns>
        [NotNull]
        [PublicAPI]
        public async Task<Layout> SetLayout(
            [CanBeNull] Layout newLayout,
            CancellationToken token = default(CancellationToken))
        {
            Contract.Requires(newLayout != null);
            if (newLayout == null) return _builder.InitialLayout;
            if (newLayout == _builder.InitialLayout) return newLayout;
            using (await _lock.LockAsync(token))
            {
                token.ThrowIfCancellationRequested();
                if (newLayout == _builder.InitialLayout) return newLayout;
                Layout existing = _builder.InitialLayout;
                _builder = new LayoutBuilder(existing.Apply(newLayout));
                return existing;
            }
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
        /// <param name="token">The cancellation token.</param>
        /// <returns>An awaitable task that returns the existing layout.</returns>
        [NotNull]
        [PublicAPI]
        public async Task<Layout> SetLayout(
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
            CancellationToken token = default(CancellationToken))
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

            using (await _lock.LockAsync(token))
            {
                token.ThrowIfCancellationRequested();
                Layout existing = _builder.InitialLayout;
                _builder =
                    new LayoutBuilder(
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
            }
        }

        /// <summary>
        /// Sets the new position.
        /// </summary>
        /// <param name="newPosition">The new position.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        [NotNull]
        [PublicAPI]
        public async Task<int> SetPosition(int newPosition, CancellationToken token = default(CancellationToken))
        {
            if (_position == newPosition) return newPosition;
            using (await _lock.LockAsync(token))
            {
                token.ThrowIfCancellationRequested();
                if (_position == newPosition) return newPosition;
                int existing = _position;
                _position = newPosition;
                return existing;
            }
        }

        /// <summary>
        /// Gets the encoding.
        /// </summary>
        /// <value>The encoding.</value>
        public override Encoding Encoding
        {
            get { return _outWriter.Encoding; }
        }

        /// <summary>
        /// Gets an object that controls formatting.
        /// </summary>
        /// <returns>An <see cref="T:System.IFormatProvider" /> object for a specific culture, or the formatting of the current culture if no other culture is specified.</returns>
        public override IFormatProvider FormatProvider
        {
            get { return _outWriter.FormatProvider; }
        }

        /// <summary>
        /// Gets or sets the line terminator string used by the current TextWriter.
        /// </summary>
        /// <returns>The line terminator string for the current TextWriter.</returns>
        public override string NewLine
        {
            get { return _outWriter.NewLine; }
            set
            {
                using (_lock.LockAsync().Result)
                    _outWriter.NewLine = value;
            }
        }

        /// <summary>
        /// Flushes this instance.
        /// </summary>
        public override void Flush()
        {
            using (_lock.LockAsync().Result)
                _outWriter.Flush();
        }

        /// <summary>
        /// Asynchronously clears all buffers for the current writer and causes any buffered data to be written to the underlying device.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous flush operation.
        /// </returns>
        [NotNull]
        public override async Task FlushAsync()
        {
            using (await _lock.LockAsync())
                // ReSharper disable once PossibleNullReferenceException
                await _outWriter.FlushAsync();
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(bool value)
        {
            using (_lock.LockAsync().Result)
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.Append(value);
                _position = _builder.WriteTo(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(char value)
        {
            using (_lock.LockAsync().Result)
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.Append(value);
                _position = _builder.WriteTo(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public override void Write([CanBeNull] char[] buffer)
        {
            using (_lock.LockAsync().Result)
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.Append(buffer);
                _position = _builder.WriteTo(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(decimal value)
        {
            using (_lock.LockAsync().Result)
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.Append(value);
                _position = _builder.WriteTo(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(double value)
        {
            using (_lock.LockAsync().Result)
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.Append(value);
                _position = _builder.WriteTo(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(float value)
        {
            using (_lock.LockAsync().Result)
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.Append(value);
                _position = _builder.WriteTo(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(int value)
        {
            using (_lock.LockAsync().Result)
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.Append(value);
                _position = _builder.WriteTo(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(long value)
        {
            using (_lock.LockAsync().Result)
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.Append(value);
                _position = _builder.WriteTo(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write([CanBeNull] object value)
        {
            using (_lock.LockAsync().Result)
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.Append(value);
                _position = _builder.WriteTo(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write([CanBeNull] string value)
        {
            using (_lock.LockAsync().Result)
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.AppendFormat(value);
                _position = _builder.WriteTo(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(uint value)
        {
            using (_lock.LockAsync().Result)
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.Append(value);
                _position = _builder.WriteTo(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(ulong value)
        {
            using (_lock.LockAsync().Result)
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.Append(value);
                _position = _builder.WriteTo(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        public override void Write(string format, [CanBeNull] object arg0)
        {
            using (_lock.LockAsync().Result)
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.AppendFormat(format, arg0);
                _position = _builder.WriteTo(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg">The argument.</param>
        public override void Write(string format, params object[] arg)
        {
            using (_lock.LockAsync().Result)
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.AppendFormat(format, arg);
                _position = _builder.WriteTo(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        public override void Write(char[] buffer, int index, int count)
        {
            using (_lock.LockAsync().Result)
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.Append(buffer, index, count);
                _position = _builder.WriteTo(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        public override void Write(string format, [CanBeNull] object arg0, [CanBeNull] object arg1)
        {
            using (_lock.LockAsync().Result)
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.AppendFormat(format, arg0, arg1);
                _position = _builder.WriteTo(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
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
            using (_lock.LockAsync().Result)
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.AppendFormat(format, arg0, arg1, arg2);
                _position = _builder.WriteTo(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        public override void WriteLine()
        {
            using (_lock.LockAsync().Result)
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.AppendLine();
                _position = _builder.WriteTo(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(bool value)
        {
            using (_lock.LockAsync().Result)
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.AppendLine(value);
                _position = _builder.WriteTo(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(char value)
        {
            using (_lock.LockAsync().Result)
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.AppendLine(value);
                _position = _builder.WriteTo(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public override void WriteLine([CanBeNull] char[] buffer)
        {
            using (_lock.LockAsync().Result)
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.AppendLine(buffer);
                _position = _builder.WriteTo(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(decimal value)
        {
            using (_lock.LockAsync().Result)
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.AppendLine(value);
                _position = _builder.WriteTo(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(double value)
        {
            using (_lock.LockAsync().Result)
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.AppendLine(value);
                _position = _builder.WriteTo(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(float value)
        {
            using (_lock.LockAsync().Result)
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.AppendLine(value);
                _position = _builder.WriteTo(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(int value)
        {
            using (_lock.LockAsync().Result)
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.AppendLine(value);
                _position = _builder.WriteTo(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(long value)
        {
            using (_lock.LockAsync().Result)
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.AppendLine(value);
                _position = _builder.WriteTo(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine([CanBeNull] object value)
        {
            using (_lock.LockAsync().Result)
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.AppendLine(value);
                _position = _builder.WriteTo(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine([CanBeNull] string value)
        {
            using (_lock.LockAsync().Result)
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.AppendFormatLine(value);
                _position = _builder.WriteTo(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(uint value)
        {
            using (_lock.LockAsync().Result)
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.AppendLine(value);
                _position = _builder.WriteTo(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(ulong value)
        {
            using (_lock.LockAsync().Result)
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.AppendLine(value);
                _position = _builder.WriteTo(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        public override void WriteLine(string format, [CanBeNull] object arg0)
        {
            using (_lock.LockAsync().Result)
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.AppendFormatLine(format, arg0);
                _position = _builder.WriteTo(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg">The argument.</param>
        public override void WriteLine(string format, params object[] arg)
        {
            using (_lock.LockAsync().Result)
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.AppendFormatLine(format, arg);
                _position = _builder.WriteTo(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        public override void WriteLine(char[] buffer, int index, int count)
        {
            using (_lock.LockAsync().Result)
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.AppendLine(buffer, index, count);
                _position = _builder.WriteTo(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        public override void WriteLine(string format, [CanBeNull] object arg0, [CanBeNull] object arg1)
        {
            using (_lock.LockAsync().Result)
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.AppendFormatLine(format, arg0, arg1);
                _position = _builder.WriteTo(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
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
            using (_lock.LockAsync().Result)
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.AppendFormatLine(format, arg0, arg1, arg2);
                _position = _builder.WriteTo(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes a character to the text string or stream asynchronously.
        /// </summary>
        /// <param name="value">The character to write to the text stream.</param>
        /// <returns>
        /// A task that represents the asynchronous write operation.
        /// </returns>
        [NotNull]
        public override async Task WriteAsync(char value)
        {
            using (await _lock.LockAsync())
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.Append(value);
                _position = await _builder.WriteToAsync(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
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
        public override async Task WriteAsync([NotNull] char[] buffer, int index, int count)
        {
            using (await _lock.LockAsync())
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.Append(buffer, index, count);
                _position = await _builder.WriteToAsync(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes a string to the text string or stream asynchronously.
        /// </summary>
        /// <param name="value">The string to write. If <paramref name="value" /> is null, nothing is written to the text stream.</param>
        /// <returns>
        /// A task that represents the asynchronous write operation.
        /// </returns>
        [NotNull]
        public override async Task WriteAsync([CanBeNull] string value)
        {
            using (await _lock.LockAsync())
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.AppendFormat(value);
                _position = await _builder.WriteToAsync(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes a string to the text string or stream asynchronously.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg">The argument.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        [NotNull]
        [PublicAPI]
        public async Task WriteAsync([CanBeNull] string format, params object[] arg)
        {
            using (await _lock.LockAsync())
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.AppendFormat(format, arg);
                _position = await _builder.WriteToAsync(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes a line terminator asynchronously to the text string or stream.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous write operation.
        /// </returns>
        [NotNull]
        public override async Task WriteLineAsync()
        {
            using (await _lock.LockAsync())
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.AppendLine();
                _position = await _builder.WriteToAsync(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes a character followed by a line terminator asynchronously to the text string or stream.
        /// </summary>
        /// <param name="value">The character to write to the text stream.</param>
        /// <returns>
        /// A task that represents the asynchronous write operation.
        /// </returns>
        [NotNull]
        public override async Task WriteLineAsync(char value)
        {
            using (await _lock.LockAsync())
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.AppendLine(value);
                _position = await _builder.WriteToAsync(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
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
        public override async Task WriteLineAsync([NotNull] char[] buffer, int index, int count)
        {
            using (await _lock.LockAsync())
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.AppendLine(buffer, index, count);
                _position = await _builder.WriteToAsync(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes a string followed by a line terminator asynchronously to the text string or stream.
        /// </summary>
        /// <param name="value">The string to write. If the value is null, only a line terminator is written.</param>
        /// <returns>
        /// A task that represents the asynchronous write operation.
        /// </returns>
        [NotNull]
        public override async Task WriteLineAsync([CanBeNull] string value)
        {
            using (await _lock.LockAsync())
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.AppendFormatLine(value);
                _position = await _builder.WriteToAsync(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }

        /// <summary>
        /// Writes a string to the text string or stream asynchronously.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg">The argument.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        [NotNull]
        [PublicAPI]
        public async Task WriteLineAsync([CanBeNull] string format, params object[] arg)
        {
            using (await _lock.LockAsync())
            {
                Contract.Assert(_builder.IsEmpty);
                _builder.AppendFormatLine(format, arg);
                _position = await _builder.WriteToAsync(_outWriter, FormatProvider, _position);
                _builder.Clear();
            }
        }
    }
}