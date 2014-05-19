#region © Copyright Web Applications (UK) Ltd, 2014.  All rights reserved.
// Copyright (c) 2014, Web Applications UK Ltd
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Web Applications UK Ltd nor the
//       names of its contributors may be used to endorse or promote products
//       derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL WEB APPLICATIONS UK LTD BE LIABLE FOR ANY
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WebApplications.Utilities.Logging;
using WebApplications.Utilities.Logging.Loggers;
using WebApplications.Utilities.Service.PipeProtocol;

namespace WebApplications.Utilities.Service
{
    /// <summary>
    /// Creates a server for a bi-directional pipe.
    /// </summary>
    public partial class NamedPipeServer
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
        [NotNull]
        private Task WriteLogs([NotNull] IEnumerable<Log> logs, CancellationToken token)
        {
            NamedPipeConnection[] connections;
            lock (_connectionLock)
                connections = _namedPipeConnections.ToArray();

            byte[] data = new LogResponse(logs).Serialize();
            return Task.WhenAll(connections.Select(c => c.Send(data, token)));
        }
    }
}