using System;
using System.Configuration;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;
using WebApplications.Utilities.Configuration;

namespace WebApplications.Utilities.Test.Configuration
{
    [TestClass]
    public class ParameterElementTests : ConfigurationTestBase
    {
        private ParameterElement _parameterElement;

        [TestInitialize]
        public void Initialize()
        {
            _parameterElement = new ParameterElement();
        }

        [TestMethod]
        public void ParameterElement_Extends_ConfigurationElement()
        {
            Assert.IsInstanceOfType(_parameterElement, typeof(System.Configuration.ConfigurationElement), "The ParameterElement class should extend System.Configuration.ConfigurationElement.");
        }

        [TestMethod]
        public void Name_SetToString_ReturnsSetValue()
        {
            string testString = GenerateRandomString(Random.Next(3, 100));
            _parameterElement.Name = testString;
            Assert.AreEqual(testString, _parameterElement.Name, "The Name field should return the same value as it was last set to.");
        }

        [TestMethod]
        public void Name_HasConfigurationProperty()
        {
            List<ConfigurationPropertyAttribute> configurationPropertyAttributes =
                GetConfigurationPropertyAttributesForProperty(typeof(ParameterElement), "Name");
            Assert.AreEqual(1, configurationPropertyAttributes.Count, "There should be exactly one ConfigurationPropertyAttribute for the Name property.");
            Assert.AreEqual("name", configurationPropertyAttributes.First().Name, "The name of the ConfigurationPropertyAttribute for the Name property should be 'name'.");
        }

        [TestMethod]
        public void Value_SetToString_ReturnsSetValue()
        {
            string testString = GenerateRandomString(Random.Next(3, 100));
            _parameterElement.Value = testString;
            Assert.AreEqual(testString, _parameterElement.Value, "The Value field should return the same value as it was last set to.");
        }

        [TestMethod]
        public void Value_SetToNull_ReturnsSetValue()
        {
            _parameterElement.Value = null;
            Assert.IsNull(_parameterElement.Value, "The Value field should return null if set to null.");
        }

        [TestMethod]
        public void Value_HasConfigurationProperty()
        {
            List<ConfigurationPropertyAttribute> configurationPropertyAttributes =
                GetConfigurationPropertyAttributesForProperty(typeof(ParameterElement), "Value");
            Assert.AreEqual(1, configurationPropertyAttributes.Count, "There should be exactly one ConfigurationPropertyAttribute for the Value property.");
            Assert.AreEqual("value", configurationPropertyAttributes.First().Name, "The name of the ConfigurationPropertyAttribute for the Value property should be 'value'.");
        }

        [TestMethod]
        public void IsRequired_SetToBool_ReturnsSetValue()
        {
            bool testValue = Random.NextDouble() < 0.5;
            _parameterElement.IsRequired = testValue;
            Assert.AreEqual(testValue, _parameterElement.IsRequired, "The IsRequired field should return the same value as it was last set to.");
        }

        [TestMethod]
        public void IsRequired_HasConfigurationProperty()
        {
            List<ConfigurationPropertyAttribute> configurationPropertyAttributes =
                GetConfigurationPropertyAttributesForProperty(typeof(ParameterElement), "IsRequired");
            Assert.AreEqual(1, configurationPropertyAttributes.Count, "There should be exactly one ConfigurationPropertyAttribute for the IsRequired property.");
            Assert.AreEqual("required", configurationPropertyAttributes.First().Name, "The name of the ConfigurationPropertyAttribute for the IsRequired property should be 'required'.");
        }

        [TestMethod]
        public void Type_SetToType_ReturnsSetValue()
        {
            Type testType = ChooseRandomTypeFromList((new List<Type>{ typeof(int), typeof(char), typeof(bool), typeof(DateTime), typeof(double) }));
            _parameterElement.Type = testType;
            Assert.AreEqual(testType, _parameterElement.Type, "The Type field should return the same value as it was last set to.");
        }

        [TestMethod]
        public void Type_SetToNull_ReturnsSetValue()
        {
            _parameterElement.Type = null;
            Assert.IsNull(_parameterElement.Type, "The Type field should return null if set to null.");
        }

        [TestMethod]
        public void Type_HasConfigurationProperty()
        {
            List<ConfigurationPropertyAttribute> configurationPropertyAttributes =
                GetConfigurationPropertyAttributesForProperty(typeof(ParameterElement), "Type");
            Assert.AreEqual(1, configurationPropertyAttributes.Count, "There should be exactly one ConfigurationPropertyAttribute for the Type property.");
            Assert.AreEqual("type", configurationPropertyAttributes.First().Name, "The name of the ConfigurationPropertyAttribute for the Type property should be 'type'.");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void TypeConverter_SetToUnacceptableType_ThrowsConfigurationErrorsException()
        {
            Type testType = ChooseRandomTypeFromList((new List<Type>{ typeof(int), typeof(char), typeof(bool), typeof(DateTime), typeof(double) }));
            _parameterElement.TypeConverter = testType;
        }

        [TestMethod]
        public void TypeConverter_SetToTypeConverter_ReturnsSetValue()
        {
            Type testType = ChooseRandomTypeFromList((new List<Type>{ typeof(InfiniteIntConverter), typeof(TypeNameConverter), typeof(TimeSpanMinutesConverter), typeof(WhiteSpaceTrimStringConverter) }));
            _parameterElement.TypeConverter = testType;
            Assert.AreEqual(testType, _parameterElement.TypeConverter, "The TypeConverter field should return the same value as it was last set to.");
        }

        [TestMethod]
        public void TypeConverter_SetToNull_ReturnsSetValue()
        {
            _parameterElement.TypeConverter = null;
            Assert.IsNull(_parameterElement.Type, "The TypeConverter field should return null if set to null.");
        }

        [TestMethod]
        public void TypeConverter_HasConfigurationProperty()
        {
            List<ConfigurationPropertyAttribute> configurationPropertyAttributes =
                GetConfigurationPropertyAttributesForProperty(typeof(ParameterElement), "TypeConverter");
            Assert.AreEqual(1, configurationPropertyAttributes.Count, "There should be exactly one ConfigurationPropertyAttribute for the TypeConverter property.");
            Assert.AreEqual("typeConverter", configurationPropertyAttributes.First().Name, "The name of the ConfigurationPropertyAttribute for the TypeConverter property should be 'typeConverter'.");
        }
    }
}
