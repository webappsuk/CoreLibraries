using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Attribute that is used to decorate assemblies with a semantic version
    /// number.
    /// </summary>
    /// TODO This should be moved to core library
    [SuppressMessage(
        "Microsoft.Design",
        "CA1019:DefineAccessorsForAttributeArguments",
        Justification = "MFC3: The version parameter to the attribute is converted to a SemanticVersion object and exposed through the Version property.")]
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    [Serializable]
    public sealed class AssemblySemanticVersionAttribute : Attribute
    {
        [NotNull]
        private readonly string _semanticVersion;

        /// <summary>
        /// Gets the semantic number.
        /// </summary>
        /// <value>
        /// The semantic number.
        /// </value>
        [NotNull]
        public string SemanticVersion { get { return _semanticVersion; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblySemanticVersionAttribute"/> class.
        /// </summary>
        /// <param name="semanticVersion">
        /// The semantic version number for the assembly.
        /// </param>
        public AssemblySemanticVersionAttribute([NotNull] string semanticVersion)
        {
            Contract.Requires(!string.IsNullOrEmpty(semanticVersion));
            Contract.Ensures(null != SemanticVersion);
            _semanticVersion = semanticVersion;
        }
    }
}
