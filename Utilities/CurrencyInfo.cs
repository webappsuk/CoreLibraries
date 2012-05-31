#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities
// File: CurrencyInfo.cs
// 
// This software, its object code and source code and all modifications made to
// the same (the “Software”) are, and shall at all times remain, the proprietary
// information and intellectual property rights of Web Applications (UK) Limited. 
// You are only entitled to use the Software as expressly permitted by Web
// Applications (UK) Limited within the Software Customisation and
// Licence Agreement (the “Agreement”).  Any copying, modification, decompiling,
// distribution, licensing, sale, transfer or other use of the Software other than
// as expressly permitted in the Agreement is expressly forbidden.  Web
// Applications (UK) Limited reserves its rights to take action against you and
// your employer in accordance with its contractual and common law rights
// (including injunctive relief) should you breach the terms of the Agreement or
// otherwise infringe its copyright or other intellectual property rights in the
// Software.
// 
// © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using JetBrains.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    ///   Holds extended information about currencies.
    /// </summary>
    /// <remarks>
    ///   <see cref="CurrencyInfo"/> contains:
    ///   <list type="bullet">
    ///     <item><description>The currency ISO Code. (http://www.xe.com/iso4217.php)</description></item>
    ///     <item><description>The currency ISO Number. (http://www.xe.com/iso4217.php)</description></item>
    ///     <item><description>The exponent, which is the number of decimals available in the currency.</description></item>
    ///     <item><description>The currency's full name.</description></item>
    ///     <item><description>Associated <see cref="RegionInfo">regions</see>.</description></item>
    ///     <item><description>Associated <see cref="CultureInfo">cultures</see>.</description></item>
    ///   </list>
    /// </remarks>
    public class CurrencyInfo
    {
        /// <summary>
        ///   Stores currency info (by code).
        /// </summary>
        private static readonly Dictionary<string, CurrencyInfo> _currencyInfos = new Dictionary<string, CurrencyInfo>();

        /// <summary>
        ///   Stores the <see cref="RegionInfo"/>s associated with <see cref="CurrencyInfo"/>.
        /// </summary>
        private static readonly Dictionary<RegionInfo, CurrencyInfo> _currencyInfoRegions =
            new Dictionary<RegionInfo, CurrencyInfo>();

        /// <summary>
        ///   Stores the <see cref="CultureInfo"/>s associated with <see cref="CurrencyInfo"/>.
        /// </summary>
        private static readonly Dictionary<CultureInfo, CurrencyInfo> _currencyInfoCultures =
            new Dictionary<CultureInfo, CurrencyInfo>();

        /// <summary>
        ///   The enumerator to iterate through the associated <see cref="CultureInfo">cultures</see>.
        /// </summary>
        public readonly IEnumerable<CultureInfo> Cultures;

        /// <summary>
        ///   The enumerator to iterate through the associated <see cref="RegionInfo">regions</see>.
        /// </summary>
        public readonly IEnumerable<RegionInfo> Regions;

        /// <summary>
        ///   Gets the ISO Code.
        /// </summary>
        [NotNull]
        public string Code { get; private set; }

        /// <summary>
        ///   Gets the ISO Number.
        /// </summary>
        public int ISONumber { get; private set; }

        /// <summary>
        ///   Gets the exponent, which is the number of decimals available in the currency.
        /// </summary>
        [CanBeNull]
        public int? Exponent { get; private set; }

        /// <summary>
        ///   Gets the currency's full name.
        /// </summary>
        [NotNull]
        public string FullName { get; private set; }

        /// <summary>
        ///   Load currency resources, which are located in Currencies.resx.
        /// </summary>
        static CurrencyInfo()
        {
            ResourceManager resourceManager = 
                new ResourceManager("WebApplications.Utilities.Currencies", Assembly.GetExecutingAssembly());

            ResourceSet resourceSet = resourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            if (resourceSet == null)
                return;

            foreach (DictionaryEntry entry in resourceSet)
                CreateCurrencyInfo(entry);
        }

        private static void CreateCurrencyInfo(DictionaryEntry entry)
        {
            string currencyCode = entry.Key.ToString();
            if ((currencyCode.Length != 3) || (_currencyInfos.ContainsKey(currencyCode)))
                return;

            string[] details = entry.Value.ToString().Split(',');

            if (details.Length != 3)
                return;

            int isoNumber = int.Parse(details[0]);
            int? exponent = ParseExponent(details[1]);
            string fullName = details[2];

            CurrencyInfo currencyInfo = new CurrencyInfo(currencyCode, isoNumber, exponent, fullName);
            _currencyInfos.Add(currencyCode, currencyInfo);

            AddRegions(currencyInfo);
            AddCultures(currencyInfo);
        }

        private static void AddCultures(CurrencyInfo info)
        {
            foreach (CultureInfo culture in info.Cultures.Where(culture => !_currencyInfoCultures.ContainsKey(culture)))
                _currencyInfoCultures.Add(culture, info);
        }

        private static void AddRegions(CurrencyInfo info)
        {
            foreach (RegionInfo region in info.Regions.Where(region => !_currencyInfoRegions.ContainsKey(region)))
                _currencyInfoRegions.Add(region, info);
        }

        private static int? ParseExponent([NotNull]string value)
        {
            if (value.Length <= 0)
                return null;

            int e;
            if (!int.TryParse(value, out e))
                return null;
            int? exponent = e;
            return exponent;
        }

        /// <summary>
        ///   Initialize a new instance of <see cref="CurrencyInfo"/>.
        /// </summary>
        /// <param name="code">The ISO Code.</param>
        /// <param name="isoNumber">The ISO Number.</param>
        /// <param name="exponent">
        ///   The exponent, which is the number of decimals available in the currency.
        /// </param>
        /// <param name="fullName">The currency's full name.</param>
        private CurrencyInfo([NotNull] string code, int isoNumber, [CanBeNull] int? exponent,
                             [NotNull] string fullName)
        {
            Code = code;
            ISONumber = isoNumber;
            Exponent = exponent;
            FullName = fullName;
            Regions = CultureHelper.RegionInfoFromCurrencyISO(code);
            Cultures = CultureHelper.CultureInfoFromCurrencyISO(code);
        }



        /// <summary>
        ///   Retrieves a <see cref="CurrencyInfo"/> with the ISO Code specified.
        /// </summary>
        /// <param name="currencyCode">The ISO Code.</param>
        /// <returns>
        ///   The <see cref="CurrencyInfo"/> that corresponds to the <paramref name="currencyCode"/> specified (if any);
        ///   otherwise the default value for the type is returned.
        /// </returns>
        /// <remarks>l
        ///   There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> for this method,
        ///   <paramref name="currencyCode"/> cannot be null.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="currencyCode"/> is <see langword="null"/>.
        /// </exception>
        [CanBeNull]
        public static CurrencyInfo Get([NotNull] string currencyCode)
        {
            Contract.Requires(currencyCode != null, Resources.CurrencyInfo_CurrencyCodeCannotBeNull);

            CurrencyInfo currencyInfo;
            _currencyInfos.TryGetValue(currencyCode, out currencyInfo);
            return currencyInfo;
        }

        /// <summary>
        ///   Retrieves the <see cref="CurrencyInfo"/> with the <see cref="RegionInfo"/> specified.
        /// </summary>
        /// <param name="regionInfo">The region info.</param>
        /// <returns>
        ///   The <see cref="CurrencyInfo"/> that corresponds to the <paramref name="regionInfo"/> specified (if any);
        ///   otherwise the default value for the type is returned.
        /// </returns>
        /// <remarks>
        ///   There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> for this method,
        ///   <paramref name="regionInfo"/> cannot be null.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="regionInfo"/> is <see langword="null"/>.
        /// </exception>
        [CanBeNull]
        public static CurrencyInfo Get([NotNull] RegionInfo regionInfo)
        {
            Contract.Requires(regionInfo != null, Resources.CurrencyInfo_RegionInfoCannotBeNull);

            CurrencyInfo currencyInfo;
            _currencyInfoRegions.TryGetValue(regionInfo, out currencyInfo);
            return currencyInfo;
        }

        /// <summary>
        ///   Retrieves the <see cref="CurrencyInfo"/> using the <see cref="CultureInfo"/> specified.
        /// </summary>
        /// <param name="cultureInfo">The culture info.</param>
        /// <returns>
        ///   The <see cref="CurrencyInfo"/> that corresponds to the <paramref name="cultureInfo"/> specified (if any);
        ///   otherwise the default value for the type is returned.
        /// </returns>
        /// <remarks>
        ///   There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> for this method,
        ///   <paramref name="cultureInfo"/> cannot be null.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="cultureInfo"/> is <see langword="null"/>.
        /// </exception>
        [CanBeNull]
        public static CurrencyInfo Get([NotNull] CultureInfo cultureInfo)
        {
            Contract.Requires(cultureInfo != null, Resources.CurrencyInfo_CultureInfoCannotBeNull);

            CurrencyInfo currencyInfo;
            _currencyInfoCultures.TryGetValue(cultureInfo, out currencyInfo);
            return currencyInfo;
        }

        /// <summary>
        ///   Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        ///   A <see cref="string"/> representation of this instance. The format string can be changed in the 
        ///   Resources.resx resource file at the key 'CurrencyInfoToString'.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   The format string was a <see langword="null"/>.
        /// </exception>
        /// <exception cref="FormatException">
        ///   An index from the format string is either less than zero or greater than or equal to the number of arguments.
        /// </exception>
        public override string ToString()
        {
            return string.Format(Resources.CurrencyInfo_ToString, FullName, Code, ISONumber);
        }
    }
}