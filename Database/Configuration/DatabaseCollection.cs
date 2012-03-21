using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using JetBrains.Annotations;
using WebApplications.Utilities.Configuration;

namespace WebApplications.Utilities.Database.Configuration
{
    /// <summary>
    ///   A collection of <see cref="DatabaseElement">database elements</see>.
    /// </summary>
    [UsedImplicitly]
    [ConfigurationCollection(typeof(DatabaseElement), CollectionType = ConfigurationElementCollectionType.BasicMapAlternate)] 
    public class DatabaseCollection : ConfigurationElementCollection<string, DatabaseElement>
    {
        private const string EName = "database";

        /// <summary>
        ///   Gets the name used to identify the element.
        /// </summary>
        /// <value>
        ///   The element name.
        /// </value>
        protected override string ElementName
        {
            get { return EName; }
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
        ///   Indicates whether the specified <see cref="System.Configuration.ConfigurationElement"/>
        ///   exists in the <see cref="ConfigurationElementCollection"/>.
        /// </summary>
        /// <param name="elementName">The name of the element to verify.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the element exists in the collection; otherwise returns <see langword="false"/>.
        ///   By default this value is <see langword="false"/>.
        /// </returns>
        protected override bool IsElementName(string elementName)
        {
            return EName.Equals(elementName);
        }

        /// <summary>
        ///   Gets the element key.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The element <see cref="DatabaseElement.Id">ID</see>.</returns>
        /// <exception cref="ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        protected override string GetElementKey(DatabaseElement element)
        {
            return element.Id;
        }
    }
}