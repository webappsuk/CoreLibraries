#region © Copyright Web Applications (UK) Ltd, 2014.  All rights reserved.
// Copyright (c) 2014, Web Applications UK Ltd
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

namespace WebApplications.Utilities.Database.Schema
{
    /// <summary>
    /// Interface for a Database schema.
    /// </summary>
    public interface ISchema : IEquatable<ISchema>
    {
        /// <summary>
        /// Holds all the SQL schemas (<see cref="SqlSchema"/>, using the <see cref="SqlSchema.ID"/> as the key.
        /// </summary>
        [PublicAPI]
        [NotNull]
        IReadOnlyDictionary<int, SqlSchema> SchemasByID { get; }

        /// <summary>
        ///   Holds all the program definitions (<see cref="SqlProgramDefinition"/>) for the schema.
        /// </summary>
        [PublicAPI]
        [NotNull]
        IReadOnlyDictionary<string, SqlProgramDefinition> ProgramDefinitionsByName { get; }

        /// <summary>
        ///   Holds all the table and view definitions (<see cref="SqlTableDefinition"/>) for the schema.
        /// </summary>
        [PublicAPI]
        [NotNull]
        IReadOnlyDictionary<string, SqlTableDefinition> TablesByName { get; }

        /// <summary>
        ///   Holds all the types for the schema, which are stored with the <see cref="SqlType.FullName">full
        ///   name</see> as the key and the <see cref="SqlType"/> as the value.
        /// </summary>
        [PublicAPI]
        [NotNull]
        IReadOnlyDictionary<string, SqlType> TypesByName { get; }

        /// <summary>
        ///   Gets the SQL schemas that were loaded from the database.
        /// </summary>
        /// <value>
        ///   An enumerable containing the schema names in ascended order.
        /// </value>
        [PublicAPI]
        [NotNull]
        IEnumerable<SqlSchema> Schemas { get; }

        /// <summary>
        ///   Gets the SQL types from the schema.
        /// </summary>
        /// <value>
        ///   The <see cref="SqlType">type</see>.
        /// </value>
        [PublicAPI]
        [NotNull]
        IEnumerable<SqlType> Types { get; }

        /// <summary>
        ///   Gets the program definitions.
        /// </summary>
        /// <value>The program definitions.</value>
        [PublicAPI]
        [NotNull]
        IEnumerable<SqlProgramDefinition> ProgramDefinitions { get; }

        /// <summary>
        ///   Gets the table and view definitions.
        /// </summary>
        /// <value>The table and view definitions.</value>
        [PublicAPI]
        [NotNull]
        IEnumerable<SqlTableDefinition> Tables { get; }

        /// <summary>
        ///   Unique identity of the schema.
        /// </summary>
        [PublicAPI]
        Guid Guid { get; }
    }
}