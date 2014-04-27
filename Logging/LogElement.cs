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
using System.Drawing;
using JetBrains.Annotations;
using WebApplications.Utilities.Formatting;

namespace WebApplications.Utilities.Logging
{
    public sealed partial class Log
    {
        /// <summary>
        /// A formattable object for each element of a log.
        /// </summary>
        private class LogElement : IFormattable
        {
            /// <summary>
            /// The key format tag.
            /// </summary>
            [NotNull]
            [PublicAPI]
            public const string KeyFormatTag = "key";

            /// <summary>
            /// The value format tag.
            /// </summary>
            [NotNull]
            [PublicAPI]
            public const string ValueFormatTag = "value";

            /// <summary>
            /// The key resource.
            /// </summary>
            [NotNull]
            [PublicAPI]
            public readonly string Key;

            /// <summary>
            /// The value
            /// </summary>
            [NotNull]
            [PublicAPI]
            public readonly object Value;

            /// <summary>
            /// Initializes a new instance of the <see cref="LogElement" /> class.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <param name="value">The value.</param>
            public LogElement([NotNull] string key, [NotNull] object value)
            {
                Contract.Requires(key != null);
                Contract.Requires(value != null);
                Key = key;
                Value = value;
            }

            /// <summary>
            /// The default format
            /// </summary>
            [NotNull]
            [PublicAPI]
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public readonly FormatBuilder DefaultFormat = new FormatBuilder()
                .AppendForegroundColor(Color.Cyan)
                .AppendFormat("{Key}")
                .AppendResetForegroundColor()
                .AppendFormat("\t: {Value}")
                .MakeReadOnly();

            /// <summary>
            /// To the string.
            /// </summary>
            /// <returns>System.String.</returns>
            public override string ToString()
            {
                return ToString(DefaultFormat);
            }

            /// <summary>
            /// Returns a <see cref="System.String"/> that represents this instance.
            /// </summary>
            /// <param name="format">The format to use.-or- A null reference (Nothing in Visual Basic) to use the default format defined for the type of the <see cref="T:System.IFormattable"/> implementation.</param>
            /// <param name="formatProvider">The provider to use to format the value.-or- A null reference (Nothing in Visual Basic) to obtain the numeric format information from the current locale setting of the operating system.</param>
            /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
            /// <exception cref="System.NotImplementedException"></exception>
            public virtual string ToString([CanBeNull] string format, [CanBeNull] IFormatProvider formatProvider = null)
            {
                return ToString(
                    (FormatBuilder) format,
                    formatProvider);
            }

            /// <summary>
            /// Returns a <see cref="System.String"/> that represents this instance.
            /// </summary>
            /// <param name="format">The format to use.-or- A null reference (Nothing in Visual Basic) to use the default format defined for the type.</param>
            /// <param name="formatProvider">The provider to use to format the value.-or- A null reference (Nothing in Visual Basic) to obtain the numeric format information from the current locale setting of the operating system.</param>
            /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
            /// <exception cref="System.NotImplementedException"></exception>
            [NotNull]
            [PublicAPI]
            public string ToString([CanBeNull] FormatBuilder format, [CanBeNull] IFormatProvider formatProvider = null)
            {
                if (format == null)
                    format = DefaultFormat;

                return format.ToString(
                    formatProvider,
                    chunk =>
                    {
                        if (chunk == null ||
                            !chunk.IsFillPoint ||
                            chunk.IsControl) return Optional<object>.Unassigned;

                        // ReSharper disable once PossibleNullReferenceException
                        switch (chunk.Tag.ToLower())
                        {
                            case KeyFormatTag:
                                return Key;
                            case ValueFormatTag:
                                return Value;
                            default:
                                return Optional<object>.Unassigned;
                        }
                    });
            }
        }
    }
}