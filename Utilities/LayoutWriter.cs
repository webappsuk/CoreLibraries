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
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Lays out text written to the underlying text writer.
    /// </summary>
    public class LayoutWriter : TextWriter
    {
        /// <summary>
        /// The current writer.
        /// </summary>
        [NotNull]
        private readonly TextWriter _writer;

        /// <summary>
        /// The current layout
        /// </summary>
        [NotNull]
        private Layout _layout;

        /// <summary>
        /// The current position.
        /// </summary>
        private int _position;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutWriter" /> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="layout">The layout.</param>
        /// <param name="startPosition">The start position, if the writer is currently not at the start of a line.</param>
        public LayoutWriter([NotNull] TextWriter writer, [NotNull] Layout layout, int startPosition = 0)
        {
            Contract.Requires(writer != null);
            Contract.Requires(layout != null);
            _writer = writer;
            _layout = layout;
            _position = startPosition;
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
        /// Gets or sets the layout.
        /// </summary>
        /// <value>The layout.</value>
        [NotNull]
        [PublicAPI]
        public Layout Layout
        {
            get { return _layout; }
            set
            {
                Contract.Requires(value != null);
                _layout = value;
            }
        }

        /// <summary>
        /// Gets or sets the line terminator string used by the current TextWriter.
        /// </summary>
        /// <value>The new line.</value>
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
        /// Gets an object that controls formatting.
        /// </summary>
        /// <value>The format provider.</value>
        /// <returns>An <see cref="T:System.IFormatProvider" /> object for a specific culture, or the formatting of the current culture if no other culture is specified.</returns>
        public override IFormatProvider FormatProvider
        {
            get { return _writer.FormatProvider; }
        }

        /// <summary>
        /// When overridden in a derived class, returns the character encoding in which the output is written.
        /// </summary>
        /// <value>The encoding.</value>
        /// <returns>The character encoding in which the output is written.</returns>
        public override Encoding Encoding
        {
            get { return _writer.Encoding; }
        }


        /// <summary>
        /// Closes the current writer and releases any system resources associated with the writer.
        /// </summary>
        public override void Close()
        {
            // So that any overriden Close() gets run
            _writer.Close();
        }


        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.IO.TextWriter" /> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                ((IDisposable) _writer).Dispose();
        }


        /// <summary>
        /// Clears all buffers for the current writer and causes any buffered data to be written to the underlying device.
        /// </summary>
        public override void Flush()
        {
            _writer.Flush();
        }

        #region Write overloads
        /// <summary>
        /// Writes a character to the text string or stream.
        /// </summary>
        /// <param name="value">The character to write to the text stream.</param>
        public override void Write(char value)
        {
            WriteInternal(value.ToString(FormatProvider), false);
        }

        /// <summary>
        /// Writes the text representation of a decimal value to the text string or stream.
        /// </summary>
        /// <param name="value">The decimal value to write.</param>
        public override void Write(decimal value)
        {
            WriteInternal(value.ToString(FormatProvider), false);
        }

        /// <summary>
        /// Writes a character array to the text string or stream.
        /// </summary>
        /// <param name="buffer">The character array to write to the text stream.</param>
        public override void Write([CanBeNull] char[] buffer)
        {
            WriteInternal(buffer == null ? null : new string(buffer), false);
        }

        /// <summary>
        /// Writes a subarray of characters to the text string or stream.
        /// </summary>
        /// <param name="buffer">The character array to write data from.</param>
        /// <param name="index">The character position in the buffer at which to start retrieving data.</param>
        /// <param name="count">The number of characters to write.</param>
        public override void Write([CanBeNull] char[] buffer, int index, int count)
        {
            WriteInternal(buffer == null ? null : new string(buffer, index, count), false);
        }

        /// <summary>
        /// Writes the text representation of a Boolean value to the text string or stream.
        /// </summary>
        /// <param name="value">The Boolean value to write.</param>
        public override void Write(bool value)
        {
            WriteInternal(value.ToString(FormatProvider), false);
        }

        /// <summary>
        /// Writes the text representation of a 4-byte signed integer to the text string or stream.
        /// </summary>
        /// <param name="value">The 4-byte signed integer to write.</param>
        public override void Write(int value)
        {
            WriteInternal(value.ToString(FormatProvider), false);
        }

        /// <summary>
        /// Writes the text representation of a 4-byte unsigned integer to the text string or stream.
        /// </summary>
        /// <param name="value">The 4-byte unsigned integer to write.</param>
        public override void Write(uint value)
        {
            WriteInternal(value.ToString(FormatProvider), false);
        }

        /// <summary>
        /// Writes the text representation of an 8-byte signed integer to the text string or stream.
        /// </summary>
        /// <param name="value">The 8-byte signed integer to write.</param>
        public override void Write(long value)
        {
            WriteInternal(value.ToString(FormatProvider), false);
        }

        /// <summary>
        /// Writes the text representation of an 8-byte unsigned integer to the text string or stream.
        /// </summary>
        /// <param name="value">The 8-byte unsigned integer to write.</param>
        public override void Write(ulong value)
        {
            WriteInternal(value.ToString(FormatProvider), false);
        }

        /// <summary>
        /// Writes the text representation of a 4-byte floating-point value to the text string or stream.
        /// </summary>
        /// <param name="value">The 4-byte floating-point value to write.</param>
        public override void Write(float value)
        {
            WriteInternal(value.ToString(FormatProvider), false);
        }

        /// <summary>
        /// Writes the text representation of an 8-byte floating-point value to the text string or stream.
        /// </summary>
        /// <param name="value">The 8-byte floating-point value to write.</param>
        public override void Write(double value)
        {
            WriteInternal(value.ToString(FormatProvider), false);
        }

        /// <summary>
        /// Writes a string to the text string or stream.
        /// </summary>
        /// <param name="value">The string to write.</param>
        public override void Write([CanBeNull] string value)
        {
            WriteInternal(value, false);
        }

        /// <summary>
        /// Writes the text representation of an object to the text string or stream by calling the ToString method on that object.
        /// </summary>
        /// <param name="value">The object to write.</param>
        public override void Write([CanBeNull] object value)
        {
            WriteInternal(value == null ? null : value.ToString(), false);
        }

        /// <summary>
        /// Writes a formatted string to the text string or stream, using the same semantics as the <see cref="M:System.String.Format(System.String,System.Object)" /> method.
        /// </summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg0">The object to format and write.</param>
        public override void Write([CanBeNull] string format, [CanBeNull] object arg0)
        {
            WriteInternal(format, false, arg0);
        }

        /// <summary>
        /// Writes a formatted string to the text string or stream, using the same semantics as the <see cref="M:System.String.Format(System.String,System.Object,System.Object)" /> method.
        /// </summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg0">The first object to format and write.</param>
        /// <param name="arg1">The second object to format and write.</param>
        public override void Write([CanBeNull] string format, [CanBeNull] object arg0, [CanBeNull] object arg1)
        {
            WriteInternal(format, false, arg0, arg1);
        }

        /// <summary>
        /// Writes a formatted string to the text string or stream, using the same semantics as the <see cref="M:System.String.Format(System.String,System.Object,System.Object,System.Object)" /> method.
        /// </summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg0">The first object to format and write.</param>
        /// <param name="arg1">The second object to format and write.</param>
        /// <param name="arg2">The third object to format and write.</param>
        public override void Write(
            [CanBeNull] string format,
            [CanBeNull] object arg0,
            [CanBeNull] object arg1,
            [CanBeNull] object arg2)
        {
            WriteInternal(format, false, arg0, arg1, arg2);
        }

        /// <summary>
        /// Writes a formatted string to the text string or stream, using the same semantics as the <see cref="M:System.String.Format(System.String,System.Object[])" /> method.
        /// </summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg">An object array that contains zero or more objects to format and write.</param>
        public override void Write([CanBeNull] string format, [CanBeNull] params object[] arg)
        {
            WriteInternal(format, false, arg);
        }
        #endregion

        #region WriteLine overloads
        /// <summary>
        /// Writes a line terminator to the text string or stream.
        /// </summary>
        public override void WriteLine()
        {
            WriteInternal(null, true);
        }

        /// <summary>
        /// Writes a character followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The character to write to the text stream.</param>
        public override void WriteLine(char value)
        {
            WriteInternal(value.ToString(FormatProvider), true);
        }

        /// <summary>
        /// Writes the text representation of a decimal value followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The decimal value to write.</param>
        public override void WriteLine(decimal value)
        {
            WriteInternal(value.ToString(FormatProvider), true);
        }

        /// <summary>
        /// Writes an array of characters followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="buffer">The character array from which data is read.</param>
        public override void WriteLine([CanBeNull] char[] buffer)
        {
            WriteInternal(buffer == null ? null : new string(buffer), true);
        }

        /// <summary>
        /// Writes a subarray of characters followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="buffer">The character array from which data is read.</param>
        /// <param name="index">The character position in <paramref name="buffer" /> at which to start reading data.</param>
        /// <param name="count">The maximum number of characters to write.</param>
        public override void WriteLine([CanBeNull] char[] buffer, int index, int count)
        {
            WriteInternal(buffer == null ? null : new string(buffer, index, count), true);
        }

        /// <summary>
        /// Writes the text representation of a Boolean value followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The Boolean value to write.</param>
        public override void WriteLine(bool value)
        {
            WriteInternal(value.ToString(FormatProvider), true);
        }

        /// <summary>
        /// Writes the text representation of a 4-byte signed integer followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The 4-byte signed integer to write.</param>
        public override void WriteLine(int value)
        {
            WriteInternal(value.ToString(FormatProvider), true);
        }

        /// <summary>
        /// Writes the text representation of a 4-byte unsigned integer followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The 4-byte unsigned integer to write.</param>
        public override void WriteLine(uint value)
        {
            WriteInternal(value.ToString(FormatProvider), true);
        }

        /// <summary>
        /// Writes the text representation of an 8-byte signed integer followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The 8-byte signed integer to write.</param>
        public override void WriteLine(long value)
        {
            WriteInternal(value.ToString(FormatProvider), true);
        }

        /// <summary>
        /// Writes the text representation of an 8-byte unsigned integer followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The 8-byte unsigned integer to write.</param>
        public override void WriteLine(ulong value)
        {
            WriteInternal(value.ToString(FormatProvider), true);
        }

        /// <summary>
        /// Writes the text representation of a 4-byte floating-point value followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The 4-byte floating-point value to write.</param>
        public override void WriteLine(float value)
        {
            WriteInternal(value.ToString(FormatProvider), true);
        }

        /// <summary>
        /// Writes the text representation of a 8-byte floating-point value followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The 8-byte floating-point value to write.</param>
        public override void WriteLine(double value)
        {
            WriteInternal(value.ToString(FormatProvider), true);
        }

        /// <summary>
        /// Writes a string followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The string to write. If <paramref name="value" /> is null, only the line terminator is written.</param>
        public override void WriteLine([CanBeNull] string value)
        {
            WriteInternal(value, true);
        }

        /// <summary>
        /// Writes the text representation of an object by calling the ToString method on that object, followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The object to write. If <paramref name="value" /> is null, only the line terminator is written.</param>
        public override void WriteLine([CanBeNull] object value)
        {
            WriteInternal(value == null ? null : value.ToString(), true);
        }

        /// <summary>
        /// Writes a formatted string and a new line to the text string or stream, using the same semantics as the <see cref="M:System.String.Format(System.String,System.Object)" /> method.
        /// </summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg0">The object to format and write.</param>
        public override void WriteLine([CanBeNull] string format, [CanBeNull] object arg0)
        {
            WriteInternal(format, true, arg0);
        }

        /// <summary>
        /// Writes a formatted string and a new line to the text string or stream, using the same semantics as the <see cref="M:System.String.Format(System.String,System.Object,System.Object)" /> method.
        /// </summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg0">The first object to format and write.</param>
        /// <param name="arg1">The second object to format and write.</param>
        public override void WriteLine([CanBeNull] string format, [CanBeNull] object arg0, [CanBeNull] object arg1)
        {
            WriteInternal(format, true, arg0, arg1);
        }

        /// <summary>
        /// Writes out a formatted string and a new line, using the same semantics as <see cref="M:System.String.Format(System.String,System.Object)" />.
        /// </summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg0">The first object to format and write.</param>
        /// <param name="arg1">The second object to format and write.</param>
        /// <param name="arg2">The third object to format and write.</param>
        public override void WriteLine(
            [CanBeNull] string format,
            [CanBeNull] object arg0,
            [CanBeNull] object arg1,
            [CanBeNull] object arg2)
        {
            WriteInternal(format, true, arg0, arg1, arg2);
        }

        /// <summary>
        /// Writes out a formatted string and a new line, using the same semantics as <see cref="M:System.String.Format(System.String,System.Object)" />.
        /// </summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg">An object array that contains zero or more objects to format and write.</param>
        public override void WriteLine([CanBeNull] string format, [CanBeNull] params object[] arg)
        {
            WriteInternal(format, true, arg);
        }
        #endregion

        /// <summary>
        /// Writes the internal.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="addNewLine">if set to <see langword="true" /> [add new line].</param>
        /// <param name="args">The arguments.</param>
        private void WriteInternal([CanBeNull] string format, bool addNewLine, [CanBeNull] params object[] args)
        {
            string newLine = _writer.NewLine;
            Layout = _layout;
            if ((args != null) && (args.Length > 0))
                format = format.SafeFormat(args);
            if (!string.IsNullOrEmpty(format))
            {
                StringBuilder builder = new StringBuilder((int) (format.Length * 1.1));
                int pos = 0;
                do
                {
                    char c = format[pos++];
                    if (c == '\t')
                    {
                        
                    }
                } while (pos < format.Length);
            }
            
            // Check if we need to add a new line.
            if (!addNewLine) return;
            _position = 0;
            _writer.WriteLine();
        }
    }
}