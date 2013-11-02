using System;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace WebApplications.Testing.Data.Exceptions
{
    /// <summary>
    /// A SqlException indicating the SQL object is invalid.
    /// </summary>
    public class SqlInvalidObjectException : SqlExceptionPrototype
    {
        /// <summary>
        /// Gets the error text.
        /// </summary>
        private static string ErrorText
        {
            get
            {
                return "Invalid object name '{0}'";
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlInvalidObjectException"/> class.
        /// </summary>
        /// <param name="conId">The con id.</param>
        public SqlInvalidObjectException(Guid conId = default(Guid))
            : base(GenerateCollection(208, 1, 16, ErrorText), "9.0.0.0", conId)
        { }

        /// <summary>
        /// Prevents a default instance of the <see cref="SqlInvalidObjectException"/> class from being created.
        /// </summary>
        /// <param name="exception">The exception.</param>
        private SqlInvalidObjectException([NotNull] SqlException exception)
            : base(exception)
        {
            Contract.Requires(exception != null);
        }

        /// <summary>
        /// Implicit conversion from <see cref="SqlInvalidObjectException" /> to <see cref="SqlException" />.
        /// </summary>
        /// <param name="prototype">The prototype.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static implicit operator SqlException(SqlInvalidObjectException prototype)
        {
            return prototype != null
                       ? prototype.SqlException
                       : null;
        }

        /// <summary>
        /// Implicit conversion from <see cref="SqlException" /> to <see cref="SqlInvalidObjectException" />.
        /// </summary>
        /// <param name="exception">The SQL exception.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static implicit operator SqlInvalidObjectException(SqlException exception)
        {
            return exception != null
                       ? new SqlInvalidObjectException(exception)
                       : null;

        }
    }
}
