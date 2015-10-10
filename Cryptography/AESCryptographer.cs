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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Cryptography.Configuration;

namespace WebApplications.Utilities.Cryptography
{
    /// <summary>
    /// Allows encryption and decryption using the Advanced Encryption Standard (AES).
    /// </summary>
    [PublicAPI]
    public class AESCryptographer : ICryptoProvider
    {
        /// <summary>
        /// The corresponding <see cref="ProviderElement"/> (if any) to access configuration data.
        /// </summary>
        [CanBeNull]
        private readonly ProviderElement _provider;

        /// <summary>
        /// The encryption keys, which are stored by expiry date (descending).
        /// </summary>
        [NotNull]
        [ItemNotNull]
        private List<Key> _aesEncryptionKeys = new List<Key>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AESCryptographer"/> class.
        /// </summary>
        /// <param name="keys">The keys to add to this provider.</param>
        public AESCryptographer(IEnumerable<Key> keys = null)
        {
            if (keys == null)
                return;

            _aesEncryptionKeys.AddRange(keys);
            // ReSharper disable PossibleNullReferenceException
            _aesEncryptionKeys.Sort((a, b) => b.Expiry.CompareTo(a.Expiry));
            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RSACryptographer" /> class.
        /// </summary>
        /// <param name="provider">The provider element.</param>
        /// <param name="keys">The keys to add to this provider.</param>
        [UsedImplicitly]
        private AESCryptographer([NotNull] ProviderElement provider, IEnumerable<Key> keys = null)
        {
            _provider = provider;

            if (keys == null)
                return;

            _aesEncryptionKeys.AddRange(keys);
            // ReSharper disable PossibleNullReferenceException
            _aesEncryptionKeys.Sort((a, b) => b.Expiry.CompareTo(a.Expiry));
            // ReSharper restore PossibleNullReferenceException
        }

        /// <inheritdoc />
        public string Id => _provider?.Id;

        /// <inheritdoc />
        public string Encrypt(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                Key encryptionKey = null;
                bool addNewKey = true;

                if (_aesEncryptionKeys.Count > 0)
                {
                    // Check if there is any non-Expired keys to use.
                    bool nonExpiredKeysFound = _aesEncryptionKeys.Count(k => k.Expiry > DateTime.Now) > 0;

                    if (nonExpiredKeysFound)
                    {
                        encryptionKey = _aesEncryptionKeys[0];
                        addNewKey = false;
                    }
                }

                if (addNewKey)
                {
                    const int defaultKeyLifeInDays = 7;

                    // Use the key automatically generated by the AesCryptoServiceProvider.
                    // Store it as a hex string.
                    string value = string.Concat(aes.Key.Select(b => b.ToString("x2")));

                    // For the expiry, use the number of days specified in the KeyLifeInDays property.
                    DateTime expiry = DateTime.Now.Add(
                        TimeSpan.FromDays(_provider != null ? _provider.KeyLifeInDays : defaultKeyLifeInDays));

                    encryptionKey = new Key(value, expiry);

                    _aesEncryptionKeys.Add(encryptionKey);
                    _aesEncryptionKeys = _aesEncryptionKeys.OrderByDescending(k => k.Expiry).ToList();

                    _provider?.AddKey(encryptionKey);
                }

                Debug.Assert(encryptionKey != null);
                byte[] encrypted = EncryptStringToBytes(input, ConvertHexStringToByteArray(encryptionKey.Value), aes.IV);

                return Base32EncoderDecoder.Base32Encode(encrypted);
            }
        }

        /// <inheritdoc />
        public string Decrypt(string input, out bool isLatestKey)
        {
            isLatestKey = false;

            if (string.IsNullOrEmpty(input))
                return input;

            byte[] initializationVector = new byte[16];

            // Note: Each symbol in a Base32 string is 5 bits.
            byte[] buffer = new byte[input.Length * 5 / 8];

            if (Base32EncoderDecoder.TryBase32Decode(input, buffer))
            {
                using (MemoryStream memoryStream = new MemoryStream(buffer))
                    // Retrieve the IV from the encrypted string.
                    memoryStream.Read(initializationVector, 0, initializationVector.Length);

                // Need just the text to decrypt now, ignoring the IV.
                byte[] cipherText = buffer.Skip(initializationVector.Length).ToArray();

                string decryptedString = null;
                bool decryptionKeyFound = false;

                // Try to find the key used in the encryption.
                foreach (Key key in _aesEncryptionKeys)
                    if (TryDecryptStringFromBytes(
                        cipherText,
                        ConvertHexStringToByteArray(key.Value),
                        initializationVector,
                        out decryptedString))
                    {
                        decryptionKeyFound = true;

                        Debug.Assert(_aesEncryptionKeys[0] != null);
                        if (key.Value == _aesEncryptionKeys[0].Value)
                            isLatestKey = true;

                        break;
                    }

                if (!decryptionKeyFound)
                    throw new CryptographicException(Resources.AESCryptographer_Decrypt_DecryptFailed_KeyNotFound);

                return decryptedString;
            }

            // If we get here, decryption failed.
            throw new CryptographicException(
                // ReSharper disable once AssignNullToNotNullAttribute
                string.Format(Resources.AESCryptographer_Decrypt_DecryptFailed_InputNotBase32String, input));
        }

        /// <summary>
        /// Tries the decrypt the provided input <see cref="string"/>.
        /// </summary>
        /// <param name="inputString">The string to decrypt.</param>
        /// <param name="decryptedString">
        /// <para>The result of the decryption.</para>
        /// <para>If unsuccessful this will be <see langword="null"/>.</para>
        /// </param>
        /// <param name="isLatestKey">
        /// <see langword="true"/> if the string was encrypted with the latest key; otherwise <see langword="false"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <paramref name="inputString"/> was decrypted successfully; otherwise <see langword="false"/>.
        /// </returns>
        public bool TryDecrypt(
            string inputString,
            out string decryptedString,
            out bool? isLatestKey)
        {
            decryptedString = null;
            isLatestKey = null;

            try
            {
                bool l;
                decryptedString = Decrypt(inputString, out l);
                isLatestKey = l;

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Encrypts the provided input <see cref="string"/> to an array of <see cref="byte"/>s.
        /// </summary>
        /// <param name="input">The input string to encrypt.</param>
        /// <param name="key">The key to use in the encryption.</param>
        /// <param name="initializationVector">
        /// The initialization vector, see <see cref="SymmetricAlgorithm.IV"/>.
        /// </param>
        /// <returns>The <see cref="string"/> encrypted into a <see cref="byte"/> array.</returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="key"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para><paramref name="initializationVector"/> is <see langword="null"/>.</para>
        /// </exception>
        private byte[] EncryptStringToBytes(
            [NotNull] string input,
            [NotNull] byte[] key,
            [NotNull] byte[] initializationVector)
        {
            if (key == null ||
                key.Length <= 0)
                throw new ArgumentNullException("key");
            if (initializationVector == null ||
                initializationVector.Length <= 0)
                throw new ArgumentNullException("initializationVector");

            byte[] encrypted;

            using (AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider())
            {
                aesCryptoServiceProvider.Key = key;
                aesCryptoServiceProvider.IV = initializationVector;

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    // Write the IV to the beginning of the stream.
                    msEncrypt.Write(aesCryptoServiceProvider.IV, 0, initializationVector.Length);

                    using (
                        ICryptoTransform encryptor =
                            aesCryptoServiceProvider.CreateEncryptor(
                                aesCryptoServiceProvider.Key,
                                aesCryptoServiceProvider.IV))
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            // Write the data to the stream.
                            swEncrypt.Write(input);

                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return encrypted;
        }

        /// <summary>
        /// Decrypts the array of <see cref="byte"/>s into a <see cref="string"/>.
        /// </summary>
        /// <param name="cipherText">The cipher text.</param>
        /// <param name="key">The key to use in the encryption.</param>
        /// <param name="initializationVector">
        /// The initialization vector, see <see cref="SymmetricAlgorithm.IV"/>.
        /// </param>
        /// <returns>The result of the decrypted <paramref name="cipherText"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="key"/> was <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para><paramref name="initializationVector"/> was <see langword="null"/>.</para>
        /// </exception>
        private string DecryptStringFromBytes(
            [NotNull] byte[] cipherText,
            [NotNull] byte[] key,
            [NotNull] byte[] initializationVector)
        {
            if (cipherText == null) throw new ArgumentNullException("cipherText");
            if (key == null ||
                key.Length <= 0)
                throw new ArgumentNullException("key");
            if (initializationVector == null ||
                initializationVector.Length <= 0)
                throw new ArgumentNullException("initializationVector");

            string decryptedText;

            using (AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider())
            {
                // Use the specified key and IV.
                aesCryptoServiceProvider.Key = key;
                aesCryptoServiceProvider.IV = initializationVector;

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                    // Create a decrytor to perform the stream transform.
                using (
                    ICryptoTransform decryptor =
                        aesCryptoServiceProvider.CreateDecryptor(
                            aesCryptoServiceProvider.Key,
                            aesCryptoServiceProvider.IV))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    // Read the decrypted bytes from the decrypting stream.
                    decryptedText = srDecrypt.ReadToEnd();
            }

            return decryptedText;
        }

        /// <summary>
        /// Tries the decrypt the encrypted text with the key specified.
        /// This is used internally to find the correct key for decryption.
        /// </summary>
        /// <param name="cipherText">The text to try and decrypt.</param>
        /// <param name="key">The decryption key.</param>
        /// <param name="initializationVector">
        /// The initialization vector, see <see cref="SymmetricAlgorithm.IV"/>.
        /// </param>
        /// <param name="decryptedString">
        /// <para>The result of the decryption.</para>
        /// <para>If unsuccessful this will be <see langword="null"/>.</para>
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <paramref name="cipherText"/> was decrypted successfully with the <paramref name="key"/> provided;
        /// otherwise returns <see langword="false"/>.
        /// </returns>
        private bool TryDecryptStringFromBytes(
            [NotNull] byte[] cipherText,
            [NotNull] byte[] key,
            [NotNull] byte[] initializationVector,
            out string decryptedString)
        {
            if (cipherText == null) throw new ArgumentNullException("cipherText");
            if (key == null) throw new ArgumentNullException("key");
            if (initializationVector == null) throw new ArgumentNullException("initializationVector");

            decryptedString = null;

            try
            {
                decryptedString = DecryptStringFromBytes(cipherText, key, initializationVector);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Converts the provided hex <see cref="string"/> to a <see cref="byte"/> array.
        /// </summary>
        /// <param name="hexString">The hex string to convert.</param>
        /// <returns>
        /// <para>The hex <see cref="string"/> as an array of <see cref="byte"/>s.</para>
        /// <para>If the hex string is <see langword="null"/> or an odd length then the array will be empty.</para>
        /// </returns>
        [NotNull]
        private static byte[] ConvertHexStringToByteArray(string hexString)
        {
            if (hexString == null ||
                hexString.Length % 2 != 0)
                return Array<byte>.Empty;

            byte[] hexAsBytes = new byte[hexString.Length / 2];

            for (int index = 0; index < hexAsBytes.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                hexAsBytes[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return hexAsBytes;
        }
    }
}