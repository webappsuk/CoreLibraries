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
using System.Data;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.SqlServer.Server;
using Microsoft.SqlServer.Types;

namespace WebApplications.Utilities.Database.Schema
{
    /// <summary>
    ///   Holds information about a column.
    /// </summary>
    public class SqlColumn : DatabaseEntity<SqlColumn>
    {
        /// <summary>
        /// The properties used for calculating differences.
        /// </summary>
        [UsedImplicitly]
        [NotNull]
        private static readonly Expression<Func<SqlColumn, object>>[] _properties =
            new Expression<Func<SqlColumn, object>>[]
            {
                c => c.Type,
                c => c.Ordinal,
                c => c.IsNullable
            };

        /// <summary>
        ///   The column's zero-indexed ordinal.
        /// </summary>
        [PublicAPI]
        public readonly int Ordinal;

        /// <summary>
        ///   The column's type information.
        /// </summary>
        [PublicAPI]
        [NotNull]
        public readonly SqlType Type;

        /// <summary>
        ///   Gets the <see cref="Microsoft.SqlServer.Server.SqlMetaData"/>.
        /// </summary>
        /// <value>
        ///   The <see cref="Microsoft.SqlServer.Server.SqlMetaData"/> of the column.
        /// </value>
        [PublicAPI]
        [NotNull]
        public readonly SqlMetaData SqlMetaData;

        /// <summary>
        ///   Whether this column is nullable.
        /// </summary>
        [PublicAPI]
        public readonly bool IsNullable;

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
        internal SqlColumn(
            int ordinal,
            [NotNull] string name,
            [NotNull] SqlType type,
            SqlTypeSize size,
            bool isNullable)
            : base(name)
            // ReSharper restore PossibleNullReferenceException
        {
            Contract.Requires(name != null);
            Contract.Requires(type != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(name));
            Ordinal = ordinal;
            IsNullable = isNullable;
            Type = type.Size.Equals(size) ? type : new SqlType(type, size);

            switch (Type.SqlDbType)
            {
                case SqlDbType.Binary:
                case SqlDbType.Char:
                case SqlDbType.VarBinary:
                case SqlDbType.VarChar:
                    SqlMetaData = new SqlMetaData(name, Type.SqlDbType, Type.Size.MaximumLength);
                    break;
                case SqlDbType.NChar:
                case SqlDbType.NVarChar:
                    SqlMetaData = new SqlMetaData(
                        name,
                        Type.SqlDbType,
                        Type.Size.MaximumLength > 0 ? Type.Size.MaximumLength / 2 : Type.Size.MaximumLength);
                    break;
                case SqlDbType.Image:
                case SqlDbType.Text:
                case SqlDbType.NText:
                    SqlMetaData = new SqlMetaData(name, Type.SqlDbType);
                    break;
                case SqlDbType.Decimal:
                    SqlMetaData = new SqlMetaData(
                        name,
                        Type.SqlDbType,
                        Type.Size.Precision,
                        Type.Size.Scale);
                    break;
                case SqlDbType.Udt:
                    switch (Type.Name)
                    {
                        case "geography":
                            SqlMetaData = new SqlMetaData(
                                name,
                                Type.SqlDbType,
                                typeof (SqlGeography));
                            break;
                        case "geometry":
                            SqlMetaData = new SqlMetaData(
                                name,
                                Type.SqlDbType,
                                typeof (SqlGeometry));
                            break;
                        case "hierarchyid":
                            SqlMetaData = new SqlMetaData(
                                name,
                                Type.SqlDbType,
                                typeof (SqlHierarchyId));
                            break;
                        default:
                            SqlMetaData = new SqlMetaData(name, Type.SqlDbType);
                            break;
                    }
                    break;
                default:
                    SqlMetaData = new SqlMetaData(name, Type.SqlDbType);
                    break;
            }
        }

        /// <summary>
        ///   Casts the CLR value to the correct SQL type.
        /// </summary>
        /// <param name="value">The CLR value to cast.</param>
        /// <param name="mode">
        ///   <para>The constraint mode.</para>
        ///   <para>By default this is set to give a warning if truncation/loss of precision occurs.</para>
        /// </param>
        /// <returns>The result of the conversion.</returns>
        [PublicAPI]
        [CanBeNull]
        public object CastCLRValue<T>(T value, TypeConstraintMode mode = TypeConstraintMode.Warn)
        {
            // TODO What about SqlMetaData.Adjust()?
            return Type.CastCLRValue(value, mode);
        }
    }
}