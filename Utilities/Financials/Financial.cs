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
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Globalization;

namespace WebApplications.Utilities.Financials
{
    /// <summary>
    /// Represents a financial value and currency binding.
    /// </summary>
    public class Financial : IEquatable<Financial>, IFormattable
    {
        private readonly decimal _amount;

        [NotNull]
        private readonly CurrencyInfo _currency;

        /// <summary>
        /// Initializes a new instance of the <see cref="Financial"/> class.
        /// </summary>
        /// <param name="currency">The currency.</param>
        /// <param name="amount">The amount.</param>
        public Financial([NotNull] CurrencyInfo currency, decimal amount)
        {
            Contract.Requires(currency != null, "Parameter 'currency' can not be null");

            _currency = currency;
            _amount = amount;
        }

        /// <summary>
        /// Gets or sets the currency.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public CurrencyInfo Currency
        {
            get { return _currency; }
        }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        [UsedImplicitly]
        public decimal Amount
        {
            get { return _amount; }
        }

        #region IEquatable<Financial> Members
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true"/> if the current object is equal to the other parameter; otherwise, <see langword="false"/>.
        /// </returns>
        public bool Equals(Financial other)
        {
            return other != null && _currency == other._currency && _amount == other._amount;
        }
        #endregion

        #region IFormattable Members
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="format">The format style: "I" for the value followed by the ISO currency code, "C" for a culture specific currency format.</param>
        /// <param name="provider">The format provider.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// <exception cref="FormatException">The format string is not supported.</exception>
        public string ToString(string format, IFormatProvider provider)
        {
            if (String.IsNullOrEmpty(format)) format = "I";
            switch (format.ToUpperInvariant())
            {
                case "I":
                    return String.Format(provider, "{0} {1}", _amount, _currency.Code);
                case "C":
                    CultureInfo culture;
                    try
                    {
                        // Ensure that the format provider passed in contains culture information 
                        culture = (CultureInfo)provider;
                    }
                    catch (InvalidCastException)
                    {
                        culture = CultureInfo.CurrentUICulture;
                    }

                    if (culture == null) culture = CultureInfo.CurrentUICulture;

                    return FormatCurrency(culture);
                default:
                    throw new FormatException(String.Format("The {0} format string is not supported.", format));
            }
        }
        #endregion

        /// <summary>
        /// Exchanges the specified currency.
        /// </summary>
        /// <param name="currency">The currency.</param>
        /// <param name="exchangeRate">The exchange rate.</param>
        /// <param name="inputCharge">The input charge.</param>
        /// <param name="outputCharge">The output charge.</param>
        /// <returns>A <see cref="Financial"/> with the exchange rate and charges applied.</returns>
        public Financial Exchange(
            [NotNull] CurrencyInfo currency,
            decimal exchangeRate = decimal.One,
            decimal inputCharge = decimal.Zero,
            decimal outputCharge = decimal.Zero)
        {
            if (_currency == currency)
                return this;

            decimal amount = _amount;
            amount += inputCharge;
            amount *= exchangeRate;
            amount += outputCharge;

            return new Financial(currency, amount);
        }

        /// <summary>
        /// Sums the amounts specified financials.
        /// </summary>
        /// <param name="financials">The financials.</param>
        /// <returns><see cref="Financial"/> object with an amount of the specified financials summed.</returns>
        public static Financial Sum([NotNull] IEnumerable<Financial> financials)
        {
            if (financials == null) throw new ArgumentNullException("financials");

            Financial[] financialsArray = financials.ToArray();

            if (!financialsArray.Any())
                throw new InvalidOperationException(Resources.Financial_Sum_EmptyEnumeration);

            // ReSharper disable once PossibleNullReferenceException
            decimal summedAmounts = financialsArray.Sum(f => f._amount);
            // ReSharper disable once PossibleNullReferenceException
            return new Financial(financialsArray.First()._currency, summedAmounts);
        }

        /// <summary>
        /// Averages the specified financials.
        /// </summary>
        /// <param name="financials">The financials.</param>
        /// <returns><see cref="Financial"/> object with an amount of the specified financials averaged.</returns>
        public static Financial Average([ItemNotNull] [NotNull] IEnumerable<Financial> financials)
        {
            if (financials == null) throw new ArgumentNullException("financials");

            Financial[] financialsArray = financials.ToArray();

            if (!financialsArray.Any())
                throw new InvalidOperationException(Resources.Financial_Sum_EmptyEnumeration);

            // ReSharper disable PossibleNullReferenceException
            decimal averageAmount = financialsArray.Average(f => f._amount);
            return new Financial(financialsArray.First()._currency, averageAmount);
            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        [NotNull]
        [UsedImplicitly]
        public static Financial operator +([NotNull] Financial a, [NotNull] Financial b)
        {
            if (a == null) throw new ArgumentNullException("a");
            if (b == null) throw new ArgumentNullException("b");

            ValidateCurrenciesMatch(a._currency, b._currency, "Addition");
            return new Financial(a._currency, a._amount + b._amount);
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="financial">The financial.</param>
        /// <param name="amount">The amount.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        [NotNull]
        [UsedImplicitly]
        public static Financial operator +([NotNull] Financial financial, decimal amount)
        {
            return new Financial(financial._currency, financial._amount + amount);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        [NotNull]
        [UsedImplicitly]
        public static Financial operator -([NotNull] Financial a, [NotNull] Financial b)
        {
            ValidateCurrenciesMatch(a._currency, b._currency, "Subtraction");
            return new Financial(a._currency, a._amount - b._amount);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="financial">The financial.</param>
        /// <param name="amount">The amount.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        [NotNull]
        [UsedImplicitly]
        public static Financial operator -([NotNull] Financial financial, decimal amount)
        {
            return new Financial(financial._currency, financial._amount - amount);
        }

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        [UsedImplicitly]
        public static bool operator <([NotNull] Financial a, [NotNull] Financial b)
        {
            ValidateCurrenciesMatch(a._currency, b._currency, "less than");
            return a._amount < b._amount;
        }

        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        [UsedImplicitly]
        public static bool operator >([NotNull] Financial a, [NotNull] Financial b)
        {
            ValidateCurrenciesMatch(a._currency, b._currency, "more than");
            return a._amount > b._amount;
        }

        /// <summary>
        /// Implements the operator &lt;=.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        [UsedImplicitly]
        public static bool operator <=([NotNull] Financial a, [NotNull] Financial b)
        {
            ValidateCurrenciesMatch(a._currency, b._currency, "less than or equal to");
            return a._amount <= b._amount;
        }

        /// <summary>
        /// Implements the operator &gt;=.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        [UsedImplicitly]
        public static bool operator >=([NotNull] Financial a, [NotNull] Financial b)
        {
            ValidateCurrenciesMatch(a._currency, b._currency, "more than or equal to");
            return a._amount >= b._amount;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Financial a, Financial b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (ReferenceEquals(a, null) ||
                ReferenceEquals(b, null)) return false;
            return Equals(a._amount, b._amount) &&
                   Equals(a._currency, b._currency);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Financial a, Financial b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        [UsedImplicitly]
        public static Financial operator *([NotNull] Financial a, [NotNull] Financial b)
        {
            return new Financial(a._currency, a._amount * b._amount);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            Financial financial = obj as Financial;
            if (ReferenceEquals(null, obj)) return false;
            return Equals(financial);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (_currency.GetHashCode() * 397) ^ _amount.GetHashCode();
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return FormatCurrency(CultureInfo.CurrentUICulture);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="format">The format style: "I" for the value followed by the ISO currency code, "C" for a culture specific currency format.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public string ToString(string format)
        {
            return ToString(format, CultureInfo.CurrentUICulture);
        }

        /// <summary>
        /// Formats the currency.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        [NotNull]
        private string FormatCurrency([NotNull] CultureInfo culture)
        {
            ExtendedCultureInfo[] cultures = CultureInfoProvider.Current.FindByCurrency(_currency).ToArray();

            bool found = false;
            foreach (ExtendedCultureInfo c in cultures)
            {
                Debug.Assert(c != null);

                if (string.Equals(culture.Name, c.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    found = true;
                    culture = c;
                    break;
                }

                if (!found &&
                    string.Equals(
                        culture.TwoLetterISOLanguageName,
                        c.TwoLetterISOLanguageName,
                        StringComparison.InvariantCultureIgnoreCase))
                {
                    found = true;
                    culture = c;
                }
            }

            if (!found &&
                cultures.Length > 0)
                culture = cultures[0];

            return String.Format(culture, "{0:C}", _amount);
        }

        /// <summary>
        /// Validates the currencies match.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <param name="operation">The operation.</param>
        private static void ValidateCurrenciesMatch(CurrencyInfo a, CurrencyInfo b, string operation)
        {
            if (a != b)
                throw new InvalidOperationException(
                    string.Format(
                        // ReSharper disable once AssignNullToNotNullAttribute
                        Resources.Financial_Currencies_Do_Not_Match,
                        a,
                        b,
                        operation));
        }
    }
}