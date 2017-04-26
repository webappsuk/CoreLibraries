#region © Copyright Web Applications (UK) Ltd, 2016.  All rights reserved.
// Copyright (c) 2016, Web Applications UK Ltd
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
        /// Class ReaderDisposer implements <see cref="IDisposable"/> and allows for manual disposal of resources.
        /// </summary>
        /// <seealso cref="System.IDisposable" />
        private class ReaderDisposer : IDisposable
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
            public IDisposable Reader;

            /// <summary>
            /// The output parameters.
            /// </summary>
            public IEnumerable<SqlProgramParameter, SqlParameter, IOut> OutputParameters;

            /// <summary>
            /// The command timeout.
            /// </summary>
            public TimeSpan CommandTimeout;

            public CancellationToken CancellationToken { get; private set; }

            /// <summary>
            /// The registration for cancellation.
            /// </summary>
            private CancellationTokenRegistration _registration;

            private bool _cancelled;

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                _registration.Dispose();
                Interlocked.Exchange(ref Reader, null)?.Dispose();
                Interlocked.Exchange(ref Command, null)?.Dispose();
                Interlocked.Exchange(ref Connection, null)?.Dispose();
                Interlocked.Exchange(ref Semaphore, null)?.Dispose();
                IEnumerable<SqlProgramParameter, SqlParameter, IOut> outs = Interlocked.Exchange(
                    ref OutputParameters,
                    null);
                if (!_cancelled && outs != null) SetOutputValues(outs);
            }

            /// <summary>
            /// Sets a cancellation token that will trigger disposal.
            /// </summary>
            /// <param name="cancellationToken">The cancellation token.</param>
            /// <returns>CancellationToken.</returns>
            public void SetCancellationToken(CancellationToken cancellationToken = default(CancellationToken))
            {
                if (CommandTimeout > TimeSpan.Zero)
                    cancellationToken = cancellationToken
                        .WithTimeout(CommandTimeout.Add(AdditionalCancellationTime))
                        .Token;

                if (cancellationToken.CanBeCanceled)
                    _registration = cancellationToken.Register(
                        () =>
                        {
                            _cancelled = true;
                            Dispose();
                        });

                CancellationToken = cancellationToken;
            }

            /// <summary>
            /// Sets the values of the output parameters from the SqlParameters.
            /// </summary>
            /// <param name="outputs">The outputs.</param>
            private void SetOutputValues(
                [NotNull] [ItemNotNull] IEnumerable<SqlProgramParameter, SqlParameter, IOut> outputs)
            {
                foreach (Tuple<SqlProgramParameter, SqlParameter, IOut> tuple in outputs)
                {
                    SqlProgramParameter programParameter = tuple.Item1;
                    SqlParameter parameter = tuple.Item2;
                    IOut output = tuple.Item3;

                    Debug.Assert(programParameter != null, "programParameter != null");
                    Debug.Assert(parameter != null, "parameter != null");
                    Debug.Assert(output != null, "output != null");

                    object outValue;
                    try
                    {
                        outValue = programParameter.CastSQLValue(parameter.Value, output.Type);
                    }
                    catch (Exception e)
                    {
                        output.SetOutputError(e, parameter);
                        continue;
                    }
                    output.SetOutputValue(outValue, parameter);
                }
            }

            #region SqlCommand.Execute methods
            /// <summary>Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.</summary>
            /// <returns>The first column of the first row in the result set, or a null reference (Nothing in Visual Basic) if the result set is empty. Returns a maximum of 2033 characters.</returns>
            /// <exception cref="T:System.InvalidCastException">A <see cref="P:System.Data.SqlClient.SqlParameter.SqlDbType" /> other than Binary or VarBinary was used when <see cref="P:System.Data.SqlClient.SqlParameter.Value" /> was set to <see cref="T:System.IO.Stream" />. For more information about streaming, see SqlClient Streaming Support.A <see cref="P:System.Data.SqlClient.SqlParameter.SqlDbType" /> other than Char, NChar, NVarChar, VarChar, or  Xml was used when <see cref="P:System.Data.SqlClient.SqlParameter.Value" /> was set to <see cref="T:System.IO.TextReader" />.A <see cref="P:System.Data.SqlClient.SqlParameter.SqlDbType" /> other than Xml was used when <see cref="P:System.Data.SqlClient.SqlParameter.Value" /> was set to <see cref="T:System.Xml.XmlReader" />.</exception>
            /// <exception cref="T:System.Data.SqlClient.SqlException">An exception occurred while executing the command against a locked row. This exception is not generated when you are using Microsoft .NET Framework version 1.0.A timeout occurred during a streaming operation. For more information about streaming, see SqlClient Streaming Support.</exception>
            /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Data.SqlClient.SqlConnection" /> closed or dropped during a streaming operation. For more information about streaming, see SqlClient Streaming Support.</exception>
            /// <exception cref="T:System.IO.IOException">An error occurred in a <see cref="T:System.IO.Stream" />, <see cref="T:System.Xml.XmlReader" /> or <see cref="T:System.IO.TextReader" /> object during a streaming operation.  For more information about streaming, see SqlClient Streaming Support.</exception>
            /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.Stream" />, <see cref="T:System.Xml.XmlReader" /> or <see cref="T:System.IO.TextReader" /> object was closed during a streaming operation.  For more information about streaming, see SqlClient Streaming Support.</exception>
            /// <PermissionSet>
            ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
            ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
            ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
            ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
            ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
            ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
            ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
            /// </PermissionSet>
            public object ExecuteScalar()
            {
                SqlCommand command = Command;
                // Should never happen
                if (command == null) throw new ObjectDisposedException(nameof(ReaderDisposer));

                SetCancellationToken();
                return command.ExecuteScalar();
            }

            /// <summary>An asynchronous version of <see cref="M:System.Data.SqlClient.SqlCommand.ExecuteScalar" />, which executes the query asynchronously and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.The cancellation token can be used to request that the operation be abandoned before the command timeout elapses. Exceptions will be reported via the returned Task object.</summary>
            /// <returns>A task representing the asynchronous operation.</returns>
            /// <param name="cancellationToken">The cancellation instruction.</param>
            /// <exception cref="T:System.InvalidCastException">A <see cref="P:System.Data.SqlClient.SqlParameter.SqlDbType" /> other than Binary or VarBinary was used when <see cref="P:System.Data.SqlClient.SqlParameter.Value" /> was set to <see cref="T:System.IO.Stream" />. For more information about streaming, see SqlClient Streaming Support.A <see cref="P:System.Data.SqlClient.SqlParameter.SqlDbType" /> other than Char, NChar, NVarChar, VarChar, or  Xml was used when <see cref="P:System.Data.SqlClient.SqlParameter.Value" /> was set to <see cref="T:System.IO.TextReader" />.A <see cref="P:System.Data.SqlClient.SqlParameter.SqlDbType" /> other than Xml was used when <see cref="P:System.Data.SqlClient.SqlParameter.Value" /> was set to <see cref="T:System.Xml.XmlReader" />.</exception>
            /// <exception cref="T:System.InvalidOperationException">Calling <see cref="M:System.Data.SqlClient.SqlCommand.ExecuteScalarAsync(System.Threading.CancellationToken)" /> more than once for the same instance before task completion.The <see cref="T:System.Data.SqlClient.SqlConnection" /> closed or dropped during a streaming operation. For more information about streaming, see SqlClient Streaming Support.Context Connection=true is specified in the connection string.</exception>
            /// <exception cref="T:System.Data.SqlClient.SqlException">SQL Server returned an error while executing the command text.A timeout occurred during a streaming operation. For more information about streaming, see SqlClient Streaming Support.</exception>
            /// <exception cref="T:System.IO.IOException">An error occurred in a <see cref="T:System.IO.Stream" />, <see cref="T:System.Xml.XmlReader" /> or <see cref="T:System.IO.TextReader" /> object during a streaming operation.  For more information about streaming, see SqlClient Streaming Support.</exception>
            /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.Stream" />, <see cref="T:System.Xml.XmlReader" /> or <see cref="T:System.IO.TextReader" /> object was closed during a streaming operation.  For more information about streaming, see SqlClient Streaming Support.</exception>
            [NotNull]
            public Task<object> ExecuteScalarAsync(CancellationToken cancellationToken)
            {
                SqlCommand command = Command;
                // Should never happen
                if (command == null) throw new ObjectDisposedException(nameof(ReaderDisposer));

                SetCancellationToken(cancellationToken);
                return command.ExecuteScalarAsync(cancellationToken);
            }

            /// <summary>Executes a Transact-SQL statement against the connection and returns the number of rows affected.</summary>
            /// <returns>The number of rows affected.</returns>
            /// <exception cref="T:System.InvalidCastException">A <see cref="P:System.Data.SqlClient.SqlParameter.SqlDbType" /> other than Binary or VarBinary was used when <see cref="P:System.Data.SqlClient.SqlParameter.Value" /> was set to <see cref="T:System.IO.Stream" />. For more information about streaming, see SqlClient Streaming Support.A <see cref="P:System.Data.SqlClient.SqlParameter.SqlDbType" /> other than Char, NChar, NVarChar, VarChar, or  Xml was used when <see cref="P:System.Data.SqlClient.SqlParameter.Value" /> was set to <see cref="T:System.IO.TextReader" />.A <see cref="P:System.Data.SqlClient.SqlParameter.SqlDbType" /> other than Xml was used when <see cref="P:System.Data.SqlClient.SqlParameter.Value" /> was set to <see cref="T:System.Xml.XmlReader" />.</exception>
            /// <exception cref="T:System.Data.SqlClient.SqlException">An exception occurred while executing the command against a locked row. This exception is not generated when you are using Microsoft .NET Framework version 1.0.A timeout occurred during a streaming operation. For more information about streaming, see SqlClient Streaming Support.</exception>
            /// <exception cref="T:System.IO.IOException">An error occurred in a <see cref="T:System.IO.Stream" />, <see cref="T:System.Xml.XmlReader" /> or <see cref="T:System.IO.TextReader" /> object during a streaming operation.  For more information about streaming, see SqlClient Streaming Support.</exception>
            /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Data.SqlClient.SqlConnection" /> closed or dropped during a streaming operation. For more information about streaming, see SqlClient Streaming Support.</exception>
            /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.Stream" />, <see cref="T:System.Xml.XmlReader" /> or <see cref="T:System.IO.TextReader" /> object was closed during a streaming operation.  For more information about streaming, see SqlClient Streaming Support.</exception>
            /// <PermissionSet>
            ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
            ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
            ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
            ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
            ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
            /// </PermissionSet>
            public int ExecuteNonQuery()
            {
                SqlCommand command = Command;
                // Should never happen
                if (command == null) throw new ObjectDisposedException(nameof(ReaderDisposer));

                SetCancellationToken();
                return command.ExecuteNonQuery();
            }

            /// <summary>An asynchronous version of <see cref="M:System.Data.SqlClient.SqlCommand.ExecuteNonQuery" />, which executes a Transact-SQL statement against the connection and returns the number of rows affected. The cancellation token can be used to request that the operation be abandoned before the command timeout elapses.  Exceptions will be reported via the returned Task object.</summary>
            /// <returns>A task representing the asynchronous operation.</returns>
            /// <param name="cancellationToken">The cancellation instruction.</param>
            /// <exception cref="T:System.InvalidCastException">A <see cref="P:System.Data.SqlClient.SqlParameter.SqlDbType" /> other than Binary or VarBinary was used when <see cref="P:System.Data.SqlClient.SqlParameter.Value" /> was set to <see cref="T:System.IO.Stream" />. For more information about streaming, see SqlClient Streaming Support.A <see cref="P:System.Data.SqlClient.SqlParameter.SqlDbType" /> other than Char, NChar, NVarChar, VarChar, or  Xml was used when <see cref="P:System.Data.SqlClient.SqlParameter.Value" /> was set to <see cref="T:System.IO.TextReader" />.A <see cref="P:System.Data.SqlClient.SqlParameter.SqlDbType" /> other than Xml was used when <see cref="P:System.Data.SqlClient.SqlParameter.Value" /> was set to <see cref="T:System.Xml.XmlReader" />.</exception>
            /// <exception cref="T:System.InvalidOperationException">Calling <see cref="M:System.Data.SqlClient.SqlCommand.ExecuteNonQueryAsync(System.Threading.CancellationToken)" /> more than once for the same instance before task completion.The <see cref="T:System.Data.SqlClient.SqlConnection" /> closed or dropped during a streaming operation. For more information about streaming, see SqlClient Streaming Support.Context Connection=true is specified in the connection string.</exception>
            /// <exception cref="T:System.Data.SqlClient.SqlException">SQL Server returned an error while executing the command text.A timeout occurred during a streaming operation. For more information about streaming, see SqlClient Streaming Support.</exception>
            /// <exception cref="T:System.IO.IOException">An error occurred in a <see cref="T:System.IO.Stream" />, <see cref="T:System.Xml.XmlReader" /> or <see cref="T:System.IO.TextReader" /> object during a streaming operation.  For more information about streaming, see SqlClient Streaming Support.</exception>
            /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.Stream" />, <see cref="T:System.Xml.XmlReader" /> or <see cref="T:System.IO.TextReader" /> object was closed during a streaming operation.  For more information about streaming, see SqlClient Streaming Support.</exception>
            [NotNull]
            public Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
            {
                SqlCommand command = Command;
                // Should never happen
                if (command == null) throw new ObjectDisposedException(nameof(ReaderDisposer));

                SetCancellationToken(cancellationToken);
                return command.ExecuteNonQueryAsync(cancellationToken);
            }

            /// <summary>Sends the <see cref="P:System.Data.SqlClient.SqlCommand.CommandText" /> to the <see cref="P:System.Data.SqlClient.SqlCommand.Connection" />, and builds a <see cref="T:System.Data.SqlClient.SqlDataReader" /> using one of the <see cref="T:System.Data.CommandBehavior" /> values.</summary>
            /// <returns>A <see cref="T:System.Data.SqlClient.SqlDataReader" /> object.</returns>
            /// <param name="behavior">One of the <see cref="T:System.Data.CommandBehavior" /> values.</param>
            /// <exception cref="T:System.InvalidCastException">A <see cref="P:System.Data.SqlClient.SqlParameter.SqlDbType" /> other than Binary or VarBinary was used when <see cref="P:System.Data.SqlClient.SqlParameter.Value" /> was set to <see cref="T:System.IO.Stream" />. For more information about streaming, see SqlClient Streaming Support.A <see cref="P:System.Data.SqlClient.SqlParameter.SqlDbType" /> other than Char, NChar, NVarChar, VarChar, or  Xml was used when <see cref="P:System.Data.SqlClient.SqlParameter.Value" /> was set to <see cref="T:System.IO.TextReader" />.A <see cref="P:System.Data.SqlClient.SqlParameter.SqlDbType" /> other than Xml was used when <see cref="P:System.Data.SqlClient.SqlParameter.Value" /> was set to <see cref="T:System.Xml.XmlReader" />.</exception>
            /// <exception cref="T:System.Data.SqlClient.SqlException">A timeout occurred during a streaming operation. For more information about streaming, see SqlClient Streaming Support.</exception>
            /// <exception cref="T:System.IO.IOException">An error occurred in a <see cref="T:System.IO.Stream" />, <see cref="T:System.Xml.XmlReader" /> or <see cref="T:System.IO.TextReader" /> object during a streaming operation.  For more information about streaming, see SqlClient Streaming Support.</exception>
            /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Data.SqlClient.SqlConnection" /> closed or dropped during a streaming operation. For more information about streaming, see SqlClient Streaming Support.</exception>
            /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.Stream" />, <see cref="T:System.Xml.XmlReader" /> or <see cref="T:System.IO.TextReader" /> object was closed during a streaming operation.  For more information about streaming, see SqlClient Streaming Support.</exception>
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
            public SqlDataReader ExecuteReader(CommandBehavior behavior)
            {
                SqlCommand command = Command;
                // Should never happen
                if (command == null) throw new ObjectDisposedException(nameof(ReaderDisposer));

                SqlDataReader reader = command.ExecuteReader(behavior);
                Reader = reader;
                SetCancellationToken();
                return reader;
            }

            /// <summary>An asynchronous version of <see cref="M:System.Data.SqlClient.SqlCommand.ExecuteReader(System.Data.CommandBehavior)" />, which sends the <see cref="P:System.Data.SqlClient.SqlCommand.CommandText" /> to the <see cref="P:System.Data.SqlClient.SqlCommand.Connection" />, and builds a <see cref="T:System.Data.SqlClient.SqlDataReader" />The cancellation token can be used to request that the operation be abandoned before the command timeout elapses.  Exceptions will be reported via the returned Task object.</summary>
            /// <returns>A task representing the asynchronous operation.</returns>
            /// <param name="behavior">Options for statement execution and data retrieval.  When is set to Default, <see cref="M:System.Data.SqlClient.SqlDataReader.ReadAsync(System.Threading.CancellationToken)" /> reads the entire row before returning a complete Task.</param>
            /// <param name="cancellationToken">The cancellation instruction.</param>
            /// <exception cref="T:System.InvalidCastException">A <see cref="P:System.Data.SqlClient.SqlParameter.SqlDbType" /> other than Binary or VarBinary was used when <see cref="P:System.Data.SqlClient.SqlParameter.Value" /> was set to <see cref="T:System.IO.Stream" />. For more information about streaming, see SqlClient Streaming Support.A <see cref="P:System.Data.SqlClient.SqlParameter.SqlDbType" /> other than Char, NChar, NVarChar, VarChar, or  Xml was used when <see cref="P:System.Data.SqlClient.SqlParameter.Value" /> was set to <see cref="T:System.IO.TextReader" />.A <see cref="P:System.Data.SqlClient.SqlParameter.SqlDbType" /> other than Xml was used when <see cref="P:System.Data.SqlClient.SqlParameter.Value" /> was set to <see cref="T:System.Xml.XmlReader" />.</exception>
            /// <exception cref="T:System.ArgumentException">An invalid <see cref="T:System.Data.CommandBehavior" /> value.</exception>
            /// <exception cref="T:System.InvalidOperationException">Calling <see cref="M:System.Data.SqlClient.SqlCommand.ExecuteReaderAsync(System.Data.CommandBehavior,System.Threading.CancellationToken)" /> more than once for the same instance before task completion.The <see cref="T:System.Data.SqlClient.SqlConnection" /> closed or dropped during a streaming operation. For more information about streaming, see SqlClient Streaming Support.Context Connection=true is specified in the connection string.</exception>
            /// <exception cref="T:System.Data.SqlClient.SqlException">SQL Server returned an error while executing the command text.A timeout occurred during a streaming operation. For more information about streaming, see SqlClient Streaming Support.</exception>
            /// <exception cref="T:System.IO.IOException">An error occurred in a <see cref="T:System.IO.Stream" />, <see cref="T:System.Xml.XmlReader" /> or <see cref="T:System.IO.TextReader" /> object during a streaming operation.  For more information about streaming, see SqlClient Streaming Support.</exception>
            /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.Stream" />, <see cref="T:System.Xml.XmlReader" /> or <see cref="T:System.IO.TextReader" /> object was closed during a streaming operation.  For more information about streaming, see SqlClient Streaming Support.</exception>
            [NotNull]
            [ItemNotNull]
            public async Task<SqlDataReader> ExecuteReaderAsync(
                CommandBehavior behavior,
                CancellationToken cancellationToken)
            {
                SqlCommand command = Command;
                // Should never happen
                if (command == null) throw new ObjectDisposedException(nameof(ReaderDisposer));

                SqlDataReader reader =
                    await command.ExecuteReaderAsync(behavior, cancellationToken).ConfigureAwait(false);
                Reader = reader;
                SetCancellationToken(cancellationToken);
                Debug.Assert(reader != null, "reader != null");
                return reader;
            }

            /// <summary>Sends the <see cref="P:System.Data.SqlClient.SqlCommand.CommandText" /> to the <see cref="P:System.Data.SqlClient.SqlCommand.Connection" /> and builds an <see cref="T:System.Xml.XmlReader" /> object.</summary>
            /// <returns>An <see cref="T:System.Xml.XmlReader" /> object.</returns>
            /// <exception cref="T:System.InvalidCastException">A <see cref="P:System.Data.SqlClient.SqlParameter.SqlDbType" /> other than Binary or VarBinary was used when <see cref="P:System.Data.SqlClient.SqlParameter.Value" /> was set to <see cref="T:System.IO.Stream" />. For more information about streaming, see SqlClient Streaming Support.A <see cref="P:System.Data.SqlClient.SqlParameter.SqlDbType" /> other than Char, NChar, NVarChar, VarChar, or  Xml was used when <see cref="P:System.Data.SqlClient.SqlParameter.Value" /> was set to <see cref="T:System.IO.TextReader" />.A <see cref="P:System.Data.SqlClient.SqlParameter.SqlDbType" /> other than Xml was used when <see cref="P:System.Data.SqlClient.SqlParameter.Value" /> was set to <see cref="T:System.Xml.XmlReader" />.</exception>
            /// <exception cref="T:System.Data.SqlClient.SqlException">An exception occurred while executing the command against a locked row. This exception is not generated when you are using Microsoft .NET Framework version 1.0.A timeout occurred during a streaming operation. For more information about streaming, see SqlClient Streaming Support.</exception>
            /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Data.SqlClient.SqlConnection" /> closed or dropped during a streaming operation. For more information about streaming, see SqlClient Streaming Support.</exception>
            /// <exception cref="T:System.IO.IOException">An error occurred in a <see cref="T:System.IO.Stream" />, <see cref="T:System.Xml.XmlReader" /> or <see cref="T:System.IO.TextReader" /> object during a streaming operation.  For more information about streaming, see SqlClient Streaming Support.</exception>
            /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.Stream" />, <see cref="T:System.Xml.XmlReader" /> or <see cref="T:System.IO.TextReader" /> object was closed during a streaming operation.  For more information about streaming, see SqlClient Streaming Support.</exception>
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
            public XmlReader ExecuteXmlReader()
            {
                SqlCommand command = Command;
                // Should never happen
                if (command == null) throw new ObjectDisposedException(nameof(ReaderDisposer));

                XmlReader reader = command.ExecuteXmlReader();
                Reader = reader;
                SetCancellationToken();
                return reader;
            }

            /// <summary>An asynchronous version of <see cref="M:System.Data.SqlClient.SqlCommand.ExecuteXmlReader" />, which sends the <see cref="P:System.Data.SqlClient.SqlCommand.CommandText" /> to the <see cref="P:System.Data.SqlClient.SqlCommand.Connection" /> and builds an <see cref="T:System.Xml.XmlReader" /> object.Exceptions will be reported via the returned Task object.</summary>
            /// <returns>A task representing the asynchronous operation.</returns>
            /// <exception cref="T:System.InvalidCastException">A <see cref="P:System.Data.SqlClient.SqlParameter.SqlDbType" /> other than Binary or VarBinary was used when <see cref="P:System.Data.SqlClient.SqlParameter.Value" /> was set to <see cref="T:System.IO.Stream" />. For more information about streaming, see SqlClient Streaming Support.A <see cref="P:System.Data.SqlClient.SqlParameter.SqlDbType" /> other than Char, NChar, NVarChar, VarChar, or  Xml was used when <see cref="P:System.Data.SqlClient.SqlParameter.Value" /> was set to <see cref="T:System.IO.TextReader" />.A <see cref="P:System.Data.SqlClient.SqlParameter.SqlDbType" /> other than Xml was used when <see cref="P:System.Data.SqlClient.SqlParameter.Value" /> was set to <see cref="T:System.Xml.XmlReader" />.</exception>
            /// <exception cref="T:System.InvalidOperationException">Calling <see cref="M:System.Data.SqlClient.SqlCommand.ExecuteScalarAsync(System.Threading.CancellationToken)" /> more than once for the same instance before task completion.The <see cref="T:System.Data.SqlClient.SqlConnection" /> closed or dropped during a streaming operation. For more information about streaming, see SqlClient Streaming Support.Context Connection=true is specified in the connection string.</exception>
            /// <exception cref="T:System.Data.SqlClient.SqlException">SQL Server returned an error while executing the command text.A timeout occurred during a streaming operation. For more information about streaming, see SqlClient Streaming Support.</exception>
            /// <exception cref="T:System.IO.IOException">An error occurred in a <see cref="T:System.IO.Stream" />, <see cref="T:System.Xml.XmlReader" /> or <see cref="T:System.IO.TextReader" /> object during a streaming operation.  For more information about streaming, see SqlClient Streaming Support.</exception>
            /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.Stream" />, <see cref="T:System.Xml.XmlReader" /> or <see cref="T:System.IO.TextReader" /> object was closed during a streaming operation.  For more information about streaming, see SqlClient Streaming Support.</exception>
            [NotNull]
            [ItemNotNull]
            public async Task<XmlReader> ExecuteXmlReaderAsync(CancellationToken cancellationToken)
            {
                SqlCommand command = Command;
                // Should never happen
                if (command == null) throw new ObjectDisposedException(nameof(ReaderDisposer));

                // ReSharper disable once PossibleNullReferenceException
                XmlReader reader = await command.ExecuteXmlReaderAsync(cancellationToken).ConfigureAwait(false);
                Reader = reader;
                SetCancellationToken(cancellationToken);
                return reader;
            }
            #endregion
        }

        [NotNull]
        private ReaderDisposer CreateReaderDisposer()
        {
            ReaderDisposer disposer = new ReaderDisposer();
            try
            {
                disposer.Semaphore = WaitSemaphoresAsync().GetAwaiter().GetResult();
                disposer.Connection = new SqlConnection(_mapping.Connection.ConnectionString);
                disposer.Connection.Open();
                disposer.Command = new SqlCommand(_program.Name, disposer.Connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = (int)CommandTimeout.TotalSeconds
                };
                disposer.CommandTimeout = CommandTimeout;
                _setSqlParameterCollection(disposer.Command, _parameters);
                disposer.OutputParameters = _outputParameters;
                return disposer;
            }
            catch
            {
                disposer.Dispose();
                throw;
            }
        }

        [NotNull]
        [ItemNotNull]
        private async Task<ReaderDisposer> CreateReaderDisposerAsync(CancellationToken cancellationToken)
        {
            ReaderDisposer disposer = new ReaderDisposer();
            try
            {
                disposer.Semaphore = await WaitSemaphoresAsync(cancellationToken).ConfigureAwait(false);
                disposer.Connection = new SqlConnection(_mapping.Connection.ConnectionString);
                await disposer.Connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                disposer.Command = new SqlCommand(_program.Name, disposer.Connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = (int)CommandTimeout.TotalSeconds
                };
                disposer.CommandTimeout = CommandTimeout;
                _setSqlParameterCollection(disposer.Command, _parameters);
                disposer.OutputParameters = _outputParameters;
                return disposer;
            }
            catch
            {
                disposer.Dispose();
                throw;
            }
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

        [CanBeNull]
        private List<SqlProgramParameter, SqlParameter, IOut> _outputParameters;

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
            _program = program ?? throw new ArgumentNullException(nameof(program));
            _mapping = mapping ?? throw new ArgumentNullException(nameof(mapping));
            CommandTimeout = commandTimeout;
            // ReSharper disable once AssignNullToNotNullAttribute
            _parameters = _createSqlParameterCollection();
        }

        /// <summary>
        ///   Gets or sets the command timeout.
        ///   This is the time to wait for the program to execute.
        /// </summary>
        /// <remarks>
        /// Set to <see cref="TimeSpan.Zero"/> or <see cref="Timeout.InfiniteTimeSpan"/> to indicate no limit.
        /// Set to a negative time to reset back to the default timeout for the program.
        /// </remarks>
        /// <value>
        ///   The time to wait for the command to execute.
        /// </value>
        public TimeSpan CommandTimeout
        {
            get => _commandTimeout;
            set
            {
                if (_commandTimeout == value)
                    return;
                if (value == Timeout.InfiniteTimeSpan)
                    _commandTimeout = TimeSpan.Zero;
                else if (value < TimeSpan.Zero)
                    _commandTimeout = _program.DefaultCommandTimeout;
                else
                    _commandTimeout = value;
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
        /// <exception cref="ArgumentNullException"><paramref name="parameterName"/> is <see langword="null" />.</exception>
        /// <exception cref="LoggingException">Could not find a match with the <paramref name="parameterName" /> specified.</exception>
        [NotNull]
        public SqlParameter GetParameter([NotNull] string parameterName)
        {
            if (parameterName == null) throw new ArgumentNullException(nameof(parameterName));

            lock (_parameters)
            {
                int index = _parameters.IndexOf(parameterName, _mapping.Definition.ParameterNameComparer);
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
        /// <exception cref="ArgumentNullException"><paramref name="parameterName"/> is <see langword="null" />.</exception>
        /// <exception cref="LoggingException">Could not find a match with the <paramref name="parameterName" /> specified.</exception>
        /// <exception cref="DatabaseSchemaException"><para>The type <typeparamref name="T" /> is invalid for the <see cref="SqlProgramParameter.Direction" />.</para>
        /// <para>-or-</para>
        /// <para>The type <typeparamref name="T" /> was unsupported.</para>
        /// <para>-or-</para>
        /// <para>A fatal error occurred.</para>
        /// <para>-or-</para>
        /// <para>The object exceeded the SQL type's maximum <see cref="SqlTypeSize">size</see>.</para>
        /// <para>-or-</para>
        /// <para>The serialized object was truncated.</para>
        /// <para>-or-</para>
        /// <para>Unicode characters were found and only ASCII characters are supported in the SQL type.</para>
        /// <para>-or-</para>
        /// <para>The date was outside the range of accepted dates for the SQL type.</para></exception>
        [NotNull]
        public SqlParameter SetParameter<T>(
            [NotNull] string parameterName,
            T value,
            TypeConstraintMode mode = TypeConstraintMode.Warn)
        {
            if (parameterName == null) throw new ArgumentNullException(nameof(parameterName));

            lock (_parameters)
            {
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
                int index = _parameters.IndexOf(parameterName, _mapping.Definition.ParameterNameComparer);
                SqlParameter parameter = index < 0
                    ? _parameters.Add(parameterDefinition.CreateSqlParameter())
                    : _parameters[index];

                Debug.Assert(parameter != null);
                parameterDefinition.SetSqlParameterValue(parameter, value, mode);
                AddOutParameter(parameterDefinition, parameter, value as IOut);
                return parameter;
            }
        }

        /// <summary>
        /// Adds the output parameter value given if it isnt null.
        /// </summary>
        /// <param name="sqlProgramParameter">The program parameter the value is for.</param>
        /// <param name="sqlParameter">The sql parameter the value is for.</param>
        /// <param name="outValue">The output value.</param>
        private void AddOutParameter(
            [NotNull] SqlProgramParameter sqlProgramParameter,
            SqlParameter sqlParameter,
            IOut outValue)
        {
            if (outValue == null) return;
            if (_outputParameters == null)
                _outputParameters = new List<SqlProgramParameter, SqlParameter, IOut>();
            _outputParameters.Add(sqlProgramParameter, sqlParameter, outValue);
        }

        /// <summary>
        /// Waits the concurrency control semaphores.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        [NotNull]
        private Task<IDisposable> WaitSemaphoresAsync(CancellationToken token = default(CancellationToken))
        {
            // NOTE! Do NOT reorder these without also reordering the semaphores in SqlBatch.ExecuteInternal
            return AsyncSemaphore.WaitAllAsync(
                token,
                _mapping.Connection.Semaphore /* Connection */,
                _program.Connection.ConnectionSemaphore /* Load Balanced Connection */,
                _program.Connection.DatabaseSemaphore /* Database */,
                _program.Semaphore /* Program */);
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
        /// <exception cref="SqlProgramExecutionException">An error occured while executing the program.</exception>
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
                using (ReaderDisposer disposer = CreateReaderDisposer())
                    return (T)disposer.ExecuteScalar();
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
        /// <exception cref="SqlProgramExecutionException">An error occured while executing the program.</exception>
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
                using (ReaderDisposer disposer =
                    await CreateReaderDisposerAsync(cancellationToken).ConfigureAwait(false))
                {
                    return (T)await disposer
                        .ExecuteScalarAsync(cancellationToken)
                        .ConfigureAwait(false);
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
        /// <exception cref="SqlProgramExecutionException">An error occured while executing the program.</exception>
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
                using (ReaderDisposer disposer = CreateReaderDisposer())
                    return disposer.ExecuteNonQuery();
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
        /// <exception cref="SqlProgramExecutionException">An error occured while executing the program.</exception>
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
                using (ReaderDisposer disposer =
                    await CreateReaderDisposerAsync(cancellationToken).ConfigureAwait(false))
                {
                    return await disposer
                        .ExecuteNonQueryAsync(cancellationToken)
                        .ConfigureAwait(false);
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
        /// <exception cref="ArgumentNullException"><paramref name="resultAction"/> is <see langword="null" />.</exception>
        /// <exception cref="SqlProgramExecutionException">An error occured while executing the program.</exception>
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
                using (ReaderDisposer disposer = CreateReaderDisposer())
                    resultAction(disposer.ExecuteReader(behavior));
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
        /// <exception cref="ArgumentNullException"><paramref name="resultAction"/> is <see langword="null" />.</exception>
        /// <exception cref="SqlProgramExecutionException">An error occured while executing the program.</exception>
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

            ReaderDisposer disposer = null;
            try
            {
                disposer = CreateReaderDisposer();
                resultAction(disposer.ExecuteReader(behavior), disposer);
            }
            catch (Exception exception)
            {
                disposer?.Dispose();
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
        /// <exception cref="ArgumentNullException"><paramref name="resultFunc"/> is <see langword="null" />.</exception>
        /// <exception cref="SqlProgramExecutionException">An error occured while executing the program.</exception>
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
                using (ReaderDisposer disposer = CreateReaderDisposer())
                    return resultFunc(disposer.ExecuteReader(behavior));
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
        /// <exception cref="ArgumentNullException"><paramref name="resultFunc"/> is <see langword="null" />.</exception>
        /// <exception cref="SqlProgramExecutionException">An error occured while executing the program.</exception>
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

            ReaderDisposer disposer = null;
            try
            {
                disposer = CreateReaderDisposer();
                return resultFunc(disposer.ExecuteReader(behavior), disposer);
            }
            catch (Exception exception)
            {
                disposer?.Dispose();
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
        /// <exception cref="ArgumentNullException"><paramref name="resultAction"/> is <see langword="null" />.</exception>
        /// <exception cref="SqlProgramExecutionException">An error occured while executing the program.</exception>
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
                using (ReaderDisposer disposer =
                    await CreateReaderDisposerAsync(cancellationToken).ConfigureAwait(false))
                {
                    SqlDataReader reader = await disposer
                        .ExecuteReaderAsync(behavior, cancellationToken)
                        .ConfigureAwait(false);

                    // ReSharper disable once PossibleNullReferenceException
                    await resultAction(reader, disposer.CancellationToken).ConfigureAwait(false);
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
        /// <exception cref="ArgumentNullException"><paramref name="resultAction"/> is <see langword="null" />.</exception>
        /// <exception cref="SqlProgramExecutionException">An error occured while executing the program.</exception>
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

            ReaderDisposer disposer = null;
            try
            {
                disposer = await CreateReaderDisposerAsync(cancellationToken).ConfigureAwait(false);

                SqlDataReader reader = await disposer
                    .ExecuteReaderAsync(behavior, cancellationToken)
                    .ConfigureAwait(false);

                // ReSharper disable once PossibleNullReferenceException
                await resultAction(reader, disposer, disposer.CancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                disposer?.Dispose();
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
        /// <exception cref="ArgumentNullException"><paramref name="resultFunc"/> is <see langword="null" />.</exception>
        /// <exception cref="SqlProgramExecutionException">An error occured while executing the program.</exception>
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
                using (ReaderDisposer disposer =
                    await CreateReaderDisposerAsync(cancellationToken).ConfigureAwait(false))
                {
                    SqlDataReader reader = await disposer
                        .ExecuteReaderAsync(behavior, cancellationToken)
                        .ConfigureAwait(false);

                    // ReSharper disable once PossibleNullReferenceException
                    return await resultFunc(reader, disposer.CancellationToken).ConfigureAwait(false);
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
        /// <exception cref="ArgumentNullException"><paramref name="resultFunc"/> is <see langword="null" />.</exception>
        /// <exception cref="SqlProgramExecutionException">An error occured while executing the program.</exception>
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

            ReaderDisposer disposer = null;
            try
            {
                disposer = await CreateReaderDisposerAsync(cancellationToken).ConfigureAwait(false);

                SqlDataReader reader = await disposer
                    .ExecuteReaderAsync(behavior, cancellationToken)
                    .ConfigureAwait(false);

                // ReSharper disable once PossibleNullReferenceException
                return await resultFunc(reader, disposer, disposer.CancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                disposer?.Dispose();
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
        /// <exception cref="ArgumentNullException"><paramref name="resultAction"/> is <see langword="null" />.</exception>
        /// <exception cref="SqlProgramExecutionException">An error occured while executing the program.</exception>
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
                using (ReaderDisposer disposer = CreateReaderDisposer())
                    resultAction(disposer.ExecuteXmlReader());
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
        /// <exception cref="ArgumentNullException"><paramref name="resultAction"/> is <see langword="null" />.</exception>
        /// <exception cref="SqlProgramExecutionException">An error occured while executing the program.</exception>
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

            ReaderDisposer disposer = null;
            try
            {
                disposer = CreateReaderDisposer();
                resultAction(disposer.ExecuteXmlReader(), disposer);
            }
            catch (Exception exception)
            {
                disposer?.Dispose();
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
        /// <exception cref="ArgumentNullException"><paramref name="resultFunc"/> is <see langword="null" />.</exception>
        /// <exception cref="SqlProgramExecutionException">An error occured while executing the program.</exception>
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
                using (ReaderDisposer disposer = CreateReaderDisposer())
                    return resultFunc(disposer.ExecuteXmlReader());
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
        /// <exception cref="ArgumentNullException"><paramref name="resultFunc"/> is <see langword="null" />.</exception>
        /// <exception cref="SqlProgramExecutionException">An error occured while executing the program.</exception>
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

            ReaderDisposer disposer = null;
            try
            {
                disposer = CreateReaderDisposer();
                return resultFunc(disposer.ExecuteXmlReader(), disposer);
            }
            catch (Exception exception)
            {
                disposer?.Dispose();
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
        /// <exception cref="ArgumentNullException"><paramref name="resultAction"/> is <see langword="null" />.</exception>
        /// <exception cref="SqlProgramExecutionException">An error occured while executing the program.</exception>
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
                using (ReaderDisposer disposer =
                    await CreateReaderDisposerAsync(cancellationToken).ConfigureAwait(false))
                {
                    XmlReader reader = await disposer
                        .ExecuteXmlReaderAsync(cancellationToken)
                        .ConfigureAwait(false);

                    // ReSharper disable once PossibleNullReferenceException
                    await resultAction(reader, disposer.CancellationToken).ConfigureAwait(false);
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
        /// <exception cref="ArgumentNullException"><paramref name="resultAction"/> is <see langword="null" />.</exception>
        /// <exception cref="SqlProgramExecutionException">An error occured while executing the program.</exception>
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

            ReaderDisposer disposer = null;
            try
            {
                disposer = await CreateReaderDisposerAsync(cancellationToken).ConfigureAwait(false);

                XmlReader reader = await disposer
                    .ExecuteXmlReaderAsync(cancellationToken)
                    .ConfigureAwait(false);

                // ReSharper disable once PossibleNullReferenceException
                await resultAction(reader, disposer, disposer.CancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                disposer?.Dispose();
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
        /// <exception cref="ArgumentNullException"><paramref name="resultFunc"/> is <see langword="null" />.</exception>
        /// <exception cref="SqlProgramExecutionException">An error occured while executing the program.</exception>
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
                using (ReaderDisposer disposer =
                    await CreateReaderDisposerAsync(cancellationToken).ConfigureAwait(false))
                {
                    XmlReader reader = await disposer
                        .ExecuteXmlReaderAsync(cancellationToken)
                        .ConfigureAwait(false);

                    // ReSharper disable once PossibleNullReferenceException
                    return await resultFunc(reader, disposer.CancellationToken).ConfigureAwait(false);
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
        /// <exception cref="ArgumentNullException"><paramref name="resultFunc"/> is <see langword="null" />.</exception>
        /// <exception cref="SqlProgramExecutionException">An error occured while executing the program.</exception>
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

            ReaderDisposer disposer = null;
            try
            {
                disposer = await CreateReaderDisposerAsync(cancellationToken).ConfigureAwait(false);

                XmlReader reader = await disposer
                    .ExecuteXmlReaderAsync(cancellationToken)
                    .ConfigureAwait(false);

                // ReSharper disable once PossibleNullReferenceException
                return await resultFunc(reader, disposer, disposer.CancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                disposer?.Dispose();
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