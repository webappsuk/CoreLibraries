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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// Implements a <see cref="TextWriter"/> for writing to the <see cref="Console"/>, with write calls synchronized.
    /// </summary>
    [PublicAPI]
    public class ConsoleTextWriter : SerialTextWriter, IColoredTextWriter, ILayoutTextWriter
    {
        /// <summary>
        /// The default <see cref="ConsoleTextWriter"/> (there can be only one).
        /// </summary>
        [NotNull]
        public static readonly ConsoleTextWriter Default;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialTextWriter" /> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        private ConsoleTextWriter([NotNull] TextWriter writer)
            : base(writer)
        {
            if (writer == null) throw new ArgumentNullException("writer");
        }

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
            if (!ConsoleHelper.IsConsole)
            {
                Default = new ConsoleTextWriter(TraceTextWriter.Default);
                return;
            }

            // ReSharper disable once AssignNullToNotNullAttribute
            Default = new ConsoleTextWriter(Console.Out);
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
        public int Width
        {
            get
            {
                if (!ConsoleHelper.IsConsole)
                {
                    ILayoutTextWriter lw = Writer as ILayoutTextWriter;
                    return lw == null ? int.MaxValue : lw.Width;
                }
                Debug.Assert(Console.BufferWidth > 0);
                return Console.BufferWidth;
            }
        }

        /// <summary>
        /// Gets or sets the current horizontal position.
        /// </summary>
        /// <value>The position.</value>
        [PublicAPI]
        public int Position
        {
            get
            {
                if (!ConsoleHelper.IsConsole)
                {
                    ILayoutTextWriter lw = Writer as ILayoutTextWriter;
                    return lw == null ? 0 : lw.Position;
                }
                Debug.Assert(Console.CursorLeft > 0);
                return Console.CursorLeft;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the writer automatically wraps on reaching <see cref="Width" />.
        /// </summary>
        /// <value><see langword="true" /> if the writer automatically wraps; otherwise, <see langword="false" />.</value>
        public bool AutoWraps
        {
            get
            {
                if (ConsoleHelper.IsConsole) return true;

                ILayoutTextWriter lw = Writer as ILayoutTextWriter;
                return lw != null && lw.AutoWraps;
            }
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
                        Console.ResetColor();
                        DefaultForeColor = Console.ForegroundColor;
                        DefaultBackColor = Console.BackgroundColor;
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
                        ConsoleColor bc = Console.BackgroundColor;
                        Console.ResetColor();
                        DefaultForeColor = Console.ForegroundColor;
                        DefaultBackColor = Console.BackgroundColor;
                        Console.BackgroundColor = bc;
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
                        ConsoleColor fc = Console.ForegroundColor;
                        Console.ResetColor();
                        DefaultForeColor = Console.ForegroundColor;
                        DefaultBackColor = Console.BackgroundColor;
                        Console.ForegroundColor = fc;
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
    }
}