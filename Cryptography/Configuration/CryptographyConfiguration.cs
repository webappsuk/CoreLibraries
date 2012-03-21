using System;
using System.Configuration;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using WebApplications.Utilities.Configuration;

namespace WebApplications.Utilities.Cryptography.Configuration
{
    public class CryptographyConfiguration : ConfigurationSection<CryptographyConfiguration>
    {
        /// <summary>
        /// Gets or sets the <see cref="ProviderCollection">providers</see>.
        /// </summary>
        [ConfigurationProperty("providers", IsRequired = true, IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(ProviderCollection), CollectionType = ConfigurationElementCollectionType.BasicMapAlternate)]
        [NotNull]
        public ProviderCollection Providers
        {
            get { return GetProperty<ProviderCollection>("providers"); }
            set { SetProperty("providers", value); }
        }

        /// <summary>
        /// Used to initialize a default set of values for the <see cref="CryptographyConfiguration"/> object.
        /// </summary>
        protected override void InitializeDefault()
        {
            // ReSharper disable ConstantNullCoalescingCondition
            Providers = Providers ?? new ProviderCollection();
            // ReSharper restore ConstantNullCoalescingCondition
            base.InitializeDefault();
        }

        /// <summary>
        /// Gets the raw XML representation of this section.
        /// </summary>
        internal string RawXml
        {
            get
            {
                StringBuilder xml = new StringBuilder("<cryptography><providers>");

                foreach (ProviderElement provider in Providers)
                {
                    string providerString;

                    if (provider.Keys.Count <= 0)
                    {
                        providerString = string.Format(@"<add name=""{0}"" type=""{1}, {2}"" enabled=""{3}"" id=""{4}"" keyLifeInDays=""{5}""/>",
                                            provider.Name, provider.Type.SimpleFullName(),
                                            provider.Type.Assembly.SimpleFullName(), provider.IsEnabled, provider.Id, provider.KeyLifeInDays);
                    }
                    else
                    {
                        providerString = string.Format(@"<add name=""{0}"" type=""{1}, {2}"" enabled=""{3}"" id=""{4}"" keyLifeInDays=""{5}"">",
                                            provider.Name, provider.Type.SimpleFullName(),
                                            provider.Type.Assembly.SimpleFullName(), provider.IsEnabled, provider.Id, provider.KeyLifeInDays);

                        providerString += "<keys>";
                        providerString = provider
                            .Keys
                            .Aggregate(providerString, (current, key) => current +
                                string.Format(@"<add value=""{0}"" expiry=""{1}""/>", key.Value, key.Expiry.ToString("yyyy-MM-dd HH:mm:ss")));
                        providerString += "</keys></add>";
                    }

                    xml.Append(providerString);
                }

                xml.Append("</providers></cryptography>");

                return xml.ToString();
            }
        }
    }
}