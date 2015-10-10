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

using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConfigurationElement = WebApplications.Utilities.Configuration.ConfigurationElement;

namespace WebApplications.Utilities.Test.Configuration
{
    [TestClass]
    public class ConfigurationElementTests : ConfigurationTestBase
    {
        [TestMethod]
        public void ConfigurationElement_Extends_ConfigurationElement()
        {
            Assert.IsInstanceOfType(
                new ConfigurationElement(),
                typeof (System.Configuration.ConfigurationElement),
                "The ConfigurationElement class should extend System.Configuration.ConfigurationElement.");
        }

        [TestMethod]
        public void GetProperty_AfterSetPropertyUsedToSetValue_ReturnsPreviouslySetValue()
        {
            // using a class which extends from ConfigurationElement in order to test protected methods
            ConfigurationElementTestClass configurationElement = new ConfigurationElementTestClass();
            Guid value = Guid.NewGuid();

            configurationElement.PropertyUsingUnderlyingProtectedMethods = value;

            Assert.AreEqual(
                value,
                configurationElement.PropertyUsingUnderlyingProtectedMethods,
                "A class extending ConfigurationElement should obtain from GetProperty the value last set using SetProperty.");
        }

        [TestMethod]
        [ExpectedException(typeof (NullReferenceException))]
        public void SetProperty_PropertyNameDoesNotMatchTheConfigurationProperty_ThrowsNullReferenceException()
        {
            // using a class which extends from ConfigurationElement in order to test protected methods
            ConfigurationElementTestClass configurationElement = new ConfigurationElementTestClass();
            Guid value = Guid.NewGuid();

            configurationElement.IncorrectlyConfiguredPropertyUsingUnderlyingProtectedMethods = value;
        }

        #region Nested type: ConfigurationElementTestClass
        private class ConfigurationElementTestClass : ConfigurationElement
        {
            [ConfigurationProperty("propertyName", IsRequired = false)]
            public Guid PropertyUsingUnderlyingProtectedMethods
            {
                get { return GetProperty<Guid>("propertyName"); }
                set { SetProperty("propertyName", value); }
            }

            [ConfigurationProperty("CorrectPropertyName", IsRequired = false)]
            public Guid IncorrectlyConfiguredPropertyUsingUnderlyingProtectedMethods
            {
                get { return GetProperty<Guid>("IncorrectPropertyName"); }
                set { SetProperty("IncorrectPropertyName", value); }
            }
        }
        #endregion
    }
}