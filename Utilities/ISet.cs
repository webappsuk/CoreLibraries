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
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Non-generic version of <see cref="ISet{T}" />
    /// </summary>
    public interface ISet : ICollection, IEquatable<ISet>, IEquatable<IEnumerable>, IEnumerable<object>
    {
        /// <summary>
        /// Clears this instance.
        /// </summary>
        void Clear();

        /// <summary>
        /// Adds an element to the current set and returns a value to indicate if the element was successfully added.
        /// </summary>
        /// <returns>
        /// true if the element is added to the set; false if the element is already in the set.
        /// </returns>
        /// <param name="item">The element to add to the set.</param>
        bool Add([NotNull] object item);

        /// <summary>
        /// Removes an element to the current set and returns a value to indicate if the element was successfully added.
        /// </summary>
        /// <returns>
        /// true if the element is added to the set; false if the element is already in the set.
        /// </returns>
        /// <param name="item">The element to add to the set.</param>
        bool Remove([NotNull] object item);

        /// <summary>
        /// Determines whether the set contains the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        ///     <see langword="true" /> if the set contains the specified item; otherwise, <see langword="false" />.
        /// </returns>
        bool Contains(object item);

        /// <summary>
        /// Modifies the current set so that it contains all elements that are present in either the current set or the specified
        /// collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="other" /> is null.
        /// </exception>
        void UnionWith([NotNull] IEnumerable other);

        /// <summary>
        /// Modifies the current set so that it contains only elements that are also in a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="other" /> is null.
        /// </exception>
        void IntersectWith([NotNull] IEnumerable other);

        /// <summary>
        /// Removes all elements in the specified collection from the current set.
        /// </summary>
        /// <param name="other">The collection of items to remove from the set.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="other" /> is null.
        /// </exception>
        void ExceptWith([NotNull] IEnumerable other);

        /// <summary>
        /// Modifies the current set so that it contains only elements that are present either in the current set or in the
        /// specified collection, but not both.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="other" /> is null.
        /// </exception>
        void SymmetricExceptWith([NotNull] IEnumerable other);

        /// <summary>
        /// Creates a new set that contains all elements that are present in either the current set or the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="other" /> is null.
        /// </exception>
        [NotNull]
        ISet Union([NotNull] IEnumerable other);

        /// <summary>
        /// Creates a new set that contains only elements that are also in a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="other" /> is null.
        /// </exception>
        [NotNull]
        ISet Intersect([NotNull] IEnumerable other);

        /// <summary>
        /// Creates a new set that contains only elements that are not in a specified collection.
        /// </summary>
        /// <param name="other">The collection of items to remove from the set.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="other" /> is null.
        /// </exception>
        [NotNull]
        ISet Except([NotNull] IEnumerable other);

        /// <summary>
        /// Determines whether a set is a subset of a specified collection.
        /// </summary>
        /// <returns>
        /// true if the current set is a subset of <paramref name="other" />; otherwise, false.
        /// </returns>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="other" /> is null.
        /// </exception>
        bool IsSubsetOf([NotNull] IEnumerable other);

        /// <summary>
        /// Determines whether the current set is a superset of a specified collection.
        /// </summary>
        /// <returns>
        /// true if the current set is a superset of <paramref name="other" />; otherwise, false.
        /// </returns>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="other" /> is null.
        /// </exception>
        bool IsSupersetOf([NotNull] IEnumerable other);

        /// <summary>
        /// Determines whether the current set is a proper (strict) superset of a specified collection.
        /// </summary>
        /// <returns>
        /// true if the current set is a proper superset of <paramref name="other" />; otherwise, false.
        /// </returns>
        /// <param name="other">The collection to compare to the current set. </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="other" /> is null.
        /// </exception>
        bool IsProperSupersetOf([NotNull] IEnumerable other);

        /// <summary>
        /// Determines whether the current set is a proper (strict) subset of a specified collection.
        /// </summary>
        /// <returns>
        /// true if the current set is a proper subset of <paramref name="other" />; otherwise, false.
        /// </returns>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="other" /> is null.
        /// </exception>
        bool IsProperSubsetOf([NotNull] IEnumerable other);

        /// <summary>
        /// Determines whether the current set overlaps with the specified collection.
        /// </summary>
        /// <returns>
        /// true if the current set and <paramref name="other" /> share at least one common element; otherwise, false.
        /// </returns>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="other" /> is null.
        /// </exception>
        bool Overlaps([NotNull] IEnumerable other);

        /// <summary>
        /// Determines whether the current set and the specified collection contain the same elements.
        /// </summary>
        /// <returns>
        /// true if the current set is equal to <paramref name="other" />; otherwise, false.
        /// </returns>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="other" /> is null.
        /// </exception>
        bool SetEquals([NotNull] IEnumerable other);

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>ISet.</returns>
        [NotNull]
        ISet Clone();

        /// <summary>
        /// Gets the type of the objects stored in the set.
        /// </summary>
        /// <value>The type.</value>
        [NotNull]
        Type Type { get; }
    }
}