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
using JetBrains.Annotations;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// Base class for creating a resolvable writer for use with <see cref="FormatBuilder"/>.
    /// </summary>
    [Serializable]
    public abstract class ResolvableWriteable : Resolvable, IFormattable, IWriteable
    {
        /// <summary>
        /// Gets the default format.
        /// </summary>
        /// <value>The default format.</value>
        [NotNull]
        [PublicAPI]
        public abstract FormatBuilder DefaultFormat { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Resolvable"/> class.
        /// </summary>
        /// <param name="isCaseSensitive">if set to <see langword="true" /> then tags are case sensitive.</param>
        /// <param name="resolveOuterTags">if set to <see langword="true" />  outer tags should be resolved automatically in formats.</param>
        protected ResolvableWriteable(bool isCaseSensitive = false, bool resolveOuterTags = true)
            :base(isCaseSensitive, resolveOuterTags)
        {
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            using (TextWriter writer = new StringWriter())
            {
                WriteTo(writer);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        [StringFormatMethod("format")]
        public string ToString([CanBeNull] string format, [CanBeNull] IFormatProvider formatProvider = null)
        {
            using (TextWriter writer = new StringWriter(formatProvider))
            {
                WriteTo(writer, format);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        [NotNull]
        [PublicAPI]
        public string ToString([CanBeNull] FormatBuilder format, [CanBeNull] IFormatProvider formatProvider = null)
        {
            using (TextWriter writer = new StringWriter(formatProvider))
            {
                WriteTo(writer, format);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Writes this instance to the <see paramref="writer" />, using the optional <see paramref="format" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        // ReSharper disable once CodeAnnotationAnalyzer
        [StringFormatMethod("format")]
        // ReSharper disable once CodeAnnotationAnalyzer
        public void WriteTo(TextWriter writer, string format)
        {
            WriteTo(writer, (FormatBuilder)format);
        }

        /// <summary>
        /// Writes this instance to the <see paramref="writer" />, using the optional <see paramref="format" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        [PublicAPI]
        public virtual void WriteTo([NotNull]TextWriter writer, [CanBeNull] FormatBuilder format = null)
        {
            Contract.Requires(writer != null);
            if (format == null)
                format = DefaultFormat;

            format.WriteTo(writer, "G", (ResolveWriterDelegate) Resolve, IsCaseSensitive);
        }
    }
}