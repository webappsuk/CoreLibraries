#region © Copyright Web Applications (UK) Ltd, 2014.  All rights reserved.
// Copyright (c) 2014, Web Applications UK Ltd
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

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Configuration;

namespace WebApplications.Utilities.Test.Configuration
{
    [TestClass]
    public class ConfigurationSectionAttributeTests
    {
        private const string NameUsedInAttribute = "Name used in attribute.";

        // Technically the existance of this class is a test... if it compiles then the attribute is a valid registered attribute.


        [TestMethod]
        public void Name_SetUsingAttribute_ReturnsValueGivenInAttribute()
        {
            // Assume there will be no bugs in the .NET framework that stop the attribute from existing, and go straight to calling First.
            ConfigurationSectionAttribute attribute =
                typeof (TestClassWithConfigurationSectionAttribute).GetCustomAttributes(false).OfType
                    <ConfigurationSectionAttribute>().First();

            Assert.AreEqual(
                NameUsedInAttribute,
                attribute.Name,
                "The name field of the ConfigurationSectionAttribute should match the first parameter supplied when adding the attribute to a class.");
        }

        #region Nested type: TestClassWithConfigurationSectionAttribute
        [ConfigurationSection(NameUsedInAttribute)]
        private class TestClassWithConfigurationSectionAttribute
        {
        }
        #endregion
    }
}