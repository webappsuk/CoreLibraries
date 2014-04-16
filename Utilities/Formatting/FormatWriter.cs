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
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// Implements a writer that uses <see cref="FormatBuilder" /> for efficiently writing chunks of complicated text.
    /// </summary>
    [PublicAPI]
    public class FormatWriter : IDisposable
    {
        /// <summary>
        /// The underlying writer.
        /// </summary>
        [NotNull]
        private readonly TextWriter _writer;

        /// <summary>
        /// The format provider.
        /// </summary>
        [NotNull]
        private readonly IFormatProvider _formatProvider;

        /// <summary>
        /// The write lock.
        /// </summary>
        [NotNull]
        protected readonly AsyncLock Lock = new AsyncLock();

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatWriter"/> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="formatProvider">The format provider.</param>
        public FormatWriter([NotNull] TextWriter writer, [CanBeNull] IFormatProvider formatProvider = null)
        {
            Contract.Requires(writer != null);
            _writer = writer;
            _formatProvider = formatProvider ?? writer.FormatProvider;
        }

        /// <summary>
        /// Gets the new line.
        /// </summary>
        /// <value>The new line.</value>
        [NotNull]
        [PublicAPI]
        public string NewLine
        {
            get { return _writer.NewLine; }
        }

        /// <summary>
        /// Gets the encoding.
        /// </summary>
        /// <value>The encoding.</value>
        [NotNull]
        [PublicAPI]
        public Encoding Encoding
        {
            get { return _writer.Encoding; }
        }

        /// <summary>
        /// Gets the format provider.
        /// </summary>
        /// <value>The format provider.</value>
        [NotNull]
        [PublicAPI]
        public IFormatProvider FormatProvider
        {
            get { return _formatProvider; }
        }
        /// <summary>
        /// Closes the current writer and releases any system resources associated with the writer.
        /// </summary>
        [PublicAPI]
        public virtual void Close()
        {
            _writer.Close();
        }

        /// <summary>
        /// Clears all buffers for the current writer and causes any buffered data to be written to the underlying device.
        /// </summary>
        [PublicAPI]
        public virtual void Flush()
        {
            _writer.Flush();
        }

        /// <summary>
        /// Writes the specified builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Write([CanBeNull] FormatBuilder builder)
        {
            WriteInternal(null, builder, false);
        }

        /// <summary>
        /// Writes the specified format provider.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="builder">The builder.</param>
        public void Write([CanBeNull] IFormatProvider formatProvider, [CanBeNull] FormatBuilder builder)
        {
            WriteInternal(formatProvider, builder, false);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        public void WriteLine()
        {
            WriteInternal(null, null, true);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void WriteLine([CanBeNull] FormatBuilder builder)
        {
            WriteInternal(null, builder, true);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="builder">The builder.</param>
        public void WriteLine([CanBeNull] IFormatProvider formatProvider, [CanBeNull] FormatBuilder builder)
        {
            WriteInternal(formatProvider, builder, true);
        }

        /// <summary>
        /// Writes the specified builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        [NotNull]
        public Task WriteAsync([CanBeNull] FormatBuilder builder)
        {
            return WriteInternalAsync(null, builder, false);
        }

        /// <summary>
        /// Writes the specified format provider.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="builder">The builder.</param>
        [NotNull]
        public Task WriteAsync([CanBeNull] IFormatProvider formatProvider, [CanBeNull] FormatBuilder builder)
        {
            return WriteInternalAsync(formatProvider, builder, false);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        [NotNull]
        public Task WriteLineAsync()
        {
            return WriteInternalAsync(null, null, true);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="builder">The builder.</param>
        [NotNull]
        public Task WriteLineAsync([CanBeNull] FormatBuilder builder)
        {
            return WriteInternalAsync(null, builder, true);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="builder">The builder.</param>
        [NotNull]
        public Task WriteLineAsync([CanBeNull] IFormatProvider formatProvider, [CanBeNull] FormatBuilder builder)
        {
            return WriteInternalAsync(formatProvider, builder, true);
        }

        /// <summary>
        /// Writes the specified builder to the output stream.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="appendNewLine">if set to <see langword="true" /> appends a new line.</param>
        private void WriteInternal(
            [CanBeNull] IFormatProvider formatProvider,
            [CanBeNull] FormatBuilder builder,
            bool appendNewLine)
        {
            if (builder == null &&
                !appendNewLine) return;
            if (formatProvider == null) formatProvider = FormatProvider;
            using (Lock.LockAsync().Result)
            {
                if (builder != null)
                    foreach (FormatChunk chunk in builder)
                    {
                        Contract.Assert(chunk != null);
                        string c = DoWriteChunk(formatProvider, chunk);
                        if (!string.IsNullOrEmpty(c))
                            _writer.Write(c);
                    }
                if (appendNewLine)
                    _writer.WriteLine();
            }
        }

        /// <summary>
        /// Writes the specified builder to the output stream.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="appendNewLine">if set to <see langword="true" /> appends a new line.</param>
        [NotNull]
        private async Task WriteInternalAsync(
            [CanBeNull] IFormatProvider formatProvider,
            [CanBeNull] FormatBuilder builder,
            bool appendNewLine)
        {
            if (builder == null &&
                !appendNewLine) return;

            if (formatProvider == null) formatProvider = FormatProvider;
            using (await Lock.LockAsync())
            {
                // ReSharper disable PossibleNullReferenceException
                if (builder != null)
                    foreach (FormatChunk chunk in builder)
                    {
                        Contract.Assert(chunk != null);
                        string c = DoWriteChunk(formatProvider, chunk);
                        if (!string.IsNullOrEmpty(c))
                            await _writer.WriteAsync(c);
                    }
                if (appendNewLine)
                    await _writer.WriteLineAsync();
                // ReSharper restore PossibleNullReferenceException
            }
        }

        /// <summary>
        /// Gets a string represent of each chunk to write.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="chunk">The chunk.</param>
        /// <returns>A string representation of the chunk; otherwise <see langword="null"/> to skip.</returns>
        [CanBeNull]
        protected virtual string DoWriteChunk(
            [NotNull] IFormatProvider formatProvider,
            [NotNull] FormatChunk chunk)
        {
            Contract.Requires(formatProvider != null);
            Contract.Requires(chunk != null);
            return chunk.ToString(formatProvider);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                ((IDisposable)_writer).Dispose();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}