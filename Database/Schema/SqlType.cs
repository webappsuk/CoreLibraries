#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: Utilities.Database 
// Project: Utilities.Database
// File: SqlType.cs
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
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
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
    public class SqlType : IEquatable<SqlType>, IEqualityComparer<SqlType>
    {
        /// <summary>
        ///   Valid date ranges for date types.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly Dictionary<SqlDbType, DateTimeRange> DateTypeSizes =
            new Dictionary<SqlDbType, DateTimeRange>
                {
                    {SqlDbType.Date, new DateTimeRange(new DateTime(1, 1, 1), new DateTime(9999, 12, 31))},
                    {SqlDbType.DateTime, new DateTimeRange(new DateTime(1753, 1, 1), new DateTime(9999, 12, 31, 23, 59, 59, 997)) },
                    {SqlDbType.SmallDateTime,new DateTimeRange(new DateTime(1900, 1, 1), new DateTime(2079, 6, 6, 23, 59, 59))},
                    {SqlDbType.DateTime2,new DateTimeRange(new DateTime(1, 1, 1), new DateTime(9999, 12, 31, 23, 59, 59, 999))},
                    {SqlDbType.DateTimeOffset,new DateTimeRange(new DateTime(1, 1, 1), new DateTime(9999, 12, 31, 23, 59, 59))}
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
                    {"smalldatetime",SqlDbType.SmallDateTime},
                    {"smallint", SqlDbType.SmallInt},
                    {"smallmoney", SqlDbType.SmallMoney},
                    {"sql_variant", SqlDbType.Variant},
                    {"sysname", SqlDbType.NVarChar},
                    {"text", SqlDbType.Text},
                    {"time", SqlDbType.Time},
                    {"timestamp", SqlDbType.Timestamp},
                    {"tinyint", SqlDbType.TinyInt},
                    {"uniqueidentifier",SqlDbType.UniqueIdentifier},
                    {"varbinary", SqlDbType.VarBinary},
                    {"varchar", SqlDbType.VarChar},
                    {"xml", SqlDbType.Xml}
                };

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
        ///   The SQL schema the type belongs to.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public readonly string SchemaName;

        /// <summary>
        ///   The <see cref="SqlTypeSize">size</see> of the type.
        /// </summary>
        [UsedImplicitly]
        public readonly SqlTypeSize Size;

        private int? _hashCode;

        /// <summary>
        ///   Initializes a new instance of the <see cref="SqlType"/> class.
        /// </summary>
        /// <param name="baseType">The base type.</param>
        /// <param name="schemaName">The name of the schema the type belongs to.</param>
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
            [CanBeNull]SqlType baseType,
            [NotNull]string schemaName,
            [NotNull]string name,
            SqlTypeSize size,
            bool isNullable,
            bool isUserDefined,
            bool isClr)
            : this(baseType, schemaName, name, size, isNullable, isUserDefined, isClr, false)
        {
        }


        /// <summary>
        ///   Initializes a new instance of the <see cref="SqlType"/> class.
        /// </summary>
        /// <param name="baseType">The base type.</param>
        /// <param name="schemaName">The name of the schema the type belongs to.</param>
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
        /// <param name="isTable">
        ///   If set to <see langword="true"/> the type is a table.
        /// </param>
        /// <remarks>
        ///   There is a <see cref="System.Diagnostics.Contracts.Contract">contact</see>
        ///   specifying that <paramref name="name"/> and <paramref name="schemaName"/>
        ///   cannot be <see cref="string.IsNullOrWhiteSpace">null or whitespace</see>.
        /// </remarks>
        protected SqlType(
            [CanBeNull]SqlType baseType,
            [NotNull]string schemaName,
            [NotNull]string name,
            SqlTypeSize size,
            bool isNullable,
            bool isUserDefined,
            bool isClr,
            bool isTable)
        {
            Contract.Requires(!String.IsNullOrWhiteSpace(schemaName));
            Contract.Requires(!String.IsNullOrWhiteSpace(name));
            Name = name;
            SchemaName = schemaName;
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
                {
                    SqlDbType = sqlDbType;
                }
                else if (BaseType != null)
                {
                    // Get the base type
                    SqlDbType = BaseType.SqlDbType;
                }
                else
                {
                    // Log error and return NVarChar as fall back.
                    new DatabaseSchemaException(Resources.SqlType_UnknownSqlSystemType, LogLevel.Warning, FullName);
                    SqlDbType = SqlDbType.NVarChar;
                }
            }
            else if (IsTable)
                SqlDbType = SqlDbType.Structured;
            else if (BaseType != null)
            {
                // Get the base type
                SqlDbType = BaseType.SqlDbType;
            }
            else
            {
                // Log error and return NVarChar as fall back.
                new DatabaseSchemaException(Resources.SqlType_UnknownSqlSystemType, LogLevel.Critical, FullName);
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
        internal SqlType([NotNull]SqlType baseType, SqlTypeSize size)
        {
            Contract.Requires(baseType != null);
            Name = baseType.Name;
            SchemaName = baseType.SchemaName;
            IsTable = baseType.IsTable;
            IsCLR = baseType.IsCLR;
            IsUserDefined = baseType.IsUserDefined;
            IsNullable = baseType.IsNullable;
            BaseType = baseType;
            Size = size;
            SqlDbType = baseType.SqlDbType;
        }

        /// <summary>
        ///   The CLR type convertors for this type.
        /// </summary>
        [NotNull]
        private readonly ConcurrentDictionary<Type, Func<object, TypeConstraintMode, object>> _clrTypeConvertors = new ConcurrentDictionary<Type, Func<object, TypeConstraintMode, object>>();

        /// <summary>
        ///   The base type (if any).
        /// </summary>
        [UsedImplicitly]
        [CanBeNull]
        public readonly SqlType BaseType;

        /// <summary>
        ///   Gets the full name of the type.
        /// </summary>
        /// <value>
        ///   The schema name followed by the type name.
        /// </value>
        [NotNull]
        public string FullName
        {
            get { return String.Format("{0}.{1}", SchemaName, Name); }
        }

        /// <summary>
        ///   Gets the corresponding <see cref="SqlDbType"/>.
        /// </summary>
        /// <value>The corresponding <see cref="SqlDbType"/>.</value>
        public SqlDbType SqlDbType { get; private set; }

        #region IEqualityComparer<SqlType> Members
        /// <summary>
        ///   Returns a <see cref="bool"/> value that determines whether the specified objects are equal.
        /// </summary>
        /// <returns>
        ///   Returns <see langword="true"/> if the specified objects are equal; otherwise returns <see langword="false"/>.
        /// </returns>
        /// <param name="x">The first <see cref="SqlType"/> to compare.</param>
        /// <param name="y">The second <see cref="SqlType"/> to compare.</param>
        public bool Equals(SqlType x, SqlType y)
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
        /// <param name="obj">The <see cref ="object"/> for which a hash code is to be returned.</param>
        /// <exception cref="ArgumentNullException">
        ///   The type of <paramref name="obj" /> is a reference type and <paramref name="obj" /> is <see langword="null"/>.
        /// </exception>
        public int GetHashCode([NotNull]SqlType obj)
        {
            if (obj._hashCode == null)
            {
                obj._hashCode = obj.SchemaName.GetHashCode() ^ obj.Name.GetHashCode() ^ obj.Size.GetHashCode() ^
                                obj.IsNullable.GetHashCode() ^ obj.IsUserDefined.GetHashCode() ^ obj.IsCLR.GetHashCode() ^
                                obj.IsTable.GetHashCode();
            }
            return (int)obj._hashCode;
        }
        #endregion

        #region IEquatable<SqlType> Members
        /// <summary>
        ///   Gets a <see cref="bool"/> value indicating whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        ///   Returns <see langword="true"/> if the current object is equal to the <paramref name="other" />;
        ///   otherwise returns <see langword="false"/>.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(SqlType other)
        {
            if (other == null)
                return false;
            return (FullName == other.FullName) && (IsNullable == other.IsNullable) &&
                   (IsUserDefined == other.IsUserDefined) && (IsCLR == other.IsCLR) &&
                   (IsTable == other.IsTable) && (Size.Equals(other.Size)) &&
                   (BaseType != null ? BaseType.Equals(other.BaseType) : other.BaseType == null);
        }
        #endregion

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
        public bool AcceptsCLRType([NotNull]Type type)
        {
            Contract.Requires(type != null);
            return GetClrToSqlConvertor(type) != null;
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
            Func<object, TypeConstraintMode, object> convertor = GetClrToSqlConvertor(typeof(T));
            return convertor != null ? convertor(value, mode) : value;
        }

        /// <summary>
        ///   Gets the CLR to SQL type convertor if one can be found for the specified CLR type.
        ///   The convertor function takes two inputs, the CLR object to convert and also the
        ///   <see cref="TypeConstraintMode">constraint mode</see>, which determines what will
        ///   happen if truncation/loss of precision occurs.
        /// </summary>
        /// <typeparam name="T">The CLR type to retrieve the convertor for.</typeparam>
        /// <returns>
        ///   The convertor (if found); otherwise returns <see langword="null"/>.
        /// </returns>
        [CanBeNull]
        [UsedImplicitly]
        public Func<object, TypeConstraintMode, object> GetClrToSqlConvertor<T>()
        {
            return GetClrToSqlConvertor(typeof(T));
        }

        /// <summary>
        ///   Gets the CLR to SQL type convertor if one can be found for the specified CLR type.
        ///   The convertor function takes two inputs, the CLR object to convert and also the
        ///   <see cref="TypeConstraintMode">constraint mode</see>, which determines what will
        ///   happen if truncation/loss of precision occurs.
        /// </summary>
        /// <param name="clrType">The CLR type to retrieve the convertor for.</param>
        /// <returns>
        ///   The convertor (if found); otherwise returns <see langword="null"/>.
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
        public Func<object, TypeConstraintMode, object> GetClrToSqlConvertor([NotNull]Type clrType)
        {
            return _clrTypeConvertors.GetOrAdd(
                clrType,
                t =>
                {
                    // Should never happen, but if it does there is not conversion from a null type!
                    if (t == null)
                    {
                        // Log error, but don't throw it.
                        new DatabaseSchemaException(
                            Resources.SqlType_GetClrToSqlConverter_NoTypeSpecified,
                            LogLevel.Critical, this);
                        return null;
                    }

                    try
                    {
                        // Check for standard conversions first
                        switch (SqlDbType)
                        {
                            case SqlDbType.BigInt:
                                return CreateConvertor<long>(t);
                            case SqlDbType.Binary:
                            case SqlDbType.Image:
                            case SqlDbType.Timestamp:
                            case SqlDbType.VarBinary:
                                // If we have a byte[] (or can cast to byte[]) then use that.
                                var convertor = CreateConvertor<byte[]>(
                                    t,
                                    (c, m) =>
                                        {
                                            if (c == null)
                                                return SqlBinary.Null;
                                            // Check for truncation.
                                            if ((m != TypeConstraintMode.Silent) &&
                                                (this.Size.MaximumLength > -1) &&
                                                (this.Size.MaximumLength < c.Length))
                                            {
                                                if (m == TypeConstraintMode.Error)
                                                    throw new DatabaseSchemaException(
                                                        Resources.SqlType_GetClrToSqlConverter_CouldNotConvertBinaryData,
                                                        LogLevel.Error,
                                                        c.Length, 
                                                        this.FullName,
                                                        this.Size.MaximumLength);

                                                // Warn - don't need to truncate as that happens under the hood anyway.
                                                Log.Add(
                                                    Resources.SqlType_GetClrToSqlConverter_BinaryDataTruncated,
                                                    LogLevel.Warning,
                                                    c.Length,
                                                    this.FullName,
                                                    this.Size.MaximumLength);
                                            }
                                            return c;
                                        });
                                if (convertor != null)
                                    return convertor;

                                // Support serializable objects.
                                if (t.IsSerializable)
                                {
                                    return
                                        (c, m) =>
                                            {
                                                if (c == null)
                                                    return SqlBinary.Null;
                                                // Serialize object
                                                byte[] serializedObject = c.SerializeToByteArray();

                                                // Check for truncation.
                                                if (this.Size.MaximumLength > -1)
                                                {
                                                    // NOTE We always fail regardless of truncation mode as a partially serialized object is junk.
                                                    if (this.Size.MaximumLength < serializedObject.Length)
                                                        throw new DatabaseSchemaException(
                                                            Resources.SqlType_GetClrToSqlConverter_CouldNotSerializeObject,
                                                            LogLevel.Error,
                                                            t.FullName,
                                                            this.FullName,
                                                            serializedObject.Length,
                                                            this.Size.MaximumLength);
                                                }
                                                return serializedObject;
                                            };
                                }

                                // Do not support this type.
                                return null;
                            case SqlDbType.Bit:
                                return CreateConvertor<bool>(t);
                            case SqlDbType.Char:
                            case SqlDbType.Text:
                            case SqlDbType.VarChar:
                                return CreateConvertor<string>(
                                    t,
                                    (c, m) =>
                                        {
                                            if (c == null)
                                                return SqlString.Null;
                                            if (m != TypeConstraintMode.Silent)
                                            {
                                                // Check for truncation
                                                if ((this.Size.MaximumLength > -1) &&
                                                    (c.Length > this.Size.MaximumLength))
                                                {
                                                    if (m == TypeConstraintMode.Error)
                                                        throw new DatabaseSchemaException(
                                                            Resources.SqlType_GetClrToSqlConverter_CouldNotConvertString,
                                                            LogLevel.Error,
                                                            c.Length,
                                                            this.FullName,
                                                            this.Size.MaximumLength);

                                                    // Warn - don't need to truncate as that happens under the hood anyway.
                                                    Log.Add(
                                                        Resources.SqlType_GetClrToSqlConverter_StringTruncated,
                                                        LogLevel.Warning,
                                                        c.Length,
                                                        this.FullName,
                                                        this.Size.MaximumLength);
                                                }

                                                // Check for non ASCII chars
                                                if (c.Any(ch => ch > 255))
                                                {
                                                    if (m == TypeConstraintMode.Error)
                                                        throw new DatabaseSchemaException(
                                                            Resources.SqlType_GetClrToSqlConverter_StringContainsUnicodeCharacters,
                                                            LogLevel.Error,
                                                            this.FullName);

                                                    // Warn - don't need to do anything as that happens under the hood anyway.
                                                    Log.Add(
                                                        Resources.SqlType_GetClrToSqlConverter_UnicodeCharactersLost,
                                                        LogLevel.Warning,
                                                        this.FullName);
                                                }
                                            }
                                            return new SqlString(c);
                                        });
                            case SqlDbType.NChar:
                            case SqlDbType.NText:
                            case SqlDbType.NVarChar:
                                return CreateConvertor<string>(
                                    t,
                                    (c, m) =>
                                    {
                                        if (c == null)
                                            return SqlString.Null;
                                        // SQL reports size in bytes rather than characters - so divide by 2.
                                        int sqlSize = this.Size.MaximumLength/2;
                                        if ((m != TypeConstraintMode.Silent) &&
                                            (this.Size.MaximumLength > -1) &&
                                            (c.Length > this.Size.MaximumLength / 2))
                                        {
                                            if (m == TypeConstraintMode.Error)
                                                throw new DatabaseSchemaException(
                                                    Resources.SqlType_GetClrToSqlConverter_CouldNotConvertString,
                                                    LogLevel.Error,
                                                    c.Length,
                                                    this.FullName,
                                                    sqlSize);

                                            // Warn - don't need to truncate as that happens under the hood anyway.
                                            Log.Add(
                                                Resources.SqlType_GetClrToSqlConverter_StringTruncated,
                                                LogLevel.Warning,
                                                c.Length,
                                                this.FullName,
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
                                       clrType.GetGenericTypeDefinition() == typeof (Nullable<>)
                                           ? CreateConvertor<DateTime?>(
                                               t,
                                               (c, m) =>
                                                   {
                                                       if (c == null)
                                                           return DBNull.Value;

                                                       DateTime original = (DateTime) c;
                                                       DateTimeRange range = DateTypeSizes[SqlDbType];
                                                       DateTime bound = range.Bind(original);

                                                       if ((m != TypeConstraintMode.Silent) &&
                                                           (original != bound))
                                                       {
                                                           if (m == TypeConstraintMode.Error)
                                                               throw new DatabaseSchemaException(
                                                                   Resources.SqlType_GetClrToSqlConverter_CouldNotConvertDateTimeToSqlType,
                                                                   LogLevel.Error,
                                                                   original,
                                                                   this.FullName,
                                                                   range);

                                                           // Warn - don't need to truncate as that happens under the hood anyway.
                                                           Log.Add(
                                                               Resources.SqlType_GetClrToSqlConverter_DateTimeTruncated,
                                                               LogLevel.Warning,
                                                               original,
                                                               this.FullName,
                                                               range);
                                                       }
                                                       return bound;
                                                   })
                                           : CreateConvertor<DateTime>(
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
                                                                   Resources.SqlType_GetClrToSqlConverter_CouldNotConvertDateTimeToSqlType,
                                                                   LogLevel.Error,
                                                                   c,
                                                                   this.FullName,
                                                                   range);

                                                           // Warn - don't need to truncate as that happens under the hood anyway.
                                                           Log.Add(
                                                               Resources.SqlType_GetClrToSqlConverter_DateTimeTruncated,
                                                               LogLevel.Warning,
                                                               c,
                                                               this.FullName,
                                                               range);
                                                       }
                                                       return bound;
                                                   });
                            case SqlDbType.Time:
                                return CreateConvertor<TimeSpan>(t);
                            case SqlDbType.Decimal:
                                return CreateConvertor<decimal>(t);
                            case SqlDbType.Float:
                                return CreateConvertor<double>(t);
                            case SqlDbType.Int:
                                return CreateConvertor<int>(t);
                            case SqlDbType.Money:
                            case SqlDbType.SmallMoney:
                                return CreateConvertor<decimal>(t);
                            case SqlDbType.Real:
                                return CreateConvertor<float>(t);
                            case SqlDbType.UniqueIdentifier:
                                return CreateConvertor<Guid>(t);
                            case SqlDbType.SmallInt:
                                return CreateConvertor<short>(t);
                            case SqlDbType.TinyInt:
                                return CreateConvertor<byte>(t);
                            case SqlDbType.Variant:
                                // TODO Variant type can't accept everything!
                                return (c, m) => c;
                            case SqlDbType.Xml:
                                return
                                    CreateConvertor<XNode>(
                                        t,
                                        (c, m) =>
                                            {
                                                if (c == null)
                                                    return (object) SqlXml.Null;
                                                using (XmlReader xmlNodeReader = c.CreateReader())
                                                    return new SqlXml(xmlNodeReader);
                                            })
                                    ??
                                    CreateConvertor<XmlNode>(
                                        t,
                                        (c, m) =>
                                            {
                                                if (c == null)
                                                    return (object) SqlXml.Null;
                                                using (XmlNodeReader xmlNodeReader = new XmlNodeReader(c))
                                                    return new SqlXml(xmlNodeReader);
                                            });
                            case SqlDbType.Udt:
                                // Add UDT conversions (which depend on the type name).
                                switch (Name)
                                {
                                    case "geography":
                                        return CreateConvertor<SqlGeography>(t, false);
                                    case "geometry":
                                        return CreateConvertor<SqlGeometry>(t, false);
                                    case "hierarchyid":
                                        return CreateConvertor<SqlHierarchyId>(t, false);
                                    default:
                                        // Log error, but don't throw it.
                                        new DatabaseSchemaException(
                                            Resources.SqlType_GetClrToSqlConverter_UdtTypeNotSupported,
                                            LogLevel.Critical, Name);
                                        return null;
                                }
                            case SqlDbType.Structured:
                                // This is actually a SqlTableType.
                                SqlTableType tableType = this as SqlTableType;
                                if (tableType == null)
                                {
                                    // Log error, but don't throw it.
                                    new DatabaseSchemaException(
                                        Resources.SqlType_GetClrToSqlConverter_NotCreatedAsSqlTableType,
                                        LogLevel.Critical, this);
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
                                {
                                    if (!columnSqlTypes[minColumns-1].IsNullable)
                                        break;
                                }

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
                                    {
                                        // We are the base non-generic table type.
                                        return (Func<object, TypeConstraintMode, object>)
                                               ((c, m) =>
                                                    {
                                                        if (c == null)
                                                            return null;
                                                        List<SqlDataRecord> records =
                                                            ((IEnumerable<SqlDataRecord>) c).ToList();
                                                        return records.Count < 1 ? null : records;
                                                    });
                                    }
                                    

                                    // Check if we are IEnumerable<Tuple<...>>
                                    if (enumerationType.IsGenericType &&
                                        enumerationType.GetInterfaces().Any(i => i.FullName == "System.ITuple"))
                                    {
                                        // Get convertors for tuple elements
                                        Type[] tupleTypes = enumerationType.GetIndexTypes();
                                        int items = tupleTypes.Length;

                                        // Check we don't have too many items in the tuple.
                                        if (items > columns)
                                        {
                                            // Log error, but don't throw it.
                                            new DatabaseSchemaException(
                                                Resources.SqlType_GetClrToSqlConverter_ColumnNumberAndTupleSizeMismatch,
                                                LogLevel.Critical, this, enumerationType.Name, columns,
                                                items);
                                            return null;
                                        }

                                        // Check we don't have too few!
                                        if (items < minColumns)
                                        {
                                            // Log error, but don't throw it.
                                            new DatabaseSchemaException(
                                                Resources.SqlType_GetClrToSqlConverter_ColumnNumberAndTupleSizeMismatch,
                                                LogLevel.Critical, this, enumerationType.Name, columns,
                                                items);
                                            return null;
                                        }

                                        // Get the tuple indexer (note this supports extended/nested tuples).
                                        Func<object, int, object> indexer = enumerationType.GetTupleIndexer();

                                        Func<object, TypeConstraintMode, object>[] convertors =
                                            new Func<object, TypeConstraintMode, object>[tupleTypes.Length];
                                        for (int i = 0; i < tupleTypes.Length; i++)
                                        {
                                            Func<object, TypeConstraintMode, object> columnConvertor = columnSqlTypes[i].GetClrToSqlConvertor(tupleTypes[i]);
                                            if (columnConvertor == null)
                                            {
                                                // Log error, but don't throw it.
                                                new DatabaseSchemaException(
                                                    Resources.SqlType_GetClrToSqlConverter_CanNotCast,
                                                    LogLevel.Critical, this, enumerationType.Name, tupleTypes[i],
                                                    columnSqlTypes[i], i);
                                                return null;
                                            }

                                            // TODO A direct expression tree would probably be quicker... need to performance test.
                                            int cindex = i;
                                            convertors[i] = (o, m) => columnConvertor(indexer(o, cindex), m);
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
                                                                record.SetValue(i, convertors[i](o, m));
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
                                        // Get convertor for column type
                                        SqlType columnType = columnSqlTypes[0];
                                        Func<object, TypeConstraintMode, object> enumConvertor =
                                            columnType.GetClrToSqlConvertor(enumerationType);
                                        if (enumConvertor != null)
                                        {
                                        // Create lambda
                                            return (Func<object, TypeConstraintMode, object>)
                                                   ((c,m) =>
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
                                                                record.SetValue(0, enumConvertor(o, m));
                                                                records.Add(record);
                                                            }

                                                            // If we have zero count return DBNull (never return empty enumeration!)
                                                            return records.Count < 1
                                                                       ? (object)null
                                                                       : records;
                                                        });
                                        }
                                    }

                                    // If we have more than 1 column, but we need less than 3, then a keyvaluepair is fine.
                                    if ((minColumns < 3) && (columns > 1) &&
                                        (enumerationType.IsGenericType &&
                                            enumerationType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>)))
                                    {
                                        // Get convertor for key and value column types
                                        Type[] kvpTypes = enumerationType.GetGenericArguments();

                                        SqlType keyColumnType = columnSqlTypes[0];
                                        SqlType valueColumnType = columnSqlTypes[1];
                                        Func<object, TypeConstraintMode, object> keyConvertor =
                                            keyColumnType.GetClrToSqlConvertor(kvpTypes[0]);
                                        if (keyConvertor == null)
                                        {
                                            // Log error, but don't throw it.
                                            new DatabaseSchemaException(
                                                Resources.SqlType_GetClrToSqlConverter_CanNotConvertKeyType,
                                                LogLevel.Critical, this, enumerationType.Name, kvpTypes[0],
                                                keyColumnType);
                                            return null;
                                        }

                                        // Create key selector
                                        Func<object, object> keySelector =
                                            (Func<object, object>)
                                            enumerationType.GetGetter("Key", typeof (object), typeof (object));
                                        Func<object, TypeConstraintMode, object> k =
                                            (o, m) => keyConvertor(keySelector(o), m);

                                        Func<object, TypeConstraintMode, object> valueConvertor =
                                            valueColumnType.GetClrToSqlConvertor(kvpTypes[1]);
                                        if (valueConvertor == null)
                                        {
                                            // Log error, but don't throw it.
                                            new DatabaseSchemaException(
                                                Resources.SqlType_GetClrToSqlConverter_CannotAcceptEnumerationType,
                                                LogLevel.Critical, this, enumerationType.Name);
                                            return null;
                                        }
                                        Func<object, object> valueSelector =
                                            (Func<object, object>)
                                            enumerationType.GetGetter("Value", typeof (object), typeof (object));
                                        Func<object, TypeConstraintMode, object> v =
                                            (o, m) => valueConvertor(valueSelector(o), m);


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
                                        Resources.SqlType_GetClrToSqlConverter_CannotAcceptEnumerationType,
                                        LogLevel.Critical, this, enumerationType);
                                    return null;
                                }

                                // Unsupported Log error, but don't throw it.
                                new DatabaseSchemaException(
                                    Resources.SqlType_GetClrToSqlConverter_CannotAcceptType,
                                    LogLevel.Critical, this, t);
                                return null;
                            default:
                                throw new DatabaseSchemaException(Resources.SqlType_GetClrToSqlConverter_UnsupportedSqlDbType, LogLevel.Critical,
                                                                  SqlDbType);
                        }
                    }
                    catch (Exception e)
                    {
                        new DatabaseSchemaException(e,
                                                    Resources.SqlType_GetClrToSqlConverter_FatalErrorOccurred,
                                                    LogLevel.Critical,
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
        ///   Returns a convertor that casts from the input type to the output type
        ///   (if the input type is assignable to the output type).
        /// </summary>
        /// <typeparam name="TClr">The CLR type to convert to.</typeparam>
        /// <param name="actualClrType">The CLR type to convert from.</param>
        /// <param name="supportNullable">
        ///   <para>If set to <see langword="true"/> then supports <see langword="null"/>.</para>
        ///   <para>By default this is set to <see langword="true"/>.</para>
        /// </param>
        /// <returns>
        ///   The created convertor; or <see langword="null"/> if the input type is not assignable to the required input type.
        /// </returns>
        private static Func<object, TypeConstraintMode, object> CreateConvertor<TClr>([NotNull]Type actualClrType, bool supportNullable = true)
        {
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
        ///   Returns a convertor that casts from the input type to the output type.
        ///   (if the input type is assignable to the output type).
        /// </summary>
        /// <typeparam name="TClr">The CLR type to convert to.</typeparam>
        /// <param name="actualClrType">The CLR type to convert from.</param>
        /// <param name="convertor">The type convertor.</param>
        /// <returns>
        ///   The created  convertor; or <see langword="null"/> if the input type is not assignable to the required input type.
        /// </returns>
        private static Func<object, TypeConstraintMode, object> CreateConvertor<TClr>([NotNull]Type actualClrType, [NotNull]Func<TClr, TypeConstraintMode, object> convertor)
        {
            Func<object, TClr> toInputType = actualClrType.GetConversion<object, TClr>();
            return toInputType != null
                       ? (Func<object, TypeConstraintMode, object>)((c, m) => convertor(toInputType(c), m))
                       : null;
        }

        /// <summary>
        ///   Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        ///   A <see cref="string"/> representation of this instance.
        /// </returns>
        public override string ToString()
        {
            return FullName;
        }
    }
}