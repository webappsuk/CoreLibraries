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
using System.Diagnostics;
using System.Linq;
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
        /// Initializes a new instance of the <see cref="SqlBatchCommand" /> class.
        /// </summary>
        /// <param name="batch">The batch that this command belongs to.</param>
        /// <param name="program">The program to execute.</param>
        /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
        internal SqlBatchCommand(
            [NotNull] SqlBatch batch,
            [NotNull] SqlProgram program,
            [CanBeNull] SetBatchParametersDelegate setParameters)
        {
            Batch = batch;
            Program = program;
            _setParameters = setParameters;
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
                    .Append(' ')
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
                : base(batch, program, setParameters)
            {
                Result = new SqlBatchResult<TResult>(this);
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
                : base(batch, program, setParameters)
            {
                Result = new SqlBatchResult<int>(this);
            }
        }

        /// <summary>
        /// Command for calling ExecuteReader on a program in a batch.
        /// </summary>
        /// <seealso cref="WebApplications.Utilities.Database.SqlBatchCommand" />
        internal class Reader : SqlBatchCommand
        {
            [CanBeNull]
            private readonly ResultDelegateAsync _resultAction;

            private readonly CommandBehavior _behavior;

            /// <summary>
            /// Initializes a new instance of the <see cref="SqlBatchCommand.Reader" /> class.
            /// </summary>
            /// <param name="batch">The batch that this command belongs to.</param>
            /// <param name="program">The program to execute scalar.</param>
            /// <param name="resultAction">The result function.</param>
            /// <param name="behavior">The behavior.</param>
            /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
            public Reader(
                [NotNull] SqlBatch batch,
                [NotNull] SqlProgram program,
                [NotNull] ResultDelegateAsync resultAction,
                CommandBehavior behavior,
                [CanBeNull] SetBatchParametersDelegate setParameters)
                : base(batch, program, setParameters)
            {
                _resultAction = resultAction;
                _behavior = behavior;
                Result = new SqlBatchResult(this);
            }
        }

        /// <summary>
        /// Command for calling ExecuteReader on a program in a batch and returning a value.
        /// </summary>
        /// <seealso cref="WebApplications.Utilities.Database.SqlBatchCommand" />
        // TODO This should be based on Reader OR there should be a common base class
        internal class Reader<TResult> : SqlBatchCommand
        {
            [CanBeNull]
            private readonly ResultDelegateAsync<TResult> _resultFunc;

            private readonly CommandBehavior _behavior;

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
            /// <param name="behavior">The behavior.</param>
            /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
            public Reader(
                [NotNull] SqlBatch batch,
                [NotNull] SqlProgram program,
                [NotNull] ResultDelegateAsync<TResult> resultFunc,
                CommandBehavior behavior,
                [CanBeNull] SetBatchParametersDelegate setParameters)
                : base(batch, program, setParameters)
            {
                _resultFunc = resultFunc;
                _behavior = behavior;
                Result = new SqlBatchResult<TResult>(this);
            }
        }
    }
}