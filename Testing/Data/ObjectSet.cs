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
using WebApplications.Testing.Annotations;

namespace WebApplications.Testing.Data
{
    /// <summary>
    /// Holds a collection of records.
    /// </summary>
    /// <remarks></remarks>
    [PublicAPI]
    public class ObjectSet : IObjectSet, ICollection<IObjectRecord>
    {
        /// <summary>
        /// Holds the <see cref="RecordSetDefinition"/>.
        /// </summary>
        [NotNull]
        private readonly RecordSetDefinition _definition;

        /// <summary>
        /// The underlying list of records.
        /// </summary>
        [NotNull]
        private readonly List<IObjectRecord> _records = new List<IObjectRecord>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectSet" /> class.
        /// </summary>
        /// <param name="recordSetDefinition">The record set definition.</param>
        /// <param name="records">The records.</param>
        /// <remarks></remarks>
        public ObjectSet([NotNull] RecordSetDefinition recordSetDefinition, IEnumerable<IObjectRecord> records = null)
        {
            if (recordSetDefinition == null) throw new ArgumentNullException("recordSetDefinition");

            _definition = recordSetDefinition;

            if (records == null)
                return;

            foreach (IObjectRecord record in records)
                Add(record);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return _records.ToString();
        }

        #region ICollection<IObjectRecord> Members
        /// <inheritdoc/>
        public void Add(IObjectRecord item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if ((item.RecordSetDefinition != _definition) &&
                (item.RecordSetDefinition != RecordSetDefinition.ExceptionRecord))
                throw new ArgumentException(
                    "The record must have an identical recordset definition to be added to the current record.",
                    "item");

            _records.Add(item);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _records.Clear();
        }

        /// <inheritdoc/>
        public bool Contains(IObjectRecord item)
        {
            return _records.Contains(item);
        }

        /// <inheritdoc/>
        public void CopyTo(IObjectRecord[] array, int arrayIndex)
        {
            _records.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public bool Remove(IObjectRecord item)
        {
            return _records.Remove(item);
        }

        /// <inheritdoc/>
        public int Count
        {
            get { return _records.Count; }
        }

        /// <inheritdoc/>
        public bool IsReadOnly
        {
            get { return false; }
        }
        #endregion

        #region IObjectSet Members
        /// <inheritdoc/>
        public RecordSetDefinition Definition
        {
            get { return _definition; }
        }

        /// <inheritdoc/>
        public IEnumerator<IObjectRecord> GetEnumerator()
        {
            return _records.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}