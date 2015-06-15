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
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Configuration;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Cryptography.Configuration;

namespace WebApplications.Utilities.Cryptography
{
    /// <summary>
    /// Allows encryption and decryption using RSA.
    /// </summary>
    internal class RSACryptographer : IEncryptorDecryptor
    {
        /// <summary>
        /// The corresponding <see cref="ProviderElement"/> (if any) to access configuration data.
        /// </summary>
        private readonly ProviderElement _provider;

        /// <summary>
        /// These are ordered by the expiry date descending.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        private List<Key> _rsaEncryptionKeys = new List<Key>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RSACryptographer"/> class.
        /// </summary>
        /// <param name="keys">The keys to add to this provider.</param>
        internal RSACryptographer(IEnumerable<Key> keys = null)
        {
            if (keys == null)
                return;

            _rsaEncryptionKeys.AddRange(keys);
            // ReSharper disable PossibleNullReferenceException
            _rsaEncryptionKeys.Sort((a, b) => b.Expiry.CompareTo(a.Expiry));
            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RSACryptographer"/> class.
        /// </summary>
        /// <param name="provider">The provider element.</param>
        /// <param name="keys">The keys to add to this provider.</param>
        [UsedImplicitly]
        internal RSACryptographer(ProviderElement provider, IEnumerable<Key> keys = null)
        {
            _provider = provider;

            if (keys == null)
                return;

            _rsaEncryptionKeys.AddRange(keys);
            // ReSharper disable PossibleNullReferenceException
            _rsaEncryptionKeys.Sort((a, b) => b.Expiry.CompareTo(a.Expiry));
            // ReSharper restore PossibleNullReferenceException
        }

        #region IEncryptorDecryptor Members
        /// <summary>
        /// Decrypts the <see cref="string"/> specified.
        /// </summary>
        /// <param name="inputStr">The string to try to decrypt.</param>
        /// <param name="decryptedString">
        /// <para>The decrypted string.</para>
        /// <para>If the decryption fails then this is set to <see langword="null"/>.</para>
        /// </param>
        /// <param name="isLatestKey">
        /// <see langword="true"/> if the decryption used the latest key; otherwise <see langword="false"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the decryption was successful; otherwise <see langword="false"/>.
        /// </returns>
        public bool TryDecrypt(string inputStr, out string decryptedString, out bool? isLatestKey)
        {
            decryptedString = null;
            isLatestKey = null;

            try
            {
                bool l;
                decryptedString = Decrypt(inputStr, out l);
                isLatestKey = l;

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Decrypts the <see cref="string"/> specified.
        /// </summary>
        /// <param name="input">The string to decrypt.</param>
        /// <param name="isLatestKey">
        /// <see langword="true"/> if the decryption used the latest key; otherwise <see langword="false"/>.
        /// </param>
        /// <returns>The decrypted <see cref="string"/>.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="input"/> was <see langword="null"/>.
        /// </exception>
        /// <exception cref="CryptographicException">
        /// None of the keys stored resulted in a successful decryption.
        /// </exception>
        public string Decrypt(string input, out bool isLatestKey)
        {
            isLatestKey = false;
            if (string.IsNullOrEmpty(input))
                return input;

            foreach (Key key in _rsaEncryptionKeys)
            {
                int startPosition = 0;

                // The end of each block is padded with an =.
                int endPosition = input.IndexOf("=", startPosition, StringComparison.Ordinal);

                string decrypted = string.Empty;

                while (endPosition > -1)
                {
                    string block = input.Substring(startPosition, endPosition - startPosition + 1);

                    CspParameters keyContainer = new CspParameters { KeyContainerName = key.Value };
                    RSACryptoServiceProvider provider = new RSACryptoServiceProvider(keyContainer);

                    byte[] byteArray;
                    try
                    {
                        byteArray = provider.Decrypt(Convert.FromBase64String(block), false);
                    }
                    catch (CryptographicException)
                    {
                        endPosition = -1;
                        continue;
                    }
                    Debug.Assert(byteArray != null);

                    decrypted += Encoding.Unicode.GetString(byteArray);
                    startPosition = endPosition + 1;

                    // If no = is found -1 is returned, this is the last block.
                    endPosition = input.IndexOf("=", startPosition, StringComparison.Ordinal);
                }

                if (decrypted != string.Empty)
                {
                    Debug.Assert(_rsaEncryptionKeys[0] != null);
                    if (key.Value == _rsaEncryptionKeys[0].Value)
                        isLatestKey = true;

                    return decrypted.TrimEnd('\0');
                }
            }

            // If we get here, decryption failed.
            throw new CryptographicException(Resources.RSACryptographer_Decrypt_DecryptionFailed);
        }

        /// <summary>
        /// Encrypts the specified input <see cref="string"/>.
        /// </summary>
        /// <param name="input">The string to encrypt.</param>
        /// <returns>The encrypted <paramref name="input"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="input"/> was <see langword="null"/>.
        /// </exception>
        public string Encrypt(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            RSACryptoServiceProvider encryptionProvider = InitialiseCryptoServiceProvider();

            const int blockSize = 32;
            string resultString = string.Empty;
            int numberOfBlocks = (input.Length / blockSize) + 1;

            for (int i = 0; i < numberOfBlocks; i++)
            {
                byte[] byteArray = new byte[blockSize * 2];

                // The amount of characters to read in for the current block.
                int readSize;

                if (i == numberOfBlocks - 1)
                    // Get the remaining characters for the final block.
                    readSize = (input.Length % blockSize);
                else
                    readSize = blockSize;

                // Get the bytes for the current block.
                Encoding.Unicode.GetBytes(input, blockSize * i, readSize, byteArray, 0);

                // Encrypt the current block.
                byteArray = encryptionProvider.Encrypt(byteArray, false);
                Debug.Assert(byteArray != null);

                resultString += Convert.ToBase64String(byteArray);
            }

            return resultString;
        }
        #endregion

        /// <summary>
        /// Initialises the crypto service provider.
        /// </summary>
        /// <returns>
        /// The <see cref="RSACryptoServiceProvider"/> containing the encryption key information.
        /// </returns>
        [NotNull]
        private RSACryptoServiceProvider InitialiseCryptoServiceProvider()
        {
            RSACryptoServiceProvider encryptionProvider = new RSACryptoServiceProvider();
            bool addNewKey = true;

            if (_rsaEncryptionKeys.Count > 0)
            {
                // Check if there's any non-Expired keys to use.
                bool nonExpiredKeysFound = _rsaEncryptionKeys.Count(k => k.Expiry > DateTime.Now) > 0;

                if (nonExpiredKeysFound)
                {
                    Key key = _rsaEncryptionKeys[0];
                    Debug.Assert(key != null);

                    CspParameters keyContainer = new CspParameters { KeyContainerName = key.Value };
                    encryptionProvider = new RSACryptoServiceProvider(keyContainer);

                    addNewKey = false;
                }
            }

            if (addNewKey)
            {
                const int defaultKeyLifeInDays = 7;

                // Create the new key container name using a Guid identifier.
                string keyContainerName = CombGuid.NewCombGuid().ToString("N");
                Debug.Assert(keyContainerName != null);
                Key key = new Key(
                    keyContainerName,
                    DateTime.Now.Add(
                        TimeSpan.FromDays(
                            _provider != null
                                ? _provider.KeyLifeInDays
                                : defaultKeyLifeInDays)));

                WriteEncryptionKeyToConfiguration(key);

                CspParameters keyContainer = new CspParameters { KeyContainerName = keyContainerName };
                encryptionProvider = new RSACryptoServiceProvider(keyContainer);
            }

            return encryptionProvider;
        }

        /// <summary>
        /// Writes the encryption keys to the configuration.
        /// </summary>
        /// <param name="newKey">The new key to add to the configuration.</param>
        private void WriteEncryptionKeyToConfiguration([NotNull] Key newKey)
        {
            _rsaEncryptionKeys.Add(newKey);
            _rsaEncryptionKeys = _rsaEncryptionKeys.OrderByDescending(k => k.Expiry).ToList();

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
            // Get the configuration object with the purpose of saving the new XML to the configuration file.
            System.Configuration.Configuration configurationObject = HttpContext.Current == null
                ? ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
                : WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);

            // Get the <cryptography> section and set the raw XML.
            ConfigurationSection cryptographySection = configurationObject.GetSection("cryptography");
            Debug.Assert(cryptographySection != null);
            cryptographySection.SectionInformation.SetRawXml(CryptographyConfiguration.Active.RawXml);

            configurationObject.Save(ConfigurationSaveMode.Minimal);
        }
    }
}