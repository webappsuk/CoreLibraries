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
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Configuration;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Cryptography.Configuration;

namespace WebApplications.Utilities.Cryptography
{
    public class AESCryptographer : IEncryptorDecryptor
    {
        /// <summary>
        /// The corresponding <see cref="ProviderElement"/> (if any) to access configuration data.
        /// </summary>
        private readonly ProviderElement _provider;

        /// <summary>
        /// The encryption keys, which are stored by expiry date (descending).
        /// </summary>
        private List<Key> _aesEncryptionKeys = new List<Key>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AESCryptographer"/> class.
        /// </summary>
        /// <param name="keys">The keys to add to this provider.</param>
        internal AESCryptographer(IEnumerable<Key> keys = null)
        {
            if (keys == null)
                return;

            foreach (Key key in keys)
            {
                _aesEncryptionKeys.Add(key);
            }

            _aesEncryptionKeys = _aesEncryptionKeys.OrderByDescending(k => k.Expiry).ToList();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RSACryptographer"/> class.
        /// </summary>
        /// <param name="provider">The provider element.</param>
        /// <param name="keys">The keys to add to this provider.</param>
        internal AESCryptographer(ProviderElement provider, IEnumerable<Key> keys = null)
        {
            _provider = provider;

            if (keys == null)
                return;

            foreach (Key key in keys)
            {
                _aesEncryptionKeys.Add(key);
            }

            _aesEncryptionKeys = _aesEncryptionKeys.OrderByDescending(k => k.Expiry).ToList();
        }

        #region IEncryptorDecryptor Members
        /// <summary>
        /// Encrypts the specified input into a Base32 <see cref="string"/>.
        /// </summary>
        /// <param name="input">The string to encrypt.</param>
        /// <returns>The result of the encryption.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="input"/> is <see langword="null"/>.
        /// </exception>
        /// <seealso cref="Base32EncoderDecoder"/>
        [CanBeNull]
        public string Encrypt([CanBeNull] string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                Key encryptionKey = new Key();
                bool addNewKey = true;

                if (_aesEncryptionKeys.Count > 0)
                {
                    // Check if there is any non-Expired keys to use.
                    bool nonExpiredKeysFound = _aesEncryptionKeys.Count(k => k.Expiry > DateTime.Now) > 0;

                    if (nonExpiredKeysFound)
                    {
                        encryptionKey = _aesEncryptionKeys.First();
                        addNewKey = false;
                    }
                }

                if (addNewKey)
                {
                    const int defaultKeyLifeInDays = 7;

                    // Use the key automatically generated by the AesCryptoServiceProvider.
                    // Store it as a hex string.
                    encryptionKey.Value = string.Concat(aes.Key.Select(b => b.ToString("x2")));

                    // For the expiry, use the number of days specified in the KeyLifeInDays property.
                    encryptionKey.Expiry = DateTime.Now.Add(
                        TimeSpan.FromDays(_provider != null ? _provider.KeyLifeInDays : defaultKeyLifeInDays));

                    WriteEncryptionKeyToConfiguration(encryptionKey);
                }

                byte[] encrypted = EncryptStringToBytes(input, ConvertHexStringToByteArray(encryptionKey.Value), aes.IV);

                return Base32EncoderDecoder.Base32Encode(encrypted);
            }
        }

        /// <summary>
        /// Decrypts the specified base32 <see cref="string"/>.
        /// </summary>
        /// <param name="input">The base32 encoded string to decrypt.</param>
        /// <param name="isLatestKey">
        /// <see langword="true"/> if the encryption was with the latest key; otherwise <see langword="false"/>.
        /// </param>
        /// <returns>The result of the decryption.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="input"/> was <see langword="null"/>.
        /// </exception>
        /// <exception cref="NotImplementedException">
        /// The code was unable to decrypt the provided <paramref name="input"/>.
        /// </exception>
        /// <seealso cref="Base32EncoderDecoder"/>
        [CanBeNull]
        public string Decrypt([CanBeNull] string input, out bool isLatestKey)
        {
            isLatestKey = false;

            if (string.IsNullOrEmpty(input))
                return input;

            byte[] initializationVector = new byte[16];

            // Note: Each symbol in a Base32 string is 5 bits.
            byte[] buffer = new byte[input.Length*5/8];

            if (Base32EncoderDecoder.TryBase32Decode(input, buffer))
            {
                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    // Retrieve the IV from the encrypted string.
                    memoryStream.Read(initializationVector, 0, initializationVector.Length);
                }

                // Need just the text to decrypt now, ignoring the IV.
                byte[] cipherText = buffer.Skip(initializationVector.Length).ToArray();

                string decryptedString = null;
                bool decryptionKeyFound = false;

                // Try to find the key used in the encryption.
                foreach (Key key in _aesEncryptionKeys)
                {
                    if (TryDecryptStringFromBytes(cipherText, ConvertHexStringToByteArray(key.Value),
                                                  initializationVector, out decryptedString))
                    {
                        decryptionKeyFound = true;

                        if (key.Value == _aesEncryptionKeys.First().Value)
                            isLatestKey = true;

                        break;
                    }
                }

                if (!decryptionKeyFound)
                    throw new CryptographicException(Resources.AESCryptographer_Decrypt_DecryptFailed_KeyNotFound);

                return decryptedString;
            }

            // If we get here, decryption failed.
            throw new CryptographicException(
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
        public bool TryDecrypt([CanBeNull] string inputString, [CanBeNull]  out string decryptedString, [CanBeNull] out bool? isLatestKey)
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
        #endregion

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
        private byte[] EncryptStringToBytes([NotNull] string input, [NotNull] byte[] key,
                                            [NotNull] byte[] initializationVector)
        {
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            if (initializationVector == null || initializationVector.Length <= 0)
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
                            aesCryptoServiceProvider.CreateEncryptor(aesCryptoServiceProvider.Key,
                                                                     aesCryptoServiceProvider.IV))
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                // Write the data to the stream.
                                swEncrypt.Write(input);
                            }

                            encrypted = msEncrypt.ToArray();
                        }
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
        private string DecryptStringFromBytes([NotNull] byte[] cipherText, [NotNull] byte[] key,
                                              [NotNull] byte[] initializationVector)
        {
            Contract.Requires(cipherText != null);
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            if (initializationVector == null || initializationVector.Length <= 0)
                throw new ArgumentNullException("initializationVector");

            string decryptedText;

            using (AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider())
            {
                // Use the specified key and IV.
                aesCryptoServiceProvider.Key = key;
                aesCryptoServiceProvider.IV = initializationVector;

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    // Create a decrytor to perform the stream transform.
                    using (
                        ICryptoTransform decryptor =
                            aesCryptoServiceProvider.CreateDecryptor(aesCryptoServiceProvider.Key,
                                                                     aesCryptoServiceProvider.IV))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream.
                                decryptedText = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
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
        private bool TryDecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] initializationVector,
                                               out string decryptedString)
        {
            Contract.Requires(cipherText != null);
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
        /// Writes the encryption keys to the configuration file.
        /// </summary>
        /// <param name="newKey">The new key to add to the configuration file.</param>
        private void WriteEncryptionKeyToConfiguration(Key newKey)
        {
            _aesEncryptionKeys.Add(newKey);
            _aesEncryptionKeys = _aesEncryptionKeys.OrderByDescending(k => k.Expiry).ToList();

            // If the ProviderElement is null we cannot save to the configuration.
            if (_provider == null)
                return;

            // Create a key element to add to the provider element.
            KeyElement newKeyElement = new KeyElement
                                           {
                                               Value = newKey.Value,
                                               Expiry = newKey.Expiry
                                           };

            _provider.Keys.Add(newKeyElement);

            SaveConfiguration();
        }

        /// <summary>
        /// Saves the current configuration to the configuration file.
        /// </summary>
        private static void SaveConfiguration()
        {
            System.Configuration.Configuration configurationObject;

            if (HttpContext.Current == null)
            {
                // Get the configuration object with the purpose of saving the new XML to the configuration file.
                configurationObject = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            }
            else
            {
                // Get the configuration object with the purpose of saving the new XML to the configuration file.
                configurationObject =
                    WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);
            }

            // Get the <cryptography> section and set the raw XML.
            ConfigurationSection cryptographySection = configurationObject.GetSection("cryptography");
            cryptographySection.SectionInformation.SetRawXml(CryptographyConfiguration.Active.RawXml);

            configurationObject.Save(ConfigurationSaveMode.Minimal);
        }

        /// <summary>
        /// Converts the provided hex <see cref="string"/> to a <see cref="byte"/> array.
        /// </summary>
        /// <param name="hexString">The hex string to convert.</param>
        /// <returns>
        /// <para>The hex <see cref="string"/> as an array of <see cref="byte"/>s.</para>
        /// <para>If the hex string is <see langword="null"/> or an odd length then the array will be empty.</para>
        /// </returns>
        private static byte[] ConvertHexStringToByteArray(string hexString)
        {
            if (hexString == null || hexString.Length%2 != 0)
                return new byte[0];

            byte[] hexAsBytes = new byte[hexString.Length/2];

            for (int index = 0; index < hexAsBytes.Length; index++)
            {
                string byteValue = hexString.Substring(index*2, 2);
                hexAsBytes[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return hexAsBytes;
        }
    }
}