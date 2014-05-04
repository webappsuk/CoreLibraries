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
    public sealed partial class FormatBuilder : IFormattable, IWriteable, IEquatable<FormatBuilder>,
        IEnumerable<FormatChunk>
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

        /// <summary>
        /// The root chunk.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public readonly FormatChunk RootChunk = new FormatChunk(null);

        /// <summary>
        /// Whether this builder is read only
        /// </summary>
        private bool _isReadOnly;

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
            Contract.Assert(InitialLayout.IsFull);
            if (!string.IsNullOrEmpty(format))
                AppendFormat(format);
            _isReadOnly = isReadOnly;
        }
        #endregion

        /// <summary>
        /// Clears this instance.
        /// </summary>
        [PublicAPI]
        [NotNull]
        public FormatBuilder Clear()
        {
            RootChunk.ChildrenInternal = null;
            return this;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><see langword="true" /> if this instance is empty; otherwise, <see langword="false" />.</value>
        [PublicAPI]
        public bool IsEmpty
        {
            get { return RootChunk.ChildrenInternal == null || RootChunk.ChildrenInternal.Count < 1; }
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

            FormatBuilder formatBuilder = new FormatBuilder(InitialLayout);
            if (readOnly)
            {
                formatBuilder.RootChunk.ChildrenInternal = RootChunk.ChildrenInternal;
                formatBuilder.MakeReadOnly();
            }
            else if (RootChunk.ChildrenInternal != null &&
                     RootChunk.ChildrenInternal.Count > 0)
            {
                Stack<FormatChunk, IEnumerable<FormatChunk>> stack = new Stack<FormatChunk, IEnumerable<FormatChunk>>();
                stack.Push(formatBuilder.RootChunk, RootChunk.ChildrenInternal.ToArray());

                while (stack.Count > 0)
                {
                    FormatChunk currParent;
                    IEnumerable<FormatChunk> chunks;
                    stack.Pop(out currParent, out chunks);

                    Contract.Assert(currParent != null);
                    Contract.Assert(chunks != null);

                    // ReSharper disable once PossibleNullReferenceException
                    currParent.ChildrenInternal = new List<FormatChunk>();

                    // Adds each chunk to the current parent
                    // ReSharper disable once PossibleNullReferenceException
                    foreach (FormatChunk chunk in chunks)
                    {
                        Contract.Assert(chunk != null);
                        // ReSharper disable PossibleNullReferenceException
                        FormatChunk newChunk = chunk.Clone();
                        // ReSharper restore PossibleNullReferenceException

                        currParent.ChildrenInternal.Add(newChunk);

                        // If the chunk has any children they need to be added to the new chunk
                        if (chunk.ChildrenInternal != null &&
                            chunk.ChildrenInternal.Count >= 1)
                            stack.Push(newChunk, chunk.ChildrenInternal);
                    }
                }
            }

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
            RootChunk.AppendChunk(new FormatChunk(value));
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
            RootChunk.AppendChunk(new FormatChunk(value));
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
            RootChunk.AppendChunk(new FormatChunk(value));
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
            RootChunk.AppendChunk(new FormatChunk(value));
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
            RootChunk.AppendChunk(new FormatChunk(value));
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
            RootChunk.AppendChunk(new FormatChunk(value));
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
            RootChunk.AppendChunk(new FormatChunk(value));
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
            RootChunk.AppendChunk(new FormatChunk(value));
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
            RootChunk.AppendChunk(new FormatChunk(value));
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
            RootChunk.AppendChunk(new FormatChunk(value));
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
            RootChunk.AppendChunk(new FormatChunk(value));
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
            RootChunk.AppendChunk(new FormatChunk(value));
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
            RootChunk.AppendChunk(new FormatChunk(value));
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

            if (value == null) return this;

            string str = value as string;
            if (str != null)
            {
                RootChunk.AppendChunk(new FormatChunk(str));
                return this;
            }

            FormatChunk chunk = value as FormatChunk;
            if (chunk != null)
            {
                RootChunk.AppendChunk(chunk);
                return this;
            }

            IEnumerable<FormatChunk> chunks = value as IEnumerable<FormatChunk>;
            if (chunks != null)
            {
                if (RootChunk.ChildrenInternal == null)
                    RootChunk.ChildrenInternal = new List<FormatChunk>(chunks);
                else
                    RootChunk.ChildrenInternal.AddRange(chunks);
                return this;
            }

            RootChunk.AppendChunk(new FormatChunk(value));
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
                RootChunk.AppendChunk(new FormatChunk(new string(value)));
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
                RootChunk.AppendChunk(new FormatChunk(new string(value, startIndex, charCount)));
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
                RootChunk.AppendChunk(new FormatChunk(new string(value, repeatCount)));
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
                RootChunk.AppendChunk(new FormatChunk(value));
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
                RootChunk.AppendChunk(chunk);
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
            if (chunks == null) return this;

            FormatChunk[] cArr = chunks.ToArray();
            if (cArr.Length > 0)
                if (RootChunk.ChildrenInternal == null)
                    RootChunk.ChildrenInternal = new List<FormatChunk>(cArr);
                else
                    RootChunk.ChildrenInternal.AddRange(cArr);
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

            if (builder == null ||
                builder.IsEmpty) return this;

            Contract.Assert(builder.RootChunk.ChildrenInternal != null);
            Contract.Assert(builder.RootChunk.ChildrenInternal.Count > 0);

            // ReSharper disable AssignNullToNotNullAttribute
            if (RootChunk.ChildrenInternal == null)
                RootChunk.ChildrenInternal = new List<FormatChunk>(builder.RootChunk.ChildrenInternal);
            else
                RootChunk.ChildrenInternal.AddRange(builder.RootChunk.ChildrenInternal);
            // ReSharper restore AssignNullToNotNullAttribute
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
            RootChunk.AppendChunk(NewLineChunk);
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
            RootChunk.AppendChunk(new FormatChunk(value));
            RootChunk.AppendChunk(NewLineChunk);
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
            RootChunk.AppendChunk(new FormatChunk(value));
            RootChunk.AppendChunk(NewLineChunk);
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
            RootChunk.AppendChunk(new FormatChunk(value));
            RootChunk.AppendChunk(NewLineChunk);
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
            RootChunk.AppendChunk(new FormatChunk(value));
            RootChunk.AppendChunk(NewLineChunk);
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
            RootChunk.AppendChunk(new FormatChunk(value));
            RootChunk.AppendChunk(NewLineChunk);
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
            RootChunk.AppendChunk(new FormatChunk(value));
            RootChunk.AppendChunk(NewLineChunk);
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
            RootChunk.AppendChunk(new FormatChunk(value));
            RootChunk.AppendChunk(NewLineChunk);
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
            RootChunk.AppendChunk(new FormatChunk(value));
            RootChunk.AppendChunk(NewLineChunk);
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
            RootChunk.AppendChunk(new FormatChunk(value));
            RootChunk.AppendChunk(NewLineChunk);
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
            RootChunk.AppendChunk(new FormatChunk(value));
            RootChunk.AppendChunk(NewLineChunk);
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
            RootChunk.AppendChunk(new FormatChunk(value));
            RootChunk.AppendChunk(NewLineChunk);
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
            RootChunk.AppendChunk(new FormatChunk(value));
            RootChunk.AppendChunk(NewLineChunk);
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
            RootChunk.AppendChunk(new FormatChunk(value));
            RootChunk.AppendChunk(NewLineChunk);
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

            if (value == null)
            {
                RootChunk.AppendChunk(NewLineChunk);
                return this;
            }

            string str = value as string;
            if (str != null)
            {
                RootChunk.AppendChunk(new FormatChunk(str));
                RootChunk.AppendChunk(NewLineChunk);
                return this;
            }

            FormatChunk chunk = value as FormatChunk;
            if (chunk != null)
            {
                RootChunk.AppendChunk(chunk);
                RootChunk.AppendChunk(NewLineChunk);
                return this;
            }

            IEnumerable<FormatChunk> chunks = value as IEnumerable<FormatChunk>;
            if (chunks != null)
            {
                if (RootChunk.ChildrenInternal == null)
                    RootChunk.ChildrenInternal = new List<FormatChunk>(chunks);
                else
                    RootChunk.ChildrenInternal.AddRange(chunks);
                RootChunk.AppendChunk(NewLineChunk);
                return this;
            }

            RootChunk.AppendChunk(new FormatChunk(value));
            RootChunk.AppendChunk(NewLineChunk);
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
                RootChunk.AppendChunk(new FormatChunk(new string(value)));

            RootChunk.AppendChunk(NewLineChunk);
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
                RootChunk.AppendChunk(new FormatChunk(new string(value, startIndex, charCount)));
            }

            RootChunk.AppendChunk(NewLineChunk);
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
                RootChunk.AppendChunk(new FormatChunk(new string(value, repeatCount)));
            RootChunk.AppendChunk(NewLineChunk);
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
                RootChunk.AppendChunk(new FormatChunk(value));
            RootChunk.AppendChunk(NewLineChunk);
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
                RootChunk.AppendChunk(chunk);
            RootChunk.AppendChunk(NewLineChunk);
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
            {
                FormatChunk[] cArr = chunks.ToArray();
                if (cArr.Length > 0)
                    if (RootChunk.ChildrenInternal == null)
                        RootChunk.ChildrenInternal = new List<FormatChunk>(cArr);
                    else
                        RootChunk.ChildrenInternal.AddRange(cArr);
            }
            RootChunk.AppendChunk(NewLineChunk);
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
            {
                Contract.Assert(builder.RootChunk.ChildrenInternal != null);
                Contract.Assert(builder.RootChunk.ChildrenInternal.Count > 0);

                // ReSharper disable AssignNullToNotNullAttribute
                if (RootChunk.ChildrenInternal == null)
                    RootChunk.ChildrenInternal = new List<FormatChunk>(builder.RootChunk.ChildrenInternal);
                else
                    RootChunk.ChildrenInternal.AddRange(builder.RootChunk.ChildrenInternal);
                // ReSharper restore AssignNullToNotNullAttribute
            }
            RootChunk.AppendChunk(NewLineChunk);
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
                RootChunk.Append(format, args == null || args.Length < 1 ? null : new ListResolvable(args, false));
            return this;
        }

        /// <summary>
        /// Appends the specified format string, replacing any integer tags with the matching arguments (overriding pre-specified replacements).
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        /// <param name="isCaseSensitive">if set to <see langword="true" /> the keys are case sensitive.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        [StringFormatMethod("format")]
        public FormatBuilder AppendFormat(
            [CanBeNull] string format,
            [CanBeNull] IReadOnlyDictionary<string, object> values,
            bool isCaseSensitive = false)
        {
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
            if (!string.IsNullOrEmpty(format))
                RootChunk.Append(
                    format,
                    values == null || values.Count < 1 ? null : new DictionaryResolvable(values, isCaseSensitive, false));
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
            [CanBeNull] ResolveDelegate resolver,
            bool isCaseSensitive = false)
        {
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
            if (!string.IsNullOrEmpty(format))
                RootChunk.Append(format, resolver == null ? null : new FuncResolvable(resolver, isCaseSensitive));
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
            RootChunk.AppendChunk(NewLineChunk);
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
                RootChunk.Append(format, args == null || args.Length < 1 ? null : new ListResolvable(args, false));
            RootChunk.AppendChunk(NewLineChunk);
            return this;
        }

        /// <summary>
        /// Appends the specified format string, replacing any integer tags with the matching arguments (overriding pre-specified replacements).
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        /// <param name="isCaseSensitive">if set to <see langword="true" /> the keys are case sensitive.</param>
        /// <returns>This instance.</returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        [NotNull]
        [PublicAPI]
        [StringFormatMethod("format")]
        public FormatBuilder AppendFormatLine(
            [CanBeNull] string format,
            [CanBeNull] IReadOnlyDictionary<string, object> values,
            bool isCaseSensitive)
        {
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
            if (!string.IsNullOrEmpty(format))
                RootChunk.Append(
                    format,
                    values == null || values.Count < 1
                        ? null
                        : new DictionaryResolvable(values, isCaseSensitive, false));
            RootChunk.AppendChunk(NewLineChunk);
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
            [CanBeNull] ResolveDelegate resolver,
            bool isCaseSensitive = false)
        {
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
            if (!string.IsNullOrEmpty(format))
                RootChunk.Append(format, resolver == null ? null : new FuncResolvable(resolver, isCaseSensitive));
            RootChunk.AppendChunk(NewLineChunk);
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
        /// <returns>This instance.</returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        [NotNull]
        [PublicAPI]
        [StringFormatMethod("format")]
        // ReSharper disable once CodeAnnotationAnalyzer
        public FormatBuilder AppendControl(
            [NotNull] string tag,
            int alignment = 0,
            [CanBeNull] string format = null,
            Optional<object> value = default(Optional<object>))
        {
            Contract.Requires(!string.IsNullOrEmpty(tag));
            Contract.Requires(!IsReadOnly);
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);

            // Ensure the tag starts with the control character
            if (tag[0] != ControlChar)
                tag = ControlChar + tag;

            RootChunk.AppendChunk(new FormatChunk(null, tag, alignment, format, value));
            return this;
        }
        #endregion

        #region Resolve Overloads
        /// <summary>
        /// The initial resolutions
        /// </summary>
        [CanBeNull]
        private Resolutions _initialResolutions;

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
            if (IsEmpty ||
                (values == null) ||
                (values.Length < 1))
                return this;

            _initialResolutions = new Resolutions(_initialResolutions, new ListResolvable(values, false));
            return this;
        }

        /// <summary>
        /// Resolves any tags.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="isCaseSensitive">if set to <see langword="true" /> tag resolution is case sensitive.</param>
        /// <returns>This instance.</returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Resolve(
            [CanBeNull] IReadOnlyDictionary<string, object> values,
            bool isCaseSensitive = false)
        {
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
            if (IsEmpty ||
                (values == null) ||
                (values.Count < 1))
                return this;

            _initialResolutions = new Resolutions(
                _initialResolutions,
                new DictionaryResolvable(values, isCaseSensitive, false));
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
        public FormatBuilder Resolve([CanBeNull] ResolveDelegate resolver, bool isCaseSensitive = false)
        {
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
            if (IsEmpty ||
                (resolver == null))
                return this;

            _initialResolutions = new Resolutions(_initialResolutions, resolver, isCaseSensitive, true);
            return this;
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
        public string ToString([CanBeNull] Layout layout, ref int position, [CanBeNull] params object[] values)
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
        /// <param name="isCaseSensitive">if set to <see langword="true" /> then tags are case sensitive.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public string ToString([CanBeNull] IReadOnlyDictionary<string, object> values, bool isCaseSensitive = false)
        {
            using (StringWriter writer = new StringWriter())
            {
                WriteTo(writer, null, values, isCaseSensitive);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="layout">The layout is applied to the original <see cref="InitialLayout"/>.</param>
        /// <param name="position">The position.</param>
        /// <param name="values">The values.</param>
        /// <param name="isCaseSensitive">if set to <see langword="true" /> then tags are case sensitive.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        [NotNull]
        [PublicAPI]
        public string ToString(
            [CanBeNull] Layout layout,
            ref int position,
            [CanBeNull] IReadOnlyDictionary<string, object> values,
            bool isCaseSensitive = false)
        {
            using (StringWriter writer = new StringWriter())
            {
                position = WriteTo(writer, layout, position, null, values, isCaseSensitive);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="resolver">The values.</param>
        /// <param name="isCaseSensitive">if set to <see langword="true" /> then tags are case sensitive.</param>
        /// <param name="resolveOuterTags">if set to <see langword="true" />  outer tags should be resolved automatically in formats.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public string ToString(
            [CanBeNull] [InstantHandle] ResolveDelegate resolver,
            bool isCaseSensitive = false,
            bool resolveOuterTags = true)
        {
            using (StringWriter writer = new StringWriter())
            {
                WriteTo(writer, null, resolver, isCaseSensitive, resolveOuterTags);
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
        /// <param name="resolveOuterTags">if set to <see langword="true" />  outer tags should be resolved automatically in formats.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        [NotNull]
        [PublicAPI]
        public string ToString(
            [CanBeNull] Layout layout,
            ref int position,
            [CanBeNull] [InstantHandle] ResolveDelegate resolver,
            bool isCaseSensitive = false,
            bool resolveOuterTags = true)
        {
            using (StringWriter writer = new StringWriter())
            {
                position = WriteTo(writer, layout, position, null, resolver, isCaseSensitive, resolveOuterTags);
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
            [CanBeNull] Layout layout,
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
        /// <param name="isCaseSensitive">if set to <see langword="true" /> then tags are case sensitive.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public string ToString(
            [CanBeNull] IFormatProvider formatProvider,
            [CanBeNull] IReadOnlyDictionary<string, object> values,
            bool isCaseSensitive = false)
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
        /// <param name="isCaseSensitive">if set to <see langword="true" /> then tags are case sensitive.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        [NotNull]
        [PublicAPI]
        public string ToString(
            [CanBeNull] Layout layout,
            ref int position,
            [CanBeNull] IFormatProvider formatProvider,
            [CanBeNull] IReadOnlyDictionary<string, object> values,
            bool isCaseSensitive = false)
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
        /// <param name="resolveOuterTags">if set to <see langword="true" />  outer tags should be resolved automatically in formats.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public string ToString(
            [CanBeNull] IFormatProvider formatProvider,
            [CanBeNull] [InstantHandle] ResolveDelegate resolver,
            bool isCaseSensitive = false,
            bool resolveOuterTags = true)
        {
            using (StringWriter writer = new StringWriter(formatProvider))
            {
                WriteTo(writer, null, resolver, isCaseSensitive, resolveOuterTags);
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
        /// <param name="resolveOuterTags">if set to <see langword="true" />  outer tags should be resolved automatically in formats.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        [NotNull]
        [PublicAPI]
        public string ToString(
            [CanBeNull] Layout layout,
            ref int position,
            [CanBeNull] IFormatProvider formatProvider,
            [CanBeNull] [InstantHandle] ResolveDelegate resolver,
            bool isCaseSensitive = false,
            bool resolveOuterTags = true)
        {
            using (StringWriter writer = new StringWriter(formatProvider))
            {
                position = WriteTo(writer, layout, position, null, resolver, isCaseSensitive, resolveOuterTags);
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
            [CanBeNull] Layout layout,
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
            [CanBeNull] Layout layout,
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
            [CanBeNull] IReadOnlyDictionary<string, object> values,
            bool isCaseSensitive = false)
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
        /// <param name="isCaseSensitive">if set to <see langword="true" /> then tags are case sensitive.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        [NotNull]
        [PublicAPI]
        [StringFormatMethod("format")]
        public string ToString(
            [CanBeNull] Layout layout,
            ref int position,
            [CanBeNull] string format,
            [CanBeNull] IFormatProvider formatProvider,
            [CanBeNull] IReadOnlyDictionary<string, object> values,
            bool isCaseSensitive = false)
        {
            using (StringWriter writer = new StringWriter(formatProvider))
            {
                position = WriteTo(writer, layout, position, format, values, isCaseSensitive);
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
        /// <param name="resolveOuterTags">if set to <see langword="true" />  outer tags should be resolved automatically in formats.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        [NotNull]
        [PublicAPI]
        [StringFormatMethod("format")]
        public string ToString(
            [CanBeNull] string format,
            [CanBeNull] IFormatProvider formatProvider,
            [CanBeNull] [InstantHandle] ResolveDelegate resolver,
            bool isCaseSensitive = false,
            bool resolveOuterTags = true)
        {
            using (StringWriter writer = new StringWriter(formatProvider))
            {
                WriteTo(writer, format, resolver, isCaseSensitive, resolveOuterTags);
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
        /// <param name="resolveOuterTags">if set to <see langword="true" />  outer tags should be resolved automatically in formats.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        [NotNull]
        [PublicAPI]
        [StringFormatMethod("format")]
        public string ToString(
            [CanBeNull] Layout layout,
            ref int position,
            [CanBeNull] string format,
            [CanBeNull] IFormatProvider formatProvider,
            [CanBeNull] [InstantHandle] ResolveDelegate resolver,
            bool isCaseSensitive = false,
            bool resolveOuterTags = true)
        {
            using (StringWriter writer = new StringWriter(formatProvider))
            {
                position = WriteTo(writer, layout, position, format, resolver, isCaseSensitive, resolveOuterTags);
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
            if (IsEmpty) return;
            WriteTo(ConsoleTextWriter.Default);
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
            if (IsEmpty) return;
            WriteTo(ConsoleTextWriter.Default, format, values);
        }

        /// <summary>
        /// Writes the builder to the console.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        /// <param name="isCaseSensitive">if set to <see langword="true" /> [is case sensitive].</param>
        [PublicAPI]
        [StringFormatMethod("format")]
        public void WriteToConsole(
            [CanBeNull] string format,
            [CanBeNull] IReadOnlyDictionary<string, object> values,
            bool isCaseSensitive = false)
        {
            if (IsEmpty) return;
            WriteTo(ConsoleTextWriter.Default, format, values, isCaseSensitive);
        }

        /// <summary>
        /// Writes the builder to the console.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="resolver">The resolver.</param>
        /// <param name="isCaseSensitive">if set to <see langword="true" /> then tags are case sensitive.</param>
        /// <param name="resolveOuterTags">if set to <see langword="true" />  outer tags should be resolved automatically in formats.</param>
        [PublicAPI]
        [StringFormatMethod("format")]
        public void WriteToConsole(
            [CanBeNull] string format,
            [CanBeNull] [InstantHandle] ResolveDelegate resolver,
            bool isCaseSensitive = false,
            bool resolveOuterTags = true)
        {
            if (IsEmpty) return;
            WriteTo(ConsoleTextWriter.Default, format, resolver, isCaseSensitive, resolveOuterTags);
        }
        #endregion

        #region WriteToTrace Overloads
        /// <summary>
        /// Writes the builder to the trace.
        /// </summary>
        [PublicAPI]
        public void WriteToTrace()
        {
            if (IsEmpty) return;
            WriteTo(TraceTextWriter.Default);
        }

        /// <summary>
        /// Writes the builder to the trace.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        [PublicAPI]
        [StringFormatMethod("format")]
        public void WriteToTrace(
            [CanBeNull] string format,
            [CanBeNull] params object[] values)
        {
            if (IsEmpty) return;
            WriteTo(TraceTextWriter.Default, format, values);
        }

        /// <summary>
        /// Writes the builder to the trace.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        /// <param name="isCaseSensitive">if set to <see langword="true" /> [is case sensitive].</param>
        [PublicAPI]
        [StringFormatMethod("format")]
        public void WriteToTrace(
            [CanBeNull] string format,
            [CanBeNull] IReadOnlyDictionary<string, object> values,
            bool isCaseSensitive = false)
        {
            if (IsEmpty) return;
            WriteTo(TraceTextWriter.Default, format, values, isCaseSensitive);
        }

        /// <summary>
        /// Writes the builder to the trace.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="resolver">The resolver.</param>
        /// <param name="isCaseSensitive">if set to <see langword="true" /> then tags are case sensitive.</param>
        /// <param name="resolveOuterTags">if set to <see langword="true" />  outer tags should be resolved automatically in formats.</param>
        [PublicAPI]
        [StringFormatMethod("format")]
        public void WriteToTrace(
            [CanBeNull] string format,
            [CanBeNull] [InstantHandle] ResolveDelegate resolver,
            bool isCaseSensitive = false,
            bool resolveOuterTags = true)
        {
            if (IsEmpty) return;
            WriteTo(TraceTextWriter.Default, format, resolver, isCaseSensitive, resolveOuterTags);
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
            if (writer == null || IsEmpty) return position;
            Contract.Assert(RootChunk.ChildrenInternal != null);

            return WriteTo(
                RootChunk,
                writer,
                _initialResolutions,
                null,
                layout == null ? InitialLayout : InitialLayout.Apply(layout),
                "G",
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
        public void WriteTo([CanBeNull] TextWriter writer, string format)
        {
            if (writer == null || IsEmpty) return;
            Contract.Assert(RootChunk.ChildrenInternal != null);

            WriteTo(
                RootChunk,
                writer,
                _initialResolutions,
                null,
                InitialLayout,
                format,
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
            if (writer == null || IsEmpty) return 0;
            Contract.Assert(RootChunk.ChildrenInternal != null);

            return WriteTo(
                RootChunk,
                writer,
                _initialResolutions,
                values == null || values.Length < 1
                    ? null
                    : new ListResolvable(values, false),
                InitialLayout,
                format,
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
            if (writer == null || IsEmpty) return position;
            Contract.Assert(RootChunk.ChildrenInternal != null);

            return WriteTo(
                RootChunk,
                writer,
                _initialResolutions,
                values == null || values.Length < 1
                    ? null
                    : new ListResolvable(values, false),
                layout == null ? InitialLayout : InitialLayout.Apply(layout),
                format,
                position);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        /// <param name="isCaseSensitive">if set to <see langword="true" /> tag resolution is case sensitive.</param>
        /// <returns>The end position.</returns>
        [PublicAPI]
        [StringFormatMethod("format")]
        public int WriteTo(
            [CanBeNull] TextWriter writer,
            [CanBeNull] string format,
            [CanBeNull] IReadOnlyDictionary<string, object> values,
            bool isCaseSensitive = false)
        {
            if (writer == null || IsEmpty) return 0;
            Contract.Assert(RootChunk.ChildrenInternal != null);

            return WriteTo(
                RootChunk,
                writer,
                _initialResolutions,
                values == null || values.Count < 1
                    ? null
                    : new DictionaryResolvable(values, isCaseSensitive, false),
                InitialLayout,
                format,
                0);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="layout">The layout is applied to the original <see cref="InitialLayout" />.</param>
        /// <param name="position">The start position.</param>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        /// <param name="isCaseSensitive">if set to <see langword="true" /> tag resolution is case sensitive.</param>
        /// <returns>The end position.</returns>
        [PublicAPI]
        [StringFormatMethod("format")]
        public int WriteTo(
            [CanBeNull] TextWriter writer,
            [CanBeNull] Layout layout,
            int position,
            [CanBeNull] string format,
            [CanBeNull] IReadOnlyDictionary<string, object> values,
            bool isCaseSensitive = false)
        {
            if (writer == null || IsEmpty) return position;
            Contract.Assert(RootChunk.ChildrenInternal != null);

            return WriteTo(
                RootChunk,
                writer,
                _initialResolutions,
                values == null || values.Count < 1
                    ? null
                    : new DictionaryResolvable(values, isCaseSensitive, false),
                layout == null ? InitialLayout : InitialLayout.Apply(layout),
                format,
                position);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <param name="resolver">The resolver.</param>
        /// <param name="isCaseSensitive">if set to <see langword="true" /> then tags are case sensitive.</param>
        /// <param name="resolveOuterTags">if set to <see langword="true" />  outer tags should be resolved automatically in formats.</param>
        /// <returns>The end position.</returns>
        [PublicAPI]
        [StringFormatMethod("format")]
        public int WriteTo(
            [CanBeNull] TextWriter writer,
            [CanBeNull] string format,
            [CanBeNull] [InstantHandle] ResolveDelegate resolver,
            bool isCaseSensitive = false,
            bool resolveOuterTags = true)
        {
            if (writer == null || IsEmpty) return 0;
            Contract.Assert(RootChunk.ChildrenInternal != null);

            return WriteTo(
                RootChunk,
                writer,
                _initialResolutions,
                resolver == null
                    ? null
                    : new FuncResolvable(resolver, isCaseSensitive, resolveOuterTags),
                InitialLayout,
                format,
                0);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="layout">The layout is applied to the original <see cref="InitialLayout" />.</param>
        /// <param name="position">The start position.</param>
        /// <param name="format">The format.</param>
        /// <param name="resolver">The resolver.</param>
        /// <param name="isCaseSensitive">if set to <see langword="true" /> then tags are case sensitive.</param>
        /// <param name="resolveOuterTags">if set to <see langword="true" />  outer tags should be resolved automatically in formats.</param>
        /// <returns>The end position.</returns>
        [PublicAPI]
        [StringFormatMethod("format")]
        public int WriteTo(
            [CanBeNull] TextWriter writer,
            [CanBeNull] Layout layout,
            int position,
            [CanBeNull] string format,
            [CanBeNull] [InstantHandle] ResolveDelegate resolver,
            bool isCaseSensitive = false,
            bool resolveOuterTags = true)
        {
            if (writer == null || IsEmpty) return position;
            Contract.Assert(RootChunk.ChildrenInternal != null);

            return WriteTo(
                RootChunk,
                writer,
                _initialResolutions,
                resolver == null
                    ? null
                    : new FuncResolvable(resolver, isCaseSensitive, resolveOuterTags),
                layout == null ? InitialLayout : InitialLayout.Apply(layout),
                format,
                position);
        }
        #endregion

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="rootChunk"></param>
        /// <param name="writer">The writer.</param>
        /// <param name="initialResolutions"></param>
        /// <param name="resolver"></param>
        /// <param name="initialLayout">The initial layout.</param>
        /// <param name="format">The format.</param>
        /// <param name="position">The position.</param>
        /// <returns>The final position.</returns>
        [PublicAPI]
        [StringFormatMethod("format")]
        private static int WriteTo(
            [NotNull] FormatChunk rootChunk,
            [NotNull] TextWriter writer,
            [CanBeNull] Resolutions initialResolutions,
            [CanBeNull] IResolvable resolver,
            [NotNull] Layout initialLayout,
            [CanBeNull] string format,
            int position)
        {
            Contract.Requires(rootChunk != null);
            Contract.Requires(writer != null);
            Contract.Requires(initialLayout != null);
            ISerialTextWriter serialTextWriter = writer as ISerialTextWriter;
            return serialTextWriter == null
                ? DoWrite(
                    rootChunk,
                    writer,
                    null,
                    initialResolutions,
                    resolver,
                    initialLayout,
                    format,
                    position)
                : serialTextWriter.Context.Invoke(
                    () =>
                        DoWrite(
                            rootChunk,
                            writer,
                            serialTextWriter.Writer,
                            initialResolutions,
                            resolver,
                            initialLayout,
                            format,
                            position));
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
        /// The new line characters.
        /// </summary>
        [NotNull]
        private static readonly char[] _newLineChars = { '\r', '\n' };

        /// <summary>
        /// Gets the chunk as a string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="alignment">The alignment.</param>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>System.String.</returns>
        [NotNull]
        private static string GetChunkString(
            [NotNull] object value,
            int alignment,
            [CanBeNull] string format,
            [CanBeNull] IFormatProvider formatProvider)
        {
            Contract.Requires(value != null);
            string vStr;
            // We are not aligning so we can output the value directly.
            if (!string.IsNullOrEmpty(format))
            {
                IWriteable writeable = value as IWriteable;
                if (writeable != null)
                    using (StringWriter sw = new StringWriter(formatProvider))
                    {
                        writeable.WriteTo(sw, format);
                        vStr = sw.ToString();
                    }
                else
                {
                    IFormattable formattable = value as IFormattable;
                    if (formattable != null)
                        // When using this interface we have to suppress <see cref="FormatException"/>.
                        try
                        {
                            vStr = formattable.ToString(format, formatProvider);
                        }
                        // ReSharper disable once EmptyGeneralCatchClause
                        catch (FormatException)
                        {
                            vStr = value.ToString();
                        }
                    else
                        vStr = value.ToString();
                }
            }
            else
                vStr = value.ToString();

            // Suppress large alignment values for safety. (Note this handles int.MinValue unlike Math.Abs())
            if (alignment == 0 ||
                alignment > 1024 ||
                alignment < -1024)
                return vStr;

            // Pad the string if necessary.
            int len = vStr.Length;
            if (len < alignment)
                return new string(' ', alignment - len) + vStr;
            if (len >= -alignment) return vStr;
            return vStr + new string(' ', -alignment - len);
        }

        /// <summary>
        /// Character types.
        /// </summary>
        private enum CharType
        {
            /// <summary>
            /// No character.
            /// </summary>
            None,

            /// <summary>
            /// A white space character
            /// </summary>
            WhiteSpace,

            /// <summary>
            /// Symbol character.
            /// </summary>
            Apostrophe,

            /// <summary>
            /// Symbol character.
            /// </summary>
            Symbol,

            /// <summary>
            /// Alphanumeric character
            /// </summary>
            Alphanumeric
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="rootChunk">The root chunk.</param>
        /// <param name="writer">The writer.</param>
        /// <param name="serialWriter">The serial writer.</param>
        /// <param name="initialResolutions">The initial resolutions.</param>
        /// <param name="resolver">The resolver.</param>
        /// <param name="initialLayout">The initial layout.</param>
        /// <param name="format">The format.</param>
        /// <param name="position">The position.</param>
        /// <returns>The final position.</returns>
        [PublicAPI]
        [StringFormatMethod("format")]
        private static int DoWrite(
            [NotNull] FormatChunk rootChunk,
            [NotNull] TextWriter writer,
            [CanBeNull] TextWriter serialWriter,
            [CanBeNull] Resolutions initialResolutions,
            [CanBeNull] IResolvable resolver,
            [NotNull] Layout initialLayout,
            [CanBeNull] string format,
            int position)
        {
            Contract.Requires(rootChunk != null);
            Contract.Requires(rootChunk.ChildrenInternal.Count > 0);
            Contract.Requires(writer != null);
            Contract.Requires(initialLayout != null);

            #region Setup
            /*
             * Setup writers
             */
            // ReSharper disable SuspiciousTypeConversion.Global
            IControllableTextWriter controller = serialWriter as IControllableTextWriter ??
                                                 writer as IControllableTextWriter;
            // ReSharper restore SuspiciousTypeConversion.Global
            ILayoutTextWriter layoutWriter = serialWriter as ILayoutTextWriter ?? writer as ILayoutTextWriter;
            IColoredTextWriter coloredTextWriter = serialWriter as IColoredTextWriter ?? writer as IColoredTextWriter;
            if (serialWriter != null) writer = serialWriter;

            int writerWidth;
            bool autoWraps;
            if (layoutWriter != null)
            {
                position = layoutWriter.Position;
                writerWidth = layoutWriter.Width;
                autoWraps = layoutWriter.AutoWraps;

                // Ensure layout doesn't exceed writer width.
                if (initialLayout.Width.Value >= writerWidth)
                    initialLayout = initialLayout.Apply(writerWidth);
            }
            else
            {
                writerWidth = int.MaxValue;
                autoWraps = false;
            }

            /*
             * Setup format
             */
            if (format == null)
                format = "g";

            // Always write tags out - 'F' format.
            bool writeTags;
            // Whether to write out unresolved tags.
            bool skipUnresolvedTags;
            // Whether we require layout.
            bool isLayoutRequired;

            // Check which format we have 'f' will just write out tags, and ignore Layout.
            switch (format.ToLowerInvariant())
            {
                // Always output's the tag if the chunk has one, otherwise output's the value as normal
                case "f":
                    writeTags = true;
                    skipUnresolvedTags = false;
                    isLayoutRequired = false;
                    break;

                // Should output the value as normal, but treats unresolved tags as an empty string value
                case "s":
                    writeTags = false;
                    skipUnresolvedTags = true;
                    isLayoutRequired = initialLayout != Layout.Default;
                    break;

                // Outputs the value if set, otherwise the format tag. Control tags ignored
                default:
                    writeTags = false;
                    skipUnresolvedTags = false;
                    isLayoutRequired = initialLayout != Layout.Default;
                    break;
            }

            initialResolutions = resolver != null
                ? new Resolutions(initialResolutions, resolver)
                : initialResolutions;

            // The layout stack is used to hold the current layout
            Stack<Layout> layoutStack = new Stack<Layout>();
            layoutStack.Push(initialLayout);

            // The current word and line (if laying out).
            Line line = null;
            StringBuilder wordBuilder = null;
            CharType lastCharType = CharType.None;

            // The stack holds any chunks that we need to process, so start by pushing the root chunks children onto it
            // in reverse, so that they are taken off in order.
            Stack<FormatChunk, Resolutions> stack = new Stack<FormatChunk, Resolutions>();
            for (int rsi = rootChunk.ChildrenInternal.Count - 1; rsi > -1; rsi--)
                stack.Push(rootChunk.ChildrenInternal[rsi], initialResolutions);
            #endregion

            /*
             * Process chunks
             */
            while (stack.Count > 0 ||
                   (wordBuilder != null && wordBuilder.Length > 0))
            {
                string chunkStr;
                if (stack.Count > 0)
                {
                    FormatChunk chunk;
                    Resolutions resolutions;
                    stack.Pop(out chunk, out resolutions);

                    #region Resolution - will flatten chunks and resolve to a series of strings.
                    if (chunk.Tag != null)
                        /*
                         * Process fill point
                         */
                        if (writeTags)
                            chunkStr = chunk.ToString("F");
                        else
                        {
                            if (chunk.Resolver != null)
                                resolutions = new Resolutions(resolutions, chunk.Resolver);

                            bool isResolved;
                            object resolvedValue;
                            // Resolve the tag if it's the first time we've seen it.
                            if (resolutions != null)
                            {
                                // ReSharper disable PossibleNullReferenceException
                                Resolution resolved = (Resolution)resolutions.Resolve(writer, chunk);
                                // ReSharper restore PossibleNullReferenceException
                                isResolved = resolved.IsResolved;
                                resolvedValue = resolved.Value;
                            }
                            else
                            {
                                isResolved = false;
                                resolvedValue = null;
                            }

                            if (isResolved || chunk.IsResolved)
                            {
                                // If we haven't resolved the value, get the chunks value.
                                if (!isResolved)
                                {
                                    // Use the current resolution.
                                    isResolved = true;
                                    resolvedValue = chunk.Value;
                                }

                                // Check for resolved to null.
                                if (resolvedValue != null)
                                {
                                    /*
                                     * Check if we have an actual FormatChunk as the value, in which case, unwrap it.
                                     */
                                    do
                                    {
                                        FormatChunk fc = resolvedValue as FormatChunk;
                                        if (fc == null) break;

                                        chunk = fc;
                                        isResolved = chunk.IsResolved;
                                        if (!isResolved)
                                            break;

                                        resolvedValue = fc.Value;
                                    } while (true);

                                    if (isResolved)
                                    {
                                        /*
                                         * Unwrap format builders, or enumerations of chunks
                                         */
                                        IEnumerable<FormatChunk> formatChunks =
                                            resolvedValue as IEnumerable<FormatChunk>;
                                        if (formatChunks != null)
                                        {
                                            foreach (FormatChunk fci in formatChunks.Reverse())
                                                stack.Push(fci, resolutions);
                                            continue;
                                        }

                                        /*
                                         * Check if we have any child chunks, and flatten
                                         */
                                        if (chunk.ChildrenInternal != null &&
                                            chunk.ChildrenInternal.Count > 0)
                                        {
                                            // Get the chunks for the fill point.
                                            Stack<FormatChunk> subFormatChunks = new Stack<FormatChunk>();
                                            bool hasFillPoint = false;
                                            bool hasItemsFillPoint = false;
                                            FormatChunk joinChunk = null;
                                            foreach (FormatChunk subFormatChunk in chunk.ChildrenInternal)
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

                                            /*
                                             * Special case enumerable, if we have an '<items>' tag, then we will treat each item in the enumeration
                                             * individually, otherwise we'll treat the enumeration as one value.
                                             */
                                            if (hasItemsFillPoint)
                                            {
                                                // We have an <item> fill point, so check if we have an enumerable.
                                                IEnumerable enumerable = resolvedValue as IEnumerable;
                                                if (enumerable != null)
                                                {
                                                    // Set the value of the joinChunk to FormatOutput.Default, which is an object that writes out it's own format!
                                                    if (joinChunk != null)
                                                        joinChunk = new FormatChunk(joinChunk.Format);

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
                                                        if (!string.Equals(
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
                                                            int value = kvp.Value;

                                                            // This will add a fall-through value for the '<item>' and '<index>' tags - a new child Resolutions will be created based on this one
                                                            // when the IResolution object is later resolved below, which means that you can still technically override the value of these tags in
                                                            // the underlying resolver.
                                                            Resolutions inner = new Resolutions(
                                                                resolutions,
                                                                (_, c) =>
                                                                {
                                                                    // ReSharper disable PossibleNullReferenceException
                                                                    switch (c.Tag.ToLowerInvariant())
                                                                    // ReSharper restore PossibleNullReferenceException
                                                                    {
                                                                        case IndexTag:
                                                                            return value;
                                                                        case ItemTag:
                                                                            return key;
                                                                        default:
                                                                            return Resolution.Unknown;
                                                                    }
                                                                },
                                                                false,
                                                                true);

                                                            // Add a new chunk with, the <Item> tag.
                                                            FormatChunk innerChunk = new FormatChunk(
                                                                null,
                                                                ItemTag,
                                                                subFormatChunk.Alignment,
                                                                subFormatChunk.Format,
                                                                key);

                                                            if (subFormatChunk.ChildrenInternal != null)
                                                                innerChunk.ChildrenInternal =
                                                                    subFormatChunk.ChildrenInternal.ToList();

                                                            stack.Push(innerChunk, inner);

                                                            // If we have join chunk, push it for all but the 'first' element.
                                                            if (value > 0 &&
                                                                joinChunk != null)
                                                                stack.Push(joinChunk, inner);
                                                        }
                                                    }
                                                    continue;
                                                }
                                            }

                                            // If we have a value, and a format, then we may need to recurse.
                                            if (hasFillPoint)
                                            {
                                                IResolvable r = resolvedValue as IResolvable;
                                                if (r != null)
                                                    resolutions = new Resolutions(
                                                        resolutions,
                                                        r.Resolve,
                                                        r.IsCaseSensitive,
                                                        r.ResolveOuterTags);

                                                while (subFormatChunks.Count > 0)
                                                    stack.Push(subFormatChunks.Pop(), resolutions);
                                                continue;
                                            } // No fill points in format.
                                        } // No children
                                    } // No resolution after unwrapping
                                } // Null resolution
                            } // No resolution.

                            /*
                             * Check for layout chunks
                             */
                            if (string.Equals(
                                chunk.Tag,
                                LayoutTag,
                                StringComparison.InvariantCultureIgnoreCase))
                            {
                                isLayoutRequired = true;
                                Layout newLayout;
                                bool hasFormat = !string.IsNullOrEmpty(chunk.Format);
                                if ((isResolved &&
                                     ((newLayout = chunk.Value as Layout) != null)) ||
                                    (hasFormat &&
                                     Layout.TryParse(chunk.Format, out newLayout)))
                                {
                                    if (!newLayout.IsEmpty)
                                    {
                                        newLayout = layoutStack.Peek().Apply(newLayout);
                                        // Ensure layout doesn't exceed writer width.
                                        if (newLayout.Width.Value >= writerWidth)
                                            newLayout = newLayout.Apply(writerWidth);
                                        layoutStack.Push(newLayout);
                                    }
                                }
                                else if ((!hasFormat) &&
                                         (layoutStack.Count > 1))
                                    layoutStack.Pop();
                            }

                            if (chunk.IsControl)
                            {
                                if (controller != null)
                                    controller.OnControlChunk(
                                        writer,
                                        chunk.Tag,
                                        chunk.Alignment,
                                        chunk.Format,
                                        chunk.Value);

                                #region Color support
                                if (coloredTextWriter == null)
                                    continue;

                                // Handle colored output.
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
                                #endregion
                            }

                            if (!isResolved)
                            {
                                if (skipUnresolvedTags)
                                    continue;
                                chunkStr = chunk.ToString("F");
                            }
                            else
                                chunkStr = GetChunkString(
                                    resolvedValue,
                                    chunk.Alignment,
                                    chunk.Format,
                                    writer.FormatProvider);
                        }
                    else
                        // We have a value chunk.
                        chunkStr = GetChunkString(
                            chunk.Value,
                            chunk.Alignment,
                            chunk.Format,
                            writer.FormatProvider);
                    #endregion

                    if (!isLayoutRequired)
                    {
                        // We're done as no layout is required.
                        position += chunkStr.Length;
                        writer.Write(chunkStr);
                        continue;
                    }
                }
                else
                    chunkStr = string.Empty;

                #region Layout
                /*
                 * Layout chunks
                 */

                // Create word builder if we're just starting.
                if (wordBuilder == null)
                    wordBuilder = new StringBuilder();

                // Take one character at a time.
                int cPos = 0;
                while (cPos < chunkStr.Length ||
                       (wordBuilder.Length > 0) ||
                       (stack.Count < 1 && line != null && !line.IsEmpty))
                {
                    string word;

                    #region Process characters into 'words'.
                    if (cPos < chunkStr.Length)
                    {
                        /*
                         * Process characters into 'words'.
                         */
                        char ch = chunkStr[cPos++];

                        if (ch == '\r')
                        {
                            // Skip next '\n' if any.
                            if ((cPos < chunkStr.Length) &&
                                (chunkStr[cPos] == '\n'))
                                cPos++;
                            word = wordBuilder.ToString();
                            wordBuilder.Clear();
                            // we normalize all newline styles to '\r'.
                            wordBuilder.Append('\r');
                            lastCharType = CharType.WhiteSpace;
                        }
                        else if (ch == '\n')
                        {
                            word = wordBuilder.ToString();
                            wordBuilder.Clear();
                            // we normalize all newline styles to '\r'.
                            wordBuilder.Append('\r');
                            lastCharType = CharType.WhiteSpace;
                        }
                        else if (char.IsLetterOrDigit(ch))
                        {
                            if ((lastCharType == CharType.Apostrophe) ||
                                (wordBuilder.Length < 2 && lastCharType != CharType.WhiteSpace) ||
                                (lastCharType == CharType.Alphanumeric))
                            {
                                lastCharType = CharType.Alphanumeric;
                                wordBuilder.Append(ch);
                                continue;
                            }
                            // This letter follows a non alpha numeric, so split the word, unless the character was an
                            // apostrophe, i.e. keep "Craig's" and "I'm" as one word, or the symbol started a word,
                            // e.g. "'quoted'".
                            lastCharType = CharType.Alphanumeric;
                            word = wordBuilder.ToString();
                            wordBuilder.Clear();
                            wordBuilder.Append(ch);
                        }
                        else if (!char.IsWhiteSpace(ch))
                        {
                            if (lastCharType == CharType.Alphanumeric)
                            {
                                lastCharType = ch == '\'' ? CharType.Apostrophe : CharType.Symbol;
                                wordBuilder.Append(ch);
                                continue;
                            }
                            lastCharType = ch == '\'' ? CharType.Apostrophe : CharType.Symbol;
                            word = wordBuilder.ToString();
                            wordBuilder.Clear();
                            wordBuilder.Append(ch);
                        }
                        else
                        {
                            lastCharType = CharType.WhiteSpace;
                            word = wordBuilder.ToString();
                            wordBuilder.Clear();
                            wordBuilder.Append(ch);
                        }
                        if (string.IsNullOrEmpty(word))
                            continue;
                    }
                    else
                    {
                        word = wordBuilder.ToString();
                        wordBuilder.Clear();
                    }
                    #endregion

                    do
                    {
                        if (!string.IsNullOrEmpty(word))
                        {
                            #region Get or create line
                            /*
                             * Get or create line
                             */
                            Layout layout;
                            // If we haven't got a line, create one.
                            if (line == null)
                            {
                                // Starting a new line, so grab the current layout at the top of the stack.
                                layout = layoutStack.Peek();
                                line = position > 0
                                    ? new Line(
                                        layout,
                                        // We can't start a layout in the middle of a line.
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
                            }
                            else if (position < 1 &&
                                     line.Layout != layoutStack.Peek())
                            {
                                // We have a new layout at the start of a line, so recreate the line.
                                Contract.Assert(line.IsEmpty);
                                layout = layoutStack.Peek();

                                // Re-create line with new layout.
                                line = new Line(
                                    layout,
                                    layout.Alignment.Value,
                                    line.IsFirstLine ? layout.FirstLineIndentSize.Value : layout.IndentSize.Value,
                                    layout.Width.Value - layout.RightMarginSize.Value,
                                    line.IsFirstLine);
                            }
                            else
                                // We use the current line's layout until we get a new line.
                                layout = line.Layout;
                            #endregion

                            char c = word[0];
                            // Check if we're at the start of a line.
                            byte split = layout.SplitLength.Value;
                            if (line.IsEmpty)
                            {
                                // Skip spaces at the start of a line, if we have an alignment
                                if ((c == ' ') &&
                                    (line.Alignment != Alignment.None))
                                {
                                    word = null;
                                    continue;
                                }

                                // We always split this word if it's too long, as we're going from the start of a line.
                                split = 1;
                            }

                            // Check for newline
                            if (c != '\r')
                            {
                                // Check remaining space.
                                int remaining = line.Remaining;
                                if (remaining > 0)
                                {
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

                                        if (tabSize > remaining)
                                            tabSize = remaining;

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
                                    int hyphenate = layout.Hyphenate.Value ? 1 : 0;
                                    if ((split > 0) &&
                                        (remaining >= (hyphenate + layout.SplitLength.Value)) &&
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
                                }
                                // No space left on the line, but not terminated.
                                line.Finish(true);
                            }
                            else
                            {
                                // Finished an input line.
                                line.Finish(true);
                                word = null;
                            }
                        }

                        #region Alignment
                        // TODO Write out line and create new one!
                        foreach (var chunk in line)
                        {
                            writer.Write(chunk);
                        }
                        if (line.Terminated)
                            writer.WriteLine();

                        position = 0;
                        line = null;
                        #endregion
                    } while (!string.IsNullOrEmpty(word));
                }
                #endregion
            }
            return position;
        }

        #region Remainder
#if OLDCODE
            IEnumerable<FormatChunk> chunks;

            // Check which format we have 'f' will just write out tags, and ignore Layout.
            if (string.Equals(format, "f", StringComparison.InvariantCultureIgnoreCase))
            {
                writeTags = true;
                chunks = rootChunk.Children;
                isLayoutRequired = false;
            }
            else
            {
                writeTags = false;
                isLayoutRequired = initialLayout != Layout.Default;
                
                /*
                 * Resolve values TODO Should only need to do this when not format 'F'.?
                 */
                initialResolutions = resolver != null
                    ? new Resolutions(initialResolutions, resolver)
                    : initialResolutions;
                chunks = Resolve(
                    rootChunk,
                    writer,
                    initialResolutions,
                    ref isLayoutRequired);
            }

            /*
             * If we're not using a layout writer, and we don't require layout, then we need to manually track position.
             * so the code is subtly different.
             */
            if (layoutWriter == null &&
                !isLayoutRequired)
            {
                foreach (FormatChunk chunk in chunks)
                    // ReSharper disable once PossibleNullReferenceException
                    if (chunk.IsControl &&
                        !writeTags)
                    {
                        if (controller != null)
                            // ReSharper disable once AssignNullToNotNullAttribute
                            controller.OnControlChunk(writer, chunk.Tag, chunk.Alignment, chunk.Format, chunk.Value);

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
                                    coloredTextWriter.SetForegroundColor((Color) chunk.Value);
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
                                    coloredTextWriter.SetBackgroundColor((Color) chunk.Value);
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
            if (isLayoutRequired ||
                (writerWidth < int.MaxValue))
                chunks = Align(
                    GetLines(
                        GetLineChunks(chunks, format, writer.FormatProvider),
                        initialLayout,
                        position,
                        writerWidth),
                    initialLayout,
                    writerWidth,
                    autoWraps,
                    ref position);

            // We try to output the builder in one go to prevent interleaving, however we split on control codes.
            foreach (FormatChunk chunk in chunks)
                // ReSharper disable once PossibleNullReferenceException
                if (chunk.IsControl &&
                    !writeTags)
                {
                    if (controller != null)
                        // ReSharper disable once AssignNullToNotNullAttribute
                        controller.OnControlChunk(writer, chunk.Tag, chunk.Alignment, chunk.Format, chunk.Value);

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
                                coloredTextWriter.SetForegroundColor((Color) chunk.Value);
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
                                coloredTextWriter.SetBackgroundColor((Color) chunk.Value);
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
#endif
        #endregion

        #region GetLines
#if OLDCODE
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
#endif
        #endregion

        #region Align
#if OLDCODE
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
                        if (remaining > 0 &&
                            line.LastWhiteSpace > 0)
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
                            chunks.Add(new FormatChunk(lb.ToString()));
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
                    chunks.Add(new FormatChunk(lb.ToString()));
                    lb.Clear();
                }
                lb.Clear();
            }
            return chunks;
        }
#endif
        #endregion

        #region Color Control
        /// <summary>
        /// The reset colors control tag.
        /// </summary>
        [
            NotNull]
        [PublicAPI]
        public const string ResetColorsTag = "!resetcolors";

        /// <summary>
        /// The reset colors chunk.
        /// </summary>
        [NotNull]
        public static readonly FormatChunk ResetColorsChunk = new FormatChunk(null, ResetColorsTag, 0, null);

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
        public static readonly FormatChunk ResetForegroundColorChunk = new FormatChunk(
            null,
            ForegroundColorTag,
            0,
            null);

        /// <summary>
        /// The reset background color chunk.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly FormatChunk ResetBackgroundColorChunk = new FormatChunk(
            null,
            BackgroundColorTag,
            0,
            null);

        /// <summary>
        /// Adds a control to reset the foreground and background colors
        /// </summary>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendResetColors()
        {
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);

            RootChunk.AppendChunk(ResetColorsChunk);
            return this;
        }

        /// <summary>
        /// Adds a control to reset the foreground color.
        /// </summary>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendResetForegroundColor()
        {
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);

            RootChunk.AppendChunk(ResetForegroundColorChunk);
            return this;
        }

        /// <summary>
        /// Adds a control to reset the background color.
        /// </summary>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendResetBackgroundColor()
        {
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);

            RootChunk.AppendChunk(ResetBackgroundColorChunk);
            return this;
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
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);

            Color c = color.ToColor();
            RootChunk.AppendChunk(new FormatChunk(null, ForegroundColorTag, 0, c.GetName(), c));
            return this;
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
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);

            RootChunk.AppendChunk(new FormatChunk(null, ForegroundColorTag, 0, color.GetName(), color));
            return this;
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
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
            if (string.IsNullOrWhiteSpace(color)) return this;

            RootChunk.AppendChunk(new FormatChunk(null, ForegroundColorTag, 0, color));
            return this;
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
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);

            Color c = color.ToColor();
            RootChunk.AppendChunk(new FormatChunk(null, BackgroundColorTag, 0, c.GetName(), c));
            return this;
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
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);

            RootChunk.AppendChunk(new FormatChunk(null, BackgroundColorTag, 0, color.GetName(), color));
            return this;
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
            if (_isReadOnly)
                throw new InvalidOperationException(Resources.FormatBuilder_ReadOnly);
            if (string.IsNullOrWhiteSpace(color)) return this;

            RootChunk.AppendChunk(new FormatChunk(null, BackgroundColorTag, 0, color));
            return this;
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
        public static readonly FormatChunk ResetLayoutChunk = new FormatChunk(null, LayoutTag, 0, null);

        /// <summary>
        /// The new line chunk
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly FormatChunk NewLineChunk = new FormatChunk(Environment.NewLine);

        /// <summary>
        /// Resets the layout.
        /// </summary>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendResetLayout()
        {
            Contract.Requires(!IsReadOnly);
            RootChunk.AppendChunk(ResetLayoutChunk);
            return this;
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
            if (layout == null) return this;
            RootChunk.AppendChunk(new FormatChunk(null, LayoutTag, 0, layout.ToString("f"), layout));
            return this;
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
            return Append(new FormatChunk(null, LayoutTag, 0, layout.ToString("f"), layout));
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
        public override bool Equals([CanBeNull] object obj)
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
        public bool Equals([CanBeNull] FormatBuilder other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _isReadOnly.Equals(other._isReadOnly) &&
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

        #region IEnumerable
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IEnumerator<FormatChunk> GetEnumerator()
        {
            return RootChunk.AllChildren.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}