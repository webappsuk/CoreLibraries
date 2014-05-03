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
using System.Data.SqlClient;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
using ProtoBuf;
using ProtoBuf.Meta;
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
        public readonly static FormatBuilder ElementDefaultFormat = new FormatBuilder()
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
        public readonly static FormatBuilder ElementNoLineFormat = new FormatBuilder()
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
        public readonly static FormatBuilder ElementXMLFormat = new FormatBuilder()
            .AppendLine()
            .AppendFormat("<{Key}>{Value}</{Key}>")
            .MakeReadOnly();

        /// <summary>
        /// The default format
        /// </summary>
        [NotNull]
        [PublicAPI]
        public readonly static FormatBuilder ElementJSONFormat = new FormatBuilder()
            .Append(',')
            .AppendLine()
            .AppendFormat("\"{Key}\"=\"{Value}\"")
            .MakeReadOnly();

        private class LogElement : ResolvableWriteable
        {
            /// <summary>
            /// The resource.
            /// </summary>
            [NotNull]
            [PublicAPI]
            public readonly Expression<Func<string>> Resource;

            /// <summary>
            /// The culture. // TODO Get during resolve.
            /// </summary>
            [CanBeNull]
            [PublicAPI]
            public readonly CultureInfo Culture;

            /// <summary>
            /// The value.
            /// </summary>
            [CanBeNull]
            [PublicAPI]
            public readonly object Value;

            /// <summary>
            /// Initializes a new instance of the <see cref="LogElement"/> class.
            /// </summary>
            /// <param name="format">The format.</param>
            /// <param name="culture">The culture.</param>
            /// <param name="resource">The resource.</param>
            /// <param name="value">The value.</param>
            public LogElement(
            [CanBeNull]CultureInfo culture,
            [NotNull] Expression<Func<string>> resource,
            [CanBeNull] object value)
            {
                Contract.Requires(resource != null);
                Culture = culture;
                Resource = resource;
                Value = value;
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
                        return new Resolution(ElementDefaultFormat);
                    case "xml":
                        return new Resolution(ElementXMLFormat);
                    case "json":
                        return new Resolution(ElementJSONFormat);
                    case "noline":
                        return new Resolution(ElementNoLineFormat);
                    case "key":
                        string key = Translation.GetResource(Resource, Culture);
                        return string.IsNullOrEmpty(key) ? Resolution.Null : new Resolution(key);
                    case "value":
                        return new Resolution(Value);
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
                get { return ElementDefaultFormat; }
            }
        }
    }
}