using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace WebApplications.Testing.Data
{
    /// <summary>
    /// The exception that is thrown when SQL Server returns a warning or error. This class cannot be inherited.
    /// </summary>
    /// <filterpriority>1</filterpriority>
    [Serializable]
    public sealed class SqlExceptionPrototype
    {
        /// <summary>
        /// Function to creates a <see cref="SqlError"/>.
        /// </summary>
        /// <remarks>
        /// This calls the
        /// internal static SqlException CreateException(SqlErrorCollection errorCollection, string serverVersion, Guid conId)
        /// constructor.</remarks>
        [NotNull]
        private static readonly Func<SqlErrorCollection, string, Guid, SqlException> _constructor;

        /// <summary>
        /// Creates the <see cref="_constructor"/> lambda.
        /// </summary>
        /// <remarks></remarks>
        static SqlExceptionPrototype()
        {
            // Find SqlError constructor (note we don't get about the overload that accepts uint win32ErrorCode
            MethodInfo methodInfo =
                typeof (SqlException).GetMethod("CreateException",
                                                BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod,
                                                null,
                                                new[]
                                                    {
                                                        typeof (SqlErrorCollection),
                                                        typeof (string),
                                                        typeof (Guid)
                                                    }, null);
            Contract.Assert(methodInfo != null);

            // Create parameters
            List<ParameterExpression> parameters = new List<ParameterExpression>(4)
                                                       {
                                                           Expression.Parameter(typeof (SqlErrorCollection),
                                                                                "errorCollection"),
                                                           Expression.Parameter(typeof (string), "serverVersion"),
                                                           Expression.Parameter(typeof (Guid), "conId")
                                                       };

            // Create lambda expression.
            _constructor = Expression.Lambda<Func<SqlErrorCollection, string, Guid, SqlException>>(
                Expression.Call(methodInfo, parameters), parameters).Compile();
        }

        /// <summary>
        /// The equivalent <see cref="SqlException"/>.
        /// </summary>
        [NotNull]
        public readonly SqlException SqlException;

        /// <summary>
        /// Gets a collection of one or more <see cref="T:System.Data.SqlClient.SqlError"/> objects that give detailed information about exceptions generated by the .NET Framework Data Provider for SQL Server.
        /// </summary>
        /// 
        /// <returns>
        /// The collected instances of the <see cref="T:System.Data.SqlClient.SqlError"/> class.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public SqlErrorCollection Errors
        {
            get { return SqlException.Errors; }
        }

        /// <summary>
        /// Gets the severity level of the error returned from the .NET Framework Data Provider for SQL Server.
        /// </summary>
        /// 
        /// <returns>
        /// A value from 1 to 25 that indicates the severity level of the error.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public byte Class
        {
            get { return SqlException.Class; }
        }

        /// <summary>
        /// Gets the line number within the Transact-SQL command batch or stored procedure that generated the error.
        /// </summary>
        /// 
        /// <returns>
        /// The line number within the Transact-SQL command batch or stored procedure that generated the error.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public int LineNumber
        {
            get
            {
                return SqlException.LineNumber;
            }
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
            get
            {
                return SqlException.Number;
            }
        }

        /// <summary>
        /// Gets the name of the stored procedure or remote procedure call (RPC) that generated the error.
        /// </summary>
        /// 
        /// <returns>
        /// The name of the stored procedure or RPC.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public string Procedure
        {
            get
            {
                return SqlException.Procedure;
            }
        }

        /// <summary>
        /// Gets the name of the computer that is running an instance of SQL Server that generated the error.
        /// </summary>
        /// 
        /// <returns>
        /// The name of the computer running an instance of SQL Server.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public string Server
        {
            get
            {
                return SqlException.Server;
            }
        }

        /// <summary>
        /// Gets a numeric error code from SQL Server that represents an error, warning or "no data found" message. For more information about how to decode these values, see SQL Server Books Online.
        /// </summary>
        /// 
        /// <returns>
        /// The number representing the error code.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public byte State
        {
            get
            {
                return SqlException.State;
            }
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
            get
            {
                return SqlException.Source;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlExceptionPrototype" /> class.
        /// </summary>
        /// <param name="errorCollection">The error collection.</param>
        /// <param name="serverVersion">The server version.</param>
        /// <param name="conId">The connection id.</param>
        /// <remarks></remarks>
        public SqlExceptionPrototype(SqlErrorCollection errorCollection, string serverVersion = null, Guid conId = default(Guid))
            : this(_constructor(errorCollection, serverVersion, conId))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlExceptionPrototype" /> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <remarks></remarks>
        public SqlExceptionPrototype([NotNull]SqlException exception)
        {
            Contract.Assert(exception != null);
            SqlException = exception;
        }

        /// <summary>
        /// Returns a string that represents the current <see cref="T:System.Data.SqlClient.SqlException"/> object, and includes the client connection ID (for more information, see <see cref="P:System.Data.SqlClient.SqlException.ClientConnectionId"/>).
        /// </summary>
        /// 
        /// <returns>
        /// A string that represents the current <see cref="T:System.Data.SqlClient.SqlException"/> object.<see cref="T:System.String"/>.
        /// </returns>
        public override string ToString()
        {
            return SqlException.ToString();
        }

        /// <summary>
        /// Implicit conversion from <see cref="SqlExceptionPrototype" /> to <see cref="SqlException" />.
        /// </summary>
        /// <param name="prototype">The prototype.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static implicit operator SqlException(SqlExceptionPrototype prototype)
        {
            return prototype != null
                       ? prototype.SqlException
                       : null;
        }

        /// <summary>
        /// Implicit conversion from <see cref="SqlException" /> to <see cref="SqlExceptionPrototype" />.
        /// </summary>
        /// <param name="exception">The SQL exception.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static implicit operator SqlExceptionPrototype(SqlException exception)
        {
            return exception != null
                       ? new SqlExceptionPrototype(exception)
                       : null;
        }
    }
}