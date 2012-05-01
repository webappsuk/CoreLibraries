using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;

namespace WebApplications.Utilities.Cryptography.Test
{
    [TestClass]
    public class RSACryptographerTests : TestBase
    {
        private const string InputString = "I do not like them sam-I-am I do not like green eggs and ham.";
        private readonly CryptoProviderWrapper _providerWrapper = new CryptoProviderWrapper("1");

        [TestMethod]
        public void Encrypt_InputString_SuccessfulEncryption()
        {
            string encrypted = _providerWrapper.Encrypt(InputString);

            Trace.WriteLine(encrypted);
            Assert.IsTrue(encrypted != null, "Encrypt method did not return a valid non-null string");
        }

        [TestMethod]
        public void Encrypt_WhiteSpace_SuccessfulEncryption()
        {
            string encrypted = _providerWrapper.Encrypt(" ");

            Trace.WriteLine(encrypted);
            Assert.IsTrue(encrypted != null, "Encrypt method did not return a valid non-null string");
        }

        [TestMethod]
        public void Encrypt_LongString_SuccessfulEncryption()
        {
            string input = GenerateRandomString(5000, false);
            string encrypted = _providerWrapper.Encrypt(input);

            Trace.WriteLine(encrypted);
            Assert.IsTrue(encrypted != null, "Encrypt method did not return a valid non-null string");
        }

        [TestMethod]
        public void Encrypt_UnicodeString_SuccessfulEncryption()
        {
            string input = GenerateRandomString(10);
            string encrypted = _providerWrapper.Encrypt(input);

            Trace.WriteLine(encrypted);
            Assert.IsTrue(encrypted != null, "Encrypt method did not return a valid non-null string");
        }

        [TestMethod]
        public void Encrypt_OneCharacterString_SuccessfulEncryption()
        {
            string encrypted = _providerWrapper.Encrypt("!");

            Trace.WriteLine(encrypted);
            Assert.IsTrue(encrypted != null, "Encrypt method did not return a valid non-null string");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Encrypt_NullInput_EmptyStringResult()
        {
            _providerWrapper.Encrypt(null);

            Assert.Fail("ArgumentNullException was expected when using a null input");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Encrypt_EmptyString_ArgumentNullException()
        {
            _providerWrapper.Encrypt(string.Empty);

            Assert.Fail("ArgumentNullException was expected when using string.Empty");
        }

        [TestMethod]
        public void Encrypt_SameTwoInputStrings_DifferentEcryptionResult()
        {
            string encryptedResult1 = _providerWrapper.Encrypt(InputString);
            Trace.WriteLine(encryptedResult1);

            string encryptedResult2 = _providerWrapper.Encrypt(InputString);
            Trace.WriteLine(encryptedResult2);

            Assert.IsFalse(encryptedResult1 == encryptedResult2, "The same string should result in a different encryption result");
        }

        [TestMethod]
        public void Encrypt_SameTwoUnicodeInputStrings_DifferentEcryptionResult()
        {
            string input = GenerateRandomString(10);

            string encryptedResult1 = _providerWrapper.Encrypt(input);
            Trace.WriteLine(encryptedResult1);

            string encryptedResult2 = _providerWrapper.Encrypt(input);
            Trace.WriteLine(encryptedResult2);

            Assert.IsFalse(encryptedResult1 == encryptedResult2, "The same string should result in a different encryption result");
        }

        [TestMethod]
        public void Decrypt_SameTwoInputStrings_SameDecryptionResult()
        {
            string input = GenerateRandomString(10, false);

            string encryptedResult1 = _providerWrapper.Encrypt(input);
            Trace.WriteLine("Encrypted A: " + encryptedResult1);

            string encryptedResult2 = _providerWrapper.Encrypt(input);
            Trace.WriteLine("Encrypted B: " + encryptedResult2 + Environment.NewLine);

            bool isLatestKey;

            string decryptedResult1 = _providerWrapper.Decrypt(encryptedResult1, out isLatestKey);
            Trace.WriteLine("Decrypted A: " + decryptedResult1);

            string decryptedResult2 = _providerWrapper.Decrypt(encryptedResult2, out isLatestKey);
            Trace.WriteLine("Decrypted B: " + decryptedResult2);

            Assert.AreEqual(decryptedResult1, decryptedResult2, "The same input strings should result in the same decryption result");
        }

        [TestMethod]
        public void Decrypt_SameTwoUnicodeInputStrings_SameDecryptionResult()
        {
            string input = GenerateRandomString(10);

            string encryptedResult1 = _providerWrapper.Encrypt(input);
            Trace.WriteLine("Encrypted A: " + encryptedResult1);

            string encryptedResult2 = _providerWrapper.Encrypt(input);
            Trace.WriteLine("Encrypted B: " + encryptedResult2 + Environment.NewLine);

            bool isLatestKey;

            string decryptedResult1 = _providerWrapper.Decrypt(encryptedResult1, out isLatestKey);
            Trace.WriteLine("Decrypted A: " + decryptedResult1);

            string decryptedResult2 = _providerWrapper.Decrypt(encryptedResult2, out isLatestKey);
            Trace.WriteLine("Decrypted B: " + decryptedResult2);

            Assert.AreEqual(decryptedResult1, decryptedResult2, "The same input strings should result in the same decryption result");
        }

        [TestMethod]
        public void Decrypt_DecryptedResult_MatchesInputString()
        {
            string encrypted = _providerWrapper.Encrypt(InputString);

            bool isLatestKey;
            string decrypted = _providerWrapper.Decrypt(encrypted, out isLatestKey);

            Trace.WriteLine(decrypted);
            Assert.AreEqual(InputString, decrypted, "decrypted text did not match the provided input");
        }

        [TestMethod]
        public void Decrypt_UnicodeString_SuccessfulDecryption()
        {
            string input = GenerateRandomString(10);
            string encrypted = _providerWrapper.Encrypt(input);

            bool isLatestKey;
            string decrypted = _providerWrapper.Decrypt(encrypted, out isLatestKey);

            Trace.WriteLine(input);
            Trace.WriteLine(decrypted);
            Assert.AreEqual(input, decrypted, "decrypted text did not match the provided input");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Decrypt_NullInput_ArgumentNullException()
        {
            bool isLatestKey;
            _providerWrapper.Decrypt(null, out isLatestKey);

            Assert.Fail("ArgumentNullException was expected when using a null input");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Decrypt_EmptyString_ArgumentNullException()
        {
            bool isLatestKey;
            _providerWrapper.Decrypt(string.Empty, out isLatestKey);

            Assert.Fail("ArgumentNullException was expected when using string.Empty");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Decrypt_NullInput_LatestKeyIsFalse()
        {
            bool isLatestKey;
            _providerWrapper.Decrypt(null, out isLatestKey);

            Assert.IsFalse(isLatestKey, "For a null input string 'isLatestKey' output should be false");
        }

        [TestMethod]
        [ExpectedException(typeof(CryptographicException))]
        public void Decrypt_InputEncryptedWithKeyNotInConfig_CryptographicException()
        {
            const string encryptedStringNotUsingKeyInConfiguration = "I0Qu6rZYA2OB1FXhhGB8J6+VyeEVVAvZkaYBKi8j4XY23ecgN1Zpprhzcrql7U6eUKZWtPaxfDwoEa9g6Bq0f1uE1pVMhrlxQDw3n6KFI5chvFLFpMA85APth08F2yzEh1yjXj6iynV9ZZlGyUQ/+lMJY0fjg45fNnv23C4aFbM=";

            bool isLatestKey;
            string decrypted = _providerWrapper.Decrypt(encryptedStringNotUsingKeyInConfiguration, out isLatestKey);

            Trace.WriteLine("Result: " + decrypted);

            Assert.Fail("CryptographicException was expected when using an encrypted string that does not exist within our configuration");
        }

        [TestMethod]
        public void TryDecrypt_ReturnValue_ReturnsFalseWhenKeyIsNotLatestKey()
        {
            const string encryptedStringUsingExpiredKeyInConfiguration = "CkTj6c4LPWXjfAxHKpuITqXFaerCiZ9rfAzyf8FS/5qYWbQ1HMGsADO6rF7fuAljjvfCM5HoYvZe7zBAjxU2kVfuVmaHKGGYJyrtjwKvRURXwkXgUUO8HanpJtU4UjvO0AU3sBgJCc5NUXS/tU9oT4D/SbaHcvQUtFfThAiuT0w=";

            string decryptedString;
            bool? isLatestKey;

            _providerWrapper.Encrypt("a new key will be made now");
            bool decrypted = _providerWrapper.TryDecrypt(encryptedStringUsingExpiredKeyInConfiguration, out decryptedString, out isLatestKey);

            Assert.IsTrue(decrypted, "'decrypted' should return true");
            Assert.IsFalse(isLatestKey.Value, "IsLatestKey should return false");
        }

        [TestMethod]
        public void TryDecrypt_NullInputString_FalseReturned()
        {
            string decryptedString;
            bool? isLatestKey;

            bool decrypted = _providerWrapper.TryDecrypt(null, out decryptedString, out isLatestKey);

            Assert.IsFalse(decrypted, "TryDecrypt should return false when using null input string");
        }
    }
}