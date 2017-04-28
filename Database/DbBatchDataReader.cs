using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Database
{
    /// <summary>
    /// Base class for reading a forward-only stream of rows from a data source for a single <see cref="SqlBatchCommand"/>.
    /// </summary>
    /// <seealso cref="System.Data.Common.DbDataReader" />
    public class DbBatchDataReader : DbDataReader
    {
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
        /// <exception cref="System.InvalidOperationException">Invalid attempt to read when no data is present.</exception>
        [NotNull]
        protected DbDataReader BaseReaderOpen([CallerMemberName] string name = null)
        {

            switch (State)
            {
                case BatchReaderState.Open:
                    return _baseReader;
                case BatchReaderState.Finished:
                    throw FinishedError();
                case BatchReaderState.Closed:
                default:
                    throw ClosedError(name);
            }
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

        private int _state = (int)BatchReaderState.Open;

        /// <summary>
        /// Gets or sets the state of the reader.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        internal BatchReaderState State
        {
            get => (BatchReaderState)_state;
            set
            {
                int iv = (int)value;
                int state;
                do
                {
                    state = _state;
                    if (iv <= state) return;


                } while (Interlocked.CompareExchange(ref _state, iv, state) != state);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="DbBatchDataReader"/> is open.
        /// </summary>
        /// <value>
        ///   <see langword="true" /> if open; otherwise, <see langword="false" />.
        /// </value>
        public bool IsOpen => State == BatchReaderState.Open;

        /// <summary>
        /// Gets a value indicating whether the reader has finished reading data.
        /// </summary>
        /// <value>
        ///   <see langword="true" /> if this reader is finished; otherwise, <see langword="false" />.
        /// </value>
        protected bool IsFinished => State == BatchReaderState.Finished;

        /// <summary>Gets a value indicating whether the <see cref="T:System.Data.Common.DbDataReader" /> is closed.</summary>
        /// <returns>true if the <see cref="T:System.Data.Common.DbDataReader" /> is closed; otherwise false.</returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Data.SqlClient.SqlDataReader" /> is closed. </exception>
        public override bool IsClosed => State == BatchReaderState.Closed;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbBatchDataReader"/> class.
        /// </summary>
        /// <param name="baseReader">The base reader.</param>
        protected internal DbBatchDataReader([NotNull] DbDataReader baseReader)
        {
            _baseReader = baseReader ?? throw new ArgumentNullException(nameof(baseReader));
        }

        /// <summary>Gets the name of the column, given the zero-based column ordinal.</summary>
        /// <returns>The name of the specified column.</returns>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        public override string GetName(int ordinal) => BaseReaderOpen().GetName(ordinal);

        /// <summary>Populates an array of objects with the column values of the current row.</summary>
        /// <returns>The number of instances of <see cref="T:System.Object" /> in the array.</returns>
        /// <param name="values">An array of <see cref="T:System.Object" /> into which to copy the attribute columns.</param>
        public override int GetValues(object[] values) => BaseReaderOpen().GetValues(values);

        /// <summary>Gets a value that indicates whether the column contains nonexistent or missing values.</summary>
        /// <returns>true if the specified column is equivalent to <see cref="T:System.DBNull" />; otherwise false.</returns>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        public override bool IsDBNull(int ordinal) => BaseReaderOpen().IsDBNull(ordinal);

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

        /// <summary>Advances the reader to the next result when reading the results of a batch of statements.</summary>
        /// <returns>true if there are more result sets; otherwise false.</returns>
        public override bool NextResult()
        {
            if (IsOpen) return BaseReader.NextResult();
            if (IsFinished) return false;
            throw ClosedError();
        }

        /// <summary>Advances the reader to the next record in a result set.</summary>
        /// <returns>true if there are more rows; otherwise false.</returns>
        public override bool Read()
        {
            if (IsOpen) return BaseReader.Read();
            if (IsFinished) return false;
            throw ClosedError();
        }

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

        /// <summary>Returns an <see cref="T:System.Collections.IEnumerator" /> that can be used to iterate through the rows in the data reader.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> that can be used to iterate through the rows in the data reader.</returns>
        public override IEnumerator GetEnumerator()
            => IsOpen ? BaseReader.GetEnumerator() : new DbEnumerator(this);

        /// <summary>Closes the <see cref="T:System.Data.Common.DbDataReader" /> object.</summary>
        public override void Close()
        {
            State = BatchReaderState.Closed;
            // TODO event handler?
        }

        /// <summary>Releases the managed resources used by the <see cref="T:System.Data.Common.DbDataReader" /> and optionally releases the unmanaged resources.</summary>
        /// <param name="disposing">true to release managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            Close();
        }

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
        /// <paramref name="T" /> doesn’t match the type returned by SQL Server or cannot be cast.</exception>
        public override T GetFieldValue<T>(int ordinal) => BaseReaderOpen().GetFieldValue<T>(ordinal);

        /// <summary>Asynchronously gets the value of the specified column as a type.</summary>
        /// <returns>The type of the value to be returned.</returns>
        /// <param name="ordinal">The type of the value to be returned.</param>
        /// <param name="cancellationToken">The cancellation instruction, which propagates a notification that operations should be canceled. This does not guarantee the cancellation. A setting of CancellationToken.None makes this method equivalent to <see cref="M:System.Data.Common.DbDataReader.GetFieldValueAsync``1(System.Int32)" />. The returned task must be marked as cancelled.</param>
        /// <typeparam name="T">The type of the value to be returned. See the remarks section for more information.</typeparam>
        /// <exception cref="T:System.InvalidOperationException">The connection drops or is closed during the data retrieval.The <see cref="T:System.Data.Common.DbDataReader" /> is closed during the data retrieval.There is no data ready to be read (for example, the first <see cref="M:System.Data.Common.DbDataReader.Read" /> hasn't been called, or returned false).Tried to read a previously-read column in sequential mode.There was an asynchronous operation in progress. This applies to all Get* methods when running in sequential mode, as they could be called while reading a stream.</exception>
        /// <exception cref="T:System.IndexOutOfRangeException">Trying to read a column that does not exist.</exception>
        /// <exception cref="T:System.InvalidCastException">
        /// <paramref name="T" /> doesn’t match the type returned by the data source or cannot be cast.</exception>
        public override Task<T> GetFieldValueAsync<T>(int ordinal, CancellationToken cancellationToken)
            => BaseReaderOpen().GetFieldValueAsync<T>(ordinal, cancellationToken);

        /// <summary>Returns the provider-specific field type of the specified column.</summary>
        /// <returns>The <see cref="T:System.Type" /> object that describes the data type of the specified column.</returns>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        public override Type GetProviderSpecificFieldType(int ordinal) => BaseReaderOpen().GetProviderSpecificFieldType(ordinal);

        /// <summary>Gets the value of the specified column as an instance of <see cref="T:System.Object" />.</summary>
        /// <returns>The value of the specified column.</returns>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        public override object GetProviderSpecificValue(int ordinal) => BaseReaderOpen().GetProviderSpecificValue(ordinal);

        /// <summary>Gets all provider-specific attribute columns in the collection for the current row.</summary>
        /// <returns>The number of instances of <see cref="T:System.Object" /> in the array.</returns>
        /// <param name="values">An array of <see cref="T:System.Object" /> into which to copy the attribute columns.</param>
        public override int GetProviderSpecificValues(object[] values) => BaseReaderOpen().GetProviderSpecificValues(values);

        /// <summary>Returns a <see cref="T:System.Data.DataTable" /> that describes the column metadata of the <see cref="T:System.Data.Common.DbDataReader" />.</summary>
        /// <returns>A <see cref="T:System.Data.DataTable" /> that describes the column metadata.</returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Data.SqlClient.SqlDataReader" /> is closed. </exception>
        public override DataTable GetSchemaTable()
        {
            if (IsOpen) return BaseReader.GetSchemaTable();
            if (IsFinished) return null;
            throw ClosedError();
        }

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

        /// <summary>An asynchronous version of <see cref="M:System.Data.Common.DbDataReader.IsDBNull(System.Int32)" />, which gets a value that indicates whether the column contains non-existent or missing values. Optionally, sends a notification that operations should be cancelled.</summary>
        /// <returns>true if the specified column value is equivalent to DBNull otherwise false.</returns>
        /// <param name="ordinal">The zero-based column to be retrieved.</param>
        /// <param name="cancellationToken">The cancellation instruction, which propagates a notification that operations should be canceled. This does not guarantee the cancellation. A setting of CancellationToken.None makes this method equivalent to <see cref="M:System.Data.Common.DbDataReader.IsDBNullAsync(System.Int32)" />. The returned task must be marked as cancelled.</param>
        /// <exception cref="T:System.InvalidOperationException">The connection drops or is closed during the data retrieval.The <see cref="T:System.Data.Common.DbDataReader" /> is closed during the data retrieval.There is no data ready to be read (for example, the first <see cref="M:System.Data.Common.DbDataReader.Read" /> hasn't been called, or returned false).Trying to read a previously read column in sequential mode.There was an asynchronous operation in progress. This applies to all Get* methods when running in sequential mode, as they could be called while reading a stream.</exception>
        /// <exception cref="T:System.IndexOutOfRangeException">Trying to read a column that does not exist.</exception>
        public override Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken)
            => BaseReaderOpen().IsDBNullAsync(ordinal, cancellationToken);

        /// <summary>This is the asynchronous version of <see cref="M:System.Data.Common.DbDataReader.NextResult" />. Providers should override with an appropriate implementation. The <paramref name="cancellationToken" /> may optionally be ignored.The default implementation invokes the synchronous <see cref="M:System.Data.Common.DbDataReader.NextResult" /> method and returns a completed task, blocking the calling thread. The default implementation will return a cancelled task if passed an already cancelled <paramref name="cancellationToken" />. Exceptions thrown by <see cref="M:System.Data.Common.DbDataReader.NextResult" /> will be communicated via the returned Task Exception property.Other methods and properties of the DbDataReader object should not be invoked while the returned Task is not yet completed.</summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <exception cref="T:System.Data.Common.DbException">An error occurred while executing the command text.</exception>
        public override Task<bool> NextResultAsync(CancellationToken cancellationToken)
        {
            if (IsOpen) return BaseReader.NextResultAsync(cancellationToken);
            if (IsFinished) return TaskResult.False;
            throw ClosedError();
        }

        /// <summary>
        /// A version of <see cref="NextResultAsync(CancellationToken)"/> which wont throw an exception.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        internal Task<bool> SafeNextResultAsync(CancellationToken cancellationToken)
        {
            return IsOpen ? BaseReader.NextResultAsync(cancellationToken) : TaskResult.False;
        }

        /// <summary>This is the asynchronous version of <see cref="M:System.Data.Common.DbDataReader.Read" />.  Providers should override with an appropriate implementation. The cancellationToken may optionally be ignored.The default implementation invokes the synchronous <see cref="M:System.Data.Common.DbDataReader.Read" /> method and returns a completed task, blocking the calling thread. The default implementation will return a cancelled task if passed an already cancelled cancellationToken.  Exceptions thrown by Read will be communicated via the returned Task Exception property.Do not invoke other methods and properties of the DbDataReader object until the returned Task is complete.</summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <exception cref="T:System.Data.Common.DbException">An error occurred while executing the command text.</exception>
        public override Task<bool> ReadAsync(CancellationToken cancellationToken)
        {
            if (IsOpen) return BaseReader.ReadAsync(cancellationToken);
            if (IsFinished) return TaskResult.False;
            throw ClosedError();
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
    }

    internal enum BatchReaderState : byte
    {
        Open,
        Finished,
        Closed
    }

    /// <summary>
    /// Base class for reading a forward-only stream of rows from a SQL Server database for a single <see cref="SqlBatchCommand"/>.
    /// </summary>
    /// <seealso cref="WebApplications.Utilities.Database.DbBatchDataReader" />
    public sealed class SqlBatchDataReader : DbBatchDataReader
    {
        /// <summary>
        /// Gets the underlying data reader.
        /// </summary>
        /// <value>
        /// The base reader.
        /// </value>
        [NotNull]
        private new SqlDataReader BaseReader => (SqlDataReader)base.BaseReader;

        private new SqlDataReader BaseReaderOpen([CallerMemberName] string name = null)
            => (SqlDataReader)base.BaseReaderOpen(name);

        /// <summary>
        /// Initializes a new instance of the <see cref="DbBatchDataReader"/> class.
        /// </summary>
        /// <param name="baseReader">The base reader.</param>
        internal SqlBatchDataReader([NotNull] SqlDataReader baseReader)
            : base(baseReader)
        {
        }

        // TODO Delegate all the methods and properties added by SqlDataReader
    }
}