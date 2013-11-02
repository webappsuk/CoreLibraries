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
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics.Contracts;
using JetBrains.Annotations;
using Microsoft.SqlServer.Types;

namespace WebApplications.Testing.Data
{
    /// <summary>
    /// Holds information about a column.
    /// </summary>
    public class ColumnDefinition
    {
        /// <summary>
        /// The .NET <see cref="Type"/> that this column accepts.
        /// </summary>
        [NotNull] public readonly Type ClassType;

        /// <summary>
        /// The <see cref="DbType"/>.
        /// </summary>
        public readonly DbType DbType;

        /// <summary>
        /// Whether column should be full.
        /// </summary>
        /// <remarks>
        /// If this is set to <see langword="true"/> then randomly filled columns will be filled, and
        /// explicitly set values must match the length; otherwise the length indicates a maximum.
        /// </remarks>
        public readonly bool Fill;

        /// <summary>
        /// The fixed length for the type (-1 indicates variable length - i.e. 'var' types).
        /// </summary>
        public readonly int FixedLength;

        /// <summary>
        /// The column name.
        /// </summary>
        [NotNull] public readonly string Name;

        /// <summary>
        /// The <see cref="SqlDbType"/>.
        /// </summary>
        public readonly SqlDbType SqlDbType;

        /// <summary>
        /// The SQL Clr Type for the column.
        /// </summary>
        [NotNull] public readonly Type SqlType;

        /// <summary>
        /// Whether the column is nullable.
        /// </summary>
        public readonly bool IsIsNullable;

        /// <summary>
        /// The columns default value.
        /// </summary>
        public readonly object DefaultValue;

        /// <summary>
        /// The equivalent SQL Server type name.
        /// </summary>
        [NotNull] public readonly string TypeName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnDefinition" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="dbType">The columns type.</param>
        /// <param name="length">The length (if fixed length).</param>
        /// <param name="fill">if set to <see langword="true" /> expects the column to be full (only appropriate for fixed length columns).</param>
        /// <param name="isNullable">if set to <see langword="true" /> the column is nullable.</param>
        /// <param name="defaultValue">The default value (required if column is not nullable).</param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        ///   <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <remarks></remarks>
        public ColumnDefinition([NotNull] string name, DbType dbType, int length = -1, bool fill = false, bool isNullable = true, [CanBeNull] object defaultValue = null)
            : this(name, dbType.ToSqlDbType(), length, fill, isNullable, defaultValue)
        {
            Contract.Requires(name != null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnDefinition" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="sqlDbType">The columns type.</param>
        /// <param name="length">The length (if fixed length).</param>
        /// <param name="fill">if set to <see langword="true" /> expects the column to be full (only appropriate for fixed length columns).</param>
        /// <param name="isNullable">if set to <see langword="true" /> the column is nullable.</param>
        /// <param name="defaultValue">The default value (required if column is not nullable).</param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <remarks></remarks>
        public ColumnDefinition([NotNull] string name, SqlDbType sqlDbType, int length = -1, bool fill = false, bool isNullable = true, [CanBeNull]object defaultValue = null)
        {
            Contract.Requires(name != null);
            Name = name;
            SqlDbType = sqlDbType;
            Fill = fill;
            IsIsNullable = isNullable;
            switch (sqlDbType)
            {
                case SqlDbType.BigInt:
                    DbType = DbType.Int64;
                    TypeName = "bigint";
                    ClassType = typeof (long);
                    SqlType = typeof (SqlInt64);
                    FixedLength = 8;
                    break;
                case SqlDbType.Binary:
                    DbType = DbType.Binary;
                    TypeName = "binary";
                    ClassType = typeof (byte[]);
                    SqlType = typeof (SqlBinary);
                    FixedLength = -1;
                    break;
                case SqlDbType.Bit:
                    DbType = DbType.Boolean;
                    TypeName = "bit";
                    ClassType = typeof (bool);
                    SqlType = typeof (SqlBoolean);
                    FixedLength = 1;
                    break;
                case SqlDbType.Char:
                    DbType = DbType.AnsiStringFixedLength;
                    TypeName = "char";
                    ClassType = typeof (string);
                    SqlType = typeof (SqlString);
                    FixedLength = -1;
                    break;
                case SqlDbType.DateTime:
                    DbType = DbType.DateTime;
                    TypeName = "datetime";
                    ClassType = typeof (DateTime);
                    SqlType = typeof (SqlDateTime);
                    FixedLength = 8;
                    break;
                case SqlDbType.Decimal:
                    DbType = DbType.Decimal;
                    TypeName = "decimal";
                    ClassType = typeof (Decimal);
                    SqlType = typeof (SqlDecimal);
                    FixedLength = 17;
                    break;
                case SqlDbType.Float:
                    DbType = DbType.Double;
                    TypeName = "float";
                    ClassType = typeof (double);
                    SqlType = typeof (SqlDouble);
                    FixedLength = 8;
                    break;
                case SqlDbType.Image:
                    DbType = DbType.Double;
                    TypeName = "image";
                    ClassType = typeof (byte[]);
                    SqlType = typeof (SqlBinary);
                    FixedLength = -1;
                    break;
                case SqlDbType.Int:
                    DbType = DbType.Int32;
                    TypeName = "int";
                    ClassType = typeof (int);
                    SqlType = typeof (SqlInt32);
                    FixedLength = 4;
                    break;
                case SqlDbType.Money:
                    DbType = DbType.Currency;
                    TypeName = "money";
                    ClassType = typeof (Decimal);
                    SqlType = typeof (SqlMoney);
                    FixedLength = 8;
                    break;
                case SqlDbType.NChar:
                    DbType = DbType.StringFixedLength;
                    TypeName = "nchar";
                    ClassType = typeof (string);
                    SqlType = typeof (SqlString);
                    FixedLength = -1;
                    break;
                case SqlDbType.NText:
                    DbType = DbType.String;
                    TypeName = "ntext";
                    ClassType = typeof (string);
                    SqlType = typeof (SqlString);
                    FixedLength = -1;
                    break;
                case SqlDbType.NVarChar:
                    DbType = DbType.String;
                    TypeName = "nvarchar";
                    ClassType = typeof (string);
                    SqlType = typeof (SqlString);
                    FixedLength = -1;
                    break;
                case SqlDbType.Real:
                    DbType = DbType.Single;
                    TypeName = "real";
                    ClassType = typeof (float);
                    SqlType = typeof (SqlSingle);
                    FixedLength = 4;
                    break;
                case SqlDbType.UniqueIdentifier:
                    DbType = DbType.Guid;
                    TypeName = "uniqueidentifier";
                    ClassType = typeof (Guid);
                    SqlType = typeof (SqlGuid);
                    FixedLength = 16;
                    break;
                case SqlDbType.SmallDateTime:
                    DbType = DbType.DateTime;
                    TypeName = "smalldatetime";
                    ClassType = typeof (DateTime);
                    SqlType = typeof (SqlDateTime);
                    FixedLength = 4;
                    break;
                case SqlDbType.SmallInt:
                    DbType = DbType.Int16;
                    TypeName = "smallint";
                    ClassType = typeof (short);
                    SqlType = typeof (SqlInt16);
                    FixedLength = 2;
                    break;
                case SqlDbType.SmallMoney:
                    DbType = DbType.Currency;
                    TypeName = "smallmoney";
                    ClassType = typeof (Decimal);
                    SqlType = typeof (SqlMoney);
                    FixedLength = 4;
                    break;
                case SqlDbType.Text:
                    DbType = DbType.AnsiString;
                    TypeName = "text";
                    ClassType = typeof (string);
                    SqlType = typeof (SqlString);
                    FixedLength = -1;
                    break;
                case SqlDbType.Timestamp:
                    DbType = DbType.Binary;
                    TypeName = "timestamp";
                    ClassType = typeof (byte[]);
                    SqlType = typeof (SqlBinary);
                    FixedLength = 8;
                    break;
                case SqlDbType.TinyInt:
                    DbType = DbType.Byte;
                    TypeName = "tinyint";
                    ClassType = typeof (byte);
                    SqlType = typeof (SqlByte);
                    FixedLength = -1;
                    break;
                case SqlDbType.VarBinary:
                    DbType = DbType.Binary;
                    TypeName = "varbinary";
                    ClassType = typeof (byte[]);
                    SqlType = typeof (SqlBinary);
                    FixedLength = -1;
                    break;
                case SqlDbType.VarChar:
                    DbType = DbType.AnsiString;
                    TypeName = "varchar";
                    ClassType = typeof (string);
                    SqlType = typeof (SqlString);
                    FixedLength = -1;
                    break;
                case SqlDbType.Variant:
                    DbType = DbType.Object;
                    TypeName = "sql_variant";
                    ClassType = typeof (object);
                    SqlType = typeof (object);
                    FixedLength = -1;
                    break;
                case SqlDbType.Xml:
                    DbType = DbType.Xml;
                    TypeName = "xml";
                    ClassType = typeof (string);
                    SqlType = typeof (SqlXml);
                    FixedLength = -1;
                    break;
                case SqlDbType.Udt:
                    DbType = DbType.Object;
                    TypeName = "udt";
                    ClassType = typeof (object);
                    SqlType = typeof (object);
                    FixedLength = -1;
                    break;
                case SqlDbType.Date:
                    DbType = DbType.Date;
                    TypeName = "date";
                    ClassType = typeof (DateTime);
                    SqlType = typeof (DateTime);
                    FixedLength = 3;
                    break;
                case SqlDbType.Time:
                    DbType = DbType.Time;
                    TypeName = "time";
                    ClassType = typeof (TimeSpan);
                    SqlType = typeof (TimeSpan);
                    FixedLength = -1;
                    break;
                case SqlDbType.DateTime2:
                    DbType = DbType.DateTime2;
                    TypeName = "datetime2";
                    ClassType = typeof (DateTime);
                    SqlType = typeof (DateTime);
                    FixedLength = -1;
                    break;
                case SqlDbType.DateTimeOffset:
                    DbType = DbType.DateTimeOffset;
                    TypeName = "datetimeoffset";
                    ClassType = typeof (DateTimeOffset);
                    SqlType = typeof (DateTimeOffset);
                    FixedLength = -1;
                    break;
                default:
                    // NB SqlDbType.Structured is not valid for a column.
                    throw new ArgumentOutOfRangeException("sqlDbType");
            }

            // If we are fixed length type then fill.
            if (FixedLength > -1)
                Fill = true;

            // Set length if specified
            if (length > -1)
            {
                // If we already have a fixed length, we can't set explicitly.
                if (FixedLength > -1)
                    throw new ArgumentOutOfRangeException("length", length,
                        string.Format(
                            "The '{0}' sql type has a fixed length of '{1}' and so cannot be set explicitly to '{2}'.",
                            SqlDbType,
                            FixedLength,
                            length));
                FixedLength = length;
            }

            if (!Validate(defaultValue, out DefaultValue))
                throw new ArgumentOutOfRangeException("defaultValue", defaultValue,
                    string.Format(
                        "The '{0}' column's default value is not valid.", this));
        }

        /// <summary>
        /// Gets the record set definition.
        /// </summary>
        /// <value>The record set definition.</value>
        [NotNull]
        public RecordSetDefinition RecordSetDefinition { get; internal set; }

        /// <summary>
        /// Gets the ordinal.
        /// </summary>
        /// <value>The ordinal.</value>
        /// <remarks></remarks>
        public int Ordinal { get; internal set; }

        /// <summary>
        /// The SQL Null value for this column.
        /// </summary>
        [NotNull]
        public object NullValue
        {
            get
            {
                if (!IsIsNullable)
                    throw new InvalidOperationException("The column cannot is not nullable.");
                return SqlDbType.NullValue();
            }
        }

        /// <summary>
        /// Whether the length is fixed.
        /// </summary>
        public bool IsFixedLength
        {
            get { return FixedLength > -1; }
        }

        /// <summary>
        /// Gets the random value.
        /// </summary>
        /// <param name="nullProbability">The probability of a column's value being set to SQL null (0.0 for no nulls) [Defaults to 0.0 = 0%].</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public object GetRandomValue(double nullProbability = 0.0)
        {
            return Tester.RandomGenerator.RandomSqlValue(SqlDbType, FixedLength, IsIsNullable ? nullProbability : 0.0,
                                                         Fill);
        }

        /// <summary>
        /// Validates the specified value, to see if it is valid for this column.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="sqlValue">The SQL value.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <remarks></remarks>
        public bool Validate(object value, out object sqlValue)
        {
            if (value.IsNull())
            {
                sqlValue = NullValue;
                return IsIsNullable;
            }

            if (!ClassType.IsInstanceOfType(value))
            {
                sqlValue = DBNull.Value;
                return false;
            }

            Contract.Assert(value != null);
            switch (SqlDbType)
            {
                case SqlDbType.BigInt:
                    sqlValue = (Int64) value;
                    return true;
                case SqlDbType.Binary:
                case SqlDbType.Image:
                case SqlDbType.Timestamp:
                case SqlDbType.VarBinary:
                    byte[] bytes = (byte[]) value;
                    sqlValue = bytes;
                    return FixedLength < 0 ||
                           (bytes.Length <= FixedLength);
                case SqlDbType.Bit:
                    sqlValue = (bool) value;
                    return true;
                case SqlDbType.Text:
                case SqlDbType.Char:
                case SqlDbType.VarChar:
                    string s = (string) value;
                    sqlValue = s;
                    return FixedLength < 0 ||
                           (s.Length <= FixedLength);
                case SqlDbType.DateTime:
                case SqlDbType.DateTime2:
                    sqlValue = (DateTime) value;
                    return true;
                case SqlDbType.Decimal:
                case SqlDbType.Money:
                case SqlDbType.SmallMoney:
                    sqlValue = (decimal) value;
                    return true;
                case SqlDbType.Float:
                    sqlValue = (double) value;
                    return true;
                case SqlDbType.Int:
                    sqlValue = (int) value;
                    return true;
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.Xml:
                    string ns = (string) value;
                    sqlValue = ns;
                    return FixedLength < 0 ||
                           (ns.Length <= (FixedLength/2));
                case SqlDbType.Real:
                    sqlValue = (float) value;
                    return true;
                case SqlDbType.UniqueIdentifier:
                    sqlValue = (Guid) value;
                    return true;
                case SqlDbType.SmallDateTime:
                    DateTime dt = (DateTime) value;
                    sqlValue = dt;
                    return dt >= Tester.MinSmallDateTime && dt <= Tester.MaxSmallDateTime;
                case SqlDbType.SmallInt:
                    sqlValue = (Int16) value;
                    return true;
                case SqlDbType.TinyInt:
                    sqlValue = (byte) value;
                    return true;
                case SqlDbType.Variant:
                    sqlValue = value;
                    return true;
                case SqlDbType.Udt:
                    sqlValue = value;
                    SqlGeography sqlGeography = value as SqlGeography;
                    if (sqlGeography != null)
                        return true;
                    SqlGeometry sqlGeometry = value as SqlGeometry;
                    if (sqlGeometry != null)
                        return true;
                    return value is SqlHierarchyId;
                case SqlDbType.Date:
                    sqlValue = ((DateTime) value).Date;
                    return true;
                case SqlDbType.Time:
                    sqlValue = (TimeSpan) value;
                    return true;
                case SqlDbType.DateTimeOffset:
                    sqlValue = (DateTimeOffset) value;
                    return true;
                default:
                    throw new ArgumentOutOfRangeException("SqlDbType", SqlDbType,
                                                          string.Format("Unsupported SqlDbType for column '{0}'", this));
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("Column #{0} \"{1}\" {2}{3}{4}{5}",
                                 Ordinal,
                                 Name,
                                 SqlDbType,
                                 IsFixedLength ? "[" + FixedLength + "]" : string.Empty,
                                 IsIsNullable ? string.Empty : " NOT NULL",
                                 !DefaultValue.IsNull() ? " (Default Value='" + DefaultValue + "')" : string.Empty);
        }
    }
}