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
using System.Text;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// Holds current context of the format writer during resolution.
    /// </summary>
    public class FormatWriteContext
    {
        /// <summary>
        /// The writer width (if any); otherwise <see cref="int.MaxValue"/>.
        /// </summary>
        [PublicAPI]
        public readonly int WriterWidth;

        /// <summary>
        /// Whether the underlying writer supports color.
        /// </summary>
        [PublicAPI]
        public readonly bool IsColorSupported;

        /// <summary>
        /// Whether the underlying writer supports custom control characters.
        /// </summary>
        [PublicAPI]
        public readonly bool IsControllableWriter;

        /// <summary>
        /// Whether the underlying writer auto wraps.
        /// </summary>
        [PublicAPI]
        public readonly bool IsAutoWrapping;

        /// <summary>
        /// An object that controls formatting.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public readonly IFormatProvider FormatProvider;

        /// <summary>
        /// The character encoding in which the output is written.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public readonly Encoding Encoding;

        /// <summary>
        /// The line terminator string.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public readonly string NewLine;

        /// <summary>
        /// The current <see cref="LineType"/>.
        /// </summary>
        [PublicAPI]
        public LineType LineType { get; internal set; }

        /// <summary>
        /// The layout.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public Layout Layout { get; internal set; }

        /// <summary>
        /// The horizontal position, before any alignment has taken place.
        /// </summary>
        [PublicAPI]
        public int Position { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatWriteContext" /> class.
        /// </summary>
        /// <param name="writerWidth">Width of the writer (if any); otherwise <see cref="int.MaxValue"/>.</param>
        /// <param name="isColorSupported">if set to <see langword="true" /> the underlying writer supports color.</param>
        /// <param name="isControllableWriter">if set to <see langword="true" /> the underlying writer supports custom control characters.</param>
        /// <param name="isAutoWrapping">if set to <see langword="true" /> the underlying writer auto wraps.</param>
        /// <param name="formatProvider">An object that controls formatting.</param>
        /// <param name="encoding">The character encoding in which the output is written.</param>
        /// <param name="newLine">The line terminator string.</param>
        internal FormatWriteContext(
            int writerWidth,
            bool isColorSupported,
            bool isControllableWriter,
            bool isAutoWrapping,
            [NotNull] IFormatProvider formatProvider,
            [NotNull] Encoding encoding,
            [NotNull] string newLine)
        {
            WriterWidth = writerWidth;
            IsColorSupported = isColorSupported;
            IsControllableWriter = isControllableWriter;
            IsAutoWrapping = isAutoWrapping;
            FormatProvider = formatProvider;
            Encoding = encoding;
            NewLine = newLine;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("X:{0}; L:{1:F}", Position, Layout);
        }
    }
}