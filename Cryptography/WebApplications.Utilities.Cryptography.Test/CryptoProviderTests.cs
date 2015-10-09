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
using System.Configuration;
using System.Diagnostics;
using WebApplications.Utilities.Cryptography.Configuration;

namespace WebApplications.Utilities.Cryptography.Test
{
    [TestClass]
    public class CryptoProviderTests : CryptographyTestBase
    {
        private const string InputString = "You spoony bard!";
        
        [TestMethod]
        public void RSAProvider_DecryptsSuccessfully()
        {
            ICryptoProvider provider = new RSACryptographer();
            string encrypted = provider.Encrypt(InputString);

            bool isLatestKey;
            string decrypted = provider.Decrypt(encrypted, out isLatestKey);
            Trace.WriteLine(decrypted);

            Assert.IsInstanceOfType(decrypted, typeof (string));
            Assert.AreEqual(decrypted, InputString, "the decrypted result should match the InputString");
        }

        [TestMethod]
        public void AESProvider_TryDecryptsSuccessfully()
        {
            ICryptoProvider provider = new AESCryptographer();
            string encrypted = provider.Encrypt(InputString);

            bool? isLatestKey;
            string decrypted;
            bool canDecrypt = provider.TryDecrypt(encrypted, out decrypted, out isLatestKey);
            Trace.WriteLine(decrypted);

            Assert.IsTrue(canDecrypt);
        }

        [TestMethod]
        public void TwoRSAProviders_CannotDecryptEachOthersEncryption()
        {
            string encrypted = RSA.Encrypt(InputString);

            bool? isLatestKey;
            string decrypted;
            bool canDecrypt = new RSACryptographer().TryDecrypt(encrypted, out decrypted, out isLatestKey);
            Trace.WriteLine(decrypted);

            Assert.IsFalse(canDecrypt);
        }

        [TestMethod]
        public void TwoAESProviders_CannotDecryptEachOthersEncryption()
        {
            string encrypted = AES2.Encrypt(InputString);

            bool? isLatestKey;
            string decrypted;
            bool canDecrypt = AES.TryDecrypt(encrypted, out decrypted, out isLatestKey);
            Trace.WriteLine(decrypted);

            Assert.IsFalse(canDecrypt);
        }

        [TestMethod]
        [ExpectedException(typeof (ConfigurationErrorsException))]
        public void DisabledProvider_ThrowsConfigurationErrorsException()
        {
            ICryptoProvider provider = CryptographyConfiguration.Active.Provider("3");
            Assert.Fail("'ConfigurationErrorsException' was expected when providing an id to a disabled provider");
        }
    }
}