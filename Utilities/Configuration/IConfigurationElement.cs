using System;
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
        IConfigurationSection Section { get; }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        [CanBeNull]
        IConfigurationElement Parent { get; }

        /// <summary>
        /// Gets the name of the current element.
        /// </summary>
        /// <value>The name of the element.</value>
        [CanBeNull]
        string PropertyName { get; }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The descendants.</value>
        /// <exception cref="ObjectDisposedException" accessor="get">The current section <see cref="IsDisposed">is disposed</see>.</exception>
        [NotNull]
        IReadOnlyCollection<IConfigurationElement> Children { get; }

        /// <summary>
        /// Gets the keys.
        /// </summary>
        /// <value>The keys.</value>
        /// <exception cref="ObjectDisposedException" accessor="get">The current section <see cref="IsDisposed">is disposed</see>.</exception>
        [NotNull]
        IEnumerable<string> Keys { get; }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>The values.</value>
        /// <exception cref="ObjectDisposedException" accessor="get">The current section <see cref="IsDisposed">is disposed</see>.</exception>
        [NotNull]
        IEnumerable<object> Values { get; }

        /// <summary>
        /// Gets the property values.
        /// </summary>
        /// <value>The property values.</value>
        /// <exception cref="ObjectDisposedException" accessor="get">The current section <see cref="IsDisposed">is disposed</see>.</exception>
        [NotNull]
        IReadOnlyDictionary<string, object> PropertyValues { get; }

        /// <summary>
        /// Gets the unknown element names.
        /// </summary>
        /// <value>The element names.</value>
        /// <exception cref="ObjectDisposedException" accessor="get">The current section <see cref="IsDisposed">is disposed</see>.</exception>
        [NotNull]
        IReadOnlyCollection<XName> ElementNames { get; }

        /// <summary>
        /// Gets the unknown elements.
        /// </summary>
        /// <value>The elements.</value>
        /// <exception cref="ObjectDisposedException" accessor="get">The current section <see cref="IsDisposed">is disposed</see>.</exception>
        [NotNull]
        IReadOnlyCollection<XElement> Elements { get; }

        /// <summary>
        /// Gets the unknown elements as a dictionary, accessible by the <see cref="XName"/>.
        /// </summary>
        /// <value>The elements.</value>
        /// <exception cref="ObjectDisposedException" accessor="get">The current section <see cref="IsDisposed">is disposed</see>.</exception>
        [NotNull]
        IReadOnlyDictionary<XName, XElement> ElementDictionary { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value><see langword="true" /> if this instance is disposed; otherwise, <see langword="false" />.</value>
        bool IsDisposed { get; }
    }
}