#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: Utilities.Database 
// Project: Utilities.Database
// File: SqlColumn.cs
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
using System.Diagnostics.Contracts;
using JetBrains.Annotations;
using Microsoft.SqlServer.Server;
using Microsoft.SqlServer.Types;

namespace WebApplications.Utilities.Database.Schema
{
    /// <summary>
    ///   Holds information about a column.
    /// </summary>
    public class SqlColumn : IEquatable<SqlColumn>, IEqualityComparer<SqlColumn>
    {
        /// <summary>
        ///   Whether this column is nullable.
        /// </summary>
        public readonly bool IsNullable;

        /// <summary>
        ///   The column's name.
        /// </summary>
        [NotNull]
        public readonly string Name;

        /// <summary>
        ///   The column's zero-indexed ordinal.
        /// </summary>
        public readonly int Ordinal;

        /// <summary>
        ///   The column's type information.
        /// </summary>
        [NotNull]
        public readonly SqlType Type;

        /// <summary>
        ///   The hash code.
        /// </summary>
        private int? _hashCode;

        /// <summary>
        ///   The column's meta data.
        /// </summary>
        private SqlMetaData _sqlMetaData;

        /// <summary>
        ///   Initializes a new instance of the <see cref="SqlColumn"/> class.
        /// </summary>
        /// <param name="ordinal">The zero-based ordinal of the column.</param>
        /// <param name="name">The column name.</param>
        /// <param name="type">The type of the column's data.</param>
        /// <param name="size">The size information.</param>
        /// <param name="isNullable">
        ///   If set to <see langword="true"/> then the column is nullable.
        /// </param>
        /// <remarks>
        ///   <para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> specifying that
        ///   <paramref name="type"/> cannot be <see langword="null"/>.</para>
        ///   <para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> specifying that
        ///   <paramref name="type"/> cannot be <see cref="string.IsNullOrWhiteSpace"/>.</para>
        /// </remarks>
        internal SqlColumn(int ordinal, [NotNull]string name, [NotNull]SqlType type, SqlTypeSize size, bool isNullable)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(name));
            Contract.Requires(type != null);
            Ordinal = ordinal;
            IsNullable = isNullable;
            Type = type.Size.Equals(size) ? type : new SqlType(type, size);
            Name = name;
        }

        /// <summary>
        ///   Gets the <see cref="SqlMetaData"/>.
        /// </summary>
        /// <value>
        ///   The <see cref="SqlMetaData"/> of the column.
        /// </value>
        [NotNull]
        public SqlMetaData SqlMetaData
        {
            get
            {
                if (_sqlMetaData == null)
                {
                    switch (Type.SqlDbType)
                    {
                        case SqlDbType.Binary:
                        case SqlDbType.Char:
                        case SqlDbType.Image:
                        case SqlDbType.NChar:
                        case SqlDbType.NText:
                        case SqlDbType.NVarChar:
                        case SqlDbType.Text:
                        case SqlDbType.VarBinary:
                        case SqlDbType.VarChar:
                            _sqlMetaData = new SqlMetaData(Name, Type.SqlDbType, Type.Size.MaximumLength);
                            break;
                        case SqlDbType.Decimal:
                            _sqlMetaData = new SqlMetaData(
                                Name, Type.SqlDbType, Type.Size.Precision, Type.Size.Scale);
                            break;
                        case SqlDbType.Udt:
                            switch (Type.Name)
                            {
                                case "geography":
                                    _sqlMetaData = new SqlMetaData(
                                        Name, Type.SqlDbType, typeof (SqlGeography));
                                    break;
                                case "geometry":
                                    _sqlMetaData = new SqlMetaData(
                                        Name, Type.SqlDbType, typeof (SqlGeometry));
                                    break;
                                case "hierarchyid":
                                    _sqlMetaData = new SqlMetaData(
                                        Name, Type.SqlDbType, typeof (SqlHierarchyId));
                                    break;
                                default:
                                    _sqlMetaData = new SqlMetaData(Name, Type.SqlDbType);
                                    break;
                            }
                            break;
                        default:
                            _sqlMetaData = new SqlMetaData(Name, Type.SqlDbType);
                            break;
                    }
                }
                return _sqlMetaData;
            }
        }

        #region IEqualityComparer<SqlColumn> Members
        /// <summary>
        ///   Determines whether the specified objects are equal.
        /// </summary>
        /// <returns>
        ///   Returns <see langword="true"/> if the specified objects are equal; otherwise returns <see langword="false"/>.
        /// </returns>
        /// <param name="x">The first <see cref="SqlColumn"/> to compare.</param>
        /// <param name="y">The second <see cref="SqlColumn"/> to compare.</param>
        public bool Equals(SqlColumn x, SqlColumn y)
        {
            return x == null
                       ? y == null
                       : y != null && x.Equals(y);
        }

        /// <summary>
        ///   Returns a hash code for the specified column.
        /// </summary>
        /// <returns>
        ///   A hash code for the specified column.
        /// </returns>
        /// <param name="obj">The <see cref="object"/> for which a hash code is to be returned.</param>
        /// <exception cref="ArgumentNullException">
        ///   The type of <paramref name="obj"/> is a reference type and <paramref name="obj" /> is <see langword="null"/>.
        /// </exception>
        public int GetHashCode([NotNull]SqlColumn obj)
        {
            if (obj._hashCode == null)
            {
                obj._hashCode = Ordinal.GetHashCode() ^ Name.GetHashCode() ^ Type.GetHashCode() ^
                                Type.Size.GetHashCode() ^ IsNullable.GetHashCode();
            }
            return (int) obj._hashCode;
        }
        #endregion

        #region IEquatable<SqlColumn> Members
        /// <summary>
        ///   Indicates whether the current column is equal to another column specified.
        /// </summary>
        /// <returns>
        ///   Returns <see langword="true"/> if the current column is equal to the <paramref name="other" />; otherwise returns <see langword="false"/>.
        /// </returns>
        /// <param name="other">A <see cref="SqlColumn"/> to compare with this instance.</param>
        public bool Equals(SqlColumn other)
        {
            return (other != null) &&
                   (Ordinal == other.Ordinal) && (Name == other.Name) && (Type.Equals(other.Type)) &&
                   (IsNullable == other.IsNullable) && (Type.Size.Equals(other.Type.Size));
        }
        #endregion

        /// <summary>
        ///   Casts the CLR value to the correct SQL type.
        /// </summary>
        /// <param name="value">The CLR value to cast.</param>
        /// <param name="mode">
        ///   <para>The constraint mode.</para>
        ///   <para>By default this is set to give a warning if truncation/loss of precision occurs.</para>
        /// </param>
        /// <returns>The result of the conversion.</returns>
        [CanBeNull]
        public object CastCLRValue<T>(T value, TypeConstraintMode mode = TypeConstraintMode.Warn)
        {
            return Type.CastCLRValue(value, mode);
        }

        /// <summary>
        ///   Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        ///   <para>A <see cref="string"/> representation of this instance.</para>
        ///   <para><b>Format:</b> <see cref="SqlColumn.Name"/> + "Column".</para>
        /// </returns>
        public override string ToString()
        {
            return Name + " Column";
        }
    }
}