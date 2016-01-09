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
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.IO;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Reflect;

namespace WebApplications.Utilities.Database.Caching
{
    /// <summary>
    /// Provides extra info and methods for a <see cref="SqlDbType"/>.
    /// </summary>
    [PublicAPI]
    public partial class SqlDbTypeInfo
    {
        /// <summary>
        /// Defines a method that will serialize a <paramref name="sqlValue">non-null provider-specific value</paramref>
        /// into the <paramref name="stream">specified stream</paramref>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="sqlValue">The provider-specific value.</param>
        public delegate void SerializeDelegate([NotNull] Stream stream, [NotNull] object sqlValue);

        /// <summary>
        /// Defines a method that will de-serialize a non-null value from
        /// the <paramref name="buffer">specified buffer</paramref>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized value</returns>
        [NotNull]
        public delegate object DeserializeDelegate([NotNull] byte[] buffer, ref long offset);

        /// <summary>
        /// Holds all <see cref="SqlDbTypeInfo"/> by <see cref="SqlDbType"/>.
        /// </summary>
        [NotNull]
        private static readonly Dictionary<SqlDbType, SqlDbTypeInfo> _typeInfos;

        /// <summary>
        /// Initializes static members of the <see cref="SqlDbTypeInfo"/> class.
        /// </summary>
        static SqlDbTypeInfo()
        {
            _typeInfos = new Dictionary<SqlDbType, SqlDbTypeInfo>
            {
                /*
                 * Fixed length
                 */
                {
                    SqlDbType.Bit,
                    new SqlDbTypeInfo(SqlDbType.Bit,
                        "bit",
                        DbType.Boolean,
                        typeof(bool),
                        typeof(SqlBoolean),
                        SerializeSqlBoolean,
                        DeserializeSqlBoolean,
                        DeserializeBoolean)
                },
                {
                    SqlDbType.TinyInt,
                    new SqlDbTypeInfo(SqlDbType.TinyInt,
                        "tinyint",
                        DbType.Byte,
                        typeof(byte),
                        typeof(SqlByte),
                        SerializeSqlByte,
                        DeserializeSqlByte,
                        DeserializeByte)
                },
                {
                    SqlDbType.SmallInt,
                    new SqlDbTypeInfo(SqlDbType.SmallInt,
                        "smallint",
                        DbType.Int16,
                        typeof(short),
                        typeof(SqlInt16),
                        SerializeSqlInt16,
                        DeserializeSqlInt16,
                        DeserializeInt16)
                },
                {
                    SqlDbType.Int,
                    new SqlDbTypeInfo(SqlDbType.Int,
                        "int",
                        DbType.Int32,
                        typeof(int),
                        typeof(SqlInt32),
                        SerializeSqlInt32,
                        DeserializeSqlInt32,
                        DeserializeInt32)
                },
                {
                    SqlDbType.Real,
                    new SqlDbTypeInfo(SqlDbType.Real,
                        "real",
                        DbType.Single,
                        typeof(float),
                        typeof(SqlSingle),
                        SerializeSqlSingle,
                        DeserializeSqlSingle,
                        DeserializeFloat)
                },
                {
                    SqlDbType.BigInt,
                    new SqlDbTypeInfo(SqlDbType.BigInt,
                        "bigint",
                        DbType.Int64,
                        typeof(long),
                        typeof(SqlInt64),
                        SerializeSqlInt64,
                        DeserializeSqlInt64,
                        DeserializeInt64)
                },
                {
                    SqlDbType.Float,
                    new SqlDbTypeInfo(SqlDbType.Float,
                        "float",
                        DbType.Double,
                        typeof(double),
                        typeof(SqlDouble),
                        SerializeSqlDouble,
                        DeserializeSqlDouble,
                        DeserializeDouble)
                },
                {
                    SqlDbType.Decimal,
                    new SqlDbTypeInfo(SqlDbType.Decimal,
                        "decimal",
                        DbType.Decimal,
                        typeof(decimal),
                        typeof(SqlDecimal),
                        SerializeSqlDecimal,
                        DeserializeSqlDecimal,
                        DeserializeDecimal)
                },
                {
                    SqlDbType.Money,
                    new SqlDbTypeInfo(SqlDbType.Money,
                        "money",
                        DbType.Currency,
                        typeof(decimal),
                        typeof(SqlMoney),
                        SerializeSqlMoney,
                        DeserializeSqlMoney,
                        DeserializeDecimal)
                },
                {
                    SqlDbType.SmallMoney,
                    new SqlDbTypeInfo(SqlDbType.SmallMoney,
                        "smallmoney",
                        DbType.Currency,
                        typeof(decimal),
                        typeof(SqlMoney),
                        SerializeSqlMoney,
                        DeserializeSqlMoney,
                        DeserializeDecimal)
                },
                {
                    SqlDbType.DateTime,
                    new SqlDbTypeInfo(SqlDbType.DateTime,
                        "datetime",
                        DbType.DateTime,
                        typeof(DateTime),
                        typeof(SqlDateTime),
                        SerializeSqlDateTime,
                        DeserializeSqlDateTime,
                        DeserializeDateTime)
                },
                {
                    SqlDbType.SmallDateTime,
                    new SqlDbTypeInfo(SqlDbType.SmallDateTime,
                        "smalldatetime",
                        DbType.DateTime,
                        typeof(DateTime),
                        typeof(SqlDateTime),
                        SerializeSqlDateTime,
                        DeserializeSqlDateTime,
                        DeserializeDateTime)
                },
                {
                    SqlDbType.Date,
                    new SqlDbTypeInfo(SqlDbType.Date,
                        "date",
                        DbType.Date,
                        typeof(DateTime),
                        typeof(DateTime),
                        SerializeDateTime,
                        DeserializeDateTime,
                        DeserializeDateTime)
                },
                {
                    SqlDbType.DateTime2,
                    new SqlDbTypeInfo(SqlDbType.DateTime2,
                        "datetime2",
                        DbType.DateTime2,
                        typeof(DateTime),
                        typeof(DateTime),
                        SerializeDateTime,
                        DeserializeDateTime,
                        DeserializeDateTime)
                },
                {
                    SqlDbType.DateTimeOffset,
                    new SqlDbTypeInfo(SqlDbType.DateTimeOffset,
                        "datetimeoffset",
                        DbType.DateTimeOffset,
                        typeof(DateTimeOffset),
                        typeof(DateTimeOffset),
                        SerializeDateTimeOffset,
                        DeserializeDateTimeOffset,
                        DeserializeDateTimeOffset)
                },
                {
                    SqlDbType.Time,
                    new SqlDbTypeInfo(SqlDbType.Time,
                        "time",
                        DbType.Time,
                        typeof(TimeSpan),
                        typeof(TimeSpan),
                        SerializeTimeSpan,
                        DeserializeTimeSpan,
                        DeserializeTimeSpan)
                },
                {
                    SqlDbType.UniqueIdentifier,
                    new SqlDbTypeInfo(SqlDbType.UniqueIdentifier,
                        "uniqueidentifier",
                        DbType.Guid,
                        typeof(Guid),
                        typeof(SqlGuid),
                        SerializeSqlGuid,
                        DeserializeSqlGuid,
                        DeserializeGuid)
                },

                /*
                 * Variable length
                 */
                {
                    SqlDbType.Binary,
                    new SqlDbTypeInfo(SqlDbType.Binary,
                        "binary",
                        DbType.Binary,
                        typeof(byte[]),
                        typeof(SqlBinary),
                        SerializeSqlBinary,
                        DeserializeSqlBinary,
                        DeserializeByteArray)
                },
                {
                    SqlDbType.Timestamp,
                    new SqlDbTypeInfo(SqlDbType.Timestamp,
                        "timestamp",
                        DbType.Binary,
                        typeof(byte[]),
                        typeof(SqlBinary),
                        SerializeSqlBinary,
                        DeserializeSqlBinary,
                        DeserializeByteArray)
                },
                {
                    SqlDbType.VarBinary,
                    new SqlDbTypeInfo(SqlDbType.VarBinary,
                        "varbinary",
                        DbType.Binary,
                        typeof(byte[]),
                        typeof(SqlBinary),
                        SerializeSqlBinary,
                        DeserializeSqlBinary,
                        DeserializeByteArray)
                },
                {
                    SqlDbType.Image,
                    new SqlDbTypeInfo(SqlDbType.Image,
                        "image",
                        DbType.Binary,
                        typeof(byte[]),
                        typeof(SqlBinary),
                        SerializeSqlBinary,
                        DeserializeSqlBinary,
                        DeserializeByteArray)
                },
                {
                    SqlDbType.Char,
                    new SqlDbTypeInfo(SqlDbType.Char,
                        "char",
                        DbType.AnsiStringFixedLength,
                        typeof(string),
                        typeof(SqlString),
                        SerializeAnsiSqlString,
                        DeserializeAnsiSqlString,
                        DeserializeAnsiString)
                },
                {
                    SqlDbType.VarChar,
                    new SqlDbTypeInfo(SqlDbType.VarChar,
                        "varchar",
                        DbType.AnsiString,
                        typeof(string),
                        typeof(SqlString),
                        SerializeAnsiSqlString,
                        DeserializeAnsiSqlString,
                        DeserializeAnsiString)
                },
                {
                    SqlDbType.Text,
                    new SqlDbTypeInfo(SqlDbType.Text,
                        "text",
                        DbType.AnsiString,
                        typeof(string),
                        typeof(SqlString),
                        SerializeAnsiSqlString,
                        DeserializeAnsiSqlString,
                        DeserializeAnsiString)
                },
                {
                    SqlDbType.NChar,
                    new SqlDbTypeInfo(SqlDbType.NChar,
                        "nchar",
                        DbType.StringFixedLength,
                        typeof(string),
                        typeof(SqlString),
                        SerializeSqlString,
                        DeserializeSqlString,
                        DeserializeString)
                },
                {
                    SqlDbType.NVarChar,
                    new SqlDbTypeInfo(SqlDbType.NVarChar,
                        "nvarchar",
                        DbType.String,
                        typeof(string),
                        typeof(SqlString),
                        SerializeSqlString,
                        DeserializeSqlString,
                        DeserializeString)
                },
                {
                    SqlDbType.NText,
                    new SqlDbTypeInfo(SqlDbType.NText,
                        "ntext",
                        DbType.String,
                        typeof(string),
                        typeof(SqlString),
                        SerializeSqlString,
                        DeserializeSqlString,
                        DeserializeString)
                },
                {
                    SqlDbType.Udt,
                    new SqlDbTypeInfo(SqlDbType.Udt,
                        "udt",
                        DbType.Object,
                        typeof(object),
                        typeof(object),
                        SerializeUdt,
                        DeserializeUdt,
                        DeserializeUdt)
                },
                {
                    SqlDbType.Variant,
                    new SqlDbTypeInfo(SqlDbType.Variant,
                        "sql_variant",
                        DbType.Object,
                        typeof(object),
                        typeof(object),
                        SerializeVariant,
                        DeserializeVariant,
                        DeserializeVariant)
                },
                {
                    SqlDbType.Structured,
                    new SqlDbTypeInfo(SqlDbType.Structured,
                        "table",
                        DbType.Object,
                        typeof(IEnumerable<DbDataRecord>),
                        typeof(IEnumerable<DbDataRecord>),
                        SerializeStructured,
                        DeserializeStructured,
                        DeserializeStructured)
                },

                {
                    SqlDbType.Xml,
                    new SqlDbTypeInfo(SqlDbType.Xml,
                        "xml",
                        DbType.Xml,
                        typeof(string),
                        typeof(SqlXml),
                        SerializeSqlXml,
                        DeserializeSqlXml,
                        DeserializeXmlString)
                }
            };
        }

        /// <summary>
        /// The SQL database value type.
        /// </summary>
        public readonly SqlDbType SqlDbType;

        /// <summary>
        /// The database value type.
        /// </summary>
        public readonly DbType DbType;

        /// <summary>
        /// The name.
        /// </summary>
        [NotNull]
        public readonly string Name;

        /// <summary>
        /// The CLR type.
        /// </summary>
        [NotNull]
        public readonly ExtendedType ClrType;

        /// <summary>
        /// The provider specific type.
        /// </summary>
        [NotNull]
        public readonly ExtendedType ProviderType;

        /// <summary>
        /// Will serialize a value of the current type.
        /// </summary>
        [NotNull]
        public readonly SerializeDelegate Serialize;

        /// <summary>
        /// Will deserialize a value of the current type to the provider-specific value.
        /// </summary>
        [NotNull]
        public readonly DeserializeDelegate DeserializeSqlValue;

        /// <summary>
        /// Will deserialize a value of the current type.
        /// </summary>
        [NotNull]
        public readonly DeserializeDelegate DeserializeValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDbTypeInfo" /> class.
        /// </summary>
        /// <param name="sqlDbType">Type of the SQL database value.</param>
        /// <param name="name">The name.</param>
        /// <param name="dbType">The equivalent Db type.</param>
        /// <param name="clrType">Type of the color.</param>
        /// <param name="providerType">Type of the provider.</param>
        /// <param name="serialize"></param>
        /// <param name="deserializeSqlValue"></param>
        /// <param name="deserializeValue"></param>
        private SqlDbTypeInfo(
            SqlDbType sqlDbType,
            [NotNull] string name,
            DbType dbType,
            [NotNull] Type clrType,
            [NotNull] Type providerType,
            [NotNull] SerializeDelegate serialize,
            [NotNull] DeserializeDelegate deserializeSqlValue,
            [NotNull] DeserializeDelegate deserializeValue)
        {
            SqlDbType = sqlDbType;
            Name = name;
            DbType = dbType;
            ClrType = clrType;
            ProviderType = providerType;
            Serialize = serialize;
            DeserializeSqlValue = deserializeSqlValue;
            DeserializeValue = deserializeValue;
        }

        /// <summary>
        /// Gets the <see cref="SqlDbTypeInfo"/> with the <paramref name="sqlDbType">specified type</paramref>.
        /// </summary>
        /// <param name="sqlDbType">Type of the SQL database value.</param>
        /// <returns>A <see cref="SqlDbTypeInfo"/>.</returns>
        [NotNull]
        // ReSharper disable once AssignNullToNotNullAttribute
        public static SqlDbTypeInfo Get(SqlDbType sqlDbType) => _typeInfos[sqlDbType];

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString() => $"{SqlDbType} Info";
    }
}