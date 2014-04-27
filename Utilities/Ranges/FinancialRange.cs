#region © Copyright Web Applications (UK) Ltd, 2014.  All rights reserved.
// Copyright (c) 2014, Web Applications UK Ltd
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
using System.Diagnostics.Contracts;
using JetBrains.Annotations;
using WebApplications.Utilities.Financials;

namespace WebApplications.Utilities.Ranges
{
    /// <summary>
    /// A range of <see cref="Financial"/> financial.
    /// </summary>
    public class FinancialRange : Range<Financial, decimal>, IFormattable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Range&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="start">The start value (inclusive).</param>
        /// <param name="end">The end value (inclusive).</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// The <paramref name="start"/> value was greater than the <paramref name="end"/> value.
        /// </exception>
        public FinancialRange([NotNull] Financial start, [NotNull] Financial end)
            : base(start, end, 1)
        {
            Contract.Requires(start != null);
            Contract.Requires(end != null);
            Contract.Requires(start.Currency == end.Currency, "The currencies in the financial parameters must match.");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Range&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="start">The start value.</param>
        /// <param name="end">The end value (inclusive).</param>
        /// <param name="step">The range step (inclusive).</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// The <paramref name="start"/> value was greater than the <paramref name="end"/> value.
        /// </exception>
        public FinancialRange([NotNull] Financial start, [NotNull] Financial end, decimal step)
            : base(start, end, step)
        {
            Contract.Requires(start != null);
            Contract.Requires(end != null);
            Contract.Requires(start.Currency == end.Currency, "The currencies in the financial parameters must match.");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Range&lt;T,S&gt;"/> class.
        /// </summary>
        /// <param name="start">The start value (inclusive).</param>
        /// <param name="end">The end value (inclusive).</param>
        /// <param name="step">The range step.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///   The <paramref name="start"/> value was greater than the <paramref name="end"/> value.
        /// </exception>
        public FinancialRange([NotNull] Financial start, [NotNull] Financial end, [NotNull] Financial step)
            : base(start, end, step.Amount)
        {
            Contract.Requires(start != null);
            Contract.Requires(end != null);
            Contract.Requires(start.Currency == end.Currency, "The currencies in the financial parameters must match.");
            Contract.Requires(step.Currency == start.Currency, "The currencies in the financial parameters must match.");
            Contract.Requires(step.Currency == end.Currency, "The currencies in the financial parameters must match.");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Range&lt;T,S&gt;"/> class.
        /// </summary>
        /// <param name="currency">The currency information.</param>
        /// <param name="start">The start value (inclusive).</param>
        /// <param name="end">The end value (inclusive).</param>
        public FinancialRange([NotNull] CurrencyInfo currency, decimal start, decimal end)
            : base(new Financial(currency, start), new Financial(currency, end), 1)
        {
            Contract.Requires(currency != null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Range&lt;T,S&gt;"/> class.
        /// </summary>
        /// <param name="currency">The currency information.</param>
        /// <param name="start">The start value (inclusive).</param>
        /// <param name="end">The end value (inclusive).</param>
        /// <param name="step">The range step.</param>
        public FinancialRange([NotNull] CurrencyInfo currency, decimal start, decimal end, decimal step)
            : base(new Financial(currency, start), new Financial(currency, end), step)
        {
            Contract.Requires(currency != null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Range&lt;T,S&gt;"/> class.
        /// </summary>
        /// <param name="currency">The currency information.</param>
        /// <param name="start">The start value (inclusive).</param>
        /// <param name="end">The end value (inclusive).</param>
        /// <param name="step">The range step.</param>
        public FinancialRange([NotNull] CurrencyInfo currency, decimal start, decimal end, [NotNull] Financial step)
            : base(new Financial(currency, start), new Financial(currency, end), step.Amount)
        {
            Contract.Requires(currency != null);
            Contract.Requires(step.Currency == currency, "The currencies in the financial parameters must match.");
        }

        /// <summary>
        /// Gets the currency.
        /// </summary>
        public CurrencyInfo Currency
        {
            get { return Start.Currency; }
        }

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
            return string.Format("{0} - {1}", Start.ToString(format, provider), End.ToString(format, provider));
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            // ReSharper disable RedundantToStringCall
            return string.Format("{0} - {1}", Start.ToString(), End.ToString());
            // ReSharper restore RedundantToStringCall
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
            return string.Format("{0} - {1}", Start.ToString(format), End.ToString(format));
        }
    }
}