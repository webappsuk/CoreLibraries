using System.Configuration;
using JetBrains.Annotations;
using WebApplications.Utilities.Configuration;

namespace WebApplications.Utilities.Database.Configuration
{
    /// <summary>
    ///   A collection of <see cref="LoadBalancedConnectionElement">load balanced</see> connections.
    /// </summary>
    [UsedImplicitly]
    [ConfigurationCollection(typeof(ParameterElement), CollectionType = ConfigurationElementCollectionType.BasicMapAlternate)]
    public class LoadBalancedConnectionCollection : ConfigurationElementCollection<string, LoadBalancedConnectionElement>
    {
        /// <summary>
        ///   Gets the name used to identify the element.
        /// </summary>
        /// <value>
        ///   The element name.
        /// </value>
        protected override string ElementName
        {
            get { return "connection"; }
        }

        /// <summary>
        ///   Gets the type of the <see cref="LoadBalancedConnectionCollection"/>.
        /// </summary>
        /// <value>
        ///   The <see cref="ConfigurationElementCollectionType"/> of this collection.
        /// </value>
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMapAlternate; }
        }

        /// <summary>
        ///   Gets the element key.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The load balanced connection <see cref="LoadBalancedConnectionElement.Id">ID</see>.</returns>
        /// <exception cref="ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        protected override string GetElementKey(LoadBalancedConnectionElement element)
        {
            return element.Id;
        }
    }
}