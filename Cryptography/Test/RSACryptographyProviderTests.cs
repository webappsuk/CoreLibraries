using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using WebApplications.Testing;

namespace WebApplications.Utilities.Cryptography.Test
{
    [TestClass]
    public class RSACryptographyProviderTests
    {

        public void TestEncryptDecrypt(int keySize, Func<int, int, int> getLengthFunc, int blocks)
        {
            RSACryptographyProvider provider = RSACryptographyProvider.Create(keySize);
            Assert.IsTrue(provider.CanEncrypt);
            Trace.WriteLine($"Input Block Size:{provider.InputBlockSize}; OutputBlockSize:{provider.OutputBlockSize}");

            // Generate random input.            
            int length = getLengthFunc(provider.InputBlockSize, provider.OutputBlockSize);

            byte[] input = new byte[length];
            if (length > 0)
                Tester.RandomGenerator.NextBytes(input);

            byte[] encryptedBytes = provider.Encrypt(input);
            Assert.IsNotNull(encryptedBytes);

            // Check we got the correct number of blocks.
            Assert.AreEqual(0, encryptedBytes.LongLength % provider.OutputBlockSize);
            Assert.AreEqual(blocks, encryptedBytes.LongLength / provider.OutputBlockSize);

            Assert.IsTrue(provider.CanDecrypt);
            byte[] output = provider.Decrypt(encryptedBytes);
            Assert.IsNotNull(output);
            Assert.AreEqual(length, output.LongLength);
            CollectionAssert.AreEqual(input, output);
        }

        [TestMethod]
        public void Test_Null_1024()
        {
            RSACryptographyProvider provider = RSACryptographyProvider.Create(1024);
            Assert.IsTrue(provider.CanEncrypt);
            Assert.IsNull(provider.Encrypt((byte[])null));
            Assert.IsTrue(provider.CanDecrypt);
            Assert.IsNull(provider.Decrypt((byte[])null));
        }

        [TestMethod]
        public void Test_Zero_Length_1024()
        {
            TestEncryptDecrypt(1024, (i, o) => 0, 1);
        }

        [TestMethod]
        public void Test_Less_Than_One_Block_1024()
        {
            TestEncryptDecrypt(1024, (i, o) => i - 2, 1);
        }

        [TestMethod]
        public void Test_Exactly_One_Block_1024()
        {
            TestEncryptDecrypt(1024, (i, o) => i - 1, 1);
        }

        [TestMethod]
        public void Test_Just_Two_Blocks_1024()
        {
            TestEncryptDecrypt(1024, (i, o) => i, 2);
        }

        [TestMethod]
        public void Test_Null_2048()
        {
            RSACryptographyProvider provider = RSACryptographyProvider.Create(2048);
            Assert.IsTrue(provider.CanEncrypt);
            Assert.IsNull(provider.Encrypt((byte[])null));
            Assert.IsTrue(provider.CanDecrypt);
            Assert.IsNull(provider.Decrypt((byte[])null));
        }

        [TestMethod]
        public void Test_Zero_Length_2048()
        {
            TestEncryptDecrypt(2048, (i, o) => 0, 1);
        }

        [TestMethod]
        public void Test_Less_Than_One_Block_2048()
        {
            TestEncryptDecrypt(2048, (i, o) => i - 2, 1);
        }

        [TestMethod]
        public void Test_Exactly_One_Block_2048()
        {
            TestEncryptDecrypt(2048, (i, o) => i - 1, 1);
        }

        [TestMethod]
        public void Test_Just_Two_Blocks_2048()
        {
            TestEncryptDecrypt(2048, (i, o) => i, 2);
        }
    }
}
