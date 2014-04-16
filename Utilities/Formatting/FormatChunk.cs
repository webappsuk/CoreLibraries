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
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// A format chunk, holds information about a chunk of formatted text.
    /// </summary>
    public class FormatChunk : IEquatable<FormatChunk>, IFormattable
    {
        /// <summary>
        /// The empty format chunk.
        /// </summary>
        [NotNull]
        public static readonly FormatChunk Empty = new FormatChunk(string.Empty);

        /// <summary>
        /// The new line format chunk.
        /// </summary>
        [NotNull]
        public static readonly FormatChunk NewLine = new FormatChunk(Environment.NewLine);

        /// <summary>
        /// The tag, if this is a formatting chunk, if any; otherwise <see langword="null"/>. (e.g. '0' for '{0,-3:G}')
        /// </summary>
        [CanBeNull]
        [PublicAPI]
        public readonly string Tag;

        /// <summary>
        /// The alignment, if any; otherwise <see langword="null"/>. (e.g. -3 for '{0,-3:G}'
        /// </summary>
        [PublicAPI]
        public readonly int? Alignment;

        /// <summary>
        /// The format, if this is a formatting chunk and a format was specified, if any; otherwise <see langword="null"/>. (e.g. 'G' for '{0,-3:G}')
        /// </summary>
        [CanBeNull]
        [PublicAPI]
        public readonly string Format;

        /// <summary>
        /// The chunk text (e.g. '{0,-3:G}' for '{0,-3:G}')
        /// </summary>
        [NotNull]
        [PublicAPI]
        public readonly string Text;

        /// <summary>
        /// Gets a value indicating whether this instance is a fill point.
        /// </summary>
        /// <value><see langword="true" /> if this instance is a fill point; otherwise, <see langword="false" />.</value>
        public bool IsFillPoint { get { return Tag != null; }}

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatChunk"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <remarks>
        /// This does not chunk the text, however if the text starts and ends with the <see cref="FormatBuilder.Open"/> and
        /// <see cref="FormatBuilder.Close"/> characters, then it splits out the relevant parts.</remarks>
        private FormatChunk([NotNull] string text)
        {
            Contract.Requires(text != null);
            Text = text;

            int end = text.Length - 1;
            // Is this a fill point?
            if (end < 1 ||
                text[0] != FormatBuilder.Open ||
                text[end] != FormatBuilder.Close) return;

            // Find alignment splitter and format splitter characters
            int al = text.IndexOf(FormatBuilder.Alignment);
            int sp = text.IndexOf(FormatBuilder.Splitter);
            if ((sp > -1) && (al > sp)) al = -1;

            // Get the format if any.
            string format = null;
            int? alignment = null;
            if (sp > -1)
            {
                sp++;
                int flen = end - sp;
                if (flen < 1) return;
                format = text.Substring(sp, end - sp);
                end = sp - 1;
            }
            
            // Check the alignment is a valid integer.
            if (al > -1)
            {
                al++;
                int allen = end - al;
                if (allen < 1)
                    return;

                string alstr = text.Substring(al, allen).Trim();
                int a;
                if (!int.TryParse(alstr, out a))
                    return;
                alignment = a;
                end = al - 1;
            }

            // Get the tag
            Tag = text.Substring(1, end - 1);
            Format = format;
            Alignment = alignment;
        }

        /// <summary>
        /// Creates a <see cref="FormatChunk"/> from the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>A <see cref="FormatChunk"/>.</returns>
        /// <remarks>
        /// This does not chunk the text, however if the text starts and ends with the <see cref="FormatBuilder.Open"/> and
        /// <see cref="FormatBuilder.Close"/> characters, then it splits out the relevant parts.</remarks>
        [NotNull]
        public static FormatChunk Create([CanBeNull]string text)
        {
            if (string.IsNullOrEmpty(text))
                return Empty;
            return text == Environment.NewLine
                ? NewLine
                : new FormatChunk(text);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><see langword="true" /> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <see langword="false" />.</returns>
        public override bool Equals([CanBeNull] object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FormatChunk) obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals([CanBeNull] FormatChunk other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Tag, other.Tag) && Alignment == other.Alignment && string.Equals(Format, other.Format) && string.Equals(Text, other.Text);
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>System.Int32.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Tag != null ? Tag.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Alignment.GetHashCode();
                hashCode = (hashCode * 397) ^ (Format != null ? Format.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Text.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Implements the ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(FormatChunk left, FormatChunk right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Implements the !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(FormatChunk left, FormatChunk right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return Text;
        }
        /// <summary>
        /// To the string.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>System.String.</returns>
        [NotNull]
        public string ToString([CanBeNull] IFormatProvider formatProvider)
        {
            return Text;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public string ToString([CanBeNull] string format, [CanBeNull] IFormatProvider formatProvider)
        {
            return Text;
        }

        /// <summary>
        /// Creates a new, non-fill point chunk from the value.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="value">The value.</param>
        /// <returns>FormatChunk.</returns>
        [NotNull]
        public FormatChunk FormatValue([CanBeNull]IFormatProvider provider, [CanBeNull]object value)
        {
            if (provider == null)
                provider = Thread.CurrentThread.CurrentCulture;

            string v;
            if (value == null) v = string.Empty;
            else
            {
                IFormattable fv = value as IFormattable;
                try
                {
                    v = fv == null ? value.ToString() : fv.ToString(Format, provider);
                }
                catch
                {
                    return this;
                }
            }

            if (Alignment != null)
            {
                int a = Alignment.Value;
                v = a < 0 ? v.PadRight(-a) : v.PadLeft(a);
            }

            return Create(v);
        }
    }
}