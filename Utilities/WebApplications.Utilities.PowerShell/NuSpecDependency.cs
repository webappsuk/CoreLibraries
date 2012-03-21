using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
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
        public NuSpecDependency(string id, string version = null) : this()
        {
            if (String.IsNullOrWhiteSpace(id))
                throw new InvalidOperationException(Resources.NuSpecDependency_DependencyIdEmpty);
            if (id.Contains(","))
                throw new InvalidOperationException(String.Format(Resources.NuSpecDependency_DependencyIdContainsComma, id));
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
            return String.IsNullOrWhiteSpace(Version) ? Id : String.Format("{0}, {1}", Id, Version);
        }
    }
}
