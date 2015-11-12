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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebApplications.Testing;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Configuration;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.Test
{
    [TestClass]
    [DeploymentItem("test.config")]
    public class TestConfiguration : UtilitiesTestBase
    {
        [NotNull]
        private static readonly List<TaskCompletionSource<TestConfigurationSection.ConfigurationChangedEventArgs>> _waiters
            = new List<TaskCompletionSource<TestConfigurationSection.ConfigurationChangedEventArgs>>();


        [NotNull]
        private static readonly AsyncLock _configFileLock = new AsyncLock();

        static TestConfiguration()
        {
            TestConfigurationSection.ActiveChanged += (s, e) =>
            {
                Trace.WriteLine($"Active configuration {e}");
                lock (_waiters)
                {
                    foreach (TaskCompletionSource<TestConfigurationSection.ConfigurationChangedEventArgs> tcs in _waiters)
                        tcs.TrySetResult(e);
                    _waiters.Clear();
                }
            };
            TestConfigurationSection.ConfigurationLoadError +=
                (e) => Trace.WriteLine($"Error occurred whilst loading {e.SectionName}.\r\n{e.Exception}");
        }

        /// <summary>
        /// Waits for changes, and returns alls changes since the wait started.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>All changes.</returns>
        public Task<TestConfigurationSection.ConfigurationChangedEventArgs> WaitForChanges(CancellationToken cancellationToken = default(CancellationToken))
        {
            TaskCompletionSource<TestConfigurationSection.ConfigurationChangedEventArgs> tcs =
                new TaskCompletionSource<TestConfigurationSection.ConfigurationChangedEventArgs>();

            lock (_waiters)
                _waiters.Add(tcs);
            return tcs.Task.WithCancellation(cancellationToken);
        }
        
        [TestMethod]
        public void TestConstructor()
        {
            using (TestConfigurationSection section = TestConfigurationSection.Create())
            {
                Assert.IsNotNull(section);

                // This tests that we can populate with top level properties (rather than parameters collection).
                Tuple<string, string, int>[] parameters = Enumerable.Range(0, 10)
                    .Select(
                        i =>
                            new Tuple<string, string, int>(
                            Tester.RandomString(minLength: 1),
                            Tester.RandomString(nullProbability: 0.3),
                            i))
                    .ToArray();
                foreach (Tuple<string, string, int> tuple in parameters)
                    section.Constructors.Add(new TestObjectConstructor(tuple.Item1, tuple.Item2, tuple.Item3));

                Assert.AreEqual(parameters.Length, section.Constructors.Count);

                int c = 0;
                foreach (TestObjectConstructor constructor in section.Constructors)
                {
                    TestObject obj = constructor.GetInstance<TestObject>();
                    Assert.IsNotNull(obj);

                    Tuple<string, string, int> tuple = parameters[c++];
                    Assert.AreEqual(tuple.Item1, obj.Name);
                    Assert.AreEqual(tuple.Item2, constructor.Value);
                    Assert.AreEqual(tuple.Item3, obj.AttribParam);
                }
            }
        }

        [TestMethod]
        public async Task TestChangeNotificationAndSave()
        {
            // Prevent multiple tests accessing the app config.
            using (await _configFileLock.LockAsync())
            {
                Assert.IsTrue(TestConfigurationSection.Active.HasFile);
                XElement custom = TestConfigurationSection.Active.Custom;
                Assert.IsNotNull(custom);
                XElement custom2 = TestConfigurationSection.Active.Custom;
                Assert.IsNotNull(custom2);
                Assert.AreNotSame(
                    custom,
                    custom2,
                    "Get element should retrieve fresh XElement objects to prevent corruption.");

                // Get original values.
                string str1 = TestConfigurationSection.Active.String;
                string str2 = TestConfigurationSection.Active.String2;
                string cStr = custom.ToString(SaveOptions.DisableFormatting);
                Assert.AreEqual(cStr, custom2.ToString(SaveOptions.DisableFormatting));
                Assert.AreEqual("custom", custom.Name.LocalName);

                // Generate random values
                string str1Random = Tester.RandomString() ?? string.Empty;
                string str2Random = Tester.RandomString() ?? string.Empty;

                // Update the custom node.
                XNamespace ns = custom.Name.Namespace;
                TestConfigurationSection.Active.Custom = new XElement(
                    ns + "custom",
                    new XElement(ns + "string1", str1Random),
                    new XElement(ns + "string2", str2Random));

                TestConfigurationSection.Active.String = str1Random;
                TestConfigurationSection.Active.String2 = str2Random;
                TestConfigurationSection.Active.Save();

                // Check for changes
                TestConfigurationSection.ConfigurationChangedEventArgs change =
                    await WaitForChanges(new CancellationTokenSource(TimeSpan.FromSeconds(1)).Token);
                Assert.IsNotNull(change);
                Assert.IsTrue(change.WasChanged("<test>"));
                Assert.IsTrue(change.Contains("<test><custom>"));
                Assert.IsTrue(change.Contains("<test>.string"));
                Assert.IsTrue(change.Contains("<test>.string2"));

                custom = TestConfigurationSection.Active.Custom;
                Assert.IsNotNull(custom, $"#{TestConfigurationSection.Active.InstanceNumber} custom was null.");
                Assert.AreEqual("custom", custom.Name.LocalName);
                Assert.AreEqual(str1Random, custom.Element("string1")?.Value);
                Assert.AreEqual(str2Random, custom.Element("string2")?.Value);
                Assert.AreEqual(str1Random, TestConfigurationSection.Active.String);
                Assert.AreEqual(str2Random, TestConfigurationSection.Active.String2);

                // Revert

                // Update the custom node.
                TestConfigurationSection.Active.Custom = XElement.Parse(cStr, LoadOptions.PreserveWhitespace);
                TestConfigurationSection.Active.String = str1;
                TestConfigurationSection.Active.String2 = str2;
                TestConfigurationSection.Active.Save();

                // Check for changeschange =
                change = await WaitForChanges(new CancellationTokenSource(TimeSpan.FromSeconds(1)).Token);
                Assert.IsNotNull(change);
                Assert.IsTrue(change.WasChanged("<test>"));
                Assert.IsTrue(change.Contains("<test><custom>"));
                Assert.IsTrue(change.Contains("<test>.string"));
                Assert.IsTrue(change.Contains("<test>.string2"));

                custom = TestConfigurationSection.Active.Custom;
                Assert.IsNotNull(change);
                Assert.AreEqual("custom", custom.Name.LocalName);
                Assert.AreEqual(cStr, custom.ToString(SaveOptions.DisableFormatting));
                Assert.AreEqual(str1, TestConfigurationSection.Active.String);
                Assert.AreEqual(str2, TestConfigurationSection.Active.String2);
            }
        }

        [TestMethod]
        public async Task TestFileChangeNotification()
        {
            // Prevent multiple tests accessing the app config.
            using (await _configFileLock.LockAsync())
            {
                // Get the test config file path.
                string filePath = TestConfigurationSection.Active.FilePath;

                Trace.WriteLine($"Configuration file: '{filePath}'");

                Assert.IsFalse(string.IsNullOrWhiteSpace(filePath));
                Assert.IsTrue(File.Exists(filePath));

                // Load config file directly (we load into string so we can recreate at end of the test.
                string original = File.ReadAllText(filePath);

                // Parse it and modify
                XDocument document = XDocument.Parse(original);
                XElement testElement = document.Element("test");
                Assert.IsNotNull(testElement);

                XElement customElement = testElement.Element("custom");
                Tuple<XObject, XObject> difference = customElement.DeepEquals(
                    TestConfigurationSection.Active.Custom,
                    XObjectComparisonOptions.Semantic);
                Assert.IsNull(difference, $"{difference?.Item1} : {difference?.Item2}");

                // Modify
                string str1Random = Tester.RandomString() ?? string.Empty;
                string str2Random = Tester.RandomString() ?? string.Empty;

                // Update the custom node.
                customElement.RemoveAll();
                XNamespace ns = customElement.Name.Namespace;
                customElement.Add(
                    new XElement(ns + "string1", str1Random),
                    new XElement(ns + "string2", str2Random));

                // Save file
                document.Save(filePath);

                // Check for changes
                TestConfigurationSection.ConfigurationChangedEventArgs change =
                    await WaitForChanges(new CancellationTokenSource(TimeSpan.FromSeconds(1)).Token);
                Assert.IsNotNull(change);
                Assert.IsTrue(change.Contains("<test>"));
                Assert.IsTrue(change.WasChanged("<test><custom>"));

                XElement activeCustom = TestConfigurationSection.Active.Custom;
                Assert.IsNotNull(activeCustom);
                Assert.AreEqual("custom", activeCustom.Name.LocalName);
                Assert.AreEqual(str1Random, activeCustom.Element("string1")?.Value);
                Assert.AreEqual(str2Random, activeCustom.Element("string2")?.Value);

                // Revert
                File.WriteAllText(filePath, original);

                change = await WaitForChanges(new CancellationTokenSource(TimeSpan.FromSeconds(1)).Token);
                Assert.IsNotNull(change);
                Assert.IsTrue(change.Contains("<test>"));
                Assert.IsTrue(change.WasChanged("<test><custom>"));
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
        public readonly string StrValue;

        public TestObject(string name, int attribParam = 4)
        {
            Name = name;
            AttribParam = attribParam;
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
        [ConfigurationProperty("string", IsRequired = true)]
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
        /// Gets or sets the custom <see cref="XElement"/>.
        /// </summary>
        /// <value>The custom.</value>
        public XElement Custom
        {
            get { return GetElement("custom"); }
            set { SetElement("custom", value); }
        }

        /// <summary>
        /// Gets or sets the constructors.
        /// </summary>
        /// <value>The constructors.</value>
        /// <remarks></remarks>
        [ConfigurationProperty("constructors", IsRequired = false, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(TestObjectConstructorCollection),
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

        /// <summary>
        /// Initializes a new instance of the <see cref="TestObjectConstructor"/> class.
        /// </summary>
        public TestObjectConstructor()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestObjectConstructor"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="attribParam">The attribute parameter.</param>
        public TestObjectConstructor([NotNull] string name, [NotNull]  string value = "VALUE", int attribParam = -1)
        {
            Type = typeof(TestObject);
            Name = name;
            Value = value;
            AttribParam = attribParam;
        }
    }
}