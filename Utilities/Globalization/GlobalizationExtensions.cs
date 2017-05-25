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
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using NodaTime.Text;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Globalization
{
    /// <summary>
    /// Provides extension methods for <see cref="CurrencyInfo"/>.
    /// </summary>
    [PublicAPI]
    public static class GlobalizationExtensions
    {
        /// <summary>
        /// Converts this <see cref="CurrencyInfoProvider" /> to binary.
        /// </summary>
        /// <param name="currencyInfoProvider">The currency information provider.</param>
        /// <param name="stream">The binary stream to save to.</param>
        /// <param name="leaveOpen">if set to <see langword="true" /> the <paramref name="stream" /> will be left open.</param>
        public static void ToBinary(
            [NotNull] this ICurrencyInfoProvider currencyInfoProvider,
            [NotNull] Stream stream,
            bool leaveOpen = false)
        {
            if (currencyInfoProvider == null) throw new ArgumentNullException(nameof(currencyInfoProvider));
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            using (BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen))
            {
                writer.Write(CurrencyInfoProvider.BinaryHeader);
                writer.Write(currencyInfoProvider.Published.Ticks);

                int count = currencyInfoProvider.Count;
                writer.Write(count);
                if (count < 1) return;

                foreach (CurrencyInfo currency in currencyInfoProvider.All)
                {
                    writer.Write(currency.ISONumber);
                    writer.Write(currency.Code);
                    writer.Write(currency.FullName);
                    writer.Write(currency.Exponent.HasValue);
                    if (currency.Exponent.HasValue)
                        writer.Write(currency.Exponent.Value);
                    writer.Write(currency.IsLatest);
                }
            }
        }

        /// <summary>
        /// Converts this <see cref="CurrencyInfoProvider"/> to XML.
        /// </summary>
        /// <returns>An XML string containing the contents of this <see cref="CurrencyInfoProvider"/>.</returns>
        [NotNull]
        public static string ToXml([NotNull] this ICurrencyInfoProvider currencyInfoProvider)
        {
            if (currencyInfoProvider == null) throw new ArgumentNullException(nameof(currencyInfoProvider));

            XElement ccyTbl = new XElement("CcyTbl");

            XDocument doc = new XDocument(
                new XElement(
                    "ISO_4217",
                    new XAttribute("Pblshd", currencyInfoProvider.Published.ToString(InstantPattern.ExtendedIsoPattern.PatternText, null)),
                    ccyTbl));

            foreach (CurrencyInfo currency in currencyInfoProvider.All)
            {
                XElement ccyNtry = new XElement(
                    "CcyNtry",
                    new XElement("CcyNm", currency.FullName),
                    new XElement("Ccy", currency.Code),
                    new XElement("CcyNbr", currency.ISONumber));
                if (currency.Exponent.HasValue)
                    ccyNtry.Add(new XElement("CcyMnrUnts", currency.Exponent.Value));
                if (!currency.IsLatest)
                    ccyNtry.Add(new XAttribute("IsLatest", "false"));

                ccyTbl.Add(ccyNtry);
            }

            return doc.ToString();
        }

        /// <summary>
        /// Merges a provider with another provider. If the same currency is in both, the one in the latest published provider will be used.
        /// Currencies that only appeared in the oldest provider will be marked as out of date.
        /// </summary>
        /// <param name="first">The first provider.</param>
        /// <param name="second">The second provider.</param>
        /// <returns></returns>
        [NotNull]
        public static ICurrencyInfoProvider Merge(
            [NotNull] this ICurrencyInfoProvider first,
            [NotNull] ICurrencyInfoProvider second)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));

            ICurrencyInfoProvider newest = first.Published > second.Published ? first : second;
            second = first.Published > second.Published ? second : first;

            bool areSameDate = newest.Published == second.Published;

            // Optimise out empty providers
            if (newest.Count < 1)
                return second.Count < 1 ? newest : second;
            if (second.Count < 1)
                return newest;

            Dictionary<string, CurrencyInfo> currencies = newest.All.ToDictionary(
                c => c.Code,
                StringComparer.InvariantCultureIgnoreCase);

            foreach (CurrencyInfo currency in second.All)
                if (!currencies.ContainsKey(currency.Code))
                    currencies.Add(currency.Code, areSameDate ? currency.GetOutOfDate() : currency);

            Debug.Assert(currencies.Count > 0);
            return new CurrencyInfoProvider(newest.Published, currencies.Values);
        }
    }
}