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
using WebApplications.Utilities.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Configuration;

namespace WebApplications.Utilities.Test
{
    [TestClass]
    public class TestConfiguration
    {
        [TestMethod]
        public void TestConstructor()
        {
            ConfigurationSection<TestConfigurationSection>.Changed += (o, e) =>
            {
                Assert.IsNotNull(o);
                Assert.IsNotNull(e);
                Assert.IsNotNull(e.OldConfiguration);
                Assert.IsNotNull(e.NewConfiguration);
                Assert.AreEqual(
                    e.NewConfiguration,
                    TestConfigurationSection.
                        Active);
                Assert.IsTrue(e.NewConfiguration.IsActive);
            };
            TestConfigurationSection section = new TestConfigurationSection();
            string a = section.String;
            string b = section.String2;

            TestConfigurationSection configuration = TestConfigurationSection.Active;
            if (configuration.IsReadOnly())
                configuration = new TestConfigurationSection();
            configuration.String2 = "Another string";
            /*string name = configuration.Constructors.First().Name;
            configuration.Constructors.Remove(name);
             */
            TestConfigurationSection.Active = configuration;
            Assert.IsNotNull(configuration);
            foreach (TestObjectConstructor constructor in configuration.Constructors)
            {
                TestObject obj = constructor.GetInstance<TestObject>();
                Assert.IsNotNull(obj);
            }
        }
    }

    public class TestObject
    {
        public enum TestEnum
        {
            One,
            Two,
            Three
        }
        public readonly int AttribParam;
        public readonly int IntValue;
        public readonly string Name;
        public readonly int Optional;
        public readonly TimeSpan TimeSpan;
        public readonly TestEnum E1;
        public readonly TestEnum E2;
        public readonly TestEnum E3;

        public TestObject(string name)
        {
            Name = name;
        }

        public TestObject(string name, TimeSpan timeSpan)
        {
            Name = name;
            TimeSpan = timeSpan;
        }

        public TestObject(
            string name,
            int intValue,
            int optional = 2,
            int attribParam = 4,
            TimeSpan timeSpan = default(TimeSpan))
        {
            Name = name;
            IntValue = intValue;
            Optional = optional;
            AttribParam = attribParam;
            TimeSpan = timeSpan;
        }

        public TestObject(TestEnum e1, TestEnum e2 = default(TestEnum), TestEnum e3 = TestEnum.Two)
        {
            E1 = e1;
            E2 = e2;
            E3 = e3;
        }

        public TestObject(string name, IEnumerable<TestObject> objects)
        {
        }
    }

    /// <summary>
    /// Test section.
    /// </summary>
    /// <remarks></remarks>
    [UsedImplicitly]
    public class TestConfigurationSection : ConfigurationSection<TestConfigurationSection>
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        /// <remarks></remarks>
        [NotNull]
        [ConfigurationProperty("string", DefaultValue = "A String")]
        public string String
        {
            get { return GetProperty<string>("string"); }
            set { SetProperty("string", value); }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        /// <remarks></remarks>
        [NotNull]
        [ConfigurationProperty("string2", DefaultValue = "A String2")]
        public string String2
        {
            get { return GetProperty<string>("string2"); }
            set { SetProperty("string2", value); }
        }

        /// <summary>
        /// Gets or sets the constructors.
        /// </summary>
        /// <value>The constructors.</value>
        /// <remarks></remarks>
        [ConfigurationProperty("constructors", IsRequired = false, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof (TestObjectConstructorCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public TestObjectConstructorCollection Constructors
        {
            get { return GetProperty<TestObjectConstructorCollection>("constructors"); }
            set { SetProperty("constructors", value); }
        }
    }

    /// <summary>
    /// The collection.
    /// </summary>
    /// <remarks></remarks>
    public class TestObjectConstructorCollection : ConfigurationElementCollection<string, TestObjectConstructor>
    {
        /// <summary>
        /// Gets the element key.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected override string GetElementKey(TestObjectConstructor element)
        {
            return element.Name;
        }
    }

    [ConfigurationSection("testObject")]
    public class TestObjectConstructor : ConstructorConfigurationElement
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        /// <remarks></remarks>
        [NotNull]
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return GetProperty<string>("name"); }
            set { SetProperty("name", value); }
        }

        [NotNull]
        [ConfigurationProperty("value", DefaultValue = "VALUE")]
        public string Value
        {
            get { return GetProperty<string>("value"); }
            set { SetProperty("value", value); }
        }

        /// <summary>
        /// Gets or sets the attrib param.
        /// </summary>
        /// <value>The attrib param.</value>
        /// <remarks></remarks>
        [ConfigurationProperty("attribParam", DefaultValue = -1, IsRequired = false)]
        public int AttribParam
        {
            get { return GetProperty<int>("attribParam"); }
            set { SetProperty("attribParam", value); }
        }
    }
}