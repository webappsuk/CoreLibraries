using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Cryptography.Configuration;

namespace WebApplications.Utilities.Cryptography.Test
{
    [TestClass]
    public class KeyElementTests : SerializationTestBase
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