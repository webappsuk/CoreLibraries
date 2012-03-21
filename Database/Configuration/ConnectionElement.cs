using System.Configuration;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Database.Configuration
{
    /// <summary>
    ///   An element that represents a single connection.
    /// </summary>
    public class ConnectionElement : Utilities.Configuration.ConfigurationElement
    {
        /// <summary>
        ///   Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        /// <exception cref="ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        [ConfigurationProperty("connectionString", IsRequired = true, IsKey = true)]
        [NotNull]
        public string ConnectionString
        {
            get { return GetProperty<string>("connectionString"); }
            set { SetProperty("connectionString", value); }
        }

        /// <summary>
        ///   Gets or sets a <see cref="bool"/> value indicating whether the connection is enabled.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if the connection is enabled; otherwise <see langword="false"/>.
        /// </value>
        /// <exception cref="ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        [ConfigurationProperty("enabled", DefaultValue = true, IsRequired = false)]
        public bool Enabled
        {
            get { return GetProperty<bool>("enabled"); }
            set { SetProperty("enabled", value); }
        }

        /// <summary>
        ///   Gets or sets a <see cref="double"/> indicating the relative weight of the connection.
        /// </summary>
        /// <value>
        ///   <para>The weight of the connection.</para>
        ///   <para>The default weighting is 1.0.</para>
        /// </value>
        /// <exception cref="ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        [ConfigurationProperty("weight", DefaultValue = 1.0D, IsRequired = false)]
        public double Weight
        {
            get { return GetProperty<double>("weight"); }
            set { SetProperty("weight", value); }
        }
    }
}
