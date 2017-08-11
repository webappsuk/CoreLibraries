#region © Copyright Web Applications (UK) Ltd, 2015.  All rights reserved.
// Copyright (c) 2015, Web Applications UK Ltd
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
using System.Drawing;
using System.Globalization;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Formatting;

namespace WebApplications.Utilities.Logging
{
    public sealed partial class Log
    {
        /// <summary>
        /// The verbose element format
        /// </summary>
        [NotNull]
        public static readonly FormatBuilder ElementVerboseFormat = new FormatBuilder()
            .AppendLine()
            .AppendForegroundColor(Color.Teal)
            .AppendFormat("{Key}")
            .AppendResetForegroundColor()
            .AppendFormat("\t: {Value}")
            .MakeReadOnly();

        /// <summary>
        /// The single line element format
        /// </summary>
        [NotNull]
        public static readonly FormatBuilder ElementNoLineFormat = new FormatBuilder()
            .AppendForegroundColor(Color.Teal)
            .AppendFormat("{Key}")
            .AppendResetForegroundColor()
            .AppendFormat("\t: {Value}")
            .MakeReadOnly();

        /// <summary>
        /// The XML element format
        /// </summary>
        [NotNull]
        public static readonly FormatBuilder ElementXMLFormat = new FormatBuilder()
            .AppendFormatLine("<{KeyXmlTag}>{ValueXml}</{KeyXmlTag}>")
            .MakeReadOnly();

        /// <summary>
        /// The JSON format for the first key-value pair in an object.
        /// </summary>
        [NotNull]
        public static readonly FormatBuilder ElementJSONFirstFormat = new FormatBuilder()
            .AppendFormat("\"{Key}\"={ValueJSON}")
            .MakeReadOnly();

        /// <summary>
        /// The JSON format for the rest of the key-value pairs in an object.
        /// </summary>
        [NotNull]
        public static readonly FormatBuilder ElementJSONFormat = new FormatBuilder()
            .Append(',')
            .AppendLine()
            .AppendFormat("\"{Key}\"={ValueJSON}")
            .MakeReadOnly();

        /// <summary>
        /// The HTML element format
        /// </summary>
        [NotNull]
        public static readonly FormatBuilder ElementHTMLFormat = new FormatBuilder()
            .AppendFormatLine("<tr><td>{KeyXml}</td><td>{ValueHtml}</td></tr>")
            .MakeReadOnly();

        /// <summary>
        /// Regex for line break characters.
        /// </summary>
        [NotNull]
        private static readonly Regex _newLineRegex = new Regex(@"\r\n|\r|\n", RegexOptions.Compiled);

        private class LogElement : ResolvableWriteable
        {
            /// <summary>
            /// The resource.
            /// </summary>
            [NotNull]
            public readonly Expression<Func<string>> Resource;

            /// <summary>
            /// The value.
            /// </summary>
            [CanBeNull]
            public readonly object Value;

            /// <summary>
            /// Initializes a new instance of the <see cref="LogElement" /> class.
            /// </summary>
            /// <param name="resource">The resource.</param>
            /// <param name="value">The value.</param>
            public LogElement([NotNull] Expression<Func<string>> resource, [CanBeNull] object value)
            {
                Resource = resource;
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
                    case "jsonfirst":
                        return ElementJSONFirstFormat;
                    case "html":
                        return ElementHTMLFormat;
                    case "noline":
                        return ElementNoLineFormat;
                    case "key":
                        string key = Translation.GetResource(
                            Resource,
                            context.FormatProvider as CultureInfo ?? Translation.DefaultCulture);
                        return string.IsNullOrEmpty(key)
                            ? Resolution.Null
                            : key;
                    case "keyxml":
                        string keyXml = Translation.GetResource(
                            Resource,
                            context.FormatProvider as CultureInfo ?? Translation.DefaultCulture);
                        return string.IsNullOrEmpty(keyXml)
                            ? Resolution.Null
                            : keyXml.XmlEscape();
                    case "keyxmltag":
                        string keyXmlTag = Translation.GetResource(
                            Resource,
                            context.FormatProvider as CultureInfo ?? Translation.DefaultCulture);
                        return string.IsNullOrEmpty(keyXmlTag)
                            ? Resolution.Null
                            : keyXmlTag.Replace(' ', '_');
                    case "value":
                        return Value;
                    case "valuexml":
                        return Value == null ? Resolution.Null : Value.XmlEscape();
                    case "valuehtml":
                        return Value == null ? Resolution.Null : _newLineRegex.Replace(Value.XmlEscape(), m => "<br/>" + m.Value);
                    case "valuejson":
                        return Value == null ? "null" : Value.ToString().ToJSON();
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