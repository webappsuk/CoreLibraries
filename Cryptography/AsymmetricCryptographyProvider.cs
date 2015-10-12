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

using System.Security.Cryptography;
using System.Xml.Linq;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Cryptography
{
    /// <summary>
    /// Base class for all asymmetric cryptographic providers.
    /// </summary>
    public abstract class AsymmetricCryptographyProvider : CryptographyProvider
    {
        /// <inheritdoc />
        public override bool CanDecrypt => true;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsymmetricCryptographyProvider" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        protected AsymmetricCryptographyProvider(XElement configuration)
            : base(configuration)
        {
        }

        /// <summary>
        /// Sets the XML configuration for this cryptographic provider.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     <i>Warning:</i> This will expose the private key and should be used with
        /// care to only store the key in a secure location.</para>
        /// </remarks>
        /// <param name="configuration">The configuration element.</param>
        /// <returns>
        /// The configuration element.
        /// </returns>
        protected override sealed XElement SetXml(XElement configuration)
            => SetXml(base.SetXml(configuration), CanEncrypt);

        /// <summary>
        /// Sets the XML configuration for this cryptographic provider.
        /// </summary>
        /// <param name="configuration">The configuration element.</param>
        /// <param name="includePrivateKey">Whether to include the private key.</param>
        /// <returns>The configuration element.</returns>
        /// <remarks><i>Warning:</i> This will expose the private key and should be used with
        /// care to only store the key in a secure location.</remarks>
        [NotNull]
        protected abstract XElement SetXml([NotNull] XElement configuration, bool includePrivateKey);

        /// <summary>
        /// Gets the configuration XML, this can be used to create a new provider in future.
        /// </summary>
        /// <param name="includePrivateKey">Whether to include the private key.</param>
        /// <param name="name">The name.</param>
        /// <returns>The configuration element.</returns>
        /// <exception cref="CryptographicException">The private key cannot be included if it is not available (<see cref="CryptographyProvider.CanEncrypt"/> is <see langword="false"/>.</exception>
        [NotNull]
        public XElement GetConfigurationXml(bool includePrivateKey, XName name = null)
        {
            if (includePrivateKey && !CanEncrypt)
                throw new CryptographicException("The private key cannot be included as it is not available.");

            return SetXml(new XElement(name ?? "CryptoProvider"), includePrivateKey);
        }
    }
}