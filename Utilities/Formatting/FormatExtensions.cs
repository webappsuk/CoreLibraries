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

            StringBuilder chunk = new StringBuilder((int) (format.Length * 1.2));
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
            Contract.Requires(!builder.IsReadonly);
            if (builder.IsReadonly) return builder;

            if (!ConsoleHelper.IsConsole) return builder;

            ConsoleBuilder cb = builder as ConsoleBuilder;
            if (cb != null)
            {
                builder.AppendControl(FormatChunk.CreateControl("ConsoleFore"));
                builder.AppendControl(FormatChunk.CreateControl("ConsoleBack"));
            }
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
            Contract.Requires(!builder.IsReadonly);
            if (builder.IsReadonly) return builder;

            if (!ConsoleHelper.IsConsole) return builder;

            ConsoleBuilder cb = builder as ConsoleBuilder;
            if (cb != null)
                builder.AppendControl(FormatChunk.CreateControl("ConsoleFore"));
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
            Contract.Requires(!builder.IsReadonly);
            if (builder.IsReadonly) return builder;

            if (!ConsoleHelper.IsConsole) return builder;

            ConsoleBuilder cb = builder as ConsoleBuilder;
            if (cb != null)
                builder.AppendControl(FormatChunk.CreateControl("ConsoleFore", null, colour.ToString(), colour));
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
            Contract.Requires(!builder.IsReadonly);
            if (builder.IsReadonly) return builder;

            if (!ConsoleHelper.IsConsole) return builder;

            ConsoleBuilder cb = builder as ConsoleBuilder;
            if (cb != null)
                builder.AppendControl(FormatChunk.CreateControl("ConsoleFore", null, colour));
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
            Contract.Requires(!builder.IsReadonly);
            if (builder.IsReadonly) return builder;

            if (!ConsoleHelper.IsConsole) return builder;

            ConsoleBuilder cb = builder as ConsoleBuilder;
            if (cb != null)
                builder.AppendControl(FormatChunk.CreateControl("ConsoleBack"));
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
            Contract.Requires(!builder.IsReadonly);
            if (builder.IsReadonly) return builder;

            if (!ConsoleHelper.IsConsole) return builder;

            ConsoleBuilder cb = builder as ConsoleBuilder;
            if (cb != null)
                builder.AppendControl(FormatChunk.CreateControl("ConsoleBack", null, colour.ToString(), colour));
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
            Contract.Requires(!builder.IsReadonly);
            if (builder.IsReadonly) return builder;

            if (!ConsoleHelper.IsConsole) return builder;

            ConsoleBuilder cb = builder as ConsoleBuilder;
            if (cb != null)
                builder.AppendControl(FormatChunk.CreateControl("ConsoleBack", null, colour));
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
            Contract.Requires(!builder.IsReadonly);
            if (builder.IsReadonly) return builder;

            LayoutBuilder lb = builder as LayoutBuilder;
            if (lb != null)
                lb.AppendControl(
                    FormatChunk.CreateControl("Layout", null, Layout.Default.ToString("f"), Layout.Default));
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
            Contract.Requires(!builder.IsReadonly);
            if (builder.IsReadonly) return builder;

            LayoutBuilder lb = builder as LayoutBuilder;
            if (lb != null)
            {
                if (layout == null)
                    layout = Layout.Default;
                if (!layout.IsEmpty)
                    lb.AppendControl(FormatChunk.CreateControl("Layout", null, layout.ToString("f"), layout));
            }
            return builder;
        }

        /// <summary>
        /// Sets the layout (if outputting to a layout writer).
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="width">The width.</param>
        /// <param name="indentSize">Size of the indent.</param>
        /// <param name="rightMarginSize">Size of the right margin.</param>
        /// <param name="indentChar">The indent character.</param>
        /// <param name="firstLineIndentSize">First size of the line indent.</param>
        /// <param name="tabStops">The tab stops.</param>
        /// <param name="tabSize">Size of the tab.</param>
        /// <param name="tabChar">The tab character.</param>
        /// <param name="alignment">The alignment.</param>
        /// <param name="splitWords">if set to <see langword="true" /> then words will split across lines.</param>
        /// <param name="hyphenate">if set to <see langword="true" /> [hyphenate].</param>
        /// <param name="hyphenChar">The hyphenation character.</param>
        /// <param name="wrapMode">The line wrap mode.</param>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        // ReSharper disable once CodeAnnotationAnalyzer
        public static FormatBuilder SetLayout(
            [NotNull] this FormatBuilder builder,
            Optional<ushort> width = default(Optional<ushort>),
            Optional<byte> indentSize = default(Optional<byte>),
            Optional<byte> rightMarginSize = default(Optional<byte>),
            Optional<char> indentChar = default(Optional<char>),
            Optional<ushort> firstLineIndentSize = default(Optional<ushort>),
            Optional<IEnumerable<ushort>> tabStops = default(Optional<IEnumerable<ushort>>),
            Optional<byte> tabSize = default(Optional<byte>),
            Optional<char> tabChar = default(Optional<char>),
            Optional<Alignment> alignment = default(Optional<Alignment>),
            Optional<bool> splitWords = default(Optional<bool>),
            Optional<bool> hyphenate = default(Optional<bool>),
            Optional<char> hyphenChar = default(Optional<char>),
            Optional<LayoutWrapMode> wrapMode = default(Optional<LayoutWrapMode>))
        {
            Contract.Requires(!builder.IsReadonly);
            if (builder.IsReadonly) return builder;

            LayoutBuilder lb = builder as LayoutBuilder;
            if (lb != null)
            {
                Layout layout = new Layout(
                    width,
                    indentSize,
                    rightMarginSize,
                    indentChar,
                    firstLineIndentSize,
                    tabStops,
                    tabSize,
                    tabChar,
                    alignment,
                    splitWords,
                    hyphenate,
                    hyphenChar,
                    wrapMode);

                if (!layout.IsEmpty)
                    lb.AppendControl(FormatChunk.CreateControl("Layout", null, layout.ToString("f"), layout));
            }
            return builder;
        }
    }
}