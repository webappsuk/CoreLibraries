#region © Copyright Web Applications (UK) Ltd, 2015.  All rights reserved.
// Copyright (c) 2015, Web Applications UK Ltd
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
using System.Threading.Tasks;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// Interface ITextWriter defines all the methods that inheritors of <see cref="TextWriter"/> implement.
    /// </summary>
    /// <remarks>
    /// Unfortunately, <see cref="TextWriter"/> does not implement this interface, but it helps us ensure that our extension interfaces are only added
    /// to types that look like <see cref="TextWriter"/>
    /// </remarks>
    [PublicAPI]
    public interface ITextWriter : IDisposable
    {
        /// <summary>
        /// When overridden in a derived class, returns the character encoding in which the output is written.
        /// </summary>
        /// <value>The encoding.</value>
        /// <returns>The character encoding in which the output is written.</returns>
        Encoding Encoding { get; }

        /// <summary>
        /// Gets an object that controls formatting.
        /// </summary>
        /// <value>The format provider.</value>
        /// <returns>An <see cref="T:System.IFormatProvider" /> object for a specific culture, or the formatting of the current culture if no other culture is specified.</returns>
        IFormatProvider FormatProvider { get; }

        /// <summary>
        /// Gets or sets the line terminator string used by the current TextWriter.
        /// </summary>
        /// <value>The new line.</value>
        /// <returns>The line terminator string for the current TextWriter.</returns>
        String NewLine { get; set; }

        /// <summary>
        /// Closes the current writer and releases any system resources associated with the writer.
        /// </summary>
        void Close();

        /// <summary>
        /// Clears all buffers for the current writer and causes any buffered data to be written to the underlying device.
        /// </summary>
        void Flush();

        /// <summary>
        /// Writes a character to the text string or stream.
        /// </summary>
        /// <param name="value">The character to write to the text stream.</param>
        void Write(char value);

        /// <summary>
        /// Writes a character array to the text string or stream.
        /// </summary>
        /// <param name="buffer">The character array to write to the text stream.</param>
        void Write(char[] buffer);

        /// <summary>
        /// Writes a subarray of characters to the text string or stream.
        /// </summary>
        /// <param name="buffer">The character array to write data from.</param>
        /// <param name="index">The character position in the buffer at which to start retrieving data.</param>
        /// <param name="count">The number of characters to write.</param>
        void Write(char[] buffer, int index, int count);

        /// <summary>
        /// Writes the text representation of a Boolean value to the text string or stream.
        /// </summary>
        /// <param name="value">The Boolean value to write.</param>
        void Write(bool value);

        /// <summary>
        /// Writes the text representation of a 4-byte signed integer to the text string or stream.
        /// </summary>
        /// <param name="value">The 4-byte signed integer to write.</param>
        void Write(int value);

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        void Write(uint value);

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        void Write(long value);

        /// <summary>
        /// Writes the text representation of an 8-byte unsigned integer to the text string or stream.
        /// </summary>
        /// <param name="value">The 8-byte unsigned integer to write.</param>
        void Write(ulong value);

        /// <summary>
        /// Writes the text representation of a 4-byte floating-point value to the text string or stream.
        /// </summary>
        /// <param name="value">The 4-byte floating-point value to write.</param>
        void Write(float value);

        /// <summary>
        /// Writes the text representation of an 8-byte floating-point value to the text string or stream.
        /// </summary>
        /// <param name="value">The 8-byte floating-point value to write.</param>
        void Write(double value);

        /// <summary>
        /// Writes the text representation of a decimal value to the text string or stream.
        /// </summary>
        /// <param name="value">The decimal value to write.</param>
        void Write(Decimal value);

        /// <summary>
        /// Writes a string to the text string or stream.
        /// </summary>
        /// <param name="value">The string to write.</param>
        void Write(String value);

        /// <summary>
        /// Writes the text representation of an object to the text string or stream by calling the ToString method on that object.
        /// </summary>
        /// <param name="value">The object to write.</param>
        void Write(Object value);

        /// <summary>
        /// Writes a formatted string to the text string or stream, using the same semantics as the <see cref="M:System.String.Format(System.String,System.Object)" /> method.
        /// </summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg0">The object to format and write.</param>
        void Write(String format, Object arg0);

        /// <summary>
        /// Writes a formatted string to the text string or stream, using the same semantics as the <see cref="M:System.String.Format(System.String,System.Object,System.Object)" /> method.
        /// </summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg0">The first object to format and write.</param>
        /// <param name="arg1">The second object to format and write.</param>
        void Write(String format, Object arg0, Object arg1);

        /// <summary>
        /// Writes a formatted string to the text string or stream, using the same semantics as the <see cref="M:System.String.Format(System.String,System.Object,System.Object,System.Object)" /> method.
        /// </summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg0">The first object to format and write.</param>
        /// <param name="arg1">The second object to format and write.</param>
        /// <param name="arg2">The third object to format and write.</param>
        void Write(String format, Object arg0, Object arg1, Object arg2);

        /// <summary>
        /// Writes a formatted string to the text string or stream, using the same semantics as the <see cref="M:System.String.Format(System.String,System.Object[])" /> method.
        /// </summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg">An object array that contains zero or more objects to format and write.</param>
        void Write(String format, params Object[] arg);

        /// <summary>
        /// Writes a line terminator to the text string or stream.
        /// </summary>
        void WriteLine();

        /// <summary>
        /// Writes a character followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The character to write to the text stream.</param>
        void WriteLine(char value);

        /// <summary>
        /// Writes the text representation of a decimal value followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The decimal value to write.</param>
        void WriteLine(decimal value);

        /// <summary>
        /// Writes an array of characters followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="buffer">The character array from which data is read.</param>
        void WriteLine(char[] buffer);

        /// <summary>
        /// Writes a subarray of characters followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="buffer">The character array from which data is read.</param>
        /// <param name="index">The character position in <paramref name="buffer" /> at which to start reading data.</param>
        /// <param name="count">The maximum number of characters to write.</param>
        void WriteLine(char[] buffer, int index, int count);

        /// <summary>
        /// Writes the text representation of a Boolean value followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The Boolean value to write.</param>
        void WriteLine(bool value);

        /// <summary>
        /// Writes the text representation of a 4-byte signed integer followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The 4-byte signed integer to write.</param>
        void WriteLine(int value);

        /// <summary>
        /// Writes the text representation of a 4-byte unsigned integer followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The 4-byte unsigned integer to write.</param>
        void WriteLine(uint value);

        /// <summary>
        /// Writes the text representation of an 8-byte signed integer followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The 8-byte signed integer to write.</param>
        void WriteLine(long value);

        /// <summary>
        /// Writes the text representation of an 8-byte unsigned integer followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The 8-byte unsigned integer to write.</param>
        void WriteLine(ulong value);

        /// <summary>
        /// Writes the text representation of a 4-byte floating-point value followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The 4-byte floating-point value to write.</param>
        void WriteLine(float value);

        /// <summary>
        /// Writes the text representation of a 8-byte floating-point value followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The 8-byte floating-point value to write.</param>
        void WriteLine(double value);

        /// <summary>
        /// Writes a string followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The string to write. If <paramref name="value" /> is null, only the line terminator is written.</param>
        void WriteLine([NotNull] String value);

        /// <summary>
        /// Writes the text representation of an object by calling the ToString method on that object, followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The object to write. If <paramref name="value" /> is null, only the line terminator is written.</param>
        void WriteLine([NotNull] Object value);

        /// <summary>
        /// Writes a formatted string and a new line to the text string or stream, using the same semantics as the <see cref="M:System.String.Format(System.String,System.Object)" /> method.
        /// </summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg0">The object to format and write.</param>
        void WriteLine([NotNull] String format, [NotNull] Object arg0);

        /// <summary>
        /// Writes a formatted string and a new line to the text string or stream, using the same semantics as the <see cref="M:System.String.Format(System.String,System.Object,System.Object)" /> method.
        /// </summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg0">The first object to format and write.</param>
        /// <param name="arg1">The second object to format and write.</param>
        void WriteLine([NotNull] String format, [NotNull] Object arg0, [NotNull] Object arg1);

        /// <summary>
        /// Writes out a formatted string and a new line, using the same semantics as <see cref="M:System.String.Format(System.String,System.Object)" />.
        /// </summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg0">The first object to format and write.</param>
        /// <param name="arg1">The second object to format and write.</param>
        /// <param name="arg2">The third object to format and write.</param>
        void WriteLine([NotNull] String format, [NotNull] Object arg0, [NotNull] Object arg1, [NotNull] Object arg2);

        /// <summary>
        /// Writes out a formatted string and a new line, using the same semantics as <see cref="M:System.String.Format(System.String,System.Object)" />.
        /// </summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg">An object array that contains zero or more objects to format and write.</param>
        void WriteLine([NotNull] String format, [NotNull] params Object[] arg);

        /// <summary>
        /// Writes a character to the text string or stream asynchronously.
        /// </summary>
        /// <param name="value">The character to write to the text stream.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        [NotNull]
        [ComVisible(false)]
        Task WriteAsync(char value);

        /// <summary>
        /// Writes a string to the text string or stream asynchronously.
        /// </summary>
        /// <param name="value">The string to write. If <paramref name="value" /> is null, nothing is written to the text stream.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        [NotNull]
        [ComVisible(false)]
        Task WriteAsync([NotNull] String value);

        /// <summary>
        /// Writes a subarray of characters to the text string or stream asynchronously.
        /// </summary>
        /// <param name="buffer">The character array to write data from.</param>
        /// <param name="index">The character position in the buffer at which to start retrieving data.</param>
        /// <param name="count">The number of characters to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        [NotNull]
        [ComVisible(false)]
        Task WriteAsync([NotNull] char[] buffer, int index, int count);

        /// <summary>
        /// Writes a character followed by a line terminator asynchronously to the text string or stream.
        /// </summary>
        /// <param name="value">The character to write to the text stream.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        [NotNull]
        [ComVisible(false)]
        Task WriteLineAsync(char value);

        /// <summary>
        /// Writes a string followed by a line terminator asynchronously to the text string or stream.
        /// </summary>
        /// <param name="value">The string to write. If the value is null, only a line terminator is written.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        [NotNull]
        [ComVisible(false)]
        Task WriteLineAsync([NotNull] String value);

        /// <summary>
        /// Writes a subarray of characters followed by a line terminator asynchronously to the text string or stream.
        /// </summary>
        /// <param name="buffer">The character array to write data from.</param>
        /// <param name="index">The character position in the buffer at which to start retrieving data.</param>
        /// <param name="count">The number of characters to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        [NotNull]
        [ComVisible(false)]
        Task WriteLineAsync([NotNull] char[] buffer, int index, int count);

        /// <summary>
        /// Asynchronously clears all buffers for the current writer and causes any buffered data to be written to the underlying device.
        /// </summary>
        /// <returns>A task that represents the asynchronous flush operation.</returns>
        [NotNull]
        [ComVisible(false)]
        Task FlushAsync();

        /// <summary>
        /// Writes a line terminator asynchronously to the text string or stream.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous write operation.
        /// </returns>
        [NotNull]
        Task WriteLineAsync();
    }
}