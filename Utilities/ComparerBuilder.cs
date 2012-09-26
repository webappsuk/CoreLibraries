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