#region © Copyright Web Applications (UK) Ltd, 2014.  All rights reserved.
// Copyright (c) 2014, Web Applications UK Ltd
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
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using WebApplications.Utilities.Reflect;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Makes any object instance implement a dictionary for accessing it's properties and fields.
    /// </summary>
    [PublicAPI]
    public abstract class Accessor : IDictionary<string, object>, IReadOnlyDictionary<string, object>
    {
        /// <summary>
        /// Creates a new <see cref="Accessor{T}"/> for the <paramref name="instance"/> given.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="includeFields">if set to <see langword="true" /> includes fields.</param>
        /// <param name="includeProperties">if set to <see langword="true" /> includes properties.</param>
        /// <param name="includeInstance">if set to <see langword="true" /> includes instance members.</param>
        /// <param name="includeStatic">if set to <see langword="true" /> includes static members.</param>
        /// <param name="includePublic">if set to <see langword="true" /> includes public members.</param>
        /// <param name="includeNonPublic">if set to <see langword="true" /> includes non-public members.</param>
        /// <param name="supportsNew">if set to <see langword="true" /> unknown keys are supported and stored.</param>
        /// <param name="isCaseSensitive">if set to 
        /// <see langword="true" /> then keys are case sensitive; otherwise matching keys (due to case insensitivity) will result
        /// in missing accessors so this setting should be used with caution.</param>
        /// <returns>A new <see cref="Accessor{T}"/> of the </returns>
        [NotNull]
        [PublicAPI]
        public static Accessor Create(
            [CanBeNull] object instance,
            bool includeFields = true,
            bool includeProperties = true,
            bool includeInstance = true,
            bool includeStatic = true,
            bool includePublic = true,
            bool includeNonPublic = false,
            bool supportsNew = false,
            bool isCaseSensitive = true)
        {
            if (instance == null)
                return new Accessor<object>(
                    null,
                    includeFields,
                    includeProperties,
                    includeInstance,
                    includeStatic,
                    includePublic,
                    includeNonPublic,
                    supportsNew,
                    isCaseSensitive);

            Type genericAccessorType = typeof(Accessor<>).MakeGenericType(new[] { instance.GetType() });

            // ReSharper disable once AssignNullToNotNullAttribute
            return (Accessor)Activator.CreateInstance(
                genericAccessorType,
                new[]
                {
                    instance,
                    includeFields,
                    includeProperties,
                    includeInstance,
                    includeStatic,
                    includePublic,
                    includeNonPublic,
                    supportsNew,
                    isCaseSensitive
                });
        }

        /// <summary>
        /// Whether keys are case sensitive.
        /// </summary>
        [PublicAPI]
        public readonly bool IsCaseSensitive;

        /// <summary>
        /// Initializes a new instance of the <see cref="Accessor"/> class.
        /// </summary>
        /// <param name="isCaseSensitive">if set to <see langword="true" /> then keys are case sensitive; 
        /// otherwise matching keys (due to case insensitivity) will resultin missing accessors so this setting should be used with caution.</param>
        protected Accessor(bool isCaseSensitive)
        {
            IsCaseSensitive = isCaseSensitive;
        }

        public abstract IEnumerator<KeyValuePair<string, object>> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public abstract void Add(KeyValuePair<string, object> item);
        public abstract void Clear();
        public abstract bool Contains(KeyValuePair<string, object> item);
        public abstract void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex);
        public abstract bool Remove(KeyValuePair<string, object> item);
        public abstract int Count { get; }
        public abstract bool IsReadOnly { get; }
        public abstract bool ContainsKey(string key);
        public abstract void Add(string key, object value);
        public abstract bool Remove(string key);
        public abstract bool TryGetValue(string key, out object value);
        public abstract object this[string key] { get; set; }
        IEnumerable<string> IReadOnlyDictionary<string, object>.Keys
        {
            get { return Keys; }
        }

        IEnumerable<object> IReadOnlyDictionary<string, object>.Values
        {
            get { return Values; }
        }

        public abstract ICollection<string> Keys { get; }
        public abstract ICollection<object> Values { get; }

        /// <summary>
        /// Applies the specified snapshot.
        /// </summary>
        /// <param name="snapshot">The snapshot.</param>
        [PublicAPI]
        public void Apply([CanBeNull] IReadOnlyDictionary<string, object> snapshot)
        {
            if (snapshot == null) return;
            foreach (KeyValuePair<string, object> kvp in snapshot)
                Add(kvp);
        }

        /// <summary>
        /// Gets a snapshot of the object.
        /// </summary>
        /// <returns>ReadOnlyDictionary&lt;System.String, System.Object&gt;.</returns>
        [NotNull]
        [PublicAPI]
        public IReadOnlyDictionary<string, object> Snapshot()
        {
            return new Dictionary<string, object>(this);
        }
    }

    /// <summary>
    /// Makes any object instance implement a dictionary for accessing it's properties and fields.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [PublicAPI]
    public class Accessor<T> : Accessor
    {
        /// <summary>
        /// Holds accessor information for properties and fields.
        /// </summary>
        private class Access
        {
            /// <summary>
            /// The name.
            /// </summary>
            [NotNull]
            public readonly string Name;

            /// <summary>
            /// The instance represents a property.
            /// </summary>
            public readonly bool IsProperty;

            /// <summary>
            /// The instance represents a static member.
            /// </summary>
            public readonly bool IsStatic;

            /// <summary>
            /// The instance represents a public member.
            /// </summary>
            public readonly bool IsPublic;

            /// <summary>
            /// The get function.
            /// </summary>
            [CanBeNull]
            public readonly Func<T, object> Get;

            /// <summary>
            /// The set action.
            /// </summary>
            [CanBeNull]
            public readonly Action<T, object> Set;

            /// <summary>
            /// Initializes a new instance of the <see cref="Access"/> class.
            /// </summary>
            /// <param name="field">The field.</param>
            public Access([NotNull] Field field)
            {
                Contract.Requires(field != null);
                Name = field.Info.Name;
                IsStatic = field.Info.IsStatic;
                IsPublic = field.Info.IsPublic;

                if (IsStatic)
                {
                    Func<object> get = field.Getter<object>();
                    Action<object> set = field.Setter<object>();

                    Get = get == null ? null : new Func<T, object>(i => get());
                    Set = set == null ? null : new Action<T, object>((i, v) => set(v));
                }
                else
                {
                    Get = field.Getter<T, object>();
                    Set = field.Setter<T, object>();
                }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Access" /> class.
            /// </summary>
            /// <param name="property">The property.</param>
            public Access([NotNull] Property property)
            {
                Contract.Requires(property != null);
                Name = property.Info.Name;
                IsProperty = true;
                if (property.GetMethod != null)
                {
                    IsStatic = property.GetMethod.IsStatic;
                    IsPublic = property.GetMethod.IsPublic &&
                               (property.SetMethod == null || property.SetMethod.IsPublic);
                }
                else if (property.SetMethod != null)
                {
                    IsStatic = property.SetMethod.IsStatic;
                    IsPublic = property.SetMethod.IsPublic;
                }
                else
                {
                    // Should never happen!
                    IsStatic = true;
                    IsPublic = false;
                }

                if (IsStatic)
                {
                    Func<object> get = property.Getter<object>();
                    Action<object> set = property.Setter<object>();

                    Get = get == null ? null : new Func<T, object>(i => get());
                    Set = set == null ? null : new Action<T, object>((i, v) => set(v));
                }
                else
                {
                    Get = property.Getter<T, object>();
                    Set = property.Setter<T, object>();
                }
            }
        }

        /// <summary>
        /// All accessors for the type.
        /// </summary>
        [NotNull]
        private static readonly Access[] _accessors;

        /// <summary>
        /// Initializes static members of the <see cref="Accessor{T}"/> class.
        /// </summary>
        static Accessor()
        {
            ExtendedType et = ExtendedType.Get(typeof(T));

            // Combine field and properties into access dictionary.
            _accessors = et.Fields.Where(f => !f.Info.IsCompilerGenerated()).Select(f => new Access(f))
                .Union(et.Properties.Where(p => !p.Info.IsCompilerGenerated()).Select(p => new Access(p)))
                .Where(a => a.Get != null)
                .ToArray();
        }

        /// <summary>
        /// The underlying dictionary.
        /// </summary>
        [NotNull]
        private readonly Dictionary<string, object> _dictionary;

        /// <summary>
        /// The underlying instance.
        /// </summary>
        [CanBeNull]
        private readonly T _instance;

        /// <summary>
        /// Whether to allow new keys.
        /// </summary>
        private readonly bool _supportsNew;

        /// <summary>
        /// Initializes a new instance of the <see cref="Accessor{T}" /> class.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="includeFields">if set to <see langword="true" /> includes fields.</param>
        /// <param name="includeProperties">if set to <see langword="true" /> includes properties.</param>
        /// <param name="includeInstance">if set to <see langword="true" /> includes instance members.</param>
        /// <param name="includeStatic">if set to <see langword="true" /> includes static members.</param>
        /// <param name="includePublic">if set to <see langword="true" /> includes public members.</param>
        /// <param name="includeNonPublic">if set to <see langword="true" /> includes non-public members.</param>
        /// <param name="supportsNew">if set to <see langword="true" /> unknown keys are supported and stored.</param>
        /// <param name="isCaseSensitive">if set to <see langword="true" /> then keys are case sensitive; 
        /// otherwise matching keys (due to case insensitivity) will resultin missing accessors so this setting should be used with caution.</param>
        public Accessor(
            [CanBeNull] T instance,
            bool includeFields = true,
            bool includeProperties = true,
            bool includeInstance = true,
            bool includeStatic = true,
            bool includePublic = true,
            bool includeNonPublic = false,
            bool supportsNew = false,
            bool isCaseSensitive = true)
            : base(isCaseSensitive)
        {
            _instance = instance;
            _supportsNew = supportsNew;
            if (ReferenceEquals(_instance, null))
                includeInstance = false;

            // Create accessor dictionary
            _dictionary = new Dictionary<string, object>(
                _accessors.Length,
                isCaseSensitive
                    ? StringComparer.CurrentCulture
                    : StringComparer.CurrentCultureIgnoreCase);
            foreach (Access accessor in _accessors)
            {
                Contract.Assert(accessor != null);
                // Filter accessors
                // ReSharper disable PossibleNullReferenceException
                if ((!includeFields && !accessor.IsProperty) ||
                    (!includeProperties && accessor.IsProperty) ||
                    (!includeInstance && !accessor.IsStatic) ||
                    (!includeStatic && accessor.IsStatic) ||
                    (!includePublic && accessor.IsPublic) ||
                    (!includeNonPublic && !accessor.IsPublic))
                    continue;
                _dictionary[accessor.Name] = accessor;
                // ReSharper restore PossibleNullReferenceException
            }
        }

        #region Dictionary implementation
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public override IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            foreach (KeyValuePair<string, object> kvp in _dictionary)
            {
                Access access = kvp.Value as Access;
                if (access == null)
                {
                    yield return kvp;
                    continue;
                }
                if (access.Get == null)
                    continue;
                yield return new KeyValuePair<string, object>(kvp.Key, access.Get(_instance));
            }
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        public override void Add(KeyValuePair<string, object> item)
        {
            object value;
            if (!_dictionary.TryGetValue(item.Key, out value))
            {
                if (!_supportsNew) return;
                _dictionary[item.Key] = item.Value;
            }

            Access access = value as Access;
            if (access != null)
            {
                if (access.Set != null)
                    access.Set(_instance, item.Value);
                return;
            }
            _dictionary[item.Key] = item.Value;
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public override void Clear()
        {
            if (!_supportsNew)
                return;
            foreach (string key in _dictionary
                .Where(kvp => !(kvp.Value is Access))
                .Select(kvp => kvp.Key)
                .ToArray())
                _dictionary.Remove(key);
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.</returns>
        public override bool Contains(KeyValuePair<string, object> item)
        {
            object value;
            if (item.Key == null ||
                !_dictionary.TryGetValue(item.Key, out value))
                return false;

            Access access = value as Access;
            if (access == null)
                return Equals(item.Value, value);

            return access.Get != null && Equals(item.Value, access.Get(_instance));
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        public override void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            ((IDictionary<string, object>)Snapshot()).CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        public override bool Remove(KeyValuePair<string, object> item)
        {
            if (!_supportsNew) return false;
            object value;
            if (item.Key == null ||
                !_dictionary.TryGetValue(item.Key, out value))
                return false;
            if (value is Access)
                return false;
            return Equals(item.Value, value) && _dictionary.Remove(item.Key);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" /> that the value can be retrieved for.
        /// </summary>
        /// <value>The count.</value>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        public override int Count
        {
            get
            {
                return _dictionary.Count(
                    kvp =>
                    {
                        Access access = kvp.Value as Access;
                        return access == null || access.Get != null;
                    });
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        /// <value><see langword="true" /> if this instance is read only; otherwise, <see langword="false" />.</value>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, false.</returns>
        public override bool IsReadOnly
        {
            get { return !_supportsNew && _dictionary.Values.OfType<Access>().All(a => a.Set == null); }
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2" />.</param>
        /// <returns>true if the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the key; otherwise, false.</returns>
        // ReSharper disable once CodeAnnotationAnalyzer
        public override bool ContainsKey(string key)
        {
            return _dictionary.ContainsKey(key);
        }

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        // ReSharper disable once CodeAnnotationAnalyzer
        public override void Add(string key, object value)
        {
            object v;
            if (!_dictionary.TryGetValue(key, out v))
            {
                if (!_supportsNew) return;
                _dictionary[key] = value;
            }

            Access access = v as Access;
            if (access != null)
            {
                if (access.Set != null)
                    access.Set(_instance, value);
                return;
            }
            _dictionary[key] = value;
        }

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.Boolean.</returns>
        // ReSharper disable once CodeAnnotationAnalyzer
        public override bool Remove(string key)
        {
            if (!_supportsNew) return false;
            object value;
            if (!_dictionary.TryGetValue(key, out value))
                return false;
            if (value is Access)
                return false;
            return _dictionary.Remove(key);
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value" /> parameter. This parameter is passed uninitialized.</param>
        /// <returns>true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified key; otherwise, false.</returns>
        public override bool TryGetValue(string key, out object value)
        {
            if (!_dictionary.TryGetValue(key, out value))
                return false;

            Access access = value as Access;
            if (access == null)
                return true;

            if (access.Get == null)
                return false;

            value = access.Get(_instance);
            return true;
        }

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="System.IndexOutOfRangeException">
        /// </exception>
        public override object this[[NotNull] string key]
        {
            get
            {
                object value;
                if (!TryGetValue(key, out value))
                    throw new IndexOutOfRangeException();
                return value;
            }
            set
            {
                object v;
                if (!_dictionary.TryGetValue(key, out v))
                    throw new IndexOutOfRangeException();

                Access access = v as Access;
                if (access != null)
                {
                    if (access.Set == null)
                        throw new IndexOutOfRangeException();
                    access.Set(_instance, value);
                }
                _dictionary[key] = value;
            }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <value>The keys.</value>
        /// <returns>An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" />.</returns>
        public override ICollection<string> Keys
        {
            get { return _dictionary.Keys; }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <value>The values.</value>
        /// <returns>An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" />.</returns>
        public override ICollection<object> Values
        {
            get
            {
                return new Dictionary<string, object>.ValueCollection(new Dictionary<string, object>(this));
            }
        }
        #endregion

        /// <summary>
        /// Performs an implicit conversion from <see cref="T"/> to <see cref="Accessor{T}"/>.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Accessor<T>(T instance)
        {
            return ReferenceEquals(instance, null) ? null : new Accessor<T>(instance);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Accessor{T}"/> to <see cref="T"/>.
        /// </summary>
        /// <param name="accessor">The accessor.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator T(Accessor<T> accessor)
        {
            return ReferenceEquals(accessor, null) ? default(T) : accessor._instance;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Accessor{T}"/> to <see cref="ReadOnlyDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <param name="accessor">The accessor.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator ReadOnlyDictionary<string, object>(Accessor<T> accessor)
        {
            return accessor == null
                ? null
                : new ReadOnlyDictionary<string, object>(new Dictionary<string, object>(accessor));
        }
    }
}