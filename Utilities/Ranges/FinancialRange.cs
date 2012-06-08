using System.Diagnostics.Contracts;
using JetBrains.Annotations;
using WebApplications.Utilities.Financials;

namespace WebApplications.Utilities.Ranges
{
    /// <summary>
    /// A range of <see cref="Financial"/> financial.
    /// </summary>
    /// <remarks></remarks>
    public class FinancialRange : Range<Financial, decimal>
    {
        /// <summary>
        /// Gets the currency.
        /// </summary>
        /// <value>The currency.</value>
        /// <remarks></remarks>
        public CurrencyInfo Currency { get { return Start.Currency; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Range&lt;T&gt;" /> class.
        /// </summary>
        /// <param name="start">The start value (inclusive).</param>
        /// <param name="end">The end value (inclusive).</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// The <paramref name="start" /> value was greater than the <paramref name="end" /> value.
        ///   </exception>
        /// <remarks></remarks>
        public FinancialRange([NotNull]Financial start, [NotNull]Financial end) 
            : base(start, end)
        {
            Contract.Requires(start.Currency == end.Currency, "The currencies in the financial parameters must match.");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Range&lt;T&gt;" /> class.
        /// </summary>
        /// <param name="start">The start value.</param>
        /// <param name="end">The end value (inclusive).</param>
        /// <param name="step">The range step (inclusive).</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// The <paramref name="start" /> value was greater than the <paramref name="end" /> value.
        ///   </exception>
        /// <remarks></remarks>
        public FinancialRange([NotNull]Financial start, [NotNull]Financial end, decimal step)
            : base(start, end, step)
        {
            Contract.Requires(start.Currency == end.Currency, "The currencies in the financial parameters must match.");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Range&lt;T,S&gt;" /> class.
        /// </summary>
        /// <param name="start">The start value (inclusive).</param>
        /// <param name="end">The end value (inclusive).</param>
        /// <param name="step">The range step.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// The <paramref name="start" /> value was greater than the <paramref name="end" /> value.
        ///   </exception>
        /// <remarks></remarks>
        public FinancialRange([NotNull]Financial start, [NotNull]Financial end, Financial step) 
            : base(start, end, step.Amount)
        {
            Contract.Requires(start.Currency == end.Currency, "The currencies in the financial parameters must match.");
            Contract.Requires(step.Currency == start.Currency, "The currencies in the financial parameters must match.");
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{0} - {1}", Start, End);
        }
    }
}
