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
        public void Create_Provider_And_Save()
        {
            CryptographyConfiguration configuration = CryptographyConfiguration.Active;
            ProviderCollection providers = configuration.Providers;
            Assert.IsNotNull(providers);

            // Generate unknown key
            string key;
            do
            {
                key = Tester.RandomString(10, minLength: 5);
            } while (providers[key] != null);

            Assert.IsFalse(string.IsNullOrWhiteSpace(key));

            // Detect changes to configuration
            EventWaitHandle ewh = new EventWaitHandle(false, EventResetMode.AutoReset);
            ConfigurationSection<CryptographyConfiguration>.ActiveChanged += (o, e) =>
            {
                Assert.IsNotNull(o);
                Assert.IsNotNull(e);
                ewh.Set();
            };

            bool added = false;
            CryptographyProvider provider = configuration.GetOrAddProvider(
                key,
                () =>
                {
                    added = true;
                    return RSACryptographyProvider.Create();
                });

            Assert.IsTrue(added);

            // Wait for the configuration changed event to fire
            Assert.IsTrue(ewh.WaitOne(TimeSpan.FromSeconds(5)), "Configuration changed event did not fire.");
            Assert.IsNotNull(provider);
            Assert.IsInstanceOfType(provider, typeof(RSACryptographyProvider));

            configuration = CryptographyConfiguration.Active;
            CryptographyProvider provider2 = configuration.GetProvider(key);
            Assert.IsNotNull(provider2);

            Tuple<XObject, XObject> difference = provider.Configuration.DeepEquals(
                provider2.Configuration,
                XObjectComparisonOptions.Semantic);

            Assert.IsNull(difference, $"{difference?.Item1} : {difference?.Item2}");

            // Remove provider from configuration
            configuration.Providers.Remove(key);
            configuration.Save();

            // Wait for the configuration changed event to fire
            Assert.IsTrue(ewh.WaitOne(TimeSpan.FromSeconds(1)), "Configuration changed event did not fire.");
            provider2 = configuration.GetProvider(key);
            Assert.IsNull(provider2);
        }
    }
}