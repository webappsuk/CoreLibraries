using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Logging.Loggers
{
    /// <summary>
    /// Allows logging to a database.
    /// </summary>
    public class SqlLogger : LoggerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLogger" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="validLevels">The valid levels.</param>
        public SqlLogger(
            [NotNull]string name,
            [NotNull]string connectionString,
            LoggingLevels validLevels = LoggingLevels.All)
            : base(name, false, true, validLevels)
        {
        }

        public override Task Add(IEnumerable<Log> logs, CancellationToken token = new CancellationToken())
        {
            throw new NotImplementedException();
        }
    }
}
