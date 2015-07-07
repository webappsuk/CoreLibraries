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
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;
using WebApplications.Utilities.Configuration;
using ConfigurationElement = System.Configuration.ConfigurationElement;

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
            Assert.IsInstanceOfType(
                _parameterElement,
                typeof (ConfigurationElement),
                "The ParameterElement class should extend System.Configuration.ConfigurationElement.");
        }

        [TestMethod]
        public void Name_SetToString_ReturnsSetValue()
        {
            string testString = Random.RandomString(Random.Next(3, 100));
            _parameterElement.Name = testString;
            Assert.AreEqual(
                testString,
                _parameterElement.Name,
                "The Name field should return the same value as it was last set to.");
        }

        [TestMethod]
        public void Name_HasConfigurationProperty()
        {
            List<ConfigurationPropertyAttribute> configurationPropertyAttributes =
                GetConfigurationPropertyAttributesForProperty(typeof (ParameterElement), "Name");
            Assert.AreEqual(
                1,
                configurationPropertyAttributes.Count,
                "There should be exactly one ConfigurationPropertyAttribute for the Name property.");
            Assert.AreEqual(
                "name",
                configurationPropertyAttributes.First().Name,
                "The name of the ConfigurationPropertyAttribute for the Name property should be 'name'.");
        }

        [TestMethod]
        public void Value_SetToString_ReturnsSetValue()
        {
            string testString = Random.RandomString(Random.Next(3, 100));
            _parameterElement.Value = testString;
            Assert.AreEqual(
                testString,
                _parameterElement.Value,
                "The Value field should return the same value as it was last set to.");
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
                GetConfigurationPropertyAttributesForProperty(typeof (ParameterElement), "Value");
            Assert.AreEqual(
                1,
                configurationPropertyAttributes.Count,
                "There should be exactly one ConfigurationPropertyAttribute for the Value property.");
            Assert.AreEqual(
                "value",
                configurationPropertyAttributes.First().Name,
                "The name of the ConfigurationPropertyAttribute for the Value property should be 'value'.");
        }

        [TestMethod]
        public void IsRequired_SetToBool_ReturnsSetValue()
        {
            bool testValue = Random.NextDouble() < 0.5;
            _parameterElement.IsRequired = testValue;
            Assert.AreEqual(
                testValue,
                _parameterElement.IsRequired,
                "The IsRequired field should return the same value as it was last set to.");
        }

        [TestMethod]
        public void IsRequired_HasConfigurationProperty()
        {
            List<ConfigurationPropertyAttribute> configurationPropertyAttributes =
                GetConfigurationPropertyAttributesForProperty(typeof (ParameterElement), "IsRequired");
            Assert.AreEqual(
                1,
                configurationPropertyAttributes.Count,
                "There should be exactly one ConfigurationPropertyAttribute for the IsRequired property.");
            Assert.AreEqual(
                "required",
                configurationPropertyAttributes.First().Name,
                "The name of the ConfigurationPropertyAttribute for the IsRequired property should be 'required'.");
        }

        [TestMethod]
        public void Type_SetToType_ReturnsSetValue()
        {
            Type testType =
                ChooseRandomTypeFromList(
                    (new List<Type> {typeof (int), typeof (char), typeof (bool), typeof (DateTime), typeof (double)}));
            _parameterElement.Type = testType;
            Assert.AreEqual(
                testType,
                _parameterElement.Type,
                "The Type field should return the same value as it was last set to.");
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
                GetConfigurationPropertyAttributesForProperty(typeof (ParameterElement), "Type");
            Assert.AreEqual(
                1,
                configurationPropertyAttributes.Count,
                "There should be exactly one ConfigurationPropertyAttribute for the Type property.");
            Assert.AreEqual(
                "type",
                configurationPropertyAttributes.First().Name,
                "The name of the ConfigurationPropertyAttribute for the Type property should be 'type'.");
        }

        [TestMethod]
        [ExpectedException(typeof (ConfigurationErrorsException))]
        public void TypeConverter_SetToUnacceptableType_ThrowsConfigurationErrorsException()
        {
            Type testType =
                ChooseRandomTypeFromList(
                    (new List<Type> {typeof (int), typeof (char), typeof (bool), typeof (DateTime), typeof (double)}));
            _parameterElement.TypeConverter = testType;
        }

        [TestMethod]
        public void TypeConverter_SetToTypeConverter_ReturnsSetValue()
        {
            Type testType =
                ChooseRandomTypeFromList(
                    (new List<Type>
                    {
                        typeof (InfiniteIntConverter),
                        typeof (TypeNameConverter),
                        typeof (TimeSpanMinutesConverter),
                        typeof (WhiteSpaceTrimStringConverter)
                    }));
            _parameterElement.TypeConverter = testType;
            Assert.AreEqual(
                testType,
                _parameterElement.TypeConverter,
                "The TypeConverter field should return the same value as it was last set to.");
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
                GetConfigurationPropertyAttributesForProperty(typeof (ParameterElement), "TypeConverter");
            Assert.AreEqual(
                1,
                configurationPropertyAttributes.Count,
                "There should be exactly one ConfigurationPropertyAttribute for the TypeConverter property.");
            Assert.AreEqual(
                "typeConverter",
                configurationPropertyAttributes.First().Name,
                "The name of the ConfigurationPropertyAttribute for the TypeConverter property should be 'typeConverter'.");
        }
    }
}