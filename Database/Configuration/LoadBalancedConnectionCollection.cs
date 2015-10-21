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

using System.Configuration;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Configuration;
using ConfigurationElement = System.Configuration.ConfigurationElement;

namespace WebApplications.Utilities.Database.Configuration
{
    /// <summary>
    ///   A collection of <see cref="LoadBalancedConnectionElement">load balanced</see> connections.
    /// </summary>
    [PublicAPI]
    [ConfigurationCollection(typeof(ParameterElement),
        CollectionType = ConfigurationElementCollectionType.BasicMapAlternate)]
    public class LoadBalancedConnectionCollection :
        ConfigurationElementCollection<string, LoadBalancedConnectionElement>
    {
        /// <summary>
        /// The database element this collection belongs to.
        /// </summary>
        internal DatabaseElement Database => Parent as DatabaseElement;

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

        /// <summary>
        /// Adds a configuration element to the <see cref="T:System.Configuration.ConfigurationElementCollection" />.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement" /> to add.</param>
        protected override void BaseAdd(ConfigurationElement element)
        {
            LoadBalancedConnectionElement connectionElement = element as LoadBalancedConnectionElement;
            if (connectionElement != null) connectionElement.Database = Database;
            base.BaseAdd(element);
        }

        /// <summary>
        /// Adds a configuration element to the configuration element collection.
        /// </summary>
        /// <param name="index">The index location at which to add the specified <see cref="T:System.Configuration.ConfigurationElement" />.</param>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement" /> to add.</param>
        protected override void BaseAdd(int index, ConfigurationElement element)
        {
            LoadBalancedConnectionElement connectionElement = element as LoadBalancedConnectionElement;
            if (connectionElement != null) connectionElement.Database = Database;
            base.BaseAdd(index, element);
        }

        /// <summary>
        /// Gets or sets the <see cref="LoadBalancedConnectionElement"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="LoadBalancedConnectionElement"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public override LoadBalancedConnectionElement this[int index]
        {
            get
            {
                LoadBalancedConnectionElement element = base[index];
                if (element != null) element.Database = Database;
                return element;
            }
            set
            {
                if (value != null) value.Database = Database;
                base[index] = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="LoadBalancedConnectionElement"/> with the specified key.
        /// </summary>
        /// <value>
        /// The <see cref="LoadBalancedConnectionElement"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public override LoadBalancedConnectionElement this[string key]
        {
            get
            {
                LoadBalancedConnectionElement element = base[key];
                if (element != null) element.Database = Database;
                return element;
            }
            set
            {
                if (value != null) value.Database = Database;
                base[key] = value;
            }
        }
    }
}