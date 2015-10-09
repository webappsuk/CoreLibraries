#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Copyright (c) 2012, Web Applications UK Ltd
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Web Applications UK Ltd nor the
//       names of its contributors may be used to endorse or promote products
//       derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL WEB APPLICATIONS UK LTD BE LIABLE FOR ANY
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Security.Cryptography;
using WebApplications.Testing;

namespace WebApplications.Utilities.Cryptography.Test
{
    [TestClass]
    public class AESCryptographerTests : CryptographyTestBase
    {
        private const string InputString = "Password01";

        [TestMethod]
        public void Encrypt_NullInput()
        {
            Assert.IsNull(AES.Encrypt(null));
        }

        [TestMethod]
        public void Encrypt_WhiteSpace_SuccessfulEncryption()
        {
            string encrypted = AES.Encrypt(" ");

            Trace.WriteLine(encrypted);
            Assert.IsTrue(encrypted != null, "Encrypt method did not return a valid non-null string");
        }

        [TestMethod]
        public void Encrypt_SameTwoInputStrings_DifferentEcryptionResult()
        {
            string encryptedResult1 = AES.Encrypt(InputString);
            Trace.WriteLine(encryptedResult1);

            string encryptedResult2 = AES.Encrypt(InputString);
            Trace.WriteLine(encryptedResult2);

            Assert.IsFalse(encryptedResult1 == encryptedResult2,
                           "The same string should result in a different encryption result");
        }

        [TestMethod]
        public void Encrypt_SameTwoUnicodeInputStrings_DifferentEcryptionResult()
        {
            string input = Random.RandomString(10);

            string encryptedResult1 = AES.Encrypt(input);
            Trace.WriteLine(encryptedResult1);

            string encryptedResult2 = AES.Encrypt(input);
            Trace.WriteLine(encryptedResult2);

            Assert.IsFalse(encryptedResult1 == encryptedResult2,
                           "The same string should result in a different encryption result");
        }

        [TestMethod]
        public void Encrypt_EmptyString()
        {
            Assert.AreEqual(AES.Encrypt(string.Empty), string.Empty);
        }

        [TestMethod]
        public void Encrypt_OneCharacterString_SuccessfulEncryption()
        {
            string encrypted = AES.Encrypt("!");

            Trace.WriteLine(encrypted);
            Assert.IsTrue(encrypted != null, "Encrypt method did not return a valid non-null string");
        }

        [TestMethod]
        public void Encrypt_LongString_SuccessfulEncryption()
        {
            string input = Random.RandomString(5000, false);
            string encrypted = AES.Encrypt(input);

            Assert.IsTrue(encrypted != null, "Encrypt method did not return a valid non-null string");
        }

        [TestMethod]
        public void Encrypt_UnicodeString_SuccessfulEncryption()
        {
            string input = Random.RandomString(10);
            string encrypted = AES.Encrypt(input);

            Trace.WriteLine(encrypted);
            Assert.IsTrue(encrypted != null, "Encrypt method did not return a valid non-null string");
        }

        [TestMethod]
        public void Decrypt_LongString_SuccessfulDecryption()
        {
            string input = Random.RandomString(5000, false);
            string encrypted = AES.Encrypt(input);

            bool isLatestKey;
            string decrypted = AES.Decrypt(encrypted, out isLatestKey);

            Trace.WriteLine(string.Format("Input: {0}{1}{0}", Environment.NewLine, input));
            Trace.WriteLine(string.Format("Decrypted String: {0}{1}", Environment.NewLine, decrypted));
            Assert.AreEqual(input, decrypted, "decrypted text did not match the input");
        }

        [TestMethod]
        public void Decrypt_UnicodeString_SuccessfulDecryption()
        {
            string input = Random.RandomString(10);
            string encrypted = AES.Encrypt(input);

            bool isLatestKey;
            string decrypted = AES.Decrypt(encrypted, out isLatestKey);

            Trace.WriteLine(input);
            Trace.WriteLine(decrypted);
            Assert.AreEqual(input, decrypted, "decrypted text did not match the provided input");
        }

        [TestMethod]
        public void Decrypt_DecryptedResult_MatchesInputString()
        {
            string encrypted = AES.Encrypt(InputString);

            bool isLatestKey;
            string decrypted = AES.Decrypt(encrypted, out isLatestKey);

            Trace.WriteLine(decrypted);
            Assert.AreEqual(InputString, decrypted, "decrypted text did not match the provided input");
        }

        [TestMethod]
        public void Decrypt_SameTwoInputStrings_SameDecryptionResult()
        {
            string input = Random.RandomString(10, false);

            string encryptedResult1 = AES.Encrypt(input);
            Trace.WriteLine("Encrypted A: " + encryptedResult1);

            string encryptedResult2 = AES.Encrypt(input);
            Trace.WriteLine("Encrypted B: " + encryptedResult2 + Environment.NewLine);

            bool isLatestKey;

            string decryptedResult1 = AES.Decrypt(encryptedResult1, out isLatestKey);
            Trace.WriteLine("Decrypted A: " + decryptedResult1);

            string decryptedResult2 = AES.Decrypt(encryptedResult2, out isLatestKey);
            Trace.WriteLine("Decrypted B: " + decryptedResult2);

            Assert.AreEqual(decryptedResult1, decryptedResult2,
                            "The same input strings should result in the same decryption result");
        }

        [TestMethod]
        public void Decrypt_SameTwoUnicodeInputStrings_SameDecryptionResult()
        {
            string input = Random.RandomString(10);

            string encryptedResult1 = AES.Encrypt(input);
            Trace.WriteLine("Encrypted A: " + encryptedResult1);

            string encryptedResult2 = AES.Encrypt(input);
            Trace.WriteLine("Encrypted B: " + encryptedResult2 + Environment.NewLine);

            bool isLatestKey;

            string decryptedResult1 = AES.Decrypt(encryptedResult1, out isLatestKey);
            Trace.WriteLine("Decrypted A: " + decryptedResult1);

            string decryptedResult2 = AES.Decrypt(encryptedResult2, out isLatestKey);
            Trace.WriteLine("Decrypted B: " + decryptedResult2);

            Assert.AreEqual(decryptedResult1, decryptedResult2,
                            "The same input strings should result in the same decryption result");
        }

        [TestMethod]
        public void Decrypt_NullInput()
        {
            bool isLatestKey;
            Assert.IsNull(AES.Decrypt(null, out isLatestKey));
        }

        [TestMethod]
        public void Decrypt_EmptyString()
        {
            bool isLatestKey;
            Assert.AreEqual(AES.Decrypt(string.Empty, out isLatestKey), string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof (CryptographicException))]
        public void Decrypt_NonBase32EncodedString_CryptographicException()
        {
            bool isLatestKey;
            AES.Decrypt("hello world", out isLatestKey);

            Assert.Fail("CryptographicException was expected when using a non base32 encoded string");
        }

        [TestMethod]
        [ExpectedException(typeof (CryptographicException))]
        public void Decrypt_StringInputNotUsingKeysInConfiguration_CryptographicException()
        {
            bool isLatestKey;
            AES.Decrypt("FK5WAQDPSRYDRS2UB4S86FZ747M5JT7CF6CTDAHJMTXJSMP8PK52", out isLatestKey);

            Assert.Fail(
                "CryptographicException was expected when the input was encrypted using a key not found within the configuration");
        }

        [TestMethod]
        public void TryDecrypt_ReturnValue_ReturnsFalseWhenKeyIsNotLatestKey()
        {
            string decryptedString;
            bool? isLatestKey;

            AES.Encrypt("a new key will be made now");
            bool decrypted = AES.TryDecrypt("FJN58QU5ZX66NCRGT6UKQ9DDZYB4DA5WBEFEWUBX9PKHS587QNZ1",
                                                         out decryptedString, out isLatestKey);

            Assert.IsFalse(isLatestKey.Value, "IsLatestKey should return false");
        }

        [TestMethod]
        public void TryDecrypt_NullInputString_FalseReturned()
        {
            string decryptedString;
            bool? isLatestKey;

            Assert.IsTrue(AES.TryDecrypt(null, out decryptedString, out isLatestKey));
            Assert.IsNull(decryptedString);
        }
    }
}