using System;
using System.Configuration;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConfigurationElement = WebApplications.Utilities.Configuration.ConfigurationElement;

namespace WebApplications.Utilities.Test.Configuration
{
    [TestClass]
    public class ConfigurationElementTests : ConfigurationTestBase
    {

        class ConfigurationElementTestClass : ConfigurationElement
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

        [TestMethod]
        public void ConfigurationElement_Extends_ConfigurationElement()
        {
            Assert.IsInstanceOfType( new ConfigurationElement(), typeof(System.Configuration.ConfigurationElement), "The ConfigurationElement class should extend System.Configuration.ConfigurationElement.");
        }

        [TestMethod]
        public void GetProperty_AfterSetPropertyUsedToSetValue_ReturnsPreviouslySetValue()
        {
            // using a class which extends from ConfigurationElement in order to test protected methods
            ConfigurationElementTestClass configurationElement = new ConfigurationElementTestClass();
            Guid value = Guid.NewGuid();

            configurationElement.PropertyUsingUnderlyingProtectedMethods = value;

            Assert.AreEqual(value, configurationElement.PropertyUsingUnderlyingProtectedMethods, "A class extending ConfigurationElement should obtain from GetProperty the value last set using SetProperty.");
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void SetProperty_PropertyNameDoesNotMatchTheConfigurationProperty_ThrowsNullReferenceException()
        {
            // using a class which extends from ConfigurationElement in order to test protected methods
            ConfigurationElementTestClass configurationElement = new ConfigurationElementTestClass();
            Guid value = Guid.NewGuid();

            configurationElement.IncorrectlyConfiguredPropertyUsingUnderlyingProtectedMethods = value;
        }
    }
}
