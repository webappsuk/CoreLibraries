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
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// Build a formatted string, which can be used to enumerate FormatChunks
    /// </summary>
    [TypeConverter(typeof(FormatBuilderConverter))]
    public sealed partial class FormatBuilder : IEnumerable<FormatChunk>, IFormattable, IWriteable, IEquatable<FormatBuilder>
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
        private List<FormatChunk> _chunks = new List<FormatChunk>();

        /// <summary>
        /// Whether this builder is read only
        /// </summary>
        private bool _isReadOnly;

        /// <summary>
        /// Whether layout is required.
        /// </summary>
        private bool _isLayoutRequired;

        /// <summary>
        /// Gets the initial layout to use when resetting the layout.
        /// </summary>
        /// <value>
        /// The initial layout.
        /// </value>
        [NotNull]
        [PublicAPI]
        public Layout InitialLayout { get; protected set; }

        #region Constructor overloads
        /// <summary>
        /// Initializes a new instance of the <see cref="FormatBuilder" /> class.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="isReadOnly">if set to <see langword="true" /> is read only.</param>
        [StringFormatMethod("format")]
        public FormatBuilder([CanBeNull] string format = null, bool isReadOnly = false)
        {
            InitialLayout = Layout.Default;
            if (!string.IsNullOrEmpty(format))
                AppendFormat(format);
            _isReadOnly = isReadOnly;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatBuilder" /> class.
        /// </summary>
        /// <param name="layout">The layout.</param>
        /// <param name="isReadOnly">if set to <see langword="true" /> is read only.</param>
        public FormatBuilder([CanBeNull] Layout layout, bool isReadOnly = false)
        {
            InitialLayout = Layout.Default.Apply(layout);
            _isLayoutRequired = InitialLayout != Layout.Default;
            Contract.Assert(InitialLayout.IsFull);
            _isReadOnly = isReadOnly;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatBuilder" /> class.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="layout">The layout.</param>
        /// <param name="isReadOnly">if set to <see langword="true" /> is read only.</param>
        [StringFormatMethod("format")]
        public FormatBuilder([CanBeNull] string format, [CanBeNull] Layout layout, bool isReadOnly = false)
        {
            InitialLayout = Layout.Default.Apply(layout);
            _isLayoutRequired = InitialLayout != Layout.Default;
            Contract.Assert(InitialLayout.IsFull);
            if (!string.IsNullOrEmpty(format))
                AppendFormat(format);
            _isReadOnly = isReadOnly;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatBuilder"/> class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="indentSize">Size of the indent.</param>
        /// <param name="rightMarginSize">Size of the right margin.</param>
        /// <param name="indentChar">The indent character.</param>
        /// <param name="firstLineIndentSize">First size of the line indent.</param>
        /// <param name="tabStops">The tab stops.</param>
        /// <param name="tabSize">Size of the tab.</param>
        /// <param name="tabChar">The tab character.</param>
        /// <param name="alignment">The alignment.</param>
        /// <param name="splitLength">The split length.</param>
        /// <param name="hyphenate">The hyphenate.</param>
        /// <param name="hyphenChar">The hyphen character.</param>
        /// <param name="wrapMode">The wrap mode.</param>
        /// <param name="isReadOnly">if set to <see langword="true" /> is read only.</param>
        public FormatBuilder(
            Optional<int> width,
            Optional<int> indentSize = default(Optional<int>),
            Optional<int> rightMarginSize = default(Optional<int>),
            Optional<char> indentChar = default(Optional<char>),
            Optional<int> firstLineIndentSize = default(Optional<int>),
            Optional<IEnumerable<int>> tabStops = default(Optional<IEnumerable<int>>),
            Optional<byte> tabSize = default(Optional<byte>),
            Optional<char> tabChar = default(Optional<char>),
            Optional<Alignment> alignment = default(Optional<Alignment>),
            Optional<byte> splitLength = default(Optional<byte>),
            Optional<bool> hyphenate = default(Optional<bool>),
            Optional<char> hyphenChar = default(Optional<char>),
            Optional<LayoutWrapMode> wrapMode = default(Optional<LayoutWrapMode>),
            bool isReadOnly = false)
        {
            InitialLayout = Layout.Default.Apply(
                width,
                indentSize,
                rightMarginSize,
                indentChar,
                firstLineIndentSize,
                tabStops,
                tabSize,
                tabChar,
                alignment,
                splitLength,
                hyphenate,
                hyphenChar,
                wrapMode);
            _isLayoutRequired = InitialLayout != Layout.Default;
            Contract.Assert(InitialLayout.IsFull);
            _isReadOnly = isReadOnly;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatBuilder" /> class.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="width">The width.</param>
        /// <param name="indentSize">Size of the indent.</param>
        /// <param name="rightMarginSize">Size of the right margin.</param>
        /// <param name="indentChar">The indent character.</param>
        /// <param name="firstLineIndentSize">First size of the line indent.</param>
        /// <param name="tabStops">The tab stops.</param>
        /// <param name="tabSize">Size of the tab.</param>
        /// <param name="tabChar">The tab character.</param>
        /// <param name="alignment">The alignment.</param>
        /// <param name="splitLength">The split length.</param>
        /// <param name="hyphenate">The hyphenate.</param>
        /// <param name="hyphenChar">The hyphen character.</param>
        /// <param name="wrapMode">The wrap mode.</param>
        /// <param name="isReadOnly">if set to <see langword="true" /> is read only.</param>
        [StringFormatMethod("format")]
        public FormatBuilder(
            [CanBeNull] string format,
            Optional<int> width,
            Optional<int> indentSize = default(Optional<int>),
            Optional<int> rightMarginSize = default(Optional<int>),
            Optional<char> indentChar = default(Optional<char>),
            Optional<int> firstLineIndentSize = default(Optional<int>),
            Optional<IEnumerable<int>> tabStops = default(Optional<IEnumerable<int>>),
            Optional<byte> tabSize = default(Optional<byte>),
            Optional<char> tabChar = default(Optional<char>),
            Optional<Alignment> alignment = default(Optional<Alignment>),
            Optional<byte> splitLength = default(Optional<byte>),
            Optional<bool> hyphenate = default(Optional<bool>),
            Optional<char> hyphenChar = default(Optional<char>),
            Optional<LayoutWrapMode> wrapMode = default(Optional<LayoutWrapMode>),
            bool isReadOnly = false)
        {
            InitialLayout = Layout.Default.Apply(
                width,
                indentSize,
                rightMarginSize,
                indentChar,
                firstLineIndentSize,
                tabStops,
                tabSize,
                tabChar,
                alignment,
                splitLength,
                hyphenate,
                hyphenChar,
                wrapMode);
            _isLayoutRequired = InitialLayout != Layout.Default;
            Contract.Assert(InitialLayout.IsFull);
            if (!string.IsNullOrEmpty(format))
                AppendFormat(format);
            _isReadOnly = isReadOnly;
        }
        #endregion

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
        [NotNull]
        public FormatBuilder Clear()
        {
            _chunks.Clear();
            _isLayoutRequired = InitialLayout != Layout.Default;
            return this;
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
        /// Gets a value indicating whether this builder is read only.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this builder is read only; otherwise, <see langword="false" />.
        /// </value>
        /// <remarks>A read only builder cannot have any more chunks appended, but fill points can still be resolved.</remarks>
        [PublicAPI]
        public bool IsReadOnly
        {
            get { return _isReadOnly; }
        }

        /// <summary>
        /// Makes this builder read only.
        /// </summary>
        [PublicAPI]
        [NotNull]
        public FormatBuilder MakeReadOnly()
        {
            _isReadOnly = true;
            return this;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <param name="readOnly">If set to <see langword="true" />, the returned builder will be read only.</param>
        /// <returns>
        /// A shallow copy of this builder.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Clone(bool readOnly = false)
        {
            Contract.Ensures(
                !readOnly || Contract.Result<FormatBuilder>().IsReadOnly,
                "Returned builder should be read only if readOnly is true");

            if (_isReadOnly && readOnly)
                return this;

            FormatBuilder formatBuilder = new FormatBuilder(InitialLayout) { _isLayoutRequired = _isLayoutRequired };
            formatBuilder._chunks.AddRange(_chunks);
            if (readOnly)
                formatBuilder.MakeReadOnly();
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
            if (value != null)
                _chunks.AddRange(
                    Resolve(
                        new[] { FormatChunk.Create(value) },
                        null,
                        false,
                        ref _isLayoutRequired));
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
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
        public FormatBuilder Append([CanBeNull] char[] value, int startIndex, int charCount)
        {
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
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
        public FormatBuilder Append([CanBeNull] string value)
        {
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
            if (!string.IsNullOrEmpty(value))
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
            if (chunk != null)
                _chunks.AddRange(
                    Resolve(
                        new[] { chunk },
                        null,
                        false,
                        ref _isLayoutRequired));
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
            if (chunks != null)
                _chunks.AddRange(Resolve(chunks, null, false, ref _isLayoutRequired));
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
            if (builder != null &&
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
            if (value != null)
                _chunks.AddRange(
                    Resolve(
                        new[] { FormatChunk.Create(value) },
                        null,
                        false,
                        ref _isLayoutRequired));
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
            if (chunk != null)
                _chunks.AddRange(
                    Resolve(
                        new[] { chunk },
                        null,
                        false,
                        ref _isLayoutRequired));
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
            if (chunks != null)
                _chunks.AddRange(Resolve(chunks, null, false, ref _isLayoutRequired));
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
            if (!string.IsNullOrEmpty(format))
                _chunks.AddRange(Resolve(format.FormatChunks(), args, ref _isLayoutRequired));
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
        [StringFormatMethod("format")]
        public FormatBuilder AppendFormat(
            [CanBeNull] string format,
            [CanBeNull] IReadOnlyDictionary<string, object> values)
        {
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
            if (!string.IsNullOrEmpty(format))
                _chunks.AddRange(Resolve(format.FormatChunks(), values, ref _isLayoutRequired));
            return this;
        }

        /// <summary>
        /// Appends the specified format string, replacing any integer tags with the matching arguments (overriding pre-specified replacements).
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="resolver">The resolver.</param>
        /// <param name="isCaseSensitive">if set to <see langword="true" /> then tags are case sensitive.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        [StringFormatMethod("format")]
        public FormatBuilder AppendFormat(
            [CanBeNull] string format,
            [CanBeNull] [InstantHandle] Func<string, Optional<object>> resolver,
            bool isCaseSensitive = false)
        {
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
            if (!string.IsNullOrEmpty(format))
                _chunks.AddRange(Resolve(format.FormatChunks(), resolver, isCaseSensitive, ref _isLayoutRequired));
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
            if (!string.IsNullOrEmpty(format))
                _chunks.AddRange(Resolve(format.FormatChunks(), args, ref _isLayoutRequired));
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
        [StringFormatMethod("format")]
        public FormatBuilder AppendFormatLine(
            [CanBeNull] string format,
            [CanBeNull] IReadOnlyDictionary<string, object> values)
        {
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
            if (!string.IsNullOrEmpty(format))
                _chunks.AddRange(Resolve(format.FormatChunks(), values, ref _isLayoutRequired));
            _chunks.Add(FormatChunk.Create(Environment.NewLine));
            return this;
        }

        /// <summary>
        /// Appends the specified format string, replacing any integer tags with the matching arguments (overriding pre-specified replacements).
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="resolver">The resolver.</param>
        /// <param name="isCaseSensitive">if set to <see langword="true" /> then tags are case sensitive.</param>
        /// <returns>
        /// This instance.
        /// </returns>
        [NotNull]
        [PublicAPI]
        [StringFormatMethod("format")]
        public FormatBuilder AppendFormatLine(
            [CanBeNull] string format,
            [CanBeNull] [InstantHandle] Func<string, Optional<object>> resolver,
            bool isCaseSensitive = false)
        {
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
            if (!string.IsNullOrEmpty(format))
                _chunks.AddRange(Resolve(format.FormatChunks(), resolver, isCaseSensitive, ref _isLayoutRequired));
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
        [StringFormatMethod("format")]
        public FormatBuilder AppendControl(
            [CanBeNull] string tag,
            int alignment = 0,
            [CanBeNull] string format = null, Optional<object> value = default(Optional<object>))
        {
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);

            FormatChunk chunk = FormatChunk.CreateControl(tag, alignment, format, value);
            _chunks.AddRange(
                Resolve(
                    new[] { chunk },
                    null,
                    false,
                    ref _isLayoutRequired));
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
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
            _chunks.AddRange(
                Resolve(
                    new[] { control },
                    null,
                    false,
                    ref _isLayoutRequired));
            return this;
        }
        #endregion

        #region Resolve Overloads
        /// <summary>
        /// Resolves any tags.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Resolve([CanBeNull] params object[] values)
        {
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
            if ((_chunks.Count < 1) ||
                (values == null) ||
                (values.Length < 1))
                return this;

            _chunks = Resolve(
                _chunks,
                values,
                ref _isLayoutRequired);
            return this;
        }

        /// <summary>
        /// Resolves any tags.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Resolve([CanBeNull] IReadOnlyDictionary<string, object> values)
        {
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
            if ((_chunks.Count < 1) ||
                (values == null) ||
                (values.Count < 1))
                return this;

            _chunks = Resolve(
                _chunks,
                values,
                ref _isLayoutRequired);
            return this;
        }

        /// <summary>
        /// Resolves any tags.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        /// <param name="isCaseSensitive">if set to <see langword="true" /> then tags are case sensitive.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Resolve([CanBeNull] Func<string, Optional<object>> resolver, bool isCaseSensitive = false)
        {
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
            if ((_chunks.Count < 1) ||
                (resolver == null))
                return this;

            _chunks = Resolve(
                _chunks,
                resolver,
                isCaseSensitive,
                ref _isLayoutRequired);
            return this;
        }

        /// <summary>
        /// Resolves the specified chunks.
        /// </summary>
        /// <param name="chunks">The chunks.</param>
        /// <param name="values">The values.</param>
        /// <param name="isLayoutRequired">if set to <see langword="true" /> then layout is required.</param>
        /// <returns></returns>
        [NotNull]
        private static List<FormatChunk> Resolve(
            [NotNull] IEnumerable<FormatChunk> chunks,
            [CanBeNull] object[] values,
            ref bool isLayoutRequired)
        {
            Contract.Requires(chunks != null);
            return Resolve(
                chunks,
                values != null && values.Length >= 1
                    ? tag =>
                    {
                        int index;
                        // ReSharper disable once PossibleNullReferenceException
                        return int.TryParse(tag, out index) &&
                               (index >= 0) &&
                               (index < values.Length)
                            ? new Optional<object>(values[index])
                            : Optional<object>.Unassigned;
                    }
                    : (Func<string, Optional<object>>)null,
                false,
                ref isLayoutRequired);
        }

        /// <summary>
        /// Resolves the specified chunks.
        /// </summary>
        /// <param name="chunks">The chunks.</param>
        /// <param name="values">The values.</param>
        /// <param name="isLayoutRequired">if set to <see langword="true" /> then layout is required.</param>
        /// <returns></returns>
        [NotNull]
        private static List<FormatChunk> Resolve(
            [NotNull] IEnumerable<FormatChunk> chunks,
            [CanBeNull] IReadOnlyDictionary<string, object> values,
            ref bool isLayoutRequired)
        {
            Contract.Requires(chunks != null);
            return Resolve(
                chunks,
                values != null && values.Count >= 1
                    ? tag =>
                    {
                        object value;
                        // ReSharper disable once PossibleNullReferenceException
                        return values.TryGetValue(tag, out value)
                            ? new Optional<object>(value)
                            : Optional<object>.Unassigned;
                    }
                    : (Func<string, Optional<object>>)null,
                false,
                ref isLayoutRequired);
        }

        /// <summary>
        /// The items tag.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public const string ItemsTag = "<items>";

        /// <summary>
        /// The item tag.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public const string ItemTag = "<item>";

        /// <summary>
        /// The index tag.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public const string IndexTag = "<index>";

        /// <summary>
        /// The join tag.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public const string JoinTag = "<join>";

        /// <summary>
        /// Resolves the specified chunks.
        /// </summary>
        /// <param name="chunks">The chunks.</param>
        /// <param name="resolver">The resolver.</param>
        /// <param name="isCaseSensitive">if set to <see langword="true" /> [is case sensitive].</param>
        /// <param name="isLayoutRequired">if set to <see langword="true" /> then layout is required.</param>
        /// <returns>List&lt;FormatChunk&gt;.</returns>
        [NotNull]
        private static List<FormatChunk> Resolve([NotNull] IEnumerable<FormatChunk> chunks, [CanBeNull] Func<string, Optional<object>> resolver, bool isCaseSensitive, ref bool isLayoutRequired)
        {
            Contract.Requires(chunks != null);
            Stack<FormatChunk, Resolutions> stack = new Stack<FormatChunk, Resolutions>();
            IEnumerator<FormatChunk> ce = chunks.GetEnumerator();
            List<FormatChunk> results = new List<FormatChunk>();
            // Holds any resolutions as we go.
            Resolutions initialResolutions = resolver != null
                ? new Resolutions(null, resolver, isCaseSensitive, false)
                : null;
            do
            {
                FormatChunk chunk;
                Resolutions resolutions;
                if (stack.Count > 0)
                    stack.Pop(out chunk, out resolutions);
                else if (!ce.MoveNext())
                    return results;
                else
                {
                    chunk = ce.Current;
                    resolutions = initialResolutions;
                }

                if (chunk == null) continue;

                if (chunk.Tag == null)
                {
                    results.Add(chunk);
                    continue;
                }

                if (!isLayoutRequired &&
                    string.Equals(chunk.Tag, LayoutTag, StringComparison.InvariantCultureIgnoreCase))
                    isLayoutRequired = true;

                // Resolve the tag if it's the first time we've seen it.
                Optional<object> resolved = resolutions != null
                    ? resolutions.Resolve(chunk.Tag)
                    : Optional<object>.Unassigned;

                // If we haven't resolved the value, get the chunks value.
                bool isAssigned = resolved.IsAssigned;
                if (!isAssigned)
                {
                    if (chunk.IsResolved)
                    {
                        // Use the current resolution.
                        resolved = new Optional<object>(chunk.Value);
                        isAssigned = true;
                    }
                    else
                    {
                        // We have no resolution so just add the chunk.
                        results.Add(chunk);
                        continue;
                    }
                }

                object resolvedValue = resolved.Value;
                // Check for resolved to null.
                if (resolvedValue == null)
                {
                    results.Add(
                        chunk.IsResolved && chunk.Value == null
                            ? chunk
                            : FormatChunk.Create(chunk, resolved));
                    continue;
                }

                // Handles getting a builder, or enumeration of chunks back.
                IEnumerable<FormatChunk> formatChunks = resolvedValue as IEnumerable<FormatChunk>;
                if (formatChunks != null)
                {
                    foreach (FormatChunk fci in formatChunks.Reverse())
                        stack.Push(fci, resolutions);
                    continue;
                }

                // Check if we have an actual FormatChunk as the value.
                FormatChunk fc = resolvedValue as FormatChunk;
                if (fc != null)
                {
                    stack.Push(fc, resolutions);
                    continue;
                }

                // Check if we have a format
                if (!string.IsNullOrEmpty(chunk.Format))
                {
                    // Get the chunks for the fill point.
                    Stack<FormatChunk> subFormatChunks = new Stack<FormatChunk>();
                    bool hasFillPoint = false;
                    bool hasItemsFillPoint = false;
                    FormatChunk joinChunk = null;
                    foreach (FormatChunk subFormatChunk in chunk.Format.FormatChunks())
                    {
                        // ReSharper disable once PossibleNullReferenceException
                        if (subFormatChunk.Tag != null)
                        {
                            hasFillPoint = true;
                            if (string.Equals(
                                subFormatChunk.Tag,
                                ItemsTag,
                                StringComparison.CurrentCultureIgnoreCase))
                                hasItemsFillPoint = true;
                            else if (string.Equals(
                                subFormatChunk.Tag,
                                JoinTag,
                                StringComparison.CurrentCultureIgnoreCase))
                            {
                                // Very special case! 
                                // We only allow one join chunk, so we take the last one, and we remove it from the outer format.
                                joinChunk = subFormatChunk;
                                continue;
                            }
                        }
                        subFormatChunks.Push(subFormatChunk);
                    }

                    if (hasItemsFillPoint)
                    {
                        // We have an <item> fill point, so check if we have an enumerable.
                        IEnumerable enumerable = resolvedValue as IEnumerable;
                        if (enumerable != null)
                        {
                            /*
                             * Special case enumerable, if we have an '<items>' tag, then we will treat each item
                             * individually, otherwise we'll treat as one item.
                             */

                            // Set the value of the joinChunk to FormatOutput.Default, which is an object that writes out it's own format!
                            if (joinChunk != null)
                                joinChunk = FormatChunk.Create(joinChunk, FormatOutput.Default);

                            // Ensure we only enumerate once, and get the enumeration, bound with it's index, in reverse order.
                            KeyValuePair<object, int>[] indexedArray = enumerable
                                .Cast<object>()
                                .Select((r, i) => new KeyValuePair<object, int>(r, i))
                                .Reverse()
                                .ToArray();

                            // We have an enumeration format, so we need to add each item back in individually with new contextual information.
                            while (subFormatChunks.Count > 0)
                            {
                                FormatChunk subFormatChunk = subFormatChunks.Pop();
                                if (
                                    !string.Equals(
                                        subFormatChunk.Tag,
                                        ItemsTag,
                                        StringComparison.CurrentCultureIgnoreCase))
                                {
                                    stack.Push(subFormatChunk, resolutions);
                                    continue;
                                }

                                // We have an <items> chunk, which we now expand for each item.
                                foreach (KeyValuePair<object, int> kvp in indexedArray)
                                {
                                    object key = kvp.Key;
                                    object value = kvp.Value;
                                    // This will add a fall-through value for the '<item>' and '<index>' tags - a new child Resolutions will be created based on this one
                                    // when the IResolution object is later resolved below, which means that you can still technically override the value of these tags in
                                    // the underlying resolver.
                                    Resolutions inner = new Resolutions(
                                        resolutions,
                                        tag =>
                                        {
                                            // ReSharper disable once PossibleNullReferenceException
                                            switch (tag.ToLowerInvariant())
                                            {
                                                case IndexTag:
                                                    return value;
                                                case ItemTag:
                                                    return key;
                                                default:
                                                    return Optional<object>.Unassigned;
                                            }
                                        },
                                        false,
                                        true);

                                    // Add a new chunk with, the <Item> tag.
                                    stack.Push(
                                        new FormatChunk(
                                            ItemTag,
                                            subFormatChunk.Alignment,
                                            subFormatChunk.Format,
                                            true,
                                            key,
                                            subFormatChunk.IsControl),
                                        inner);

                                    // If we have join chunk, push it for all but the 'first' element.
                                    if (kvp.Value > 0 &&
                                        joinChunk != null)
                                        stack.Push(joinChunk, inner);
                                }
                            }
                            continue;
                        }
                    }

                    // If we have a value, and a format, then we may need to recurse.
                    if (isAssigned && hasFillPoint)
                    {
                        IResolvable resolvable = resolvedValue as IResolvable;
                        if (resolvable != null)
                            resolutions = new Resolutions(
                                resolutions,
                                resolvable.Resolve,
                                resolvable.IsCaseSensitive,
                                resolvable.ResolveOuterTags);

                        while (subFormatChunks.Count > 0)
                            stack.Push(subFormatChunks.Pop(), resolutions);
                        continue;
                    }
                }

                if (chunk.IsResolved && chunk.Value == resolvedValue)
                {
                    results.Add(chunk);
                    continue;
                }

                results.Add(FormatChunk.Create(chunk, resolved));
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
                WriteTo(writer);
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
        /// <param name="layout">The layout is applied to the original <see cref="InitialLayout"/>.</param>
        /// <param name="position">The position.</param>
        /// <param name="values">The values.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        [NotNull]
        [PublicAPI]
        public string ToString([CanBeNull]Layout layout, ref int position, [CanBeNull] params object[] values)
        {
            using (StringWriter writer = new StringWriter())
            {
                position = WriteTo(writer, layout, position, null, values);
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
        /// <param name="layout">The layout is applied to the original <see cref="InitialLayout"/>.</param>
        /// <param name="position">The position.</param>
        /// <param name="values">The values.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        [NotNull]
        [PublicAPI]
        public string ToString([CanBeNull]Layout layout, ref int position, [CanBeNull] IReadOnlyDictionary<string, object> values)
        {
            using (StringWriter writer = new StringWriter())
            {
                position = WriteTo(writer, layout, position, null, values);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="resolver">The values.</param>
        /// <param name="isCaseSensitive">if set to <see langword="true" /> then tags are case sensitive.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public string ToString([CanBeNull] [InstantHandle] Func<string, Optional<object>> resolver, bool isCaseSensitive = false)
        {
            using (StringWriter writer = new StringWriter())
            {
                WriteTo(writer, null, resolver, isCaseSensitive);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="layout">The layout is applied to the original <see cref="InitialLayout"/>.</param>
        /// <param name="position">The position.</param>
        /// <param name="resolver">The values.</param>
        /// <param name="isCaseSensitive">if set to <see langword="true" /> then tags are case sensitive.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        [NotNull]
        [PublicAPI]
        public string ToString(
            [CanBeNull]Layout layout,
            ref int position,
            [CanBeNull] [InstantHandle] Func<string, Optional<object>> resolver,
            bool isCaseSensitive = false)
        {
            using (StringWriter writer = new StringWriter())
            {
                position = WriteTo(writer, layout, position, null, resolver, isCaseSensitive);
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
        /// <param name="layout">The layout is applied to the original <see cref="InitialLayout"/>.</param>
        /// <param name="position">The position.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="values">The values.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        [NotNull]
        [PublicAPI]
        public string ToString(
            [CanBeNull]Layout layout,
            ref int position,
            [CanBeNull] IFormatProvider formatProvider,
            [CanBeNull] params object[] values)
        {
            using (StringWriter writer = new StringWriter(formatProvider))
            {
                position = WriteTo(writer, layout, position, null, values);
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
        /// <param name="layout">The layout is applied to the original <see cref="InitialLayout"/>.</param>
        /// <param name="position">The position.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="values">The values.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        [NotNull]
        [PublicAPI]
        public string ToString(
            [CanBeNull]Layout layout,
            ref int position,
            [CanBeNull] IFormatProvider formatProvider,
            [CanBeNull] IReadOnlyDictionary<string, object> values)
        {
            using (StringWriter writer = new StringWriter(formatProvider))
            {
                position = WriteTo(writer, layout, position, null, values);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="resolver">The values.</param>
        /// <param name="isCaseSensitive">if set to <see langword="true" /> then tags are case sensitive.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public string ToString(
            [CanBeNull] IFormatProvider formatProvider,
            [CanBeNull] [InstantHandle] Func<string, Optional<object>> resolver,
            bool isCaseSensitive = false)
        {
            using (StringWriter writer = new StringWriter(formatProvider))
            {
                WriteTo(writer, null, resolver, isCaseSensitive);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="layout">The layout is applied to the original <see cref="InitialLayout"/>.</param>
        /// <param name="position">The position.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="resolver">The values.</param>
        /// <param name="isCaseSensitive">if set to <see langword="true" /> then tags are case sensitive.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        [NotNull]
        [PublicAPI]
        public string ToString(
            [CanBeNull]Layout layout,
            ref int position,
            [CanBeNull] IFormatProvider formatProvider,
            [CanBeNull] [InstantHandle] Func<string, Optional<object>> resolver,
            bool isCaseSensitive = false)
        {
            using (StringWriter writer = new StringWriter(formatProvider))
            {
                position = WriteTo(writer, layout, position, null, resolver, isCaseSensitive);
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
        [StringFormatMethod("format")]
        public string ToString([CanBeNull] string format, [CanBeNull] IFormatProvider formatProvider = null)
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
        /// <param name="layout">The layout is applied to the original <see cref="InitialLayout" />.</param>
        /// <param name="position">The position.</param>
        /// <param name="format">The format.
        /// <list type="table"><listheader><term>Format string</term><description>Description</description></listheader><item><term>G/g/null</term><description>Any unresolved fill points will have their tags output. Control chunks are ignored.</description></item><item><term>F/f</term><description>All control and fill point chunks will have their tags output.</description></item><item><term>S/s</term><description>Any unresolved fill points will be treated as an empty string. Control chunks are ignored.</description></item></list></param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        [NotNull]
        [PublicAPI]
        [StringFormatMethod("format")]
        public string ToString(
            [CanBeNull]Layout layout,
            ref int position,
            [CanBeNull] string format,
            [CanBeNull] IFormatProvider formatProvider = null)
        {
            using (StringWriter writer = new StringWriter(formatProvider))
            {
                position = WriteTo(writer, layout, position, format);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format.
        /// <list type="table"><listheader><term>Format string</term><description>Description</description></listheader><item><term>G/g/null</term><description>Any unresolved fill points will have their tags output. Control chunks are ignored.</description></item><item><term>F/f</term><description>All control and fill point chunks will have their tags output.</description></item><item><term>S/s</term><description>Any unresolved fill points will be treated as an empty string. Control chunks are ignored.</description></item></list></param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="values">The values.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        [NotNull]
        [PublicAPI]
        [StringFormatMethod("format")]
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
        /// <param name="layout">The layout is applied to the original <see cref="InitialLayout"/>.</param>
        /// <param name="position">The position.</param>
        /// <param name="format">The format.
        /// <list type="table"><listheader><term>Format string</term><description>Description</description></listheader><item><term>G/g/null</term><description>Any unresolved fill points will have their tags output. Control chunks are ignored.</description></item><item><term>F/f</term><description>All control and fill point chunks will have their tags output.</description></item><item><term>S/s</term><description>Any unresolved fill points will be treated as an empty string. Control chunks are ignored.</description></item></list></param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="values">The values.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        [NotNull]
        [PublicAPI]
        [StringFormatMethod("format")]
        public string ToString(
            [CanBeNull]Layout layout,
            ref int position,
            [CanBeNull] string format,
            [CanBeNull] IFormatProvider formatProvider,
            [CanBeNull] params object[] values)
        {
            using (StringWriter writer = new StringWriter(formatProvider))
            {
                position = WriteTo(writer, layout, position, format, values);
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
        [StringFormatMethod("format")]
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
        /// <param name="layout">The layout is applied to the original <see cref="InitialLayout"/>.</param>
        /// <param name="position">The position.</param>
        /// <param name="format">The format.
        /// <list type="table"><listheader><term>Format string</term><description>Description</description></listheader><item><term>G/g/null</term><description>Any unresolved fill points will have their tags output. Control chunks are ignored.</description></item><item><term>F/f</term><description>All control and fill point chunks will have their tags output.</description></item><item><term>S/s</term><description>Any unresolved fill points will be treated as an empty string. Control chunks are ignored.</description></item></list></param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="values">The values.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        [NotNull]
        [PublicAPI]
        [StringFormatMethod("format")]
        public string ToString(
            [CanBeNull]Layout layout,
            ref int position,
            [CanBeNull] string format,
            [CanBeNull] IFormatProvider formatProvider,
            [CanBeNull] IReadOnlyDictionary<string, object> values)
        {
            using (StringWriter writer = new StringWriter(formatProvider))
            {
                position = WriteTo(writer, layout, position, format, values);
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
        /// <param name="isCaseSensitive">if set to <see langword="true" /> then tags are case sensitive.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        [NotNull]
        [PublicAPI]
        [StringFormatMethod("format")]
        public string ToString(
            [CanBeNull] string format,
            [CanBeNull] IFormatProvider formatProvider,
            [CanBeNull] Func<string, Optional<object>> resolver,
            bool isCaseSensitive = false)
        {
            using (StringWriter writer = new StringWriter(formatProvider))
            {
                WriteTo(writer, format, resolver, isCaseSensitive);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="layout">The layout is applied to the original <see cref="InitialLayout"/>.</param>
        /// <param name="position">The position.</param>
        /// <param name="format">The format.
        /// <list type="table"><listheader><term>Format string</term><description>Description</description></listheader><item><term>G/g/null</term><description>Any unresolved fill points will have their tags output. Control chunks are ignored.</description></item><item><term>F/f</term><description>All control and fill point chunks will have their tags output.</description></item><item><term>S/s</term><description>Any unresolved fill points will be treated as an empty string. Control chunks are ignored.</description></item></list></param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="resolver">The resolver.</param>
        /// <param name="isCaseSensitive">if set to <see langword="true" /> then tags are case sensitive.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        [NotNull]
        [PublicAPI]
        [StringFormatMethod("format")]
        public string ToString(
            [CanBeNull]Layout layout,
            ref int position,
            [CanBeNull] string format,
            [CanBeNull] IFormatProvider formatProvider,
            [CanBeNull] Func<string, Optional<object>> resolver,
            bool isCaseSensitive = false)
        {
            using (StringWriter writer = new StringWriter(formatProvider))
            {
                position = WriteTo(writer, layout, position, format, resolver, isCaseSensitive);
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
            if (_chunks.Count < 1) return;
            WriteTo(_chunks, ConsoleTextWriter.Default, InitialLayout, "G", _isLayoutRequired, 0);
        }

        /// <summary>
        /// Writes the builder to the console.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        [PublicAPI]
        [StringFormatMethod("format")]
        public void WriteToConsole(
            [CanBeNull] string format,
            [CanBeNull] params object[] values)
        {
            if (_chunks.Count < 1) return;
            bool isLayoutRequired = _isLayoutRequired;
            WriteTo(
                values != null && values.Length > 0
                    ? Resolve(_chunks, values, ref isLayoutRequired)
                    : _chunks,
                ConsoleTextWriter.Default,
                InitialLayout,
                format,
                isLayoutRequired,
                0);
        }

        /// <summary>
        /// Writes the builder to the console.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        [PublicAPI]
        [StringFormatMethod("format")]
        public void WriteToConsole(
            [CanBeNull] string format,
            [CanBeNull] IReadOnlyDictionary<string, object> values)
        {
            if (_chunks.Count < 1) return;
            bool isLayoutRequired = _isLayoutRequired;
            WriteTo(
                values != null && values.Count > 0
                    ? Resolve(_chunks, values, ref isLayoutRequired)
                    : _chunks,
                ConsoleTextWriter.Default,
                InitialLayout,
                format,
                isLayoutRequired,
                0);
        }

        /// <summary>
        /// Writes the builder to the console.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="resolver">The resolver.</param>
        /// <param name="isCaseSensitive">if set to <see langword="true" /> then tags are case sensitive.</param>
        [PublicAPI]
        [StringFormatMethod("format")]
        public void WriteToConsole(
            [CanBeNull] string format,
            [CanBeNull] [InstantHandle] Func<string, Optional<object>> resolver,
            bool isCaseSensitive = false)
        {
            if (_chunks.Count < 1) return;
            bool isLayoutRequired = _isLayoutRequired;
            WriteTo(
                resolver != null
                    ? Resolve(_chunks, resolver, isCaseSensitive, ref isLayoutRequired)
                    : _chunks,
                ConsoleTextWriter.Default,
                InitialLayout,
                format,
                isLayoutRequired,
                0);
        }
        #endregion

        #region WriteToTrace Overloads
        /// <summary>
        /// Writes the builder to <see cref="Trace"/>.
        /// </summary>
        [PublicAPI]
        public void WriteToTrace()
        {
            if (_chunks.Count < 1) return;
            WriteTo(_chunks, TraceTextWriter.Default, InitialLayout, "G", _isLayoutRequired, 0);
        }

        /// <summary>
        /// Writes the builder to <see cref="Trace" />.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        [PublicAPI]
        [StringFormatMethod("format")]
        public void WriteToTrace(
            [CanBeNull] string format,
            [CanBeNull] params object[] values)
        {
            if (_chunks.Count < 1) return;
            bool isLayoutRequired = _isLayoutRequired;
            WriteTo(
                values != null && values.Length > 0
                    ? Resolve(_chunks, values, ref isLayoutRequired)
                    : _chunks,
                TraceTextWriter.Default,
                InitialLayout,
                format,
                isLayoutRequired,
                0);
        }

        /// <summary>
        /// Writes the builder to <see cref="Trace" />.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        [PublicAPI]
        [StringFormatMethod("format")]
        public void WriteToTrace(
            [CanBeNull] string format,
            [CanBeNull] IReadOnlyDictionary<string, object> values)
        {
            if (_chunks.Count < 1) return;
            bool isLayoutRequired = _isLayoutRequired;
            WriteTo(
                values != null && values.Count > 0
                    ? Resolve(_chunks, values, ref isLayoutRequired)
                    : _chunks,
                TraceTextWriter.Default,
                InitialLayout,
                format,
                isLayoutRequired,
                0);
        }

        /// <summary>
        /// Writes the builder to <see cref="Trace" />.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="resolver">The resolver.</param>
        /// <param name="isCaseSensitive">if set to <see langword="true" /> then tags are case sensitive.</param>
        [PublicAPI]
        [StringFormatMethod("format")]
        public void WriteToTrace(
            [CanBeNull] string format,
            [CanBeNull] [InstantHandle] Func<string, Optional<object>> resolver,
            bool isCaseSensitive = false)
        {
            if (_chunks.Count < 1) return;
            bool isLayoutRequired = _isLayoutRequired;
            WriteTo(
                resolver != null
                    ? Resolve(_chunks, resolver, isCaseSensitive, ref isLayoutRequired)
                    : _chunks,
                TraceTextWriter.Default,
                InitialLayout,
                format,
                isLayoutRequired,
                0);
        }
        #endregion

        #region WriteTo Overloads
        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="position">The start position.</param>
        /// <param name="layout">The layout is applied to the original <see cref="InitialLayout"/>.</param>
        /// <returns>The end position.</returns>
        [PublicAPI]
        public int WriteTo([CanBeNull] TextWriter writer, [CanBeNull] Layout layout = null, int position = 0)
        {
            return writer == null || _chunks.Count < 1
                ? position
                : WriteTo(
                    _chunks,
                    writer,
                    layout == null ? InitialLayout : InitialLayout.Apply(layout),
                    "G",
                    _isLayoutRequired,
                    position);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <returns>The end position.</returns>
        [PublicAPI]
        [StringFormatMethod("format")]
        public void WriteTo(
            [CanBeNull] TextWriter writer, string format)
        {
            if (writer == null ||
                _chunks.Count < 1) return;
            bool isLayoutRequired = _isLayoutRequired;
            WriteTo(
                _chunks,
                writer,
                InitialLayout,
                format,
                isLayoutRequired,
                0);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        /// <returns>The end position.</returns>
        [PublicAPI]
        [StringFormatMethod("format")]
        public int WriteTo(
            [CanBeNull] TextWriter writer,
            [CanBeNull] string format,
            [CanBeNull] params object[] values)
        {
            if (writer == null ||
                _chunks.Count < 1) return 0;
            bool isLayoutRequired = _isLayoutRequired;
            return WriteTo(
                values != null && values.Length > 0
                    ? Resolve(_chunks, values, ref isLayoutRequired)
                    : _chunks,
                writer,
                InitialLayout,
                format,
                isLayoutRequired,
                0);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="layout">The layout is applied to the original <see cref="InitialLayout"/>.</param>
        /// <param name="position">The start position.</param>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        /// <returns>The end position.</returns>
        [PublicAPI]
        [StringFormatMethod("format")]
        public int WriteTo(
            [CanBeNull] TextWriter writer,
            [CanBeNull] Layout layout,
            int position,
            [CanBeNull] string format,
            [CanBeNull] params object[] values)
        {
            if (writer == null ||
                _chunks.Count < 1) return position;
            bool isLayoutRequired = _isLayoutRequired;
            return WriteTo(
                values != null && values.Length > 0
                    ? Resolve(_chunks, values, ref isLayoutRequired)
                    : _chunks,
                writer,
                layout == null ? InitialLayout : InitialLayout.Apply(layout),
                format,
                isLayoutRequired,
                position);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        /// <returns>The end position.</returns>
        [PublicAPI]
        [StringFormatMethod("format")]
        public int WriteTo(
            [CanBeNull] TextWriter writer,
            [CanBeNull] string format,
            [CanBeNull] IReadOnlyDictionary<string, object> values)
        {
            if (writer == null ||
                _chunks.Count < 1) return 0;
            bool isLayoutRequired = _isLayoutRequired;
            return WriteTo(
                values != null && values.Count > 0
                    ? Resolve(_chunks, values, ref isLayoutRequired)
                    : _chunks,
                writer,
                InitialLayout,
                format,
                isLayoutRequired,
                0);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="layout">The layout is applied to the original <see cref="InitialLayout"/>.</param>
        /// <param name="position">The start position.</param>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        /// <returns>The end position.</returns>
        [PublicAPI]
        [StringFormatMethod("format")]
        public int WriteTo(
            [CanBeNull] TextWriter writer,
            [CanBeNull] Layout layout,
            int position,
            [CanBeNull] string format,
            [CanBeNull] IReadOnlyDictionary<string, object> values)
        {
            if (writer == null ||
                _chunks.Count < 1) return position;
            bool isLayoutRequired = _isLayoutRequired;
            return WriteTo(
                values != null && values.Count > 0
                    ? Resolve(_chunks, values, ref isLayoutRequired)
                    : _chunks,
                writer,
                layout == null ? InitialLayout : InitialLayout.Apply(layout),
                format,
                isLayoutRequired,
                position);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <param name="resolver">The resolver.</param>
        /// <param name="isCaseSensitive">if set to <see langword="true" /> then tags are case sensitive.</param>
        /// <returns>The end position.</returns>
        [PublicAPI]
        [StringFormatMethod("format")]
        public int WriteTo(
            [CanBeNull] TextWriter writer,
            [CanBeNull] string format,
            [CanBeNull] [InstantHandle] Func<string, Optional<object>> resolver,
            bool isCaseSensitive = false)
        {
            if (writer == null ||
                _chunks.Count < 1) return 0;
            bool isLayoutRequired = _isLayoutRequired;
            return WriteTo(
                resolver != null
                    ? Resolve(_chunks, resolver, isCaseSensitive, ref isLayoutRequired)
                    : _chunks,
                writer,
                InitialLayout,
                format,
                isLayoutRequired,
                0);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="layout">The layout is applied to the original <see cref="InitialLayout"/>.</param>
        /// <param name="position">The start position.</param>
        /// <param name="format">The format.</param>
        /// <param name="resolver">The resolver.</param>
        /// <param name="isCaseSensitive">if set to <see langword="true" /> then tags are case sensitive.</param>
        /// <returns>The end position.</returns>
        [PublicAPI]
        [StringFormatMethod("format")]
        public int WriteTo(
            [CanBeNull] TextWriter writer,
            [CanBeNull] Layout layout,
            int position,
            [CanBeNull] string format,
            [CanBeNull] [InstantHandle] Func<string, Optional<object>> resolver,
            bool isCaseSensitive = false)
        {
            if (writer == null ||
                _chunks.Count < 1) return position;
            bool isLayoutRequired = _isLayoutRequired;
            return WriteTo(
                resolver != null
                    ? Resolve(_chunks, resolver, isCaseSensitive, ref isLayoutRequired)
                    : _chunks,
                writer,
                layout == null ? InitialLayout : InitialLayout.Apply(layout),
                format,
                isLayoutRequired,
                position);
        }
        #endregion

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="chunks">The chunks.</param>
        /// <param name="writer">The writer.</param>
        /// <param name="initialLayout">The initial layout.</param>
        /// <param name="format">The format.</param>
        /// <param name="isLayoutRequired">if set to <see langword="true" /> then layout is required.</param>
        /// <param name="position">The position.</param>
        /// <returns>The final position.</returns>
        [PublicAPI]
        [StringFormatMethod("format")]
        private static int WriteTo(
            [NotNull] [InstantHandle] IEnumerable<FormatChunk> chunks,
            [NotNull] TextWriter writer,
            [NotNull] Layout initialLayout,
            [CanBeNull] string format,
            bool isLayoutRequired,
            int position)
        {
            Contract.Requires(chunks != null);
            Contract.Requires(writer != null);
            Contract.Requires(initialLayout != null);
            ISerialTextWriter serialTextWriter = writer as ISerialTextWriter;
            return serialTextWriter == null
                ? DoWrite(chunks, writer, null, initialLayout, format, isLayoutRequired, position)
                : serialTextWriter.Context.Invoke(
                    () =>
                        DoWrite(
                            chunks,
                            writer,
                            serialTextWriter.Writer,
                            initialLayout,
                            format,
                            isLayoutRequired,
                            position));
        }

        /// <summary>
        /// The new line characters.
        /// </summary>
        [NotNull]
        private static readonly char[] _newLineChars = { '\r', '\n' };

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="chunks">The chunks.</param>
        /// <param name="writer">The writer.</param>
        /// <param name="serialWriter">The serial writer.</param>
        /// <param name="initialLayout">The initial layout.</param>
        /// <param name="format">The format.</param>
        /// <param name="isLayoutRequired">if set to <see langword="true" /> then layout is required.</param>
        /// <param name="position">The position.</param>
        /// <returns>The final position.</returns>
        [PublicAPI]
        [StringFormatMethod("format")]
        private static int DoWrite(
            [NotNull] [InstantHandle] IEnumerable<FormatChunk> chunks,
            [NotNull] TextWriter writer,
            [CanBeNull] TextWriter serialWriter,
            [NotNull] Layout initialLayout,
            [CanBeNull] string format,
            bool isLayoutRequired,
            int position)
        {
            Contract.Requires(chunks != null);
            Contract.Requires(writer != null);
            Contract.Requires(initialLayout != null);
            if (format == null)
                format = "g";

            bool writeTags;
            if (string.Equals(format, "f", StringComparison.InvariantCultureIgnoreCase))
            {
                writeTags = true;
                isLayoutRequired = false;
            }
            else
                writeTags = false;

            // ReSharper disable SuspiciousTypeConversion.Global
            IControllableTextWriter controller = serialWriter as IControllableTextWriter ?? writer as IControllableTextWriter;
            // ReSharper restore SuspiciousTypeConversion.Global
            ILayoutTextWriter layoutWriter = serialWriter as ILayoutTextWriter ?? writer as ILayoutTextWriter;
            IColoredTextWriter coloredTextWriter = serialWriter as IColoredTextWriter ?? writer as IColoredTextWriter;
            if (serialWriter != null) writer = serialWriter;
            IFormatProvider formatProvider = writer.FormatProvider;

            /*
             * If we're not using a layout writer, and we don't require layout, then we need to manually track position.
             * so the code is subtly different.
             */
            if (layoutWriter == null && !isLayoutRequired)
            {
                foreach (FormatChunk chunk in chunks)
                    // ReSharper disable once PossibleNullReferenceException
                    if (chunk.IsControl &&
                        !writeTags)
                    {
                        if (controller != null)
                            controller.OnControlChunk(chunk, format, formatProvider);

                        if (coloredTextWriter == null)
                            continue;

                        // Handle colored output.
                        // ReSharper disable PossibleNullReferenceException
                        switch (chunk.Tag.ToLowerInvariant())
                        // ReSharper restore PossibleNullReferenceException
                        {
                            case ResetColorsTag:
                                coloredTextWriter.ResetColors();
                                continue;
                            case ForegroundColorTag:
                                if (String.IsNullOrWhiteSpace(chunk.Format))
                                    coloredTextWriter.ResetForegroundColor();
                                else if (chunk.IsResolved &&
                                         chunk.Value is Color)
                                    coloredTextWriter.SetForegroundColor((Color)chunk.Value);
                                else
                                {
                                    // ReSharper disable once AssignNullToNotNullAttribute
                                    Optional<Color> color = ColorHelper.GetColor(chunk.Format);
                                    if (color.IsAssigned)
                                        coloredTextWriter.SetForegroundColor(color.Value);
                                }
                                continue;
                            case BackgroundColorTag:
                                if (String.IsNullOrWhiteSpace(chunk.Format))
                                    coloredTextWriter.ResetBackgroundColor();
                                else if (chunk.IsResolved &&
                                         chunk.Value is Color)
                                    coloredTextWriter.SetBackgroundColor((Color)chunk.Value);
                                else
                                {
                                    // ReSharper disable once AssignNullToNotNullAttribute
                                    Optional<Color> color = ColorHelper.GetColor(chunk.Format);
                                    if (color.IsAssigned)
                                        coloredTextWriter.SetBackgroundColor(color.Value);
                                }
                                continue;
                            default:
                                continue;
                        }
                    }
                    else
                    {
                        // Get the chunk as a string
                        string result = chunk.ToString(format, writer.FormatProvider);
                        if (result.Length < 1) continue;
                        writer.Write(result);

                        // The result is a chunk, so we need to find the distance since the last newline.
                        int index = result.LastIndexOfAny(_newLineChars);
                        position = index < 0
                            ? result.Length + position
                            : result.Length - index - 1;
                    }

                // Get current position from writer.
                return position;
            }

            /*
             * We don't have a layout writer so we have to track horizontal position ourselves.
             */

            // If we require layout, run chunks through layout engine.
            // Get current state from the writer.

            int writerWidth;
            bool autoWraps;
            if (layoutWriter != null)
            {
                position = layoutWriter.Position;
                writerWidth = layoutWriter.Width;
                autoWraps = layoutWriter.AutoWraps;
            }
            else
            {
                writerWidth = int.MaxValue;
                autoWraps = false;
            }

            // If we require layout, run chunks through layout engine.
            IEnumerable<FormatChunk> enumerable = isLayoutRequired ||
                                     (writerWidth < int.MaxValue)
                ? Align(
                    GetLines(
                        GetLineChunks(chunks, format, writer.FormatProvider),
                        initialLayout,
                        position,
                        writerWidth),
                    initialLayout,
                    writerWidth,
                    autoWraps,
                    ref position)
                : chunks;

            // We try to output the builder in one go to prevent interleaving, however we split on control codes.
            foreach (FormatChunk chunk in enumerable)
                // ReSharper disable once PossibleNullReferenceException
                if (chunk.IsControl &&
                    !writeTags)
                {
                    if (controller != null)
                        controller.OnControlChunk(chunk, format, formatProvider);

                    if (coloredTextWriter == null)
                        continue;

                    // Handle colored output.
                    // ReSharper disable once PossibleNullReferenceException
                    switch (chunk.Tag.ToLowerInvariant())
                    {
                        case ResetColorsTag:
                            coloredTextWriter.ResetColors();
                            continue;
                        case ForegroundColorTag:
                            if (String.IsNullOrWhiteSpace(chunk.Format))
                                coloredTextWriter.ResetForegroundColor();
                            else if (chunk.IsResolved &&
                                     chunk.Value is Color)
                                coloredTextWriter.SetForegroundColor((Color)chunk.Value);
                            else
                            {
                                // ReSharper disable once AssignNullToNotNullAttribute
                                Optional<Color> color = ColorHelper.GetColor(chunk.Format);
                                if (color.IsAssigned)
                                    coloredTextWriter.SetForegroundColor(color.Value);
                            }
                            continue;
                        case BackgroundColorTag:
                            if (String.IsNullOrWhiteSpace(chunk.Format))
                                coloredTextWriter.ResetBackgroundColor();
                            else if (chunk.IsResolved &&
                                     chunk.Value is Color)
                                coloredTextWriter.SetBackgroundColor((Color)chunk.Value);
                            else
                            {
                                // ReSharper disable once AssignNullToNotNullAttribute
                                Optional<Color> color = ColorHelper.GetColor(chunk.Format);
                                if (color.IsAssigned)
                                    coloredTextWriter.SetBackgroundColor(color.Value);
                            }
                            continue;
                        default:
                            continue;
                    }
                }
                else
                    chunk.WriteTo(writer, format);

            // Get current position from writer if we have one, otherwise our current position should be accurate anyway.
            return layoutWriter == null
                ? position
                : layoutWriter.Position;
        }

        /// <summary>
        /// Gets the line chunks from a set of chunks.
        /// </summary>
        /// <param name="chunks">The chunks.</param>
        /// <param name="format">The format.</param>
        /// <param name="provider">The format provider.</param>
        /// <returns>An enumeration of chunks.</returns>
        [NotNull]
        [StringFormatMethod("format")]
        private static Tuple<IEnumerable<string>, IEnumerable<FormatChunk>> GetLineChunks(
            [NotNull] [InstantHandle] IEnumerable<FormatChunk> chunks,
            [CanBeNull] string format,
            [CanBeNull] IFormatProvider provider)
        {
            Contract.Requires(chunks != null);
            List<string> words = new List<string>();
            List<FormatChunk> controlChunks = new List<FormatChunk>();

            StringBuilder word = new StringBuilder();
            bool lastCharR = false;
            char lastCharSymb = '\0';
            foreach (FormatChunk chunk in chunks)
            {
                // ReSharper disable once PossibleNullReferenceException
                if (chunk.IsControl)
                {
                    if (word.Length > 0)
                    {
                        words.Add(word.ToString());
                        word.Clear();
                    }

                    // Use null to indicate location of a control chunks
                    words.Add(null);
                    controlChunks.Add(chunk);
                    continue;
                }

                string chunk1 = chunk.ToString(format, provider);
                if (string.IsNullOrEmpty(chunk1)) continue;

                foreach (char ch in chunk1)
                {
                    if (char.IsLetterOrDigit(ch))
                    {
                        if (lastCharSymb != '\0')
                        {
                            // This letter follows a non alpha numeric, so split the word, unless the character was an
                            // apostrophe, i.e. keep "Craig's" and "I'm" as one word.
                            if (lastCharSymb != '\'' &&
                                word.Length > 0)
                            {
                                words.Add(word.ToString());
                                word.Clear();
                            }
                            lastCharSymb = '\0';
                        }
                        word.Append(ch);
                        continue;
                    }
                    if (word.Length > 0)
                    {
                        if (lastCharSymb == '\0' &&
                            !char.IsWhiteSpace(ch))
                        {
                            lastCharSymb = ch;
                            word.Append(ch);
                            continue;
                        }
                        words.Add(word.ToString());
                        word.Clear();
                    }

                    lastCharSymb = '\0';
                    if (ch == '\n')
                    {
                        // Skip '\n' after '\r'
                        if (!lastCharR)
                            words.Add("\r");

                        lastCharR = false;
                        continue;
                    }

                    lastCharR = ch == '\r';

                    words.Add(ch.ToString(provider));
                }
            }
            if (word.Length > 0)
                words.Add(word.ToString());

            return new Tuple<IEnumerable<string>, IEnumerable<FormatChunk>>(words, controlChunks);
        }

        /// <summary>
        /// Gets the lines from an enumeration of chunks.
        /// </summary>
        /// <param name="chunks">The chunks.</param>
        /// <param name="initialLayout">The initial layout.</param>
        /// <param name="position">The position.</param>
        /// <param name="writerWidth">The writer's width.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        [NotNull]
        private static IEnumerable<Line> GetLines(
            [NotNull] Tuple<IEnumerable<string>, IEnumerable<FormatChunk>> chunks,
            [NotNull] Layout initialLayout,
            int position,
            int writerWidth)
        {
            Contract.Requires(chunks != null);
            Contract.Requires(initialLayout != null);
            Contract.Requires(chunks.Item1 != null);
            Contract.Requires(chunks.Item2 != null);

            // Only grab the layout at the start of each line.
            Layout nextLayout = initialLayout;
            if (nextLayout.Width.Value >= writerWidth)
                nextLayout = nextLayout.Apply(writerWidth);

            Layout layout = nextLayout;

            // Create the first line, if we're part way through a line then we cannot align the remainder of the line.
            Line line = position > 0
                ? new Line(
                    layout,
                    Alignment.None,
                    position,
                    layout.Width.Value - layout.RightMarginSize.Value,
                    false)
                : new Line(
                    layout,
                    layout.Alignment.Value,
                    layout.FirstLineIndentSize.Value,
                    layout.Width.Value - layout.RightMarginSize.Value,
                    true);
            bool firstLine = false;

            byte splitLength = layout.SplitLength.Value;
            int hyphenate = layout.Hyphenate.Value ? 1 : 0;

            // ReSharper disable PossibleNullReferenceException
            IEnumerator<string> chunkEnumerator = chunks.Item1.GetEnumerator();
            IEnumerator<FormatChunk> controlEnumerator = chunks.Item2.GetEnumerator();
            // ReSharper restore PossibleNullReferenceException

            string word = null;
            bool newLine = false;

            do
            {
                // Check if we need to start a new line.
                if (newLine)
                {
                    // Close out existing line.
                    line.Finish(true, firstLine);
                    yield return line;

                    // Start a new line
                    if (nextLayout.Width.Value > writerWidth)
                        nextLayout = nextLayout.Apply(writerWidth);

                    layout = nextLayout;
                    line = new Line(
                        layout,
                        layout.Alignment.Value,
                        firstLine ? layout.FirstLineIndentSize.Value : layout.IndentSize.Value,
                        layout.Width.Value - layout.RightMarginSize.Value,
                        firstLine);
                    firstLine = false;
                    splitLength = layout.SplitLength.Value;
                    hyphenate = layout.Hyphenate.Value ? 1 : 0;
                    newLine = false;
                    position = 0;
                }

                // If we don't have a word, get one.
                if (string.IsNullOrEmpty(word))
                    do
                    {
                        if (!chunkEnumerator.MoveNext())
                        {
                            if (line.ChunkCount > 0)
                            {
                                line.Finish(false, false);
                                yield return line;
                            }

                            // No more words, so finish.
                            yield break;
                        }
                        word = chunkEnumerator.Current;

                        // Check if we have a control marker
                        if (!string.IsNullOrEmpty(word)) break;

                        controlEnumerator.MoveNext();

                        FormatChunk controlChunk = controlEnumerator.Current;
                        Contract.Assert(controlChunk != null);

                        // If the control chunk is a layout chunk, we need to get the layout
                        // ReSharper disable once PossibleNullReferenceException
                        if (string.Equals(controlChunk.Tag, "!layout", StringComparison.InvariantCultureIgnoreCase))
                        {
                            Layout newLayout;
                            if ((controlChunk.IsResolved) &&
                                ((newLayout = controlChunk.Value as Layout) != null))
                                nextLayout = ReferenceEquals(newLayout, Layout.Default)
                                    ? initialLayout
                                    : nextLayout.Apply(newLayout);
                            else if (string.IsNullOrEmpty(controlChunk.Format))
                                nextLayout = initialLayout;
                            else if (Layout.TryParse(controlChunk.Format, out newLayout))
                                nextLayout = nextLayout.Apply(newLayout);

                            // If the line is empty, we can recreate the line using the new layout,
                            // otherwise the new layout only applies on the next line.
                            if (!line.IsEmpty) continue;

                            // Check if the current position is past the new layout's width
                            int start = position > 0 ? position : line.Position;
                            if (start >= nextLayout.Width.Value)
                            {
                                // Start a new line, as we are past the current lines width.
                                newLine = true;
                                // ReSharper disable once RedundantAssignment
                                firstLine = false;
                                continue;
                            }

                            // Re-create current line now.
                            if (nextLayout.Width.Value >= writerWidth)
                                nextLayout = nextLayout.Apply(writerWidth);

                            layout = nextLayout;
                            firstLine = line.IsFirstLine;

                            if (position < 1)
                            {
                                // Move start if we're not already 
                                int newStart = firstLine ? layout.FirstLineIndentSize.Value : layout.IndentSize.Value;
                                if (newStart > start) start = newStart;
                            }

                            line = new Line(
                                layout,
                                layout.Alignment.Value,
                                start,
                                line.End,
                                firstLine);
                            splitLength = layout.SplitLength.Value;
                            hyphenate = layout.Hyphenate.Value ? 1 : 0;
                        }
                        else
                            line.AddControl(controlChunk);
                    } while (true);

                char c = word[0];

                // Check if we're at the start of a line.
                byte split = splitLength;
                if (line.IsEmpty)
                {
                    // Skip spaces at the start of a line, if we have an alignment
                    if ((c == ' ') &&
                        (line.Alignment != Alignment.None))
                    {
                        word = null;
                        continue;
                    }

                    // We split this word if it's too long, as we're going from the start of a line.
                    split = 1;
                }

                // Check for newline
                if (c == '\r')
                {
                    newLine = true;
                    firstLine = true;
                    word = null;
                    continue;
                }

                int remaining = line.Remaining;

                if (remaining < 1)
                {
                    // Process word on a new line, as we're at the end of this one.
                    newLine = true;
                    continue;
                }

                // Check for tab
                if (c == '\t')
                {
                    int tabSize;
                    if (layout.TabStops.IsAssigned &&
                        layout.TabStops.Value != null)
                    {
                        // ReSharper disable once PossibleNullReferenceException
                        int nextTab = layout.TabStops.Value.FirstOrDefault(t => t > line.Position);
                        tabSize = nextTab > line.Position
                            ? nextTab - line.Position
                            : layout.TabSize.Value;
                    }
                    else
                        tabSize = layout.TabSize.Value;

                    // Change word to spacer
                    word = new string(layout.TabChar.Value, tabSize);
                }

                // Append word if short enough.
                if (word.Length <= remaining)
                {
                    line.Add(word);
                    word = null;
                    continue;
                }

                // The word is too long to fit on the current line.
                int maxSplit = word.Length - split;
                if ((split > 0) &&
                    (remaining >= (hyphenate + splitLength)) &&
                    (maxSplit >= split))
                {
                    // Split the current word to fill remaining space
                    int splitPoint = remaining - hyphenate;

                    // Can only split if enough characters are left on line.
                    if (splitPoint > maxSplit)
                        splitPoint = maxSplit;

                    string part = word.Substring(0, splitPoint);
                    if (hyphenate > 0) part += layout.HyphenChar;
                    line.Add(part);
                    word = word.Substring(splitPoint);
                }

                // Start a new line
                newLine = true;
            } while (true);
        }

        /// <summary>
        /// Aligns the specified lines.
        /// </summary>
        /// <param name="lines">The lines.</param>
        /// <param name="initialLayout">The initial layout.</param>
        /// <param name="writerWidth">Width of the writer.</param>
        /// <param name="autoWraps">if set to <see langword="true" /> then the writer automatically wraps on reaching width.</param>
        /// <param name="position">The position.</param>
        /// <returns>An enumeration of terminated lines, laid out for writing.</returns>
        [NotNull]
        private static IEnumerable<FormatChunk> Align(
            [NotNull] [InstantHandle] IEnumerable<Line> lines,
            [NotNull] Layout initialLayout,
            int writerWidth,
            bool autoWraps,
            ref int position)
        {
            Contract.Requires(lines != null);
            Contract.Requires(initialLayout != null);
            StringBuilder lb = new StringBuilder(initialLayout.Width.Value < 1024 ? initialLayout.Width.Value : 1024);
            bool dontIndentFirstLine = position > 0;

            List<FormatChunk> chunks = new List<FormatChunk>();

            foreach (Line line in lines)
            {
                // ReSharper disable once PossibleNullReferenceException
                char indentChar = line.Layout.IndentChar.Value;
                int indent;
                Queue<int> spacers = null;
                // Calculate indentation
                switch (line.Alignment)
                {
                    case Alignment.None:
                    case Alignment.Left:
                        indent = line.Start;
                        break;
                    case Alignment.Centre:
                        indent = (line.Start + line.End - line.Length) / 2;
                        break;
                    case Alignment.Right:
                        indent = line.End - line.Length;
                        break;
                    case Alignment.Justify:
                        indent = line.Start;
                        int remaining = line.Remaining;
                        if (remaining > 0 && line.LastWhiteSpace > 0)
                        {
                            decimal space = (decimal)(line.LastWhiteSpace - line.Start) / remaining;
                            int o = (int)Math.Round(space / 2);
                            spacers = new Queue<int>(Enumerable.Range(0, remaining).Select(r => o + (int)(space * r)));
                        }
                        break;
                    default:
                        indent = 0;
                        break;
                }

                if (dontIndentFirstLine)
                    dontIndentFirstLine = false;
                else if (indent > 0)
                    lb.Append(indentChar, indent);

                int p = 0;
                IEnumerator<FormatChunk> controlEnumerator = line.Controls.GetEnumerator();
                foreach (string chunk in line)
                {
                    if (chunk == null)
                    {
                        // We got a control chunk, so need to split line
                        if (lb.Length > 0)
                        {
                            chunks.Add(FormatChunk.Create(lb.ToString()));
                            lb.Clear();
                        }
                        // Recover control chunk
                        controlEnumerator.MoveNext();
                        Contract.Assert(controlEnumerator.Current != null);
                        chunks.Add(controlEnumerator.Current);
                        continue;
                    }

                    p += chunk.Length;
                    if (!string.IsNullOrWhiteSpace(chunk))
                    {
                        lb.Append(chunk);
                        continue;
                    }

                    // We have a white-space chunk, check if we have to add justification spaces
                    if (spacers != null)
                    {
                        while ((spacers.Count > 0) &&
                               (spacers.Peek() <= p))
                        {
                            lb.Append(indentChar);
                            spacers.Dequeue();
                            p++;
                        }

                        // Check if justification is finished
                        if (spacers.Count < 1)
                            spacers = null;
                    }

                    lb.Append(chunk);
                }

                // Add any remaining spacers
                if ((spacers != null) &&
                    (spacers.Count > 0))
                {
                    lb.Append(indentChar, spacers.Count);
                    p += spacers.Count;
                }

                // Calculate our finish position
                position = p + indent;

                if (line.Terminated)
                {
                    // Wrap the line according to our mode.
                    switch (line.Layout.WrapMode.Value)
                    {
                        case LayoutWrapMode.NewLineOnShort:
                            if (position < line.Layout.Width.Value)
                                lb.AppendLine();
                            break;
                        case LayoutWrapMode.PadToWrap:
                            lb.Append(
                                line.Layout.IndentChar.Value,
                                (writerWidth < int.MaxValue ? writerWidth : line.Layout.Width.Value) - position);
                            break;
                        default:
                            if (!autoWraps ||
                                (position < writerWidth))
                                lb.AppendLine();
                            break;
                    }

                    // Set position to start of line.
                    position = 0;
                }

                if (lb.Length > 0)
                {
                    chunks.Add(FormatChunk.Create(lb.ToString()));
                    lb.Clear();
                }
                lb.Clear();
            }
            return chunks;
        }

        #region Color Control
        /// <summary>
        /// The reset colors control tag.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public const string ResetColorsTag = "!resetcolors";

        /// <summary>
        /// The reset colors chunk.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly FormatChunk ResetColorsChunk = FormatChunk.CreateControl(ResetColorsTag);

        /// <summary>
        /// The foreground color control tag.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public const string ForegroundColorTag = "!fgcolor";

        /// <summary>
        /// The background color control tag.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public const string BackgroundColorTag = "!bgcolor";

        /// <summary>
        /// The reset foreground color chunk.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly FormatChunk ResetForegroundColorChunk = FormatChunk.CreateControl(ForegroundColorTag);

        /// <summary>
        /// The reset background color chunk.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly FormatChunk ResetBackgroundColorChunk = FormatChunk.CreateControl(BackgroundColorTag);

        /// <summary>
        /// Adds a control to reset the foreground and background colors
        /// </summary>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendResetColors()
        {
            return AppendControl(ResetColorsChunk);
        }

        /// <summary>
        /// Adds a control to reset the foreground color.
        /// </summary>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendResetForegroundColor()
        {
            return AppendControl(ResetForegroundColorChunk);
        }

        /// <summary>
        /// Adds a control to reset the background color.
        /// </summary>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendResetBackgroundColor()
        {
            return AppendControl(ResetBackgroundColorChunk);
        }

        /// <summary>
        /// Adds a control to set the foreground color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendForegroundColor(ConsoleColor color)
        {
            Color c = color.ToColor();
            return AppendControl(FormatChunk.CreateControl(ForegroundColorTag, 0, c.GetName(), c));
        }

        /// <summary>
        /// Adds a control to set the foreground color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendForegroundColor(Color color)
        {
            return AppendControl(FormatChunk.CreateControl(ForegroundColorTag, 0, color.GetName(), color));
        }

        /// <summary>
        /// Adds a control to set the foreground color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendForegroundColor([CanBeNull] string color)
        {
            return string.IsNullOrWhiteSpace(color) ? this : AppendControl(FormatChunk.CreateControl(ForegroundColorTag, 0, color));
        }

        /// <summary>
        /// Adds a control to set the background color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendBackgroundColor(ConsoleColor color)
        {
            Color c = color.ToColor();
            return AppendControl(FormatChunk.CreateControl(BackgroundColorTag, 0, c.GetName(), c));
        }

        /// <summary>
        /// Adds a control to set the background color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendBackgroundColor(Color color)
        {
            return AppendControl(FormatChunk.CreateControl(BackgroundColorTag, 0, color.GetName(), color));
        }

        /// <summary>
        /// Adds a control to set the console's background color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendBackgroundColor([CanBeNull] string color)
        {
            return string.IsNullOrWhiteSpace(color) ? this : AppendControl(FormatChunk.CreateControl(BackgroundColorTag, 0, color));
        }
        #endregion

        #region Layout Control
        /// <summary>
        /// The layout control tag.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public const string LayoutTag = "!layout";

        /// <summary>
        /// The reset layout chunk.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly FormatChunk ResetLayoutChunk = FormatChunk.CreateControl(LayoutTag);

        /// <summary>
        /// Resets the layout.
        /// </summary>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendResetLayout()
        {
            Contract.Requires(!IsReadOnly);
            return AppendControl(ResetLayoutChunk);
        }

        /// <summary>
        /// Sets the layout (if outputting to a layout writer).
        /// </summary>
        /// <param name="layout">The layout.</param>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendLayout([CanBeNull] Layout layout)
        {
            Contract.Requires(!IsReadOnly);
            return layout == null
                ? this
                : AppendControl(FormatChunk.CreateControl(LayoutTag, 0, layout.ToString("f"), layout));
        }

        /// <summary>
        /// Sets the layout (if outputting to a layout writer).
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="indentSize">Size of the indent.</param>
        /// <param name="rightMarginSize">Size of the right margin.</param>
        /// <param name="indentChar">The indent character.</param>
        /// <param name="firstLineIndentSize">First size of the line indent.</param>
        /// <param name="tabStops">The tab stops.</param>
        /// <param name="tabSize">Size of the tab.</param>
        /// <param name="tabChar">The tab character.</param>
        /// <param name="alignment">The alignment.</param>
        /// <param name="splitLength">if set to <see langword="true" /> then words will split across lines.</param>
        /// <param name="hyphenate">if set to <see langword="true" /> [hyphenate].</param>
        /// <param name="hyphenChar">The hyphenation character.</param>
        /// <param name="wrapMode">The line wrap mode.</param>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendLayout(
            Optional<int> width = default(Optional<int>),
            Optional<int> indentSize = default(Optional<int>),
            Optional<int> rightMarginSize = default(Optional<int>),
            Optional<char> indentChar = default(Optional<char>),
            Optional<int> firstLineIndentSize = default(Optional<int>),
            Optional<IEnumerable<int>> tabStops = default(Optional<IEnumerable<int>>),
            Optional<byte> tabSize = default(Optional<byte>),
            Optional<char> tabChar = default(Optional<char>),
            Optional<Alignment> alignment = default(Optional<Alignment>),
            Optional<byte> splitLength = default(Optional<byte>),
            Optional<bool> hyphenate = default(Optional<bool>),
            Optional<char> hyphenChar = default(Optional<char>),
            Optional<LayoutWrapMode> wrapMode = default(Optional<LayoutWrapMode>))
        {
            Contract.Requires(!IsReadOnly);
            Layout layout = new Layout(
                width,
                indentSize,
                rightMarginSize,
                indentChar,
                firstLineIndentSize,
                tabStops,
                tabSize,
                tabChar,
                alignment,
                splitLength,
                hyphenate,
                hyphenChar,
                wrapMode);
            return Append(FormatChunk.CreateControl(LayoutTag, 0, layout.ToString("f"), layout));
        }
        #endregion

        #region Conversions
        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="FormatBuilder"/>.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>The result of the conversion.</returns>
        [StringFormatMethod("format")]
        public static implicit operator FormatBuilder(string format)
        {
            return format != null
                ? new FormatBuilder(format)
                : null;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="FormatBuilder"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator string(FormatBuilder format)
        {
            return format != null
                ? format.ToString()
                : null;
        }
        #endregion

        #region Equality
        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><see langword="true" /> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <see langword="false" />.</returns>
        public override bool Equals([CanBeNull]object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is FormatBuilder &&
                Equals((FormatBuilder)obj);
        }
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals([CanBeNull]FormatBuilder other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _isReadOnly.Equals(other._isReadOnly) &&
                   _isLayoutRequired.Equals(other._isLayoutRequired) &&
                   InitialLayout.Equals(other.InitialLayout) &&
                   string.Equals(ToString("F"), other.ToString("F"));
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return ToString("F").GetHashCode();
        }

        /// <summary>
        /// Implements the ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(FormatBuilder left, FormatBuilder right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Implements the !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(FormatBuilder left, FormatBuilder right)
        {
            return !Equals(left, right);
        }
        #endregion
    }
}