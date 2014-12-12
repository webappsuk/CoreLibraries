using System;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using WebApplications.Testing.Annotations;

namespace WebApplications.Testing.Data.Exceptions
{
    /// <summary>
    /// A SqlException indicating the failure to convert data from one data type to another. 
    /// </summary>
    public class SqlConversionException : SqlExceptionPrototype
    {
        /// <summary>
        /// Gets the error text.
        /// </summary>
        private static string ErrorText
        {
            get
            {
                return
                    "Conversion failed when converting the '{0}' value '{1}' to data type '{2}'.";
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlConversionException"/> class.
        /// </summary>
        /// <param name="conId">The con id.</param>
        public SqlConversionException(Guid conId = default(Guid))
            : base(GenerateCollection(245, 1, 16, ErrorText), "9.0.0.0", conId)
        { }

        /// <summary>
        /// Prevents a default instance of the <see cref="SqlConversionException"/> class from being created.
        /// </summary>
        /// <param name="exception">The exception.</param>
        private SqlConversionException([NotNull] SqlException exception)
            : base(exception)
        {
            Contract.Requires(exception != null);
        }

        /// <summary>
        /// Implicit conversion from <see cref="SqlConversionException" /> to <see cref="SqlException" />.
        /// </summary>
        /// <param name="prototype">The prototype.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static implicit operator SqlException(SqlConversionException prototype)
        {
            return prototype != null
                       ? prototype.SqlException
                       : null;
        }

        /// <summary>
        /// Implicit conversion from <see cref="SqlException" /> to <see cref="SqlConversionException" />.
        /// </summary>
        /// <param name="exception">The SQL exception.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static implicit operator SqlConversionException(SqlException exception)
        {
            return exception != null
                       ? new SqlConversionException(exception)
                       : null;

        }
    }
}
