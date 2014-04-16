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
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// Used to write out coloured, laid out, formatted content to the console.
    /// </summary>
    public class ConsoleWriter : LayoutWriter
    {
        /// <summary>
        /// The default console writer.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly ConsoleWriter Default = new ConsoleWriter();

        /// <summary>
        /// Indicates the foreground colour should be reset.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static string ResetForeColour = "{+_}";

        /// <summary>
        /// Indicates the background colour should be reset.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static string ResetBackColour = "{-_}";

        /// <summary>
        /// Indicates the background colour should be reset.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static string ResetColour = "{+_}{-_}";

        /// <summary>
        /// The custom colour names.
        /// </summary>
        [NotNull]
        private static readonly ConcurrentDictionary<string, ConsoleColor> _customColors =
            new ConcurrentDictionary<string, ConsoleColor>();

        /// <summary>
        /// The default foreground colour.
        /// </summary>
        [PublicAPI]
        public static ConsoleColor DefaultForeColour;

        /// <summary>
        /// The default background colour.
        /// </summary>
        [PublicAPI]
        public static ConsoleColor DefaultBackColour;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutWriter" /> class.
        /// </summary>
        private ConsoleWriter()
            : base((ConsoleHelper.IsConsole ? Console.Out : null) ?? TraceTextWriter.Default)
        {
            UpdateLayout();
        }

        /// <summary>
        /// Updates the layout.
        /// </summary>
        public void UpdateLayout()
        {
            if (ConsoleHelper.IsConsole)
            {
                // Grab the default colours.
                using (Lock.LockAsync().Result)
                {
                    ConsoleColor fc = Console.ForegroundColor;
                    ConsoleColor bc = Console.BackgroundColor;

                    Console.ResetColor();
                    DefaultForeColour = Console.ForegroundColor;
                    DefaultBackColour = Console.BackgroundColor;

                    Console.ForegroundColor = fc;
                    Console.BackgroundColor = bc;
                }

                int width = Console.BufferWidth;
                ushort w = width > ushort.MaxValue
                    ? ushort.MaxValue
                    : (width < 1
                        ? (ushort) 1
                        : (ushort) width);

                // TODO Add set width to base?
                Layout layout = Layout;
                if (layout.Width != w)
                    Layout = new Layout(
                        w,
                        layout.IndentSize,
                        layout.RightMarginSize,
                        layout.IndentChar,
                        layout.FirstLineIndentSize,
                        layout.TabStops,
                        layout.TabChar,
                        layout.SplitWords);
            }
            else
            {
                DefaultForeColour = ConsoleColor.White;
                DefaultBackColour = ConsoleColor.Black;
            }
        }

        /// <summary>
        /// Sets the custom name of the <see cref="ConsoleColor"/>.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="colour">The colour.</param>
        [PublicAPI]
        public static void SetCustomColourName([NotNull] string name, ConsoleColor colour)
        {
            Contract.Requires(name != null);
            _customColors.AddOrUpdate(name, colour, (n, e) => colour);
        }

        /// <summary>
        /// Tries to get the colour with the custom name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="colour">The colour.</param>
        /// <returns><see langword="true" /> if found, <see langword="false" /> otherwise.</returns>
        [PublicAPI]
        public static bool TryGetCustomColour([NotNull] string name, out ConsoleColor colour)
        {
            Contract.Requires(name != null);
            return _customColors.TryGetValue(name, out colour);
        }

        /// <summary>
        /// Tries to remove the custom name for the colour.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="colour">The colour that was removed.</param>
        /// <returns><see langword="true" /> if removed, <see langword="false" /> otherwise.</returns>
        [PublicAPI]
        public static bool RemoveCustomColour([NotNull] string name, out ConsoleColor colour)
        {
            Contract.Requires(name != null);
            return _customColors.TryRemove(name, out colour);
        }

        /// <summary>
        /// Gets a string represent of each chunk to write.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="chunk">The chunk.</param>
        /// <returns>A string representation of the chunk; otherwise <see langword="null"/> to skip.</returns>
        protected override string DoWriteChunk(IFormatProvider formatProvider, FormatChunk chunk)
        {
            if (chunk.Tag != null &&
                chunk.Tag.Length > 1)
            {
                char prefix = chunk.Tag[0];
                switch (prefix)
                {
                    case '+':
                    case '-':
                        bool isBack = prefix == '-';
                        string colourStr = chunk.Tag.Substring(1);
                        ConsoleColor colour;
                        if (colourStr == "_")
                            colour = isBack ? DefaultBackColour : DefaultForeColour;
                        else if (!_customColors.TryGetValue(colourStr, out colour) &&
                                 !Enum.TryParse(colourStr, true, out colour))
                            break;

                        if (isBack)
                            Console.BackgroundColor = colour;
                        else
                            Console.ForegroundColor = colour;

                        return null;
                }
            }

            return base.DoWriteChunk(formatProvider, chunk);
        }
    }
}