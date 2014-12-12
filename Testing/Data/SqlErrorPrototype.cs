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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;
using WebApplications.Testing.Annotations;

namespace WebApplications.Testing.Data
{
    /// <summary>
    /// Allows creation of a <see cref="SqlError"/>.
    /// </summary>
    /// <remarks></remarks>
    public class SqlErrorPrototype
    {
        /// <summary>
        /// Function to creates a <see cref="SqlError"/>.
        /// </summary>
        /// <remarks>
        /// This calls the
        /// internal SqlError(int infoNumber, byte errorState, byte errorClass, string server, string errorMessage, string procedure, int lineNumber, uint win32ErrorCode)
        /// constructor.</remarks>
        [NotNull] private static readonly Func<int, byte, byte, string, string, string, int, uint, SqlError>
            _constructor;

        /// <summary>
        /// The equivalent <see cref="SqlError"/>.
        /// </summary>
        [NotNull] public readonly SqlError SqlError;

        /// <summary>
        /// Creates the <see cref="_constructor"/> lambda.
        /// </summary>
        /// <remarks></remarks>
        static SqlErrorPrototype()
        {
            // Find SqlError constructor.
            ConstructorInfo constructorInfo =
                typeof (SqlError).GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance, null,
                    new[]
                        {
                            typeof (int),
                            typeof (byte),
                            typeof (byte),
                            typeof (string),
                            typeof (string),
                            typeof (string),
                            typeof (int),
                            typeof (uint)
                        }, null);
            Contract.Assert(constructorInfo != null);

            // Create parameters
            List<ParameterExpression> parameters = new List<ParameterExpression>(4)
                                                       {
                                                           Expression.Parameter(typeof (int), "infoNumber"),
                                                           Expression.Parameter(typeof (byte), "errorState"),
                                                           Expression.Parameter(typeof (byte), "errorClass"),
                                                           Expression.Parameter(typeof (string), "server"),
                                                           Expression.Parameter(typeof (string), "errorMessage"),
                                                           Expression.Parameter(typeof (string), "procedure"),
                                                           Expression.Parameter(typeof (int), "lineNumber"),
                                                           Expression.Parameter(typeof (uint), "win32ErrorCode")
                                                       };

            // Create lambda expression.
            _constructor = Expression.Lambda<Func<int, byte, byte, string, string, string, int, uint, SqlError>>(
                Expression.New(constructorInfo, parameters), parameters).Compile();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlErrorPrototype" /> class, and in doing so initializes the <see cref="SqlError" /> property.
        /// </summary>
        /// <param name="infoNumber">The info number.</param>
        /// <param name="errorState">State of the error.</param>
        /// <param name="errorClass">The error class.</param>
        /// <param name="server">The server.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="procedure">The procedure.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="win32ErrorCode">The win32 error code (if this error is the first in a <see cref="SqlException">SqlException's</see> collection then
        /// this value will create an <see cref="Exception.InnerException">inner exception</see> of type <see cref="Win32Exception"/>.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <remarks></remarks>
        public SqlErrorPrototype(int infoNumber, byte errorState, byte errorClass = 17,
                                 string server = "Unspecified server", string errorMessage = "Unspecified error",
                                 string procedure = "Unspecified procedure", int lineNumber = 0, uint win32ErrorCode = 0)
            : this(
                _constructor(infoNumber, errorState, errorClass, server, errorMessage, procedure, lineNumber,
                             win32ErrorCode))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlErrorPrototype" /> class.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <remarks></remarks>
        public SqlErrorPrototype([NotNull] SqlError error)
        {
            Contract.Requires(error != null);
            SqlError = error;
        }

        /// <summary>
        /// Gets the name of the provider that generated the error.
        /// </summary>
        /// 
        /// <returns>
        /// The name of the provider that generated the error.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public string Source
        {
            get { return SqlError.Source; }
        }

        /// <summary>
        /// Gets a number that identifies the type of error.
        /// </summary>
        /// 
        /// <returns>
        /// The number that identifies the type of error.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public int Number
        {
            get { return SqlError.Number; }
        }

        /// <summary>
        /// Gets a numeric error code from SQL Server that represents an error, warning or "no data found" message.
        /// </summary>
        /// 
        /// <returns>
        /// The number that represents the error code.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public byte State
        {
            get { return SqlError.State; }
        }

        /// <summary>
        /// Gets the severity level of the error returned from SQL Server.
        /// </summary>
        /// 
        /// <returns>
        /// A value from 1 to 25 that indicates the severity level of the error. The default is 0.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public byte Class
        {
            get { return SqlError.Class; }
        }

        /// <summary>
        /// Gets the name of the instance of SQL Server that generated the error.
        /// </summary>
        /// 
        /// <returns>
        /// The name of the instance of SQL Server.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public string Server
        {
            get { return SqlError.Server; }
        }

        /// <summary>
        /// Gets the text describing the error.
        /// </summary>
        /// 
        /// <returns>
        /// The text describing the error.For more information on errors generated by SQL Server, see SQL Server Books Online.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public string Message
        {
            get { return SqlError.Message; }
        }

        /// <summary>
        /// Gets the name of the stored procedure or remote procedure call (RPC) that generated the error.
        /// </summary>
        /// 
        /// <returns>
        /// The name of the stored procedure or RPC.For more information on errors generated by SQL Server, see SQL Server Books Online.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public string Procedure
        {
            get { return SqlError.Procedure; }
        }

        /// <summary>
        /// Gets the line number within the Transact-SQL command batch or stored procedure that contains the error.
        /// </summary>
        /// 
        /// <returns>
        /// The line number within the Transact-SQL command batch or stored procedure that contains the error.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public int LineNumber
        {
            get { return SqlError.LineNumber; }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return SqlError.ToString();
        }

        /// <summary>
        /// Implicit conversion from <see cref="SqlErrorPrototype"/> to <see cref="SqlError"/>.
        /// </summary>
        /// <param name="prototype">The prototype.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static implicit operator SqlError(SqlErrorPrototype prototype)
        {
            return prototype != null
                       ? prototype.SqlError
                       : null;
        }

        /// <summary>
        /// Implicit conversion from <see cref="SqlError" /> to <see cref="SqlErrorPrototype" />.
        /// </summary>
        /// <param name="sqlError">The SQL error.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static implicit operator SqlErrorPrototype(SqlError sqlError)
        {
            return sqlError != null
                       ? new SqlErrorPrototype(sqlError)
                       : null;
        }
    }
}