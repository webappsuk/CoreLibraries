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
using System.Text;
using JetBrains.Annotations;
using WebApplications.Utilities.Enumerations;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// Formattign extension methods.
    /// </summary>
    public static class FormatExtensions
    {
        /// <summary>
        /// Chunks a format string safely.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>An enumeration of tuples.</returns>
        /// <remarks><para>if <see cref="Tuple{T1,T2,T3}.Item1" /> is <see langword="null" /> then the block isn't a fill point; otherwise
        /// it contains the tag (e.g. '0')</para>
        /// <para>
        ///   <see cref="Tuple{T1,T2,T3}.Item2" /> contains the format (if any); otherwise <see langword="null" />.</para>
        /// <para>
        ///   <see cref="Tuple{T1,T2,T3}.Item3" /> contains the raw text.</para></remarks>
        [NotNull]
        [PublicAPI]
        public static IEnumerable<FormatChunk> FormatChunks([CanBeNull] this string format)
        {
            if (String.IsNullOrEmpty(format))
                yield break;

            StringBuilder chunk = new StringBuilder((int)(format.Length * 1.2));
            bool inFillPoint = false;
            int i = 0;
            while (i < format.Length)
            {
                char c = format[i++];
                if (!inFillPoint)
                {
                    if (c != FormatBuilder.OpenChar)
                    {
                        if (c != FormatBuilder.CloseChar ||
                            (chunk.Length < 1) ||
                            (chunk[chunk.Length - 1] != FormatBuilder.CloseChar))
                            chunk.Append(c);
                        continue;
                    }

                    if (chunk.Length > 0)
                    {
                        // Yield block of text.
                        yield return FormatChunk.Create(chunk.ToString());
                        chunk.Clear();
                    }

                    chunk.Append(c);
                    inFillPoint = true;
                    continue;
                }

                chunk.Append(c);
                if (c != FormatBuilder.CloseChar)
                {
                    if (c == FormatBuilder.OpenChar)
                        inFillPoint = false;
                    continue;
                }

                // Reached end of fill point
                inFillPoint = false;
                yield return FormatChunk.Create(chunk.ToString());
                chunk.Clear();
            }

            if (chunk.Length > 0)
                yield return FormatChunk.Create(chunk.ToString());
        }

        /// <summary>
        /// Adds a control to reset the console's foreground and background colours (if outputting to a console).
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        // ReSharper disable once CodeAnnotationAnalyzer
        public static FormatBuilder ConsoleResetColours([NotNull] this FormatBuilder builder)
        {
            if (!ConsoleHelper.IsConsole) return builder;

            ConsoleBuilder cb = builder as ConsoleBuilder;
            if (cb != null)
                builder.AppendControl(ConsoleColourControl.Reset);
            return builder;
        }

        /// <summary>
        /// Adds a control to reset the console's foreground colour (if outputting to a console).
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        // ReSharper disable once CodeAnnotationAnalyzer
        public static FormatBuilder ConsoleResetForeColour([NotNull] this FormatBuilder builder)
        {
            if (!ConsoleHelper.IsConsole) return builder;

            ConsoleBuilder cb = builder as ConsoleBuilder;
            if (cb != null)
                builder.AppendControl(ConsoleColourControl.ResetForeground);
            return builder;
        }

        /// <summary>
        /// Adds a control to set the console's foreground colour (if outputting to a console).
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="colour">The colour.</param>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        // ReSharper disable once CodeAnnotationAnalyzer
        public static FormatBuilder ConsoleForeColour([NotNull] this FormatBuilder builder, ConsoleColor colour)
        {
            if (!ConsoleHelper.IsConsole) return builder;

            ConsoleBuilder cb = builder as ConsoleBuilder;
            if (cb != null)
                builder.AppendControl(new ConsoleColourControl(TriState.True, colour.ToString()));
            return builder;
        }

        /// <summary>
        /// Adds a control to set the console's foreground colour (if outputting to a console).
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="colour">The colour.</param>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        // ReSharper disable once CodeAnnotationAnalyzer
        public static FormatBuilder ConsoleForeColour([NotNull] this FormatBuilder builder, [CanBeNull] string colour)
        {
            if (!ConsoleHelper.IsConsole) return builder;

            ConsoleBuilder cb = builder as ConsoleBuilder;
            if (cb != null)
                builder.AppendControl(new ConsoleColourControl(TriState.True, colour));
            return builder;
        }

        /// <summary>
        /// Adds a control to reset the console's background colour (if outputting to a console).
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        // ReSharper disable once CodeAnnotationAnalyzer
        public static FormatBuilder ConsoleResetBackColour([NotNull] this FormatBuilder builder)
        {
            if (!ConsoleHelper.IsConsole) return builder;

            ConsoleBuilder cb = builder as ConsoleBuilder;
            if (cb != null)
                builder.AppendControl(ConsoleColourControl.ResetBackground);
            return builder;
        }

        /// <summary>
        /// Adds a control to set the console's background colour (if outputting to a console).
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="colour">The colour.</param>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        // ReSharper disable once CodeAnnotationAnalyzer
        public static FormatBuilder ConsoleBackColour([NotNull] this FormatBuilder builder, ConsoleColor colour)
        {
            if (!ConsoleHelper.IsConsole) return builder;

            ConsoleBuilder cb = builder as ConsoleBuilder;
            if (cb != null)
                builder.AppendControl(new ConsoleColourControl(TriState.False, colour.ToString()));
            return builder;
        }

        /// <summary>
        /// Adds a control to set the console's background colour (if outputting to a console).
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="colour">The colour.</param>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        // ReSharper disable once CodeAnnotationAnalyzer
        public static FormatBuilder ConsoleBackColour([NotNull] this FormatBuilder builder, [CanBeNull] string colour)
        {
            if (!ConsoleHelper.IsConsole) return builder;

            ConsoleBuilder cb = builder as ConsoleBuilder;
            if (cb != null)
                builder.AppendControl(new ConsoleColourControl(TriState.False, colour));
            return builder;
        }

        /// <summary>
        /// Resets the layout (if outputting to a layout writer).
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        // ReSharper disable once CodeAnnotationAnalyzer
        public static FormatBuilder ResetLayout([NotNull] this FormatBuilder builder)
        {
            LayoutBuilder lb = builder as LayoutBuilder;
            if (lb != null)
                lb.AppendControl(Layout.Default);
            return builder;
        }

        /// <summary>
        /// Sets the layout (if outputting to a layout writer).
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="layout">The layout.</param>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        // ReSharper disable once CodeAnnotationAnalyzer
        public static FormatBuilder SetLayout([NotNull] this FormatBuilder builder, [CanBeNull] Layout layout)
        {
            LayoutBuilder lb = builder as LayoutBuilder;
            if (lb != null)
                lb.AppendControl(layout ?? Layout.Default);
            return builder;
        }
    }
}