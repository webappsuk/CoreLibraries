using System.Collections.Generic;
using System.Xml.Linq;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Configuration
{
    /// <summary>
    /// Internal version of interface for exposing configuration hierarchy.
    /// </summary>
    internal interface IInternalConfigurationElement : IConfigurationElement
    {
        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        [CanBeNull]
        new IInternalConfigurationElement Parent { get; set; }

        /// <summary>
        /// Called when this element, or a child element is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="propertyName">Name of the property.</param>
        void OnChanged([NotNull] IInternalConfigurationElement sender, [CanBeNull] string propertyName);

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <remarks>This is a short-cut to calling the protected Init() virtual method.</remarks>
        void Initialize();

        /// <summary>
        /// Gets the elements as a dictionary, accessible by the <see cref="XName"/>.
        /// </summary>
        /// <value>The elements.</value>
        [NotNull]
        Dictionary<XName, string> ElementsClone { get; }
    }
}