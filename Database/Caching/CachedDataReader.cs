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
using System.Data.Common;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Database.Caching
{
    /// <summary>
    /// Class CachedDataReader. This class cannot be inherited.
    /// </summary>
    public sealed class CachedDataReader : DbDataReader
    {
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
        /// The header information.
        /// </summary>
        [NotNull]
        public readonly Header Header;

        /// <summary>
        /// The current table definition.
        /// </summary>
        [NotNull]
        public TableDefinition TableDefinition;

        /// <summary>
        /// The current row.
        /// </summary>
        [NotNull]
        public Row Row = Row.NotRead;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedDataReader" /> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public CachedDataReader([NotNull] byte[] data) : this(new MemoryStream(data, false))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedDataReader"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public CachedDataReader([NotNull] Stream stream)
        {
            _stream = stream;

            // Read header
            Header = Header.Read(stream);

            // Read first table definition
            TableDefinition = TableDefinition.Read(stream);
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public override void Close()
        {
            TableDefinition = TableDefinition.End;
            Row = Row.End;
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

            TableDefinition = TableDefinition.End;
            Row = Row.End;
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
        public override string GetName(int ordinal) => TableDefinition[ordinal].Name;

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>System.Int32.</returns>
        public override int GetValues(object[] values)
        {
            if (values.Length < 1) return 0;

            int c = 0;
            foreach (object value in Row.Values)
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
            foreach (object value in Row.SqlValues)
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
        public override bool IsDBNull(int ordinal) => Row.SqlValues[ordinal].IsNull();

        /// <summary>
        /// Gets the field count.
        /// </summary>
        /// <value>The field count.</value>
        public override int FieldCount => TableDefinition.Count;

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
        public override bool HasRows => TableDefinition.HasRows;

        /// <summary>
        /// Gets a value indicating whether this instance is closed.
        /// </summary>
        /// <value><see langword="true" /> if this instance is closed; otherwise, <see langword="false" />.</value>
        public override bool IsClosed => _streamState > 0;

        /// <summary>
        /// Gets the records affected.
        /// </summary>
        /// <value>The records affected.</value>
        public override int RecordsAffected
        {
            get
            {
                if (_streamState != 0)
                    throw new InvalidOperationException(Resources.CachedDataReader_Closed);
                return Header.RecordsAffected;
            }
        }

        /// <summary>
        /// Gets the next result
        /// </summary>
        /// <returns><see langword="true" /> if XXXX, <see langword="false" /> otherwise.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override bool NextResult()
        {
            if (_streamState != 0)
                throw new InvalidOperationException(Resources.CachedDataReader_Closed);

            // Check whether we're already at th end
            if (TableDefinition.IsEnd)
                return false;

            // Skip over any remaining rows, rows are encoded with a length first, and are terminated
            // by a zero-length row.
            do
            {
                int read;
                ulong rowLength = VariableLengthEncoding.DecodeULong(_stream, out read);
                if (rowLength < 1) break;
                _stream.Seek((long)rowLength, SeekOrigin.Current);
            } while (true);

            // Read next table definition.
            TableDefinition = TableDefinition.Read(_stream);

            // Set row to not read.
            Row = Row.NotRead;

            // Return true if we're not at the end.
            return !TableDefinition.IsEnd;
        }

        /// <summary>
        /// Reads this instance.
        /// </summary>
        /// <returns><see langword="true" /> if XXXX, <see langword="false" /> otherwise.</returns>
        // ReSharper disable once PossibleNullReferenceException
        public override bool Read()
        {
            if (_streamState != 0)
                throw new InvalidOperationException(Resources.CachedDataReader_Closed);

            TableDefinition tableDefinition = TableDefinition;
            if (tableDefinition.IsEnd || !tableDefinition.HasRows || Row == Row.End) return false;

            // Read next row.
            Row = Row.Read(tableDefinition, _stream);
            return !Row.IsEnd;
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
                    throw new InvalidOperationException(Resources.CachedDataReader_Closed);
                return Header.Depth;
            }
        }

        /// <summary>
        /// Gets the ordinal.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.Int32.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetOrdinal(string name) => TableDefinition[name].Ordinal;

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
        public override string GetDataTypeName(int ordinal) => TableDefinition[ordinal].SqlDbTypeInfo.Name;

        /// <summary>
        /// Gets the type of the field.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>Type.</returns>
        public override Type GetFieldType(int ordinal) => TableDefinition[ordinal].SqlDbTypeInfo.ClrType;

        /// <summary>
        /// Returns the provider-specific field type of the specified column.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The <see cref="T:System.Type" /> object that describes the data type of the specified column.</returns>
        public override Type GetProviderSpecificFieldType(int ordinal)
            => TableDefinition[ordinal].SqlDbTypeInfo.ProviderType;

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>System.Object.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override object GetValue(int ordinal) => Row.Values[ordinal];

        /// <summary>
        /// Gets the value of the specified column as an instance of <see cref="T:System.Object" />.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override object GetProviderSpecificValue(int ordinal) => Row.SqlValues[ordinal];

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