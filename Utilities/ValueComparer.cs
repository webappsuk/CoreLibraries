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

using System.Collections.Generic;
using JetBrains.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Equality comparer for <see cref="KeyValuePair{TKey,TValue}"/> that only considers values.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class ValueComparer<TKey, TValue> : IEqualityComparer<KeyValuePair<TKey, TValue>>
    {
        /// <summary>
        /// The default <see cref="KeyValuePair{TKey, TValue}"/>.
        /// </summary>
        [NotNull]
        public static readonly ValueComparer<TKey, TValue> Default = new ValueComparer<TKey, TValue>();

        [NotNull]
        private readonly IEqualityComparer<TValue> _comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueComparer{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="comparer">The value comparer.</param>
        public ValueComparer([CanBeNull] IEqualityComparer<TValue> comparer = null)
        {
            _comparer = comparer ?? EqualityComparer<TValue>.Default;
        }

        /// <summary>
        /// Equalses the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>System.Boolean.</returns>
        public bool Equals(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
        {
            return _comparer.Equals(x.Value, y.Value);
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>System.Int32.</returns>
        public int GetHashCode(KeyValuePair<TKey, TValue> obj)
        {
            return ReferenceEquals(obj.Value, null)
                ? int.MinValue
                : _comparer.GetHashCode(obj.Value);
        }
    }
}