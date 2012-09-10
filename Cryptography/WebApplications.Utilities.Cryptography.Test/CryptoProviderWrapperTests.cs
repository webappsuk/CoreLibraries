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

using System.Configuration;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebApplications.Utilities.Cryptography.Test
{
    [TestClass]
    public class CryptoProviderWrapperTests
    {
        private const string InputString = "You spoony bard!";
        private readonly CryptoProviderWrapper _providerWrapper = new CryptoProviderWrapper("none");

        [TestMethod]
        public void IdNotInConfiguration_EncryptsSuccessfully()
        {
            string encrypted = _providerWrapper.Encrypt(InputString);
            Trace.WriteLine(encrypted);

            Assert.IsInstanceOfType(encrypted, typeof (string),
                                    "A provider with an ID not in the test configuration should create an RSA Cryptographer");
        }

        [TestMethod]
        public void IdNotInConfiguration_DecryptsSuccessfully()
        {
            string encrypted = _providerWrapper.Encrypt(InputString);

            bool isLatestKey;
            string decrypted = _providerWrapper.Decrypt(encrypted, out isLatestKey);
            Trace.WriteLine(decrypted);

            Assert.IsInstanceOfType(decrypted, typeof (string));
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
        [ExpectedException(typeof (ConfigurationErrorsException))]
        public void Constructor_DisabledProvider_ThrowsConfigurationErrorsException()
        {
            new CryptoProviderWrapper("3");

            Assert.Fail("'ConfigurationErrorsException' was expected when providing an id to a disabled provider");
        }
    }
}