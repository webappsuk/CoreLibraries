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
using WebApplications.Utilities.Cryptography.Configuration;

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
        /// <param name="providerElement">The provider element (if any).</param>
        /// <param name="configuration">The configuration (if any).</param>
        /// <param name="preservesLength"><see langword="true"/> if the provider preserves the length.</param>
        protected AsymmetricCryptographyProvider(
            [CanBeNull] ProviderElement providerElement = null,
            [CanBeNull] XElement configuration = null,
            bool preservesLength = true)
            : base(providerElement, configuration, preservesLength)
        {
        }

        /// <summary>
        /// Creates a <see cref="CryptographyProvider" /> from an <see cref="AsymmetricAlgorithm" />.
        /// </summary>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="configurationElement">The optional configuration element.</param>
        /// <returns>A <see cref="CryptographyProvider" />.</returns>
        /// <exception cref="CryptographicException">The algorithm is unsupported.</exception>
        [NotNull]
        public static CryptographyProvider Create(
            [NotNull] AsymmetricAlgorithm algorithm,
            [CanBeNull] XElement configurationElement = null) => Create(algorithm, null, configurationElement);

        /// <summary>
        /// Creates a <see cref="CryptographyProvider" /> from an <see cref="AsymmetricAlgorithm" />.
        /// </summary>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="providerElement">The optional provider element.</param>
        /// <param name="configurationElement">The optional configuration element.</param>
        /// <returns>A <see cref="CryptographyProvider" />.</returns>
        /// <exception cref="CryptographicException">The algorithm is unsupported.</exception>
        [NotNull]
        internal static CryptographyProvider Create(
            [NotNull] AsymmetricAlgorithm algorithm,
            [CanBeNull] ProviderElement providerElement,
            [CanBeNull] XElement configurationElement)
        {
            // TODO We currently only support RSA
            RSACryptoServiceProvider rsa = algorithm as RSACryptoServiceProvider;
            if (rsa == null)
                throw new CryptographicException(
                    string.Format("Unknown, or unsupported, cryptographic provider '{0}'.", algorithm.GetType()));

            return RSACryptographyProvider.Create(rsa, providerElement, configurationElement);
        }
    }
}