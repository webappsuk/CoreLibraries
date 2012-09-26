#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Copyright (c) 2012, Web Applications UK Ltd
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
        /// Initializes a new instance of the <see cref="Range&lt;T&gt;" /> class.
        /// </summary>
        /// <param name="start">The start value (inclusive).</param>
        /// <param name="end">The end value (inclusive).</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// The <paramref name="start" /> value was greater than the <paramref name="end" /> value.
        ///   </exception>
        /// <remarks></remarks>
        public FinancialRange([NotNull] Financial start, [NotNull] Financial end)
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
        public FinancialRange([NotNull] Financial start, [NotNull] Financial end, decimal step)
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
        public FinancialRange([NotNull] Financial start, [NotNull] Financial end, [NotNull] Financial step)
            : base(start, end, step.Amount)
        {
            Contract.Requires(start.Currency == end.Currency, "The currencies in the financial parameters must match.");
            Contract.Requires(step.Currency == start.Currency, "The currencies in the financial parameters must match.");
            Contract.Requires(step.Currency == start.Currency, "The currencies in the financial parameters must match.");
        }

        /// <summary>
        /// Gets the currency.
        /// </summary>
        /// <value>The currency.</value>
        /// <remarks></remarks>
        public CurrencyInfo Currency
        {
            get { return Start.Currency; }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{0} - {1}", Start, End);
        }
    }
}