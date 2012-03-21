#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities
// File: DecimalRange.cs
// 
// This software, its object code and source code and all modifications made to
// the same (the “Software”) are, and shall at all times remain, the proprietary
// information and intellectual property rights of Web Applications (UK) Limited. 
// You are only entitled to use the Software as expressly permitted by Web
// Applications (UK) Limited within the Software Customisation and
// Licence Agreement (the “Agreement”).  Any copying, modification, decompiling,
// distribution, licensing, sale, transfer or other use of the Software other than
// as expressly permitted in the Agreement is expressly forbidden.  Web
// Applications (UK) Limited reserves its rights to take action against you and
// your employer in accordance with its contractual and common law rights
// (including injunctive relief) should you breach the terms of the Agreement or
// otherwise infringe its copyright or other intellectual property rights in the
// Software.
// 
// © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
#endregion

namespace WebApplications.Utilities.Ranges
{
    /// <summary>
    ///   A range of <see cref="decimal"/>s.
    /// </summary>
    public class DecimalRange : Range<decimal>
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="DecimalRange"/> class.
        /// </summary>
        /// <param name="start">The start number (inclusive).</param>
        /// <param name="end">The end number (inclusive).</param>
        /// <param name="step">
        ///   <para>The step between each value in the range.</para>
        ///   <para>By default this is set to 1.0M.</para>
        /// </param>
        /// <remarks>
        ///   Imprecise step values can result in cumulative rounding errors.
        /// </remarks>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///   The <paramref name="start"/> value was greater than the <paramref name="end"/> value.
        /// </exception>
        public DecimalRange(decimal start, decimal end, decimal step = 1.0M)
            : base(start, end, step)
        {
        }

        /// <summary>
        ///   Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        ///   A <see cref="string"/> representation of this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} - {1}", Start, End);
        }
    }
}