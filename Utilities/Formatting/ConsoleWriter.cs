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
using WebApplications.Utilities.Enumerations;

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
        /// The custom colour names.
        /// </summary>
        [NotNull]
        private static readonly ConcurrentDictionary<string, ConsoleColor> _customColours =
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
        private void UpdateLayout()
        {
            if (ConsoleHelper.IsConsole)
            {
                // Grab the default colours.
                ConsoleColor fc = Console.ForegroundColor;
                ConsoleColor bc = Console.BackgroundColor;

                Console.ResetColor();
                DefaultForeColour = Console.ForegroundColor;
                DefaultBackColour = Console.BackgroundColor;

                Console.ForegroundColor = fc;
                Console.BackgroundColor = bc;

                // Update the width
                int width = Console.BufferWidth;
                ApplyLayout(
                    width > ushort.MaxValue
                        ? ushort.MaxValue
                        : (width < 1
                            ? (ushort)1
                            : (ushort)width));
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
            _customColours.AddOrUpdate(name, colour, (n, e) => colour);
        }

        /// <summary>
        /// Tries to get the colour with the custom name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="colour">The colour.</param>
        /// <returns><see langword="true" /> if found, <see langword="false" /> otherwise.</returns>
        [PublicAPI]
        public static bool TryGetColour([NotNull] string name, out ConsoleColor colour)
        {
            Contract.Requires(name != null);
            return _customColours.TryGetValue(name, out colour) ||
                   Enum.TryParse(name, true, out colour);
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
            return _customColours.TryRemove(name, out colour);
        }

        /// <summary>
        /// Gets a string represent of each chunk to write.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="builder">The builder.</param>
        /// <returns>A string representation of the chunk; otherwise <see langword="null" /> to skip.</returns>
        // ReSharper disable once CodeAnnotationAnalyzer
        protected override string GetString(IFormatProvider formatProvider, FormatBuilder builder)
        {
            UpdateLayout();
            return base.GetString(formatProvider, builder);
        }

        /// <summary>
        /// Gets a string represent of each chunk to write.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="chunk">The chunk.</param>
        /// <returns>A string representation of the chunk; otherwise <see langword="null"/> to skip.</returns>
        // ReSharper disable once CodeAnnotationAnalyzer
        protected override string GetChunk(IFormatProvider formatProvider, FormatChunk chunk)
        {
            ConsoleColor colour;
            /*
             * Check for supported control tags,
             * e.g. {ConsoleFore:Red}
             * or {ConsoleFore} to reset.
             */
            if (!string.IsNullOrEmpty(chunk.Tag))
            {
                switch (chunk.Tag.ToLower())
                {
                    case "consolefore":
                        if (string.IsNullOrEmpty(chunk.Format))
                            Console.ForegroundColor = DefaultForeColour;
                        else if (TryGetColour(chunk.Format, out colour))
                            Console.ForegroundColor = colour;
                        return null;
                    case "consoleback":
                        if (string.IsNullOrEmpty(chunk.Format))
                            Console.BackgroundColor = DefaultBackColour;
                        else if (TryGetColour(chunk.Format, out colour))
                            Console.BackgroundColor = colour;
                        return null;
                }
            }

            /*
             * Check for FormatBuilder's control chunks
             */
            if (chunk.IsControl)
            {
                ConsoleColourControl colourControl = chunk.Value as ConsoleColourControl;
                if (colourControl != null)
                {
                    if (colourControl.Colour == null)
                    {
                        if (colourControl.IsForeground == TriState.Equal)
                        {
                            Console.BackgroundColor = DefaultBackColour;
                            Console.ForegroundColor = DefaultForeColour;
                        }
                        else if (colourControl.IsForeground == TriState.Yes)
                            Console.ForegroundColor = DefaultForeColour;
                        else if (colourControl.IsForeground == TriState.No)
                            Console.BackgroundColor = DefaultBackColour;
                        return null;
                    }

                    if (!TryGetColour(colourControl.Colour, out colour))
                        return null;

                    if (colourControl.IsForeground == TriState.Equal)
                    {
                        Console.BackgroundColor = colour;
                        Console.ForegroundColor = colour;
                    }
                    else if (colourControl.IsForeground == TriState.Yes)
                        Console.ForegroundColor = colour;
                    else if (colourControl.IsForeground == TriState.No)
                        Console.BackgroundColor = colour;
                }
            }

            // Use base implementation.
            return base.GetChunk(formatProvider, chunk);
        }
    }
}