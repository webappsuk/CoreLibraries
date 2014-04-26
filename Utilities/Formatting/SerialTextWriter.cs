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
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// Wraps a text writer so that is is synchronized by a <see cref="SynchronizationContext"/>.
    /// </summary>
    [Serializable]
    [PublicAPI]
    public class SerialTextWriter : TextWriter, ISerialTextWriter
    {
        /// <summary>
        /// The underlying writer.
        /// </summary>
        [NotNull]
        [PublicAPI]
        protected readonly TextWriter Writer;

        /// <summary>
        /// The synchronization context.
        /// </summary>
        [NotNull]
        private readonly SerializingSynchronizationContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialTextWriter" /> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        internal SerialTextWriter([NotNull] TextWriter writer)
            : base(writer.FormatProvider)
        {
            Contract.Requires(writer != null);
            Writer = writer;
            
            // Sanity check, we shouldn't normally create a serial text writer on a serialized text writer,
            // but it can happen (for example when initializing ConsoleTextWriter to use TraceTextWriter).
            ISerialTextWriter stw = writer as ISerialTextWriter;
            _context = stw != null ? stw.Context : new SerializingSynchronizationContext();
        }

        /// <summary>
        /// The synchronization context.
        /// </summary>
        [NotNull]
        public SerializingSynchronizationContext Context
        {
            get { return _context; }
        }

        /// <summary>
        /// When overridden in a derived class, returns the character encoding in which the output is written.
        /// </summary>
        /// <value>The encoding.</value>
        /// <returns>The character encoding in which the output is written.</returns>
        public override Encoding Encoding
        {
            get { return Writer.Encoding; }
        }

        /// <summary>
        /// Gets an object that controls formatting.
        /// </summary>
        /// <value>The format provider.</value>
        /// <returns>An <see cref="T:System.IFormatProvider" /> object for a specific culture, or the formatting of the current culture if no other culture is specified.</returns>
        public override IFormatProvider FormatProvider
        {
            get { return Writer.FormatProvider; }
        }

        /// <summary>
        /// Gets or sets the line terminator string used by the current TextWriter.
        /// </summary>
        /// <value>The new line.</value>
        /// <returns>The line terminator string for the current TextWriter.</returns>
        public override String NewLine
        {
            get { return Writer.NewLine; }
            set { Context.Invoke(() => Writer.NewLine = value); }
        }


        /// <summary>
        /// Closes the current writer and releases any system resources associated with the writer.
        /// </summary>
        public override void Close()
        {
            // So that any overriden Close() gets run
            Context.Invoke(Writer.Close);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.IO.TextWriter" /> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            // Explicitly pick up a potentially methodimpl'ed Dispose
            if (disposing)
                Context.Invoke(((IDisposable)Writer).Dispose);
        }

        /// <summary>
        /// Clears all buffers for the current writer and causes any buffered data to be written to the underlying device.
        /// </summary>
        public override void Flush()
        {
            Context.Invoke(Writer.Flush);
        }

        /// <summary>
        /// Writes a character to the text string or stream.
        /// </summary>
        /// <param name="value">The character to write to the text stream.</param>
        public override void Write(char value)
        {
            Context.Invoke(() => Writer.Write(value));
        }

        /// <summary>
        /// Writes a character array to the text string or stream.
        /// </summary>
        /// <param name="buffer">The character array to write to the text stream.</param>
        public override void Write(char[] buffer)
        {
            Context.Invoke(() => Writer.Write(buffer));
        }

        /// <summary>
        /// Writes a subarray of characters to the text string or stream.
        /// </summary>
        /// <param name="buffer">The character array to write data from.</param>
        /// <param name="index">The character position in the buffer at which to start retrieving data.</param>
        /// <param name="count">The number of characters to write.</param>
        [SuppressMessage("Microsoft.Contracts", "CC1055")]
        // Skip extra error checking to avoid *potential* AppCompat problems.
        // ReSharper disable once CodeAnnotationAnalyzer
        public override void Write(char[] buffer, int index, int count)
        {
            Context.Invoke(() => Writer.Write(buffer, index, count));
        }

        /// <summary>
        /// Writes the text representation of a Boolean value to the text string or stream.
        /// </summary>
        /// <param name="value">The Boolean value to write.</param>
        public override void Write(bool value)
        {
            Context.Invoke(() => Writer.Write(value));
        }

        /// <summary>
        /// Writes the text representation of a 4-byte signed integer to the text string or stream.
        /// </summary>
        /// <param name="value">The 4-byte signed integer to write.</param>
        public override void Write(int value)
        {
            Context.Invoke(() => Writer.Write(value));
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(uint value)
        {
            Context.Invoke(() => Writer.Write(value));
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(long value)
        {
            Context.Invoke(() => Writer.Write(value));
        }

        /// <summary>
        /// Writes the text representation of an 8-byte unsigned integer to the text string or stream.
        /// </summary>
        /// <param name="value">The 8-byte unsigned integer to write.</param>
        public override void Write(ulong value)
        {
            Context.Invoke(() => Writer.Write(value));
        }

        /// <summary>
        /// Writes the text representation of a 4-byte floating-point value to the text string or stream.
        /// </summary>
        /// <param name="value">The 4-byte floating-point value to write.</param>
        public override void Write(float value)
        {
            Context.Invoke(() => Writer.Write(value));
        }

        /// <summary>
        /// Writes the text representation of an 8-byte floating-point value to the text string or stream.
        /// </summary>
        /// <param name="value">The 8-byte floating-point value to write.</param>
        public override void Write(double value)
        {
            Context.Invoke(() => Writer.Write(value));
        }

        /// <summary>
        /// Writes the text representation of a decimal value to the text string or stream.
        /// </summary>
        /// <param name="value">The decimal value to write.</param>
        public override void Write(Decimal value)
        {
            Context.Invoke(() => Writer.Write(value));
        }

        /// <summary>
        /// Writes a string to the text string or stream.
        /// </summary>
        /// <param name="value">The string to write.</param>
        public override void Write(String value)
        {
            Context.Invoke(() => Writer.Write(value));
        }

        /// <summary>
        /// Writes the text representation of an object to the text string or stream by calling the ToString method on that object.
        /// </summary>
        /// <param name="value">The object to write.</param>
        public override void Write(Object value)
        {
            Context.Invoke(() => Writer.Write(value));
        }

        /// <summary>
        /// Writes a formatted string to the text string or stream, using the same semantics as the <see cref="M:System.String.Format(System.String,System.Object)" /> method.
        /// </summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg0">The object to format and write.</param>
        public override void Write(String format, Object arg0)
        {
            Context.Invoke(() => Writer.Write(format, arg0));
        }

        /// <summary>
        /// Writes a formatted string to the text string or stream, using the same semantics as the <see cref="M:System.String.Format(System.String,System.Object,System.Object)" /> method.
        /// </summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg0">The first object to format and write.</param>
        /// <param name="arg1">The second object to format and write.</param>
        public override void Write(String format, Object arg0, Object arg1)
        {
            Context.Invoke(() => Writer.Write(format, arg0, arg1));
        }

        /// <summary>
        /// Writes a formatted string to the text string or stream, using the same semantics as the <see cref="M:System.String.Format(System.String,System.Object,System.Object,System.Object)" /> method.
        /// </summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg0">The first object to format and write.</param>
        /// <param name="arg1">The second object to format and write.</param>
        /// <param name="arg2">The third object to format and write.</param>
        public override void Write(String format, Object arg0, Object arg1, Object arg2)
        {
            Context.Invoke(() => Writer.Write(format, arg0, arg1, arg2));
        }

        /// <summary>
        /// Writes a formatted string to the text string or stream, using the same semantics as the <see cref="M:System.String.Format(System.String,System.Object[])" /> method.
        /// </summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg">An object array that contains zero or more objects to format and write.</param>
        public override void Write(String format, params Object[] arg)
        {
            Context.Invoke(() => Writer.Write(format, arg));
        }

        /// <summary>
        /// Writes a line terminator to the text string or stream.
        /// </summary>
        public override void WriteLine()
        {
            Context.Invoke(() => Writer.WriteLine());
        }

        /// <summary>
        /// Writes a character followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The character to write to the text stream.</param>
        public override void WriteLine(char value)
        {
            Context.Invoke(() => Writer.WriteLine(value));
        }

        /// <summary>
        /// Writes the text representation of a decimal value followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The decimal value to write.</param>
        public override void WriteLine(decimal value)
        {
            Context.Invoke(() => Writer.WriteLine(value));
        }

        /// <summary>
        /// Writes an array of characters followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="buffer">The character array from which data is read.</param>
        public override void WriteLine(char[] buffer)
        {
            Context.Invoke(() => Writer.WriteLine(buffer));
        }

        /// <summary>
        /// Writes a subarray of characters followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="buffer">The character array from which data is read.</param>
        /// <param name="index">The character position in <paramref name="buffer" /> at which to start reading data.</param>
        /// <param name="count">The maximum number of characters to write.</param>
        public override void WriteLine(char[] buffer, int index, int count)
        {
            Context.Invoke(() => Writer.WriteLine(buffer, index, count));
        }

        /// <summary>
        /// Writes the text representation of a Boolean value followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The Boolean value to write.</param>
        public override void WriteLine(bool value)
        {
            Context.Invoke(() => Writer.WriteLine(value));
        }

        /// <summary>
        /// Writes the text representation of a 4-byte signed integer followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The 4-byte signed integer to write.</param>
        public override void WriteLine(int value)
        {
            Context.Invoke(() => Writer.WriteLine(value));
        }

        /// <summary>
        /// Writes the text representation of a 4-byte unsigned integer followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The 4-byte unsigned integer to write.</param>
        public override void WriteLine(uint value)
        {
            Context.Invoke(() => Writer.WriteLine(value));
        }

        /// <summary>
        /// Writes the text representation of an 8-byte signed integer followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The 8-byte signed integer to write.</param>
        public override void WriteLine(long value)
        {
            Context.Invoke(() => Writer.WriteLine(value));
        }

        /// <summary>
        /// Writes the text representation of an 8-byte unsigned integer followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The 8-byte unsigned integer to write.</param>
        public override void WriteLine(ulong value)
        {
            Context.Invoke(() => Writer.WriteLine(value));
        }

        /// <summary>
        /// Writes the text representation of a 4-byte floating-point value followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The 4-byte floating-point value to write.</param>
        public override void WriteLine(float value)
        {
            Context.Invoke(() => Writer.WriteLine(value));
        }

        /// <summary>
        /// Writes the text representation of a 8-byte floating-point value followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The 8-byte floating-point value to write.</param>
        public override void WriteLine(double value)
        {
            Context.Invoke(() => Writer.WriteLine(value));
        }

        /// <summary>
        /// Writes a string followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The string to write. If <paramref name="value" /> is null, only the line terminator is written.</param>
        public override void WriteLine(String value)
        {
            Context.Invoke(() => Writer.WriteLine(value));
        }

        /// <summary>
        /// Writes the text representation of an object by calling the ToString method on that object, followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The object to write. If <paramref name="value" /> is null, only the line terminator is written.</param>
        public override void WriteLine(Object value)
        {
            Context.Invoke(() => Writer.WriteLine(value));
        }

        /// <summary>
        /// Writes a formatted string and a new line to the text string or stream, using the same semantics as the <see cref="M:System.String.Format(System.String,System.Object)" /> method.
        /// </summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg0">The object to format and write.</param>
        public override void WriteLine(String format, Object arg0)
        {
            Context.Invoke(() => Writer.WriteLine(format, arg0));
        }

        /// <summary>
        /// Writes a formatted string and a new line to the text string or stream, using the same semantics as the <see cref="M:System.String.Format(System.String,System.Object,System.Object)" /> method.
        /// </summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg0">The first object to format and write.</param>
        /// <param name="arg1">The second object to format and write.</param>
        public override void WriteLine(String format, Object arg0, Object arg1)
        {
            Context.Invoke(() => Writer.WriteLine(format, arg0, arg1));
        }

        /// <summary>
        /// Writes out a formatted string and a new line, using the same semantics as <see cref="M:System.String.Format(System.String,System.Object)" />.
        /// </summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg0">The first object to format and write.</param>
        /// <param name="arg1">The second object to format and write.</param>
        /// <param name="arg2">The third object to format and write.</param>
        public override void WriteLine(String format, Object arg0, Object arg1, Object arg2)
        {
            Context.Invoke(() => Writer.WriteLine(format, arg0, arg1, arg2));
        }

        /// <summary>
        /// Writes out a formatted string and a new line, using the same semantics as <see cref="M:System.String.Format(System.String,System.Object)" />.
        /// </summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg">An object array that contains zero or more objects to format and write.</param>
        public override void WriteLine(String format, params Object[] arg)
        {
            Context.Invoke(() => Writer.WriteLine(format, arg));
        }

        //
        // On SyncTextWriter all APIs should run synchronously, even the async ones.
        //

        /// <summary>
        /// Writes a character to the text string or stream asynchronously.
        /// </summary>
        /// <param name="value">The character to write to the text stream.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        [ComVisible(false)]
        public override Task WriteAsync(char value)
        {
            return Context.Invoke(
                () =>
                {
                    Write(value);
                    return TaskResult.Completed;
                });
        }

        /// <summary>
        /// Writes a string to the text string or stream asynchronously.
        /// </summary>
        /// <param name="value">The string to write. If <paramref name="value" /> is null, nothing is written to the text stream.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        [ComVisible(false)]
        public override Task WriteAsync(String value)
        {
            return Context.Invoke(
                () =>
                {
                    Write(value);
                    return TaskResult.Completed;
                });
        }

        /// <summary>
        /// Writes a subarray of characters to the text string or stream asynchronously.
        /// </summary>
        /// <param name="buffer">The character array to write data from.</param>
        /// <param name="index">The character position in the buffer at which to start retrieving data.</param>
        /// <param name="count">The number of characters to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        [ComVisible(false)]
        public override Task WriteAsync(char[] buffer, int index, int count)
        {
            return Context.Invoke(
                () =>
                {
                    Write(buffer, index, count);
                    return TaskResult.Completed;
                });
        }

        /// <summary>
        /// Writes a character followed by a line terminator asynchronously to the text string or stream.
        /// </summary>
        /// <param name="value">The character to write to the text stream.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        [ComVisible(false)]
        public override Task WriteLineAsync(char value)
        {
            return Context.Invoke(
                () =>
                {
                    WriteLine(value);
                    return TaskResult.Completed;
                });
        }

        /// <summary>
        /// Writes a string followed by a line terminator asynchronously to the text string or stream.
        /// </summary>
        /// <param name="value">The string to write. If the value is null, only a line terminator is written.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        [ComVisible(false)]
        public override Task WriteLineAsync(String value)
        {
            return Context.Invoke(
                () =>
                {
                    WriteLine(value);
                    return TaskResult.Completed;
                });
        }

        /// <summary>
        /// Writes a subarray of characters followed by a line terminator asynchronously to the text string or stream.
        /// </summary>
        /// <param name="buffer">The character array to write data from.</param>
        /// <param name="index">The character position in the buffer at which to start retrieving data.</param>
        /// <param name="count">The number of characters to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        [ComVisible(false)]
        public override Task WriteLineAsync(char[] buffer, int index, int count)
        {
            return Context.Invoke(
                () =>
                {
                    WriteLine(buffer, index, count);
                    return TaskResult.Completed;
                });
        }

        /// <summary>
        /// Asynchronously clears all buffers for the current writer and causes any buffered data to be written to the underlying device.
        /// </summary>
        /// <returns>A task that represents the asynchronous flush operation.</returns>
        [ComVisible(false)]
        public override Task FlushAsync()
        {
            return Context.Invoke(
                () =>
                {
                    Flush();
                    return TaskResult.Completed;
                });
        }
    }
}