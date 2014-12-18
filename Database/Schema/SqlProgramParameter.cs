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
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Database.Schema
{
    /// <summary>
    ///   A class for holding information about a program parameter.
    /// </summary>
    public class SqlProgramParameter : DatabaseEntity<SqlProgramParameter>
    {
        /// <summary>
        /// The properties used for calculating differences.
        /// </summary>
        [UsedImplicitly]
        [NotNull]
        private static readonly Expression<Func<SqlProgramParameter, object>>[] _properties =
            new Expression<Func<SqlProgramParameter, object>>[]
            {
                p => p.Ordinal,
                p => p.Type,
                p => p.Direction,
                p => p.IsReadOnly
            };

        /// <summary>
        ///   The <see cref="ParameterDirection"/>.
        /// </summary>
        [PublicAPI]
        public readonly ParameterDirection Direction;

        /// <summary>
        ///   A <see cref="bool"/> value indicating whether the parameter is read only.
        /// </summary>
        [PublicAPI]
        public readonly bool IsReadOnly;

        /// <summary>
        ///   The zero-based index that is the parameter's position.
        /// </summary>
        [PublicAPI]
        public readonly int Ordinal;

        /// <summary>
        ///   The parameter type information.
        /// </summary>
        [PublicAPI]
        [NotNull]
        public readonly SqlType Type;

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
        /// <param name="isReadOnly">
        ///   If set to <see langword="true"/> the parameter is <see cref="IsReadOnly">read-only.</see>
        /// </param>
        /// <remarks>
        ///   There is a <see cref="System.Diagnostics.Contracts.Contract">contact</see> specifying that
        ///   <paramref name="name"/> and <paramref name="type"/> cannot be <see langword="null"/>.
        /// </remarks>
        internal SqlProgramParameter(
            int ordinal,
            [NotNull] string name,
            [NotNull] SqlType type,
            SqlTypeSize size,
            ParameterDirection direction,
            bool isReadOnly)
            : base(name)
        {
            Contract.Requires(name != null);
            Contract.Requires(type != null);
            Ordinal = ordinal;
            IsReadOnly = isReadOnly;
            Direction = direction;
            Type = type.Size.Equals(size) ? type : new SqlType(type, size);
        }

        /// <summary>
        ///   Creates a <see cref="SqlParameter"/> using the current <see cref="SqlProgramParameter"/> instance.
        /// </summary>
        /// <returns>
        ///   The created <see cref="SqlParameter"/> object.
        /// </returns>
        [NotNull]
        public SqlParameter CreateSqlParameter()
        {
            SqlParameter parameter = new SqlParameter(FullName, Type.SqlDbType, Type.Size.MaximumLength);
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
        public object CastCLRValue<T>([CanBeNull] T value, TypeConstraintMode mode = TypeConstraintMode.Warn)
        {
            return Type.CastCLRValue(value, mode);
        }
    }
}