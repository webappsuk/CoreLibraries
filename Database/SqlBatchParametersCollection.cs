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
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Database.Exceptions;
using WebApplications.Utilities.Database.Schema;
using WebApplications.Utilities.Logging;

namespace WebApplications.Utilities.Database
{
    /// <summary>
    /// A delegate for setting the parameters to pass to a batch for a single program.
    /// </summary>
    public delegate void SetBatchParametersDelegate(SqlBatchParametersCollection parameters);

    /// <summary>
    /// Holds the collection of parameters to a <see cref="SqlBatchCommand"/>.
    /// </summary>
    public partial class SqlBatchParametersCollection
    {
        [NotNull]
        private readonly SqlBatchCommand _command;

        private readonly ushort _commandIndex;

        [NotNull]
        private readonly SqlProgramMapping _mapping;

        [NotNull]
        private readonly List<DbBatchParameter> _parameters = new List<DbBatchParameter>();

        [CanBeNull]
        private List<(DbBatchParameter, IOut)> _outputParameters;

        /// <summary>
        /// Gets the parameters in the collection.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        [NotNull]
        [ItemNotNull]
        public IReadOnlyList<DbBatchParameter> Parameters => _parameters;

        /// <summary>
        /// Gets the return value parameter.
        /// </summary>
        /// <value>
        /// The return value parameter.
        /// </value>
        [CanBeNull]
        public DbBatchParameter ReturnValueParameter { get; private set; }

        /// <summary>
        /// Gets the output parameters in the collection.
        /// </summary>
        /// <value>
        /// The output parameters.
        /// </value>
        [CanBeNull]
        internal IReadOnlyList<(DbBatchParameter, IOut)> OutputParameters => _outputParameters;

        /// <summary>
        /// Gets the program mapping.
        /// </summary>
        /// <value>The mapping.</value>
        [NotNull]
        internal SqlProgramMapping Mapping => _mapping;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlBatchParametersCollection" /> class.
        /// </summary>
        /// <param name="mapping">The mapping.</param>
        /// <param name="command">The command.</param>
        /// <param name="commandIndex">Index of the command.</param>
        internal SqlBatchParametersCollection([NotNull] SqlProgramMapping mapping, [NotNull] SqlBatchCommand command, ushort commandIndex)
        {
            _mapping = mapping;
            _command = command;
            _commandIndex = commandIndex;
        }

        /// <summary>
        /// Gets the batch parameter for the program parameter given, creating it if it doesn't already exist.
        /// </summary>
        /// <param name="programParameter">The program parameter.</param>
        /// <returns>The batch parameter.</returns>
        [NotNull]
        private DbBatchParameter GetOrAddParameter([NotNull] SqlProgramParameter programParameter)
        {
            IEqualityComparer<string> comparer = _mapping.Definition.ParameterNameComparer;
            int index = _parameters.FindIndex(p => comparer.Equals(p.ParameterName, programParameter.FullName));
            if (index >= 0)
                return _parameters[index];

            if (_parameters.Count >= ushort.MaxValue)
                throw new InvalidOperationException(Resources.SqlBatchParametersCollection_GetOrAddParameter_OnlyAllowed65536);

            SqlBatchParameter batchParameter = new SqlBatchParameter(
                programParameter,
                programParameter.CreateSqlParameter(),
                "_" + _commandIndex.ToString("X4") + _parameters.Count.ToString("X4"));
            _parameters.Add(batchParameter);

            if (programParameter.Direction == ParameterDirection.ReturnValue)
            {
                if (ReturnValueParameter != null)
                {
                    throw new LoggingException(
                        LoggingLevel.Error,
                        () => Resources.SqlBatchParametersCollection_GetOrAddParameter_MultipleReturns,
                        _command.Program.Name);
                }

                ReturnValueParameter = batchParameter;
            }

            return batchParameter;
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
        public DbParameter SetParameter<T>(
            [NotNull] string parameterName,
            Input<T> value,
            TypeConstraintMode mode = TypeConstraintMode.Warn)
        {
            if (parameterName == null) throw new ArgumentNullException(nameof(parameterName));

            // Find parameter definition
            SqlProgramParameter parameterDefinition;
            if (!_mapping.Definition.TryGetParameter(parameterName, out parameterDefinition))
                throw new LoggingException(
                    LoggingLevel.Critical,
                    () => Resources.SqlProgramCommand_SetParameter_ProgramDoesNotHaveParameter,
                    _command.Program.Name,
                    parameterName);
            Debug.Assert(parameterDefinition != null);

            lock (_parameters)
            {
                // Find or create SQL Parameter.
                DbBatchParameter parameter = GetOrAddParameter(parameterDefinition);

                Debug.Assert(parameter != null);
                parameter.SetParameterValue(parameterDefinition, value, mode);
                AddOutParameter(parameter, value.Value as IOut);
                return parameter;
            }
        }

        /// <summary>
        /// Adds the output parameter value given if it isn't null.
        /// </summary>
        /// <param name="parameter">The sql parameter the value is for.</param>
        /// <param name="outValue">The output value.</param>
        private void AddOutParameter(
            DbBatchParameter parameter,
            IOut outValue)
        {
            if (outValue == null) return;
            if (_outputParameters == null)
                _outputParameters = new List<(DbBatchParameter, IOut)>();
            _outputParameters.Add((parameter, outValue));
        }
    }
}