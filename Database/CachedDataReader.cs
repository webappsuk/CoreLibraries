// ***********************************************************************
// Assembly         : WebApplications.Utilities.Database
// Author           : Craig.Dean
// Created          : 12-08-2015
//
// Last Modified By : Craig.Dean
// Last Modified On : 12-08-2015
// ***********************************************************************
// <copyright file="CachedDataReader.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
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
using System.Threading;
using System.Threading.Tasks;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Database
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
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public override void Close()
        {
            if (Interlocked.CompareExchange(ref _streamState, 1, 0) == 0)
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
        public override string GetName(int ordinal)
        {
            throw new NotImplementedException();
        }

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
        public override bool IsDBNull(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the field count.
        /// </summary>
        /// <value>The field count.</value>
        public override int FieldCount { get; }

        /// <summary>
        /// Gets the <see cref="System.Object"/> with the specified ordinal.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override object this[int ordinal]
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the <see cref="System.Object"/> with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override object this[string name]
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has rows.
        /// </summary>
        /// <value><see langword="true" /> if this instance has rows; otherwise, <see langword="false" />.</value>
        public override bool HasRows { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is closed.
        /// </summary>
        /// <value><see langword="true" /> if this instance is closed; otherwise, <see langword="false" />.</value>
        public override bool IsClosed => _streamState > 0;

        /// <summary>
        /// Gets the records affected.
        /// </summary>
        /// <value>The records affected.</value>
        public override int RecordsAffected { get; }

        /// <summary>
        /// Nexts the result.
        /// </summary>
        /// <returns><see langword="true" /> if XXXX, <see langword="false" /> otherwise.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override bool NextResult()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Nexts the result asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public override Task<bool> NextResultAsync(CancellationToken cancellationToken)
        {
            // TODO
            return base.NextResultAsync(cancellationToken);
        }

        /// <summary>
        /// Reads this instance.
        /// </summary>
        /// <returns><see langword="true" /> if XXXX, <see langword="false" /> otherwise.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override bool Read()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads the asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public override Task<bool> ReadAsync(CancellationToken cancellationToken)
        {
            // TODO
            return base.ReadAsync(cancellationToken);
        }

        /// <summary>
        /// Gets the depth.
        /// </summary>
        /// <value>The depth.</value>
        public override int Depth { get; }

        /// <summary>
        /// Gets the ordinal.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.Int32.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override int GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

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