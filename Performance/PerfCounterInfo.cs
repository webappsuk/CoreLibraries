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
using JetBrains.Annotations;
using WebApplications.Utilities.Formatting;

namespace WebApplications.Utilities.Performance
{
    /// <summary>
    /// Provides read-only access to a <see cref="PerfCategory">PerfCategory's</see> information.
    /// </summary>
    public abstract class PerfCounterInfo : IFormattable
    {
        /// <summary>
        /// The value type
        /// </summary>
        [NotNull]
        [PublicAPI]
        public readonly Type ValueType;

        /// <summary>
        /// The name
        /// </summary>
        [NotNull]
        [PublicAPI]
        public readonly string Name;

        /// <summary>
        /// The help
        /// </summary>
        [NotNull]
        [PublicAPI]
        public readonly string Help;

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        [CanBeNull]
        [PublicAPI]
        public abstract object Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerfCounterInfo" /> class.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <param name="name">The name.</param>
        /// <param name="help">The help.</param>
        protected PerfCounterInfo([NotNull] Type valueType, [NotNull] string name, [NotNull] string help)
        {
            Contract.Requires(valueType != null);
            Contract.Requires(name != null);
            Contract.Requires(help != null);
            ValueType = valueType;
            Name = name;
            Help = help;
        }

        /// <summary>
        /// The default builder for writing out information.
        /// </summary>
        [NotNull]
        private static readonly FormatBuilder _defaultBuilder =
            new FormatBuilder(firstLineIndentSize: 0, indentSize: 0)
                .AppendFormatLine("{Name}\t: {Value}");

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return ToString(_defaultBuilder);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format to use.-or- A null reference (Nothing in Visual Basic) to use the default format defined for the type of the <see cref="T:System.IFormattable" /> implementation.</param>
        /// <param name="formatProvider">The provider to use to format the value.-or- A null reference (Nothing in Visual Basic) to obtain the numeric format information from the current locale setting of the operating system.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public string ToString([CanBeNull] string format, [CanBeNull] IFormatProvider formatProvider = null)
        {
            return ToString(
                string.IsNullOrEmpty(format) ? _defaultBuilder : new FormatBuilder().AppendFormat(format),
                formatProvider);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="formatProvider">The provider to use to format the value.-or- A null reference (Nothing in Visual Basic) to obtain the numeric format information from the current locale setting of the operating system.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        [NotNull]
        [PublicAPI]
        public string ToString([CanBeNull] FormatBuilder builder, [CanBeNull] IFormatProvider formatProvider = null)
        {
            if (builder == null)
                builder = _defaultBuilder;

            return builder
                .ToString(
                    null,
                    formatProvider,
                    chunk =>
                    {
                        Contract.Assert(chunk != null);
                        Contract.Assert(!string.IsNullOrWhiteSpace(chunk.Tag));

                        if (chunk.IsControl)
                            return Optional<object>.Unassigned;
                        // ReSharper disable once PossibleNullReferenceException
                        switch (chunk.Tag.ToLowerInvariant())
                        {
                            case "name":
                                return Name;
                            case "help":
                                return Help;
                            case "value":
                                return Value;
                            default:
                                return Optional<object>.Unassigned;
                        }
                    });
        }
    }

    /// <summary>
    /// Provides read-only access to a <see cref="PerfCategory">PerfCategory's</see> information.
    /// </summary>
    [PublicAPI]
    public class PerfCounterInfo<T> : PerfCounterInfo
    {
        [NotNull]
        private readonly Func<T> _getLatestValueFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="PerfCounterInfo{T}"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="help">The help.</param>
        /// <param name="getLatestValueFunc">The function to get the latest value.</param>
        public PerfCounterInfo([NotNull] string name, [NotNull] string help, [NotNull] Func<T> getLatestValueFunc)
            : base(typeof (T), name, help)
        {
            Contract.Requires(name != null);
            Contract.Requires(help != null);
            Contract.Requires(getLatestValueFunc != null);
            _getLatestValueFunc = getLatestValueFunc;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public override object Value
        {
            get { return _getLatestValueFunc(); }
        }
    }
}