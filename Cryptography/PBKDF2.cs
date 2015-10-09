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
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Cryptography
{
    /// <summary>
    /// Implements password-based key derivation functionality using PBKDF2.
    /// </summary>
    /// <typeparam name="T">The type of hash function that will be used to derive the key.</typeparam>
    /// <threadsafety static="true" instance="false" />
    [PublicAPI]
    public class PBKDF2<T> : DeriveBytes
        where T : HMAC, new()
    {
        [CanBeNull]
        private byte[] _salt;

        [CanBeNull]
        private T _hmac;

        private readonly int _iterations;

        /// <summary>
        /// Gets the size, in bits, of the hash code computed by the underlaying <see cref="HMAC"/>.
        /// </summary>
        /// <value>
        /// The size, in bits, of the computed hash code.
        /// </value>
        public int HashSize
        {
            get
            {
                if (_hmac == null) throw new ObjectDisposedException("PBKDF2");
                return _hmac.HashSize;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PBKDF2{T}"/> class using a password, a salt, and number of iterations to derive the key.
        /// </summary>
        /// <param name="password">The password used to derive the key.</param>
        /// <param name="salt">The key salt used to derive the key.</param>
        /// <param name="iterations">The number of iterations for the operation.</param>
        public PBKDF2(
            [NotNull] string password,
            [NotNull] byte[] salt,
            int iterations = 1000)
            : this(new UTF8Encoding(false).GetBytes(password), salt, iterations)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PBKDF2{T}"/> class using a password, a salt, and number of iterations to derive the key.
        /// </summary>
        /// <param name="password">The password used to derive the key.</param>
        /// <param name="salt">The key salt used to derive the key.</param>
        /// <param name="iterations">The number of iterations for the operation.</param>
        public PBKDF2(
            [NotNull] byte[] password,
            [NotNull] byte[] salt,
            int iterations = 1000)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (salt == null) throw new ArgumentNullException(nameof(salt));
            if (password.Length < 1) throw new ArgumentException(Resources.PBKDF2_PasswordEmpty, nameof(password));
            if (salt.Length < 1) throw new ArgumentException(Resources.PBKDF2_SaltEmpty, nameof(salt));
            if (iterations < 1) throw new ArgumentOutOfRangeException(nameof(iterations));

            _hmac = new T { Key = password };
            _salt = (byte[])salt.Clone();
            _iterations = iterations;
        }

        /// <summary>
        /// Returns the pseudo-random key for the given password and salt, using the size of the underlaying <see cref="HMAC"/> 
        /// for the desired length of the returned byte array.
        /// </summary>
        /// <param name="password">The password used to derive the key.</param>
        /// <param name="salt">The key salt used to derive the key.</param>
        /// <param name="iterations">The number of iterations for the operation.</param>
        /// <returns></returns>
        [NotNull]
        public static byte[] GetBytes(
            [NotNull] string password,
            [NotNull] byte[] salt,
            int iterations)
        {
            using (PBKDF2<T> dk = new PBKDF2<T>(password, salt, iterations))
                return dk.GetBytes();
        }

        /// <summary>
        /// Returns the pseudo-random key for the given password and salt, using the size of the underlaying <see cref="HMAC"/> 
        /// for the desired length of the returned byte array.
        /// </summary>
        /// <param name="password">The password used to derive the key.</param>
        /// <param name="salt">The key salt used to derive the key.</param>
        /// <param name="iterations">The number of iterations for the operation.</param>
        /// <returns></returns>
        [NotNull]
        public static byte[] GetBytes(
            [NotNull] byte[] password,
            [NotNull] byte[] salt,
            int iterations)
        {
            using (PBKDF2<T> dk = new PBKDF2<T>(password, salt, iterations))
                return dk.GetBytes();
        }

        /// <summary>
        /// Returns the pseudo-random key for the given password and salt.
        /// </summary>
        /// <param name="password">The password used to derive the key.</param>
        /// <param name="salt">The key salt used to derive the key.</param>
        /// <param name="iterations">The number of iterations for the operation.</param>
        /// <param name="keyLength">The desired length of the key in bytes.</param>
        /// <returns></returns>
        [NotNull]
        public static byte[] GetBytes(
            [NotNull] string password,
            [NotNull] byte[] salt,
            int iterations,
            int keyLength)
        {
            using (PBKDF2<T> dk = new PBKDF2<T>(password, salt, iterations))
                return dk.GetBytes(keyLength);
        }

        /// <summary>
        /// Returns the pseudo-random key for the given password and salt.
        /// </summary>
        /// <param name="password">The password used to derive the key.</param>
        /// <param name="salt">The key salt used to derive the key.</param>
        /// <param name="iterations">The number of iterations for the operation.</param>
        /// <param name="keyLength">The desired length of the key in bytes.</param>
        /// <returns></returns>
        [NotNull]
        public static byte[] GetBytes(
            [NotNull] byte[] password,
            [NotNull] byte[] salt,
            int iterations,
            int keyLength)
        {
            using (PBKDF2<T> dk = new PBKDF2<T>(password, salt, iterations))
                return dk.GetBytes(keyLength);
        }

        /// <summary>
        /// Returns the pseudo-random key for this object, using the size of the underlaying <see cref="HMAC"/> 
        /// for the desired length of the returned byte array.
        /// </summary>
        /// <returns>
        /// A byte array filled with pseudo-random key bytes.
        /// </returns>
        [NotNull]
        public byte[] GetBytes()
        {
            return GetBytes(HashSize / 8);
        }

        /// <summary>
        /// Returns the pseudo-random key for this object.
        /// </summary>
        /// <returns>
        /// A byte array filled with pseudo-random key bytes.
        /// </returns>
        /// <param name="cb">The desired length of the key in bytes.</param>
        public override byte[] GetBytes(int cb)
        {
            T hmac = _hmac;
            byte[] salt = _salt;
            if (hmac == null ||
                salt == null)
                throw new ObjectDisposedException("PBKDF2");

            int hashSizeBytes = hmac.HashSize / 8;

            uint blockCount = (uint)((cb + hashSizeBytes - 1) / hashSizeBytes);

            byte[] output = new byte[cb];
            int outputOffset = 0;

            // for each block of desired output 
            for (uint blockIndex = 1; blockIndex <= blockCount; blockIndex++)
            {
                byte[] intBlock = BitConverter.GetBytes(blockIndex);
                if (BitConverter.IsLittleEndian)
                    intBlock = new[] { intBlock[3], intBlock[2], intBlock[1], intBlock[0] };

                hmac.TransformBlock(salt, 0, salt.Length, salt, 0);
                hmac.TransformFinalBlock(intBlock, 0, intBlock.Length);
                byte[] temp = hmac.Hash;
                hmac.Initialize();

                Debug.Assert(temp.Length == hashSizeBytes);

                for (int j = 0, k = outputOffset; j < temp.Length && k < cb; j++, k++)
                    output[k] = temp[j];

                for (int i = 1; i < _iterations; i++)
                {
                    temp = hmac.ComputeHash(temp);
                    Debug.Assert(temp.Length == hashSizeBytes);
                    for (int j = 0, k = outputOffset; j < temp.Length && k < cb; j++, k++)
                        output[k] ^= temp[j];
                }

                outputOffset += hashSizeBytes;
            }

            return output;
        }

        /// <summary>
        /// When overridden in a derived class, resets the state of the operation.
        /// </summary>
        public override void Reset()
        {
        }

        /// <summary>
        /// When overridden in a derived class, releases the unmanaged resources used by the <see cref="T:System.Security.Cryptography.DeriveBytes"/> class and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                T hmac = Interlocked.Exchange(ref _hmac, null);
                if (hmac != null)
                    hmac.Dispose();

                byte[] salt = Interlocked.Exchange(ref _salt, null);
                if (salt != null)
                    Array.Clear(salt, 0, salt.Length);
            }
        }
    }
}