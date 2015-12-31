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
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Class for performing variable length encoding.
    /// </summary>
    /// <remarks>
    /// <para>Due to the nature of the encoding, it is not recommended for data types where the size is less than 4
    /// bytes.  The probability of exceeding the original size increases for smaller data types.</para>
    /// <para>Overloads are provided for both signed and unsigned data types, as the signed types are encoded to
    /// prefer values around zero, just as unsigned types are encoded to prefer values closer to zero.  To do this the
    /// negative number space is effectively interleaved with the positive one.</para>
    /// <para>If you know the values are always positive then you should use the unsigned equivalent data type.</para>
    /// </remarks>
    [PublicAPI]
    public static class VariableLengthEncoding
    {
        /// <summary>
        /// The maximum size (in bytes) of an encoded int/uint.
        /// </summary>
        public const int UIntMaxSize = 5;

        /// <summary>
        /// The maximum size (in bytes) of an encoded long/ulong.
        /// </summary>
        private const int ULongMaxSize = 10;

        /// <summary>
        /// The maximum size (in bytes) of an encoded long/ulong.
        /// </summary>
        private const int DecimalMaxSize = 20;

        #region Int overloads
        /// <summary>
        /// Maps an <see cref="int"/> to an <see cref="ulong"/>, optimizing for values close to zero.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>An <see cref="ulong"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong MapInt(int value)
            // WARNING: the cast to long is required for the case of int.MinValue...
            => value < 0 ? ((ulong)-(long)value << 1) - 1 : (ulong)value << 1;

        /// <summary>
        /// Maps an <see cref="ulong"/> to a <see cref="int"/>, optimizing for values close to zero.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>An <see cref="int"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int MapInt(ulong value)
            => (value & 1) < 1 ? (int)(value >> 1) : -1 - (int)(value >> 1);

        /// <summary>
        /// Encodes the specified value to a byte[].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The compressed value.</returns>
        /// <seealso cref="DecodeInt(byte[],long)"/>
        /// <seealso cref="DecodeInt(byte[],ref long)"/>
        /// <remarks>
        /// <para>This compression assumes a weighting towards lower values, an is ideally suited for lengths.  It uses
        /// 7-bits per byte, with the most significant bit indicating that another byte is used.  This means it also
        /// encodes its own length and can act as a variable length header.</para>
        /// <para>Values less than 268,435,456 (1 &lt;&lt; 28) take a maximum of four bytes, and only 1 byte for values
        /// less than 128 (1 &lt;&lt; 7).  As such the compression becomes inefficient above 28 bits, however it has
        /// the distinct benefit of being able to encode all possible long values, requiring up to 
        /// <see cref="UIntMaxSize"/> bytes for the largest values, at a cost of two bytes compared to a long.</para>
        /// </remarks>
        [NotNull]
        [PublicAPI]
        public static byte[] Encode(int value)
        {
            byte[] result = new byte[UIntMaxSize];
            long offset = 0;
            // ReSharper disable once ExceptionNotDocumented
            Encode(value, result, ref offset);
            if (offset < result.Length) Array.Resize(ref result, (int)offset);
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
        /// <seealso cref="DecodeInt(byte[],long)"/>
        /// <seealso cref="DecodeInt(byte[],ref long)"/>
        /// <remarks><para>This compression assumes a weighting towards lower values, an is ideally suited for lengths.  It uses
        /// 7-bits per byte, with the most significant bit indicating that another byte is used.  This means it also
        /// encodes its own length and can act as a variable length header.</para>
        /// <para>Values less than 268,435,456 (1 &lt;&lt; 28) take a maximum of four bytes, and only 1 byte for values
        /// less than 128 (1 &lt;&lt; 7).  As such the compression becomes inefficient above 28 bits, however it has
        /// the distinct benefit of being able to encode all possible long values, requiring up to 
        /// <see cref="UIntMaxSize"/> bytes for the largest values, at a cost of two bytes compared to a long.</para></remarks>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
        /// <exception cref="InternalBufferOverflowException">Could not encode the value as the buffer ran out of space.</exception>
        [PublicAPI]
        public static void Encode(int value, [NotNull] byte[] buffer, long offset = 0)
            => Encode(value, buffer, ref offset);

        /// <summary>
        /// Encodes the specified value to a byte[].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The compressed value.</returns>
        /// <seealso cref="DecodeInt(byte[],long)"/>
        /// <seealso cref="DecodeInt(byte[],ref long)"/>
        /// <remarks><para>This compression assumes a weighting towards lower values, an is ideally suited for lengths.  It uses
        /// 7-bits per byte, with the most significant bit indicating that another byte is used.  This means it also
        /// encodes its own length and can act as a variable length header.</para>
        /// <para>Values less than 268,435,456 (1 &lt;&lt; 28) take a maximum of four bytes, and only 1 byte for values
        /// less than 128 (1 &lt;&lt; 7).  As such the compression becomes inefficient above 28 bits, however it has
        /// the distinct benefit of being able to encode all possible long values, requiring up to 
        /// <see cref="UIntMaxSize"/> bytes for the largest values, at a cost of two bytes compared to a long.</para></remarks>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
        /// <exception cref="InternalBufferOverflowException">Could not encode the value as the buffer ran out of space.</exception>
        [PublicAPI]
        public static void Encode(int value, [NotNull] byte[] buffer, ref long offset)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            ulong v = MapInt(value);
            long l = buffer.LongLength;
            do
            {
                if (offset > l)
                    throw new InternalBufferOverflowException(Resources.VariableLengthEncoding_Encode_Buffer_Overflow);
                byte b = (byte)(v & 0x7F);
                bool end = b == v;
                buffer[offset++] = (byte)(b | (end ? 0 : 0x80));
                if (end) break;
                v >>= 7;
            } while (true);
        }

        /// <summary>
        /// Encodes the specified value to a <see cref="Stream" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>The compressed value.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="stream" /> is <see langword="null" />.</exception>
        /// <seealso cref="DecodeInt(System.IO.Stream,out int)" />
        /// <remarks><para>This compression assumes a weighting towards lower values, an is ideally suited for lengths.  It uses
        /// 7-bits per byte, with the most significant bit indicating that another byte is used.  This means it also
        /// encodes its own length and can act as a variable length header.</para>
        /// <para>Values less than 268,435,456 (1 &lt;&lt; 28) take a maximum of four bytes, and only 1 byte for values
        /// less than 128 (1 &lt;&lt; 7).  As such the compression becomes inefficient above 28 bits, however it has
        /// the distinct benefit of being able to encode all possible long values, requiring up to 
        /// <see cref="UIntMaxSize"/> bytes for the largest values, at a cost of two bytes compared to a long.</para></remarks>
        [PublicAPI]
        public static int Encode(int value, [NotNull] Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            ulong v = MapInt(value);
            int written = 0;
            do
            {
                byte b = (byte)(v & 0x7F);
                bool end = b == v;
                stream.WriteByte((byte)(b | (end ? 0 : 0x80)));
                written++;
                if (end) break;
                v >>= 7;
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
        /// <seealso cref="DecodeIntAsync" />
        /// <remarks><para>This compression assumes a weighting towards lower values, an is ideally suited for lengths.  It uses
        /// 7-bits per byte, with the most significant bit indicating that another byte is used.  This means it also
        /// encodes its own length and can act as a variable length header.</para>
        /// <para>Values less than 268,435,456 (1 &lt;&lt; 28) take a maximum of four bytes, and only 1 byte for values
        /// less than 128 (1 &lt;&lt; 7).  As such the compression becomes inefficient above 28 bits, however it has
        /// the distinct benefit of being able to encode all possible long values, requiring up to 
        /// <see cref="UIntMaxSize"/> bytes for the largest values, at a cost of two bytes compared to a long.</para></remarks>
        [PublicAPI]
        public static async Task<int> EncodeAsync(int value, [NotNull] Stream stream, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            byte[] result = new byte[UIntMaxSize];
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
        /// <seealso cref="Encode(long)"/>
        /// <seealso cref="Encode(long,byte[],long)"/>
        /// <seealso cref="Encode(long,byte[],ref long)"/>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
        /// <remarks><para>As this uses a variable length encoding that encodes it's own length a length is not required.</para></remarks>
        /// <exception cref="InternalBufferOverflowException">The encoding was invalid, ran out of bytes before getting an end of encoding byte.</exception>
        [PublicAPI]
        public static int DecodeInt([NotNull] byte[] buffer, long offset = 0) => DecodeInt(buffer, ref offset);

        /// <summary>
        /// Decodes the specified value from the buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset to start from.</param>
        /// <returns>The uncompressed value.</returns>
        /// <seealso cref="Encode(long)"/>
        /// <seealso cref="Encode(long,byte[],long)"/>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
        /// <remarks><para>As this uses a variable length encoding that encodes it's own length a length is not required.</para></remarks>
        /// <exception cref="InternalBufferOverflowException">The encoding was invalid, ran out of bytes before getting an end of encoding byte.</exception>
        [PublicAPI]
        public static int DecodeInt([NotNull] byte[] buffer, ref long offset)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            ulong result = 0;
            int shift = 0;
            // ReSharper disable ExceptionNotDocumented
            long end = (buffer.LongLength - offset) < UIntMaxSize ? buffer.LongLength : offset + UIntMaxSize;
            // ReSharper restore ExceptionNotDocumented
            while (offset < end)
            {
                byte b = buffer[offset++];
                result += (ulong)(b & 0x7F) << shift;
                if ((b & 0x80) != 0x80) break;
                if (offset >= end)
                    throw new InternalBufferOverflowException(Resources.VariableLengthEncoding_Decode_Buffer_Overflow);
                shift += 7;
            }
            return MapInt(result);
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
        /// <seealso cref="Encode(long)" />
        /// <seealso cref="Encode(long,byte[],long)" />
        /// <remarks>As this uses a variable length encoding that encodes it's own length a length is not required.</remarks>
        [PublicAPI]
        public static int DecodeInt([NotNull] Stream stream, out int read)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            ulong result = 0;
            int shift = 0;
            // ReSharper disable ExceptionNotDocumented
            read = 0;
            // ReSharper restore ExceptionNotDocumented
            while (read < UIntMaxSize)
            {
                int readByte = stream.ReadByte();
                if (readByte < 0)
                    throw new InternalBufferOverflowException(Resources.VariableLengthEncoding_Decode_Buffer_Overflow);
                read++;
                byte b = (byte)readByte;
                result += (ulong)(b & 0x7F) << shift;
                if ((b & 0x80) != 0x80) break;

                if (read >= UIntMaxSize)
                    throw new InternalBufferOverflowException(Resources.VariableLengthEncoding_Decode_Buffer_Overflow);
                shift += 7;
            }
            return MapInt(result);
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
        /// <seealso cref="Encode(long)" />
        /// <seealso cref="Encode(long,byte[],long)" />
        /// <remarks>As this uses a variable length encoding that encodes it's own length a length is not required.</remarks>
        [PublicAPI]
        public static async Task<int> DecodeIntAsync([NotNull] Stream stream, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            ulong result = 0;
            int shift = 0;
            // ReSharper disable ExceptionNotDocumented
            int read = 0;
            byte[] buffer = new byte[1];
            // ReSharper restore ExceptionNotDocumented
            while (read < UIntMaxSize)
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

                if (read >= UIntMaxSize)
                    throw new InternalBufferOverflowException(Resources.VariableLengthEncoding_Decode_Buffer_Overflow);
                shift += 7;
            }
            return MapInt(result);
        }
        #endregion

        #region UInt overloads
        /// <summary>
        /// Encodes the specified value to a byte[].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The compressed value.</returns>
        /// <seealso cref="DecodeUInt(byte[],long)"/>
        /// <seealso cref="DecodeUInt(byte[],ref long)"/>
        /// <remarks>
        /// <para>This compression assumes a weighting towards lower values, an is ideally suited for lengths.  It uses
        /// 7-bits per byte, with the most significant bit indicating that another byte is used.  This means it also
        /// encodes its own length and can act as a variable length header.</para>
        /// <para>Values less than 268,435,456 (1 &lt;&lt; 28) take a maximum of four bytes, and only 1 byte for values
        /// less than 128 (1 &lt;&lt; 7).  As such the compression becomes inefficient above 28 bits, however it has
        /// the distinct benefit of being able to encode all possible uint values, requiring up to 
        /// <see cref="UIntMaxSize"/> bytes for the largest values, at a cost of two bytes compared to a uint.</para>
        /// </remarks>
        [NotNull]
        [PublicAPI]
        public static byte[] Encode(uint value)
        {
            byte[] result = new byte[UIntMaxSize];
            long offset = 0;
            // ReSharper disable once ExceptionNotDocumented
            Encode(value, result, ref offset);
            if (offset < result.Length) Array.Resize(ref result, (int)offset);
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
        /// <seealso cref="DecodeUInt(byte[],long)"/>
        /// <seealso cref="DecodeUInt(byte[],ref long)"/>
        /// <remarks><para>This compression assumes a weighting towards lower values, an is ideally suited for lengths.  It uses
        /// 7-bits per byte, with the most significant bit indicating that another byte is used.  This means it also
        /// encodes its own length and can act as a variable length header.</para>
        /// <para>Values less than 268,435,456 (1 &lt;&lt; 28) take a maximum of four bytes, and only 1 byte for values
        /// less than 128 (1 &lt;&lt; 7).  As such the compression becomes inefficient above 28 bits, however it has
        /// the distinct benefit of being able to encode all possible uint values, requiring up to 
        /// <see cref="UIntMaxSize"/> bytes for the largest values, at a cost of two bytes compared to a uint.</para></remarks>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
        /// <exception cref="InternalBufferOverflowException">Could not encode the value as the buffer ran out of space.</exception>
        [PublicAPI]
        public static void Encode(uint value, [NotNull] byte[] buffer, long offset = 0)
            => Encode(value, buffer, ref offset);

        /// <summary>
        /// Encodes the specified value to a byte[].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The compressed value.</returns>
        /// <seealso cref="DecodeUInt(byte[],long)"/>
        /// <seealso cref="DecodeUInt(byte[],ref long)"/>
        /// <remarks><para>This compression assumes a weighting towards lower values, an is ideally suited for lengths.  It uses
        /// 7-bits per byte, with the most significant bit indicating that another byte is used.  This means it also
        /// encodes its own length and can act as a variable length header.</para>
        /// <para>Values less than 268,435,456 (1 &lt;&lt; 28) take a maximum of four bytes, and only 1 byte for values
        /// less than 128 (1 &lt;&lt; 7).  As such the compression becomes inefficient above 28 bits, however it has
        /// the distinct benefit of being able to encode all possible uint values, requiring up to 
        /// <see cref="UIntMaxSize"/> bytes for the largest values, at a cost of two bytes compared to a uint.</para></remarks>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
        /// <exception cref="InternalBufferOverflowException">Could not encode the value as the buffer ran out of space.</exception>
        [PublicAPI]
        public static void Encode(uint value, [NotNull] byte[] buffer, ref long offset)
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
        /// <seealso cref="DecodeUInt(System.IO.Stream,out int)" />
        /// <remarks><para>This compression assumes a weighting towards lower values, an is ideally suited for lengths.  It uses
        /// 7-bits per byte, with the most significant bit indicating that another byte is used.  This means it also
        /// encodes its own length and can act as a variable length header.</para>
        /// <para>Values less than 268,435,456 (1 &lt;&lt; 28) take a maximum of four bytes, and only 1 byte for values
        /// less than 128 (1 &lt;&lt; 7).  As such the compression becomes inefficient above 28 bits, however it has
        /// the distinct benefit of being able to encode all possible uint values, requiring up to 
        /// <see cref="UIntMaxSize"/> bytes for the largest values, at a cost of two bytes compared to a uint.</para></remarks>
        [PublicAPI]
        public static int Encode(uint value, [NotNull] Stream stream)
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
        /// <seealso cref="DecodeUIntAsync" />
        /// <remarks><para>This compression assumes a weighting towards lower values, an is ideally suited for lengths.  It uses
        /// 7-bits per byte, with the most significant bit indicating that another byte is used.  This means it also
        /// encodes its own length and can act as a variable length header.</para>
        /// <para>Values less than 268,435,456 (1 &lt;&lt; 28) take a maximum of four bytes, and only 1 byte for values
        /// less than 128 (1 &lt;&lt; 7).  As such the compression becomes inefficient above 28 bits, however it has
        /// the distinct benefit of being able to encode all possible uint values, requiring up to 
        /// <see cref="UIntMaxSize"/> bytes for the largest values, at a cost of two bytes compared to a uint.</para></remarks>
        [PublicAPI]
        public static async Task<int> EncodeAsync(uint value, [NotNull] Stream stream, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            byte[] result = new byte[UIntMaxSize];
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
        /// <seealso cref="Encode(uint)"/>
        /// <seealso cref="Encode(uint,byte[],long)"/>
        /// <seealso cref="Encode(uint,byte[],ref long)"/>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
        /// <remarks><para>As this uses a variable length encoding that encodes it's own length a length is not required.</para></remarks>
        /// <exception cref="InternalBufferOverflowException">The encoding was invalid, ran out of bytes before getting an end of encoding byte.</exception>
        [PublicAPI]
        public static uint DecodeUInt([NotNull] byte[] buffer, long offset = 0) => DecodeUInt(buffer, ref offset);

        /// <summary>
        /// Decodes the specified value from the buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset to start from.</param>
        /// <returns>The uncompressed value.</returns>
        /// <seealso cref="Encode(uint)"/>
        /// <seealso cref="Encode(uint,byte[],long)"/>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
        /// <remarks><para>As this uses a variable length encoding that encodes it's own length a length is not required.</para></remarks>
        /// <exception cref="InternalBufferOverflowException">The encoding was invalid, ran out of bytes before getting an end of encoding byte.</exception>
        [PublicAPI]
        public static uint DecodeUInt([NotNull] byte[] buffer, ref long offset)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            uint result = 0;
            int shift = 0;
            // ReSharper disable ExceptionNotDocumented
            long end = (buffer.LongLength - offset) < UIntMaxSize ? buffer.LongLength : offset + UIntMaxSize;
            // ReSharper restore ExceptionNotDocumented
            while (offset < end)
            {
                byte b = buffer[offset++];
                result += (uint)(b & 0x7F) << shift;
                if ((b & 0x80) != 0x80) break;
                if (offset >= end)
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
        /// <seealso cref="Encode(uint)" />
        /// <seealso cref="Encode(uint,byte[],long)" />
        /// <remarks>As this uses a variable length encoding that encodes it's own length a length is not required.</remarks>
        [PublicAPI]
        public static uint DecodeUInt([NotNull] Stream stream, out int read)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            uint result = 0;
            int shift = 0;
            // ReSharper disable ExceptionNotDocumented
            read = 0;
            // ReSharper restore ExceptionNotDocumented
            while (read < UIntMaxSize)
            {
                int readByte = stream.ReadByte();
                if (readByte < 0)
                    throw new InternalBufferOverflowException(Resources.VariableLengthEncoding_Decode_Buffer_Overflow);
                read++;
                byte b = (byte)readByte;
                result += (uint)(b & 0x7F) << shift;
                if ((b & 0x80) != 0x80) break;

                if (read >= UIntMaxSize)
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
        /// <seealso cref="Encode(uint)" />
        /// <seealso cref="Encode(uint,byte[],long)" />
        /// <remarks>As this uses a variable length encoding that encodes it's own length a length is not required.</remarks>
        [PublicAPI]
        public static async Task<uint> DecodeUIntAsync([NotNull] Stream stream, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            uint result = 0;
            int shift = 0;
            // ReSharper disable ExceptionNotDocumented
            int read = 0;
            byte[] buffer = new byte[1];
            // ReSharper restore ExceptionNotDocumented
            while (read < UIntMaxSize)
            {
                // ReSharper disable PossibleNullReferenceException
                int readBytes = await stream.ReadAsync(buffer, 0, 1, cancellationToken).ConfigureAwait(false);
                // ReSharper restore PossibleNullReferenceException

                if (readBytes < 1)
                    throw new InternalBufferOverflowException(Resources.VariableLengthEncoding_Decode_Buffer_Overflow);
                read++;
                byte b = buffer[0];
                result += (uint)(b & 0x7F) << shift;
                if ((b & 0x80) != 0x80) break;

                if (read >= UIntMaxSize)
                    throw new InternalBufferOverflowException(Resources.VariableLengthEncoding_Decode_Buffer_Overflow);
                shift += 7;
            }
            return result;
        }
        #endregion

        #region Long overloads
        /// <summary>
        /// Maps a <see cref="long"/> to an <see cref="ulong"/>, optimizing for values close to zero.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>An <see cref="ulong"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong MapLong(long value)
            => value == long.MinValue
                ? ulong.MaxValue
                : value < 0 ? ((ulong)-value << 1) - 1 : (ulong)value << 1;

        /// <summary>
        /// Maps an <see cref="ulong"/> to a <see cref="long"/>, optimizing for values close to zero.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="long"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long MapLong(ulong value)
            => value == ulong.MaxValue
                ? long.MinValue
                : (value & 1) < 1 ? (long)(value >> 1) : -1 - (long)(value >> 1);

        /// <summary>
        /// Encodes the specified value to a byte[].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The compressed value.</returns>
        /// <seealso cref="DecodeLong(byte[],long)"/>
        /// <seealso cref="DecodeLong(byte[],ref long)"/>
        /// <remarks>
        /// <para>This compression assumes a weighting towards lower values, an is ideally suited for lengths.  It uses
        /// 7-bits per byte, with the most significant bit indicating that another byte is used.  This means it also
        /// encodes its own length and can act as a variable length header.</para>
        /// <para>Values less than 268,435,456 (1 &lt;&lt; 28) take a maximum of four bytes, and only 1 byte for values
        /// less than 128 (1 &lt;&lt; 7).  As such the compression becomes inefficient above 28 bits, however it has
        /// the distinct benefit of being able to encode all possible long values, requiring up to 
        /// <see cref="ULongMaxSize"/> bytes for the largest values, at a cost of two bytes compared to a long.</para>
        /// </remarks>
        [NotNull]
        [PublicAPI]
        public static byte[] Encode(long value)
        {
            byte[] result = new byte[ULongMaxSize];
            long offset = 0;
            // ReSharper disable once ExceptionNotDocumented
            Encode(value, result, ref offset);
            if (offset < result.Length) Array.Resize(ref result, (int)offset);
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
        /// <seealso cref="DecodeLong(byte[],long)"/>
        /// <seealso cref="DecodeLong(byte[],ref long)"/>
        /// <remarks><para>This compression assumes a weighting towards lower values, an is ideally suited for lengths.  It uses
        /// 7-bits per byte, with the most significant bit indicating that another byte is used.  This means it also
        /// encodes its own length and can act as a variable length header.</para>
        /// <para>Values less than 268,435,456 (1 &lt;&lt; 28) take a maximum of four bytes, and only 1 byte for values
        /// less than 128 (1 &lt;&lt; 7).  As such the compression becomes inefficient above 28 bits, however it has
        /// the distinct benefit of being able to encode all possible long values, requiring up to 
        /// <see cref="ULongMaxSize"/> bytes for the largest values, at a cost of two bytes compared to a long.</para></remarks>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
        /// <exception cref="InternalBufferOverflowException">Could not encode the value as the buffer ran out of space.</exception>
        [PublicAPI]
        public static void Encode(long value, [NotNull] byte[] buffer, long offset = 0)
            => Encode(value, buffer, ref offset);

        /// <summary>
        /// Encodes the specified value to a byte[].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The compressed value.</returns>
        /// <seealso cref="DecodeLong(byte[],long)"/>
        /// <seealso cref="DecodeLong(byte[],ref long)"/>
        /// <remarks><para>This compression assumes a weighting towards lower values, an is ideally suited for lengths.  It uses
        /// 7-bits per byte, with the most significant bit indicating that another byte is used.  This means it also
        /// encodes its own length and can act as a variable length header.</para>
        /// <para>Values less than 268,435,456 (1 &lt;&lt; 28) take a maximum of four bytes, and only 1 byte for values
        /// less than 128 (1 &lt;&lt; 7).  As such the compression becomes inefficient above 28 bits, however it has
        /// the distinct benefit of being able to encode all possible long values, requiring up to 
        /// <see cref="ULongMaxSize"/> bytes for the largest values, at a cost of two bytes compared to a long.</para></remarks>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
        /// <exception cref="InternalBufferOverflowException">Could not encode the value as the buffer ran out of space.</exception>
        [PublicAPI]
        public static void Encode(long value, [NotNull] byte[] buffer, ref long offset)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            ulong v = MapLong(value);
            long l = buffer.LongLength;
            do
            {
                if (offset > l)
                    throw new InternalBufferOverflowException(Resources.VariableLengthEncoding_Encode_Buffer_Overflow);
                byte b = (byte)(v & 0x7F);
                bool end = b == v;
                buffer[offset++] = (byte)(b | (end ? 0 : 0x80));
                if (end) break;
                v >>= 7;
            } while (true);
        }

        /// <summary>
        /// Encodes the specified value to a <see cref="Stream" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>The compressed value.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="stream" /> is <see langword="null" />.</exception>
        /// <seealso cref="DecodeLong(System.IO.Stream,out int)" />
        /// <remarks><para>This compression assumes a weighting towards lower values, an is ideally suited for lengths.  It uses
        /// 7-bits per byte, with the most significant bit indicating that another byte is used.  This means it also
        /// encodes its own length and can act as a variable length header.</para>
        /// <para>Values less than 268,435,456 (1 &lt;&lt; 28) take a maximum of four bytes, and only 1 byte for values
        /// less than 128 (1 &lt;&lt; 7).  As such the compression becomes inefficient above 28 bits, however it has
        /// the distinct benefit of being able to encode all possible long values, requiring up to 
        /// <see cref="ULongMaxSize"/> bytes for the largest values, at a cost of two bytes compared to a long.</para></remarks>
        [PublicAPI]
        public static int Encode(long value, [NotNull] Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            ulong v = MapLong(value);
            int written = 0;
            do
            {
                byte b = (byte)(v & 0x7F);
                bool end = b == v;
                stream.WriteByte((byte)(b | (end ? 0 : 0x80)));
                written++;
                if (end) break;
                v >>= 7;
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
        /// <seealso cref="DecodeLongAsync" />
        /// <remarks><para>This compression assumes a weighting towards lower values, an is ideally suited for lengths.  It uses
        /// 7-bits per byte, with the most significant bit indicating that another byte is used.  This means it also
        /// encodes its own length and can act as a variable length header.</para>
        /// <para>Values less than 268,435,456 (1 &lt;&lt; 28) take a maximum of four bytes, and only 1 byte for values
        /// less than 128 (1 &lt;&lt; 7).  As such the compression becomes inefficient above 28 bits, however it has
        /// the distinct benefit of being able to encode all possible long values, requiring up to 
        /// <see cref="ULongMaxSize"/> bytes for the largest values, at a cost of two bytes compared to a long.</para></remarks>
        [PublicAPI]
        public static async Task<int> EncodeAsync(long value, [NotNull] Stream stream, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            byte[] result = new byte[ULongMaxSize];
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
        /// <seealso cref="Encode(long)"/>
        /// <seealso cref="Encode(long,byte[],long)"/>
        /// <seealso cref="Encode(long,byte[],ref long)"/>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
        /// <remarks><para>As this uses a variable length encoding that encodes it's own length a length is not required.</para></remarks>
        /// <exception cref="InternalBufferOverflowException">The encoding was invalid, ran out of bytes before getting an end of encoding byte.</exception>
        [PublicAPI]
        public static long DecodeLong([NotNull] byte[] buffer, long offset = 0) => DecodeLong(buffer, ref offset);

        /// <summary>
        /// Decodes the specified value from the buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset to start from.</param>
        /// <returns>The uncompressed value.</returns>
        /// <seealso cref="Encode(long)"/>
        /// <seealso cref="Encode(long,byte[],long)"/>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
        /// <remarks><para>As this uses a variable length encoding that encodes it's own length a length is not required.</para></remarks>
        /// <exception cref="InternalBufferOverflowException">The encoding was invalid, ran out of bytes before getting an end of encoding byte.</exception>
        [PublicAPI]
        public static long DecodeLong([NotNull] byte[] buffer, ref long offset)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            ulong result = 0;
            int shift = 0;
            // ReSharper disable ExceptionNotDocumented
            long end = (buffer.LongLength - offset) < ULongMaxSize ? buffer.LongLength : offset + ULongMaxSize;
            // ReSharper restore ExceptionNotDocumented
            while (offset < end)
            {
                byte b = buffer[offset++];
                result += (ulong)(b & 0x7F) << shift;
                if ((b & 0x80) != 0x80) break;
                if (offset >= end)
                    throw new InternalBufferOverflowException(Resources.VariableLengthEncoding_Decode_Buffer_Overflow);
                shift += 7;
            }
            return MapLong(result);
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
        /// <seealso cref="Encode(long)" />
        /// <seealso cref="Encode(long,byte[],long)" />
        /// <remarks>As this uses a variable length encoding that encodes it's own length a length is not required.</remarks>
        [PublicAPI]
        public static long DecodeLong([NotNull] Stream stream, out int read)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            ulong result = 0;
            int shift = 0;
            // ReSharper disable ExceptionNotDocumented
            read = 0;
            // ReSharper restore ExceptionNotDocumented
            while (read < ULongMaxSize)
            {
                int readByte = stream.ReadByte();
                if (readByte < 0)
                    throw new InternalBufferOverflowException(Resources.VariableLengthEncoding_Decode_Buffer_Overflow);
                read++;
                byte b = (byte)readByte;
                result += (ulong)(b & 0x7F) << shift;
                if ((b & 0x80) != 0x80) break;

                if (read >= ULongMaxSize)
                    throw new InternalBufferOverflowException(Resources.VariableLengthEncoding_Decode_Buffer_Overflow);
                shift += 7;
            }
            return MapLong(result);
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
        /// <seealso cref="Encode(long)" />
        /// <seealso cref="Encode(long,byte[],long)" />
        /// <remarks>As this uses a variable length encoding that encodes it's own length a length is not required.</remarks>
        [PublicAPI]
        public static async Task<long> DecodeLongAsync([NotNull] Stream stream, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            ulong result = 0;
            int shift = 0;
            // ReSharper disable ExceptionNotDocumented
            int read = 0;
            byte[] buffer = new byte[1];
            // ReSharper restore ExceptionNotDocumented
            while (read < ULongMaxSize)
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

                if (read >= ULongMaxSize)
                    throw new InternalBufferOverflowException(Resources.VariableLengthEncoding_Decode_Buffer_Overflow);
                shift += 7;
            }
            return MapLong(result);
        }
        #endregion

        #region ULong overloads
        /// <summary>
        /// Encodes the specified value to a byte[].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The compressed value.</returns>
        /// <seealso cref="DecodeULong(byte[],long)"/>
        /// <seealso cref="DecodeULong(byte[],ref long)"/>
        /// <remarks>
        /// <para>This compression assumes a weighting towards lower values, an is ideally suited for lengths.  It uses
        /// 7-bits per byte, with the most significant bit indicating that another byte is used.  This means it also
        /// encodes its own length and can act as a variable length header.</para>
        /// <para>Values less than 268,435,456 (1 &lt;&lt; 28) take a maximum of four bytes, and only 1 byte for values
        /// less than 128 (1 &lt;&lt; 7).  As such the compression becomes inefficient above 28 bits, however it has
        /// the distinct benefit of being able to encode all possible ulong values, requiring up to 
        /// <see cref="ULongMaxSize"/> bytes for the largest values, at a cost of two bytes compared to a ulong.</para>
        /// </remarks>
        [NotNull]
        [PublicAPI]
        public static byte[] Encode(ulong value)
        {
            byte[] result = new byte[ULongMaxSize];
            long offset = 0;
            // ReSharper disable once ExceptionNotDocumented
            Encode(value, result, ref offset);
            if (offset < result.Length) Array.Resize(ref result, (int)offset);
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
        /// <seealso cref="DecodeULong(byte[],long)"/>
        /// <seealso cref="DecodeULong(byte[],ref long)"/>
        /// <remarks><para>This compression assumes a weighting towards lower values, an is ideally suited for lengths.  It uses
        /// 7-bits per byte, with the most significant bit indicating that another byte is used.  This means it also
        /// encodes its own length and can act as a variable length header.</para>
        /// <para>Values less than 268,435,456 (1 &lt;&lt; 28) take a maximum of four bytes, and only 1 byte for values
        /// less than 128 (1 &lt;&lt; 7).  As such the compression becomes inefficient above 28 bits, however it has
        /// the distinct benefit of being able to encode all possible ulong values, requiring up to 
        /// <see cref="ULongMaxSize"/> bytes for the largest values, at a cost of two bytes compared to a ulong.</para></remarks>
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
        /// <seealso cref="DecodeULong(byte[],long)"/>
        /// <seealso cref="DecodeULong(byte[],ref long)"/>
        /// <remarks><para>This compression assumes a weighting towards lower values, an is ideally suited for lengths.  It uses
        /// 7-bits per byte, with the most significant bit indicating that another byte is used.  This means it also
        /// encodes its own length and can act as a variable length header.</para>
        /// <para>Values less than 268,435,456 (1 &lt;&lt; 28) take a maximum of four bytes, and only 1 byte for values
        /// less than 128 (1 &lt;&lt; 7).  As such the compression becomes inefficient above 28 bits, however it has
        /// the distinct benefit of being able to encode all possible ulong values, requiring up to 
        /// <see cref="ULongMaxSize"/> bytes for the largest values, at a cost of two bytes compared to a ulong.</para></remarks>
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
        /// <seealso cref="DecodeULong(System.IO.Stream,out int)" />
        /// <remarks><para>This compression assumes a weighting towards lower values, an is ideally suited for lengths.  It uses
        /// 7-bits per byte, with the most significant bit indicating that another byte is used.  This means it also
        /// encodes its own length and can act as a variable length header.</para>
        /// <para>Values less than 268,435,456 (1 &lt;&lt; 28) take a maximum of four bytes, and only 1 byte for values
        /// less than 128 (1 &lt;&lt; 7).  As such the compression becomes inefficient above 28 bits, however it has
        /// the distinct benefit of being able to encode all possible ulong values, requiring up to 
        /// <see cref="ULongMaxSize"/> bytes for the largest values, at a cost of two bytes compared to a ulong.</para></remarks>
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
        /// <seealso cref="DecodeULongAsync" />
        /// <remarks><para>This compression assumes a weighting towards lower values, an is ideally suited for lengths.  It uses
        /// 7-bits per byte, with the most significant bit indicating that another byte is used.  This means it also
        /// encodes its own length and can act as a variable length header.</para>
        /// <para>Values less than 268,435,456 (1 &lt;&lt; 28) take a maximum of four bytes, and only 1 byte for values
        /// less than 128 (1 &lt;&lt; 7).  As such the compression becomes inefficient above 28 bits, however it has
        /// the distinct benefit of being able to encode all possible ulong values, requiring up to 
        /// <see cref="ULongMaxSize"/> bytes for the largest values, at a cost of two bytes compared to a ulong.</para></remarks>
        [PublicAPI]
        public static async Task<int> EncodeAsync(ulong value, [NotNull] Stream stream, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            byte[] result = new byte[ULongMaxSize];
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
        public static ulong DecodeULong([NotNull] byte[] buffer, long offset = 0) => DecodeULong(buffer, ref offset);

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
        public static ulong DecodeULong([NotNull] byte[] buffer, ref long offset)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            ulong result = 0;
            int shift = 0;
            // ReSharper disable ExceptionNotDocumented
            long end = (buffer.LongLength - offset) < ULongMaxSize ? buffer.LongLength : offset + ULongMaxSize;
            // ReSharper restore ExceptionNotDocumented
            while (offset < end)
            {
                byte b = buffer[offset++];
                result += (ulong)(b & 0x7F) << shift;
                if ((b & 0x80) != 0x80) break;
                if (offset >= end)
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
        public static ulong DecodeULong([NotNull] Stream stream, out int read)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            ulong result = 0;
            int shift = 0;
            // ReSharper disable ExceptionNotDocumented
            read = 0;
            // ReSharper restore ExceptionNotDocumented
            while (read < ULongMaxSize)
            {
                int readByte = stream.ReadByte();
                if (readByte < 0)
                    throw new InternalBufferOverflowException(Resources.VariableLengthEncoding_Decode_Buffer_Overflow);
                read++;
                byte b = (byte)readByte;
                result += (ulong)(b & 0x7F) << shift;
                if ((b & 0x80) != 0x80) break;

                if (read >= ULongMaxSize)
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
        public static async Task<ulong> DecodeULongAsync([NotNull] Stream stream, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            ulong result = 0;
            int shift = 0;
            // ReSharper disable ExceptionNotDocumented
            int read = 0;
            byte[] buffer = new byte[1];
            // ReSharper restore ExceptionNotDocumented
            while (read < ULongMaxSize)
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

                if (read >= ULongMaxSize)
                    throw new InternalBufferOverflowException(Resources.VariableLengthEncoding_Decode_Buffer_Overflow);
                shift += 7;
            }
            return result;
        }
        #endregion
    }
}