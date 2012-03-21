#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities.PowerShell
// File: Project.cs
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace WebApplications.Utilities.PowerShell
{
    /// <summary>
    /// Info about projects.
    /// </summary>
    /// <remarks></remarks>
    public class Project : InitialisedSingleton<string, Project>
    {
        /// <summary>
        /// Dependencies.
        /// </summary>
        [NotNull] private Task<IEnumerable<string>> _packageDependencies;

        /// <summary>
        /// Initializes a new instance of the <see cref="Project"/> class.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <remarks></remarks>
        private Project([NotNull] string filename)
            : base(filename)
        {
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Project"/> exists on the file system.
        /// </summary>
        /// <remarks></remarks>
        [UsedImplicitly]
        public bool Exists { get; private set; }

        /// <summary>
        /// The directory.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public string Directory { get; private set; }

        /// <summary>
        /// The project file name.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public string FileName
        {
            get { return Key; }
        }

        /// <summary>
        /// The packages config file.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public string NuSpecPath { get; private set; }

        /// <summary>
        /// The packages config file.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public string PackagesConfig { get; private set; }

        /// <summary>
        /// All known solutions the project is part of.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public IEnumerable<Solution> Solutions
        {
            get { return SolutionProjects.Select(sp => sp.Solution); }
        }

        /// <summary>
        /// Finds all known solution associations.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public IEnumerable<SolutionProject> SolutionProjects
        {
            get { return Enumerable.Empty<SolutionProject>(); }
        }

        /// <summary>
        /// Gets all known projects.
        /// </summary>
        /// <remarks></remarks>
        public static IEnumerable<Project> All
        {
            get { return Singletons.Values; }
        }

        /// <summary>
        /// Gets the dependencies.
        /// </summary>
        /// <remarks></remarks>
        [NotNull]
        [UsedImplicitly]
        public IEnumerable<string> DependencyNames
        {
            get { return _packageDependencies.Result ?? Enumerable.Empty<string>(); }
        }

        /// <summary>
        /// Gets the known dependencies (i.e. known solution projects that this project is dependant upon).
        /// </summary>
        /// <remarks></remarks>
        [NotNull]
        [UsedImplicitly]
        public IEnumerable<SolutionProject> SolutionProjectDependencies
        {
            get
            {
                return DependencyNames
                    .Distinct()
                    .Select(p =>
                                {
                                    IEnumerable<SolutionProject> sps =
                                        SolutionProject.Get(p).Where(
                                            sp => !string.IsNullOrWhiteSpace(sp.Project.NuSpecPath));
                                    if (sps.Count() != 1)
                                        return (SolutionProject) null;
                                    return sps.First();
                                })
                    .Where(sp => sp != null)
                    .Distinct();
            }
        }

        /// <summary>
        /// Gets the known dependencies (i.e. known solutions that that this project is dependant upon).
        /// </summary>
        /// <remarks></remarks>
        [NotNull]
        [UsedImplicitly]
        public IEnumerable<Solution> SolutionDependencies
        {
            get { return SolutionProjectDependencies.Select(sp => sp.Solution).Distinct(); }
        }

        /// <summary>
        /// Gets the known dependencies (i.e. known projects that that this project is dependant upon).
        /// </summary>
        /// <remarks></remarks>
        [NotNull]
        [UsedImplicitly]
        public IEnumerable<Project> ProjectDependencies
        {
            get { return SolutionProjectDependencies.Select(sp => sp.Project).Distinct(); }
        }

        /// <summary>
        /// Initialises this instance.
        /// </summary>
        /// <remarks></remarks>
        protected override void Initialise()
        {
            Exists = File.Exists(FileName);

            // Get the directory name
            Directory = Path.GetDirectoryName(FileName) ?? String.Empty;

            // Look for nuspec file for project.
            string nuspec = Path.ChangeExtension(FileName, ".nuspec");
            NuSpecPath = !String.IsNullOrWhiteSpace(nuspec) && File.Exists(nuspec) ? nuspec : String.Empty;

            // Look for packages configuration
            string config = Path.Combine(Directory, "packages.config");

            if (!File.Exists(config))
            {
                // No packages config
                PackagesConfig = string.Empty;
                // We don't have any package dependencies.
                _packageDependencies = Task<IEnumerable<string>>.Factory.StartNew(Enumerable.Empty<string>);
            }
            else
            {
                // We need to parse the packages config asynchronously.
                PackagesConfig = config;

                // Parse file asynchronously
                FileInfo fi = new FileInfo(config);
                byte[] data = new byte[fi.Length];
                FileStream fs = new FileStream(config, FileMode.Open, FileAccess.Read, FileShare.Read, data.Length, true);

                Task<int> task = Task<int>.Factory.FromAsync(fs.BeginRead, fs.EndRead, data, 0, data.Length, null);

                _packageDependencies = task.ContinueWith(
                    t =>
                        {
                            try
                            {
                                fs.Close();

                                if ((t == null) ||
                                    (t.Exception != null) ||
                                    (t.Status != TaskStatus.RanToCompletion))
                                    return Enumerable.Empty<string>();

                                // If we did not receive the entire file, the end of the
                                // data buffer will contain garbage.
                                if (t.Result < data.Length)
                                    Array.Resize(ref data, t.Result);

                                // Load config XML.
                                XDocument document;
                                using (MemoryStream stream = new MemoryStream(data))
                                    document = XDocument.Load(stream);

                                // Get package elements
                                return (from element in document.Descendants("package")
                                        select element.Attribute("id")
                                        into attribute
                                        where
                                            (attribute != null) &&
                                            (!String.IsNullOrWhiteSpace(attribute.Value))
                                        select attribute.Value).ToList();
                            }
                            catch
                            {
                                // Suppress errors
                                return Enumerable.Empty<string>();
                            }
                        }, TaskContinuationOptions.LongRunning);
            }
        }

        /// <summary>
        /// Gets the project with the specified name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        [NotNull]
        public static Project Get([NotNull] string fileName)
        {
            return GetSingleton(fileName);
        }
        
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        /// <remarks></remarks>
        public override string ToString()
        {
            return FileName;
        }
    }
}