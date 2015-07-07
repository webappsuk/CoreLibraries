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

using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Cryptography.Configuration;

namespace WebApplications.Utilities.Cryptography
{
    /// <summary>
    /// A wrapper around crypto provider.
    /// </summary>
    public class CryptoProviderWrapper : IEncryptorDecryptor
    {
        /// <summary>
        /// The ID of the provider being wrapped
        /// </summary>
        public readonly string ID;

        [NotNull]
        private readonly IEncryptorDecryptor _encryptorDecryptor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CryptoProviderWrapper"/> class.
        /// </summary>
        /// <param name="id">The ID of the provider to wrap.</param>
        /// <exception cref="System.Configuration.ConfigurationErrorsException"></exception>
        public CryptoProviderWrapper(string id = null)
        {
            CryptographyConfiguration configuration = CryptographyConfiguration.Active;

            ID = id;

            if (id == null)
                // Default to RSA.
                _encryptorDecryptor = new RSACryptographer();
            else
            {
                ProviderElement providerElement = configuration.Providers.SingleOrDefault(provider => provider.Id == id);

                if (providerElement == null)
                {
                    // If no element is found, defaults to RSA.
                    _encryptorDecryptor = new RSACryptographer();

                    return;
                }

                if (!providerElement.IsEnabled)
                    throw new ConfigurationErrorsException(
                        // ReSharper disable once AssignNullToNotNullAttribute
                        string.Format(Resources.CryptoProviderWrapper_Constructor_ProviderNotEnabled, id));

                // Get keys to pass through to the provider constructor.
                List<Key> keys = providerElement.Keys.Select(key => new Key(key.Value, key.Expiry)).ToList();

                // ReSharper disable AssignNullToNotNullAttribute
                _encryptorDecryptor = providerElement
                    .Type
                    .ConstructorFunc<ProviderElement, IEnumerable<Key>, IEncryptorDecryptor>()
                    (providerElement, keys.Count > 0 ? keys : null);
                // ReSharper restore AssignNullToNotNullAttribute
            }
        }

        /// <summary>
        /// Encrypts the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The encrypted string.</returns>
        public string Encrypt(string input)
        {
            return _encryptorDecryptor.Encrypt(input);
        }

        /// <summary>
        /// Decrypts the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="isLatestKey">if set to <see langword="true" /> the input was encrypted using the latest key.</param>
        /// <returns>The decrypted string.</returns>
        public string Decrypt(string input, out bool isLatestKey)
        {
            bool l;

            string result = _encryptorDecryptor.Decrypt(input, out l);
            isLatestKey = l;

            return result;
        }

        /// <summary>
        /// Attempts to decrypt the input given.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="result">The result.</param>
        /// <param name="isLatestKey">If set to <see langword="true" /> the input was encrypted using the latest key. Will be <see langword="null" /> if the operation failed.</param>
        /// <returns>
        ///   <see langword="true" /> if successful; otherwise <see langword="false" />.
        /// </returns>
        public bool TryDecrypt(string input, out string result, out bool? isLatestKey)
        {
            string r;
            bool? l;

            bool success = _encryptorDecryptor.TryDecrypt(input, out r, out l);
            result = r;
            isLatestKey = l;

            return success;
        }
    }
}