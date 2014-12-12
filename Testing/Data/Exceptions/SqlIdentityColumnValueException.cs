using System;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
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
                return "Cannot insert explicit value for identity column in table '{0}' when IDENTITY_INSERT is set to OFF.";
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlIdentityColumnValueException"/> class.
        /// </summary>
        /// <param name="conId">The con id.</param>
        public SqlIdentityColumnValueException(Guid conId = default(Guid))
            : base(GenerateCollection(56, 1, 16, ErrorText), "9.0.0.0", conId)
        { }

        /// <summary>
        /// Prevents a default instance of the <see cref="SqlIdentityColumnValueException"/> class from being created.
        /// </summary>
        /// <param name="exception">The exception.</param>
        private SqlIdentityColumnValueException([NotNull] SqlException exception)
            : base(exception)
        {
            Contract.Requires(exception != null);
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
