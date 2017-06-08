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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Database.Schema;

namespace WebApplications.Utilities.Database
{
    /// <summary>
    /// Maps a <see cref="SqlProgram"/> to text.
    /// </summary>
    [PublicAPI]
    public class SqlTextMapping : DbMapping
    {
        /// <summary>
        /// Gets the mapping for the <paramref name="program" /> given from the specified <paramref name="schema" />.
        /// </summary>
        /// <param name="program">The program to get the mapping for.</param>
        /// <param name="connection">The connection the mapping is for.</param>
        /// <param name="schema">The schema to get the mapping from.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The mapping.
        /// </returns>
        internal static async Task<SqlTextMapping> GetMapping(
            [NotNull] SqlProgram program,
            [NotNull] Connection connection,
            [NotNull] DatabaseSchema schema,
            CancellationToken cancellationToken)
        {
            IReadOnlyList<SqlProgramParameter> parameters = await schema
                .ValidateTextAsync(program.Text, program.Parameters, cancellationToken)
                .ConfigureAwait(false);

            return new SqlTextMapping(connection, program.Text, parameters, schema.ServerCollation);
        }

        /// <summary>
        ///   The underlying <see cref="SqlProgramDefinition">program definition</see>.
        /// </summary>
        [NotNull]
        public readonly string Text;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlProgramMapping" /> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="text">The text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="parameterNameComparer">The parameter name comparer.</param>
        private SqlTextMapping(
            [NotNull] Connection connection,
            [NotNull] string text,
            [NotNull] IReadOnlyList<SqlProgramParameter> parameters,
            [NotNull] IEqualityComparer<string> parameterNameComparer)
            : base(connection, parameters, parameterNameComparer)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }

        /// <summary>
        /// Appends the text for executing the program to the <paramref name="builder"/> given.
        /// </summary>
        /// <param name="builder">The builder to append to.</param>
        /// <param name="parameters">The parameters passed to the program.</param>
        public override void AppendExecute(SqlStringBuilder builder, ParametersCollection parameters)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));

            builder.Append("EXECUTE ");

            // If there is a return value parameter, need to assign the result to it
            if (parameters.ReturnValueParameter != null)
            {
                builder
                    .Append(parameters.ReturnValueParameter.BaseParameter.ParameterName)
                    .Append(" = ");
            }

            builder
                .Append("[sys].[sp_executesql] ")
                .AppendNVarChar(Text);

            // If there are no parameters, just return now.
            if (parameters.Parameters.Count < 1)
            {
                builder.AppendLine(";");
                return;
            }

            string GetParameterDefinition(DbBatchParameter p)
            {
                if (p.ProgramParameter.ExactSize)
                    return p.ProgramParameter.ToString();

                short maximumLength;
                if (p.Size > short.MaxValue)
                    maximumLength = -1;

                // Despite what the description of the property says, Size is the length of the string in chars for unicode strings,
                // so need to double it as SqlTypeSize expects byte length
                else if (p.DbType == DbType.String || p.DbType == DbType.StringFixedLength)
                    maximumLength = p.Size > short.MaxValue / 2 ? (short)-1 : (short)(p.Size * 2);
                else
                    maximumLength = (short)p.Size;
                
                return p.ProgramParameter.ToString(
                    // Need to calculate the actual size, as the size for Text parameters is not always exactly known
                    new SqlTypeSize(
                        maximumLength,
                        p.Precision,
                        p.Scale));
            }

            builder
                .AppendLine(",")
                .Append("\t")

                // Append the definition of the parameters.
                .AppendNVarChar(
                    string.Join(
                        ",\r\n\t",
                        parameters.Parameters
                            .Where(p => p.Direction != ParameterDirection.ReturnValue)
                            .Select(GetParameterDefinition)));

            foreach (DbBatchParameter parameter in parameters.Parameters)
            {
                // Already dealt with return value parameter
                if (parameter.Direction == ParameterDirection.ReturnValue)
                    continue;

                builder
                    .AppendLine(",")
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
    }
}