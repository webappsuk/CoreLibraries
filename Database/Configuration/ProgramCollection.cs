using System.Configuration;
using JetBrains.Annotations;
using WebApplications.Utilities.Configuration;

namespace WebApplications.Utilities.Database.Configuration
{
    /// <summary>
    ///   A collection of <see cref="ProgramElement">program elements</see>.
    /// </summary>
    [UsedImplicitly]
    [ConfigurationCollection(typeof(ProgramElement), CollectionType = ConfigurationElementCollectionType.BasicMapAlternate)] 
    public class ProgramCollection : ConfigurationElementCollection<string, ProgramElement>
    {
        /// <summary>
        ///   Gets the name used to identify the element.
        /// </summary>
        /// <value>
        ///   The element name.
        /// </value>
        protected override string ElementName
        {
            get { return "program"; }
        }

        /// <summary>
        ///   Gets the type of the <see cref="ProgramCollection"/>.
        /// </summary>
        /// <returns>
        ///   The <see cref="ConfigurationElementCollectionType"/> of this collection.
        /// </returns>
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMapAlternate; }
        }

        /// <summary>
        ///   Gets the element key.
        /// </summary>
        /// <param name="element">The program element.</param>
        /// <returns>
        /// The <see cref="ProgramElement.Name">name</see> of the element.
        /// </returns>
        /// <exception cref="ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        protected override string GetElementKey(ProgramElement element)
        {
            return element.Name;
        }
    }
}