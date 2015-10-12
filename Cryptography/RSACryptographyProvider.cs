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
    /// Implements the RSA algorihmn as an <see cref="AsymmetricCryptographyProvider" />.
    /// </summary>
    /// TODO Support padding in .Net 4.5, and RSAEncryptionPadding in 4.6
    public class RSACryptographyProvider : AsymmetricCryptographyProvider
    {
        /// <summary>
        /// The parmaters for the RSA algorithm.
        /// </summary>
        private readonly RSAParameters _parameters;

        /// <summary>
        /// Whether the encrypted data should include length information.
        /// </summary>
        /// <remarks><para>By default this is set to <see langword="true"/> otherwise there is no reliable way of
        /// deciphering the length of the input data.</para>
        /// <para>When <see langword="false"/> the decrypted data will always be a multiple of <see cref="OutputBlockSize"/>
        /// in length.</para>
        /// </remarks>
        [PublicAPI]
        public readonly bool EncodeLength;

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
        /// Initializes a new instance of the <see cref="RSACryptographyProvider" /> class.
        /// </summary>
        /// <exception cref="CryptographicException">Error initializing the cyptographic service provider.</exception>
        public RSACryptographyProvider(int keySize = 1024, bool encodeLength = true)
            : base(null)
        {
            // Generate new key.
            using (RSACryptoServiceProvider provider = new RSACryptoServiceProvider(keySize))
            {
                CanEncrypt = !provider.PublicOnly;
                _parameters = provider.ExportParameters(!provider.PublicOnly);
                OutputBlockSize = (ushort)(provider.KeySize / 8);
                // TODO Calculation depends on padding...
                InputBlockSize = (ushort)(OutputBlockSize - 11);
            }
            EncodeLength = encodeLength;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RSACryptographyProvider" /> class.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <exception cref="CryptographicException">Error initializing the cyptographic service provider.</exception>
        public RSACryptographyProvider(RSAParameters parameters, bool encodeLength = true)
            : base(null)
        {
            // Generate new key.
            using (RSACryptoServiceProvider provider = new RSACryptoServiceProvider())
            {
                provider.ImportParameters(parameters);
                CanEncrypt = !provider.PublicOnly;
                _parameters = provider.ExportParameters(!provider.PublicOnly);
                OutputBlockSize = (ushort)(provider.KeySize / 8);
                // TODO Calculation depends on padding...
                InputBlockSize = (ushort)(OutputBlockSize - 11);
            }
            EncodeLength = encodeLength;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RSACryptographyProvider" /> class from an
        /// <see cref="XElement">XML</see> configuration.
        /// </summary>
        /// <param name="configuration">The provider element.</param>
        /// <exception cref="CryptographicException">Error initializing the cyptographic service provider.</exception>
        public RSACryptographyProvider([NotNull] XElement configuration)
            : base(configuration)
        {
            XNamespace ns = configuration.Name.Namespace;

            // We encode length unless we have and 'encodeLength' attribute set to false.
            string encodeLengthStr = configuration.Attribute("encodeLength")?.Value;
            bool encodeLength;
            if (string.IsNullOrWhiteSpace(encodeLengthStr) ||
                !bool.TryParse(encodeLengthStr, out encodeLength))
                encodeLength = true;

            XElement rsakvElement = configuration.Element(ns + "RSAKeyValue");

            if (rsakvElement == null)
                throw new CryptographicException(
                    "The expected 'RSAKeyValue' element was not found in the configuration element.");

            string modulus = rsakvElement.Element(ns + "Modulus")?.Value;
            if (string.IsNullOrWhiteSpace(modulus))
                throw new CryptographicException(
                    "The expected 'Modulus' element was not found in the 'RSAKeyValue' element.");

            string exponent = rsakvElement.Element(ns + "Exponent")?.Value;
            if (string.IsNullOrWhiteSpace(exponent))
                throw new CryptographicException(
                    "The expected 'Exponent' element was not found in the 'RSAKeyValue' element.");

            RSAParameters parameters = new RSAParameters();
            parameters.Modulus = Convert.FromBase64String(modulus);
            parameters.Exponent = Convert.FromBase64String(exponent);

            // Grab private key elements
            string p = rsakvElement.Element(ns + "P")?.Value;
            if (!string.IsNullOrWhiteSpace(p)) parameters.P = Convert.FromBase64String(p);

            string q = rsakvElement.Element(ns + "Q")?.Value;
            if (!string.IsNullOrWhiteSpace(q)) parameters.Q = Convert.FromBase64String(q);

            string dp = rsakvElement.Element(ns + "DP")?.Value;
            if (!string.IsNullOrWhiteSpace(dp)) parameters.DP = Convert.FromBase64String(dp);

            string dq = rsakvElement.Element(ns + "DQ")?.Value;
            if (!string.IsNullOrWhiteSpace(dq)) parameters.DQ = Convert.FromBase64String(dq);

            string inverseQ = rsakvElement.Element(ns + "InverseQ")?.Value;
            if (!string.IsNullOrWhiteSpace(inverseQ)) parameters.InverseQ = Convert.FromBase64String(inverseQ);

            string d = rsakvElement.Element(ns + "D")?.Value;
            if (!string.IsNullOrWhiteSpace(d)) parameters.D = Convert.FromBase64String(d);

            // Ensure parameters are valid.
            using (RSACryptoServiceProvider provider = new RSACryptoServiceProvider())
            {
                provider.ImportParameters(parameters);
                CanEncrypt = !provider.PublicOnly;
                _parameters = provider.ExportParameters(!provider.PublicOnly);
                OutputBlockSize = (ushort)(provider.KeySize / 8);
                // TODO Calculation depends on padding...
                InputBlockSize = (ushort)(OutputBlockSize - 11);
            }
            EncodeLength = encodeLength;
        }

        /// <inheritdoc />
        /// <exception cref="CryptographicException">The cryptographic provider cannot perform encryption.</exception>
        public override byte[] Encrypt(byte[] input)
        {
            if (!CanEncrypt) throw new CryptographicException("The cryptographic provider cannot perform encryption.");
            if (input == null) return null;

            // Encode first block including header.
            long length = input.LongLength;
            if (length < 1) return new byte[0];

            byte[] inputBuffer = new byte[InputBlockSize];
            long inputOffset;
            long remainder;
            if (EncodeLength)
            {
                long offset = 0;
                VariableLengthEncoding.Encode((ulong)length, inputBuffer, ref offset);
                length += offset;
                remainder = InputBlockSize - offset;
                if (remainder > input.LongLength) remainder = input.LongLength;
                Array.Copy(input, 0, inputBuffer, offset, remainder);
                inputOffset = -offset;
            }
            else
            {
                remainder = input.LongLength;
                if (remainder > InputBlockSize) remainder = InputBlockSize;
                Array.Copy(input, inputBuffer, remainder);
                inputOffset = 0;
            }

            // Calculate blocks.
            long blocks = 1 + ((length - 1) / InputBlockSize);

            // Encrypt blocks
            using (RSACryptoServiceProvider provider = new RSACryptoServiceProvider())
            {
                provider.ImportParameters(_parameters);

                // If we only have one block encrypt and return;
                if (blocks < 2) return provider.Encrypt(inputBuffer, false);

                // Create output buffer
                byte[] outputBuffer = new byte[OutputBlockSize * blocks];
                long outputOffset = 0;

                int b = 0;
                while (true)
                {
                    byte[] encrypted = provider.Encrypt(inputBuffer, false);
                    // Copy encrypted data into output buffer.
                    Array.Copy(encrypted, 0, outputBuffer, outputOffset, OutputBlockSize);

                    // Exit after processing all blocks.
                    if (++b >= blocks) return outputBuffer;

                    inputOffset += InputBlockSize;
                    outputOffset += OutputBlockSize;
                    remainder = input.LongLength - inputOffset;
                    if (remainder >= InputBlockSize)
                        Array.Copy(input, inputOffset, inputBuffer, 0, InputBlockSize);
                    else
                    {
                        Array.Copy(input, inputOffset, inputBuffer, 0, remainder);
                        Array.Clear(inputBuffer, (int)remainder, (int)(InputBlockSize - remainder));
                    }
                }
            }
        }

        /// <inheritdoc />
        public override byte[] Decrypt(byte[] input)
        {
            if (input == null) return null;

            // Calculate maximum length
            long inputLength = input.LongLength;
            if (inputLength < 1) return new byte[0];

            if (inputLength % OutputBlockSize != 0)
                throw new CryptographicException("Cannot decrypt input block as wrong size.");

            long blocks = inputLength / OutputBlockSize;
            // Decrypt blocks
            using (RSACryptoServiceProvider provider = new RSACryptoServiceProvider())
            {
                provider.ImportParameters(_parameters);

                // Decrypt the first block and get the length.
                byte[] inputBuffer;

                if (blocks < 2)
                    inputBuffer = input;
                else
                {
                    inputBuffer = new byte[OutputBlockSize];
                    Array.Copy(input, inputBuffer, OutputBlockSize);
                }

                byte[] decrypted = provider.Decrypt(inputBuffer, false);

                // If we haven't encoded length and we have only one block, we're done.
                if (!EncodeLength && blocks < 2) return decrypted;

                long outputOffset = 0;
                long outputLength;
                long remainder;
                if (EncodeLength)
                {
                    outputLength = (long)VariableLengthEncoding.Decode(decrypted, ref outputOffset);

                    if (outputOffset + outputLength > blocks * InputBlockSize)
                        throw new CryptographicException("Cannot decrypt input block as invalid length found.");

                    remainder = decrypted.LongLength - outputOffset;
                    if (remainder > outputLength) remainder = outputLength;
                }
                else
                {
                    outputLength = blocks * InputBlockSize;
                    remainder = decrypted.LongLength;
                }

                // Create output array and copy in first block of data
                byte[] output = new byte[outputLength];
                Array.Copy(decrypted, outputOffset, output, 0, remainder);

                // If we have one block we're done.
                if (blocks < 2) return output;

                // Decrypt remaining blocks
                int b = 1;
                long inputOffset = 0;
                outputOffset = 0;
                while (true)
                {
                    outputOffset += remainder;
                    inputOffset += OutputBlockSize;

                    // Get next block
                    remainder = inputLength - inputOffset;
                    if (remainder >= OutputBlockSize)
                        Array.Copy(input, inputOffset, inputBuffer, 0, OutputBlockSize);
                    else
                    {
                        Array.Copy(input, inputOffset, inputBuffer, 0, remainder);
                        Array.Clear(inputBuffer, (int)remainder, (int)(OutputBlockSize - remainder));
                    }

                    // Decrypt block and copy into output
                    decrypted = provider.Decrypt(inputBuffer, false);

                    remainder = outputLength - outputOffset;
                    if (remainder >= decrypted.LongLength) remainder = decrypted.LongLength;
                    Array.Copy(decrypted, 0, output, outputOffset, remainder);

                    // If we're done return output.
                    if (++b >= blocks) return output;
                }
            }
        }


        /// <summary>
        /// Sets the XML configuration for this cryptographic provider.
        /// </summary>
        /// <param name="configuration">The configuration element.</param>
        /// <param name="includePrivateKey">Whether to include the private key.</param>
        /// <returns>The configuration element.</returns>
        /// <exception cref="CryptographicException">The private key cannot be included if it is not available (<see cref="CanEncrypt" /> is <see langword="false" />.</exception>
        /// <remarks><para>
        ///   <i>Warning:</i> This will expose the private key and should be used with
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
        protected override XElement SetXml(XElement configuration, bool includePrivateKey)
        {
            if (includePrivateKey && !CanEncrypt)
                throw new CryptographicException("The private key cannot be included as it is not available.");

            if (!EncodeLength)
                configuration.SetAttributeValue("encodeLength", false);

            XNamespace ns = configuration.Name.Namespace;
            // ReSharper disable AssignNullToNotNullAttribute
            XElement rsakv = new XElement(
                ns + "RSAKeyValue",
                new XElement(ns + "Modulus", Convert.ToBase64String(_parameters.Modulus)),
                new XElement(ns + "Exponent", Convert.ToBase64String(_parameters.Exponent)));
            configuration.Add(rsakv);

            // If we're not including the private key we're done.
            if (!includePrivateKey) return configuration;

            if (_parameters.P != null) rsakv.Add(new XElement(ns + "P", Convert.ToBase64String(_parameters.P)));
            if (_parameters.Q != null) rsakv.Add(new XElement(ns + "Q", Convert.ToBase64String(_parameters.Q)));
            if (_parameters.DP != null) rsakv.Add(new XElement(ns + "DP", Convert.ToBase64String(_parameters.DP)));
            if (_parameters.DQ != null) rsakv.Add(new XElement(ns + "DQ", Convert.ToBase64String(_parameters.DQ)));
            if (_parameters.InverseQ != null)
                rsakv.Add(new XElement(ns + "InverseQ", Convert.ToBase64String(_parameters.InverseQ)));
            if (_parameters.D != null) rsakv.Add(new XElement(ns + "D", Convert.ToBase64String(_parameters.D)));
            // ReSharper restore AssignNullToNotNullAttribute
            return configuration;
        }
    }
}