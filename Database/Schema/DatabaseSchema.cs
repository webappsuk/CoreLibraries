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

using NodaTime;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Database.Exceptions;
using WebApplications.Utilities.Logging;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.Database.Schema
{
    /// <summary>
    ///   Holds information about a database schema
    /// </summary>
    [PublicAPI]
    public partial class DatabaseSchema : ISchema
    {
        /// <summary>
        ///   Holds schemas against connections strings.
        /// </summary>
        [NotNull]
        private static readonly ConcurrentDictionary<string, DatabaseSchema> _databaseSchemas =
            new ConcurrentDictionary<string, DatabaseSchema>();

        /// <summary>
        ///  Holds data for a program definition as it's read.
        /// </summary>
        private class ProgramDefinitionData
        {
            public readonly SqlObjectType Type;
            public readonly int SchemaID;

            [NotNull]
            public readonly string Name;

            [CanBeNull]
            public readonly SqlProgramParameter Parameter;

            /// <summary>
            /// Initializes a new instance of the <see cref="ProgramDefinitionData" /> class.
            /// </summary>
            /// <param name="type">The type.</param>
            /// <param name="schemaID">The schema identifier.</param>
            /// <param name="name">The name.</param>
            public ProgramDefinitionData(
                SqlObjectType type,
                int schemaID,
                [NotNull] string name)
            {
                if (name == null) throw new ArgumentNullException(nameof(name));

                Type = type;
                SchemaID = schemaID;
                Name = name;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ProgramDefinitionData" /> class.
            /// </summary>
            /// <param name="type">The type.</param>
            /// <param name="schemaID">The schema identifier.</param>
            /// <param name="name">The name.</param>
            /// <param name="ordinal">The ordinal.</param>
            /// <param name="parameterName">Name of the parameter.</param>
            /// <param name="parameterType">Type of the parameter.</param>
            /// <param name="parameterSize">Size of the parameter.</param>
            /// <param name="parameterDirection">The parameter direction.</param>
            /// <param name="isReadonly">if set to <see langword="true" /> [is readonly].</param>
            public ProgramDefinitionData(
                SqlObjectType type,
                int schemaID,
                [NotNull] string name,
                int ordinal,
                [NotNull] string parameterName,
                [NotNull] SqlType parameterType,
                SqlTypeSize parameterSize,
                ParameterDirection parameterDirection,
                bool isReadonly)
            {
                if (name == null) throw new ArgumentNullException(nameof(name));
                if (parameterName == null) throw new ArgumentNullException(nameof(parameterName));
                if (parameterType == null) throw new ArgumentNullException(nameof(parameterType));

                Type = type;
                SchemaID = schemaID;
                Name = name;
                Parameter = new SqlProgramParameter(
                    ordinal,
                    parameterName,
                    parameterType,
                    parameterSize,
                    parameterDirection,
                    isReadonly);
            }

            /// <summary>
            /// Returns a <see cref="System.String" /> that represents this instance.
            /// </summary>
            /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
            public override string ToString()
            {
                return string.Format("{0}.{1}", Name, SchemaID);
            }
        }

        /// <summary>
        ///  Holds data for a table definition as it's read.
        /// </summary>
        private class TableDefinitionData
        {
            public readonly SqlObjectType Type;
            public readonly int SchemaID;

            [NotNull]
            public readonly string Name;

            [NotNull]
            public readonly SqlColumn Column;

            [CanBeNull]
            public readonly int? TableTypeID;

            /// <summary>
            /// Initializes a new instance of the <see cref="TableDefinitionData" /> class.
            /// </summary>
            /// <param name="type">The type.</param>
            /// <param name="schemaID">The schema identifier.</param>
            /// <param name="name">The name.</param>
            /// <param name="ordinal">The ordinal.</param>
            /// <param name="columnName">Name of the column.</param>
            /// <param name="columnType">Type of the column.</param>
            /// <param name="columnSize">Size of the column.</param>
            /// <param name="isNullable">if set to <see langword="true" /> [is nullable].</param>
            /// <param name="tableTypeID">The ID of the associated <see cref="SqlType"/> if this table defines a <see cref="SqlType"/>.</param>
            public TableDefinitionData(
                SqlObjectType type,
                int schemaID,
                [NotNull] string name,
                int ordinal,
                [NotNull] string columnName,
                [NotNull] SqlType columnType,
                SqlTypeSize columnSize,
                bool isNullable,
                int? tableTypeID)
            {
                if (name == null) throw new ArgumentNullException(nameof(name));
                if (columnName == null) throw new ArgumentNullException(nameof(columnName));
                if (columnType == null) throw new ArgumentNullException(nameof(columnType));

                Type = type;
                SchemaID = schemaID;
                Name = name;
                Column = new SqlColumn(ordinal, columnName, columnType, columnSize, isNullable);
                TableTypeID = tableTypeID;
            }

            /// <summary>
            /// Returns a <see cref="System.String" /> that represents this instance.
            /// </summary>
            /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
            public override string ToString()
            {
                return string.Format("{0}.{1}", Name, SchemaID);
            }
        }

        /// <summary>
        /// Groups last loaded schema with when it was loaded and any errors that occured during loading, this makes it atomic.
        /// </summary>
        private class CurrentSchema
        {
            [CanBeNull]
            public readonly Schema Schema;

            public readonly Instant Loaded;

            [CanBeNull]
            public readonly ExceptionDispatchInfo ExceptionDispatchInfo;

            public CurrentSchema([NotNull] Schema schema)
            {
                Loaded = TimeHelpers.Clock.Now;
                Schema = schema;
            }

            public CurrentSchema([NotNull] ExceptionDispatchInfo exceptionDispatchInfo)
            {
                Loaded = TimeHelpers.Clock.Now;
                ExceptionDispatchInfo = exceptionDispatchInfo;
            }
        }

        /// <summary>
        ///   The connection string which was used to generate schema initially.
        /// </summary>
        [NotNull]
        public readonly string ConnectionString;

        /// <summary>
        /// The loading lock.
        /// </summary>
        [NotNull]
        private readonly AsyncLock _lock = new AsyncLock();

        /// <summary>
        /// The current schema, when it was loaded, and any error.
        /// </summary>
        private CurrentSchema _current;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseSchema" /> class.
        /// </summary>
        /// <param name="connectionString">The connection string to the database to load the schema of.</param>
        private DatabaseSchema([NotNull] string connectionString)
        {
            ConnectionString = connectionString;
        }

        /// <summary>
        /// Gets the current complete and immutable schema.
        /// </summary>
        /// <value>The current <see cref="Schema"/>.</value>
        [NotNull]
        public Schema Current
        {
            get
            {
                CurrentSchema current = _current;
                if (current == null) throw new InvalidOperationException();

                if (current.ExceptionDispatchInfo != null)
                    current.ExceptionDispatchInfo.Throw();

                Debug.Assert(current.Schema != null);
                return current.Schema;
            }
        }

        /// <summary>
        /// When the <see cref="Current"/> <see cref="Schema" /> was loaded.
        /// </summary>
        /// <value>The loaded <see cref="Instant"/>.</value>
        public Instant Loaded
        {
            get
            {
                CurrentSchema current = _current;
                if (current == null) throw new InvalidOperationException();

                if (current.ExceptionDispatchInfo != null)
                    current.ExceptionDispatchInfo.Throw();

                return current.Loaded;
            }
        }

        #region Pass throughs
        /// <summary>
        /// Holds all the SQL schemas (<see cref="SqlSchema"/>, using the <see cref="SqlSchema.ID"/> as the key.
        /// </summary>
        public IReadOnlyDictionary<int, SqlSchema> SchemasByID
        {
            get { return Current.SchemasByID; }
        }

        /// <summary>
        ///   Holds all the program definitions (<see cref="SqlProgramDefinition"/>) for the schema, which are stored with the <see cref="T:WebApplications.Utilities.Database.Schema.SqlProgramDefinition.FullName">full
        ///   name</see> and <see cref="SqlProgramDefinition.Name">name</see> as the keys and the <see cref="SqlType"/> as the value.
        /// </summary>
        public IReadOnlyDictionary<string, SqlProgramDefinition> ProgramDefinitionsByName
        {
            get { return Current.ProgramDefinitionsByName; }
        }

        /// <summary>
        ///   Holds all the table and view definitions (<see cref="SqlTableDefinition"/>) for the schema.
        /// </summary>
        public IReadOnlyDictionary<string, SqlTableDefinition> TablesByName
        {
            get { return Current.TablesByName; }
        }

        /// <summary>
        ///   Holds all the types for the schema, which are stored with the <see cref="T:WebApplications.Utilities.Database.Schema.SqlType.FullName">full
        ///   name</see> and <see cref="SqlType.Name">name</see> as the keys and the <see cref="SqlType"/> as the value.
        /// </summary>
        public IReadOnlyDictionary<string, SqlType> TypesByName
        {
            get { return Current.TypesByName; }
        }

        /// <summary>
        ///   Gets the SQL schemas that were loaded from the database.
        /// </summary>
        /// <value>
        ///   An enumerable containing the schema names in ascended order.
        /// </value>
        public IEnumerable<SqlSchema> Schemas
        {
            get { return Current.Schemas; }
        }

        /// <summary>
        ///   Gets the SQL types from the schema.
        /// </summary>
        /// <value>
        ///   The <see cref="SqlType">type</see>.
        /// </value>
        public IEnumerable<SqlType> Types
        {
            get { return Current.Types; }
        }

        /// <summary>
        ///   Gets the program definitions.
        /// </summary>
        /// <value>The program definitions.</value>
        public IEnumerable<SqlProgramDefinition> ProgramDefinitions
        {
            get { return Current.ProgramDefinitions; }
        }

        /// <summary>
        ///   Gets the table and view definitions.
        /// </summary>
        /// <value>The table and view definitions.</value>
        public IEnumerable<SqlTableDefinition> Tables
        {
            get { return Current.Tables; }
        }

        /// <summary>
        ///   Unique identity of the schema.
        /// </summary>
        public Guid Guid
        {
            get { return Current.Guid; }
        }
        #endregion

        /// <summary>
        /// Gets or adds a schema for the given connection string.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="forceReload">If set to <see langword="true" /> forces the schema to <see cref="DatabaseSchema.Load">reload</see>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The added/retrieved schema.</returns>
        [NotNull]
        public static Task<DatabaseSchema> GetOrAdd(
            [NotNull] Connection connection,
            bool forceReload = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));

            // ReSharper disable PossibleNullReferenceException
            return _databaseSchemas.GetOrAdd(
                connection.ConnectionString,
                cs => new DatabaseSchema(connection.ConnectionString))
                .Load(forceReload, cancellationToken);
            // ReSharper restore PossibleNullReferenceException
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
        [NotNull]
        private async Task<DatabaseSchema> Load(bool forceReload, CancellationToken cancellationToken)
        {
            Instant requested = TimeHelpers.Clock.Now;
            using (await _lock.LockAsync(cancellationToken).ConfigureAwait(false))
            {
                // Check to see if the currently loaded schema is acceptable.
                CurrentSchema current = _current;

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if ((current != null) &&
                    (!forceReload || (current.Loaded > requested)))
                {
                    // Rethrow load errors.
                    if (current.ExceptionDispatchInfo != null)
                        current.ExceptionDispatchInfo.Throw();

                    Debug.Assert(current.Schema != null);
                    return this;
                }

                // Create dictionaries
                Dictionary<int, SqlSchema> sqlSchemas = new Dictionary<int, SqlSchema>();
                Dictionary<int, SqlType> typesByID = new Dictionary<int, SqlType>();
                Dictionary<string, SqlType> typesByName =
                    new Dictionary<string, SqlType>(StringComparer.InvariantCultureIgnoreCase);
                Dictionary<string, SqlProgramDefinition> programDefinitions =
                    new Dictionary<string, SqlProgramDefinition>(StringComparer.InvariantCultureIgnoreCase);
                Dictionary<string, SqlTableDefinition> tables =
                    new Dictionary<string, SqlTableDefinition>(StringComparer.InvariantCultureIgnoreCase);

                try
                {
                    // Open a connection
                    using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                    {
                        // ReSharper disable once PossibleNullReferenceException
                        await sqlConnection.OpenAsync(cancellationToken).ConfigureAwait(false);

                        Version version;
                        if (!Version.TryParse(sqlConnection.ServerVersion, out version))
                            throw new DatabaseSchemaException(
                                () => Resources.DatabaseSchema_Load_CouldNotParseVersionInformation);
                        Debug.Assert(version != null);

                        if (version.Major < 9)
                            throw new DatabaseSchemaException(
                                () => Resources.DatabaseSchema_Load_VersionNotSupported,
                                version);

                        string sql = version.Major == 9 ? SQLResources.RetrieveSchema9 : SQLResources.RetrieveSchema10;

                        // Create the command first, as we will reuse on each connection.
                        using (
                            SqlCommand command = new SqlCommand(sql, sqlConnection) { CommandType = CommandType.Text })
                            // Execute command
                        using (SqlDataReader reader =
                            // ReSharper disable once PossibleNullReferenceException
                            await
                                command.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken)
                                    .ConfigureAwait(false))
                        {
                            /*
                             * Load SQL Schemas
                             */
                            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                            {
                                SqlSchema sqlSchema = new SqlSchema(reader.GetInt32(0), reader.GetString(1));
                                sqlSchemas.Add(sqlSchema.ID, sqlSchema);
                            }

                            if (sqlSchemas.Count < 1)
                                throw new DatabaseSchemaException(
                                    () => Resources.DatabaseSchema_Load_CouldNotRetrieveSchemas);

                            /*
                             * Load types
                             */
                            if (!(await reader.NextResultAsync(cancellationToken).ConfigureAwait(false)))
                                throw new DatabaseSchemaException(
                                    () => Resources.DatabaseSchema_Load_RanOutOfResultsRetrievingTypes);

                            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                            {
                                int schemaId = reader.GetInt32(0);
                                SqlSchema sqlSchema;
                                if (!sqlSchemas.TryGetValue(schemaId, out sqlSchema) ||
                                    (sqlSchema == null))
                                    throw new DatabaseSchemaException(
                                        () => Resources.DatabaseSchema_Load_CouldNotFindSchema,
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
                                    typesByID.TryGetValue(baseId, out baseType);
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
                                        sqlSchema,
                                        name,
                                        new SqlTypeSize(maxLength, precision, scale),
                                        isNullable,
                                        isUserDefined,
                                        isCLR)
                                    : new SqlType(
                                        baseType,
                                        sqlSchema,
                                        name,
                                        new SqlTypeSize(maxLength, precision, scale),
                                        isNullable,
                                        isUserDefined,
                                        isCLR);

                                // Add to dictionary
                                typesByName.Add(type.FullName, type);
                                if (!typesByName.ContainsKey(type.Name))
                                    typesByName.Add(type.Name, type);
                                typesByID.Add(id, type);
                            }

                            if (typesByName.Count < 1)
                                throw new DatabaseSchemaException(
                                    () => Resources.DatabaseSchema_Load_CouldNotRetrieveTypes);

                            /*
                             * Load program definitions
                             */
                            if (!(await reader.NextResultAsync(cancellationToken).ConfigureAwait(false)))
                                throw new DatabaseSchemaException(
                                    () => Resources.DatabaseSchema_Load_RanOutOfResultsRetrievingPrograms);

                            List<ProgramDefinitionData> programDefinitionData = new List<ProgramDefinitionData>();
                            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                            {
                                SqlObjectType type;
                                string typeString = reader.GetString(0) ?? string.Empty;
                                if (!ExtendedEnum<SqlObjectType>.TryParse(typeString, true, out type))
                                    throw new DatabaseSchemaException(
                                        () => Resources.DatabaseSchema_Load_CouldNotFindTypeWhenLoadingPrograms,
                                        typeString);

                                int schemaId = reader.GetInt32(1);
                                SqlSchema sqlSchema;
                                if (!sqlSchemas.TryGetValue(schemaId, out sqlSchema))
                                    throw new DatabaseSchemaException(
                                        () => Resources.DatabaseSchema_Load_CouldNotFindSchemaWhenLoadingPrograms,
                                        schemaId);
                                string name = reader.GetString(2).ToLower();

                                // If we have a null ordinal, we have no parameters.
                                if (reader.IsDBNull(3))
                                {
                                    programDefinitionData.Add(new ProgramDefinitionData(type, schemaId, name));
                                    continue;
                                }

                                int ordinal = reader.GetInt32(3);
                                string parameterName = reader.GetString(4).ToLower();
                                int typeId = reader.GetInt32(5);
                                SqlType parameterType;
                                if (!typesByID.TryGetValue(typeId, out parameterType) ||
                                    (parameterType == null))
                                    throw new DatabaseSchemaException(
                                        () => Resources.DatabaseSchema_Load_ParameterTypeNotFound,
                                        parameterName,
                                        typeId,
                                        name);

                                short maxLength = reader.GetInt16(6);
                                byte precision = reader.GetByte(7);
                                byte scale = reader.GetByte(8);
                                SqlTypeSize parameterSize = new SqlTypeSize(maxLength, precision, scale);

                                bool isOutput = reader.GetBoolean(9);
                                ParameterDirection parameterDirection;
                                if (!isOutput)
                                    parameterDirection = ParameterDirection.Input;
                                else if (parameterName == string.Empty)
                                    parameterDirection = ParameterDirection.ReturnValue;
                                else
                                    parameterDirection = ParameterDirection.InputOutput;

                                bool parameterIsReadOnly = reader.GetBoolean(10);
                                programDefinitionData.Add(
                                    new ProgramDefinitionData(
                                        type,
                                        schemaId,
                                        name,
                                        ordinal,
                                        parameterName,
                                        parameterType,
                                        parameterSize,
                                        parameterDirection,
                                        parameterIsReadOnly));
                            }

                            // Create unique program definitions.
                            foreach (SqlProgramDefinition program in programDefinitionData
                                // ReSharper disable once PossibleNullReferenceException
                                .GroupBy(d => d.ToString())
                                .Select(
                                    g =>
                                    {
                                        Debug.Assert(g != null);

                                        // Get columns ordered by ordinal.
                                        SqlProgramParameter[] parameters = g
                                            // ReSharper disable once PossibleNullReferenceException
                                            .Select(d => d.Parameter)
                                            .Where(p => p != null)
                                            .OrderBy(p => p.Ordinal)
                                            .ToArray();

                                        ProgramDefinitionData first = g.First();
                                        Debug.Assert(first != null);
                                        Debug.Assert(first.Name != null);

                                        Debug.Assert(sqlSchemas != null);

                                        SqlSchema sqlSchema;
                                        if (!sqlSchemas.TryGetValue(first.SchemaID, out sqlSchema))
                                            throw new DatabaseSchemaException(
                                                () =>
                                                    Resources
                                                    .DatabaseSchema_Load_CouldNotFindSchemaLoadingTablesAndViews,
                                                first.SchemaID);
                                        Debug.Assert(sqlSchema != null);

                                        return new SqlProgramDefinition(
                                            first.Type,
                                            sqlSchema,
                                            first.Name,
                                            parameters);
                                    }))
                            {
                                Debug.Assert(program != null);
                                programDefinitions[program.FullName] = program;

                                if (!programDefinitions.ContainsKey(program.Name))
                                    programDefinitions.Add(program.Name, program);
                            }

                            /*
                             * Load tables and views
                             */
                            if (!(await reader.NextResultAsync(cancellationToken).ConfigureAwait(false)))
                                throw new DatabaseSchemaException(
                                    () => Resources.DatabaseSchema_Load_RanOutOfTablesAndViews);

                            // Read raw data in.
                            List<TableDefinitionData> tableDefinitionData = new List<TableDefinitionData>();
                            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                            {
                                SqlObjectType type;
                                string typeString = reader.GetString(0) ?? string.Empty;
                                if (!ExtendedEnum<SqlObjectType>.TryParse(typeString, true, out type))
                                    throw new DatabaseSchemaException(
                                        () => Resources.DatabaseSchema_Load_CouldNotFindObjectType,
                                        typeString);
                                int schemaId = reader.GetInt32(1);
                                string name = reader.GetString(2).ToLower();
                                int ordinal = reader.GetInt32(3);
                                string columnName = reader.GetString(4).ToLower();
                                int typeId = reader.GetInt32(5);
                                SqlType sqlType;
                                if (!typesByID.TryGetValue(typeId, out sqlType) ||
                                    (sqlType == null))
                                    throw new DatabaseSchemaException(
                                        () => Resources.DatabaseSchema_Load_ColumnTypeNotFound,
                                        columnName,
                                        typeId,
                                        name);

                                short maxLength = reader.GetInt16(6);
                                byte precision = reader.GetByte(7);
                                byte scale = reader.GetByte(8);
                                SqlTypeSize sqlTypeSize = new SqlTypeSize(maxLength, precision, scale);

                                bool isNullable = reader.GetBoolean(9);

                                int? tableType = reader.IsDBNull(10) ? null : (int?)reader.GetInt32(10);

                                tableDefinitionData.Add(
                                    new TableDefinitionData(
                                        type,
                                        schemaId,
                                        name,
                                        ordinal,
                                        columnName,
                                        sqlType,
                                        sqlTypeSize,
                                        isNullable,
                                        tableType));
                            }

                            // Create unique table definitions.
                            foreach (SqlTableDefinition table in tableDefinitionData
                                // ReSharper disable once PossibleNullReferenceException
                                .GroupBy(d => d.ToString())
                                .Select(
                                    g =>
                                    {
                                        Debug.Assert(g != null);

                                        // Get columns ordered by ordinal.
                                        SqlColumn[] columns = g
                                            // ReSharper disable PossibleNullReferenceException
                                            .Select(d => d.Column)
                                            .OrderBy(c => c.Ordinal)
                                            // ReSharper restore PossibleNullReferenceException
                                            .ToArray();
                                        Debug.Assert(columns.Length > 0);

                                        TableDefinitionData first = g.First();
                                        Debug.Assert(first != null);
                                        Debug.Assert(first.Name != null);
                                        Debug.Assert(sqlSchemas != null);

                                        SqlSchema sqlSchema;
                                        if (!sqlSchemas.TryGetValue(first.SchemaID, out sqlSchema))
                                            throw new DatabaseSchemaException(
                                                () =>
                                                    Resources
                                                    .DatabaseSchema_Load_CouldNotFindSchemaLoadingTablesAndViews,
                                                first.SchemaID);
                                        Debug.Assert(sqlSchema != null);

                                        SqlTableType tableType;
                                        if (first.TableTypeID != null)
                                        {
                                            Debug.Assert(typesByID != null);

                                            SqlType tType;
                                            if (!typesByID.TryGetValue(first.TableTypeID.Value, out tType))
                                                throw new DatabaseSchemaException(
                                                    () => Resources.DatabaseSchema_Load_TableTypeNotFound,
                                                    first.TableTypeID.Value,
                                                    first.Name);
                                            tableType = tType as SqlTableType;
                                            if (tableType == null)
                                                throw new DatabaseSchemaException(
                                                    () => Resources.DatabaseSchema_Load_TypeNotTableType,
                                                    first.TableTypeID.Value,
                                                    first.Name);
                                        }
                                        else tableType = null;

                                        return new SqlTableDefinition(
                                            first.Type,
                                            sqlSchema,
                                            first.Name,
                                            columns,
                                            tableType);
                                    }))
                            {
                                Debug.Assert(table != null);
                                tables[table.FullName] = table;

                                if (!tables.ContainsKey(table.Name))
                                    tables.Add(table.Name, table);
                            }
                        }
                    }

                    // Update the current schema.
                    _current = new CurrentSchema(Schema.GetOrAdd(sqlSchemas, programDefinitions, tables, typesByName));

                    // Always return this
                    return this;
                }
                    // In the event of an error we don't set the loaded flag - this allows retries.
                catch (DatabaseSchemaException databaseSchemaException)
                {
                    // Capture the exception in the current schema.
                    _current = new CurrentSchema(ExceptionDispatchInfo.Capture(databaseSchemaException));
                    throw;
                }
                catch (Exception exception)
                {
                    // Wrap exception in Database exception.
                    DatabaseSchemaException databaseSchemaException = new DatabaseSchemaException(
                        exception,
                        LoggingLevel.Critical,
                        () => Resources.DatabaseSchema_Load_ErrorOccurred);
                    // Capture the exception in the current schema.
                    _current = new CurrentSchema(ExceptionDispatchInfo.Capture(databaseSchemaException));
                    throw databaseSchemaException;
                }
            }
        }

        /// <summary>
        /// Reloads the current schema.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable task.</returns>
        [NotNull]
        public Task<DatabaseSchema> ReLoad(CancellationToken cancellationToken)
        {
            return Load(true, cancellationToken);
        }

        /// <summary>
        /// Reloads all the schemas.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable task.</returns>
        [NotNull]
        public static Task ReloadAll(CancellationToken cancellationToken = default(CancellationToken))
        {
            // ReSharper disable AssignNullToNotNullAttribute, PossibleNullReferenceException
            return Task.WhenAll(_databaseSchemas.Values.Select(s => s.Load(true, cancellationToken)));
            // ReSharper restore AssignNullToNotNullAttribute, PossibleNullReferenceException
        }

        #region Equalities
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals([CanBeNull] ISchema other)
        {
            return Current.Equals(other);
        }
        #endregion
    }
}