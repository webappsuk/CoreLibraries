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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq;
using JetBrains.Annotations;
using WebApplications.Utilities.Database.Exceptions;
using WebApplications.Utilities.Logging;

namespace WebApplications.Utilities.Database.Schema
{
    /// <summary>
    ///   Holds information about a database schema
    /// </summary>
    public class DatabaseSchema : IEqualityComparer<DatabaseSchema>, IEquatable<DatabaseSchema>
    {
        /// <summary>
        ///   Holds schemas against connections strings.
        /// </summary>
        private static readonly ConcurrentDictionary<string, DatabaseSchema> _schemas =
            new ConcurrentDictionary<string, DatabaseSchema>();

        private static int _tempCounter = 1;

        /// <summary>
        ///   The connection string which was used to generate schema initially.
        /// </summary>
        [NotNull] private readonly string _connectionString;

        /// <summary>
        ///   A lock object, ensures only one thread loads the schema.
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        ///   Holds all the program definitions (<see cref="SqlProgramDefinition"/>) for the schema.
        /// </summary>
        private readonly Dictionary<string, SqlProgramDefinition> _programDefinitions =
            new Dictionary<string, SqlProgramDefinition>();

        /// <summary>
        ///   Holds all the table and view definitions (<see cref="SqlTableDefinition"/>) for the schema.
        /// </summary>
        private readonly Dictionary<string, SqlTableDefinition> _tables = new Dictionary<string, SqlTableDefinition>();

        /// <summary>
        ///   Holds all the types for the schema, which are stored with the <see cref="SqlType.FullName">full
        ///   name</see> as the key and the <see cref="SqlType"/> as the value.
        /// </summary>
        private readonly Dictionary<string, SqlType> _types = new Dictionary<string, SqlType>();

        /// <summary>
        ///   Holds any exceptions that are thrown when loading the schema.
        /// </summary>
        private DatabaseSchemaException _error;

        /// <summary>
        ///   Hash code cache.
        /// </summary>
        private int _hashCode;

        /// <summary>
        ///   A <see cref="bool"/> flag indicating whether the schema has been loaded.
        /// </summary>
        private bool _loaded;

        /// <summary>
        ///   Initializes a new instance of the <see cref="DatabaseSchema"/> class.
        /// </summary>
        /// <param name="connectionString">
        ///   The connection string to the database to load the schema of.
        /// </param>
        private DatabaseSchema([NotNull] string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        ///   Gets the SQL schemas that were loaded from the database.
        /// </summary>
        /// <value>
        ///   An enumerable containing the schema names in ascended order.
        /// </value>
        [NotNull]
        [UsedImplicitly]
        public IEnumerable<string> Schemas { get; private set; }

        /// <summary>
        ///   Gets the SQL types from the schema.
        /// </summary>
        /// <value>
        ///   The <see cref="SqlType">type</see>.
        /// </value>
        [NotNull]
        [UsedImplicitly]
        public IEnumerable<SqlType> Types { get; private set; }

        /// <summary>
        ///   Gets the program definitions.
        /// </summary>
        /// <value>The program definitions.</value>
        [NotNull]
        [UsedImplicitly]
        public IEnumerable<SqlProgramDefinition> ProgramDefinitions { get; private set; }

        /// <summary>
        ///   Gets the table and view definitions.
        /// </summary>
        /// <value>The table and view definitions.</value>
        [NotNull]
        [UsedImplicitly]
        public IEnumerable<SqlTableDefinition> Tables { get; private set; }

        #region IEqualityComparer<DatabaseSchema> Members
        /// <summary>
        ///   Determines whether the specified objects are equal.
        /// </summary>
        /// <returns>
        ///   Returns <see langword="true"/> if the specified objects are equal; otherwise returns <see langword="false"/>.
        /// </returns>
        /// <param name="x">The first <see cref="DatabaseSchema"/> to compare.</param>
        /// <param name="y">The second <see cref="DatabaseSchema"/> to compare.</param>
        public bool Equals([CanBeNull] DatabaseSchema x, [CanBeNull] DatabaseSchema y)
        {
            if (x == null)
                return y == null;
            return y != null && x.Equals(y);
        }

        /// <summary>
        ///   Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj">
        ///   The <see cref="object"/> for which a hash code is to be returned.
        /// </param>
        /// <returns>A hash code for the specified object.</returns>
        /// <exception cref="ArgumentNullException">
        ///   The type of <paramref name="obj"/> is a reference type and is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        ///   There is a <see cref="System.Diagnostics.Contracts">contract</see> specifying
        ///   <paramref name="obj"/> cannot be <see langword="null"/>.
        /// </remarks>
        public int GetHashCode([NotNull] DatabaseSchema obj)
        {
            Contract.Requires(obj != null, Resources.DatabaseSchema_GetHashCode_ObjCanNotBeNull);

            return obj._hashCode;
        }
        #endregion

        #region IEquatable<DatabaseSchema> Members
        /// <summary>
        ///   Indicates whether the current <see cref="DatabaseSchema"/> is equal to another <see cref="DatabaseSchema"/>.
        /// </summary>
        /// <param name="other">A <see cref="DatabaseSchema"/> to compare with this instance.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the current instance is equal to the <paramref name="other"/> provided;
        ///   otherwise returns <see langword="false"/>.
        /// </returns>
        /// <remarks>
        ///   There is a <see cref="System.Diagnostics.Contracts">contract</see> specifying
        ///   <paramref name="other"/> cannot be <see langword="null"/>.
        /// </remarks>
        public bool Equals([NotNull] DatabaseSchema other)
        {
            Contract.Requires(other != null, "Parameter 'other' can not be null");

            return _hashCode == other._hashCode;
        }
        #endregion

        /// <summary>
        ///   Gets or adds a schema for the given connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>The added/retrieved schema.</returns>
        /// <remarks>
        ///   There is a <see cref="System.Diagnostics.Contracts">contract</see> specifying
        ///   <paramref name="connectionString"/> cannot be <see langword="null"/>.
        /// </remarks>
        /// <exception cref="DatabaseSchemaException">
        ///   An error occurred when loading the schema.
        /// </exception>
        [NotNull]
        public static DatabaseSchema GetOrAdd([NotNull] string connectionString)
        {
            Contract.Requires(connectionString != null, Resources.DatabaseSchema_GetOrAdd_ConnectionStringCanNotBeNull);

            bool hasChanged;
            return GetOrAdd(connectionString, false, out hasChanged);
        }

        /// <summary>
        ///   Gets or adds a schema for the given connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="forceReload">
        ///   If set to <see langword="true"/> forces the schema to <see cref="DatabaseSchema.Load">reload</see>.
        /// </param>
        /// <param name="hasChanged">
        ///   <para>Will output <see langword="true"/> if the schema has changed.</para>
        ///   <para>Can only be <see langword="true"/> if <see paramref="forceReload"/> is also <see langword="true"/>.</para>
        /// </param>
        /// <returns>The added/retrieved schema.</returns>
        /// <remarks>
        ///   There is a <see cref="System.Diagnostics.Contracts">contract</see> specifying
        ///   <paramref name="connectionString"/> cannot be <see langword="null"/>.
        /// </remarks>
        /// <exception cref="DatabaseSchemaException">
        ///   An error occurred when loading the schema.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="connectionString"/> is <see langword="null"/>.
        /// </exception>
        [NotNull]
        public static DatabaseSchema GetOrAdd([NotNull] string connectionString, bool forceReload, out bool hasChanged)
        {
            Contract.Requires(connectionString != null, Resources.DatabaseSchema_GetOrAdd_ConnectionStringCanNotBeNull);
            DatabaseSchema databaseSchema;
            if (forceReload)
            {
                // If we're doing a reload, load a new connection first
                databaseSchema = new DatabaseSchema(connectionString);

                // Get the new or duplicate schema
                DatabaseSchema duplicate = databaseSchema.Load() ?? databaseSchema;

                // And update regardless
                bool hc = false;
                _schemas.AddOrUpdate(
                    connectionString,
                    cs => duplicate,
                    (cs, ds) =>
                        {
                            // ReSharper disable PossibleNullReferenceException
                            hc = !ds.Equals(duplicate);
                            // ReSharper restore PossibleNullReferenceException
                            return duplicate;
                        });
                hasChanged = hc;
            }
            else
            {
                hasChanged = false;
                // Load the schema or add a new one
                databaseSchema = _schemas.GetOrAdd(connectionString, cs => new DatabaseSchema(connectionString));

                // Only load if not already loaded
                if (!databaseSchema._loaded)
                {
                    DatabaseSchema duplicate = databaseSchema.Load();

                    // If we have a duplicate, set our connection string to it.
                    if (duplicate != null)
                    {
                        _schemas.AddOrUpdate(connectionString, cs => duplicate, (cs, ds) => duplicate);
                        databaseSchema = duplicate;
                    }
                }
            }

            // If there was an error for this connection string, throw it.
            if (databaseSchema._error != null)
                throw databaseSchema._error;

            return databaseSchema;
        }

        /// <summary>
        ///   Loads the schema for this instance.
        /// </summary>
        /// <returns>
        ///   Returns the original schema if this instance is a duplicate;
        ///   otherwise returns the default value of <see langword="null"/>.
        /// </returns>
        /// <exception cref="DatabaseSchemaException">
        ///   <para>Could not parse the version information for the connection.</para>
        ///   <para>-or-</para>
        ///   <para>SQL database versions less than version 9 are not supported.</para>
        ///   <para>-or-</para>
        ///   <para>Could not retrieve any schemas from database.</para>
        ///   <para>-or-</para>
        ///   <para>One of the schema could not be found.</para>
        ///   <para>-or-</para>
        ///   <para>One of the types was not found.</para>
        ///   <para>-or-</para>
        ///   <para>Ran out of results whilst retrieving programs/tables/views.</para>
        /// </exception>
        [CanBeNull]
        private DatabaseSchema Load()
        {
            lock (_lock)
            {
                // Check another thread hasn't loaded us.
                if (_loaded)
                    return null;

                // Reset the error, we may be trying again.
                _error = null;
                _types.Clear();
                _programDefinitions.Clear();
                _tables.Clear();

                // Create ID based dictionaries during load
                Dictionary<int, string> sqlSchemas = new Dictionary<int, string>();
                Dictionary<int, SqlType> types = new Dictionary<int, SqlType>();

                try
                {
                    // Open a connection
                    using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                    {
                        sqlConnection.Open();
                        Version version;
                        if (!Version.TryParse(sqlConnection.ServerVersion, out version))
                            throw new DatabaseSchemaException(
                                Resources.DatabaseSchema_Load_CouldNotParseVersionInformation);

                        if (version.Major < 9)
                            throw new DatabaseSchemaException(Resources.DatabaseSchema_Load_VersionNotSupported,
                                                              version);

                        string sql = version.Major == 9 ? SQLResources.RetrieveSchema9 : SQLResources.RetrieveSchema10;

                        // Create the command first, as we will reuse on each connection.
                        using (
                            SqlCommand command = new SqlCommand(sql, sqlConnection) {CommandType = CommandType.Text})
                        {
                            // Execute command
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                /*
                                 * Load SQL Schemas
                                 */
                                while (reader.Read())
                                {
                                    int schemaId = reader.GetInt32(0);
                                    string name = reader.GetString(1).ToLower();
                                    sqlSchemas.Add(schemaId, name);
                                }

                                if (sqlSchemas.Count < 1)
                                    throw new DatabaseSchemaException(
                                        Resources.DatabaseSchema_Load_CouldNotRetrieveSchemas);

                                // Order schemas
                                Schemas = sqlSchemas.Values.OrderBy(s => s).ToList();

                                /*
                                 * Load types
                                 */
                                if (!reader.NextResult())
                                    throw new DatabaseSchemaException(
                                        Resources.DatabaseSchema_Load_RanOutOfResultsRetrievingTypes);

                                while (reader.Read())
                                {
                                    int schemaId = reader.GetInt32(0);
                                    string schema;
                                    if (!sqlSchemas.TryGetValue(schemaId, out schema) || (schema == null))
                                        throw new DatabaseSchemaException(
                                            Resources.DatabaseSchema_Load_CouldNotFindSchema,
                                            schemaId);
                                    int id = reader.GetInt32(1);
                                    string name = reader.GetString(2).ToLower();
                                    SqlType baseType;
                                    if (reader.IsDBNull(3))
                                        baseType = null;
                                    else
                                    {
                                        // NB SQL returns types in dependency order
                                        // i.e. base types are always seen first, so this code is much easier.
                                        int baseId = reader.GetInt32(3);
                                        if (!types.TryGetValue(baseId, out baseType))
                                            baseType = null;
                                    }

                                    short maxLength = reader.GetInt16(4);
                                    byte precision = reader.GetByte(5);
                                    byte scale = reader.GetByte(6);
                                    bool isNullable = reader.GetBoolean(7);
                                    bool isUserDefined = reader.GetBoolean(8);
                                    bool isCLR = reader.GetBoolean(9);
                                    bool isTable = reader.GetBoolean(10);

                                    // Create type
                                    SqlType type = isTable
                                                       ? new SqlTableType(
                                                             baseType,
                                                             schema,
                                                             name,
                                                             new SqlTypeSize(maxLength, precision, scale),
                                                             isNullable,
                                                             isUserDefined,
                                                             isCLR)
                                                       : new SqlType(
                                                             baseType,
                                                             schema,
                                                             name,
                                                             new SqlTypeSize(maxLength, precision, scale),
                                                             isNullable,
                                                             isUserDefined,
                                                             isCLR);

                                    // Add to dictionary
                                    _types.Add(type.FullName, type);
                                    types.Add(id, type);
                                }

                                if (_types.Count < 1)
                                    throw new DatabaseSchemaException(Resources.DatabaseSchema_Load_CouldNotRetrieveTypes);

                                // Order types
                                // ReSharper disable PossibleNullReferenceException
                                Types = _types.Values.OrderBy(t => t.FullName).ToList();
                                // ReSharper restore PossibleNullReferenceException

                                /*
                                 * Load program definitions
                                 */
                                if (!reader.NextResult())
                                    throw new DatabaseSchemaException(Resources.DatabaseSchema_Load_RanOutOfResultsRetrievingPrograms);

                                SqlProgramDefinition lastProgramDefinition = null;
                                while (reader.Read())
                                {
                                    SqlObjectType type;
                                    string typeString = reader.GetString(0) ?? string.Empty;
                                    if (!ExtendedEnum<SqlObjectType>.TryParse(typeString, true, out type))
                                        throw new DatabaseSchemaException(
                                            Resources.DatabaseSchema_Load_CouldNotFindTypeWhenLoadingPrograms,
                                            typeString);

                                    int schemaId = reader.GetInt32(1);
                                    string schema;
                                    if (!sqlSchemas.TryGetValue(schemaId, out schema))
                                        throw new DatabaseSchemaException(
                                            Resources.DatabaseSchema_Load_CouldNotFindSchemaWhenLoadingPrograms,
                                            schemaId);
                                    string name = reader.GetString(2).ToLower();
                                    string fullName = string.Format("{0}.{1}", schema, name);

                                    // Now create or find program definition
                                    SqlProgramDefinition programDefinition;
                                    if ((lastProgramDefinition != null) && (lastProgramDefinition.FullName == fullName))
                                        programDefinition = lastProgramDefinition;
                                    else if (!_programDefinitions.TryGetValue(fullName, out programDefinition))
                                    {
                                        programDefinition = new SqlProgramDefinition(type, schema, name);
                                        _programDefinitions.Add(fullName, programDefinition);
                                    }
                                    else if (programDefinition.Type != type)
                                        throw new DatabaseSchemaException(
                                            Resources.DatabaseSchema_Load_InconsistentType,
                                            fullName);
                                    lastProgramDefinition = programDefinition;

                                    // If we have a null ordinal, we have no parameters.
                                    if (reader.IsDBNull(3))
                                        continue;

                                    int ordinal = reader.GetInt32(3);
                                    string parameterName = reader.GetString(4).ToLower();
                                    int typeId = reader.GetInt32(5);
                                    SqlType pType;
                                    if (!types.TryGetValue(typeId, out pType) || (pType == null))
                                    {
                                        throw new DatabaseSchemaException(
                                            Resources.DatabaseSchema_Load_ParameterTypeNotFound,
                                            parameterName,
                                            typeId,
                                            fullName);
                                    }

                                    short maxLength = reader.GetInt16(6);
                                    byte precision = reader.GetByte(7);
                                    byte scale = reader.GetByte(8);
                                    bool isOutput = reader.GetBoolean(9);
                                    bool isReadonly = reader.GetBoolean(10);
                                    ParameterDirection direction;
                                    if (!isOutput)
                                        direction = ParameterDirection.Input;
                                    else if (parameterName == string.Empty)
                                        direction = ParameterDirection.ReturnValue;
                                    else
                                        direction = ParameterDirection.InputOutput;

                                    // Add parameter to program definition
                                    programDefinition.AddParameter(
                                        new SqlProgramParameter(
                                            ordinal,
                                            parameterName,
                                            pType,
                                            new SqlTypeSize(maxLength, precision, scale),
                                            direction,
                                            isReadonly));
                                }

                                // Order programs
                                // ReSharper disable PossibleNullReferenceException
                                ProgramDefinitions =
                                    _programDefinitions.Select(kvp => kvp.Value).OrderBy(p => p.FullName).
                                        ToList();
                                // ReSharper restore PossibleNullReferenceException

                                /*
                                 * Load tables and views
                                 */
                                if (!reader.NextResult())
                                    throw new DatabaseSchemaException(Resources.DatabaseSchema_Load_RanOutOfTablesAndViews);

                                Dictionary<int, SqlTableDefinition> tableTypeTables =
                                    new Dictionary<int, SqlTableDefinition>();
                                SqlTableDefinition lastTableDefinition = null;
                                while (reader.Read())
                                {
                                    SqlObjectType type;
                                    string typeString = reader.GetString(0) ?? string.Empty;
                                    if (!ExtendedEnum<SqlObjectType>.TryParse(typeString, true, out type))
                                        throw new DatabaseSchemaException(
                                            Resources.DatabaseSchema_Load_CouldNotFindObjectType,
                                            typeString);

                                    int schemaId = reader.GetInt32(1);
                                    string schema;
                                    if (!sqlSchemas.TryGetValue(schemaId, out schema))
                                        throw new DatabaseSchemaException(
                                            Resources.DatabaseSchema_Load_CouldNotFindSchemaLoadingTablesAndViews,
                                            schemaId);
                                    string name = reader.GetString(2).ToLower();
                                    string fullName = string.Format("{0}.{1}", schema, name);

                                    // Now create or find table/view definition
                                    SqlTableDefinition tableDefinition;
                                    if ((lastTableDefinition != null) && (lastTableDefinition.FullName == fullName))
                                        tableDefinition = lastTableDefinition;
                                    else if (!_tables.TryGetValue(fullName, out tableDefinition))
                                    {
                                        tableDefinition = new SqlTableDefinition(type, schema, name);
                                        _tables.Add(fullName, tableDefinition);
                                    }
                                    else if (tableDefinition.Type != type)
                                    {
                                        throw new DatabaseSchemaException(
                                            Resources.DatabaseSchema_Load_InconsistentTypeLoadingTablesAndViews,
                                            fullName);
                                    }
                                    lastTableDefinition = tableDefinition;

                                    int ordinal = reader.GetInt32(3);
                                    string columnName = reader.GetString(4).ToLower();
                                    int typeId = reader.GetInt32(5);
                                    SqlType cType;
                                    if (!types.TryGetValue(typeId, out cType) || (cType == null))
                                    {
                                        throw new DatabaseSchemaException(
                                            Resources.DatabaseSchema_Load_ColumnTypeNotFound,
                                            columnName,
                                            typeId,
                                            fullName);
                                    }

                                    short maxLength = reader.GetInt16(6);
                                    byte precision = reader.GetByte(7);
                                    byte scale = reader.GetByte(8);
                                    bool isNullable = reader.GetBoolean(9);

                                    // Add parameter to program definition
                                    tableDefinition.AddColumn(
                                        new SqlColumn(
                                            ordinal - 1,
                                            columnName,
                                            cType,
                                            new SqlTypeSize(maxLength, precision, scale),
                                            isNullable));

                                    if (!reader.IsDBNull(10))
                                    {
                                        // This is a table type table
                                        int tableTypeId = reader.GetInt32(10);
                                        if (!tableTypeTables.ContainsKey(tableTypeId))
                                        {
                                            tableTypeTables.Add(tableTypeId, tableDefinition);
                                        }
                                    }
                                }

                                // Order tables
                                // ReSharper disable PossibleNullReferenceException
                                Tables = _tables.Select(kvp => kvp.Value).OrderBy(t => t.FullName).ToList();
                                // ReSharper restore PossibleNullReferenceException

                                foreach (KeyValuePair<int, SqlTableDefinition> kvp in tableTypeTables)
                                {
                                    SqlType tType;
                                    if (!types.TryGetValue(kvp.Key, out tType))
                                    {
                                        throw new DatabaseSchemaException(
                                            Resources.DatabaseSchema_Load_TableTypeNotFound,
                                            kvp.Key,
                                            kvp.Value.FullName);
                                    }
                                    SqlTableType tableType = tType as SqlTableType;
                                    if (tableType == null)
                                    {
                                        throw new DatabaseSchemaException(
                                            Resources.DatabaseSchema_Load_TypeNotTableType,
                                            kvp.Key,
                                            kvp.Value.FullName);
                                    }
                                    // ReSharper disable AssignNullToNotNullAttribute
                                    tableType.TableDefinition = kvp.Value;
                                    // ReSharper restore AssignNullToNotNullAttribute
                                }
                            }
                        }
                    }

                    _loaded = true;
                }
                    // In the event of an error we don't set the loaded flag - this allows retries.
                catch (DatabaseSchemaException databaseSchemaException)
                {
                    _error = databaseSchemaException;
                    _loaded = false;
                    Schemas = Enumerable.Empty<string>();
                    Types = Enumerable.Empty<SqlType>();
                    ProgramDefinitions = Enumerable.Empty<SqlProgramDefinition>();
                    Tables = Enumerable.Empty<SqlTableDefinition>();
                }
                catch (Exception exception)
                {
                    _error = new DatabaseSchemaException(
                        exception,
                        LoggingLevel.Critical,
                        Resources.DatabaseSchema_Load_ErrorOccurred);
                    _loaded = false;
                    Schemas = Enumerable.Empty<string>();
                    Types = Enumerable.Empty<SqlType>();
                    ProgramDefinitions = Enumerable.Empty<SqlProgramDefinition>();
                    Tables = Enumerable.Empty<SqlTableDefinition>();
                }

                // ReSharper disable PossibleNullReferenceException
                //_hashCode = Schemas.Aggregate(
                //    _error != null ? _error.GetHashCode() : 0, (h, s) => h ^ s.GetHashCode());
                //_hashCode = Types.Aggregate(_hashCode, (h, t) => h ^ t.GetHashCode(t));
                //_hashCode = ProgramDefinitions.Aggregate(_hashCode, (h, p) => h ^ p.GetHashCode(p));
                //_hashCode = Tables.Aggregate(_hashCode, (h, t) => h ^ t.GetHashCode(t));

                _hashCode = _tempCounter++;

                // Look for duplicate and return if present.
                return
                    _schemas.Values.FirstOrDefault(
                        schema =>
                        (schema._connectionString != _connectionString) && (Equals(schema)));
                // ReSharper restore PossibleNullReferenceException
            }
        }

        /// <summary>
        ///   Tries to get the <see cref="SqlType"/> with the type's <see cref="SqlType.FullName">full name</see>.
        /// </summary>
        /// <param name="fullName">
        ///   The <see cref="SqlType.FullName">full name</see> of the type to retrieve.
        /// </param>
        /// <param name="sqlType">The SQL type.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if a corresponding type was found; otherwise returns <see langword="false"/>.
        /// </returns>
        /// <remarks>
        ///   There is a <see cref="System.Diagnostics.Contracts.Contract"/> specifying that
        ///   <paramref name="fullName"/> cannot be <see langword="null"/>.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="fullName"/> was <see langword="null"/>.
        /// </exception>
        [UsedImplicitly]
        public bool TryGetType([NotNull] string fullName, [NotNull] out SqlType sqlType)
        {
            Contract.Requires(fullName != null, Resources.DatabaseSchema_TypeGetType_FullNameCanNotBeNull);

            return _types.TryGetValue(fullName, out sqlType);
        }

        /// <summary>
        ///   Reloads all the schemas.
        /// </summary>
        /// <returns>
        ///   Returns <see langword="true"/> if there were any changes detected during re-load;
        ///   otherwise returns <see langword="false"/>.
        /// </returns>
        [UsedImplicitly]
        public static bool ReloadAll()
        {
            IEnumerable<DatabaseSchemaException> errors;
            return ReloadAll(out errors);
        }

        /// <summary>
        ///   Reloads all the schemas.
        /// </summary>
        /// <param name="errors">Any errors that occurred during schema loading.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if there were any changes detected during re-load;
        ///   otherwise returns <see langword="false"/>.
        /// </returns>
        [UsedImplicitly]
        public static bool ReloadAll([CanBeNull] out IEnumerable<DatabaseSchemaException> errors)
        {
            List<DatabaseSchemaException> errorsList = new List<DatabaseSchemaException>();
            errors = errorsList;
            bool hasChanges = false;
            foreach (string schema in _schemas.Keys)
            {
                bool hc = false;
                try
                {
                    // ReSharper disable AssignNullToNotNullAttribute
                    DatabaseSchema s = GetOrAdd(schema, true, out hc);
                    // ReSharper restore AssignNullToNotNullAttribute
                    if (s._error != null)
                        errorsList.Add(s._error);
                }
                catch (DatabaseSchemaException error)
                {
                    errorsList.Add(error);
                }
                hasChanges |= hc;
            }

            return hasChanges;
        }
    }
}