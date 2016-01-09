#region © Copyright Web Applications (UK) Ltd, 2016.  All rights reserved.
// Copyright (c) 2016, Web Applications UK Ltd
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
using System.Data;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Database.Caching
{
    /// <summary>
    /// Holds a table definition.
    /// </summary>
    internal class TableDefinition : IReadOnlyCollection<Column>
    {
        /// <summary>
        /// The <see cref="TableDefinition"/> indicating the end of stream.
        /// </summary>
        [NotNull]
        public static readonly TableDefinition End = new TableDefinition();

        /// <summary>
        /// Whether the table contains rows.
        /// </summary>
        private readonly bool _hasRows;

        /// <summary>
        /// The columns by name (case-insensitive).
        /// </summary>
        private readonly Dictionary<string, Column> _columnsByName;

        /// <summary>
        /// The columns in ordinal order.
        /// </summary>
        private readonly List<Column> _columns;

        /// <summary>
        /// The nullable columns.
        /// </summary>
        private readonly List<Column> _nullableColumns;

        /// <summary>
        /// Creates a default, invalid instance of the <see cref="TableDefinition"/>.
        /// </summary>
        private TableDefinition()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableDefinition"/> class.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <param name="hasRows">if set to <see langword="true" /> the table has rows.</param>
        public TableDefinition(int count, bool hasRows)
        {
            _columnsByName = new Dictionary<string, Column>(count, StringComparer.InvariantCultureIgnoreCase);
            _columns = new List<Column>(count);
            _nullableColumns = new List<Column>(count);
            _hasRows = hasRows;
        }

        /// <summary>
        /// Whether the table contains rows.
        /// </summary>
        public bool HasRows
        {
            get
            {
                if (_columns == null)
                    throw new InvalidOperationException("Invalid attempt to read when no data is present.");
                return _hasRows;
            }
        }

        /// <summary>
        /// Adds the specified column.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="sqlDbType">Type of the SQL database.</param>
        /// <param name="allowDBNull">if set to <see langword="true" /> then the column is nullable.</param>
        /// <returns>The newly created <see cref="Column"/>.</returns>
        public Column Add([NotNull] string name, SqlDbType sqlDbType, bool allowDBNull)
        {
            if (_columns == null)
                throw new InvalidOperationException("Invalid attempt to add a column.");
            Column column = new Column(_columns.Count, name, sqlDbType, allowDBNull);
            _columnsByName.Add(name, column);
            _columns.Add(column);
            if (allowDBNull)
                _nullableColumns.Add(column);
            return column;
        }

        /// <summary>
        /// Gets the nullable columns, in ordinal order.
        /// </summary>
        /// <value>The nullable columns.</value>
        /// <exception cref="InvalidOperationException">Invalid attempt to read when no data is present.</exception>
        [NotNull]
        public IReadOnlyCollection<Column> NullableColumns
        {
            get
            {
                if (_nullableColumns == null)
                    throw new InvalidOperationException("Invalid attempt to read when no data is present.");
                return _nullableColumns;
            }
        }

        /// <summary>
        /// Gets the <see cref="Column"/> with the specified ordinal.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>Column.</returns>
        /// <exception cref="InvalidOperationException">Invalid attempt to read when no data is present.</exception>
        public Column this[int ordinal]
        {
            get
            {
                if (_columns == null)
                    throw new InvalidOperationException("Invalid attempt to read when no data is present.");
                return _columns[ordinal];
            }
        }

        /// <summary>
        /// Gets the <see cref="Column"/> with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Column.</returns>
        /// <exception cref="InvalidOperationException">Invalid attempt to read when no data is present.</exception>
        public Column this[string name]
        {
            get
            {
                if (_columnsByName == null)
                    throw new InvalidOperationException("Invalid attempt to read when no data is present.");
                return _columnsByName[name];
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        /// <exception cref="InvalidOperationException">Invalid attempt to read when no data is present.</exception>
        public IEnumerator<Column> GetEnumerator()
        {
            if (_columns == null)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            return _columns.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        /// <value>The count.</value>
        /// <exception cref="System.InvalidOperationException">Invalid attempt to read when no data is present.</exception>
        public int Count
        {
            get
            {
                if (_columns == null)
                    throw new InvalidOperationException("Invalid attempt to read when no data is present.");
                return _columns.Count;
            }
        }
    }
}