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
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Configuration;
using WebApplications.Utilities.Cryptography.Configuration;

namespace WebApplications.Utilities.Cryptography
{
    /// <summary>
    /// Base class for all cryptography providers.
    /// </summary>
    public abstract class CryptographyProvider
    {
        /// <summary>
        /// The provider element, if any.
        /// </summary>
        [CanBeNull]
        protected readonly XElement Configuration;

        /// <summary>
        /// Whether the provider can decrypt.
        /// </summary>
        /// <remarks>
        /// <para><see langword="false"/> when only the public key is available for an asymmetric algorithm.</para>
        /// </remarks>
        [PublicAPI]
        public virtual bool CanEncrypt => false;

        /// <summary>
        /// Whether the provider can decrypt.
        /// </summary>
        /// <remarks>
        /// <para><see langword="false"/> for hashing algorithms.</para>
        /// </remarks>
        [PublicAPI]
        public virtual bool CanDecrypt => false;

        /// <summary>
        /// Initializes a new instance of the <see cref="CryptographyProvider" /> class.
        /// </summary>
        /// <param name="configuration">The provider element.</param>
        protected CryptographyProvider([CanBeNull] XElement configuration)
        {
            Configuration = configuration;
            Id = configuration?.Attribute("id")?.Value;
        }

        /// <summary>
        /// Gets the identifier, if the provider is persisted to the configuration; otherwise <see langword="null" />.
        /// </summary>
        /// <value>The identifier.</value>
        [PublicAPI]
        [CanBeNull]
        public string Id { get; }

        /// <summary>
        /// Encrypts the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>A base 64 encoded string of the encrypted data.</returns>
        /// <exception cref="CryptographicException">The cryptographic provider cannot perform encryption.</exception>
        [ContractAnnotation("null => null; notnull => notnull")]
        public string EncryptToString(string input)
        {
            if (!CanEncrypt) throw new CryptographicException("The cryptographic provider cannot perform encryption.");
            return input == null ? null : Convert.ToBase64String(Encrypt(Encoding.Unicode.GetBytes(input)));
        }

        /// <summary>
        /// Encrypts the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>A base 64 encoded string of the encrypted data.</returns>
        /// <exception cref="CryptographicException">The cryptographic provider cannot perform encryption.</exception>
        [ContractAnnotation("null => null; notnull => notnull")]
        public byte[] Encrypt(string input)
        {
            if (!CanEncrypt) throw new CryptographicException("The cryptographic provider cannot perform encryption.");
            return input == null ? null : Encrypt(Encoding.Unicode.GetBytes(input));
        }

        /// <summary>
        /// Encrypts the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The encrypted data.</returns>
        /// <exception cref="CryptographicException">The cryptographic provider cannot perform encryption.</exception>
        [PublicAPI]
        [ContractAnnotation("null => null; notnull => notnull")]
        public virtual byte[] Encrypt(byte[] input)
        {
            throw new CryptographicException("The cryptographic provider cannot perform encryption.");
        }

        /// <summary>
        /// Decrypts the specified base 64 input string to a string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The decrypted string.</returns>
        /// <exception cref="CryptographicException">The cryptographic provider cannot perform decryption.</exception>
        [ContractAnnotation("null => null; notnull => notnull")]
        public string DecryptFromStringToString(string input)
        {
            if (!CanDecrypt) throw new CryptographicException("The cryptographic provider cannot perform decryption.");
            return input == null ? null : Encoding.Unicode.GetString(Decrypt(Convert.FromBase64String(input)));
        }

        /// <summary>
        /// Decrypts the specified base 64 input string to a string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The decrypted data.</returns>
        /// <exception cref="CryptographicException">The cryptographic provider cannot perform decryption.</exception>
        [ContractAnnotation("null => null; notnull => notnull")]
        public byte[] DecryptFromString(string input)
        {
            if (!CanDecrypt) throw new CryptographicException("The cryptographic provider cannot perform decryption.");
            return input == null ? null : Decrypt(Convert.FromBase64String(input));
        }

        /// <summary>
        /// Tries to decrypt the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="output">The decrypted output.</param>
        /// <returns><see langword="true"/> if succeeded; otherwise <see langword="false"/>.</returns>
        [ContractAnnotation("input:null=>true,output:null; true<=input:notnull, output:notnull; false<=output:null")]
        public bool TryDecryptFromStringToString(string input, out string output)
        {
            if (input == null)
            {
                output = null;
                return true;
            }
            try
            {
                output = Encoding.Unicode.GetString(Decrypt(Convert.FromBase64String(input)));
                return true;
            }
                // ReSharper disable once CatchAllClause
            catch (Exception)
            {
                output = null;
                return false;
            }
        }

        /// <summary>
        /// Tries to decrypt the specified base-64 encoded input string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="output">The output.</param>
        /// <returns><see langword="true"/> if succeeded; otherwise <see langword="false"/>.</returns>
        [ContractAnnotation("input:null=>true,output:null; true<=input:notnull, output:notnull; false<=output:null")]
        public virtual bool TryDecryptFromString(string input, out byte[] output)
        {
            if (!CanDecrypt)
            {
                output = null;
                return false;
            }
            if (input == null)
            {
                output = null;
                return true;
            }
            try
            {
                output = Decrypt(Convert.FromBase64String(input));
                return true;
            }
                // ReSharper disable once CatchAllClause
            catch (Exception)
            {
                output = null;
                return false;
            }
        }

        /// <summary>
        /// Tries to decrypt the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="output">The output.</param>
        /// <returns><see langword="true"/> if succeeded; otherwise <see langword="false"/>.</returns>
        [ContractAnnotation("input:null=>true,output:null; true<=input:notnull, output:notnull; false<=output:null")]
        public virtual bool TryDecrypt(byte[] input, out byte[] output)
        {
            if (!CanDecrypt)
            {
                output = null;
                return false;
            }
            if (input == null)
            {
                output = null;
                return true;
            }
            try
            {
                output = Decrypt(input);
                return true;
            }
                // ReSharper disable once CatchAllClause
            catch (Exception)
            {
                output = null;
                return false;
            }
        }

        /// <summary>
        /// Decrypts the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The decyrpted data.</returns>
        /// <exception cref="CryptographicException">The cryptographic provider cannot perform decryption.</exception>
        [PublicAPI]
        [ContractAnnotation("null => null; notnull => notnull")]
        public virtual byte[] Decrypt(byte[] input)
        {
            throw new CryptographicException("The cryptographic provider cannot perform decryption.");
        }

        /// <summary>
        /// Gets the configuration XML, this can be used to create a new provider in future.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The <see paramref="configuration"/> element.</returns>
        [NotNull]
        public XElement GetConfigurationXml(XName name = null) => SetXml(new XElement(name ?? "CryptoProvider"));

        /// <summary>
        /// Saves this cryptography provider's configuration to the specified <see paramref="configuration"/>.
        /// </summary>
        /// <param name="id">The identifier; defaults to <see cref="Id"/> if not specified.</param>
        /// <param name="configuration">The configuration; defaults to the 
        /// <see cref="ConfigurationSection{T}.Active">active</see> <see cref="CryptographyConfiguration"/>.</param>
        /// <exception cref="ConfigurationErrorsException">Cannot persist the current cryptography provider to the configuration if no <see paramref="id"/> is supplied, and there is no <see cref="Id"/>.</exception>
        public void SaveToConfiguration(
            [CanBeNull] string id = null,
            [CanBeNull] CryptographyConfiguration configuration = null)
        {
            if (id == null)
            {
                id = Id;
                if (id == null)
                    throw new ConfigurationErrorsException(
                        "Cannot persist the current cryptography provider to the configuration as it doesn't have an existing id and no id was specified.");
            }
            if (configuration == null) configuration = CryptographyConfiguration.Active;

            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the XML configuration for this cryptographic provider.
        /// </summary>
        /// <param name="configuration">The configuration element.</param>
        /// <returns>The <see paramref="configuration"/> element.</returns>
        [NotNull]
        protected virtual XElement SetXml([NotNull] XElement configuration)
        {
            configuration.RemoveAll();
            if (!string.IsNullOrWhiteSpace(Id))
                configuration.SetAttributeValue("id", Id);
            return configuration;
        }

        /// <summary>
        /// Creates a <see cref="CryptographyProvider" /> from an <see cref="XElement">XML</see> configuration.
        /// </summary>
        /// <param name="configuration">The configuration element.</param>
        /// <returns>A <see cref="CryptographyProvider" />.</returns>
        public static CryptographyProvider Create([NotNull] XElement configuration)
        {
            throw new NotImplementedException();
        }
    }
}