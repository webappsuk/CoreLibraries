using System;
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
        /// Holds unknown elements as strings so that we always get a clean copy and we can detect changes easily. 
        /// </summary>
        [NotNull]
        private Dictionary<XName, string> _elements = new Dictionary<XName, string>();

        /// <summary>
        /// The modified flag.
        /// </summary>
        private bool _isModified;

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
            lock (_elements)
                if (!_elements.TryGetValue(elementName, out elementStr) || string.IsNullOrWhiteSpace(elementStr))
                    return null;

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

            string elementStr = element.ToString(SaveOptions.DisableFormatting);

            lock (_elements)
            {
                string original;
                if (_elements.TryGetValue(element.Name, out original) && string.Equals(original, elementStr)) return;

                if (IsReadOnly())
                    throw new ConfigurationErrorsException(Resources.XmlConfigurationSection_SetElement_ReadOnly);
                _isModified = true;
                _elements[element.Name] = elementStr;
            }
        }

        /// <inheritdoc />
        protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
        {
            // Read the element, getting it's XName.
            XElement element = (XElement)XNode.ReadFrom(reader);
            string elementStr = element.ToString(SaveOptions.DisableFormatting);

            lock (_elements)
                _elements[elementName] = elementStr;

            return true;
        }

        /// <inheritdoc />
        protected override bool SerializeElement([CanBeNull]XmlWriter writer, bool serializeCollectionKey)
        {
            bool dataToWrite = base.SerializeElement(writer, serializeCollectionKey);

            string[] elements;
            lock (_elements)
                elements = _elements.Values.ToArray();

            if (elements.Length < 1) return dataToWrite;

            if (writer == null) return true;

            foreach (string element in elements)
                writer.WriteRaw(element);

            return true;
        }

        /// <inheritdoc />
        protected override void Unmerge(System.Configuration.ConfigurationElement sourceElement, System.Configuration.ConfigurationElement parentElement, ConfigurationSaveMode saveMode)
        {
            base.Unmerge(sourceElement, parentElement, saveMode);

            XmlConfigurationElement source = sourceElement as XmlConfigurationElement;
            if (source == null) return;

            lock (_elements)
                _elements = new Dictionary<XName, string>(source._elements);
        }

        /// <inheritdoc />
        protected override bool IsModified()
        {
            lock (_elements)
                return _isModified || base.IsModified();
        }

        /// <inheritdoc />
        protected override void Reset(System.Configuration.ConfigurationElement parentElement)
        {
            base.Reset(parentElement);
            lock (_elements)
            {
                _elements.Clear();
                _isModified = false;
            }
        }

        /// <inheritdoc />
        protected override void ResetModified()
        {
            base.ResetModified();
            lock (_elements)
                _isModified = false;
        }
    }
}