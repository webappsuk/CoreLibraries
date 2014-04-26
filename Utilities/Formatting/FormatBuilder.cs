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
using System.Drawing;
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
    public sealed partial class FormatBuilder : IEnumerable<FormatChunk>, IFormattable
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
        /// Gets the initial layout to use when resetting the layout.
        /// </summary>
        /// <value>
        /// The initial layout.
        /// </value>
        [NotNull]
        [PublicAPI]
        public Layout InitialLayout { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatBuilder" /> class.
        /// </summary>
        public FormatBuilder()
        {
            InitialLayout = Layout.Default;
            Contract.Assert(InitialLayout.IsFull);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatBuilder" /> class.
        /// </summary>
        /// <param name="layout">The layout.</param>
        public FormatBuilder([CanBeNull] Layout layout)
        {
            InitialLayout = Layout.Default.Apply(layout);
            Contract.Assert(InitialLayout.IsFull);
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
        /// <param name="splitWords">The split words.</param>
        /// <param name="hyphenate">The hyphenate.</param>
        /// <param name="hyphenChar">The hyphen character.</param>
        /// <param name="wrapMode">The wrap mode.</param>
        /// <param name="?">The ?.</param>
        public FormatBuilder(
            Optional<ushort> width = default(Optional<ushort>),
            Optional<byte> indentSize = default(Optional<byte>),
            Optional<byte> rightMarginSize = default(Optional<byte>),
            Optional<char> indentChar = default(Optional<char>),
            Optional<ushort> firstLineIndentSize = default(Optional<ushort>),
            Optional<IEnumerable<ushort>> tabStops = default(Optional<IEnumerable<ushort>>),
            Optional<byte> tabSize = default(Optional<byte>),
            Optional<char> tabChar = default(Optional<char>),
            Optional<Alignment> alignment = default(Optional<Alignment>),
            Optional<bool> splitWords = default(Optional<bool>),
            Optional<bool> hyphenate = default(Optional<bool>),
            Optional<char> hyphenChar = default(Optional<char>),
            Optional<LayoutWrapMode> wrapMode = default(Optional<LayoutWrapMode>))
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
                splitWords,
                hyphenate,
                hyphenChar,
                wrapMode);
            Contract.Assert(InitialLayout.IsFull);
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
        /// <param name="makeReadonly">If set to <see langword="true" />, the returned builder will be readonly.</param>
        /// <returns>
        /// A shallow copy of this builder.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Clone(bool makeReadonly = false)
        {
            Contract.Ensures(
                Contract.Result<FormatBuilder>().GetType() == GetType(),
                "All classes derived from FormatBuilder should overload this method and return a builder of their own type");
            Contract.Ensures(
                !makeReadonly || Contract.Result<FormatBuilder>().IsReadonly,
                "Returned builder should be readonly if makeReadonly is true");

            if (IsReadonly)
                return this;

            FormatBuilder formatBuilder = new FormatBuilder(InitialLayout);
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
            if (!_isReadonly &&
                value != null)
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
                _chunks.AddRange(
                    args != null && args.Length > 0
                        ? Resolve(format.FormatChunks(), args)
                        : format.FormatChunks());
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
                _chunks.AddRange(
                    values != null && values.Count > 0
                        ? Resolve(format.FormatChunks(), values)
                        : format.FormatChunks());
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
            [CanBeNull] [InstantHandle] Func<FormatChunk, Optional<object>> resolver)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            if (!string.IsNullOrEmpty(format))
                _chunks.AddRange(
                    resolver != null
                        ? Resolve(format.FormatChunks(), resolver)
                        : format.FormatChunks());
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
                _chunks.AddRange(
                    args != null && args.Length > 0
                        ? Resolve(format.FormatChunks(), args)
                        : format.FormatChunks());
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
                _chunks.AddRange(
                    values != null && values.Count > 0
                        ? Resolve(format.FormatChunks(), values)
                        : format.FormatChunks());
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
            [CanBeNull] [InstantHandle] Func<FormatChunk, Optional<object>> resolver)
        {
            Contract.Requires(!IsReadonly);
            if (_isReadonly) return this;
            if (!string.IsNullOrEmpty(format))
                _chunks.AddRange(
                    resolver != null
                        ? Resolve(format.FormatChunks(), resolver)
                        : format.FormatChunks());
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
        private static IEnumerable<FormatChunk> Resolve([NotNull] FormatChunk chunk)
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
        private static IEnumerable<FormatChunk> Resolve(
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

                if (!found ||
                    (chunk.Value == value))
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
        private static IEnumerable<FormatChunk> Resolve(
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

                if (!found ||
                    (chunk.Value == value))
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
        private static IEnumerable<FormatChunk> Resolve(
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
        /// <param name="position">The position.</param>
        /// <param name="values">The values.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        [NotNull]
        [PublicAPI]
        public string ToString(ref ushort position, [CanBeNull] params object[] values)
        {
            using (StringWriter writer = new StringWriter())
            {
                position = WriteTo(writer, position, null, values);
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
        /// <param name="position">The position.</param>
        /// <param name="values">The values.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        [NotNull]
        [PublicAPI]
        public string ToString(ref ushort position, [CanBeNull] IReadOnlyDictionary<string, object> values)
        {
            using (StringWriter writer = new StringWriter())
            {
                position = WriteTo(writer, position, null, values);
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
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="resolver">The values.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        [NotNull]
        [PublicAPI]
        public string ToString(
            ref ushort position,
            [CanBeNull] [InstantHandle] Func<FormatChunk, Optional<object>> resolver)
        {
            using (StringWriter writer = new StringWriter())
            {
                position = WriteTo(writer, position, null, resolver);
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
        /// <param name="position">The position.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="values">The values.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        [NotNull]
        [PublicAPI]
        public string ToString(
            ref ushort position,
            [CanBeNull] IFormatProvider formatProvider,
            [CanBeNull] params object[] values)
        {
            using (StringWriter writer = new StringWriter(formatProvider))
            {
                position = WriteTo(writer, position, null, values);
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
        /// <param name="position">The position.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="values">The values.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        [NotNull]
        [PublicAPI]
        public string ToString(
            ref ushort position,
            [CanBeNull] IFormatProvider formatProvider,
            [CanBeNull] IReadOnlyDictionary<string, object> values)
        {
            using (StringWriter writer = new StringWriter(formatProvider))
            {
                position = WriteTo(writer, position, null, values);
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
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="resolver">The values.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        [NotNull]
        [PublicAPI]
        public string ToString(
            ref ushort position,
            [CanBeNull] IFormatProvider formatProvider,
            [CanBeNull] [InstantHandle] Func<FormatChunk, Optional<object>> resolver)
        {
            using (StringWriter writer = new StringWriter(formatProvider))
            {
                position = WriteTo(writer, position, null, resolver);
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
        /// <param name="position">The position.</param>
        /// <param name="format">The format.
        /// <list type="table"><listheader><term>Format string</term><description>Description</description></listheader><item><term>G/g/null</term><description>Any unresolved fill points will have their tags output. Control chunks are ignored.</description></item><item><term>F/f</term><description>All control and fill point chunks will have their tags output.</description></item><item><term>S/s</term><description>Any unresolved fill points will be treated as an empty string. Control chunks are ignored.</description></item></list></param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        [NotNull]
        [PublicAPI]
        public string ToString(
            ref ushort position,
            [CanBeNull] string format,
            [CanBeNull] IFormatProvider formatProvider = null)
        {
            using (StringWriter writer = new StringWriter(formatProvider))
            {
                position = WriteTo(writer, position, format);
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
        /// <param name="position">The position.</param>
        /// <param name="format">The format.
        /// <list type="table"><listheader><term>Format string</term><description>Description</description></listheader><item><term>G/g/null</term><description>Any unresolved fill points will have their tags output. Control chunks are ignored.</description></item><item><term>F/f</term><description>All control and fill point chunks will have their tags output.</description></item><item><term>S/s</term><description>Any unresolved fill points will be treated as an empty string. Control chunks are ignored.</description></item></list></param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="values">The values.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        [NotNull]
        [PublicAPI]
        public string ToString(
            ref ushort position,
            [CanBeNull] string format,
            [CanBeNull] IFormatProvider formatProvider,
            [CanBeNull] params object[] values)
        {
            using (StringWriter writer = new StringWriter(formatProvider))
            {
                position = WriteTo(writer, position, format, values);
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
        /// <param name="position">The position.</param>
        /// <param name="format">The format.
        /// <list type="table"><listheader><term>Format string</term><description>Description</description></listheader><item><term>G/g/null</term><description>Any unresolved fill points will have their tags output. Control chunks are ignored.</description></item><item><term>F/f</term><description>All control and fill point chunks will have their tags output.</description></item><item><term>S/s</term><description>Any unresolved fill points will be treated as an empty string. Control chunks are ignored.</description></item></list></param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="values">The values.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        [NotNull]
        [PublicAPI]
        public string ToString(
            ref ushort position,
            [CanBeNull] string format,
            [CanBeNull] IFormatProvider formatProvider,
            [CanBeNull] IReadOnlyDictionary<string, object> values)
        {
            using (StringWriter writer = new StringWriter(formatProvider))
            {
                position = WriteTo(writer, position, format, values);
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

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="format">The format.
        /// <list type="table"><listheader><term>Format string</term><description>Description</description></listheader><item><term>G/g/null</term><description>Any unresolved fill points will have their tags output. Control chunks are ignored.</description></item><item><term>F/f</term><description>All control and fill point chunks will have their tags output.</description></item><item><term>S/s</term><description>Any unresolved fill points will be treated as an empty string. Control chunks are ignored.</description></item></list></param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        [NotNull]
        [PublicAPI]
        public string ToString(
            ref ushort position,
            [CanBeNull] string format,
            [CanBeNull] IFormatProvider formatProvider,
            [CanBeNull] Func<FormatChunk, Optional<object>> resolver)
        {
            using (StringWriter writer = new StringWriter(formatProvider))
            {
                position = WriteTo(writer, position, format, resolver);
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
            WriteTo(_chunks, ConsoleTextWriter.Default, "G", 0);
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
            WriteTo(
                values != null && values.Length > 0
                    ? Resolve(_chunks, values)
                    : _chunks,
                ConsoleTextWriter.Default,
                format,
                0);
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
            WriteTo(
                values != null && values.Count > 0
                    ? Resolve(_chunks, values)
                    : _chunks,
                ConsoleTextWriter.Default,
                format,
                0);
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
            WriteTo(
                resolver != null
                    ? Resolve(_chunks, resolver)
                    : _chunks,
                ConsoleTextWriter.Default,
                format,
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
            WriteTo(_chunks, TraceTextWriter.Default, "G", 0);
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
            WriteTo(
                values != null && values.Length > 0
                    ? Resolve(_chunks, values)
                    : _chunks,
                TraceTextWriter.Default,
                format,
                0);
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
            WriteTo(
                values != null && values.Count > 0
                    ? Resolve(_chunks, values)
                    : _chunks,
                TraceTextWriter.Default,
                format,
                0);
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
            WriteTo(
                resolver != null
                    ? Resolve(_chunks, resolver)
                    : _chunks,
                TraceTextWriter.Default,
                format,
                0);
        }
        #endregion

        #region WriteTo Overloads
        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="position">The start position.</param>
        /// <returns>The end position.</returns>
        [PublicAPI]
        public ushort WriteTo([CanBeNull] TextWriter writer, ushort position = 0)
        {
            return writer == null ? position : WriteTo(_chunks, writer, "G", position);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        /// <returns>The end position.</returns>
        [PublicAPI]
        public ushort WriteTo(
            [CanBeNull] TextWriter writer,
            [CanBeNull] string format,
            [CanBeNull] params object[] values)
        {
            if (writer == null) return 0;
            if (format == null)
                format = "G";
            return WriteTo(
                values != null && values.Length > 0
                    ? Resolve(_chunks, values)
                    : _chunks,
                writer,
                format,
                0);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="position">The start position.</param>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        /// <returns>The end position.</returns>
        [PublicAPI]
        public ushort WriteTo(
            [CanBeNull] TextWriter writer,
            ushort position,
            [CanBeNull] string format,
            [CanBeNull] params object[] values)
        {
            if (writer == null) return position;
            if (format == null)
                format = "G";
            return WriteTo(
                values != null && values.Length > 0
                    ? Resolve(_chunks, values)
                    : _chunks,
                writer,
                format,
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
        public ushort WriteTo(
            [CanBeNull] TextWriter writer,
            [CanBeNull] string format,
            [CanBeNull] IReadOnlyDictionary<string, object> values)
        {
            if (writer == null) return 0;
            if (format == null)
                format = "G";
            return WriteTo(
                values != null && values.Count > 0
                    ? Resolve(_chunks, values)
                    : _chunks,
                writer,
                format,
                0);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="position">The start position.</param>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        /// <returns>The end position.</returns>
        [PublicAPI]
        public ushort WriteTo(
            [CanBeNull] TextWriter writer,
            ushort position,
            [CanBeNull] string format,
            [CanBeNull] IReadOnlyDictionary<string, object> values)
        {
            if (writer == null) return position;
            if (format == null)
                format = "G";
            return WriteTo(
                values != null && values.Count > 0
                    ? Resolve(_chunks, values)
                    : _chunks,
                writer,
                format,
                position);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns>The end position.</returns>
        [PublicAPI]
        public ushort WriteTo(
            [CanBeNull] TextWriter writer,
            [CanBeNull] string format,
            [CanBeNull] [InstantHandle] Func<FormatChunk, Optional<object>> resolver)
        {
            if (writer == null) return 0;
            if (format == null)
                format = "G";
            return WriteTo(
                resolver != null
                    ? Resolve(_chunks, resolver)
                    : _chunks,
                writer,
                format,
                0);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="position">The start position.</param>
        /// <param name="format">The format.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns>The end position.</returns>
        [PublicAPI]
        public ushort WriteTo(
            [CanBeNull] TextWriter writer,
            ushort position,
            [CanBeNull] string format,
            [CanBeNull] [InstantHandle] Func<FormatChunk, Optional<object>> resolver)
        {
            if (writer == null) return position;
            if (format == null)
                format = "G";
            return WriteTo(
                resolver != null
                    ? Resolve(_chunks, resolver)
                    : _chunks,
                writer,
                format,
                position);
        }
        #endregion

        #region WriteToAsync Overloads
        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="position">The start position.</param>
        /// <returns>And awaitable <see cref="Task"/> that returns the end position.</returns>
        [NotNull]
        [PublicAPI]
        public Task<ushort> WriteToAsync([CanBeNull] TextWriter writer, ushort position = 0)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            return writer == null
                ? Task.FromResult(position)
                : WriteToAsync(_chunks, writer, "G", position);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        /// <returns>And awaitable <see cref="Task" /> that returns the end position.</returns>
        [NotNull]
        [PublicAPI]
        public Task<ushort> WriteToAsync(
            [CanBeNull] TextWriter writer,
            [CanBeNull] string format,
            [CanBeNull] params object[] values)
        {
            if (writer == null) return TaskResult<ushort>.Default;
            if (format == null)
                format = "G";
            return WriteToAsync(
                values != null && values.Length > 0
                    ? Resolve(_chunks, values)
                    : _chunks,
                writer,
                format,
                0);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="position">The start position.</param>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        /// <returns>And awaitable <see cref="Task"/> that returns the end position.</returns>
        [NotNull]
        [PublicAPI]
        public Task<ushort> WriteToAsync(
            [CanBeNull] TextWriter writer,
            ushort position,
            [CanBeNull] string format,
            [CanBeNull] params object[] values)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            if (writer == null) return Task.FromResult(position);
            if (format == null)
                format = "G";
            return WriteToAsync(
                values != null && values.Length > 0
                    ? Resolve(_chunks, values)
                    : _chunks,
                writer,
                format,
                position);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        /// <returns>And awaitable <see cref="Task" /> that returns the end position.</returns>
        [NotNull]
        [PublicAPI]
        public Task<ushort> WriteToAsync(
            [CanBeNull] TextWriter writer,
            [CanBeNull] string format,
            [CanBeNull] IReadOnlyDictionary<string, object> values)
        {
            if (writer == null) return TaskResult<ushort>.Default;
            if (format == null)
                format = "G";
            return WriteToAsync(
                values != null && values.Count > 0
                    ? Resolve(_chunks, values)
                    : _chunks,
                writer,
                format,
                0);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="position">The start position.</param>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        /// <returns>And awaitable <see cref="Task" /> that returns the end position.</returns>
        [NotNull]
        [PublicAPI]
        public Task<ushort> WriteToAsync(
            [CanBeNull] TextWriter writer,
            ushort position,
            [CanBeNull] string format,
            [CanBeNull] IReadOnlyDictionary<string, object> values)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            if (writer == null) return Task.FromResult(position);
            if (format == null)
                format = "G";
            return WriteToAsync(
                values != null && values.Count > 0
                    ? Resolve(_chunks, values)
                    : _chunks,
                writer,
                format,
                position);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns>And awaitable <see cref="Task" /> that returns the end position.</returns>
        [NotNull]
        [PublicAPI]
        public Task<ushort> WriteToAsync(
            [CanBeNull] TextWriter writer,
            [CanBeNull] string format,
            [CanBeNull] [InstantHandle] Func<FormatChunk, Optional<object>> resolver)
        {
            if (writer == null) return TaskResult<ushort>.Default;
            if (format == null)
                format = "G";
            return WriteToAsync(
                resolver != null
                    ? Resolve(_chunks, resolver)
                    : _chunks,
                writer,
                format,
                0);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="position">The start position.</param>
        /// <param name="format">The format.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns>And awaitable <see cref="Task" /> that returns the end position.</returns>
        [NotNull]
        [PublicAPI]
        public Task<ushort> WriteToAsync(
            [CanBeNull] TextWriter writer,
            ushort position,
            [CanBeNull] string format,
            [CanBeNull] [InstantHandle] Func<FormatChunk, Optional<object>> resolver)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            if (writer == null) return Task.FromResult(position);
            if (format == null)
                format = "G";
            return WriteToAsync(
                resolver != null
                    ? Resolve(_chunks, resolver)
                    : _chunks,
                writer,
                format,
                position);
        }
        #endregion

        #region Synchronization wrappers
        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="chunks">The chunks.</param>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <param name="position">The position.</param>
        /// <returns>System.UInt16.</returns>
        [PublicAPI]
        private ushort WriteTo(
            [NotNull] [InstantHandle] IEnumerable<FormatChunk> chunks,
            [NotNull] TextWriter writer,
            [NotNull] string format,
            ushort position)
        {
            Contract.Requires(chunks != null);
            Contract.Requires(writer != null);
            Contract.Requires(format != null);
            ISerialTextWriter serialTextWriter = writer as ISerialTextWriter;
            return serialTextWriter == null
                ? DoWrite(chunks, writer, format, position)
                : serialTextWriter.Context.Invoke(() => DoWrite(chunks, writer, format, position));
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="chunks">The chunks.</param>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <param name="position">The position.</param>
        /// <returns>Task&lt;System.UInt16&gt;.</returns>
        [NotNull]
        [PublicAPI]
        private async Task<ushort> WriteToAsync(
            [NotNull] [InstantHandle] IEnumerable<FormatChunk> chunks,
            [NotNull] TextWriter writer,
            [NotNull] string format,
            ushort position)
        {
            Contract.Requires(chunks != null);
            Contract.Requires(writer != null);
            Contract.Requires(format != null);
            ISerialTextWriter serialTextWriter = writer as ISerialTextWriter;
            return serialTextWriter == null
                ? await DoWriteAsync(chunks, writer, format, position)
                // Note we run synchronous code if we're synchronized!
                : serialTextWriter.Context.Invoke(() => DoWrite(chunks, writer, format, position));
        }
        #endregion

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="chunks">The chunks.</param>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <param name="position">The position.</param>
        /// <returns>System.UInt16.</returns>
        [PublicAPI]
        private ushort DoWrite(
            [NotNull] [InstantHandle] IEnumerable<FormatChunk> chunks,
            [NotNull] TextWriter writer,
            [NotNull] string format,
            ushort position)
        {
            Contract.Requires(chunks != null);
            Contract.Requires(writer != null);
            Contract.Requires(format != null);
            bool writeTags = string.Equals(format, "f", StringComparison.InvariantCultureIgnoreCase);
            IFormatProvider formatProvider = writer.FormatProvider;
            IControllableTextWriter controller = writer as IControllableTextWriter;
            ILayoutTextWriter layoutWriter = writer as ILayoutTextWriter;

            ushort writerWidth;
            bool autoWraps;
            if (layoutWriter != null)
            {
                // Get current state from the writer.
                position = layoutWriter.Position;
                writerWidth = layoutWriter.Width;
                autoWraps = layoutWriter.AutoWraps;
            }
            else
            {
                writerWidth = ushort.MaxValue;
                autoWraps = false;
            }

            // We try to output the builder in one go to prevent interleaving, however we split on control codes.
            StringBuilder sb = new StringBuilder();
            foreach (FormatChunk chunk in Align(
                GetLines(
                    GetLineChunks(chunks, format, writer.FormatProvider),
                    position,
                    writerWidth),
                writerWidth,
                autoWraps,
                ref position))
                // ReSharper disable once PossibleNullReferenceException
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

            if (sb.Length > 0)
                writer.Write(sb.ToString());

            if (layoutWriter != null)
                // Get current position from writer.
                layoutWriter.Position = position;

            return position;
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="chunks">The chunks.</param>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <param name="position">The position.</param>
        /// <returns>Task&lt;System.UInt16&gt;.</returns>
        [NotNull]
        [PublicAPI]
        private async Task<ushort> DoWriteAsync(
            [NotNull] [InstantHandle] IEnumerable<FormatChunk> chunks,
            [NotNull] TextWriter writer,
            [NotNull] string format,
            ushort position)
        {
            Contract.Requires(chunks != null);
            Contract.Requires(writer != null);
            Contract.Requires(format != null);
            bool writeTags = string.Equals(format, "f", StringComparison.InvariantCultureIgnoreCase);
            IFormatProvider formatProvider = writer.FormatProvider;
            IControllableTextWriter controller = writer as IControllableTextWriter;
            ILayoutTextWriter layoutWriter = writer as ILayoutTextWriter;

            ushort writerWidth;
            bool autoWraps;
            if (layoutWriter != null)
            {
                // Get current state from the writer.
                position = layoutWriter.Position;
                writerWidth = layoutWriter.Width;
                autoWraps = layoutWriter.AutoWraps;
            }
            else
            {
                writerWidth = ushort.MaxValue;
                autoWraps = false;
            }

            // We try to output the builder in one go to prevent interleaving, however we split on control codes.
            StringBuilder sb = new StringBuilder();
            foreach (FormatChunk chunk in Align(
                GetLines(
                    GetLineChunks(chunks, format, writer.FormatProvider),
                    position,
                    writerWidth),
                writerWidth,
                autoWraps,
                ref position))
                // ReSharper disable once PossibleNullReferenceException
                if (chunk.IsControl &&
                    !writeTags)
                {
                    if (controller == null) continue;

                    // If we have anything to write out, do so before calling the controller.
                    if (sb.Length > 0)
                    {
                        // ReSharper disable PossibleNullReferenceException
                        await writer.WriteAsync(sb.ToString());
                        // ReSharper restore PossibleNullReferenceException
                        sb.Clear();
                    }

                    controller.OnControlChunk(chunk, format, formatProvider);
                }
                else
                    sb.Append(chunk.ToString(format, formatProvider));

            if (sb.Length > 0)
                // ReSharper disable once PossibleNullReferenceException
                await writer.WriteAsync(sb.ToString());

            if (layoutWriter != null)
                // Get current position from writer.
                layoutWriter.Position = position;

            return position;
        }

        /// <summary>
        /// Gets the line chunks from a set of chunks.
        /// </summary>
        /// <param name="chunks">The chunks.</param>
        /// <param name="format">The format.</param>
        /// <param name="provider">The format provider.</param>
        /// <returns>An enumeration of chunks.</returns>
        [NotNull]
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
                    if (!char.IsWhiteSpace(ch))
                    {
                        word.Append(ch);
                        continue;
                    }
                    if (word.Length > 0)
                    {
                        words.Add(word.ToString());
                        word.Clear();
                    }

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
        /// <param name="position">The position.</param>
        /// <param name="writerWidth">The writer's width.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        [NotNull]
        private IEnumerable<Line> GetLines(
            [NotNull] Tuple<IEnumerable<string>, IEnumerable<FormatChunk>> chunks,
            ushort position,
            ushort writerWidth)
        {
            Contract.Requires(chunks != null);
            Contract.Requires(chunks.Item1 != null);
            Contract.Requires(chunks.Item2 != null);

            // Only grab the layout at the start of each line.
            Layout nextLayout = InitialLayout;
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

            bool splitWords = layout.SplitWords.Value;
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
                    if (nextLayout.Width.Value >= writerWidth)
                        nextLayout = nextLayout.Apply(writerWidth);

                    layout = nextLayout;
                    line = new Line(
                        layout,
                        layout.Alignment.Value,
                        firstLine ? layout.FirstLineIndentSize.Value : layout.IndentSize.Value,
                        layout.Width.Value - layout.RightMarginSize.Value,
                        firstLine);
                    firstLine = false;
                    splitWords = layout.SplitWords.Value;
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
                            Layout newLayout = controlChunk.Value as Layout;

                            if (newLayout != null)
                                nextLayout = ReferenceEquals(newLayout, Layout.Default)
                                    ? InitialLayout
                                    : nextLayout.Apply(newLayout);
                            else if (string.IsNullOrEmpty(controlChunk.Format))
                                nextLayout = InitialLayout;
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
                            splitWords = layout.SplitWords.Value;
                            hyphenate = layout.Hyphenate.Value ? 1 : 0;
                        }
                        else
                            line.AddControl(controlChunk);
                    } while (true);

                char c = word[0];

                // Check if we're at the start of a line.
                bool split = splitWords;
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
                    split = true;
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

                // Check for tab
                if (c == '\t')
                {
                    if (remaining < 1)
                    {
                        // Process tab on a new line, as we're at the end of this one.
                        newLine = true;
                        continue;
                    }

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
                    {
                        word = null;
                        continue;
                    }
                }

                // The word is too long to fit on the current line.
                if (split &&
                    (remaining > hyphenate))
                {
                    // Split the current word to fill remaining space
                    int splitPoint = remaining - hyphenate;
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
        /// <param name="writerWidth">Width of the writer.</param>
        /// <param name="autoWraps">if set to <see langword="true" /> then the writer automatically wraps on reaching width.</param>
        /// <param name="position">The position.</param>
        /// <returns>An enumeration of terminated lines, laid out for writing.</returns>
        [NotNull]
        private IEnumerable<FormatChunk> Align(
            [NotNull] [InstantHandle] IEnumerable<Line> lines,
            ushort writerWidth,
            bool autoWraps,
            ref ushort position)
        {
            Contract.Requires(lines != null);
            StringBuilder lb = new StringBuilder(InitialLayout.Width.Value);
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
                        if (remaining > 0)
                        {
                            decimal space = (decimal) (line.End - line.LastWordLength - line.Start) / remaining;
                            int o = (int) Math.Round(space / 2);
                            spacers = new Queue<int>(Enumerable.Range(0, remaining).Select(r => o + (int) (space * r)));
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
                    if (string.IsNullOrEmpty(chunk))
                    {
                        // We got a control chunk, so need to split line
                        if (lb.Length > 0)
                        {
                            chunks.Add(FormatChunk.Create(lb.ToString()));
                            lb.Clear();
                        }
                        controlEnumerator.MoveNext();
                        Contract.Assert(controlEnumerator.Current != null);
                        chunks.Add(controlEnumerator.Current);
                        continue;
                    }

                    lb.Append(chunk);
                    p += chunk.Length;

                    // Check if we have to add justification spaces
                    if (spacers == null) continue;

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

                // Add any remaining spacers
                if ((spacers != null) &&
                    (spacers.Count > 0))
                {
                    lb.Append(indentChar, spacers.Count);
                    p += spacers.Count;
                }

                // Calculate our finish position
                int np = p + indent;
                position = np < ushort.MaxValue ? (ushort) np : ushort.MaxValue;

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
                                (writerWidth < ushort.MaxValue ? writerWidth : line.Layout.Width.Value) - position);
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
            return AppendControl(FormatChunk.CreateControl(ForegroundColorTag, null, c.GetName(), c));
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
            return AppendControl(FormatChunk.CreateControl(ForegroundColorTag, null, color.GetName(), color));
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
            if (string.IsNullOrWhiteSpace(color)) return this;
            Optional<Color> c = ColorHelper.GetColor(color);
            return !c.IsAssigned
                ? this
                : AppendControl(FormatChunk.CreateControl(ForegroundColorTag, null, c.Value.GetName(), c.Value));
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
            return AppendControl(FormatChunk.CreateControl(BackgroundColorTag, null, c.GetName(), c));
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
            return AppendControl(FormatChunk.CreateControl(BackgroundColorTag, null, color.GetName(), color));
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
            if (string.IsNullOrWhiteSpace(color)) return this;
            Optional<Color> c = ColorHelper.GetColor(color);
            return !c.IsAssigned
                ? this
                : AppendControl(FormatChunk.CreateControl(BackgroundColorTag, null, c.Value.GetName(), c.Value));
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
            Contract.Requires(!IsReadonly);
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
            Contract.Requires(!IsReadonly);
            return AppendControl(FormatChunk.CreateControl(LayoutTag, null, layout.ToString("f"), layout));
        }

        /// <summary>
        /// Sets the layout (if outputting to a layout writer).
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="width">The width.</param>
        /// <param name="indentSize">Size of the indent.</param>
        /// <param name="rightMarginSize">Size of the right margin.</param>
        /// <param name="indentChar">The indent character.</param>
        /// <param name="firstLineIndentSize">First size of the line indent.</param>
        /// <param name="tabStops">The tab stops.</param>
        /// <param name="tabSize">Size of the tab.</param>
        /// <param name="tabChar">The tab character.</param>
        /// <param name="alignment">The alignment.</param>
        /// <param name="splitWords">if set to <see langword="true" /> then words will split across lines.</param>
        /// <param name="hyphenate">if set to <see langword="true" /> [hyphenate].</param>
        /// <param name="hyphenChar">The hyphenation character.</param>
        /// <param name="wrapMode">The line wrap mode.</param>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendLayout(
            Optional<ushort> width = default(Optional<ushort>),
            Optional<byte> indentSize = default(Optional<byte>),
            Optional<byte> rightMarginSize = default(Optional<byte>),
            Optional<char> indentChar = default(Optional<char>),
            Optional<ushort> firstLineIndentSize = default(Optional<ushort>),
            Optional<IEnumerable<ushort>> tabStops = default(Optional<IEnumerable<ushort>>),
            Optional<byte> tabSize = default(Optional<byte>),
            Optional<char> tabChar = default(Optional<char>),
            Optional<Alignment> alignment = default(Optional<Alignment>),
            Optional<bool> splitWords = default(Optional<bool>),
            Optional<bool> hyphenate = default(Optional<bool>),
            Optional<char> hyphenChar = default(Optional<char>),
            Optional<LayoutWrapMode> wrapMode = default(Optional<LayoutWrapMode>))
        {
            Contract.Requires(!IsReadonly);
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
                splitWords,
                hyphenate,
                hyphenChar,
                wrapMode);
            return Append(FormatChunk.CreateControl(LayoutTag, null, layout.ToString("f"), layout));
        }
        #endregion
    }
}