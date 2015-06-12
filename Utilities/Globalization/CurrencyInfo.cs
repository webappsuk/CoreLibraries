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

#if !BUILD_TASKS
using System;
#endif
using System.Globalization;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Globalization
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
        /// Initialize a new instance of <see cref="CurrencyInfo" />.
        /// </summary>
        /// <param name="code">The ISO Code.</param>
        /// <param name="isoNumber">The ISO Number.</param>
        /// <param name="exponent">The exponent, which is the number of decimals available in the currency.</param>
        /// <param name="fullName">The currency's full name.</param>
        /// <param name="isLatest">if set to <see langword="true" /> the currency appears in the latest official ISO 4217 list.</param>
        public CurrencyInfo(
            [NotNull] string code,
            int isoNumber,
            [CanBeNull] int? exponent,
            [NotNull] string fullName,
            bool isLatest)
        {
            Code = code;
            ISONumber = isoNumber;
            Exponent = exponent;
            FullName = fullName;
            IsLatest = isLatest;
        }

        /// <summary>
        /// Gets the out of date equivalent of this <see cref="CurrencyInfo"/>.
        /// </summary>
        /// <returns></returns>
        public CurrencyInfo GetOutOfDate()
        {
            return !IsLatest ? this : new CurrencyInfo(Code, ISONumber, Exponent, FullName, false);
        }

        /// <summary>
        ///   Gets the ISO Code.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public readonly string Code;

        /// <summary>
        ///   Gets the ISO Number.
        /// </summary>
        [PublicAPI]
        public readonly int ISONumber;

        /// <summary>
        ///   Gets the exponent, which is the number of decimals available in the currency.
        /// </summary>
        [CanBeNull]
        [PublicAPI]
        public readonly int? Exponent;

        /// <summary>
        ///   Gets the currency's full name.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public readonly string FullName;

        /// <summary>
        /// Indicates that this currency appears in the latest official ISO 4217 list.
        /// </summary>
        [PublicAPI]
        public readonly bool IsLatest;

#if !BUILD_TASKS
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
            // ReSharper disable once AssignNullToNotNullAttribute
            return string.Format(Resources.CurrencyInfo_ToString, FullName, Code, ISONumber);
        }
#endif
    }
}