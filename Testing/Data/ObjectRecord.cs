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
        public readonly RecordSetDefinition RecordSetDefinition;

        /// <summary>
        /// The column values.
        /// </summary>
        [NotNull]
        private readonly object[] _columnValues;

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
        /// <param name="recordSetDefinition">The table definition.</param>
        /// <param name="randomData">if set to <see langword="true" /> fills columns with random data; otherwise fills them with SQL null values.</param>
        /// <param name="nullProbability">The probability of a column's value being set to SQL null (0.0 for no nulls) - 
        /// this is only applicable is <see cref="randomData"/> is set to <see langword="true"/> [Defaults to 0.1 = 10%].</param>
        /// <remarks></remarks>
        public ObjectRecord([NotNull]RecordSetDefinition recordSetDefinition, bool randomData = true, double nullProbability = 0.1)
        {
            RecordSetDefinition = recordSetDefinition;
            int columnCount = recordSetDefinition.ColumnsArray.Length;
            _columnValues = new object[columnCount];

            if (!randomData)
            {
                // Just fill with nulls
                for (int c = 0; c < columnCount; c++)
                    _columnValues[c] = recordSetDefinition.ColumnsArray[c].NullValue;
            } else
            {
                // Fill values with random data
                for (int c = 0; c < columnCount; c++)
                    _columnValues[c] = recordSetDefinition.ColumnsArray[c].GetRandomValue(nullProbability);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectRecord" /> class.
        /// </summary>
        /// <param name="recordSetDefinition">The table definition.</param>
        /// <param name="columnValues">The column values.</param>
        /// <remarks></remarks>
        public ObjectRecord([NotNull]RecordSetDefinition recordSetDefinition, [NotNull]params object[] columnValues)
        {
            RecordSetDefinition = recordSetDefinition;
            _columnValues = columnValues;
            // TODO Validate column values.
        }

        /// <inhertidoc />
        public string GetName(int i)
        {
            return RecordSetDefinition.ColumnsArray[i].Name;
        }

        /// <inhertidoc />
        public string GetDataTypeName(int i)
        {
            return RecordSetDefinition.ColumnsArray[i].TypeName;
        }

        /// <inhertidoc />
        public Type GetFieldType(int i)
        {
            return RecordSetDefinition.ColumnsArray[i].ClassType;
        }

        /// <inhertidoc />
        public object GetValue(int i)
        {
            return _columnValues[i];
        }

        /// <inhertidoc />
        public int GetValues(object[] values)
        {
            if (values == null)
                throw new NullReferenceException();
            int length = values.Length < _columnValues.Length ? values.Length : _columnValues.Length;
            Array.Copy(_columnValues, values, length);
            return length;
        }

        /// <inhertidoc />
        public int GetOrdinal(string name)
        {
            CompareInfo compare = CultureInfo.InvariantCulture.CompareInfo;
            for (int c =0; c < RecordSetDefinition.ColumnsArray.Length; c++)
            {
                if (compare.Compare(RecordSetDefinition.ColumnsArray[c].Name, name, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth) == 0)
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
            get { return RecordSetDefinition.FieldCount; }
        }

        /// <inhertidoc />
        public object this[int i]
        {
            get { return GetValue(i); }
        }

        /// <inhertidoc />
        public object this[string name]
        {
            get { return GetValue(GetOrdinal(name)); }
        }
    }
}