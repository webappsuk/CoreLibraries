#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities
// File: ComparerBuilder.cs
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

using System;
using System.Collections.Generic;

namespace WebApplications.Utilities
{
    /// <summary>
    ///   Allows you to define generic comparers using lambda functions.
    ///   (http://msdn.microsoft.com/en-us/library/bb397687.aspx)
    /// </summary>
    /// <typeparam name="T">The type to compare.</typeparam>
    /// <seealso cref="T:System.Collections.Generic.IComparer`1"/>
    public class ComparerBuilder<T> : EqualityBuilder<T>, IComparer<T>
    {
        /// <summary>
        ///   The function used to provide comparisons.
        /// </summary>
        public readonly Func<T, T, int> CompareFunction;

        /// <summary>
        ///   Initializes a new instance of the <see cref="ComparerBuilder&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        /// <param name="equalityComparer">
        ///   <para>The equality comparer.</para>
        ///   <para>By default it will use the <paramref name="comparer"/> and check to see if the result is 0.</para>
        /// </param>
        /// <param name="hashCodeGenerator">
        ///   <para>The hash code generator.</para>
        ///   <para>By default <see cref="System.Object.GetHashCode"/> will be used.</para>
        /// </param>
        public ComparerBuilder(
            Func<T, T, int> comparer,
            Func<T, T, bool> equalityComparer = null,
            Func<T, int> hashCodeGenerator = null)
            : base(equalityComparer ?? ((a, b) => comparer(a, b) == 0), hashCodeGenerator)
        {
            CompareFunction = comparer;
        }

        #region IComparer<T> Members
        /// <summary>
        ///   Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object of type <typeparamref name="T"/> to compare.</param>
        /// <param name="y">The second object of type <typeparamref name="T"/> to compare.</param>
        /// <returns>
        ///   <para>A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>:</para>
        ///   <list type="bullet">
        ///     <item><description>Value less than zero: <paramref name="x"/> is less than <paramref name="y"/>.</description></item>
        ///     <item><description>Zero: <paramref name="x"/> equals <paramref name="y"/>.</description></item>
        ///     <item><description>Value greater than zero: <paramref name="x"/> is greater than <paramref name="y"/>.</description></item>
        ///   </list>
        /// </returns>
        public int Compare(T x, T y)
        {
            return CompareFunction(x, y);
        }
        #endregion
    }
}