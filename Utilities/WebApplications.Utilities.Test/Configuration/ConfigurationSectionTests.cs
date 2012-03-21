using System;
using System.Configuration;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Configuration;

namespace WebApplications.Utilities.Test.Configuration
{
    [TestClass]
    public class ConfigurationSectionTests : ConfigurationTestBase
    {

        /// <summary>
        /// As some of the things under test are static states, it is important to make sure the same static object is not used by two tests.
        /// This has been done using generics. For each type supplied as the type parameter, a new "instance" of the static class is made.
        /// As a result, setting Active on ConfigSection&lt;T1&gt; will leave Active for ConfigSection&lt;T2&gt; unassigned.
        /// I apologise for this crude hack.
        /// </summary>
        private class UniqueIdentifierForStaticMethods
        {
            
        }

        private class UniqueIdentifierForStaticMethods1 : UniqueIdentifierForStaticMethods
        {
        }

        private class UniqueIdentifierForStaticMethods2 : UniqueIdentifierForStaticMethods
        {
        }

        private class UniqueIdentifierForStaticMethods3 : UniqueIdentifierForStaticMethods
        {
        }

        private class UniqueIdentifierForStaticMethods4 : UniqueIdentifierForStaticMethods
        {
        }

        private class UniqueIdentifierForStaticMethods5 : UniqueIdentifierForStaticMethods
        {
        }

        class ConfigurationSectionTestClass : ConfigurationSection<ConfigurationSectionTestClass>
        {
            
        }

        private static ConfigurationSectionTestClass GenerateBlankConfigurationSectionTestClass()
        {
            return new ConfigurationSectionTestClass();
        }

        [TestMethod]
        public void Xmlns_SetToString_ReturnsSetValue()
        {
            ConfigurationSectionTestClass configurationSection = GenerateBlankConfigurationSectionTestClass();
            string testString = GenerateRandomString(Random.Next(3, 100));
            configurationSection.Xmlns = testString;
            Assert.AreEqual(testString, configurationSection.Xmlns, "The Xmlns field should return the same value as it was last set to.");
        }

        [TestMethod]
        public void Xmlns_HasConfigurationProperty()
        {
            List<ConfigurationPropertyAttribute> configurationPropertyAttributes =
                GetConfigurationPropertyAttributesForProperty(typeof(ConfigurationSectionTestClass), "Xmlns");
            Assert.AreEqual(1, configurationPropertyAttributes.Count, "There should be exactly one ConfigurationPropertyAttribute for the Xmlns property.");
            Assert.AreEqual("xmlns", configurationPropertyAttributes.First().Name, "The name of the ConfigurationPropertyAttribute for the Xmlns property should be 'xmlns'.");
        }

        class ConfigurationSectionNameTest : ConfigurationSection<ConfigurationSectionNameTest> { }

        [TestMethod]
        public void SectionName_ClassNameEndsInNothingSpecial_ClassNameUsedButFirstLetterLowercase()
        {
            Assert.AreEqual("configurationSectionNameTest", ConfigurationSectionNameTest.SectionName);
        }

        [ConfigurationSection("configurationSectionNameTest")]
        class ConfigurationSectionNameTestWithAttribute : ConfigurationSection<ConfigurationSectionNameTestWithAttribute> { }

        [TestMethod]
        public void SectionName_ConfigurationSectionAttributeExists_ValueFromAttributeUsed()
        {
            Assert.AreEqual("configurationSectionNameTest", ConfigurationSectionNameTestWithAttribute.SectionName);
        }

        class ConfigurationSectionNameTestConfigurationSection : ConfigurationSection<ConfigurationSectionNameTestConfigurationSection> { }

        [TestMethod]
        public void SectionName_ClassNameEndsInWordsConfigurationSection_ClassNameUsedButFirstLetterLowercaseAndWordsConfigurationSectionRemoved()
        {
            Assert.AreEqual("configurationSectionNameTest", ConfigurationSectionNameTestConfigurationSection.SectionName);
        }

        class ConfigurationSectionNameTestConfiguration : ConfigurationSection<ConfigurationSectionNameTestConfiguration> { }

        [TestMethod]
        public void SectionName_ClassNameEndsInWordConfiguration_ClassNameUsedButFirstLetterLowercaseAndWordConfigurationRemoved()
        {
            Assert.AreEqual("configurationSectionNameTest", ConfigurationSectionNameTestConfiguration.SectionName);
        }

        class ConfigurationSectionNameTestSection : ConfigurationSection<ConfigurationSectionNameTestSection> { }

        [TestMethod]
        public void SectionName_ClassNameEndsInWordSection_ClassNameUsedButFirstLetterLowercaseAndWordSectionRemoved()
        {
            Assert.AreEqual("configurationSectionNameTest", ConfigurationSectionNameTestSection.SectionName);
        }

        class ConfigurationSectionNameTestConfig : ConfigurationSection<ConfigurationSectionNameTestConfig> { }

        [TestMethod]
        public void SectionName_ClassNameEndsInWordConfig_ClassNameUsedButFirstLetterLowercaseAndWordConfigRemoved()
        {
            Assert.AreEqual("configurationSectionNameTest", ConfigurationSectionNameTestConfig.SectionName);
        }

        class ConfigurationSectionWhichIsNotPresentInConfigFileTestClass<TUnique> : ConfigurationSection<ConfigurationSectionWhichIsNotPresentInConfigFileTestClass<TUnique>>
            where TUnique : UniqueIdentifierForStaticMethods
        {
            public const string DefaultValueForProperty = "Default Value";

            [ConfigurationProperty("propertyWithDefaultValue", DefaultValue = DefaultValueForProperty)]
            public string PropertyWithDefaultValue
            {
                get { return GetProperty<string>("propertyWithDefaultValue"); }
                set { SetProperty("propertyWithDefaultValue", value); }
            }
        }

        [TestMethod]
        public void Active_ConfigurationSectionWhichIsNotPresentInConfigFile_CreatesInstanceWithDefaultValues()
        {
            Assert.AreEqual(ConfigurationSectionWhichIsNotPresentInConfigFileTestClass<UniqueIdentifierForStaticMethods1>.DefaultValueForProperty,
                            ConfigurationSectionWhichIsNotPresentInConfigFileTestClass<UniqueIdentifierForStaticMethods1>.Active.PropertyWithDefaultValue,
                            "If the section in question has not been specified in the config file, then the active configuration (Active) property" +
                            " should return a default instance with all properties taking their default values. ");
        }

        [TestMethod]
        public void Active_SetToNewInstance_ReturnsSetInstance()
        {
            ConfigurationSectionWhichIsNotPresentInConfigFileTestClass<UniqueIdentifierForStaticMethods2>
                configurationSection =
                    new ConfigurationSectionWhichIsNotPresentInConfigFileTestClass<UniqueIdentifierForStaticMethods2>();
            ConfigurationSectionWhichIsNotPresentInConfigFileTestClass<UniqueIdentifierForStaticMethods2>.Active = configurationSection;
            Assert.AreSame(configurationSection,
                            ConfigurationSectionWhichIsNotPresentInConfigFileTestClass<UniqueIdentifierForStaticMethods2>.Active);
        }

        [TestMethod]
        public void Active_SetToNullForConfigurationSectionWhichIsNotPresentInConfigFile_RestoresDefaultValues()
        {
            // In general this test is testing that when Active is set to null, the initial value is restored. When there is no config section, this will be the default values.

            // First set up some data and check the test would fail before Active is set to null:
            ConfigurationSectionWhichIsNotPresentInConfigFileTestClass<UniqueIdentifierForStaticMethods3> configurationSection =
                new ConfigurationSectionWhichIsNotPresentInConfigFileTestClass<UniqueIdentifierForStaticMethods3>
                    {PropertyWithDefaultValue = "Not the default value"};
            ConfigurationSectionWhichIsNotPresentInConfigFileTestClass<UniqueIdentifierForStaticMethods3>.Active = configurationSection;
            Assert.AreNotEqual(ConfigurationSectionWhichIsNotPresentInConfigFileTestClass<UniqueIdentifierForStaticMethods3>.DefaultValueForProperty,
                               ConfigurationSectionWhichIsNotPresentInConfigFileTestClass<UniqueIdentifierForStaticMethods3>.Active.
                                   PropertyWithDefaultValue);

            // Now set it to null and check it returns to its initial state:
            ConfigurationSectionWhichIsNotPresentInConfigFileTestClass<UniqueIdentifierForStaticMethods3>.Active = null;

            Assert.AreEqual(ConfigurationSectionWhichIsNotPresentInConfigFileTestClass<UniqueIdentifierForStaticMethods3>.DefaultValueForProperty,
                            ConfigurationSectionWhichIsNotPresentInConfigFileTestClass<UniqueIdentifierForStaticMethods3>.Active.PropertyWithDefaultValue,
                            "If the section in question has not been specified in the config file, then the active configuration (Active) property" +
                            " should return a default instance with all properties taking their default values if it is previously set to null. ");
        }

        [TestMethod]
        public void IsActive_ForActiveConfiguration_ReturnsTrue()
        {
            Assert.IsTrue(ConfigurationSectionTestClass.Active.IsActive, "The IsActive property should be true for the active configuration section (obtained from the Active property).");
        }

        [TestMethod]
        public void IsActive_ForNewConfiguration_ReturnsFalse()
        {
            ConfigurationSectionTestClass configurationSection = GenerateBlankConfigurationSectionTestClass();

            Assert.IsFalse(configurationSection.IsActive, "The IsActive property should be false for a newly created (and thus inactive) configuration section.");
        }

        [TestMethod]
        public void IsActive_ForNewConfigurationAfterAssigningToActive_ReturnsTrue()
        {
            ConfigurationSectionTestClass configurationSection = GenerateBlankConfigurationSectionTestClass();

            ConfigurationSectionTestClass.Active = configurationSection;

            Assert.IsTrue(configurationSection.IsActive, "The IsActive property should be false for a newly created (and thus inactive) configuration section.");
        }

        [TestMethod]
        public void IsReadOnly_ForActiveConfiguration_ReturnsTrue()
        {
            Assert.IsTrue(ConfigurationSectionTestClass.Active.IsReadOnly(), "The IsReadOnly method should be true for the active configuration section (obtained from the Active property).");
        }

        [TestMethod]
        public void IsReadOnly_ForNewConfiguration_ReturnsFalse()
        {
            ConfigurationSectionTestClass configurationSection = GenerateBlankConfigurationSectionTestClass();

            Assert.IsFalse(configurationSection.IsReadOnly(), "The IsReadOnly method should be false for a newly created (and thus inactive) configuration section.");
        }

        [TestMethod]
        public void IsReadOnly_AfterNonExistingSectionIsReloaded_ReturnsFalse()
        {
            ConfigurationSectionTestClass.Active = null; // Load the section (to prove this works even when the contents of the section will be identical)
            ConfigurationSectionTestClass oldConfigurationSection = ConfigurationSectionTestClass.Active; // Store this old instantiation for later

            ConfigurationSectionTestClass.Active = null; // Force the section to be reloaded

            Assert.IsFalse(oldConfigurationSection.IsReadOnly(), "The IsReadOnly method for a previous value of Active should be false after the configuration section is reloaded by setting Active to null.");
        }

        /*
         * This code relies upon certain sections being present in the App.config, and so are being removed.
         * 
        class TestConfigurationSectionWhichIsPresentInConfigFile : ConfigurationSection<TestConfigurationSectionWhichIsPresentInConfigFile>
        {
            [ConfigurationProperty("someAttribute", DefaultValue = "A String")]
            public string someAttribute
            {
                get { return GetProperty<string>("someAttribute"); }
                set { SetProperty("someAttribute", value); }
            }
        }

        [TestMethod]
        public void IsReadOnly_AfterExistingSectionIsReloaded_ReturnsFalse()
        {
            TestConfigurationSectionWhichIsPresentInConfigFile.Active = null; // Load the section (to prove this works even when the contents of the section will be identical)
            Assert.AreEqual("some value", TestConfigurationSectionWhichIsPresentInConfigFile.Active.someAttribute); // Test that the data is loaded correctly as this value is given in App.config
            TestConfigurationSectionWhichIsPresentInConfigFile oldConfigurationSection = TestConfigurationSectionWhichIsPresentInConfigFile.Active; // Store this old instantiation for later

            TestConfigurationSectionWhichIsPresentInConfigFile.Active = null; // Force the section to be reloaded

            Assert.IsTrue(oldConfigurationSection.IsReadOnly(), "The IsReadOnly method for a previous value of Active should be false after the configuration section is reloaded by setting Active to null.");
        }
        */

        class ConfigurationSectionWithOnChangedEvent<TUnique> : ConfigurationSection<ConfigurationSectionWithOnChangedEvent<TUnique>>
            where TUnique : UniqueIdentifierForStaticMethods
        {
            public static bool ChangedEventCalled = false;

            static ConfigurationSectionWithOnChangedEvent()
            {
                Changed += (o, e) =>
                {
                    ChangedEventCalled = true;
                };
            }

            public static void ResetChangedEventCalledFlag()
            {
                ChangedEventCalled = false;
            }
        }

        [TestMethod]
        public void Changed_ActiveSetToNewValueForFirstTimeEver_EventNotCalled()
        {
            ConfigurationSectionWithOnChangedEvent<UniqueIdentifierForStaticMethods1> configurationSection = new ConfigurationSectionWithOnChangedEvent<UniqueIdentifierForStaticMethods1>();
            ConfigurationSectionWithOnChangedEvent<UniqueIdentifierForStaticMethods1>.ResetChangedEventCalledFlag();
            ConfigurationSectionWithOnChangedEvent<UniqueIdentifierForStaticMethods1>.Active = configurationSection;
            Assert.IsFalse(ConfigurationSectionWithOnChangedEvent<UniqueIdentifierForStaticMethods1>.ChangedEventCalled, "When the Active property is set to a new value, the Changed event should be called.");
        }

        [TestMethod]
        public void Changed_ActiveSetToNewValueAfterBeingSetToNull_EventCalled()
        {
            ConfigurationSectionWithOnChangedEvent<UniqueIdentifierForStaticMethods2>.Active = null;
            ConfigurationSectionWithOnChangedEvent<UniqueIdentifierForStaticMethods2> configurationSection = new ConfigurationSectionWithOnChangedEvent<UniqueIdentifierForStaticMethods2>();
            ConfigurationSectionWithOnChangedEvent<UniqueIdentifierForStaticMethods2>.ResetChangedEventCalledFlag();
            ConfigurationSectionWithOnChangedEvent<UniqueIdentifierForStaticMethods2>.Active = configurationSection;
            Assert.IsTrue(ConfigurationSectionWithOnChangedEvent<UniqueIdentifierForStaticMethods2>.ChangedEventCalled, "When the Active property is set to a new value, the Changed event should be called.");
        }

        [TestMethod]
        public void Changed_ActiveSetToNewValueAfterBeingSetToSomethingNotNull_EventCalled()
        {
            ConfigurationSectionWithOnChangedEvent<UniqueIdentifierForStaticMethods4>.Active = new ConfigurationSectionWithOnChangedEvent<UniqueIdentifierForStaticMethods4>();
            ConfigurationSectionWithOnChangedEvent<UniqueIdentifierForStaticMethods4> configurationSection = new ConfigurationSectionWithOnChangedEvent<UniqueIdentifierForStaticMethods4>();
            ConfigurationSectionWithOnChangedEvent<UniqueIdentifierForStaticMethods4>.ResetChangedEventCalledFlag();
            ConfigurationSectionWithOnChangedEvent<UniqueIdentifierForStaticMethods4>.Active = configurationSection;
            Assert.IsTrue(ConfigurationSectionWithOnChangedEvent<UniqueIdentifierForStaticMethods4>.ChangedEventCalled, "When the Active property is set to a new value, the Changed event should be called.");
        }

        [TestMethod]
        public void Changed_ActiveSetToNewForFirstTimeEverButAfterGettingActive_EventNotCalled()
        {
            var active = ConfigurationSectionWithOnChangedEvent<UniqueIdentifierForStaticMethods3>.Active;
            ConfigurationSectionWithOnChangedEvent<UniqueIdentifierForStaticMethods3> configurationSection = new ConfigurationSectionWithOnChangedEvent<UniqueIdentifierForStaticMethods3>();
            ConfigurationSectionWithOnChangedEvent<UniqueIdentifierForStaticMethods3>.ResetChangedEventCalledFlag();
            ConfigurationSectionWithOnChangedEvent<UniqueIdentifierForStaticMethods3>.Active = configurationSection;
            Assert.IsFalse(ConfigurationSectionWithOnChangedEvent<UniqueIdentifierForStaticMethods3>.ChangedEventCalled, "When the Active property is set for the first time, the Changed event should be not be called.");
        }

        [TestMethod]
        public void Changed_ActiveSetToSameValue_EventNotCalled() 
        {
            ConfigurationSectionWithOnChangedEvent<UniqueIdentifierForStaticMethods5> configurationSection = new ConfigurationSectionWithOnChangedEvent<UniqueIdentifierForStaticMethods5>();
            ConfigurationSectionWithOnChangedEvent<UniqueIdentifierForStaticMethods5>.Active = configurationSection;
            ConfigurationSectionWithOnChangedEvent<UniqueIdentifierForStaticMethods5>.ResetChangedEventCalledFlag();
            ConfigurationSectionWithOnChangedEvent<UniqueIdentifierForStaticMethods5>.Active = configurationSection;
            Assert.IsFalse(ConfigurationSectionWithOnChangedEvent<UniqueIdentifierForStaticMethods5>.ChangedEventCalled, "When the Active property is set to the same value as it was last set to, the Changed event should not be called.");
        }

        static private void SetAndTriggerChangedEvent( ConfigurationSection<ConfigurationSectionTestClass>.ConfigurationChangedEventHandler eventMethod )
        {
            ConfigurationSectionTestClass.Changed += eventMethod; // Subscribe test to event

            // Trigger the event:
            // This first line makes sure we are never dealing with a completely fresh copy of the class, as otherwise the second line does not count as an update.
            ConfigurationSectionTestClass.Active = ConfigurationSectionTestClass.Active;
            ConfigurationSectionTestClass.Active = GenerateBlankConfigurationSectionTestClass();

            ConfigurationSectionTestClass.Changed -= eventMethod; // Clean up afterwards
        }

        [TestMethod]
        public void ConfigurationChangedHandler_WhenTriggered_SenderNotNull()
        {
            SetAndTriggerChangedEvent( (sender, arguments) => Assert.IsNotNull(sender) );
        }

        [TestMethod]
        public void ConfigurationChangedHandler_WhenTriggered_ArgumentsNotNull()
        {
            SetAndTriggerChangedEvent((sender, arguments) => Assert.IsNotNull(arguments));
        }

        [TestMethod]
        public void ConfigurationChangedHandler_WhenTriggered_ArgumentsContainsNotNullOldConfigurationProperty()
        {
            SetAndTriggerChangedEvent((sender, arguments) => Assert.IsNotNull(arguments.OldConfiguration));
        }

        [TestMethod]
        public void ConfigurationChangedHandler_WhenTriggered_ArgumentsContainsNotNullNewConfigurationProperty()
        {
            SetAndTriggerChangedEvent((sender, arguments) => Assert.IsNotNull(arguments.NewConfiguration));
        }

        [TestMethod]
        public void ConfigurationChangedHandler_WhenTriggered_NewConfigurationArgumentMatchesActiveConfiguration()
        {
            SetAndTriggerChangedEvent(
                (sender, arguments) =>
                Assert.AreEqual(ConfigurationSectionTestClass.Active, arguments.NewConfiguration,
                                "The NewConfiguration property of the arguments sent to the Changed event should match the Active property of the ConfigurationSection."));
        }
    }
}
