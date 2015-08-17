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
using System.Collections.Generic;
using System.IO;
using System.Text;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// Formattign extension methods.
    /// </summary>
    [PublicAPI]
    public static class FormatExtensions
    {
        /// <summary>
        /// A safe <see cref="string" /> format.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="parameters">The values used in the format string.</param>
        /// <returns>
        /// Returns the formatted <see cref="string" /> if successful; otherwise returns the <paramref name="format" /> string.
        /// </returns>
        [ContractAnnotation("format:null=>null;format:notnull=>notnull")]
        [StringFormatMethod("format")]
        public static string SafeFormat([CanBeNull] this string format, [CanBeNull] params object[] parameters)
        {
            if (format == null) return null;
            if (parameters == null || parameters.Length < 1)
                return format;

            return TryFormat(format, null, parameters);
        }

        /// <summary>
        /// A safe <see cref="string" /> format.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="parameters">The values used in the format string.</param>
        /// <returns>
        /// Returns the formatted <see cref="string" /> if successful; otherwise returns the <paramref name="format" /> string.
        /// </returns>
        [ContractAnnotation("format:null=>null;format:notnull=>notnull")]
        [StringFormatMethod("format")]
        public static string SafeFormat([CanBeNull] this string format, [CanBeNull] IFormatProvider formatProvider, [CanBeNull] params object[] parameters)
        {
            if (format == null) return null;
            if (parameters == null || parameters.Length < 1)
                return format;

            return TryFormat(format, formatProvider, parameters);
        }

        /// <summary>
        /// A safe <see cref="string" /> format.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="formatOptions">The format options string. <seealso cref="FormatBuilder.ToString(String, IFormatProvider)"/></param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="parameters">The values used in the format string.</param>
        /// <returns>Returns the formatted <see cref="string" /> if successful; otherwise returns the <paramref name="format" /> string.</returns>
        [ContractAnnotation("format:null=>null;format:notnull=>notnull")]
        [StringFormatMethod("format")]
        public static string SafeFormat(
            [CanBeNull] this string format,
            [CanBeNull] string formatOptions,
            [CanBeNull] IFormatProvider formatProvider,
            [CanBeNull] params object[] parameters)
        {
            if (format == null) return null;
            if (parameters == null || parameters.Length < 1)
                return format;

            return new FormatBuilder().AppendFormat(format, parameters).ToString(formatOptions, formatProvider);
        }

        /// <summary>
        /// Attempts to replace the format item in a specified string with the string representation of a corresponding object in a specified array.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="provider">The provider.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>
        /// A copy of <paramref name="format" /> in which the format items have been replaced by the string representation of the corresponding objects in <paramref name="args" />,
        /// or <see langword="null"/> if the format string was invalid.
        /// </returns>
        // NOTE: Same implementation as the base string.Format, but throws replaced with returns.
        private static string TryFormat(string format, IFormatProvider provider, params object[] args)
        {
            if (format == null || args == null) return null;

            int pos = 0;
            int len = format.Length;

            ICustomFormatter cf = null;
            if (provider != null)
                cf = (ICustomFormatter)provider.GetFormat(typeof(ICustomFormatter));

            StringBuilder sb = new StringBuilder(format.Length + args.Length * 10);

            while (true)
            {
                char ch;
                while (pos < len)
                {
                    ch = format[pos];

                    pos++;
                    if (ch == '}')
                    {
                        if (pos < len && format[pos] == '}') // Treat as escape character for }}
                            pos++;
                        else
                            return null;
                    }

                    if (ch == '{')
                    {
                        if (pos < len && format[pos] == '{') // Treat as escape character for {{
                            pos++;
                        else
                        {
                            pos--;
                            break;
                        }
                    }

                    sb.Append(ch);
                }

                if (pos == len) break;
                pos++;
                if (pos == len || (ch = format[pos]) < '0' || ch > '9') return null;

                int index = 0;
                do
                {
                    index = index * 10 + ch - '0';
                    pos++;
                    if (pos == len) return null;
                    ch = format[pos];
                } while (ch >= '0' && ch <= '9' && index < 1000000);

                if (index >= args.Length) return null;
                while (pos < len && (ch = format[pos]) == ' ') pos++;
                bool leftJustify = false;
                int width = 0;
                if (ch == ',')
                {
                    pos++;
                    while (pos < len && format[pos] == ' ') pos++;

                    if (pos == len) return null;
                    ch = format[pos];
                    if (ch == '-')
                    {
                        leftJustify = true;
                        pos++;
                        if (pos == len) return null;
                        ch = format[pos];
                    }

                    if (ch < '0' || ch > '9') return null;
                    do
                    {
                        width = width * 10 + ch - '0';
                        pos++;
                        if (pos == len) return null;
                        ch = format[pos];
                    } while (ch >= '0' && ch <= '9' && width < 1000000);
                }

                while (pos < len && (ch = format[pos]) == ' ') pos++;
                object arg = args[index];
                StringBuilder fmt = null;

                if (ch == ':')
                {
                    pos++;
                    while (true)
                    {
                        if (pos == len) return null;
                        ch = format[pos];
                        pos++;
                        if (ch == '{')
                        {
                            if (pos < len && format[pos] == '{')  // Treat as escape character for {{
                                pos++;
                            else
                                return null;
                        }
                        else if (ch == '}')
                        {
                            if (pos < len && format[pos] == '}')  // Treat as escape character for }}
                                pos++;
                            else
                            {
                                pos--;
                                break;
                            }
                        }

                        if (fmt == null)
                            fmt = new StringBuilder();
                        fmt.Append(ch);
                    }
                }

                if (ch != '}') return null;
                pos++;
                string sFmt = null;
                string s = null;

                if (cf != null)
                {
                    if (fmt != null)
                        sFmt = fmt.ToString();
                    s = cf.Format(sFmt, arg, provider);
                }

                if (s == null)
                {
                    IFormattable formattableArg = arg as IFormattable;

                    if (formattableArg != null)
                    {
                        if (sFmt == null && fmt != null)
                        {
                            sFmt = fmt.ToString();
                        }

                        s = formattableArg.ToString(sFmt, provider);
                    }
                    else if (arg != null)
                    {
                        s = arg.ToString();
                    }
                }

                if (s == null) s = string.Empty;
                int pad = width - s.Length;
                if (!leftJustify && pad > 0) sb.Append(' ', pad);
                sb.Append(s);
                if (leftJustify && pad > 0) sb.Append(' ', pad);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Produces a serialized version of the specified writer, using <see cref="SerializingSynchronizationContext" />, that
        /// will write output serially.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <returns>A synchronized TextWriter.</returns>
        [NotNull]
        public static TextWriter Serialize([NotNull] this TextWriter writer)
        {
            if (writer == null) throw new ArgumentNullException("writer");

            ISerialTextWriter stw = writer as ISerialTextWriter;
            return stw != null
                ? writer
                : new SerialTextWriter(writer);
        }

        /// <summary>
        /// Produces a formatted version of the specified writer, using <see cref="FormatTextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="layout">The layout.</param>
        /// <param name="startPosition">The start position.</param>
        /// <returns>A laid out TextWriter.</returns>
        [NotNull]
        public static FormatTextWriter Format(
            [NotNull] this TextWriter writer,
            [CanBeNull] Layout layout = null,
            ushort startPosition = 0)
        {
            if (writer == null) throw new ArgumentNullException("writer");

            FormatTextWriter ltw = writer as FormatTextWriter;
            if (ltw == null) return new FormatTextWriter(writer, layout, startPosition);

            ltw.ApplyLayout(layout);
            return ltw;
        }

        /// <summary>
        /// Produces a formatted version of the specified writer, using <see cref="FormatTextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
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
        /// <param name="startPosition">The start position.</param>
        /// <returns>A laid out TextWriter.</returns>
        [NotNull]
        public static FormatTextWriter Format(
            [NotNull] this TextWriter writer,
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
            ushort startPosition = 0)
        {
            if (writer == null) throw new ArgumentNullException("writer");

            FormatTextWriter ltw = writer as FormatTextWriter;
            if (ltw == null)
                return new FormatTextWriter(
                    writer,
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
                    wrapMode,
                    startPosition);

            ltw.ApplyLayout(
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
            return ltw;
        }
    }
}