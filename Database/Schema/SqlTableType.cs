#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Copyright (c) 2012, Web Applications UK Ltd
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

using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Database.Schema
{
    /// <summary>
    ///   Extends SqlType to store column information for table types.
    /// </summary>
    public sealed class SqlTableType : SqlType
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="SqlTableType"/> class.
        /// </summary>
        /// <param name="baseType">The base type.</param>
        /// <param name="sqlSchema">The schema name.</param>
        /// <param name="name">The table name.</param>
        /// <param name="defaultSize">The default size information.</param>
        /// <param name="isNullable">
        ///   If set to <see langword="true"/> the value can be <see langword="null"/>.
        /// </param>
        /// <param name="isUserDefined">
        ///   If set to <see langword="true"/> the value is a user defined type.
        /// </param>
        /// <param name="isClr">
        ///   If set to <see langword="true"/> the value is a CLR type.
        /// </param>
        internal SqlTableType(
            [CanBeNull] SqlType baseType,
            [NotNull] SqlSchema sqlSchema,
            [NotNull] string name,
            SqlTypeSize defaultSize,
            bool isNullable,
            bool isUserDefined,
            bool isClr)
            : base(
                baseType,
                sqlSchema,
                name,
                defaultSize,
                isNullable,
                isUserDefined,
                isClr,
                true)
        {
            Contract.Requires(sqlSchema != null);
            Contract.Requires(name != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(name));
        }

        /// <summary>
        ///   Gets the <see cref="SqlTableDefinition"/> associated with this type.
        /// </summary>
        /// <value>The definition for the table.</value>
        [NotNull]
        public SqlTableDefinition TableDefinition { get; internal set; }
    }
}