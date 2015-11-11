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
    ///   Allows you to define customized equality comparers using lambda functions.
    ///   (http://msdn.microsoft.com/en-us/library/bb397687.aspx)
    /// </summary>
    /// <typeparam name="T">The type to compare.</typeparam>
    /// <seealso cref="T:System.Collections.Generic.EqualityComparer`1"/>
    [PublicAPI]
    public class EqualityBuilder<T> : EqualityComparer<T>
    {
        /// <summary>
        ///   Stores the default GetHashCode function call <see cref="System.Object.GetHashCode"/>.
        /// </summary>
        [NotNull]
        // ReSharper disable PossibleNullReferenceException - Let it throw
        public static readonly Func<T, int> DefaultGetHashCodeFunction = o => o.GetHashCode();

        // ReSharper restore PossibleNullReferenceException

        /// <summary>
        ///   The function used to calculate equality between two <see cref="object"/>s.
        /// </summary>
        [NotNull]
        public readonly Func<T, T, bool> EqualsFunction;

        /// <summary>
        ///   The function used to generate a hash code.
        /// </summary>
        [NotNull]
        public readonly Func<T, int> GetHashCodeFunction;

        /// <summary>
        ///   Initializes a new instance of the <see cref="EqualityBuilder&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="equalityComparer">The equality comparer.</param>
        /// <param name="hashCodeGenerator">
        ///   <para>The hash code generator.</para>
        ///   <para>By default <see cref="System.Object.GetHashCode"/> will be used.</para>
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="equalityComparer"/> is <see langword="null" />.</exception>
        public EqualityBuilder([NotNull] Func<T, T, bool> equalityComparer, Func<T, int> hashCodeGenerator = null)
        {
            if (equalityComparer == null) throw new ArgumentNullException(nameof(equalityComparer));

            EqualsFunction = equalityComparer;
            GetHashCodeFunction = hashCodeGenerator ?? DefaultGetHashCodeFunction;
        }

        /// <inheritdoc/>
        // ReSharper disable once EventExceptionNotDocumented
        public override bool Equals(T x, T y) => EqualsFunction(x, y);

        /// <inheritdoc/>
        // ReSharper disable once EventExceptionNotDocumented
        public override int GetHashCode(T obj) => GetHashCodeFunction(obj);
    }


    /// <summary>
    /// Allows you to define customized equality comparers using lambda functions.
    /// (http://msdn.microsoft.com/en-us/library/bb397687.aspx)
    /// </summary>
    /// <typeparam name="T1">The type of the first operand.</typeparam>
    /// <typeparam name="T2">The type of the second operand.</typeparam>
    /// <seealso cref="T:System.IEqualityComparer" />
    [PublicAPI]
    public class EqualityBuilder<T1, T2> : IEqualityComparer
    {
        /// <summary>
        ///   Stores the default GetHashCode function call <see cref="System.Object.GetHashCode"/> for the <typeparamref name="T1"/> type.
        /// </summary>
        [NotNull]
        // ReSharper disable PossibleNullReferenceException - Let it throw
        public static readonly Func<T1, int> DefaultGetHashCodeFunction1 = o => o.GetHashCode();

        /// <summary>
        ///   Stores the default GetHashCode function call <see cref="System.Object.GetHashCode"/> for the <typeparamref name="T2"/> type.
        /// </summary>
        [NotNull]
        // ReSharper disable PossibleNullReferenceException - Let it throw
        public static readonly Func<T2, int> DefaultGetHashCodeFunction2 = o => o.GetHashCode();

        // ReSharper restore PossibleNullReferenceException

        /// <summary>
        ///   The function used to calculate equality between two <see cref="object"/>s.
        /// </summary>
        [NotNull]
        public readonly Func<T1, T2, bool> EqualsFunction;

        /// <summary>
        ///   The function used to generate a hash code for the <typeparamref name="T1"/> type.
        /// </summary>
        [NotNull]
        public readonly Func<T1, int> GetHashCodeFunction1;

        /// <summary>
        ///   The function used to generate a hash code for the <typeparamref name="T2"/> type.
        /// </summary>
        [NotNull]
        public readonly Func<T2, int> GetHashCodeFunction2;

        /// <summary>
        /// Initializes a new instance of the <see cref="EqualityBuilder&lt;T1,T2&gt;" /> class.
        /// </summary>
        /// <param name="equalityComparer">The equality comparer.</param>
        /// <param name="hashCodeGenerator1">The hash code generator for the <typeparamref name="T1" /> type.</param>
        /// <param name="hashCodeGenerator2">The hash code generator for the <typeparamref name="T2" /> type.</param>
        /// <exception cref="ArgumentNullException"><paramref name="equalityComparer" /> is <see langword="null" />.</exception>
        public EqualityBuilder(
            [NotNull] Func<T1, T2, bool> equalityComparer,
            Func<T1, int> hashCodeGenerator1 = null,
            Func<T2, int> hashCodeGenerator2 = null)
        {
            if (equalityComparer == null) throw new ArgumentNullException(nameof(equalityComparer));

            EqualsFunction = equalityComparer;
            GetHashCodeFunction1 = hashCodeGenerator1 ?? DefaultGetHashCodeFunction1;
            GetHashCodeFunction2 = hashCodeGenerator2 ?? DefaultGetHashCodeFunction2;
        }

        // ReSharper disable EventExceptionNotDocumented
        /// <inheritdoc />
        public new bool Equals(object x, object y) =>
            x is T1
                ? y is T2 && EqualsFunction((T1)x, (T2)y)
                : x is T2 && y is T1 && EqualsFunction((T1)y, (T2)x);

        /// <inheritdoc />
        public int GetHashCode(object obj)
        {
            if (obj is T1) return GetHashCodeFunction1((T1)obj);
            if (obj is T2) return GetHashCodeFunction2((T2)obj);
            return obj.GetHashCode();
        }
        // ReSharper restore EventExceptionNotDocumented
    }
}