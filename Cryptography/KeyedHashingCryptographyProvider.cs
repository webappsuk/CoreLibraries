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
using System.Security.Cryptography;
using System.Xml.Linq;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Cryptography
{
    /// <summary>
    /// Base class for all hashing cryptographic providers that have a key.
    /// </summary>
    public class KeyedHashingCryptographyProvider : HashingCryptographyProvider
    {
        [NotNull]
        private readonly byte[] _keyBytes;

        /// <summary>
        /// Initializes a new instance of the <see cref="HashingCryptographyProvider" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="keyBytes">The key bytes.</param>
        protected KeyedHashingCryptographyProvider(
            [NotNull] string name,
            [NotNull] XElement configuration,
            [NotNull] byte[] keyBytes)
            : base(name, configuration)
        {
            _keyBytes = keyBytes;
        }

        /// <summary>
        /// Gets the algorithm.
        /// </summary>
        /// <returns>A <see cref="System.Security.Cryptography.HashAlgorithm"/>.</returns>
        [NotNull]
        protected override System.Security.Cryptography.HashAlgorithm GetAlgorithm()
        {
            KeyedHashAlgorithm algorithm = CryptoConfig.CreateFromName(Name) as KeyedHashAlgorithm;
            if (algorithm == null) throw new InvalidOperationException(string.Format(Resources.KeyedHashingCryptographyProvider_GetEncryptor_Create_Failed, Name));
            algorithm.Key = _keyBytes;
            return algorithm;
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
        internal static KeyedHashingCryptographyProvider Create(
            [NotNull] string name,
            [NotNull] KeyedHashAlgorithm algorithm,
            [CanBeNull] XElement configurationElement = null)
        {
            XNamespace ns;
            byte[] keyBytes;
            if (configurationElement != null)
            {
                ns = configurationElement.Name.Namespace;
                string key = configurationElement.Element(ns + "Key")?.Value;
                if (string.IsNullOrWhiteSpace(key))
                    throw new CryptographicException(
                        "The expected 'Key' element was not found.");

                keyBytes = Convert.FromBase64String(key);
            }
            else
            {
                ns = XNamespace.None;
                keyBytes = algorithm.Key;
                // ReSharper disable AssignNullToNotNullAttribute
                configurationElement = new XElement(
                    ns + "configuration",
                    new XElement(ns + "Key", Convert.ToBase64String(keyBytes)));
            }
            
            return new KeyedHashingCryptographyProvider(name, configurationElement, keyBytes);
        }
    }
}