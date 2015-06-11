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
using System.Globalization;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Globalization
{
    /// <summary>
    /// Provides extended information about a specific culture.
    /// </summary>
    public class ExtendedCultureInfo : CultureInfo
    {
        /// <summary>
        /// Gets the associated region information.
        /// </summary>
        /// <value>
        /// The region information.
        /// </value>
        [PublicAPI]
        [CanBeNull]
        public readonly RegionInfo RegionInfo;

        /// <summary>
        /// Gets the three-character ISO 4217 currency symbol associated with the cultures region, 
        /// or <see langword="null"/> if the culture does not have a region.
        /// </summary>
        /// <value>
        /// The three-character ISO 4217 currency symbol associated with the cultures region,
        /// or <see langword="null"/> if the culture does not have a region.
        /// </value>
        [PublicAPI]
        [CanBeNull]
        public string ISOCurrencySymbol
        {
            get { return RegionInfo == null ? null : RegionInfo.ISOCurrencySymbol; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedCultureInfo" /> class.
        /// </summary>
        /// <param name="cultureInfo">The culture information.</param>
        internal ExtendedCultureInfo([NotNull] CultureInfo cultureInfo)
            : base(cultureInfo.Name, cultureInfo.UseUserOverride)
        {
            // Neutral cultures do not have a region
            if (!cultureInfo.IsNeutralCulture)
                try
                {
                    RegionInfo = new RegionInfo(cultureInfo.Name);
                }
                catch (ArgumentException)
                {
                    // There is no region for the culture
                }
        }
    }
}