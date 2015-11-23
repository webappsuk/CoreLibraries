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
using System.Security.Cryptography;
using System.Text;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Cryptography
{
    /// <summary>
    /// Extension methods supporting clean Base32 encoding, with custom digits (skips easily confused characters).
    /// </summary>
    [PublicAPI]
    public static class Base32EncoderDecoder
    {
        /// <summary>
        /// Size of the regular byte in bits
        /// </summary>
        private const int InByteSize = 8;

        /// <summary>
        /// Size of converted byte in bits
        /// </summary>
        private const int OutByteSize = 5;

        /// <summary>
        /// The digits for a base 32 number system.
        /// </summary>
        [NotNull]
        public static readonly IReadOnlyList<char> DefaultDigits = "123456789ABCDEFGHJKMNPQRSTUVWXYZ".ToCharArray();

        /// <summary>
        /// Encodes a Guid into a Base32 string.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <param name="digits">The digits.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <exception cref="OverflowException">The buffer is multidimensional and contains more than <see cref="F:System.UInt32.MaxValue" /> elements.</exception>
        [NotNull]
        public static string Base32Encode(this Guid guid, [CanBeNull] IReadOnlyList<char> digits = null)
            => Base32Encode(guid.ToByteArray(), digits);

        /// <summary>
        /// Encodes a Guid into a Base32 string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="digits">The digits.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="OverflowException">The buffer is multidimensional and contains more than <see cref="F:System.UInt32.MaxValue" /> elements.</exception>
        [ContractAnnotation("input:notnull=>notnull;input:null=>null")]
        public static string Base32Encode([CanBeNull] this string input, [CanBeNull] IReadOnlyList<char> digits = null) 
            => Base32Encode(input != null ? Encoding.Unicode.GetBytes(input) : null, digits);


        /// <summary>
        /// Encodes a byte[] into a Base32 string.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="digits">The digits.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <exception cref="OverflowException">The buffer is multidimensional and contains more than <see cref="F:System.UInt32.MaxValue" /> elements.</exception>
        [ContractAnnotation("bytes:notnull=>notnull;bytes:null=>null")]
        public static string Base32Encode([CanBeNull] this byte[] bytes, [CanBeNull] IReadOnlyList<char> digits = null)
        {
            if (bytes == null)
                return null;

            uint inputLength = (uint)bytes.Length;

            if (inputLength < 1)
                return string.Empty;

            if (digits == null) digits = DefaultDigits;
            else if (digits.Count < 32)
                throw new ArgumentException(nameof(digits), Resources.Base32EncoderDecoder_Base32Encode_Invalid_Digits);

            // Calculate the output length
            uint outputLength = (uint) Math.Ceiling((double)inputLength * InByteSize / OutByteSize);

            // Create the output character cache
            char[] output = new char[outputLength];
            
            // Position in the input buffer
            int p = 0;

            // Offset inside a single byte that <bytesPosition> points to (from left to right)
            // 0 - highest bit, 7 - lowest bit
            int sp = 0;

            // Byte to look up in the dictionary
            byte o = 0;

            // The number of bits filled in the current output byte
            int op = 0;

            // The current character position.
            int c = 0;

            // Iterate through input buffer until we reach past the end of it
            while (p < bytes.Length)
            {
                // Calculate the number of bits we can extract out of current input byte to fill missing bits in the output byte
                int bitsAvailableInByte = Math.Min(InByteSize - sp, OutByteSize - op);

                // Make space in the output byte
                o <<= bitsAvailableInByte;

                // Extract the part of the input byte and move it to the output byte
                o |= (byte)(bytes[p] >> (InByteSize - (sp + bitsAvailableInByte)));

                // Update current sub-byte position
                sp += bitsAvailableInByte;

                // Check overflow
                if (sp >= InByteSize)
                {
                    // Move to the next byte
                    p++;
                    sp = 0;
                }

                // Update current base32 byte completion
                op += bitsAvailableInByte;

                // Check overflow or end of input array
                if (op < OutByteSize) continue;

                // Drop the overflow bits
                o &= 0x1F;  // 0x1F = 00011111 in binary

                // Add current Base32 byte and convert it to character
                output[c++] = digits[o];

                // Move to the next byte
                op = 0;
            }

            // Check if we have a remainder
            if (op < 1) return new string(output);

            // Move to the right bits
            o <<= (OutByteSize - op);

            // Drop the overflow bits
            o &= 0x1F;  // 0x1F = 00011111 in binary

            // Add current Base32 byte and convert it to character
            output[c] = digits[o];

            return new string(output);
        }

        /// <summary>
        /// Tries to convert base32 string to a GUID.
        /// </summary>
        /// <param name="base32String">Base32 string to convert.</param>
        /// <param name="digits">The digits.</param>
        /// <param name="comparer">The character comparer (defaults to <see cref="CharComparer.Ordinal"/>).</param>
        /// <returns>The decoded GUID.</returns>
        public static Guid Base32DecodeGuid(
            [NotNull] this string base32String,
            [CanBeNull] IReadOnlyList<char> digits = null,
            [CanBeNull] IEqualityComparer<char> comparer = null)
        {
            if (base32String == null || base32String.Length != 26)
                throw new CryptographicException(Resources.Base32EncoderDecoder_Base32DecodeGuid_Invalid);
            Guid guid;
            if (!TryBase32DecodeGuid(base32String, out guid, digits, comparer))
                throw new CryptographicException(Resources.Base32EncoderDecoder_Base32DecodeGuid_Invalid);
            return guid;
        }

        /// <summary>
        /// Tries to convert base32 string to a string.
        /// </summary>
        /// <param name="base32String">Base32 string to convert.</param>
        /// <param name="digits">The digits.</param>
        /// <param name="comparer">The character comparer (defaults to <see cref="CharComparer.Ordinal"/>).</param>
        /// <returns>The decoded string.</returns>
        public static string Base32DecodeString(
            [CanBeNull] this string base32String,
            [CanBeNull] IReadOnlyList<char> digits = null,
            [CanBeNull] IEqualityComparer<char> comparer = null)
        {
            string output;
            if (!TryBase32DecodeString(base32String, out output, digits, comparer))
                throw new CryptographicException(Resources.Base32EncoderDecoder_Base32DecodeString_Invalid);
            return output;
        }

        /// <summary>
        /// Tries to convert base32 string to a byte array.
        /// </summary>
        /// <param name="base32String">Base32 string to convert.</param>
        /// <param name="digits">The digits.</param>
        /// <param name="comparer">The character comparer (defaults to <see cref="CharComparer.Ordinal"/>).</param>
        /// <returns>The decoded byte array.</returns>
        public static byte[] Base32Decode(
            [CanBeNull] this string base32String,
            [CanBeNull] IReadOnlyList<char> digits = null,
            [CanBeNull] IEqualityComparer<char> comparer = null)
        {
            byte[] output;
            if (!TryBase32Decode(base32String, out output, digits, comparer))
                throw new CryptographicException(Resources.Base32EncoderDecoder_Base32Decode_Invalid);
            return output;
        }

        /// <summary>
        /// Tries to convert base32 string to a GUID.
        /// </summary>
        /// <param name="base32String">Base32 string to convert.</param>
        /// <param name="output">The output.</param>
        /// <param name="digits">The digits.</param>
        /// <param name="comparer">The character comparer (defaults to <see cref="CharComparer.Ordinal"/>).</param>
        /// <returns><see langword="true"/> if the string was successfully decoded; otherwise <see langword="false"/>.</returns>
        public static bool TryBase32DecodeGuid(
            [NotNull] this string base32String,
            out Guid output,
            [CanBeNull] IReadOnlyList<char> digits = null,
            [CanBeNull] IEqualityComparer<char> comparer = null)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (base32String == null || base32String.Length != 26)
            {
                output = Guid.Empty;
                return false;
            }

            byte[] outputBytes = new byte[16];
            if (!TryBase32Decode(base32String, outputBytes, 0, 16, digits, comparer))
            {
                output = Guid.Empty;
                return false;
            }

            output = new Guid(outputBytes);
            return true;
        }

        /// <summary>
        /// Tries to convert base32 string to string.
        /// </summary>
        /// <param name="base32String">Base32 string to convert.</param>
        /// <param name="output">The output.</param>
        /// <param name="digits">The digits.</param>
        /// <param name="comparer">The character comparer (defaults to <see cref="CharComparer.Ordinal"/>).</param>
        /// <returns><see langword="true"/> if the string was successfully decoded; otherwise <see langword="false"/>.</returns>
        public static bool TryBase32DecodeString(
            [CanBeNull] this string base32String,
            out string output,
            [CanBeNull] IReadOnlyList<char> digits = null,
            [CanBeNull] IEqualityComparer<char> comparer = null)
        {
            byte[] outputBytes;
            if (!TryBase32Decode(base32String, out outputBytes, digits, comparer))
            {
                output = null;
                return false;
            }

            if (outputBytes == null) output = null;
            else if (outputBytes.Length < 1) output = string.Empty;
            else output = Encoding.Unicode.GetString(outputBytes);
            return true;
        }

        /// <summary>
        /// Tries to convert base32 string to array of bytes.
        /// </summary>
        /// <param name="base32String">Base32 string to convert.</param>
        /// <param name="outputBytes">The output bytes.</param>
        /// <param name="digits">The digits.</param>
        /// <param name="comparer">The character comparer (defaults to <see cref="CharComparer.Ordinal"/>).</param>
        /// <returns><see langword="true"/> if the string was successfully decoded; otherwise <see langword="false"/>.</returns>
        public static bool TryBase32Decode(
            [CanBeNull] this string base32String,
            out byte[] outputBytes,
            [CanBeNull] IReadOnlyList<char> digits = null,
            [CanBeNull] IEqualityComparer<char> comparer = null)
        {
            if (base32String == null)
            {
                outputBytes = null;
                return true;
            }
            if (base32String == string.Empty)
            {
                outputBytes = Array<byte>.Empty;
                return true;
            }

            outputBytes = new byte[base32String.Length * OutByteSize / InByteSize];
            return TryBase32Decode(base32String, outputBytes, 0, -1, digits, comparer);
        }

        /// <summary>
        /// Tries to convert base32 string to array of bytes.
        /// </summary>
        /// <param name="base32String">Base32 string to convert.</param>
        /// <param name="buffer">The buffer to fill.</param>
        /// <param name="offset">The offset in the buffer.</param>
        /// <param name="length">The length (if set will ensure that length bytes are filled).</param>
        /// <param name="digits">The digits.</param>
        /// <param name="comparer">The character comparer (defaults to <see cref="CharComparer.Ordinal"/>).</param>
        /// <returns>
        ///   <see langword="true" /> if the string was successfully decoded; otherwise <see langword="false" />.</returns>
        public static bool TryBase32Decode(
            [CanBeNull] this string base32String,
            [NotNull] byte[] buffer,
            int offset = 0,
            int length = -1,
            [CanBeNull] IReadOnlyList<char> digits = null,
            [CanBeNull] IEqualityComparer<char> comparer = null)
        {
            // Check if string is null
            if (string.IsNullOrEmpty(base32String))
                return length < 1;

            if (digits == null)
                digits = DefaultDigits;
            else if (digits.Count < 32)
                throw new ArgumentException(nameof(digits), Resources.Base32EncoderDecoder_Base32Encode_Invalid_Digits);

            if (comparer == null) comparer = CharComparer.Ordinal;

            // Calculate output length
            int ol = base32String.Length * OutByteSize / InByteSize;
            if (length < 0) length = ol;
            else if (length != ol) return false;

            // Check we have enough room in buffer for specified length
            int end = offset + length;
            if (end > buffer.Length) return false;

            // Position in the string
            int p = 0;

            // Offset inside the character in the string
            int sp = 0;

            // The number of bits filled in the current output byte
            int op = 0;

            // Normally we would iterate on the input array but in this case we actually iterate on the output array
            // We do it because output array doesn't have overflow bits, while input does and it will cause output array overflow if we don't stop in time
            while (offset < end)
            {
                // Look up current character in the dictionary to convert it to byte
                int c = digits.IndexOf(base32String[p], comparer);

                // Check if found
                if (c < 0) return false;

                // Calculate the number of bits we can extract out of current input character to fill missing bits in the output byte
                int bitsAvailableInByte = Math.Min(OutByteSize - sp, InByteSize - op);

                // Make space in the output byte
                buffer[offset] <<= bitsAvailableInByte;

                // Extract the part of the input character and move it to the output byte
                buffer[offset] |= (byte)(c >> (OutByteSize - (sp + bitsAvailableInByte)));

                // Update current sub-byte position
                op += bitsAvailableInByte;

                // Check overflow
                if (op >= InByteSize)
                {
                    // Move to the next byte
                    offset++;
                    op = 0;
                }

                // Update current base32 byte completion
                sp += bitsAvailableInByte;

                // Check overflow or end of input array
                if (sp < OutByteSize) continue;

                // Move to the next character
                p++;
                sp = 0;
            }

            return true;
        }
    }
}