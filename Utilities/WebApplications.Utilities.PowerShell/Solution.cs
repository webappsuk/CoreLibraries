#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities.PowerShell
// File: Solution.cs
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace WebApplications.Utilities.PowerShell
{
    /// <summary>
    /// The solution.
    /// </summary>
    /// <remarks></remarks>
    [UsedImplicitly]
    public class Solution : InitialisedSingleton<string, Solution>, IDisposable
    {
        /// <summary>
        /// Parse projects.
        /// </summary>
        private static readonly Regex _parseProjects =
            new Regex(@"Project\s*\([^)]*\)\s*=\s*\""(?<name>.[^""]+)\""\s*,\s*\""(?<filename>.[^""]+)\""",
                      RegexOptions.IgnoreCase | RegexOptions.Multiline);

        /// <summary>
        /// Cache of solutions in a directory.
        /// </summary>
        private static readonly ConcurrentDictionary<string, IEnumerable<Solution>> _solutionCache =
            new ConcurrentDictionary<string, IEnumerable<Solution>>();

        /// <summary>
        /// The directory.
        /// </summary>
        [NotNull] [UsedImplicitly] public readonly string Directory;

        /// <summary>
        /// The solution name.
        /// </summary>
        [NotNull] [UsedImplicitly] public readonly string Name;

        /// <summary>
        /// The loading task.
        /// </summary>
        private Task<IEnumerable<SolutionProject>> _projects;

        /// <summary>
        /// Initializes a new instance of the <see cref="Solution"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <remarks></remarks>
        private Solution([NotNull] string fileName)
            : base(fileName)
        {
            Directory = Path.GetDirectoryName(fileName) ?? String.Empty;
            Name = Path.GetFileNameWithoutExtension(FileName) ?? String.Empty;
        }

        /// <summary>
        /// Whether the solution actually exists.
        /// </summary>
        [UsedImplicitly]
        public bool Exists { get; private set; }

        /// <summary>
        /// The filename.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public string FileName
        {
            get { return Key; }
        }

        /// <summary>
        /// Gets all known solutions.
        /// </summary>
        /// <remarks></remarks>
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<Solution> All
        {
            get { return Singletons.Values; }
        }

        /// <summary>
        /// Gets the associated projects.
        /// </summary>
        /// <remarks></remarks>
        [NotNull]
        [UsedImplicitly]
        public IEnumerable<SolutionProject> SolutionProjects
        {
            get { return _projects.Result ?? Enumerable.Empty<SolutionProject>(); }
        }

        /// <summary>
        /// Gets the projects for the solution.
        /// </summary>
        /// <remarks></remarks>
        [NotNull]
        [UsedImplicitly]
        public IEnumerable<Project> Projects
        {
            get { return SolutionProjects.Select(sp => sp.Project); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has any projects with a nuget specification.
        /// </summary>
        /// <remarks></remarks>
        [UsedImplicitly]
        public bool HasNuSpecs
        {
            get { return Projects.Any(p => !String.IsNullOrWhiteSpace(p.NuSpecPath)); }
        }

        /// <summary>
        /// Gets the known dependencies (i.e. known solution projects that this solution is dependant upon).
        /// </summary>
        /// <remarks></remarks>
        [NotNull]
        [UsedImplicitly]
        public IEnumerable<SolutionProject> SolutionProjectDependencies
        {
            get
            {
                return Projects
                    .SelectMany(p => p.DependencyNames)
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
        /// Gets a value indicating whether this instance has known dependencies.
        /// </summary>
        /// <remarks></remarks>
        [UsedImplicitly]
        public bool HasKnownDependencies
        {
            get { return SolutionProjectDependencies.Any(); }
        }

        /// <summary>
        /// Gets the known dependencies (i.e. known solutions that that this solution is dependant upon).
        /// </summary>
        /// <remarks></remarks>
        [NotNull]
        [UsedImplicitly]
        public IEnumerable<Solution> SolutionDependencies
        {
            get { return SolutionProjectDependencies.Select(sp => sp.Solution).Distinct(); }
        }

        /// <summary>
        /// Gets the known dependencies (i.e. known projects that that this solution is dependant upon).
        /// </summary>
        /// <remarks></remarks>
        [NotNull]
        [UsedImplicitly]
        public IEnumerable<Project> ProjectDependencies
        {
            get { return SolutionProjectDependencies.Select(sp => sp.Project).Distinct(); }
        }

        #region IDisposable Members
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (_projects != null)
            {
                _projects.Dispose();
                _projects = null;
            }
        }
        #endregion

        /// <summary>
        /// Initialises this instance.
        /// </summary>
        /// <remarks></remarks>
        protected override void Initialise()
        {
            // Check if the file exists.
            Exists = File.Exists(FileName);

            if (!Exists)
            {
                _projects = Task<IEnumerable<SolutionProject>>.Factory.StartNew(Enumerable.Empty<SolutionProject>);
                return;
            }

            // Parse file asynchronously
            FileInfo fi = new FileInfo(FileName);
            byte[] data = new byte[fi.Length];
            FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read, data.Length, true);

            Task<int> task = Task<int>.Factory.FromAsync(fs.BeginRead, fs.EndRead, data, 0, data.Length, null);

            _projects = task.ContinueWith(
                t =>
                    {
                        try
                        {
                            fs.Close();

                            if ((t == null) ||
                                (t.Exception != null) ||
                                (t.Status != TaskStatus.RanToCompletion))
                                return Enumerable.Empty<SolutionProject>();

                            // If we did not receive the entire file, the end of the
                            // data buffer will contain garbage.
                            if (t.Result < data.Length)
                                Array.Resize(ref data, t.Result);

                            // Decode to string
                            string contents = new UTF8Encoding().GetString(data);

                            List<SolutionProject> projects = new List<SolutionProject>();
                            // Look for project definitions
                            foreach (Match match in _parseProjects.Matches(contents))
                            {
                                if (!match.Success ||
                                    !match.Groups["name"].Success ||
                                    !match.Groups["filename"].Success)
                                    continue;

                                string name = match.Groups["name"].Value;
                                string pFile = Path.Combine(Directory, match.Groups["filename"].Value);

                                // If we have a valid project file add it.
                                if (!String.IsNullOrWhiteSpace(name) &&
                                    !String.IsNullOrWhiteSpace(pFile) &&
                                    pFile.EndsWith("proj", StringComparison.InvariantCultureIgnoreCase) &&
                                    File.Exists(pFile))
                                    projects.Add(new SolutionProject(this, pFile, name));
                            }

                            return projects;
                        }
                        catch
                        {
                            // Suppress errors
                            return Enumerable.Empty<SolutionProject>();
                        }
                    }, TaskContinuationOptions.LongRunning);
        }

        /// <summary>
        /// Gets the solution with the specified filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        [CanBeNull]
        public static Solution Get([NotNull] string filename)
        {
            return GetSingleton(filename);
        }

        /// <summary>
        /// Gets all solutions in a directory.
        /// </summary>
        /// <param name="rootPath">The root path.</param>
        /// <param name="recursive">if set to <see langword="true"/> returns all solutions in sub-directories recursively.</param>
        /// <param name="force">if set to <see langword="true"/> forces refresh.</param>
        /// <param name="includeHidden">if set to <see langword="true"/> includes hidden files and folders.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        [NotNull]
        public static IEnumerable<Solution> GetAll([NotNull] string rootPath, bool recursive = false,
                                                   bool force = false, bool includeHidden = false)
        {
            // Get solutions
            IEnumerable<Solution> solutions =
                _solutionCache
                    .GetOrAdd(
                        rootPath,
                        rp =>
                            {
                                // If this is not a valid directory return empty.
                                if (!System.IO.Directory.Exists(rp))
                                    return Enumerable.Empty<Solution>();

                                // Create a list of parsed solution files in this directory
                                // and union with all files from sub directories (recursively)
                                // Skips hidden files & folders automatically.
                                List<Solution> s =
                                    System.IO.Directory.EnumerateFiles(rp, "*.sln", SearchOption.TopDirectoryOnly)
                                        .Select(fn => new FileInfo(fn))
                                        .Where(
                                            f =>
                                            (f.Attributes & FileAttributes.Hidden) ==
                                            0)
                                        .Select(f => Get(f.FullName))
                                        .ToList();

                                // Ensure everything is loaded.
                                foreach (
                                    IEnumerable<string> d in
                                        from solution in s
                                        from project in solution.Projects
                                        select project.DependencyNames)
                                {
                                }

                                return s;
                            }) ?? Enumerable.Empty<Solution>();

            // If we're not recursive just return, otherwise union with sub-directories
            // which of course is itself recursive.
            return !recursive
                       ? solutions
                       : solutions.Union(
                           System.IO.Directory.EnumerateDirectories(rootPath, "*", SearchOption.TopDirectoryOnly)
                               .Select(dn => new DirectoryInfo(dn))
                               .Where(d => (d.Attributes & FileAttributes.Hidden) == 0)
                               .SelectMany(d => GetAll(d.FullName, true, force, includeHidden))
                             );
        }

        /// <summary>
        /// Gets the nuget build order.
        /// </summary>
        /// <param name="rootPath">The root path.</param>
        /// <param name="solutionPath">The optional solution path, if specified will look to see what needs building
        /// if the solution is built first.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<Solution> GetNugetBuildOrder([NotNull] string rootPath,
                                                               [CanBeNull] string solutionPath = null)
        {
            // Get all projects that have nuspecs (without duplications).
            return GetAll(rootPath, true)
                .Where(s => s.HasNuSpecs || s.SolutionDependencies.Any())
                .Distinct()
                .TopologicalSortDependencies(s => s.SolutionDependencies);
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> representation of this instance. The format strings can be changed in the 
        /// Resources.resx resource file at the key 'SolutionToString', 'SolutionProjects' and
        /// 'SolutionProjectDependencies'.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The format string was a <see langword="null"/>.
        /// </exception>
        /// <exception cref="FormatException">
        /// An index from the format string is either less than zero or greater than or equal to the number of arguments.
        /// </exception>
        public override string ToString()
        {
            return String.Format(Resources.Solution_ToString,
                                 Environment.NewLine,
                                 '\t',
                                 Name,
                                 FileName,
                                 SolutionProjects.Any()
                                     ? SolutionProjects.Aggregate(String.Empty,
                                                                  (t, p) => string.Format("{0}{1}{2}{2}{3}",
                                                                                          t,
                                                                                          Environment.NewLine,
                                                                                          '\t',
                                                                                          p))
                                     : "None",
                                 SolutionProjectDependencies.Any()
                                     ? SolutionProjectDependencies.Aggregate(String.Empty,
                                                                             (t, p) =>
                                                                             string.Format("{0}{1}{2}{2}{3}:{4}",
                                                                                           t,
                                                                                           Environment.NewLine,
                                                                                           '\t',
                                                                                           p.Solution.Name,
                                                                                           p.ProjectName))
                                     : "None");
        }
    }
}