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

using Microsoft.SqlServer.Server;
using Microsoft.SqlServer.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Text;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Blit;
using WebApplications.Utilities.Reflect;

namespace WebApplications.Utilities.Database.Caching
{
    /// <summary>
    /// Provides extra info and methods for a <see cref="SqlDbType"/>.
    /// </summary>
    [PublicAPI]
    public class SqlDbTypeInfo
    {
        /// <summary>
        /// Defines a method that will serialize a <paramref name="sqlValue">non-null provider-specific value</paramref>
        /// into the <paramref name="buffer">specified buffer</paramref>.
        /// </summary>
        /// <param name="sqlValue">The provider-specific value.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        public delegate void SerializeDelegate([NotNull] object sqlValue, [NotNull] byte[] buffer, ref long offset);

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
        /// Holds all <see cref="SqlDbTypeInfo"/> by <see cref="ID"/>.
        /// </summary>
        [NotNull]
        private static readonly Dictionary<byte, SqlDbTypeInfo> _typeInfos;

        /// <summary>
        /// Initializes static members of the <see cref="SqlDbTypeInfo"/> class.
        /// </summary>
        static SqlDbTypeInfo()
        {
            _typeInfos = new Dictionary<byte, SqlDbTypeInfo>
            {
                /*
                 * Fixed length
                 */
                {
                    1,
                    new SqlDbTypeInfo(
                        1,
                        SqlDbType.Bit,
                        "bit",
                        DbType.Boolean,
                        typeof(bool),
                        typeof(SqlBoolean),
                        1,
                        SerializeSqlBoolean,
                        DeserializeSqlBoolean,
                        DeserializeBoolean)
                },
                {
                    2,
                    new SqlDbTypeInfo(
                        2,
                        SqlDbType.TinyInt,
                        "tinyint",
                        DbType.Byte,
                        typeof(byte),
                        typeof(SqlByte),
                        1,
                        SerializeSqlByte,
                        DeserializeSqlByte,
                        DeserializeByte)
                },
                {
                    3,
                    new SqlDbTypeInfo(
                        3,
                        SqlDbType.SmallInt,
                        "smallint",
                        DbType.Int16,
                        typeof(short),
                        typeof(SqlInt16),
                        2,
                        SerializeSqlInt16,
                        DeserializeSqlInt16,
                        DeserializeInt16)
                },
                {
                    4,
                    new SqlDbTypeInfo(
                        4,
                        SqlDbType.Int,
                        "int",
                        DbType.Int32,
                        typeof(int),
                        typeof(SqlInt32),
                        4,
                        SerializeSqlInt32,
                        DeserializeSqlInt32,
                        DeserializeInt32)
                },
                {
                    5,
                    new SqlDbTypeInfo(
                        5,
                        SqlDbType.Real,
                        "real",
                        DbType.Single,
                        typeof(float),
                        typeof(SqlSingle),
                        4,
                        SerializeSqlSingle,
                        DeserializeSqlSingle,
                        DeserializeFloat)
                },
                {
                    6,
                    new SqlDbTypeInfo(
                        6,
                        SqlDbType.BigInt,
                        "bigint",
                        DbType.Int64,
                        typeof(long),
                        typeof(SqlInt64),
                        8,
                        SerializeSqlInt64,
                        DeserializeSqlInt64,
                        DeserializeInt64)
                },
                {
                    7,
                    new SqlDbTypeInfo(
                        7,
                        SqlDbType.Float,
                        "float",
                        DbType.Double,
                        typeof(double),
                        typeof(SqlDouble),
                        8,
                        SerializeSqlDouble,
                        DeserializeSqlDouble,
                        DeserializeDouble)
                },
                {
                    8,
                    new SqlDbTypeInfo(
                        8,
                        SqlDbType.Decimal,
                        "decimal",
                        DbType.Decimal,
                        typeof(decimal),
                        typeof(SqlDecimal),
                        17,
                        SerializeSqlDecimal,
                        DeserializeSqlDecimal,
                        DeserializeDecimal /* TODO Why not 16? */)
                },
                {
                    9,
                    new SqlDbTypeInfo(
                        9,
                        SqlDbType.Money,
                        "money",
                        DbType.Currency,
                        typeof(decimal),
                        typeof(SqlMoney),
                        8,
                        SerializeSqlMoney,
                        DeserializeSqlMoney,
                        DeserializeDecimal /* TODO Again, how only 8 and not 16? */)
                },
                {
                    10,
                    new SqlDbTypeInfo(
                        10,
                        SqlDbType.SmallMoney,
                        "smallmoney",
                        DbType.Currency,
                        typeof(decimal),
                        typeof(SqlMoney),
                        4,
                        SerializeSqlMoney,
                        DeserializeSqlMoney,
                        DeserializeDecimal /* TODO Again, how only 4 and not 16? */)
                },
                {
                    11,
                    new SqlDbTypeInfo(
                        11,
                        SqlDbType.DateTime,
                        "datetime",
                        DbType.DateTime,
                        typeof(DateTime),
                        typeof(SqlDateTime),
                        8,
                        SerializeSqlDateTime,
                        DeserializeSqlDateTime,
                        DeserializeDateTime)
                },
                {
                    12,
                    new SqlDbTypeInfo(
                        12,
                        SqlDbType.SmallDateTime,
                        "smalldatetime",
                        DbType.DateTime,
                        typeof(DateTime),
                        typeof(SqlDateTime),
                        4,
                        SerializeSqlDateTime,
                        DeserializeSqlDateTime,
                        DeserializeDateTime /* TODO Only use 4 bytes! */)
                },
                {
                    13,
                    new SqlDbTypeInfo(
                        13,
                        SqlDbType.Date,
                        "date",
                        DbType.Date,
                        typeof(DateTime),
                        typeof(DateTime),
                        3,
                        SerializeDateTime,
                        DeserializeDateTime,
                        DeserializeDateTime /* TODO Why not 4 */)
                },
                {
                    14,
                    new SqlDbTypeInfo(
                        14,
                        SqlDbType.DateTime2,
                        "datetime2",
                        DbType.DateTime2,
                        typeof(DateTime),
                        typeof(DateTime),
                        7,
                        SerializeDateTime,
                        DeserializeDateTime,
                        DeserializeDateTime /* TODO Why not 8 */)
                },
                {
                    15,
                    new SqlDbTypeInfo(
                        15,
                        SqlDbType.DateTimeOffset,
                        "datetimeoffset",
                        DbType.DateTimeOffset,
                        typeof(DateTimeOffset),
                        typeof(DateTimeOffset),
                        7,
                        SerializeDateTimeOffset,
                        DeserializeDateTimeOffset,
                        DeserializeDateTimeOffset /* TODO Why not 8? */)
                },
                {
                    16,
                    new SqlDbTypeInfo(
                        16,
                        SqlDbType.Time,
                        "time",
                        DbType.Time,
                        typeof(TimeSpan),
                        typeof(TimeSpan),
                        7,
                        SerializeTimeSpan,
                        DeserializeTimeSpan,
                        DeserializeTimeSpan /* TODO Why not 8 */)
                },
                {
                    17,
                    new SqlDbTypeInfo(
                        17,
                        SqlDbType.UniqueIdentifier,
                        "uniqueidentifier",
                        DbType.Guid,
                        typeof(Guid),
                        typeof(SqlGuid),
                        16,
                        SerializeSqlGuid,
                        DeserializeSqlGuid,
                        DeserializeGuid)
                },

                /*
                 * Variable length
                 */
                {
                    64,
                    new SqlDbTypeInfo(
                        64,
                        SqlDbType.Binary,
                        "binary",
                        DbType.Binary,
                        typeof(byte[]),
                        typeof(SqlBinary),
                        -1,
                        SerializeSqlBinary,
                        DeserializeSqlBinary,
                        DeserializeByteArray)
                },
                {
                    65,
                    new SqlDbTypeInfo(
                        65,
                        SqlDbType.Timestamp,
                        "timestamp",
                        DbType.Binary,
                        typeof(byte[]),
                        typeof(SqlBinary),
                        -1,
                        SerializeSqlBinary,
                        DeserializeSqlBinary,
                        DeserializeByteArray)
                },
                {
                    66,
                    new SqlDbTypeInfo(
                        66,
                        SqlDbType.VarBinary,
                        "varbinary",
                        DbType.Binary,
                        typeof(byte[]),
                        typeof(SqlBinary),
                        -1,
                        SerializeSqlBinary,
                        DeserializeSqlBinary,
                        DeserializeByteArray)
                },
                {
                    67,
                    new SqlDbTypeInfo(
                        67,
                        SqlDbType.Image,
                        "image",
                        DbType.Binary,
                        typeof(byte[]),
                        typeof(SqlBinary),
                        -1,
                        SerializeSqlBinary,
                        DeserializeSqlBinary,
                        DeserializeByteArray)
                },
                {
                    68,
                    new SqlDbTypeInfo(
                        68,
                        SqlDbType.Char,
                        "char",
                        DbType.AnsiStringFixedLength,
                        typeof(string),
                        typeof(SqlString),
                        -1,
                        SerializeAnsiSqlString,
                        DeserializeAnsiSqlString,
                        DeserializeAnsiString)
                },
                {
                    69,
                    new SqlDbTypeInfo(
                        69,
                        SqlDbType.VarChar,
                        "varchar",
                        DbType.AnsiString,
                        typeof(string),
                        typeof(SqlString),
                        -1,
                        SerializeAnsiSqlString,
                        DeserializeAnsiSqlString,
                        DeserializeAnsiString)
                },
                {
                    70,
                    new SqlDbTypeInfo(
                        70,
                        SqlDbType.Text,
                        "text",
                        DbType.AnsiString,
                        typeof(string),
                        typeof(SqlString),
                        -1,
                        SerializeAnsiSqlString,
                        DeserializeAnsiSqlString,
                        DeserializeAnsiString)
                },
                {
                    71,
                    new SqlDbTypeInfo(
                        71,
                        SqlDbType.NChar,
                        "nchar",
                        DbType.StringFixedLength,
                        typeof(string),
                        typeof(SqlString),
                        -1,
                        SerializeSqlString,
                        DeserializeSqlString,
                        DeserializeString)
                },
                {
                    72,
                    new SqlDbTypeInfo(
                        72,
                        SqlDbType.NVarChar,
                        "nvarchar",
                        DbType.String,
                        typeof(string),
                        typeof(SqlString),
                        -1,
                        SerializeSqlString,
                        DeserializeSqlString,
                        DeserializeString)
                },
                {
                    73,
                    new SqlDbTypeInfo(
                        73,
                        SqlDbType.NText,
                        "ntext",
                        DbType.String,
                        typeof(string),
                        typeof(SqlString),
                        -1,
                        SerializeSqlString,
                        DeserializeSqlString,
                        DeserializeString)
                },

                {
                    74,
                    new SqlDbTypeInfo(
                        74,
                        SqlDbType.Udt,
                        "udt",
                        DbType.Object,
                        typeof(object),
                        typeof(object),
                        -1,
                        SerializeUdt,
                        DeserializeUdt,
                        DeserializeUdt)
                },
                {
                    75,
                    new SqlDbTypeInfo(
                        75,
                        SqlDbType.Variant,
                        "sql_variant",
                        DbType.Object,
                        typeof(object),
                        typeof(object),
                        -1,
                        SerializeVariant,
                        DeserializeVariant,
                        DeserializeVariant)
                },
                {
                    76,
                    new SqlDbTypeInfo(
                        76,
                        SqlDbType.Structured,
                        "table",
                        DbType.Object,
                        typeof(IEnumerable<DbDataRecord>),
                        typeof(IEnumerable<DbDataRecord>),
                        -1,
                        SerializeStructured,
                        DeserializeStructured,
                        DeserializeStructured)
                },

                {
                    77,
                    new SqlDbTypeInfo(
                        77,
                        SqlDbType.Xml,
                        "xml",
                        DbType.Xml,
                        typeof(string),
                        typeof(SqlXml),
                        -1,
                        SerializeSqlXml,
                        DeserializeSqlXml,
                        DeserializeXmlString)
                }
            };
        }

        /// <summary>
        /// The identifier.
        /// </summary>
        public readonly byte ID;

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
        /// The fixed length in bytes; otherwise -1.
        /// </summary>
        public readonly sbyte FixedLength;

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
        /// <param name="id">The identifier.</param>
        /// <param name="sqlDbType">Type of the SQL database value.</param>
        /// <param name="name">The name.</param>
        /// <param name="dbType">The equivalent Db type.</param>
        /// <param name="clrType">Type of the color.</param>
        /// <param name="providerType">Type of the provider.</param>
        /// <param name="fixedLength">The fixed length</param>
        /// <param name="serialize"></param>
        /// <param name="deserializeSqlValue"></param>
        /// <param name="deserializeValue"></param>
        private SqlDbTypeInfo(
            byte id,
            SqlDbType sqlDbType,
            [NotNull] string name,
            DbType dbType,
            [NotNull] Type clrType,
            [NotNull] Type providerType,
            sbyte fixedLength,
            [NotNull] SerializeDelegate serialize,
            [NotNull] DeserializeDelegate deserializeSqlValue,
            [NotNull] DeserializeDelegate deserializeValue)
        {
            ID = id;
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
        public static SqlDbTypeInfo Get(SqlDbType sqlDbType) => _typeInfos[(byte)sqlDbType];

        /// <summary>
        /// Gets the <see cref="SqlDbTypeInfo"/> with the <paramref name="id">specified id</paramref>.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>SqlDbTypeInfo.</returns>
        [NotNull]
        // ReSharper disable once AssignNullToNotNullAttribute
        public static SqlDbTypeInfo Get(byte id) => _typeInfos[id];

        #region Serialize Methods

        #region SqlBoolean - for completeness, not used
        /// <summary>
        /// Serializes a <see cref="SqlBoolean"/>.
        /// </summary>
        /// <param name="sqlValue">The <see cref="SqlBoolean"/>.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        public static void SerializeSqlBoolean([NotNull] object sqlValue, [NotNull] byte[] buffer, ref long offset)
        {
            buffer[offset++] = ((SqlBoolean)sqlValue).Value ? (byte)1 : (byte)0;
        }

        /// <summary>
        /// Deserializes a <see cref="SqlBoolean"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="SqlBoolean"/></returns>
        [NotNull]
        public static object DeserializeSqlBoolean([NotNull] byte[] buffer, ref long offset)
            => new SqlBoolean(buffer[offset++] != 0);

        /// <summary>
        /// Deserializes a <see cref="bool"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="bool"/></returns>
        [NotNull]
        public static object DeserializeBoolean([NotNull] byte[] buffer, ref long offset)
            => buffer[offset++] != 0;
        #endregion

        #region SqlByte
        /// <summary>
        /// Serializes a <see cref="SqlByte"/>.
        /// </summary>
        /// <param name="sqlValue">The <see cref="SqlByte"/>.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        public static void SerializeSqlByte([NotNull] object sqlValue, [NotNull] byte[] buffer, ref long offset)
        {
            buffer[offset++] = ((SqlByte)sqlValue).Value;
        }

        /// <summary>
        /// Deserializes a <see cref="SqlByte"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="SqlByte"/></returns>
        [NotNull]
        public static object DeserializeSqlByte([NotNull] byte[] buffer, ref long offset)
            => new SqlByte(buffer[offset++]);

        /// <summary>
        /// Deserializes a <see cref="byte"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="byte"/></returns>
        [NotNull]
        public static object DeserializeByte([NotNull] byte[] buffer, ref long offset)
            => buffer[offset++];
        #endregion

        #region SqlInt16
        /// <summary>
        /// Serializes a <see cref="SqlInt16"/>.
        /// </summary>
        /// <param name="sqlValue">The <see cref="SqlInt16"/>.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        public static void SerializeSqlInt16([NotNull] object sqlValue, [NotNull] byte[] buffer, ref long offset)
        {
            Blittable2 b = ((SqlInt16)sqlValue).Value;
            buffer[offset++] = b.Byte0;
            buffer[offset++] = b.Byte1;
        }

        /// <summary>
        /// Deserializes a <see cref="SqlInt16"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="SqlInt16"/></returns>
        [NotNull]
        public static object DeserializeSqlInt16([NotNull] byte[] buffer, ref long offset)
            => new SqlInt16(new Blittable2(buffer, ref offset));

        /// <summary>
        /// Deserializes a <see cref="short"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="short"/></returns>
        [NotNull]
        public static object DeserializeInt16([NotNull] byte[] buffer, ref long offset)
            => (short)new Blittable2(buffer, ref offset);
        #endregion

        #region SqlInt32
        /// <summary>
        /// Serializes a <see cref="SqlInt32"/>.
        /// </summary>
        /// <param name="sqlValue">The <see cref="SqlInt32"/>.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        public static void SerializeSqlInt32([NotNull] object sqlValue, [NotNull] byte[] buffer, ref long offset)
            => VariableLengthEncoding.Encode(((SqlInt32)sqlValue).Value, buffer, ref offset);

        /// <summary>
        /// Deserializes a <see cref="SqlInt32"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="SqlInt32"/></returns>
        [NotNull]
        public static object DeserializeSqlInt32([NotNull] byte[] buffer, ref long offset)
            => new SqlInt32(VariableLengthEncoding.DecodeInt(buffer, ref offset));

        /// <summary>
        /// Deserializes a <see cref="int"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="int"/></returns>
        [NotNull]
        public static object DeserializeInt32([NotNull] byte[] buffer, ref long offset)
            => VariableLengthEncoding.DecodeInt(buffer, ref offset);
        #endregion

        #region SqlInt64
        /// <summary>
        /// Serializes a <see cref="SqlInt64"/>.
        /// </summary>
        /// <param name="sqlValue">The <see cref="SqlInt64"/>.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        public static void SerializeSqlInt64([NotNull] object sqlValue, [NotNull] byte[] buffer, ref long offset)
            => VariableLengthEncoding.Encode(((SqlInt64)sqlValue).Value, buffer, ref offset);

        /// <summary>
        /// Deserializes a <see cref="SqlInt64"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="SqlInt64"/></returns>
        [NotNull]
        public static object DeserializeSqlInt64([NotNull] byte[] buffer, ref long offset)
            => new SqlInt64(VariableLengthEncoding.DecodeLong(buffer, ref offset));

        /// <summary>
        /// Deserializes a <see cref="long"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="long"/></returns>
        [NotNull]
        public static object DeserializeInt64([NotNull] byte[] buffer, ref long offset)
            => VariableLengthEncoding.DecodeLong(buffer, ref offset);
        #endregion

        #region SqlSingle
        /// <summary>
        /// Serializes a <see cref="SqlSingle"/>.
        /// </summary>
        /// <param name="sqlValue">The <see cref="SqlSingle"/>.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        public static void SerializeSqlSingle([NotNull] object sqlValue, [NotNull] byte[] buffer, ref long offset)
            => VariableLengthEncoding.Encode((uint)(Blittable4)((SqlSingle)sqlValue).Value, buffer, ref offset);

        /// <summary>
        /// Deserializes a <see cref="SqlSingle"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="SqlSingle"/></returns>
        [NotNull]
        public static object DeserializeSqlSingle([NotNull] byte[] buffer, ref long offset)
            => new SqlSingle((Blittable4)VariableLengthEncoding.DecodeUInt(buffer, ref offset));

        /// <summary>
        /// Deserializes a <see cref="float"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="float"/></returns>
        [NotNull]
        public static object DeserializeFloat([NotNull] byte[] buffer, ref long offset)
            => (float)(Blittable4)VariableLengthEncoding.DecodeUInt(buffer, ref offset);
        #endregion

        #region SqlDouble
        /// <summary>
        /// Serializes a <see cref="SqlDouble"/>.
        /// </summary>
        /// <param name="sqlValue">The <see cref="SqlDouble"/>.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        public static void SerializeSqlDouble([NotNull] object sqlValue, [NotNull] byte[] buffer, ref long offset)
            => VariableLengthEncoding.Encode((ulong)(Blittable8)((SqlDouble)sqlValue).Value, buffer, ref offset);

        /// <summary>
        /// Deserializes a <see cref="SqlSingle"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="SqlSingle"/></returns>
        [NotNull]
        public static object DeserializeSqlDouble([NotNull] byte[] buffer, ref long offset)
            => new SqlDouble((Blittable8)VariableLengthEncoding.DecodeULong(buffer, ref offset));

        /// <summary>
        /// Deserializes a <see cref="double"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="double"/></returns>
        [NotNull]
        public static object DeserializeDouble([NotNull] byte[] buffer, ref long offset)
            => (double)(Blittable8)VariableLengthEncoding.DecodeULong(buffer, ref offset);
        #endregion

        #region SqlDecimal
        /// <summary>
        /// Serializes a <see cref="SqlDecimal"/>.
        /// </summary>
        /// <param name="sqlValue">The <see cref="SqlDecimal"/>.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        public static void SerializeSqlDecimal([NotNull] object sqlValue, [NotNull] byte[] buffer, ref long offset)
        {
            Blittable16 b = ((SqlDecimal)sqlValue).Value;
            VariableLengthEncoding.Encode(b.ULong0, buffer, ref offset);
            VariableLengthEncoding.Encode(b.ULong1, buffer, ref offset);
        }

        /// <summary>
        /// Deserializes a <see cref="SqlSingle"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="SqlSingle"/></returns>
        [NotNull]
        public static object DeserializeSqlDecimal([NotNull] byte[] buffer, ref long offset)
            => new SqlDecimal(
                new Blittable16(
                    VariableLengthEncoding.DecodeULong(buffer, ref offset),
                    VariableLengthEncoding.DecodeULong(buffer, ref offset)));

        /// <summary>
        /// Deserializes a <see cref="decimal"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="decimal"/></returns>
        [NotNull]
        public static object DeserializeDecimal([NotNull] byte[] buffer, ref long offset)
            => new Blittable16(
                VariableLengthEncoding.DecodeULong(buffer, ref offset),
                VariableLengthEncoding.DecodeULong(buffer, ref offset));
        #endregion

        #region SqlMoney
        /// <summary>
        /// Serializes a <see cref="SqlMoney"/>.
        /// </summary>
        /// <param name="sqlValue">The <see cref="SqlMoney"/>.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        public static void SerializeSqlMoney([NotNull] object sqlValue, [NotNull] byte[] buffer, ref long offset)
        {
            Blittable16 b = ((SqlMoney)sqlValue).Value;
            VariableLengthEncoding.Encode(b.ULong0, buffer, ref offset);
            VariableLengthEncoding.Encode(b.ULong1, buffer, ref offset);
        }

        /// <summary>
        /// Deserializes a <see cref="SqlSingle"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="SqlSingle"/></returns>
        [NotNull]
        public static object DeserializeSqlMoney([NotNull] byte[] buffer, ref long offset)
            => new SqlMoney(
                new Blittable16(
                    VariableLengthEncoding.DecodeULong(buffer, ref offset),
                    VariableLengthEncoding.DecodeULong(buffer, ref offset)));


        #endregion

        #region SqlDateTime
        /// <summary>
        /// Serializes a <see cref="SqlDateTime"/>.
        /// </summary>
        /// <param name="sqlValue">The <see cref="SqlDateTime"/>.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        public static void SerializeSqlDateTime([NotNull] object sqlValue, [NotNull] byte[] buffer, ref long offset)
            => VariableLengthEncoding.Encode((ulong)(Blittable8)((SqlDateTime)sqlValue).Value, buffer, ref offset);

        /// <summary>
        /// Serializes a <see cref="SqlDateTime"/>.
        /// </summary>
        /// <param name="sqlValue">The <see cref="SqlDateTime"/>.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        public static void SerializeDateTime([NotNull] object sqlValue, [NotNull] byte[] buffer, ref long offset)
            => VariableLengthEncoding.Encode((ulong)(Blittable8)(DateTime)sqlValue, buffer, ref offset);

        /// <summary>
        /// Deserializes a <see cref="SqlSingle"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="SqlSingle"/></returns>
        [NotNull]
        public static object DeserializeSqlDateTime([NotNull] byte[] buffer, ref long offset)
            => new SqlDateTime((Blittable8)VariableLengthEncoding.DecodeULong(buffer, ref offset));

        /// <summary>
        /// Deserializes a <see cref="double"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="double"/></returns>
        [NotNull]
        public static object DeserializeDateTime([NotNull] byte[] buffer, ref long offset)
            => (DateTime)(Blittable8)VariableLengthEncoding.DecodeULong(buffer, ref offset);
        #endregion

        #region DateTimeOffset
        /// <summary>
        /// Serializes a <see cref="DateTimeOffset"/>.
        /// </summary>
        /// <param name="sqlValue">The <see cref="DateTimeOffset"/>.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        public static void SerializeDateTimeOffset([NotNull] object sqlValue, [NotNull] byte[] buffer, ref long offset)
        {
            DateTimeOffset d = (DateTimeOffset)sqlValue;
            Blittable2 o = (short)(d.Offset.Ticks / TimeSpan.TicksPerMinute);
            buffer[offset++] = o.Byte0;
            buffer[offset++] = o.Byte1;
            VariableLengthEncoding.Encode((ulong)d.Ticks, buffer, ref offset);
        }

        /// <summary>
        /// Deserializes a <see cref="SqlSingle"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="SqlSingle"/></returns>
        [NotNull]
        public static object DeserializeDateTimeOffset([NotNull] byte[] buffer, ref long offset)
        {
            short o = new Blittable2(buffer[offset++], buffer[offset++]);
            return new DateTimeOffset(
                (long)VariableLengthEncoding.DecodeULong(buffer, ref offset),
                new TimeSpan(o * TimeSpan.TicksPerMinute));
        }
        #endregion

        #region TimeSpan
        /// <summary>
        /// Serializes a <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="sqlValue">The <see cref="TimeSpan"/>.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        public static void SerializeTimeSpan([NotNull] object sqlValue, [NotNull] byte[] buffer, ref long offset)
            => VariableLengthEncoding.Encode((ulong)(Blittable8)(TimeSpan)sqlValue, buffer, ref offset);

        /// <summary>
        /// Deserializes a <see cref="SqlSingle"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="SqlSingle"/></returns>
        [NotNull]
        public static object DeserializeTimeSpan([NotNull] byte[] buffer, ref long offset)
            => (TimeSpan)(Blittable8)VariableLengthEncoding.DecodeULong(buffer, ref offset);
        #endregion

        #region SqlGuid
        /// <summary>
        /// Serializes a <see cref="SqlGuid"/>.
        /// </summary>
        /// <param name="sqlValue">The <see cref="SqlGuid"/>.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        public static void SerializeSqlGuid([NotNull] object sqlValue, [NotNull] byte[] buffer, ref long offset)
        {
            Blittable16 b = ((SqlGuid)sqlValue).Value;
            buffer[offset++] = b.Byte0;
            buffer[offset++] = b.Byte1;
            buffer[offset++] = b.Byte2;
            buffer[offset++] = b.Byte3;
            buffer[offset++] = b.Byte4;
            buffer[offset++] = b.Byte5;
            buffer[offset++] = b.Byte6;
            buffer[offset++] = b.Byte7;
            buffer[offset++] = b.Byte8;
            buffer[offset++] = b.Byte9;
            buffer[offset++] = b.Byte10;
            buffer[offset++] = b.Byte11;
            buffer[offset++] = b.Byte12;
            buffer[offset++] = b.Byte13;
            buffer[offset++] = b.Byte14;
            buffer[offset++] = b.Byte15;
        }

        /// <summary>
        /// Deserializes a <see cref="SqlSingle"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="SqlSingle"/></returns>
        [NotNull]
        public static object DeserializeSqlGuid([NotNull] byte[] buffer, ref long offset)
            => new SqlGuid(
                new Blittable16(
                    buffer[offset++],
                    buffer[offset++],
                    buffer[offset++],
                    buffer[offset++],
                    buffer[offset++],
                    buffer[offset++],
                    buffer[offset++],
                    buffer[offset++],
                    buffer[offset++],
                    buffer[offset++],
                    buffer[offset++],
                    buffer[offset++],
                    buffer[offset++],
                    buffer[offset++],
                    buffer[offset++],
                    buffer[offset++]));

        /// <summary>
        /// Deserializes a <see cref="decimal"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="decimal"/></returns>
        [NotNull]
        public static object DeserializeGuid([NotNull] byte[] buffer, ref long offset)
            => (Guid)new Blittable16(
                buffer[offset++],
                buffer[offset++],
                buffer[offset++],
                buffer[offset++],
                buffer[offset++],
                buffer[offset++],
                buffer[offset++],
                buffer[offset++],
                buffer[offset++],
                buffer[offset++],
                buffer[offset++],
                buffer[offset++],
                buffer[offset++],
                buffer[offset++],
                buffer[offset++],
                buffer[offset++]);
        #endregion

        #region SqlBinary
        /// <summary>
        /// Serializes a <see cref="SqlBinary"/>.
        /// </summary>
        /// <param name="sqlValue">The <see cref="SqlBinary"/>.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        public static void SerializeSqlBinary([NotNull] object sqlValue, [NotNull] byte[] buffer, ref long offset)
        {
            SqlBinary b = (SqlBinary)sqlValue;
            int length = b.Length;
            VariableLengthEncoding.Encode((uint)length, buffer, ref offset);
            for (int i = 0; i < length; i++)
                buffer[offset++] = b[i];
        }

        /// <summary>
        /// Deserializes a <see cref="SqlSingle"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="SqlSingle"/></returns>
        [NotNull]
        public static object DeserializeSqlBinary([NotNull] byte[] buffer, ref long offset)
        {
            int length = (int)VariableLengthEncoding.DecodeUInt(buffer, ref offset);
            byte[] bytes = new byte[length];
            Array.Copy(buffer, offset, bytes, 0, length);
            offset += length;
            return new SqlBinary(bytes);
        }

        /// <summary>
        /// Deserializes a <see cref="decimal"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="decimal"/></returns>
        [NotNull]
        public static object DeserializeByteArray([NotNull] byte[] buffer, ref long offset)
        {
            int length = (int)VariableLengthEncoding.DecodeUInt(buffer, ref offset);
            byte[] bytes = new byte[length];
            Array.Copy(buffer, offset, bytes, 0, length);
            return bytes;
        }
        #endregion

        #region SqlString
        /// <summary>
        /// Serializes a <see cref="SqlString"/>.
        /// </summary>
        /// <param name="sqlValue">The <see cref="SqlString"/>.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        public static void SerializeAnsiSqlString([NotNull] object sqlValue, [NotNull] byte[] buffer, ref long offset)
        {
            SqlString s = (SqlString)sqlValue;
            VariableLengthEncoding.Encode(s.LCID, buffer, ref offset);
            VariableLengthEncoding.Encode((uint)s.SqlCompareOptions, buffer, ref offset);
            string str = s.Value;
            Encoding encoding = Encoding.GetEncoding(new CultureInfo(s.LCID).TextInfo.ANSICodePage);
            // ReSharper disable AssignNullToNotNullAttribute, PossibleNullReferenceException
            uint length = (uint)encoding.GetByteCount(str);
            VariableLengthEncoding.Encode(length, buffer, ref offset);
            if (offset + length <= int.MaxValue)
                offset += encoding.GetBytes(str, 0, str.Length, buffer, (int)offset);
            else
            {
                byte[] bytes = encoding.GetBytes(str);
                long longLength = bytes.LongLength;
                Array.Copy(bytes, 0, buffer, offset, longLength);
                offset += longLength;
            }
            // ReSharper restore AssignNullToNotNullAttribute, PossibleNullReferenceException
        }

        /// <summary>
        /// Serializes a <see cref="SqlString"/>.
        /// </summary>
        /// <param name="sqlValue">The <see cref="SqlString"/>.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        public static void SerializeSqlString([NotNull] object sqlValue, [NotNull] byte[] buffer, ref long offset)
        {
            SqlString s = (SqlString)sqlValue;
            VariableLengthEncoding.Encode(s.LCID, buffer, ref offset);
            VariableLengthEncoding.Encode((uint)s.SqlCompareOptions, buffer, ref offset);
            string str = s.Value;
            Encoding encoding = Encoding.UTF8;
            // ReSharper disable AssignNullToNotNullAttribute, PossibleNullReferenceException
            uint length = (uint)encoding.GetByteCount(str);
            VariableLengthEncoding.Encode(length, buffer, ref offset);
            if (offset + length <= int.MaxValue)
                offset += encoding.GetBytes(str, 0, str.Length, buffer, (int)offset);
            else
            {
                byte[] bytes = encoding.GetBytes(str);
                long longLength = bytes.LongLength;
                Array.Copy(bytes, 0, buffer, offset, longLength);
                offset += longLength;
            }
            // ReSharper restore AssignNullToNotNullAttribute, PossibleNullReferenceException
        }

        /// <summary>
        /// Serializes a <see cref="SqlXml"/>.
        /// </summary>
        /// <param name="sqlValue">The <see cref="SqlXml"/>.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        public static void SerializeString([NotNull] object sqlValue, [NotNull] byte[] buffer, ref long offset)
        {
            string str = (string)sqlValue;
            Encoding encoding = Encoding.UTF8;
            // ReSharper disable AssignNullToNotNullAttribute, PossibleNullReferenceException
            uint length = (uint)encoding.GetByteCount(str);
            VariableLengthEncoding.Encode(length, buffer, ref offset);
            if (offset + length <= int.MaxValue)
                offset += encoding.GetBytes(str, 0, str.Length, buffer, (int)offset);
            else
            {
                byte[] bytes = encoding.GetBytes(str);
                long longLength = bytes.LongLength;
                Array.Copy(bytes, 0, buffer, offset, longLength);
                offset += longLength;
            }
            // ReSharper restore AssignNullToNotNullAttribute, PossibleNullReferenceException
        }

        /// <summary>
        /// Deserializes a <see cref="SqlSingle"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="SqlSingle"/></returns>
        [NotNull]
        public static object DeserializeAnsiSqlString([NotNull] byte[] buffer, ref long offset)
        {
            int lcid = VariableLengthEncoding.DecodeInt(buffer, ref offset);
            SqlCompareOptions options = (SqlCompareOptions)VariableLengthEncoding.DecodeUInt(buffer, ref offset);
            int length = (int)VariableLengthEncoding.DecodeUInt(buffer, ref offset);

            SqlString result;
            if (offset + length <= int.MaxValue)
                result = new SqlString(lcid, options, buffer, (int)offset, length, false);
            else
            {
                byte[] bytes = new byte[length];
                Array.Copy(buffer, offset, bytes, 0, length);
                result = new SqlString(lcid, options, bytes, 0, length, false);
            }
            offset += length;
            return result;
        }

        /// <summary>
        /// Deserializes a <see cref="SqlSingle"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="SqlSingle"/></returns>
        [NotNull]
        public static object DeserializeSqlString([NotNull] byte[] buffer, ref long offset)
        {
            int lcid = VariableLengthEncoding.DecodeInt(buffer, ref offset);
            SqlCompareOptions options = (SqlCompareOptions)VariableLengthEncoding.DecodeUInt(buffer, ref offset);
            int length = (int)VariableLengthEncoding.DecodeUInt(buffer, ref offset);
            byte[] bytes;
            if (offset + length <= int.MaxValue)
                bytes = Encoding.Convert(Encoding.UTF8, Encoding.Unicode, buffer, (int)offset, length);
            else
            {
                byte[] b = new byte[length];
                Array.Copy(buffer, offset, b, 0, length);
                bytes = Encoding.Convert(Encoding.UTF8, Encoding.Unicode, b, 0, length);
            }
            offset += length;
            return new SqlString(lcid, options, bytes, true);
        }

        /// <summary>
        /// Deserializes a <see cref="string"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="string"/></returns>
        [NotNull]
        public static object DeserializeAnsiString([NotNull] byte[] buffer, ref long offset)
        {
            int lcid = VariableLengthEncoding.DecodeInt(buffer, ref offset);
            VariableLengthEncoding.DecodeUInt(buffer, ref offset);
            Encoding encoding = Encoding.GetEncoding(new CultureInfo(lcid).TextInfo.ANSICodePage);
            int length = (int)VariableLengthEncoding.DecodeUInt(buffer, ref offset);

            string result;
            if (offset + length <= int.MaxValue)
                result = encoding.GetString(buffer, (int)offset, length);
            else
            {
                byte[] bytes = new byte[length];
                Array.Copy(buffer, offset, bytes, 0, length);
                result = encoding.GetString(buffer, 0, length);
            }
            offset += length;
            return result;
        }

        /// <summary>
        /// Deserializes a <see cref="string"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="string"/></returns>
        [NotNull]
        public static object DeserializeString([NotNull] byte[] buffer, ref long offset)
        {
            VariableLengthEncoding.DecodeInt(buffer, ref offset);
            VariableLengthEncoding.DecodeUInt(buffer, ref offset);
            Encoding encoding = Encoding.UTF8;
            int length = (int)VariableLengthEncoding.DecodeUInt(buffer, ref offset);

            string result;
            if (offset + length <= int.MaxValue)
                result = encoding.GetString(buffer, (int)offset, length);
            else
            {
                byte[] bytes = new byte[length];
                Array.Copy(buffer, offset, bytes, 0, length);
                result = encoding.GetString(buffer, 0, length);
            }
            offset += length;
            return result;
        }
        #endregion

        #region SqlXml
        /// <summary>
        /// Serializes a <see cref="SqlXml"/>.
        /// </summary>
        /// <param name="sqlValue">The <see cref="SqlXml"/>.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        public static void SerializeSqlXml([NotNull] object sqlValue, [NotNull] byte[] buffer, ref long offset)
        {
            string str = ((SqlXml)sqlValue).Value;
            Encoding encoding = Encoding.UTF8;
            // ReSharper disable AssignNullToNotNullAttribute, PossibleNullReferenceException
            uint length = (uint)encoding.GetByteCount(str);
            VariableLengthEncoding.Encode(length, buffer, ref offset);
            if (offset + length <= int.MaxValue)
                offset += encoding.GetBytes(str, 0, str.Length, buffer, (int)offset);
            else
            {
                byte[] bytes = encoding.GetBytes(str);
                long longLength = bytes.LongLength;
                Array.Copy(bytes, 0, buffer, offset, longLength);
                offset += longLength;
            }
            // ReSharper restore AssignNullToNotNullAttribute, PossibleNullReferenceException
        }

        /// <summary>
        /// Deserializes a <see cref="SqlSingle"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="SqlSingle"/></returns>
        [NotNull]
        public static object DeserializeSqlXml([NotNull] byte[] buffer, ref long offset)
        {
            int length = (int)VariableLengthEncoding.DecodeUInt(buffer, ref offset);
            byte[] bytes;
            if (offset + length <= int.MaxValue)
                bytes = Encoding.Convert(Encoding.UTF8, Encoding.Unicode, buffer, (int)offset, length);
            else
            {
                byte[] b = new byte[length];
                Array.Copy(buffer, offset, b, 0, length);
                bytes = Encoding.Convert(Encoding.UTF8, Encoding.Unicode, b, 0, length);
            }
            offset += length;
            using (MemoryStream ms = new MemoryStream(bytes))
                return new SqlXml(ms);
        }

        /// <summary>
        /// Deserializes a <see cref="string"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="string"/></returns>
        [NotNull]
        public static object DeserializeXmlString([NotNull] byte[] buffer, ref long offset)
        {
            Encoding encoding = Encoding.UTF8;
            int length = (int)VariableLengthEncoding.DecodeUInt(buffer, ref offset);

            string result;
            if (offset + length <= int.MaxValue)
                result = encoding.GetString(buffer, (int)offset, length);
            else
            {
                byte[] bytes = new byte[length];
                Array.Copy(buffer, offset, bytes, 0, length);
                result = encoding.GetString(buffer, 0, length);
            }
            offset += length;
            return result;
        }
        #endregion

        #region UDT
        /// <summary>
        /// SQL User Defined Types, with special casing for performance.
        /// </summary>
        private enum UdtType : byte
        {
            BinarySerialize = 0,
            Geography = 1,
            GeographyNull = 2,
            GeographyZero = 3,
            Geometry = 8,
            GeometryNull = 9,
            GeometryZero = 10,
            HierarchyId = 16,
            HierarchyIdNull = 17,
            Serializable = 128
        }

        /// <summary>
        /// The earth SRID.
        /// </summary>
        public const int EarthSrid = 4326;

        /// <summary>
        /// The geography zero point (on <see cref="EarthSrid">Earth</see>).
        /// </summary>
        [NotNull]
        // ReSharper disable once AssignNullToNotNullAttribute
        public static SqlGeography GeographyZeroPoint = SqlGeography.Point(0D, 0D, EarthSrid);

        /// <summary>
        /// The geometry zero point (on <see cref="EarthSrid">Earth</see>).
        /// </summary>
        [NotNull]
        // ReSharper disable once AssignNullToNotNullAttribute
        public static SqlGeometry GeometryZeroPoint = SqlGeometry.Point(0D, 0D, EarthSrid);

        /// <summary>
        /// The <see cref="SqlGeography"/> simple full name.
        /// </summary>
        [NotNull]
        private static readonly string _sqlGeographySimpleFullName =
            ExtendedType.Get(typeof(SqlGeography)).SimpleFullName;

        /// <summary>
        /// The <see cref="SqlGeometry"/> simple full name.
        /// </summary>
        [NotNull]
        private static readonly string _sqlGeometrySimpleFullName = ExtendedType.Get(typeof(SqlGeometry)).SimpleFullName;

        /// <summary>
        /// The <see cref="SqlHierarchyId"/> simple full name.
        /// </summary>
        [NotNull]
        private static readonly string _sqlHierarchyIdSimpleFullName =
            ExtendedType.Get(typeof(SqlHierarchyId)).SimpleFullName;

        /// <summary>
        /// Serializes a UDT.
        /// </summary>
        /// <param name="sqlValue">The UDT.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        public static void SerializeUdt([NotNull] object sqlValue, [NotNull] byte[] buffer, ref long offset)
        {
            IBinarySerialize value = sqlValue as IBinarySerialize;
            if (value != null)
            {
                // The UDT is Format.UserDefined
                // (see https://msdn.microsoft.com/en-us/library/microsoft.sqlserver.server.format(v=vs.110).aspx)

                // Get the type
                ExtendedType et = value.GetType();

                if (string.Equals(et.SimpleFullName, _sqlGeographySimpleFullName))
                {
                    SqlGeography geography = (SqlGeography)sqlValue;

                    // Special case for null
                    if (geography.IsNull)
                    {
                        buffer[offset++] = (byte)UdtType.GeographyNull;
                        return;
                    }

                    // Special case for Zero point
                    if (!geography.HasM &&
                        !geography.HasZ &&
                        !geography.Lat.IsNull &&
                        !geography.Long.IsNull &&
                        !geography.STSrid.IsNull &&
                        // ReSharper disable CompareOfFloatsByEqualityOperator
                        geography.Lat.Value == 0D &&
                        geography.Long.Value == 0D &&
                        // ReSharper restore CompareOfFloatsByEqualityOperator
                        geography.STSrid.Value == EarthSrid)
                    {
                        buffer[offset++] = (byte)UdtType.GeographyZero;
                        return;
                    }

                    buffer[offset++] = (byte)UdtType.Geography;
                }
                else if (string.Equals(et.SimpleFullName, _sqlGeometrySimpleFullName))
                {
                    SqlGeometry geometry = (SqlGeometry)sqlValue;

                    // Special case for null
                    if (geometry.IsNull)
                    {
                        buffer[offset++] = (byte)UdtType.GeometryNull;
                        return;
                    }

                    // Special case for Zero point
                    if (!geometry.HasM &&
                        !geometry.HasZ &&
                        !geometry.STX.IsNull &&
                        !geometry.STY.IsNull &&
                        !geometry.STSrid.IsNull &&
                        // ReSharper disable CompareOfFloatsByEqualityOperator
                        geometry.STX.Value == 0D &&
                        geometry.STY.Value == 0D &&
                        // ReSharper restore CompareOfFloatsByEqualityOperator
                        geometry.STSrid.Value == EarthSrid)
                    {
                        buffer[offset++] = (byte)UdtType.GeometryZero;
                        return;
                    }

                    buffer[offset++] = (byte)UdtType.Geometry;
                }
                else if (string.Equals(et.SimpleFullName, _sqlHierarchyIdSimpleFullName))
                {
                    SqlHierarchyId hierarchyId = (SqlHierarchyId)sqlValue;

                    // Special case for null
                    if (hierarchyId.IsNull)
                    {
                        buffer[offset++] = (byte)UdtType.HierarchyIdNull;
                        return;
                    }

                    buffer[offset++] = (byte)UdtType.HierarchyId;
                }
                else
                {
                    // Mark format
                    buffer[offset++] = (byte)UdtType.BinarySerialize;

                    // Write the simplified type name so we can recreate the type later
                    SerializeString(et.SimpleFullName, buffer, ref offset);
                }

                // Serialize object
                using (MemoryStream ms = new MemoryStream())
                {
                    using (BinaryWriter writer = new BinaryWriter(ms))
                        value.Write(writer);

                    // Encode stream length
                    long length = ms.Length;
                    VariableLengthEncoding.Encode((ulong)length, buffer, ref offset);

                    // Return to start
                    ms.Seek(0, SeekOrigin.Begin);

                    if (offset + length <= int.MaxValue)
                        ms.Write(buffer, (int)offset, (int)length);
                    else
                    {
                        byte[] bytes = ms.GetBuffer();
                        // ReSharper disable once PossibleNullReferenceException
                        long longLength = bytes.LongLength;
                        Array.Copy(bytes, 0, buffer, offset, longLength);
                        offset += longLength;
                    }
                    return;
                }
            }

            // We have to assume the object is serializable
            buffer[offset++] = (byte)UdtType.Serializable;

            using (MemoryStream ms = new MemoryStream())
            {
                Serialization.Serialize.GetFormatter().Serialize(ms, sqlValue);

                // Encode stream length
                long length = ms.Length;
                VariableLengthEncoding.Encode((ulong)length, buffer, ref offset);

                // Return to start
                ms.Seek(0, SeekOrigin.Begin);

                if (offset + length <= int.MaxValue)
                    ms.Write(buffer, (int)offset, (int)length);
                else
                {
                    byte[] bytes = ms.GetBuffer();
                    // ReSharper disable once PossibleNullReferenceException
                    long longLength = bytes.LongLength;
                    Array.Copy(bytes, 0, buffer, offset, longLength);
                    offset += longLength;
                }
            }
        }

        /// <summary>
        /// Deserializes a UDT.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized UDT.</returns>
        [NotNull]
        public static object DeserializeUdt([NotNull] byte[] buffer, ref long offset)
        {
            // Get the type and create the binary object 
            UdtType udtType = (UdtType)buffer[offset++];
            IBinarySerialize binaryObject;
            switch (udtType)
            {
                case UdtType.BinarySerialize:
                    binaryObject = (IBinarySerialize)Activator.CreateInstance(
                        // ReSharper disable once AssignNullToNotNullAttribute
                        ExtendedType.FindType(
                            (string)DeserializeString(buffer, ref offset),
                            false,
                            true));
                    break;
                case UdtType.Geography:
                    binaryObject = new SqlGeography();
                    break;
                case UdtType.GeographyNull:
                    // ReSharper disable once AssignNullToNotNullAttribute
                    return SqlGeography.Null;
                case UdtType.GeographyZero:
                    return GeographyZeroPoint;
                case UdtType.Geometry:
                    binaryObject = new SqlGeometry();
                    break;
                case UdtType.GeometryNull:
                    // ReSharper disable once AssignNullToNotNullAttribute
                    return SqlGeometry.Null;
                case UdtType.GeometryZero:
                    return GeometryZeroPoint;
                case UdtType.HierarchyId:
                    binaryObject = new SqlHierarchyId();
                    break;
                case UdtType.HierarchyIdNull:
                    return SqlHierarchyId.Null;
                default:
                    binaryObject = null;
                    break;
            }

            // Get data length
            long length = (long)VariableLengthEncoding.DecodeULong(buffer, ref offset);

            // Make 32-bit safe
            byte[] b;
            int o;
            int l = checked ((int)length);
            if (offset + length > int.MaxValue)
            {
                b = new byte[length];
                o = 0;
                Array.Copy(buffer, offset, b, 0L, length);
            }
            else
            {
                b = buffer;
                o = checked ((int)offset);
            }

            // Update offset
            offset += length;

            using (MemoryStream ms = new MemoryStream(b, o, l))
            {
                // Deserialize object
                if (binaryObject == null) return Serialization.Serialize.GetFormatter().Deserialize(ms);

                // Deserialize data into binary object
                using (BinaryReader reader = new BinaryReader(ms))
                    binaryObject.Read(reader);
                return binaryObject;
            }
        }
        #endregion

        #region Variant
        /// <summary>
        /// Serializes a Variant.
        /// </summary>
        /// <param name="sqlValue">The UDT.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        public static void SerializeVariant([NotNull] object sqlValue, [NotNull] byte[] buffer, ref long offset)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deserializes a Variant.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized Variant.</returns>
        [NotNull]
        public static object DeserializeVariant([NotNull] byte[] buffer, ref long offset)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Structured
        /// <summary>
        /// Serializes a Structured.
        /// </summary>
        /// <param name="sqlValue">The UDT.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        public static void SerializeStructured([NotNull] object sqlValue, [NotNull] byte[] buffer, ref long offset)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deserializes a Structured.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized Structured.</returns>
        [NotNull]
        public static object DeserializeStructured([NotNull] byte[] buffer, ref long offset)
        {
            throw new NotImplementedException();
        }
        #endregion

        #endregion
    }
}