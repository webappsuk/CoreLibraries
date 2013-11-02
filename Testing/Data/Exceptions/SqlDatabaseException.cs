using System;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace WebApplications.Testing.Data.Exceptions
{
    /// <summary>
    /// A SqlException indicating the database cannot be opened.
    /// </summary>
    public class SqlDatabaseException : SqlExceptionPrototype
    {
        /// <summary>
        /// Gets the error text.
        /// </summary>
        private static string ErrorText
        {
            get
            {
                return "Cannot open database '{0}' requested by the login. The login failed.";
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDatabaseException"/> class.
        /// </summary>
        /// <param name="conId">The con id.</param>
        public SqlDatabaseException(Guid conId = default(Guid))
            : base(GenerateCollection(4060, 1, 11, ErrorText), "9.0.0.0", conId)
        { }

        /// <summary>
        /// Prevents a default instance of the <see cref="SqlDatabaseException"/> class from being created.
        /// </summary>
        /// <param name="exception">The exception.</param>
        private SqlDatabaseException([NotNull] SqlException exception)
            : base(exception)
        {
            Contract.Requires(exception != null);
        }

        /// <summary>
        /// Implicit conversion from <see cref="SqlDatabaseException" /> to <see cref="SqlException" />.
        /// </summary>
        /// <param name="prototype">The prototype.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static implicit operator SqlException(SqlDatabaseException prototype)
        {
            return prototype != null
                       ? prototype.SqlException
                       : null;
        }

        /// <summary>
        /// Implicit conversion from <see cref="SqlException" /> to <see cref="SqlDatabaseException" />.
        /// </summary>
        /// <param name="exception">The SQL exception.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static implicit operator SqlDatabaseException(SqlException exception)
        {
            return exception != null
                       ? new SqlDatabaseException(exception)
                       : null;

        }
    }
}
