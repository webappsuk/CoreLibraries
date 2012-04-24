#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: Utilities.Database 
// Project: Utilities.Database
// File: SqlTableDefinition.cs
// 
// This software, its object code and source code and all modifications made to
// the same (the “Software”) are, and shall at all times remain, the proprietary
// information and intellectual property rights of Web Applications (UK) Limited. 
// You are only entitled to use the Software as expressly permitted by Web
// Applications (UK) Limited within the Software Customisation and
// Licence Agreement (the “Agreement”).  Any copying, modification, decompiling,
// distribution, licensing, sale, transfer or other use of the Software other than
// as expressly permitted in the Agreement is expressly forbidden.  Web
// Applications (UK) Limited reserves its rights to take action against you and
// your employer in accordance with its contractual and common law rights
// (including injunctive relief) should you breach the terms of the Agreement or
// otherwise infringe its copyright or other intellectual property rights in the
// Software.
// 
// © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.SqlServer.Server;
using WebApplications.Testing.Data;

namespace WebApplications.Testing.Data
{
    /// <summary>
    ///   Holds a table or view definition.
    /// </summary>
    public class SqlTableDefinition : IEquatable<SqlTableDefinition>, IEqualityComparer<SqlTableDefinition>
    {
        /// <summary>
        ///   The name of the table/view.
        /// </summary>
        public readonly string Name;

        /// <summary>
        ///   The schema name.
        /// </summary>
        public readonly string SchemaName;

        /// <summary>
        ///   An enumeration storing the type of object this instance is.
        /// </summary>
        public readonly SqlObjectType Type;

        /// <summary>
        ///   A dictionary containing all of the columns.
        /// </summary>
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
        /// <param name="schemaName">The schema name.</param>
        /// <param name="name">The table/view name.</param>
        /// <remarks>
        ///   There is a <see cref="System.Diagnostics.Contracts.Contract"/> specifying
        ///   that <paramref name="schemaName"/> and <paramref name="name"/> cannot be
        ///   <see cref="string.IsNullOrWhiteSpace">null or whitespace</see>.
        /// </remarks>
        public SqlTableDefinition(string schemaName, string name, SqlObjectType type = SqlObjectType.Table)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(schemaName));
            Contract.Requires(!string.IsNullOrWhiteSpace(name));
            switch (type)
            {
                case SqlObjectType.U:
                case SqlObjectType.IT:
                case SqlObjectType.S:
                case SqlObjectType.TT:
                case SqlObjectType.V:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type", type,
                                                          "The type for a table definition must be one of the table types.");
            }
            Type = type; 
            Name = name;
            SchemaName = schemaName;
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
        ///   The full name, which is formatted as: <see cref="SchemaName"/>.<see cref="Name"/>
        /// </value>
        public string FullName
        {
            get { return string.Format("{0}.{1}", SchemaName, Name); }
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
        public int GetHashCode([NotNull]SqlTableDefinition obj)
        {
            if (obj._hashCode == null)
            {
                // ReSharper disable PossibleNullReferenceException
                obj._hashCode =
                    Columns.Aggregate(
                        Type.GetHashCode() ^ Name.GetHashCode() ^ SchemaName.GetHashCode(),
                        (h, c) => h ^ c.GetHashCode());
                // ReSharper restore PossibleNullReferenceException
            }
            return (int)obj._hashCode;
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
        internal void AddColumn([NotNull]SqlColumn column)
        {
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
        public bool TryGetColumn([NotNull]string columnName, out SqlColumn column)
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