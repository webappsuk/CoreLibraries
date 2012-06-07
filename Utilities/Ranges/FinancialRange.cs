using WebApplications.Utilities.Financials;

namespace WebApplications.Utilities.Ranges
{
    public class FinancialRange : Range<Financial>
    {
        public FinancialRange(Financial start, Financial end) 
            : base(start, end)
        { }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{0} - {1}", Start, End);
        }
    }
}
