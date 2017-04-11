#region � Copyright Web Applications (UK) Ltd, 2015.  All rights reserved.
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
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Difference;
using WebApplications.Utilities.Formatting;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Extension methods for the difference engine.
    /// </summary>
    /// <seealso cref="Differences{T}"/>
    [PublicAPI]
    public static class DifferenceExtensions
    {
        #region Formatting Tags
        /// <summary>
        /// The flag background color name.
        /// </summary>
        [NotNull]
        public const string FlagBackgroundColorName = "bgcolorFlag";

        /// <summary>
        /// The flag foreground color name.
        /// </summary>
        [NotNull]
        public const string FlagForegroundColorName = "fgcolorFlag";

        /// <summary>
        /// The chunk a background color name.
        /// </summary>
        [NotNull]
        public const string ChunkABackgroundColorName = "bgcolorChunkA";

        /// <summary>
        /// The chunk a foreground color name.
        /// </summary>
        [NotNull]
        public const string ChunkAForegroundColorName = "fgcolorChunkA";

        /// <summary>
        /// The chunk b background color name.
        /// </summary>
        [NotNull]
        public const string ChunkBBackgroundColorName = "bgcolorChunkB";

        /// <summary>
        /// The chunk b foreground color name.
        /// </summary>
        [NotNull]
        public const string ChunkBForegroundColorName = "fgcolorChunkB";

        /// <summary>
        /// The separator tag
        /// </summary>
        [NotNull]
        public const string SeparatorTag = "separator";

        /// <summary>
        /// The flag tag.
        /// </summary>
        [NotNull]
        public const string FlagTag = "flag";

        /// <summary>
        /// The chunk tag.
        /// </summary>
        [NotNull]
        public const string ChunkTag = "chunk";

        /// <summary>
        /// The chunk a tag.
        /// </summary>
        [NotNull]
        public const string ChunkATag = "chunkA";

        /// <summary>
        /// The chunk b tag.
        /// </summary>
        [NotNull]
        public const string ChunkBTag = "chunkB";
        #endregion

        /// <summary>
        /// The default line format, used for each line of the chunk.
        /// </summary>
        [NotNull]
        public static readonly FormatBuilder DefaultLineFormat =
            @"{" + FormatBuilder.BackgroundColorTag + ":" + FlagBackgroundColorName + "}{" +
            FormatBuilder.ForegroundColorTag + ":" + FlagForegroundColorName + "}{" + FlagTag + ",3}{" +
            FormatBuilder.ResetColorsTag + "}{" + FormatBuilder.BackgroundColorTag + ":" + ChunkABackgroundColorName +
            "}{" + FormatBuilder.ForegroundColorTag + ":" + ChunkAForegroundColorName + "}{" + ChunkATag + ",40}{" +
            FormatBuilder.ResetColorsTag + "}{" + SeparatorTag + "}{" + FormatBuilder.BackgroundColorTag + ":" +
            ChunkBBackgroundColorName + "}{" + FormatBuilder.ForegroundColorTag + ":" + ChunkBForegroundColorName + "}{" +
            ChunkBTag + ",40}{" + FormatBuilder.ResetColorsTag + "}" + Environment.NewLine;

        /// <summary>
        /// The default layout for chunks.
        /// </summary>
        [NotNull]
        public static readonly FormatBuilder DefaultChunkFormat = new FormatBuilder(
            new Layout(40, wrapMode: LayoutWrapMode.PadToNewLine),
            "{" + ChunkTag + ":[{<Items>:{<Item>}}{<JOIN>:, }]}");

        /// <summary>
        /// The default layout for chunks.
        /// </summary>
        [NotNull]
        public static readonly FormatBuilder DefaultStringChunkFormat = new FormatBuilder(
            new Layout(40, wrapMode: LayoutWrapMode.PadToNewLine),
            "{" + ChunkTag + "}");

        /// <summary>
        /// Find the differences between two strings.
        /// </summary>
        /// <param name="a">The 'A' string.</param>
        /// <param name="b">The 'B' string.</param>
        /// <param name="tokenStrategy">The tokenization strategy.</param>
        /// <param name="textOptions">The text options.</param>
        /// <param name="comparer">The character comparer.</param>
        /// <returns>Returns the <see cref="StringDifferences" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="a" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="b" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="textOptions" /> cannot be set to 
        /// <see cref="TextOptions.IgnoreWhiteSpace"/> for any text tokenization strategy other than 
        /// <see cref="TextTokenStrategy.Character"/> as it would prevent correct tokenization.</exception>
        /// <remarks>Based on "An O(ND) Difference Algorithm and its Variations" by Eugene Myers
        /// (http://www.xmailserver.org/diff2.pdf) Algorithmica Vol. 1 No. 2, 1986, p 251.</remarks>
        [NotNull]
        [ItemNotNull]
        public static StringDifferences Diff(
            [NotNull] this string a,
            [NotNull] string b,
            TextTokenStrategy tokenStrategy = TextTokenStrategy.Character,
            TextOptions textOptions = TextOptions.Default,
            IEqualityComparer<char> comparer = null)
        {
            if (comparer == null) comparer = CharComparer.CurrentCulture;
            // ReSharper disable ExceptionNotDocumented
            return tokenStrategy == TextTokenStrategy.Character
                ? new StringDifferences(a, 0, a.Length, b, 0, b.Length, textOptions, (x, y) => comparer.Equals(x, y))
                : new StringDifferences(
                    tokenStrategy,
                    a,
                    0,
                    a.Length,
                    b,
                    0,
                    b.Length,
                    textOptions,
                    (x, y) => StringComparer.CurrentCulture.Equals(x, y)); // TODO
            // ReSharper restore ExceptionNotDocumented
        }

        /// <summary>
        /// Find the differences between two strings.
        /// </summary>
        /// <param name="a">The 'A' string.</param>
        /// <param name="b">The 'B' string.</param>
        /// <param name="tokenStrategy">The tokenization strategy.</param>
        /// <param name="textOptions">The text options.</param>
        /// <param name="comparer">The character comparer.</param>
        /// <returns>Returns the <see cref="StringDifferences"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="a" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="b" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="comparer" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="textOptions" /> cannot be set to 
        /// <see cref="TextOptions.IgnoreWhiteSpace"/> for any text tokenization strategy other than 
        /// <see cref="TextTokenStrategy.Character"/> as it would prevent correct tokenization.</exception>
        /// <remarks>
        /// <para> 
        /// Based on "An O(ND) Difference Algorithm and its Variations" by Eugene Myers
        /// (http://www.xmailserver.org/diff2.pdf) Algorithmica Vol. 1 No. 2, 1986, p 251.  
        /// </para>
        /// </remarks>
        [NotNull]
        [ItemNotNull]
        public static StringDifferences Diff(
            [NotNull] this string a,
            [NotNull] string b,
            TextTokenStrategy tokenStrategy,
            TextOptions textOptions,
            [NotNull] StringComparer comparer)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            CharComparer c = CharComparer.Create(comparer);

            // ReSharper disable ExceptionNotDocumented
            return tokenStrategy == TextTokenStrategy.Character
                ? new StringDifferences(a, 0, a.Length, b, 0, b.Length, textOptions, (x, y) => c.Equals(x, y))
                : new StringDifferences(
                    tokenStrategy,
                    a,
                    0,
                    a.Length,
                    b,
                    0,
                    b.Length,
                    textOptions,
                    (x, y) => c.Equals(x, y));
            // ReSharper restore ExceptionNotDocumented
        }

        /// <summary>
        /// Find the differences between two strings.
        /// </summary>
        /// <param name="a">The 'A' string.</param>
        /// <param name="b">The 'B' string.</param>
        /// <param name="tokenStrategy">The tokenization strategy.</param>
        /// <param name="textOptions">The text options.</param>
        /// <param name="comparer">The character comparer.</param>
        /// <returns>Returns the <see cref="StringDifferences"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="a" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="b" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="comparer" /> is <see langword="null" />.</exception>
        /// <exception cref="Exception">The <paramref name="comparer"/> throws an exception.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="textOptions" /> cannot be set to 
        /// <see cref="TextOptions.IgnoreWhiteSpace"/> for any text tokenization strategy other than 
        /// <see cref="TextTokenStrategy.Character"/> as it would prevent correct tokenization.</exception>
        /// <remarks>
        /// <para> 
        /// Based on "An O(ND) Difference Algorithm and its Variations" by Eugene Myers
        /// (http://www.xmailserver.org/diff2.pdf) Algorithmica Vol. 1 No. 2, 1986, p 251.  
        /// </para>
        /// </remarks>
        [NotNull]
        [ItemNotNull]
        public static StringDifferences Diff(
            [NotNull] this string a,
            [NotNull] string b,
            TextTokenStrategy tokenStrategy,
            TextOptions textOptions,
            [NotNull] Func<char, char, bool> comparer)
            => tokenStrategy == TextTokenStrategy.Character
                ? new StringDifferences(a, 0, a.Length, b, 0, b.Length, textOptions, comparer)
                : new StringDifferences(
                    tokenStrategy,
                    a,
                    0,
                    a.Length,
                    b,
                    0,
                    b.Length,
                    textOptions,
                    (x, y) => StringComparer.CurrentCulture.Equals(x, y)); // TODO

        /// <summary>
        /// Find the differences between two strings.
        /// </summary>
        /// <param name="a">The 'A' collection.</param>
        /// <param name="offsetA">The offset to the start of a window in the first collection.</param>
        /// <param name="lengthA">The length of the window in the first collection.</param>
        /// <param name="b">The 'B' collection.</param>
        /// <param name="offsetB">The offset to the start of a window in the second collection.</param>
        /// <param name="lengthB">The length of the window in the second collection.</param>
        /// <param name="tokenStrategy">The tokenization strategy.</param>
        /// <param name="textOptions">The text options.</param>
        /// <param name="comparer">The comparer to compare items in each collection.</param>
        /// <returns>Returns the <see cref="Differences{T}"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="a" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="b" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="offsetA" /> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="lengthA" /> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="offsetB" /> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="lengthB" /> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="textOptions" /> cannot be set to 
        /// <see cref="TextOptions.IgnoreWhiteSpace"/> for any text tokenization strategy other than 
        /// <see cref="TextTokenStrategy.Character"/> as it would prevent correct tokenization.</exception>
        /// <remarks>
        /// <para> 
        /// Based on "An O(ND) Difference Algorithm and its Variations" by Eugene Myers
        /// (http://www.xmailserver.org/diff2.pdf) Algorithmica Vol. 1 No. 2, 1986, p 251.  
        /// </para>
        /// </remarks>
        [NotNull]
        [ItemNotNull]
        public static StringDifferences Diff(
            [NotNull] this string a,
            int offsetA,
            int lengthA,
            [NotNull] string b,
            int offsetB,
            int lengthB,
            TextTokenStrategy tokenStrategy = TextTokenStrategy.Character,
            TextOptions textOptions = TextOptions.Default,
            IEqualityComparer<char> comparer = null)
        {
            if (comparer == null) comparer = EqualityComparer<char>.Default;
            // ReSharper disable ExceptionNotDocumented
            return tokenStrategy == TextTokenStrategy.Character
                ? new StringDifferences(
                    a,
                    offsetA,
                    lengthA,
                    b,
                    offsetB,
                    lengthB,
                    textOptions,
                    (x, y) => comparer.Equals(x, y))
                : new StringDifferences(
                    tokenStrategy,
                    a,
                    offsetA,
                    lengthA,
                    b,
                    offsetB,
                    lengthB,
                    textOptions,
                    (x, y) => StringComparer.CurrentCulture.Equals(x, y)); // TODO
            // ReSharper restore ExceptionNotDocumented
        }

        /// <summary>
        /// Find the differences between two strings.
        /// </summary>
        /// <param name="a">The 'A' collection.</param>
        /// <param name="offsetA">The offset to the start of a window in the first collection.</param>
        /// <param name="lengthA">The length of the window in the first collection.</param>
        /// <param name="b">The 'B' collection.</param>
        /// <param name="offsetB">The offset to the start of a window in the second collection.</param>
        /// <param name="lengthB">The length of the window in the second collection.</param>
        /// <param name="tokenStrategy">The tokenization strategy.</param>
        /// <param name="textOptions">The text options.</param>
        /// <param name="comparer">The comparer to compare items in each collection.</param>
        /// <returns>Returns the <see cref="Differences{T}"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="a" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="b" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="offsetA" /> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="lengthA" /> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="offsetB" /> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="lengthB" /> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="textOptions" /> cannot be set to 
        /// <see cref="TextOptions.IgnoreWhiteSpace"/> for any text tokenization strategy other than 
        /// <see cref="TextTokenStrategy.Character"/> as it would prevent correct tokenization.</exception>
        /// <remarks>
        /// <para> 
        /// Based on "An O(ND) Difference Algorithm and its Variations" by Eugene Myers
        /// (http://www.xmailserver.org/diff2.pdf) Algorithmica Vol. 1 No. 2, 1986, p 251.  
        /// </para>
        /// </remarks>
        [NotNull]
        [ItemNotNull]
        public static StringDifferences Diff(
            [NotNull] this string a,
            int offsetA,
            int lengthA,
            [NotNull] string b,
            int offsetB,
            int lengthB,
            TextTokenStrategy tokenStrategy,
            TextOptions textOptions,
            [NotNull] StringComparer comparer)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            CharComparer c = CharComparer.Create(comparer);

            // ReSharper disable ExceptionNotDocumented
            return tokenStrategy == TextTokenStrategy.Character
                ? new StringDifferences(
                    a,
                    offsetA,
                    lengthA,
                    b,
                    offsetB,
                    lengthB,
                    textOptions,
                    (x, y) => c.Equals(x, y))
                : new StringDifferences(
                    tokenStrategy,
                    a,
                    offsetA,
                    lengthA,
                    b,
                    offsetB,
                    lengthB,
                    textOptions,
                    (x, y) => c.Equals(x, y));
            // ReSharper restore ExceptionNotDocumented
        }

        /// <summary>
        /// Find the differences between two strings.
        /// </summary>
        /// <param name="a">The 'A' collection.</param>
        /// <param name="offsetA">The offset to the start of a window in the first collection.</param>
        /// <param name="lengthA">The length of the window in the first collection.</param>
        /// <param name="b">The 'B' collection.</param>
        /// <param name="offsetB">The offset to the start of a window in the second collection.</param>
        /// <param name="lengthB">The length of the window in the second collection.</param>
        /// <param name="tokenStrategy">The tokenization strategy.</param>
        /// <param name="textOptions">The text options.</param>
        /// <param name="comparer">The comparer to compare items in each collection.</param>
        /// <returns>Returns the <see cref="Differences{T}" />.</returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="a" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="b" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="offsetA" /> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="lengthA" /> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="offsetB" /> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="lengthB" /> is out of range.</exception>
        /// <exception cref="Exception">The <paramref name="comparer" /> throws an exception.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="textOptions" /> cannot be set to 
        /// <see cref="TextOptions.IgnoreWhiteSpace"/> for any text tokenization strategy other than 
        /// <see cref="TextTokenStrategy.Character"/> as it would prevent correct tokenization.</exception>
        /// <remarks>Based on "An O(ND) Difference Algorithm and its Variations" by Eugene Myers
        /// (http://www.xmailserver.org/diff2.pdf) Algorithmica Vol. 1 No. 2, 1986, p 251.</remarks>
        [NotNull]
        [ItemNotNull]
        public static StringDifferences Diff(
            [NotNull] this string a,
            int offsetA,
            int lengthA,
            [NotNull] string b,
            int offsetB,
            int lengthB,
            TextTokenStrategy tokenStrategy,
            TextOptions textOptions,
            [NotNull] Func<char, char, bool> comparer)
            =>
                tokenStrategy == TextTokenStrategy.Character
                    ? new StringDifferences(a, offsetA, lengthA, b, offsetB, lengthB, textOptions, comparer)
                    : new StringDifferences(
                        tokenStrategy,
                        a,
                        offsetA,
                        lengthA,
                        b,
                        offsetB,
                        lengthB,
                        textOptions,
                        (x, y) => StringComparer.CurrentCulture.Equals(x, y)); // TODO

        /// <summary>
        /// Find the differences between two collections.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="a">The 'A' collection.</param>
        /// <param name="b">The 'B' collection.</param>
        /// <param name="comparer">The comparer to compare items in each collection.</param>
        /// <returns>Returns the <see cref="Differences{T}"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="a" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="b" /> is <see langword="null" />.</exception>
        /// <remarks>
        /// <para> 
        /// Based on "An O(ND) Difference Algorithm and its Variations" by Eugene Myers
        /// (http://www.xmailserver.org/diff2.pdf) Algorithmica Vol. 1 No. 2, 1986, p 251.  
        /// </para>
        /// </remarks>
        [NotNull]
        [ItemNotNull]
        public static Differences<T> Diff<T>(
            [NotNull] this IReadOnlyList<T> a,
            [NotNull] IReadOnlyList<T> b,
            IEqualityComparer<T> comparer = null)
        {
            if (comparer == null) comparer = EqualityComparer<T>.Default;
            // ReSharper disable ExceptionNotDocumented
            return new Differences<T>(a, 0, a.Count, b, 0, b.Count, (x, y) => comparer.Equals(x, y));
            // ReSharper restore ExceptionNotDocumented
        }

        /// <summary>
        /// Find the differences between two collections.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="a">The 'A' collection.</param>
        /// <param name="b">The 'B' collection.</param>
        /// <param name="comparer">The comparer to compare items in each collection.</param>
        /// <returns>Returns the <see cref="Differences{T}"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="a" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="b" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="comparer" /> is <see langword="null" />.</exception>
        /// <exception cref="Exception">The <paramref name="comparer"/> throws an exception.</exception>
        /// <remarks>
        /// <para> 
        /// Based on "An O(ND) Difference Algorithm and its Variations" by Eugene Myers
        /// (http://www.xmailserver.org/diff2.pdf) Algorithmica Vol. 1 No. 2, 1986, p 251.  
        /// </para>
        /// </remarks>
        [NotNull]
        [ItemNotNull]
        public static Differences<T> Diff<T>(
            [NotNull] this IReadOnlyList<T> a,
            [NotNull] IReadOnlyList<T> b,
            [NotNull] Func<T, T, bool> comparer)
            => new Differences<T>(a, 0, a.Count, b, 0, b.Count, comparer);

        /// <summary>
        /// Find the differences between two collections.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="a">The 'A' collection.</param>
        /// <param name="offsetA">The offset to the start of a window in the first collection.</param>
        /// <param name="lengthA">The length of the window in the first collection.</param>
        /// <param name="b">The 'B' collection.</param>
        /// <param name="offsetB">The offset to the start of a window in the second collection.</param>
        /// <param name="lengthB">The length of the window in the second collection.</param>
        /// <param name="comparer">The comparer to compare items in each collection.</param>
        /// <returns>Returns the <see cref="Differences{T}"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="a" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="b" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="offsetA" /> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="lengthA" /> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="offsetB" /> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="lengthB" /> is out of range.</exception>
        /// <remarks>
        /// <para> 
        /// Based on "An O(ND) Difference Algorithm and its Variations" by Eugene Myers
        /// (http://www.xmailserver.org/diff2.pdf) Algorithmica Vol. 1 No. 2, 1986, p 251.  
        /// </para>
        /// </remarks>
        [NotNull]
        [ItemNotNull]
        public static Differences<T> Diff<T>(
            [NotNull] this IReadOnlyList<T> a,
            int offsetA,
            int lengthA,
            [NotNull] IReadOnlyList<T> b,
            int offsetB,
            int lengthB,
            IEqualityComparer<T> comparer = null)
        {
            if (comparer == null) comparer = EqualityComparer<T>.Default;
            // ReSharper disable ExceptionNotDocumented
            return new Differences<T>(a, offsetA, lengthA, b, offsetB, lengthB, (x, y) => comparer.Equals(x, y));
            // ReSharper restore ExceptionNotDocumented
        }

        /// <summary>
        /// Find the differences between two collections.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="a">The 'A' collection.</param>
        /// <param name="offsetA">The offset to the start of a window in the first collection.</param>
        /// <param name="lengthA">The length of the window in the first collection.</param>
        /// <param name="b">The 'B' collection.</param>
        /// <param name="offsetB">The offset to the start of a window in the second collection.</param>
        /// <param name="lengthB">The length of the window in the second collection.</param>
        /// <param name="comparer">The comparer to compare items in each collection.</param>
        /// <returns>Returns the <see cref="Differences{T}"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="a" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="b" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="offsetA" /> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="lengthA" /> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="offsetB" /> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="lengthB" /> is out of range.</exception>
        /// <exception cref="Exception">The <paramref name="comparer"/> throws an exception.</exception>
        /// <remarks>
        /// <para> 
        /// Based on "An O(ND) Difference Algorithm and its Variations" by Eugene Myers
        /// (http://www.xmailserver.org/diff2.pdf) Algorithmica Vol. 1 No. 2, 1986, p 251.  
        /// </para>
        /// </remarks>
        [NotNull]
        [ItemNotNull]
        public static Differences<T> Diff<T>(
            [NotNull] this IReadOnlyList<T> a,
            int offsetA,
            int lengthA,
            [NotNull] IReadOnlyList<T> b,
            int offsetB,
            int lengthB,
            [NotNull] Func<T, T, bool> comparer)
            => new Differences<T>(a, offsetA, lengthA, b, offsetB, lengthB, comparer);

        /// <summary>
        /// Converts a string to a <see cref="ReadOnlyOffsetMap{T}"/> of <see cref="char">characters</see> by applying
        /// the specified <paramref name="options"/>.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="options">The options.</param>
        /// <returns>A <see cref="ReadOnlyOffsetMap{T}"/> of <see cref="char">characters</see>.</returns>
        [NotNull]
        public static StringMap ToMapped(
            [NotNull] this string input,
            TextOptions options = TextOptions.Default) => new StringMap(input, options);

        /// <summary>
        /// Tokenizes a string (optionally using the <see paramref="textOptions">text options</see>).
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="strategy">The token strategy.</param>
        /// <param name="options">The text options.</param>
        /// <returns>An enumeration of <see cref="string" />.</returns>
        [NotNull]
        public static IEnumerable<string> ToTokens(
            [NotNull] this string input,
            TextTokenStrategy strategy,
            TextOptions options = TextOptions.None)
            => options == TextOptions.None
                ? ToTokens((IEnumerable<char>)input, strategy)
                : ToTokens(input.ToMapped(options), strategy);

        /// <summary>
        /// Tokenizes an enumeration of characters.
        /// </summary>
        /// <param name="characters">The characters.</param>
        /// <param name="strategy">The token strategy.</param>
        /// <returns>An enumeration of <see cref="string"/>.</returns>
        [NotNull]
        public static IEnumerable<string> ToTokens(
            [NotNull] this IEnumerable<char> characters,
            TextTokenStrategy strategy)
        {
            switch (strategy)
            {
                case TextTokenStrategy.Character:
                    // Just case characters to a string
                    foreach (char c in characters)
                        yield return c.ToString();
                    yield break;

                case TextTokenStrategy.Word:
                    // See http://www.unicode.org/Public/UNIDATA/auxiliary/WordBreakTest.html

                    // TODO This is not a direct implementation of the above spec.

                    StringBuilder wordBuilder = new StringBuilder();
                    bool? inWord = null;
                    foreach (char c in characters)
                    {
                        // Check for word boundary changes.
                        bool isWord = c.IsWord();
                        if (inWord == null) inWord = isWord;
                        else if (inWord.Value != isWord)
                        {
                            // Changed boundary.
                            yield return wordBuilder.ToString();
                            wordBuilder.Clear();
                            inWord = isWord;
                        }

                        // Add character to builder
                        wordBuilder.Append(c);
                    }

                    // If we have anything else, yield it.
                    if (wordBuilder.Length > 0)
                        yield return wordBuilder.ToString();
                    break;
                case TextTokenStrategy.Line:
                    // See http://www.unicode.org/Public/UNIDATA/auxiliary/LineBreakTest.html
                    throw new NotImplementedException();
                    break;
                case TextTokenStrategy.Sentence:
                    // See http://www.unicode.org/Public/7.0.0/ucd/auxiliary/SentenceBreakTest.html
                    // except we only break on lines if we are using the 'Sentence' strategy
                    throw new NotImplementedException();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null);
            }
        }

        /// <summary>
        /// The word categories.
        /// </summary>
        [NotNull]
        private static readonly HashSet<UnicodeCategory> _wordCategories = new HashCollection<UnicodeCategory>(
            new[]
            {
                UnicodeCategory.DecimalDigitNumber,
                UnicodeCategory.UppercaseLetter,
                UnicodeCategory.ConnectorPunctuation,
                UnicodeCategory.LowercaseLetter,
                UnicodeCategory.OtherLetter,
                UnicodeCategory.TitlecaseLetter,
                UnicodeCategory.ModifierLetter,
                UnicodeCategory.NonSpacingMark,
            });

        /// <summary>
        /// Determines whether the <paramref name="c">specified character</paramref> is a word character.
        /// </summary>
        /// <param name="c">The character.</param>
        /// <returns><see langword="true"/> if the <paramref name="c">specified character</paramref> is a word 
        /// character; otherwise <see langword="false"/>.</returns>
        /// <remarks><para>This method is designed to be directly equivalent to using '\w' in a 
        /// <see cref="Regex">regular expression</see>, but is faster for checking a single character.</para>
        /// <para>Despite the specification at https://msdn.microsoft.com/en-us/library/20bw873z(v=vs.110).aspx#WordCharacter,
        /// The '\w' word character also includes the <see cref="UnicodeCategory.NonSpacingMark">NonSpacingMark</see><see cref="UnicodeCategory">unicode category</see>.
        /// </para></remarks>
        public static bool IsWord(this char c) => _wordCategories.Contains(char.GetUnicodeCategory(c));
        
    }
}
