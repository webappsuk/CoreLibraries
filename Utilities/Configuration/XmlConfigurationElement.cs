using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Configuration
{
    /// <summary>
    ///  A configuration element where the child XML elements are accessible and modifiable.
    /// </summary>
    /// <remarks>
    /// This has the added functionality of not throwing errors for unknown child elements, which are preserved between reads/writes.
    /// </remarks>
    [PublicAPI]
    public class XmlConfigurationElement : ConfigurationElement
    {
        /// <summary>
        /// Whether the elements are modified.
        /// </summary>
        private bool _isModified;

        /// <summary>
        /// Holds unknown elements as strings so that we always get a clean copy and we can detect changes easily.
        /// </summary>
        [NotNull]
        private ConcurrentDictionary<XName, string> _unknownElements = new ConcurrentDictionary<XName, string>();

        /// <summary>
        /// Gets this configuration element's child element by name.
        /// </summary>
        /// <param name="elementName">Name of the child element to retrieve.</param>
        /// <returns>The child element; otherwise <see langword="null"/>.</returns>
        /// <remarks><para>This is a clone of the child element, modifying it will have no effect on the configuration.</para>
        /// <para>To update the configuration, use <see cref="SetElement"/></para></remarks>
        /// <exception cref="ArgumentNullException"><paramref name="elementName"/> is <see langword="null" />.</exception>
        [CanBeNull]
        protected XElement GetElement([NotNull] XName elementName)
        {
            if (elementName == null) throw new ArgumentNullException(nameof(elementName));
            string elementStr;
            if (!_unknownElements.TryGetValue(elementName, out elementStr) || string.IsNullOrWhiteSpace(elementStr)) return null;
            return XElement.Parse(elementStr, LoadOptions.PreserveWhitespace);
        }

        /// <summary>
        /// Sets the configuration element's child element.
        /// </summary>
        /// <param name="elementName">Name of the child element.</param>
        /// <param name="element">The child element.</param>
        /// <exception cref="ArgumentNullException"><paramref name="elementName"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="element"/> is <see langword="null" />.</exception>
        /// <exception cref="ConfigurationErrorsException">The <paramref name="element">element's</paramref> <see cref="XElement.Name">name</see> doesn't match the specified <paramref name="elementName">element name</paramref>.</exception>
        /// <exception cref="ConfigurationErrorsException">The configuration is read only.</exception>
        protected void SetElement([NotNull] XName elementName, [NotNull] XElement element)
        {
            if (elementName == null) throw new ArgumentNullException(nameof(elementName));
            if (element == null) throw new ArgumentNullException(nameof(element));
            if (element.Name != elementName)
                throw new ConfigurationErrorsException(string.Format(Resources.XmlConfigurationSection_SetElement_Element_Name_Mismatch, element.Name, elementName));

            if (IsReadOnly())
                throw new ConfigurationErrorsException(Resources.XmlConfigurationSection_SetElement_ReadOnly);

            string elementStr = element.ToString(SaveOptions.DisableFormatting);
            _unknownElements.AddOrUpdate(
                element.Name,
                n =>
                {
                    _isModified = true;
                    return elementStr;
                },
                (n, e) =>
                {
                    if (!_isModified &&
                        !string.Equals(e, elementStr))
                        _isModified = true;
                    return elementStr;
                });
        }

        /// <inheritdoc />
        protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
        {
            // Read the element, getting it's XName.
            XElement element = (XElement)XNode.ReadFrom(reader);
            string elementStr = element.ToString(SaveOptions.DisableFormatting);
            // Add the element to the dictionary, serializing back to a string.
            _unknownElements.AddOrUpdate(element.Name, elementStr, (n, e) => elementStr);
            return true;
        }

        /// <inheritdoc />
        protected override bool SerializeElement([CanBeNull]XmlWriter writer, bool serializeCollectionKey)
        {
            bool dataToWrite = base.SerializeElement(writer, serializeCollectionKey);
            string[] elements = _unknownElements.Values.ToArray();
            if (elements.Length < 1) return dataToWrite;

            if (writer == null) return true;

            foreach (string elementStr in elements)
                writer.WriteRaw(elementStr);

            return true;
        }

        /// <inheritdoc />
        protected override void Unmerge(System.Configuration.ConfigurationElement sourceElement, System.Configuration.ConfigurationElement parentElement, ConfigurationSaveMode saveMode)
        {
            base.Unmerge(sourceElement, parentElement, saveMode);

            XmlConfigurationElement source = sourceElement as XmlConfigurationElement;
            if (source == null) return;

            foreach (KeyValuePair<XName, string> kvp in source._unknownElements)
                _unknownElements.AddOrUpdate(kvp.Key, kvp.Value, (n, e) => kvp.Value);
        }

        /// <inheritdoc />
        protected override bool IsModified() => _isModified || base.IsModified();

        /// <inheritdoc />
        protected override void ResetModified()
        {
            _isModified = false;
            base.ResetModified();
        }
    }
}