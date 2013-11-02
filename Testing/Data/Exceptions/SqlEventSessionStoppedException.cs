using System;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace WebApplications.Testing.Data.Exceptions
{
    /// <summary>
    /// A SqlException indicating the event session has been stopped.
    /// </summary>
    public class SqlEventSessionStoppedException : SqlExceptionPrototype
    {
        /// <summary>
        /// Gets the error text.
        /// </summary>
        private static string ErrorText
        {
            get { return "The event session has already been stopped."; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlEventSessionStoppedException"/> class.
        /// </summary>
        /// <param name="conId">The con id.</param>
        public SqlEventSessionStoppedException(Guid conId = default(Guid))
            : base(GenerateCollection(25704, 1, 16, ErrorText), "9.0.0.0", conId)
        { }

        /// <summary>
        /// Prevents a default instance of the <see cref="SqlEventSessionStoppedException"/> class from being created.
        /// </summary>
        /// <param name="exception">The exception.</param>
        private SqlEventSessionStoppedException([NotNull] SqlException exception)
            : base(exception)
        {
            Contract.Requires(exception != null);
        }

        /// <summary>
        /// Implicit conversion from <see cref="SqlEventSessionStoppedException" /> to <see cref="SqlException" />.
        /// </summary>
        /// <param name="prototype">The prototype.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static implicit operator SqlException(SqlEventSessionStoppedException prototype)
        {
            return prototype != null
                       ? prototype.SqlException
                       : null;
        }

        /// <summary>
        /// Implicit conversion from <see cref="SqlException" /> to <see cref="SqlEventSessionStoppedException" />.
        /// </summary>
        /// <param name="exception">The SQL exception.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static implicit operator SqlEventSessionStoppedException(SqlException exception)
        {
            return exception != null
                       ? new SqlEventSessionStoppedException(exception)
                       : null;

        }
    }
}
