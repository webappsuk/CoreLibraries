#region � Copyright Web Applications (UK) Ltd, 2015.  All rights reserved.
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
    /// A SqlException indicating the user credentials are invalid or the database
    /// specified in the connection string is invalid.
    /// </summary>
    public class InvalidUserOrDatabaseException : SqlExceptionPrototype
    {
        /// <summary>
        /// Gets the error text.
        /// </summary>
        private static string ErrorText
        {
            get
            {
                return "A connection was successfully established with the server, " +
                       "but then an error occurred during the login process. " +
                       "(provider: Shared Memory Provider, error: 0 - No process is on the other end of the pipe.)";
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidUserOrDatabaseException"/> class.
        /// </summary>
        /// <param name="conId">The con id.</param>
        public InvalidUserOrDatabaseException(Guid conId = default(Guid))
            : base(GenerateCollection(-1, 1, 20, ErrorText), "9.0.0.0", conId)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidUserOrDatabaseException"/> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public InvalidUserOrDatabaseException([NotNull] SqlException exception)
            : base(exception)
        {
        }

        /// <summary>
        /// Implicit conversion from <see cref="InvalidUserOrDatabaseException" /> to <see cref="SqlException" />.
        /// </summary>
        /// <param name="prototype">The prototype.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static implicit operator SqlException(InvalidUserOrDatabaseException prototype)
        {
            return prototype != null
                ? prototype.SqlException
                : null;
        }

        /// <summary>
        /// Implicit conversion from <see cref="SqlException" /> to <see cref="InvalidUserOrDatabaseException" />.
        /// </summary>
        /// <param name="exception">The SQL exception.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static implicit operator InvalidUserOrDatabaseException(SqlException exception)
        {
            return exception != null
                ? new InvalidUserOrDatabaseException(exception)
                : null;
        }
    }
}