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
using System.Threading;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Database.Caching
{
    /// <summary>
    /// Holds the current row.
    /// </summary>
    internal class Row
    {
        /// <summary>
        /// The not read <see cref="Row"/>.
        /// </summary>
        [NotNull]
        public static readonly Row NotRead = new Row();

        /// <summary>
        /// The end <see cref="Row"/>.
        /// </summary>
        [NotNull]
        public static readonly Row End = new Row();

        /// <summary>
        /// The associated table definition.
        /// </summary>
        private readonly TableDefinition _tableDefinition;

        /// <summary>
        /// The SQL values
        /// </summary>
        private readonly object[] _sqlValues;

        /// <summary>
        /// The CLR values lazy initializer.
        /// </summary>
        private readonly Lazy<object[]> _values;

        /// <summary>
        /// Creates a default, invalid, instance of the <see cref="Row"/> class from being created.
        /// </summary>
        private Row()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Row"/> class.
        /// </summary>
        /// <param name="tableDefinition">The table definition.</param>
        /// <param name="sqlValues">The SQL values.</param>
        public Row([NotNull] TableDefinition tableDefinition, [NotNull] object[] sqlValues)
        {
            _tableDefinition = tableDefinition;
            _sqlValues = sqlValues;

            // Create lazy initializer for values.
            _values = new Lazy<object[]>(
                () =>
                {
                    object[] values = new object[tableDefinition.Count];
                    int v = 0;
                    foreach (Column column in tableDefinition)
                    {
                        if (column != null)
                            values[v] = SqlValueSerialization.GetCLRValueFromSqlVariant(
                                column.SqlDbType,
                                sqlValues[v]);
                        v++;
                    }
                    return values;
                },
                LazyThreadSafetyMode.ExecutionAndPublication);
        }

        /// <summary>
        /// The SQL values
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Invalid attempt to read when no data is present.</exception>
        public object[] SqlValues
        {
            get
            {
                if (_sqlValues == null)
                    throw new InvalidOperationException("Invalid attempt to read when no data is present.");
                return _sqlValues;
            }
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>The values.</value>
        /// <exception cref="System.InvalidOperationException">Invalid attempt to read when no data is present.</exception>
        public object[] Values
        {
            get
            {
                if (_values == null)
                    throw new InvalidOperationException("Invalid attempt to read when no data is present.");
                return _values.Value;
            }
        }

        /// <summary>
        /// The associated table definition.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Invalid attempt to read when no data is present.</exception>
        public TableDefinition TableDefinition
        {
            get
            {
                if (_tableDefinition == null)
                    throw new InvalidOperationException("Invalid attempt to read when no data is present.");
                return _tableDefinition;
            }
        }
    }
}