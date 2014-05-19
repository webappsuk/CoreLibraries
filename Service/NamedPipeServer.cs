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

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
using WebApplications.Utilities.Logging;
using WebApplications.Utilities.Service.PipeProtocol;

namespace WebApplications.Utilities.Service
{
    /// <summary>
    /// Creates a server for a bi-directional pipe.
    /// </summary>
    internal partial class NamedPipeServer : IDisposable
    {
        /// <summary>
        /// The connection lock.
        /// </summary>
        [NotNull]
        private readonly object _connectionLock = new object();

        /// <summary>
        /// The named pipe connections.
        /// </summary>
        [NotNull]
        private readonly List<NamedPipeConnection> _namedPipeConnections;

        /// <summary>
        /// The service.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public readonly BaseService Service;

        /// <summary>
        /// The pipe name.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public readonly string Name;

        /// <summary>
        /// Gets the maximum number of connections.
        /// </summary>
        /// <value>The maximum connections.</value>
        [PublicAPI]
        public readonly int MaximumConnections;

        /// <summary>
        /// Gets the connection count.
        /// </summary>
        /// <value>The connection count.</value>
        [PublicAPI]
        public int ConnectionCount
        {
            get
            {
                lock (_connectionLock)
                    // ReSharper disable once PossibleNullReferenceException
                    return _namedPipeConnections.Count(c => c.State == PipeState.Connected);
            }
        }

        /// <summary>
        /// The input buffer size
        /// </summary>
        [PublicAPI]
        public const int InBufferSize = 16384;

        /// <summary>
        /// The output buffer size
        /// </summary>
        [PublicAPI]
        public const int OutBufferSize = 32768;

        /// <summary>
        /// The pipe security.
        /// </summary>
        [NotNull]
        private readonly PipeSecurity _pipeSecurity;

        private Timer _connectionCheckTimer;

        private readonly TimeSpan _heartbeat;

        private NamedPipeServerLogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedPipeServer" /> class.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="configuration">The configuration.</param>
        /// <exception cref="ServiceException">
        /// </exception>
        public NamedPipeServer(
            [NotNull] BaseService service,
            [NotNull] ServerConfig configuration)
        {
            Contract.Requires<RequiredContractException>(service != null, "Parameter_Null");
            Contract.Requires<RequiredContractException>(configuration != null, "Parameter_Null");
            Contract.Requires<RequiredContractException>(
                configuration.MaximumConnections > 0,
                "NamedPipeServer_MaxConnections");

            Service = service;
            MaximumConnections = configuration.MaximumConnections;
            if (!string.IsNullOrWhiteSpace(configuration.Name))
                Name = configuration.Name;
            else
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(Guid.NewGuid().ToString("D"))
                    .Append('_');
                foreach (char c in service.ServiceName)
                    builder.Append(char.IsLetterOrDigit(c) ? c : '_');
                builder.Append(Common.NameSuffix);
                Name = builder.ToString();
            }

            // Create security context
            try
            {
                WindowsIdentity currentIdentity = WindowsIdentity.GetCurrent();
                if (currentIdentity == null ||
                    currentIdentity.Owner == null)
                    throw new ServiceException(() => ServiceResources.Err_NamedPipeServer_CannotGetCurrentOwner);

                _pipeSecurity = new PipeSecurity();
                _pipeSecurity.AddAccessRule(
                    new PipeAccessRule(
                        configuration.Identity,
                        PipeAccessRights.ReadWrite,
                        AccessControlType.Allow));
                _pipeSecurity.AddAccessRule(
                    new PipeAccessRule(
                        currentIdentity.Owner,
                        PipeAccessRights.FullControl,
                        AccessControlType.Allow));
            }
            catch (Exception exception)
            {
                throw new ServiceException(exception, () => ServiceResources.Err_NamedPipeServer_Fatal_Error_Securing);
            }

            // Check no one has tried to create the pipe before us, combined with the GUID name this makes the most common
            // form of pipe attack (pre-registration) impossible.
            if (File.Exists(@"\\.\pipe\" + Name))
                throw new ServiceException(() => ServiceResources.Err_NamedPipeServer_PipeAlreadyExists);

            // Create a connection, before adding it to the list and starting.
            NamedPipeConnection connection = new NamedPipeConnection(this);
            _namedPipeConnections = new List<NamedPipeConnection>(MaximumConnections) {connection};
            connection.Start();
            _logger = new NamedPipeServerLogger(this);
            Log.AddLogger(_logger);

            _heartbeat = configuration.Heartbeat;
            if (_heartbeat < TimeSpan.Zero) return;

            _connectionCheckTimer = new Timer(CheckConnections);
            _connectionCheckTimer.Change(_heartbeat, Timeout.InfiniteTimeSpan);
        }

        /// <summary>
        /// Checks the connections to ensure we have at least one open, this should never happen but services are long running, and so
        /// if something truly fatal happens this should restore connectivity.
        /// </summary>
        /// <param name="state">The state.</param>
        private void CheckConnections([CanBeNull] object state)
        {
            lock (_connectionLock)
            {
                if (_connectionCheckTimer == null) return;

                if (_namedPipeConnections.Count < MaximumConnections)
                {
                    _connectionCheckTimer.Change(Timeout.Infinite, Timeout.Infinite);

                    if (!_namedPipeConnections.Any(c => c.State == PipeState.Open))
                        Add();
                }

                // Kick off timer again
                _connectionCheckTimer.Change(_heartbeat, Timeout.InfiniteTimeSpan);
            }
        }

        /// <summary>
        /// Removes the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        private void Remove(NamedPipeConnection connection)
        {
            lock (_namedPipeConnections)
                _namedPipeConnections.Remove(connection);
            Add();
        }

        /// <summary>
        /// Starts a new listening connection, if there is capactiy.
        /// </summary>
        private void Add()
        {
            lock (_namedPipeConnections)
            {
                // Sanity check, this will remove any connections that were terminated before they could tell us!
                // ReSharper disable once PossibleNullReferenceException
                _namedPipeConnections.RemoveAll(c => c.State == PipeState.Closed);

                if (_namedPipeConnections.Count >= MaximumConnections) return;

                // Create a new connection.
                NamedPipeConnection connection = new NamedPipeConnection(this);
                _namedPipeConnections.Add(connection);
                // Note we never start the connection until we've added it to the list, to avoid a race condition where
                // it is removed before it is started.
                connection.Start();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Timer timer = Interlocked.Exchange(ref _connectionCheckTimer, null);
            if (timer != null)
            {
                timer.Change(Timeout.Infinite, Timeout.Infinite);
                timer.Dispose();
            }

            NamedPipeServerLogger logger = Interlocked.Exchange(ref _logger, null);
            if (logger != null)
            {
                Log.RemoveLogger(logger);
                logger.Dispose();
            }

            lock (_namedPipeConnections)
            {
                foreach (NamedPipeConnection connection in _namedPipeConnections.ToArray())
                    // ReSharper disable once PossibleNullReferenceException
                    connection.OnDisconnect();

                // Should already be clear, but do anyway.
                _namedPipeConnections.Clear();
            }
        }
    }
}