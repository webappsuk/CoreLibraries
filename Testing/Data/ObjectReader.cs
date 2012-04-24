using System;
using System.Collections.Generic;
using System.Data;
using JetBrains.Annotations;

namespace WebApplications.Testing.Data
{
    public class ObjectReader : IDataReader
    {
        /// <summary>
        /// The internal enumeror.
        /// </summary>
        [NotNull]
        private readonly IEnumerator<object[]> _enumerator;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectReader" /> class.
        /// </summary>
        /// <param name="objects">The objects.</param>
        /// <remarks></remarks>
        private ObjectReader()
        {
        }

        public void AddRecordSet(RecordSetDefinition recordSetDefinition)
        {
            
        }

        /// <summary>
        /// Adds a random record set.
        /// </summary>
        /// <param name="maxColumns">The max number of columns, recordset will have between 1 and max columns inclusive [defaults to 10].</param>
        /// <param name="minRows">The minimum number of rows [defaults to 0].</param>
        /// <param name="maxRows">The maximum number of rows [defaults to 1000].</param>
        /// <remarks></remarks>
        public void AddRandomRecordSet(
            int maxColumns = 10,
            int minRows = 0,
            int maxRows = 1000)
        {
            if (maxColumns < 1)
                throw new ArgumentOutOfRangeException("maxColumns", maxColumns,
                                                      String.Format(
                                                          "The maximum number of columns '{0}' must be greater than 1.",
                                                          maxColumns));

            int columnCounts = Tester.RandomGenerator.Next(maxRows) + 1;
            string[] columnNames = new string[columnCounts];
            Type[] types = new Type[columnCounts];

        }

        public void AddRandomRecordSet(
            [NotNull]string[] columnNames,
            [CanBeNull]Type[] columnTypes = null,
            int minRows = 0,
            int maxRows = 1000)
        {
            int columnCount = columnNames.Length;
            if (columnTypes != null)
            {
                if (columnTypes.Length != columnCount)
                    throw new ArgumentOutOfRangeException("columnTypes", columnTypes,
                                                          String.Format(
                                                              "The number of column types '{0}' must match the number of column names '{1}'.",
                                                              columnTypes.Length,
                                                              columnCount));
            }

            ColumnDefinition[] columnDefinition = new ColumnDefinition[columnCount];
            for (int c = 0; c < columnCount; c++)
            {
                Type columnType = columnTypes != null
                                      ? columnTypes[c]
                                      : typeof (int); // TODO Randomise

                // TODO tableDefinition.AddColumn(new SqlColumn(c, columnNames[c],));
            }

            RecordSetDefinition recordSetDefinition = new RecordSetDefinition(columnDefinition);

            // Add the record set with the new fake table definition.
            AddRandomRecordSet(recordSetDefinition, minRows, maxRows);
        }

        public void AddRandomRecordSet(RecordSetDefinition recordSetDefinition, int minRows = 0, int maxRows = 1000)
        {
            if (minRows < 0)
                throw new ArgumentOutOfRangeException("minRows", minRows,
                                                      String.Format(
                                                          "The minimum number of rows '{0}' cannot be negative.",
                                                          minRows));
            if (maxRows < 0)
                throw new ArgumentOutOfRangeException("maxRows", maxRows,
                                                      String.Format(
                                                          "The minimum number of rows '{0}' cannot be negative.",
                                                          maxRows));

            if (minRows > maxRows)
            {
                throw new ArgumentOutOfRangeException("minRows", minRows,
                                                      String.Format(
                                                          "The minimum number of rows '{0}' cannot exceed the maximum number of rows '{1}'.",
                                                          minRows,
                                                          maxRows));
            }

            // Calculate number of rows.
            int rows = minRows == maxRows
                           ? minRows
                           : Tester.RandomGenerator.Next(minRows, maxRows);
            
            // Add the record set.
            AddRecordSet(recordSetDefinition);

            // Generate fake data.
            for (int row = 0; row < rows; row++)
            {

            }
        }

        /// <summary>
        /// Resets the current enumerator.
        /// </summary>
        /// <remarks></remarks>
        public void Reset()
        {
            _enumerator.Reset();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _enumerator.Dispose();
        }

        /// <inheritdoc/>
        public string GetName(int i)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Type GetFieldType(int i)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public object GetValue(int i)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public int GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool GetBoolean(int i)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public byte GetByte(int i)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public char GetChar(int i)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public short GetInt16(int i)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public int GetInt32(int i)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public long GetInt64(int i)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public float GetFloat(int i)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public double GetDouble(int i)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public string GetString(int i)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public decimal GetDecimal(int i)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public DateTime GetDateTime(int i)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool IsDBNull(int i)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public int FieldCount
        {
            get { throw new NotImplementedException(); }
        }

        /// <inheritdoc/>
        object IDataRecord.this[int i]
        {
            get { throw new NotImplementedException(); }
        }

        /// <inheritdoc/>
        object IDataRecord.this[string name]
        {
            get { throw new NotImplementedException(); }
        }

        /// <inheritdoc/>
        public void Close()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool NextResult()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool Read()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public int Depth
        {
            get { return 0; }
        }

        /// <inheritdoc/>
        public bool IsClosed
        {
            get { throw new NotImplementedException(); }
        }

        /// <inheritdoc/>
        public int RecordsAffected
        {
            get { return -1; }
        }
    }
}