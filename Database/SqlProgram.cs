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
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using JetBrains.Annotations;
using WebApplications.Utilities.Database.Exceptions;
using WebApplications.Utilities.Database.Schema;
using WebApplications.Utilities.Logging;

namespace WebApplications.Utilities.Database
{
    /// <summary>
    ///   Used to create an object for easy calling of stored procedures or functions in a database.
    /// </summary>
    public class SqlProgram
    {
        /// <summary>
        ///   The <see cref="TypeConstraintMode">type constraint mode</see>,
        ///   determines what happens if truncation or loss of precision occurs.
        /// </summary>
        public readonly TypeConstraintMode ConstraintMode;

        /// <summary>
        ///   The name of the stored procedure or function.
        /// </summary>
        [NotNull] public readonly string Name;

        /// <summary>
        ///   The parameters required by this <see cref="SqlProgram"/> (if specified).
        /// </summary>
        [UsedImplicitly] [NotNull] public readonly IEnumerable<KeyValuePair<string, Type>> Parameters;

        /// <summary>
        ///   The <see cref="LoadBalancedConnection">load balanced connection</see>.
        /// </summary>
        [NotNull] private readonly LoadBalancedConnection _connection;

        /// <summary>
        ///  A lock object to prevent multiple validations at the same time.
        /// </summary>
        [NotNull] private readonly object _validationLock = new object();

        /// <summary>
        ///   The default command timeout to use.
        ///   This is the time to wait for the program to execute before generating an error.
        /// </summary>
        private TimeSpan _defaultCommandTimeout = TimeSpan.FromSeconds(30);

        /// <summary>
        ///   Gets or sets the default command timeout.
        ///   This is the time to wait for the program to execute before generating an error.
        /// </summary>
        /// <value>
        ///   The default command timeout, this is initialized to be 30 seconds by default.
        ///   When setting the default timeout if the value is less than <see cref="TimeSpan.Zero"/>
        ///   then this will be set to the original interval of 30 seconds.
        /// </value>
        [UsedImplicitly]
        public TimeSpan DefaultCommandTimeout
        {
            get { return _defaultCommandTimeout; }
            set
            {
                if (_defaultCommandTimeout == value)
                    return;
                _defaultCommandTimeout = value < TimeSpan.Zero
                                             ? TimeSpan.FromSeconds(30)
                                             : value;
            }
        }

        /// <summary>
        ///   Holds the <see cref="SqlProgramParameter">parameter definitions</see> that are used by this <see cref="SqlProgram"/>
        /// </summary>
        [UsedImplicitly]
        [NotNull]
        protected IEnumerable<SqlProgramParameter> ProgramParameters { get; private set; }

        /// <summary>
        ///   The underlying <see cref="SqlProgramDefinition">program definition</see>.
        /// </summary>
        [NotNull]
        public SqlProgramDefinition Definition { get; private set; }

        /// <summary>
        ///   Gets the validation error (if any).
        /// </summary>
        /// <value>
        ///   Any errors that occurred during program <see cref="Validate">validation</see>.
        /// </value>
        public LoggingException ValidationError { get; private set; }

        #region Constructors
        /// <summary>
        ///   Initializes a new instance of the <see cref="SqlProgram"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="name">The <see cref="Name">name</see> of the program.</param>
        /// <param name="parameters">The program <see cref="ProgramParameters">parameters</see>.</param>
        /// <param name="ignoreValidationErrors">
        ///   <para>If set to <see langword="true"/> then don't throw any parameter validation errors.</para>
        ///   <para>By default this is set to <see langword="false"/>.</para>
        /// </param>
        /// <param name="checkOrder">
        ///   <para>If set to <see langword="true"/> then check the order of the <see paramref="parameters"/>.</para>
        ///   <para>By default this is set to <see langword="false"/>.</para>
        /// </param>
        /// <param name="defaultCommandTimeout">
        ///   <para>The <see cref="DefaultCommandTimeout">default command timeout</see></para>
        ///   <para>This is the time to wait for the command to execute.</para>
        ///   <para>If set to <see langword="null"/> then the timeout will be 30 seconds.</para>
        /// </param>
        /// <param name="constraintMode">
        ///   <para>The type constraint mode.</para>
        ///   <para>By default this is set to log a warning if truncation/loss of precision occurs.</para>
        /// </param>
        /// <remarks>
        ///   There is a <see cref="System.Diagnostics.Contracts.Contract"/> which specifies that
        ///   <paramref name="connectionString"/> and <paramref name="name"/> cannot be <see langword="null"/>.
        /// </remarks>
        /// <exception cref="LoggingException">
        ///   <para><paramref name="connectionString"/> is <see langword="null"/>.</para>
        ///   <para>-or-</para>
        ///   <para>No program <paramref name="name"/> specified.</para>
        ///   <para>-or-</para>
        ///   <para>Invalid program definition.</para>
        /// </exception>
        [UsedImplicitly]
        public SqlProgram(
            [NotNull] string connectionString,
            [NotNull] string name,
            [CanBeNull] IEnumerable<KeyValuePair<string, Type>> parameters = null,
            bool ignoreValidationErrors = false,
            bool checkOrder = false,
            TimeSpan? defaultCommandTimeout = null,
            TypeConstraintMode constraintMode = TypeConstraintMode.Warn)
            : this(
                new LoadBalancedConnection(connectionString), name, parameters, ignoreValidationErrors, checkOrder,
                defaultCommandTimeout, constraintMode)
        {
            Contract.Requires(connectionString != null, Resources.SqlProgram_ConnectionStringCanNotBeNull);
            Contract.Requires(name != null, Resources.SqlProgram_NameCanNotBeNull);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SqlProgram"/> class.
        /// </summary>
        /// <param name="connection">The load balanced connection.</param>
        /// <param name="name">The <see cref="Name">name</see> of the program.</param>
        /// <param name="parameters">The program <see cref="ProgramParameters">parameters</see>.</param>
        /// <param name="ignoreValidationErrors">
        ///   <para>If set to <see langword="true"/> then don't throw any parameter validation errors.</para>
        ///   <para>By default this is set to <see langword="false"/>.</para>
        /// </param>
        /// <param name="checkOrder">
        ///   <para>If set to <see langword="true"/> then check the order of the <see paramref="parameters"/>.</para>
        ///   <para>By default this is set to <see langword="false"/>.</para>
        /// </param>
        /// <param name="defaultCommandTimeout">
        ///   <para>The <see cref="DefaultCommandTimeout">default command timeout</see></para>
        ///   <para>This is the time to wait for the command to execute.</para>
        ///   <para>If set to <see langword="null"/> then the timeout will be 30 seconds.</para>
        /// </param>
        /// <param name="constraintMode">
        ///   <para>The type constraint mode.</para>
        ///   <para>By default this is set to log a warning if truncation/loss of precision occurs.</para>
        /// </param>
        /// <remarks>
        ///   There is a <see cref="System.Diagnostics.Contracts.Contract"/> which specifies that
        ///   <paramref name="connection"/> and <paramref name="name"/> cannot be <see langword="null"/>.
        /// </remarks>
        /// <exception cref="LoggingException">
        ///   <para>No <paramref name="connection"/> specified.</para>
        ///   <para>-or-</para>
        ///   <para>No program <paramref name="name"/> specified.</para>
        ///   <para>-or-</para>
        ///   <para>Invalid program definition.</para>
        /// </exception>
        public SqlProgram(
            [NotNull] LoadBalancedConnection connection,
            [NotNull] string name,
            [CanBeNull] IEnumerable<KeyValuePair<string, Type>> parameters = null,
            bool ignoreValidationErrors = false,
            bool checkOrder = false,
            TimeSpan? defaultCommandTimeout = null,
            TypeConstraintMode constraintMode = TypeConstraintMode.Warn)
        {
            Contract.Requires(connection != null, Resources.SqlProgram_ConnectionCanNotBeNull);
            Contract.Requires(name != null, Resources.SqlProgram_NameCanNotBeNull);

            if (connection == null)
                throw new LoggingException(LoggingLevel.Critical, Resources.SqlProgram_NoConnectionSpecified);
            if (String.IsNullOrWhiteSpace(name))
                throw new LoggingException(LoggingLevel.Critical, Resources.SqlProgram_NoProgramNameSpecified);

            Name = name;
            _connection = connection;

            DefaultCommandTimeout = (defaultCommandTimeout == null || defaultCommandTimeout < TimeSpan.Zero)
                                        ? TimeSpan.FromSeconds(30)
                                        : (TimeSpan) defaultCommandTimeout;

            ConstraintMode = constraintMode;

            if (parameters == null)
            {
                Parameters = Enumerable.Empty<KeyValuePair<string, Type>>();
                ProgramParameters = Enumerable.Empty<SqlProgramParameter>();
            }
            else
            {
                Parameters = parameters;
                LoggingException error = Validate(checkOrder);
                if (!ignoreValidationErrors && error != null)
                    throw error;
            }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SqlProgram"/> class.
        /// </summary>
        /// <param name="program">The base program (stored procedure/function).</param>
        /// <param name="ignoreValidationErrors">
        ///   If set to <see langword="true"/> then don't throw any parameter validation errors.
        /// </param>
        /// <param name="defaultCommandTimeout">
        ///   <para>The <see cref="DefaultCommandTimeout">default command timeout</see></para>
        ///   <para>This is the time to wait for the command to execute.</para>
        ///   <para>If set to <see langword="null"/> then the timeout the default timeout from the base program.</para>
        /// </param>
        /// <param name="constraintMode">
        ///   The type constraint mode, this defined the behavior when truncation/loss of precision occurs.
        /// </param>
        /// <param name="parameterTypes">The parameter types.</param>
        /// <exception cref="LoggingException">Invalid parameters.</exception>
        [UsedImplicitly]
        protected SqlProgram(
            [NotNull] SqlProgram program,
            bool ignoreValidationErrors,
            TimeSpan? defaultCommandTimeout,
            TypeConstraintMode constraintMode,
            [NotNull] params Type[] parameterTypes)
            : this(
                program,
                parameterTypes.Select(t => new KeyValuePair<string, Type>(null, t)),
                ignoreValidationErrors,
                true,
                defaultCommandTimeout,
                constraintMode
                )
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SqlProgram"/> class.
        /// </summary>
        /// <param name="program">The base program (stored procedure/function).</param>
        /// <param name="ignoreValidationErrors">
        ///   If set to <see langword="true"/> then don't throw any parameter validation errors.
        /// </param>
        /// <param name="checkOrder">
        ///   If set to <see langword="true"/> then check the order of the parameters.
        /// </param>
        /// <param name="defaultCommandTimeout">
        ///   <para>The <see cref="DefaultCommandTimeout">default command timeout</see></para>
        ///   <para>This is the time to wait for the command to execute.</para>
        ///   <para>If set to <see langword="null"/> then the timeout the default timeout from the base program.</para>
        /// </param>
        /// <param name="constraintMode">
        ///   The type constraint mode, this defined the behavior when truncation/loss of precision occurs.
        /// </param>
        /// <param name="parameterNames">The parameter names.</param>
        /// <param name="parameterTypes">The parameter types.</param>
        /// <exception cref="LoggingException">Invalid parameters</exception>
        [UsedImplicitly]
        protected SqlProgram(
            [NotNull] SqlProgram program,
            bool ignoreValidationErrors,
            bool checkOrder,
            TimeSpan? defaultCommandTimeout,
            TypeConstraintMode constraintMode,
            [NotNull] IEnumerable<string> parameterNames,
            [NotNull] params Type[] parameterTypes)
            : this(
                program,
                SqlProgramDefinition.ToKVP(parameterNames, parameterTypes),
                ignoreValidationErrors,
                checkOrder,
                defaultCommandTimeout,
                constraintMode)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SqlProgram"/> class.
        /// </summary>
        /// <param name="program">The base program (stored procedure/function).</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="ignoreValidationErrors">
        ///   If set to <see langword="true"/> then don't throw any parameter validation errors.
        /// </param>
        /// <param name="checkOrder">
        ///   If set to <see langword="true"/> then check the order of the <see paramref="parameters"/>.
        /// </param>
        /// <param name="defaultCommandTimeout">
        ///   <para>The <see cref="DefaultCommandTimeout">default command timeout</see></para>
        ///   <para>This is the time to wait for the command to execute.</para>
        ///   <para>If set to <see langword="null"/> then the timeout the default timeout from the base program.</para>
        /// </param>
        /// <param name="constraintMode">
        ///   The type constraint mode, this defined the behavior when truncation/loss of precision occurs.
        /// </param>
        /// <exception cref="LoggingException">Invalid parameters.</exception>
        private SqlProgram(
            [NotNull] SqlProgram program,
            [NotNull] IEnumerable<KeyValuePair<string, Type>> parameters,
            bool ignoreValidationErrors,
            bool checkOrder,
            TimeSpan? defaultCommandTimeout,
            TypeConstraintMode constraintMode)
        {
            _connection = program._connection;
            Name = program.Name;

            // If we have no specific default command timeout set it to the existing one for the program.
            if (defaultCommandTimeout == null)
                DefaultCommandTimeout = program.DefaultCommandTimeout;
            else
                DefaultCommandTimeout = (TimeSpan) defaultCommandTimeout;
            ConstraintMode = constraintMode;
            Parameters = parameters;
            LoggingException error = Validate(checkOrder);
            if (!ignoreValidationErrors && error != null)
                throw error;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SqlProgram"/> class.
        /// </summary>
        /// <param name="connection">The load balanced connection.</param>
        /// <param name="name">The program name.</param>
        /// <param name="ignoreValidationErrors">
        ///   If set to <see langword="true"/> then don't throw any parameter validation errors.
        /// </param>
        /// <param name="defaultCommandTimeout">
        ///   <para>The <see cref="DefaultCommandTimeout">default command timeout</see></para>
        ///   <para>This is the time to wait for the command to execute.</para>
        ///   <para>If set to <see langword="null"/> then the timeout will be 30 seconds.</para>
        /// </param>
        /// <param name="constraintMode">
        ///   The type constraint mode, this defined the behavior when truncation/loss of precision occurs.
        /// </param>
        /// <param name="parameterTypes">The parameter types.</param>
        /// <remarks>
        ///   There is a <see cref="System.Diagnostics.Contracts.Contract"/> which specifies that
        ///   <paramref name="connection"/> and <paramref name="name"/> cannot be <see langword="null"/>.
        /// </remarks>
        /// <exception cref="LoggingException">
        ///   <para>No <paramref name="connection"/> specified.</para>
        ///   <para>-or-</para>
        ///   <para>No program <paramref name="name"/> specified.</para>
        ///   <para>-or-</para>
        ///   <para>Invalid program definition.</para>
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="parameterTypes"/> is <see langword="null"/>.
        /// </exception>
        [UsedImplicitly]
        protected SqlProgram(
            [NotNull] LoadBalancedConnection connection,
            [NotNull] string name,
            bool ignoreValidationErrors,
            TimeSpan? defaultCommandTimeout,
            TypeConstraintMode constraintMode,
            [NotNull] params Type[] parameterTypes)
            : this(
                connection, name,
                parameterTypes.Select(t => new KeyValuePair<string, Type>(null, t)), ignoreValidationErrors, true,
                defaultCommandTimeout, constraintMode)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SqlProgram"/> class.
        /// </summary>
        /// <param name="connection">The load balanced connection.</param>
        /// <param name="name">The program name.</param>
        /// <param name="ignoreValidationErrors">
        ///   If set to <see langword="true"/> then don't throw any parameter validation errors.
        /// </param>
        /// <param name="checkOrder">
        ///   If set to <see langword="true"/> then check the order of the parameters.
        /// </param>
        /// <param name="defaultCommandTimeout">
        ///   <para>The <see cref="DefaultCommandTimeout">default command timeout</see></para>
        ///   <para>This is the time to wait for the command to execute.</para>
        ///   <para>If set to <see langword="null"/> then the timeout will be 30 seconds.</para>
        /// </param>
        /// <param name="constraintMode">
        ///   The type constraint mode, this defined the behavior when truncation/loss of precision occurs.
        /// </param>
        /// <param name="parameterNames">The parameter names.</param>
        /// <param name="parameterTypes">The parameter types.</param>
        /// <remarks>
        ///   There is a <see cref="System.Diagnostics.Contracts.Contract"/> which specifies that
        ///   <paramref name="connection"/> and <paramref name="name"/> cannot be <see langword="null"/>.
        /// </remarks>
        /// <exception cref="LoggingException">
        ///   <para>The supplied <paramref name="parameterNames"/> count did not match the <paramref name="parameterTypes"/> count.</para>
        ///   <para>-or-</para>
        ///   <para>No <paramref name="connection"/> specified.</para>
        ///   <para>-or-</para>
        ///   <para>No program <paramref name="name"/> specified.</para>
        ///   <para>-or-</para>
        ///   <para>Invalid program definition.</para>
        /// </exception>
        [UsedImplicitly]
        protected SqlProgram(
            [NotNull] LoadBalancedConnection connection,
            [NotNull] string name,
            bool ignoreValidationErrors,
            bool checkOrder,
            TimeSpan? defaultCommandTimeout,
            TypeConstraintMode constraintMode,
            [NotNull] IEnumerable<string> parameterNames,
            [NotNull] params Type[] parameterTypes)
            : this(
                connection, name, SqlProgramDefinition.ToKVP(parameterNames, parameterTypes), ignoreValidationErrors,
                checkOrder, defaultCommandTimeout, constraintMode)
        {
        }
        #endregion

        /// <summary>
        ///   Re-validates the <see cref="SqlProgram">SQL Program</see>.
        /// </summary>
        /// <param name="checkOrder">
        ///   <para>If set to <see langword="true"/> then check the order of the parameters.</para>
        ///   <para>By default this is set to <see langword="false"/>.</para>
        /// </param>
        /// <param name="forceSchemaReload">If set to <see langword="true"/> forces a schema reload.</param>
        /// <returns>
        ///   Any <see cref="ValidationError">validation errors</see> that occurred (if any).
        /// </returns>
        /// <exception cref="LoggingException">
        ///   <para>Program definition was not found.</para>
        ///   <para>-or-</para>
        ///   <para>Inconsistent program definitions found in different connections from the load balanced connection.</para>
        /// </exception>
        [CanBeNull]
        public LoggingException Validate(bool checkOrder = false, bool forceSchemaReload = false)
        {
            lock (_validationLock)
            {
                ValidationError = null;
                string name = Name.ToLower();

                bool includeSqlSchema = name.Contains(".");
                // Find the program definition
                bool first = true;
                try
                {
                    foreach (string cs in _connection)
                    {
                        // Grab the schema for the connection string.
                        bool changed;
                        DatabaseSchema schema = DatabaseSchema.GetOrAdd(cs, forceSchemaReload, out changed);

                        // Find the program
                        SqlProgramDefinition programDefinition =
                            schema.ProgramDefinitions.FirstOrDefault(
                                pd => (includeSqlSchema ? pd.FullName : pd.Name) == name);

                        if (programDefinition == null)
                            throw new LoggingException(
                                LoggingLevel.Critical, 
                                Resources.SqlProgram_Validate_DefinitionsNotFound, name);

                        // If this is the first connection just set the program definition
                        if (first)
                        {
                            Definition = programDefinition;
                            first = false;
                        }
                        else if (!Definition.Equals(programDefinition))
                            // If the program definition is different we have a fatal error.
                            throw new LoggingException(
                                LoggingLevel.Critical,
                                Resources.SqlProgram_Validate_InconsistentProgramDefinitions,
                                name);

                        // If schemas are identical, no need to check anymore
                        if (_connection.IdenticalSchemas)
                            break;
                    }

                    if (Parameters.Any())
                    {
                        try
                        {
                            ProgramParameters = Definition.ValidateParameters(Parameters, checkOrder);
                        }
                        catch (LoggingException error)
                        {
                            ValidationError = error;
                        }
                    }
                }
                catch (LoggingException error)
                {
                    ValidationError = error;
                }
                return ValidationError;
            }
        }

        /// <summary>
        ///   Creates a new command against a random connection.
        /// </summary>
        /// <param name="timeout">
        ///   <para>The time to wait for an executing program before throwing an error.</para>
        ///   <para>If <see langword="null"/> is specified then <see cref="DefaultCommandTimeout"/> is used.</para>
        /// </param>
        /// <returns>A <see cref="SqlProgramCommand"/>.</returns>
        /// <remarks>
        ///   As <see cref="SqlProgramCommand"/> implements <see cref="IDisposable"/> it's
        ///   recommended that you have this in a <c>using</c> statement.
        /// </remarks>
        /// <exception cref="LoggingException">
        ///   Failed to select a valid connection string from the <see cref="LoadBalancedConnection"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///   The connection did not specify a source or server.
        /// </exception>
        /// <exception cref="SqlException">
        ///   A connection-level error occurred whilst opening the connection.
        ///   See <see cref="SqlConnection.Open"/> for more details.
        /// </exception>
        [NotNull]
        [UsedImplicitly]
        public SqlProgramCommand CreateCommand(TimeSpan? timeout = null)
        {
            return new SqlProgramCommand(this, _connection.CreateConnection(),
                                         (timeout == null || timeout < TimeSpan.Zero)
                                             ? DefaultCommandTimeout
                                             : (TimeSpan) timeout);
        }

        /// <summary>
        ///   Creates the commands for all connections in a load balanced connection.
        /// </summary>
        /// <param name="timeout">
        ///   <para>The time to wait for an executing program before throwing an error.</para>
        ///   <para>If <see langword="null"/> or less than <see cref="TimeSpan.Zero"/> then <see cref="DefaultCommandTimeout"/> is used.</para>
        /// </param>
        /// <returns>An enumeration of <see cref="SqlProgramCommand"/> (one for each connection).</returns>
        /// <remarks>
        ///   As <see cref="SqlProgramCommand"/> implements <see cref="IDisposable"/> it is recommended that
        ///   you use wrap your code in a try...finally and ensure you dispose all returned <see cref="SqlProgramCommand"/>
        ///   in the event of an error.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        ///   The connection did not specify a source or server.
        /// </exception>
        /// <exception cref="SqlException">
        ///   A connection-level error occurred whilst opening the connection.
        ///   See <see cref="SqlConnection.Open"/> for more details.
        /// </exception>
        [UsedImplicitly]
        [NotNull]
        public IEnumerable<SqlProgramCommand> CreateCommandsForAllConnections(TimeSpan? timeout = null)
        {
            TimeSpan t = (timeout == null || timeout < TimeSpan.Zero)
                             ? DefaultCommandTimeout
                             : (TimeSpan) timeout;
            return
                _connection.Select(
                    connectionString =>
                        {
                            SqlConnection connection = new SqlConnection(connectionString);
                            connection.Open();
                            return new SqlProgramCommand(this, connection, t);
                        }).ToList
                    ();
        }

        /// <summary>
        ///   Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        ///   <para>A <see cref="string"/> representation of this instance.</para>
        ///   <para><b>Format:</b> <see cref="Name"/> + "Program".</para>
        /// </returns>
        [NotNull]
        public override string ToString()
        {
            return Name + " Program";
        }

        /// <summary>
        ///   Generic method for creating an async version of the command execution.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="command">The command to create an async version of.</param>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="taskCreator">
        ///   The function used to create the <see cref="Task&lt;TResult&gt;"/>.
        /// </param>
        /// <param name="state">The state.</param>
        /// <returns>
        ///   The completed <see cref="Task&lt;TResult&gt;"/>.
        /// </returns>
        /// <exception cref="SqlProgramExecutionException">
        ///   An exception occurred whilst trying to set the parameters to the command.
        /// </exception>
        /// <exception cref="NullReferenceException">
        ///   <paramref name="taskCreator"/> is <see langword="null"/>.
        /// </exception>
        private async Task<TResult> ExecuteAsync<TResult>(
            [NotNull] SqlProgramCommand command,
            [CanBeNull] Action<SqlProgramCommand> setParameters,
            [NotNull] Func<SqlProgramCommand, Task<TResult>> taskCreator,
            [CanBeNull] object state)
        {
            return await ExecuteAsync(command, setParameters, taskCreator, r => r, state);
        }

        /// <summary>
        ///   Generic method for creating an async version of the command execution.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TTaskResult">The type of the task's result.</typeparam>
        /// <param name="command">The command to create an async version of.</param>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="taskCreator">
        ///   The function used to create the <see cref="Task&lt;TResult&gt;"/>.
        /// </param>
        /// <param name="function">
        ///   <para>The continuation function, which runs before disposing the <paramref name="command"/>.</para>
        ///   <para>This takes in the result of the task created by the <paramref name="taskCreator"/>.</para>
        /// </param>
        /// <param name="state">
        ///   The state to use as the <see cref="Task&lt;TResult&gt;"/>'s
        ///   <see cref="System.Threading.Tasks.Task.AsyncState"/>.
        /// </param>
        /// <returns>
        ///   The completed <see cref="Task&lt;TResult&gt;"/>.
        /// </returns>
        /// <exception cref="SqlProgramExecutionException">
        ///   An exception occurred whilst trying to set the parameters to the command.
        /// </exception>
        /// <exception cref="NullReferenceException">
        ///   <paramref name="taskCreator"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="function"/> is <see langword="null"/>.
        /// </exception>
        private async Task<TResult> ExecuteAsync<TResult, TTaskResult>(
            [NotNull] SqlProgramCommand command,
            [CanBeNull] Action<SqlProgramCommand> setParameters,
            [NotNull] Func<SqlProgramCommand, Task<TTaskResult>> taskCreator,
            [NotNull] Func<TTaskResult, TResult> function,
            [CanBeNull] object state)
        {
            try
            {
                if (setParameters != null)
                    setParameters(command);
            }
            catch (Exception exception)
            {
                throw new SqlProgramExecutionException(this, exception);
            }

            TaskCompletionSource<TResult> taskCompletion = new TaskCompletionSource<TResult>(state);

            Task task = taskCreator(command)
                .ContinueWith(t => t.IsCompleted
                                       ? function(t.Result)
                                       : default(TResult),
                              TaskContinuationOptions.ExecuteSynchronously)
                .ContinueWith(t =>
                                  {
                                      command.Dispose();
                                      taskCompletion.SetFromTask(t);
                                  },
                              TaskContinuationOptions.ExecuteSynchronously
                );

            return await taskCompletion.Task;
        }

        /// <summary>
        ///   Generic method for creating an async version of the command execution on all connections.
        /// </summary>
        /// <typeparam name="TResult">The type of the results.</typeparam>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="taskCreator">
        ///   The function used to create the <see cref="Task&lt;TResult&gt;"/>.
        /// </param>
        /// <param name="state">
        ///   The state to use as the <see cref="System.Threading.Tasks.Task.AsyncState"/>.
        /// </param>
        /// <returns>
        ///   The completed <see cref="Task&lt;TResult&gt;"/> executed on all connections.
        /// </returns>
        /// <exception cref="SqlException">
        ///   A connection-level error occurred.
        ///   See <see cref="SqlConnection.Open"/>.
        /// </exception>
        /// <exception cref="SqlProgramExecutionException">
        ///   An exception occurred whilst trying to set the parameters to the command.
        /// </exception>
        /// <exception cref="NullReferenceException">
        ///   <paramref name="taskCreator"/> is <see langword="null"/>.
        /// </exception>
        private async Task<IEnumerable<TResult>> ExecuteAllAsync<TResult>(
            [CanBeNull] Action<SqlProgramCommand> setParameters,
            [NotNull] Func<SqlProgramCommand, Task<TResult>> taskCreator,
            [CanBeNull] object state)
        {
            return await ExecuteAllAsync(setParameters, taskCreator, r => r, state);
        }

        /// <summary>
        ///   Generic method for creating an async version of the command execution on all connections.
        /// </summary>
        /// <typeparam name="TResult">The type of the results.</typeparam>
        /// <typeparam name="TTaskResult">The type of the task's result.</typeparam>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="taskCreator">
        ///   The function used to create the <see cref="Task&lt;TResult&gt;"/>.
        /// </param>
        /// <param name="function">
        ///   <para>The continuation function, which runs before disposing the command.</para>
        ///   <para>This takes in the result of the task created by the <paramref name="taskCreator"/>.</para>
        /// </param>
        /// <param name="state">
        ///   The state to use as the <see cref="System.Threading.Tasks.Task.AsyncState"/>.
        /// </param>
        /// <returns>
        ///   The completed <see cref="Task&lt;TResult&gt;"/> executed on all connections.
        /// </returns>
        /// <exception cref="SqlException">
        ///   A connection-level error occurred.
        ///   See <see cref="SqlConnection.Open"/>.
        /// </exception>
        /// <exception cref="SqlProgramExecutionException">
        ///   An exception occurred whilst trying to set the parameters to the command.
        /// </exception>
        /// <exception cref="NullReferenceException">
        ///   <paramref name="taskCreator"/> is <see langword="null"/>.
        /// </exception>
        private async Task<IEnumerable<TResult>> ExecuteAllAsync<TResult, TTaskResult>(
            [CanBeNull] Action<SqlProgramCommand> setParameters,
            [NotNull] Func<SqlProgramCommand, Task<TTaskResult>> taskCreator,
            [NotNull] Func<TTaskResult, TResult> function,
            [CanBeNull] object state)
        {
            TaskCompletionSource<IEnumerable<TResult>> taskCompletion =
                new TaskCompletionSource<IEnumerable<TResult>>(state);
            // Create tasks.
            return await Task<IEnumerable<TResult>>.Factory.ContinueWhenAll(
                _connection.Select(
                    connectionString =>
                        {
                            // Create and open a connection
                            SqlConnection connection = new SqlConnection(connectionString);
                            connection.Open();

                            SqlProgramCommand command = new SqlProgramCommand(this, connection, DefaultCommandTimeout);
                            try
                            {
                                if (setParameters != null)
                                    setParameters(command);
                            }
                            catch (Exception exception)
                            {
                                throw new SqlProgramExecutionException(this, exception);
                            }
                            return taskCreator(command)
                                .ContinueWith(t =>
                                                  {
                                                      if (!t.IsCompleted)
                                                          return default(TResult);

                                                      try
                                                      {
                                                          return function(t.Result);
                                                      }
                                                      finally
                                                      {
                                                          // This will also dispose of the connection
                                                          command.Dispose();
                                                      }
                                                  },
                                              TaskContinuationOptions.ExecuteSynchronously);
                        }).ToArray(),
                tasks =>
                    {
                        // Propagate all exceptions and mark all faulted tasks as observed.
                        Task.WaitAll(tasks);

                        return tasks.Select(t => t.Result);
                    });
        }

        #region ExecuteScalar and ExecuteScalarAll overloads
        /// <summary>
        ///   Executes the query, and returns the first column of the first row in the result set returned by the query.
        ///   Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="T">The output type expected.</typeparam>
        /// <returns>The scalar value.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        [CanBeNull]
        public T ExecuteScalar<T>()
        {
            return ExecuteScalar<T>(null);
        }

        /// <summary>
        ///   Executes the query against each connection, and returns the first column of the first row in the result set returned by the query.
        ///   Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="T">The output type expected.</typeparam>
        /// <returns>
        ///   An enumerable containing the scalar values returned by each connection.
        /// </returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <exception cref="InvalidOperationException">
        ///   The connection did not specify a source or server.
        /// </exception>
        /// <exception cref="SqlException">
        ///   A connection-level error occurred whilst opening the connection.
        ///   See <see cref="SqlConnection.Open"/> for more details.
        /// </exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        [NotNull]
        public IEnumerable<T> ExecuteScalarAll<T>()
        {
            return ExecuteScalarAll<T>(null);
        }

        /// <summary>
        ///   Executes the query, and returns the first column of the first row in the result set returned by the query.
        ///   Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="T">The output type expected.</typeparam>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <returns>The scalar value.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        [CanBeNull]
        public T ExecuteScalar<T>([CanBeNull] Action<SqlProgramCommand> setParameters)
        {
            using (SqlProgramCommand command = CreateCommand())
            {
                T result;
                try
                {
                    if (setParameters != null)
                        setParameters(command);

                    result = command.ExecuteScalar<T>();
                }
                catch (Exception exception)
                {
                    throw new SqlProgramExecutionException(this, exception);
                }
                return result;
            }
        }

        /// <summary>
        ///   Executes the query against each connection, and returns the first column of the first row in the result set returned by the query.
        ///   Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="T">The output type expected.</typeparam>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <returns>
        ///   An enumerable containing the scalar values returned by each connection.
        /// </returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <exception cref="InvalidOperationException">
        ///   The connection did not specify a source or server.
        /// </exception>
        /// <exception cref="SqlException">
        ///   A connection-level error occurred whilst opening the connection.
        ///   See <see cref="SqlConnection.Open"/> for more details.
        /// </exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        [NotNull]
        public IEnumerable<T> ExecuteScalarAll<T>([CanBeNull] Action<SqlProgramCommand> setParameters)
        {
            IEnumerable<SqlProgramCommand> commands = CreateCommandsForAllConnections();

            List<T> results = new List<T>(commands.Count());
            try
            {
                foreach (SqlProgramCommand command in commands)
                {
                    if (setParameters != null)
                        setParameters(command);

                    results.Add(command.ExecuteScalar<T>());
                }
            }
            catch (Exception exception)
            {
                throw new SqlProgramExecutionException(this, exception);
            }
            finally
            {
                foreach (SqlProgramCommand command in commands)
                    command.Dispose();
            }
            return results;
        }
        #endregion

        #region ExecuteScalarAsync and ExecuteScalarAllAsync overloads
        /// <summary>
        ///   Executes the query, and returns the first column of the first row in the result set returned by the query.
        ///   Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="T">The output type expected.</typeparam>
        /// <param name="state">
        ///   <para>The object qualifying the state.</para>
        ///   <para>By default this is <see langword="null"/>.</para>
        /// </param>
        /// <returns>A task containing the resulting scalar value.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <exception cref="LoggingException">
        ///   Failed to select a valid connection string from the <see cref="LoadBalancedConnection"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///   The connection did not specify a source or server.
        /// </exception>
        /// <exception cref="SqlException">
        ///   A connection-level error occurred whilst opening the connection.
        ///   See <see cref="SqlConnection.Open"/> for more details.
        /// </exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        [CanBeNull]
        public async Task<T> ExecuteScalarAsync<T>(object state = null)
        {
            return await ExecuteScalarAsync<T>(null, state);
        }

        /// <summary>
        ///   Executes the query against each connection, and returns the first column of the first row in the result set returned by the query.
        ///   Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="T">The output type expected.</typeparam>
        /// <param name="state">
        ///   <para>The object qualifying the state.</para>
        ///   <para>By default this is <see langword="null"/>.</para>
        /// </param>
        /// <returns>
        ///   A <see cref="Task&lt;TResult&gt;"/> containing an enumerable of the scalar values returned by each connection.
        /// </returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        [NotNull]
        public async Task<IEnumerable<T>> ExecuteScalarAllAsync<T>(object state = null)
        {
            return await ExecuteScalarAllAsync<T>(null, state);
        }

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="T">The output type expected.</typeparam>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="state">
        ///   <para>The object qualifying the state.</para>
        ///   <para>By default this is <see langword="null"/>.</para>
        /// </param>
        /// <returns>A task containing the resulting scalar value.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <exception cref="LoggingException">
        ///   Failed to select a valid connection string from the <see cref="LoadBalancedConnection"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///   The connection did not specify a source or server.
        /// </exception>
        /// <exception cref="SqlException">
        ///   A connection-level error occurred whilst opening the connection.
        ///   See <see cref="SqlConnection.Open"/> for more details.
        /// </exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        [CanBeNull]
        public async Task<T> ExecuteScalarAsync<T>([CanBeNull] Action<SqlProgramCommand> setParameters,
                                                   object state = null)
        {
            return
                await ExecuteAsync(new SqlProgramCommand(this, _connection.CreateConnection(), DefaultCommandTimeout),
                                   setParameters,
                                   c => c.ExecuteScalarAsync<T>(state),
                                   state);
        }

        /// <summary>
        /// Executes the query against each connection, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="T">The output type expected.</typeparam>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="state">
        ///   <para>The object qualifying the state.</para>
        ///   <para>By default this is <see langword="null"/>.</para>
        /// </param>
        /// <returns>
        ///   A <see cref="Task&lt;TResult&gt;"/> containing an enumerable of the scalar values returned by each connection.
        /// </returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <exception cref="SqlException">
        ///   A connection-level error occurred.
        ///   See <see cref="SqlConnection.Open"/>.
        /// </exception>
        /// <exception cref="SqlProgramExecutionException">
        ///   An exception occurred whilst trying to set the parameters to the command.
        /// </exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        [NotNull]
        public async Task<IEnumerable<T>> ExecuteScalarAllAsync<T>([CanBeNull] Action<SqlProgramCommand> setParameters,
                                                                   object state = null)
        {
            return await ExecuteAllAsync(setParameters, c => c.ExecuteScalarAsync<T>(state), state);
        }
        #endregion

        #region ExecuteNonQuery and ExecuteNonQueryAll overloads
        /// <summary>
        ///   Executes a Transact-SQL statement against the connection and returns the number of rows affected.
        /// </summary>
        /// <returns>The number of rows affected.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        public int ExecuteNonQuery()
        {
            return ExecuteNonQuery(null);
        }

        /// <summary>
        ///   Executes a Transact-SQL statement against every connection and returns the number of rows affected for each connection.
        /// </summary>
        /// <returns>The number of rows affected.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        [NotNull]
        public IEnumerable<int> ExecuteNonQueryAll()
        {
            return ExecuteNonQueryAll(null);
        }

        /// <summary>
        /// Executes a Transact-SQL statement against the connection and returns the number of rows affected.
        /// </summary>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <returns>The number of rows affected.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        public int ExecuteNonQuery([CanBeNull] Action<SqlProgramCommand> setParameters)
        {
            using (SqlProgramCommand command = CreateCommand())
            {
                int result;
                try
                {
                    if (setParameters != null)
                        setParameters(command);

                    result = command.ExecuteNonQuery();
                }
                catch (Exception exception)
                {
                    throw new SqlProgramExecutionException(this, exception);
                }
                finally
                {
                    command.Dispose();
                }
                return result;
            }
        }

        /// <summary>
        ///   Executes a Transact-SQL statement against every connection and returns the number of rows affected for each connection.
        /// </summary>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <returns>An enumerable containing the number of rows affected for each connection.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        [NotNull]
        public IEnumerable<int> ExecuteNonQueryAll([CanBeNull] Action<SqlProgramCommand> setParameters)
        {
            IEnumerable<SqlProgramCommand> commands = CreateCommandsForAllConnections();

            List<int> results = new List<int>(commands.Count());
            try
            {
                foreach (SqlProgramCommand command in commands)
                {
                    if (setParameters != null)
                        setParameters(command);

                    results.Add(command.ExecuteNonQuery());
                }
            }
            catch (Exception exception)
            {
                throw new SqlProgramExecutionException(this, exception);
            }
            finally
            {
                foreach (SqlProgramCommand command in commands)
                    command.Dispose();
            }
            return results;
        }
        #endregion

        #region ExecuteNonQueryAsync and ExecuteNonQueryAllAsync overloads
        /// <summary>
        ///   Executes a Transact-SQL statement against the connection and returns the number of rows affected.
        /// </summary>
        /// <param name="state">
        ///   <para>The object qualifying the state.</para>
        ///   <para>By default this is <see langword="null"/>.</para>
        /// </param>
        /// <returns>The number of rows affected.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        public async Task<int> ExecuteNonQueryAsync(object state = null)
        {
            return await ExecuteNonQueryAsync(null, state);
        }

        /// <summary>
        ///   Executes a Transact-SQL statement against every connection and returns the number of rows affected for each connection.
        /// </summary>
        /// <param name="state">
        ///   <para>The object qualifying the state.</para>
        ///   <para>By default this is <see langword="null"/>.</para>
        /// </param>
        /// <returns>The number of rows affected.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        [NotNull]
        public async Task<IEnumerable<int>> ExecuteNonQueryAllAsync(object state = null)
        {
            return await ExecuteNonQueryAllAsync(null, state);
        }

        /// <summary>
        ///   Executes a Transact-SQL statement against the connection and returns the number of rows affected.
        /// </summary>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="state">
        ///   <para>The object qualifying the state.</para>
        ///   <para>By default this is <see langword="null"/>.</para>
        /// </param>
        /// <returns>The number of rows affected.</returns>
        /// <exception cref="SqlProgramExecutionException">
        ///   An error occurred executing the program.
        /// </exception>
        /// <exception cref="LoggingException">
        ///   Failed to select a valid connection string from the <see cref="LoadBalancedConnection"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///   The connection did not specify a source or server.
        /// </exception>
        /// <exception cref="SqlException">
        ///   A connection-level error occurred whilst opening the connection.
        ///   See <see cref="SqlConnection.Open"/> for more details.
        /// </exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        public async Task<int> ExecuteNonQueryAsync([CanBeNull] Action<SqlProgramCommand> setParameters,
                                                    object state = null)
        {
            return
                await ExecuteAsync(new SqlProgramCommand(this, _connection.CreateConnection(), DefaultCommandTimeout),
                                   setParameters,
                                   c => c.ExecuteNonQueryAsync(state), state);
        }

        /// <summary>
        ///   Executes a Transact-SQL statement against every connection and returns the number of rows affected for each connection.
        /// </summary>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="state">
        ///   <para>The object qualifying the state.</para>
        ///   <para>By default this is <see langword="null"/>.</para>
        /// </param>
        /// <returns>The number of rows affected.</returns>
        /// <exception cref="SqlProgramExecutionException">
        ///   An error occurred executing the program.
        /// </exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        [NotNull]
        public async Task<IEnumerable<int>> ExecuteNonQueryAllAsync([CanBeNull] Action<SqlProgramCommand> setParameters,
                                                                    object state = null)
        {
            return await ExecuteAllAsync(setParameters, c => c.ExecuteNonQueryAsync(state), state);
        }
        #endregion

        #region ExecuteReader and ExecuteReaderAll overloads
        /// <summary>
        ///   Executes the program with the specified parameters.
        /// </summary>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        public void ExecuteReader()
        {
            ExecuteNonQuery(null);
        }

        /// <summary>
        ///   Executes the program with the specified parameters against every connection.
        /// </summary>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        public void ExecuteReaderAll()
        {
            ExecuteNonQueryAll(null);
        }

        /// <summary>
        ///   Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <returns>The scalar value.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        [CanBeNull]
        public T ExecuteReader<T>()
        {
            return ExecuteScalar<T>(null);
        }

        /// <summary>
        ///   Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type of the results.</typeparam>
        /// <returns>
        ///   An enumerable containing the scalar value returned from each connection.
        /// </returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        [NotNull]
        public IEnumerable<T> ExecuteReaderAll<T>()
        {
            return ExecuteScalarAll<T>(null);
        }

        /// <summary>
        ///   Executes the program with the specified parameters.
        /// </summary>
        /// <param name="resultAction">
        ///   The action to use to process the results.
        /// </param>
        /// <param name="behavior">
        ///   The behaviour of the results and how they effect the database.
        /// </param>
        /// <exception cref="SqlProgramExecutionException">A
        ///   An error occurred executing the program.
        /// </exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        public void ExecuteReader([NotNull] Action<SqlDataReader> resultAction,
                                  CommandBehavior behavior = CommandBehavior.Default)
        {
            ExecuteReader(null, resultAction, behavior);
        }

        /// <summary>
        ///   Executes the program with the specified parameters.
        /// </summary>
        /// <param name="resultAction">
        ///   The action to use to process the results.
        /// </param>
        /// <param name="behavior">
        ///   The behaviour of the results and how they effect the database.
        /// </param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        public void ExecuteReaderAll([NotNull] Action<SqlDataReader> resultAction,
                                     CommandBehavior behavior = CommandBehavior.Default)
        {
            ExecuteReaderAll(null, resultAction, behavior);
        }

        /// <summary>
        ///   Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="resultFunc">
        ///   The function to use to process the results.
        /// </param>
        /// <param name="behavior">
        ///   The behaviour of the results and how they effect the database.
        /// </param>
        /// <returns>The result.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        [CanBeNull]
        public T ExecuteReader<T>([NotNull] Func<SqlDataReader, T> resultFunc,
                                  CommandBehavior behavior = CommandBehavior.Default)
        {
            return ExecuteReader(null, resultFunc, behavior);
        }

        /// <summary>
        ///   Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type of the results.</typeparam>
        /// <param name="resultFunc">
        ///   The function to use to process the results.
        /// </param>
        /// <param name="behavior">
        ///   The behaviour of the results and how they effect the database.
        /// </param>
        /// <returns>
        ///   An enumerable containing the results from all connections.
        /// </returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <exception cref="InvalidOperationException">
        ///   The connection did not specify a source or server.
        /// </exception>
        /// <exception cref="SqlException">
        ///   A connection-level error occurred whilst opening the connection.
        ///   See <see cref="SqlConnection.Open"/> for more details.
        /// </exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        [CanBeNull]
        public IEnumerable<T> ExecuteReaderAll<T>([NotNull] Func<SqlDataReader, T> resultFunc,
                                                  CommandBehavior behavior = CommandBehavior.Default)
        {
            return ExecuteReaderAll(null, resultFunc, behavior);
        }

        /// <summary>
        ///   Executes the program with the specified parameters.
        /// </summary>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="resultAction">
        ///   The action to use to process the results.
        /// </param>
        /// <param name="behavior">
        ///   The behaviour of the results and how they effect the database.
        /// </param>
        /// <exception cref="LoggingException">An error occurred executing the program.</exception>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        public void ExecuteReader([CanBeNull] Action<SqlProgramCommand> setParameters,
                                  [CanBeNull] Action<SqlDataReader> resultAction = null,
                                  CommandBehavior behavior = CommandBehavior.Default)
        {
            // If there's no action, we can execute non query!
            if (resultAction == null)
            {
                ExecuteNonQuery(setParameters);
                return;
            }

            using (SqlProgramCommand command = CreateCommand())
            {
                try
                {
                    if (setParameters != null)
                        setParameters(command);

                    using (SqlDataReader reader = command.ExecuteReader(behavior))
                    {
                        resultAction(reader);
                    }
                }
                catch (LoggingException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    throw new SqlProgramExecutionException(this, exception);
                }
            }
        }

        /// <summary>
        ///   Executes the program with the specified parameters against all connections.
        /// </summary>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="resultAction">
        ///   The action to use to process the results.
        /// </param>
        /// <param name="behavior">
        ///   The behaviour of the results and how they effect the database.
        /// </param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        public void ExecuteReaderAll([CanBeNull] Action<SqlProgramCommand> setParameters,
                                     [CanBeNull] Action<SqlDataReader> resultAction = null,
                                     CommandBehavior behavior = CommandBehavior.Default)
        {
            // If there's no action, we can execute non query!
            if (resultAction == null)
            {
                ExecuteNonQueryAll(setParameters);
                return;
            }

            IEnumerable<SqlProgramCommand> commands = CreateCommandsForAllConnections();
            try
            {
                foreach (SqlProgramCommand command in commands)
                {
                    if (setParameters != null)
                        setParameters(command);

                    using (SqlDataReader reader = command.ExecuteReader(behavior))
                    {
                        resultAction(reader);
                    }
                }
            }
            catch (LoggingException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new SqlProgramExecutionException(this, exception);
            }
            finally
            {
                foreach (SqlProgramCommand command in commands)
                    command.Dispose();
            }
        }

        /// <summary>
        ///   Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type to return from the result function</typeparam>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="behavior">
        ///   The behaviour of the results and how they effect the database.
        /// </param>
        /// <param name="resultFunc">
        ///   The function to use to process the results.
        /// </param>
        /// <returns>The result.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        [CanBeNull]
        public T ExecuteReader<T>([CanBeNull] Action<SqlProgramCommand> setParameters,
                                  [CanBeNull] Func<SqlDataReader, T> resultFunc = null,
                                  CommandBehavior behavior = CommandBehavior.Default)
        {
            // If there's no function, execute scalar.
            if (resultFunc == null)
                return ExecuteScalar<T>(setParameters);

            using (SqlProgramCommand command = CreateCommand())
            {
                try
                {
                    if (setParameters != null)
                        setParameters(command);

                    using (SqlDataReader reader = command.ExecuteReader(behavior))
                    {
                        return resultFunc(reader);
                    }
                }
                catch (LoggingException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    throw new SqlProgramExecutionException(this, exception);
                }
            }
        }

        /// <summary>
        ///   Executes the program with the specified parameters against all connections.
        /// </summary>
        /// <typeparam name="T">The type of the results.</typeparam>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="resultFunc">
        ///   The function to use to process the results.
        /// </param>
        /// <param name="behavior">
        ///   The behaviour of the results and how they effect the database.
        /// </param>
        /// <returns>
        ///   An enumerable containing the results from all the connections.
        /// </returns>
        /// <exception cref="SqlProgramExecutionException">
        ///   An error occurred executing the program.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///   The connection did not specify a source or server.
        /// </exception>
        /// <exception cref="SqlException">
        ///   A connection-level error occurred whilst opening the connection.
        ///   See <see cref="SqlConnection.Open"/> for more details.
        /// </exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        [NotNull]
        public IEnumerable<T> ExecuteReaderAll<T>([CanBeNull] Action<SqlProgramCommand> setParameters,
                                                  [CanBeNull] Func<SqlDataReader, T> resultFunc = null,
                                                  CommandBehavior behavior = CommandBehavior.Default)
        {
            // If there's no function, execute scalar.
            if (resultFunc == null)
                return ExecuteScalarAll<T>(setParameters);

            List<T> results = new List<T>();

            IEnumerable<SqlProgramCommand> commands = CreateCommandsForAllConnections();
            try
            {
                foreach (SqlProgramCommand command in commands)
                {
                    if (setParameters != null)
                        setParameters(command);

                    using (SqlDataReader reader = command.ExecuteReader(behavior))
                    {
                        results.Add(resultFunc(reader));
                    }
                }
            }
            catch (LoggingException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new SqlProgramExecutionException(this, exception);
            }
            finally
            {
                foreach (SqlProgramCommand command in commands)
                    command.Dispose();
            }

            return results;
        }
        #endregion

        #region ExecuteReaderAsync and ExecuteReaderAllAsync overloads
        /// <summary>
        ///   Executes the program with the specified parameters.
        /// </summary>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <exception cref="LoggingException">
        ///   Failed to select a valid connection string from the <see cref="LoadBalancedConnection"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///   The connection did not specify a source or server.
        /// </exception>
        /// <exception cref="SqlException">
        ///   A connection-level error occurred whilst opening the connection.
        ///   See <see cref="SqlConnection.Open"/> for more details.
        /// </exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        public async Task ExecuteReaderAsync(object state = null)
        {
            await ExecuteNonQueryAsync(null, state);
        }

        /// <summary>
        ///   Executes the program with the specified parameters.
        /// </summary>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        public async Task ExecuteReaderAllAsync(object state = null)
        {
            await ExecuteNonQueryAllAsync(null, state);
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <returns>The result.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        [CanBeNull]
        public async Task<T> ExecuteReaderAsync<T>(object state = null)
        {
            return await ExecuteScalarAsync<T>(null, state);
        }

        /// <summary>
        ///   Executes the program with the specified parameters against all connections.
        /// </summary>
        /// <typeparam name="T">The type of the results.</typeparam>
        /// <returns>An enumerable containing the results from all connections.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        [NotNull]
        public async Task<IEnumerable<T>> ExecuteReaderAllAsync<T>(object state = null)
        {
            return await ExecuteScalarAllAsync<T>(null, state);
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <param name="resultAction">The action used to process the result.</param>
        /// <param name="behavior">The query's effect on the database.</param>
        /// <param name="state">The object qualifying the state.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        public async Task ExecuteReaderAsync([NotNull] Action<SqlDataReader> resultAction,
                                             CommandBehavior behavior = CommandBehavior.Default,
                                             object state = null)
        {
            await ExecuteReaderAsync(null, resultAction, behavior, state);
        }

        /// <summary>
        ///   Executes the program with the specified parameters.
        /// </summary>
        /// <param name="resultAction">The action used to process the result.</param>
        /// <param name="behavior">The query's effect on the database.</param>
        /// <param name="state">The object qualifying the state.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        public async Task ExecuteReaderAllAsync([NotNull] Action<SqlDataReader> resultAction,
                                                CommandBehavior behavior = CommandBehavior.Default,
                                                object state = null)
        {
            await ExecuteReaderAllAsync(null, resultAction, behavior, state);
        }

        /// <summary>
        ///   Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="resultFunc">The function used to process the result.</param>
        /// <param name="behavior">The query's effect on the database.</param>
        /// <param name="state">The object qualifying the state.</param>
        /// <returns>The result.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        [CanBeNull]
        public async Task<T> ExecuteReaderAsync<T>([NotNull] Func<SqlDataReader, T> resultFunc,
                                                   CommandBehavior behavior = CommandBehavior.Default,
                                                   object state = null)
        {
            return await ExecuteReaderAsync(null, resultFunc, behavior, state);
        }

        /// <summary>
        ///   Executes the program with the specified parameters against all connections.
        /// </summary>
        /// <typeparam name="T">The type of the results.</typeparam>
        /// <param name="resultFunc">The function used to process the result.</param>
        /// <param name="behavior">The query's effect on the database.</param>
        /// <param name="state">The object qualifying the state.</param>
        /// <returns>An enumerable containing the results from all the connections.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        [CanBeNull]
        public async Task<IEnumerable<T>> ExecuteReaderAllAsync<T>([NotNull] Func<SqlDataReader, T> resultFunc,
                                                                   CommandBehavior behavior = CommandBehavior.Default,
                                                                   object state = null)
        {
            return await ExecuteReaderAllAsync(null, resultFunc, behavior, state);
        }

        /// <summary>
        ///   Executes the program with the specified parameters.
        /// </summary>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="resultAction">The action used to process the result.</param>
        /// <param name="behavior">The query's effect on the database.</param>
        /// <param name="state">The object qualifying the state.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        public async Task ExecuteReaderAsync([CanBeNull] Action<SqlProgramCommand> setParameters,
                                             [CanBeNull] Action<SqlDataReader> resultAction = null,
                                             CommandBehavior behavior = CommandBehavior.Default,
                                             object state = null)
        {
            // If there's no action, we can execute non query!
            if (resultAction == null)
                await ExecuteNonQueryAsync(setParameters, state);
            else
                await ExecuteAsync(new SqlProgramCommand(this, _connection.CreateConnection(), DefaultCommandTimeout),
                                   setParameters,
                                   c => c.ExecuteReaderAsync(behavior, state),
                                   r =>
                                       {
                                           try
                                           {
                                               resultAction(r);
                                               return true;
                                           }
                                           finally
                                           {
                                               r.Dispose();
                                           }
                                       }, state);
        }

        /// <summary>
        ///   Executes the program with the specified parameters.
        /// </summary>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="resultAction">The action used to process the result.</param>
        /// <param name="behavior">The query's effect on the database.</param>
        /// <param name="state">The object qualifying the state.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        public async Task ExecuteReaderAllAsync([CanBeNull] Action<SqlProgramCommand> setParameters,
                                                [CanBeNull] Action<SqlDataReader> resultAction = null,
                                                CommandBehavior behavior = CommandBehavior.Default,
                                                object state = null)
        {
            // If there's no action, we can execute non query!
            if (resultAction == null)
                await ExecuteNonQueryAllAsync(setParameters, state);
            else
                await ExecuteAllAsync(setParameters,
                                      c => c.ExecuteReaderAsync(behavior, state),
                                      r =>
                                          {
                                              try
                                              {
                                                  resultAction(r);
                                                  return true;
                                              }
                                              finally
                                              {
                                                  r.Dispose();
                                              }
                                          }, state);
        }

        /// <summary>
        ///   Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type to return from the result function</typeparam>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="resultFunc">The function used to process the result.</param>
        /// <param name="behavior">The query's effect on the database.</param>
        /// <param name="state">The object qualifying the state.</param>
        /// <returns>The result.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        [CanBeNull]
        public async Task<T> ExecuteReaderAsync<T>([CanBeNull] Action<SqlProgramCommand> setParameters,
                                                   [CanBeNull] Func<SqlDataReader, T> resultFunc = null,
                                                   CommandBehavior behavior = CommandBehavior.Default,
                                                   object state = null)
        {
            // If there's no function, execute scalar.
            if (resultFunc == null)
                return await ExecuteScalarAsync<T>(setParameters, state);

            return
                await ExecuteAsync(new SqlProgramCommand(this, _connection.CreateConnection(), DefaultCommandTimeout),
                                   setParameters,
                                   c => c.ExecuteReaderAsync(behavior, state),
                                   r =>
                                       {
                                           try
                                           {
                                               return resultFunc(r);
                                           }
                                           finally
                                           {
                                               r.Dispose();
                                           }
                                       }, state);
        }

        /// <summary>
        ///   Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type of the results.</typeparam>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="resultFunc">The function used to process the result.</param>
        /// <param name="behavior">The query's effect on the database.</param>
        /// <param name="state">The object qualifying the state.</param>
        /// <returns>An enumerable containing the results from all connections.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        [NotNull]
        public async Task<IEnumerable<T>> ExecuteReaderAllAsync<T>([CanBeNull] Action<SqlProgramCommand> setParameters,
                                                                   [CanBeNull] Func<SqlDataReader, T> resultFunc = null,
                                                                   CommandBehavior behavior = CommandBehavior.Default,
                                                                   object state = null)
        {
            // If there's no function, execute scalar.
            if (resultFunc == null)
                return await ExecuteScalarAllAsync<T>(setParameters, state);

            return await ExecuteAllAsync(setParameters,
                                         c => c.ExecuteReaderAsync(behavior, state),
                                         r =>
                                             {
                                                 try
                                                 {
                                                     return resultFunc(r);
                                                 }
                                                 finally
                                                 {
                                                     r.Dispose();
                                                 }
                                             }, state);
        }
        #endregion

        #region ExecuteXmlReader and ExecuteXmlReaderAll overloads
        /// <summary>
        ///   Executes the program with the specified parameters.
        /// </summary>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        public void ExecuteXmlReader()
        {
            ExecuteNonQuery(null);
        }

        /// <summary>
        ///   Executes the program with the specified parameters.
        /// </summary>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        public void ExecuteXmlReaderAll()
        {
            ExecuteNonQueryAll(null);
        }

        /// <summary>
        ///   Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <returns>The result.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        [CanBeNull]
        public T ExecuteXmlReader<T>()
        {
            return ExecuteScalar<T>(null);
        }

        /// <summary>
        ///   Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type of the results.</typeparam>
        /// <returns>An enumerable containing the result from all connections.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        [NotNull]
        public IEnumerable<T> ExecuteXmlReaderAll<T>()
        {
            return ExecuteScalarAll<T>(null);
        }

        /// <summary>
        ///   Executes the program with the specified parameters.
        /// </summary>
        /// <param name="resultAction">The result function.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        public void ExecuteXmlReader([NotNull] Action<XmlReader> resultAction)
        {
            ExecuteXmlReader(null, resultAction);
        }

        /// <summary>
        ///   Executes the program with the specified parameters against all connections.
        /// </summary>
        /// <param name="resultAction">The action to process the results.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        public void ExecuteXmlReaderAll([NotNull] Action<XmlReader> resultAction)
        {
            ExecuteXmlReaderAll(null, resultAction);
        }

        /// <summary>
        ///   Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="resultFunc">The function used to process the result.</param>
        /// <returns>The result.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        [CanBeNull]
        public T ExecuteXmlReader<T>([NotNull] Func<XmlReader, T> resultFunc)
        {
            return ExecuteXmlReader(null, resultFunc);
        }

        /// <summary>
        ///   Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="resultFunc">The function used to process the result.</param>
        /// <returns>The result.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        [CanBeNull]
        public IEnumerable<T> ExecuteXmlReaderAll<T>([NotNull] Func<XmlReader, T> resultFunc)
        {
            return ExecuteXmlReaderAll(null, resultFunc);
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="resultAction">The action to process the result.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        public void ExecuteXmlReader([CanBeNull] Action<SqlProgramCommand> setParameters,
                                     [CanBeNull] Action<XmlReader> resultAction = null)
        {
            // If there's no action, we can execute non query!
            if (resultAction == null)
            {
                ExecuteNonQuery(setParameters);
                return;
            }

            using (SqlProgramCommand command = CreateCommand())
            {
                try
                {
                    if (setParameters != null)
                        setParameters(command);

                    using (XmlReader reader = command.ExecuteXmlReader())
                    {
                        resultAction(reader);
                    }
                }
                catch (LoggingException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    throw new SqlProgramExecutionException(this, exception);
                }
            }
        }

        /// <summary>
        ///   Executes the program with the specified parameters.
        /// </summary>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="resultAction">The action to process the result.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        public void ExecuteXmlReaderAll([CanBeNull] Action<SqlProgramCommand> setParameters,
                                        [CanBeNull] Action<XmlReader> resultAction = null)
        {
            // If there's no action, we can execute non query!
            if (resultAction == null)
            {
                ExecuteNonQueryAll(setParameters);
                return;
            }

            IEnumerable<SqlProgramCommand> commands = CreateCommandsForAllConnections();
            try
            {
                foreach (SqlProgramCommand command in commands)
                {
                    if (setParameters != null)
                        setParameters(command);

                    using (XmlReader reader = command.ExecuteXmlReader())
                    {
                        resultAction(reader);
                    }
                }
            }
            catch (LoggingException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new SqlProgramExecutionException(this, exception);
            }
            finally
            {
                foreach (SqlProgramCommand command in commands)
                    command.Dispose();
            }
        }

        /// <summary>
        ///   Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type to return from the result function</typeparam>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="resultFunc">The function to process the result.</param>
        /// <returns>The result.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        [CanBeNull]
        public T ExecuteXmlReader<T>([CanBeNull] Action<SqlProgramCommand> setParameters,
                                     [CanBeNull] Func<XmlReader, T> resultFunc = null)
        {
            // If there's no function, execute scalar.
            if (resultFunc == null)
                return ExecuteScalar<T>(setParameters);

            using (SqlProgramCommand command = CreateCommand())
            {
                try
                {
                    if (setParameters != null)
                        setParameters(command);

                    using (XmlReader reader = command.ExecuteXmlReader())
                    {
                        return resultFunc(reader);
                    }
                }
                catch (LoggingException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    throw new SqlProgramExecutionException(this, exception);
                }
            }
        }

        /// <summary>
        ///   Executes the program with the specified parameters against all connections.
        /// </summary>
        /// <typeparam name="T">The type of the results.</typeparam>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="resultFunc">The function to process the result.</param>
        /// <returns>An enumerable containing the result from all connections.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        [NotNull]
        public IEnumerable<T> ExecuteXmlReaderAll<T>([CanBeNull] Action<SqlProgramCommand> setParameters,
                                                     [CanBeNull] Func<XmlReader, T> resultFunc = null)
        {
            // If there's no function, execute scalar.
            if (resultFunc == null)
                return ExecuteScalarAll<T>(setParameters);

            List<T> results = new List<T>();

            IEnumerable<SqlProgramCommand> commands = CreateCommandsForAllConnections();
            try
            {
                foreach (SqlProgramCommand command in commands)
                {
                    if (setParameters != null)
                        setParameters(command);

                    using (XmlReader reader = command.ExecuteXmlReader())
                    {
                        results.Add(resultFunc(reader));
                    }
                }
            }
            catch (LoggingException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new SqlProgramExecutionException(this, exception);
            }
            finally
            {
                foreach (SqlProgramCommand command in commands)
                    command.Dispose();
            }

            return results;
        }
        #endregion

        #region ExecuteXmlReaderAsync and ExecuteXmlReaderAllAsync overloads
        /// <summary>
        ///   Executes the program with the specified parameters.
        /// </summary>
        /// <param name="state">The object qualifying the state.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        public async Task ExecuteXmlReaderAsync(object state = null)
        {
            await ExecuteNonQueryAsync(null, state);
        }

        /// <summary>
        ///   Executes the program with the specified parameters.
        /// </summary>
        /// <param name="state">The object qualifying the state.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        public async Task ExecuteXmlReaderAllAsync(object state = null)
        {
            await ExecuteNonQueryAllAsync(null, state);
        }

        /// <summary>
        ///   Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="state">The object qualifying the state.</param>
        /// <returns>The result.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        [CanBeNull]
        public async Task<T> ExecuteXmlReaderAsync<T>(object state = null)
        {
            return await ExecuteScalarAsync<T>(null, state);
        }

        /// <summary>
        ///   Executes the program with the specified parameters against all connections.
        /// </summary>
        /// <typeparam name="T">The type of the results.</typeparam>
        /// <returns>An enumerable containing the result for all connections.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        [NotNull]
        public async Task<IEnumerable<T>> ExecuteXmlReaderAllAsync<T>(object state = null)
        {
            return await ExecuteScalarAllAsync<T>(null, state);
        }

        /// <summary>
        ///   Executes the program with the specified parameters.
        /// </summary>
        /// <param name="resultAction">The action to process the result.</param>
        /// <param name="state">The object qualifying the state.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        public async Task ExecuteXmlReaderAsync([NotNull] Action<XmlReader> resultAction,
                                                object state = null)
        {
            await ExecuteXmlReaderAsync(null, resultAction, state);
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <param name="resultAction">The action to process the result.</param>
        /// <param name="state">The object qualifying the state.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        public async Task ExecuteXmlReaderAllAsync([NotNull] Action<XmlReader> resultAction,
                                                   object state = null)
        {
            await ExecuteXmlReaderAllAsync(null, resultAction, state);
        }

        /// <summary>
        ///   Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="resultFunc">The function to process the result.</param>
        /// <param name="state">The object qualifying the state.</param>
        /// <returns>The result.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        [CanBeNull]
        public async Task<T> ExecuteXmlReaderAsync<T>([NotNull] Func<XmlReader, T> resultFunc,
                                                      object state = null)
        {
            return await ExecuteXmlReaderAsync(null, resultFunc, state);
        }

        /// <summary>
        ///   Executes the program with the specified parameters against all connections.
        /// </summary>
        /// <typeparam name="T">The type of the results.</typeparam>
        /// <param name="resultFunc">The function to process the result.</param>
        /// <param name="state">The object qualifying the state.</param>
        /// <returns>An enumerable containing the result for all connections.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        [CanBeNull]
        public async Task<IEnumerable<T>> ExecuteXmlReaderAllAsync<T>([NotNull] Func<XmlReader, T> resultFunc,
                                                                      object state = null)
        {
            return await ExecuteXmlReaderAllAsync(null, resultFunc, state);
        }

        /// <summary>
        ///   Executes the program with the specified parameters.
        /// </summary>
        /// <param name="setParameters">The action to set the program parameters.</param>
        /// <param name="resultAction">The action to process the result.</param>
        /// <param name="state">The object qualifying the state.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        public async Task ExecuteXmlReaderAsync([CanBeNull] Action<SqlProgramCommand> setParameters,
                                                [CanBeNull] Action<XmlReader> resultAction = null, object state = null)
        {
            // If there's no action, we can execute non query!
            if (resultAction == null)
                await ExecuteNonQueryAsync(setParameters, state);
            else
                await ExecuteAsync(new SqlProgramCommand(this, _connection.CreateConnection(), DefaultCommandTimeout),
                                   setParameters,
                                   c => c.ExecuteXmlReaderAsync(state),
                                   r =>
                                       {
                                           try
                                           {
                                               resultAction(r);
                                               return true;
                                           }
                                           finally
                                           {
                                               r.Close();
                                           }
                                       }, state);
        }

        /// <summary>
        ///   Executes the program with the specified parameters.
        /// </summary>
        /// <param name="setParameters">The action to set the program parameters.</param>
        /// <param name="resultAction">The action to process the result.</param>
        /// <param name="state">The object qualifying the state.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        public async Task ExecuteXmlReaderAllAsync([CanBeNull] Action<SqlProgramCommand> setParameters,
                                                   [CanBeNull] Action<XmlReader> resultAction = null,
                                                   object state = null)
        {
            // If there's no action, we can execute non query!
            if (resultAction == null)
                await ExecuteNonQueryAllAsync(setParameters, state);
            else
                await ExecuteAllAsync(setParameters, c => c.ExecuteXmlReaderAsync(state),
                                      r =>
                                          {
                                              try
                                              {
                                                  resultAction(r);
                                                  return true;
                                              }
                                              finally
                                              {
                                                  r.Close();
                                              }
                                          }, state);
        }

        /// <summary>
        ///   Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type to return from the result function.</typeparam>
        /// <param name="setParameters">The action to set the program parameters.</param>
        /// <param name="resultFunc">The function to process the result.</param>
        /// <param name="state">The object qualifying the state.</param>
        /// <returns>The result.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        [CanBeNull]
        public async Task<T> ExecuteXmlReaderAsync<T>([CanBeNull] Action<SqlProgramCommand> setParameters,
                                                      [CanBeNull] Func<XmlReader, T> resultFunc = null,
                                                      object state = null)
        {
            // If there's no function, execute scalar.
            if (resultFunc == null)
                return await ExecuteScalarAsync<T>(setParameters, state);

            return
                await ExecuteAsync(new SqlProgramCommand(this, _connection.CreateConnection(), DefaultCommandTimeout),
                                   setParameters,
                                   c => c.ExecuteXmlReaderAsync(state),
                                   r =>
                                       {
                                           try
                                           {
                                               return resultFunc(r);
                                           }
                                           finally
                                           {
                                               r.Close();
                                           }
                                       }, state);
        }

        /// <summary>
        ///   Executes the program with the specified parameters against all connections.
        /// </summary>
        /// <typeparam name="T">The type of the results.</typeparam>
        /// <param name="setParameters">The action to set the program parameters.</param>
        /// <param name="resultFunc">The function to process the result.</param>
        /// <param name="state">The object qualifying the state.</param>
        /// <returns>An enumerable containing the result for all connections.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [UsedImplicitly]
        [NotNull]
        public async Task<IEnumerable<T>> ExecuteXmlReaderAllAsync<T>(
            [CanBeNull] Action<SqlProgramCommand> setParameters,
            [CanBeNull] Func<XmlReader, T> resultFunc = null,
            object state = null)
        {
            // If there's no function, execute scalar.
            if (resultFunc == null)
                return await ExecuteScalarAllAsync<T>(setParameters, state);

            return await ExecuteAllAsync(setParameters,
                                         c => c.ExecuteXmlReaderAsync(state),
                                         r =>
                                             {
                                                 try
                                                 {
                                                     return resultFunc(r);
                                                 }
                                                 finally
                                                 {
                                                     r.Close();
                                                 }
                                             }, state);
        }
        #endregion
    }
}