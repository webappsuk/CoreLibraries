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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Difference;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Allows viewing of a string based on specific <see cref="Options"/>.
    /// </summary>
    /// <seealso cref="DifferenceExtensions.ToMapped"/>
    [PublicAPI]
    [Serializable]
    public class StringMap : IReadOnlyList<char>
    {
        /// <summary>
        /// The empty map.
        /// </summary>
        [NotNull]
        private static readonly IReadOnlyList<int> _emptyMap = new[] { 0, 0, 0 };

        /// <summary>
        /// The map, in tuples with the start (inclusive) character index, the chunk length and the cumulative count.
        /// </summary>
        [NotNull]
        protected readonly IReadOnlyList<int> Map;

        /// <summary>
        /// The original string.
        /// </summary>
        [NotNull]
        public readonly string Original;

        /// <summary>
        /// Gets the original number of characters.
        /// </summary>
        /// <value>The original number of characters.</value>
        public int OriginalCount => Original.Length;

        /// <summary>
        /// The mapped string.
        /// </summary>
        [NotNull]
        public readonly string Mapped;

        /// <summary>
        /// The text options.
        /// </summary>
        public readonly TextOptions Options;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringMap" /> class.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="options">The options.</param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="input" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="options" /> are invalid.</exception>
        internal StringMap(
            [NotNull] string input,
            TextOptions options = TextOptions.Default)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (options == (TextOptions.Trim | TextOptions.CollapseWhiteSpace))
                throw new ArgumentOutOfRangeException(nameof(options));
            Original = input;
            Options = options;
            int length = input.Length;
            if (length < 1)
            {
                Mapped = string.Empty;
                Map = _emptyMap;
                return;
            }
            if (options == TextOptions.None)
            {
                Mapped = input;
                Map = new[] { 0, length, length };
                return;
            }

            StringBuilder builder = new StringBuilder(Original.Length);
            List<int> map = new List<int>(4);

            int i = 0;
            int offset = 0;
            int count = 0;
            char c;
            bool skip;

            // Logic for adding a map
            Action<int, int> addMap = (s, e) =>
            {
                int len = e - s;
                count += len;
                map.Add(s);
                map.Add(len);
                map.Add(count);
            };

            // ReSharper disable EventExceptionNotDocumented
            switch (options)
            {
                /* 
                 * Simplest case is to skip all white space - this is less useful in that it effectively ignores word
                 * boundaries.  Note this implicitly 'trims', 'normalizes line endings' and 'collapses white space' as
                 * ALL white space is ignored.
                 */
                case TextOptions.IgnoreWhiteSpace:
                    while (i < length)
                    {
                        c = Original[i++];
                        if (char.IsWhiteSpace(c))
                        {
                            if (i - offset > 1)
                                addMap(offset, i - 1);
                            offset = i;
                            continue;
                        }
                        builder.Append(c);
                    }
                    if (i - offset > 0)
                        addMap(offset, i);
                    break;
                /* 
                 * Collapse white space ignores all but the first white space character, effectively preserving the
                 * word boundaries.  Note this implicitly 'normalizes line endings' as only the first character of
                 * white space is preserved, so only on line ending character will be kept.
                 * 
                 * It makes not sense to allow this in conjunction with trim as line breaks are effectively treated the
                 * same as any white space.
                 */
                case TextOptions.CollapseWhiteSpace:
                    skip = false;
                    while (i < length)
                    {
                        c = Original[i++];
                        if (char.IsWhiteSpace(c))
                        {
                            if (!skip)
                            {
                                skip = true;
                                builder.Append(c);
                                continue;
                            }
                            if (i - offset > 1)
                                addMap(offset, i - 1);
                            offset = i;
                            continue;
                        }
                        builder.Append(c);
                        skip = false;
                    }
                    if (i - offset > 0)
                        addMap(offset, i);
                    break;

                /*
                 * Trim and NormalizeLineEndings both require line detection and so we treat together, they can also
                 * be used in combination.
                 */
                case TextOptions.Trim:
                case TextOptions.NormalizeLineEndings:
                case TextOptions.Trim | TextOptions.NormalizeLineEndings:
                    bool normalizeLineEndings = options.HasFlag(TextOptions.NormalizeLineEndings);
                    bool trim = options.HasFlag(TextOptions.Trim);

                    // Skip is used to indicate we've had the first character in a line.
                    skip = false;

                    // The index after the last non-white space character
                    StringBuilder trailingWhiteSpace = new StringBuilder(16);
                    while (i < length)
                    {
                        c = Original[i++];
                        if (char.IsWhiteSpace(c))
                        {
                            bool isNewLine = c == '\n';
                            // Detect line endings
                            if (isNewLine || c == '\r')
                            {
                                if (i < 2 || !skip || !normalizeLineEndings || !isNewLine)
                                {
                                    if (!skip && (i - offset > 1))
                                    {
                                        int end = i;

                                        // Trim trailing white space if any
                                        if (trim && trailingWhiteSpace.Length > 0)
                                        {
                                            end -= trailingWhiteSpace.Length;
                                            trailingWhiteSpace.Clear();
                                        }
                                        if (end - offset > 1)
                                            addMap(offset, end - 1);
                                        offset = i - 1;
                                    }
                                    // Preserve first line ending when normalizing or all line endings.
                                    skip = true;
                                    builder.Append(c);
                                    continue;
                                }
                                
                                // Skip second line ending character if == '\n'
                                if (i - offset > 1)
                                    addMap(offset, i - 1);
                                offset = i;
                                continue;
                            }
                            if (trim)
                            {
                                if (i < 2 || skip)
                                {
                                    skip = true;
                                    // White space at start of line.
                                    if (i - offset > 1)
                                        addMap(offset, i - 1);
                                    offset = i;
                                    continue;
                                }

                                // Hold whitespace in temporary builder.
                                trailingWhiteSpace.Append(c);
                                continue;
                            }
                        }

                        if (trailingWhiteSpace.Length > 0)
                        {
                            builder.Append(trailingWhiteSpace);
                            trailingWhiteSpace.Clear();
                        }
                        builder.Append(c);
                        skip = false;
                    }

                    i -= trailingWhiteSpace.Length;
                    trailingWhiteSpace.Clear();

                    if (i - offset > 0)
                        addMap(offset, i);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(options), options, null);
            }
            // ReSharper restore EventExceptionNotDocumented

            if (map.Count < 1)
            {
                Mapped = string.Empty;
                Map = _emptyMap;
                return;
            }
            // Store mapped string and the map.
            Mapped = builder.ToString();
            Map = map.ToArray();

#if false
    // Create map
            List<int> map = new List<int>();
            bool ignoreWhiteSpace = options.HasFlag(TextOptions.IgnoreWhiteSpace);
            bool collapseWhiteSpace = ignoreWhiteSpace || options.HasFlag(TextOptions.CollapseWhiteSpace);
            bool normalizeLineEndings = ignoreWhiteSpace || options.HasFlag(TextOptions.NormalizeLineEndings);
            bool trim = ignoreWhiteSpace || options.HasFlag(TextOptions.Trim);

            int o = 0;
            int l = 0;
            int lnws = -1;
            int i = 0;
            int end;

            // Logic for adding a map
            Action<int, int> addMap = (int s, int e) =>
            {
                int mc = map.Count;
                int len = e - s;
                Count += len;
                map.Add(s);
                map.Add(len);
                map.Add(Count);
            };

            // TODO Not working for most tests!

            // ReSharper disable EventExceptionNotDocumented
            while (i < length)
            {
                char c = Original[i++];

                // Check for white space
                if (char.IsWhiteSpace(c))
                {
                    if (collapseWhiteSpace && (ignoreWhiteSpace || lnws < i - 2))
                    {
                        if (lnws - o >= 0 && lnws >= 0)
                            addMap(o, 1 + lnws);

                        o = ignoreWhiteSpace ? i : i - 1;
                        lnws = -1;
                        continue;
                    }
                    if ((c == '\r' || c == '\n') && (i < length))
                    {
                        c = Original[i];
                        if (c == '\r' || c == '\n')
                        {
                            end = trim ? lnws : i - 1;
                            if (end - o >= 0)
                                addMap(o, 1 + end);

                            i++;
                            o = i;
                            lnws = -1;

                            // TODO We have a line ending
                            continue;
                        }
                    }

                    if (trim && lnws < 0)
                        o = i;
                }
                else
                    lnws = i - 1;
            }

            end = ignoreWhiteSpace || trim ? lnws : i - 1;
            if (end - o >= 0)
                addMap(o, 1 + end);

            // ReSharper restore EventExceptionNotDocumented
            // Store map
            Map = map.ToArray();
#endif
        }

        /// <inheritdoc />
        public IEnumerator<char> GetEnumerator() => ((IEnumerable<char>)Mapped).GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => Mapped.GetEnumerator();

        /// <inheritdoc />
        public int Count => Mapped.Length;

        /// <summary>
        /// Gets the <see cref="char" /> at the <paramref name="index">specified index</paramref>.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The specified <see cref="char"/>.</returns>
        /// <exception cref="IndexOutOfRangeException" accessor="get"><paramref name="index" /> is out of range.</exception>
        public char this[int index] => Mapped[index];


        /// <summary>
        /// The <see cref="Map"/> index at which we last found returned a character for our
        /// <see cref="M:Item(int)">indexer</see>.
        /// </summary>
        [NotNull]
        private ThreadLocal<int> _lastMap = new ThreadLocal<int>(() => -1);

        /// <summary>
        /// Gets the index into the <see cref="Original"/> string for the <paramref name="index">specified
        /// index</paramref> into the <see cref="Mapped"/> string.
        /// </summary>
        /// <param name="index">The index in the <see cref="Mapped"/> string.</param>
        /// <returns>The index into the <see cref="Original"/> string.</returns>
        /// <exception cref="IndexOutOfRangeException"><paramref name="index" /> is out of range.</exception>
        /// <remarks>
        /// <para>Lookup is optimized for sequential access (either forward or backwards) on the same thread.</para>
        /// <para>When accessing the index randomly the mapping uses an optimized divide and conquer strategy.</para>
        /// </remarks>
        public int GetOriginalIndex(int index)
        {
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException(nameof(index));

            int startMap = 0;
            int startIndex = 0;
            int endMap = Map.Count - 3;
            int endIndex = Count;
            int cs;

            // Retrieve the last map.
            // ReSharper disable ExceptionNotDocumented
            int lastMap = _lastMap.Value;
            // ReSharper restore ExceptionNotDocumented

            // calculate first split point, we use the last map if we have one (as often requests come in clumps),
            // then we guess based on a linear interpolation.
            int map = lastMap < 0
                ? 3 * (int)((1 + endMap / 3) * ((float)index / Count))
                : lastMap;

            do
            {
                int length = Map[map + 1];
                int cc = Map[map + 2];
                cs = cc - length;
                if (cs <= index)
                {
                    // Check if we have the correct map.
                    if (cc > index) break;

                    // Scan forwards
                    startMap = map + 3;
                    startIndex = cc;
                }
                else
                {
                    // Scan backwards
                    endIndex = cs;
                    endMap = map - 3;
                }

                // Choose next map, again based on linear interpolation,
                // we're effectively using an optimized divide and conquer strategy.
                map = startMap +
                      (3 *
                       // ReSharper disable once PossibleLossOfFraction
                       (int)(((1 + endMap - startMap) / 3) *
                             ((float)(index - startIndex) / (endIndex - startIndex))));
            } while (true);

            // Update last map used to make sequential access quicker.
            // ReSharper disable ExceptionNotDocumentedOptional, ExceptionNotDocumented
            _lastMap.Value = map;
            // ReSharper restore ExceptionNotDocumentedOptional, ExceptionNotDocumented

            return Map[map] + index - cs;
        }

        /// <inheritdoc />
        public override string ToString() => Mapped;
    }
}