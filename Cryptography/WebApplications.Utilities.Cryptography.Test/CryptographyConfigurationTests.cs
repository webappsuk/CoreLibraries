using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Cryptography.Configuration;

namespace WebApplications.Utilities.Cryptography.Test
{
    [TestClass]
    public class CryptographyConfigurationTests : SerializationTestBase
    {
        [TestMethod]
        public void Active_ReturnsActiveInstance()
        {
            CryptographyConfiguration configuration = CryptographyConfiguration.Active;
            Assert.IsNotNull(configuration);
        }

        [TestMethod]
        public void Providers_Get_ReturnsProvidersElement()
        {
            CryptographyConfiguration configuration = CryptographyConfiguration.Active;
            ProviderCollection providers = configuration.Providers;
            Assert.IsNotNull(providers);
        }
    }
}