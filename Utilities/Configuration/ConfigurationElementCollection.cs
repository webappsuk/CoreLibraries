#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities
// File: ConfigurationElementCollection.cs
// 
// This software, its object code and source code and all modifications made to
// the same (the “Software”) are, and shall at all times remain, the proprietary
// information and intellectual property rights of Web Applications (UK) Limited. 
// You are only entitled to use the Software as expressly permitted by Web
// Applications (UK) Limited within the Software Customisation and
// Licence Agreement (the “Agreement”).  Any copying, modification, decompiling,
// distribution, licensing, sale, transfer or other use of the Software other than
// as expressly permitted in the Agreement is expressly forbidden.  Web
// Applications (UK) Limited reserves its rights to take action against you and
// your employer in accordance with its contractual and common law rights
// (including injunctive relief) should you breach the terms of the Agreement or
// otherwise infringe its copyright or other intellectual property rights in the
// Software.
// 
// © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Configuration
{
    /// <summary>
    ///   Generic version of <see cref="System.Configuration.ConfigurationElementCollection"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the elements.</typeparam>
    public abstract class ConfigurationElementCollection<TKey, TValue> : ConfigurationElementCollection,
                                                                         ICollection<TValue>
        where TValue : ConfigurationElement, new()
    {
        /// <summary>
        ///   Gets the configuration property.
        /// </summary>
        /// <typeparam name="T">The property type.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The specified property, attribute or child element.</returns>
        [UsedImplicitly]
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
        [UsedImplicitly]
        protected void SetProperty<T>(string propertyName, T value)
        {
            base[propertyName] = value;
        }

        /// <summary>
        ///   Gets or sets a property, attribute, or child element of this configuration element.
        /// </summary>
        /// <param name="key">The name of the property to access.</param>
        /// <returns>The specified property, attribute, or child element</returns>
        /// <exception cref="ConfigurationErrorsException">
        ///   <paramref name="key"/> is read-only or locked.
        /// </exception>
        [CanBeNull]
        [UsedImplicitly]
        public virtual TValue this[[NotNull] TKey key]
        {
            get { return (TValue) BaseGet(key); }
            set
            {
                BaseRemove(key);
                if (value != null)
                {
                    TKey elementKey = GetElementKey(value);
                    if (!key.Equals(elementKey))
                    {
                        throw new InvalidOperationException(
                            String.Format(
                                Resources.ConfigurationElementCollection_SetElement_KeyMismatch,
                                key, elementKey));
                    }
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
        [UsedImplicitly]
        public virtual TValue this[int index]
        {
            get { return (TValue) BaseGet(index); }
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
                yield return (TValue) enumerator.Current;
        }

        /// <summary>
        ///   Adds the specified value to the collection.
        /// </summary>
        /// <param name="value">This is the element to add to the collection.</param>
        [UsedImplicitly]
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
            ((ICollection) this).CopyTo(array, arrayIndex);
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
        [UsedImplicitly]
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
        public bool Contains(TValue item)
        {
            return BaseGet(GetElementKey(item)) != null;
        }
        #endregion

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
            return GetElementKey((TValue) element);
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
        [UsedImplicitly]
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
        [UsedImplicitly]
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
        [UsedImplicitly]
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
        [UsedImplicitly]
        public virtual TKey GetKey(int index)
        {
            return (TKey) BaseGetKey(index);
        }
    }
}