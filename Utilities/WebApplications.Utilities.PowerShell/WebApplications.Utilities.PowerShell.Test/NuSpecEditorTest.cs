using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebApplications.Utilities.PowerShell.Test
{
    [TestClass]
    public class NuSpecEditorTest
    {
        private const string TestSpec = "Test.nuspec";

        public static string NuSpecPath;

        [AssemblyInitialize()]
        public static void AssemblyInit(TestContext context)
        {
            /*
             * Copies the embedded resource to a temporary file.
             */
            Type testType = typeof(NuSpecEditorTest);
            Assembly testAssembly = testType.Assembly;
            string resourceName = testType.Namespace + "." + TestSpec;

            // Get temporary file name for nuspec.
            NuSpecPath = Path.ChangeExtension(Path.GetTempFileName(), ".nuspec");

            // Write out resource steam to file.
            using (BinaryReader reader = new BinaryReader(testAssembly.GetManifestResourceStream(resourceName)))
            {
                using (BinaryWriter writer = new BinaryWriter(new FileStream(NuSpecPath, FileMode.Create)))
                {
                    long bytesLeft = reader.BaseStream.Length;
                    while (bytesLeft > 0)
                    {
                        // 65535L is < Int32.MaxValue, so no need to test for overflow
                        byte[] chunk = reader.ReadBytes((int)Math.Min(bytesLeft, 65536L));
                        writer.Write(chunk);

                        bytesLeft -= chunk.Length;
                    }
                }
            }

            Assert.IsTrue(File.Exists(NuSpecPath), "Could not find test spec '{0}'.", NuSpecPath);
        }

        [AssemblyCleanup()]
        public static void AssemblyCleanup()
        {
            File.Delete(NuSpecPath);
        }


        [TestMethod]
        public void TestConstructor()
        {
            Assert.IsFalse(String.IsNullOrWhiteSpace(NuSpecPath), "The nuspec path was empty!");

            // Create editor.
            NuSpecEditor editor = new NuSpecEditor(NuSpecPath);

            Assert.IsNotNull(editor, "The nuspec editor object was not found!");
            Assert.AreEqual(NuSpecPath, editor.Path,
                            "The nuspec path as reported by the editor '{0}' did not match the expected path '{1}.",
                            editor.Path, NuSpecPath);

            Assert.IsNotNull(editor.Document, "The nuspec editor did not load the XML Document from the spec.");
            Assert.IsNotNull(editor.MetadataElement, "The nuspec editor did not find the metadata element.");
        }

        [TestMethod]
        public void TestMetadataManipulation()
        {
            Assert.IsFalse(String.IsNullOrWhiteSpace(NuSpecPath), "The nuspec path was empty!");

            // Create editor.
            NuSpecEditor editor = new NuSpecEditor(NuSpecPath);

            Assert.AreEqual(editor.LicenseUrl, "https://nuget.webappuk.com/license.htm");
            Assert.IsFalse(editor.HasChanges, "Editor should not have registered any changes yet.");

            string id = editor.ID;
            Assert.IsFalse(editor.HasChanges, "Editor should not have registered any changes yet.");

            editor.ID = "1.0";
            Assert.IsTrue(editor.HasChanges, "Editor should have registered a change.");

            Assert.AreEqual("1.0", editor.ID, "Editor did not update ID correctly");

            editor.Save();
            Assert.IsFalse(editor.HasChanges, "Editor should not have any changes after saving.");
        }

        [TestMethod]
        public void TestEnsureDependencies()
        {
            Assert.IsFalse(String.IsNullOrWhiteSpace(NuSpecPath), "The nuspec path was empty!");

            // Create editor.
            NuSpecEditor editor = new NuSpecEditor(NuSpecPath);

            // Check for pre-existing dependency with version
            int dcount = editor.Dependencies.Count();
            Assert.IsTrue(dcount > 0, "There are no existing dependencies in the nuspec.");

            NuSpecDependency existingDependency =
                editor.Dependencies.FirstOrDefault(d => !String.IsNullOrWhiteSpace(d.Version));

            Assert.AreNotEqual(default(NuSpecDependency), existingDependency,
                               "The nuspec did not already contain a pre-existing versioned dependency");

            // Neither of these should require chagnes
            editor.EnsureDependency(existingDependency.Id);
            editor.EnsureDependency(existingDependency);

            Assert.AreNotEqual(default(NuSpecDependency), editor.Dependencies.FirstOrDefault(
                d => d.Id.Equals(existingDependency.Id, StringComparison.OrdinalIgnoreCase) &&
                     (d.Version != null) &&
                     d.Version.Equals(existingDependency.Version, StringComparison.OrdinalIgnoreCase)),
                             "The existing dependency '{0}' disappeared!", existingDependency);

            Assert.IsFalse(editor.HasChanges, "Editor should not have registered any changes yet.");

            string newDepId = Guid.NewGuid().ToString();
            string newDepVers = Guid.NewGuid().ToString();
            editor.EnsureDependency(newDepId);
            editor.EnsureDependency(new NuSpecDependency(newDepId, newDepVers));
            Assert.AreNotEqual(default(NuSpecDependency), editor.Dependencies.SingleOrDefault(
                d => d.Id.Equals(newDepId, StringComparison.OrdinalIgnoreCase) &&
                     (d.Version != null) &&
                     d.Version.Equals(newDepVers, StringComparison.OrdinalIgnoreCase)),
                             "The '{0}, {1}' dependency was not added.", newDepId, newDepVers);
            Assert.IsTrue(editor.HasChanges, "Editor should have registered a change.");

            Assert.AreEqual(dcount + 1, editor.Dependencies.Count(), "The number of dependencies should have increased by one!");
        }

        [TestMethod]
        public void TestRemoveDependencies()
        {
            Assert.IsFalse(String.IsNullOrWhiteSpace(NuSpecPath), "The nuspec path was empty!");

            // Create editor.
            NuSpecEditor editor = new NuSpecEditor(NuSpecPath);

            // Get current dependencies count
            int dcount = editor.Dependencies.Count();

            Random random = new Random();
            int addCount = random.Next(2, 10);

            // Create random dependencies to add
            List<NuSpecDependency> dependencies = new List<NuSpecDependency>(addCount);
            for (int i = 0; i < addCount; i++)
            {
                NuSpecDependency newDependency = new NuSpecDependency(
                    Guid.NewGuid().ToString(),
                    random.Next(2) == 0
                        ? null
                        : Guid.NewGuid().ToString());
                dependencies.Add(newDependency);
                editor.EnsureDependency(newDependency);
            }

            // Check dependencies added
            Assert.IsTrue(editor.HasChanges, "Editor should have registered a change.");
            Assert.AreEqual(dcount + addCount, editor.Dependencies.Count(), "The number of dependencies should have increased by one!");
            foreach (NuSpecDependency dependency in dependencies)
            {
                Assert.AreNotEqual(default(NuSpecDependency), editor.Dependencies.SingleOrDefault(
                    d => d.Id.Equals(dependency.Id, StringComparison.OrdinalIgnoreCase) &&
                         (dependency.Version == null) || ((d.Version != null) &&
                                                          d.Version.Equals(dependency.Version,
                                                                           StringComparison.OrdinalIgnoreCase))),
                                   "The '{0}, {1}' dependency was not added.", dependency.Id, dependency.Version);
            }

            // Remove dependencies in one go
            editor.RemoveDependencies(String.Join(";", dependencies));

            // Check dependencies were removed
            Assert.AreEqual(dcount, editor.Dependencies.Count(), "The number of dependencies should have been restored after removal!");
            foreach (NuSpecDependency dependency in dependencies)
            {
                Assert.AreEqual(default(NuSpecDependency), editor.Dependencies.SingleOrDefault(
                    d => d.Id.Equals(dependency.Id, StringComparison.OrdinalIgnoreCase) &&
                         (dependency.Version == null) || ((d.Version != null) &&
                                                          d.Version.Equals(dependency.Version,
                                                                           StringComparison.OrdinalIgnoreCase))),
                                "The '{0}, {1}' dependency was not removed.", dependency.Id, dependency.Version);
            }
        }
    }
}
