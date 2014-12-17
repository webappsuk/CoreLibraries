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

#region Using Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Threading;
using WebApplications.Utilities.Annotations;

#endregion

namespace WebApplications.Utilities.Globalization
{
    /// <summary>
    ///   Helps map cultures, regions and currencies.
    /// </summary>
    public static class CultureHelper
    {
        /// <summary>
        ///   A lookup of <see cref="CultureInfo"/>s and <see cref="System.Globalization.RegionInfo"/>s
        ///   by currency ISO code (e.g. USD, GBP, JPY).
        /// </summary>
        [NotNull]
        private static readonly Dictionary<string, Dictionary<CultureInfo, RegionInfo>> _currencyCultureInfo;

        /// <summary>
        ///   A lookup of regions by their English name.
        /// </summary>
        [NotNull]
        private static readonly Dictionary<string, RegionInfo> _regionNames;

        /// <summary>
        ///   All the specified culture names.
        /// </summary>
        [NotNull]
        private static readonly Dictionary<string, CultureInfo> _cultureNames;

        /// <summary>
        ///   The invariant culture LCID.
        /// </summary>
        /// <seealso cref="System.Globalization.CultureInfo.InvariantCulture"/>
        [UsedImplicitly]
        public static readonly int InvariantLCID;

        /// <summary>
        ///   Gets the cultures (both specific and neutral) as well as the currency and
        ///   <see cref="System.Globalization.RegionInfo"/>.
        /// </summary>
        /// <seealso cref="Globalization.CurrencyInfo"/>
        static CultureHelper()
        {
            // get the list of cultures. We are not interested in neutral cultures, since
            // currency and RegionInfo is only applicable to specific cultures
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            int length = cultures.GetLength(0);
            _currencyCultureInfo = new Dictionary<string, Dictionary<CultureInfo, RegionInfo>>(
                length,
                StringComparer
                    .InvariantCultureIgnoreCase);
            _regionNames = new Dictionary<string, RegionInfo>(length, StringComparer.InvariantCultureIgnoreCase);
            _cultureNames = new Dictionary<string, CultureInfo>(length, StringComparer.InvariantCultureIgnoreCase);
            InvariantLCID = CultureInfo.InvariantCulture.LCID;

            foreach (CultureInfo ci in cultures)
            {
                // Create a RegionInfo from culture id. 
                // RegionInfo holds the currency ISO code
                RegionInfo ri = ci.RegionInfo();

                if (!_cultureNames.ContainsKey(ci.Name))
                    _cultureNames.Add(ci.Name, ci);

                // multiple cultures can have the same currency code
                Dictionary<CultureInfo, RegionInfo> cdict;
                if (!_currencyCultureInfo.TryGetValue(ri.ISOCurrencySymbol, out cdict))
                {
                    cdict = new Dictionary<CultureInfo, RegionInfo>();
                    _currencyCultureInfo.Add(ri.ISOCurrencySymbol, cdict);
                }
                Contract.Assert(cdict != null);
                cdict.Add(ci, ri);
                if (!_regionNames.ContainsKey(ri.EnglishName))
                    _regionNames.Add(ri.EnglishName, ri);
            }

            // Now add neutral culture names.
            foreach (CultureInfo ci in
                CultureInfo.GetCultures(CultureTypes.NeutralCultures)
                    .Distinct()
                    .Where(ci => !_cultureNames.ContainsKey(ci.Name)))
                _cultureNames.Add(ci.Name, ci);
        }

        /// <summary>
        ///   Gets the culture names (specific and neutral cultures).
        /// </summary>
        /// <remarks>
        ///   This is particularly useful when looking for culture specific directories (e.g. for resource files).
        /// </remarks>
        [UsedImplicitly]
        [NotNull]
        public static IEnumerable<string> CultureNames
        {
            get { return _cultureNames.Keys; }
        }

        /// <summary>
        ///   Gets the region names.
        /// </summary>
        /// <remarks>
        ///   This is particularly useful when looking for culture specific directories (e.g. for resource files).
        /// </remarks>
        [UsedImplicitly]
        [NotNull]
        public static IEnumerable<string> RegionNames
        {
            get { return _regionNames.Keys; }
        }

        /// <summary>
        ///   Gets the region names.
        /// </summary>
        /// <remarks>
        ///   This is particularly useful when looking for culture specific directories (e.g. for resource files).
        /// </remarks>
        [UsedImplicitly]
        [NotNull]
        public static IEnumerable<string> CurrencyNames
        {
            get { return _currencyCultureInfo.Keys; }
        }

        /// <summary>
        /// Tries to get the culture info with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><see langword="true" /> if found, otherwise <see langword="false" />.</returns>
        public static CultureInfo GetCultureInfo(string name)
        {
            CultureInfo cultureInfo;
            return _cultureNames.TryGetValue(name, out cultureInfo) ? cultureInfo : null;
        }

        /// <summary>
        /// Tries to get the culture info with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><see langword="true" /> if found, otherwise <see langword="false" />.</returns>
        public static RegionInfo GetRegionInfo(string name)
        {
            RegionInfo regionInfo;
            return _regionNames.TryGetValue(name, out regionInfo) ? regionInfo : null;
        }

        /// <summary>
        ///   Gets the <see cref="System.Globalization.RegionInfo"/> for the specified
        ///   <see cref="CultureInfo">culture</see>.
        /// </summary>
        /// <param name="cultureInfo">The specific culture information.</param>
        /// <returns>
        ///   The corresponding <see cref="System.Globalization.RegionInfo"/> for the
        ///   <paramref name="cultureInfo"/> specified.
        /// </returns>
        /// <remarks>
        ///   There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> for this method,
        ///   <paramref name="cultureInfo"/> cannot be null.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="cultureInfo"/> is <see langword="null"/>.
        /// </exception>
        [NotNull]
        public static RegionInfo RegionInfo([NotNull] this CultureInfo cultureInfo)
        {
            Contract.Requires(cultureInfo != null, Resources.CultureHelper_CultureInfoCannotBeNull);

            return new RegionInfo(cultureInfo.LCID);
        }

        /// <summary>
        ///   Gets the currency info for the specified region (if any).
        /// </summary>
        /// <param name="regionInfo">The region information.</param>
        /// <returns>
        ///   The corresponding currency info for the <paramref name="regionInfo"/> specified.
        /// </returns>
        /// <remarks>
        ///   There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> for this method,
        ///   <paramref name="regionInfo"/> cannot be null.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="regionInfo"/> is <see langword="null"/>.
        /// </exception>
        /// <seealso cref="Globalization.CurrencyInfo"/>
        [CanBeNull]
        public static CurrencyInfo CurrencyInfo([NotNull] this RegionInfo regionInfo)
        {
            Contract.Requires(regionInfo != null, Resources.CultureHelper_RegionInfoCannotBeNull);
            
            return CurrencyInfoProvider.Current.Get(regionInfo.ISOCurrencySymbol);
        }

        /// <summary>
        ///   Gets the <see cref="Globalization.CurrencyInfo">currency info</see> for the specified culture (if any).
        /// </summary>
        /// <param name="cultureInfo">The specific culture information.</param>
        /// <returns>
        ///   The corresponding currency info for the <paramref name="cultureInfo"/> specified.
        /// </returns>
        /// <remarks>
        ///   There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> for this method,
        ///   <paramref name="cultureInfo"/> cannot be null.
        /// </remarks>
        /// <seealso cref="Globalization.CurrencyInfo"/>
        [CanBeNull]
        public static CurrencyInfo CurrencyInfo([NotNull] this CultureInfo cultureInfo)
        {
            Contract.Requires(cultureInfo != null, Resources.CultureHelper_CultureInfoCannotBeNull);

            return CurrencyInfoProvider.Current.Get(cultureInfo);
        }

        /// <summary>
        ///   Lookup a <see cref="CultureInfo"/> by the specified currency ISO code.
        /// </summary>
        /// <param name="isoCode">The ISO Code.</param>
        /// <returns>
        ///   A list of <see cref="CultureInfo"/>s that have the specified currency <paramref name="isoCode"/>.
        /// </returns>
        /// <remarks>
        ///   There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> for this method,
        ///   <paramref name="isoCode"/> cannot be null.
        /// </remarks>
        [NotNull]
        public static IEnumerable<CultureInfo> CultureInfoFromCurrencyISO([NotNull] string isoCode)
        {
            Contract.Requires(isoCode != null, Resources.CultureHelper_RegionInfoCannotBeNull);

            if (string.IsNullOrEmpty(isoCode))
                return new List<CultureInfo>(0);
            return _currencyCultureInfo.ContainsKey(isoCode)
                ? new List<CultureInfo>(_currencyCultureInfo[isoCode].Keys.Distinct())
                : Enumerable.Empty<CultureInfo>();
        }

        /// <summary>
        ///   Lookup <see cref="System.Globalization.RegionInfo">region information</see> by the currency ISO code.
        /// </summary>
        /// <param name="isoCode">The ISO Code.</param>
        /// <returns>
        ///   A list of <see cref="System.Globalization.RegionInfo"/>s that have the specified currency.
        /// </returns>
        /// <remarks>
        ///   There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> for this method,
        ///   <paramref name="isoCode"/> cannot be null.
        /// </remarks>
        [NotNull]
        public static IEnumerable<RegionInfo> RegionInfoFromCurrencyISO([NotNull] string isoCode)
        {
            Contract.Requires(isoCode != null, Resources.CultureHelper_IsoCodeCannotBeNull);

            if (string.IsNullOrEmpty(isoCode))
                return new List<RegionInfo>(0);
            return _currencyCultureInfo.ContainsKey(isoCode)
                ? new List<RegionInfo>(_currencyCultureInfo[isoCode].Values.Distinct())
                : Enumerable.Empty<RegionInfo>();
        }

        /// <summary>
        ///   Format a <see cref="decimal"/> value to a <see cref="string"/> using the currency format specified.
        ///   If the specified currency ISO Code doesn't match a <see cref="CultureInfo">culture</see> then there
        ///   will be no currency symbol and the <paramref name="currencyISO"/> Code will be the prefix.
        /// </summary>
        /// <param name="amount">The numerical amount to format.</param>
        /// <param name="currencyISO">The currency ISO Code.</param>
        /// <param name="countryISO">The country ISO Code.</param>
        /// <returns>
        ///   A formatted <see cref="string"/> in the correct currency format.
        /// </returns>
        /// <remarks>
        ///   There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> for this method,
        ///   <paramref name="currencyISO"/> cannot be null.
        /// </remarks>
        /// <seealso cref="CultureInfo"/>
        /// <seealso cref="System.Globalization.RegionInfo"/>
        [NotNull]
        public static string FormatCurrency(
            decimal amount,
            [NotNull] string currencyISO,
            [CanBeNull] string countryISO = null)
        {
            Contract.Requires(currencyISO != null, Resources.CultureHelper_CurrencyIsoCannotBeNull);

            CultureInfo[] c = null;

            if (!string.IsNullOrEmpty(currencyISO))
                c = CultureInfoFromCurrencyISO(currencyISO).ToArray();

            if ((c != null) &&
                (c.Length > 0))
            {
                CultureInfo cinfo = c[0];
                if (countryISO != null)
                    // Find best match
                    for (int i = c.Length - 1; i >= 0; i--)
                    {
                        cinfo = c[i];
                        if (cinfo == null) continue;
                        RegionInfo r = new RegionInfo(cinfo.LCID);
                        if (r.TwoLetterISORegionName.Equals(countryISO))
                            break;
                    }

                return FormatCurrency(amount, cinfo);
            }

            // If currency ISO code doesn't match any culture
            // create a new culture without currency symbol
            // and use the ISO code as a prefix (e.g. YEN 123,123.00)
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-GB");
            culture.NumberFormat.CurrencySymbol = string.Empty;
            culture.NumberFormat.CurrencyDecimalDigits = 2;
            culture.NumberFormat.CurrencyDecimalSeparator = ".";
            culture.NumberFormat.CurrencyGroupSeparator = ",";

            return String.Format("{0} {1}", currencyISO, amount.ToString("C", culture.NumberFormat));
        }

        /// <summary>
        ///   Format a <see cref="decimal"/> value to a <see cref="string"/> using the specified <see cref="CultureInfo"/>.
        /// </summary>
        /// <param name="amount">The numerical amount to format.</param>
        /// <param name="cultureInfo">
        ///   <para>The culture info.</para>
        ///   <para>If this is a null value then the thread's <see cref="System.Threading.Thread.CurrentUICulture">current
        ///   UI culture</see> is used.</para>
        /// </param>
        /// <returns>
        ///   A formatted <see cref="string"/> in the correct currency format.
        /// </returns>
        [NotNull]
        public static string FormatCurrency(decimal amount, [CanBeNull] CultureInfo cultureInfo = null)
        {
            cultureInfo = cultureInfo ?? Thread.CurrentThread.CurrentUICulture;
            return amount.ToString("C", cultureInfo.NumberFormat);
        }

        /// <summary>
        ///   Retrieves the <see cref="System.Globalization.RegionInfo"/> using the
        ///   <see cref="System.Globalization.RegionInfo.DisplayName">display name</see> specified.
        /// </summary>
        ///   <param name="name">The display name (the country's name in the current culture).</param>
        /// <returns>
        ///   The <see cref="System.Globalization.RegionInfo"/> that corresponds to the <paramref name="name"/> specified.
        /// </returns>
        /// <remarks>
        ///   There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> for this method,
        ///   <paramref name="name"/> cannot be null.
        /// </remarks>
        /// <seealso cref="CultureInfo"/>
        /// <seealso cref="System.Globalization.RegionInfo.DisplayName"/>
        [CanBeNull]
        public static RegionInfo FindRegionFromName([NotNull] string name)
        {
            Contract.Requires(name != null, Resources.CultureHelper_NameCannotBeNull);

            if (string.IsNullOrEmpty(name))
                return null;

            RegionInfo r;
            if (_regionNames.TryGetValue(name, out r))
                return r;
            // Check if we're in English, if we are we've finished looking.
            CultureInfo culture = Thread.CurrentThread.CurrentUICulture;

            // Scan regions for name in current culture.
            return culture.TwoLetterISOLanguageName.Equals("en")
                ? null
                : _regionNames.Values.FirstOrDefault(
                    info => (info.DisplayName.Equals(name)) || (info.NativeName.Equals(name)));
        }

        /// <summary>
        ///   Retrieves the <see cref="System.Globalization.RegionInfo"/> using the display name specified.
        /// </summary>
        /// <param name="name">The display name (the country's name in the current culture).</param>
        /// <returns>
        ///   The <see cref="System.Globalization.RegionInfo"/> that corresponds to the <paramref name="name"/> specified.
        /// </returns>
        /// <remarks>
        ///   There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> for this method,
        ///   <paramref name="name"/> cannot be null.
        /// </remarks>
        [CanBeNull]
        public static RegionInfo FindRegion([NotNull] string name)
        {
            Contract.Requires(name != null, Resources.CultureHelper_NameCannotBeNull);

            if (string.IsNullOrEmpty(name))
                return null;

            RegionInfo r;
            try
            {
                r = new RegionInfo(name);
            }
            catch (ArgumentException)
            {
                r = null;
            }
            return r ?? FindRegionFromName(name);
        }

        /// <summary>
        ///   Determines whether or not the <see cref="CultureInfo"/> is the invariant culture.
        ///   (http://msdn.microsoft.com/en-us/library/4c5zdc6a.aspx)
        /// </summary>
        /// <param name="cultureInfo">The culture info.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the specified <paramref name="cultureInfo"/> is invariant; otherwise returns <see langword="false"/>.
        /// </returns>
        /// <remarks>
        ///   The invariant culture is culture-insensitive, it is useful for when culture-specific presentation isn't required/needed.
        /// </remarks>
        /// <seealso cref="System.Globalization.CultureInfo.InvariantCulture"/>
        public static bool IsInvariant(this CultureInfo cultureInfo)
        {
            return cultureInfo != null && cultureInfo.LCID == InvariantLCID;
        }
    }
}