#region © Copyright Web Applications (UK) Ltd, 2016.  All rights reserved.
// Copyright (c) 2016, Web Applications UK Ltd
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
using System.Data.Common;
using System.IO;
using System.Threading;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Database.Exceptions;
using WebApplications.Utilities.Serialization;

namespace WebApplications.Utilities.Database.Caching
{
    /// <summary>
    /// Holds the current row.
    /// </summary>
    public class Row
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
        /// The underlying row data.
        /// </summary>
        [CanBeNull]
        private readonly byte[] _data;

        /// <summary>
        /// The SQL values
        /// </summary>
        [CanBeNull]
        private readonly Lazy<object[]> _sqlValues;

        /// <summary>
        /// The CLR values lazy initializer.
        /// </summary>
        [CanBeNull]
        private readonly Lazy<object[]> _values;

        /// <summary>
        /// Creates a default, invalid, instance of the <see cref="Row"/> class from being created.
        /// </summary>
        private Row()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Row" /> class.
        /// </summary>
        /// <param name="tableDefinition">The table definition.</param>
        /// <param name="data">The data.</param>
        private Row([NotNull] TableDefinition tableDefinition, [NotNull] byte[] data)
        {
            _tableDefinition = tableDefinition;
            _data = data;
            _sqlValues = new Lazy<object[]>(
                () => tableDefinition.DeserializeSqlValues(data),
                LazyThreadSafetyMode.ExecutionAndPublication);
            _values = new Lazy<object[]>(
                () => tableDefinition.DeserializeValues(data),
                LazyThreadSafetyMode.ExecutionAndPublication);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Row"/> class.
        /// </summary>
        /// <param name="tableDefinition">The table definition.</param>
        /// <param name="sqlValues">The SQL values.</param>
        private Row([NotNull] TableDefinition tableDefinition, [NotNull] object[] sqlValues)
        {
            _tableDefinition = tableDefinition;
            _data = tableDefinition.SerializeRow(sqlValues);
            _sqlValues = new Lazy<object[]>(() => sqlValues, true);
            _values = new Lazy<object[]>(
                () => tableDefinition.DeserializeValues(_data),
                LazyThreadSafetyMode.ExecutionAndPublication);
        }

        /// <summary>
        /// Reads a <see cref="Row" /> from the <paramref name="dataReader">specified data reader</paramref>.
        /// </summary>
        /// <param name="tableDefinition">The table definition.</param>
        /// <param name="dataReader">The data reader.</param>
        /// <returns>A <see cref="Row" />.</returns>
        [NotNull]
        internal static Row Read([NotNull] TableDefinition tableDefinition, [NotNull] DbDataReader dataReader)
        {
            // Get SQL Values
            object[] sqlValues = new object[tableDefinition.Count];
            dataReader.GetProviderSpecificValues(sqlValues);

            return new Row(tableDefinition, sqlValues);
        }

        /// <summary>
        /// Reads a <see cref="Row" /> from the <paramref name="stream">specified stream</paramref>.
        /// </summary>
        /// <param name="tableDefinition">The table definition.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>A <see cref="Row" />.</returns>
        [NotNull]
        internal static Row Read([NotNull] TableDefinition tableDefinition, [NotNull]Stream stream)
        {
            long length = (long)VariableLengthEncoding.DecodeULong(stream);
            if (length == 0) return End;

            byte[] data = new byte[length];
            if (stream.Read(data, 0, length) != length)
                throw new SqlCachingException(() => Resources.Deserialize_EOF);

            return new Row(tableDefinition, data);
        }

        /// <summary>
        /// Serializes this instance to the <paramref name="stream">specified stream</paramref>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void Serialize([NotNull]Stream stream)
        {
            // Create zero-length row to indicate end row (table terminator).
            if (IsEnd)
            {
                stream.WriteByte(0);
                return;
            }

            // Check for data
            if (_data == null || _data.Length < 1)
                throw new InvalidOperationException("Invalid attempt to write when no data is present.");

            // Encode length
            long length = _data.LongLength;
            VariableLengthEncoding.Encode((ulong)length, stream);

            // Write out data.
            stream.Write(_data, 0, length);
        }

        /// <summary>
        /// The SQL values
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Invalid attempt to read when no data is present.</exception>
        [NotNull]
        public object[] SqlValues
        {
            get
            {
                if (_sqlValues == null)
                    throw new InvalidOperationException("Invalid attempt to read when no data is present.");
                // ReSharper disable once AssignNullToNotNullAttribute
                return _sqlValues.Value;
            }
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>The values.</value>
        /// <exception cref="System.InvalidOperationException">Invalid attempt to read when no data is present.</exception>
        [NotNull]
        public object[] Values
        {
            get
            {
                if (_values == null)
                    throw new InvalidOperationException("Invalid attempt to read when no data is present.");
                // ReSharper disable once AssignNullToNotNullAttribute
                return _values.Value;
            }
        }

        /// <summary>
        /// The associated table definition.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Invalid attempt to read when no data is present.</exception>
        [NotNull]
        public TableDefinition TableDefinition
        {
            get
            {
                if (_tableDefinition == null)
                    throw new InvalidOperationException("Invalid attempt to read when no data is present.");
                return _tableDefinition;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is the end.
        /// </summary>
        /// <value><see langword="true" /> if this instance is end; otherwise, <see langword="false" />.</value>
        public bool IsEnd => ReferenceEquals(this, End);

        /// <summary>
        /// Gets a value indicating whether this instance is not read.
        /// </summary>
        /// <value><see langword="true" /> if this instance is not read; otherwise, <see langword="false" />.</value>
        public bool IsNotRead => ReferenceEquals(this, NotRead);
    }
}