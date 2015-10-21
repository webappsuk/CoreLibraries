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
using System.Xml.Linq;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Cryptography.Configuration;

namespace WebApplications.Utilities.Cryptography
{
    /// <summary>
    /// Implements the RSA algorithm as an <see cref="AsymmetricCryptographyProvider" />.
    /// </summary>
    /// TODO Support padding in .Net 4.5, and RSAEncryptionPadding in 4.6
    public class RSACryptographyProvider : AsymmetricCryptographyProvider
    {
        /// <summary>
        /// The parameters for the RSA algorithm.
        /// </summary>
        private readonly RSAParameters _parameters;

        /// <inheritdoc />
        public override bool CanEncrypt { get; }

        /// <summary>
        /// The input block size.
        /// </summary>
        [PublicAPI]
        public readonly ushort InputBlockSize;

        /// <summary>
        /// The output block size.
        /// </summary>
        [PublicAPI]
        public readonly ushort OutputBlockSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="RSACryptographyProvider" /> class from an
        /// <see cref="XElement">XML</see> configuration.
        /// </summary>
        /// <param name="providerElement">The provider element.</param>
        /// <param name="configuration">The provider element.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="canEncrypt">if set to <c>true</c> [can encrypt].</param>
        /// <param name="inputBlockSize">Size of the input block.</param>
        /// <param name="outputBlockSize">Size of the output block.</param>
        /// <exception cref="CryptographicException">Error initializing the cryptographic service provider.</exception>
        private RSACryptographyProvider(
            [CanBeNull] ProviderElement providerElement,
            [CanBeNull] XElement configuration,
            RSAParameters parameters,
            bool canEncrypt,
            ushort inputBlockSize,
            ushort outputBlockSize)
            : base("RSA", providerElement, configuration, false)
        {
            _parameters = parameters;
            CanEncrypt = canEncrypt;
            InputBlockSize = inputBlockSize;
            OutputBlockSize = outputBlockSize;
        }

        /// <inheritdoc />
        public override ICryptoTransform GetEncryptor()
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.ImportParameters(_parameters);
            return new CryptoTransform<RSACryptoServiceProvider>(
                provider,
                EncryptBlock,
                EncryptFinalBlock,
                InputBlockSize,
                OutputBlockSize);
        }

        /// <summary>
        /// Encrypts the specified region of the input byte array and copies the resulting transform to the specified region of the output byte array.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="inputBuffer">The input for which to compute the transform.</param>
        /// <param name="inputOffset">The offset into the input byte array from which to begin using data.</param>
        /// <param name="inputCount">The number of bytes in the input byte array to use as data.</param>
        /// <param name="outputBuffer">The output to which to write the transform.</param>
        /// <param name="outputOffset">The offset into the output byte array from which to begin writing data.</param>
        /// <returns>The number of bytes written.</returns>
        private int EncryptBlock(
            [NotNull] RSACryptoServiceProvider provider,
            [NotNull] byte[] inputBuffer,
            int inputOffset,
            int inputCount,
            [NotNull] byte[] outputBuffer,
            int outputOffset)
        {
            // As we do not support multi-block encryption the input buffer should always start at 0, and be InputBlockSize in length.
            Debug.Assert(inputOffset == 0 && inputCount == InputBlockSize);
            if (inputBuffer.Length > inputCount)
            {
                byte[] ib = new byte[inputCount];
                Array.Copy(inputBuffer, ib, inputCount);
                inputBuffer = ib;
            }
            byte[] encrypted = provider.Encrypt(inputBuffer, false);
            int encryptedLength = encrypted.Length;
            Debug.Assert(encryptedLength == OutputBlockSize);
            Array.Copy(encrypted, outputBuffer, encryptedLength);
            return encryptedLength;
        }

        /// <summary>
        /// Encrypts the specified region of the specified byte array.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="inputBuffer">The input for which to compute the transform.</param>
        /// <param name="inputOffset">The offset into the byte array from which to begin using data.</param>
        /// <param name="inputCount">The number of bytes in the byte array to use as data.</param>
        /// <returns>The computed transform.</returns>
        private byte[] EncryptFinalBlock(
            [NotNull] RSACryptoServiceProvider provider,
            [NotNull] byte[] inputBuffer,
            int inputOffset,
            int inputCount)
        {
            if (inputCount < 1) return Array<byte>.Empty;

            // As we do not support multi-block encryption the input buffer should always start at 0
            Debug.Assert(inputOffset == 0 && inputCount <= InputBlockSize);

            byte[] encrypted = provider.Encrypt(inputBuffer, false);
            Debug.Assert(encrypted.Length == OutputBlockSize);
            return encrypted;
        }

        /// <inheritdoc />
        public override ICryptoTransform GetDecryptor()
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.ImportParameters(_parameters);
            return new CryptoTransform<RSACryptoServiceProvider>(
                provider,
                DecryptBlock,
                DecryptFinalBlock,
                OutputBlockSize,
                InputBlockSize);
        }
        
        /// <summary>
        /// Decrypts the specified region of the input byte array and copies the resulting transform to the specified region of the output byte array.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="inputBuffer">The input for which to compute the transform.</param>
        /// <param name="inputOffset">The offset into the input byte array from which to begin using data.</param>
        /// <param name="inputCount">The number of bytes in the input byte array to use as data.</param>
        /// <param name="outputBuffer">The output to which to write the transform.</param>
        /// <param name="outputOffset">The offset into the output byte array from which to begin writing data.</param>
        /// <returns>The number of bytes written.</returns>
        private int DecryptBlock(
            [NotNull] RSACryptoServiceProvider provider,
            [NotNull] byte[] inputBuffer,
            int inputOffset,
            int inputCount,
            [NotNull] byte[] outputBuffer,
            int outputOffset)
        {
            // As we do not support multi-block encryption the input buffer should always start at 0, and be OutputBlockSize in length.
            Debug.Assert(inputOffset == 0 && inputCount == OutputBlockSize);
            byte[] decrypted = provider.Decrypt(inputBuffer, false);
            int decryptedLength = decrypted.Length;
            Debug.Assert(decryptedLength == InputBlockSize);
            Array.Copy(decrypted, outputBuffer, decryptedLength);
            return decryptedLength;
        }

        /// <summary>
        /// Decrypts the specified region of the specified byte array.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="inputBuffer">The input for which to compute the transform.</param>
        /// <param name="inputOffset">The offset into the byte array from which to begin using data.</param>
        /// <param name="inputCount">The number of bytes in the byte array to use as data.</param>
        /// <returns>The computed transform.</returns>
        private byte[] DecryptFinalBlock(
            [NotNull] RSACryptoServiceProvider provider,
            [NotNull] byte[] inputBuffer,
            int inputOffset,
            int inputCount)
        {
            // As we do not support multi-block encryption the input buffer should always start at 0
            Debug.Assert(inputOffset == 0);
            if (inputCount < 1) return Array<byte>.Empty;

            Debug.Assert(inputCount == OutputBlockSize);
            byte[] decrypted = provider.Decrypt(inputBuffer, false);
            Debug.Assert(decrypted.Length == InputBlockSize);
            return decrypted;
        }

        /// <summary>
        /// Sets the XML configuration for this cryptographic provider.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="includePrivateKey">Whether to include the private key.</param>
        /// <returns>The configuration element.</returns>
        /// <exception cref="CryptographicException">The private key cannot be included if it is not available (<see cref="CanEncrypt" /> is <see langword="false" />.</exception>
        /// <remarks><para>
        ///   <i>Warning:</i> This may expose the private key and should be used with
        /// care to only store the key in a secure location.</para>
        /// <para> Conforms with the XMLDSIG spec, <see href="https://www.ietf.org/rfc/rfc3075.txt">RFC 3075, Section 6.4.2</see>:
        /// </para>
        /// <code><![CDATA[
        /// <element name = "RSAKeyValue" >
        /// <complexType >
        /// <sequence >
        /// <element name="Modulus" type="ds:CryptoBinary"/>
        /// <element name="Exponent" type="ds:CryptoBinary"/>
        /// </sequence>
        /// </complexType>
        /// </element>
        /// ]]></code>
        /// <para>However, additional private key elements are added:</para>
        /// <code><![CDATA[
        /// <element name = "RSAKeyValue" >
        /// <complexType >
        /// <sequence >
        /// <element name="Modulus" type="ds:CryptoBinary"/>
        /// <element name="Exponent" type="ds:CryptoBinary"/>
        /// <element name="P" type="ds:CryptoBinary"/>
        /// <element name="Q" type="ds:CryptoBinary"/>
        /// <element name="DP" type="ds:CryptoBinary"/>
        /// <element name="DQ" type="ds:CryptoBinary"/>
        /// <element name="InverseQ" type="ds:CryptoBinary"/>
        /// <element name="D" type="ds:CryptoBinary"/>
        /// </sequence>
        /// </complexType>
        /// </element>
        /// ]]></code></remarks>
        private static XElement GetConfiguration(
            RSAParameters parameters,
            [NotNull] XName elementName,
            bool includePrivateKey)
        {
            XNamespace ns = elementName.Namespace;
            // ReSharper disable AssignNullToNotNullAttribute
            XElement configuration = new XElement(
                elementName,
                new XElement(ns + "Modulus", Convert.ToBase64String(parameters.Modulus)),
                new XElement(ns + "Exponent", Convert.ToBase64String(parameters.Exponent)));

            // If we're not including the private key we're done.
            if (!includePrivateKey) return configuration;

            if (parameters.P != null) configuration.Add(new XElement(ns + "P", Convert.ToBase64String(parameters.P)));
            if (parameters.Q != null) configuration.Add(new XElement(ns + "Q", Convert.ToBase64String(parameters.Q)));
            if (parameters.DP != null)
                configuration.Add(new XElement(ns + "DP", Convert.ToBase64String(parameters.DP)));
            if (parameters.DQ != null)
                configuration.Add(new XElement(ns + "DQ", Convert.ToBase64String(parameters.DQ)));
            if (parameters.InverseQ != null)
                configuration.Add(new XElement(ns + "InverseQ", Convert.ToBase64String(parameters.InverseQ)));
            if (parameters.D != null) configuration.Add(new XElement(ns + "D", Convert.ToBase64String(parameters.D)));
            // ReSharper restore AssignNullToNotNullAttribute
            return configuration;
        }


        /// <summary>
        /// Gets the <see cref="RSAParameters"/> from the configuration..
        /// </summary>
        /// <param name="configurationElement">The configuration element.</param>
        /// <exception cref="CryptographicException">The configuration was invalid.</exception>
        /// <returns>An <see cref="RSAParameters"/>.</returns>
        private static RSAParameters GetParameters([NotNull] XElement configurationElement)
        {
            XNamespace ns = configurationElement.Name.Namespace;
            string modulus = configurationElement.Element(ns + "Modulus")?.Value;
            if (string.IsNullOrWhiteSpace(modulus))
                throw new CryptographicException(
                    "The expected 'Modulus' element was not found in the 'RSAKeyValue' element.");

            string exponent = configurationElement.Element(ns + "Exponent")?.Value;
            if (string.IsNullOrWhiteSpace(exponent))
                throw new CryptographicException(
                    "The expected 'Exponent' element was not found in the 'RSAKeyValue' element.");

            RSAParameters parameters = new RSAParameters
            {
                Modulus = Convert.FromBase64String(modulus.DiscardWhiteSpaces()),
                Exponent = Convert.FromBase64String(exponent.DiscardWhiteSpaces())
            };

            // Grab private key elements
            string p = configurationElement.Element(ns + "P")?.Value;
            if (!string.IsNullOrWhiteSpace(p)) parameters.P = Convert.FromBase64String(p.DiscardWhiteSpaces());

            string q = configurationElement.Element(ns + "Q")?.Value;
            if (!string.IsNullOrWhiteSpace(q)) parameters.Q = Convert.FromBase64String(q.DiscardWhiteSpaces());

            string dp = configurationElement.Element(ns + "DP")?.Value;
            if (!string.IsNullOrWhiteSpace(dp)) parameters.DP = Convert.FromBase64String(dp.DiscardWhiteSpaces());

            string dq = configurationElement.Element(ns + "DQ")?.Value;
            if (!string.IsNullOrWhiteSpace(dq)) parameters.DQ = Convert.FromBase64String(dq.DiscardWhiteSpaces());

            string inverseQ = configurationElement.Element(ns + "InverseQ")?.Value;
            if (!string.IsNullOrWhiteSpace(inverseQ))
                parameters.InverseQ = Convert.FromBase64String(inverseQ.DiscardWhiteSpaces());

            string d = configurationElement.Element(ns + "D")?.Value;
            if (!string.IsNullOrWhiteSpace(d)) parameters.D = Convert.FromBase64String(d.DiscardWhiteSpaces());

            return parameters;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RSACryptographyProvider" /> class.
        /// </summary>
        /// <param name="keySize">Size of the key.</param>
        /// <returns>An <see cref="RSACryptographyProvider"/>.</returns>
        /// <exception cref="CryptographicException">Error initializing the cryptographic service provider.</exception>
        [NotNull]
        public static RSACryptographyProvider Create(int keySize = 1024)
        {
            // Generate new key.
            using (RSACryptoServiceProvider provider = new RSACryptoServiceProvider(keySize))
                return Create(provider, null, null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RSACryptographyProvider" /> class.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>An <see cref="RSACryptographyProvider"/>.</returns>
        /// <exception cref="CryptographicException">Error initializing the cryptographic service provider.</exception>
        [NotNull]
        public static RSACryptographyProvider Create(RSAParameters parameters)
        {
            using (RSACryptoServiceProvider provider = new RSACryptoServiceProvider())
            {
                provider.ImportParameters(parameters);
                return Create(provider, null, null);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RSACryptographyProvider" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>An <see cref="RSACryptographyProvider" />.</returns>
        /// <exception cref="CryptographicException">Error initializing the cryptographic service provider.</exception>
        /// <exception cref="CryptographicException">The configuration was invalid.</exception>
        [NotNull]
        public static RSACryptographyProvider Create([CanBeNull] XElement configuration)
        {
            using (RSACryptoServiceProvider provider = new RSACryptoServiceProvider())
                return Create(provider, null, configuration);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RSACryptographyProvider" /> class from an
        /// <see cref="XElement">XML</see> configuration.
        /// </summary>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="providerElement">The provider element.</param>
        /// <param name="configuration">The provider element.</param>
        /// <exception cref="CryptographicException">The configuration was invalid.</exception>
        [NotNull]
        internal static RSACryptographyProvider Create(
            [NotNull] RSACryptoServiceProvider algorithm,
            [CanBeNull] ProviderElement providerElement,
            [CanBeNull] XElement configuration)
        {
            if (providerElement != null)
                configuration = providerElement.Configuration;

            RSAParameters parameters;
            if (configuration != null)
            {
                // Import the parameters from the configuration
                parameters = GetParameters(configuration);
                algorithm.ImportParameters(parameters);
            }
            else
            {
                // Set the configuration from the parameters
                parameters = algorithm.ExportParameters(!algorithm.PublicOnly);
                configuration = GetConfiguration(parameters, "configuration", !algorithm.PublicOnly);
            }

            ushort outputBlockSize = (ushort)(algorithm.KeySize / 8);
            // TODO Calculation depends on padding...
            ushort inputBlockSize = (ushort)(outputBlockSize - 11);

            return new RSACryptographyProvider(
                providerElement,
                configuration,
                parameters,
                !algorithm.PublicOnly,
                inputBlockSize,
                outputBlockSize);
        }
    }
}