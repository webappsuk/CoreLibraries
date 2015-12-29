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
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.Database.Caching
{
    /// <summary>
    /// Class CachedDataReader. This class cannot be inherited.
    /// </summary>
    public sealed class CachedDataReader : DbDataReader
    {
        /// <summary>
        /// The asynchronous read lock.
        /// </summary>
        [NotNull]
        private readonly AsyncLock _asyncLock = new AsyncLock();

        /// <summary>
        /// Holds header information
        /// </summary>
        private class Header
        {
            /// <summary>
            /// The records affected.
            /// </summary>
            public readonly int RecordsAffected;
            /// <summary>
            /// Initializes a new instance of the <see cref="Header"/> class.
            /// </summary>
            /// <param name="recordsAffected">The records affected.</param>
            public Header(int recordsAffected)
            {
                RecordsAffected = recordsAffected;
            }
        }

        private class TableDefinition
        {
            public readonly bool HasRows;
            [NotNull]
            public readonly Dictionary<string, Column> ColumnsByName;

            [NotNull]
            public readonly List<Column> Columns;

            [NotNull]
            public readonly List<Column> NullableColumns;

            public TableDefinition(int count, bool hasRows)
            {
                ColumnsByName = new Dictionary<string, Column>(count, StringComparer.InvariantCultureIgnoreCase);
                Columns = new List<Column>(count);
                NullableColumns = new List<Column>(count);
                HasRows = hasRows;
            }

            public class Column
            {
                public readonly int Ordinal;
                public readonly string Name;
                public readonly SqlDbType SqlDbType;
                public readonly bool AllowDBNull;

                public Column(int ordinal, string name, SqlDbType sqlDbType, bool allowDBNull)
                {
                    Ordinal = ordinal;
                    Name = name;
                    SqlDbType = sqlDbType;
                    AllowDBNull = allowDBNull;
                }
            }

            public Column Add(string name, SqlDbType sqlDbType, bool allowDBNull)
            {
                Column column = new Column(Columns.Count, name, sqlDbType, allowDBNull);
                ColumnsByName.Add(name, column);
                Columns.Add(column);
                if (allowDBNull)
                    NullableColumns.Add(column);
                return column;
            }
        }

        /// <summary>
        /// Whether we've finished reading from the record set.
        /// </summary>
        private bool _eor = true;

        /// <summary>
        /// Whether we've finished reading from the stream.
        /// </summary>
        private bool _eof;

        /// <summary>
        /// Whether the <see cref="_stream">underlying data stream</see> is closed (1) or disposed (2).
        /// </summary>
        private int _streamState;

        /// <summary>
        /// The underlying data stream.
        /// </summary>
        [NotNull]
        private readonly Stream _stream;

        /// <summary>
        /// The current row (if any).
        /// </summary>
        [NotNull]
        private TaskCompletionSource<object[]> _row = new TaskCompletionSource<object[]>();

        /// <summary>
        /// The current table definition.
        /// </summary>
        [NotNull]
        private TaskCompletionSource<TableDefinition> _tableDefinition = new TaskCompletionSource<TableDefinition>();

        /// <summary>
        /// The header information is set once the first stream is read.
        /// </summary>
        [NotNull]
        private readonly TaskCompletionSource<Header> _header = new TaskCompletionSource<Header>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedDataReader" /> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public CachedDataReader([NotNull] byte[] data): this(new MemoryStream(data, false))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedDataReader"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public CachedDataReader([NotNull] Stream stream)
        {
            _stream = stream;
            _row.TrySetException(new InvalidOperationException("Invalid attempt to read when no data is present."));
            
            // Get the first result
            NextResultAsync();
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public override void Close()
        {
            if (Interlocked.Exchange(ref _streamState, 1) == 0)
                _stream.Close();
            base.Close();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            Interlocked.Exchange(ref _tableDefinition, null);
            
            if (Interlocked.CompareExchange(ref _streamState, 1, 0) == 0)
                _stream.Close();
            if (Interlocked.CompareExchange(ref _streamState, 2, 1) == 1)
                _stream.Dispose();
        }

        /// <summary>
        /// Gets the stream.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>Stream.</returns>
        public override Stream GetStream(int ordinal)
        {
            // TODO
            return base.GetStream(ordinal);
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override string GetName(int ordinal) => _tableDefinition.Task.Result.Columns[ordinal].Name;

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>System.Int32.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether [is database null] [the specified ordinal].
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns><see langword="true" /> if [is database null] [the specified ordinal]; otherwise, <see langword="false" />.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override bool IsDBNull(int ordinal) => _row.Task.Result[ordinal].IsNull();

        /// <summary>
        /// Gets the field count.
        /// </summary>
        /// <value>The field count.</value>
        public override int FieldCount => _tableDefinition.Task.Result.Columns.Count;

        /// <summary>
        /// Gets the <see cref="System.Object"/> with the specified ordinal.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>System.Object.</returns>
        public override object this[int ordinal] => GetValue(ordinal);

        /// <summary>
        /// Gets the <see cref="System.Object"/> with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.Object.</returns>
        public override object this[string name] => GetValue(GetOrdinal(name));

        /// <summary>
        /// Gets a value indicating whether this instance has rows.
        /// </summary>
        /// <value><see langword="true" /> if this instance has rows; otherwise, <see langword="false" />.</value>
        public override bool HasRows => _tableDefinition.Task.Result.HasRows;

        /// <summary>
        /// Gets a value indicating whether this instance is closed.
        /// </summary>
        /// <value><see langword="true" /> if this instance is closed; otherwise, <see langword="false" />.</value>
        public override bool IsClosed => _streamState > 0;

        /// <summary>
        /// Gets the records affected.
        /// </summary>
        /// <value>The records affected.</value>
        public override int RecordsAffected => _header.Task.Result.RecordsAffected;
        /// <summary>
        /// Nexts the result.
        /// </summary>
        /// <returns><see langword="true" /> if XXXX, <see langword="false" /> otherwise.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        // ReSharper disable once PossibleNullReferenceException
        // TODO Should we provide a timeout here?
        public override bool NextResult() => NextResultAsync(CancellationToken.None).Result;

        /// <summary>
        /// Gets the next result set asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public override async Task<bool> NextResultAsync(CancellationToken cancellationToken)
        {
            if (_streamState != 0)
                throw new InvalidOperationException("The reader is closed or disposed");

            try
            {
                using (await _asyncLock.LockAsync(cancellationToken).ConfigureAwait(false))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (_streamState != 0)
                        throw new InvalidOperationException("The reader is closed or disposed");

                    if (_eof) return false;
                    
                    // Check if we've read the header yet.
                    if (_header.Task?.IsCompleted != true)
                    {
                        // Read Header
                        int recordsAffected =
                            await
                                VariableLengthEncoding.DecodeIntAsync(_stream, cancellationToken).ConfigureAwait(false);
                        _header.TrySetResult(new Header(recordsAffected));
                    }

                    // Read any remaining rows
                    if (!_eor)
                        while (
                            await ReadRowAsync(_tableDefinition.Task.Result, cancellationToken)
                                .ConfigureAwait(false) != null)
                        {
                        }

                    // Create new task completion task for table definition.
                    if (_tableDefinition.Task?.IsCompleted == true)
                        _tableDefinition = new TaskCompletionSource<TableDefinition>();

                    // Read field count and hasRows flag
                    uint fieldCount =
                        await VariableLengthEncoding.DecodeUIntAsync(_stream, cancellationToken).ConfigureAwait(false);

                    // Detect end of results
                    if (fieldCount < 1)
                    {
                        _eor = true;
                        _eof = true;
                        _tableDefinition.TrySetException(new InvalidOperationException("Invalid attempt to read when no data is present."));
                        return false;
                    }

                    bool hasRows = (fieldCount & 1) == 1;
                    _eor = !hasRows;

                    fieldCount >>= 1;

                    // Create a new data table
                    TableDefinition tableDefinition = new TableDefinition((int)fieldCount, hasRows);
                    for (int ordinal = 0; ordinal < fieldCount; ordinal++)
                    {
                        // Read flags
                        bool[] flags = await SqlValueSerialization.ReadFlagsAsync(_stream, 2, cancellationToken)
                            .ConfigureAwait(false);
                        // ReSharper disable once PossibleNullReferenceException
                        bool allowDbNull = flags[0];
                        bool cnIsNull = flags[1];

                        // Read SqlDbType
                        SqlDbType sqlDbType =
                            (SqlDbType)await VariableLengthEncoding.DecodeIntAsync(_stream, cancellationToken)
                                .ConfigureAwait(false);

                        // Read column name
                        string columnName = cnIsNull
                            ? null
                            : await SqlValueSerialization.ReadStringAsync(_stream, cancellationToken)
                                .ConfigureAwait(false);

                        tableDefinition.Add(columnName, sqlDbType, allowDbNull);
                        cancellationToken.ThrowIfCancellationRequested();
                    }

                    // Set the table definition.
                    _tableDefinition.TrySetResult(tableDefinition);
                }

                return true;
            }
            catch (OperationCanceledException)
            {
                _header.TrySetCanceled();
                _tableDefinition.TrySetCanceled();
                _row.TrySetCanceled();
                throw;
            }
            catch (Exception exception)
            {
                _header.TrySetException(exception);
                _tableDefinition.TrySetException(exception);
                _row.TrySetException(exception);
                throw;
            }
        }

        /// <summary>
        /// Reads this instance.
        /// </summary>
        /// <returns><see langword="true" /> if XXXX, <see langword="false" /> otherwise.</returns>
        // ReSharper disable once PossibleNullReferenceException
        // TODO Should we provide a timeout here?
        public override bool Read() => ReadAsync(CancellationToken.None).Result;

        /// <summary>
        /// Reads the asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public override async Task<bool> ReadAsync(CancellationToken cancellationToken)
        {
            if (_streamState != 0)
                throw new InvalidOperationException("The reader is closed or disposed");

            try
            {
                using (await _asyncLock.LockAsync(cancellationToken).ConfigureAwait(false))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (_streamState != 0)
                        throw new InvalidOperationException("The reader is closed or disposed");

                    if (_eor) return false;

                    // Create new row task
                    _row = new TaskCompletionSource<object[]>();

                    // Get the table definition.
                    TableDefinition tableDefinition = await _tableDefinition.Task.ConfigureAwait(false);
                    object[] values = await ReadRowAsync(tableDefinition, cancellationToken).ConfigureAwait(false);

                    // Update current row.
                    if (values == null)
                    {
                        // Reached end of record set
                        _row.TrySetException(new InvalidOperationException("Invalid attempt to read when no data is present."));
                        return false;
                    }

                    _row.TrySetResult(values);
                    return true;
                }
            }
            catch (OperationCanceledException)
            {
                _row.TrySetCanceled();
                throw;
            }
            catch (Exception exception)
            {
                _row.TrySetException(exception);
                throw;
            }
        }

        /// <summary>
        /// Reads the next row asynchronously.
        /// </summary>
        /// <param name="tableDefinition">The table definition.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        private async Task<object[]> ReadRowAsync([NotNull] TableDefinition tableDefinition, CancellationToken cancellationToken)
        {
            if (!tableDefinition.HasRows)
            {
                _eor = true;
                return null;
            }

            // Read flags
            int nullableColumnCount = tableDefinition.NullableColumns.Count;
            bool[] flags = await SqlValueSerialization.ReadFlagsAsync(
                _stream,
                nullableColumnCount + 1,
                cancellationToken)
                .ConfigureAwait(false);

            // ReSharper disable once PossibleNullReferenceException
            if (flags[0])
            {
                _eor = true;
                return null;
            }

            // Read nulls
            int f = 1;
            HashSet<int> nullColumns = new HashSet<int>();
            foreach (TableDefinition.Column nullableColumn in tableDefinition.NullableColumns)
                if (flags[f++])
                    // ReSharper disable once PossibleNullReferenceException
                    nullColumns.Add(nullableColumn.Ordinal);

            // Read values
            object[] values = new object[tableDefinition.Columns.Count];
            foreach (TableDefinition.Column column in tableDefinition.Columns)
            {
                // ReSharper disable once PossibleNullReferenceException
                int ordinal = column.Ordinal;
                if (nullColumns.Contains(ordinal))
                {
                    // TODO Get Proper null!
                    values[ordinal] = DBNull.Value;
                    continue;
                }

                // Deserialize value.
                values[ordinal] =
                    await SqlValueSerialization.DeSerializeAsync(column.SqlDbType, _stream, cancellationToken)
                        .ConfigureAwait(false);

                cancellationToken.ThrowIfCancellationRequested();
            }
            return values;
        }

        /// <summary>
        /// Gets the depth.
        /// </summary>
        /// <value>The depth.</value>
        public override int Depth
        {
            get
            {

                if (_streamState != 0)
                    throw new InvalidOperationException("The reader is closed or disposed"); // TODO Support Depth?
                return 1;
            }
        }

        /// <summary>
        /// Gets the ordinal.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.Int32.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetOrdinal(string name) => _tableDefinition.Task.Result.ColumnsByName[name].Ordinal;

        /// <summary>
        /// Gets the boolean.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns><see langword="true" /> if XXXX, <see langword="false" /> otherwise.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override bool GetBoolean(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the byte.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>System.Byte.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override byte GetByte(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the bytes.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <param name="dataOffset">The data offset.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="bufferOffset">The buffer offset.</param>
        /// <param name="length">The length.</param>
        /// <returns>System.Int64.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the character.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>System.Char.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override char GetChar(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the chars.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <param name="dataOffset">The data offset.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="bufferOffset">The buffer offset.</param>
        /// <param name="length">The length.</param>
        /// <returns>System.Int64.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>Guid.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override Guid GetGuid(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the int16.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>System.Int16.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override short GetInt16(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the int32.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>System.Int32.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override int GetInt32(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the int64.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>System.Int64.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override long GetInt64(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the date time.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>DateTime.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override DateTime GetDateTime(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override string GetString(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the decimal.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>System.Decimal.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override decimal GetDecimal(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the double.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>System.Double.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override double GetDouble(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the float.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>System.Single.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override float GetFloat(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the name of the data type.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override string GetDataTypeName(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the type of the field.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>Type.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override Type GetFieldType(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override object GetValue(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>IEnumerator.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}