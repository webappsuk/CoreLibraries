#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities
// File: EqualityBuilder.cs
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
using System.Diagnostics.Contracts;

namespace WebApplications.Utilities
{
    /// <summary>
    ///   Allows you to define customized equality comparers using lambda functions.
    ///   (http://msdn.microsoft.com/en-us/library/bb397687.aspx)
    /// </summary>
    /// <typeparam name="T">The type to compare.</typeparam>
    /// <seealso cref="T:System.Collections.Generic.EqualityComparer`1"/>
    public class EqualityBuilder<T> : EqualityComparer<T>
    {
        /// <summary>
        ///   Stores the default GetHashCode function call <see cref="System.Object.GetHashCode"/>.
        /// </summary>
        public static readonly Func<T, int> DefaultGetHashCodeFunction = o => o.GetHashCode();

        /// <summary>
        ///   The function used to calculate equality between two <see cref="object"/>s.
        /// </summary>
        public readonly Func<T, T, bool> EqualsFunction;

        /// <summary>
        ///   The function used to generate a hash code.
        /// </summary>
        public readonly Func<T, int> GetHashCodeFunction;

        /// <summary>
        ///   Initializes a new instance of the <see cref="EqualityBuilder&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="equalityComparer">The equality comparer.</param>
        /// <param name="hashCodeGenerator">
        ///   <para>The hash code generator.</para>
        ///   <para>By default <see cref="System.Object.GetHashCode"/> will be used.</para>
        /// </param>
        public EqualityBuilder(Func<T, T, bool> equalityComparer, Func<T, int> hashCodeGenerator = null)
        {
            Contract.Assert(equalityComparer != null);
            EqualsFunction = equalityComparer;
            GetHashCodeFunction = hashCodeGenerator ?? DefaultGetHashCodeFunction;
        }

        /// <summary>
        ///   Determines whether the two specified <see cref="object"/>s are equal.
        /// </summary>
        /// <param name="x">The first object of type <typeparamref name="T"/> to compare.</param>
        /// <param name="y">The second object of type <typeparamref name="T"/> to compare.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the specified objects are equal; otherwise returns <see langword="false"/>.
        /// </returns>
        public override bool Equals(T x, T y)
        {
            return EqualsFunction(x, y);
        }

        /// <summary>
        ///   Returns a hash code for the specified <see cref="object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to get a hash code for.</param>
        /// <returns>
        ///   A hash code for the specified <paramref name="obj"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///   <paramref name="obj"/> is a reference type and <paramref name="obj"/> is a <see langword="null"/>.
        /// </exception>
        public override int GetHashCode(T obj)
        {
            return GetHashCodeFunction(obj);
        }
    }
}