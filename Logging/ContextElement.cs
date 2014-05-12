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
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using JetBrains.Annotations;
using WebApplications.Utilities.Formatting;

namespace WebApplications.Utilities.Logging
{
    public partial class LogContext
    {
        /// <summary>
        /// The default format
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly FormatBuilder ElementVerboseFormat = new FormatBuilder()
            .AppendLine()
            .AppendForegroundColor(Color.DarkCyan)
            .AppendFormat("{Key}")
            .AppendResetForegroundColor()
            .AppendFormat("\t: {Value}")
            .MakeReadOnly();

        /// <summary>
        /// The default format
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly FormatBuilder ElementNoLineFormat = new FormatBuilder()
            .AppendForegroundColor(Color.DarkCyan)
            .AppendFormat("{Key}")
            .AppendResetForegroundColor()
            .AppendFormat("\t: {Value}")
            .MakeReadOnly();

        /// <summary>
        /// The default format
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly FormatBuilder ElementXMLFormat = new FormatBuilder()
            .AppendFormatLine("<{KeyXmlTag}>{Value}</{KeyXmlTag}>")
            .MakeReadOnly();

        /// <summary>
        /// The default format
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly FormatBuilder ElementJSONFormat = new FormatBuilder()
            .Append(',')
            .AppendLine()
            .AppendFormat("\"{Key}\"=\"{Value}\"")
            .MakeReadOnly();

        private class ContextElement : ResolvableWriteable
        {
            /// <summary>
            /// The resource.
            /// </summary>
            [NotNull]
            [PublicAPI]
            public readonly string Key;

            /// <summary>
            /// The value.
            /// </summary>
            [CanBeNull]
            [PublicAPI]
            public readonly string Value;

            /// <summary>
            /// Initializes a new instance of the <see cref="ContextElement" /> class.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <param name="value">The value.</param>
            public ContextElement([NotNull] string key, [CanBeNull] string value)
            {
                Contract.Requires(key != null);
                Key = key;
                Value = value;
            }

            /// <summary>
            /// Resolves the specified tag.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="chunk">The chunk.</param>
            /// <returns>An object that will be cached unless it is a <see cref="T:WebApplications.Utilities.Formatting.Resolution" />.</returns>
            // ReSharper disable once CodeAnnotationAnalyzer
            public override object Resolve(FormatWriteContext context, FormatChunk chunk)
            {
                // ReSharper disable once PossibleNullReferenceException
                switch (chunk.Tag.ToLowerInvariant())
                {
                    case "default":
                    case "verbose":
                        return ElementVerboseFormat;
                    case "xml":
                        return ElementXMLFormat;
                    case "json":
                        return ElementJSONFormat;
                    case "noline":
                        return ElementNoLineFormat;
                    case "key":
                        return Key;
                    case "keyxml":
                        return Key.XmlEscape();
                    case "keyxmltag":
                        return Key.Replace(' ', '_');
                    case "value":
                        return Value;
                    case "valuexml":
                        return Value.XmlEscape();
                    default:
                        return Resolution.Unknown;
                }
            }

            /// <summary>
            /// Gets the default format.
            /// </summary>
            /// <value>The default format.</value>
            /// <exception cref="System.NotImplementedException"></exception>
            public override FormatBuilder DefaultFormat
            {
                get { return ElementVerboseFormat; }
            }
        }
    }
}