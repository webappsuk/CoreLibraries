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
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Xml;
using System.Xml.Linq;
using JetBrains.Annotations;
using Microsoft.SqlServer.Server;
using Microsoft.SqlServer.Types;
using WebApplications.Utilities.Database.Exceptions;
using WebApplications.Utilities.Logging;
using WebApplications.Utilities.Ranges;
using WebApplications.Utilities.Serialization;

namespace WebApplications.Utilities.Database.Schema
{
    /// <summary>
    ///   Holds information about a type.
    /// </summary>
    public class SqlType : DatabaseSchemaEntity<SqlType>
    {
        /// <summary>
        /// The properties used for calculating differences.
        /// </summary>
        [UsedImplicitly]
        [NotNull]
        private static readonly Expression<Func<SqlType, object>>[] _properties =
            new Expression<Func<SqlType, object>>[]
            {
                t => t.IsCLR,
                t => t.IsNullable,
                t => t.IsUserDefined,
                t => t.IsTable,
                t => t.Size,
                t => t.BaseType
            };

        /// <summary>
        ///   Valid date ranges for date types.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly Dictionary<SqlDbType, DateTimeRange> DateTypeSizes =
            new Dictionary<SqlDbType, DateTimeRange>
                {
                    {SqlDbType.Date, new DateTimeRange(new DateTime(1, 1, 1), new DateTime(9999, 12, 31))},
                    {
                        SqlDbType.DateTime,
                        new DateTimeRange(new DateTime(1753, 1, 1), new DateTime(9999, 12, 31, 23, 59, 59, 997))
                    },
                    {
                        SqlDbType.SmallDateTime,
                        new DateTimeRange(new DateTime(1900, 1, 1), new DateTime(2079, 6, 6, 23, 59, 59))
                    },
                    {
                        SqlDbType.DateTime2,
                        new DateTimeRange(new DateTime(1, 1, 1), new DateTime(9999, 12, 31, 23, 59, 59, 999))
                    },
                    {
                        SqlDbType.DateTimeOffset,
                        new DateTimeRange(new DateTime(1, 1, 1), new DateTime(9999, 12, 31, 23, 59, 59))
                    }
                };

        /// <summary>
        ///   Holds the SQL data type to <see cref="SqlDbType"/> mapping.
        /// </summary>
        [NotNull]
        private static readonly Dictionary<string, SqlDbType> _systemTypes =
            new Dictionary<string, SqlDbType>
                {
                    {"bigint", SqlDbType.BigInt},
                    {"binary", SqlDbType.Binary},
                    {"bit", SqlDbType.Bit},
                    {"char", SqlDbType.Char},
                    {"date", SqlDbType.Date},
                    {"datetime", SqlDbType.DateTime},
                    {"datetime2", SqlDbType.DateTime2},
                    {"datetimeoffset", SqlDbType.DateTimeOffset},
                    {"decimal", SqlDbType.Decimal},
                    {"float", SqlDbType.Float},
                    {"geography", SqlDbType.Udt},
                    {"geometry", SqlDbType.Udt},
                    {"hierarchyid", SqlDbType.Udt},
                    {"image", SqlDbType.Image},
                    {"int", SqlDbType.Int},
                    {"money", SqlDbType.Money},
                    {"nchar", SqlDbType.NChar},
                    {"ntext", SqlDbType.NText},
                    {"numeric", SqlDbType.Decimal},
                    {"nvarchar", SqlDbType.NVarChar},
                    {"real", SqlDbType.Real},
                    {"smalldatetime", SqlDbType.SmallDateTime},
                    {"smallint", SqlDbType.SmallInt},
                    {"smallmoney", SqlDbType.SmallMoney},
                    {"sql_variant", SqlDbType.Variant},
                    {"sysname", SqlDbType.NVarChar},
                    {"text", SqlDbType.Text},
                    {"time", SqlDbType.Time},
                    {"timestamp", SqlDbType.Timestamp},
                    {"tinyint", SqlDbType.TinyInt},
                    {"uniqueidentifier", SqlDbType.UniqueIdentifier},
                    {"varbinary", SqlDbType.VarBinary},
                    {"varchar", SqlDbType.VarChar},
                    {"xml", SqlDbType.Xml}
                };

        /// <summary>
        ///   The base type (if any).
        /// </summary>
        [UsedImplicitly]
        [CanBeNull]
        public readonly SqlType BaseType;

        /// <summary>
        ///   A <see cref="bool"/> value indicating whether this is a CLR type.
        /// </summary>
        [UsedImplicitly]
        public readonly bool IsCLR;

        /// <summary>
        ///   A <see cref="bool"/> value indicating whether the type accepts nulls.
        /// </summary>
        [UsedImplicitly]
        public readonly bool IsNullable;

        /// <summary>
        ///   A <see cref="bool"/> value indicating whether this type is a table.
        /// </summary>
        [UsedImplicitly]
        public readonly bool IsTable;

        /// <summary>
        ///   A <see cref="bool"/> value indicating whether this is a user defined type.
        /// </summary>
        [UsedImplicitly]
        public readonly bool IsUserDefined;

        /// <summary>
        ///   The type name.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public readonly string Name;

        /// <summary>
        ///   The <see cref="SqlTypeSize">size</see> of the type.
        /// </summary>
        [UsedImplicitly]
        public readonly SqlTypeSize Size;

        /// <summary>
        ///   The CLR type converters for this type.
        /// </summary>
        [NotNull]
        private readonly ConcurrentDictionary<Type, Func<object, TypeConstraintMode, object>>
            _clrTypeConverters = new ConcurrentDictionary<Type, Func<object, TypeConstraintMode, object>>();

        /// <summary>
        ///   Initializes a new instance of the <see cref="SqlType"/> class.
        /// </summary>
        /// <param name="baseType">The base type.</param>
        /// <param name="sqlSchema">The name of the schema the type belongs to.</param>
        /// <param name="name">The type name.</param>
        /// <param name="size">The size information.</param>
        /// <param name="isNullable">
        ///   If set to <see langword="true"/> the value can be <see langword="null"/>.
        /// </param>
        /// <param name="isUserDefined">
        ///   If set to <see langword="true"/> the value is a user defined type.
        /// </param>
        /// <param name="isClr">
        ///   If set to <see langword="true"/> the value is a CLR type.
        /// </param>
        internal SqlType(
            [CanBeNull] SqlType baseType,
            [NotNull] SqlSchema sqlSchema,
            [NotNull] string name,
            SqlTypeSize size,
            bool isNullable,
            bool isUserDefined,
            bool isClr)
            : this(
                baseType,
                sqlSchema,
                name,
                size,
                isNullable,
                isUserDefined,
                isClr,
                false)
        {
            Contract.Requires(sqlSchema != null);
            Contract.Requires(!String.IsNullOrWhiteSpace(name));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlType" /> class.
        /// </summary>
        /// <param name="baseType">The base type.</param>
        /// <param name="sqlSchema">The name of the schema the type belongs to.</param>
        /// <param name="name">The type name.</param>
        /// <param name="size">The size information.</param>
        /// <param name="isNullable">If set to <see langword="true" /> the value can be <see langword="null" />.</param>
        /// <param name="isUserDefined">If set to <see langword="true" /> the value is a user defined type.</param>
        /// <param name="isClr">If set to <see langword="true" /> the value is a CLR type.</param>
        /// <param name="isTable">If set to <see langword="true" /> the type is a table.</param>
        protected SqlType(
            [CanBeNull] SqlType baseType,
            [NotNull] SqlSchema sqlSchema,
            [NotNull] string name,
            SqlTypeSize size,
            bool isNullable,
            bool isUserDefined,
            bool isClr,
            bool isTable)
            : base(sqlSchema, name)
        {
            Contract.Requires(sqlSchema != null);
            Contract.Requires(name != null);
            Contract.Requires(!String.IsNullOrWhiteSpace(name));
            Name = name;
            IsTable = isTable;
            IsCLR = isClr;
            IsUserDefined = isUserDefined;
            IsNullable = isNullable;
            Size = size;
            BaseType = baseType;

            // Calculate our underlying SqlDbType
            if (!IsUserDefined)
            {
                SqlDbType sqlDbType;
                // We are a system type, look up.
                if (_systemTypes.TryGetValue(Name, out sqlDbType))
                    SqlDbType = sqlDbType;
                else if (BaseType != null)
                    // Get the base type
                    SqlDbType = BaseType.SqlDbType;
                else
                {
                    // Log error and return NVarChar as fall back.
                    new DatabaseSchemaException(
                        LoggingLevel.Warning,
                        () => Resources.SqlType_UnknownSqlSystemType,
                        FullName);
                    SqlDbType = SqlDbType.NVarChar;
                }
            }
            else if (IsTable)
                SqlDbType = SqlDbType.Structured;
            else if (BaseType != null)
                // Get the base type
                SqlDbType = BaseType.SqlDbType;
            else
            {
                // Log error and return NVarChar as fall back.
                new DatabaseSchemaException(
                    LoggingLevel.Critical,
                    () => Resources.SqlType_UnknownSqlSystemType,
                    FullName);
                SqlDbType = SqlDbType.NVarChar;
            }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SqlType"/> class.
        /// </summary>
        /// <param name="baseType">The base type.</param>
        /// <param name="size">The size information.</param>
        /// <remarks>
        ///   There is a <see cref="System.Diagnostics.Contracts.Contract">contact</see>
        ///   that <paramref name="baseType"/> cannot be <see langword="null"/>.
        /// </remarks>
        internal SqlType([NotNull] SqlType baseType, SqlTypeSize size)
            : base(baseType.SqlSchema, baseType.Name)
        {
            Contract.Requires(baseType != null);
            Name = baseType.Name;
            IsTable = baseType.IsTable;
            IsCLR = baseType.IsCLR;
            IsUserDefined = baseType.IsUserDefined;
            IsNullable = baseType.IsNullable;
            BaseType = baseType;
            Size = size;
            SqlDbType = baseType.SqlDbType;
        }

        /// <summary>
        ///   Gets the corresponding <see cref="SqlDbType"/>.
        /// </summary>
        /// <value>The corresponding <see cref="SqlDbType"/>.</value>
        public SqlDbType SqlDbType { get; private set; }

        /// <summary>
        ///   Checks to see whether the specified CLR type can be used to represent the current <see cref="SqlType"/>.
        /// </summary>
        /// <param name="type">The CLR type to check.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the CLR type is acceptable; otherwise returns <see langword="false"/>.
        /// </returns>
        /// <remarks>
        ///   There is a <see cref="System.Diagnostics.Contracts.Contract">contact</see>
        ///   that <paramref name="type"/> cannot be <see langword="null"/>.
        /// </remarks>
        public bool AcceptsCLRType([NotNull] Type type)
        {
            Contract.Requires(type != null);
            return GetClrToSqlConverter(type) != null;
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
        public object CastCLRValue<T>(T value, TypeConstraintMode mode = TypeConstraintMode.Warn)
        {
            Func<object, TypeConstraintMode, object> converter = GetClrToSqlConverter(typeof(T));
            return converter != null ? converter(value, mode) : value;
        }

        /// <summary>
        ///   Gets the CLR to SQL type converter if one can be found for the specified CLR type.
        ///   The converter function takes two inputs, the CLR object to convert and also the
        ///   <see cref="TypeConstraintMode">constraint mode</see>, which determines what will
        ///   happen if truncation/loss of precision occurs.
        /// </summary>
        /// <typeparam name="T">The CLR type to retrieve the converter for.</typeparam>
        /// <returns>
        ///   The converter (if found); otherwise returns <see langword="null"/>.
        /// </returns>
        [CanBeNull]
        [UsedImplicitly]
        public Func<object, TypeConstraintMode, object> GetClrToSqlConverter<T>()
        {
            return GetClrToSqlConverter(typeof(T));
        }

        /// <summary>
        ///   Gets the CLR to SQL type converter if one can be found for the specified CLR type.
        ///   The converter function takes two inputs, the CLR object to convert and also the
        ///   <see cref="TypeConstraintMode">constraint mode</see>, which determines what will
        ///   happen if truncation/loss of precision occurs.
        /// </summary>
        /// <param name="clrType">The CLR type to retrieve the converter for.</param>
        /// <returns>
        ///   The converter (if found); otherwise returns <see langword="null"/>.
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
        [CanBeNull]
        [UsedImplicitly]
        public Func<object, TypeConstraintMode, object> GetClrToSqlConverter([NotNull] Type clrType)
        {
            Contract.Requires(clrType != null);
            return _clrTypeConverters.GetOrAdd(
                clrType,
                t =>
                {
                    // Should never happen, but if it does there is not conversion from a null type!
                    if (t == null)
                    {
                        // Log error, but don't throw it.
                        new DatabaseSchemaException(
                            LoggingLevel.Critical,
                            () => Resources.SqlType_GetClrToSqlConverter_NoTypeSpecified,
                            this);
                        return null;
                    }

                    try
                    {
                        // Check for standard conversions first
                        switch (SqlDbType)
                        {
                            case SqlDbType.BigInt:
                                return CreateConverter<long>(t);
                            case SqlDbType.Binary:
                            case SqlDbType.Image:
                            case SqlDbType.Timestamp:
                            case SqlDbType.VarBinary:
                                // If we have a byte[] (or can cast to byte[]) then use that.
                                Func<object, TypeConstraintMode, object> converter = CreateConverter<byte[]>(
                                    t,
                                    (c, m) =>
                                    {
                                        if (c == null)
                                            return SqlBinary.Null;
                                        // Check for truncation.
                                        if ((m != TypeConstraintMode.Silent) &&
                                            (Size.MaximumLength > -1) &&
                                            (Size.MaximumLength < c.Length))
                                        {
                                            if (m == TypeConstraintMode.Error)
                                                throw new DatabaseSchemaException(
                                                    LoggingLevel.Error,
                                                    () => Resources.
                                                        SqlType_GetClrToSqlConverter_CouldNotConvertBinaryData,
                                                    c.Length,
                                                    FullName,
                                                    Size.MaximumLength);

                                            // Warn - don't need to truncate as that happens under the hood anyway.
                                            Log.Add(
                                                LoggingLevel.Warning,
                                                () => Resources.SqlType_GetClrToSqlConverter_BinaryDataTruncated,
                                                c.Length,
                                                FullName,
                                                Size.MaximumLength);
                                        }
                                        return c;
                                    });
                                if (converter != null)
                                    return converter;

                                // Support serializable objects.
                                if (t.IsSerializable)
                                    return
                                        (c, m) =>
                                        {
                                            if (c == null)
                                                return SqlBinary.Null;
                                            // Serialize object
                                            byte[] serializedObject = c.SerializeToByteArray();

                                            // Check for truncation.
                                            if (Size.MaximumLength > -1)
                                                // NOTE We always fail regardless of truncation mode as a partially serialized object is junk.
                                                if (Size.MaximumLength < serializedObject.Length)
                                                    throw new DatabaseSchemaException(
                                                        LoggingLevel.Error,
                                                        () => Resources.
                                                            SqlType_GetClrToSqlConverter_CouldNotSerializeObject,
                                                        t.FullName,
                                                        FullName,
                                                        serializedObject.Length,
                                                        Size.MaximumLength);
                                            return serializedObject;
                                        };

                                // Do not support this type.
                                return null;
                            case SqlDbType.Bit:
                                return CreateConverter<bool>(t);
                            case SqlDbType.Char:
                            case SqlDbType.Text:
                            case SqlDbType.VarChar:
                                return CreateConverter<string>(
                                    t,
                                    (c, m) =>
                                    {
                                        if (c == null)
                                            return SqlString.Null;
                                        if (m != TypeConstraintMode.Silent)
                                        {
                                            // Check for truncation
                                            if ((Size.MaximumLength > -1) &&
                                                (c.Length > Size.MaximumLength))
                                            {
                                                if (m == TypeConstraintMode.Error)
                                                    throw new DatabaseSchemaException(
                                                        LoggingLevel.Error,
                                                        () => Resources.
                                                            SqlType_GetClrToSqlConverter_CouldNotConvertString,
                                                        c.Length,
                                                        FullName,
                                                        Size.MaximumLength);

                                                // Warn - don't need to truncate as that happens under the hood anyway.
                                                Log.Add(
                                                    LoggingLevel.Warning,
                                                    () => Resources.SqlType_GetClrToSqlConverter_StringTruncated,
                                                    c.Length,
                                                    FullName,
                                                    Size.MaximumLength);
                                            }

                                            // Check for non ASCII chars
                                            if (c.Any(ch => ch > 255))
                                            {
                                                if (m == TypeConstraintMode.Error)
                                                    throw new DatabaseSchemaException(
                                                        LoggingLevel.Error,
                                                        () => Resources.
                                                            SqlType_GetClrToSqlConverter_StringContainsUnicodeCharacters,
                                                        FullName);

                                                // Warn - don't need to do anything as that happens under the hood anyway.
                                                Log.Add(
                                                    LoggingLevel.Warning,
                                                    () => Resources.SqlType_GetClrToSqlConverter_UnicodeCharactersLost,
                                                    FullName);
                                            }
                                        }
                                        return new SqlString(c);
                                    });
                            case SqlDbType.NChar:
                            case SqlDbType.NText:
                            case SqlDbType.NVarChar:
                                return CreateConverter<string>(
                                    t,
                                    (c, m) =>
                                    {
                                        if (c == null)
                                            return SqlString.Null;
                                        // SQL reports size in bytes rather than characters - so divide by 2.
                                        int sqlSize = Size.MaximumLength / 2;
                                        if ((m != TypeConstraintMode.Silent) &&
                                            (Size.MaximumLength > -1) &&
                                            (c.Length > Size.MaximumLength / 2))
                                        {
                                            if (m == TypeConstraintMode.Error)
                                                throw new DatabaseSchemaException(
                                                    LoggingLevel.Error,
                                                    () => Resources
                                                        .SqlType_GetClrToSqlConverter_CouldNotConvertString,
                                                    c.Length,
                                                    FullName,
                                                    sqlSize);

                                            // Warn - don't need to truncate as that happens under the hood anyway.
                                            Log.Add(
                                                LoggingLevel.Warning,
                                                () => Resources.SqlType_GetClrToSqlConverter_StringTruncated,
                                                c.Length,
                                                FullName,
                                                sqlSize);
                                        }
                                        return new SqlString(c);
                                    });
                            case SqlDbType.SmallDateTime:
                            case SqlDbType.DateTime:
                            case SqlDbType.Date:
                            case SqlDbType.DateTime2:
                            case SqlDbType.DateTimeOffset:
                                return clrType.IsGenericType &&
                                       clrType.GetGenericTypeDefinition() == typeof(Nullable<>)
                                    ? CreateConverter<DateTime?>(
                                        t,
                                        (c, m) =>
                                        {
                                            if (c == null)
                                                return DBNull.Value;

                                            DateTime original = (DateTime)c;
                                            DateTimeRange range = DateTypeSizes[SqlDbType];
                                            DateTime bound = range.Bind(original);

                                            if ((m != TypeConstraintMode.Silent) &&
                                                (original != bound))
                                            {
                                                if (m == TypeConstraintMode.Error)
                                                    throw new DatabaseSchemaException(
                                                        LoggingLevel.Error,
                                                        () => Resources.
                                                            SqlType_GetClrToSqlConverter_CouldNotConvertDateTimeToSqlType,
                                                        original,
                                                        FullName,
                                                        range);

                                                // Warn - don't need to truncate as that happens under the hood anyway.
                                                Log.Add(
                                                    LoggingLevel.Warning,
                                                    () => Resources.
                                                        SqlType_GetClrToSqlConverter_DateTimeTruncated,
                                                    original,
                                                    FullName,
                                                    range);
                                            }
                                            return bound;
                                        })
                                    : CreateConverter<DateTime>(
                                        t,
                                        (c, m) =>
                                        {
                                            DateTimeRange range = DateTypeSizes[SqlDbType];
                                            DateTime bound = range.Bind(c);

                                            if ((m != TypeConstraintMode.Silent) &&
                                                (c != bound))
                                            {
                                                if (m == TypeConstraintMode.Error)
                                                    throw new DatabaseSchemaException(
                                                        LoggingLevel.Error,
                                                        () => Resources.
                                                            SqlType_GetClrToSqlConverter_CouldNotConvertDateTimeToSqlType,
                                                        c,
                                                        FullName,
                                                        range);

                                                // Warn - don't need to truncate as that happens under the hood anyway.
                                                Log.Add(
                                                    LoggingLevel.Warning,
                                                    () => Resources.
                                                        SqlType_GetClrToSqlConverter_DateTimeTruncated,
                                                    c,
                                                    FullName,
                                                    range);
                                            }
                                            return bound;
                                        });
                            case SqlDbType.Time:
                                return CreateConverter<TimeSpan>(t);
                            case SqlDbType.Decimal:
                                return CreateConverter<decimal>(t);
                            case SqlDbType.Float:
                                return CreateConverter<double>(t);
                            case SqlDbType.Int:
                                return CreateConverter<int>(t);
                            case SqlDbType.Money:
                            case SqlDbType.SmallMoney:
                                return CreateConverter<decimal>(t);
                            case SqlDbType.Real:
                                return CreateConverter<float>(t);
                            case SqlDbType.UniqueIdentifier:
                                return CreateConverter<Guid>(t);
                            case SqlDbType.SmallInt:
                                return CreateConverter<short>(t);
                            case SqlDbType.TinyInt:
                                return CreateConverter<byte>(t);
                            case SqlDbType.Variant:
                                // TODO Variant type can't accept everything!
                                return (c, m) => c;
                            case SqlDbType.Xml:
                                return
                                    CreateConverter<XNode>(
                                        t,
                                        (c, m) =>
                                        {
                                            if (c == null)
                                                return (object)SqlXml.Null;
                                            using (XmlReader xmlNodeReader = c.CreateReader())
                                                return new SqlXml(xmlNodeReader);
                                        })
                                    ??
                                    CreateConverter<XmlNode>(
                                        t,
                                        (c, m) =>
                                        {
                                            if (c == null)
                                                return (object)SqlXml.Null;
                                            using (XmlNodeReader xmlNodeReader = new XmlNodeReader(c))
                                                return new SqlXml(xmlNodeReader);
                                        });
                            case SqlDbType.Udt:
                                // Add UDT conversions (which depend on the type name).
                                switch (Name)
                                {
                                    case "geography":
                                        return CreateConverter<SqlGeography>(t, false);
                                    case "geometry":
                                        return CreateConverter<SqlGeometry>(t, false);
                                    case "hierarchyid":
                                        return CreateConverter<SqlHierarchyId>(t, false);
                                    default:
                                        // Log error, but don't throw it.
                                        new DatabaseSchemaException(
                                            LoggingLevel.Critical,
                                            () => Resources
                                                .SqlType_GetClrToSqlConverter_UdtTypeNotSupported,
                                            Name);
                                        return null;
                                }
                            case SqlDbType.Structured:
                                // This is actually a SqlTableType.
                                SqlTableType tableType = this as SqlTableType;
                                if (tableType == null)
                                {
                                    // Log error, but don't throw it.
                                    new DatabaseSchemaException(
                                        LoggingLevel.Critical,
                                        () => Resources
                                            .SqlType_GetClrToSqlConverter_NotCreatedAsSqlTableType,
                                        this);
                                    return null;
                                }

                                SqlType[] columnSqlTypes =
                                    tableType.TableDefinition.Columns.Select(c => c.Type).ToArray();
                                int columns = columnSqlTypes.Length;

                                // Find out last non-nullable column to determine the minimum number of columns that must
                                // be supplied - as columns are unnamed on the input (we only have the type to go by), the we
                                // cannot 'skip' columns, and all non-null columns must be supplied.
                                int minColumns;
                                for (minColumns = columns; minColumns > 1; minColumns--)
                                    if (!columnSqlTypes[minColumns - 1].IsNullable)
                                        break;

                                // Check if type implements IEnumerable<T>
                                Type enumerableInterface =
                                    t.IsGenericType && (t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                                        ? t
                                        : t.GetInterfaces()
                                            .FirstOrDefault(
                                                it =>
                                                    it.IsGenericType &&
                                                    it.GetGenericTypeDefinition() == typeof(IEnumerable<>));

                                if (enumerableInterface != null)
                                {
                                    // Get the enumeration type
                                    Type enumerationType = enumerableInterface.GetGenericArguments().First();

                                    // If the type passed in is assignable from IEnumerable<SqlDataRecord> then we can pass straight through
                                    // after checking there are rows available.
                                    if (typeof(SqlDataRecord).IsAssignableFrom(enumerationType))
                                        // We are the base non-generic table type.
                                        return (Func<object, TypeConstraintMode, object>)
                                            ((c, m) =>
                                            {
                                                if (c == null)
                                                    return null;
                                                List<SqlDataRecord> records =
                                                    ((IEnumerable<SqlDataRecord>)c).ToList();
                                                return records.Count < 1 ? null : records;
                                            });


                                    // Check if we are IEnumerable<Tuple<...>>
                                    if (enumerationType.IsGenericType &&
                                        enumerationType.GetInterfaces().Any(i => i.FullName == "System.ITuple"))
                                    {
                                        // Get converters for tuple elements
                                        Type[] tupleTypes = enumerationType.GetIndexTypes();
                                        int items = tupleTypes.Length;

                                        // Check we don't have too many items in the tuple.
                                        if (items > columns)
                                        {
                                            // Log error, but don't throw it.
                                            new DatabaseSchemaException(
                                                LoggingLevel.Critical,
                                                () => Resources.
                                                    SqlType_GetClrToSqlConverter_ColumnNumberAndTupleSizeMismatch,
                                                this,
                                                enumerationType.Name,
                                                columns,
                                                items);
                                            return null;
                                        }

                                        // Check we don't have too few!
                                        if (items < minColumns)
                                        {
                                            // Log error, but don't throw it.
                                            new DatabaseSchemaException(
                                                LoggingLevel.Critical,
                                                () => Resources.
                                                    SqlType_GetClrToSqlConverter_ColumnNumberAndTupleSizeMismatch,
                                                this,
                                                enumerationType.Name,
                                                columns,
                                                items);
                                            return null;
                                        }

                                        // Get the tuple indexer (note this supports extended/nested tuples).
                                        Func<object, int, object> indexer = enumerationType.GetTupleIndexer();

                                        Func<object, TypeConstraintMode, object>[] converters =
                                            new Func<object, TypeConstraintMode, object>[tupleTypes.Length];
                                        for (int i = 0; i < tupleTypes.Length; i++)
                                        {
                                            Func<object, TypeConstraintMode, object> columnConverter =
                                                columnSqlTypes[i].GetClrToSqlConverter(tupleTypes[i]);
                                            if (columnConverter == null)
                                            {
                                                // Log error, but don't throw it.
                                                new DatabaseSchemaException(
                                                    LoggingLevel.Critical,
                                                    () => Resources
                                                        .SqlType_GetClrToSqlConverter_CanNotCast,
                                                    this,
                                                    enumerationType.Name,
                                                    tupleTypes[i],
                                                    columnSqlTypes[i],
                                                    i);
                                                return null;
                                            }

                                            // TODO A direct expression tree would probably be quicker... need to performance test.
                                            int cindex = i;
                                            converters[i] = (o, m) => columnConverter(indexer(o, cindex), m);
                                        }

                                        // Create lambda
                                        return (Func<object, TypeConstraintMode, object>)
                                            ((c, m) =>
                                            {
                                                IEnumerable enumerable = c as IEnumerable;
                                                if (enumerable == null)
                                                    return null;
                                                List<SqlDataRecord> records = new List<SqlDataRecord>();
                                                SqlMetaData[] sqlMetaData =
                                                    tableType.TableDefinition.SqlMetaData;
                                                foreach (object o in enumerable)
                                                {
                                                    SqlDataRecord record = new SqlDataRecord(sqlMetaData);
                                                    for (int i = 0; i < items; i++)
                                                        record.SetValue(i, converters[i](o, m));
                                                    records.Add(record);
                                                }

                                                // If we have zero count return DBNull (never return empty enumeration!)
                                                return records.Count < 1
                                                    ? (object)null
                                                    : records;
                                            });
                                    }

                                    // If we're single column we support enumeration of column type
                                    if (minColumns < 2)
                                    {
                                        // Get converter for column type
                                        SqlType columnType = columnSqlTypes[0];
                                        Func<object, TypeConstraintMode, object> enumConverter =
                                            columnType.GetClrToSqlConverter(enumerationType);
                                        if (enumConverter != null)
                                            // Create lambda
                                            return (Func<object, TypeConstraintMode, object>)
                                                ((c, m) =>
                                                {
                                                    IEnumerable enumerable = c as IEnumerable;
                                                    if (enumerable == null)
                                                        return null;
                                                    List<SqlDataRecord> records = new List<SqlDataRecord>();
                                                    SqlMetaData[] sqlMetaData =
                                                        tableType.TableDefinition.SqlMetaData;
                                                    foreach (object o in enumerable)
                                                    {
                                                        SqlDataRecord record = new SqlDataRecord(sqlMetaData);
                                                        record.SetValue(0, enumConverter(o, m));
                                                        records.Add(record);
                                                    }

                                                    // If we have zero count return DBNull (never return empty enumeration!)
                                                    return records.Count < 1
                                                        ? (object)null
                                                        : records;
                                                });
                                    }

                                    // If we have more than 1 column, but we need less than 3, then a keyvaluepair is fine.
                                    if ((minColumns < 3) &&
                                        (columns > 1) &&
                                        (enumerationType.IsGenericType &&
                                         enumerationType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>)))
                                    {
                                        // Get converter for key and value column types
                                        Type[] kvpTypes = enumerationType.GetGenericArguments();

                                        SqlType keyColumnType = columnSqlTypes[0];
                                        SqlType valueColumnType = columnSqlTypes[1];
                                        Func<object, TypeConstraintMode, object> keyConverter =
                                            keyColumnType.GetClrToSqlConverter(kvpTypes[0]);
                                        if (keyConverter == null)
                                        {
                                            // Log error, but don't throw it.
                                            new DatabaseSchemaException(
                                                LoggingLevel.Critical,
                                                () => Resources
                                                    .SqlType_GetClrToSqlConverter_CanNotConvertKeyType,
                                                this,
                                                enumerationType.Name,
                                                kvpTypes[0],
                                                keyColumnType);
                                            return null;
                                        }

                                        // Create key selector
                                        Func<object, object> keySelector =
                                            enumerationType.GetGetter<object, object>("Key");
                                        Func<object, TypeConstraintMode, object> k =
                                            (o, m) => keyConverter(keySelector(o), m);

                                        Func<object, TypeConstraintMode, object> valueConverter =
                                            valueColumnType.GetClrToSqlConverter(kvpTypes[1]);
                                        if (valueConverter == null)
                                        {
                                            // Log error, but don't throw it.
                                            new DatabaseSchemaException(
                                                LoggingLevel.Critical,
                                                () => Resources
                                                    .SqlType_GetClrToSqlConverter_CannotAcceptEnumerationType,
                                                this,
                                                enumerationType.Name);
                                            return null;
                                        }
                                        Func<object, object> valueSelector =
                                            enumerationType.GetGetter<object, object>("Value");
                                        Func<object, TypeConstraintMode, object> v =
                                            (o, m) => valueConverter(valueSelector(o), m);


                                        // Create lambda
                                        return (Func<object, TypeConstraintMode, object>)
                                            ((c, m) =>
                                            {
                                                IEnumerable enumerable = c as IEnumerable;
                                                if (enumerable == null)
                                                    return null;
                                                List<SqlDataRecord> records = new List<SqlDataRecord>();
                                                SqlMetaData[] sqlMetaData =
                                                    tableType.TableDefinition.SqlMetaData;
                                                foreach (object o in enumerable)
                                                {
                                                    SqlDataRecord record = new SqlDataRecord(sqlMetaData);
                                                    record.SetValue(0, k(o, m));
                                                    record.SetValue(1, v(o, m));
                                                    records.Add(record);
                                                }

                                                // If we have zero count return DBNull (never return empty enumeration!)
                                                return records.Count < 1
                                                    ? (object)null
                                                    : records;
                                            });
                                    }

                                    // Unsupported Log error, but don't throw it.
                                    new DatabaseSchemaException(
                                        LoggingLevel.Critical,
                                        () => Resources
                                            .SqlType_GetClrToSqlConverter_CannotAcceptEnumerationType,
                                        this,
                                        enumerationType);
                                    return null;
                                }

                                // Unsupported Log error, but don't throw it.
                                new DatabaseSchemaException(
                                    LoggingLevel.Critical,
                                    () => Resources.SqlType_GetClrToSqlConverter_CannotAcceptType,
                                    this,
                                    t);
                                return null;
                            default:
                                throw new DatabaseSchemaException(
                                    LoggingLevel.Critical,
                                    () => Resources.SqlType_GetClrToSqlConverter_UnsupportedSqlDbType,
                                    SqlDbType);
                        }
                    }
                    catch (Exception e)
                    {
                        new DatabaseSchemaException(
                            e,
                            LoggingLevel.Critical,
                            () => Resources.SqlType_GetClrToSqlConverter_FatalErrorOccurred,
                            t,
                            this,
                            e.Message
                            );
                    }

                    // Unsupported conversion
                    return null;
                });
        }

        /// <summary>
        ///   Returns a converter that casts from the input type to the output type
        ///   (if the input type is assignable to the output type).
        /// </summary>
        /// <typeparam name="TClr">The CLR type to convert to.</typeparam>
        /// <param name="actualClrType">The CLR type to convert from.</param>
        /// <param name="supportNullable">
        ///   <para>If set to <see langword="true"/> then supports <see langword="null"/>.</para>
        ///   <para>By default this is set to <see langword="true"/>.</para>
        /// </param>
        /// <returns>
        ///   The created converter; or <see langword="null"/> if the input type is not assignable to the required input type.
        /// </returns>
        [CanBeNull]
        private static Func<object, TypeConstraintMode, object> CreateConverter<TClr>(
            [NotNull] Type actualClrType,
            bool supportNullable = true)
        {
            Contract.Requires(actualClrType != null);
            Contract.Requires(!supportNullable || actualClrType != null);

            bool isNullable = false;

            // Check if we support nullable types and the type is Nullable<>
            if (supportNullable &&
                actualClrType.IsGenericType &&
                actualClrType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // Get the generic parameter and find the relevant converter
                actualClrType = actualClrType.GetGenericArguments().First();
                isNullable = true;
            }

            // Find a conversion if any
            Func<object, object> converter = actualClrType.GetConversion(typeof(TClr));

            // If we didn't find one there is no known conversion.
            if (converter == null)
                return null;

            // If we're actually a nullable type then wrap.
            if (isNullable)
            {
                Func<object, object> conCopy = converter;
                converter = c => c.IsNull() ? DBNull.Value : conCopy(c);
            }

            // Ignore the type constraint.
            return (o, m) => converter(o);
        }

        /// <summary>
        ///   Returns a converter that casts from the input type to the output type.
        ///   (if the input type is assignable to the output type).
        /// </summary>
        /// <typeparam name="TClr">The CLR type to convert to.</typeparam>
        /// <param name="actualClrType">The CLR type to convert from.</param>
        /// <param name="converter">The type converter.</param>
        /// <returns>
        ///   The created  converter; or <see langword="null"/> if the input type is not assignable to the required input type.
        /// </returns>
        [CanBeNull]
        private static Func<object, TypeConstraintMode, object> CreateConverter<TClr>(
            [NotNull] Type actualClrType,
            [NotNull] Func<TClr, TypeConstraintMode, object> converter)
        {
            Contract.Requires(actualClrType != null);
            Contract.Requires(converter != null);
            Func<object, TClr> toInputType = actualClrType.GetConversion<object, TClr>();
            return toInputType != null
                ? (Func<object, TypeConstraintMode, object>)((c, m) => converter(toInputType(c), m))
                : null;
        }
    }
}