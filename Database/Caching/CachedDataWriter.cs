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
using System.Data;
using System.Data.Common;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WebApplications.Utilities.Annotations;

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

            // Create a header record and write to stream.
            Header.Read(dataReader).Serialize(stream);

            // Loop through results
            do
            {
                // Get table definition from reader and write to stream.
                TableDefinition tableDefinition = TableDefinition.Read(dataReader);
                tableDefinition.Serialize(stream);

                if (tableDefinition.HasRows)
                {
                    /*
                     * Write table data
                     */
                    // ReSharper disable PossibleNullReferenceException
                    while (await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
                        // ReSharper restore PossibleNullReferenceException

                        // Get a row from the data reader and write to stream.
                        Row.Read(tableDefinition, dataReader).Serialize(stream);
                }

                // Write the end row marker to the stream.
                Row.End.Serialize(stream);

                // ReSharper disable PossibleNullReferenceException
            } while (await dataReader.NextResultAsync(cancellationToken).ConfigureAwait(false));
            // ReSharper restore PossibleNullReferenceException

            // Terminate stream with end marker
            TableDefinition.End.Serialize(stream);
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