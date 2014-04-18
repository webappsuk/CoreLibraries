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
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// Implements a <see cref="TextWriter"/> that works like a <see cref="StringWriter"/> but uses a <see cref="FormatBuilder" /> for efficiently writing chunks of complicated text.
    /// </summary>
    [PublicAPI]
    [Serializable]
    [ComVisible(true)]
    public class FormattedStringWriter : TextWriter, IFormattable
    {
        /// <summary>
        /// The encoding.
        /// </summary>
        private static UnicodeEncoding _encoding;

        /// <summary>
        /// Whether this instance is currently open.
        /// </summary>
        private bool _isOpen;

        /// <summary>
        /// The underlying writer.
        /// </summary>
        [NotNull]
        private readonly FormatBuilder _builder;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatWriter" /> class.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="formatProvider">The format provider.</param>
        public FormattedStringWriter(
            [CanBeNull] FormatBuilder builder = null,
            [CanBeNull] IFormatProvider formatProvider = null)
            : base(formatProvider)
        {
            _builder = builder ?? new FormatBuilder();
            _isOpen = true;
        }

        /// <summary>
        /// Closes the current writer and releases any system resources associated with the writer.
        /// </summary>
        public override void Close()
        {
            Dispose(true);
        }

        /// <summary>
        /// Gets the encoding.
        /// </summary>
        /// <value>The encoding.</value>
        public override Encoding Encoding
        {
            get { return _encoding ?? (_encoding = new UnicodeEncoding(false, false)); }
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(bool value)
        {
            if (!_isOpen)
                throw new InvalidOperationException(Resources.FormatWriter_IsClosed);
            _builder.Append(value);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(char value)
        {
            if (!_isOpen)
                throw new InvalidOperationException(Resources.FormatWriter_IsClosed);
            _builder.Append(value);
        }

        /// <summary>
        /// Writes the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public override void Write([CanBeNull] char[] buffer)
        {
            if (!_isOpen)
                throw new InvalidOperationException(Resources.FormatWriter_IsClosed);
            _builder.Append(buffer);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(decimal value)
        {
            if (!_isOpen)
                throw new InvalidOperationException(Resources.FormatWriter_IsClosed);
            _builder.Append(value);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(double value)
        {
            if (!_isOpen)
                throw new InvalidOperationException(Resources.FormatWriter_IsClosed);
            _builder.Append(value);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(float value)
        {
            if (!_isOpen)
                throw new InvalidOperationException(Resources.FormatWriter_IsClosed);
            _builder.Append(value);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(int value)
        {
            if (!_isOpen)
                throw new InvalidOperationException(Resources.FormatWriter_IsClosed);
            _builder.Append(value);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(long value)
        {
            if (!_isOpen)
                throw new InvalidOperationException(Resources.FormatWriter_IsClosed);
            _builder.Append(value);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write([CanBeNull] object value)
        {
            if (!_isOpen)
                throw new InvalidOperationException(Resources.FormatWriter_IsClosed);
            _builder.Append(value);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write([CanBeNull] string value)
        {
            if (!_isOpen)
                throw new InvalidOperationException(Resources.FormatWriter_IsClosed);
            _builder.AppendFormat(value);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(uint value)
        {
            if (!_isOpen)
                throw new InvalidOperationException(Resources.FormatWriter_IsClosed);
            _builder.Append(value);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(ulong value)
        {
            if (!_isOpen)
                throw new InvalidOperationException(Resources.FormatWriter_IsClosed);
            _builder.Append(value);
        }

        /// <summary>
        /// Writes the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        public override void Write([CanBeNull] string format, [CanBeNull] object arg0)
        {
            if (!_isOpen)
                throw new InvalidOperationException(Resources.FormatWriter_IsClosed);
            _builder.AppendFormat(format, arg0);
        }

        /// <summary>
        /// Writes the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg">The argument.</param>
        public override void Write([CanBeNull] string format, [CanBeNull] params object[] arg)
        {
            if (!_isOpen)
                throw new InvalidOperationException(Resources.FormatWriter_IsClosed);
            _builder.AppendFormat(format, arg);
        }

        /// <summary>
        /// Writes the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        public override void Write([CanBeNull] char[] buffer, int index, int count)
        {
            if (!_isOpen)
                throw new InvalidOperationException(Resources.FormatWriter_IsClosed);
            _builder.Append(buffer, index, count);
        }

        /// <summary>
        /// Writes the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        public override void Write([CanBeNull] string format, [CanBeNull] object arg0, [CanBeNull] object arg1)
        {
            if (!_isOpen)
                throw new InvalidOperationException(Resources.FormatWriter_IsClosed);
            _builder.AppendFormat(format, arg0, arg1);
        }

        /// <summary>
        /// Writes the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        public override void Write(
            [CanBeNull] string format,
            [CanBeNull] object arg0,
            [CanBeNull] object arg1,
            [CanBeNull] object arg2)
        {
            if (!_isOpen)
                throw new InvalidOperationException(Resources.FormatWriter_IsClosed);
            _builder.AppendFormat(format, arg0, arg1, arg2);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        public override void WriteLine()
        {
            if (!_isOpen)
                throw new InvalidOperationException(Resources.FormatWriter_IsClosed);
            _builder.AppendLine();
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(bool value)
        {
            if (!_isOpen)
                throw new InvalidOperationException(Resources.FormatWriter_IsClosed);
            _builder.AppendLine(value);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(char value)
        {
            if (!_isOpen)
                throw new InvalidOperationException(Resources.FormatWriter_IsClosed);
            _builder.AppendLine(value);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public override void WriteLine([CanBeNull] char[] buffer)
        {
            if (!_isOpen)
                throw new InvalidOperationException(Resources.FormatWriter_IsClosed);
            _builder.AppendLine(buffer);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(decimal value)
        {
            if (!_isOpen)
                throw new InvalidOperationException(Resources.FormatWriter_IsClosed);
            _builder.AppendLine(value);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(double value)
        {
            if (!_isOpen)
                throw new InvalidOperationException(Resources.FormatWriter_IsClosed);
            _builder.AppendLine(value);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(float value)
        {
            if (!_isOpen)
                throw new InvalidOperationException(Resources.FormatWriter_IsClosed);
            _builder.AppendLine(value);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(int value)
        {
            if (!_isOpen)
                throw new InvalidOperationException(Resources.FormatWriter_IsClosed);
            _builder.AppendLine(value);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(long value)
        {
            if (!_isOpen)
                throw new InvalidOperationException(Resources.FormatWriter_IsClosed);
            _builder.AppendLine(value);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine([CanBeNull] object value)
        {
            if (!_isOpen)
                throw new InvalidOperationException(Resources.FormatWriter_IsClosed);
            _builder.AppendLine(value);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine([CanBeNull] string value)
        {
            if (!_isOpen)
                throw new InvalidOperationException(Resources.FormatWriter_IsClosed);
            _builder.AppendFormatLine(value);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(uint value)
        {
            if (!_isOpen)
                throw new InvalidOperationException(Resources.FormatWriter_IsClosed);
            _builder.AppendLine(value);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(ulong value)
        {
            if (!_isOpen)
                throw new InvalidOperationException(Resources.FormatWriter_IsClosed);
            _builder.AppendLine(value);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        public override void WriteLine([CanBeNull] string format, [CanBeNull] object arg0)
        {
            if (!_isOpen)
                throw new InvalidOperationException(Resources.FormatWriter_IsClosed);
            _builder.AppendFormatLine(format, arg0);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg">The argument.</param>
        public override void WriteLine([CanBeNull] string format, [CanBeNull] params object[] arg)
        {
            if (!_isOpen)
                throw new InvalidOperationException(Resources.FormatWriter_IsClosed);
            _builder.AppendFormatLine(format, arg);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        public override void WriteLine([CanBeNull] char[] buffer, int index, int count)
        {
            if (!_isOpen)
                throw new InvalidOperationException(Resources.FormatWriter_IsClosed);
            _builder.AppendLine(buffer, index, count);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        public override void WriteLine([CanBeNull] string format, [CanBeNull] object arg0, [CanBeNull] object arg1)
        {
            if (!_isOpen)
                throw new InvalidOperationException(Resources.FormatWriter_IsClosed);
            _builder.AppendFormatLine(format, arg0, arg1);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        public override void WriteLine(
            [CanBeNull] string format,
            [CanBeNull] object arg0,
            [CanBeNull] object arg1,
            [CanBeNull] object arg2)
        {
            if (!_isOpen)
                throw new InvalidOperationException(Resources.FormatWriter_IsClosed);
            _builder.AppendFormatLine(format, arg0, arg1, arg2);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            // Do not destroy _builder, so that we can extract it even after we are
            // done writing (similar to MemoryStream's GetBuffer & ToArray methods) 
            _isOpen = false;
            base.Dispose(disposing);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return _builder.ToString(null, null);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        [NotNull]
        [PublicAPI]
        public string ToString([CanBeNull] IFormatProvider formatProvider)
        {
            return _builder.ToString(null, formatProvider);
        }

        /// <summary>
        /// To the string.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>System.String.</returns>
        public string ToString([CanBeNull] string format, [CanBeNull] IFormatProvider formatProvider)
        {
            return _builder.ToString(format, formatProvider);
        }
    }
}