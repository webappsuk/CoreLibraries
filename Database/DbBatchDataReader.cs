#region © Copyright Web Applications (UK) Ltd, 2017.  All rights reserved.
// Copyright (c) 2017, Web Applications UK Ltd
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
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Database.Exceptions;
using WebApplications.Utilities.Logging;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.Database
{
    /// <summary>
    /// Base class for reading a forward-only stream of rows from a data source for a single <see cref="SqlBatchCommand"/>.
    /// </summary>
    /// <seealso cref="System.Data.Common.DbDataReader" />
    [PublicAPI]
    public abstract class DbBatchDataReader : DbDataReader
    {
        [NotNull]
        private readonly SqlBatch _batch;

        [NotNull]
        private readonly DbDataReader _baseReader;

        /// <summary>
        /// Gets the underlying data reader.
        /// </summary>
        /// <value>
        /// The base reader.
        /// </value>
        [NotNull]
        protected DbDataReader BaseReader => _baseReader;

        /// <summary>
        /// Gets the underlying data reader, throwing an exception if it is no longer open.
        /// </summary>
        /// <value>
        /// The open base reader.
        /// </value>
        [NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected DbDataReader BaseReaderOpen([CallerMemberName] string name = null)
        {
            if (IsOpen) return _baseReader;
            if (IsFinished) throw FinishedError();
            // ReSharper disable once ExplicitCallerInfoArgument
            throw ClosedError(name);
        }

        /// <summary>
        /// Gets the error for when the reader has finished reading.
        /// </summary>
        [NotNull]
        private static InvalidOperationException FinishedError()
            => new InvalidOperationException("Invalid attempt to read when no data is present.");

        /// <summary>
        /// Gets the error for when the reader is closed.
        /// </summary>
        /// <param name="name">The member name.</param>
        [NotNull]
        private static InvalidOperationException ClosedError([CallerMemberName] string name = null)
            => new InvalidOperationException($"Invalid attempt to call {name} when reader is closed.");

        [Flags]
        private enum BatchReaderState : byte
        {
            Open = 0,
            Finished = 1,
            Closed = 2
        }

        [NotNull]
        private readonly object _stateLock = new object();
        private BatchReaderState _state = BatchReaderState.Open;

        /// <summary>
        /// Sets the state of the reader.
        /// </summary>
        /// <param name="state">The state.</param>
        private void SetState(BatchReaderState state)
        {
            Debug.Assert(state != BatchReaderState.Open);

            if ((state & _state) == state) return;

            lock (_stateLock)
            {
                if ((state & _state) == state) return;
                _state |= state;
            }

            Debug.Assert(_state != BatchReaderState.Open);

            _finishedCompletionSource?.TrySetCompleted();
        }

        /// <summary>
        /// Gets a value indicating whether the reader has finished reading data from the underlying reader.
        /// </summary>
        /// <value>
        ///   <see langword="true" /> if this instance is finished reading from the underlying reader; otherwise, <see langword="false" />.
        /// </value>
        /// <exception cref="System.ArgumentOutOfRangeException">value</exception>
        protected internal bool IsFinishedReading => (_state & BatchReaderState.Finished) == BatchReaderState.Finished;

        /// <summary>
        /// Indicates that this reader has finished reading from the underlying reader.
        /// </summary>
        internal void SetFinished() => SetState(BatchReaderState.Finished);

        /// <summary>
        /// Gets a value indicating whether this <see cref="DbBatchDataReader"/> is open.
        /// </summary>
        /// <value>
        ///   <see langword="true" /> if open; otherwise, <see langword="false" />.
        /// </value>
        public bool IsOpen => _state == BatchReaderState.Open && !_skipRows;

        /// <summary>
        /// Gets a value indicating whether the reader has finished reading data or the rest of the data should be skipped.
        /// </summary>
        /// <value>
        ///   <see langword="true" /> if this reader is finished; otherwise, <see langword="false" />.
        /// </value>
        protected bool IsFinished => IsFinishedReading || (_state == BatchReaderState.Open && _skipRows);

        /// <summary>Gets a value indicating whether the <see cref="T:System.Data.Common.DbDataReader" /> is closed.</summary>
        /// <returns>true if the <see cref="T:System.Data.Common.DbDataReader" /> is closed; otherwise false.</returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Data.SqlClient.SqlDataReader" /> is closed. </exception>
        public override bool IsClosed => (_state & BatchReaderState.Closed) == BatchReaderState.Closed;

        /// <summary>
        /// The command behavior.
        /// </summary>
        protected readonly CommandBehavior CommandBehavior;

        /// <summary>
        /// The reference to the flag indicating if there are any more result sets.
        /// </summary>
        [NotNull]
        private readonly ValueReference<bool> _hasResultSet;

        /// <summary>
        /// Whether to skip the remaining rows
        /// </summary>
        private bool _skipRows;

        /// <summary>
        /// Determines whether behavior given applies to this reader.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <returns>
        ///   <see langword="true" /> if the behavior applies; otherwise, <see langword="false" />.
        /// </returns>
        protected bool IsCommandBehavior(CommandBehavior condition)
            => condition == (condition & CommandBehavior);

        /// <summary>
        /// Initializes a new instance of the <see cref="DbBatchDataReader" /> class.
        /// </summary>
        /// <param name="batch">The batch being read.</param>
        /// <param name="baseReader">The base reader.</param>
        /// <param name="commandBehavior">The command behavior.</param>
        /// <param name="hasResultSet">The reference to the flag indicating if there are any more result sets.</param>
        /// <exception cref="System.ArgumentNullException">baseReader</exception>
        protected internal DbBatchDataReader(
            [NotNull]SqlBatch batch,
            [NotNull] DbDataReader baseReader,
            CommandBehavior commandBehavior,
            [NotNull] ValueReference<bool> hasResultSet)
        {
            _batch = batch ?? throw new ArgumentNullException(nameof(batch));
            _baseReader = baseReader ?? throw new ArgumentNullException(nameof(baseReader));
            CommandBehavior = commandBehavior;
            _hasResultSet = hasResultSet ?? throw new ArgumentNullException(nameof(hasResultSet));
        }

        /// <summary>Gets the number of columns in the current row.</summary>
        /// <returns>The number of columns in the current row.</returns>
        /// <exception cref="T:System.NotSupportedException">There is no current connection to an instance of SQL Server. </exception>
        public override int FieldCount
        {
            get
            {
                if (IsOpen) return BaseReader.FieldCount;
                if (IsFinished) return 0;
                throw ClosedError();
            }
        }

        /// <summary>Gets the number of fields in the <see cref="T:System.Data.Common.DbDataReader" /> that are not hidden.</summary>
        /// <returns>The number of fields that are not hidden.</returns>
        public override int VisibleFieldCount
        {
            get
            {
                if (IsOpen) return BaseReader.VisibleFieldCount;
                if (IsFinished) return 0;
                throw ClosedError();
            }
        }

        /// <summary>Gets a value that indicates whether this <see cref="T:System.Data.Common.DbDataReader" /> contains one or more rows.</summary>
        /// <returns>true if the <see cref="T:System.Data.Common.DbDataReader" /> contains one or more rows; otherwise false.</returns>
        public override bool HasRows
        {
            get
            {
                if (IsOpen) return BaseReader.HasRows;
                if (IsFinished) return false;
                throw ClosedError();
            }
        }

        /// <summary>Gets the number of rows changed, inserted, or deleted by execution of the SQL statement. </summary>
        /// <returns>The number of rows changed, inserted, or deleted. -1 for SELECT statements; 0 if no rows were affected or the statement failed.</returns>
        public override int RecordsAffected => IsOpen ? BaseReader.RecordsAffected : -1;

        /// <summary>Gets a value indicating the depth of nesting for the current row.</summary>
        /// <returns>The depth of nesting for the current row.</returns>
        public override int Depth
        {
            get
            {
                if (IsOpen) return BaseReader.Depth;
                if (IsFinished) return 0;
                throw ClosedError();
            }
        }

        /// <summary>Advances the reader to the next record in a result set.</summary>
        /// <returns>true if there are more rows; otherwise false.</returns>
        public override bool Read()
        {
            if (IsOpen)
            {
                if (_skipRows) return false;
                if (IsCommandBehavior(CommandBehavior.SingleRow))
                    _skipRows = true;
                return BaseReader.Read();
            }
            if (IsFinished) return false;
            throw ClosedError();
        }

        /// <summary>This is the asynchronous version of <see cref="M:System.Data.Common.DbDataReader.Read" />.  Providers should override with an appropriate implementation. The cancellationToken may optionally be ignored.The default implementation invokes the synchronous <see cref="M:System.Data.Common.DbDataReader.Read" /> method and returns a completed task, blocking the calling thread. The default implementation will return a cancelled task if passed an already cancelled cancellationToken.  Exceptions thrown by Read will be communicated via the returned Task Exception property.Do not invoke other methods and properties of the DbDataReader object until the returned Task is complete.</summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <exception cref="T:System.Data.Common.DbException">An error occurred while executing the command text.</exception>
        public override Task<bool> ReadAsync(CancellationToken cancellationToken)
        {
            if (IsOpen)
            {
                if (_skipRows) return TaskResult.False;
                if (IsCommandBehavior(CommandBehavior.SingleRow))
                    _skipRows = true;
                return BaseReader.ReadAsync(cancellationToken);
            }
            if (IsFinished) return TaskResult.False;
            throw ClosedError();
        }

        /// <summary>Advances the reader to the next result when reading the results of a batch of statements.</summary>
        /// <returns>true if there are more result sets; otherwise false.</returns>
        public override bool NextResult()
        {
            if (IsOpen)
            {
                if (_skipRows) return false;
                if (IsCommandBehavior(CommandBehavior.SingleResult))
                {
                    _skipRows = true;
                    return false;
                }

                // Read the rest of the record set
                while (BaseReader.Read())
                {
                }

                bool result = BaseReader.NextResult();
                _hasResultSet.Value = result;
                if (!result)
                    return false;
                if (IsOpen) return true;
            }
            if (IsFinished) return false;
            throw ClosedError();
        }

        /// <summary>This is the asynchronous version of <see cref="M:System.Data.Common.DbDataReader.NextResult" />. Providers should override with an appropriate implementation. The <paramref name="cancellationToken" /> may optionally be ignored.The default implementation invokes the synchronous <see cref="M:System.Data.Common.DbDataReader.NextResult" /> method and returns a completed task, blocking the calling thread. The default implementation will return a cancelled task if passed an already cancelled <paramref name="cancellationToken" />. Exceptions thrown by <see cref="M:System.Data.Common.DbDataReader.NextResult" /> will be communicated via the returned Task Exception property.Other methods and properties of the DbDataReader object should not be invoked while the returned Task is not yet completed.</summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <exception cref="T:System.Data.Common.DbException">An error occurred while executing the command text.</exception>
        public override Task<bool> NextResultAsync(CancellationToken cancellationToken)
        {
            if (IsOpen)
            {
                if (_skipRows) return TaskResult.False;
                if (IsCommandBehavior(CommandBehavior.SingleResult))
                {
                    _skipRows = true;
                    return TaskResult.False;
                }

                return Next();
            }
            if (IsFinished) return TaskResult.False;
            throw ClosedError();

            async Task<bool> Next()
            {
                // Read the rest of the record set
                while (await BaseReader.ReadAsync().ConfigureAwait(false))
                {
                }

                // ReSharper disable once PossibleNullReferenceException
                bool result = await BaseReader.NextResultAsync(cancellationToken).ConfigureAwait(false);
                _hasResultSet.Value = result;
                if (!result)
                    return false;

                if (IsOpen) return true;
                if (IsFinished) return false;
                throw ClosedError();
            }
        }

        /// <summary>
        /// Starts the reader.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        internal async Task StartAsync(CancellationToken cancellationToken)
        {
            // Expecting a single row with a single column with the string "Start"

            if (!await BaseReader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                throw new SqlBatchExecutionException(
                    _batch,
                    LoggingLevel.Critical,
                    () => Resources.DbBatchDataReader_StartAsync_MissingStart);
            }

            if (BaseReader.FieldCount != 1
                // ReSharper disable once PossibleNullReferenceException
                || await BaseReader.IsDBNullAsync(0, cancellationToken).ConfigureAwait(false)
                || !"Start".Equals(BaseReader.GetValue(0))
                || await BaseReader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                throw new SqlBatchExecutionException(
                    _batch,
                    LoggingLevel.Critical,
                    () => Resources.DbBatchDataReader_StartAsync_WrongFormat);
            }

            // ReSharper disable once PossibleNullReferenceException
            _hasResultSet.Value = await BaseReader.NextResultAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Reads the result sets while the reader is not <see cref="BatchReaderState.Finished"/>.
        /// </summary>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        [NotNull]
        internal Task ReadTillFinishedAsync(CancellationToken cancellationToken)
        {
            return IsFinishedReading ? TaskResult.Completed : DoAsync();

            async Task DoAsync()
            {
                // ReSharper disable once PossibleNullReferenceException
                while (!IsFinishedReading && _hasResultSet)
                {
                    _hasResultSet.Value = await BaseReader.NextResultAsync(cancellationToken).ConfigureAwait(false);
                }
            }
        }

        private TaskCompletionSource _finishedCompletionSource;
        internal TaskCompletionSource FinishedCompletionSource => _finishedCompletionSource;

        /// <summary>
        /// Gets an <see cref="IDisposable"/> which will close this reader and complete the <see cref="FinishedCompletionSource"/> when disposed.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [NotNull]
        internal IDisposable GetDisposer(CancellationToken cancellationToken)
        {
            // Create a new TCS if there isn't already one
            if (_finishedCompletionSource == null)
            {
                TaskCompletionSource newSource = new TaskCompletionSource(SqlBatchResult.CompletionSourceOptions);
                Interlocked.CompareExchange(ref _finishedCompletionSource, newSource, null);
            }


            TaskCompletionSource completionSource = _finishedCompletionSource;
            CancellationTokenRegistration reg = default(CancellationTokenRegistration);

            // If the reader is already over, we can just complete it
            if (!IsOpen) completionSource.TrySetCompleted();

            // If the token can be cancelled, then the source should be cancelled when the token is
            else if (cancellationToken.CanBeCanceled)
                reg = cancellationToken.Register(() => completionSource.TrySetCanceled());

            return new Disposer(this, reg);
        }

        /// <summary>
        /// Closes a reader on <see cref="Dispose"/>.
        /// </summary>
        private class Disposer : IDisposable
        {
            [NotNull]
            private readonly DbBatchDataReader _reader;
            private readonly CancellationTokenRegistration _cancellation;

            public Disposer([NotNull] DbBatchDataReader reader, CancellationTokenRegistration cancellation)
            {
                _reader = reader;
                _cancellation = cancellation;
            }


            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            public void Dispose()
            {
                // Closing the reader sets the State, which completes the TCS 
                _reader.Close();
                _cancellation.Dispose();
            }
        }

        /// <summary>Gets a value that indicates whether the column contains nonexistent or missing values.</summary>
        /// <returns>true if the specified column is equivalent to <see cref="T:System.DBNull" />; otherwise false.</returns>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        public override bool IsDBNull(int ordinal) => BaseReaderOpen().IsDBNull(ordinal);

        /// <summary>An asynchronous version of <see cref="M:System.Data.Common.DbDataReader.IsDBNull(System.Int32)" />, which gets a value that indicates whether the column contains non-existent or missing values. Optionally, sends a notification that operations should be cancelled.</summary>
        /// <returns>true if the specified column value is equivalent to DBNull otherwise false.</returns>
        /// <param name="ordinal">The zero-based column to be retrieved.</param>
        /// <param name="cancellationToken">The cancellation instruction, which propagates a notification that operations should be canceled. This does not guarantee the cancellation. A setting of CancellationToken.None makes this method equivalent to <see cref="M:System.Data.Common.DbDataReader.IsDBNullAsync(System.Int32)" />. The returned task must be marked as cancelled.</param>
        /// <exception cref="T:System.InvalidOperationException">The connection drops or is closed during the data retrieval.The <see cref="T:System.Data.Common.DbDataReader" /> is closed during the data retrieval.There is no data ready to be read (for example, the first <see cref="M:System.Data.Common.DbDataReader.Read" /> hasn't been called, or returned false).Trying to read a previously read column in sequential mode.There was an asynchronous operation in progress. This applies to all Get* methods when running in sequential mode, as they could be called while reading a stream.</exception>
        /// <exception cref="T:System.IndexOutOfRangeException">Trying to read a column that does not exist.</exception>
        public override Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken)
            => BaseReaderOpen().IsDBNullAsync(ordinal, cancellationToken);

        /// <summary>Closes the <see cref="T:System.Data.Common.DbDataReader" /> object.</summary>
        public override void Close() => SetState(BatchReaderState.Closed);

        /// <summary>Releases the managed resources used by the <see cref="T:System.Data.Common.DbDataReader" /> and optionally releases the unmanaged resources.</summary>
        /// <param name="disposing">true to release managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            Close();
        }

        /// <summary>Gets the value of the specified column as an instance of <see cref="T:System.Object" />.</summary>
        /// <returns>The value of the specified column.</returns>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. </exception>
        public override object this[int ordinal] => BaseReaderOpen()[ordinal];

        /// <summary>Gets the value of the specified column as an instance of <see cref="T:System.Object" />.</summary>
        /// <returns>The value of the specified column.</returns>
        /// <param name="name">The name of the column.</param>
        /// <exception cref="T:System.IndexOutOfRangeException">No column with the specified name was found. </exception>
        public override object this[string name] => BaseReaderOpen()[name];

        /// <summary>Gets the name of the column, given the zero-based column ordinal.</summary>
        /// <returns>The name of the specified column.</returns>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        public override string GetName(int ordinal) => BaseReaderOpen().GetName(ordinal);

        /// <summary>Populates an array of objects with the column values of the current row.</summary>
        /// <returns>The number of instances of <see cref="T:System.Object" /> in the array.</returns>
        /// <param name="values">An array of <see cref="T:System.Object" /> into which to copy the attribute columns.</param>
        public override int GetValues(object[] values) => BaseReaderOpen().GetValues(values);

        /// <summary>Gets the column ordinal given the name of the column.</summary>
        /// <returns>The zero-based column ordinal.</returns>
        /// <param name="name">The name of the column.</param>
        /// <exception cref="T:System.IndexOutOfRangeException">The name specified is not a valid column name.</exception>
        public override int GetOrdinal(string name) => BaseReaderOpen().GetOrdinal(name);

        /// <summary>Gets the value of the specified column as a Boolean.</summary>
        /// <returns>The value of the specified column.</returns>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <exception cref="T:System.InvalidCastException">The specified cast is not valid. </exception>
        public override bool GetBoolean(int ordinal) => BaseReaderOpen().GetBoolean(ordinal);

        /// <summary>Gets the value of the specified column as a byte.</summary>
        /// <returns>The value of the specified column.</returns>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <exception cref="T:System.InvalidCastException">The specified cast is not valid. </exception>
        public override byte GetByte(int ordinal) => BaseReaderOpen().GetByte(ordinal);

        /// <summary>Reads a stream of bytes from the specified column, starting at location indicated by <paramref name="dataOffset" />, into the buffer, starting at the location indicated by <paramref name="bufferOffset" />.</summary>
        /// <returns>The actual number of bytes read.</returns>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <param name="dataOffset">The index within the row from which to begin the read operation.</param>
        /// <param name="buffer">The buffer into which to copy the data.</param>
        /// <param name="bufferOffset">The index with the buffer to which the data will be copied.</param>
        /// <param name="length">The maximum number of characters to read.</param>
        /// <exception cref="T:System.InvalidCastException">The specified cast is not valid. </exception>
        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
            => BaseReaderOpen().GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);

        /// <summary>Gets the value of the specified column as a single character.</summary>
        /// <returns>The value of the specified column.</returns>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <exception cref="T:System.InvalidCastException">The specified cast is not valid. </exception>
        public override char GetChar(int ordinal) => BaseReaderOpen().GetChar(ordinal);

        /// <summary>Reads a stream of characters from the specified column, starting at location indicated by <paramref name="dataOffset" />, into the buffer, starting at the location indicated by <paramref name="bufferOffset" />.</summary>
        /// <returns>The actual number of characters read.</returns>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <param name="dataOffset">The index within the row from which to begin the read operation.</param>
        /// <param name="buffer">The buffer into which to copy the data.</param>
        /// <param name="bufferOffset">The index with the buffer to which the data will be copied.</param>
        /// <param name="length">The maximum number of characters to read.</param>
        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
            => BaseReaderOpen().GetChars(ordinal, dataOffset, buffer, bufferOffset, length);

        /// <summary>Gets the value of the specified column as a globally-unique identifier (GUID).</summary>
        /// <returns>The value of the specified column.</returns>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <exception cref="T:System.InvalidCastException">The specified cast is not valid. </exception>
        public override Guid GetGuid(int ordinal) => BaseReaderOpen().GetGuid(ordinal);

        /// <summary>Gets the value of the specified column as a 16-bit signed integer.</summary>
        /// <returns>The value of the specified column.</returns>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <exception cref="T:System.InvalidCastException">The specified cast is not valid. </exception>
        public override short GetInt16(int ordinal) => BaseReaderOpen().GetInt16(ordinal);

        /// <summary>Gets the value of the specified column as a 32-bit signed integer.</summary>
        /// <returns>The value of the specified column.</returns>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <exception cref="T:System.InvalidCastException">The specified cast is not valid. </exception>
        public override int GetInt32(int ordinal) => BaseReaderOpen().GetInt32(ordinal);

        /// <summary>Gets the value of the specified column as a 64-bit signed integer.</summary>
        /// <returns>The value of the specified column.</returns>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <exception cref="T:System.InvalidCastException">The specified cast is not valid. </exception>
        public override long GetInt64(int ordinal) => BaseReaderOpen().GetInt64(ordinal);

        /// <summary>Gets the value of the specified column as a <see cref="T:System.DateTime" /> object.</summary>
        /// <returns>The value of the specified column.</returns>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <exception cref="T:System.InvalidCastException">The specified cast is not valid. </exception>
        public override DateTime GetDateTime(int ordinal) => BaseReaderOpen().GetDateTime(ordinal);

        /// <summary>Gets the value of the specified column as an instance of <see cref="T:System.String" />.</summary>
        /// <returns>The value of the specified column.</returns>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <exception cref="T:System.InvalidCastException">The specified cast is not valid. </exception>
        public override string GetString(int ordinal) => BaseReaderOpen().GetString(ordinal);

        /// <summary>Gets the value of the specified column as a <see cref="T:System.Decimal" /> object.</summary>
        /// <returns>The value of the specified column.</returns>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <exception cref="T:System.InvalidCastException">The specified cast is not valid. </exception>
        public override decimal GetDecimal(int ordinal) => BaseReaderOpen().GetDecimal(ordinal);

        /// <summary>Gets the value of the specified column as a double-precision floating point number.</summary>
        /// <returns>The value of the specified column.</returns>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <exception cref="T:System.InvalidCastException">The specified cast is not valid. </exception>
        public override double GetDouble(int ordinal) => BaseReaderOpen().GetDouble(ordinal);

        /// <summary>Gets the value of the specified column as a single-precision floating point number.</summary>
        /// <returns>The value of the specified column.</returns>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <exception cref="T:System.InvalidCastException">The specified cast is not valid. </exception>
        public override float GetFloat(int ordinal) => BaseReaderOpen().GetFloat(ordinal);

        /// <summary>Gets name of the data type of the specified column.</summary>
        /// <returns>A string representing the name of the data type.</returns>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <exception cref="T:System.InvalidCastException">The specified cast is not valid. </exception>
        public override string GetDataTypeName(int ordinal) => BaseReaderOpen().GetDataTypeName(ordinal);

        /// <summary>Gets the data type of the specified column.</summary>
        /// <returns>The data type of the specified column.</returns>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <exception cref="T:System.InvalidCastException">The specified cast is not valid. </exception>
        public override Type GetFieldType(int ordinal) => BaseReaderOpen().GetFieldType(ordinal);

        /// <summary>Gets the value of the specified column as an instance of <see cref="T:System.Object" />.</summary>
        /// <returns>The value of the specified column.</returns>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        public override object GetValue(int ordinal) => BaseReaderOpen().GetValue(ordinal);

        /// <summary>Returns a <see cref="T:System.Data.Common.DbDataReader" /> object for the requested column ordinal that can be overridden with a provider-specific implementation.</summary>
        /// <returns>A <see cref="T:System.Data.Common.DbDataReader" /> object.</returns>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        protected override DbDataReader GetDbDataReader(int ordinal) => BaseReaderOpen().GetData(ordinal);

        /// <summary>Synchronously gets the value of the specified column as a type.</summary>
        /// <returns>The column to be retrieved.</returns>
        /// <param name="ordinal">The column to be retrieved.</param>
        /// <typeparam name="T">Synchronously gets the value of the specified column as a type.</typeparam>
        /// <exception cref="T:System.InvalidOperationException">The connection drops or is closed during the data retrieval.The <see cref="T:System.Data.SqlClient.SqlDataReader" /> is closed during the data retrieval.There is no data ready to be read (for example, the first <see cref="M:System.Data.SqlClient.SqlDataReader.Read" /> hasn't been called, or returned false).Tried to read a previously-read column in sequential mode.There was an asynchronous operation in progress. This applies to all Get* methods when running in sequential mode, as they could be called while reading a stream.</exception>
        /// <exception cref="T:System.IndexOutOfRangeException">Trying to read a column that does not exist.</exception>
        /// <exception cref="T:System.InvalidCastException">
        /// <typeparamref name="T" /> doesn’t match the type returned by SQL Server or cannot be cast.</exception>
        public override T GetFieldValue<T>(int ordinal) => BaseReaderOpen().GetFieldValue<T>(ordinal);

        /// <summary>Asynchronously gets the value of the specified column as a type.</summary>
        /// <returns>The type of the value to be returned.</returns>
        /// <param name="ordinal">The type of the value to be returned.</param>
        /// <param name="cancellationToken">The cancellation instruction, which propagates a notification that operations should be canceled. This does not guarantee the cancellation. A setting of CancellationToken.None makes this method equivalent to <see cref="M:System.Data.Common.DbDataReader.GetFieldValueAsync``1(System.Int32)" />. The returned task must be marked as cancelled.</param>
        /// <typeparam name="T">The type of the value to be returned. See the remarks section for more information.</typeparam>
        /// <exception cref="T:System.InvalidOperationException">The connection drops or is closed during the data retrieval.The <see cref="T:System.Data.Common.DbDataReader" /> is closed during the data retrieval.There is no data ready to be read (for example, the first <see cref="M:System.Data.Common.DbDataReader.Read" /> hasn't been called, or returned false).Tried to read a previously-read column in sequential mode.There was an asynchronous operation in progress. This applies to all Get* methods when running in sequential mode, as they could be called while reading a stream.</exception>
        /// <exception cref="T:System.IndexOutOfRangeException">Trying to read a column that does not exist.</exception>
        /// <exception cref="T:System.InvalidCastException">
        /// <typeparamref name="T" /> doesn’t match the type returned by the data source or cannot be cast.</exception>
        public override Task<T> GetFieldValueAsync<T>(int ordinal, CancellationToken cancellationToken)
            => BaseReaderOpen().GetFieldValueAsync<T>(ordinal, cancellationToken);

        /// <summary>Returns the provider-specific field type of the specified column.</summary>
        /// <returns>The <see cref="T:System.Type" /> object that describes the data type of the specified column.</returns>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        public override Type GetProviderSpecificFieldType(int ordinal) => BaseReaderOpen()
            .GetProviderSpecificFieldType(ordinal);

        /// <summary>Gets the value of the specified column as an instance of <see cref="T:System.Object" />.</summary>
        /// <returns>The value of the specified column.</returns>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        public override object GetProviderSpecificValue(int ordinal) => BaseReaderOpen()
            .GetProviderSpecificValue(ordinal);

        /// <summary>Gets all provider-specific attribute columns in the collection for the current row.</summary>
        /// <returns>The number of instances of <see cref="T:System.Object" /> in the array.</returns>
        /// <param name="values">An array of <see cref="T:System.Object" /> into which to copy the attribute columns.</param>
        public override int GetProviderSpecificValues(object[] values) => BaseReaderOpen()
            .GetProviderSpecificValues(values);

        /// <summary>Retrieves data as a <see cref="T:System.IO.Stream" />.</summary>
        /// <returns>The returned object.</returns>
        /// <param name="ordinal">Retrieves data as a <see cref="T:System.IO.Stream" />.</param>
        /// <exception cref="T:System.InvalidOperationException">The connection drops or is closed during the data retrieval.The <see cref="T:System.Data.Common.DbDataReader" /> is closed during the data retrieval.There is no data ready to be read (for example, the first <see cref="M:System.Data.Common.DbDataReader.Read" /> hasn't been called, or returned false).Tried to read a previously-read column in sequential mode.There was an asynchronous operation in progress. This applies to all Get* methods when running in sequential mode, as they could be called while reading a stream.</exception>
        /// <exception cref="T:System.IndexOutOfRangeException">Trying to read a column that does not exist.</exception>
        /// <exception cref="T:System.InvalidCastException">The returned type was not one of the types below:binaryimagevarbinaryudt</exception>
        public override Stream GetStream(int ordinal) => BaseReaderOpen().GetStream(ordinal);

        /// <summary>Retrieves data as a <see cref="T:System.IO.TextReader" />.</summary>
        /// <returns>The returned object.</returns>
        /// <param name="ordinal">Retrieves data as a <see cref="T:System.IO.TextReader" />.</param>
        /// <exception cref="T:System.InvalidOperationException">The connection drops or is closed during the data retrieval.The <see cref="T:System.Data.Common.DbDataReader" /> is closed during the data retrieval.There is no data ready to be read (for example, the first <see cref="M:System.Data.Common.DbDataReader.Read" /> hasn't been called, or returned false).Tried to read a previously-read column in sequential mode.There was an asynchronous operation in progress. This applies to all Get* methods when running in sequential mode, as they could be called while reading a stream.</exception>
        /// <exception cref="T:System.IndexOutOfRangeException">Trying to read a column that does not exist.</exception>
        /// <exception cref="T:System.InvalidCastException">The returned type was not one of the types below:charncharntextnvarchartextvarchar</exception>
        public override TextReader GetTextReader(int ordinal) => BaseReaderOpen().GetTextReader(ordinal);

        /// <summary>Returns a <see cref="T:System.Data.DataTable" /> that describes the column metadata of the <see cref="T:System.Data.Common.DbDataReader" />.</summary>
        /// <returns>A <see cref="T:System.Data.DataTable" /> that describes the column metadata.</returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Data.SqlClient.SqlDataReader" /> is closed. </exception>
        public override DataTable GetSchemaTable()
        {
            if (IsOpen) return BaseReader.GetSchemaTable();
            if (IsFinished) return null;
            throw ClosedError();
        }

        /// <summary>Returns an <see cref="T:System.Collections.IEnumerator" /> that can be used to iterate through the rows in the data reader.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> that can be used to iterate through the rows in the data reader.</returns>
        public override IEnumerator GetEnumerator()
            => IsOpen ? BaseReader.GetEnumerator() : new DbEnumerator(this);

        /// <summary>
        /// Gets an <see cref="XmlReader"/> for reading the current record set as XML.
        /// </summary>
        /// <returns>An <see cref="XmlReader"/> for reading XML from the current record set.</returns>
        [NotNull]
        protected internal abstract XmlReader GetXmlReader();
    }

    /// <summary>
    /// Base class for reading a forward-only stream of rows from a SQL Server database for a single <see cref="SqlBatchCommand"/>.
    /// </summary>
    /// <seealso cref="WebApplications.Utilities.Database.DbBatchDataReader" />
    [PublicAPI]
    public sealed class SqlBatchDataReader : DbBatchDataReader
    {
        private static readonly XmlReaderSettings _xmlReaderSettings =
            new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment, CloseInput = true };

        /// <summary>
        /// Gets the underlying data reader.
        /// </summary>
        /// <value>
        /// The base reader.
        /// </value>
        [NotNull]
        private new SqlDataReader BaseReader => (SqlDataReader)base.BaseReader;

        /// <summary>
        /// Gets the underlying data reader, throwing an exception if it is no longer open.
        /// </summary>
        /// <value>
        /// The open base reader.
        /// </value>
        [NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private new SqlDataReader BaseReaderOpen([CallerMemberName] string name = null)
            // ReSharper disable once ExplicitCallerInfoArgument
            => (SqlDataReader)base.BaseReaderOpen(name);

        /// <summary>
        /// Initializes a new instance of the <see cref="DbBatchDataReader" /> class.
        /// </summary>
        /// <param name="batch">The batch being read.</param>
        /// <param name="baseReader">The base reader.</param>
        /// <param name="commandBehavior">The command behavior.</param>
        /// <param name="hasResultSet">The reference to the flag indicating if there are any more result sets.</param>
        internal SqlBatchDataReader(
            [NotNull] SqlBatch batch,
            [NotNull] SqlDataReader baseReader,
            CommandBehavior commandBehavior,
            [NotNull] ValueReference<bool> hasResultSet)
            : base(batch, baseReader, commandBehavior, hasResultSet)
        {
        }

        /// <summary>Fills an array of <see cref="T:System.Object" /> that contains the values for all the columns in the record, expressed as SQL Server types.</summary>
        /// <returns>An integer indicating the number of columns copied.</returns>
        /// <param name="values">An array of <see cref="T:System.Object" /> into which to copy the values. The column values are expressed as SQL Server types.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="values" /> is null. </exception>
        public int GetSqlValues([NotNull] Object[] values) => BaseReaderOpen().GetSqlValues(values);

        /// <summary>Gets the value of the specified column as <see cref="T:System.Data.SqlTypes.SqlBytes" />.</summary>
        /// <returns>The value of the specified column.</returns>
        /// <param name="i">The zero-based column ordinal. </param>
        [NotNull]
        public SqlBytes GetSqlBytes(int i) => BaseReaderOpen().GetSqlBytes(i);

        /// <summary>Gets the value of the specified column as <see cref="T:System.Data.SqlTypes.SqlChars" />.</summary>
        /// <returns>The value of the specified column.</returns>
        /// <param name="i">The zero-based column ordinal. </param>
        [NotNull]
        public SqlChars GetSqlChars(int i) => BaseReaderOpen().GetSqlChars(i);

        /// <summary>Gets the value of the specified column as a <see cref="T:System.Data.SqlTypes.SqlBinary" />.</summary>
        /// <returns>The value of the column expressed as a <see cref="T:System.Data.SqlTypes.SqlBinary" />.</returns>
        /// <param name="i">The zero-based column ordinal. </param>
        public SqlBinary GetSqlBinary(int i) => BaseReaderOpen().GetSqlBinary(i);

        /// <summary>Gets the value of the specified column as a <see cref="T:System.Data.SqlTypes.SqlBoolean" />.</summary>
        /// <returns>The value of the column.</returns>
        /// <param name="i">The zero-based column ordinal. </param>
        public SqlBoolean GetSqlBoolean(int i) => BaseReaderOpen().GetSqlBoolean(i);

        /// <summary>Gets the value of the specified column as a <see cref="T:System.Data.SqlTypes.SqlByte" />.</summary>
        /// <returns>The value of the column expressed as a <see cref="T:System.Data.SqlTypes.SqlByte" />.</returns>
        /// <param name="i">The zero-based column ordinal. </param>
        public SqlByte GetSqlByte(int i) => BaseReaderOpen().GetSqlByte(i);

        /// <summary>Gets the value of the specified column as a <see cref="T:System.Data.SqlTypes.SqlDateTime" />.</summary>
        /// <returns>The value of the column expressed as a <see cref="T:System.Data.SqlTypes.SqlDateTime" />.</returns>
        /// <param name="i">The zero-based column ordinal. </param>
        public SqlDateTime GetSqlDateTime(int i) => BaseReaderOpen().GetSqlDateTime(i);

        /// <summary>Gets the value of the specified column as a <see cref="T:System.Data.SqlTypes.SqlDecimal" />.</summary>
        /// <returns>The value of the column expressed as a <see cref="T:System.Data.SqlTypes.SqlDecimal" />.</returns>
        /// <param name="i">The zero-based column ordinal. </param>
        public SqlDecimal GetSqlDecimal(int i) => BaseReaderOpen().GetSqlDecimal(i);

        /// <summary>Gets the value of the specified column as a <see cref="T:System.Data.SqlTypes.SqlDouble" />.</summary>
        /// <returns>The value of the column expressed as a <see cref="T:System.Data.SqlTypes.SqlDouble" />.</returns>
        /// <param name="i">The zero-based column ordinal. </param>
        public SqlDouble GetSqlDouble(int i) => BaseReaderOpen().GetSqlDouble(i);

        /// <summary>Gets the value of the specified column as a <see cref="T:System.Data.SqlTypes.SqlGuid" />.</summary>
        /// <returns>The value of the column expressed as a <see cref="T:System.Data.SqlTypes.SqlGuid" />.</returns>
        /// <param name="i">The zero-based column ordinal. </param>
        public SqlGuid GetSqlGuid(int i) => BaseReaderOpen().GetSqlGuid(i);

        /// <summary>Gets the value of the specified column as a <see cref="T:System.Data.SqlTypes.SqlInt16" />.</summary>
        /// <returns>The value of the column expressed as a <see cref="T:System.Data.SqlTypes.SqlInt16" />.</returns>
        /// <param name="i">The zero-based column ordinal. </param>
        public SqlInt16 GetSqlInt16(int i) => BaseReaderOpen().GetSqlInt16(i);

        /// <summary>Gets the value of the specified column as a <see cref="T:System.Data.SqlTypes.SqlInt32" />.</summary>
        /// <returns>The value of the column expressed as a <see cref="T:System.Data.SqlTypes.SqlInt32" />.</returns>
        /// <param name="i">The zero-based column ordinal. </param>
        public SqlInt32 GetSqlInt32(int i) => BaseReaderOpen().GetSqlInt32(i);

        /// <summary>Gets the value of the specified column as a <see cref="T:System.Data.SqlTypes.SqlInt64" />.</summary>
        /// <returns>The value of the column expressed as a <see cref="T:System.Data.SqlTypes.SqlInt64" />.</returns>
        /// <param name="i">The zero-based column ordinal. </param>
        public SqlInt64 GetSqlInt64(int i) => BaseReaderOpen().GetSqlInt64(i);

        /// <summary>Gets the value of the specified column as a <see cref="T:System.Data.SqlTypes.SqlMoney" />.</summary>
        /// <returns>The value of the column expressed as a <see cref="T:System.Data.SqlTypes.SqlMoney" />.</returns>
        /// <param name="i">The zero-based column ordinal. </param>
        public SqlMoney GetSqlMoney(int i) => BaseReaderOpen().GetSqlMoney(i);

        /// <summary>Gets the value of the specified column as a <see cref="T:System.Data.SqlTypes.SqlSingle" />.</summary>
        /// <returns>The value of the column expressed as a <see cref="T:System.Data.SqlTypes.SqlSingle" />.</returns>
        /// <param name="i">The zero-based column ordinal. </param>
        public SqlSingle GetSqlSingle(int i) => BaseReaderOpen().GetSqlSingle(i);

        /// <summary>Gets the value of the specified column as a <see cref="T:System.Data.SqlTypes.SqlString" />.</summary>
        /// <returns>The value of the column expressed as a <see cref="T:System.Data.SqlTypes.SqlString" />.</returns>
        /// <param name="i">The zero-based column ordinal. </param>
        public SqlString GetSqlString(int i) => BaseReaderOpen().GetSqlString(i);

        /// <summary>Gets the value of the specified column as an XML value.</summary>
        /// <returns>A <see cref="T:System.Data.SqlTypes.SqlXml" /> value that contains the XML stored within the corresponding field. </returns>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The index passed was outside the range of 0 to <see cref="P:System.Data.DataTableReader.FieldCount" /> - 1</exception>
        /// <exception cref="T:System.InvalidOperationException">An attempt was made to read or access columns in a closed <see cref="T:System.Data.SqlClient.SqlDataReader" />.</exception>
        /// <exception cref="T:System.InvalidCastException">The retrieved data is not compatible with the <see cref="T:System.Data.SqlTypes.SqlXml" /> type.</exception>
        [NotNull]
        // ReSharper disable once AssignNullToNotNullAttribute
        public SqlXml GetSqlXml(int i) => BaseReaderOpen().GetSqlXml(i);

        /// <summary>Retrieves data of type XML as an <see cref="T:System.Xml.XmlReader" />.</summary>
        /// <returns>A <see cref="T:System.Xml.XmlReader" /> for reading the XML stored within the corresponding field. </returns>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The index passed was outside the range of 0 to <see cref="P:System.Data.DataTableReader.FieldCount" /> - 1</exception>
        /// <exception cref="T:System.InvalidOperationException">An attempt was made to read or access columns in a closed <see cref="T:System.Data.SqlClient.SqlDataReader" />.</exception>
        /// <exception cref="T:System.InvalidCastException">The retrieved data is not compatible with the <see cref="T:System.Data.SqlTypes.SqlXml" /> type.</exception>
        [NotNull]
        // ReSharper disable once AssignNullToNotNullAttribute
        public XmlReader GetXmlReader(int i) => BaseReaderOpen().GetXmlReader(i);

        /// <summary>Retrieves the value of the specified column as a <see cref="T:System.DateTimeOffset" /> object.</summary>
        /// <returns>The value of the specified column.</returns>
        /// <param name="i">The zero-based column ordinal. </param>
        /// <exception cref="T:System.InvalidCastException">The specified cast is not valid. </exception>
        public DateTimeOffset GetDateTimeOffset(int i) => BaseReaderOpen().GetDateTimeOffset(i);

        /// <summary>Retrieves the value of the specified column as a <see cref="T:System.TimeSpan" /> object.</summary>
        /// <returns>The value of the specified column.</returns>
        /// <param name="i">The zero-based column ordinal. </param>
        /// <exception cref="T:System.InvalidCastException">The specified cast is not valid. </exception>
        public TimeSpan GetTimeSpan(int i) => BaseReaderOpen().GetTimeSpan(i);

        /// <summary>Returns the data value in the specified column as a SQL Server type. </summary>
        /// <returns>The value of the column expressed as a <see cref="T:System.Data.SqlDbType" />.</returns>
        /// <param name="i">The zero-based column ordinal. </param>
        public object GetSqlValue(int i) => BaseReaderOpen().GetSqlValue(i);

        /// <summary>
        /// Gets an <see cref="XmlReader"/> for reading the current record set as XML.
        /// </summary>
        /// <returns>An <see cref="XmlReader"/> for reading XML from the current record set.</returns>
        protected internal override XmlReader GetXmlReader()
        {
            if (BaseReader.FieldCount != 1)
                throw new InvalidOperationException("The command must return a single column.");

            switch (BaseReader.GetDataTypeName(0))
            {
                case "ntext":
                case "nvarchar":
                    return XmlReader.Create(new SqlBatchDataTextReader(this), _xmlReaderSettings);
                case "xml":
                    // ReSharper disable once AssignNullToNotNullAttribute
                    return BaseReader.Read()
                        ? BaseReader.GetXmlReader(0)
                        : XmlReader.Create(new StringReader(string.Empty), _xmlReaderSettings);
            }

            throw new InvalidOperationException("The command must return an Xml result.");
        }
    }
}