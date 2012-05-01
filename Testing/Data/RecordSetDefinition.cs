#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Copyright (c) 2012, Web Applications UK Ltd
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
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;

namespace WebApplications.Testing.Data
{
    /// <summary>
    /// Defines a record set.
    /// </summary>
    /// <remarks></remarks>
    public class RecordSetDefinition
    {
        /// <summary>
        /// Special case record set definition used to identify exception records.
        /// </summary>
        [NotNull]
        public static readonly RecordSetDefinition ExceptionRecord = new RecordSetDefinition(
            new ColumnDefinition("Exception", SqlDbType.Variant));

        /// <summary>
        /// Gets the column definitions array.
        /// </summary>
        /// <value>The column definitions array.</value>
        /// <remarks></remarks>
        [NotNull] private readonly ColumnDefinition[] _columnsArray;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordSetDefinition" /> class.
        /// </summary>
        /// <param name="columnDefinitions">The column definitions.</param>
        /// <remarks></remarks>
        public RecordSetDefinition([NotNull] IEnumerable<ColumnDefinition> columnDefinitions)
        {
            _columnsArray = columnDefinitions.ToArray();

            if (_columnsArray.Length < 1)
                throw new ArgumentOutOfRangeException("columnDefinitions", columnDefinitions,
                                                      "The column definitions must have at least one column.");

            for (int c = 0; c < _columnsArray.Length; c++)
            {
                ColumnDefinition columnDefinition = _columnsArray[c];
                if (columnDefinition == null)
                    throw new ArgumentOutOfRangeException("columnDefinitions", columnDefinitions,
                                                          string.Format(
                                                              "The column definition at index '{0} must not be null.", c));

                if (columnDefinition.RecordSetDefinition != null)
                    throw new InvalidOperationException(
                        "The column definition cannot be added to the recordset definition as it already belongs to a different record set definition.");

                // ReSharper disable HeuristicUnreachableCode
                columnDefinition.RecordSetDefinition = this;
                columnDefinition.Ordinal = c;
                // ReSharper restore HeuristicUnreachableCode
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordSetDefinition" /> class.
        /// </summary>
        /// <param name="columnDefinitions">The column definitions.</param>
        /// <remarks></remarks>
        public RecordSetDefinition([NotNull] params ColumnDefinition[] columnDefinitions)
            : this((IEnumerable<ColumnDefinition>) columnDefinitions)
        {
        }

        /// <summary>
        /// Gets the column definitions in the record set.
        /// </summary>
        /// <value>The column definitions.</value>
        /// <remarks></remarks>
        [NotNull]
        public IEnumerable<ColumnDefinition> Columns
        {
            get { return _columnsArray; }
        }

        /// <summary>
        /// Gets the field count (number of columns).
        /// </summary>
        /// <value>The field count.</value>
        /// <remarks></remarks>
        public int FieldCount
        {
            get { return _columnsArray.Length; }
        }

        /// <summary>
        /// Gets the <see cref="ColumnDefinition" /> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The <see cref="ColumnDefinition"/>.</returns>
        /// <remarks></remarks>
        [NotNull]
        public ColumnDefinition this[int index]
        {
            get
            {
                if ((index < 0) ||
                    (index > FieldCount))
                    throw new IndexOutOfRangeException(index.ToString(CultureInfo.InvariantCulture));
                return _columnsArray[index];
            }
        }


        /// <summary>
        /// Gets the ordinal.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        /// <remarks></remarks>
        public int GetOrdinal(string name)
        {
            CompareInfo compare = CultureInfo.InvariantCulture.CompareInfo;
            for (int c = 0; c < FieldCount; c++)
            {
                if (
                    compare.Compare(_columnsArray[c].Name, name,
                                    CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType |
                                    CompareOptions.IgnoreWidth) == 0)
                    return c;
            }
            throw new IndexOutOfRangeException(name);
        }
    }
}