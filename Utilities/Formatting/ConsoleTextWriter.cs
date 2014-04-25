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
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// Implements a <see cref="TextWriter"/> for writing to the <see cref="Console"/>, with write calls synchronized.
    /// </summary>
    [PublicAPI]
    public class ConsoleTextWriter : TextWriter
    {
        /// <summary>
        /// The default
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly ConsoleTextWriter Default;

        /// <summary>
        /// Initializes the <see cref="ConsoleTextWriter"/> class.
        /// </summary>
        static ConsoleTextWriter()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Default = new ConsoleTextWriter(Console.Out);

            Console.SetOut(Default);
        }

        /// <summary>
        /// The out writer
        /// </summary>
        [NotNull]
        private readonly TextWriter _outWriter;

        /// <summary>
        /// Prevents a default instance of the <see cref="ConsoleTextWriter"/> class from being created.
        /// </summary>
        private ConsoleTextWriter([NotNull] TextWriter outWriter)
        {
            Contract.Requires(outWriter != null);
            _outWriter = outWriter;
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
            set { _outWriter.NewLine = value; }
        }

        /// <summary>
        /// Flushes this instance.
        /// </summary>
        public override void Flush()
        {
            ConsoleHelper.SynchronizationContext.Invoke(_outWriter.Flush);
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
            await ConsoleHelper.SynchronizationContext;
            // ReSharper disable once PossibleNullReferenceException
            await _outWriter.FlushAsync();
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(bool value)
        {
            ConsoleHelper.SynchronizationContext.Invoke(() => _outWriter.Write(value));
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(char value)
        {
            ConsoleHelper.SynchronizationContext.Invoke(() => _outWriter.Write(value));
        }

        /// <summary>
        /// Writes the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public override void Write([CanBeNull] char[] buffer)
        {
            ConsoleHelper.SynchronizationContext.Invoke(() => _outWriter.Write(buffer));
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(decimal value)
        {
            ConsoleHelper.SynchronizationContext.Invoke(() => _outWriter.Write(value));
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(double value)
        {
            ConsoleHelper.SynchronizationContext.Invoke(() => _outWriter.Write(value));
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(float value)
        {
            ConsoleHelper.SynchronizationContext.Invoke(() => _outWriter.Write(value));
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(int value)
        {
            ConsoleHelper.SynchronizationContext.Invoke(() => _outWriter.Write(value));
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(long value)
        {
            ConsoleHelper.SynchronizationContext.Invoke(() => _outWriter.Write(value));
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write([CanBeNull] object value)
        {
            ConsoleHelper.SynchronizationContext.Invoke(() => _outWriter.Write(value));
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write([CanBeNull] string value)
        {
            ConsoleHelper.SynchronizationContext.Invoke(() => _outWriter.Write(value));
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(uint value)
        {
            ConsoleHelper.SynchronizationContext.Invoke(() => _outWriter.Write(value));
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(ulong value)
        {
            ConsoleHelper.SynchronizationContext.Invoke(() => _outWriter.Write(value));
        }

        /// <summary>
        /// Writes the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        public override void Write(string format, [CanBeNull] object arg0)
        {
            ConsoleHelper.SynchronizationContext.Invoke(() => _outWriter.Write(format, arg0));
        }

        /// <summary>
        /// Writes the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg">The argument.</param>
        public override void Write(string format, params object[] arg)
        {
            ConsoleHelper.SynchronizationContext.Invoke(() => _outWriter.Write(format, arg));
        }

        /// <summary>
        /// Writes the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        public override void Write(char[] buffer, int index, int count)
        {
            ConsoleHelper.SynchronizationContext.Invoke(() => _outWriter.Write(buffer, index, count));
        }

        /// <summary>
        /// Writes the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        public override void Write(string format, [CanBeNull] object arg0, [CanBeNull] object arg1)
        {
            ConsoleHelper.SynchronizationContext.Invoke(() => _outWriter.Write(format, arg0, arg1));
        }

        /// <summary>
        /// Writes the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        public override void Write(string format, [CanBeNull] object arg0, [CanBeNull] object arg1, [CanBeNull] object arg2)
        {
            ConsoleHelper.SynchronizationContext.Invoke(() => _outWriter.Write(format, arg0, arg1, arg2));
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        public override void WriteLine()
        {
            ConsoleHelper.SynchronizationContext.Invoke(() => _outWriter.WriteLine());
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(bool value)
        {
            ConsoleHelper.SynchronizationContext.Invoke(() => _outWriter.WriteLine(value));
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(char value)
        {
            ConsoleHelper.SynchronizationContext.Invoke(() => _outWriter.WriteLine(value));
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public override void WriteLine([CanBeNull] char[] buffer)
        {
            ConsoleHelper.SynchronizationContext.Invoke(() => _outWriter.WriteLine(buffer));
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(decimal value)
        {
            ConsoleHelper.SynchronizationContext.Invoke(() => _outWriter.WriteLine(value));
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(double value)
        {
            ConsoleHelper.SynchronizationContext.Invoke(() => _outWriter.WriteLine(value));
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(float value)
        {
            ConsoleHelper.SynchronizationContext.Invoke(() => _outWriter.WriteLine(value));
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(int value)
        {
            ConsoleHelper.SynchronizationContext.Invoke(() => _outWriter.WriteLine(value));
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(long value)
        {
            ConsoleHelper.SynchronizationContext.Invoke(() => _outWriter.WriteLine(value));
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine([CanBeNull] object value)
        {
            ConsoleHelper.SynchronizationContext.Invoke(() => _outWriter.WriteLine(value));
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine([CanBeNull] string value)
        {
            ConsoleHelper.SynchronizationContext.Invoke(() => _outWriter.WriteLine(value));
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(uint value)
        {
            ConsoleHelper.SynchronizationContext.Invoke(() => _outWriter.WriteLine(value));
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(ulong value)
        {
            ConsoleHelper.SynchronizationContext.Invoke(() => _outWriter.WriteLine(value));
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        public override void WriteLine(string format, [CanBeNull] object arg0)
        {
            ConsoleHelper.SynchronizationContext.Invoke(() => _outWriter.WriteLine(format, arg0));
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg">The argument.</param>
        public override void WriteLine(string format, params object[] arg)
        {
            ConsoleHelper.SynchronizationContext.Invoke(() => _outWriter.WriteLine(format, arg));
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        public override void WriteLine(char[] buffer, int index, int count)
        {
            string x = new string(buffer, index, count);
            ConsoleHelper.SynchronizationContext.Invoke(() => _outWriter.WriteLine(x));
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        public override void WriteLine(string format, [CanBeNull] object arg0, [CanBeNull] object arg1)
        {
            ConsoleHelper.SynchronizationContext.Invoke(() => _outWriter.WriteLine(format, arg0, arg1));
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        public override void WriteLine(string format, [CanBeNull] object arg0, [CanBeNull] object arg1, [CanBeNull] object arg2)
        {
            ConsoleHelper.SynchronizationContext.Invoke(() => _outWriter.WriteLine(format, arg0, arg1, arg2));
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
            await ConsoleHelper.SynchronizationContext;
            // ReSharper disable once PossibleNullReferenceException
            await _outWriter.WriteAsync(value);
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
            await ConsoleHelper.SynchronizationContext;
            // ReSharper disable once PossibleNullReferenceException
            await _outWriter.WriteAsync(buffer, index, count);
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
            await ConsoleHelper.SynchronizationContext;
            // ReSharper disable once PossibleNullReferenceException
            await _outWriter.WriteAsync(value);
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
            await ConsoleHelper.SynchronizationContext;
            // ReSharper disable once PossibleNullReferenceException
            await _outWriter.WriteLineAsync();
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
            await ConsoleHelper.SynchronizationContext;
            // ReSharper disable once PossibleNullReferenceException
            await _outWriter.WriteLineAsync(value);
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
            await ConsoleHelper.SynchronizationContext;
            // ReSharper disable once PossibleNullReferenceException
            await _outWriter.WriteLineAsync(buffer, index, count);
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
            await ConsoleHelper.SynchronizationContext;
            // ReSharper disable once PossibleNullReferenceException
            await _outWriter.WriteLineAsync(value);
        }
    }
}