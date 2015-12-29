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
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Database.Exceptions;

namespace WebApplications.Utilities.Database.Caching
{
    /// <summary>
    /// Extension methods for serializing data to a binary stream.
    /// </summary>
    public static class CachedDataWriter
    {
        /// <summary>
        /// Serialize a <see cref="DataSet" /> to a <see cref="T:byte[]" />.
        /// </summary>
        /// <param name="dataSet">The <see cref="DataSet" /> to serialize.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable <see cref="Task"/> that results in a <see cref="T:byte[]"/>.</returns>
        [PublicAPI]
        [NotNull]
        public static async Task<byte[]> GetBytesAsync([NotNull] this DataSet dataSet, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (dataSet == null)
                throw new ArgumentNullException(nameof(dataSet));

            using (MemoryStream memoryStream = new MemoryStream())
            {
                await SerializeAsync(dataSet.CreateDataReader(), memoryStream, cancellationToken).ConfigureAwait(false);
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Serialize a <see cref="DataSet" /> to a binary stream.
        /// </summary>
        /// <param name="dataSet">The <see cref="DataSet" /> to serialize.</param>
        /// <param name="stream">The <see cref="Stream" /> to write to.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable <see cref="Task"/>.</returns>
        [PublicAPI]
        [NotNull]
        public static Task SerializeAsync([NotNull] this DataSet dataSet, [NotNull] Stream stream, CancellationToken cancellationToken = default(CancellationToken))
            => SerializeAsync(dataSet.CreateDataReader(), stream, cancellationToken);

        /// <summary>
        /// Serialize a <see cref="DataTable" /> to a <see cref="T:byte[]" />.
        /// </summary>
        /// <param name="dataTable">The <see cref="DataTable" /> to serialize.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable <see cref="Task"/> that results in a <see cref="T:byte[]"/>.</returns>
        [PublicAPI]
        [NotNull]
        public static async Task<byte[]> GetBytesAsync([NotNull] this DataTable dataTable, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (dataTable == null)
                throw new ArgumentNullException(nameof(dataTable));
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await SerializeAsync(dataTable.CreateDataReader(), memoryStream, cancellationToken).ConfigureAwait(false);
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Serialize a <see cref="DataTable" /> to a binary stream.
        /// </summary>
        /// <param name="dataTable">The <see cref="DataTable" /> to serialize.</param>
        /// <param name="stream">The <see cref="Stream" /> to write to.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable <see cref="Task"/>.</returns>
        [PublicAPI]
        [NotNull]
        public static Task SerializeAsync([NotNull] this DataTable dataTable, [NotNull] Stream stream, CancellationToken cancellationToken = default(CancellationToken))
            => SerializeAsync(dataTable.CreateDataReader(), stream, cancellationToken);

        /// <summary>
        /// Serialize a <see cref="DbDataReader" /> to a <see cref="T:byte[]" /> using protocol-buffers.
        /// </summary>
        /// <param name="dataReader">The <see cref="DbDataReader" /> to serialize.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable <see cref="Task"/> that results in a <see cref="T:byte[]"/>.</returns>
        [PublicAPI]
        [NotNull]
        public static async Task<byte[]> GetBytesAsync([NotNull] this DbDataReader dataReader, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (dataReader == null)
                throw new ArgumentNullException(nameof(dataReader));
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await SerializeAsync(dataReader, memoryStream, cancellationToken).ConfigureAwait(false);
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Serialize a <see cref="DbDataReader" /> to a binary stream.
        /// </summary>
        /// <param name="dataReader">The <see cref="DbDataReader" /> to serialize.</param>
        /// <param name="stream">The <see cref="Stream" /> to write to.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable <see cref="Task"/>.</returns>
        [PublicAPI]
        [NotNull]
        public static async Task SerializeAsync([NotNull] this DbDataReader dataReader, [NotNull] Stream stream, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (dataReader == null)
                throw new ArgumentNullException(nameof(dataReader));
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            List<int> nullableColumns = new List<int>();
            HashSet<int> isNull = new HashSet<int>();
            SqlDbType[] sqlDbTypes = null;
            object[] values = null;
            bool[] rowFlags = null;

            /*
             * Write header
             */
            await VariableLengthEncoding.EncodeAsync(dataReader.RecordsAffected, stream, cancellationToken)
                    .ConfigureAwait(false);

            // Loop through results
            do
            {
                /*
                 * Write Table header
                 */
                uint fieldCount = (uint)dataReader.FieldCount;

                // Empty result
                if (fieldCount == 0)
                    break;

                bool hasRows = dataReader.HasRows;

                // Encode number of columns, and flag for has rows.
                await VariableLengthEncoding.EncodeAsync((uint) (fieldCount*2 + (hasRows ? 1 : 0)), stream, cancellationToken).ConfigureAwait(false);
                
                // Grow buffers
                if (sqlDbTypes == null || sqlDbTypes.Length < fieldCount)
                {
                    sqlDbTypes = new SqlDbType[fieldCount];
                    values = new object[fieldCount];
                }

                // Get the schema so we can store information about the columns
                using (DataTable table = dataReader.GetSchemaTable())
                {
                    Debug.Assert(table.Rows != null);
                    Debug.Assert(table.Rows.Count == fieldCount);

                    // Find ordinals for key columns
                    int cnindex = table.Columns.IndexOf("ColumnName");
                    int ptindex = table.Columns.IndexOf("ProviderType");
                    int anindex = table.Columns.IndexOf("AllowDBNull");
                    int coindex = table.Columns.IndexOf("ColumnOrdinal");
                    Debug.Assert(cnindex > -1);
                    Debug.Assert(ptindex > -1);
                    int ordinal = 0;
                    foreach (DataRow row in table.Rows.Cast<DataRow>().OrderBy(r => (int)r[coindex]))
                    {
                        // Grab the values we care about
                        // ReSharper disable PossibleNullReferenceException
                        int reportedOrdinal = (int)row[coindex];
                        bool allowDbNull = (bool)row[anindex];
                        SqlDbType sqlDbType = (SqlDbType)row[ptindex];
                        string columnName = row[cnindex] as string;
                        bool cnIsNull = columnName == null;
                        // ReSharper restore PossibleNullReferenceException

                        // Check we have all ordinals from 0 to fieldCount -1
                        if (reportedOrdinal != ordinal++)
                            throw new SqlCachingException(
                                () => Resources.CachedDataWriter_SerializeAsync_Sparse_Columns);

                        // Keep track of nullable columns
                        if (allowDbNull)
                            nullableColumns.Add(ordinal);

                        // Store SqlDbType for serializing values
                        sqlDbTypes[reportedOrdinal] = sqlDbType;

                        // Write flags
                        await SqlValueSerialization.WriteFlagsAsync(
                            stream,
                            new[] { allowDbNull, cnIsNull },
                            cancellationToken)
                            .ConfigureAwait(false);

                        // Encode the SqlDbType
                        // ReSharper disable once PossibleNullReferenceException
                        await VariableLengthEncoding.EncodeAsync((int)row[ptindex], stream, cancellationToken)
                            .ConfigureAwait(false);

                        // Encode the name
                        if (!cnIsNull)
                            await SqlValueSerialization.WriteAsync(stream, columnName, cancellationToken)
                                .ConfigureAwait(false);
                    }
                }

                // Get the number of nullable columns and create or enlarge (if necessary) the rowFlags array
                int nullableColumnsCount = nullableColumns.Count;
                if (rowFlags == null || rowFlags.Length <= nullableColumnsCount)
                    rowFlags = new bool[nullableColumnsCount + 1];
                else
                    rowFlags[0] = false;

                /*
                 * Write table data
                 */
                while (await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    // Read columns
                    dataReader.GetProviderSpecificValues(values);

                    // Set nullable flags
                    int f = 1;
                    foreach (int ordinal in nullableColumns)
                    {
                        bool i = values[ordinal].IsNull();
                        rowFlags[f++] = i;
                        if (i)
                            isNull.Add(ordinal);
                    }

                    // Write row flags
                    await SqlValueSerialization.WriteFlagsAsync(
                        stream,
                        rowFlags,
                        cancellationToken)
                        .ConfigureAwait(false);

                    // Write column data
                    for (int ordinal = 0; ordinal < fieldCount; ordinal++)
                    {
                        if (isNull.Contains(ordinal)) continue;

                        await SqlValueSerialization.SerializeAsync(
                            sqlDbTypes[ordinal],
                            stream,
                            // ReSharper disable once AssignNullToNotNullAttribute
                            values[ordinal],
                            cancellationToken).ConfigureAwait(false);
                    }
                }

                // Write end of record set rowflags
                rowFlags[0] = true;
                await SqlValueSerialization.WriteFlagsAsync(
                    stream,
                    rowFlags,
                    cancellationToken)
                    .ConfigureAwait(false);


                // Clear nullable columns
                nullableColumns.Clear();
                isNull.Clear();
            } while (await dataReader.NextResultAsync(cancellationToken).ConfigureAwait(false));

            // Terminate stream
            await stream.WriteAsync(new byte[] { 0 }, 0, 1, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Serialize a <see cref="DbDataReader" /> to a <see cref="T:byte[]" />.
        /// </summary>
        /// <param name="parameterCollection">The parameter collection.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable <see cref="Task"/> that results in a <see cref="T:byte[]"/>.</returns>
        [PublicAPI]
        [NotNull]
        public static async Task<byte[]> GetBytesAsync([NotNull] this IDataParameterCollection parameterCollection, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await SerializeAsync(parameterCollection, memoryStream, cancellationToken).ConfigureAwait(false);
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Serialize a <see cref="DbDataReader" /> to a binary stream.
        /// </summary>
        /// <param name="parameterCollection">The parameter collection.</param>
        /// <param name="stream">The <see cref="Stream" /> to write to.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable <see cref="Task"/>.</returns>
        [PublicAPI]
        [NotNull]
        public static async Task SerializeAsync([NotNull] this IDataParameterCollection parameterCollection, [NotNull] Stream stream, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }


    }
}