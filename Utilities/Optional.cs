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
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Holds an optional value.
    /// </summary>
    /// <typeparam name="T">The underlying type.</typeparam>
    /// <remarks>
    /// This is used for setting optional properties.
    /// </remarks>
    public struct Optional<T> : IOptional<T>,
        IEquatable<Optional<T>>,
        IEquatable<T>,
        IComparable<Optional<T>>,
        IComparable<T>
    {
        /// <summary>
        /// The delegate used for methods that return true when successful.
        /// </summary>
        /// <typeparam name="TIn">The type of the input.</typeparam>
        /// <typeparam name="TOut">The type of the T out.</typeparam>
        /// <param name="input">The input.</param>
        /// <param name="output">The output.</param>
        /// <returns>
        ///     <c>true</c> if successful, <c>false</c> otherwise
        /// </returns>
        public delegate bool TryDelegate<in TIn, TOut>(TIn input, out TOut output) where TOut : T;

        /// <summary>
        /// Returns the optional indicating a value is not assigned.
        /// </summary>
        public static readonly Optional<T> Unassigned = default(Optional<T>);

        /// <summary>
        /// Returns the optional indicating a value is assigned to it's default value.
        /// </summary>
        [UsedImplicitly]
        public static readonly Optional<T> DefaultAssigned = new Optional<T>(default(T), isAssigned: true);

        private readonly bool _isAssigned;

        private readonly T _value;

        /// <summary>
        /// Create a new Optional object with the specified value, indicating if it
        /// has been set
        /// </summary>
        /// <param name="value">The value to for the optional item.</param>
        /// <param name="isAssigned">
        /// Value to indicate if it has been set (defaults to <see langword="false" />.
        /// </param>
        private Optional(T value, bool isAssigned)
        {
            Contract.Requires(isAssigned || Equals(value, default(T)));
            _value = value;
            _isAssigned = isAssigned;
        }

        /// <summary>
        /// Create a new Optional object with the specified value, indicating that it
        /// has been set
        /// </summary>
        /// <param name="value">The value to for the optional item.</param>
        [UsedImplicitly]
        public Optional(T value)
            : this(value, true)
        {
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the
        /// following meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero
        /// This object is equal to <paramref name="other" />. Greater than zero This object is greater than
        ///     <paramref name="other" />.
        /// </returns>
        public int CompareTo(Optional<T> other)
        {
            return _isAssigned
                ? (other.IsAssigned ? Comparer<T>.Default.Compare(_value, other.Value) : 1)
                : (other.IsAssigned ? -1 : 0);
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the
        /// following meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero
        /// This object is equal to <paramref name="other" />. Greater than zero This object is greater than
        ///     <paramref name="other" />.
        /// </returns>
        public int CompareTo(T other)
        {
            if (!_isAssigned) return -1;
            return Comparer<T>.Default.Compare(_value, other);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(Optional<T> other)
        {
            return (_isAssigned.Equals(other.IsAssigned) &&
                    (!_isAssigned || EqualityComparer<T>.Default.Equals(_value, other.Value)));
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(T other)
        {
            return _isAssigned &&
                   (ReferenceEquals(other, null)
                       ? ReferenceEquals(_value, null)
                       : EqualityComparer<T>.Default.Equals(_value, other));
        }

        /// <summary>
        /// Whether the value is assigned.
        /// </summary>
        public bool IsAssigned
        {
            get { return _isAssigned; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is null.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if this instance is null; otherwise, <see langword="false" />.
        /// </value>
        public bool IsNull
        {
            get { return _isAssigned && _value.IsNull(); }
        }

        /// <summary>
        /// The value if assigned; otherwise <see langword="default{T}" />.
        /// </summary>
        public T Value
        {
            get { return _value; }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        object IOptional.Value
        {
            get { return _value; }
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>
        ///     <see langword="true" /> if the specified <see cref="System.Object" /> is equal to this instance; otherwise,
        ///     <see langword="false" />.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return (obj is Optional<T> && Equals((Optional<T>) obj)) ||
                   (obj is T && Equals((T) obj));
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return _isAssigned
                    ? EqualityComparer<T>.Default.GetHashCode(_value)
                    : int.MinValue;
            }
        }

        /// <summary>
        /// Creates an optional value from a value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static implicit operator Optional<T>(T value)
        {
            return new Optional<T>(value, true);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return _isAssigned
                ? (ReferenceEquals(_value, null)
                    ? "null"
                    : _value.ToString())
                : "Unassigned";
        }

        /// <summary>
        /// Retrieves the value from an optional value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks>
        ///     <para>
        /// This ignores the <see cref="_isAssigned" /> property and returns whatever
        /// value is in the underlying <see cref="_value" /> property.
        ///     </para>
        /// </remarks>
        public static explicit operator T(Optional<T> value)
        {
            return value._value;
        }

        /// <summary>
        /// Implements the ==.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Optional<T> a, Optional<T> b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Implements the !=.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Optional<T> a, Optional<T> b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        /// Returns an assigned value if not null, otherwise returns <see cref="Unassigned" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        [UsedImplicitly]
        public static Optional<T> AssignIfNotNull(T value)
        {
            return ReferenceEquals(value, null) ? Unassigned : new Optional<T>(value, true);
        }

        /// <summary>
        /// Returns an assigned value if not null, otherwise returns <see cref="Unassigned" />.
        /// </summary>
        /// <param name="valueFunction">The value.</param>
        /// <param name="unassignedOnNull">
        /// if set to <c>true</c> returns <see cref="Unassigned" /> if <paramref name="valueFunction" /> returns
        ///     <see langword="null" />.
        /// </param>
        /// <returns></returns>
        public static Optional<T> UnassignedOnError([NotNull] Func<T> valueFunction, bool unassignedOnNull = false)
        {
            Contract.Requires(valueFunction != null);
            try
            {
                T result = valueFunction();
                return unassignedOnNull && ReferenceEquals(result, null)
                    ? Unassigned
                    : new Optional<T>(result, true);
            }
            catch
            {
                return Unassigned;
            }
        }

        /// <summary>
        /// Returns the assigned value if the supplied try function returns true for the specified input.
        /// </summary>
        /// <typeparam name="TIn">The type of the input.</typeparam>
        /// <typeparam name="TOut">The type of the output.</typeparam>
        /// <param name="tryFunction">The try function.</param>
        /// <param name="input">The input.</param>
        /// <param name="unassignedOnNull">
        /// if set to <c>true</c> returns <see cref="Unassigned" /> if <paramref name="tryFunction" /> outputs a
        ///     <see langword="null" />,
        /// even when it returns <see langword="true" />.
        /// </param>
        /// <returns>Optional{`0}.</returns>
        public static Optional<T> UnassignedOnFailure<TIn, TOut>(
            [NotNull] TryDelegate<TIn, TOut> tryFunction,
            TIn input,
            bool unassignedOnNull = false)
            where TOut : T
        {
            Contract.Requires(tryFunction != null);
            TOut result;
            if (tryFunction(input, out result))
                return unassignedOnNull && ReferenceEquals(result, null)
                    ? Unassigned
                    : new Optional<T>(result, true);
            return Unassigned;
        }
    }
}