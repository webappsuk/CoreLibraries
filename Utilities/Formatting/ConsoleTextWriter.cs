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
using System.IO;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// Implements a <see cref="TextWriter"/> for writing to the <see cref="Console"/>, with write calls synchronized.
    /// </summary>
    [PublicAPI]
    public class ConsoleTextWriter : LayoutTextWriter, IColoredTextWriter
    {
        /// <summary>
        /// The default <see cref="ConsoleTextWriter"/> (there can be only one).
        /// </summary>
        [NotNull]
        public static readonly ConsoleTextWriter Default;

        /// <summary>
        /// The default foreground color.
        /// </summary>
        [PublicAPI]
        public static ConsoleColor DefaultForeColor { get; private set; }

        /// <summary>
        /// The default background color.
        /// </summary>
        [PublicAPI]
        public static ConsoleColor DefaultBackColor { get; private set; }

        /// <summary>
        /// Initializes static members of the <see cref="ConsoleTextWriter"/> class.
        /// </summary>
        static ConsoleTextWriter()
        {
            Default = new ConsoleTextWriter();

            // Set the console's default output to use this one.
            Console.SetOut(Default);
        }

        /// <summary>
        /// Gets the width of the console.
        /// </summary>
        /// <value>
        /// The width of the console.
        /// </value>
        [PublicAPI]
        public ushort ConsoleWidth
        {
            get
            {
                if (!ConsoleHelper.IsConsole) return Layout.Width.Value;

                int width = Console.BufferWidth;
                return width > ushort.MaxValue
                    ? ushort.MaxValue
                    : (width < 1
                        ? (ushort) 1
                        : (ushort) width);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedTextWriter" /> class.
        /// </summary>
        private ConsoleTextWriter()
            : base(ConsoleHelper.IsConsole
                ? Console.Out ?? TraceTextWriter.Default
                : TraceTextWriter.Default)
        {
            Update();
        }

        /// <summary>
        /// Updates the <see cref="Layout"/>, <see cref="LayoutTextWriter.Position"/> and default colors,
        /// based on the console's current settings.
        /// </summary>
        [PublicAPI]
        public void Update()
        {
            if (!ConsoleHelper.IsConsole)
            {
                DefaultForeColor = ConsoleColor.White;
                DefaultBackColor = ConsoleColor.Black;
                return;
            }
            Context.Invoke(
                () =>
                {
                    // Grab the default colors.
                    ConsoleColor fc = Console.ForegroundColor;
                    ConsoleColor bc = Console.BackgroundColor;

                    Console.ResetColor();
                    DefaultForeColor = Console.ForegroundColor;
                    DefaultBackColor = Console.BackgroundColor;

                    Console.ForegroundColor = fc;
                    Console.BackgroundColor = bc;

                    ushort cw = ConsoleWidth;
                    ushort lw = Layout.Width.Value;
                    if (cw <= lw)
                        ApplyLayout(
                            ConsoleWidth,
                            wrapMode: LayoutWrapMode.NewLineOnShort);
                    else if (cw > lw)
                        ApplyLayout(wrapMode: LayoutWrapMode.NewLine);
                });
        }

        /// <summary>
        /// Gets or sets the current horizontal position.
        /// </summary>
        /// <value>The position.</value>
        public override int Position
        {
            get { return Console.CursorLeft; }
            // Ignore position updates.
            set { }
        }

        /// <summary>
        /// Sets the layout.
        /// </summary>
        /// <param name="newLayout">The new layout.</param>
        /// <returns>An awaitable task that returns the existing layout.</returns>
        public override Layout ApplyLayout(Layout newLayout)
        {
            Contract.Requires(newLayout != null);
            return ApplyLayout(
                newLayout.Width,
                newLayout.IndentSize,
                newLayout.RightMarginSize,
                newLayout.IndentChar,
                newLayout.FirstLineIndentSize,
                newLayout.TabStops,
                newLayout.TabSize,
                newLayout.TabChar,
                newLayout.Alignment,
                newLayout.SplitWords,
                newLayout.Hyphenate,
                newLayout.HyphenChar,
                newLayout.WrapMode);
        }

        /// <summary>
        /// Applies the layout.
        /// </summary>
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
        /// <returns>WebApplications.Utilities.Formatting.Layout.</returns>
        public override Layout ApplyLayout(
            Optional<ushort> width = new Optional<ushort>(),
            Optional<byte> indentSize = new Optional<byte>(),
            Optional<byte> rightMarginSize = new Optional<byte>(),
            Optional<char> indentChar = new Optional<char>(),
            Optional<ushort> firstLineIndentSize = new Optional<ushort>(),
            Optional<IEnumerable<ushort>> tabStops = new Optional<IEnumerable<ushort>>(),
            Optional<byte> tabSize = new Optional<byte>(),
            Optional<char> tabChar = new Optional<char>(),
            Optional<Alignment> alignment = new Optional<Alignment>(),
            Optional<bool> splitWords = new Optional<bool>(),
            Optional<bool> hyphenate = new Optional<bool>(),
            Optional<char> hyphenChar = new Optional<char>(),
            Optional<LayoutWrapMode> wrapMode = new Optional<LayoutWrapMode>())
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            return Context.Invoke(
                () =>
                {
                    if (width.IsAssigned)
                    {
                        ushort cw = ConsoleWidth;
                        ushort lw = width.Value;
                        if (cw <= lw)
                        {
                            width = cw;
                            wrapMode = LayoutWrapMode.NewLineOnShort;
                        }
                        else if (cw > lw)
                            wrapMode = LayoutWrapMode.NewLine;
                    }
                    return base.ApplyLayout(
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
                });
        }

        /// <summary>
        /// Resets the foreground and background colors of the writer.
        /// </summary>
        public void ResetColors()
        {
            if (ConsoleHelper.IsConsole)
                Context.Invoke(
                    () =>
                    {
                        Update();
                        Console.ForegroundColor = DefaultBackColor;
                        Console.BackgroundColor = DefaultBackColor;
                    });
        }

        /// <summary>
        /// Resets the foreground color of the writer.
        /// </summary>
        public void ResetForegroundColor()
        {
            if (ConsoleHelper.IsConsole)
                Context.Invoke(
                    () =>
                    {
                        Update();
                        Console.ForegroundColor = DefaultBackColor;
                    });
        }

        /// <summary>
        /// Sets the foreground color of the writer.
        /// </summary>
        /// <param name="color">The color.</param>
        public void SetForegroundColor(Color color)
        {
            if (ConsoleHelper.IsConsole)
                Context.Invoke(() => Console.ForegroundColor = color.ToConsoleColor());
        }

        /// <summary>
        /// Sets the background color of the writer.
        /// </summary>
        public void ResetBackgroundColor()
        {
            if (ConsoleHelper.IsConsole)
                Context.Invoke(
                    () =>
                    {
                        Update();
                        Console.BackgroundColor = DefaultBackColor;
                    });
        }

        /// <summary>
        /// Sets the background color of the writer.
        /// </summary>
        /// <param name="color">The color.</param>
        public void SetBackgroundColor(Color color)
        {
            if (ConsoleHelper.IsConsole)
                Context.Invoke(() => Console.BackgroundColor = color.ToConsoleColor());
        }

        /// <summary>
        /// Called when a control chunk is encountered.
        /// </summary>
        /// <param name="controlChunk">The control chunk.</param>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void OnControlChunk(FormatChunk controlChunk, string format, IFormatProvider formatProvider)
        {
            Contract.Requires(controlChunk != null);
            Contract.Requires(controlChunk.IsControl);
            this.SetColor(controlChunk);
        }
    }
}