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
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Xml;
using System.Xml.Linq;
using Microsoft.SqlServer.Server;
using Microsoft.SqlServer.Types;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Database.Exceptions;
using WebApplications.Utilities.Logging;
using WebApplications.Utilities.Ranges;
using WebApplications.Utilities.Serialization;

namespace WebApplications.Utilities.Database.Schema
{
    /// <summary>
    ///   Holds information about a type.
    /// </summary>
    [PublicAPI]
    public class SqlType : DatabaseSchemaEntity<SqlType>
    {
        private enum SystemType
        {
            VarBinary        = SqlDbType.VarBinary        | TypeKind.Binary << 8   | (0 << 16),
            Binary           = SqlDbType.Binary           | TypeKind.Binary << 8   | (1 << 16),
            Image            = SqlDbType.Image            | TypeKind.Binary << 8   | (2 << 16),
            Timestamp        = SqlDbType.Timestamp        | TypeKind.Binary << 8   | (3 << 16),
            DateTime2        = SqlDbType.DateTime2        | TypeKind.DateTime << 8 | (0 << 16),
            DateTimeOffset   = SqlDbType.DateTimeOffset   | TypeKind.DateTime << 8 | (1 << 16),
            DateTime         = SqlDbType.DateTime         | TypeKind.DateTime << 8 | (2 << 16),
            SmallDateTime    = SqlDbType.SmallDateTime    | TypeKind.DateTime << 8 | (3 << 16),
            Date             = SqlDbType.Date             | TypeKind.DateTime << 8 | (4 << 16),
            Time             = SqlDbType.Time             | TypeKind.DateTime << 8 | (4 << 16),
            BigInt           = SqlDbType.BigInt           | TypeKind.Number << 8   | (0 << 16),
            Decimal          = SqlDbType.Decimal          | TypeKind.Number << 8   | (0 << 16),
            Float            = SqlDbType.Float            | TypeKind.Number << 8   | (0 << 16),
            Int              = SqlDbType.Int              | TypeKind.Number << 8   | (1 << 16),
            Money            = SqlDbType.Money            | TypeKind.Number << 8   | (1 << 16),
            Real             = SqlDbType.Real             | TypeKind.Number << 8   | (1 << 16),
            SmallInt         = SqlDbType.SmallInt         | TypeKind.Number << 8   | (2 << 16),
            SmallMoney       = SqlDbType.SmallMoney       | TypeKind.Number << 8   | (2 << 16),
            TinyInt          = SqlDbType.TinyInt          | TypeKind.Number << 8   | (3 << 16),
            Udt              = SqlDbType.Udt              | TypeKind.Object << 8   | (0 << 16),
            Bit              = SqlDbType.Bit              | TypeKind.Object << 8   | (1 << 16),
            UniqueIdentifier = SqlDbType.UniqueIdentifier | TypeKind.Object << 8   | (1 << 16),
            Xml              = SqlDbType.Xml              | TypeKind.Object << 8   | (1 << 16),
            NVarChar         = SqlDbType.NVarChar         | TypeKind.String << 8   | (0 << 16),
            VarChar          = SqlDbType.VarChar          | TypeKind.String << 8   | (1 << 16),
            NChar            = SqlDbType.NChar            | TypeKind.String << 8   | (2 << 16),
            Char             = SqlDbType.Char             | TypeKind.String << 8   | (3 << 16),
            NText            = SqlDbType.NText            | TypeKind.String << 8   | (4 << 16),
            Text             = SqlDbType.Text             | TypeKind.String << 8   | (5 << 16),
            Structured       = SqlDbType.Structured       | TypeKind.Table << 8    | (0 << 16),
            Variant          = SqlDbType.Variant          | TypeKind.Variant << 8  | (0 << 16),

            SqlDbTypeMask = 255,
            TypeKindMask = 255 << 8,
            PrecedenceMask = 255 << 16
        }

        internal enum TypeKind : byte
        {
            Variant,
            Binary,
            String,
            Number,
            DateTime,
            Object,
            Table
        }

        /// <summary>
        /// The properties used for calculating differences.
        /// </summary>
        [UsedImplicitly]
        [NotNull]
        private static readonly Expression<Func<SqlType, object>>[] _properties =
        {
            t => t.IsCLR,
            t => t.IsNullable,
            t => t.IsUserDefined,
            t => t.IsTable,
            t => t.Size,
            t => t.BaseType,
            t => t.FullName
        };

        /// <summary>
        ///   Valid date ranges for date types.
        /// </summary>
        [NotNull]
        public static readonly Dictionary<SqlDbType, DateTimeRange> DateTypeSizes =
            new Dictionary<SqlDbType, DateTimeRange>
            {
                { SqlDbType.Date, new DateTimeRange(new DateTime(1, 1, 1), new DateTime(9999, 12, 31)) },
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
        private static readonly Dictionary<string, SystemType> _systemTypes =
            new Dictionary<string, SystemType>(StringComparer.InvariantCultureIgnoreCase)
            {
                { "bigint", SystemType.BigInt },
                { "binary", SystemType.Binary },
                { "bit", SystemType.Bit },
                { "char", SystemType.Char },
                { "date", SystemType.Date },
                { "datetime", SystemType.DateTime },
                { "datetime2", SystemType.DateTime2 },
                { "datetimeoffset", SystemType.DateTimeOffset },
                { "decimal", SystemType.Decimal },
                { "float", SystemType.Float },
                { "geography", SystemType.Udt },
                { "geometry", SystemType.Udt },
                { "hierarchyid", SystemType.Udt },
                { "image", SystemType.Image },
                { "int", SystemType.Int },
                { "money", SystemType.Money },
                { "nchar", SystemType.NChar },
                { "ntext", SystemType.NText },
                { "numeric", SystemType.Decimal },
                { "nvarchar", SystemType.NVarChar },
                { "real", SystemType.Real },
                { "smalldatetime", SystemType.SmallDateTime },
                { "smallint", SystemType.SmallInt },
                { "smallmoney", SystemType.SmallMoney },
                { "sql_variant", SystemType.Variant },
                { "sysname", SystemType.NVarChar },
                { "text", SystemType.Text },
                { "time", SystemType.Time },
                { "timestamp", SystemType.Timestamp },
                { "tinyint", SystemType.TinyInt },
                { "uniqueidentifier", SystemType.UniqueIdentifier },
                { "varbinary", SystemType.VarBinary },
                { "varchar", SystemType.VarChar },
                { "xml", SystemType.Xml }
            };

        /// <summary>
        /// Holds the name of the SQL type to use for built in CLR types.
        /// </summary>
        [NotNull]
        private static readonly Dictionary<Type, string> _clrTypes = new Dictionary<Type, string>
        {
            { typeof(SqlBinary), "varbinary" },
            { typeof(SqlBytes), "varbinary" },
            { typeof(SqlBoolean), "bit" },
            { typeof(SqlByte), "tinyint" },
            { typeof(SqlChars), "nvarchar" },
            { typeof(SqlString), "nvarchar" },
            { typeof(SqlDateTime), "datetime2" },
            { typeof(SqlDecimal), "decimal" },
            { typeof(SqlDouble), "float" },
            { typeof(SqlGuid), "uniqueidentifier" },
            { typeof(SqlInt16), "smallint" },
            { typeof(SqlInt32), "int" },
            { typeof(SqlInt64), "bigint" },
            { typeof(SqlMoney), "money" },
            { typeof(SqlSingle), "real" },
            { typeof(SqlXml), "xml" },

            { typeof(byte[]), "varbinary" },
            { typeof(bool), "bit" },
            { typeof(byte), "tinyint" },
            { typeof(Guid), "uniqueidentifier" },
            { typeof(string), "nvarchar" },
            { typeof(char), "nvarchar" },
            { typeof(char[]), "nvarchar" },
            { typeof(TimeSpan), "time" },
            { typeof(DateTimeOffset), "datetimeoffset" },
            { typeof(Stream), "varbinary" },
            { typeof(TextReader), "nvarchar" },
            { typeof(XmlReader), "xml" },
            { typeof(short), "smallint" },
            { typeof(int), "int" },
            { typeof(long), "bigint" },
            { typeof(float), "real" },
            { typeof(double), "float" },
            { typeof(decimal), "decimal" },
            { typeof(DateTime), "datetime2" },

            { typeof(SqlGeography), "geography" },
            { typeof(SqlGeometry), "geometry" },
            { typeof(SqlHierarchyId), "hierarchyid" },
        };

        /// <summary>
        /// Attempts to get the SQL type name for the CLR type given.
        /// </summary>
        /// <param name="type">The CLR type.</param>
        /// <param name="typeName">The name of the SQL type, if found.</param>
        /// <returns></returns>
        internal static bool TryGetSqlTypeName(Type type, out string typeName)
            => _clrTypes.TryGetValue(type, out typeName);

        /// <summary>
        ///   The base type (if any).
        /// </summary>
        [CanBeNull]
        public readonly SqlType BaseType;

        /// <summary>
        ///   A <see cref="bool"/> value indicating whether this is a CLR type.
        /// </summary>
        public readonly bool IsCLR;

        /// <summary>
        ///   A <see cref="bool"/> value indicating whether the type accepts nulls.
        /// </summary>
        public readonly bool IsNullable;

        /// <summary>
        ///   A <see cref="bool"/> value indicating whether this type is a table.
        /// </summary>
        public readonly bool IsTable;

        /// <summary>
        ///   A <see cref="bool"/> value indicating whether this is a user defined type.
        /// </summary>
        public readonly bool IsUserDefined;

        /// <summary>
        ///   The type name.
        /// </summary>
        [NotNull]
        public readonly string Name;

        /// <summary>
        ///   The <see cref="SqlTypeSize">size</see> of the type.
        /// </summary>
        public readonly SqlTypeSize Size;

        private readonly SystemType _sysType;

        /// <summary>
        ///   Gets the corresponding <see cref="SqlDbType"/>.
        /// </summary>
        /// <value>The corresponding <see cref="SqlDbType"/>.</value>
        public SqlDbType SqlDbType => (SqlDbType)(_sysType & SystemType.SqlDbTypeMask);

        /// <summary>
        /// Gets the kind of the type (string, binary, etc).
        /// </summary>
        internal TypeKind Kind => (TypeKind)((int)(_sysType & SystemType.TypeKindMask) >> 8);

        /// <summary>
        /// Gets the precedence of the type within its <see cref="Kind"/>.
        /// </summary>
        /// <remarks>For table types, the precedence is based on the number of columns in the table.</remarks>
        internal int Precedence => IsTable
            ? ((SqlTableType)this).TableDefinition.ColumnCount
            : ((int)(_sysType & SystemType.PrecedenceMask) >> 16);

        /// <summary>
        ///   The CLR type converters for this type.
        /// </summary>
        [NotNull]
        private readonly ConcurrentDictionary<Type, Func<object, TypeConstraintMode, object>> _clrTypeConverters
            = new ConcurrentDictionary<Type, Func<object, TypeConstraintMode, object>>();

        /// <summary>
        ///   The SQL type converters for this type.
        /// </summary>
        [NotNull]
        private readonly ConcurrentDictionary<Type, Func<object, TypeConstraintMode, object>> _sqlTypeConverters
            = new ConcurrentDictionary<Type, Func<object, TypeConstraintMode, object>>();

        /// <summary>
        /// Whether this SQL type accepts the CLR type.
        /// </summary>
        [NotNull]
        private readonly ConcurrentDictionary<Type, bool> _acceptsType = new ConcurrentDictionary<Type, bool>();
        
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
            Name = name;
            IsTable = isTable;
            IsCLR = isClr;
            IsUserDefined = isUserDefined;
            IsNullable = isNullable;
            Size = size;
            BaseType = baseType;

            // Calculate our underlying SqlDbType (represented by SystemType)
            if (!IsUserDefined)
            {
                // We are a system type, look up.
                if (_systemTypes.TryGetValue(Name, out SystemType sysType))
                    _sysType = sysType;
                else if (BaseType != null)
                    // Get the base type
                    _sysType = BaseType._sysType;
                else
                {
                    // Log error and return NVarChar as fall back.
                    // ReSharper disable once ObjectCreationAsStatement
                    new DatabaseSchemaException(
                        LoggingLevel.Warning,
                        () => Resources.SqlType_UnknownSqlSystemType,
                        FullName);
                    _sysType = SystemType.NVarChar;
                }
            }
            else if (IsTable)
                _sysType = SystemType.Structured;
            else if (BaseType != null)
                // Get the base type
                _sysType = BaseType._sysType;
            else
            {
                // Log error and return NVarChar as fall back.
                // ReSharper disable once ObjectCreationAsStatement
                new DatabaseSchemaException(
                    LoggingLevel.Critical,
                    () => Resources.SqlType_UnknownSqlSystemType,
                    FullName);
                _sysType = SystemType.NVarChar;
            }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SqlType"/> class.
        /// </summary>
        /// <param name="baseType">The base type.</param>
        /// <param name="size">The size information.</param>
        internal SqlType([NotNull] SqlType baseType, SqlTypeSize size)
            : base(baseType.SqlSchema, baseType.Name)
        {
            Name = baseType.Name;
            IsTable = baseType.IsTable;
            IsCLR = baseType.IsCLR;
            IsUserDefined = baseType.IsUserDefined;
            IsNullable = baseType.IsNullable;
            BaseType = baseType;
            Size = size;
            _sysType = baseType._sysType;
        }

        /* TODO AcceptsCLRType and the converter methods should mirror to/from SQL checking
         *  For example, you might be able to convert a type to a byte[], but not from a byte[]
         */

        /// <summary>
        ///   Checks to see whether the specified CLR type can be used to represent the current <see cref="SqlType"/>.
        /// </summary>
        /// <param name="clrType">The CLR type to check.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the CLR type is acceptable; otherwise returns <see langword="false"/>.
        /// </returns>
        /// <remarks>
        ///   There is a <paramref name="clrType"/> cannot be <see langword="null"/>.
        /// </remarks>
        public bool AcceptsCLRType([NotNull] Type clrType)
        {
            if (clrType == null) throw new ArgumentNullException("clrType");

            Func<object, TypeConstraintMode, object> temp;
            if (_clrTypeConverters.TryGetValue(clrType, out temp))
                return temp != null;

            return _acceptsType.GetOrAdd(
                clrType,
                type =>
                {
                    Debug.Assert(type != null);
                    try
                    {
                        // Check for standard conversions first
                        switch (SqlDbType)
                        {
                            case SqlDbType.BigInt:
                                return type.GetNonNullableType().CanConvertTo(typeof(long));
                            case SqlDbType.Image:
                            case SqlDbType.Binary:
                            case SqlDbType.Timestamp:
                            case SqlDbType.VarBinary:
                                // If we have a byte[] (or can cast to byte[]) then use that.
                                if (type.CanConvertTo(typeof(byte[])))
                                    return true;

                                // Support serializable objects.
                                return type.IsSerializable;

                            case SqlDbType.Bit:
                                return type.GetNonNullableType().CanConvertTo(typeof(bool));
                            case SqlDbType.Text:
                            case SqlDbType.Char:
                            case SqlDbType.VarChar:
                                return type.CanConvertTo(typeof(string));
                            case SqlDbType.NChar:
                            case SqlDbType.NText:
                            case SqlDbType.NVarChar:
                                return type.CanConvertTo(typeof(string));
                            case SqlDbType.SmallDateTime:
                            case SqlDbType.DateTime:
                            case SqlDbType.Date:
                            case SqlDbType.DateTime2:
                            case SqlDbType.DateTimeOffset:
                                return type.GetNonNullableType().CanConvertTo(typeof(DateTime));
                            case SqlDbType.Time:
                                return type.GetNonNullableType().CanConvertTo(typeof(TimeSpan));
                            case SqlDbType.Decimal:
                                return type.GetNonNullableType().CanConvertTo(typeof(decimal));
                            case SqlDbType.Float:
                                return type.GetNonNullableType().CanConvertTo(typeof(double));
                            case SqlDbType.Int:
                                return type.GetNonNullableType().CanConvertTo(typeof(int));
                            case SqlDbType.Money:
                            case SqlDbType.SmallMoney:
                                return type.GetNonNullableType().CanConvertTo(typeof(decimal));
                            case SqlDbType.Real:
                                return type.GetNonNullableType().CanConvertTo(typeof(float));
                            case SqlDbType.UniqueIdentifier:
                                return type.GetNonNullableType().CanConvertTo(typeof(Guid));
                            case SqlDbType.SmallInt:
                                return type.GetNonNullableType().CanConvertTo(typeof(short));
                            case SqlDbType.TinyInt:
                                return type.GetNonNullableType().CanConvertTo(typeof(byte));
                            case SqlDbType.Variant:
                                // TODO Variant type can't accept everything!
                                return true;
                            case SqlDbType.Xml:
                                return type.CanConvertTo(typeof(XNode)) || type.CanConvertTo(typeof(XmlNode));
                            case SqlDbType.Udt:
                                // Add UDT conversions (which depend on the type name).
                                switch (Name)
                                {
                                    case "geography":
                                        return type.CanConvertTo(typeof(SqlGeography));
                                    case "geometry":
                                        return type.CanConvertTo(typeof(SqlGeometry));
                                    case "hierarchyid":
                                        return type.CanConvertTo(typeof(SqlHierarchyId));
                                    default:
                                        return false;
                                }
                            case SqlDbType.Structured:
                                // This is actually a SqlTableType.
                                SqlTableType tableType = this as SqlTableType;
                                if (tableType == null)
                                    return false;

                                SqlType[] columnSqlTypes =
                                    tableType.TableDefinition.Columns.Select(c => c.Type).ToArray();
                                int columns = columnSqlTypes.Length;

                                // Find out last non-nullable column to determine the minimum number of columns that must
                                // be supplied - as columns are unnamed on the input (we only have the type to go by), the we
                                // cannot 'skip' columns, and all non-null columns must be supplied.
                                int minColumns;
                                for (minColumns = columns; minColumns > 1; minColumns--)
                                    // ReSharper disable once PossibleNullReferenceException
                                    if (!columnSqlTypes[minColumns - 1].IsNullable)
                                        break;

                                // Check if type implements IEnumerable<T>
                                Type enumerableInterface =
                                    type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                                        ? type
                                        : type.GetInterfaces().FirstOrDefault(
                                            // ReSharper disable once PossibleNullReferenceException
                                            it =>
                                                it.IsGenericType &&
                                                it.GetGenericTypeDefinition() == typeof(IEnumerable<>));

                                if (enumerableInterface != null)
                                {
                                    // Get the enumeration type
                                    Type enumerationType = enumerableInterface.GetGenericArguments().First();
                                    Debug.Assert(enumerationType != null);

                                    // If the type passed in is assignable from IEnumerable<SqlDataRecord> then we can pass straight through
                                    // after checking there are rows available.
                                    if (typeof(SqlDataRecord).IsAssignableFrom(enumerationType))
                                        // We are the base non-generic table type.
                                        return true;

                                    // Check if we are IEnumerable<Tuple<...>>
                                    // ReSharper disable once PossibleNullReferenceException
                                    if (enumerationType.IsGenericType &&
                                        enumerationType.FullName.StartsWith("System.Tuple`"))
                                    {
                                        // Get converters for tuple elements
                                        Type[] tupleTypes = enumerationType.GetIndexTypes();
                                        int items = tupleTypes.Length;

                                        // Check we don't have too many items in the tuple.
                                        if (items > columns)
                                            return false;

                                        // Check we don't have too few!
                                        if (items < minColumns)
                                            return false;

                                        for (int i = 0; i < tupleTypes.Length; i++)
                                        {
                                            Debug.Assert(columnSqlTypes[i] != null);
                                            Debug.Assert(tupleTypes[i] != null);
                                            if (!columnSqlTypes[i].AcceptsCLRType(tupleTypes[i]))
                                                return false;
                                        }

                                        // Create lambda
                                        return true;
                                    }

                                    // If we're single column we support enumeration of column type
                                    if (minColumns < 2)
                                    {
                                        // Get converter for column type
                                        Debug.Assert(columnSqlTypes[0] != null);
                                        if (columnSqlTypes[0].AcceptsCLRType(enumerationType))
                                            return true;
                                    }

                                    // If we have more than 1 column, but we need less than 3, then a keyvaluepair is fine.
                                    if ((minColumns < 3) &&
                                        (columns > 1) &&
                                        (enumerationType.IsGenericType &&
                                         enumerationType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>)))
                                    {
                                        // Get converter for key and value column types
                                        Type[] kvpTypes = enumerationType.GetGenericArguments();

                                        Debug.Assert(columnSqlTypes[0] != null);
                                        Debug.Assert(kvpTypes[0] != null);
                                        if (!columnSqlTypes[0].AcceptsCLRType(kvpTypes[0]))
                                            return false;

                                        Debug.Assert(columnSqlTypes[1] != null);
                                        Debug.Assert(kvpTypes[1] != null);
                                        return columnSqlTypes[1].AcceptsCLRType(kvpTypes[1]);
                                    }

                                    return false;
                                }

                                return false;
                            default:
                                return false;
                        }
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                });
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
            object value,
            [NotNull] Type clrType,
            TypeConstraintMode mode = TypeConstraintMode.Warn)
        {
            if (clrType == null) throw new ArgumentNullException(nameof(clrType));
            Func<object, TypeConstraintMode, object> converter = GetClrToSqlConverter(clrType);
            return converter != null ? converter(value, mode) : value;
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
        public object CastCLRValue<T>(T value, TypeConstraintMode mode = TypeConstraintMode.Warn)
        {
            Func<object, TypeConstraintMode, object> converter = GetClrToSqlConverter(typeof(T));
            return converter != null ? converter(value, mode) : value;
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
        public object CastSQLValue(
            object value,
            [NotNull] Type clrType,
            TypeConstraintMode mode = TypeConstraintMode.Warn)
        {
            if (clrType == null) throw new ArgumentNullException(nameof(clrType));
            Func<object, TypeConstraintMode, object> converter = GetSqlToClrConverter(clrType);
            return converter != null ? converter(value, mode) : value;
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
            Func<object, TypeConstraintMode, object> converter = GetSqlToClrConverter(typeof(T));
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
        public Func<object, TypeConstraintMode, object> GetClrToSqlConverter<T>() => GetClrToSqlConverter(typeof(T));

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
        /// <exception cref="ArgumentNullException"><paramref name="clrType"/> is <see langword="null" />.</exception>
        [CanBeNull]
        public Func<object, TypeConstraintMode, object> GetClrToSqlConverter([NotNull] Type clrType)
        {
            if (clrType == null) throw new ArgumentNullException(nameof(clrType));

            Func<object, TypeConstraintMode, object> conv = _clrTypeConverters.GetOrAdd(
                clrType,
                t =>
                {
                    // Should never happen, but if it does there is not conversion from a null type!
                    if (t == null)
                    {
                        // Log error, but don't throw it.
                        // ReSharper disable once ObjectCreationAsStatement
                        new DatabaseSchemaException(
                            LoggingLevel.Critical,
                            () => Resources.SqlType_GetClrToSqlConverter_NoTypeSpecified,
                            this);
                        return null;
                    }

                    // Check if we have already determined if this type is not accepted
                    bool acceptsType;
                    if (_acceptsType.TryGetValue(t, out acceptsType) && !acceptsType)
                        return null;

                    try
                    {
                        // Check for standard conversions first
                        switch (SqlDbType)
                        {
                            case SqlDbType.BigInt:
                                return CreateConverterTo<long>(t);
                            case SqlDbType.Image:
                            case SqlDbType.Binary:
                            case SqlDbType.Timestamp:
                            case SqlDbType.VarBinary:
                                // If we have a byte[] (or can cast to byte[]) then use that.
                                Func<object, TypeConstraintMode, object> converter = CreateConverterTo<byte[]>(
                                    t,
                                    (c, m) =>
                                    {
                                        if (c == null)
                                            return SqlBinary.Null;
                                        // Check for truncation.
                                        // Note: the Image type cannot have a max length (and Size.MaximumLength will always be 16), so don't check
                                        if ((SqlDbType != SqlDbType.Image) &&
                                            (m != TypeConstraintMode.Silent) &&
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
                                            // NOTE We always fail regardless of truncation mode as a partially serialized object is junk.
                                            // Note: the Image type cannot have a max length (and Size.MaximumLength will always be 16), so don't check
                                            if ((SqlDbType != SqlDbType.Image) &&
                                                (Size.MaximumLength > -1) &&
                                                (Size.MaximumLength < serializedObject.Length))
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
                                return CreateConverterTo<bool>(t);
                            case SqlDbType.Text:
                            case SqlDbType.Char:
                            case SqlDbType.VarChar:
                                return CreateConverterTo<string>(
                                    t,
                                    (c, m) =>
                                    {
                                        if (c == null)
                                            return SqlString.Null;
                                        if (m != TypeConstraintMode.Silent)
                                        {
                                            // Check for truncation
                                            // Note: the Text type cannot have a max length (and Size.MaximumLength will always be 16), so don't check
                                            if ((SqlDbType != SqlDbType.Text) &&
                                                (Size.MaximumLength > -1) &&
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
                                            // BUG Non-unicode strings are NOT ASCII, depends on the collation
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
                                return CreateConverterTo<string>(
                                    t,
                                    (c, m) =>
                                    {
                                        if (c == null)
                                            return SqlString.Null;
                                        // SQL reports size in bytes rather than characters - so divide by 2.
                                        // Note: the NText type cannot have a max length (and Size.MaximumLength will always be 16), so don't check
                                        int sqlSize = Size.MaximumLength / 2;
                                        if ((SqlDbType != SqlDbType.NText) &&
                                            (m != TypeConstraintMode.Silent) &&
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
                                    ? CreateConverterTo<DateTime?>(
                                        t,
                                        (c, m) =>
                                        {
                                            if (c == null)
                                                return DBNull.Value;

                                            DateTime original = (DateTime)c;

                                            DateTimeRange range = DateTypeSizes[SqlDbType];
                                            Debug.Assert(range != null);

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
                                    : CreateConverterTo<DateTime>(
                                        t,
                                        (c, m) =>
                                        {
                                            DateTimeRange range = DateTypeSizes[SqlDbType];
                                            Debug.Assert(range != null);

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
                                return CreateConverterTo<TimeSpan>(t);
                            case SqlDbType.Decimal:
                                return CreateConverterTo<decimal>(t);
                            case SqlDbType.Float:
                                return CreateConverterTo<double>(t);
                            case SqlDbType.Int:
                                return CreateConverterTo<int>(t);
                            case SqlDbType.Money:
                            case SqlDbType.SmallMoney:
                                return CreateConverterTo<decimal>(t);
                            case SqlDbType.Real:
                                return CreateConverterTo<float>(t);
                            case SqlDbType.UniqueIdentifier:
                                return CreateConverterTo<Guid>(t);
                            case SqlDbType.SmallInt:
                                return CreateConverterTo<short>(t);
                            case SqlDbType.TinyInt:
                                return CreateConverterTo<byte>(t);
                            case SqlDbType.Variant:
                                // TODO Variant type can't accept everything!
                                return (c, m) => c;
                            case SqlDbType.Xml:
                                return CreateConverterTo<XNode>(
                                        t,
                                        (c, m) =>
                                        {
                                            if (c == null)
                                                return (object)SqlXml.Null;
                                            using (XmlReader xmlNodeReader = c.CreateReader())
                                                return new SqlXml(xmlNodeReader);
                                        })
                                    ?? CreateConverterTo<XmlNode>(
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
                                        return CreateConverterTo<SqlGeography>(t, false);
                                    case "geometry":
                                        return CreateConverterTo<SqlGeometry>(t, false);
                                    case "hierarchyid":
                                        return CreateConverterTo<SqlHierarchyId>(t, false);
                                    default:
                                        // Log error, but don't throw it.
                                        // ReSharper disable once ObjectCreationAsStatement
                                        new DatabaseSchemaException(
                                            LoggingLevel.Critical,
                                            () => Resources
                                                .SqlType_GetConverter_UdtTypeNotSupported,
                                            Name);
                                        return null;
                                }
                            case SqlDbType.Structured:
                                // This is actually a SqlTableType.
                                SqlTableType tableType = this as SqlTableType;
                                if (tableType == null)
                                {
                                    // Log error, but don't throw it.
                                    // ReSharper disable once ObjectCreationAsStatement
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
                                    // ReSharper disable once PossibleNullReferenceException
                                    if (!columnSqlTypes[minColumns - 1].IsNullable)
                                        break;

                                // Check if type implements IEnumerable<T>
                                Type enumerableInterface =
                                    t.IsGenericType && (t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                                        ? t
                                        : t.GetInterfaces()
                                            .FirstOrDefault(
                                                it =>
                                                    // ReSharper disable once PossibleNullReferenceException
                                                    it.IsGenericType &&
                                                    it.GetGenericTypeDefinition() == typeof(IEnumerable<>));

                                if (enumerableInterface != null)
                                {
                                    // Get the enumeration type
                                    Type enumerationType = enumerableInterface.GetGenericArguments().First();
                                    Debug.Assert(enumerationType != null);

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
                                        // ReSharper disable once PossibleNullReferenceException
                                        enumerationType.FullName.StartsWith("System.Tuple`"))
                                    {
                                        // Get converters for tuple elements
                                        Type[] tupleTypes = enumerationType.GetIndexTypes();
                                        int items = tupleTypes.Length;

                                        // Check we don't have too many items in the tuple.
                                        if (items > columns)
                                        {
                                            // Log error, but don't throw it.
                                            // ReSharper disable once ObjectCreationAsStatement
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
                                            // ReSharper disable once ObjectCreationAsStatement
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
                                                // ReSharper disable once PossibleNullReferenceException, AssignNullToNotNullAttribute
                                                columnSqlTypes[i].GetClrToSqlConverter(tupleTypes[i]);
                                            if (columnConverter == null)
                                            {
                                                // Log error, but don't throw it.
                                                // ReSharper disable once ObjectCreationAsStatement
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
                                                    {
                                                        Debug.Assert(converters[i] != null);
                                                        // ReSharper disable once AssignNullToNotNullAttribute
                                                        record.SetValue(i, converters[i](o, m));
                                                    }
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
                                        Debug.Assert(columnType != null);

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
                                                        // ReSharper disable once AssignNullToNotNullAttribute
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
                                        Debug.Assert(keyColumnType != null);
                                        Debug.Assert(valueColumnType != null);

                                        Debug.Assert(kvpTypes[0] != null);
                                        Func<object, TypeConstraintMode, object> keyConverter =
                                            keyColumnType.GetClrToSqlConverter(kvpTypes[0]);
                                        if (keyConverter == null)
                                        {
                                            // Log error, but don't throw it.
                                            // ReSharper disable once ObjectCreationAsStatement
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
                                        Debug.Assert(keySelector != null);

                                        Func<object, TypeConstraintMode, object> k =
                                            (o, m) => keyConverter(keySelector(o), m);

                                        Debug.Assert(kvpTypes[1] != null);
                                        Func<object, TypeConstraintMode, object> valueConverter =
                                            valueColumnType.GetClrToSqlConverter(kvpTypes[1]);
                                        if (valueConverter == null)
                                        {
                                            // Log error, but don't throw it.
                                            // ReSharper disable once ObjectCreationAsStatement
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
                                        Debug.Assert(valueSelector != null);

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
                                                    // ReSharper disable AssignNullToNotNullAttribute
                                                    record.SetValue(0, k(o, m));
                                                    record.SetValue(1, v(o, m));
                                                    // ReSharper restore AssignNullToNotNullAttribute
                                                    records.Add(record);
                                                }

                                                // If we have zero count return DBNull (never return empty enumeration!)
                                                return records.Count < 1
                                                    ? (object)null
                                                    : records;
                                            });
                                    }

                                    // Unsupported Log error, but don't throw it.
                                    // ReSharper disable once ObjectCreationAsStatement
                                    new DatabaseSchemaException(
                                        LoggingLevel.Critical,
                                        () => Resources
                                            .SqlType_GetClrToSqlConverter_CannotAcceptEnumerationType,
                                        this,
                                        enumerationType);
                                    return null;
                                }

                                // Unsupported Log error, but don't throw it.
                                // ReSharper disable once ObjectCreationAsStatement
                                new DatabaseSchemaException(
                                    LoggingLevel.Critical,
                                    () => Resources.SqlType_GetClrToSqlConverter_CannotAcceptType,
                                    this,
                                    t);
                                return null;
                            default:
                                throw new DatabaseSchemaException(
                                    LoggingLevel.Critical,
                                    () => Resources.SqlType_GetConverter_UnsupportedSqlDbType,
                                    SqlDbType);
                        }
                    }
                    catch (Exception e)
                    {
                        // ReSharper disable once ObjectCreationAsStatement
                        new DatabaseSchemaException(
                            e,
                            LoggingLevel.Critical,
                            () => Resources.SqlType_GetConverter_FatalErrorOccurred,
                            t,
                            this,
                            e.Message
                            );
                    }

                    // Unsupported conversion
                    return null;
                });

            bool tmp;
            _acceptsType.TryRemove(clrType, out tmp);

            return conv;
        }

        /// <summary>
        ///   Gets the SQL to CLR type converter if one can be found for the specified CLR type.
        ///   The converter function takes two inputs, the CLR object to convert and also the
        ///   <see cref="TypeConstraintMode">constraint mode</see>, which determines what will
        ///   happen if truncation/loss of precision occurs.
        /// </summary>
        /// <typeparam name="T">The CLR type to retrieve the converter for.</typeparam>
        /// <returns>
        ///   The converter (if found); otherwise returns <see langword="null"/>.
        /// </returns>
        /// <exception cref="DatabaseSchemaException">
        ///   <para>The type <typeparamref name="T"/> was unsupported.</para>
        ///   <para>-or-</para>
        ///   <para>A fatal error occurred.</para>
        /// </exception>
        [CanBeNull]
        public Func<object, TypeConstraintMode, object> GetSqlToClrConverter<T>() => GetSqlToClrConverter(typeof(T));

        /// <summary>
        ///   Gets the SQL to CLR type converter if one can be found for the specified CLR type.
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
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="clrType"/> is <see langword="null" />.</exception>
        public Func<object, TypeConstraintMode, object> GetSqlToClrConverter([NotNull] Type clrType)
        {
            if (clrType == null) throw new ArgumentNullException(nameof(clrType));

            Func<object, TypeConstraintMode, object> conv = _sqlTypeConverters.GetOrAdd(
                clrType,
                t =>
                {
                    // Should never happen, but if it does there is not conversion from a null type!
                    if (t == null)
                    {
                        // Log error, but don't throw it.
                        // ReSharper disable once ObjectCreationAsStatement
                        new DatabaseSchemaException(
                            LoggingLevel.Critical,
                            () => Resources.SqlType_GetClrToSqlConverter_NoTypeSpecified,
                            this);
                        return null;
                    }

                    // Check if we have already determined if this type is not accepted
                    bool acceptsType;
                    if (_acceptsType.TryGetValue(t, out acceptsType) && !acceptsType)
                        return null;

                    // TODO Support Sql* types?
                    try
                    {
                        switch (SqlDbType)
                        {
                            case SqlDbType.BigInt:
                                return CreateConverterFrom<long>(t);
                            case SqlDbType.Image:
                            case SqlDbType.Binary:
                            case SqlDbType.Timestamp:
                            case SqlDbType.VarBinary:
                                Func<object, TypeConstraintMode, object> binaryConverter = 
                                    CreateConverterFrom<byte[]>(t);
                                if (binaryConverter != null)
                                    return binaryConverter;

                                if (t.IsSerializable)
                                {
                                    return (c, m) =>
                                    {
                                        if (c.IsNull())
                                            return null;

                                        // Serialize object
                                        byte[] serializedObject = (byte[])c;

                                        return serializedObject.Deserialize<object>();
                                    };
                                }

                                return null;
                            case SqlDbType.Bit:
                                return CreateConverterFrom<bool>(t);
                            case SqlDbType.Text:
                            case SqlDbType.Char:
                            case SqlDbType.VarChar:
                            case SqlDbType.NChar:
                            case SqlDbType.NText:
                            case SqlDbType.NVarChar:
                                return CreateConverterFrom<string>(t);
                            case SqlDbType.SmallDateTime:
                            case SqlDbType.DateTime:
                            case SqlDbType.Date:
                            case SqlDbType.DateTime2:
                                return CreateConverterFrom<DateTime>(t);
                            case SqlDbType.DateTimeOffset:
                                return CreateConverterFrom<DateTimeOffset>(t);
                            case SqlDbType.Time:
                                return CreateConverterFrom<TimeSpan>(t);
                            case SqlDbType.Decimal:
                                return CreateConverterFrom<decimal>(t);
                            case SqlDbType.Float:
                                return CreateConverterFrom<double>(t);
                            case SqlDbType.Int:
                                return CreateConverterFrom<int>(t);
                            case SqlDbType.Money:
                            case SqlDbType.SmallMoney:
                                return CreateConverterFrom<decimal>(t);
                            case SqlDbType.Real:
                                return CreateConverterFrom<float>(t);
                            case SqlDbType.UniqueIdentifier:
                                return CreateConverterFrom<Guid>(t);
                            case SqlDbType.SmallInt:
                                return CreateConverterFrom<short>(t);
                            case SqlDbType.TinyInt:
                                return CreateConverterFrom<byte>(t);
                            case SqlDbType.Variant:
                                return (c, m) => c;
                            case SqlDbType.Xml:
                                var xmlConverter = CreateConverterFrom<XNode>(t);
                                if (xmlConverter != null)
                                    return (c, m) => c.IsNull() ? null : xmlConverter(XElement.Parse((string)c), m);

                                xmlConverter = CreateConverterFrom<XmlNode>(t);
                                if (xmlConverter != null)
                                    return (c, m) =>
                                    {
                                        if (c.IsNull()) return null;

                                        XmlDocument doc = new XmlDocument();
                                        doc.LoadXml((string)c);
                                        return xmlConverter(doc, m);
                                    };

                                return CreateConverterFrom<string>(t);
                            case SqlDbType.Udt:
                                // Add UDT conversions (which depend on the type name).
                                switch (Name)
                                {
                                    case "geography":
                                        return CreateConverterFrom<SqlGeography>(t, false);
                                    case "geometry":
                                        return CreateConverterFrom<SqlGeometry>(t, false);
                                    case "hierarchyid":
                                        return CreateConverterFrom<SqlHierarchyId>(t, false);
                                    default:
                                        // Log error, but don't throw it.
                                        // ReSharper disable once ObjectCreationAsStatement
                                        new DatabaseSchemaException(
                                            LoggingLevel.Critical,
                                            () => Resources.SqlType_GetConverter_UdtTypeNotSupported,
                                            Name);
                                        return null;
                                }
                                // TODO Structured. Currently this method is only *needed* for output parameters, and TVPs cant be output.
                            case SqlDbType.Structured:
                            default:
                                throw new DatabaseSchemaException(
                                    LoggingLevel.Critical,
                                    () => Resources.SqlType_GetConverter_UnsupportedSqlDbType,
                                    SqlDbType);
                        }
                    }
                    catch (Exception e)
                    {
                        // ReSharper disable once ObjectCreationAsStatement
                        new DatabaseSchemaException(
                            e,
                            LoggingLevel.Critical,
                            () => Resources.SqlType_GetConverter_FatalErrorOccurred,
                            t,
                            this,
                            e.Message);
                    }

                    // Unsupported conversion
                    return null;
                });

            bool tmp;
            _acceptsType.TryRemove(clrType, out tmp);

            return conv;
        }

        /// <summary>
        /// Returns a converter that casts a CLR type to/from a SQL CLR type
        /// (if the input type is assignable to the output type).
        /// </summary>
        /// <typeparam name="TSql">The CLR type for the SQL type.</typeparam>
        /// <param name="convertTo">
        /// If set to <see langword="true" /> convert from the <paramref name="actualClrType"/> to the <typeparamref name="TSql"/> type;
        /// otherwise convert from the <typeparamref name="TSql"/> type to the <paramref name="actualClrType"/>.
        /// </param>
        /// <param name="actualClrType">The actual CLR type.</param>
        /// <param name="supportNullable"><para>If set to <see langword="true" /> then supports <see langword="null" />.</para>
        /// <para>By default this is set to <see langword="true" />.</para></param>
        /// <returns>
        /// The created converter; or <see langword="null" /> if the input type is not assignable to the required input type.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">actualClrType</exception>
        [CanBeNull]
        private static Func<object, TypeConstraintMode, object> CreateConverter<TSql>(
            bool convertTo,
            [NotNull] Type actualClrType,
            bool supportNullable = true)
        {
            if (actualClrType == null) throw new ArgumentNullException("actualClrType");

            bool isNullable = false;

            // Check if we support nullable types and the type is Nullable<>
            if (supportNullable &&
                actualClrType.IsGenericType &&
                actualClrType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // Get the generic parameter and find the relevant converter
                actualClrType = actualClrType.GetGenericArguments().First();
                isNullable = true;
                Debug.Assert(actualClrType != null);
            }

            // Find a conversion if any
            Func<object, object> converter = convertTo
                ? actualClrType.GetConversion(typeof(TSql))
                : typeof(TSql).GetConversion(actualClrType);

            // If we didn't find one there is no known conversion.
            if (converter == null)
                return null;

            // If we're actually a nullable type then wrap.
            if (isNullable)
            {
                Func<object, object> conCopy = converter;
                if (convertTo) converter = c => c.IsNull() ? DBNull.Value : conCopy(c);
                else converter = c => c.IsNull() ? null : conCopy(c);
            }

            // Ignore the type constraint.
            return (o, m) => converter(o);
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
        private static Func<object, TypeConstraintMode, object> CreateConverterTo<TClr>(
            [NotNull] Type actualClrType,
            bool supportNullable = true)
        {
            return CreateConverter<TClr>(true, actualClrType, supportNullable);
        }

        /// <summary>
        ///   Returns a converter that casts from the input type to the output type
        ///   (if the input type is assignable to the output type).
        /// </summary>
        /// <typeparam name="TClr">The CLR type to convert from.</typeparam>
        /// <param name="actualClrType">The CLR type to convert to.</param>
        /// <param name="supportNullable">
        ///   <para>If set to <see langword="true"/> then supports <see langword="null"/>.</para>
        ///   <para>By default this is set to <see langword="true"/>.</para>
        /// </param>
        /// <returns>
        ///   The created converter; or <see langword="null"/> if the input type is not assignable to the required input type.
        /// </returns>
        private static Func<object, TypeConstraintMode, object> CreateConverterFrom<TClr>(
            [NotNull] Type actualClrType,
            bool supportNullable = true)
        {
            return CreateConverter<TClr>(false, actualClrType, supportNullable);
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
        private static Func<object, TypeConstraintMode, object> CreateConverterTo<TClr>(
            [NotNull] Type actualClrType,
            [NotNull] Func<TClr, TypeConstraintMode, object> converter)
        {
            if (actualClrType == null) throw new ArgumentNullException("actualClrType");
            if (converter == null) throw new ArgumentNullException("converter");

            if (typeof(TClr) == actualClrType)
                return (c, m) => converter((TClr)c, m);

            Func<object, TClr> toInputType = actualClrType.GetConversion<object, TClr>();
            return toInputType != null
                ? (Func<object, TypeConstraintMode, object>)((c, m) => converter(toInputType(c), m))
                : null;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString() => ToString(Size);

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
            if (IsUserDefined)
                return FullName;

            switch (SqlDbType)
            {
                case SqlDbType.BigInt:
                case SqlDbType.Bit:
                case SqlDbType.DateTime:
                case SqlDbType.Image:
                case SqlDbType.Int:
                case SqlDbType.Money:
                case SqlDbType.NText:
                case SqlDbType.UniqueIdentifier:
                case SqlDbType.SmallDateTime:
                case SqlDbType.SmallInt:
                case SqlDbType.SmallMoney:
                case SqlDbType.Text:
                case SqlDbType.Timestamp:
                case SqlDbType.TinyInt:
                case SqlDbType.Variant:
                case SqlDbType.Xml:
                case SqlDbType.Udt:
                case SqlDbType.Structured:
                case SqlDbType.Date:
                    return Name;

                case SqlDbType.Binary:
                case SqlDbType.Char:
                    return Name + "(" + size.MaximumLength.ToString(CultureInfo.InvariantCulture) + ")";
                case SqlDbType.NChar:
                    return Name + "(" + (size.MaximumLength / 2).ToString(CultureInfo.InvariantCulture) + ")";

                case SqlDbType.VarBinary:
                case SqlDbType.VarChar:
                    if (size.MaximumLength > 8000 || size.MaximumLength < 0) return Name + "(MAX)";
                    return Name + "(" + size.MaximumLength.ToString(CultureInfo.InvariantCulture) + ")";
                case SqlDbType.NVarChar:
                    if (size.MaximumLength > 8000 || size.MaximumLength < 0) return Name + "(MAX)";
                    return Name + "(" + (size.MaximumLength / 2).ToString(CultureInfo.InvariantCulture) + ")";

                case SqlDbType.Decimal:
                    return Name + "(" + size.Precision.ToString(CultureInfo.InvariantCulture) + "," + size.Scale.ToString(CultureInfo.InvariantCulture) + ")";

                case SqlDbType.Float:
                case SqlDbType.Real:
                    return size.Precision <= 24 ? "real" : "float";

                case SqlDbType.Time:
                case SqlDbType.DateTime2:
                case SqlDbType.DateTimeOffset:
                    return Name + "(" + size.Scale.ToString(CultureInfo.InvariantCulture) + ")";

                default:
                    Debug.Fail($"SqlDbType '{SqlDbType}'");
                    throw new ArgumentOutOfRangeException(nameof(SqlDbType));
            }
        }
    }
}