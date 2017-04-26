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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Blit;

namespace WebApplications.Utilities.Database.Schema
{
    /// <summary>
    ///   Holds information about a database schema
    /// </summary>
    public partial class DatabaseSchema
    {
        /// <summary>
        /// The scheme class holds all the data for a <see cref="DatabaseSchema"/>, in an immutable form.  It can then be swapped atomically when updating a
        /// <see cref="DatabaseSchema"/>.
        /// </summary>
        public class Schema : IEquatable<Schema>, ISchema
        {
            /// <summary>
            /// The schemas by unique identifier.
            /// </summary>
            [NotNull]
            private static readonly ConcurrentDictionary<Guid, Schema> _schemasById =
                new ConcurrentDictionary<Guid, Schema>();

            /// <summary>
            /// Holds all the SQL schemas (<see cref="SqlSchema"/>, using the <see cref="SqlSchema.ID"/> as the key.
            /// </summary>
            [PublicAPI]
            public IReadOnlyDictionary<int, SqlSchema> SchemasByID { get; }

            /// <summary>
            ///   Gets the SQL schemas that were loaded from the database.
            /// </summary>
            /// <value>
            ///   An enumerable containing the schema names in ascended order.
            /// </value>
            [PublicAPI]
            public IReadOnlyCollection<SqlSchema> Schemas { get; }

            /// <summary>
            ///   Holds all the program definitions (<see cref="SqlProgramDefinition"/>) for the schema.
            /// </summary>
            [PublicAPI]
            public IReadOnlyDictionary<string, SqlProgramDefinition> ProgramsByName { get; }

            /// <summary>
            ///   Gets the program definitions.
            /// </summary>
            /// <value>The program definitions.</value>
            [PublicAPI]
            public IReadOnlyCollection<SqlProgramDefinition> Programs { get; }

            /// <summary>
            ///   Holds all the table and view definitions (<see cref="SqlTableDefinition"/>) for the schema.
            /// </summary>
            [PublicAPI]
            public IReadOnlyDictionary<string, SqlTableDefinition> TablesByName { get; }

            /// <summary>
            ///   Gets the table and view definitions.
            /// </summary>
            /// <value>The table and view definitions.</value>
            [PublicAPI]
            public IReadOnlyCollection<SqlTableDefinition> Tables { get; }

            /// <summary>
            ///   Holds all the types for the schema, which are stored with the <see cref="T:WebApplications.Utilities.Database.Schema.SqlType.FullName">full
            ///   name</see> as the key and the <see cref="SqlType"/> as the value.
            /// </summary>
            [PublicAPI]
            public IReadOnlyDictionary<string, SqlType> TypesByName { get; }

            /// <summary>
            ///   Gets the SQL types from the schema.
            /// </summary>
            /// <value>
            ///   The <see cref="SqlType">type</see>.
            /// </value>
            [PublicAPI]
            public IReadOnlyCollection<SqlType> Types { get; }

            /// <summary>
            /// Holds all the collations for the schema
            /// </summary>
            public IReadOnlyDictionary<string, SqlCollation> CollationsByName { get; }

            /// <summary>
            /// Gets the collations for the schema.
            /// </summary>
            /// <value>
            /// The collations.
            /// </value>
            public IReadOnlyCollection<SqlCollation> Collations { get; }

            /// <summary>
            /// Gets the server collation.
            /// </summary>
            /// <value>
            /// The server collation.
            /// </value>
            public SqlCollation ServerCollation { get; }

            /// <summary>
            /// Gets the database collation.
            /// </summary>
            /// <value>
            /// The database collation.
            /// </value>
            public SqlCollation DatabaseCollation { get; }

            /// <summary>
            ///   Unique identity of the schema.
            /// </summary>
            [PublicAPI]
            public Guid Guid { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Schema" /> class.
            /// </summary>
            /// <param name="guid">The unique identifier.</param>
            /// <param name="schemasByID">The schemas by identifier.</param>
            /// <param name="programDefinitionsByName">Name of the program definitions by.</param>
            /// <param name="tablesByName">Name of the tables by.</param>
            /// <param name="typesByName">Name of the types by.</param>
            /// <param name="collationsByName">Name of the collations by.</param>
            /// <param name="schemas">The schemas.</param>
            /// <param name="programDefinitions">The program definitions.</param>
            /// <param name="tables">The tables.</param>
            /// <param name="types">The types.</param>
            /// <param name="collations">The collations.</param>
            /// <param name="serverCollation">The server collation.</param>
            /// <param name="databaseCollation">The database collation.</param>
            private Schema(
                Guid guid,
                [NotNull] IReadOnlyDictionary<int, SqlSchema> schemasByID,
                [NotNull] IReadOnlyDictionary<string, SqlProgramDefinition> programDefinitionsByName,
                [NotNull] IReadOnlyDictionary<string, SqlTableDefinition> tablesByName,
                [NotNull] IReadOnlyDictionary<string, SqlType> typesByName,
                [NotNull] IReadOnlyDictionary<string, SqlCollation> collationsByName,
                [NotNull] IReadOnlyCollection<SqlSchema> schemas,
                [NotNull] IReadOnlyCollection<SqlProgramDefinition> programDefinitions,
                [NotNull] IReadOnlyCollection<SqlTableDefinition> tables,
                [NotNull] IReadOnlyCollection<SqlType> types,
                [NotNull] IReadOnlyCollection<SqlCollation> collations,
                [NotNull] SqlCollation serverCollation,
                [NotNull] SqlCollation databaseCollation)
            {
                Guid = guid;
                SchemasByID = schemasByID;
                ProgramsByName = programDefinitionsByName;
                TablesByName = tablesByName;
                TypesByName = typesByName;
                Schemas = schemas;
                Programs = programDefinitions;
                Tables = tables;
                Types = types;
                CollationsByName = collationsByName;
                Collations = collations;
                ServerCollation = serverCollation;
                DatabaseCollation = databaseCollation;
            }

            /// <summary>
            /// Gets or adds a schema.
            /// </summary>
            /// <param name="schemasByID">The schemas by identifier.</param>
            /// <param name="programsByName">The programs by name.</param>
            /// <param name="tablesByName">The tables by name.</param>
            /// <param name="typesByName">The types by name.</param>
            /// <param name="collationsByName">The collations by name.</param>
            /// <param name="serverCollation">The server collation.</param>
            /// <param name="databaseCollation">The database collation.</param>
            /// <returns>The schema.</returns>
            [NotNull]
            protected internal static Schema GetOrAdd(
                [NotNull] IReadOnlyDictionary<int, SqlSchema> schemasByID,
                [NotNull] IReadOnlyDictionary<string, SqlProgramDefinition> programsByName,
                [NotNull] IReadOnlyDictionary<string, SqlTableDefinition> tablesByName,
                [NotNull] IReadOnlyDictionary<string, SqlType> typesByName,
                [NotNull] IReadOnlyDictionary<string, SqlCollation> collationsByName,
                [NotNull] SqlCollation serverCollation,
                [NotNull] SqlCollation databaseCollation)
            {
                // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute
                SqlSchema[] schemas = schemasByID.Values.OrderBy(s => s.FullName).ToArray();
                SqlProgramDefinition[] programs = programsByName.Values.Distinct().OrderBy(p => p.FullName).ToArray();
                SqlTableDefinition[] tables = tablesByName.Values.Distinct().OrderBy(t => t.FullName).ToArray();
                SqlType[] types = typesByName.Values.Distinct().OrderBy(t => t.FullName).ToArray();
                SqlCollation[] collations = collationsByName.Values.Distinct().OrderBy(t => t.Name).ToArray();
                // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute

                // ReSharper disable PossibleNullReferenceException
                Blittable4 blit4 = new Blittable4(
                    collations.Aggregate(
                        (serverCollation.GetHashCode() * 397) ^ databaseCollation.GetHashCode(),
                        (hash, collation) => (397 * hash) ^ collation.GetHashCode()));

                Blittable16 blit16 = new Blittable16(
                    schemas.Aggregate(
                        ((blit4.Byte0 << 16) * 397) ^ schemas.Length,
                        (hash, schema) => (397 * hash) ^ schema.GetHashCode()),
                    types.Aggregate(
                        ((blit4.Byte1 << 16) * 397) ^ types.Length,
                        (hash, type) => (397 * hash) ^ type.GetHashCode()),
                    programs.Aggregate(
                        ((blit4.Byte2 << 16) * 397) ^ programs.Length,
                        (hash, programDefinition) => (397 * hash) ^ programDefinition.GetHashCode()),
                    tables.Aggregate(
                        ((blit4.Byte3 << 16) * 397) ^ tables.Length,
                        (hash, tableDefinition) => (397 * hash) ^ tableDefinition.GetHashCode()));
                // ReSharper restore PossibleNullReferenceException

                // ReSharper disable once AssignNullToNotNullAttribute
                return _schemasById.GetOrAdd(
                    blit16.Guid,
                    g =>
                        new Schema(
                            g,
                            schemasByID,
                            programsByName,
                            tablesByName,
                            typesByName,
                            collationsByName,
                            schemas,
                            programs,
                            tables,
                            types,
                            collations,
                            serverCollation,
                            databaseCollation));
            }

            #region Equality methods
            /// <summary>
            /// Returns a boolean indicating if the passed in object is equal to this instance.
            /// </summary>
            /// <param name="obj">The object.</param>
            /// <returns>System.Boolean.</returns>
            public override bool Equals([CanBeNull] object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                Schema other = obj as Schema;
                return other != null && Guid.Equals(other.Guid);
            }

            /// <summary>
            /// Returns a boolean indicating if the passed in <see cref="Schema"/> is equal to this instance.
            /// </summary>
            /// <param name="other">The other <see cref="Schema"/>.</param>
            /// <returns>System.Boolean.</returns>
            public bool Equals([CanBeNull] ISchema other)
            {
                if (ReferenceEquals(null, other)) return false;
                return ReferenceEquals(this, other) || Guid.Equals(other.Guid);
            }

            /// <summary>
            /// Returns a boolean indicating if the passed in <see cref="Schema"/> is equal to this instance.
            /// </summary>
            /// <param name="other">The other <see cref="Schema"/>.</param>
            /// <returns>System.Boolean.</returns>
            public bool Equals([CanBeNull] Schema other)
            {
                if (ReferenceEquals(null, other)) return false;
                return ReferenceEquals(this, other) || Guid.Equals(other.Guid);
            }

            /// <summary>
            /// Gets the hash code.
            /// </summary>
            /// <returns>System.Int32.</returns>
            public override int GetHashCode()
            {
                return Guid.GetHashCode();
            }

            /// <summary>
            /// Implements the operator ==.
            /// </summary>
            /// <param name="left">The left.</param>
            /// <param name="right">The right.</param>
            /// <returns>The result of the operator.</returns>
            public static bool operator ==(Schema left, Schema right)
            {
                if (ReferenceEquals(left, null)) return ReferenceEquals(right, null);
                return !ReferenceEquals(right, null) && left.Guid.Equals(right.Guid);
            }

            /// <summary>
            /// Implements the operator !=.
            /// </summary>
            /// <param name="left">The left.</param>
            /// <param name="right">The right.</param>
            /// <returns>The result of the operator.</returns>
            public static bool operator !=(Schema left, Schema right)
            {
                if (ReferenceEquals(left, null)) return !ReferenceEquals(right, null);
                return ReferenceEquals(right, null) || !left.Guid.Equals(right.Guid);
            }
            #endregion
        }
    }
}