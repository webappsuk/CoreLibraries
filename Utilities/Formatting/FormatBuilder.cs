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
        public FormatBuilder([CanBeNull] IEnumerable<object> values)
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
            _values = values == null || _values.Count < 1 ? _empty : values;
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
        public bool IsEmpty
        {
            get { return _chunks.Count < 1; }
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
        public FormatBuilder Append([CanBeNull]object value)
        {
            if (value != null)
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
        public FormatBuilder Append([CanBeNull]char[] value)
        {
            if ((value != null) &&
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
        public FormatBuilder Append([CanBeNull]char[] value, int startIndex, int charCount)
        {
            if ((value != null) &&
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
            if (repeatCount > 0)
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
        public FormatBuilder Append([CanBeNull]string value)
        {
            if (!string.IsNullOrEmpty(value))
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
        public FormatBuilder Append([CanBeNull]IEnumerable<FormatChunk> chunks)
        {
            if (chunks != null)
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
        public FormatBuilder Append([CanBeNull]FormatBuilder builder)
        {
            if (builder != null && !builder.IsEmpty)
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
        public FormatBuilder AppendLine([CanBeNull]object value)
        {
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
        public FormatBuilder AppendLine([CanBeNull]char[] value)
        {
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
        public FormatBuilder AppendLine([CanBeNull]char[] value, int startIndex, int charCount)
        {
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
        public FormatBuilder AppendLine([CanBeNull]string value)
        {
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
        public FormatBuilder AppendLine([CanBeNull]IEnumerable<FormatChunk> chunks)
        {
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
        public FormatBuilder AppendLine([CanBeNull]FormatBuilder builder)
        {
            if (builder != null && !builder.IsEmpty)
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
        public FormatBuilder AppendFormat([CanBeNull] string format, [CanBeNull] params object[] args)
        {
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
        public FormatBuilder AppendFormat([CanBeNull] string format, [CanBeNull] IReadOnlyDictionary<string, object> values)
        {
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
        public FormatBuilder AppendFormatLine([CanBeNull] string format, [CanBeNull] params object[] args)
        {
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
                    _chunks.Add(FormatChunk.Create(value));
                else
                    _chunks.Add(chunk);
            }
        }

        /// <summary>
        /// Appends the control object for controlling formatting.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendControl([NotNull] object control)
        {
            Contract.Requires(control != null);
            _chunks.Add(FormatChunk.CreateControl(control));
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
            [CanBeNull] Func<string, object> resolver)
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
            [CanBeNull] IEnumerable<object> values)
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
        public FormatBuilder Resolve(
            [CanBeNull] IReadOnlyDictionary<string, object> values)
        {
            if ((values == null) ||
                (values.Count < 1)) return this;

            for (int a = 0; a < _chunks.Count; a++)
            {
                FormatChunk chunk = _chunks[a];
                Contract.Assert(chunk != null);
                if (!chunk.IsFillPoint) continue;

                Contract.Assert(chunk.Tag != null);
                object resolved;
                if (values.TryGetValue(chunk.Tag, out resolved))
                    _chunks[a] = FormatChunk.Create(resolved);
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
        /// To the string.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>System.String.</returns>
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
        public void WriteTo([CanBeNull]TextWriter writer, [CanBeNull] IFormatProvider formatProvider = null)
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
        public virtual void WriteTo([CanBeNull]TextWriter writer, [CanBeNull] string format = null, [CanBeNull] IFormatProvider formatProvider = null)
        {
            if (writer == null) return;

            // We try to output the builder in one go to prevent interleaving, however we split on control codes.
            StringBuilder sb = new StringBuilder();
            foreach (FormatChunk chunk in _chunks)
            {
                Contract.Assert(chunk != null);
                if (chunk.IsControl)
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
        /// Writes the builder to the specified <see cref="TextWriter" /> asynchronously.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format passed to each chunk.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>An awaitable task.</returns>
        [NotNull]
        [PublicAPI]
        public virtual async Task WriteToAsync([CanBeNull]TextWriter writer, [CanBeNull] string format = null, [CanBeNull] IFormatProvider formatProvider = null)
        {
            if (writer == null) return;

            // We try to output the builder in one go to prevent interleaving, however we split on control codes.
            StringBuilder sb = new StringBuilder();
            foreach (FormatChunk chunk in _chunks)
            {
                Contract.Assert(chunk != null);
                if (chunk.IsControl)
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