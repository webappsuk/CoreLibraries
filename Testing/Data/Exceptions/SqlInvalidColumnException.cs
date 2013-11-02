using System;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace WebApplications.Testing.Data.Exceptions
{
    /// <summary>
    /// A SqlException indicating the column name invalid.
    /// </summary>
    public class SqlInvalidColumnException : SqlExceptionPrototype
    {
        /// <summary>
        /// Gets the error text.
        /// </summary>
        private static string ErrorText
        {
            get
            {
                return "Invalid column name '{0}'";
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlInvalidColumnException"/> class.
        /// </summary>
        /// <param name="conId">The con id.</param>
        public SqlInvalidColumnException(Guid conId = default(Guid))
            : base(GenerateCollection(207, 1, 16, ErrorText), "9.0.0.0", conId)
        { }

        /// <summary>
        /// Prevents a default instance of the <see cref="SqlInvalidColumnException"/> class from being created.
        /// </summary>
        /// <param name="exception">The exception.</param>
        private SqlInvalidColumnException([NotNull] SqlException exception)
            : base(exception)
        {
            Contract.Requires(exception != null);
        }

        /// <summary>
        /// Implicit conversion from <see cref="SqlInvalidColumnException" /> to <see cref="SqlException" />.
        /// </summary>
        /// <param name="prototype">The prototype.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static implicit operator SqlException(SqlInvalidColumnException prototype)
        {
            return prototype != null
                       ? prototype.SqlException
                       : null;
        }

        /// <summary>
        /// Implicit conversion from <see cref="SqlException" /> to <see cref="SqlInvalidColumnException" />.
        /// </summary>
        /// <param name="exception">The SQL exception.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static implicit operator SqlInvalidColumnException(SqlException exception)
        {
            return exception != null
                       ? new SqlInvalidColumnException(exception)
                       : null;

        }
    }
}
