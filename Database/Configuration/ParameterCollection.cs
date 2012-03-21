using System.Configuration;
using JetBrains.Annotations;
using WebApplications.Utilities.Configuration;

namespace WebApplications.Utilities.Database.Configuration
{
    /// <summary>
    ///   A collection of <see cref="ParameterElement">parameter elements</see>.
    /// </summary>
    [UsedImplicitly]
    [ConfigurationCollection(typeof(ParameterElement), CollectionType = ConfigurationElementCollectionType.BasicMapAlternate)] 
    public class ParameterCollection : ConfigurationElementCollection<string, ParameterElement>
    {
        /// <summary>
        ///   Gets the name used to identify the element.
        /// </summary>
        /// <value>
        ///   The element name.
        /// </value>
        protected override string ElementName
        {
            get { return "parameter"; }
        }

        /// <summary>
        ///   Gets the type of the <see cref="ConfigurationElementCollection"/>.
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
        /// <param name="element">The parameter element.</param>
        /// <returns>
        ///   The <see cref="ParameterElement.Name">name</see> of the element.
        /// </returns>
        /// <exception cref="ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        protected override string GetElementKey(ParameterElement element)
        {
            return element.Name;
        }
    }
}