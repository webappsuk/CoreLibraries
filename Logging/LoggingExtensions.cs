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
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Logging
{
    /// <summary>
    ///   Extension methods for logging.
    /// </summary>
    public static class LoggingExtensions
    {
        /// <summary>
        /// The lower-case hexadecimal digits.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly char[] HexDigitsLower = "0123456789abcdef".ToCharArray();

        /// <summary>
        /// TODO Move to utilities
        /// Unescapes the specified string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>System.String.</returns>
        [NotNull]
        [PublicAPI]
        public static string Unescape([CanBeNull] this string str)
        {
            if (string.IsNullOrEmpty(str)) return str ?? string.Empty;
            StringBuilder builder = new StringBuilder(str.Length);
            builder.AddUnescaped(str);
            return builder.ToString();
        }

        /// <summary>
        /// TODO Move to utilities
        /// Unescapes the specified string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>System.String.</returns>
        [NotNull]
        [PublicAPI]
        public static string Escape([CanBeNull] this string str)
        {
            if (string.IsNullOrEmpty(str)) return str ?? string.Empty;
            StringBuilder builder = new StringBuilder(str.Length + 10);
            builder.AddEscaped(str);
            return builder.ToString();
        }

        /// <summary>
        /// TODO Move to utilities
        /// Adds the string, removing unescaping.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="str">The string.</param>
        [PublicAPI]
        public static void AddUnescaped([NotNull] this StringBuilder builder, [CanBeNull] string str)
        {
            Contract.Requires(builder != null);
            if (string.IsNullOrEmpty(str)) return;
            int i = 0;
            bool escaped = false;
            while (i < str.Length)
            {
                char c = str[i++];
                if (!escaped)
                {
                    if (c == '\\')
                    {
                        escaped = true;
                        continue;
                    }
                    builder.Append(c);
                    continue;
                }

                escaped = false;
                switch (c)
                {
                    case '0':
                        builder.Append('\0');
                        break;
                    case 'a':
                        builder.Append('\a');
                        break;
                    case 'b':
                        builder.Append('\b');
                        break;
                    case 'f':
                        builder.Append('\f');
                        break;
                    case 'n':
                        builder.Append('\n');
                        break;
                    case 'r':
                        builder.Append('\r');
                        break;
                    case 't':
                        builder.Append('\t');
                        break;
                    case 'v':
                        builder.Append('\v');
                        break;
                    case 'u':
                        if (i + 4 > str.Length)
                        {
                            builder.Append(c);
                            continue;
                        }
                        string d4 = str.Substring(i, i + 4);
                        int n4;
                        if (!int.TryParse(d4, NumberStyles.HexNumber, null, out n4))
                        {
                            builder.Append(c);
                            continue;
                        }
                        builder.Append((Char)n4);
                        i += 4;
                        break;
                    case 'U':
                        if (i + 8 > str.Length)
                        {
                            builder.Append(c);
                            continue;
                        }
                        string d8 = str.Substring(i, i + 8);
                        int n8;
                        if (!int.TryParse(d8, NumberStyles.HexNumber, null, out n8))
                        {
                            builder.Append(c);
                            continue;
                        }
                        builder.Append(Char.ConvertFromUtf32(n8));
                        i += 8;
                        break;
                    case 'x':
                        if (i + 1 > str.Length)
                        {
                            builder.Append(c);
                            continue;
                        }
                        int j = 0;
                        StringBuilder dx = new StringBuilder(4);
                        while ((i + j) < str.Length && (j < 4))
                        {
                            char h = str[i + j++];
                            if (!HexDigitsLower.Contains(h)) break;
                            dx.Append(h);
                        }
                        int nx;
                        if ((dx.Length < 1) ||
                            !int.TryParse(dx.ToString(), NumberStyles.HexNumber, null, out nx))
                        {
                            builder.Append(c);
                            continue;
                        }
                        builder.Append((Char)nx);
                        i += j;
                        break;
                    default:
                        builder.Append(c);
                        break;
                }
            }
            if (escaped)
                builder.Append('\\');
        }

        /// <summary>
        /// TODO Move to utilities
        /// Adds the string, escaping characters.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="str">The string.</param>
        [PublicAPI]
        public static void AddEscaped([NotNull] this StringBuilder builder, [CanBeNull] string str)
        {
            Contract.Requires(builder != null);
            if (string.IsNullOrEmpty(str)) return;
            int i = 0;
            while (i < str.Length)
            {
                char c = str[i++];
                switch (c)
                {
                    case '\'':
                        builder.Append(@"\'");
                        break;
                    case '\"':
                        builder.Append("\\\"");
                        break;
                    case '\\':
                        builder.Append(@"\\");
                        break;
                    case '\0':
                        builder.Append(@"\0");
                        break;
                    case '\a':
                        builder.Append(@"\a");
                        break;
                    case '\b':
                        builder.Append(@"\b");
                        break;
                    case '\f':
                        builder.Append(@"\f");
                        break;
                    case '\n':
                        builder.Append(@"\n");
                        break;
                    case '\r':
                        builder.Append(@"\r");
                        break;
                    case '\t':
                        builder.Append(@"\t");
                        break;
                    case '\v':
                        builder.Append(@"\v");
                        break;
                    default:
                        if (Char.GetUnicodeCategory(c) != UnicodeCategory.Control)
                            builder.Append(c);
                        else
                        {
                            builder.Append(@"\u")
                                .Append(HexDigitsLower[c >> 12 & 0x0F])
                                .Append(HexDigitsLower[c >> 8 & 0x0F])
                                .Append(HexDigitsLower[c >> 4 & 0x0F])
                                .Append(HexDigitsLower[c & 0x0F]);
                        }
                        break;
                }
            }
        }

        /// <summary>
        ///   Returns a <see cref="bool"/> value indicating whether the specified
        ///   <see cref="LoggingLevel">log level</see> is within the valid
        ///   <see cref="LoggingLevels">levels</see>.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="validLevels">The valid levels.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the specified <paramref name="level"/> is within the <paramref name="validLevels"/>;
        ///   provided; otherwise returns <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid(this LoggingLevel level, LoggingLevels validLevels)
        {
            LoggingLevels l = (LoggingLevels)level;
            return l == (l & validLevels);
        }
    }
}