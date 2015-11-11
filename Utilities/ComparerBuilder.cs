#region © Copyright Web Applications (UK) Ltd, 2015.  All rights reserved.
// Copyright (c) 2015, Web Applications UK Ltd
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
using System.Collections;
using System.Collections.Generic;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    ///   Allows you to define generic comparers using lambda functions.
    ///   (http://msdn.microsoft.com/en-us/library/bb397687.aspx)
    /// </summary>
    /// <typeparam name="T">The type to compare.</typeparam>
    /// <seealso cref="T:System.Collections.Generic.IComparer`1"/>
    [PublicAPI]
    public class ComparerBuilder<T> : EqualityBuilder<T>, IComparer<T>
    {
        /// <summary>
        ///   The function used to provide comparisons.
        /// </summary>
        [NotNull]
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
        /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is <see langword="null" />.</exception>
        public ComparerBuilder(
            [NotNull] Func<T, T, int> comparer,
            [CanBeNull] Func<T, T, bool> equalityComparer = null,
            [CanBeNull] Func<T, int> hashCodeGenerator = null)
            // ReSharper disable once EventExceptionNotDocumented
            : base(equalityComparer ?? ((a, b) => comparer(a, b) == 0), hashCodeGenerator)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            CompareFunction = comparer;
        }

        /// <inheritdoc/>
        // ReSharper disable once EventExceptionNotDocumented
        public int Compare(T x, T y) => CompareFunction(x, y);
    }

    /// <summary>
    /// Allows you to define generic comparers using lambda functions.
    /// (http://msdn.microsoft.com/en-us/library/bb397687.aspx)
    /// </summary>
    /// <typeparam name="T1">The type of the first operand.</typeparam>
    /// <typeparam name="T2">The type of the second operand.</typeparam>
    /// <seealso cref="T:System.Collections.IComparer" />
    public class ComparerBuilder<T1, T2> : EqualityBuilder<T1, T2>, IComparer
    {
        /// <summary>
        ///   The function used to provide comparisons.
        /// </summary>
        [NotNull]
        public readonly Func<T1, T2, int> CompareFunction;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComparerBuilder{T1, T2}"/> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        /// <param name="hashCodeGenerator1">The hash code generator1.</param>
        /// <param name="hashCodeGenerator2">The hash code generator2.</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is <see langword="null" />.</exception>
        public ComparerBuilder(
            [NotNull] Func<T1, T2, int> comparer,
            [CanBeNull] Func<T1, T2, bool> equalityComparer = null,
            [CanBeNull] Func<T1, int> hashCodeGenerator1 = null,
            [CanBeNull] Func<T2, int> hashCodeGenerator2 = null)
            // ReSharper disable once EventExceptionNotDocumented
            : base(equalityComparer ?? ((a, b) => comparer(a, b) == 0), hashCodeGenerator1, hashCodeGenerator2)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            CompareFunction = comparer;
        }

        /// <inheritdoc/>
        /// <exception cref="InvalidOperationException">The objects cannot be compared.</exception>
        public int Compare(object x, object y)
        {
            // ReSharper disable EventExceptionNotDocumented
            if (x is T1 && y is T2)
                return CompareFunction((T1)x, (T2)y);
            if (y is T1 && x is T2)
                return -CompareFunction((T1)y, (T2)x);
            // ReSharper restore EventExceptionNotDocumented
            IComparable c = x as IComparable;
            if (c != null)
                return c.CompareTo(y);
            c = y as IComparable;
            if (c != null)
                return c.CompareTo(x);
            throw new InvalidOperationException(Resources.ComparerBuilder_Compare_Incomparable);
        }
    }
}