using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebApplications.Utilities.Initializer.Test
{
    [TestClass]
    public class InstrumentationTest
    {
        /// <summary>
        /// The name of the test project.
        /// </summary>
        private const string TestAppProjectName = "WebApplications.Utilities.Initializer.TestApp";

        /// <summary>
        /// The build message indicitive of success.
        /// </summary>
        private const string AddedMessage =
            "Method 'Initialize' in type 'ModuleInitializer' in assembly '{0}' will be called during Module initialization.";

        /// <summary>
        /// Flags used by <see cref="MoveFileEx"/>.
        /// </summary>
        /// <remarks></remarks>
        [Flags]
        enum MoveFileFlags
        {
            MOVEFILE_REPLACE_EXISTING = 0x00000001,
            MOVEFILE_COPY_ALLOWED = 0x00000002,
            MOVEFILE_DELAY_UNTIL_REBOOT = 0x00000004,
            MOVEFILE_WRITE_THROUGH = 0x00000008,
            MOVEFILE_CREATE_HARDLINK = 0x00000010,
            MOVEFILE_FAIL_IF_NOT_TRACKABLE = 0x00000020
        }

        /// <summary>
        /// Kernal method to move a file
        /// </summary>
        /// <param name="lpExistingFileName">Name of the existing file.</param>
        /// <param name="lpNewFileName">Name of the new file.</param>
        /// <param name="dwFlags">The dw flags.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName,
           MoveFileFlags dwFlags);

        /// <summary>
        /// Builds the test.
        /// </summary>
        /// <remarks>
        /// <para>As we build the test app from this domain then debugging is enabled if you run the test in debug mode!</para>
        /// </remarks>
        [TestMethod]
        [Description("Builds TestApp using the instrumentation library and check that it is instrumented correctly.")]
        public void BuildTest()
        {
            /*
             * Find the initializer assembly
             */
            string instrumentationAssemblyLocation = typeof(InjectModuleInitializer).Assembly.Location;
            Assert.IsTrue(File.Exists(instrumentationAssemblyLocation), "Could not find Initializer assembly at '{0}'.", instrumentationAssemblyLocation);

            /*
             * Find test app project
             */
            const string projectFileName = TestAppProjectName + ".csproj";
            string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Assert.IsNotNull(directory, "Could not find executing assembly directory - location was null.");
            Assert.IsTrue(Directory.Exists(directory), "Could not find executing assembly directory.");

            string root = Path.GetPathRoot(directory);
            string projectFile = null;

            while (directory != root)
            {
                string[] subDirs = Directory.GetDirectories(directory, TestAppProjectName, SearchOption.AllDirectories);

                if (subDirs.Length > 0)
                {
                    // Only one match allowed
                    Assert.AreEqual(1, subDirs.Length, "Found '{1}' sub directories matching '{2}'.{0}{3}",
                                    Environment.NewLine,
                                    subDirs.Length,
                                    TestAppProjectName,
                                    String.Join(Environment.NewLine, subDirs));

                    // Look for project file.
                    string pd = subDirs.Single();
                    Assert.IsNotNull(pd, "Null sub directory!");

                    string[] files = Directory.GetFiles(pd, projectFileName, SearchOption.AllDirectories);
                    if (files.Length > 0)
                    {
                        // Only one match allowed
                        Assert.AreEqual(1, files.Length, "Found '{1}' project files matching '{2}'.{0}{3}",
                                        Environment.NewLine,
                                        files.Length,
                                        projectFileName,
                                        String.Join(Environment.NewLine, files));

                        // Found our file
                        directory = pd;
                        projectFile = files.Single();
                        break;
                    }
                }

                // Go up a directory
                directory = Path.GetDirectoryName(directory);
            }

            // Check we have the project
            Assert.AreNotEqual(root, directory, "Could not find the '{0}' project - reached root '{1}'.",
                               projectFileName, root);
            Assert.IsNotNull(projectFile, "Could not find the '{0}' project.", projectFileName);

            /*
             * Create a copy of the instrumentation library - this stops the problems that occur with locking.
             */
            string assemblyExtension = Path.GetExtension(instrumentationAssemblyLocation);
            string tempInstrumentationAssemblyLocation = Path.ChangeExtension(directory + Path.DirectorySeparatorChar + Guid.NewGuid(),
                                                       assemblyExtension);
            Assert.IsNotNull(tempInstrumentationAssemblyLocation, "Temporary location for instrumentation assembly is null!");
            Assert.IsFalse(File.Exists(tempInstrumentationAssemblyLocation), "Temporary location for instrumentation assembly already exists.");
            File.Copy(instrumentationAssemblyLocation, tempInstrumentationAssemblyLocation);

            try
            {
                // Load the project
                Project project = new Project(projectFile);
                UnitTestBuildLogger logger = new UnitTestBuildLogger();

                // Find solution directory
                string solutionDirectory = directory;
                while (solutionDirectory != root)
                {
                    if (Directory.GetFiles(solutionDirectory, "*.sln", SearchOption.TopDirectoryOnly).Any())
                        break;

                    // Go up a directory
                    solutionDirectory = Path.GetDirectoryName(solutionDirectory);
                }

                Assert.AreNotEqual(root, solutionDirectory,
                                   "Could not find solution directory for '{0}' project - reached root '{1}'",
                                   projectFile, root);

                // Variable to hold the newly created assembly.
                string instrumentedAssemblyLocation;
                
                // Create a project collection
                using (ProjectCollection projectCollection = new ProjectCollection(ToolsetDefinitionLocations.Registry | ToolsetDefinitionLocations.ConfigurationFile))
                {
                    // Set up environment - explicitly setting path to tool.
                    BuildRequestData requestData = new BuildRequestData(project.FullPath,
                                                                        new Dictionary<string, string>
                                                                            {
                                                                                {"SolutionDir", solutionDirectory},
                                                                                {
                                                                                    "InjectModuleInitializerTool",
                                                                                    tempInstrumentationAssemblyLocation
                                                                                    }
                                                                            },
                                                                        project.ToolsVersion,
                                                                        new[] {"Rebuild"},
                                                                        null);

                    // Set parameters.
                    BuildParameters parameters = new BuildParameters(projectCollection)
                                                     {
                                                         Loggers = new[] {logger},
                                                         ToolsetDefinitionLocations = projectCollection.ToolsetLocations,
                                                         NodeExeLocation = Assembly.GetExecutingAssembly().Location
                                                     };

                    // Get build manager and reset any caches.
                    BuildManager buildManager = BuildManager.DefaultBuildManager;
                    buildManager.ResetCaches();

                    // Build
                    BuildResult result = buildManager.Build(parameters, requestData);

                    // Check it built correctly.
                    Assert.AreEqual(BuildResultCode.Success, result.OverallResult, "Project failed to build.{0}{1}",
                                    Environment.NewLine, logger.Errors);
                    Assert.IsTrue(String.IsNullOrWhiteSpace(logger.Errors), "The build reported errors.{0}{1}",
                                  Environment.NewLine, logger.Errors);

                    // Try to get output path from build results.
                    string targetPath = null;
                    TargetResult targetResult;
                    if (result.ResultsByTarget.TryGetValue("Build", out targetResult) && (targetResult.Items.Any()))
                        targetPath = targetResult.Items.First().ItemSpec;

                    instrumentedAssemblyLocation = targetPath ?? project.GetPropertyValue("TargetPath");

                    // Check instrumented assembly.
                    Assert.IsNotNull(instrumentedAssemblyLocation, "No instrumented assembly found after build.");
                    Assert.IsTrue(File.Exists(instrumentedAssemblyLocation),
                                  "Could not find instrumented assembly '{0}'.", instrumentedAssemblyLocation);

                    // Check for success message in build output.
                    string successMessage = string.Format(AddedMessage, instrumentedAssemblyLocation);
                    Assert.IsTrue(logger.Output.Contains(successMessage), "The build output did not contain '{0}'.",
                                  successMessage);
                }

                DateTime beforeLoad = DateTime.Now;

                // Load instrumented assembly to test.
                Assembly builtAssembly = Assembly.LoadFile(instrumentedAssemblyLocation);
                Assert.IsNotNull(builtAssembly, "Failed to load instrumented assembly '{0}'.");
                
                Type initializerType =
                    builtAssembly.GetTypes().FirstOrDefault(
                        t => t.Name.Equals("ModuleInitializer", StringComparison.InvariantCultureIgnoreCase));
                
                Assert.IsNotNull(initializerType, "Could not find initializer type in instrumented assembly '{0}'.", instrumentedAssemblyLocation);
                
                // Get InitializerHit value.
                FieldInfo initializerHitField = initializerType.GetField("InitializerHit",
                                          BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                Assert.IsNotNull(initializerHitField, "Could not find InitializerHit field on type '{0}' in instrumented assembly '{1}'.",
                                 initializerType.FullName, instrumentedAssemblyLocation);
                DateTime initializerHit = (DateTime) initializerHitField.GetValue(null);

                DateTime afterFirstAccess = DateTime.Now;
                
                // Check initializer was hit by the time the first access occurred.
                Assert.AreNotEqual(default(DateTime), initializerHit, "The initializer was not hit.");
                Assert.IsTrue(initializerHit > beforeLoad, "The initializer was hit at '{0:mm:ss.fffffff}' before the assembly was loaded at '{1:mm:ss.fffffff}'!", initializerHit, beforeLoad);
                Assert.IsTrue(initializerHit < afterFirstAccess, "The initializer was hit at '{0:mm:ss.fffffff}' after the assembly was finished loading at '{1:mm:ss.fffffff}'!", initializerHit, afterFirstAccess);

                // Get StaticConstructorHit value.
                FieldInfo staticConstructorHitField = initializerType.GetField("StaticConstructorHit",
                                          BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                Assert.IsNotNull(staticConstructorHitField, "Could not find StaticConstructorHit field on type '{0}' in instrumented assembly '{1}'.",
                                 initializerType.FullName, instrumentedAssemblyLocation);
                DateTime staticConstructorHit = (DateTime)staticConstructorHitField.GetValue(null);

                // Check the static constructor was hit before the initializer method itself was hit.
                Assert.AreNotEqual(default(DateTime), staticConstructorHit, "The initializer static constructor was not hit.");
                Assert.IsTrue(staticConstructorHit > beforeLoad, "The initializer static constructor was hit at '{0:mm:ss.fffffff}' before the assembly was loaded at '{1:mm:ss.fffffff}'!", staticConstructorHit, beforeLoad);
                Assert.IsTrue(staticConstructorHit < initializerHit, "The initializer static constructor was hit at '{0:mm:ss.fffffff}' after the initializer method was hit at '{1:mm:ss.fffffff}'!", staticConstructorHit, initializerHit);
                Assert.IsTrue(staticConstructorHit < afterFirstAccess, "The initializer static constructor was hit at '{0:mm:ss.fffffff}' after the assembly was finished loading at '{1:mm:ss.fffffff}'!", staticConstructorHit, afterFirstAccess);
            }
            finally
            {
                // Lock for all assemblies in directory.
                string pattern = Path.ChangeExtension("*", assemblyExtension);
                foreach (string tempAssembly in Directory.GetFiles(directory, pattern))
                {
                    if (String.IsNullOrWhiteSpace(tempAssembly))
                        continue;

                    // If the assembly has a guid name we can delete it.
                    string guidStr = Path.GetFileNameWithoutExtension(tempAssembly);
                    Guid guid;
                    if (!Guid.TryParse(guidStr, out guid)) continue;

                    try
                    {
                        // Try to delete immediately
                        File.Delete(tempAssembly);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Delete file on next reboot as locked.
                        MoveFileEx(tempAssembly, null, MoveFileFlags.MOVEFILE_DELAY_UNTIL_REBOOT);
                    }
                }
            }
        }
    }
}
