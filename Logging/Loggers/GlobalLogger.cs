using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Logging.Loggers
{
    /// <summary>
    /// The <see cref="GlobalLogger"/> gives access to all logs, from all processes, on the current machine.
    /// </summary>
    public sealed class GlobalLogger : LoggerBase
    {
        /// <summary>
        /// The log file path.
        /// </summary>
        [CanBeNull]
        [PublicAPI]
        public static readonly string LogFilePath;

        /// <summary>
        /// The log file name.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public const string LogFileName = @"WebApplications.Utilities.Logging.GlobalLog.log";

        static GlobalLogger()
        {
            try
            {
                // Get the file path to the shared log file, which should be in the common application data folder.
                LogFilePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    LogFileName);

                if (!File.Exists(LogFilePath))
                {
                    
                }
            }
            catch
            {
                LogFilePath = null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalLogger" /> class.
        /// </summary>
        /// <param name="validLevels">The valid levels.</param>
        public GlobalLogger(LoggingLevels validLevels = LoggingLevels.All) :
            base("Global Logger", true, false, validLevels)
        {
        }

        /// <summary>
        /// Adds the specified logs to storage in batches.
        /// </summary>
        /// <param name="logs">The logs to add to storage.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override Task Add(IEnumerable<Log> logs, CancellationToken token = new CancellationToken())
        {
            Contract.Requires(logs != null);
            throw new NotImplementedException();
        }

        public override IQbservable<Log> Qbserve
        {
            get
            {
                return base.Qbserve;
            }
        }
    }
}
