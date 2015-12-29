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
        /// Holds header information.
        /// </summary>
        private class Header
        {
            /// <summary>
            /// The number of records affected.
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

        /// <summary>
        /// Holds a table definition.
        /// </summary>
        private class TableDefinition : IReadOnlyCollection<Column>
        {
            /// <summary>
            /// The <see cref="TableDefinition"/> indicating the end of stream.
            /// </summary>
            [NotNull]
            public static readonly TableDefinition End = new TableDefinition();

            /// <summary>
            /// Whether the table contains rows.
            /// </summary>
            private readonly bool _hasRows;

            /// <summary>
            /// The columns by name (case-insensitive).
            /// </summary>
            private readonly Dictionary<string, Column> _columnsByName;

            /// <summary>
            /// The columns in ordinal order.
            /// </summary>
            private readonly List<Column> _columns;

            /// <summary>
            /// The nullable columns.
            /// </summary>
            private readonly List<Column> _nullableColumns;

            /// <summary>
            /// Creates a default, invalid instance of the <see cref="TableDefinition"/>.
            /// </summary>
            private TableDefinition()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="TableDefinition"/> class.
            /// </summary>
            /// <param name="count">The count.</param>
            /// <param name="hasRows">if set to <see langword="true" /> the table has rows.</param>
            public TableDefinition(int count, bool hasRows)
            {
                _columnsByName = new Dictionary<string, Column>(count, StringComparer.InvariantCultureIgnoreCase);
                _columns = new List<Column>(count);
                _nullableColumns = new List<Column>(count);
                _hasRows = hasRows;
            }

            /// <summary>
            /// Whether the table contains rows.
            /// </summary>
            public bool HasRows
            {
                get
                {
                    if (_columns == null)
                        throw new InvalidOperationException("Invalid attempt to read when no data is present.");
                    return _hasRows;
                }
            }

            /// <summary>
            /// Adds the specified column.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="sqlDbType">Type of the SQL database.</param>
            /// <param name="allowDBNull">if set to <see langword="true" /> then the column is nullable.</param>
            /// <returns>The newly created <see cref="Column"/>.</returns>
            public Column Add([NotNull] string name, SqlDbType sqlDbType, bool allowDBNull)
            {
                if (_columns == null)
                    throw new InvalidOperationException("Invalid attempt to add a column.");
                Column column = new Column(_columns.Count, name, sqlDbType, allowDBNull);
                _columnsByName.Add(name, column);
                _columns.Add(column);
                if (allowDBNull)
                    _nullableColumns.Add(column);
                return column;
            }
            /// <summary>
            /// Gets the nullable columns, in ordinal order.
            /// </summary>
            /// <value>The nullable columns.</value>
            /// <exception cref="InvalidOperationException">Invalid attempt to read when no data is present.</exception>
            [NotNull]
            public IReadOnlyCollection<Column> NullableColumns
            {
                get
                {
                    if (_nullableColumns == null)
                        throw new InvalidOperationException("Invalid attempt to read when no data is present.");
                    return _nullableColumns;
                }
            }

            /// <summary>
            /// Gets the <see cref="Column"/> with the specified ordinal.
            /// </summary>
            /// <param name="ordinal">The ordinal.</param>
            /// <returns>Column.</returns>
            /// <exception cref="InvalidOperationException">Invalid attempt to read when no data is present.</exception>
            public Column this[int ordinal]
            {
                get
                {
                    if (_columns == null)
                        throw new InvalidOperationException("Invalid attempt to read when no data is present.");
                    return _columns[ordinal];
                }
            }

            /// <summary>
            /// Gets the <see cref="Column"/> with the specified name.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <returns>Column.</returns>
            /// <exception cref="InvalidOperationException">Invalid attempt to read when no data is present.</exception>
            public Column this[string name]
            {
                get
                {
                    if (_columnsByName == null)
                        throw new InvalidOperationException("Invalid attempt to read when no data is present.");
                    return _columnsByName[name];
                }
            }

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
            /// <exception cref="InvalidOperationException">Invalid attempt to read when no data is present.</exception>
            public IEnumerator<Column> GetEnumerator()
            {
                if (_columns == null)
                    throw new InvalidOperationException("Invalid attempt to read when no data is present.");
                return _columns.GetEnumerator();
            }

            /// <summary>
            /// Returns an enumerator that iterates through a collection.
            /// </summary>
            /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            /// <summary>
            /// Gets the number of elements in the collection.
            /// </summary>
            /// <value>The count.</value>
            /// <exception cref="System.InvalidOperationException">Invalid attempt to read when no data is present.</exception>
            public int Count
            {
                get
                {
                    if (_columns == null)
                        throw new InvalidOperationException("Invalid attempt to read when no data is present.");
                    return _columns.Count;
                }
            }
        }

        /// <summary>
        /// Holds a column definition.
        /// </summary>
        private class Column
        {
            /// <summary>
            /// The ordinal.
            /// </summary>
            public readonly int Ordinal;

            /// <summary>
            /// The name.
            /// </summary>
            public readonly string Name;

            /// <summary>
            /// The SQL database type.
            /// </summary>
            public readonly SqlDbType SqlDbType;

            /// <summary>
            /// Whether the column can be null.
            /// </summary>
            public readonly bool AllowDBNull;

            /// <summary>
            /// Initializes a new instance of the <see cref="Column"/> class.
            /// </summary>
            /// <param name="ordinal">The ordinal.</param>
            /// <param name="name">The name.</param>
            /// <param name="sqlDbType">Type of the SQL database.</param>
            /// <param name="allowDBNull">if set to <see langword="true" /> column is nullable.</param>
            public Column(int ordinal, string name, SqlDbType sqlDbType, bool allowDBNull)
            {
                Ordinal = ordinal;
                Name = name;
                SqlDbType = sqlDbType;
                AllowDBNull = allowDBNull;
            }

            /// <summary>
            /// Returns a <see cref="string" /> that represents this instance.
            /// </summary>
            /// <returns>A <see cref="string" /> that represents this instance.</returns>
            public override string ToString() => $"{Name} [{SqlDbType}]";
        }

        /// <summary>
        /// Holds the current row.
        /// </summary>
        private class Row
        {
            /// <summary>
            /// The not read <see cref="Row"/>.
            /// </summary>
            [NotNull]
            public static readonly Row NotRead = new Row();

            /// <summary>
            /// The end <see cref="Row"/>.
            /// </summary>
            [NotNull]
            public static readonly Row End = new Row();

            /// <summary>
            /// The associated table definition.
            /// </summary>
            private readonly TableDefinition _tableDefinition;

            /// <summary>
            /// The SQL values
            /// </summary>
            private readonly object[] _sqlValues;

            /// <summary>
            /// The CLR values lazy initializer.
            /// </summary>
            private readonly Lazy<object[]> _values;

            /// <summary>
            /// Creates a default, invalid, instance of the <see cref="Row"/> class from being created.
            /// </summary>
            private Row()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Row"/> class.
            /// </summary>
            /// <param name="tableDefinition">The table definition.</param>
            /// <param name="sqlValues">The SQL values.</param>
            public Row([NotNull] TableDefinition tableDefinition, [NotNull] object[] sqlValues)
            {
                _tableDefinition = tableDefinition;
                _sqlValues = sqlValues;

                // Create lazy initializer for values.
                _values = new Lazy<object[]>(
                    () =>
                    {
                        object[] values = new object[tableDefinition.Count];
                        int v = 0;
                        foreach (CachedDataReader.Column column in tableDefinition)
                        {
                            if (column != null)
                                values[v] = SqlValueSerialization.GetCLRValueFromSqlVariant(
                                    column.SqlDbType,
                                    sqlValues[v]);
                            v++;
                        }
                        return values;
                    },
                    LazyThreadSafetyMode.ExecutionAndPublication);
            }

            /// <summary>
            /// The SQL values
            /// </summary>
            /// <exception cref="System.InvalidOperationException">Invalid attempt to read when no data is present.</exception>
            public object[] SqlValues
            {
                get
                {
                    if (_sqlValues == null)
                        throw new InvalidOperationException("Invalid attempt to read when no data is present.");
                    return _sqlValues;
                }
            }

            /// <summary>
            /// Gets the values.
            /// </summary>
            /// <value>The values.</value>
            /// <exception cref="System.InvalidOperationException">Invalid attempt to read when no data is present.</exception>
            public object[] Values
            {
                get
                {
                    if (_values == null)
                        throw new InvalidOperationException("Invalid attempt to read when no data is present.");
                    return _values.Value;
                }
            }

            /// <summary>
            /// The associated table definition.
            /// </summary>
            /// <exception cref="System.InvalidOperationException">Invalid attempt to read when no data is present.</exception>
            public TableDefinition TableDefinition
            {
                get
                {
                    if (_tableDefinition == null)
                        throw new InvalidOperationException("Invalid attempt to read when no data is present.");
                    return _tableDefinition; }
            }
        }

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
        private TaskCompletionSource<Row> _row = new TaskCompletionSource<Row>();

        /// <summary>
        /// The current table definition.
        /// </summary>
        [NotNull]
        private TaskCompletionSource<TableDefinition> _tableDefinition = new TaskCompletionSource<TableDefinition>();

        /// <summary>
        /// The header information is set once the first stream is read.
        /// </summary>
        [NotNull]
        private TaskCompletionSource<Header> _header = new TaskCompletionSource<Header>();

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
            
            // Trigger retrieval of first result, but don't wait.
            NextResultAsync(CancellationToken.None).ConfigureAwait(false);
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
        /// Gets the name.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override string GetName(int ordinal) => _tableDefinition.Task.Result[ordinal].Name;

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>System.Int32.</returns>
        public override int GetValues(object[] values)
        {
            if (values.Length < 1) return 0;

            int c = 0;
            foreach (object value in _row.Task.Result.Values)
            {
                values[c] = value;
                if (c++ >= values.Length) break;
            }
            return c;
        }

        /// <summary>
        /// Gets all provider-specific attribute columns in the collection for the current row.
        /// </summary>
        /// <param name="values">An array of <see cref="T:System.Object" /> into which to copy the attribute columns.</param>
        /// <returns>The number of instances of <see cref="T:System.Object" /> in the array.</returns>
        public override int GetProviderSpecificValues(object[] values)
        {
            if (values.Length < 1) return 0;

            int c = 0;
            foreach (object value in _row.Task.Result.SqlValues)
            {
                values[c] = value;
                if (c++ >= values.Length) break;
            }
            return c;
        }

        /// <summary>
        /// Determines whether [is database null] [the specified ordinal].
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns><see langword="true" /> if [is database null] [the specified ordinal]; otherwise, <see langword="false" />.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override bool IsDBNull(int ordinal) => _row.Task.Result.SqlValues[ordinal].IsNull();

        /// <summary>
        /// Gets the field count.
        /// </summary>
        /// <value>The field count.</value>
        public override int FieldCount => _tableDefinition.Task.Result.Count;

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
                throw new InvalidOperationException(Resources.CachedDataReader_Closed);

            try
            {
                using (await _asyncLock.LockAsync(cancellationToken).ConfigureAwait(false))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (_streamState != 0)
                        throw new InvalidOperationException(Resources.CachedDataReader_Closed);

                    // If this isn't the first call we should have a completed task.
                    if (_tableDefinition.Task.IsCompleted)
                    {
                        // Grab current table definition (will also throw cancellations/failures).
                        TableDefinition lastDefinition = _tableDefinition.Task.Result;

                        // Check whether we're already at th end
                        if (lastDefinition == TableDefinition.End)
                            return false;

                        // Skip over any remaining rows
                        if (_row.Task.Result != Row.End)
                            while (await ReadRowAsync(lastDefinition, cancellationToken)
                                .ConfigureAwait(false) != null)
                            {
                            }

                        // Create new task completion task for table definition and row.
                        _tableDefinition = new TaskCompletionSource<TableDefinition>();
                        _row = new TaskCompletionSource<Row>();
                    }
                    else if (!_header.Task.IsCompleted)
                    {
                        // Read Header
                        int recordsAffected = await
                            VariableLengthEncoding.DecodeIntAsync(_stream, cancellationToken).ConfigureAwait(false);
                        _header.SetResult(new Header(recordsAffected));
                    }
                    
                    // Read field count and hasRows flag
                    uint fieldCount =
                        await VariableLengthEncoding.DecodeUIntAsync(_stream, cancellationToken).ConfigureAwait(false);

                    // Detect end of results
                    if (fieldCount < 1)
                    {
                        _tableDefinition.SetResult(TableDefinition.End);
                        _row.SetResult(Row.End);
                        return false;
                    }

                    bool hasRows = (fieldCount & 1) == 1;
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
                    _tableDefinition.SetResult(tableDefinition);

                    // Set the row as not yet read.
                    _row.SetResult(Row.NotRead);
                }

                return true;
            }
            catch (OperationCanceledException)
            {
                if (!_header.TrySetCanceled())
                {
                    TaskCompletionSource<Header> newHeader = new TaskCompletionSource<Header>();
                    newHeader.SetCanceled();
                    _header = newHeader;
                }
                if (!_tableDefinition.TrySetCanceled())
                {
                    TaskCompletionSource<TableDefinition> newTableDefinition = new TaskCompletionSource<TableDefinition>();
                    newTableDefinition.SetCanceled();
                    _tableDefinition = newTableDefinition;
                }
                if (!_row.TrySetCanceled())
                {
                    TaskCompletionSource<Row> newRow = new TaskCompletionSource<Row>();
                    newRow.SetCanceled();
                    _row = newRow;
                }
                throw;
            }
            catch (Exception exception)
            {
                if (!_header.TrySetException(exception))
                {
                    TaskCompletionSource<Header> newHeader = new TaskCompletionSource<Header>();
                    newHeader.SetException(exception);
                    _header = newHeader;
                }
                if (!_tableDefinition.TrySetException(exception))
                {
                    TaskCompletionSource<TableDefinition> newTableDefinition = new TaskCompletionSource<TableDefinition>();
                    newTableDefinition.SetException(exception);
                    _tableDefinition = newTableDefinition;
                }
                if (!_row.TrySetException(exception))
                {
                    TaskCompletionSource<Row> newRow = new TaskCompletionSource<Row>();
                    newRow.SetException(exception);
                    _row = newRow;
                }
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
                throw new InvalidOperationException(Resources.CachedDataReader_Closed);

            try
            {
                using (await _asyncLock.LockAsync(cancellationToken).ConfigureAwait(false))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (_streamState != 0)
                        throw new InvalidOperationException(Resources.CachedDataReader_Closed);

                    // Ensure the row task is in a completed state.
                    if (!_row.Task.IsCompleted || !_tableDefinition.Task.IsCompleted)
                        throw new InvalidOperationException("The reader is not initialized properly.");

                    Row current = _row.Task.Result;
                    if (current == Row.End) return false;

                    // Create new row task
                    _row = new TaskCompletionSource<Row>();

                    // Get the table definition.
                    TableDefinition tableDefinition = _tableDefinition.Task.Result;

                    // Sanity check we should have a table definition that matches the last row (unless we haven't
                    // read a row yet).
                    if (tableDefinition == null ||
                        (current != Row.NotRead && current?.TableDefinition != tableDefinition))
                        throw new InvalidOperationException("The reader is not in a valid state.");

                    // Get the SQL values for the row.
                    object[] sqlValues = await ReadRowAsync(tableDefinition, cancellationToken).ConfigureAwait(false);

                    // Update current row.
                    if (sqlValues == null)
                    {
                        // Reached end of record set
                        _row.SetResult(Row.End);
                        return false;
                    }

                    _row.SetResult(new Row(tableDefinition, sqlValues));
                    return true;
                }
            }
            catch (OperationCanceledException)
            {
                if (!_header.TrySetCanceled())
                {
                    TaskCompletionSource<Header> newHeader = new TaskCompletionSource<Header>();
                    newHeader.SetCanceled();
                    _header = newHeader;
                }
                if (!_tableDefinition.TrySetCanceled())
                {
                    TaskCompletionSource<TableDefinition> newTableDefinition = new TaskCompletionSource<TableDefinition>();
                    newTableDefinition.SetCanceled();
                    _tableDefinition = newTableDefinition;
                }
                if (!_row.TrySetCanceled())
                {
                    TaskCompletionSource<Row> newRow = new TaskCompletionSource<Row>();
                    newRow.SetCanceled();
                    _row = newRow;
                }
                throw;
            }
            catch (Exception exception)
            {
                if (!_header.TrySetException(exception))
                {
                    TaskCompletionSource<Header> newHeader = new TaskCompletionSource<Header>();
                    newHeader.SetException(exception);
                    _header = newHeader;
                }
                if (!_tableDefinition.TrySetException(exception))
                {
                    TaskCompletionSource<TableDefinition> newTableDefinition = new TaskCompletionSource<TableDefinition>();
                    newTableDefinition.SetException(exception);
                    _tableDefinition = newTableDefinition;
                }
                if (!_row.TrySetException(exception))
                {
                    TaskCompletionSource<Row> newRow = new TaskCompletionSource<Row>();
                    newRow.SetException(exception);
                    _row = newRow;
                }
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
                return null;

            // Read flags
            int nullableColumnCount = tableDefinition.NullableColumns.Count;
            bool[] flags = await SqlValueSerialization.ReadFlagsAsync(
                _stream,
                nullableColumnCount + 1,
                cancellationToken)
                .ConfigureAwait(false);

            // ReSharper disable once PossibleNullReferenceException
            if (flags[0])
                return null;

            // Read nulls
            int f = 1;
            HashSet<int> nullColumns = new HashSet<int>();
            foreach (Column nullableColumn in tableDefinition.NullableColumns)
                if (flags[f++])
                    // ReSharper disable once PossibleNullReferenceException
                    nullColumns.Add(nullableColumn.Ordinal);

            // Read values
            object[] values = new object[tableDefinition.Count];
            foreach (Column column in tableDefinition)
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
                    throw new InvalidOperationException(Resources.CachedDataReader_Closed); // TODO Support Depth?
                return 1;
            }
        }

        /// <summary>
        /// Gets the ordinal.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.Int32.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetOrdinal(string name) => _tableDefinition.Task.Result[name].Ordinal;

        /// <summary>
        /// Gets the boolean.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns><see langword="true" /> if XXXX, <see langword="false" /> otherwise.</returns>
        /// <exception cref="InvalidCastException">If the value is not of the specified type.</exception>
        public override bool GetBoolean(int ordinal) => (bool)GetValue(ordinal);

        /// <summary>
        /// Gets the byte.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>System.Byte.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="InvalidCastException">If the value is not of the specified type.</exception>
        public override byte GetByte(int ordinal) => (byte)GetValue(ordinal);

        /// <summary>
        /// Gets the bytes.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <param name="dataOffset">The data offset.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="bufferOffset">The buffer offset.</param>
        /// <param name="length">The length.</param>
        /// <returns>The actual number of bytes read.</returns>
        /// <remarks><para>GetBytes returns the number of available bytes in the field. Most of the time this is the
        /// exact length of the field. However, the number returned may be less than the true length of the field if
        /// GetBytes has already been used to obtain bytes from the field. This may be the case, for example, if the
        /// reader is reading a large data structure into a buffer. For more information, see the SequentialAccess
        /// setting for CommandBehavior.</para>
        /// <para>If you pass a buffer that is null, GetBytes returns the length of the entire field in bytes, not the
        /// remaining size based on the buffer offset parameter.</para>
        /// <para>No conversions are performed; therefore, the data retrieved must already be a byte array.</para>
        /// </remarks>
        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            byte[] data = (byte[])GetValue(ordinal);
            if (data == null) return 0;

            long dataLength = data.LongLength;
            if (buffer == null)
                return dataLength;

            long remainder = dataLength - dataOffset;

            // If the number of bytes is more than are left, reduce length
            if (length > remainder) length = (int)remainder;

            // Copy bytes
            if (length > 0)
                Array.Copy(data, dataOffset, buffer, bufferOffset, length);

            return length;
        }

        /// <summary>
        /// Gets the character.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>System.Char.</returns>
        /// <exception cref="NotSupportedException">Sql Server does not support single character values.</exception>
        public override char GetChar(int ordinal)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the chars.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <param name="dataOffset">The data offset.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="bufferOffset">The buffer offset.</param>
        /// <param name="length">The length.</param>
        /// <returns>The actual number of chars read.</returns>
        /// <remarks><para>GetChars returns the number of available chars in the field. Most of the time this is the
        /// exact length of the field. However, the number returned may be less than the true length of the field if
        /// GetChars has already been used to obtain chars from the field. This may be the case, for example, if the
        /// reader is reading a large data structure into a buffer. For more information, see the SequentialAccess
        /// setting for CommandBehavior.</para>
        /// <para>If you pass a buffer that is null, GetBytes returns the length of the entire field in chars, not the
        /// remaining size based on the buffer offset parameter.</para>
        /// <para>No conversions are performed; therefore, the data retrieved must already be a byte array.</para>
        /// </remarks>
        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            string data = (string)GetValue(ordinal);
            if (data == null) return 0;

            if (dataOffset > int.MaxValue)
                throw new InvalidOperationException("GetChars does not support long data offsets.");
            int offset = (int)dataOffset;

            int dataLength = data.Length;
            if (buffer == null)
                return dataLength;

            int remainder = dataLength - offset;

            // If the number of chars is more than are left, reduce length
            if (length > remainder) length = remainder;

            // Copy bytes
            if (length > 0)
            {
                int d = 0;
                for (int c = offset; c < offset + length; c++)
                    buffer[d++] = data[c];
            }

            return length;
        }

        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>Guid.</returns>
        /// <exception cref="InvalidCastException">If the value is not of the specified type.</exception>
        public override Guid GetGuid(int ordinal) => (Guid)GetValue(ordinal);

        /// <summary>
        /// Gets the int16.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>System.Int16.</returns>
        /// <exception cref="InvalidCastException">If the value is not of the specified type.</exception>
        public override short GetInt16(int ordinal) => (short)GetValue(ordinal);

        /// <summary>
        /// Gets the int32.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>System.Int32.</returns>
        /// <exception cref="InvalidCastException">If the value is not of the specified type.</exception>
        public override int GetInt32(int ordinal) => (int)GetValue(ordinal);

        /// <summary>
        /// Gets the int64.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>System.Int64.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override long GetInt64(int ordinal) => (long)GetValue(ordinal);

        /// <summary>
        /// Gets the date time.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>DateTime.</returns>
        /// <exception cref="InvalidCastException">If the value is not of the specified type.</exception>
        public override DateTime GetDateTime(int ordinal) => (DateTime)GetValue(ordinal);

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="InvalidCastException">If the value is not of the specified type.</exception>
        public override string GetString(int ordinal) => (string)GetValue(ordinal);

        /// <summary>
        /// Gets the decimal.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>System.Decimal.</returns>
        /// <exception cref="InvalidCastException">If the value is not of the specified type.</exception>
        public override decimal GetDecimal(int ordinal) => (decimal)GetValue(ordinal);

        /// <summary>
        /// Gets the double.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>System.Double.</returns>
        /// <exception cref="InvalidCastException">If the value is not of the specified type.</exception>
        public override double GetDouble(int ordinal) => (double)GetValue(ordinal);

        /// <summary>
        /// Gets the float.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>System.Single.</returns>
        /// <exception cref="InvalidCastException">If the value is not of the specified type.</exception>
        public override float GetFloat(int ordinal) => (float)GetValue(ordinal);

        /// <summary>
        /// Gets the name of the data type.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="InvalidCastException">If the value is not of the specified type.</exception>
        public override string GetDataTypeName(int ordinal)
        {
            // TODO
            switch (_tableDefinition.Task.Result[ordinal].SqlDbType)
            {
                case SqlDbType.BigInt:
                    return null;
                case SqlDbType.Binary:
                    return null;
                case SqlDbType.Bit:
                    return null;
                case SqlDbType.Char:
                    return null;
                case SqlDbType.DateTime:
                    return null;
                case SqlDbType.Decimal:
                    return null;
                case SqlDbType.Float:
                    return null;
                case SqlDbType.Image:
                    return null;
                case SqlDbType.Int:
                    return null;
                case SqlDbType.Money:
                    return null;
                case SqlDbType.NChar:
                    return null;
                case SqlDbType.NText:
                    return null;
                case SqlDbType.NVarChar:
                    return null;
                case SqlDbType.Real:
                    return null;
                case SqlDbType.UniqueIdentifier:
                    return null;
                case SqlDbType.SmallDateTime:
                    return null;
                case SqlDbType.SmallInt:
                    return null;
                case SqlDbType.SmallMoney:
                    return null;
                case SqlDbType.Text:
                    return null;
                case SqlDbType.Timestamp:
                    return null;
                case SqlDbType.TinyInt:
                    return null;
                case SqlDbType.VarBinary:
                    return null;
                case SqlDbType.VarChar:
                    return null;
                case SqlDbType.Variant:
                    return null;
                case SqlDbType.Xml:
                    return null;
                case SqlDbType.Udt:
                    return null;
                case SqlDbType.Structured:
                    return null;
                case SqlDbType.Date:
                    return null;
                case SqlDbType.Time:
                    return null;
                case SqlDbType.DateTime2:
                    return null;
                case SqlDbType.DateTimeOffset:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Gets the type of the field.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>Type.</returns>
        public override Type GetFieldType(int ordinal)
        {
            // TODO
            switch (_tableDefinition.Task.Result[ordinal].SqlDbType)
            {
                case SqlDbType.BigInt:
                    return null;
                case SqlDbType.Binary:
                    return null;
                case SqlDbType.Bit:
                    return null;
                case SqlDbType.Char:
                    return null;
                case SqlDbType.DateTime:
                    return null;
                case SqlDbType.Decimal:
                    return null;
                case SqlDbType.Float:
                    return null;
                case SqlDbType.Image:
                    return null;
                case SqlDbType.Int:
                    return null;
                case SqlDbType.Money:
                    return null;
                case SqlDbType.NChar:
                    return null;
                case SqlDbType.NText:
                    return null;
                case SqlDbType.NVarChar:
                    return null;
                case SqlDbType.Real:
                    return null;
                case SqlDbType.UniqueIdentifier:
                    return null;
                case SqlDbType.SmallDateTime:
                    return null;
                case SqlDbType.SmallInt:
                    return null;
                case SqlDbType.SmallMoney:
                    return null;
                case SqlDbType.Text:
                    return null;
                case SqlDbType.Timestamp:
                    return null;
                case SqlDbType.TinyInt:
                    return null;
                case SqlDbType.VarBinary:
                    return null;
                case SqlDbType.VarChar:
                    return null;
                case SqlDbType.Variant:
                    return null;
                case SqlDbType.Xml:
                    return null;
                case SqlDbType.Udt:
                    return null;
                case SqlDbType.Structured:
                    return null;
                case SqlDbType.Date:
                    return null;
                case SqlDbType.Time:
                    return null;
                case SqlDbType.DateTime2:
                    return null;
                case SqlDbType.DateTimeOffset:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Returns the provider-specific field type of the specified column.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The <see cref="T:System.Type" /> object that describes the data type of the specified column.</returns>
        public override Type GetProviderSpecificFieldType(int ordinal)
        {
            // TODO
            switch (_tableDefinition.Task.Result[ordinal].SqlDbType)
            {
                case SqlDbType.BigInt:
                    return null;
                case SqlDbType.Binary:
                    return null;
                case SqlDbType.Bit:
                    return null;
                case SqlDbType.Char:
                    return null;
                case SqlDbType.DateTime:
                    return null;
                case SqlDbType.Decimal:
                    return null;
                case SqlDbType.Float:
                    return null;
                case SqlDbType.Image:
                    return null;
                case SqlDbType.Int:
                    return null;
                case SqlDbType.Money:
                    return null;
                case SqlDbType.NChar:
                    return null;
                case SqlDbType.NText:
                    return null;
                case SqlDbType.NVarChar:
                    return null;
                case SqlDbType.Real:
                    return null;
                case SqlDbType.UniqueIdentifier:
                    return null;
                case SqlDbType.SmallDateTime:
                    return null;
                case SqlDbType.SmallInt:
                    return null;
                case SqlDbType.SmallMoney:
                    return null;
                case SqlDbType.Text:
                    return null;
                case SqlDbType.Timestamp:
                    return null;
                case SqlDbType.TinyInt:
                    return null;
                case SqlDbType.VarBinary:
                    return null;
                case SqlDbType.VarChar:
                    return null;
                case SqlDbType.Variant:
                    return null;
                case SqlDbType.Xml:
                    return null;
                case SqlDbType.Udt:
                    return null;
                case SqlDbType.Structured:
                    return null;
                case SqlDbType.Date:
                    return null;
                case SqlDbType.Time:
                    return null;
                case SqlDbType.DateTime2:
                    return null;
                case SqlDbType.DateTimeOffset:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>System.Object.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override object GetValue(int ordinal) => _row.Task.Result.Values[ordinal];

        /// <summary>
        /// Gets the value of the specified column as an instance of <see cref="T:System.Object" />.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override object GetProviderSpecificValue(int ordinal) => _row.Task.Result.SqlValues[ordinal];

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>IEnumerator.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override IEnumerator GetEnumerator()
        {
            // TODO Create enumerable
            throw new NotImplementedException();
        }
    }
}