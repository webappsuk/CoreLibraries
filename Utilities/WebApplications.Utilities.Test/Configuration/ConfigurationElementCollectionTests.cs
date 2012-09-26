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

using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Configuration;
using ConfigurationElement = WebApplications.Utilities.Configuration.ConfigurationElement;

namespace WebApplications.Utilities.Test.Configuration
{
    [TestClass]
    public class ConfigurationElementCollectionTests : ConfigurationTestBase
    {
        private static ConfigurationElementWithGuidKey GenerateConfigurationElementWithGuidKey(out Guid key)
        {
            key = Guid.NewGuid();
            return new ConfigurationElementWithGuidKey {Key = key};
        }

        private static ConfigurationElementCollectionTestClass GenerateEmptyConfigurationElementCollectionTestClass()
        {
            return new ConfigurationElementCollectionTestClass();
        }

        [TestMethod]
        public void ConfigurationElementCollection_Extends_ConfigurationElementCollection()
        {
            ConfigurationElementCollectionTestClass configurationElementCollection =
                GenerateEmptyConfigurationElementCollectionTestClass();
            Assert.IsInstanceOfType(configurationElementCollection, typeof (ConfigurationElementCollection),
                                    "The ConfigurationElementCollection class should extend System.Configuration.ConfigurationElementCollection.");
        }

        [TestMethod]
        public void Item_SetThenGetUsingIndex_ReturnsSameValue()
        {
            ConfigurationElementCollectionTestClass configurationElementCollection =
                GenerateEmptyConfigurationElementCollectionTestClass();

            Guid key;
            ConfigurationElementWithGuidKey configurationElement = GenerateConfigurationElementWithGuidKey(out key);

            configurationElementCollection[0] = configurationElement;

            Assert.AreEqual(configurationElement, configurationElementCollection[0]);
        }

        [TestMethod]
        public void Item_SetThenGetUsingCorrectKey_ReturnsSameValue()
        {
            ConfigurationElementCollectionTestClass configurationElementCollection =
                GenerateEmptyConfigurationElementCollectionTestClass();

            Guid key;
            ConfigurationElementWithGuidKey configurationElement = GenerateConfigurationElementWithGuidKey(out key);

            configurationElementCollection[key] = configurationElement;

            Assert.AreEqual(configurationElement, configurationElementCollection[key]);
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void Item_SetUsingIncorrectKey_ThrowsInvalidOperationException()
        {
            ConfigurationElementCollectionTestClass configurationElementCollection =
                GenerateEmptyConfigurationElementCollectionTestClass();

            Guid key;
            ConfigurationElementWithGuidKey configurationElement = GenerateConfigurationElementWithGuidKey(out key);

            Guid wrongKey = Guid.NewGuid();
            Assert.AreNotEqual(key, wrongKey,
                               "The state under test is one where a different key to the internal key is supplied to set the item.");

            configurationElementCollection[wrongKey] = configurationElement;
        }

        [TestMethod]
        public void Contains_EmptyCollection_ReturnsFalse()
        {
            ConfigurationElementCollectionTestClass configurationElementCollection =
                GenerateEmptyConfigurationElementCollectionTestClass();

            Guid key;
            ConfigurationElementWithGuidKey configurationElement = GenerateConfigurationElementWithGuidKey(out key);

            Assert.IsFalse(configurationElementCollection.Contains(configurationElement),
                           "The contains method should always return false for an empty collection.");
        }

        [TestMethod]
        public void Contains_WithElementNotInCollection_ReturnsFalse()
        {
            ConfigurationElementCollectionTestClass configurationElementCollection =
                GenerateEmptyConfigurationElementCollectionTestClass();

            Guid keyA;
            ConfigurationElementWithGuidKey configurationElementInCollection =
                GenerateConfigurationElementWithGuidKey(out keyA);

            Guid keyB;
            ConfigurationElementWithGuidKey configurationElementNotInCollection =
                GenerateConfigurationElementWithGuidKey(out keyB);

            configurationElementCollection.Add(configurationElementInCollection);

            Assert.IsFalse(configurationElementCollection.Contains(configurationElementNotInCollection),
                           "The contains method should return false for an element not in the collection.");
        }

        [TestMethod]
        public void Contains_WithElementAlreadyAdded_ReturnsTrue()
        {
            ConfigurationElementCollectionTestClass configurationElementCollection =
                GenerateEmptyConfigurationElementCollectionTestClass();

            Guid key;
            ConfigurationElementWithGuidKey configurationElement = GenerateConfigurationElementWithGuidKey(out key);

            configurationElementCollection.Add(configurationElement);

            Assert.IsTrue(configurationElementCollection.Contains(configurationElement),
                          "After using add to add an element to the ConfigurationElementCollection, the contains method should return true for this element.");
        }

        [TestMethod]
        public void Contains_WithElementAddedThenRemoved_ReturnsFalse()
        {
            ConfigurationElementCollectionTestClass configurationElementCollection =
                GenerateEmptyConfigurationElementCollectionTestClass();

            Guid key;
            ConfigurationElementWithGuidKey configurationElement = GenerateConfigurationElementWithGuidKey(out key);

            configurationElementCollection.Add(configurationElement);
            configurationElementCollection.Remove(configurationElement);

            Assert.IsFalse(configurationElementCollection.Contains(configurationElement),
                           "After using remove to remove a previously added element from the ConfigurationElementCollection, the contains method should return false for this element.");
        }

        [TestMethod]
        public void Contains_WithElementAddedThenRemovedUsingKey_ReturnsFalse()
        {
            ConfigurationElementCollectionTestClass configurationElementCollection =
                GenerateEmptyConfigurationElementCollectionTestClass();

            Guid key;
            ConfigurationElementWithGuidKey configurationElement = GenerateConfigurationElementWithGuidKey(out key);

            configurationElementCollection.Add(configurationElement);
            configurationElementCollection.Remove(key);

            Assert.IsFalse(configurationElementCollection.Contains(configurationElement),
                           "After supplying the elements key to remove in order to remove a previously added element from the ConfigurationElementCollection, the contains method should return false for this element.");
        }

        [TestMethod]
        public void Contains_WithElementAddedThenClearCalled_ReturnsFalse()
        {
            ConfigurationElementCollectionTestClass configurationElementCollection =
                GenerateEmptyConfigurationElementCollectionTestClass();

            Guid key;
            ConfigurationElementWithGuidKey configurationElement = GenerateConfigurationElementWithGuidKey(out key);

            configurationElementCollection.Add(configurationElement);
            configurationElementCollection.Clear();

            Assert.IsFalse(configurationElementCollection.Contains(configurationElement),
                           "After using remove to remove a previously added element from the ConfigurationElementCollection, the contains method should return false for this element.");
        }

        [TestMethod]
        public void Contains_WithElementAlreadyAddedUsingIndex_ReturnsTrue()
        {
            ConfigurationElementCollectionTestClass configurationElementCollection =
                GenerateEmptyConfigurationElementCollectionTestClass();

            Guid key;
            ConfigurationElementWithGuidKey configurationElement = GenerateConfigurationElementWithGuidKey(out key);

            configurationElementCollection[0] = configurationElement;

            Assert.IsTrue(configurationElementCollection.Contains(configurationElement),
                          "After assigning an element to the ConfigurationElementCollection by index, the contains method should return true for this element.");
        }

        [TestMethod]
        public void Contains_WithElementAlreadyAddedUsingKey_ReturnsTrue()
        {
            ConfigurationElementCollectionTestClass configurationElementCollection =
                GenerateEmptyConfigurationElementCollectionTestClass();

            Guid key;
            ConfigurationElementWithGuidKey configurationElement = GenerateConfigurationElementWithGuidKey(out key);

            configurationElementCollection[key] = configurationElement;

            Assert.IsTrue(configurationElementCollection.Contains(configurationElement),
                          "After assigning an element to the ConfigurationElementCollection by index, the contains method should return true for this element.");
        }

        [TestMethod]
        public void ICollectionRemove_WithElementAlreadyAdded_ReturnsTrue()
        {
            ConfigurationElementCollectionTestClass configurationElementCollection =
                GenerateEmptyConfigurationElementCollectionTestClass();

            Guid key;
            ConfigurationElementWithGuidKey configurationElement = GenerateConfigurationElementWithGuidKey(out key);

            configurationElementCollection.Add(configurationElement);

            bool wasRemoved =
                ((ICollection<ConfigurationElementWithGuidKey>) configurationElementCollection).Remove(
                    configurationElement);

            Assert.IsTrue(wasRemoved,
                          "The result of ICollection.remove for ConfigurationElementCollection should return true if the element removed was previously added.");
        }

        [TestMethod]
        public void ICollectionRemove_WithElementAlreadyAdded_ReturnsFalse()
        {
            ConfigurationElementCollectionTestClass configurationElementCollection =
                GenerateEmptyConfigurationElementCollectionTestClass();

            Guid key;
            ConfigurationElementWithGuidKey configurationElement = GenerateConfigurationElementWithGuidKey(out key);

            bool wasRemoved =
                ((ICollection<ConfigurationElementWithGuidKey>) configurationElementCollection).Remove(
                    configurationElement);

            Assert.IsFalse(wasRemoved,
                           "The result of ICollection.remove for ConfigurationElementCollection should return false if the element removed was never previously added.");
        }

        [TestMethod]
        public void IsReadOnly_AfterSetReadOnlyCalled_ReturnsTrue()
        {
            ConfigurationElementCollectionTestClass configurationElementCollection =
                GenerateEmptyConfigurationElementCollectionTestClass();

            configurationElementCollection.CallInternalSetReadOnly();

            Assert.IsTrue(configurationElementCollection.IsReadOnly(),
                          "The result of IsReadOnly for ConfigurationElementCollection should return true if the SetReadOnly method was previously called.");
        }

        #region Nested type: ConfigurationElementCollectionTestClass
        private class ConfigurationElementCollectionTestClass :
            ConfigurationElementCollection<Guid, ConfigurationElementWithGuidKey>
        {
            protected override Guid GetElementKey(ConfigurationElementWithGuidKey configurationElement)
            {
                return configurationElement.Key;
            }

            public void CallInternalSetReadOnly()
            {
                SetReadOnly();
            }
        }
        #endregion

        #region Nested type: ConfigurationElementWithGuidKey
        private class ConfigurationElementWithGuidKey : ConfigurationElement
        {
            public Guid Key { get; set; }
        }
        #endregion
    }
}