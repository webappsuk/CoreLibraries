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
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using WebApplications.Testing;
using WebApplications.Utilities.Configuration;
using WebApplications.Utilities.Cryptography.Configuration;

namespace WebApplications.Utilities.Cryptography.Test
{
    [TestClass]
    public class CryptographyConfigurationTests : CryptographyTestBase
    {
        [TestMethod]
        public void Create_Providers_And_Save()
        {
            // Supported algorithmns (see https://msdn.microsoft.com/en-us/library/system.security.cryptography.cryptoconfig(v=vs.110).aspx)
            string[] names = new[]
            {
                // Asymmetric algorithms
                "RSA",
                /*
                "DSA",
                "ECDH",
                "ECDsa"
                */

                // Symmetric algorithms
                "AES",
                "DES",
                "3DES",
                "RC2",
                "Rijndael",

                // Hashing algorithms
                "MD5",
                "RIPEMD160",
                "SHA",
                "SHA256",
                "SHA384",
                "SHA512",

                // Keyed hashing algorithms
                "HMACMD5",
                "HMACRIPEMD160",
                "HMACSHA1",
                "HMACSHA256",
                "HMACSHA384",
                "HMACSHA512",
                "MACTripleDES",
                
                // Random number generator
                "RandomNumberGenerator"
            };

            int providerCount = names.Length;

            CryptographyConfiguration configuration = CryptographyConfiguration.Active;
            Trace.WriteLine($"Configuration file : {configuration.FilePath}");

            ProviderCollection providerCollection = configuration.Providers;
            Assert.IsNotNull(providerCollection);

            // Generate random keys for each algorithm
            string[] keys = names.Select(n => $"{n}-{Guid.NewGuid()}").ToArray();

            // Detect changes to configuration
            EventWaitHandle ewh = new EventWaitHandle(false, EventResetMode.AutoReset);
            ConfigurationSection<CryptographyConfiguration>.ActiveChanged += (o, e) =>
            {
                Assert.IsNotNull(o);
                Assert.IsNotNull(e);
                ewh.Set();
            };

            // Add providers
            CryptographyProvider[] providers = new CryptographyProvider[providerCount];
            for (int k = 0; k < providerCount; k++)
            {
                bool added = false;
                providers[k] = CryptographyConfiguration.Active.GetOrAddProvider(
                    // ReSharper disable once AssignNullToNotNullAttribute
                    keys[k],
                    () =>
                    {
                        added = true;
                        return CryptographyProvider.Create(names[k]);
                    });

                Assert.IsTrue(added);
            }

            // Wait for the configuration changed event to fire
            Assert.IsTrue(ewh.WaitOne(TimeSpan.FromSeconds(5)), "Configuration changed event did not fire.");
            CollectionAssert.AllItemsAreNotNull(providers);
            CollectionAssert.AllItemsAreInstancesOfType(providers, typeof(CryptographyProvider));

            configuration = CryptographyConfiguration.Active;


            for (int k = 0; k < providerCount; k++)
            {
                CryptographyProvider provider = providers[k];
                Assert.IsNotNull(provider);

                string key = keys[k];
                Assert.IsNotNull(key);
                Assert.AreEqual(key, provider.Id, "The provider did not have it's ID set!");
                Assert.IsNotNull(provider.Configuration);

                CryptographyProvider reloadProvider = configuration.GetProvider(key);
                Assert.IsNotNull(reloadProvider);
                Assert.AreEqual(key, reloadProvider.Id, "The loaded provider did not have it's ID set!");

                // Check configs match
                Tuple<XObject, XObject> difference = provider.Configuration.DeepEquals(
                    reloadProvider.Configuration,
                    XObjectComparisonOptions.Semantic);

                Assert.IsNull(difference, $"Configurations do not match {difference?.Item1} : {difference?.Item2}");

                if (reloadProvider.CanEncrypt)
                {
                    // Test encryption
                    string random = Tester.RandomString(minLength: 1);
                    string encrypted = provider.EncryptToString(random);
                    Assert.IsNotNull(encrypted);
                    Assert.IsFalse(random == encrypted, "Expected and actual string are equal.");

                    if (reloadProvider.CanDecrypt)
                    {
                        string decrypted = provider.DecryptToString(encrypted);
                        Assert.IsNotNull(decrypted);
                        Assert.IsTrue(
                            random == decrypted,
                            "Expected: {0}\r\n  Actual: {1}",
                            Convert.ToBase64String(Encoding.Unicode.GetBytes(random)),
                            Convert.ToBase64String(Encoding.Unicode.GetBytes(decrypted)));
                    }
                }


                // Remove provider from configuration
                configuration.Providers.Remove(key);
            }

            // Re-save configuration
            configuration.Save();

            // Wait for the configuration changed event to fire
            Assert.IsTrue(ewh.WaitOne(TimeSpan.FromSeconds(1)), "Configuration changed event did not fire.");

            for (int k = 0; k < providerCount; k++)
            {
                string key = keys[k];
                Assert.IsNotNull(key);
                Assert.IsNull(
                    configuration.GetProvider(key),
                    "The provider with key '{key'} was not removed successfully.");
            }
        }
    }
}