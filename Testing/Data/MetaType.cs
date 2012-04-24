// Type: System.Data.SqlClient.MetaType
// Assembly: System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.Data.dll

using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Xml;

namespace WebApplications.Testing.Data
{
    public sealed class MetaType
    {
        private static readonly MetaType MetaBigInt = new MetaType((byte)19, byte.MaxValue, 8, true, false, false, (byte)127, (byte)38, "bigint", typeof(long), typeof(SqlInt64), SqlDbType.BigInt, DbType.Int64, (byte)0);
        private static readonly MetaType MetaFloat = new MetaType((byte)15, byte.MaxValue, 8, true, false, false, (byte)62, (byte)109, "float", typeof(double), typeof(SqlDouble), SqlDbType.Float, DbType.Double, (byte)0);
        private static readonly MetaType MetaReal = new MetaType((byte)7, byte.MaxValue, 4, true, false, false, (byte)59, (byte)109, "real", typeof(float), typeof(SqlSingle), SqlDbType.Real, DbType.Single, (byte)0);
        private static readonly MetaType MetaBinary = new MetaType(byte.MaxValue, byte.MaxValue, -1, false, false, false, (byte)173, (byte)173, "binary", typeof(byte[]), typeof(SqlBinary), SqlDbType.Binary, DbType.Binary, (byte)2);
        private static readonly MetaType MetaTimestamp = new MetaType(byte.MaxValue, byte.MaxValue, -1, false, false, false, (byte)173, (byte)173, "timestamp", typeof(byte[]), typeof(SqlBinary), SqlDbType.Timestamp, DbType.Binary, (byte)2);
        internal static readonly MetaType MetaVarBinary = new MetaType(byte.MaxValue, byte.MaxValue, -1, false, false, false, (byte)165, (byte)165, "varbinary", typeof(byte[]), typeof(SqlBinary), SqlDbType.VarBinary, DbType.Binary, (byte)2);
        internal static readonly MetaType MetaMaxVarBinary = new MetaType(byte.MaxValue, byte.MaxValue, -1, false, true, true, (byte)165, (byte)165, "varbinary", typeof(byte[]), typeof(SqlBinary), SqlDbType.VarBinary, DbType.Binary, (byte)2);
        private static readonly MetaType MetaSmallVarBinary = new MetaType(byte.MaxValue, byte.MaxValue, -1, false, false, false, (byte)37, (byte)173, string.Empty, typeof(byte[]), typeof(SqlBinary), (SqlDbType)24, DbType.Binary, (byte)2);
        internal static readonly MetaType MetaImage = new MetaType(byte.MaxValue, byte.MaxValue, -1, false, true, false, (byte)34, (byte)34, "image", typeof(byte[]), typeof(SqlBinary), SqlDbType.Image, DbType.Binary, (byte)0);
        private static readonly MetaType MetaBit = new MetaType(byte.MaxValue, byte.MaxValue, 1, true, false, false, (byte)50, (byte)104, "bit", typeof(bool), typeof(SqlBoolean), SqlDbType.Bit, DbType.Boolean, (byte)0);
        private static readonly MetaType MetaTinyInt = new MetaType((byte)3, byte.MaxValue, 1, true, false, false, (byte)48, (byte)38, "tinyint", typeof(byte), typeof(SqlByte), SqlDbType.TinyInt, DbType.Byte, (byte)0);
        private static readonly MetaType MetaSmallInt = new MetaType((byte)5, byte.MaxValue, 2, true, false, false, (byte)52, (byte)38, "smallint", typeof(short), typeof(SqlInt16), SqlDbType.SmallInt, DbType.Int16, (byte)0);
        private static readonly MetaType MetaInt = new MetaType((byte)10, byte.MaxValue, 4, true, false, false, (byte)56, (byte)38, "int", typeof(int), typeof(SqlInt32), SqlDbType.Int, DbType.Int32, (byte)0);
        private static readonly MetaType MetaChar = new MetaType(byte.MaxValue, byte.MaxValue, -1, false, false, false, (byte)175, (byte)175, "char", typeof(string), typeof(SqlString), SqlDbType.Char, DbType.AnsiStringFixedLength, (byte)7);
        private static readonly MetaType MetaVarChar = new MetaType(byte.MaxValue, byte.MaxValue, -1, false, false, false, (byte)167, (byte)167, "varchar", typeof(string), typeof(SqlString), SqlDbType.VarChar, DbType.AnsiString, (byte)7);
        internal static readonly MetaType MetaMaxVarChar = new MetaType(byte.MaxValue, byte.MaxValue, -1, false, true, true, (byte)167, (byte)167, "varchar", typeof(string), typeof(SqlString), SqlDbType.VarChar, DbType.AnsiString, (byte)7);
        internal static readonly MetaType MetaText = new MetaType(byte.MaxValue, byte.MaxValue, -1, false, true, false, (byte)35, (byte)35, "text", typeof(string), typeof(SqlString), SqlDbType.Text, DbType.AnsiString, (byte)0);
        private static readonly MetaType MetaNChar = new MetaType(byte.MaxValue, byte.MaxValue, -1, false, false, false, (byte)239, (byte)239, "nchar", typeof(string), typeof(SqlString), SqlDbType.NChar, DbType.StringFixedLength, (byte)7);
        internal static readonly MetaType MetaNVarChar = new MetaType(byte.MaxValue, byte.MaxValue, -1, false, false, false, (byte)231, (byte)231, "nvarchar", typeof(string), typeof(SqlString), SqlDbType.NVarChar, DbType.String, (byte)7);
        internal static readonly MetaType MetaMaxNVarChar = new MetaType(byte.MaxValue, byte.MaxValue, -1, false, true, true, (byte)231, (byte)231, "nvarchar", typeof(string), typeof(SqlString), SqlDbType.NVarChar, DbType.String, (byte)7);
        internal static readonly MetaType MetaNText = new MetaType(byte.MaxValue, byte.MaxValue, -1, false, true, false, (byte)99, (byte)99, "ntext", typeof(string), typeof(SqlString), SqlDbType.NText, DbType.String, (byte)7);
        internal static readonly MetaType MetaDecimal = new MetaType((byte)38, (byte)4, 17, true, false, false, (byte)108, (byte)108, "decimal", typeof(Decimal), typeof(SqlDecimal), SqlDbType.Decimal, DbType.Decimal, (byte)2);
        internal static readonly MetaType MetaXml = new MetaType(byte.MaxValue, byte.MaxValue, -1, false, true, true, (byte)241, (byte)241, "xml", typeof(string), typeof(SqlXml), SqlDbType.Xml, DbType.Xml, (byte)0);
        private static readonly MetaType MetaDateTime = new MetaType((byte)23, (byte)3, 8, true, false, false, (byte)61, (byte)111, "datetime", typeof(DateTime), typeof(SqlDateTime), SqlDbType.DateTime, DbType.DateTime, (byte)0);
        private static readonly MetaType MetaSmallDateTime = new MetaType((byte)16, (byte)0, 4, true, false, false, (byte)58, (byte)111, "smalldatetime", typeof(DateTime), typeof(SqlDateTime), SqlDbType.SmallDateTime, DbType.DateTime, (byte)0);
        private static readonly MetaType MetaMoney = new MetaType((byte)19, byte.MaxValue, 8, true, false, false, (byte)60, (byte)110, "money", typeof(Decimal), typeof(SqlMoney), SqlDbType.Money, DbType.Currency, (byte)0);
        private static readonly MetaType MetaSmallMoney = new MetaType((byte)10, byte.MaxValue, 4, true, false, false, (byte)122, (byte)110, "smallmoney", typeof(Decimal), typeof(SqlMoney), SqlDbType.SmallMoney, DbType.Currency, (byte)0);
        private static readonly MetaType MetaUniqueId = new MetaType(byte.MaxValue, byte.MaxValue, 16, true, false, false, (byte)36, (byte)36, "uniqueidentifier", typeof(Guid), typeof(SqlGuid), SqlDbType.UniqueIdentifier, DbType.Guid, (byte)0);
        private static readonly MetaType MetaVariant = new MetaType(byte.MaxValue, byte.MaxValue, -1, true, false, false, (byte)98, (byte)98, "sql_variant", typeof(object), typeof(object), SqlDbType.Variant, DbType.Object, (byte)0);
        internal static readonly MetaType MetaUdt = new MetaType(byte.MaxValue, byte.MaxValue, -1, false, false, true, (byte)240, (byte)240, "udt", typeof(object), typeof(object), SqlDbType.Udt, DbType.Object, (byte)0);
        private static readonly MetaType MetaMaxUdt = new MetaType(byte.MaxValue, byte.MaxValue, -1, false, true, true, (byte)240, (byte)240, "udt", typeof(object), typeof(object), SqlDbType.Udt, DbType.Object, (byte)0);
        private static readonly MetaType MetaTable = new MetaType(byte.MaxValue, byte.MaxValue, -1, false, false, false, (byte)243, (byte)243, "table", typeof(IEnumerable<DbDataRecord>), typeof(IEnumerable<DbDataRecord>), SqlDbType.Structured, DbType.Object, (byte)0);
        private static readonly MetaType MetaSUDT = new MetaType(byte.MaxValue, byte.MaxValue, -1, false, false, false, (byte)31, (byte)31, "", typeof(SqlDataRecord), typeof(SqlDataRecord), SqlDbType.Structured, DbType.Object, (byte)0);
        private static readonly MetaType MetaDate = new MetaType(byte.MaxValue, byte.MaxValue, 3, true, false, false, (byte)40, (byte)40, "date", typeof(DateTime), typeof(DateTime), SqlDbType.Date, DbType.Date, (byte)0);
        internal static readonly MetaType MetaTime = new MetaType(byte.MaxValue, (byte)7, -1, false, false, false, (byte)41, (byte)41, "time", typeof(TimeSpan), typeof(TimeSpan), SqlDbType.Time, DbType.Time, (byte)1);
        private static readonly MetaType MetaDateTime2 = new MetaType(byte.MaxValue, (byte)7, -1, false, false, false, (byte)42, (byte)42, "datetime2", typeof(DateTime), typeof(DateTime), SqlDbType.DateTime2, DbType.DateTime2, (byte)1);
        internal static readonly MetaType MetaDateTimeOffset = new MetaType(byte.MaxValue, (byte)7, -1, false, false, false, (byte)43, (byte)43, "datetimeoffset", typeof(DateTimeOffset), typeof(DateTimeOffset), SqlDbType.DateTimeOffset, DbType.DateTimeOffset, (byte)1);
        internal readonly Type ClassType;
        internal readonly Type SqlType;
        internal readonly int FixedLength;
        internal readonly bool IsFixed;
        internal readonly bool IsLong;
        internal readonly bool IsPlp;
        internal readonly byte Precision;
        internal readonly byte Scale;
        internal readonly byte TDSType;
        internal readonly byte NullableType;
        internal readonly string TypeName;
        internal readonly SqlDbType SqlDbType;
        internal readonly DbType DbType;
        internal readonly byte PropBytes;
        internal readonly bool IsAnsiType;
        internal readonly bool IsBinType;
        internal readonly bool IsCharType;
        internal readonly bool IsNCharType;
        internal readonly bool IsSizeInCharacters;
        internal readonly bool IsNewKatmaiType;
        internal readonly bool IsVarTime;
        internal readonly bool Is70Supported;
        internal readonly bool Is80Supported;
        internal readonly bool Is90Supported;
        internal readonly bool Is100Supported;

        public int TypeId
        {
            get
            {
                return 0;
            }
        }

        static MetaType()
        {
        }

        public MetaType(byte precision, byte scale, int fixedLength, bool isFixed, bool isLong, bool isPlp, byte tdsType, byte nullableTdsType, string typeName, Type classType, Type sqlType, SqlDbType sqldbType, DbType dbType, byte propBytes)
        {
            this.Precision = precision;
            this.Scale = scale;
            this.FixedLength = fixedLength;
            this.IsFixed = isFixed;
            this.IsLong = isLong;
            this.IsPlp = isPlp;
            this.TDSType = tdsType;
            this.NullableType = nullableTdsType;
            this.TypeName = typeName;
            this.SqlDbType = sqldbType;
            this.DbType = dbType;
            this.ClassType = classType;
            this.SqlType = sqlType;
            this.PropBytes = propBytes;
            this.IsAnsiType = MetaType._IsAnsiType(sqldbType);
            this.IsBinType = MetaType._IsBinType(sqldbType);
            this.IsCharType = MetaType._IsCharType(sqldbType);
            this.IsNCharType = MetaType._IsNCharType(sqldbType);
            this.IsSizeInCharacters = MetaType._IsSizeInCharacters(sqldbType);
            this.IsNewKatmaiType = MetaType._IsNewKatmaiType(sqldbType);
            this.IsVarTime = MetaType._IsVarTime(sqldbType);
            this.Is70Supported = MetaType._Is70Supported(this.SqlDbType);
            this.Is80Supported = MetaType._Is80Supported(this.SqlDbType);
            this.Is90Supported = MetaType._Is90Supported(this.SqlDbType);
            this.Is100Supported = MetaType._Is100Supported(this.SqlDbType);
        }

        private static bool _IsAnsiType(SqlDbType type)
        {
            if (type != SqlDbType.Char && type != SqlDbType.VarChar)
                return type == SqlDbType.Text;
            else
                return true;
        }

        private static bool _IsSizeInCharacters(SqlDbType type)
        {
            if (type != SqlDbType.NChar && type != SqlDbType.NVarChar && type != SqlDbType.Xml)
                return type == SqlDbType.NText;
            else
                return true;
        }

        private static bool _IsCharType(SqlDbType type)
        {
            if (type != SqlDbType.NChar && type != SqlDbType.NVarChar && (type != SqlDbType.NText && type != SqlDbType.Char) && (type != SqlDbType.VarChar && type != SqlDbType.Text))
                return type == SqlDbType.Xml;
            else
                return true;
        }

        private static bool _IsNCharType(SqlDbType type)
        {
            if (type != SqlDbType.NChar && type != SqlDbType.NVarChar && type != SqlDbType.NText)
                return type == SqlDbType.Xml;
            else
                return true;
        }

        private static bool _IsBinType(SqlDbType type)
        {
            if (type != SqlDbType.Image && type != SqlDbType.Binary && (type != SqlDbType.VarBinary && type != SqlDbType.Timestamp) && type != SqlDbType.Udt)
                return type == (SqlDbType)24;
            else
                return true;
        }

        private static bool _Is70Supported(SqlDbType type)
        {
            if (type != SqlDbType.BigInt && type > SqlDbType.BigInt)
                return type <= SqlDbType.VarChar;
            else
                return false;
        }

        private static bool _Is80Supported(SqlDbType type)
        {
            if (type >= SqlDbType.BigInt)
                return type <= SqlDbType.Variant;
            else
                return false;
        }

        private static bool _Is90Supported(SqlDbType type)
        {
            if (!MetaType._Is80Supported(type) && SqlDbType.Xml != type)
                return SqlDbType.Udt == type;
            else
                return true;
        }

        private static bool _Is100Supported(SqlDbType type)
        {
            if (!MetaType._Is90Supported(type) && SqlDbType.Date != type && (SqlDbType.Time != type && SqlDbType.DateTime2 != type))
                return SqlDbType.DateTimeOffset == type;
            else
                return true;
        }

        private static bool _IsNewKatmaiType(SqlDbType type)
        {
            return SqlDbType.Structured == type;
        }

        internal static bool _IsVarTime(SqlDbType type)
        {
            if (type != SqlDbType.Time && type != SqlDbType.DateTime2)
                return type == SqlDbType.DateTimeOffset;
            else
                return true;
        }

        internal static MetaType GetMetaTypeFromSqlDbType(SqlDbType target, bool isMultiValued)
        {
            switch (target)
            {
                case SqlDbType.BigInt:
                    return MetaType.MetaBigInt;
                case SqlDbType.Binary:
                    return MetaType.MetaBinary;
                case SqlDbType.Bit:
                    return MetaType.MetaBit;
                case SqlDbType.Char:
                    return MetaType.MetaChar;
                case SqlDbType.DateTime:
                    return MetaType.MetaDateTime;
                case SqlDbType.Decimal:
                    return MetaType.MetaDecimal;
                case SqlDbType.Float:
                    return MetaType.MetaFloat;
                case SqlDbType.Image:
                    return MetaType.MetaImage;
                case SqlDbType.Int:
                    return MetaType.MetaInt;
                case SqlDbType.Money:
                    return MetaType.MetaMoney;
                case SqlDbType.NChar:
                    return MetaType.MetaNChar;
                case SqlDbType.NText:
                    return MetaType.MetaNText;
                case SqlDbType.NVarChar:
                    return MetaType.MetaNVarChar;
                case SqlDbType.Real:
                    return MetaType.MetaReal;
                case SqlDbType.UniqueIdentifier:
                    return MetaType.MetaUniqueId;
                case SqlDbType.SmallDateTime:
                    return MetaType.MetaSmallDateTime;
                case SqlDbType.SmallInt:
                    return MetaType.MetaSmallInt;
                case SqlDbType.SmallMoney:
                    return MetaType.MetaSmallMoney;
                case SqlDbType.Text:
                    return MetaType.MetaText;
                case SqlDbType.Timestamp:
                    return MetaType.MetaTimestamp;
                case SqlDbType.TinyInt:
                    return MetaType.MetaTinyInt;
                case SqlDbType.VarBinary:
                    return MetaType.MetaVarBinary;
                case SqlDbType.VarChar:
                    return MetaType.MetaVarChar;
                case SqlDbType.Variant:
                    return MetaType.MetaVariant;
                case (SqlDbType)24:
                    return MetaType.MetaSmallVarBinary;
                case SqlDbType.Xml:
                    return MetaType.MetaXml;
                case SqlDbType.Udt:
                    return MetaType.MetaUdt;
                case SqlDbType.Structured:
                    if (isMultiValued)
                        return MetaType.MetaTable;
                    else
                        return MetaType.MetaSUDT;
                case SqlDbType.Date:
                    return MetaType.MetaDate;
                case SqlDbType.Time:
                    return MetaType.MetaTime;
                case SqlDbType.DateTime2:
                    return MetaType.MetaDateTime2;
                case SqlDbType.DateTimeOffset:
                    return MetaType.MetaDateTimeOffset;
                default:
                    throw new ArgumentOutOfRangeException("target");
            }
        }

        internal static MetaType GetMetaTypeFromDbType(DbType target)
        {
            switch (target)
            {
                case DbType.AnsiString:
                    return MetaType.MetaVarChar;
                case DbType.Binary:
                    return MetaType.MetaVarBinary;
                case DbType.Byte:
                    return MetaType.MetaTinyInt;
                case DbType.Boolean:
                    return MetaType.MetaBit;
                case DbType.Currency:
                    return MetaType.MetaMoney;
                case DbType.Date:
                case DbType.DateTime:
                    return MetaType.MetaDateTime;
                case DbType.Decimal:
                    return MetaType.MetaDecimal;
                case DbType.Double:
                    return MetaType.MetaFloat;
                case DbType.Guid:
                    return MetaType.MetaUniqueId;
                case DbType.Int16:
                    return MetaType.MetaSmallInt;
                case DbType.Int32:
                    return MetaType.MetaInt;
                case DbType.Int64:
                    return MetaType.MetaBigInt;
                case DbType.Object:
                    return MetaType.MetaVariant;
                case DbType.Single:
                    return MetaType.MetaReal;
                case DbType.String:
                    return MetaType.MetaNVarChar;
                case DbType.Time:
                    return MetaType.MetaDateTime;
                case DbType.AnsiStringFixedLength:
                    return MetaType.MetaChar;
                case DbType.StringFixedLength:
                    return MetaType.MetaNChar;
                case DbType.Xml:
                    return MetaType.MetaXml;
                case DbType.DateTime2:
                    return MetaType.MetaDateTime2;
                case DbType.DateTimeOffset:
                    return MetaType.MetaDateTimeOffset;
                default:
                    throw new ArgumentOutOfRangeException("target");
            }
        }

        internal static MetaType GetMaxMetaTypeFromMetaType(MetaType mt)
        {
            switch (mt.SqlDbType)
            {
                case SqlDbType.VarBinary:
                case SqlDbType.Binary:
                    return MetaType.MetaMaxVarBinary;
                case SqlDbType.VarChar:
                case SqlDbType.Char:
                    return MetaType.MetaMaxVarChar;
                case SqlDbType.Udt:
                    return MetaType.MetaMaxUdt;
                case SqlDbType.NChar:
                case SqlDbType.NVarChar:
                    return MetaType.MetaMaxNVarChar;
                default:
                    return mt;
            }
        }

        internal static MetaType GetMetaTypeFromType(Type dataType)
        {
            return MetaType.GetMetaTypeFromValue(dataType, (object)null, false);
        }

        internal static MetaType GetMetaTypeFromValue(object value)
        {
            return MetaType.GetMetaTypeFromValue(value.GetType(), value, true);
        }

        private static MetaType GetMetaTypeFromValue(Type dataType, object value, bool inferLen)
        {
            switch (Type.GetTypeCode(dataType))
            {
                case TypeCode.Empty:
                    throw new ArgumentOutOfRangeException("dataType");
                case TypeCode.Object:
                    if (dataType == typeof(byte[]))
                    {
                        if (!inferLen || ((byte[])value).Length <= 8000)
                            return MetaType.MetaVarBinary;
                        else
                            return MetaType.MetaImage;
                    }
                    else
                    {
                        if (dataType == typeof(Guid))
                            return MetaType.MetaUniqueId;
                        if (dataType == typeof(object))
                            return MetaType.MetaVariant;
                        if (dataType == typeof(SqlBinary))
                            return MetaType.MetaVarBinary;
                        if (dataType == typeof(SqlBoolean))
                            return MetaType.MetaBit;
                        if (dataType == typeof(SqlByte))
                            return MetaType.MetaTinyInt;
                        if (dataType == typeof(SqlBytes))
                            return MetaType.MetaVarBinary;
                        if (dataType == typeof(SqlChars))
                            return MetaType.MetaNVarChar;
                        if (dataType == typeof(SqlDateTime))
                            return MetaType.MetaDateTime;
                        if (dataType == typeof(SqlDouble))
                            return MetaType.MetaFloat;
                        if (dataType == typeof(SqlGuid))
                            return MetaType.MetaUniqueId;
                        if (dataType == typeof(SqlInt16))
                            return MetaType.MetaSmallInt;
                        if (dataType == typeof(SqlInt32))
                            return MetaType.MetaInt;
                        if (dataType == typeof(SqlInt64))
                            return MetaType.MetaBigInt;
                        if (dataType == typeof(SqlMoney))
                            return MetaType.MetaMoney;
                        if (dataType == typeof(SqlDecimal))
                            return MetaType.MetaDecimal;
                        if (dataType == typeof(SqlSingle))
                            return MetaType.MetaReal;
                        if (dataType == typeof(SqlXml) || dataType == typeof(XmlReader))
                            return MetaType.MetaXml;
                        if (dataType == typeof(SqlString))
                        {
                            if (!inferLen || ((SqlString)value).IsNull)
                                return MetaType.MetaNVarChar;
                            else
                                return MetaType.PromoteStringType(((SqlString)value).Value);
                        }
                        else
                        {
                            if (dataType == typeof(IEnumerable<DbDataRecord>) || dataType == typeof(DataTable))
                                return MetaType.MetaTable;
                            if (dataType == typeof(TimeSpan))
                                return MetaType.MetaTime;
                            if (dataType == typeof(DateTimeOffset))
                                return MetaType.MetaDateTimeOffset;
                            if (SqlUdtInfo.TryGetFromType(dataType) != null)
                                return MetaType.MetaUdt;
                            else
                                throw new ArgumentOutOfRangeException("dataType");
                        }
                    }
                case TypeCode.DBNull:
                    throw new ArgumentOutOfRangeException("dataType");
                case TypeCode.Boolean:
                    return MetaType.MetaBit;
                case TypeCode.Char:
                    throw new ArgumentOutOfRangeException("dataType");
                case TypeCode.SByte:
                    throw new ArgumentOutOfRangeException("dataType");
                case TypeCode.Byte:
                    return MetaType.MetaTinyInt;
                case TypeCode.Int16:
                    return MetaType.MetaSmallInt;
                case TypeCode.UInt16:
                    throw new ArgumentOutOfRangeException("dataType");
                case TypeCode.Int32:
                    return MetaType.MetaInt;
                case TypeCode.UInt32:
                    throw new ArgumentOutOfRangeException("dataType");
                case TypeCode.Int64:
                    return MetaType.MetaBigInt;
                case TypeCode.UInt64:
                    throw new ArgumentOutOfRangeException("dataType");
                case TypeCode.Single:
                    return MetaType.MetaReal;
                case TypeCode.Double:
                    return MetaType.MetaFloat;
                case TypeCode.Decimal:
                    return MetaType.MetaDecimal;
                case TypeCode.DateTime:
                    return MetaType.MetaDateTime;
                case TypeCode.String:
                    return !inferLen ? MetaType.MetaNVarChar : MetaType.PromoteStringType((string)value);
                default:
                    throw new ArgumentOutOfRangeException("dataType");
            }
        }

        internal static object GetNullSqlValue(Type sqlType)
        {
            if (sqlType == typeof(SqlSingle))
                return (object)SqlSingle.Null;
            if (sqlType == typeof(SqlString))
                return (object)SqlString.Null;
            if (sqlType == typeof(SqlDouble))
                return (object)SqlDouble.Null;
            if (sqlType == typeof(SqlBinary))
                return (object)SqlBinary.Null;
            if (sqlType == typeof(SqlGuid))
                return (object)SqlGuid.Null;
            if (sqlType == typeof(SqlBoolean))
                return (object)SqlBoolean.Null;
            if (sqlType == typeof(SqlByte))
                return (object)SqlByte.Null;
            if (sqlType == typeof(SqlInt16))
                return (object)SqlInt16.Null;
            if (sqlType == typeof(SqlInt32))
                return (object)SqlInt32.Null;
            if (sqlType == typeof(SqlInt64))
                return (object)SqlInt64.Null;
            if (sqlType == typeof(SqlDecimal))
                return (object)SqlDecimal.Null;
            if (sqlType == typeof(SqlDateTime))
                return (object)SqlDateTime.Null;
            if (sqlType == typeof(SqlMoney))
                return (object)SqlMoney.Null;
            if (sqlType == typeof(SqlXml))
                return (object)SqlXml.Null;
            if (sqlType == typeof(object))
                return (object)DBNull.Value;
            if (sqlType == typeof(IEnumerable<DbDataRecord>))
                return (object)DBNull.Value;
            if (sqlType == typeof(DataTable))
                return (object)DBNull.Value;
            if (sqlType == typeof(DateTime))
                return (object)DBNull.Value;
            if (sqlType == typeof(TimeSpan))
                return (object)DBNull.Value;
            if (sqlType == typeof(DateTimeOffset))
                return (object)DBNull.Value;
            else
                return (object)DBNull.Value;
        }

        internal static MetaType PromoteStringType(string s)
        {
            if (s.Length << 1 > 8000)
                return MetaType.MetaVarChar;
            else
                return MetaType.MetaNVarChar;
        }

        internal static object GetComValueFromSqlVariant(object sqlVal)
        {
            if (sqlVal.IsNull())
                return null;

            object obj = null;
            if (sqlVal is SqlSingle)
                obj = (object)((SqlSingle)sqlVal).Value;
            else if (sqlVal is SqlString)
                obj = (object)((SqlString)sqlVal).Value;
            else if (sqlVal is SqlDouble)
                obj = (object)((SqlDouble)sqlVal).Value;
            else if (sqlVal is SqlBinary)
                obj = (object)((SqlBinary)sqlVal).Value;
            else if (sqlVal is SqlGuid)
                obj = (object)((SqlGuid)sqlVal).Value;
            else if (sqlVal is SqlBoolean)
                obj = ((SqlBoolean)sqlVal).Value;
            else if (sqlVal is SqlByte)
                obj = (object)((SqlByte)sqlVal).Value;
            else if (sqlVal is SqlInt16)
                obj = (object)((SqlInt16)sqlVal).Value;
            else if (sqlVal is SqlInt32)
                obj = (object)((SqlInt32)sqlVal).Value;
            else if (sqlVal is SqlInt64)
                obj = (object)((SqlInt64)sqlVal).Value;
            else if (sqlVal is SqlDecimal)
                obj = (object)((SqlDecimal)sqlVal).Value;
            else if (sqlVal is SqlDateTime)
                obj = (object)((SqlDateTime)sqlVal).Value;
            else if (sqlVal is SqlMoney)
                obj = (object)((SqlMoney)sqlVal).Value;
            else if (sqlVal is SqlXml)
                obj = (object)((SqlXml)sqlVal).Value;
            return obj;
        }

        [Conditional("DEBUG")]
        private static void AssertIsUserDefinedTypeInstance(object sqlValue, string failedAssertMessage)
        {
            SqlUserDefinedTypeAttribute[] definedTypeAttributeArray = (SqlUserDefinedTypeAttribute[])sqlValue.GetType().GetCustomAttributes(typeof(SqlUserDefinedTypeAttribute), true);
        }

        internal static object GetSqlValueFromComVariant(object comVal)
        {
            object obj = (object)null;
            if (comVal != null && DBNull.Value != comVal)
            {
                if (comVal is float)
                    obj = (object)new SqlSingle((float)comVal);
                else if (comVal is string)
                    obj = (object)new SqlString((string)comVal);
                else if (comVal is double)
                    obj = (object)new SqlDouble((double)comVal);
                else if (comVal is byte[])
                    obj = (object)new SqlBinary((byte[])comVal);
                else if (comVal is char)
                    obj = (object)new SqlString(((char)comVal).ToString());
                else if (comVal is char[])
                    obj = (object)new SqlChars((char[])comVal);
                else if (comVal is Guid)
                    obj = (object)new SqlGuid((Guid)comVal);
                else if (comVal is bool)
                    obj = (object)new SqlBoolean((bool)comVal);
                else if (comVal is byte)
                    obj = (object)new SqlByte((byte)comVal);
                else if (comVal is short)
                    obj = (object)new SqlInt16((short)comVal);
                else if (comVal is int)
                    obj = (object)new SqlInt32((int)comVal);
                else if (comVal is long)
                    obj = (object)new SqlInt64((long)comVal);
                else if (comVal is Decimal)
                    obj = (object)new SqlDecimal((Decimal)comVal);
                else if (comVal is DateTime)
                    obj = (object)new SqlDateTime((DateTime)comVal);
                else if (comVal is XmlReader)
                    obj = (object)new SqlXml((XmlReader)comVal);
                else if (comVal is TimeSpan || comVal is DateTimeOffset)
                    obj = comVal;
            }
            return obj;
        }

        internal static SqlDbType GetSqlDbTypeFromOleDbType(short dbType, string typeName)
        {
            SqlDbType sqlDbType = SqlDbType.Variant;
            switch ((OleDbType)dbType)
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
                    break;
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

        internal static MetaType GetSqlDataType(int tdsType, uint userType, int length)
        {
            switch (tdsType)
            {
                case 231:
                    return MetaType.MetaNVarChar;
                case 239:
                    return MetaType.MetaNChar;
                case 240:
                    return MetaType.MetaUdt;
                case 241:
                    return MetaType.MetaXml;
                case 243:
                    return MetaType.MetaTable;
                case 165:
                    return MetaType.MetaVarBinary;
                case 167:
                case 39:
                    return MetaType.MetaVarChar;
                case 173:
                case 45:
                    if (80 != (int)userType)
                        return MetaType.MetaBinary;
                    else
                        return MetaType.MetaTimestamp;
                case 175:
                case 47:
                    return MetaType.MetaChar;
                case 122:
                    return MetaType.MetaSmallMoney;
                case (int)sbyte.MaxValue:
                    return MetaType.MetaBigInt;
                case 34:
                    return MetaType.MetaImage;
                case 35:
                    return MetaType.MetaText;
                case 36:
                    return MetaType.MetaUniqueId;
                case 37:
                    return MetaType.MetaSmallVarBinary;
                case 38:
                    if (4 > length)
                    {
                        if (2 != length)
                            return MetaType.MetaTinyInt;
                        else
                            return MetaType.MetaSmallInt;
                    }
                    else if (4 != length)
                        return MetaType.MetaBigInt;
                    else
                        return MetaType.MetaInt;
                case 40:
                    return MetaType.MetaDate;
                case 41:
                    return MetaType.MetaTime;
                case 42:
                    return MetaType.MetaDateTime2;
                case 43:
                    return MetaType.MetaDateTimeOffset;
                case 48:
                    return MetaType.MetaTinyInt;
                case 50:
                case 104:
                    return MetaType.MetaBit;
                case 52:
                    return MetaType.MetaSmallInt;
                case 56:
                    return MetaType.MetaInt;
                case 58:
                    return MetaType.MetaSmallDateTime;
                case 59:
                    return MetaType.MetaReal;
                case 60:
                    return MetaType.MetaMoney;
                case 61:
                    return MetaType.MetaDateTime;
                case 62:
                    return MetaType.MetaFloat;
                case 98:
                    return MetaType.MetaVariant;
                case 99:
                    return MetaType.MetaNText;
                case 106:
                case 108:
                    return MetaType.MetaDecimal;
                case 109:
                    if (4 != length)
                        return MetaType.MetaFloat;
                    else
                        return MetaType.MetaReal;
                case 110:
                    if (4 != length)
                        return MetaType.MetaMoney;
                    else
                        return MetaType.MetaSmallMoney;
                case 111:
                    if (4 != length)
                        return MetaType.MetaDateTime;
                    else
                        return MetaType.MetaSmallDateTime;
                default:
                    throw new ArgumentOutOfRangeException("tdsType");
            }
        }

        internal static MetaType GetDefaultMetaType()
        {
            return MetaType.MetaNVarChar;
        }

        internal static string GetStringFromXml(XmlReader xmlreader)
        {
            return new SqlXml(xmlreader).Value;
        }

        public static TdsDateTime FromDateTime(DateTime dateTime, byte cb)
        {
            TdsDateTime tdsDateTime = new TdsDateTime();
            SqlDateTime sqlDateTime;
            if ((int)cb == 8)
            {
                sqlDateTime = new SqlDateTime(dateTime);
                tdsDateTime.Time = sqlDateTime.TimeTicks;
            }
            else
            {
                sqlDateTime = new SqlDateTime(dateTime.AddSeconds(30.0));
                tdsDateTime.Time = sqlDateTime.TimeTicks / SqlDateTime.SQLTicksPerMinute;
            }
            tdsDateTime.Days = sqlDateTime.DayTicks;
            return tdsDateTime;
        }

        public static DateTime ToDateTime(int sqlDays, int sqlTime, int length)
        {
            if (length == 4)
                return new SqlDateTime(sqlDays, sqlTime * SqlDateTime.SQLTicksPerMinute).Value;
            else
                return new SqlDateTime(sqlDays, sqlTime).Value;
        }

        internal static int GetTimeSizeFromScale(byte scale)
        {
            if ((int)scale <= 2)
                return 3;
            return (int)scale <= 4 ? 4 : 5;
        }
    }
}
