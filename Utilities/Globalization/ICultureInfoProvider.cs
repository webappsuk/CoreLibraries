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
    /// Interface to an object that provides <see cref="CultureInfo"/>.
    /// </summary>
    [PublicAPI]
    public interface ICultureInfoProvider
    {
        /// <summary>
        /// The date this provider was published.
        /// </summary>
        DateTime Published { get; }

        /// <summary>
        /// The cultures in the provider.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        IEnumerable<ExtendedCultureInfo> All { get; }

        /// <summary>
        /// Gets the number of cultures specified in the provider.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        int Count { get; }

        /// <summary>
        /// Retrieves an <see cref="ExtendedCultureInfo" /> with the name specified (see <see cref="CultureInfo.Name"/>).
        /// </summary>
        /// <param name="cultureName">The ISO Code.</param>
        /// <returns>
        ///   The <see cref="CultureInfo"/> that corresponds to the <paramref name="cultureName"/> specified (if any);
        ///   otherwise the default value for the type is returned.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="cultureName"/> is <see langword="null"/>.
        /// </exception>
        [CanBeNull]
        ExtendedCultureInfo Get([NotNull] string cultureName);

        /// <summary>
        /// Retrieves an <see cref="ExtendedCultureInfo" /> equivalent to the specified (see <see cref="CultureInfo"/>).
        /// </summary>
        /// <param name="cultureInfo">The culture info.</param>
        /// <returns>
        ///   The <see cref="CultureInfo"/> that corresponds to the <paramref name="cultureInfo"/> specified (if any);
        ///   otherwise the default value for the type is returned.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="cultureInfo"/> is <see langword="null"/>.
        /// </exception>
        [CanBeNull]
        ExtendedCultureInfo Get([NotNull] CultureInfo cultureInfo);

        /// <summary>
        /// Finds the cultures that use a specific currency.
        /// </summary>
        /// <param name="currencyInfo">The currency information.</param>
        /// <returns>The cultures that us the specified currency.</returns>
        [NotNull]
        IEnumerable<ExtendedCultureInfo> FindByCurrency([NotNull] CurrencyInfo currencyInfo);
    }
}