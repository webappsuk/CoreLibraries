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
using System.Globalization;
using System.Linq;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// Used to manipulate <see cref="Color"/> more easily, and supports custom naming for colors.
    /// </summary>
    public static class ColorHelper
    {
        /// <summary>
        /// The colors by name.
        /// </summary>
        [NotNull]
        private static readonly IReadOnlyDictionary<string, Color> _builtInNamedColors =
            typeof (Color).GetProperties()
                .Where(p => p.PropertyType == typeof (Color))
                .ToDictionary(p => p.Name, p => (Color) p.GetValue(null), StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// The colors by name.
        /// </summary>
        [NotNull]
        private static readonly Dictionary<string, Color> _namedColors = new Dictionary<string, Color>();

        /// <summary>
        /// The names by color.
        /// </summary>
        [NotNull]
        private static readonly Dictionary<Color, string> _colorNames = new Dictionary<Color, string>();

        /// <summary>
        /// Gets the known color names.
        /// </summary>
        /// <value>The known names.</value>
        [NotNull]
        [PublicAPI]
        public static IEnumerable<string> KnownNames
        {
            get { return _namedColors.Keys.Union(_builtInNamedColors.Keys).Distinct(); }
        }

        /// <summary>
        /// Gets the named colors.
        /// </summary>
        /// <value>The known names.</value>
        [NotNull]
        [PublicAPI]
        public static IEnumerable<KeyValuePair<string, Color>> NamedColors
        {
            get { return _namedColors.Union(_builtInNamedColors).Distinct(KeyComparer<string, Color>.Default); }
        }

        /// <summary>
        /// Sets a custom name for the color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="name">The name.</param>
        [PublicAPI]
        public static void SetName(this Color color, [NotNull] string name)
        {
            Contract.Requires(name != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(name));
            lock (_namedColors)
            {
                _namedColors[name] = color;
                _colorNames[color] = name;
            }
        }

        /// <summary>
        /// Removes the custom color name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The previous <see cref="Color"/>; otherwise <see cref="Optional{Color}.Unassigned"/>.</returns>
        [PublicAPI]
        public static Optional<Color> RemoveName([NotNull] string name)
        {
            Contract.Requires(name != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(name));
            lock (_namedColors)
            {
                Color existing;
                if (!_namedColors.TryGetValue(name, out existing))
                    return Optional<Color>.Unassigned;
                _namedColors.Remove(name);

                string existingName;
                if (_colorNames.TryGetValue(existing, out existingName) &&
                    string.Equals(name, existingName, StringComparison.InvariantCultureIgnoreCase))
                    _colorNames.Remove(existing);

                return existing;
            }
        }

        /// <summary>
        /// Gets the color corresponding to the name; otherwise returns <see cref="Optional{Color}.Unassigned"/>.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The <see cref="Color"/>.</returns>
        [PublicAPI]
        public static Optional<Color> GetColor([NotNull] string name)
        {
            Contract.Requires(name != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(name));
            Color color;
            lock (_namedColors)
                if (_namedColors.TryGetValue(name, out color))
                    return color;

            if (_builtInNamedColors.TryGetValue(name, out color))
                return color;

            // Try to parse hex name.
            int argb;
            if ((name.Length > 1) &&
                (name[0] == '#') &&
                int.TryParse(name.Substring(1), NumberStyles.HexNumber, null, out argb))
                return Color.FromArgb(argb);

            return Optional<Color>.Unassigned;
        }

        /// <summary>
        /// Gets the color's name.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The <see cref="Color" />.</returns>
        [NotNull]
        [PublicAPI]
        public static string GetName(this Color color)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            string name;
            lock (_namedColors)
                if (_colorNames.TryGetValue(color, out name))
                    return name;
            return color.IsNamedColor
                ? color.Name
                : "#" + color.Name;
            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Gets the <see cref="Color"/> of a <see cref="ConsoleColor"/>.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The equivalent <see cref="Color"/>.</returns>
        [PublicAPI]
        public static Color ToColor(this ConsoleColor color)
        {
            return ConsoleHelper.ToColor(color);
        }

        /// <summary>
        /// Converts a <see cref="Color"/> to the nearest <see cref="ConsoleColor"/>.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The closest <see cref="ConsoleColor"/>.</returns>
        [PublicAPI]
        public static ConsoleColor ToConsoleColor(this Color color)
        {
            return ConsoleHelper.ToConsoleColor(color);
        }
    }
}