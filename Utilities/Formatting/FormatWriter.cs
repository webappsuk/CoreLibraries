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
    /// Implements a writer that uses <see cref="FormatBuilder" /> for efficiently writing chunks of complicated text to a <see cref="TextWriter"/>.
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

        #region Write Overloads
        /// <summary>
        /// Writes the format builder returned from the build factory.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="formatProvider">The format provider.</param>
        [PublicAPI]
        public void Write([CanBeNull] Func<FormatBuilder, FormatBuilder> build, [CanBeNull]IFormatProvider formatProvider = null)
        {
            if (build == null) return;
            FormatBuilder builder = build(new FormatBuilder());
            if (builder == null ||
                builder.IsEmpty) return;
            if (formatProvider == null)
                formatProvider = FormatProvider;
            string s = GetString(formatProvider, builder);
            if (!string.IsNullOrEmpty(s))
                _writer.Write(s);
        }

        /// <summary>
        /// Writes the format builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="formatProvider">The format provider.</param>
        [PublicAPI]
        public void Write([CanBeNull] FormatBuilder builder, [CanBeNull]IFormatProvider formatProvider = null)
        {
            if (builder == null ||
                builder.IsEmpty) return;
            if (formatProvider == null)
                formatProvider = FormatProvider;
            string s = GetString(formatProvider, builder);
            if (!string.IsNullOrEmpty(s))
                _writer.Write(s);
        }


        /// <summary>
        /// Writes the format builder returned from the build factory.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>Task.</returns>
        [NotNull]
        [PublicAPI]
        public Task WriteAsync([CanBeNull] Func<FormatBuilder, FormatBuilder> build, [CanBeNull]IFormatProvider formatProvider = null)
        {
            if (build == null) return TaskResult.Completed;
            FormatBuilder builder = build(new FormatBuilder());
            if (builder == null ||
                builder.IsEmpty) return TaskResult.Completed;
            if (formatProvider == null)
                formatProvider = FormatProvider;
            string s = GetString(formatProvider, builder);
            // ReSharper disable once AssignNullToNotNullAttribute
            return string.IsNullOrEmpty(s)
                ? TaskResult.Completed
                : _writer.WriteAsync(s);
        }

        /// <summary>
        /// Writes the format builder.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="builder">The builder.</param>
        [NotNull]
        [PublicAPI]
        public Task WriteAsync([CanBeNull] FormatBuilder builder, [CanBeNull]IFormatProvider formatProvider = null)
        {
            if (builder == null ||
                builder.IsEmpty) return TaskResult.Completed;
            if (formatProvider == null)
                formatProvider = FormatProvider;
            string s = GetString(formatProvider, builder);
            // ReSharper disable once AssignNullToNotNullAttribute
            return string.IsNullOrEmpty(s)
                ? TaskResult.Completed
                : _writer.WriteAsync(s);
        }
        #endregion

        /// <summary>
        /// Gets a string representation of the builder.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="builder">The builder.</param>
        /// <returns>A string representation of the chunk; otherwise <see langword="null" /> to skip.</returns>
        [CanBeNull]
        protected virtual string GetString([CanBeNull] IFormatProvider formatProvider, [NotNull] FormatBuilder builder)
        {
            Contract.Requires(formatProvider != null);
            Contract.Requires(builder != null);
            return builder.ToString(formatProvider);
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