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
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Database.Schema;

namespace WebApplications.Utilities.Database
{
    /// <summary>
    /// A command for controlling a batch execution of a <see cref="SqlProgram" />.
    /// </summary>
    public abstract class SqlBatchCommand
    {
        /// <summary>
        /// Gets the command identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public ushort Id { get; internal set; }

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
        public SqlBatchResult Result { get; private set; }

        /// <summary>
        /// The behavior of the command.
        /// </summary>
        internal readonly CommandBehavior CommandBehavior;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlBatchCommand" /> class.
        /// </summary>
        /// <param name="batch">The batch that this command belongs to.</param>
        /// <param name="program">The program to execute.</param>
        /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
        /// <param name="commandBehavior">The preferred command behavior.</param>
        internal SqlBatchCommand(
            [NotNull] SqlBatch batch,
            [NotNull] SqlProgram program,
            [CanBeNull] SetBatchParametersDelegate setParameters,
            CommandBehavior commandBehavior)
        {
            Batch = batch;
            Program = program;
            _setParameters = setParameters;
            if ((commandBehavior & CommandBehavior.SingleRow) == CommandBehavior.SingleRow)
                commandBehavior |= CommandBehavior.SingleResult;
            CommandBehavior = commandBehavior;
        }

        /// <summary>
        /// Gets the parameters for the connection with the connection string given.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        [NotNull]
        internal SqlBatchParametersCollection GetParametersForConnection([NotNull] string connectionString)
        {
            SqlProgramMapping mapping = Program.Mappings.Single(m => m.Connection.ConnectionString == connectionString);
            Debug.Assert(mapping != null, "mapping != null");

            SqlBatchParametersCollection parameters = new SqlBatchParametersCollection(mapping, this);

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
            public new SqlBatchResult<TResult> Result
            {
                get => (SqlBatchResult<TResult>)base.Result;
                private set => base.Result = value;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="SqlBatchCommand.Scalar{TResult}" /> class.
            /// </summary>
            /// <param name="batch">The batch that this command belongs to.</param>
            /// <param name="program">The program to execute scalar.</param>
            /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
            public Scalar(
                [NotNull] SqlBatch batch,
                [NotNull] SqlProgram program,
                SetBatchParametersDelegate setParameters)
                : base(batch, program, setParameters, CommandBehavior.SequentialAccess)
            {
                Result = new SqlBatchResult<TResult>(this);
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
            /// Gets the result object.
            /// </summary>
            /// <value>The result.</value>
            [NotNull]
            public new SqlBatchResult<int> Result
            {
                get => (SqlBatchResult<int>)base.Result;
                private set => base.Result = value;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="SqlBatchCommand.NonQuery" /> class.
            /// </summary>
            /// <param name="batch">The batch that this command belongs to.</param>
            /// <param name="program">The program to execute scalar.</param>
            /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
            public NonQuery(
                [NotNull] SqlBatch batch,
                [NotNull] SqlProgram program,
                SetBatchParametersDelegate setParameters)
                : base(batch, program, setParameters, CommandBehavior.SequentialAccess)
            {
                Result = new SqlBatchResult<int>(this);
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

                int count = 0;

                StatementCompletedEventHandler handler = (_, args) => count += args.RecordCount;

                try
                {
                    sqlCommand.StatementCompleted += handler;

                    do
                    {
                        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                            count--;
                    } while (await reader.NextResultAsync(cancellationToken).ConfigureAwait(false));

                    Result.SetResult(connectionIndex, count);
                }
                finally
                {
                    sqlCommand.StatementCompleted -= handler;
                }
            }
        }

        internal abstract class BaseReader : SqlBatchCommand
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SqlBatchCommand.BaseReader" /> class.
            /// </summary>
            /// <param name="batch">The batch that this command belongs to.</param>
            /// <param name="program">The program to execute.</param>
            /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
            /// <param name="commandBehavior">The behavior.</param>
            protected BaseReader(
                [NotNull] SqlBatch batch,
                [NotNull] SqlProgram program,
                [CanBeNull] SetBatchParametersDelegate setParameters,
                CommandBehavior commandBehavior)
                : base(batch, program, setParameters, commandBehavior)
            {
            }

            /// <summary>
            /// Appends the SQL for executing the command to the <paramref name="builder" /> provided.
            /// </summary>
            /// <param name="builder">The <see cref="SqlStringBuilder" /> to append the SQL to.</param>
            /// <param name="parameters">The parameters to execute with.</param>
            public override void AppendExecuteSql(SqlStringBuilder builder, SqlBatchParametersCollection parameters)
            {
                if ((CommandBehavior & CommandBehavior.KeyInfo) == CommandBehavior.KeyInfo)
                    builder.AppendLine("SET NO_BROWSETABLE ON;");
                if ((CommandBehavior & CommandBehavior.SchemaOnly) == CommandBehavior.SchemaOnly)
                    builder.AppendLine("SET FMTONLY ON;");

                base.AppendExecuteSql(builder, parameters);

                if ((CommandBehavior & CommandBehavior.SchemaOnly) == CommandBehavior.SchemaOnly)
                    builder.AppendLine("SET FMTONLY OFF;");
                if ((CommandBehavior & CommandBehavior.KeyInfo) == CommandBehavior.KeyInfo)
                    builder.AppendLine("SET NO_BROWSETABLE OFF;");
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
            protected abstract Task ExecuteResultDelegate([NotNull] DbDataReader reader, CancellationToken cancellationToken);

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
            public Reader(
                [NotNull] SqlBatch batch,
                [NotNull] SqlProgram program,
                [NotNull] ResultDelegateAsync resultAction,
                CommandBehavior commandBehavior,
                [CanBeNull] SetBatchParametersDelegate setParameters)
                : base(batch, program, setParameters, commandBehavior)
            {
                _resultAction = resultAction;
                Result = new SqlBatchResult<bool>(this);
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
            public new SqlBatchResult<TResult> Result
            {
                get => (SqlBatchResult<TResult>)base.Result;
                private set => base.Result = value;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="SqlBatchCommand.Reader{TResult}" /> class.
            /// </summary>
            /// <param name="batch">The batch that this command belongs to.</param>
            /// <param name="program">The program to execute scalar.</param>
            /// <param name="resultFunc">The result function.</param>
            /// <param name="commandBehavior">The behavior.</param>
            /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
            public Reader(
                [NotNull] SqlBatch batch,
                [NotNull] SqlProgram program,
                [NotNull] ResultDelegateAsync<TResult> resultFunc,
                CommandBehavior commandBehavior,
                [CanBeNull] SetBatchParametersDelegate setParameters)
                : base(batch, program, setParameters, commandBehavior)
            {
                _resultFunc = resultFunc;
                Result = new SqlBatchResult<TResult>(this);
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

        internal abstract class BaseXmlReader : SqlBatchCommand
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SqlBatchCommand.BaseXmlReader" /> class.
            /// </summary>
            /// <param name="batch">The batch that this command belongs to.</param>
            /// <param name="program">The program to execute.</param>
            /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
            protected BaseXmlReader(
                [NotNull] SqlBatch batch,
                [NotNull] SqlProgram program,
                [CanBeNull] SetBatchParametersDelegate setParameters)
                : base(batch, program, setParameters, CommandBehavior.SequentialAccess)
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
            protected abstract Task ExecuteResultDelegate([NotNull] System.Xml.XmlReader reader, CancellationToken cancellationToken);

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
            public XmlReader(
                [NotNull] SqlBatch batch,
                [NotNull] SqlProgram program,
                [NotNull] XmlResultDelegateAsync resultAction,
                [CanBeNull] SetBatchParametersDelegate setParameters)
                : base(batch, program, setParameters)
            {
                _resultAction = resultAction;
                Result = new SqlBatchResult<bool>(this);
            }

            /// <summary>
            /// Executes the result delegate.
            /// </summary>
            /// <param name="reader">The reader.</param>
            /// <param name="cancellationToken">A cancellation token which can be used to cancel the entire batch operation.</param>
            /// <returns>
            /// An awaitable task.
            /// </returns>
            protected override Task ExecuteResultDelegate(System.Xml.XmlReader reader, CancellationToken cancellationToken)
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
            public new SqlBatchResult<TResult> Result
            {
                get => (SqlBatchResult<TResult>)base.Result;
                private set => base.Result = value;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="SqlBatchCommand.XmlReader{TResult}" /> class.
            /// </summary>
            /// <param name="batch">The batch that this command belongs to.</param>
            /// <param name="program">The program to execute scalar.</param>
            /// <param name="resultFunc">The result function.</param>
            /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
            public XmlReader(
                [NotNull] SqlBatch batch,
                [NotNull] SqlProgram program,
                [NotNull] XmlResultDelegateAsync<TResult> resultFunc,
                [CanBeNull] SetBatchParametersDelegate setParameters)
                : base(batch, program, setParameters)
            {
                _resultFunc = resultFunc;
                Result = new SqlBatchResult<TResult>(this);
            }

            /// <summary>
            /// Executes the result delegate.
            /// </summary>
            /// <param name="reader">The reader.</param>
            /// <param name="cancellationToken">A cancellation token which can be used to cancel the entire batch operation.</param>
            /// <returns>
            /// An awaitable task.
            /// </returns>
            protected override Task ExecuteResultDelegate(System.Xml.XmlReader reader, CancellationToken cancellationToken)
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