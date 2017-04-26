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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NodaTime;
using WebApplications.Utilities;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Database.Schema;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlBatch"/> class.
        /// </summary>
        /// <param name="batchTimeout">The batch timeout.</param>
        public SqlBatch(Duration batchTimeout)
        {
            BatchTimeout = batchTimeout;
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

        // TODO Transaction support
        // TODO ResultDelegate needs to take a DbDataReader instead
        // TODO Not all CommandBehaviors may be possible to do

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
                if (_commands.Count >= ushort.MaxValue)
                    throw new InvalidOperationException("Only allowed 65536 commands per batch.");

                command.Id = (ushort)_commands.Count;
                _commands.Add(command);
            }
        }

        /// <summary>
        /// Adds the specified program to the batch. 
        /// The first column of the first row in the result set returned by the query will be returned from the <see cref="SqlBatchResult{T}"/>.
        /// Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="TOut">The output type expected.</typeparam>
        /// <param name="program">The program to add to the batch.</param>
        /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
        /// <returns>A <see cref="SqlBatchResult{T}"/> which can be used to get the scalar value returned by the program.</returns>
        public SqlBatchResult<TOut> AddExecuteScalar<TOut>(
            [NotNull] SqlProgram program,
            [CanBeNull] SetBatchParametersDelegate setParameters = null)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));
            CheckState();

            SqlBatchCommand.Scalar<TOut> command = new SqlBatchCommand.Scalar<TOut>(this, program, setParameters);
            AddCommand(command);
            return command.Result;
        }

        /// <summary>
        /// Adds the specified program to the batch. 
        /// The number of records affected by the program will be returned from the <see cref="SqlBatchResult{T}"/>.
        /// </summary>
        /// <param name="program">The program to add to the batch.</param>
        /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
        /// <returns>
        /// A <see cref="SqlBatchResult" /> which can be used to get the number of records affected by the program.
        /// </returns>
        public SqlBatchResult<int> AddExecuteNonQuery(
            [NotNull] SqlProgram program,
            [CanBeNull] SetBatchParametersDelegate setParameters = null)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));
            CheckState();

            SqlBatchCommand.NonQuery command = new SqlBatchCommand.NonQuery(this, program, setParameters);
            AddCommand(command);
            return command.Result;
        }

        /// <summary>
        /// Adds the specified program to the batch.
        /// </summary>
        /// <param name="program">The program to add to the batch.</param>
        /// <param name="resultAction">The action used to process the result.</param>
        /// <param name="behavior">The query's effect on the database.</param>
        /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
        /// <returns>
        /// A <see cref="SqlBatchResult" /> which can be used to wait for the program to finish executing.
        /// </returns>
        public SqlBatchResult AddExecuteReader(
            [NotNull] SqlProgram program,
            [NotNull] ResultDelegateAsync resultAction,
            CommandBehavior behavior = CommandBehavior.Default,
            [CanBeNull] SetBatchParametersDelegate setParameters = null)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));
            if (resultAction == null) throw new ArgumentNullException(nameof(resultAction));
            CheckState();

            SqlBatchCommand.Reader command = new SqlBatchCommand.Reader(
                this,
                program,
                resultAction,
                behavior,
                setParameters);
            AddCommand(command);
            return command.Result;
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
        /// <returns>
        /// A <see cref="SqlBatchResult" /> which can be used to get the value returned by the <paramref name="resultFunc"/>.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public SqlBatchResult<TOut> AddExecuteReader<TOut>(
            [NotNull] SqlProgram program,
            [NotNull] ResultDelegateAsync<TOut> resultFunc,
            CommandBehavior behavior = CommandBehavior.Default,
            [CanBeNull] SetBatchParametersDelegate setParameters = null)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));
            if (resultFunc == null) throw new ArgumentNullException(nameof(resultFunc));
            CheckState();

            SqlBatchCommand.Reader<TOut> command = new SqlBatchCommand.Reader<TOut>(
                this,
                program,
                resultFunc,
                behavior,
                setParameters);
            AddCommand(command);
            return command.Result;
        }

        // TODO Execute XML reader

        /// <summary>
        /// Executes the batch on a single connection, asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token which can be used to cancel the entire batch operation.</param>
        /// <returns>An awaitable task which completes when the batch is complete.</returns>
        public async Task ExecuteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_commands.Count < 1)
                throw new InvalidOperationException(Resources.SqlBatch_ExecuteAsync_Empty);

            int state = Interlocked.CompareExchange(ref _state, Executing, Building);
            if (state == Completed) return;

            using (await _executeLock.LockAsync(cancellationToken).ConfigureAwait(false))
            {
                if (_state == Completed) return;

                try
                {
                    string connectionString = DetermineConnection();

                    await ExecuteInternal(connectionString, cancellationToken).ConfigureAwait(false);
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
            if (_commands.Count < 1)
                throw new InvalidOperationException(Resources.SqlBatch_ExecuteAsync_Empty);

            int state = Interlocked.CompareExchange(ref _state, Executing, Building);
            if (state == Completed) return;

            using (await _executeLock.LockAsync(cancellationToken).ConfigureAwait(false))
            {
                if (_state == Completed) return;

                try
                {
                    // Get the connection strings which are common to each program
                    HashSet<string> commonConnections = GetCommonConnections();

                    Task[] tasks = commonConnections
                        .Select(cs => ExecuteInternal(cs, cancellationToken))
                        .ToArray();

                    await Task.WhenAll(tasks).ConfigureAwait(false);
                }
                finally
                {
                    _state = Completed;
                }
            }
        }

        [NotNull]
        private async Task ExecuteInternal(
            [NotNull] string connectionString,
            CancellationToken cancellationToken)
        {
            HashSet<AsyncSemaphore> connectionSemaphores = new HashSet<AsyncSemaphore>();
            HashSet<AsyncSemaphore> loadBalConnectionSemaphores = new HashSet<AsyncSemaphore>();
            HashSet<AsyncSemaphore> databaseSemaphores = new HashSet<AsyncSemaphore>();

            StringBuilder sqlBuilder = new StringBuilder();
            CommandBehavior allBehavior = CommandBehavior.Default;

            List<DbParameter> allParameters = new List<DbParameter>();

            Dictionary<IOut, DbParameter> outParameters = new Dictionary<IOut, DbParameter>();

            // Build the batch SQL from the commands
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
                foreach ((DbBatchParameter batchParameter, IOut outValue) in parameters.OutputParameters)
                {
                    if (outParameters.ContainsKey(outValue))
                        throw new NotImplementedException("proper error");

                    outParameters.Add(outValue, (SqlParameter)batchParameter.BaseParameter);
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

                // TODO Build SQL
            }

            // NOTE! Do NOT reorder these without also reordering the semaphores in SqlProgramCommand.WaitSemaphoresAsync
            AsyncSemaphore[] semaphores = connectionSemaphores
                .Concat(loadBalConnectionSemaphores)
                .Concat(databaseSemaphores)
                .ToArray();

            using (await AsyncSemaphore.WaitAllAsync(cancellationToken, semaphores).ConfigureAwait(false))
            using (DbConnection dbConnection = new SqlConnection(connectionString))
            {
                await dbConnection.OpenAsync(cancellationToken).ConfigureAwait(false);

                DbTransaction transaction = null;

                using (DbCommand dbCommand = CreateCommand(sqlBuilder.ToString(), dbConnection, allParameters.ToArray(), transaction))
                using (DbDataReader reader = await dbCommand.ExecuteReaderAsync(allBehavior, cancellationToken)
                    .ConfigureAwait(false))
                {

                }
            }
        }

        /// <summary>
        /// Creates a command for the batch.
        /// </summary>
        /// <param name="text">The text to execute.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="transaction">The transaction.</param>
        /// <returns></returns>
        [NotNull]
        private DbCommand CreateCommand(
            [NotNull] string text,
            [NotNull] DbConnection connection,
            [NotNull] DbParameter[] parameters,
            DbTransaction transaction)
        {
            DbCommand command = connection.CreateCommand();
            try
            {
                Debug.Assert(command.Connection == connection);

                command.CommandText = text;
                command.CommandType = CommandType.Text;
                command.CommandTimeout = (int)BatchTimeout.TotalSeconds();
                command.Transaction = transaction;
                command.Parameters.AddRange(parameters);

                // TODO if there are any NonQuery commands, need to use this
                // sqlCommand.StatementCompleted += ...

                return command;
            }
            catch
            {
                command.Dispose();
                throw;
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
            HashSet<string> commonConnections = GetCommonConnections();

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
                    // ReSharper disable once PossibleNullReferenceException
                    connWeightCounts
                        .GetOrAdd(mapping.Connection.ConnectionString, _ => new WeightCounter())
                        .Increment(mapping.Connection.Weight / totWeight);
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
        /// Type used as the return type of the wrapper resultFunc for <see cref="AddExecuteReader"/>.
        /// </summary>
        private struct VoidType
        {
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
                _weightedCounts.AddOrUpdate(weight, _ => 1, (_, c) => c + 1);
            }

            /// <summary>
            /// Gets the aggregated weight.
            /// </summary>
            /// <returns></returns>
            public double GetWeight()
            {
                if (_weightedCounts.Count == 1) return _weightedCounts.Keys.Single();

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