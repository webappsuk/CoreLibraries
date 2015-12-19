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
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Class for performing variable length encoding.
    /// </summary>
    [PublicAPI]
    public static class VariableLengthEncoding
    {
        /// <summary>
        /// Encodes the specified value to a byte[].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The compressed value.</returns>
        /// <seealso cref="Decode(byte[],long)"/>
        /// <seealso cref="Decode(byte[],ref long)"/>
        /// <remarks>
        /// <para>This compression assumes a weighting towards lower values, an is ideally suited for lengths.  It uses
        /// 7-bits per byte, with the most significant bit indicating that another byte is used.  This means it also
        /// encodes its own length and can act as a variable length header.</para>
        /// <para>Values less than 268,435,456 (1 &lt;&lt; 28) take a maximum of four bytes, and only 1 byte for values
        /// less than 128 (1 &lt;&lt; 7).  As such the compression becomes inefficient above 28 bits, however it has
        /// the distinct benefit of being able to encode all possible ulong values, requiring up to 10 bytes for the
        /// largest values, at a cost of two bytes compared to a ulong.</para>
        /// </remarks>
        [NotNull]
        [PublicAPI]
        public static byte[] Encode(ulong value)
        {
            byte[] result = new byte[10];
            long offset = 0;
            // ReSharper disable once ExceptionNotDocumented
            Encode(value, result, ref offset);
            if (offset < 9) Array.Resize(ref result, (int)offset);
            // ReSharper disable once AssignNullToNotNullAttribute
            return result;
        }

        /// <summary>
        /// Encodes the specified value to a byte[].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The compressed value.</returns>
        /// <seealso cref="Decode(byte[],long)"/>
        /// <seealso cref="Decode(byte[],ref long)"/>
        /// <remarks><para>This compression assumes a weighting towards lower values, an is ideally suited for lengths.  It uses
        /// 7-bits per byte, with the most significant bit indicating that another byte is used.  This means it also
        /// encodes its own length and can act as a variable length header.</para>
        /// <para>Values less than 268,435,456 (1 &lt;&lt; 28) take a maximum of four bytes, and only 1 byte for values
        /// less than 128 (1 &lt;&lt; 7).  As such the compression becomes inefficient above 28 bits, however it has
        /// the distinct benefit of being able to encode all possible ulong values, requiring up to 10 bytes for the
        /// largest values, at a cost of two bytes compared to a ulong.</para></remarks>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
        /// <exception cref="InternalBufferOverflowException">Could not encode the value as the buffer ran out of space.</exception>
        [PublicAPI]
        public static void Encode(ulong value, [NotNull] byte[] buffer, long offset = 0)
            => Encode(value, buffer, ref offset);

        /// <summary>
        /// Encodes the specified value to a byte[].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The compressed value.</returns>
        /// <seealso cref="Decode(byte[],long)"/>
        /// <seealso cref="Decode(byte[],ref long)"/>
        /// <remarks><para>This compression assumes a weighting towards lower values, an is ideally suited for lengths.  It uses
        /// 7-bits per byte, with the most significant bit indicating that another byte is used.  This means it also
        /// encodes its own length and can act as a variable length header.</para>
        /// <para>Values less than 268,435,456 (1 &lt;&lt; 28) take a maximum of four bytes, and only 1 byte for values
        /// less than 128 (1 &lt;&lt; 7).  As such the compression becomes inefficient above 28 bits, however it has
        /// the distinct benefit of being able to encode all possible ulong values, requiring up to 10 bytes for the
        /// largest values, at a cost of two bytes compared to a ulong.</para></remarks>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
        /// <exception cref="InternalBufferOverflowException">Could not encode the value as the buffer ran out of space.</exception>
        [PublicAPI]
        public static void Encode(ulong value, [NotNull] byte[] buffer, ref long offset)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            long l = buffer.LongLength;
            do
            {
                if (offset > l)
                    throw new InternalBufferOverflowException(Resources.VariableLengthEncoding_Encode_Buffer_Overflow);
                byte b = (byte)(value & 0x7F);
                bool end = b == value;
                buffer[offset++] = (byte)(b | (end ? 0 : 0x80));
                if (end) break;
                value >>= 7;
            } while (true);
        }

        /// <summary>
        /// Encodes the specified value to a <see cref="Stream" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>The compressed value.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="stream" /> is <see langword="null" />.</exception>
        /// <seealso cref="Decode(Stream, out int)" />
        /// <remarks><para>This compression assumes a weighting towards lower values, an is ideally suited for lengths.  It uses
        /// 7-bits per byte, with the most significant bit indicating that another byte is used.  This means it also
        /// encodes its own length and can act as a variable length header.</para>
        /// <para>Values less than 268,435,456 (1 &lt;&lt; 28) take a maximum of four bytes, and only 1 byte for values
        /// less than 128 (1 &lt;&lt; 7).  As such the compression becomes inefficient above 28 bits, however it has
        /// the distinct benefit of being able to encode all possible ulong values, requiring up to 10 bytes for the
        /// largest values, at a cost of two bytes compared to a ulong.</para></remarks>
        [PublicAPI]
        public static int Encode(ulong value, [NotNull] Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            int written = 0;
            do
            {
                byte b = (byte)(value & 0x7F);
                bool end = b == value;
                stream.WriteByte((byte)(b | (end ? 0 : 0x80)));
                written++;
                if (end) break;
                value >>= 7;
            } while (true);
            return written;
        }

        /// <summary>
        /// Encodes the specified value to a <see cref="Stream" /> asynchronously.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns><see cref="Task{T}">An awaitable task</see>; the <see cref="Task{T}.Result">result of which</see>
        /// is the number of bytes written.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <seealso cref="DecodeAsync(Stream, CancellationToken)" />
        /// <remarks><para>This compression assumes a weighting towards lower values, an is ideally suited for lengths.  It uses
        /// 7-bits per byte, with the most significant bit indicating that another byte is used.  This means it also
        /// encodes its own length and can act as a variable length header.</para>
        /// <para>Values less than 268,435,456 (1 &lt;&lt; 28) take a maximum of four bytes, and only 1 byte for values
        /// less than 128 (1 &lt;&lt; 7).  As such the compression becomes inefficient above 28 bits, however it has
        /// the distinct benefit of being able to encode all possible ulong values, requiring up to 10 bytes for the
        /// largest values, at a cost of two bytes compared to a ulong.</para></remarks>
        [PublicAPI]
        public static async Task<int> EncodeAsync(ulong value, [NotNull] Stream stream, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            byte[] result = new byte[10];
            long offset = 0;
            // ReSharper disable once ExceptionNotDocumented
            Encode(value, result, ref offset);
            int written = (int)offset;
            // ReSharper disable once PossibleNullReferenceException
            await stream.WriteAsync(result, 0, written, cancellationToken).ConfigureAwait(false);
            return written;
        }

        /// <summary>
        /// Decodes the specified value from the buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset to start from.</param>
        /// <returns>The uncompressed value.</returns>
        /// <seealso cref="Encode(ulong)"/>
        /// <seealso cref="Encode(ulong,byte[],long)"/>
        /// <seealso cref="Encode(ulong,byte[],ref long)"/>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
        /// <remarks><para>As this uses a variable length encoding that encodes it's own length a length is not required.</para></remarks>
        /// <exception cref="InternalBufferOverflowException">The encoding was invalid, ran out of bytes before getting an end of encoding byte.</exception>
        [PublicAPI]
        public static ulong Decode([NotNull] byte[] buffer, long offset = 0) => Decode(buffer, ref offset);

        /// <summary>
        /// Decodes the specified value from the buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset to start from.</param>
        /// <returns>The uncompressed value.</returns>
        /// <seealso cref="Encode(ulong)"/>
        /// <seealso cref="Encode(ulong,byte[],long)"/>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
        /// <remarks><para>As this uses a variable length encoding that encodes it's own length a length is not required.</para></remarks>
        /// <exception cref="InternalBufferOverflowException">The encoding was invalid, ran out of bytes before getting an end of encoding byte.</exception>
        [PublicAPI]
        public static ulong Decode([NotNull] byte[] buffer, ref long offset)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            ulong result = 0;
            int shift = 0;
            // ReSharper disable ExceptionNotDocumented
            long end = (buffer.LongLength - offset) < 10 ? buffer.LongLength : offset + 10;
            // ReSharper restore ExceptionNotDocumented
            while (offset < end)
            {
                byte b = buffer[offset++];
                result += (ulong)(b & 0x7F) << shift;
                if ((b & 0x80) != 0x80) break;
                if (offset == end)
                    throw new InternalBufferOverflowException(Resources.VariableLengthEncoding_Decode_Buffer_Overflow);
                shift += 7;
            }
            return result;
        }

        /// <summary>
        /// Decodes the specified value from the <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="read">The number of bytes read.</param>
        /// <returns>The uncompressed value.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.IO.InternalBufferOverflowException"></exception>
        /// <exception cref="ArgumentNullException"><paramref name="stream" /> is <see langword="null" />.</exception>
        /// <exception cref="InternalBufferOverflowException">The encoding was invalid, ran out of bytes before getting an end of encoding byte.</exception>
        /// <seealso cref="Encode(ulong)" />
        /// <seealso cref="Encode(ulong,byte[],long)" />
        /// <remarks>As this uses a variable length encoding that encodes it's own length a length is not required.</remarks>
        [PublicAPI]
        public static ulong Decode([NotNull] Stream stream, out int read)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            ulong result = 0;
            int shift = 0;
            // ReSharper disable ExceptionNotDocumented
            read = 0;
            // ReSharper restore ExceptionNotDocumented
            while (read < 11)
            {
                int readByte = stream.ReadByte();
                if (readByte < 0)
                    throw new InternalBufferOverflowException(Resources.VariableLengthEncoding_Decode_Buffer_Overflow);
                read++;
                byte b = (byte)readByte;
                result += (ulong)(b & 0x7F) << shift;
                if ((b & 0x80) != 0x80) break;

                if (read > 9)
                    throw new InternalBufferOverflowException(Resources.VariableLengthEncoding_Decode_Buffer_Overflow);
                shift += 7;
            }
            return result;
        }

        /// <summary>
        /// Decodes the specified value from the <see cref="Stream" /> asynchronously.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The uncompressed value.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.IO.InternalBufferOverflowException">
        /// </exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InternalBufferOverflowException"></exception>
        /// <seealso cref="Encode(ulong)" />
        /// <seealso cref="Encode(ulong,byte[],long)" />
        /// <remarks>As this uses a variable length encoding that encodes it's own length a length is not required.</remarks>
        [PublicAPI]
        public static async Task<ulong> DecodeAsync([NotNull] Stream stream, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            ulong result = 0;
            int shift = 0;
            // ReSharper disable ExceptionNotDocumented
            int read = 0;
            byte[] buffer = new byte[1];
            // ReSharper restore ExceptionNotDocumented
            while (read < 11)
            {
                // ReSharper disable PossibleNullReferenceException
                int readBytes = await stream.ReadAsync(buffer, 0, 1, cancellationToken).ConfigureAwait(false);
                // ReSharper restore PossibleNullReferenceException

                if (readBytes < 1)
                    throw new InternalBufferOverflowException(Resources.VariableLengthEncoding_Decode_Buffer_Overflow);
                read++;
                byte b = buffer[0];
                result += (ulong)(b & 0x7F) << shift;
                if ((b & 0x80) != 0x80) break;

                if (read > 9)
                    throw new InternalBufferOverflowException(Resources.VariableLengthEncoding_Decode_Buffer_Overflow);
                shift += 7;
            }
            return result;
        }
    }
}