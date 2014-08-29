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
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Database.Schema
{
    /// <summary>
    /// Base class of all entities in a <see cref="DatabaseSchema"/> contained in a <see cref="SqlSchema"/>.
    /// </summary>
    /// <typeparam name="T">This type</typeparam>
    public abstract class DatabaseSchemaEntity<T> : DatabaseEntity<T>
        where T : DatabaseSchemaEntity<T>
    {
        /// <summary>
        /// The SQL schema.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public readonly SqlSchema SqlSchema;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseEntity{T}" /> class.
        /// </summary>
        /// <param name="sqlSchema">The schema.</param>
        /// <param name="name">The name.</param>
        /// <param name="propertyExpressions">The property expressions.</param>
        protected DatabaseSchemaEntity(
            [NotNull] SqlSchema sqlSchema,
            [NotNull] string name,
            [NotNull] params Expression<Func<T, object>>[] propertyExpressions)
            : base(string.Format("{0}.{1}", sqlSchema.FullName, name), propertyExpressions)
        {
            Contract.Requires(sqlSchema != null);
            Contract.Requires(name != null);
            Contract.Requires(propertyExpressions != null);
            SqlSchema = sqlSchema;
        }
    }
}