using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using WebApplications.Testing;

namespace WebApplications.Utilities.Cryptography.Test
{
    [TestClass]
    public class RSACryptographyProviderTests
    {
        [TestMethod]
        public void Test_Encrypt_With_Length_Less_Than_One_Block()
        {
            RSACryptographyProvider provider = new RSACryptographyProvider();
            Assert.IsTrue(provider.CanEncrypt);
            Assert.IsTrue(provider.EncodeLength);

            // Generate random input.            
            byte[] input = new byte[provider.InputBlockSize - 2];
            Assert.IsTrue(input.LongLength < 128, "The input length has to be less than 128, otherwise we will encode a length greater than 127, which will take more than 1 byte to encode.");
            Tester.RandomGenerator.NextBytes(input);

            byte[] encryptedBytes = provider.Encrypt(input);
            Assert.IsNotNull(encryptedBytes);

            // We expect exactly one block back
            Assert.AreEqual(0, encryptedBytes.LongLength % provider.OutputBlockSize);
            Assert.AreEqual(1, encryptedBytes.LongLength / provider.OutputBlockSize);

            Assert.IsTrue(provider.CanDecrypt);
            byte[] output = provider.Decrypt(encryptedBytes);
            Assert.IsNotNull(output);
            Assert.AreEqual(input.LongLength, output.LongLength);
            Assert.AreNotEqual(0, output.LongLength % provider.InputBlockSize);
            CollectionAssert.AreEqual(input, output);
        }

        [TestMethod]
        public void Test_Encrypt_Without_Length_Less_Than_One_Block()
        {
            RSACryptographyProvider provider = new RSACryptographyProvider(encodeLength: false);
            Assert.IsTrue(provider.CanEncrypt);
            Assert.IsFalse(provider.EncodeLength);

            // Generate random input.            
            byte[] input = new byte[provider.InputBlockSize - 1];
            Tester.RandomGenerator.NextBytes(input);

            byte[] encryptedBytes = provider.Encrypt(input);
            Assert.IsNotNull(encryptedBytes);

            // We expect exactly one block back
            Assert.AreEqual(0, encryptedBytes.LongLength % provider.OutputBlockSize);
            Assert.AreEqual(1, encryptedBytes.LongLength / provider.OutputBlockSize);

            Assert.IsTrue(provider.CanDecrypt);
            byte[] output = provider.Decrypt(encryptedBytes);
            Assert.IsNotNull(output);
            Assert.AreEqual(input.LongLength, output.LongLength-1, "We expect the output to be padded to the nearest block.");
            Assert.AreEqual(0, output.LongLength % provider.InputBlockSize);
            Assert.AreEqual(1, output.LongLength / provider.InputBlockSize);
            CollectionAssert.AreEqual(input,output.Take(input.Length).ToArray());
        }

        [TestMethod]
        public void Test_Encrypt_With_Length_Exactly_One_Block()
        {
            RSACryptographyProvider provider = new RSACryptographyProvider();
            Assert.IsTrue(provider.CanEncrypt);
            Assert.IsTrue(provider.EncodeLength);

            // Generate random input.            
            byte[] input = new byte[provider.InputBlockSize - 1];
            Assert.IsTrue(input.LongLength < 128, "The input length has to be less than 128, otherwise we will encode a length greater than 127, which will take more than 1 byte to encode.");
            Tester.RandomGenerator.NextBytes(input);

            byte[] encryptedBytes = provider.Encrypt(input);
            Assert.IsNotNull(encryptedBytes);

            // We expect exactly one block back
            Assert.AreEqual(0, encryptedBytes.LongLength % provider.OutputBlockSize);
            Assert.AreEqual(1, encryptedBytes.LongLength / provider.OutputBlockSize);

            Assert.IsTrue(provider.CanDecrypt);
            byte[] output = provider.Decrypt(encryptedBytes);
            Assert.IsNotNull(output);
            Assert.AreEqual(input.LongLength, output.LongLength);
            CollectionAssert.AreEqual(input, output);
        }
        [TestMethod]
        public void Test_Encrypt_Without_Length_Exactly_One_Block()
        {
            RSACryptographyProvider provider = new RSACryptographyProvider(encodeLength: false);
            Assert.IsTrue(provider.CanEncrypt);
            Assert.IsFalse(provider.EncodeLength);

            // Generate random input.            
            byte[] input = new byte[provider.InputBlockSize];
            Tester.RandomGenerator.NextBytes(input);

            byte[] encryptedBytes = provider.Encrypt(input);
            Assert.IsNotNull(encryptedBytes);

            // We expect exactly one block back
            Assert.AreEqual(0, encryptedBytes.LongLength % provider.OutputBlockSize);
            Assert.AreEqual(1, encryptedBytes.LongLength / provider.OutputBlockSize);

            Assert.IsTrue(provider.CanDecrypt);
            byte[] output = provider.Decrypt(encryptedBytes);
            Assert.IsNotNull(output);
            Assert.AreEqual(input.LongLength, output.LongLength);
            CollectionAssert.AreEqual(input, output);
        }

        [TestMethod]
        public void Test_Encrypt_With_Length_Just_Two_Blocks()
        {
            RSACryptographyProvider provider = new RSACryptographyProvider();
            Assert.IsTrue(provider.CanEncrypt);
            Assert.IsTrue(provider.EncodeLength);

            // Generate random input, which is exactly input block size, so the header will force into a second block.            
            byte[] input = new byte[provider.InputBlockSize];
            Assert.IsTrue(input.LongLength < 128, "The input length has to be less than 128, otherwise we will encode a length greater than 127, which will take more than 1 byte to encode.");
            Tester.RandomGenerator.NextBytes(input);

            byte[] encryptedBytes = provider.Encrypt(input);
            Assert.IsNotNull(encryptedBytes);

            // We expect exactly two blocks back
            Assert.AreEqual(0, encryptedBytes.LongLength % provider.OutputBlockSize);
            Assert.AreEqual(2, encryptedBytes.LongLength / provider.OutputBlockSize);


            Assert.IsTrue(provider.CanDecrypt);
            byte[] output = provider.Decrypt(encryptedBytes);
            Assert.IsNotNull(output);
            Assert.AreEqual(input.LongLength, output.LongLength);
            CollectionAssert.AreEqual(input, output);
        }

        [TestMethod]
        public void Test_Encrypt_Without_Length_Just_Two_Blocks()
        {
            RSACryptographyProvider provider = new RSACryptographyProvider(encodeLength:false);
            Assert.IsTrue(provider.CanEncrypt);
            Assert.IsFalse(provider.EncodeLength);

            // Generate random input, which is exactly input block size, so the header will force into a second block.            
            byte[] input = new byte[provider.InputBlockSize+1];
            Assert.IsTrue(input.LongLength < 128, "The input length has to be less than 128, otherwise we will encode a length greater than 127, which will take more than 1 byte to encode.");
            Tester.RandomGenerator.NextBytes(input);

            byte[] encryptedBytes = provider.Encrypt(input);
            Assert.IsNotNull(encryptedBytes);

            // We expect exactly two blocks back
            Assert.AreEqual(0, encryptedBytes.LongLength % provider.OutputBlockSize);
            Assert.AreEqual(2, encryptedBytes.LongLength / provider.OutputBlockSize);


            Assert.IsTrue(provider.CanDecrypt);
            byte[] output = provider.Decrypt(encryptedBytes);
            Assert.IsNotNull(output);
            Assert.AreNotEqual(input.LongLength, output.LongLength, "We expect the output to be padded to the nearest block.");
            Assert.AreEqual(0, output.LongLength % provider.InputBlockSize);
            Assert.AreEqual(2, output.LongLength / provider.InputBlockSize);
            CollectionAssert.AreEqual(input, output.Take(input.Length).ToArray());
        }
    }
}
