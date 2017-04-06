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
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Configuration;

namespace WebApplications.Utilities.Cryptography.Configuration
{
    /// <summary>
    /// Configuration section for cryptography
    /// </summary>
    public class CryptographyConfiguration : ConfigurationSection<CryptographyConfiguration>
    {
        /// <summary>
        /// Gets or sets the <see cref="ProviderCollection">providers</see>.
        /// </summary>
        [ConfigurationProperty("providers", IsRequired = true, IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(ProviderCollection),
            CollectionType = ConfigurationElementCollectionType.BasicMapAlternate)]
        [NotNull]
        [ItemNotNull]
        public ProviderCollection Providers
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            get { return GetProperty<ProviderCollection>("providers"); }
            set { SetProperty("providers", value); }
        }

        /// <summary>
        /// Gets or sets the size of a secure identifier.
        /// </summary>
        /// <value>The size of the secure identifier.</value>
        [ConfigurationProperty("secureIdentifierSize", DefaultValue = 20, IsRequired = false)]
        [IntegerValidator(MinValue = SecureIdentifier.MinLength, MaxValue = 255)]
        public int SecureIdentifierSize
        {
            get { return GetProperty<int>("secureIdentifierSize"); }
            set { SetProperty("secureIdentifierSize", value); }
        }

        /// <summary>
        /// Gets or sets the size of a secure identifier.
        /// </summary>
        /// <value>The size of the secure identifier.</value>
        [ConfigurationProperty("secureIdentifierDigits", DefaultValue =null, IsRequired = false)]
        [CanBeNull]
        public string SecureIdentifierDigits
        {
            get { return GetProperty<string>("secureIdentifierDigits"); }
            set { SetProperty("secureIdentifierDigits", value); }
        }

        /// <summary>
        /// Gets or sets the size of a secure identifier.
        /// </summary>
        /// <value>The size of the secure identifier.</value>
        [ConfigurationProperty("secureIdentifierIsCaseSensitive", DefaultValue = true, IsRequired = false)]
        public bool SecureIdentifierIsCaseSensitive
        {
            get { return GetProperty<bool>("secureIdentifierIsCaseSensitive"); }
            set { SetProperty("secureIdentifierIsCaseSensitive", value); }
        }

        /// <summary>
        /// Gets the provider with the specified identity.
        /// </summary>
        /// <param name="providerId">The provider identifier.</param>
        /// <returns>A <see cref="CryptographyProvider"/>.</returns>
        [CanBeNull]
        public CryptographyProvider GetProvider([NotNull] string providerId) => Providers[providerId]?.GetProvider();

        /// <summary>
        /// Gets the provider with the specified key, or creates one and saves it to the configuration.
        /// </summary>
        /// <param name="providerId">The provider identifier.</param>
        /// <param name="addFunc">The add function.</param>
        /// <returns>A <see cref="CryptographyProvider"/>.</returns>
        /// <exception cref="CryptographicException">The <paramref name="addFunc"/> returns null.</exception>
        /// <exception cref="Exception">The <paramref name="addFunc"/> throws an exception.</exception>
        /// <exception cref="ArgumentException">Invalid <paramref name="providerId"/> supplied.</exception>
        [NotNull]
        public CryptographyProvider GetOrAddProvider(
            [NotNull] string providerId,
            [NotNull] Func<CryptographyProvider> addFunc)
        {
            if (string.IsNullOrWhiteSpace(providerId))
                throw new ArgumentException(
                    Resources.CryptographyConfiguration_GetOrAddProvider_Invalid_Provider_ID,
                    nameof(providerId));

            // ReSharper disable ExceptionNotDocumented
            ProviderElement providerElement = Providers[providerId];
            CryptographyProvider provider;
            if (providerElement != null)
            {
                // Enable the provider if disabled.
                if (!providerElement.IsEnabled) providerElement.IsEnabled = true;

                // Try to get the provider
                provider = providerElement.GetProvider();
                if (provider != null)
                {
                    // Set the provider's ID so it knows it came from the configuration.
                    provider.Id = providerId;
                    return provider;
                }
            }

            // Create a new provider
            provider = addFunc();
            if (provider == null)
                throw new CryptographicException(
                    Resources.CryptographyConfiguration_GetOrAddProvider_Add_Returned_Null);

            // Set the provider's ID so it knows it came from the configuration.
            provider.Id = providerId;
            
            if (providerElement == null)
            {
                providerElement = new ProviderElement
                {
                    Id = providerId,
                    Name = provider.Name,
                    Configuration = provider.Configuration,
                    IsEnabled = true
                };
                Providers.Add(providerElement);
            }
            else
            {
                providerElement.Id = providerId;
                providerElement.Name = provider.Name;
                providerElement.Configuration = provider.Configuration;
                providerElement.IsEnabled = true;
            }

            // Save this configuration
            Save();

            // Return the provider
            return provider;
        }
    }
}