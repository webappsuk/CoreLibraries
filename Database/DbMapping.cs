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
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Database.Schema;

namespace WebApplications.Utilities.Database
{
    /// <summary>
    /// Maps a <see cref="SqlProgram"/> to a database object.
    /// </summary>
    public abstract class DbMapping
    {
        /// <summary>
        /// Gets the connection that this mapping belongs to.
        /// </summary>
        /// <value>The connection.</value>
        [NotNull]
        public Connection Connection { get; }

        /// <summary>
        /// Gets the <see cref="SqlProgramParameter">parameter definitions</see> that are used by the <see cref="SqlProgram"/>.
        /// </summary>
        /// <value>The parameters.</value>
        [NotNull]
        [ItemNotNull]
        public IReadOnlyList<SqlProgramParameter> Parameters { get; }

        /// <summary>
        /// Gets the parameter name comparer.
        /// </summary>
        /// <value>The parameter name comparer.</value>
        [NotNull]
        public IEqualityComparer<string> ParameterNameComparer { get; }

        /// <summary>
        /// The parameters by name.
        /// </summary>
        [NotNull]
        private readonly Dictionary<string, SqlProgramParameter> _parametersByName;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbMapping" /> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="parameterNameComparer">The parameter name comparer.</param>
        protected DbMapping(
            [NotNull] Connection connection,
            [NotNull][ItemNotNull] IReadOnlyList<SqlProgramParameter> parameters,
            [NotNull] IEqualityComparer<string> parameterNameComparer)
        {
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            ParameterNameComparer = parameterNameComparer ??
                                    throw new ArgumentNullException(nameof(parameterNameComparer));

            _parametersByName = new Dictionary<string, SqlProgramParameter>(parameters.Count, parameterNameComparer);
            foreach (SqlProgramParameter parameter in parameters)
                _parametersByName.Add(parameter.FullName, parameter);
        }

        /// <summary>
        ///   Tries to get the parameter with the specified name.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="parameter">The retrieved parameter (if found).</param>
        /// <returns>
        ///   Returns <see langword="true"/> if a parameter with the specified <paramref name="parameterName"/> was found;
        ///   otherwise returns <see langword="false"/>.
        /// </returns>
        [ContractAnnotation("=>true, parameter:notnull;=>false, parameter:null")]
        public bool TryGetParameter([NotNull] string parameterName, out SqlProgramParameter parameter)
            => _parametersByName.TryGetValue(parameterName, out parameter);

        /// <summary>
        /// Appends the SQL for executing the program to the <paramref name="builder"/> given.
        /// </summary>
        /// <param name="builder">The builder to append to.</param>
        /// <param name="parameters">The parameters passed to the program.</param>
        public abstract void AppendExecute(
            [NotNull] SqlStringBuilder builder,
            [NotNull] ParametersCollection parameters);
    }
}