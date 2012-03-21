#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities
// File: ShortRange.cs
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
    ///   A range of <see cref="short"/>s.
    /// </summary>
    public class ShortRange : Range<short>
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="ShortRange"/> class.
        /// </summary>
        /// <param name="start">The start number.</param>
        /// <param name="end">The end number.</param>
        /// <param name="step">
        ///   <para>The step between each value in the range.</para>
        ///   <para>By default this is set to 1.</para>
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///   The <paramref name="start"/> value was greater than the <paramref name="end"/> value.
        /// </exception>
        public ShortRange(short start, short end, short step = (short) 1)
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