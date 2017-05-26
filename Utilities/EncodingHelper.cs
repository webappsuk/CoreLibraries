#region © Copyright Web Applications (UK) Ltd, 2017.  All rights reserved.
// Copyright (c) 2017, Web Applications UK Ltd
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
using System.Diagnostics;
using System.Linq;
using System.Text;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Helper methods for Encodings.
    /// </summary>
    [PublicAPI]
    public static class EncodingHelper
    {
        /// <summary>
        /// Dictionary of encodings by code page
        /// </summary>
        [NotNull]
        private static readonly Dictionary<int, Encoding> _encodings =
            Encoding.GetEncodings().ToDictionary(e => e.CodePage, e => e.GetEncoding());

        private static readonly int _asciiCodePage = Encoding.ASCII.CodePage;

        /// <summary>
        /// Thread local encoding validator.
        /// </summary>
        [CanBeNull]
        [ThreadStatic]
        private static EncodingValidator _encodingValidator;

        /// <summary>
        /// Gets the encoding validator for the current thread.
        /// </summary>
        [NotNull]
        private static EncodingValidator CurrEncodingValidator
            => _encodingValidator ?? (_encodingValidator = new EncodingValidator());

        /// <summary>
        /// Returns the encoding associated with the specified code page identifier.
        /// </summary>
        /// <param name="codePage">The code page identifier of the preferred encoding.</param>
        /// <returns>The encoding that is associated with the specified code page.</returns>
        [CanBeNull]
        public static Encoding GetEncoding(int codePage)
            => _encodings.TryGetValue(codePage, out Encoding encoding) ? encoding : null;

        #region IsValidEncoding(char[])
        /// <summary>
        /// Checks if all the characters in the character array given are valid in the encoding specified.
        /// </summary>
        /// <param name="chars">The characters to validate.</param>
        /// <param name="encoding">The encoding to validate the characters with.</param>
        /// <returns><see langword="true"/> if the character array is valid in the encoding; otherwise <see langword="false"/>.</returns>
        public static bool IsValidEncoding([NotNull] this char[] chars, [NotNull] Encoding encoding)
        {
            if (encoding == null) throw new ArgumentNullException(nameof(encoding));
            return IsValidEncoding(chars, encoding.CodePage, 0, chars.Length);
        }

        /// <summary>
        /// Checks if all the characters in the character array given are valid in the encoding specified.
        /// </summary>
        /// <param name="chars">The characters to validate.</param>
        /// <param name="encoding">The encoding to validate the characters with.</param>
        /// <param name="index">The index of the first character to validate.</param>
        /// <param name="count">The number of characters to validate.</param>
        /// <returns><see langword="true"/> if the character array is valid in the encoding; otherwise <see langword="false"/>.</returns>
        public static bool IsValidEncoding([NotNull] this char[] chars, [NotNull] Encoding encoding, int index, int count)
        {
            if (encoding == null) throw new ArgumentNullException(nameof(encoding));
            return IsValidEncoding(chars, encoding.CodePage, index, count);
        }

        /// <summary>
        /// Checks if all the characters in the character array given are valid in the encoding with the code page specified.
        /// </summary>
        /// <param name="chars">The characters to validate.</param>
        /// <param name="codePage">The code page of the encoding to validate the characters with.</param>
        /// <returns><see langword="true"/> if the character array is valid in the encoding; otherwise <see langword="false"/>.</returns>
        public static bool IsValidEncoding([NotNull] this char[] chars, int codePage)
            => IsValidEncoding(chars, codePage, 0, chars.Length);

        /// <summary>
        /// Checks if all the characters in the character array given are valid in the encoding with the code page specified.
        /// </summary>
        /// <param name="chars">The characters to validate.</param>
        /// <param name="codePage">The code page of the encoding to validate the characters with.</param>
        /// <param name="index">The index of the first character to validate.</param>
        /// <param name="count">The number of characters to validate.</param>
        /// <returns><see langword="true"/> if the character array is valid in the encoding; otherwise <see langword="false"/>.</returns>
        public static bool IsValidEncoding([NotNull] this char[] chars, int codePage, int index, int count)
        {
            if (chars == null) throw new ArgumentNullException(nameof(chars));

            if (codePage == _asciiCodePage)
                return IsValidAscii(chars);

            if (index < 0 || count < 0)
                throw new ArgumentOutOfRangeException(index < 0 ? "index" : "count", Resources.NeedNonNegNum);
            if (chars.Length - index < count)
                throw new ArgumentOutOfRangeException("chars", Resources.EncodingHelper_Validate_IndexAndCountInvalid);

            if (!_encodings.ContainsKey(codePage))
                throw new ArgumentOutOfRangeException(nameof(codePage), codePage, Resources.EncodingHelper_Validate_UnknownCodePage);

            return CurrEncodingValidator.Validate(chars, codePage, index, count);
        }

        /// <summary>
        /// Checks if all the characters in the character array given are valid ASCII characters.
        /// </summary>
        /// <param name="chars">The characters to validate.</param>
        /// <returns><see langword="true"/> if the character array is valid ASCII; otherwise <see langword="false"/>.</returns>
        public static bool IsValidAscii([NotNull] this char[] chars)
            => IsValidAscii(chars, 0, chars.Length);

        /// <summary>
        /// Checks if all the characters in the character array given are valid ASCII characters.
        /// </summary>
        /// <param name="chars">The characters to validate.</param>
        /// <param name="index">The index of the first character to validate.</param>
        /// <param name="count">The number of characters to validate.</param>
        /// <returns><see langword="true"/> if the character array is valid ASCII; otherwise <see langword="false"/>.</returns>
        public static bool IsValidAscii([NotNull] this char[] chars, int index, int count)
        {
            if (chars == null) throw new ArgumentNullException(nameof(chars));
            if (index < 0 || count < 0)
                throw new ArgumentOutOfRangeException(index < 0 ? "index" : "count", Resources.NeedNonNegNum);
            if (chars.Length - index < count)
                throw new ArgumentOutOfRangeException("chars", Resources.EncodingHelper_Validate_IndexAndCountInvalid);

            for (int l = index + count; index < l; index++)
            {
                char ch = chars[index];
                if (ch > 127)
                    return false;
            }
            return true;
        }
        #endregion

        #region IsValidEncoding(string)
        /// <summary>
        /// Checks if all the characters in the string given are valid in the encoding specified.
        /// </summary>
        /// <param name="str">The string to validate.</param>
        /// <param name="encoding">The encoding to validate the string with.</param>
        /// <returns><see langword="true"/> if the string is valid in the encoding; otherwise <see langword="false"/>.</returns>
        public static bool IsValidEncoding([NotNull] this string str, [NotNull] Encoding encoding)
        {
            if (encoding == null) throw new ArgumentNullException(nameof(encoding));
            return IsValidEncoding(str, encoding.CodePage);
        }

        /// <summary>
        /// Checks if all the characters in the string given are valid in the encoding with the code page specified.
        /// </summary>
        /// <param name="str">The string to validate.</param>
        /// <param name="codePage">The code page of the encoding to validate the string with.</param>
        /// <returns><see langword="true"/> if the string is valid in the encoding; otherwise <see langword="false"/>.</returns>
        public static bool IsValidEncoding([NotNull] this string str, int codePage)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));

            if (codePage == _asciiCodePage)
                return IsValidAscii(str);

            if (!_encodings.ContainsKey(codePage))
                throw new ArgumentOutOfRangeException(nameof(codePage), codePage, Resources.EncodingHelper_Validate_UnknownCodePage);

            return CurrEncodingValidator.Validate(str, codePage);
        }

        /// <summary>
        /// Checks if all the characters in the string given are valid ASCII characters.
        /// </summary>
        /// <param name="str">The string to validate.</param>
        /// <returns><see langword="true"/> if the string is valid ASCII; otherwise <see langword="false"/>.</returns>
        public static bool IsValidAscii([NotNull] this string str)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));
            foreach (char ch in str)
                if (ch > 127)
                    return false;
            return true;
        }
        #endregion

        #region IsValidEncoding(byte[])
        /// <summary>
        /// Checks if the byte array given is a valid encoded string in the encoding specified.
        /// </summary>
        /// <param name="bytes">The bytes to validate.</param>
        /// <param name="encoding">The encoding to validate the byte array with.</param>
        /// <returns><see langword="true"/> if the byte array is valid in the encoding; otherwise <see langword="false"/>.</returns>
        public static bool IsValidEncoding([NotNull] this byte[] bytes, [NotNull] Encoding encoding)
        {
            if (encoding == null) throw new ArgumentNullException(nameof(encoding));
            return IsValidEncoding(bytes, encoding.CodePage, 0, bytes.Length);
        }

        /// <summary>
        /// Checks if the byte array given is a valid encoded string in the encoding specified.
        /// </summary>
        /// <param name="bytes">The bytes to validate.</param>
        /// <param name="encoding">The encoding to validate the byte array with.</param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns><see langword="true"/> if the byte array is valid in the encoding; otherwise <see langword="false"/>.</returns>
        public static bool IsValidEncoding([NotNull] this byte[] bytes, [NotNull] Encoding encoding, int index, int count)
        {
            if (encoding == null) throw new ArgumentNullException(nameof(encoding));
            return IsValidEncoding(bytes, encoding.CodePage, index, count);
        }

        /// <summary>
        /// Checks if the byte array given is a valid encoded string in the encoding with the code page specified.
        /// </summary>
        /// <param name="bytes">The bytes to validate.</param>
        /// <param name="codePage">The code page of the encoding to validate the byte array with.</param>
        /// <returns><see langword="true"/> if the byte array is valid in the encoding; otherwise <see langword="false"/>.</returns>
        public static bool IsValidEncoding([NotNull] this byte[] bytes, int codePage)
            => IsValidEncoding(bytes, codePage, 0, bytes.Length);

        /// <summary>
        /// Checks if the byte array given is a valid encoded string in the encoding with the code page specified.
        /// </summary>
        /// <param name="bytes">The bytes to validate.</param>
        /// <param name="codePage">The code page of the encoding to validate the byte array with.</param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns><see langword="true"/> if the byte array is valid in the encoding; otherwise <see langword="false"/>.</returns>
        public static bool IsValidEncoding([NotNull] this byte[] bytes, int codePage, int index, int count)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));

            if (codePage == _asciiCodePage)
                return IsValidAscii(bytes);

            if (index < 0 || count < 0)
                throw new ArgumentOutOfRangeException(index < 0 ? "index" : "count", Resources.NeedNonNegNum);
            if (bytes.Length - index < count)
                throw new ArgumentOutOfRangeException("bytes", Resources.EncodingHelper_Validate_IndexAndCountInvalid);

            if (!_encodings.ContainsKey(codePage))
                throw new ArgumentOutOfRangeException(nameof(codePage), codePage, Resources.EncodingHelper_Validate_UnknownCodePage);

            return CurrEncodingValidator.Validate(bytes, codePage, index, count);
        }

        /// <summary>
        /// Checks if all the byte array given are valid encoded ASCII characters.
        /// </summary>
        /// <param name="bytes">The bytes to validate.</param>
        /// <returns><see langword="true"/> if the byte array is valid ASCII; otherwise <see langword="false"/>.</returns>
        public static bool IsValidAscii([NotNull] this byte[] bytes)
            => IsValidAscii(bytes, 0, bytes.Length);

        /// <summary>
        /// Checks if all the byte array given are valid encoded ASCII characters.
        /// </summary>
        /// <param name="bytes">The bytes to validate.</param>
        /// <param name="index">The index of the first byte to validate.</param>
        /// <param name="count">The number of bytes to validate.</param>
        /// <returns><see langword="true"/> if the byte array is valid ASCII; otherwise <see langword="false"/>.</returns>
        public static bool IsValidAscii([NotNull] this byte[] bytes, int index, int count)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            if (index < 0 || count < 0)
                throw new ArgumentOutOfRangeException(index < 0 ? "index" : "count", Resources.NeedNonNegNum);
            if (bytes.Length - index < count)
                throw new ArgumentOutOfRangeException("bytes", Resources.EncodingHelper_Validate_IndexAndCountInvalid);

            for (int l = index + count; index < l; index++)
            {
                byte bt = bytes[index];
                if (bt > 127)
                    return false;
            }
            return true;
        }
        #endregion

        /// <summary>
        /// Class for validating strings and bytes based on an encoding.
        /// </summary>
        private sealed class EncodingValidator
        {
            private bool _valid;

            [NotNull]
            private readonly Dictionary<int, Encoding> _encs = new Dictionary<int, Encoding>();

            [NotNull]
            private readonly ValidatorEncoderFallback _encoderFallback;

            [NotNull]
            private readonly ValidatorDecoderFallback _decoderFallback;

            /// <summary>
            /// Initializes a new instance of the <see cref="EncodingValidator"/> class.
            /// </summary>
            public EncodingValidator()
            {
                _encoderFallback = new ValidatorEncoderFallback(this);
                _decoderFallback = new ValidatorDecoderFallback(this);
            }

            /* TODO Can't just split a block up, might be cutting a character in half
            /// <summary>
            /// The size of the block that character/byte arrays are processed in
            /// </summary>
            /// <remarks>Value based on some performance testing. There is a noticeable (if small) 
            /// bump in the speed at this size for long strings which are valid or have invalid characters at the end. 
            /// For smaller strings, this point has similar speeds for both valid and any invalid stings.</remarks>
            private const int BlockSize = 64;
            //*/

            /// <summary>
            /// Gets the validator encoding.
            /// </summary>
            /// <param name="codePage">The code page.</param>
            [NotNull]
            private Encoding GetValidatorEncoding(int codePage)
            {
                if (_encs.TryGetValue(codePage, out Encoding encoding))
                    return encoding;
                encoding = Encoding.GetEncoding(codePage, _encoderFallback, _decoderFallback);
                _encs.Add(codePage, encoding);

                Debug.Assert(encoding != null, "encoding != null");
                return encoding;
            }

            /// <summary>
            /// Validates the specified characters.
            /// </summary>
            /// <param name="chars">The characters to validate.</param>
            /// <param name="codePage">The code page of the encoding to validate the characters with.</param>
            /// <param name="index">The index of the first character to validate.</param>
            /// <param name="count">The number of characters to validate.</param>
            /// <returns><see langword="true"/> if the character array is valid in the encoding; otherwise <see langword="false"/>.</returns>
            public bool Validate([NotNull] char[] chars, int codePage, int index, int count)
            {
                _valid = true;

                Encoding encoding = GetValidatorEncoding(codePage);

                /* TODO Can't just split a block up, might be cutting a character in half 
                for (; count > 0; index += BlockSize, count -= BlockSize)
                {
                    // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                    encoding.GetByteCount(chars, index, count > BlockSize ? BlockSize : count);

                    if (!_valid) return false;
                }
                //*/
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                encoding.GetByteCount(chars, index, count);

                return _valid;
            }

            /// <summary>
            /// Validates the specified string.
            /// </summary>
            /// <param name="str">The string to validate.</param>
            /// <param name="codePage">The code page of the encoding to validate the string with.</param>
            /// <returns><see langword="true"/> if the string is valid in the encoding; otherwise <see langword="false"/>.</returns>
            public bool Validate([NotNull] string str, int codePage)
            {
                /* TODO Can't just split a block up, might be cutting a character in half 
                // If the string is bigger than the block size, it might be quicker validating a character array
                int strLength = str.Length;
                if (strLength > BlockSize)
                    return Validate(str.ToCharArray(), codePage, 0, strLength);
                //*/

                _valid = true;

                Encoding encoding = GetValidatorEncoding(codePage);

                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                encoding.GetByteCount(str);

                return _valid;
            }

            /// <summary>
            /// Validates the specified bytes.
            /// </summary>
            /// <param name="bytes">The bytes to validate.</param>
            /// <param name="codePage">The code page of the encoding to validate the byte array with.</param>
            /// <param name="index"></param>
            /// <param name="count"></param>
            /// <returns><see langword="true"/> if the byte array is valid in the encoding; otherwise <see langword="false"/>.</returns>
            public bool Validate([NotNull] byte[] bytes, int codePage, int index, int count)
            {
                _valid = true;

                Encoding encoding = GetValidatorEncoding(codePage);

                /* TODO Can't just split a block up, might be cutting a character in half 
                for (; count > 0; index += BlockSize, count -= BlockSize)
                {
                    // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                    encoding.GetCharCount(bytes, index, count > BlockSize ? BlockSize : count);

                    if (!_valid) return false;
                }
                //*/
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                encoding.GetCharCount(bytes, index, count);

                return _valid;
            }

            #region Fallback
            // If fallback occurs, then the string/characters/bytes are not valid.

            private sealed class ValidatorEncoderFallback : EncoderFallback
            {
                [NotNull]
                private readonly EncodingValidator _encoding;

                public ValidatorEncoderFallback([NotNull] EncodingValidator encoding) => _encoding = encoding;

                public override EncoderFallbackBuffer CreateFallbackBuffer()
                    => new ValidatorEncoderFallbackBuffer(_encoding);

                public override int MaxCharCount => 0;

                public override bool Equals(Object value)
                    => value is ValidatorEncoderFallback other && _encoding == other._encoding;

                public override int GetHashCode()
                    => _encoding.GetHashCode();
            }

            private sealed class ValidatorEncoderFallbackBuffer : EncoderFallbackBuffer
            {
                [NotNull]
                private readonly EncodingValidator _encoding;

                public ValidatorEncoderFallbackBuffer([NotNull] EncodingValidator encoding) => _encoding = encoding;

                public override bool Fallback(char charUnknown, int index)
                    => _encoding._valid = false;

                public override bool Fallback(char charUnknownHigh, char charUnknownLow, int index)
                    => _encoding._valid = false;

                public override char GetNextChar() => '\0';

                public override bool MovePrevious() => false;

                public override int Remaining => 0;

                public override void Reset()
                {
                }
            }

            private sealed class ValidatorDecoderFallback : DecoderFallback
            {
                [NotNull]
                private readonly EncodingValidator _encoding;

                public ValidatorDecoderFallback([NotNull] EncodingValidator encoding) => _encoding = encoding;

                public override DecoderFallbackBuffer CreateFallbackBuffer()
                    => new ValidatorDecoderFallbackBuffer(_encoding);

                public override int MaxCharCount => 0;

                public override bool Equals(Object value)
                    => value is ValidatorDecoderFallback other && _encoding == other._encoding;

                public override int GetHashCode()
                    => _encoding.GetHashCode();
            }

            private sealed class ValidatorDecoderFallbackBuffer : DecoderFallbackBuffer
            {
                [NotNull]
                private readonly EncodingValidator _encoding;

                public ValidatorDecoderFallbackBuffer([NotNull] EncodingValidator encoding) => _encoding = encoding;

                public override bool Fallback(byte[] bytesUnknown, int index) => _encoding._valid = false;

                public override char GetNextChar() => '\0';

                public override bool MovePrevious() => false;

                public override int Remaining => 0;

                public override void Reset()
                {
                }
            }
            #endregion
        }
    }
}