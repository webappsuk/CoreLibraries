#region © Copyright Web Applications (UK) Ltd, 2017.  All rights reserved.
// Copyright (c) 2017, Web Applications UK Ltd
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
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Globalization;

namespace WebApplications.Utilities.Database.Schema
{
    /// <summary>
    /// Holds information about a SQL collation.
    /// </summary>
    /// <seealso cref="System.StringComparer" />
    [PublicAPI]
    public class SqlCollation : DatabaseEntity<SqlCollation>, IEqualityComparer<string>, IComparer<string>
    {
        /// <summary>
        /// The properties used for calculating differences.
        /// </summary>
        [UsedImplicitly]
        [NotNull]
        private static readonly Expression<Func<SqlCollation, object>>[] _properties =
        {
            p => p.Name,
            p => p.CodePage,
            p => p.LCID,
            p => p.Options,
            p => p.Version
        };

        /// <summary>
        /// Gets the name of the collation.
        /// </summary>
        /// <value>The name.</value>
        [NotNull]
        public string Name { get; }

        /// <summary>
        /// Gets the encoding.
        /// </summary>
        /// <value>The encoding.</value>
        [CanBeNull]
        public Encoding Encoding { get; }

        /// <summary>
        /// Gets the code page.
        /// </summary>
        /// <value>The code page.</value>
        public int CodePage { get; }

        /// <summary>
        /// Gets the culture.
        /// </summary>
        /// <value>The culture.</value>
        [CanBeNull]
        public CultureInfo Culture { get; }

        /// <summary>
        /// Gets the locale ID.
        /// </summary>
        /// <value>The locale ID.</value>
        public int LCID { get; }

        /// <summary>
        /// Gets the compare options for comparing strings.
        /// </summary>
        /// <value>The options.</value>
        public CompareOptions Options { get; }

        /// <summary>
        /// Gets the SQL compare options.
        /// </summary>
        /// <value>
        /// The SQL compare options.
        /// </value>
        public SqlCompareOptions SqlCompareOptions
        {
            get
            {
                if (Options == CompareOptions.Ordinal) return SqlCompareOptions.BinarySort;

                SqlCompareOptions sqlOptions = SqlCompareOptions.None;
                
                if ((Options & CompareOptions.IgnoreCase) != 0)
                    sqlOptions |= SqlCompareOptions.IgnoreCase;
                if ((Options & CompareOptions.IgnoreNonSpace) != 0)
                    sqlOptions |= SqlCompareOptions.IgnoreNonSpace;
                if ((Options & CompareOptions.IgnoreKanaType) != 0)
                    sqlOptions |= SqlCompareOptions.IgnoreKanaType;
                if ((Options & CompareOptions.IgnoreWidth) != 0)
                    sqlOptions |= SqlCompareOptions.IgnoreWidth;

                return sqlOptions;
            }
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <value>The version.</value>
        public SqlCollationVersion Version { get; }

        /// <summary>
        /// Gets a value indicating whether this is a SQL server collation.
        /// </summary>
        /// <value>
        ///   <see langword="true" /> if this is a SQL server collation; otherwise, <see langword="false" />.
        /// </value>
        public bool IsSqlServerCollation => Name.StartsWith("SQL_");

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlCollation"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="options">The options.</param>
        /// <param name="version">The version.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="name"/>, <paramref name="encoding"/> or <paramref name="culture"/> was null.
        /// </exception>
        public SqlCollation(
            [NotNull] string name,
            [NotNull] Encoding encoding,
            [NotNull] CultureInfo culture,
            CompareOptions options,
            SqlCollationVersion version) 
            : base(name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
            CodePage = encoding.CodePage;
            Culture = culture ?? throw new ArgumentNullException(nameof(culture));
            LCID = culture.LCID;
            Options = options;
            Version = version;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlCollation"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="codePage">The code page.</param>
        /// <param name="lcid">The lcid.</param>
        /// <param name="comparisonStyle">The comparison style.</param>
        /// <param name="version">The version.</param>
        internal SqlCollation([NotNull] string name, int codePage, int lcid, int comparisonStyle, byte version)
            : base(name)
        {
            Name = name;
            CodePage = checked((ushort)codePage);
            LCID = lcid;
            Version = (SqlCollationVersion)version;

            try
            {
                Encoding = Encoding.GetEncoding(CodePage);
            }
            catch
            {
                /* Ignore */
            }

            try
            {
                Culture = CultureInfo.GetCultureInfo(LCID);
            }
            catch
            {
                /* Ignore */
            }

            if (comparisonStyle == 0)
            {
                Options = CompareOptions.Ordinal;
            }
            else
            {
                // ReSharper disable InconsistentNaming, IdentifierTypo
                // Ignores case
                const int NORM_IGNORECASE = 0x00000001;
                const int LINGUISTIC_IGNORECASE = 0x00000010;
                // Does not differentiate between Hiragana and Katakana characters. Corresponding Hiragana and Katakana will compare as equal.
                const int NORM_IGNOREKANATYPE = 0x00010000;
                // Ignores nonspacing. This flag also removes Japanese accent characters. 
                const int NORM_IGNORENONSPACE = 0x00000002;
                const int LINGUISTIC_IGNOREDIACRITIC = 0x00000020;
                // Ignores symbols.
                const int NORM_IGNORESYMBOLS = 0x00000004;
                // Does not differentiate between a single-byte character and the same character as a double-byte character.
                const int NORM_IGNOREWIDTH = 0x00020000;
                // Treats punctuation the same as symbols.
                const int SORT_STRINGSORT = 0x00001000;
                // ReSharper restore InconsistentNaming, IdentifierTypo

                CompareOptions options = 0;
                if ((comparisonStyle & (NORM_IGNORECASE | LINGUISTIC_IGNORECASE)) != 0)
                    options |= CompareOptions.IgnoreCase;
                if ((comparisonStyle & NORM_IGNOREKANATYPE) != 0)
                    options |= CompareOptions.IgnoreKanaType;
                if ((comparisonStyle & (NORM_IGNORENONSPACE | LINGUISTIC_IGNOREDIACRITIC)) != 0)
                    options |= CompareOptions.IgnoreNonSpace;
                if ((comparisonStyle & NORM_IGNORESYMBOLS) != 0)
                    options |= CompareOptions.IgnoreSymbols;
                if ((comparisonStyle & NORM_IGNOREWIDTH) != 0)
                    options |= CompareOptions.IgnoreWidth;
                if ((comparisonStyle & SORT_STRINGSORT) != 0)
                    options |= CompareOptions.StringSort;
                Options = options;
            }
        }

        /// <summary>When overridden in a derived class, compares two strings and returns an indication of their relative sort order.</summary>
        /// <returns>A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.ValueMeaningLess than zero<paramref name="x" /> is less than <paramref name="y" />.-or-<paramref name="x" /> is null.Zero<paramref name="x" /> is equal to <paramref name="y" />.Greater than zero<paramref name="x" /> is greater than <paramref name="y" />.-or-<paramref name="y" /> is null.</returns>
        /// <param name="x">A string to compare to <paramref name="y" />.</param>
        /// <param name="y">A string to compare to <paramref name="x" />.</param>
        /// <filterpriority>1</filterpriority>
        public int Compare(string x, string y)
        {
            if (Culture == null) throw new NotSupportedException("Culture missing");
            return Culture.CompareInfo.Compare(x, y, Options);
        }

        /// <summary>When overridden in a derived class, indicates whether two strings are equal.</summary>
        /// <returns>true if <paramref name="x" /> and <paramref name="y" /> refer to the same object, or <paramref name="x" /> and <paramref name="y" /> are equal; otherwise, false.</returns>
        /// <param name="x">A string to compare to <paramref name="y" />.</param>
        /// <param name="y">A string to compare to <paramref name="x" />.</param>
        /// <filterpriority>1</filterpriority>
        public bool Equals(string x, string y)
        {
            if (Culture == null) throw new NotSupportedException("Culture missing");
            return Culture.CompareInfo.Compare(x, y, Options) == 0;
        }

        /// <summary>When overridden in a derived class, gets the hash code for the specified string.</summary>
        /// <returns>A 32-bit signed hash code calculated from the value of the <paramref name="str" /> parameter.</returns>
        /// <param name="str">A string.</param>
        /// <exception cref="T:System.ArgumentException">Not enough memory is available to allocate the buffer that is required to compute the hash code.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="str" /> is null. </exception>
        /// <filterpriority>2</filterpriority>
        public int GetHashCode(string str)
        {
            if (Culture == null) throw new NotSupportedException("Culture missing");
            return Culture.CompareInfo.GetHashCode(str, Options);
        }
    }
}