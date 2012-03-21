#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities.PowerShell
// File: SolutionProject.cs
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
using JetBrains.Annotations;
using WebApplications.Utilities.Caching;

namespace WebApplications.Utilities.PowerShell
{
    /// <summary>
    /// Holds information about projects in a solution.
    /// </summary>
    /// <remarks></remarks>
    [UsedImplicitly]
    public sealed class SolutionProject : IObservableFinalize
    {
        /// <summary>
        /// Holds references to projects by name.
        /// </summary>
        private static readonly WeakConcurrentLookup<string, SolutionProject> _solutionProjectsByName =
            new WeakConcurrentLookup<string, SolutionProject>();

        /// <summary>
        /// Holds references to projects by name.
        /// </summary>
        private static readonly WeakConcurrentLookup<Project, SolutionProject> _solutionProjectsByProject =
            new WeakConcurrentLookup<Project, SolutionProject>();

        /// <summary>
        /// The project.
        /// </summary>
        [NotNull] [UsedImplicitly] public readonly Project Project;

        /// <summary>
        /// The name of the project in the solution.
        /// </summary>
        [NotNull] [UsedImplicitly] public readonly string ProjectName;

        /// <summary>
        /// The solution.
        /// </summary>
        [NotNull] [UsedImplicitly] public readonly Solution Solution;

        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionProject"/> class.
        /// </summary>
        /// <param name="solution">The solution.</param>
        /// <param name="projectFileName">Name of the project file.</param>
        /// <param name="projectName">Name of the project.</param>
        /// <remarks></remarks>
        internal SolutionProject([NotNull] Solution solution, [NotNull] string projectFileName,
                                 [NotNull] string projectName)
        {
            Solution = solution;
            Project = Project.Get(projectFileName);
            ProjectName = projectName;
            _solutionProjectsByName.Add(projectName, this);
        }

        /// <summary>
        /// Gets all known projects with the specified name.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static IEnumerable<SolutionProject> Get([NotNull] string projectName)
        {
            return _solutionProjectsByName[projectName];
        }

        /// <summary>
        /// Gets all known projects with the specified name.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static IEnumerable<SolutionProject> Get([NotNull] Project project)
        {
            return _solutionProjectsByProject[project];
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        /// <remarks></remarks>
        public override string ToString()
        {
            return string.Format("{0}{1}", ProjectName, !string.IsNullOrWhiteSpace(Project.NuSpecPath) ? " *NUGET*" : "");
        }
        
        private EventHandler _finalized;

        /// <inheritdoc />
        public event EventHandler Finalized
        {
            add
            {
                if (_finalized == null)
                    GC.ReRegisterForFinalize(this);

                _finalized += value;
            }

            remove
            {
                _finalized -= value;

                if (_finalized == null)
                    GC.SuppressFinalize(this);
            }
        }

        /// <inheritdoc />
        ~SolutionProject()
        {
            if (_finalized != null)
                _finalized(this, EventArgs.Empty);
        }
    }
}