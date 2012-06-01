using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Financials
{
    /// <summary>
    /// Represents a financial value and currency binding.
    /// </summary>
    public class Financial : IEquatable<Financial>
    {
        /// <summary>
        /// Gets or sets the currency.
        /// </summary>
        /// <value>
        /// The currency.
        /// </value>
        [NotNull]
        [UsedImplicitly]
        public CurrencyInfo Currency { get; private set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        /// <value>
        /// The amount.
        /// </value>
        [UsedImplicitly]
        public decimal Amount { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Financial"/> class.
        /// </summary>
        /// <param name="currency">The currency.</param>
        /// <param name="amount"> </param>
        public Financial([NotNull]CurrencyInfo currency, decimal amount)
        {
            Contract.Requires(currency != null, "Parameter 'currency' can not be null");

            Currency = currency;
            Amount = amount;
        }

        /// <summary>
        /// Exchanges the specified currency.
        /// </summary>
        /// <param name="currency">The currency.</param>
        /// <param name="exchangeRate">The exchange rate.</param>
        /// <param name="inputCharge">The input charge.</param>
        /// <param name="outputCharge">The output charge.</param>
        /// <returns>A <see cref="Financial"/> with the exchange rate and charges applied.</returns>
        public Financial Exchange([NotNull] CurrencyInfo currency, decimal exchangeRate = decimal.One, decimal inputCharge = decimal.Zero, decimal outputCharge = decimal.Zero)
        {
            if (Currency == currency)
                return this;

            decimal amount = Amount;
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
            Financial[] financialsArray = financials.ToArray();

            if (!financialsArray.Any())
                throw new InvalidOperationException(Resources.Financial_Sum_EmptyEnumeration);

            decimal summedAmounts = financialsArray.Sum(f => f.Amount);
            return new Financial(financialsArray.First().Currency, summedAmounts);
        }

        /// <summary>
        /// Averages the specified financials.
        /// </summary>
        /// <param name="financials">The financials.</param>
        /// <returns><see cref="Financial"/> object with an amount of the specified financials averaged.</returns>
        public static Financial Average(IEnumerable<Financial> financials)
        {
            Financial[] financialsArray = financials.ToArray();

            if (!financialsArray.Any())
                throw new InvalidOperationException(Resources.Financial_Sum_EmptyEnumeration);

            decimal averageAmount = financialsArray.Average(f => f.Amount);
            return new Financial(financialsArray.First().Currency, averageAmount);
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
        public static Financial operator +([NotNull]Financial a, [NotNull]Financial b)
        {
            ValidateCurrenciesMatch(a.Currency, b.Currency, "Addition");
            return new Financial(a.Currency, a.Amount + b.Amount);
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
        public static Financial operator +([NotNull]Financial financial, decimal amount)
        {
            return new Financial(financial.Currency, financial.Amount + amount);
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
        public static Financial operator -([NotNull]Financial a, [NotNull]Financial b)
        {
            ValidateCurrenciesMatch(a.Currency, b.Currency, "Subtraction");
            return new Financial(a.Currency, a.Amount - b.Amount);
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
        public static Financial operator -([NotNull]Financial financial, decimal amount)
        {
            return new Financial(financial.Currency, financial.Amount - amount);
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
        public static bool operator <([NotNull]Financial a, [NotNull]Financial b)
        {
            ValidateCurrenciesMatch(a.Currency, b.Currency, "less than");
            return a.Amount < b.Amount;
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
        public static bool operator >([NotNull]Financial a, [NotNull]Financial b)
        {
            ValidateCurrenciesMatch(a.Currency, b.Currency, "more than");
            return a.Amount > b.Amount;
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
        public static bool operator <=([NotNull]Financial a, [NotNull]Financial b)
        {
            ValidateCurrenciesMatch(a.Currency, b.Currency, "less than or equal to");
            return a.Amount <= b.Amount;
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
        public static bool operator >=([NotNull]Financial a, [NotNull]Financial b)
        {
            ValidateCurrenciesMatch(a.Currency, b.Currency, "more than or equal to");
            return a.Amount >= b.Amount;
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
        public static Financial operator *([NotNull]Financial a, [NotNull]Financial b)
        {
            return new Financial(a.Currency, a.Amount * b.Amount);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the other parameter; otherwise, false.
        /// </returns>
        public bool Equals(Financial other)
        {
            return other != null && Currency == other.Currency && Amount == other.Amount;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            CultureInfo culture = CultureInfo.CurrentUICulture;
            if (!Currency.Cultures.Contains(CultureInfo.CurrentUICulture))
            {
                List<CultureInfo> matchingCultures = Currency.Cultures.Where(c=>c.TwoLetterISOLanguageName==CultureInfo.CurrentUICulture.TwoLetterISOLanguageName).ToList();
                if( matchingCultures.Count > 0 )
                {
                    culture = matchingCultures.First();
                } else
                {
                    if( Currency.Cultures.Any() )
                    {
                        culture = Currency.Cultures.First();
                    }
                }
            }
            return string.Format(culture,"Financial {0:C}", Amount);
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
                        Resources.Financial_Currencies_Do_Not_Match,
                        a,
                        b,
                        operation));
        }
    }
}
