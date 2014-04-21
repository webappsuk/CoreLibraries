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
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// Used to write out coloured, laid out, formatted content to the console.
    /// </summary>
    public class ConsoleBuilder : LayoutBuilder
    {
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
        /// Gets the width of the console.
        /// </summary>
        /// <value>
        /// The width of the console.
        /// </value>
        [PublicAPI]
        public static ushort ConsoleWidth
        {
            get
            {
                int width = Console.BufferWidth;
                return width > ushort.MaxValue
                    ? ushort.MaxValue
                    : (width < 1
                        ? (ushort)1
                        : (ushort)width);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleBuilder" /> class.
        /// </summary>
        /// <param name="layout">The layout.</param>
        [PublicAPI]
        public ConsoleBuilder(
            [CanBeNull] Layout layout = null)
            : base(layout == null ? null : layout.Apply(ConsoleWidth, wrapMode: LayoutWrapMode.PadToWrap))
        {
            UpdateLayout();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleBuilder" /> class.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="layout">The layout.</param>
        public ConsoleBuilder(
            [CanBeNull] [InstantHandle] IEnumerable<object> values,
            [CanBeNull] Layout layout = null)
            : base(values, layout == null ? null : layout.Apply(ConsoleWidth, wrapMode: LayoutWrapMode.PadToWrap))
        {
            UpdateLayout();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleBuilder" /> class.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="layout">The layout.</param>
        [PublicAPI]
        public ConsoleBuilder(
            [CanBeNull] IReadOnlyDictionary<string, object> values,
            [CanBeNull] Layout layout = null)
            : base(values, layout == null ? null : layout.Apply(ConsoleWidth, wrapMode: LayoutWrapMode.PadToWrap))
        {
            UpdateLayout();
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>
        /// A shallow copy of this builder.
        /// </returns>
        public override FormatBuilder Clone()
        {
            ConsoleBuilder consoleBuilder = new ConsoleBuilder(Values, InitialLayout);
            consoleBuilder.Append(this.Select(c => c.Clone()));
            return consoleBuilder;
        }

        /// <summary>
        /// Gets the initial layout to use when resetting the layout.
        /// </summary>
        /// <value>
        /// The initial layout. If the current application running in a console, the current <see cref="ConsoleWidth"/> 
        /// and wrap mode <see cref="LayoutWrapMode.PadToWrap"/> will be applied.
        /// </value>
        protected override Layout InitialLayout
        {
            get
            {
                if (!ConsoleHelper.IsConsole)
                    return base.InitialLayout;
                return base.InitialLayout.Apply(
                    ConsoleWidth,
                    wrapMode: LayoutWrapMode.PadToWrap);
            }
        }

        /// <summary>
        /// Updates the layout.
        /// </summary>
        private int UpdateLayout()
        {
            if (!ConsoleHelper.IsConsole)
            {
                DefaultForeColour = ConsoleColor.White;
                DefaultBackColour = ConsoleColor.Black;
                return 0;
            }

            // Grab the default colours.
            ConsoleColor fc = Console.ForegroundColor;
            ConsoleColor bc = Console.BackgroundColor;

            Console.ResetColor();
            DefaultForeColour = Console.ForegroundColor;
            DefaultBackColour = Console.BackgroundColor;

            Console.ForegroundColor = fc;
            Console.BackgroundColor = bc;

            // Update the width and wrap mode
            if (!IsReadonly)
                ApplyLayout(
                    ConsoleWidth,
                    wrapMode: LayoutWrapMode.PadToWrap);

            return Console.CursorLeft;
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
        /// Writes this instance to the console.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        [PublicAPI]
        public void Write([CanBeNull] string format = null, [CanBeNull] IFormatProvider formatProvider = null)
        {
            if (ConsoleHelper.IsConsole)
                WriteTo(Console.Out, format, formatProvider);
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format passed to each chunk.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="position">The position.</param>
        public override void WriteTo(TextWriter writer, string format, IFormatProvider formatProvider, int position)
        {
            // Update the layout based on the console.
            using (ConsoleHelper.Lock.LockAsync().Result)
                base.WriteTo(writer, format, formatProvider, UpdateLayout());
        }

        /// <summary>
        /// Writes the builder to the specified <see cref="TextWriter" /> asynchronously.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format passed to each chunk.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="position">The position.</param>
        /// <returns>An awaitable task.</returns>
        public override async Task WriteToAsync(
            TextWriter writer,
            string format,
            IFormatProvider formatProvider,
            int position)
        {
            // Update the layout based on the console.
            using (await ConsoleHelper.Lock.LockAsync())
                await base.WriteToAsync(writer, format, formatProvider, UpdateLayout());
        }

        /// <summary>
        /// Called when a control chunk is encountered.
        /// </summary>
        /// <param name="controlChunk">The control chunk.</param>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        // ReSharper disable once CodeAnnotationAnalyzer
        protected override void OnControlChunk(
            FormatChunk controlChunk,
            TextWriter writer,
            string format,
            IFormatProvider formatProvider)
        {
            // We can only change colours if we are writing to the console!
            if (ConsoleHelper.IsConsole)
            {
                ConsoleColor colour;
                /*
                 * Check for supported control tags,
                 * e.g. {ConsoleFore:Red}
                 * or {ConsoleFore} to reset.
                 */
                if (!string.IsNullOrEmpty(controlChunk.Tag))
                {
                    // ReSharper disable once PossibleNullReferenceException
                    switch (controlChunk.Tag.ToLower())
                    {
                        case "consolefore":
                            if (controlChunk.Value is ConsoleColor)
                                Console.ForegroundColor = (ConsoleColor)controlChunk.Value;
                            else if (string.IsNullOrEmpty(controlChunk.Format))
                                Console.ForegroundColor = DefaultForeColour;
                            // ReSharper disable once AssignNullToNotNullAttribute
                            else if (TryGetColour(controlChunk.Format, out colour))
                                Console.ForegroundColor = colour;
                            return;
                        case "consoleback":
                            if (controlChunk.Value is ConsoleColor)
                                Console.BackgroundColor = (ConsoleColor)controlChunk.Value;
                            else if (string.IsNullOrEmpty(controlChunk.Format))
                                Console.BackgroundColor = DefaultBackColour;
                            // ReSharper disable once AssignNullToNotNullAttribute
                            else if (TryGetColour(controlChunk.Format, out colour))
                                Console.BackgroundColor = colour;
                            return;
                    }
                    return;
                }
            }

            base.OnControlChunk(controlChunk, writer, format, formatProvider);
        }
    }
}