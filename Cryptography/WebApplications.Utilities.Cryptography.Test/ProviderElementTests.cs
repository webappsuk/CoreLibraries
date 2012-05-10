using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;
using WebApplications.Utilities.Cryptography.Configuration;

namespace WebApplications.Utilities.Cryptography.Test
{
    [TestClass]
    public class ProviderElementTests : SerializationTestBase
    {
        private ProviderElement _providerElement;

        [TestInitialize]
        public void Initialize()
        {
            CryptographyConfiguration configuration = CryptographyConfiguration.Active;
            _providerElement = configuration.Providers[0];
        }

        [TestMethod]
        public void Id_ReturnsInstance()
        {
            string id = _providerElement.Id;
            Assert.IsNotNull(id);
        }

        [TestMethod]
        public void Keys_ReturnsInstance()
        {
            KeyCollection keys = _providerElement.Keys;
            Assert.IsNotNull(keys);
        }

        [TestMethod]
        public void KeyLifeInDays_ReturnsValue()
        {
            Assert.IsNotNull(_providerElement.KeyLifeInDays);
        }

        [TestMethod]
        public void KeyLifeInDays_Set_SetsValue()
        {
            int randomInt = Random.Next(0, 100);
            _providerElement.KeyLifeInDays = randomInt;

            Assert.AreEqual(randomInt, _providerElement.KeyLifeInDays);
        }

        [TestMethod]
        public void Type_Set_SetsValue()
        {
            string randomString = Random.RandomString(10);
            _providerElement.Id = randomString;

            Assert.AreEqual(randomString, _providerElement.Id);
        }
    }
}