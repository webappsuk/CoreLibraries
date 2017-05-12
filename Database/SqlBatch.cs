#region © Copyright Web Applications (UK) Ltd, 2017.  All rights reserved.
// Copyright (c) 2017, Web Applications UK Ltd
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

// TODO Remove when migrated to .net standard project format
#define NET452

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NodaTime;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.Database
{
    /// <summary>
    /// Allows multiple <see cref="SqlProgram">SqlPrograms</see> to be executed in a single database call.
    /// </summary>
    public partial class SqlBatch : IReadOnlyList<SqlBatchCommand>
    {
        private const int Building = 0;
        private const int Executing = 1;
        private const int Completed = 2;

        [NotNull]
        private readonly object _addLock = new object();

        [NotNull]
        private readonly AsyncLock _executeLock = new AsyncLock();

        private int _state = Building;

        [NotNull]
        [ItemNotNull]
        private readonly List<SqlBatchCommand> _commands = new List<SqlBatchCommand>();

        private Duration _batchTimeout;

        [NotNull]
        private readonly ResettableLazy<HashSet<string>> _commonConnectionStrings;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlBatch"/> class.
        /// </summary>
        /// <param name="batchTimeout">The batch timeout. Defaults to 30 seconds.</param>
        public SqlBatch(Duration? batchTimeout = null)
        {
            BatchTimeout = batchTimeout ?? Duration.FromSeconds(30);
            _commonConnectionStrings = new ResettableLazy<HashSet<string>>(GetCommonConnections);
        }

        /// <summary>
        ///   Gets or sets the batch timeout.
        ///   This is the time to wait for the batch to execute.
        /// </summary>
        /// <remarks>
        /// Set to <see cref="Duration.Zero"/> or <see cref="TimeHelpers.InfiniteDuration"/> to indicate no limit.
        /// </remarks>
        /// <value>
        ///   The time to wait for the batch to execute.
        /// </value>
        public Duration BatchTimeout
        {
            get => _batchTimeout;
            set
            {
                if (_batchTimeout == value)
                    return;
                if (value == TimeHelpers.InfiniteDuration)
                    _batchTimeout = Duration.Zero;
                else if (value < Duration.Zero)
                    throw new ArgumentOutOfRangeException(nameof(value));
                else
                    _batchTimeout = value;
            }
        }

        /// <summary>
        /// Gets the connection strings which are common to all the commands that have been added to this batch.
        /// </summary>
        /// <value>
        /// The common connection strings.
        /// </value>
        [NotNull]
        [ItemNotNull]
        public IReadOnlyCollection<string> CommonConnectionStrings
        {
            get
            {
                lock (_addLock)
                {
                    return _commonConnectionStrings.Value
#if NET452
                            .ToArray()
#endif
                        ;
                }
            }
        }

        /// <summary>Gets the number of elements in the collection.</summary>
        /// <returns>The number of elements in the collection. </returns>
        public int Count => _commands.Count;

        /// <summary>Gets the element at the specified index in the read-only list.</summary>
        /// <returns>The element at the specified index in the read-only list.</returns>
        /// <param name="index">The zero-based index of the element to get. </param>
        public SqlBatchCommand this[int index] => _commands[index];

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<SqlBatchCommand> GetEnumerator() => _commands.GetEnumerator();
        
        /// <summary>
        /// Checks the state is valid for adding to the batch.
        /// </summary>
        private void CheckState()
        {
            if (_state != Building)
            {
                throw new InvalidOperationException(
                    _state == Executing
                        ? Resources.SqlBatch_CheckState_Executing
                        : Resources.SqlBatch_CheckState_Completed);
            }
        }

        /// <summary>
        /// Adds the command given to the batch.
        /// </summary>
        /// <param name="command">The command.</param>
        private void AddCommand([NotNull] SqlBatchCommand command)
        {
            lock (_addLock)
            {
                CheckState();

                if (_commands.Count >= ushort.MaxValue)
                    throw new InvalidOperationException("Only allowed 65536 commands per batch.");

                command.Id = (ushort)_commands.Count;
                _commands.Add(command);
                _commonConnectionStrings.Reset();
            }
        }
        
        /// <summary>
        /// Adds the specified program to the batch.
        /// The first column of the first row in the result set returned by the query will be returned from the <see cref="SqlBatchResult{T}" />.
        /// Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="TOut">The output type expected.</typeparam>
        /// <param name="program">The program to add to the batch.</param>
        /// <param name="result">A <see cref="SqlBatchResult{T}"/> which can be used to get the scalar value returned by the program.</param>
        /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
        /// <returns>This <see cref="SqlBatch"/> instance.</returns>
        public SqlBatch AddExecuteScalar<TOut>(
            [NotNull] SqlProgram program,
            [NotNull] out SqlBatchResult<TOut> result,
            [CanBeNull] SetBatchParametersDelegate setParameters = null)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));
            CheckState();

            SqlBatchCommand.Scalar<TOut> command = new SqlBatchCommand.Scalar<TOut>(this, program, setParameters);
            AddCommand(command);
            result = command.Result;

            return this;
        }

        /// <summary>
        /// Adds the specified program to the batch. 
        /// The number of records affected by the program will be returned from the <see cref="SqlBatchResult{T}"/>.
        /// </summary>
        /// <param name="program">The program to add to the batch.</param>
        /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
        /// <param name="result">A <see cref="SqlBatchResult{T}" /> which can be used to get the number of records affected by the program.</param>
        /// <returns>This <see cref="SqlBatch"/> instance.</returns>
        public SqlBatch AddExecuteNonQuery(
            [NotNull] SqlProgram program,
            [NotNull] out SqlBatchResult<int> result,
            [CanBeNull] SetBatchParametersDelegate setParameters = null)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));
            CheckState();

            SqlBatchCommand.NonQuery command = new SqlBatchCommand.NonQuery(this, program, setParameters);
            AddCommand(command);
            result = command.Result;

            return this;
        }

        /// <summary>
        /// Adds the specified program to the batch.
        /// </summary>
        /// <param name="program">The program to add to the batch.</param>
        /// <param name="resultAction">The action used to process the result.</param>
        /// <param name="behavior">The query's effect on the database.</param>
        /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
        /// <param name="result">A <see cref="SqlBatchResult" /> which can be used to wait for the program to finish executing.</param>
        /// <returns>This <see cref="SqlBatch"/> instance.</returns>
        public SqlBatch AddExecuteReader(
            [NotNull] SqlProgram program,
            [NotNull] ResultDelegateAsync resultAction,
            [NotNull] out SqlBatchResult result,
            CommandBehavior behavior = CommandBehavior.Default,
            [CanBeNull] SetBatchParametersDelegate setParameters = null)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));
            if (resultAction == null) throw new ArgumentNullException(nameof(resultAction));
            if ((behavior & CommandBehavior.CloseConnection) != 0)
                throw new ArgumentOutOfRangeException(
                    nameof(behavior),
                    "CommandBehavior.CloseConnection is not supported");
            CheckState();

            SqlBatchCommand.Reader command = new SqlBatchCommand.Reader(
                this,
                program,
                resultAction,
                behavior,
                setParameters);
            AddCommand(command);
            result = command.Result;

            return this;
        }
        
        /// <summary>
        /// Adds the specified program to the batch. 
        /// The value returned by the <paramref name="resultFunc"/> will be returned by the <see cref="SqlBatchResult{T}"/>.
        /// </summary>
        /// <typeparam name="TOut">The type of the result.</typeparam>
        /// <param name="program">The program to add to the batch.</param>
        /// <param name="resultFunc">The function used to process the result.</param>
        /// <param name="behavior">The query's effect on the database.</param>
        /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
        /// <param name="result">A <see cref="SqlBatchResult{T}" /> which can be used to wait for the program to finish executing
        /// and get the value returned by <paramref name="resultFunc"/>.</param>
        /// <returns>This <see cref="SqlBatch"/> instance.</returns>
        public SqlBatch AddExecuteReader<TOut>(
            [NotNull] SqlProgram program,
            [NotNull] ResultDelegateAsync<TOut> resultFunc,
            [NotNull] out SqlBatchResult<TOut> result,
            CommandBehavior behavior = CommandBehavior.Default,
            [CanBeNull] SetBatchParametersDelegate setParameters = null)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));
            if (resultFunc == null) throw new ArgumentNullException(nameof(resultFunc));
            if ((behavior & CommandBehavior.CloseConnection) != 0)
                throw new ArgumentOutOfRangeException(
                    nameof(behavior),
                    "CommandBehavior.CloseConnection is not supported");
            CheckState();

            SqlBatchCommand.Reader<TOut> command = new SqlBatchCommand.Reader<TOut>(
                this,
                program,
                resultFunc,
                behavior,
                setParameters);
            AddCommand(command);
            result = command.Result;

            return this;
        }
        
        /// <summary>
        /// Adds the specified program to the batch.
        /// </summary>
        /// <param name="program">The program to add to the batch.</param>
        /// <param name="resultAction">The action used to process the result.</param>
        /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
        /// <param name="result">A <see cref="SqlBatchResult" /> which can be used to wait for the program to finish executing.</param>
        /// <returns>This <see cref="SqlBatch"/> instance.</returns>
        public SqlBatch AddExecuteXmlReader(
            [NotNull] SqlProgram program,
            [NotNull] XmlResultDelegateAsync resultAction,
            [NotNull] out SqlBatchResult result,
            [CanBeNull] SetBatchParametersDelegate setParameters = null)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));
            if (resultAction == null) throw new ArgumentNullException(nameof(resultAction));
            CheckState();

            SqlBatchCommand.XmlReader command = new SqlBatchCommand.XmlReader(
                this,
                program,
                resultAction,
                setParameters);
            AddCommand(command);
            result = command.Result;

            return this;
        }
        /// <summary>
        /// Adds the specified program to the batch. 
        /// The value returned by the <paramref name="resultFunc"/> will be returned by the <see cref="SqlBatchResult{T}"/>.
        /// </summary>
        /// <typeparam name="TOut">The type of the result.</typeparam>
        /// <param name="program">The program to add to the batch.</param>
        /// <param name="resultFunc">The function used to process the result.</param>
        /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
        /// <param name="result">A <see cref="SqlBatchResult{T}" /> which can be used to wait for the program to finish executing
        /// and get the value returned by <paramref name="resultFunc"/>.</param>
        /// <returns>This <see cref="SqlBatch"/> instance.</returns>
        public SqlBatch AddExecuteXmlReader<TOut>(
            [NotNull] SqlProgram program,
            [NotNull] XmlResultDelegateAsync<TOut> resultFunc,
            [NotNull] out SqlBatchResult<TOut> result,
            [CanBeNull] SetBatchParametersDelegate setParameters = null)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));
            if (resultFunc == null) throw new ArgumentNullException(nameof(resultFunc));
            CheckState();

            SqlBatchCommand.XmlReader<TOut> command = new SqlBatchCommand.XmlReader<TOut>(
                this,
                program,
                resultFunc,
                setParameters);
            AddCommand(command);
            result = command.Result;

            return this;
        }

        // TODO Add nested batch

        /// <summary>
        /// Executes the batch on a single connection, asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token which can be used to cancel the entire batch operation.</param>
        /// <returns>An awaitable task which completes when the batch is complete.</returns>
        public async Task ExecuteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            lock (_addLock)
            {
                if (_commands.Count < 1)
                    throw new InvalidOperationException(Resources.SqlBatch_ExecuteAsync_Empty);

                if (Interlocked.CompareExchange(ref _state, Executing, Building) == Completed) return;
            }

            using (await _executeLock.LockAsync(cancellationToken).ConfigureAwait(false))
            {
                if (_state == Completed) return;

                try
                {
                    string connectionString = DetermineConnection();

                    // Set the result count for each command to the number of connections
                    foreach (SqlBatchCommand command in _commands)
                        command.Result.SetResultCount(1);

                    await ExecuteInternal(connectionString, 0, cancellationToken).ConfigureAwait(false);
                }
                finally
                {
                    _state = Completed;
                }
            }
        }

        /// <summary>
        /// Executes the batch on all supported connections, asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token which can be used to cancel the entire batch operation.</param>
        /// <returns>An awaitable task which completes when the batch is complete.</returns>
        public async Task ExecuteAllAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            lock (_addLock)
            {
                if (_commands.Count < 1)
                    throw new InvalidOperationException(Resources.SqlBatch_ExecuteAsync_Empty);

                if (Interlocked.CompareExchange(ref _state, Executing, Building) == Completed) return;
            }

            using (await _executeLock.LockAsync(cancellationToken).ConfigureAwait(false))
            {
                if (_state == Completed) return;

                try
                {
                    // Get the connection strings which are common to each program
                    HashSet<string> commonConnections = _commonConnectionStrings.Value;
                    Debug.Assert(commonConnections != null, "commonConnections != null");

                    // Set the result count for each command to the number of connections
                    foreach (SqlBatchCommand command in _commands)
                        command.Result.SetResultCount(commonConnections.Count);

                    Task[] tasks = commonConnections
                        .Select((cs, i) => Task.Run(() => ExecuteInternal(cs, i, cancellationToken)))
                        .ToArray();

                    await Task.WhenAll(tasks).ConfigureAwait(false);
                }
                finally
                {
                    _state = Completed;
                }
            }
        }

        /// <summary>
        /// Begins executing the batch if it is not already executing.
        /// </summary>
        /// <param name="all">if set to <see langword="true" /> execute the batch on all connections.</param>
        internal void BeginExecute(bool all)
        {
            // If the state is Executing or Completed, don't need to do anything.
            lock (_addLock)
                if (_state != Building)
                    return;

            // Execute the batch
            if (all)
                Task.Run(() => ExecuteAllAsync());
            else
                Task.Run(() => ExecuteAsync());
        }

        /// <summary>
        /// Indicates the next record set will be for the output parameters for the command.
        /// </summary>
        private const string OutputState = "Output";

        /// <summary>
        /// Indicates the command has ended.
        /// </summary>
        private const string EndState = "End";

        /// <summary>
        /// Executes the batch.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="connectionIndex">Index of the connection.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        [NotNull]
        private async Task ExecuteInternal(
            [NotNull] string connectionString,
            int connectionIndex,
            CancellationToken cancellationToken)
        {
            SqlStringBuilder sqlBuilder = new SqlStringBuilder();
            string uid = $"{Guid.NewGuid():B}@{DateTime.UtcNow:O}:";

            List<DbParameter> allParameters = new List<DbParameter>();

            Dictionary<IOut, DbParameter> outParameters = new Dictionary<IOut, DbParameter>();

            Dictionary<SqlBatchCommand, IReadOnlyList<(DbBatchParameter param, IOut output)>> commandOutParams =
                new Dictionary<SqlBatchCommand, IReadOnlyList<(DbBatchParameter param, IOut output)>>();

            // Build the batch SQL and get the parameters to the commands
            PreProcess(
                uid,
                connectionString,
                allParameters,
                outParameters,
                commandOutParams,
                sqlBuilder,
                out AsyncSemaphore[] semaphores,
                out CommandBehavior allBehavior);

            string state = null;
            int index = -1;
            int actualIndex = 0;
            DbBatchDataReader commandReader = null;

            void MessageHandler(string message)
            {
                if (!TryParseInfoMessage(message, out var info)) return;

                state = info.state;
                index = info.index;

                if (commandReader != null)
                    commandReader.State = BatchReaderState.Finished;
            }

            using (await AsyncSemaphore.WaitAllAsync(cancellationToken, semaphores).ConfigureAwait(false))
            using (DbConnection dbConnection = await CreateOpenConnectionAsync(connectionString, uid, MessageHandler, cancellationToken)
                    .ConfigureAwait(false))
            using (DbCommand dbCommand = CreateCommand(sqlBuilder.ToString(), dbConnection, allParameters.ToArray()))
            using (DbDataReader reader = await dbCommand.ExecuteReaderAsync(allBehavior, cancellationToken)
                .ConfigureAwait(false))
            {
                Debug.Assert(reader != null, "reader != null");

                object[] values = null;

                List<SqlBatchCommand>.Enumerator commandEnumerator = _commands.GetEnumerator();
                while (commandEnumerator.MoveNext())
                {
                    SqlBatchCommand command = commandEnumerator.Current;
                    Debug.Assert(command != null, "command != null");

                    try
                    {
                        using (commandReader = CreateReader(reader, command.CommandBehavior))
                        {
                            if (index >= actualIndex)
                                commandReader.State = BatchReaderState.Finished;

                            await command.HandleCommandAsync(
                                    commandReader,
                                    dbCommand,
                                    connectionIndex,
                                    cancellationToken)
                                .ConfigureAwait(false);

                            if (commandReader.IsOpen)
                            {
                                // Read the rest of the result sets until an info message finishes the reader
                                await commandReader.ReadTillClosedAsync(cancellationToken).ConfigureAwait(false);
                            }
                        }
                        commandReader = null;

                        // Check the end states
                        while (state != EndState && index == actualIndex)
                        {
                            if (state == OutputState)
                            {
                                // Get the expected output parameters for the command
                                if (!commandOutParams.TryGetValue(command, out var outs))
                                    throw new NotImplementedException(
                                        "Proper exception, unexpected output parameters");

                                // No longer expect the output parameters
                                commandOutParams.Remove(command);

                                if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                                    throw new NotImplementedException("Proper exception, missing data");

                                if (outs.Count != reader.VisibleFieldCount)
                                    throw new NotImplementedException("Proper exception, field count mismatch");

                                // Expand the values buffer if needed
                                if (values == null)
                                    values = new object[reader.VisibleFieldCount];
                                else if (values.Length < reader.VisibleFieldCount)
                                    Array.Resize(ref values, reader.VisibleFieldCount);

                                // Get the output values record
                                reader.GetValues(values);

                                // Set the output values
                                for (int i = 0; i < outs.Count; i++)
                                {
                                    Debug.Assert(outs[i].output != null, "outs[i].output != null");
                                    Debug.Assert(outs[i].param != null, "outs[i].param != null");

                                    outs[i].output.SetOutputValue(values[i], outs[i].param.BaseParameter);
                                }

                                if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                                    throw new NotImplementedException("Proper exception, unexpected data");

                                // TODO Do something with this?
                                bool hasNext = await reader.NextResultAsync(cancellationToken)
                                    .ConfigureAwait(false);
                            }
                            else
                                throw new NotImplementedException("Proper exception, unexpected state");
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        Debug.Assert(cancellationToken.IsCancellationRequested);
                        command.Result.SetCanceled(connectionIndex);

                        while (commandEnumerator.MoveNext())
                            commandEnumerator.Current.Result.SetCanceled(connectionIndex);
                        commandEnumerator.Dispose();

                        throw;
                    }
                    catch (Exception e)
                    {
                        command.Result.SetException(connectionIndex, e);

                        while (commandEnumerator.MoveNext())
                            commandEnumerator.Current.Result.SetCanceled(connectionIndex);
                        commandEnumerator.Dispose();

                        throw;
                    }
                    finally
                    {
                        command.Result.SetCompleted(connectionIndex);
                    }

                    actualIndex++;
                }
            }
        }

        /// <summary>
        /// Pre-processes the commands to be executed
        /// </summary>
        /// <param name="uid">The uid.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="allParameters">All parameters.</param>
        /// <param name="outParameters">The out parameters.</param>
        /// <param name="commandOutParams">The command out parameters.</param>
        /// <param name="sqlBuilder">The SQL builder.</param>
        /// <param name="semaphores">The semaphores.</param>
        /// <param name="allBehavior">All behavior.</param>
        private void PreProcess(
            [NotNull] string uid,
            [NotNull] string connectionString,
            [NotNull] List<DbParameter> allParameters,
            [NotNull] Dictionary<IOut, DbParameter> outParameters,
            [NotNull] Dictionary<SqlBatchCommand, IReadOnlyList<(DbBatchParameter, IOut)>> commandOutParams,
            [NotNull] SqlStringBuilder sqlBuilder,
            [NotNull] out AsyncSemaphore[] semaphores,
            out CommandBehavior allBehavior)
        {
            HashSet<AsyncSemaphore> connectionSemaphores = new HashSet<AsyncSemaphore>();
            HashSet<AsyncSemaphore> loadBalConnectionSemaphores = new HashSet<AsyncSemaphore>();
            HashSet<AsyncSemaphore> databaseSemaphores = new HashSet<AsyncSemaphore>();

            // Start the behavior asking for sequential access. All commands must want it to be able to use it
            allBehavior = CommandBehavior.SequentialAccess;

            int commandIndex = 0;
            foreach (SqlBatchCommand command in _commands)
            {
                // Get the parameters for the command
                SqlBatchParametersCollection parameters = command.GetParametersForConnection(connectionString);

                // Add the parameters to the collection to pass to the command
                foreach (DbBatchParameter batchParameter in parameters.Parameters)
                {
                    if (batchParameter.OutputValue != null)
                    {
                        if (!outParameters.TryGetValue(batchParameter.OutputValue, out DbParameter dbParameter))
                            throw new NotImplementedException("proper error");
                        batchParameter.BaseParameter = dbParameter;
                    }
                    else
                    {
                        allParameters.Add(batchParameter.BaseParameter);
                    }
                }

                // Add any output parameters to the dictionary for passing into following commands
                if (parameters.OutputParameters != null)
                {
                    foreach ((DbBatchParameter batchParameter, IOut outValue) in parameters.OutputParameters)
                    {
                        if (outParameters.ContainsKey(outValue))
                            throw new NotImplementedException("proper error");

                        outParameters.Add(outValue, (SqlParameter)batchParameter.BaseParameter);
                    }

                    commandOutParams.Add(command, parameters.OutputParameters);
                }

                SqlProgramMapping mapping = parameters.Mapping;
                SqlProgram program = command.Program;
                LoadBalancedConnection loadBalancedConnection = program.Connection;
                Connection connection = mapping.Connection;

                // Need to wait on the semaphores for all the connections and databases
                if (connection.Semaphore != null)
                    connectionSemaphores.Add(connection.Semaphore);
                if (loadBalancedConnection.ConnectionSemaphore != null)
                    loadBalConnectionSemaphores.Add(loadBalancedConnection.ConnectionSemaphore);
                if (loadBalancedConnection.DatabaseSemaphore != null)
                    databaseSemaphores.Add(loadBalancedConnection.DatabaseSemaphore);

                // The mask the behavior with this commands behavior
                allBehavior &= command.CommandBehavior;

                // Build batch SQL
                sqlBuilder
                    .Append("-- ")
                    .AppendLine(command.Program.Name);

                command.AppendExecuteSql(sqlBuilder, parameters);

                if (parameters.OutputParameters != null)
                {
                    AppendInfo(sqlBuilder, uid, OutputState, commandIndex)
                        .Append("SELECT ");

                    bool firstParam = true;
                    foreach ((DbBatchParameter batchParameter, IOut outValue) in parameters.OutputParameters)
                    {
                        if (firstParam) firstParam = false;
                        else sqlBuilder.Append(", ");

                        sqlBuilder.Append(batchParameter.BaseParameter.ParameterName);
                    }
                    sqlBuilder.AppendLine(";");
                }

                AppendInfo(sqlBuilder, uid, EndState, commandIndex).AppendLine();

                commandIndex++;
            }

            // Concat the semaphores to a single array
            int semaphoreCount =
                connectionSemaphores.Count + loadBalConnectionSemaphores.Count + databaseSemaphores.Count;
            if (semaphoreCount < 1)
                semaphores = Array<AsyncSemaphore>.Empty;
            else
            {
                semaphores = new AsyncSemaphore[semaphoreCount];
                int i = 0;

                // NOTE! Do NOT reorder these without also reordering the semaphores in SqlProgramCommand.WaitSemaphoresAsync
                foreach (AsyncSemaphore semaphore in connectionSemaphores)
                    semaphores[i++] = semaphore;
                foreach (AsyncSemaphore semaphore in loadBalConnectionSemaphores)
                    semaphores[i++] = semaphore;
                foreach (AsyncSemaphore semaphore in databaseSemaphores)
                    semaphores[i++] = semaphore;
            }
        }

        // this would be provider specific
        [NotNull]
        private static SqlStringBuilder AppendInfo(
            [NotNull] SqlStringBuilder sqlBuilder,
            [NotNull] string uid,
            [NotNull] string state,
            int index)
        {
            Debug.Assert(!state.Contains(":"));
            return sqlBuilder
                .Append("RAISERROR(")
                .AppendVarChar($"{uid}{state}:{index}")
                .Append(",4,")
                .Append(unchecked((byte)index))
                .AppendLine(");");
        }

        private static bool TryParseInfoMessage([NotNull] string message, out (string state, int index) info)
        {
            int ind = message.IndexOf(':');
            if (ind < 0 || !ushort.TryParse(message.Substring(ind + 1), out ushort index))
            {
                info = (null, -1);
                return false;
            }

            info = (message.Substring(0, ind), index);
            return true;
        }

        private void RegisterInfoMessageHandler(
            [NotNull] DbConnection connection,
            [NotNull] string uid,
            [NotNull] Action<string> handler)
        {
            switch (connection)
            {
                case SqlConnection sqlConnection:
                    sqlConnection.InfoMessage += (sender, args) =>
                    {
                        foreach (SqlError error in args.Errors)
                        {
                            // Check if the error is one we care about
                            // The RAISERROR will return specific values to avoid conflicting with user RAISERRORs
                            // Class == 4 was chosen as the built in messages use 0 or 10+, so 4 should rarely happen
                            // Number == 50000 is the message ID returned when a string is passed to RAISERROR
                            // The message also starts with a specific string
                            if (error.Class == 4 &&
                                error.Number == 50000 &&
                                error.Message.StartsWith(uid))
                            {
                                handler(error.Message.Substring(uid.Length));
                            }
                        }
                    };
                    break;
                default:
                    // Eventually will support multiple db providers
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Creates and opens a connection asynchronously.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="uid">The uid.</param>
        /// <param name="messageHandler">The message handler.</param>
        /// <param name="cancellationToken">A cancellation token which can be used to cancel the entire batch operation.</param>
        [NotNull]
        [ItemNotNull]
        private async Task<DbConnection> CreateOpenConnectionAsync(
            string connectionString,
            string uid,
            Action<string> messageHandler,
            CancellationToken cancellationToken)
        {
            DbConnection dbConnection = null;
            try
            {
                dbConnection = new SqlConnection(connectionString);

                RegisterInfoMessageHandler(dbConnection, uid, messageHandler);

                await dbConnection.OpenAsync(cancellationToken).ConfigureAwait(false);

                return dbConnection;
            }
            catch
            {
                dbConnection.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Creates a command for the batch.
        /// </summary>
        /// <param name="text">The text to execute.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        [NotNull]
        private DbCommand CreateCommand(
            [NotNull] string text,
            [NotNull] DbConnection connection,
            [NotNull] DbParameter[] parameters)
        {
            DbCommand command = connection.CreateCommand();
            try
            {
                Debug.Assert(command.Connection == connection);

                command.CommandText = text;
                command.CommandType = CommandType.Text;
                command.CommandTimeout = (int)BatchTimeout.TotalSeconds();
                command.Parameters.AddRange(parameters);

                return command;
            }
            catch
            {
                command.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Creates the <see cref="DbBatchDataReader" /> for the underlying reader given.
        /// </summary>
        /// <param name="reader">The base reader.</param>
        /// <param name="commandBehavior">The command behavior.</param>
        /// <returns></returns>
        [NotNull]
        private DbBatchDataReader CreateReader([NotNull] DbDataReader reader, CommandBehavior commandBehavior)
        {
            switch (reader)
            {
                case SqlDataReader sqlReader:
                    return new SqlBatchDataReader(sqlReader, commandBehavior);
                default:
                    // Eventually will support multiple db providers
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Determines the connection string to use.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">
        /// </exception>
        [NotNull]
        private string DetermineConnection()
        {
            Debug.Assert(_commands.Count > 0);

            // Get the connection strings which are common to each program
            HashSet<string> commonConnections = _commonConnectionStrings.Value;

            // If there is a single common connection string, just use that
            if (commonConnections.Count == 1)
                return commonConnections.First();

            Debug.Assert(commonConnections != null);

            Dictionary<string, WeightCounter> connWeightCounts =
                new Dictionary<string, WeightCounter>();

            foreach (SqlBatchCommand command in _commands)
            {
                SqlProgramMapping[] mappings = command.Program.Mappings
                    .Where(m => commonConnections.Contains(m.Connection.ConnectionString))
                    .ToArray();

                double totWeight = mappings.Sum(m => m.Connection.Weight);

                foreach (SqlProgramMapping mapping in mappings)
                {
                    if (!connWeightCounts.TryGetValue(mapping.Connection.ConnectionString, out var counter))
                    {
                        counter = new WeightCounter();
                        connWeightCounts.Add(mapping.Connection.ConnectionString, counter);
                    }

                    counter.Increment(mapping.Connection.Weight / totWeight);
                }
            }

            return connWeightCounts.Choose(kvp => kvp.Value.GetWeight()).Key;
        }

        /// <summary>
        /// Gets the connection strings common to all commands.
        /// </summary>
        /// <returns>The set of common connections.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// </exception>
        [NotNull]
        [ItemNotNull]
        private HashSet<string> GetCommonConnections()
        {
            string commonConnectionStr = null;
            HashSet<string> commonConnections = null;
            foreach (SqlBatchCommand command in _commands)
            {
                if (commonConnections == null)
                {
                    commonConnections = new HashSet<string>(
                        command.Program.Mappings
                            .Where(m => m.Connection.Weight > 0)
                            .Select(m => m.Connection.ConnectionString));
                }
                else
                {
                    // If there's a single common connection, just check if any mapping has that connection.
                    if (commonConnectionStr != null)
                    {
                        bool contains = false;
                        foreach (SqlProgramMapping mapping in command.Program.Mappings)
                        {
                            if (mapping.Connection.Weight > 0 &&
                                mapping.Connection.ConnectionString == commonConnectionStr)
                            {
                                contains = true;
                                break;
                            }
                        }

                        if (!contains)
                            throw new InvalidOperationException(Resources.SqlBatch_AddCommand_NoCommonConnections);
                        continue;
                    }

                    commonConnections.IntersectWith(
                        command.Program.Mappings
                            .Where(m => m.Connection.Weight > 0)
                            .Select(m => m.Connection.ConnectionString));
                }

                if (commonConnections.Count < 1)
                    throw new InvalidOperationException(Resources.SqlBatch_AddCommand_NoCommonConnections);

                if (commonConnections.Count == 1)
                    commonConnectionStr = commonConnections.Single();
            }
            Debug.Assert(commonConnections != null, "commonConnections != null");
            return commonConnections;
        }

        /// <summary>
        /// Used to aggregate weights.
        /// </summary>
        private class WeightCounter
        {
            /// <summary>
            /// The total count, equal to the sum of the values of <see cref="_weightedCounts"/>.
            /// </summary>
            private int _totalCount;

            /// <summary>
            /// The the count for each weight.
            /// </summary>
            [NotNull]
            private readonly Dictionary<double, int> _weightedCounts = new Dictionary<double, int>();

            /// <summary>
            /// Increments the count for the specified weight.
            /// </summary>
            /// <param name="weight">The weight.</param>
            public void Increment(double weight)
            {
                _totalCount++;
                _weightedCounts.TryGetValue(weight, out var count);
                _weightedCounts[weight] = count + 1;
            }

            /// <summary>
            /// Gets the aggregated weight.
            /// </summary>
            /// <returns></returns>
            public double GetWeight()
            {
                if (_weightedCounts.Count == 1)
                {
                    Dictionary<double, int>.KeyCollection.Enumerator enumerator = _weightedCounts.Keys.GetEnumerator();
                    enumerator.MoveNext();
                    return enumerator.Current;
                }

                double totCount = _totalCount;
                double weight = 0;
                bool first = true;
                foreach (KeyValuePair<double, int> kvp in _weightedCounts)
                {
                    double currWeight = Math.Pow(kvp.Key, kvp.Value / totCount);

                    if (first)
                    {
                        first = false;
                        weight = currWeight;
                    }
                    else
                    {
                        weight *= currWeight;
                    }
                }
                return weight;
            }
        }
    }
}