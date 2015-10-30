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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Configuration
{
    /// <summary>
    /// Generic version of <see cref="System.Configuration.ConfigurationElementCollection" />.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the elements.</typeparam>
    [PublicAPI]
    public abstract partial class ConfigurationElementCollection<TKey, TValue> : ConfigurationElementCollection,
        ICollection<TValue>, IInternalConfigurationElement
        where TValue : ConfigurationElement, IConfigurationElement, new()
    {
        /// <summary>
        /// Gets or sets a property, attribute, or child element of this configuration element.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The specified property, attribute, or child element</returns>
        [CanBeNull]
        // ReSharper disable once VirtualMemberNeverOverriden.Global
        public virtual TValue this[[NotNull] TKey key]
        {
            get
            {
                TValue value = (TValue)BaseGet(key);
                if (value != null)
                    ((IInternalConfigurationElement)value).PropertyName = $"[{key}]";
                return value;
            }
            set
            {
                string elementName = $"[{key}]";
                lock (_children)
                {
                    TValue original = (TValue)BaseGet(key);
                    if (Equals(original, value)) return;

                    IInternalConfigurationElement ice = original;

                    if (ice != null)
                    {
                        _children.Remove(original);
                        ice.Parent = null;
                        ice.PropertyName = null;
                    }

                    ice = value;
                    if (ice != null)
                    {
                        _children.Add(ice);
                        ice.Parent = this;
                        ice.PropertyName = elementName;
                        base.BaseAdd(value);
                    }
                }
                ((IInternalConfigurationElement)this).OnChanged(this.GetFullPath(elementName));
            }
        }

        /// <summary>
        ///   Gets or sets a property, attribute, or child element of this configuration element.
        /// </summary>
        /// <returns>The specified property, attribute, or child element</returns>
        /// <exception cref="ConfigurationErrorsException">
        ///   <paramref name="index"/> is read-only or locked.
        /// </exception>
        /// <exception cref="ArgumentNullException" accessor="set"><paramref name="value"/> is <see langword="null" />.</exception>
        [NotNull]
        // ReSharper disable once VirtualMemberNeverOverriden.Global
        public virtual TValue this[int index]
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            get
            {

                TValue value = (TValue)BaseGet(index);
                if (value != null && value.ConfigurationElementName == null)
                    ((IInternalConfigurationElement)value).PropertyName = $"[{GetElementKey(value)}]";
                return value;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                TKey originalKey;
                TKey newKey = GetElementKey(value);
                string newElementName = $"[{newKey}]";
                lock (_children)
                {
                    IInternalConfigurationElement ice = (IInternalConfigurationElement)BaseGet(index);
                    if (Equals(ice, value)) return;

                    // Cannot have null values
                    Debug.Assert(ice != null);
                    originalKey = GetElementKey((TValue)ice);
                    BaseRemoveAt(index);

                    if (_children.Contains(ice))
                    {
                        _children.Remove(ice);
                        ice.Parent = null;
                        ice.PropertyName = null;
                    }

                    ice = value;
                    _children.Add(ice);
                    ice.Parent = this;
                    ice.PropertyName = newElementName;
                    base.BaseAdd(index, value);
                }
                ((IInternalConfigurationElement)this).OnChanged(this.GetFullPath($"[{originalKey}]"));
                ((IInternalConfigurationElement)this).OnChanged(this.GetFullPath(newElementName));
            }
        }

        #region ICollection<TValue> Members
        /// <summary>
        ///   Returns an enumerator that iterates through the element collection.
        /// </summary>
        /// <returns>
        ///   A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public new IEnumerator<TValue> GetEnumerator()
        {
            IEnumerator enumerator = base.GetEnumerator();
            while (enumerator.MoveNext())
            {
                TValue value = (TValue) enumerator.Current;
                if (value != null && value.ConfigurationElementName == null)
                    ((IInternalConfigurationElement)value).PropertyName = $"[{GetElementKey(value)}]";
                yield return (TValue)enumerator.Current;
            }
        }

        /// <summary>
        ///   Adds the specified value to the collection.
        /// </summary>
        /// <param name="value">This is the element to add to the collection.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null" />.</exception>
        // ReSharper disable once ExceptionNotDocumented
        public virtual void Add([NotNull] TValue value) => BaseAdd(value);

        /// <summary>
        ///   Copies the elements in the collection to the provided <see cref="System.Array"/>.
        /// </summary>
        /// <param name="array">The array to copy the elements to.</param>
        /// <param name="arrayIndex">
        ///   The zero-based index of the <see cref="System.Array"/> where the copy begins.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        ///   <paramref name="array"/> is a <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///   <paramref name="arrayIndex"/> is less than 0;
        /// </exception>
        /// <exception cref="System.ArgumentException">
        ///   <para><paramref name="array"/> is multidimensional.</para>
        ///   <para>-or-</para>
        ///   <para>The number of elements is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination array.</para>
        /// </exception>
        public void CopyTo(TValue[] array, int arrayIndex)
        {
            ((ICollection)this).CopyTo(array, arrayIndex);
        }

        /// <summary>
        ///   Removes the first occurrence of a specific object from the collection.
        /// </summary>
        /// <param name="item">The element to remove from the collection.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the element was successfully removed; otherwise returns <see langword="false"/>.
        ///   This method also returns false if the element is not found.
        /// </returns>
        /// <exception cref="System.NotSupportedException">
        ///   The collection is read-only.
        /// </exception>
        bool ICollection<TValue>.Remove([NotNull] TValue item)
        {
            TKey key = GetElementKey(item);
            bool found;
            lock (_children)
            {
                TValue value = (TValue)BaseGet(key);
                if (value == null)
                    found = false;
                else
                {
                    IInternalConfigurationElement ice = value;
                    ice.Parent = null;
                    ice.PropertyName = null;
                    _children.Remove(ice);
                    BaseRemove(item);
                    found = true;
                }
            }

            ((IInternalConfigurationElement)this).OnChanged(this.GetFullPath($"[{key}]"));
            return found;
        }

        /// <summary>
        ///   Gets a <see cref="bool"/> value indicating whether the collection is
        ///   <see cref="System.Configuration.ConfigurationElementCollection.IsReadOnly">read-only</see>.
        /// </summary>
        /// <returns>
        ///   Returns <see langword="true"/> collection is read-only; otherwise returns <see langword="false"/>.
        /// </returns>
        bool ICollection<TValue>.IsReadOnly => base.IsReadOnly();

        /// <summary>
        ///   Clears this instance, removing all elements from the collection.
        /// </summary>
        /// <exception cref="ConfigurationErrorsException">
        ///   <para>The configuration is read-only.</para>
        ///   <para>-or-</para>
        ///   <para>A collection item has been locked in a higher-level configuration.</para>  
        /// </exception>
        public virtual void Clear()
        {
            lock (_children)
            {
                foreach (IInternalConfigurationElement element in this)
                {
                    Debug.Assert(element != null);
                    _children.Remove(element);
                    element.Parent = null;
                    element.PropertyName = null;
                }
                BaseClear();
            }
            ((IInternalConfigurationElement)this).OnChanged(this.GetFullPath(".*"));
        }

        /// <summary>
        ///   Determines whether the collection contains a specific element.
        /// </summary>
        /// <param name="item">The element to locate.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if <paramref name="item"/> is found in the collection; otherwise returns <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="null" />.</exception>
        public bool Contains([NotNull] TValue item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            return BaseGet(GetElementKey(item)) != null;
        }
        #endregion

        /// <summary>
        /// Overrides the base add method to link parents.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <exception cref="ArgumentNullException"><paramref name="element"/> is <see langword="null" />.</exception>
        /// <exception cref="ConfigurationErrorsException">The element is of the wrong type.</exception>
        protected override void BaseAdd(System.Configuration.ConfigurationElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            TValue value = element as TValue;
            if (value == null)
                throw new ConfigurationErrorsException(
                    string.Format(
                        // ReSharper disable once AssignNullToNotNullAttribute
                        Resources.ConfigurationElement_Init_Invalid_Configuration_Property_Type,
                        element.GetType()));

            TKey key = GetElementKey(value);
            string elementName = $"[{key}]";
            IInternalConfigurationElement ice = value;
            lock (_children)
            {
                ice.Parent = this;
                ice.PropertyName = elementName;
                _children.Add(ice);
                base.BaseAdd(element);
            }
            ((IInternalConfigurationElement)this).OnChanged(this.GetFullPath(elementName));
        }

        /// <summary>
        ///   When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </summary>
        /// <returns>A new <see cref="System.Configuration.ConfigurationElement"/>.</returns>
        protected override System.Configuration.ConfigurationElement CreateNewElement()
            // ReSharper disable once AssignNullToNotNullAttribute
            => ConfigurationElement.Create<TValue>();

        /// <summary>
        ///   Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element">
        ///   The <see cref="System.Configuration.ConfigurationElement"/> to return the key for.
        /// </param>
        /// <returns>
        ///   An <see cref="object"/> that acts as the key for the specified <see cref="System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override sealed object GetElementKey(System.Configuration.ConfigurationElement element)
            => GetElementKey((TValue)element);

        /// <summary>
        ///   Gets the element key.
        /// </summary>
        /// <param name="element">The element whose key we want to retrieve.</param>
        /// <returns>
        ///   An <see cref="object"/> that acts as the key for the specified element.
        /// </returns>
        [NotNull]
        protected abstract TKey GetElementKey([NotNull] TValue element);

        /// <summary>
        ///   Removes an element by the specified key.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <exception cref="System.Exception">
        ///   The element does not exist in the collection, the element has already been removed,
        ///    or the element cannot be removed because the collection type is not
        ///   <see cref="System.Configuration.ConfigurationElementCollectionType">AddRemoveClearMap</see>
        /// </exception>
        // ReSharper disable once VirtualMemberNeverOverriden.Global
        public virtual void Remove([NotNull] TKey key)
        {
            lock (_children)
            {
                IInternalConfigurationElement ice = (IInternalConfigurationElement)BaseGet(key);
                if (ice == null)
                    return;

                ice.Parent = null;
                ice.PropertyName = null;
                _children.Remove(ice);
                BaseRemove(key);
            }

            ((IInternalConfigurationElement)this).OnChanged(this.GetFullPath($"[{key}]"));
        }

        /// <summary>
        ///   Removes the specified element by value.
        /// </summary>
        /// <param name="value">The element to remove.</param>
        /// <exception cref="System.Exception">
        ///   The element does not exist in the collection, the element has already been removed,
        ///    or the element cannot be removed because the collection type is not
        ///   <see cref="System.Configuration.ConfigurationElementCollectionType">AddRemoveClearMap</see>
        /// </exception>
        // ReSharper disable once VirtualMemberNeverOverriden.Global
        public virtual void Remove([NotNull] TValue value)
        {
            Remove(GetElementKey(value));
        }

        /// <summary>
        ///   Removes the element at the specified index.
        /// </summary>
        /// <param name="index">The index of the element to remove.</param>
        /// <exception cref="ConfigurationErrorsException">
        ///   <para>The configuration is read-only.</para>
        ///   <para>-or-</para>
        ///   <para>The index is less than 0 or greater than the number of elements in the collection.</para>
        ///   <para>-or-</para>
        ///   <para>The element has already been removed.</para>
        ///   <para>-or-</para>
        ///   <para>The element has been locked at a higher level.</para>
        /// </exception>
        // ReSharper disable once VirtualMemberNeverOverriden.Global
        public virtual void RemoveAt(int index)
        {
            TKey key;
            lock (_children)
            {
                IInternalConfigurationElement ice = (IInternalConfigurationElement)BaseGet(index);
                if (ice == null)
                    return;

                key = GetElementKey((TValue)ice);
                _children.Remove(ice);
                ice.Parent = null;
                ice.PropertyName = null;

                BaseRemove(ice);
            }
            ((IInternalConfigurationElement)this).OnChanged(this.GetFullPath($"[{key}]"));
        }

        /// <summary>
        ///   Gets the key for the element at the specified index.
        /// </summary>
        /// <param name="index">The index of the element.</param>
        /// <returns>
        ///   The key for the element at the specified <paramref name="index"/>.
        /// </returns>
        /// <exception cref="ConfigurationErrorsException">
        ///   <para><paramref name="index"/> is less than 0.</para>
        ///   <para>-or-</para>
        ///   <para>There is no element at the specified <paramref name="index"/>.</para>
        /// </exception>
        [CanBeNull]
        // ReSharper disable once VirtualMemberNeverOverriden.Global
        public virtual TKey GetKey(int index)
        {
            return (TKey)BaseGetKey(index);
        }

        /// <inheritdoc />
        IInternalConfigurationSection IInternalConfigurationElement.Section => _parent?.Section;

        /// <inheritdoc />
        public bool IsDisposed => Section?.IsDisposed ?? false;
    }
}