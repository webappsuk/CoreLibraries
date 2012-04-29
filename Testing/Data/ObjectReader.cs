using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace WebApplications.Testing.Data
{
    /// <summary>
    /// The object reader.
    /// </summary>
    /// <remarks></remarks>
    public class ObjectReader : IDataReader, ICollection<IObjectRecordSet>
    {
        /// <summary>
        /// Holds the internal record sets.
        /// </summary>
        [NotNull]
        private readonly List<IObjectRecordSet> _recordSets = new List<IObjectRecordSet>();

        /// <summary>
        /// The internal enumeror over the sets.
        /// </summary>
        [CanBeNull]
        private IEnumerator<IObjectRecordSet> _setEnumerator;

        /// <summary>
        /// The internal enumeror over the records in the current set.
        /// </summary>
        [CanBeNull]
        private IEnumerator<IObjectRecord> _recordEnumerator;

        /// <summary>
        /// Whether the reader is closed.
        /// </summary>
        private bool _isClosed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectReader" /> class.
        /// </summary>
        /// <remarks></remarks>
        private ObjectReader()
        {
        }

        /// <summary>
        /// Gets the current record.
        /// </summary>
        /// <value>The current record.</value>
        /// <remarks></remarks>
        [NotNull]
        public IObjectRecordSet CurrentSet
        {
            get
            {
                if (_isClosed)
                    throw new InvalidOperationException("Data reader is closed.");

                // Create a set enumerator.
                if (_setEnumerator == null)
                    _setEnumerator = _recordSets.GetEnumerator();

                IObjectRecordSet currentSet = _setEnumerator.Current;
                if (currentSet == null)
                    throw new InvalidOperationException("Reached end of recordsets.");

                return currentSet;
            }
        }

        /// <summary>
        /// Gets the current record.
        /// </summary>
        /// <value>The current record.</value>
        /// <remarks></remarks>
        [NotNull]
        public IObjectRecord Current
        {
            get
            {
                if (_isClosed)
                    throw new InvalidOperationException("Data reader is closed.");

                // If we haven't got a record enumerator create one.
                if (_recordEnumerator == null)
                    _recordEnumerator = CurrentSet.GetEnumerator();

                if (_recordEnumerator.Current == null)
                    throw new InvalidOperationException("Reached end of the current recordset.");

                return _recordEnumerator.Current;
            }
        }

        /// <summary>
        /// Resets the data reader, clearing enumerators, re-opening and allowing recordset collection to be modified.
        /// </summary>
        /// <remarks></remarks>
        public void Reset()
        {
            _isClosed = false;
            Dispose();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_setEnumerator != null)
                _setEnumerator.Dispose();
            _setEnumerator = null;
            if (_recordEnumerator != null)
                _recordEnumerator.Dispose();
            _recordEnumerator = null;
        }

        /// <inheritdoc/>
        public string GetName(int i)
        {
            if (_isClosed)
                throw new InvalidOperationException("Data reader is closed.");
            return CurrentSet.Definition[i].Name;
        }

        /// <inheritdoc/>
        public string GetDataTypeName(int i)
        {
            if (_isClosed)
                throw new InvalidOperationException("Data reader is closed.");
            return CurrentSet.Definition[i].TypeName;
        }

        /// <inheritdoc/>
        public Type GetFieldType(int i)
        {
            if (_isClosed)
                throw new InvalidOperationException("Data reader is closed.");
            return CurrentSet.Definition[i].ClassType;
        }

        /// <inheritdoc/>
        public object GetValue(int i)
        {
            if (_isClosed)
                throw new InvalidOperationException("Data reader is closed.");
            return Current.GetValue(i);
        }

        /// <inheritdoc/>
        public int GetValues(object[] values)
        {
            if (_isClosed)
                throw new InvalidOperationException("Data reader is closed.");
            return Current.GetValues(values);
        }

        /// <inheritdoc/>
        public int GetOrdinal(string name)
        {
            if (_isClosed)
                throw new InvalidOperationException("Data reader is closed.");
            return CurrentSet.Definition.GetOrdinal(name);
        }

        /// <inheritdoc/>
        public bool GetBoolean(int i)
        {
            if (_isClosed)
                throw new InvalidOperationException("Data reader is closed.");
            return Current.GetBoolean(i);
        }

        /// <inheritdoc/>
        public byte GetByte(int i)
        {
            if (_isClosed)
                throw new InvalidOperationException("Data reader is closed.");
            return Current.GetByte(i);
        }

        /// <inheritdoc/>
        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            if (_isClosed)
                throw new InvalidOperationException("Data reader is closed.");
            return Current.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        /// <inheritdoc/>
        public char GetChar(int i)
        {
            if (_isClosed)
                throw new InvalidOperationException("Data reader is closed.");
            return Current.GetChar(i);
        }

        /// <inheritdoc/>
        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            if (_isClosed)
                throw new InvalidOperationException("Data reader is closed.");
            return Current.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        /// <inheritdoc/>
        public Guid GetGuid(int i)
        {
            if (_isClosed)
                throw new InvalidOperationException("Data reader is closed.");
            return Current.GetGuid(i);
        }

        /// <inheritdoc/>
        public short GetInt16(int i)
        {
            if (_isClosed)
                throw new InvalidOperationException("Data reader is closed.");
            return Current.GetInt16(i);
        }

        /// <inheritdoc/>
        public int GetInt32(int i)
        {
            if (_isClosed)
                throw new InvalidOperationException("Data reader is closed.");
            return Current.GetInt32(i);
        }

        /// <inheritdoc/>
        public long GetInt64(int i)
        {
            if (_isClosed)
                throw new InvalidOperationException("Data reader is closed.");
            return Current.GetInt64(i);
        }

        /// <inheritdoc/>
        public float GetFloat(int i)
        {
            if (_isClosed)
                throw new InvalidOperationException("Data reader is closed.");
            return Current.GetFloat(i);
        }

        /// <inheritdoc/>
        public double GetDouble(int i)
        {
            if (_isClosed)
                throw new InvalidOperationException("Data reader is closed.");
            return Current.GetDouble(i);
        }

        /// <inheritdoc/>
        public string GetString(int i)
        {
            if (_isClosed)
                throw new InvalidOperationException("Data reader is closed.");
            return Current.GetString(i);
        }

        /// <inheritdoc/>
        public decimal GetDecimal(int i)
        {
            if (_isClosed)
                throw new InvalidOperationException("Data reader is closed.");
            return Current.GetDecimal(i);
        }

        /// <inheritdoc/>
        public DateTime GetDateTime(int i)
        {
            if (_isClosed)
                throw new InvalidOperationException("Data reader is closed.");
            return Current.GetDateTime(i);
        }

        /// <inheritdoc/>
        public IDataReader GetData(int i)
        {
            if (_isClosed)
                throw new InvalidOperationException("Data reader is closed.");
            return Current.GetData(i);
        }

        /// <inheritdoc/>
        public bool IsDBNull(int i)
        {
            if (_isClosed)
                throw new InvalidOperationException("Data reader is closed.");
            return Current.IsDBNull(i);
        }

        /// <inheritdoc/>
        public int FieldCount
        {
            get
            {
                if (_isClosed)
                    throw new InvalidOperationException("Data reader is closed.");
                return CurrentSet.Definition.FieldCount;
            }
        }

        /// <inheritdoc/>
        object IDataRecord.this[int i]
        {
            get { return GetValue(i); }
        }

        /// <inheritdoc/>
        object IDataRecord.this[string name]
        {
            get { return GetValue(GetOrdinal(name)); }
        }

        /// <inheritdoc/>
        public void Close()
        {
            Dispose();
            _isClosed = true;
        }

        /// <inheritdoc/>
        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool NextResult()
        {
            if (_isClosed)
                throw new InvalidOperationException("Data reader is closed.");
            _recordEnumerator = null;
            
            // Create set enumerator if not present.
            if (_setEnumerator == null)
                _setEnumerator = _recordSets.GetEnumerator();

            return _setEnumerator.MoveNext();
        }

        /// <inheritdoc/>
        public bool Read()
        {
            if (_isClosed)
                throw new InvalidOperationException("Data reader is closed.");

            // If we haven't got a record enumerator create one.
            if (_recordEnumerator == null)
                _recordEnumerator = CurrentSet.GetEnumerator();

            return _recordEnumerator.Current != null && _recordEnumerator.MoveNext();
        }

        /// <inheritdoc/>
        public int Depth
        {
            get { return 0; }
        }

        /// <inheritdoc/>
        public bool IsClosed
        {
            get { return _isClosed; }
        }

        /// <inheritdoc/>
        public int RecordsAffected
        {
            get { return -1; }
        }

        /// <inheritdoc/>
        public IEnumerator<IObjectRecordSet> GetEnumerator()
        {
            return _recordSets.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        public void Add(IObjectRecordSet item)
        {
            if (_isClosed)
                throw new InvalidOperationException("Data reader is closed.");
            if (_setEnumerator != null)
                throw new InvalidOperationException("Cannot modify data reader once it has started being read.");

            _recordSets.Add(item);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            if (_isClosed)
                throw new InvalidOperationException("Data reader is closed.");
            if (_setEnumerator != null)
                throw new InvalidOperationException("Cannot modify data reader once it has started being read.");

            _recordSets.Clear();
        }

        /// <inheritdoc/>
        public bool Contains(IObjectRecordSet item)
        {
            return _recordSets.Contains(item);
        }

        /// <inheritdoc/>
        public void CopyTo(IObjectRecordSet[] array, int arrayIndex)
        {
            _recordSets.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public bool Remove(IObjectRecordSet item)
        {
            if (_isClosed)
                throw new InvalidOperationException("Data reader is closed.");
            if (_setEnumerator != null)
                throw new InvalidOperationException("Cannot modify data reader once it has started being read.");

            return _recordSets.Remove(item);
        }

        /// <inheritdoc/>
        public int Count
        {
            get { return _recordSets.Count; }
        }

        /// <inheritdoc/>
        public bool IsReadOnly
        {
            get { return false; }
        }
    }
}