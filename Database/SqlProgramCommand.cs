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
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Database.Exceptions;
using WebApplications.Utilities.Database.Schema;
using WebApplications.Utilities.Logging;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.Database
{
    /// <summary>
    ///   A specialised command that allows finer grained control when using <see cref="SqlProgram"/>s.
    /// </summary>
    [PublicAPI]
    public partial class SqlProgramCommand
    {
        /// <summary>
        /// Class CommandDisposer implements <see cref="IDisposable"/> and allows for manual disposal of resources.
        /// </summary>
        /// <seealso cref="System.IDisposable" />
        private class ReaderDisposer<TReader> : IDisposable
            where TReader: class, IDisposable
        {
            /// <summary>
            /// The semaphore.
            /// </summary>
            [CanBeNull]
            public IDisposable Semaphore;

            /// <summary>
            /// The connection.
            /// </summary>
            [CanBeNull]
            public SqlConnection Connection;

            /// <summary>
            /// The command.
            /// </summary>
            [CanBeNull]
            public SqlCommand Command;

            /// <summary>
            /// The reader.
            /// </summary>
            [CanBeNull]
            public TReader Reader;

            /// <summary>
            /// The registration for cancellation.
            /// </summary>
            [CanBeNull]
            private IDisposable _registration;

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                Interlocked.Exchange(ref _registration, null)?.Dispose();
                Interlocked.Exchange(ref Reader, null)?.Dispose();
                Interlocked.Exchange(ref Command, null)?.Dispose();
                Interlocked.Exchange(ref Connection, null)?.Dispose();
                Interlocked.Exchange(ref Semaphore, null)?.Dispose();
            }

            /// <summary>
            /// Sets a cancellation token that will trigger disposal.
            /// </summary>
            /// <param name="cancellationToken">The cancellation token.</param>
            /// <returns>CancellationToken.</returns>
            public CancellationToken SetCancellationToken(CancellationToken cancellationToken)
            {
                if (cancellationToken.CanBeCanceled)
                    _registration = cancellationToken.Register(Dispose);

                return cancellationToken;
            }
        }

        /// <summary>
        /// Creates a cancellation token that will eventually timeout if there is a command timeout.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>CancellationToken.</returns>
        private CancellationToken CreateCancellationToken(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (CommandTimeout > TimeSpan.Zero)
                cancellationToken =
                    cancellationToken.WithTimeout(CommandTimeout.Add(AdditionalCancellationTime))
                        .Token;
            return cancellationToken;
        }

        /// <summary>
        /// Function to create a <see cref="SqlParameterCollection"/>.
        /// </summary>
        [NotNull]
        private static readonly Func<SqlParameterCollection> _createSqlParameterCollection =
            typeof(SqlParameterCollection).ConstructorFunc<SqlParameterCollection>();

        /// <summary>
        /// Allows rapid setting of a commands parameters.
        /// </summary>
        [NotNull]
        private static readonly Action<SqlCommand, SqlParameterCollection> _setSqlParameterCollection =
            // ReSharper disable once AssignNullToNotNullAttribute
            typeof(SqlCommand).GetSetter<SqlCommand, SqlParameterCollection>("_parameters");

        [NotNull]
        private readonly SqlProgramMapping _mapping;

        [NotNull]
        private readonly SqlProgram _program;

        [NotNull]
        private readonly SqlParameterCollection _parameters;

        private TimeSpan _commandTimeout;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlProgramCommand" /> class.
        /// </summary>
        /// <param name="program">The SQL program.</param>
        /// <param name="mapping">The mapping.</param>
        /// <param name="commandTimeout">The time to wait for the program to execute before raising an error.</param>
        internal SqlProgramCommand(
            [NotNull] SqlProgram program,
            [NotNull] SqlProgramMapping mapping,
            TimeSpan commandTimeout)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));
            if (commandTimeout < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(commandTimeout));

            _program = program;
            _mapping = mapping;
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
        /// The additional time given to a command before a cancellation is triggered.
        /// </summary>
        /// <remarks>
        /// <para>All commands, even manually disposed ones, will be cleaned up if the <see cref="CommandTimeout"/> is greater
        /// than <see cref="TimeSpan.Zero"/> and the <see cref="CommandTimeout"/> plus the
        /// <see cref="AdditionalCancellationTime"/> has elapsed.</para>
        /// <para>This ensures that resources don't leak over time, by badly written consumers.</para>
        /// </remarks>
        public static readonly TimeSpan AdditionalCancellationTime = TimeSpan.FromSeconds(1);

        /// <summary>
        /// Gets the parameter with the specified name and returns it as an <see cref="SqlParameter" /> object.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to retrieve.</param>
        /// <returns>The <see cref="SqlParameter" /> that corresponds to the <paramref name="parameterName" /> provided.</returns>
        /// <exception cref="WebApplications.Utilities.Logging.LoggingException">Could not find a match with the <paramref name="parameterName" /> specified.</exception>
        [NotNull]
        public SqlParameter GetParameter([NotNull] string parameterName)
        {
            if (parameterName == null) throw new ArgumentNullException(nameof(parameterName));

            lock (_parameters)
            {
                parameterName = parameterName.ToLower();

                int index = _parameters.IndexOf(parameterName);
                SqlParameter parameter;
                if (index < 0)
                {
                    // Parameter not added yet
                    SqlProgramParameter parameterDefinition;
                    if (!_mapping.Definition.TryGetParameter(parameterName, out parameterDefinition))
                        throw new LoggingException(
                            LoggingLevel.Critical,
                            () => Resources.SqlProgramCommand_GetParameter_ProgramDoesNotHaveParameter,
                            _program.Name,
                            parameterName);

                    // Create the parameter and add it to the collection
                    Debug.Assert(parameterDefinition != null);
                    parameter = _parameters.Add(parameterDefinition.CreateSqlParameter());
                }
                else
                // Get the parameter and set it's value.
                    parameter = _parameters[index];

                // Create the parameter and add it to the collection
                Debug.Assert(parameter != null);
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
        public SqlParameter SetParameter<T>(
            [NotNull] string parameterName,
            T value,
            TypeConstraintMode mode = TypeConstraintMode.Warn)
        {
            if (parameterName == null) throw new ArgumentNullException(nameof(parameterName));

            lock (_parameters)
            {
                parameterName = parameterName.ToLower();

                // Find parameter definition
                SqlProgramParameter parameterDefinition;
                if (!_mapping.Definition.TryGetParameter(parameterName, out parameterDefinition))
                    throw new LoggingException(
                        LoggingLevel.Critical,
                        () => Resources.SqlProgramCommand_SetParameter_ProgramDoesNotHaveParameter,
                        _program.Name,
                        parameterName);
                Debug.Assert(parameterDefinition != null);

                // Find or create SQL Parameter.
                int index = _parameters.IndexOf(parameterName);
                SqlParameter parameter = index < 0
                    ? _parameters.Add(parameterDefinition.CreateSqlParameter())
                    : _parameters[index];

                Debug.Assert(parameter != null);
                parameter.Value = parameterDefinition.CastCLRValue(value, mode);
                return parameter;
            }
        }

        /// <summary>
        /// Waits the concurrency control semaphores.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        [NotNull]
        private Task<IDisposable> WaitSemaphoresAsync(CancellationToken token = default(CancellationToken))
        {
            return AsyncSemaphore.WaitAllAsync(
                token,
                _program.Semaphore,
                _mapping.Connection.Semaphore,
                _program.Connection.ConnectionSemaphore,
                _program.Connection.DatabaseSemaphore);
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
                using (WaitSemaphoresAsync().Result)
                using (SqlConnection connection = new SqlConnection(_mapping.Connection.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(_program.Name, connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = (int)CommandTimeout.TotalSeconds
                    })
                    {
                        _setSqlParameterCollection(sqlCommand, _parameters);
                        return (T)sqlCommand.ExecuteScalar();
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
                using (await WaitSemaphoresAsync(cancellationToken).ConfigureAwait(false))
                using (SqlConnection connection = new SqlConnection(_mapping.Connection.ConnectionString))
                {
                    // ReSharper disable once PossibleNullReferenceException
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    using (SqlCommand sqlCommand = new SqlCommand(_program.Name, connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = (int)CommandTimeout.TotalSeconds
                    })
                    {
                        _setSqlParameterCollection(sqlCommand, _parameters);

                        // ReSharper disable PossibleNullReferenceException
                        return (T)await sqlCommand.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
                        // ReSharper restore PossibleNullReferenceException
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
        public int ExecuteNonQuery()
        {
            try
            {
                using (WaitSemaphoresAsync().Result)
                using (SqlConnection connection = new SqlConnection(_mapping.Connection.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(_program.Name, connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = (int)CommandTimeout.TotalSeconds
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
        public async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                using (await WaitSemaphoresAsync(cancellationToken).ConfigureAwait(false))
                using (SqlConnection connection = new SqlConnection(_mapping.Connection.ConnectionString))
                {
                    // ReSharper disable once PossibleNullReferenceException
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    using (SqlCommand sqlCommand = new SqlCommand(_program.Name, connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = (int)CommandTimeout.TotalSeconds
                    })
                    {
                        _setSqlParameterCollection(sqlCommand, _parameters);
                        // ReSharper disable PossibleNullReferenceException
                        return await sqlCommand.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                        // ReSharper restore PossibleNullReferenceException
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
        public void ExecuteReader(
            [NotNull] ResultDelegate resultAction,
            CommandBehavior behavior = CommandBehavior.Default)
        {
            if (resultAction == null) throw new ArgumentNullException(nameof(resultAction));
            try
            {
                using (WaitSemaphoresAsync().Result)
                using (SqlConnection connection = new SqlConnection(_mapping.Connection.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(_program.Name, connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = (int)CommandTimeout.TotalSeconds
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
        /// and builds a <see cref="SqlDataReader" /> using the provided <see cref="CommandBehavior">behavior</see>,
        /// requires manual disposal.
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
        public void ExecuteReader(
            [NotNull] ResultDisposableDelegate resultAction,
            CommandBehavior behavior = CommandBehavior.Default)
        {
            if (resultAction == null) throw new ArgumentNullException(nameof(resultAction));
            
            ReaderDisposer<SqlDataReader> disposer = new ReaderDisposer<SqlDataReader>();
            try
            {
                disposer.Semaphore = WaitSemaphoresAsync().Result;
                disposer.Connection = new SqlConnection(_mapping.Connection.ConnectionString);
                disposer.Connection.Open();
                disposer.Command = new SqlCommand(_program.Name, disposer.Connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = (int)CommandTimeout.TotalSeconds
                };
                _setSqlParameterCollection(disposer.Command, _parameters);
                disposer.Reader = disposer.Command.ExecuteReader(behavior);
                disposer.SetCancellationToken(CreateCancellationToken());
                // ReSharper disable once AssignNullToNotNullAttribute
                resultAction(disposer.Reader, disposer);
            }
            catch (Exception exception)
            {
                disposer.Dispose();
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
        public T ExecuteReader<T>(
            [NotNull] ResultDelegate<T> resultFunc,
            CommandBehavior behavior = CommandBehavior.Default)
        {
            if (resultFunc == null) throw new ArgumentNullException(nameof(resultFunc));

            try
            {
                using (WaitSemaphoresAsync().Result)
                using (SqlConnection connection = new SqlConnection(_mapping.Connection.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(_program.Name, connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = (int)CommandTimeout.TotalSeconds
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
        /// Executes the <see cref="SqlCommand.CommandText" /> to the <see cref="SqlCommand.Connection" />,
        /// and builds a <see cref="SqlDataReader" /> using the provided <see cref="CommandBehavior">behavior</see>,
        /// requires manual disposal.
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
        public T ExecuteReader<T>(
            [NotNull] ResultDisposableDelegate<T> resultFunc,
            CommandBehavior behavior = CommandBehavior.Default)
        {
            if (resultFunc == null) throw new ArgumentNullException(nameof(resultFunc));

            ReaderDisposer<SqlDataReader> disposer = new ReaderDisposer<SqlDataReader>();
            try
            {
                disposer.Semaphore = WaitSemaphoresAsync().Result;
                disposer.Connection = new SqlConnection(_mapping.Connection.ConnectionString);
                disposer.Connection.Open();
                disposer.Command = new SqlCommand(_program.Name, disposer.Connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = (int)CommandTimeout.TotalSeconds
                };
                _setSqlParameterCollection(disposer.Command, _parameters);
                disposer.Reader = disposer.Command.ExecuteReader(behavior);
                disposer.SetCancellationToken(CreateCancellationToken());
                // ReSharper disable once AssignNullToNotNullAttribute
                return resultFunc(disposer.Reader, disposer);
            }
            catch (Exception exception)
            {
                disposer.Dispose();
                throw new SqlProgramExecutionException(_program, exception);
            }
        }

        /// <summary>
        /// Creates a <see cref="Task&lt;TResult&gt;">Task&lt;SqlDataReader&gt;</see> that executes an operation asynchronously.
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
        public async Task ExecuteReaderAsync(
            [NotNull] ResultDelegateAsync resultAction,
            CommandBehavior behavior = CommandBehavior.Default,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (resultAction == null) throw new ArgumentNullException(nameof(resultAction));

            try
            {
                using (await WaitSemaphoresAsync(cancellationToken).ConfigureAwait(false))
                using (SqlConnection connection = new SqlConnection(_mapping.Connection.ConnectionString))
                {
                    // ReSharper disable PossibleNullReferenceException
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    using (SqlCommand sqlCommand = new SqlCommand(_program.Name, connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = (int)CommandTimeout.TotalSeconds
                    })
                    {
                        _setSqlParameterCollection(sqlCommand, _parameters);
                        using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync(behavior, cancellationToken).ConfigureAwait(false))
                            await resultAction(reader, CreateCancellationToken(cancellationToken)).ConfigureAwait(false);
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
        /// Creates a <see cref="Task&lt;TResult&gt;">Task&lt;SqlDataReader&gt;</see> that executes an operation asynchronously, requires manual disposal.
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
        public async Task ExecuteReaderAsync(
            [NotNull] ResultDisposableDelegateAsync resultAction,
            CommandBehavior behavior = CommandBehavior.Default,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (resultAction == null) throw new ArgumentNullException(nameof(resultAction));

            ReaderDisposer<SqlDataReader> disposer = new ReaderDisposer<SqlDataReader>();
            try
            {
                disposer.Semaphore = await WaitSemaphoresAsync(cancellationToken).ConfigureAwait(false);
                disposer.Connection = new SqlConnection(_mapping.Connection.ConnectionString);
                // ReSharper disable PossibleNullReferenceException
                await disposer.Connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                disposer.Command = new SqlCommand(_program.Name, disposer.Connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = (int)CommandTimeout.TotalSeconds
                };
                _setSqlParameterCollection(disposer.Command, _parameters);
                disposer.Reader = await disposer.Command.ExecuteReaderAsync(behavior, cancellationToken)
                    .ConfigureAwait(false);
                cancellationToken = disposer.SetCancellationToken(CreateCancellationToken(cancellationToken));
                await resultAction(disposer.Reader, disposer, cancellationToken).ConfigureAwait(false);
                // ReSharper restore PossibleNullReferenceException
            }
            catch (Exception exception)
            {
                disposer.Dispose();
                throw new SqlProgramExecutionException(_program, exception);
            }
        }

        /// <summary>
        /// Creates a <see cref="Task&lt;TResult&gt;">Task&lt;SqlDataReader&gt;</see> that executes an operation asynchronously.
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
        public async Task<T> ExecuteReaderAsync<T>(
            [NotNull] ResultDelegateAsync<T> resultFunc,
            CommandBehavior behavior = CommandBehavior.Default,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (resultFunc == null) throw new ArgumentNullException(nameof(resultFunc));

            try
            {
                using (await WaitSemaphoresAsync(cancellationToken).ConfigureAwait(false))
                using (SqlConnection connection = new SqlConnection(_mapping.Connection.ConnectionString))
                {
                    // ReSharper disable PossibleNullReferenceException
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    using (SqlCommand sqlCommand = new SqlCommand(_program.Name, connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = (int)CommandTimeout.TotalSeconds
                    })
                    {
                        _setSqlParameterCollection(sqlCommand, _parameters);
                        using (
                            SqlDataReader reader =
                                await sqlCommand.ExecuteReaderAsync(behavior, cancellationToken).ConfigureAwait(false))
                            return await resultFunc(reader, CreateCancellationToken(cancellationToken)).ConfigureAwait(false);
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
        /// Creates a <see cref="Task&lt;TResult&gt;">Task&lt;SqlDataReader&gt;</see> that executes an operation asynchronously, requires manual disposal.
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
        public async Task<T> ExecuteReaderAsync<T>(
            [NotNull] ResultDisposableDelegateAsync<T> resultFunc,
            CommandBehavior behavior = CommandBehavior.Default,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (resultFunc == null) throw new ArgumentNullException(nameof(resultFunc));

            ReaderDisposer<SqlDataReader> disposer = new ReaderDisposer<SqlDataReader>();
            try
            {
                disposer.Semaphore = await WaitSemaphoresAsync(cancellationToken).ConfigureAwait(false);
                disposer.Connection = new SqlConnection(_mapping.Connection.ConnectionString);
                // ReSharper disable PossibleNullReferenceException
                await disposer.Connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                disposer.Command = new SqlCommand(_program.Name, disposer.Connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = (int)CommandTimeout.TotalSeconds
                };
                _setSqlParameterCollection(disposer.Command, _parameters);
                disposer.Reader = await disposer.Command.ExecuteReaderAsync(behavior, cancellationToken)
                    .ConfigureAwait(false);
                cancellationToken = disposer.SetCancellationToken(CreateCancellationToken(cancellationToken));
                return await resultFunc(disposer.Reader, disposer, cancellationToken).ConfigureAwait(false);
                // ReSharper restore PossibleNullReferenceException
            }
            catch (Exception exception)
            {
                disposer.Dispose();
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
        public void ExecuteXmlReader([NotNull] XmlResultDelegate resultAction)
        {
            if (resultAction == null) throw new ArgumentNullException(nameof(resultAction));

            try
            {
                using (WaitSemaphoresAsync().Result)
                using (SqlConnection connection = new SqlConnection(_mapping.Connection.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(_program.Name, connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = (int)CommandTimeout.TotalSeconds
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
        /// and builds a <see cref="XmlReader" /> using the provided <see cref="CommandBehavior">behavior</see>,
        /// requires manual disposal.
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
        public void ExecuteXmlReader([NotNull] XmlResultDisposableDelegate resultAction)
        {
            if (resultAction == null) throw new ArgumentNullException(nameof(resultAction));

            ReaderDisposer<XmlReader> disposer = new ReaderDisposer<XmlReader>();
            try
            {
                disposer.Semaphore = WaitSemaphoresAsync().Result;
                disposer.Connection = new SqlConnection(_mapping.Connection.ConnectionString);
                disposer.Connection.Open();
                disposer.Command = new SqlCommand(_program.Name, disposer.Connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = (int)CommandTimeout.TotalSeconds
                };
                _setSqlParameterCollection(disposer.Command, _parameters);
                disposer.Reader = disposer.Command.ExecuteXmlReader();
                disposer.SetCancellationToken(CreateCancellationToken());
                // ReSharper disable once AssignNullToNotNullAttribute
                resultAction(disposer.Reader, disposer);
            }
            catch (Exception exception)
            {
                disposer.Dispose();
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
        public T ExecuteXmlReader<T>([NotNull] XmlResultDelegate<T> resultFunc)
        {
            if (resultFunc == null) throw new ArgumentNullException(nameof(resultFunc));

            try
            {
                using (WaitSemaphoresAsync().Result)
                using (SqlConnection connection = new SqlConnection(_mapping.Connection.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(_program.Name, connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = (int)CommandTimeout.TotalSeconds
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
        public T ExecuteXmlReader<T>([NotNull] XmlResultDisposableDelegate<T> resultFunc)
        {
            if (resultFunc == null) throw new ArgumentNullException(nameof(resultFunc));
            
            ReaderDisposer<XmlReader> disposer = new ReaderDisposer<XmlReader>();
            try
            {
                disposer.Semaphore = WaitSemaphoresAsync().Result;
                disposer.Connection = new SqlConnection(_mapping.Connection.ConnectionString);
                disposer.Connection.Open();
                disposer.Command = new SqlCommand(_program.Name, disposer.Connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = (int)CommandTimeout.TotalSeconds
                };
                _setSqlParameterCollection(disposer.Command, _parameters);
                disposer.Reader = disposer.Command.ExecuteXmlReader();
                disposer.SetCancellationToken(CreateCancellationToken());
                // ReSharper disable once AssignNullToNotNullAttribute
                return resultFunc(disposer.Reader, disposer);
            }
            catch (Exception exception)
            {
                disposer.Dispose();
                throw new SqlProgramExecutionException(_program, exception);
            }
        }

        /// <summary>
        /// Creates a <see cref="Task&lt;TXmlResult&gt;">Task&lt;XmlReader&gt;</see> that executes an operation
        /// asynchronously.
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
        public async Task ExecuteXmlReaderAsync(
            [NotNull] XmlResultDelegateAsync resultAction,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (resultAction == null) throw new ArgumentNullException(nameof(resultAction));

            try
            {
                using (await WaitSemaphoresAsync(cancellationToken).ConfigureAwait(false))
                using (SqlConnection connection = new SqlConnection(_mapping.Connection.ConnectionString))
                {
                    // ReSharper disable PossibleNullReferenceException
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    using (SqlCommand sqlCommand = new SqlCommand(_program.Name, connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = (int)CommandTimeout.TotalSeconds
                    })
                    {
                        _setSqlParameterCollection(sqlCommand, _parameters);
                        using (XmlReader reader =
                                await sqlCommand.ExecuteXmlReaderAsync(cancellationToken).ConfigureAwait(false))
                            await resultAction(reader, CreateCancellationToken(cancellationToken)).ConfigureAwait(false);
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
        /// Creates a <see cref="Task&lt;TXmlResult&gt;">Task&lt;XmlReader&gt;</see> that executes an operation
        /// asynchronously, requires manual disposal.
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
        public async Task ExecuteXmlReaderAsync(
            [NotNull] XmlResultDisposableDelegateAsync resultAction,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (resultAction == null) throw new ArgumentNullException(nameof(resultAction));

            ReaderDisposer<XmlReader> disposer = new ReaderDisposer<XmlReader>();
            try
            {
                disposer.Semaphore = await WaitSemaphoresAsync(cancellationToken).ConfigureAwait(false);
                disposer.Connection = new SqlConnection(_mapping.Connection.ConnectionString);
                // ReSharper disable PossibleNullReferenceException
                await disposer.Connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                disposer.Command = new SqlCommand(_program.Name, disposer.Connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = (int)CommandTimeout.TotalSeconds
                };
                _setSqlParameterCollection(disposer.Command, _parameters);
                disposer.Reader = await disposer.Command.ExecuteXmlReaderAsync(cancellationToken)
                    .ConfigureAwait(false);
                cancellationToken = disposer.SetCancellationToken(CreateCancellationToken(cancellationToken));
                await resultAction(disposer.Reader, disposer, cancellationToken).ConfigureAwait(false);
                // ReSharper restore PossibleNullReferenceException
            }
            catch (Exception exception)
            {
                disposer.Dispose();
                throw new SqlProgramExecutionException(_program, exception);
            }
        }

        /// <summary>
        /// Creates a <see cref="Task&lt;TXmlResult&gt;">Task&lt;XmlReader&gt;</see> that executes an operation
        /// asynchronously.
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
        public async Task<T> ExecuteXmlReaderAsync<T>(
            [NotNull] XmlResultDelegateAsync<T> resultFunc,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (resultFunc == null) throw new ArgumentNullException(nameof(resultFunc));

            try
            {
                using (await WaitSemaphoresAsync(cancellationToken).ConfigureAwait(false))
                using (SqlConnection connection = new SqlConnection(_mapping.Connection.ConnectionString))
                {
                    // ReSharper disable PossibleNullReferenceException
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    using (SqlCommand sqlCommand = new SqlCommand(_program.Name, connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = (int)CommandTimeout.TotalSeconds
                    })
                    {
                        _setSqlParameterCollection(sqlCommand, _parameters);
                        using (
                            XmlReader reader =
                                await sqlCommand.ExecuteXmlReaderAsync(cancellationToken).ConfigureAwait(false))
                            return
                                await
                                    resultFunc(reader, CreateCancellationToken(cancellationToken)).ConfigureAwait(false);
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
        /// Creates a <see cref="Task&lt;TXmlResult&gt;">Task&lt;XmlReader&gt;</see> that executes an operation
        /// asynchronously, requires manual disposal.
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
        public async Task<T> ExecuteXmlReaderAsync<T>(
            [NotNull] XmlResultDisposableDelegateAsync<T> resultFunc,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (resultFunc == null) throw new ArgumentNullException(nameof(resultFunc));

            ReaderDisposer<XmlReader> disposer = new ReaderDisposer<XmlReader>();
            try
            {
                disposer.Semaphore = await WaitSemaphoresAsync(cancellationToken).ConfigureAwait(false);
                disposer.Connection = new SqlConnection(_mapping.Connection.ConnectionString);
                // ReSharper disable PossibleNullReferenceException
                await disposer.Connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                disposer.Command = new SqlCommand(_program.Name, disposer.Connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = (int)CommandTimeout.TotalSeconds
                };
                _setSqlParameterCollection(disposer.Command, _parameters);
                disposer.Reader = await disposer.Command.ExecuteXmlReaderAsync(cancellationToken)
                    .ConfigureAwait(false);
                cancellationToken = disposer.SetCancellationToken(CreateCancellationToken(cancellationToken));
                return await resultFunc(disposer.Reader, disposer, cancellationToken).ConfigureAwait(false);
                // ReSharper restore PossibleNullReferenceException
            }
            catch (Exception exception)
            {
                disposer.Dispose();
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
        public override string ToString() => $"A SqlProgramCommand for the '{_program.Name}' SqlProgram";
    }
}