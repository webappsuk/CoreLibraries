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

using System.IO;
using System.Security.Cryptography;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Cryptography
{
    /// <summary>
    /// Extensions for hashing algorithms.
    /// </summary>
    [PublicAPI]
    public static class CryptographyExtensions
    {
        /// <summary>
        /// Computes the hash of a string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="algorithm">The algorithm (defaults to <see cref="SHA256Cng"/>).</param>
        /// <returns>The hash in bytes.</returns>
        [NotNull]
        [PublicAPI]
        public static byte[] GetHash([NotNull] this string input, [CanBeNull] HashAlgorithm algorithm = null)
            => HashingCryptographyProvider.GetHash(input, algorithm);

        /// <summary>
        /// Computes the hash of a string and returns it as a base 64 encoded string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="algorithm">The algorithm (defaults to <see cref="SHA256Cng"/>).</param>
        /// <returns>The hash in a base 64 encoded string.</returns>
        [NotNull]
        [PublicAPI]
        public static string GetHashString([NotNull] this string input, [CanBeNull] HashAlgorithm algorithm = null)
            => HashingCryptographyProvider.GetHashString(input, algorithm);

        /// <summary>
        /// Computes the hash.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <param name="algorithm">The algorithm (defaults to <see cref="SHA256Cng"/>).</param>
        /// <returns>The hash in bytes.</returns>
        [NotNull]
        [PublicAPI]
        public static byte[] GetHash([NotNull]this byte[] buffer, int offset = 0, int count = -1, [CanBeNull] HashAlgorithm algorithm = null)
            => HashingCryptographyProvider.GetHash(buffer, offset, count, algorithm);

        /// <summary>
        /// Computes the hash and returns it as a base 64 encoded string.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <param name="algorithm">The algorithm (defaults to <see cref="SHA256Cng"/>).</param>
        /// <returns>The hash in a base 64 encoded string.</returns>
        [NotNull]
        [PublicAPI]
        public static string GetHashString([NotNull]this byte[] buffer, int offset = 0, int count = -1, [CanBeNull] HashAlgorithm algorithm = null)
            => HashingCryptographyProvider.GetHashString(buffer, offset, count, algorithm);

        /// <summary>
        /// Computes the hash from an input stream.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="algorithm">The algorithm (defaults to <see cref="SHA256Cng"/>).</param>
        /// <returns>The hash in bytes.</returns>
        [NotNull]
        [PublicAPI]
        public static byte[] GetHash([NotNull]this Stream inputStream, [CanBeNull] HashAlgorithm algorithm = null)
            => HashingCryptographyProvider.GetHash(inputStream, algorithm);

        /// <summary>
        /// Computes the hash from an input stream and returns it as a base 64 encoded string.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="algorithm">The algorithm (defaults to <see cref="SHA256Cng"/>).</param>
        /// <returns>The hash in a base 64 encoded string.</returns>
        [NotNull]
        [PublicAPI]
        public static string GetHashString([NotNull]this Stream inputStream, [CanBeNull] HashAlgorithm algorithm = null)
            => HashingCryptographyProvider.GetHashString(inputStream, algorithm);

        /// <summary>
        /// Fills the byte array with random bytes.
        /// </summary>
        /// <param name="data">The array to fill.</param>
        [PublicAPI]
        public static void GetRandomBytes([NotNull] this byte[] data) 
            => RandomCryptographyProvider.GetRandomBytes(data);

        /// <summary>
        /// Fills the byte array with non-zero random bytes.
        /// </summary>
        /// <param name="data">The array to fill.</param>
        [PublicAPI]
        public static void GetNonZeroRandomBytes([NotNull] this byte[] data)
            => RandomCryptographyProvider.GetNonZeroRandomBytes(data);
    }
}