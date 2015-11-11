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
using System.Globalization;
using System.Reflection;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    /// A <see cref="char">character</see> comparer.
    /// </summary>
    [Serializable]
    [PublicAPI]
    public abstract class CharComparer : IComparer, IEqualityComparer, IComparer<char>, IEqualityComparer<char>
    {
        /// <summary>
        /// The invariant culture <see cref="CharComparer"/>.
        /// </summary>
        [NotNull]
        public static readonly CharComparer InvariantCulture =
            new CultureAwareComparer(CultureInfo.InvariantCulture.CompareInfo, false);

        /// <summary>
        /// The case-insensitive invariant culture <see cref="CharComparer"/>.
        /// </summary>
        [NotNull]
        public static readonly CharComparer InvariantCultureIgnoreCase =
            new CultureAwareComparer(CultureInfo.InvariantCulture.CompareInfo, true);

        /// <summary>
        /// The ordinal <see cref="CharComparer"/>.
        /// </summary>
        [NotNull]
        public static readonly CharComparer Ordinal = new OrdinalComparer(false);

        /// <summary>
        /// The case-insensitive ordinal <see cref="CharComparer"/>.
        /// </summary>
        [NotNull]
        public static readonly CharComparer OrdinalIgnoreCase = new OrdinalComparer(true);

        /// <summary>
        /// Gets the <see cref="CharComparer"/> for the current <see cref="CultureInfo">culture</see>.
        /// </summary>
        /// <value>A <see cref="CharComparer"/>.</value>
        [NotNull]
        public static CharComparer CurrentCulture
            => new CultureAwareComparer(CultureInfo.CurrentCulture.CompareInfo, false);

        /// <summary>
        /// Gets the case-insensitive <see cref="CharComparer"/> for the current <see cref="CultureInfo">culture</see>.
        /// </summary>
        /// <value>A <see cref="CharComparer"/>.</value>
        [NotNull]
        public static CharComparer CurrentCultureIgnoreCase
            => new CultureAwareComparer(CultureInfo.CurrentCulture.CompareInfo, true);

        /// <summary>
        /// Creates a <see cref="CharComparer"/> based on the specified culture.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="ignoreCase">if set to <see langword="true" /> will ignore case..</param>
        /// <returns>A <see cref="CharComparer"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="culture" /> is <see langword="null" />.</exception>
        [NotNull]
        public static CharComparer Create([NotNull] CultureInfo culture, bool ignoreCase)
        {
            if (culture == null)
                throw new ArgumentNullException(nameof(culture));

            if (culture.Equals(CultureInfo.InvariantCulture))
                return ignoreCase ? InvariantCultureIgnoreCase : InvariantCulture;

            return new CultureAwareComparer(culture.CompareInfo, ignoreCase);
        }

        /// <summary>
        /// Creates a <see cref="CharComparer"/> based on the specified <see cref="CompareInfo"/>.
        /// </summary>
        /// <param name="compareInfo">The compare info.</param>
        /// <param name="ignoreCase">if set to <see langword="true" /> will ignore case..</param>
        /// <returns>A <see cref="CharComparer"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="compareInfo"/> is <see langword="null" />.</exception>
        [NotNull]
        public static CharComparer Create([NotNull] CompareInfo compareInfo, bool ignoreCase)
        {
            if (compareInfo == null)
                throw new ArgumentNullException(nameof(compareInfo));

            if (compareInfo.Equals(CultureInfo.InvariantCulture.CompareInfo))
                return ignoreCase ? InvariantCultureIgnoreCase : InvariantCulture;

            return new CultureAwareComparer(compareInfo, ignoreCase);
        }

        /// <summary>
        /// Creates a <see cref="CharComparer" /> based on the specified <see cref="System.StringComparer" />.
        /// </summary>
        /// <param name="stringComparer">The comparer.</param>
        /// <returns>A <see cref="CharComparer" />.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="ArgumentNullException"><paramref name="stringComparer" /> is <see langword="null" />.</exception>
        [NotNull]
        public static CharComparer Create([NotNull] System.StringComparer stringComparer)
        {
            if (stringComparer == null)
                throw new ArgumentNullException(nameof(stringComparer));

            // Handle common equivalents.
            if (stringComparer.Equals(System.StringComparer.InvariantCulture)) return InvariantCulture;
            if (stringComparer.Equals(System.StringComparer.InvariantCultureIgnoreCase))
                return InvariantCultureIgnoreCase;
            if (stringComparer.Equals(System.StringComparer.Ordinal)) return Ordinal;
            if (stringComparer.Equals(System.StringComparer.OrdinalIgnoreCase)) return OrdinalIgnoreCase;

            // Build a comparer based on the string comparer.
            return new StringComparer(stringComparer);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentException">The specified parameters are not comparable.</exception>
        public int Compare(object x, object y)
        {
            if (x == y) return 0;
            if (x == null) return -1;
            if (y == null) return 1;

            if (x is char && y is char)
                return Compare((char)x, (char)y);

            IComparable ia = x as IComparable;
            if (ia != null)
                return ia.CompareTo(y);

            throw new ArgumentException(Resources.CharComparer_Compare_Invalid_Arguments, nameof(x));
        }

        /// <inheritdoc />
        public new bool Equals(object x, object y)
        {
            if (x == y) return true;
            if (x == null || y == null) return false;

            if (x is char && y is char)
                return Equals((char)x, (char)y);

            return x.Equals(y);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> is <see langword="null" />.</exception>
        public int GetHashCode(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            if (obj is char)
                return GetHashCode((char)obj);
            return obj.GetHashCode();
        }

        /// <inheritdoc />
        public abstract int Compare(char x, char y);

        /// <inheritdoc />
        public abstract bool Equals(char x, char y);

        /// <inheritdoc />
        public abstract int GetHashCode(char obj);

        /// <summary>
        /// Implements a <see cref="CharComparer"/> based on a <see cref="StringComparer"/>.
        /// </summary>
        [Serializable]
        private sealed class StringComparer : CharComparer
        {
            /// <summary>
            /// The string comparer.
            /// </summary>
            [NotNull]
            [PublicAPI]
            private readonly System.StringComparer _stringComparer;

            /// <summary>
            /// Initializes a new instance of the <see cref="CharComparer"/> class.
            /// </summary>
            /// <param name="stringComparer">The string comparer to use.</param>
            public StringComparer([NotNull] System.StringComparer stringComparer)
            {
                _stringComparer = stringComparer;
            }

            /// <inheritdoc />
            public override int Compare(char x, char y) => _stringComparer.Compare(x.ToString(), y.ToString());

            /// <inheritdoc />
            public override bool Equals(char x, char y) => _stringComparer.Equals(x.ToString(), y.ToString());

            /// <inheritdoc />
            public override int GetHashCode(char obj) => _stringComparer.GetHashCode(obj.ToString());

            /// <inheritdoc />
            public override bool Equals(object obj)
            {
                StringComparer comparer = obj as StringComparer;
                return comparer != null &&
                       (_stringComparer == comparer._stringComparer);
            }

            /// <inheritdoc />
            public override int GetHashCode() => _stringComparer.GetHashCode();
        }

        /// <summary>
        /// Implements a <see cref="CharComparer"/> based on a <see cref="CultureInfo"/> and case-sensitivity.
        /// </summary>
        [Serializable]
        private sealed class CultureAwareComparer : CharComparer
        {
            // ReSharper disable AssignNullToNotNullAttribute
            /// <summary>
            /// Gets hash codes for a given <see cref="CompareInfo"/>.
            /// </summary>
            [NotNull]
            private static readonly Func<CompareInfo, string, CompareOptions, int> _getHashCode
                = typeof(CompareInfo)
                    .GetMethod(
                        "GetHashCodeOfString",
                        BindingFlags.Instance | BindingFlags.NonPublic,
                        null,
                        new[] { typeof(string), typeof(CompareOptions) },
                        null)
                    .Func<CompareInfo, string, CompareOptions, int>();

            // ReSharper restore AssignNullToNotNullAttribute


            /// <summary>
            /// The <see cref="CompareInfo"/> is used for actual comparisons.
            /// </summary>
            [NotNull]
            private readonly CompareInfo _compareInfo;

            /// <summary>
            /// Whether case should be ignored.
            /// </summary>
            private readonly bool _ignoreCase;

            /// <summary>
            /// Initializes a new instance of the <see cref="CultureAwareComparer" /> class.
            /// </summary>
            /// <param name="compareInfo">The compare information.</param>
            /// <param name="ignoreCase">Whether to ignore the case.</param>
            public CultureAwareComparer([NotNull] CompareInfo compareInfo, bool ignoreCase)
            {
                _compareInfo = compareInfo;
                _ignoreCase = ignoreCase;
            }

            /// <inheritdoc />
            public override int Compare(char x, char y) =>
                x == y
                    ? 0
                    : _compareInfo.Compare(
                        x.ToString(),
                        y.ToString(),
                        _ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);

            /// <inheritdoc />
            public override bool Equals(char x, char y) =>
                x == y ||
                _compareInfo.Compare(
                    x.ToString(),
                    y.ToString(),
                    _ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None) == 0;

            /// <inheritdoc />
            public override int GetHashCode(char obj) =>
                // ReSharper disable once EventExceptionNotDocumented
                _getHashCode(
                    _compareInfo,
                    obj.ToString(),
                    _ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);

            /// <inheritdoc />
            public override bool Equals(object obj)
            {
                CultureAwareComparer comparer = obj as CultureAwareComparer;
                return comparer != null &&
                       ((_ignoreCase == comparer._ignoreCase) && (_compareInfo.Equals(comparer._compareInfo)));
            }

            /// <inheritdoc />
            public override int GetHashCode()
            {
                int hashCode = _compareInfo.GetHashCode();
                return _ignoreCase ? (~hashCode) : hashCode;
            }
        }

        /// <summary>
        /// Optimized <see cref="CharComparer"/> based on ordinal comparisons with case-sensitivity.
        /// </summary>
        [Serializable]
        internal sealed class OrdinalComparer : CharComparer
        {
            /// <summary>
            /// Whether to ignore case.
            /// </summary>
            private readonly bool _ignoreCase;

            internal OrdinalComparer(bool ignoreCase)
            {
                _ignoreCase = ignoreCase;
            }

            /// <inheritdoc />
            public override int Compare(char x, char y) => x - y;

            /// <inheritdoc />
            public override bool Equals(char x, char y) =>
                _ignoreCase
                    ? char.ToLowerInvariant(x).Equals(char.ToLowerInvariant(y))
                    : x.Equals(y);

            /// <inheritdoc />
            public override int GetHashCode(char obj)
            {
                if (_ignoreCase) obj = char.ToLower(obj);
                return obj.GetHashCode();
            }

            // Equals method for the comparer itself.
            public override bool Equals(object obj)
            {
                OrdinalComparer comparer = obj as OrdinalComparer;
                return comparer != null && _ignoreCase == comparer._ignoreCase;
            }

            /// <summary>
            /// Start of the hash code calculation for this object.
            /// </summary>
            private static readonly int _hashCodeBase = nameof(OrdinalComparer).GetHashCode();

            /// <inheritdoc />
            public override int GetHashCode() => _ignoreCase ? (~_hashCodeBase) : _hashCodeBase;
        }
    }
}