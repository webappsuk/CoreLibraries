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
using System.Globalization;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Globalization
{
    /// <summary>
    /// Interface to an object that provides <see cref="CurrencyInfo"/>.
    /// </summary>
    public interface ICurrencyInfoProvider
    {
        /// <summary>
        /// The date this provider was published.
        /// </summary>
        [PublicAPI]
        DateTime Published { get; }

        /// <summary>
        /// The currencies in the provider.
        /// </summary>
        [PublicAPI]
        [NotNull]
        [ItemNotNull]
        IEnumerable<CurrencyInfo> All { get; }

        /// <summary>
        /// Gets the number of currencies specified in the provider.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        int Count { get; }

        /// <summary>
        ///   Retrieves a <see cref="CurrencyInfo"/> with the ISO Code specified.
        /// </summary>
        /// <param name="currencyCode">The ISO Code.</param>
        /// <returns>
        ///   The <see cref="CurrencyInfo"/> that corresponds to the <paramref name="currencyCode"/> specified (if any);
        ///   otherwise the default value for the type is returned.
        /// </returns>
        /// <remarks>
        ///   There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> for this method,
        ///   <paramref name="currencyCode"/> cannot be null.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="currencyCode"/> is <see langword="null"/>.
        /// </exception>
        [PublicAPI]
        [CanBeNull]
        CurrencyInfo Get([NotNull] string currencyCode);

        /// <summary>
        /// Gets the <see cref="CurrencyInfo"/> from the specified region information.
        /// </summary>
        /// <param name="regionInfo">The region information.</param>
        /// <returns>The associated <see cref="CurrencyInfo"/> if found; otherwise <see langword="null"/>.</returns>
        [PublicAPI]
        [CanBeNull]
        CurrencyInfo Get([NotNull] RegionInfo regionInfo);

        /// <summary>
        /// Gets the <see cref="CurrencyInfo" /> from the specified culture information.
        /// </summary>
        /// <param name="cultureInfo">The culture information.</param>
        /// <returns>
        /// The associated <see cref="CurrencyInfo" /> if found; otherwise <see langword="null" />.
        /// </returns>
        [PublicAPI]
        [CanBeNull]
        CurrencyInfo Get([NotNull] CultureInfo cultureInfo);

        /// <summary>
        /// Gets the <see cref="CurrencyInfo" /> from the specified extended culture information.
        /// </summary>
        /// <param name="cultureInfo">The culture information.</param>
        /// <returns>
        /// The associated <see cref="CurrencyInfo" /> if found; otherwise <see langword="null" />.
        /// </returns>
        [PublicAPI]
        [CanBeNull]
        CurrencyInfo Get([NotNull] ExtendedCultureInfo cultureInfo);
    }
}