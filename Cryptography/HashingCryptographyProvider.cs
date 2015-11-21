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
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Cryptography
{
    /// <summary>
    /// Base class for all hashing cryptographic providers.
    /// </summary>
    public class HashingCryptographyProvider : CryptographyProvider
    {
        /// <inheritdoc />
        public override bool CanEncrypt => true;

        /// <summary>
        /// Initializes a new instance of the <see cref="HashingCryptographyProvider" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="configuration">The configuration.</param>
        protected HashingCryptographyProvider(
            [NotNull] string name,
            [NotNull] XElement configuration)
            : base(name, configuration, false)
        {
        }

        #region Static shortcuts
        /// <summary>
        /// Computes the hash of a string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The hash in bytes.</returns>
        [NotNull]
        [PublicAPI]
        public static byte[] GetHash([NotNull] string input)
        {
            byte[] buffer = Encoding.Unicode.GetBytes(input);
            using (HashAlgorithm algorithm = new SHA256Cng())
                return algorithm.ComputeHash(buffer);
        }

        /// <summary>
        /// Computes the hash of a string and returns it as a base 64 encoded string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The hash in a base 64 encoded string.</returns>
        [NotNull]
        [PublicAPI]
        public static string GetHashString([NotNull] string input)
        {
            byte[] buffer = Encoding.Unicode.GetBytes(input);
            using (HashAlgorithm algorithm = new SHA256Cng())
                return Convert.ToBase64String(algorithm.ComputeHash(buffer));
        }

        /// <summary>
        /// Computes the hash.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <returns>The hash in bytes.</returns>
        [NotNull]
        [PublicAPI]
        public static byte[] GetHash([NotNull]byte[] buffer, int offset = 0, int count = -1)
        {
            if (count < 0) count = buffer.Length;
            using (HashAlgorithm algorithm = new SHA256Cng())
            {
                return offset != 0 || count != buffer.Length
                    ? algorithm.ComputeHash(buffer, offset, count)
                    : algorithm.ComputeHash(buffer);
            }
        }

        /// <summary>
        /// Computes the hash and returns it as a base 64 encoded string.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <returns>The hash in a base 64 encoded string.</returns>
        [NotNull]
        [PublicAPI]
        public static string GetHashString([NotNull]byte[] buffer, int offset = 0, int count = -1)
        {
            if (count < 0) count = buffer.Length;
            byte[] output;
            using (HashAlgorithm algorithm = new SHA256Cng())
            {
                output = offset != 0 || count != buffer.Length
                    ? algorithm.ComputeHash(buffer, offset, count)
                    : algorithm.ComputeHash(buffer);
            }
            return Convert.ToBase64String(output);
        }

        /// <summary>
        /// Computes the hash from an input stream.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <returns>The hash in bytes.</returns>
        [NotNull]
        [PublicAPI]
        public static byte[] GetHash([NotNull]Stream inputStream)
        {
            using (HashAlgorithm algorithm = new SHA256Cng())
                return algorithm.ComputeHash(inputStream);
        }

        /// <summary>
        /// Computes the hash from an input stream and returns it as a base 64 encoded string.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <returns>The hash in a base 64 encoded string.</returns>
        [NotNull]
        [PublicAPI]
        public static string GetHashString([NotNull]Stream inputStream)
        {
            using (HashAlgorithm algorithm = new SHA256Cng())
                return Convert.ToBase64String(algorithm.ComputeHash(inputStream));
        }
        #endregion

        /// <summary>
        /// Gets the algorithm.
        /// </summary>
        /// <returns>A <see cref="System.Security.Cryptography.HashAlgorithm"/>.</returns>
        [NotNull]
        protected virtual HashAlgorithm GetAlgorithm()
        {
            HashAlgorithm algorithm = CryptoConfig.CreateFromName(Name) as HashAlgorithm;
            if (algorithm == null) throw new InvalidOperationException(string.Format(Resources.HashingCryptographyProvider_GetEncryptor_Create_Failed, Name));
            return algorithm;
        }

        /// <summary>
        /// Computes the hash of a string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The hash in bytes.</returns>
        [NotNull]
        [PublicAPI]
        public byte[] ComputeHash([NotNull] string input)
        {
            byte[] buffer = Encoding.Unicode.GetBytes(input);
            using (HashAlgorithm algorithm = GetAlgorithm())
                return algorithm.ComputeHash(buffer);
        }

        /// <summary>
        /// Computes the hash of a string and returns it as a base 64 encoded string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The hash in a base 64 encoded string.</returns>
        [NotNull]
        [PublicAPI]
        public string ComputeHashString([NotNull] string input)
        {
            byte[] buffer = Encoding.Unicode.GetBytes(input);
            using (HashAlgorithm algorithm = GetAlgorithm())
                return Convert.ToBase64String(algorithm.ComputeHash(buffer));
        }

        /// <summary>
        /// Computes the hash.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <returns>The hash in bytes.</returns>
        [NotNull]
        [PublicAPI]
        public byte[] ComputeHash([NotNull]byte[] buffer, int offset = 0, int count = -1)
        {
            if (count < 0) count = buffer.Length;
            using (HashAlgorithm algorithm = GetAlgorithm())
            {
                return offset != 0 || count != buffer.Length
                    ? algorithm.ComputeHash(buffer, offset, count)
                    : algorithm.ComputeHash(buffer);
            }
        }

        /// <summary>
        /// Computes the hash and returns it as a base 64 encoded string.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <returns>The hash in a base 64 encoded string.</returns>
        [NotNull]
        [PublicAPI]
        public string ComputeHashString([NotNull]byte[] buffer, int offset = 0, int count = -1)
        {
            if (count < 0) count = buffer.Length;
            byte[] output;
            using (HashAlgorithm algorithm = GetAlgorithm())
            {
                output = offset != 0 || count != buffer.Length
                    ? algorithm.ComputeHash(buffer, offset, count)
                    : algorithm.ComputeHash(buffer);
            }
            return Convert.ToBase64String(output);
        }

        /// <summary>
        /// Computes the hash from an input stream.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <returns>The hash in bytes.</returns>
        [NotNull]
        [PublicAPI]
        public byte[] ComputeHash([NotNull]Stream inputStream)
        {
            using (HashAlgorithm algorithm = GetAlgorithm())
                return algorithm.ComputeHash(inputStream);
        }

        /// <summary>
        /// Computes the hash from an input stream and returns it as a base 64 encoded string.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <returns>The hash in a base 64 encoded string.</returns>
        [NotNull]
        [PublicAPI]
        public string ComputeHashString([NotNull]Stream inputStream)
        {
            using (HashAlgorithm algorithm = GetAlgorithm())
                return Convert.ToBase64String(algorithm.ComputeHash(inputStream));
        }

        /// <inheritdoc />
        public sealed override ICryptoTransform GetEncryptor()
        {
            HashAlgorithm algorithm = GetAlgorithm();
            return new CryptoTransform<HashAlgorithm>(
                algorithm,
                (a, inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset) =>
                    a.TransformBlock(inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset),
                (a, inputBuffer, inputOffset, inputCount) =>
                    a.TransformFinalBlock(inputBuffer, inputOffset, inputCount),
                algorithm.InputBlockSize,
                algorithm.OutputBlockSize,
                algorithm.CanTransformMultipleBlocks,
                algorithm.CanReuseTransform);
        }

        /// <inheritdoc />
        public sealed override ICryptoTransform GetDecryptor()
        {
            throw new CryptographicException(Resources.CryptographyProvider_Decryption_Not_Supported);
        }

        /// <summary>
        /// Creates a <see cref="CryptographyProvider" /> from an <see cref="AsymmetricAlgorithm" />.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="configurationElement">The optional configuration element.</param>
        /// <returns>A <see cref="CryptographyProvider" />.</returns>
        /// <exception cref="CryptographicException">The algorithm is unsupported.</exception>
        [NotNull]
        internal static HashingCryptographyProvider Create(
            [NotNull] string name,
            [NotNull] HashAlgorithm algorithm,
            [CanBeNull] XElement configurationElement = null)
        {
            // Check for keyed hashing algorithmns
            KeyedHashAlgorithm keyed = algorithm as KeyedHashAlgorithm;
            if (keyed != null) return KeyedHashingCryptographyProvider.Create(name, keyed, configurationElement);

            // Simple hashing algorithm, no real configuration.
            if (configurationElement == null)
                configurationElement = new XElement("configuration");

            return new HashingCryptographyProvider(name, configurationElement);
        }
    }
}