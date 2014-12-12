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
using WebApplications.Utilities.Annotations;
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
        [NotNull]
        [UsedImplicitly]
        public readonly Project Project;

        /// <summary>
        /// The name of the project in the solution.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public readonly string ProjectName;

        /// <summary>
        /// The solution.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public readonly Solution Solution;

        private EventHandler _finalized;

        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionProject"/> class.
        /// </summary>
        /// <param name="solution">The solution.</param>
        /// <param name="projectFileName">Name of the project file.</param>
        /// <param name="projectName">Name of the project.</param>
        /// <remarks></remarks>
        internal SolutionProject(
            [NotNull] Solution solution,
            [NotNull] string projectFileName,
            [NotNull] string projectName)
        {
            Solution = solution;
            Project = Project.Get(projectFileName);
            ProjectName = projectName;
            _solutionProjectsByName.Add(projectName, this);
        }

        #region IObservableFinalize Members
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
        #endregion

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
            return string.Format(
                "{0}{1}",
                ProjectName,
                !string.IsNullOrWhiteSpace(Project.NuSpecPath) ? " *NUGET*" : "");
        }

        /// <inheritdoc />
        ~SolutionProject()
        {
            if (_finalized != null)
                _finalized(this, EventArgs.Empty);
        }
    }
}