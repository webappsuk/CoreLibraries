#region © Copyright Web Applications (UK) Ltd, 2015.  All rights reserved.
// Copyright (c) 2015, Web Applications UK Ltd
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
using System.Data.SqlClient;
using WebApplications.Testing.Annotations;

namespace WebApplications.Testing.Data.Exceptions
{
    /// <summary>
    /// A SqlException indicating that explicit value can't be inserted in identity column if IDENTITY_INSERT is set to OFF.
    /// </summary>
    public class SqlIdentityColumnValueException : SqlExceptionPrototype
    {
        /// <summary>
        /// Gets the error text.
        /// </summary>
        private static string ErrorText
        {
            get
            {
                return
                    "Cannot insert explicit value for identity column in table '{0}' when IDENTITY_INSERT is set to OFF.";
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlIdentityColumnValueException"/> class.
        /// </summary>
        /// <param name="conId">The con id.</param>
        public SqlIdentityColumnValueException(Guid conId = default(Guid))
            : base(GenerateCollection(56, 1, 16, ErrorText), "9.0.0.0", conId)
        {
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="SqlIdentityColumnValueException"/> class from being created.
        /// </summary>
        /// <param name="exception">The exception.</param>
        private SqlIdentityColumnValueException([NotNull] SqlException exception)
            : base(exception)
        {
        }

        /// <summary>
        /// Implicit conversion from <see cref="SqlIdentityColumnValueException" /> to <see cref="SqlException" />.
        /// </summary>
        /// <param name="prototype">The prototype.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static implicit operator SqlException(SqlIdentityColumnValueException prototype)
        {
            return prototype != null
                ? prototype.SqlException
                : null;
        }

        /// <summary>
        /// Implicit conversion from <see cref="SqlException" /> to <see cref="SqlIdentityColumnValueException" />.
        /// </summary>
        /// <param name="exception">The SQL exception.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static implicit operator SqlIdentityColumnValueException(SqlException exception)
        {
            return exception != null
                ? new SqlIdentityColumnValueException(exception)
                : null;
        }
    }
}