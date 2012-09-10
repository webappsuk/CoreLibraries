#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Copyright (c) 2012, Web Applications UK Ltd
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
using System.Security.Cryptography;
using System.Text;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Cryptography
{
    public static class HashAlgorithmExtensions
    {
        #region Static HashAlgorithms
        /// <summary>
        /// The Sha1Managed hash algorithm.
        /// </summary>
        [UsedImplicitly] [NotNull] public static readonly HashAlgorithm Sha1Managed = new SHA1Managed();

        /// <summary>
        /// The SHA1Cng hash algorithm.
        /// </summary>
        [UsedImplicitly] [NotNull] public static readonly HashAlgorithm Sha1Cng = new SHA1Cng();

        /// <summary>
        /// The Sha1Crypto hash algorithm.
        /// </summary>
        [UsedImplicitly] [NotNull] public static readonly HashAlgorithm Sha1Crypto = new SHA1CryptoServiceProvider();

        /// <summary>
        /// The Sha256Managed hash algorithm.
        /// </summary>
        [UsedImplicitly] [NotNull] public static readonly HashAlgorithm Sha256Managed = new SHA256Managed();

        /// <summary>
        /// The Sha256Cng hash algorithm.
        /// </summary>
        [UsedImplicitly] [NotNull] public static readonly HashAlgorithm Sha256Cng = new SHA256Cng();

        /// <summary>
        /// The Sha256Crypto hash algorithm.
        /// </summary>
        [UsedImplicitly] [NotNull] public static readonly HashAlgorithm Sha256Crypto = new SHA256CryptoServiceProvider();

        /// <summary>
        /// The Sha384Managed hash algorithm.
        /// </summary>
        [UsedImplicitly] [NotNull] public static readonly HashAlgorithm Sha384Managed = new SHA384Managed();

        /// <summary>
        /// The Sha384Cng hash algorithm.
        /// </summary>
        [UsedImplicitly] [NotNull] public static readonly HashAlgorithm Sha384Cng = new SHA384Cng();

        /// <summary>
        /// The Sha384Crypto hash algorithm.
        /// </summary>
        [UsedImplicitly] [NotNull] public static readonly HashAlgorithm Sha384Crypto = new SHA384CryptoServiceProvider();

        /// <summary>
        /// The Sha512Managed hash algorithm.
        /// </summary>
        [UsedImplicitly] [NotNull] public static readonly HashAlgorithm Sha512Managed = new SHA512Managed();

        /// <summary>
        /// The Sha512Cng hash algorithm.
        /// </summary>
        [UsedImplicitly] [NotNull] public static readonly HashAlgorithm Sha512Cng = new SHA512Cng();

        /// <summary>
        /// The Sha512Crypto hash algorithm.
        /// </summary>
        [UsedImplicitly] [NotNull] public static readonly HashAlgorithm Sha512Crypto = new SHA512CryptoServiceProvider();
        #endregion

        #region HashExtensions
        /// <summary>
        /// Gets the hash.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="hashAlgorithm">The hash algorithm.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        [NotNull]
        [UsedImplicitly]
        public static string GetHash([NotNull] this string input, [NotNull] HashAlgorithm hashAlgorithm)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;
            byte[] data = Encoding.Unicode.GetBytes(input);
            byte[] output = hashAlgorithm.ComputeHash(data);
            return Convert.ToBase64String(output);
        }

        /// <summary>
        /// Gets the hash bytes.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="hashAlgorithm">The hash algorithm.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        [NotNull]
        [UsedImplicitly]
        public static byte[] GetHashBytes([NotNull] this string input, [NotNull] HashAlgorithm hashAlgorithm)
        {
            if (string.IsNullOrEmpty(input))
                // sha byte array length is 20.
                return new byte[20];
            byte[] data = Encoding.Unicode.GetBytes(input);
            byte[] output = hashAlgorithm.ComputeHash(data);
            return output;
        }

        /// <summary>
        ///   This static method hashes the input string through SHA1 algorithm.
        /// </summary>
        /// <returns>The hash value as a base64 string</returns>
        [NotNull]
        [UsedImplicitly]
        public static string GetHash([NotNull] this string input)
        {
            return GetHash(input, input.Length < 427 ? Sha1Managed : Sha1Crypto);
        }

        /// <summary>
        ///   This static method hashes the input string through SHA1 algorithm.
        /// </summary>
        /// <returns>The hash value in raw bytes</returns>
        [NotNull]
        [UsedImplicitly]
        public static byte[] GetHashBytes([NotNull] this string input)
        {
            return GetHashBytes(input, input.Length < 427 ? Sha1Managed : Sha1Crypto);
        }

        /// <summary>
        ///   This static method hashes the input string through SHA256 algorithm.
        /// </summary>
        /// <returns>The hash value as a base64 string</returns>
        [NotNull]
        [UsedImplicitly]
        public static string GetHashSHA256([NotNull] this string input)
        {
            return GetHash(input, input.Length < 110 ? Sha256Managed : Sha256Cng);
        }

        /// <summary>
        ///   This static method hashes the input string through SHA256 algorithm.
        /// </summary>
        /// <returns>The hash value as a base64 string</returns>
        [NotNull]
        [UsedImplicitly]
        public static byte[] GetHashBytesSHA256([NotNull] this string input)
        {
            return GetHashBytes(input, input.Length < 110 ? Sha256Managed : Sha256Cng);
        }

        /// <summary>
        ///   This static method hashes the input string through SHA384 algorithm.
        /// </summary>
        /// <returns>The hash value as a base64 string</returns>
        [NotNull]
        [UsedImplicitly]
        public static string GetHashSHA384([NotNull] this string input)
        {
            return GetHash(input, Sha384Cng);
        }

        /// <summary>
        ///   This static method hashes the input string through SHA384 algorithm.
        /// </summary>
        /// <returns>The hash value as a base64 string</returns>
        [NotNull]
        [UsedImplicitly]
        public static byte[] GetHashBytesSHA384([NotNull] this string input)
        {
            return GetHashBytes(input, Sha384Cng);
        }

        /// <summary>
        ///   This static method hashes the input string through SHA512 algorithm.
        /// </summary>
        /// <returns>The hash value as a base64 string</returns>
        [NotNull]
        [UsedImplicitly]
        public static string GetHashSHA512([NotNull] this string input)
        {
            return GetHash(input, Sha512Cng);
        }

        /// <summary>
        ///   This static method hashes the input string through SHA512 algorithm.
        /// </summary>
        /// <returns>The hash value as a base64 string</returns>
        [NotNull]
        [UsedImplicitly]
        public static byte[] GetHashBytesSHA512([NotNull] this string input)
        {
            return GetHashBytes(input, Sha512Cng);
        }

        /// <summary>
        ///   This static method hashes the input string through MD5 algorithm.  Use SHA1 in
        ///   preference to MD5.
        /// </summary>
        /// <returns>The hash value as a base64 string</returns>
        [NotNull]
        [UsedImplicitly]
        public static string GetHashMD5([NotNull] this string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] data = Encoding.Unicode.GetBytes(input);
            byte[] output = md5.ComputeHash(data);
            return Convert.ToBase64String(output);
        }

        /// <summary>
        ///   This static method encodes the input string through SHA1 algorithm.  Use SHA1
        ///   in preference to MD5.
        /// </summary>
        /// <returns>The hash value in raw bytes</returns>
        [NotNull]
        [UsedImplicitly]
        public static byte[] GetHashBytesMD5([NotNull] this string input)
        {
            if (string.IsNullOrEmpty(input))
                // md5 byte array length is 16.
                return new byte[16];
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] data = Encoding.Unicode.GetBytes(input);
            return md5.ComputeHash(data);
        }
        #endregion
    }
}