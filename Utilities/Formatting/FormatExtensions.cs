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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Odbc;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web.SessionState;
using JetBrains.Annotations;
using WebApplications.Utilities.Threading;

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
        /// <remarks>To insert a '{', '}' or '\' character in the format, then it should be preceded with a '\' character.
        /// A '\' before any other character will be ignored. Escapes within a fill point will be kept in case the chunks format is another format string.</remarks>
        [NotNull]
        [PublicAPI]
        public static IEnumerable<FormatChunk> FormatChunks([CanBeNull] this string format)
        {
            if (String.IsNullOrEmpty(format))
                yield break;

            StringBuilder chunk = new StringBuilder(format.Length);

            bool inFillPoint = false;

            int openCount = 0;
            int i = 0;

            while (i < format.Length)
            {
                char c = format[i++];

                if (c == '\\')
                {
                    if (i < format.Length)
                    {
                        if (inFillPoint)
                            chunk.Append(c);
                        chunk.Append(format[i++]);
                    }
                    else
                        chunk.Append(c);
                    continue;
                }

                if (!inFillPoint)
                {
                    if (c == FormatBuilder.OpenChar)
                    {
                        inFillPoint = true;

                        if (chunk.Length > 0)
                        {
                            // Yield block of text.
                            yield return FormatChunk.Create(chunk.ToString());
                            chunk.Clear();
                        }
                    }

                    chunk.Append(c);
                    continue;
                }

                chunk.Append(c);

                if (c == FormatBuilder.OpenChar)
                    openCount++;
                else if (c == FormatBuilder.CloseChar)
                {
                    if (openCount == 0)
                    {
                        // Reached end of fill point
                        inFillPoint = false;
                        yield return FormatChunk.Create(chunk.ToString());
                        chunk.Clear();
                    }
                    else
                        openCount--;
                }
            }

            if (chunk.Length > 0)
                yield return FormatChunk.Create(chunk.ToString());
        }

        #region Color Control
        /// <summary>
        /// The reset colors control tag.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public const string ResetColorsTag = "!resetcolors";

        /// <summary>
        /// The reset colors chunk.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly FormatChunk ResetColorsChunk = FormatChunk.CreateControl(ResetColorsTag);

        /// <summary>
        /// The foreground color control tag.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public const string ForegroundColorTag = "!fgcolor";

        /// <summary>
        /// The background color control tag.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public const string BackgroundColorTag = "!bgcolor";

        /// <summary>
        /// The reset foreground color chunk.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly FormatChunk ResetForegroundColorChunk = FormatChunk.CreateControl(ForegroundColorTag);

        /// <summary>
        /// The reset background color chunk.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly FormatChunk ResetBackgroundColorChunk = FormatChunk.CreateControl(BackgroundColorTag);

        /// <summary>
        /// Adds a control to reset the foreground and background colors
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        // ReSharper disable once CodeAnnotationAnalyzer
        public static FormatBuilder AppendResetColors([NotNull] this FormatBuilder builder)
        {
            Contract.Requires(!builder.IsReadonly);
            return builder.AppendControl(ResetColorsChunk);
        }

        /// <summary>
        /// Adds a control to reset the foreground color.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        // ReSharper disable once CodeAnnotationAnalyzer
        public static FormatBuilder AppendResetForegroundColor([NotNull] this FormatBuilder builder)
        {
            Contract.Requires(!builder.IsReadonly);
            return builder.AppendControl(ResetForegroundColorChunk);
        }

        /// <summary>
        /// Adds a control to reset the background color.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        // ReSharper disable once CodeAnnotationAnalyzer
        public static FormatBuilder AppendResetBackgroundColor([NotNull] this FormatBuilder builder)
        {
            Contract.Requires(!builder.IsReadonly);
            return builder.AppendControl(ResetBackgroundColorChunk);
        }

        /// <summary>
        /// Adds a control to set the foreground color.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="color">The color.</param>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        // ReSharper disable once CodeAnnotationAnalyzer
        public static FormatBuilder AppendForegroundColor([NotNull] this FormatBuilder builder, ConsoleColor color)
        {
            Contract.Requires(!builder.IsReadonly);
            Color c = color.ToColor();
            return builder.AppendControl(FormatChunk.CreateControl(ForegroundColorTag, null, c.GetName(), c));
        }

        /// <summary>
        /// Adds a control to set the foreground color.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="color">The color.</param>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        // ReSharper disable once CodeAnnotationAnalyzer
        public static FormatBuilder AppendForegroundColor([NotNull] this FormatBuilder builder, Color color)
        {
            Contract.Requires(!builder.IsReadonly);
            return builder.AppendControl(FormatChunk.CreateControl(ForegroundColorTag, null, color.GetName(), color));
        }

        /// <summary>
        /// Adds a control to set the foreground color.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="color">The color.</param>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        // ReSharper disable once CodeAnnotationAnalyzer
        public static FormatBuilder AppendForegroundColor([NotNull] this FormatBuilder builder, [CanBeNull] string color)
        {
            Contract.Requires(!builder.IsReadonly);
            Optional<Color> c = ColorHelper.GetColor(color);
            return !c.IsAssigned
                ? builder
                : builder.AppendControl(FormatChunk.CreateControl(ForegroundColorTag, null, c.Value.GetName(), c.Value));
        }

        /// <summary>
        /// Adds a control to set the background color.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="color">The color.</param>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        // ReSharper disable once CodeAnnotationAnalyzer
        public static FormatBuilder AppendBackgroundColor([NotNull] this FormatBuilder builder, ConsoleColor color)
        {
            Contract.Requires(!builder.IsReadonly);
            Color c = color.ToColor();
            return builder.AppendControl(FormatChunk.CreateControl(BackgroundColorTag, null, c.GetName(), c));
        }

        /// <summary>
        /// Adds a control to set the background color.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="color">The color.</param>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        // ReSharper disable once CodeAnnotationAnalyzer
        public static FormatBuilder AppendBackgroundColor([NotNull] this FormatBuilder builder, Color color)
        {
            Contract.Requires(!builder.IsReadonly);
            return builder.AppendControl(FormatChunk.CreateControl(BackgroundColorTag, null, color.GetName(), color));
        }

        /// <summary>
        /// Adds a control to set the console's background color.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="color">The color.</param>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        // ReSharper disable once CodeAnnotationAnalyzer
        public static FormatBuilder AppendBackgroundColor([NotNull] this FormatBuilder builder, [CanBeNull] string color)
        {
            Contract.Requires(!builder.IsReadonly);
            Optional<Color> c = ColorHelper.GetColor(color);
            return !c.IsAssigned
                ? builder
                : builder.AppendControl(FormatChunk.CreateControl(BackgroundColorTag, null, c.Value.GetName(), c.Value));
        }

        /// <summary>
        /// Sets the color based on the <see paramref="chunk" />
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="chunk">The chunk.</param>
        /// <returns><see langword="true" /> if the <see paramref="chunk"/> was a color control, <see langword="false" /> otherwise.</returns>
        [PublicAPI]
        public static bool SetColor([NotNull] this IColoredTextWriter writer, [NotNull] FormatChunk chunk)
        {
            Contract.Requires(writer != null);
            Contract.Requires(chunk != null);
            Contract.Requires(chunk.IsControl);
            // ReSharper disable once PossibleNullReferenceException
            switch (chunk.Tag.ToLowerInvariant())
            {
                case ResetColorsTag:
                    writer.ResetColors();
                    return true;
                case ForegroundColorTag:

                    if (string.IsNullOrWhiteSpace(chunk.Format))
                        writer.ResetForegroundColor();
                    else if (chunk.Value is Color)
                        writer.SetForegroundColor((Color) chunk.Value);
                    else
                    {
                        // ReSharper disable once AssignNullToNotNullAttribute
                        Optional<Color> color = ColorHelper.GetColor(chunk.Format);
                        if (color.IsAssigned)
                            writer.SetForegroundColor(color.Value);
                    }
                    return true;
                case BackgroundColorTag:

                    if (string.IsNullOrWhiteSpace(chunk.Format))
                        writer.ResetBackgroundColor();
                    else if (chunk.Value is Color)
                        writer.SetBackgroundColor((Color) chunk.Value);
                    else
                    {
                        // ReSharper disable once AssignNullToNotNullAttribute
                        Optional<Color> color = ColorHelper.GetColor(chunk.Format);
                        if (color.IsAssigned)
                            writer.SetBackgroundColor(color.Value);
                    }
                    return true;
                default:
                    return false;
            }
        }
        #endregion

        #region Layout Control
        /// <summary>
        /// The layout control tag.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public const string LayoutTag = "!layout";

        /// <summary>
        /// The reset layout chunk.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly FormatChunk ResetLayoutChunk = FormatChunk.CreateControl(LayoutTag);

        /// <summary>
        /// Resets the layout.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>FormatBuilder.</returns>
        [NotNull]
        [PublicAPI]
        // ReSharper disable once CodeAnnotationAnalyzer
        public static FormatBuilder AppendResetLayout([NotNull] this FormatBuilder builder)
        {
            Contract.Requires(!builder.IsReadonly);
            return builder.Append(ResetLayoutChunk);
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
        public static FormatBuilder AppendLayout([NotNull] this FormatBuilder builder, [CanBeNull] Layout layout)
        {
            Contract.Requires(!builder.IsReadonly);
            return builder.Append(FormatChunk.CreateControl(LayoutTag, null, layout.ToString("f"), layout));
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
        public static FormatBuilder AppendLayout(
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
            return builder.Append(FormatChunk.CreateControl(LayoutTag, null, layout.ToString("f"), layout));
        }
        #endregion

        /// <summary>
        /// Produces a serialized version of the specified writer, using <see cref="SerializingSynchronizationContext" />, that
        /// will write output serially.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <returns>A synchronized TextWriter.</returns>
        [NotNull]
        [PublicAPI]
        public static TextWriter Serialize([NotNull] this TextWriter writer)
        {
            Contract.Requires(writer != null);
            Contract.Ensures(Contract.Result<TextWriter>() != null);
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
        [PublicAPI]
        public static FormatTextWriter Format([NotNull] this TextWriter writer, [CanBeNull] Layout layout = null, ushort startPosition = 0)
        {
            Contract.Requires(writer != null);
            Contract.Ensures(Contract.Result<TextWriter>() != null);

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
        /// <param name="splitWords">The split words.</param>
        /// <param name="hyphenate">The hyphenate.</param>
        /// <param name="hyphenChar">The hyphen character.</param>
        /// <param name="wrapMode">The wrap mode.</param>
        /// <param name="startPosition">The start position.</param>
        /// <returns>A laid out TextWriter.</returns>
        [NotNull]
        [PublicAPI]
        public static FormatTextWriter Format(
            [NotNull] this TextWriter writer,
            Optional<ushort> width,
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
            Optional<LayoutWrapMode> wrapMode = default(Optional<LayoutWrapMode>),
            ushort startPosition = 0)
        {
            Contract.Requires(writer != null);
            Contract.Ensures(Contract.Result<TextWriter>() != null);

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
                    splitWords,
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
                splitWords,
                hyphenate,
                hyphenChar,
                wrapMode);
            return ltw;
        }
    }
}