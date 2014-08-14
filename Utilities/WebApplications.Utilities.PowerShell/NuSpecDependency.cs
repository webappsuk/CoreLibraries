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
using JetBrains.Annotations;

namespace WebApplications.Utilities.PowerShell
{
    /// <summary>
    /// A nuspec dependency.
    /// </summary>
    public struct NuSpecDependency
    {
        /// <summary>
        /// The Id.
        /// </summary>
        [NotNull]
        public readonly string Id;

        /// <summary>
        /// The Version.
        /// </summary>
        [CanBeNull]
        public readonly string Version;

        /// <summary>
        /// Initializes a new instance of the <see cref="NuSpecDependency"/> struct.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="version">The version.</param>
        /// <remarks></remarks>
        public NuSpecDependency(string id, string version = null)
            : this()
        {
            if (String.IsNullOrWhiteSpace(id))
                throw new InvalidOperationException(Resources.NuSpecDependency_DependencyIdEmpty);
            if (id.Contains(","))
                throw new InvalidOperationException(
                    String.Format(
                        Resources.NuSpecDependency_DependencyIdContainsComma,
                        id));
            Id = id.Trim();
            Version = String.IsNullOrWhiteSpace(version) ? null : version.Trim();
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="WebApplications.Utilities.PowerShell.NuSpecDependency"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="dependency">The dependency.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks></remarks>
        public static implicit operator String(NuSpecDependency dependency)
        {
            return dependency.ToString();
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="WebApplications.Utilities.PowerShell.NuSpecDependency"/>.
        /// </summary>
        /// <param name="dependency">The dependency.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks></remarks>
        public static explicit operator NuSpecDependency(string dependency)
        {
            return Parse(dependency);
        }

        /// <summary>
        /// Parses the specified dependency.
        /// </summary>
        /// <param name="dependency">The dependency.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static NuSpecDependency Parse(string dependency)
        {
            NuSpecDependency result;
            if (!TryParse(dependency, out result))
                throw new FormatException(string.Format(Resources.NuSpecDependency_Parse_InvalidDependency, dependency));
            return result;
        }

        /// <summary>
        /// Tries to parse the specified input into a dependency.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="dependency">The dependency.</param>
        /// <returns><see langword="true"/> if successful; otherwise <see langword="false"/>.</returns>
        /// <remarks></remarks>
        public static bool TryParse(string input, out NuSpecDependency dependency)
        {
            dependency = default(NuSpecDependency);
            if (String.IsNullOrWhiteSpace(input))
                return false;

            string[] split = input.Split(new[] {','}, 2);
            dependency = split.Length != 2 ? new NuSpecDependency(split[0]) : new NuSpecDependency(split[0], split[1]);
            return true;
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> representation of this instance. The format string can be changed in the 
        /// Resources.resx resource file at the key 'NuSpecDependencyToString'.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The format string was a <see langword="null"/>.
        /// </exception>
        /// <exception cref="FormatException">
        /// An index from the format string is either less than zero or greater than or equal to the number of arguments.
        /// </exception>
        public override string ToString()
        {
            return String.IsNullOrWhiteSpace(Version) ? (Id ?? "Unknown ID") : String.Format("{0}, {1}", Id, Version);
        }
    }
}