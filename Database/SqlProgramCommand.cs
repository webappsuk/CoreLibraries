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
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using JetBrains.Annotations;
using WebApplications.Utilities.Database.Schema;
using WebApplications.Utilities.Logging;

namespace WebApplications.Utilities.Database
{
    /// <summary>
    ///   A specialised command that allows finer grained control when using <see cref="SqlProgram"/>s.
    /// </summary>
    public partial class SqlProgramCommand : IDisposable
    {
        private readonly SqlCommand _command;
        private readonly SqlConnection _connection;
        private readonly SqlProgram _program;
        private int _disposed;

        /// <summary>
        ///   Initializes a new instance of the <see cref="SqlProgramCommand"/> class.
        /// </summary>
        /// <param name="program">The SQL program.</param>
        /// <param name="connection">An open connection to the SQL server database.</param>
        /// <param name="commandTimeout">
        ///   The time to wait for the program to execute before raising an error.
        /// </param>
        internal SqlProgramCommand([NotNull] SqlProgram program, [NotNull] SqlConnection connection,
                                   TimeSpan commandTimeout)
        {
            _program = program;
            _connection = connection;

            // Create underlying command
            _command = new SqlCommand(program.Name, connection)
                           {
                               CommandType = CommandType.StoredProcedure,
                               CommandTimeout = (int) commandTimeout.TotalSeconds
                           };
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
            get { return TimeSpan.FromSeconds(_command.CommandTimeout); }
            set { _command.CommandTimeout = (int) value.TotalSeconds; }
        }

        #region IDisposable Members
        /// <summary>
        ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 1)
                return;

            if (_command != null)
                _command.Dispose();
            if (_connection != null)
                _connection.Dispose();
        }
        #endregion

        /// <summary>
        ///   Gets the parameter with the specified name and returns it as an <see cref="SqlParameter"/> object.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to retrieve.</param>
        /// <returns>
        ///   The <see cref="SqlParameter"/> that corresponds to the <paramref name="parameterName"/> provided.
        /// </returns>
        /// <exception cref="LoggingException">
        ///   Could not find a match with the <paramref name="parameterName"/> specified.
        /// </exception>
        /// <exception cref="NullReferenceException">
        ///   <paramref name="parameterName"/> was <see langword="null"/>.
        /// </exception>
        [NotNull]
        [UsedImplicitly]
        public SqlParameter GetParameter([NotNull] string parameterName)
        {
            SqlParameter parameter;
            parameterName = parameterName.ToLower();

            int index = _command.Parameters.IndexOf(parameterName);
            if (index < 0)
            {
                // Parameter not added yet
                SqlProgramParameter parameterDefinition;
                if (!_program.Definition.TryGetParameter(parameterName, out parameterDefinition))
                    throw new LoggingException(
                        Resources.SqlProgramCommand_GetParameter_ProgramDoesNotHaveParameter,
                        LogLevel.Critical,
                        _program.Name,
                        parameterName);

                // Create the parameter and add it to the collection
                parameter = _command.Parameters.Add(parameterDefinition.CreateSqlParameter());
            }
            else
            {
                // Get the parameter and set it's value.
                parameter = _command.Parameters[index];
            }
            return parameter;
        }

        /// <summary>
        ///   Sets the specified parameter with the value provided and returns it as an <see cref="SqlParameter"/> object.
        /// </summary>
        /// <typeparam name="T">The type of the value to set.</typeparam>
        /// <param name="parameterName">The name of the parameter to set.</param>
        /// <param name="value">The value to set the parameter to.</param>
        /// <param name="mode">
        ///   <para>The constraint mode.</para>
        ///   <para>By default this is set to give a warning if truncation/loss of precision occurs.</para>
        /// </param>
        /// <returns>The SqlParameter with the specified name.</returns>
        /// <returns>
        ///   The <see cref="SqlParameter"/> that corresponds to <paramref name="parameterName"/>,
        ///   set to the <paramref name="value"/> specified.
        /// </returns>
        /// <exception cref="LoggingException">
        ///   Could not find a match with the <paramref name="parameterName"/> specified.
        /// </exception>
        /// <exception cref="NullReferenceException">
        ///   <paramref name="parameterName"/> was <see langword="null"/>.
        /// </exception>
        public SqlParameter SetParameter<T>(string parameterName, T value,
                                            TypeConstraintMode mode = TypeConstraintMode.Warn)
        {
            SqlParameter parameter;
            parameterName = parameterName.ToLower();

            // Find parameter definition
            SqlProgramParameter parameterDefinition;
            if (!_program.Definition.TryGetParameter(parameterName, out parameterDefinition))
                throw new LoggingException(
                    Resources.SqlProgramCommand_SetParameter_ProgramDoesNotHaveParameter,
                    LogLevel.Critical,
                    _program.Name,
                    parameterName);

            // Find or create SQL Parameter.
            int index = _command.Parameters.IndexOf(parameterName);
            parameter = index < 0
                            ? _command.Parameters.Add(parameterDefinition.CreateSqlParameter())
                            : _command.Parameters[index];

            parameter.Value = parameterDefinition.CastCLRValue(value, mode);
            return parameter;
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
            return (T) _command.ExecuteScalar();
        }

        /// <summary>
        ///   Initiates the execution of the asynchronous operation.
        /// </summary>
        /// <param name="callback">
        ///   The method to call once the operation completes.
        /// </param>
        /// <param name="stateObject">
        ///   A user-defined state object. 
        ///   Retrieve this object from within the callback procedure using the AsyncState property.
        /// </param>
        /// <returns>
        ///   The <see cref="IAsyncResult"/>, which represents the status of the asynchronous operation.
        /// </returns>
        /// <remarks>
        ///   <see cref="EndExecuteScalar&lt;T&gt;"/> <b>must</b> be called to finish the operation.
        /// </remarks>
        /// <exception cref="SqlException">
        ///   An error occurred whilst executing the command text.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///   The name/value pair <c>Asynchronous Processing=true</c> was not included within the
        ///   connection string defining the connection for this <see cref="SqlCommand"/>
        /// </exception>
        public IAsyncResult BeginExecuteScalar(AsyncCallback callback = null, object stateObject = null)
        {
            return _command.BeginExecuteReader(callback, stateObject,
                                               CommandBehavior.SingleResult | CommandBehavior.SingleRow);
        }

        /// <summary>
        ///   Finishes the execution of the asynchronous operation.
        /// </summary>
        /// <typeparam name="T">The type of the value in the first column.</typeparam>
        /// <param name="asyncResult">
        ///   The status of the asynchronous operation.
        /// </param>
        /// <returns>
        ///   Returns the value of the first column in the first row.
        ///   If no records are present then the default value of <typeparamref name="T"/> is returned.
        /// </returns>
        /// <remarks>
        ///   <see cref="BeginExecuteScalar"/> <b>must</b> be called to commence the operation's execution.
        /// </remarks>
        /// <exception cref="ArgumentException">
        ///   <paramref name="asyncResult"/> was <see langword="null"/>.
        /// </exception>
        public T EndExecuteScalar<T>([NotNull] IAsyncResult asyncResult)
        {
            using (SqlDataReader reader = _command.EndExecuteReader(asyncResult))
            {
                if ((reader != null) && (reader.Read()))
                    return reader.GetValue<T>(0);
                return default(T);
            }
        }

        /// <summary>
        ///   Creates a <see cref="Task&lt;TResult&gt;">Task&lt;T&gt;</see> that executes an operation asynchronously,
        ///   returning the first column of the first row in the result set. Any additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="T">
        ///   The type of the value in the result (the value first column of the first row).
        /// </typeparam>
        /// <param name="state">
        ///   A state object containing data to be used by the <see cref="BeginExecuteScalar"/> method.
        /// </param>
        /// <returns>
        ///   A <see cref="Task&lt;TResult&gt;">Task&lt;T&gt;</see> which represents the begin and end methods
        ///   <see cref="BeginExecuteScalar"/> and <see cref="EndExecuteScalar&lt;T&gt;"/>.
        /// </returns>
        /// <exception cref="SqlException">
        ///   An exception occurred while executing the command against a locked row.
        ///   This exception is not generated when you are using Microsoft .NET Framework version 1.0.
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
        [NotNull]
        public Task<T> ExecuteScalarAsync<T>(object state = null)
        {
            return Task.Factory.FromAsync<T>(BeginExecuteScalar, EndExecuteScalar<T>, state);
        }

        /// <summary>
        ///   Executes a Transact-SQL statement against the connection and returns the number of rows affected.
        /// </summary>
        /// <returns>
        ///   The number of rows affected.
        /// </returns>
        /// <exception cref="SqlException">
        ///   An exception occurred whilst executing the command against a locked row.
        ///   This exception is not generated when using Microsoft .NET Framework version 1.0.
        /// </exception>
        /// <filterpriority>1</filterpriority>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public int ExecuteNonQuery()
        {
            return _command.ExecuteNonQuery();
        }

        /// <summary>
        ///   Begins to execute the non-query asynchronously, given a callback and state information.
        /// </summary>
        /// <param name="callback">
        ///   The method to call when the asynchronous operation is complete.
        /// </param>
        /// <param name="stateObject">
        ///   A user-defined state object that is passed to the callback. 
        ///   Retrieve this object from the callback procedure using the <see cref="IAsyncResult.AsyncState"/> property.
        /// </param>
        /// <returns>
        ///   The <see cref="IAsyncResult"/>, which represents the status of the asynchronous operation.
        /// </returns>
        /// <remarks>
        ///   <see cref="EndExecuteNonQuery"/> <b>must</b> be called to finish the operation.
        /// </remarks>
        /// <exception cref="SqlException">
        ///   An error occurred whilst executing the command text.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///   The name/value pair <c>Asynchronous Processing=true</c> was not included within the
        ///   connection string defining the connection for this <see cref="SqlCommand"/>
        /// </exception>
        public IAsyncResult BeginExecuteNonQuery(AsyncCallback callback = null, object stateObject = null)
        {
            return _command.BeginExecuteNonQuery(callback, stateObject);
        }

        /// <summary>
        ///   Ends the execution of the asynchronous non-query.
        /// </summary>
        /// <param name="asyncResult">
        ///   The status of the asynchronous operation.
        /// </param>
        /// <returns>
        ///   The number of rows affected by the query.
        /// </returns>
        public object EndExecuteNonQuery([NotNull] IAsyncResult asyncResult)
        {
            return _command.EndExecuteNonQuery(asyncResult);
        }

        /// <summary>
        ///   Creates a <see cref="Task&lt;TResult&gt;">Task&lt;int&gt;</see> that executes an non-query asynchronously,
        ///   and returns the number of rows affected.
        /// </summary>
        /// <param name="state">
        ///   A state object containing data to be used by the <see cref="BeginExecuteNonQuery"/> method.
        /// </param>
        /// <returns>
        ///   A <see cref="Task&lt;TResult&gt;">Task&lt;int&gt;</see> which represents the begin and end methods
        ///   <see cref="BeginExecuteNonQuery"/> and <see cref="EndExecuteNonQuery"/>.
        /// </returns>
        /// <exception cref="SqlException">
        ///   An exception occurred whilst executing the command against a locked row.
        ///   This exception is not generated when using Microsoft .NET Framework version 1.0.
        /// </exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [NotNull]
        public Task<int> ExecuteNonQueryAsync(object state = null)
        {
            return Task.Factory.FromAsync<int>(_command.BeginExecuteNonQuery, _command.EndExecuteNonQuery, state);
        }

        /// <summary>
        ///   Executes the <see cref="SqlCommand.CommandText"/> to the <see cref="SqlCommand.Connection"/>,
        ///   and builds a <see cref="SqlDataReader"/> using the provided <see cref="CommandBehavior">behavior</see>.
        /// </summary>
        /// <param name="behavior">
        ///   <para>Describes the results of the query and its effect on the database.</para>
        ///   <para>By default this is set to CommandBehavior.Default.</para>
        /// </param>
        /// <returns>
        ///   The built <see cref="SqlDataReader"/> object.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public SqlDataReader ExecuteReader(CommandBehavior behavior = CommandBehavior.Default)
        {
            return _command.ExecuteReader(behavior);
        }

        /// <summary>
        ///   Initiates the execution of the operation asynchronously using the provided
        ///   <see cref="CommandBehavior">behavior</see>.
        /// </summary>
        /// <param name="callback">The method to call once the operation is complete.</param>
        /// <param name="stateObject">
        ///   A user-defined state object that is passed to the callback. 
        ///   Retrieve this object from the callback procedure using the <see cref="IAsyncResult.AsyncState"/> property.
        /// </param>
        /// <param name="behavior">
        ///   <para>Describes the results of the query and its effect on the database.</para>
        ///   <para>By default this is set to CommandBehavior.Default.</para>
        /// </param>
        /// <returns>
        ///   The <see cref="IAsyncResult"/>, which represents the status of the asynchronous operation.
        /// </returns>
        /// <remarks>
        ///   <see cref="EndExecuteReader"/> <b>must</b> be called to finish the execution.
        /// </remarks>
        /// <exception cref="SqlException">
        ///   An error occurred whilst executing the command text.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///   The name/value pair <c>Asynchronous Processing=true</c> was not included within the
        ///   connection string defining the connection for this <see cref="SqlCommand"/>
        /// </exception>
        public IAsyncResult BeginExecuteReader(AsyncCallback callback = null, object stateObject = null,
                                               CommandBehavior behavior = CommandBehavior.Default)
        {
            return _command.BeginExecuteReader(callback, stateObject, behavior);
        }

        /// <summary>
        ///   Finishes the execution of an asynchronous operation.
        /// </summary>
        /// <param name="asyncResult">
        ///   The async result returned from <see cref="BeginExecuteReader"/>.
        /// </param>
        /// <returns>
        ///   A <see cref="SqlDataReader"/> that can be used to retrieve the records.
        /// </returns>
        /// <remarks>
        ///   <see cref="BeginExecuteReader"/> <b>must</b> be called initialise the operation's execution.
        /// </remarks>
        /// <exception cref="ArgumentException">
        ///   <paramref name="asyncResult"/> was <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///   The name/value pair <c>Asynchronous Processing=true</c> was not included within the
        ///   connection string defining the connection for this <see cref="SqlCommand"/>
        /// </exception>
        public SqlDataReader EndExecuteReader([NotNull] IAsyncResult asyncResult)
        {
            return _command.EndExecuteReader(asyncResult);
        }

        /// <summary>
        ///   Creates a <see cref="Task&lt;TResult&gt;">Task&lt;SqlDataReader&gt;</see> that executes an operation asynchronously
        /// </summary>
        /// <param name="behavior">
        ///   <para>Describes the results of the query and its effect on the database.</para>
        ///   <para>By default this is set to CommandBehavior.Default.</para>
        /// </param>
        /// <param name="state">
        ///   A state object containing data to be used by the <see cref="BeginExecuteReader"/> method.
        /// </param>
        /// <returns>
        ///   A <see cref="Task&lt;TResult&gt;">Task&lt;DataReader&gt;</see> which represents the begin and end methods
        ///   <see cref="BeginExecuteReader"/> and <see cref="EndExecuteReader"/>.
        /// </returns>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [NotNull]
        public Task<SqlDataReader> ExecuteReaderAsync(CommandBehavior behavior = CommandBehavior.Default,
                                                      object state = null)
        {
            return Task.Factory.FromAsync<SqlDataReader>((a, o) => _command.BeginExecuteReader(a, o, behavior),
                                                         _command.EndExecuteReader, state);
        }

        /// <summary>
        ///   Sends the <see cref="SqlCommand.CommandText"/> to the <see cref="SqlCommand.Connection"/>
        ///   and builds an <see cref="XmlReader"/> object.
        /// </summary>
        /// <returns>
        ///   An <see cref="XmlReader"/> object.
        /// </returns>
        /// <remarks>
        ///   The command text should either contain a valid FOR XML clause, return a result of type ntext or nvarchar
        ///   (containing valid XML) or return the contents of a column defined as the xml data type.
        /// </remarks>
        /// <exception cref="SqlException">
        ///   An exception occurred whilst executing the command against a locked row.
        ///   This exception is not generated when using Microsoft .NET Framework version 1.0.
        /// </exception>
        /// <filterpriority>1</filterpriority>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public XmlReader ExecuteXmlReader()
        {
            return _command.ExecuteXmlReader();
        }

        /// <summary>
        ///   Initiates the execution of operation asynchronously.
        /// </summary>
        /// <param name="callback">
        ///   The method to call once the asynchronous operation completes.
        /// </param>
        /// <param name="stateObject">
        ///   A user-defined state object that is passed to the callback. 
        ///   Retrieve this object from the callback procedure using the <see cref="IAsyncResult.AsyncState"/> property.
        /// </param>
        /// <returns>
        ///   The <see cref="IAsyncResult"/>, which represents the status of the asynchronous operation.
        /// </returns>
        /// <remarks>
        ///   <see cref="EndExecuteXmlReader"/> <b>must</b> be called to finish the execution.
        /// </remarks>
        /// <exception cref="SqlException">
        ///   An error occurred whilst executing the command text.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///   The name/value pair <c>Asynchronous Processing=true</c> was not included within the
        ///   connection string defining the connection for this <see cref="SqlCommand"/>
        /// </exception>
        /// <seealso cref="SqlCommand.BeginExecuteXmlReader(AsyncCallback, object)"/>
        public IAsyncResult BeginExecuteXmlReader(AsyncCallback callback = null, object stateObject = null)
        {
            return _command.BeginExecuteXmlReader(callback, stateObject);
        }

        /// <summary>
        ///   Finishes the execution of an asynchronous operation, returning the results as XML.
        /// </summary>
        /// <param name="asyncResult">
        ///   The async result, which represents the status of the asynchronous operation.
        /// </param>
        /// <returns>
        ///   An <see cref="XmlReader"/> object that can be used to retrieve the results. 
        /// </returns>
        /// <remarks>
        ///   <see cref="BeginExecuteXmlReader"/> <b>must</b> be called to initialise the execution.
        /// </remarks>
        /// <exception cref="ArgumentException">
        ///   <paramref name="asyncResult"/> is <see langword="null"/>.
        /// </exception>
        /// <seealso cref="SqlCommand.EndExecuteXmlReader(IAsyncResult)"/>
        public XmlReader EndExecuteXmlReader([NotNull] IAsyncResult asyncResult)
        {
            return _command.EndExecuteXmlReader(asyncResult);
        }

        /// <summary>
        ///   Creates a <see cref="Task&lt;TResult&gt;">Task&lt;XmlReader&gt;</see> that executes an operation asynchronously,
        /// </summary>
        /// <param name="state">
        ///   A state object containing data to be used by the <see cref="BeginExecuteXmlReader"/> method.
        /// </param>
        /// <returns>
        ///   A <see cref="Task&lt;TResult&gt;">Task&lt;XmlReader&gt;</see> which represents the begin and end methods
        ///   <see cref="BeginExecuteXmlReader"/> and <see cref="EndExecuteXmlReader"/>.
        /// </returns>
        /// <exception cref="SqlException">
        ///   An exception occurred while executing the command against a locked row.
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
        [NotNull]
        public Task<XmlReader> ExecuteXmlReaderAsync(object state = null)
        {
            return Task.Factory.FromAsync<XmlReader>(_command.BeginExecuteXmlReader, _command.EndExecuteXmlReader, state);
        }

        /// <summary>
        ///   Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        ///   A <see cref="string"/> representation of this instance.
        /// </returns>
        public override string ToString()
        {
            return "SqlProgramCommand";
        }
    }
}