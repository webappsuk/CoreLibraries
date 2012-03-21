#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities.Test
// File: TestConfiguration.cs
// 
// This software, its object code and source code and all modifications made to
// the same (the “Software”) are, and shall at all times remain, the proprietary
// information and intellectual property rights of Web Applications (UK) Limited. 
// You are only entitled to use the Software as expressly permitted by Web
// Applications (UK) Limited within the Software Customisation and
// Licence Agreement (the “Agreement”).  Any copying, modification, decompiling,
// distribution, licensing, sale, transfer or other use of the Software other than
// as expressly permitted in the Agreement is expressly forbidden.  Web
// Applications (UK) Limited reserves its rights to take action against you and
// your employer in accordance with its contractual and common law rights
// (including injunctive relief) should you breach the terms of the Agreement or
// otherwise infringe its copyright or other intellectual property rights in the
// Software.
// 
// © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
#endregion

using System;
using System.Configuration;
using JetBrains.Annotations;
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
                                                                              Assert.AreEqual(e.NewConfiguration,
                                                                                              TestConfigurationSection.
                                                                                                  Active);
                                                                              Assert.IsTrue(e.NewConfiguration.IsActive);
                                                                          };
            TestConfigurationSection section = new TestConfigurationSection();
            string a = section.String;
            string b = section.String2;

            TestConfigurationSection configuration = TestConfigurationSection.Active;
            if (configuration.IsReadOnly())
            {
                configuration = new TestConfigurationSection();
            }
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
        public readonly int AttribParam;
        public readonly int IntValue;
        public readonly string Name;
        public readonly int Optional;
        public readonly TimeSpan TimeSpan;

        public TestObject(string name)
        {
            Name = name;
        }

        public TestObject(string name, TimeSpan timeSpan)
        {
            Name = name;
            TimeSpan = timeSpan;
        }

        public TestObject(string name, int intValue, int optional = 2, int attribParam = 4,
                          TimeSpan timeSpan = default(TimeSpan))
        {
            Name = name;
            IntValue = intValue;
            Optional = optional;
            AttribParam = attribParam;
            TimeSpan = timeSpan;
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