using System;
using System.ComponentModel;
using System.Configuration;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Cryptography.Configuration
{
    public class ProviderElement : Utilities.Configuration.ConfigurationElement
    {
        [ConfigurationProperty("id", IsRequired = true, IsKey = true)]
        [NotNull]
        public string Id
        {
            get { return GetProperty<string>("id"); }
            set { SetProperty("id", value); }
        }

        [ConfigurationProperty("type")]
        [TypeConverter(typeof(TypeNameConverter))]
        [SubclassTypeValidator(typeof(IEncryptorDecryptor))]
        public Type Type
        {
            get { return GetProperty<Type>("type"); } 
            set { SetProperty("type", value); }
        }

        [ConfigurationProperty("enabled", DefaultValue = true, IsRequired = false)]
        public bool IsEnabled
        {
            get { return GetProperty<bool>("enabled"); }
            set { SetProperty("enabled", value); }
        }

        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        [NotNull]
        public string Name
        {
            get { return GetProperty<string>("name"); }
            set { SetProperty("name", value); }
        }

        [ConfigurationProperty("keyLifeInDays", IsRequired = false, DefaultValue = 7)]
        public int KeyLifeInDays
        {
            get { return GetProperty<int>("keyLifeInDays"); }
            set { this["keyLifeInDays"] = value; }
        }

        [ConfigurationProperty("keys", IsRequired = false, IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(KeyCollection), CollectionType = ConfigurationElementCollectionType.BasicMapAlternate)]
        [NotNull]
        [UsedImplicitly]
        public virtual KeyCollection Keys
        {
            get { return GetProperty<KeyCollection>("keys"); }
            set { this["keys"] = value; }
        }

        public override bool IsReadOnly()
        {
            return false;
        }
    }
}