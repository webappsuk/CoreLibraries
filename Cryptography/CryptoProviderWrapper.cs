using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using WebApplications.Utilities.Cryptography.Configuration;

namespace WebApplications.Utilities.Cryptography
{
    public class CryptoProviderWrapper
    {
        private readonly IEncryptorDecryptor _encryptorDecryptor;

        public CryptoProviderWrapper(string id = null)
        {
            CryptographyConfiguration configuration = CryptographyConfiguration.Active;

            if (id == null || configuration == null)
            {
                // Default to RSA.
                _encryptorDecryptor = new RSACryptographer();
            }
            else
            {
                ProviderElement providerElement = configuration.Providers.SingleOrDefault(provider => provider.Id == id);

                if (providerElement == null)
                {
                    // If no element is found, defaults to RSA.
                    _encryptorDecryptor = new RSACryptographer();

                    return;
                }

                if (!providerElement.IsEnabled)
                    throw new ConfigurationErrorsException(string.Format(Resources.CryptoProviderWrapper_Constructor_ProviderNotEnabled, id));

                // Get keys to pass through to the provider constructor.
                List<Key> keys = providerElement.Keys.Select(key => new Key { Value = key.Value, Expiry = key.Expiry }).ToList();

                _encryptorDecryptor = providerElement
                    .Type
                    .ConstructorFunc<ProviderElement, IEnumerable<Key>, IEncryptorDecryptor>()(providerElement, keys.Count > 0 ? keys : null);
            }
        }

        public string Encrypt(string input)
        {
            return _encryptorDecryptor.Encrypt(input);
        }

        public string Decrypt(string input, out bool isLatestKey)
        {
            bool l;

            string result = _encryptorDecryptor.Decrypt(input, out l);
            isLatestKey = l;

            return result;
        }

        public bool TryDecrypt(string input, out string result, out bool? isLatestKey)
        {
            string r;
            bool? l;

            bool success = _encryptorDecryptor.TryDecrypt(input, out r, out l);
            result = r;
            isLatestKey = l;

            return success;
        }
    }
}