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
using System.Globalization;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// A format chunk, holds information about a chunk of formatted Value.
    /// </summary>
    public class FormatChunk : IEquatable<FormatChunk>, IFormattable
    {
        /// <summary>
        /// The empty format chunk.
        /// </summary>
        [NotNull]
        public static readonly FormatChunk Empty = new FormatChunk(null, null, null, string.Empty, false);

        /// <summary>
        /// Control chunks are never written out when you call <see cref="ToString()"/>, but can be used by consumers of a <see cref="FormatBuilder"/> to
        /// extend functionality.
        /// </summary>
        public readonly bool IsControl;

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
        /// The chunk Value, if any.
        /// </summary>
        [CanBeNull]
        [PublicAPI]
        public readonly object Value;

        /// <summary>
        /// Gets a value indicating whether this instance is a fill point.
        /// </summary>
        /// <value><see langword="true" /> if this instance is a fill point; otherwise, <see langword="false" />.</value>
        public bool IsFillPoint
        {
            get { return Tag != null; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatChunk" /> class.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="alignment"></param>
        /// <param name="format"></param>
        /// <param name="value">The Value.</param>
        /// <param name="isControl">if set to <see langword="true" /> this is a hidden control chunk.</param>
        /// <remarks>This does not chunk the Value, however if the value is a string that starts and ends with the
        /// <see cref="FormatBuilder.OpenChar" /> and
        /// <see cref="FormatBuilder.CloseChar" /> characters, then it splits out the relevant parts.</remarks>
        private FormatChunk(
            [CanBeNull] string tag,
            [CanBeNull] int? alignment,
            [CanBeNull] string format,
            [CanBeNull] object value,
            bool isControl)
        {
            Contract.Requires(tag != null || value != null);
            Tag = tag;
            Alignment = alignment;
            Format = format;
            Value = value;
            IsControl = isControl;
        }

        /// <summary>
        /// Creates a <see cref="FormatChunk"/> from the specified value.
        /// </summary>
        /// <param name="value">The Value.</param>
        /// <returns>A <see cref="FormatChunk"/>.</returns>
        /// <remarks>
        /// This does not chunk the Value, however if the Value starts and ends with the <see cref="FormatBuilder.OpenChar"/> and
        /// <see cref="FormatBuilder.CloseChar"/> characters, then it splits out the relevant parts.</remarks>
        [NotNull]
        [PublicAPI]
        public static FormatChunk Create([CanBeNull] object value)
        {
            if (value == null) return Empty;
            string str = value as string;
            if (str == null) return new FormatChunk(null, null, null, value, false);

            if (str.Length < 1) return Empty;

            int end = str.Length - 1;
            // Is this a fill point?
            if (end < 1 ||
                str[0] != FormatBuilder.OpenChar ||
                str[end] != FormatBuilder.CloseChar) return new FormatChunk(null, null, null, str, false);

            // Find alignment splitter and format splitter characters
            int al = str.IndexOf(FormatBuilder.AlignmentChar);
            int sp = str.IndexOf(FormatBuilder.FormatChar);
            if ((sp > -1) &&
                (al > sp)) al = -1;

            // Get the format if any.
            string format = null;
            int? alignment = null;
            if (sp > -1)
            {
                sp++;
                int flen = end - sp;
                if (flen < 1) return new FormatChunk(null, null, null, str, false);
                format = str.Substring(sp, end - sp);
                end = sp - 1;
            }

            // Check the alignment is a valid integer.
            if (al > -1)
            {
                al++;
                int allen = end - al;
                if (allen < 1)
                    return new FormatChunk(null, null, null, str, false);

                string alstr = str.Substring(al, allen).Trim();
                int a;
                if (!int.TryParse(alstr, out a))
                    return new FormatChunk(null, null, null, str, false);
                alignment = a;
                end = al - 1;
            }

            // Check if we are a control tag
            bool isControl;
            string tag;
            if (str[1] == FormatBuilder.ControlChar)
            {
                isControl = true;
                if (end < 3) return new FormatChunk(null, null, null, str, false);
            }
            else
                isControl = false;
            tag = str.Substring(1, end - 1);

            return new FormatChunk(tag, alignment, format, null, isControl);
        }

        /// <summary>
        /// Creates a new <see cref="FormatChunk"/> from an existing chunk, with a new value.
        /// </summary>
        /// <param name="chunk">The chunk to copy.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        [NotNull]
        [PublicAPI]
        public static FormatChunk Create([NotNull] FormatChunk chunk, [CanBeNull] object value)
        {
            Contract.Requires(chunk != null);
            return new FormatChunk(chunk.Tag, chunk.Alignment, chunk.Format, value, chunk.IsControl);
        }

        /// <summary>
        /// Creates a control <see cref="FormatChunk" /> from the specified value.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="alignment">The alignment.</param>
        /// <param name="format">The format.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A <see cref="FormatChunk" />.
        /// </returns>
        /// <remarks>
        /// Control chunks are not written out, but can be used to extend functionality.
        /// </remarks>
        [NotNull]
        [PublicAPI]
        public static FormatChunk CreateControl(
            [CanBeNull] string tag,
            [CanBeNull] int? alignment = null,
            [CanBeNull] string format = null,
            [CanBeNull] object value = null)
        {
            return string.IsNullOrEmpty(tag)
                ? Empty
                : new FormatChunk(
                    tag[0] == FormatBuilder.ControlChar ? tag : FormatBuilder.ControlChar + tag,
                    alignment,
                    format,
                    value,
                    true);
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
            if (obj.GetType() != GetType()) return false;
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
            return string.Equals(Tag, other.Tag) && Alignment == other.Alignment && string.Equals(Format, other.Format) &&
                   Equals(Value, other.Value);
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
                hashCode = (hashCode * 397) ^ (Value != null ? Value.GetHashCode() : 0);
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
            return ToString(null, null);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public string ToString([CanBeNull] string format, [CanBeNull] IFormatProvider formatProvider)
        {
            if (format == null)
                format = "g";

            bool pad;
            string value;

            IFormattable fv = Value as IFormattable;
            switch (format.ToLowerInvariant())
            {
                // Always output's the tag if the chunk has one, otherwise output's the value as normal
                case "f":
                    if (Tag != null)
                    {
                        pad = false;
                        value = string.Format(
                            "{{{0}{1}{2}}}",
                            Tag,
                            Alignment != null
                                ? FormatBuilder.AlignmentChar + Alignment.Value.ToString("D")
                                : string.Empty,
                            !string.IsNullOrEmpty(Format) ? FormatBuilder.FormatChar + Format : string.Empty);
                    }
                    else
                    {
                        Contract.Assert(Value != null);
                        pad = true;
                        value = fv != null ? fv.ToString(Format, formatProvider) : Value.ToString();
                    }
                    break;
                
                // Always output's the control tags, otherwise output the value as normal
                case "c":
                    if (Tag != null)
                    {
                        pad = false;
                        value = string.Format(
                            "{{{0}{1}{2}}}",
                            Tag,
                            Alignment != null
                                ? FormatBuilder.AlignmentChar + Alignment.Value.ToString("D")
                                : string.Empty,
                            !string.IsNullOrEmpty(Format) ? FormatBuilder.FormatChar + Format : string.Empty);
                    }
                    else
                    {
                        Contract.Assert(Value != null);
                        pad = true;
                        value = fv != null ? fv.ToString(Format, formatProvider) : Value.ToString();
                    }
                    break;

                // Should output the value as normal, but treats unresolved tags as an empty string value
                case "s":
                    if (IsControl) return string.Empty;

                    pad = true;
                    value = Value == null
                        ? null
                        : (fv != null ? fv.ToString(Format, formatProvider) : Value.ToString());
                    break;

                // Outputs the value if set, otherwise the format tag. Control tags ignored
                default:
                    if (IsControl) return string.Empty;

                    if (Value == null)
                    {
                        Contract.Assert(Tag != null);
                        pad = false;
                        value = string.Format(
                            "{{{0}{1}{2}}}",
                            Tag,
                            Alignment != null
                                ? FormatBuilder.AlignmentChar + Alignment.Value.ToString("D")
                                : string.Empty,
                            !string.IsNullOrEmpty(Format) ? FormatBuilder.FormatChar + Format : string.Empty);
                    }
                    else
                    {
                        pad = true;
                        value = fv != null ? fv.ToString(Format, formatProvider) : Value.ToString();
                    }
                    break;
            }

            if (value == null)
                value = string.Empty;

            if (pad && Alignment != null)
                return Alignment.Value > 0
                    ? value.PadLeft(Alignment.Value)
                    : value.PadRight(-Alignment.Value);

            return value;
        }
    }
}