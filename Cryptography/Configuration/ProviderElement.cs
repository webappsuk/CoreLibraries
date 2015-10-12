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
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Threading;
using WebApplications.Utilities.Annotations;
using ConfigurationElement = WebApplications.Utilities.Configuration.ConfigurationElement;

namespace WebApplications.Utilities.Cryptography.Configuration
{
    /// <summary>
    /// A crypto provider element.
    /// </summary>
    [PublicAPI]
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class ProviderElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the provider ID.
        /// </summary>
        /// <value>
        /// The provider ID.
        /// </value>
        [ConfigurationProperty("id", IsRequired = true, IsKey = true)]
        [NotNull]
        public string Id
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            get { return GetProperty<string>("id"); }
            set { SetProperty("id", value); }
        }

        /// <summary>
        /// Gets or sets the type of the provider.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [ConfigurationProperty("type", IsRequired = true)]
        [TypeConverter(typeof(TypeNameConverter))]
        [SubclassTypeValidator(typeof(ICryptoProvider))]
        [NotNull]
        public Type Type
        {
            get { return GetProperty<Type>("type"); }
            set { SetProperty("type", value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this provider is enabled.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this provider is enabled; otherwise, <see langword="false" />.
        /// </value>
        [ConfigurationProperty("enabled", DefaultValue = true, IsRequired = false)]
        public bool IsEnabled
        {
            get { return GetProperty<bool>("enabled"); }
            set { SetProperty("enabled", value); }
        }

        /// <summary>
        /// Gets or sets the provider name.
        /// </summary>
        /// <value>
        /// The provider name.
        /// </value>
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        [NotNull]
        public string Name
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            get { return GetProperty<string>("name"); }
            set { SetProperty("name", value); }
        }

        /// <summary>
        /// Gets or sets the life of the key in days.
        /// </summary>
        /// <value>
        /// The key life in days.
        /// </value>
        [ConfigurationProperty("keyLifeInDays", IsRequired = false, DefaultValue = 7)]
        public int KeyLifeInDays
        {
            get { return GetProperty<int>("keyLifeInDays"); }
            set { this["keyLifeInDays"] = value; }
        }

        /// <summary>
        /// Gets or sets the keys for this provider.
        /// </summary>
        /// <value>
        /// The keys.
        /// </value>
        [ConfigurationProperty("keys", IsRequired = false, IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(KeyCollection),
            CollectionType = ConfigurationElementCollectionType.BasicMapAlternate)]
        [NotNull]
        [ItemNotNull]
        public virtual KeyCollection Keys
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            get { return GetProperty<KeyCollection>("keys"); }
            set { this["keys"] = value; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Configuration.ConfigurationElement" /> object is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Configuration.ConfigurationElement" /> object is read-only; otherwise, false.
        /// </returns>
        public override bool IsReadOnly()
        {
            return false;
        }

        private ICryptoProvider _provider;

        /// <summary>
        /// Gets an instance of this provider.
        /// </summary>
        /// <value>An instance of this provider.</value>
        /// <exception cref="System.Configuration.ConfigurationErrorsException" accessor="get">The provider is not <see cref="IsEnabled">enabled</see>.</exception>
        [NotNull]
        public ICryptoProvider Provider
        {
            get
            {
                if (!IsEnabled)
                    throw new ConfigurationErrorsException(
                        // ReSharper disable once AssignNullToNotNullAttribute
                        string.Format(Resources.CryptoProviderWrapper_Constructor_ProviderNotEnabled, Id));

                ICryptoProvider provider = _provider;
                if (provider != null) return provider;


                // Get keys to pass through to the provider constructor.
                Key[] keys = Keys
                    .Select(key => new Key(key.Value, key.Expiry))
                    .ToArray();

                // Create provider
                provider = Type
                    .ConstructorFunc<ProviderElement, IEnumerable<Key>, ICryptoProvider>()
                    (this, keys.Length > 0 ? keys : null);

                // Only set a new provider if there isn't already one set.
                return Interlocked.CompareExchange(ref _provider, provider, null) ?? provider;
            }
        }


        /// <summary>
        /// Adds a new encryption key to the configuration and saves.
        /// </summary>
        /// <param name="newKey">The new key to add to the configuration file.</param>
        internal void AddKey([NotNull] Key newKey)
        {
            if (newKey == null) throw new ArgumentNullException("newKey");

            //_aesEncryptionKeys.Add(newKey);
            //_aesEncryptionKeys = _aesEncryptionKeys.OrderByDescending(k => k.Expiry).ToList();

            //// If the ProviderElement is null we cannot save to the configuration.
            //if (_provider == null)
            //    return;

            // Create a key element to add to the provider element.
            KeyElement newKeyElement = new KeyElement
            {
                Value = newKey.Value,
                Expiry = newKey.Expiry
            };

            Keys.Add(newKeyElement);

            System.Configuration.Configuration currentConfiguration = CurrentConfiguration;
            if (currentConfiguration?.HasFile == true)
                currentConfiguration.Save(ConfigurationSaveMode.Modified);
        }
    }
}