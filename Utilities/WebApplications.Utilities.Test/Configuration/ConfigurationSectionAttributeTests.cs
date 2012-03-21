using System;
using System.Text;
using System.Collections.Generic;
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
        [ConfigurationSection(NameUsedInAttribute)]
        class TestClassWithConfigurationSectionAttribute
        {

        }


        [TestMethod]
        public void Name_SetUsingAttribute_ReturnsValueGivenInAttribute()
        {
            // Assume there will be no bugs in the .NET framework that stop the attribute from existing, and go straight to calling First.
            ConfigurationSectionAttribute attribute =
                typeof (TestClassWithConfigurationSectionAttribute).GetCustomAttributes(false).OfType
                    <ConfigurationSectionAttribute>().First();

            Assert.AreEqual(NameUsedInAttribute, attribute.Name, "The name field of the ConfigurationSectionAttribute should match the first parameter supplied when adding the attribute to a class.");
        }
    }
}
