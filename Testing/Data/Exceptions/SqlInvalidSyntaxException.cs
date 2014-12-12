using System;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using WebApplications.Testing.Annotations;

namespace WebApplications.Testing.Data.Exceptions
{
    /// <summary>
    /// A SqlException indicating the SQL syntax is invalid.
    /// </summary>
    public class SqlInvalidSyntaxException : SqlExceptionPrototype
    {
        /// <summary>
        /// Gets the error text.
        /// </summary>
        private static string ErrorText
        {
            get
            {
                return "Incorrect syntax near the keyword '{0}'";
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlInvalidSyntaxException"/> class.
        /// </summary>
        /// <param name="conId">The con id.</param>
        public SqlInvalidSyntaxException(Guid conId = default(Guid))
            : base(GenerateCollection(156, 1, 15, ErrorText), "9.0.0.0", conId)
        { }

        /// <summary>
        /// Prevents a default instance of the <see cref="SqlInvalidSyntaxException"/> class from being created.
        /// </summary>
        /// <param name="exception">The exception.</param>
        private SqlInvalidSyntaxException([NotNull] SqlException exception)
            : base(exception)
        {
            Contract.Requires(exception != null);
        }

        /// <summary>
        /// Implicit conversion from <see cref="SqlInvalidSyntaxException" /> to <see cref="SqlException" />.
        /// </summary>
        /// <param name="prototype">The prototype.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static implicit operator SqlException(SqlInvalidSyntaxException prototype)
        {
            return prototype != null
                       ? prototype.SqlException
                       : null;
        }

        /// <summary>
        /// Implicit conversion from <see cref="SqlException" /> to <see cref="SqlInvalidSyntaxException" />.
        /// </summary>
        /// <param name="exception">The SQL exception.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static implicit operator SqlInvalidSyntaxException(SqlException exception)
        {
            return exception != null
                       ? new SqlInvalidSyntaxException(exception)
                       : null;

        }
    }
}
