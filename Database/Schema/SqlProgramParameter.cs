#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: Utilities.Database 
// Project: Utilities.Database
// File: SqlProgramParameter.cs
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
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Database.Schema
{
    /// <summary>
    ///   A class for holding information about a program parameter.
    /// </summary>
    public class SqlProgramParameter : IEquatable<SqlProgramParameter>, IEqualityComparer<SqlProgramParameter>
    {
        /// <summary>
        ///   The <see cref="ParameterDirection"/>.
        /// </summary>
        [UsedImplicitly]
        public readonly ParameterDirection Direction;

        /// <summary>
        ///   A <see cref="bool"/> value indicating whether the parameter is read only.
        /// </summary>
        [UsedImplicitly]
        public readonly bool IsReadonly;

        /// <summary>
        ///   The parameter name (including the obligatory '@').
        /// </summary>
        [NotNull]
        public readonly string Name;

        /// <summary>
        ///   The zero-based index that is the parameter's position.
        /// </summary>
        public readonly int Ordinal;

        /// <summary>
        ///   The parameter type information.
        /// </summary>
        [NotNull]
        public readonly SqlType Type;

        /// <summary>
        ///   Hash code cache.
        /// </summary>
        private int? _hashCode;

        /// <summary>
        ///   Initializes a new instance of the <see cref="SqlProgramParameter"/> class.
        /// </summary>
        /// <param name="ordinal">
        ///   The zero-based index that is the parameter position.
        /// </param>
        /// <param name="name">
        ///   <para>The <see cref="SqlProgramParameter.Name">parameter name</see>.</para>
        ///   <para>The name should include the obligatory '@'.</para>
        /// </param>
        /// <param name="type">The type information.</param>
        /// <param name="size">The size information.</param>
        /// <param name="direction">The parameter direction.</param>
        /// <param name="isReadonly">
        ///   If set to <see langword="true"/> the parameter is <see cref="SqlProgramParameter.IsReadonly">read-only.</see>
        /// </param>
        /// <remarks>
        ///   There is a <see cref="System.Diagnostics.Contracts.Contract">contact</see> specifying that
        ///   <paramref name="name"/> and <paramref name="type"/> cannot be <see langword="null"/>.
        /// </remarks>
        internal SqlProgramParameter(int ordinal, [NotNull]string name, [NotNull]SqlType type, SqlTypeSize size,
                                     ParameterDirection direction, bool isReadonly)
        {
            Contract.Requires(name != null);
            Contract.Requires(type != null);
            Ordinal = ordinal;
            Name = name;
            IsReadonly = isReadonly;
            Direction = direction;
            Type = type.Size.Equals(size) ? type : new SqlType(type, size);
        }

        #region IEqualityComparer<SqlProgramParameter> Members
        /// <summary>
        ///   Determines whether the specified objects are equal.
        /// </summary>
        /// <returns>
        ///   Returns <see langword="true"/> if the specified <see cref="SqlProgramParameter"/>s are equal;
        ///   otherwise returns <see langword="false"/>.
        /// </returns>
        /// <param name = "x">The first <see cref="SqlProgramParameter"/> to compare.</param>
        /// <param name = "y">The second <see cref="SqlProgramParameter"/> to compare.</param>
        public bool Equals([CanBeNull]SqlProgramParameter x, [CanBeNull]SqlProgramParameter y)
        {
            return x == null
                       ? y == null
                       : y != null && x.Equals(y);
        }

        /// <summary>
        ///   Returns a hash code for the specified object.
        /// </summary>
        /// <returns>
        ///   A hash code for the specified object.
        /// </returns>
        /// <param name="obj">The <see cref="object" /> for which a hash code is to be returned.</param>
        /// <exception cref="ArgumentNullException">
        ///   The type of <paramref name = "obj" /> is a reference type and is <see langword="null"/>.
        /// </exception>
        public int GetHashCode([NotNull]SqlProgramParameter obj)
        {
            if (obj._hashCode == null)
            {
                obj._hashCode = obj.Name.GetHashCode() ^ obj.IsReadonly.GetHashCode() ^ obj.Direction.GetHashCode() ^
                                obj.Type.Size.GetHashCode() ^ obj.Type.GetHashCode();
            }
            return (int) obj._hashCode;
        }
        #endregion

        #region IEquatable<SqlProgramParameter> Members
        /// <summary>
        ///   Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        ///   true if the current object is equal to the <paramref name = "other" /> parameter; otherwise, false.
        /// </returns>
        /// <param name = "other">An object to compare with this object.</param>
        public bool Equals([CanBeNull]SqlProgramParameter other)
        {
            return (other != null) &&
                   (Name == other.Name) && (Type.Equals(other.Type)) && (Type.Size.Equals(other.Type.Size)) &&
                   (Direction == other.Direction) && (IsReadonly == other.IsReadonly);
        }
        #endregion

        /// <summary>
        ///   Creates a <see cref="SqlParameter"/> using the current <see cref="SqlProgramParameter"/> instance.
        /// </summary>
        /// <returns>
        ///   The created <see cref="SqlParameter"/> object.
        /// </returns>
        [NotNull]
        public SqlParameter CreateSqlParameter()
        {
            SqlParameter parameter = new SqlParameter(Name, Type.SqlDbType, Type.Size.MaximumLength)
                                         {Size = Type.Size.MaximumLength};
            if (Type.Size.Precision != 0)
                parameter.Precision = Type.Size.Precision;
            if (Type.Size.Scale != 0)
                parameter.Scale = Type.Size.Scale;
            if (Type.SqlDbType == SqlDbType.Udt)
                parameter.UdtTypeName = Type.Name;
            return parameter;
        }

        /// <summary>
        ///   Casts the CLR value to the correct SQL type.
        /// </summary>
        /// <typeparam name="T">The CLR type of the value to cast.</typeparam>
        /// <param name="value">The CLR value to cast.</param>
        /// <param name="mode">
        ///   <para>The constraint mode.</para>
        ///   <para>By default this is set to give a warning if truncation/loss of precision occurs.</para>
        /// </param>
        /// <returns>
        ///   The result (if possible); otherwise returns the <paramref name="value"/> passed in.
        /// </returns>
        [CanBeNull]
        public object CastCLRValue<T>([CanBeNull]T value, TypeConstraintMode mode = TypeConstraintMode.Warn)
        {
            return Type.CastCLRValue(value, mode);
        }

        /// <summary>
        ///   Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        ///   <para>A <see cref="string"/> representation of this instance.</para>
        ///   <para><b>Format:</b> <see cref="SqlProgramParameter.Name"/> + "Program Parameter".</para>
        /// </returns>
        public override string ToString()
        {
            return Name + " Program Parameter";
        }
    }
}