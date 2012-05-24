using System;

namespace WebApplications.Utilities.Financials
{
    /// <summary>
    /// Represents a financial value and currency binding.
    /// </summary>
    public class Financial : ICloneable, IEquatable<Financial>
    {
        /// <summary>
        /// Gets or sets the currency.
        /// </summary>
        /// <value>
        /// The currency.
        /// </value>
        public CurrencyInfo Currency { get; private set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        /// <value>
        /// The amount.
        /// </value>
        public decimal Amount { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Financial"/> class.
        /// </summary>
        /// <param name="currency">The currency.</param>
        /// <param name="amount"> </param>
        public Financial(CurrencyInfo currency, decimal amount)
        {
            Currency = currency;
            Amount = amount;
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Financial operator +(Financial a, Financial b)
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
        public static Financial operator +(Financial financial, decimal amount)
        {
            Financial clone = (Financial)financial.Clone();
            clone.Amount = financial.Amount + amount;
            return clone;
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Financial operator -(Financial a, Financial b)
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
        public static Financial operator -(Financial financial, decimal amount)
        {
            Financial clone = (Financial)financial.Clone();
            clone.Amount = financial.Amount - amount;
            return clone;
        }

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator <(Financial a, Financial b)
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
        public static bool operator >(Financial a, Financial b)
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
        public static bool operator <=(Financial a, Financial b)
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
        public static bool operator >=(Financial a, Financial b)
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
        public static Financial operator *(Financial a, Financial b)
        {
            return new Financial(a.Currency, a.Amount * b.Amount);
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
                        "The currency of the first operand '{0}' did not match that of the second '{1}' during an {2} operation.",
                        a,
                        b,
                        operation));
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
            return string.Format("Financial {0}{1}", Amount, Currency.Code);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return new Financial(Currency, Amount);
        }
    }
}
