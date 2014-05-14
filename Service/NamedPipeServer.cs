﻿#region © Copyright Web Applications (UK) Ltd, 2014.  All rights reserved.
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
using System.Linq;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Service
{
    /// <summary>
    /// Creates a server for a bi-directional pipe.
    /// </summary>
    public partial class NamedPipeServer : IDisposable
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
        /// Initializes a new instance of the <see cref="NamedPipeServer" /> class.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="name">The name.</param>
        /// <param name="maximumConnections">The maximum number of connections.</param>
        public NamedPipeServer([NotNull]BaseService service, [NotNull]string name, int maximumConnections = 1)
        {
            Contract.Requires(maximumConnections > 0);
            Service = service;
            Name = name;
            MaximumConnections = maximumConnections;
            // Create a connection, before adding it to the list and starting.
            NamedPipeConnection connection = new NamedPipeConnection(this);
            _namedPipeConnections = new List<NamedPipeConnection>(MaximumConnections) { connection };
            connection.Start();
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
            lock (_namedPipeConnections)
            {
                foreach (NamedPipeConnection connection in _namedPipeConnections)
                    // ReSharper disable once PossibleNullReferenceException
                    connection.Dispose();

                _namedPipeConnections.Clear();
            }
        }
    }
}