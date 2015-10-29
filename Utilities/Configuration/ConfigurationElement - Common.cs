#region © Copyright Web Applications (UK) Ltd, 2015.  All rights reserved.
// Copyright (c) 2015, Web Applications UK Ltd
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Web Applications UK Ltd nor the
//       names of its contributors may be used to endorse or promote products
//       derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL WEB APPLICATIONS UK LTD BE LIABLE FOR ANY
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

namespace WebApplications.Utilities.Configuration
{
    /*
     * NOTE: Using statements must appear inside namespace to prevent issues when duplicating file
     */
    using Annotations;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    ///   Represents a configuration element in a configuration file.
    /// </summary>
    public abstract partial class ConfigurationElement
    {
        /// <summary>
        /// Holds unknown elements as strings so that we always get a clean copy and we can detect changes easily. 
        /// </summary>
        [NotNull]
        private Dictionary<XName, string> _elements = new Dictionary<XName, string>();

        /// <summary>
        /// Indicates whether we have any changes.
        /// </summary>
        private bool _isModified;

        /// <summary>
        /// The children.
        /// </summary>
        [NotNull]
        private readonly HashSet<IInternalConfigurationElement> _children = new HashSet<IInternalConfigurationElement>();

        /// <summary>
        /// The properties, strongly typed!
        /// </summary>
        [CanBeNull]
        private IReadOnlyCollection<ConfigurationProperty> _properties;

        private IInternalConfigurationElement _parent;

        /// <summary>
        /// Called when initializing.
        /// </summary>
        partial void OnInit();

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <exception cref="ConfigurationErrorsException">Invalid property type.</exception>
        protected override void Init()
        {
            base.Init();
            // ReSharper disable once PossibleNullReferenceException
            // ReSharper disable once AssignNullToNotNullAttribute
            foreach (ConfigurationProperty property in Properties.Where(p => p.Type.DescendsFrom(typeof(System.Configuration.ConfigurationElement))))
            {
                Debug.Assert(property.Type != null);
                if (!property.Type.ImplementsInterface(typeof(IInternalConfigurationElement)))
                    throw new ConfigurationErrorsException(
                        string.Format(
                            // ReSharper disable AssignNullToNotNullAttribute
                            Resources.ConfigurationElement_Init_Invalid_Configuration_Property_Type,
                            // ReSharper restore AssignNullToNotNullAttribute
                            property.Type));

                // Get the value or create a new element, if this is an element type that is not yet created.
                // ReSharper disable once ArrangeStaticMemberQualifier
                IInternalConfigurationElement value =
                    (IInternalConfigurationElement)(this[property.Name] ?? ConfigurationElement.Create(property.Type));

                if (value == null) continue;

                value.Parent = this;
                value.PropertyName = property.Name;
                _children.Add(value);
            }
            OnInit();
        }

        /// <inheritdoc />
        IInternalConfigurationElement IInternalConfigurationElement.Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        /// <inheritdoc />
        public IConfigurationElement Parent => ((IInternalConfigurationElement)this).Parent;

        /// <inheritdoc />
        public IConfigurationSection Section => ((IInternalConfigurationElement)this).Section;

        /// <inheritdoc />
        string IInternalConfigurationElement.PropertyName { get; set; }

        /// <inheritdoc />
        // ReSharper disable once ArrangeStaticMemberQualifier
        public string FullPath => IsDisposed ? "*DISPOSED*" : ConfigurationElement.GetFullPath(Parent, PropertyName);

        /// <inheritdoc />
        public string PropertyName => ((IInternalConfigurationElement)this).PropertyName;

        /// <inheritdoc />
        public IReadOnlyCollection<IConfigurationElement> Children
        {
            get
            {
                if (IsDisposed) throw new ObjectDisposedException(ToString());
                // ReSharper disable ExceptionNotDocumentedOptional, ExceptionNotDocumented
                lock (_children)
                    return _children.Cast<IConfigurationElement>().ToArray();
                // ReSharper restore ExceptionNotDocumentedOptional, ExceptionNotDocumented
            }
        }

        /// <inheritdoc />
        void IInternalConfigurationElement.Initialize()
        {
            Init();
        }

        /// <inheritdoc />
        void IInternalConfigurationElement.OnChanged(IInternalConfigurationElement sender, string propertyName)
        {
            // Propagate to parent.
            ((IInternalConfigurationElement)this).Parent?.OnChanged(sender, propertyName);
            _isModified = true;
            DoChanged(sender, propertyName);
        }

        /// <summary>
        /// Called when the OnChange event is called.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="propertyName">Name of the property.</param>
        partial void DoChanged(IInternalConfigurationElement sender, string propertyName);

        /// <summary>
        ///   Gets the configuration property.
        /// </summary>
        /// <typeparam name="T">The property type.</typeparam>
        /// <param name="propertyName">The name of the property to get.</param>
        /// <returns>The specified property, attribute or child element.</returns>
        protected T GetProperty<T>([NotNull] string propertyName) => (T)this[propertyName];

        /// <summary>
        ///   Sets the configuration property to the value specified.
        /// </summary>
        /// <typeparam name="T">The property type.</typeparam>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <param name="value">The value to set.</param>
        protected void SetProperty<T>([NotNull] string propertyName, T value)
        {
            this[propertyName] = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="ObjectDisposedException" accessor="get">The current section <see cref="IsDisposed">is disposed</see>.</exception>
        /// <exception cref="ObjectDisposedException" accessor="set">The current section <see cref="IsDisposed">is disposed</see>.</exception>
        /// <exception cref="ArgumentNullException" accessor="get"><paramref name="propertyName"/> is <see langword="null" /> or empty.</exception>
        /// <exception cref="ArgumentNullException" accessor="set"><paramref name="propertyName"/> is <see langword="null" /> or empty.</exception>
        private new object this[string propertyName]
        {
            get
            {
                if (IsDisposed) throw new ObjectDisposedException(ToString());

                if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException(nameof(propertyName));
                object value = base[propertyName];
                IInternalConfigurationElement ice = value as IInternalConfigurationElement;
                if (ice != null)
                    ice.PropertyName = propertyName;
                return value;
            }
            set
            {
                if (IsDisposed) throw new ObjectDisposedException(ToString());
                if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException(nameof(propertyName));

                lock (_children)
                {
                    object original = base[propertyName];
                    if (Equals(original, value)) return;

                    IInternalConfigurationElement ice = original as IInternalConfigurationElement;
                    if (ice != null && _children.Contains(ice))
                    {
                        _children.Remove(ice);
                        ice.Parent = null;
                        ice.PropertyName = null;
                    }

                    ice = value as IInternalConfigurationElement;
                    if (ice != null)
                    {
                        _children.Add(ice);
                        ice.Parent = this;
                        ice.PropertyName = propertyName;
                    }

                    base[propertyName] = value;
                }

                // Notify change handler
                ((IInternalConfigurationElement)this).OnChanged(this, propertyName);
            }
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>The properties.</value>
        [NotNull]
        // ReSharper disable ExceptionNotDocumentedOptional, ExceptionNotDocumented
        protected new IReadOnlyCollection<ConfigurationProperty> Properties
        {
            get
            {
                if (IsDisposed) throw new ObjectDisposedException(ToString());
                return _properties ??
                       (_properties =
                           base.Properties == null || base.Properties.Count < 1
                               ? Array<ConfigurationProperty>.Empty
                               : base.Properties.Cast<ConfigurationProperty>().ToArray());
            }
        }

        // ReSharper restore ExceptionNotDocumentedOptional, ExceptionNotDocumented

        /// <summary>
        /// Gets the keys.
        /// </summary>
        /// <value>The keys.</value>
        /// <exception cref="ObjectDisposedException" accessor="get">The current section <see cref="IsDisposed">is disposed</see>.</exception>
        // ReSharper disable PossibleNullReferenceException
        public IEnumerable<string> Keys
        {
            get
            {
                if (IsDisposed) throw new ObjectDisposedException(ToString());
                return Properties.Select(p => p.Name);
            }
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>The values.</value>
        /// <exception cref="ObjectDisposedException" accessor="get">The current section <see cref="IsDisposed">is disposed</see>.</exception>
        public IEnumerable<object> Values
        {
            get
            {
                if (IsDisposed) throw new ObjectDisposedException(ToString());
                return Properties.Select(p => this[p.Name]);
            }
        }

        /// <summary>
        /// Gets the property values.
        /// </summary>
        /// <value>The property values.</value>
        /// <exception cref="ObjectDisposedException" accessor="get">The current section <see cref="IsDisposed">is disposed</see>.</exception>
        public IReadOnlyDictionary<string, object> PropertyValues
        {
            get
            {
                if (IsDisposed) throw new ObjectDisposedException(ToString());
                return Properties.ToDictionary(p => p.Name, p => this[p.Name]);
            }
        }

        // ReSharper restore PossibleNullReferenceException

        /// <summary>
        /// Gets this configuration element's child element by name.
        /// </summary>
        /// <param name="elementName">Name of the child element to retrieve.</param>
        /// <returns>The child element; otherwise <see langword="null"/>.</returns>
        /// <remarks><para>This is a clone of the child element, modifying it will have no effect on the configuration.</para>
        /// <para>To update the configuration, use <see cref="SetElement"/></para></remarks>
        /// <exception cref="ArgumentNullException"><paramref name="elementName"/> is <see langword="null" />.</exception>
        /// <exception cref="ObjectDisposedException">The current section <see cref="IsDisposed">is disposed</see>.</exception>
        [CanBeNull]
        protected XElement GetElement([NotNull] XName elementName)
        {
            if (IsDisposed) throw new ObjectDisposedException(ToString());
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
        /// <exception cref="ObjectDisposedException">The current section <see cref="IsDisposed">is disposed</see>.</exception>
        protected void SetElement([NotNull] XName elementName, [NotNull] XElement element)
        {
            if (IsDisposed) throw new ObjectDisposedException(ToString());
            if (elementName == null) throw new ArgumentNullException(nameof(elementName));
            if (element == null) throw new ArgumentNullException(nameof(element));
            // ReSharper disable AssignNullToNotNullAttribute
            if (element.Name != elementName)
                throw new ConfigurationErrorsException(string.Format(Resources.XmlConfigurationSection_SetElement_Element_Name_Mismatch, element.Name, elementName));
            // ReSharper restore AssignNullToNotNullAttribute

            if (IsReadOnly())
                throw new ConfigurationErrorsException(Resources.XmlConfigurationSection_SetElement_ReadOnly);

            string elementStr = element.ToString(SaveOptions.DisableFormatting);

            lock (_elements)
            {
                string original;
                if (_elements.TryGetValue(element.Name, out original) && string.Equals(original, elementStr)) return;
                _isModified = true;
                _elements[element.Name] = elementStr;
            }
            ((IInternalConfigurationElement)this).OnChanged(this, $"<{elementName}>");
        }

        /// <inheritdoc />
        /// <exception cref="InvalidOperationException">The <see cref="T:System.Xml.XmlReader" /> is not positioned on a recognized node type.</exception>
        /// <exception cref="XmlException">The underlying <see cref="T:System.Xml.XmlReader" /> throws an exception.</exception>
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
        /// <exception cref="ConfigurationErrorsException">The current attribute is locked at a higher configuration level.</exception>
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

            IInternalConfigurationElement source = sourceElement as IInternalConfigurationElement;
            if (source == null) return;

            lock (_elements)
                _elements = source.ElementsClone;
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
                if (_elements.Count < 1) return;
                _elements.Clear();
                _isModified = false;
            }
            ((IInternalConfigurationElement)this).OnChanged(this, "<>");
        }

        /// <inheritdoc />
        protected override void ResetModified()
        {
            base.ResetModified();
            lock (_elements)
                _isModified = false;
        }

        /// <inheritdoc />
        public IReadOnlyCollection<XName> ElementNames
        {
            get
            {
                if (IsDisposed) throw new ObjectDisposedException(ToString());
                lock (_elements)
                    return _elements.Keys.ToArray();
            }
        }

        /// <inheritdoc />
        public IReadOnlyCollection<XElement> Elements
        {
            get
            {
                if (IsDisposed) throw new ObjectDisposedException(ToString());
                lock (_elements)
                    return _elements.Values
                            .Select(e => XElement.Parse(e, LoadOptions.PreserveWhitespace))
                            .ToArray();

            }
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<XName, XElement> ElementDictionary
        {
            get
            {
                if (IsDisposed) throw new ObjectDisposedException(ToString());
                lock (_elements)
                    return _elements
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => XElement.Parse(kvp.Value, LoadOptions.PreserveWhitespace)
                        );

            }
        }

        /// <inheritdoc />
        public Dictionary<XName, string> ElementsClone
        {
            get
            {
                if (IsDisposed) throw new ObjectDisposedException(ToString());
                lock (_elements)
                    return new Dictionary<XName, string>(_elements);
            }
        }

        /// <inheritdoc />
        public override string ToString() => FullPath;
    }
}