using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;

namespace WebApplications.Utilities.Cryptography.Test
{
    [TestClass]
    public class SymmetricProviderTests : CryptographyTestBase
    {
        [TestMethod]
        public void TestAes()
        {
            CryptographyProvider provider = CryptographyProvider.Create("AES");
            string random = Tester.RandomString(minLength: 10);
            string encrypted = provider.EncryptToString(random);
            Assert.IsNotNull(encrypted);
            Assert.AreNotEqual(random, encrypted);

            string decrypted = provider.DecryptToString(encrypted);
            Assert.IsNotNull(decrypted);
            Assert.AreEqual(random, decrypted);
        }
    }
}