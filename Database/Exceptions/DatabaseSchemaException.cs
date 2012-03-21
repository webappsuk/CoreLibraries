using System;
using JetBrains.Annotations;
using WebApplications.Utilities.Logging;

namespace WebApplications.Utilities.Database.Exceptions
{
    /// <summary>
    ///   Exceptions thrown during schema parsing.
    /// </summary>
    public class DatabaseSchemaException : LoggingException
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="LoggingException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="parameters">
        ///   The parameters, which are formatted by the <paramref name="message"/> <see cref="string"/>.
        /// </param>
        public DatabaseSchemaException([NotNull] string message, [NotNull] params object[] parameters) : base(message, parameters)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LoggingException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="level">The severity of the exception being logged.</param>
        /// <param name="parameters">
        ///   The parameters, which are formatted by the <paramref name="message"/> <see cref="string"/>.
        /// </param>
        public DatabaseSchemaException([NotNull] string message, LogLevel level, [NotNull] params object[] parameters) : base(message, level, parameters)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LoggingException"/> class.
        /// </summary>
        /// <param name="innerException">
        ///   The exception that occurred during parsing.
        /// </param>
        /// <param name="level">The severity of the exception being logged.</param>
        public DatabaseSchemaException([NotNull] Exception innerException, LogLevel level) : base(innerException, level)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LoggingException"/> class.
        /// </summary>
        /// <param name="innerException">
        ///   The exception that occurred during parsing.
        /// </param>
        /// <param name="message">The exception message.</param>
        /// <param name="parameters">
        ///   The parameters, which are formatted by the <paramref name="message"/> <see cref="string"/>.
        /// </param>
        public DatabaseSchemaException([CanBeNull] Exception innerException, [NotNull] string message, [NotNull] params object[] parameters) : base(innerException, message, parameters)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LoggingException"/> class.
        /// </summary>
        /// <param name="innerException">
        ///   The exception that occurred during parsing.
        /// </param>
        /// <param name="message">The exception message.</param>
        /// <param name="level">The severity of the exception being logged.</param>
        /// <param name="parameters">
        ///   The parameters, which are formatted by the <paramref name="message"/> <see cref="string"/>.
        /// </param>
        public DatabaseSchemaException([CanBeNull] Exception innerException, [NotNull] string message, LogLevel level, [NotNull] params object[] parameters) : base(innerException, message, level, parameters)
        {
        }
    }
}