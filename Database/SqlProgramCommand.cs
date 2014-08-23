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
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using JetBrains.Annotations;
using WebApplications.Utilities.Database.Exceptions;
using WebApplications.Utilities.Database.Schema;
using WebApplications.Utilities.Logging;

namespace WebApplications.Utilities.Database
{
    /// <summary>
    ///   A specialised command that allows finer grained control when using <see cref="SqlProgram"/>s.
    /// </summary>
    public partial class SqlProgramCommand
    {
        /// <summary>
        /// Function to create a <see cref="SqlParameterCollection"/>.
        /// </summary>
        [NotNull]
        private static readonly Func<SqlParameterCollection> _createSqlParameterCollection =
            typeof (SqlParameterCollection).ConstructorFunc<SqlParameterCollection>();

        /// <summary>
        /// Allows rapid setting of a commands parameters.
        /// </summary>
        [NotNull]
        private static readonly Action<SqlCommand, SqlParameterCollection> _setSqlParameterCollection =
            typeof (SqlCommand).GetSetter<SqlCommand, SqlParameterCollection>("_parameters");

        [NotNull]
        private readonly string _connectionString;

        [NotNull]
        private readonly SqlProgram _program;

        [NotNull]
        private readonly SqlParameterCollection _parameters;

        private TimeSpan _commandTimeout;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlProgramCommand" /> class.
        /// </summary>
        /// <param name="program">The SQL program.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="commandTimeout">The time to wait for the program to execute before raising an error.</param>
        internal SqlProgramCommand(
            [NotNull] SqlProgram program,
            [NotNull] string connectionString,
            TimeSpan commandTimeout)
        {
            Contract.Requires(program != null);
            Contract.Requires(connectionString != null);
            Contract.Requires(commandTimeout >= TimeSpan.Zero);
            _program = program;
            _connectionString = connectionString;
            CommandTimeout = commandTimeout;
            // ReSharper disable once AssignNullToNotNullAttribute
            _parameters = _createSqlParameterCollection();
        }

        /// <summary>
        ///   Gets or sets the command timeout.
        ///   This is the time to wait for the program to execute.
        /// </summary>
        /// <value>
        ///   The time to wait (in seconds) for the command to execute.
        /// </value>
        [UsedImplicitly]
        public TimeSpan CommandTimeout
        {
            get { return _commandTimeout; }
            set
            {
                if (_commandTimeout == value)
                    return;
                _commandTimeout = value < TimeSpan.Zero
                    ? TimeSpan.FromSeconds(30)
                    : value;
            }
        }

        /// <summary>
        /// Gets the parameter with the specified name and returns it as an <see cref="SqlParameter" /> object.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to retrieve.</param>
        /// <returns>The <see cref="SqlParameter" /> that corresponds to the <paramref name="parameterName" /> provided.</returns>
        /// <exception cref="WebApplications.Utilities.Logging.LoggingException">Could not find a match with the <paramref name="parameterName" /> specified.</exception>
        [NotNull]
        [UsedImplicitly]
        public SqlParameter GetParameter([NotNull] string parameterName)
        {
            Contract.Requires(parameterName != null);
            Contract.Ensures(Contract.Result<SqlParameter>() != null);

            lock (_parameters)
            {
                parameterName = parameterName.ToLower();

                int index = _parameters.IndexOf(parameterName);
                SqlParameter parameter;
                if (index < 0)
                {
                    // Parameter not added yet
                    SqlProgramParameter parameterDefinition;
                    if (!_program.Definition.TryGetParameter(parameterName, out parameterDefinition))
                        throw new LoggingException(
                            LoggingLevel.Critical,
                            () => Resources.SqlProgramCommand_GetParameter_ProgramDoesNotHaveParameter,
                            _program.Name,
                            parameterName);

                    // Create the parameter and add it to the collection
                    Contract.Assert(parameterDefinition != null);
                    parameter = _parameters.Add(parameterDefinition.CreateSqlParameter());
                }
                else
                    // Get the parameter and set it's value.
                    parameter = _parameters[index];

                // Create the parameter and add it to the collection
                Contract.Assert(parameter != null);
                return parameter;
            }
        }

        /// <summary>
        /// Sets the specified parameter with the value provided and returns it as an <see cref="SqlParameter" /> object.
        /// </summary>
        /// <typeparam name="T">The type of the value to set.</typeparam>
        /// <param name="parameterName">The name of the parameter to set.</param>
        /// <param name="value">The value to set the parameter to.</param>
        /// <param name="mode"><para>The constraint mode.</para>
        /// <para>By default this is set to give a warning if truncation/loss of precision occurs.</para></param>
        /// <returns>The SqlParameter with the specified name.</returns>
        /// <exception cref="WebApplications.Utilities.Logging.LoggingException">Could not find a match with the <paramref name="parameterName" /> specified.</exception>
        [NotNull]
        [PublicAPI]
        public SqlParameter SetParameter<T>(
            [NotNull] string parameterName,
            T value,
            TypeConstraintMode mode = TypeConstraintMode.Warn)
        {
            Contract.Requires(parameterName != null);

            lock (_parameters)
            {
                parameterName = parameterName.ToLower();

                // Find parameter definition
                SqlProgramParameter parameterDefinition;
                if (!_program.Definition.TryGetParameter(parameterName, out parameterDefinition))
                    throw new LoggingException(
                        LoggingLevel.Critical,
                        () => Resources.SqlProgramCommand_SetParameter_ProgramDoesNotHaveParameter,
                        _program.Name,
                        parameterName);
                Contract.Assert(parameterDefinition != null);

                // Find or create SQL Parameter.
                int index = _parameters.IndexOf(parameterName);
                SqlParameter parameter = index < 0
                    ? _parameters.Add(parameterDefinition.CreateSqlParameter())
                    : _parameters[index];

                Contract.Assert(parameter != null);
                parameter.Value = parameterDefinition.CastCLRValue(value, mode);
                return parameter;
            }
        }

        /// <summary>
        ///   Executes the query and returns the first column of the first row in the result set returned by the query.
        ///   Any additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="T">The type of the value in the first column.</typeparam>
        /// <returns>
        ///   The first column of the first row in the result set, or <see langword="null"/> if the result set is empty.
        ///   Returns a maximum of 2033 characters.
        /// </returns>
        /// <exception cref="SqlException">
        ///   An exception occurred whilst executing the command against a locked row.
        ///   This exception is not generated when using Microsoft .NET Framework version 1.0.
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
        public T ExecuteScalar<T>()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(_program.Name, connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = (int) CommandTimeout.TotalSeconds
                    })
                    {
                        _setSqlParameterCollection(sqlCommand, _parameters);
                        return (T) sqlCommand.ExecuteScalar();
                    }
                }
            }
            catch (Exception exception)
            {
                throw new SqlProgramExecutionException(_program, exception);
            }
        }

        /// <summary>
        /// Creates a <see cref="Task&lt;TResult&gt;">Task&lt;T&gt;</see> that executes an operation asynchronously,
        /// returning the first column of the first row in the result set. Any additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="T">The type of the value in the result (the value first column of the first row).</typeparam>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable task, containing the scalar result.</returns>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        public async Task<T> ExecuteScalarAsync<T>(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    // ReSharper disable once PossibleNullReferenceException
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    using (SqlCommand sqlCommand = new SqlCommand(_program.Name, connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = (int) CommandTimeout.TotalSeconds
                    })
                    {
                        _setSqlParameterCollection(sqlCommand, _parameters);
                        // ReSharper disable once PossibleNullReferenceException
                        return (T) await sqlCommand.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception exception)
            {
                throw new SqlProgramExecutionException(_program, exception);
            }
        }

        /// <summary>
        /// Executes a Transact-SQL statement against the connection and returns the number of rows affected.
        /// </summary>
        /// <returns>The number of rows affected.</returns>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [PublicAPI]
        public int ExecuteNonQuery()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(_program.Name, connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = (int) CommandTimeout.TotalSeconds
                    })
                    {
                        _setSqlParameterCollection(sqlCommand, _parameters);
                        return sqlCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception exception)
            {
                throw new SqlProgramExecutionException(_program, exception);
            }
        }

        /// <summary>
        /// Creates a <see cref="Task&lt;TResult&gt;">Task&lt;int&gt;</see> that executes an non-query asynchronously,
        /// and returns the number of rows affected.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable task, containing the number of rows affected.</returns>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        [PublicAPI]
        public async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    // ReSharper disable once PossibleNullReferenceException
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    using (SqlCommand sqlCommand = new SqlCommand(_program.Name, connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = (int) CommandTimeout.TotalSeconds
                    })
                    {
                        _setSqlParameterCollection(sqlCommand, _parameters);
                        // ReSharper disable once PossibleNullReferenceException
                        return await sqlCommand.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception exception)
            {
                throw new SqlProgramExecutionException(_program, exception);
            }
        }

        #region ExecuteReader
        /// <summary>
        /// Executes the <see cref="SqlCommand.CommandText" /> to the <see cref="SqlCommand.Connection" />,
        /// and builds a <see cref="SqlDataReader" /> using the provided <see cref="CommandBehavior">behavior</see>.
        /// </summary>
        /// <param name="resultAction">The action to use to process the results.</param>
        /// <param name="behavior"><para>Describes the results of the query and its effect on the database.</para>
        /// <para>By default this is set to CommandBehavior.Default.</para></param>
        /// <returns>The built <see cref="SqlDataReader" /> object.</returns>
        /// <exception cref="WebApplications.Utilities.Database.Exceptions.SqlProgramExecutionException"></exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [PublicAPI]
        public void ExecuteReader(
            [NotNull] ResultDelegate resultAction,
            CommandBehavior behavior = CommandBehavior.Default)
        {
            Contract.Requires(resultAction != null);
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(_program.Name, connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = (int) CommandTimeout.TotalSeconds
                    })
                    {
                        _setSqlParameterCollection(sqlCommand, _parameters);
                        using (SqlDataReader reader = sqlCommand.ExecuteReader(behavior))
                            resultAction(reader);
                    }
                }
            }
            catch (Exception exception)
            {
                throw new SqlProgramExecutionException(_program, exception);
            }
        }

        /// <summary>
        /// Executes the <see cref="SqlCommand.CommandText" /> to the <see cref="SqlCommand.Connection" />,
        /// and builds a <see cref="SqlDataReader" /> using the provided <see cref="CommandBehavior">behavior</see>.
        /// </summary>
        /// <param name="resultFunc">The function to use to process the results.</param>
        /// <param name="behavior"><para>Describes the results of the query and its effect on the database.</para>
        /// <para>By default this is set to CommandBehavior.Default.</para></param>
        /// <returns>The built <see cref="SqlDataReader" /> object.</returns>
        /// <exception cref="WebApplications.Utilities.Database.Exceptions.SqlProgramExecutionException"></exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [PublicAPI]
        public T ExecuteReader<T>(
            [NotNull] ResultDelegate<T> resultFunc,
            CommandBehavior behavior = CommandBehavior.Default)
        {
            Contract.Requires(resultFunc != null);
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(_program.Name, connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = (int) CommandTimeout.TotalSeconds
                    })
                    {
                        _setSqlParameterCollection(sqlCommand, _parameters);
                        using (SqlDataReader reader = sqlCommand.ExecuteReader(behavior))
                            return resultFunc(reader);
                    }
                }
            }
            catch (Exception exception)
            {
                throw new SqlProgramExecutionException(_program, exception);
            }
        }

        /// <summary>
        /// Creates a <see cref="Task&lt;TResult&gt;">Task&lt;SqlDataReader&gt;</see> that executes an operation asynchronously
        /// </summary>
        /// <param name="resultAction">The result action.</param>
        /// <param name="behavior"><para>Describes the results of the query and its effect on the database.</para>
        /// <para>By default this is set to CommandBehavior.Default.</para></param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable task, containing a <see cref="SqlDataReader" />.</returns>
        /// <exception cref="WebApplications.Utilities.Database.Exceptions.SqlProgramExecutionException"></exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        [PublicAPI]
        public async Task ExecuteReaderAsync(
            [NotNull] ResultDelegateAsync resultAction,
            CommandBehavior behavior = CommandBehavior.Default,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(resultAction != null);
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    // ReSharper disable PossibleNullReferenceException
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    using (SqlCommand sqlCommand = new SqlCommand(_program.Name, connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = (int) CommandTimeout.TotalSeconds
                    })
                    {
                        _setSqlParameterCollection(sqlCommand, _parameters);
                        using (
                            SqlDataReader reader =
                                await sqlCommand.ExecuteReaderAsync(behavior, cancellationToken).ConfigureAwait(false))
                            await resultAction(reader, cancellationToken).ConfigureAwait(false);
                    }
                    // ReSharper restore PossibleNullReferenceException
                }
            }
            catch (Exception exception)
            {
                throw new SqlProgramExecutionException(_program, exception);
            }
        }

        /// <summary>
        /// Creates a <see cref="Task&lt;TResult&gt;">Task&lt;SqlDataReader&gt;</see> that executes an operation asynchronously
        /// </summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="resultFunc">The result function.</param>
        /// <param name="behavior"><para>Describes the results of the query and its effect on the database.</para>
        /// <para>By default this is set to CommandBehavior.Default.</para></param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable task, containing a <see cref="SqlDataReader" />.</returns>
        /// <exception cref="WebApplications.Utilities.Database.Exceptions.SqlProgramExecutionException"></exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        [PublicAPI]
        public async Task<T> ExecuteReaderAsync<T>(
            [NotNull] ResultDelegateAsync<T> resultFunc,
            CommandBehavior behavior = CommandBehavior.Default,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(resultFunc != null);
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    // ReSharper disable PossibleNullReferenceException
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    using (SqlCommand sqlCommand = new SqlCommand(_program.Name, connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = (int) CommandTimeout.TotalSeconds
                    })
                    {
                        _setSqlParameterCollection(sqlCommand, _parameters);
                        using (
                            SqlDataReader reader =
                                await sqlCommand.ExecuteReaderAsync(behavior, cancellationToken).ConfigureAwait(false))
                            return await resultFunc(reader, cancellationToken).ConfigureAwait(false);
                    }
                    // ReSharper restore PossibleNullReferenceException
                }
            }
            catch (Exception exception)
            {
                throw new SqlProgramExecutionException(_program, exception);
            }
        }
        #endregion

        #region ExecuteXmlReader
        /// <summary>
        /// Executes the <see cref="SqlCommand.CommandText" /> to the <see cref="SqlCommand.Connection" />,
        /// and builds a <see cref="XmlReader" /> using the provided <see cref="CommandBehavior">behavior</see>.
        /// </summary>
        /// <param name="resultAction">The action to use to process the results.</param>
        /// <returns>The built <see cref="XmlReader" /> object.</returns>
        /// <exception cref="WebApplications.Utilities.Database.Exceptions.SqlProgramExecutionException"></exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [PublicAPI]
        public void ExecuteXmlReader([NotNull] XmlResultDelegate resultAction)
        {
            Contract.Requires(resultAction != null);
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(_program.Name, connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = (int) CommandTimeout.TotalSeconds
                    })
                    {
                        _setSqlParameterCollection(sqlCommand, _parameters);
                        using (XmlReader reader = sqlCommand.ExecuteXmlReader())
                            resultAction(reader);
                    }
                }
            }
            catch (Exception exception)
            {
                throw new SqlProgramExecutionException(_program, exception);
            }
        }

        /// <summary>
        /// Executes the <see cref="SqlCommand.CommandText" /> to the <see cref="SqlCommand.Connection" />,
        /// and builds a <see cref="XmlReader" /> using the provided <see cref="CommandBehavior">behavior</see>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resultFunc">The function to use to process the results.</param>
        /// <returns>The built <see cref="XmlReader" /> object.</returns>
        /// <exception cref="WebApplications.Utilities.Database.Exceptions.SqlProgramExecutionException"></exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [PublicAPI]
        public T ExecuteXmlReader<T>([NotNull] XmlResultDelegate<T> resultFunc)
        {
            Contract.Requires(resultFunc != null);
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(_program.Name, connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = (int) CommandTimeout.TotalSeconds
                    })
                    {
                        _setSqlParameterCollection(sqlCommand, _parameters);
                        using (XmlReader reader = sqlCommand.ExecuteXmlReader())
                            return resultFunc(reader);
                    }
                }
            }
            catch (Exception exception)
            {
                throw new SqlProgramExecutionException(_program, exception);
            }
        }

        /// <summary>
        /// Creates a <see cref="Task&lt;TXmlResult&gt;">Task&lt;XmlReader&gt;</see> that executes an operation asynchronously
        /// </summary>
        /// <param name="resultAction">The result action.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable task, containing a <see cref="XmlReader" />.</returns>
        /// <exception cref="WebApplications.Utilities.Database.Exceptions.SqlProgramExecutionException"></exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        [PublicAPI]
        public async Task ExecuteXmlReaderAsync(
            [NotNull] XmlResultDelegateAsync resultAction,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(resultAction != null);
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    // ReSharper disable PossibleNullReferenceException
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    using (SqlCommand sqlCommand = new SqlCommand(_program.Name, connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = (int) CommandTimeout.TotalSeconds
                    })
                    {
                        _setSqlParameterCollection(sqlCommand, _parameters);
                        using (
                            XmlReader reader =
                                await sqlCommand.ExecuteXmlReaderAsync(cancellationToken).ConfigureAwait(false))
                            await resultAction(reader, cancellationToken).ConfigureAwait(false);
                    }
                    // ReSharper restore PossibleNullReferenceException
                }
            }
            catch (Exception exception)
            {
                throw new SqlProgramExecutionException(_program, exception);
            }
        }

        /// <summary>
        /// Creates a <see cref="Task&lt;TXmlResult&gt;">Task&lt;XmlReader&gt;</see> that executes an operation asynchronously
        /// </summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="resultFunc">The result function.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable task, containing a <see cref="XmlReader" />.</returns>
        /// <exception cref="WebApplications.Utilities.Database.Exceptions.SqlProgramExecutionException"></exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        [PublicAPI]
        public async Task<T> ExecuteXmlReaderAsync<T>(
            [NotNull] XmlResultDelegateAsync<T> resultFunc,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(resultFunc != null);
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    // ReSharper disable PossibleNullReferenceException
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    using (SqlCommand sqlCommand = new SqlCommand(_program.Name, connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = (int) CommandTimeout.TotalSeconds
                    })
                    {
                        _setSqlParameterCollection(sqlCommand, _parameters);
                        using (
                            XmlReader reader =
                                await sqlCommand.ExecuteXmlReaderAsync(cancellationToken).ConfigureAwait(false))
                            return await resultFunc(reader, cancellationToken).ConfigureAwait(false);
                    }
                    // ReSharper restore PossibleNullReferenceException
                }
            }
            catch (Exception exception)
            {
                throw new SqlProgramExecutionException(_program, exception);
            }
        }
        #endregion

        /// <summary>
        ///   Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        ///   A <see cref="string"/> representation of this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("A SqlProgramCommand for the '{0}' SqlProgram", _program.Name);
        }
    }
}