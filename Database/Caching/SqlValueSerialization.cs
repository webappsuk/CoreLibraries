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
using System.Data.SqlTypes;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Database.Exceptions;

namespace WebApplications.Utilities.Database.Caching
{
    /// <summary>
    /// Handles serialization of values based on their <see cref="SqlDbType"/>.
    /// </summary>
    internal static class SqlValueSerialization
    {
        /// <summary>
        /// Serializes the <paramref name="sqlValue">value</paramref> to the <paramref name="stream" /> based on the
        /// <paramref name="type">specified type</paramref>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="sqlValue">The SQL value.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="stream"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="sqlValue"/> is <see langword="null"/>.</exception>
        /// <exception cref="SqlCachingException">The <paramref name="type"/> is unsupported.</exception>
        [NotNull]
        public static async Task SerializeAsync(
            SqlDbType type,
            [NotNull] Stream stream,
            [NotNull] object sqlValue,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (sqlValue == null) throw new ArgumentNullException(nameof(sqlValue));
            
            switch (type)
            {
                /*
                 * Variable length encodable types
                 */
                case SqlDbType.BigInt:
                    await VariableLengthEncoding.EncodeAsync(((SqlInt64)sqlValue).Value, stream, cancellationToken)
                        .ConfigureAwait(false);
                    return;
                case SqlDbType.Int:
                    await VariableLengthEncoding.EncodeAsync(((SqlInt32)sqlValue).Value, stream, cancellationToken)
                        .ConfigureAwait(false);
                    return;
                case SqlDbType.Decimal:
                    await VariableLengthEncoding.EncodeAsync(((SqlDecimal)sqlValue).Value, stream, cancellationToken)
                        .ConfigureAwait(false);
                    return;
                case SqlDbType.SmallMoney:
                case SqlDbType.Money:
                    await VariableLengthEncoding.EncodeAsync(((SqlMoney)sqlValue).Value, stream, cancellationToken)
                        .ConfigureAwait(false);
                    return;
                case SqlDbType.Real:
                case SqlDbType.Float:
                    await VariableLengthEncoding.EncodeAsync(((SqlSingle)sqlValue).Value, stream, cancellationToken)
                        .ConfigureAwait(false);
                    return;
                case SqlDbType.SmallDateTime:
                case SqlDbType.DateTime:
                    await
                        VariableLengthEncoding.EncodeAsync(
                            ((SqlDateTime)sqlValue).Value.ToBinary(),
                            stream,
                            cancellationToken)
                            .ConfigureAwait(false);
                    return;
                case SqlDbType.Date:
                case SqlDbType.DateTime2:
                    await VariableLengthEncoding.EncodeAsync(((DateTime)sqlValue).ToBinary(), stream, cancellationToken)
                        .ConfigureAwait(false);
                    return;
                case SqlDbType.Time:
                    await VariableLengthEncoding.EncodeAsync(((TimeSpan)sqlValue).Ticks, stream, cancellationToken)
                        .ConfigureAwait(false);
                    return;
                case SqlDbType.DateTimeOffset:
                    DateTimeOffset dto = (DateTimeOffset)sqlValue;
                    await
                        WriteAsync(
                            stream,
                            BitConverter.GetBytes((short)(dto.Offset.Ticks / TimeSpan.TicksPerMinute)),
                            true,
                            cancellationToken)
                            .ConfigureAwait(false);
                    // Encode ticks
                    await VariableLengthEncoding.EncodeAsync(dto.Ticks, stream, cancellationToken)
                        .ConfigureAwait(false);
                    break;

                /*
                 * Fixed length byte arrays
                 */
                case SqlDbType.SmallInt:
                    await
                        WriteAsync(stream, BitConverter.GetBytes(((SqlInt16)sqlValue).Value), true, cancellationToken)
                            .ConfigureAwait(false);
                    break;
                case SqlDbType.TinyInt:
                    await
                        WriteAsync(stream, new[] { ((SqlByte)sqlValue).Value }, true, cancellationToken)
                            .ConfigureAwait(false);
                    break;
                case SqlDbType.Bit:
                    await
                        WriteAsync(
                            stream,
                            new[] { ((SqlBoolean)sqlValue).Value ? (byte)1 : (byte)0 },
                            true,
                            cancellationToken)
                            .ConfigureAwait(false);
                    break;
                case SqlDbType.UniqueIdentifier:
                    await
                        WriteAsync(stream, ((SqlGuid)sqlValue).ToByteArray(), true, cancellationToken)
                            .ConfigureAwait(false);
                    break;

                /*
                 * Variable length byte arrays
                 */
                case SqlDbType.Image:
                case SqlDbType.Timestamp:
                case SqlDbType.VarBinary:
                case SqlDbType.Binary:
                    await WriteAsync(stream, ((SqlBinary)sqlValue).Value, false, cancellationToken).ConfigureAwait(false);
                    break;
                case SqlDbType.Char:
                case SqlDbType.VarChar:
                case SqlDbType.Text:
                    SqlString charString = (SqlString)sqlValue;
                    await VariableLengthEncoding.EncodeAsync(charString.LCID, stream, cancellationToken)
                            .ConfigureAwait(false);
                    await
                        VariableLengthEncoding.EncodeAsync((int)charString.SqlCompareOptions, stream, cancellationToken)
                            .ConfigureAwait(false);
                    await WriteAsync(stream, charString.GetNonUnicodeBytes(), false, cancellationToken)
                        .ConfigureAwait(false);
                    return;
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                    SqlString nCharString = (SqlString)sqlValue;
                    await VariableLengthEncoding.EncodeAsync(nCharString.LCID, stream, cancellationToken)
                            .ConfigureAwait(false);
                    await
                        VariableLengthEncoding.EncodeAsync((int)nCharString.SqlCompareOptions, stream, cancellationToken)
                            .ConfigureAwait(false);
                    await WriteAsync(stream, nCharString.GetUnicodeBytes(), false, cancellationToken)
                        .ConfigureAwait(false);
                    return;
                case SqlDbType.Xml:
                    await WriteAsync(stream, Encoding.Unicode.GetBytes(((SqlXml)sqlValue).Value), false, cancellationToken).ConfigureAwait(false);
                    return;
                case SqlDbType.Udt:
                case SqlDbType.Structured:
                case SqlDbType.Variant:
                    // TODO All these are possible to an extent, but are more complicated
                    throw new NotImplementedException();
                default:
                    throw new SqlCachingException(() => Resources.SqlValueSerialization_Serialize_Unsupported_SqlDbType, type);
            }
        }

        /// <summary>
        /// Writes the <paramref name="buffer">buffer's</paramref> length to the <paramref name="stream" /> if
        /// <paramref name="isFixedLength" /> is <see langword="false" />, followed by the contents, asynchronously.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="isFixedLength">if set to <see langword="true" /> the buffer is fixed length.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="Task">awaitable task</see>.</returns>
        [NotNull]
        public static async Task WriteAsync(
                    [NotNull] Stream stream,
                    [NotNull] byte[] buffer,
                    bool isFixedLength,
                    CancellationToken cancellationToken)
        {
            if (!isFixedLength)
                await VariableLengthEncoding.EncodeAsync(buffer.Length, stream, cancellationToken).ConfigureAwait(false);
            // ReSharper disable PossibleNullReferenceException
            await stream.WriteAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>
        /// Writes the <paramref name="value">string</paramref> to the <paramref name="stream" /> using UTF-8.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="value">The value.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="Task">awaitable task</see>.</returns>
        [NotNull]
        public static async Task WriteAsync(
                    [NotNull] Stream stream,
                    [NotNull] string value,
                    CancellationToken cancellationToken)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(value);
            await VariableLengthEncoding.EncodeAsync(buffer.Length, stream, cancellationToken).ConfigureAwait(false);
            // ReSharper disable PossibleNullReferenceException
            await stream.WriteAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>
        /// Writes the <paramref name="flags"/> to the <paramref name="stream" />.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="Task">awaitable task</see>.</returns>
        [NotNull]
        public static async Task<int> WriteFlagsAsync(
                    [NotNull] Stream stream,
                    [NotNull] IReadOnlyCollection<bool> flags,
                    CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[1 + ((flags.Count-1) >> 3)];
            int byt = 0;
            byte bit = 1;
            foreach (bool flag in flags)
            {
                if (flag) buffer[byt] |= bit;
                if (bit == 128)
                {
                    byt++;
                    bit = 1;
                }
                else bit <<= 1;
            }

            // ReSharper disable PossibleNullReferenceException
            await stream.WriteAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
            // ReSharper restore PossibleNullReferenceException

            return buffer.Length;
        }

        /// <summary>
        /// De-serializes a value from the <paramref name="stream" /> based on the
        /// <paramref name="type">specified type</paramref>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
        /// <exception cref="SqlCachingException">The <paramref name="type"/> is unsupported.</exception>
        public static async Task<object> DeSerializeAsync(
            SqlDbType type,
            Stream stream,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            
            byte[] buffer;
            switch (type)
            {
                /*
                 * Variable length encodable types
                 */
                case SqlDbType.BigInt:
                    return new SqlInt64(
                        await VariableLengthEncoding.DecodeLongAsync(stream, cancellationToken)
                            .ConfigureAwait(false));
                case SqlDbType.Int:
                    return new SqlInt32(
                        await VariableLengthEncoding.DecodeIntAsync(stream, cancellationToken)
                            .ConfigureAwait(false));
                case SqlDbType.Decimal:
                    return new SqlDecimal(
                        await VariableLengthEncoding.DecodeDecimalAsync(stream, cancellationToken)
                            .ConfigureAwait(false));
                case SqlDbType.SmallMoney:
                case SqlDbType.Money:
                    return new SqlMoney(
                        await VariableLengthEncoding.DecodeDecimalAsync(stream, cancellationToken)
                            .ConfigureAwait(false));
                case SqlDbType.Real:
                case SqlDbType.Float:
                    return new SqlSingle(
                        await VariableLengthEncoding.DecodeFloatAsync(stream, cancellationToken)
                            .ConfigureAwait(false));
                case SqlDbType.SmallDateTime:
                case SqlDbType.DateTime:
                    return
                        new SqlDateTime(
                            new DateTime(
                                await
                                    VariableLengthEncoding.DecodeLongAsync(stream, cancellationToken)
                                    .ConfigureAwait(false)));
                case SqlDbType.Date:
                case SqlDbType.DateTime2:
                    return new DateTime(
                        await VariableLengthEncoding.DecodeLongAsync(stream, cancellationToken)
                            .ConfigureAwait(false));
                case SqlDbType.Time:
                    return new TimeSpan(
                        await VariableLengthEncoding.DecodeLongAsync(stream, cancellationToken)
                            .ConfigureAwait(false));
                case SqlDbType.DateTimeOffset:
                    buffer = await ReadAsync(stream, 2, cancellationToken).ConfigureAwait(false);
                    TimeSpan offset = new TimeSpan(BitConverter.ToInt16(buffer, 0) * TimeSpan.TicksPerMinute);
                    return new DateTimeOffset(
                        await VariableLengthEncoding.DecodeLongAsync(stream, cancellationToken)
                            .ConfigureAwait(false),
                        offset);

                /*
                 * Fixed length byte arrays
                 */
                case SqlDbType.SmallInt:
                    buffer = await ReadAsync(stream, 2, cancellationToken).ConfigureAwait(false);
                    return new SqlInt16(BitConverter.ToInt16(buffer, 0));
                case SqlDbType.TinyInt:
                    buffer = await ReadAsync(stream, 1, cancellationToken).ConfigureAwait(false);
                    return new SqlByte(buffer[0]);
                case SqlDbType.Bit:
                    buffer = await ReadAsync(stream, 1, cancellationToken).ConfigureAwait(false);
                    return buffer[0] > 0 ? SqlBoolean.True : SqlBoolean.False;
                case SqlDbType.UniqueIdentifier:
                    buffer = await ReadAsync(stream, 16, cancellationToken).ConfigureAwait(false);
                    return new SqlGuid(buffer);

                /*
                 * Variable length byte arrays - read length, followed by bytes.
                 */
                case SqlDbType.Image:
                case SqlDbType.Timestamp:
                case SqlDbType.VarBinary:
                case SqlDbType.Binary:
                    buffer = await ReadAsync(stream, -1, cancellationToken).ConfigureAwait(false);
                    return new SqlBinary(buffer);
                case SqlDbType.Char:
                case SqlDbType.VarChar:
                case SqlDbType.Text:
                    return new SqlString(
                        await VariableLengthEncoding.DecodeIntAsync(stream, cancellationToken)
                            .ConfigureAwait(false),
                        (SqlCompareOptions)await VariableLengthEncoding.DecodeIntAsync(stream, cancellationToken)
                            .ConfigureAwait(false),
                        await ReadAsync(stream, -1, cancellationToken).ConfigureAwait(false),
                        false);
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                    return new SqlString(
                        await VariableLengthEncoding.DecodeIntAsync(stream, cancellationToken)
                            .ConfigureAwait(false),
                        (SqlCompareOptions)await VariableLengthEncoding.DecodeIntAsync(stream, cancellationToken)
                            .ConfigureAwait(false),
                        await ReadAsync(stream, -1, cancellationToken).ConfigureAwait(false),
                        false);
                case SqlDbType.Xml:
                    using (MemoryStream ms =
                        new MemoryStream(await ReadAsync(stream, -1, cancellationToken).ConfigureAwait(false)))
                        return new SqlXml(ms);
                case SqlDbType.Udt:
                case SqlDbType.Structured:
                case SqlDbType.Variant:
                    // TODO All these are possible to an extent, but are more complicated
                    throw new NotImplementedException();
                default:
                    throw new SqlCachingException(() => Resources.SqlValueSerialization_Serialize_Unsupported_SqlDbType, type);
            }
        }

        /// <summary>
        /// Reads the <paramref name="stream"/> asynchronously.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="length">The length in bytes; or negative to read length from stream first.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="Task">awaitable task</see> the result of which contains the bytes read.</returns>
        /// <exception cref="SqlCachingException">Unexpected end of <paramref name="stream"/>.</exception>
        [NotNull]
        public static async Task<byte[]> ReadAsync(
                    [NotNull] Stream stream,
                    int length,
                    CancellationToken cancellationToken)
        {
            if (length < 0)
                length = await VariableLengthEncoding.DecodeIntAsync(stream, cancellationToken).ConfigureAwait(false);
            // ReSharper disable PossibleNullReferenceException
            byte[] buffer = new byte[length];
            if (await stream.ReadAsync(buffer, 0, length, cancellationToken).ConfigureAwait(false) != length)
                throw new SqlCachingException(() => Resources.SqlValueSerialization_Deserialize_EOF);
            // ReSharper restore PossibleNullReferenceException
            return buffer;
        }

        /// <summary>
        /// Reads a UTF-8 string from the <paramref name="stream" /> asynchronously.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="Task">awaitable task</see> the result of which contains the bytes read.</returns>
        /// <exception cref="SqlCachingException">Unexpected end of <paramref name="stream"/>.</exception>
        [NotNull]
        public static async Task<string> ReadStringAsync(
                    [NotNull] Stream stream,
                    CancellationToken cancellationToken)
        {
            int length = await VariableLengthEncoding.DecodeIntAsync(stream, cancellationToken).ConfigureAwait(false);
            // ReSharper disable PossibleNullReferenceException
            byte[] buffer = new byte[length];
            if (await stream.ReadAsync(buffer, 0, length, cancellationToken).ConfigureAwait(false) != length)
                throw new SqlCachingException(() => Resources.SqlValueSerialization_Deserialize_EOF);
            // ReSharper restore PossibleNullReferenceException
            return Encoding.UTF8.GetString(buffer);
        }

        /// <summary>
        /// Reads the <paramref name="length" /> flags from the <paramref name="stream" />.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="length">The length.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="Task">awaitable task</see>.</returns>
        /// <exception cref="SqlCachingException">Unexpected end of <paramref name="stream"/>.</exception>
        [NotNull]
        public static async Task<bool[]> ReadFlagsAsync(
                    [NotNull] Stream stream,
                    int length,
                    CancellationToken cancellationToken)
        {
            int byts = 1 + ((length - 1) >> 3);
            byte[] buffer = new byte[byts];
            // ReSharper disable PossibleNullReferenceException
            if (await stream.ReadAsync(buffer, 0, byts, cancellationToken).ConfigureAwait(false) != byts)
                throw new SqlCachingException(() => Resources.SqlValueSerialization_Deserialize_EOF);
            // ReSharper restore PossibleNullReferenceException

            bool[] flags = new bool[length];
            int f = 0;
            foreach (byte byt in buffer)
            {
                byte b = byt;
                for (int a = 0; a< 8; a++)
                {
                    if (1 == (b & 1)) flags[f] = true;
                    b >>= 1;
                    f++;
                    if (f >= length) break;
                }
            }

            return flags;
        }
    }
}