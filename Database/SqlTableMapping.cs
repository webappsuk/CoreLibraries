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
using System.Diagnostics;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Database.Schema;
using WebApplications.Utilities.Logging;

namespace WebApplications.Utilities.Database
{
    /// <summary>
    /// Maps a <see cref="SqlProgram"/> to a <see cref="SqlTableDefinition"/>.
    /// </summary>
    public class SqlTableMapping : DbMapping
    {
        /// <summary>
        /// Gets the mapping for the <paramref name="program"/> given from the specified <paramref name="schema"/>.
        /// </summary>
        /// <param name="program">The program to get the mapping for.</param>
        /// <param name="connection">The connection the mapping is for.</param>
        /// <param name="schema">The schema to get the mapping from.</param>
        /// <returns>The mapping.</returns>
        internal static SqlTableMapping GetMapping(
            [NotNull] SqlProgram program,
            [NotNull] Connection connection,
            [NotNull] DatabaseSchema schema)
        {
            // Find the program
            if (!schema.TablesByName.TryGetValue(program.Text, out SqlTableDefinition tableDefinition))
                throw new LoggingException(
                    LoggingLevel.Critical,
                    () => Resources.SqlTableMapping_GetMapping_DefinitionNotFound,
                    program.Text,
                    program.Name);
            Debug.Assert(tableDefinition != null);

            if (program.ParameterCount > 0)
                throw new LoggingException(
                    LoggingLevel.Critical,
                    () => Resources.SqlTableMapping_ParametersNotSupported,
                    program.Text,
                    program.Name);

            return new SqlTableMapping(connection, tableDefinition);
        }

        /// <summary>
        /// The underlying <see cref="SqlTableDefinition"/>.
        /// </summary>
        [NotNull]
        public readonly SqlTableDefinition Definition;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbMapping" /> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="definition">The definition.</param>
        public SqlTableMapping([NotNull] Connection connection, SqlTableDefinition definition)
            : base(connection, Array<SqlProgramParameter>.Empty, StringComparer.Ordinal)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
        }

        /// <summary>
        /// Appends the SQL for executing the program to the <paramref name="builder"/> given.
        /// </summary>
        /// <param name="builder">The builder to append to.</param>
        /// <param name="parameters">The parameters passed to the program.</param>
        public override void AppendExecute(SqlStringBuilder builder, ParametersCollection parameters)
        {
            builder
                .Append("SELECT");

            bool first = true;
            foreach (SqlColumn column in Definition.Columns)
            {
                if (first) first = false;
                else builder.AppendLine(",");

                builder
                    .Append('\t')
                    .AppendIdentifier(column.FullName);
            }

            builder
                .AppendLine()
                .Append("FROM\t")
                .AppendIdentifier(Definition.SqlSchema.FullName)
                .Append('.')
                .AppendIdentifier(Definition.Name)
                .AppendLine(";");
        }
    }
}