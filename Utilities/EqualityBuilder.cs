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
        public EqualityBuilder([NotNull] Func<T, T, bool> equalityComparer, Func<T, int> hashCodeGenerator = null)
        {
            if (equalityComparer == null) throw new ArgumentNullException("equalityComparer");

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