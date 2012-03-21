#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: Utilities.Database 
// Project: Utilities.Database
// File: SqlTypeSize.cs
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