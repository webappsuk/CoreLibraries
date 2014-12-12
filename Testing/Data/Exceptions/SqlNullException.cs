using System;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using WebApplications.Testing.Annotations;

namespace WebApplications.Testing.Data.Exceptions
{
    /// <summary>
    /// A SqlException indicating the 'NULL' value cannot be inserted in a 'NOT NULL' column. 
    /// </summary>
    public class SqlNullException : SqlExceptionPrototype
    {
        /// <summary>
        /// Gets the error text.
        /// </summary>
        private static string ErrorText
        {
            get
            {
                return
                    "Cannot insert the value NULL into column '{0}', table '{1}'; column does not allow nulls. '{2}' fails.";
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlNullException"/> class.
        /// </summary>
        /// <param name="conId">The con id.</param>
        public SqlNullException(Guid conId = default(Guid))
            : base(GenerateCollection(515, 1, 16, ErrorText), "9.0.0.0", conId)
        { }

        /// <summary>
        /// Prevents a default instance of the <see cref="SqlNullException"/> class from being created.
        /// </summary>
        /// <param name="exception">The exception.</param>
        private SqlNullException([NotNull] SqlException exception)
            : base(exception)
        {
            Contract.Requires(exception != null);
        }

        /// <summary>
        /// Implicit conversion from <see cref="SqlNullException" /> to <see cref="SqlException" />.
        /// </summary>
        /// <param name="prototype">The prototype.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static implicit operator SqlException(SqlNullException prototype)
        {
            return prototype != null
                       ? prototype.SqlException
                       : null;
        }

        /// <summary>
        /// Implicit conversion from <see cref="SqlException" /> to <see cref="SqlNullException" />.
        /// </summary>
        /// <param name="exception">The SQL exception.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static implicit operator SqlNullException(SqlException exception)
        {
            return exception != null
                       ? new SqlNullException(exception)
                       : null;

        }
    }
}
