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
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Configuration
{
    /// <summary>
    ///   Generic version of <see cref="System.Configuration.ConfigurationElementCollection"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the elements.</typeparam>
    [PublicAPI]
    public abstract class ConfigurationElementCollection<TKey, TValue> : ConfigurationElementCollection,
        ICollection<TValue>
        where TValue : ConfigurationElement, new()
    {
        /// <summary>
        ///   Gets or sets a property, attribute, or child element of this configuration element.
        /// </summary>
        /// <param name="key">The name of the property to access.</param>
        /// <returns>The specified property, attribute, or child element</returns>
        /// <exception cref="ConfigurationErrorsException">
        ///   <paramref name="key"/> is read-only or locked.
        /// </exception>
        [CanBeNull]
        // ReSharper disable once VirtualMemberNeverOverriden.Global
        public virtual TValue this[[NotNull] TKey key]
        {
            get { return (TValue)BaseGet(key); }
            set
            {
                BaseRemove(key);
                if (value != null)
                {
                    TKey elementKey = GetElementKey(value);
                    if (!key.Equals(elementKey))
                        throw new InvalidOperationException(
                            String.Format(
                                // ReSharper disable once AssignNullToNotNullAttribute
                                Resources.ConfigurationElementCollection_SetElement_KeyMismatch,
                                key,
                                elementKey));
                    BaseAdd(value);
                }
            }
        }

        /// <summary>
        ///   Gets or sets a property, attribute, or child element of this configuration element.
        /// </summary>
        /// <returns>The specified property, attribute, or child element</returns>
        /// <exception cref="ConfigurationErrorsException">
        ///   <paramref name="index"/> is read-only or locked.
        /// </exception>
        [CanBeNull]
        // ReSharper disable once VirtualMemberNeverOverriden.Global
        public virtual TValue this[int index]
        {
            get { return (TValue)BaseGet(index); }
            set
            {
                BaseRemove(index);
                if (value != null)
                    BaseAdd(index, value);
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
                yield return (TValue)enumerator.Current;
        }

        /// <summary>
        ///   Adds the specified value to the collection.
        /// </summary>
        /// <param name="value">This is the element to add to the collection.</param>
        public virtual void Add([NotNull] TValue value)
        {
            BaseAdd(value);
        }

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
            System.Configuration.ConfigurationElement element = BaseGet(GetElementKey(item));
            if (element == null)
                return false;
            BaseRemove(item);
            return true;
            //return BaseIsRemoved(item);
        }

        /// <summary>
        ///   Gets a <see cref="bool"/> value indicating whether the collection is
        ///   <see cref="System.Configuration.ConfigurationElementCollection.IsReadOnly">read-only</see>.
        /// </summary>
        /// <returns>
        ///   Returns <see langword="true"/> collection is read-only; otherwise returns <see langword="false"/>.
        /// </returns>
        bool ICollection<TValue>.IsReadOnly
        {
            get { return base.IsReadOnly(); }
        }

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
            BaseClear();
        }

        /// <summary>
        ///   Determines whether the collection contains a specific element.
        /// </summary>
        /// <param name="item">The element to locate.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if <paramref name="item"/> is found in the collection; otherwise returns <see langword="false"/>.
        /// </returns>
        public bool Contains([NotNull] TValue item)
        {
            if (item == null) throw new ArgumentNullException("item");
            return BaseGet(GetElementKey(item)) != null;
        }
        #endregion

        /// <summary>
        ///   Gets the configuration property.
        /// </summary>
        /// <typeparam name="T">The property type.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The specified property, attribute or child element.</returns>
        protected T GetProperty<T>(string propertyName)
        {
            return (T)base[propertyName];
        }

        /// <summary>
        ///   Sets the configuration property to the specified value.
        /// </summary>
        /// <typeparam name="T">The property type.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="value">The value to set the property.</param>
        protected void SetProperty<T>(string propertyName, T value)
        {
            base[propertyName] = value;
        }

        /// <summary>
        ///   When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </summary>
        /// <returns>A new <see cref="System.Configuration.ConfigurationElement"/>.</returns>
        protected override System.Configuration.ConfigurationElement CreateNewElement()
        {
            return new TValue();
        }

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
        {
            return GetElementKey((TValue)element);
        }

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
            BaseRemove(key);
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
            BaseRemove(GetElementKey(value));
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
            BaseRemoveAt(index);
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
    }
}