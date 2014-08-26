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
using System.Diagnostics.Contracts;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.SqlServer.Server;

namespace WebApplications.Utilities.Database.Schema
{
    /// <summary>
    ///   Holds a table or view definition.
    /// </summary>
    public class SqlTableDefinition : IEquatable<SqlTableDefinition>, IEqualityComparer<SqlTableDefinition>
    {
        /// <summary>
        ///   The name of the table/view.
        /// </summary>
        [NotNull]
        public readonly string Name;

        /// <summary>
        ///   The schema.
        /// </summary>
        [NotNull]
        public readonly SqlSchema SqlSchema;

        /// <summary>
        ///   An enumeration storing the type of object this instance is.
        /// </summary>
        public readonly SqlObjectType Type;

        /// <summary>
        ///   A dictionary containing all of the columns.
        /// </summary>
        [NotNull]
        private readonly Dictionary<string, SqlColumn> _columns = new Dictionary<string, SqlColumn>();

        /// <summary>
        ///   Storage for the ordered <see cref="Columns">columns</see> (in ordinal order).
        /// </summary>
        private IEnumerable<SqlColumn> _columnsOrdered;

        /// <summary>
        ///   The hash code cache.
        /// </summary>
        private int? _hashCode;

        /// <summary>
        ///   The <see cref="SqlMetaData"/> for all the columns.
        /// </summary>
        private SqlMetaData[] _sqlMetaData;

        /// <summary>
        ///   Initializes a new instance of the <see cref="SqlTableDefinition" /> class.
        /// </summary>
        /// <param name="type">The object <see cref="Type">type</see>.</param>
        /// <param name="sqlSchema">The schema.</param>
        /// <param name="name">The table/view name.</param>
        /// <remarks>
        ///   There is a <see cref="System.Diagnostics.Contracts.Contract"/> specifying
        ///   that <paramref name="sqlSchema"/> and <paramref name="name"/> cannot be
        ///   <see cref="string.IsNullOrWhiteSpace">null or whitespace</see>.
        /// </remarks>
        internal SqlTableDefinition(SqlObjectType type, SqlSchema sqlSchema, string name)
        {
            Contract.Requires(sqlSchema != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(name));
            Type = type;
            Name = name;
            SqlSchema = sqlSchema;
        }

        /// <summary>
        ///   Gets all the <see cref="SqlColumn">columns</see> (in ordinal order).
        /// </summary>
        /// <value>
        ///   An enumerable containing all of the columns in ordinal order.
        /// </value>
        [NotNull]
        public IEnumerable<SqlColumn> Columns
        {
            get
            {
                // ReSharper disable PossibleNullReferenceException
                return _columnsOrdered ??
                       (_columnsOrdered = _columns.Values.OrderBy(c => c.Ordinal).ToList());
                // ReSharper restore PossibleNullReferenceException
            }
        }

        /// <summary>
        ///   Gets the <see cref="SqlMetaData"/> for all the columns.
        /// </summary>
        /// <value>
        ///   An <see cref="Array"/> containing the <see cref="SqlMetaData"/> for all the
        ///   <see cref="Columns">columns</see> in the view/table.
        /// </value>
        [NotNull]
        public SqlMetaData[] SqlMetaData
        {
            // ReSharper disable PossibleNullReferenceException
            get { return _sqlMetaData ?? (_sqlMetaData = Columns.Select(c => c.SqlMetaData).ToArray()); }
            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>
        ///   Gets the full name of the table/view.
        /// </summary>
        /// <value>
        ///   The full name, which is formatted as: <see cref="SqlSchema"/>.<see cref="Name"/>
        /// </value>
        public string FullName
        {
            get { return string.Format("{0}.{1}", SqlSchema.Name, Name); }
        }

        #region IEqualityComparer<SqlTableDefinition> Members
        /// <summary>
        ///   Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first <see cref="SqlTableDefinition"/> to compare.</param>
        /// <param name="y">The second <see cref="SqlTableDefinition"/> to compare.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the specified objects are equal; otherwise returns <see langword="false"/>.
        /// </returns>
        public bool Equals(SqlTableDefinition x, SqlTableDefinition y)
        {
            return x == null
                       ? y == null
                       : y != null && x.Equals(y);
        }

        /// <summary>
        ///   Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> for which a hash code is to be returned.</param>
        /// <returns>
        ///   A hash code for the specified object.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   The type of <paramref name="obj"/> is a reference type and is <see langword="null"/>.
        /// </exception>
        public int GetHashCode([NotNull] SqlTableDefinition obj)
        {
            if (obj._hashCode == null)
            {
                // ReSharper disable PossibleNullReferenceException
                obj._hashCode =
                    Columns.Aggregate(
                        Type.GetHashCode() ^ Name.GetHashCode() ^ SqlSchema.GetHashCode(),
                        (h, c) => h ^ c.GetHashCode());
                // ReSharper restore PossibleNullReferenceException
            }
            return (int) obj._hashCode;
        }
        #endregion

        #region IEquatable<SqlTableDefinition> Members
        /// <summary>
        ///   Indicates whether the current <see cref="SqlTableDefinition"/> is equal to another instance.
        /// </summary>
        /// <param name="other">The <see cref="SqlTableDefinition"/> to compare with this object.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the current instance is equal to the
        ///   <paramref name="other"/> instance; otherwise returns <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="other"/> is <see langword="null"/>.
        /// </exception>
        public bool Equals(SqlTableDefinition other)
        {
            if (other == null)
                return false;
            return (Type == other.Type) && (FullName == other.FullName) &&
                   (Columns.SequenceEqual(other.Columns));
        }
        #endregion

        /// <summary>
        ///   Adds the column information to the collection.
        /// </summary>
        /// <param name="column">The column to add.</param>
        internal void AddColumn([NotNull] SqlColumn column)
        {
            Contract.Requires(column != null);
            _columns.Add(column.Name, column);
        }

        /// <summary>
        ///   Tries to get the column with the specified name.
        /// </summary>
        /// <param name="columnName">The name of the column to retrieve.</param>
        /// <param name="column">The retrieved column object.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the column is found; otherwise returns <see langword="false"/>.
        /// </returns>
        public bool TryGetColumn([NotNull] string columnName, out SqlColumn column)
        {
            return _columns.TryGetValue(columnName.ToLower(), out column);
        }

        /// <summary>
        ///   Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        ///   <para>A <see cref="string"/> representation of this instance.</para>
        ///   <para><b>Format:</b> <see cref="Name"/> + "Table".</para>
        /// </returns>
        public override string ToString()
        {
            return Name + " Table";
        }
    }
}