using System.Diagnostics.Contracts;
using JetBrains.Annotations;
using WebApplications.Utilities.Financials;

namespace WebApplications.Utilities.Ranges
{
    /// <summary>
    /// A range of <see cref="Financial"/> financial.
    /// </summary>
    /// <remarks></remarks>
    public class FinancialRange : Range<Financial>
    {
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
            : base(start, end, new Financial(start.Currency, step))
        {
            Contract.Requires(start.Currency == end.Currency, "The currencies in the financial parameters must match.");
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{0} - {1}", Start, End);
        }
    }
}
