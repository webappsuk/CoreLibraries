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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebApplications.Utilities.Annotations;

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
        [NotNull]
        private Task<IEnumerable<string>> _packageDependencies;

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
                    .Select(
                        p =>
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
                FileStream fs = new FileStream(
                    config,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    data.Length,
                    true);

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
                    },
                    TaskContinuationOptions.LongRunning);
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