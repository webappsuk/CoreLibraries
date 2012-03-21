using System;
using System.Configuration;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebApplications.Utilities.Cryptography.Test
{
    [TestClass]
    public class CryptoProviderWrapperTests
    {
        private readonly CryptoProviderWrapper _providerWrapper = new CryptoProviderWrapper("none");
        private const string InputString = "You spoony bard!";

        [TestMethod]
        public void IdNotInConfiguration_EncryptsSuccessfully()
        {
            string encrypted = _providerWrapper.Encrypt(InputString);
            Trace.WriteLine(encrypted);

            Assert.IsInstanceOfType(encrypted, typeof(string), "A provider with an ID not in the test configuration should create an RSA Cryptographer");
        }

        [TestMethod]
        public void IdNotInConfiguration_DecryptsSuccessfully()
        {
            string encrypted = _providerWrapper.Encrypt(InputString);

            bool isLatestKey;
            string decrypted = _providerWrapper.Decrypt(encrypted, out isLatestKey);
            Trace.WriteLine(decrypted);

            Assert.IsInstanceOfType(decrypted, typeof(string));
            Assert.AreEqual(decrypted, InputString, "the decrypted result should match the InputString");
        }

        [TestMethod]
        public void IdNotInConfiguration_TryDecryptsSuccessfully()
        {
            string encrypted = _providerWrapper.Encrypt(InputString);

            bool? isLatestKey;
            string decrypted;
            bool canDecrypt = _providerWrapper.TryDecrypt(encrypted, out decrypted, out isLatestKey);
            Trace.WriteLine(decrypted);

            Assert.IsTrue(canDecrypt);
        }

        [TestMethod]
        public void TwoRSAProviders_CannotDecryptEachOthersEncryption()
        {
            CryptoProviderWrapper providerB = new CryptoProviderWrapper("1");
            string encrypted = _providerWrapper.Encrypt(InputString);

            bool? isLatestKey;
            string decrypted;
            bool canDecrypt = providerB.TryDecrypt(encrypted, out decrypted, out isLatestKey);
            Trace.WriteLine(decrypted);

            Assert.IsFalse(canDecrypt);
        }

        [TestMethod]
        public void TwoAESProviders_CannotDecryptEachOthersEncryption()
        {
            CryptoProviderWrapper providerA = new CryptoProviderWrapper("2");
            CryptoProviderWrapper providerB = new CryptoProviderWrapper("4");

            string encrypted = providerA.Encrypt(InputString);

            bool? isLatestKey;
            string decrypted;
            bool canDecrypt = providerB.TryDecrypt(encrypted, out decrypted, out isLatestKey);
            Trace.WriteLine(decrypted);

            Assert.IsFalse(canDecrypt);
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void Constructor_DisabledProvider_ThrowsConfigurationErrorsException()
        {
            new CryptoProviderWrapper("3");

            Assert.Fail("'ConfigurationErrorsException' was expected when providing an id to a disabled provider");
        }
    }
}