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
using System.Diagnostics.Contracts;
using System.Linq;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Implements a <see cref="HashSet{T}" /> with <see cref="ICollection" /> support, allowing extraction of non-generic
    /// information.
    /// </summary>
    /// <typeparam name="T">The object type.</typeparam>
    public class HashCollection<T> : HashSet<T>, ISet, IEquatable<HashCollection<T>>, IEquatable<IEnumerable<T>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HashCollection{T}" /> class.
        /// </summary>
        public HashCollection()
            : base(EqualityComparer<T>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashCollection{T}" /> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        public HashCollection(IEqualityComparer<T> comparer)
            : base(comparer)
        {
        }

        /// <summary>
        /// Implementation Notes:
        /// Since resizes are relatively expensive (require rehashing), this attempts to minimize
        /// the need to resize by setting the initial capacity based on size of collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public HashCollection([NotNull] IEnumerable<T> collection)
            : base(collection, EqualityComparer<T>.Default)
        {
            Contract.Requires(collection != null);
        }

        /// <summary>
        /// Implementation Notes:
        /// Since resizes are relatively expensive (require rehashing), this attempts to minimize
        /// the need to resize by setting the initial capacity based on size of collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="comparer">The comparer.</param>
        public HashCollection([NotNull] IEnumerable<T> collection, IEqualityComparer<T> comparer)
            : base(collection, comparer)
        {
            Contract.Requires(collection != null);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(HashCollection<T> other)
        {
            if (ReferenceEquals(null, other))
                return false;
            return SetEquals(other);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(IEnumerable<T> other)
        {
            if (ReferenceEquals(null, other))
                return false;
            return SetEquals(other);
        }

        /// <summary>
        /// Copies th.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            // Grab source array (not very efficient, but we shouldn't use this method normally anyway).
            T[] source = this.ToArray<T>();

            Contract.Assert(arrayIndex < array.Length && source.Length <= array.Length - arrayIndex);
            for (int i = 0; i < source.Length; i++)
                array.SetValue(source[i], arrayIndex + i);
        }

        /// <summary>
        /// Gets the sync root.
        /// </summary>
        /// <value>The sync root.</value>
        object ICollection.SyncRoot
        {
            get { return this; }
        }

        /// <summary>
        /// Gets the is synchronized.
        /// </summary>
        /// <value>The is synchronized.</value>
        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        /// <summary>
        /// Adds an element to the current set and returns a value to indicate if the element was successfully added.
        /// </summary>
        /// <param name="item">The element to add to the set.</param>
        /// <returns>true if the element is added to the set; false if the element is already in the set.</returns>
        bool ISet.Add(object item)
        {
            Contract.Assert(item == null || item is T);
            return Add((T)item);
        }

        /// <summary>
        /// Removes an element to the current set and returns a value to indicate if the element was successfully added.
        /// </summary>
        /// <param name="item">The element to add to the set.</param>
        /// <returns>true if the element is added to the set; false if the element is already in the set.</returns>
        public bool Remove(object item)
        {
            return base.Remove((T)item);
        }

        /// <summary>
        /// Determines whether the set contains the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        ///     <see langword="true" /> if the set contains the specified item; otherwise, <see langword="false" />.
        /// </returns>
        public bool Contains(object item)
        {
            Contract.Assert(item == null || item is T);
            return base.Contains((T)item);
        }

        /// <summary>
        /// Modifies the current set so that it contains all elements that are present in either the current set or the specified
        /// collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        void ISet.UnionWith(IEnumerable other)
        {
            Contract.Assert(other != null);
            UnionWith(other.Cast<T>());
        }

        /// <summary>
        /// Modifies the current set so that it contains only elements that are also in a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        void ISet.IntersectWith(IEnumerable other)
        {
            Contract.Assert(other != null);
            IntersectWith(other.Cast<T>());
        }

        /// <summary>
        /// Removes all elements in the specified collection from the current set.
        /// </summary>
        /// <param name="other">The collection of items to remove from the set.</param>
        void ISet.ExceptWith(IEnumerable other)
        {
            Contract.Assert(other != null);
            ExceptWith(other.Cast<T>());
        }

        /// <summary>
        /// Gets the number of elements that are contained in a set.
        /// </summary>
        /// <value>The count.</value>
        int ICollection.Count
        {
            get { return Count; }
        }

        /// <summary>
        /// Modifies the current set so that it contains only elements that are present either in the current set or in the
        /// specified collection, but not both.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        void ISet.SymmetricExceptWith(IEnumerable other)
        {
            Contract.Assert(other != null);
            SymmetricExceptWith(other.Cast<T>());
        }

        /// <summary>
        /// Creates a new set that contains all elements that are present in either the current set or the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns>ISet.</returns>
        public ISet Union(IEnumerable other)
        {
            return new HashCollection<T>(Enumerable.Union(this, other.Cast<T>()));
        }

        /// <summary>
        /// Creates a new set that contains only elements that are also in a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns>ISet.</returns>
        public ISet Intersect(IEnumerable other)
        {
            return new HashCollection<T>(Enumerable.Intersect(this, other.Cast<T>()));
        }

        /// <summary>
        /// Creates a new set that contains only elements that are not in a specified collection.
        /// </summary>
        /// <param name="other">The collection of items to remove from the set.</param>
        /// <returns>ISet.</returns>
        public ISet Except(IEnumerable other)
        {
            return new HashCollection<T>(Enumerable.Except(this, other.Cast<T>()));
        }

        /// <summary>
        /// Determines whether a set is a subset of a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns>
        /// true if the current set is a subset of <paramref name="other" />; otherwise, false.
        /// </returns>
        bool ISet.IsSubsetOf(IEnumerable other)
        {
            Contract.Assert(other != null);
            return IsSubsetOf(other.OfType<T>());
        }

        /// <summary>
        /// Determines whether the current set is a superset of a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns>
        /// true if the current set is a superset of <paramref name="other" />; otherwise, false.
        /// </returns>
        bool ISet.IsSupersetOf(IEnumerable other)
        {
            Contract.Assert(other != null);
            return IsSupersetOf(other.Cast<T>());
        }

        /// <summary>
        /// Determines whether the current set is a proper (strict) superset of a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns>
        /// true if the current set is a proper superset of <paramref name="other" />; otherwise, false.
        /// </returns>
        bool ISet.IsProperSupersetOf(IEnumerable other)
        {
            Contract.Assert(other != null);
            return IsProperSupersetOf(other.Cast<T>());
        }

        /// <summary>
        /// Determines whether the current set is a proper (strict) subset of a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns>
        /// true if the current set is a proper subset of <paramref name="other" />; otherwise, false.
        /// </returns>
        bool ISet.IsProperSubsetOf(IEnumerable other)
        {
            Contract.Assert(other != null);
            return IsProperSubsetOf(other.OfType<T>());
        }

        /// <summary>
        /// Determines whether the current set overlaps with the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns>
        /// true if the current set and <paramref name="other" /> share at least one common element; otherwise, false.
        /// </returns>
        bool ISet.Overlaps(IEnumerable other)
        {
            Contract.Assert(other != null);
            return Overlaps(other.OfType<T>());
        }

        /// <summary>
        /// Determines whether the current set and the specified collection contain the same elements.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns>
        /// true if the current set is equal to <paramref name="other" />; otherwise, false.
        /// </returns>
        bool ISet.SetEquals(IEnumerable other)
        {
            Contract.Assert(other != null);
            return SetEquals(other.OfType<T>());
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>ISet.</returns>
        ISet ISet.Clone()
        {
            return new HashCollection<T>(this);
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>HashCollection.</returns>
        [NotNull]
        [PublicAPI]
        public HashCollection<T> Clone()
        {
            return new HashCollection<T>(this);
        }

        /// <summary>
        /// Gets the type of the objects stored in the set.
        /// </summary>
        /// <value>The type.</value>
        public Type Type
        {
            get { return typeof(T); }
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(ISet other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (Count != other.Count)
                return false;
            return SetEquals(other.OfType<T>());
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(IEnumerable other)
        {
            if (ReferenceEquals(null, other))
                return false;
            return SetEquals(other.OfType<T>());
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        ///     <see langword="true" /> if the specified <see cref="System.Object" /> is equal to this instance; otherwise,
        ///     <see langword="false" />.
        /// </returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as IEnumerable);
        }

        /// <summary>
        /// Implements the ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(HashCollection<T> left, HashCollection<T> right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Implements the !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(HashCollection<T> left, HashCollection<T> right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return Count;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        IEnumerator<object> IEnumerable<object>.GetEnumerator()
        {
            // ReSharper disable once RedundantCast
            return ((HashSet<T>)this).ToArray().Cast<object>().GetEnumerator();
        }

        /// <summary>
        /// Returns a string representation of the collection.
        /// </summary>
        /// <returns>A string representation of the collection.</returns>
        public override string ToString()
        {
            return string.Join(", ", this.Select<T, string>(x => ReferenceEquals(x, null) ? "null" : x.ToString()));
        }
    }
}