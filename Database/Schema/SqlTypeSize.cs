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

namespace WebApplications.Utilities.Database.Schema
{
    /// <summary>
    ///   A structure that holds size information for a <see cref="SqlType"/>
    /// </summary>
    public struct SqlTypeSize : IEquatable<SqlTypeSize>, IEqualityComparer<SqlTypeSize>
    {
        /// <summary>
        ///   The maximum length.
        /// </summary>
        public readonly short MaximumLength;

        /// <summary>
        ///   The precision.
        /// </summary>
        public readonly byte Precision;

        /// <summary>
        ///   The scale.
        /// </summary>
        public readonly byte Scale;

        private int? _hashCode;

        /// <summary>
        ///   Initializes a new instance of the <see cref="SqlTypeSize" /> struct.
        /// </summary>
        /// <param name="maximumLength">The maximum length.</param>
        /// <param name="precision">The number of digits in a numerical value.</param>
        /// <param name="scale">
        ///   The number of digits to the right of the decimal point in a number.
        /// </param>
        public SqlTypeSize(short maximumLength, byte precision, byte scale)
            : this()
        {
            MaximumLength = maximumLength;
            Scale = scale;
            Precision = precision;
        }

        #region IEqualityComparer<SqlTypeSize> Members
        /// <summary>
        ///   Checks if two <see cref="SqlTypeSize">sizes</see> are equal.
        /// </summary>
        /// <param name="x">The first <see cref="SqlTypeSize"/> to compare.</param>
        /// <param name="y">The second <see cref="SqlTypeSize"/> to compare.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the two <see cref="SqlTypeSize"/>s are equal; otherwise returns <see langword="false"/>.
        /// </returns>
        public bool Equals(SqlTypeSize x, SqlTypeSize y)
        {
            return x.Equals(y);
        }

        /// <summary>
        ///   Returns a hash code for the specified object.
        /// </summary>
        /// <returns>
        ///   A hash code for the specified object.
        /// </returns>
        /// <param name="obj">The <see cref="object"/> for which a hash code is to be returned.</param>
        /// <exception cref="ArgumentNullException">
        ///   The type of <paramref name="obj"/> is a reference type and is <see langword="null"/>.
        /// </exception>
        public int GetHashCode(SqlTypeSize obj)
        {
            if (obj._hashCode == null)
                obj._hashCode = obj.MaximumLength.GetHashCode() ^ obj.Precision.GetHashCode() ^ obj.Scale.GetHashCode();
            return (int) obj._hashCode;
        }
        #endregion

        #region IEquatable<SqlTypeSize> Members
        /// <summary>
        ///   Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        ///   Returns <see langword="true"/> if the current object is equal to the <paramref name="other"/>; otherwise returns <see langword="false"/>.
        /// </returns>
        /// <param name="other">An <see cref="SqlTypeSize"/> to compare with this object.</param>
        public bool Equals(SqlTypeSize other)
        {
            return (other.MaximumLength == MaximumLength) && (other.Precision == Precision) &&
                   (other.Scale == Scale);
        }
        #endregion

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
            return string.Format(Resources.SqlTypeSize_ToString, MaximumLength, Precision, Scale);
        }
    }
}