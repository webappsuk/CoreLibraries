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

namespace WebApplications.Utilities.Database.Schema
{
    /// <summary>
    /// Holds a single difference to a <see cref="DatabaseEntity"/>.
    /// </summary>
    public abstract class Difference
    {
        /// <summary>
        /// The name.
        /// </summary>
        [PublicAPI]
        [NotNull]
        public readonly string Name;

        /// <summary>
        /// The value type.
        /// </summary>
        [PublicAPI]
        [NotNull]
        public readonly Type ValueType;

        /// <summary>
        /// The left value.
        /// </summary>
        [PublicAPI]
        [CanBeNull]
        public readonly object LeftValue;

        /// <summary>
        /// The right value.
        /// </summary>
        [PublicAPI]
        [CanBeNull]
        public readonly object RightValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="Difference"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="valueType">Type of the value.</param>
        /// <param name="leftValue">The left value.</param>
        /// <param name="rightValue">The right value.</param>
        protected Difference(
            [NotNull] string name,
            [NotNull] Type valueType,
            [CanBeNull] object leftValue,
            [CanBeNull] object rightValue)
        {
            Contract.Requires(name != null);
            Contract.Requires(valueType != null);
            Name = name;
            ValueType = valueType;
            LeftValue = leftValue;
            RightValue = rightValue;
        }
    }

    /// <summary>
    /// Holds a single difference to a <see cref="DatabaseEntity"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    public class Difference<T> : Difference
    {
        /// <summary>
        /// The left value.
        /// </summary>
        [PublicAPI]
        [CanBeNull]
        public new T LeftValue
        {
            get { return (T) base.LeftValue; }
        }

        /// <summary>
        /// The right value.
        /// </summary>
        [PublicAPI]
        [CanBeNull]
        public new T RightValue
        {
            get { return (T) base.RightValue; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Difference{T}"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="leftValue">The left value.</param>
        /// <param name="rightValue">The right value.</param>
        internal Difference([NotNull] string name, [CanBeNull] T leftValue, [CanBeNull] T rightValue)
            : base(name, typeof (T), leftValue, rightValue)
        {
            Contract.Requires(name != null);
        }
    }
}