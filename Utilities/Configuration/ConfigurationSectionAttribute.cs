using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Configuration
{
    /// <summary>
    ///   An attribute that allows additional information to be specified for a Configuration Section.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ConfigurationSectionAttribute : Attribute
    {
        /// <summary>
        ///   The configuration section's name.
        /// </summary>
        [NotNull]
        public readonly string Name;

        /// <summary>
        ///   Initializes a new instance of the <see cref="ConfigurationSectionAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the configuration section.</param>
        public ConfigurationSectionAttribute([NotNull]string name)
        {
            Name = name;
        }
    }
}
