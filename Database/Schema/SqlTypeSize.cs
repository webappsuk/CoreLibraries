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
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Database.Schema
{
    /// <summary>
    ///   A structure that holds size information for a <see cref="SqlType"/>
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct SqlTypeSize : IEquatable<SqlTypeSize>
    {
        /// <summary>
        ///   The maximum length.
        /// </summary>
        [FieldOffset(0)]
        public readonly short MaximumLength;

        /// <summary>
        ///   The precision.
        /// </summary>
        [FieldOffset(2)]
        public readonly byte Precision;

        /// <summary>
        ///   The scale.
        /// </summary>
        [FieldOffset(3)]
        public readonly byte Scale;

        [FieldOffset(0)]
        private readonly int _hashCode;

        /// <summary>
        ///   Initializes a new instance of the <see cref="SqlTypeSize" /> struct.
        /// </summary>
        /// <param name="maximumLength">The maximum length.</param>
        /// <param name="precision">The number of digits in a numerical value.</param>
        /// <param name="scale">
        ///   The number of digits to the right of the decimal point in a number.
        /// </param>
        public SqlTypeSize(short maximumLength, byte precision, byte scale)
        {
            _hashCode = 0;
            MaximumLength = maximumLength;
            Scale = scale;
            Precision = precision;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns><see langword="true" /> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <see langword="false" />.</returns>
        public override bool Equals([CanBeNull]object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is SqlTypeSize &&
                   _hashCode == ((SqlTypeSize)obj)._hashCode;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(SqlTypeSize other)
        {
            return _hashCode == other._hashCode;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return _hashCode;
        }

        /// <summary>
        /// Implements the ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(SqlTypeSize left, SqlTypeSize right)
        {
            return left._hashCode.Equals(right._hashCode);
        }

        /// <summary>
        /// Implements the !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(SqlTypeSize left, SqlTypeSize right)
        {
            return !left._hashCode.Equals(right._hashCode);
        }

        /// <summary>
        ///   Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        ///   A <see cref="string"/> representation of this instance. The format string can be changed in the 
        ///   Resources.resx resource file at the key 'SqlTypeSizeToString'.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///   The format string was a <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.FormatException">
        ///   An index from the format string is either less than zero or greater than or equal to the number of arguments.
        /// </exception>
        public override string ToString()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            return string.Format(Resources.SqlTypeSize_ToString, MaximumLength, Precision, Scale);
        }
    }
}