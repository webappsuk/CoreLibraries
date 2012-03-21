using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;
using WebApplications.Utilities.Cryptography.Configuration;

namespace WebApplications.Utilities.Cryptography.Test
{
    [TestClass]
    public class CryptographyConfigurationTests : TestBase
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

    [TestClass]
    public class ProviderElementTests : TestBase
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
            Random random = new Random();
            int randomInt = random.Next(0, 100);
            _providerElement.KeyLifeInDays = randomInt;

            Assert.AreEqual(randomInt, _providerElement.KeyLifeInDays);
        }

        [TestMethod]
        public void Type_Set_SetsValue()
        {
            string randomString = GenerateRandomString(10);
            _providerElement.Id = randomString;

            Assert.AreEqual(randomString, _providerElement.Id);
        }
    }

    [TestClass]
    public class KeyElementTests : TestBase
    {
        private ProviderElement _providerElement;

        [TestInitialize]
        public void Initialize()
        {
            CryptographyConfiguration configuration = CryptographyConfiguration.Active;
            _providerElement = configuration.Providers[1];
        }

        [TestMethod]
        public void Value_ReturnsInstance()
        {
            string value = _providerElement.Keys.First().Value;
            Trace.Write(value);
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void Expiry_ReturnsInstance()
        {
            DateTime expiry = _providerElement.Keys.First().Expiry;
            Trace.Write(expiry);
            Assert.IsNotNull(expiry);
        }
    }
}