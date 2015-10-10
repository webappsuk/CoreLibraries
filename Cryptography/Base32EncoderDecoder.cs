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
using System.Linq;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Cryptography
{
    /// <summary>
    /// Extension methods supporting clean Base32 encoding, with custom digits (skips easily confused characters).
    /// </summary>
    /// <remarks></remarks>
    [PublicAPI]
    public static class Base32EncoderDecoder
    {
        /// <summary>
        /// The digits for a base 32 number system.
        /// </summary>
        [NotNull]
        private static readonly char[] _digits = "123456789ABCDEFGHJKMNPQRSTUVWXYZ".ToCharArray();

        /// <summary>
        /// Gets the default digits.
        /// </summary>
        /// <remarks></remarks>
        [NotNull]
        public static IEnumerable<char> DefaultDigits => _digits;

        /// <summary>
        /// Encodes a Guid into a Base32 string.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <param name="digits">The digits.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <exception cref="OverflowException">The buffer is multidimensional and contains more than <see cref="F:System.UInt32.MaxValue" /> elements.</exception>
        public static string Base32Encode(Guid guid, IEnumerable<char> digits = null)
        {
            return Base32Encode(guid.ToByteArray(), digits);
        }

        /// <summary>
        /// Encodes a byte[] into a Base32 string.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="digits">The digits.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <exception cref="OverflowException">The buffer is multidimensional and contains more than <see cref="F:System.UInt32.MaxValue" /> elements.</exception>
        public static string Base32Encode(byte[] bytes, IEnumerable<char> digits = null)
        {
            char[] digitsArray = digits?.ToArray() ?? _digits;

            if (bytes == null)
                return digitsArray[0].ToString();

            uint inputLength = (uint)bytes.Length;
            uint lastBit = (inputLength * 8) - 1;

            if (inputLength < 1)
                return digitsArray[0].ToString();

            // Pad bytes by 8 bytes
            byte[] buffer = new byte[inputLength + 9];
            Buffer.BlockCopy(bytes, 0, buffer, 0, (int)inputLength);
            bytes = buffer;

            // Keeps track of the bit currently in position 0 of our 64 bit data cache
            uint currentBit = 0;
            // Keeps track of the next bit currently not in the cache
            uint nextBit = 0;
            // Keeps track of the current character being output
            uint charIndex = 0;

            // Holds 64 bits at a time.
            UInt64 data = 0;

            // Calculate the output length
            // Note log(256)/log(32) is 1.6 - we are converting from base 256 (a byte) to base 32 - also 8/5.
            uint outputLength = (uint)Math.Ceiling(1.6 * inputLength);

            // Create the output character cache
            char[] output = new char[outputLength];

            // Get the first 16 bytes into 
            while (currentBit <= lastBit)
            {
                if (currentBit + 11 > nextBit)
                {
                    // Calculate the next byte
                    uint nextByte = nextBit >> 3;
                    // Create a 64 bit number for quick processing
                    UInt64 nextBlock = BitConverter.ToUInt64(bytes, (int)nextByte);

                    // Calculate bit offset
                    uint rem = (nextByte << 3) - currentBit;
                    data |= nextBlock << (int)rem;
                    nextBit = (nextByte * 8) + 64 - rem;
                }

                // Set digit for least significant 5 bits
                output[charIndex++] = digitsArray[data & 31];

                // Shift data cache left 5 bits
                data >>= 5;
                currentBit += 5;
            }

            // Return the character array as a string
            return new string(output);
        }

        /// <summary>
        /// Decodes a base 32 string in a Guid.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="guid">The GUID.</param>
        /// <param name="digits">The digits.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <exception cref="ArgumentNullException">buffer is <see langword="null" />.</exception>
        /// <exception cref="OverflowException">The buffer is multidimensional and contains more than <see cref="F:System.Int32.MaxValue" /> elements.</exception>
        public static bool TryBase32DecodeGuid(string number, out Guid guid, IEnumerable<char> digits = null)
        {
            byte[] bytes = new byte[16];
            if (!TryBase32Decode(number, bytes, digits))
            {
                guid = Guid.Empty;
                return false;
            }
            guid = new Guid(bytes);
            return true;
        }

        /// <summary>
        /// Decodes a base 32 string into a byte[] starting at index 0.  Fails if the buffer is too small, or the string contains invalid characters.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="digits">The digits.</param>
        /// <returns><see langword="true"/> if decode succeeded; otherwise <see langword="false"/>.</returns>
        /// <remarks></remarks>
        /// <exception cref="ArgumentNullException">buffer is <see langword="null" />.</exception>
        /// <exception cref="OverflowException">The buffer is multidimensional and contains more than <see cref="F:System.Int32.MaxValue" /> elements.</exception>
        public static bool TryBase32Decode(string number, [NotNull] byte[] buffer, IEnumerable<char> digits = null)
        {
            return TryBase32Decode(number, buffer, 0, digits);
        }

        /// <summary>
        /// Decodes a base 32 string into a byte[].  Fails if the buffer is too small, or the string contains invalid characters.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="digits">The digits.</param>
        /// <returns><see langword="true"/> if decode succeeded; otherwise <see langword="false"/>.</returns>
        /// <remarks></remarks>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null" />.</exception>
        /// <exception cref="OverflowException">The buffer is multidimensional and contains more than <see cref="F:System.Int32.MaxValue" /> elements.</exception>
        public static bool TryBase32Decode(
            string number,
            [NotNull] byte[] buffer,
            int startIndex,
            IEnumerable<char> digits = null)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));

            char[] digitsArray = digits?.ToArray() ?? _digits;

            if (string.IsNullOrEmpty(number))
                return true;

            int inputLength = number.Length;
            int bufferLength = buffer.Length;
            // Calculate the output length
            // Note log(256)/log(32) is 1.6 - we are converting from base 256 (a byte) to base 32 - also 8/5.
            int outputLength = (int)Math.Ceiling(inputLength / 1.6);

            // If our buffer is smaller than the output length.
            // Note we allow the buffer to be one byte smaller than the potential decoded length, due to bit
            // alignment, however, the last byte must be decoded to zero.
            if ((startIndex + bufferLength) < (outputLength - 1))
                return false;

            byte[] bytes = new byte[outputLength];
            char[] characters = number.ToUpper().ToCharArray();
            int charIndex = 0;
            while (charIndex < inputLength)
            {
                int index = Array.IndexOf(digitsArray, characters[charIndex]);
                if (index < 0)
                    return false;
                uint a = (uint)index;

                int bit = charIndex * 5;
                int mb = bit % 8;
                int byt = bit >> 3;
                bytes[byt] |= (byte)(a << mb);
                if (mb > 3)
                    bytes[byt + 1] |= (byte)(a >> (8 - mb));
                charIndex++;
            }

            if (((startIndex + bufferLength) < outputLength) &&
                (bytes[outputLength - 1] != 0))
                return false;

            // Copy result into buffer
            Buffer.BlockCopy(bytes, 0, buffer, startIndex, bufferLength - startIndex);

            return true;
        }
    }
}