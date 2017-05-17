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
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Database.Schema;

namespace WebApplications.Utilities.Database
{
    /// <summary>
    /// A command for controlling a batch execution of a <see cref="SqlProgram" />.
    /// </summary>
    public abstract class SqlBatchCommand : IBatchItem
    {
        /// <summary>
        /// Gets the batch that this command belongs to.
        /// </summary>
        /// <value>The batch.</value>
        [NotNull]
        public SqlBatch Batch { get; }

        /// <summary>
        /// Gets the program being batched.
        /// </summary>
        /// <value>The program.</value>
        [NotNull]
        public SqlProgram Program { get; }

        /// <summary>
        /// The set parameters delegate.
        /// </summary>
        [CanBeNull]
        private readonly SetBatchParametersDelegate _setParameters;

        /// <summary>
        /// Gets the result object.
        /// </summary>
        /// <value>The result.</value>
        [NotNull]
        public SqlBatchResult Result { get; }

        /// <summary>
        /// The behavior of the command.
        /// </summary>
        internal readonly CommandBehavior CommandBehavior;

        /// <summary>
        /// Whether errors should be suppressed.
        /// </summary>
        private readonly bool _suppressErrors;

        /// <summary>
        /// The exception handler.
        /// </summary>
        private readonly ExceptionHandler<DbException> _exceptionHandler;

        /// <summary>
        /// The index of the command in the root batch.
        /// </summary>
        internal int Index;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlBatchCommand" /> class.
        /// </summary>
        /// <param name="batch">The batch that this command belongs to.</param>
        /// <param name="program">The program to execute.</param>
        /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
        /// <param name="commandBehavior">The preferred command behavior.</param>
        /// <param name="suppressErrors"></param>
        /// <param name="exceptionHandler"></param>
        /// <param name="result">The result object.</param>
        internal SqlBatchCommand(
            [NotNull] SqlBatch batch,
            [NotNull] SqlProgram program,
            [CanBeNull] SetBatchParametersDelegate setParameters,
            CommandBehavior commandBehavior,
            bool suppressErrors,
            [CanBeNull] ExceptionHandler<DbException> exceptionHandler,
            [NotNull] SqlBatchResult result)
        {
            Batch = batch;
            Program = program;
            _setParameters = setParameters;
            if ((commandBehavior & CommandBehavior.SingleRow) == CommandBehavior.SingleRow)
                commandBehavior |= CommandBehavior.SingleResult;
            CommandBehavior = commandBehavior;
            _suppressErrors = suppressErrors;
            _exceptionHandler = exceptionHandler;
            Result = result;
            result.Command = this;
        }

        /// <summary>
        /// Gets the transaction for this item.
        /// </summary>
        TransactionType IBatchItem.Transaction => TransactionType.None;

        /// <summary>
        /// Gets the isolation level of the transaction for this item.
        /// </summary>
        IsolationLevel IBatchItem.IsolationLevel => IsolationLevel.Unspecified;

        /// <summary>
        /// Gets the name of the transaction, if there is one.
        /// </summary>
        string IBatchItem.TransactionName => null;

        /// <summary>
        /// Gets a value indicating whether errors are suppressed in the batch for this item.
        /// </summary>
        /// <value>
        ///   <see langword="true" /> if errors should be suppressed; otherwise, <see langword="false" />.
        /// </value>
        bool IBatchItem.SuppressErrors => _suppressErrors;

        /// <summary>
        /// Processes the command to be executed.
        /// </summary>
        /// <param name="args">The arguments.</param>
        void IBatchItem.Process(BatchProcessArgs args)
        {
            Index = args.CommandIndex;

            // Get the parameters for the command
            SqlBatchParametersCollection parameters = GetParametersForConnection(args.ConnectionString, args.CommandIndex);

            List<DbBatchParameter> dependentParams = null;

            // Add the parameters to the collection to pass to the command
            foreach (DbBatchParameter batchParameter in parameters.Parameters)
            {
                if (batchParameter.OutputValue != null)
                {
                    if (!args.OutParameters.TryGetValue(batchParameter.OutputValue, out DbParameter dbParameter))
                        throw new NotImplementedException("proper error");
                    batchParameter.BaseParameter = dbParameter;

                    if (dependentParams == null) dependentParams = new List<DbBatchParameter>();
                    dependentParams.Add(batchParameter);
                }
                else
                {
                    args.AllParameters.Add(batchParameter.BaseParameter);
                }
            }

            // Add any output parameters to the dictionary for passing into following commands
            if (parameters.OutputParameters != null)
            {
                foreach ((DbBatchParameter batchParameter, IOut outValue) in parameters.OutputParameters)
                {
                    if (args.OutParameters.ContainsKey(outValue))
                        throw new NotImplementedException("proper error");

                    args.OutParameters.Add(outValue, batchParameter.BaseParameter);
                    args.OutParameterCommands.Add(outValue, this);
                }

                args.CommandOutParams.Add(this, parameters.OutputParameters);
            }

            // The mask the behavior with this commands behavior
            args.Behavior &= CommandBehavior;

            // Build command SQL
            args.SqlBuilder
                .Append("-- ")
                .AppendLine(Program.Name);

            this.BeginTry(args, out int startIndex);

            // Used in CATCH statements to know which command failed
            args.SqlBuilder
                .Append("SET @CmdIndex = ")
                .Append(args.CommandIndex)
                .AppendLine(";");
            
            // If any of the parameters come from output parameters of previous commands, 
            // we need to make sure the commands executed successfully
            if (dependentParams != null)
            {
                foreach (IGrouping<SqlBatchCommand, DbBatchParameter> cmd in dependentParams.GroupBy(
                    p => args.OutParameterCommands[p.OutputValue]))
                {
                    string[] paramNames = cmd.Select(p => p.ParameterName).ToArray();
                    // TODO get messages from Resources?
                    string message = paramNames.Length == 1
                        ? $"The value of the {paramNames[0]} parameter depends on the output of a previous command which has not been executed successfully."
                        : $"The value of the parameters {string.Join(",", paramNames)} depend on the output of a previous command which has not been executed successfully.";

                    args.SqlBuilder
                        .Append("IF (ISNULL(@Cmd")
                        .Append(cmd.Key.Index)
                        .AppendLine("Success,0) <> 1)")
                        .Append("\tRAISERROR(")
                        .AppendNVarChar(message)
                        .AppendLine(",16,0);");
                }
                args.SqlBuilder.AppendLine();
            }

            // Indicate the start of the command. The select is used to wait for the 
            // command to be ready before actually executing the programs SQL.
            SqlBatch.AppendInfo(args, Constants.ExecuteState.Start)
                .AppendLine("SELECT NULL;");

            // Output the sql for actually executing the commands program
            AppendExecuteSql(args.SqlBuilder, parameters);

            if (parameters.OutputParameters != null)
            {
                // Sets a flag indicating that this command executed successfully,
                // so any command using the output parameters can check
                // HAVE_TODO If this command is inside a transaction, this flag needs to be set to 0 if the transaction is rolled back
                args.SqlBuilder
                    .Append("DECLARE @Cmd")
                    .Append(args.CommandIndex)
                    .Append("Success bit")
                    .AppendLine(
                        args.ServerVersion.Major > 9
                            ? " = 1;"
                            : $"; SET @Cmd{args.CommandIndex}Success = 1;");

                SqlBatch
                    .AppendInfo(args, Constants.ExecuteState.Output)
                    .Append("SELECT ");

                bool firstParam = true;
                foreach ((DbBatchParameter batchParameter, _) in parameters.OutputParameters)
                {
                    if (firstParam) firstParam = false;
                    else args.SqlBuilder.Append(", ");

                    args.SqlBuilder.Append(batchParameter.BaseParameter.ParameterName);
                }
                args.SqlBuilder.AppendLine(";");
            }

            this.EndTry(args, startIndex);

            SqlBatch.AppendInfo(args, Constants.ExecuteState.End).AppendLine();

            SqlProgramMapping mapping = parameters.Mapping;
            SqlProgram program = Program;
            LoadBalancedConnection loadBalancedConnection = program.Connection;
            Connection connection = mapping.Connection;

            // Need to wait on the semaphores for all the connections and databases
            if (connection.Semaphore != null)
                args.ConnectionSemaphores.Add(connection.Semaphore);
            if (loadBalancedConnection.ConnectionSemaphore != null)
                args.LoadBalConnectionSemaphores.Add(loadBalancedConnection.ConnectionSemaphore);
            if (loadBalancedConnection.DatabaseSemaphore != null)
                args.DatabaseSemaphores.Add(loadBalancedConnection.DatabaseSemaphore);

            args.CommandIndex++;
        }

        /// <summary>
        /// Gets the parameters for the connection with the connection string given.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="commandIndex">Index of the command in the batch.</param>
        /// <returns>The collection of parameters.</returns>
        [NotNull]
        internal SqlBatchParametersCollection GetParametersForConnection(
            [NotNull] string connectionString,
            ushort commandIndex)
        {
            SqlProgramMapping mapping = Program.Mappings.Single(m => m.Connection.ConnectionString == connectionString);
            Debug.Assert(mapping != null, "mapping != null");

            SqlBatchParametersCollection parameters = new SqlBatchParametersCollection(mapping, this, commandIndex);

            _setParameters?.Invoke(parameters);

            return parameters;
        }

        /// <summary>
        /// Appends the SQL for executing the command to the <paramref name="builder" /> provided.
        /// </summary>
        /// <param name="builder">The <see cref="SqlStringBuilder" /> to append the SQL to.</param>
        /// <param name="parameters">The parameters to execute with.</param>
        public virtual void AppendExecuteSql(
            [NotNull] SqlStringBuilder builder,
            [NotNull] SqlBatchParametersCollection parameters)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));

            SqlProgramDefinition def = parameters.Mapping.Definition;

            builder.Append("EXECUTE ");

            // If there is a return value parameter, need to assign the result to it
            if (parameters.ReturnValueParameter != null)
            {
                builder
                    .Append(parameters.ReturnValueParameter.BaseParameter.ParameterName)
                    .Append(" = ");
            }

            builder
                .AppendIdentifier(def.SqlSchema.FullName)
                .Append('.')
                .AppendIdentifier(def.Name);

            bool first = true;
            foreach (DbBatchParameter parameter in parameters.Parameters)
            {
                // Already dealt with return value parameter
                if (parameter.Direction == ParameterDirection.ReturnValue)
                    continue;

                if (first) first = false;
                else builder.Append(',');

                builder
                    .AppendLine()
                    .Append('\t')
                    .Append(parameter.ProgramParameter.FullName)
                    .Append(" = ")
                    .Append(parameter.BaseParameter.ParameterName);

                // If the parameter value is Out<T>, need to add OUT to actually get the return value
                if (parameter.IsOutputUsed)
                    builder.Append(" OUT");
            }

            builder.AppendLine(";");
        }

        /// <summary>
        /// Starts the reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        protected async Task StartReader(DbBatchDataReader reader, CancellationToken cancellationToken)
        {
            if (!await reader.NextResultAsync(cancellationToken).ConfigureAwait(false))
                throw new InvalidDataException("Missing start record set.");
        }

        /// <summary>
        /// Handles the command asynchronously.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="dbCommand"></param>
        /// <param name="connectionIndex">Index of the connection this command is being run on.</param>
        /// <param name="cancellationToken">A cancellation token which can be used to cancel the entire batch operation.</param>
        /// <returns>
        /// An awaitable task.
        /// </returns>
        [NotNull]
        internal abstract Task HandleCommandAsync(
            [NotNull] DbBatchDataReader reader,
            DbCommand dbCommand,
            int connectionIndex,
            CancellationToken cancellationToken);

        /// <summary>
        /// Command for calling ExecuteScalar on a program in a batch.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <seealso cref="WebApplications.Utilities.Database.SqlBatchCommand" />
        internal class Scalar<TResult> : SqlBatchCommand
        {
            /// <summary>
            /// Gets the result object.
            /// </summary>
            /// <value>The result.</value>
            [NotNull]
            public new SqlBatchResult<TResult> Result => (SqlBatchResult<TResult>)base.Result;

            /// <summary>
            /// Initializes a new instance of the <see cref="SqlBatchCommand.Scalar{TResult}" /> class.
            /// </summary>
            /// <param name="batch">The batch that this command belongs to.</param>
            /// <param name="program">The program to execute scalar.</param>
            /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
            /// <param name="exceptionHandler"></param>
            /// <param name="suppressErrors"></param>
            public Scalar(
                [NotNull] SqlBatch batch,
                [NotNull] SqlProgram program,
                SetBatchParametersDelegate setParameters,
                ExceptionHandler<DbException> exceptionHandler,
                bool suppressErrors)
                : base(
                    batch,
                    program,
                    setParameters,
                    CommandBehavior.SequentialAccess,
                    suppressErrors,
                    exceptionHandler,
                    new SqlBatchResult<TResult>())
            {
            }

            /// <summary>
            /// Handles the command asynchronously.
            /// </summary>
            /// <param name="reader">The reader.</param>
            /// <param name="dbCommand"></param>
            /// <param name="connectionIndex">Index of the connection this command is being run on.</param>
            /// <param name="cancellationToken">A cancellation token which can be used to cancel the entire batch operation.</param>
            /// <returns>
            /// An awaitable task.
            /// </returns>
            internal override async Task HandleCommandAsync(
                DbBatchDataReader reader,
                DbCommand dbCommand,
                int connectionIndex,
                CancellationToken cancellationToken)
            {
                await StartReader(reader, cancellationToken).ConfigureAwait(false);
                if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    object value = (reader.FieldCount < 1 || reader.IsDBNull(0)) ? null : reader.GetValue(0);
                    Result.SetResult(connectionIndex, (TResult)value);
                }
            }
        }

        /// <summary>
        /// Command for calling ExecuteNonQuery on a program in a batch.
        /// </summary>
        /// <seealso cref="WebApplications.Utilities.Database.SqlBatchCommand" />
        internal class NonQuery : SqlBatchCommand
        {
            /// <summary>
            /// The SqlCommand._rowsAffected field setter.
            /// </summary>
            private static readonly Action<SqlCommand, int> _setRecordsAffected;

            /// <summary>
            /// Initializes the <see cref="SqlBatchCommand.NonQuery" /> class.
            /// </summary>
            static NonQuery()
            {
                FieldInfo fieldInfo = typeof(SqlCommand).GetField(
                    "_rowsAffected",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                if (fieldInfo == null)
                    return;

                ParameterExpression thisParam = Expression.Parameter(typeof(SqlCommand));
                ParameterExpression valueParam = Expression.Parameter(typeof(int));
                _setRecordsAffected = Expression.Lambda<Action<SqlCommand, int>>(
                    Expression.Assign(Expression.Field(thisParam, fieldInfo), valueParam),
                    thisParam,
                    valueParam).Compile();
            }

            /// <summary>
            /// Gets the result object.
            /// </summary>
            /// <value>The result.</value>
            [NotNull]
            public new SqlBatchResult<int> Result => (SqlBatchResult<int>)base.Result;

            /// <summary>
            /// Initializes a new instance of the <see cref="SqlBatchCommand.NonQuery" /> class.
            /// </summary>
            /// <param name="batch">The batch that this command belongs to.</param>
            /// <param name="program">The program to execute scalar.</param>
            /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
            /// <param name="exceptionHandler"></param>
            /// <param name="suppressErrors"></param>
            public NonQuery(
                [NotNull] SqlBatch batch,
                [NotNull] SqlProgram program,
                SetBatchParametersDelegate setParameters,
                ExceptionHandler<DbException> exceptionHandler,
                bool suppressErrors)
                : base(
                    batch,
                    program,
                    setParameters,
                    CommandBehavior.SequentialAccess,
                    suppressErrors,
                    exceptionHandler,
                    new SqlBatchResult<int>())
            {
            }

            /// <summary>
            /// Handles the command asynchronously.
            /// </summary>
            /// <param name="reader">The reader.</param>
            /// <param name="dbCommand"></param>
            /// <param name="connectionIndex">Index of the connection this command is being run on.</param>
            /// <param name="cancellationToken">A cancellation token which can be used to cancel the entire batch operation.</param>
            /// <returns>
            /// An awaitable task.
            /// </returns>
            internal override async Task HandleCommandAsync(
                DbBatchDataReader reader,
                DbCommand dbCommand,
                int connectionIndex,
                CancellationToken cancellationToken)
            {
                Debug.Assert(dbCommand is SqlCommand);

                SqlCommand sqlCommand = (SqlCommand)dbCommand;

                int initialCount = 0;

                // If this isn't the first command
                if (Index > 0)
                {
                    // If we can set the records affected field, reset it to -1
                    if (_setRecordsAffected != null)
                        _setRecordsAffected(sqlCommand, -1);
                    else
                    {
                        // Otherwise, get the current number of records affected
                        initialCount = reader.RecordsAffected;
                        if (initialCount < 0) initialCount = 0;
                    }
                }

                // Read all the rows
                await StartReader(reader, cancellationToken).ConfigureAwait(false);
                do
                {
                    while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                    {
                    }
                } while (await reader.NextResultAsync(cancellationToken).ConfigureAwait(false));

                // If this is the first command, or we have a _setRecordsAffected method, we can just return the property directly
                // Otherwise we need to subtract the initial count from the current count
                int count = Index == 0 || _setRecordsAffected != null
                    ? reader.RecordsAffected
                    : reader.RecordsAffected - initialCount;

                Result.SetResult(connectionIndex, count);
            }
        }

        /// <summary>
        /// Base class for calling ExecuteReader on a program in a batch.
        /// </summary>
        /// <seealso cref="WebApplications.Utilities.Database.SqlBatchCommand" />
        internal abstract class BaseReader : SqlBatchCommand
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SqlBatchCommand.BaseReader" /> class.
            /// </summary>
            /// <param name="batch">The batch that this command belongs to.</param>
            /// <param name="program">The program to execute.</param>
            /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
            /// <param name="commandBehavior">The behavior.</param>
            /// <param name="exceptionHandler"></param>
            /// <param name="suppressErrors"></param>
            /// <param name="result">The result.</param>
            protected BaseReader(
                [NotNull] SqlBatch batch,
                [NotNull] SqlProgram program,
                [CanBeNull] SetBatchParametersDelegate setParameters,
                CommandBehavior commandBehavior,
                ExceptionHandler<DbException> exceptionHandler,
                bool suppressErrors,
                [NotNull] SqlBatchResult result)
                : base(batch, program, setParameters, commandBehavior, suppressErrors, exceptionHandler, result)
            {
            }

            /// <summary>
            /// Appends the SQL for executing the command to the <paramref name="builder" /> provided.
            /// </summary>
            /// <param name="builder">The <see cref="SqlStringBuilder" /> to append the SQL to.</param>
            /// <param name="parameters">The parameters to execute with.</param>
            public override void AppendExecuteSql(SqlStringBuilder builder, SqlBatchParametersCollection parameters)
            {
                // ReSharper disable StringLiteralTypo
                if ((CommandBehavior & CommandBehavior.KeyInfo) == CommandBehavior.KeyInfo)
                    builder.AppendLine("SET NO_BROWSETABLE ON;");
                if ((CommandBehavior & CommandBehavior.SchemaOnly) == CommandBehavior.SchemaOnly)
                    builder.AppendLine("SET FMTONLY ON;");

                base.AppendExecuteSql(builder, parameters);

                if ((CommandBehavior & CommandBehavior.SchemaOnly) == CommandBehavior.SchemaOnly)
                    builder.AppendLine("SET FMTONLY OFF;");
                if ((CommandBehavior & CommandBehavior.KeyInfo) == CommandBehavior.KeyInfo)
                    builder.AppendLine("SET NO_BROWSETABLE OFF;");
                // ReSharper restore StringLiteralTypo
            }

            /// <summary>
            /// Handles the command asynchronously.
            /// </summary>
            /// <param name="reader">The reader.</param>
            /// <param name="dbCommand"></param>
            /// <param name="connectionIndex">Index of the connection this command is being run on.</param>
            /// <param name="cancellationToken">A cancellation token which can be used to cancel the entire batch operation.</param>
            /// <returns>
            /// An awaitable task.
            /// </returns>
            internal override async Task HandleCommandAsync(
                DbBatchDataReader reader,
                DbCommand dbCommand,
                int connectionIndex,
                CancellationToken cancellationToken)
            {
                await StartReader(reader, cancellationToken).ConfigureAwait(false);
                Task task = ExecuteResultDelegate(reader, cancellationToken);
                await task.ConfigureAwait(false);
                SetResult(task, connectionIndex);
            }

            /// <summary>
            /// Executes the result delegate.
            /// </summary>
            /// <param name="reader">The reader.</param>
            /// <param name="cancellationToken">A cancellation token which can be used to cancel the entire batch operation.</param>
            /// <returns>An awaitable task.</returns>
            [NotNull]
            protected abstract Task ExecuteResultDelegate(
                [NotNull] DbDataReader reader,
                CancellationToken cancellationToken);

            /// <summary>
            /// Sets the result of the command from the <see cref="Task"/> returned by <see cref="ExecuteResultDelegate"/>.
            /// </summary>
            /// <param name="task">The completed task returned by <see cref="ExecuteResultDelegate"/>.</param>
            /// <param name="index">The index.</param>
            protected abstract void SetResult([NotNull] Task task, int index);
        }

        /// <summary>
        /// Command for calling ExecuteReader on a program in a batch.
        /// </summary>
        /// <seealso cref="WebApplications.Utilities.Database.SqlBatchCommand" />
        internal class Reader : BaseReader
        {
            [NotNull]
            private readonly ResultDelegateAsync _resultAction;

            /// <summary>
            /// Initializes a new instance of the <see cref="SqlBatchCommand.Reader" /> class.
            /// </summary>
            /// <param name="batch">The batch that this command belongs to.</param>
            /// <param name="program">The program to execute scalar.</param>
            /// <param name="resultAction">The result function.</param>
            /// <param name="commandBehavior">The behavior.</param>
            /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
            /// <param name="exceptionHandler"></param>
            /// <param name="suppressErrors"></param>
            public Reader(
                [NotNull] SqlBatch batch,
                [NotNull] SqlProgram program,
                [NotNull] ResultDelegateAsync resultAction,
                CommandBehavior commandBehavior,
                [CanBeNull] SetBatchParametersDelegate setParameters,
                ExceptionHandler<DbException> exceptionHandler,
                bool suppressErrors)
                : base(
                    batch,
                    program,
                    setParameters,
                    commandBehavior,
                    exceptionHandler,
                    suppressErrors,
                    new SqlBatchResult<bool>())
            {
                _resultAction = resultAction;
            }

            /// <summary>
            /// Executes the result delegate.
            /// </summary>
            /// <param name="reader">The reader.</param>
            /// <param name="cancellationToken">A cancellation token which can be used to cancel the entire batch operation.</param>
            /// <returns>
            /// An awaitable task.
            /// </returns>
            protected override Task ExecuteResultDelegate(DbDataReader reader, CancellationToken cancellationToken)
            {
                return _resultAction(reader, cancellationToken);
            }

            /// <summary>
            /// Sets the result of the command from the <see cref="Task" /> returned by <see cref="ExecuteResultDelegate" />.
            /// </summary>
            /// <param name="task">The completed task returned by <see cref="ExecuteResultDelegate" />.</param>
            /// <param name="index">The index.</param>
            protected override void SetResult(Task task, int index)
            {
            }
        }

        /// <summary>
        /// Command for calling ExecuteReader on a program in a batch and returning a value.
        /// </summary>
        /// <seealso cref="WebApplications.Utilities.Database.SqlBatchCommand" />
        internal class Reader<TResult> : BaseReader
        {
            [NotNull]
            private readonly ResultDelegateAsync<TResult> _resultFunc;

            /// <summary>
            /// Gets the result object.
            /// </summary>
            /// <value>The result.</value>
            [NotNull]
            public new SqlBatchResult<TResult> Result => (SqlBatchResult<TResult>)base.Result;

            /// <summary>
            /// Initializes a new instance of the <see cref="SqlBatchCommand.Reader{TResult}" /> class.
            /// </summary>
            /// <param name="batch">The batch that this command belongs to.</param>
            /// <param name="program">The program to execute scalar.</param>
            /// <param name="resultFunc">The result function.</param>
            /// <param name="commandBehavior">The behavior.</param>
            /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
            /// <param name="exceptionHandler"></param>
            /// <param name="suppressErrors"></param>
            public Reader(
                [NotNull] SqlBatch batch,
                [NotNull] SqlProgram program,
                [NotNull] ResultDelegateAsync<TResult> resultFunc,
                CommandBehavior commandBehavior,
                [CanBeNull] SetBatchParametersDelegate setParameters,
                ExceptionHandler<DbException> exceptionHandler,
                bool suppressErrors)
                : base(
                    batch,
                    program,
                    setParameters,
                    commandBehavior,
                    exceptionHandler,
                    suppressErrors,
                    new SqlBatchResult<TResult>())
            {
                _resultFunc = resultFunc;
            }

            /// <summary>
            /// Executes the result delegate.
            /// </summary>
            /// <param name="reader">The reader.</param>
            /// <param name="cancellationToken">A cancellation token which can be used to cancel the entire batch operation.</param>
            /// <returns>
            /// An awaitable task.
            /// </returns>
            protected override Task ExecuteResultDelegate(DbDataReader reader, CancellationToken cancellationToken)
            {
                return _resultFunc(reader, cancellationToken);
            }

            /// <summary>
            /// Sets the result of the command from the <see cref="Task" /> returned by <see cref="ExecuteResultDelegate" />.
            /// </summary>
            /// <param name="task">The completed task returned by <see cref="ExecuteResultDelegate" />.</param>
            /// <param name="index">The index.</param>
            protected override void SetResult(Task task, int index)
            {
                Result.SetResult(index, ((Task<TResult>)task).Result);
            }
        }

        /// <summary>
        /// Base class for calling ExecuteXmlReader on a program in a batch.
        /// </summary>
        /// <seealso cref="WebApplications.Utilities.Database.SqlBatchCommand" />
        internal abstract class BaseXmlReader : SqlBatchCommand
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SqlBatchCommand.BaseXmlReader" /> class.
            /// </summary>
            /// <param name="batch">The batch that this command belongs to.</param>
            /// <param name="program">The program to execute.</param>
            /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
            /// <param name="exceptionHandler"></param>
            /// <param name="suppressErrors"></param>
            /// <param name="result">The result.</param>
            protected BaseXmlReader(
                [NotNull] SqlBatch batch,
                [NotNull] SqlProgram program,
                [CanBeNull] SetBatchParametersDelegate setParameters,
                ExceptionHandler<DbException> exceptionHandler,
                bool suppressErrors,
                [NotNull] SqlBatchResult result)
                : base(
                    batch,
                    program,
                    setParameters,
                    CommandBehavior.SequentialAccess,
                    suppressErrors,
                    exceptionHandler,
                    result)
            {
            }

            /// <summary>
            /// Handles the command asynchronously.
            /// </summary>
            /// <param name="reader">The reader.</param>
            /// <param name="dbCommand"></param>
            /// <param name="connectionIndex">Index of the connection this command is being run on.</param>
            /// <param name="cancellationToken">A cancellation token which can be used to cancel the entire batch operation.</param>
            /// <returns>
            /// An awaitable task.
            /// </returns>
            internal override async Task HandleCommandAsync(
                DbBatchDataReader reader,
                DbCommand dbCommand,
                int connectionIndex,
                CancellationToken cancellationToken)
            {
                await StartReader(reader, cancellationToken).ConfigureAwait(false);
                Task task = ExecuteResultDelegate(reader.GetXmlReader(), cancellationToken);
                await task.ConfigureAwait(false);
                SetResult(task, connectionIndex);
            }

            /// <summary>
            /// Executes the result delegate.
            /// </summary>
            /// <param name="reader">The reader.</param>
            /// <param name="cancellationToken">A cancellation token which can be used to cancel the entire batch operation.</param>
            /// <returns>An awaitable task.</returns>
            [NotNull]
            protected abstract Task ExecuteResultDelegate(
                [NotNull] System.Xml.XmlReader reader,
                CancellationToken cancellationToken);

            /// <summary>
            /// Sets the result of the command from the <see cref="Task"/> returned by <see cref="ExecuteResultDelegate"/>.
            /// </summary>
            /// <param name="task">The completed task returned by <see cref="ExecuteResultDelegate"/>.</param>
            /// <param name="index">The index.</param>
            protected abstract void SetResult([NotNull] Task task, int index);
        }

        /// <summary>
        /// Command for calling ExecuteXmlReader on a program in a batch.
        /// </summary>
        /// <seealso cref="WebApplications.Utilities.Database.SqlBatchCommand" />
        internal class XmlReader : BaseXmlReader
        {
            [NotNull]
            private readonly XmlResultDelegateAsync _resultAction;

            /// <summary>
            /// Initializes a new instance of the <see cref="SqlBatchCommand.XmlReader" /> class.
            /// </summary>
            /// <param name="batch">The batch that this command belongs to.</param>
            /// <param name="program">The program to execute scalar.</param>
            /// <param name="resultAction">The result function.</param>
            /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
            /// <param name="exceptionHandler"></param>
            /// <param name="suppressErrors"></param>
            public XmlReader(
                [NotNull] SqlBatch batch,
                [NotNull] SqlProgram program,
                [NotNull] XmlResultDelegateAsync resultAction,
                [CanBeNull] SetBatchParametersDelegate setParameters,
                ExceptionHandler<DbException> exceptionHandler,
                bool suppressErrors)
                : base(batch, program, setParameters, exceptionHandler, suppressErrors, new SqlBatchResult<bool>())
            {
                _resultAction = resultAction;
            }

            /// <summary>
            /// Executes the result delegate.
            /// </summary>
            /// <param name="reader">The reader.</param>
            /// <param name="cancellationToken">A cancellation token which can be used to cancel the entire batch operation.</param>
            /// <returns>
            /// An awaitable task.
            /// </returns>
            protected override Task ExecuteResultDelegate(
                System.Xml.XmlReader reader,
                CancellationToken cancellationToken)
            {
                return _resultAction(reader, cancellationToken);
            }

            /// <summary>
            /// Sets the result of the command from the <see cref="Task" /> returned by <see cref="ExecuteResultDelegate" />.
            /// </summary>
            /// <param name="task">The completed task returned by <see cref="ExecuteResultDelegate" />.</param>
            /// <param name="index">The index.</param>
            protected override void SetResult(Task task, int index)
            {
            }
        }

        /// <summary>
        /// Command for calling ExecuteXmlReader on a program in a batch and returning a value.
        /// </summary>
        /// <seealso cref="WebApplications.Utilities.Database.SqlBatchCommand" />
        internal class XmlReader<TResult> : BaseXmlReader
        {
            [NotNull]
            private readonly XmlResultDelegateAsync<TResult> _resultFunc;

            /// <summary>
            /// Gets the result object.
            /// </summary>
            /// <value>The result.</value>
            [NotNull]
            public new SqlBatchResult<TResult> Result => (SqlBatchResult<TResult>)base.Result;

            /// <summary>
            /// Initializes a new instance of the <see cref="SqlBatchCommand.XmlReader{TResult}" /> class.
            /// </summary>
            /// <param name="batch">The batch that this command belongs to.</param>
            /// <param name="program">The program to execute scalar.</param>
            /// <param name="resultFunc">The result function.</param>
            /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
            /// <param name="exceptionHandler"></param>
            /// <param name="suppressErrors"></param>
            public XmlReader(
                [NotNull] SqlBatch batch,
                [NotNull] SqlProgram program,
                [NotNull] XmlResultDelegateAsync<TResult> resultFunc,
                [CanBeNull] SetBatchParametersDelegate setParameters,
                ExceptionHandler<DbException> exceptionHandler,
                bool suppressErrors)
                : base(batch, program, setParameters, exceptionHandler, suppressErrors, new SqlBatchResult<TResult>())
            {
                _resultFunc = resultFunc;
            }

            /// <summary>
            /// Executes the result delegate.
            /// </summary>
            /// <param name="reader">The reader.</param>
            /// <param name="cancellationToken">A cancellation token which can be used to cancel the entire batch operation.</param>
            /// <returns>
            /// An awaitable task.
            /// </returns>
            protected override Task ExecuteResultDelegate(
                System.Xml.XmlReader reader,
                CancellationToken cancellationToken)
            {
                return _resultFunc(reader, cancellationToken);
            }

            /// <summary>
            /// Sets the result of the command from the <see cref="Task" /> returned by <see cref="ExecuteResultDelegate" />.
            /// </summary>
            /// <param name="task">The completed task returned by <see cref="ExecuteResultDelegate" />.</param>
            /// <param name="index">The index.</param>
            protected override void SetResult(Task task, int index)
            {
                Result.SetResult(index, ((Task<TResult>)task).Result);
            }
        }
    }
}