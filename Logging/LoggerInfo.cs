using System.Diagnostics.Contracts;
using JetBrains.Annotations;
using WebApplications.Utilities.Logging.Interfaces;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.Logging
{
    /// <summary>
    /// Holds information about loggers in the system.
    /// </summary>
    internal class LoggerInfo
    {
        /// <summary>
        /// Whether the logger came from the configuration.
        /// </summary>
        public readonly bool IsFromConfiguration;

        /// <summary>
        /// The asynchronous lock controls access to the logger.
        /// </summary>
        [NotNull]
        public readonly AsyncLock Lock = new AsyncLock();

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerInfo" /> class.
        /// </summary>
        /// <param name="isFromConfiguration">if set to <see langword="true" /> [is from configuration].</param>
        internal LoggerInfo(bool isFromConfiguration)
        {
            IsFromConfiguration = isFromConfiguration;
        }
    }
}