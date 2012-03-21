using System;
using System.Configuration;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Database.Configuration
{
    /// <summary>
    ///   A configuration element that represents a <see cref="WebApplications.Utilities.Database.SqlProgram"/>'s parameter.
    /// </summary>
    public class ParameterElement : Utilities.Configuration.ConfigurationElement
    {
        /// <summary>
        ///   Gets or sets the identifier for the parameter.
        /// </summary>
        /// <value>The name of the element.</value>
        /// <exception cref="ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        [ConfigurationProperty("name", IsRequired = true)]
        [NotNull]
        public string Name
        {
            get { return GetProperty<string>("name"); }
            set { SetProperty("name", value); }
        }

        /// <summary>
        ///   Gets or sets the mapTo property, which maps the element to the
        ///   <see cref="WebApplications.Utilities.Database.SqlProgram"/> parameter it represents.
        /// </summary>
        /// <value>The <see cref="string"/> specifying the parameter to map to.</value>
        /// <exception cref="ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        [ConfigurationProperty("mapTo", IsRequired = true)]
        [NotNull]
        public string MapTo
        {
            get { return GetProperty<string>("mapTo"); }
            set { SetProperty("mapTo", value); }
        }
    }
}