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
using System.Linq;
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
        /// The original string as a character array.
        /// </summary>
        [NotNull]
        private readonly char[] _original;

        /// <summary>
        /// The original string.
        /// </summary>
        [NotNull]
        public string Original => new string(_original);

        /// <summary>
        /// Gets the original number of characters.
        /// </summary>
        /// <value>The original number of characters.</value>
        public int OriginalCount => _original.Length;

        /// <summary>
        /// The mapped string.
        /// </summary>
        [NotNull]
        public string Mapped => new string(this.ToArray());

        /// <summary>
        /// The text options.
        /// </summary>
        public readonly TextOptions Options;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringMap"/> class.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="options">The options.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        internal StringMap(
            [NotNull] string input,
            TextOptions options = TextOptions.Default)
        {
            if (input == null) throw new ArgumentNullException();
            Options = options;
            int length = input.Length;
            if (length < 1)
            {
                _original = Array<char>.Empty;
                Map = _emptyMap;
                return;
            }

            _original = input.ToCharArray();
            if (options == TextOptions.None)
            {
                Map = new[] { 0, length, length };
                Count = length;
                return;
            }

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
                char c = _original[i++];

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
                        c = _original[i];
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
        }

        /// <inheritdoc />
        public IEnumerator<char> GetEnumerator()
        {
            for (int m = 0; m < Map.Count;)
            {
                int start = Map[m++];
                int end = start + Map[m++];
                // Skip count
                m++;
                for (int i = start; i < end; i++)
                    yield return _original[i];
            }
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<char>)this).GetEnumerator();

        /// <inheritdoc />
        public int Count { get; private set; }

        /// <summary>
        /// Gets the <see cref="char" /> at the <paramref name="index">specified index</paramref>.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The specified <see cref="char"/>.</returns>
        /// <exception cref="IndexOutOfRangeException" accessor="get"><paramref name="index" /> is out of range.</exception>
        /// <remarks>
        /// <para>The indexer is optimized for sequential access (either forward or backwards) on the same thread.</para>
        /// <para>When accessing the index randomly the mapping uses an optimized divide and conquer strategy.</para>
        /// </remarks>
        public char this[int index] => _original[GetOriginalIndex(index)];


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