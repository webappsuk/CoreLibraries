using System;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

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
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidUserOrDatabaseException"/> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public InvalidUserOrDatabaseException([NotNull] SqlException exception) 
            : base(exception)
        {
            Contract.Requires(exception != null);
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