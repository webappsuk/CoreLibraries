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
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Globalization
{
    /// <summary>
    /// Provides <see cref="CultureInfo" />.
    /// </summary>
    [PublicAPI]
    public class CultureInfoProvider : ICultureInfoProvider
    {
        /// <summary>
        /// A <see cref="ICurrencyInfoProvider" /> with no currencies!
        /// </summary>
        private class EmptyCultureInfoProvider : ICultureInfoProvider
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="EmptyCultureInfoProvider"/> class.
            /// </summary>
            /// <param name="published">The published.</param>
            public EmptyCultureInfoProvider(DateTime published)
            {
                Published = published;
            }

            /// <summary>
            /// The date this provider was published.
            /// </summary>
            public DateTime Published { get; }

            /// <summary>
            /// The cultures in the provider.
            /// </summary>
            public IEnumerable<ExtendedCultureInfo> All => Enumerable.Empty<ExtendedCultureInfo>();

            /// <summary>
            /// Gets the number of cultures specified in the provider.
            /// </summary>
            /// <value>
            /// The count.
            /// </value>
            public int Count => 0;

            /// <summary>
            /// Retrieves an <see cref="ExtendedCultureInfo" /> with the ISO Code specified.
            /// </summary>
            /// <param name="cultureName">The ISO Code.</param>
            /// <returns>
            /// The 
            /// <see cref="CultureInfo" /> that corresponds to the 
            /// <paramref name="cultureName" /> specified (if any);
            ///   otherwise the default value for the type is returned.
            /// </returns>
            public ExtendedCultureInfo Get(string cultureName)
            {
                return null;
            }

            /// <summary>
            /// Retrieves an <see cref="ExtendedCultureInfo" /> with the name specified <see cref="CultureInfo.LCID" />.
            /// </summary>
            /// <param name="lcid">The lcid.</param>
            /// <returns>
            /// The <see cref="ExtendedCultureInfo" /> that corresponds to the <paramref name="lcid" /> specified (if any);
            /// otherwise <see langword="null" />.
            /// </returns>
            public ExtendedCultureInfo Get(int lcid)
            {
                return null;
            }

            /// <summary>
            /// Retrieves an <see cref="ExtendedCultureInfo" /> equivalent to the specified (see <see cref="CultureInfo" />).
            /// </summary>
            /// <param name="cultureInfo">The culture info.</param>
            /// <returns>
            /// The 
            /// <see cref="CultureInfo" /> that corresponds to the 
            /// <paramref name="cultureInfo" /> specified (if any);
            ///   otherwise the default value for the type is returned.
            /// </returns>
            public ExtendedCultureInfo Get(CultureInfo cultureInfo)
            {
                return null;
            }

            /// <summary>
            /// Finds the cultures that use a specific currency.
            /// </summary>
            /// <param name="currencyInfo">The currency information.</param>
            /// <returns>
            /// The cultures that us the specified currency.
            /// </returns>
            public IEnumerable<ExtendedCultureInfo> FindByCurrency(CurrencyInfo currencyInfo)
            {
                return Enumerable.Empty<ExtendedCultureInfo>();
            }

            /// <summary>
            /// Gets the child cultures of the specified culture.
            /// </summary>
            /// <param name="cultureInfo">The culture to get the children of.</param>
            /// <returns>The child cultures of the specified culture.</returns>
            public IEnumerable<ExtendedCultureInfo> GetChildren(CultureInfo cultureInfo)
            {
                return Enumerable.Empty<ExtendedCultureInfo>();
            }
        }

        /// <summary>
        /// The empty culture info provider has no cultures.
        /// </summary>
        [NotNull]
        public static readonly ICultureInfoProvider Empty;

        /// <summary>
        /// The provider that supplies the BCL <see cref="CultureInfo">cultures</see>.
        /// </summary>
        [NotNull]
        public static readonly BclCultureInfoProvider Bcl;

        [CanBeNull]
        private static readonly FieldInfo _cultureParentField = typeof(CultureInfo).GetField(
            "m_parent",
            BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// The current provider.
        /// </summary>
        [NotNull]
        private static ICultureInfoProvider _current;

        /// <summary>
        /// Gets or sets the current provider.
        /// </summary>
        /// <value>
        /// The current.
        /// </value>
        [NotNull]
        public static ICultureInfoProvider Current
        {
            get { return _current; }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                _current = value;
            }
        }

        /// <summary>
        /// Initializes the <see cref="CultureInfoProvider"/> class.
        /// </summary>
        static CultureInfoProvider()
        {
            Empty = new EmptyCultureInfoProvider(DateTime.MinValue);

            // Create BCL provider
            // Note we must update the DateTime whenever we build against a new framework.
            _current = Bcl = new BclCultureInfoProvider();
        }

        /// <summary>
        /// Stores culture info (by name).
        /// </summary>
        [NotNull]
        private readonly IReadOnlyDictionary<string, ExtendedCultureInfo> _cultureInfos;

        /// <summary>
        /// Stores culture info (by LCID).
        /// </summary>
        [NotNull]
        private readonly IReadOnlyDictionary<int, ExtendedCultureInfo> _cultureInfosByLCID;

        /// <summary>
        /// The cultures by currency code.
        /// </summary>
        [NotNull]
        private readonly IReadOnlyDictionary<string, IEnumerable<ExtendedCultureInfo>> _currencyCultureInfos;

        /// <summary>
        /// The child cultures of each culture.
        /// </summary>
        [NotNull]
        private readonly IReadOnlyDictionary<ExtendedCultureInfo, IEnumerable<ExtendedCultureInfo>> _childCultures;

        /// <summary>
        /// Initializes a new instance of the <see cref="CultureInfoProvider" /> class.
        /// </summary>
        /// <param name="published">The published date time.</param>
        /// <param name="cultures">The cultures.</param>
        public CultureInfoProvider(
            DateTime published,
            [ItemNotNull] [NotNull] IEnumerable<ExtendedCultureInfo> cultures)
        {
            if (cultures == null) throw new ArgumentNullException(nameof(cultures));

            Published = published;
            // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute
            _cultureInfos = cultures.Distinct().ToDictionary(c => c.Name, StringComparer.InvariantCultureIgnoreCase);
            _cultureInfosByLCID = _cultureInfos.Values
                .Where(c => c.LCID != 4096) // Filter out custom cultures
                .GroupBy(c => c.LCID)
                .Select(
                    g =>
                    {
                        ExtendedCultureInfo[] v = g.ToArray();
                        return v.Length == 1 ? v[0] : v.OrderBy(GetDepth).ThenBy(c => c.Name).First();
                    })
                .ToDictionary(c => c.LCID);
            _currencyCultureInfos = _cultureInfos.Values
                .OrderBy(c => c.Name)
                .GroupBy(c => c.ISOCurrencySymbol, StringComparer.InvariantCultureIgnoreCase)
                .Where(g => g.Key != null)
                .ToDictionary(
                    g => g.Key,
                    g => (IEnumerable<ExtendedCultureInfo>)g.ToArray(),
                    StringComparer.InvariantCultureIgnoreCase);

            if (_cultureParentField != null)
                foreach (ExtendedCultureInfo culture in _cultureInfos.Values)
                {
                    ExtendedCultureInfo parent;
                    if (_cultureInfos.TryGetValue(culture.Parent.Name, out parent))
                        _cultureParentField.SetValue(culture, parent);
                    else if (culture.IsInvariant)
                        _cultureParentField.SetValue(culture, culture);
                }

            _childCultures = _cultureInfos.Values
                .Where(c => !c.IsInvariant)
                .OrderBy(c => c.Name)
                .GroupBy(c => c.Parent)
                .ToDictionary(
                    g => g.Key,
                    g => (IEnumerable<ExtendedCultureInfo>)g.ToArray());
            // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Gets the depth of the given culture info.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns></returns>
        private static int GetDepth([NotNull] ExtendedCultureInfo info)
        {
            int i = 0;
            while (!info.IsInvariant)
            {
                i++;
                info = info.Parent;
            }
            return i;
        }

        /// <summary>
        /// The date this provider was published.
        /// </summary>
        public DateTime Published { get; }

        /// <summary>
        /// The cultures in the provider.
        /// </summary>
        public IEnumerable<ExtendedCultureInfo> All
        {
            get
            {
                Debug.Assert(_cultureInfos.Values != null);
                return _cultureInfos.Values;
            }
        }

        /// <summary>
        /// Gets the number of cultures specified in the provider.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int Count => _cultureInfos.Count;

        /// <summary>
        /// Retrieves an <see cref="ExtendedCultureInfo" /> with the name specified <see cref="CultureInfo.Name" />.
        /// </summary>
        /// <param name="cultureName">The ISO Code.</param>
        /// <returns>
        /// The <see cref="ExtendedCultureInfo" /> that corresponds to the <paramref name="cultureName" /> specified (if any);
        /// otherwise <see langword="null" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="cultureName" /> is <see langword="null" />.</exception>
        public ExtendedCultureInfo Get(string cultureName)
        {
            if (cultureName == null) throw new ArgumentNullException(nameof(cultureName));
            ExtendedCultureInfo cultureInfo;
            _cultureInfos.TryGetValue(cultureName, out cultureInfo);
            return cultureInfo;
        }

        /// <summary>
        /// Retrieves an <see cref="ExtendedCultureInfo" /> with the name specified <see cref="CultureInfo.LCID" />.
        /// </summary>
        /// <param name="lcid">The lcid.</param>
        /// <returns>
        /// The <see cref="ExtendedCultureInfo" /> that corresponds to the <paramref name="lcid" /> specified (if any);
        /// otherwise <see langword="null" />.
        /// </returns>
        public ExtendedCultureInfo Get(int lcid)
        {
            ExtendedCultureInfo cultureInfo;
            _cultureInfosByLCID.TryGetValue(lcid, out cultureInfo);
            return cultureInfo;
        }

        /// <summary>
        /// Retrieves an <see cref="ExtendedCultureInfo" /> equivalent to the <see cref="CultureInfo" />.
        /// </summary>
        /// <param name="cultureInfo">The culture info.</param>
        /// <returns>
        /// The <see cref="CultureInfo" /> that corresponds to the <paramref name="cultureInfo" /> specified (if any);
        /// otherwise <see langword="null" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="cultureInfo" /> is <see langword="null" />.</exception>
        public ExtendedCultureInfo Get(CultureInfo cultureInfo)
        {
            if (cultureInfo == null) throw new ArgumentNullException(nameof(cultureInfo));
            ExtendedCultureInfo eci;
            _cultureInfos.TryGetValue(cultureInfo.Name, out eci);
            return eci;
        }

        /// <summary>
        /// Finds the cultures that use a specific currency.
        /// </summary>
        /// <param name="currencyInfo">The currency information.</param>
        /// <returns>
        /// The cultures that us the specified currency.
        /// </returns>
        public IEnumerable<ExtendedCultureInfo> FindByCurrency(CurrencyInfo currencyInfo)
        {
            if (currencyInfo == null) throw new ArgumentNullException(nameof(currencyInfo));
            IEnumerable<ExtendedCultureInfo> cultures;
            // ReSharper disable once AssignNullToNotNullAttribute
            return _currencyCultureInfos.TryGetValue(currencyInfo.Code, out cultures)
                ? cultures
                : Enumerable.Empty<ExtendedCultureInfo>();
        }

        /// <summary>
        /// Gets the child cultures of the specified culture.
        /// </summary>
        /// <param name="cultureInfo">The culture to get the children of.</param>
        /// <returns>The child cultures of the specified culture.</returns>
        public IEnumerable<ExtendedCultureInfo> GetChildren(CultureInfo cultureInfo)
        {
            if (cultureInfo == null) throw new ArgumentNullException(nameof(cultureInfo));
            IEnumerable<ExtendedCultureInfo> children;
            // ReSharper disable once AssignNullToNotNullAttribute
            return _childCultures.TryGetValue(Get(cultureInfo), out children)
                ? children
                : Enumerable.Empty<ExtendedCultureInfo>();
        }
    }
}