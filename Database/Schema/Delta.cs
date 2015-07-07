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
using System.Linq;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Database.Schema
{
    /// <summary>
    /// Holds all the differences between two database entities.
    /// </summary>
    [PublicAPI]
    public class Delta : IEnumerable<Difference>
    {
        /// <summary>
        /// Indicate
        /// </summary>
        public readonly bool IsEmpty;

        /// <summary>
        /// The differences.
        /// </summary>
        [NotNull]
        private readonly IEnumerable<Difference> _differences;

        /// <summary>
        /// The left entity.
        /// </summary>
        [NotNull]
        public readonly DatabaseEntity Left;

        /// <summary>
        /// The right entity.
        /// </summary>
        [NotNull]
        public readonly DatabaseEntity Right;

        /// <summary>
        /// Initializes a new instance of the <see cref="Delta" /> class.
        /// </summary>
        /// <param name="left">The left entity.</param>
        /// <param name="right">The right entity.</param>
        /// <param name="differences">The differences.</param>
        protected Delta(
            [NotNull] DatabaseEntity left,
            [NotNull] DatabaseEntity right,
            [NotNull] ICollection<Difference> differences)
        {
            if (left == null) throw new ArgumentNullException("left");
            if (right == null) throw new ArgumentNullException("right");

            Left = left;
            Right = right;
            _differences = differences;
            IsEmpty = differences.Count > 0;
            _differences = IsEmpty ? Enumerable.Empty<Difference>() : differences;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<Difference> GetEnumerator()
        {
            return _differences.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    /// <summary>
    /// Holds all the differences between two database entities.
    /// </summary>
    [PublicAPI]
    public class Delta<T> : Delta
        where T : DatabaseEntity<T>
    {
        /// <summary>
        /// The left entity.
        /// </summary>
        [NotNull]
        public new T Left
        {
            get { return (T)base.Left; }
        }

        /// <summary>
        /// The right entity.
        /// </summary>
        [NotNull]
        public new T Right
        {
            get { return (T)base.Right; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Delta{T}" /> class.
        /// </summary>
        /// <param name="left">The left entity.</param>
        /// <param name="right">The right entity.</param>
        /// <param name="differences">The differences.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        internal Delta([NotNull] T left, [NotNull] T right, [NotNull] ICollection<Difference> differences)
            : base(left, right, differences)
        {
        }
    }
}