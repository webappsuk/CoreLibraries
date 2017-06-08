#region © Copyright Web Applications (UK) Ltd, 2015.  All rights reserved.
// Copyright (c) 2015, Web Applications UK Ltd
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
using System.Diagnostics;
using System.Linq.Expressions;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Database.Exceptions;
using WebApplications.Utilities.Logging;

namespace WebApplications.Utilities.Database.Schema
{
    /// <summary>
    ///   A class for holding information about a program parameter.
    /// </summary>
    [PublicAPI]
    public class SqlProgramParameter : DatabaseEntity<SqlProgramParameter>
    {
        /// <summary>
        /// The properties used for calculating differences.
        /// </summary>
        [UsedImplicitly]
        [NotNull]
        private static readonly Expression<Func<SqlProgramParameter, object>>[] _properties =
        {
            p => p.Ordinal,
            p => p.Type,
            p => p.Direction,
            p => p.IsReadOnly
        };

        /// <summary>
        ///   The <see cref="ParameterDirection"/>.
        /// </summary>
        public readonly ParameterDirection Direction;

        /// <summary>
        ///   A <see cref="bool"/> value indicating whether the parameter is read only.
        /// </summary>
        public readonly bool IsReadOnly;

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
        ///    Whether the size of the type of the parameter is exactly known.
        /// </summary>
        internal readonly bool ExactSize;

        /// <summary>
        ///   Initializes a new instance of the <see cref="SqlProgramParameter"/> class.
        /// </summary>
        /// <param name="ordinal">
        ///   The zero-based index that is the parameter position.
        /// </param>
        /// <param name="name">
        ///   <para>The <see cref="DatabaseEntity.FullName">parameter name</see>.</para>
        ///   <para>The name should include the obligatory '@'.</para>
        /// </param>
        /// <param name="type">The type information.</param>
        /// <param name="size">The size information.</param>
        /// <param name="direction">The parameter direction.</param>
        /// <param name="isReadOnly">
        ///   If set to <see langword="true"/> the parameter is <see cref="IsReadOnly">read-only.</see>
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> or <paramref name="type"/> is <see langword="null" />.</exception>
        internal SqlProgramParameter(
            int ordinal,
            [NotNull] string name,
            [NotNull] SqlType type,
            SqlTypeSize? size,
            ParameterDirection direction,
            bool isReadOnly)
            : base(name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (type == null) throw new ArgumentNullException(nameof(type));
            Ordinal = ordinal;
            IsReadOnly = isReadOnly;
            Direction = direction;
            ExactSize = size.HasValue;
            Type = !size.HasValue || type.Size.Equals(size.Value)
                ? type
                : new SqlType(type, size.Value);
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
            SqlParameter parameter = new SqlParameter(FullName, Type.SqlDbType);

            if (ExactSize)
            {
                parameter.Size = Type.Size.MaximumLength;
                if (Type.Size.Precision != 0)
                    parameter.Precision = Type.Size.Precision;
                if (Type.Size.Scale != 0)
                    parameter.Scale = Type.Size.Scale;
            }

            if (Type.SqlDbType == SqlDbType.Udt)
                parameter.UdtTypeName = Type.Name;
            else if (Type.SqlDbType == SqlDbType.Structured)
                parameter.TypeName = Type.Name;

            parameter.Direction = Direction;

            return parameter;
        }

        /// <summary>
        /// Sets the SQL parameter value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameter">The parameter.</param>
        /// <param name="value">The value.</param>
        /// <param name="mode">The mode.</param>
        /// <exception cref="ArgumentNullException"><paramref name="parameter"/> is <see langword="null" />.</exception>
        /// <exception cref="DatabaseSchemaException"><para>The type <typeparamref name="T" /> is invalid for the <see cref="Direction" />.</para>
        /// <para>-or-</para>
        /// <para>The type <typeparamref name="T" /> was unsupported.</para>
        /// <para>-or-</para>
        /// <para>A fatal error occurred.</para>
        /// <para>-or-</para>
        /// <para>The object exceeded the SQL type's maximum <see cref="SqlTypeSize">size</see>.</para>
        /// <para>-or-</para>
        /// <para>The serialized object was truncated.</para>
        /// <para>-or-</para>
        /// <para>Unicode characters were found and only ASCII characters are supported in the SQL type.</para>
        /// <para>-or-</para>
        /// <para>The date was outside the range of accepted dates for the SQL type.</para></exception>
        public void SetSqlParameterValue<T>(
            [NotNull] SqlParameter parameter,
            T value,
            TypeConstraintMode mode = TypeConstraintMode.Warn)
        {
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));

            if (value == null)
            {
                parameter.Value = Type.IsTable ? null : DBNull.Value;
                return;
            }

            IOut output = value as IOut;
            if (output != null)
            {
                bool hasInput = output?.InputValue.IsAssigned ?? false;
                string dir = hasInput ? "input/output" : "output";
                switch (Direction)
                {
                    case ParameterDirection.Input:
                        throw new DatabaseSchemaException(
                            LoggingLevel.Error,
                            () => "Cannot pass {0} value to input only parameter {1}",
                            dir,
                            FullName);
                    case ParameterDirection.Output:
                    case ParameterDirection.ReturnValue:
                        if (hasInput)
                            throw new DatabaseSchemaException(
                                LoggingLevel.Error,
                                () => "Cannot pass {0} value to output only parameter {1}",
                                dir,
                                FullName);
                        break;
                }
                
                output.SetParameter(parameter);

                parameter.Value = hasInput
                    ? Type.CastCLRValue(output.InputValue.Value, output.Type, mode)
                    : DBNull.Value;
            }
            else
                parameter.Value = Type.CastCLRValue(value, mode);
        }

        /// <summary>
        ///   Casts the CLR value to the correct SQL type.
        /// </summary>
        /// <param name="value">The CLR value to cast.</param>
        /// <param name="clrType">The CLR type of the value to cast.</param>
        /// <param name="mode">
        ///   <para>The constraint mode.</para>
        ///   <para>By default this is set to give a warning if truncation/loss of precision occurs.</para>
        /// </param>
        /// <returns>
        ///   The result (if possible); otherwise returns the <paramref name="value"/> passed in.
        /// </returns>
        /// <exception cref="DatabaseSchemaException">
        ///   <para>The <paramref name="clrType"/> was unsupported.</para>
        ///   <para>-or-</para>
        ///   <para>A fatal error occurred.</para>
        ///   <para>-or-</para>
        ///   <para>The object exceeded the SQL type's maximum <see cref="SqlTypeSize">size</see>.</para>
        ///   <para>-or-</para>
        ///   <para>The serialized object was truncated.</para>
        ///   <para>-or-</para>
        ///   <para>Unicode characters were found and only ASCII characters are supported in the SQL type.</para>
        ///   <para>-or-</para>
        ///   <para>The date was outside the range of accepted dates for the SQL type.</para>
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="clrType"/> is <see langword="null" />.</exception>
        [CanBeNull]
        public object CastCLRValue(
            [CanBeNull] object value,
            [NotNull] Type clrType,
            TypeConstraintMode mode = TypeConstraintMode.Warn)
        {
            return Type.CastCLRValue(value, clrType, mode);
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
        /// <exception cref="DatabaseSchemaException">
        ///   <para>The type <typeparamref name="T"/> was unsupported.</para>
        ///   <para>-or-</para>
        ///   <para>A fatal error occurred.</para>
        ///   <para>-or-</para>
        ///   <para>The object exceeded the SQL type's maximum <see cref="SqlTypeSize">size</see>.</para>
        ///   <para>-or-</para>
        ///   <para>The serialized object was truncated.</para>
        ///   <para>-or-</para>
        ///   <para>Unicode characters were found and only ASCII characters are supported in the SQL type.</para>
        ///   <para>-or-</para>
        ///   <para>The date was outside the range of accepted dates for the SQL type.</para>
        /// </exception>
        [CanBeNull]
        public object CastCLRValue<T>([CanBeNull] T value, TypeConstraintMode mode = TypeConstraintMode.Warn)
        {
            return Type.CastCLRValue(value, mode);
        }

        /// <summary>
        ///   Casts the SQL value to the correct CLR type.
        /// </summary>
        /// <param name="value">The CLR value to cast.</param>
        /// <param name="clrType">The CLR type of the value to cast.</param>
        /// <param name="mode">
        ///   <para>The constraint mode.</para>
        ///   <para>By default this is set to give a warning if truncation/loss of precision occurs.</para>
        /// </param>
        /// <returns>
        ///   The result (if possible); otherwise returns the <paramref name="value"/> passed in.
        /// </returns>
        /// <exception cref="DatabaseSchemaException">
        ///   <para>The type <typeparamref name="T"/> was unsupported.</para>
        ///   <para>-or-</para>
        ///   <para>A fatal error occurred.</para>
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="clrType"/> is <see langword="null" />.</exception>
        [CanBeNull]
        public object CastSQLValue(object value, [NotNull] Type clrType, TypeConstraintMode mode = TypeConstraintMode.Warn)
        {
            return Type.CastSQLValue(value, clrType, mode);
        }

        /// <summary>
        ///   Casts the SQL value to the correct CLR type.
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
        /// <exception cref="DatabaseSchemaException">
        ///   <para>The type <typeparamref name="T"/> was unsupported.</para>
        ///   <para>-or-</para>
        ///   <para>A fatal error occurred.</para>
        /// </exception>
        [CanBeNull]
        public object CastSQLValue<T>(T value, TypeConstraintMode mode = TypeConstraintMode.Warn)
        {
            return Type.CastSQLValue(value, mode);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString() => ToString(Type.Size);

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="size">The size of the type.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        [NotNull]
        public string ToString(SqlTypeSize size)
        {
            if (Type.IsTable)
            {
                Debug.Assert(IsReadOnly);
                return FullName + " " + Type.ToString(size) + " READONLY";
            }
            Debug.Assert(!IsReadOnly);

            if (Direction == ParameterDirection.Input)
                return FullName + " " + Type.ToString(size);
            return FullName + " " + Type.ToString(size) + " OUTPUT";
        }
    }
}