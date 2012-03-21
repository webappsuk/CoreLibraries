using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Configuration
{
    /// <summary>
    ///   Represents a configuration element in a configuration file.
    /// </summary>
    public class ConfigurationElement : System.Configuration.ConfigurationElement
    {
        /// <summary>
        ///   Gets the configuration property.
        /// </summary>
        /// <typeparam name="T">The property type.</typeparam>
        /// <param name="propertyName">The name of the property to get.</param>
        /// <returns>The specified property, attribute or child element.</returns>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        [UsedImplicitly]
        protected T GetProperty<T>(string propertyName)
        {
            return (T)base[propertyName];
        }

        /// <summary>
        ///   Sets the configuration property to the value specified.
        /// </summary>
        /// <typeparam name="T">The property type.</typeparam>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        [UsedImplicitly]
        protected void SetProperty<T>(string propertyName, T value)
        {
            base[propertyName] = value;
        }
    }
}
