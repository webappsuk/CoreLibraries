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
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using JetBrains.Annotations;
using WebApplications.Utilities.Formatting;

namespace WebApplications.Utilities.Logging
{
    public sealed partial class Log
    {
        /// <summary>
        /// The default format
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly FormatBuilder EnumerableElementVerboseFormat = new FormatBuilder()
            .AppendLine()
            .AppendForegroundColor(Color.DarkCyan)
            .AppendFormat("{Key}")
            .AppendResetForegroundColor()
            .AppendFormat("\t: {Value:{<items>:{<item>}}{<join>:, }}")
            .MakeReadOnly();

        /// <summary>
        /// The default format
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly FormatBuilder EnumerableElementNoLineFormat = new FormatBuilder()
            .AppendForegroundColor(Color.DarkCyan)
            .AppendFormat("{Key}")
            .AppendResetForegroundColor()
            .AppendFormat("\t: {Value:{<items>:{<item>}}{<join>:, }}")
            .MakeReadOnly();

        /// <summary>
        /// The default format
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly FormatBuilder EnumerableElementXMLFormat = new FormatBuilder()
            .AppendLine()
            .AppendFormat("<{Key}>{Value:{<items>:<item>{<item}</item>}}</{Key}>")
            .MakeReadOnly();

        /// <summary>
        /// The default format
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly FormatBuilder EnumerableElementJSONFormat = new FormatBuilder()
            .Append(',')
            .AppendLine()
            .AppendFormat("\"{Key}\"={Value:{<items>:\"{<item>}\"}{<join>:, }}")
            .MakeReadOnly();

        private class LogEnumerableElement : ResolvableWriteable
        {
            /// <summary>
            /// The resource.
            /// </summary>
            [NotNull]
            [PublicAPI]
            public readonly Expression<Func<string>> Resource;

            /// <summary>
            /// The value.
            /// </summary>
            [CanBeNull]
            [PublicAPI]
            public readonly IEnumerable<object> Values;

            /// <summary>
            /// Initializes a new instance of the <see cref="LogElement" /> class.
            /// </summary>
            /// <param name="resource">The resource.</param>
            /// <param name="values">The values.</param>
            public LogEnumerableElement(
                [NotNull] Expression<Func<string>> resource,
                [CanBeNull] IEnumerable<object> values)
            {
                Contract.Requires(resource != null);
                Resource = resource;
                Values = values;
            }

            /// <summary>
            /// Resolves the specified tag.
            /// </summary>
            /// <param name="writer">The writer.</param>
            /// <param name="chunk">The chunk.</param>
            /// <returns>A <see cref="T:WebApplications.Utilities.Formatting.Resolution" />.</returns>
            // ReSharper disable once CodeAnnotationAnalyzer
            public override Resolution Resolve(TextWriter writer, FormatChunk chunk)
            {
                // ReSharper disable once PossibleNullReferenceException
                switch (chunk.Tag.ToLowerInvariant())
                {
                    case "default":
                    case "verbose":
                        return new Resolution(EnumerableElementVerboseFormat);
                    case "xml":
                        return new Resolution(EnumerableElementXMLFormat);
                    case "json":
                        return new Resolution(EnumerableElementJSONFormat);
                    case "noline":
                        return new Resolution(EnumerableElementNoLineFormat);
                    case "key":
                        string key = Translation.GetResource(Resource, writer.FormatProvider as CultureInfo ?? Translation.DefaultCulture);
                        return string.IsNullOrEmpty(key) ? Resolution.Null : new Resolution(key);
                    case "value":
                        return new Resolution(Values);
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
                get { return EnumerableElementVerboseFormat; }
            }
        }
    }
}