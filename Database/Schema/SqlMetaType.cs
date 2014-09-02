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
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using JetBrains.Annotations;
using Microsoft.SqlServer.Server;
using WebApplications.Utilities.Database.Exceptions;

namespace WebApplications.Utilities.Database.Schema
{
    /// <summary>
    ///   Holds information about known SQL Types.
    /// </summary>
    public sealed class SqlMetaType
    {
        #region Pre-defined meta types
        /// <summary>
        ///   The meta data for the <c>bigint</c> data type.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly SqlMetaType MetaBigInt = new SqlMetaType(
            19,
            byte.MaxValue,
            8,
            true,
            false,
            false,
            127,
            38,
            "bigint",
            typeof (long),
            typeof (SqlInt64),
            SqlDbType.BigInt,
            DbType.Int64,
            0);

        /// <summary>
        ///   The meta data for the <c>float</c> data type.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly SqlMetaType MetaFloat = new SqlMetaType(
            15,
            byte.MaxValue,
            8,
            true,
            false,
            false,
            62,
            109,
            "float",
            typeof (double),
            typeof (SqlDouble),
            SqlDbType.Float,
            DbType.Double,
            0);

        /// <summary>
        ///   The meta data for the <c>real</c> data type.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly SqlMetaType MetaReal = new SqlMetaType(
            7,
            byte.MaxValue,
            4,
            true,
            false,
            false,
            59,
            109,
            "real",
            typeof (float),
            typeof (SqlSingle),
            SqlDbType.Real,
            DbType.Single,
            0);

        /// <summary>
        ///   The meta data for the <c>binary</c> data type.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly SqlMetaType MetaBinary = new SqlMetaType(
            byte.MaxValue,
            byte.MaxValue,
            -1,
            false,
            false,
            false,
            173,
            173,
            "binary",
            typeof (byte[]),
            typeof (SqlBinary),
            SqlDbType.Binary,
            DbType.Binary,
            2);

        /// <summary>
        ///   The meta data for the <c>timestamp</c> data type.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly SqlMetaType MetaTimestamp = new SqlMetaType(
            byte.MaxValue,
            byte.MaxValue,
            -1,
            false,
            false,
            false,
            173,
            173,
            "timestamp",
            typeof (byte[]),
            typeof (SqlBinary),
            SqlDbType.
                Timestamp,
            DbType.Binary,
            2);

        /// <summary>
        ///   The meta data for the <c>varbinary</c> data type.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly SqlMetaType MetaVarBinary = new SqlMetaType(
            byte.MaxValue,
            byte.MaxValue,
            -1,
            false,
            false,
            false,
            165,
            165,
            "varbinary",
            typeof (byte[]),
            typeof (SqlBinary),
            SqlDbType.
                VarBinary,
            DbType.Binary,
            2);

        /// <summary>
        ///   The meta data for the <c>varbinary(max)</c> data type (a large value data type).
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly SqlMetaType MetaMaxVarBinary = new SqlMetaType(
            byte.MaxValue,
            byte.MaxValue,
            -1,
            false,
            true,
            true,
            165,
            165,
            "varbinary",
            typeof (byte[]),
            typeof (
                SqlBinary),
            SqlDbType.
                VarBinary,
            DbType.Binary,
            2);

        /// <summary>
        ///   The meta data for a small <c>varbinary</c> data type.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly SqlMetaType MetaSmallVarBinary = new SqlMetaType(
            byte.MaxValue,
            byte.MaxValue,
            -1,
            false,
            false,
            false,
            37,
            173,
            string.Empty,
            typeof (byte[]),
            typeof (SqlBinary),
            (SqlDbType) 24,
            DbType.Binary,
            2);

        /// <summary>
        ///   The meta data for the <c>image</c> data type (a large object data type).
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly SqlMetaType MetaImage = new SqlMetaType(
            byte.MaxValue,
            byte.MaxValue,
            -1,
            false,
            true,
            false,
            34,
            34,
            "image",
            typeof (byte[]),
            typeof (SqlBinary),
            SqlDbType.Image,
            DbType.Binary,
            0);

        /// <summary>
        ///   The meta data for the <c>bit</c> data type.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly SqlMetaType MetaBit = new SqlMetaType(
            byte.MaxValue,
            byte.MaxValue,
            1,
            true,
            false,
            false,
            50,
            104,
            "bit",
            typeof (bool),
            typeof (SqlBoolean),
            SqlDbType.Bit,
            DbType.Boolean,
            0);

        /// <summary>
        ///   The meta data for the <c>tinyint</c> data type.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly SqlMetaType MetaTinyInt = new SqlMetaType(
            3,
            byte.MaxValue,
            1,
            true,
            false,
            false,
            48,
            38,
            "tinyint",
            typeof (byte),
            typeof (SqlByte),
            SqlDbType.TinyInt,
            DbType.Byte,
            0);

        /// <summary>
        ///   The meta data for the <c>smallint</c> data type.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly SqlMetaType MetaSmallInt = new SqlMetaType(
            5,
            byte.MaxValue,
            2,
            true,
            false,
            false,
            52,
            38,
            "smallint",
            typeof (short),
            typeof (SqlInt16),
            SqlDbType.SmallInt,
            DbType.Int16,
            0);

        /// <summary>
        ///   The meta data for the <c>int</c> data type.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly SqlMetaType MetaInt = new SqlMetaType(
            10,
            byte.MaxValue,
            4,
            true,
            false,
            false,
            56,
            38,
            "int",
            typeof (int),
            typeof (SqlInt32),
            SqlDbType.Int,
            DbType.Int32,
            0);

        /// <summary>
        ///   The meta data for the <c>char</c> data type.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly SqlMetaType MetaChar = new SqlMetaType(
            byte.MaxValue,
            byte.MaxValue,
            -1,
            false,
            false,
            false,
            175,
            175,
            "char",
            typeof (string),
            typeof (SqlString),
            SqlDbType.Char,
            DbType.
                AnsiStringFixedLength,
            7);

        /// <summary>
        ///   The meta data for the <c>varchar</c> data type.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly SqlMetaType MetaVarChar = new SqlMetaType(
            byte.MaxValue,
            byte.MaxValue,
            -1,
            false,
            false,
            false,
            167,
            167,
            "varchar",
            typeof (string),
            typeof (SqlString),
            SqlDbType.VarChar,
            DbType.AnsiString,
            7);

        /// <summary>
        ///   The meta data for the <c>varchar(max)</c> data type (a large value data type).
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly SqlMetaType MetaMaxVarChar = new SqlMetaType(
            byte.MaxValue,
            byte.MaxValue,
            -1,
            false,
            true,
            true,
            167,
            167,
            "varchar",
            typeof (string),
            typeof (SqlString
                ),
            SqlDbType.VarChar,
            DbType.AnsiString,
            7);

        /// <summary>
        ///   The meta data for the <c>text</c> data type (a large object data type).
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly SqlMetaType MetaText = new SqlMetaType(
            byte.MaxValue,
            byte.MaxValue,
            -1,
            false,
            true,
            false,
            35,
            35,
            "text",
            typeof (string),
            typeof (SqlString),
            SqlDbType.Text,
            DbType.AnsiString,
            0);

        /// <summary>
        ///   The meta data for the <c>nchar</c> data type.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly SqlMetaType MetaNChar = new SqlMetaType(
            byte.MaxValue,
            byte.MaxValue,
            -1,
            false,
            false,
            false,
            239,
            239,
            "nchar",
            typeof (string),
            typeof (SqlString),
            SqlDbType.NChar,
            DbType.
                StringFixedLength,
            7);

        /// <summary>
        ///   The meta data for the <c>nvarchar</c> data type.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly SqlMetaType MetaNVarChar = new SqlMetaType(
            byte.MaxValue,
            byte.MaxValue,
            -1,
            false,
            false,
            false,
            231,
            231,
            "nvarchar",
            typeof (string),
            typeof (SqlString),
            SqlDbType.NVarChar,
            DbType.String,
            7);

        /// <summary>
        ///   The meta data for the <c>nvarchar(max)</c> data type (a large value data type).
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly SqlMetaType MetaMaxNVarChar = new SqlMetaType(
            byte.MaxValue,
            byte.MaxValue,
            -1,
            false,
            true,
            true,
            231,
            231,
            "nvarchar",
            typeof (string),
            typeof (
                SqlString),
            SqlDbType.
                NVarChar,
            DbType.String,
            7);

        /// <summary>
        ///   The meta data for the <c>ntext</c> data type (a large object data type).
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly SqlMetaType MetaNText = new SqlMetaType(
            byte.MaxValue,
            byte.MaxValue,
            -1,
            false,
            true,
            false,
            99,
            99,
            "ntext",
            typeof (string),
            typeof (SqlString),
            SqlDbType.NText,
            DbType.String,
            7);

        /// <summary>
        ///   The meta data for the <c>decimal</c> data type.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly SqlMetaType MetaDecimal = new SqlMetaType(
            38,
            4,
            17,
            true,
            false,
            false,
            108,
            108,
            "decimal",
            typeof (Decimal),
            typeof (SqlDecimal),
            SqlDbType.Decimal,
            DbType.Decimal,
            2);

        /// <summary>
        ///   The meta data for the <c>xml</c> data type (a large object data type).
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly SqlMetaType MetaXml = new SqlMetaType(
            byte.MaxValue,
            byte.MaxValue,
            -1,
            false,
            true,
            true,
            241,
            241,
            "xml",
            typeof (string),
            typeof (SqlXml),
            SqlDbType.Xml,
            DbType.Xml,
            0);

        /// <summary>
        ///   The meta data for the <c>datetime</c> data type.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly SqlMetaType MetaDateTime = new SqlMetaType(
            23,
            3,
            8,
            true,
            false,
            false,
            61,
            111,
            "datetime",
            typeof (DateTime),
            typeof (SqlDateTime
                ),
            SqlDbType.DateTime,
            DbType.DateTime,
            0);

        /// <summary>
        ///   The meta data for the <c>smalldatetime</c> type.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly SqlMetaType MetaSmallDateTime = new SqlMetaType(
            16,
            0,
            4,
            true,
            false,
            false,
            58,
            111,
            "smalldatetime",
            typeof (
                DateTime),
            typeof (
                SqlDateTime
                ),
            SqlDbType.
                SmallDateTime,
            DbType.
                DateTime,
            0);

        /// <summary>
        ///   The meta data for the <c>money</c> data type.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly SqlMetaType MetaMoney = new SqlMetaType(
            19,
            byte.MaxValue,
            8,
            true,
            false,
            false,
            60,
            110,
            "money",
            typeof (Decimal),
            typeof (SqlMoney),
            SqlDbType.Money,
            DbType.Currency,
            0);

        /// <summary>
        ///   The meta data for the <c>smallmoney</c> data type.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly SqlMetaType MetaSmallMoney = new SqlMetaType(
            10,
            byte.MaxValue,
            4,
            true,
            false,
            false,
            122,
            110,
            "smallmoney",
            typeof (Decimal),
            typeof (SqlMoney),
            SqlDbType.
                SmallMoney,
            DbType.Currency,
            0);

        /// <summary>
        ///   The meta data for the <c>uniqueidentifier</c> data type.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly SqlMetaType MetaUniqueId = new SqlMetaType(
            byte.MaxValue,
            byte.MaxValue,
            16,
            true,
            false,
            false,
            36,
            36,
            "uniqueidentifier",
            typeof (Guid),
            typeof (SqlGuid),
            SqlDbType.
                UniqueIdentifier,
            DbType.Guid,
            0);

        /// <summary>
        ///   The meta data for the <c>sql_variant</c> data type.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly SqlMetaType MetaVariant = new SqlMetaType(
            byte.MaxValue,
            byte.MaxValue,
            -1,
            true,
            false,
            false,
            98,
            98,
            "sql_variant",
            typeof (object),
            typeof (object),
            SqlDbType.Variant,
            DbType.Object,
            0);

        /// <summary>
        ///   The meta data for user-defined data types.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly SqlMetaType MetaUdt = new SqlMetaType(
            byte.MaxValue,
            byte.MaxValue,
            -1,
            false,
            false,
            true,
            240,
            240,
            "udt",
            typeof (object),
            typeof (object),
            SqlDbType.Udt,
            DbType.Object,
            0);

        /// <summary>
        ///   The meta data for user-defined data types.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly SqlMetaType MetaMaxUdt = new SqlMetaType(
            byte.MaxValue,
            byte.MaxValue,
            -1,
            false,
            true,
            true,
            240,
            240,
            "udt",
            typeof (object),
            typeof (object),
            SqlDbType.Udt,
            DbType.Object,
            0);

        /// <summary>
        ///   The meta data for the <c>table</c> data type.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly SqlMetaType MetaTable = new SqlMetaType(
            byte.MaxValue,
            byte.MaxValue,
            -1,
            false,
            false,
            false,
            243,
            243,
            "table",
            typeof (
                IEnumerable
                <DbDataRecord>),
            typeof (
                IEnumerable
                <DbDataRecord>),
            SqlDbType.Structured,
            DbType.Object,
            0);

        /// <summary>
        ///   TODO small udt?
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly SqlMetaType MetaSUDT = new SqlMetaType(
            byte.MaxValue,
            byte.MaxValue,
            -1,
            false,
            false,
            false,
            31,
            31,
            "",
            typeof (SqlDataRecord),
            typeof (SqlDataRecord),
            SqlDbType.Structured,
            DbType.Object,
            0);

        /// <summary>
        ///   The meta data for the <c>date</c> data type.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly SqlMetaType MetaDate = new SqlMetaType(
            byte.MaxValue,
            byte.MaxValue,
            3,
            true,
            false,
            false,
            40,
            40,
            "date",
            typeof (DateTime),
            typeof (DateTime),
            SqlDbType.Date,
            DbType.Date,
            0);

        /// <summary>
        ///   The meta data for the <c>time</c> data type.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly SqlMetaType MetaTime = new SqlMetaType(
            byte.MaxValue,
            7,
            -1,
            false,
            false,
            false,
            41,
            41,
            "time",
            typeof (TimeSpan),
            typeof (TimeSpan),
            SqlDbType.Time,
            DbType.Time,
            1);

        /// <summary>
        ///   The meta data for the <c>datetime2</c> data type.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly SqlMetaType MetaDateTime2 = new SqlMetaType(
            byte.MaxValue,
            7,
            -1,
            false,
            false,
            false,
            42,
            42,
            "datetime2",
            typeof (DateTime),
            typeof (DateTime),
            SqlDbType.
                DateTime2,
            DbType.DateTime2,
            1);

        /// <summary>
        ///   The meta data for the <c>datetimeoffset</c> data type.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly SqlMetaType MetaDateTimeOffset = new SqlMetaType(
            byte.MaxValue,
            7,
            -1,
            false,
            false,
            false,
            43,
            43,
            "datetimeoffset",
            typeof (DateTimeOffset),
            typeof (DateTimeOffset),
            SqlDbType.DateTimeOffset,
            DbType.DateTimeOffset,
            1);
        #endregion

        /// <summary>
        ///   The class type.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public readonly Type ClassType;

        /// <summary>
        ///   The SQL type.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public readonly Type SqlType;

        /// <summary>
        ///   The number of bytes the type is stored in.
        ///   If this is -1 then the data type is variable-length.
        /// </summary>
        [UsedImplicitly]
        public readonly int FixedLength;

        /// <summary>
        ///   A <see cref="bool"/> value indicating whether the data type is fixed-length.
        /// </summary>
        [UsedImplicitly]
        public readonly bool IsFixed;

        /// <summary>
        ///   A <see cref="bool"/> value indicating whether the data type is a large object/value.
        /// </summary>
        [UsedImplicitly]
        public readonly bool IsLong;

        /// <summary>
        ///   PLP.
        /// </summary>
        [UsedImplicitly]
        public readonly bool IsPlp;

        /// <summary>
        ///   The precision, which is the number of digits in a numerical data type.
        /// </summary>
        [UsedImplicitly]
        public readonly byte Precision;

        /// <summary>
        ///   The scale, which is the number of digits to the right of the decimal point.
        /// </summary>
        [UsedImplicitly]
        public readonly byte Scale;

        /// <summary>
        ///   TDS value.
        /// </summary>
        [UsedImplicitly]
        public readonly byte TDSType;

        /// <summary>
        ///   Nullable TDS value.
        /// </summary>
        [UsedImplicitly]
        public readonly byte NullableType;

        /// <summary>
        ///   The name of the type.
        /// </summary>
        [UsedImplicitly]
        [NotNull]
        public readonly string TypeName;

        /// <summary>
        ///   The equivalent <see cref="SqlDbType"/>
        /// </summary>
        [UsedImplicitly]
        public readonly SqlDbType SqlDbType;

        /// <summary>
        ///   The equivalent <see cref="DbType"/>
        /// </summary>
        [UsedImplicitly]
        public readonly DbType DbType;

        /// <summary>
        ///   Propbytes.
        /// </summary>
        [UsedImplicitly]
        public readonly byte PropBytes;

        /// <summary>
        ///   Type id.
        /// </summary>
        [UsedImplicitly]
        public const int TypeId = 0;

        /// <summary>
        ///   Initializes a new instance of the <see cref="SqlMetaType"/> class.
        /// </summary>
        /// <param name="precision">
        ///   The number of digits in a numerical data type.
        /// </param>
        /// <param name="scale">
        ///   The number of digits to the right of the decimal point.
        /// </param>
        /// <param name="fixedLength">
        ///   <para>The number of bytes that the type is stored in.</para>
        ///   <para>Use -1 to specify variable-length.</para>
        /// </param>
        /// <param name="isFixed">
        ///   If set to <see langword="true"/> then the type is fixed-length.
        /// </param>
        /// <param name="isLong">
        ///   If set to <see langword="true"/> then the type is a large object/value.
        /// </param>
        /// <param name="isPlp">If set to <see langword="true"/> [is PLP].</param>
        /// <param name="tdsType">TDS type.</param>
        /// <param name="nullableTdsType">Nullable TDS type.</param>
        /// <param name="typeName">The name of the type.</param>
        /// <param name="classType">The class type.</param>
        /// <param name="sqlType">The SQL type</param>
        /// <param name="sqldbType">The SQL server specific data type.</param>
        /// <param name="dbType">The data type of a .net data provider.</param>
        /// <param name="propBytes">The prop bytes.</param>
        private SqlMetaType(
            byte precision,
            byte scale,
            int fixedLength,
            bool isFixed,
            bool isLong,
            bool isPlp,
            byte tdsType,
            byte nullableTdsType,
            [NotNull] string typeName,
            [NotNull] Type classType,
            [NotNull] Type sqlType,
            SqlDbType sqldbType,
            DbType dbType,
            byte propBytes)
        {
            Precision = precision;
            Scale = scale;
            FixedLength = fixedLength;
            IsFixed = isFixed;
            IsLong = isLong;
            IsPlp = isPlp;
            TDSType = tdsType;
            NullableType = nullableTdsType;
            TypeName = typeName;
            SqlDbType = sqldbType;
            DbType = dbType;
            ClassType = classType;
            SqlType = sqlType;
            PropBytes = propBytes;
        }

        /// <summary>
        ///   TODO
        /// </summary>
        /// <param name="type">The SQL type to check.</param>
        /// <returns>
        ///   TODO
        /// </returns>
        [UsedImplicitly]
        public static bool IsAnsiType(SqlDbType type)
        {
            if (type != SqlDbType.Char &&
                type != SqlDbType.VarChar)
                return type == SqlDbType.Text;
            return true;
        }

        /// <summary>
        ///   Determines whether the data size can be determined by the number of characters in the type.
        /// </summary>
        /// <param name="type">The SQL type to check.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if size can be determined by character length; otherwise returns <see langword="false"/>.
        /// </returns>
        [UsedImplicitly]
        public static bool IsSizeInCharacters(SqlDbType type)
        {
            if (type != SqlDbType.NChar &&
                type != SqlDbType.NVarChar &&
                type != SqlDbType.Xml)
                return type == SqlDbType.NText;
            return true;
        }

        /// <summary>
        ///   Determines whether the data type is a character stream.
        /// </summary>
        /// <param name="type">The SQL type to check.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the type is a character stream; otherwise returns <see langword="false"/>.
        /// </returns>
        [UsedImplicitly]
        public static bool IsCharType(SqlDbType type)
        {
            if (type != SqlDbType.NChar &&
                type != SqlDbType.NVarChar &&
                (type != SqlDbType.NText && type != SqlDbType.Char) &&
                (type != SqlDbType.VarChar && type != SqlDbType.Text))
                return type == SqlDbType.Xml;
            return true;
        }

        /// <summary>
        ///   Determines whether the type can contain Unicode characters.
        /// </summary>
        /// <param name="type">The SQL type to check.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the type accepts Unicode characters; otherwise returns <see langword="false"/>.
        /// </returns>
        [UsedImplicitly]
        public static bool IsNCharType(SqlDbType type)
        {
            if (type != SqlDbType.NChar &&
                type != SqlDbType.NVarChar &&
                type != SqlDbType.NText)
                return type == SqlDbType.Xml;
            return true;
        }

        /// <summary>
        ///   Determines whether the type is a binary data type.
        /// </summary>
        /// <param name="type">The SQL type to check.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the type is a binary type; otherwise returns <see langword="false"/>.
        /// </returns>
        [UsedImplicitly]
        public static bool IsBinType(SqlDbType type)
        {
            if (type != SqlDbType.Image &&
                type != SqlDbType.Binary &&
                (type != SqlDbType.VarBinary && type != SqlDbType.Timestamp) &&
                type != SqlDbType.Udt)
                return type == (SqlDbType) 24;
            return true;
        }

        /// <summary>
        ///   Determines whether the type is compatible in SQL Server 7.0.
        /// </summary>
        /// <param name="type">The SQL type to check.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the type is supported; otherwise returns <see langword="false"/>.
        /// </returns>
        [UsedImplicitly]
        public static bool Is70Supported(SqlDbType type)
        {
            if (type != SqlDbType.BigInt &&
                type > SqlDbType.BigInt)
                return type <= SqlDbType.VarChar;
            return false;
        }

        /// <summary>
        ///   Determines whether the type is compatible in SQL Server 2000.
        /// </summary>
        /// <param name="type">The SQL type to check.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the type is supported; otherwise returns <see langword="false"/>.
        /// </returns>
        [UsedImplicitly]
        public static bool Is80Supported(SqlDbType type)
        {
            if (type >= SqlDbType.BigInt)
                return type <= SqlDbType.Variant;
            return false;
        }

        /// <summary>
        ///   Determines whether the type is compatible in SQL Server 2005.
        /// </summary>
        /// <param name="type">The SQL type to check.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the type is supported; otherwise returns <see langword="false"/>.
        /// </returns>
        [UsedImplicitly]
        public static bool Is90Supported(SqlDbType type)
        {
            if (!Is80Supported(type) &&
                SqlDbType.Xml != type)
                return SqlDbType.Udt == type;
            return true;
        }

        /// <summary>
        ///   Determines whether the type is compatible in SQL Server 2008.
        /// </summary>
        /// <param name="type">The SQL type to check.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the type is supported; otherwise returns <see langword="false"/>.
        /// </returns>
        [UsedImplicitly]
        public static bool Is100Supported(SqlDbType type)
        {
            if (!Is90Supported(type) &&
                SqlDbType.Date != type &&
                (SqlDbType.Time != type && SqlDbType.DateTime2 != type))
                return SqlDbType.DateTimeOffset == type;
            return true;
        }

        /// <summary>
        ///   Determines whether the type is new from SQL Server 2008 onwards.
        /// </summary>
        /// <param name="type">The SQL type to check.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the type is from SQL Server 2008 onwards; otherwise returns <see langword="false"/>.
        /// </returns>
        [UsedImplicitly]
        public static bool IsNewKatmaiType(SqlDbType type)
        {
            return SqlDbType.Structured == type;
        }

        /// <summary>
        ///   Determines whether the specified type is a time type.
        /// </summary>
        /// <param name="type">The SQL type to check.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the <paramref name="type"/> is a time type; otherwise returns <see langword="false"/>.
        /// </returns>
        [UsedImplicitly]
        public static bool IsVarTime(SqlDbType type)
        {
            if (type != SqlDbType.Time &&
                type != SqlDbType.DateTime2)
                return type == SqlDbType.DateTimeOffset;
            return true;
        }

        /// <summary>
        ///   Gets the <see cref="SqlMetaType"/> equivalent of the <see cref="SqlDbType"/>.
        /// </summary>
        /// <param name="target">The type to get the equivalent <see cref="SqlMetaType"/> for.</param>
        /// <param name="isMultiValued">
        ///   <para>If set to <see langword="true"/> then the type is multi valued (only used for structured types).</para>
        ///   <para>By default this is set to <see langword="false"/>.</para>
        /// </param>
        /// <returns>
        ///   The <see cref="SqlMetaType"/> equivalent of the specified <see cref="SqlDbType"/>.
        /// </returns>
        /// <exception cref="DatabaseSchemaException">
        ///   <paramref name="target"/> was an unsupported <see cref="SqlDbType"/>.
        /// </exception>
        [NotNull]
        [UsedImplicitly]
        public static SqlMetaType GetMetaTypeFromSqlDbType(SqlDbType target, bool isMultiValued = false)
        {
            switch (target)
            {
                case SqlDbType.BigInt:
                    return MetaBigInt;
                case SqlDbType.Binary:
                    return MetaBinary;
                case SqlDbType.Bit:
                    return MetaBit;
                case SqlDbType.Char:
                    return MetaChar;
                case SqlDbType.DateTime:
                    return MetaDateTime;
                case SqlDbType.Decimal:
                    return MetaDecimal;
                case SqlDbType.Float:
                    return MetaFloat;
                case SqlDbType.Image:
                    return MetaImage;
                case SqlDbType.Int:
                    return MetaInt;
                case SqlDbType.Money:
                    return MetaMoney;
                case SqlDbType.NChar:
                    return MetaNChar;
                case SqlDbType.NText:
                    return MetaNText;
                case SqlDbType.NVarChar:
                    return MetaNVarChar;
                case SqlDbType.Real:
                    return MetaReal;
                case SqlDbType.UniqueIdentifier:
                    return MetaUniqueId;
                case SqlDbType.SmallDateTime:
                    return MetaSmallDateTime;
                case SqlDbType.SmallInt:
                    return MetaSmallInt;
                case SqlDbType.SmallMoney:
                    return MetaSmallMoney;
                case SqlDbType.Text:
                    return MetaText;
                case SqlDbType.Timestamp:
                    return MetaTimestamp;
                case SqlDbType.TinyInt:
                    return MetaTinyInt;
                case SqlDbType.VarBinary:
                    return MetaVarBinary;
                case SqlDbType.VarChar:
                    return MetaVarChar;
                case SqlDbType.Variant:
                    return MetaVariant;
                case (SqlDbType) 24:
                    return MetaSmallVarBinary;
                case SqlDbType.Xml:
                    return MetaXml;
                case SqlDbType.Udt:
                    return MetaUdt;
                case SqlDbType.Structured:
                    return isMultiValued ? MetaTable : MetaSUDT;
                case SqlDbType.Date:
                    return MetaDate;
                case SqlDbType.Time:
                    return MetaTime;
                case SqlDbType.DateTime2:
                    return MetaDateTime2;
                case SqlDbType.DateTimeOffset:
                    return MetaDateTimeOffset;
                default:
                    throw new DatabaseSchemaException(
                        () => Resources.SqlMetaType_GetMetaTypeFromSqlDbType_UnsupportedType,
                        target);
            }
        }

#if false

    /// <summary>
    /// Gets the <see cref="SqlMetaType"/> equivalent of the <see cref="DbType"/>.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns></returns>
        [NotNull]
        [UsedImplicitly]
        public static SqlMetaType GetMetaTypeFromDbType(DbType target)
        {
            switch (target)
            {
                case DbType.AnsiString:
                    return MetaVarChar;
                case DbType.Binary:
                    return MetaVarBinary;
                case DbType.Byte:
                    return MetaTinyInt;
                case DbType.Boolean:
                    return MetaBit;
                case DbType.Currency:
                    return MetaMoney;
                case DbType.Date:
                case DbType.DateTime:
                    return MetaDateTime;
                case DbType.Decimal:
                    return MetaDecimal;
                case DbType.Double:
                    return MetaFloat;
                case DbType.Guid:
                    return MetaUniqueId;
                case DbType.Int16:
                    return MetaSmallInt;
                case DbType.Int32:
                    return MetaInt;
                case DbType.Int64:
                    return MetaBigInt;
                case DbType.Object:
                    return MetaVariant;
                case DbType.Single:
                    return MetaReal;
                case DbType.String:
                    return MetaNVarChar;
                case DbType.Time:
                    return MetaDateTime;
                case DbType.AnsiStringFixedLength:
                    return MetaChar;
                case DbType.StringFixedLength:
                    return MetaNChar;
                case DbType.Xml:
                    return MetaXml;
                case DbType.DateTime2:
                    return MetaDateTime2;
                case DbType.DateTimeOffset:
                    return MetaDateTimeOffset;
                default:
                    throw new DatabaseSchemaException("Unsupported DbType '{0}'.", LogLevel.Error, target);
            }
        }

        /// <summary>
        /// Gets the largest <see cref="SqlMetaType"/> for a given <see cref="SqlMetaType"/>.
        /// </summary>
        /// <param name="mt">The mt.</param>
        /// <returns></returns>
        [NotNull]
        [UsedImplicitly]
        public static SqlMetaType GetMaxMetaTypeFromMetaType([NotNull]SqlMetaType mt)
        {
            switch (mt.SqlDbType)
            {
                case SqlDbType.VarBinary:
                case SqlDbType.Binary:
                    return MetaMaxVarBinary;
                case SqlDbType.VarChar:
                case SqlDbType.Char:
                    return MetaMaxVarChar;
                case SqlDbType.Udt:
                    return MetaMaxUdt;
                case SqlDbType.NChar:
                case SqlDbType.NVarChar:
                    return MetaMaxNVarChar;
                default:
                    return mt;
            }
        }

        internal static SqlMetaType GetMetaTypeFromType(Type dataType)
        {
            return GetMetaTypeFromValue(dataType, (object)null, false);
        }

        internal static SqlMetaType GetMetaTypeFromValue(object value)
        {
            return GetMetaTypeFromValue(value.GetType(), value, true);
        }

        private static SqlMetaType GetMetaTypeFromValue(Type dataType, object value, bool inferLen)
        {
            switch (Type.GetTypeCode(dataType))
            {
                case TypeCode.Empty:
                    throw ADP.InvalidDataType(TypeCode.Empty);
                case TypeCode.Object:
                    if (dataType == typeof(byte[]))
                    {
                        if (!inferLen || ((byte[])value).Length <= 8000)
                            return SqlMetaType.MetaVarBinary;
                        else
                            return SqlMetaType.MetaImage;
                    }
                    else
                    {
                        if (dataType == typeof(Guid))
                            return SqlMetaType.MetaUniqueId;
                        if (dataType == typeof(object))
                            return SqlMetaType.MetaVariant;
                        if (dataType == typeof(SqlBinary))
                            return SqlMetaType.MetaVarBinary;
                        if (dataType == typeof(SqlBoolean))
                            return SqlMetaType.MetaBit;
                        if (dataType == typeof(SqlByte))
                            return SqlMetaType.MetaTinyInt;
                        if (dataType == typeof(SqlBytes))
                            return SqlMetaType.MetaVarBinary;
                        if (dataType == typeof(SqlChars))
                            return SqlMetaType.MetaNVarChar;
                        if (dataType == typeof(SqlDateTime))
                            return SqlMetaType.MetaDateTime;
                        if (dataType == typeof(SqlDouble))
                            return SqlMetaType.MetaFloat;
                        if (dataType == typeof(SqlGuid))
                            return SqlMetaType.MetaUniqueId;
                        if (dataType == typeof(SqlInt16))
                            return SqlMetaType.MetaSmallInt;
                        if (dataType == typeof(SqlInt32))
                            return SqlMetaType.MetaInt;
                        if (dataType == typeof(SqlInt64))
                            return SqlMetaType.MetaBigInt;
                        if (dataType == typeof(SqlMoney))
                            return SqlMetaType.MetaMoney;
                        if (dataType == typeof(SqlDecimal))
                            return SqlMetaType.MetaDecimal;
                        if (dataType == typeof(SqlSingle))
                            return SqlMetaType.MetaReal;
                        if (dataType == typeof(SqlXml) || dataType == typeof(XmlReader))
                            return SqlMetaType.MetaXml;
                        if (dataType == typeof(SqlString))
                        {
                            if (!inferLen || ((SqlString)value).IsNull)
                                return SqlMetaType.MetaNVarChar;
                            else
                                return SqlMetaType.PromoteStringType(((SqlString)value).Value);
                        }
                        else
                        {
                            if (dataType == typeof(IEnumerable<DbDataRecord>) || dataType == typeof(DataTable))
                                return SqlMetaType.MetaTable;
                            if (dataType == typeof(TimeSpan))
                                return SqlMetaType.MetaTime;
                            if (dataType == typeof(DateTimeOffset))
                                return SqlMetaType.MetaDateTimeOffset;
                            if (SqlUdtInfo.TryGetFromType(dataType) != null)
                                return SqlMetaType.MetaUdt;
                            else
                                throw ADP.UnknownDataType(dataType);
                        }
                    }
                case TypeCode.DBNull:
                    throw ADP.InvalidDataType(TypeCode.DBNull);
                case TypeCode.Boolean:
                    return SqlMetaType.MetaBit;
                case TypeCode.Char:
                    throw ADP.InvalidDataType(TypeCode.Char);
                case TypeCode.SByte:
                    throw ADP.InvalidDataType(TypeCode.SByte);
                case TypeCode.Byte:
                    return SqlMetaType.MetaTinyInt;
                case TypeCode.Int16:
                    return SqlMetaType.MetaSmallInt;
                case TypeCode.UInt16:
                    throw ADP.InvalidDataType(TypeCode.UInt16);
                case TypeCode.Int32:
                    return SqlMetaType.MetaInt;
                case TypeCode.UInt32:
                    throw ADP.InvalidDataType(TypeCode.UInt32);
                case TypeCode.Int64:
                    return SqlMetaType.MetaBigInt;
                case TypeCode.UInt64:
                    throw ADP.InvalidDataType(TypeCode.UInt64);
                case TypeCode.Single:
                    return SqlMetaType.MetaReal;
                case TypeCode.Double:
                    return SqlMetaType.MetaFloat;
                case TypeCode.Decimal:
                    return SqlMetaType.MetaDecimal;
                case TypeCode.DateTime:
                    return SqlMetaType.MetaDateTime;
                case TypeCode.String:
                    if (!inferLen)
                        return SqlMetaType.MetaNVarChar;
                    else
                        return SqlMetaType.PromoteStringType((string)value);
                default:
                    throw ADP.UnknownDataTypeCode(dataType, Type.GetTypeCode(dataType));
            }
        }

        /// <summary>
        /// Gets the null SQL value for a given type.
        /// </summary>
        /// <param name="sqlType">Type of the SQL.</param>
        /// <returns></returns>
        [NotNull]
        public static object GetNullSqlValue(Type sqlType)
        {
            if (sqlType == typeof(SqlSingle))
                return SqlSingle.Null;
            if (sqlType == typeof(SqlString))
                return SqlString.Null;
            if (sqlType == typeof(SqlDouble))
                return SqlDouble.Null;
            if (sqlType == typeof(SqlBinary))
                return SqlBinary.Null;
            if (sqlType == typeof(SqlGuid))
                return SqlGuid.Null;
            if (sqlType == typeof(SqlBoolean))
                return SqlBoolean.Null;
            if (sqlType == typeof(SqlByte))
                return SqlByte.Null;
            if (sqlType == typeof(SqlInt16))
                return SqlInt16.Null;
            if (sqlType == typeof(SqlInt32))
                return SqlInt32.Null;
            if (sqlType == typeof(SqlInt64))
                return SqlInt64.Null;
            if (sqlType == typeof(SqlDecimal))
                return SqlDecimal.Null;
            if (sqlType == typeof(SqlDateTime))
                return SqlDateTime.Null;
            if (sqlType == typeof(SqlMoney))
                return SqlMoney.Null;
            if (sqlType == typeof(SqlXml))
                return SqlXml.Null;
            /*
            if (sqlType == typeof (object))
                return DBNull.Value;
            if (sqlType == typeof (IEnumerable<DbDataRecord>))
                return DBNull.Value;
            if (sqlType == typeof (DataTable))
                return DBNull.Value;
            if (sqlType == typeof (DateTime))
                return DBNull.Value;
            if (sqlType == typeof (TimeSpan))
                return DBNull.Value;
            if (sqlType == typeof (DateTimeOffset))
                return DBNull.Value;
             */
            // ReSharper disable AssignNullToNotNullAttribute
            return DBNull.Value;
            // ReSharper restore AssignNullToNotNullAttribute
        }

        internal static SqlMetaType PromoteStringType(string s)
        {
            if (s.Length << 1 > 8000)
                return SqlMetaType.MetaVarChar;
            else
                return SqlMetaType.MetaNVarChar;
        }


        /// <summary>
        /// Gets the CLR value from SQL variant.
        /// </summary>
        /// <param name="sqlVal">The SQL val.</param>
        /// <returns></returns>
        [CanBeNull]
        public static object GetCLRValueFromSqlVariant(object sqlVal)
        {
            if (sqlVal.IsNull())
                return null;
            if (sqlVal is SqlSingle)
                return ((SqlSingle) sqlVal).Value;
            if (sqlVal is SqlString)
                return ((SqlString) sqlVal).Value;
            if (sqlVal is SqlDouble)
                return ((SqlDouble) sqlVal).Value;
            if (sqlVal is SqlBinary)
                return ((SqlBinary) sqlVal).Value;
            if (sqlVal is SqlGuid)
                return ((SqlGuid) sqlVal).Value;
            if (sqlVal is SqlBoolean)
                return ((SqlBoolean) sqlVal).Value;
            if (sqlVal is SqlByte)
                return ((SqlByte) sqlVal).Value;
            if (sqlVal is SqlInt16)
                return ((SqlInt16) sqlVal).Value;
            if (sqlVal is SqlInt32)
                return ((SqlInt32) sqlVal).Value;
            if (sqlVal is SqlInt64)
                return ((SqlInt64) sqlVal).Value;
            if (sqlVal is SqlDecimal)
                return ((SqlDecimal) sqlVal).Value;
            if (sqlVal is SqlDateTime)
                return ((SqlDateTime) sqlVal).Value;
            if (sqlVal is SqlMoney)
                return ((SqlMoney) sqlVal).Value;
            if (sqlVal is SqlXml)
                return ((SqlXml) sqlVal).Value;

            return null;
        }

        /// <summary>
        /// Gets the SQL value from CLR variant.
        /// </summary>
        /// <param name="clrVal">The COM val.</param>
        /// <returns></returns>
        [CanBeNull]
        public static object GetSqlValueFromCLRVariant(object clrVal)
        {
            object obj = (object)null;
            if (clrVal != null && DBNull.Value != clrVal)
            {
                if (clrVal is float)
                    obj = (object)new SqlSingle((float)clrVal);
                else if (clrVal is string)
                    obj = (object)new SqlString((string)clrVal);
                else if (clrVal is double)
                    obj = (object)new SqlDouble((double)clrVal);
                else if (clrVal is byte[])
                    obj = (object)new SqlBinary((byte[])clrVal);
                else if (clrVal is char)
                    obj = (object)new SqlString(((char)clrVal).ToString());
                else if (clrVal is char[])
                    obj = (object)new SqlChars((char[])clrVal);
                else if (clrVal is Guid)
                    obj = (object)new SqlGuid((Guid)clrVal);
                else if (clrVal is bool)
                    obj = (object)new SqlBoolean((bool)clrVal);
                else if (clrVal is byte)
                    obj = (object)new SqlByte((byte)clrVal);
                else if (clrVal is short)
                    obj = (object)new SqlInt16((short)clrVal);
                else if (clrVal is int)
                    obj = (object)new SqlInt32((int)clrVal);
                else if (clrVal is long)
                    obj = (object)new SqlInt64((long)clrVal);
                else if (clrVal is Decimal)
                    obj = (object)new SqlDecimal((Decimal)clrVal);
                else if (clrVal is DateTime)
                    obj = (object)new SqlDateTime((DateTime)clrVal);
                else if (clrVal is XmlReader)
                    obj = (object)new SqlXml((XmlReader)clrVal);
                else if (clrVal is TimeSpan || clrVal is DateTimeOffset)
                    obj = clrVal;
            }
            return obj;
        }

        /// <summary>
        /// Gets the type of the SQL db type from OLE db.
        /// </summary>
        /// <param name="dbType">Type of the db.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <returns></returns>
        public static SqlDbType GetSqlDbTypeFromOleDbType(OleDbType dbType, string typeName)
        {
            SqlDbType sqlDbType = SqlDbType.Variant;
            switch (dbType)
            {
                case OleDbType.Guid:
                    sqlDbType = SqlDbType.UniqueIdentifier;
                    break;
                case OleDbType.Binary:
                case OleDbType.VarBinary:
                    sqlDbType = typeName == "binary" ? SqlDbType.Binary : SqlDbType.VarBinary;
                    break;
                case OleDbType.Char:
                case OleDbType.VarChar:
                    sqlDbType = typeName == "char" ? SqlDbType.Char : SqlDbType.VarChar;
                    break;
                case OleDbType.WChar:
                case OleDbType.VarWChar:
                case OleDbType.BSTR:
                    sqlDbType = typeName == "nchar" ? SqlDbType.NChar : SqlDbType.NVarChar;
                    break;
                case OleDbType.Numeric:
                case OleDbType.Decimal:
                    sqlDbType = SqlDbType.Decimal;
                    break;
                case (OleDbType)132:
                    sqlDbType = SqlDbType.Udt;
                    break;
                case OleDbType.DBDate:
                    sqlDbType = SqlDbType.Date;
                    break;
                case OleDbType.DBTimeStamp:
                case OleDbType.Date:
                case OleDbType.Filetime:
                    switch (typeName)
                    {
                        case "smalldatetime":
                            sqlDbType = SqlDbType.SmallDateTime;
                            break;
                        case "datetime2":
                            sqlDbType = SqlDbType.DateTime2;
                            break;
                        default:
                            sqlDbType = SqlDbType.DateTime;
                            break;
                    }
                case (OleDbType)141:
                    sqlDbType = SqlDbType.Xml;
                    break;
                case (OleDbType)145:
                    sqlDbType = SqlDbType.Time;
                    break;
                case (OleDbType)146:
                    sqlDbType = SqlDbType.DateTimeOffset;
                    break;
                case OleDbType.LongVarChar:
                    sqlDbType = SqlDbType.Text;
                    break;
                case OleDbType.LongVarWChar:
                    sqlDbType = SqlDbType.NText;
                    break;
                case OleDbType.LongVarBinary:
                    sqlDbType = SqlDbType.Image;
                    break;
                case OleDbType.SmallInt:
                case OleDbType.UnsignedSmallInt:
                    sqlDbType = SqlDbType.SmallInt;
                    break;
                case OleDbType.Integer:
                    sqlDbType = SqlDbType.Int;
                    break;
                case OleDbType.Single:
                    sqlDbType = SqlDbType.Real;
                    break;
                case OleDbType.Double:
                    sqlDbType = SqlDbType.Float;
                    break;
                case OleDbType.Currency:
                    sqlDbType = typeName == "smallmoney" ? SqlDbType.SmallMoney : SqlDbType.Money;
                    break;
                case OleDbType.Boolean:
                    sqlDbType = SqlDbType.Bit;
                    break;
                case OleDbType.Variant:
                    sqlDbType = SqlDbType.Variant;
                    break;
                case OleDbType.TinyInt:
                case OleDbType.UnsignedTinyInt:
                    sqlDbType = SqlDbType.TinyInt;
                    break;
                case OleDbType.BigInt:
                    sqlDbType = SqlDbType.BigInt;
                    break;
            }
            return sqlDbType;
        }

        internal static SqlMetaType GetSqlDataType(int tdsType, uint userType, int length)
        {
            switch (tdsType)
            {
                case 231:
                    return SqlMetaType.MetaNVarChar;
                case 239:
                    return SqlMetaType.MetaNChar;
                case 240:
                    return SqlMetaType.MetaUdt;
                case 241:
                    return SqlMetaType.MetaXml;
                case 243:
                    return SqlMetaType.MetaTable;
                case 165:
                    return SqlMetaType.MetaVarBinary;
                case 167:
                case 39:
                    return SqlMetaType.MetaVarChar;
                case 173:
                case 45:
                    if (80 != (int)userType)
                        return SqlMetaType.MetaBinary;
                    else
                        return SqlMetaType.MetaTimestamp;
                case 175:
                case 47:
                    return SqlMetaType.MetaChar;
                case 122:
                    return SqlMetaType.MetaSmallMoney;
                case (int)sbyte.MaxValue:
                    return SqlMetaType.MetaBigInt;
                case 34:
                    return SqlMetaType.MetaImage;
                case 35:
                    return SqlMetaType.MetaText;
                case 36:
                    return SqlMetaType.MetaUniqueId;
                case 37:
                    return SqlMetaType.MetaSmallVarBinary;
                case 38:
                    if (4 > length)
                    {
                        if (2 != length)
                            return SqlMetaType.MetaTinyInt;
                        else
                            return SqlMetaType.MetaSmallInt;
                    }
                    else if (4 != length)
                        return SqlMetaType.MetaBigInt;
                    else
                        return SqlMetaType.MetaInt;
                case 40:
                    return SqlMetaType.MetaDate;
                case 41:
                    return SqlMetaType.MetaTime;
                case 42:
                    return SqlMetaType.MetaDateTime2;
                case 43:
                    return SqlMetaType.MetaDateTimeOffset;
                case 48:
                    return SqlMetaType.MetaTinyInt;
                case 50:
                case 104:
                    return SqlMetaType.MetaBit;
                case 52:
                    return SqlMetaType.MetaSmallInt;
                case 56:
                    return SqlMetaType.MetaInt;
                case 58:
                    return SqlMetaType.MetaSmallDateTime;
                case 59:
                    return SqlMetaType.MetaReal;
                case 60:
                    return SqlMetaType.MetaMoney;
                case 61:
                    return SqlMetaType.MetaDateTime;
                case 62:
                    return SqlMetaType.MetaFloat;
                case 98:
                    return SqlMetaType.MetaVariant;
                case 99:
                    return SqlMetaType.MetaNText;
                case 106:
                case 108:
                    return SqlMetaType.MetaDecimal;
                case 109:
                    if (4 != length)
                        return SqlMetaType.MetaFloat;
                    else
                        return SqlMetaType.MetaReal;
                case 110:
                    if (4 != length)
                        return SqlMetaType.MetaMoney;
                    else
                        return SqlMetaType.MetaSmallMoney;
                case 111:
                    if (4 != length)
                        return SqlMetaType.MetaDateTime;
                    else
                        return SqlMetaType.MetaSmallDateTime;
                default:
                    throw SQL.InvalidSqlDbType((SqlDbType)tdsType);
            }
        }
#endif
    }
}