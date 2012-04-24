using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.SqlServer.Server;

namespace WebApplications.Testing.Data
{
    /// <summary>
    /// Implements a record.
    /// </summary>
    /// <remarks></remarks>
    public class ObjectRecord : IDataRecord
    {
        /// <summary>
        /// The current table definition.
        /// </summary>
        [NotNull]
        public readonly RecordSetDefinition TableDefinition;

        /// <summary>
        /// The column values.
        /// </summary>
        [NotNull]
        private readonly object[] _columnValues;

        /// <summary>
        /// Holds columns as an array.
        /// </summary>
        [NotNull]
        private readonly ColumnDefinition[] _columnsDefinition;

        /// <summary>
        /// Gets the column values.
        /// </summary>
        /// <value>The column data.</value>
        /// <remarks></remarks>
        [NotNull]
        public IEnumerable<object> ColumnValues { get { return _columnValues; }}

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectRecord" /> class.
        /// </summary>
        /// <param name="tableDefinition">The table definition.</param>
        /// <param name="columnValues">The column values.</param>
        /// <remarks></remarks>
        public ObjectRecord([NotNull]RecordSetDefinition tableDefinition, [NotNull]params object[] columnValues)
        {
            TableDefinition = tableDefinition;
            _columnsDefinition = TableDefinition.Columns.ToArray();
            _columnValues = columnValues;
            // TODO Validate column values.
        }

        /// <inhertidoc />
        public string GetName(int i)
        {
            return _columnsDefinition[i].Name;
        }

        /// <inhertidoc />
        public string GetDataTypeName(int i)
        {
            return _columnsDefinition[i].MetaType.TypeName;
        }

        /// <inhertidoc />
        public Type GetFieldType(int i)
        {
            return _columnsDefinition[i].MetaType.ClassType;
        }

        /// <inhertidoc />
        public object GetValue(int i)
        {
            return _columnsDefinition[i];
        }

        /// <inhertidoc />
        public int GetValues(object[] values)
        {
            if (values == null)
                throw new NullReferenceException();
            int length = values.Length < _columnsDefinition.Length ? values.Length : _columnsDefinition.Length;
            Array.Copy(_columnsDefinition, values, length);
            return length;
        }

        /// <inhertidoc />
        public int GetOrdinal(string name)
        {
            CompareInfo compare = CultureInfo.InvariantCulture.CompareInfo;
            for (int c =0; c < _columnsDefinition.Length; c++)
            {
                if (compare.Compare(_columnsDefinition[c].Name, name, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth) == 0)
                    return c;
            }
            throw new IndexOutOfRangeException();
        }

        /// <inhertidoc />
        public bool GetBoolean(int i)
        {
            object o = _columnValues[i];
            if (o == null)
                throw new SqlNullValueException();
            if (!(o is bool))
                throw new InvalidCastException();
            return (bool) o;
        }

        /// <inhertidoc />
        public byte GetByte(int i)
        {
            object o = _columnValues[i];
            if (o == null)
                throw new SqlNullValueException();
            if (!(o is byte))
                throw new InvalidCastException();
            return (byte)o;
        }

        /// <inhertidoc />
        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            if (buffer == null)
                return 0;

            object o = _columnValues[i];
            if (o == null)
                throw new SqlNullValueException();
            if (!(o is byte[]))
                throw new InvalidCastException();
            byte[] bytes = (byte[]) o;
            length = (bytes.Length - fieldOffset) < length ? (int)(bytes.Length-fieldOffset) : length;
            Array.Copy(bytes, fieldOffset, buffer, bufferoffset, length);
            return length;
        }

        /// <inhertidoc />
        public char GetChar(int i)
        {
            object o = _columnValues[i];
            if (o == null)
                throw new SqlNullValueException();
            if (!(o is char))
                throw new InvalidCastException();
            return (char)o;
        }

        /// <inhertidoc />
        public long GetChars(int i, long fieldOffset, char[] buffer, int bufferoffset, int length)
        {
            if (buffer == null)
                return 0;

            object o = _columnValues[i];
            if (o == null)
                throw new SqlNullValueException();
            if (!(o is char[]))
                throw new InvalidCastException();
            char[] chars = (char[])o;
            length = (chars.Length - fieldOffset) < length ? (int)(chars.Length - fieldOffset) : length;
            Array.Copy(chars, fieldOffset, buffer, bufferoffset, length);
            return length;
        }

        /// <inhertidoc />
        public Guid GetGuid(int i)
        {
            object o = _columnValues[i];
            if (o == null)
                throw new SqlNullValueException();
            if (!(o is Guid))
                throw new InvalidCastException();
            return (Guid)o;
        }

        /// <inhertidoc />
        public short GetInt16(int i)
        {
            object o = _columnValues[i];
            if (o == null)
                throw new SqlNullValueException();
            if (!(o is short))
                throw new InvalidCastException();
            return (short)o;
        }

        /// <inhertidoc />
        public int GetInt32(int i)
        {
            object o = _columnValues[i];
            if (o == null)
                throw new SqlNullValueException();
            if (!(o is int))
                throw new InvalidCastException();
            return (int)o;
        }

        /// <inhertidoc />
        public long GetInt64(int i)
        {
            object o = _columnValues[i];
            if (o == null)
                throw new SqlNullValueException();
            if (!(o is long))
                throw new InvalidCastException();
            return (long)o;
        }

        /// <inhertidoc />
        public float GetFloat(int i)
        {
            object o = _columnValues[i];
            if (o == null)
                throw new SqlNullValueException();
            if (!(o is float))
                throw new InvalidCastException();
            return (float)o;
        }

        /// <inhertidoc />
        public double GetDouble(int i)
        {
            object o = _columnValues[i];
            if (o == null)
                throw new SqlNullValueException();
            if (!(o is double))
                throw new InvalidCastException();
            return (double)o;
        }

        /// <inhertidoc />
        public string GetString(int i)
        {
            object o = _columnValues[i];
            if (!(o is string))
                throw new InvalidCastException();
            return (string)o;
        }

        /// <inhertidoc />
        public decimal GetDecimal(int i)
        {
            object o = _columnValues[i];
            if (o == null)
                throw new SqlNullValueException();
            if (!(o is decimal))
                throw new InvalidCastException();
            return (decimal)o;
        }

        /// <inhertidoc />
        public DateTime GetDateTime(int i)
        {
            object o = _columnValues[i];
            if (o == null)
                throw new SqlNullValueException();
            if (!(o is DateTime))
                throw new InvalidCastException();
            return (DateTime)o;
        }

        /// <inhertidoc />
        IDataReader IDataRecord.GetData(int i)
        {
            throw new NotSupportedException();
        }

        /// <inhertidoc />
        public bool IsDBNull(int i)
        {
            return _columnValues[i] == null;
        }

        /// <inhertidoc />
        public int FieldCount
        {
            get { return _columnsDefinition.Length; }
        }

        /// <inhertidoc />
        public object this[int i]
        {
            get { return _columnsDefinition[i]; }
        }

        /// <inhertidoc />
        public object this[string name]
        {
            get { return _columnsDefinition[GetOrdinal(name)]; }
        }
    }
}