using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WebApplications.Utilities.Logging;
using WebApplications.Utilities.Logging.Interfaces;
using WebApplications.Utilities.Logging.Loggers;
using WebApplications.Utilities.Service.PipeProtocol;

namespace WebApplications.Utilities.Service
{
    /// <summary>
    /// Creates a server for a bi-directional pipe.
    /// </summary>
    public partial class NamedPipeServer : IDisposable
    {
        /// <summary>
        /// Class NamedPipeServerLogger.
        /// </summary>
        private class NamedPipeServerLogger : LoggerBase
        {
            private readonly NamedPipeServer _server;

            /// <summary>
            /// Initializes a new instance of the <see cref="NamedPipeServerLogger"/> class.
            /// </summary>
            /// <param name="server">The server.</param>
            public NamedPipeServerLogger([NotNull] NamedPipeServer server)
                : base(server.Service.ServiceName, false, true, LoggingLevels.All)
            {
                _server = server;
            }

            /// <summary>
            /// Adds the specified logs to storage in batches.
            /// </summary>
            /// <param name="logs">The logs to add to storage.</param>
            /// <param name="token">The token.</param>
            /// <returns>Task.</returns>
            public override Task Add(IEnumerable<Log> logs, CancellationToken token = new CancellationToken())
            {
                return _server.WriteLogs(logs, token);
            }
        }

        /// <summary>
        /// Writes the logs to all connections.
        /// </summary>
        /// <param name="logs">The logs.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task.</returns>
        private Task WriteLogs(IEnumerable<Log> logs, CancellationToken token)
        {
            NamedPipeConnection[] connections;
            lock (_connectionLock)
                connections = _namedPipeConnections.ToArray();

            // TODO Add support for serializing IEnumerable<Log>
            var log = logs.FirstOrDefault();
            if (log == null) return TaskResult.Completed;

            byte[] data = new LogResponse(log).Serialize();
            return Task.WhenAll(connections.Select(c => c.Send(data, token)));
        }
    }
}
