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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// Build a formatted string, which can be used to enumerate FormatChunks
    /// </summary>
    public class FormatBuilder : IEnumerable<FormatChunk>, IFormattable
    {
        /// <summary>
        /// The first character of a fill point.
        /// </summary>
        public const char OpenChar = '{';

        /// <summary>
        /// The last character of a fill point.
        /// </summary>
        public const char CloseChar = '}';

        /// <summary>
        /// The control character precedes a tag to indicate it is a control chunk.
        /// </summary>
        public const char ControlChar = '!';

        /// <summary>
        /// The alignment character separates the tag from an alignment
        /// </summary>
        public const char AlignmentChar = ',';

        /// <summary>
        /// The splitter character separates the tag/alignment from the format.
        /// </summary>
        public const char FormatChar = ':';

        [NotNull]
        private readonly List<FormatChunk> _chunks = new List<FormatChunk>();

        /// <summary>
        /// Whether this builder is readonly
        /// </summary>
        private bool _isReadonly;

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<FormatChunk> GetEnumerator()
        {
            return _chunks.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        [PublicAPI]
        public void Clear()
        {
            _chunks.Clear();
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><see langword="true" /> if this instance is empty; otherwise, <see langword="false" />.</value>
        [PublicAPI]
        public bool IsEmpty
        {
            get { return _chunks.Count < 1; }
        }

        /// <summary>
        /// Gets a value indicating whether this builder is readonly.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this builder is readonly; otherwise, <see langword="false" />.
        /// </value>
        /// <remarks>A readonly builder cannot have any more chunks appended, but fill points can still be resolved.</remarks>
        [PublicAPI]
        public bool IsReadonly
        {
            get { return _isReadonly; }
        }

        /// <summary>
        /// Makes this builder readonly.
        /// </summary>
        [PublicAPI]
        public void MakeReadonly()
        {
            _isReadonly = true;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <param name="makeReadonly">If set to <see langword="true" />, the returned builder will be readonly.</param>
        /// <returns>
        /// A shallow copy of this builder.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public virtual FormatBuilder Clone(bool makeReadonly = false)
        {
            Contract.Ensures(Contract.Result<FormatBuilder>().GetType() == this.GetType(),
                "All classes derived from FormatBuilder should overload this method and return a builder of their own type");
            Contract.Ensures(!makeReadonly || Contract.Result<FormatBuilder>().IsReadonly,
                "Returned builder should be readonly if makeReadonly is true");

            if (IsReadonly)
                return this;

            FormatBuilder formatBuilder = new FormatBuilder();
            formatBuilder._chunks.AddRange(_chunks);
            if (makeReadonly)
                formatBuilder.MakeReadonly();
            return formatBuilder;
        }

        #region Append overloads
        /// <summary>
        /// Appends the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Append(bool value)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            _chunks.Add(FormatChunk.Create(value));
            return this;
        }

        /// <summary>
        /// Appends the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Append(sbyte value)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            _chunks.Add(FormatChunk.Create(value));
            return this;
        }

        /// <summary>
        /// Appends the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Append(byte value)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            _chunks.Add(FormatChunk.Create(value));
            return this;
        }

        /// <summary>
        /// Appends the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Append(char value)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            _chunks.Add(FormatChunk.Create(value));
            return this;
        }

        /// <summary>
        /// Appends the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Append(short value)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            _chunks.Add(FormatChunk.Create(value));
            return this;
        }

        /// <summary>
        /// Appends the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Append(int value)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            _chunks.Add(FormatChunk.Create(value));
            return this;
        }

        /// <summary>
        /// Appends the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Append(long value)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            _chunks.Add(FormatChunk.Create(value));
            return this;
        }

        /// <summary>
        /// Appends the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Append(float value)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            _chunks.Add(FormatChunk.Create(value));
            return this;
        }

        /// <summary>
        /// Appends the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Append(double value)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            _chunks.Add(FormatChunk.Create(value));
            return this;
        }

        /// <summary>
        /// Appends the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Append(decimal value)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            _chunks.Add(FormatChunk.Create(value));
            return this;
        }

        /// <summary>
        /// Appends the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Append(ushort value)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            _chunks.Add(FormatChunk.Create(value));
            return this;
        }

        /// <summary>
        /// Appends the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Append(uint value)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            _chunks.Add(FormatChunk.Create(value));
            return this;
        }

        /// <summary>
        /// Appends the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Append(ulong value)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            _chunks.Add(FormatChunk.Create(value));
            return this;
        }

        /// <summary>
        /// Appends the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Append([CanBeNull] object value)
        {
            Contract.Requires(!IsReadonly);
            if (!_isReadonly && value != null)
                _chunks.AddRange(Resolve(FormatChunk.Create(value)));
            return this;
        }

        /// <summary>
        /// Appends the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Append([CanBeNull] char[] value)
        {
            Contract.Requires(!IsReadonly);
            if (!_isReadonly &&
                (value != null) &&
                (value.Length > 0))
                _chunks.Add(FormatChunk.Create(new string(value)));
            return this;
        }

        /// <summary>
        /// Appends the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="charCount">The character count.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Append([CanBeNull] char[] value, int startIndex, int charCount)
        {
            Contract.Requires(!IsReadonly);
            if (!_isReadonly &&
                (value != null) &&
                (value.Length > 0) &&
                (startIndex >= 0) &&
                (charCount >= 0))
            {
                if (startIndex + charCount > value.Length)
                    charCount = value.Length - startIndex;
                _chunks.Add(FormatChunk.Create(new string(value, startIndex, charCount)));
            }
            return this;
        }

        /// <summary>
        /// Appends the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="repeatCount">The repeat count.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Append(char value, int repeatCount)
        {
            Contract.Requires(!IsReadonly);
            if (!_isReadonly &&
                repeatCount > 0)
                _chunks.Add(FormatChunk.Create(new string(value, repeatCount)));
            return this;
        }

        /// <summary>
        /// Appends the string, without additional formatting.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Append([CanBeNull] string value)
        {
            Contract.Requires(!IsReadonly);
            if (!_isReadonly &&
                !string.IsNullOrEmpty(value))
                _chunks.Add(FormatChunk.Create(value));
            return this;
        }

        /// <summary>
        /// Appends the chunks.
        /// </summary>
        /// <param name="chunk">The chunk.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Append([CanBeNull] FormatChunk chunk)
        {
            Contract.Requires(!IsReadonly);
            if (!_isReadonly &&
                chunk != null)
                _chunks.AddRange(Resolve(chunk));
            return this;
        }

        /// <summary>
        /// Appends the chunks.
        /// </summary>
        /// <param name="chunks">The chunks.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Append([CanBeNull] [InstantHandle] IEnumerable<FormatChunk> chunks)
        {
            Contract.Requires(!IsReadonly);
            if (!_isReadonly &&
                chunks != null)
                _chunks.AddRange(chunks.SelectMany(Resolve));
            return this;
        }

        /// <summary>
        /// Appends the builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Append([CanBeNull] FormatBuilder builder)
        {
            Contract.Requires(!IsReadonly);
            if (!_isReadonly &&
                builder != null &&
                !builder.IsEmpty)
                _chunks.AddRange(builder);
            return this;
        }
        #endregion

        #region AppendLine overloads
        /// <summary>
        /// Appends a line.
        /// </summary>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendLine()
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            _chunks.Add(FormatChunk.Create(Environment.NewLine));
            return this;
        }

        /// <summary>
        /// Appends the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendLine(bool value)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            _chunks.Add(FormatChunk.Create(value));
            _chunks.Add(FormatChunk.Create(Environment.NewLine));
            return this;
        }

        /// <summary>
        /// Appends the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendLine(sbyte value)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            _chunks.Add(FormatChunk.Create(value));
            _chunks.Add(FormatChunk.Create(Environment.NewLine));
            return this;
        }

        /// <summary>
        /// Appends the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendLine(byte value)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            _chunks.Add(FormatChunk.Create(value));
            _chunks.Add(FormatChunk.Create(Environment.NewLine));
            return this;
        }

        /// <summary>
        /// Appends the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendLine(char value)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            _chunks.Add(FormatChunk.Create(value));
            _chunks.Add(FormatChunk.Create(Environment.NewLine));
            return this;
        }

        /// <summary>
        /// Appends the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendLine(short value)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            _chunks.Add(FormatChunk.Create(value));
            _chunks.Add(FormatChunk.Create(Environment.NewLine));
            return this;
        }

        /// <summary>
        /// Appends the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendLine(int value)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            _chunks.Add(FormatChunk.Create(value));
            _chunks.Add(FormatChunk.Create(Environment.NewLine));
            return this;
        }

        /// <summary>
        /// Appends the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendLine(long value)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            _chunks.Add(FormatChunk.Create(value));
            _chunks.Add(FormatChunk.Create(Environment.NewLine));
            return this;
        }

        /// <summary>
        /// Appends the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendLine(float value)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            _chunks.Add(FormatChunk.Create(value));
            _chunks.Add(FormatChunk.Create(Environment.NewLine));
            return this;
        }

        /// <summary>
        /// Appends the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendLine(double value)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            _chunks.Add(FormatChunk.Create(value));
            _chunks.Add(FormatChunk.Create(Environment.NewLine));
            return this;
        }

        /// <summary>
        /// Appends the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendLine(decimal value)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            _chunks.Add(FormatChunk.Create(value));
            _chunks.Add(FormatChunk.Create(Environment.NewLine));
            return this;
        }

        /// <summary>
        /// Appends the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendLine(ushort value)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            _chunks.Add(FormatChunk.Create(value));
            _chunks.Add(FormatChunk.Create(Environment.NewLine));
            return this;
        }

        /// <summary>
        /// Appends the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendLine(uint value)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            _chunks.Add(FormatChunk.Create(value));
            _chunks.Add(FormatChunk.Create(Environment.NewLine));
            return this;
        }

        /// <summary>
        /// Appends the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendLine(ulong value)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            _chunks.Add(FormatChunk.Create(value));
            _chunks.Add(FormatChunk.Create(Environment.NewLine));
            return this;
        }

        /// <summary>
        /// Appends the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendLine([CanBeNull] object value)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            if (value != null)
                _chunks.AddRange(Resolve(FormatChunk.Create(value)));
            _chunks.Add(FormatChunk.Create(Environment.NewLine));
            return this;
        }

        /// <summary>
        /// Appends the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendLine([CanBeNull] char[] value)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            if ((value != null) &&
                (value.Length > 0))
                _chunks.Add(FormatChunk.Create(new string(value)));
            _chunks.Add(FormatChunk.Create(Environment.NewLine));
            return this;
        }

        /// <summary>
        /// Appends the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="charCount">The character count.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendLine([CanBeNull] char[] value, int startIndex, int charCount)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            if ((value != null) &&
                (value.Length > 0) &&
                (startIndex >= 0) &&
                (charCount >= 0))
            {
                if (startIndex + charCount > value.Length)
                    charCount = value.Length - startIndex;
                _chunks.Add(FormatChunk.Create(new string(value, startIndex, charCount)));
            }
            _chunks.Add(FormatChunk.Create(Environment.NewLine));
            return this;
        }

        /// <summary>
        /// Appends the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="repeatCount">The repeat count.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendLine(char value, int repeatCount)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            if (repeatCount > 0)
                _chunks.Add(FormatChunk.Create(new string(value, repeatCount)));
            _chunks.Add(FormatChunk.Create(Environment.NewLine));
            return this;
        }

        /// <summary>
        /// Appends the string, without additional formatting.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendLine([CanBeNull] string value)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            if (!string.IsNullOrEmpty(value))
                _chunks.Add(FormatChunk.Create(value));
            _chunks.Add(FormatChunk.Create(Environment.NewLine));
            return this;
        }

        /// <summary>
        /// Appends the chunks.
        /// </summary>
        /// <param name="chunk">The chunk.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendLine([CanBeNull] FormatChunk chunk)
        {
            Contract.Requires(!IsReadonly);
            if (!_isReadonly &&
                chunk != null)
                _chunks.AddRange(Resolve(chunk));
            _chunks.Add(FormatChunk.Create(Environment.NewLine));
            return this;
        }

        /// <summary>
        /// Appends the chunks.
        /// </summary>
        /// <param name="chunks">The chunks.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendLine([CanBeNull] [InstantHandle] IEnumerable<FormatChunk> chunks)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            if (chunks != null)
                _chunks.AddRange(chunks.SelectMany(Resolve));
            _chunks.Add(FormatChunk.Create(Environment.NewLine));
            return this;
        }

        /// <summary>
        /// Appends the builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendLine([CanBeNull] FormatBuilder builder)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            if (builder != null &&
                !builder.IsEmpty)
                _chunks.AddRange(builder);
            _chunks.Add(FormatChunk.Create(Environment.NewLine));
            return this;
        }
        #endregion

        #region AppendFormat overloads
        /// <summary>
        /// Appends the specified format string, replacing any integer tags with the matching arguments (overriding pre-specified replacements).
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        [StringFormatMethod("format")]
        public FormatBuilder AppendFormat([CanBeNull] string format, [CanBeNull] params object[] args)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            if (!string.IsNullOrEmpty(format))
            {
                _chunks.AddRange(
                    args != null && args.Length > 0
                        ? Resolve(format.FormatChunks(), args)
                        : format.FormatChunks());
            }
            return this;
        }

        /// <summary>
        /// Appends the specified format string, replacing any integer tags with the matching arguments (overriding pre-specified replacements).
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendFormat(
            [CanBeNull] string format,
            [CanBeNull] IReadOnlyDictionary<string, object> values)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            if (!string.IsNullOrEmpty(format))
            {
                _chunks.AddRange(
                    values != null && values.Count > 0
                        ? Resolve(format.FormatChunks(), values)
                        : format.FormatChunks());
            }
            return this;
        }

        /// <summary>
        /// Appends the specified format string, replacing any integer tags with the matching arguments (overriding pre-specified replacements).
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns>
        /// This instance.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendFormat(
            [CanBeNull] string format,
            [CanBeNull][InstantHandle] Func<FormatChunk, Optional<object>> resolver)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            if (!string.IsNullOrEmpty(format))
            {
                _chunks.AddRange(
                    resolver != null
                        ? Resolve(format.FormatChunks(), resolver)
                        : format.FormatChunks());
            }
            return this;
        }
        #endregion

        #region AppendFormatLine overloads
        /// <summary>
        /// Appends a new line.
        /// </summary>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendFormatLine()
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            _chunks.Add(FormatChunk.Create(Environment.NewLine));
            return this;
        }

        /// <summary>
        /// Appends the specified format string, replacing any integer tags with the matching arguments (overriding pre-specified replacements).
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        [StringFormatMethod("format")]
        public FormatBuilder AppendFormatLine([CanBeNull] string format, [CanBeNull] params object[] args)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            if (!string.IsNullOrEmpty(format))
            {
                _chunks.AddRange(
                    args != null && args.Length > 0
                        ? Resolve(format.FormatChunks(), args)
                        : format.FormatChunks());
            }
            _chunks.Add(FormatChunk.Create(Environment.NewLine));
            return this;
        }

        /// <summary>
        /// Appends the specified format string, replacing any integer tags with the matching arguments (overriding pre-specified replacements).
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendFormatLine(
            [CanBeNull] string format,
            [CanBeNull] IReadOnlyDictionary<string, object> values)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            if (!string.IsNullOrEmpty(format))
            {
                _chunks.AddRange(
                    values != null && values.Count > 0
                        ? Resolve(format.FormatChunks(), values)
                        : format.FormatChunks());
            }
            _chunks.Add(FormatChunk.Create(Environment.NewLine));
            return this;
        }

        /// <summary>
        /// Appends the specified format string, replacing any integer tags with the matching arguments (overriding pre-specified replacements).
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns>
        /// This instance.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendFormatLine(
            [CanBeNull] string format,
            [CanBeNull][InstantHandle] Func<FormatChunk, Optional<object>> resolver)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            if (!string.IsNullOrEmpty(format))
            {
                _chunks.AddRange(
                    resolver != null
                        ? Resolve(format.FormatChunks(), resolver)
                        : format.FormatChunks());
            }
            _chunks.Add(FormatChunk.Create(Environment.NewLine));
            return this;
        }
        #endregion

        #region AppendControl
        /// <summary>
        /// Appends the control object for controlling formatting.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="alignment">The alignment.</param>
        /// <param name="format">The format.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// This instance.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendControl(
            [CanBeNull] string tag,
            [CanBeNull] int? alignment = null,
            [CanBeNull] string format = null,
            [CanBeNull] object value = null)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;

            FormatChunk chunk = FormatChunk.CreateControl(tag, alignment, format, value);

            if (value != null)
                _chunks.AddRange(Resolve(chunk));
            else
                _chunks.Add(chunk);
            return this;
        }

        /// <summary>
        /// Appends the control object for controlling formatting.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendControl([NotNull] FormatChunk control)
        {
            Contract.Requires(control != null);
            Contract.Requires(control.IsControl);
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;

            if (control.Value != null)
                _chunks.AddRange(Resolve(control));
            else
                _chunks.Add(control);

            return this;
        }
        #endregion

        #region Resolve Overloads
        /// <summary>
        /// Resolves the specified chunk.
        /// </summary>
        /// <param name="chunk">The chunk.</param>
        /// <returns>An enumeration of chunks.</returns>
        [NotNull]
        private static IEnumerable<FormatChunk> Resolve([NotNull]FormatChunk chunk)
        {
            Contract.Requires(chunk != null);
            Stack<FormatChunk> stack = new Stack<FormatChunk>();
            stack.Push(chunk);
            do
            {
                FormatChunk c;
                if (stack.Count > 0)
                    c = stack.Pop();
                else
                    yield break;

                if (c == null) continue;

                if (!c.IsFillPoint)
                {
                    yield return c;
                    continue;
                }

                object value = c.Value;

                IEnumerable<FormatChunk> cs = value as IEnumerable<FormatChunk>;
                if (cs != null)
                {
                    // Push chunks in reverse order.
                    foreach (FormatChunk ic in cs.Reverse())
                        stack.Push(ic);
                    continue;
                }

                FormatChunk fc = value as FormatChunk;
                if (fc != null)
                {
                    stack.Push(fc);
                    continue;
                }

                yield return c;
            } while (true);
        }

        /// <summary>
        /// Resolves the specified chunks.
        /// </summary>
        /// <param name="chunks">The chunks.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        [NotNull]
        protected static IEnumerable<FormatChunk> Resolve(
            [NotNull] IEnumerable<FormatChunk> chunks,
            [NotNull] object[] values)
        {
            Contract.Requires(chunks != null);
            Contract.Requires(values != null);
            Stack<FormatChunk> stack = new Stack<FormatChunk>();
            IEnumerator<FormatChunk> ce = chunks.GetEnumerator();
            do
            {
                FormatChunk chunk;
                if (stack.Count > 0)
                    chunk = stack.Pop();
                else if (!ce.MoveNext())
                    yield break;
                else chunk = ce.Current;

                if (chunk == null) continue;

                if (!chunk.IsFillPoint)
                {
                    yield return chunk;
                    continue;
                }

                int i;
                bool found = int.TryParse(chunk.Tag, out i) &&
                             (i >= 0) &&
                             (i < values.Length);
                object value = found
                    ? values[i]
                    : chunk.Value;

                IEnumerable<FormatChunk> cs = value as IEnumerable<FormatChunk>;
                if (cs != null)
                {
                    // Push chunks in reverse order.
                    foreach (FormatChunk c in cs.Reverse())
                        stack.Push(c);
                    continue;
                }

                FormatChunk fc = value as FormatChunk;
                if (fc != null)
                {
                    stack.Push(fc);
                    continue;
                }

                if (!found || (chunk.Value == value))
                {
                    yield return chunk;
                    continue;
                }

                yield return FormatChunk.Create(chunk, value);
            } while (true);
        }

        /// <summary>
        /// Resolves the specified chunks.
        /// </summary>
        /// <param name="chunks">The chunks.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        [NotNull]
        protected static IEnumerable<FormatChunk> Resolve(
            [NotNull] IEnumerable<FormatChunk> chunks,
            [NotNull] IReadOnlyDictionary<string, object> values)
        {
            Contract.Requires(chunks != null);
            Contract.Requires(values != null);
            Stack<FormatChunk> stack = new Stack<FormatChunk>();
            IEnumerator<FormatChunk> ce = chunks.GetEnumerator();
            do
            {
                FormatChunk chunk;
                if (stack.Count > 0)
                    chunk = stack.Pop();
                else if (!ce.MoveNext())
                    yield break;
                else chunk = ce.Current;

                if (chunk == null) continue;

                if (!chunk.IsFillPoint)
                {
                    yield return chunk;
                    continue;
                }

                object value;
                bool found = values.TryGetValue(chunk.Tag, out value);
                if (!found)
                    value = chunk.Value;

                IEnumerable<FormatChunk> cs = value as IEnumerable<FormatChunk>;
                if (cs != null)
                {
                    // Push chunks in reverse order.
                    foreach (FormatChunk c in cs.Reverse())
                        stack.Push(c);
                    continue;
                }

                FormatChunk fc = value as FormatChunk;
                if (fc != null)
                {
                    stack.Push(fc);
                    continue;
                }

                if (!found || (chunk.Value == value))
                {
                    yield return chunk;
                    continue;
                }

                yield return FormatChunk.Create(chunk, value);
            } while (true);
        }

        /// <summary>
        /// Resolves the specified chunks.
        /// </summary>
        /// <param name="chunks">The chunks.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns></returns>
        [NotNull]
        protected static IEnumerable<FormatChunk> Resolve(
            [NotNull] IEnumerable<FormatChunk> chunks,
            [NotNull] Func<FormatChunk, Optional<object>> resolver)
        {
            Contract.Requires(chunks != null);
            Contract.Requires(resolver != null);
            Stack<FormatChunk> stack = new Stack<FormatChunk>();
            IEnumerator<FormatChunk> ce = chunks.GetEnumerator();
            do
            {
                FormatChunk chunk;
                if (stack.Count > 0)
                    chunk = stack.Pop();
                else if (!ce.MoveNext())
                    yield break;
                else chunk = ce.Current;

                if (chunk == null) continue;

                if (!chunk.IsFillPoint)
                {
                    yield return chunk;
                    continue;
                }

                Optional<object> resolved = resolver(chunk);
                object value = resolved.IsAssigned ? resolved.Value : chunk.Value;

                IEnumerable<FormatChunk> cs = value as IEnumerable<FormatChunk>;
                if (cs != null)
                {
                    // Push chunks in reverse order.
                    foreach (FormatChunk c in cs.Reverse())
                        stack.Push(c);
                    continue;
                }

                FormatChunk fc = value as FormatChunk;
                if (fc != null)
                {
                    stack.Push(fc);
                    continue;
                }

                if (!resolved.IsAssigned ||
                    (chunk.Value == value))
                {
                    yield return chunk;
                    continue;
                }

                yield return FormatChunk.Create(chunk, value);
            } while (true);
        }
        #endregion

        #region ToString Overloads
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            using (StringWriter writer = new StringWriter())
            {
                WriteTo(writer, null);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public string ToString([CanBeNull] params object[] values)
        {
            using (StringWriter writer = new StringWriter())
            {
                WriteTo(writer, null, values);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public string ToString([CanBeNull] [InstantHandle] IEnumerable<object> values)
        {
            using (StringWriter writer = new StringWriter())
            {
                WriteTo(writer, null, values);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public string ToString([CanBeNull] IReadOnlyDictionary<string, object> values)
        {
            using (StringWriter writer = new StringWriter())
            {
                WriteTo(writer, null, values);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="resolver">The values.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public string ToString([CanBeNull] [InstantHandle] Func<FormatChunk, Optional<object>> resolver)
        {
            using (StringWriter writer = new StringWriter())
            {
                WriteTo(writer, null, resolver);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        [NotNull]
        [PublicAPI]
        public string ToString([CanBeNull] IFormatProvider formatProvider)
        {
            using (StringWriter writer = new StringWriter(formatProvider))
            {
                WriteTo(writer, null);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="values">The values.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public string ToString([CanBeNull] IFormatProvider formatProvider, [CanBeNull] params object[] values)
        {
            using (StringWriter writer = new StringWriter(formatProvider))
            {
                WriteTo(writer, null, values);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="values">The values.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public string ToString(
            [CanBeNull] IFormatProvider formatProvider,
            [CanBeNull] [InstantHandle] IEnumerable<object> values)
        {
            using (StringWriter writer = new StringWriter(formatProvider))
            {
                WriteTo(writer, null, values);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="values">The values.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public string ToString(
            [CanBeNull] IFormatProvider formatProvider,
            [CanBeNull] IReadOnlyDictionary<string, object> values)
        {
            using (StringWriter writer = new StringWriter(formatProvider))
            {
                WriteTo(writer, null, values);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="resolver">The values.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public string ToString(
            [CanBeNull] IFormatProvider formatProvider,
            [CanBeNull] [InstantHandle] Func<FormatChunk, Optional<object>> resolver)
        {
            using (StringWriter writer = new StringWriter(formatProvider))
            {
                WriteTo(writer, null, resolver);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="format">The format. 
        /// <list type="table">
        ///     <listheader> <term>Format string</term> <description>Description</description> </listheader>
        ///     <item> <term>G/g/null</term> <description>Any unresolved fill points will have their tags output. Control chunks are ignored.</description> </item>
        ///     <item> <term>F/f</term> <description>All control and fill point chunks will have their tags output.</description> </item>
        ///     <item> <term>S/s</term> <description>Any unresolved fill points will be treated as an empty string. Control chunks are ignored.</description> </item>
        /// </list></param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>A <see cref="System.String"/> that represents this instance. </returns>
        [PublicAPI]
        public virtual string ToString([CanBeNull] string format, [CanBeNull] IFormatProvider formatProvider = null)
        {
            using (StringWriter writer = new StringWriter(formatProvider))
            {
                WriteTo(writer, format);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format. 
        /// <list type="table">
        ///     <listheader> <term>Format string</term> <description>Description</description> </listheader>
        ///     <item> <term>G/g/null</term> <description>Any unresolved fill points will have their tags output. Control chunks are ignored.</description> </item>
        ///     <item> <term>F/f</term> <description>All control and fill point chunks will have their tags output.</description> </item>
        ///     <item> <term>S/s</term> <description>Any unresolved fill points will be treated as an empty string. Control chunks are ignored.</description> </item>
        /// </list></param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="values">The values.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public string ToString(
            [CanBeNull] string format,
            [CanBeNull] IFormatProvider formatProvider,
            [CanBeNull] params object[] values)
        {
            using (StringWriter writer = new StringWriter(formatProvider))
            {
                WriteTo(writer, format, values);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format. 
        /// <list type="table">
        ///     <listheader> <term>Format string</term> <description>Description</description> </listheader>
        ///     <item> <term>G/g/null</term> <description>Any unresolved fill points will have their tags output. Control chunks are ignored.</description> </item>
        ///     <item> <term>F/f</term> <description>All control and fill point chunks will have their tags output.</description> </item>
        ///     <item> <term>S/s</term> <description>Any unresolved fill points will be treated as an empty string. Control chunks are ignored.</description> </item>
        /// </list></param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="values">The values.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public string ToString(
            [CanBeNull] string format,
            [CanBeNull] IFormatProvider formatProvider,
            [CanBeNull] [InstantHandle] IEnumerable<object> values)
        {
            using (StringWriter writer = new StringWriter(formatProvider))
            {
                WriteTo(writer, format, values);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format. 
        /// <list type="table">
        ///     <listheader> <term>Format string</term> <description>Description</description> </listheader>
        ///     <item> <term>G/g/null</term> <description>Any unresolved fill points will have their tags output. Control chunks are ignored.</description> </item>
        ///     <item> <term>F/f</term> <description>All control and fill point chunks will have their tags output.</description> </item>
        ///     <item> <term>S/s</term> <description>Any unresolved fill points will be treated as an empty string. Control chunks are ignored.</description> </item>
        /// </list></param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="values">The values.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public string ToString(
            [CanBeNull] string format,
            [CanBeNull] IFormatProvider formatProvider,
            [CanBeNull] IReadOnlyDictionary<string, object> values)
        {
            using (StringWriter writer = new StringWriter(formatProvider))
            {
                WriteTo(writer, format, values);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format.
        /// <list type="table">
        /// <listheader> <term>Format string</term> <description>Description</description> </listheader>
        /// <item> <term>G/g/null</term> <description>Any unresolved fill points will have their tags output. Control chunks are ignored.</description> </item>
        /// <item> <term>F/f</term> <description>All control and fill point chunks will have their tags output.</description> </item>
        /// <item> <term>S/s</term> <description>Any unresolved fill points will be treated as an empty string. Control chunks are ignored.</description> </item>
        /// </list></param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public string ToString(
            [CanBeNull] string format,
            [CanBeNull] IFormatProvider formatProvider,
            [CanBeNull] Func<FormatChunk, Optional<object>> resolver)
        {
            using (StringWriter writer = new StringWriter(formatProvider))
            {
                WriteTo(writer, format, resolver);
                return writer.ToString();
            }
        }
        #endregion

        #region WriteToConsole Overloads
        /// <summary>
        /// Writes the builder to the console.
        /// </summary>
        [PublicAPI]
        public void WriteToConsole()
        {
            WriteToInternal(_chunks, ConsoleTextWriter.Default, "G");
        }

        /// <summary>
        /// Writes the builder to the console.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        [PublicAPI]
        public void WriteToConsole(
            [CanBeNull] string format,
            [CanBeNull] params object[] values)
        {
            if (format == null)
                format = "G";
            WriteToInternal(
                values != null && values.Length > 0
                    ? Resolve(_chunks, values)
                    : _chunks,
                ConsoleTextWriter.Default,
                format);
        }

        /// <summary>
        /// Writes the builder to the console.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        [PublicAPI]
        public void WriteToConsole(
            [CanBeNull] string format,
            [CanBeNull] [InstantHandle] IEnumerable<object> values)
        {
            if (format == null)
                format = "G";
            if (values != null)
            {
                object[] vArray = values.ToArray();
                if (vArray.Length > 0)
                {
                    WriteToInternal(
                        Resolve(_chunks, vArray),
                        ConsoleTextWriter.Default,
                        format);
                    return;
                }
            }

            WriteToInternal(
                _chunks,
                ConsoleTextWriter.Default,
                format);
        }

        /// <summary>
        /// Writes the builder to the console.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        [PublicAPI]
        public void WriteToConsole(
            [CanBeNull] string format,
            [CanBeNull] IReadOnlyDictionary<string, object> values)
        {
            if (format == null)
                format = "G";
            WriteToInternal(
                values != null && values.Count > 0
                    ? Resolve(_chunks, values)
                    : _chunks,
                ConsoleTextWriter.Default,
                format);
        }

        /// <summary>
        /// Writes the builder to the console.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="resolver">The resolver.</param>
        [PublicAPI]
        public void WriteToConsole(
            [CanBeNull] string format,
            [CanBeNull] [InstantHandle] Func<FormatChunk, Optional<object>> resolver)
        {
            if (format == null)
                format = "G";
            WriteToInternal(
                resolver != null
                    ? Resolve(_chunks, resolver)
                    : _chunks,
                ConsoleTextWriter.Default,
                format);
        }
        #endregion

        #region WriteToTrace Overloads
        /// <summary>
        /// Writes the builder to <see cref="Trace"/>.
        /// </summary>
        [PublicAPI]
        public void WriteToTrace()
        {
            WriteToInternal(_chunks, TraceTextWriter.Default, "G");
        }

        /// <summary>
        /// Writes the builder to <see cref="Trace" />.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        [PublicAPI]
        public void WriteToTrace(
            [CanBeNull] string format,
            [CanBeNull] params object[] values)
        {
            if (format == null)
                format = "G";
            WriteToInternal(
                values != null && values.Length > 0
                    ? Resolve(_chunks, values)
                    : _chunks,
                TraceTextWriter.Default,
                format);
        }

        /// <summary>
        /// Writes the builder to <see cref="Trace" />.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        [PublicAPI]
        public void WriteToTrace(
            [CanBeNull] string format,
            [CanBeNull] [InstantHandle] IEnumerable<object> values)
        {
            if (format == null)
                format = "G";
            if (values != null)
            {
                object[] vArray = values.ToArray();
                if (vArray.Length > 0)
                {
                    WriteToInternal(
                        Resolve(_chunks, vArray),
                        TraceTextWriter.Default,
                        format);
                    return;
                }
            }

            WriteToInternal(
                _chunks,
                TraceTextWriter.Default,
                format);
        }

        /// <summary>
        /// Writes the builder to <see cref="Trace" />.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        [PublicAPI]
        public void WriteToTrace(
            [CanBeNull] string format,
            [CanBeNull] IReadOnlyDictionary<string, object> values)
        {
            if (format == null)
                format = "G";
            WriteToInternal(
                values != null && values.Count > 0
                    ? Resolve(_chunks, values)
                    : _chunks,
                TraceTextWriter.Default,
                format);
        }

        /// <summary>
        /// Writes the builder to <see cref="Trace" />.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="resolver">The resolver.</param>
        [PublicAPI]
        public void WriteToTrace(
            [CanBeNull] string format,
            [CanBeNull] [InstantHandle] Func<FormatChunk, Optional<object>> resolver)
        {
            if (format == null)
                format = "G";
            WriteToInternal(
                resolver != null
                    ? Resolve(_chunks, resolver)
                    : _chunks,
                TraceTextWriter.Default,
                format);
        }
        #endregion

        #region WriteTo Overloads
        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        [PublicAPI]
        public void WriteTo([CanBeNull] TextWriter writer)
        {
            if (writer == null) return;
            WriteToInternal(_chunks, writer, "G");
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        [PublicAPI]
        public void WriteTo(
            [CanBeNull] TextWriter writer,
            [CanBeNull] string format,
            [CanBeNull] params object[] values)
        {
            if (writer == null) return;
            if (format == null)
                format = "G";
            WriteToInternal(
                values != null && values.Length > 0
                    ? Resolve(_chunks, values)
                    : _chunks,
                writer,
                format);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        [PublicAPI]
        public void WriteTo(
            [CanBeNull] TextWriter writer,
            [CanBeNull] string format,
            [CanBeNull] [InstantHandle] IEnumerable<object> values)
        {
            if (writer == null) return;
            if (format == null)
                format = "G";
            if (values != null)
            {
                object[] vArray = values.ToArray();
                if (vArray.Length > 0)
                {
                    WriteToInternal(
                        Resolve(_chunks, vArray),
                        writer,
                        format);
                    return;
                }
            }

            WriteToInternal(
                _chunks,
                writer,
                format);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        [PublicAPI]
        public void WriteTo(
            [CanBeNull] TextWriter writer,
            [CanBeNull] string format,
            [CanBeNull] IReadOnlyDictionary<string, object> values)
        {
            if (writer == null) return;
            if (format == null)
                format = "G";
            WriteToInternal(
                values != null && values.Count > 0
                    ? Resolve(_chunks, values)
                    : _chunks,
                writer,
                format);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <param name="resolver">The resolver.</param>
        [PublicAPI]
        public void WriteTo(
            [CanBeNull] TextWriter writer,
            [CanBeNull] string format,
            [CanBeNull] [InstantHandle] Func<FormatChunk, Optional<object>> resolver)
        {
            if (writer == null) return;
            if (format == null)
                format = "G";
            WriteToInternal(
                resolver != null
                    ? Resolve(_chunks, resolver)
                    : _chunks,
                writer,
                format);
        }
        #endregion

        #region WriteToAsync Overloads
        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <returns>Task.</returns>
        [NotNull]
        [PublicAPI]
        public Task WriteToAsync([CanBeNull] TextWriter writer)
        {
            return writer == null
                ? TaskResult.Completed
                : WriteToInternalAsync(_chunks, writer, "G");
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        /// <returns>Task.</returns>
        [NotNull]
        [PublicAPI]
        public Task WriteToAsync(
            [CanBeNull] TextWriter writer,
            [CanBeNull] string format,
            [CanBeNull] params object[] values)
        {
            if (writer == null) return TaskResult.Completed;
            if (format == null)
                format = "G";
            return WriteToInternalAsync(
                values != null && values.Length > 0
                    ? Resolve(_chunks, values)
                    : _chunks,
                writer,
                format);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        /// <returns>Task.</returns>
        [NotNull]
        [PublicAPI]
        public Task WriteToAsync(
            [CanBeNull] TextWriter writer,
            [CanBeNull] string format,
            [CanBeNull] [InstantHandle] IEnumerable<object> values)
        {
            if (writer == null) return TaskResult.Completed;
            if (format == null)
                format = "G";
            if (values != null)
            {
                object[] vArray = values.ToArray();
                if (vArray.Length > 0)
                {
                    return WriteToInternalAsync(
                        Resolve(_chunks, vArray),
                        writer,
                        format);
                }
            }

            return WriteToInternalAsync(
                _chunks,
                writer,
                format);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        /// <returns>Task.</returns>
        [NotNull]
        [PublicAPI]
        public Task WriteToAsync(
            [CanBeNull] TextWriter writer,
            [CanBeNull] string format,
            [CanBeNull] IReadOnlyDictionary<string, object> values)
        {
            if (writer == null) return TaskResult.Completed;
            if (format == null)
                format = "G";
            return WriteToInternalAsync(
                values != null && values.Count > 0
                    ? Resolve(_chunks, values)
                    : _chunks,
                writer,
                format);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns>Task.</returns>
        [NotNull]
        [PublicAPI]
        public Task WriteToAsync(
            [CanBeNull] TextWriter writer,
            [CanBeNull] string format,
            [CanBeNull]  [InstantHandle] Func<FormatChunk, Optional<object>> resolver)
        {
            if (writer == null) return TaskResult.Completed;
            if (format == null)
                format = "G";
            return WriteToInternalAsync(
                resolver != null
                    ? Resolve(_chunks, resolver)
                    : _chunks,
                writer,
                format);
        }
        #endregion

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="chunks">The chunks.</param>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        [PublicAPI]
        protected virtual void WriteToInternal([NotNull, InstantHandle] IEnumerable<FormatChunk> chunks, [NotNull] TextWriter writer, [NotNull] string format)
        {
            Contract.Requires(chunks != null);
            Contract.Requires(writer != null);
            Contract.Requires(format != null);
            bool writeTags = string.Equals(format, "f", StringComparison.InvariantCultureIgnoreCase);
            IFormatProvider formatProvider = writer.FormatProvider;
            IControllableTextWriter controller = writer as IControllableTextWriter;

            // We try to output the builder in one go to prevent interleaving, however we split on control codes.
            StringBuilder sb = new StringBuilder();
            foreach (FormatChunk chunk in chunks)
            {
                Contract.Assert(chunk != null);

                if (chunk.IsControl &&
                    !writeTags)
                {
                    if (controller == null) continue;

                    // If we have anything to write out, do so before calling the controller.
                    if (sb.Length > 0)
                    {
                        writer.Write(sb.ToString());
                        sb.Clear();
                    }

                    controller.OnControlChunk(chunk, format, formatProvider);
                }
                else
                    sb.Append(chunk.ToString(format, formatProvider));
            }

            if (sb.Length > 0)
                writer.Write(sb.ToString());
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="chunks">The chunks.</param>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        [NotNull]
        [PublicAPI]
        protected virtual async Task WriteToInternalAsync([NotNull, InstantHandle] IEnumerable<FormatChunk> chunks, [NotNull] TextWriter writer, [NotNull] string format)
        {
            Contract.Requires(chunks != null);
            Contract.Requires(writer != null);
            Contract.Requires(format != null);
            bool writeTags = string.Equals(format, "f", StringComparison.InvariantCultureIgnoreCase);
            IFormatProvider formatProvider = writer.FormatProvider;
            IControllableTextWriter controller = writer as IControllableTextWriter;

            // We try to output the builder in one go to prevent interleaving, however we split on control codes.
            StringBuilder sb = new StringBuilder();
            foreach (FormatChunk chunk in chunks)
            {
                Contract.Assert(chunk != null);

                if (chunk.IsControl &&
                    !writeTags)
                {
                    if (controller == null) continue;

                    // If we have anything to write out, do so before calling the controller.
                    if (sb.Length > 0)
                    {
                        // ReSharper disable once PossibleNullReferenceException
                        await writer.WriteAsync(sb.ToString());
                        sb.Clear();
                    }

                    controller.OnControlChunk(chunk, format, formatProvider);
                }
                else
                    sb.Append(chunk.ToString(format, formatProvider));
            }

            if (sb.Length > 0)
                // ReSharper disable once PossibleNullReferenceException
                await writer.WriteAsync(sb.ToString());
        }
    }
}