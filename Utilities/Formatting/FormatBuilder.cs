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
using System.Linq;
using System.Text;
using System.Threading;
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
        public const char Open = '{';

        /// <summary>
        /// The last character of a fill point.
        /// </summary>
        public const char Close = '}';

        /// <summary>
        /// The alignment character separates the tag from an alignment
        /// </summary>
        public const char Alignment = ',';

        /// <summary>
        /// The splitter character separates the tag/alignment from the format.
        /// </summary>
        public const char Splitter = ':';

        [NotNull]
        private readonly IReadOnlyDictionary<string, object> _values;

        [NotNull]
        private readonly IFormatProvider _formatProvider;

        [NotNull]
        private readonly List<FormatChunk> _chunks = new List<FormatChunk>();

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FormatBuilder" /> class.
        /// </summary>
        /// <param name="values">The values.</param>
        public FormatBuilder([CanBeNull] params object[] values)
        {
            _formatProvider = Thread.CurrentThread.CurrentCulture;
            _values = ToDictionary(_formatProvider, values);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatBuilder" /> class.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="values">The values.</param>
        public FormatBuilder([CanBeNull] IFormatProvider formatProvider, [CanBeNull] params object[] values)
        {
            _formatProvider = formatProvider ?? Thread.CurrentThread.CurrentCulture;
            _values = ToDictionary(_formatProvider, values);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatBuilder"/> class.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="formatProvider">The format provider.</param>
        public FormatBuilder([CanBeNull] IEnumerable<object> values, [CanBeNull] IFormatProvider formatProvider = null)
        {
            _formatProvider = formatProvider ?? Thread.CurrentThread.CurrentCulture;
            _values = values == null ? _empty : ToDictionary(_formatProvider, values.ToArray());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatBuilder"/> class.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="formatProvider">The format provider.</param>
        public FormatBuilder(
            [CanBeNull] IReadOnlyDictionary<string, object> values,
            [CanBeNull] IFormatProvider formatProvider = null)
        {
            _formatProvider = formatProvider ?? Thread.CurrentThread.CurrentCulture;
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
        /// Gets the current format provider.
        /// </summary>
        /// <value>The format provider.</value>
        [NotNull]
        [PublicAPI]
        public IFormatProvider FormatProvider
        {
            get { return _formatProvider; }
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
        
        #region Append overloads
        /// <summary>
        /// Appends the specified format string.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Append([CanBeNull] string format)
        {
            return AppendInternal(format, false, _empty);
        }

        /// <summary>
        /// Appends the specified format string, replacing any integer tags with the matching arguments (overriding pre-specified replacements).
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Append([CanBeNull] string format, [CanBeNull] params object[] args)
        {
            return AppendInternal(format, false, ToDictionary(FormatProvider, args));
        }

        /// <summary>
        /// Appends the specified format string, replacing any integer tags with the matching arguments (overriding pre-specified replacements).
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Append([CanBeNull] string format, [CanBeNull] IReadOnlyDictionary<string, object> values)
        {
            return AppendInternal(format, false, values ?? _empty);
        }
        #endregion

        #region AppendLine overloads
        /// <summary>
        /// Appends a new line.
        /// </summary>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendLine()
        {
            return AppendInternal(null, true, _empty);
        }

        /// <summary>
        /// Appends the specified format string.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendLine([CanBeNull] string format)
        {
            return AppendInternal(format, true, _empty);
        }

        /// <summary>
        /// Appends the specified format string, replacing any integer tags with the matching arguments (overriding pre-specified replacements).
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendLine([CanBeNull] string format, [CanBeNull] params object[] args)
        {
            return AppendInternal(format, true, ToDictionary(FormatProvider, args));
        }

        /// <summary>
        /// Appends the specified format string, replacing any integer tags with the matching arguments (overriding pre-specified replacements).
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder AppendLine(
            [CanBeNull] string format,
            [CanBeNull] IReadOnlyDictionary<string, object> values)
        {
            return AppendInternal(format, true, values ?? _empty);
        }
        #endregion

        /// <summary>
        /// Internal append implementation.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="addLine">if set to <see langword="true" /> [add line].</param>
        /// <param name="values">The values.</param>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        private FormatBuilder AppendInternal([CanBeNull] string format, bool addLine, [NotNull] IReadOnlyDictionary<string, object> values)
        {
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

            if (addLine)
                _chunks.Add(FormatChunk.Create(Environment.NewLine));

            return this;
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
        /// <param name="provider">The provider.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Resolve(
            [CanBeNull] Func<string, object> resolver,
            [CanBeNull] IFormatProvider provider = null)
        {
            if (resolver == null) return this;
            if (provider == null) provider = FormatProvider;

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
        public FormatBuilder Resolve([CanBeNull] params object[] values)
        {
            if ((values == null) ||
                (values.Length < 1)) return this;
            IFormatProvider provider = FormatProvider;
            return Resolve(ToDictionary(provider, values), provider);
        }

        /// <summary>
        /// Allows you to resolve any unresolved fill points.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="provider">The provider.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Resolve(
            [CanBeNull] IEnumerable<object> values,
            [CanBeNull] IFormatProvider provider)
        {
            if (values == null) return this;
            if (provider == null) provider = FormatProvider;
            return Resolve(ToDictionary(provider, values.ToArray()), provider);
        }

        /// <summary>
        /// Allows you to resolve any unresolved fill points.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="provider">The provider.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Resolve(
            [CanBeNull] IFormatProvider provider,
            [CanBeNull] params object[] values)
        {
            if ((values == null) ||
                (values.Length < 1)) return this;
            if (provider == null) provider = FormatProvider;
            return Resolve(ToDictionary(provider, values), provider);
        }

        /// <summary>
        /// Allows you to resolve any unresolved fill points.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="provider">The provider.</param>
        /// <returns>This instance.</returns>
        [NotNull]
        [PublicAPI]
        public FormatBuilder Resolve(
            [CanBeNull] IReadOnlyDictionary<string, object> values,
            [CanBeNull] IFormatProvider provider = null)
        {
            if ((values == null) ||
                (values.Count < 1)) return this;
            if (provider == null) provider = FormatProvider;

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
        /// Converts to dictionary lookup.
        /// </summary>
        /// <param name="provider">The format provider.</param>
        /// <param name="values">The values.</param>
        /// <returns>A <see cref="IReadOnlyDictionary{TKey,TValue}"/>.</returns>
        [NotNull]
        private static IReadOnlyDictionary<string, object> ToDictionary(
            [NotNull] IFormatProvider provider,
            [CanBeNull] object[] values)
        {
            Contract.Requires(provider != null);
            return values == null || values.Length < 1
                ? _empty
                : values
                    .Select((v, i) => new KeyValuePair<string, object>(i.ToString(provider), v))
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
        public string ToString([CanBeNull] string format, [CanBeNull] IFormatProvider formatProvider)
        {
            if (formatProvider == null) formatProvider = FormatProvider;

            StringBuilder builder = new StringBuilder();
            foreach (FormatChunk chunk in this)
            {
                Contract.Assert(chunk != null);
                builder.Append(chunk.ToString(format, formatProvider));
            }

            return builder.ToString();
        }
    }
}