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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using Microsoft.SqlServer.Server;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Database.Schema
{
    /// <summary>
    ///   Holds a table or view definition.
    /// </summary>
    [PublicAPI]
    public sealed class SqlTableDefinition : DatabaseSchemaEntity<SqlTableDefinition>
    {
        /// <summary>
        /// The properties used for calculating differences.
        /// </summary>
        [UsedImplicitly]
        [NotNull]
        private static readonly Expression<Func<SqlTableDefinition, object>>[] _properties =
        {
            t => t.TableType,
            t => t.Type,
            t => t.Columns
        };

        /// <summary>
        ///   The name of the table/view.
        /// </summary>
        [NotNull]
        public readonly string Name;

        /// <summary>
        ///   An enumeration storing the type of object this instance is.
        /// </summary>
        public readonly SqlObjectType Type;

        /// <summary>
        /// The <see cref="SqlTableType"/> if this instance <see cref="SqlTableType.TableDefinition">defines</see> a <see cref="SqlTableType"/>;
        /// otherwise <see langword="null"/>.
        /// </summary>
        public readonly SqlTableType TableType;

        /// <summary>
        ///   Storage for the ordered <see cref="Columns">columns</see> (in ordinal order).
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public readonly IEnumerable<SqlColumn> Columns;

        /// <summary>
        /// Gets the column count.
        /// </summary>
        /// <value>The column count.</value>
        public int ColumnCount
        {
            get { return SqlMetaData.Length; }
        }

        /// <summary>
        ///   A dictionary containing all of the columns.
        /// </summary>
        [NotNull]
        private readonly IReadOnlyDictionary<string, SqlColumn> _columnsByName;

        /// <summary>
        ///   Gets the <see cref="Microsoft.SqlServer.Server.SqlMetaData"/> for all the columns.
        /// </summary>
        /// <value>
        ///   An <see cref="Array"/> containing the <see cref="Microsoft.SqlServer.Server.SqlMetaData"/> for all the
        ///   <see cref="Columns">columns</see> in the view/table.
        /// </value>
        [NotNull]
        internal readonly SqlMetaData[] SqlMetaData;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlTableDefinition" /> class.
        /// </summary>
        /// <param name="type">The object <see cref="Type">type</see>.</param>
        /// <param name="sqlSchema">The schema.</param>
        /// <param name="name">The table/view name.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="tableType">Type of the table.</param>
        internal SqlTableDefinition(
            SqlObjectType type,
            [NotNull] SqlSchema sqlSchema,
            [NotNull] string name,
            [NotNull] SqlColumn[] columns,
            [CanBeNull] SqlTableType tableType)
            : base(sqlSchema, name)
            // ReSharper restore PossibleNullReferenceException
        {
            if (columns == null) throw new ArgumentNullException("columns");

            Type = type;
            Name = name;
            TableType = tableType;
            Columns = columns;
            Dictionary<string, SqlColumn> columnsByName = new Dictionary<string, SqlColumn>(
                columns.Length,
                StringComparer.InvariantCultureIgnoreCase);
            _columnsByName = columnsByName;
            SqlMetaData = new SqlMetaData[columns.Length];
            if (tableType != null)
                tableType.TableDefinition = this;

            int i = 0;
            foreach (SqlColumn column in columns)
            {
                Debug.Assert(column != null);
                SqlMetaData[i++] = column.SqlMetaData;
                columnsByName[column.FullName] = column;
            }
        }

        /// <summary>
        /// Gets the column with the specified name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>Returns a <see cref="SqlColumn"/>, if found; otherwise <see langword="null"/>.</returns>
        [CanBeNull]
        public SqlColumn GetColumn([NotNull] string columnName)
        {
            if (columnName == null) throw new ArgumentNullException("columnName");

            SqlColumn column;
            return _columnsByName.TryGetValue(columnName.ToLower(), out column)
                ? column
                : null;
        }

        /// <summary>
        ///   Tries to get the column with the specified name.
        /// </summary>
        /// <param name="columnName">The name of the column to retrieve.</param>
        /// <param name="column">The retrieved column object.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the column is found; otherwise returns <see langword="false"/>.
        /// </returns>
        [ContractAnnotation("=>true, column:notnull;=>false, column:null")]
        public bool TryGetColumn([NotNull] string columnName, out SqlColumn column)
        {
            if (columnName == null) throw new ArgumentNullException("columnName");

            return _columnsByName.TryGetValue(columnName.ToLower(), out column);
        }
    }
}