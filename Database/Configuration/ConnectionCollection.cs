using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using JetBrains.Annotations;
using WebApplications.Utilities.Configuration;

namespace WebApplications.Utilities.Database.Configuration
{
    /// <summary>
    ///   A collection of <see cref="ConnectionElement">connections</see>.
    /// </summary>
    [UsedImplicitly]
    [ConfigurationCollection(typeof(ConnectionElement))]
    public class ConnectionCollection : ConfigurationElementCollection<string, ConnectionElement>
    {
        /// <summary>
        ///   Gets the element key.
        /// </summary>
        /// <param name="element">The connection element.</param>
        /// <returns>
        ///   The <see cref="ConnectionElement.ConnectionString">connection string</see>.
        /// </returns>
        /// <exception cref="ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        protected override string GetElementKey(ConnectionElement element)
        {
            return element.ConnectionString;
        }
    }
}