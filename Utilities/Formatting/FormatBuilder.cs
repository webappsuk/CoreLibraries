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
using System.Globalization;
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
        /// The empty lookup dictionary.
        /// </summary>
        [NotNull]
        private static readonly IReadOnlyDictionary<string, object> _empty = new Dictionary<string, object>(0);

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
        private readonly IReadOnlyDictionary<string, object> _values;

        [NotNull]
        private readonly List<FormatChunk> _chunks = new List<FormatChunk>();

        /// <summary>
        /// Whether this builder is readonly
        /// </summary>
        private bool _isReadonly;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FormatBuilder" /> class.
        /// </summary>
        /// <param name="values">The values.</param>
        public FormatBuilder([CanBeNull] params object[] values)
        {
            _values = ToDictionary(values);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatBuilder" /> class.
        /// </summary>
        /// <param name="values">The values.</param>
        public FormatBuilder([CanBeNull] [InstantHandle] IEnumerable<object> values)
        {
            _values = values == null ? _empty : ToDictionary(values.ToArray());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatBuilder" /> class.
        /// </summary>
        /// <param name="values">The values.</param>
        public FormatBuilder(
            [CanBeNull] IReadOnlyDictionary<string, object> values)
        {
            _values = values == null || values.Count < 1 ? _empty : values;
        }
        #endregion

        /// <summary>
        /// Gets the values that are automatically substituted.
        /// </summary>
        /// <value>The values.</value>
        [NotNull]
        [PublicAPI]
        public IReadOnlyDictionary<string, object> Values
        {
            get { return _values; }
        }

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
        /// <returns>A shallow copy of this builder.</returns>
        [NotNull]
        [PublicAPI]
        public virtual FormatBuilder Clone()
        {
            Contract.Ensures(Contract.Result<FormatBuilder>().GetType() == this.GetType(),
                "All classes derived from FormatBuilder should overload this method and return a builder of their own type");

            FormatBuilder formatBuilder = new FormatBuilder(_values);
            formatBuilder._chunks.AddRange(_chunks.Select(c => c.Clone()));
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
        /// <param name="chunks">The chunks.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Append([CanBeNull] [InstantHandle] IEnumerable<FormatChunk> chunks)
        {
            Contract.Requires(!IsReadonly);
            if (!_isReadonly &&
                chunks != null)
                _chunks.AddRange(chunks);
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
        /// <param name="chunks">The chunks.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendLine([CanBeNull] [InstantHandle] IEnumerable<FormatChunk> chunks)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            if (chunks != null)
                _chunks.AddRange(chunks);
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
                AppendFormatInternal(format, ToDictionary(args));
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
                AppendFormatInternal(format, values ?? _empty);
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
        /// Appends the specified format string.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendFormatLine([CanBeNull] string format)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            if (!string.IsNullOrEmpty(format))
                AppendFormatInternal(format, _empty);
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
                AppendFormatInternal(format, ToDictionary(args));
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
                AppendFormatInternal(format, values ?? _empty);
            _chunks.Add(FormatChunk.Create(Environment.NewLine));
            return this;
        }
        #endregion

        /// <summary>
        /// Internal append implementation.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        /// <returns>FormatBuilder.</returns>
        private void AppendFormatInternal([NotNull] string format, [NotNull] IReadOnlyDictionary<string, object> values)
        {
            Contract.Requires(format != null);
            Contract.Requires(values != null);
            Contract.Requires(!IsReadonly);
            values = values.Count < 1
                ? _values
                : values.Union(_values)
                    .Distinct(KeyComparer<string, object>.Default)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            bool hasValues = values.Count > 0;

            foreach (FormatChunk chunk in format.FormatChunks())
            {
                Contract.Assert(chunk != null);
                object value;
                if (hasValues &&
                    chunk.IsFillPoint &&
                    values.TryGetValue(chunk.Tag, out value))
                    chunk.Value = value;

                _chunks.Add(chunk);
            }
        }

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
            _chunks.Add(FormatChunk.CreateControl(tag, alignment, format, value));
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
            _chunks.Add(control);
            return this;
        }

        #region Resolve overloads
        /// <summary>
        /// Allows you to resolve any unresolved fill points.
        /// </summary>
        /// <param name="resolver">The resolver should return a non-null object to resolve the fill-point; otherwise <see langword="null" />.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Resolve(
            [CanBeNull] [InstantHandle] Func<string, object> resolver)
        {
            if (resolver == null) return this;

            for (int a = 0; a < _chunks.Count; a++)
            {
                FormatChunk chunk = _chunks[a];
                Contract.Assert(chunk != null);
                if (!chunk.IsFillPoint) continue;

                Contract.Assert(chunk.Tag != null);
                object resolved = resolver(chunk.Tag);
                if (resolved == null) continue;

                _chunks[a] = FormatChunk.Create(resolved);
            }
            return this;
        }

        /// <summary>
        /// Allows you to resolve any unresolved fill points.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Resolve(
            [CanBeNull] [InstantHandle] IEnumerable<object> values)
        {
            if (values == null) return this;
            return Resolve(ToDictionary(values.ToArray()));
        }

        /// <summary>
        /// Allows you to resolve any unresolved fill points.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Resolve([CanBeNull] params object[] values)
        {
            if ((values == null) ||
                (values.Length < 1)) return this;
            return Resolve(ToDictionary(values));
        }

        /// <summary>
        /// Allows you to resolve any unresolved fill points.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Resolve([CanBeNull] IReadOnlyDictionary<string, object> values)
        {
            if ((values == null) ||
                (values.Count < 1)) return this;

            foreach (FormatChunk chunk in _chunks)
            {
                Contract.Assert(chunk != null);
                if (!chunk.IsFillPoint) continue;

                Contract.Assert(chunk.Tag != null);
                object resolved;
                if (values.TryGetValue(chunk.Tag, out resolved))
                    chunk.Value = resolved;
            }
            return this;
        }
        #endregion

        /// <summary>
        /// Converts the objects to a numbered dictionary lookup.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>A <see cref="IReadOnlyDictionary{TKey,TValue}" />.</returns>
        [NotNull]
        private static IReadOnlyDictionary<string, object> ToDictionary(
            [CanBeNull] object[] values)
        {
            return values == null || values.Length < 1
                ? _empty
                : values
                    .Select((v, i) => new KeyValuePair<string, object>(i.ToString(CultureInfo.InvariantCulture), v))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
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
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        [NotNull]
        [PublicAPI]
        public string ToString([CanBeNull] IFormatProvider formatProvider)
        {
            return ToString(null, formatProvider);
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
        public virtual string ToString([CanBeNull] string format, [CanBeNull] IFormatProvider formatProvider)
        {
            using (StringWriter writer = new StringWriter())
            {
                WriteTo(writer, format, formatProvider);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="formatProvider">The format provider.</param>
        [PublicAPI]
        public void WriteTo([CanBeNull] TextWriter writer, [CanBeNull] IFormatProvider formatProvider = null)
        {
            WriteTo(writer, null, formatProvider);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>An awaitable task.</returns>
        [NotNull]
        [PublicAPI]
        public Task WriteToAsync(
            [CanBeNull] TextWriter writer,
            [CanBeNull] IFormatProvider formatProvider = null)
        {
            return WriteToAsync(writer, null, formatProvider);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format passed to each chunk.</param>
        /// <param name="formatProvider">The format provider.</param>
        [PublicAPI]
        public virtual void WriteTo(
            [CanBeNull] TextWriter writer,
            [CanBeNull] string format = null,
            [CanBeNull] IFormatProvider formatProvider = null)
        {
            if (writer == null) return;

            bool writeTags = format != null && string.Equals(format, "f", StringComparison.InvariantCultureIgnoreCase);

            // We try to output the builder in one go to prevent interleaving, however we split on control codes.
            StringBuilder sb = new StringBuilder();
            foreach (FormatChunk chunk in _chunks)
            {
                Contract.Assert(chunk != null);
                if (chunk.IsControl &&
                    !writeTags)
                {
                    if (sb.Length > 0)
                    {
                        writer.Write(sb.ToString());
                        sb.Clear();
                    }
                    OnControlChunk(chunk, writer, format, formatProvider);
                }
                else
                    sb.Append(chunk.ToString(format, formatProvider));
            }

            if (sb.Length > 0)
                writer.Write(sb.ToString());
        }

        /// <summary>
        /// Writes the builder to the console.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        [PublicAPI]
        public void WriteToConsole(
            [CanBeNull] string format = null,
            [CanBeNull] IFormatProvider formatProvider = null)
        {
            if (!ConsoleHelper.IsConsole) return;
            WriteTo(Console.Out, format, formatProvider);
        }

        /// <summary>
        /// Writes the builder to <see cref="Trace"/>.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        [PublicAPI]
        public void WriteToTrace(
            [CanBeNull] string format = null,
            [CanBeNull] IFormatProvider formatProvider = null)
        {
            WriteTo(TraceTextWriter.Default, format, formatProvider);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" /> asynchronously.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format passed to each chunk.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>An awaitable task.</returns>
        [NotNull]
        [PublicAPI]
        public virtual async Task WriteToAsync(
            [CanBeNull] TextWriter writer,
            [CanBeNull] string format = null,
            [CanBeNull] IFormatProvider formatProvider = null)
        {
            if (writer == null) return;

            bool writeTags = format != null && string.Equals(format, "f", StringComparison.InvariantCultureIgnoreCase);

            // We try to output the builder in one go to prevent interleaving, however we split on control codes.
            StringBuilder sb = new StringBuilder();
            foreach (FormatChunk chunk in _chunks)
            {
                Contract.Assert(chunk != null);
                // If the format is F/f, then control tags will need to be output
                if (chunk.IsControl &&
                    !writeTags)
                {
                    if (sb.Length > 0)
                    {
                        // ReSharper disable once PossibleNullReferenceException
                        await writer.WriteAsync(sb.ToString());
                        sb.Clear();
                    }
                    OnControlChunk(chunk, writer, format, formatProvider);
                }
                else
                    sb.Append(chunk.ToString(format, formatProvider));
            }

            if (sb.Length > 0)
                // ReSharper disable once PossibleNullReferenceException
                await writer.WriteAsync(sb.ToString());
        }

        /// <summary>
        /// Called when a control chunk is encountered.
        /// </summary>
        /// <param name="controlChunk">The control chunk.</param>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        [PublicAPI]
        protected virtual void OnControlChunk(
            [NotNull] FormatChunk controlChunk,
            [NotNull] TextWriter writer,
            [CanBeNull] string format,
            [CanBeNull] IFormatProvider formatProvider)
        {
            Contract.Requires(controlChunk != null);
            Contract.Requires(writer != null);
        }
    }
}