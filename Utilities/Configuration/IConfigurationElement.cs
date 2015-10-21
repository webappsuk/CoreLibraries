using System.Collections.Generic;
using System.Xml.Linq;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Configuration
{
    /// <summary>
    /// Interface for exposing configuration hierarchy.
    /// </summary>
    /// <typeparam name="T">The <see cref="Section"/> type.</typeparam>
    [PublicAPI]
    // ReSharper disable once TypeParameterCanBeVariant
    public interface IConfigurationElement
    {
        /// <summary>
        /// Gets the curretn section.
        /// </summary>
        /// <value>The section.</value>
        IConfigurationElement Section { get; }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        [CanBeNull]
        IConfigurationElement Parent { get; }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The descendants.</value>
        [NotNull]
        IReadOnlyCollection<IConfigurationElement> Children { get; }

        /// <summary>
        /// Gets the keys.
        /// </summary>
        /// <value>The keys.</value>
        [NotNull]
        IEnumerable<string> Keys { get; }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>The values.</value>
        [NotNull]
        IEnumerable<object> Values { get; }

        /// <summary>
        /// Gets the property values.
        /// </summary>
        /// <value>The property values.</value>
        [NotNull]
        IReadOnlyDictionary<string, object> PropertyValues { get; }

        /// <summary>
        /// Gets the unknown element names.
        /// </summary>
        /// <value>The element names.</value>
        [NotNull]
        IReadOnlyCollection<XName> ElementNames { get; }

        /// <summary>
        /// Gets the unknown elements.
        /// </summary>
        /// <value>The elements.</value>
        [NotNull]
        IReadOnlyCollection<XElement> Elements { get; }

        /// <summary>
        /// Gets the unknown elements as a dictionary, accessible by the <see cref="XName"/>.
        /// </summary>
        /// <value>The elements.</value>
        [NotNull]
        IReadOnlyDictionary<XName, XElement> ElementDictionary { get; }
    }
}