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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using WebApplications.Utilities.Annotations;

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
            private static readonly ConcurrentDictionary<Guid, Schema> _schemasByGuid =
                new ConcurrentDictionary<Guid, Schema>();

            [NotNull]
            private readonly IReadOnlyDictionary<int, SqlSchema> _schemasByID;

            /// <summary>
            /// Holds all the SQL schemas (<see cref="SqlSchema"/>, using the <see cref="SqlSchema.ID"/> as the key.
            /// </summary>
            [PublicAPI]
            public IReadOnlyDictionary<int, SqlSchema> SchemasByID
            {
                get { return _schemasByID; }
            }

            [NotNull]
            private readonly IReadOnlyDictionary<string, SqlProgramDefinition> _programDefinitionsByName;

            /// <summary>
            ///   Holds all the program definitions (<see cref="SqlProgramDefinition"/>) for the schema.
            /// </summary>
            [PublicAPI]
            public IReadOnlyDictionary<string, SqlProgramDefinition> ProgramDefinitionsByName
            {
                get { return _programDefinitionsByName; }
            }

            [NotNull]
            private readonly IReadOnlyDictionary<string, SqlTableDefinition> _tablesByName;

            /// <summary>
            ///   Holds all the table and view definitions (<see cref="SqlTableDefinition"/>) for the schema.
            /// </summary>
            [PublicAPI]
            public IReadOnlyDictionary<string, SqlTableDefinition> TablesByName
            {
                get { return _tablesByName; }
            }

            [NotNull]
            private readonly IReadOnlyDictionary<string, SqlType> _typesByName;

            /// <summary>
            ///   Holds all the types for the schema, which are stored with the <see cref="SqlType.FullName">full
            ///   name</see> as the key and the <see cref="SqlType"/> as the value.
            /// </summary>
            [PublicAPI]
            public IReadOnlyDictionary<string, SqlType> TypesByName
            {
                get { return _typesByName; }
            }

            [NotNull]
            private readonly IEnumerable<SqlSchema> _schemas;

            /// <summary>
            ///   Gets the SQL schemas that were loaded from the database.
            /// </summary>
            /// <value>
            ///   An enumerable containing the schema names in ascended order.
            /// </value>
            [PublicAPI]
            public IEnumerable<SqlSchema> Schemas
            {
                get { return _schemas; }
            }

            [NotNull]
            private readonly IEnumerable<SqlType> _types;

            /// <summary>
            ///   Gets the SQL types from the schema.
            /// </summary>
            /// <value>
            ///   The <see cref="SqlType">type</see>.
            /// </value>
            [PublicAPI]
            public IEnumerable<SqlType> Types
            {
                get { return _types; }
            }

            [NotNull]
            private readonly IEnumerable<SqlProgramDefinition> _programDefinitions;

            /// <summary>
            ///   Gets the program definitions.
            /// </summary>
            /// <value>The program definitions.</value>
            [PublicAPI]
            public IEnumerable<SqlProgramDefinition> ProgramDefinitions
            {
                get { return _programDefinitions; }
            }

            [NotNull]
            private readonly IEnumerable<SqlTableDefinition> _tables;

            /// <summary>
            ///   Gets the table and view definitions.
            /// </summary>
            /// <value>The table and view definitions.</value>
            [PublicAPI]
            public IEnumerable<SqlTableDefinition> Tables
            {
                get { return _tables; }
            }

            private readonly Guid _guid;

            /// <summary>
            ///   Unique identity of the schema.
            /// </summary>
            [PublicAPI]
            public Guid Guid
            {
                get { return _guid; }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Schema"/> class.
            /// </summary>
            /// <param name="guid">The unique identifier.</param>
            /// <param name="schemasByID">The schemas by identifier.</param>
            /// <param name="programDefinitionsByName">Name of the program definitions by.</param>
            /// <param name="tablesByName">Name of the tables by.</param>
            /// <param name="typesByName">Name of the types by.</param>
            /// <param name="schemas">The schemas.</param>
            /// <param name="programDefinitions">The program definitions.</param>
            /// <param name="tables">The tables.</param>
            /// <param name="types">The types.</param>
            private Schema(
                Guid guid,
                [NotNull] IReadOnlyDictionary<int, SqlSchema> schemasByID,
                [NotNull] IReadOnlyDictionary<string, SqlProgramDefinition> programDefinitionsByName,
                [NotNull] IReadOnlyDictionary<string, SqlTableDefinition> tablesByName,
                [NotNull] IReadOnlyDictionary<string, SqlType> typesByName,
                [NotNull] IEnumerable<SqlSchema> schemas,
                [NotNull] IEnumerable<SqlProgramDefinition> programDefinitions,
                [NotNull] IEnumerable<SqlTableDefinition> tables,
                [NotNull] IEnumerable<SqlType> types)
            {
                Contract.Requires(schemasByID != null);
                Contract.Requires(programDefinitionsByName != null);
                Contract.Requires(tablesByName != null);
                Contract.Requires(typesByName != null);
                Contract.Requires(schemas != null);
                Contract.Requires(programDefinitions != null);
                Contract.Requires(tables != null);
                Contract.Requires(types != null);

                _guid = guid;
                _schemasByID = schemasByID;
                _programDefinitionsByName = programDefinitionsByName;
                _tablesByName = tablesByName;
                _typesByName = typesByName;
                _schemas = schemas;
                _programDefinitions = programDefinitions;
                _tables = tables;
                _types = types;
            }

            /// <summary>
            /// Gets or adds a schema.
            /// </summary>
            /// <param name="schemasByID">The schemas by identifier.</param>
            /// <param name="programDefinitionsByName">Name of the program definitions by.</param>
            /// <param name="tablesByName">Name of the tables by.</param>
            /// <param name="typesByName">Name of the types by.</param>
            /// <returns>WebApplications.Utilities.Database.Schema.DatabaseSchema.Schema.</returns>
            [NotNull]
            protected internal static Schema GetOrAdd(
                [NotNull] IReadOnlyDictionary<int, SqlSchema> schemasByID,
                [NotNull] IReadOnlyDictionary<string, SqlProgramDefinition> programDefinitionsByName,
                [NotNull] IReadOnlyDictionary<string, SqlTableDefinition> tablesByName,
                [NotNull] IReadOnlyDictionary<string, SqlType> typesByName)
            {
                Contract.Requires(schemasByID != null);
                Contract.Requires(programDefinitionsByName != null);
                Contract.Requires(tablesByName != null);
                Contract.Requires(typesByName != null);

                // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute
                SqlSchema[] schemas = schemasByID.Values.OrderBy(s => s.FullName).ToArray();
                SqlProgramDefinition[] programDefinitions =
                    programDefinitionsByName.Values.Distinct().OrderBy(p => p.FullName).ToArray();
                SqlTableDefinition[] tables = tablesByName.Values.Distinct().OrderBy(t => t.FullName).ToArray();
                SqlType[] types = typesByName.Values.Distinct().OrderBy(t => t.FullName).ToArray();
                // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute

                List<byte> guidBytes = new List<byte>(16);
                unchecked
                {
                    guidBytes.AddRange(
                        BitConverter.GetBytes(
                            schemas.Aggregate(0, (hash, schema) => (397 * hash) ^ schema.GetHashCode())));
                    guidBytes.AddRange(
                        BitConverter.GetBytes(types.Aggregate(0, (hash, type) => (397 * hash) ^ type.GetHashCode())));
                    guidBytes.AddRange(
                        BitConverter.GetBytes(
                            programDefinitions.Aggregate(
                                0,
                                (hash, programDefinition) => (397 * hash) ^ programDefinition.GetHashCode())));
                    guidBytes.AddRange(
                        BitConverter.GetBytes(
                            tables.Aggregate(0, (hash, tableDefinition) => (397 * hash) ^ tableDefinition.GetHashCode())));
                }

                return _schemasByGuid.GetOrAdd(
                    new Guid(guidBytes.ToArray()),
                    g =>
                        new Schema(
                        g,
                        schemasByID,
                        programDefinitionsByName,
                        tablesByName,
                        typesByName,
                        schemas,
                        programDefinitions,
                        tables,
                        types));
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
                return other != null && _guid.Equals(other.Guid);
            }

            /// <summary>
            /// Returns a boolean indicating if the passed in <see cref="Schema"/> is equal to this instance.
            /// </summary>
            /// <param name="other">The other <see cref="Schema"/>.</param>
            /// <returns>System.Boolean.</returns>
            public bool Equals([CanBeNull] ISchema other)
            {
                if (ReferenceEquals(null, other)) return false;
                return ReferenceEquals(this, other) || _guid.Equals(other.Guid);
            }

            /// <summary>
            /// Returns a boolean indicating if the passed in <see cref="Schema"/> is equal to this instance.
            /// </summary>
            /// <param name="other">The other <see cref="Schema"/>.</param>
            /// <returns>System.Boolean.</returns>
            public bool Equals([CanBeNull] Schema other)
            {
                if (ReferenceEquals(null, other)) return false;
                return ReferenceEquals(this, other) || _guid.Equals(other._guid);
            }

            /// <summary>
            /// Gets the hash code.
            /// </summary>
            /// <returns>System.Int32.</returns>
            public override int GetHashCode()
            {
                return _guid.GetHashCode();
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
                return !ReferenceEquals(right, null) && left._guid.Equals(right._guid);
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
                return ReferenceEquals(right, null) || !left._guid.Equals(right._guid);
            }
            #endregion
        }
    }
}