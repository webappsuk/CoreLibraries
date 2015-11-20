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
using System.Collections.Generic;
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
            ChunkBTag + ",40}{" + FormatBuilder.ResetColorsTag + "}"+Environment.NewLine;

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
        /// <param name="textOptions">The text options.</param>
        /// <param name="comparer">The character comparer.</param>
        /// <returns>Returns the <see cref="StringDifferences" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="a" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="b" /> is <see langword="null" />.</exception>
        /// <remarks>Based on "An O(ND) Difference Algorithm and its Variations" by Eugene Myers
        /// (http://www.xmailserver.org/diff2.pdf) Algorithmica Vol. 1 No. 2, 1986, p 251.</remarks>
        [NotNull]
        [ItemNotNull]
        public static StringDifferences Diff(
            [NotNull] this string a,
            [NotNull] string b,
            TextOptions textOptions = TextOptions.Default,
            IEqualityComparer<char> comparer = null)
        {
            if (comparer == null) comparer = CharComparer.CurrentCulture;
            // ReSharper disable ExceptionNotDocumented
            return new StringDifferences(a, 0, a.Length, b, 0, b.Length, textOptions, (x, y) => comparer.Equals(x, y));
            // ReSharper restore ExceptionNotDocumented
        }

        /// <summary>
        /// Find the differences between two strings.
        /// </summary>
        /// <param name="a">The 'A' string.</param>
        /// <param name="b">The 'B' string.</param>
        /// <param name="textOptions">The text options.</param>
        /// <param name="comparer">The character comparer.</param>
        /// <returns>Returns the <see cref="StringDifferences"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="a" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="b" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="comparer" /> is <see langword="null" />.</exception>
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
            TextOptions textOptions,
            [NotNull] StringComparer comparer)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            CharComparer c = CharComparer.Create(comparer);

            // ReSharper disable ExceptionNotDocumented
            return new StringDifferences(a, 0, a.Length, b, 0, b.Length, textOptions, (x, y) => c.Equals(x, y));
            // ReSharper restore ExceptionNotDocumented
        }

        /// <summary>
        /// Find the differences between two strings.
        /// </summary>
        /// <param name="a">The 'A' string.</param>
        /// <param name="b">The 'B' string.</param>
        /// <param name="textOptions">The text options.</param>
        /// <param name="comparer">The character comparer.</param>
        /// <returns>Returns the <see cref="StringDifferences"/>.</returns>
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
        public static StringDifferences Diff(
            [NotNull] this string a,
            [NotNull] string b,
            TextOptions textOptions,
            [NotNull] Func<char, char, bool> comparer)
            => new StringDifferences(a, 0, a.Length, b, 0, b.Length, textOptions, comparer);

        /// <summary>
        /// Find the differences between two strings.
        /// </summary>
        /// <param name="a">The 'A' collection.</param>
        /// <param name="offsetA">The offset to the start of a window in the first collection.</param>
        /// <param name="lengthA">The length of the window in the first collection.</param>
        /// <param name="b">The 'B' collection.</param>
        /// <param name="offsetB">The offset to the start of a window in the second collection.</param>
        /// <param name="lengthB">The length of the window in the second collection.</param>
        /// <param name="textOptions">The text options.</param>
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
        public static StringDifferences Diff(
            [NotNull] this string a,
            int offsetA,
            int lengthA,
            [NotNull] string b,
            int offsetB,
            int lengthB,
            TextOptions textOptions = TextOptions.Default,
            IEqualityComparer<char> comparer = null)
        {
            if (comparer == null) comparer = EqualityComparer<char>.Default;
            // ReSharper disable ExceptionNotDocumented
            return new StringDifferences(
                a,
                offsetA,
                lengthA,
                b,
                offsetB,
                lengthB,
                textOptions,
                (x, y) => comparer.Equals(x, y));
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
        /// <param name="textOptions">The text options.</param>
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
        public static StringDifferences Diff(
            [NotNull] this string a,
            int offsetA,
            int lengthA,
            [NotNull] string b,
            int offsetB,
            int lengthB,
            TextOptions textOptions,
            [NotNull] StringComparer comparer)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            CharComparer c = CharComparer.Create(comparer);

            // ReSharper disable ExceptionNotDocumented
            return new StringDifferences(
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
        /// <param name="textOptions">The text options.</param>
        /// <param name="comparer">The comparer to compare items in each collection.</param>
        /// <returns>Returns the <see cref="Differences{T}" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="a" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="b" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="offsetA" /> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="lengthA" /> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="offsetB" /> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="lengthB" /> is out of range.</exception>
        /// <exception cref="Exception">The <paramref name="comparer" /> throws an exception.</exception>
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
            TextOptions textOptions,
            [NotNull] Func<char, char, bool> comparer)
            => new StringDifferences(a, offsetA, lengthA, b, offsetB, lengthB, textOptions, comparer);

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
    }
}